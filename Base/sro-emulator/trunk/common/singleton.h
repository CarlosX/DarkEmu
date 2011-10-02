#ifndef _SINGLETON_H_
#define _SINGLETON_H_

//////////////////////////////////////////////////////////////////////////////

template <typename target>
class Singleton 
{
public:
	static target* getSingleton		()
	{
		if (m_instance == NULL)
			m_instance = new target();

		return m_instance;
	}

	static void Singleton::destroySingleton()
	{
		delete m_instance;
	}

private:
	static target* m_instance ;
};

template <typename target> target* Singleton<target>::m_instance = NULL;
//////////////////////////////////////////////////////////////////////////////

#endif // _SINGLETON_H_