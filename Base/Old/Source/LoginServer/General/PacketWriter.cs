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
    unsafe class PacketWriter
    {
        private static int index = 0;
        private static int MAX_BUFFER_SIZE = 8192;
        private static byte[] workspace = new byte[MAX_BUFFER_SIZE];
        private static ushort opcode_;

        public PacketWriter()
        {
            index = 6;
            workspace = new byte[MAX_BUFFER_SIZE];
            opcode_ = 0;
        }


        public void SetOpcode(ushort opcode)
        {
            Reset();
            fixed (byte* ptr_workspace = workspace)
                *(ushort*)(ptr_workspace + 2) = opcode;
            opcode_ = opcode;
        }

        public void AppendByte(byte item)
        {
            fixed (byte* ptr_workspace = workspace)
                *(byte*)(ptr_workspace + index) = item;
            index += 1;
        }
        public void AppendWord(ushort item)
        {
            fixed (byte* ptr_workspace = workspace)
                *(ushort*)(ptr_workspace + index) = item;
            index += 2;
        }

        public void AppendDword(uint item)
        {
            fixed (byte* ptr_workspace = workspace)
                *(uint*)(ptr_workspace + index) = item;
            index += 4;
        }

        public void AppendLword(ulong item)
        {
            fixed (byte* ptr_workspace = workspace)
                *(ulong*)(ptr_workspace + index) = item;
            index += 8;
        }

        public void AppendFloat(float item)
        {
            fixed (byte* ptr_workspace = workspace)
                *(float*)(ptr_workspace + index) = item;
            index += 4;
        }

        public void AppendDouble(double item)
        {
            fixed (byte* ptr_workspace = workspace)
                *(double*)(ptr_workspace + index) = item;
            index += 8;
        }

        public void AppendString(bool unicode, string item)
        {
            byte[] tmp;
            if (unicode)
                tmp = Encoding.Unicode.GetBytes(item);
            else
                tmp = Encoding.ASCII.GetBytes(item);

            fixed (byte* ptr_workspace = workspace)
            {
                for (byte i = 0; i < tmp.Length; i++)
                    AppendByte(tmp[i]);
            }
        }
        public void Skip(int bytes)
        {
            index += bytes;
        }

        public void SetWriteIndex(int newIndex)
        {
            if (newIndex < 0 || newIndex >= MAX_BUFFER_SIZE)
                throw new Exception("SetWriteIndex invalid index.");
            index = newIndex;
        }

        public int getIndex()
        {
            return index;
        }

        public ushort getOpcode()
        {
            return opcode_;
        }

        public byte[] getWorkspace()
        {
            ushort tmpindex = (ushort)(index - 6);

            fixed (byte* ptr_workspace = workspace)
                *(ushort*)(ptr_workspace) = tmpindex;

            Array.Resize<byte>(ref workspace, index);

            return workspace;
        }

        public void Reset()
        {
            index = 6;
            opcode_ = 0;
            workspace = new byte[MAX_BUFFER_SIZE];
        }

    }
}
