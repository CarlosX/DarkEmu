#include "global_manager.h"
#include "../common/global_configs.h"
#include "FileMgr.h"

#include <boost/bind.hpp>
//////////////////////////////////////////////////////////////////////////////////////////////////////


GlobalManager::GlobalManager()
	: m_socket(m_service)
{
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
	m_security.ChangeIdentity("DownloadServer", 0x00);

	uint32_t recv = m_socket.receive(boost::asio::buffer(m_buffer, 1024));
	m_security.Recv(m_buffer, recv);
	
	//BTW: this won't work enabled with handshake.
	while(m_security.HasPacketToSend()) // implement 0x2001.
		m_socket.send(boost::asio::buffer(m_security.GetPacketToSend()));

	// inform gatewayserver about the Downloads
	StreamUtility stream = StreamUtility();
	std::multimap<uint32_t, File>::iterator iter;
	for(iter = FileMgr::getSingleton()->GetMapPtr()->begin(); iter != FileMgr::getSingleton()->GetMapPtr()->end(); ++iter)
	{
		stream.Write<uint8_t>	 (0x01);			// new file
		stream.Write<uint32_t>	 (iter->second.ID);
		stream.Write<uint16_t>	 (iter->second.Filename.length());
		stream.Write_Ascii       (iter->second.Filename);
		stream.Write<uint16_t>	 (iter->second.Path.length());
		stream.Write_Ascii		 (iter->second.Path);
		stream.Write<uint32_t>	 (iter->second.Size);
		stream.Write<uint8_t>	 (iter->second.Pk2Compressed);
	}
	stream.Write<uint8_t>	 (0x00);
	m_security.Send(0xC004, stream, false, true); // this packet could be large. so let's use Joymax 0x600D feature.
	m_socket.send(boost::asio::buffer(m_security.GetPacketToSend()));
	printl("GlobalManager successed.\n");

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

	if(m_security.HasPacketToRecv())
	{
		PacketContainer packet = m_security.GetPacketToRecv();

		switch(packet.opcode)
		{
		case 0xD001:
			{
				StreamUtility stream = StreamUtility();
				std::multimap<uint32_t, File>::iterator iter;
				for(iter = FileMgr::getSingleton()->GetMapPtr()->begin(); iter != FileMgr::getSingleton()->GetMapPtr()->end(); ++iter)
				{
					stream.Write<uint8_t>	 (0x01);			// new file
					stream.Write<uint32_t>	 (iter->second.ID);
					stream.Write<uint16_t>	 (iter->second.Filename.length());
					stream.Write_Ascii       (iter->second.Filename);
					stream.Write<uint16_t>	 (iter->second.Path.length());
					stream.Write_Ascii		 (iter->second.Path);
					stream.Write<uint32_t>	 (iter->second.Size);
					stream.Write<uint8_t>	 (iter->second.Pk2Compressed);
				}
				stream.Write<uint8_t>	 (0x00);
				m_security.Send(0xC004, stream, false, true); // this packet could be large. so let's use Joymax 0x600D feature.
				m_socket.send(boost::asio::buffer(m_security.GetPacketToSend()));
				break;
			}
		}
	}
}