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

namespace SrxRevo
{
    public partial class Packet
    {
        public static byte[] TeleportStart2()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_TELEPORTSTART);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] TeleportStart()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_TELEPORTSTART);
            Writer.Byte(2);
            Writer.Word(1);
            return Writer.GetBytes();
        }
        public static byte[] IngameMessages(ushort opcode,ushort id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(opcode);
            Writer.Byte(2);
            Writer.Word(id);
            return Writer.GetBytes();
        }
        public static byte[] ErrorArmorType(int itemid)
        {
            //Dunno what this is for yet.
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_EQUIP_CHECK);
            Writer.DWord(itemid);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] TeleportOtherStart()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_TELEPORTOTHERSTART);
            Writer.DWord(0);
            return Writer.GetBytes();
        }
        public static byte[] TeleportImage(byte xsec, byte ysec)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_TELEPORTIMAGE);
            Writer.Byte(xsec);
            Writer.Byte(ysec);
            return Writer.GetBytes();
        }
        public static byte[] UpdatePlace()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SAVE_PLACE);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
    }
}
