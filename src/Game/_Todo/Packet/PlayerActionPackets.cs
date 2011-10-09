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
        public static byte[] ChangeStatus(int id, byte type, byte stand)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHANGE_STATUS);
            if (type == 2)
            {
                Writer.DWord(id);
                Writer.Byte(1);
                Writer.Byte(2);
            }
            else if (type == 3)
            {
                Writer.DWord(id);
                Writer.Byte(1);
                Writer.Byte(3);
            }
            else if (type == 4)
            {
                Writer.DWord(id);
                Writer.Byte(1);
                Writer.Byte(stand);
            }
            else if (type == 5)
            {
                Writer.DWord(id);
                Writer.Byte(4);
                Writer.Byte(0);
            }
            else if (type == 6)
            {
                Writer.Byte(stand);
                Writer.Byte(0x40);
                Writer.Byte(3);
            }
            return Writer.GetBytes();
        }
        
        public static byte[] QuestionMark(int ID, byte info)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_QUESTMARK);
            Writer.DWord(ID);
            Writer.Byte(info);

            return Writer.GetBytes();
        }
        public static byte[] Transform(int Modelid, int ID)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_TRANSFORM);
            Writer.DWord(ID);
            Writer.DWord(Modelid);
            return Writer.GetBytes();
        }
        public static byte[] PvpSystemWait(int userid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PVP_WAIT);
            Writer.DWord(userid);
            Writer.Byte(2);
            Writer.Byte(1);
            Writer.Byte(0x0A);
            return Writer.GetBytes();
        }
        public static byte[] PvpInterupt(int userid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PVP_INTERUPT);
            Writer.DWord(userid);
            Writer.Byte(2);
            return Writer.GetBytes();
        }
        public static byte[] PvpSystemData(int userid, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PVP_DATA);
            Writer.Byte(1);
            Writer.DWord(userid);
            Writer.Byte(type);
            return Writer.GetBytes();
        }
        public static byte[] PkPlayer()
        {   // TODO: Get server opcode / data
            PacketWriter Writer = new PacketWriter();
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Pickup Animation
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] Pickup_Animation(int id, byte infobyte)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PICKUPITEM_ANIM);  //Select opcode
            Writer.DWord(id);                               //Character ID
            Writer.Byte(infobyte);                          //Byte (0). Unless different seems static.
            return Writer.GetBytes();
        }
        public static byte[] StatePack(int id, byte type1, byte type2, bool type3)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHANGE_STATUS);
            Writer.DWord(id);
            Writer.Byte(type1);
            Writer.Byte(type2);
            if (type1 == 4 && type2 == 1) Writer.Bool(type3);
            return Writer.GetBytes();
        }
        public static byte[] UpdateStr()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_UPDATE_STR);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
        public static byte[] UpdateInt()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_UPDATE_INT);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
        public static byte[] UpdatePlayer(int objectid, ushort packetcode, byte type, int prob)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SKILL_EFFECTS);
            Writer.DWord(objectid);
            Writer.Word(packetcode);
            Writer.Byte(type);
            Writer.DWord(prob);
            return Writer.GetBytes();
        }
        public static byte[] Testeffect(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SKILL_EFFECTS);
            Writer.DWord(id);
            Writer.Word(1);
            Writer.Byte(5);
            Writer.DWord(0);
            Writer.DWord(0);
            return Writer.GetBytes();

        }
        public static byte[] ActionState(byte b1, byte b2)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ACTIONSTATE);
            Writer.Byte(b1);
            Writer.Byte(b2);
            return Writer.GetBytes();
        }
        public static byte[] MasteryUpPacket(int mastery, byte level)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_MASTERY_UP);
            Writer.Bool(true);
            Writer.DWord(mastery);
            Writer.Byte(level);
            return Writer.GetBytes();
        }
        public static byte[] InfoUpdate(byte type, int obje, byte bT)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_INFO_UPDATE);
            Writer.Byte(type);
            switch (type)
            {
                case 1:
                    Writer.LWord(obje);
                    Writer.Byte(0);
                    break;
                case 2:
                    Writer.DWord(obje);
                    Writer.Byte(0);
                    break;
                case 4:
                    Writer.Byte(bT);
                    Writer.DWord(obje);
                    break;
                default:
                    break;
            }

            return Writer.GetBytes();
        }
        public static byte[] SkillUpdate(int skillid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SKILL_UPDATE);
            Writer.Byte(1);
            Writer.DWord(skillid);
            return Writer.GetBytes();
        }
        public static byte[] Player_Emote(int id, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EMOTE);
            Writer.DWord(id);
            if (type == 1) Writer.Byte(1);
            else Writer.Byte(type);
            return Writer.GetBytes();
        }
        public static byte[] StorageBox()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_STORAGE_BOX);
            Writer.Word(1);
            Writer.Word(1);
            return Writer.GetBytes();
        }
        public static byte[] StorageBoxLog()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_BOX_LOG);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] RepairItems(byte slot, double durability)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_REPAIR);
            Writer.Byte(slot);
            Writer.DWord(Convert.ToInt32(durability));
            return Writer.GetBytes();
        }
        public static byte[] SendWebMall(int Myid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MALL_WEB);
            Writer.Byte(1);
            Writer.DWord(Myid);//Account ID for silk check
            Writer.Text("http://www.xfsgames.com.ar");//url need to check 
            return Writer.GetBytes();
        }
        public static byte[] SelectObject(int id, int model, byte type, int hp)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SELECT_OBJECT);
            Writer.Bool(true);
            Writer.DWord(id);
            switch (type)
            {
                case 1:
                    Writer.Byte(1);
                    Writer.DWord(hp);
                    Writer.Byte(1);
                    Writer.Byte(5);
                    break;
                case 2:
                    Systems.NPC.Chat(model, Writer);
                    break;
                case 3:
                    Systems.NPC.Chat(model, Writer);
                    break;
                case 4:
                    Writer.Byte(1);
                    Writer.DWord(hp);
                    Writer.Byte(1);
                    Writer.Byte(5);
                    break;
                case 5:
                    Writer.Byte(1);
                    Writer.Byte(5);
                    //Writer.Byte(4);
                    break;
                default:
                    Console.WriteLine("Non Coded Select Type: " + type + "");
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] Player_LevelUpEffect(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_LEVELUP_EFFECT);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        public static byte[] Player_getExp(int id, long exp, long sp, short level)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_GET_EXP);
            Writer.DWord(id);
            Writer.LWord(exp);
            Writer.LWord(sp);
            Writer.Byte(0);
            if (level != 0) Writer.Word(level);
            return Writer.GetBytes();
        }
        public static byte[] Player_HandleEffect(int id, int itemid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_HANDLE_EFFECT);
            Writer.DWord(id);
            Writer.DWord(itemid);
            return Writer.GetBytes();
        }
        public static byte[] Player_HandleUpdateSlot(byte slot, ushort amount, int packet)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_HANDLE_UPDATE_SLOT);
            Writer.Byte(1);
            Writer.Byte(slot);
            Writer.Word(amount);
            Writer.DWord(packet);
            return Writer.GetBytes();
        }
        public static byte[] Player_HandleUpdateSlotu(byte slot, short givenitems)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_HANDLE_UPDATE_SLOT);
            Writer.Byte(slot);
            Writer.Byte(8);
            Writer.Word(givenitems);
            return Writer.GetBytes();
        }
        public static byte[] SetSpeed(int id, float speed1, float speed2)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SETSPEED);
            Writer.DWord(id);
            Writer.Float(speed1);
            Writer.Float(speed2);
            return Writer.GetBytes();
        }
    }
}
