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

#ifndef _SREMU_THREADSYNC_H_
#define _SREMU_THREADSYNC_H_

#include "../Common.h"

class SREMU_SHARED Mutex
{
public:
	Mutex();
	~Mutex();

	void lock();		// Acquire Ownership
	void unlock();		// Release Ownership
	bool trylock();		// Attempt to acquire Ownership.  Return true if successful.

	void Acquire();			// Acquire Ownership
	void Release();			// Release Ownership
	bool AttemptAcquire();	// Attempt to Acquire Ownership.  Return true if successful.

private:
#if PLATFORM == PLATFORM_WIN32
	RTL_CRITICAL_SECTION cs;
#else
	static bool attr_initalized;
	static pthread_mutexattr_t attr;

	pthread_mutex_t mutex;
#endif
};

#define FastMutex Mutex

#endif // _SREMU_THREADSYNC_H_
