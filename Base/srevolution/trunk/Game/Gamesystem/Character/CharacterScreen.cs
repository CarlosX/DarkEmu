///////////////////////////////////////////////////////////////////////////
// SRX Revo: Character switch
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Text;

namespace SrxRevo
{
    public partial class Systems
    {
        public void CharacterScreen()
        {
            //Wrap our function inside a catcher
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte type = Reader.Byte();
                //Switch on byte type
                switch (type)
                {
                    case 1:
                        //Character creation
                        CharacterCreate();
                        break;
                    case 2:
                        //Character listening
                        client.Send(CharacterListing(Player));
                        break;
                    case 3:
                        //Character deletion
                        CharacterDelete();
                        break;
                    case 4:
                        //Character checking
                        CharacterCheck(PacketInformation.buffer);
                        break;
                    case 5:
                        //Character restoring
                        CharacterRestore();
                        break;
                    case 9:
                        //Character job information
                        CharacterJobInfo();
                        break;
                    case 16:
                        //Select job
                        CharacterJobPick(PacketInformation.buffer);
                        break;
                    default:
                        //We use this if we get a new case.
                        Console.WriteLine("Character Screen Type: " + type);
                        break;
                }
                //Close packet reader
                Reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Characterscreen switch error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }   
    }
}
