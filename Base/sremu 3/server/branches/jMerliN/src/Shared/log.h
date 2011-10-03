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


#ifndef _SREMU_LOG_H_
#define _SREMU_LOG_H_

#include "common.h"

// Include threading headers
#include "threading/sync.h"

class SREMU_SHARED Log {
public:
	Log(const char* logfile=0);
	~Log();

	void print(const char* fmt,...);
	void echo(const char* fmt,...);
	void log(const char* fmt,...);

private:
	int fd;
	FastMutex log_lock,list_lock;
};

#endif // #ifndef _SREMU_LOG_H_