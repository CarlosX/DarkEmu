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
    unsafe class PacketReader
    {
        private static int readIndex = 0;
        private static int maxReadIndex = 0;
        private static byte[] workspace;

        public PacketReader(byte[] buffer, int count)
        {
            workspace = buffer;
            maxReadIndex = count;
            readIndex = 0;
        }

        public PacketReader(byte* buffer,int count)
        {
            Marshal.Copy(new IntPtr(buffer),workspace,0,count);
            maxReadIndex = count;
            readIndex = 0;
        }

        public byte ReadByte()
        {
            fixed (byte* ptr_workspace = workspace)
            {
                byte tmp = *((byte*)(ptr_workspace + readIndex));
                readIndex++;
                return tmp;
            }
        }

        public ushort ReadWord()
        {
            fixed (byte* ptr_workspace = workspace)
            {
                ushort tmp = *((ushort*)(ptr_workspace + readIndex));
                readIndex += 2;
                return tmp;
            }
        }

        public uint ReadDword()
        {
            fixed (byte* ptr_workspace = workspace)
            {
                uint tmp = *((uint*)(ptr_workspace + readIndex));
                readIndex += 4;
                return tmp;
            }
        }

        public ulong ReadLword()
        {
            fixed (byte* ptr_workspace = workspace)
            {
                ulong tmp = *((ulong*)(ptr_workspace + readIndex));
                readIndex += 8;
                return tmp;
            }
        }

        public float ReadFloat()
        {
            fixed (byte* ptr_workspace = workspace)
            {
                float tmp = *((float*)(ptr_workspace + readIndex));
                readIndex += 4;
                return tmp;
            }
        }

        public double ReadDouble()
        {
            fixed (byte* ptr_workspace = workspace)
            {
                double tmp = *((double*)(ptr_workspace + readIndex));
                readIndex += 8;
                return tmp;
            }
        }

        public string ReadString(bool unicode,int count)
        { 
            byte[] tmp = new byte[count];
            for (int i = 0; i < count; i++)
                tmp[i] = ReadByte();
            if (unicode)
                return Encoding.Unicode.GetString(tmp);
            else
                return Encoding.ASCII.GetString(tmp);
        }

        public int GetBytesLeft()
        {
            return maxReadIndex - readIndex;
        }

        public void ModifyIndex(int i)
        {
            if (readIndex + i >= 0 && readIndex + i < maxReadIndex)
                readIndex += i;
        }

        public void SetIndex(int i)
        {
            if (i >= 0 && i < maxReadIndex)
                readIndex = i;
        }

        public byte[] getWorkspace()
        {
            return workspace;
        }
        public int getIndex()
        {
            return readIndex;
        }

        public void SetToNull()
        {
            workspace = new byte[0];
            readIndex = 0;
            maxReadIndex = 0;
        }
    }

}