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

#include "../common/global_configs.h"
#include "../common/database.h"
#include "../common/common.h"

#include "gateway_config.h"
#include "gateway_server.h"
#include "news_mgr.h"
#include "patch_mgr.h"
#include "global_manager.h"

#include <boost/thread.hpp>

int main()
{

#ifdef WIN32
	SetConsoleTitle(TEXT("Teemo.SR ~ GatewayServer [Windows build]"));
#endif

#ifdef LINUX
	printl("Teemo.SR ~ GatewayServer [Linux build]\n\n");
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

	if(GatewayConfig::Load())
	{
		printl("Loading configs success (gateway.xml)\n");
	}
	else
	{
		printl("Loading configs failed, in gateway.xml\n");
		_getch();
		return 0;
	}

	printf("================================================================================\n");

	if(NewsMgr::Load())
	{
		printl("Loading news success. (");
		std::cout << NewsMgr::Count() << " news loaded. )" << std::endl;;
	}
	else
	{
		printl("Loading news failed. \n");
		_getch();
		return 0;
	}
	
	printf("================================================================================\n");
	printl("Initialize the MySQL server.\n");

	Database::getSingleton   (GatewayConfig::DatabaseHost(),
							  GatewayConfig::Database(),
							  GatewayConfig::Username(),
							  GatewayConfig::Password());

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

	printf("================================================================================\n");

	Sleep(100); // remove some async console output problems.
	GatewayServer gateway_server (GatewayConfig::GatewayPort(), 6); 

	printl("Gatewayserver is starting. on 6 threads.\n");
	boost::thread(boost::bind(&GatewayServer::start, &gateway_server));

///////////////////////////////////////////////////////////////////////////////////////////////////////

	while(_getch() != 27) {
	}	
	// Escape pressed
	printl("Server shutting down.\n");

	return 1;
}
