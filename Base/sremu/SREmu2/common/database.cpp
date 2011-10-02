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

#include "database.h"

// Result..
void MyResult::release(){
	if(res) mysql_free_result(res);
}

MyResult& MyResult::operator=(MYSQL_RES* result){
	release();
	res = result;
	return *this;
}

MyResult& MyResult::operator=(MyResult& other){
	release();
	res = other.res;
	other.res = 0;
	return *this;
}

MyResult::operator MYSQL_RES*(){
	return res;
}

// Manager...
MyUnlocker MySQLManager::get(MYSQL** conn){
	access_lock.lock();
	*conn = instance;
	return MyUnlocker(new MyScopeUnlock(&access_lock));
}

// Helpers
int mysql_queryf(MYSQL* my,const char* fmt,...){
	char qstring[4096];
	va_list args;
	va_start(args,fmt);
	int len = vsprintf_s(qstring,sizeof(qstring),fmt,args);
	va_end(args);

	//printf("QUERY: %s\n",qstring);

	return mysql_real_query(my,qstring,len);
}