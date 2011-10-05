///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Collections.Generic;
namespace SrxRevo
{
    public partial class Systems
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Move Items
        /////////////////////////////////////////////////////////////////////////////////
        void ItemMove(byte fromSlot, byte toSlot, short quantity)
        {
            #region Move items
            try
            {
                //Get from both slots the item information details
                #region Get slot information
                Global.slotItem fromItem = GetItem((uint)Character.Information.CharacterID, fromSlot, 0);
                Global.slotItem toItem = GetItem((uint)Character.Information.CharacterID, toSlot, 0);
                #endregion
                //Define (rename for easy usage).
                #region Redefine slotnames
                fromItem.Slot = fromSlot;
                toItem.Slot = toSlot;
                #endregion
                //Checks
                #region Check slots available
                //If we unequip a item, and place it on a taken slot
                //With this part if unequipping a item, we just use toslot , because it will be changed here.
                if (fromSlot < 13 && toItem.ID != 0)
                {
                    //We get our next free available slot.
                    toSlot = GetFreeSlot();
                    //Though if our free slots is to low, we return.
                    if (toSlot <= 12) return;
                }
                #endregion
                //Job slot handling
                #region Job slot
                //When equipping a job suit, we cannot equip another (Switch them out).
                if (toSlot == 8 && toItem.ID == 0)//So our to slot must be empty
                {
                    //If item is not a  hunter suit but character job is hunter , stop
                    if (Character.Job.type == 1 && Data.ItemBase[fromItem.ID].Type != Global.item_database.ArmorType.HUNTER) return;
                    //If item is not a thief suit but character job is thief , stop
                    if (Character.Job.type == 2 && Data.ItemBase[fromItem.ID].Type != Global.item_database.ArmorType.THIEF) return;
                    //TODO: Find out more about trader job specifications
                    //If no job
                    if (Character.Job.type == 0) return;
                }
                //If we unequip from our job slot
                if (fromSlot == 8 && fromItem.ID != 0)
                {
                    ItemUnEquiped(fromItem);
                }
                #endregion
                //If we equip to slot 7 (Shield / Arrow slot) Completed.
                #region Shield / Arrow slot
                if (toSlot == 7)
                {
                    //We get more information about the items
                    Global.slotItem shieldItem = GetItem((uint)Character.Information.CharacterID, 7, 0);
                    Global.slotItem weaponItem = GetItem((uint)Character.Information.CharacterID, 6, 0);

                    //First we check if our level is high enough.
                    if (!CheckItemLevel(Character.Information.Level, fromItem.ID))
                    {
                        client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_HIGHER_LEVEL_REQUIRED));
                        return;
                    }
                    //Then we check the race the item belongs to EU / CH.
                    else if (!CheckRace(Character.Information.Model, fromItem.ID))
                    {
                        client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_COUNTRY_MISMATCH));
                        return;
                    }
                    //If we allready have a weapon equipped
                    if (weaponItem.ID != 0)
                    {
                        //If we allready have a shield equipped
                        if (shieldItem.ID != 0)
                        {
                            //We compare what item we are going to equip first. (If this is a shield).
                            if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.CH_SHIELD || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_SHIELD)
                            {
                                //Now we check if the weapon we are holding is not two handed, so we dont need to unequip the weapon item first.
                                if (Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.SWORD ||
                                    Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.BLADE ||
                                    Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                                    Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_STAFF)
                                {
                                    #region Done
                                    //Equip new shield
                                    ItemEquiped(fromItem, 7);
                                    //Visually update
                                    GetUpdateSlot(fromItem, 7, 0, 1);
                                    //Now we update the database information of the item to the new slot.                                   
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                                //If the user is holding a two handed weapon.
                                else
                                {
                                    #region Done
                                    byte new_slot = GetFreeSlot();
                                    //We start to unequip the two handed weapon before we equip our weapon.   
                                    ItemUnEquiped(weaponItem);
                                    //Unequip our arrows
                                    ItemUnEquiped(shieldItem);
                                    //Equip new shield
                                    ItemEquiped(fromItem, 7);
                                    //Global info
                                    //Set the amount of arrows globally
                                    Character.Information.Item.sAmount = 0;
                                    Character.Information.Item.sID = 0;
                                    //Visually update
                                    GetUpdateSlot(fromItem, 7, 0, 1);
                                    //Visually update
                                    GetUpdateSlot(weaponItem, new_slot, 0, 1);
                                    //Database update
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + new_slot + "',slot='" + new_slot + "' WHERE id='" + weaponItem.dbID + "'");
                                    //Database update
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    //Database update
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + shieldItem.dbID + "'");
                                    #endregion
                                }
                            }
                            //If the user wants to equip arrows
                            if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BOLT || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.ARROW)
                            {
                                //We firstly check if the user has a bow equiped
                                if (Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.BOW || Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                                {
                                    #region Done
                                    //If the user has a bow equip we begin equipping our arrow / bolt item
                                    GetUpdateSlot(shieldItem, fromItem.Slot, 0, shieldItem.Amount);
                                    //We update the database with the old equipped slot item to new freeslot information
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + shieldItem.dbID + "'");
                                    //We set the item amount to global information
                                    Character.Information.Item.sAmount = fromItem.Amount;
                                    Character.Information.Item.sID = fromItem.ID;
                                    //And finally set new database information for the new equipped item
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                                //If the user wants to equip arrows, and is not holding a bow
                                else
                                {
                                    //Return with no action taken
                                    return;
                                }
                            }
                        }
                        //If the player does not have a shield equipped yet. but has a weapon equiped
                        else
                        {
                            //We check if the player is equipping a new shield.
                            if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.CH_SHIELD || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_SHIELD)
                            {
                                //Then we check if the player weapon type can have a shield or not.
                                if (Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.SWORD || Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.BLADE || Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF || Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_STAFF || Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_SWORD)
                                {
                                    #region Done
                                    //We begin equipping the item
                                    ItemEquiped(fromItem, 7);
                                    //Now we update the remaining information
                                    client.Send(Packet.MoveItem(0, fromSlot, toSlot, quantity, 0, "MOVE_INSIDE_INVENTORY"));
                                    //Then we update the item information in the database with the new slot.
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                                else
                                {
                                    #region Done
                                    //Get free slot for unequiping the weapon
                                    byte new_slot = GetFreeSlot();
                                    //If not enough slots
                                    if (new_slot <= 12) return;
                                    //We start to unequip the two handed weapon before we equip our shield.
                                    ItemUnEquiped(weaponItem);
                                    //Now we update the old slot (or freeslot), to new info.
                                    ItemEquiped(fromItem, 7);
                                    //Update visually the shielditem info
                                    GetUpdateSlot(fromItem, 7, 0, 1);
                                    //Update weapon
                                    GetUpdateSlot(weaponItem, new_slot, 0, 1);
                                    //Now we update the database information of the item to the new slot.                                   
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    //And then we update the old item to the new freeslot information
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + new_slot + "',slot='" + new_slot + "' WHERE id='" + weaponItem.dbID + "'");
                                    #endregion
                                }
                            }
                            //Again if the player wants to equip an arrow type, while holding a weapon
                            if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BOLT || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.ARROW)
                            {
                                //We check if the player has a bow or not.
                                if (Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.BOW || Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                                {
                                    #region Done

                                    //Set the amount of arrows globally
                                    Character.Information.Item.sAmount = fromItem.Amount;
                                    Character.Information.Item.sID = fromItem.ID;
                                    //Now we update the remaining information
                                    GetUpdateSlot(fromItem, 7, 0, fromItem.Amount);
                                    //Update the slot information in database
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                            }
                        }
                    }
                    else
                    //If the user is not holding a weapon
                    {
                        //We check if the user is holding a shield.
                        if (shieldItem.ID != 0)
                        {
                            //If the user has a shield equipped.
                            if (Data.ItemBase[shieldItem.ID].Itemtype == Global.item_database.ItemType.CH_SHIELD || Data.ItemBase[shieldItem.ID].Itemtype == Global.item_database.ItemType.EU_SHIELD)
                            {
                                //We check if the user wants to equip another shield type.
                                if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.CH_SHIELD || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_SHIELD)
                                {
                                    #region Done
                                    //If the weapon is one handed and not a bow related item we start unequipping our shield.
                                    ItemUnEquiped(shieldItem);
                                    //Now we update the old slot (or freeslot), to new info.
                                    ItemEquiped(fromItem, 7);
                                    //Now we update the remaining information
                                    client.Send(Packet.MoveItem(0, fromSlot, 7, quantity, 0, "MOVE_INSIDE_INVENTORY"));
                                    //Now we update the database information of the item to the new slot.                                   
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                    //And then we update the old item to the new freeslot information
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + shieldItem.dbID + "'");
                                    #endregion
                                }
                                else return;
                            }
                            else return;
                        }
                        //If there's no shield or weapon equiped.
                        else
                        {
                            //We check if the user wants to equip a shield
                            if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.CH_SHIELD ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_SHIELD)
                            {
                                #region Done
                                //Now we update the old slot (or freeslot), to new info.
                                ItemEquiped(fromItem, 7);
                                //Now we update the remaining information
                                client.Send(Packet.MoveItem(0, fromSlot, 7, 1, 0, "MOVE_INSIDE_INVENTORY"));
                                //Now we update the database information of the item to the new slot.                                   
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + fromItem.dbID + "'");
                                #endregion
                            }
                            else return;
                        }
                    }
                }
                #endregion
                //Weapon slot Completed.
                #region Weapon slot
                if (toSlot == 6)
                {
                    //Get global item information
                    Global.slotItem shieldItem = GetItem((uint)Character.Information.CharacterID, 7, 0);
                    Global.slotItem weaponItem = GetItem((uint)Character.Information.CharacterID, 6, 0);

                    //First we check if our level is high enough.
                    if (!CheckItemLevel(Character.Information.Level, fromItem.ID))
                    {
                        client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_HIGHER_LEVEL_REQUIRED));
                        return;
                    }
                    //Then we check the race the item belongs to EU / CH.
                    else if (!CheckRace(Character.Information.Model, fromItem.ID))
                    {
                        client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_COUNTRY_MISMATCH));
                        return;
                    }

                    //If the player has a weapon equipped
                    if (weaponItem.ID != 0)
                    {
                        //If the player has a 1 handed weapon equipped
                        if (Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.SWORD || 
                            Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.BLADE || 
                            Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF || 
                            Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_STAFF || 
                            Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_SWORD)
                        {
                            //If the player is holding a shield item
                            if (shieldItem.ID != 0)
                            {
                                //If the shield item is a shield
                                if (Data.ItemBase[shieldItem.ID].Itemtype == Global.item_database.ItemType.CH_SHIELD || Data.ItemBase[shieldItem.ID].Itemtype == Global.item_database.ItemType.EU_SHIELD)
                                {
                                    //if the player wants to equip a bow
                                    if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BOW || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                                    {
                                        #region Done
                                        byte new_slot = GetFreeSlot();
                                        //If freeslots to low
                                        if (new_slot <= 12) return;
                                        //Unequip the current weapon
                                        ItemUnEquiped(weaponItem);
                                        //Equip new bow
                                        ItemEquiped(fromItem, 6);
                                        //Unequip shield
                                        ItemUnEquiped(shieldItem);
                                        //Visually update
                                        GetUpdateSlot(fromItem, 6, 0, 1);
                                        //Check if player has arrows equiped
                                        byte ammoslot = GetAmmoSlot(Character);
                                        //We check if the slot is not empty allready has arrows equipped.
                                        if (ammoslot != 0)
                                        {
                                            //Get the arrow information
                                            Global.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                            //Set the amount of arrows globally
                                            Character.Information.Item.sAmount = AmmoItem.Amount;
                                            Character.Information.Item.sID = AmmoItem.ID;
                                            //Now we update the remaining information
                                            GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                            //Update the slot information in database
                                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                            //Update database
                                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + ammoslot + "',slot='" + ammoslot + "' WHERE id='" + shieldItem.dbID + "'");
                                        }
                                        else
                                        {
                                            //Visual update
                                            GetUpdateSlot(shieldItem, new_slot, 0, 1);
                                            //Update database
                                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + new_slot + "',slot='" + new_slot + "' WHERE id='" + shieldItem.dbID + "'");
                                        }

                                        //Update database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                        //Update database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                        #endregion
                                    }
                                    //If the player wants to equip an other type two handed
                                    #region Done
                                    else if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SPEAR ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.GLAVIE ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_AXE ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_HARP ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSWORD)
                                    {
                                        byte freeslot = GetFreeSlot();
                                        //Unequip the current shield
                                        ItemUnEquiped(shieldItem);
                                        //Unequip the current weapon
                                        ItemUnEquiped(weaponItem);
                                        //Equip new weapon
                                        ItemEquiped(fromItem, 6);
                                        //Visual update weapon slot
                                        GetUpdateSlot(fromItem, 6, 0, 1);
                                        //Visual update shield slot
                                        GetUpdateSlot(shieldItem, freeslot, 0, 1);
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                        //Update database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                        //Update database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + freeslot + "',slot='" + freeslot + "' WHERE id='" + shieldItem.dbID + "'");
                                    }
                                    #endregion
                                    //If player wants to equip 1 handed item, we can keep the shield equiped
                                    else if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SWORD || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BLADE || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_SWORD || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_STAFF || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF)
                                    {
                                        #region Done
                                        //Unequip Weapon
                                        ItemUnEquiped(weaponItem);
                                        //Update visually
                                        GetUpdateSlot(weaponItem, fromItem.Slot, 0, 1);
                                        //Equip new weapon
                                        ItemEquiped(fromItem, 6);
                                        //Update Database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                        //Update database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                        #endregion
                                    }
                                }
                                else
                                    return;
                            }
                            //If no shield is equiped
                            else
                            {
                                //if player wants to equip bow
                                if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BOW || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                                {
                                    #region Done
                                    //Unequip the current weapon
                                    ItemUnEquiped(weaponItem);
                                    //Equip new bow
                                    ItemEquiped(fromItem, 6);
                                    //Visually update
                                    GetUpdateSlot(weaponItem, fromSlot, 0, 1);
                                    //Check if player has arrows equiped
                                    byte ammoslot = GetAmmoSlot(Character);
                                    //We check if the slot is not empty allready has arrows equipped.
                                    if (ammoslot != 0)
                                    {
                                        //Get the arrow information
                                        Global.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                        //Set the amount of arrows globally
                                        Character.Information.Item.sAmount = AmmoItem.Amount;
                                        Character.Information.Item.sID = AmmoItem.ID;
                                        //Now we update the remaining information
                                        GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                        //Update the slot information in database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                    }
                                    //Update database
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                    //Update database
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                                //If player wants to equip other weapon type
                                #region Done
                                if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SPEAR ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.GLAVIE ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SWORD ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BLADE ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_AXE ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_HARP ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSWORD)
                                {
                                    //Unequip our weapon
                                    ItemUnEquiped(weaponItem);
                                    //Equip new weapon
                                    ItemEquiped(fromItem, 6);
                                    //Visually update
                                    GetUpdateSlot(fromItem, 6, 0, 1);
                                    //Update database
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                    //Update database
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                }
                                #endregion
                            }
                        }
                        //If player has a 2 handed weapon equiped
                        else if (Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.GLAVIE ||
                            Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.SPEAR ||
                            Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_TSWORD ||
                            Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                            Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_HARP ||
                            Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                            Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_AXE)
                        {

                            //If player wants to equip a bow
                            if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BOW || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                            {
                                #region Done
                                //Unequip the current weapon
                                ItemUnEquiped(weaponItem);
                                //Equip new bow
                                ItemEquiped(fromItem, 6);
                                //Visually update
                                GetUpdateSlot(weaponItem, fromSlot, 0, 1);
                                //Check if player has arrows equiped
                                byte ammoslot = GetAmmoSlot(Character);
                                //We check if the slot is not empty allready has arrows equipped.
                                if (ammoslot != 0)
                                {
                                    //Get the arrow information
                                    Global.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                    //Set the amount of arrows globally
                                    Character.Information.Item.sAmount = AmmoItem.Amount;
                                    Character.Information.Item.sID = AmmoItem.ID;
                                    //Now we update the remaining information
                                    GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                    //Update the slot information in database
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                }
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                #endregion
                            }
                            //If player wants to equip other weapon types
                            #region Done
                            else if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SPEAR ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.GLAVIE ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SWORD ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BLADE ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_AXE ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_HARP ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSWORD)
                            {
                                //Unequip our weapon
                                ItemUnEquiped(weaponItem);
                                //Equip new weapon
                                ItemEquiped(fromItem, 6);
                                //Visually update
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            #endregion
                        }
                        //If player has a bow equiped
                        else if (Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.BOW || Data.ItemBase[weaponItem.ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                        {
                            //If arrows are equiped
                            if (shieldItem.ID != 0)
                            {
                                //Check to make sure its a arrow
                                if (Data.ItemBase[shieldItem.ID].Itemtype == Global.item_database.ItemType.ARROW || Data.ItemBase[shieldItem.ID].Itemtype == Global.item_database.ItemType.BOLT)
                                {
                                    //Check what we are equipping
                                    #region Done
                                    if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SPEAR ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.GLAVIE ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SWORD ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BLADE ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_AXE ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_HARP ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                                        Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSWORD)
                                    {
                                        //Get free slot to move to
                                        byte freeslot = GetFreeSlot();
                                        //If slots free is to low
                                        if (freeslot <= 12) return;
                                        //Unequip our bow
                                        ItemUnEquiped(weaponItem);
                                        //Unequip arrow
                                        ItemUnEquiped(shieldItem);
                                        //Equip new weapon
                                        ItemEquiped(fromItem, 6);
                                        //Visual update
                                        GetUpdateSlot(weaponItem, fromItem.Slot, 0, 1);
                                        //Visual update
                                        GetUpdateSlot(shieldItem, freeslot, 0, 1);
                                        //Set global info
                                        Character.Information.Item.sAmount = 0;
                                        Character.Information.Item.sID = 0;
                                        //Update Database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                        //Update Database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + freeslot + "',slot='" + freeslot + "' WHERE id='" + shieldItem.dbID + "'");
                                        //Update Database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                    }
                                    #endregion
                                    //If player wants to equip another bow
                                    #region Done
                                    else if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BOW || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                                    {
                                        //Unequip current bow
                                        ItemUnEquiped(weaponItem);
                                        //Equip new bow
                                        ItemEquiped(fromItem, 6);
                                        //Visually update
                                        GetUpdateSlot(fromItem, 6, 0, 1);
                                        //Update database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                        //Update database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                    }
                                    #endregion
                                }
                            }
                            //If no arrow item is equiped
                            else
                            {
                                //If player wants to equip another bow
                                if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BOW || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                                {
                                    #region Done
                                    //Unequip weapon
                                    ItemUnEquiped(weaponItem);
                                    //Equip new bow
                                    ItemEquiped(fromItem, 6);
                                    //Visualy update
                                    GetUpdateSlot(fromItem, 6, 0, 1);
                                    //Check if user has arrows
                                    byte ammoslot = GetAmmoSlot(Character);
                                    //If the user has arrows
                                    if (ammoslot != 0)
                                    {
                                        //Get the arrow information
                                        Global.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                        //Set the amount of arrows globally
                                        Character.Information.Item.sAmount = AmmoItem.Amount;
                                        Character.Information.Item.sID = AmmoItem.ID;
                                        //Now we update the remaining information
                                        GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                        //Update the slot information in database
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                    }
                                    //Update database info
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                    //Update database info
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                    #endregion
                                }
                                //If player wants to equip another weapon type
                                #region Done
                                if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SPEAR ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.GLAVIE ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SWORD ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BLADE ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_AXE ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_HARP ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSWORD)
                                {
                                    //Unequip our bow
                                    ItemUnEquiped(weaponItem);
                                    //Equip new weapon
                                    ItemEquiped(fromItem, 6);
                                    //Visual update
                                    GetUpdateSlot(fromItem, 6, 0, 1);
                                    //Update Database
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + weaponItem.dbID + "'");
                                    //Update Database
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                }
                                #endregion
                            }
                        }
                    }
                    //If no weapon is equiped
                    else
                    {
                        //If shield has been equipped.
                        if (shieldItem.ID != 0)
                        {
                            //If player wants to equip a bow
                            if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BOW || 
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                            {
                                #region Done
                                //Equip the bow
                                ItemEquiped(fromItem, 6);
                                //Unequip shield item
                                ItemUnEquiped(shieldItem);
                                //Send visual equip bow
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Get arrows in inventory
                                byte ammoslot = GetAmmoSlot(Character);
                                //If player has arrows
                                if (ammoslot != 0)
                                {
                                    //Get the arrow information
                                    Global.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                    //Set the amount of arrows globally
                                    Character.Information.Item.sAmount = AmmoItem.Amount;
                                    Character.Information.Item.sID = AmmoItem.ID;
                                    //Now we update the remaining information
                                    GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                    //Update the slot information in database
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                    //Update in database
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + ammoslot + "',slot='" + ammoslot + "' WHERE id='" + shieldItem.dbID + "'");
                                }
                                else
                                {
                                    GetUpdateSlot(shieldItem, fromSlot, 0, 1);
                                    //Update in database
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + shieldItem.dbID + "'");
                                }
                                //Update in database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                #endregion
                            }
                            //If 2 handed weapon
                            #region Done
                            else if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SPEAR ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.GLAVIE ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_AXE ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_HARP ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                                    Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSWORD)
                            {
                                //Unequip old item
                                ItemUnEquiped(shieldItem);
                                //Equip the item
                                ItemEquiped(fromItem, 6);
                                //Visual update
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Visual update
                                GetUpdateSlot(shieldItem, fromSlot, 0, 1);
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + shieldItem.dbID + "'");
                                //Update database information
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            #endregion
                            //If one handed weapon
                            #region Done
                            else if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SWORD ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BLADE ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_SWORD)
                            {
                                //Equip the item
                                ItemEquiped(fromItem, 6);
                                //Visually update the inventory
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Update database information
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            #endregion
                        }
                        //Nothing equipped
                        else
                        {
                            //If player wants to equip bow
                            #region Done
                            if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BOW || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                            {

                                //Equip the bow
                                ItemEquiped(fromItem, 6);
                                //Visually update
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Updat database information
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                                //Check if player has arrows in equipment
                                byte ammoslot = GetAmmoSlot(Character);
                                //If the player has arrows
                                if (ammoslot != 0)
                                {
                                    //Get arrow information
                                    Global.slotItem AmmoItem = GetItem((uint)Character.Information.CharacterID, ammoslot, 0);
                                    //Equip the arrows
                                    ItemEquiped(fromItem, 7);
                                    //Set global information                                 
                                    Character.Information.Item.sAmount = AmmoItem.Amount;
                                    Character.Information.Item.sID = AmmoItem.ID;
                                    //Visually show changes
                                    GetUpdateSlot(AmmoItem, 7, 0, AmmoItem.Amount);
                                    //Update database information
                                    MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE id='" + AmmoItem.dbID + "'");
                                }
                            }
                            #endregion
                            //If player wants to equip other weapons
                            #region Equip other items (done)
                            else if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SPEAR ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.GLAVIE ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.SWORD ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BLADE ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_AXE ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_HARP ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                                Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.EU_TSWORD)
                            {
                                //Equip the item
                                ItemEquiped(fromItem, 6);
                                //Show visual changes
                                GetUpdateSlot(fromItem, 6, 0, 1);
                                //Update database information
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 6 + "',slot='" + 6 + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            #endregion
                        }
                    }
                }
                #endregion
                //Clothing items (Armor / Jewelry).
                #region Clothing items
                //Check if our item is a none weapon type and clothing type
                if (Data.ItemBase[fromItem.ID].Class_D == 1 && toSlot != 6 && toSlot != 7 && toSlot < 13)
                {

                    if (toSlot < 13)
                    {
                        //First we check if our level is high enough.
                        if (!CheckItemLevel(Character.Information.Level, fromItem.ID))
                        {
                            client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_HIGHER_LEVEL_REQUIRED));
                            return;
                        }
                        //Then we check if the gender is the same as ours.
                        else if (!CheckGender(Character.Information.Model, fromItem.ID))
                        {
                            client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_GENDER_MISMATCH));
                            return;
                        }
                        //Then we check the race the item belongs to EU / CH.
                        else if (!CheckRace(Character.Information.Model, fromItem.ID))
                        {
                            client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_COUNTRY_MISMATCH));
                            return;
                        }
                        //Then we check if armor type equals the current equipped ones.
                        else if (!CheckArmorType(fromItem.ID, Character.Information.CharacterID))
                        {
                            client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UITT_MSG_CUSTOM_ARMOR_TYPE_WRONG));
                            return;
                        }
                        //All checks ok we equip the item
                        else
                        {
                            if (toItem.ID != 0)
                            {
                                //Unequip item
                                ItemUnEquiped(toItem);
                                //Equip the new item
                                ItemEquiped(fromItem, toSlot);
                                //Update visual
                                GetUpdateSlot(fromItem, toSlot, 0, 1);
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                            }
                            else
                            {
                                //Equip the new item
                                ItemEquiped(fromItem, toSlot);
                                GetUpdateSlot(fromItem, toSlot, 0, 1);
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                        }
                    }
                    else if (fromSlot < 13)
                    {
                        if (toItem.ID != 0)
                        {
                            byte new_slot = GetFreeSlot();
                            ItemUnEquiped(fromItem);
                            GetUpdateSlot(fromItem, new_slot, 0, 1);
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + new_slot + "',slot='" + new_slot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                        else
                        {
                            ItemUnEquiped(fromItem);
                            GetUpdateSlot(fromItem, toSlot, 0, 1);
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                    }
                    else
                    {
                        if (toItem.ID != 0)
                        {
                            byte new_slot = GetFreeSlot();
                            ItemUnEquiped(fromItem);
                            GetUpdateSlot(fromItem, new_slot, 0, 1);
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + new_slot + "',slot='" + new_slot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                        else
                        {
                            ItemUnEquiped(fromItem);
                            GetUpdateSlot(fromItem, toSlot, 0, 1);
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                    }
                }
                #endregion
                //Normal item movement non equpping inside inventory
                #region Normal inventory movement
                short newquantity = 0;
                short fromquantity = 0;
                if (fromSlot >= 13 && toSlot >= 13)
                {
                    if (toItem.ID != 0)
                    {
                        if (toItem.ID == fromItem.ID)
                        {
                            if (Data.ItemBase[fromItem.ID].Class_D == 3 && Data.ItemBase[toItem.ID].Class_D == 3)
                            {
                                if (Data.ItemBase[fromItem.ID].Max_Stack > 1)
                                {
                                    newquantity = (short)(fromItem.Amount + toItem.Amount);
                                    if (newquantity > Data.ItemBase[fromItem.ID].Max_Stack)
                                    {
                                        GetUpdateSlot(fromItem, toSlot, 0, fromItem.Amount);
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                                        MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                                    }
                                    else if (newquantity <= Data.ItemBase[fromItem.ID].Max_Stack)
                                    {
                                        MsSQL.InsertData("delete from char_items where id='" + fromItem.dbID + "'");
                                        MsSQL.InsertData("UPDATE char_items SET quantity='" + newquantity + "' WHERE id='" + toItem.dbID + "'");
                                        GetUpdateSlot(fromItem, toSlot, 0, newquantity);
                                    }
                                    else
                                    {
                                        fromquantity = (short)(newquantity % Data.ItemBase[fromItem.ID].Max_Stack);
                                        newquantity -= fromquantity;
                                        MsSQL.InsertData("UPDATE char_items SET quantity='" + fromquantity + "' WHERE id='" + fromItem.dbID + "'");
                                        MsSQL.InsertData("UPDATE char_items SET quantity='" + newquantity + "' WHERE id='" + toItem.dbID + "'");
                                    }
                                }
                            }
                            else
                            {
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                            }
                        }
                        else
                        {
                            GetUpdateSlot(fromItem, toSlot, 0, quantity);
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                        }
                    }
                    else
                    {
                        if (fromItem.Amount != quantity && Data.ItemBase[fromItem.ID].Class_D == 3)
                        {
                            AddItem(fromItem.ID, quantity, toSlot, Character.Information.CharacterID, 0);
                            int calc = (fromItem.Amount - quantity);
                            if (calc < 1) calc = 1;
                            GetUpdateSlot(fromItem, toSlot,0, quantity);

                            MsSQL.InsertData("UPDATE char_items SET quantity='" + calc + "' WHERE id='" + fromItem.dbID + "'");
                        }
                        else if (toItem.ID != 0)
                        {

                            GetUpdateSlot(fromItem, toSlot, 0, quantity);
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + fromSlot + "',slot='" + fromSlot + "' WHERE id='" + toItem.dbID + "'");
                        }
                        else
                        {
                            GetUpdateSlot(fromItem, toSlot, 0, quantity);
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                    }
                }
                #endregion
                //Unequip from slot
                #region Unequip items
                if (fromSlot < 13 && toSlot > 13)
                {
                    #region From slot 6
                    Global.slotItem weaponitem = GetItem((uint)Character.Information.CharacterID, 6, 0);
                    Global.slotItem shieldslotitem = GetItem((uint)Character.Information.CharacterID, 7, 0);
                    //If we unequip a bow or other item
                    if (fromSlot == 6 && shieldslotitem.ID != 0)
                    {
                        //if we unequip a bow
                        if (Data.ItemBase[weaponitem.ID].Itemtype == Global.item_database.ItemType.BOW || Data.ItemBase[weaponitem.ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW)
                        {
                            //If we drag it to a none free slot
                            if (toItem.ID != 0)
                            {
                                //Free arrow slot
                                byte freeweaponslot = GetFreeSlot();
                                //Unequip the arrows
                                ItemUnEquiped(shieldslotitem);
                                //Update visually arrow
                                GetUpdateSlot(shieldslotitem, freeweaponslot, 0, quantity);
                                //Update database bow
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + freeweaponslot + "',slot='" + freeweaponslot + "' WHERE id='" + shieldslotitem.dbID + "'");
                                //Free weapon slot
                                byte freearrowslot = GetFreeSlot();
                                //Unequip the weapon
                                ItemUnEquiped(fromItem);
                                //Update visually bow
                                GetUpdateSlot(fromItem, freearrowslot, 0, 1);
                                //Update global information
                                Character.Information.Item.sAmount = 0;
                                Character.Information.Item.sID = 0;
                                //Update database arrow
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + freearrowslot + "',slot='" + freearrowslot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            //If we drag to free slot
                            else
                            {
                                //Unequip the arrow
                                ItemUnEquiped(fromItem);
                                //Unequip the weapon
                                ItemUnEquiped(shieldslotitem);
                                //Update visually bow
                                GetUpdateSlot(fromItem, toSlot, 0, 1);
                                //Update bow database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                                //Free weapon slot
                                byte newslot = GetFreeSlot();
                                //Update visually arrows
                                GetUpdateSlot(shieldslotitem, newslot, 0, quantity);
                                //Update global information
                                Character.Information.Item.sAmount = 0;
                                Character.Information.Item.sID = 0;
                                //Update arrow in database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + newslot + "',slot='" + newslot + "' WHERE id='" + shieldslotitem.dbID + "'");
                            }
                        }
                        //If we unequip another weapon type and shield is equiped we keep the shield.
                        else
                        {
                            if (toItem.ID != 0)
                            {
                                //Free weapon slot
                                byte weaponslot = GetFreeSlot();
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, weaponslot, 0, 1);
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + weaponslot + "',slot='" + weaponslot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            else
                            {
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, toSlot, 0, 1);
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                        }
                    }
                    //If no shield item has been equiped
                    else if (fromSlot == 6 && shieldslotitem.ID == 0)
                    {
                        if (toItem.ID != 0)
                        {
                            //Free weapon slot
                            byte weaponslot = GetFreeSlot();
                            //Unequip our weapon
                            ItemUnEquiped(fromItem);
                            //Update visually
                            GetUpdateSlot(fromItem, weaponslot, 0, 1);
                            //Update database
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + weaponslot + "',slot='" + weaponslot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                        else
                        {
                            ItemUnEquiped(fromItem);
                            //Update visually
                            GetUpdateSlot(fromItem, toSlot, 0, 1);
                            //Update database
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                    }
                    #endregion
                    //It we unequip a shield or arrows
                    #region From slot 7
                    else if (fromSlot == 7)
                    {
                        //if we unequip arrows
                        if (Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.ARROW || Data.ItemBase[fromItem.ID].Itemtype == Global.item_database.ItemType.BOLT)
                        {
                            if (toItem.ID != 0)
                            {
                                //Free arrow slot
                                byte arrowslot = GetFreeSlot();
                                //Unequip our arrows
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, arrowslot, 0, quantity);
                                //Set global data
                                Character.Information.Item.sAmount = 0;
                                Character.Information.Item.sID = 0;
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + arrowslot + "',slot='" + arrowslot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            else
                            {
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, toSlot, 0, quantity);
                                //Set global data
                                Character.Information.Item.sAmount = 0;
                                Character.Information.Item.sID = 0;
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                        }
                        //If we unequip shields
                        else
                        {
                            if (toItem.ID != 0)
                            {
                                //Free weapon slot
                                byte newshieldslot = GetFreeSlot();
                                //Unequip our weapon
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, newshieldslot, 0, 1);
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + newshieldslot + "',slot='" + newshieldslot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                            else
                            {
                                ItemUnEquiped(fromItem);
                                //Update visually
                                GetUpdateSlot(fromItem, toSlot, 0, 1);
                                //Update database
                                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                            }
                        }
                    }
                    #endregion
                    //Other slots
                    #region Other from slots
                    else
                    {
                        if (toItem.ID != 0)
                        {
                            //Free weapon slot
                            byte newunequipslot = GetFreeSlot();
                            //Unequip our weapon
                            ItemUnEquiped(fromItem);
                            //Update visually
                            GetUpdateSlot(fromItem, newunequipslot, 0, 1);
                            //Update database
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + newunequipslot + "',slot='" + newunequipslot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                        else
                        {
                            ItemUnEquiped(fromItem);
                            //Update visually
                            GetUpdateSlot(fromItem, toSlot, 0, 1);
                            //Update database
                            MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "' WHERE id='" + fromItem.dbID + "'");
                        }
                    }
                    #endregion
                }
                #endregion
                //Save player information
                SavePlayerInfo();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Unequip Items
        /////////////////////////////////////////////////////////////////////////////////
        public void ItemUnEquiped(Global.slotItem item)
        {
            #region Unequip items
            LoadBluesid(item.dbID);
            try
            {
                if (item.Slot <= 5)
                {
                    Character.Stat.MagDef -= Data.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.MagDefINC);
                    Character.Stat.PhyDef -= Data.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.PhyDefINC);
                    Character.Stat.Parry -= Data.ItemBase[item.ID].Defans.Parry;
                    if (Data.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, false);
                    client.Send(Packet.PlayerStat(Character));
                }
                if (item.Slot == 6)
                {
                    Character.Stat.MinPhyAttack -= Data.ItemBase[item.ID].Attack.Min_LPhyAttack + (item.PlusValue * (double)Data.ItemBase[item.ID].Attack.PhyAttackInc);
                    Character.Stat.MaxPhyAttack -= Data.ItemBase[item.ID].Attack.Min_HPhyAttack + (item.PlusValue * (double)Data.ItemBase[item.ID].Attack.PhyAttackInc);
                    Character.Stat.MinMagAttack -= Data.ItemBase[item.ID].Attack.Min_LMagAttack + (item.PlusValue * (double)Data.ItemBase[item.ID].Attack.MagAttackINC);
                    Character.Stat.MaxMagAttack -= Data.ItemBase[item.ID].Attack.Min_HMagAttack + (item.PlusValue * (double)Data.ItemBase[item.ID].Attack.MagAttackINC);
                    Character.Stat.Hit -= Data.ItemBase[item.ID].Attack.MinAttackRating;
                    if (Data.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, false);
                    client.Send(Packet.PlayerStat(Character));
                    Character.Information.Item.wID = 0;
                }
                if (item.Slot == 7)
                {
                    if (Data.ItemBase[item.ID].Itemtype == Global.item_database.ItemType.CH_SHIELD || Data.ItemBase[item.ID].Itemtype == Global.item_database.ItemType.EU_SHIELD)
                    {
                        Character.Stat.MagDef -= Data.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.MagDefINC);
                        Character.Stat.PhyDef -= Data.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.PhyDefINC);
                        if (Data.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, false);
                    }
                    client.Send(Packet.PlayerStat(Character));
                    Character.Information.Item.sAmount = 0;
                    Character.Information.Item.sID = 0;
                }
                if (item.Slot == 8)
                {
                    if (Character.Job.Jobname != "0" && Character.Job.state == 1)
                    {
                        //Teleport user back to binded location
                        Send(Packet.StatePack(Character.Information.UniqueID, 0x0B, 0x01, false));
                        Character.Information.Scroll = true;
                        StartScrollTimer(0);
                        MsSQL.UpdateData("UPDATE character_jobs SET job_state='0' WHERE character_name='" + Character.Information.Name + "'");
                        Character.Job.state = 0;
                        SavePlayerReturn();
                    }
                }
                if (item.Slot >= 9 && item.Slot <= 12)
                {
                    Character.Stat.MagDef -= (short)(Data.ItemBase[item.ID].Defans.MagAbsorb + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.AbsorbINC) * 10);
                    Character.Stat.PhyDef -= (short)(Data.ItemBase[item.ID].Defans.PhyAbsorb + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.AbsorbINC) * 10);
                    client.Send(Packet.PlayerStat(Character));
                    if (Data.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, false);
                }
                Send(Packet.MoveItemUnequipEffect(Character.Information.UniqueID, item.Slot, item.ID));
                SavePlayerInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Equiped items
        /////////////////////////////////////////////////////////////////////////////////
        public void ItemEquiped(Global.slotItem item, byte slot)
        {
            #region Equiped items
            try
            {       
                if (Character.Information.Level >= Data.ItemBase[item.ID].Level)
                {
                    if (slot <= 5)
                    {
                        Character.Stat.MagDef += Data.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.MagDefINC);
                        Character.Stat.PhyDef += Data.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.PhyDefINC);
                        Character.Stat.Parry += Data.ItemBase[item.ID].Defans.Parry;
                        if (Data.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, true);
                        client.Send(Packet.PlayerStat(Character));
                    }
                    if (slot == 6)
                    {
                        Character.Stat.MinPhyAttack += Data.ItemBase[item.ID].Attack.Min_LPhyAttack + (item.PlusValue * (double)Data.ItemBase[item.ID].Attack.PhyAttackInc);
                        Character.Stat.MaxPhyAttack += Data.ItemBase[item.ID].Attack.Min_HPhyAttack + (item.PlusValue * (double)Data.ItemBase[item.ID].Attack.PhyAttackInc);
                        Character.Stat.MinMagAttack += Data.ItemBase[item.ID].Attack.Min_LMagAttack + (item.PlusValue * (double)Data.ItemBase[item.ID].Attack.MagAttackINC);
                        Character.Stat.MaxMagAttack += Data.ItemBase[item.ID].Attack.Min_HMagAttack + (item.PlusValue * (double)Data.ItemBase[item.ID].Attack.MagAttackINC);
                        Character.Stat.Hit += Data.ItemBase[item.ID].Attack.MinAttackRating;
                        Character.Information.Item.wID = item.ID;
                        if (Data.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, true);
                        client.Send(Packet.PlayerStat(Character));
                    }
                    if (slot == 7)
                    {
                        if (Data.ItemBase[item.ID].Itemtype == Global.item_database.ItemType.CH_SHIELD || Data.ItemBase[item.ID].Itemtype == Global.item_database.ItemType.EU_SHIELD)
                        {
                            Character.Stat.MagDef += Data.ItemBase[item.ID].Defans.MinMagDef + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.MagDefINC);
                            Character.Stat.PhyDef += Data.ItemBase[item.ID].Defans.MinPhyDef + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.PhyDefINC);
                            if (Data.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, true);
                        }
                        Character.Information.Item.sAmount = item.Amount;
                        Character.Information.Item.sID = item.ID;
                        client.Send(Packet.PlayerStat(Character));
                    }
                    if (slot == 8)
                    {
                        //Teleport user back to binded location
                        Send(Packet.StatePack(Character.Information.UniqueID, 0x0B, 0x01, false));
                        Character.Information.Scroll = true;
                        StartScrollTimer(0);//Set to job time info
                        MsSQL.UpdateData("UPDATE character_jobs SET job_state='1' WHERE character_name='" + Character.Information.Name + "'");
                        Character.Job.state = 1;
                        SavePlayerReturn();
                    }
                    if (slot >= 9 && slot <= 12)
                    {
                        Character.Stat.MagDef += (short)(Data.ItemBase[item.ID].Defans.MagAbsorb + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.AbsorbINC) * 10);
                        Character.Stat.PhyDef += (short)(Data.ItemBase[item.ID].Defans.PhyAbsorb + (item.PlusValue * (double)Data.ItemBase[item.ID].Defans.AbsorbINC) * 10);
                        client.Send(Packet.PlayerStat(Character));
                        if (Data.ItemBlue[item.dbID].totalblue != 0) AddRemoveBlues(this, item, true);
                    }
                    Send(Packet.MoveItemEnquipEffect(Character.Information.UniqueID, item.Slot, item.ID, item.PlusValue));
                    SavePlayerInfo();
                }
                else
                    return;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Item equip error: {0}", ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Avatar Unequiped
        /////////////////////////////////////////////////////////////////////////////////
        void ItemAvatarUnEquip(byte fromSlot, byte toSlot)
        {
            #region Avatar unequiped
            try
            {
                GetFreeSlot();
                Global.slotItem toItem = GetItem((uint)Character.Information.CharacterID, toSlot, 0);
                int avatarid = 0;
                int dbID = 0;

                if (toItem.ID != 0) toSlot = GetFreeSlot();
                if (toSlot <= 12) return;

                MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE itemnumber='avatar" + fromSlot + "' AND owner='" + Character.Information.CharacterID + "' AND inAvatar='1'");
                using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        avatarid = reader.GetInt32(2);
                        dbID = reader.GetInt32(0);
                    }
                }
                ms.Close();

                client.Send(Packet.MoveItem(35, fromSlot, toSlot, 1,0,"MOVE_INSIDE_INVENTORY"));
                Send(Packet.MoveItemUnequipEffect(Character.Information.UniqueID, fromSlot, avatarid));

                string nonquery = "UPDATE char_items SET itemnumber='item" + toSlot + "',slot='" + toSlot + "',inAvatar='0' WHERE owner='" + Character.Information.CharacterID + "' AND itemnumber='avatar" + fromSlot + "' AND id='" + dbID + "'";
                MsSQL.InsertData(nonquery);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Avatar unequiped error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Avatar Equiped
        /////////////////////////////////////////////////////////////////////////////////
        void ItemAvatarEquip(byte fromSlot, byte toSlot)
        {
            #region Avatar equiped
            try
            {
                GetFreeSlot();
                Global.slotItem toItem = new SrxRevo.Global.slotItem();
                Global.slotItem fromItem = GetItem((uint)Character.Information.CharacterID, fromSlot, 0);

                MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE itemnumber='avatar" + toSlot + "' AND owner='" + Character.Information.CharacterID + "'");
                using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                    {
                        toItem.ID = reader.GetInt32(2);
                        toItem.dbID = reader.GetInt32(0);
                    }
                }
                ms.Close();

                if (toItem.ID != 0) return;

                if (fromItem.ID == 0) return;
                if (!CheckGender(Character.Information.Model, fromItem.ID))
                {
                    return;
                }

                else
                {
                    string nonquery = "UPDATE char_items SET itemnumber='avatar" + toSlot + "',inAvatar='1',slot='" + toSlot + "' WHERE owner='" + Character.Information.CharacterID + "' AND itemnumber='item" + fromSlot + "' AND id='" + fromItem.dbID + "'";
                    MsSQL.InsertData(nonquery);
                }
                Send(Packet.MoveItemEnquipEffect(Character.Information.UniqueID, toSlot, fromItem.ID, 0));
                client.Send(Packet.MoveItem(36, fromSlot, toSlot, 1, 0, "MOVE_INSIDE_INVENTORY"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Avatar equiped error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Drop Item
        /////////////////////////////////////////////////////////////////////////////////
        void Player_DropItem(byte slot)
        {
            #region Drop Item
            try
            {
                if (Character.Action.nAttack) return; 
                if (Character.Action.sAttack) return; 
                if (Character.Stall.Stallactive) return;
                if (Character.State.Exchanging) return;
                if (Character.Alchemy.working) return;
                if (Character.State.Busy) return;
                if (Character.Network.Guild.UsingStorage) return;
                //Else we continue
                else
                {
                    //Get item information from slot.
                    Global.slotItem item = GetItem((uint)Character.Information.CharacterID, slot, 0);
                    //Check if the item is a item mall item before dropping.
                    if (Data.ItemBase[item.ID].Etctype == Global.item_database.EtcType.AVATAR28D ||
                        Data.ItemBase[item.ID].Etctype == Global.item_database.EtcType.CHANGESKIN ||
                        Data.ItemBase[item.ID].Etctype == Global.item_database.EtcType.GLOBALCHAT ||
                        Data.ItemBase[item.ID].Etctype == Global.item_database.EtcType.INVENTORYEXPANSION ||
                        Data.ItemBase[item.ID].Etctype == Global.item_database.EtcType.REVERSESCROLL ||
                        Data.ItemBase[item.ID].Etctype == Global.item_database.EtcType.STALLDECORATION ||
                        Data.ItemBase[item.ID].Etctype == Global.item_database.EtcType.WAREHOUSE ||
                        Data.ItemBase[item.ID].Etctype == Global.item_database.EtcType.AVATAR28D ||
                        Data.ItemBase[item.ID].Type == Global.item_database.ArmorType.AVATAR ||
                        Data.ItemBase[item.ID].Type == Global.item_database.ArmorType.AVATARATTACH ||
                        Data.ItemBase[item.ID].Type == Global.item_database.ArmorType.AVATARHAT ||
                        Data.ItemBase[item.ID].Pettype == Global.item_database.PetType.ATTACKPET ||
                        Data.ItemBase[item.ID].Pettype == Global.item_database.PetType.GRABPET)
                        return;
                    //If the id model is lower then 4 dont allow to drop.
                    //Gold drop requires another drop part.
                    if (item.ID < 4) return;
                    //Anti hack check
                    int owner = MsSQL.GetDataInt("SELECT * FROM char_items WHERE id='" + item.dbID + "'", "owner");
                    //If the player really is the owner of this item we continue to drop it.
                    if (owner == Character.Information.CharacterID)
                    {
                        //Check for item amount.
                        if (item.Amount <= Data.ItemBase[item.ID].Max_Stack)
                        {
                            //Spawn new item globally
                            world_item sitem = new world_item();
                            sitem.Model = item.ID;
                            sitem.Ids = new Global.ID(Global.ID.IDS.World);
                            sitem.UniqueID = sitem.Ids.GetUniqueID;
                            sitem.amount = item.Amount;
                            sitem.PlusValue = item.PlusValue;
                            sitem.x = Character.Position.x;
                            sitem.z = Character.Position.z;
                            sitem.y = Character.Position.y;
                            Systems.aRound(ref sitem.x, ref sitem.y, 1);
                            sitem.xSec = Character.Position.xSec;
                            sitem.ySec = Character.Position.ySec;
                            //Todo sniff structure for ticket / jewelry box item drop
                            if (Data.ItemBase[sitem.Model].Etctype == Global.item_database.EtcType.EVENT)
                                sitem.Type = 4;
                            #region Amount definition
                            else if (Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.BLADE ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.CH_SHIELD ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.EU_SHIELD ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.BOW ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.EU_AXE ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.EU_CROSSBOW ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.EU_HARP ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.EU_TSWORD ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.GLAVIE ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.SPEAR ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.SWORD ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.EARRING ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.RING ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.NECKLACE ||
                                 Data.ItemBase[sitem.Model].Type == Global.item_database.ArmorType.ARMOR ||
                                 Data.ItemBase[sitem.Model].Type == Global.item_database.ArmorType.GARMENT ||
                                 Data.ItemBase[sitem.Model].Type == Global.item_database.ArmorType.GM ||
                                 Data.ItemBase[sitem.Model].Type == Global.item_database.ArmorType.HEAVY ||
                                 Data.ItemBase[sitem.Model].Type == Global.item_database.ArmorType.LIGHT ||
                                 Data.ItemBase[sitem.Model].Type == Global.item_database.ArmorType.PROTECTOR ||
                                 Data.ItemBase[sitem.Model].Itemtype == Global.item_database.ItemType.AVATAR ||
                                 Data.ItemBase[sitem.Model].Type == Global.item_database.ArmorType.ROBE)
                                sitem.Type = 2;
                            else
                                sitem.Type = 3;
                            #endregion

                            sitem.fromType = 6;
                            sitem.fromOwner = Character.Information.UniqueID;
                            sitem.downType = false;
                            sitem.Owner = 0;
                            sitem.Send(Packet.ObjectSpawn(sitem), true);
                            Systems.WorldItem.Add(sitem);
                            //Send visual packet for removing the item from inventory.
                            client.Send(Packet.DespawnFromInventory(sitem.UniqueID));
                            client.Send(Packet.MoveItem(7, slot, 0, 0, 0, "DELETE_ITEM"));
                            //Update database and remove the item
                            MsSQL.UpdateData("delete from char_items where itemnumber='item" + slot + "' AND owner='" + Character.Information.CharacterID + "'");
                            //Save player information
                            SavePlayerInfo();
                        }
                        else
                            return;
                    }
                    //If the player is not the owner of the item beeing dropped we ban the player.
                    else
                    {
                        Disconnect("ban");
                        Console.WriteLine("Autobanned user: " + Player.AccountName + " Due to hacking");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Player_DropItem Error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Drop Gold
        /////////////////////////////////////////////////////////////////////////////////
        void Player_DropGold(ulong Gold)
        {
            #region Drop Gold
            try
            {
                if (Character.Action.nAttack) return;
                if (Character.Action.sAttack) return;
                if (Character.Stall.Stallactive) return;
                if (Character.State.Exchanging) return;
                if (Character.Alchemy.working) return;
                if ((ulong)Character.Information.Gold >= Gold)
                {
                    GetFreeSlot();
                    world_item item = new world_item();
                    item.amount = (int)Gold;
                    item.Model = 1;
                    if (item.amount < 1000) item.Model = 1;
                    else if (item.amount > 1000 && item.amount < 10000) item.Model = 2;
                    else if (item.amount > 10000) item.Model = 3;
                    item.Ids = new Global.ID(Global.ID.IDS.World);
                    item.UniqueID = item.Ids.GetUniqueID;
                    item.x = Character.Position.x;
                    item.z = Character.Position.z;
                    item.y = Character.Position.y;
                    Systems.aRound(ref item.x, ref item.y, 1);
                    item.xSec = Character.Position.xSec;
                    item.ySec = Character.Position.ySec;
                    item.Type = 1;
                    item.fromType = 6;
                    item.Owner = 0;
                    Systems.WorldItem.Add(item);
                    item.Send(Packet.ObjectSpawn(item), true);
                    Character.Information.Gold -= (long)Gold;
                    client.Send(Packet.InfoUpdate(1, (int)Character.Information.Gold, 0));
                    client.Send(Packet.MoveItem(0x0A, 0, 0, 0, (long)Gold, "DELETE_GOLD"));
                    SaveGold();
                }
                else
                {
                    client.Send(Packet.IngameMessages(SERVER_UPDATEGOLD, IngameMessages.UIIT_MSG_STRGERR_NOT_ENOUGH_GOLD));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Player_DropGold error: {0}", ex);
                Systems.Debugger.Write(ex);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Buy Item
        /////////////////////////////////////////////////////////////////////////////////
        void Player_BuyItem(byte selected_tab, byte selected_line, short buy_amount, int selected_npc_id)
        {
            #region Buy item
            try
            {
                //Create new object
                obj npc_details = GetObject(selected_npc_id);
                //Get shop line information
                string s = Data.ObjectBase[npc_details.ID].Tab[selected_tab];
                //Get shop index content
                Global.shop_data shopitem = Global.shop_data.GetShopIndex(s);
                //Get items
                short items = (short)Data.ItemBase[Global.item_database.GetItem(shopitem.Item[selected_line])].ID;
                //Set item price
                int gold = Data.ItemBase[items].Shop_price;
                //Check slot information
                byte slot = GetFreeSlot();
                if (slot <= 12) return;
                //Check the max amount of the item.
                if (buy_amount <= Data.ItemBase[Global.item_database.GetItem(shopitem.Item[selected_line])].Max_Stack)
                {
                    //Get item id
                    int Itemidinfo = Data.ItemBase[Global.item_database.GetItem(shopitem.Item[selected_line])].ID;
                    //Check if what the player is buying is a wearable type and not ETC so max stack is 1
                    if (Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.SPEAR ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.GLAVIE ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.SWORD ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.BLADE ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.EU_AXE ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.EU_HARP ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.EU_TSWORD ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.CH_SHIELD ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.EU_SHIELD ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.BOW ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.EU_CROSSBOW ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.HUNTERSUIT ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.THIEFSUIT ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.TRADERSUIT ||
                                Data.ItemBase[Itemidinfo].Type == Global.item_database.ArmorType.ARMOR||
                                Data.ItemBase[Itemidinfo].Type == Global.item_database.ArmorType.GARMENT ||
                                Data.ItemBase[Itemidinfo].Type == Global.item_database.ArmorType.HEAVY ||
                                Data.ItemBase[Itemidinfo].Type == Global.item_database.ArmorType.LIGHT ||
                                Data.ItemBase[Itemidinfo].Type == Global.item_database.ArmorType.PROTECTOR ||
                                Data.ItemBase[Itemidinfo].Type == Global.item_database.ArmorType.ROBE ||
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.EARRING || 
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.RING || 
                                Data.ItemBase[Itemidinfo].Itemtype == Global.item_database.ItemType.NECKLACE 
                        )
                        buy_amount = 1;
                    //Multiply the amount / price.
                    gold *= buy_amount;
                    //Reduct the gold from the player

                    //If the character gold equals or is higher then the price , continue
                    if (Character.Information.Gold >= gold)
                    {
                        Character.Information.Gold -= gold;
                        //Send packet to update gold visual
                        client.Send(Packet.UpdateGold(Character.Information.Gold));
                        //Save player gold
                        SaveGold();
                        //Add new item to user
                        client.Send(Packet.MoveItemBuy(8, selected_tab, selected_line, (byte)buy_amount, slot, buy_amount));
                        //Amount information
                        if (buy_amount > 1)
                            AddItem(items, buy_amount, slot, Character.Information.CharacterID, 0);
                        else if (buy_amount == 1)
                            AddItem(items, 0, slot, Character.Information.CharacterID, 0);
                    }
                    else
                    {
                        client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_NOT_ENOUGH_GOLD));
                    }
                }
                else
                {
                    client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_POSSESSION_LIMIT_EXCEEDED));
                }
            }
            catch (Exception ex)
            {
                Print.Format("Player_BuyItem({0},{1},{2},{3})::Error..", selected_tab, selected_line, buy_amount, selected_npc_id);
                Console.WriteLine("Buy item error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            SavePlayerInfo();
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Sell Item
        /////////////////////////////////////////////////////////////////////////////////
        void Player_SellItem(byte slot, short amount, int o_id)
        {
            #region Sell item
            try
            {
                Global.slotItem item = GetItem((uint)Character.Information.CharacterID, slot, 0);
                Character.Information.Gold += Data.ItemBase[item.ID].Sell_Price * amount;
                client.Send(Packet.UpdateGold(Character.Information.Gold));
                SaveGold();
                int owner = Convert.ToInt32(MsSQL.GetData("SELECT * FROM char_items WHERE id='" + item.dbID + "'", "owner"));
                if (owner == Character.Information.CharacterID)
                {
                    if (amount <= Data.ItemBase[item.ID].Max_Stack)
                    {
                        client.Send(Packet.MoveItemSell(9, slot, amount, o_id));
                        if (item.Amount != amount)
                        {
                            int calc = (item.Amount - amount);
                            if (calc < 1) calc = 1;
                            MsSQL.UpdateData("UPDATE char_items SET quantity='" + calc + "' WHERE itemnumber='" + "item" + slot + "' AND owner='" + Character.Information.CharacterID + "' AND itemid='" + item.ID + "'");
                        }
                        else
                        {
                            MsSQL.UpdateData("delete from char_items where itemnumber='item" + slot + "' AND owner='" + Character.Information.CharacterID + "'");
                        }
                        Character.Buy_Pack.Add(item);
                    }
                    else
                    {
                        client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_POSSESSION_LIMIT_EXCEEDED));
                    }
                }
                else
                {
                    Disconnect("ban");
                    Console.WriteLine("Autobanned user: " + Player.AccountName + " Due to hacking");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Sell item error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            SavePlayerInfo();
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Buy back item (Needs work).
        /////////////////////////////////////////////////////////////////////////////////
        void Player_BuyPack()
        {
            #region Player Buy Back
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);

                int id = Reader.Int32(); byte b_slot = Reader.Byte();
                Reader.Close();
                byte i_slot = GetFreeSlot();
                if (i_slot <= 12) return;

                Print.Format(b_slot.ToString());

                Global.slotItem item = Character.Buy_Pack.Get(b_slot);
                if (item.Amount < 1) item.Amount = 1;
                if (item.Amount <= Data.ItemBase[item.ID].Max_Stack)
                {
                    if (Character.Information.Gold >= item.Amount * Data.ItemBase[item.ID].Sell_Price)
                    {
                        Character.Information.Gold -= item.Amount * Data.ItemBase[item.ID].Sell_Price;
                        client.Send(Packet.UpdateGold(Character.Information.Gold));
                    }
                    else
                    {
                        client.Send(Packet.IngameMessages(SERVER_UPDATEGOLD, IngameMessages.UIIT_MSG_STRGERR_NOT_ENOUGH_GOLD));
                    }
                    SaveGold();
                    if (Data.ItemBase[item.ID].Class_D == 1)
                    {
                        MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot) VALUES ('" + item.ID + "','" + item.PlusValue + "','" + Data.ItemBase[item.ID].Defans.Durability + "','" + Character.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "')");
                        client.Send(Packet.MoveItemBuyGetBack(i_slot, b_slot, 1));
                    }
                    else if (Data.ItemBase[item.ID].Class_D == 2)
                    {
                        MsSQL.UpdateData("Insert Into char_items (itemid,quantity,owner,itemnumber,slot) VALUES ('" + item.ID + "','" + item.Amount + "','" + Character.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "' )");
                        client.Send(Packet.MoveItemBuyGetBack(i_slot, b_slot, item.Amount));
                    }
                    else if (Data.ItemBase[item.ID].Class_D == 3)
                    {
                        MsSQL.UpdateData("Insert Into char_items (itemid,quantity,owner,itemnumber,slot) VALUES ('" + item.ID + "','" + item.Amount + "','" + Character.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "' )");
                        client.Send(Packet.MoveItemBuyGetBack(i_slot, b_slot, item.Amount));
                    }
                    else if (Data.ItemBase[item.ID].Class_D == 4)
                    {
                        MsSQL.UpdateData("Insert Into char_items (itemid,quantity,owner,itemnumber,slot) VALUES ('" + item.ID + "','" + 1 + "','" + Character.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "' )");
                        client.Send(Packet.MoveItemBuyGetBack(i_slot, b_slot, item.Amount));
                    }
                    else if (Data.ItemBase[item.ID].Class_D == 6)
                    {
                        MsSQL.UpdateData("Insert Into char_items (itemid,quantity,owner,itemnumber,slot) VALUES ('" + item.ID + "','" + 1 + "','" + Character.Information.CharacterID + "','item" + i_slot + "','" + i_slot + "' )");
                        client.Send(Packet.MoveItemBuyGetBack(i_slot, b_slot, item.Amount));
                    }
                }
                else
                {
                    client.Send(Packet.IngameMessages(SERVER_ITEM_MOVE, IngameMessages.UIIT_MSG_STRGERR_POSSESSION_LIMIT_EXCEEDED));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Buy back error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            SavePlayerInfo();
            #endregion
        }
    }
}