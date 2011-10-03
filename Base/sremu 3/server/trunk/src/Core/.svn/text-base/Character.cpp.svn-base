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

void GameSocket::OnCharacter() 
{
	char pType = Reader.ReadByte();
	switch(pType) 
	{
		case 1:
			OnCharCreation();
			break;
		case 2:
			SendCharacterList();
			break;
		case 3:
			OnCharDeletion();
			break;
		case 4:
			OnCharnameCheck();
			break;
		case 5:
			OnCharRestore();
			break;
	}
}

void GameSocket::SendCharacterList() 
{
	Writer.Create(GAME_SERVER_CHARACTER);
		Writer.WriteWord(0x0102);

		mysqlpp::StoreQueryResult res = 
			db.Query("select * from characters where account=%d", Player.General.AccountID).store();

		Writer.WriteByte (res.num_rows());

		for(unsigned int i = 0; i < res.num_rows(); ++i) 
		{	
			unsigned char* name  = (unsigned char*)res[i]["name"].c_str();
			unsigned int namelen = strlen((char*)name);

			Writer.WriteDWord(atoi(res[i]["chartype"]));
			Writer.WriteWord(strlen(res[i]["name"]));
			Writer.WriteString(name, namelen);
			Writer.WriteByte(atoi(res[i]["volume"]));
			Writer.WriteByte(atoi(res[i]["level"]));
			Writer.WriteQWord(atoi(res[i]["experience"]));
			Writer.WriteWord(atoi(res[i]["strength"]));
			Writer.WriteWord(atoi(res[i]["intelligence"]));
			Writer.WriteWord(atoi(res[i]["attribute"]));
			Writer.WriteDWord(atoi(res[i]["hp"]));
			Writer.WriteDWord(atoi(res[i]["mp"]));
			Writer.WriteByte(0x00);
			Writer.WriteWord(0x00);
			Writer.WriteByte(0x00);

			mysqlpp::StoreQueryResult items = 
				db.Query("select itemtype, plusvalue from items where owner=%d and slot >= 0 and slot <= 8", atoi(res[i]["id"])).store();

			Writer.WriteByte(items.num_rows());
			for(unsigned int j = 0; j < items.num_rows(); j++) 
			{
				Writer.WriteDWord(atoi(items[j]["itemtype"]));
				Writer.WriteByte(atoi(items[j]["plusvalue"]));
			}
			Writer.WriteByte(0x00);
		}
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());
}

void GameSocket::OnCharCreation() 
{
	unsigned short charlen = Reader.ReadWord();
	char* name = new char[charlen+1];
	memset(name, 0x00, charlen+1);
	Reader.ReadString((unsigned char*)name, charlen);

	unsigned int  model	 = Reader.ReadDWord();
	unsigned char volume = Reader.ReadByte();

	/* TODO: These items should be validated. */
	unsigned int _item[4];
	_item[0] = Reader.ReadDWord();
	_item[1] = Reader.ReadDWord();
	_item[2] = Reader.ReadDWord();
	_item[3] = Reader.ReadDWord();

	Writer.Create(GAME_SERVER_CHARACTER);

		Writer.WriteByte(0x01);

		if(db.Query("select * from characters where account=%d", Player.General.AccountID).store().num_rows() < 4) 
		{
			unsigned int charid = CharacterExists(name);
			if(!charid) 
			{
				db.Query("insert into characters(account, name, chartype, volume) values(%d, '%s', %d, %d)",
					Player.General.AccountID,
					name,
					model,
					volume)
				.execute();

				charid = CharacterExists(name);

				AddItem(charid, 1, _item[0]);
				for(unsigned int i = 1; i < 4; i++) 
				{
					AddItem(charid, i+3, _item[i]);
				}

				/* Note: need European support here. */
				AddMastery(charid, 257);
				AddMastery(charid, 258);
				AddMastery(charid, 259);
				AddMastery(charid, 273);
				AddMastery(charid, 274);
				AddMastery(charid, 275);
				AddMastery(charid, 276);

				Writer.WriteByte(0x01);

			} 
			else 
			{
				Writer.WriteByte(0x02);
				Writer.WriteByte(0x10);	/* Character exists. */
			}
		} 
		else 
		{
			Writer.WriteByte(0x02);
			Writer.WriteByte(0x10); /* Account has > 4 characters. */
		}

	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	delete[] name;
}

