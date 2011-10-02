#include "../common/common.h"
#include "../common/singleton.h"

#include <map>

#include <boost/filesystem.hpp>

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#ifndef _FILE_MGR_H_
#define _FILE_MGR_H_
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
struct File
{
	uint32_t				ID;
	std::string				Path;
	std::string				Filename;
	std::size_t				Size;
	boost::filesystem::path FullPath;
	bool					Pk2Compressed;
};
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
class FileMgr : public Singleton<FileMgr>
{
	friend class Singleton<FileMgr>;

private:
	std::multimap<uint32_t, File> m_file_map; // version, container for files.

public:
	FileMgr	 ();
	bool Load();
	bool Load(boost::filesystem::path path, uint32_t version, uint16_t loop);

	File* GetFile(uint32_t id, bool& error); // error = true if file wasn´t found.
	static bool string_help_contains(std::string obj, std::string contains);

	std::multimap<uint32_t, File>* GetMapPtr();
};
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#endif //_FILE_MGR_H_