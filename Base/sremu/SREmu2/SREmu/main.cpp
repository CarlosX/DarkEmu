 /* 
  * This file is part of SREmu.
  *
  * Copyright (c) 2010 Justin "jMerliN" Summerlin, SREmu <http://sremu.sourceforge.net>
  *
  * SREmu is free software: you can redistribute it and/or modify
  * it under the terms of the GNU Affero General Public License as published by
  * the Free Software Foundation, either version 3 of the License, or
  * (at your option) any later version.
  * 
  * SREmu is distributed in the hope that it will be useful,
  * but WITHOUT ANY WARRANTY; without even the implied warranty of
  * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  * GNU Affero General Public License for more details.
  * 
  * You should have received a copy of the GNU Affero General Public License
  * along with SREmu.  If not, see <http://www.gnu.org/licenses/>.
  */

#include <database.h>
#include "sremu.h"
#include <stdio.h>

MySQLManager* g_mysql = 0;
AgentServer* g_agentServer = 0;
GatewayServer* g_gatewayServer = 0;

void setConsoleColor(ushort col,bool intense=true){
	static HANDLE con = GetStdHandle(STD_OUTPUT_HANDLE);
	SetConsoleTextAttribute(con,col|(intense?FOREGROUND_INTENSITY:0));
}

#define TXT_GREEN FOREGROUND_GREEN
#define TXT_BLUE FOREGROUND_BLUE
#define TXT_RED FOREGROUND_RED
#define TXT_WHITE (TXT_GREEN|TXT_BLUE|TXT_RED)
#define TXT_YELLOW (TXT_RED|TXT_GREEN)
#define TXT_PURPLE (TXT_RED|TXT_BLUE)
#define TXT_CYAN (TXT_GREEN|TXT_BLUE)

#define SREMU_DB "jmerlin-pc"
#define SREMU_DB_USER "sremu2"
#define SREMU_DB_PASS "sremu2"
#define SREMU_DB_NAME "sremu2"
#define SREMU_DB_PORT 3306

#define STEP(x) printf("%-40s",x)
#define CON_ERR() setConsoleColor(TXT_RED); printf("[ERROR]\n"); setConsoleColor(TXT_YELLOW)
#define CON_OK() setConsoleColor(TXT_GREEN); printf("[OK]\n"); setConsoleColor(TXT_WHITE)

ThreadWaiter running;
void mysig(int sig){
	running.set();
}

void printSysError(int err){
	char temp[1024];
	FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM,NULL,err,NULL,temp,sizeof(temp),NULL);
	printf("%s",temp);
}

void printConsoleTitle(const char* str){
	int len = strlen(str);
	char temp[100];
	memset(temp,'=',len+6);
	temp[len+6] = 0;
	setConsoleColor(TXT_WHITE,false);
	printf("%s\n%s",temp,"== ");
	setConsoleColor(TXT_YELLOW);
	printf("%s",str);
	setConsoleColor(TXT_WHITE,false);
	printf("%s\n%s\n"," ==",temp);
}

int exit_cleanup(int i){
	setConsoleColor(TXT_WHITE,false);
	return i;
}

int main(int argc, char** argv){
	printConsoleTitle("SREmu2 Pre-Alpha Dev Version");
	setConsoleColor(TXT_WHITE);

	STEP("Initializing winsock...");
	WSADATA wsa={0};
	int res = WSAStartup(MAKEWORD(2,2),&wsa);
	if(res){
		CON_ERR();
		res = WSAGetLastError();
		printf("Last error: %d\n",res);
		printSysError(res);
		return exit_cleanup(1);
	}
	CON_OK();

	STEP("Initializing libmysql...");
	res = mysql_library_init(argc, argv, 0);
	if(res){
		CON_ERR();
		printf("Error initializing mysql library: %d\n",res);
		return exit_cleanup(1);
	}

	MYSQL* my = mysql_init(NULL);
	if(!my){
		CON_ERR();
		printf("Error calling mysql_init, returned null!\n");
		return exit_cleanup(1);
	}
	CON_OK();

	STEP("Connecting to MySQL Server...");

	// Initialize our sql handler
	g_mysql = new MySQLManager(my);

	MYSQL* c = mysql_real_connect(my,SREMU_DB,SREMU_DB_USER,SREMU_DB_PASS,SREMU_DB_NAME,SREMU_DB_PORT,0,0);
	if(c == NULL){
		CON_ERR();
		printf("Unable to connect to MYSQL server %s:%d with user=%s\n",SREMU_DB,SREMU_DB_PORT,SREMU_DB_USER);
		return exit_cleanup(1);
	}
	CON_OK();

	STEP("Constructing Gateway server...");
	g_gatewayServer = new GatewayServer("15779");
	std::auto_ptr<GatewayServer> gate(g_gatewayServer);
	CON_OK();

	STEP("Constructing Agent server...");
	g_agentServer = new AgentServer("15780");
	std::auto_ptr<AgentServer> agent(g_agentServer);
	CON_OK();

	STEP("Dispatching threads to listen...");
	if(!gate->ready()){
		CON_ERR();
		printf("Unable to start Gateway server...\n");
		return exit_cleanup(1);
	}else if(!agent->ready()){
		CON_ERR();
		printf("Unable to start Agent server...\n");
		return exit_cleanup(1);
	}
	Thread gateThread,agentThread;
	gateThread.start(g_gatewayServer);
	agentThread.start(g_agentServer);
	
	CON_OK();

	STEP("Installing signal handlers...");
	signal(SIGINT,mysig);
	signal(SIGTERM,mysig);
	signal(SIGBREAK,mysig);
	CON_OK();

	// Main busy loop...
	setConsoleColor(TXT_WHITE,false);
	printf("Initialization complete!\n");
	running.wait(INFINITE);

	mysql_close(my);
	
	printf("Cleaning up...\n");

	// Cleanups...
	mysql_library_end();
	WSACleanup();

	return exit_cleanup(0);
}