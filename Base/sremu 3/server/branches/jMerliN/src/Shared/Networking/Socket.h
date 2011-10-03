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

#ifndef _SREMU_SOCKET_H_
#define _SREMU_SOCKET_H_

#include "../common.h"

#include <winsock2.h>

// Our overlapped structure
enum NET_OPERATION {
	OP_NONE, OP_CLOSE, OP_SEND, OP_RECV
};

struct SREMU_OVERLAPPED {
	OVERLAPPED ovr;
	WSABUF buf;
	NET_OPERATION op;
};

// TODO:  Move this to a 'network.h' header somewhere that includes all of the networking utilities ;o
bool SREMU_SHARED sremu_net_start();
bool SREMU_SHARED sremu_net_cleanup();

/*
 * Goal:  Create a simple implementation of what we think of as a "socket"
 * in networking terms.  Also, we need virtual read, connect, and disconnect
 * event handlers.  The socket should *NOT* be involved with any memory allocation
 * or be responsible for keeping track of its own buffer, this creates memory
 * mismanagement where unnecessary resources are assigned.  Rather, the virtual functions
 * should receive a pointer to a buffer loaded with the data transferred from the read,
 * and when sending data, the caller should be responsible for grabbing a buffer from a
 * resource pool.  This way, the socket is lightweight and represents only the endpoint
 * where communication occurs.
 * - jMerliN
 */

class SREMU_SHARED AsyncSocket {
public:
	virtual void on_read(char* buffer,unsigned long len)=0;		// Called on each new packet received
	virtual void on_send(char* buffer,unsigned long len)=0;		// Called on each completed packet send
	virtual void on_connect(SOCKADDR_IN* peer)=0;				// Called when a connection is made (after this class is instantiated)
	virtual void on_disconnect()=0;								// Called when socket looses connection

};


// The socket class should *NOT* function as a connection object, it represents a connected
// socket where data may be transmitted, or rather a socket is the "result" of a connection.
// This is the reasoning behind why i do not provide a 'connect' or 'disconnect' function here
class SREMU_SHARED Socket : public AsyncSocket {
public:
	Socket(SOCKET fd):s(fd),connected(true){}

	// send function
	bool send(SREMU_OVERLAPPED* ovr);	// Needs data buffer in 'buf' with 'len' bytes written
	bool read(SREMU_OVERLAPPED* ovr);	// Needs data buffer in 'buf' with 'len' bytes avail

	// get_fd returns the internal socket
	SOCKET get_fd(){return s;}

	// obvious
	bool is_connected(){
		return connected;
	}

private:
	friend class SocketManager;
	void _on_disconnect(){
		connected=false;
		shutdown(s,SD_BOTH);
		closesocket(s);
		s=NULL;

		on_disconnect();
	}
	bool connected;
	SOCKET s;
};

#endif // #ifndef _SREMU_SOCKET_H_