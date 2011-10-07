///////////////////////////////////////////////////////////////////////////
// Copyright (C) <2011>  <DarkEmu>
// HOMEPAGE: WWW.XFSGAMES.COM.AR
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
        public static byte[] ServerListPacket(int cliVersion)
        {
            PacketWriter Writer = new PacketWriter();

            Writer.Create(SERVER.SERVER_SERVERLIST);

            Writer.Word(0x0201);
            Writer.Text("SRX_DARKEMU");
            Writer.Byte(0);

            foreach (KeyValuePair<int, Systems.SRX_Serverinfo> GS in Systems.GSList)
            {
                if (GS.Value.Version == 0 || cliVersion == GS.Value.Version)
                {
                    Writer.Bool(true);
                    Writer.Word(GS.Value.id);
                    Writer.Text("1"+GS.Value.name);
                    Writer.Word(GS.Value.usedSlots);
                    Writer.Word(GS.Value.maxSlots);
                    Writer.Byte(GS.Value.status);
                }
            }

            Writer.Byte(0);
            return Writer.GetBytes();

        }
    }
}
