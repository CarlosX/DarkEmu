#ifndef _GLOBALMANAGER_H_
#define _GLOBALMANAGER_H_

#include "../common/common.h"
#include "../common/singleton.h"
#include "../common/silkroad_security.h"

#include "agent_configs.h"
#include "Status.h"

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

	void OnReceive			(const boost::system::error_code& ec, std::size_t recv);

public:
	GlobalManager();

	bool Start();
	void Stop ();

	void     AuthAs				();			// auth to the globalmanager
	bool	 VerifySession		(uint32_t session);		// verify session.
	void	 UpdateServer		();						// update capacity
};


#endif // _GLOBALMANAGER_H_