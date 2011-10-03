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

#ifndef _SREMU_SILKROAD_PACKET_READER_H_
#define _SREMU_SILKROAD_PACKET_READER_H_

#include "PacketReader.h"

/**
 * We have a potential threading problem here.. 
 * If we commit a read to the buffer here and before that completes another read is initiated,
 * we have a slight problem :o.
 *
 * Alternatively, this shouldn't be a problem as more than 1 thread should never read
 * from the same socket at the same time.
 **/

// TODO: Modify this to require very little knowledge of what goes on?
// Do this after we have our smart pointers, as this won't require a higher level
// of code to complete an async operation that we start here

// Interface for packet managing buffer
class SREMU_SHARED SilkroadPacketReader : public PacketReader {
public:
	SilkroadPacketReader(uint32 size);
	~SilkroadPacketReader();

	// Tells how many "logical" packets exist in the buffer
	uint32 num_packets() {
		return pack_count;
	}

	// True if num_packets would return a non-zero result
	bool has_packet() {
		return pack_count > 0;
	}

	// Returns the next packet if one exists, returns same as has_packet
	ubyte* get_next(uint16& len);

	// Get read buffer from buffer
	ubyte* get_buffer(uint32& size){
		size=(bufsize-pos);
		return buffer+pos;
	}

	// Commit read buffer
	void commit(uint32 len);

	// Decommits the buffer for re-use
	void free(uint32 len);

private:
	// Buffer data
	ubyte* buffer;		// Our bytebuffer
	uint32 bufsize;		// Size of our buffer

	// Packet data
	uint32 pack_count;	// # of packets we've identified
	uint32 pack_base;	// pos of next packet in buffer
	uint32 pack_end;	// End of packet data we've identified (may be BEFORE the pos if we get a packet fragment)

	// Ptr data
	uint32 pos;			// Current position, offset 0 to pos-1 are all packet data (recognized or not), after pos is free buffer store

};

#endif // #ifndef _SREMU_SILKROAD_PACKET_READER_H_