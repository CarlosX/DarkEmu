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
using System.Runtime.InteropServices;

namespace DarkEmu_GameServer
{
    struct _Position
    {
        public float X, Y, Z;
        public byte XSector, YSector;
    }

    class Movement
    {
        private static PacketWriter writer = new PacketWriter();

        unsafe public static void OnMovement(byte* ptr, int Index_)
        {
            Silkroad.C_S.MOVEMENT_GROUND* tmpPtr = (Silkroad.C_S.MOVEMENT_GROUND*)ptr;

            if (tmpPtr->Type == 0)
            {
                Silkroad.C_S.MOVEMENT_SKY* MovingSky = (Silkroad.C_S.MOVEMENT_SKY*)ptr;
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_MOVEMENT);
                writer.AppendDword(Player.General[Index_].UniqueID);
                writer.AppendByte(MovingSky->Type);
                writer.AppendByte(MovingSky->Flag);
                writer.AppendWord((ushort)((MovingSky->Angle * 360) / 65536));
                writer.AppendByte(1);
                writer.AppendByte(Player.Position[Index_].XSector);
                writer.AppendByte(Player.Position[Index_].YSector);
                writer.AppendWord((ushort)Player.Position[Index_].X);
                writer.AppendFloat(Player.Position[Index_].Z);
                writer.AppendWord((ushort)Player.Position[Index_].Y);


            }
            else if (tmpPtr->Type == 1)
            {
                if (Player.General[Index_].State != 4 || Player.General[Index_].State != 10 || Player.General[Index_].State != 1)
                {
                    Player.Objects[Index_].AttackingSkillID = 0;
                    Player.Objects[Index_].NormalAttack = false;
                    Player.Objects[Index_].UsingSkill = false;
                    Player.General[Index_].Busy = false;
                    Timers.PlayerAttack[Index_].Stop();

                    Player.Position[Index_].XSector = tmpPtr->XSector;
                    Player.Position[Index_].YSector = tmpPtr->YSector;
                    Player.Position[Index_].X = tmpPtr->X;
                    Player.Position[Index_].Z = tmpPtr->Z;
                    Player.Position[Index_].Y = tmpPtr->Y;

                    writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_MOVEMENT);
                    writer.AppendDword(Player.General[Index_].UniqueID);
                    writer.AppendByte(tmpPtr->Type);
                    writer.AppendByte(Player.Position[Index_].XSector);
                    writer.AppendByte(Player.Position[Index_].YSector);
                    writer.AppendWord((ushort)Player.Position[Index_].X);
                    writer.AppendWord((ushort)Player.Position[Index_].Z);
                    writer.AppendWord((ushort)Player.Position[Index_].Y);
                    writer.AppendByte(0);

                    byte[] tmpBuffer = writer.getWorkspace();

                    for (int i = 0; i < Player.PlayersOnline; i++)
                    {
                        if (Player.General[i].CharacterID != 0)
                        {
                            if (i == Index_)
                                ServerSocket.Send(tmpBuffer, i);
                            else
                            {
                                if (Formula.CalculateDistance(Player.Position[Index_], Player.Position[i]) <= 800 && Player.Objects[i].SpawnedIndex.Contains(Index_))
                                    ServerSocket.Send(tmpBuffer, i);
                            }
                        }
                    }

                    DatabaseCore.WriteQuery("UPDATE characters SET xsect='{0}', ysect='{1}', xpos='{2}', zpos='{3}', ypos='{4}' where id='{5}'", Player.Position[Index_].XSector, Player.Position[Index_].YSector, Player.Position[Index_].X, Player.Position[Index_].Z, Player.Position[Index_].Y, Player.General[Index_].CharacterID);

                    for (int i = 0; i < Player.PlayersOnline; i++)
                    {
                        if (Player.General[i].CharacterID != 0 && Index_ != i)
                        {
                            double Distance = Formula.CalculateDistance(Player.Position[Index_], Player.Position[i]);
                            if (Distance <= 800)
                            {
                                if (!Player.Objects[Index_].SpawnedIndex.Contains(i))
                                {
                                    ServerSocket.Send(Character.CreateSpawnPacket(Player.General[i], Player.Flags[i], Player.Position[i], Player.Stats[i], Player.Speeds[i]), Index_);
                                    Player.Objects[Index_].SpawnedIndex.Add(i);

                                    ServerSocket.Send(Character.CreateSpawnPacket(Player.General[Index_], Player.Flags[Index_], Player.Position[Index_], Player.Stats[Index_], Player.Speeds[Index_]), i);
                                    Player.Objects[i].SpawnedIndex.Add(Index_);

                                }
                            }
                            else if (Distance > 800)
                            {
                                if (Player.Objects[Index_].SpawnedIndex.Contains(i))
                                {
                                    ServerSocket.Send(Players.CreateDeSpawnPacket(Player.General[i].UniqueID), Index_);
                                    Player.Objects[Index_].SpawnedIndex.Remove(i);

                                    ServerSocket.Send(Players.CreateDeSpawnPacket(Player.General[Index_].UniqueID), i);
                                    Player.Objects[i].SpawnedIndex.Remove(Index_);

                                }
                            }
                        }
                    }

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
                            else if (Distance > 800)
                            {
                                if (Player.Objects[Index_].SpawnedMonsterIndex.Contains(i))
                                {
                                    ServerSocket.Send(Players.CreateDeSpawnPacket(Monsters.General[i].UniqueID), Index_);
                                    Player.Objects[Index_].SpawnedMonsterIndex.Remove(i);
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
                            else if (Distance > 800)
                            {
                                if (Player.Objects[Index_].SpawnedItemsIndex.Contains(i))
                                {
                                    ServerSocket.Send(Players.CreateDeSpawnPacket(Item.General[i].UniqueID), Index_);
                                    Player.Objects[Index_].SpawnedItemsIndex.Remove(i);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool MoveToObject(int Index_, ref _Position obj_1, _Position obj_2, uint uniqueid, bool Monster)
        {
            return MoveToObject(Index_, ref obj_1, obj_2, uniqueid, 0, 28, 0, Monster);
        }

        public static bool MoveToObject(int Index_, ref _Position obj_1, _Position obj_2, Player._General general, bool Monster)
        {
            return MoveToObject(Index_, ref obj_1, obj_2, general.UniqueID, general.CharacterID, 28, general.WeaponType, Monster);
        }

        public static bool MoveToObject(int Index_, ref _Position obj_1, _Position obj_2, uint uniqueid, uint characterid, double weapondistance, byte weapontype, bool Monster)
        {
            double distance_x = obj_1.X - obj_2.X;
            double distance_y = obj_1.Y - obj_2.Y;
            double distance = Formula.CalculateDistance(distance_x, distance_y);

            if (weapontype == 6 || weapontype == 12)
            {
                weapondistance = 250;
                distance_y -= 100;
                distance_x -= 150;
            }

            if (distance > weapondistance)
            {
                distance_x = (distance_x - ((distance_x * 10) / distance));
                distance_y = (distance_y - ((distance_y * 10) / distance));

                obj_1.X -= (float)distance_x;
                obj_1.Y -= (float)distance_y;

                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_MOVEMENT);
                writer.AppendDword(uniqueid);
                writer.AppendByte(1);
                writer.AppendByte(obj_1.XSector);
                writer.AppendByte(obj_1.YSector);
                writer.AppendWord((ushort)obj_1.X);
                writer.AppendWord((ushort)obj_1.Z);
                writer.AppendWord((ushort)obj_1.Y);
                writer.AppendByte(0);
                if (Monster)
                    ServerSocket.SendPacketIfMonsterIsSpawned(writer.getWorkspace(), Index_);
                else
                    ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

                if (characterid != 0)
                    DatabaseCore.WriteQuery("UPDATE characters SET xpos='{0}',ypos='{1}' WHERE id='{2}'", (int)obj_1.X, (int)obj_2.Y, characterid);

                return false;
            }
            return true;
        }
    }
}