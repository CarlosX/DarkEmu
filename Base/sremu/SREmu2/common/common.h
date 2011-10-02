 /* 
  * This file is part of SREmu.
  *
  * Copyright (c) 2010 Justin "jMerliN" Summerlin, SREmu <http://sremu.sourceforge.net>
  *
  * SREmu is free software: you can redistribute it and/or modify
  * it under the terms of the GNU Affero General Public License as published by
  * the Free Software Foundation, either version 3 of the License, or
  * (at your option) any later version.
  * 
  * SREmu is distributed in the hope that it will be useful,
  * but WITHOUT ANY WARRANTY; without even the implied warranty of
  * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  * GNU Affero General Public License for more details.
  * 
  * You should have received a copy of the GNU Affero General Public License
  * along with SREmu.  If not, see <http://www.gnu.org/licenses/>.
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
#include <string>
#include <exception>

// Windows
#define WIN32_LEAN_AND_MEAN		// Get rid of shit we really don't need, like old ass winsock lib.. wtf?
#include <windows.h>

// Typedefs
typedef unsigned char			ubyte;
typedef unsigned short			ushort;
typedef unsigned int			uint;
typedef long long				qword;
typedef unsigned long long		uqword;
typedef float					real32;
typedef double					real64;

#endif  // #ifndef _SREMU_COMMON_H_