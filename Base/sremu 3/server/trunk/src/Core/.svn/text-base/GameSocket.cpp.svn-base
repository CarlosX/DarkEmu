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

void GameSocket::ProcessData(unsigned char* data, int size) 
{
	Reader.SetBuffer(data);

	int offset = 0;
	while(offset < size) 
	{
		unsigned short psize  = Reader.ReadWord() + 6;
		unsigned short opcode = Reader.ReadWord();
		Reader.Skip(2);

		switch(opcode) {

			case GAME_CLIENT_KEEP_ALIVE:
			case GAME_CLIENT_ACCEPT_HANDSHAKE:
				break;

			case GAME_CLIENT_INFO:
				SendServerInfo();
				break;

			case GAME_CLIENT_PATCH_REQUEST:
				SendPatchInfo();
				break;

			case GAME_CLIENT_AUTH:
				OnAuth();
				break;

			case GAME_CLIENT_CHARACTER:
				OnCharacter();
				break;
			
			case GAME_CLIENT_INGAME_REQUEST:
				OnIngameRequest();
				break;

			case GAME_CLIENT_MOVEMENT:
				OnMovement();
				break;

			case GAME_CLIENT_CLOSE:
				OnGameQuit();
				break;

			case GAME_CLIENT_CHAT:
				OnChat();
				break;

			case GAME_CLIENT_ITEM_MOVE:
				OnItem();
				break;

			case GAME_CLIENT_TARGET:
				OnTarget();
				break;

			case GAME_CLIENT_INGAME_NOTIFY:
				OnIngameNotify();
				break;

			case GAME_CLIENT_GM:
				OnGM();
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

void GameSocket::ReceiveThread(LPVOID s) 
{
	GameSocket Client(*((SOCKET*)s));

	Client.Init();
	Client.Receive();
}

void GameSocket::UpdateCharacter() 
{
	/* This should update everything from the Player struct. */
	db.Query("update characters set xsect=%d, ysect=%d, xpos=%d, ypos=%d, zpos=%d where id=%d",
		Player.Position.XSector,
		Player.Position.YSector,
		int(Player.Position.X),
		int(Player.Position.Y),
		int(Player.Position.Z),
		Player.General.CharacterID)
	.execute();

	// Update items
	Player.Items.UpdateItems(Player.General.CharacterID);
}

GameSocket::~GameSocket() 
{
	if(Player.Flags.Ingame)
	{
		DespawnMe();									// Tell other players that we're gone.
		UpdateCharacter();								// Save changes to DB
		Objects.Players.erase(Player.General.UniqueID);	// Delete our map.
	}
}

void GameSocket::OnGameQuit() 
{
	unsigned char type = Reader.ReadByte();	/* Exit or Restart */

	const unsigned char countdown = 5;

	Writer.Create(GAME_SERVER_COUNTDOWN);
		Writer.WriteByte (1);
		Writer.WriteByte (countdown);
		Writer.WriteByte (type);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	/* We will want to interrupt this cooldown on movement or quit cancel. */
	Sleep(countdown * 1000);

	Writer.Create(GAME_SERVER_QUIT_GAME);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Close(); /* Just making sure :) */
}