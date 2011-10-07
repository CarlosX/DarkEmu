///////////////////////////////////////////////////////////////////////////
// DarkEmu: guild permissions
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
        void GuildPermissions()
        {
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Skip first
                Reader.Skip(1);
                //Read member id
                int memberid = Reader.Int32();
                //Read permission byte
                byte permissions = (byte)Reader.Int32();
                //Set new char bits 8
                char[] bits = new char[8];
                //Repeat for each 8 bits
                for (int i = 0; i < 8; ++i) bits[i] = (char)0;
                //Convert bits to string / to char array
                bits = Convert.ToString(permissions, 2).ToCharArray();
                //Close reader
                Reader.Close();
                //Set amount to player targetindex
                int targetindex = this.Character.Network.Guild.MembersInfo.FindIndex(i => i.MemberID == memberid);
                //If character is online
                if (this.Character.Network.Guild.MembersInfo[targetindex].Online)
                {
                    //Get detailed player information
                    Systems member = GetPlayerMainid(memberid);
                    //Set bits
                    member.Character.Network.Guild.joinRight = bits[4] == '1' ? true : false;
                    member.Character.Network.Guild.withdrawRight = bits[3] == '1' ? true : false;
                    member.Character.Network.Guild.unionRight = bits[2] == '1' ? true : false;
                    member.Character.Network.Guild.guildstorageRight = bits[0] == '1' ? true : false;
                    member.Character.Network.Guild.noticeeditRight = bits[1] == '1' ? true : false;
                }
                // set new amount to every guild members guild class
                foreach (int m in Character.Network.Guild.Members)
                {
                    //Set int index (Find member id)
                    int index = Character.Network.Guild.MembersInfo.FindIndex(i => i.MemberID == m);
                    //If the character is online
                    if (Character.Network.Guild.MembersInfo[index].Online)
                    {
                        //Get detailed information of the player
                        Systems sys = Systems.GetPlayerMainid(m);

                        //Set new guild player
                        Global.guild_player mygp = new Global.guild_player();
                        int myindex = 0;
                        //Repeat for each player
                        foreach (Global.guild_player gp in sys.Character.Network.Guild.MembersInfo)
                        {
                            //if the member id equals the player
                            if (gp.MemberID == memberid)
                            {
                                //Set my gp
                                mygp = gp;
                                //Set bits
                                mygp.joinRight = bits[4] == '1' ? true : false;
                                mygp.withdrawRight = bits[3] == '1' ? true : false;
                                mygp.unionRight = bits[2] == '1' ? true : false;
                                mygp.guildstorageRight = bits[0] == '1' ? true : false;
                                mygp.noticeeditRight = bits[1] == '1' ? true : false;
                                break;
                            }
                            //Index ++
                            myindex++;
                        }
                        //Set membersinfo index as mygp
                        sys.Character.Network.Guild.MembersInfo[myindex] = mygp;
                    }
                }
                //Update guild database information rights
                MsSQL.UpdateData("UPDATE guild_members SET guild_perm_join='" + bits[4] + "',guild_perm_withdraw='" + bits[3] + "',guild_perm_union='" + bits[2] + "',guild_perm_storage='" + bits[0] + "',guild_perm_notice='" + bits[1] + "' WHERE guild_member_id='" + memberid + "'");
                //Send to everyone in guild update of permissions
                Character.Network.Guild.Send(Packet.GuildUpdate(Character, 4, 0, permissions, 0));
            }
            //If an error happens
            catch (Exception ex)
            {
                //Write the information to the console
                Console.WriteLine("Guild permission error: {0}", ex);
                //Write information to the debug log
                Systems.Debugger.Write(ex);
            }
        }
    }
}