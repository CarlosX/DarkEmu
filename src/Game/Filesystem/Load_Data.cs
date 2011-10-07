///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.IO;
using DarkEmu_GameServer.Global;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;


namespace DarkEmu_GameServer.File
{
    public class FileLoad
    {
        /////////////////////////////////////////////////////////////////////////////
        // File loading
        /////////////////////////////////////////////////////////////////////////////
        public static void Load()
        {
            //Load skill data
            Load_Skills.LoadSkillData(@"\data\Skills\skilldata_5000.txt");
            Load_Skills.LoadSkillData(@"\data\Skills\skilldata_10000.txt");
            Load_Skills.LoadSkillData(@"\data\Skills\skilldata_15000.txt");
            Load_Skills.LoadSkillData(@"\data\Skills\skilldata_20000.txt");
            Load_Skills.LoadSkillData(@"\data\Skills\skilldata_25000.txt");
            Load_Skills.LoadSkillData(@"\data\Skills\skilldata_30000.txt");
            Load_Skills.LoadSkillData(@"\data\Skills\skilldata_35000.txt");
            //Load timer items (Example: 30days rental items).
            Load_Items.SetTimerItems(@"\data\Items\refrentitem.txt");
            //Load Item data
            Load_Items.ItemDatabase(@"\data\Items\itemdata_5000.txt");
            Load_Items.ItemDatabase(@"\data\Items\itemdata_10000.txt");
            Load_Items.ItemDatabase(@"\data\Items\itemdata_15000.txt");
            Load_Items.ItemDatabase(@"\data\Items\itemdata_20000.txt");
            Load_Items.ItemDatabase(@"\data\Items\itemdata_25000.txt");
            Load_Items.ItemDatabase(@"\data\Items\itemdata_30000.txt");
            Load_Items.ItemDatabase(@"\data\Items\itemdata_35000.txt");
            Load_Items.ItemDatabase(@"\data\Items\itemdata_40000.txt");
            //Load event items
            Load_Items.EventItems(@"\data\Event\Event_Drops.txt");
            Load_Items.EventMonsters(@"\data\Event\Event_Monsters.txt");
            //Load objects
            Load_Objects.ObjectDataBase(@"\data\Objects\characterdata_5000.txt");
            Load_Objects.ObjectDataBase(@"\data\Objects\characterdata_10000.txt");
            Load_Objects.ObjectDataBase(@"\data\Objects\characterdata_15000.txt");
            Load_Objects.ObjectDataBase(@"\data\Objects\characterdata_20000.txt");
            Load_Objects.ObjectDataBase(@"\data\Objects\characterdata_25000.txt");
            Load_Objects.ObjectDataBase(@"\data\Objects\characterdata_30000.txt");
            Load_Objects.ObjectDataBase(@"\data\Objects\characterdata_35000.txt");
            Load_Objects.ObjectDataBase(@"\data\Objects\characterdata_40000.txt");
            //Load drop databases
            Load_Drops.Databases();

            LoadNpcs();
            LoadObject();
            LoadQuestData(@"\data\questdata.txt");
            LoadCaveTeleports();
            cavedata();
            TeleportBuilding();
            LoadCaveTeleports();
            LoadCaves();
            LoadTeleportPrice();
            LoadRegionCodes();
            LoadMasteryData();
            LoadGoldData();
            LoadJobLevels();
            LoadLevelData();
            LoadShopTabData();
            LoadMagicOptions();
            ReverseData();
        }

