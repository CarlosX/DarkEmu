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
using System.Runtime.InteropServices;

namespace DarkEmu_LoginServer
{
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
    unsafe public struct TPacket
    {
        public ushort size;
        public ushort opcode;
        public byte securityCount;
        public byte securityCRC;
        public fixed byte data[4000];
    }

    unsafe class Silkroad
    {
        public static void SendHandshake(int ClientIndex)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_HANDSHAKE);
            writer.AppendByte(1);//NO HANDSHAKE
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);           
        }

        public static TPacket* ToTPacket(byte[] src)
        {
            fixed (byte* tmp = src)
                return (TPacket*)tmp;
        }
    }
}
