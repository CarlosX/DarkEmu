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

#ifndef _LOADER_H_
#define _LOADER_H_

#define WINVER 0x0510
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <winsock2.h>

#include <cstdlib>
#include <cstdio>
#include <ctime>

// Define our loader class
class Loader {
	STARTUPINFOA si;
	PROCESS_INFORMATION pi;
	bool loaded;
	bool running;
	bool shared;
	HANDLE share;
public:
	Loader();
	~Loader();
	bool load(const char* executable);
	bool sharedata(const char* connect_to,unsigned short port);
	bool inject();
	bool finish();
};


#endif // #ifndef _LOADER_H_