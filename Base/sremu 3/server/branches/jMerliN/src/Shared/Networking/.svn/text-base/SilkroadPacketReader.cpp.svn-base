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

#include "SilkroadPacketReader.h"

SilkroadPacketReader::SilkroadPacketReader(uint32 size):bufsize(size),pack_count(0),pack_base(0),pack_end(0),pos(0){
	// Point of optimization
	buffer = new ubyte[size];
}

SilkroadPacketReader::~SilkroadPacketReader(){
	// Here too
	delete [] buffer;
}

ubyte* SilkroadPacketReader::get_next(uint16& len){
	if(!pack_count)return 0;

	len = *((uint16*)buffer+pack_base);
	return buffer+pack_base;
}

// THere are potential problems with this design i know.. but meh, will fix some other time
// 4KB bytes should never show these problems
void SilkroadPacketReader::commit(uint32 len){
	// Run through the # of packets here
	ubyte* ptr=buffer+pack_end;

	uint16 num_pkts=0;
	while(true){
		if((pos-pack_end)<2)break;
		uint16 len_of_next = *((uint16*)ptr);

		if(len_of_next > (pos-pack_end))break;
		num_pkts++;
		pack_end += len_of_next;
	}
	pack_count += num_pkts;

	pos += len;
}

void SilkroadPacketReader::free(uint32 len){
	// TODO: verify that we're actually freeing a packet.. lol
	pack_base+=len;
	pack_count--;

	if(pack_base==pos)
		pack_base=pos=pack_end=0;
}