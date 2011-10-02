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
#include <string>

// ============================================
// == CLIENT ==
// ============================================
THREAD_RUN_RESULT AgentClient::run(){
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

void AgentClient::init(){
	startPacket(GATEWAY_SERVER_HANDSHAKE);
	out.writeByte(1);
	sendPacket();
}

void AgentClient::processData(void* data, int size){
	SimpleBuffer buf(data,size);

	ushort psize = buf.readWord() + 6;
	ushort opcode = buf.readWord();
	buf.seek(2);

	switch(opcode){
	case AGENT_CLIENT_KEEP_ALIVE:
	case AGENT_CLIENT_ACCEPT_HANDSHAKE:
		break;

	case AGENT_CLIENT_INFO:
		onServerRequest(buf);
		break;

	case AGENT_CLIENT_PATCH_REQUEST:
		onPatchRequest(buf);

	case AGENT_CLIENT_AUTH:
		onAuthRequest(buf);
		break;

	case AGENT_CLIENT_CHARACTER:
		onCharacterRequest(buf);
		break;

	case AGENT_CLIENT_PLAY:
		onPlayRequest(buf);
		break;

	default:
		printf("Unknown opcode: %.4x\n", opcode);
		printPacket(buf.getBuffer(),buf.size(),"[UNKNOWN]");
	}
}

void AgentClient::startPacket(ushort opcode){
	out.reset();
	out.writeWord(0).writeWord(opcode).writeWord(0);
}

void AgentClient::sendPacket(){
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
void AgentClient::onServerRequest(SimpleBuffer& in){
	const char* srvname = "AgentServer";
	int len = strlen(srvname);
	startPacket(AGENT_SERVER_INFO);
	out.writeWord(len).writeByteArray(srvname,len).writeByte(0);
	sendPacket();
}

void AgentClient::onPatchRequest(SimpleBuffer& in){
	startPacket(AGENT_SERVER_PATCH_INFO);
	out.writeDWord(0x05000101).writeByte(0x20);
	sendPacket();

	startPacket(AGENT_SERVER_PATCH_INFO);
	out.writeDWord(0x01000100).writeDWord(0x00050628).writeWord(0).writeByte(2);
	sendPacket();

	startPacket(AGENT_SERVER_PATCH_INFO);
	out.writeDWord(0x05000101).writeByte(0x60);
	sendPacket();

	startPacket(AGENT_SERVER_PATCH_INFO);
	out.writeDWord(0x02000300).writeWord(0x0200);
	sendPacket();

	startPacket(AGENT_SERVER_PATCH_INFO);
	out.writeDWord(0x00000101).writeByte(0xA1);
	sendPacket();

	startPacket(AGENT_SERVER_PATCH_INFO);
	out.writeWord(0x0100);
	sendPacket();
}

void AgentClient::onAuthRequest(SimpleBuffer& in){
	printPacket(in.getBuffer(),in.size(),"AgentClient::onAuthRequest");
	uint key = in.readDWord();
	printf("Got key: %p\n",key);

	// Read in redundant user/pwd
	ushort userlen = in.readWord();
	printf("Got user len: %d\n",userlen);
	char username[21];
	if(userlen > 20 || userlen < 5){
		printf("Username length invalid!\n");
		// Kill connection..
		closesocket(_c);
		return;
	}
	in.readByteArray(username,userlen);
	username[userlen] = 0;

	ushort passlen = in.readWord();
	char password[31];
	if(passlen > 30 || passlen < 4){
		printf("Password length invalid!\n");
		closesocket(_c);
		return;
	}
	in.readByteArray(password,passlen);
	password[passlen] = 0;

	memset(username,0,sizeof(username));
	memset(password,0,sizeof(password));
	
	startPacket(AGENT_SERVER_AUTH);

	// Check if this connection is finally valid..
	SOCKADDR_IN sin = {0};
	int sinlen = sizeof(sin);
	getpeername(_c,(sockaddr*)&sin,&sinlen);

	if(account_id = g_agentServer->auth(key,sin.sin_addr.S_un.S_addr)){
		printf("Authentication via key + ip successful!  Our account_id = %d\n",account_id);

		out.writeByte(true);
		sendPacket();
	}else{
		out.writeByte(2).writeByte(1);
		sendPacket();
		closesocket(_c);
	}
}

void AgentClient::onCharacterRequest(SimpleBuffer& in){	// OnCharacter
	uint action = in.readByte();

	printf("Character request with action: %d\n",action);
	switch(action){
		case 1:
			onCharCreateRequest(in);
			break;
		case 2:
			onCharListRequest(in);
			break;
		case 3:
			onCharDeleteRequest(in);
			break;
		case 4:
			onCharNameRequest(in);
			break;
		case 5:
			onCharRestoreRequest(in);
			break;
		default:
			closesocket(_c);
			break;
	}
}

int nameUniqueCheck(const char* name, ushort len){
	MYSQL_ACQUIRE();

	char safename[27];
	mysql_real_escape_string(mysql,safename,name,len);

	int res = mysql_queryf(mysql,"SELECT COUNT(*) FROM `characters` WHERE `name` = '%s'",safename);
	if(res) return -1;

	MyResult result = mysql_store_result(mysql);
	MYSQL_ROW row = mysql_fetch_row(result);

	return atoi(row[0]) == 0;
}

void cc_addItem(uint charid, ubyte slot, uint type, uint stackamt=1, ubyte plus=0){
	MYSQL_ACQUIRE();
	mysql_queryf(mysql,"INSERT INTO `items` (`itemtype`,`owner`,`slot`,`quantity`,`plusvalue`) VALUES (%d,%d,%d,%d,%d)",type,charid,slot,stackamt,plus);
}

void cc_addMastery(uint charid, uint mastery, uint level=0){
	MYSQL_ACQUIRE();
	mysql_queryf(mysql,"INSERT INTO `masteries` (`owner`,`mastery`,`slevel`) VALUES (%d,%d,%d)",charid,mastery,level);
}

void AgentClient::onCharCreateRequest(SimpleBuffer& in){
	printPacket(in.getBuffer(),in.size(),"Char Create Debug");
	ushort namelen = in.readWord();
	startPacket(AGENT_SERVER_CHARACTER);
	out.writeByte(1);	// action

	if(namelen < 2 || namelen > 12){
		out.writeByte(2).writeByte(12);
		sendPacket();
		return;
	}
	
	char name[13];
	in.readByteArray(name,namelen);
	name[namelen] = 0;

	char* p = name;
	while(*p){
		if(*p == '[' || *p == ']' || *p == '<' || *p == '\''){
			out.writeByte(2).writeByte(13);
			sendPacket();
			return;
		}
		p++;
	}

	int res = nameUniqueCheck(name,namelen);
	if(res==-1){
		printf("Query for charname failed (CREATE)!\n");
		out.writeByte(2).writeByte(2);
		sendPacket();
		return;
	}

	if(!res){
		out.writeByte(2).writeByte(16);
		sendPacket();
		return;
	}

	// TODO: Check model
	uint model = in.readDWord();
	ubyte volume = in.readByte();

	// TODO: Check these items against valid starter items for nationality based on model check
	uint items[4] = {0};
	items[0] = in.readDWord();
	items[1] = in.readDWord();
	items[2] = in.readDWord();
	items[3] = in.readDWord();

	printf("Got items: %p %p %p %p\n",items[0],items[1],items[2],items[3]);

	// Get # of current characters owned..
	MYSQL_ACQUIRE();
	res = mysql_queryf(mysql,"SELECT COUNT(*) FROM `characters` WHERE `account`=%d AND ( `deletion_time` = 0 OR UNIX_TIMESTAMP() < `deletion_time` )",account_id);

	if(res){
		out.writeByte(2).writeByte(2);
		sendPacket();
		return;
	}

	MyResult result = mysql_store_result(mysql);
	MYSQL_ROW row = mysql_fetch_row(result);
	if(atoi(row[0])>=4){
		out.writeByte(2).writeByte(5);
		sendPacket();
		return;
	}

	// Insert it...
	// TODO: Fill in other fields, like starting position, which might be dynamic based on EU/CH
	char safename[27];
	mysql_real_escape_string(mysql,safename,name,namelen);
	res = mysql_queryf(mysql,"INSERT INTO `characters` (`account`,`name`,`chartype`,`volume`) VALUES (%d, '%s', %d, %d)",account_id,safename,model,volume);

	if(res){
		out.writeByte(2).writeByte(2);
		sendPacket();
		return;
	}

	if(mysql_affected_rows(mysql)!=1){
		out.writeByte(2).writeByte(3);
		sendPacket();
		return;
	}

	uint cid = (uint)mysql_insert_id(mysql);

	// Add the items and masteries...
	cc_addItem(cid, 1, items[0]);
	cc_addItem(cid, 4, items[1]);
	cc_addItem(cid, 5, items[2]);
	cc_addItem(cid, 6, items[3]);

	// Hacky..
	// if(chinese){
	cc_addMastery(cid, 257);
	cc_addMastery(cid, 258);
	cc_addMastery(cid, 259);
	cc_addMastery(cid, 273);
	cc_addMastery(cid, 274);
	cc_addMastery(cid, 275);
	cc_addMastery(cid, 276);
	// } else {
	// cc_addMastery(cid, 513);
	// cc_addMastery(cid, 515);
	// cc_addMastery(cid, 514);
	// cc_addMastery(cid, 516);
	// cc_addMastery(cid, 517);
	// cc_addMastery(cid, 518);
	// }

	out.writeByte(1);

	sendPacket();
}

void AgentClient::onCharListRequest(SimpleBuffer& in){
	//printf("Performing character list!\n");
	startPacket(AGENT_SERVER_CHARACTER);
	out.writeByte(2);	// Action

	MYSQL_ACQUIRE();

	//printf("Querying for characters...\n");
	int res = mysql_queryf(mysql,"SELECT `chartype`,`name`,`volume`,`level`,`experience`,`strength`,`intelligence`,`attribute`,`hp`,`mp`,`id`,`deletion_time`,UNIX_TIMESTAMP() FROM `characters` WHERE `account`=%d AND ( `deletion_time` = 0 OR UNIX_TIMESTAMP() < `deletion_time` )",account_id);
	if(res) {
		out.writeByte(2).writeByte(2);
		sendPacket();
		return;
	}
	MyResult result = mysql_store_result(mysql);

	uint nrows = (uint)mysql_num_rows(result);
	if(!nrows){
		out.writeByte(2).writeByte(1);
		sendPacket();
		return;
	}
	out.writeByte(1);

	//printf("Stored result, got %d rows.. iterating..\n",nrows);
	out.writeByte(nrows);	// # of chars
	while(MYSQL_ROW row = mysql_fetch_row(result)){
		ushort namelen = strlen(row[1]);
		// Chartype + name
		out.writeDWord(strtoul(row[0],NULL,0)).writeWord(namelen).writeByteArray(row[1],namelen);
		// Volume + level + experience
		out.writeByte(atoi(row[2])).writeByte(atoi(row[3])).writeQWord(std::stoull(row[4]));
		// Strength + intel + attr + hp + mp
		out.writeWord(atoi(row[5])).writeWord(atoi(row[6])).writeWord(atoi(row[7])).writeDWord(atoi(row[8])).writeDWord(atoi(row[9]));

		// Is this account going to be deleted?
		if(atoi(row[11])){
			// Grab times..
			uint deltime = strtoul(row[11],NULL,10);
			uint curtime = strtoul(row[12],NULL,10);
			printf("Deletion time: %p, curtime: %p\n",deltime,curtime);

			deltime = (deltime-curtime)/60;	// Always positive guaranteed by our search criteria...
			printf("That means %d minutes...\n",deltime);

			out.writeByte(true).writeDWord(deltime);
		}else
			out.writeByte(false);
		
		out.writeByte(0).writeByte(0).writeByte(0);		// ??

		//  Items..
		//printf("Looking for items...\n");
		res = mysql_queryf(mysql,"SELECT `itemtype`,`plusvalue` FROM `items` WHERE `owner`=%d AND `slot` <= 8",atoi(row[10]));
		if(res)return;
		//printf("Iterating result...\n");
		MyResult result2 = mysql_store_result(mysql);
		out.writeByte((ubyte)mysql_num_rows(result2));
		while(MYSQL_ROW row2 = mysql_fetch_row(result2))
			out.writeDWord(strtoul(row2[0],NULL,0)).writeByte(atoi(row2[1]));
		//printf("Done with items!\n");
		out.writeByte(0);
	}
	//printf("Done, sending!!\n");
	//printPacket(out.getBuffer(),out.getMax(),"CHAR LIST DEBUG");
	sendPacket();
}

void AgentClient::onCharDeleteRequest(SimpleBuffer& in){
	startPacket(AGENT_SERVER_CHARACTER);
	out.writeByte(3);

	ushort len = in.readWord();
	if(len < 2 || len > 12){
		out.writeByte(2).writeByte(13);
		sendPacket();
		return;
	}

	char username[13];
	in.readByteArray(username,len);
	username[len]=0;

	char safeuser[27];
	MYSQL_ACQUIRE();
	mysql_real_escape_string(mysql,safeuser,username,len);

	int res = mysql_queryf(mysql,"UPDATE `characters` SET `deletion_time` = UNIX_TIMESTAMP()+604800 WHERE `name` = '%s' AND `account`=%d AND `deletion_time` = 0",safeuser,account_id);
	if(res){
		printf("Error querying for char delete!\n");
		out.writeByte(2).writeByte(2);
		sendPacket();
		return;
	}
	
	uint nrows = (uint)mysql_affected_rows(mysql);
	if(nrows){
		out.writeByte(1);
	}else{
		out.writeByte(2).writeByte(1);	// This is only going to happen if someone "hax da sysdem" so we'll give a silent error.
	}
	sendPacket();
}

void AgentClient::onCharNameRequest(SimpleBuffer& in){
	startPacket(AGENT_SERVER_CHARACTER);
	out.writeByte(4);

	ushort len = in.readWord();
	if(len < 2 || len > 12){
		out.writeByte(2).writeByte(12);
		sendPacket();
		return;
	}

	char name[13];
	in.readByteArray(name,len);
	name[len] = 0;

	char* p = name;
	while(*p){
		if(*p == '[' || *p == ']' || *p == '<' || *p == '\''){
			out.writeByte(2).writeByte(13);
			sendPacket();
			return;
		}
		p++;
	}

	// Check if this user exists...
	int res = (uint)nameUniqueCheck(name,len);
	if(res==-1){
		printf("Query for charname failed!\n");
		out.writeByte(2).writeByte(2);
		sendPacket();
		return;
	}

	if(res){
		out.writeByte(1);
	}else{
		out.writeByte(2).writeByte(16);
	}

	sendPacket();
}

void AgentClient::onCharRestoreRequest(SimpleBuffer& in){
	startPacket(AGENT_SERVER_CHARACTER);
	out.writeByte(5);

	ushort len = in.readWord();
	if(len < 2 || len > 12){
		out.writeByte(2).writeByte(13);
		sendPacket();
		return;
	}

	char username[13];
	in.readByteArray(username,len);
	username[len]=0;

	char safeuser[27];
	MYSQL_ACQUIRE();
	mysql_real_escape_string(mysql,safeuser,username,len);

	int res = mysql_queryf(mysql,"UPDATE `characters` SET `deletion_time` = 0 WHERE `name` = '%s' AND `account`=%d AND UNIX_TIMESTAMP() < `deletion_time`",safeuser,account_id);
	if(res){
		printf("Error querying for char restore!\n");
		out.writeByte(2).writeByte(2);
		sendPacket();
		return;
	}
	
	uint nrows = (uint)mysql_affected_rows(mysql);
	if(nrows){
		out.writeByte(1);
	}else{
		out.writeByte(2).writeByte(1);	// This is only going to happen if someone "hax da sysdem" so we'll give a silent error.
	}
	sendPacket();
}

void AgentClient::onPlayRequest(SimpleBuffer& in){
	startPacket(AGENT_SERVER_PLAY);
	out.writeByte(1);
	//out.writeDWord(0xFFFFFFFF);
	sendPacket();
}