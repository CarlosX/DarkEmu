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
        // List free slots in inventory
        /////////////////////////////////////////////////////////////////////////////////
        public byte GetFreeSlot()
        {
            #region List free slots
            List<byte> ListSlot = new List<byte>(Character.Information.Slots);
            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + Character.Information.CharacterID + "' AND slot >= '13' AND slot <= '" + Character.Information.Slots + "' AND inavatar='0'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    ListSlot.Add(reader.GetByte(5));
                }
            }
            ms.Close();
            for (byte i = 13; i < Character.Information.Slots; i++)
            {
                if (!GetCheckFreeSlot(ListSlot, i)) return i;
            }
            return 0;
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Return values for free slots
        /////////////////////////////////////////////////////////////////////////////////
        bool GetCheckFreeSlot(List<byte> b, byte bs)
        {
            #region Return free slots
            bool result = b.Exists(
                    delegate(byte bk)
                    {
                        return bk == bs;
                    }
                    );
            return result;
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get Max Slots
        /////////////////////////////////////////////////////////////////////////////////
        public byte GetFreeSlotMax()
        {
            #region Get Max Slots Available
            List<byte> ListSlot = new List<byte>(Character.Information.Slots);
            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + Character.Information.CharacterID + "' AND slot >= '13' AND slot <= '" + Character.Information.Slots + "' AND inavatar='0'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    ListSlot.Add(reader.GetByte(5));
                }
            }
            ms.Close();
            byte add = 0;
            for (byte i = 13; i < Character.Information.Slots; i++)
            {
                if (!GetCheckFreeSlot(ListSlot, i)) add++;
            }
            return add;
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get Slot Item Information
        /////////////////////////////////////////////////////////////////////////////////
        static Global.slotItem GetItem(uint id, byte slot, int type)
        {
            #region Slot item info

            try
            {
                if (id != 0)
                {
                    Global.slotItem slotItem = new Global.slotItem();
                    int row = type;

                    MsSQL ms;
                    if (row == 1)
                    {
                        ms = new MsSQL("SELECT * FROM char_items WHERE itemnumber='item" + slot + "' AND storageacc='" + id + "' AND storagetype='" + row + "' AND slot='" + slot + "'");
                    }
                    else if (row == 3)
                    {
                        ms = new MsSQL("SELECT * FROM char_items WHERE itemnumber='item" + slot + "' AND storagetype='" + row + "' AND slot='" + slot + "'");
                    }
                    else
                    {
                        ms = new MsSQL("SELECT * FROM char_items WHERE itemnumber='item" + slot + "' AND owner='" + id + "' AND storagetype='" + row + "' AND slot='" + slot + "'");
                    }

                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            slotItem.dbID = reader.GetInt32(0);
                            slotItem.ID = reader.GetInt32(2);
                            slotItem.PlusValue = reader.GetByte(4);
                            slotItem.Amount = reader.GetInt16(6);
                            slotItem.Durability = reader.GetInt32(7);
                            slotItem.Slot = slot;
                            LoadBluesid(slotItem.dbID);
                        }
                    }
                    ms.Close();
                    return slotItem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Data item load error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            return null;
            #endregion

        }
        /////////////////////////////////////////////////////////////////////////////////
        // Slot update packet
        /////////////////////////////////////////////////////////////////////////////////
        void GetUpdateSlot(Global.slotItem item, byte toSlot, int toItemID, short quantity)
        {
            #region Send item move packet
            client.Send(Packet.MoveItem(0, item.Slot, toSlot, quantity, 0, "MOVE_INSIDE_INVENTORY"));
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Update Item Count
        /////////////////////////////////////////////////////////////////////////////////
        void ItemUpdateAmount(Global.slotItem sItem, int owner)
        {
            #region Item Update Amount
            if (sItem.Amount <= 0)
                MsSQL.UpdateData("delete from char_items where slot='" + sItem.Slot + "' AND owner='" + owner + "'");
            else
                MsSQL.InsertData("UPDATE char_items SET quantity='" + Math.Abs(sItem.Amount) + "' WHERE slot='" + sItem.Slot + "' AND owner='" + owner + "'");
            client.Send(Packet.ItemUpdate_Quantity(sItem.Slot, sItem.Amount));
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Arrow Item Check
        /////////////////////////////////////////////////////////////////////////////////
        bool ItemCheckArrow()
        {
            #region Check Arrow
            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + Character.Information.CharacterID + "' AND slot >= '13' AND slot <= '" + Character.Information.Slots + "' AND inavatar='0' AND itemid='62' AND storagetype='0'");
            if (ms.Count() == 0)
            {
                ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + Character.Information.CharacterID + "' AND slot >= '13' AND slot <= '" + Character.Information.Slots + "' AND inavatar='0' AND itemid='3823' AND storagetype='0'");
            }
            else if (ms.Count() == 0)
            {
                ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + Character.Information.CharacterID + "' AND slot >= '13' AND slot <= '" + Character.Information.Slots + "' AND inavatar='0' AND itemid='10302' AND storagetype='0'");
            }
            else if (ms.Count() == 0)
            {
                ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + Character.Information.CharacterID + "' AND slot >= '13' AND slot <= '" + Character.Information.Slots + "' AND inavatar='0' AND itemid='10487' AND storagetype='0'");
            }
            if (ms.Count() == 0) return false;
            else
            {
                Global.slotItem items = null;
                using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                        items = ConvertToItem(reader.GetInt32(2), reader.GetByte(5), reader.GetInt16(6), 1);
                }
                ms.Close();
                MsSQL.InsertData("UPDATE char_items SET itemnumber='item" + 7 + "',slot='" + 7 + "' WHERE itemnumber='" + "item" + items.Slot + "' AND owner='" + Character.Information.CharacterID + "' AND itemid='" + items.ID + "'");
                client.Send(Packet.MoveItem(0, items.Slot, 7, items.Amount, 0, "MOVE_INSIDE_INVENTORY"));

                Character.Information.Item.sAmount = items.Amount;
                Character.Information.Item.sID = items.ID;
                ;
                return true;
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Convert to item
        /////////////////////////////////////////////////////////////////////////////////
        Global.slotItem ConvertToItem(int id, byte slots, short amount, byte index)
        {
            #region Arrow convert info
            Global.slotItem slot = new SrxRevo.Global.slotItem();
            slot.ID = id;
            slot.Slot = slots;
            slot.Amount = amount;
            return slot;
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Check Item Level
        /////////////////////////////////////////////////////////////////////////////////
        public static bool CheckItemLevel(byte level, int itemID)
        {
            #region Item Level
            bool bln = false;
            if (Data.ItemBase[itemID].Level <= level) bln = true;
            return bln;
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Check Gender
        /////////////////////////////////////////////////////////////////////////////////
        public static bool CheckGender(int my_gender_db, int itemID)
        {
            #region Item Gender Check
            byte my_gender = 0;
            bool bln_gender = false;

            if (my_gender_db >= 1907 && my_gender_db <= 1919) my_gender = 1;
            if (my_gender_db >= 1920 && my_gender_db <= 1932) my_gender = 0;
            if (my_gender_db >= 14717 && my_gender_db <= 14729) my_gender = 1;
            if (my_gender_db >= 14730 && my_gender_db <= 14742) my_gender = 0;
            if (my_gender == Data.ItemBase[itemID].Gender) bln_gender = true; else bln_gender = false;
            if (Data.ItemBase[itemID].Gender == 2) bln_gender = true;
            return bln_gender;
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Check race eu / ch
        /////////////////////////////////////////////////////////////////////////////////
        public static bool CheckRace(int raceinfo, int itemID)
        {
            byte racer = 2;
            bool raceis = false;
            if (raceinfo > 10000)
            {
                racer = 1;
            }
            else
            {
                racer = 0;
            }
            if (Data.ItemBase[itemID].Race == racer)
            {
                raceis = true;
                return raceis;
            }
            else
            {
                raceis = false;
                return raceis;
            }           
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get player inventory items (We will use this for stacking and checking).
        // So herer's the simple one ive made we can use to check player inventory
        /////////////////////////////////////////////////////////////////////////////////
        public static List<byte> GetPlayerItems(character c)
        {
            List<byte> items = new List<byte>();
            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + c.Information.CharacterID + "'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    items.Add(reader.GetByte(5));
                }
            }
            ms.Close();
            return items;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Check Armor Type
        /////////////////////////////////////////////////////////////////////////////////
        public static bool CheckArmorType(int FromItemID, int uid)
        {
            #region Armor Type
            
                Global.item_database.ArmorType[] SameType = new Global.item_database.ArmorType[6];
                for (byte i = 0; i <= 5; i++)
                {
                    Global.slotItem env = GetItem((uint)uid, i, 0);
                    if (env.ID != 0)
                    {
                        SameType[i] = Data.ItemBase[env.ID].Type;
                    }
                }
                if (SameType != null)
                {
                    if ((Data.ItemBase[FromItemID].Type == Global.item_database.ArmorType.ARMOR && SameType.Count(st => (st == Global.item_database.ArmorType.GARMENT)) > 0))
                        return false;
                    else if (Data.ItemBase[FromItemID].Type == Global.item_database.ArmorType.PROTECTOR && SameType.Count(st => (st == Global.item_database.ArmorType.GARMENT)) > 0)
                        return false;
                    else if (Data.ItemBase[FromItemID].Type == Global.item_database.ArmorType.HEAVY && SameType.Count(st => (st == Global.item_database.ArmorType.ROBE)) > 0)
                        return false;
                    else if (Data.ItemBase[FromItemID].Type == Global.item_database.ArmorType.LIGHT && SameType.Count(st => (st == Global.item_database.ArmorType.ROBE)) > 0)
                        return false;
                    else if (Data.ItemBase[FromItemID].Type == Global.item_database.ArmorType.GARMENT && ((SameType.Count(st => (st == Global.item_database.ArmorType.ARMOR)) > 0) || (SameType.Count(st => (st == Global.item_database.ArmorType.PROTECTOR)) > 0)))
                        return false;
                    else if (Data.ItemBase[FromItemID].Type == Global.item_database.ArmorType.ROBE && ((SameType.Count(st => (st == Global.item_database.ArmorType.HEAVY)) > 0) || (SameType.Count(st => (st == Global.item_database.ArmorType.LIGHT)) > 0)))
                        return false;
                    return true;
                }
                return true;
            #endregion
        }
        public static byte GetAmmoSlot(character ch)
        {
            MsSQL ms = new MsSQL("SELECT * FROM char_items WHERE owner='" + ch.Information.CharacterID + "' AND (itemid='62' OR itemid='3655' OR itemid='10376' OR itemid='10727')");
            byte ammo = 0;
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    ammo = reader.GetByte(5);
                }
                ms.Close();
                return Convert.ToByte(ammo);
            }
        }

    }
}