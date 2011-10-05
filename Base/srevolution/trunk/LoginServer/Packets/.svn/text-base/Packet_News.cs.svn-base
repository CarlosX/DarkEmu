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
        public static byte[] News()
        {
            PacketWriter Writer = new PacketWriter();

            Writer.Create(SERVER.SERVER_MAIN);
            Writer.Byte(0);
            Writer.Byte((byte)Systems.News_List.Count);

            foreach (NewsList n in Systems.News_List)
            {
                Writer.Text(n.Head);
                Writer.Text(n.Msg);
                Writer.Word(0);

                Writer.Word(n.Month);
                Writer.Word(n.Day);

                Writer.Word(0);
                Writer.LWord(0);
            }

            Writer.Word(0);

            return Writer.GetBytes();
        }
    }
}
