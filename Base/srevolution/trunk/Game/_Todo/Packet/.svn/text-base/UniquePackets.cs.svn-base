///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
// File info: Public packet data
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
        public static byte[] Unique_Data(byte type, int mobid, string name)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_UNIQUE_ANNOUNCE);
            Writer.Byte(type);
            switch (type)
            {
                case 5:
                    Writer.Byte(0x0C);
                    Writer.DWord(mobid);
                    break;
                case 6:
                    Writer.Byte(0x0C);
                    Writer.DWord(mobid);
                    Writer.Text(name);
                    break;
            }


            return Writer.GetBytes();
        }
    }
}
