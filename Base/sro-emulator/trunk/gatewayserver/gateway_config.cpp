#include "gateway_config.h"

#include <sstream>
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
uint16_t	GatewayConfig::m_gateway_port			= 0;
uint32_t	GatewayConfig::m_maximal_connections	= 0;
std::string GatewayConfig::m_download_ip			= "";
uint16_t	GatewayConfig::m_download_port			= 0;
std::string GatewayConfig::m_database_host          = "";
std::string GatewayConfig::m_database				= "";
std::string GatewayConfig::m_database_user		    = "";
std::string GatewayConfig::m_database_pw	        = "";

uint16_t GatewayConfig::GatewayPort()
{
	return GatewayConfig::m_gateway_port;
};

uint32_t GatewayConfig::MaximalConnections()
{
	return GatewayConfig::m_maximal_connections;
};

std::string GatewayConfig::DownloadServerAddress()
{
	return GatewayConfig::m_download_ip;
};

uint16_t GatewayConfig::DownloadServerPort()
{
	return GatewayConfig::m_download_port;
};

std::string GatewayConfig::DatabaseHost()
{
	return GatewayConfig::m_database_host;
};

std::string GatewayConfig::Database()
{
	return GatewayConfig::m_database;
};

std::string GatewayConfig::Username()
{
	return GatewayConfig::m_database_user;
};

std::string GatewayConfig::Password()
{
	return GatewayConfig::m_database_pw;
};
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

bool GatewayConfig::Load() {

	try {
		GatewayConfig cf;

		std::ifstream ifs ("configs\\gateway.xml" , std::ifstream::binary | std::ifstream::in);
		boost::archive::xml_iarchive xml(ifs);
		xml >> boost::serialization::make_nvp("GatewayConfigs", cf);
		return true;
	}
	catch(...) {
		return false;
	}
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////