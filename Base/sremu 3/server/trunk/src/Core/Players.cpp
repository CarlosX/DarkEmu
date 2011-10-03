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

void GameSocket::Broadcast(bool relay, int radius) 
{
	/* This will be implemented much better later. */
	for(Objects.pIter = Objects.Players.begin(); Objects.pIter != Objects.Players.end(); ++Objects.pIter) 
	{
		if(Objects.pIter->first == Player.General.UniqueID && !relay) continue;
		if(radius == 0) 
		{
			Objects.pIter->second->Send(Writer.Buffer, Writer.Size());
		}
	}
}

void GameSocket::OnTarget() 
{
	/* Check object type here. Only players supported now. */
	unsigned int objectid = Reader.ReadDWord();

	Writer.Create(GAME_SERVER_TARGET);

		Objects.pIter = Objects.Players.find(objectid);

		if(Objects.pIter != Objects.Players.end()) 
		{
			// Player.Stats.target = objectid;
			Writer.WriteByte (1);
			Writer.WriteDWord(objectid);
			Writer.WriteDWord(0x10);	// Player object type
			Writer.WriteByte (4);		// Job icon, set appropriately.
		} 
		else 
		{
			/* This shouldn't be happening until we implement monsters/npc */
			//Writer.WriteByte (2);
			Writer.WriteByte (1);
            Writer.WriteDWord(objectid);
			Writer.WriteByte (1);
			Writer.WriteDWord(4);//vida
			Writer.WriteDWord(1);
		}

	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());
}

void GameSocket::OnIngameNotify() 
{
	/* 1 is sent when going ingame. 
	   5 is sent shortly after.
	   7 is sent after moving the first time. */
	unsigned int id = Reader.ReadDWord();
	switch(id) 
	{
		case 1:
			Objects.Players[Player.General.UniqueID] = this;
			Player.Flags.Ingame = true;
			SpawnMe();
			SpawnPlayers();
			SpawnAllM();//Spawn Auto
			break;
		case 5:
			break;
		case 7:
			break;
	}
}
void GameSocket::OnEmotion()
{
    unsigned char type = Reader.ReadByte();
	Writer.Create(GAME_CLIENT_EMOTION);
    Writer.WriteDWord(Player.General.UniqueID);
	Writer.WriteByte (type);
	Writer.Finalize();
	Broadcast(true);
}
void GameSocket::OnSit()
{
	unsigned char type = Reader.ReadByte();
	
	if(Player.Stats.startx != 4 )
	{
        Player.Stats.startx = type;
    }
	else
	{
	   type = 0;
	   Player.Stats.startx = type;
	}
    Writer.Create(0x3122);
	Writer.WriteDWord(Player.General.UniqueID);
	Writer.WriteByte(1);
	Writer.WriteByte(type);
    Writer.Finalize();
    Broadcast(true);
}
void GameSocket::OnEstrella()
{
	unsigned char type = Reader.ReadByte();
    Writer.Create(0xB683);
	Writer.WriteDWord(Player.General.UniqueID);
	Writer.WriteByte(type);
    Writer.Finalize();
    Broadcast(true);   
}

void GameSocket::OnUpdateSkill()
{ 
	printf("Error Falta: 72CB \n");
}
void GameSocket::OnMastertSkill()
{
    printf("Error Falta: 7165 \n");
}

void GameSocket::OnATSkill()
{
	printf("Error Falta: Skill at \n");
    unsigned char type = Reader.ReadByte();
	int num7;
	float num4;
	float num5;
	switch(type)
	{
	case 1:
		num4 = Player.Position.X;
		num5 = Player.Position.Y;
		Writer.Create(0xB738);
		Writer.WriteDWord(Player.General.UniqueID);
		Writer.WriteByte (1);
		Writer.WriteFloat(Player.Position.XSector);
		Writer.WriteFloat(Player.Position.YSector);
		Writer.WriteFloat(Player.Position.X + num4);
		Writer.WriteFloat(Player.Position.Z);
        Writer.WriteFloat(Player.Position.Z + num5);
		Writer.WriteByte (0);
		Writer.Finalize();
		//Send(Writer.Buffer, Writer.Size());
		break;
	case 2:
		break;


	case 4:
        num7 = Reader.ReadDWord();
		switch(type)
		{
		case 0:
			break;
		case 1:
			break;
		case 4:
			break;

		}
		break;
	}

}

void GameSocket::OnADDStr()
{
	char strx = db.Query("SELECT strength FROM characters WHERE id=%d", Player.General.CharacterID).store().num_rows();
	if(strx != 0)
	{
	   db.Query("UPDATE characters SET strength=strength+1 WHERE id=%d", Player.General.CharacterID).execute();
	   db.Query("UPDATE characters SET attribute=attribute-1 WHERE id=%d", Player.General.CharacterID).execute();
       db.Query("UPDATE characters SET hp=strength*10 WHERE id=%d", Player.General.CharacterID).execute();
       db.Query("UPDATE characters SET cur_hp=strength*10 WHERE id=%d", Player.General.CharacterID).execute();
	   Writer.Create(0xB27A);
	   Writer.WriteByte(1);
	   Writer.Finalize();
       Send(Writer.Buffer, Writer.Size());
	}
	else 
	{
     printf("se cree vivo: %s\n", Player.General.CharacterName);
	}
}
void GameSocket::OnADDInt()
{
	char intx = db.Query("SELECT intelligence FROM characters WHERE id=%d", Player.General.CharacterID).store().num_rows();
    if(intx != 0)
     {
       db.Query("UPDATE characters SET intelligence=intelligence+1 WHERE id=%d", Player.General.CharacterID).execute();
	   db.Query("UPDATE characters SET attribute=attribute-1 WHERE id=%d", Player.General.CharacterID).execute();
       db.Query("UPDATE characters SET mp=intelligence*10 WHERE id=%d", Player.General.CharacterID).execute();
	   db.Query("UPDATE characters SET cur_mp=intelligence*10 WHERE id=%d", Player.General.CharacterID).execute();
	   Writer.Create(0xB552);
	   Writer.WriteByte(1);
	   Writer.Finalize();
       Send(Writer.Buffer, Writer.Size());
	}
	else 
	{
     printf("se cree vivo: %s\n", Player.General.CharacterName);
	}
}
