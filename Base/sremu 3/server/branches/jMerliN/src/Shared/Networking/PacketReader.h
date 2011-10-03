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

#ifndef _SREMU_PACKET_READER_H_
#define _SREMU_PACKET_READER_H_

#include "BinaryReader.hpp"
#include "Socket.h"

// Interface for packet managing buffer
struct SREMU_SHARED PacketReader {
	// Tells how many "logical" packets exist in the buffer
	virtual uint32 num_packets()=0;

	// True if num_packets would return a non-zero result
	virtual bool has_packet()=0;

	// Returns the next packet if one exists, returns same as has_packet
	virtual bool get(BinaryReader& reader)=0;

	// Read data from the socket (encapsulating the buffer here..)
	virtual void read_data(Socket* s)=0;

	// Decommits the buffer for re-use
	virtual void free(BinaryReader& reader)=0;
};

#endif // #ifndef _SREMU_PACKET_READER_H_