void GameSocket::OnCharDeletion() 
{
	/*  This is where we should set restore flag, deletion time, etc.
		Just deleting from database for now.
		Also, we MIGHT want to delete items related to this character from items table. */

	unsigned short charlen = Reader.ReadWord();
	char* name = new char[charlen+1];
	memset(name, 0x00, charlen+1);
	Reader.ReadString((unsigned char*)name, charlen);

	int charid = CharacterExists(name);

	Writer.Create(GAME_SERVER_CHARACTER);

		Writer.WriteByte(0x03);

		if(charid) 
		{
			db.Query("delete from characters where name='%s'", name).execute();
			db.Query("delete from items where owner=%d", charid).execute();
			Writer.WriteByte(0x01);
		} 
		else 
		{
			Writer.WriteByte(0x02);
			Writer.WriteByte(0x01);
		}

	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	SendCharacterList();

	delete[] name;
}

void GameSocket::OnCharnameCheck() 
{
	unsigned short charlen = Reader.ReadWord();
	char* name = new char[charlen+1];
	memset(name, 0x00, charlen+1);
	Reader.ReadString((unsigned char*)name, charlen);

	Writer.Create(GAME_SERVER_CHARACTER);
		Writer.WriteByte(0x04);
		if(!CharacterExists(name)) 
		{
			Writer.WriteByte(0x01);
		} 
		else 
		{
			Writer.WriteByte(0x02);
			Writer.WriteByte(0x10);
		}
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	delete[] name;
}

void GameSocket::OnCharRestore() {}

int GameSocket::CharacterExists(char* charname)
{
	mysqlpp::StoreQueryResult res = db.Query("select id from characters where name='%s'", charname).store();
	return res.num_rows() == 0 ? 0 : atoi(res[0]["id"]); 
}

