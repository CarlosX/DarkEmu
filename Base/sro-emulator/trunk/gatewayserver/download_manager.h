//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#ifndef _DOWNLOAD_MGR_H_
#define _DOWNLOAD_MGR_H_

#include "../common/silkroad_security.h"
#include "../common/common.h"
#include "../common/singleton.h"

#include <boost/filesystem.hpp>
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
struct File // from FileMgr.h in Teemo.SR.Download.
{
	uint32_t				ID;
	std::string				Path;
	std::string				Filename;
	std::size_t				Size;
	boost::filesystem::path FullPath;
	bool					Pk2Compressed;
};

class DownloadManager : public Singleton<DownloadManager>
{
	friend class Singleton<DownloadManager>;

private:
	std::multimap<uint32_t, File> m_file_map;

public:
	DownloadManager();

	void HandlePacket(PacketContainer* packet);
	std::multimap<uint32_t, File> *GetMultimap();
};
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#endif // _DOWNLOAD_MGR_H_