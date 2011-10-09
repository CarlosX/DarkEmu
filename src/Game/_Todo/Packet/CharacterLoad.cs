///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
// File info: Private packet data
///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Data.SqlClient;

namespace DarkEmu_GameServer
{
    public partial class Packet
    {

        public static byte[] Testingpacket(int uniqueid, int skillid, int targetid)
        {
            PacketWriter writer = new PacketWriter();
            writer.Create(0xB070);
            writer.Byte(1);
            writer.Byte(2);
            writer.Byte(0x30);
            writer.DWord(skillid);
            writer.DWord(uniqueid);
            writer.DWord(0x400000); // random :)
            writer.DWord(targetid);
            //writer.Byte(0); // so hm 
            writer.Byte(1);
            writer.Byte(1);
            writer.Byte(1);
            writer.DWord(targetid);
            writer.Byte(0); // static i suppose and it signs the no more target affected
            writer.DWord(0x11); 
            writer.DWord(0x02); // effectid lets modify it a bit :p
            return writer.GetBytes();
            // could be  so if we dont need thoose bytes we can use it like
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Character listening packet
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] CharacterListing(string name)
        {
            
            Systems.MsSQL ms    = new Systems.MsSQL("SELECT TOP 4 * FROM character WHERE account='" + name + "'");
            PacketWriter Writer = new PacketWriter();

            Writer.Create(Systems.SERVER_CHARACTERSCREEN);      // Select opcode
            Writer.Byte(2);                                     // Static byte 2
            Writer.Byte(1);                                     // Static byte 1
            Writer.Byte(ms.Count());                            // Byte Character Count

            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    Writer.DWord(reader.GetInt32(3));           // DWord Skin Model
                    Writer.Text(reader.GetString(2));           // String Name
 
                    Writer.Byte(reader.GetByte(4));             // Byte Skin Volume
                    Writer.Byte(reader.GetByte(5));             // Byte Level

                    Writer.LWord(reader.GetInt64(12));          // Long Experience

                    Writer.Word(reader.GetInt16(6));            // Word STR
                    Writer.Word(reader.GetInt16(7));            // Word INT
                    Writer.Word(reader.GetInt16(8));            // Attribute points
                    
                    Writer.DWord(reader.GetInt32(9));           // HP
                    Writer.DWord(reader.GetInt32(10));          // MP

                    TimeSpan ts = Convert.ToDateTime(reader.GetDateTime(43)) - DateTime.Now;
                    double time = ts.TotalMinutes;

                    if (Math.Round(time) > 0)
                    {
                        Writer.Byte(1);
                        Writer.DWord(Math.Round(time));
                    }
                    else
                    {
                        Writer.Byte(0);
                    }

                    if (Math.Round(time) < 0 && DateTime.Now != reader.GetDateTime(43))
                    {
                        Systems.MsSQL.UpdateData("UPDATE character SET deleted='1' Where id='" + reader.GetInt32(0) + "'");
                    }

                    Writer.Word(0);
                    Writer.Byte(0);

                    Function.Items.PrivateItemPacket(Writer, reader.GetInt32(0), 8, 0,false);
                    Function.Items.PrivateItemPacket(Writer, reader.GetInt32(0), 5, 1,false);
                }
                //Jobtype information
                int jobinfo = Systems.MsSQL.GetDataInt("SELECT * FROM users WHERE id='"+ name +"'", "jobtype");
                Writer.Byte((byte)(jobinfo));
            }
            ms.Close();

