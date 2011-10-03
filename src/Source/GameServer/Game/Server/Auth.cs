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
    class Auth
    {
        private static PacketWriter writer = new PacketWriter();
        private static PacketReader reader;

        public static void SendServerInfo(int Index_)
        {
            string name = "AgentServer";
            ushort namelen = (ushort)name.Length;

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_INFO);
            writer.AppendWord(namelen);
            writer.AppendString(false, name);
            writer.AppendByte(0);
            ServerSocket.Send(writer.getWorkspace(), Index_);

        }

        public static void SendPatchInfo(int Index_)
        {
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_PATCH_INFO);
            writer.AppendDword(0x05000101);
            writer.AppendByte(0x20);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_PATCH_INFO);
            writer.AppendDword(0x01000100);
            writer.AppendDword(0x00050628);
            writer.AppendWord(0);
            writer.AppendByte(2);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_PATCH_INFO);
            writer.AppendDword(0x05000101);
            writer.AppendByte(0x60);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_PATCH_INFO);
            writer.AppendDword(0x02000300);
            writer.AppendWord(0x0200);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_PATCH_INFO);
            writer.AppendDword(0x00000101);
            writer.AppendByte(0xA1);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_PATCH_INFO);
            writer.AppendWord(0x0100);
            ServerSocket.Send(writer.getWorkspace(), Index_);
        }

        public static void OnAuth(PacketReader Reader_,int Index_)
        {
            reader = Reader_;

            uint session = reader.ReadDword();

            ushort userlen = reader.ReadWord();
            string user =      reader.ReadString(false, userlen);

            ushort passlen = reader.ReadWord();
            string pass = reader.ReadString(false, passlen);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_LOGIN_RESULT);

            int id = DatabaseCore.User.GetIndexByName(user);

            if (id != -1)
            {
                writer.AppendByte(0x01);
                Player.General[Index_].AccountID = DatabaseCore.User.UserId[id];
                Player.General[Index_].Index = Index_;
                Player.General[Index_].User = user;
                Player.General[Index_].Pass = pass;
            }
            else
            {
                writer.AppendByte(0x02);
                writer.AppendByte(0x01);
            }

            ServerSocket.Send(writer.getWorkspace(), Index_);
        }
    }
}
