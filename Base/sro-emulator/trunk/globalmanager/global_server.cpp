#include "global_server.h"

#include "../common/global_configs.h"

#include <boost/bind.hpp>
#include <boost/thread.hpp>
#include <boost/shared_ptr.hpp>
////////////////////////////////////////////////////////////////////////////////////////////

Client::Client(boost::asio::io_service& service)
	: Socket(service)
{
	IsConnected = false;
	Buffer = new uint8_t[1024];
}
////////////////////////////////////////////////////////////////////////////////////////////

GlobalServer::GlobalServer(uint16_t port, size_t threads)
	: m_service		(threads),
	  m_thread_size (threads),
	  m_acceptor    (m_service)
{
	m_completed = false;
	m_gateway_disconnected = false;
	m_gateway_server = NULL;
	m_download_server = NULL;

	boost::asio::ip::tcp::endpoint endpoint (boost::asio::ip::tcp::v4(), port);
	m_acceptor.open(endpoint.protocol());
	m_acceptor.bind(endpoint);
	m_acceptor.listen();

	Client* cl = new Client(m_service);
	m_acceptor.async_accept(cl->Socket, boost::bind(&GlobalServer::OnAccept, this, boost::asio::placeholders::error, cl));
}

void GlobalServer::OnAccept(const boost::system::error_code& e, Client* client)
{
	client->HasToDelete = false;
	client->IsConnected = true;
	client->Security.GenerateHandshake(false, false, false); // disabled because it made some troubles.
	while(client->Security.HasPacketToSend())
		client->Socket.send(boost::asio::buffer(client->Security.GetPacketToSend()));

	client->Socket.async_receive
		(boost::asio::buffer(client->Buffer, 1024), boost::bind(&GlobalServer::OnReceive, this, boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred, client));

	// Accept & Create a new client. 
	Client* cl = new Client(m_service);
	m_acceptor.async_accept(cl->Socket, boost::bind(&GlobalServer::OnAccept, this, boost::asio::placeholders::error, cl));
}


