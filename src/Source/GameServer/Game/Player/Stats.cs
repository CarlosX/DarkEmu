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
    class Stats
    {
        private static PacketWriter writer = new PacketWriter();
        private static Random random = new Random();

        public static void OnStatPacket(int Index_)
        {
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_STATS);
            writer.AppendDword(Player.Stats[Index_].MinPhy);
            writer.AppendDword(Player.Stats[Index_].MaxPhy);
            writer.AppendDword(Player.Stats[Index_].MinMag);
            writer.AppendDword(Player.Stats[Index_].MaxMag);
            writer.AppendWord(Player.Stats[Index_].PhyDef);
            writer.AppendWord(Player.Stats[Index_].MagDef);
            writer.AppendWord(Player.Stats[Index_].Hit);
            writer.AppendWord(Player.Stats[Index_].Parry);
            writer.AppendDword(Player.Stats[Index_].HP);
            writer.AppendDword(Player.Stats[Index_].MP);
            writer.AppendWord(Player.Stats[Index_].Strength);
            writer.AppendWord(Player.Stats[Index_].Intelligence);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            DatabaseCore.WriteQuery(
            "UPDATE characters SET min_phyatk='{0}', max_phyatk='{1}', min_magatk='{2}', max_magatk='{3}', phydef='{4}', magdef='{5}', hit='{6}', parry='{7}', hp='{8}', mp='{9}', strength='{10}', intelligence='{11}', attribute='{12}' WHERE id='{13}'", Player.Stats[Index_].MinPhy, Player.Stats[Index_].MaxPhy, Player.Stats[Index_].MinMag, Player.Stats[Index_].MaxMag, Player.Stats[Index_].PhyDef, Player.Stats[Index_].MagDef, Player.Stats[Index_].Hit, Player.Stats[Index_].Parry, Player.Stats[Index_].HP, Player.Stats[Index_].MP, Player.Stats[Index_].Strength, Player.Stats[Index_].Intelligence, Player.Stats[Index_].Attributes, Player.General[Index_].CharacterID);
        }

        public static void HPUpdate(int Index_, bool decrease)
        {
            // make sure that we don't add a higher value than maxHP
            if (Player.Stats[Index_].CHP > Player.Stats[Index_].HP)
            {
                Player.Stats[Index_].CHP = (int)Player.Stats[Index_].HP;
            }

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_HPMP_UPDATE);
            writer.AppendDword(Player.General[Index_].UniqueID);
            if (decrease)
                writer.AppendByte(0x01);
            else
                writer.AppendByte(0x10);
            writer.AppendByte(0x00);
            writer.AppendByte(0x01);
            writer.AppendDword((uint)Player.Stats[Index_].CHP);
            ServerSocket.Send(writer.getWorkspace(), Index_);
        }

        public static void MPUpdate(int Index_, bool decrease)
        {
            // make sure that we don't add a higher value than maxMP
            if (Player.Stats[Index_].CMP > Player.Stats[Index_].MP)
            {
                Player.Stats[Index_].CMP = (int)Player.Stats[Index_].MP;
            }

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_HPMP_UPDATE);
            writer.AppendDword(Player.General[Index_].UniqueID);
            if(decrease)
                writer.AppendByte(0x04);
            else
                writer.AppendByte(0x10);
            writer.AppendByte(0x00);
            writer.AppendByte(0x02);
            writer.AppendDword((uint)Player.Stats[Index_].CMP);
            ServerSocket.Send(writer.getWorkspace(), Index_);
        }

        public static void HPMPUpdate(int Index_)
        {
            // make sure that we don't add a higher value than maxHP
            if (Player.Stats[Index_].CHP > Player.Stats[Index_].HP)
            {
                Player.Stats[Index_].CHP = (int)Player.Stats[Index_].HP;
            }
            // make sure that we don't add a higher value than maxMP
            if (Player.Stats[Index_].CMP > Player.Stats[Index_].MP)
            {
                Player.Stats[Index_].CMP = (int)Player.Stats[Index_].MP;
            }

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_HPMP_UPDATE);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendByte(0x10);
            writer.AppendByte(0x00);
            writer.AppendByte(0x03);
            writer.AppendDword((uint)Player.Stats[Index_].CHP);
            writer.AppendDword((uint)Player.Stats[Index_].CMP);
            ServerSocket.Send(writer.getWorkspace(), Index_);
        }

        public static void STRUpdate(int Index_)
        {
            Player.Stats[Index_].Attributes--;
            Player.Stats[Index_].Strength++;
            OnStatPacket(Index_);

            Formula.CalculateHP(Index_);
            HPUpdate(Index_, false);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_STR_UPDATE);
            writer.AppendByte(0x01);
            ServerSocket.Send(writer.getWorkspace(), Index_);
        }

        public static void INTUpdate(int Index_)
        {
            Player.Stats[Index_].Attributes--;
            Player.Stats[Index_].Intelligence++;
            OnStatPacket(Index_);

            Formula.CalculateMP(Index_);
            MPUpdate(Index_, false);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_INT_UPDATE);
            writer.AppendByte(0x01);
            ServerSocket.Send(writer.getWorkspace(), Index_);
        }

        public static void GetXP(int Index_, int ObjectIndex)
        {
            int xprate = 2, sprate = 2, goldrate = 2;
            Silkroad.Gold_ tmpGold = Silkroad.GetGoldDataByLevel(Monsters.General[ObjectIndex].Level);
            ulong sp = (ulong)random.Next(tmpGold.Skillpoints, tmpGold.Skillpoints + 25);
            ulong exp = Monsters.General[ObjectIndex].Exp;

            if (Player.Stats[Index_].Level < Monsters.General[ObjectIndex].Level)
                exp = ((ulong)(Monsters.General[ObjectIndex].Level - Player.Stats[Index_].Level) * Monsters.General[ObjectIndex].Exp) * (ulong)xprate;
            if (Player.Stats[Index_].Level > Monsters.General[ObjectIndex].Level)
            {
                exp = ((ulong)(Player.Stats[Index_].Level - Monsters.General[ObjectIndex].Level) / Monsters.General[ObjectIndex].Exp) * (ulong)xprate;
                if (exp == 0)
                    exp = 1;
            }
            if (Player.Stats[Index_].Level == Monsters.General[ObjectIndex].Level)
                exp = (Monsters.General[ObjectIndex].Exp * (ulong)xprate);

            sp *= (ulong)sprate;

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_EXP);
            writer.AppendDword(Monsters.General[ObjectIndex].UniqueID);
            writer.AppendLword(exp);
            writer.AppendLword(sp);
            writer.AppendByte(0);
            if (CheckIfNewLevel(Index_, exp))
            {
                writer.AppendWord(Player.Stats[Index_].Attributes);
                ServerSocket.Send(writer.getWorkspace(), Index_);

                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ANIMATION_LEVEL_UP);
                writer.AppendDword(Player.General[Index_].UniqueID);
    
                ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

                Formula.CalculateHP(Index_);
                Formula.CalculateMP(Index_);
                STRUpdate(Index_);
                INTUpdate(Index_);
            }
            else
                ServerSocket.Send(writer.getWorkspace(), Index_);

            CheckIfNewSp(Index_, sp);

            HPUpdate(Index_, false);
            MPUpdate(Index_, false);
            OnStatPacket(Index_);

            DatabaseCore.WriteQuery("UPDATE characters SET experience='{0}',level = '{1}',sp = '{2}',skillpointbar = '{3}' WHERE name='{4}'", Player.Stats[Index_].Experience, Player.Stats[Index_].Level, Player.Stats[Index_].Skillpoints, Player.Stats[Index_].SkillpointBar, Player.General[Index_].CharacterName);
            uint goldamount = (uint)(tmpGold.Gold * goldrate);

            int ItemIndex = Item.ItemAmount;
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
            Item.Position[ItemIndex].XSector = Monsters.Position[ObjectIndex].XSector;
            Item.Position[ItemIndex].YSector = Monsters.Position[ObjectIndex].YSector;
            byte randomplace = (byte)random.Next(1, 7);
            Item.Position[ItemIndex].X = Monsters.Position[ObjectIndex].X + randomplace;
            Item.Position[ItemIndex].Z = Monsters.Position[ObjectIndex].Z;
            Item.Position[ItemIndex].Y = Monsters.Position[ObjectIndex].Y + randomplace;
            Item.General[ItemIndex].DroppedByUniqueId = Monsters.General[ObjectIndex].UniqueID;
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
            writer.AppendByte(5);
            writer.AppendDword(0);
            byte[] tmpBuffer = writer.getWorkspace();

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

        }

        public static void GetXP(int Index_, ulong exp, ulong sp)
        {
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_EXP);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendLword(exp);
            writer.AppendLword(sp);
            writer.AppendByte(0);
            if (CheckIfNewLevel(Index_, exp))
            {
                writer.AppendWord(Player.Stats[Index_].Attributes);
                ServerSocket.Send(writer.getWorkspace(), Index_);

                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ANIMATION_LEVEL_UP);
                writer.AppendDword(Player.General[Index_].UniqueID);
                ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

                Formula.CalculateHP(Index_);
                Formula.CalculateMP(Index_);
                STRUpdate(Index_);
                INTUpdate(Index_);
            }
            else
                ServerSocket.Send(writer.getWorkspace(), Index_);

            CheckIfNewSp(Index_, sp);

            HPUpdate(Index_, false);
            MPUpdate(Index_, false);
            OnStatPacket(Index_);
            DatabaseCore.WriteQuery("UPDATE characters SET experience='{0}',level = '{1}',sp = '{2}',skillpointbar = '{3}' WHERE name='{4}'", Player.Stats[Index_].Experience , Player.Stats[Index_].Level,Player.Stats[Index_].Skillpoints, Player.Stats[Index_].SkillpointBar , Player.General[Index_].CharacterName);
        }

        private static bool CheckIfNewLevel(int Index_, ulong EXP)
        {
            Silkroad.Level_ tmpLevel = Silkroad.GetLevelDataByLevel(Player.Stats[Index_].Level);
            bool tmpBool = false;
            long tmpLong = (long)EXP + (long)Player.Stats[Index_].Experience;
            for (byte i = 0; i < 255; i++)
            {
                tmpLong = (long)tmpLevel.Experience - (long)tmpLong;
                if (tmpLong > 0)
                {
                    if (i == 0)
                        Player.Stats[Index_].Experience += (ulong)EXP;
                    else
                        Player.Stats[Index_].Experience += (ulong)(tmpLevel.Experience - (ulong)tmpLong);
                    return tmpBool;
                }

                else if (tmpLong <= 0)
                {
                    Player.Stats[Index_].Experience = 0;
                    tmpBool = true;
                    tmpLong *= -1;
                    Player.Stats[Index_].Attributes += 3;
                    Player.Stats[Index_].Level++;
                    tmpLevel = Silkroad.GetLevelDataByLevel(Player.Stats[Index_].Level);
                }

            }
            return false;
        }

        private static bool CheckIfNewSp(int Index_, ulong SEXP)
        {
            ulong tmpLong = Player.Stats[Index_].SkillpointBar + SEXP;
            if (tmpLong >= 400)
            {
                ulong tmpDiv = tmpLong / 400;
                Player.Stats[Index_].SkillpointBar = (ushort)(tmpLong - 400 * tmpDiv);
                Player.Stats[Index_].Skillpoints += (ushort)tmpDiv;
                Mastery.SPUpdate(Index_);
                return true;
            }
            else
            {
                Player.Stats[Index_].SkillpointBar += (ushort)SEXP;
                return false;
            }
        }

        public static void GetBerserk(int Index_, int ObjectIndex)
        {
            if (random.Next(0, 10) > 7 && Player.Stats[Index_].BerserkBar != 5)
            {
                Player.Stats[Index_].BerserkBar++;

                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SKILLPOINTS);
                writer.AppendByte(4);
                writer.AppendByte(Player.Flags[Index_].Berserk);
                writer.AppendDword(Monsters.General[ObjectIndex].UniqueID);
                ServerSocket.Send(writer.getWorkspace(), Index_);
                DatabaseCore.WriteQuery("UPDATE characters SET berserkbar='{0}' WHERE name='{1}'", Player.Flags[Index_].Berserk, Player.General[Index_].CharacterName);

            }
        }
    }
}
