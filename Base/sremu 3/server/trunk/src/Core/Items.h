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

#ifndef _ITEMS_H_
#define _ITEMS_H_

#include "../Database.h"

class _Items 
{
private:

	struct item
	{
		unsigned int	model;
		unsigned char	type;
		unsigned short	quantity;
		unsigned int	durability;
		unsigned char	plusvalue;
		bool			in_database;
	};

	item* items[62];

public:

	_Items::_Items() {
		memset(items, 0, sizeof(items));
	}

	void Add(
		unsigned char	slot, 
		unsigned int	model, 
		unsigned char	type,
		unsigned short	quantity	= 1,
		unsigned int	durability	= 30,
		unsigned char	plusvalue	= 0,
		bool			in_database	= false)
	{
		items[slot] = new item;
		items[slot]->model		= model;
		items[slot]->type		= type;
		items[slot]->quantity	= quantity;
		items[slot]->durability = durability;
		items[slot]->plusvalue	= plusvalue;
		items[slot]->in_database= in_database;
	}

	void Swap(unsigned char source, unsigned char dest)
	{
		item* temp	  = items[source];
		items[source] = items[dest];
		items[dest]   = items[source];
	}

	void Slot(unsigned char source, unsigned char dest)
	{
		items[dest] = items[source];
		items[source]=0;
		//delete items[source];
	}

	void Del(unsigned char slot)
	{
		delete items[slot];
		items[slot]=0;
	}

	item* Item(unsigned char slot) 
	{ 
		return items[slot]; 
	}

	unsigned int FreeSlot() 
	{
		for(unsigned int i = 13; i < 62; i++)
		{
			if(items[i] == NULL) return i;
		}
		return 0;
	}

	void UpdateItems(unsigned long charid){
		for(unsigned int i=0;i<62;i++){
			if(items[i]&&!items[i]->in_database){
				db.Query("insert into items(itemtype, owner, slot, quantity, type, plusvalue) values(%d, %d, %d, %d, %d, %d)", 
					items[i]->model, 
					charid, 
					i,
					items[i]->quantity,
					items[i]->type,
					items[i]->plusvalue).execute();
			}

			if(items[i])Del(i);
		}
	}

};

#endif // _ITEMS_H_