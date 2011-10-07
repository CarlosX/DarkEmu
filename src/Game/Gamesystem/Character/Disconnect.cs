///////////////////////////////////////////////////////////////////////////
// DarkEmu: Disconnection system
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    partial class Systems
    {
        //When a player disconnects or is disconnected forcefuly
        public void Disconnect(string type)
        {
            //We open with a try to catch bad exception errors
            try
            {
                //Defined by the string type, we choose the action this case normal disconnection
                if (type == "normal")
                {
                    //Make sure the character is not null
                    if (Character != null)
                    {
                        //Update and set account online to 0
                        MsSQL.UpdateData("UPDATE users SET online='0' WHERE id='" + Player.AccountName + "'");
                        MsSQL.UpdateData("UPDATE character SET online='0' WHERE name='" + Character.Information.Name + "'");
                        //Remove client from client list
                        Systems.clients.Remove(this);
                        //Write information to the console
                        Console.WriteLine("Character: {1} Logged off [Online Players: {0}]", Systems.clients.Count, Character.Information.Name);
                        //If the character is in a party
                        if (Character.Network.Party != null)
                        {
                            //We call remove void for removing the party member
                            RemoveOnDisconnect(Character.Network.Party, this);
                        }
                        //If the character is in a guild
                        if (Character.Network.Guild != null)
                        {
                            //We call remove void to update guild with offline status etc.
                            RemoveMeFromGuildDisconnect(Character.Network.Guild, Character);

                        }
                        //If the character has an active transportation
                        if (this.Character.Transport.Right)
                            //Despawn the transport
                            this.Character.Transport.Horse.DeSpawnMe();
                        //If the character has an active grabpet
                        if (this.Character.Grabpet.Active)
                            //Despawn the grabpet
                            this.Character.Grabpet.Details.DeSpawnMe();
                        //If the character has an open excange window
                        if (this.Character.Network.Exchange.Window)
                            //Close the exchange window (for other player).
                            this.Exchange_Close();
                        //If the character is sitting while closing the client
                        if (Character.State.Sitting)
                        {
                            //Check if the sit down timer is running
                            if (SitDown_HPMP_RegenTimer != null)
                            {
                                //Stop sit down timer
                                StopSitDownTimer();
                            }
                        }
                        //If there is still an active attack timer
                        if (Timer.Attack != null)
                            //Close the timer
                            StopAttackTimer();
                        //If the player has an active buff
                        if (Character.Action.Buff.count > 0)
                            //Close the buff
                            BuffAllClose();
                        //Despawn our character
                        DeSpawnMe();
                        //If mp regeneration timer is not null
                        if (MPRegen != null)
                            //Stop regen timer
                            StopMPRegen();
                        //If hp regeneration timer is not null
                        if (HPRegen != null)
                            //Stop regen timer
                            StopHPRegen();
                        //Save (remove blue data)
                        //Load blue data
                        LoadBlues(Character);
                        //Get list of equipped items
                        List<Global.slotItem> EquipedItems = new List<Global.slotItem>();
                        //If slot is lower then 13 we add the item to the list
                        for (byte q = 0; q < 13; q++)
                        {
                            //Add the equipped items to the list
                            EquipedItems.Add(GetItem((uint)Character.Information.CharacterID, q, 0));
                        }
                        //Load blues for each item
                        foreach (Global.slotItem sitem in EquipedItems)
                        {
                            //If the item has a blue valua that matches our database of blues
                            if (Data.ItemBlue.ContainsKey(sitem.dbID))
                            {
                                //Load blues for the item id
                                LoadBluesid(sitem.dbID);
                                //If the total amount of blues is not 0
                                if (Data.ItemBlue[sitem.dbID].totalblue != 0)
                                    //Remove blue information
                                    AddRemoveBlues(this, sitem, false);
                            }
                        }
                        //Save player current location
                        SavePlayerPosition();
                        //Save player information
                        SavePlayerInfo();
                        //Set ingame bool to false
                        Character.InGame = false;
                        //If player is not null
                        if (Player != null)
                        {
                            //Dispose of player
                            Player.Dispose();
                        }
                        //Stop ping timer
                        PingStop();
                        //Close client
                        client.Close();
                    }
                }
                //If our disconnection type is banning.
                if (type == "ban")
                {

                    //Make sure the character is not null
                    if (Character != null)
                    {
                        //Update and set account online to 0
                        MsSQL.UpdateData("UPDATE users SET online='0' WHERE id='" + Player.AccountName + "'");
                        MsSQL.UpdateData("UPDATE character SET online='0' WHERE name='" + Character.Information.Name + "'");
                        //Remove client from client list
                        Systems.clients.Remove(this);
                        //Write information to the console
                        Console.WriteLine("Character: {1} has been banned hack attempt [Online Players: {0}]", Systems.clients.Count, Character.Information.Name);
                        //If the character is in a party
                        if (Character.Network.Party != null)
                        {
                            //We call remove void for removing the party member
                            RemoveOnDisconnect(Character.Network.Party, this);
                        }
                        //If the character is in a guild
                        if (Character.Network.Guild != null)
                        {
                            //We call remove void to update guild with offline status etc.

                        }
                        //If the character has an active transportation
                        if (this.Character.Transport.Right)
                            //Despawn the transport
                            this.Character.Transport.Horse.DeSpawnMe();
                        //If the character has an active grabpet
                        if (this.Character.Grabpet.Active)
                            //Despawn the grabpet
                            this.Character.Grabpet.Details.DeSpawnMe();
                        //If the character has an open excange window
                        if (this.Character.Network.Exchange.Window)
                            //Close the exchange window (for other player).
                            this.Exchange_Close();
                        //If the character is sitting while closing the client
                        if (Character.State.Sitting)
                        {
                            //Check if the sit down timer is running
                            if (SitDown_HPMP_RegenTimer != null)
                            {
                                //Stop sit down timer
                                StopSitDownTimer();
                            }
                        }
                        //If there is still an active attack timer
                        if (Timer.Attack != null)
                            //Close the timer
                            StopAttackTimer();
                        //If the player has an active buff
                        if (Character.Action.Buff.count > 0)
                            //Close the buff
                            BuffAllClose();
                        //Despawn our character
                        DeSpawnMe();
                        //If mp regeneration timer is not null
                        if (MPRegen != null)
                            //Stop regen timer
                            StopMPRegen();
                        //If hp regeneration timer is not null
                        if (HPRegen != null)
                            //Stop regen timer
                            StopHPRegen();
                        //Save (remove blue data)
                        //Load blue data
                        LoadBlues(Character);
                        //Get list of equipped items
                        List<Global.slotItem> EquipedItems = new List<Global.slotItem>();
                        //If slot is lower then 13 we add the item to the list
                        for (byte q = 0; q < 13; q++)
                        {
                            //Add the equipped items to the list
                            EquipedItems.Add(GetItem((uint)Character.Information.CharacterID, q, 0));
                        }
                        //Load blues for each item
                        foreach (Global.slotItem sitem in EquipedItems)
                        {
                            //If the item has a blue valua that matches our database of blues
                            if (Data.ItemBlue.ContainsKey(sitem.dbID))
                            {
                                //Load blues for the item id
                                LoadBluesid(sitem.dbID);
                                //If the total amount of blues is not 0
                                if (Data.ItemBlue[sitem.dbID].totalblue != 0)
                                    //Remove blue information
                                    AddRemoveBlues(this, sitem, false);
                            }
                        }
                        //Save player current location
                        SavePlayerPosition();
                        //Save player information
                        SavePlayerInfo();
                        //Set ingame bool to false
                        Character.InGame = false;
                        //If player is not null
                        if (Player != null)
                        {
                            //Dispose of player
                            Player.Dispose();
                        }
                        //Stop ping timer
                        PingStop();
                        //Close client
                        client.Close();
                    }
                }
            }
            //When a error happens
            catch (Exception ex)
            {
                //Write the exception error to the console
                Console.WriteLine("Disconnect.cs error {0}", ex);
                //Write info to the debug logger
                Systems.Debugger.Write(ex);
            }
        }
    }
}