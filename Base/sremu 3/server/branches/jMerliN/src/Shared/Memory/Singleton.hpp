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

#ifndef _SREMU_SINGLETON_H_
#define _SREMU_SINGLETON_H_

#include "../common.h"

template <class T>
class Singleton {
public:
	static T* get_singleton()		{ return _myptr; }
	static T& get_singleton_ref()	{ return *_myptr; }

protected:
	Singleton() { _myptr = (T*)this; }
	~Singleton() { _myptr=0; }			// Setting to 0 is unneeded tbh

	static T* _myptr;
};

#endif // #ifndef _SREMU_SINGLETON_H_