            return Writer.GetBytes();
        }
        public static byte[] CharacterName(byte errocode)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHARACTERSCREEN);
            Writer.Byte(4);
            Writer.Byte(2);
            Writer.Byte(0x10);
            Writer.Byte(errocode);
            return Writer.GetBytes();
        }
        public static byte[] ScreenSuccess(byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHARACTERSCREEN);
            Writer.Byte(type);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] LoginScreen()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_LOGINSCREEN_ACCEPT);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] StartPlayerLoad()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_STARTPLAYERDATA);
            return Writer.GetBytes();
        }
        public static byte[] EndPlayerLoad()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ENDPLAYERDATA);
            return Writer.GetBytes();
        }
        public static byte[] Load(character c)
        {

            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYERDATA);
            /////////////////////////////////////////////////////// Character basic info
            #region Basic info   
            Writer.DWord(c.Ids.GetLoginID);
            Writer.DWord(c.Information.Model);
            Writer.Byte(c.Information.Volume);
            Writer.Byte(c.Information.Level);
            Writer.Byte(c.Information.Level);
            Writer.LWord(c.Information.XP);
            Writer.DWord(c.Information.SpBar);
            Writer.LWord(c.Information.Gold);
            Writer.DWord(c.Information.SkillPoint);
            Writer.Word(c.Information.Attributes);
            Writer.Byte(c.Information.BerserkBar);
            Writer.DWord(0);
            Writer.DWord(c.Stat.SecondHp);
            Writer.DWord(c.Stat.SecondMP);
            Writer.Bool(c.Information.Level < 20 ? true : false);
            #endregion
            /////////////////////////////////////////////////////// Character Player Kill Info
            #region Pk information
            //Mssql perfection reading with multiple data adapters... while this one is open i can still read anything else from the database
            //With no speed reduction...
            Systems.MsSQL checkpk = new Systems.MsSQL("SELECT * FROM character WHERE name ='" + c.Information.Name + "'");
            using (System.Data.SqlClient.SqlDataReader getinfo = checkpk.Read())
            {
                while (getinfo.Read())
                {
                    byte dailypk = getinfo.GetByte(48);
                    byte pklevel = getinfo.GetByte(49);
                    byte murderlevel = getinfo.GetByte(50);

                    Writer.Byte(dailypk);
                    Writer.Word(pklevel);
                    Writer.DWord(murderlevel);
                    if (murderlevel != 0) c.Information.Murderer = true;
                }
            }
            #endregion
            /////////////////////////////////////////////////////// Character Title
            #region Title
            Writer.Byte(c.Information.Title);
            #endregion
            /////////////////////////////////////////////////////// Character Pvpstate
            #region Pvp
            Writer.Byte(c.Information.Pvpstate);
            if (c.Information.Pvpstate > 0) 
                c.Information.PvP = true;
            #endregion
            /////////////////////////////////////////////////////// Character Items
            #region Item
            
            Writer.Byte(c.Information.Slots);

            Systems.MsSQL ms = new Systems.MsSQL("SELECT * FROM char_items WHERE owner='" + c.Information.CharacterID + "' AND slot >= '0' AND slot <= '" + c.Information.Slots + "' AND inavatar='0' AND storagetype='0'");
            Writer.Byte(ms.Count());
            using (System.Data.SqlClient.SqlDataReader msreader = ms.Read())
            {

                while (msreader.Read())
                {

                    short amount = msreader.GetInt16(6);

                    if (amount < 1) amount = 1;
                    Systems.MsSQL.InsertData("UPDATE char_items SET quantity='" + amount + "' WHERE owner='" + c.Information.CharacterID + "' AND itemid='" + msreader.GetInt32(2) + "' AND id='" + msreader.GetInt32(0) + "' AND storagetype='0'");

                    if (msreader.GetByte(5) == 6)
                        c.Information.Item.wID = (Int32)(msreader.GetInt32(2));
                    if (msreader.GetByte(5) == 7)
                    {
                        c.Information.Item.sID = msreader.GetInt32(2);
                        c.Information.Item.sAmount = msreader.GetInt16(6);
                    }
                    
                    Item.AddItemPacket(Writer, msreader.GetByte(5), msreader.GetInt32(2), msreader.GetByte(4), amount, msreader.GetInt32(7),msreader.GetInt32(0), msreader.GetInt32(9), msreader.GetInt32(30));
                }
            }
            ms.Close();

            //Avatar
            Writer.Byte(5);

            ms = new Systems.MsSQL("SELECT * FROM char_items WHERE owner='" + c.Information.CharacterID + "' AND slot >= '0' AND slot <= '" + c.Information.Slots + "' AND inavatar='1' AND storagetype='0'");

            Writer.Byte(ms.Count());
            using (System.Data.SqlClient.SqlDataReader msreader = ms.Read())
            {
                while (msreader.Read())
                {
                    Item.AddItemPacket(Writer, msreader.GetByte(5), msreader.GetInt32(2), msreader.GetByte(4), msreader.GetInt16(6), msreader.GetInt32(7), msreader.GetInt32(0), msreader.GetInt32(9),msreader.GetInt32(30));
                }
            }
            ms.Close();

            Writer.Byte(0);

            // job mastery 
            Writer.Byte(0x0B);
            Writer.Byte(0);
            Writer.Byte(0);

            #endregion
            ///////////////////////////////////////////////////////  Mastery
            #region Mastery
            if (c.Information.Model <= 12000)
            {
                for (byte i = 1; i <= 8; i++)
                {
                    Writer.Byte(1);
                    Writer.DWord(c.Stat.Skill.Mastery[i]);
                    Writer.Byte(c.Stat.Skill.Mastery_Level[i]);
                }
            }
            else
            {
                if (c.Information.Model >= 14000)
                {
                    for (byte i = 1; i < 8; i++)
                    {
                        Writer.Byte(1);
                        Writer.DWord(c.Stat.Skill.Mastery[i]);
                        Writer.Byte(c.Stat.Skill.Mastery_Level[i]);
                    }
                }
            }
            #endregion
            /////////////////////////////////////////////////////// Skills
            #region Skill
            Writer.Byte(2);
            Writer.Byte(0);
                for (int i = 1; i <= c.Stat.Skill.AmountSkill; i++)
                {
                    Writer.Byte(1);
                    Writer.DWord(c.Stat.Skill.Skill[i]);
                    Writer.Byte(1);
                }
            Writer.Byte(2);
            #endregion
            /////////////////////////////////////////////////////// Quests
            #region Quest
            Writer.Word(1); // how many Quest ids completed/aborted
            Writer.DWord(1);// Quest id
            Writer.Byte(0);//number of Quests that are live
            #endregion
            Writer.Byte(0);//? for now
            /////////////////////////////////////////////////////// Talisman
            #region Talisman
            Writer.DWord(1);//new
            Writer.DWord(1);//new
            Writer.DWord(0);//? for now
            Writer.DWord(0x0C);//new
            #endregion
            /////////////////////////////////////////////////////// Position + id + speed
            #region Character id / Position / Speed
            Writer.DWord(c.Information.UniqueID);
            Writer.Byte(c.Position.xSec);
            Writer.Byte(c.Position.ySec);
            if (!File.FileLoad.CheckCave(c.Position.xSec, c.Position.ySec))
            {
                Writer.Float(Formule.packetx(c.Position.x, c.Position.xSec));
                Writer.Float(c.Position.z);
                Writer.Float(Formule.packety(c.Position.y, c.Position.ySec));
            }
            else
            {
                Writer.Float(Formule.cavepacketx(c.Position.x));// Added for cave Coords
                Writer.Float(c.Position.z);
                Writer.Float(Formule.cavepackety(c.Position.y));// Added for cave Coords

            }
            Writer.Word(0);							// Angle
            Writer.Byte(0);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Word(0);							// Angle
            Writer.Word(0);
            Writer.Byte(0);
            Writer.Bool(false); //berserk

            Writer.Byte(0);//new ?

            Writer.Float(c.Speed.WalkSpeed);
            Writer.Float(c.Speed.RunSpeed);
            Writer.Float(c.Speed.BerserkSpeed);
            #endregion
            /////////////////////////////////////////////////////// Premium Tickets
            #region Premium ticket
            Writer.Byte(0); //ITEM_MALL_GOLD_TIME_SERVICE_TICKET_4W
            #endregion
            /////////////////////////////////////////////////////// GM Check + Name
            #region GM Check + Name
            Writer.Text(c.Information.Name);
            #endregion
            /////////////////////////////////////////////////////// Character jobs
            #region Character Job / hunter thief trader ( old job things )
                //Writer info with job name when on job
                /*if (c.Job.state == 1 && c.Job.Jobname != "0")
                {
                    Writer.Text(c.Job.Jobname);
                    Writer.Byte(3);
                    Writer.Byte(1);
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                }
                //Write basic info noname
                if (c.Job.Jobname == "0")
                {
                    Writer.Word(0);
                    Writer.Byte(3);
                    Writer.Byte(1);
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                }
                //Write no info
                else
                {
                    Writer.Word(0);
                    Writer.Byte(0);
                    Writer.Byte(1);
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                }*/
            #endregion
            #region New job system
            if (c.Job.state == 1)
            {
                Writer.Text(c.Job.Jobname);
                Writer.Byte(1);
                Writer.Byte(c.Job.level);//Level job
                Writer.Byte(c.Information.Level);//Level char
                Writer.Byte(1); // job level? myb
                Writer.LWord(0);// job exp probably y
                Writer.Byte(0);
                Writer.Byte(0);
                Writer.Byte(0);
                Writer.Byte(0);
            }
            else
            {

                Writer.Word(0);
                Writer.Byte(0);
                Writer.Byte(0);
                Writer.Byte(2); // job type
                Writer.Byte(1); // job level? myb
                Writer.LWord(0);// job exp probably y
                Writer.Byte(0);
                Writer.Byte(0);
                Writer.Byte(0);
                Writer.Byte(0);
            }

            #endregion
                /////////////////////////////////////////////////////// Pvp / Pk State
            #region Pvp / Pk State
            if (c.Information.Pvpstate == 1 || c.Information.Murderer)
            {
                Writer.Byte(0x22);
            }
            else if (c.Information.Pvpstate == 0 || !c.Information.Murderer)
            {
                Writer.Byte(0xFF);
            }
            #endregion
            /////////////////////////////////////////////////////// Guide Data 
            #region Guide Data this data stacks on itself so if guide id is 0400000000000000 and next guide is 0300000000000000 the data to send is 0700000000000000

            for (int i = 0; i < 8; ++i)//Main Guide Packet Info
            {
                Writer.Byte(c.Guideinfo.G1[i]);//Reads From Int Array
            }
            #endregion
            /////////////////////////////////////////////////////// Account / Gm Check
            #region Account ID + Gm Check
            Writer.DWord(c.Account.ID);
            Writer.Byte(0);//c.Information.GM
            #endregion
            /////////////////////////////////////////////////////// Quickbar + Autopotion
            #region Bar information
            Writer.Byte(7);
            PacketReader reader = new PacketReader(System.IO.File.ReadAllBytes(Environment.CurrentDirectory + @"\player\info\quickbar\" + c.Information.Name + ".dat"));
            PlayerQuickBar(reader, Writer);
            reader = new PacketReader(System.IO.File.ReadAllBytes(Environment.CurrentDirectory + @"\player\info\autopot\" + c.Information.Name + ".dat"));
            PlayerAutoPot(reader, Writer);
            #endregion
            /////////////////////////////////////////////////////// Academy
            #region Academy
            Writer.Byte(0); // number of player in academy
            /* // if we have players there 
             Writer.Byte(1);
             Writer.Text("asd"); 
             */
            Writer.Byte(0);//added byte today for 1.310
            Writer.Byte(0);
            Writer.Word(1); 
            Writer.Word(1);
            Writer.Byte(0);
            Writer.Byte(1);
            #endregion
            return Writer.GetBytes();
        }
        public static void PlayerAutoPot(PacketReader Reader, PacketWriter Writer)
        {
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Writer.Byte(Reader.Byte());
            Reader.Close();
        }
        public static void PlayerQuickBar(PacketReader Reader, PacketWriter Writer)
        {
            byte amm = 0;
            int[] skillid = new int[51];
            byte[] slotn = new byte[51];
            for (byte i = 0; i <= 50; i++)
            {
                slotn[i] = Reader.Byte();
                if (slotn[i] != 0)
                {
                    skillid[i] = Reader.Int32();
                    amm++;
                }
                else Reader.Skip(4);
            }
            Writer.Byte(amm);
            for (byte i = 0; i <= 50; i++)
            {
                if (slotn[i] != 0)
                {
                    Writer.Byte(i);
                    Writer.Byte(slotn[i]);
                    Writer.DWord(skillid[i]);
                }
            }
            Reader.Close();
        }
        public static byte[] PlayerStat(character c)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYERSTAT);
            Writer.DWord((int)c.Stat.MinPhyAttack);
            Writer.DWord((int)c.Stat.MaxPhyAttack);
            Writer.DWord((int)c.Stat.MinMagAttack);
            Writer.DWord((int)c.Stat.MaxMagAttack);
            Writer.Word((ushort)c.Stat.PhyDef);
            Writer.Word((ushort)c.Stat.MagDef);
            Writer.Word((ushort)c.Stat.Hit);
            Writer.Word((ushort)c.Stat.Parry);
            Writer.DWord((int)c.Stat.Hp);
            Writer.DWord((int)c.Stat.Mp);
            Writer.Word((ushort)c.Stat.Strength);
            Writer.Word((ushort)c.Stat.Intelligence);
            return Writer.GetBytes();
        }
        public static byte[] SnowFlakeEvent()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x30D6);
            Writer.DWord(0x8848E5EC);
            Writer.Byte(0x82);
            Writer.Byte(1);
            Writer.Word(0x5B88);
            Writer.DWord(0x000003B6);
            Writer.DWord(0x0000000B);
            Writer.DWord(0x000004F6);
            Writer.DWord(0x00000012);
            return Writer.GetBytes();
        }

        public static byte[] Weather(int type, int speed)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SEND_WEATHER);
            Writer.Byte(type);
            Writer.Byte(speed);
            return Writer.GetBytes();
        }
        public static byte[] PlayerUnknowPack(int id)
        {
            TimeSpan ts = Systems.ServerStartedTime - DateTime.Now;
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHARACTER_CELESTIAL_POSITION);
            Writer.DWord(id);
            Writer.DWord(Math.Abs(ts.TotalSeconds));

            return Writer.GetBytes();
        }
        public static byte[] SendFriendListstatic()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SEND_FRIEND_LIST);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] SendFriendList(byte count, character c)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SEND_FRIEND_LIST);
            //Groups
            Systems.MsSQL ms = new Systems.MsSQL("SELECT * FROM friends_groups WHERE playerid='"+ c.Information.CharacterID  +"'");
            int groupcount = ms.Count();
            groupcount = groupcount + 1;
            Writer.Byte((byte)groupcount);
            
            Writer.Word(0);
            Writer.Text("NonClassified");
            int groupid = 0;
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    string groupname = reader.GetString(2);
                    groupid = groupid +1;
                    Writer.Word((short)groupid);
                    Writer.Text(groupname);
                }
            }
            //Friends
            Writer.Byte(count);
            ms = new Systems.MsSQL("SELECT * FROM friends WHERE owner='"+ c.Information.CharacterID  +"'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    int model       = Systems.MsSQL.GetDataInt("SELECT * FROM character WHERE id='" + reader.GetInt32(2) + "'", "chartype");
                    int status      = Systems.MsSQL.GetDataInt("SELECT * FROM character WHERE id='" + reader.GetInt32(2) + "'", "Online");
                    int charid      = Systems.MsSQL.GetDataInt("SELECT * FROM character WHERE id='" + reader.GetInt32(2) + "'", "id");
                    string charname = Systems.MsSQL.GetData("SELECT * FROM character WHERE id='" + reader.GetInt32(2) + "'", "name");
                    string groupname = reader.GetString(4);
                    
                    Writer.DWord(charid);               // Friend CharID
                    Writer.Text(charname);              // Friend Name
                    Writer.DWord(model);                // Friend Model Type
                    if (groupname == "none")
                        Writer.Word(0);
                    else
                        Writer.Word(groupid);

                    if (status != 0)
                    {
                        Writer.Byte(0);                 // Friend is online
                    }
                    else
                    {
                        Writer.Byte(1);                 // Inverted, Friend is offline
                    }
                }
                reader.Close();
            }
            ms.Close();
            return Writer.GetBytes();
        }
        public static byte[] Completeload()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x3077);
            Writer.Word(0);
            return Writer.GetBytes();
        }
        public static byte[] TestPacket()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0xB50E);
            Writer.Byte(1);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] Silk(int normalsilk, int premsilk)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SILKPACK);
            Writer.DWord(normalsilk);
            Writer.DWord(premsilk);
            return Writer.GetBytes();
        }
        public static byte[] UnknownPacket()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_UNKNOWNINFO);
            Writer.Byte(2);
            Writer.Byte(3);
            Writer.DWord(1);
            Writer.DWord(0x00007531);
            Writer.DWord(0x00004FFB);
            Writer.DWord(0x0000B998);
            return Writer.GetBytes();
        }
        public static byte[] Tickets(int charid, int type, byte slot)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_TICKET);
            Writer.DWord(charid);
            Writer.Word(type);
            Writer.Byte(slot);
            Writer.Byte(0x2A);
            Writer.Byte(0x00);

            return Writer.GetBytes();
        }
        public static byte[] PremiumTicketData(int ticketid, int time)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_TICKET);
            Writer.Byte(0x0F);//Not sure about this byte
            Writer.DWord(ticketid);
            Writer.DWord(time);
            return Writer.GetBytes();
        }
        public static byte[] BalloonTicket(int charid, int info)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_TICKET);
            Writer.Byte(0);
            Writer.DWord(charid);
            Writer.DWord(info);
            return Writer.GetBytes();
        }
    }
}