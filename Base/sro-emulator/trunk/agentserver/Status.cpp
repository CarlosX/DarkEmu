#include "Status.h"

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

uint32_t Status::m_current_users = 0;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

uint32_t Status::CurrentUsers()
{
	return m_current_users;
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////