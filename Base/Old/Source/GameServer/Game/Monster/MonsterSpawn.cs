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
using System.Collections;
using System.Text;

namespace DarkEmu_GameServer
{
    class MonsterSpawn
    {
        public static void KillMonster(PacketReader reader_, int Index_)
        {
            uint ObjectId = reader_.ReadDword();

            int ObjectIndex = Players.GetObjectIndex(ObjectId);

            ServerSocket.SendPacketIfMonsterIsSpawned(Players.CreateDeSpawnPacket(ObjectId), ObjectIndex);

            for (int i = 0; i <= Monsters.MonsterAmount; i++)
            {
                if (Monsters.General[i].ID == ObjectId)
                {
                    Timers.MonsterAttack[i].Stop();
                    Timers.MonsterMovement[i].Stop();
                    break;
                }
            }
        }

        public static void CreateSpawnPacket(ref PacketWriter writer, uint monsterid, uint uniqueid, _Position pos, byte monstertype)
        {
            writer.AppendDword(monsterid);
            writer.AppendDword(uniqueid);

            writer.AppendByte(pos.XSector);
            writer.AppendByte(pos.YSector);
            writer.AppendFloat(pos.X);
            writer.AppendFloat(pos.Z);
            writer.AppendFloat(pos.Y);

            writer.AppendWord(0);
            writer.AppendByte(0);
            writer.AppendByte(1);
            writer.AppendByte(0);
            writer.AppendWord(0);
            writer.AppendWord(1);
            writer.AppendByte(0);

            writer.AppendFloat(16f);
            writer.AppendFloat(16f);
            writer.AppendFloat(16f);

            writer.AppendByte(0);
            writer.AppendByte(0);

            writer.AppendByte(monstertype);
            writer.AppendByte(1);
        }

        public static void CreateDeSpawnPacket(ref PacketWriter writer, uint monsterid)
        {
            writer.AppendDword(monsterid);
        }

        public static byte[] CreateSpawnPacket(uint monsterid, uint uniqueid, _Position pos, byte monstertype)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SPAWN);
            writer.AppendDword(monsterid);
            writer.AppendDword(uniqueid);

            writer.AppendByte(pos.XSector);
            writer.AppendByte(pos.YSector);
            writer.AppendFloat(pos.X);
            writer.AppendFloat(pos.Z);
            writer.AppendFloat(pos.Y);

            writer.AppendWord(0);
            writer.AppendByte(0);
            writer.AppendByte(1);
            writer.AppendByte(0);
            writer.AppendWord(0);
            writer.AppendWord(1);
            writer.AppendByte(0);

            writer.AppendFloat(16f);
            writer.AppendFloat(16f);
            writer.AppendFloat(16f);

            writer.AppendByte(0);
            writer.AppendByte(0);

