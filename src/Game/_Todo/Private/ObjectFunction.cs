///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using DarkEmu_GameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;

namespace DarkEmu_GameServer
{
    public partial class obj
    {
		public Thread AttackHandle;
		public void FollowHim(Systems sys)
		{
			try
			{
				if (sys != null && this.LocalType == 1 && !sys.Character.State.Die/* && !sys.Character.Action.Check() */&& !sys.Character.Transport.Right)
				{
					if (!this.Busy && this.Agresif == 1 && this.Attacking == false)
					{
                        if (this.x >= (sys.Character.Position.x - 10) && this.x <= ((sys.Character.Position.x - 10) + 20) && this.y >= (sys.Character.Position.y - 10) && this.y <= ((sys.Character.Position.y - 10) + 20))
                        {
                            if (this.Walking)
                            {       //Notes / bugs :
                                    //- Monster skill attacks does not count amount of attacks for example if an attack strikes 2 times, onlye does 1 time damage. since its static. AttackHim()
                                    //- Player attack distance is really bad, needs timer to check monster location so attacking distance is more accurate. rly complex so navmesh here myb .. coz mob coordinate not apropriate need to check it
                                    //- Monsters agro needs max agro count, because having 100 on top of you is rediculous
                                    //- when player has to die (when mob attacks him) dont die at 0hp player needs to get one more hit to die :P

                                    this.Walking = false;
                                    StopMovement();

                                    Target = sys;
                                    AddAgroDmg(sys.Character.Information.UniqueID, 1);

                                    if (AttackHandle != null)
                                        if (AttackHandle.IsAlive)
                                            return;

                                    AttackHandle = new Thread(new ThreadStart(AttackMain));
                                    AttackHandle.Start();

                                    return;
                            }
                        }
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("FollowHim()::Error...");
				Systems.Debugger.Write(ex);
			}
		}
        public void GotoPlayer(character Player, double distance)
        {
            try
            {
                this.StopMovement();

                float farkx = Player.Position.x + 2; // later have to modify
                float farky = Player.Position.y;

                //Systems.aRound(ref Player.aRound, ref farkx, ref farky);


                this.xSec = (byte)((farkx / 192) + 135);
                this.ySec = (byte)((farky / 192) + 92);

                //don't follow the player into the town.
                /*bool inTown = Data.safeZone.Exists(
                    delegate(Global.region r)
                    {
                        if (r.SecX == this.xSec && r.SecY == this.ySec)
                        {
                            return true;
                        }
                        return false;
                    });*/

                Send(Packet.Movement(new DarkEmu_GameServer.Global.vektor(this.UniqueID,
                    (float)Formule.packetx((float)farkx, Player.Position.xSec),
                    (float)Player.Position.z,
                    (float)Formule.packety((float)farky, Player.Position.ySec),
                    this.xSec,
                    this.ySec)));

                /*if (inTown)
                {
                    StopAttackTimer();
                    this.Attacking = false;

                    this.GetDie = true;
                    this.Die = true;
                    this.DeSpawnMe();

                    return;
                }*/

                // Keep track of the current position of the mob.
                this.wx = farkx - this.x;
                this.wy = farky - this.y;
                // Calc for time if mob follows initalise speed info Run
                WalkingTime = (double)(distance / (this.SpeedRun * 0.0768)) * 1000.0;
                RecordedTime = WalkingTime;

                this.StartMovement((int)(WalkingTime / 10));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Follow Player Error: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void CheckEveryOne(object e)
        {
            try
            {
                lock (Systems.clients)
                {
                    for (int b = 0; b < Systems.clients.Count; b++)
                    {
                        try
                        {
                            if (this.Spawned(Systems.clients[b].Character.Information.UniqueID))
                            {
                                FollowHim(Systems.clients[b]);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("CheckEveryOne Error on index {1}/{2}: {0}", ex, b, Systems.clients.Count);
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Check all: {0}",ex);
                Systems.Debugger.Write(ex);
            }
        }   
        public void AttackStop()
        {
            try
            {
                this.Busy = false;
                this.Attacking = false;

                // mob start walking <-- have to parse this one
                //ChangeState(0, 3);

                //start auto walking
                if (this.AutoMovement) this.StartRunTimer((rnd.Next(4, 8) * 1000));
                else if (this.Agresif == 1) this.StartAgressiveTimer(1000);
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void AttackMain() // have to add this to thread pool
        {
            try
            {
                Systems sys;
                double distance;

                if (Agro != null) sys = (Systems)GetTarget();
                else { return; }
                //Print.Format("AttackMain()");
                if (bSleep) return;

                if (sys == null || Die || GetDie) { Attacking = false; return; }
                if (sys != null && !this.Spawned(sys.Character.Information.UniqueID)) { Attacking = false; return; }
                if (!sys.MonsterCheck(this.UniqueID)) sys.Character.Action.MonsterID.Add(this.UniqueID);
                if (!sys.Character.InGame) { Attacking = false; return; }

                if (this.AutoMovement) StopAutoRunTimer();
                else StopAgressiveTimer();

                this.Busy = true;
                this.Attacking = true;
                bool Hit = false;

                int AttackType = 0;
                int AttackDistance = 0;
                int AttackTime = 1000;

                int acount = this.UniqueID;

                //mob starts running
                ChangeState(1, 3);

                Stopwatch PursuitWatch = new Stopwatch();
                PursuitWatch.Start();

                // mob brain loop
                while (this.Attacking)
                {
                    if (sys == null || Die || GetDie) { AttackStop(); break; }
                    if (sys != null && !this.Spawned(sys.Character.Information.UniqueID)) { AttackStop(); break; }
                    if (!sys.Character.InGame) { AttackStop(); break; }

                    // make every new skill random
                    AttackType = Data.ObjectBase[this.ID].Skill[rnd.Next(0, Data.ObjectBase[this.ID].amountSkill)];
                    AttackDistance = Data.SkillBase[AttackType].Distance;
                    AttackTime = Data.SkillBase[AttackType].AttackTime;

                    distance = Formule.gamedistance((float)this.x, (float)this.y, sys.Character.Position.x, sys.Character.Position.y);

                    // mob's attack ranged 
                    if (AttackDistance != 0)
                    {
                        distance -= AttackDistance;
                        if(distance < 0) distance = 0;
                    }

                    #region Monster action switch
                    // stop pursuit coz player too far
                    if (distance > 19 /*&& Attack != null*/)
                    {
                        ChangeState(8, 1);
                        AttackStop();
                        break;
                    }
                    // player try to escape from mob's agro => go and pursuit the player 
                    else if ((distance > 3 && distance < 19) && Walking == false /*&& PursuitWatch.ElapsedMilliseconds >= 500*/)
                    {
                        //restrict the loop to call this branch so frequent
                        //PursuitWatch.Restart();
                        if (this.ID == 1979 || this.ID == 2101 || this.ID == 2124 || this.ID == 2111 || this.ID == 2112) return;
                        this.Walking = true;
                        GotoPlayer(sys.Character, distance);
                    }
                    else if (distance <= 3 && Walking == false)
                    {
                        Hit = true;
                        AttackHim(AttackType);
                    }
                    #endregion
                   
                    if (Hit) // wait for the attack animation
                    {
                        Thread.Sleep(AttackTime);
                        Hit = false;
                    }
                    else
                    {
                        int SleepTime = 300;
                        //magic
                        /*if (this.Walking)
                        {
                            if (this.RecordedTime < SleepTime)
                            {
                                SleepTime = (int)this.RecordedTime-50;
                                if (this.RecordedTime < 50) SleepTime = 50;
                            }
                        }*/
                        Thread.Sleep(SleepTime);
                    }
                }

                GC.Collect();
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
                Console.WriteLine("Attackmain error {0}", ex);
            }
        }

        public void AttackHim(int AttackType)
        {
            try
            {
                if (!Walking && Attacking && !bSleep)
                {

                    Systems sys = (Systems)GetTarget();

                    if (sys == null || Die || GetDie) 
                    { 
                        StopAttackTimer(); 
                        return; 
                    }
                    if (sys != null && !this.Spawned(sys.Character.Information.UniqueID)) 
                    { 
                        StopAttackTimer(); 
                        return; 
                    }

                    if (!sys.Character.InGame) 
                    { 
                        StopAttackTimer(); 
                        return; 
                    }
                    
                    byte NumberAttack = 1;

                    int p_dmg = 0;
                    byte status = 0, crit = 1;

                    PacketWriter Writer = new PacketWriter();
                    Writer.Create(Systems.SERVER_ACTION_DATA);
                    Writer.Byte(1);
                    Writer.Byte(2);
                    Writer.Byte(0x30);

                    Writer.DWord(AttackType);
                    Writer.DWord(this.UniqueID);

                    this.LastCasting = this.Ids.GetCastingID();

                    Writer.DWord(this.LastCasting);
                    Writer.DWord(sys.Character.Information.UniqueID);

                    Writer.Bool(true);
                    Writer.Byte(NumberAttack);
                    Writer.Byte(1);

                    Writer.DWord(sys.Character.Information.UniqueID);

                    for (byte n = 1; n <= NumberAttack; n++)
                    {
                        bool block = false;

                        if (sys.Character.Information.Item.sID != 0 && Data.ItemBase[sys.Character.Information.Item.sID].Class_D == 1)
                        {
                            if (sys.rnd.Next(25) < 10) block = true;
                        }
                        if (!block)
                        {
                            status = 0;
                            crit = 1;

                            p_dmg = (int)Formule.gamedamage(Data.SkillBase[AttackType].MaxAttack, 0, sys.Character.Stat.phy_Absorb, sys.Character.Stat.PhyDef, 50, Data.SkillBase[AttackType].MagPer);
                            p_dmg += rnd.Next(0, p_dmg.ToString().Length);
                            if (p_dmg <= 0) p_dmg = 1;

                            if (rnd.Next(20) > 15)
                            {
                                p_dmg *= 2;
                                crit = 2;
                            }

                            if (sys.Character.Stat.Absorb_mp > 0)
                            {
                                int static_dmg = (p_dmg * (100 - (int)sys.Character.Stat.Absorb_mp)) / 100;
                                sys.Character.Stat.SecondMP -= static_dmg;
                                if (sys.Character.Stat.SecondMP < 0) sys.Character.Stat.SecondMP = 0;
                                sys.UpdateMp();
                                p_dmg = static_dmg;
                            }

                            sys.Character.Stat.SecondHp -= p_dmg;

                            if (sys.Character.Stat.SecondHp <= 0)
                            {
                                sys.BuffAllClose();
                                status = 128;
                                sys.Character.Stat.SecondHp = 0;
                                sys.Character.State.Die = true;
                                sys.Character.State.DeadType = 1;

                                _agro agro = GetAgroClass(sys.Character.Information.UniqueID);
                                if (agro != null) Agro.Remove(agro);
                                DeleteTarget();
                                StopAttackTimer();
                                CheckAgro();

                                if (sys.Character.Action.nAttack) sys.StopAttackTimer();
                                else if (sys.Character.Action.sAttack || sys.Character.Action.sCasting) sys.StopSkillTimer();
                            }

                            sys.UpdateHp();

                            Writer.Byte(status);
                            Writer.Byte(crit);
                            Writer.DWord(p_dmg);
                            Writer.Byte(0);
                            Writer.Word(0);
                        }
                        else
                            Writer.Byte(2);
                    }
                    Send(Writer.GetBytes());
                    //Game.Effect.EffectMain(sys, AttackType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AttackHim {0} ", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void CheckAgro()
        {
            try
            {
                if (this.Agro.Count > 0)
                {
                    if (this.Attacking == false)
                    {
                        if (AttackHandle != null)
                            if (AttackHandle.IsAlive)
                                return;

                        AttackHandle = new Thread(new ThreadStart(AttackMain));
                        AttackHandle.Start();
                    }
                }
                else
                    StartRunTimer((rnd.Next(1, 2) * 1000));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void ChangeState(byte type, byte type2)
        {
            try
            {
                Send(Packet.StatePack(this.UniqueID, type, type2, false));
            }
            catch (Exception ex)
            {
                Console.WriteLine("State error: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void Send(byte[] buff)
        {
            try
            {
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            if (Systems.clients[i] != null && this.Spawned(Systems.clients[i].Character.Information.UniqueID))
                            {
                                if (!Systems.clients[i].Character.Spawning)
                                {
                                    Systems.clients[i].client.Send(buff);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Send Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send error: {0}", ex);
            }
        }
        public void SpawnMe()
        {
            try
            {
                if (this.Die) return;
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            Systems sys = Systems.clients[i];
                            if (this.x >= (sys.Character.Position.x - 50) && this.x <= ((sys.Character.Position.x - 50) + 100) && this.y >= (sys.Character.Position.y - 50) && this.y <= ((sys.Character.Position.y - 50) + 100) && this.Spawned(sys.Character.Information.UniqueID) == false)
                            {
                                this.Spawn.Add(sys.Character.Information.UniqueID);
                                sys.client.Send(Packet.ObjectSpawn(this));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Spawnme error + " + ex + "");
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
            
                if (this.AutoMovement) StartRunTimer(rnd.Next(500,2000));
            else if (this.Agresif == 1) StartAgressiveTimer(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SpawnMe error: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public byte DeBuffGetFreeSlot()
        {
            try
            {
                for (byte b = 0; b <= this.DeBuff.Effect.EffectID.Length - 1; b++)
                    if (this.DeBuff.Effect.EffectID[b] == 0) return b;
                
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return 255;
        }
        public void CheckEveryOne()
        {
            try
            {
                lock (Systems.clients)
                {
                    for (int b = 0; b < Systems.clients.Count; b++)
                    {
                        try
                        {
                            if (this.Spawned(Systems.clients[b].Character.Information.UniqueID))
                                FollowHim(Systems.clients[b]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Check all error + "+ex+"");
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Check all: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void DeSpawnMe()
        {
            try
            {
                StopAutoRunTimer();
                StopAgressiveTimer();
                if (this.Die)
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            Systems sys = Systems.clients[i];
                            if (Spawned(sys.Character.Information.UniqueID))
                            {
                                Spawn.Remove(sys.Character.Information.UniqueID);
                                sys.client.Send(Packet.ObjectDeSpawn(this.UniqueID));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("DeSpawnMe Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DespawnMe: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public List<Systems> GetRangePlayers(int dist)
        {
            try
            {
                List<Systems> Players = new List<Systems>();
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {

                        Systems s = Systems.clients[i];
                        //double distance = Formule.gamedistance((float)this.x, (float)this.y, s.Character.Position.x, s.Character.Position.y);
                        double distance = Formule.gamedistance(this, s.Character.Position);
                        if (distance <= dist)
                            Players.Add(s);
                    }
                    return Players;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return null;
        }
        public void reSpawn()
        {
            try
            {
                this.Agro = new List<_agro>();
                this.HP = Data.ObjectBase[ID].HP;
                this.Agresif = this.oldAgresif;
                this.Attacking = false;
                this.x = oX + rnd.Next(5,30);
                this.y = oY + rnd.Next(5,30);
                Systems.aRound(ref oX , ref oY, 1);
                List<Systems> Players = new List<Systems>();
                Players = GetRangePlayers(50);

                bool isSharePartyInRange = Players.Exists(
                delegate(Systems s)
                {
                    bool retValue = false;

                    // player has exp/item share pt?
                    if (s.Character.Network.Party != null)
                        if (
                        s.Character.Network.Party.Type == (byte)Systems.PartyTypes.EXPSHARE ||
                        s.Character.Network.Party.Type == (byte)Systems.PartyTypes.EXPSHARE_NO_PERMISSION ||
                        s.Character.Network.Party.Type == (byte)Systems.PartyTypes.FULLSHARE ||
                        s.Character.Network.Party.Type == (byte)Systems.PartyTypes.FULLSHARE_NO_PERMISSION ||
                        s.Character.Network.Party.Type == (byte)Systems.PartyTypes.ITEMSHARE ||
                        s.Character.Network.Party.Type == (byte)Systems.PartyTypes.ITEMSHARE_NO_PERMISSION
                        )
                        {
                            // at least 2 players in range
                            bool isInRange = s.Character.Network.Party.Members.Exists(
                            delegate(int m)
                            {
                                // if me -> false
                                if (m == s.Character.Information.UniqueID) return false;

                                Systems ptmate = Systems.GetPlayer(m);

                                if (Players.Exists(sys => (sys.Character.Information.UniqueID == ptmate.Character.Information.UniqueID)))
                                    return true;
                                else
                                    return false;
                            }
                            );
                            if (isInRange) retValue = true;
                        }

                    return retValue;
                }
                );

                if (!isSharePartyInRange)
                    this.Type = Systems.RandomType(Data.ObjectBase[this.ID].Level, ref this.Kat, false, ref this.Agresif);
                else
                    this.Type = Systems.RandomType(Data.ObjectBase[this.ID].Level, ref this.Kat, true, ref this.Agresif);

                switch (this.Type)
                {
                    case 1:
                        this.Agresif = 1;
                        this.HP *= 2;
                        break;
                    case 4:
                        this.HP *= 20;
                        this.Agresif = 1;
                        break;
                   case 5:
                        this.HP *= 100;
                        this.Agresif = 1;
                        break;
                    case 16:
                        this.HP *= 10;
                        break;
                    case 17:
                        this.HP *= 20;
                        this.Agresif = 1;
                        break;
                    case 20:
                        this.HP *= 210;
                        this.Agresif = 1;
                        break;
                }

                this.GetDie = false;
                this.Die = false;
                this.Busy = false;

                this.SpawnMe();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Respawn monster: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void RandomMonster(int sID, byte randomTYPE)
        {
            try
            {
                obj o = new obj();
                o.ID = sID;
                o.Ids = new Global.ID(Global.ID.IDS.Object);
                o.UniqueID = o.Ids.GetUniqueID;
                o.x = this.x;
                o.z = this.z;
                o.y = this.y;
                o.oY = this.oY;
                o.oX = this.oX;
                //Systems.aRound(ref o.oX, ref o.oY, 1);

                o.xSec = this.xSec;
                o.ySec = this.ySec;

                o.AutoMovement = this.AutoMovement;
                if (ID == 1979 || ID == 2101 || ID == 2124 || ID == 2111 || ID == 2112) o.AutoMovement = false;
                o.AutoSpawn = true;
                o.Move = 1;

                o.HP = Data.ObjectBase[o.ID].HP;
                o.SpeedWalk = Data.ObjectBase[o.ID].SpeedWalk;
                o.SpeedRun = Data.ObjectBase[o.ID].SpeedRun;
                o.SpeedZerk = Data.ObjectBase[o.ID].SpeedZerk;
                o.Agresif = 0;
                o.LocalType = 1;
                o.State = 2;
                o.Kat = 1;
                o.Agro = new List<_agro>();
                o.spawnOran = 0;

                if (randomTYPE == 0) // Standart
                {
                    o.Type = Systems.RandomType(Data.ObjectBase[o.ID].Level, ref this.Kat, false, ref o.Agresif);
                    if (o.Type == 1) o.Agresif = 1;
                    if (Data.ObjectBase[o.ID].Agresif == 1)
                    {
                        o.Agresif = 1;
                    }
                    o.HP *= this.Kat;
                }
                else
                {
                    if (randomTYPE == 6)
                        o.HP *= 4;
                    else if (randomTYPE == 4)
                        o.HP *= 20;
                    else if (randomTYPE == 1)
                        o.HP *= 2;
                    else if (randomTYPE == 16)
                        o.HP *= 10;
                    else if (randomTYPE == 17)
                        o.HP *= 20;
                    else if (randomTYPE == 20)
                        o.HP *= 210;
                    o.AutoSpawn = false;
                    o.Type = randomTYPE;
                    o.Agresif = 1;
                }

                o.SpawnMe();
                Systems.Objects.Add(o);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Respawn monster: {0}",ex);
                Systems.Debugger.Write(ex);
            }
        }
        public List<int> GetLevelItem(byte level)
        {
            try
            {
                List<int> item = new List<int>();
                for (int i = 0; i < Data.ItemBase.Length; i++)
                {
                    if (Data.ItemBase[i] != null)
                    {
                        string namecheck = Data.ObjectBase[ID].Name;
                        {
                            if (Data.ItemBase[i].Level != 0 && Data.ItemBase[i].Level >= level - 4 && Data.ItemBase[i].Level <= (level + 4) && Data.ItemBase[i].SOX == 0 && GetItemType(Data.ItemBase[i].ID) == 0)
                            {
                                if (i != 0)
                                    item.Add(i);
                            }
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return null;
        }
        public List<int> GetBlueRandom()
        {
            try
            {
                List<int> item = new List<int>();
                for (int i = 0; i < Data.MagicOptions.Count; i++)
                {
                    if (Data.ItemBase[i] != null)
                    {
                        {
                            if (i != 0)
                                item.Add(i);
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return null;
        }
        public List<int> GetMaterials(byte level)
        {
            try
            {
                List<int> item = new List<int>();

                for (int i = 0; i < Data.ItemBase.Length; i++)
                {
                    if (Data.ItemBase[i] != null)
                    {
                        if (Data.ItemBase[i].Class_C == 20 && Data.ItemBase[i].Level >= level - 1 && Data.ItemBase[i].Level <= (level + 1) && Data.ItemBase[i].Class_D == 3 && Data.ItemBase[i].Item_Mall_Type == 0 && Data.ItemBase[i].Race == 3)//&& Data.ItemBase[i].Class_B == 11)//(Data.ItemBase[i].Name.Contains("ETC"))
                        {
                            item.Add(i);
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return null;
        }
        public List<int> GetPotions(byte level)
        {
            try
            {
                List<int> item = new List<int>();

                for (int i = 0; i < Data.ItemBase.Length; i++)
                {
                    if (Data.ItemBase[i] != null)
                    {
                        if (Data.ItemBase[i].Etctype == Global.item_database.EtcType.HP_POTION || Data.ItemBase[i].Etctype == Global.item_database.EtcType.MP_POTION || Data.ItemBase[i].Etctype == Global.item_database.EtcType.VIGOR_POTION)
                        {
                            item.Add(i);
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return null;
        }
        public List<int> GetElixir(byte level)
        {
            try
            {
                List<int> item = new List<int>();

                for (int i = 0; i < Data.ItemBase.Length; i++)
                {
                    if (Data.ItemBase[i] != null)
                    {
                        if (Data.ItemBase[i].Etctype == Global.item_database.EtcType.ELIXIR)
                        {
                            item.Add(i);
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return null;
        }
        public List<int> GetLevelItemSOX(byte level)
        {
            try
            {
                List<int> item = new List<int>();
                for (int i = 0; i < Data.ItemBase.Length; i++)
                {
                    if (Data.ItemBase[i] != null)
                    {
                        if (Data.ItemBase[i].Level >= level - 4 && Data.ItemBase[i].Level <= (level + 4) && GetItemType(Data.ItemBase[i].ID) == 0 && Data.ItemBase[i].SOX == 2)
                        {
                            if (i != 0)
                                item.Add(i);
                        }
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return null;
        }
        public static byte GetItemType(int id)
        {
            try
            {
                if (Data.ItemBase[id].Class_D == 1) return 0;
                else if (Data.ItemBase[id].Class_D == 3) return 1;
                else if (Data.ItemBase[id].Class_D == 4) return 4;
                else if (Data.ItemBase[id].Class_D == 8) return 8;
                else if (Data.ItemBase[id].Class_D == 9) return 9;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return 255;
        }
        public void SetExperience()
        {
            try
            {
                //Check death
                if (this.GetDie)
                {
                    //Set default parameters
                    int percent = 0;
                    //Exp information of the monster
                    long exp = Data.ObjectBase[this.ID].Exp;
                    //Sp information
                    long sp = 119;
                    //Level default false
                    bool level = false;
                    //Main perfect information
                    byte mainpercent = 100;
                    //Set quick info to player
                    Systems player;
                    //If Agro list isnt empty
                    if (Agro != null)
                        //Check how many have agro state
                        for (byte b = 0; b < Agro.Count; b++)
                        {
                            //Make sure the player isnt null
                            if (Agro[b].playerID != 0)
                            {
                                //Get player information
                                player = Systems.GetPlayer(Agro[b].playerID);
                                //Again double check to make sure the called information isnt null
                                if (player != null)
                                {
                                    //Set definition for stat attributes
                                    short stat = player.Character.Information.Attributes;
                                    //Calculate the damage dealt of the player and divide it by the monster type and hp total.
                                    percent = Agro[b].playerDMD * 100 / Data.ObjectBase[this.ID].HP * this.Kat;
                                    //If the % is higher or equals 100%, we reset our % to normal
                                    if (percent >= mainpercent) 
                                        percent = mainpercent;
                                    //Set default bool info for level
                                    level = false;
                                    //Make sure our % isnt 0 , so we dont do none needed actions
                                    if (mainpercent > 0)
                                    {
                                        //If the player is currently in a party
                                        if (player.Character.Network.Party != null)
                                        {
                                            //Set definition for the party info
                                            party ept = player.Character.Network.Party;
                                            //Set party type information from our party
                                            Systems.PartyTypes ptType = (Systems.PartyTypes)ept.Type;
                                            //If its a non shared party
                                            if (ptType == Systems.PartyTypes.NONSHARE_NO_PERMISSION || ptType == Systems.PartyTypes.NONSHARE)
                                            {
                                                //Get gap information of the players mastery level
                                                int gap = Math.Abs(player.Character.Information.Level - player.MasteryGetBigLevel) * 10;
                                                if (gap >= 90) gap = 90;

                                                mainpercent -= (byte)percent;
                                                //Premium tickets should be added here to increase the exp
                                                exp *= Systems.Rate.Xp;
                                                exp -= (exp * ((Math.Abs(Data.ObjectBase[this.ID].Level - player.Character.Information.Level) - Math.Abs(player.Character.Information.Level - player.MasteryGetBigLevel)) * 10)) / 100;
                                                // kat == the type of the mob. this is the multiplier of the mob's HP and the amount of exp what the mob gives.
                                                byte monstertype = this.Kat;

                                                if (monstertype == 20)
                                                    monstertype = 25;

                                                exp *= monstertype;
                                                exp = (exp * percent) / 100;
                                                exp = (exp * (100 - gap)) / 100;

                                                if (Math.Abs(Data.ObjectBase[this.ID].Level - player.Character.Information.Level) < 10)
                                                {
                                                    sp = (sp * (100 + gap)) / 100;
                                                    sp *= monstertype;
                                                    sp *= Systems.Rate.Sp;
                                                }
                                                else sp = 10;
                                                if (exp <= 1) exp = 1;
                                                if (sp <= 1) sp = 10;
                                                //simple non-share pt.
                                                if (ept.Members.Count > 1)
                                                {

                                                    if (this.Type == 0) //normal mob
                                                    {
                                                        exp *= 1;
                                                        sp *= 1;
                                                    }
                                                    else if (this.Type == 1) //champion
                                                    {
                                                        exp *= 2;
                                                        sp *= 2;
                                                    }
                                                    else if (this.Type == 3)// Unique
                                                    {
                                                        exp *= 7;
                                                        sp *= 7;
                                                    }
                                                    else if (this.Type == 4) //giant
                                                    {
                                                        exp *= 3;
                                                        sp *= 3;
                                                    }
                                                    else if (this.Type == 6) //elite
                                                    {
                                                        exp *= 2;
                                                        sp *= 2;
                                                    }
                                                    else if (this.Type == 16) //normal ptmob
                                                    {
                                                        exp *= 3;
                                                        sp *= 3;
                                                    }
                                                    else if (this.Type == 17) //champion ptmob
                                                    {
                                                        exp *= 4;
                                                        sp *= 4;
                                                    }
                                                    else if (this.Type == 20)//party giant
                                                    {
                                                        exp *= 6;
                                                        sp *= 6;
                                                    }

                                                    switch (ept.Members.Count)
                                                    {
                                                        case 2:
                                                            exp += (long)(exp * 0.05);
                                                            sp += (long)(sp * 0.05);
                                                            break;
                                                        case 3:
                                                            exp += (long)(exp * 0.05) * 2;
                                                            sp += (long)(sp * 0.05) * 2;
                                                            break;
                                                        case 4:
                                                            exp += (long)(exp * 0.05) * 3;
                                                            sp += (long)(sp * 0.05) * 3;
                                                            break;
                                                    }
                                                }
                                            }

                                            if (ptType == Systems.PartyTypes.EXPSHARE_NO_PERMISSION || ptType == Systems.PartyTypes.EXPSHARE || ptType == Systems.PartyTypes.FULLSHARE || ptType == Systems.PartyTypes.FULLSHARE_NO_PERMISSION)
                                            {
                                                if (ept.Members.Count > 1)
                                                {

                                                    if (this.Type == 0) //normal mob
                                                    {
                                                        exp *= 1;
                                                        sp *= 1;
                                                    }
                                                    else if (this.Type == 1) //champion
                                                    {
                                                        exp *= 2;
                                                        sp *= 2;
                                                    }
                                                    else if (this.Type == 3)// Unique
                                                    {
                                                        exp *= 7;
                                                        sp *= 7;
                                                    }
                                                    else if (this.Type == 4) //giant
                                                    {
                                                        exp *= 3;
                                                        sp *= 3;
                                                    }
                                                    else if (this.Type == 6) //elite
                                                    {
                                                        exp *= 2;
                                                        sp *= 2;
                                                    }
                                                    else if (this.Type == 16) //normal ptmob
                                                    {
                                                        exp *= 3;
                                                        sp *= 3;
                                                    }
                                                    else if (this.Type == 17) //champion ptmob
                                                    {
                                                        exp *= 4;
                                                        sp *= 4;
                                                    }
                                                    else if (this.Type == 20)//party giant
                                                    {
                                                        exp *= 6;
                                                        sp *= 6;
                                                    }
                                                    CalcSharedPartyExpSp(Data.ObjectBase[this.ID].Exp, ept, player, ref exp);
                                                }
                                            }
                                        }
                                        //Player not in party
                                        else
                                        {
                                            int gap = Math.Abs(player.Character.Information.Level - player.MasteryGetBigLevel) * 10;
                                            if (gap >= 90) gap = 90;

                                            mainpercent -= (byte)percent;

                                            if (player.Character.Information.Level != Data.ObjectBase[this.ID].Level)
                                                exp -= (exp * ((Math.Abs(Data.ObjectBase[this.ID].Level - player.Character.Information.Level) - Math.Abs(player.Character.Information.Level - player.MasteryGetBigLevel)) * 10)) / 100;

                                            if (this.Type == 0) //normal mob
                                            {
                                                exp *= 1;
                                                sp *= 1;
                                            }
                                            else if (this.Type == 1) //champion
                                            {
                                                exp *= 2;
                                                sp *= 2;
                                            }
                                            else if (this.Type == 3)// Unique
                                            {
                                                exp *= 7;
                                                sp *= 7;
                                            }
                                            else if (this.Type == 4) //giant
                                            {
                                                exp *= 3;
                                                sp *= 3;
                                            }
                                            else if (this.Type == 6) //elite
                                            {
                                                exp *= 2;
                                                sp *= 2;
                                            }
                                            else if (this.Type == 16) //normal ptmob
                                            {
                                                exp *= 3;
                                                sp *= 3;
                                            }
                                            else if (this.Type == 17) //champion ptmob
                                            {
                                                exp *= 4;
                                                sp *= 4;
                                            }
                                            else if (this.Type == 20)//party giant
                                            {
                                                exp *= 6;
                                                sp *= 6;
                                            }
                                            exp = (exp * percent) / 100;
                                            exp = (exp * (97 + gap)) / 100;

                                            if (player.Character.Information.Level == 140 && player.Character.Information.XP >= 130527554553)
                                            {
                                                exp = 0;
                                            }

                                            if (Math.Abs(Data.ObjectBase[this.ID].Level - player.Character.Information.Level) < 10)
                                            {
                                                int gaplevel = Data.ObjectBase[this.ID].Level - player.Character.Information.Level;
                                                sp = (sp * (100 + gaplevel)) / 100;
                                                sp *= this.Kat;
                                                sp *= Systems.Rate.Sp;

                                                exp = (exp * (100 + gaplevel)) / 100;
                                                exp *= this.Kat;
                                                exp *= Systems.Rate.Xp;
                                            }
                                            else
                                            {
                                                exp = 10;
                                                sp = 10;
                                            }

                                            if (exp <= 1) exp = 1;
                                            if (sp <= 1) sp = 10;
                                        }
                                        player.Character.Information.XP += exp;

                                        while (player.Character.Information.XP >= Data.LevelData[player.Character.Information.Level])
                                        {
                                            player.Character.Information.XP -= Data.LevelData[player.Character.Information.Level];
                                            if (player.Character.Information.XP < 1) player.Character.Information.XP = 0;
                                            stat += 3;
                                            player.Character.Information.Attributes += 3;
                                            player.Character.Information.Level++;
                                            player.Character.Stat.Intelligence++;
                                            player.Character.Stat.Strength++;
                                            player.UpdateIntelligenceInfo(1);
                                            player.UpdateStrengthInfo(1);
                                            player.SetStat();
                                            level = true;
                                        }


                                        SetSp(player, sp);
                                        player.Character.Network.Guild.LastDonate += 2;
                                        if (player.Character.Network.Guild.Guildid != 0)
                                        {
                                            if (Math.Abs(player.Character.Network.Guild.LastDonate - player.Character.Network.Guild.DonateGP) >= 10)
                                            {
                                                int gpinformation = rnd.Next(1, 9);//Need to make formula
                                                Systems.MsSQL.UpdateData("UPDATE guild_members SET guild_points='" + gpinformation + "' WHERE guild_member_id='" + player.Character.Information.CharacterID + "'");
                                                player.Character.Network.Guild.LastDonate = player.Character.Network.Guild.DonateGP;
                                                //Reload information
                                                player.LoadPlayerGuildInfo(false);
                                                // set new amount to every guild members guild class
                                                foreach (int m in player.Character.Network.Guild.Members)
                                                {
                                                    if (m != 0)
                                                    {
                                                        Systems gmember = Systems.GetPlayerMainid(m);
                                                        if (gmember != null)
                                                        {
                                                            gmember.client.Send(Packet.GuildUpdate(player.Character, 9, 0, 0, 0));
                                                        }
                                                    }
                                                }
                                                player.Character.Network.Guild.LastDonate = 0;
                                            }
                                        }
                                        //Check pet level information
                                        if (player.Character.Attackpet.Active)
                                        {
                                            while (player.Character.Attackpet.Details.exp >= Data.LevelData[player.Character.Attackpet.Details.exp])
                                            {
                                                player.Character.Attackpet.Details.exp -= Data.LevelData[player.Character.Attackpet.Details.Level];
                                                if (player.Character.Attackpet.Details.exp < 1)
                                                    player.Character.Attackpet.Details.exp = 0;
                                                /*
                                                stat += 3;
                                                player.Character.Information.Attributes += 3;
                                                player.Character.Information.Level++;
                                                player.Character.Stat.Intelligence++;
                                                player.Character.Stat.Strength++;
                                                player.UpdateIntelligenceInfo(1);
                                                player.UpdateStrengthInfo(1);
                                                player.SetStat();
                                                 */
                                                level = true;
                                            }
                                        }

                                        if (level)
                                        {
                                            if (player.Character.Network.Guild.Guildid != 0)
                                            {
                                                if (Math.Abs(player.Character.Network.Guild.LastDonate - player.Character.Network.Guild.DonateGP) >= 10)
                                                {
                                                    int gpinformation = rnd.Next(1, 9);//Need to make formula
                                                    Systems.MsSQL.UpdateData("UPDATE guild_members SET guild_points='" + gpinformation + "' WHERE guild_member_id='" + player.Character.Information.CharacterID + "'");
                                                    player.Character.Network.Guild.LastDonate = player.Character.Network.Guild.DonateGP;
                                                    //Reload information
                                                    player.LoadPlayerGuildInfo(false);
                                                    // set new amount to every guild members guild class
                                                    foreach (int m in player.Character.Network.Guild.Members)
                                                    {
                                                        if (m != 0)
                                                        {
                                                            Systems gmember = Systems.GetPlayerMainid(m);
                                                            if (gmember != null)
                                                            {
                                                                gmember.client.Send(Packet.GuildUpdate(player.Character, 9, 0, 0, 0));
                                                            }
                                                        }
                                                    }
                                                    player.Character.Network.Guild.LastDonate = 0;
                                                }
                                            }

                                            if (player.Character.Attackpet.Active)
                                            {
                                                //test
                                                player.Send(Packet.Player_LevelUpEffect(player.Character.Attackpet.Details.UniqueID));
                                                player.client.Send(Packet.Player_getExp(player.Character.Attackpet.Details.UniqueID, exp, sp, stat));

                                            }
                                            if (player.Character.Information.Level == 110 && player.Character.Information.XP >= 4000000000)
                                            {
                                                exp = 0;
                                            }
                                            player.Character.Stat.SecondHp = player.Character.Stat.Hp;
                                            player.Character.Stat.SecondMP = player.Character.Stat.Mp;
                                            player.UpdateHp();
                                            player.UpdateMp();
                                            player.Send(Packet.Player_LevelUpEffect(player.Character.Information.UniqueID));
                                            player.client.Send(Packet.Player_getExp(player.Character.Action.Target, exp, sp, stat));
                                            player.SavePlayerInfo();
                                        }
                                        else
                                        {
                                            if (player.Character.Network.Guild.Guildid != 0)
                                            {
                                                if (Math.Abs(player.Character.Network.Guild.LastDonate - player.Character.Network.Guild.DonateGP) >= 10)
                                                {
                                                    int gpinformation = rnd.Next(1, 9);//Need to make formula
                                                    Systems.MsSQL.UpdateData("UPDATE guild_members SET guild_points='" + gpinformation + "' WHERE guild_member_id='" + player.Character.Information.CharacterID + "'");
                                                    player.Character.Network.Guild.LastDonate = player.Character.Network.Guild.DonateGP;
                                                    //Reload information
                                                    player.LoadPlayerGuildInfo(false);
                                                    // set new amount to every guild members guild class
                                                    foreach (int m in player.Character.Network.Guild.Members)
                                                    {
                                                        if (m != 0)
                                                        {
                                                            Systems gmember = Systems.GetPlayerMainid(m);
                                                            if (gmember != null)
                                                            {
                                                                gmember.client.Send(Packet.GuildUpdate(player.Character, 9, 0, 0, 0));
                                                            }
                                                        }
                                                    }
                                                    player.Character.Network.Guild.LastDonate = 0;
                                                }
                                            }
                                            if (player.Character.Information.Level == 110 && player.Character.Information.XP >= 4000000000)
                                            {
                                                exp = 0;
                                            }
                                            //Player experience
                                            player.client.Send(Packet.Player_getExp(player.Character.Action.Target, exp, sp, 0));
                                            player.SavePlayerExperince();

                                            
                                            // Attack pet experience
                                            if (player.Character.Attackpet.Active)
                                            {bool petlevel;
                                                player.Character.Attackpet.Details.exp += exp * 2;

                                                while (player.Character.Attackpet.Details.exp >= Data.LevelData[player.Character.Attackpet.Details.exp])
                                                {
                                                    player.Character.Attackpet.Details.exp -= Data.LevelData[player.Character.Attackpet.Details.Level];
                                                    if (player.Character.Attackpet.Details.exp < 1)
                                                        player.Character.Attackpet.Details.exp = 0;
                                                    petlevel = true;
                                                }
                                                
                                                // Add exp
                                                player.client.Send(Packet.PetSpawn(player.Character.Attackpet.Details.UniqueID, 3, player.Character.Attackpet.Details));
                                                // Save pet exp
                                                player.SaveAttackPetExp();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetExperience :error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void CalcSharedPartyExpSp(int paramexp, party pt, Systems targetplayer, ref long outexp)
        {
            try
            {
                byte mlv = Data.ObjectBase[this.ID].Level;
                // party average
                int k = 0;
                List<int> NearbyMembers = new List<int>(9);
                if (pt.Members.Count != 0)
                {
                    double playerDist;
                    foreach (int memb in pt.Members)
                    {
                        Systems i = Systems.GetPlayer(memb);
                        //playerDist = Formule.gamedistance(targetplayer.Character.Position.x, targetplayer.Character.Position.y, i.Character.Position.x, i.Character.Position.y);
                        playerDist = Formule.gamedistance(targetplayer.Character.Position, i.Character.Position);
                        if (playerDist <= 75)
                        {
                            NearbyMembers.Add(i.Character.Information.UniqueID);
                        }
                    }
                    foreach (int l in NearbyMembers)
                    {
                        k += Systems.GetPlayer(l).Character.Information.Level;
                    }
                    k = (int)(k / pt.Members.Count);
                    //k = Ã¡tlag.
                    foreach (int mem in NearbyMembers)
                    {

                        Systems ch = Systems.GetPlayer(mem);
                        int ptsp = 97;
                        //This isn't the right formula. TODO: We must find the right one!
                        int ptexp = (int)((((paramexp / mlv) + (mlv - k)) * mlv) / k) * ch.Character.Information.Level;
                        ptexp *= Systems.Rate.Xp;
                        byte kat = this.Kat;
                        if (kat == 20) kat = 25;
                        ptexp *= kat; //we multiply the exp according to type of the mob.
                        //TODO: premium ticket
                        //gap:
                        ptexp -= (ptexp * (ch.Character.Information.Level) - Math.Abs(ch.Character.Information.Level - ch.MasteryGetBigLevel)) * 10 / 100;
                        if (ch.Character.Information.Level == 110 && ch.Character.Information.XP >= 4000000000) ptexp = 0;
                        //we calculate the amount of sp:
                        if (Math.Abs(Data.ObjectBase[this.ID].Level - k) < 10)
                        {
                            int gap = Math.Abs(ch.Character.Information.Level - ch.MasteryGetBigLevel) * 10;
                            if (gap >= 90) gap = 90;
                            ptsp = (ptsp * (100 + gap)) / k; //Instead of 100 I share with the avareage of the party, so we get a proportionate number.
                            ptsp *= kat;
                            ptsp *= Systems.Rate.Sp;
                        }
                        //Send total to all in party (Set exp from formula)
                        SetPartyMemberExp(ch, (long)ptexp, ch.Character.Information.Attributes, (long)ptsp);
                        SetSp(ch, (long)ptsp);
                        if (ch.Character.Network.Guild.Guildid != 0)
                        {
                            if (Math.Abs(ch.Character.Network.Guild.LastDonate - ch.Character.Network.Guild.DonateGP) >= 10)
                            {
                                Systems.MsSQL.UpdateData("UPDATE guild_members SET guild_points='" + ch.Character.Network.Guild.DonateGP + "' WHERE guild_member_id='" + ch.Character.Information.CharacterID + "'");
                                ch.Character.Network.Guild.LastDonate = ch.Character.Network.Guild.DonateGP;

                                // set new amount to every guild members guild class
                                foreach (int m in ch.Character.Network.Guild.Members)
                                {
                                    int targetindex = ch.Character.Network.Guild.MembersInfo.FindIndex(i => i.MemberID == m);
                                    if (ch.Character.Network.Guild.MembersInfo[targetindex].Online)
                                    {
                                        Systems sys = Systems.GetPlayer(m);

                                        // here comes the messy way 
                                        Global.guild_player mygp = new Global.guild_player();
                                        int myindex = 0;
                                        foreach (Global.guild_player gp in sys.Character.Network.Guild.MembersInfo)
                                        {
                                            if (gp.MemberID == ch.Character.Information.CharacterID)
                                            {
                                                mygp = gp;
                                                mygp.DonateGP = ch.Character.Network.Guild.DonateGP;
                                                break;
                                            }
                                            myindex++;
                                        }
                                        sys.Character.Network.Guild.MembersInfo[myindex] = mygp;
                                    }
                                }

                                ch.Character.Network.Guild.Send(Packet.GuildUpdate(ch.Character, 9, 0, 0, 0));
                            }
                        }
                        outexp = ptexp;
                    }
                }
                else return;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public static void SetPartyMemberExp(Systems ch, long expamount, short stat, long sp)
        {
            try
            {
                ch.Character.Information.XP += expamount;
                bool level = false;
                while (ch.Character.Information.XP >= Data.LevelData[ch.Character.Information.Level])
                {
                    ch.Character.Information.XP -= Data.LevelData[ch.Character.Information.Level];
                    if (ch.Character.Information.XP < 1) ch.Character.Information.XP = 0;
                    stat += 3;
                    ch.Character.Information.Attributes += 3;
                    ch.Character.Information.Level++;
                    ch.Character.Stat.Intelligence++;
                    ch.Character.Stat.Strength++;
                    ch.UpdateIntelligenceInfo(1);
                    ch.UpdateStrengthInfo(1);
                    ch.SetStat();
                    level = true;
                }
                if (level)
                {
                    if (ch.Character.Network.Guild.Guildid != 0)
                    {
                        // 1 question again where we  set info to databse at lvlup? ah got it :)
                        ch.Character.Network.Guild.Send(Packet.GuildUpdate(ch.Character, 8, 0, 0, 0));

                        // set new amount to every guild members guild class
                        foreach (int m in ch.Character.Network.Guild.Members)
                        {
                            int targetindex = ch.Character.Network.Guild.MembersInfo.FindIndex(i => i.MemberID == m);
                            if (ch.Character.Network.Guild.MembersInfo[targetindex].Online)
                            {
                                Systems sys = Systems.GetPlayer(m);

                                // here comes the messy way 
                                Global.guild_player mygp = new Global.guild_player();
                                int myindex = 0;
                                foreach (Global.guild_player gp in sys.Character.Network.Guild.MembersInfo)
                                {
                                    if (gp.MemberID == ch.Character.Information.CharacterID)
                                    {
                                        mygp = gp;
                                        mygp.Level = ch.Character.Information.Level;
                                        break;
                                    }
                                    myindex++;
                                }
                                sys.Character.Network.Guild.MembersInfo[myindex] = mygp;
                            }
                        }
                    }
                    ch.Send(Packet.Player_LevelUpEffect(ch.Character.Information.UniqueID));
                    ch.client.Send(Packet.Player_getExp(ch.Character.Information.UniqueID, expamount, sp, stat));
                    ch.SavePlayerInfo();
                }
                else
                {
                    ch.client.Send(Packet.Player_getExp(ch.Character.Information.UniqueID, expamount, sp, 0));
                    ch.SavePlayerExperince();
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public static void SetSp(Systems sys, long sp)
        {
            try
            {
                sys.Character.Information.SkillPoint += (((int)sp + sys.Character.Information.SpBar) / 400);
                sys.Character.Information.SpBar = (((int)sp + sys.Character.Information.SpBar) % 400);
                sys.client.Send(Packet.InfoUpdate(2, sys.Character.Information.SkillPoint, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Set Sp Error: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void CheckUnique()
        {
            try
            {
                if (this.Type == 3)
                    if (this.ID == 1954 || this.ID == 5871 || this.ID == 1982 || this.ID == 2002 || this.ID == 3810 || this.ID == 3875 || this.ID == 14538)
                    {
                        int yuzde = ((this.HP * 100) / Data.ObjectBase[this.ID].HP);
                        int[] bs = Systems.GetEliteIds(this.ID);
                        if (yuzde > 99)
                        {
                            if (!guard[0])
                                for (byte b = 0; b <= 8; b++)
                                {
                                    if (bs[b] != 0)
                                        RandomMonster((int)bs[b], 1);
                                    guard[0] = true;
                                }
                        }
                        else if (yuzde < 80 && yuzde > 70)
                        {
                            if (!guard[1])
                                for (byte b = 0; b <= 8; b++)
                                {
                                    if (bs[b] != 0)
                                        RandomMonster((int)bs[b], 6);
                                    guard[1] = true;
                                }
                        }
                        else if (yuzde < 60 && yuzde > 50)
                        {
                            if (!guard[2])
                                for (byte b = 0; b <= 8; b++)
                                {
                                    if (bs[b] != 0)
                                        RandomMonster((int)bs[b], 4);
                                    guard[2] = true;
                                }
                        }
                    }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void CheckUnique(Systems s)
        {
            try
            {
                if (this.Type == 3)
                {
                    Systems.SendAll(Packet.Unique_Data(6, (int)this.ID, s.Character.Information.Name));
                    #region Tiger Girl
                        if (this.ID == 1954)
                        {
                            DarkEmu_GameServer.GlobalUnique.Tiger = false;
                        }
                    #endregion
                    #region Cerberus
                        if (this.ID == 5871)
                        {
                            DarkEmu_GameServer.GlobalUnique.Cerb = false;
                        }
                    #endregion
                    #region Captain ivy
                        if (this.ID == 14778)
                        {
                            DarkEmu_GameServer.GlobalUnique.Ivy = false;
                        }
                    #endregion
                    #region Uruchi
                        if (this.ID == 1982)
                        {
                            DarkEmu_GameServer.GlobalUnique.Uri = false;
                        }
                    #endregion
                    #region Isyutaru
                        if (this.ID == 2002)
                        {
                            DarkEmu_GameServer.GlobalUnique.Isy = false;
                        }
                    #endregion
                    #region Lord Yarkan
                        if (this.ID == 3810)
                        {
                            DarkEmu_GameServer.GlobalUnique.Lord = false;
                        }
                    #endregion
                    #region Demon Shaitan
                        if (this.ID == 3875)
                        {
                            DarkEmu_GameServer.GlobalUnique.Demon = false;
                        }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
    }
}