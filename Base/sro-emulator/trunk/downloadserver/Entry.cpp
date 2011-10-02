#define WIN32_LEAN_AND_MEAN

#include <iostream>
#include <conio.h>

#ifdef WIN32
#include <Windows.h>
#else
#error Linux isn´t supported yet. Sorry
#endif

#include "../common/global_configs.h"

#include "download_config.h"
#include "download_server.h"
#include "global_manager.h"
#include "FileMgr.h"

#include <boost/bind.hpp>
#include <boost/thread.hpp>

int main()
{
#ifdef WIN32
	SetConsoleTitle(TEXT("Teemo.SR ~ DownloadServer [Windows build]"));
#endif

#ifdef LINUX
	// yeah I began to make the source working on linux too.
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

	if(DownloadConfig::Load())
	{
		printl("Loading configs success (download.xml)\n");
	}
	else
	{
		printl("Loading configs failed, in download.xml\n");
		_getch();
		return 0;
	}

	printf("================================================================================\n");

	if(FileMgr::getSingleton()->Load())
	{
		printl("Loading files and patches success.\n");
	}
	else
	{
		printl("Loading files and patches failed.\n");
		_getch();
		return 0;
	}

	printf("================================================================================\n");

	if(GlobalManager::getSingleton()->Start())
	{
		GlobalManager::getSingleton()->AuthAs();
	}
	else
	{
		printl("GlobalManager failed at startup.\n");
		_getch();
		return 0;
	}

	printf("================================================================================\n");

	DownloadServer server (DownloadConfig::DownloadPort(), /*Todo: read thread count from configs. */16);
	printl("DownloadServer is starting. on 6 threads.\n");	
	boost::thread(boost::bind(&DownloadServer::start, &server)); 

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	while(_getch() != 27) {
	}	
	// Escape pressed

	return 1;
}