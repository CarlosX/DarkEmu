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

#ifndef _SREMU_SILKROAD_PACKET_WRITER_H_
#define _SREMU_SILKROAD_PACKET_WRITER_H_

#include "PacketWriter.h"
#include "Socket.h"

// Huge point of optimization!  Optimize when memory system is worked on!

class SREMU_SHARED SilkroadPacketWriter : public PacketWriter {
public:
	SilkroadPacketWriter(Socket* s=0);
	~SilkroadPacketWriter();

	// Just in case we need this sometime
	void set_endpoint(Socket* s){
		endpt=s;
	}

	// Begins a logical packet with given opcode
	void begin(uint16 opcode);

	// Commits the current logical packet
	void end();

	// Cancels the current packet, TODO: Implement this?
	//void cancel();

	// Write all packet data to socket
	void flush();
protected:
	bool on_overflow();
	ubyte* mk_buffer();

private:
	// Packet data
	bool in_packet;			// Are we writing a packet?
	uint32 pkt_base;		// The base ptr in the buffer of the current packet
	ubyte* pkt_which;		// The buffer where the current packet began
	uint16 pkt_len;			// Size of packet up to any overflows that might occur

	typedef std::list< std::pair< ubyte*, uint32 > > BufferList;
	BufferList bufflist;	// slist of buffer ptrs that we're using

	// Endpoint data
	Socket* endpt;
};

#endif // #ifndef _SREMU_SILKROAD_PACKET_WRITER_H_