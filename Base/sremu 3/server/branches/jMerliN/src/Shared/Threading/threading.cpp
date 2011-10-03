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

#include "sync.h"
#include "threadbase.h"
#include "threadpool.h"

/********
 * Threading synchronization structures
 ********/

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


// ThreadQueue Methods

ThreadQueue::ThreadQueue(unsigned int start,unsigned int max){
	semaphore=CreateSemaphore(NULL,start,max>0?max:50,NULL);
}

ThreadQueue::~ThreadQueue(){
	CloseHandle(semaphore);
}

bool ThreadQueue::enter(unsigned long time){
	return 0==WaitForSingleObject(semaphore,time);
}

void ThreadQueue::leave(){
	ReleaseSemaphore(semaphore,1,NULL);
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

bool ThreadWaiter::wait(unsigned long time){
	return 0==WaitForSingleObject(ev,time);
}

// ========================================================================


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



/**************
 * ThreadPool implementation
 **************/

ThreadPool::ThreadPool(unsigned int threadcount):running(false),queue(0,1000){
	SYSTEM_INFO si={0};
	GetSystemInfo(&si);

	if(threadcount==0)
		threadcount=si.dwNumberOfProcessors;

	// Spawn our threads
	threads.reserve(threadcount);
	for(unsigned int i=0;i<threadcount;i++){
		Thread* t=new Thread;
		threads.push_back(t);
		SetThreadAffinityMask(t->get_handle(),1<<(i%si.dwNumberOfProcessors));
	}
}

ThreadPool::~ThreadPool(){
	for(std::vector<Thread*>::iterator i=threads.begin();i!=threads.end();i++){
		Thread* ptr = *i;
		ptr->shutdown();
		delete ptr;
	}
}

// WARNING: DO NOT CALL THIS FUNCTION FROM A THREAD IN THE POOL.  NEVER.  ONLY THE APP'S MAIN THREAD SHOULD CALL THIS FUNCTION
bool ThreadPool::startup(){
	if(running)return false;

	for(std::vector<Thread*>::iterator i=threads.begin();i!=threads.end();i++){
		Thread* ptr = *i;
		ptr->start(this);
	}

	return (running=true);
}

// WARNING: DO NOT CALL THIS FUNCTION FROM A THREAD IN THE POOL.  NEVER.  ONLY THE APP'S MAIN THREAD SHOULD CALL THIS FUNCTION
bool ThreadPool::shutdown(){
	if(!running)return false;

	for(std::vector<Thread*>::iterator i=threads.begin();i!=threads.end();i++){
		Thread* ptr=*i;
		ptr->suspend();	// Set it into a suspended state
		ptr->wait();	// Wait for it to ender an idle state
	}

	running = false;

	return true;
}

void ThreadPool::add_task(Runnable* task,THREAD_PRIORITY pri){
	task_lock.lock();
	task_s mytask(task,pri);
	tasks.push(mytask);
	task_lock.lock();
}

unsigned int ThreadPool::run(){
	// Wait for a job
	if(queue.enter(200)){		// Only wait for 200ms, if no jobs come in by then, do other stuff?
		// Grab one
		task_lock.lock();
		task_s mytask=tasks.top();
		tasks.pop();
		task_lock.unlock();

		// Execute it
		unsigned int result=mytask.task->run();

		// Return '0' for "clean up this object", 1 for "do nothing to this object", and 2 for "re-queue this object" from your task's
		// run function!  Your task should *NOT* be run directly from a thread unless you developed it for that purpose.
		switch(result){
			case RUN_DELETE:
				// Delete the object!
				delete mytask.task;
				break;
			case RUN_REPEAT:
				// Re-add the object
				task_lock.lock();
				tasks.push(mytask);
				task_lock.unlock();
				break;
			default:
				// Do nothing to the object
				break;
		}

		queue.leave();
	}
	return RUN_REPEAT;
}