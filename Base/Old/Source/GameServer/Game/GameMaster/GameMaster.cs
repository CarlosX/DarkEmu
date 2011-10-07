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
    class GameMaster
    {
        public static void OnGM(PacketReader reader_, int Index_)
        {
            if (Player.Flags[Index_].GM == 1)
            {
                byte id = reader_.ReadByte();

                switch (id)
                {
                    case 6:
                        MonsterSpawn.OnSpawn(reader_, Index_);
                        break;
                    case 7:
                        Items.CreateItem(reader_, Index_);
                        break;
                    case 13:
                        BanPlayer(reader_, Index_);
                        break;
                    case 14:
                        OnInvisible(Index_);
                        break;

                    case 20:
                        MonsterSpawn.KillMonster(reader_, Index_);
                        break;
                }
            }
        }

        private static void OnInvisible(int Index_)
        {
            if (Player.Flags[Index_].Invisible)
            {
                Character.SpawnMe(Index_);
                Character.OnState(Index_, 4, 13);
                Player.Flags[Index_].Invisible = false;
            }
            else
            {
                Character.DeSpawnMe(Index_);
                Character.OnState(Index_, 4, 4);
                Player.Flags[Index_].Invisible = true;
            }
        }

        private static void BanPlayer(PacketReader reader_, int Index_)
        {
            ushort charlen = reader_.ReadWord();

            string name = reader_.ReadString(false,charlen);

            bool banned = false;

            for (int i = 0; i <= Player.PlayersOnline; i++)
            {
                if (name == Player.General[i].CharacterName && Index_ != i)
                {
                    ServerSocket.DisconnectSocket(i);
                    DatabaseCore.WriteQuery("UPDATE user SET banned='1' WHERE name='{0}'",Player.General[i].User);
                    banned = true;
                }
            }

            if (!banned)            
                DatabaseCore.WriteQuery("UPDATE user SET banned='1' WHERE name='{0}'",name);            
        }

    }
}