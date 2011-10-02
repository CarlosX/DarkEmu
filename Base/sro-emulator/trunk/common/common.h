#ifndef _COMMON_H_
#define _COMMON_H_

#include "shared_types.h"
#include <boost/date_time.hpp>
#include <boost/asio.hpp>

// Logging
#define printl(msg) \
	printf("%s", boost::posix_time::to_simple_string(boost::posix_time::second_clock::local_time()).c_str()); \
	printf("\t"); \
	printf(msg);

// Win32 stuff


#endif //_COMMON_H_