#include "FileMgr.h"
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
FileMgr::FileMgr()
{
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
bool FileMgr::Load()
{
	try
	{
		if(boost::filesystem::exists("files/"))
		{
			boost::filesystem::directory_iterator end ;
			for( boost::filesystem::directory_iterator iter("files/") ; iter != end ; ++iter )
				if ( is_directory( *iter ) )
				{
					std::string s = iter->path().filename().string();
					if(iter->path().filename().string() != ".svn") // ignore folder's which was created by SVN 
					{
						uint32_t patchversion = std::atoi(iter->path().filename().string().c_str());

						if(!FileMgr::Load(iter->path(), patchversion, 0))
							return false;
					}
				}
		}
	}
	catch (...)
	{
		return false;
	}

	return true;
}


bool FileMgr::Load(boost::filesystem::path path, uint32_t version, uint16_t loop)
{
	try
	{
		uint16_t file_num = loop;

		if( boost::filesystem::exists(path) )
		{
			boost::filesystem::directory_iterator end ;
			for( boost::filesystem::directory_iterator iter(path) ; iter != end ; ++iter )
				if ( is_directory( *iter ) )
				{
					if(iter->path().filename().string() != ".svn")
						if(!FileMgr::Load(iter->path(), version, file_num))
							return false;
				}
				else 
				{
					// create fileid, by setting loword and hiword.. really unsafe, but all pointers will deleted.
					uint32_t zero = 0;
					uint32_t* fileID = &zero;
					uint16_t* fileVer = (uint16_t*)&zero;  // HIWORD
					uint16_t* fileNum = fileVer + 1;       // LOWORD

					*fileVer = static_cast<uint16_t>(version);
					*fileNum = static_cast<uint16_t>(file_num);


					File f ;
					f.ID		= *fileID;
					f.FullPath  = iter->path();
					f.Filename  = iter->path().filename().string();
					
					std::ifstream ifs (f.FullPath.string(), std::ios::binary );
					ifs.seekg (0, std::ios::end);
					f.Size = ifs.tellg();
					ifs.close();

					std::string sversion;
					std::ostringstream ss;
					ss << version;
					sversion = ss.str();

					if(iter->path().parent_path().filename().string() == sversion)
					{
						f.Path			= "";
						f.Pk2Compressed = false;
					}
					else
					{
						uint32_t  endOfVersion  = path.string().find(sversion);
						uint32_t  calcSize		= endOfVersion + sversion.length() + 1;
						uint32_t  calcFileSize  = iter->path().filename().string().length();
						f.Path	= iter->path().string().substr(calcSize, iter->path().string().length() - calcSize - calcFileSize); 

						if(	   string_help_contains(f.Path, "Media") 
							|| string_help_contains(f.Path, "Data")
							|| string_help_contains(f.Path, "Map")
							|| string_help_contains(f.Path, "Music")
							|| string_help_contains(f.Path, "Particles"))

								f.Pk2Compressed = true;
							else
								f.Pk2Compressed = false;
					}

					m_file_map.insert(std::pair<uint32_t, File>(version, f));
					file_num++;
				}
		}
	}
	catch (...)
	{
		return false;
	}

	return true;
}


File* FileMgr::GetFile(uint32_t id, bool& error) // yeah I know multimap performance isn't really good.
{
	error = false;

	//uint32_t test = HIWORD(id);    // for debug.
	//uint32_t test2 = LOWORD(id);

	if(m_file_map.count(static_cast<uint32_t>(LOWORD(id))) >= 1) // check version.
	{
		std::multimap<uint32_t, File>::iterator iter;
		for(iter = m_file_map.begin(); iter != m_file_map.end(); ++iter)
		{
			if(iter->first == LOWORD(id))
				if(iter->second.ID == id)
					return &iter->second;
		}

		error = true;
		return NULL;
	}
	else
	{
		error = true;
		return NULL;
	}
}

bool FileMgr::string_help_contains(std::string obj, std::string contains)
{
		size_t count; 
        count = obj.find(contains);
 
        if(count != std::string::npos)
                return true;
        else
                return false;
}

std::multimap<uint32_t, File>* FileMgr::GetMapPtr()
{
	return &m_file_map;
}