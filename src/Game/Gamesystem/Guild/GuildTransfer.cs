///////////////////////////////////////////////////////////////////////////
// DarkEmu: guild transfer leadership
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
        void GuildTransferLeaderShip()
        {
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Read guild id
                int Guildid = Reader.Int32();
                //Read guild member id to transfer to
                int GuildMemberID = Reader.Int32();
                //Close reader
                Reader.Close();

                //Get detailed player information
                Systems NewLeader = GetPlayerid(GuildMemberID);

                //Update database
                MsSQL.InsertData("UPDATE guild_members SET guild_rank='10',guild_perm_join='0',guild_perm_withdraw='0',guild_perm_union='0',guild_perm_storage='0',guild_perm_notice='0' WHERE guild_member_id='" + Character.Information.CharacterID + "'");
                MsSQL.InsertData("UPDATE guild_members SET guild_rank='0',guild_perm_join='1',guild_perm_withdraw='1',guild_perm_union='1',guild_perm_storage='1',guild_perm_notice='1' WHERE guild_member_id='" + GuildMemberID + "'");

                //Repeat for each member in our guild
                foreach (int member in Character.Network.Guild.Members)
                {
                    //Make sure member is not null
                    if (member != 0)
                    {
                        //Get information for the guildmember
                        Systems guildmember = GetPlayerMainid(member);
                        //Make sure the guildmember isnt null
                        if (guildmember != null)
                        {
                            //Send update packet of new leader
                            guildmember.client.Send(Packet.GuildUpdate(Character, 3, GuildMemberID, 0, 0));
                        }
                    }
                }
                //Send message to old owner
                PacketWriter Writer = new PacketWriter();
                //Add opcode
                Writer.Create(Systems.SERVER_GUILD_TRANSFER_MSG);
                //Static byte 1
                Writer.Byte(1);
                //Send bytes to client
                client.Send(Writer.GetBytes());
            }
            //If a bad exception error happens
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Guild Transfer Error: {0}", ex);
                //Write information to the debug log
                Systems.Debugger.Write(ex);
            }
        }
    }
}