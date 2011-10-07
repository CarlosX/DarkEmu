///////////////////////////////////////////////////////////////////////////
// DarkEmu: Guild packets
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DarkEmu_GameServer
{
    public partial class Packet
    {

        public static byte[] SendGuildStart()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x34B3);
            return Writer.GetBytes();
        }
        public static byte[] GuildUpdate(character c, byte type, int memberid, int permissions, int donatedgp)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_UPDATE);
            switch (type)
            {
                case 1:
                    //Invited user to guild
                    Writer.Byte(2);
                    Writer.DWord(c.Information.CharacterID);
                    Writer.Text(c.Information.Name);
                    Writer.Byte(0x0A); //Check 
                    Writer.Byte(c.Information.Level);
                    Writer.DWord(0);   //Permissions below
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.Word(0);
                    Writer.DWord(c.Information.Model); //Character Model For Icon In Guild
                    Writer.Byte(0);
                    Writer.Byte(c.Position.xSec);
                    Writer.Byte(c.Position.ySec);
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.Byte(1);
                    break;
                case 2:
                    //Disband guild
                    Writer.Byte(1);
                    break;
                case 3:
                    //Transfer Leadership
                    Writer.Byte(0x16);
                    Writer.Byte(2);
                    Writer.Byte(0x54);
                    Writer.DWord(memberid);
                    Writer.Byte(0);
                    Writer.DWord(0xFFFFFFFF);
                    Writer.Byte(1);
                    Writer.DWord(c.Information.CharacterID);
                    Writer.Byte(0x0A);
                    Writer.DWord(0);
                    Writer.Byte(0);
                    break;
                case 4:
                    //Change permissions
                    Writer.Byte(0x16);
                    Writer.Byte(1);
                    Writer.Byte(0x10);
                    Writer.DWord(c.Information.CharacterID);
                    Writer.DWord(permissions);
                    break;
                case 5:
                    //Guild upgrade
                    Writer.Byte(5);
                    Writer.Byte(0x0C);//Members allowed? 12
                    Writer.Byte(c.Network.Guild.Level);
                    Writer.DWord(c.Network.Guild.PointsTotal);
                    break;
                //Invite user online
                case 6:
                    //Static byte 6
                    Writer.Byte(6);
                    //Write member id
                    Writer.DWord(c.Information.CharacterID);
                    //Static byte 2
                    Writer.Byte(2);
                    //Online byte (If online byte = 0, If offline byte = 1).
                    Writer.Byte(Convert.ToBoolean(c.Information.Online) ? 0 : 1);
                    break;
                case 7:
                    //User online / offline
                    Writer.Byte(3);
                    Writer.DWord(c.Information.CharacterID);
                    Writer.Byte(2);
                    break;
                case 8:
                    //Update player level
                    Writer.Byte(6);
                    Writer.DWord(c.Information.CharacterID);
                    Writer.Byte(1);
                    Writer.Byte(c.Information.Level);
                    break;
                case 9:
                    //Update gp information guild
                    Writer.Byte(6);
                    Writer.DWord(c.Information.CharacterID);
                    Writer.Byte(8);
                    Writer.DWord(c.Network.Guild.DonateGP);
                    break;
                case 10:
                    //Update user location
                    Writer.Byte(6);
                    Writer.DWord(c.Information.CharacterID);
                    Writer.Byte(0x80);//need to check
                    Writer.Byte(c.Position.xSec);
                    Writer.Byte(c.Position.ySec);
                    break;
                case 11:
                    //Guild message update
                    Writer.Byte(0x05);
                    Writer.Byte(0x10);
                    Writer.Text(c.Network.Guild.NewsTitle);
                    Writer.Text(c.Network.Guild.NewsMessage);
                    break;
                case 12:
                    //Leave guild
                    Writer.Byte(3);
                    Writer.DWord(c.Information.CharacterID);
                    Writer.Byte(1);
                    break;
                case 13:
                    //Donated gp #1
                    Writer.Byte(5);
                    Writer.Byte(8);
                    Writer.DWord(donatedgp);
                    break;
                case 14:
                    //Union invite send
                    Writer.Byte(c.Network.Guild.TotalMembers);
                    Writer.DWord(c.Network.Guild.Guildid);
                    Writer.Text(c.Network.Guild.Name);
                    Writer.Byte(c.Network.Guild.Level);
                    Writer.Text(c.Information.Name);
                    Writer.DWord(c.Information.Model);
                    Writer.Byte(0x11);//??
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] IconSend(byte type, string icon)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ICON_SEND);
            Writer.Byte(type);
            Writer.Text(icon);
            return Writer.GetBytes();
        }
        public static byte[] PromoteOrDisband(byte information)
        {
            PacketWriter Writer = new PacketWriter();

            switch (information)
            {
                case 1:
                    //Disband message
                    Writer.Create(Systems.SERVER_GUILD_PROMOTE_MSG);
                    Writer.Byte(1);
                    break;
                case 2:
                    //Promote upgrade message
                    Writer.Create(Systems.SERVER_GUILD_DISBAND_MSG);
                    Writer.Byte(1);
                    break;
            }
            return Writer.GetBytes();
        }
        
        public static byte[] GuildKick(int memberid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_DISBAND);
            Writer.DWord(memberid);
            return Writer.GetBytes();
        }
        public static byte[] GuildLeave()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_LEAVE);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] SendGuildInfo(guild guildGlobal)
        {
            PacketWriter Writer = new PacketWriter();
            /////////////////////////////////////////////////////////////////////////
            //Opcode information
            /////////////////////////////////////////////////////////////////////////
            Writer.Create(Systems.SERVER_GUILD_INFO_LOAD);
            /////////////////////////////////////////////////////////////////////////
            //Packet Structure
            /////////////////////////////////////////////////////////////////////////
            //Writer.Byte(1); // Guild update
            Writer.DWord(guildGlobal.Guildid);              // Unique Guild ID
            Writer.Text(guildGlobal.Name);                  // Guild Name
            Writer.Byte(guildGlobal.Level);                 // Guild level
            Writer.DWord(guildGlobal.PointsTotal);          // Guild GP
            Writer.Word(guildGlobal.NewsTitle.Length);      // Guild Message Title Lenght
            Writer.String(guildGlobal.NewsTitle);           // Guild Message Title
            Writer.Word(guildGlobal.NewsMessage.Length);    // Guild Message Lenght
            Writer.String(guildGlobal.NewsMessage);         // Guild Message
            Writer.DWord(0);                                // War on guild id
            Writer.Byte(0);                                 // War status?
            /////////////////////////////////////////////////////////////////////////
            // Write Guild Member information for each excisting member.
            /////////////////////////////////////////////////////////////////////////
            Guild_ListPlayersInfo(guildGlobal.MembersInfo, Writer);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        public static byte[] SendGuildEnd()
        {
            /////////////////////////////////////////////////////////////////////////
            // Guild end
            /////////////////////////////////////////////////////////////////////////
            PacketWriter Writer = new PacketWriter();
            Writer.Create(0x34B4);
            return Writer.GetBytes();
        }
        public static byte[] SendGuildInfo2(character c)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_ONLINE);
            Writer.DWord(c.Information.UniqueID);               // Guildmaster ID
            Writer.DWord(c.Network.Guild.Guildid);              // Guild ID
            Writer.Text(c.Network.Guild.Name);
            if (c.Network.Guild.GrantName != "")
                Writer.Text(c.Network.Guild.GrantName);         // Guildmaster grand name len
            else
                Writer.Word(0);
            Writer.DWord(0);                                    // ?
            Writer.DWord(0);                                    // ?
            Writer.DWord(0);                                    // ? Amount of guilds in union ?
            Writer.Byte(1);                                     // ?
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] GuildSetOnline(guild c, int memberid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_ONLINE);
            Writer.DWord(memberid);
            Writer.DWord(c.Guildid);
            Writer.Text(c.Name);
            Writer.Word(0);
            Writer.DWord(0);
            Writer.DWord(0);
            Writer.DWord(0);
            Writer.Byte(0);
            Writer.Byte(1);
            return Writer.GetBytes();
        }

        public static void Guild_ListPlayersInfo(List<Global.guild_player> guildMembers, PacketWriter Writer) 
        {
            Writer.Byte(guildMembers.Count);
            foreach (Global.guild_player m in guildMembers)
            {
                Writer.DWord(m.MemberID);
                Writer.Text(m.Name);
                Writer.Byte(m.Rank);
                Writer.Byte(m.Level);
                Writer.DWord(m.DonateGP);

                System.Collections.BitArray bits = new System.Collections.BitArray(new bool[]
                {
                    m.noticeeditRight,
                    m.guildstorageRight,
                    m.unionRight,
                    m.withdrawRight,
                    m.joinRight,
                    false,false,false
                });
                byte[] bytes = new byte[1];
                bits.CopyTo(bytes, 0);

                Writer.DWord((int)bytes[0]);
                Writer.DWord(0);
                Writer.DWord(0);
                Writer.DWord(0);
                if (m.GrantName != null)
                {
                    if (m.GrantName != "")
                        Writer.Text(m.GrantName);
                    else
                        Writer.Word(0);
                }
                else
                    Writer.Word(0);

                Writer.DWord(m.Model);
                Writer.Byte(m.FWrank);
                
                Writer.Byte(m.Xsector);
                Writer.Byte(m.Ysector);
                Writer.DWord(0xFFFFFFFF); // when he entered last time 25794314 
                Writer.DWord(0x0189EECA); // when he leveled up last time 25816778 later :P
                Writer.Bool(!m.Online);
            }
        }
        public static byte[] Guild_Create(guild guildGlobal)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD);
            Writer.Byte(1);
            Writer.DWord(guildGlobal.Guildid);
            Writer.Text(guildGlobal.Name);
            Writer.Byte(1);
            Writer.DWord(0);
            if (guildGlobal.NewsTitle != null)
            {
                if (guildGlobal.NewsTitle != "")
                    Writer.Text(guildGlobal.NewsTitle);
                else
                    Writer.Word(0);
            }
            else
                Writer.Word(0);

            if (guildGlobal.NewsMessage != null)
            {
                if (guildGlobal.NewsMessage != "")
                    Writer.Text(guildGlobal.NewsMessage);
                else
                    Writer.Word(0);
            }
            else
                Writer.Word(0);

            Writer.DWord(0);
            Writer.Byte(0);

            Guild_ListPlayersInfo(guildGlobal.MembersInfo, Writer);

            Writer.Byte(0);
            return Writer.GetBytes();
        }

        public static byte[] GuildSetTitle(int charid, string charname, string title)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_SET_TITLE);
            Writer.DWord(charid);
            Writer.Text(charname);
            Writer.Text(title);
            return Writer.GetBytes();
        }
        public static byte[] GuildSetTitle2(int guildid, int charid, string title)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_SET_TITLE2);
            Writer.Byte(1);
            Writer.DWord(guildid);
            Writer.DWord(charid);
            Writer.Word(title.Length);
            Writer.String(title);
            return Writer.GetBytes();
        }
        public static byte[] GuildWarMsg(byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_WAR_MSG);
            switch (type)
            {
                case 2:
                    Writer.Byte(type);
                    Writer.Word(0x4C45);
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] GuildStorageStart(byte type)
        {
            
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_STORAGE);
            switch (type)
            {
                case 1:
                    //Success opening
                    //Need to sniff packet data
                    break;
                case 2:
                    //Level to low
                    Writer.Byte(type);  //Type
                    Writer.Word(0x4C4A);//Message type revert
                    break;

            }
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] GuildStorageGold(character c)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_STORAGE_GOLD);
            Writer.LWord(c.Network.Guild.StorageGold);
            return Writer.GetBytes();
        }

        public static byte[] GuildStorageEnd()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_STORAGE4);
            Writer.Byte(3);
            Writer.Byte(4);
            return Writer.GetBytes();
        }
        public static byte[] GuildStorageDataEnd()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_STORAGE_END);
            return Writer.GetBytes();
        }
        public static byte[] GuildStorageClose()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_STORAGE_CLOSE);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] GuildStorageData(character c)
        {
            Systems.MsSQL getstorage = new Systems.MsSQL("SELECT * FROM char_items WHERE guild_storage_id='" + c.Network.Guild.Guildid + "' AND storagetype='3'");
            int itemcount = getstorage.Count();
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_STORAGE3);
            Writer.Byte(c.Network.Guild.StorageSlots);
            Writer.Byte(itemcount);
            if (itemcount != 0)
            {
                using (System.Data.SqlClient.SqlDataReader reader = getstorage.Read())
                {
                    while (reader.Read())
                    {
                        Item.AddItemPacket(Writer, reader.GetByte(5), reader.GetInt32(2), reader.GetByte(4), reader.GetInt16(6), reader.GetInt32(7), reader.GetInt32(0), reader.GetInt32(9), reader.GetInt32(30));
                    }
                }
            }
            getstorage.Close();  
            return Writer.GetBytes();
        }
        public static byte[] UnionInfo(Systems c)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GUILD_UNIONS);
            Writer.DWord(c.Character.Information.CharacterID);                // Union owner id
            Writer.DWord(7);                                                  // ??Need to check
            Writer.DWord(c.Character.Network.Guild.Guildid);                  // Union leader guild id
            Writer.Byte(c.Character.Network.Guild.Unions.Count);              // Count guilds in union
            //Get all guilds details
            foreach (int guild in c.Character.Network.Guild.Unions)
            {
                //Load other guild data
                Systems.MsSQL guild_data = new Systems.MsSQL("SELECT * FROM guild WHERE id='"+ guild +"'");
                
                using (System.Data.SqlClient.SqlDataReader reader = guild_data.Read())
                {
                    while (reader.Read())
                    {
                        string Guildname        = reader.GetString(1);
                        byte Guildlevel         = reader.GetByte(2);
                        byte Guildmembercount   = reader.GetByte(6);
                        int Ownerid             = reader.GetInt32(9);

                        string Charname = Systems.MsSQL.GetData("SELECT name FROM character WHERE id='" + Ownerid + "'", "name").ToString();
                        int Charmodel   = Convert.ToInt32(Systems.MsSQL.GetData("SELECT chartype FROM character WHERE id='" + Ownerid + "'", "chartype"));
                        
                        Writer.DWord(guild);                        //Guild ID
                        Writer.Text(reader.GetString(1));           //Guildname
                        Writer.Byte(reader.GetByte(2));             //Guildlevel
                        Writer.Text(Charname);                      //Ownername
                        Writer.DWord(Charmodel);                    //Owner model
                        Writer.Byte(reader.GetByte(6));             //Guild member count
                        
                        //Get guild details
                        Systems Guildmembers = Systems.GetGuildPlayer(guild);
                        //Add clients that are online to union list
                        //Null check
                        if (Guildmembers != null)
                        {
                            foreach (int member in Guildmembers.Character.Network.Guild.Members)
                            {
                                //make sure member isnt 0
                                if (member != 0)
                                {
                                    //Get player details
                                    Systems getmember = Systems.GetPlayerMainid(member);
                                    //Make sure that the player is there
                                    if (getmember != null)
                                    {
                                        //Add client to union list
                                        c.Character.Network.Guild.UnionMembers.Add(getmember.Character.Information.CharacterID);
                                        //Add to member
                                        if (c.Character.Information.CharacterID != getmember.Character.Information.CharacterID)
                                        {
                                            getmember.Character.Network.Guild.UnionMembers.Add(c.Character.Information.CharacterID);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Writer.GetBytes();
            
        }
        public static byte[] FORTRESSNOTE()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_FORTRESS_NOTIFY);
            Writer.Byte(0);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
    }
}