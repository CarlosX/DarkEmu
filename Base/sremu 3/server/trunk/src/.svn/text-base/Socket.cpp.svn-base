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

#include "Socket.h"

Socket::Socket(SOCKET s) 
{
	this->s = s;
}

void Socket::Receive()
{
	int ReceivedBytes = 0;
	while(ReceivedBytes = recv(s, (char*)pBuffer, sizeof(pBuffer), 0)) 
	{
		if(ReceivedBytes == 0 || ReceivedBytes == -1) break;
		ProcessData(pBuffer, ReceivedBytes);
	}
}

Socket::~Socket() 
{
	closesocket(s);
}

void Socket::Send(unsigned char* data, int size) 
{
	send(s, (char*)data, size, 0);
}

void Socket::Close() 
{
	closesocket(s);
}