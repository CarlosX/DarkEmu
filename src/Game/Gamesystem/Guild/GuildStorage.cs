///////////////////////////////////////////////////////////////////////////
// DarkEmu: guild storage system
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
        void GuildStorage()
        {
            //Wrap our function inside a catcher
            try
            {
                //If guild level is to low send message
                if (Character.Network.Guild.Level == 1)
                {
                    //Need to sniff to check what opcode is sending for the message
                    client.Send(Packet.IngameMessages(Systems.SERVER_GUILD_STORAGE, IngameMessages.GUILD_STORAGE_LEVEL_TO_LOW));
                }
                //If guild level is 2 meaning it has storage option
                else
                {
                    //Make sure the user has guild storage rights
                    if (Character.Network.Guild.guildstorageRight)
                    {
                        //Check if other guild members are currently in storage
                        foreach (int member in Character.Network.Guild.Members)
                        {
                            //Make sure member isnt 0
                            if (member != 0)
                            {
                                //Get player details
                                Systems getplayer = GetPlayerMainid(member);
                                //Make sure player isnt null
                                if (getplayer != null)
                                {
                                    //Check if the player is using storage
                                    if (getplayer.Character.Network.Guild.UsingStorage)
                                    {
                                        //Send storage message error
                                        client.Send(Packet.IngameMessages(Systems.SERVER_GUILD_WAIT, IngameMessages.UIIT_MSG_STRGERR_STORAGE_OPERATION_BLOCKED));
                                        return;
                                    }
                                }
                            }
                        }
                        //Make sure that the user isnt using storage allready
                        if (!Character.Network.Guild.UsingStorage)
                        {
                            byte type = 1;
                            //Set user as active storage user
                            Character.Network.Guild.UsingStorage = true;
                            //Send storage begin packet
                            client.Send(Packet.GuildStorageStart(type));
                        }
                    }
                    //If the player has no storage rights
                    else
                    {
                        //Send error message to user not allowed
                        client.Send(Packet.IngameMessages(SERVER_GUILD_STORAGE, IngameMessages.UIIT_MSG_STRGERR_STORAGE_OPERATION_BLOCKED));
                    }
                }
            }
            //Catch any bad errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("GuildStorage Open Error: {0}", ex);
                //Write information to the debug log
                Systems.Debugger.Write(ex);
            }
        }

        void GuildStorage2()
        {
            //Make sure the player is still using storage.
            try
            {
                if (Character.Network.Guild.UsingStorage)
                {
                    LoadPlayerGuildInfo(false);
                    //Send storage data load information
                    client.Send(Packet.GuildStorageGold(Character));
                    client.Send(Packet.GuildStorageData(Character));
                    client.Send(Packet.GuildStorageDataEnd());
                }
            }
            //Catch any bad errors
            catch (Exception ex)
            {
                //Write information to the debug log
                Systems.Debugger.Write(ex);
            }
        }

        void GuildStorageClose()
        {
            try
            {
                //Set bool to false
                Character.Network.Guild.UsingStorage = false;
                //Send close packet for storage window
                client.Send(Packet.GuildStorageClose());
                //Send close packet for npc id window
                Close_NPC();
            }
            //Catch any bad errors
            catch (Exception ex)
            {
                //Write information to the debug log
                Systems.Debugger.Write(ex);
            }
        }
    }
}