            writer.AppendByte(monstertype);
            writer.AppendByte(1);
            return writer.getWorkspace();
        }

        public static void OnSpawn(PacketReader reader_, int Index_)
        {
            int MonsterIndex = Monsters.MonsterAmount;
            uint monsterid = reader_.ReadDword();
            byte monstertype = reader_.ReadByte();


            uint uniqueid = (uint)Monsters.MonsterAmount + 1000;
            if (monstertype == 1)
                monstertype = 0;

            Silkroad.Object_ tmpMonster = Silkroad.GetObjectById(monsterid);

            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SPAWN);

            writer.AppendDword(monsterid);
            writer.AppendDword(uniqueid);

            writer.AppendByte(Player.Position[Index_].XSector);
            writer.AppendByte(Player.Position[Index_].YSector);
            writer.AppendFloat(Player.Position[Index_].X);
            writer.AppendFloat(Player.Position[Index_].Z);
            writer.AppendFloat(Player.Position[Index_].Y);

            writer.AppendWord(0);
            writer.AppendByte(0);
            writer.AppendByte(1);
            writer.AppendByte(0);
            writer.AppendWord(0);
            writer.AppendWord(1);
            writer.AppendByte(0);

            writer.AppendFloat(tmpMonster.Speed);
            writer.AppendFloat(tmpMonster.Speed);
            writer.AppendFloat(tmpMonster.Speed);

            writer.AppendByte(0);
            writer.AppendByte(0);

            writer.AppendByte(monstertype);
            writer.AppendByte(1);
            byte[] tmpBuffer = writer.getWorkspace();
            //   ServerSocket.SendPacketInRange(writer.getWorkspace(),Index_);


            int monsterhp = (int)tmpMonster.Hp;
            ulong exp = tmpMonster.Exp;
            switch (monstertype)
            {
                case 2:
                    monsterhp *= 2;
                    exp *= 2;
                    break;

                case 3:
                    exp *= 7;
                    break;

                case 4:
                    monsterhp *= 20;
                    exp *= 3;
                    break;

                case 6:
                    monsterhp *= 4;
                    exp *= 4;
                    break;

                case 16:
                    monsterhp *= 10;
                    exp *= 5;
                    break;

                case 17:
                    monsterhp *= 20;
                    exp *= 6;

                    break;

                case 20:
                    monsterhp *= 200;
                    exp *= 8;
                    break;
            }

            Monsters.General[MonsterIndex].ID = monsterid;
            Monsters.General[MonsterIndex].UniqueID = uniqueid;
            Monsters.General[MonsterIndex].Type = monstertype;
            Monsters.General[MonsterIndex].HP = monsterhp;
            Monsters.Position[MonsterIndex].XSector = Player.Position[Index_].XSector;
            Monsters.Position[MonsterIndex].YSector = Player.Position[Index_].YSector;
            Monsters.Position[MonsterIndex].X = Player.Position[Index_].X;
            Monsters.Position[MonsterIndex].Z = Player.Position[Index_].Z;
            Monsters.Position[MonsterIndex].Y = Player.Position[Index_].Y;
            Monsters.General[MonsterIndex].Exp = exp;
            Monsters.General[MonsterIndex].Level = tmpMonster.Level;
            Monsters.General[MonsterIndex].Skills = new ArrayList();
            Monsters.General[MonsterIndex].Dead = false;

            for (int i = 0; i < Player.PlayersOnline; i++)
            {
                if (Player.General[i].CharacterID != 0)
                {
                    if (Formula.CalculateDistance(Monsters.Position[MonsterIndex], Player.Position[i]) <= 800)
                    {
                        ServerSocket.Send(tmpBuffer, i);
                        Player.Objects[i].SpawnedMonsterIndex.Add(MonsterIndex);
                    }
                }
            }


            uint[] skill = new uint[9];
            skill[0] = tmpMonster.Skill1;
            skill[1] = tmpMonster.Skill2;
            skill[2] = tmpMonster.Skill3;
            skill[3] = tmpMonster.Skill4;
            skill[4] = tmpMonster.Skill5;
            skill[5] = tmpMonster.Skill6;
            skill[6] = tmpMonster.Skill7;
            skill[7] = tmpMonster.Skill8;
            skill[8] = tmpMonster.Skill9;

            for (int i = 0; i <= 8; i++)
            {
                if (skill[i] != 0 && skill[i] <= 3000)
                    Monsters.General[MonsterIndex].Skills.Add(skill[i]);
            }

            Monsters.MonsterAmount++;

            if (monstertype == 3)
                Unique.OnUnique(monsterid, false, null);


            Timers.MonsterMovement[MonsterIndex].Interval = 5000;
            Timers.MonsterMovement[MonsterIndex].Start();
        }

        public static void OnDeSpawn(uint monsterid)
        {
            if (monsterid != 0)
            {
                PacketWriter writer = new PacketWriter();
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_DESPAWN);
                writer.AppendDword(monsterid);
                int ObjectIndex = Players.GetObjectIndex(monsterid);
                ServerSocket.SendPacketIfMonsterIsSpawned(writer.getWorkspace(), ObjectIndex);
            }
        }

        public static void OnDeSpawn(int Index_, uint ObjectId)
        {
            if (ObjectId != 0)
            {
                int ObjectIndex = Players.GetObjectIndexAndType(Index_, ObjectId);

                Timers.MonsterAttack[ObjectIndex].Stop();
                Timers.MonsterMovement[ObjectIndex].Stop();

                Monsters.General[ObjectIndex] = new Monsters._General();
                Monsters.Position[ObjectIndex] = new _Position();

                for (int i = 0; i < Player.PlayersOnline; i++)
                {
                    if (Player.General[i].UniqueID != 0 && Player.Objects[i].SpawnedMonsterIndex.Contains(ObjectIndex))
                    {
                        ServerSocket.Send(Players.CreateDeSpawnPacket(ObjectId), i);
                        Player.Objects[i].SpawnedMonsterIndex.Remove(ObjectIndex);

                    }
                }
            }
        }
    }
}

