using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.IO;
using System.Data;

namespace DarkEmu_GameServer
{
    class DatabaseCore
    {
        private static Timer PulseTimer;
        private static bool PulseFlag;
        public static Character_ Character;
        public static Mastery_ Mastery;
        public static Item_ Item;
        public static Skill_ Skill;
        public static User_ User;

        public static void SetPulseTime(int iTime)
        {
            PulseTimer = new Timer();
            PulseTimer.Interval = iTime;
        }

        public static void SetPulseFlag(bool Flag)
        {
            PulseFlag = Flag;
        }

        public static void Start()
        {
            ExecuteSavedQueries();
            PulseTimer.Elapsed += new ElapsedEventHandler(Pulse);
            Character = new Character_();
            Mastery = new Mastery_();
            Item = new Item_();
            Skill = new Skill_();
            User = new User_();
            if (PulseFlag)
                PulseTimer.Start();
            Pulse(null, null);
        }

        private static void Pulse(object sender, EventArgs e)
        {
            //ExecuteQuery();
            Character.Pulse();
            Mastery.Pulse();
            Item.Pulse();
            Skill.Pulse();
            User.Pulse();
        }

        public class Character_
        {
            public int NumberOfCharacters = 0;

            public string[] CharacterName = new string[0];
            public uint[] CharacterId = new uint[0];
            public uint[] UniqueId = new uint[0];
            public uint[] HP = new uint[0];
            public uint[] MP = new uint[0];
            public int[] CHP = new int[0];
            public int[] CMP = new int[0];
            public uint[] Model = new uint[0];
            public byte[] Volume = new byte[0];
            public byte[] Level = new byte[0];
            public ulong[] Experience = new ulong[0];
            public ulong[] Gold = new ulong[0];
            public uint[] SkillPoints = new uint[0];
            public ushort[] SkillPointBar = new ushort[0];
            public ushort[] Attributes = new ushort[0];
            public byte[] BerserkBar = new byte[0];
            public byte[] Berserk = new byte[0];
            public float[] WalkSpeed = new float[0];
            public float[] RunSpeed = new float[0];
            public float[] BerserkSpeed = new float[0];
            public ushort[] MinPhy = new ushort[0];
            public ushort[] MaxPhy = new ushort[0];
            public ushort[] MinMag = new ushort[0];
            public ushort[] MaxMag = new ushort[0];
            public ushort[] PhyDef = new ushort[0];
            public ushort[] MagDef = new ushort[0];
            public ushort[] Hit = new ushort[0];
            public ushort[] Parry = new ushort[0];
            public ushort[] Strength = new ushort[0];
            public ushort[] Intelligence = new ushort[0];
            public byte[] GM = new byte[0];
            public sbyte[] PVP = new sbyte[0];
            public byte[] XSector = new byte[0];
            public byte[] YSector = new byte[0];
            public float[] X = new float[0];
            public float[] Z = new float[0];
            public float[] Y = new float[0];
            public byte[] MaxSlots = new byte[0];

