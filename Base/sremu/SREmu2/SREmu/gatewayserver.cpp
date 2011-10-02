 /* 
  * This file is part of SREmu.
  *
  * Copyright (c) 2010 Justin "jMerliN" Summerlin, SREmu <http://sremu.sourceforge.net>
  *
  * SREmu is free software: you can redistribute it and/or modify
  * it under the terms of the GNU Affero General Public License as published by
  * the Free Software Foundation, either version 3 of the License, or
  * (at your option) any later version.
  * 
  * SREmu is distributed in the hope that it will be useful,
  * but WITHOUT ANY WARRANTY; without even the implied warranty of
  * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  * GNU Affero General Public License for more details.
  * 
  * You should have received a copy of the GNU Affero General Public License
  * along with SREmu.  If not, see <http://www.gnu.org/licenses/>.
  */

#include "sremu.h"
#include <ws2tcpip.h>

// ============================================
// == SERVER ==
// ============================================

GatewayServer::GatewayServer(const char* port):_s(0){
	// Create our socket and bind..
	addrinfo hints = {0}, *res = 0;
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_flags = AI_PASSIVE | AI_NUMERICSERV;
	getaddrinfo(NULL,(PCSTR)port,&hints,&res);
	SOCKET s = socket(res->ai_family, res->ai_socktype, res->ai_protocol);
	bind(s, res->ai_addr, res->ai_addrlen);
	listen(s, 10);
	_s = s;
	freeaddrinfo(res);
}

THREAD_RUN_RESULT GatewayServer::run(){
	while(true){
		SOCKADDR_IN sin = {0};
		int len = sizeof(sin);
		SOCKET c = accept(_s,(sockaddr*)&sin,&len);

		// We got a connection!
		if(c == INVALID_SOCKET){
			printf("Invalid socket on Gateway server.. exiting...\n");
			int err = WSAGetLastError();
			char ex[2048];
			FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM,NULL,err,NULL,ex,2048,NULL);
			printf("Error (%p):\n%s\n",err,ex);
			break;
		}
		printf("[Gateway] Client connecting...\n");
		
		// Create the client connection..
		GatewayClient* cl = new GatewayClient(c);	// Bad..
		Thread* t = new Thread;						// Bad..
		t->start(cl);
		// Memory leak here..
	}
	return RUN_NOREPEAT;
}