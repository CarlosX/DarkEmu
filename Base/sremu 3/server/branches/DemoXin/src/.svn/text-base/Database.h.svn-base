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

#ifndef _DATABASE_H_
#define _DATABASE_H_

#include <mysql++.h>
#include <stdarg.h>

class Database 
{
private:
	
	mysqlpp::Connection connection;

public:

	Database(char* username, char* password, char* database, char* addr);

	bool IsConnected();

	mysqlpp::Query				Query(char* query, ...);
	mysqlpp::StoreQueryResult	res;

};

extern Database db;

#endif // #ifndef _DATABASE_H_