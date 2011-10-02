#include "download_server.h"
/////////////////////////////////////////////////////////////////////////////////////////////////////

DownloadServer::DownloadServer(uint16_t port, std::size_t thread_count)
	:   m_acceptor     (m_service)
{
	current_position = 0;
	m_thread_count = thread_count;

	boost::asio::ip::tcp::endpoint endpoint (boost::asio::ip::tcp::v4(), port);
	m_acceptor.open(endpoint.protocol());
	m_acceptor.bind(endpoint);
	m_acceptor.listen();

	uint32_t id = 0;
	ClientContext* client = get_next_client(&id);
	m_acceptor.async_accept(client->getSocket(), boost::bind(&DownloadServer::OnAccept, this, boost::asio::placeholders::error, id));
};
/////////////////////////////////////////////////////////////////////////////////////////////////////


ClientContext* DownloadServer::get_next_client(uint32_t *id)
{
	*id = free_client_ident();
	ClientContext *context = new ClientContext(m_service);
	context->setID(*id);
	context->setDisconnectHandler(boost::bind(&DownloadServer::manage_disconnect, this, _1));

	m_client_container.insert(std::pair<uint32_t, ClientContext*>(*id, context));
	return context;
};

uint32_t DownloadServer::free_client_ident()
{
		// check if there is already one free
		if(cleaned.size() != 0)
		{
			uint32_t res = cleaned.front();
			cleaned.erase(cleaned.begin());

			return res;
		}
		// nothing free create a new one if there is enough place
		else
		{
			if(current_position > DownloadConfig::MaximalConnections())
			{
				printl("Server is full, client tried to connect.\n");
				return SERVER_IS_FULL; // there will never so much clients on the server, lol^^
			}
			else
			{
				return current_position++;
			}
		}
};

void DownloadServer::OnAccept(const boost::system::error_code& e, uint32_t id)
{
	// finish & check connection
	printl("Client connected to the server on slot %d", id);
	std::cout << " (" 
		<<  m_client_container[id]->getSocket().remote_endpoint().address().to_string() << ":" 
		<<  m_client_container[id]->getSocket().remote_endpoint().port() << ")" << std::endl;

	uint32_t id_2 = 0;	
	ClientContext* client = get_next_client(&id_2);
	m_acceptor.async_accept(client->getSocket(), boost::bind(&DownloadServer::OnAccept, this, boost::asio::placeholders::error, id_2));

	if(id == SERVER_IS_FULL) {
		delete client;
		return;
	}

	m_client_container[id]->start();
}


void DownloadServer::start()
{
	// Create a pool of threads to run all of the io_services.
	std::vector<boost::shared_ptr<boost::thread> > threads;
	for (std::size_t i = 0; i < m_thread_count; ++i)
	{
		boost::shared_ptr<boost::thread> thread(new boost::thread(
			 boost::bind(&boost::asio::io_service::run, &m_service)));
		threads.push_back(thread);
	}

	// Wait for all threads in the pool to exit.
	for (std::size_t i = 0; i < threads.size(); ++i)
		threads[i]->join();
}

void DownloadServer::manage_disconnect(uint32_t id)
{
	try
	{
		delete m_client_container[id];
		m_client_container.erase(id);

		cleaned.push_back(id);
	}
	catch(...)
	{
		printl("Error on closing client in container.\n");
	}
}