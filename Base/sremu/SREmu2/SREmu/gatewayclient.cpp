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
#include <opcodes.h>

// ============================================
// == CLIENT ==
// ============================================
THREAD_RUN_RESULT GatewayClient::run(){
	ubyte buffer[4096];
	while(true){
		int res = recv(_c, (char*)buffer, 2, 0);
		
		if(res < 1){
			/*
			printf("Result from recv = %p\n",res);
			int err = WSAGetLastError();
			char ex[2048];
			FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM,NULL,err,NULL,ex,2048,NULL);
			printf("Error (%p):\n%s",err,ex);
			*/
			printf("Client exited.. terminating client object.\n");
			break;
		}

		// We have 2 bytes... now let's read the rest of the packet..
		ushort size = *(ushort*)buffer + 6;
		if(size > 4096){
			printf("Client sent invalid buffer size.. terminating client object.\n");
			break;
		}

		size -= 2;

		ushort read = 0;
		while(size - read) {
			res = recv(_c, (char*)&buffer[2+read], size-read, 0);
			if(res < 1)break;
			read += (ushort)res;
		}

		// Process the packet now...
		processData(buffer,size+2);
	}

	return RUN_NOREPEAT;
}

void GatewayClient::init(){
	startPacket(GATEWAY_SERVER_HANDSHAKE);
	out.writeByte(1);
	sendPacket();
}

void GatewayClient::processData(void* data, int size){
	SimpleBuffer buf(data,size);

	ushort psize = buf.readWord() + 6;
	ushort opcode = buf.readWord();
	buf.seek(2);

	switch(opcode){
	case GATEWAY_CLIENT_KEEP_ALIVE:
	case GATEWAY_CLIENT_ACCEPT_HANDSHAKE:
		break;

	case GATEWAY_CLIENT_LAUNCHER:
		onLauncherRequest(buf);
	break;

	case GATEWAY_CLIENT_INFO:
		onServerRequest(buf);
	break;

	case GATEWAY_CLIENT_PATCH_REQUEST:
		onPatchRequest(buf);
	break;

	case GATEWAY_CLIENT_SERVERLIST_REQUEST:
		onServerListRequest(buf);
	break;

	case GATEWAY_CLIENT_AUTH:
		onLoginRequest(buf);
	break;

	default:
		printf("Unknown opcode: %.4x\n", opcode);
		printPacket(buf.getBuffer(),buf.size(),"[UNKNOWN]");
	}
}

void GatewayClient::startPacket(ushort opcode){
	out.reset();
	out.writeWord(0).writeWord(opcode).writeWord(0);
}

void GatewayClient::sendPacket(){
	out.rewind();
	ushort size = out.getMax();
	out.writeWord( size - 6 );

	char* p = (char*)_outdata;

	// Dump log...
	//printPacket(out.getBuffer(),out.getMax(),"[SENDPACKET]");
	while(size > 0){
		int res = send(_c,p,size,0);
		if(res == SOCKET_ERROR){
			printf("Client write failed!\n");
			return;
		}
		size -= (ushort)res;
		p += (ushort)res;
	}
}


// ============================================
// == Event Handlers ==
// ============================================
void GatewayClient::onServerRequest(SimpleBuffer& in){
	ushort clinamelen = in.readWord();
	char cliname[64];
	in.readByteArray(cliname,clinamelen);
	cliname[clinamelen]=0;

	printf("Server request made by agent: %s\n",cliname);

	const char* name = "GatewayServer";
	int len = strlen(name);

	startPacket(GATEWAY_SERVER_INFO);
	out.writeWord(len).writeByteArray(name,len).writeByte(0);
	sendPacket();
}

void GatewayClient::onLauncherRequest(SimpleBuffer& in){
	//ubyte locale = in.readByte();

	startPacket(GATEWAY_SERVER_PATCH_INFO);
	out.writeDWord(0x04000101).writeByte(0xA1);
	sendPacket();

	startPacket(GATEWAY_SERVER_LAUNCHER);

	// Pull news...
	printf("Writing news...\n");
	MYSQL_ACQUIRE();

	int res = mysql_query(mysql,"SELECT id,head,text,month,day FROM news ORDER BY id DESC");
	if(res)return;

	MyResult result = mysql_store_result(mysql);
	uint nrows = (uint)mysql_num_rows(result);
	printf("Number of news items: %d\n",nrows);
	out.writeByte(0).writeByte(nrows < 5 ? nrows : 5);

	while(MYSQL_ROW row = mysql_fetch_row(result)){
		int headlen = strlen(row[1]), textlen = strlen(row[2]);
		printf("Header: %s\n",row[1]);
		out.writeWord(headlen).writeByteArray(row[1],headlen).writeWord(textlen).writeByteArray(row[2],textlen);
		out.writeWord(0x07D8).writeWord(atoi(row[3])).writeWord(atoi(row[4])).writeDWord(0x0025000B).writeDWord(0xAFC00012).writeWord(0x35D2);
		sendPacket();
	}
}

void GatewayClient::onPatchRequest(SimpleBuffer& in){
	startPacket(GATEWAY_SERVER_PATCH_INFO);
	out.writeDWord(0x05000101).writeByte(0x20);
	sendPacket();

	startPacket(GATEWAY_SERVER_PATCH_INFO);
	out.writeDWord(0x01000100).writeDWord(0x00050628).writeWord(0).writeByte(2);
	sendPacket();

	startPacket(GATEWAY_SERVER_PATCH_INFO);
	out.writeDWord(0x05000101).writeByte(0x60);
	sendPacket();

	startPacket(GATEWAY_SERVER_PATCH_INFO);
	out.writeDWord(0x02000300).writeWord(0x0200);
	sendPacket();

	startPacket(GATEWAY_SERVER_PATCH_INFO);
	out.writeDWord(0x00000101).writeByte(0xA1);
	sendPacket();

	startPacket(GATEWAY_SERVER_PATCH_INFO);
	out.writeWord(0x0100);
	sendPacket();
}

