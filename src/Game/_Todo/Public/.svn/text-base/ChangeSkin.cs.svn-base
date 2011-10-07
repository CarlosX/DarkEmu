///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
// File info: Public packet data
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        //##########################################
        // Character change skin scrolls
        //##########################################
        public void HandleSkinScroll(int skinmodel, int itemid)
        {
            try
            {
                //checks
                if (itemid == 0) return;
                //Set new skin model in database
                MsSQL.UpdateData("UPDATE character SET chartype='" + skinmodel + "' WHERE name='" + Character.Information.Name + "'");
                //Teleport user to same location (Test location normal return)
                PlayerDataLoad();
                Character.Information.Scroll = true;
                StartScrollTimer(0);
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        //##########################################
        // Item gender change
        //##########################################
        public void HandleItemChange(int itemid, byte slot, byte target_slot)
        {
            try
            {
                //Get information for the current item
                Global.slotItem iteminfo = GetItem((uint)Character.Information.CharacterID, target_slot, 0);
                Global.slotItem toolinfo = GetItem((uint)Character.Information.CharacterID, slot, 0);
                //Get item name
                string itemname = Data.ItemBase[iteminfo.ID].Name;

                //Checks before continuing (degree item).
                if (Data.ItemBase[toolinfo.ID].Name.Contains("_01"))
                {
                    if (Data.ItemBase[iteminfo.ID].Degree > 3) return;
                }
                else if (Data.ItemBase[toolinfo.ID].Name.Contains("_02"))
                {
                    if (Data.ItemBase[iteminfo.ID].Degree > 6 && Data.ItemBase[iteminfo.ID].Degree < 8) return;
                }
                else if (Data.ItemBase[toolinfo.ID].Name.Contains("_03"))
                {
                    if (Data.ItemBase[iteminfo.ID].Degree > 9 && Data.ItemBase[iteminfo.ID].Degree < 6) return;
                }
                else if (Data.ItemBase[toolinfo.ID].Name.Contains("_04"))
                {
                    if (Data.ItemBase[iteminfo.ID].Degree > 12 && Data.ItemBase[iteminfo.ID].Degree < 10) return;
                }
                //Rename the item to the opposite gender for getting the new id
                if (itemname.Contains("_M_"))
                    itemname = itemname.Replace("_M_", "_W_");
                else if (itemname.Contains("_W_"))
                    itemname = itemname.Replace("_W_", "_M_");
                //Return the new itemid value
                iteminfo.ID = GetGenderItem(itemname);
                //Send 1st packet
                client.Send(Packet.ChangeItemQ(target_slot, iteminfo.ID));
                //Remove the gender change item visually (amount).
                HandleUpdateSlotChange(target_slot, iteminfo, iteminfo.ID);
                //Need to refactor the packets for item move will do that later
                client.Send(Packet.MoveItem(target_slot, slot,0,0,0,"MOVE_GENDER_CHANGE"));
                //Need to check refresh info for the item. (Rest works).
            }
            catch (Exception ex)
            {
                Console.WriteLine("Item gender change error {0}", ex);
                Systems.Debugger.Write(ex);
            }

        }
        public int GetGenderItem(string itemname)
        {
            try
            {
                for (int i = 0; i < Data.ItemBase.Length; i++)
                {
                    if (Data.ItemBase[i] != null)
                    {
                        if (Data.ItemBase[i].Name == itemname)
                            return Data.ItemBase[i].ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return 0;
        }
    }
}
