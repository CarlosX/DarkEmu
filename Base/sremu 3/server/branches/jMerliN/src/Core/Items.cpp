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
#include "Items.h"

void GameSocket::OnItem() 
{
	unsigned char type = Reader.ReadByte();
	switch(type) 
	{
		case 0:	/* Move in inventory. */
		{
			unsigned char slot_source = Reader.ReadByte();
			unsigned char slot_destination = Reader.ReadByte();
			unsigned short amount = Reader.ReadWord();

			if(Player.Items.Item(slot_source) != NULL)					// Existence check.
			{
				if(amount <= Player.Items.Item(slot_source)->quantity)	// Quantity check.
				{
					if(Player.Items.Item(slot_source)->type == 0)		// Equip / Unequip check.
					{
						if(slot_source < 13) 
						{
							UnEquipItem(slot_source, Player.Items.Item(slot_source)->model);
						}
						if(slot_destination < 13) 
						{
							/* We should check item stats to see if we are allowed to equip it. */
							EquipItem
							(
								slot_destination, 
								Player.Items.Item(slot_source)->model, 
								Player.Items.Item(slot_source)->plusvalue
							);
						}
					}
					if(Player.Items.Item(slot_destination) == NULL) 
					{ 
						if(amount == 0) 
						{
							Player.Items.Slot(slot_source, slot_destination);
						} 
						else 
						{		
							Player.Items.Add
							(
								slot_destination,
								Player.Items.Item(slot_source)->model,
								Player.Items.Item(slot_source)->type,
								amount,
								Player.Items.Item(slot_source)->durability,
								Player.Items.Item(slot_source)->plusvalue
							);

							Player.Items.Item(slot_source)->quantity -= amount;
						}
					} 
					else 
					{ 
						if(Player.Items.Item(slot_source)->model == Player.Items.Item(slot_destination)->model &&
							Player.Items.Item(slot_source)->type == 1) 
						{
							Player.Items.Del(slot_source);
							Player.Items.Item(slot_destination)->quantity += amount;
						} 
						else 
						{
							Player.Items.Swap(slot_source, slot_destination);
						}
					}
					Writer.Create(GAME_SERVER_ITEM_MOVEMENT);
						Writer.WriteByte (1);
						Writer.WriteByte (type);
						Writer.WriteByte (slot_source);
						Writer.WriteByte (slot_destination);
						Writer.WriteWord (amount);
						Writer.WriteByte (0);		// ?
					Writer.Finalize();
					Send(Writer.Buffer, Writer.Size());
				} 
				else 
				{
					/* Source amount is less than moving amount. Hack attempt? */
					return;
				}
			} 
			else 
			{
				/* Source item does not exist. Hack attempt? */
				return;
			}
			break;
		}
		case 7:	/* Drop to ground. */
		{
			unsigned char slot_source = Reader.ReadByte();

			if(Player.Items.Item(slot_source) != NULL)
			{
				/* Make item disappear :) */
				Writer.Create(GAME_SERVER_ITEM_MOVEMENT);
					Writer.WriteByte (1);
					Writer.WriteByte (type);
					Writer.WriteByte (slot_source);
				Writer.Finalize();
				Send(Writer.Buffer, Writer.Size());

				/* This only supports CH/EU! */
				Writer.Create(GAME_SERVER_SPAWN);

					Writer.WriteDWord(Player.Items.Item(slot_source)->model);
						// if(itemtype == CH/EU)
						Writer.WriteByte (Player.Items.Item(slot_source)->plusvalue);

					Writer.WriteDWord(313337); // Pickup ID, this should be a unique value!

					/* Positional data, randomize it around the player. */
					Writer.WriteByte (Player.Position.XSector);
					Writer.WriteByte (Player.Position.YSector);
					Writer.WriteFloat(Player.Position.X + 5.0f);
					Writer.WriteFloat(Player.Position.Z + 5.0f);
					Writer.WriteFloat(Player.Position.Y + 5.0f);

					Writer.WriteWord (0xA6AA);		// Angle?
					Writer.WriteByte (1);
					Writer.WriteDWord(0xFFFFFFFF);	// Owner.
					Writer.WriteByte (0);
					Writer.WriteByte (6);
					Writer.WriteDWord(Player.General.UniqueID);

				Writer.Finalize();
				Send(Writer.Buffer, Writer.Size());

				/* This item should go into our "drop list" here... */
				Player.Items.Del(slot_source);
			}
			else 
			{
				/* Source item does not exist. Hack Attempt? */
				return;
			}
			break;
		}

		default:
			printf("Unknown item action: %d\n", type);
	}

}

void GameSocket::UnEquipItem(unsigned char source, unsigned int itemtype) 
{
	Writer.Create(GAME_SERVER_ITEM_UNEQUIP);
		Writer.WriteDWord(Player.General.UniqueID);
		Writer.WriteByte (source);
		Writer.WriteDWord(itemtype);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Broadcast(true);
}


void GameSocket::EquipItem(unsigned char dest, unsigned int itemtype, unsigned int plusvalue) 
{
	Writer.Create(GAME_SERVER_ITEM_EQUIP);
		Writer.WriteDWord(Player.General.UniqueID);
		Writer.WriteByte (dest);
		Writer.WriteDWord(itemtype);
		Writer.WriteByte (plusvalue);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Broadcast(true);
}

void GameSocket::ItemToInventory(unsigned char slot) 
{
	Writer.Create(GAME_SERVER_ITEM_MOVEMENT);
		Writer.WriteByte (1);
		Writer.WriteByte (6);		// "Gain item".
		Writer.WriteByte (slot);
		Writer.WriteDWord(Player.Items.Item(slot)->model);
		Writer.WriteByte (Player.Items.Item(slot)->plusvalue);
		/* We should grab this stuff from DB. */
		Writer.WriteWord (300);		// Reinforce PHY
		Writer.WriteWord (300);		// Reinforce mag
		Writer.WriteDWord(0);
		Writer.WriteDWord(Player.Items.Item(slot)->durability);
		Writer.WriteByte (0);		// Blue stats
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());
}

/*	Do not like this:
	but we'll keep it temporarily so we don't break char creation. */
void GameSocket::AddItem(unsigned int charid, unsigned char slot, unsigned int itemtype, unsigned int amount, unsigned int type, unsigned int plusvalue) 
{
	db.Query("insert into items(itemtype, owner, slot, quantity, type, plusvalue) values(%d, %d, %d, %d, %d, %d)", 
		itemtype, 
		charid, 
		slot,
		amount,
		type,
		plusvalue)
	.execute();
}