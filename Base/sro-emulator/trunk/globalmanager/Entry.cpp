#define WIN32_LEAN_AND_MEAN

#include <iostream>
#include <conio.h>

#ifdef WIN32
#include <Windows.h>
#else
#error Linux isn´t supported yet. Sorry
#endif

#include "global_server.h"

#include "../common/global_configs.h"
#include "../common/common.h"

int main()
{
#ifdef WIN32
	SetConsoleTitle(TEXT("Teemo.SR ~ GlobalManager [Windows build]"));
#endif

#ifdef LINUX
	// yeah I began to make the source working on linux too.
	printl("Teemo.SR ~ GlobalManager [Linux build]\n\n");
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

	printf("================================================================================\n");

	printl("Wait for following servers: \n");
	printl("  -> GatewayServer\n");
	printl("  -> DownloadServer\n");
	printl("  -> GameServer(s)\n");
	printf("================================================================================\n");

	GlobalServer global (GlobalConfigs::Port(), 6);
	global.start();

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	while(_getch() != 27) {
	}	
	// Escape pressed

	return 1;
}