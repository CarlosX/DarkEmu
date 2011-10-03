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

#ifndef _SREMU_THREADBASE_H_
#define _SREMU_THREADBASE_H_

#include "../common.h"
#include "sync.h"

// Runnable interface.  Anything you want to start with a thread should
// extend from Runnable publicly.
class SREMU_SHARED Runnable {
protected:
	friend class Thread;
	friend class ThreadPool;
	virtual unsigned int run()=0;
};

enum THREAD_RUN_RESULT {
	RUN_DELETE = 0,
	RUN_NOREPEAT = 1,
	RUN_REPEAT = 2
};

class SREMU_SHARED Thread {
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

#endif // #ifndef _SREMU_THREADBASE_H_