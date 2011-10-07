///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
// File info: Private packet data
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
        public static byte[] StartingLeaveGame(byte time, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_LEAVE_ACCEPT);
            Writer.Byte(1);
            Writer.DWord(time);
            Writer.Byte(type);
            return Writer.GetBytes();
        }
        public static byte[] EndLeaveGame()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_LEAVE_SUCCESS);
            return Writer.GetBytes();
        }
        public static byte[] CancelLeaveGame()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_LEAVE_CALCEL);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
    }
}
