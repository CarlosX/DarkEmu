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

#include <iostream>
#include "LoginSocket.h"

void LoginSocket::ReceiveThread(LPVOID s) 
{
	LoginSocket Client(*((SOCKET*)s));
	Client.Init();
	Client.Receive();
}

void LoginSocket::ProcessData(unsigned char* data, int size) 
{
	Reader.SetBuffer(data);

	int offset = 0;
	while(offset < size) {

		unsigned short psize  = Reader.ReadWord() + 6;
		unsigned short opcode = Reader.ReadWord();
		Reader.Skip(2);

		switch(opcode) 
		{
			case LOGIN_CLIENT_KEEP_ALIVE:
			case LOGIN_CLIENT_ACCEPT_HANDSHAKE:
				break;

			case LOGIN_CLIENT_LAUNCHER:
				SendLauncherInfo();
				break;

			case LOGIN_CLIENT_INFO:
				SendServerInfo();
				break;

			case LOGIN_CLIENT_PATCH_REQUEST:
				SendPatchInfo();
				break;

			case LOGIN_CLIENT_SERVERLIST_REQUEST:
				SendServerList();
				break;

			case LOGIN_CLIENT_AUTH:
				OnLogin();
				break;

			default:
				printf("Unknown opcode: %.4x\n", opcode);
		}
		Reader.Reset();
		Reader.Skip(psize);
		offset += psize;
	}
	Reader.Reset();
}

void LoginSocket::Init() 
{
	Writer.Create(LOGIN_SERVER_HANDSHAKE);
		Writer.WriteByte(1);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());
}

void LoginSocket::SendServerInfo() 
{
	unsigned char name[] = "GatewayServer";
	int namelen = strlen((char*)name);

	Writer.Create(LOGIN_SERVER_INFO);
		Writer.WriteWord(namelen);
		Writer.WriteString(name, namelen);
		Writer.WriteByte(0);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());
}

void LoginSocket::SendLauncherInfo() 
{
	Writer.Create(LOGIN_SERVER_PATCH_INFO);
		Writer.WriteDWord(0x04000101);
		Writer.WriteByte(0xA1);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(LOGIN_SERVER_LAUNCHER);

		mysqlpp::StoreQueryResult res = db.Query("select * from news order by id desc").store();

		Writer.WriteByte(0x00);
		Writer.WriteByte(res.num_rows() < 5 ? res.num_rows() : 4);

		for(unsigned int i = 0; i < res.num_rows(); ++i) 
		{
			unsigned char* head = (unsigned char*)res[i]["head"].c_str();
			unsigned int headlen = strlen((char*)head);

			unsigned char* text = (unsigned char*)res[i]["text"].c_str();
			unsigned int textlen = strlen((char*)text); 

			/* Some data is incomplete here. */
			Writer.WriteWord(strlen(res[i]["head"]));
			Writer.WriteString(head, headlen);
			Writer.WriteWord(strlen(res[i]["text"]));
			Writer.WriteString(text, textlen);
			Writer.WriteWord(0x07D8);
			Writer.WriteByte(atoi(res[i]["month"]));
			Writer.WriteByte(0x00);
			Writer.WriteByte(atoi(res[i]["day"]));
			Writer.WriteByte(0x00);
			Writer.WriteDWord(0x0025000B);
			Writer.WriteDWord(0xAFC00012);
			Writer.WriteWord(0x35D2);
		}

	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());	
}
void LoginSocket::SendPatchInfo() 
{
	Writer.Create(LOGIN_SERVER_PATCH_INFO);
		Writer.WriteDWord(0x05000101);
		Writer.WriteByte(0x20);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(LOGIN_SERVER_PATCH_INFO);
		Writer.WriteDWord(0x01000100);
		Writer.WriteDWord(0x00050628);
		Writer.WriteWord(0);
		Writer.WriteByte(2);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(LOGIN_SERVER_PATCH_INFO);
		Writer.WriteDWord(0x05000101);
		Writer.WriteByte(0x60);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(LOGIN_SERVER_PATCH_INFO);
		Writer.WriteDWord(0x02000300);
		Writer.WriteWord(0x0200);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(LOGIN_SERVER_PATCH_INFO);
		Writer.WriteDWord(0x00000101);
		Writer.WriteByte(0xA1);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(LOGIN_SERVER_PATCH_INFO);
		Writer.WriteWord(0x0100);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());
}

void LoginSocket::SendServerList() 
{
	unsigned char name[] = "SRO_Global_TestBed";
	int namelen = strlen((char*)name);

	Writer.Create(LOGIN_SERVER_LIST);

		Writer.WriteByte(0x01);
		Writer.WriteByte(0x15);
		
		Writer.WriteWord(namelen);
		Writer.WriteString(name, namelen);
		Writer.WriteByte(0);

		mysqlpp::StoreQueryResult res = db.Query("select * from servers").store();
		for(unsigned int i = 0; i < res.num_rows(); ++i) 
		{
			unsigned char* name = (unsigned char*)res[i]["name"].c_str();
			unsigned int namelen = strlen((char*)name);

			Writer.WriteByte(1);
			Writer.WriteWord(atoi(res[i]["id"]));
			Writer.WriteWord(strlen(res[i]["name"]));
			Writer.WriteString(name, namelen);
			Writer.WriteWord(atoi(res[i]["users_current"]));
			Writer.WriteWord(atoi(res[i]["users_max"]));
			Writer.WriteByte(atoi(res[i]["state"]));

		}
		Writer.WriteByte(0);

	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());
}

void LoginSocket::OnLogin() 
{
	Reader.Skip(1);

	unsigned short userlen	= Reader.ReadWord();
	unsigned char* user		= new unsigned char[userlen+1];
	memset(user, 0x00, userlen+1);
	Reader.ReadString(user, userlen);

	unsigned short passlen	= Reader.ReadWord();
	unsigned char* pass		= new unsigned char[passlen+1];
	memset(pass, 0x00, passlen+1);
	Reader.ReadString(pass, passlen);

	unsigned short serverid = Reader.ReadWord(); 
	printf("User attempting to log in...\n");

	Writer.Create(LOGIN_SERVER_AUTH_INFO);
	
		mysqlpp::StoreQueryResult res = 
			db.Query("select * from users where username='%s'", (char*)user).store();

		if(res.num_rows()!=0) 
		{
			if(strcmp((char*)pass, res[0]["password"].c_str())==0) 
			{
				db.Query("update users set failed_logins=0 where username='%s'", (char*)user).execute();

				mysqlpp::StoreQueryResult res = 
					db.Query("select * from servers where id=%d", serverid).store();

				Writer.WriteByte(0x01);
				Writer.WriteDWord(0x1234);
				Writer.WriteWord(strlen(res[0]["ip"]));
				Writer.WriteString((unsigned char*)res[0]["ip"].c_str(), strlen(res[0]["ip"]));
				Writer.WriteWord(atoi(res[0]["port"]));	
			} 
			else 
			{
				Writer.WriteByte(0x02);

				if(atoi(res[0]["failed_logins"])==5) 
				{
					Writer.WriteByte(0x06);
				}  
				else 
				{
					Writer.WriteByte(0x01);	
					Writer.WriteDWord(res[0]["failed_logins"]+1);
					Writer.WriteDWord(0x05);

					db.Query("update users set failed_logins=%d", res[0]["failed_logins"]+1).execute();
				}
			}
		} 
		else 
		{
			Writer.WriteByte(0x02);
			Writer.WriteByte(0x04);

		}

	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	delete[] user;
	delete[] pass;
}
