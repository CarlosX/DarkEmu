#include "agent_configs.h"
#include <sstream>
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
uint16_t	AgentConfigs::m_port				= 0;
uint32_t	AgentConfigs::m_maximal_connections	= 0;
uint32_t	AgentConfigs::m_server_id			= 0;
std::string AgentConfigs::m_database_string     = "";
std::string AgentConfigs::m_server_name         = "";

uint16_t AgentConfigs::Port()
{
	return AgentConfigs::m_port;
};

uint32_t AgentConfigs::MaximalConnections()
{
	return AgentConfigs::m_maximal_connections;
};

uint32_t AgentConfigs::ServerID()
{
	return AgentConfigs::m_server_id;
};

std::string AgentConfigs::DatabaseString()
{
	return AgentConfigs::m_database_string;
};

std::string AgentConfigs::ServerName()
{
	return AgentConfigs::m_server_name;
};
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

bool AgentConfigs::Load() {

	try {

		AgentConfigs cf;

		std::ifstream ifs ("configs\\agent.xml" , std::ifstream::binary | std::ifstream::in);
		boost::archive::xml_iarchive xml(ifs);
		xml >> boost::serialization::make_nvp("AgentConfigs", cf);
		return true;
	}
	catch(...) {
		return false;
	}
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////