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

#ifndef _SREMU_BINARY_READER_H_
#define _SREMU_BINARY_READER_H_

#include "../common.h"

// Binary reader is a *VERY* lightweight class that simply attaches to a
// binary buffer and lets you read out objectively any type you want :>

enum READ_RESULT {
	READ_EOF,
	READ_SUCCESS,
	READ_NOBUFFER
};

class SREMU_SHARED Deserializable {
protected:
	friend class BinaryReader;
	virtual READ_RESULT deserialize(BinaryReader* reader) =0;
};

class SREMU_SHARED BinaryReader {
public:
	BinaryReader(ubyte* buf=0,uint32 length=0):curbuf(buf),size(length),ptr(0){}

	/**
	 * Pointer manipulation functions
	 **/
	// Return current position of pointer
	uint32 tell() {
		return ptr;
	}

	ubyte* get_buffer() {
		return curbuf;
	}

	uint32 get_size() {
		return size;
	}

	// Seek to the indicated position (from < 0, seek backward from current, from=0, seek to pos, from>0, seek forward from current)
	READ_RESULT seek(uint32 amt,int from) {
		if(!curbuf)return READ_NOBUFFER;

		if(from<0){
			if(amt>=ptr)ptr=0;
			else ptr -= amt;
		}else if(from>0){
			if((amt+ptr)>=size)ptr=size;
			else ptr += amt;
		}else{
			if(amt >= size)ptr=size;
			else ptr=amt;
		}
		return READ_SUCCESS;
	}

	// Rewind to beginning of buffer
	READ_RESULT rewind() {
		if(!curbuf)return READ_NOBUFFER;

		ptr=0;

		return READ_SUCCESS;
	}

	// Load a new buffer into the reader
	void reload(ubyte* buf,uint32 length){
		curbuf=buf;
		size=length;
		ptr=0;
	}

	/**
	 * Reading functions
	 **/
	READ_RESULT read_raw(ubyte* out,uint32 len) {
		if(!curbuf) return READ_NOBUFFER;

		if(len>(size-ptr))return READ_EOF;

		memcpy(out,curbuf+ptr,len);
		ptr+=len;

		return READ_SUCCESS;
	}


	template <typename T>
	T read_basic() {
		if(!curbuf) return T(0);

		if(sizeof(T)>(size-ptr)) return T(0);		// Teh fail, buffer overflow prevention

		T retval = *((T*)(curbuf+ptr));
		ptr += sizeof(T);

		return retval;
	}

	template <typename T>
	READ_RESULT read_copy(T* t) {
		if(!curbuf)return READ_NOBUFFER;

		if(sizeof(T)>(size-ptr)) return READ_EOF;		// Teh fail, buffer overflow prevention

		memcpy(t,(curbuf+ptr),sizeof(T));

		ptr+=sizeof(T);
		return READ_SUCCESS;
	}

	// object reading deserializes an object from the stream, KEWL!
	READ_RESULT read_object(Deserializable* obj) {
		return obj->deserialize(this);
	}

	template <typename T>
	BinaryReader& operator >>(T& obj){
		read_copy<T>((T*)&obj);
		return *this;
	}

private:
	ubyte* curbuf;
	uint32 size;
	uint32 ptr;
};

#endif // #ifndef _SREMU_BINARY_READER_H_