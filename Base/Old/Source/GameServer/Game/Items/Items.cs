/*    <DarkEmu GameServer>
    Copyright (C) <2011>  <DarkEmu>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace DarkEmu_GameServer
{
    class Items
    {
        private static Random random = new Random();

        public static void CreateItem(PacketReader reader_, int Index_)
        {
            int[] CharacterItemIndex = DatabaseCore.Item.GetIndexByName(Player.General[Index_].CharacterName);

            byte freeslot = FreeSlot(CharacterItemIndex);

            uint itemid = reader_.ReadDword();
            byte itemplus = reader_.ReadByte();

            Silkroad.Item_ DestinationItem_ = Silkroad.GetItemById(itemid);

            if (DestinationItem_.ITEM_TYPE_NAME.Contains("ITEM_ETC_GOLD_"))
                freeslot = 254;

            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_MOVEMENT);
            writer.AppendWord(0x601);
            writer.AppendByte(freeslot);
            if (freeslot != 254)
                writer.AppendDword(itemid);
            if (DestinationItem_.ITEM_TYPE_NAME.Contains("CH") || DestinationItem_.ITEM_TYPE_NAME.Contains("EU"))
            {
                writer.AppendByte(itemplus);
                writer.AppendLword(0);
                writer.AppendDword((uint)DestinationItem_.MIN_DURA);
                writer.AppendByte(0);

                DatabaseCore.WriteQuery("UPDATE items SET itemid='{0}',plusvalue='{1}',type='0',durability='{2}' WHERE itemnumber='item{3}' AND owner ='{4}'", itemid, itemplus, DestinationItem_.MIN_DURA, freeslot, Player.General[Index_].CharacterName);
                AddItemToDatabase(CharacterItemIndex[freeslot], itemid, 0, 0, (byte)DestinationItem_.MIN_DURA, itemplus, 0);

            }
            else if (DestinationItem_.ITEM_TYPE_NAME.Contains("ETC"))
            {
                if (freeslot != 254)
                {
                    writer.AppendWord(itemplus);
                    DatabaseCore.WriteQuery("UPDATE items SET itemid='{0}',plusvalue='0',type='1',durability='{1}',quantity='{2}' WHERE itemnumber='item{3}' AND owner ='{4}'", itemid, DestinationItem_.MIN_DURA, itemplus, freeslot, Player.General[Index_].CharacterName);
                    AddItemToDatabase(CharacterItemIndex[freeslot], itemid, 1, itemplus, (byte)DestinationItem_.MIN_DURA, 0, 0);
                }
            }

            ServerSocket.Send(writer.getWorkspace(), Index_);
        }

        private static bool CheckItemGender(Silkroad.Item_ tmpItem, int Index_)
        {
            int Gender = 0;

            if ((Player.Stats[Index_].Model >= 1907 && Player.Stats[Index_].Model <= 1919) || (Player.Stats[Index_].Model >= 14717 && Player.Stats[Index_].Model <= 14729))
                Gender = 1;
            if ((Player.Stats[Index_].Model >= 1920 && Player.Stats[Index_].Model <= 1932) || (Player.Stats[Index_].Model >= 14730 && Player.Stats[Index_].Model <= 14742))
                Gender = 0;

            if (Gender == tmpItem.GENDER || tmpItem.GENDER == 2)
                return true;
            else
                return false;
        }

        private static bool CheckItemLevel(Silkroad.Item_ tmpItem, int Index_)
        {
            return tmpItem.LV_REQ <= Player.Stats[Index_].Level;
        }

        public static void AddItemToDatabase(int CharacterItemIndexDestination, uint model, byte type, byte quantity, byte durability, byte plusvalue, byte blueamount)
        {
            DatabaseCore.Item.ItemId[CharacterItemIndexDestination] = model;
            DatabaseCore.Item.Type[CharacterItemIndexDestination] = type;
            DatabaseCore.Item.Quantity[CharacterItemIndexDestination] = quantity;
            DatabaseCore.Item.Durability[CharacterItemIndexDestination] = durability;
            DatabaseCore.Item.PlusValue[CharacterItemIndexDestination] = plusvalue;
            DatabaseCore.Item.BlueAmount[CharacterItemIndexDestination] = blueamount;
        }

        public static void DeleteFromDatabase(int CharacterItemIndexSource)
        {
            DatabaseCore.Item.ItemId[CharacterItemIndexSource] = 0;
            DatabaseCore.Item.Type[CharacterItemIndexSource] = 0;
            DatabaseCore.Item.Quantity[CharacterItemIndexSource] = 0;
            DatabaseCore.Item.Durability[CharacterItemIndexSource] = 30;
            DatabaseCore.Item.BlueAmount[CharacterItemIndexSource] = 0;
            DatabaseCore.Item.Blue[CharacterItemIndexSource] = new DatabaseCore.Item_.Blue_();
        }

        public static void MoveItemToDatabase(int CharacterItemIndexSource, int CharacterItemIndexDestination, string CharacterName)
        {
            uint tmpItemId = DatabaseCore.Item.ItemId[CharacterItemIndexSource];
            byte tmpType = DatabaseCore.Item.Type[CharacterItemIndexSource];
            byte tmpQuantity = DatabaseCore.Item.Quantity[CharacterItemIndexSource];
            byte tmpDurability = DatabaseCore.Item.Durability[CharacterItemIndexSource];
            byte tmpBlueAmount = DatabaseCore.Item.BlueAmount[CharacterItemIndexSource];
            DatabaseCore.Item_.Blue_ tmpBlue = DatabaseCore.Item.Blue[CharacterItemIndexSource];

            DatabaseCore.Item.ItemId[CharacterItemIndexSource] = DatabaseCore.Item.ItemId[CharacterItemIndexDestination];
            DatabaseCore.Item.Type[CharacterItemIndexSource] = DatabaseCore.Item.Type[CharacterItemIndexDestination];
            DatabaseCore.Item.Quantity[CharacterItemIndexSource] = DatabaseCore.Item.Quantity[CharacterItemIndexDestination];
            DatabaseCore.Item.Durability[CharacterItemIndexSource] = DatabaseCore.Item.Durability[CharacterItemIndexDestination];
            DatabaseCore.Item.BlueAmount[CharacterItemIndexSource] = DatabaseCore.Item.BlueAmount[CharacterItemIndexDestination];
            DatabaseCore.Item.Blue[CharacterItemIndexSource] = DatabaseCore.Item.Blue[CharacterItemIndexDestination];

            DatabaseCore.Item.ItemId[CharacterItemIndexDestination] = tmpItemId;
            DatabaseCore.Item.Type[CharacterItemIndexDestination] = tmpType;
            DatabaseCore.Item.Quantity[CharacterItemIndexDestination] = tmpQuantity;
            DatabaseCore.Item.Durability[CharacterItemIndexDestination] = tmpDurability;
            DatabaseCore.Item.BlueAmount[CharacterItemIndexDestination] = tmpBlueAmount;
            DatabaseCore.Item.Blue[CharacterItemIndexDestination] = tmpBlue;

            DatabaseCore.WriteQuery("UPDATE items SET itemid='{0}',type='{1}',quantity='{2}',durability='{3}',BlueAmount='{4}' WHERE slot='{5}' AND owner='{6}'", DatabaseCore.Item.ItemId[CharacterItemIndexDestination], DatabaseCore.Item.Type[CharacterItemIndexDestination], DatabaseCore.Item.Quantity[CharacterItemIndexDestination], DatabaseCore.Item.Durability[CharacterItemIndexDestination], DatabaseCore.Item.BlueAmount[CharacterItemIndexDestination], DatabaseCore.Item.Slot[CharacterItemIndexDestination], CharacterName);
            DatabaseCore.WriteQuery("UPDATE items SET itemid='{0}',type='{1}',quantity='{2}',durability='{3}',BlueAmount='{4}' WHERE slot='{5}' AND owner='{6}'", DatabaseCore.Item.ItemId[CharacterItemIndexSource], DatabaseCore.Item.Type[CharacterItemIndexSource], DatabaseCore.Item.Quantity[CharacterItemIndexSource], DatabaseCore.Item.Durability[CharacterItemIndexSource], DatabaseCore.Item.BlueAmount[CharacterItemIndexSource], DatabaseCore.Item.Slot[CharacterItemIndexSource], CharacterName);

            for (uint j = 1; j < 9; j++)
            {
                DatabaseCore.WriteQuery("UPDATE items SET blue{0}='{1}',blue{0}amount='{2}' WHERE slot='{3}' AND owner='{4}'", j, DatabaseCore.Item.Blue[CharacterItemIndexDestination].BlueAmount[j], DatabaseCore.Item.Blue[CharacterItemIndexDestination].Blue[j], DatabaseCore.Item.Slot[CharacterItemIndexDestination], CharacterName);
                DatabaseCore.WriteQuery("UPDATE items SET blue{0}='{1}',blue{0}amount='{2}' WHERE slot='{3}' AND owner='{4}'", j, DatabaseCore.Item.Blue[CharacterItemIndexSource].BlueAmount[j], DatabaseCore.Item.Blue[CharacterItemIndexSource].Blue[j], DatabaseCore.Item.Slot[CharacterItemIndexSource], CharacterName);
            }
        }

        public static byte FreeSlot(int[] CharacterItemIndex)
        {
            for (byte i = 13; i < CharacterItemIndex.Length; i++)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemIndex[i]] == 0) return i;
            }
            return 0;
        }

        unsafe public static void MoveItem(byte* ptr, int Index_)
        {
            Silkroad.C_S.MOVE_ITEM* tmpPtr = (Silkroad.C_S.MOVE_ITEM*)ptr;

            PacketWriter writer = new PacketWriter();
            switch (tmpPtr->Type)
            {
                case 0:

                    int[] CharacterItemIndex = DatabaseCore.Item.GetIndexByName(Player.General[Index_].CharacterName);
                    Silkroad.Item_ SourceItem_ = Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterItemIndex[tmpPtr->Source]]);
                    Silkroad.Item_ DestinationItem_ = Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterItemIndex[tmpPtr->Destination]]);

                    if (SourceItem_.ITEM_TYPE_NAME.Contains("FRPVP_VOUCHER"))
                    {
                        if (Player.Stats[Index_].Level >= 10 && CheckItemGender(SourceItem_, Index_))
                        {
                            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ANIMATION_CAPE);
                            writer.AppendDword(Player.General[Index_].UniqueID);
                            writer.AppendWord(0x102);
                            writer.AppendByte(0xA);
                
                            ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);
                            Timers.UsingItemTimer[Index_].Interval = 10000;
                            Player.Objects[Index_].SourceItemIndex = CharacterItemIndex[tmpPtr->Source];
                            Player.Objects[Index_].DestinationItemIndex = CharacterItemIndex[tmpPtr->Destination];
                            Timers.UsingItemTimer[Index_].Start();
                        }
                        else
                        {
                            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_MOVEMENT);
                            writer.AppendWord(0x1002);
                            ServerSocket.Send(writer.getWorkspace(), Index_);
                        }
                    }
                    else
                    {
                        if (tmpPtr->Destination < 13)
                        {
                            if (!SourceItem_.ITEM_TYPE_NAME.Contains("ETC"))
                            {
                                if (!CheckItemGender(SourceItem_, Index_))
                                {
                                    writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_MOVEMENT);
                                    writer.AppendWord(0x1602);
                                    ServerSocket.Send(writer.getWorkspace(), Index_);
                                    return;
                                }
                                else if (!CheckItemLevel(SourceItem_, Index_))
                                {
                                    writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_MOVEMENT);
                                    writer.AppendWord(0x1002);
                                    ServerSocket.Send(writer.getWorkspace(), Index_);
                                    return;
                                }

                                if (tmpPtr->Destination == 6)
                                    Player.General[Index_].WeaponType = SourceItem_.CLASS_C;

                                Silkroad.Item_ WeaponItem = Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterItemIndex[6]]);

                                Console.WriteLine(WeaponItem.CLASS_C);
                                if (tmpPtr->Destination == 6 && DatabaseCore.Item.ItemId[CharacterItemIndex[7]] != 0 && (Player.General[Index_].WeaponType != 2 && Player.General[Index_].WeaponType != 3 && Player.General[Index_].WeaponType != 7 && Player.General[Index_].WeaponType != 10 && Player.General[Index_].WeaponType != 15))
                                {
                                    byte unequipped = UnEnquipShield(Index_, CharacterItemIndex, CharacterItemIndex[7]);
                                    if (unequipped != 255)
                                        MoveItemToDatabase(CharacterItemIndex[7], unequipped, Player.General[Index_].CharacterName);
                                }

                                if (tmpPtr->Destination == 7 && DatabaseCore.Item.ItemId[CharacterItemIndex[6]] != 0 && (WeaponItem.CLASS_C != 2 && WeaponItem.CLASS_C != 3 && WeaponItem.CLASS_C != 7 && WeaponItem.CLASS_C != 10 && WeaponItem.CLASS_C != 15))
                                {
                                    byte unequipped = UnEnquipWeapon(Index_, CharacterItemIndex, CharacterItemIndex[6]);
                                    if (unequipped != 255)
                                        MoveItemToDatabase(CharacterItemIndex[6], unequipped, Player.General[Index_].CharacterName);
                                }

                                EquipItem(Index_, CharacterItemIndex[tmpPtr->Source], CharacterItemIndex[tmpPtr->Destination]);
                            }
                            else
                            {
                                if (!(tmpPtr->Destination == 7 && (Player.General[Index_].WeaponType == 6 || Player.General[Index_].WeaponType == 12) && DatabaseCore.Item.ItemId[CharacterItemIndex[7]] != 0))
                                    return;
                            }
                        }
                        if (tmpPtr->Source < 13)
                            UnEquipItem(Index_, CharacterItemIndex[tmpPtr->Source], CharacterItemIndex[tmpPtr->Destination]);

                        writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_MOVEMENT);
                        writer.AppendByte(1);
                        writer.AppendByte(tmpPtr->Type);
                        writer.AppendByte(tmpPtr->Source);
                        writer.AppendByte(tmpPtr->Destination);
                        writer.AppendWord(tmpPtr->Amount);
                        writer.AppendByte(0);
                        ServerSocket.Send(writer.getWorkspace(), Index_);

                        MoveItemToDatabase(CharacterItemIndex[tmpPtr->Source], CharacterItemIndex[tmpPtr->Destination], Player.General[Index_].CharacterName);
                    }
                    Stats.OnStatPacket(Index_);
                    break;
                case 7:
                    CharacterItemIndex = DatabaseCore.Item.GetIndexByName(Player.General[Index_].CharacterName);

                    int ItemIndex = Item.ItemAmount;
                    Item.General[ItemIndex].Pk2ID = DatabaseCore.Item.ItemId[CharacterItemIndex[tmpPtr->Source]];
                    Item.General[ItemIndex].UniqueID = (uint)random.Next(76000000, 79999999);
                    Item.General[ItemIndex].Plus = DatabaseCore.Item.PlusValue[CharacterItemIndex[tmpPtr->Source]];
                    Item.General[ItemIndex].Durability = DatabaseCore.Item.Durability[CharacterItemIndex[tmpPtr->Source]];
                    Item.General[ItemIndex].Pickable = true;
                    Item.General[ItemIndex].Quantity = DatabaseCore.Item.Quantity[CharacterItemIndex[tmpPtr->Source]];
                    Item.Position[ItemIndex].XSector = Player.Position[Index_].XSector;
                    Item.Position[ItemIndex].YSector = Player.Position[Index_].YSector;
                    byte randomplace = (byte)random.Next(1, 7);
                    Item.Position[ItemIndex].X = Player.Position[Index_].X + randomplace;
                    Item.Position[ItemIndex].Z = Player.Position[Index_].Z;
                    Item.Position[ItemIndex].Y = Player.Position[Index_].Y + randomplace;
                    Item.General[ItemIndex].DroppedByUniqueId = Player.General[Index_].UniqueID;
                    Item.ItemAmount++;

                    writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_MOVEMENT);
                    writer.AppendByte(1);
                    writer.AppendByte(tmpPtr->Type);
                    writer.AppendByte(tmpPtr->Source);
                    ServerSocket.Send(writer.getWorkspace(), Index_);

                    Silkroad.Item_ tmpItem = Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterItemIndex[tmpPtr->Source]]);

                    byte[] tmpBuffer = new byte[0];
                    if (tmpItem.ITEM_TYPE_NAME.Contains("CH") || tmpItem.ITEM_TYPE_NAME.Contains("EU"))
                    {
                        writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SPAWN);
                        writer.AppendDword(DatabaseCore.Item.ItemId[CharacterItemIndex[tmpPtr->Source]]);
                        writer.AppendByte(DatabaseCore.Item.PlusValue[CharacterItemIndex[tmpPtr->Source]]);
                        writer.AppendDword(Item.General[ItemIndex].UniqueID);
                        writer.AppendByte(Item.Position[ItemIndex].XSector);
                        writer.AppendByte(Item.Position[ItemIndex].YSector);
                        writer.AppendFloat(Item.Position[ItemIndex].X);
                        writer.AppendFloat(Item.Position[ItemIndex].X);
                        writer.AppendFloat(Item.Position[ItemIndex].Y);
                        writer.AppendWord(0xAAA6);
                        writer.AppendByte(0);
                        writer.AppendByte(0);
                        writer.AppendByte(6);
                        writer.AppendDword(Item.General[ItemIndex].DroppedByUniqueId);
                        tmpBuffer = writer.getWorkspace();
                    }
                    else if (tmpItem.ITEM_TYPE_NAME.Contains("ETC"))
                    {
                        writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SPAWN);
                        writer.AppendDword(DatabaseCore.Item.ItemId[CharacterItemIndex[tmpPtr->Source]]);
                        writer.AppendDword(Item.General[ItemIndex].UniqueID);
                        writer.AppendByte(Item.Position[ItemIndex].XSector);
                        writer.AppendByte(Item.Position[ItemIndex].YSector);
                        writer.AppendFloat(Item.Position[ItemIndex].X);
                        writer.AppendFloat(Item.Position[ItemIndex].X);
                        writer.AppendFloat(Item.Position[ItemIndex].Y);
                        writer.AppendWord(0xAAA6);
                        writer.AppendByte(0);
                        writer.AppendByte(0);
                        writer.AppendByte(6);
                        writer.AppendDword(Item.General[ItemIndex].DroppedByUniqueId);
                        tmpBuffer = writer.getWorkspace();
                    }

                    for (int i = 0; i < Player.PlayersOnline; i++)
                    {
                        if (Player.General[i].CharacterID != 0)
                        {
                            if (Formula.CalculateDistance(Item.Position[ItemIndex], Player.Position[i]) <= 800)
                            {
                                ServerSocket.Send(tmpBuffer, i);
                                Player.Objects[i].SpawnedItemsIndex.Add(ItemIndex);
                            }
                        }
                    }

                    DatabaseCore.WriteQuery("UPDATE items SET itemid='0',plusvalue='0' ,durability='30' WHERE itemnumber='item{0}' AND owner='{1}'", tmpPtr->Source, Player.General[Index_].CharacterName);
                    Stats.OnStatPacket(Index_);

                    DeleteFromDatabase(CharacterItemIndex[tmpPtr->Source]);

                    break;
                case 10:
                    if (Player.Stats[Index_].Gold != 0)
                    {
                        PacketReader reader = new PacketReader(ptr, 5);
                        reader.ModifyIndex(1);
                        uint goldamount = reader.ReadDword();


                        ItemIndex = Item.ItemAmount;
                        if (goldamount < 10000)
                            Item.General[ItemIndex].Pk2ID = 1;
                        else if (goldamount >= 10000 && goldamount <= 500000)
                            Item.General[ItemIndex].Pk2ID = 2;
                        else if (goldamount >= 500001)
                            Item.General[ItemIndex].Pk2ID = 3;

                        Item.General[ItemIndex].UniqueID = (uint)random.Next(76000000, 79999999);
                        Item.General[ItemIndex].Plus = 0;
                        Item.General[ItemIndex].Durability = 0;
                        Item.General[ItemIndex].Pickable = true;
                        Item.General[ItemIndex].Quantity = goldamount;
                        Item.Position[ItemIndex].XSector = Player.Position[Index_].XSector;
                        Item.Position[ItemIndex].YSector = Player.Position[Index_].YSector;
                        randomplace = (byte)random.Next(1, 7);
                        Item.Position[ItemIndex].X = Player.Position[Index_].X + randomplace;
                        Item.Position[ItemIndex].Z = Player.Position[Index_].Z;
                        Item.Position[ItemIndex].Y = Player.Position[Index_].Y + randomplace;
                        Item.General[ItemIndex].DroppedByUniqueId = Player.General[Index_].UniqueID;
                        Item.ItemAmount++;

                        writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SPAWN);
                        if (goldamount < 10000)
                            writer.AppendDword(1);
                        else if (goldamount >= 10000 && goldamount <= 500000)
                            writer.AppendDword(2);
                        else if (goldamount >= 500001)
                            writer.AppendDword(3);
                        writer.AppendDword(goldamount);
                        writer.AppendDword(Item.General[ItemIndex].UniqueID);
                        writer.AppendByte(Item.Position[ItemIndex].XSector);
                        writer.AppendByte(Item.Position[ItemIndex].YSector);
                        writer.AppendFloat(Item.Position[ItemIndex].X);
                        writer.AppendFloat(Item.Position[ItemIndex].X);
                        writer.AppendFloat(Item.Position[ItemIndex].Y);
                        writer.AppendWord(0xDC72);
                        writer.AppendByte(0);
                        writer.AppendByte(0);
                        writer.AppendByte(6);
                        writer.AppendDword(0);

                        tmpBuffer = writer.getWorkspace();

                        for (int i = 0; i < Player.PlayersOnline; i++)
                        {
                            if (Player.General[i].CharacterID != 0)
                            {
                                if (Formula.CalculateDistance(Item.Position[ItemIndex], Player.Position[i]) <= 800)
                                {
                                    ServerSocket.Send(tmpBuffer, i);
                                    Player.Objects[i].SpawnedItemsIndex.Add(ItemIndex);
                                }
                            }
                        }

                        Player.Stats[Index_].Gold -= goldamount;
                        writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_NEW_GOLD_AMOUNT);
                        writer.AppendByte(1);
                        writer.AppendLword(Player.Stats[Index_].Gold);
                        writer.AppendByte(0);
                        ServerSocket.Send(writer.getWorkspace(), Index_);


                        writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_MOVEMENT);
                        writer.AppendByte(1);
                        writer.AppendByte(10);
                        writer.AppendDword(goldamount);
                        ServerSocket.Send(writer.getWorkspace(), Index_);

                        DatabaseCore.WriteQuery("UPDATE characters SET gold='{0}' WHERE name='{1}'", Player.Stats[Index_].Gold, Player.General[Index_].CharacterName);

                    }
                    break;
            }
        }


        private static void EquipItem(int Index_, int CharacterItemSourceIndex, int CharacterItemDestinationIndex)
        {
            PacketWriter writer = new PacketWriter();

            Silkroad.Item_ SourceItem = Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterItemSourceIndex]);
            Silkroad.Item_ DestinationItem = new Silkroad.Item_();
            if (DatabaseCore.Item.ItemId[CharacterItemDestinationIndex] != 0)
                DestinationItem = Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterItemDestinationIndex]);

            if (DatabaseCore.Item.Slot[CharacterItemDestinationIndex] <= 5)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemDestinationIndex] != 0)
                {
                    Player.Stats[Index_].PhyDef -= (ushort)DestinationItem.MIN_PHYSDEF;
                    Player.Stats[Index_].MagDef -= (ushort)DestinationItem.MAGDEF_MIN;
                    Player.Stats[Index_].Parry -= (ushort)DestinationItem.MIN_PARRY;
                }

                Player.Stats[Index_].PhyDef += (ushort)SourceItem.MAX_PHYSDEF;
                Player.Stats[Index_].MagDef += (ushort)SourceItem.MAGDEF_MAX;
                Player.Stats[Index_].Parry += (ushort)SourceItem.MIN_PARRY;
            }
            else if (DatabaseCore.Item.Slot[CharacterItemDestinationIndex] == 6)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemDestinationIndex] != 0)
                {
                    Player.Stats[Index_].MinPhy -= (ushort)DestinationItem.MIN_LPHYATK;
                    Player.Stats[Index_].MaxPhy -= (ushort)DestinationItem.MIN_HPHYATK;
                    Player.Stats[Index_].MinMag -= (ushort)DestinationItem.MIN_LMAGATK;
                    Player.Stats[Index_].MaxMag -= (ushort)DestinationItem.MIN_HMAGATK;
                }

                Player.Stats[Index_].MinPhy += (ushort)SourceItem.MIN_LPHYATK;
                Player.Stats[Index_].MaxPhy += (ushort)SourceItem.MIN_HPHYATK;
                Player.Stats[Index_].MinMag += (ushort)SourceItem.MIN_LMAGATK;
                Player.Stats[Index_].MaxMag += (ushort)SourceItem.MIN_HMAGATK;
                Player.General[Index_].WeaponType = SourceItem.CLASS_C;

            }
             else if (DatabaseCore.Item.Slot[CharacterItemDestinationIndex] == 7)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemDestinationIndex] != 0)
                {
                    Player.Stats[Index_].PhyDef -= (ushort)DestinationItem.MIN_PHYSDEF;
                    Player.Stats[Index_].MagDef -= (ushort)DestinationItem.MAGDEF_MIN;

                }
                Player.Stats[Index_].PhyDef += (ushort)SourceItem.MAX_PHYSDEF;
                Player.Stats[Index_].MagDef += (ushort)SourceItem.MAGDEF_MAX;
            }

            else if (DatabaseCore.Item.Slot[CharacterItemDestinationIndex] >= 9 && DatabaseCore.Item.Slot[CharacterItemDestinationIndex] <= 12)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemDestinationIndex] != 0)
                {
                    Player.Stats[Index_].TotalAccessoriesAbsorption -= DestinationItem.ABSORB_INC;
                }
                Player.Stats[Index_].TotalAccessoriesAbsorption += SourceItem.ABSORB_INC;
            }

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_EQUIP);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendByte(DatabaseCore.Item.Slot[CharacterItemDestinationIndex]);
            writer.AppendDword(DatabaseCore.Item.ItemId[CharacterItemSourceIndex]);
            writer.AppendByte(DatabaseCore.Item.PlusValue[CharacterItemSourceIndex]);
            ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

        }

        private static void UnEquipItem(int Index_, int CharacterItemSourceIndex, int CharacterItemDestinationIndex)
        {
            PacketWriter writer = new PacketWriter();
            Silkroad.Item_ SourceItem = Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterItemSourceIndex]);

            if (DatabaseCore.Item.Slot[CharacterItemSourceIndex] <= 5)
            {
                Player.Stats[Index_].PhyDef -= (ushort)SourceItem.MAX_PHYSDEF;
                Player.Stats[Index_].MagDef -= (ushort)SourceItem.MAGDEF_MAX;
                Player.Stats[Index_].Parry -= (ushort)SourceItem.MIN_PARRY;

            }
            else if (DatabaseCore.Item.Slot[CharacterItemSourceIndex] == 6)
            {
                Player.Stats[Index_].MinPhy -= (ushort)SourceItem.MIN_LPHYATK;
                Player.Stats[Index_].MaxPhy -= (ushort)SourceItem.MIN_HPHYATK;
                Player.Stats[Index_].MinMag -= (ushort)SourceItem.MIN_LMAGATK;
                Player.Stats[Index_].MaxMag -= (ushort)SourceItem.MIN_HMAGATK;
                Player.General[Index_].WeaponType = 0;
            }
            else if (DatabaseCore.Item.Slot[CharacterItemSourceIndex] == 7)
            {
                Player.Stats[Index_].PhyDef -= (ushort)SourceItem.MAX_PHYSDEF;
                Player.Stats[Index_].MagDef -= (ushort)SourceItem.MAGDEF_MAX;
            }

            else if (DatabaseCore.Item.Slot[CharacterItemSourceIndex] >= 9 && DatabaseCore.Item.Slot[CharacterItemSourceIndex] <= 12)
            {
                Player.Stats[Index_].TotalAccessoriesAbsorption -= SourceItem.ABSORB_INC;
            }

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_UNEQUIP);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendByte(DatabaseCore.Item.Slot[CharacterItemSourceIndex]);
            writer.AppendDword(DatabaseCore.Item.ItemId[CharacterItemSourceIndex]);

            ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

        }

        private static byte UnEnquipShield(int Index_, int[] CharacterItemIndex, int CharacterShieldIndex)
        {
            Silkroad.Item_ SourceItem = Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterShieldIndex]);

            if (SourceItem.CLASS_C != 2 && SourceItem.CLASS_C != 3)
            {
                if (Player.Stats[Index_].PhyDef < SourceItem.MAX_PHYSDEF)
                    Player.Stats[Index_].PhyDef = 1;
                else if (Player.Stats[Index_].MagDef < SourceItem.MAGDEF_MAX)
                    Player.Stats[Index_].MagDef = 1;
                else
                {
                    Player.Stats[Index_].PhyDef -= (ushort)SourceItem.MAX_PHYSDEF;
                    Player.Stats[Index_].MagDef -= (ushort)SourceItem.MAGDEF_MAX;
                }

                byte freeslot = FreeSlot(CharacterItemIndex);

                PacketWriter writer = new PacketWriter();
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_UNEQUIP);
                writer.AppendDword(Player.General[Index_].UniqueID);
                writer.AppendByte(7);
                writer.AppendDword(DatabaseCore.Item.ItemId[CharacterShieldIndex]);

                ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_MOVEMENT);
                writer.AppendByte(1);
                writer.AppendByte(0);
                writer.AppendByte(7);
                writer.AppendByte(freeslot);
                writer.AppendWord(1);
                writer.AppendByte(0);
                ServerSocket.Send(writer.getWorkspace(), Index_);

                return freeslot;
            }
            return 255;
        }

        private static byte UnEnquipWeapon(int Index_, int[] CharacterItemIndex, int CharacterItemSourceIndex)
        {
            Silkroad.Item_ SourceItem = Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterItemSourceIndex]);

            if (DatabaseCore.Item.ItemId[CharacterItemSourceIndex] != 0 && SourceItem.CLASS_C != 2 && SourceItem.CLASS_C != 3)
            {
                Player.Stats[Index_].MinPhy -= (ushort)SourceItem.MIN_LPHYATK;
                Player.Stats[Index_].MaxPhy -= (ushort)SourceItem.MIN_HPHYATK;
                Player.Stats[Index_].MinMag -= (ushort)SourceItem.MIN_LMAGATK;
                Player.Stats[Index_].MaxMag -= (ushort)SourceItem.MIN_HMAGATK;
                Player.General[Index_].WeaponType = 0;

                byte freeslot = FreeSlot(CharacterItemIndex);

                PacketWriter writer = new PacketWriter();

                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_UNEQUIP);
                writer.AppendDword(Player.General[Index_].UniqueID);
                writer.AppendByte(6);
                writer.AppendDword(DatabaseCore.Item.ItemId[CharacterItemSourceIndex]);

                ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_MOVEMENT);
                writer.AppendByte(1);
                writer.AppendByte(0);
                writer.AppendByte(6);
                writer.AppendByte(freeslot);
                writer.AppendWord(1);
                writer.AppendByte(0);
                ServerSocket.Send(writer.getWorkspace(), Index_);

                return freeslot;
            }
            return 255;
        }

        public static void EquipCape(int Index_)
        {
            PacketWriter writer = new PacketWriter();
            if (!Player.Flags[Index_].WearingCape)
            {
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_EQUIP);
                writer.AppendDword(Player.General[Index_].UniqueID);
                writer.AppendByte(DatabaseCore.Item.Slot[Player.Objects[Index_].SourceItemIndex]);
                writer.AppendDword(DatabaseCore.Item.ItemId[Player.Objects[Index_].SourceItemIndex]);
                writer.AppendByte(0);

                ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);
                Player.Flags[Index_].WearingCape = true;
            }
            else
            {
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_UNEQUIP);
                writer.AppendDword(Player.General[Index_].UniqueID);
                writer.AppendByte(DatabaseCore.Item.Slot[Player.Objects[Index_].SourceItemIndex]);
                writer.AppendDword(DatabaseCore.Item.ItemId[Player.Objects[Index_].SourceItemIndex]);
    
                ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);
                Player.Flags[Index_].WearingCape = false;
            }

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_MOVEMENT);
            writer.AppendWord(1);
            writer.AppendByte(DatabaseCore.Item.Slot[Player.Objects[Index_].SourceItemIndex]);
            writer.AppendByte(DatabaseCore.Item.Slot[Player.Objects[Index_].DestinationItemIndex]);
            writer.AppendWord(0);
            writer.AppendByte(0);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            MoveItemToDatabase(Player.Objects[Index_].SourceItemIndex, Player.Objects[Index_].DestinationItemIndex, Player.General[Index_].CharacterName);
        }

        public static void OnUseItem(PacketReader reader, int Index_)
        {
            int[] CharacterItemIndex = DatabaseCore.Item.GetIndexByName(Player.General[Index_].CharacterName);
            byte slot = reader.ReadByte();
            Silkroad.Item_ item = Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterItemIndex[slot]]);

            if (DatabaseCore.Item.ItemId[CharacterItemIndex[slot]] >= 4 && DatabaseCore.Item.ItemId[CharacterItemIndex[slot]] <= 23)
            {
                if ((DatabaseCore.Item.Quantity[CharacterItemIndex[slot]] - 1) > 0)
                    DatabaseCore.Item.Quantity[CharacterItemIndex[slot]]--;
                else
                {
                    DeleteFromDatabase(CharacterItemIndex[slot]);
                    DatabaseCore.WriteQuery("DELETE FROM items WHERE itemnumber='item{0}' AND owner='{1}'", slot, Player.General[Index_].CharacterName);
                }

                PacketWriter writer = new PacketWriter();
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_USE);
                writer.AppendByte(1);
                writer.AppendByte(slot);
                writer.AppendWord(DatabaseCore.Item.Quantity[CharacterItemIndex[slot]]);
                writer.AppendWord(reader.ReadWord());
                ServerSocket.Send(writer.getWorkspace(), Index_);

                writer = new PacketWriter();
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ANIMATION_POTION);
                writer.AppendDword(Player.General[Index_].UniqueID);
                if (item.ITEM_TYPE_NAME.Contains("HP") && !item.ITEM_TYPE_NAME.Contains("SPOTION"))
                {
                    writer.AppendDword(0x04);
                    ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

                    Player.Stats[Index_].CHP += item.USE_TIME;
                    Stats.HPUpdate(Index_, false);
                }
                else if (item.ITEM_TYPE_NAME.Contains("HP") && item.ITEM_TYPE_NAME.Contains("SPOTION"))
                {
                    writer.AppendDword(0x04);
                    ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

                    Player.Stats[Index_].CHP += ((int)Player.Stats[Index_].HP * item.USE_TIME2 / 100);
                    Stats.HPUpdate(Index_, false);
                }
                else if (item.ITEM_TYPE_NAME.Contains("MP") && !item.ITEM_TYPE_NAME.Contains("SPOTION"))
                {
                    writer.AppendDword(0x0E);
                    ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

                    Player.Stats[Index_].CMP += item.USE_TIME3;
                    Stats.MPUpdate(Index_, false);
                }
                else if (item.ITEM_TYPE_NAME.Contains("MP") && item.ITEM_TYPE_NAME.Contains("SPOTION"))
                {
                    writer.AppendDword(0x0E);
                    ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

                    Player.Stats[Index_].CMP += ((int)Player.Stats[Index_].MP * item.USE_TIME4 / 100);
                    Stats.MPUpdate(Index_, false);
                }
                else if (item.ITEM_TYPE_NAME.Contains("ALL") && !item.ITEM_TYPE_NAME.Contains("SPOTION"))
                {
                    writer.AppendDword(0x17);
                    ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

                    Player.Stats[Index_].CHP += item.USE_TIME;
                    Player.Stats[Index_].CMP += item.USE_TIME3;
                    Stats.HPMPUpdate(Index_);
                }
                else if (item.ITEM_TYPE_NAME.Contains("ALL") && item.ITEM_TYPE_NAME.Contains("SPOTION"))
                {
                    writer.AppendDword(0x17);
                    ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

                    Player.Stats[Index_].CHP += ((int)Player.Stats[Index_].HP * item.USE_TIME2 / 100);
                    Player.Stats[Index_].CMP += ((int)Player.Stats[Index_].MP * item.USE_TIME4 / 100);
                    Stats.HPMPUpdate(Index_);
                }
            }
        }
        public static byte[] CreateSpawnPacket(Item._General general,_Position pos)
        {
            PacketWriter writer = new PacketWriter();
            Silkroad.Item_ tmpItem = Silkroad.GetItemById(general.Pk2ID);
            if (tmpItem.ITEM_TYPE_NAME.Contains("CH") || tmpItem.ITEM_TYPE_NAME.Contains("EU"))
            {
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SPAWN);
                writer.AppendDword(general.Pk2ID);
                writer.AppendByte(general.Plus);
                writer.AppendDword(general.UniqueID);
                writer.AppendByte(pos.XSector);
                writer.AppendByte(pos.YSector);
                writer.AppendFloat(pos.X);
                writer.AppendFloat(pos.X);
                writer.AppendFloat(pos.Y);
                writer.AppendWord(0xAAA6);
                writer.AppendByte(0);
                writer.AppendByte(0);
                writer.AppendByte(6);
                writer.AppendDword(general.DroppedByUniqueId);

            }
            else if (tmpItem.ITEM_TYPE_NAME.Contains("ETC"))
            {
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SPAWN);
                writer.AppendDword(general.Pk2ID);
                if (general.Pk2ID == 1 || general.Pk2ID == 2 || general.Pk2ID == 3)
                    writer.AppendDword(general.Quantity);
                writer.AppendDword(general.UniqueID);
                writer.AppendByte(pos.XSector);
                writer.AppendByte(pos.YSector);
                writer.AppendFloat(pos.X);
                writer.AppendFloat(pos.X);
                writer.AppendFloat(pos.Y);
                writer.AppendWord(0xAAA6);
                writer.AppendByte(0);
                writer.AppendByte(0);
                writer.AppendByte(6);
                writer.AppendDword(general.DroppedByUniqueId);

            }
            return writer.getWorkspace();
        }
    }
}
