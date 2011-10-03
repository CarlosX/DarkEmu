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

#ifndef _PLAYER_H_
#define _PLAYER_H_

#include "Items.h"
#include "Position.h"

class Player 
{
private:

	struct _Stats 
	{
		unsigned long HP, MP;
		unsigned long Model;
		unsigned long Skillpoints;
		unsigned char Volume;
		unsigned char Level;
		unsigned short MinMag, MaxMag;
		unsigned short MinPhy, MaxPhy;
		unsigned short PhyDef, MagDef;
		unsigned short Hit, Parry;
		unsigned short Strength;
		unsigned short Intelligence;
		unsigned short Attributes;
		unsigned long long Gold;
		unsigned long long Experience;
	};

	struct _Flags 
	{
		bool GM;
		bool Berserk;
		bool Ingame;
		char PvP;
	};

	struct _Speeds 
	{
		float WalkSpeed;
		float RunSpeed;
		float BerserkSpeed;
	};

	struct _General 
	{
		unsigned long AccountID;
		unsigned long CharacterID;
		unsigned long UniqueID;
		unsigned char* CharacterName;
	};

public:

	_Items		Items;
	_Stats		Stats;
	_Position	Position;
	_Flags		Flags;
	_Speeds		Speeds;
	_General	General;

	Player::~Player() 
	{
		if(General.CharacterName != NULL) delete[] General.CharacterName;
	}

};

#endif // _PLAYER_H_