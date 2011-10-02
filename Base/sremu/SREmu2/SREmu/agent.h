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

#ifndef _SREMU_AGENT_H_
#define _SREMU_AGENT_H_

#include <threading.h>
#include <winsock2.h>
#include <memory.h>
#include <database.h>

class AgentClient : public Runnable {
public:
	AgentClient(SOCKET c):_c(c),out(_outdata,4096),account_id(0){init();}

protected:
	THREAD_RUN_RESULT run();

	void init();
	void processData(void* data, int size);

	// Event handlers
	void onServerRequest(SimpleBuffer& in);		// SendServerInfo
	void onPatchRequest(SimpleBuffer& in);		// SendPatchInfo
	void onAuthRequest(SimpleBuffer& in);		// OnAuth
	void onCharacterRequest(SimpleBuffer& in);	// OnCharacter

	// Character req handlers..
	void onCharCreateRequest(SimpleBuffer& in);
	void onCharListRequest(SimpleBuffer& in);
	void onCharDeleteRequest(SimpleBuffer& in);
	void onCharNameRequest(SimpleBuffer& in);
	void onCharRestoreRequest(SimpleBuffer& in);
	void onPlayRequest(SimpleBuffer& in);

private:
	void startPacket(ushort opcode);
	void sendPacket();

private:
	SOCKET _c;
	ubyte _outdata[4096];
	SimpleBuffer out;
	uint account_id;
};

class AgentServer : public Runnable {
public:
	AgentServer(const char* port);
	bool ready(){ return _s != 0; }

	// Authing
	bool prepare(uint key, uint client, uint account_id);
	uint auth(uint key, uint client);
	void cleanAuths();
protected:
	THREAD_RUN_RESULT run();
	
private:
	SOCKET _s;

	struct connstate_s {
		uint client;
		uint time;
		uint account_id;
	};
	FastMutex _lock;
	typedef std::map<uint,connstate_s> KeyMap;
	typedef std::pair<uint,connstate_s> KeyPair;
	KeyMap _keys;
	std::map<uint,uint> _clients;
};

extern AgentServer* g_agentServer;

#endif // #ifndef _SREMU_AGENT_H_