///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SrxRevo
{
    public partial class Systems
    {
        //########################################
        // Buy from item mall
        //########################################
        void Player_BuyItemFromMall(byte type1, byte type2, byte type3, byte type4, byte type5, string itemname)
        {
            try
            {
                //Set default info
                int amount = 1;
                string silktype = "normalsilk";
                
                //If premium the item is not bound, and we must reduce other silk count
                if (itemname.Contains("PRE")) silktype = "premiumsilk";

                //TMP Fix avatars until time items added
                if (itemname.Contains("28D"))
                    itemname = itemname.Remove(itemname.Length - 4, 4);

                //Parse the item name from reader
                itemname = itemname.Remove(0, 8);
                
                //Get item information id
                int itemid = Global.item_database.GetItem(itemname);

                //Slot pre-check
                byte slot = GetFreeSlot();
                
                //Checks before we continue               
                if (slot <= 12 || Data.ItemBase[itemid].Shop_price == 0 || itemid == 0) return;
                
                //Check player silk and if item is defined in our server
                if (Player.Silk >= Data.ItemBase[itemid].Shop_price || Player.SilkPrem >= Data.ItemBase[itemid].Shop_price)
                {
                    //Update player silk
                    if (silktype == "normalsilk")
                    {
                        Player.Silk -= Data.ItemBase[itemid].Shop_price;
                        MsSQL.UpdateData("UPDATE users SET silk="+ Player.Silk +" WHERE id='" + Player.AccountName + "'");
                        client.Send(Packet.Silk(Player.Silk, Player.SilkPrem));
                    }
                    else if (silktype == "premiumsilk")
                    {
                        Player.SilkPrem -= Data.ItemBase[itemid].Shop_price;
                        MsSQL.UpdateData("UPDATE users SET premsilk=" + Player.SilkPrem + " WHERE id='" + Player.AccountName + "'");
                        client.Send(Packet.Silk(Player.Silk, Player.SilkPrem));
                    }
                   
                    //Send first packet to client
                    client.Send(Packet.BuyItemFromMall(type1, type2, type3, type4, type5, slot));
                    
                    //Set up defined information per type.
                    if (Data.ItemBase[itemid].Etctype == Global.item_database.EtcType.GLOBALCHAT ||
                        Data.ItemBase[itemid].Etctype == Global.item_database.EtcType.REVERSESCROLL ||
                        Data.ItemBase[itemid].Etctype == Global.item_database.EtcType.RETURNSCROLL)
                    {
                        amount = 11;
                        AddItem(itemid, (short)amount, slot, Character.Information.CharacterID, 0);
                    }
                    //Pets and others related
                    else if (Data.ItemBase[itemid].Pettype == Global.item_database.PetType.JOBTRANSPORT ||
                             Data.ItemBase[itemid].Pettype == Global.item_database.PetType.TRANSPORT ||
                             Data.ItemBase[itemid].Etctype == Global.item_database.EtcType.AVATAR28D ||
                             Data.ItemBase[itemid].Type == Global.item_database.ArmorType.AVATAR ||
                             Data.ItemBase[itemid].Type == Global.item_database.ArmorType.AVATARATTACH ||
                             Data.ItemBase[itemid].Type == Global.item_database.ArmorType.AVATARHAT ||
                             Data.ItemBase[itemid].Etctype == Global.item_database.EtcType.WAREHOUSE ||
                             Data.ItemBase[itemid].Etctype == Global.item_database.EtcType.INVENTORYEXPANSION ||
                             Data.ItemBase[itemid].Etctype == Global.item_database.EtcType.CHANGESKIN ||
                             Data.ItemBase[itemid].Etctype == Global.item_database.EtcType.STALLDECORATION ||
                             Data.ItemBase[itemid].Ticket == Global.item_database.Tickets.BATTLE_ARENA ||
                             Data.ItemBase[itemid].Ticket == Global.item_database.Tickets.DUNGEON_EGYPT ||
                             Data.ItemBase[itemid].Ticket == Global.item_database.Tickets.DUNGEON_FORGOTTEN_WORLD ||
                             Data.ItemBase[itemid].Ticket == Global.item_database.Tickets.OPEN_MARKET)
                    {
                        amount = 1;
                        AddItem(itemid, (short)amount, slot, Character.Information.CharacterID, 0);
                    }
                    //#############################################################################
                    // Grabpets
                    //#############################################################################
                    else if (Data.ItemBase[itemid].Pettype == Global.item_database.PetType.GRABPET)
                    {
                        amount = 1;
                        AddItem(itemid, (short)amount, slot, Character.Information.CharacterID, 0);
                    }
                    //#############################################################################
                    // Attack Pets
                    //#############################################################################
                    else if (Data.ItemBase[itemid].Pettype == Global.item_database.PetType.ATTACKPET)
                    {
                        amount = 1;
                        AddItem(itemid, (short)amount, slot, Character.Information.CharacterID, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Item mall buy error {0}", ex);
            }
        }
        //########################################
        // Setup item mall prices
        //########################################
        public static int SetSilk(int id)
        {
            //Normal silk items (Will define prem / normal later).
            if (Data.ItemBase[id].Etctype == Global.item_database.EtcType.INVENTORYEXPANSION)
                return 250;
            else if (Data.ItemBase[id].Etctype == Global.item_database.EtcType.REVERSESCROLL)
                return 14;
            else if (Data.ItemBase[id].Pettype == Global.item_database.PetType.GRABPET)
                return 88;
            else if (Data.ItemBase[id].Pettype == Global.item_database.PetType.ATTACKPET)
                return 130;
            else if (Data.ItemBase[id].Pettype == Global.item_database.PetType.TRANSPORT)
                return 1;
            else if (Data.ItemBase[id].Type == Global.item_database.ArmorType.AVATAR)
                return 123;
            else if (Data.ItemBase[id].Type == Global.item_database.ArmorType.AVATARHAT)
                return 28;
            else if (Data.ItemBase[id].Type == Global.item_database.ArmorType.AVATARATTACH)
                return 14;
            else if (Data.ItemBase[id].Etctype == Global.item_database.EtcType.RETURNSCROLL)
                return 10;
            else if (Data.ItemBase[id].Etctype == Global.item_database.EtcType.CHANGESKIN)
                return 80;
            else if (Data.ItemBase[id].Etctype == Global.item_database.EtcType.STALLDECORATION)
                return 24;

            else
            return 0;
        }
    }
}