void GatewayClient::onServerListRequest(SimpleBuffer& in){
	char* name = "SREmu2_Alpha_TestBed";
	int len = strlen(name);

	startPacket(GATEWAY_SERVER_LIST);
	out.writeByte(0x01).writeByte(0x15).writeWord(len).writeByteArray(name,len).writeByte(0);

	// Write the rows now...
	MYSQL_ACQUIRE();

	// Query..
	int res = mysql_query(mysql,"SELECT id,name,users_current,users_max,state FROM servers");
	if(res){
		printf("Unable to complete server list query...\n");
		return;
	}
	MyResult result = mysql_use_result(mysql);
	while(MYSQL_ROW row = mysql_fetch_row(result)){
		out.writeByte(1).writeWord(atoi(row[0]));
		ushort srvlen = strlen(row[1]);
		out.writeWord(srvlen).writeByteArray(row[1],srvlen);
		out.writeWord(atoi(row[2])).writeWord(atoi(row[3])).writeByte(atoi(row[4]));
	}

	// Termination...
	out.writeByte(0);

	//printPacket(out.getBuffer(),out.getMax(),"SERVER LIST");

	sendPacket();
}

void GatewayClient::onLoginRequest(SimpleBuffer& in){
	in.seek(1);
	startPacket(GATEWAY_SERVER_AUTH_INFO);

	char username[21] = {0};
	char password[31] = {0};

	// Read username
	ushort userlen = in.readWord();
	if(userlen > 20 || userlen < 5){
		printf("Username length invalid!\n");
		out.writeByte(2).writeByte(ERR_C7);
		sendPacket();
		return;
	}
	in.readByteArray(username,userlen);
	username[userlen] = 0;

	// Read password
	ushort passlen = in.readWord();
	if(passlen > 30 || passlen < 4){
		printf("Password length invalid!\n");
		out.writeByte(2).writeByte(ERR_C7);
		sendPacket();
		return;
	}
	in.readByteArray(password,passlen);
	password[passlen] = 0;

	// Get server
	ubyte unknown = in.readByte();
	ushort serverid = in.readWord();

	printf("User wants to auth: (%s,%d)\n",username,serverid);

	MYSQL_ACQUIRE();

	// Check if account is good...
	printf("Checking account...\n");
	char safeuser[64],safepass[64];
	userlen = (ushort)mysql_real_escape_string(mysql,safeuser,username,userlen);
	passlen = (ushort)mysql_real_escape_string(mysql,safepass,password,passlen);
	int res = mysql_queryf(mysql,"SELECT `id`,`banned`,UNIX_TIMESTAMP() FROM `users` WHERE `username`='%s' AND `password`='%s'",safeuser,safepass);

	if(res){
		printf("Username query failed...\n");
		out.writeByte(2).writeByte(ERR_FAILED_C5);
		sendPacket();
		return;
	}

	MyResult result = mysql_store_result(mysql);
	if(!mysql_num_rows(result)){
		out.writeByte(2).writeByte(ERR_FAILED).writeDWord(++fail_count).writeDWord(5);
		sendPacket();
		printf("Unable to find this user...\n");
		if(fail_count == 5){
			// Disconnect..
			printf("User failed 5/5 times, killing connection...\n");
			closesocket(_c);
			return;
		}
		return;
	}

	MYSQL_ROW row = mysql_fetch_row(result);
	uint account_id = strtoul(row[0],NULL,0);

	int bant = atoi(row[1]);
	int now = atoi(row[2]);

	if((bant == -1) || bant && (bant>now)){
		// User is banned, -1 == perm, or temporarily.  Send error..
		out.writeByte(2).writeByte(ERR_C8);
		sendPacket();
		return;
	}
	
	// User logged in successfully.. let's grab server info...
	memset(username,0,sizeof(username));
	memset(safeuser,0,sizeof(safeuser));
	memset(safepass,0,sizeof(safepass));
	memset(password,0,sizeof(password));

	res = mysql_queryf(mysql,"SELECT host,port FROM servers WHERE id=%d AND state=1",serverid);
	if(res){
		printf("Failed to query for servers...\n");
		out.writeByte(2).writeByte(ERR_FAILED_C5);
		sendPacket();
		return;
	}

	result = mysql_store_result(mysql);
	if(!mysql_num_rows(result)){
		printf("Bad server!\n");
		out.writeByte(2).writeByte(ERR_FAILED_C5);
		sendPacket();
		return;
	}

	row = mysql_fetch_row(result);

	// Generate a key for this user...
	SOCKADDR_IN sin = {0};
	int len = sizeof(sin);
	getpeername(_c,(sockaddr*)&sin,&len);
	uint client = sin.sin_addr.S_un.S_addr;
	uint session_key = 0;
	do{
		session_key = (rand()&0xFF)|((rand()&0xFF)<<8)|((rand()&0xFF)<<16)|((rand()&0xFF)<<24);
	}while(!g_agentServer->prepare(session_key,client,account_id));

	printf("Client session key created: %p\n",session_key);

	out.writeByte(1).writeDWord(session_key);
	int hostlen = strlen(row[0]);
	out.writeWord(hostlen).writeByteArray(row[0],hostlen).writeWord(atoi(row[1]));

	sendPacket();
}