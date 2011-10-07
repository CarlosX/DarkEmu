///////////////////////////////////////////////////////////////////////////
// DarkEmu: Party creation
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Text;
using Framework;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        void CreateFormedParty()
        {
            try
            {
                //Get packet data
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //First and second dword are static
                Reader.Int32();
                Reader.Int32();
                //First byte is our party type
                byte PartyType = Reader.Byte();
                //Second byte is purpose information
                byte PartyPurpose = Reader.Byte();
                //3rd byte is minimal level required to join
                byte PartyMinLevelReq = Reader.Byte();
                //4th byte is max level to join
                byte PartyMaxLevelReq = Reader.Byte();
                //5th is short, party name lenght
                //6th is party name
                string PartyName = Reader.Text3();
                //Close our reader
                Reader.Close();

                //Make sure the user isnt in a party yet.
                if (Character.Network.Party != null)
                {
                    //If party is formed allready return
                    if (Character.Network.Party.IsFormed)
                    {
                        return;
                    }
                    //If party is not formed
                    else
                    {
                        //Get current party information
                        party CurrentParty = Character.Network.Party;
                        //Set formed state
                        CurrentParty.IsFormed = true;
                        //Party type
                        CurrentParty.Type = PartyType;
                        //Party purpose
                        CurrentParty.ptpurpose = PartyPurpose;
                        //Party minimal level
                        CurrentParty.minlevel = PartyMinLevelReq;
                        //Party maximum level
                        CurrentParty.maxlevel = PartyMaxLevelReq;
                        //Party name
                        CurrentParty.partyname = PartyName;
                        //Party owner
                        CurrentParty.LeaderID = Character.Information.UniqueID;
                        //Add party eu / ch information by model
                        CurrentParty.Race = Character.Information.Model;
                        //Send packet information to user
                        client.Send(Packet.CreateFormedParty(CurrentParty));
                        //Add party to list
                        Party.Add(CurrentParty);
                    }
                }
                //If a new formed party is created from 0
                else
                {
                    //New party for details
                    party newparty = new party();
                    //Set formed state
                    newparty.IsFormed = true;
                    //Party type
                    newparty.Type = PartyType;
                    //Party purpose
                    newparty.ptpurpose = PartyPurpose;
                    //Party minimal level
                    newparty.minlevel = PartyMinLevelReq;
                    //Party maximum level
                    newparty.maxlevel = PartyMaxLevelReq;
                    //Party name
                    newparty.partyname = PartyName;
                    //Party owner
                    newparty.LeaderID = Character.Information.UniqueID;
                    //Add party eu / ch information by model
                    newparty.Race = Character.Information.Model;
                    //Add our player to the member list
                    newparty.Members.Add(Character.Information.UniqueID);
                    //Add player client to party list information
                    newparty.MembersClient.Add(client);
                    //Party id , is current count of party's + 1
                    newparty.ptid = Party.Count + 1;
                    //Add the new party list
                    Party.Add(newparty);
                    //Set party to player
                    Character.Network.Party = newparty;
                    //bool set player in party
                    Character.Network.Party.InParty = true;
                    //Send packet information to user
                    client.Send(Packet.CreateFormedParty(newparty));
                }

            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Create formed party error {0}", ex);
                //Write info to the debug log
                Systems.Debugger.Write(ex);
            }
        }
        void PartyAddmembers()
        {
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Read target id
                int targetid = Reader.Int32();
                //Close packet reader
                Reader.Close();
                //Get detailed information from target
                Systems InvitedPlayer = GetPlayer(targetid);
                //Check if the targeted player allready is in a party.
                if (InvitedPlayer.Character.Network.Party == null)
                {
                    //Set target id of target player to our id
                    InvitedPlayer.Character.Network.TargetID = this.Character.Information.UniqueID;
                    //Send request
                    InvitedPlayer.client.Send(Packet.PartyRequest(2, Character.Information.UniqueID, Character.Network.Party.Type));
                }
            }
            //Write bad exception errors
            catch (Exception ex)
            {
                //Write error to the console.
                Console.WriteLine(ex);
                //Write error to the debug log
                Systems.Debugger.Write(ex);
            }
        }
        void NormalRequest()
        {
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Get invited member
                int Target = Reader.Int32();
                //Get party type
                byte PartyType = Reader.Byte();
                //Close reader
                Reader.Close();

                //Get target player information
                Systems InvitedPlayer = GetPlayer(Target);
                //First we check the our own player level
                if (Character.Information.Level < 5)
                {
                    //Send message

                    //Return
                    return;
                }
                //Check target level
                if (InvitedPlayer.Character.Information.Level < 5)
                {
                    //Send message

                    //Return
                    return;
                }
                //Set target information for invited player
                InvitedPlayer.Character.Network.TargetID = Character.Information.UniqueID;
                //If the player inviting, has no party yet.
                if (Character.Network.Party == null)
                {
                    //Create new party
                    party Party = new party();
                    //Set leader of party
                    Party.LeaderID = Character.Information.UniqueID;
                    //Set party type
                    Party.Type = PartyType;
                    //Add to party net info
                    Character.Network.Party = Party;
                }
                //If the target player has no party yet.
                if (InvitedPlayer.Character.Network.Party == null)
                {
                    //Send invitation packet
                    InvitedPlayer.client.Send(Packet.PartyRequest(2, this.Character.Information.UniqueID, PartyType));
                    //Set invite bools
                    InvitedPlayer.Character.Information.CheckParty = true;
                    Character.Information.CheckParty = true;
                }
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine(ex);
                //Write information to the debug log
                Systems.Debugger.Write(ex);
            }
        }
    }
}
