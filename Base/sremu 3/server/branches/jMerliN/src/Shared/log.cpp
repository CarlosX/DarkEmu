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

void Log::print(const char* fmt,...){
	log_lock.lock();

	// Create argument list
	va_list args;
	va_start(args,fmt);

	// Write to file if it's open
	if(fd)
		vfprintf_s((FILE*)fd,fmt,args);

	// Reset the va ptr
	va_start(args,fmt);
	
	// Write to stdout
	vprintf_s(fmt,args);

	log_lock.unlock();
}

void Log::echo(const char* fmt,...){
	log_lock.lock();

	// Create arglist
	va_list args;
	va_start(args,fmt);

	// Write to stdout
	vprintf_s(fmt,args);

	log_lock.unlock();
}

void Log::log(const char* fmt,...){
	// Check if fd is open!
	if(!fd)return;

	log_lock.lock();

	// Create arglist
	va_list args;
	va_start(args,fmt);

	// Write to file
	vfprintf_s((FILE*)fd,fmt,args);

	log_lock.unlock();
}