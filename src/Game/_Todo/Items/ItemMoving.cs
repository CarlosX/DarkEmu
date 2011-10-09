///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Collections.Generic;
namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Take gold from storage
        /////////////////////////////////////////////////////////////////////////////////
        void Player_TakeGoldW(byte type, long gold)
        {
            #region Take gold from storage
            try
            {
                //Check if the user isnt in any state/action that we will not allow.
                if (!Character.State.Die || !Character.Stall.Stallactive || !Character.Alchemy.working || !Character.Position.Walking)
                {
                    //Normal storage
                    if (!Character.Network.Guild.UsingStorage)
                    {
                        //Check if the gold is equally or higher to the amount withdrawing
                        if (Character.Account.StorageGold >= gold)
                        {
                            //If ok, reduce the gold from storage
                            Character.Account.StorageGold -= gold;
                            //Add the gold to inventory gold
                            Character.Information.Gold += gold;
                            //Send packet update gold
                            client.Send(Packet.UpdateGold(Character.Information.Gold));
                            //Send visual update
                            client.Send(Packet.MoveItem(type, 0, 0, 0, gold, "MOVE_WAREHOUSE_GOLD"));
                            //Save the gold information
                            SaveGold();
                        }
                        //If gold is to low
                        else
                        {
                            //Send error message to the player.
                            client.Send(Packet.IngameMessages(SERVER_UPDATEGOLD, IngameMessages.UIIT_MSG_STRGERR_NOT_ENOUGH_GOLD));
                        }
                    }
                    //Guild storage
                    else
                    {
                        //Check if the gold is equally or higher to the amount withdrawing
                        if (Character.Network.Guild.StorageGold >= gold)
                        {
                            //If ok, reduce the gold from storage
                            Character.Network.Guild.StorageGold -= gold;
                            //Add the gold to inventory gold
                            Character.Information.Gold += gold;
                            //Send packet update gold
                            client.Send(Packet.UpdateGold(Character.Information.Gold));
                            client.Send(Packet.GuildGoldUpdate(Character.Network.Guild.StorageGold, 0x21));
                            client.Send(Packet.GuildStorageGold(Character));
                            //Save the gold information
                            SaveGold();
                            //Save guild gold
                            SaveGuildGold();
                        }
                        //If gold is to low
                        else
                        {
                            //Send error message to the player.
                            client.Send(Packet.IngameMessages(SERVER_UPDATEGOLD, IngameMessages.UIIT_MSG_STRGERR_NOT_ENOUGH_GOLD));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
                Console.WriteLine("Take gold from warehouse error {0}", ex);
            }
            //Save player information
            SavePlayerInfo();
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Put gold to storage
        /////////////////////////////////////////////////////////////////////////////////
        void Player_GiveGoldW(byte type, long gold)
        {
            #region Instert gold into storage
            try
            {
                //Check if the user isnt in any state/action that we will not allow.
                if (!Character.State.Die || !Character.Stall.Stallactive || !Character.Alchemy.working || !Character.Position.Walking)
                {
                    //Check if the deposit gold is equally or higher then player inventory gold
                    if (Character.Information.Gold >= gold)
                    {
                        //If the user is storing in normal storage
                        if (!Character.Network.Guild.UsingStorage)
                        {
                            //Set new storage gold plus
                            Character.Account.StorageGold += gold;
                            //Update player inventory gold minus
                            Character.Information.Gold -= gold;
                            //Send update packet
                            client.Send(Packet.UpdateGold(Character.Information.Gold));
                            //Send visual packet of moving gold
                            client.Send(Packet.MoveItem(type, 0, 0, 0, gold, "MOVE_WAREHOUSE_GOLD"));
                            //Save gold information
                            SaveGold();
                        }
                        //If the user is using guild storage
                        else
                        {
                            //Set new storage gold plus
                            Character.Network.Guild.StorageGold += gold;
                            //Update player inventory gold minus
                            Character.Information.Gold -= gold;
                            //Send update packet
                            client.Send(Packet.UpdateGold(Character.Information.Gold));
                            client.Send(Packet.GuildGoldUpdate(Character.Network.Guild.StorageGold, 0x20));
                            client.Send(Packet.GuildStorageGold(Character));
                            //Save gold information
                            SaveGold();
                            //Save guild gold
                            SaveGuildGold();
                        }
                    }
                    //If gold is to low
                    else
                    {
                        //Send error message to player
                        client.Send(Packet.IngameMessages(SERVER_UPDATEGOLD, IngameMessages.UIIT_MSG_STRGERR_NOT_ENOUGH_GOLD));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Deposit gold error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            //Save player information
            SavePlayerInfo();
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Move item to storage
        /////////////////////////////////////////////////////////////////////////////////
        void Player_MoveItemToStorage(byte f_slot, byte t_slot, int o_id)
        {
            #region Move item to storage
            try
            {
                //Check if the user isnt in any state/action that we will not allow.
                if (Character.State.Die) return;
                if (Character.Stall.Stallactive) return;
                if (Character.Alchemy.working) return;
                if (Character.Position.Walking) return;
                //Normal storage
                if (!Character.Network.Guild.UsingStorage)
                {
                    //Get item information
                    Global.slotItem item = GetItem((uint)Character.Information.CharacterID, f_slot, 0);
                    //Get storage price of the item
                    int storageprice = Data.ItemBase[item.ID].Storage_price;
                    //Set amount if its only 1
                    if (item.Amount == 0) item.Amount = 1;
                    //Multiply the price, per amount
                    storageprice *= item.Amount;
                    //Anti hack check make sure that the owner of the item is correct
                    int ownerid = (Int32)(MsSQL.GetData("SELECT * FROM char_items WHERE id='" + item.dbID + "'", "owner"));
                    //Check if the owner really owns the item.
                    if (ownerid == Character.Information.CharacterID)
                    {
                        //Make sure the stack count is equal or lower then max stack amount
                        if (item.Amount <= Data.ItemBase[item.ID].Max_Stack)
                        {
                            //If the user has enough gold (Equally or higher to storage price).
                            if (Character.Information.Gold >= storageprice)
                            {
                                //Set character gold
                                Character.Information.Gold -= storageprice;
                                //Send update packet for gold
                                client.Send(Packet.UpdateGold(Character.Information.Gold));
                                //Update mssql database information
                                MsSQL.UpdateData("UPDATE char_items SET itemnumber='item" + t_slot + "',slot='" + t_slot + "',storagetype='1' WHERE storageacc='" + Player.ID + "' AND id='" + item.dbID + "'");
                                //Send visual move packet to player
                                client.Send(Packet.MoveItem(2, f_slot, t_slot, 0, 0, "MOVE_TO_STORAGE"));
                                //Save the player gold
                                SaveGold();
                            }
                            //If the user does not have enough gold
                            else
                            {
                                client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_NOT_ENOUGH_GOLD));
                            }
                        }
                        //This should not happen but if the stack is to high.
                        else
                            return;
                    }
                    //If the player is not the item owner (Hacking attempt).
                    else
                    {
                        Disconnect("ban");
                        Console.WriteLine("Autobanned user: " + Player.AccountName + " Due to hacking");
                    }
                }
                //Guild storage
                else
                {
                    //Get item information
                    Global.slotItem item = GetItem((uint)Character.Information.CharacterID, f_slot, 0);
                    //Get storage price of the item
                    int storageprice = Data.ItemBase[item.ID].Storage_price;
                    //Set amount if its only 1
                    if (item.Amount == 0) item.Amount = 1;
                    //Multiply the price, per amount
                    storageprice *= item.Amount;
                    //Anti hack check make sure that the owner of the item is correct
                    int ownerid = (Int32)(MsSQL.GetData("SELECT * FROM char_items WHERE id='" + item.dbID + "'", "owner"));
                    //Check if the owner really owns the item.
                    if (ownerid == Character.Information.CharacterID)
                    {
                        //Make sure the stack count is equal or lower then max stack amount
                        if (item.Amount <= Data.ItemBase[item.ID].Max_Stack)
                        {
                            //If the user has enough gold (Equally or higher to storage price).
                            if (Character.Information.Gold >= storageprice)
                            {
                                //Set character gold
                                Character.Information.Gold -= storageprice;
                                //Send update packet for gold
                                client.Send(Packet.UpdateGold(Character.Information.Gold));
                                //Update mssql database information
                                MsSQL.UpdateData("UPDATE char_items SET itemnumber='item" + t_slot + "',slot='" + t_slot + "',storagetype='3' ,guild_storage_id='" + Character.Network.Guild.Guildid + "' WHERE id='" + item.dbID + "'");
                                //Send visual move packet to player
                                client.Send(Packet.MoveItem(2, f_slot, t_slot, 0, 0, "MOVE_TO_GUILD_STORAGE"));
                                //Save the player gold
                                SaveGold();
                            }
                            //If the user does not have enough gold
                            else
                            {
                                client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_NOT_ENOUGH_GOLD));
                            }
                        }
                        //This should not happen but if the stack is to high.
                        else
                            return;
                    }
                    //If the player is not the item owner (Hacking attempt).
                    else
                    {
                        Disconnect("ban");
                        Console.WriteLine("Autobanned user: " + Player.AccountName + " Due to hacking");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Storage move error : {0}", ex);
                Systems.Debugger.Write(ex);
            }
            //Save player information
            SavePlayerInfo();
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Move From Storage To Inventory
        /////////////////////////////////////////////////////////////////////////////////
        void Player_MoveStorageItemToInv(byte f_slot, byte t_slot, int o_id)
        {
            #region Move from storage to inv
            try
            {
                //Check if the user isnt in any state/action that we will not allow.
                if (!Character.State.Die || !Character.Stall.Stallactive || !Character.Alchemy.working || !Character.Position.Walking)
                {
                    //Normal storage
                    if (!Character.Network.Guild.UsingStorage)
                    {
                        //Get free slots of player inventory
                        byte freeslot = GetFreeSlot();
                        //Get item information the one that user selected to move.
                        Global.slotItem item = GetItem((uint)Player.ID, f_slot, 1);
                        //Cannot drag and drop onto equip slots.
                        if (t_slot < 13) return;
                        //Update database information
                        MsSQL.UpdateData("UPDATE char_items SET itemnumber='item" + t_slot + "',slot='" + t_slot + "',storagetype='0', owner=" + Character.Information.CharacterID + " WHERE storageacc='" + Player.ID + "' AND id='" + item.dbID + "'");
                        //Send visual packet
                        client.Send(Packet.MoveItem(3, f_slot, t_slot, 0, 0, "MOVE_FROM_STORAGE"));
                    }
                    //Guild storage
                    else
                    {
                        //Get free slots of player inventory
                        byte freeslot = GetFreeSlot();
                        //Get item information the one that user selected to move.
                        Global.slotItem item = GetItem((uint)Player.ID, f_slot, 3);
                        //Cannot drag and drop onto equip slots.
                        if (t_slot < 13) return;
                        //Update database information
                        MsSQL.UpdateData("UPDATE char_items SET itemnumber='item" + t_slot + "',slot='" + t_slot + "',storagetype='0', owner=" + Character.Information.CharacterID + " WHERE guild_storage_id='" + Character.Network.Guild.Guildid + "' AND id='" + item.dbID + "'");
                        //Send visual packet
                        client.Send(Packet.MoveItem(3, f_slot, t_slot, 0, 0, "MOVE_FROM_GUILD_STORAGE"));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Move item from storage error: {0}", ex);
                Systems.Debugger.Write(ex);
            }
            //Save player information
            SavePlayerInfo();
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Move inside Storages
        /////////////////////////////////////////////////////////////////////////////////
        void ItemMoveInStorage(byte fromSlot, byte toSlot, short quantity)
        {
            #region Move Inside Storage
            try
            {
                //Get item information of selected item
                Global.slotItem fromItem = GetItem((uint)Character.Information.CharacterID, fromSlot, 1);
                //Check item where we are moving to
                Global.slotItem toItem = GetItem((uint)Character.Information.CharacterID, toSlot, 1);
                //If the slot where we are moving to has an item

                //Check what type of storage we use.
                int storagetype = 1;
                //Set storage type
                if (Character.Network.Guild.UsingStorage) storagetype = 3;
                if (!Character.Network.Guild.UsingStorage)
                {
                    if (toItem.ID != 0)
                    {
                        //Visual packet
                        client.Send(Packet.MoveItem(0, toSlot, fromSlot, quantity, 0, "MOVE_INSIDE_STORAGE"));
                        //First we update database with the 2 items (From item).
                        MsSQL.UpdateData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE itemnumber='" + "item" + fromSlot + "' AND owner='" + Player.ID + "' AND itemid='" + fromItem.ID + "' AND id='" + fromItem.dbID + "' AND storagetype='" + storagetype + "'");
                        //To item database update
                        MsSQL.UpdateData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE itemnumber='" + "item" + toSlot + "' AND owner='" + Player.ID + "' AND itemid='" + toItem.ID + "' AND id='" + toItem.dbID + "' AND storagetype='" + storagetype + "'");
                    }
                    else
                    {
                        client.Send(Packet.MoveItem(0, fromSlot, toSlot, quantity, 0, "MOVE_INSIDE_STORAGE"));
                        MsSQL.UpdateData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE itemnumber='" + "item" + fromSlot + "' AND owner='" + Player.ID + "' AND itemid='" + fromItem.ID + "' AND storagetype='"+ storagetype +"'");
                    }
                }
                //Guild storage
                else
                {
                    fromItem = GetItem((uint)Character.Information.CharacterID, fromSlot, 3);
                    //Check item where we are moving to
                    toItem = GetItem((uint)Character.Information.CharacterID, toSlot, 3);

                    if (toItem.ID != 0)
                    {
                        //Visual packet
                        client.Send(Packet.MoveItem(0, toSlot, fromSlot, quantity, 0, "MOVE_INSIDE_GUILD_STORAGE"));
                        //First we update database with the 2 items (From item).
                        MsSQL.UpdateData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE itemnumber='" + "item" + fromSlot + "' AND guild_storage_id='" + Character.Network.Guild.Guildid + "' AND itemid='" + fromItem.ID + "' AND id='" + fromItem.dbID + "' AND storagetype='" + storagetype + "'");
                        //To item database update
                        MsSQL.UpdateData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE itemnumber='" + "item" + toSlot + "' AND guild_storage_id='" + Character.Network.Guild.Guildid + "' AND itemid='" + toItem.ID + "' AND id='" + toItem.dbID + "' AND storagetype='" + storagetype + "'");
                        
                    }
                    else
                    {
                        client.Send(Packet.MoveItem(0, fromSlot, toSlot, quantity, 0, "MOVE_INSIDE_GUILD_STORAGE"));
                        MsSQL.UpdateData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE itemnumber='" + "item" + fromSlot + "' AND guild_storage_id='" + Character.Network.Guild.Guildid + "' AND itemid='" + fromItem.ID + "' AND storagetype='"+ storagetype +"'");
                    }
                }
            }
            catch (Exception ex)
            {
               Console.WriteLine("Move inside storage error {0}", ex);
               Systems.Debugger.Write(ex);
            }
            //Save player information
            SavePlayerInfo();
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Move item to exchange slot (Will come when fixing exchange system).
        /////////////////////////////////////////////////////////////////////////////////
        void ItemMoveToExhangePage(byte f_slot)
        {
            #region Move to exchange
            try
            {
                Systems sys = GetPlayer(Character.Network.TargetID);
                if (Character.Network.Exchange.ItemList.Count < 12 && sys.GetFreeSlotMax() > (byte)Character.Network.Exchange.ItemList.Count)
                {

                    Global.slotItem newitem = GetItem((uint)Character.Information.CharacterID, f_slot, 0);

                    LoadBluesid(newitem.dbID);
                    if (newitem.Amount <= Data.ItemBase[newitem.ID].Max_Stack)
                    {
                        Character.Network.Exchange.ItemList.Add(newitem);

                        client.Send(Packet.Exchange_ItemPacket(this.Character.Information.UniqueID, this.Character.Network.Exchange.ItemList, true));
                        sys.Send(Packet.Exchange_ItemPacket(sys.Character.Information.UniqueID, sys.Character.Network.Exchange.ItemList, true));

                        client.Send(Packet.Exchange_ItemSlot(4, f_slot));
                        sys.Send(Packet.Exchange_ItemSlot(4, f_slot));
                    }
                    else
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exchange add item error: {0}", ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Move from exchange to inventory (Will come when fixing exchange system).
        /////////////////////////////////////////////////////////////////////////////////
        void ItemMoveExchangeToInv(byte f_slot)
        {
            #region From exchange to inventory
            Global.slotItem wannadeleteitem = Character.Network.Exchange.ItemList[f_slot];
            Character.Network.Exchange.ItemList.Remove(wannadeleteitem);

            client.Send(Packet.Exchange_ItemPacket(this.Character.Information.UniqueID, this.Character.Network.Exchange.ItemList, true));
            client.Send(Packet.Exchange_ItemSlot(5, f_slot));
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Move exchange gold (Will come when fixing exchange system).
        /////////////////////////////////////////////////////////////////////////////////
        void ItemExchangeGold(long gold)
        {
            #region Move exchange gold
            client.Send(Packet.ItemExchange_Gold(gold));
            Character.Network.Exchange.Gold = gold;
            #endregion
        }
    }
}