/*    <DarkEmu GameServer>
    Copyright (C) <2011>  <DarkEmu>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
namespace DarkEmu_GameServer
{
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
    unsafe public struct TPacket
    {
        public ushort size;
        public ushort opcode;
        public byte securityCount;
        public byte securityCRC;
        public fixed byte data[4000];
    }
    class Silkroad
    {
        public static void SendHandshake(int ClientIndex)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_HANDSHAKE);
            writer.AppendByte(1);
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);
        }

        unsafe public static TPacket* ToTPacket(byte[] src)
        {
            fixed (byte* tmp = src)
                return (TPacket*)tmp;
        }


        public static void DumpObjects()
        {
            Console.Write("Loading static objects from data\\...");
            if (File.Exists("data\\characterdata.txt"))
            {
                string[] lines = File.ReadAllLines("data\\characterdata.txt");
                for (byte i = 0; i < lines.Length; i++)
                    DumpFilesToObjects(string.Format("data\\{0}", lines[i]));
            }
            else
                Console.WriteLine("Couldnt find data\\characterdata.txt");            
            if (File.Exists("data\\itemdata.txt"))
            {
                string[] lines = File.ReadAllLines("data\\itemdata.txt");
                for (byte i = 0; i < lines.Length; i++)
                    DumpFilesToItems(string.Format("data\\{0}", lines[i]));
            }
            else
                Console.WriteLine("Couldnt find data\\characterdata.txt");
            if (File.Exists("data\\itemdata.txt"))
            {
                string[] lines = File.ReadAllLines("data\\skilldata.txt");
                for (byte i = 0; i < lines.Length; i++)
                    DumpFilesToTmpSkill(string.Format("data\\{0}", lines[i]));
                for (byte i = 0; i < lines.Length; i++)
                    DumpFilesToSkills(string.Format("data\\{0}", lines[i]));
                tmpSkills.Clear();
            }
            else
                Console.WriteLine("Couldnt find data\\skilldata.txt");
            if (File.Exists("data\\leveldata.txt"))            
                DumpFilesToLevel("data\\leveldata.txt");
            else
                Console.WriteLine("Couldnt find data\\leveldata.txt");
            if (File.Exists("data\\levelgold.txt"))
                DumpFilesToGold("data\\levelgold.txt");
            else
                Console.WriteLine("Couldnt find data\\levelgold.txt");

            Console.WriteLine("finished!");
        }

        public struct Object_
        {
            public uint Id;
            public string Name;
            public string OtherName;
            public byte Type;
            public byte Type1;
            public float Speed;
            public byte Level;
            public uint Hp;
            public byte InvSize;
            public ushort PhyDef;
            public ushort MagDef;
            public byte HitRatio;
            public byte ParryRatio;
            public ulong Exp;
            public uint Skill1;
            public uint Skill2;
            public uint Skill3;
            public uint Skill4;
            public uint Skill5;
            public uint Skill6;
            public uint Skill7;
            public uint Skill8;
            public uint Skill9;
        }
        public static List<Object_> Objects = new List<Object_>();

        private static void DumpFilesToObjects(string path)
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] tmpString = lines[i].Split('\t');
                Object_ tmp = new Object_();
                tmp.Id = Convert.ToUInt32(tmpString[1]);
                tmp.Name = tmpString[2];
                tmp.OtherName = tmpString[3];
                tmp.Type = 1;
                tmp.Type1 = 1;
                tmp.Speed = Convert.ToSingle(tmpString[50]);
                tmp.Level = Convert.ToByte(tmpString[57]);
                tmp.Hp = Convert.ToUInt32(tmpString[59]);
                tmp.InvSize = 0;
                tmp.PhyDef = Convert.ToUInt16(tmpString[71]);
                tmp.MagDef = Convert.ToUInt16(tmpString[72]);
                tmp.HitRatio = Convert.ToByte(tmpString[75]);
                tmp.ParryRatio = Convert.ToByte(tmpString[77]);
                tmp.Exp = Convert.ToUInt64(tmpString[79]);
                tmp.Skill1 = Convert.ToUInt32(tmpString[83]);
                tmp.Skill2 = Convert.ToUInt32(tmpString[85]);
                tmp.Skill3 = Convert.ToUInt32(tmpString[86]);
                tmp.Skill4 = Convert.ToUInt32(tmpString[87]);
                tmp.Skill5 = Convert.ToUInt32(tmpString[88]);
                tmp.Skill6 = Convert.ToUInt32(tmpString[89]);
                tmp.Skill7 = Convert.ToUInt32(tmpString[90]);
                tmp.Skill8 = Convert.ToUInt32(tmpString[91]);
                tmp.Skill9 = Convert.ToUInt32(tmpString[92]);
                Objects.Add(tmp);
            }
        }

        public static Object_ GetObjectById(uint ItemId)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i].Id == ItemId)
                    return Objects[i];
            }
            return new Object_();
        }

        public struct Item_
        {
            public uint ITEM_TYPE;
            public string ITEM_TYPE_NAME;
            public byte ITEM_MALL;
            public byte CLASS_A;
            public byte CLASS_B;
            public byte CLASS_C;
            public byte RACE;
            public ulong SHOP_PRICE;
            public ushort MIN_REPAIR;
            public ushort MAX_REPAIR;
            public ulong STORE_PRICE;
            public ulong SELL_PRICE;
            public byte LV_REQ;
            public int REQ1;
            public byte REQ1_LV;
            public int REQ2;
            public byte REQ2_LV;
            public int REQ3;
            public byte REQ3_LV;
            public int MAX_POSSES;
            public ushort MAX_STACK;
            public byte GENDER;
            public float MIN_DURA;
            public float MAX_DURA;
            public double MIN_PHYSDEF;
            public double MAX_PHYSDEF;
            public double PHYSDEF_INC;
            public float MIN_PARRY;
            public float MAX_PARRY;
            public double MIN_ABSORB;
            public double MAX_ABSORB;
            public double ABSORB_INC;
            public float MIN_BLOCK;
            public float MAX_BLOCK;
            public double MAGDEF_MIN;
            public double MAGDEF_MAX;
            public double MAGDEF_INC;
            public float MIN_APHYS_REINFORCE;
            public float MAX_APHYS_REINFORCE;
            public float MIN_AMAG_REINFORCE;
            public float MAX_AMAG_REINFORCE;
            public float ATTACK_DISTANCE;
            public double MIN_LPHYATK;
            public double MAX_LPHYATK;
            public double MIN_HPHYATK;
            public double MAX_HPHYATK;
            public double PHYATK_INC;
            public double MIN_LMAGATK;
            public double MAX_LMAGATK;
            public double MIN_HMAGATK;
            public double MAX_HMAGATK;
            public double MAGATK_INC;
            public float MIN_LPHYS_REINFORCE;
            public float MAX_LPHYS_REINFORCE;
            public float MIN_HPHYS_REINFORCE;
            public float MAX_HPHYS_REINFORCE;
            public float MIN_LMAG_REINFORCE;
            public float MAX_LMAG_REINFORCE;
            public float MIN_HMAG_REINFORCE;
            public float MAX_HMAG_REINFORCE;
            public float MIN_ATTACK_RATING;
            public float MAX_ATTACK_RATING;
            public float MIN_CRITICAL;
            public float MAX_CRITICAL;
            public int USE_TIME;    // steht drin wieviel HP ein potion heilt           //*******************************
            public int USE_TIME2;   // steht drin wieviel prozent HP ein grain heilt    //* Das hier muss wahrscheinlich
            public int USE_TIME3;   // steht drin wieviel MP ein potion heilt           //* umbenannt werden, USE_TIME
            public int USE_TIME4;   // steht drin wieviel prozent MP ein grain heilt    //* passt nicht ganz.
        }                                                                               //*******************************
        public static List<Item_> Items = new List<Item_>();

        private static void DumpFilesToItems(string path)
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace(".", ",");
                string[] tmpString = lines[i].Split('\t');
                Item_ tmp = new Item_();
                tmp.ITEM_TYPE = Convert.ToUInt32(tmpString[1]);
                tmp.ITEM_TYPE_NAME = tmpString[2];
                tmp.ITEM_MALL = Convert.ToByte(tmpString[7]);
                tmp.CLASS_A = Convert.ToByte(tmpString[10]);
                tmp.CLASS_B = Convert.ToByte(tmpString[11]);
                tmp.CLASS_C = Convert.ToByte(tmpString[12]);
                tmp.RACE = Convert.ToByte(tmpString[14]);
                tmp.SHOP_PRICE = Convert.ToUInt64(tmpString[26]);
                tmp.MIN_REPAIR = Convert.ToUInt16(tmpString[27]);
                tmp.MAX_REPAIR = Convert.ToUInt16(tmpString[28]);
                tmp.STORE_PRICE = Convert.ToUInt64(tmpString[30]);
                tmp.SELL_PRICE = Convert.ToUInt64(tmpString[31]);
                tmp.LV_REQ = Convert.ToByte(tmpString[33]);
                tmp.REQ1 = Convert.ToInt32(tmpString[34]);
                tmp.REQ1_LV = Convert.ToByte(tmpString[35]);
                tmp.REQ2 = Convert.ToInt32(tmpString[36]);
                tmp.REQ2_LV = Convert.ToByte(tmpString[37]);
                tmp.REQ3 = Convert.ToInt32(tmpString[38]);
                tmp.REQ3_LV = Convert.ToByte(tmpString[39]);
                tmp.MAX_POSSES = Convert.ToInt32(tmpString[40]);
                tmp.MAX_STACK = Convert.ToUInt16(tmpString[57]);
                tmp.GENDER = Convert.ToByte(tmpString[58]);
                tmp.MIN_DURA = Convert.ToSingle(tmpString[63]);
                tmp.MAX_DURA = Convert.ToSingle(tmpString[64]);
                tmp.MIN_PHYSDEF = Convert.ToDouble(tmpString[65]);
                tmp.MAX_PHYSDEF = Convert.ToDouble(tmpString[66]);
                tmp.PHYSDEF_INC = Convert.ToDouble(tmpString[67]);
                tmp.MIN_PARRY = Convert.ToSingle(tmpString[68]);
                tmp.MAX_PARRY = Convert.ToSingle(tmpString[69]);
                tmp.MIN_ABSORB = Convert.ToDouble(tmpString[70]);
                tmp.MAX_ABSORB = Convert.ToDouble(tmpString[71]);
                tmp.ABSORB_INC = Convert.ToDouble(tmpString[72]);
                tmp.MIN_BLOCK = Convert.ToSingle(tmpString[73]);
                tmp.MAX_BLOCK = Convert.ToSingle(tmpString[74]);
                tmp.MAGDEF_MIN = Convert.ToDouble(tmpString[75]);
                tmp.MAGDEF_MAX = Convert.ToDouble(tmpString[76]);
                tmp.MAGDEF_INC = Convert.ToDouble(tmpString[77]);
                tmp.MIN_APHYS_REINFORCE = Convert.ToSingle(tmpString[78]);
                tmp.MAX_APHYS_REINFORCE = Convert.ToSingle(tmpString[79]);
                tmp.MIN_AMAG_REINFORCE = Convert.ToSingle(tmpString[80]);
                tmp.MAX_AMAG_REINFORCE = Convert.ToSingle(tmpString[81]);
                tmp.ATTACK_DISTANCE = Convert.ToSingle(tmpString[94]);
                tmp.MIN_LPHYATK = Convert.ToDouble(tmpString[95]);
                tmp.MAX_LPHYATK = Convert.ToDouble(tmpString[96]);
                tmp.MIN_HPHYATK = Convert.ToDouble(tmpString[97]);
                tmp.MAX_HPHYATK = Convert.ToDouble(tmpString[99]);
                tmp.PHYATK_INC = Convert.ToDouble(tmpString[100]);
                tmp.MIN_LMAGATK = Convert.ToDouble(tmpString[101]);
                tmp.MAX_LMAGATK = Convert.ToDouble(tmpString[102]);
                tmp.MIN_HMAGATK = Convert.ToDouble(tmpString[103]);
                tmp.MAX_HMAGATK = Convert.ToDouble(tmpString[104]);
                tmp.MAGATK_INC = Convert.ToDouble(tmpString[105]);
                tmp.MIN_LPHYS_REINFORCE = Convert.ToSingle(tmpString[106]);
                tmp.MAX_LPHYS_REINFORCE = Convert.ToSingle(tmpString[107]);
                tmp.MIN_HPHYS_REINFORCE = Convert.ToSingle(tmpString[108]);
                tmp.MAX_HPHYS_REINFORCE = Convert.ToSingle(tmpString[109]);
                tmp.MIN_LMAG_REINFORCE = Convert.ToSingle(tmpString[110]);
                tmp.MAX_LMAG_REINFORCE = Convert.ToSingle(tmpString[111]);
                tmp.MIN_HMAG_REINFORCE = Convert.ToSingle(tmpString[112]);
                tmp.MAX_HMAG_REINFORCE = Convert.ToSingle(tmpString[113]);
                tmp.MIN_ATTACK_RATING = Convert.ToSingle(tmpString[114]);
                tmp.MAX_ATTACK_RATING = Convert.ToSingle(tmpString[115]);
                tmp.MIN_CRITICAL = Convert.ToSingle(tmpString[116]);
                tmp.MAX_CRITICAL = Convert.ToSingle(tmpString[117]);
                tmp.USE_TIME = Convert.ToInt32(tmpString[118]);
                tmp.USE_TIME2 = Convert.ToInt32(tmpString[120]);
                tmp.USE_TIME3 = Convert.ToInt32(tmpString[122]);
                tmp.USE_TIME4 = Convert.ToInt32(tmpString[124]);
                Items.Add(tmp);
            }
        }

        public static Item_ GetItemById(uint ItemId)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].ITEM_TYPE == ItemId)
                    return Items[i];
            }
            return new Item_();
        }

        public struct Skill_
        {
            public string Name;
            public uint Id;
            public uint NextId;
            public ulong RequiredSp;
            public ushort RequiredMp;
            public byte CastTime;
            public int PwrPercent;
            public int PwrMin;
            public int PwrMax;
            public int Distance;
            public byte NumberOfAttacks;
            public byte Type;
            public byte Type2;
        }
        public struct tmpSkill_
        {
            public uint Id;
            public uint NextId;
        }

        public static List<Skill_> Skills = new List<Skill_>();
        public static List<tmpSkill_> tmpSkills = new List<tmpSkill_>();

        private static void DumpFilesToTmpSkill(string path)
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] tmpString = lines[i].Split('\t');
                tmpSkill_ tmp = new tmpSkill_();
                tmp.Id = Convert.ToUInt32(tmpString[1]);
                tmp.NextId = Convert.ToUInt32(tmpString[9]);
                tmpSkills.Add(tmp);
            }
        }
        private static void DumpFilesToSkills(string path)
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] tmpString = lines[i].Split('\t');
                Skill_ tmp = new Skill_();   
                tmp.Id = Convert.ToUInt32(tmpString[1]);
                tmp.Name = tmpString[3];
                tmp.NextId = Convert.ToUInt32(tmpString[9]);
                tmp.RequiredSp = Convert.ToUInt64(tmpString[46]);
                tmp.RequiredMp = Convert.ToUInt16(tmpString[53]);
                tmp.CastTime = Convert.ToByte(tmpString[68]);
                tmp.PwrPercent = Convert.ToInt32(tmpString[71]);
                tmp.PwrMin = Convert.ToInt32(tmpString[72]);
                tmp.PwrMax = Convert.ToInt32(tmpString[73]);
                tmp.Distance = Convert.ToInt32(tmpString[78]);
                if (tmp.Distance == 0)
                    tmp.Distance = 21;

                tmp.NumberOfAttacks = GetNumberOfAttacks(GetTmpSkillById(tmp.Id));
                if (tmpString[3].Contains("SWORD"))
                {
                    tmp.Type = TypeTable.Phy;
                    tmp.Type2 = TypeTable.Bicheon;
                }
                if (tmpString[3].Contains("SPEAR"))
                {
                    tmp.Type = TypeTable.Phy;
                    tmp.Type2 = TypeTable.Heuksal;
                }
                if (tmpString[3].Contains("BOW"))
                {
                    tmp.Type = TypeTable.Phy;
                    tmp.Type2 = TypeTable.Bow;
                }
                if (tmpString[3].Contains("FIRE") || tmpString[3].Contains("LIGHTNING") || tmpString[3].Contains("COLD") || tmpString[3].Contains("WATER"))
                {
                    tmp.Type = TypeTable.Mag;
                    tmp.Type2 = TypeTable.All;
                }
                if (tmpString[3].Contains("PUNCH"))
                {
                    tmp.Type = TypeTable.Phy;
                    tmp.Type2 = TypeTable.All;
                }
                if (tmpString[3].Contains("ROG") || tmpString[3].Contains("WARRIOR"))
                {
                    tmp.Type = TypeTable.Phy;
                    tmp.Type2 = TypeTable.All;
                }

                if (tmpString[3].Contains("WIZARD") ||tmpString[3].Contains("STAFF")  || tmpString[3].Contains("WARLOCK") || tmpString[3].Contains("BARD")|| tmpString[3].Contains("HARP") || tmpString[3].Contains("CLERIC"))
                {
                    tmp.Type = TypeTable.Mag;
                    tmp.Type2 = TypeTable.All;
                }
                Skills.Add(tmp);
            }
        }

        public class TypeTable
        {
            public const byte
                Phy = 0x1,
                Mag = 0x2,
                Bicheon = 0x3,
                Heuksal = 0x4,
                Bow = 0x5,
                All = 0x6;
        }

        private static byte GetNumberOfAttacks(tmpSkill_ tmp)
        {
            for (byte i = 0; i < 10; i++)
            {
                if (tmp.NextId != 0)                
                    tmp = Silkroad.GetTmpSkillById(tmp.NextId);                
                else
                    return (byte)(i + 1);               
            }
            return 1;
        }

        public static Skill_ GetSkillById(uint ItemId)
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                if (Skills[i].Id == ItemId)
                    return Skills[i];
            }
            return new Skill_();
        }

        private static tmpSkill_ GetTmpSkillById(uint NextId)
        {
            for (int i = 0; i < tmpSkills.Count; i++)
            {
                if (tmpSkills[i].Id == NextId)
                    return tmpSkills[i];
            }
            return new tmpSkill_();
        }

        public struct Level_
        {
            public byte Level;
            public ulong Experience;
            public ulong Skillpoints;
        }
        public static List<Level_> LevelData = new List<Level_>();

        private static void DumpFilesToLevel(string path)
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] tmpString = lines[i].Split('\t');
                Level_ tmp = new Level_();
                tmp.Level = Convert.ToByte(tmpString[0]);
                tmp.Experience = Convert.ToUInt64(tmpString[1]);
                if (tmp.Level == 1)
                    tmp.Skillpoints = 0;
                else
                    tmp.Skillpoints = Convert.ToUInt64(tmpString[2]);
                LevelData.Add(tmp);
            }
        }

        public static Level_ GetLevelDataByLevel(byte Level)
        {
            for (int i = 0; i < LevelData.Count; i++)
            {
                if (LevelData[i].Level == Level)
                    return LevelData[i];
            }
            return new Level_();
        }

        public struct Gold_
        {
            public byte Level;
            public ushort Skillpoints;
            public ushort Gold;
        }
        public static List<Gold_> GoldData = new List<Gold_>();

        private static void DumpFilesToGold(string path)
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] tmpString = lines[i].Split('\t');
                Gold_ tmp = new Gold_();
                tmp.Level = Convert.ToByte(tmpString[0]);
                tmp.Skillpoints = Convert.ToUInt16(tmpString[1]);
                tmp.Gold = Convert.ToUInt16(tmpString[2]);
                GoldData.Add(tmp);
            }
        }

        public static Gold_ GetGoldDataByLevel(byte Level)
        {
            for (int i = 0; i < LevelData.Count; i++)
            {
                if (GoldData[i].Level == Level)
                    return GoldData[i];
            }
            return new Gold_();
        }

        public struct NpcData_
        {
            public uint Id;
            public byte XSector;
            public byte YSector;
            public float X;
            public float Z;
            public float Y;            
        }
        public static List<NpcData_> NpcDatas = new List<NpcData_>();

        private static void DumpFilesToNpcData(string path)
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] tmpString = lines[i].Split('\t');
                NpcData_ tmp = new NpcData_();
                tmp.Id = Convert.ToUInt32(tmpString[0]);
                tmp.XSector = byte.Parse(Convert.ToInt16(tmpString[1]).ToString("X").Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                tmp.YSector = byte.Parse(Convert.ToInt16(tmpString[1]).ToString("X").Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                tmp.X = Convert.ToSingle(tmpString[2]);
                tmp.Z = Convert.ToSingle(tmpString[3]);
                tmp.Y = Convert.ToSingle(tmpString[4]);
                NpcDatas.Add(tmp);
            }
        }

        public class C_S
        {
            [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
            public struct CREATE_ITEM
            {
                public uint ItemId;
                public uint ItemPlus;
            }

            [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
            public struct MOVE_ITEM
            {
                public byte Type;
                public byte Source;
                public byte Destination;
                public ushort Amount;
            }

            [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
            public struct MOVE_ITEM_GROUND
            {
                public byte Type;
                public byte Source;
            }

            [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
            public struct MOVEMENT_GROUND
            {
                public byte Type;
                public byte XSector;
                public byte YSector;
                public ushort X;
                public ushort Z;
                public ushort Y;
            }

            [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
            public struct MOVEMENT_SKY
            {
                public byte Type;
                public byte Flag;
                public ushort Angle;
            }
        }
    }
}
