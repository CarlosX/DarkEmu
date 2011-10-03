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

#ifndef _SREMU_BINARY_WRITER_H_
#define _SREMU_BINARY_WRITER_H_

#include "../common.h"

// Binary writer is also a *VERY* lightweight class that simply attaches to a
// binary buffer and lets you write objectively any type you want :>

enum WRITE_RESULT {
	WRITE_NOSPACE,
	WRITE_SUCCESS,
	WRITE_NOBUFFER,
	WRITE_LOCKED
};

class SREMU_SHARED Serializable {
protected:
	friend class BinaryWriter;
	virtual WRITE_RESULT serialize(BinaryWriter* reader) =0;
};

class SREMU_SHARED BinaryWriter {
public:

	BinaryWriter(ubyte* buf=0,uint32 length=0):curbuf(buf),size(length),ptr(0),maxptr(0),can_write(true){}

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
		return maxptr;
	}

	// Seek to the indicated position (from < 0, seek backward from current, from=0, seek to pos, from>0, seek forward from current)
	void seek(uint32 amt,int from) {
		if(!curbuf)return;

		if(from<0){
			if(amt>=ptr)ptr=0;
			else ptr -= amt;
		}else if(from>0){
			if((amt+ptr)>=maxptr)ptr=maxptr;
			else ptr += amt;
		}else{
			if(amt >= maxptr)ptr=maxptr;
			else ptr=amt;
		}
	}

	// Rewind to beginning of buffer
	void rewind() {
		if(!curbuf)return;

		ptr=0;
	}

	// Load a new buffer into the reader
	void reload(ubyte* buf,uint32 length){
		curbuf=buf;
		size=length;
		maxptr=ptr=0;
	}

	/**
	 * Writing functions
	 **/
	WRITE_RESULT write_raw(const ubyte* in,uint32 len) {
		if(!curbuf) return WRITE_NOBUFFER;
		if(!can_write)return WRITE_LOCKED;

		if(len>(size-ptr)) {
			if(!on_overflow())
				return WRITE_NOSPACE;
		}

		memcpy(curbuf+ptr,in,len);
		ptr+=len;
		if(ptr>maxptr)maxptr=ptr;

		return WRITE_SUCCESS;
	}


	template <typename T>
	WRITE_RESULT write_basic(const T& t) {
		if(!curbuf) return WRITE_NOBUFFER;
		if(!can_write)return WRITE_LOCKED;

		if(sizeof(T)>(size-ptr)) {
			if(!on_overflow())
				return WRITE_NOSPACE;
		}

		*((T*)(curbuf+ptr))=t;
		ptr += sizeof(T);
		if(ptr>maxptr)maxptr=ptr;

		return WRITE_SUCCESS;
	}

	// object writing serializes an object to the stream, KEWL!
	WRITE_RESULT write_object(Serializable* obj) {
		return obj->serialize(this);
	}

	template <typename T>
	BinaryWriter& operator<<(const T& obj){
		if(!can_write)return *this;	// Wut?

		write_basic<T>(obj);
		return *this;
	}

protected:
	// Returns true if overflow was handled, false if overflow was not handled.
	virtual bool on_overflow() {
		return false;
	}

	void block_write(){
		can_write=false;
	}

	void allow_write(){
		can_write=true;
	}

private:
	// Data
	ubyte* curbuf;			// Stores current buffer
	uint32 size;			// size of current buffer
	uint32 ptr;				// offset in current buffer
	uint32 maxptr;			// maximum reached size in buffer
	bool can_write;
};

#endif // #ifndef _SREMU_BINARY_WRITER_H_