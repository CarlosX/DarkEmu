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
        public static byte[] GateWayPacket()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(SERVER.SERVER_INFO);
            Writer.Text("GatewayServer");
            Writer.Byte(0);
            return Writer.GetBytes();
        }
    }
}
