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

#if PLATFORM == PLATFORM_NIX
Mutex::attr_initalized = false;
pthread_mutexattr_t Mutex::attr;
#endif

Mutex::Mutex()
{
#if PLATFORM == PLATFORM_WIN32
	InitializeCriticalSection(&cs);
#else

#if NIX_FLAVOR == NIX_FLAVOR_BSD
#define recursive_mutex_flag PTHREAD_MUTEX_RECURSIVE	
#else
#define recursive_mutex_flag PTHREAD_MUTEX_RECURSIVE_NP
#endif

	if(!attr_initalized)
	{
		pthread_mutexattr_init(&attr);
		pthread_mutexattr_settype(&attr, recursive_mutex_flag);
		attr_initalized = true;
	}

	pthread_mutex_init(&mutex, &attr);
#endif
}

Mutex::~Mutex()
{
#if PLATFORM == PLATFORM_WIN32
	DeleteCriticalSection(&cs);
#else
	pthread_mutex_destroy(&mutex);
#endif
}

void Mutex::Acquire()
{
#if PLATFORM == PLATFORM_WIN32
	EnterCriticalSection(&cs);
#else
	pthread_mutex_lock(&mutex);
#endif
}

void Mutex::Release()
{
#if PLATFORM == PLATFORM_WIN32
	LeaveCriticalSection(&cs);
#else
	pthread_mutex_unlock(&mutex);
#endif
}

bool Mutex::AttemptAcquire()
{
#if PLATFORM == PLATFORM_WIN32
	return (TryEnterCriticalSection(&cs) != 0 ? true : false);
#else
	return (pthread_mutex_trylock(&mutex) == 0);
#endif
}

void Mutex::lock()
{
#if PLATFORM == PLATFORM_WIN32
	EnterCriticalSection(&cs);
#else
	pthread_mutex_lock(&mutex);
#endif
}

void Mutex::unlock()
{
#if PLATFORM == PLATFORM_WIN32
	LeaveCriticalSection(&cs);
#else
	pthread_mutex_unlock(&mutex);
#endif
}

bool Mutex::trylock()
{
#if PLATFORM == PLATFORM_WIN32
	return (TryEnterCriticalSection(&cs) != 0 ? true : false);
#else
	return (pthread_mutex_trylock(&mutex) == 0);
#endif
}