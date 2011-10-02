#ifndef _GLOBAL_SERVER_H_
#define _GLOBAL_SERVER_H_

#include <map>

#include "../common/common.h"
#include "../common/silkroad_security.h"
#include "../common/stream_utility.h"

#include <boost/asio.hpp>
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


class Client
{
public:
	SilkroadSecurity Security;
	boost::asio::ip::tcp::socket Socket;

	bool IsConnected;
	bool HasToDelete;

	uint8_t* Buffer;

	Client(boost::asio::io_service & service);

	// gameserver only
	std::string Name;
	uint32_t ID;
	uint32_t Current;
	uint32_t Maximal;
	uint16_t Port;

private:
	
};

class GlobalServer
{
	private:

	std::size_t m_thread_size;
	boost::asio::io_service m_service;
	boost::asio::ip::tcp::acceptor m_acceptor;


	Client* m_gateway_server;
	Client* m_download_server;
	std::map<uint32_t, Client*> m_gameservers;
	
	std::vector<uint32_t> m_sessions; 

	bool m_completed;
	bool m_gateway_disconnected;

	void OnAccept(const boost::system::error_code& e, Client* client);
	void OnReceive(const boost::system::error_code& e, std::size_t recv, Client* client);

public:
	GlobalServer(uint16_t port, std::size_t service_count);
	
	void start  ();
	void stop   ();
};


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#endif