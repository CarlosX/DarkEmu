///////////////////////////////////////////////////////////////////////////
// DarkEmu: Party joining
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////

//TODO: Add timer for joining (Response timer).
using System;
using System.Linq;
using System.Text;
using Framework;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        void JoinFormedParty()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Packet reader party id
                int PartyID = Reader.Int32();
                Reader.Close();
                //Checks
                if (PartyID == 0) return;
                //Get character id from the party id
                int Playerid = GetPartyleader(PartyID);
                //Get character information
                Systems sys = GetPlayer(Playerid);
                //Open the invite
                sys.client.Send(Packet.JoinFormedRequest(Character, sys.Character));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Formed party join error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////
        //Formed party join response
        /////////////////////////////////////////////////////////////////////////
        void FormedResponse()
        {
            try
            {
                //Open our packet data reader
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                int Requestermodel = reader.Int32();
                int Characterid = reader.Int32();
                byte responsetype = reader.Byte();
                reader.Close();
                //Get player information
                Systems sys = GetPlayer(Characterid);
                //If the requester is not in a party yet
                if (sys.Character.Network.Party == null)
                {
                    //If party doesnt excist anymore or is not listed anymore
                    if (Character.Network.Party != null)
                    {
                        //If not formed anymore
                        if (!Character.Network.Party.IsFormed)
                        {
                            //Send cannot find party message
                            sys.client.Send(Packet.IngameMessages(Systems.SERVER_PARTY_MESSAGES, IngameMessages.UIIT_MSG_PARTYERR_CANT_FIND_PARTY));
                            return;
                        }
                    }
                    //If party is null
                    else
                    {
                        //Send cannot find party message
                        sys.client.Send(Packet.IngameMessages(Systems.SERVER_PARTY_MESSAGES, IngameMessages.UIIT_MSG_PARTYERR_CANT_FIND_PARTY));
                        return;
                    }
                    //Accept new member
                    if (responsetype == 1)
                    {
                        //Check party type members allowed need message if full
                        if (Character.Network.Party.Type == 4 && Character.Network.Party.Members.Count > 3)
                        {
                            //Add msg party full
                            return;
                        }
                        if (Character.Network.Party.Type == 5 && Character.Network.Party.Members.Count > 7)
                        {
                            //Add msg party full
                            return;
                        }
                        //Send packets to creator and invited member
                        sys.client.Send(Packet.Party_Member(sys.Character.Information.UniqueID));
                        client.Send(Packet.PartyOwnerInformation(Character.Information.UniqueID));

                        //Set bools for check
                        Character.Information.CheckParty = true;
                        //Bool for requester
                        sys.Character.Information.CheckParty = true;

                        //Add member
                        Character.Network.Party.Members.Add(sys.Character.Information.UniqueID);
                        Character.Network.Party.MembersClient.Add(sys.client);

                        //Send packet for each member in party
                        foreach (int member in Character.Network.Party.Members)
                        {
                            if (member != 0)
                            {
                                Systems mainParty = GetPlayer(member);
                                //If the member is the owner
                                if (mainParty != null)
                                {
                                    if (mainParty.Character.Information.CharacterID == Character.Information.CharacterID)
                                    {
                                        //Just send update packet
                                        mainParty.client.Send(Packet.Party_DataMember(mainParty.Character.Network.Party));
                                    }
                                    //For other members
                                    else
                                    {
                                        //Send member joined packet
                                        mainParty.client.Send(Packet.JoinResponseMessage(1));
                                        //Send update packet
                                        mainParty.client.Send(Packet.Party_DataMember(Character.Network.Party));
                                        //Set new party data
                                        mainParty.Character.Network.Party = Character.Network.Party;
                                    }
                                }

                            }
                        }
                    }
                    //Refuse
                    else
                    {
                        //If the party is a new party
                        if (sys.Character.Network.Party.Members.Count < 1)
                        {
                            Character.Information.CheckParty = false;
                        }
                        else
                        {
                            sys.client.Send(Packet.Party_Member(sys.Character.Information.UniqueID));
                            Character.Information.CheckParty = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Formed response error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
    }
}
