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

/* DON'T TOUCH THIS SECTION!  Just don't do it...  It's like dividing by 0, it causes badness. */
#define PLATFORM_WIN32 0
#define PLATFORM_NIX   1
#define NIX_FLAVOR_LINUX  0
#define NIX_FLAVOR_BSD    1
#define NIX_FLAVOR_OTHER  2
#define COMPILER_MICROSOFT 0
#define COMPILER_BORLAND   1
#define COMPILER_GNU       2
/* Okay,  end of the division by zero section.  Settings are below for now.  Eventually, I'll make these automated. */

/* Set Platform Here
 *    Options:
 *      PLATFORM_WIN32: 32-bit Microsoft Windows
 *      PLATFORM_NIX:   Any *Nix-Based Platform
 */
#define PLATFORM PLATFORM_WIN32

/* Set *Nix Flavor Here (If Applicable)
 *     Options:
 *       NIX_FLAVOR_LINUX: Linux
 *       NIX_FLAVOR_BSD:   BSD-Derived
 *       NIX_FLAVOR_OTHER: Not Listed
 */
#if PLATFORM == PLATFORM_NIX
	#define NIX_FLAVOR NIX_FLAVOR_LINUX
#endif

/* Set Compiler Brand Here
 *     Options:
 *       COMPILER_MICROSOFT: Microsoft
 *       COMPILER_BORLAND:   Borland
 *       COMPILER_GNU:       GNU
 */
#define COMPILER COMPILER_MICROSOFT

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
#include <deque>

#if PLATFORM == PLATFORM_WIN32
#define _WIN32_WINNT 0x0510
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#endif

#if PLATFORM == PLATFORM_WIN32
#define SREMU_SHARED __declspec(dllexport)
#define PLUGIN_SHARED __declspec(dllimport)
#endif

#if PLATFORM == PLATFORM_WIN32
#define SR_INLINE __forceinline
#else
#define SR_INLINE inline
#endif

// Visual Studio doesn't like to be subtle sometimes =(
#if COMPILER == MICROSOFT && _MSC_VER >= 1300
// Force calls to snprtinf and vsnprintf to be redirected to the appropriate functions for VC++.
#define snprintf _snprintf
#define vsnprintf _vsnprintf

// Disable whining about using 'this' as a member initializer on VC++.
#pragma warning(disable: 4355)
// Silence Warnings about the new secure functions.
#pragma warning(disable: 4996)

#endif

#ifdef min
#undef min
#endif
#ifdef max
#undef max
#endif
#ifdef MIN
#undef MIN
#endif
#ifdef MAX
#undef MAX
#endif

// Typedef our variables so that correct sizes are always used.
// Different Typedef'ing is needed if compiling on a GNU compiler.
#ifdef USING_COMPILER_GNU
typedef int64_t int64;
typedef int32_t int32;
typedef int16_t int16;
typedef int8_t int8;

typedef uint64_t uint64;
typedef uint32_t uint32;
typedef uint16_t uint16;
typedef uint8_t uint8;
#else
typedef signed __int64 int64;
typedef signed __int32 int32;
typedef signed __int16 int16;
typedef signed __int8 int8;

typedef unsigned __int64 uint64;
typedef unsigned __int32 uint32;
typedef unsigned __int16 uint16;
typedef unsigned __int8 uint8;
#endif

#endif		// _SREMU_COMMON_H