 /* 
  * This file is part of SREmu.
  *
  * Copyright (c) 2010 Justin "jMerliN" Summerlin, SREmu <http://sremu.sourceforge.net>
  *
  * SREmu is free software: you can redistribute it and/or modify
  * it under the terms of the GNU Affero General Public License as published by
  * the Free Software Foundation, either version 3 of the License, or
  * (at your option) any later version.
  * 
  * SREmu is distributed in the hope that it will be useful,
  * but WITHOUT ANY WARRANTY; without even the implied warranty of
  * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  * GNU Affero General Public License for more details.
  * 
  * You should have received a copy of the GNU Affero General Public License
  * along with SREmu.  If not, see <http://www.gnu.org/licenses/>.
  */

#ifndef _SREMU_MEMORY_H_
#define _SREMU_MEMORY_H_

#include "common.h"

// ================================================================
// == Read/Write Buffers ==
// ================================================================

class SimpleBuffer {
public:
	SimpleBuffer(void* c_buffer, uint size);

	// Reading
	ubyte	readByte();
	bool	readByteArray(void* arr, uint len);
	ushort	readWord();
	uint	readDWord();
	uqword	readQWord();
	real32	readReal32();
	real64	readReal64();

	// Writing
	SimpleBuffer& writeByte(ubyte x);
	SimpleBuffer& writeByteArray(const void* arr,uint len);
	SimpleBuffer& writeWord(ushort x);
	SimpleBuffer& writeDWord(uint x);
	SimpleBuffer& writeQWord(uqword x);
	SimpleBuffer& writeReal32(real32 x);
	SimpleBuffer& writeReal64(real64 x);

	// Buffer manip/info
	void reload(void* c_buffer, uint size);

	uint size()			{return _length;		}
	uint remain()		{return _length - _cur;	}
	uint wremain()		{return _length - _max;	}
	uint tell()			{return _cur;			}
	uint getMax()		{return _max;			}
	ubyte* getBuffer()	{return _buffer;		}
	void rewind()		{_cur = 0;				}
	void reset()		{_cur = _max = 0;		}

	// 0: from current, nonzer
	void seek(int amt, bool current = true);

private:
	ubyte* _buffer;
	uint _length;
	uint _cur;
	uint _max;
};

#endif // #ifndef _SREMU_MEMORY_H_