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
    class PlayerAction
    {
        private static Random random = new Random();

        public static void Action(PacketReader reader_, int Index_)
        {
            PacketWriter writer = new PacketWriter();

            reader_.ModifyIndex(1);
            byte type = reader_.ReadByte();

            if (Player.General[Index_].State != 1)
            {
                switch (type)
                {
                    case 1:

                        reader_.ModifyIndex(1);
                        uint ObjectId = reader_.ReadDword();
                        Player.Objects[Index_].AttackingObjectId = ObjectId;
                        int ObjectIndex = Players.GetObjectIndexAndType(Index_, Player.Objects[Index_].AttackingObjectId);

                        bool attack = false;

                        if (Player.Objects[Index_].SelectedObjectType == 2)
                            attack = Movement.MoveToObject(Index_, ref Player.Position[Index_], Monsters.Position[ObjectIndex], Player.General[Index_], false);

                        else
                            attack = Movement.MoveToObject(Index_, ref Player.Position[Index_], Player.Position[ObjectIndex], Player.General[Index_], false);

                        if (attack)
                        {
                            if (!Player.Objects[Index_].NormalAttack)
                                Attack.NormalAttack(Index_);
                        }
                        break;
                    case 2:
                        reader_.ModifyIndex(1);
                        ObjectId = reader_.ReadDword();

                        ObjectIndex = 0;
                        for (int i = 0; i <= Item.ItemAmount; i++)
                        {

                            if (Item.General[i].UniqueID == ObjectId && Index_ != i)
                            {
                                ObjectIndex = i;
                                break;
                            }
                        }

                        bool pickup = false;

                        pickup = Movement.MoveToObject(Index_, ref Player.Position[Index_], Item.Position[ObjectIndex], Player.General[Index_], false);

                        if (pickup)
                            PickUpItem(Index_, ObjectIndex);

                        break;

                    case 4:

                        uint skillid = reader_.ReadDword();

                        bool skillexist = false;
                        int CharacterSkillIndex = DatabaseCore.Skill.GetIndexByName(Player.General[Index_].CharacterName);

                        for (int i = 0; i <= DatabaseCore.Skill.SkillAmount[CharacterSkillIndex]; i++)
                        {
                            if (skillid == DatabaseCore.Skill.Skills[CharacterSkillIndex].SkillId[i])
                            {
                                skillexist = true;
                                break;
                            }
                            skillexist = false;
                        }

                        if (skillexist)
                        {
                            byte type_ = reader_.ReadByte();
                            switch (type_)
                            {
                                case 0:
                                    bool alreadyactive = false;
                                    for (byte i = 0; i < Player.Objects[Index_].ActiveBuffs.Length; i++)
                                    {
                                        if (Player.Objects[Index_].ActiveBuffs[i].Id == skillid)
                                            alreadyactive = true;
                                    }
                                    if (!alreadyactive)
                                    {
                                        Player.Objects[Index_].UsingSkillID = skillid;
                                        if (!Player.Objects[Index_].UsingSkill)
                                            PrepareBuff(Index_);
                                    }
                                    return;

                                case 1:
                                    uint attackingObjectId = reader_.ReadDword();

                                    Player.Objects[Index_].AttackingSkillID = skillid;
                                    Player.Objects[Index_].AttackingObjectId = attackingObjectId;
                                    Player.Objects[Index_].NormalAttack = false;
                                    if (!Player.Objects[Index_].UsingSkill)
                                        SkillAttackType(Index_);

                                    return;
                            }
                        }
                        break;

                    default:
                        Timers.PlayerAttack[Index_].Stop();
                        break;
                }
            }
        }

        private static void PrepareBuff(int Index_)
        {
            PacketWriter writer = new PacketWriter();

            Player.Objects[Index_].UsingSkill = true;

            uint CastingId = (uint)random.Next(65536, 1048575);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ATTACK);
            writer.AppendByte(1);
            writer.AppendByte(0);
            writer.AppendDword(Player.Objects[Index_].UsingSkillID);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendDword(CastingId);
            writer.AppendByte(0);
            writer.AppendDword(0);

            ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

            Silkroad.Skill_ tmpSkill = Silkroad.GetSkillById(Player.Objects[Index_].UsingSkillID);
            Player.Objects[Index_].BuffCastingID = CastingId;
            Timers.CastBuffTimer[Index_].Interval = tmpSkill.CastTime * 1000;
            Timers.CastBuffTimer[Index_].Start();


        }

        private static void SkillAttackType(int Index_)
        {
            PacketWriter writer = new PacketWriter();

            Silkroad.Skill_ tmpSkill = Silkroad.GetSkillById(Player.Objects[Index_].AttackingSkillID);

            byte weapontype = 0;
            if (Player.General[Index_].WeaponType == 6)
                weapontype = Silkroad.TypeTable.Bow;
            if ((Player.General[Index_].WeaponType == 2) || (Player.General[Index_].WeaponType == 3))
                weapontype = Silkroad.TypeTable.Bicheon;
            if ((Player.General[Index_].WeaponType == 4) || (Player.General[Index_].WeaponType == 5))
                weapontype = Silkroad.TypeTable.Heuksal;

            byte skilltype = tmpSkill.Type2;

            if (!(weapontype == skilltype) && !(skilltype == Silkroad.TypeTable.All))
            {
                Player.Objects[Index_].UsingSkill = false;
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ATTACK);
                writer.AppendWord(0xD02);
                ServerSocket.Send(writer.getWorkspace(), Index_);

                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SKILL_ATTACK);
                writer.AppendWord(3);
                writer.AppendByte(4);
                ServerSocket.Send(writer.getWorkspace(), Index_);
            }
            else
            {
                int ObjectIndex = Players.GetObjectIndexAndType(Index_, Player.Objects[Index_].AttackingObjectId);
                if (Player.Objects[Index_].SelectedObjectType == 2)
                    Timers.MonsterMovement[ObjectIndex].Stop();

                bool attack = false;

                if (Player.Objects[Index_].SelectedObjectType == 2)
                    attack = Movement.MoveToObject(Index_,ref Player.Position[Index_], Monsters.Position[ObjectIndex], Player.General[Index_].UniqueID, Player.General[Index_].CharacterID, tmpSkill.Distance * 10, weapontype,false);

                else
                    attack = Movement.MoveToObject(Index_, ref Player.Position[Index_], Player.Position[ObjectIndex], Player.General[Index_].UniqueID, Player.General[Index_].CharacterID, tmpSkill.Distance * 10, weapontype, false);

                if (attack)
                    Attack.BeginSkill(Index_);

            }
        }

        private static void PickUpItem(int Index_, int ObjectIndex)
        {
            int[] CharacterItemIndex = DatabaseCore.Item.GetIndexByName(Player.General[Index_].CharacterName);

            PacketWriter writer = new PacketWriter();
          
            for (int i = 0; i < Player.PlayersOnline; i++)
            {
                if (Player.General[i].UniqueID != 0)
                {
                    double Distance = Formula.CalculateDistance(Item.Position[ObjectIndex], Player.Position[i]);
                    if (Distance <= 800)
                    {
                        if (Player.Objects[i].SpawnedItemsIndex.Contains(ObjectIndex))
                        {
                            ServerSocket.Send(Players.CreateDeSpawnPacket(Item.General[ObjectIndex].UniqueID), i);
                            Player.Objects[i].SpawnedItemsIndex.Remove(ObjectIndex);
                        }
                    }
                }
            }

            Silkroad.Item_ tmpItem = Silkroad.GetItemById(Item.General[ObjectIndex].Pk2ID);

            byte freeslot = 0;
            if (Item.General[ObjectIndex].Pk2ID == 1 || Item.General[ObjectIndex].Pk2ID == 2 || Item.General[ObjectIndex].Pk2ID == 3)
                freeslot = 254;
            else
                freeslot = Items.FreeSlot(CharacterItemIndex);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ITEM_MOVEMENT);
            writer.AppendWord(0x601);
            writer.AppendByte(freeslot);
            if (freeslot != 254)
                writer.AppendDword(Item.General[ObjectIndex].Pk2ID);
            if (tmpItem.ITEM_TYPE_NAME.Contains("CH") || tmpItem.ITEM_TYPE_NAME.Contains("EU"))
            {
                writer.AppendByte(Item.General[ObjectIndex].Plus);
                writer.AppendLword(0);
                writer.AppendDword((uint)tmpItem.MIN_DURA);
                writer.AppendByte(0);

                DatabaseCore.WriteQuery("UPDATE items SET itemid='{0}',plusvalue='{1}',type='0',durability='{2}' WHERE itemnumber='item{3}' AND owner ='{4}'", Item.General[ObjectIndex].Pk2ID, Item.General[ObjectIndex].Plus, Item.General[ObjectIndex].Durability, freeslot, Player.General[Index_].CharacterName);
                Items.AddItemToDatabase(CharacterItemIndex[freeslot], Item.General[ObjectIndex].Pk2ID, 0, 0, (byte)Item.General[ObjectIndex].Durability, Item.General[ObjectIndex].Plus, 0);
            }
            else if (tmpItem.ITEM_TYPE_NAME.Contains("ETC"))
            {
                if (freeslot == 254)
                {
                    writer.AppendDword(Item.General[ObjectIndex].Quantity);
                    Player.Stats[Index_].Gold += Item.General[ObjectIndex].Quantity;
                    DatabaseCore.WriteQuery("UPDATE characters SET gold='{0}' WHERE name='{1}'", Player.Stats[Index_].Gold, Player.General[Index_].CharacterName);
                }
                else
                {
                    writer.AppendWord((ushort)Item.General[ObjectIndex].Quantity);
                    DatabaseCore.WriteQuery("UPDATE items SET itemid='{0}',plusvalue='0',type='1',durability='{1}',quantity='{2}' WHERE itemnumber='item{3}' AND owner ='{4}'", Item.General[ObjectIndex].Pk2ID, Item.General[ObjectIndex].Durability, Item.General[ObjectIndex].Quantity, freeslot, Player.General[Index_].CharacterName);
                    Items.AddItemToDatabase(CharacterItemIndex[freeslot], Item.General[ObjectIndex].Pk2ID, 1, (byte)Item.General[ObjectIndex].Quantity, (byte)Item.General[ObjectIndex].Durability, 0, 0);
                }
            }

            ServerSocket.Send(writer.getWorkspace(), Index_);

            Item.General[ObjectIndex] = new Item._General();
            Item.Position[ObjectIndex] = new _Position();

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_NEW_GOLD_AMOUNT);
            writer.AppendByte(1);
            writer.AppendLword(Player.Stats[Index_].Gold);
            writer.AppendByte(0);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ANIMATION_ITEM_PICKUP);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendByte(141);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SKILL_ATTACK);
            writer.AppendByte(2);
            writer.AppendByte(0);
            ServerSocket.Send(writer.getWorkspace(), Index_);

        }

        public static void OnEmotion(PacketReader reader_, int Index_)
        {
            PacketWriter writer = new PacketWriter();
            byte type = reader_.ReadByte();

            writer.SetOpcode(CLIENT_OPCODES.GAME_CLIENT_EMOTION);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendByte(type);
            ServerSocket.Send(writer.getWorkspace(), Index_);
        }
    }
}