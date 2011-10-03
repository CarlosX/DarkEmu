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

#include "ServerSocket.h"

ServerSocket::ServerSocket(short port) 
{
	WSAData wsa;
	if(WSAStartup(MAKEWORD(2,2), &wsa) != 0) 
	{
		return;
	}
	
	SOCKADDR_IN* sa = new SOCKADDR_IN;

	sa->sin_port = htons(port);
	sa->sin_family = AF_INET;
	sa->sin_addr.s_addr = INADDR_ANY;

	s = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if(s == SOCKET_ERROR) 
	{
		return;
	}

	int ret = bind(s, (sockaddr*)sa, sizeof(*sa));

	if(ret == SOCKET_ERROR) 
	{
		closesocket(s);
		return;
	}

	listen(s, 500);

	delete[] sa;
}

SOCKET ServerSocket::Accept() 
{
	return accept(s, 0, 0);
}

ServerSocket::~ServerSocket() 
{
	WSACleanup();
}