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

void GameSocket::OnMovement()
{
	unsigned char type = Reader.ReadByte();

	Player.Position.XSector = Reader.ReadByte();
	Player.Position.YSector	= Reader.ReadByte();
	Player.Position.X = float(Reader.ReadWord());
	Player.Position.Z = float(Reader.ReadWord());
	Player.Position.Y = float(Reader.ReadWord());
	
	if(type) 
	{
		Writer.Create(GAME_SERVER_MOVEMENT);
			Writer.WriteDWord(Player.General.UniqueID);
			Writer.WriteByte (1);
			Writer.WriteByte (Player.Position.XSector);
			Writer.WriteByte (Player.Position.YSector);
			Writer.WriteWord (short(Player.Position.X));
			Writer.WriteWord (short(Player.Position.Z));
			Writer.WriteWord (short(Player.Position.Y));
			Writer.WriteByte (0);
		Writer.Finalize();
		Broadcast(true);
	}
}