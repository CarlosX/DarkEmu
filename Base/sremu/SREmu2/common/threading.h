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

#ifndef _SREMU_THREADING_H_
#define _SREMU_THREADING_H_

#include "common.h"

// ====================================================================
// == Synchronization
// ====================================================================

// Your generic fast mutex..
class FastMutex {
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

// Event implementation
class ThreadWaiter {
public:
	ThreadWaiter();
	~ThreadWaiter();

	void set();

	bool wait(uint time);
private:
	HANDLE ev;
};

// ====================================================================
// == Threads
// ====================================================================

// Runnable interface.  Anything you want to start with a thread should
// extend from Runnable publicly.

enum THREAD_RUN_RESULT {
	RUN_DELETE = 0,
	RUN_NOREPEAT = 1,
	RUN_REPEAT = 2
};

class Runnable {
protected:
	friend class Thread;
	friend class ThreadPool;
	virtual THREAD_RUN_RESULT run()=0;
};

class Thread {
public:
	Thread();
	~Thread();

	bool start(Runnable* obj);	// Runs 'obj'
	bool shutdown();				// Shutdown any running thread

	// Suspend means "stop what you're doing, wait for a new job" in this context.
	void suspend()				{ suspended = true; }
	bool is_suspended()	const	{ return suspended; }

	// Wait waits for the object.  If the thread is not suspended, it returns false
	// immediately.  Otherwise it waits for the thread to signal that it is ready.
	bool wait();	// Wait for this thread to be ready for a new job

	HANDLE get_handle() { return thread; }

private:
	ThreadWaiter waiter;
	FastMutex tlock,runlock;
	Runnable* target;
	bool suspended,running;
	HANDLE thread;
	static unsigned long threadproc(Thread* me);
};

#endif // #ifndef _SREMU_THREADING_H_