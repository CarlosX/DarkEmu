///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
// File info: Public packet data
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
