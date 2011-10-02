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

#include "log.h"

Log::Log(const char* logfile):fd(0){
	if(logfile==0){
		char log_buffer[64];
		sprintf_s(log_buffer,sizeof(log_buffer),"./sremu_%u.log",(unsigned long)time(0));
		if(fopen_s((FILE**)&fd,log_buffer,"a+"))
			fd=0;
	}else{
		if(fopen_s((FILE**)&fd,logfile,"a+"))
			fd=0;
	}
}

Log::~Log(){
	if(fd)
		fclose((FILE*)fd);
}

void Log::log(const char* fmt,...){
	if(!fd)return;
	log_lock.lock();
	va_list args;
	va_start(args,fmt);
	vfprintf_s((FILE*)fd,fmt,args);
	log_lock.unlock();
}