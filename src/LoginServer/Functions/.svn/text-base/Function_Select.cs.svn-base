///////////////////////////////////////////////////////////////////////////
// PROJECT SRX
// HOMEPAGE: WWW.XCODING.NET
///////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SrxRevo.Network;
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

                switch (decode.opcode)
                {
                    case CLIENT.CLIENT_PING_CHECK:
                        //To be checked
                        break;
                    case CLIENT.CLIENT_INFO:
                        if (Reader.Text() == "SR_Client")
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
                        break;
                }
            }
            catch (Exception)
            {
            }
        }        
    }
}

