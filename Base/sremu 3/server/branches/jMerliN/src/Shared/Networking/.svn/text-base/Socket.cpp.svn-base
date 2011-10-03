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
#include <mswsock.h>

// Winsock startup/cleanup code
WSADATA wsa={0};

bool sremu_net_start(){
	if(!(NO_ERROR==WSAStartup(MAKEWORD(2,2),&wsa)))return false;
	
	// Do anything else here that we need for the networking..., unrelated to the server
	return true;
}

bool sremu_net_cleanup(){
	return(0==WSACleanup());
}

//=========================SOCKSET CODE!

// TODO: param testing

// Slight bottleneck for large bursts
bool Socket::send(SREMU_OVERLAPPED* ovr){
	if(!connected)return false;

	if(WSA_IO_PENDING==WSASend(s,&ovr->buf,1,NULL,NULL,(LPWSAOVERLAPPED)ovr,NULL))
		return true;
	return false;
}

bool Socket::read(SREMU_OVERLAPPED* ovr){
	if(!connected)return false;

	if(WSA_IO_PENDING==WSARecv(s,&ovr->buf,1,NULL,NULL,(LPWSAOVERLAPPED)ovr,NULL))
		return true;
	return false;
}