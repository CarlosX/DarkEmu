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

// TODO: Incomplete.  Committing to keep things up-to-date.

Socket::Socket(SOCKET fd):s(0){

}

Socket::~Socket(){

}

bool Socket::connect(const char* host,unsigned short port){

	return true;
}

void Socket::disconnect(){

}

bool Socket::send(const SimpleBuffer& buffer){

	return true;
}

void Socket::conn_info(SOCKADDR_IN& addr){

}

bool Socket::is_connected(){

	return true;
}