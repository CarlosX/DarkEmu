///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Client connecting
        /////////////////////////////////////////////////////////////////////////////////
        #region Client Connect
        public void Connect()
        {
            //Wrap our function inside a catcher
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte type = Reader.Byte();
                Reader.Skip(3);
                string ID = Reader.Text();
                string PW = Reader.Text();
                Reader.Close();
                //Set login result information
                int LoginResult = LoginUser(ID, ref PW, ref Player, true);
                //If the login is succesfull
                if (LoginResult == 0)
                {
                    //Send succes packet
                    client.Send(Packet.ConnectSuccess());
                }
                //If the login is wrong
                else
                {
                    //Disconnect the user
                    client.Disconnect(PacketInformation.Client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client connect error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Loading game packets
        /////////////////////////////////////////////////////////////////////////////////
        #region Send Load Packets
        public void Patch()
        {
            //Wrap our function inside a catcher
            try
            {
                //Send packets (Needs to be more specified).
                client.Send(Packet.AgentServer());
                client.Send(Packet.LoadGame_1());
                client.Send(Packet.LoadGame_2());
                client.Send(Packet.LoadGame_3());
                client.Send(Packet.LoadGame_4());
                client.Send(Packet.LoadGame_5());
                client.Send(Packet.LoadGame_6());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Patch error Send load packets {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion

    }
}
