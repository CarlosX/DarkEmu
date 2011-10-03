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

#include "SilkroadPacketWriter.h"

SilkroadPacketWriter::SilkroadPacketWriter(Socket* s):in_packet(false),pkt_base(0),endpt(0),pkt_len(0),pkt_which(0){
	set_endpoint(s);
	block_write();
}

SilkroadPacketWriter::~SilkroadPacketWriter(){
	// Dealloc all remaining buffers..
	if(bufflist.size()){
		for(BufferList::iterator i=bufflist.begin();i!=bufflist.end();i++)
			delete [] (ubyte*)(i->first);		// Dealloc here
	}
	bufflist.clear();
}

void SilkroadPacketWriter::begin(uint16 opcode){
	if(in_packet||!endpt)return;	// Ewpz!  Should we 'end' here??

	if(!get_buffer() || ((get_size()-tell()) < 6) ){
		// Get a buffar!
		ubyte* nbuf=mk_buffer();
		reload(nbuf,256);
		pkt_base = 0;
		pkt_which = nbuf;
		pkt_len = 0;
	} else {
		pkt_base = tell();
		pkt_which = get_buffer();
		pkt_len = 0;
	}

	write_basic<uint16>(0);				// Placeholder for 'size'
	write_basic<uint16>(opcode);		// Opcode
	write_basic<uint16>(0);				// "Security" bytes (TODO: Implement if we're using security!)

	in_packet = true;
	allow_write();
}

void SilkroadPacketWriter::end(){
	if(!in_packet||!endpt)return;

	uint32 current=tell();

	// Write the current buffer's total length
	bufflist.begin()->second = current;

	// If we're in the same buffer (no overflowing occured!)
	if(get_buffer()==pkt_which){
		uint16 len = (uint16)(current-pkt_base);
		seek(pkt_base,0);
		write_basic<uint16>(len);
	}else {
		uint16 len = pkt_len + (uint16)current;

		BinaryWriter bw(pkt_which,256);	// Alloc size here
		bw.seek(pkt_base,0);
		bw.write_basic<uint16>(len);
	}


	// Either case, let's clear things up
	block_write();
	in_packet = false;
}

void SilkroadPacketWriter::flush(){
	// Flush teh packetz!
	if(in_packet||!endpt)return;	// Need error here.. we can't flush if we aren't done writing -.-

	reload(0,0);
	for(BufferList::reverse_iterator ri=bufflist.rbegin();ri!=bufflist.rend();ri++){
		SREMU_OVERLAPPED* ovr=new SREMU_OVERLAPPED;		// Allocator here.. point of optimization!
		memset(ovr,0,sizeof(SREMU_OVERLAPPED));
		ovr->buf.buf = (CHAR*)ri->first;
		ovr->buf.len = ri->second;
		ovr->op = OP_SEND;
		if(!endpt->send(ovr)){
			delete ovr;
			delete [] ri->first;
		}
	}

	bufflist.clear();
}

bool SilkroadPacketWriter::on_overflow() {
	// Update this buffer's size..
	bufflist.begin()->second = tell();

	// Grab a new buffer
	ubyte* buf2=mk_buffer();

	// Set it up
	reload(buf2,256);		// Allocation length here
	return true;
}

ubyte* SilkroadPacketWriter::mk_buffer(){
	ubyte* nbuf=new ubyte[256];		// Alloc here
	bufflist.push_front(std::make_pair(nbuf,0));
	return nbuf;
}