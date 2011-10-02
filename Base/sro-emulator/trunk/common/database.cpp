#include "database.h"

// ---------------------------------------------------------
Database* Database::m_instance = NULL;
// ---------------------------------------------------------

Database::Database(std::string host, std::string db, std::string user, std::string pw)
	: m_connection_pool (db.c_str(), host.c_str(), user.c_str(), pw.c_str())
{
}

// ---------------------------------------------------------

mysqlpp::Connection* Database::GetConnection()
{
	mysqlpp::Connection* ptr = m_connection_pool.grab();
	return ptr;
}

void Database::ReleaseConnection(mysqlpp::Connection* con)
{
	m_connection_pool.release(con);
}

// ---------------------------------------------------------

Database* Database::getSingleton(std::string host, std::string db, std::string user, std::string pw)
{
	if(m_instance == NULL)
	{
		m_instance = new Database(host, db, user, pw);
	}

	return m_instance;
}