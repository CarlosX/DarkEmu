#include "global_manager.h"
#include "download_manager.h"

#include "../common/global_configs.h"

#include <boost/bind.hpp>
//////////////////////////////////////////////////////////////////////////////////////////////////////


GlobalManager::GlobalManager()
	: m_socket(m_service)
{
	m_next_session = 0;
	m_init = false;
	m_buffer = new uint8_t[1024];
}
//////////////////////////////////////////////////////////////////////////////////////////////////////

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
	m_security.ChangeIdentity("GatewayServer", 0x00);

	uint32_t recv = m_socket.receive(boost::asio::buffer(m_buffer, 1024));
	m_security.Recv(m_buffer, recv);
	
	//BTW: this won't work enabled with handshake.
	while(m_security.HasPacketToSend()) // implement 0x2001.
		m_socket.send(boost::asio::buffer(m_security.GetPacketToSend()));
	
	printl("GlobalManager was successfully. \n");
	// Auth completed now we can receive.
	//m_socket.async_receive(boost::asio::buffer(m_buffer,1024) , boost::bind(&GlobalManager::OnReceive, this, boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred)); 

	boost::system::error_code ec;
	while (true)
	{
		recv = m_socket.receive(boost::asio::buffer(m_buffer, 1024));

		if(recv >= 6)
			OnReceive(ec, recv);
	}
}

//////////////////////////////////////////////////////////////////////////////////////////////////////

void GlobalManager::OnReceive(const boost::system::error_code& ec, std::size_t recv)
{
	m_security.Recv(m_buffer, recv);
	PacketContainer packet;

	if(m_security.HasPacketToRecv())
	{
		packet = m_security.GetPacketToRecv();

		switch (packet.opcode) {

		case 0x100C:
			{
			Gameserver game;
			game.Name = packet.data.Read_Ascii			(packet.data.Read<uint16_t>());
			game.ID   = packet.data.Read<uint32_t>		();
			game.Current = packet.data.Read<uint32_t>	();
			game.Maximal = packet.data.Read<uint32_t>	();
			game.Status  = packet.data.Read<uint8_t>	();
			game.IPAddress = packet.data.Read_Ascii		(packet.data.Read<uint16_t>());
			game.Port      = packet.data.Read<uint16_t> ();

			GameserverManager::getSingleton()->UpdateOrAddServer(game);
		
			printl("Gameserver is back online or a new one connected.\n");
			break;
			}

		case 0x400C:
			{
			printl("Download Service enabled.\n");		
			DownloadManager::getSingleton()->HandlePacket(&packet);

			break;
			}

		// User disconnected.
		case 0x500C:
			GameserverManager::getSingleton()->GetGameserver(packet.data.Read<uint32_t>())->second.Current--; // decrease .. 			
			std::string name = packet.data.Read_Ascii(packet.data.Read<uint16_t>());

			for(std::vector<std::string>::iterator iter = m_logged_users.begin(); iter != m_logged_users.end(); ++iter)
			{
				if((*iter) == name)
				{
					m_logged_users.erase(iter); // I know that a swap/pop_back delete is much faster, but it's not thread safe.
				}
			}
			break;

		// Gameserver disconnected.
		case 0x600C:
			GameserverManager::getSingleton()->GetGameserver(packet.data.Read<uint32_t>())->second.Status = 0x00;
			break;

		default:
			printl("GlobalManager sent a unknown message, maybe outdated?\n");
			break;
		}
	}
}

uint32_t GlobalManager::AddSession(uint32_t servID)
{
	uint32_t session_full = 0;
	*(uint16_t*)&session_full = m_next_session++;// HIWORD
	*(uint16_t*)(&session_full + 2) = servID;    // LOWORD

	StreamUtility stream = StreamUtility();
	stream.Write<uint32_t>(session_full);
	m_security.Send(0xC003, stream);
	m_socket.send(boost::asio::buffer(m_security.GetPacketToSend()));

	return session_full;
}

void GlobalManager::UserAuthComplete(std::string username)
{
	m_logged_users.push_back(username);
}

bool GlobalManager::IsUserConnected(std::string username)
{
	for(std::vector<std::string>::iterator iter = m_logged_users.begin(); iter != m_logged_users.end(); ++iter)
	{
		if(*iter == username)
			return true;
	}

	return false;
}
