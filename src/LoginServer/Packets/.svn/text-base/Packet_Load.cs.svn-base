///////////////////////////////////////////////////////////////////////////
// PROJECT SRX
// HOMEPAGE: WWW.XCODING.NET
///////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace LoginServer
{
    public partial class Systems
    {
        public static byte[] LoadGame_1()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_MAIN);
            Writer.Word(0x0101);
            Writer.Word(0x0500);
            Writer.Byte(0x20);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_2()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_MAIN);
            Writer.Word(0x0100);
            Writer.Word(0x0100);
            Writer.Byte(0x69);
            Writer.Byte(0x0C);
            Writer.DWord(0x00000005);
            Writer.Byte(0x02);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_3()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_MAIN);
            Writer.Word(0x0101);
            Writer.Word(0x0500);
            Writer.Byte(0x60);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_4()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_MAIN);
            Writer.Word(0x0300);
            Writer.Word(0x0200);
            Writer.Word(0x0200);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_5()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_MAIN);
            Writer.Word(0x0101);
            Writer.Word(0);
            Writer.Byte(0xA1);
            return Writer.GetBytes();
        }
        public static byte[] LoadGame_6()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_MAIN);
            Writer.Byte(1);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Byte(4);
            Writer.Byte(0xA1);

            return Writer.GetBytes();
        }
        public static byte[] Clientok()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_MAIN);
            Writer.Word(0x0100);
            return Writer.GetBytes();
        }
    }
}
