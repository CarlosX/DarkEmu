#ifndef _GATEWAYCONFIG_H_
#define _GATEWAYCONFIG_H_
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#include <boost/archive/xml_iarchive.hpp>
#include <boost/archive/xml_oarchive.hpp>
#include <fstream>

#include "../common/common.h"
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Contains informations, which are needed from all servers.
class GatewayConfig {

private:
	friend class boost::serialization::access;

    template<class Archive> void serialize(Archive & ar, const unsigned int version) {

		ar & boost::serialization::make_nvp("Port",				     GatewayConfig::m_gateway_port);
		ar & boost::serialization::make_nvp("Maximal_Connections",   GatewayConfig::m_maximal_connections);
		ar & boost::serialization::make_nvp("DownloadServerAddress", GatewayConfig::m_download_ip);
		ar & boost::serialization::make_nvp("DownloadServerPort",    GatewayConfig::m_download_port);
		ar & boost::serialization::make_nvp("DatabaseHost",			 GatewayConfig::m_database_host);
		ar & boost::serialization::make_nvp("Database",				 GatewayConfig::m_database);
		ar & boost::serialization::make_nvp("Username",				     GatewayConfig::m_database_user);
		ar & boost::serialization::make_nvp("Password",				 GatewayConfig::m_database_pw);
    }

	static uint16_t m_gateway_port;
	static uint32_t m_maximal_connections;

	static std::string m_download_ip;
	static uint16_t	   m_download_port;

	static std::string m_database_host;
	static std::string m_database;
	static std::string m_database_user;
	static std::string m_database_pw;

public:
	static bool Load();

	static uint16_t GatewayPort();
	static uint32_t MaximalConnections();

	static std::string	DownloadServerAddress();
	static uint16_t		DownloadServerPort();

	static std::string  DatabaseHost();
	static std::string  Database();
	static std::string  Username();
	static std::string  Password();
};


#endif // _GATEWAYCONFIG_H_