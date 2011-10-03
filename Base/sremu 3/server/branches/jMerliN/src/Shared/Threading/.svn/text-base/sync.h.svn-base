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

#include "../common.h"

// Thread sync. structures
class SREMU_SHARED FastMutex {
public:
	FastMutex();
	~FastMutex();

	void lock();		// Acquire ownership
	void unlock();		// Release ownership
	bool trylock();		// Attempt to acquire a lock, return true if succ.

private:
	FastMutex(const FastMutex& other);

	void* cs;
};

// Semaphore implementation
class SREMU_SHARED ThreadQueue {
public:
	ThreadQueue(unsigned int start=0,unsigned int max=50);
	~ThreadQueue();

	bool enter(unsigned long time);
	void leave();

private:
	HANDLE semaphore;
};

// Event implementation
class SREMU_SHARED ThreadWaiter {
public:
	ThreadWaiter();
	~ThreadWaiter();

	void set();

	bool wait(unsigned long time);
private:
	HANDLE ev;
};

#endif // #ifndef _SREMU_THREADSYNC_H_
