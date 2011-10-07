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
        // Add items to database
        /////////////////////////////////////////////////////////////////////////////////
        public void AddItem(int itemid, short prob, byte slot, int id, int modelid)
        {
            #region Add item to db
            try
            {
                if (Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.BLADE ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.CH_SHIELD ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.EU_SHIELD ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.BOW ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.EU_AXE ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.EU_CROSSBOW ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.EU_HARP ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.EU_TSWORD ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.GLAVIE ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.SPEAR ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.SWORD ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.EARRING ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.RING ||
                     Data.ItemBase[itemid].Itemtype == Global.item_database.ItemType.NECKLACE ||
                     Data.ItemBase[itemid].Type == Global.item_database.ArmorType.ARMOR ||
                     Data.ItemBase[itemid].Type == Global.item_database.ArmorType.GARMENT ||
                     Data.ItemBase[itemid].Type == Global.item_database.ArmorType.GM ||
                     Data.ItemBase[itemid].Type == Global.item_database.ArmorType.HEAVY ||
                     Data.ItemBase[itemid].Type == Global.item_database.ArmorType.LIGHT ||
                     Data.ItemBase[itemid].Type == Global.item_database.ArmorType.PROTECTOR ||
                     Data.ItemBase[itemid].Type == Global.item_database.ArmorType.AVATAR ||
                     Data.ItemBase[itemid].Type == Global.item_database.ArmorType.ROBE)
                {
                    MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,storageacc,inavatar,storagetype,charbound) VALUES ('" + itemid + "','" + prob + "','" + Data.ItemBase[itemid].Defans.Durability + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','0','0','" + Data.ItemBase[itemid].SoulBound + "' )");

                }
                else if (Data.ItemBase[itemid].Pettype == Global.item_database.PetType.GRABPET)
                {
                    MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,storageacc,charbound) VALUES ('" + itemid + "','" + 0 + "','" + Data.ItemBase[itemid].Defans.Durability + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','" + Data.ItemBase[itemid].SoulBound + "')");

                    MsSQL ms = new MsSQL("SELECT TOP 1 * FROM char_items WHERE owner='" + Character.Information.CharacterID + "' ORDER BY id DESC");
                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            int idinfo = reader.GetInt32(0);
                            MsSQL.UpdateData("Insert Into pets (playerid, pet_type, pet_name, pet_state, pet_itemid, pet_active, pet_check, pet_unique) VALUES ('" + Character.Information.CharacterID + "','4','No name','1','" + itemid + "','0','item" + slot + "','" + idinfo + "')");
                        }
                    }
                    ms.Close();
                }
                else if (Data.ItemBase[itemid].Pettype == Global.item_database.PetType.ATTACKPET)
                {
                    MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,storageacc,charbound) VALUES ('" + itemid + "','" + 0 + "','" + Data.ItemBase[itemid].Defans.Durability + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','" + Data.ItemBase[itemid].SoulBound + "')");

                    MsSQL ms = new MsSQL("SELECT TOP 1 * FROM char_items WHERE owner='" + Character.Information.CharacterID + "' ORDER BY id DESC");
                    using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                    {
                        while (reader.Read())
                        {
                            int idinfo = reader.GetInt32(0);
                            MsSQL.UpdateData("Insert Into pets (playerid, pet_type, pet_name, pet_state, pet_itemid, pet_active, pet_check, pet_unique) VALUES ('" + Character.Information.CharacterID + "','2','No name','1','" + itemid + "','0','item" + slot + "','" + idinfo + "')");
                        }
                    }
                    ms.Close();
                }
                else if (Data.ItemBase[itemid].Etctype == Global.item_database.EtcType.STONES)
                {
                    MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,storageacc,inavatar,storagetype,charbound) VALUES ('" + itemid + "','" + 0 + "','" + 0 + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','0','0','" + Data.ItemBase[itemid].SoulBound + "' )");
                }
                else if (Data.ItemBase[itemid].Etctype == Global.item_database.EtcType.MONSTERMASK)
                {
                    MsSQL.UpdateData("Insert Into char_items (itemid,plusvalue,durability,owner,itemnumber,slot,storageacc,modelid,charbound) VALUES ('" + itemid + "','" + 0 + "','" + 0 + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','" + modelid + "','" + Data.ItemBase[itemid].SoulBound + "' )");
                }
                else
                {
                    if (prob < 2) prob = 1;
                    MsSQL.UpdateData("Insert Into char_items (itemid,quantity,owner,itemnumber,slot,storageacc,charbound) VALUES ('" + itemid + "','" + prob + "','" + id + "','item" + slot + "','" + slot + "','" + Player.ID + "','" + Data.ItemBase[itemid].SoulBound + "' )");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Item add error: ", ex);
            }
            #endregion
        }            
    }
}