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
        public static byte[] CloseNPC()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_CLOSE_NPC);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
        public static byte[] OpenNPC(byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_OPEN_NPC);
            Writer.Bool(true);
            switch (type)
            {
                case 1:
                    //Purchase items
                    Writer.Byte(type);
                    break;
                case 2:
                    Writer.Byte(2);
                    break;
                case 12:
                    Writer.Byte(0x01);
                    Writer.Byte(0x01);
                    break;
                default:
                    Writer.Byte(type);
                    break;
            }
            return Writer.GetBytes();
        }
    }
}