void GlobalServer::OnReceive(const boost::system::error_code& e, std::size_t recv, Client* client)
{
	if(!e)
	{
		client->Security.Recv(client->Buffer, recv);

		if(client->Security.HasPacketToRecv())
		{
			PacketContainer packet = client->Security.GetPacketToRecv();

			switch(packet.opcode) {

			// Client IDent
			case 0x2001:
				{

				uint16_t len = packet.data.Read<uint16_t>();
				std::string name = packet.data.Read_Ascii(len);
				uint8_t flag = packet.data.Read<uint8_t>();

				client->Security.ChangeIdentity(name, flag);

				if(client->Security.GetIdentify() == "GatewayServer")
				{
					printl("GatewayServer connected................. OK \n");

					if(m_gateway_server != NULL && m_gateway_server->HasToDelete)
					{
						delete m_gateway_server;
						m_gateway_server = NULL;
					

						// send download informations again.
						StreamUtility stream = StreamUtility();
							stream.Write<uint8_t>(0x01);
							m_download_server->Security.Send(0xD001, stream); // refresh
							m_download_server->Socket.send(boost::asio::buffer(m_download_server->Security.GetPacketToSend()));
					}

					m_gateway_server = client;
				}
				else if(client->Security.GetIdentify() == "DownloadServer")
				{
					printl("DownloadServer connected................ OK \n");

					if(m_download_server != NULL && m_download_server->HasToDelete)
					{
						delete m_download_server;
						m_download_server = NULL;
					}
				}
				else if(client->Security.GetIdentify() == "WorldServer")
				{
					// Wait for the next packet of the world server.
				}

				//Helper Packet (so the client extactly know when security is finished.) // UPDATE: disabled.
					//StreamUtility stream = StreamUtility();
					//stream.Write<uint8_t>(0x01);
					//client->Security.Send(0xCCCC, stream);
					//client->Socket.send(boost::asio::buffer(client->Security.GetPacketToSend()));
				break;
				}

			if(client->Security.GetIdentify() == "WorldServer")
			{
			case 0xC001:
				{
				client->ID      = packet.data.Read<uint32_t>();
				client->Name    = packet.data.Read_Ascii    (packet.data.Read<uint16_t>());
				client->Current = packet.data.Read<uint32_t>();
				client->Maximal = packet.data.Read<uint32_t>();
				client->Port    = packet.data.Read<uint16_t>();
				
				if(m_gateway_server != NULL)
					if(m_gateway_server->IsConnected)
					{
						StreamUtility stream = StreamUtility();
						stream.Write<uint16_t>	(client->Name.length());
						stream.Write_Ascii		(client->Name);
						stream.Write<uint32_t>	(client->ID);
						stream.Write<uint32_t>	(client->Current);
						stream.Write<uint32_t>	(client->Maximal);
						stream.Write<uint8_t>	(client->IsConnected);
						stream.Write<uint16_t>	(client->Socket.remote_endpoint().address().to_string().length());
						stream.Write_Ascii		(client->Socket.remote_endpoint().address().to_string());
						stream.Write<uint16_t>	(client->Port);			
						m_gateway_server->Security.Send(0x100C, stream); // Send gateway server infos about the client.

						while(m_gateway_server->Security.HasPacketToSend())
							m_gateway_server->Socket.send(boost::asio::buffer(m_gateway_server->Security.GetPacketToSend()));
					}

				if(m_gameservers[client->ID] != NULL && m_gameservers[client->ID]->HasToDelete)
				{
					delete m_gameservers[client->ID];
					m_gameservers[client->ID] = client;
				}
				else
				{
					m_gameservers.insert(std::pair<uint32_t, Client*>(client->ID, client));
				}
				
				printl("GameServer connected................. OK \n");
				break;
				}

			// Verify Session pls.
			case 0xC002:
				{
				uint16_t servID  = packet.data.Read<uint16_t>();
				uint16_t sessID  = packet.data.Read<uint16_t>();
				packet.data.SeekRead(4, SeekDirection::Seek_Backward);
				uint32_t session = packet.data.Read<uint32_t>();

				StreamUtility stream = StreamUtility();
					
				// 1. Check -> Vaild server.
				if(servID == client->ID )
				{
					// 2. Check -> Vaild sessionid.
					bool res = false;
					for(std::vector<uint32_t>::iterator it = m_sessions.begin(); it != m_sessions.end(); ++it)
					{
						if(*it == session)						
							res = true;						
					}

					if(res)
					{
						stream.Write<uint8_t>(0x01);
					}
					else
					{
						stream.Write<uint8_t>(0x00);
					}
				}
				else
				{
					stream.Write<uint8_t>(0x00);
				}

				client->Security.Send(0x200C, stream);
				client->Socket.send(boost::asio::buffer(m_gateway_server->Security.GetPacketToSend()));
				break;
				}
			}
			else if(client->Security.GetIdentify() == "GatewayServer")
			{
			// Add session. / Generate new session.
			case 0xC003:
				{
				uint32_t session_full = packet.data.Read<uint32_t>();
				m_sessions.push_back(session_full);
				break;
				}
			}
			else if(client->Security.GetIdentify() == "DownloadServer")
			{
				// DownloadServer informations.
				case 0xC004:
				{
					m_gateway_server->Security.Send(0x400C, packet.data.GetStreamPtr(), packet.data.GetStreamSize(), false, true);
					m_gateway_server->Socket.send(boost::asio::buffer(m_gateway_server->Security.GetPacketToSend()));
					break;
				}
			}

			default:
				{
				printl("Unknown operation received. Ignore for now. \n");
				break;
				}
			}
		}
		else
		{
			// maybe the incoming message was a security packet.
			while(client->Security.HasPacketToSend())
				client->Socket.send(boost::asio::buffer(m_gateway_server->Security.GetPacketToSend()));
		}

		client->Socket.async_receive
			(boost::asio::buffer(client->Buffer, 1024), boost::bind(&GlobalServer::OnReceive, this, boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred, client));
	}
	else
	{
		// Maybe socket disconnect
		if(client->Security.GetIdentify() == "GatewayServer")
		{
			m_gateway_server->IsConnected = false;
			m_gateway_server->HasToDelete = true;
			printl("GatewayServer disconnected, wait for a new connection.\n");
		}
		else if( client->Security.GetIdentify() == "WorldServer")
		{
			m_gameservers[client->ID]->HasToDelete = true;
			m_gameservers[client->ID]->IsConnected = false;
			printl("A WorldServer disconnected, server is check until it reconnects.\n");

			// set to check.
			StreamUtility stream = StreamUtility();
			stream.Write<uint32_t>(client->ID); // server id.
			m_gateway_server->Security.Send(0x600C, stream);
			m_gateway_server->Socket.send(boost::asio::buffer(m_gateway_server->Security.GetPacketToSend()));

		}
		else if(client->Security.GetIdentify() == "DownloadServer")
		{
			m_download_server->HasToDelete = true;
			m_download_server->IsConnected = false;
			printl("DownloadServer disconnected, patching feature disabled.\n");

			// inform gatewayserver
			StreamUtility stream = StreamUtility();
			stream.Write<uint8_t>(0x01);
			m_gateway_server->Security.Send(0x800C, stream);
			m_gateway_server->Socket.send(boost::asio::buffer(m_gateway_server->Security.GetPacketToSend()));
		}
		else
		{
			delete client;
			client = NULL;

			printl("Unknown client disconnected. Removed from memory.\n");
		}
	}		
}


void GlobalServer::start()
{
// Create a pool of threads to run all of the io_services.
  std::vector<boost::shared_ptr<boost::thread> > threads;
  for (std::size_t i = 0; i < m_thread_size; ++i)
  {
    boost::shared_ptr<boost::thread> thread(new boost::thread(
          boost::bind(&boost::asio::io_service::run, &m_service)));
    threads.push_back(thread);
  }

  // Wait for all threads in the pool to exit.
  for (std::size_t i = 0; i < threads.size(); ++i)
    threads[i]->join();
}

void GlobalServer::stop()
{
	m_service.stop();
}
//////////////////////////////////////////////////////////////////////////////////////////////