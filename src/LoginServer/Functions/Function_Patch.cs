///////////////////////////////////////////////////////////////////////////
// Copyright (C) <2011>  <DarkEmu>
// HOMEPAGE: WWW.XFSGAMES.COM.AR
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
            //PacketReader Reader = new PacketReader(PacketInformation.buffer);
            //Reader.Skip(1);
            try
            {
                PacketWriter writer = new PacketWriter();
                writer.Create(SERVER.SERVER_MAIN);
                writer.DWord(0x05000101);
                writer.Byte(0x20);
                client.Send(writer.GetBytes());

                writer.Create(SERVER.SERVER_MAIN);
                writer.DWord(0x01000100);
                writer.DWord(0x00050628);
                writer.Word(0x00);
                writer.Byte(0x02);
                client.Send(writer.GetBytes());

                writer.Create(SERVER.SERVER_MAIN);
                writer.DWord(0x05000101);
                writer.Byte(0x60);
                client.Send(writer.GetBytes());

                writer.Create(SERVER.SERVER_MAIN);
                writer.DWord(0x02000300);
                writer.Word(0x0200);
                client.Send(writer.GetBytes());

                writer.Create(SERVER.SERVER_MAIN);
                writer.DWord(0x00000101);
                writer.Byte(0xA1);
                client.Send(writer.GetBytes());

                writer.Create(SERVER.SERVER_MAIN);
                writer.Word(0x0100);
                client.Send(writer.GetBytes());

                /*if (Reader.Text() == "SR_Client")
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
                }*/
            }
            catch (Exception)
            {
            }
        }
    }
}