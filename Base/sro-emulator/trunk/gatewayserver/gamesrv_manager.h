#ifndef _GAMESERVER_MANAGER_H_
#define _GAMESERVER_MANAGER_H_

#include "../common/common.h"
#include "../common/singleton.h"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
struct Gameserver
{
	uint32_t	ID;
	std::string Name;

	std::string IPAddress;
	uint16_t    Port;

	uint32_t	Current;	// Current connections.
	uint32_t	Maximal;	// Maximal allowed connections.
	uint8_t		Status;		// 0 = Check, 1 = Online
};
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
class GameserverManager : public Singleton<GameserverManager>
{
	friend class Singleton<GameserverManager>;

private:
	std::map<uint32_t, Gameserver> m_gamesrv_map;

public:
	GameserverManager();
	
	std::map<uint32_t, Gameserver>::iterator Begin();
	std::map<uint32_t, Gameserver>::iterator End  ();

	std::map<uint32_t, Gameserver>::iterator		GetGameserver				    (uint32_t id);
	void											UpdateOrAddServer				(Gameserver gsrv);
};

#endif //_GAMESERVER_MANAGER_H_