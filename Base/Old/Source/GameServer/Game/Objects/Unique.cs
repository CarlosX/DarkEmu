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
    class Unique
    {
        public static void OnUnique(uint monsterid, bool kill, string name)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_UNIQUE);
            if (kill)
            {
                writer.AppendByte(6);
                writer.AppendDword(monsterid);
                writer.AppendWord((ushort)name.Length);
                writer.AppendString(false, name);
            }
            else
            {
                writer.AppendByte(5);
                writer.AppendDword(monsterid);
            }

            int ObjectIndex = Players.GetObjectIndex(monsterid);
            ServerSocket.SendPacketIfMonsterIsSpawned(writer.getWorkspace(), ObjectIndex);
        }

    }
}
