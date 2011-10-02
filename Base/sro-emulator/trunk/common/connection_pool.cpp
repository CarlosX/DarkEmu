#include "connection_pool.h"

// --------------------------------------------------------------------------

MyConnectionPool::MyConnectionPool(const char* db, const char* server, const char* user, const char* password)
	: db_(db ? db : ""),
	server_(server ? server : ""),
	user_(user ? user : ""),
	password_(password ? password : "")
{
}

MyConnectionPool::~MyConnectionPool()
{
	clear();
}

// --------------------------------------------------------------------------


mysqlpp::Connection* MyConnectionPool::create()
{
	// Create connection using the parameters we were passed upon
	// creation.  This could be something much more complex, but for
	// the purposes of the example, this suffices.
	return new mysqlpp::Connection(
		db_.empty() ? 0 : db_.c_str(),
		server_.empty() ? 0 : server_.c_str(),
		user_.empty() ? 0 : user_.c_str(),
		password_.empty() ? "" : password_.c_str());
}

void MyConnectionPool::destroy(mysqlpp::Connection* cp)
{
	delete cp;
}

unsigned int MyConnectionPool::max_idle_time()
{
	return 30; // I'm not sure about this time.
}