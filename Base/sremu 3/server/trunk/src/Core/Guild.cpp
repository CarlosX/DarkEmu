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

/*	These are just packet implementations!
	No interaction with database, nor a "globally" known object/guild manager. */

#include "GameSocket.h"

void GameSocket::SendGuildInfo(bool update)
{
	char guildname[]	  = "SREmu Development Team";
	char guildnotetitle[] = "Read this!";
	char guildnotemsg[]   = "Welcome to our guild!";

	Writer.Create(update ? GAME_SERVER_GUILD_UPDATE : GAME_SERVER_GUILD_INFO);
		if(!update) Writer.WriteByte(1);
		Writer.WriteDWord(0x1234);	// Unique Guild ID
		Writer.WriteWord (strlen(guildname));
		Writer.WriteString(
			(unsigned char*)guildname, strlen(guildname));
		Writer.WriteByte (1);		// Guild level
		Writer.WriteDWord(8);		// Guild GP
		Writer.WriteWord (strlen(guildnotetitle));
		Writer.WriteString(
			(unsigned char*)guildnotetitle, strlen(guildnotetitle));
		Writer.WriteWord (strlen(guildnotemsg));
		Writer.WriteString(
			(unsigned char*)guildnotemsg, strlen(guildnotemsg));
		Writer.WriteDWord(0);		// ? Related to War/Union ?
		Writer.WriteByte (0);		// ? Related to War/Union ?

		unsigned int num_players = 0;
		Writer.WriteByte (num_players);
		/* for(unsigned int i = 0; i < num_players; i++) 
		{
			Writer.WriteDWord(player[i]->CharacterID);
			Writer.WriteWord (strlen(player[i]->CharacterName));
			Writer.WriteString(
				(unsigned char*)player[i]->CharacterName, strlen(player[i]->CharacterName));
			Writer.WriteByte (0);	// ? Point contribution to war ?
			Writer.WriteByte (player[i]->Level);
			Writer.WriteDWord(0);	// Donated GP
			Writer.WriteDWord(-1);	// ?
			Writer.WriteDWord(0);	// ?
			Writer.WriteDWord(0);	// ?
			Writer.WriteDword(0);	// ?
			Writer.WriteWord (0);	// Grant name len
			Writer.WriteDWord(player[i]->Model);
		}*/
	Writer.Finalize();
}

void GameSocket::SendGuildUnknown1() 
{
	/* This doesnt seem to be guild invite :) (jMerliN & Nick are noobs) */
	char guildname[] = "SREmu Development Team";
	Writer.Create(GAME_SERVER_GUILD_UNKNOWN1);
		Writer.WriteDWord(0x1234);	// Guildmaster ID
		Writer.WriteDWord(0x5678);	// Guild ID
		Writer.WriteWord (strlen(guildname));
		Writer.WriteString(
			(unsigned char*)guildname, strlen(guildname));
		Writer.WriteWord (0);		// Guildmaster grand name len
		Writer.WriteDWord(0);		// ?
		Writer.WriteDWord(0);		// ?
		Writer.WriteDWord(0);		// ? Amount of guilds in union ?
		Writer.WriteByte (1);		// ?
	Writer.Finalize();
}