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

#include "memory.h"

SimpleBuffer::SimpleBuffer(void* c_buffer, uint size):_buffer((ubyte*)c_buffer),_length(size),_cur(0),_max(0){}

// Reading
template <typename T>
T __inline genericRead(ubyte* stream,uint& pos,uint len){
	if(pos + sizeof(T) > len) return T(0);

	T val = *(T*)(&stream[pos]);
	pos += sizeof(T);

	return val;
}

template <typename T>
void __inline genericWrite(ubyte* stream,T x,uint& pos,uint& max,uint len){
	if(pos + sizeof(T) > len) return;

	*(T*)(&stream[pos]) = x;
	pos += sizeof(T);

	if(pos > max) max = pos;
}

bool SimpleBuffer::readByteArray(void* arr, uint len){
	if(_cur+len > _length) return false;

	memcpy(arr,&_buffer[_cur],len);
	_cur += len;
	return true;
}

ubyte	SimpleBuffer::readByte()	{return genericRead<ubyte>(_buffer,_cur,_length);}
ushort	SimpleBuffer::readWord()	{return genericRead<ushort>(_buffer,_cur,_length);}
uint	SimpleBuffer::readDWord()	{return genericRead<uint>(_buffer,_cur,_length);}
uqword	SimpleBuffer::readQWord()	{return genericRead<uqword>(_buffer,_cur,_length);}
real32	SimpleBuffer::readReal32()	{return genericRead<real32>(_buffer,_cur,_length);}
real64	SimpleBuffer::readReal64()	{return genericRead<real64>(_buffer,_cur,_length);}

// Writing
SimpleBuffer& SimpleBuffer::writeByteArray(const void* arr,uint len){
	if(_cur+len > _length) return *this;

	memcpy(&_buffer[_cur],arr,len);
	_cur += len;
	if(_cur > _max) _max = _cur;

	return *this;
}

SimpleBuffer& SimpleBuffer::writeByte(ubyte x)		{genericWrite<ubyte>(_buffer,x,_cur,_max,_length); return *this;}
SimpleBuffer& SimpleBuffer::writeWord(ushort x)		{genericWrite<ushort>(_buffer,x,_cur,_max,_length); return *this;}
SimpleBuffer& SimpleBuffer::writeDWord(uint x)		{genericWrite<uint>(_buffer,x,_cur,_max,_length); return *this;}
SimpleBuffer& SimpleBuffer::writeQWord(uqword x)	{genericWrite<uqword>(_buffer,x,_cur,_max,_length); return *this;}
SimpleBuffer& SimpleBuffer::writeReal32(real32 x)	{genericWrite<real32>(_buffer,x,_cur,_max,_length); return *this;}
SimpleBuffer& SimpleBuffer::writeReal64(real64 x)	{genericWrite<real64>(_buffer,x,_cur,_max,_length); return *this;}

void SimpleBuffer::reload(void* c_buffer, uint size){
	_buffer = (ubyte*)c_buffer;
	_length = size;
	reset();
}

void SimpleBuffer::seek(int amt, bool current){
	if(current){
		if(amt > 0){
			if((uint)amt > remain())
				_cur = _length;
			else
				_cur += (uint)amt;
		}else{
			amt = -1 * amt;
			if((uint)amt > _cur) _cur = 0;
			else _cur += (uint)amt;
		}
	}else{
		if(amt <= 0) return;

		if((uint)amt > remain()) _cur = _length;
		else
			_cur += (uint)amt;
	}
}