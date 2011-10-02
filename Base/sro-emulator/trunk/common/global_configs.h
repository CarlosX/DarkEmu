#ifndef _GLOBALCONFIG_H_
#define _GLOBALCONFIG_H_
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#include <boost/archive/xml_iarchive.hpp>
#include <boost/archive/xml_oarchive.hpp>
#include <boost/archive/text_oarchive.hpp>
#include <fstream>

#include "shared_types.h"
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Contains informations, which are needed from all servers.
class GlobalConfigs {

private:
	friend class boost::serialization::access;

    template<class Archive> void serialize(Archive & ar, const unsigned int version) {
        ar & boost::serialization::make_nvp("OldestVersion", GlobalConfigs::m_version_oldest);
        ar & boost::serialization::make_nvp("CurrnetVersion", GlobalConfigs::m_version_current);
		ar & boost::serialization::make_nvp("UseHandshake", GlobalConfigs::m_use_handshake);
		ar & boost::serialization::make_nvp("UseBlowfish", GlobalConfigs::m_use_blowfish);
		ar & boost::serialization::make_nvp("UseSecurityBytes", GlobalConfigs::m_use_security_bytes);
		ar & boost::serialization::make_nvp("IPAddress", GlobalConfigs::m_ip_address);
		ar & boost::serialization::make_nvp("Port", GlobalConfigs::m_port);
    }

	static uint32_t m_version_oldest;
	static uint32_t m_version_current;

	static bool m_use_handshake;
	static bool m_use_security_bytes;
	static bool m_use_blowfish;

	static std::string  m_ip_address;
	static uint16_t		m_port;

public:
	static bool Load();
	static bool Save();

	static uint32_t Current_Version();
	static uint32_t Oldest_Version();
	static bool UseHandshake();
	static bool UseBlowfish();
	static bool UseSecurityBytes();
	static std::string IPAddress();
	static uint16_t Port();
};


#endif // _GLOBALCONFIG_H_