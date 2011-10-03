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

#include "SocketManager.h"


SocketManager::SocketManager():running(false),/*bytes_in(0),bytes_out(0),*/thread_cnt(0),closing(false){
	// Create IOCP
	iocp=CreateIoCompletionPort(NULL,NULL,0,0);
}

SocketManager::~SocketManager() {
	if(running)
		shutdown();

	CloseHandle(iocp);
}

bool SocketManager::add_socket(Socket* s){
	list_lock.lock();
	if(!running||closing){
		list_lock.unlock();	// kik
		return false;
	}

	// TODO: error checking here!!
	CreateIoCompletionPort((HANDLE)s->get_fd(),iocp,(ULONG_PTR)s,NULL);
	// Insert it into the rb-tree
	sock_list.insert(s);

	list_lock.unlock();

	return true;
}

bool SocketManager::startup(ThreadPool* pool){
	if(running)return false;

	SYSTEM_INFO si={0};
	GetSystemInfo(&si);

	for(unsigned int i=0;i<si.dwNumberOfProcessors;i++)
		pool->add_task(this,THREAD_PRI_NORMAL);

	return(running=true);
}

bool SocketManager::shutdown(){
	if(!running)return false;

	// Prevent race condition "last-second" socket adds, this will effectively reject new connections
	// by blocking any additions to the socket list
	closing=true;

	// Lock the list
	list_lock.lock();

	// Now loop through sockets and close them
	for(std::set<Socket*>::iterator i=sock_list.begin();i!=sock_list.end();i++){
		Socket* s = *i;
		close_socket(s);
	}

	list_lock.unlock();

	// Set our running flag to false, this will allow the events to exit
	// TODO: There's a potential race condition here..
	running=false;

	// Wait for all of our threads to exit ;o
	shutdown_wait.wait(INFINITE);

	return true;
}

void SocketManager::close_socket(Socket* s){
	// Bottleneck...  not a big one, but a bottleneck nonetheless.
	SREMU_OVERLAPPED* ol = new SREMU_OVERLAPPED;
	memset(ol,0,sizeof(SREMU_OVERLAPPED));
	ol->op = OP_CLOSE;

	PostQueuedCompletionStatus(iocp,0,(ULONG_PTR)s,(LPOVERLAPPED)ol);
}

bool SocketManager::on_thread_exit(){
	if(!running){
		count_lock.lock();
		thread_cnt--;

		if(!thread_cnt)shutdown_wait.set();
		count_lock.unlock();
		return true;
	}
	return false;
}

unsigned int SocketManager::run(){
	if(on_thread_exit())
		return RUN_NOREPEAT;

	// Jump into the queue!
	unsigned long bytes=0,key=0;

	SREMU_OVERLAPPED* ovr=0;

	// 200msec, let's tweak this bitch!
	if(FALSE==GetQueuedCompletionStatus(iocp,&bytes,&key,(LPOVERLAPPED*)&ovr,200)){
		if(GetLastError()!=WAIT_TIMEOUT){
			// ewps.. report the error here??
		}
	}else{
		// TODO: Check for errors here...
		//if(!key) 
		AsyncSocket* skt = (AsyncSocket*)key;

		// Process operation!
		switch(ovr->op){
			case OP_CLOSE: {
				((Socket*)skt)->_on_disconnect();
				// Delete the socket
				list_lock.lock();
				// Now locate it in the rb-tree
				std::set<Socket*>::iterator i=sock_list.find((Socket*)skt);
				if(i!=sock_list.end())
					sock_list.erase(i);

				delete skt;
				list_lock.unlock();

				// Also delete the overlapped structure, prevent memleak and continue bottleneck!  :>
				delete ovr;
						   }
				break;
			case OP_SEND:
				skt->on_send(ovr->buf.buf,bytes);
				break;
			case OP_RECV:
				skt->on_read(ovr->buf.buf,bytes);
				break;
			default:
				// Should never happenz!
				// TODO: Print a log error or something here if this ever happens...
				break;
		}
	}

	return RUN_REPEAT;
}