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

AgentServer::AgentServer(const char* port):_s(0){
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

THREAD_RUN_RESULT AgentServer::run(){
	while(true){
		SOCKADDR_IN sin = {0};
		int len = sizeof(sin);
		SOCKET c = accept(_s,(sockaddr*)&sin,&len);

		// We got a connection!
		if(c == INVALID_SOCKET){
			printf("Invalid socket on Agent server.. exiting...\n");
			int err = WSAGetLastError();
			char ex[2048];
			FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM,NULL,err,NULL,ex,2048,NULL);
			printf("Error (%p):\n%s\n",err,ex);
			break;
		}
		printf("[Agent] Client connecting...\n");

		// Grab the client's ip..
		len = sizeof(sin);
		getpeername(c,(sockaddr*)&sin,&len);

		// Lock our auth lock
		_lock.lock();

		// Let's see if we're expecting a connection here...
		std::map<uint,uint>::iterator i = _clients.find(sin.sin_addr.S_un.S_addr);

		printf("[Agent] Looking for client's IP in reservation list....\n");
		if(i == _clients.end()){
			printf("[Agent] NOT FOUND!!\n");
			// FAIL!
			closesocket(c);
			_lock.unlock();
			continue;
		}
		printf("[Agent] Found!  Dispatching...\n");

		_lock.unlock();

		// Oh shiz.. dispatch the client!
		AgentClient* cl = new AgentClient(c);	// Bad..
		Thread* t = new Thread;					// Bad..
		t->start(cl);
		// Memory leak here..
	}
	return RUN_NOREPEAT;
}


bool AgentServer::prepare(uint key, uint client,uint account_id){
	_lock.lock();
	KeyMap::iterator i = _keys.find(key);
	if(i != _keys.end()){
		_lock.unlock();
		return false;
	}
	connstate_s state = {client,(uint)time(NULL),account_id};
	_keys.insert(KeyPair(key,state));
	std::map<uint,uint>::iterator x = _clients.find(client);
	if(x == _clients.end()){
		_clients.insert(std::pair<uint,uint>(client,1));
	}else{
		x->second++;
	}
	_lock.unlock();
	return true;
}

uint AgentServer::auth(uint key, uint client){
	_lock.lock();
	KeyMap::iterator i = _keys.find(key);
	if(i->second.client != client) {
		_lock.unlock();
		return false;
	}
	std::map<uint,uint>::iterator x = _clients.find(client);
	x->second--;
	if(!x->second)
		_clients.erase(x);
	uint res = i->second.account_id;
	_keys.erase(i);
	_lock.unlock();
	return res;
}

void AgentServer::cleanAuths(){
	_lock.lock();
	uint curtime = (uint)time(NULL);
	for(KeyMap::iterator i = _keys.begin(); i != _keys.end(); i++){
		if(curtime - i->second.time > 20000){
			std::map<uint,uint>::iterator x = _clients.find(i->second.client);
			x->second--;
			if(!x->second)
				_clients.erase(x);
			i = _keys.erase(i);
		}
	}
	_lock.unlock();
}