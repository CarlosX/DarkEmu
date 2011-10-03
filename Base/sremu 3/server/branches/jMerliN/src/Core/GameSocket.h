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

#include "../Socket.h"
#include "../PacketReader.h"
#include "../PacketWriter.h"
#include "../Opcodes.h"
#include "../Database.h"

#include "Player.h"

class GameSocket : public Socket 
{
private:

	PacketReader Reader;
	PacketWriter Writer;
	Player Player;

	int  IsAuthorized(char* username, char* password);
	void Init();
	void SendServerInfo();
	void SendPatchInfo();
	void OnAuth();
	void OnIngameRequest();
	void OnIngameNotify();
	void OnMovement();
	void Broadcast(bool relay = false, int radius = 0);
	void OnGameQuit();
	void OnChat();
	void OnChatPublic();
	void OnChatWhisper();
	void OnChatParty();
	void OnChatGuild();
	void OnChatGlobal();
	void OnChatNotice();
	void OnChatUnion();
	void SendNotice(char* msg);
	void SendCharacterList();
	void OnCharacter();
	void OnCharCreation();
	void OnCharDeletion();
	void OnCharnameCheck();
	void OnCharRestore();
	void UpdateCharacter();
	int  CharacterExists(char* charname);
	void OnTarget();
	void CreateSpawnPacket(GameSocket* player);
	void SpawnMe();
	void DespawnMe();
	void SpawnPlayers();
	void OnItem();
	void AddItem		(unsigned int  charid, unsigned char slot, unsigned int itemtype, unsigned int amount = 1, unsigned int type = 0, unsigned int plusvalue = 0);
	void DeleteItem		(unsigned int  charid, unsigned char slot);
	void UnEquipItem	(unsigned char source, unsigned int itemtype);
	void EquipItem		(unsigned char dest,   unsigned int itemtype, unsigned int plusvalue);
	void ItemToInventory(unsigned char slot);
	void AddMastery		(unsigned int charid, unsigned int mastery, unsigned int level = 0);
	void OnGM();
	void OnMakeItem();
	void OnSpawnMonster();
	void SpawnMonster	(const _Position* pos, unsigned int id);

protected:

	void ProcessData(unsigned char* data, int size);

public:

	static void ReceiveThread(LPVOID s);

	GameSocket(SOCKET s) : Socket(s) {}
	~GameSocket();

};