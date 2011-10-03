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
#include "../ServerSocket.h"

/* Read this from a settings file. */
Database db("root", "123456", "sremux", "127.0.0.1");

int main()
{

	ServerSocket sSocket(15780);
	printf("   Server Online....!! \n");
	if(db.IsConnected()) 
	{
		while(true) 
		{
			SOCKET s = sSocket.Accept();
			printf("Accepted connection...\n");
			CreateThread(0, 0, (LPTHREAD_START_ROUTINE)GameSocket::ReceiveThread, &s, 0, 0);
		}
	} 
	else 
	{
		printf("Connection with database failed.\n");
		return 1;
	}
	return 0;
}