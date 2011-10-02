#ifndef _SHARED_TYPES_H_
#define _SHARED_TYPES_H_


#ifndef NO_BOOST

	#include <boost/cstdint.hpp>

	using boost::uint64_t;
	using boost::uint32_t;
	using boost::uint16_t;
	using boost::uint8_t;

	using boost::int64_t;
	using boost::int32_t;
	using boost::int16_t;
	using boost::int8_t;

#else


#endif

#endif // _SHARED_TYPES_H_
