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

#ifndef _SREMU_SILKROAD_CRYPT_H_
#define _SREMU_SILKROAD_CRYPT_H_

// TODO: implement handshake, final level of sro security

#ifdef USE_SECURITY
#if USE_SECURITY&1

class SilkroadCrypt {
	static unsigned long* table;
	static unsigned long base_table[256];

	unsigned long mutate(unsigned long* m);

	unsigned char cnt_bytes[3];
	unsigned long check_seed;
public:
	static void startup();
	static void shutdown();

	void init(unsigned long cnt_seed,unsigned long chk_seed);
	unsigned char get_counter(void);
	unsigned char get_checksum(const char* data,unsigned int len);
};

#endif // #if USE_SECURITY&1
#endif // #ifdef USE_SECURITY

#endif // #ifndef _SREMU_SILKROAD_CRYPT_H_