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

#ifndef _SREMU_THREADPOOL_H_
#define _SREMU_THREADPOOL_H_

#include "../common.h"
#include "sync.h"
#include "threadbase.h"

enum THREAD_PRIORITY {
	THREAD_PRI_LOW,
	THREAD_PRI_NORMAL,
	THREAD_PRI_HIGH,
	THREAD_PRI_NOW
};

class SREMU_SHARED ThreadPool : public Runnable {
public:
	ThreadPool(unsigned int threadcount=0);		// Using '0' threads will cause the thread pool to spawn as many threads as there are processors avail.
	~ThreadPool();

	bool startup();
	bool shutdown();

	// Add a task to the pool with the given priority.  If the delay is bigger than 0,
	// the task will be added to a timed queue instead of the primary queue, and moved
	// when appropriate.
	void add_task(Runnable* task,THREAD_PRIORITY pri);
protected:
	unsigned int run();

private:
	bool running;
	FastMutex task_lock;
	ThreadQueue queue;
	std::vector<Thread*> threads;

	// TODO:  Make a task memory pool here, new/delete are abysmally slow.
	struct task_s {
		Runnable* task;			// The task.
		THREAD_PRIORITY pri;	// The priority.

		task_s(Runnable* t,THREAD_PRIORITY p):task(t),pri(p){}

		task_s(const task_s& other){
			task = other.task;
			pri = other.pri;
		}

		bool operator<(const task_s& other) const {return pri < other.pri;}
		bool operator>(const task_s& other) const {return pri > other.pri;}
	};

	std::priority_queue<task_s> tasks;
};

#endif // #ifndef _SREMU_THREADPOOL_H_