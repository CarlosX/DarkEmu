///////////////////////////////////////////////////////////////////////////
// SRX Revo: Character restoring
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;

namespace SrxRevo
{
    public partial class Systems
    {
        void CharacterRestore()
        {
            //Wrap our function in a catcher
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Skip one byte
                Reader.Skip(1);
                //Read charactername to be restored
                string CharacterName = Reader.Text();
                Reader.Close();
                //Update database information set deleted state to 0
                MsSQL.InsertData("UPDATE character SET deleted='0' WHERE name='" + CharacterName + "'");
                //Send state packet to client character standing up
                client.Send(Packet.ScreenSuccess(5));
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Character restore error {0}", ex);
                //Write information to the debug log
                Systems.Debugger.Write(ex);
            }   
        }
    }
}