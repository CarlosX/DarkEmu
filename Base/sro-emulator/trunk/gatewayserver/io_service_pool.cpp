#include "io_service_pool.h"

#include <boost/thread.hpp>
//////////////////////////////////////////////////////////////////////////////////////////////////////

IOServicePool::IOServicePool(std::size_t size)
	: get_next_service(0)
{
	  if (size == 0)
    throw std::runtime_error("io_service_pool size is 0");

  // Give all the io_services work to do so that their run() functions will not
  // exit until they are explicitly stopped.
  for (std::size_t i = 0; i < size; ++i)
  {
		io_service_ptr io_service(new boost::asio::io_service);
		work_ptr work(new boost::asio::io_service::work(*io_service));
		services.push_back(io_service);
		worker.push_back(work);
  }
}

void IOServicePool::start()
{
	  // Create a pool of threads to run all of the io_services.
	  std::vector<boost::shared_ptr<boost::thread> > threads;
	  for (std::size_t i = 0; i < services.size(); ++i)
	  {
		boost::shared_ptr<boost::thread> thread(new boost::thread(
			  boost::bind(&boost::asio::io_service::run, services[i])));
		threads.push_back(thread);
	  }

	  // Wait for all threads in the pool to exit.
	  for (std::size_t i = 0; i < threads.size(); ++i)
			threads[i]->join();
}

void IOServicePool::stop()
{
	  // Explicitly stop all io_services.
	  for (std::size_t i = 0; i < services.size(); ++i)
		services[i]->stop();
}

boost::asio::io_service& IOServicePool::get_free_service()
{
	  // Use a round-robin scheme to choose the next io_service to use.
	  boost::asio::io_service& io_service = *services[get_next_service];
	  ++get_next_service;
	  if (get_next_service == services.size())
		get_next_service = 0;

	  return io_service;
}