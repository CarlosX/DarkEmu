///////////////////////////////////////////////////////////////////////////
// PROJECT SRX
// HOMEPAGE: WWW.XCODING.NET
///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoginServer
{
    public partial class Systems
    {
        public void Patch()
        {
            PacketReader Reader = new PacketReader(PacketInformation.buffer);
            Reader.Skip(1);
            try
            {
                if (Reader.Text() == "SR_Client")
                {
                    int version = Reader.Int32();
                    client.Version = version;
                    //We must keep the version equal to implent patch server later on in development.
                    //When a client isnt equal to our server information, disconnect or update.
                    if (version == LoginServer.Global.Versions.clientVersion) 
                    {
                        client.Send(LoadGame_1());
                        client.Send(LoadGame_2());
                        client.Send(LoadGame_3());
                        client.Send(LoadGame_4());
                        client.Send(LoadGame_5());
                        client.Send(Clientok());
                    }
                    else
                    {
                        // client.Send(PatchRequest());
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}