            public void Pulse()
            {
                DataSet tmpDataSet = Database.GetDataSet("SELECT * FROM characters");

                NumberOfCharacters = tmpDataSet.Tables[0].Rows.Count;
                CharacterName = new string[NumberOfCharacters];
                CharacterId = new uint[NumberOfCharacters];
                UniqueId = new uint[NumberOfCharacters];
                HP = new uint[NumberOfCharacters];
                MP = new uint[NumberOfCharacters];
                CHP = new int[NumberOfCharacters];
                CMP = new int[NumberOfCharacters];
                Model = new uint[NumberOfCharacters];
                Volume = new byte[NumberOfCharacters];
                Level = new byte[NumberOfCharacters];
                Experience = new ulong[NumberOfCharacters];
                Gold = new ulong[NumberOfCharacters];
                SkillPoints = new uint[NumberOfCharacters];
                SkillPointBar = new ushort[NumberOfCharacters];
                Attributes = new ushort[NumberOfCharacters];
                BerserkBar = new byte[NumberOfCharacters];
                Berserk = new byte[NumberOfCharacters];
                WalkSpeed = new float[NumberOfCharacters];
                RunSpeed = new float[NumberOfCharacters];
                BerserkSpeed = new float[NumberOfCharacters];
                MinPhy = new ushort[NumberOfCharacters];
                MaxPhy = new ushort[NumberOfCharacters];
                MinMag = new ushort[NumberOfCharacters];
                MaxMag = new ushort[NumberOfCharacters];
                PhyDef = new ushort[NumberOfCharacters];
                MagDef = new ushort[NumberOfCharacters];
                Hit = new ushort[NumberOfCharacters];
                Parry = new ushort[NumberOfCharacters];
                Strength = new ushort[NumberOfCharacters];
                Intelligence = new ushort[NumberOfCharacters];
                GM = new byte[NumberOfCharacters];
                PVP = new sbyte[NumberOfCharacters];
                XSector = new byte[NumberOfCharacters];
                YSector = new byte[NumberOfCharacters];
                X = new float[NumberOfCharacters];
                Z = new float[NumberOfCharacters];
                Y = new float[NumberOfCharacters];
                MaxSlots = new byte[NumberOfCharacters];

                for (int i = 0; i < NumberOfCharacters; i++)
                {
                    CharacterId[i] = Convert.ToUInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[0]);
                    UniqueId[i] = CharacterId[i] + 300000;
                    CharacterName[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[2].ToString();
                    Model[i] = Convert.ToUInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[3]);
                    Volume[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[4]);
                    Level[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[5]);
                    Strength[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[6]);
                    Intelligence[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[7]);
                    Attributes[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[8]);
                    HP[i] = Convert.ToUInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[9]);
                    MP[i] = Convert.ToUInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[10]);
                    CHP[i] = Convert.ToInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[11]);
                    CMP[i] = Convert.ToInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[12]);
                    Gold[i] = Convert.ToUInt64(tmpDataSet.Tables[0].Rows[i].ItemArray[13]);
                    Experience[i] = Convert.ToUInt64(tmpDataSet.Tables[0].Rows[i].ItemArray[14]);
                    SkillPoints[i] = Convert.ToUInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[15]);
                    GM[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[16]);
                    XSector[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[17]);
                    YSector[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[18]);
                    X[i] = Convert.ToSingle(tmpDataSet.Tables[0].Rows[i].ItemArray[19]);
                    Z[i] = Convert.ToSingle(tmpDataSet.Tables[0].Rows[i].ItemArray[20]);
                    Y[i] = Convert.ToSingle(tmpDataSet.Tables[0].Rows[i].ItemArray[21]);
                    MinPhy[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[22]);
                    MaxPhy[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[23]);
                    MinMag[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[24]);
                    MaxMag[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[25]);
                    PhyDef[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[26]);
                    MagDef[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[27]);
                    Hit[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[28]);
                    Parry[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[29]);
                    WalkSpeed[i] = Convert.ToSingle(tmpDataSet.Tables[0].Rows[i].ItemArray[30]);
                    RunSpeed[i] = Convert.ToSingle(tmpDataSet.Tables[0].Rows[i].ItemArray[31]);
                    BerserkSpeed[i] = Convert.ToSingle(tmpDataSet.Tables[0].Rows[i].ItemArray[32]);
                    BerserkBar[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[33]);
                    SkillPointBar[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[34]);
                    Berserk[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[35]);
                    PVP[i] = (sbyte)Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[36]);
                    MaxSlots[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[37]);
                }
            }

            public int GetIndexByName(string Name)
            {
                for (int i = 0; i < NumberOfCharacters; i++)
                {
                    if (CharacterName[i] == Name)
                        return i;
                }
                return -1;
            }
        }

        public class Mastery_
        {
            public int NumberOfMasteries = 0;

            public string[] CharacterName = new string[0];
            public ushort[] MasteryId = new ushort[0];
            public byte[] MasteryLevel = new byte[0];

            public void Pulse()
            {
                DataSet tmpDataSet = Database.GetDataSet("SELECT * FROM mastery");

                NumberOfMasteries = tmpDataSet.Tables[0].Rows.Count;
                CharacterName = new string[NumberOfMasteries];
                MasteryId = new ushort[NumberOfMasteries];
                MasteryLevel = new byte[NumberOfMasteries];

                for (int i = 0; i < NumberOfMasteries; i++)
                {
                    CharacterName[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    MasteryId[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[2]);
                    MasteryLevel[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[3]);
                }
            }

            public int[] GetIndexByName(string Name, bool Euro)
            {
                for (int i = 0; i < NumberOfMasteries; i++)
                {
                    if (CharacterName[i] == Name)
                    {
                        if (Euro)
                            return new int[]{
                              i,
                              i+1,
                              i+2,
                              i+3,
                              i+4,
                              i+5
                          };
                        else
                            return new int[] {     
                              i,
                              i+1,
                              i+2,
                              i+3,
                              i+4,
                              i+5,
                              i+6   
                            };

                    }
                }
                return new int[0];
            }
        }

        public class Item_
        {
            public int NumberOfItems = 0;

            public uint[] ItemId = new uint[0];
            public string[] CharacterName = new string[0];
            public byte[] PlusValue = new byte[0];
            public byte[] Slot = new byte[0];
            public byte[] Type = new byte[0];
            public byte[] Quantity = new byte[0];
            public byte[] Durability = new byte[0];
            public byte[] BlueAmount = new byte[0];
            public Blue_[] Blue = new Blue_[0];

            public struct Blue_
            {
                public uint[] Blue;
                public byte[] BlueAmount;
            }

            public void Pulse()
            {
                DataSet tmpDataSet = Database.GetDataSet("SELECT * FROM items");

                NumberOfItems = tmpDataSet.Tables[0].Rows.Count;
                ItemId = new uint[NumberOfItems];
                CharacterName = new string[NumberOfItems];
                PlusValue = new byte[NumberOfItems];
                Slot = new byte[NumberOfItems];
                Type = new byte[NumberOfItems];
                Quantity = new byte[NumberOfItems];
                Durability = new byte[NumberOfItems];
                BlueAmount = new byte[NumberOfItems];
                Blue = new Blue_[NumberOfItems];

                for (int i = 0; i < NumberOfItems; i++)
                {
                    ItemId[i] = Convert.ToUInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[2]);
                    CharacterName[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[3].ToString();
                    PlusValue[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[4]);
                    Slot[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[5]);
                    Type[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[6]);
                    Quantity[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[7]);
                    Durability[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[8]);
                    BlueAmount[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[9]);

                    Blue[i].Blue = new uint[9];
                    Blue[i].BlueAmount = new byte[9];
                    for (byte j = 0; j < 18; j++)
                    {
                        Blue[i].Blue[j / 2] = Convert.ToUInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[j + 10]);
                        j++;
                        Blue[i].BlueAmount[j / 2] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[j + 10]);
                    }
                }
            }

            public int[] GetIndexByName(string Name)
            {
                for (int i = 0; i < NumberOfItems; i++)
                {
                    if (CharacterName[i] == Name)
                    {
                        int[] tmpInt = new int[46];
                        for (byte j = 0; j < 46; j++)
                            tmpInt[j] = i + j;
                        return tmpInt;
                    }
                }
                return new int[0];

            }
        }

        public class Skill_
        {
            public int NumberOfSkills = 0;
            public string[] CharacterName = new string[0];
            public int[] SkillAmount = new int[0];
            public Skills_[] Skills = new Skills_[0];

            public struct Skills_
            {
                public uint[] SkillId;
            }

            public void Pulse()
            {
                DataSet tmpDataSet = Database.GetDataSet("SELECT * FROM skills");

                NumberOfSkills = tmpDataSet.Tables[0].Rows.Count;
                CharacterName = new string[NumberOfSkills];
                SkillAmount = new int[NumberOfSkills];
                Skills = new Skills_[NumberOfSkills];

                for (int i = 0; i < NumberOfSkills; i++)
                {
                    CharacterName[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    SkillAmount[i] = Convert.ToInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[2]);
                    Skills[i].SkillId = new uint[501];
                    for (int j = 0; j < 500; j++)
                        Skills[i].SkillId[j] = Convert.ToUInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[j + 3]);
                }
            }

            public int GetIndexByName(string Name)
            {
                for (int i = 0; i < NumberOfSkills; i++)
                {
                    if (CharacterName[i] == Name)
                        return i;
                }
                return -1;
            }
        }

        public class User_
        {
            public int NumberOfUsers = 0;
            public uint[] UserId = new uint[0];
            public string[] User = new string[0];
            public string[] Password = new string[0];
            public byte[] CharacterCount = new byte[0];
            public Characters_[] Characters = new Characters_[0];

            public struct Characters_
            {
                public string[] CharacterName;
            }

            public void Pulse()
            {
                DataSet tmpDataSet = Database.GetDataSet("SELECT * FROM user");
                NumberOfUsers = (byte)tmpDataSet.Tables[0].Rows.Count;

                UserId = new uint[NumberOfUsers];
                User = new string[NumberOfUsers];
                Password = new string[NumberOfUsers];
                CharacterCount = new byte[NumberOfUsers];
                Characters = new Characters_[NumberOfUsers];

                for (int i = 0; i < NumberOfUsers; i++)
                {
                    UserId[i] = Convert.ToUInt32(tmpDataSet.Tables[0].Rows[i].ItemArray[0]);
                    User[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    Password[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[2].ToString();
                    CharacterCount[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[5]);
                    Characters[i].CharacterName = new string[CharacterCount[i]];
                    for (byte j = 0; j < CharacterCount[i]; j++)
                        Characters[i].CharacterName[j] = tmpDataSet.Tables[0].Rows[i].ItemArray[6 + j].ToString();
                }
                tmpDataSet.Clear();
            }

            public int GetIndexByName(string UserName)
            {
                for (int i = 0; i < NumberOfUsers; i++)
                {
                    if (User[i] == UserName)
                        return i;
                }
                return 0;
            }
        }


        private static StreamWriter QueryWriter;
        private static string Path;

        public static void SetQueryLocation(string path)
        {
            Path = path;
        }

        public static void WriteQuery(string Query)
        {
            if (QueryWriter != null)
            {
                QueryWriter.WriteLine(Query);
                QueryWriter.Flush();
            }
            else
                Console.WriteLine("Error:QueryWriter == NULL\t-Could not save the queries.");
        }

        public static void WriteQuery(string Query, params object[] args)
        {
            WriteQuery(string.Format(null, Query, args));
        }

        public static void ExecuteQuery()
        {
            QueryWriter.Close();
            string[] tmpString = File.ReadAllLines(Path);
            for (int i = 0; i < tmpString.Length; i++)
            {
                if (tmpString[i] != null)
                    Database.ExecuteQueryAsnyc(tmpString[i]);
            }
            QueryWriter = new StreamWriter(Path);
        }

        public static void ExecuteSavedQueries()
        {
            if (File.Exists(Path))
            {
                string[] tmpString = File.ReadAllLines(Path);
                Console.WriteLine("{0} queries to execute!", tmpString.Length);
                for (int i = 0; i < tmpString.Length; i++)
                {
                    if (tmpString[i] != null)
                        Database.ExecuteQueryAsnyc(tmpString[i]);
                }
            }

            Console.Clear();
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine(" <Silkroad Gameserver>  Copyright (C) <2011>  <DarkEmu>\nThis program comes with ABSOLUTELY NO WARRANTY; for details take a look at the \nGPL.txt.This is free software, and you are welcome to redistribute it\nunder certain conditions.For more information look at the GPL.txt or type ‘show c’ for details.");
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Loading essentiel data and connecting to database.\nThis could take some minutes!");
            Database.CheckTables(new string[] { "mastery", "user", "characters", "skills", "items" });
            Console.WriteLine("Executed the queries!");

            QueryWriter = new StreamWriter(Path);
        }
    }
}