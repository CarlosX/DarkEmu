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

void GameSocket::SpawnMe() 
{
	CreateSpawnPacket(this);
	Broadcast(false);
}

void GameSocket::DespawnMe() 
{
	Writer.Create(GAME_SERVER_DESPAWN_PLAYER);
		Writer.WriteDWord(Player.General.UniqueID);
	Writer.Finalize();
	Broadcast(false);
}

void GameSocket::SpawnPlayers() 
{
	for(Objects.pIter = Objects.Players.begin(); Objects.pIter != Objects.Players.end(); ++Objects.pIter) 
	{
		if(Objects.pIter->first != Player.General.UniqueID) 
		{
			CreateSpawnPacket(Objects.pIter->second);
			Send(Writer.Buffer, Writer.Size());
		}
	}
}

void GameSocket::CreateSpawnPacket(GameSocket* player) 
{
	Writer.Create(GAME_SERVER_SPAWN);

		Writer.WriteDWord(player->Player.Stats.Model);
		Writer.WriteByte (player->Player.Stats.Volume);
		Writer.WriteByte (player->Player.Stats.Level < 20 ? 1 : 0);
		Writer.WriteByte (0x2D);	// Max item slot

		// Item data here.
		mysqlpp::StoreQueryResult items = 
			db.Query("select * from items where owner=%d and type=0", player->Player.General.CharacterID).store();

		Writer.WriteByte (items.num_rows());
		for(unsigned int i = 0; i < items.num_rows(); i++) 
		{
			Writer.WriteDWord(atoi(items[i]["itemtype"]));
			Writer.WriteByte (atoi(items[i]["plusvalue"]));
		}

		// Avatar data here.
		Writer.WriteByte (4);
		Writer.WriteByte (0);

		Writer.WriteByte (0);
		Writer.WriteDWord(player->Player.General.UniqueID);

		Writer.WriteByte (player->Player.Position.XSector);
		Writer.WriteByte (player->Player.Position.YSector);
		Writer.WriteFloat(player->Player.Position.X);
		Writer.WriteFloat(player->Player.Position.Z);
		Writer.WriteFloat(player->Player.Position.X);
	
		// Angle & movement flags.
		Writer.WriteWord (0);	// Angle
		Writer.WriteByte (0);
		Writer.WriteByte (1);
		Writer.WriteByte (0);
		Writer.WriteWord (0);	// Angle

		Writer.WriteWord (1);
		
		Writer.WriteByte (player->Player.Flags.Berserk);
		Writer.WriteFloat(player->Player.Speeds.WalkSpeed);
		Writer.WriteFloat(player->Player.Speeds.RunSpeed);
		Writer.WriteFloat(player->Player.Speeds.BerserkSpeed);

		Writer.WriteByte (0);	// Number of active buffs

		Writer.WriteWord(strlen((char*)player->Player.General.CharacterName));
		Writer.WriteString(player->Player.General.CharacterName, strlen((char*)player->Player.General.CharacterName));

		// Unknown data.
		Writer.WriteByte (0);
		Writer.WriteByte (1);
		Writer.WriteDWord(0);
		Writer.WriteDWord(0);
		Writer.WriteDWord(0);
		Writer.WriteDWord(0);
		Writer.WriteDWord(0);
		Writer.WriteDWord(0);
		Writer.WriteDWord(0);
		Writer.WriteByte (1);
		Writer.WriteByte (player->Player.Flags.PvP);	// 0 = blue costume pvp flag, 1 = blue costume no flag

	Writer.Finalize();
}