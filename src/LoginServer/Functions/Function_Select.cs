///////////////////////////////////////////////////////////////////////////
// Copyright (C) <2011>  <DarkEmu>
// HOMEPAGE: WWW.XFSGAMES.COM.AR
///////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using DarkEmu_GameServer.Network;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace LoginServer
{
    public partial class Systems
    {        
        public static void oPCode(Decode decode)
        {
            try
            {
                Systems sys = (Systems)decode.Packet;
                sys.PacketInformation = decode;
                PacketReader Reader = new PacketReader(sys.PacketInformation.buffer);

                //Console.WriteLine("Opcode: {0}",decode.opcode);

                switch (decode.opcode)
                {
                    case CLIENT.CLIENT_HANDSHAKE:
                        break;
                    case CLIENT.CLIENT_PING_CHECK:
                        //To be checked
                        break;
                    case CLIENT.CLIENT_INFO:
                        //if (Reader.Text() == "SR_Client")
                        sys.client.Send(GateWayPacket());
                        break;
                    case CLIENT.CLIENT_UPDATE:
                        sys.Patch();
                        break;
                    case CLIENT.CLIENT_SERVERLIST:
                        sys.client.Send(ServerListPacket(sys.client.Version));
                        break;
                    case CLIENT.CLIENT_AUTH:
                        sys.Connect();
                        break;
                    case CLIENT.CLIENT_OPEN:
                        sys.client.Send(LoadGame_6());
                        sys.client.Send(News());
                        break;
                    case 1905:
                        byte[] buffer = decode.buffer;
                        User_Current = BitConverter.ToInt16(buffer, 0);
                        break;
                    default:
                        Console.WriteLine("Default Opcode: {0}", decode.opcode);
                        break;
                }
            }
            catch (Exception)
            {
            }
        }        
    }
}

