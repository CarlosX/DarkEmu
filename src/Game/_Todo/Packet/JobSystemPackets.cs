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
        public static byte[] MakeAlias(string name, byte switchinfo)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_MAKE_ALIAS);
            Writer.Byte(1);
            switch (switchinfo)
            {
                case 0:
                    Writer.Byte(0);
                    Writer.Text(name);
                    break;
                case 1:
                    Writer.Byte(1);
                    Writer.Text(name);
                    break;

                default:
                    Console.WriteLine("Alias Case: " + switchinfo);
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] MakeAliasError(string name, byte switchinfo)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_MAKE_ALIAS);
            Writer.Byte(2);
            Writer.Word(0);
            Writer.Byte(0);
            Writer.Text(name);
            return Writer.GetBytes();
        }
        public static byte[] PrevJobInfo(int character, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PREV_JOB);
            switch (type)
            {
                case 244:
                    Writer.Byte(0x02);
                    Writer.Byte(0x29);
                    Writer.Byte(0x48);
                    break;
                default:
                    //Console.WriteLine("Job Case: " + type);
                    break;
            }

            return Writer.GetBytes();
        }
        public static byte[] JoinMerchant(int id, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_JOIN_MERC);
            switch (type)
            {
                case 3:
                    Writer.Byte(1);
                    Writer.Byte(type);
                    Writer.Byte(1);
                    Writer.DWord(0);
                    break;
                default:
                    //Console.WriteLine("Join hunter Case: " + type);
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] LeaveJob()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_LEAVE_JOB);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
    }
}
