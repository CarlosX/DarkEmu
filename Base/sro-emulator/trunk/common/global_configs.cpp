#include "global_configs.h"

#include <sstream>
#include <direct.h>
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
uint32_t GlobalConfigs::m_version_oldest = 1;
uint32_t GlobalConfigs::m_version_current = 30;
bool GlobalConfigs::m_use_blowfish = false;
bool GlobalConfigs::m_use_handshake = false;
bool GlobalConfigs::m_use_security_bytes = false;
std::string GlobalConfigs::m_ip_address = "";
uint16_t GlobalConfigs::m_port = 0;

uint32_t GlobalConfigs::Current_Version()
{
	return GlobalConfigs::m_version_current;
};

uint32_t GlobalConfigs::Oldest_Version()
{
	return GlobalConfigs::m_version_oldest;
};

bool GlobalConfigs::UseHandshake()
{
	return GlobalConfigs::m_use_handshake;
};

bool GlobalConfigs::UseBlowfish()
{
	return GlobalConfigs::m_use_blowfish;
};

bool GlobalConfigs::UseSecurityBytes()
{
	return GlobalConfigs::m_use_security_bytes;
};

std::string GlobalConfigs::IPAddress()
{
	return GlobalConfigs::m_ip_address;
};

uint16_t GlobalConfigs::Port()
{
	return GlobalConfigs::m_port;
};

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

bool GlobalConfigs::Load() {

	try {
		GlobalConfigs cf;

		std::ifstream ifs ("configs\\global.xml" , std::ifstream::binary | std::ifstream::in);
		boost::archive::xml_iarchive xml(ifs);
		xml >> boost::serialization::make_nvp("GlobalConfigs", cf);
		return true;
	}
	catch(...) {
		return false;
	}
};

bool GlobalConfigs::Save() {

	std::ofstream oss;
	oss.open("configs\\global.xml", std::ofstream::binary | std::ofstream::out);
	GlobalConfigs* cf = new GlobalConfigs();
	boost::archive::xml_oarchive text(oss);

	text << boost::serialization::make_nvp("GlobalConfigs", cf);
	//text << cf; 
	//std::cout << oss.str() << '\n';

	return true;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////