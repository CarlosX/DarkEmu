///////////////////////////////////////////////////////////////////////////
// DarkEmu: Guild system
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
        public void LoadGuildMembers()
        {
            //Wrap our code in a try to catch bad exception errors
            try
            {
                //Load guild member id's
                LoadGuildMemberIds(Character.Network.Guild.Guildid, ref Character.Network.Guild.Members);
                //Repeat for each member in the guild member list
                foreach (int Guildmember in Character.Network.Guild.Members)
                {
                    //Set new guild player information
                    Global.guild_player PlayerGuild = new Global.guild_player();
                    //Set guildmember id
                    PlayerGuild.MemberID = Guildmember;
                    //Create new mssql query to get player information
                    Systems.MsSQL ms = new Systems.MsSQL("SELECT * FROM character WHERE id='" + Guildmember + "'");
                    //Create mssql data reader
                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        //While the reader is reading
                        while (reader.Read())
                        {
                            //Get player model
                            PlayerGuild.Model = reader.GetInt32(3);
                            //Get player xsector
                            PlayerGuild.Xsector = reader.GetByte(16);
                            //Get player ysector
                            PlayerGuild.Ysector = reader.GetByte(17);
                            //Get player level
                            PlayerGuild.Level = reader.GetByte(5);
                            //Get player name
                            PlayerGuild.Name = reader.GetString(2);
                            //Get player online state
                            PlayerGuild.Online = (reader.GetInt32(47) == 1);
                            //If player is online
                            if (PlayerGuild.Online)
                            {
                                //Get detailed player information
                                Systems sys = GetPlayerMainid(Guildmember);
                                //Make sure sys is not null
                                if (sys != null)
                                    //Add the character client to the client list
                                    this.Character.Network.Guild.MembersClient.Add(sys.client);
                            }
                        }
                    }
                    //Create new query to select from table guild_members
                    ms = new Systems.MsSQL("SELECT * FROM guild_members WHERE guild_member_id='" + Guildmember + "'");
                    //Create new sql data reader
                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        //While the sql data reader is reading
                        while (reader.Read())
                        {
                            //Get player rights
                            PlayerGuild.joinRight = (reader.GetByte(7) == 1);
                            PlayerGuild.withdrawRight = (reader.GetByte(8) == 1);
                            PlayerGuild.unionRight = (reader.GetByte(9) == 1);
                            PlayerGuild.guildstorageRight = (reader.GetByte(10) == 1);
                            PlayerGuild.noticeeditRight = (reader.GetByte(11) == 1);
                            PlayerGuild.FWrank = reader.GetByte(6);
                            //Get player donated gp
                            PlayerGuild.DonateGP = reader.GetInt32(4);
                            //Get player rank
                            PlayerGuild.Rank = reader.GetByte(3);
                        }
                    }
                    //Close mssql
                    ms.Close();
                    //Add our character to the guild member info
                    Character.Network.Guild.MembersInfo.Add(PlayerGuild);
                }
            }
            //Catch bad exceptions
            catch (Exception ex)
            {
                //Write error to the console
                Console.WriteLine(ex);
                //Write error to the debug log file
                Systems.Debugger.Write(ex);
            }
        }

        public void LoadPlayerGuildInfo(bool logon)
        {
            try
            {
                MsSQL ms = new MsSQL("SELECT * FROM guild_members WHERE guild_member_id='" + Character.Information.CharacterID + "'");
                using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        Character.Network.Guild.Guildid = reader.GetInt32(1);
                        Character.Network.Guild.GrantName = reader.GetString(5);
                        Character.Network.Guild.FWrank = reader.GetByte(6);
                        Character.Network.Guild.DonateGP = reader.GetInt32(4);
                        Character.Network.Guild.LastDonate = Character.Network.Guild.DonateGP;
                        Character.Network.Guild.joinRight = (reader.GetByte(7) == 1);
                        Character.Network.Guild.withdrawRight = (reader.GetByte(8) == 1);
                        Character.Network.Guild.unionRight = (reader.GetByte(9) == 1);
                        Character.Network.Guild.guildstorageRight = (reader.GetByte(10) == 1);
                        Character.Network.Guild.noticeeditRight = (reader.GetByte(11) == 1);
                    }
                }

                ms = new MsSQL("SELECT * FROM guild WHERE id='" + Character.Network.Guild.Guildid + "'");
                using (System.Data.SqlClient.SqlDataReader reader2 = ms.Read())
                {
                    while (reader2.Read())
                    {
                        Character.Network.Guild.Name = reader2.GetString(1);
                        Character.Network.Guild.Level = reader2.GetByte(2);
                        Character.Network.Guild.PointsTotal = reader2.GetInt32(3);
                        Character.Network.Guild.NewsTitle = reader2.GetString(4);
                        Character.Network.Guild.NewsMessage = reader2.GetString(5);
                        Character.Network.Guild.StorageSlots = reader2.GetInt32(7);
                        Character.Network.Guild.Wargold = reader2.GetInt32(8);
                        Character.Network.Guild.StorageGold = reader2.GetInt64(11);
                        Character.Network.Guild.GuildOwner = reader2.GetInt32(9);
                    }
                }

                ms = new MsSQL("SELECT * FROM guild_members WHERE guild_id='" + Character.Network.Guild.Guildid + "'");
                Character.Network.Guild.TotalMembers = (byte)(ms.Count());

                ms.Close();
                //Set max players allowed in guild
                switch (Character.Network.Guild.Level)
                {
                    case 1:
                        Character.Network.Guild.MaxMembers = 20;
                        break;
                    case 2:
                        Character.Network.Guild.MaxMembers = 25;
                        break;
                    case 3:
                        Character.Network.Guild.MaxMembers = 30;
                        break;
                    case 4:
                        Character.Network.Guild.MaxMembers = 35;
                        break;
                    case 5:
                        Character.Network.Guild.MaxMembers = 50;
                        break;
                }

                //Only load on player login
                if (logon)
                {
                    LoadGuildMembers();
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
                Console.WriteLine("LoadPlayerGuildInfo error {0}", ex);
            }
        }

        public void LoadGuildMemberIds(int guildid, ref List<int> MemberIDs)
        {
            try
            {
                //Make sure we start with a clean list
                if (MemberIDs != null)
                    //If not null clear the list
                    MemberIDs.Clear();
                //Create new query to get guild member information
                MsSQL ms = new MsSQL("SELECT * FROM guild_members WHERE guild_id='" + guildid + "'");
                //Create sql data reader to read database content
                using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                {
                    //While the reader is reading
                    while (reader.Read())
                    {
                        //Add member id to the list
                        MemberIDs.Add(reader.GetInt32(2));
                    }
                }
            }
            //Catch any bad exception error
            catch (Exception ex)
            {
                //Write information to debug log
                Systems.Debugger.Write(ex);
            }
        }       
    }
}