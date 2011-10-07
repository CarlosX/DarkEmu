///////////////////////////////////////////////////////////////////////////
// DarkEmu: Ingame Login
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Text;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        void checkSameChar(string name, int id)
        {
            //Wrap our function inside a catcher
            try
            {
                //Lock the client
                lock (Systems.clients)
                {
                    //For each client currently connected
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        //If the client is null, or the client character name is same as our login name
                        if (Systems.clients[i] != null && Systems.clients[i].Character.Information.Name == name || Systems.clients[i].Character.Information.UniqueID == id)
                        {
                            //Disconnect the user
                            Systems.clients[i].Disconnect("normal");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Check same character error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }

        public void IngameLogin()
        {
            //Wrap our function inside a catcher
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Read character name from packet
                string CharacterName = Reader.Text();
                //Close reader
                Reader.Close();
                //Anti hack checking sql query
                Systems.MsSQL ms = new Systems.MsSQL("SELECT name FROM character WHERE account='" + Player.AccountName + "' AND name='" + CharacterName + "'");
                //Check if the player account and character belongs together (count row).
                int checkinfo = ms.Count();
                //If there's no result
                if (checkinfo == 0)
                {
                    //Optional ban user here for hacking.

                    //Disconnect the user if hack attempt
                    client.Disconnect(this.client.clientSocket);
                    return;
                }
                //If there's a result we continue loading
                else
                {

                    //Create new character definition details
                    Character = new character();
                    //Set character name
                    Character.Information.Name = CharacterName;
                    //Set player id
                    Character.Account.ID = Player.ID;
                    //Load player data
                    PlayerDataLoad();
                    //Load job data
                    LoadJobData();
                    //Check same character
                    checkSameChar(CharacterName, Character.Information.UniqueID);
                    //Check character stats
                    CheckCharStats(Character);
                    //Lock while we add new client
                    lock (Systems.clients)
                    {
                        //Add new client
                        Systems.clients.Add(this);
                    }
                    //Send login screen packet
                    client.Send(Packet.LoginScreen());
                    //Send player data load start packet
                    client.Send(Packet.StartPlayerLoad());
                    //Send player data load data
                    client.Send(Packet.Load(Character));
                    //Send end load for player data
                    client.Send(Packet.EndPlayerLoad());
                    //Update online status in database
                    MsSQL.UpdateData("UPDATE character SET online='1' WHERE id='" + Character.Information.CharacterID + "'");
                    //Set PVP State
                    MsSQL.UpdateData("UPDATE character SET Pvpstate='0' WHERE id='" + Character.Information.CharacterID + "'");
                    //Update server information (Players online).
                    UpdateServerInfo();
                    //Open our timers for spawn checks etc.
                    OpenTimer();
                    //Load blue data for character
                    LoadBlues(Character);
                    //Create new list for equiped items
                    List<Global.slotItem> EquipedItems = new List<Global.slotItem>();
                    //For each equiped item under slot 13
                    for (byte q = 0; q < 13; q++)
                    {
                        //Add items to the list
                        EquipedItems.Add(GetItem((uint)Character.Information.CharacterID, q, 0));
                    }
                    //Load blues for each item
                    foreach (Global.slotItem sitem in EquipedItems)
                    {
                        //Check if the dictionary contains our blue id on item
                        if (Data.ItemBlue.ContainsKey(sitem.dbID))
                        {
                            //If exists, load blue for the item
                            LoadBluesid(sitem.dbID);
                            //If blue amount is not 0
                            if (Data.ItemBlue[sitem.dbID].totalblue != 0)
                                //Add blue to stats and information
                                AddRemoveBlues(this, sitem, true);
                        }
                    }
                    //Default luck (Will be based on tickets increasment etc.
                    this.Character.Blues.Luck = 100;
                }
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write to debug log
                Systems.Debugger.Write(ex);
            }
        }
    }
}
