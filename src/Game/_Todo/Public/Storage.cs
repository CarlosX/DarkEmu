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
        // Expand warehouse
        /////////////////////////////////////////////////////////////////////////////////
        void HandleWareHouse(int itemid)
        {
            try
            {
                //First we check the itemid (To make sure its not altered!).
                //TOD: Add anti item hack check(Class for itemid check).

                //If the user allready used one of these items before.
                if (Character.Account.StorageSlots > 151)
                {
                    //Need to check error message if item allready used.
                    //TMP message
                    client.Send(Packet.ChatPacket(7, Character.Information.UniqueID, "Allready expanded.", ""));
                }
                //If not continue to expand the storage
                else
                {
                    //We dont use reader info since we use static info 30 slots increase.
                    byte warehouseslots = Character.Account.StorageSlots += 30;
                    //Update sql database information
                    MsSQL.UpdateData("UPDATE users SET warehouse_slots='" + warehouseslots + "' WHERE id='" + Player.AccountName + "'");
                    //Need to check official message
                    client.Send(Packet.ChatPacket(7, Character.Information.UniqueID, "Your storage has been upgraded.", ""));
                    //Reload characters
                    PlayerDataLoad();
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Open Warehouse
        /////////////////////////////////////////////////////////////////////////////////
        protected void Open_Warehouse()
        {
            #region Open Warehouse
            try
            {
                client.Send(Packet.OpenWarehouse(Character.Account.StorageGold));
                client.Send(Packet.OpenWarehouse2(Character.Account.StorageSlots, this.Player));
                client.Send(Packet.OpenWarehouse3());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warehouse open error : {0}", ex);
            }
            #endregion
        }
    }
}
