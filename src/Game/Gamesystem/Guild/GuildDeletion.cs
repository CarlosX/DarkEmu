///////////////////////////////////////////////////////////////////////////
// DarkEmu: guild deletion
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        //On leave / disband / kick
        public void SetWaitTime(character c)
        {
            //Update database information
            MsSQL.UpdateData("UPDATE character SET GuildJoining='1',GuildTime='dateadd(dd,3,getdate())' WHERE name='"+ c.Information.Name +"'");
            c.Information.JoinGuildWait = true;
        }

        //When player logs off
        public void RemoveMeFromGuildDisconnect(guild g, character c)
        {
            //Repeat for each client in members of guild
            foreach (Client member in g.MembersClient)
            {
                //Make sure the client is not null
                if (member != null)
                {
                    //Make sure the client is not the one of the player that logs off
                    if (member != client)
                    {
                        //Send packet to client
                        member.Send(Packet.GuildUpdate(c, 6, 0, 0, 0));
                    }
                }
            }
        }

        void KickFromGuild()
        {
            //Wrap our function inside a catcher
            try
            {
                //Open a new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Read short value lenght of name below
                short CharacterNameLen = Reader.Int16();
                //Read string charactername
                string CharacterName = Reader.String(CharacterNameLen);
                //Close packet reader
                Reader.Close();
                //Get player information
                Systems TargetCharacter = GetPlayerName(CharacterName);
                //Send required packets to network
                foreach (int member in Character.Network.Guild.Members)
                {
                    //Make sure member s not null
                    if (member != 0)
                    {
                        //Get information for the guildmember
                        Systems guildmember = GetPlayerMainid(member);
                        //Make sure the guildmember isnt null
                        if (guildmember != null)
                        {
                            //Make sure the kicked member does not receive the packet
                            if (guildmember.Character.Information.CharacterID != TargetCharacter.Character.Information.CharacterID)
                            {
                                guildmember.client.Send(Packet.GuildUpdate(TargetCharacter.Character, 7, 0, 0, 0));
                            }
                        }
                    }
                }
                //Send update packet to the kicked player
                TargetCharacter.client.Send(Packet.GuildUpdate(TargetCharacter.Character, 7, 0, 0, 0));
                //Send guild kick message packet to the kicked player
                PacketWriter Writer = new PacketWriter();
                //Add opcode
                Writer.Create(Systems.SERVER_GUILD_KICK);
                //Add static byte 1
                Writer.Byte(1);
                //Send packet to kicked member
                TargetCharacter.client.Send(Writer.GetBytes());
                //Send guildkick visual packet update to kicked player
                TargetCharacter.Send(Packet.GuildKick(TargetCharacter.Character.Information.UniqueID));
                //Remove the player from database
                MsSQL.UpdateData("DELETE from guild_members where guild_member_id='" + TargetCharacter.Character.Information.CharacterID + "'");
                //Update database
                Character.Network.Guild.TotalMembers -= 1;
                MsSQL.InsertData("UPDATE guild SET guild_members_t='" + Character.Network.Guild.TotalMembers + "' WHERE guild_name='" + Character.Network.Guild.Name + "'");
                TargetCharacter.CleanUp(TargetCharacter);

            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Guild Kick Error: {0}", ex);
                //Write information to the debug log
                Systems.Debugger.Write(ex);
            }
        }

        void CleanUp(Systems c)
        {
            c.Character.Network.Guild.MembersClient.Remove(c.client);
            c.Character.Network.Guild.Members.Remove(c.Character.Information.CharacterID);
            c.Character.Network.Guild.Name = null;
            c.Character.Network.Guild.Level = 0;
            c.Character.Network.Guild.PointsTotal = 0;
            c.Character.Network.Guild.NewsTitle = null;
            c.Character.Network.Guild.NewsMessage = null;
            c.Character.Network.Guild.StorageSlots = 0;
            c.Character.Network.Guild.Wargold = 0;
            c.Character.Network.Guild.StorageGold = 0;
            c.Character.Network.Guild.GuildOwner = 0;
            c.Character.Network.Guild.Guildid = 0;
            c.Character.Network.Guild.GrantName = null;
            c.Character.Network.Guild.FWrank = 0;
            c.Character.Network.Guild.DonateGP = 0;
            c.Character.Network.Guild.LastDonate = 0;
            c.Character.Network.Guild.joinRight = false;
            c.Character.Network.Guild.withdrawRight = false;
            c.Character.Network.Guild.unionRight = false;
            c.Character.Network.Guild.guildstorageRight = false;
            c.Character.Network.Guild.noticeeditRight = false;
            c.Character.Network.Guild.MembersClient = null;
            c.Character.Network.Guild.MembersClient = null;
        }

        void GuildLeave()
        {
            //Write our function inside a catcher
            try
            {
                //Send required packets to network
                foreach (int member in Character.Network.Guild.Members)
                {
                    //Make sure member s not null
                    if (member != 0)
                    {
                        //Get information for the guildmember
                        Systems guildmember = GetPlayerMainid(member);
                        //Make sure the guildmember isnt null
                        if (guildmember != null)
                        {
                            //Send update packet
                            guildmember.client.Send(Packet.GuildUpdate(Character, 12, 0, 0, 0));
                        }
                    }
                }
                
                //Send public packet to in range players (removal guild name).
                Send(Packet.GuildKick(Character.Information.UniqueID));
                //Removal from guild
                client.Send(Packet.GuildLeave());
                //Send normal state packet
                client.Send(Packet.StatePack(Character.Information.UniqueID, 0x04, 0x00, false));
                //Count new members minus 1
                int Membercount = Character.Network.Guild.Members.Count - 1;
                //Update database
                MsSQL.InsertData("UPDATE guild SET guild_members_t='" + Membercount + "' WHERE guild_name='" + Character.Network.Guild.Name + "'");
                //Remove the player from database
                MsSQL.UpdateData("DELETE from guild_members where guild_member_id='" + Character.Information.CharacterID + "'");
                //If the guild has a union
                if (Character.Network.Guild.UniqueUnion != 0)
                {
                    //Set boolean unionactive to false
                    Character.Network.Guild.UnionActive = false;
                    //Remove the member from union member list
                    Character.Network.Guild.UnionMembers.Remove(Character.Information.CharacterID);
                }
                //Cleanup character
                CleanUp(this);
                //Reload information
                LoadPlayerGuildInfo(true);
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information the the debug log
                Systems.Debugger.Write(ex);
            }
        }

        /////////////////////////////////////////////////////////////////////////
        //Guild Disbanding
        /////////////////////////////////////////////////////////////////////////
        #region Guild Disband
        void GuildDisband()
        {
            try
            {
                foreach (int member in Character.Network.Guild.Members)
                {
                    if (member != 0)
                    {
                        Systems guildplayer = GetPlayerMainid(member);
                        if (guildplayer != null)
                        {
                            if (guildplayer.Character.Information.CharacterID != Character.Information.CharacterID)
                            {
                                //Guild disband message packet
                                guildplayer.client.Send(Packet.GuildUpdate(Character, 2, 0, 0, 0));
                                //Remove guild name and details from player
                                Send(Packet.GuildKick(guildplayer.Character.Information.UniqueID));
                                //State packet
                                guildplayer.client.Send(Packet.StatePack(guildplayer.Character.Information.UniqueID, 4, 0, false));
                                //Set all values to null.
                                guildplayer.Character.Network.Guild.Members.Remove(guildplayer.Character.Information.CharacterID);
                                guildplayer.Character.Network.Guild.MembersClient.Remove(guildplayer.client);
                                guildplayer.Character.Network.Guild.Guildid = 0;

                                if (guildplayer.Character.Network.Guild.UniqueUnion != 0)
                                {
                                    guildplayer.Character.Network.Guild.UnionActive = false;
                                    guildplayer.Character.Network.Guild.UnionMembers.Remove(guildplayer.Character.Information.CharacterID);
                                }
                            }
                        }
                    }
                }
                //Guild disband message packet
                client.Send(Packet.GuildUpdate(Character, 2, 0, 0, 0));
                //Remove guild name and details from player
                Send(Packet.GuildKick(Character.Information.UniqueID));
                //State packet
                client.Send(Packet.StatePack(Character.Information.UniqueID, 4, 0, false));
                //Set all values to null.

                //Remove all rows that contains guildname
                MsSQL.UpdateData("DELETE FROM guild_members WHERE guild_id=" + Character.Network.Guild.Guildid + "");
                //Remove guild from guild table
                MsSQL.UpdateData("DELETE FROM guild WHERE id=" + Character.Network.Guild.Guildid + "");
                //Remove ourself
                if (Character.Network.Guild.UniqueUnion != 0)
                {
                    Character.Network.Guild.UnionActive = false;
                    Character.Network.Guild.UnionMembers.Remove(Character.Information.CharacterID);
                }

                Character.Network.Guild.Members.Remove(Character.Information.UniqueID);
                Character.Network.Guild.MembersClient.Remove(client);
                Character.Network.Guild.Guildid = 0;

                //Packet Final message
                client.Send(Packet.PromoteOrDisband(1));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild Disband Error: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
    }
}