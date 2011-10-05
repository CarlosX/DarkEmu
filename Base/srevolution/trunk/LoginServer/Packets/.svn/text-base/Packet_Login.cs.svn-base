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
        public byte[] WrongInformation()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_AUTH);
            Writer.Byte(3);
            Writer.Byte(0x0F);
            Writer.Byte(2);
            Writer.Text(string.Format("[SRX]: Incorrect password: {0}/3 times.", WrongPassword));
            Writer.Word(0);
            return Writer.GetBytes();
        }
        public static byte[] SomethingWentWrong()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_AUTH);
            Writer.Byte(3);
            Writer.Byte(0x0F);
            Writer.Byte(2);
            Writer.Text("[SRX]: Account not found");
            Writer.Word(0);
            return Writer.GetBytes();
        }
        public static byte[] BannedUser(string sReason)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_AUTH);

            Writer.Byte(3);
            Writer.Byte(0x0F);
            Writer.Byte(2);
            Writer.Text(string.Format("[SRX] Banned: {0}", sReason));
            Writer.Word(0);

            return Writer.GetBytes();
        }
        public static byte[] ConnectWrong(ushort type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_AUTH);
            Writer.Word(type);
            return Writer.GetBytes();
        }
        public static byte[] NoSuchUser()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0xA102);
            Writer.Byte(3);
            Writer.Byte(0x0F);
            Writer.Byte(2);
            Writer.Text("[SRX] Account not found");
            return Writer.GetBytes();
        }
        public static byte[] ServerIsFull()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_AUTH);
            Writer.Byte(0x02);
            Writer.Byte(0x04);
            return Writer.GetBytes();
        }
        public static byte[] AllreadyConnected()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_AUTH);
            Writer.Byte(0x02);
            Writer.Byte(0x03);
            return Writer.GetBytes();
        }
        public static byte[] ConnectSucces(string ip, short port, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_AUTH);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.Byte(0);
            Writer.Word(0);
            Writer.Text(ip);
            Writer.Word(port);
            Writer.Byte(3);
            return Writer.GetBytes();
        }
    }
}
