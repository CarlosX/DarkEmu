#include "news_mgr.h"

#include <boost/filesystem.hpp>


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
std::list<News> NewsMgr::m_news_list ;
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

bool NewsMgr::Load()
{
	try
	{
		if( boost::filesystem::exists("news/") )
		{
			boost::filesystem::directory_iterator end ;
			for( boost::filesystem::directory_iterator iter("news/") ; iter != end ; ++iter )
				if ( is_directory( *iter ) )
				{

				}
				else 
				{
					if(iter->path().extension() == ".html")
					{
						//std::cout << iter->path() << std::endl;

						// Get some, without opening a stream
						News news;
						news.Title = iter->path().filename().string();
						news.Title = news.Title.substr(0, news.Title.length() - 5);
						
						boost::posix_time::ptime time 
							= boost::posix_time::from_time_t(boost::filesystem::last_write_time(iter->path()));

						news.Creation = boost::posix_time::to_tm(time);
						
						// get text from stream
						std::ifstream ifs (iter->path().wstring());
						
						 // get pointer to associated buffer object
						  std::filebuf *pbuf = ifs.rdbuf();

						  // get file size using buffer's members
						  unsigned int size = pbuf->pubseekoff (0, std::ios::end, std::ios::in);
						  pbuf->pubseekpos (0, std::ios::in); // back to pos 0

						  // allocate memory to contain file data
						  char* buffer = new char[size];
						   
						  // get file data  
						  pbuf->sgetn (buffer, size);

						  // set in news
						  news.Text = std::string(buffer, size);

						  //close
						  ifs.close();

						  // add to archive
						  m_news_list.push_back(news);

						  // freeeedom
						  delete[] buffer;
					}
				}
		}
		else
		{
			return false;
		}

		return true;
	}
	catch(...)
	{
		return false;
	}
}

std::list<News>* NewsMgr::GetNewest()
{
	return &m_news_list;
}

uint16_t NewsMgr::Count()
{
	return m_news_list.size();
}