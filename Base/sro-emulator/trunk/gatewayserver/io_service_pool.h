#ifndef _IOSERVICE_POOL_H
#define _IOSERVICE_POOL_H

#include "../common/common.h"

#include <boost/asio.hpp>
#include <boost/shared_ptr.hpp>
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class IOServicePool 
{
	
private:
	typedef boost::shared_ptr<boost::asio::io_service> io_service_ptr;
	typedef boost::shared_ptr<boost::asio::io_service::work> work_ptr;

	std::vector<io_service_ptr> services;
	std::vector<work_ptr>		worker  ;

	std::size_t get_next_service;


public:
	explicit IOServicePool(std::size_t size);

	void start (); // start all io_service`s
	void stop  (); // stop  all io_service´s

	boost::asio::io_service& get_free_service();
};


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#endif // _IOSERVICE_POOL_H