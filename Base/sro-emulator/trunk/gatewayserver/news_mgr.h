#ifndef _NEWS_MGR_H_
#define _NEWS_MGR_H_

#include "../common/common.h"
#include <list>

#include <boost/date_time.hpp>
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct News
{
	std::string Title;
	std::string Text;
	
	std::tm Creation;
};
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

class NewsMgr
{
private:
	static std::list<News> m_news_list;

public:
	static bool Load();
	static std::list<News>* GetNewest();

	static uint16_t Count();
};


#endif // _NEWS_MGR_H_