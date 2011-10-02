#ifndef _CONNECTION_POOL_H_
#define _CONNECTION_POOL_H_
#include "mysql++\mysql++.h"

// --------------------------------------------------------------------------

class MyConnectionPool : public mysqlpp::ConnectionPool
{
public:
    // The object's only constructor
    MyConnectionPool(const char* db, const char* server, const char* user, const char* password);
    ~MyConnectionPool();

protected:
    // overrides.
    mysqlpp::Connection* create();
	void destroy(mysqlpp::Connection* cp);
    unsigned int max_idle_time();

private:
    // Our connection parameters
    std::string db_, server_, user_, password_;
};


// --------------------------------------------------------------------------

#endif //_CONNECTION_POOL_H_