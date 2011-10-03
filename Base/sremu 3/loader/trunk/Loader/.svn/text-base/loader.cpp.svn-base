/* 
 * Copyright (C) 2005-2008 SREmu <http://www.sremu.org/>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

// Written by jMerliN

#include "loader.h"

Loader::Loader():loaded(false),running(false),shared(false),share(NULL){
	memset(&si,0,sizeof(STARTUPINFOA));
	memset(&pi,0,sizeof(PROCESS_INFORMATION));
}
Loader::~Loader(){
	if(loaded&&!running){
		// Kill the process :(
		TerminateProcess(pi.hProcess,0);
	}
	
	if(shared){
		// Close our share
		CloseHandle(share);
	}

	if(loaded){
		CloseHandle(pi.hThread);
		CloseHandle(pi.hProcess);
	}
}

bool Loader::load(const char* executable) {
	if(loaded)return false;

	// Attempt to create the process in a suspended state
	int len=strlen(executable);
	char cmdline[1024];
	char path[256];
	GetModuleFileName(NULL,path,256);
	char* p=strstr(path,".exe");
	while(*p!='\\'&&*p!='/')p--;
	*(++p)=0;

	srand((unsigned long)time(0));
	sprintf_s(cmdline,1024,"%s%s %d /18 0 1",path,executable,rand());

	if(!CreateProcess(NULL,cmdline,NULL,NULL,FALSE,CREATE_SUSPENDED,NULL,NULL,&si,&pi))
		return false;

	loaded=true;

	return true;
}

bool Loader::sharedata(const char* connect_to,unsigned short port){
	if(!loaded||running||shared)return false;

	// Resolve the IP address!
	unsigned long iaddr=inet_addr(connect_to);
	if(iaddr==INADDR_ANY||iaddr==INADDR_NONE){
		// Invalid IP address, try to resolve it using DNS!
		hostent* he=gethostbyname(connect_to);
		if(he==NULL){
			int errcode=WSAGetLastError();
			if(errcode==WSAHOST_NOT_FOUND)
				printf("Unable to find host %s!\n",connect_to);
			else if(errcode==WSANO_DATA)
				printf("No record found for %s!\n",connect_to);
			else
				printf("Unable to resolve %s, error returned: %d\n",errcode);
			return false;
		}
		iaddr = *(unsigned long*)he->h_addr_list[0];
	}

	int size=6;

	share=CreateFileMapping((HANDLE)0xffffffff,NULL,PAGE_READWRITE,0,size,"SRLoader");
	if(share==NULL)return false;

	char* addr=(char*)MapViewOfFile(share,FILE_MAP_WRITE,0,0,size);
	if(addr==0){
		// Kill the mapping and return false..
		CloseHandle(share);
		return false;
	}

	// Write our data
	memcpy(addr,&iaddr,4);
	memcpy(addr+4,&port,2);
	
	// We don't need our mapped view any longer
	UnmapViewOfFile(addr);

	return (shared=true);
}

bool Loader::inject(){
	if(!shared||running)return false;

	// Write in our DLL name
	char buffer[512];
	GetModuleFileName(NULL,buffer,512);
	char* p=strstr(buffer,".exe");
	while(*p!='\\'&&*p!='/')p--;
	*(++p)=0;

	strcat_s(buffer,512,"silkhook.dll");

	// Try to inject it.. first we need some virtual memory alloc'ed in SRO's process
	void* addr = VirtualAllocEx(pi.hProcess,NULL,512,MEM_COMMIT,PAGE_READWRITE);
	if(!addr){
		printf("Unable to allocate virtual memory in target process!  Got: %p\n",addr);
		char buffer[512];
		FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM,NULL,GetLastError(),0,buffer,512,NULL);
		printf("Sys error: %s\n",buffer);
		return false;	// TEH FAILURE.
	}

	// Now we need to write to it...
	if(!WriteProcessMemory(pi.hProcess,addr,buffer,512,NULL)){
		// Remove our virtual alloc, failed!
		printf("Unable to write dll string into virtual memory of target process!\n");
		char buffer[512];
		FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM,NULL,GetLastError(),0,buffer,512,NULL);
		printf("Sys error: %s\n",buffer);
		VirtualFreeEx(pi.hProcess,addr,NULL,MEM_DECOMMIT);
		return false;
	}

	// Now call 'LoadLibrary' !  Kernel32.dll is statically mapped to the same addr. in all processes!
	void* lladdr=GetProcAddress(GetModuleHandle("kernel32.dll"),"LoadLibraryA");
	
	// Call it
	HANDLE hthread = CreateRemoteThread(pi.hProcess,NULL,NULL,(LPTHREAD_START_ROUTINE)lladdr,addr,0,NULL);

	// Wait for our thread to complete!
	WaitForSingleObject(hthread,INFINITE);

	// Dealloc our memory, we wanna be nice and not fuck with SRO's head TOOOO much ;>
	CloseHandle(hthread);
	VirtualFreeEx(pi.hProcess,addr,NULL,MEM_DECOMMIT);

	// And we're done!
	return true;
}

// Finish simply sets the running flag and resumes SRO
bool Loader::finish(){
	if(!shared||running)return false;

	// Resume the thread!
	ResumeThread(pi.hThread);

	// Done!
	return (running=true);
}




//=================================================================================================
void print_help(const char* p){
	printf("Usage: %s [-h <host>] [-p <port>]\n");
	printf("\t<host> is an IP address or host name eg. test.mysremu.net\n");
	printf("\t<port> is a valid port from 1-65535\n\n");
	printf("Note: SRLoader *MUST* be in the same folder as your silkroad executable!\n");
}

int main(int argc,char** argv){
	char* addr="127.0.0.1";
	char* exe="sro_client.exe";
	unsigned short port=15779;

	printf("==============================================\n");
	printf("SRLoader v1.0 (c) SREmu\n");
	printf("Created by jMerliN\n");
	printf("==============================================\n");

	// Check args
	for(int i=1;i<argc;i++){
		if(!_stricmp(argv[i],"-host")){
			if(i+1>argc){
				printf("Argument -host must be followed by a host/ip address!\n");
				exit(1);
			}
			addr=argv[++i];
		}else if(!_stricmp(argv[i],"-port")){
			if(i+1>argc){
				printf("Argument -port must be followed by a port!\n");
				exit(1);
			}
			port=(unsigned short)atoi(argv[++i]);
		}else if(!_stricmp(argv[i],"-h")){
			print_help(argv[0]);
			exit(0);
		}
	}
	WSADATA w;
	WSAStartup(MAKEWORD(2,2),&w);

	// Let's do this shit kthx?
	Loader l;
	bool cont=true;
	printf("Launching %s...\n",exe);
	if(cont&&!l.load(exe)){
		printf("Unable to start %s!\n",exe);
		cont=false;
	}

	printf("Creating shared memory mapping...\n");
	if(cont&&!l.sharedata(addr,port)){
		printf("Unable to map shared memory!\n");
		cont=false;
	}

	printf("Injecting silkhook...\n");
	if(cont&&!l.inject()){
		printf("Unable to inject!\n");
		cont=false;
	}

	printf("Resuming %s...\n",exe);
	if(cont&&!l.finish()){
		printf("Unable to resume process!\n");
		cont=false;
	}

	WSACleanup();

	if(cont){
		printf("Successfully loaded!\n");
	}

	exit(cont==false);
}