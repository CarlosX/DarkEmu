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

class PacketWriter 
{
private:

	int	offset;

	void Reset();
	void Skip(int bytes);

public:

	unsigned char Buffer[8096];

	unsigned int Size();

	PacketWriter();

	void Create(unsigned short opcode);

	void WriteByte	(unsigned char a);
	void WriteWord	(unsigned short a);
	void WriteDWord	(unsigned int a);
	void WriteQWord	(__int64 a);
	void WriteFloat	(float a);

	void WriteString (unsigned char* a, unsigned int size);	// Ascii
	void WriteUString(unsigned char* a, unsigned int size);	// Unicode

	void Finalize();
};