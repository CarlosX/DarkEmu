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

#include "GameSocket.h"
#include "ObjectMgr.h"

void GameSocket::OnChat() 
{
	unsigned char type = Reader.ReadByte();
	unsigned char chatoffset = Reader.ReadByte();

	Writer.Create(GAME_SERVER_CHAT_ACCEPT);
		Writer.WriteByte (1);
		Writer.WriteByte (type);
		Writer.WriteByte (chatoffset);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	switch(type) {
		case 1:
			OnChatPublic();
			break;
		case 2:
			OnChatWhisper();
			break;
		case 4:
			OnChatParty();
			break;
		case 5:
			OnChatGuild();
			break;
		case 6:
			OnChatGlobal();
			break;
		case 7:
			OnChatNotice();
			break;
		case 11:
			OnChatUnion();
			break;
	}
}

void GameSocket::OnChatPublic() 
{
	unsigned short chatlen = Reader.ReadWord() * 2;
	unsigned char* msg = new unsigned char[chatlen];
	Reader.ReadString(msg, chatlen);

	Writer.Create(GAME_SERVER_CHAT);
		Writer.WriteByte (1);
		Writer.WriteDWord(Player.General.UniqueID);	
		Writer.WriteWord (chatlen / 2);
		Writer.WriteString(msg, chatlen);
	Writer.Finalize();
	Broadcast(false);

	delete[] msg;
}

void GameSocket::OnChatWhisper() 
{
	unsigned short charlen = Reader.ReadWord();
	unsigned char* charname = new unsigned char[charlen+1];
	memset(charname, 0x00, charlen+1);
	Reader.ReadString(charname, charlen);

	unsigned short chatlen = Reader.ReadWord() * 2;
	unsigned char* msg = new unsigned char[chatlen];
	Reader.ReadString(msg, chatlen);

	int charid = CharacterExists((char*)charname);

	Writer.Create(GAME_SERVER_CHAT);
		Writer.WriteByte (2);
		Writer.WriteWord (
			strlen((char*)Player.General.CharacterName));
		Writer.WriteString(Player.General.CharacterName, 
			strlen((char*)Player.General.CharacterName));
		Writer.WriteWord (chatlen / 2);
		Writer.WriteString(msg, chatlen);
	Writer.Finalize();

	for(Objects.pIter = Objects.Players.begin(); Objects.pIter != Objects.Players.end(); ++Objects.pIter) {
		if(Objects.pIter->second->Player.General.CharacterID == charid) {
			Objects.pIter->second->Send(Writer.Buffer, Writer.Size());
		}
	}

	delete[] msg;
	delete[] charname;
}

void GameSocket::SendNotice(char* msg)
{
	Writer.Create(GAME_SERVER_CHAT);
		Writer.WriteByte	(7);
		Writer.WriteByte	(strlen(msg));
		Writer.WriteUString ((unsigned char*)msg, strlen(msg));
	Writer.Finalize();
	Broadcast(true);
}


void GameSocket::OnChatGlobal()  {}
void GameSocket::OnChatGuild()	 {}
void GameSocket::OnChatNotice()  {}
void GameSocket::OnChatUnion()   {}
void GameSocket::OnChatParty()	 {}
