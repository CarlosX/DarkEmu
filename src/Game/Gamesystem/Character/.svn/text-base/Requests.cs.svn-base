///////////////////////////////////////////////////////////////////////////
// SRX Revo: Requests
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using SrxRevo.Network;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        void CharacterRequest()
        {
            try
            {
                //Open packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);

                //Get targeted player information
                Systems sys = GetPlayer(Character.Network.TargetID);

                if (this.Character.State.UnionApply && sys.Character.State.UnionApply)
                {
                    if (Reader.Byte() == 1 && Reader.Byte() == 0)
                    {
                        //Need to sniff decline packet info
                        Character.State.UnionApply = false;
                        sys.Character.State.UnionApply = false;
                    }
                    else
                    {
                        //Check for null and set unique id for union and insert our own guild.  
                        if (sys.Character.Network.Guild.Unions == null)
                        {
                            int uniqueid = MsSQL.GetRowsCount("SELECT * FROM guild_unions");
                            sys.Character.Network.Guild.UniqueUnion = uniqueid + 1;
                            MsSQL.InsertData("INSERT INTO guild_unions (union_leader, union_guildid, union_unique_id) VALUES ('" + sys.Character.Network.Guild.Guildid + "','" + sys.Character.Network.Guild.Guildid + "','" + sys.Character.Network.Guild.UniqueUnion + "')");
                        }
                        //Update database
                        MsSQL.InsertData("INSERT INTO guild_unions (union_leader, union_guildid, union_unique_id) VALUES ('" + sys.Character.Network.Guild.Guildid + "','" + Character.Network.Guild.Guildid + "','" + sys.Character.Network.Guild.UniqueUnion + "')");

                        //Send union packet to newly joined player
                        client.Send(Packet.UnionInfo(this));

                        //Send update packet to all guilds in union
                        foreach (int member in Character.Network.Guild.Members)
                        {
                            //Make sure the member is there
                            if (member != 0)
                            {
                                //If the user is not the newly invited member get player info
                                Systems tomember = GetPlayerMainid(member);
                                //Send guild update packet
                                if (tomember != null)
                                {
                                    if (!tomember.Character.Network.Guild.SingleSend)
                                    {
                                        if (Character.Information.CharacterID != tomember.Character.Information.CharacterID)
                                        {
                                            tomember.Character.Network.Guild.SingleSend = true;
                                            tomember.client.Send(Packet.GuildUpdate(Character, 14, 0, 0, 0));
                                        }
                                    }
                                }
                            }
                        }
                        foreach (int member in Character.Network.Guild.UnionMembers)
                        {
                            //Make sure the member is there
                            if (member != 0)
                            {
                                //If the user is not the newly invited member get player info
                                Systems tomember = GetPlayerMainid(member);
                                //Send guild update packet
                                if (tomember != null)
                                {
                                    if (!tomember.Character.Network.Guild.SingleSend)
                                    {
                                        tomember.Character.Network.Guild.SingleSend = true;
                                        tomember.client.Send(Packet.GuildUpdate(Character, 14, 0, 0, 0));
                                    }
                                }
                            }
                        }
                        //Disable the bool again
                        foreach (int member in Character.Network.Guild.Members)
                        {
                            //Make sure member isnt null
                            if (member != 0)
                            {
                                //Get guildmember details
                                Systems guildmember = GetPlayerMainid(member);
                                //Make sure guildmember isnt null
                                if (guildmember != null)
                                {
                                    //Disable bool to allow resend new packets.
                                    guildmember.Character.Network.Guild.SingleSend = false;
                                }
                            }
                        }
                        //Disable the bool again
                        foreach (int member in Character.Network.Guild.UnionMembers)
                        {
                            //Make sure member isnt null
                            if (member != 0)
                            {
                                //Get guildmember details
                                Systems guildmember = GetPlayerMainid(member);
                                //Make sure guildmember isnt null
                                if (guildmember != null)
                                {
                                    //Disable bool to allow resend new packets.
                                    guildmember.Character.Network.Guild.SingleSend = false;
                                }
                            }
                        }
                        //Reset bools
                        Character.State.UnionApply = false;
                        sys.Character.State.UnionApply = false;
                    }
                }
                //------------------------------------- [ Exchange invite ] -------------------------------------//
                else if (this.Character.State.Exchanging && sys.Character.State.Exchanging)
                {

                    if (Reader.Byte() == 1 && Reader.Byte() == 0)
                    {

                        sys.client.Send(Packet.Exchange_Cancel());
                        Character.State.Exchanging = false;
                        sys.Character.State.Exchanging = false;
                    }
                    else
                    {
                        sys.client.Send(Packet.OpenExhangeWindow(1, this.Character.Information.UniqueID));
                        client.Send(Packet.OpenExhangeWindow(sys.Character.Information.UniqueID));

                        Character.Network.Exchange.Window = true;
                        Character.Network.Exchange.ItemList = new List<SrxRevo.Global.slotItem>();
                        sys.Character.Network.Exchange.Window = true;
                        sys.Character.Network.Exchange.ItemList = new List<SrxRevo.Global.slotItem>();
                    }
                }
                //------------------------------------- [ Guild invite ] -------------------------------------//
                else if (this.Character.State.GuildInvite && sys.Character.State.GuildInvite)
                {
                    //If byte equals 2 the type is denied
                    if (Reader.Byte() == 2)
                    {
                        //Denied request
                        Character.State.GuildInvite = false;
                        sys.Character.State.GuildInvite = false;
                        //Send refused packet to sender
                        sys.client.Send(Packet.IngameMessages(SERVER_GUILD, IngameMessages.UIIT_MSG_GUILDERR_JOIN_GUILD_REFUSED));
                    }
                    //If not denied we start adding the new member
                    else
                    {
                        //Invite guild member (Add member count + 1).
                        int guildmemberadd = sys.Character.Network.Guild.Members.Count + 1;
                        //Update database
                        MsSQL.InsertData("INSERT INTO guild_members (guild_id, guild_member_id, guild_rank, guild_points, guild_fortress, guild_grant, guild_perm_join, guild_perm_withdraw, guild_perm_union, guild_perm_storage, guild_perm_notice) VALUES ('" + sys.Character.Network.Guild.Guildid + "','" + Character.Information.CharacterID + "','10','0','1','" + "" + "','0','0','0','0','0')");
                        MsSQL.UpdateData("UPDATE guild SET guild_members_t='" + guildmemberadd + "' WHERE guild_name='" + sys.Character.Network.Guild.Name + "'");
                        //Reload new member and load character data for guildinfo
                        LoadPlayerGuildInfo(true);
                        //Send packets to network and spawned players
                        foreach (int member in Character.Network.Guild.Members)
                        {
                            //Make sure the member is there
                            if (member != 0)
                            {
                                //We dont send this info to the invited user.
                                if (member != Character.Information.CharacterID)
                                {
                                    //If the user is not the newly invited member get player info
                                    Systems tomember = GetPlayerMainid(member);
                                    //Send guild update packet
                                    tomember.LoadPlayerGuildInfo(true);
                                    tomember.client.Send(Packet.GuildUpdate(Character, 1, Character.Information.CharacterID, 0, 0));
                                    tomember.client.Send(Packet.GuildSetOnline(Character.Network.Guild, Character.Information.UniqueID));
                                    tomember.client.Send(Packet.GuildUpdate(Character, 6, Character.Information.CharacterID, 0, 0));
                                }
                                //Send information to the invited player
                                else
                                {
                                    //Send guild data packets to invited
                                    client.Send(Packet.SendGuildStart());
                                    client.Send(Packet.SendGuildInfo(Character.Network.Guild));
                                    client.Send(Packet.SendGuildEnd());
                                    //Load union data for new invited player
                                    LoadUnions();
                                }
                            }
                        }
                        //Set bools to false for new invite
                        Character.State.GuildInvite = false;
                        sys.Character.State.GuildInvite = false;
                    }
                }

                //------------------------------------- [ Party invite ] -------------------------------------//
                else
                {
                    //If invitation is accepted
                    if (Reader.Byte() == 1 && Reader.Byte() == 1)
                    {
                        //First we set our main checks (Check if player is in party or not).
                        if (Character.Network.Party != null)
                            return;
                        //Set bools for check
                        Character.Information.CheckParty = true;
                        sys.Character.Information.CheckParty = true;

                        //Set main party information
                        party JoiningParty = sys.Character.Network.Party;

                        //Check party type members allowed need message if full
                        if (JoiningParty.Type == 4 && JoiningParty.Members.Count > 3)
                        {
                            //Send party is full message to player
                            client.Send(Packet.IngameMessages(Systems.SERVER_PARTY_MESSAGES, IngameMessages.UIIT_MSG_PARTYERR_ALREADY_FULL));
                            return;
                        }
                        if (JoiningParty.Type == 5 && JoiningParty.Members.Count > 7)
                        {
                            //Send party is full message to player
                            client.Send(Packet.IngameMessages(Systems.SERVER_PARTY_MESSAGES, IngameMessages.UIIT_MSG_PARTYERR_ALREADY_FULL));
                            return;
                        }
                        //If the current count == 0 then add party and add me
                        if (JoiningParty.Members.Count == 0)
                        {
                            //Add ourselfs to the party list
                            JoiningParty.Members.Add(sys.Character.Information.UniqueID);
                            //Add our client to the party list
                            JoiningParty.MembersClient.Add(sys.client);
                            //Set party id
                            JoiningParty.ptid = Party.Count + 1;
                            //Set party network info
                            sys.Character.Network.Party = JoiningParty;
                            //Send permissions
                            sys.client.Send(Packet.Party_Member(sys.Character.Information.UniqueID));
                            //Send party data to leader
                            sys.client.Send(Packet.Party_DataMember(JoiningParty));
                            //Send party data packet to leader (Other player that joined).
                            sys.client.Send(Packet.Party_Data(2, Character.Information.UniqueID));
                            //Add invited member to the list
                            JoiningParty.Members.Add(Character.Information.UniqueID);
                            JoiningParty.MembersClient.Add(client);
                            //Set party info for invited member
                            Character.Network.Party = JoiningParty;
                            //Send permissions
                            client.Send(Packet.PartyOwnerInformation(Character.Information.UniqueID));
                            //Send party data
                            client.Send(Packet.Party_DataMember(JoiningParty));
                            //return
                            return;
                        }
                        //If there are more members in the current party
                        else
                        {
                            //Repeat for each member using count
                            for (byte b = 0; b <= JoiningParty.Members.Count - 1; b++)
                            {
                                //Get player information from [b]
                                Systems others = GetPlayer(JoiningParty.Members[b]);
                                //Send party data to member
                                others.client.Send(Packet.Party_Data(2, Character.Information.UniqueID));
                            }
                            //Add the invited member to list
                            JoiningParty.Members.Add(Character.Information.UniqueID);
                            //Add the invited client to the list
                            JoiningParty.MembersClient.Add(client);
                            //Set party
                            Character.Network.Party = JoiningParty;
                            //Send permissions
                            client.Send(Packet.PartyOwnerInformation(Character.Information.UniqueID));
                            //Send party data
                            client.Send(Packet.Party_DataMember(JoiningParty));
                            return;
                        }
                    }
                    //If denied request
                    else
                    {
                        //Send denied message to the player joining
                        client.Send(Packet.IngameMessages(Systems.SERVER_PARTY_MEMBER, IngameMessages.UIIT_MSG_PARTYERR_JOIN_PARTY_REFUSED));
                        //Set both bools to false so inviting can be done again
                        sys.Character.Information.CheckParty = false;
                        Character.Information.CheckParty = false;
                    }
                }
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Request error: {0}", ex);
                //Write information to the debug log
                Debugger.Write(ex);
            }
        }
    }
}
