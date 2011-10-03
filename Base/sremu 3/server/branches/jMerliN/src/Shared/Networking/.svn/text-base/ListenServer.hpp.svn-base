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

#ifndef _SREMU_LISTEN_SERVER_H_
#define _SREMU_LISTEN_SERVER_H_

#include "../common.h"
#include <winsock2.h>
#include <mswsock.h>	// AcceptEx cancer!

#include "SocketManager.h"


// Include our threading base for the listen socket
#include "../threading/threadbase.h"

template <typename T>
class SREMU_SHARED ListenSocket : public Runnable {
public:
	ListenSocket(SocketManager* mgr):running(false),sktmgr(mgr){}
	~ListenSocket(){
		shutdown();
	}

	bool startup(const char* host,unsigned short port,SocketManager* mgr){
		if(running)return false;

		s=WSASocket(AF_INET,SOCK_STREAM,IPPROTO_TCP,NULL,0,WSA_FLAG_OVERLAPPED);	// Make our overlapped socket for i/o
		// If s is invalid here.. we need an exception.. TODO: Check error when we implement exception heirarchy

		sin.sin_family=AF_INET;
		sin.sin_port=htons(port);
		sin.sin_addr.S_un.S_addr=INADDR_ANY;

		// Get the addr
		unsigned long addr=inet_addr(host);
		if(addr!=INADDR_NONE && addr!=INADDR_ANY)
			sin.sin_addr.S_un.S_addr=addr;
		else{
			hostent* ent=gethostbyname(host);
			if(ent)
				sin.sin_addr.S_un.S_addr = *((unsigned long*)&ent->h_addr_list[0]);
		}

		// Bind the socket
		// TODO: Catch errors here...
		bind(s,(const sockaddr*)&sin,sizeof(sin));

		// Listen
		// TODO: Catch errors here...
		listen(s,10);

		running=true;

	}

	bool shutdown(){
		if(!running)return false;

		running=false;
		shutdown(s,SD_BOTH);
		closesocket(s);
		return true;
	}

protected:
	int run(){
		if(!running)return RUN_NOREPEAT;

		SOCKADDR_IN tmp;
		int size=0;
		while(running){
			// Accept a connection
			SOCKET c=accept(s,(sockaddr*)&tmp,&size);

			// Check socket..
			if(c != INVALID_SOCKET){
				T* cl=new T(c);
				sktmgr->add_socket(T);
				cl->on_connect(&tmp);
			}
		}

		return RUN_NOREPEAT;
	}

private:
	SOCKET s;
	SOCKADDR_IN sin;
	SocketManager* sktmgr;
	bool running;
};


#endif // #ifndef _SREMU_LISTEN_H_