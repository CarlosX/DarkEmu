///////////////////////////////////////////////////////////////////////////
// SRX Revo: Party deleting
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Text;
using Framework;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        void DeleteFormedParty(int partynetid)
        {
            try
            {
                //If the party is beeing deleted manually from listening
                if (partynetid == 0)
                {
                    //Read our packet data
                    PacketReader Reader = new PacketReader(PacketInformation.buffer);
                    //Read party id integer
                    int partyid = Reader.Int32();
                    //Close packet reader
                    Reader.Close();
                    //Find the related party id
                    Party.Remove(Party.Find(delegate(party pt)
                    {
                        //If found return the information
                        return pt.IsFormed && (pt.ptid == partyid);
                    }));
                    //Send removal packet for listening
                    PacketWriter Writer = new PacketWriter();
                    Writer.Create(Systems.SERVER_DELETE_FORMED_PARTY);
                    Writer.Byte(1);
                    Writer.DWord(partyid);
                    client.Send(Writer.GetBytes());
                    //Set party state
                    Character.Network.Party.IsFormed = false;
                }
                //If listening is deleted due to auto disband
                else
                {
                    //Find the related party given from partynetid
                    Party.Remove(Party.Find(delegate(party pt)
                    {
                        //Return information
                        return pt.IsFormed && (pt.ptid == partynetid);
                    }));
                    //Remove from listening
                    PacketWriter Writer = new PacketWriter();
                    Writer.Create(Systems.SERVER_DELETE_FORMED_PARTY);
                    Writer.Byte(1);
                    Writer.DWord(partynetid);
                    client.Send(Writer.GetBytes());
                    //Set party state
                    Character.Network.Party.IsFormed = false;
                }
                //If theres only one member leave and remove party data
                if (Character.Network.Party.Members.Count == 1)
                {
                    Character.Network.Party.Members.Remove(Character.Information.UniqueID);
                    Character.Network.Party.MembersClient.Remove(client);

                    Character.Network.Party = null;
                }
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the debug log
                Systems.Debugger.Write(ex);
            }
        }
        void LeaveParty()
        {
            try
            {
                //Make sure the party isnt null to prevent errors.
                if (Character.Network.Party != null)
                {
                    //Remove the character member info
                    Character.Network.Party.Members.Remove(Character.Information.UniqueID);
                    //Remove client
                    Character.Network.Party.MembersClient.Remove(client);
                    //If the owner removes the party set new leader
                    if (Character.Information.UniqueID == Character.Network.Party.LeaderID)
                    {
                        //Repeat for each member in the party
                        foreach (int partymembers in Character.Network.Party.Members)
                        {
                            Systems partymember = GetPlayer(partymembers);
                            //Send party update data to player
                            client.Send(Packet.Party_Data(1, 0));
                            //If the count is 1, we remove the party information
                            if (partymember != null)
                            {
                                if (Character.Network.Party.Members.Count == 1)
                                {
                                    //If its a formed party remove the entry. (check).
                                    if (Character.Network.Party.IsFormed)
                                        DeleteFormedParty(Character.Network.Party.ptid);
                                    //Remove party member from member list
                                    partymember.Character.Network.Party.Members.Remove(partymember.Character.Information.UniqueID);
                                    //Remove the party member client from the list
                                    partymember.Character.Network.Party.MembersClient.Remove(partymember.client);
                                    //Set party to null
                                    partymember.Character.Network.Party = null;
                                    //Set bool to false so the player can join another party
                                    partymember.Character.Information.CheckParty = false;
                                    //Send packet to member
                                    partymember.client.Send(Packet.Party_Data(1, 0));
                                }
                                //If more party members are in the party we dont remove the party.
                                else
                                {
                                    //Get first available member for new leader
                                    partymember.Character.Network.Party.LeaderID = Character.Network.Party.Members[0];
                                    //Send update packet to member
                                    partymember.client.Send(Packet.Party_Data(9, Character.Network.Party.Members[0]));
                                    //Send removal of the user
                                    partymember.client.Send(Packet.Party_Data(3, Character.Information.UniqueID));
                                }
                            }
                        }
                        //Set player information
                        Character.Network.Party = null;
                        Character.Information.CheckParty = false;
                    }

                    else
                    {
                        //Send party update data to player
                        client.Send(Packet.Party_Data(1, 0));
                        //For each member in the party
                        foreach (int partymember in Character.Network.Party.Members)
                        {
                            //Get player information
                            Systems partym = GetPlayer(partymember);
                            //If auto disband party
                            if (Character.Network.Party.Members.Count == 1)
                            {
                                //If its a formed party remove the entry. (check).
                                if (partym.Character.Network.Party.IsFormed) partym.DeleteFormedParty(Character.Network.Party.ptid);
                                //Remove the owner member
                                partym.Character.Network.Party.Members.Remove(this.Character.Information.UniqueID);
                                //Remove the client
                                partym.Character.Network.Party.MembersClient.Remove(this.client);
                                //Set party to null
                                partym.Character.Network.Party = null;
                                //Bool to false so can be invited again
                                partym.Character.Information.CheckParty = false;
                                //Visual update packet
                                partym.client.Send(Packet.Party_Data(1, 0));
                            }
                            //If the player has enough players (Not auto disband).
                            else
                            {
                                //Remove information for all party members
                                partym.Character.Network.Party.Members.Remove(Character.Information.UniqueID);
                                //Remove the client
                                partym.Character.Network.Party.MembersClient.Remove(client);
                                //Remove the id
                                partym.client.Send(Packet.Party_Data(3, Character.Information.UniqueID));
                                //Set null party info
                                partym.Character.Network.Party = null;
                                //Set bool
                                partym.Character.Information.CheckParty = false;
                            }
                        }
                        //Set party network to null
                        Character.Network.Party = null;
                        //Set bool to false so player can go in new party.
                        Character.Information.CheckParty = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Leave party error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }

        public static void RemoveOnDisconnect(party p, Systems c)
        {
            //Remove client and member if it contains our removing character
            if (p.Members.Contains(c.Character.Information.UniqueID))
            {
                p.Members.Remove(c.Character.Information.UniqueID);
                p.MembersClient.Remove(c.client);
            }
            //Send packet to each player
            foreach (int member in p.Members)
            {
                Systems playerdetail = GetPlayer(member);

                if (p.Members.Count > 1)
                {
                    playerdetail.client.Send(Packet.Party_Data(1, 0));
                }
                else
                {
                    //Send removal of party
                    playerdetail.client.Send(Packet.Party_Data(3, playerdetail.Character.Information.UniqueID));
                    //Remove party state
                    playerdetail.Character.Network.Party = null;
                }
            }
        }
        void PartyBan()
        {
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int TargetID = Reader.Int32();
                Reader.Close();
                //Get targeted player information
                Systems s = GetPlayers(TargetID);
                //Remove id of the member
                Character.Network.Party.Members.Remove(s.Character.Information.UniqueID);
                //Remove the client of the member
                Character.Network.Party.MembersClient.Remove(s.client);
                //Repeat for each member the updated party information
                foreach (int partymember in Character.Network.Party.Members)
                {
                    //Get player information for the next member
                    Systems partym = GetPlayer(partymember);
                    //Remove the kicked player
                    partym.Character.Network.Party.Members.Remove(s.Character.Information.UniqueID);
                    partym.Character.Network.Party.MembersClient.Remove(s.client);

                    //If we have one member remaining in the party we disband the party
                    if (partym.Character.Network.Party.Members.Count == 1)
                    {
                        //If its formed in the list remove the listening
                        if (partym.Character.Network.Party.IsFormed)
                            partym.DeleteFormedParty(Character.Network.Party.ptid);
                        //Send update packet to the party member                       
                        partym.client.Send(Packet.Party_Data(1, 0));
                        //Send update packet to the current player
                        client.Send(Packet.Party_Data(1, 0));
                        //Set party to null for the current player
                        Character.Network.Party = null;
                        //Set party to null for the remaining member
                        partym.Character.Network.Party = null;
                        //Set bool for current player
                        Character.Information.CheckParty = false;
                        //Set bool for the remaining party member
                        partym.Character.Information.CheckParty = false;
                    }
                    //If there are more members (Not autodisband party).
                    else
                    {
                        //Send the update packet to the party member
                        partym.client.Send(Packet.Party_Data(3, TargetID));
                    }
                }
                //Set the kicked player bool to false
                s.Character.Information.CheckParty = false;
                //Remove the party network for the kicked player
                s.Character.Network.Party = null;
                //Send update packet to the kicked player
                s.client.Send(Packet.Party_Data(1, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Party Ban Error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
    }
}