        /////////////////////////////////////////////////////////////////////////////
        // Load objects
        /////////////////////////////////////////////////////////////////////////////
        public static void LoadObject()
        {

            //Junk ........ ugh
            TxtFile.ReadFromFile(@"\data\npcpos.txt", '\t');
            string Spawninfo = Systems.Rate.Spawns.ToString();
            string s = null;
            int count = TxtFile.amountLine;
            uint index = 0;
            int npcamount = Convert.ToInt16(Spawninfo);
            int countme = 0;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int ID = Convert.ToInt32(TxtFile.commands[0]);
                string namecheck = Data.ObjectBase[ID].Name;

                if (namecheck.Contains("MOB_"))
                {
                    countme += 1;

                    //Should redo this , if static non moving monster , set AROUND , random location from source location.
                    if (ID == 1979 || ID == 2101 || ID == 2124 || ID == 2111 || ID == 2112) 
                        npcamount = 1;


                    for (int i = 1; i <= npcamount; i++)
                    {
                        obj o = new obj();
                        index++;
                        short AREA = short.Parse(TxtFile.commands[1]);
                        float x = Convert.ToInt32(TxtFile.commands[2]);
                        float z = Convert.ToInt32(TxtFile.commands[3]);
                        float y = Convert.ToInt32(TxtFile.commands[4]);

                        //:S localtype not needed just set state speed information
                        if (ID == 1979 || ID == 2101 || ID == 2124 || ID == 2111 || ID == 2112)
                        {
                            o.AutoMovement = false;
                            o.LocalType = Data.ObjectBase[ID].Type;
                        }
                        else
                        {
                            o.AutoMovement = true;
                            o.LocalType = Data.ObjectBase[ID].Type;
                        }

                        o.OrgMovement = o.AutoMovement;
                        if (o.AutoMovement)
                            o.StartRunTimer(Global.RandomID.GetRandom(500, 2000));
                        o.ID = ID;
                        o.Ids = new Global.ID(Global.ID.IDS.Object);
                        o.UniqueID = o.Ids.GetUniqueID;

                        o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                        o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                        o.x = (o.xSec - 135) * 192 + (x) / 10;
                        o.z = z;
                        o.y = (o.ySec - 92) * 192 + (y) / 10;

                        o.oX = o.x;
                        o.oY = o.y;
                        Systems.aRound(ref o.oX, ref o.oY, 9);
                        o.State = 1;
                        o.Move = 1;
                        o.AutoSpawn = true;
                        o.State = 2;
                        o.HP = Data.ObjectBase[ID].HP;
                        o.Kat = 1;
                        o.Agro = new List<_agro>();
                        o.SpeedWalk = Data.ObjectBase[o.ID].SpeedWalk;
                        o.SpeedRun = Data.ObjectBase[o.ID].SpeedRun;
                        o.SpeedZerk = Data.ObjectBase[o.ID].SpeedZerk;
                        o.oldAgresif = o.Agresif;
                        if (o.Type == 1) o.Agresif = 1;
                        //if (o.Type == 0) o.Agresif = 0;
                        o.spawnOran = 20;
                        if (ID == 1979 || ID == 2101 || ID == 2124 || ID == 2111 || ID == 2112) o.AutoMovement = false;
                        o.OrgMovement = o.AutoMovement;

                        if (o.AutoMovement) o.StartRunTimer(Global.RandomID.GetRandom(500, 2000));

                        if (Data.ObjectBase[ID].ObjectType != 3)
                        {
                            o.Type = Systems.RandomType(Data.ObjectBase[ID].Level, ref o.Kat, false, ref o.Agresif);
                            o.HP *= o.Kat;
                            if (o.Type == 1)
                                o.Agresif = 1;
                            Systems.Objects.Add(o);

                        }
                        else
                        {
                            o.AutoSpawn = false;
                            o.Type = Data.ObjectBase[ID].ObjectType; GlobalUnique.AddObject(o);
                            
                        }
                        if (namecheck.Contains("CH")) Data.ObjectBase[ID].Race = 0;
                        if (namecheck.Contains("EU")) Data.ObjectBase[ID].Race = 1;
                        if (!namecheck.Contains("CH") && (!namecheck.Contains("EU"))) Data.ObjectBase[ID].Race = 2;
                    }
                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " monsters");
        }
        /////////////////////////////////////////////////////////////////////////////
        // Load npcs
        /////////////////////////////////////////////////////////////////////////////
        public static void LoadNpcs()
        {
            TxtFile.ReadFromFile(@"\data\npcpos.txt", '\t');
            string input = null;
            string s = null;
            string[] npcangle1;
            int count = TxtFile.amountLine;
            uint index = 0;
            int countme = 0;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int ID = Convert.ToInt32(TxtFile.commands[0]);
                byte race = Data.ObjectBase[ID].Type;
                string namecheck = Data.ObjectBase[ID].Name;
                
                if (namecheck.Contains("NPC_"))
                {
                    countme += 1;
                    TextReader Npcangle = new StreamReader(Environment.CurrentDirectory + @"\data\NpcAngles.txt");
                    obj o = new obj();
                    index++;
                    short AREA = short.Parse(TxtFile.commands[1]);
                    double x = Convert.ToDouble(TxtFile.commands[2].Replace('.', ','));
                    double z = Convert.ToDouble(TxtFile.commands[3].Replace('.', ','));
                    double y = Convert.ToDouble(TxtFile.commands[4].Replace('.', ','));


                    byte movement = 0;
                    o.Agresif = movement;
                    o.AutoMovement = true;
                    o.ID = ID;
                    o.Ids = new Global.ID(Global.ID.IDS.Object);
                    o.UniqueID = o.Ids.GetUniqueID;
                    o.area = AREA;
                    o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                    o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                    o.x = (o.xSec - 135) * 192 + (x) / 10;
                    o.z = z;
                    o.y = (o.ySec - 92) * 192 + (y) / 10;
                    o.oX = o.x;
                    o.oY = o.y;
                    o.State = 1;
                    o.Move = 1;
                    o.LocalType = 2;
                    o.AutoSpawn = true;
                    o.State = 2;
                    o.HP = Data.ObjectBase[ID].HP;
                    o.Kat = 1;
                    o.Agro = new List<_agro>();
                    o.spawnOran = 20;
                    o.OrgMovement = o.AutoMovement;

                    while ((input = Npcangle.ReadLine()) != null)
                    {
                        npcangle1 = input.Split(',');
                        if (ID == int.Parse(npcangle1[0]) && AREA == int.Parse(npcangle1[2]))
                        {
                            o.rotation = Int32.Parse(npcangle1[1]);
                            break;
                        }
                    }
                    Npcangle.Close();

                    if (Data.ObjectBase[ID].ObjectType != 3)
                    {
                        o.Type = Systems.RandomType(Data.ObjectBase[ID].Level, ref o.Kat, false, ref o.Agresif);
                        o.HP *= o.Kat;
                        if (o.Type == 1) o.Agresif = 1;
                        Systems.Objects.Add(o);
                    }
                    else
                    {
                        o.AutoSpawn = false;
                        o.Type = Data.ObjectBase[ID].ObjectType;
                        GlobalUnique.AddObject(o);
                    }
                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " npc's");
        }

