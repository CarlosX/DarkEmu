///////////////////////////////////////////////////////////////////////////
// SRX Revo: guild upgrade system
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        bool IsUpgrading;

        void GuildPromote()
        {
            try
            {
                //Read client information int32 id
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                int guildid = reader.Int32();
                reader.Close();

                //When a guild has reached its max level
                if (Character.Network.Guild.Level == 5)
                {
                    //Send packet to client and return (Message level up max).
                    client.Send(Packet.IngameMessages(Systems.SERVER_GUILD_WAIT, IngameMessages.UIIT_MSG_ERROR_GUILD_LEVEL_UP_FULL));
                    return;
                }
                //Definition for gold needed
                int GoldRequired;
                //Definition for guild points needed
                int GuildPointRequired;
                //Check if player is allready in process of upgrading
                if (!IsUpgrading)
                {
                    //Set bool to true, so we cant dupe
                    IsUpgrading = true;
                    //Load player guild information before we continue (Check last info).
                    LoadPlayerGuildInfo(false);
                    //Create switch on guildl evel
                    switch (Character.Network.Guild.Level)
                    {
                        case 1:
                            GoldRequired = 3000000;
                            GuildPointRequired = 5400;
                            break;
                        case 2:
                            GoldRequired = 9000000;
                            GuildPointRequired = 50400;
                            break;
                        case 3:
                            GoldRequired = 15000000;
                            GuildPointRequired = 135000;
                            break;
                        case 4:
                            GoldRequired = 21000000;
                            GuildPointRequired = 378000;
                            break;
                        default:
                            return;
                    }
                    //Set new guild level definition + 1
                    int NewLevel = Character.Network.Guild.Level + 1;
                    //Set new guild storage slot  amount
                    int NewStorageSlots = Character.Network.Guild.StorageSlots + 30;
                    //If character's gold is not enough
                    if (Character.Information.Gold < GoldRequired)
                    {
                        //Send message to client
                        client.Send(Packet.IngameMessages(SERVER_GUILD_PROMOTE_MSG, IngameMessages.UIIT_MSG_ERROR_GUILD_LEVEL_UP_GOLD_DEFICIT));
                        return;
                    }
                    //Not enough guildpoints
                    if (Character.Network.Guild.PointsTotal < GuildPointRequired)
                    {
                        //Send client message
                        client.Send(Packet.IngameMessages(SERVER_GUILD_PROMOTE_MSG, IngameMessages.UIIT_MSG_ERROR_GUILD_LEVEL_UP_GP_DEFICIT));
                        return;
                    }
                    //Max level
                    if (Character.Network.Guild.Level == 5)
                    {
                        //Send client message
                        client.Send(Packet.IngameMessages(SERVER_GUILD_PROMOTE_MSG, IngameMessages.UIIT_MSG_ERROR_GUILD_LEVEL_UP_FULL));
                        return;
                    }
                    //If everything else is fine
                    else
                    //Upgrade guild initiate
                    {
                        //If max level return just incase.
                        if (Character.Network.Guild.Level == 5) return;
                        //Reduct guildpoints
                        Character.Network.Guild.PointsTotal -= GuildPointRequired;
                        //If the upgrade is final upgrade set points to 0
                        if (Character.Network.Guild.Level == 4) Character.Network.Guild.PointsTotal = 0;
                        //Reduct gold
                        Character.Information.Gold -= GoldRequired;
                        //Send update information to client
                        client.Send(Packet.InfoUpdate(1, Character.Network.Guild.PointsTotal, 0));
                        //Send success message to client
                        client.Send(Packet.PromoteOrDisband(2));
                        //Update guild in database
                        MsSQL.UpdateData("UPDATE guild SET guild_level='" + NewLevel + "',guild_points='" + Character.Network.Guild.PointsTotal + "',guild_storage_slots='" + NewStorageSlots + "' WHERE guild_name='" + Character.Network.Guild.Name + "'");
                        //Repeat for each member in our guild
                        foreach (int member in Character.Network.Guild.Members)
                        {
                            //Check if memeber is not null
                            if (member != 0)
                            {
                                //Get detailed member information
                                Systems memberinfo = GetPlayerMainid(member);
                                //Make sure the member is not null
                                if (memberinfo != null)
                                {
                                    //Reload information for the current guild member
                                    memberinfo.LoadPlayerGuildInfo(false);
                                    //Send guild update packet for current guild member
                                    memberinfo.client.Send(Packet.GuildUpdate(Character, 5, 0, 0, 0));
                                }
                            }
                        }
                        //Save player's gold
                        SaveGold();
                        //Send update gold packet to client
                        client.Send(Packet.UpdateGold(Character.Information.Gold));
                    }
                }
                //Disable the bool so we can upgrade again
                IsUpgrading = false;
            }
            //If any error accures 
            catch (Exception ex)
            {
                //Write error to the console
                Console.WriteLine("Guild Promote Error: {0}", ex);
                //Write error to debug log
                Systems.Debugger.Write(ex);
            }
        }
    }
}