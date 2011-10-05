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

namespace SrxRevo
{
    public partial class Packet
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Inventory item movement
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] MoveItem(byte type, byte fromSlot, byte toSlot, short quantity, long gold, string action)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            switch (action)
            {
                case "MOVE_INSIDE_INVENTORY":
                        Writer.Byte(1);
                        Writer.Byte(type);
                        Writer.Byte(fromSlot);
                        Writer.Byte(toSlot);
                        Writer.Word(quantity);
                        Writer.Byte(0);
                    break;
                case "MOVE_INSIDE_STORAGE":
                        Writer.Bool(true);
                        Writer.Byte(1);
                        Writer.Byte(fromSlot);
                        Writer.Byte(toSlot);
                        Writer.Word(quantity);
                    break;
                case "MOVE_INSIDE_GUILD_STORAGE":
                        Writer.Byte(1);
                        Writer.Byte(0x1D);//Type
                        Writer.Byte(fromSlot);
                        Writer.Byte(toSlot);
                        Writer.Word(quantity);
                    break;
                case "MOVE_TO_STORAGE":
                        Writer.Byte(1);
                        Writer.Byte(2);
                        Writer.Byte(fromSlot);
                        Writer.Byte(toSlot);
                    break;
                case "MOVE_TO_GUILD_STORAGE":
                        Writer.Byte(1);
                        Writer.Byte(0x1E);
                        Writer.Byte(fromSlot);
                        Writer.Byte(toSlot);
                    break;
                case "MOVE_FROM_STORAGE":
                        Writer.Byte(1);
                        Writer.Byte(3);
                        Writer.Byte(fromSlot);
                        Writer.Byte(toSlot);
                        break;
                case "MOVE_FROM_GUILD_STORAGE":
                        Writer.Byte(1);
                        Writer.Byte(0x1F);
                        Writer.Byte(fromSlot);
                        Writer.Byte(toSlot);
                        break;
                case "MOVE_WAREHOUSE_GOLD":
                        Writer.Bool(true);
                        Writer.Byte(type);
                        Writer.LWord(gold);
                        break;
                case "MOVE_GENDER_CHANGE":
                        Writer.Byte(1);
                        Writer.Byte(fromSlot);
                        Writer.Byte(0x13);
                        Writer.Byte(2);
                        break;
                case "DELETE_ITEM":
                        Writer.Byte(1);
                        Writer.Byte(type);
                        Writer.Byte(fromSlot);
                        if (type == 0x0F) Writer.Byte(4);
                    break;
                case "DELETE_GOLD":
                        Writer.Byte(1);
                        Writer.Byte(type);
                        Writer.LWord(gold);
                        break;
            }
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Pet related item inventory movement
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] MoveItemPet(int itemid, byte f_slot, byte t_slot,pet_obj o,short info, string action)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            switch (action)
            {
                case "MOVE_TO_PET":
                case "MOVE_FROM_PET":
                        Writer.Byte(1);
                        Writer.Byte(o.Slots);
                        Writer.DWord(itemid);
                        Writer.Byte(f_slot);
                        Writer.Byte(t_slot);
                    break;
                case "MOVE_INSIDE_PET":
                        Writer.Byte(1);
                        Writer.Byte(0x10);
                        Writer.DWord(itemid);
                        Writer.Byte(f_slot);
                        Writer.Byte(t_slot);
                        Writer.Word(info);
                    break;
            }
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Move Item From Buying To Inventory
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] MoveItemBuy(byte type, byte shopLine, byte itemLine, byte max, byte slot, short amount)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Bool(true);
            Writer.Byte(type);
            Writer.Byte(shopLine);
            Writer.Byte(itemLine);
            Writer.Byte(1);
            Writer.Byte(slot);
            Writer.Word(amount);
            Writer.DWord(0);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Move Item From Inventory To Npc
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] MoveItemSell(byte type, byte slot, short amount, int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Bool(true);
            Writer.Byte(type);
            Writer.Byte(slot);
            Writer.Word(amount);
            Writer.DWord(id);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Buy Item Back From Npc
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] MoveItemBuyGetBack(byte slot, byte b_slot, short amount)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Bool(true);
            Writer.Byte(0x22);
            Writer.Byte(slot);
            Writer.Byte(b_slot);
            Writer.Word(amount);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Unequip Item // Needs Switch type for item Type as packet below this one
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] MoveItemUnequipEffect(int id, byte Slot, int iid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_UN_EFFECT);
            Writer.DWord(id);
            Writer.Byte(Slot);
            Writer.DWord(iid);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Unequip Item
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] MoveItemEnquipEffect(int id, byte slot, int iid, byte plus)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_EFFECT);
            Writer.DWord(id);
            Writer.Byte(slot);
            Writer.DWord(iid);
            Writer.Byte(plus);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Buy Item From Item Mall
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] BuyItemFromMall(byte type1, byte type2, byte type3, byte type4, byte type5, byte slot)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);    //Select opcode
            Writer.Byte(1);                             //Static Byte 1
            Writer.Byte(0x18);                          //Static Byte 0x18 /switch possible perhaps
            Writer.Byte(type1);                         // Recheck
            Writer.Byte(type2);                         // Recheck
            Writer.Byte(type3);                         // Recheck
            Writer.Byte(type4);                         // Recheck
            Writer.Byte(type5);                         // Recheck
            Writer.Byte(1);                             //Static Byte 1
            Writer.Byte(slot);                          //Select To Slot
            Writer.Word(1);                             //Static
            Writer.DWord(0);                            //Static
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Create Item From Gm console
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] GM_MAKEITEM(byte type, byte Slot, int id, short plus, int durability, int itemid, int bluecount)
        {
            int msid = Systems.MsSQL.GetDataInt("SELECT id FROM char_items WHERE owner='" + id + "' AND slot = '" + Slot + "'", "id");
            Systems.LoadBluesid(msid);

            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Bool(true);
            Writer.Byte(6);
            Item.AddItemPacket(Writer, Slot, id, (byte)plus, plus, durability, itemid, msid, 0);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Delete Item from inventory visual
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] DespawnFromInventory(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_DELETE);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Update gold
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] UpdateGold(long gold)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_UPDATEGOLD);
            Writer.Byte(1);
            Writer.LWord(gold);
            Writer.Byte(0);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Alchemy items gaining. (Maybe refactor with main move item packet).
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] GainElements(byte slot, int itemid, short amount)
        {
            PacketWriter writer = new PacketWriter();
            writer.Create(Systems.SERVER_ITEM_MOVE);
            writer.Byte(1);
            writer.Byte(0x0E);
            writer.Byte(slot);
            writer.Byte(0);
            writer.DWord(0);
            writer.DWord(itemid);
            writer.Word(amount);
            return writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Destroy item with alchemy
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] DestroyItem()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_DISSEMBLE_ITEM);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Update arrow amount
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] Arrow(short amount)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ARROW_UPDATE);
            Writer.Word(amount);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Update quantity of items
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] ItemUpdate_Quantity(byte slot, short amount)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_QUANTITY_UPDATE);
            Writer.Byte(slot);
            Writer.Byte(8);
            Writer.Word(amount);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Change item quantity
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] ChangeItemQ(byte tslot, int itemid)
        {
            PacketWriter writer = new PacketWriter();
            writer.Create(Systems.SERVER_ITEM_QUANTITY_UPDATE);
            writer.Byte(tslot);
            writer.Byte(1);
            writer.DWord(itemid);
            return writer.GetBytes();
        }
        public static byte[] Update2(byte slot)
        {
            PacketWriter writer = new PacketWriter();
            writer.Create(Systems.SERVER_ITEM_QUANTITY_UPDATE);
            writer.Byte(slot);
            writer.Byte(0x40);
            writer.Byte(2);
            return writer.GetBytes();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Add Item Packet
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    class Item
    {
        public static void AddItemPacket(PacketWriter Writer, byte slot, int id, byte plus, short amount, int durability, int itemid, int bluecount, int modelid)
        {
            try
            {
                //Blues pre loading
                #region Load blues for items
                Systems.LoadBluesid(itemid);
                #endregion
                //Static information for packet
                #region Static packet info
                if (slot != 255) Writer.Byte(slot); // slot
                Writer.DWord(0);                    // sttaic 0
                Writer.DWord(id);                   // Item ID
                #endregion
                //Armor types and jewerly
                #region Armor related
                if (Data.ItemBase[id].Type == Global.item_database.ArmorType.ARMOR ||
                    Data.ItemBase[id].Type == Global.item_database.ArmorType.GARMENT ||
                    Data.ItemBase[id].Type == Global.item_database.ArmorType.GM ||
                    Data.ItemBase[id].Type == Global.item_database.ArmorType.HEAVY ||
                    Data.ItemBase[id].Type == Global.item_database.ArmorType.LIGHT ||
                    Data.ItemBase[id].Type == Global.item_database.ArmorType.PROTECTOR ||
                    Data.ItemBase[id].Type == Global.item_database.ArmorType.ROBE ||
                    Data.ItemBase[id].Itemtype == Global.item_database.ItemType.EARRING ||
                    Data.ItemBase[id].Itemtype == Global.item_database.ItemType.RING ||
                    Data.ItemBase[id].Itemtype == Global.item_database.ItemType.NECKLACE)
                {
                    Writer.Byte(plus);
                    Writer.Byte(0);                 //Durability + %
                    Writer.Byte(0);                 //Item Phy/Mag reinforce value %
                    Writer.Byte(0);                 //Physical defense power %
                    Writer.Byte(0);                 //Magical defense power %
                    Writer.Byte(0);                 //Static byte ?
                    Writer.Byte(0);                 //Static byte ?
                    Writer.Byte(0);                 //Static byte ?
                    Writer.Byte(0);                 //Static byte ?
                    Writer.DWord(durability);       //Durability

                    if (bluecount != 0)
                    {
                        Writer.Byte(Convert.ToByte(Data.ItemBlue[itemid].totalblue));
                        for (int i = 0; i <= Data.ItemBlue[itemid].totalblue - 1; i++)
                        {

                            Writer.DWord(Data.MagicOptions.Find(mg => (mg.Name == Convert.ToString(Data.ItemBlue[itemid].blue[i]))).ID);
                            Writer.DWord(Data.ItemBlue[itemid].blueamount[i]);

                        }
                    }
                    else
                    {
                        Writer.Byte(0);
                    }

                    Writer.Byte(1);
                    Writer.Byte(0);
                    Writer.Byte(2);
                    Writer.Byte(0);
                    Writer.Byte(3);
                    Writer.Byte(0);
                    return;
                }
                #endregion
                //Weaponry (Weapons).
                #region Weapons
                else if (Data.ItemBase[id].Itemtype == Global.item_database.ItemType.BLADE ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.BOW ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.EU_AXE ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.EU_CROSSBOW ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.EU_HARP ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.EU_TSWORD ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.GLAVIE ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.SPEAR ||
                     Data.ItemBase[id].Itemtype == Global.item_database.ItemType.SWORD)
                {
                    Writer.Byte(plus);              //Item Plus Value
                    Writer.Byte(0x64);                 //Durability + %
                    Writer.Byte(0);                 //Item Phy/Mag reinforce value %
                    Writer.Byte(0);                 //Attack rating / blocking ratio
                    Writer.Byte(0);                 //Magical defense power %
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.DWord(durability);

                    if (bluecount != 0)
                    {
                        Writer.Byte(Convert.ToByte(Data.ItemBlue[itemid].totalblue));
                        for (int i = 0; i <= Data.ItemBlue[itemid].totalblue - 1; i++)
                        {
                            Writer.DWord(Data.MagicOptions.Find(mg => (mg.Name == Convert.ToString(Data.ItemBlue[itemid].blue[i]))).ID);
                            Writer.DWord(Data.ItemBlue[itemid].blueamount[i]);
                        }
                    }

                    else
                    {
                        Writer.Byte(0);
                    }
                    Writer.Byte(1);
                    Writer.Byte(0);
                    Writer.Byte(2);
                    Writer.Byte(0);
                    Writer.Byte(3);
                    Writer.Byte(0);
                    return;
                }
                #endregion
                //Shields
                #region Shields
                else if (Data.ItemBase[id].Itemtype == Global.item_database.ItemType.CH_SHIELD || 
                    Data.ItemBase[id].Itemtype == Global.item_database.ItemType.EU_SHIELD)
                {
                    Writer.Byte(plus);              //Item Plus Value
                    Writer.Byte(0);                 //Durability + %
                    Writer.Byte(0);                 //Item Phy/Mag reinforce value %
                    Writer.Byte(9);                 //blocking ratio
                    Writer.Byte(0);                 //Magical defense power %
                    Writer.Byte(0);                 
                    Writer.Byte(0);                 
                    Writer.Byte(0);                 
                    Writer.Byte(0);                 
                    Writer.DWord(durability);       //Durability

                    if (bluecount != 0)
                    {
                        Writer.Byte(Convert.ToByte(Data.ItemBlue[itemid].totalblue));
                        for (int i = 0; i <= Data.ItemBlue[itemid].totalblue - 1; i++)
                        {

                            Writer.DWord(Data.MagicOptions.Find(mg => (mg.Name == Convert.ToString(Data.ItemBlue[itemid].blue[i]))).ID);
                            Writer.DWord(Data.ItemBlue[itemid].blueamount[i]);

                        }
                    }

                    else
                    {
                        Writer.Byte(0);
                    }
                    Writer.Byte(1);
                    Writer.Byte(0);
                    Writer.Byte(2);
                    Writer.Byte(0);
                    Writer.Byte(3);
                    Writer.Byte(0);
                    return;
                }
                #endregion
                //Avatars
                #region Avatars
                else if (Data.ItemBase[id].Type == Global.item_database.ArmorType.AVATAR ||
                         Data.ItemBase[id].Type == Global.item_database.ArmorType.AVATARATTACH ||
                         Data.ItemBase[id].Type == Global.item_database.ArmorType.AVATARHAT)
                {
                    Writer.Byte(plus);              //Item Plus Value
                    Writer.Byte(0);                 //Item Magical Reinforce value
                    Writer.Byte(0);                 //Item Phy reinforce value
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Word(0);
                    Writer.Word(0);
                    Writer.DWord(durability);       //Item Durability
                    Writer.Byte(0);
                    Writer.Word(1);
                    Writer.Word(2);
                    Writer.Word(3);
                    return;
                }
                #endregion
                //Grabpets
                #region Grabpets
                else if (Data.ItemBase[id].Pettype == Global.item_database.PetType.GRABPET)
                {
                    Writer.Byte(0); // State :0 = Not opened yet, 1 =  , 3 = Expired
                    Writer.DWord(0x00002432); //Time date probably
                    Writer.Word(0);
                    Writer.DWord(1);
                    Writer.Byte(0);
                    return;
                }
                #endregion
                //Attack pets
                #region Attackpets
                else if (Data.ItemBase[id].Pettype == Global.item_database.PetType.ATTACKPET)
                {
                    Writer.Byte(0);             //
                    Writer.Byte(2);             //
                    Writer.Byte(24);            // Level
                    Writer.Word(0);             //
                    Writer.Word(0);             //Petname
                    Writer.Byte(0);
                    return;
                }
                #endregion
                //Normal stones
                #region Stones
                else if (Data.ItemBase[id].Etctype == Global.item_database.EtcType.STONES)
                {
                    Writer.Word(amount);
                    Writer.Byte(0);//Assumability %
                    return;
                }
                #endregion
                //Monster masks
                #region Monster masks
                else if (Data.ItemBase[id].Etctype == Global.item_database.EtcType.MONSTERMASK)
                {
                    Writer.DWord(modelid);
                    return;
                }
                #endregion
                //Stall decoration
                #region  Stall decoration
                else if (Data.ItemBase[id].Etctype == Global.item_database.EtcType.STALLDECORATION)
                {
                    Writer.Byte(0);
                    Writer.Word(1);
                    return;
                }
                #endregion
                //Elixirs
                #region Elixirs
                else if (Data.ItemBase[id].Etctype == Global.item_database.EtcType.ELIXIR)
                {
                    Writer.Word(1);
                    return;
                }
                #endregion
                //Other items (To filter later).
                #region Other items
                //Need to add more and add more on fileload
                else if (Data.ItemBase[id].Itemtype == Global.item_database.ItemType.ARROW || 
                    Data.ItemBase[id].Itemtype == Global.item_database.ItemType.BOLT ||
                    Data.ItemBase[id].Etctype == Global.item_database.EtcType.HP_POTION ||
                    Data.ItemBase[id].Etctype == Global.item_database.EtcType.MP_POTION ||
                    Data.ItemBase[id].Etctype == Global.item_database.EtcType.VIGOR_POTION ||
                    Data.ItemBase[id].Etctype == Global.item_database.EtcType.SPEED_POTION ||
                    Data.ItemBase[id].Ticket == Global.item_database.Tickets.BEGINNER_HELPERS ||
                    Data.ItemBase[id].Etctype == Global.item_database.EtcType.ELIXIR ||
                    Data.ItemBase[id].Etctype == Global.item_database.EtcType.ALCHEMY_MATERIAL ||
                    Data.ItemBase[id].Etctype == Global.item_database.EtcType.EVENT ||
                    Data.ItemBase[id].Class_D == 3)
                {
                    Writer.Word(amount);
                    return;
                }
                else
                {
                    Writer.Byte(0);
                    Writer.Word(1);
                    return;
                }
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine("Load item error: {0}", ex);
            }
        }
    }
}