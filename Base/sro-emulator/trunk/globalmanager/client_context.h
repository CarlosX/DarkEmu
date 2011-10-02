#ifndef _CLIENT_CONTEXT_H_
#define _CLIENT_CONTEXT_H_

// define some disconnect reasons.
#define CLIENT_LOST_CONNECTION						0x01
#define CLIENT_QUIT_OR_RESTARTED					0x02
#define CLIENT_GOT_KICKED							0x03  // got kicked because of hacks/bot etc..
#define CLIENT_GOT_KICKED_DUE_CRITICLE_SERVER_ERROR 0x04  // got kicked because server has to restart and save this context.

// common
#include "../common/shared_types.h"
#include "../common/silkroad_security.h"
#include "../common/global_configs.h"
#include "../common/stream_utility.h"
#include "../common/operations.h"
// boost includes
#include <boost/asio.hpp>
#include <boost/bind.hpp>

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class ClientContext
{
private:
	SilkroadSecurity m_security;

	uint8_t* m_message;

	uint32_t m_id;

	boost::asio::ip::tcp::socket m_socket;
	boost::function<void (uint32_t)> m_dc_handler;

	
public:
	ClientContext(boost::asio::io_service& service);             // invoked after client connect to the server.
	//~ClientContext();											// invoked after disconnect() called.


	boost::asio::ip::tcp::socket& getSocket();
	void setID(uint32_t id);
	void setDisconnectHandler(boost::function<void (uint32_t)> handler);

	void start();

	void message_recv(const boost::system::error_code &ec, std::size_t recv);		// invoked after client sent a message.
	void SendQueue();

	void disconnected(uint8_t reason);							// invoked after client disconnected, 0x01 -> lost_connection, 0x02 -> client_quit_or_restart, 0x03 -> client_got_kicked


private:

	// methods which aren´t in client_context.cpp
	void HandleMessage(PacketContainer* packet);
};



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#endif

// the whole client_context  class is splitted to more than 1 *.cpp files