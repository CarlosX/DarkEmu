#ifndef _DOWNLOADCONFIG_H_
#define _DOWNLOADCONFIG_H_
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#include <boost/archive/xml_iarchive.hpp>
#include <boost/archive/xml_oarchive.hpp>
#include <fstream>

#include "../common/shared_types.h"
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Contains informations, which are needed from all servers.
class DownloadConfig {

private:
	friend class boost::serialization::access;

    template<class Archive> void serialize(Archive & ar, const unsigned int version) {

		ar & boost::serialization::make_nvp("Port",				     DownloadConfig::m_port);
		ar & boost::serialization::make_nvp("Maximal_Speed",		 DownloadConfig::m_max_speed);
		ar & boost::serialization::make_nvp("Maximal_Connections",   DownloadConfig::m_max_connections);
		ar & boost::serialization::make_nvp("Buffer_Size",			 DownloadConfig::m_buffer_size);
    }
	
	static uint32_t m_max_speed;
	static uint32_t m_max_connections;
	static uint32_t m_buffer_size;

	static uint16_t m_port;

public:
	static bool Load();

	static uint16_t DownloadPort();
	static uint32_t MaximalConnections();
	static uint32_t MaximalSpeed();
	static uint32_t BufferSize();
};


#endif // _DOWNLOADCONFIG_H_