#define WIN32_LEAN_AND_MEAN 

#include <iostream>

#ifndef WIN32
#error Please use windows.
#error Linux isn´t supported for now.
#endif

#include <conio.h>
#ifdef WIN32
#include <Windows.h>
#endif

#include "../common/common.h"
#include "../common/global_configs.h"

#include "agent_configs.h"
#include "global_manager.h"

#include <boost/thread.hpp>
#include <boost/bind.hpp>

int main()
{

#ifdef WIN32
	SetConsoleTitle(TEXT("Teemo.SR ~ WorldServer [Windows build]"));
#endif

#ifdef LINUX
	printl("Teemo.SR ~ WorldServer [Linux build]\n\n");
#endif
//////////////////////////////////////////////////////////////////////////////////////////////////////

	if(GlobalConfigs::Load())
	{
		printl("Loading configs success (globals.xml)\n");
	}
	else
	{
		printl("Loading configs failed, in globals.xml\n");
		_getch();
		return 0;
	}

	if(AgentConfigs::Load())
	{
		printl("Loading configs success (agent.xml)\n");
	}
	else
	{
		printl("Loading configs failed, in agent.xml\n");
		_getch();
		return 0;
	}

	printf("================================================================================\n");
	printl("GlobalManager connecting to Globalserver ...\n");

	if(GlobalManager::getSingleton()->Start())
	{
		printl("Authorizise as Gatewayserver ...\n");
		
		boost::thread(boost::bind(&GlobalManager::AuthAs, GlobalManager::getSingleton()));
	}
	else
	{
		printl("Couldn´t connect to Globalserver. Please turn it always at first on.\n");
		_getch();
		return 0;
	}

	Sleep(200);
	printf("================================================================================\n");

	printl("WorldServer is starting. on 6 threads.\n");

///////////////////////////////////////////////////////////////////////////////////////////////////////

	while(_getch() != 27) {
	}	

	// Save & ..
	//_endthreadex(id);

	//// I'm not sure, probably windows should automaticly delete it, but I'm not sure on other systems.
	//delete GlobalManager::getSingleton();

	// Escape pressed
	printl("Server shutting down.\n");

	return 1;
}
