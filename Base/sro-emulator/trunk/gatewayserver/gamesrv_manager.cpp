#include "gamesrv_manager.h"
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

GameserverManager::GameserverManager()
{
}


std::map<uint32_t, Gameserver>::iterator GameserverManager::Begin()
{
	return m_gamesrv_map.begin();
}

std::map<uint32_t, Gameserver>::iterator GameserverManager::End  ()
{
	return m_gamesrv_map.end();
}

void GameserverManager::UpdateOrAddServer(Gameserver gsrv)
{
	if(m_gamesrv_map.find(gsrv.ID) == m_gamesrv_map.end())
	{
		m_gamesrv_map.insert(std::pair<uint32_t, Gameserver>(gsrv.ID, gsrv));
	}
	else
	{
		m_gamesrv_map[gsrv.ID] = gsrv;
	}
}

std::map<uint32_t, Gameserver>::iterator GameserverManager::GetGameserver(uint32_t id)
{
	if(m_gamesrv_map.find(id) == m_gamesrv_map.end())
		return NULL;

	return m_gamesrv_map.find(id);
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////