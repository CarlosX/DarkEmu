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

#include "threading.h"

// ====================================================================
// == Synchronization
// ====================================================================

// FastMutex Methods

FastMutex::FastMutex(){
	cs=new CRITICAL_SECTION;
	InitializeCriticalSection((LPCRITICAL_SECTION)cs);
}
FastMutex::~FastMutex(){
	DeleteCriticalSection((LPCRITICAL_SECTION)cs);
	delete cs;
}

void FastMutex::lock(){		// Acquire ownership
	EnterCriticalSection((LPCRITICAL_SECTION)cs);
}

void FastMutex::unlock(){		// Release ownership
	LeaveCriticalSection((LPCRITICAL_SECTION)cs);
}

bool FastMutex::trylock(){		// Attempt to acquire a lock, return true if succ.
	return(TryEnterCriticalSection((LPCRITICAL_SECTION)cs)==TRUE);
}

// ========================================================================

// ThreadWaiter Methods
ThreadWaiter::ThreadWaiter(){
	ev=CreateEvent(NULL,FALSE,FALSE,NULL);
}

ThreadWaiter::~ThreadWaiter(){
	CloseHandle(ev);
}

void ThreadWaiter::set(){
	SetEvent(ev);
}

bool ThreadWaiter::wait(uint time){
	return 0==WaitForSingleObject(ev,time);
}

// ========================================================================


// ====================================================================
// == Threads
// ====================================================================


/**************
 * Thread class implementation
 **************/
Thread::Thread():suspended(true),thread(NULL),running(true),target(0){
	thread=CreateThread(NULL,NULL,(LPTHREAD_START_ROUTINE)&Thread::threadproc,(void*)this,NULL,NULL);
}

// Perhaps we should poll the runnable target if it should be deleted on completion and if so,
// delete it.
Thread::~Thread(){
	if(thread!=NULL)
		TerminateThread(thread,1);
}

bool Thread::start(Runnable* obj){
	// If we're not suspended, we can't start a new task!
	if(!suspended)return false;

	// Acquire lock
	tlock.lock();
	target = obj;
	tlock.unlock();

	// Resume thread
	suspended = false;
	waiter.set();

	return true;
}

bool Thread::shutdown(){
	// The thread will cleanly exit when it is done
	running=false;
	suspend();

	// Now wait for the thread to shutdown
	WaitForSingleObject(thread,INFINITE);

	// It's destroyed, let's set our handle back to null
	thread=NULL;

	return true;
}

bool Thread::wait(){
	if(!suspended)return false;

	runlock.lock();
	runlock.unlock();

	return true;
}

unsigned long Thread::threadproc(Thread* me){
	me->runlock.lock();

	while(me->running){
		unsigned int result=0;
		me->tlock.lock();
		if(me->target){
			// TODO: implement a switch here, 0 = delete, 1 is do not repeat (no delete), 2 means repeat immediately, 3+ = "wait x msec before repeating"
			result=me->target->run();
			switch(result){
				case RUN_DELETE:
					// Delete the object :o!!!
					delete me->target;
					me->target = 0;
					me->suspended = true;
					break;

				case RUN_NOREPEAT:
					// Do not repeat (do not delete!)
					me->target = 0;
					me->suspended = true;
					break;

				case RUN_REPEAT:
					// Repeat immediately!
					break;
			}
		}
		me->tlock.unlock();

		if(me->suspended){
			me->runlock.unlock();
			me->waiter.wait(INFINITE);	// Ender suspended wait
			me->runlock.lock();
		}

		if(result>RUN_REPEAT)
			me->waiter.wait(result);	// Wait for result msec before resuming processing
	}

	return 0;
}