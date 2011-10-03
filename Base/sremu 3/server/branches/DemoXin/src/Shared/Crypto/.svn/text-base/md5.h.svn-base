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

#ifndef _SREMU_MD5_H_
#define _SREMU_MD5_H_

#include "../common.h"

// NOTE:  All 'out' variables need to have 4 dwords of memory allocated!
class SREMU_SHARED MD5 {
public:
	// Digests a binary string "in" with length "len" and puts the result in "out"
	void digest(const char* in,unsigned long len,unsigned char* out);

	// Digests a unicode string "in" with length "len" and puts the result in "out"
	void digest(const wchar_t* in,unsigned long len,unsigned char* out);

	// Digests a file with name "f" and puts the result in "out"
	void fdigest(const char* f,unsigned char* out);

private:
	void init();
	void update(const unsigned char* inp,unsigned long len);
	void final();

	void transform(const unsigned char* block);
	void encode(unsigned char* out,const unsigned long* in,unsigned long len);
	void decode(unsigned long* out,const unsigned char* in,unsigned long len);

	unsigned long state[4];
	unsigned long count[2];
	unsigned char buffer[64];
};

#endif // #ifndef _SREMU_MD5_H_