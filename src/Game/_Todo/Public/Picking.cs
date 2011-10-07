///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Movement checks before picking
        /////////////////////////////////////////////////////////////////////////////////
        void Player_PickUp()
        {
            try
            {
                if (Character.State.Sitting) return; 
                if (Character.State.Exchanging) return;
                
                if (Character.Action.Target != 0)
                {
                    world_item item     = GetWorldItem(Character.Action.Target);

                    if (item == null) 
                        return;

                    double distance = Formule.gamedistance(Character.Position.x,Character.Position.y,(float)item.x,(float)item.y);

                    if (distance >= 1)
                    {
                        Character.Position.wX = (float)item.x - Character.Position.x;
                        Character.Position.wY = (float)item.y - Character.Position.y;

                        Send(Packet.Movement(new DarkEmu_GameServer.Global.vektor(Character.Information.UniqueID,(float)Formule.packetx((float)item.x, item.xSec),(float)Character.Position.z,(float)Formule.packety((float)(float)item.y, item.ySec),Character.Position.xSec,Character.Position.ySec)));

                        Character.Position.Time = (distance / (Character.Speed.RunSpeed * 0.0768)) * 1000.0;
                        Character.Position.RecordedTime = Character.Position.Time;

                        StartMovementTimer((int)(Character.Position.Time * 0.1));
                        return;
                    }

                    Player_PickUpItem();
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Pickup item
        /////////////////////////////////////////////////////////////////////////////////
        void Player_PickUpItem()
        {
            try
            {
                //First check if player allready is picking up an item
                if (Character.Action.PickUping)
                {
                    //Get item information that the player has selected.
                    world_item item = GetWorldItem(Character.Action.Target);
                    //Checks
                    if (item == null) { Character.Action.PickUping = false; return; }
                    //If the amount is lower then one
                    if (item.amount < 1) item.amount = 1;
                    //If not gold model
                    if (item.Model > 3)
                    {
                        //Get our free slots
                        byte slot = GetFreeSlot();
                        //If to low
                        if (slot <= 12)
                        {
                            Character.Action.PickUping = false;
                            client.Send(Packet.MoveItemError());
                            return;
                        }
                        //Else continue stop delete timer because its allready beeing removed.
                        item.StopDeleteTimer();
                        //Remove the world item spawn
                        Systems.WorldItem.Remove(item);
                        //Delete the global id
                        Global.ID.Delete(item.UniqueID);
                        //Move towards the item
                        Send(Packet.MovementOnPickup(new DarkEmu_GameServer.Global.vektor(Character.Information.UniqueID,
                                        (float)Formule.packetx((float)item.x, item.xSec),
                                        (float)Character.Position.z,
                                        (float)Formule.packety((float)(float)item.y, item.ySec),
                                        Character.Position.xSec,
                                        Character.Position.ySec)));
                        //Send animation packet pickup
                        Send(Packet.Pickup_Animation(Character.Information.UniqueID, 0));
                        //Check what item type we have (Etc, or armor / weapon).
                        int amount = 0;
                        //Set amount or plusvalue
                        #region Amount definition
                        if (Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.BLADE ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.CH_SHIELD ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.EU_SHIELD ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.BOW ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.EU_AXE ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.EU_CROSSBOW ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.EU_HARP ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.EU_TSWORD ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.GLAVIE ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.SPEAR ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.SWORD ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.EARRING ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.RING ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.NECKLACE ||
                             Data.ItemBase[item.Model].Type == Global.item_database.ArmorType.ARMOR ||
                             Data.ItemBase[item.Model].Type == Global.item_database.ArmorType.GARMENT ||
                             Data.ItemBase[item.Model].Type == Global.item_database.ArmorType.GM ||
                             Data.ItemBase[item.Model].Type == Global.item_database.ArmorType.HEAVY ||
                             Data.ItemBase[item.Model].Type == Global.item_database.ArmorType.LIGHT ||
                             Data.ItemBase[item.Model].Type == Global.item_database.ArmorType.PROTECTOR ||
                             Data.ItemBase[item.Model].Itemtype == Global.item_database.ItemType.AVATAR ||
                             Data.ItemBase[item.Model].Type == Global.item_database.ArmorType.ROBE) 
                                amount = item.PlusValue;
                        else amount = item.amount;
                        #endregion
                        //Send item creation packet
                        client.Send(Packet.GM_MAKEITEM(0, slot, item.Model, (short)amount, (int)Data.ItemBase[item.Model].Defans.Durability, Data.ItemBase[item.Model].ID, item.UniqueID));
                        //Save to database
                        AddItem(item.Model, (short)amount, slot, Character.Information.CharacterID, item.Model);
                    }
                    //If the item is gold
                    else
                    {
                        //Remove the spawned item
                        Systems.WorldItem.Remove(item);
                        //Remove global id
                        Global.ID.Delete(item.UniqueID);
                        //Movement packet
                        Send(Packet.MovementOnPickup(new DarkEmu_GameServer.Global.vektor(Character.Information.UniqueID,
                                    (float)Formule.packetx((float)item.x, item.xSec),
                                    (float)Character.Position.z,
                                    (float)Formule.packety((float)(float)item.y, item.ySec),
                                    Character.Position.xSec,
                                    Character.Position.ySec)));
                        //Send animation packet
                        Send(Packet.Pickup_Animation(Character.Information.UniqueID, 0));
                        //Add gold to player information
                        Character.Information.Gold += item.amount;
                        //Send visual packet for gold
                        client.Send(Packet.UpdateGold(Character.Information.Gold));
                        //Send message packet gold gained
                        client.Send(Packet.GoldMessagePick(item.amount));
                        //Save player gold
                        SaveGold();
                    }
                    //Despawn item for us
                    item.DeSpawnMe();
                    //Dispose of the item
                    item.Dispose();
                    //Set picking to false
                    Character.Action.PickUping = false;
                    if (Timer.Pickup != null) Timer.Pickup.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Pickup Error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void Pet_PickupItem(world_item item)
        {
           
        }
        
    }
}
