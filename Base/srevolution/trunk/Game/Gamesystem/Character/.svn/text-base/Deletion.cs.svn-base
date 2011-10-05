///////////////////////////////////////////////////////////////////////////
// SRX Revo: Character deletion
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;

namespace SrxRevo
{
    public partial class Systems
    {
        //Void character delete
        void CharacterDelete()
        {
            //Wrap our function in a catcher
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Skip one not used byte
                Reader.Skip(1);
                //Get character name information
                string CharacterName = Reader.Text();
                //Close packet reader
                Reader.Close();
                //Update and set time + deletion state into the database
                MsSQL.InsertData("UPDATE character SET deletedtime=dateadd(dd,7,getdate()),deleted='1' WHERE name='" + CharacterName + "'");
                //Send visual state of character on screen sit down
                client.Send(Packet.ScreenSuccess(3));
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Character deletion error {0}", ex);
                //Write information to debug logger
                Systems.Debugger.Write(ex);
            }
        }
    }
}