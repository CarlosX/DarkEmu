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

#ifndef _SREMU_SOCKET_MANAGER_H_
#define _SREMU_SOCKET_MANAGER_H_

#include "../common.h"
#include "Socket.h"

#include "../Threading/sync.h"
#include "../Threading/threadpool.h"

/**
 * TODO:
 *
 * Figure out a way to increment the byte counters for tracking networking usage,
 * also add more complete statistics & data tracking
 **/

class SREMU_SHARED SocketManager : public Runnable {
public:
	SocketManager();
	~SocketManager();

	bool add_socket(Socket* s);

	bool startup(ThreadPool* pool);
	bool shutdown();

protected:
	unsigned int run();
	
private:
	bool on_thread_exit();
	void close_socket(Socket* s);

	HANDLE iocp;
	bool running,closing;
	std::set<Socket*> sock_list;
	FastMutex list_lock,count_lock;
	ThreadWaiter shutdown_wait;
	unsigned int thread_cnt;

	/*
	unsigned long long bytes_in;		// Need to figure out a good way to synchronize access to these
	unsigned long long bytes_out;		// a simple mutex would be too much of a bottleneck :\ 
	*/
};


#endif // #ifndef _SREMU_SOCKET_MANAGER_H_