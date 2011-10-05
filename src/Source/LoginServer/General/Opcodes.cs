/*    <DarkEmu LoginServer>
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

namespace DarkEmu_LoginServer
{
    class CLIENT_OPCODES
    {
        public const ushort LOGIN_CLIENT_INFO = 0x2001,
        LOGIN_CLIENT_KEEP_ALIVE = 0x2002,
        LOGIN_CLIENT_PATCH_REQUEST = 0x6100,
        LOGIN_CLIENT_SERVERLIST_REQUEST = 0x6101,
        LOGIN_CLIENT_AUTH = 0x6102,
        LOGIN_CLIENT_ACCEPT_HANDSHAKE = 0x9000,
        LOGIN_CLIENT_LAUNCHER = 0x6104,

        LOGIN_CLIENT_AUTH_UNK1 = 0x9CC7,

        LOGIN_CLIENT_LAUNCHER_UNK1 = 0xB405;
    }

    class SERVER_OPCODES
    {
        public const ushort LOGIN_SERVER_INFO = 0x2001,
        LOGIN_SERVER_HANDSHAKE = 0x5000,
        LOGIN_SERVER_PATCH_INFO = 0x600D,
        LOGIN_SERVER_LAUNCHER = 0x600D,
        LOGIN_SERVER_LIST = 0xA101,
        LOGIN_SERVER_AUTH_INFO = 0xA102,

        LOGIN_SERVER_AUTH_UNK1 = 0xA107,

        LOGIN_SERVER_LAUNCHER_UNK1 = 0x7805;
    }
}
