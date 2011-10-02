#ifndef _AGENTCONFIG_H_
#define _AGENTCONFIG_H_
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#include <boost/archive/xml_iarchive.hpp>
#include <boost/archive/xml_oarchive.hpp>
#include <fstream>

#include "../common/common.h"
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Contains informations, which are needed from all servers.
class AgentConfigs {

private:
	friend class boost::serialization::access;

    template<class Archive> void serialize(Archive & ar, const unsigned int version) {

		ar & boost::serialization::make_nvp("Port",				     AgentConfigs::m_port);
		ar & boost::serialization::make_nvp("Maximal_Connections",   AgentConfigs::m_maximal_connections);
		ar & boost::serialization::make_nvp("Database",				 AgentConfigs::m_database_string);
		ar & boost::serialization::make_nvp("ServerName",			 AgentConfigs::m_server_name);
		ar & boost::serialization::make_nvp("ServerID",				 AgentConfigs::m_server_id);
		//ar & boost::serialization::make_nvp("DropRate",				 AgentConfigs::m_server_name);
		//ar & boost::serialization::make_nvp("ExpRate",				 AgentConfigs::m_server_name);
		//ar & boost::serialization::make_nvp("SpRate",				 AgentConfigs::m_server_name);
    }

	static std::string m_database_string;
	static std::string m_server_name;

	static uint32_t m_server_id;
	static uint16_t m_port;
	static uint32_t m_maximal_connections;

public:
	static bool Load();

	static uint32_t ServerID();
	static uint16_t Port();
	static uint32_t MaximalConnections();
	static std::string ServerName();
	static std::string DatabaseString();
};


#endif // _AGENTCONFIG_H_