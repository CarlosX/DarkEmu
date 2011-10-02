#include "client_context.h"
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

ClientContext::ClientContext(boost::asio::io_service &service)
	: m_socket(service)
{
	m_message = new uint8_t[4096];
};

//ClientContext::~ClientContext()
//{
//};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void ClientContext::disconnected(uint8_t reason)
{
	if(reason == CLIENT_GOT_KICKED)
	{
		m_socket.close();
		printl("Client got kicked from the server. \n");

	}
	else if(reason == CLIENT_LOST_CONNECTION)
	{
		// todo: say to server that this can be deleted.

		printl("Server lost connection to a client. (restart or quit is possible)\n");
	}
	//else if(reason == CLIENT_QUIT_OR_RESTARTED) // client marked as lost connection after we sent the quit/restart packet
	//{
	//	m_socket.close();
	//}
	else if( reason == CLIENT_GOT_KICKED_DUE_CRITICLE_SERVER_ERROR)
	{
		m_socket.close();
	}

	m_dc_handler(m_id); // call the disconnect handler to delete this clientcontext from memory.
};

void ClientContext::message_recv(const boost::system::error_code &ec, std::size_t recv)
{
	if(!m_socket.is_open())
	{
		disconnected(CLIENT_LOST_CONNECTION);
		return;
	}

	if(!ec)
	{
		printl("Client sent message to server. \n");
		//////////////////////////////////////////////////////////////////////////////////////

		m_security.Recv(m_message, recv);
		PacketContainer pc;

		if(m_security.HasPacketToRecv()) // else it was a security packet.
		{
			pc = m_security.GetPacketToRecv();		
			HandleMessage(&pc);

			SendQueue();
		}
		else
		{
			SendQueue();
		}

		// after packet handled, receive a new one.
		m_socket.async_receive(
			boost::asio::buffer(m_message, 4096), 
			boost::bind(&ClientContext::message_recv, this, boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
	}
	else
	{
		if(recv == 0)
		{
			disconnected(CLIENT_LOST_CONNECTION);
			return;
		}

		printl("");
		std::cout << "Error occured: " << ec.message() << std::endl;
	}

};

void ClientContext::start()
{
	m_security.GenerateHandshake(
		GlobalConfigs::UseBlowfish      (),
		GlobalConfigs::UseSecurityBytes (),
		GlobalConfigs::UseHandshake     ()      );

	SendQueue();

	m_socket.async_receive(
		boost::asio::buffer(m_message, 4096), 
		boost::bind(&ClientContext::message_recv, this, boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred));
}

void ClientContext::SendQueue()
{
	while(m_security.HasPacketToSend())
	{
		m_socket.send(boost::asio::buffer(m_security.GetPacketToSend()));
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
boost::asio::ip::tcp::socket& ClientContext::getSocket()
{
	return m_socket;
};


void ClientContext::setID(uint32_t id)
{
	m_id = id;
};

void ClientContext::setDisconnectHandler(boost::function<void (uint32_t)> handler)
{
	m_dc_handler = handler;
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////