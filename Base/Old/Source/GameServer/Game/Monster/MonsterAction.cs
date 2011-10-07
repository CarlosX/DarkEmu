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
    class MonsterAction
    {
        private static Random random = new Random();

        public static void MonsterMovement(int Index_)
        {
            int X = random.Next(-20, 20);
            int Z = random.Next(-5, 10);
            int Y = random.Next(-20, 20);

            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_MOVEMENT);
            writer.AppendDword(Monsters.General[Index_].UniqueID);
            writer.AppendByte(1);
            writer.AppendByte(Monsters.Position[Index_].XSector);
            writer.AppendByte(Monsters.Position[Index_].YSector);
            writer.AppendWord((ushort)(Monsters.Position[Index_].X + X));
            writer.AppendWord((ushort)(Monsters.Position[Index_].Z + Z));
            writer.AppendWord((ushort)(Monsters.Position[Index_].Y + Y));
            writer.AppendByte(0);

            byte[] tmpBuffer = writer.getWorkspace();

            for (int i = 0; i < Player.PlayersOnline; i++)
            {
                if (Player.General[i].CharacterID != 0 && Player.Objects[i].SpawnedMonsterIndex.Contains(Index_))
                {
                    if (Formula.CalculateDistance(Monsters.Position[Index_], Player.Position[i]) <= 800)                    
                        ServerSocket.Send(tmpBuffer, i);                    
                }
            }
        }

        public static void CheckAttack(int Index_)
        {
            Timers.MonsterMovement[Index_].Stop();

            if (Player.General[Monsters.General[Index_].AttackingObjectIndex].User != null)
            {
                if (Movement.MoveToObject(Index_, ref Monsters.Position[Index_], Player.Position[Monsters.General[Index_].AttackingObjectIndex], Monsters.General[Index_].UniqueID,true))
                    Attack.OnMonsterAttack(Index_);
            }
        }
    }
}
