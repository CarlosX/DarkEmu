#ifndef _GLOBALMANAGER_H_
#define _GLOBALMANAGER_H_

#include "../common/common.h"
#include "../common/singleton.h"
#include "../common/silkroad_security.h"

#include "gamesrv_manager.h"

#include <boost/asio.hpp>
//////////////////////////////////////////////////////////////////////////////////////////////////////

// should be static.
class GlobalManager : public Singleton<GlobalManager>
{
	friend class Singleton<GlobalManager>;

private:
	boost::asio::io_service			m_service;
	boost::asio::ip::tcp::socket	m_socket;
	bool							m_init;
	SilkroadSecurity				m_security;
	uint8_t*						m_buffer;
	GameserverManager				m_manager;
	uint16_t						m_next_session;
	std::vector<std::string>		m_logged_users;

	void OnReceive			(const boost::system::error_code& ec, std::size_t recv);

public:
	GlobalManager();
	bool Start();
	void Stop ();

	//std::map<uint32_t, Gameserver>::iterator GetServerIterator();

	void	 AuthAs				();									// auth to the globalmanager
	uint32_t AddSession			(uint32_t servID);					// add session.
	void	 UserAuthComplete   (std::string username);				// add user to globalmanager, it implement the user already connected "feature".
	bool	 IsUserConnected    (std::string username);             // check whether a user is already connected.
};


#endif // _GLOBALMANAGER_H_