void GameSocket::OnIngameRequest() 
{
	Writer.Create(GAME_SERVER_INGAME_ACCEPT);
		Writer.WriteByte(0x01);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(GAME_SERVER_LOADING_START);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	unsigned short charlen = Reader.ReadWord();
	Player.General.CharacterName = new unsigned char[charlen+1];
	memset(Player.General.CharacterName, 0x00, charlen+1);
	Reader.ReadString(Player.General.CharacterName, charlen);

	Player.General.CharacterID = CharacterExists((char*)Player.General.CharacterName);
	Player.General.UniqueID = Player.General.CharacterID + 300000;

	mysqlpp::StoreQueryResult res = 
		db.Query("select * from characters where name='%s'", Player.General.CharacterName).store();

	Player.Stats.HP				= atoi(res[0]["hp"]); /* Max should be calculated from gear stats! */
	Player.Stats.MP				= atoi(res[0]["mp"]); /* Max should be calculated from gear stats! */
	Player.Stats.HP				= atoi(res[0]["cur_hp"]);
	Player.Stats.MP				= atoi(res[0]["cur_mp"]);
	Player.Stats.Model			= atoi(res[0]["chartype"]);
	Player.Stats.Volume			= atoi(res[0]["volume"]);
	Player.Stats.Level			= atoi(res[0]["level"]);
	Player.Stats.Experience		= atoi(res[0]["experience"]);
	Player.Stats.Gold			= atoi(res[0]["gold"]);
	Player.Stats.Skillpoints	= atoi(res[0]["sp"]);
	Player.Stats.Attributes		= atoi(res[0]["attribute"]);
	Player.Flags.Berserk		= atoi(res[0]["berserking"]);
	Player.Speeds.WalkSpeed		= float(atoi(res[0]["walkspeed"]));
	Player.Speeds.RunSpeed		= float(atoi(res[0]["runspeed"]));
	Player.Speeds.BerserkSpeed	= float(atoi(res[0]["berserkspeed"]));
	Player.Stats.MinPhy			= atoi(res[0]["min_phyatk"]);
	Player.Stats.MaxPhy			= atoi(res[0]["max_phyatk"]);
	Player.Stats.MinMag			= atoi(res[0]["min_magatk"]);
	Player.Stats.MaxMag			= atoi(res[0]["max_magatk"]);
	Player.Stats.PhyDef			= atoi(res[0]["phydef"]);
	Player.Stats.MagDef			= atoi(res[0]["magdef"]);
	Player.Stats.Hit			= atoi(res[0]["hit"]);
	Player.Stats.Parry			= atoi(res[0]["parry"]);
	Player.Stats.Strength		= atoi(res[0]["strength"]);
	Player.Stats.Intelligence 	= atoi(res[0]["intelligence"]);
	Player.Flags.GM				= atoi(res[0]["gm"]);
	Player.Flags.PvP			= -1 /*atoi(res[0]["pvp"])*/;
	Player.Position.XSector		= atoi(res[0]["xsect"]);
	Player.Position.YSector		= atoi(res[0]["ysect"]);
	Player.Position.X			= float(atoi(res[0]["xpos"]));
	Player.Position.Y			= float(atoi(res[0]["ypos"]));
	Player.Position.Z			= float(atoi(res[0]["zpos"]));

	mysqlpp::StoreQueryResult items =
		db.Query("select * from items where owner=%d", Player.General.CharacterID).store();
	
	unsigned char num_items = items.num_rows();
	for(unsigned int i = 0; i < num_items; i++) 
	{
		unsigned char slot = atoi(items[i]["slot"]);
		Player.Items.Add
		(
			slot, 
			atoi(items[i]["itemtype"]),
			atoi(items[i]["type"]),
			atoi(items[i]["quantity"]),
			atoi(items[i]["durability"]),
			atoi(items[i]["plusvalue"]),
			true
		);
	}

	Writer.Create(GAME_SERVER_CHARDATA);

		Writer.WriteDWord(Player.Stats.Model);
		Writer.WriteByte (Player.Stats.Volume);
		Writer.WriteByte (Player.Stats.Level);
		Writer.WriteByte (Player.Stats.Level);
		Writer.WriteQWord(Player.Stats.Experience);
		Writer.WriteWord (0);							// SP Bar
		Writer.WriteWord (0);							// XP Bar
		Writer.WriteQWord(Player.Stats.Gold);
		Writer.WriteDWord(Player.Stats.Skillpoints);
		Writer.WriteWord (Player.Stats.Attributes);
		Writer.WriteByte (0);							// Berserk bar
		Writer.WriteDWord(0);							// Gathered exp?
		Writer.WriteDWord(Player.Stats.HP);
		Writer.WriteDWord(Player.Stats.MP);
		Writer.WriteByte (Player.Stats.Level < 20 ? 1 : 0);
		Writer.WriteByte (0);							// Daily PK
		Writer.WriteWord (0);							// PK level
		Writer.WriteDWord(0);							// Murder level
		Writer.WriteByte (0x2D);						// Max item slots

		Writer.WriteByte (num_items);
		for(unsigned int i = 0; i < 62; i++) 
		{
			if(Player.Items.Item(i) != NULL)
			{
				Writer.WriteByte (i);
				Writer.WriteDWord(Player.Items.Item(i)->model);
				switch(Player.Items.Item(i)->type)
				{
					case 0:
						Writer.WriteByte (Player.Items.Item(i)->plusvalue);
						Writer.WriteQWord(0);
						Writer.WriteDWord(Player.Items.Item(i)->durability);
						Writer.WriteByte (0);
						break;
					case 1:
						Writer.WriteWord (Player.Items.Item(i)->quantity);
						break;
				}
			}
		}

		// Avatar data.
		Writer.WriteByte (5); // Avatar start
		Writer.WriteByte (0); // Avatar count

		// Mastery data.
		Writer.WriteByte (0);
		mysqlpp::StoreQueryResult masteries = 
			db.Query("select * from masteries where owner=%d", Player.General.CharacterID).store();
		for(unsigned int i = 0; i < masteries.num_rows(); i++) 
		{
			Writer.WriteByte (1);
			Writer.WriteDWord(atoi(masteries[i]["mastery"]));
			Writer.WriteByte (atoi(masteries[i]["slevel"]));
		}

		// Skill data.
		Writer.WriteByte (2);
		Writer.WriteByte (0);
		Writer.WriteByte (2);
		Writer.WriteWord (1);
		Writer.WriteDWord(0);

		// Quest data.
		mysqlpp::StoreQueryResult quests =
			db.Query("select * from quests where owner=%d", Player.General.CharacterID).store();
		Writer.WriteByte (quests.num_rows());

		Writer.WriteByte (0);
		Writer.WriteDWord(Player.General.UniqueID);

		Writer.WriteByte (Player.Position.XSector);
		Writer.WriteByte (Player.Position.YSector);
		Writer.WriteFloat(Player.Position.X);
		Writer.WriteFloat(Player.Position.Z);
		Writer.WriteFloat(Player.Position.Y);

		// Angle data & movement flags.
		Writer.WriteWord (0);							// Angle
		Writer.WriteByte (0);						
		Writer.WriteByte (1);
		Writer.WriteByte (0);
		Writer.WriteWord (0);							// Angle
		Writer.WriteWord (0);			
		
		Writer.WriteByte (Player.Flags.Berserk);
		Writer.WriteFloat(Player.Speeds.WalkSpeed);
		Writer.WriteFloat(Player.Speeds.RunSpeed);
		Writer.WriteFloat(Player.Speeds.BerserkSpeed);

		Writer.WriteByte (0);

		Writer.WriteWord (charlen);
		Writer.WriteString(Player.General.CharacterName, charlen);

		// Job data.
		Writer.WriteWord (0);							// Job alias
		Writer.WriteDWord(0x00000100);
		Writer.WriteDWord(0);
		Writer.WriteDWord(0);
		Writer.WriteDWord(0);
		Writer.WriteByte (0);
	
		Writer.WriteByte (Player.Flags.PvP);			// PK Flag (02: enabled, FF: disabled)
		Writer.WriteDWord(0);		
		Writer.WriteDWord(Player.General.AccountID);
		Writer.WriteByte (Player.Flags.GM);

		// Unknown data.
		Writer.WriteDWord(0);
		Writer.WriteDWord(0);
		Writer.WriteWord (0);
		Writer.WriteWord (1);
		Writer.WriteWord (1);
		Writer.WriteWord (0);

	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(GAME_SERVER_STATS);
		Writer.WriteDWord(Player.Stats.MinPhy);
		Writer.WriteDWord(Player.Stats.MaxPhy);
		Writer.WriteDWord(Player.Stats.MinMag);
		Writer.WriteDWord(Player.Stats.MaxMag);
		Writer.WriteWord (Player.Stats.PhyDef);
		Writer.WriteWord (Player.Stats.MagDef);
		Writer.WriteWord (Player.Stats.Hit);
		Writer.WriteWord (Player.Stats.Parry);
		Writer.WriteDWord(Player.Stats.HP);
		Writer.WriteDWord(Player.Stats.MP);
		Writer.WriteWord (Player.Stats.Strength);
		Writer.WriteWord (Player.Stats.Intelligence);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(GAME_SERVER_LOADING_END);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());
}