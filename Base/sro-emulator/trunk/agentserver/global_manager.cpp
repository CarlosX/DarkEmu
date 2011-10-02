#include "global_manager.h"
#include "../common/global_configs.h"

#include <boost/bind.hpp>
//////////////////////////////////////////////////////////////////////////////////////////////////////



//////////////////////////////////////////////////////////////////////////////////////////////////////

GlobalManager::GlobalManager()
	: m_socket (m_service)
{
	m_buffer = new uint8_t[1024];
	m_init = false;
}

bool GlobalManager::Start()
{
	if(!m_init)
	{
		boost::asio::ip::tcp::endpoint endpoint (boost::asio::ip::address::from_string(GlobalConfigs::IPAddress()), GlobalConfigs::Port());	
		m_socket.connect(endpoint);

		if(!m_socket.is_open())
		{
			printl("Couldn't connect to the GlobalManager. Server instable for now.\n");
			return false;
		}
	}

	m_service.run();
	m_init = true;

	return true;
}

void GlobalManager::Stop()
{
	m_service.stop();
}

void GlobalManager::AuthAs()
{
	// Do SilkroadSecurity.
	m_security.ChangeIdentity("WorldServer", 0x00);

	uint32_t recv = m_socket.receive(boost::asio::buffer(m_buffer, 1024));
	m_security.Recv(m_buffer, recv);
	
	//BTW: this won't work enabled with handshake.
	while(m_security.HasPacketToSend()) // implement 0x2001.
		m_socket.send(boost::asio::buffer(m_security.GetPacketToSend()));

	StreamUtility stream    = StreamUtility();
	stream.Write<uint32_t>	(AgentConfigs::ServerID());
	stream.Write<uint16_t>	(AgentConfigs::ServerName().length());
	stream.Write_Ascii		(AgentConfigs::ServerName().c_str());
	stream.Write<uint32_t>	(Status::CurrentUsers());
	stream.Write<uint32_t>	(AgentConfigs::MaximalConnections());
	stream.Write<uint16_t>	(AgentConfigs::Port());
	m_security.Send			(0xC001, stream);
	m_socket.send			(boost::asio::buffer(m_security.GetPacketToSend()));
	

	printl("GlobalManager was successfully. \n");
	// Auth completed now we can receive. (not needed)
	//m_socket.async_receive(boost::asio::buffer(m_buffer, 1024), boost::bind(&GlobalManager::OnReceive, boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred)); 

	//return 1; // todo: add some try/catch blocks to check the connection.
}

//////////////////////////////////////////////////////////////////////////////////////////////////////

void GlobalManager::OnReceive(const boost::system::error_code& ec, std::size_t recv)
{
	// not implemented yet  / not needed.
	//printl("[GlobalMgr] Unknown message received. \n");
	//m_security.Recv(m_buffer, recv);
	//PacketContainer packet = m_security.GetPacketToRecv();

	//m_socket.async_receive(boost::asio::buffer(m_buffer,1024) , boost::bind(&GlobalManager::OnReceive, boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred)); 
}

void GlobalManager::UpdateServer()
{
	StreamUtility stream = StreamUtility();
	stream.Write<uint32_t>	(AgentConfigs::ServerID());
	stream.Write<uint16_t>	(AgentConfigs::ServerName().length());
	stream.Write_Ascii		(AgentConfigs::ServerName().c_str());
	stream.Write<uint32_t>	(Status::CurrentUsers());
	stream.Write<uint32_t>	(AgentConfigs::MaximalConnections());
	stream.Write<uint16_t>	(AgentConfigs::Port());
	m_security.Send			(0xC001, stream);
	m_socket.send			(boost::asio::buffer(m_security.GetPacketToSend()));
}

bool GlobalManager::VerifySession(uint32_t session)
{
	StreamUtility stream = StreamUtility();
	stream.Write<uint32_t>	(session);
	m_security.Send			(0xC002, stream);
	m_socket.send			(boost::asio::buffer(m_security.GetPacketToSend()));

	uint32_t recv = m_socket.receive(boost::asio::buffer(m_buffer, 1024));
	m_security.Recv(m_buffer, recv);

	bool result = m_security.GetPacketToRecv().data.Read<uint8_t>();
	return result;
}