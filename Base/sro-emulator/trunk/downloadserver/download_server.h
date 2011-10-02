#ifndef _GATEWAY_H_
#define _GATEWAY_H_

#include "../common/common.h"

#include "io_service_pool.h"
#include "client_context.h"
#include "download_config.h"

#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/thread.hpp>

#include <map>

//////////////////////////////////////////////////////////////////////////////////////////////////////

#define SERVER_IS_FULL 1337133713

//////////////////////////////////////////////////////////////////////////////////////////////////////

class DownloadServer
{

private:
	boost::asio::io_service m_service;
	boost::asio::ip::tcp::acceptor m_acceptor;

	std::size_t m_thread_count;

	std::map<uint32_t, ClientContext*> m_client_container;
	std::vector<uint32_t> cleaned;

	ClientContext* get_next_client(uint32_t *id);
	uint32_t free_client_ident();

	uint32_t current_position;

	void OnAccept(const boost::system::error_code& e, uint32_t id);


public:
	DownloadServer(uint16_t port, std::size_t thread_count);
	
	void start  ();
	void stop   ();

	void manage_disconnect(uint32_t id);
};

//////////////////////////////////////////////////////////////////////////////////////////////////////
#endif // _GATEWAY_H_