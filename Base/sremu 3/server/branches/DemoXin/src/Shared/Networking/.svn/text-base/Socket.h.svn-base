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
#include "SimpleBuffer.h"
#include "BinaryReader.h"

#include <winsock2.h>

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

class SREMU_SHARED Socket {
public:
	Socket(SOCKET fd=0);	// Ctor, if fd is 0, we will allocate one
	~Socket();				// Dtor

	// Connect to server given host name or ip-address & port
	bool connect(const char* host,unsigned short port);

	// Disconnect from endpoint
	void disconnect();

	// Virtual methods (default implementation makes no sense.)
	virtual void on_read(const BinaryReader& buffer){};
	virtual void on_connect(){};
	virtual void on_disconnect(){};

	// send function, requires a const buffer (filled with data!)
	bool send(const SimpleBuffer& buffer);

	// conn_info returns the remote IP address & connected port via a SOCKADDR_IN structure
	void conn_info(SOCKADDR_IN& addr);

	// get_fd returns the internal socket
	SOCKET get_fd(){return s;}

	// obvious
	bool is_connected();

private:
	SOCKET s;

	// We shouldn't need mutexes to protect read & write operations on our sockets
	// because they don't explicitly manage a resource.  The ws2_32 api is threadsafe, so
	// we should be good.
};

#endif // #ifndef _SREMU_SOCKET_H_