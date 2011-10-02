#include "patch_mgr.h"

#include <boost/filesystem.hpp>
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

std::multimap<uint32_t, Patchfile> PatchMgr::m_patch_map;
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


bool PatchMgr::Load()
{
	try
	{
		if( boost::filesystem::exists("patches/") )
		{
			boost::filesystem::directory_iterator end ;
			for( boost::filesystem::directory_iterator iter("patches/") ; iter != end ; ++iter )
				if ( !is_directory( *iter ) )
				{
					if(iter->path().extension().string() == ".patch")
					{
						uint32_t patchversion = atoi(iter->path().filename().string().c_str());

						std::ifstream ifs (iter->path().string());

						// read line by line
						Patchfile filesIn[512];

						int pnum = 0;
						while(ifs)
						{
							uint32_t t = 0;
							uint32_t* ptr = &t;
							uint16_t* loword = (uint16_t*)ptr;
							uint16_t* hiword = (uint16_t*)(ptr + 2);
							*loword = patchversion;
							*hiword = pnum;
							
							if(ifs.eof())
								break;

							char curLine[255];
							ifs.getline(curLine, 255);

							char* filename = std::strtok(curLine, "\t");
							char* size	   = std::strtok(NULL, "\t");

							Patchfile p ;
							boost::filesystem::path filep (filename);
							
							p.ID = t;
							p.Filename = filep.filename().string();
							p.Path = filep.parent_path().string();
							
							if(	   string_help_contains(p.Path, "Media") 
								|| string_help_contains(p.Path, "Data")
								|| string_help_contains(p.Path, "Map")
								|| string_help_contains(p.Path, "Music")
								|| string_help_contains(p.Path, "Particles"))

								p.IsPk2Compressed = true;
							else
								p.IsPk2Compressed = false;

							p.Size = atoi(size);

							filesIn[pnum] = p;


							// inc pnum
							pnum++;
						}

						for(int i = 0; i < (pnum ); i++)
						{
							m_patch_map.insert(std::pair<uint32_t, Patchfile>(patchversion, filesIn[i]));
						}
					}
				}
		}
		else
		{
			return false;
		}


		return true;
	}
	catch (...)
	{
		return false;
	}
}

uint16_t PatchMgr::Count()
{
	return m_patch_map.size();
}


bool PatchMgr::string_help_contains(std::string obj, std::string contains)
{
		size_t count; 
        count = obj.find(contains);
 
        if(count != std::string::npos)
                return true;
        else
                return false;
}

std::multimap<uint32_t, Patchfile>* PatchMgr::GetPatches()
{
	return &m_patch_map;
}