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

#ifndef _SREMU_COMMON_H_
#define _SREMU_COMMON_H_

// CRT
#include <cstdlib>
#include <cstdio>
#include <ctime>
#include <cmath>
#include <cstdarg>
#include <csignal>

// STL
#include <set>
#include <map>
#include <queue>
#include <list>
//#include <slist>

// Windows
#define WIN32_LEAN_AND_MEAN		// Get rid of shit we really don't need, like old ass winsock lib.. wtf?
// These two require windows xp sp2 +, change to 0x0501 if you want windows XP pre-sp2, or 0x0500 if you want server 2000!
#define _WIN32_WINNT 0x0502
#define WINVER 0x0502
#include <windows.h>

#ifdef SREMU_SHARED_LIB		// Are we compiling our shared lib?
#define SREMU_SHARED __declspec(dllexport)
#else						// If not, define the linkage as import!
#define SREMU_SHARED __declspec(dllimport)
#endif

// Typedefs
typedef unsigned int uint32;
typedef unsigned long long uint64;
typedef unsigned short uint16;
typedef unsigned char uint8;

typedef unsigned char ubyte;
typedef unsigned short ushort;

#endif