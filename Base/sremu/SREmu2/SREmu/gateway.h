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

#ifndef _SREMU_GATEWAY_H_
#define _SREMU_GATEWAY_H_

#include <threading.h>
#include <winsock2.h>
#include <memory.h>
#include <database.h>

class GatewayClient : public Runnable {
public:
	GatewayClient(SOCKET c):_c(c),out(_outdata,4096),fail_count(0){init();}

protected:
	THREAD_RUN_RESULT run();

	enum AUTH_ERRORS {
		ERR_NONE			= 0,
		ERR_FAILED			= 1,	// failed login, needs args: {numtries:dword, maxtries:dword}
		ERR_ALREADY_CONN	= 3,	// already connected
		ERR_FAILED_C5		= 4,	// failed to connect (C5)
		ERR_FULL			= 5,	// capacity of the server full, needs args: {seconds_before_retry:dword}
		ERR_C7				= 6,	// (C7)
		ERR_C8				= 7,	// (C8)
		ERR_IP_LIMIT		= 8,	// IP limit exceeded
		ERR_18_PLUS			= 10,	// 18+
		ERR_12_PLUS			= 11,	// 12+
		ERR_PEDO			= 12	// 18+ not allowed on 12+
	};

	void init();
	void processData(void* data, int size);

	// Event handlers
	void onServerRequest(SimpleBuffer& in);
	void onLauncherRequest(SimpleBuffer& in);
	void onPatchRequest(SimpleBuffer& in);
	void onServerListRequest(SimpleBuffer& in);
	void onLoginRequest(SimpleBuffer& in);	

private:
	void startPacket(ushort opcode);
	void sendPacket();

private:
	SOCKET _c;
	ubyte _outdata[4096];
	SimpleBuffer out;
	int fail_count;
};

class GatewayServer : public Runnable {
public:
	GatewayServer(const char* port);
	bool ready(){ return _s != 0; }

protected:
	THREAD_RUN_RESULT run();
	
private:
	SOCKET _s;
};

extern GatewayServer* g_gatewayServer;

#endif // #ifndef _SREMU_GATEWAY_H_