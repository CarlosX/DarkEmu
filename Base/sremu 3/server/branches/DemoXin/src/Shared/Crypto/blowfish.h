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

// This implementation is simply a C++ conversion of Bruce Schneier's original blowfish c implementation.
// currently UNUSED (and not finished!), just here for future purposes!  don't try compiling it just yet ;O

#ifndef _SREMU_BLOWFISH_H_
#define _SREMU_BLOWFISH_H_

#ifdef USE_SECURITY
#if USE_SECURITY&2

#define BF_LITTLE_ENDIAN 1              /* Eg: Intel */
//#define BF_BIG_ENDIAN 1            /* Eg: Motorola */

class Blowfish {
	static unsigned long p[18];
	static unsigned long s[4][256];

	unsigned long _p[18];
	unsigned long _s[4][256];

	void encipher_block(unsigned long* l,unsigned long* r);
	void decipher_block(unsigned long* l,unsigned long* r);
	unsigned long F(unsigned long f);
public:
	Blowfish();

	void update(const char* key,unsigned int keylen);
	void reset();
	void encipher(const char* data,unsigned int len,char* out);
	void decipher(const char* data,unsigned int len,char* out);
};

#endif // #if USE_SECURITY&2
#endif // #ifdef USE_SECURITY

#endif // #ifndef _SREMU_BLOWFISH_H_