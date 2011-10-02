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


#ifndef _SREMU_LOG_H_
#define _SREMU_LOG_H_

// Include threading
#include "common.h"
#include "threading.h"

class Log{
public:
	Log(const char* logfile=0);
	~Log();

	void log(const char* fmt,...);

private:
	int fd;
	FastMutex log_lock;
};

#endif // #ifndef _SREMU_LOG_H_