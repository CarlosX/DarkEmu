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

void GameSocket::OnGM() 
{   
    
	if(Player.Flags.GM) //Player.Flags.GM
	{
	   unsigned char id = Reader.ReadByte();
	   //printf(" GM ID %d \n", id);
       switch(id) 
	   {
			case 6:  // Create monster.
				OnSpawnMonster();
				break;
			case 7:	 // Create item.
				OnMakeItem();
				break;
			case 16: // Teleport
				 break;
		}
		
	} 
	else 
	{
		/* Hack attempt. */
		printf(" Player no es GM \n");
	}
}

void GameSocket::OnMakeItem() 
{
	unsigned int item	= Reader.ReadDWord();
	unsigned char plus	= Reader.ReadByte();

	/* We should check for item type here. Since items are not yet in DB, we'll assume CH/EU. */
	unsigned char type = 0;
	switch(type) 
	{
		case 0:
			unsigned int slot = Player.Items.FreeSlot();
			Player.Items.Add(slot, item, type, 1, 30, plus);
			ItemToInventory(slot);
			break;
	}
}

void GameSocket::OnSpawnMonster() 
{
	unsigned int id = Reader.ReadDWord();
	
	/* Temporary: */
	_Position* x = new _Position;
	x->XSector = Player.Position.XSector;
	x->YSector = Player.Position.YSector;
	x->X = Player.Position.X + 5;
	x->Y = Player.Position.Y + 5;
	x->Z = Player.Position.Z;

	SpawnMonster(x, id);

	delete x;
	//printf(" primera \n");
}