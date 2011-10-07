///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
// File info: Public packet data
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
        public void StallOpen()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);

                if (Character.Stall.Stallactive || Character.State.Sitting || Character.Transport.Spawned || Character.Information.Scroll) return;


                Character.Network.Stall = new stall();

                Character.Network.Stall.ItemList = new List<stall.stallItem>();
                Character.Network.Stall.StallName = Reader.Text3();
                Character.Network.Stall.Members.Add(Character.Information.UniqueID);
                Character.Network.Stall.MembersClient.Add(this.client);
                Character.Network.Stall.ownerID = Character.Information.UniqueID;
                Character.Network.Stall.isOpened = false;


                client.Send(Packet.StallOpen(Character.Network.Stall.StallName, Character.Information.UniqueID, Character.Information.StallModel));
                client.Send(Packet.StallOpened());
                Send(Packet.StallOpenGlobal(Character.Network.Stall.StallName, Character.Information.UniqueID, Character.Information.StallModel));
                Character.Stall.Stallactive = true;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void StallClose()
        {
            try
            {
                client.Send(Packet.StallClose());
                Send(Packet.StallCloseGlobal(Character.Information.UniqueID));

                Character.Network.Stall.Dispose();
                Character.Network.Stall = null;
                Character.Stall.Stallactive = false;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void StallDeco(int itemid, byte slot)
        {
            try
            {
                int model = Data.ItemBase[itemid].ID;
                MsSQL.UpdateData("UPDATE character SET StallModel='" + model + "' WHERE name='" + Character.Information.Name + "'");
                PlayerDataLoad();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Stall deco error {0}",ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void StallMain()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte Type = Reader.Byte();

                //Item edit
                if (Type == 1)
                {
                    if (Character.Network.Stall.isOpened) return;
                    byte stallslot = Reader.Byte();
                    Reader.Skip(2);
                    ulong price = Reader.UInt64();
                    if (price <= 999999999)
                    {
                        int StallItemIndex = Character.Network.Stall.ItemList.FindIndex(i => (i.stallSlot == stallslot));
                        Character.Network.Stall.ItemList[StallItemIndex].price = price;

                        Character.Network.Stall.Send(Packet.StallModifyItem(stallslot, price));
                    }
                    else
                        return;
                }
                //Add an item
                else if (Type == 2)
                {
                    if (Character.Network.Stall.isOpened) return;
                    byte stallslot = Reader.Byte();
                    byte invpos = Reader.Byte();
                    short quantity = Reader.Int16();
                    ulong price = Reader.UInt64();
                    
                    Global.slotItem uItemID = GetItem((uint)Character.Information.CharacterID, invpos, 0);

                    //Disable item mall items in stalls for now.
                    if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.GLOBALCHAT ||
                        Data.ItemBase[uItemID.ID].Type == Global.item_database.ArmorType.AVATAR ||
                        Data.ItemBase[uItemID.ID].Type == Global.item_database.ArmorType.AVATARHAT ||
                        Data.ItemBase[uItemID.ID].Type == Global.item_database.ArmorType.AVATARATTACH
                        ) return;

                    if (quantity <= Data.ItemBase[uItemID.ID].Max_Stack)
                    {
                        stall.stallItem StallItem = new stall.stallItem();
                        LoadBluesid(uItemID.dbID);
                        StallItem.stallSlot = stallslot;
                        StallItem.price = price;
                        StallItem.Item = (GetItem((uint)Character.Information.CharacterID, invpos, 0));

                        if (Character.Network.Stall.ItemList.Exists((get => get.Item.dbID == StallItem.Item.dbID))) return;

                        Character.Network.Stall.ItemList.Add(StallItem);
                        Character.Network.Stall.Send(Packet.StallItemMain(Character.Network.Stall.ItemList));
                    }
                    else
                        return;
                }
                //Item pulling out
                else if (Type == 3)
                {
                        if (Character.Network.Stall.isOpened) return;
                        byte stallslot = Reader.Byte();
                        
                        //remove stallItem from stall
                        Character.Network.Stall.ItemList.Remove(Character.Network.Stall.ItemList.Find(i => (i.stallSlot == stallslot)));
                        Character.Network.Stall.Send(Packet.StallItemMain(Character.Network.Stall.ItemList));
                }
                //Stall modify state
                else if (Type == 5)
                {
                    byte State = Reader.Byte();
                    Character.Network.Stall.isOpened = (State == 1) ? true : false;

                    Character.Network.Stall.Send(Packet.StallSetState(State));
                }
                //Set Welcome msg
                else if (Type == 6)
                {
                    if (Character.Network.Stall.isOpened) return;
                    short length = Reader.Int16();
                    Character.Network.Stall.WelcomeMsg = Reader.Text3();
                    //Console.WriteLine("New Welcome msg:" + welcome);
                    Character.Network.Stall.Send(Packet.StallWelcome(Character.Network.Stall.WelcomeMsg));
                }
                //Set StallName
                else if (Type == 7)
                {
                    string stallname = Reader.Text3();
                    Send(Packet.StallNameGlobal(Character.Information.UniqueID, stallname));
                    Character.Network.Stall.Send(Packet.StallName(stallname));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Additem Stall error: {0}", ex);
            }
        }
        public void EnterStall()
        {
            try
            {
                if (Character.Information.Scroll) return;
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Systems staller = GetPlayer(Reader.Int32());

                Character.Network.Stall = staller.Character.Network.Stall;

                staller.Character.Network.Stall.Members.Add(Character.Information.UniqueID);
                staller.Character.Network.Stall.MembersClient.Add(this.client);

                client.Send(Packet.EnterStall(Character.Information.UniqueID, staller.Character.Network.Stall));

                staller.Character.Network.Stall.Send(Packet.StallPlayerUpdate(Character.Information.UniqueID, 2), this.client);
                Character.Stall.Stallactive = true;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void LeaveStall()
        {
            try
            {
                Character.Network.Stall.Members.Remove(this.Character.Information.UniqueID);
                Character.Network.Stall.MembersClient.Remove(this.client);

                client.Send(Packet.LeaveStall());
                Character.Network.Stall.Send(Packet.StallPlayerUpdate(this.Character.Information.UniqueID, 1), this.client);

                Character.Network.Stall.Dispose();
                Character.Network.Stall = null;
                Character.Stall.Stallactive = false;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void StallBuy()
        {
            try
            {
                if (!Character.Network.Stall.isOpened) return;
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte stallslot = Reader.Byte();

                stall currentStall = Character.Network.Stall;
                stall.stallItem sitem = currentStall.ItemList.Find(i => (i.stallSlot == stallslot));

                // stall buyer update
                byte slot = GetFreeSlot();
                if (slot <= 12) return;
                if (Character.Information.Gold >= (long)sitem.price)
                {
                    Character.Information.Gold -= (long)sitem.price;
                    client.Send(Packet.UpdateGold(Character.Information.Gold));
                    SaveGold();
                }
                else return; //insufficent gold

                // staller update
                if (currentStall.ItemList.Contains(sitem))
                {
                    Systems staller = GetPlayer(currentStall.ownerID);
                    staller.Character.Information.Gold += (long)sitem.price;
                    staller.client.Send(Packet.UpdateGold(staller.Character.Information.Gold));

                    MsSQL.UpdateData("update character set gold='" + staller.Character.Information.Gold + "' where id='" + staller.Character.Information.CharacterID + "'");
                    //MsSQL.UpdateData("delete from char_items where itemnumber='item" + sitem.Item.Slot + "' AND owner='" + staller.Character.Information.CharacterID + "'");
                    Systems.MsSQL.InsertData("UPDATE char_items SET owner='" + Character.Information.CharacterID + "',slot='" + slot + "',itemnumber='item" + slot + "' WHERE owner='" + staller.Character.Information.CharacterID + "' AND itemid='" + sitem.Item.ID + "' AND id='" + sitem.Item.dbID + "' AND storagetype='0'");
                    //take item out from stall
                    if (currentStall.ItemList.Count == 1)
                    {
                        staller.Character.Stall.Stallactive = false;
                        Character.Stall.Stallactive = false;
                    }
                    currentStall.ItemList.Remove(sitem);
                    client.Send(Packet.StallBuyItem(stallslot, 1));
                    currentStall.Send(Packet.StallBuyItem2(Character.Information.Name, stallslot, currentStall.ItemList));
                }
                else
                {
                    Disconnect("ban");
                    Console.WriteLine("Autobanned user: " + Player.AccountName + " Due to hacking");
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }            
        }
    }
}