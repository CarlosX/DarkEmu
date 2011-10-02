#include "download_config.h"

#include <sstream>
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
uint32_t	DownloadConfig::m_max_connections		= 0;
uint32_t	DownloadConfig::m_max_speed				= 0;
uint16_t	DownloadConfig::m_port					= 0;
uint32_t    DownloadConfig::m_buffer_size           = 0;

uint16_t DownloadConfig::DownloadPort()
{
	return DownloadConfig::m_port;
};

uint32_t DownloadConfig::MaximalConnections()
{
	return DownloadConfig::m_max_connections;
};

uint32_t DownloadConfig::MaximalSpeed()
{
	return DownloadConfig::m_max_speed;
};

uint32_t DownloadConfig::BufferSize()
{
	return DownloadConfig::m_buffer_size;
};
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

bool DownloadConfig::Load() {

	try {
		DownloadConfig cf;

		std::ifstream ifs ("configs\\download.xml" , std::ifstream::binary | std::ifstream::in);
		boost::archive::xml_iarchive xml(ifs);
		xml >> boost::serialization::make_nvp("DownloadConfig", cf);
		return true;
	}
	catch(...) {
		return false;
	}
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////