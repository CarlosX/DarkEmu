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
        public static byte[] OpenWarehouse(long t)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_OPEN_WAREHOUSE);
            Writer.LWord(t);
            return Writer.GetBytes();
        }
        public static byte[] OpenWarehouse2(byte storageslots, player c)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_OPEN_WAREPROB);
            Writer.Byte(storageslots);

            Systems.MsSQL ms = new Systems.MsSQL("SELECT * FROM char_items WHERE storageacc='" + c.ID + "' AND storagetype='1'");

            Writer.Byte(ms.Count());
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                    
                    Item.AddItemPacket(Writer, reader.GetByte(5), reader.GetInt32(2), reader.GetByte(4), reader.GetInt16(6), reader.GetInt32(7), reader.GetInt32(0), reader.GetInt32(9),reader.GetInt32(30));
            }
            ms.Close();
            return Writer.GetBytes();
        }
        public static byte[] OpenWarehouse3()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_OPEN_WAREHOUSE2);
            return Writer.GetBytes();
        }
    }
}
