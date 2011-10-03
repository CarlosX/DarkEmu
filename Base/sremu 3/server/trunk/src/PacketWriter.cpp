/* 
 * Copyright (C) 2005-2008 SREmu <http://www.sremu.org/>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#include "PacketWriter.h"

PacketWriter::PacketWriter() 
{
	offset = 0;
}

void PacketWriter::Skip(int bytes)
{
	offset += bytes;
}

void PacketWriter::Reset() 
{
	offset = 0;
}

void PacketWriter::Create(unsigned short opcode) 
{
	Reset();
	Skip(2);
	WriteWord(opcode);
	WriteWord(0);
}

void PacketWriter::WriteByte(unsigned char a) 
{
	offset++;
	Buffer[offset-1] = a;
}

void PacketWriter::WriteWord(unsigned short a) 
{
	offset += 2;
	*(unsigned short*)(Buffer+(offset-2)) = a;
}

void PacketWriter::WriteDWord(unsigned int a) 
{
	offset += 4;
	*(unsigned int*)(Buffer+(offset-4)) = a;
}

void PacketWriter::WriteString(unsigned char* a, unsigned int size) 
{
	for(unsigned int i = 0; i < size; i++, offset++) 
	{
		Buffer[offset] = a[i];
	}
}

void PacketWriter::Finalize() 
{
	*(unsigned short*)(Buffer) = offset - 6;
}

unsigned int PacketWriter::Size() 
{
	return offset;
}

void PacketWriter::WriteQWord(__int64 a) 
{
	offset += 8;
	*(__int64*)(Buffer+(offset-8)) = a;
}

void PacketWriter::WriteFloat(float a) 
{
	offset += 4;
	*(float *)(Buffer+(offset-4)) = a;
}

void PacketWriter::WriteUString(unsigned char* a, unsigned int size) 
{
	for(unsigned int i = 0; i < size; i++, offset+=2) 
	{
		Buffer[offset]   = a[i];
		Buffer[offset+1] = 0;
	}
}