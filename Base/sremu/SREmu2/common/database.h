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

#ifndef _SREMU_DATABASE_H_
#define _SREMU_DATABASE_H_

#include "common.h"

#include <winsock2.h>
#include <mysql.h>
#pragma comment(lib,"libmysql.lib")
#include <threading.h>
#include <memory>

class MySQLManager;

// RAII wrappers for exception safety..
class MyScopeUnlock {
public:
	MyScopeUnlock(FastMutex* lock):lck(lock){}
	~MyScopeUnlock(){
		lck->unlock();
	}

private:
	FastMutex* lck;
};
typedef std::auto_ptr<MyScopeUnlock> MyUnlocker;

class MyResult {
public:
	MyResult(MYSQL_RES* result=0):res(result){}
	MyResult(MyResult& other):res(other.res){
		other.res = 0;
	}
	~MyResult(){
		release();
	}
	
	void release();
	MyResult& operator =(MYSQL_RES* result);
	MyResult& operator =(MyResult& other);
	operator MYSQL_RES*();
private:
	MYSQL_RES* res;
};

class MySQLManager {
public:
	MySQLManager(MYSQL* my):instance(my){}
	MyUnlocker get(MYSQL** conn);
	
private:
	MYSQL* instance;
	FastMutex access_lock;
};

#define MYSQL_ACQUIRE() MYSQL* mysql;  MyUnlocker _LCK = g_mysql->get(&mysql)

// Helpers
int mysql_queryf(MYSQL* my,const char* fmt,...);

#endif // #ifndef _SREMU_DATABASE_H_