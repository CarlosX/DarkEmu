///////////////////////////////////////////////////////////////////////////
// DarkEmu: Load Skills
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.IO;
using System.Linq;
using System.Text;
using DarkEmu_GameServer.Global;

namespace DarkEmu_GameServer.File
{
    class Load_Skills
    {
        public static void LoadSkillData(string path)
        {
            TxtFile.ReadFromFile(path, '\t');

            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int ID = Convert.ToInt32(TxtFile.commands[1]);
                s_data sd = new s_data();
                sd.ID = ID;
                sd.Name = TxtFile.commands[3];
                sd.Series = TxtFile.commands[5];
                sd.SkillType = (s_data.SkillTypes)Convert.ToByte(TxtFile.commands[8]);
                sd.NextSkill = Convert.ToInt32(TxtFile.commands[9]);
                sd.CastingTimePhase1 = Convert.ToInt32(TxtFile.commands[11]);
                sd.CastingTimePhase2 = Convert.ToInt32(TxtFile.commands[12]);
                sd.CastingTimePhase3 = Convert.ToInt32(TxtFile.commands[13]);
                sd.CastingTime = sd.CastingTimePhase1 + sd.CastingTimePhase2 + sd.CastingTimePhase3;
                sd.reUseTime = Convert.ToInt32(TxtFile.commands[14]);
                sd.AttackTime = Convert.ToInt32(TxtFile.commands[15]);
                sd.Mastery = Convert.ToInt16(TxtFile.commands[34]);
                sd.SkillPoint = Convert.ToInt32(TxtFile.commands[46]);
                sd.Weapon1 = Convert.ToByte(TxtFile.commands[50]);
                sd.Weapon2 = Convert.ToByte(TxtFile.commands[51]);
                sd.Mana = Convert.ToInt32(TxtFile.commands[53]);
                sd.tmpProp = Convert.ToInt32(TxtFile.commands[75]);
                sd.AmountEffect = 0;
                sd.AttackCount = Convert.ToInt32(TxtFile.commands[77]);
                sd.RadiusType = s_data.RadiusTypes.ONETARGET;
                sd.isAttackSkill = false;

                int propIndex = 69;
                bool effectEnd = false;
                int skillInfo;
                //Imbue
                if (sd.Name.Contains("_GIGONGTA_"))
                    sd.Definedname = s_data.Definedtype.Imbue;
                try
                {
                    while ((skillInfo = Convert.ToInt32(TxtFile.commands[propIndex])) != 0 && !effectEnd)
                    {
                        propIndex++;

                        string nameString = ASCIIIntToString(skillInfo);

                        switch (nameString)
                        {
                            // get value - only to client
                            case "getv":
                            case "MAAT":
                            // warrior
                            case "E2AH":
                            case "E2AA":
                            case "E1SA":
                            case "E2SA":
                            // rogue
                            case "CBAT":
                            case "CBRA":
                            case "DGAT":
                            case "DGHR":
                            case "DGAA":
                            case "STDU":
                            case "STSP":
                            case "RPDU":
                            case "RPTU":
                            case "RPBU":
                            // wizard
                            case "WIMD":
                            case "WIRU":
                            case "EAAT":
                            case "COAT":
                            case "FIAT":
                            case "LIAT":
                            // warlock
                            case "DTAT":
                            case "DTDR":
                            case "BLAT":
                            case "TRAA":
                            case "BSHP":
                            case "SAAA":
                            // bard
                            case "MUAT":
                            case "BDMD":
                            case "MUER":
                            case "MUCR":
                            case "DSER":
                            case "DSCR":
                            // cleric
                            case "HLAT":
                            case "HLRU":
                            case "HLMD":
                            case "HLFS":
                            case "HLMI":
                            case "HLBP":
                            case "HLSM":
                            // attribute only - no effect
                            case "nmh": // Healing stone (socket stone)
                            case "nmf": // Movement stone (socket stone)
                            case "eshp":
                            case "reqn":
                            case "fitp":
                            case "ao":   // fortress ??
                            case "rpkt": // fortress repair kit
                            case "hitm": // Taunt the enemy into attacking only you
                            case "efta": // bard tambour
                            case "lks2": // warlock damage buff
                            case "hntp": // tag point
                            case "trap": // csapda??
                            case "cbuf": //itembuff
                            case "nbuf": // ??(ticketnél volt) nem látszika  buff másnak?
                            case "bbuf": //debuff
                            case null:
                                break;
                            case "setv":  //set value
                                string setvType = ASCIIIntToString(Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;

                                switch (setvType)
                                {   // warrior
                                    case "E1SA": // phy attack % //done
                                    case "E2SA": // phy attack % //done
                                    case "E2AA":
                                    case "E2AH": // hit rate inc //done
                                    // rogue
                                    case "CBAT":
                                    case "CBRA":
                                    case "DGAT":
                                    case "DGHR":
                                    case "DGAA":
                                    case "STSP": // speed %
                                    case "STDU": // set stealth duration
                                    case "RPDU": // phy attack %
                                    case "RPBU": // poison duration 
                                    // wizard
                                    case "WIMD": // wizard mana decrease %
                                    case "WIRU": // Increase the range of magic attacks
                                    case "EAAT": // Magical Attack Power %Increase earth
                                    case "COAT": // Magical Attack Power %Increase cold
                                    case "FIAT": // Magical Attack Power %Increase fire
                                    case "LIAT": // Magical Attack Power %Increase fire
                                    // warlock
                                    case "DTAT": // Magical Attack Power %Increase
                                    case "DTDR": // Increase the abnormal status duration inflicted by Dark magic
                                    case "BLAT": // Magical Attack Power %Increase Blood Line row 
                                    case "TRAA": // Increases a Warlocks trap damage 
                                    case "BSHP": // HP draining skill attack power increase
                                    // bard
                                    case "MUAT": // Magical Attack Power % Increase
                                    case "MUER": // Increase the range of harp magic
                                    case "BDMD": // Lowers the MP consumption of skills
                                    case "MUCR": // Resistance Ratio % Increase.
                                    case "DSER": // Increase the range for dance skill.
                                    case "DSCR": // Increase resistance ratio. You don't stop dancing even under attack 
                                    // cleric
                                    case "HLAT": // Increase the damage of cleric magic. %
                                    case "HLRU": // HP recovery % Inrease
                                        sd.Properties1.Add(setvType, Convert.ToInt32(TxtFile.commands[propIndex]));
                                        propIndex++;
                                        break;
                                    // cleric
                                    case "HLFS": // charity
                                    case "HLMI": // charity
                                    case "HLBP": // charity
                                    case "HLSM": // charity
                                        sd.Properties1.Add(setvType, Convert.ToInt32(TxtFile.commands[propIndex]));
                                        propIndex++;
                                        sd.Properties2.Add(setvType, Convert.ToInt32(TxtFile.commands[propIndex]));
                                        propIndex++;

                                        break;
                                }
                                break;
                            // 1 properties
                            case "tant":
                            case "rcur": // randomly cure number of bad statuses
                            case "ck":
                            case "ovl2":
                            case "mwhs":
                            case "scls":
                            case "mwmh":
                            case "mwhh":
                            case "rmut":
                            case "abnb":
                            case "mscc":
                            case "bcnt": // cos bag count [slot]
                            case "chpi": // cos hp increase [%]
                            case "chst": // cos speed increase [%]
                            case "csum": // cos summon [coslevel]
                            case "jobs": // job skill [charlevel]
                            case "hwit": // ITEM_ETC_SOCKET_STONE_HWAN ?? duno what is it
                            case "spi": // Gives additional skill points when hunting monsters. [%inc]
                            case "dura": // skill duration
                            case "msid": // mod def ignore prob%
                            case "hwir": // honor buff new zerk %
                            case "hst3": // honor buff speed %inc
                            case "hst2": // rogue haste speed %inc
                            case "lkdd": // Share % damage taken from a selected person. (link damage)
                            case "gdr":  // gold drop rate %inc
                            case "chcr": // target loses % HP
                            case "cmcr": // target loses % MP
                            case "dcmp": // MP Cost % Decrease
                            case "mwdt": // Weapon Magical Attack Power %Reflect
                            case "pdmg": // Absolute Damage
                            case "lfst": // life steal Absorb HP
                            case "puls": // pulsing skill frequenzy
                            case "pwtt": // Weapon Physical Attack Power reflect.
                            case "pdm2": // ghost killer
                            case "luck": // lucky %inc
                            case "alcu": // alchemy lucky %inc
                            case "terd": // parry reduce
                            case "thrd": // Attack ratio reduce
                            case "ru": // range incrase
                            case "hste": // speed %inc
                            case "da": // downattack %inc
                            case "reqc": // required status?
                            case "dgmp": // damage mana absorb
                            case "dcri": // critical parry inc
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            // 2 properties
                            case "mc":
                            case "atca":
                            case "reat":
                            case "defr":
                            case "msr": // Triggered at a certain chance, the next spell cast does not cost MP. [%chance to trigger|%mana reduce]
                            case "kb": // knockback
                            case "ko":  // knockdown
                            case "zb": // zombie
                            case "fz":  // frozen
                            case "fb":  // frostbite
                            case "spda": // Shield physical defence % reduce. Physical attack power increase.
                            case "expi": // sp|exp %inc PET?
                            case "stri": // str increase
                            case "inti": // int increase
                            case "rhru": // Increase the amount of HP healed. %
                            case "dmgt": // Absorption %? 
                            case "dtnt": // Aggro Reduce
                            case "mcap": // monster mask lvl cap
                            case "apau": // attack power inc [phy|mag]
                            case "lkag": // Share aggro
                            case "dttp": // detect stealth [|maxstealvl]
                            case "tnt2": // taunt inc | aggro %increase
                            case "expu": // exp|sp %inc
                            case "msch": // monster transform
                            case "dtt": // detect invis [ | maxinvilvl]
                            case "hpi": // hp incrase [inc|%inc]
                            case "mpi": // mp incrase [inc|%inc]
                            case "odar": // damage absorbation
                            case "resu": // resurrection [lvl|expback%]
                            case "er": // evasion | parry %inc 
                            case "hr": // hit rating inc | attack rating inc 
                            case "tele": // light teleport [sec|meter*10]
                            case "tel2": // wizard teleport [sec|meter*10]
                            case "tel3": // warrior sprint teleport [sec|m]
                            case "onff": // mana consume per second
                            case "br":  // blocking ratio [|%inc]
                            case "cr":  // critical inc
                            case "dru": // damage %inc [phy|mag]
                            case "irgc": // reincarnation [hp|mp]
                            case "pola": // Preemptive attack prevention [hp|mp]
                            case "curt": // negative status effect reduce target player
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties2.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            // 3 properties
                            case "curl": //anti effect: cure [|effect cure amount|effect level]
                            case "real":
                            case "skc":
                            case "bldr": // Reflects damage upon successful block.
                            case "ca": // confusion
                            case "rt":  // restraint (wizard) << restraint i guess it should restrain the target or put to ground as in same spot like chains on feet :) 
                            case "fe": // fear 
                            case "sl": // dull
                            case "st": // stun
                            case "se": // sleep
                            case "es":  // lightening
                            case "bu":  // burn
                            case "ps":  // poison
                            case "lkdh": // link Damage % MP Absorption (Mana Switch)
                            case "stns": // Petrified status
                            case "hide": // stealth hide
                            case "lkdr": // Share damage
                            case "defp": // defensepower inc [phy|mag]
                            case "bgra": // negative status effect reduce
                            case "cnsm": // consume item
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties2.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties3.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            // 4 properties
                            case "csit": // division
                            case "tb": // hidden
                            case "my": // short sight
                            case "ds": // disease
                            case "csmd":  // weaken
                            case "cspd":  // decay
                            case "cssr": // impotent
                            case "dn": // darkness
                            case "mom": // duration | Berserk mode Attack damage/Defence/Hit rate/Parry rate will increase % | on every X mins
                            case "pmdp": // maximum physical defence strength decrease %
                            case "pmhp": // hp reduce
                            case "dmgr": // damage return [prob%|return%||]
                            case "lnks": // Connection between players
                            case "pmdg": // damage reduce [dura|phy%|mag%|?]
                            case "qest": // some quest related skill?
                            case "heal": // heal [hp|hp%|mp|mp%]
                            case "pw": // player wall
                            case "summ": // summon bird
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties2.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties3.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties4.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            // 5 properties
                            case "bl": // bleed
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties2.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties3.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties4.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties5.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            // 6 properties
                            case "cshp": // panic
                            case "csmp": // combustion
                                sd.Properties1.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties2.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties3.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties4.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties5.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                sd.Properties6.Add(nameString, Convert.ToInt32(TxtFile.commands[propIndex]));
                                propIndex++;
                                break;
                            case "reqi": // required item
                                int weapType1 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                int weapType2 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;

                                if (weapType1 == 4 && weapType2 == 1)
                                    sd.ReqItems.Add(s_data.ItemTypes.SHIELD);
                                else if (weapType1 == 4 && weapType2 == 2)
                                    sd.ReqItems.Add(s_data.ItemTypes.EUSHIELD);
                                else if (weapType1 == 6 && weapType2 == 6)
                                    sd.ReqItems.Add(s_data.ItemTypes.BOW);
                                else if (weapType1 == 6 && weapType2 == 7)
                                    sd.ReqItems.Add(s_data.ItemTypes.ONEHANDED);
                                else if (weapType1 == 6 && weapType2 == 8)
                                    sd.ReqItems.Add(s_data.ItemTypes.TWOHANDED);
                                else if (weapType1 == 6 && weapType2 == 9)
                                    sd.ReqItems.Add(s_data.ItemTypes.AXE);
                                else if (weapType1 == 6 && weapType2 == 10)
                                    sd.ReqItems.Add(s_data.ItemTypes.WARLOCKROD);
                                else if (weapType1 == 6 && weapType2 == 11)
                                    sd.ReqItems.Add(s_data.ItemTypes.STAFF);
                                else if (weapType1 == 6 && weapType2 == 12)
                                    sd.ReqItems.Add(s_data.ItemTypes.XBOW);
                                else if (weapType1 == 6 && weapType2 == 13)
                                    sd.ReqItems.Add(s_data.ItemTypes.DAGGER);
                                else if (weapType1 == 6 && weapType2 == 14)
                                    sd.ReqItems.Add(s_data.ItemTypes.BARD);
                                else if (weapType1 == 6 && weapType2 == 15)
                                    sd.ReqItems.Add(s_data.ItemTypes.CLERICROD);
                                else if (weapType1 == 10 && weapType2 == 0)
                                    sd.ReqItems.Add(s_data.ItemTypes.LIGHTARMOR);
                                else if (weapType1 == 14 && weapType2 == 1)
                                    sd.ReqItems.Add(s_data.ItemTypes.DEVILSPIRIT);
                                break;
                            case "ssou": // summon monster
                                s_data.summon_data summon;
                                while (Convert.ToInt32(TxtFile.commands[propIndex]) != 0)
                                {
                                    summon = new s_data.summon_data();
                                    summon.ID = Convert.ToInt32(TxtFile.commands[propIndex]);
                                    propIndex++;
                                    summon.Type = Convert.ToByte(TxtFile.commands[propIndex]);
                                    propIndex++;
                                    summon.MinSummon = Convert.ToByte(TxtFile.commands[propIndex]);
                                    propIndex++;
                                    summon.MaxSummon = Convert.ToByte(TxtFile.commands[propIndex]);
                                    propIndex++;

                                    sd.SummonList.Add(summon);
                                }
                                break;
                            case "att": // if attack skill
                                sd.isAttackSkill = true;
                                sd.Time = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                sd.MagPer = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                sd.MinAttack = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                sd.MaxAttack = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                sd.PhyPer = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                break;
                            case "efr":
                                sd.efrUnk1 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                int type2 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                if (type2 == 6)
                                    sd.RadiusType = s_data.RadiusTypes.TRANSFERRANGE;
                                else if (type2 == 2)
                                    sd.RadiusType = s_data.RadiusTypes.FRONTRANGERADIUS;
                                else if (type2 == 7)
                                    sd.RadiusType = s_data.RadiusTypes.MULTIPLETARGET;
                                else if (type2 == 4)
                                    sd.RadiusType = s_data.RadiusTypes.PENETRATION;
                                else if (type2 == 3)
                                    sd.RadiusType = s_data.RadiusTypes.PENETRATIONRANGED;
                                else if (type2 == 1)
                                    sd.RadiusType = s_data.RadiusTypes.SURROUNDRANGERADIUS;
                                propIndex++;

                                sd.Distance = Convert.ToInt32(TxtFile.commands[propIndex]); // in decimeters
                                propIndex++;
                                sd.SimultAttack = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                int unk2 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                int unk3 = Convert.ToInt32(TxtFile.commands[propIndex]);
                                propIndex++;
                                break;
                            default:
                                //Console.WriteLine(" {0}  {1}  {2}", propIndex, nameString, sd.Name);
                                effectEnd = true;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Systems.Debugger.Write(ex);
                }

                // this property only affects target when player target is set
                sd.canSelfTargeted = true;
                sd.needPVPstate = true;

                ////////////// set skill property's additional info for non attack skillz
                if (!sd.isAttackSkill)
                {
                    if (sd.Properties1.ContainsKey("heal") || // heal
                        sd.Properties1.ContainsKey("curl"))   // bad status removal
                    {
                        sd.TargetType = Global.s_data.TargetTypes.PLAYER;
                        sd.needPVPstate = false;
                    }
                    if (sd.Properties1.ContainsKey("resu")) // resurrection
                    {
                        sd.TargetType = Global.s_data.TargetTypes.PLAYER;
                        sd.canSelfTargeted = false;
                        sd.needPVPstate = false;
                    }
                    if (sd.Properties1.ContainsKey("terd") ||  // parry reduce
                        sd.Properties1.ContainsKey("thrd") ||  // Attack ratio reduce
                        sd.Properties1.ContainsKey("cspd") ||  // decay
                        sd.Properties1.ContainsKey("csmd") ||  // weaken
                        sd.Properties1.ContainsKey("cssr") ||  // impotent
                        sd.Properties1.ContainsKey("st") ||  // stun
                        sd.Properties1.ContainsKey("bu") ||  // burn
                        sd.Properties1.ContainsKey("fb"))      // frostbite
                    {
                        sd.TargetType = Global.s_data.TargetTypes.PLAYER | s_data.TargetTypes.MOB;
                        sd.canSelfTargeted = false;
                    }
                }
                #region Information
                /*
                sd.EffectLvl1 = Convert.ToInt32(TxtFile.commands[82]);
                if (sd.ID == 3977)
                    Console.WriteLine("{0}", sd.Properties2);
                if(Convert.ToInt64(TxtFile.commands[78]) < 255) sd.eLevel = Convert.ToByte(TxtFile.commands[78]);
                if (ID == 3639)
                {
                    for (int i = 0; i <= TxtFile.commands.Length - 1; i++)
                        Console.WriteLine(" {0}  {1}", i, TxtFile.commands[i]);
                }
                if (ID == 160)
                {
                    for (int i = 0; i <= TxtFile.commands.Length - 1; i++)
                        Console.WriteLine(" {0}  {1}", i, TxtFile.commands[i]);
                }
                */
                #endregion
                Data.SkillBase[ID] = sd;
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " skills");
        }
        public static string ASCIIIntToString(int skillInfo)
        {
            byte[] name;
            if (skillInfo <= 0xFF)
            {
                name = new byte[1];
                name[0] = (byte)(skillInfo);
            }
            else if (skillInfo <= 0xFFFF)
            {
                name = new byte[2];
                name[0] = (byte)(skillInfo >> 8);
                name[1] = (byte)(skillInfo);
            }
            else if (skillInfo <= 0xFFFFFF)
            {
                name = new byte[3];
                name[0] = (byte)(skillInfo >> 16);
                name[1] = (byte)(skillInfo >> 8);
                name[2] = (byte)(skillInfo);
            }
            else
            {
                name = new byte[4];
                name[0] = (byte)(skillInfo >> 24);
                name[1] = (byte)(skillInfo >> 16);
                name[2] = (byte)(skillInfo >> 8);
                name[3] = (byte)(skillInfo);
            }
            return Encoding.ASCII.GetString(name);
        }
    }
}
