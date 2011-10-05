///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Packet
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // On Gold Pickup
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] GoldMessagePick(int gold)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);    //Select opcode
            Writer.Byte(1);                             //Static byte
            Writer.Byte(6);                             //Static byte
            Writer.Byte(0xFE);                          //Message type
            Writer.DWord(gold);                         //Gold amount
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Item Move Error // Refactor needed, since we will only use the below one. And send message code!
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] MoveItemError()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);    //Select opcode
            Writer.Byte(0x02);                          //Type
            Writer.Word(0x1807);                        //Message info type
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Item Move Error Packet || This is what we will use
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] MoveItemError(byte action, byte type)
        {
        	PacketWriter Writer = new PacketWriter();
        	Writer.Create(Systems.SERVER_ITEM_MOVE);
        	Writer.Byte(2);
        	Writer.Byte(action);
        	Writer.Byte(type);
        	return Writer.GetBytes();
        }
		public static byte[] SafeState_SkillUse_Fail()
		{
			PacketWriter Writer = new PacketWriter();
			Writer.Create(0xB034);
			Writer.Byte(2);
            Writer.Word(SrxRevo.IngameMessages.UIIT_STT_SKILLUSE_FAIL_CANT_BATTLE_STATE);
			return Writer.GetBytes();
		}
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Chat Packet
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] ChatPacket(byte type, int id, string text, string name)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHAT);
            Writer.Byte(type);
            switch (type)
            {
                case 1:
                    Writer.DWord(id);
                    Writer.Text3(text);
                    break;
                case 3:
                    Writer.DWord(id);
                    Writer.Text3(text);
                    break;
                case 2:
                    Writer.Text(name);
                    Writer.Text3(text);
                    break;
                case 4:
                    Writer.Text(name);
                    Writer.Text3(text);
                    break;
                case 5:
                    Writer.Text(name);
                    Writer.Text3(text);
                    break;
                case 6:
                    Writer.Text(name);
                    Writer.Text3(text);
                    break;
                case 7:
                    Writer.Text3(text);
                    break;
                case 9:
                    Writer.Text(name);
                    Writer.Text3(text);
                    break;
                case 11:
                    Writer.Text(name);
                    Writer.Text3(text);
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] guide(byte info1, byte info2, byte info3)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SEND_GUIDE);
            Writer.Word(info1);
            Writer.DWord(info2);
            Writer.Word(info3);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] ChatIndexPacket(byte type, byte index)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CHAT_INDEX);
            Writer.Bool(true);
            Writer.Byte(type);
            Writer.Byte(index);
            return Writer.GetBytes();
        }
    }
}
