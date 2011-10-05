///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Game
{
    public partial class Systems
    {
        #region Listing
        void CharacterListing()
        {
            try
            {
                client.Send(Packet.CharacterListing(Player.AccountName));
            }
            catch (Exception ex)
            {
                Console.WriteLine("CharacterListing()");
                deBug.Write(ex);
                Console.WriteLine(ex);
            }
        }
        #endregion

        #region Delete & Restore
        void CharacterDelete()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Reader.Byte();
                string name = Reader.Text();
                Reader.Close();
                MsSQL.InsertData("UPDATE character SET deletedtime=dateadd(dd,7,getdate()) WHERE name='" + name + "'");
                client.Send(Packet.ScreenSuccess(3));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ChracterDelete()");
                deBug.Write(ex);
                Console.WriteLine(ex);//Write source of the error
            }
        }

        void CharacterRestore()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Reader.Byte();
                string name = Reader.Text();
                Reader.Close();
                MsSQL.InsertData("UPDATE character SET deletedtime=0 WHERE name='" + name + "'");
                client.Send(Packet.ScreenSuccess(5));
            }
            catch (Exception ex)
            {
                Console.WriteLine("CharacterRestore()");
                deBug.Write(ex);
                Console.WriteLine(ex); //Write source of the error
            }
        }
        #endregion

        #region Create & Name
        void CharacterCreate()
        {
            try
            {
                //Define amount of characters allowed!
                if (MsSQL.GetRowsCount("SELECT * FROM character WHERE account='" + Player.AccountName + "'") == 4) 
                    return;

                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Reader.Byte();
                string name = Reader.Text();
                int model = Reader.Int32();
                byte volume = Reader.Byte();
                int[] Item = new int[4];
                Item[0] = Reader.Int32();
                Item[1] = Reader.Int32();
                Item[2] = Reader.Int32();
                Item[3] = Reader.Int32();
                Reader.Close();

                #region Check Name
                if (CharacterCheck(name))
                {
                    client.Send(Packet.CharacterName(4));
                    return;
                }
                if (name.Contains("[")) return;
                if (name.Contains("GM")) return;
                if (name.Contains("]")) return;
                if (name.Contains("_")) return;
                if (name.Contains("-")) return;
                #endregion
                //#################################################################### European models begin

                if (model >= 14715 && model <= 14745)
                {
                    MsSQL.InsertData("INSERT INTO character (account, name, chartype, volume, xsect, ysect, xpos, zpos, ypos, savearea,GuideData) VALUES ('" + Player.AccountName + "','" + name + "', '" + model + "', '" + volume + "','79','105','1000','1000','83','20','0000000000000000')"); 
                    MsSQL.InsertData("INSERT INTO character_rev (charname, revxsec, revysec, revx, revy, revz) VALUES ('" + name + "','79','105','1000','22','83')");
                    Player.CreatingCharID = MsSQL.GetDataInt("Select * from character Where name='" + name + "'", "id");

                    double MagDef = 3;
                    double PhyDef = 6;
                    double Parry = 11;
                    double Hit = 11;
                    double MinPhyA = 6;
                    double MaxPhyA = 9;
                    double MinMagA = 6;
                    double MaxMagA = 10;

                    Framework.Ini ini;
                    ini = new Framework.Ini(Environment.CurrentDirectory + @"\Settings\Settings_Custom.ini");
                    string Item1 = ini.GetValue("START ITEMS EUROPE", "Item001").ToString();
                    string Item2 = ini.GetValue("START ITEMS EUROPE", "Item002").ToString();
                    string Item3 = ini.GetValue("START ITEMS EUROPE", "Item003").ToString();
                    string Item4 = ini.GetValue("START ITEMS EUROPE", "Item004").ToString();
                    string Item5 = ini.GetValue("START ITEMS EUROPE", "Item005").ToString();

                    string Amount1 = ini.GetValue("START ITEMS EUROPE", "Amount001").ToString();
                    string Amount2 = ini.GetValue("START ITEMS EUROPE", "Amount002").ToString();
                    string Amount3 = ini.GetValue("START ITEMS EUROPE", "Amount003").ToString();
                    string Amount4 = ini.GetValue("START ITEMS EUROPE", "Amount004").ToString();
                    string Amount5 = ini.GetValue("START ITEMS EUROPE", "Amount005").ToString();
                    try
                    {
                    AddItem(Convert.ToInt32(Item1), Convert.ToByte(Amount1), 13, 1, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item2), Convert.ToByte(Amount2), 14, 1, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item3), Convert.ToByte(Amount3), 15, 1, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item4), Convert.ToByte(Amount4), 16, 1, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item5), Convert.ToByte(Amount5), 17, 1, Player.CreatingCharID, 0);
                    }
                    catch (Exception)
                    {
                        
                    }

                    AddItem(Item[0], 0, 1, 0, Player.CreatingCharID, 0);
                    AddItem(Item[1], 0, 4, 0, Player.CreatingCharID, 0);
                    AddItem(Item[2], 0, 5, 0, Player.CreatingCharID, 0);
                    AddItem(Item[3], 0, 6, 0, Player.CreatingCharID, 0);

                    MagDef += Data.ItemBase[Item[0]].Defans.MinMagDef;
                    MagDef += Data.ItemBase[Item[1]].Defans.MinMagDef;
                    MagDef += Data.ItemBase[Item[2]].Defans.MinMagDef;
                    PhyDef += Data.ItemBase[Item[0]].Defans.MinPhyDef;
                    PhyDef += Data.ItemBase[Item[1]].Defans.MinPhyDef;
                    PhyDef += Data.ItemBase[Item[2]].Defans.MinPhyDef;
                    Parry += Data.ItemBase[Item[0]].Defans.Parry;
                    Parry += Data.ItemBase[Item[1]].Defans.Parry;
                    Parry += Data.ItemBase[Item[2]].Defans.Parry;
                    Hit += Data.ItemBase[Item[3]].Attack.MinAttackRating;
                    MinPhyA += Data.ItemBase[Item[3]].Attack.Min_LPhyAttack;
                    MaxPhyA += Data.ItemBase[Item[3]].Attack.Min_HPhyAttack;
                    MinMagA += Data.ItemBase[Item[3]].Attack.Min_LMagAttack;
                    MaxMagA += Data.ItemBase[Item[3]].Attack.Min_HMagAttack;

                    if (Item[3] == 10730 || Item[3] == 10734)
                    {
                        MagDef += Data.ItemBase[251].Defans.MinMagDef;
                        PhyDef += Data.ItemBase[251].Defans.MinPhyDef;
                        Parry += Data.ItemBase[251].Defans.Parry;
                        AddItem(10738, 0, 7, 0, Player.CreatingCharID, 0);
                    }
                    if (Item[3] == 10733)
                    {
                        AddItem(10376, 250, 7, 1, Player.CreatingCharID, 0);
                    }
                    MsSQL.UpdateData("update character set min_phyatk='" + (int)Math.Round(MinPhyA) +
                            "', max_phyatk='" + Math.Round(MaxPhyA) +
                            "', min_magatk='" + Math.Round(MinMagA) +
                            "', max_magatk='" + Math.Round(MaxMagA) +
                            "', phydef='" + Math.Round(PhyDef) +
                            "', magdef='" + Math.Round(PhyDef) +
                            "', parry='" + Math.Round(Parry) +
                            "', hit='" + Math.Round(Hit) +
                            "' where name='" + name + "'");

                    AddMastery(513, Player.CreatingCharID); //Warrior
                    AddMastery(515, Player.CreatingCharID); //Rogue
                    AddMastery(514, Player.CreatingCharID); //Wizard
                    AddMastery(516, Player.CreatingCharID); //Warlock
                    AddMastery(517, Player.CreatingCharID); //Bard
                    AddMastery(518, Player.CreatingCharID); //Cleric
                    Console.WriteLine("@Gameserver:             Character: " + name + " Has been created");
                    client.Send(Packet.ScreenSuccess(1));
                } 
                else
                {

                    #region Check Name
                    if (CharacterCheck(name))
                    {
                        client.Send(Packet.CharacterName(4));
                        return;
                    }
                    #endregion
                    //#################################################################### Chinese models begin
                    MsSQL.InsertData("INSERT INTO character (account, name, chartype, volume,GuideData) VALUES ('" + Player.AccountName + "','" + name + "', '" + model + "', '" + volume + "','0000000000000000')"); 
                    MsSQL.InsertData("INSERT INTO character_rev (charname, revxsec, revysec, revx, revy, revz) VALUES ('" + name + "','168','79','911','1083','-32')");
                    Player.CreatingCharID = MsSQL.GetDataInt("Select * from character Where name='" + name + "'", "id");
                   
                    #region Item
                    double MagDef   = 3;
                    double PhyDef   = 6;
                    double Parry    = 11;
                    double Hit      = 11;
                    double MinPhyA  = 6;
                    double MaxPhyA  = 9;
                    double MinMagA  = 6;
                    double MaxMagA  = 10;

                    Framework.Ini ini;
                    ini = new Framework.Ini(Environment.CurrentDirectory + @"\Settings\Settings_Custom.ini");

                    string Item1 = ini.GetValue("START ITEMS CHINESE", "Item001").ToString();
                    string Item2 = ini.GetValue("START ITEMS CHINESE", "Item002").ToString();
                    string Item3 = ini.GetValue("START ITEMS CHINESE", "Item003").ToString();
                    string Item4 = ini.GetValue("START ITEMS CHINESE", "Item004").ToString();
                    string Item5 = ini.GetValue("START ITEMS CHINESE", "Item005").ToString();

                    string Amount1 = ini.GetValue("START ITEMS CHINESE", "Amount001").ToString();
                    string Amount2 = ini.GetValue("START ITEMS CHINESE", "Amount002").ToString();
                    string Amount3 = ini.GetValue("START ITEMS CHINESE", "Amount003").ToString();
                    string Amount4 = ini.GetValue("START ITEMS CHINESE", "Amount004").ToString();
                    string Amount5 = ini.GetValue("START ITEMS CHINESE", "Amount005").ToString();

                    try
                    {
                        AddItem(Convert.ToInt32(Item1), Convert.ToByte(Amount1), 13, 1, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item2), Convert.ToByte(Amount2), 14, 1, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item3), Convert.ToByte(Amount3), 15, 1, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item4), Convert.ToByte(Amount4), 16, 1, Player.CreatingCharID, 0);
                        AddItem(Convert.ToInt32(Item5), Convert.ToByte(Amount5), 17, 1, Player.CreatingCharID, 0);
                    }
                    catch (Exception)
                    {

                    }

                    AddItem(Item[0], 0, 1, 0, Player.CreatingCharID, 0);
                    AddItem(Item[1], 0, 4, 0, Player.CreatingCharID, 0);
                    AddItem(Item[2], 0, 5, 0, Player.CreatingCharID, 0);
                    AddItem(Item[3], 0, 6, 0, Player.CreatingCharID, 0);

                    MagDef +=   Data.ItemBase[Item[0]].Defans.MinMagDef;
                    MagDef +=   Data.ItemBase[Item[1]].Defans.MinMagDef;
                    MagDef +=   Data.ItemBase[Item[2]].Defans.MinMagDef;
                    PhyDef +=   Data.ItemBase[Item[0]].Defans.MinPhyDef;
                    PhyDef +=   Data.ItemBase[Item[1]].Defans.MinPhyDef;
                    PhyDef +=   Data.ItemBase[Item[2]].Defans.MinPhyDef;
                    Parry +=    Data.ItemBase[Item[0]].Defans.Parry;
                    Parry +=    Data.ItemBase[Item[1]].Defans.Parry;
                    Parry +=    Data.ItemBase[Item[2]].Defans.Parry;
                    Hit +=      Data.ItemBase[Item[3]].Attack.MinAttackRating;
                    MinPhyA +=  Data.ItemBase[Item[3]].Attack.Min_LPhyAttack;
                    MaxPhyA +=  Data.ItemBase[Item[3]].Attack.Min_HPhyAttack;
                    MinMagA +=  Data.ItemBase[Item[3]].Attack.Min_LMagAttack;
                    MaxMagA +=  Data.ItemBase[Item[3]].Attack.Min_HMagAttack;

                    if (Item[3] == 3632 || Item[3] == 3633)
                    {
                        MagDef +=   Data.ItemBase[251].Defans.MinMagDef;
                        PhyDef +=   Data.ItemBase[251].Defans.MinPhyDef;
                        Parry +=    Data.ItemBase[251].Defans.Parry;
                        AddItem(251, 0, 7, 0, Player.CreatingCharID, 0);
                    }
                    if (Item[3] == 3636)
                    {
                        AddItem(62, 250, 7, 1, Player.CreatingCharID, 0);
                    }
                    #endregion

                    MsSQL.UpdateData("update character set min_phyatk='" + (int)Math.Round(MinPhyA) +
                            "', max_phyatk='"   + Math.Round(MaxPhyA) +
                            "', min_magatk='"   + Math.Round(MinMagA) +
                            "', max_magatk='"   + Math.Round(MaxMagA) +
                            "', phydef='"       + Math.Round(PhyDef) +
                            "', magdef='"       + Math.Round(PhyDef) +
                            "', parry='"        + Math.Round(Parry) +
                            "', hit='"          + Math.Round(Hit) +
                            "' where name='"    + name + "'");

                    AddMastery(257, Player.CreatingCharID); //blade
                    AddMastery(258, Player.CreatingCharID); //heuksal
                    AddMastery(259, Player.CreatingCharID); //bow
                    AddMastery(273, Player.CreatingCharID); //cold
                    AddMastery(274, Player.CreatingCharID); //light
                    AddMastery(275, Player.CreatingCharID); //fire
                    AddMastery(276, Player.CreatingCharID); //force
                    Console.WriteLine("@Gameserver:             Character: " + name + " Has been created");
                    client.Send(Packet.ScreenSuccess(1));

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CharacterCreate()");
                deBug.Write(ex);
                Console.WriteLine(ex);
            }
        }
        public void CharacterCheck(byte[] buff)
        {
            try
            {
                PacketReader Reader = new PacketReader(buff);
                Reader.Byte();

                string name = Reader.Text();
                Reader.Close();
                if (CharacterCheck(name))
                    client.Send(Packet.CharacterName(4));
                else client.Send(Packet.ScreenSuccess(4));
            }
            catch (Exception ex)
            {
                Console.WriteLine("CharacterCheck()");
                deBug.Write(ex);
                Console.WriteLine(ex);
            }
        }
        public bool CharacterCheck(string name)
        {
            bool tr = false;
            try
            {
                if (name.Length > 3 && name.Length < 12)
                {
                    string dbname = MsSQL.GetData("SELECT name FROM character WHERE name='" + name + "'", "name");
                    if (dbname == null)
                    {
                        tr = false;
                    }
                    else tr = true;
                }
                else tr = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CharacterCheck()");
                deBug.Write(ex);
                Console.WriteLine(ex);
            }
            return tr;
        }
        #endregion
        public void CheckCharStats(character ch)
        {
            if (ch.Stat.AttackPower < 0)
                Math.Abs(ch.Stat.AttackPower);
            if (ch.Stat.Hit < 0)
                Math.Abs(ch.Stat.Hit);
            if (ch.Stat.MagDef < 0)
                Math.Abs(ch.Stat.MagDef);
            if (ch.Stat.PhyDef < 0)
                Math.Abs(ch.Stat.PhyDef);
            if (ch.Stat.MinPhyAttack < 0)
                Math.Abs(ch.Stat.MinPhyAttack);
            if (ch.Stat.MinMagAttack < 0)
                Math.Abs(ch.Stat.MinMagAttack);
            if (ch.Stat.MaxPhyAttack < 0)
                Math.Abs(ch.Stat.MaxPhyAttack);
            if (ch.Stat.MaxMagAttack < 0)
                Math.Abs(ch.Stat.MaxMagAttack);
            if (ch.Information.XP < 0)
                Math.Abs(ch.Information.XP);


        }
        public void LoginScreen()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                string name = Reader.Text();
                Reader.Close();
                #region Check name before entering (exploit)
                Systems.MsSQL ms = new Systems.MsSQL("SELECT name FROM character WHERE account='" + Player.AccountName + "' AND name='" + name + "'");
                int checkinfo = ms.Count();

                if (checkinfo == 0)
                {
                    return;
                }
                #endregion
                else
                {
                    Character = new character();

                    Character.Information.Name = name;
                    Character.Account.ID = Player.ID;
                    PlayerDataLoad();
                    LoadJobData();
                    checkSameChar(name, Character.Information.UniqueID);
                    CheckCharStats(Character);
                    clients.Add(this);
                    client.Send(Packet.LoginScreen());
                    client.Send(Packet.StartPlayerLoad());
                    client.Send(Packet.Load(Character));
                   
                    client.Send(Packet.EndPlayerLoad());
                    client.Send(Packet.PlayerUnknowPack(Character.Information.UniqueID));
                    client.Send(Packet.UnknownPacket());

                    OpenTimer();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error login by: {0}",Character.Information.Name);
                deBug.Write(ex);
                Console.WriteLine(ex);
            }
        }

        void checkSameChar(string name, int id)
        {
            for (int i = 0; i < Systems.clients.Count; i++)
            {
                if (Systems.clients[i] != null && Systems.clients[i].Character.Information.Name == name || Systems.clients[i].Character.Information.UniqueID == id)
                {
                    Systems.clients[i].Disconnect("normal"); 
                }
            }
        }
        void InGameSuccess()
        {
        	try
        	{
        		if (!Character.InGame)
        		{
        			/////////////////////////////////////////////////////////////////////////////////
        			// Load spawns
        			/////////////////////////////////////////////////////////////////////////////////
        			ObjectSpawnCheck();
        			/////////////////////////////////////////////////////////////////////////////////
        			// Send packets required
        			/////////////////////////////////////////////////////////////////////////////////
        			client.Send(Packet.PlayerStat(Character));
        			client.Send(Packet.StatePack(Character.Information.UniqueID, 0x04, 0x02, false));
        			//client.Send(Packet.FORTRESSNOTE());
        			client.Send(Packet.Completeload());
        			client.Send(Packet.Silk(Player.Silk, Player.SilkPrem, 0));
        			/////////////////////////////////////////////////////////////////////////////////
        			// Load friends
        			/////////////////////////////////////////////////////////////////////////////////
        			GetFriendsList();
        			/////////////////////////////////////////////////////////////////////////////////
        			// Set player ingame status
        			/////////////////////////////////////////////////////////////////////////////////
        			MsSQL.UpdateData("UPDATE character SET online='1' WHERE id='" + Character.Information.CharacterID + "'");
        			this.Character.InGame = true;
        			/////////////////////////////////////////////////////////////////////////////////
        			// Load welcome message
        			/////////////////////////////////////////////////////////////////////////////////
        			LoadMessage();
        			/////////////////////////////////////////////////////////////////////////////////
        			// Guild + union
        			/////////////////////////////////////////////////////////////////////////////////
        			GetGuildData();
        			LoadUnions();
        			/////////////////////////////////////////////////////////////////////////////////
        			// Set player state
        			/////////////////////////////////////////////////////////////////////////////////
        			client.Send(Packet.StatePack(Character.Information.UniqueID, 0x04, 0x00, false));
        			/////////////////////////////////////////////////////////////////////////////////
        			// Update hp and mp of player
        			/////////////////////////////////////////////////////////////////////////////////
        			UpdateHp();
        			UpdateMp();
        			/////////////////////////////////////////////////////////////////////////////////
        			// Load transport if spawned
        			/////////////////////////////////////////////////////////////////////////////////
        			LoadTransport();
        			/////////////////////////////////////////////////////////////////////////////////
        			// Load grabpet if active state
        			/////////////////////////////////////////////////////////////////////////////////
        			//LoadGrabPet();
        			//set safe state for 5 sec.
        			this.Character.State.SafeState = true;
        			NotAttackableTimer(5000);
        			this.HPregen();
        			this.MPregen();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error login load: {0}",ex);
                deBug.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Load grabpet if active state
        /////////////////////////////////////////////////////////////////////////////////
        void LoadGrabPet()
        {
            try
            {
                #region Grab pet loading
                //Query check
                MsSQL ms = new MsSQL("SELECT * FROM pets WHERE playerid='" + Character.Information.CharacterID + "' AND pet_active='1'");
                //Get active pet count
                int checkactive = ms.Count();
                //If 1 pet is active then..
                if (checkactive > 0)
                {
                    //Spawn our pet
                    pet_obj o = new pet_obj();

                    using (SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            //Get slot of pet from inventory 12
                            string slot = reader.GetString(12);
                            int slotcheck = MsSQL.GetDataInt("SELECT * FROM char_items WHERE itemnumber='" + slot + "' AND owner='" + Character.Information.CharacterID + "' AND storagetype='0'", "slot");
                            Global.slotItem item = GetItem((uint)Character.Information.CharacterID, Convert.ToByte(slotcheck), 0);
                            int model = Global.objectdata.GetItem(Data.ItemBase[item.ID].ObjectName);

                            int itemid = reader.GetInt32(7);
                            Character.Grabpet.Grabpetid = item.dbID;
                            o.UniqueID = Character.Grabpet.Grabpetid;
                            o.Model = model;
                            o.x = Character.Position.x + rnd.Next(1, 3);
                            o.z = Character.Position.z;
                            o.y = Character.Position.y + rnd.Next(1, 3);
                            o.xSec = Character.Position.xSec;
                            o.ySec = Character.Position.ySec;
                            o.OwnerID = Character.Information.CharacterID;
                            o.OwnerName = Character.Information.Name;
                            o.Walking = Character.Position.Walking;
                            o.Petname = reader.GetString(3);
                            o.Named = 2;
                            o.Run = Character.Speed.RunSpeed;
                            o.Walk = Character.Speed.WalkSpeed;
                            o.Zerk = Character.Speed.BerserkSpeed;

                            Character.Grabpet.Active = true;
                            List<int> S = Character.Spawn;
                            o.Information = true;

                            Systems.HelperObject.Add(o);
                            Character.Grabpet.Details = o;

                            client.Send(Packet.Pet_Information_grab(o, Convert.ToByte(slotcheck)));
                            o.SpawnMe();

                            MsSQL.UpdateData("UPDATE pets SET pet_active='1' WHERE pet_unique='" + Character.Grabpet.Grabpetid + "' AND playerid='" + Character.Information.CharacterID + "'");
                        }
                        ms.Close();
                    }
                    //Set state
                    Character.Grabpet.Active = true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("Grab pet player load error {0}", ex);
                deBug.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Load transport if needed
        /////////////////////////////////////////////////////////////////////////////////
        void LoadTransport()
        {
            #region Transport
            if (Character.Transport.Right)
            {
                pet_obj o = Character.Transport.Horse;
                List<int> S = o.SpawnMe();

                Character.Transport.Spawned = true;
                Character.Transport.Horse.Information = true;

                client.Send(Packet.Pet_Information(o.UniqueID, o.Model, o.Hp, Character.Information.CharacterID, o));
                Send(S, Packet.Player_UpToHorse(this.Character.Information.UniqueID, true, o.UniqueID));

                Character.Transport.Horse.Speedsend();
                Character.Transport.Horse.statussend();
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Load our starting message
        /////////////////////////////////////////////////////////////////////////////////
        void LoadMessage()
        {
            #region Welcome message
            if (!Character.Information.WelcomeMessage)
            {
                Framework.Ini ini;
                ini = new Framework.Ini(Environment.CurrentDirectory + @"\Settings\Settings_Custom.ini");
                string servername = ini.GetValue("SERVER_MESSAGE", "Message").ToString();

                int wait = 3000;
                string welcome = "Welcome to " + servername + "";
                client.Send(sendnoticecon(7, 0, welcome, ""));
                for (int i = 1; i < wait + 1; i += 1)
                {
                    if (i == wait)
                    {
                        welcome = "Programmed by: http://xcoding.net";
                        client.Send(sendnoticecon(7, 0, welcome, ""));
                    }
                }
                Character.Information.WelcomeMessage = true;
            }
            #endregion
        }
        #region Message conv
        private string MessageToMessagelong(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Load our job information
        /////////////////////////////////////////////////////////////////////////////////
        void LoadJobData()
        { 
            #region Job Data
            try
            {
                //Query the database for information
                MsSQL ms = new MsSQL("SELECT * FROM character_jobs WHERE character_name='" + Character.Information.Name + "'");
                //Count rows to see if the player has a job
                int checkjob = ms.Count();
                //If the player has a job
                if (checkjob >= 0)
                {
                    using (SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            //Define information
                            Character.Job.Jobname = reader.GetString(2);
                            Character.Job.type = reader.GetByte(3);
                            Character.Job.exp = reader.GetInt32(4);
                            Character.Job.rank = reader.GetByte(5);
                            Character.Job.state = reader.GetInt32(6);
                            Character.Job.level = reader.GetByte(7);
                            //If jobtype = Trader
                            if (Character.Job.type == 1)
                            {

                            }
                            //If jobtype = Hunter
                            if (Character.Job.type == 2)
                            {
                                client.Send(Packet.JoinMerchant(Character.Information.CharacterID, 3));

                            }
                            //If jobtype = Thief
                            if (Character.Job.type == 3)
                            {

                            }
                        }
                    }
                }
                ms.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Job load error {0}", ex);
                deBug.Write(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Load our personal friends list.
        /////////////////////////////////////////////////////////////////////////////////
        void GetFriendsList()
        {
            #region Friend list
            try
            {
                MsSQL ms = new MsSQL("SELECT * FROM friends WHERE owner='"+ Character.Information.CharacterID +"'");
                int count = ms.Count();
                if (count >= 0)
                {
                    client.Send(Packet.SendFriendList(Convert.ToByte(count), Character));
                    using (SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            int getid = reader.GetInt32(2);
                            Systems sys = GetPlayerid(getid);

                            if (sys != null)
                            {
                                sys.client.Send(Packet.FriendData(Character.Information.CharacterID, 4, Character.Information.Name, Character, false));
                            }
                        }
                    }
                }
                else
                {
                    //Send nothing
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading friends list: " + ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Load our guild
        /////////////////////////////////////////////////////////////////////////////////
        void GetGuildData()
        {
            #region Guild

            MsSQL ms = new MsSQL("SELECT * FROM guild_members WHERE guild_member_id='" + Character.Information.CharacterID + "'");
            int checkinguild = ms.Count();
            try
            {
                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        int Guildid = reader.GetInt32(1);
                        guild sys = GetGuildInfo(Guildid);
                        if (checkinguild == 1 && Character.Guild.GuildID == sys.Guildid)
                        {
                            Character.Network.GuildSystem = sys;
  
                            sys.Members.Add(this.Character.Information.CharacterID);
                            sys.MembersClient.Add(this.client);
                            
                            Character.Guild.Inguild = true;
                            client.Send(Packet.SendGuildStart());
                            client.Send(Packet.SendGuildInfo(this.Character));
                            client.Send(Packet.SendGuildEnd());
                            client.Send(Packet.SendGuildInfo2(this.Character));
                            Character.Network.GuildSystem.Send(Packet.GuildUpdate(Character,6,Character.Information.CharacterID,0));
                        }
                    }
                }
                ms.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Load our unions
        /////////////////////////////////////////////////////////////////////////////////
        public void LoadUnions()
        {
            //Redoing this
            #region Load unions
            /*try
            {
                MsSQL ms2 = new MsSQL("SELECT * FROM guild_unions WHERE union_leader='" + Character.Guild.MyGuildName + "'");
                int checkunion = ms2.Count();

                if (checkunion != 0)
                {
                    using (SqlDataReader reader = ms2.Read())
                    {
                        for (int i = 0; i < checkunion; )
                        {
                            while (reader.Read())
                            {
                                client.Send(Packet.UnionInfo(
                                    Character.Information.CharacterID,
                                    Character.Guild.GuildID,
                                    Character.Guild.UnionGuildID,
                                    Character.Guild.UnionGuildLeaderID,
                                    Character.Guild.UnionCount,
                                    Character.Guild.UnionGuildName,
                                    Character.Guild.UnionGuildLeader,
                                    Character.Guild.UnionLevel,
                                    Character.Guild.UnionGuildLeaderModel,
                                    Character.Guild.UnionMemberCount
                                    ));
                            }
                        }
                    }
                    ms2.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
             */
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Load player data
        /////////////////////////////////////////////////////////////////////////////////
        public void PlayerDataLoad()
        {
            #region Load Player Information
            try
            {
                //////////////////////////////////////////////////////////////////////////////////////
                // Load Character Data
                //////////////////////////////////////////////////////////////////////////////////////
                #region Character Data
                if (Character == null) return;
                MsSQL ms = new MsSQL("SELECT * FROM character WHERE name='" + Character.Information.Name + "'");

                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        // Character id information
                        Character.Information.CharacterID    = reader.GetInt32(0);
                        Character.Ids                        = new Global.ID(Character.Information.CharacterID, Global.ID.IDS.Player);
                        Character.Information.UniqueID       = Character.Ids.GetUniqueID;
                        Character.Account.ID                 = Player.ID;
                        // Character model information
                        Character.Information.Model          = reader.GetInt32(3);
                        Character.Information.Volume         = reader.GetByte(4);
                        // Character base stats
                        Character.Information.Level          = reader.GetByte(5);
                        Character.Stat.Strength              = reader.GetInt16(6);
                        Character.Stat.Intelligence          = reader.GetInt16(7);
                        Character.Information.Attributes     = reader.GetInt16(8);
                        Character.Stat.Hp                    = reader.GetInt32(9);
                        Character.Stat.Mp                    = reader.GetInt32(10);
                        // Character gold information
                        Character.Information.Gold           = reader.GetInt64(11);
                        // Character Points
                        Character.Information.XP             = reader.GetInt64(12);
                        Character.Information.SpBar          = reader.GetInt32(13);
                        Character.Information.SkillPoint     = reader.GetInt32(14);
                        // Character GM information
                        Character.Information.GM             = reader.GetByte(15);
                        // Character Location
                        Character.Position.xSec              = reader.GetByte(16);
                        Character.Position.ySec              = reader.GetByte(17);
                        Character.Position.x                 = reader.GetInt32(19);
                        Character.Position.y                 = reader.GetInt32(20);
                        Character.Position.z                 = reader.GetInt32(21);
                        Character.Information.Place          = reader.GetByte(40);
                        // Character Main Stats
                        Character.Stat.SecondHp              = reader.GetInt32(22);
                        Character.Stat.SecondMP              = reader.GetInt32(23);
                        Character.Stat.MinPhyAttack          = reader.GetInt32(24);
                        Character.Stat.MaxPhyAttack          = reader.GetInt32(25);
                        Character.Stat.MinMagAttack          = reader.GetInt32(26);
                        Character.Stat.MaxMagAttack          = reader.GetInt32(27);
                        Character.Stat.PhyDef                = reader.GetInt16(28);
                        Character.Stat.MagDef                = reader.GetInt16(29);
                        Character.Stat.Hit                   = reader.GetInt16(30);
                        Character.Stat.Parry                 = reader.GetInt16(31);
                        Character.Speed.WalkSpeed            = reader.GetInt32(33);
                        Character.Speed.RunSpeed             = reader.GetInt32(34);
                        Character.Speed.BerserkSpeed         = reader.GetInt32(35);
                        Character.Information.BerserkBar     = reader.GetByte(36);
                        Character.Speed.DefaultSpeed         = Character.Speed.RunSpeed;
                        Character.Stat.mag_Absorb            = reader.GetInt16(38);
                        Character.Stat.phy_Absorb            = reader.GetInt16(39);
                        // Character Other information
                        Character.Information.Pvpstate       = reader.GetByte(45);
                        Character.Account.StorageGold        = Player.pGold;
                        Character.State.type1                = 1;
                        Character.LogNum                     = 53;
                        Character.Information.Slots          = reader.GetInt32(44);
                        Character.Information.Title          = reader.GetByte(41);
                        Character.Information.Online         = reader.GetInt32(47);
                        Character.Information.StallModel     = reader.GetInt32(52);
                        // Character Guide Info
                        Character.Guideinfo.G1 = new int[8];//Main Int Array Fro Guide Packet
                        Character.Guideinfo.Gchk = new int[8];//Main Guide Check Packet Array

                         
                        string Guideread                     = reader.GetString(51);
                        int t = 0;
                        for (int g = 0; g < 8; ++g)//READ FROM DATABASE FOR GUIDE
                        {
                            Character.Guideinfo.G1[g] = int.Parse(Guideread.Substring(t,2),System.Globalization.NumberStyles.HexNumber, null);
                            t = t + 2;
                        }

                        for (int gc = 0; gc < 8; ++gc)//Sets The Check Data At 0 On Start
                        {
                            Character.Guideinfo.Gchk[gc] = 0;
                        }
                        //#########################END GUIDE DATA#################################################
                    }
                }
                ms.Close();
                #endregion
                //////////////////////////////////////////////////////////////////////////////////////
                // Set Skill And Mastery Information Max
                //////////////////////////////////////////////////////////////////////////////////////
                #region Set Skill / Mastery
                Character.Stat.Skill.Mastery         = new int[8];
                Character.Stat.Skill.Mastery_Level   = new byte[8];
                Character.Stat.Skill.Skill           = new int[20000];
                #endregion
                //////////////////////////////////////////////////////////////////////////////////////
                // Load Mastery Data
                //////////////////////////////////////////////////////////////////////////////////////
                #region Load Mastery Data
                ms = new MsSQL("SELECT * FROM mastery WHERE owner='" + Character.Information.CharacterID + "'");
                using (SqlDataReader reader = ms.Read())
                {
                    byte c = 1;
                    while (reader.Read())
                    {
                        Character.Stat.Skill.Mastery[c]          = reader.GetInt32(1);
                        Character.Stat.Skill.Mastery_Level[c]    = reader.GetByte(2);
                        c++;
                    }
                }
                ms.Close();
                #endregion
                //////////////////////////////////////////////////////////////////////////////////////
                // Load Skill Data
                //////////////////////////////////////////////////////////////////////////////////////
                #region Load Mastery Data
                ms = new MsSQL("SELECT * FROM saved_skills WHERE owner='" + Character.Information.CharacterID + "'");
                using (SqlDataReader reader = ms.Read())
                    Character.Stat.Skill.AmountSkill = ms.Count();
                int i = 1;
                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        Character.Stat.Skill.Skill[i] = reader.GetInt32(2);
                        i++;
                    }
                }
                ms.Close();
                #endregion
                //////////////////////////////////////////////////////////////////////////////////////
                // Load Guild Player Data
                //////////////////////////////////////////////////////////////////////////////////////
                #region Load Guil Player Data
                ms = new MsSQL("SELECT * FROM guild_members WHERE guild_member_id='" + Character.Information.CharacterID + "'");
                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        Character.Guild.GuildID         = reader.GetInt32(1);
                        Character.Guild.MyGuildName     = MsSQL.GetData("SELECT guild_name FROM guild WHERE id='" + Character.Guild.GuildID + "'", "guild_name");
                        Character.Guild.GuildMemberId   = reader.GetInt32(2);
                        Character.Guild.MyGuildRank     = reader.GetByte(3);
                        Character.Guild.MyGuildPoints   = reader.GetInt32(4);
                        Character.Guild.MyGuildAuth     = reader.GetInt32(5);
                        Character.Guild.MyGrant         = reader.GetString(6);
                    }
                }
                ms.Close();
                #endregion
                //////////////////////////////////////////////////////////////////////////////////////
                // Load Guild Info Data
                //////////////////////////////////////////////////////////////////////////////////////
                #region Load Guild Information
                ms = new MsSQL("SELECT * FROM guild WHERE guild_name='" + Character.Guild.MyGuildName + "'");
                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        Character.Guild.Name = Convert.ToString(reader.GetString(1));
                        Character.Guild.Level = reader.GetByte(2);
                        Character.Guild.GuildPoints = reader.GetInt32(3);
                        Character.Guild.MessageTitle = Convert.ToString(reader.GetString(4));
                        Character.Guild.Message = Convert.ToString(reader.GetString(5));
                        Character.Guild.GuildMemberCount = reader.GetByte(6);
                        Character.Guild.GuildStorageSlots = reader.GetInt32(7);
                        Character.Guild.GuildWarGold = reader.GetInt32(8);
                    }
                }
                ms.Close();
                #endregion
                //////////////////////////////////////////////////////////////////////////////////////
                // Add Player To Guild Network
                //////////////////////////////////////////////////////////////////////////////////////
                #region Add Member To Network List
                //Need to add list for guild users
                //if (Character.Guild.GuildID != 0)
                //{
                    //Make check list
                    //List<string> GuildNames = Character.GuildNetWork.GuildList;
                    //Systems Guild = GetPlayer();
                    //Check what guild
                    //if (GuildNames.Contains(Character.Guild.Name))
                    //{
                    //
                    //Character.Network.GuildNetwork.UniqueGuildMembers.Add(Character.Information.UniqueID);
                    //Character.Network.GuildNetwork.MembersClient.Add(client);
                    //}
                //}
                #endregion
                //////////////////////////////////////////////////////////////////////////////////////
                // Load Union Info Data
                //////////////////////////////////////////////////////////////////////////////////////
                #region Load Union Data
                ms = new MsSQL("SELECT * FROM guild_unions WHERE union_leader='" + Character.Guild.MyGuildName + "'");
                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        Character.Guild.UnionCount      = ms.Count();
                        Character.Guild.UnionGuildName  = reader.GetString(2);
                    }
                }
                ms.Close();
                #endregion
                /*
                //////////////////////////////////////////////////////////////////////////////////////
                // Load Union Detail Data
                //////////////////////////////////////////////////////////////////////////////////////
                #region Load Union Details
                ms = new MsSQL("SELECT * FROM guild_members WHERE guild_id='" + Character.Guild.GuildID + "'");
                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        Character.Guild.GuildID = reader.GetInt32(1);
                        Character.Guild.UnionGuildLeaderID = reader.GetInt32(2);
                        Character.Guild.UnionMemberCount = ms.Count();
                    }
                }
                ms.Close();
                ms = new MsSQL("SELECT * FROM guild WHERE guild_name='" + Character.Guild.UnionGuildName + "'");
                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        Character.Guild.UnionGuildID = reader.GetInt32(0);
                    }
                }
                ms.Close();
                ms = new MsSQL("SELECT * FROM character WHERE id='" + Character.Guild.UnionGuildLeaderID + "'");
                using (SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        Character.Guild.UnionGuildLeaderModel = reader.GetInt32(3);
                        Character.Guild.UnionGuildLeader = reader.GetString(2);
                    }
                }
                ms.Close();
                #endregion
                 */
                //////////////////////////////////////////////////////////////////////////////////////
                // Load Second Batch
                //////////////////////////////////////////////////////////////////////////////////////
                #region Load Secondary Data
                UpdateXY();
                if (Character.Stat.SecondHp < 1)
                {
                    Character.Stat.SecondHp = (Character.Stat.Hp / 2);
                    Teleport_UpdateXYZ(Character.Information.Place);
                }
                SetBalance();
                CheckFile();
                SetStat();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("Player Data: {0} Error ",ex);
                deBug.Write(ex);
                Console.WriteLine(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Load weather
        /////////////////////////////////////////////////////////////////////////////////
        void LoadWeather()
        {
            #region Weather
            //Weather should be defined per zone
            Ini ini = new Ini();
            ini = new Framework.Ini(Environment.CurrentDirectory + @"\Settings\Settings_Custom.ini");
            string Enabled = ini.GetValue("WEATHER_ENABLED", "Enabled").ToString();
            string Type = ini.GetValue("WEATHER_TYPE", "Type").ToString();
            string Info = ini.GetValue("WEATHER_INTENSITY", "Info").ToString();
            if (Enabled == "1")
            {
                client.Send(Packet.Weather(Convert.ToByte(Type), Convert.ToInt32(Info)));
            }
            else
            {
                client.Send(Packet.Weather(0, 1));
            }
            //client.Send(Packet.SnowFlakeEvent());
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Update gamex / y
        /////////////////////////////////////////////////////////////////////////////////
        void UpdateXY()
        {
            Character.Position.x = Function.Formule.gamex(Character.Position.x, Character.Position.xSec);
            Character.Position.y = Function.Formule.gamey(Character.Position.y, Character.Position.ySec);
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Load player quick bar and autopotion
        /////////////////////////////////////////////////////////////////////////////////
        void CheckFile()
        {
            string player_path = Environment.CurrentDirectory + @"\player\info\";
            if (!System.IO.File.Exists(player_path + @"quickbar\" + Character.Information.Name + ".dat"))
            {
                byte[] by = new byte[255];
                for (byte i = 0; i <= 254; i++) by[i] = 0x00;
                System.IO.File.Create(player_path + @"quickbar\" + Character.Information.Name + ".dat").Close();
                System.IO.File.WriteAllBytes(player_path + @"quickbar\" + Character.Information.Name + ".dat", by);
            }
            if (!System.IO.File.Exists(player_path + @"autopot\" + Character.Information.Name + ".dat"))
            {
                byte[] by = new byte[255];
                for (byte i = 0; i <= 6; i++) by[i] = 0x00;
                System.IO.File.Create(player_path + @"autopot\" + Character.Information.Name + ".dat").Close();
                System.IO.File.WriteAllBytes(player_path + @"autopot\" + Character.Information.Name + ".dat", by);
            }
            if (!System.IO.File.Exists(player_path + @"debug\" + Character.Information.Name + ".txt"))
            {
                System.IO.File.Create(player_path + @"debug\" + Character.Information.Name + ".txt").Close();
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Saving related
        /////////////////////////////////////////////////////////////////////////////////
        public void SaveGuideInfo()//Guide Save To DB
        {
            try
            {
                string GuideHex = "";
                for (int i = 0; i < 8; ++i)
                {
                    string Numlen = String.Format("{0:X}", Character.Guideinfo.G1[i]);
                    if (Numlen.Length == 1) Numlen = "0" + Numlen;
                    GuideHex = GuideHex + Numlen;
                }
                MsSQL.UpdateData("update character set GuideData='" + GuideHex +
             "' where id='" + Character.Information.CharacterID + "'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Saving Guide error: ");
                Console.WriteLine(ex);
            }
        }
		public void SavePlayerHPMP()
		{
			try
			{
				MsSQL.UpdateData(string.Format("update character set s_hp='{0}', s_mp='{1}' where id='{2}'", 
						Character.Stat.SecondHp, Character.Stat.SecondMP, Character.Information.CharacterID));
			}
			catch (Exception ex)
            {
                Console.WriteLine("Saving player's HP/MP error: ");
                Console.WriteLine(ex);
            }
		}
        public void SavePlayerInfo()
        {
            try
            {
                MsSQL.UpdateData("update character set min_phyatk='" + Convert.ToInt32(Math.Round(Character.Stat.MinPhyAttack)) +
                    "', max_phyatk='" + Convert.ToInt32(Math.Round(Character.Stat.MaxPhyAttack)) +
                    "', min_magatk='" + Convert.ToInt32(Math.Round(Character.Stat.MinMagAttack)) +
                    "', max_magatk='" + Convert.ToInt32(Math.Round(Character.Stat.MaxMagAttack)) +
                    "', phydef='" + Convert.ToInt32(Math.Round(Character.Stat.PhyDef - Character.Stat.uPhyDef)) +
                    "', magdef='" + Convert.ToInt32(Math.Round(Character.Stat.MagDef - Character.Stat.uMagDef)) +
                    "', hit='" + Convert.ToInt16(Math.Round(Character.Stat.Hit)) +
                    "', parry='" + Convert.ToInt16(Math.Round(Character.Stat.Parry)) +
                    "', hp='" + Character.Stat.Hp +
                    "', mp='" + Character.Stat.Mp +
                    "', s_hp='" + Character.Stat.SecondHp +
                    "', s_mp='" + Character.Stat.SecondMP +
                    "', attribute='" + Character.Information.Attributes +
                    "', strength='" + Character.Stat.Strength +
                    "', intelligence='" + Character.Stat.Intelligence +
                    "', experience='" + Convert.ToInt64(Character.Information.XP) +
                    "', spbar='" + Character.Information.SpBar +
                    "', sp='" + Character.Information.SkillPoint +
                    "', level='" + Character.Information.Level +
                    "', mag_absord='" + Character.Stat.mag_Absorb +
                    "', phy_absord='" + Character.Stat.phy_Absorb +
                    "' where id='" + Character.Information.CharacterID + "'");
                    SaveGuideInfo();//Call To Update Guide In DB
            }
            catch (Exception ex)
            {
                Console.WriteLine("Saving player error: ");
                Console.WriteLine(ex);
            }
        }
        public void SavePlayerExperince()
        {
            try
            {
                MsSQL.UpdateData("update character set  experience='" + Convert.ToInt64(Character.Information.XP) +
                    "', spbar='" + Character.Information.SpBar +
                    "', sp='" + Character.Information.SkillPoint +
                    "' where id='" + Character.Information.CharacterID + "'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Save Experience error: ");
                Console.WriteLine(ex);
            }
        }
        protected void SavePlayerPosition()
        {
            MsSQL.UpdateData("update character set xsect='" + Character.Position.xSec +
                "', ysect='" + Character.Position.ySec +
                "', xpos='" + Math.Round(Function.Formule.packetx(Character.Position.x, Character.Position.xSec)) +
                "', ypos='" + Math.Round(Function.Formule.packety(Character.Position.y, Character.Position.ySec)) + 
                "', zpos='" + Math.Round(Character.Position.z) + 
                "' where id='" + Character.Information.CharacterID + "'");
        }
        protected void SavePlayerReturn()
        {
            MsSQL.UpdateData("update character_rev set revxsec='" + Character.Position.xSec +
                "', revysec='" + Character.Position.ySec +
                "', revx='" + Math.Round(Function.Formule.packetx(Character.Position.x, Character.Position.xSec)) +
                "', revy='" + Math.Round(Function.Formule.packety(Character.Position.y, Character.Position.ySec)) +
                "', revz='" + Math.Round(Character.Position.z) +
                "' where charname='" + Character.Information.Name + "'");
        }
        protected void SetBalance()
        {
            int maxstat = 28 + Character.Information.Level * 4;
            Character.Information.Phy_Balance = (byte)(99 - (100 * 2 / 3 * (maxstat - Character.Stat.Strength) / maxstat));
            Character.Information.Mag_Balance = (byte)(100 * Character.Stat.Intelligence / maxstat);
        }
        protected void SaveGold()
        {
            MsSQL.UpdateData("update character set gold='" + Character.Information.Gold +
                "' where id='" + Character.Information.CharacterID + "'");

            MsSQL.UpdateData("update users set gold='" + Character.Account.StorageGold +
                "' where id='" + Player.AccountName + "'");
        }
        protected void Save()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            string player_path;
            byte[] file;
            switch (Reader.Byte())
            {
                case 1:
                    player_path = Environment.CurrentDirectory + @"\player\info\quickbar\" + Character.Information.Name + ".dat";
                    file = System.IO.File.ReadAllBytes(player_path);

                    byte Slot = Reader.Byte();
                    byte sType = Reader.Byte();

                    Slot *= 5;

                    file[Slot] = sType;
                    file[Slot + 1] = Reader.Byte();
                    file[Slot + 2] = Reader.Byte();
                    file[Slot + 3] = Reader.Byte();
                    file[Slot + 4] = Reader.Byte();
                    System.IO.File.WriteAllBytes(player_path, file);
                    break;
                case 2:
                    player_path = Environment.CurrentDirectory + @"\player\info\autopot\" + Character.Information.Name + ".dat";
                    file = System.IO.File.ReadAllBytes(player_path);
                    file[0] = Reader.Byte();
                    file[1] = Reader.Byte();
                    file[2] = Reader.Byte();
                    file[3] = Reader.Byte();
                    file[4] = Reader.Byte();
                    file[5] = Reader.Byte();
                    file[6] = Reader.Byte();
                    System.IO.File.WriteAllBytes(player_path, file);
                    UpdateHp();
                    UpdateMp();
                    break;
            }
            Reader.Close();
        }
        protected void SavePlace()
        {

            PacketReader reader = new PacketReader(PacketInformation.buffer);
            int ObjectID = reader.Int32();
            obj o = GetObject(ObjectID);
            byte type = 0;
            switch (Data.ObjectBase[o.ID].Name)
            {
                case "STORE_CH_GATE":
                    type = 1;
                    break;
                case "STORE_WC_GATE":
                    type = 2;
                    break;
                case "STORE_KT_GATE":
                    type = 5;
                    break;
                case "STORE_EU_GATE":
                    type = 20;
                    break;
                case "STORE_CA_GATE":
                    type = 25;
                    break;
                case "STORE_SD_GATE1":
                    type = 175;
                    break;
                case "STORE_SD_GATE2":
                    type = 176;
                    break;
            }
            Character.Information.Place = type;
            MsSQL.InsertData("update character set savearea='" + Character.Information.Place + "' where id='" + Character.Information.CharacterID + "'");
            client.Send(Packet.UpdatePlace());
        }
        public void UpdateHp()
        {
            Send(Packet.UpdatePlayer(this.Character.Information.UniqueID, 0x20, 1, this.Character.Stat.SecondHp));
            if (Character.Network.Party != null)
            {
                Character.Network.Party.Send(Packet.Party_Data(6, this.Character.Information.UniqueID));
            }
        }
        public void UpdateHp(character Character)
        {
            Send(Packet.UpdatePlayer(Character.Information.UniqueID, 0x20, 1, Character.Stat.SecondHp));
        }
        public void UpdateMp()
        {
            Send(Packet.UpdatePlayer(this.Character.Information.UniqueID, 0x10, 2, this.Character.Stat.SecondMP));
            if (Character.Network.Party != null)
            {
                Character.Network.Party.Send(Packet.Party_Data(6, this.Character.Information.UniqueID));
            }
        }
        public void SetStat()
        {
            Character.Stat.Hp = Function.Formule.gamePhp(Character.Information.Level, Character.Stat.Strength);
            Character.Stat.Mp = Function.Formule.gamePmp(Character.Information.Level, Character.Stat.Intelligence);
            client.Send(Packet.PlayerStat(Character));
        }
        public void UpdateStrengthInfo(sbyte amount)
        {
            Character.Stat.MinPhyAttack += (0.45 * amount);
            Character.Stat.MaxPhyAttack += (0.65 * amount);
            Character.Stat.PhyDef += (0.40 * amount);
        }
        public void UpdateIntelligenceInfo(sbyte amount)
        {
            Character.Stat.MinMagAttack += (0.45 * amount);
            Character.Stat.MaxMagAttack += (0.65 * amount);
            Character.Stat.MagDef += (0.40 * amount);
        }
        public void InsertStr()
        {
            try
            {
                if (Character.Information.Attributes > 0)
                {
                    Character.Information.Attributes -= 1;
                    Character.Stat.Strength++;
                    UpdateStrengthInfo(1);
                    client.Send(Packet.UpdateStr());
                    SetStat();
                    SavePlayerInfo();
                }
            }
            catch(Exception ex)
            {
                Console.Write("InsertStr()");
                deBug.Write(ex);
                Console.WriteLine(ex);
            }
        }
        public void InsertInt()
        {
            try
            {
                if (Character.Information.Attributes > 0)
                {
                    Character.Information.Attributes -= 1;
                    Character.Stat.Intelligence++;
                    UpdateIntelligenceInfo(1);
                    client.Send(Packet.UpdateInt());
                    SetStat();
                    SavePlayerInfo();
                }
            }
            catch (Exception ex)
            {
                Console.Write("InsertInt()");
                deBug.Write(ex);
                Console.WriteLine(ex);
            }
        }
        protected void Close_NPC()
        {
            Character.State.Busy = false;
            client.Send(Packet.CloseNPC());
        }
        protected void Open_NPC()
        {
            try
            {
                Character.State.Busy = true;
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Reader.UInt32();
                byte type = Reader.Byte();

                if (type == 1)
                {
                    client.Send(Packet.OpenNPC(type));
                }
                else
                {
                    client.Send(Packet.OpenNPC(type));
                }
                Reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

        }
        protected void Open_Warehouse()
        {
            try
            {
                client.Send(Packet.OpenWarehouse(Character.Account.StorageGold));
                client.Send(Packet.OpenWarehouse2(0x96, this.Player));//Add storage slots count
                client.Send(Packet.OpenWarehouse3());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warehouse open error : {0}",ex);
            }
        }
    }
}

