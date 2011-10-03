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
    class Players
    {
        public static int GetObjectIndexAndType(int Index_, uint ObjectId)
        {
            int ObjectIndex = 0;
            Player.Objects[Index_].SelectedObjectType = 0;
            for (int i = 0; i <= Player.PlayersOnline; i++)
            {
                if (Player.General[i].UniqueID == ObjectId && Index_ != i)
                {
                    Player.Objects[Index_].SelectedObjectType = 1;
                    ObjectIndex = i;
                    break;
                }
            }
            if (Player.Objects[Index_].SelectedObjectType != 1)
            {
                for (int j = 0; j <= Monsters.MonsterAmount; j++)
                {
                    if (Monsters.General[j].UniqueID == ObjectId)
                    {
                        Player.Objects[Index_].SelectedObjectType = 2;
                        ObjectIndex = j;
                        break;
                    }
                }
            }
            return ObjectIndex;
        }

        public static int GetObjectIndex( uint ObjectId)
        {
            for (int i = 0; i <= Player.PlayersOnline; i++)
            {
                if (Player.General[i].UniqueID == ObjectId)
                {
                    return i;
                }
            }

            for (int j = 0; j <= Monsters.MonsterAmount; j++)
            {
                if (Monsters.General[j].UniqueID == ObjectId)
                {
                    return j;
                }
            }

            return 0;
        }

        public static void OnTarget(PacketReader reader_, int Index_)
        {
            PacketWriter writer = new PacketWriter();
            uint ObjectId = reader_.ReadDword();

            int ObjectIndex = GetObjectIndexAndType(Index_,ObjectId);                 

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_TARGET);
            if (ObjectId == Player.General[Index_].UniqueID)
            {
                writer.AppendWord(0x602);
                ServerSocket.Send(writer.getWorkspace(), Index_);
            }
            else
            {
                writer.AppendByte(1);
                writer.AppendDword(ObjectId);
                switch (Player.Objects[Index_].SelectedObjectType)
                {
                    case 1:
                        writer.AppendDword(0x10);
                        writer.AppendByte(4);
                        break;
                    case 2:
                        writer.AppendByte(1);
                        writer.AppendDword((uint)Monsters.General[ObjectIndex].HP);
                        if (Player.Flags[ObjectIndex].Dead)
                            writer.AppendByte(4);
                        else
                            writer.AppendDword(1);
                        break;
                }

                ServerSocket.Send(writer.getWorkspace(), Index_);
            }

            Player.Objects[Index_].SelectedObjectId = ObjectId;
        }

        public static void OnIngameNotify(PacketReader reader_, int Index_)
        {
            uint id = reader_.ReadDword();
            switch (id)
            {
                case 1:
                    Player.PlayersOnline++;
                    Player.Flags[Index_].Ingame = 1;
                    Character.SpawnMe(Index_);
                    Character.SpawnOtherPlayer(Index_);
                    OnRangeSpawn(Index_);
                    break;
            }
        }

        public static byte[] CreateDeSpawnPacket(uint UniqueId)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_DESPAWN);
            writer.AppendDword(UniqueId);
            return writer.getWorkspace();
        }


        public static void OnRangeSpawn(int Index_)
        {
            for (int i = 0; i < Monsters.MonsterAmount; i++)
            {
                if (Monsters.General[i].UniqueID != 0 && !Monsters.General[i].Dead)
                {
                    double Distance = Formula.CalculateDistance(Player.Position[Index_], Monsters.Position[i]);
                    if (Distance <= 800)
                    {
                        if (!Player.Objects[Index_].SpawnedMonsterIndex.Contains(i))
                        {
                            ServerSocket.Send(MonsterSpawn.CreateSpawnPacket(Monsters.General[i].ID, Monsters.General[i].UniqueID, Monsters.Position[i], Monsters.General[i].Type), Index_);
                            Player.Objects[Index_].SpawnedMonsterIndex.Add(i);
                        }
                    }
                }
            }
            for (int i = 0; i < Item.ItemAmount; i++)
            {
                if (Item.General[i].UniqueID != 0)
                {
                    double Distance = Formula.CalculateDistance(Player.Position[Index_], Item.Position[i]);
                    if (Distance <= 800)
                    {
                        if (!Player.Objects[Index_].SpawnedItemsIndex.Contains(i))
                        {
                            ServerSocket.Send(Items.CreateSpawnPacket(Item.General[i], Item.Position[i]), Index_);
                            Player.Objects[Index_].SpawnedItemsIndex.Add(i);
                        }
                    }
                }
            }
        }
    }
}

