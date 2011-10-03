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
		case 3:		// Not sure what this is...
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
	unsigned char msg[1024];
	Reader.ReadString(msg, chatlen);

	unsigned int k=0;
	for(unsigned int i=0;i<chatlen;i+=2){
		msg[k++] = msg[i];
	}
	msg[chatlen/2]=0;
		
	// TODO: Replace the !stricmp shit with a map, much more efficient.
	if(msg[0] == '.'){	// Debug admin commands
		char* p = strtok((char*)msg," ");
		printf("Got admin command: %s\n",p);
		if(!stricmp(p,".makeitem")){			// Create an item

		}else if(!stricmp(p,".dmakeitem")){		// Create an item with COMPLETE control
			unsigned long item_id = (unsigned long)atol(strtok(NULL," "));
			unsigned int item_plus = (unsigned int)atoi(strtok(NULL," "));
			unsigned char item_mod_1 = (unsigned char)atoi(strtok(NULL," "));
			unsigned char item_mod_2 = (unsigned char)atoi(strtok(NULL," "));
			unsigned char item_mod_3 = (unsigned char)atoi(strtok(NULL," "));
			unsigned char item_mod_4 = (unsigned char)atoi(strtok(NULL," "));
			unsigned char item_mod_5 = (unsigned char)atoi(strtok(NULL," "));
			unsigned char item_mod_6 = (unsigned char)atoi(strtok(NULL," "));
			unsigned char item_mod_7 = (unsigned char)atoi(strtok(NULL," "));
			unsigned char item_mod_8 = (unsigned char)atoi(strtok(NULL," "));
			unsigned long item_dura = (unsigned long)atol(strtok(NULL," "));
			unsigned char item_num_mods = (unsigned char)atoi(strtok(NULL," "));

			// Make packet!
			unsigned int slot = Player.Items.FreeSlot();
			Player.Items.Add(slot, item_id, 0, 1, item_dura, item_plus);

			Writer.Create(GAME_SERVER_ITEM_MOVEMENT);
				Writer.WriteByte (1);
				Writer.WriteByte (6);		// "Gain item".
				Writer.WriteByte (slot);
				Writer.WriteDWord(item_id);
				Writer.WriteByte (item_plus);

				Writer.WriteByte(item_mod_1);
				Writer.WriteByte(item_mod_2);
				Writer.WriteByte(item_mod_3);
				Writer.WriteByte(item_mod_4);
				Writer.WriteByte(item_mod_5);
				Writer.WriteByte(item_mod_6);
				Writer.WriteByte(item_mod_7);
				Writer.WriteByte(item_mod_8);

				Writer.WriteDWord(item_dura);
				Writer.WriteByte(item_num_mods);		// Blue stats

				for(unsigned char i=0;i<item_num_mods;i++){
					Writer.WriteDWord((unsigned long)atol(strtok(NULL," ")));
					Writer.WriteDWord((unsigned long)atol(strtok(NULL," ")));
				}
			Writer.Finalize();
			Send(Writer.Buffer, Writer.Size());

			while(strtok(NULL," "));	// Eat any remaining tokens we didn't use
		}
	}else{				// Normal chat

		Writer.Create(GAME_SERVER_CHAT);
			Writer.WriteByte (1);
			Writer.WriteDWord(Player.General.UniqueID);	
			Writer.WriteWord (chatlen / 2);
			Writer.WriteString(msg, chatlen);
		Writer.Finalize();
		Broadcast(false);

	}
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
