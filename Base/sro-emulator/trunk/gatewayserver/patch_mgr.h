#ifndef _PATCHMGR_H_
#define _PATCHMGR_H_

#include "../common/common.h"

#include <iostream>
#include <map>
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
struct Patchfile
{
	uint32_t ID;
	std::string Filename;
	std::string Path;	
	std::size_t Size;

	bool IsPk2Compressed;
};

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


class PatchMgr
{
public:
	
	static bool Load();

	static uint16_t Count();
	static std::multimap<uint32_t, Patchfile>* GetPatches();

private:
	static std::multimap<uint32_t, Patchfile> m_patch_map;
	static bool string_help_contains(std::string obj, std::string contains);
};

#endif // _PATCHMGR_H_