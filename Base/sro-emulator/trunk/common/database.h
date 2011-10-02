/* 
	This class require mysql++
*/

#ifndef _DATABASE_H_
#define _DATABASE_H_

////////////////////////////////////////////////////
#include "common.h"
#include "singleton.h"

#include "mysql++/mysql++.h"
#include "connection_pool.h"
////////////////////////////////////////////////////

class Database /*: public Singleton<Database> */ // can't use singelton becuase it needs arguments.
{
	friend class Singleton<Database>;

private:
	static Database* m_instance;

	MyConnectionPool m_connection_pool;

public:
	Database    (std::string host, std::string db, std::string user, std::string pw);

	static Database* getSingleton(std::string host = "", std::string db = "", std::string user = "", std::string pw = "");
	mysqlpp::Connection* GetConnection();
	void                 ReleaseConnection(mysqlpp::Connection* conn);

	template <class type> // return type-
	type ExecuteFunction(mysqlpp::Connection* connection, std::string name, uint8_t size, ...);
};
////////////////////////////////////////////////////

// a small marco. just for shorter syntax :)
#define DatabasePtr(t) Database::getSingleton(t)


template <class type> // return type - must be supported from mysql++
type Database::ExecuteFunction(mysqlpp::Connection* connection, std::string name, uint8_t size, ...)
{
	std::string builder = "SELECT ";

	va_list arg_ptr;
	va_start(arg_ptr, size);

	builder += name ;
	builder += "("  ;

	for(int i = 0; i < size; i++)
	{
		builder += "'";
		builder += va_arg( arg_ptr, std::string);
		builder += "'";

		if(i != (size - 1))
			builder += ",";
		else
			builder += ");";
	}
	va_end(arg_ptr);
	

	mysqlpp::StoreQueryResult res;
	try
	{
		res = connection->query(builder).store();
	}
	catch (mysqlpp::BadQuery & bad)
	{
		printl("Error bad query. \n");
		return 0;
	}

	uint32_t debug = res.num_rows();
	return static_cast<type>(res[0].front());
};


#endif // _DATABASE_H_