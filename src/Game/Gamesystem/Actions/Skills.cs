///////////////////////////////////////////////////////////////////////////
// DarkEmu: Attack Actions
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        #region Skill Main

        
        void Player_Trace(int targetid)
        {
            try
            {
                if (Character.Action.Target != 0)
                {
                    obj monster = GetObject(Character.Action.Target);
                    if (monster == null) return;
                    double distance = Formule.gamedistance(Character.Position.x, Character.Position.y, (float)monster.x, (float)monster.y);

                    if (distance >= 2)
                    {
                        Character.Position.wX = (float)monster.x - Character.Position.x;// -Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                        Character.Position.wY = (float)monster.y - Character.Position.y;// -Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                        if (!Character.InGame) return;
                        Send(Packet.Movement(new DarkEmu_GameServer.Global.vektor(Character.Information.UniqueID,
                                    (float)Formule.packetx((float)monster.x, monster.xSec),
                                    (float)Character.Position.z,
                                    (float)Formule.packety((float)(float)monster.y, monster.ySec),
                                    Character.Position.xSec,
                                    Character.Position.ySec)));

                        Character.Position.Time = (distance / (Character.Speed.RunSpeed * 0.0768)) * 1000.0;
                        Character.Position.RecordedTime = Character.Position.Time;

                        StartMovementTimer((int)(Character.Position.Time * 0.1));
                        return;

                    }
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        void SkillMain(byte type, PacketReader Reader)
        {
            try
            {
                if (!SkillGetOpened(Character.Action.UsingSkillID)) return;
                client.Send(Packet.ActionState(1, 1));
                //Console.WriteLine("Skill id: {0}", Data.SkillBase[Character.Action.UsingSkillID].Series); // Dev Mode
                switch (type)
                {
                    case 1:
                        if (Character.Action.sAttack) return;
                        if (Character.Action.sCasting) return;
                        if (Character.Action.nAttack) StopAttackTimer();

                        if (!Base.Skill.CheckWeapon(Character.Information.Item.wID, Character.Action.UsingSkillID))
                        {
                            client.Send(Packet.IngameMessages(SERVER_ACTION_DATA, IngameMessages.UIIT_SKILL_USE_FAIL_WRONGWEAPON));
                            client.Send(Packet.IngameMessages2(SERVER_ACTIONSTATE, IngameMessages.UIIT_SKILL_USE_FAIL_WRONGWEAPON));
                            return;
                        }

                        Character.Action.Target = Reader.Int32();
                        Character.Action.Skill.MainSkill = Character.Action.UsingSkillID;
                        Character.Action.UsingSkillID = 0;
                        Character.Action.Object = GetObjects(Character.Action.Target);

                        if (Data.SkillBase[Character.Action.Skill.MainSkill].isAttackSkill)
                        {
                            Character.Action.Skill = Base.Skill.Info(Character.Action.Skill.MainSkill, Character);
                            if (!Character.Action.Skill.canUse || Character.Action.Target == Character.Information.UniqueID) return;

                            obj o = null;
                            if (Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "DarkEmu_GameServer.obj")
                            {
                                o = Character.Action.Object as obj;
                                if (o.Agro == null) o.Agro = new List<_agro>();
                                if (Character.Action.Skill.OzelEffect == 5 && o.State != 4) return;
                                if (o.State == 4 && Character.Action.Skill.OzelEffect != 5) return;
                            }

                            if (o == null && Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "DarkEmu_GameServer.Systems")
                            {
                                if (!Character.Information.PvP || Character.State.Die) return;
                                Systems sys = Character.Action.Object as Systems;
                                if (Character.Action.Skill.OzelEffect == 5 && sys.Character.State.LastState != 5) return;
                                if (sys.Character.State.LastState == 4 && Character.Action.Skill.OzelEffect != 5) return;
                            }

                            Character.Action.sAttack = true;
                            ActionSkillAttack();

                            Reader.Close();
                        }
                        else
                        {
                            Character.Action.sAttack = true;
                            ActionSkill();
                        }
                        break;
                    case 0:
                        SkillBuff();
                        break;
                    case 2:
                        MovementSkill(Reader);
                        break;
                    default:
                        Console.WriteLine("Skillmain type: {0}", type);
                        break;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }

        #endregion

        #region Buff

        void SkillBuff()
        {
            try
            {
                if (SkillGetSameBuff(Character.Action.UsingSkillID))
                    return;

                if (Character.Speed.DefaultSpeed != Character.Speed.RunSpeed && Data.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("hste"))
                    return;
                else if (Character.Speed.DefaultSpeed + Character.Speed.Updateded[20] != Character.Speed.RunSpeed && Data.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("hste") && Character.Information.Berserking)
                    return;

                if (Data.SkillBase[Character.Action.UsingSkillID].Name.Contains("SKILL_OP") && Character.Information.GM == 0) return;
                if (Data.SkillBase[Character.Action.UsingSkillID].Name.Contains("TRADE")) return;
                
                if (!Base.Skill.CheckWeapon(Character.Information.Item.wID, Character.Action.UsingSkillID))
                {
                    client.Send(Packet.ActionPacket(2, 0x0D));
                    client.Send(Packet.IngameMessages2(SERVER_ACTIONSTATE, IngameMessages.UIIT_SKILL_USE_FAIL_WRONGWEAPON));
                    return;
                }
                
                if (Character.Action.ImbueID != 0 && 
                    Data.SkillBase[Character.Action.UsingSkillID].SkillType == Global.s_data.SkillTypes.IMBUE &&
                    !Data.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("hste")) 
                    return;

                if (Character.Action.Buff.count > 21) return;
                byte slot = SkillBuffGetFreeSlot();
                if (slot == 255) return;
                if (Character.Stat.SecondMP < Data.SkillBase[Character.Action.UsingSkillID].Mana) { client.Send(Packet.ActionPacket(2, 4)); Character.Action.Cast = false; return; }
                else
                {
                    Character.Stat.SecondMP -= Data.SkillBase[Character.Action.UsingSkillID].Mana;

                    if (Character.Stat.SecondMP < 0) Character.Stat.SecondMP = 1;
                    UpdateMp();

                    Character.Action.CastingSkill = Character.Ids.GetCastingID();
                    
                    if (Data.SkillBase[Character.Action.UsingSkillID].RadiusType == Global.s_data.RadiusTypes.ONETARGET)
                    {
                        Character.Action.Buff.SkillID[slot] = Character.Action.UsingSkillID;
                        Character.Action.Buff.OverID[slot] = Character.Ids.GetBuffID();

                        Character.Action.Buff.slot = slot;
                        Character.Action.Buff.count++;

                        foreach (KeyValuePair<string, int> p in Data.SkillBase[Character.Action.UsingSkillID].Properties1)
                        {
                            if (SkillAdd_Properties(this, p.Key, true, slot))
                            {
                                Character.Action.Cast = false;
                                return;
                            };
                        }
                    }

                    List<int> lis = Character.Spawn;
                    Send(Packet.ActionPacket(1, 0, Character.Action.UsingSkillID, Character.Information.UniqueID, Character.Action.CastingSkill, 0));

                    Character.Action.Cast = true;
                    Effect.EffectMain(Character.Action.Object, Character.Action.UsingSkillID);
                    StartCastingTimer(Data.SkillBase[Character.Action.UsingSkillID].CastingTime, lis);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Skill buff {0}", ex);
            }
        }
        public void SpecialBuff(int skillID)
        {
            try
            {
                Character.Action.UsingSkillID = skillID;
                if (SkillGetSameBuff(Character.Action.UsingSkillID)) return;
                if (Character.Action.Buff.count > 21) return;
                byte slot = SkillBuffGetFreeSlot();
                if (slot == 255) return;

                Character.Action.Buff.SkillID[slot] = Character.Action.UsingSkillID;
                Character.Action.Buff.OverID[slot] = Character.Ids.GetBuffID();

                Character.Action.Buff.slot = slot;
                Character.Action.Buff.count++;
                List<int> lis = Character.Spawn;

                //add properties
                foreach (KeyValuePair<string, int> p in Data.SkillBase[Character.Action.UsingSkillID].Properties1)
                {
                    if (SkillAdd_Properties(this, p.Key, true, slot)) 
                    { 
                        return; 
                    };
                }

                // if imbue add to current imbue
                if (Data.SkillBase[Character.Action.UsingSkillID].SkillType == Global.s_data.SkillTypes.IMBUE &&
                    !Data.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("hste"))
                {
                    if (Character.Action.ImbueID != 0) return;
                    Character.Action.ImbueID = Character.Action.UsingSkillID;
                }

                Send(lis, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot], false));

                if (Data.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("dura") &&
                    (Data.SkillBase[Character.Action.UsingSkillID].RadiusType == Global.s_data.RadiusTypes.ONETARGET || (Data.SkillBase[Character.Action.UsingSkillID].RadiusType != Global.s_data.RadiusTypes.ONETARGET && Data.SkillBase[Character.Action.UsingSkillID].efrUnk1 != 3)))
                {
                    // Skills with duration and self targeted
                    //Character.Action.CastingSkill = Character.Ids.GetCastingID();

                    //Send(lis, Packet.ActionPacket(1, 0, Character.Action.UsingSkillID, Character.Information.UniqueID, Character.Action.CastingSkill, 0));

                    StartBuffTimer(Data.SkillBase[Character.Action.UsingSkillID].Properties1["dura"], Character.Action.Buff.slot);
                }
                else if (Data.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("onff"))
                {
                    // mana consume/time buffs 
                    //Send(lis, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot], false));

                    string series = Data.SkillBase[Character.Action.UsingSkillID].Series.Remove(Data.SkillBase[Character.Action.UsingSkillID].Series.Length - 2);
                    Character.Action.Buff.InfiniteBuffs.Add(series, Character.Action.Buff.slot);
                }
                else if (!Data.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("dura"))
                {
                    // fast buffs
                    //Send(lis, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot], false));

                    SkillBuffEnd(Character.Action.Buff.slot);
                }

                Character.Action.Buff.slot = 255;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SpecialBuff error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        void SkillBuffCasting(List<int> list)
        {
            try
            {
                Character.Action.Buff.Casting = true;

                // add buff's special attributes (example: harmony terapy spawn packet etc)
                HandleSpecialBuff(Character.Action.UsingSkillID);

                Send(list, Packet.SkillPacket(0, Character.Action.CastingSkill));

                // every skill is handled as surround range radius
                if (Data.SkillBase[Character.Action.UsingSkillID].RadiusType == Global.s_data.RadiusTypes.ONETARGET || Data.SkillBase[Character.Action.UsingSkillID].SkillType == Global.s_data.SkillTypes.IMBUE)
                {
                    this.SpecialBuff(Character.Action.UsingSkillID);
                }
                else if (Data.SkillBase[Character.Action.UsingSkillID].RadiusType != Global.s_data.RadiusTypes.ONETARGET && Data.SkillBase[Character.Action.UsingSkillID].efrUnk1 != 3) // i use it as efr not handled here :) (example: harmony)
                {
                    byte currentSimult = 0;
                    lock (Systems.clients)
                    {
                        for (int i = 0; i <= Systems.clients.Count - 1; i++)
                        {
                            try
                            {
                                Systems s = Systems.clients[i];

                                //double distance = Formule.gamedistance((float)this.Character.Position.x, (float)this.Character.Position.y, s.Character.Position.x, s.Character.Position.y);
                                double distance = Formule.gamedistance(this.Character.Position, s.Character.Position);
                                if (distance <= Data.SkillBase[Character.Action.UsingSkillID].Distance / 10)
                                {
                                    if (Data.SkillBase[Character.Action.UsingSkillID].SimultAttack == 0 || currentSimult < Data.SkillBase[Character.Action.UsingSkillID].SimultAttack)
                                    {
                                        // todo: here handle inv and steal detect // only add "buff" to char on pvp state (dttp)
                                        s.SpecialBuff(Character.Action.UsingSkillID);
                                        currentSimult++;
                                    }
                                    else break;
                                }
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine("SkillBuffCast Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                                Systems.Debugger.Write(ex);
                            }
                        }
                    }
                }

                if (Data.SkillBase[Character.Action.UsingSkillID].SkillType == Global.s_data.SkillTypes.IMBUE ||
                   (Data.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("dura") &&
                   (Data.SkillBase[Character.Action.UsingSkillID].RadiusType == Global.s_data.RadiusTypes.ONETARGET || (Data.SkillBase[Character.Action.UsingSkillID].RadiusType != Global.s_data.RadiusTypes.ONETARGET && Data.SkillBase[Character.Action.UsingSkillID].efrUnk1 != 3))))
                {
                    //Skills with duration and self targeted
                    Send(list, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot],false));
                    client.Send(Packet.PlayerStat(Character));
                    UpdateMp();
                    client.Send(Packet.ActionState(2, 0));
                    StartBuffTimer(Data.SkillBase[Character.Action.UsingSkillID].Properties1["dura"], Character.Action.Buff.slot);

                }
                else if (Data.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("onff"))
                {
                    //area buffs / mana consumer - time buffs 
                    Send(list, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot], false));
                    client.Send(Packet.PlayerStat(Character));
                    UpdateMp();
                    client.Send(Packet.ActionState(2, 0));

                    string series = Data.SkillBase[Character.Action.UsingSkillID].Series.Remove(Data.SkillBase[Character.Action.UsingSkillID].Series.Length - 2);
                    Character.Action.Buff.InfiniteBuffs.Add(series, Character.Action.Buff.slot);
                }
                else if ( !Data.SkillBase[Character.Action.UsingSkillID].Properties1.ContainsKey("dura") )
                {
                    // fast buffs
                    Send(list, Packet.SkillIconPacket(Character.Information.UniqueID, Character.Action.UsingSkillID, Character.Action.Buff.OverID[Character.Action.Buff.slot], false));
                    client.Send(Packet.PlayerStat(Character));
                    UpdateMp();
                    client.Send(Packet.ActionState(2, 0));

                    SkillBuffEnd(Character.Action.Buff.slot);

                }
                Character.Action.Buff.Casting = false;
                Character.Action.Buff.slot = 255;
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("SkillBuffCast Error: {0}",ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void SkillBuffEnd(byte b)
        {
            try
            {
                foreach (KeyValuePair<string, int> p in Data.SkillBase[Character.Action.Buff.SkillID[b]].Properties1)
                {
                    SkillDelete_Properties(this, p.Key, true, b);
                }

                // if imbue delete the current imbue
                if (Data.SkillBase[Character.Action.Buff.SkillID[b]].SkillType == Global.s_data.SkillTypes.IMBUE)
                {
                    Character.Action.ImbueID = 0;
                }

                if (Timer.Buff[b] != null)
                {
                    Timer.Buff[b].Dispose();
                    Timer.Buff[b] = null;
                }
                else
                {
                    foreach (var pair in Character.Action.Buff.InfiniteBuffs)
                    {
                        if (pair.Value == b)
                        {
                            Character.Action.Buff.InfiniteBuffs.Remove(pair.Key);
                            break;
                        }
                    }
                }

                Send(Packet.SkillEndBuffPacket(Character.Action.Buff.OverID[b]));
                Global.ID.Delete(Character.Action.Buff.OverID[b]);
                Character.Action.Buff.OverID[b] = 0;
                Character.Action.Buff.SkillID[b] = 0;
                Character.Action.Buff.count--;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public byte SkillBuffGetFreeSlot()
        {
            for (byte b = 0; b <= Character.Action.Buff.SkillID.Length - 1; b++)
                if (Character.Action.Buff.SkillID[b] == 0) return b;
            return 255;
        }
        public byte DeBuffGetFreeSlot()
        {
            for (byte b = 0; b <= Character.Action.DeBuff.Effect.EffectID.Length - 1; b++)
                if (Character.Action.DeBuff.Effect.EffectID[b] == 0) return b;
            return 255;
        }
        public bool SkillGetSameBuff(int SkillID)
        {
            for (byte b = 0; b <= 19; b++)
            {
                if (Character.Action.Buff.SkillID[b] != 0)
                {
                    if (Data.SkillBase[Character.Action.Buff.SkillID[b]].Series.Remove(Data.SkillBase[Character.Action.Buff.SkillID[b]].Series.Length - 2)
                        == Data.SkillBase[SkillID].Series.Remove(Data.SkillBase[SkillID].Series.Length - 2)) return true;
                }
            }
            return false;
        }
        public bool SkillGetOpened(int SkillID)
        {
            if (Character.Information.GM == 1) return true;
            for (int b = 0; b <= Character.Stat.Skill.AmountSkill; b++)
            {
                if (Character.Stat.Skill.Skill[b] != 0 && Character.Information.GM == 0 && Character.Stat.Skill.Skill[b] == SkillID) return true;
            }
            return false;
        }
        byte SkillGetBuffIndex(int SkillID)
        {
            for (byte b = 0; b <= 19; b++)
            {
                if (Character.Action.Buff.SkillID[b] != 0 && Character.Action.Buff.SkillID[b] == SkillID)
                {
                    return b;
                }
            }
            return 255;
        }
        public void BuffAllClose()
        {
            try
            {
                if (Character != null)
                {
                    //Todo: Add item buff type "CBUF" check here, item buff remains after teleport etc
                    for (byte b = 0; b < Character.Action.Buff.SkillID.Length; b++)
                        if (Character.Action.Buff.SkillID[b] != 0) SkillBuffEnd(b);
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        bool BuffAdd() // todo: need to clean up
        {
            try
            {
                
                int status = rnd.Next(0, 100);
                //Console.WriteLine("BuffAdd Case: " + Data.SkillBase[Character.Action.UsingSkillID].Series.Remove(Data.SkillBase[Character.Action.UsingSkillID].Series.Length - 2));
                switch (Data.SkillBase[Character.Action.UsingSkillID].Series.Remove(Data.SkillBase[Character.Action.UsingSkillID].Series.Length - 2))
                {
                    
                    case "SKILL_CH_COLD_GIGONGTA":
                        if (Character.Action.ImbueID != 0) return true;
                        Character.Action.ImbueID = Character.Action.UsingSkillID;
                        Character.Action.Buff.castingtime = 0;
                        break;
                    case "SKILL_CH_LIGHTNING_GIGONGTA":
                        if (Character.Action.ImbueID != 0) return true;
                        Character.Action.ImbueID = Character.Action.UsingSkillID;
                        Character.Action.Buff.castingtime = 0;
                        break;
                    case "SKILL_CH_FIRE_GIGONGTA":
                        if (Character.Action.ImbueID != 0) return true;
                        Character.Action.ImbueID = Character.Action.UsingSkillID;
                        Character.Action.Buff.castingtime = 0;
                        break;
                    case "SKILL_CH_LIGHTNING_GYEONGGONG":
                        Character.Speed.DefaultSpeedSkill = Character.Action.UsingSkillID;
                        //Character.Speed.Updateded += Data.SkillBase[Character.Action.UsingSkillID].Properties1["hste"];
                        //Player_SetNewSpeed();
                        Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed, Character.Speed.RunSpeed));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_LIGHTNING_GWANTONG":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.UpdatededMagAttack += Data.SkillBase[Character.Action.UsingSkillID].Properties2["dru"]; //* (Character.Stat.MaxMagAttack / 100);
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_LIGHTNING_JIPJUNG":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.Parry += Data.SkillBase[Character.Action.UsingSkillID].Properties1["er"];
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_FIRE_GONGUP":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.UpdatededPhyAttack += Data.SkillBase[Character.Action.UsingSkillID].Properties1["dru"];
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_FIRE_GANGGI":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.MagDef += Data.SkillBase[Character.Action.UsingSkillID].Properties2["defp"];
                        Character.Stat.uMagDef += Data.SkillBase[Character.Action.UsingSkillID].Properties2["defp"];
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_COLD_GANGGI":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.PhyDef += Data.SkillBase[Character.Action.UsingSkillID].Properties1["defp"];
                        Character.Stat.uPhyDef += Data.SkillBase[Character.Action.UsingSkillID].Properties1["defp"];
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 2500;
                        break;
                    case "SKILL_CH_COLD_SHIELD":
                        if (Timer.Attack != null) StopAttackTimer();
                        Character.Stat.Absorb_mp = Data.SkillBase[Character.Action.UsingSkillID].Properties1["dgmp"];
                        client.Send(Packet.PlayerStat(Character));
                        Character.Action.Buff.castingtime = 0;
                        break;
                    case "SKILL_CH_SPEAR_SPIN":

                        //if (Data.SkillBase[Character.Action.UsingSkillID].Properties1[""] == 0)
                        //{
                            Character.Stat.MagDef += Data.SkillBase[Character.Action.UsingSkillID].Properties2["defp"];
                            Character.Stat.uMagDef += Data.SkillBase[Character.Action.UsingSkillID].Properties2["defp"];
                            client.Send(Packet.PlayerStat(Character));
                        //}

                        break;
                    case "SKILL_CH_SWORD_SHIELD":
                        //if (Data.SkillBase[Character.Action.UsingSkillID].Properties4 != 7)
                        //{
                        Character.Stat.PhyDef += Data.SkillBase[Character.Action.UsingSkillID].Properties1["defp"];
                        Character.Stat.uPhyDef += Data.SkillBase[Character.Action.UsingSkillID].Properties1["defp"];
                        client.Send(Packet.PlayerStat(Character));
                        //}
                        break;
                    case "SKILL_CH_SWORD_SHIELDPD":
                        
                        break;
                    case "SKILL_CH_BOW_CALL":
                        StopAttackTimer();
                        //if (Data.SkillBase[Character.Action.UsingSkillID].Properties4["summ"] == 0)
                            //Character.Stat.AttackPower += Data.SkillBase[Character.Action.UsingSkillID].Properties1;
                        break;
                    case "SKILL_CH_BOW_NORMAL":
                        //Character.Stat.EkstraMetre += (Data.SkillBase[Character.Action.UsingSkillID].Properties1);
                        break;
                    case "SKILL_CH_WATER_SELFHEAL":
                        Character.Stat.SecondHp += Data.SkillBase[Character.Action.UsingSkillID].Properties1["heal"];
                        UpdateHp();
                        break;
                    case "SKILL_CH_WATER_HEAL":
                        Systems s = GetPlayer(Character.Action.Target);
                        
                        if (s.Character.Stat.SecondHp + Data.SkillBase[Character.Action.UsingSkillID].Time < s.Character.Stat.Hp)
                            s.Character.Stat.SecondHp += Data.SkillBase[Character.Action.UsingSkillID].Time;
                        else
                            s.Character.Stat.SecondHp += s.Character.Stat.Hp - s.Character.Stat.SecondHp;

                        Character.Action.Buff.castingtime = (short)Data.SkillBase[Character.Action.UsingSkillID].CastingTime;
                        break;
                    case "SKILL_ETC_ARCHEMY_POTION_SPEED_":
                    case "SKILL_EU_BARD_SPEEDUPA_MSPEED":
                        Character.Speed.DefaultSpeedSkill = Character.Action.UsingSkillID;
                        //Character.Speed.Updateded += Data.SkillBase[Character.Action.UsingSkillID].Properties1["hste"];
                        //Player_SetNewSpeed();
                        Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed, Character.Speed.RunSpeed));
                        Character.Action.Buff.castingtime = 3000;
                        break;
                    case "SKILL_EU_BARD_BATTLAA_GUARD":

                        break;
                        //////////////////////////////////////////////////////////////
                        // Eu buffs
                        //////////////////////////////////////////////////////////////

                    case "SKILL_EU_BARD_SPEEDUPA_HITRATE":
                        Character.Stat.Hit += Data.SkillBase[Character.Action.UsingSkillID].Properties2["hr"];
                        break;
                    //default:
                        //Console.WriteLine("Non coded skill case: " + Data.SkillBase[Character.Action.UsingSkillID].Series.Remove(Data.SkillBase[Character.Action.UsingSkillID].Series.Length - 2));
                        //Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed, Character.Speed.RunSpeed));
                    //    break;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return false;
        }
        void BuffDelete(byte b_index)
        {
            try
            {
                switch (Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Series.Remove(Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Series.Length - 2))
                {
                    case "SKILL_CH_COLD_GIGONGTA":
                    case "SKILL_CH_LIGHTNING_GIGONGTA":
                    case "SKILL_CH_FIRE_GIGONGTA":
                        Character.Action.ImbueID = 0;
                        break;
                    case "SKILL_CH_LIGHTNING_GYEONGGONG":
                        //Character.Speed.Updateded -= Data.SkillBase[Character.Speed.DefaultSpeedSkill].Properties1["hste"];
                        //Player_SetNewSpeed();
                        Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed, Character.Speed.RunSpeed));
                        break;
                    case "SKILL_CH_LIGHTNING_GWANTONG":
                        Character.Stat.UpdatededMagAttack -= Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties2["dru"];
                        client.Send(Packet.PlayerStat(Character));
                        break;
                    case "SKILL_CH_LIGHTNING_JIPJUNG":
                        Character.Stat.Parry -= Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1["er"];
                        client.Send(Packet.PlayerStat(Character));
                        break;
                    case "SKILL_CH_FIRE_GONGUP":
                        Character.Stat.UpdatededPhyAttack = 0;
                        client.Send(Packet.PlayerStat(Character));
                        break;
                    case "SKILL_CH_FIRE_GANGGI":
                        Character.Stat.MagDef -= Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties2["defp"];
                        Character.Stat.uMagDef -= Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties2["defp"];
                        client.Send(Packet.PlayerStat(Character));
                        break;
                    case "SKILL_CH_COLD_GANGGI":
                        Character.Stat.PhyDef -= Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1["defp"];
                        Character.Stat.uPhyDef -= Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1["defp"];
                        client.Send(Packet.PlayerStat(Character));
                        break;
                    case "SKILL_CH_COLD_SHIELD":
                        Character.Stat.Absorb_mp = 0;
                        break;
                    case "SKILL_CH_SPEAR_SPIN":
                        //if (Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1 == 0)
                        //{
                            Character.Stat.MagDef -= Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties2["defp"];
                            Character.Stat.uMagDef -= Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties2["defp"];
                            client.Send(Packet.PlayerStat(Character));
                        //}
                        break;
                    case "SKILL_CH_SWORD_SHIELD":
                        //if (Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties4 != 7)
                        //{
                            Character.Stat.PhyDef -= Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1["defp"];
                            Character.Stat.uPhyDef -= Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1["defp"];
                            client.Send(Packet.PlayerStat(Character));
                        //}
                        break;
                    case "SKILL_CH_SWORD_SHIELDPD":
                        
                        break;
                    case "SKILL_CH_BOW_CALL":
                        //if (Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties3 == 0) Character.Stat.AttackPower -= Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1;
                        break;
                    case "SKILL_CH_BOW_NORMAL":
                        //Character.Stat.EkstraMetre -= (Data.SkillBase[Character.Action.Buff.SkillID[b_index]].Properties1);
                        break;
                    case "SKILL_ETC_ARCHEMY_POTION_SPEED_":
                    //////////////////////////////////////////////////////////////
                    // Eu buffs
                    //////////////////////////////////////////////////////////////
                    case "SKILL_EU_BARD_SPEEDUPA_MSPEED":
                        //Character.Speed.Updateded -= Data.SkillBase[Character.Speed.DefaultSpeedSkill].Properties1["hste"];
                        //Player_SetNewSpeed();
                        Send(Packet.SetSpeed(Character.Information.UniqueID, Character.Speed.WalkSpeed, Character.Speed.RunSpeed));
                        break;
                    case "SKILL_EU_BARD_BATTLAA_GUARD":

                        break;
                    case "SKILL_EU_BARD_SPEEDUPA_HITRATE":
                        Character.Stat.Hit += Data.SkillBase[Character.Action.UsingSkillID].Properties2["hr"];
                        break;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
 
        #endregion

        #region Regular Attack

        
        void ActionAttack()
        {
            try
            {
                //Predefined info needs work!
                #region Predefined info
                byte AttackAmount = 2;
                int AttackID = 1;
                int[] found = new int[3];
                byte numbert = 1;
                int PhysicalDamage = 0;
                byte status = 0, crit = 1;
                #endregion

                targetObject TargetObject = new targetObject(Character.Action.Object, this);

                if (TargetObject != null)
                {
                    client.Send(Packet.ActionState(1, 1));
                    //Bow information
                    #region Arrow for bow
                    if (Character.Information.Item.wID != 0)
                    {
                        if (Data.ItemBase[Character.Information.Item.wID].Itemtype == Global.item_database.ItemType.BOW || Data.ItemBase[Character.Information.Item.wID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                        {
                            if (Character.Information.Item.sID == 0)
                            {
                                if (!ItemCheckArrow())
                                {
                                    Character.Action.nAttack = false;
                                    client.Send(Packet.ActionPacket(2, 0x0e));
                                    StopAttackTimer();
                                    return;
                                }
                            }
                            else
                            {
                                Character.Information.Item.sAmount--;
                                client.Send(Packet.Arrow(Character.Information.Item.sAmount));

                                if (Character.Information.Item.sAmount <= 0)
                                {
                                    Character.Information.Item.sID = 0;
                                    MsSQL.UpdateData("delete from char_items where itemnumber='item" + 7 + "' AND owner='" + Character.Information.CharacterID + "'");
                                    if (!ItemCheckArrow())
                                    {
                                        Character.Action.nAttack = false;
                                        client.Send(Packet.ActionPacket(2, 0x0e));
                                        StopAttackTimer();
                                        return;
                                    }
                                }
                                else
                                {
                                    MsSQL.InsertData("UPDATE char_items SET quantity='" + Character.Information.Item.sAmount + "' WHERE itemnumber='" + "item" + 7 + "' AND owner='" + Character.Information.CharacterID + "' AND itemid='" + Character.Information.Item.sID + "'");
                                }
                            }
                        }
                    }
                    #endregion

                    if (Character.Action.ImbueID != 0 && Data.SkillBase[Character.Action.ImbueID].Series.Remove(Data.SkillBase[Character.Action.ImbueID].Series.Length - 2) == "SKILL_CH_LIGHTNING_GIGONGTA")
                    {
                        numbert = ActionGetObject(ref found, 2, TargetObject.x, TargetObject.y, Character.Action.Target, 5);
                    }
                    else found[1] = Character.Action.Target;

                    if (Character.Information.Item.wID != 0)
                    {
                        switch (Data.ItemBase[Character.Information.Item.wID].Class_C)
                        {
                            //Chinese base skills
                            case 2:                 //One handed sword
                            case 3:
                                AttackAmount = 2;
                                AttackID = 2;
                                break;
                            case 4:                 //Spear attack + glavie
                            case 5:
                                AttackAmount = 1;
                                AttackID = 40;
                                break;
                            case 6:                 //Bow attack
                                AttackAmount = 1;
                                AttackID = 70;
                                break;
                            //Europe Base skills
                            case 7:
                                AttackAmount = 1;
                                AttackID = 7127; // One handed sword
                                break;
                            case 8:
                                AttackAmount = 1;
                                AttackID = 7128; // Two handed sword
                                break;
                            case 9:
                                AttackAmount = 2;
                                AttackID = 7129; // Axe basic attack
                                break;
                            case 10:
                                AttackAmount = 1;
                                AttackID = 9069; // Warlock base
                                break;
                            case 11:
                                AttackAmount = 1;
                                AttackID = 8454; // Staff / Tstaff
                                break;
                            case 12:
                                AttackAmount = 1;
                                AttackID = 7909; // Crossbow base
                                break;
                            case 13:
                                AttackAmount = 2; //Dagger
                                AttackID = 7910;
                                break;
                            case 14:
                                AttackAmount = 1;
                                AttackID = 9606; // Harp base
                                break;
                            case 15:
                                AttackAmount = 1;
                                AttackID = 9970; // Light staff cleric
                                break;
                            case 16:
                                AttackAmount = 1;
                                AttackID = Data.SkillBase[Character.Action.UsingSkillID].ID;
                                break;
                            default:
                                Console.WriteLine("Action attack case: {0} , SkillID = {1}" + Data.ItemBase[Character.Information.Item.wID].Class_C, Data.SkillBase[Character.Action.UsingSkillID].ID);
                                break;
                        }
                    }
                    else
                    {
                        //Punch attack
                        AttackAmount = 1;
                        AttackID = 1;
                    }
                    //Get casting id
                    Character.Action.AttackingID = Character.Ids.GetCastingID();
                    //Create new packet writer
                    PacketWriter Writer = new PacketWriter();
                    Writer.Create(Systems.SERVER_ACTION_DATA);
                    Writer.Byte(1);
                    Writer.Byte(2);
                    Writer.Byte(0x30);
                    Writer.DWord(AttackID);
                    Writer.DWord(Character.Information.UniqueID);
                    Writer.DWord(Character.Action.AttackingID);
                    Writer.DWord(Character.Action.Target);
                    Writer.Bool(true);
                    Writer.Byte(AttackAmount);
                    Writer.Byte(numbert);

                    for (byte t = 1; t <= numbert; t++)
                    {
                        Writer.DWord(found[t]);

                        for (byte n = 1; n <= AttackAmount; n++)
                        {
                            PhysicalDamage = 0;
                            status = 0;
                            crit = 1;

                            if (t == 2) //for light skill
                            {
                                TargetObject = new targetObject(GetObjects(found[t]), this);
                                if (Character.Action.ImbueID != 0)
                                {
                                    PhysicalDamage = (int)Formule.gamedamage((Data.SkillBase[Character.Action.ImbueID].tmpProp), MasteryGetPower(Character.Action.ImbueID), 0, 0, Character.Information.Mag_Balance, Character.Stat.UpdatededMagAttack);

                                    PhysicalDamage += rnd.Next(0, PhysicalDamage.ToString().Length);
                                }
                                else PhysicalDamage = 1;

                                if (status != 128)
                                    status = TargetObject.HP((int)PhysicalDamage);
                                else TargetObject.GetDead();
                            }
                            else if (t == 1)
                            {
                                PhysicalDamage = (int)Formule.gamedamage(Character.Stat.MaxPhyAttack, Character.Stat.AttackPower + MasteryGetPower(AttackID), 0, (double)TargetObject.PhyDef, Character.Information.Phy_Balance, Character.Stat.UpdatededPhyAttack);
                                if (Character.Action.ImbueID != 0) PhysicalDamage += (int)Formule.gamedamage((Character.Stat.MinMagAttack + Data.SkillBase[Character.Action.ImbueID].tmpProp), MasteryGetPower(Character.Action.ImbueID), 0, TargetObject.MagDef, Character.Information.Mag_Balance, Character.Stat.UpdatededMagAttack);

                                PhysicalDamage /= AttackAmount;

                                if (rnd.Next(15) <= 5)
                                {
                                    PhysicalDamage *= 2;
                                    crit = 2;
                                }

                                if (Character.Information.Berserking)
                                    PhysicalDamage = (PhysicalDamage * Character.Information.BerserkOran) / 100;

                                if (PhysicalDamage <= 0)
                                    PhysicalDamage = 1;
                                else
                                {
                                    if (TargetObject.mAbsorb() > 0)
                                    {
                                        int static_dmg = (PhysicalDamage * (100 - (int)TargetObject.mAbsorb())) / 100;
                                        TargetObject.MP((static_dmg));
                                        PhysicalDamage = static_dmg;
                                    }
                                    PhysicalDamage += rnd.Next(0, PhysicalDamage.ToString().Length);
                                }

                                if (status != 128)
                                {
                                    status = TargetObject.HP((int)PhysicalDamage);
                                }
                                else TargetObject.GetDead();
                            }


                            Writer.Byte(status);
                            Writer.Byte(crit);
                            Writer.DWord(PhysicalDamage);
                            Writer.Byte(0);
                            Writer.Word(0);
                        }
                    }
                    Send(Writer.GetBytes());
                    client.Send(Packet.ActionState(2, 0));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ActionAttack Error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }

        byte ActionGetObject(ref int[] found, byte max, float ox, float oy, int objectid, byte metre)
        {
            byte founded = 1;
            found[1] = Character.Action.Target;
            try
            {
                float x = (float)ox - metre;
                float y = (float)oy - metre;
                for (int i = 0; i <= Systems.Objects.Count - 1; i++)
                {
                    if (founded == max) return founded;
                    obj o = Systems.Objects[i];
                    if (!o.Die && o.LocalType == 1)
                    {
                        if (o.x >= x && o.x <= (x + (metre * 2)) && o.y >= y && o.y <= (y + (metre * 2)) && o.UniqueID != objectid)
                        {
                            founded++;
                            if (o.Agro == null) o.Agro = new List<_agro>();
                            found[founded] = o.UniqueID;
                        }
                    }
                }
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            if (founded == max) return founded;
                            Systems sys = Systems.clients[i];
                            if (sys.Character.Information.PvP)
                            {
                                if (sys.Character.Information.UniqueID != objectid && Character.Information.UniqueID != objectid && Systems.clients[i].Character.Information.UniqueID != this.Character.Information.UniqueID)
                                {
                                    if (sys.Character.Position.x >= x && sys.Character.Position.x <= (x + (metre * 2)) && sys.Character.Position.y >= y && sys.Character.Position.y <= (y + (metre * 2)))
                                    {
                                        founded++;
                                        found[founded] = sys.Character.Information.UniqueID;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ActionGetObject Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ActionGetObject()::error...");
                Systems.Debugger.Write(ex);
            }
            return founded;

        }

        #endregion

        #region Skills

        void ActionSkill()
        {
            try
            {
                Global.s_data.TargetTypes TargetType = Data.SkillBase[Character.Action.Skill.MainSkill].TargetType;

                /*foreach (KeyValuePair<string, int> p in Data.SkillBase[Character.Action.Skill.MainSkill].Properties1)
                {
                    Console.WriteLine("{0}", p.Key);
                }*/

                bool gotoTarget = false;

                float x = 0, y = 0;
                bool[] aRound = null;
                if (!Character.Action.sAttack) return;

                obj o = null;
                Systems sys = null;

                if (Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "DarkEmu_GameServer.obj")
                    //&& (TargetType == Global.s_data.TargetTypes.MOB
                    // || TargetType == (Global.s_data.TargetTypes.MOB | Global.s_data.TargetTypes.PLAYER)
                    // || TargetType == (Global.s_data.TargetTypes.MOB | Global.s_data.TargetTypes.NOTHING)
                    // || TargetType == (Global.s_data.TargetTypes.MOB | Global.s_data.TargetTypes.PLAYER | Global.s_data.TargetTypes.NOTHING)))
                {
                    o = Character.Action.Object as obj;
                    if (o.Die || o.GetDie || o.LocalType != 1) { Character.Action.sAttack = false; StopSkillTimer(); return; }
                    if (o.Agro == null) o.Agro = new List<_agro>();

                    x = (float)o.x;
                    y = (float)o.y;
                    aRound = o.aRound;

                    if (!o.Attacking)
                        o.AddAgroDmg(Character.Information.UniqueID, 1);

                    gotoTarget = true;
                }
                else if (o == null && Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "DarkEmu_GameServer.Systems")
                    // && (TargetType == Global.s_data.TargetTypes.PLAYER
                    // || TargetType == Global.s_data.TargetTypes.PLAYER | TargetType == Global.s_data.TargetTypes.MOB
                    // || TargetType == Global.s_data.TargetTypes.PLAYER | TargetType == Global.s_data.TargetTypes.NOTHING
                    // || TargetType == Global.s_data.TargetTypes.PLAYER | TargetType == Global.s_data.TargetTypes.MOB | TargetType == Global.s_data.TargetTypes.NOTHING))
                {
                    if (Data.SkillBase[Character.Action.Skill.MainSkill].canSelfTargeted && (Character.Action.Target == this.Character.Information.UniqueID))
                    {
                        if (Data.SkillBase[Character.Action.Skill.MainSkill].needPVPstate && Character.Information.PvP)
                        { }
                        else
                        {
                            /* have to seek for error msg
                            client.Send(Packet.IngameMessages2(SERVER_ACTIONSTATE, IngameMessages.UIIT_STT_ERR_COMMON_INVALID_TARGET));*/
                            Character.Action.sAttack = false;
                            StopSkillTimer();
                            return;
                        }

                        sys = Character.Action.Object as Systems;
                        x = sys.Character.Position.x;
                        y = sys.Character.Position.y;
                        aRound = sys.Character.aRound;

                        gotoTarget = true;
                    }
                    else
                    {
                        /* have to seek for error msg
                        client.Send(Packet.IngameMessages2(SERVER_ACTIONSTATE, IngameMessages.UIIT_STT_ERR_COMMON_INVALID_TARGET));*/
                        Character.Action.sAttack = false;
                        StopSkillTimer();
                        return;
                    }
                }
                // no target TargetType = 3;
                else if (Character.Action.Object == null
                     && (TargetType == Global.s_data.TargetTypes.NOTHING
                     || TargetType == Global.s_data.TargetTypes.NOTHING | TargetType == Global.s_data.TargetTypes.MOB
                     || TargetType == Global.s_data.TargetTypes.NOTHING | TargetType == Global.s_data.TargetTypes.PLAYER
                     || TargetType == Global.s_data.TargetTypes.NOTHING | TargetType == Global.s_data.TargetTypes.MOB | TargetType == Global.s_data.TargetTypes.PLAYER))
                {
                    gotoTarget = false;
                }
                else
                {
                    Character.Action.sAttack = false;
                    StopSkillTimer();
                    return;
                }

                // go to target
                if (gotoTarget)
                {
                    double distance = Formule.gamedistance(Character.Position.x,
                                                                    Character.Position.y,
                                                                    x,
                                                                    y);

                    if (Character.Action.Skill.Distance == 0)
                        distance -= Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                    else distance -= Character.Action.Skill.Distance;

                    if (distance >= 0)
                    {
                        float farkx = x;
                        float farky = y;

                        if (Character.Action.Skill.Distance == 0)
                        {
                            if (Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE <= 10)
                                if (Systems.aRound(ref aRound, ref farkx, ref farky))
                                { Systems.aRound(ref farkx, ref farky, 1); }
                        }

                        Character.Position.wX = farkx - Character.Position.x; //- Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                        Character.Position.wY = farky - Character.Position.y; //- Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                        Character.Position.kX = Character.Position.wX;
                        Character.Position.kY = Character.Position.wY;
                        if (!Character.InGame) return;
                        Send(Packet.Movement(new DarkEmu_GameServer.Global.vektor(Character.Information.UniqueID,
                                    (float)Formule.packetx((float)farkx, Character.Position.xSec),
                                    (float)Character.Position.z,
                                    (float)Formule.packety((float)farky, Character.Position.ySec),
                                    Character.Position.xSec,
                                    Character.Position.ySec)));

                        distance += Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                        Character.Position.Time = (distance / (Character.Speed.RunSpeed * 0.0768)) * 1000.0;
                        Character.Position.RecordedTime = Character.Position.Time;

                        Character.Position.packetxSec = Character.Position.xSec;
                        Character.Position.packetySec = Character.Position.ySec;

                        Character.Position.packetX = (ushort)Formule.packetx((float)farkx, Character.Position.xSec);
                        Character.Position.packetY = (ushort)Formule.packety((float)farky, Character.Position.ySec);


                        Character.Position.Walking = true;
                        StartMovementTimer((int)(Character.Position.Time * 0.1));
                        return;
                    }
                }

                StartSkill();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ActionSkill error {0}", ex);
                Systems.Debugger.Write(ex);
            } 
        }

        void ActionSkillAttack()
        {
            try
            {

                float x = 0, y = 0;
                bool[] aRound = null;
                if (!Character.Action.sAttack) return;
                obj o = null;
                if (Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "DarkEmu_GameServer.obj")
                {
                    o = Character.Action.Object as obj;
                    if (o.Die || o.GetDie || o.LocalType != 1) { Character.Action.sAttack = false; StopSkillTimer(); return; }
                    if (o.Agro == null) o.Agro = new List<_agro>();
                    x = (float)o.x;
                    y = (float)o.y;
                    aRound = o.aRound;
                    if (!o.Attacking)
                        o.AddAgroDmg(Character.Information.UniqueID, 1);

                }
                //else { StopAttackTimer(); return; }
                if (o == null && Character.Action.Object != null && Character.Action.Object.GetType().ToString() == "DarkEmu_GameServer.Systems")
                {
                    Systems sys = Character.Action.Object as Systems;
                    if (!Character.Information.PvP || sys.Character.State.Die) { Character.Action.sAttack = false; StopSkillTimer(); return; }
                    if (!(Character.Information.PvP && sys.Character.Information.PvP)) { Character.Action.sAttack = false; StopSkillTimer(); return; }
                    x = sys.Character.Position.x;
                    y = sys.Character.Position.y;
                    aRound = sys.Character.aRound;
                }
                //else { StopAttackTimer(); return; }

                if (x == 0 && y == 0 && aRound == null) { Character.Action.sAttack = false; StopSkillTimer(); return; }

                double distance = Formule.gamedistance(Character.Position.x,
                        Character.Position.y,
                        x,
                        y);

                if (Character.Action.Skill.Distance == 0)
                    distance -= Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                else distance -= Character.Action.Skill.Distance;

                if (distance >= 0)
                {
                    float farkx = x;
                    float farky = y;

                    if (Character.Action.Skill.Distance == 0)
                    {
                        if (Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE <= 10)
                            if (Systems.aRound(ref aRound, ref farkx, ref farky))
                            
                            { 
                                //Systems.aRound(ref farkx, ref farky, 1); 
                            }
                    }

                    Character.Position.wX = farkx - Character.Position.x; //- Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                    Character.Position.wY = farky - Character.Position.y; //- Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                    Character.Position.kX = Character.Position.wX;
                    Character.Position.kY = Character.Position.wY;
                    if (!Character.InGame) return;
                    Send(Packet.Movement(new DarkEmu_GameServer.Global.vektor(Character.Information.UniqueID,
                                (float)Formule.packetx((float)farkx, Character.Position.xSec),
                                (float)Character.Position.z,
                                (float)Formule.packety((float)farky, Character.Position.ySec),
                                Character.Position.xSec,
                                Character.Position.ySec)));

                    if (Character.Information.Item.wID != 0)
                        distance += Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                    else
                        distance += Data.SkillBase[Character.Action.Skill.MainSkill].Distance;
                    //Todo implent modified speed checks
                    Character.Position.Time = (distance / (Character.Speed.RunSpeed * 0.0768)) * 1000.0;
                    Character.Position.RecordedTime = Character.Position.Time;

                    Character.Position.packetxSec = Character.Position.xSec;
                    Character.Position.packetySec = Character.Position.ySec;

                    Character.Position.packetX = (ushort)Formule.packetx((float)farkx, Character.Position.xSec);
                    Character.Position.packetY = (ushort)Formule.packety((float)farky, Character.Position.ySec);

                    Character.Position.Walking = true;
                    StartMovementTimer((int)(Character.Position.Time * 0.1));
                    return;
                }

                StartSkill();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Skill attack error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        void StartSkill()
        {
            try
            {
                if (!Character.Action.sAttack) return;

                #region Arrow for bow
                if (!Character.Action.Skill.P_M)
                {
                    if (Data.ItemBase[Character.Information.Item.wID].Class_C == 6)
                    {
                        if (Character.Information.Item.sID == 0) //arrow yoksa slota
                        {
                            if (!ItemCheckArrow()) //inventorydeki arrowlar kontrol et 
                            {
                                Character.Action.sAttack = false;
                                client.Send(Packet.ActionPacket(2, 0x0e)); //hic arrow yoksa hata ver
                                StopSkillTimer();
                                return;
                            }
                        }
                        else  //arrow varsa slotta
                        {
                            Character.Information.Item.sAmount--;
                            client.Send(Packet.Arrow(Character.Information.Item.sAmount));

                            if (Character.Information.Item.sAmount <= 0) // arrow bitti
                            {
                                Character.Information.Item.sID = 0;
                                MsSQL.UpdateData("delete from char_items where itemnumber='item" + 7 + "' AND owner='" + Character.Information.CharacterID + "'");
                                if (!ItemCheckArrow()) //inventorydeki arrowlar kontrol et 
                                {
                                    Character.Action.sAttack = false;
                                    client.Send(Packet.ActionPacket(2, 0x0e)); //hic arrow yoksa hata ver
                                    StopSkillTimer();
                                    return;
                                }
                            }
                            else
                            {
                                MsSQL.InsertData("UPDATE char_items SET quantity='" + Math.Abs(Character.Information.Item.sAmount) + "' WHERE itemnumber='" + "item" + 7 + "' AND owner='" + Character.Information.CharacterID + "' AND itemid='" + Character.Information.Item.sID + "'");
                            }

                        }
                    }
                }
                #endregion

                if (Character.Stat.SecondMP < Data.SkillBase[Character.Action.Skill.MainSkill].Mana) 
                { 
                    Character.Action.sAttack = false; 
                    StopSkillTimer(); 
                    client.Send(Packet.ActionPacket(2, 4)); 
                    return; 
                }
                else
                {
                    Character.Stat.SecondMP -= Data.SkillBase[Character.Action.Skill.MainSkill].Mana;

                    if (Character.Stat.SecondMP < 0) Character.Stat.SecondMP = 1;
                    UpdateMp();

                    Character.Action.Skill.MainCasting = Character.Ids.GetCastingID();
                    List<int> lis = Character.Spawn;

                    Send(lis, Packet.ActionPacket(1, 0, Character.Action.Skill.MainSkill, Character.Information.UniqueID, Character.Action.Skill.MainCasting, Character.Action.Target));

                    if (Data.SkillBase[Character.Action.Skill.MainSkill].isAttackSkill)
                    {

                        if (Character.Action.Skill.Instant == 0) 
                            MainSkill_Attack(lis);
                        else
                            StartSkillCastingTimer(Character.Action.Skill.Instant * 1000, lis);
                    }
                    else
                    {
                        Character.Action.sAttack = false;
                        Character.Action.sCasting = false;

                        // todo: here iterate through all target player (not only one object => efr)
                        if (Data.SkillBase[Character.Action.Skill.MainSkill].Properties1.ContainsKey("heal") ||
                            Data.SkillBase[Character.Action.Skill.MainSkill].Properties1.ContainsKey("curl"))
                        {
                            Systems Target = Character.Action.Object as Systems;

                            Target.SpecialBuff(Character.Action.Skill.MainSkill);
                        }
                        Effect.EffectMain(Character.Action.Object, Character.Action.Skill.MainSkill);
                    }
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        void MainSkill_Attack(List<int> list)
        {
            if (!Character.Action.sAttack) return;
            try
            {

                AmountControl();

                int[,] p_dmg = new int[Character.Action.Skill.Found, Character.Action.Skill.NumberOfAttack];
                int[] statichp = new int[Character.Action.Skill.Found];

                PacketWriter Writer = new PacketWriter();

                Writer.Create(Systems.SERVER_SKILL_DATA);
                Writer.Byte(1);
                Writer.DWord(Character.Action.Skill.MainCasting);
                Writer.DWord(Character.Action.Target);
                Writer.Byte(1);
                Writer.Byte(Character.Action.Skill.NumberOfAttack);
                Writer.Byte(Character.Action.Skill.Found);
                byte[] status;
                status = new byte[Character.Action.Skill.Found];
                targetObject[] target = new targetObject[Character.Action.Skill.Found];
                for (byte f = 0; f < Character.Action.Skill.Found; f++)
                {
                    if (Character.Action.Skill.FoundID[f] != 0)
                    {
                        Writer.DWord(Character.Action.Skill.FoundID[f]);
                        target[f] = new targetObject(GetObjects(Character.Action.Skill.FoundID[f]), this);
                        if (target[f].sys == null && target[f].os == null) { }
                        else
                        {
                            statichp[f] = target[f].GetHp;
                            for (byte n = 0; n < Character.Action.Skill.NumberOfAttack; n++)
                            {
                                bool block = false;
                                /*if (Character.Information.Item.sID != 0 && Character.Information.Item.sID != 62)
                                {
                                    if (Global.RandomID.GetRandom(0, 25) < 10) block = true;
                                }*/
                                if (!block)
                                {
                                    byte crit = 1;
                                    p_dmg[f, n] = 1;

                                    if (Character.Action.Skill.P_M) // for magic damage
                                    {
                                        p_dmg[f, n] = (int)Formule.gamedamage((Character.Stat.MaxMagAttack + Data.SkillBase[Character.Action.Skill.SkillID[n]].MaxAttack), MasteryGetPower(Character.Action.Skill.SkillID[n]), target[f].AbsrobMag, target[f].MagDef, this.Character.Information.Mag_Balance, (Data.SkillBase[Character.Action.Skill.SkillID[n]].MagPer + Character.Stat.UpdatededMagAttack));
                                        if (Character.Action.ImbueID != 0) p_dmg[f, n] += (int)Formule.gamedamage((Character.Stat.MinMagAttack + Data.SkillBase[Character.Action.ImbueID].MaxAttack), MasteryGetPower(Character.Action.ImbueID), 0, target[f].MagDef, Character.Information.Mag_Balance, Character.Stat.UpdatededMagAttack);
                                    }
                                    else // for phy damage
                                    {
                                        p_dmg[f, n] = (int)Formule.gamedamage((Character.Stat.MaxPhyAttack + Data.SkillBase[Character.Action.Skill.SkillID[n]].MaxAttack), MasteryGetPower(Character.Action.Skill.SkillID[n]), target[f].AbsrobPhy, target[f].PhyDef, this.Character.Information.Phy_Balance, Character.Stat.UpdatededPhyAttack + Data.SkillBase[Character.Action.Skill.SkillID[n]].MagPer);
                                        if (Character.Action.ImbueID != 0) p_dmg[f, n] += (int)Formule.gamedamage((Character.Stat.MinMagAttack + Data.SkillBase[Character.Action.ImbueID].MaxAttack), MasteryGetPower(Character.Action.ImbueID), 0, target[f].MagDef, Character.Information.Mag_Balance, Character.Stat.UpdatededMagAttack);
                                        if (rnd.Next(16) < 5)
                                        {
                                            p_dmg[f, n] *= 2;
                                            crit = 2;
                                        }
                                    }

                                    if (f > 0) p_dmg[f, n] = (p_dmg[f, n] * (100 - (f * 10))) / 100;

                                    if (Character.Information.Berserking)
                                        p_dmg[f, n] = (p_dmg[f, n] * Character.Information.BerserkOran) / 100;

                                    if (p_dmg[f, n] <= 0)
                                        p_dmg[f, n] = 1;
                                    else
                                    {
                                        if (target[f].mAbsorb() > 0)
                                        {
                                            int static_dmg = (p_dmg[f, n] * (100 - (int)target[f].mAbsorb())) / 100;
                                            target[f].MP(static_dmg);
                                            p_dmg[f, n] = static_dmg;
                                        }
                                        p_dmg[f, n] += rnd.Next(0, p_dmg.ToString().Length);
                                    }


                                    statichp[f] -= p_dmg[f, n];
                                    if (statichp[f] < 1)
                                    {
                                        status[f] = 128;
                                        target[f].GetDead();
                                    }


                                    if (Character.Action.Skill.OzelEffect == 4 && status[f] != 128)
                                    {
                                        if (rnd.Next(10) > 5)
                                            status[f] = 4;
                                    }

                                    Writer.Byte(status[f]); // so here we add status same opcode
                                    Writer.Byte(crit);
                                    Writer.DWord(p_dmg[f, n]);
                                    Writer.Byte(0);
                                    Writer.Word(0);
                                    
                                    if (status[f] == 4) // if status was knockdown just add the new position of the mob where it should be knocked down
                                    {
                                        Writer.Byte(target[f].xSec);
                                        Writer.Byte(target[f].ySec);
                                        Writer.Word(Formule.packetx(target[f].x + 0.1f, target[f].xSec));
                                        Writer.Word(0);
                                        Writer.Word(Formule.packety(target[f].y - 0.1f, target[f].ySec));
                                        Writer.Word(0);
                                        Writer.Word(0);
                                        Writer.Word(0);
                                    }

                                }
                                else Writer.Byte(2);
                            }
                        }

                    }
                }
                Send(list, Writer.GetBytes());

                for (byte f = 0; f < Character.Action.Skill.Found; f++)
                {
                    if (target[f].sys == null && target[f].os == null)
                    { }
                    else
                    {
                        if (target[f].sys != null && target[f].os == null) // player
                            DarkEmu_GameServer.Effect.EffectMain(target[f].sys, Character.Action.Skill.MainSkill);

                        if (target[f].sys == null && target[f].os != null) // mob
                            DarkEmu_GameServer.Effect.EffectMain(target[f].os, Character.Action.Skill.MainSkill);
                    }
                }

                Character.Action.sCasting = true;
                Character.Action.sAttack = false;
                if (Data.SkillBase[Character.Action.Skill.MainSkill].AttackTime == 0) 
                    Data.SkillBase[Character.Action.Skill.MainSkill].AttackTime = 150;

                StartsWaitTimer(Data.SkillBase[Character.Action.Skill.MainSkill].AttackTime, target, p_dmg, status);
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        void AmountControl()
        {
            try
            {
                Character.Action.Skill.FoundID[Character.Action.Skill.Found] = Character.Action.Target;
                Character.Action.Skill.Found++;
                if (Character.Action.Skill.Tdistance > 1)
                {
                    object mainObject = GetObjects(Character.Action.Target);
                    if (mainObject == null) return;
                    targetObject target = new targetObject(mainObject, this);
                    float x = target.x - Character.Action.Skill.Tdistance;
                    float y = target.y - Character.Action.Skill.Tdistance;
                    for (int i = 0; i < Systems.Objects.Count; i++)
                    {
                        if (Character.Action.Skill.Found == Character.Action.Skill.Tdistance) return;
                        if (Systems.Objects[i] != null)
                        {
                            obj o = Systems.Objects[i];
                            if (!o.Die && o.LocalType == 1)
                            {
                                if (o.x >= x && o.x <= (x + (Character.Action.Skill.Tdistance * 2)) && o.y >= y - Character.Action.Skill.Tdistance && o.y <= (y + (Character.Action.Skill.Tdistance * 2)) && o.UniqueID != Character.Action.Target)
                                {
                                    if (o.Agro == null) o.Agro = new List<_agro>();
                                    Character.Action.Skill.FoundID[Character.Action.Skill.Found] = o.UniqueID;
                                    Character.Action.Skill.Found++;
                                }
                            }
                        }
                    }
                    lock (Systems.clients)
                    {
                        for (int i = 0; i < Systems.clients.Count; i++)
                        {
                            try
                            {
                                if (Character.Action.Skill.Found == Character.Action.Skill.Tdistance) return;
                                if (Systems.clients[i] != null)
                                {
                                    Systems sys = Systems.clients[i];
                                    if (sys.Character.Information.PvP && sys != this && !sys.Character.State.Die)
                                        if (sys.Character.Information.UniqueID != Character.Action.Target && Character.Information.UniqueID != Character.Action.Target)
                                        {
                                            if (sys.Character.Position.x >= x && sys.Character.Position.x <= (x + (Character.Action.Skill.Tdistance * 2)) && sys.Character.Position.y >= y && sys.Character.Position.y <= (y + (Character.Action.Skill.Tdistance * 2)))
                                            {
                                                Character.Action.Skill.FoundID[Character.Action.Skill.Found] = sys.Character.Information.UniqueID;
                                                Character.Action.Skill.Found++;
                                            }
                                        }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("AmountControl Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                                Systems.Debugger.Write(ex);
                            }
                        }
                    }
                    target = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Distance error: " + ex);
            }
        }

        #endregion
        #region Petskills
        void PetSkill(int skillid, pet_obj o)
        {
            PacketWriter writer = new PacketWriter();
            writer.Create(Systems.SERVER_ACTION_DATA);
            /* Will add this later
             * Packet sniff:
             * [S -> C][B070]
             * 01                                                ................
             * 02 30                                             .0..............
             * 47 0F 00 00                                       G...............
             * AF FD 1A 03                                       ................
             * 1F 51 03 00                                       .Q..............
             * 93 AA 1A 00                                       ................
             * 01                                                ................
             * 01                                                ................
             * 01                                                ................
             * 93 AA 1A 00                                       ................
             * 00                                                ................
             * 01 0F 00 00                                       ................
             * 00 00 00 00                                       ................
             */
        }
        #endregion

        #region Moving Skill
        void MovementSkill(PacketReader Reader)
        {
            try
            {
                if (!Character.Action.movementskill)
                {
                    Character.Action.movementskill = true;
                    MovementSkillTimer(Data.SkillBase[Character.Action.UsingSkillID].Properties1["tele"] + 500);
                    if (Character.Action.sAttack || Character.Action.sCasting) return;

                    if (Character.Stat.SecondMP < Data.SkillBase[Character.Action.UsingSkillID].Mana) { client.Send(Packet.ActionPacket(2, 4)); return; }
                    else
                    {
                        Character.Stat.SecondMP -= Data.SkillBase[Character.Action.UsingSkillID].Mana;
                        UpdateMp();
                        if (Timer.Movement != null) { Timer.Movement.Dispose(); Character.Position.Walking = false; }

                        byte xSec = Reader.Byte(), ySec = Reader.Byte();
                        int x = Reader.Int32(), z = Reader.Int32(), y = Reader.Int32();
                        Reader.Close();

                        float gamex = DarkEmu_GameServer.Formule.gamex((float)x, xSec);
                        float gamey = DarkEmu_GameServer.Formule.gamey((float)y, ySec);

                        float farkx = gamex - Character.Position.x;
                        float farky = gamey - Character.Position.y;

                        float hesapy = 0, hesapx = 0;

                        while (hesapx + hesapy < Data.SkillBase[Character.Action.UsingSkillID].Properties2["tele"] / 10)
                        {
                            Character.Position.x += (farkx / 30);
                            Character.Position.y += (farky / 30);
                            hesapx += Math.Abs((farkx / 30));
                            hesapy += Math.Abs((farky / 30));
                        }

                        PacketWriter Writer = new PacketWriter();

                        Writer.Create(SERVER_ACTION_DATA);
                        Writer.Byte(1);
                        Writer.Byte(2);
                        Writer.Byte(0x30);
                        int overid = Character.Ids.GetCastingID();
                        Writer.DWord(Character.Action.UsingSkillID);//skillid
                        Writer.DWord(Character.Information.UniqueID); //charid
                        Writer.DWord(overid);//overid
                        Writer.DWord(0);
                        Writer.Byte(8);
                        Writer.Byte(xSec);
                        Writer.Byte(ySec);
                        Writer.DWord(Formule.packetx(Character.Position.x, xSec));
                        Writer.DWord(0);
                        Writer.DWord(Formule.packety(Character.Position.y, ySec));

                        Send(Writer.GetBytes());

                        client.Send(Packet.ActionState(2, 0));

                        ObjectSpawnCheck();
                    }
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            } 
        }
        #endregion

        #region Handle Buff Attributes
        public bool SkillAdd_Properties(Systems Target, string PropertiesName, bool UpdatePacket, byte slot = 255, int skillid = -1)
        {
            try
            {
                switch (PropertiesName)
                {
                    case "hpi":
                        ChangeMaxHP_hpi(Target, slot, false, UpdatePacket);
                        break;
                    case "mpi":
                        ChangeMaxMP_mpi(Target, slot, false, UpdatePacket);
                        break;
                    case "dru":
                        ChangeAtk_dru(Target, slot, false,UpdatePacket);
                        break;
                    case "er":
                        ChangeParry_er(Target, slot, false,UpdatePacket);
                        break;
                    case "stri":
                        ChangeStr_stri(Target, slot, false,UpdatePacket);
                        break;
                    case "inti":
                        ChangeInt_inti(Target, slot, false, UpdatePacket);
                        break;
                    case "cr":
                        ChangeCrit_cr(Target, slot, false,UpdatePacket);
                        break;
                    case "br":
                        ChangeBlockingRatio_br(Target, slot, false,UpdatePacket);
                        break;
                    case "spda":
                        Change_spda(Target, slot, false,UpdatePacket);
                        break;
                    case "ru":
                        ChangeRange_ru(Target, slot, false,UpdatePacket);
                        break;
                    case "dgmp":
                        ChangeAbsorbMP_dgmp(Target, slot, false,UpdatePacket);
                        break;
                    case "defp":
                        ChangeDefPower_defp(Target, slot, false, UpdatePacket);
                        break;
                    case "hste":
                        ChangeSpeed_hste(Target,slot,false,UpdatePacket);
                        break;
                    case "drci":
                        ChangeCriticalParry_dcri(Target, slot, false, UpdatePacket);
                        break;
                    case "heal":
                        HealHPMP(Target, slot, skillid, false, UpdatePacket);
                        break;
                    case "E1SA": // setvaluek ( valószínű ) nem így lesznek 
                        ChangePhyAtk_E1SA(Target, slot, false, UpdatePacket);
                        break;
                    case "E2SA":
                        ChangePhyAtk_E2SA(Target, slot, false, UpdatePacket);
                        break;
                    case "E2AH":
                        ChangeHitRate_E2AH(Target, slot, false, UpdatePacket);
                        break;
                    case "terd":
                        ChangeParry_terd(Target, slot, false, UpdatePacket);
                        break;
                    case "chcr":
                        ChangeTargetHp_chcr(Target, slot, false, UpdatePacket);
                        break;
                    case "cmcr":
                        ChangeTargetHp_cmcr(Target, slot, false, UpdatePacket);
                        break;
                    case "thrd":
                        ChangeDecAttkRate_thrd(Target, slot, false, UpdatePacket);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("BuffAdd_Properties() error..");
                Systems.Debugger.Write(ex);
            }
            return false;
        }
        bool SkillDelete_Properties(Systems Target, string PropertiesName, bool UpdatePacket, byte slot = 255, int skillid = -1)
        {
            try
            {
                switch (PropertiesName)
                {
                    case "hpi":
                        ChangeMaxHP_hpi(Target, slot, true, UpdatePacket);
                        break;
                    case "mpi":
                        ChangeMaxMP_mpi(Target, slot, true, UpdatePacket);
                        break;
                    case "dru":
                        ChangeAtk_dru(Target, slot, true,UpdatePacket);
                        break;
                    case "er":
                        ChangeParry_er(Target, slot, true,UpdatePacket);
                        break;
                    case "stri":
                        ChangeStr_stri(Target, slot, true,UpdatePacket);
                        break;
                    case "inti":
                        ChangeInt_inti(Target, slot, true, UpdatePacket);
                        break;
                    case "cr":
                        ChangeCrit_cr(Target, slot, true,UpdatePacket);
                        break;
                    case "br":
                        ChangeBlockingRatio_br(Target, slot, true,UpdatePacket);
                        break;
                    case "spda":
                        Change_spda(Target, slot, true,UpdatePacket);
                        break;
                    case "ru":
                        ChangeRange_ru(Target, slot, true,UpdatePacket);
                        break;
                    case "dgmp":
                        ChangeAbsorbMP_dgmp(Target, slot, true,UpdatePacket);
                        break;
                    case "defp":
                        ChangeDefPower_defp(Target, slot, true, UpdatePacket);
                        break;
                    case "hste":
                        ChangeSpeed_hste(Target, slot, true, UpdatePacket);
                        break;
                    case "drci":
                        ChangeCriticalParry_dcri(Target, slot, true, UpdatePacket);
                        break;
                    case "heal":
                        HealHPMP(Target, slot, skillid, true, UpdatePacket);
                        break;
                    case "E1SA": // setvaluek ( valószínű ) nem így leszneek 
                        ChangePhyAtk_E1SA(Target, slot, true, UpdatePacket);
                        break;
                    case "E2SA":
                        ChangePhyAtk_E2SA(Target, slot, true, UpdatePacket);
                        break;
                    case "E2AH":
                        ChangeHitRate_E2AH(Target, slot, true, UpdatePacket);
                        break;
                    case "terd":
                        ChangeParry_terd(Target, slot, true, UpdatePacket);
                        break;
                    case "chcr":
                        ChangeTargetHp_chcr(Target, slot, true, UpdatePacket);
                        break;
                    case "cmcr":
                        ChangeTargetHp_cmcr(Target, slot, true, UpdatePacket);
                        break;
                    case "thrd":
                        ChangeDecAttkRate_thrd(Target, slot, true, UpdatePacket);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("BuffDelete_Properties() error..");
                Systems.Debugger.Write(ex);
            }
            return false;
        }

        public void ChangeMaxHP_hpi(Systems Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["hpi"] != 0) // point inc
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["hpi"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Hp = amount;
                    }
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["hpi"] != 0) // %inc
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["hpi"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Hp = (Target.Character.Stat.Hp / 100) * (amount);
                    }
                    // add it
                    Target.Character.Stat.Hp += Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                    Target.Character.Stat.SecondHp += Target.Character.Action.Buff.UpdatedStats[slot].Hp;

                    if ((Target.Character.Stat.SecondHp + Target.Character.Action.Buff.UpdatedStats[slot].Hp) > Target.Character.Stat.Hp)
                    {
                        Target.Character.Stat.SecondHp = Target.Character.Stat.Hp;
                    }
                    else
                    {
                        Target.Character.Stat.SecondHp += Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                    }
                }
                else
                {
                    // sub it
                    Target.Character.Stat.Hp -= Target.Character.Action.Buff.UpdatedStats[slot].Hp;

                    // dont kill him :)
                    if (Target.Character.Stat.SecondHp - Target.Character.Action.Buff.UpdatedStats[slot].Hp < 1)
                    {
                        Target.Character.Stat.SecondHp = 1;
                    }
                    else
                    {
                        Target.Character.Stat.SecondHp -= Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                    }

                    Target.Character.Action.Buff.UpdatedStats[slot].Hp = 0;

                }

                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeMaxMP_mpi(Systems Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["mpi"] != 0) // point inc
                    {
                        int amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["mpi"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Mp = amount;
                    }
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["mpi"] != 0) // %inc
                    {
                        int amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["mpi"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Mp = (Target.Character.Stat.Hp / 100) * (amount);
                    }

                    Target.Character.Stat.Mp += Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                    Target.Character.Stat.SecondMP += Target.Character.Action.Buff.UpdatedStats[slot].Mp;

                    if ((Target.Character.Stat.SecondMP + Target.Character.Action.Buff.UpdatedStats[slot].Mp) > Target.Character.Stat.Mp)
                    {
                        Target.Character.Stat.SecondMP = Target.Character.Stat.Mp;
                    }
                    else
                    {
                        Target.Character.Stat.SecondMP += Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                    }
                }
                else
                {
                    // sub it
                    Target.Character.Stat.Mp -= Target.Character.Action.Buff.UpdatedStats[slot].Mp;

                    // dont want negative mana
                    if (Target.Character.Stat.SecondMP - Target.Character.Action.Buff.UpdatedStats[slot].Mp < 1)
                    {
                        Target.Character.Stat.SecondMP = 1;
                    }
                    else
                    {
                        Target.Character.Stat.SecondMP -= Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                    }

                    Target.Character.Action.Buff.UpdatedStats[slot].Mp = 0;
                }

                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
               Systems.Debugger.Write(ex);
            }
        }
        public void ChangeAtk_dru(Systems Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dru"] != 0) // phy attack %inc
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dru"];
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = (Target.Character.Stat.MinPhyAttack / 100) * (amount);
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = (Target.Character.Stat.MaxPhyAttack / 100) * (amount);
                        Target.Character.Stat.MinPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                    }
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["dru"] != 0) // mag attack %inc
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["dru"];
                        Target.Character.Action.Buff.UpdatedStats[slot].MinMagAttack = (Target.Character.Stat.MinMagAttack / 100) * (amount);
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxMagAttack = (Target.Character.Stat.MaxMagAttack / 100) * (amount);
                        Target.Character.Stat.MinMagAttack += Target.Character.Action.Buff.UpdatedStats[slot].MinMagAttack;
                        Target.Character.Stat.MaxMagAttack += Target.Character.Action.Buff.UpdatedStats[slot].MaxMagAttack;
                    }


                }
                else
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dru"] != 0) // phy attack %inc
                    {
                        Target.Character.Stat.MinPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = 0;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = 0;
                    }
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["dru"] != 0) // mag attack %inc
                    {
                        Target.Character.Stat.MinMagAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MinMagAttack;
                        Target.Character.Stat.MaxMagAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MaxMagAttack;
                        Target.Character.Action.Buff.UpdatedStats[slot].MinMagAttack = 0;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxMagAttack = 0;
                    }
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeParry_er(Systems Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["er"] != 0) // parry inc
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["er"];

                        Target.Character.Action.Buff.UpdatedStats[slot].Parry = amount;
                        Target.Character.Stat.Parry += Target.Character.Action.Buff.UpdatedStats[slot].Parry;
                    }
                    else if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["er"] != 0) // parry %inc?
                    {

                    }
                }
                else
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["er"] != 0) // parry inc
                    {
                        Target.Character.Stat.Parry -= Target.Character.Action.Buff.UpdatedStats[slot].Parry;
                        Target.Character.Action.Buff.UpdatedStats[slot].Parry = 0;
                    }
                    else if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["er"] != 0) // parry %inc?
                    {
                    }
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeStr_stri(Systems Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["stri"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["stri"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Strength = (short)amount;
                    }
                    /*if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2.ContainsKey("stri"))
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["stri"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Strength = (short)((Target.Character.Stat.Strength / 100) * (amount));
                    }*/
                    Target.Character.Stat.Strength += Target.Character.Action.Buff.UpdatedStats[slot].Strength;
                }
                else
                {
                    Target.Character.Stat.Strength -= Target.Character.Action.Buff.UpdatedStats[slot].Strength;
                    Target.Character.Action.Buff.UpdatedStats[slot].Strength = 0;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeInt_inti(Systems Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["inti"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["inti"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Intelligence = (short)amount;
                    }
                    //TODO majd uncomment.
                    /*if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2.ContainsKey("inti"))
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["inti"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Intelligence = (short)((Target.Character.Stat.Intelligence / 100) * (amount));
                    }*/
                    Target.Character.Stat.Intelligence += Target.Character.Action.Buff.UpdatedStats[slot].Intelligence;
                }
                else
                {
                    Target.Character.Stat.Intelligence -= Target.Character.Action.Buff.UpdatedStats[slot].Intelligence;
                    Target.Character.Action.Buff.UpdatedStats[slot].Intelligence = 0;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeBlockingRatio_br(Systems Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                int amount;
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["br"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["br"];
                        Target.Character.Action.Buff.UpdatedStats[slot].BlockRatio = amount;
                    }
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["br"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["br"];
                        Target.Character.Action.Buff.UpdatedStats[slot].BlockRatio = (Target.Character.Stat.BlockRatio / 100) * (amount);
                    }
                    Target.Character.Stat.BlockRatio += Target.Character.Action.Buff.UpdatedStats[slot].BlockRatio;
                }
                else
                {
                    Target.Character.Stat.BlockRatio -= Target.Character.Action.Buff.UpdatedStats[slot].BlockRatio;
                    Target.Character.Action.Buff.UpdatedStats[slot].BlockRatio = 0;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeCrit_cr(Systems Target, int slot, bool delete,bool UpdatePacket)
        {
            if (!delete)
            {

            }
        }
        public void Change_spda(Systems Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                double amount;
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1.ContainsKey("spda"))  //Phydef decrease?
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["spda"];
                        Target.Character.Action.Buff.UpdatedStats[slot].uPhyDef = (Target.Character.Stat.PhyDef / 100) * (amount);
                        Target.Character.Stat.PhyDef -= Target.Character.Action.Buff.UpdatedStats[slot].uPhyDef;
                    }
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2.ContainsKey("spda")) //Phy attack inc?
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["spda"];
                        Target.Character.Stat.UpdatededPhyAttack = (Target.Character.Stat.MaxPhyAttack / 100) * (amount);
                        Target.Character.Stat.MaxPhyAttack += Target.Character.Stat.UpdatededPhyAttack;
                    }
                }
                else
                {
                    Target.Character.Stat.PhyDef += Target.Character.Action.Buff.UpdatedStats[slot].uPhyDef;
                    Target.Character.Stat.MaxPhyAttack -= Target.Character.Stat.UpdatededPhyAttack;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeSpeed_hste(Systems Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["hste"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["hste"];
                        Target.Character.Speed.Updateded[slot] += (Target.Character.Speed.RunSpeed / 100) * amount;

                    }
                    Target.Character.Speed.RunSpeed += Target.Character.Speed.Updateded[slot];
                }
                else
                {
                    Target.Character.Speed.RunSpeed -= Target.Character.Speed.Updateded[slot];
                    Target.Character.Speed.Updateded[slot] = 0;
                }
                if (UpdatePacket) Target.client.Send(Packet.SetSpeed(Target.Character.Information.UniqueID, Target.Character.Speed.WalkSpeed, Target.Character.Speed.RunSpeed));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeRange_ru(Systems Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["ru"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["ru"];
                        Target.Character.Action.Buff.UpdatedStats[slot].EkstraMetre = (byte)amount;
                        Target.Character.Stat.EkstraMetre += Target.Character.Action.Buff.UpdatedStats[slot].EkstraMetre;
                    }
                }
                else
                {
                    Target.Character.Stat.EkstraMetre -= Target.Character.Action.Buff.UpdatedStats[slot].EkstraMetre;
                    Target.Character.Action.Buff.UpdatedStats[slot].EkstraMetre = 0;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeAbsorbMP_dgmp(Systems Target, int slot, bool delete,bool UpdatePacket) // dgmp stat ellenőrzése hogy mire jó??
        {
            try
            {
                if (!delete)
                {
                    int amount;
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dgmp"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dgmp"];
                        Target.Character.Stat.Absorb_mp = amount;
                    }
                }
                else
                {
                    Target.Character.Stat.Absorb_mp = 0;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeCriticalParry_dcri(Systems Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                int amount;
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dcri"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dcri"];
                        Target.Character.Stat.CritParryRatio += amount;
                    }
                }
                else
                {
                    amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["dcri"];
                    Target.Character.Stat.CritParryRatio -= amount;
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeDefPower_defp(Systems Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                int amount;
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["defp"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["defp"];
                        Target.Character.Action.Buff.UpdatedStats[slot].PhyDef = amount;
                        Target.Character.Stat.PhyDef += Target.Character.Action.Buff.UpdatedStats[slot].PhyDef;
                    }
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["defp"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["defp"];
                        Target.Character.Action.Buff.UpdatedStats[slot].MagDef = amount;
                        Target.Character.Stat.MagDef += Target.Character.Action.Buff.UpdatedStats[slot].MagDef;
                    }
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties3["defp"] != 0)
                    {
                        //nemtudjuk
                    }
                }
                else
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["defp"] != 0)
                    {
                        Target.Character.Stat.PhyDef -= Target.Character.Action.Buff.UpdatedStats[slot].PhyDef;
                        Target.Character.Action.Buff.UpdatedStats[slot].PhyDef = 0;
                    }
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties2["defp"] != 0)
                    {
                        Target.Character.Stat.MagDef -= Target.Character.Action.Buff.UpdatedStats[slot].MagDef;
                        Target.Character.Action.Buff.UpdatedStats[slot].MagDef = 0;
                    }
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties3["defp"] != 0)
                    {
                        //nemtudjuk
                    }
                }

                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        static public void HealHPMP(Systems Target, int slot, int skillid, bool delete, bool UpdatePacket)
        {
            try
            {
                int amount;
                int sid;

                // get skillid from parameters
                if (skillid == -1)
                    sid = Target.Character.Action.Buff.SkillID[slot];
                else
                    sid = skillid;

                if (!delete)
                {
                    // if hp full
                    if (Target.Character.Stat.SecondHp == Target.Character.Stat.Hp) return;

                    if (Data.SkillBase[sid].Properties1["heal"] != 0)
                    {
                        amount = Data.SkillBase[sid].Properties1["heal"];

                        // add the calculated amount
                        if (Target.Character.Stat.SecondHp + amount < Target.Character.Stat.Hp)
                            Target.Character.Stat.SecondHp += amount;
                        else if (Target.Character.Stat.SecondHp != Target.Character.Stat.Hp)
                            Target.Character.Stat.SecondHp += Target.Character.Stat.Hp - Target.Character.Stat.SecondHp;

                        if (UpdatePacket) Target.UpdateHp();
                    }
                    if (Data.SkillBase[sid].Properties2["heal"] != 0)
                    {
                        amount = Data.SkillBase[sid].Properties2["heal"];
                        amount = (Target.Character.Stat.Hp / 100) * amount;

                        // add the calculated amount
                        if (Target.Character.Stat.SecondHp + amount < Target.Character.Stat.Hp)
                            Target.Character.Stat.SecondHp += amount;
                        else if (Target.Character.Stat.SecondHp != Target.Character.Stat.Hp)
                            Target.Character.Stat.SecondHp += Target.Character.Stat.Hp - Target.Character.Stat.SecondHp;

                        if (UpdatePacket) Target.UpdateHp();
                    }
                    if (Data.SkillBase[sid].Properties3["heal"] != 0)
                    {
                        amount = Data.SkillBase[sid].Properties3["heal"];

                        // add the calculated amount
                        if (Target.Character.Stat.SecondMP + amount < Target.Character.Stat.Mp)
                            Target.Character.Stat.SecondMP += amount;
                        else if (Target.Character.Stat.SecondMP != Target.Character.Stat.Mp)
                            Target.Character.Stat.SecondMP += Target.Character.Stat.Mp - Target.Character.Stat.SecondMP;

                        if (UpdatePacket) Target.UpdateMp();

                    }
                    if (Data.SkillBase[sid].Properties3["heal"] != 0)
                    {
                        amount = Data.SkillBase[sid].Properties4["heal"];
                        amount = (Target.Character.Stat.Mp / 100) * amount;

                        // add the calculated amount
                        if (Target.Character.Stat.SecondMP + amount < Target.Character.Stat.Mp)
                            Target.Character.Stat.SecondMP += amount;
                        else if (Target.Character.Stat.SecondMP != Target.Character.Stat.Mp)
                            Target.Character.Stat.SecondMP += Target.Character.Stat.Mp - Target.Character.Stat.SecondMP;

                        if (UpdatePacket) Target.UpdateMp();
                    }
                }
                else
                {
                    //dunno....
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            
        }
        public void ChangeHitrate_hr(Systems Target, int slot, bool delete,bool UpdatePacket)
        {
            try
            {
                //AttackRate = HitRate ???
                int amount;
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1.ContainsKey("hr"))
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["hr"];


                    }
                }

                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangePhyAtk_E1SA(Systems Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                int amount;
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E1SA"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E1SA"];
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = (Target.Character.Stat.MinPhyAttack / 100) * amount;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = (Target.Character.Stat.MaxPhyAttack / 100) * amount;
                        Target.Character.Stat.MinPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                    }

                }
                else
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E1SA"] != 0)
                    {
                        Target.Character.Stat.MinPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = 0;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = 0;
                    }

                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangePhyAtk_E2SA(Systems Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                int amount;
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2SA"] != 0)
                    {
                        amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2SA"];
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = (Target.Character.Stat.MinPhyAttack / 100) * amount;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = (Target.Character.Stat.MaxPhyAttack / 100) * amount;
                        Target.Character.Stat.MinPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack += Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                    }

                }
                else
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2SA"] != 0)
                    {
                        Target.Character.Stat.MinPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack;
                        Target.Character.Stat.MaxPhyAttack -= Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack;
                        Target.Character.Action.Buff.UpdatedStats[slot].MinPhyAttack = 0;
                        Target.Character.Action.Buff.UpdatedStats[slot].MaxPhyAttack = 0;
                    }

                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }

        }
        public void ChangeHitRate_E2AH(Systems Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2AH"] != 0)
                    {
                        int amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2AH"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Hit = amount;
                        Target.Character.Stat.Hit += Target.Character.Action.Buff.UpdatedStats[slot].Hit;
                    }

                }
                else
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["E2AH"] != 0)
                    {
                        Target.Character.Stat.Hit -= Target.Character.Action.Buff.UpdatedStats[slot].Hit;
                        Target.Character.Action.Buff.UpdatedStats[slot].Hit = 0;
                    }
                }

                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }

        }      // setvaluek ( valószínű ) nem így leszneek 
        public void ChangeParry_terd(Systems Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["terd"] != 0)
                    {
                        int amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["terd"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Parry = amount;
                        Target.Character.Stat.Parry -= Target.Character.Action.Buff.UpdatedStats[slot].Parry;
                    }

                }
                else
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["terd"] != 0)
                    {
                        Target.Character.Stat.Parry += Target.Character.Action.Buff.UpdatedStats[slot].Parry;
                        Target.Character.Action.Buff.UpdatedStats[slot].Parry = 0;
                    }

                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeTargetHp_chcr(Systems Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["chcr"] != 0)
                    {
                        int amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["chcr"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Hp = amount;
                        Target.Character.Stat.Hp -= Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                        Target.Character.Stat.SecondHp -= Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                    }
                }

                else
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["chcr"] != 0)
                    {
                        Target.Character.Stat.Hp += Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                        Target.Character.Stat.SecondHp += Target.Character.Action.Buff.UpdatedStats[slot].Hp;
                        Target.Character.Action.Buff.UpdatedStats[slot].Hp = 0;
                    }
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeTargetHp_cmcr(Systems Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["cmcr"] != 0)
                    {
                        int amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["cmcr"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Mp = amount;
                        Target.Character.Stat.Mp -= Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                        Target.Character.Stat.SecondMP -= Target.Character.Action.Buff.UpdatedStats[slot].Mp;


                    }
                }

                else
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["cmcr"] != 0)
                    {
                        Target.Character.Stat.Mp += Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                        Target.Character.Stat.SecondMP += Target.Character.Action.Buff.UpdatedStats[slot].Mp;
                        Target.Character.Action.Buff.UpdatedStats[slot].Mp = 0;
                    }
                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }

        }
        public void ChangeDecAttkRate_thrd(Systems Target, int slot, bool delete, bool UpdatePacket)
        {
            try
            {
                if (!delete)
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["thrd"] != 0)
                    {
                        int amount = Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["thrd"];
                        Target.Character.Action.Buff.UpdatedStats[slot].Hit = amount;
                        Target.Character.Stat.Hit -= Target.Character.Action.Buff.UpdatedStats[slot].Hit;
                    }
                }
                else
                {
                    if (Data.SkillBase[Target.Character.Action.Buff.SkillID[slot]].Properties1["thrd"] != 0)
                    {
                        Target.Character.Stat.Hit += Target.Character.Action.Buff.UpdatedStats[slot].Hit;
                        Target.Character.Action.Buff.UpdatedStats[slot].Hit = 0;
                    }

                }
                if (UpdatePacket) Target.client.Send(Packet.PlayerStat(Target.Character));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }

        }
        public void HandleEffect()
        {

        }
        public void HandleSpecialBuff(int skillid) // theese attributes are not serializable
        {
            try
            {
                string series = Data.SkillBase[skillid].Series.Remove(Data.SkillBase[skillid].Series.Length - 2);
                switch (series)
                {
                    case "SKILL_OP_HARMONY":
                    case "SKILL_CH_WATER_HARMONY":

                        spez_obj so = new spez_obj();

                        so.Name = series;
                        so.ID = skillid;
                        so.spezType = 0x850;
                        so.Radius = Data.SkillBase[skillid].Distance / 10;
                        so.Ids = new Global.ID(Global.ID.IDS.Object);
                        so.UniqueID = so.Ids.GetUniqueID;

                        so.xSec = Character.Position.xSec;
                        so.ySec = Character.Position.ySec;
                        so.x = Character.Position.x;
                        so.z = Character.Position.z;
                        so.y = Character.Position.y;

                        Systems.SpecialObjects.Add(so);
                        so.SpawnMe(Data.SkillBase[skillid].Properties1["dura"]);

                        break;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }

        #endregion

       /*#region Handle Skill Attributes
        bool SkillHandle_Properties(Systems Target, string PropertiesName, bool UpdatePacket)
        {
            try
            {
                switch (PropertiesName)
                {
                    case "heal":
                        ChangeHeal(Target, slot, false, UpdatePacket);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SkillHandle_Properties() error..");
                deBug.Write(ex);
            }

            return false;
        }
        #endregion*/
    }
}
