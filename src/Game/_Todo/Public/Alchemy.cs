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
        Timer Alchemy;
        /////////////////////////////////////////////////////////////////////////////////
        // Create stones
        /////////////////////////////////////////////////////////////////////////////////
        public void AlchemyCreateStone()
        {
            try
            {
                //Open packet reader
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                byte type = reader.Byte();
                byte type1 = reader.Byte();
                byte type2 = reader.Byte();
                byte tabletslot = reader.Byte();
                byte elementslot1 = reader.Byte();
                byte elementslot2 = reader.Byte();
                byte elementslot3 = reader.Byte();
                byte elementslot4 = reader.Byte();
                reader.Close();

                //Tablet information
                Global.slotItem tabletslotitem = GetItem((uint)Character.Information.CharacterID, tabletslot, 0);
                
                //Get stone information equaly to the tablet
                int stone = GetStoneFromTablet(tabletslotitem.ID);
                
                //Earth element information
                Global.slotItem element1slotitem = GetItem((uint)Character.Information.CharacterID, elementslot1, 0);
                //Water element information
                Global.slotItem element2slotitem = GetItem((uint)Character.Information.CharacterID, elementslot2, 0);
                //Fire element information
                Global.slotItem element3slotitem = GetItem((uint)Character.Information.CharacterID, elementslot3, 0);
                //Wind element information
                Global.slotItem element4slotitem = GetItem((uint)Character.Information.CharacterID, elementslot4, 0);

                //Check if the requirements are ok (Extra check amount).
                if (element1slotitem.Amount < Data.ItemBase[tabletslotitem.ID].EARTH_ELEMENTS_AMOUNT_REQ) return;
                if (element2slotitem.Amount < Data.ItemBase[tabletslotitem.ID].WATER_ELEMENTS_AMOUNT_REQ) return;
                if (element3slotitem.Amount < Data.ItemBase[tabletslotitem.ID].FIRE_ELEMENTS_AMOUNT_REQ) return;
                if (element2slotitem.Amount < Data.ItemBase[tabletslotitem.ID].WIND_ELEMENTS_AMOUNT_REQ) return;
                
                //Check if the requirements are ok (Extra check element name).
                if (Data.ItemBase[element1slotitem.ID].Name != Data.ItemBase[tabletslotitem.ID].EARTH_ELEMENTS_NAME) return;
                if (Data.ItemBase[element2slotitem.ID].Name != Data.ItemBase[tabletslotitem.ID].WATER_ELEMENTS_NAME) return;
                if (Data.ItemBase[element3slotitem.ID].Name != Data.ItemBase[tabletslotitem.ID].FIRE_ELEMENTS_NAME) return;
                if (Data.ItemBase[element4slotitem.ID].Name != Data.ItemBase[tabletslotitem.ID].WIND_ELEMENTS_NAME) return;

                //Update amount of elements
                element1slotitem.Amount -= (short)Data.ItemBase[tabletslotitem.ID].EARTH_ELEMENTS_AMOUNT_REQ;
                element2slotitem.Amount -= (short)Data.ItemBase[tabletslotitem.ID].WATER_ELEMENTS_AMOUNT_REQ;
                element3slotitem.Amount -= (short)Data.ItemBase[tabletslotitem.ID].FIRE_ELEMENTS_AMOUNT_REQ;
                element4slotitem.Amount -= (short)Data.ItemBase[tabletslotitem.ID].WIND_ELEMENTS_AMOUNT_REQ;

                ItemUpdateAmount(element1slotitem, Character.Information.CharacterID);
                ItemUpdateAmount(element2slotitem, Character.Information.CharacterID);
                ItemUpdateAmount(element3slotitem, Character.Information.CharacterID);
                ItemUpdateAmount(element4slotitem, Character.Information.CharacterID);

                //Update amount of tablet
                tabletslotitem.Amount -= 1;
                ItemUpdateAmount(tabletslotitem, Character.Information.CharacterID);

                //Send alchemy packet
                client.Send(Packet.StoneCreation(tabletslot));

                //Check for new free slots in inventory
                byte freeslot = GetFreeSlot();
                //Update database and insert new item
                AddItem(stone, 1, freeslot, Character.Information.CharacterID, 0);
                //Send visual packet add stone (creation works, just need to check why it sends 2x same packet).
                client.Send(Packet.GainElements(freeslot, stone, 1));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Stone creation error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Elixir alchemy
        /////////////////////////////////////////////////////////////////////////////////
        public void AlchemyElixirMain()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                this.Character.Alchemy.ItemList = new List<Global.slotItem>();

                byte Type = Reader.Byte();

                if (Type == 1)
                {
                    try
                    {
                        this.Character.Alchemy.AlchemyThread.Abort();
                        this.client.Send(Packet.AlchemyCancel());
                    }
                    catch (Exception ex)
                    {
                        Systems.Debugger.Write(ex);
                    }
                }
                else if (Type == 2)
                {
                    Reader.Skip(1);
                    byte numItem = Reader.Byte();

                    if (numItem == 2)
                    {
                        this.Character.Alchemy.ItemList.Add(GetItem((uint)this.Character.Information.CharacterID, Reader.Byte(), 0));
                        this.Character.Alchemy.ItemList.Add(GetItem((uint)this.Character.Information.CharacterID, Reader.Byte(), 0));

                    }
                    else if (numItem == 3)
                    {
                        this.Character.Alchemy.ItemList.Add(GetItem((uint)this.Character.Information.CharacterID, Reader.Byte(), 0));
                        this.Character.Alchemy.ItemList.Add(GetItem((uint)this.Character.Information.CharacterID, Reader.Byte(), 0));
                        this.Character.Alchemy.ItemList.Add(GetItem((uint)this.Character.Information.CharacterID, Reader.Byte(), 0));
                    }
                    Alchemy = new Timer(new TimerCallback(StartAlchemyElixirResponse), 0, 3000, 0);
                }
            }
            catch (Exception ex)
            {
               Systems.Debugger.Write(ex);
            }
        }
        public void StartAlchemyElixirResponse(object e)
        {
            try
            {
                this.Character.Alchemy.AlchemyThread = new Thread(new ThreadStart(AlchemyElixirResponse));
                this.Character.Alchemy.AlchemyThread.Start();
                while (!this.Character.Alchemy.AlchemyThread.IsAlive) ;
                Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void AlchemyElixirResponse()
        {
            try
            {
                int chance = 0;
                bool success = false;
                Random plus = new Random();
                int random = plus.Next(0, this.Character.Blues.Luck); // Luck's default value = 100
                // successrate table
                switch (this.Character.Alchemy.ItemList[0].PlusValue)
                {
                    case 0: chance = this.Character.Blues.Luck - 95;
                        break;
                    case 1: chance = this.Character.Blues.Luck - 75;
                        break;
                    case 2: chance = this.Character.Blues.Luck - 63;
                        break;
                    case 3: chance = this.Character.Blues.Luck - 50;
                        break;
                    case 4: chance = this.Character.Blues.Luck - 38;
                        break;
                    case 5: chance = this.Character.Blues.Luck - 31;
                        break;
                    case 6: chance = this.Character.Blues.Luck - 27;
                        break;
                    case 7: chance = this.Character.Blues.Luck - 21;
                        break;
                    case 8: chance = this.Character.Blues.Luck - 13;
                        break;
                    case 9: chance = this.Character.Blues.Luck - 8;
                        break;
                    default: chance = this.Character.Blues.Luck - 5;
                        break;
                }

                // if with lucky
                if (this.Character.Alchemy.ItemList.Count == 3)
                {
                    chance -= 5; // +5% :))
                    // dec lucky powder amount
                    this.Character.Alchemy.ItemList[2].Amount--;
                    ItemUpdateAmount(this.Character.Alchemy.ItemList[2], this.Character.Information.CharacterID);
                }
                // success or not :P
                if (random > chance)
                    success = true;
                // update plus value
                if (success)
                {
                    this.Character.Alchemy.ItemList[0].PlusValue++;
                    MsSQL.InsertData("UPDATE char_items SET plusvalue='" + this.Character.Alchemy.ItemList[0].PlusValue + "' WHERE slot='" + this.Character.Alchemy.ItemList[0].Slot + "' AND owner='" + this.Character.Information.CharacterID + "'");
                }
                else
                {
                    if (this.Character.Alchemy.ItemList[0].PlusValue >= 4)
                    {
                        if (Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].blue.Contains("MATTR_ASTRAL"))
                        {
                            this.Character.Alchemy.ItemList[0].PlusValue = 4;
                        }
                        else
                        {
                            this.Character.Alchemy.ItemList[0].PlusValue = 0;
                        }
                    }
                    else
                    {
                        this.Character.Alchemy.ItemList[0].PlusValue = 0;
                    }
                        MsSQL.InsertData("UPDATE char_items SET plusvalue='0' WHERE slot ='" + this.Character.Alchemy.ItemList[0].Slot + "' AND owner='" + this.Character.Information.CharacterID + "'");
                }
                this.client.Send(Packet.AlchemyResponse(success, this.Character.Alchemy.ItemList[0], 1, (byte)(Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].totalblue)));
                //delete elixir
                MsSQL.InsertData("DELETE FROM char_items WHERE slot='" + this.Character.Alchemy.ItemList[1].Slot + "' AND owner='" + this.Character.Information.CharacterID + "'");
                this.client.Send(Packet.MoveItem(0x0F, this.Character.Alchemy.ItemList[1].Slot, 0, 0, 0, "DELETE_ITEM"));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Alchemy Error: {0}",ex);
                Systems.Debugger.Write(ex);
            }

        }
        /////////////////////////////////////////////////////////////////////////////////
        // Item reinforce with stones
        /////////////////////////////////////////////////////////////////////////////////
        public void AlchemyStoneMain()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Character.Alchemy.ItemList = new List<Global.slotItem>();
                byte type = Reader.Byte();
                if (type == 1)
                {
                    try
                    {
                        this.Character.Alchemy.AlchemyThread.Abort();
                        this.client.Send(Packet.AlchemyCancel());
                    }
                    catch (Exception ex)
                    {
                        Systems.Debugger.Write(ex);
                    }
                }
                else if (type == 2)
                {
                    Reader.Skip(1);
                    byte numitem = Reader.Byte();
                    this.Character.Alchemy.ItemList.Add(GetItem((uint)this.Character.Information.CharacterID, Reader.Byte(), 0));
                    this.Character.Alchemy.ItemList.Add(GetItem((uint)this.Character.Information.CharacterID, Reader.Byte(), 0));

                }
                Alchemy = new Timer(new TimerCallback(StartAlchemyStoneResponse), 0, 3000, 0);
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void StartAlchemyStoneResponse(object e)
        {
            try
            {
                this.Character.Alchemy.AlchemyThread = new Thread(new ThreadStart(AlchemyStoneResponse));
                this.Character.Alchemy.AlchemyThread.Start();
                while (!this.Character.Alchemy.AlchemyThread.IsAlive) ;
                Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void AlchemyStoneResponse()
        {
            try
            {
                Random rnd = new Random();
                int random = rnd.Next(1, 100);
                bool success = true;
                LoadBluesid(this.Character.Alchemy.ItemList[0].dbID);
                if (random <= 70)
                {
                    success = true;
                }
                else
                {
                    success = false;
                }

                if (success)
                {
                    if (Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].totalblue <= Data.ItemBase[this.Character.Alchemy.ItemList[0].ID].MaxBlueAmount)
                    {
                        Random blue = new Random();
                        int min = Data.MagicOptions.Find(aa => (aa.Name == Data.ItemBase[this.Character.Alchemy.ItemList[1].ID].ObjectName) && aa.Level == GetItemDegree(this.Character.Alchemy.ItemList[0]) + 1).MinValue;
                        int max = Data.MagicOptions.Find(aa => (aa.Name == Data.ItemBase[this.Character.Alchemy.ItemList[1].ID].ObjectName) && aa.Level == GetItemDegree(this.Character.Alchemy.ItemList[0]) + 1).MaxValue;
                        int value = blue.Next(min, max);
                        if (Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].blue.Contains(Data.ItemBase[this.Character.Alchemy.ItemList[1].ID].ObjectName))
                        {
                            int index = Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].blue.IndexOf(Data.ItemBase[this.Character.Alchemy.ItemList[1].ID].ObjectName);
                            index++;
                            MsSQL.UpdateData("UPDATE char_items SET blue" + index + "amount='" + value + "' WHERE id='" + this.Character.Alchemy.ItemList[0].dbID + "'");
                        }
                        else
                        {
                            Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].totalblue++;
                            MsSQL.UpdateData("UPDATE char_items SET BlueAmount='" + Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].totalblue + "',blue" + Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].totalblue + "='" + Data.ItemBase[this.Character.Alchemy.ItemList[1].ID].ObjectName + "',blue" + Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].totalblue + "amount='" + value + "' WHERE id='" + this.Character.Alchemy.ItemList[0].dbID + "'");
                        }
                    }
                    else
                        return;
                }

                LoadBluesid(this.Character.Alchemy.ItemList[0].dbID);
                if (Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].blue.Contains("MATTR_DUR"))
                {
                    this.Character.Alchemy.ItemList[0].Durability += this.Character.Alchemy.ItemList[0].Durability * ((Int32)(Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].blue[Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].blue.IndexOf("MATTR_DUR")]) / 100);
                }
                if (Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].blue.Contains("MATTR_REINFORCE_ITEM"))
                {
                    this.Character.Alchemy.ItemList[0].PlusValue += (byte)(Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].blue[Data.ItemBlue[this.Character.Alchemy.ItemList[0].dbID].blue.IndexOf("MATTR_REINFORCE_ITEM")]);
                }
                MsSQL.InsertData("UPDATE char_items SET durability='" + this.Character.Alchemy.ItemList[0].Durability + "',plusvalue='" + this.Character.Alchemy.ItemList[0].PlusValue + "' WHERE id='" + this.Character.Alchemy.ItemList[0].dbID + "'");
                this.client.Send(Packet.AlchemyStoneResponse(success, this.Character.Alchemy.ItemList[0]));
                MsSQL.InsertData("DELETE FROM char_items WHERE slot='" + this.Character.Alchemy.ItemList[1].Slot + "' AND owner='" + this.Character.Information.CharacterID + "'");
                this.client.Send(Packet.MoveItem(0x0F, this.Character.Alchemy.ItemList[1].Slot, 0, 0, 0, "DELETE_ITEM"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Alchemy Terminated...");
                Systems.Debugger.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get degree information
        /////////////////////////////////////////////////////////////////////////////////
        public int GetItemDegree(Global.slotItem item)
        {
            try
            {
                if (1 <= Data.ItemBase[item.ID].Level && Data.ItemBase[item.ID].Level < 8)
                    return 1;
                else if (8 <= Data.ItemBase[item.ID].Level && Data.ItemBase[item.ID].Level < 16)
                    return 2;
                else if (16 <= Data.ItemBase[item.ID].Level && Data.ItemBase[item.ID].Level < 24)
                    return 3;
                else if (24 <= Data.ItemBase[item.ID].Level && Data.ItemBase[item.ID].Level < 32)
                    return 4;
                else if (32 <= Data.ItemBase[item.ID].Level && Data.ItemBase[item.ID].Level < 42)
                    return 5;
                else if (42 <= Data.ItemBase[item.ID].Level && Data.ItemBase[item.ID].Level < 52)
                    return 6;
                else if (52 <= Data.ItemBase[item.ID].Level && Data.ItemBase[item.ID].Level < 64)
                    return 7;
                else if (64 <= Data.ItemBase[item.ID].Level && Data.ItemBase[item.ID].Level < 76)
                    return 8;
                else if (76 <= Data.ItemBase[item.ID].Level && Data.ItemBase[item.ID].Level < 90)
                    return 9;
                else if (90 <= Data.ItemBase[item.ID].Level && Data.ItemBase[item.ID].Level < 101)
                    return 10;
                else if (101 <= Data.ItemBase[item.ID].Level && Data.ItemBase[item.ID].Level < 110)
                    return 11;
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return 1;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Item destruction with alchemy
        /////////////////////////////////////////////////////////////////////////////////
        public void BreakItem()
        {
            try
            {
                //Checks before we continue
                if (Character.Stall.Stallactive || Character.Action.nAttack || Character.Action.sAttack || Character.Alchemy.working) 
                    return;
                //Set bool
                Character.Alchemy.working = true;
                //TODO: Timer for alchemy start / end
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                byte rondorequired = reader.Byte();
                byte slot = reader.Byte();
                reader.Close();


                //Get our item information (item)
                Global.slotItem item = GetItem((uint)Character.Information.CharacterID, slot, 0);
                
                //Get our degree information
                byte itemdegree = Data.ItemBase[item.ID].Degree;

                //First we get our elements (Same degree as weapon)
                //This should return 4 items
                //Add check rondo count if high enough.
                Character.Alchemy.Elementlist = GetDegreeElements(item.ID, Character);
                //Check if the item has any blues on it.
                if (Data.ItemBlue[item.dbID].totalblue != 0)
                    Character.Alchemy.StonesList = GetStonesDegree(item.ID, Character);
                
                //Check current free slots of the player
                byte slotcheck = GetFreeSlot();
                //If slot amount is lower then 4 return
                //Slots free must be 6 i believe because of stones (TODO: Check info official).
                if (slotcheck < 4)
                {
                    //Send error message inventory full ...
                    return;
                }
                //Player has enough slots so we continue adding the new items
                else
                {
                    //Update rondo quantity
                    Character.Information.InventorylistSlot = GetPlayerItems(Character);
                    foreach (byte e in Character.Information.InventorylistSlot)
                    {
                        //Set slotitem
                        Global.slotItem itemrondoinfo = GetItem((uint)Character.Information.CharacterID, e, 0);
                        if (itemrondoinfo.ID != 0)
                        {
                            if (Data.ItemBase[itemrondoinfo.ID].Etctype == Global.item_database.EtcType.DESTROYER_RONDO)
                            {
                                //Update amount
                                itemrondoinfo.Amount -= rondorequired;
                                ItemUpdateAmount(itemrondoinfo, Character.Information.CharacterID);
                            }
                        }
                    }
                    //Clean our list
                    Character.Information.InventorylistSlot.Clear();
                    //Remove the item used in dissembling (Query).
                    MsSQL.DeleteData("DELETE FROM char_items WHERE id='"+ item.dbID +"' AND owner='"+ Character.Information.CharacterID +"'");
                    //Remove the item used in dissembling (Visual).
                    ItemUpdateAmount(item, Character.Information.CharacterID);
                    //Send packet #2 
                    client.Send(Packet.DestroyItem());
                    //Repeat for each element in our list.
                    foreach (int e in Character.Alchemy.Elementlist)
                    {
                        if (e != 0)
                        {
                            //TODO: Make detailed randoms
                            //Make random add count for the elements
                            //NOTE: Check what item has what element on destruction. if pk2 contains or not.
                            int elementamount = 0;
                            
                            #region Amounts
                            if (Data.ItemBase[item.ID].Degree == 1)
                                elementamount = rnd.Next(1, 60);
                            else if (Data.ItemBase[item.ID].Degree == 2)
                                elementamount = rnd.Next(1, 90);
                            else if (Data.ItemBase[item.ID].Degree == 3)
                                elementamount = rnd.Next(1, 120);
                            else if (Data.ItemBase[item.ID].Degree == 4)
                                elementamount = rnd.Next(1, 150);
                            else if (Data.ItemBase[item.ID].Degree == 5)
                                elementamount = rnd.Next(1, 200);
                            else if (Data.ItemBase[item.ID].Degree == 6)
                                elementamount = rnd.Next(1, 250);
                            else if (Data.ItemBase[item.ID].Degree == 7)
                                elementamount = rnd.Next(1, 300);
                            else if (Data.ItemBase[item.ID].Degree == 8)
                                elementamount = rnd.Next(1, 375);
                            else if (Data.ItemBase[item.ID].Degree == 9)
                                elementamount = rnd.Next(1, 450);
                            else if (Data.ItemBase[item.ID].Degree == 10)
                                elementamount = rnd.Next(1, 600);
                            else if (Data.ItemBase[item.ID].Degree == 11)
                                elementamount = rnd.Next(1, 800);
                            #endregion

                            int stoneamount = 0;

                            #region Stones
                            if (Data.ItemBlue[item.dbID].totalblue != 0)
                            {
                                if (Data.ItemBlue[item.dbID].totalblue == 1)
                                    stoneamount = rnd.Next(0, 1);
                                else if (Data.ItemBlue[item.dbID].totalblue == 2)
                                    stoneamount = rnd.Next(0, 2);
                            }
                            #endregion

                            slotcheck = GetFreeSlot();
                            //Stack items todo
                            AddItem(Data.ItemBase[e].ID, 10, slotcheck, Character.Information.CharacterID, 0);
                            client.Send(Packet.GainElements(slotcheck, Data.ItemBase[e].ID, (short)elementamount));
                        }
                    }
                    //Clear created list content.
                    Character.Alchemy.Elementlist.Clear();
                }
                //Reset bool
                Character.Alchemy.working = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Alchemy error destroyer {0}",ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get matching degree elements for item
        /////////////////////////////////////////////////////////////////////////////////
        public List<int> GetDegreeElements(int itemid, character c)
        {
            try
            {
                List<int> elements = new List<int>();
                for (int i = 0; i < Data.ItemBase.Length; i++)
                {
                    if (Data.ItemBase[i] != null)
                    {
                        if (Data.ItemBase[i].Etctype == Global.item_database.EtcType.ELEMENTS && Data.ItemBase[i].Degree == Data.ItemBase[itemid].Degree)
                        {
                            if (i != 0)
                                elements.Add(i);
                        }
                    }
                }
                return elements;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return null;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get matching degree stones for item
        /////////////////////////////////////////////////////////////////////////////////
        public List<int> GetStonesDegree(int itemid, character c)
        {
            try
            {
                List<int> stones = new List<int>();
                for (int i = 0; i < Data.ItemBase.Length; i++)
                {
                    if (Data.ItemBase[i] != null)
                    {
                        if (Data.ItemBase[i].Etctype == Global.item_database.EtcType.STONES && Data.ItemBase[i].Degree == Data.ItemBase[itemid].Degree)
                        {
                            if (i != 0)
                                stones.Add(i);
                        }
                    }
                }
                return stones;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return null;
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Get stone from tablet information
        /////////////////////////////////////////////////////////////////////////////////
        public int GetStoneFromTablet(int itemid)
        {
            try
            {
                for (int i = 0; i < Data.ItemBase.Length; i++)
                {
                    if (Data.ItemBase[i] != null)
                    {
                        if (Data.ItemBase[i].Name == Data.ItemBase[itemid].StoneName)
                            return Data.ItemBase[i].ID;
                    }

                }
                return 0;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return 0;
        }
    }
}
