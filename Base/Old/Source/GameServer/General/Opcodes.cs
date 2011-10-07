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
    class CLIENT_OPCODES
    {
        public const ushort GAME_CLIENT_INFO = 0x2001,
        GAME_CLIENT_KEEP_ALIVE = 0x2002,
        GAME_CLIENT_PATCH_REQUEST = 0x6100,
        GAME_CLIENT_AUTH = 0x6103,
        GAME_CLIENT_ITEM_MOVE = 0x7106,
        GAME_CLIENT_INGAME_NOTIFY = 0x769E,
        GAME_CLIENT_CLOSE = 0x7527,
        GAME_CLIENT_CHARACTER = 0x07007,
        GAME_CLIENT_CHAT = 0x77E3,
        GAME_CLIENT_INGAME_REQUEST = 0x7001,//0x7435,
        GAME_CLIENT_TARGET = 0x7111,
        GAME_CLIENT_GM = 0x769A,
        GAME_CLIENT_MOVEMENT = 0x7604,
        GAME_CLIENT_ACCEPT_HANDSHAKE = 0x9000,
        GAME_CLIENT_PLAYER_ACTION = 0x7145,
        GAME_CLIENT_STR_UPDATE = 0x766F,
        GAME_CLIENT_INT_UPDATE = 0x75EF,
        GAME_CLIENT_CHARACTER_STATE = 0x7432,
        GAME_CLIENT_RESPAWN = 0x330E,
        GAME_CLIENT_MASTERYUPDATE = 0x710D,
        GAME_CLIENT_SKILLUPDATE = 0x7747,
        GAME_CLIENT_EMOTION = 0x3283,
        GAME_CLIENT_GAME_START = 0x7001,
        GAME_CLIENT_ITEM_USE = 0x71BA;
    }
    class SERVER_OPCODES
    {
        public const ushort
        GAME_SERVER_INFO = 0x2001,
        GAME_SERVER_HANDSHAKE = 0x5000,
        GAME_SERVER_PATCH_INFO = 0x600D,
        GAME_SERVER_LOGIN_RESULT = 0xA103,

        GAME_SERVER_CHARACTER = 0xB007,
        GAME_SERVER_CHARDATA = 0x3013,
        GAME_SERVER_INGAME_ACCEPT = 0xB001,
        GAME_SERVER_LOADING_START = 0x34A5,
        GAME_SERVER_LOADING_END = 0x34A6,
        GAME_SERVER_CHARACTER_CELESTIAL_POSITION = 0x3020,

        GAME_SERVER_SPAWN = 0x329D,
        GAME_SERVER_DESPAWN = 0x37F2,

        GAME_SERVER_ITEM_EQUIP = 0x30EE,
        GAME_SERVER_ITEM_UNEQUIP = 0x369F,
        GAME_SERVER_ITEM_MOVEMENT = 0xB106,
        GAME_SERVER_NEW_GOLD_AMOUNT = 0x3732,
        GAME_SERVER_ANIMATION_ITEM_PICKUP = 0x3310,
        GAME_SERVER_ITEM_USE = 0xB1BA,
        GAME_SERVER_ANIMATION_POTION = 0x31D6,
        GAME_SERVER_ANIMATION_CAPE = 0x30FA,

        GAME_SERVER_QUIT_GAME = 0x317F,
        GAME_SERVER_COUNTDOWN = 0xB527,

        GAME_SERVER_STATS = 0xBD9C,//0x303D,//0x37D8,
        GAME_SERVER_STR_UPDATE = 0xB66F,
        GAME_SERVER_INT_UPDATE = 0xB5EF,
        GAME_SERVER_CHARACTER_STATE = 0x3454,
        GAME_SERVER_HPMP_UPDATE = 0x30BA,
        GAME_SERVER_ANIMATION_LEVEL_UP = 0x3169,
        GAME_SERVER_EXP = 0x3380,
        GAME_SERVER_MASTERYUPDATE = 0xB10D,
        GAME_SERVER_SKILLPOINTS = 0x3732,
        GAME_SERVER_SKILLUPDATE = 0xB747,

        GAME_SERVER_CHAT = 0x343F,
        GAME_SERVER_CHAT_ACCEPT = 0xB7E3,

        GAME_SERVER_TARGET = 0xB111,
        GAME_SERVER_MOVEMENT = 0xB604,
        GAME_SERVER_UNIQUE = 0x30C2,

        GAME_SERVER_ATTACK = 0xB099,
        GAME_SERVER_SKILL_ATTACK = 0xB145,
        GAME_SERVER_END_SKILL = 0xB2DB,

        GAME_SERVER_BUFF_1 = 0xB02F,
        GAME_SERVER_DEAD = 0x326C,
        GAME_SERVER_DEAD2 = 0x332A;

    }
}