        /////////////////////////////////////////////////////////////////////////////
        // Teleports
        /////////////////////////////////////////////////////////////////////////////
        public static void TeleportBuilding()
        {
            TxtFile.ReadFromFile(@"\data\teleportbuilding.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');

                if (!(short.Parse(TxtFile.commands[41]) == 0))
                {
                    obj o = new obj();
                    int ID = Convert.ToInt32(TxtFile.commands[1]);
                    short AREA = short.Parse(TxtFile.commands[41]);
                    double x = Convert.ToDouble(TxtFile.commands[43]);
                    double z = Convert.ToDouble(TxtFile.commands[44]);
                    double y = Convert.ToDouble(TxtFile.commands[45]);
                    o.Ids = new Global.ID(Global.ID.IDS.Object);
                    o.UniqueID = o.Ids.GetUniqueID;
                    objectdata os = new objectdata();
                    os.Name = TxtFile.commands[2];
                    Data.ObjectBase[ID] = os;
                    o.ID = ID;
                    o.area = AREA;
                    o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                    o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                    o.x = (o.xSec - 135) * 192 + (x) / 10;
                    o.z = z;
                    o.y = (o.ySec - 92) * 192 + (y) / 10;
                    o.HP = 0x000000C0;
                    o.LocalType = 3;
                    Data.ObjectBase[o.ID].Object_type = Global.objectdata.NamdedType.TELEPORT;
                    Systems.Objects.Add(o);
                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " teleport buildings");
            TeleportData();
        }

        /////////////////////////////////////////////////////////////////////////////
        // cave Teleport data
        /////////////////////////////////////////////////////////////////////////////
        public static void cavedata()// this is added as the location where you end up in cave from telepad also formula was added for coords
        {
            TxtFile.ReadFromFile(@"\data\cavespawns.txt", '\t');

            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');

                cavepoint o = new cavepoint();
                int Number = Convert.ToInt32(TxtFile.commands[1]);
                int ID = Convert.ToInt32(TxtFile.commands[3]);
                short AREA = short.Parse(TxtFile.commands[5]);
                double x = Convert.ToDouble(TxtFile.commands[6]);
                double z = Convert.ToDouble(TxtFile.commands[7]);
                double y = Convert.ToDouble(TxtFile.commands[8]);

                o.test = Convert.ToByte(TxtFile.commands[12]);
                o.Name = TxtFile.commands[2];
                o.ID = ID;
                o.Number = Number;
                o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                if (!(o.xSec < 8))
                {
                    o.x = (o.xSec - 135) * 192 + (x) / 10;
                    o.z = z;
                    o.y = (o.ySec - 92) * 192 + (y) / 10;
                }
                else
                {
                    o.x = Formule.cavegamex((float)x);
                    o.z = z;
                    o.y = Formule.cavegamey((float)y);

                }
                Data.cavePointBase[Number] = o;
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Internal cave spawns");
        }
        
        /////////////////////////////////////////////////////////////////////////////
        // Teleport data
        /////////////////////////////////////////////////////////////////////////////
        public static void TeleportData()
        {
            TxtFile.ReadFromFile(@"\data\teleportdata.txt", '\t');

            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');

                point o = new point();
                int Number = Convert.ToInt32(TxtFile.commands[1]);
                int ID = Convert.ToInt32(TxtFile.commands[3]);
                short AREA = short.Parse(TxtFile.commands[5]);
                double x = Convert.ToDouble(TxtFile.commands[6]);
                double z = Convert.ToDouble(TxtFile.commands[7]);
                double y = Convert.ToDouble(TxtFile.commands[8]);

                o.test = Convert.ToByte(TxtFile.commands[12]);
                o.Name = TxtFile.commands[2];
                o.ID = ID;
                o.Number = Number;
                o.xSec = Convert.ToByte((AREA).ToString("X4").Substring(2, 2), 16);
                o.ySec = Convert.ToByte((AREA).ToString("X4").Substring(0, 2), 16);
                if (!(o.xSec < 8))
                {
                    o.x = (o.xSec - 135) * 192 + (x) / 10;
                    o.z = z;
                    o.y = (o.ySec - 92) * 192 + (y) / 10;
                }
                else
                {
                    o.x = Formule.cavegamex((float)x);//formula was added for coords
                    o.z = z;
                    o.y = Formule.cavegamey((float)y);

                }
                Data.PointBase[Number] = o;
            }
        }
        /////////////////////////////////////////////////////////////////////////////
        // Get teleports
        /////////////////////////////////////////////////////////////////////////////
        public static byte GetTeleport(string name)
        {
            try
            {
                byte rNum = Convert.ToByte(name);
                return rNum;
            }
            catch
            {
                for (byte b = 0; b <= Data.PointBase.Length - 1; b++)
                {
                    if (Data.PointBase[b] != null && Data.PointBase[b].Name == name) return b;
                }
                return 1;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        // Mastery data
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadMasteryData()
        {
            TxtFile.ReadFromFile(@"\data\mastery.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int sp = Convert.ToInt16(TxtFile.commands[1]);
                byte level = Convert.ToByte(TxtFile.commands[0]);

                Data.MasteryBase[level] = sp;
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " mastery levels");
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Gold data
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadGoldData()
        {
            TxtFile.ReadFromFile(@"\data\levelgold.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                levelgold lg = new levelgold();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                byte level = Convert.ToByte(TxtFile.commands[0]);
                lg.min = Convert.ToInt16(TxtFile.commands[1]);
                lg.max = Convert.ToInt16(TxtFile.commands[2]);

                Data.LevelGold[level] = lg;
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " gold stats");
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Job Level Data
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadJobLevels()
        {
            TxtFile.ReadFromFile(@"\data\tradeconflict_joblevel.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                JobLevel levelinfo = new JobLevel();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                levelinfo.level = Convert.ToByte(TxtFile.commands[0]);
                levelinfo.exp = Convert.ToInt64(TxtFile.commands[1]);
                levelinfo.jobtitle = Convert.ToByte(TxtFile.commands[2]);

                Data.Joblevelinfo.Add(levelinfo);
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Job levels");
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Level info
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadLevelData()
        {
            TxtFile.ReadFromFile(@"\data\leveldata.txt", '\t');

            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                byte level = Convert.ToByte(TxtFile.commands[0]);
                long exp = Convert.ToInt64(TxtFile.commands[1]);

                Data.LevelData[level] = exp;
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " player levels");
        }
        /////////////////////////////////////////////////////////////////////////////////
        // magic options
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadMagicOptions()
        {
            TxtFile.ReadFromFile(@"\data\magicoption.txt", '\t');
            int count = TxtFile.amountLine;
            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                MagicOption m = new MagicOption();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                m.ID = Convert.ToInt32(TxtFile.commands[1]);
                m.Name = TxtFile.commands[2];
                m.Type = TxtFile.commands[3];
                m.Level = Convert.ToInt32(TxtFile.commands[4]);
                m.OptionPercent = Convert.ToDouble(TxtFile.commands[5].Replace('.', ','));


                    if (ConvertBlueValue(Convert.ToInt32(TxtFile.commands[9])) != 0)
                    {
                        m.MinValue = ConvertBlueValue(Convert.ToInt32(TxtFile.commands[9]));
                    }
                    else
                    {
                        m.MinValue = Convert.ToInt32(TxtFile.commands[9]);
                    }
                    if (Convert.ToInt32(TxtFile.commands[10]) != 0)
                    {
                        if (ConvertBlueValue(Convert.ToInt32(TxtFile.commands[10])) != 0)
                        {
                            m.MaxValue = ConvertBlueValue(Convert.ToInt32(TxtFile.commands[10]));
                        }
                        else
                        {
                            m.MaxValue = Convert.ToInt32(TxtFile.commands[10]);
                        }
                    }
                    else
                    {
                        if (ConvertBlueValue(Convert.ToInt32(TxtFile.commands[9])) != 0)
                        {
                            m.MinValue = ConvertBlueValue(Convert.ToInt32(TxtFile.commands[8]));
                            m.MaxValue = ConvertBlueValue(Convert.ToInt32(TxtFile.commands[9]));
                        }

                    }
                
                m.ApplicableOn[0] = TxtFile.commands[31];
                if (TxtFile.commands[33] != "xxx") m.ApplicableOn[1] = TxtFile.commands[33];
                if (TxtFile.commands[35] != "xxx") m.ApplicableOn[2] = TxtFile.commands[35];
                if (TxtFile.commands[37] != "xxx") m.ApplicableOn[3] = TxtFile.commands[37];

                Data.MagicOptions.Add(m);
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " blue options");
        }
        public static int ConvertBlueValue(int num)
        {
            int number = num >>= 16;
            return number;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Shop tabs
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadShopTabData()
        {
            TxtFile.ReadFromFile(@"\data\shopdata.txt", '\t');
            string s = null;
            int count = TxtFile.amountLine;
            byte[] co = new byte[28000];
            Shop_Alexandria();
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int ID = Convert.ToInt32(TxtFile.commands[5]);

                if (ID > 0)
                    if (Data.ObjectBase[ID] != null)
                    {
                        string name = TxtFile.commands[2];
                        Data.ObjectBase[ID].StoreName = name;
                    }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Npc shop data");

            TxtFile.ReadFromFile(@"\data\refmappingshopwithtab.txt", '\t');
            count += TxtFile.amountLine;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                string name = TxtFile.commands[2];
                int ID = GetNpcID(name);
                if (Data.ObjectBase[ID] != null)
                {
                    Data.ObjectBase[ID].Shop[co[ID]] = TxtFile.commands[3];
                    co[ID]++;
                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Shop tabs");
            co = null;
            co = new byte[25000];

            TxtFile.ReadFromFile(@"\data\refshoptab.txt", '\t');
            count += TxtFile.amountLine;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                string name = TxtFile.commands[4];
                int ID = GetNpcID_(name);
                if (Data.ObjectBase[ID] != null)
                {
                    shop_data sh = new shop_data();
                    sh.tab = TxtFile.commands[3];
                    Data.ShopData.Add(sh);
                    Data.ObjectBase[ID].Tab[co[ID]] = TxtFile.commands[3];
                    co[ID]++;
                }
            }
            co = null;

            TxtFile.ReadFromFile(@"\data\refshopgoods.txt", '\t');
            count += TxtFile.amountLine;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                string name = TxtFile.commands[2];
                shop_data ID = shop_data.GetShopIndex(name);
                if (ID != null)
                {
                    TxtFile.commands[3] = TxtFile.commands[3].Remove(0, 8);
                    ID.Item[Convert.ToInt16(TxtFile.commands[4])] = TxtFile.commands[3];

                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Shop items");
            SetShopData();
        }

        /////////////////////////////////////////////////////////////////////////////////
        // Get npc by string
        /////////////////////////////////////////////////////////////////////////////////
        public static int GetNpcID(string name)
        {
            for (int i = 0; i < Data.ObjectBase.Length; i++)
            {
                if (Data.ObjectBase[i] != null && Data.ObjectBase[i].StoreName == name)
                    return (int)Data.ObjectBase[i].ID;
            }
            return 0;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get npc by string
        /////////////////////////////////////////////////////////////////////////////////
        public static int GetNpcID_(string name)
        {
            for (int i = 0; i <= Data.ObjectBase.Length - 1; i++)
            {
                if (Data.ObjectBase[i] != null)
                {
                    for (int b = 0; b <= 10 - 1; b++)
                        if (Data.ObjectBase[i].Shop[b] == name)
                            return (int)Data.ObjectBase[i].ID;
                }
            }
            return 0;
        }
        public static void SetShopData()
        {
            #region Jangang
            Data.ObjectBase[1915].Tab[0] = "STORE_CH_POTION_TAB1";

            Data.ObjectBase[2008].Tab[0] = "STORE_CH_ACCESSORY_TAB1";
            Data.ObjectBase[2008].Tab[1] = "STORE_CH_ACCESSORY_TAB2";
            Data.ObjectBase[2008].Tab[2] = "STORE_CH_ACCESSORY_TAB3";

            Data.ObjectBase[2010].Tab[0] = "STORE_CH_SPECIAL_TAB1";


            #endregion

            #region Donwhang

            #endregion

            #region Hotan
            Data.ObjectBase[2021].Tab[0] = "STORE_CH_GUILD_TAB1";
            Data.ObjectBase[2021].Tab[1] = "STORE_CH_GUILD_TAB3";

            Data.ObjectBase[2072].Tab[0] = "STORE_KT_SMITH_EU_TAB1";
            Data.ObjectBase[2072].Tab[1] = "STORE_KT_SMITH_EU_TAB2";
            Data.ObjectBase[2072].Tab[2] = "STORE_KT_SMITH_EU_TAB3";
            Data.ObjectBase[2072].Tab[3] = "STORE_KT_SMITH_TAB1";
            Data.ObjectBase[2072].Tab[4] = "STORE_KT_SMITH_TAB2";
            Data.ObjectBase[2072].Tab[5] = "STORE_KT_SMITH_TAB3";


            Data.ObjectBase[2073].Tab[0] = "STORE_KT_ARMOR_EU_TAB1";
            Data.ObjectBase[2073].Tab[1] = "STORE_KT_ARMOR_EU_TAB2";
            Data.ObjectBase[2073].Tab[2] = "STORE_KT_ARMOR_EU_TAB3";
            Data.ObjectBase[2073].Tab[3] = "STORE_KT_ARMOR_EU_TAB4";
            Data.ObjectBase[2073].Tab[4] = "STORE_KT_ARMOR_EU_TAB5";
            Data.ObjectBase[2073].Tab[5] = "STORE_KT_ARMOR_EU_TAB6";
            Data.ObjectBase[2073].Tab[6] = "STORE_KT_ARMOR_TAB1";
            Data.ObjectBase[2073].Tab[7] = "STORE_KT_ARMOR_TAB2";
            Data.ObjectBase[2073].Tab[8] = "STORE_KT_ARMOR_TAB3";
            Data.ObjectBase[2073].Tab[9] = "STORE_KT_ARMOR_TAB4";
            Data.ObjectBase[2073].Tab[10] = "STORE_KT_ARMOR_TAB5";
            Data.ObjectBase[2073].Tab[11] = "STORE_KT_ARMOR_TAB6";

            Data.ObjectBase[2075].Tab[0] = "STORE_KT_ACCESSORY_EU_TAB1";
            Data.ObjectBase[2075].Tab[1] = "STORE_KT_ACCESSORY_EU_TAB2";
            Data.ObjectBase[2075].Tab[2] = "STORE_KT_ACCESSORY_EU_TAB3";
            Data.ObjectBase[2075].Tab[3] = "STORE_KT_ACCESSORY_TAB1";
            Data.ObjectBase[2075].Tab[4] = "STORE_KT_ACCESSORY_TAB2";
            Data.ObjectBase[2075].Tab[5] = "STORE_KT_ACCESSORY_TAB3";

            Data.ObjectBase[9274].Tab[0] = "STORE_KT_GUILD_TAB1";
            Data.ObjectBase[9274].Tab[1] = "STORE_KT_GUILD_TAB3";
            #endregion

            #region Europe sm
            Data.ObjectBase[7531].Tab[0] = "STORE_CA_ARMOR_TAB1";
            Data.ObjectBase[7531].Tab[1] = "STORE_CA_ARMOR_TAB2";
            Data.ObjectBase[7531].Tab[2] = "STORE_CA_ARMOR_TAB3";
            Data.ObjectBase[7531].Tab[3] = "STORE_CA_ARMOR_TAB4";
            Data.ObjectBase[7531].Tab[4] = "STORE_CA_ARMOR_TAB5";
            Data.ObjectBase[7531].Tab[5] = "STORE_CA_ARMOR_TAB6";

            Data.ObjectBase[7530].Tab[0] = "STORE_CA_SMITH_TAB1";
            Data.ObjectBase[7530].Tab[1] = "STORE_CA_SMITH_TAB2";
            Data.ObjectBase[7530].Tab[2] = "STORE_CA_SMITH_TAB3";

            Data.ObjectBase[7534].Tab[0] = "STORE_CA_STABLE_TAB1";
            Data.ObjectBase[7534].Tab[1] = "STORE_CA_STABLE_TAB2";
            Data.ObjectBase[7534].Tab[2] = "STORE_CA_STABLE_TAB3";
            Data.ObjectBase[7534].Tab[3] = "STORE_CA_STABLE_TAB4";

            Data.ObjectBase[7536].Tab[0] = "STORE_CA_TRADER_TAB1";
            Data.ObjectBase[7536].Tab[1] = "STORE_CA_TRADER_TAB2";

            Data.ObjectBase[7533].Tab[0] = "STORE_CA_ACCESSORY_TAB1";
            Data.ObjectBase[7533].Tab[1] = "STORE_CA_ACCESSORY_TAB2";
            Data.ObjectBase[7533].Tab[2] = "STORE_CA_ACCESSORY_TAB3";

            Data.ObjectBase[7532].Tab[0] = "STORE_CA_POTION_TAB1";

            #endregion
        }
        public static void Shop_Alexandria()
        {
            #region Jangang
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CH_ACCESSORY")].StoreName = "STORE_CA_ACCESSORY";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CH_ACCESSORY")].Tab[0] = "STORE_CH_ACCESSORY_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CH_ACCESSORY")].Tab[1] = "STORE_CH_ACCESSORY_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CH_ACCESSORY")].Tab[2] = "STORE_CH_ACCESSORY_TAB3";

            #endregion

            #region Eu sm
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_ARMOR")].StoreName = "STORE_CA_ARMOR";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_ARMOR")].Tab[0] = "STORE_CA_ARMOR_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_ARMOR")].Tab[1] = "STORE_CA_ARMOR_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_ARMOR")].Tab[2] = "STORE_CA_ARMOR_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_ARMOR")].Tab[3] = "STORE_CA_ARMOR_TAB4";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_ARMOR")].Tab[4] = "STORE_CA_ARMOR_TAB5";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_ARMOR")].Tab[5] = "STORE_CA_ARMOR_TAB6";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_SMITH")].StoreName = "STORE_CA_SMITH";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_SMITH")].Tab[0] = "STORE_CA_SMITH_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_SMITH")].Tab[1] = "STORE_CA_SMITH_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_SMITH")].Tab[2] = "STORE_CA_SMITH_TAB3";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_HORSE")].StoreName = "STORE_CA_STABLE";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_HORSE")].Tab[0] = "STORE_CA_STABLE_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_HORSE")].Tab[1] = "STORE_CA_STABLE_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_HORSE")].Tab[2] = "STORE_CA_STABLE_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_HORSE")].Tab[3] = "STORE_CA_STABLE_TAB4";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_MERCHANT")].StoreName = "STORE_CA_TRADER";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_MERCHANT")].Tab[0] = "STORE_CA_TRADER_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_MERCHANT")].Tab[1] = "STORE_CA_TRADER_TAB2";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_ACCESSORY")].StoreName = "STORE_CA_ACCESSORY";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_ACCESSORY")].Tab[0] = "STORE_CA_ACCESSORY_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_ACCESSORY")].Tab[1] = "STORE_CA_ACCESSORY_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_ACCESSORY")].Tab[2] = "STORE_CA_ACCESSORY_TAB3";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_POTION")].StoreName = "STORE_CA_POTION";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_POTION")].Tab[0] = "STORE_CA_POTION_TAB1";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_HUNTER")].StoreName = "STORE_CA_HUNTER";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_HUNTER")].Tab[0] = "STORE_CA_HUNTER_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_CA_HUNTER")].Tab[0] = "STORE_CA_HUNTER_TAB2";
            #endregion

            #region Constantinople
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_ARMOR")].StoreName = "STORE_EU_ARMOR";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_ARMOR")].Tab[0] = "STORE_EU_ARMOR_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_ARMOR")].Tab[1] = "STORE_EU_ARMOR_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_ARMOR")].Tab[2] = "STORE_EU_ARMOR_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_ARMOR")].Tab[3] = "STORE_EU_ARMOR_TAB4";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_ARMOR")].Tab[4] = "STORE_EU_ARMOR_TAB5";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_ARMOR")].Tab[5] = "STORE_EU_ARMOR_TAB6";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_SMITH")].StoreName = "STORE_EU_SMITH";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_SMITH")].Tab[0] = "STORE_EU_SMITH_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_SMITH")].Tab[1] = "STORE_EU_SMITH_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_SMITH")].Tab[2] = "STORE_EU_SMITH_TAB3";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_HORSE")].StoreName = "STORE_EU_STABLE";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_HORSE")].Tab[0] = "STORE_EU_STABLE_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_HORSE")].Tab[1] = "STORE_EU_STABLE_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_HORSE")].Tab[2] = "STORE_EU_STABLE_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_HORSE")].Tab[3] = "STORE_EU_STABLE_TAB4";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_MERCHANT")].StoreName = "STORE_EU_TRADER";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_MERCHANT")].Tab[0] = "STORE_EU_TRADER_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_MERCHANT")].Tab[1] = "STORE_EU_TRADER_TAB2";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_ACCESSORY")].StoreName = "STORE_EU_ACCESSORY";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_ACCESSORY")].Tab[0] = "STORE_EU_ACCESSORY_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_ACCESSORY")].Tab[1] = "STORE_EU_ACCESSORY_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_ACCESSORY")].Tab[2] = "STORE_EU_ACCESSORY_TAB3";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_POTION")].StoreName = "STORE_EU_POTION";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_POTION")].Tab[0] = "STORE_EU_POTION_TAB1";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_HUNTER")].StoreName = "STORE_EU_HUNTER";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_HUNTER")].Tab[0] = "STORE_EU_HUNTER_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_EU_HUNTER")].Tab[0] = "STORE_EU_HUNTER_TAB2";
            #endregion
            #region GUILD
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_GUILD")].StoreName = "STORE_KT_GUILD";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_GUILD")].Tab[0] = "STORE_CH_GUILD_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_GUILD")].Tab[1] = "STORE_CH_GUILD_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_WC_GUILD_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_WC_GUILD_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_KT_GUILD_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_KT_GUILD_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_EU_GUILD_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_EU_GUILD_TAB3";

            #endregion
            #region SMITH
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].StoreName = "STORE_KT_SMITH";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[0] = "STORE_KT_SMITH_EU_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[1] = "STORE_KT_SMITH_EU_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[2] = "STORE_KT_SMITH_EU_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[3] = "STORE_KT_SMITH_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[4] = "STORE_KT_SMITH_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_SMITH")].Tab[5] = "STORE_KT_SMITH_TAB3";


            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].StoreName = "STORE_KT_SMITH";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[0] = "STORE_KT_SMITH_EU_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[1] = "STORE_KT_SMITH_EU_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[2] = "STORE_KT_SMITH_EU_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[3] = "STORE_KT_SMITH_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[4] = "STORE_KT_SMITH_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_SMITH")].Tab[5] = "STORE_KT_SMITH_TAB3";
            #endregion

            #region ACCESSORY
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].StoreName = "STORE_KT_ACCESSORY";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[0] = "STORE_KT_ACCESSORY_EU_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[1] = "STORE_KT_ACCESSORY_EU_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[2] = "STORE_KT_ACCESSORY_EU_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[3] = "STORE_KT_ACCESSORY_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[4] = "STORE_KT_ACCESSORY_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_ACCESSORY")].Tab[5] = "STORE_KT_ACCESSORY_TAB3";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].StoreName = "STORE_KT_ACCESSORY";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[0] = "STORE_KT_ACCESSORY_EU_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[1] = "STORE_KT_ACCESSORY_EU_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[2] = "STORE_KT_ACCESSORY_EU_TAB3";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[3] = "STORE_KT_ACCESSORY_TAB1";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[4] = "STORE_KT_ACCESSORY_TAB2";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_ACCESSORY")].Tab[5] = "STORE_KT_ACCESSORY_TAB3"; //STORE_KT_POTION STORE_KT_POTION_TAB1
            #endregion

            #region POTION
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_POTION")].StoreName = "STORE_KT_POTION";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_M_AREA_POTION")].Tab[0] = "STORE_KT_POTION_TAB1";

            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_POTION")].StoreName = "STORE_KT_POTION";
            Data.ObjectBase[Global.objectdata.GetItem("NPC_SD_T_AREA_POTION")].Tab[0] = "STORE_KT_POTION_TAB1";
            #endregion

        }
        // Region Informations:
        public static void LoadRegionCodes()
        {
            TxtFile.ReadFromFile(@"\data\regioncode.txt", '\t');
            int count = TxtFile.amountLine;
            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                region r = new region();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                r.ID = Convert.ToInt32(TxtFile.commands[1]);
                r.Name = TxtFile.commands[2];
                if (r.Name == "xxx") r.Name = "";
                //r.SecX = Convert.ToInt32(TxtFile.commands[5]);
                //r.SecY = Convert.ToInt32(TxtFile.commands[6]);
                Data.RegionBase.Add(r);
            }

            //get safe zones
            foreach (region r in Data.RegionBase)
            {
                switch (r.ID)
                {
                    //the edges of cities:
                    case 25257:
                    case 24999:
                    case 25001:
                    case 26521:
                    case 26263:
                    case 26265:
                    case 23686:
                    case 23689:
                    case 23175:
                    case 27244:
                    case 27245:
                    case 27501:
                    case 27500:
                    case 27499:
                    case 26957:
                    case 27471:
                    //the area of cities:
                    case 26958:
                    case 26702:
                    case 26446:
                    case 26704:
                    case 26448:
                    case 26960:
                    case 27216:
                    case 27472:
                    case 27217:
                    case 27473:
                    case 27985:
                    case 27729:
                    case 27243:
                    case 23687:
                    case 23431:
                    case 23943:
                    case 24199:
                    case 23688:
                    case 23432:
                    case 26519:
                    case 25000:
                    case 25256:
                    case 25255:
                    case 24743:
                        Data.safeZone.Add(r);
                        break;

                }
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Zones");
        }
        public static void ReverseData()
        {
            TxtFile.ReadFromFile(@"\data\refoptionalteleport.txt", '\t');
            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');

                int ID = Convert.ToInt32(TxtFile.commands[1]);
                short area = short.Parse(TxtFile.commands[4]);
                int x = Convert.ToInt32(TxtFile.commands[5]);
                int z = Convert.ToInt32(TxtFile.commands[6]);
                int y = Convert.ToInt32(TxtFile.commands[7]);
                reverse o = new reverse();
                o.ID = ID;
                o.xSec = Convert.ToByte((area).ToString("X4").Substring(2, 2), 16);
                o.ySec = Convert.ToByte((area).ToString("X4").Substring(0, 2), 16);
                o.x = (float)(o.xSec - 135) * 192 + (x) / 10;
                o.z = (float)z;
                o.y = (float)((o.ySec - 92) * 192 + (y) / 10);

                Data.ReverseData[ID] = o;

            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Reverse locations");
        }
        public static void LoadTeleportPrice()
        {
            TxtFile.ReadFromFile(@"\data\teleportlink.txt", '\t');
            int count = TxtFile.amountLine;
            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                TeleportPrice t = new TeleportPrice();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int id = Convert.ToInt32(TxtFile.commands[2]);
                t.ID = id;
                t.price = Convert.ToInt32(TxtFile.commands[3]);
                t.level = Convert.ToInt32(TxtFile.commands[7]);
                Data.TeleportPrice.Add(t);
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Teleport locations");
        }
        

        public static void LoadCaves()
        {
            TxtFile.ReadFromFile(@"\data\caveteleportdata.txt", '\t');// made some changes here but still need to add some data to the text files
            string s = null;
            int count = TxtFile.amountLine;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                region r = new region();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                r.ID = l;
                short area = short.Parse(TxtFile.commands[5]);
                r.Name = TxtFile.commands[2];
                r.SecX = Convert.ToByte((area).ToString("X4").Substring(2, 2), 16);
                r.SecY = Convert.ToByte((area).ToString("X4").Substring(0, 2), 16);
                Data.Cave.Add(r);
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Cave teleport data");
        }
        
        
        
        public static bool CheckCave(byte xsec, byte ysec)
        {
            bool s = Data.Cave.Exists(delegate(Global.region r)
            {
                if (r.SecX == xsec && r.SecY == ysec)
                {
                    if (!(r.Name.ToUpper() == "GATE_DUNGEON_DH_OUT" || r.Name.ToUpper() == "GATE_JINSI_OUT"))// added this so when out of cave the server know we are back to basic movement not cave
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    }
                return false;
            });
            return s;
        }
        public static void LoadCaveTeleports()
        {
            TxtFile.ReadFromFile(@"\data\caveteleportok1.txt", '\t');// changed file name as this is still work in progress
            int count = TxtFile.amountLine;
            string s = null;
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                CaveTeleports c = new CaveTeleports();
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                c.name = TxtFile.commands[0];
                c.xsec = Convert.ToByte(TxtFile.commands[1]);
                c.ysec = Convert.ToByte(TxtFile.commands[2]);
                string[] x1x = new string[2];
                string[] z1z = new string[2];
                string[] y1y = new string[2];
                x1x = TxtFile.commands[3].Split(',');// the Convert ToDouble from a file read will sometimes put 10567,00234 as 105670.3 that is not good for us so i split the , out of the data to get true coord
                c.x = Convert.ToDouble(x1x[0]);
                z1z = TxtFile.commands[4].Split(',');
                c.z = Convert.ToDouble(z1z[0]);
                y1y = TxtFile.commands[5].Split(',');
                c.y = Convert.ToDouble(y1y[0]);
                Data.CaveTeleports.Add(c);
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Cave teleports");
        }

        /*public static void ConvertTeleports()
        {
            TxtFile.ReadFromFile(@"\data\cavecircle.txt", '\t');
            int count = TxtFile.amountLine;
            string s = null;
            using (StreamWriter w = new StreamWriter(@"caveteleportoookk.txt"))
            {
                for (int l = 0; l <= TxtFile.amountLine - 1; l++)
                {
                    s = TxtFile.lines[l].ToString();
                    TxtFile.commands = s.Split(' ');
                    string name;
                    string e;
                    string e1;
                    string e2;
                    string e3;
                    string e4;
                    string e5;

                    int xsec;
                    int ysec;
                    List<string> lista = new List<string>();
                    e = TxtFile.commands[0];
                    e1 = TxtFile.commands[1];
                    xsec = Convert.ToInt32(TxtFile.commands[2]);
                    ysec = Convert.ToInt32(TxtFile.commands[3]);
                    e2 = TxtFile.commands[4];
                    e3 = TxtFile.commands[5];
                    e4 = TxtFile.commands[6];
                    e5 = TxtFile.commands[7];
                    name = TxtFile.commands[8];
                    lista.Add(e);
                    lista.Add(e1);
                    lista.Add(e2);
                    lista.Add(e3);
                    lista.Add(e4);
                    lista.Add(e5);
                    List<float> fl = new List<float>();
                    for (int i = 0; i <= lista.Count() - 1; i++)
                    {

                        uint num = uint.Parse(lista[i], System.Globalization.NumberStyles.AllowHexSpecifier);

                        byte[] floatVals = BitConverter.GetBytes(num);
                        float f = BitConverter.ToSingle(floatVals, 0);
                        fl.Add(f);
                    }

                    w.WriteLine(name + '\t' + xsec + '\t' + ysec + '\t' + fl[2] + '\t' + fl[3] + '\t' + fl[4]);
                }
            }

        }*/

        /////////////////////////////////////////////////////////////////////////////////
        // Quest Data
        /////////////////////////////////////////////////////////////////////////////////
        public static void LoadQuestData(string file)
        {
            TxtFile.ReadFromFile(file, '\t');
            string line = null;
            quest_database Quest = new quest_database();

            for (int i = 0; i <= TxtFile.amountLine - 1; i++)
            {
                line = TxtFile.lines[i].ToString();
                TxtFile.commands = line.Split('\t');
                Quest.Questid = Convert.ToInt32(TxtFile.commands[1]);
                Quest.QuestNPC = Convert.ToString(TxtFile.commands[2]);
                Quest.QuestLevel = Convert.ToInt32(TxtFile.commands[3]);
                Data.QuestData.Add(Quest);
            }
            TxtFile.ReadFromFile(@"\data\questcontentsdata.txt", '\t');
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                line = TxtFile.lines[l].ToString();
                TxtFile.commands = line.Split('\t');
                Quest.Questname = TxtFile.commands[5];
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Quests");
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Trader Data (Max amount scales).
        /////////////////////////////////////////////////////////////////////////////////
        public static void TraderData()
        {
            //Trade scales 1 / 5 stars
            TxtFile.ReadFromFile(@"data\maxtradescaledata.txt", '\t');
            string line = null;
            for (int i = 0; i <= TxtFile.amountLine - 1; i++)
            {
                line = TxtFile.lines[i].ToString();
                TxtFile.commands = line.Split('\t');
                trader_data Data = new trader_data();
                Data.Amount = Convert.ToInt32(TxtFile.commands[0]);
                /* ################
                 * Definitions
                 * Extra check on trader amounts to ensure no exploiting
                 * Must add Max trade amount to 5 stars
                 * ################
                if (Data.Amount > 0     && Data.Amount < 510)       Data.Details  = trader_data.stars.ONESTAR;
                if (Data.Amount > 510   && Data.Amount < 918)       Data.Details  = trader_data.stars.TWOSTARS;
                if (Data.Amount > 918   && Data.Amount < 1428)      Data.Details  = trader_data.stars.THREESTARS;
                if (Data.Amount > 1428  && Data.Amount < 2142)      Data.Details  = trader_data.stars.FOURSTARS;
                if (Data.Amount > 2142  && Data.Amount < 2856)      Data.Details  = trader_data.stars.FIVESTARS;
                 */
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Agressive monsters
        /////////////////////////////////////////////////////////////////////////////////
        public static void SetAgroMobs(string listfile)
        {
            TxtFile.ReadFromFile(listfile, '\t');
            int count = TxtFile.amountLine;
            string s = null;

            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');

                List<obj> filter = Systems.Objects.FindAll(o => o.ID == Convert.ToInt32(TxtFile.commands[0]));
                foreach (obj o in filter)
                {
                    o.oldAgresif = Convert.ToByte(TxtFile.commands[1]);
                    if (o.Type == 0 || o.Type == 16) o.Agresif = Convert.ToByte(TxtFile.commands[1]);
                }
            }
        }

        public static void LoadMapObject(string filename)
        {
            try
            {
                string path = Directory.GetCurrentDirectory() + filename;

                /* Get file content to memory */
                FileStream fs = System.IO.File.OpenRead(path);
                MemoryStream ms = new MemoryStream();

                ms.SetLength(fs.Length);
                fs.Read(ms.GetBuffer(), 0, (int)fs.Length);

                ms.Flush();
                fs.Close();

                BinaryReader br = new BinaryReader(ms);

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    SectorObject obj = new SectorObject();

                    /* Read each sector's data */

                    byte xSector = br.ReadByte();
                    byte ySector = br.ReadByte();
                    short region = DarkEmu_GameServer.Formule.makeRegion(xSector, ySector);

                    for (int i = 0; i < 9409; ++i)
                        obj.heightmap[i] = br.ReadSingle();

                    obj.entityCount = br.ReadInt32();

                    for (int i = 0; i < obj.entityCount; ++i)
                    {

                        SectorObject.n7nEntity entity = new SectorObject.n7nEntity();
                        entity.Points = new List<SectorObject.n7nEntity.sPoint>();
                        entity.OutLines = new List<SectorObject.n7nEntity.sLine>();

                        entity.Position.x = br.ReadSingle();
                        entity.Position.z = br.ReadSingle();
                        entity.Position.y = br.ReadSingle();

                        entity.ObjectMapflag = br.ReadByte();

                        if (entity.ObjectMapflag == 1)
                        {
                            int PointCount = br.ReadInt32();
                            for (int j = 0; j < PointCount; ++j)
                            {
                                SectorObject.n7nEntity.sPoint p = new SectorObject.n7nEntity.sPoint();

                                p.x = br.ReadSingle();
                                p.z = br.ReadSingle();
                                p.y = br.ReadSingle();

                                entity.Points.Add(p);
                            }

                            int LineCount = br.ReadInt32();

                            for (int j = 0; j < LineCount; ++j)
                            {
                                SectorObject.n7nEntity.sLine l = new SectorObject.n7nEntity.sLine();

                                l.PointA = br.ReadInt16();
                                l.PointB = br.ReadInt16();
                                l.flag = br.ReadByte();

                                entity.OutLines.Add(l);
                            }
                        }
                        obj.entitys.Add(entity);
                    }

                    Data.MapObject.Add(region, obj);
                }
                Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Map objects");
            }
            catch(Exception)
            {
                // ToDo: when going live with map, enabled error logging
                //Systems.Debugger.Write(ex);
            }
        }
    }
}



