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
    class Buffs
    {
        private static PacketWriter writer = new PacketWriter();
        private static Random random = new Random();

        public static void BeginBuff(int Index_)
        {
            uint SkillOverID = (uint)random.Next(65536, 1048575);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_END_SKILL);
            writer.AppendByte(1);
            writer.AppendDword(Player.Objects[Index_].BuffCastingID);
            writer.AppendByte(0);
            writer.AppendDword(0);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_BUFF_1);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendDword(Player.Objects[Index_].UsingSkillID);
            writer.AppendDword(SkillOverID);

            ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

            for (byte i = 0; i <= 19; i++)
            {
                if (Player.Objects[Index_].ActiveBuffs[i].Id == 0)
                {
                    Player.Objects[Index_].ActiveBuffs[i].Id = Player.Objects[Index_].UsingSkillID;
                    Player.Objects[Index_].ActiveBuffs[i].OverId = SkillOverID;
                }
            }

            Player.Objects[Index_].UsingSkill = false;

        }
    }
}
