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

#include "hook.h"

/**
 * Hook code.  These are the hook functions used
 **/

// Host & port (host is the address in compressed 4byte form), neither host or port is in network order!
#ifdef HOOK_CONNECT

unsigned long host=0;
unsigned short port=0;

// gethostbyname hook -- checks if 'gwgt' is in the name, if so returns our address
ghpfn ogethostbyname;
detour_s* gethost_det=0;
bool resolved=false;
hostent* __stdcall mygethostbyname(const char* name){
#ifdef LOG_SHIT
	log("gethostbyname called with name=\"%s\"!\n",name);
#endif
	if(strstr(name,"gwgt")){
#ifdef LOG_SHIT
		log("gethostbyname was called with \"gwgt*\" as the param, substituting our address instead!\n");
#endif
		in_addr sin;
		sin.S_un.S_addr=host;
		hostent* pt=gethostbyaddr((char*)&host,4,AF_INET);
#ifdef LOG_SHIT
		log("we got a ptr: %p, returning!\n",pt);
#endif
		resolved=true;
		return pt;
	}
	return ogethostbyname(name);
}

// Ugly inefficient wrapper :o
void __declspec(naked) gethostbyname_(void){
	__asm {
		pushad;
		push [esp+0x24];
		call mygethostbyname;
		push eax;
		add esp,4;
		popad;
		mov eax,[esp-0x24];
		retn 4;
	}
}

// connect hook -- checks if the address is our address, if so, changes the port to our port.
bpfn oconnect;
detour_s* connect_det=0;
int __stdcall myconnect(SOCKET s,const sockaddr* name,int namelen){
	sockaddr_in* ptr=(sockaddr_in*)name;
	if(resolved&&!memcmp(&ptr->sin_addr.S_un.S_addr,&host,4)){
#ifdef LOG_SHIT
		log("connect called with addr matching ours!  replacing with port=%u!\n",port);
#endif
		ptr->sin_port=htons(port);
		resolved=false;
	}
	return oconnect(s,name,namelen);
}

// Another ugly inefficient wrapper ;o
void __declspec(naked) connect_(void){
	__asm {
		pushad;
		push [esp+0x2C];
		push [esp+0x2C];
		push [esp+0x2C];
		call myconnect;
		push eax;
		add esp,4;
		popad;
		mov eax,[esp-0x24];
		retn 0xC;
	}
		
}
#endif // #ifdef HOOK_CONNECT

#ifdef LOG_PACKETS
// MsgStreamBuffer::ReadBytes hook -- lets us log silkroad's reads of packets from ze serv3r
rbhook oReadBytes;
detour_s* readbytes_det=0;
void __declspec(naked) ReadBytes_(void){
	__asm {
		pushad;
		push [esp+0x28];	// Orig size
		push [esp+0x28];	// Orig buffer
		push [esp+0x28];	// Orig retaddr (for logging)
		push ecx;
		call OnReadBytes;
		popad;
		jmp oReadBytes;
	}
}
#endif // #ifdef LOG_PACKETS

//================ END HOOK CODE ====================


void do_hook(){
#ifdef HOOK_CONNECT
		// Let's get our shared memory up!  (need some error handling here :\)
	HANDLE share=CreateFileMapping((HANDLE)0xffffffff,NULL,PAGE_READWRITE,0,32,"SRLoader");
	if(share!=NULL){
		// Map a view to our shared memory
		unsigned char* mapaddr=(unsigned char*)MapViewOfFile(share,FILE_MAP_READ,0,0,6);
		// If the view is good, copy the data over to our variables
		if(mapaddr){
			memcpy(&host,mapaddr,4);
			memcpy(&port,mapaddr+4,2);
#ifdef LOG_SHIT
			log("Copied address: %d.%d.%d.%d and port: %d from mapped memory!\n",(int)mapaddr[0],(int)mapaddr[1],
				(int)mapaddr[2],(int)mapaddr[3],*(unsigned short*)&mapaddr[4]);
#endif
		}
		// Close the mapping 
		UnmapViewOfFile(mapaddr);
		CloseHandle(share);
	}else
		MessageBox(0,"Unable to find shared memory mapping!","SRLoader",MB_ICONINFORMATION);
#endif // #ifdef HOOK_CONNECT

	// Get the silkroad .text section
	unsigned long txt_sct, txt_len;
	if(!get_mod("sro_client.exe",".text",txt_sct,txt_len)){
		MessageBox(0,"Unable to find section!","SRLoader",MB_ICONINFORMATION);
		return;
	}
#ifdef LOG_SHIT
	log("Text section in sro_client.exe found: %p with length %u\n",txt_sct,txt_len);
#endif
	// Patch the 'Please start Silkroad.exe' error
	unsigned long addr=sig_find("\x83\xc4\x04\x84\xc0\x75\x00\x6a\x00\x68","xxxxxx?xxx",10,(void*)txt_sct,txt_len);
#ifdef LOG_SHIT
	log("Found \"Please start Silkroad.exe\" signature at: %p\n",addr);
#endif

	if(!addr)
		MessageBox(0,"Unable to patch call!","SRLoader",MB_ICONINFORMATION);
	else
		write_patch("\xEB","x",1,(void*)(addr+5));

	// Patch the 'bind' check to see if 15779 is already being used
	addr=sig_find("\x75\x25\x56\xff\x15\x00\x00\x00\x00\xff\x15\x00\x00\x00\x00\x33\xc0\x5e","xxxxx????xx????xxx",18,(void*)txt_sct,txt_len);
#ifdef LOG_SHIT
	log("Found bind port check at: %p\n",addr);
#endif

	if(!addr)
		MessageBox(0,"Unable to patch bind!","SRLoader",MB_ICONINFORMATION);
	else{
		write_patch("\xD2\xFE","xx",2,(void*)(addr-51));	// Patch port to 65234
		write_patch("\xEB","x",1,(void*)addr);				// Patch 'jnz' to 'jmp'
	}


	// Patch the mutex check out for multi-client
	addr=sig_find("\x6a\x00\x56\xff\xd7\x68\x00\x00\x00\x00\x6a\x00\x6a\x00","x?xxxx????xxxx",14,(void*)txt_sct,txt_len);
#ifdef LOG_SHIT
	log("Found mutex check at: %p\n",addr);
#endif

	if(!addr)
		MessageBox(0,"Unable to patch mutex!","SRLoader",MB_ICONINFORMATION);
	else
		write_patch("\x6a\x00\x90\x90\x90","xxxxx",5,(void*)(addr+5));


	// Patch the nprotect update stuff
	addr=sig_find("\x6a\x00\x6a\x00\x68\x00\x00\x00\x00\x6a\x00\x6a\x00\xa3\x00\x00\x00\x00\xff\x15","xxxxx????xxxxx????xx",20,(void*)txt_sct,txt_len);
#ifdef LOG_SHIT
	log("Found nprotect check at: %p\n",addr);
#endif

	if(!addr)
		MessageBox(0,"Unable to patch nprotect!","SRLoader",MB_ICONINFORMATION);
	else
		write_patch("\x83\xc4\x18\x90\x90\x90","xxxxxx",6,(void*)(addr+18));

	// Patch zoom-hack
	addr=sig_find("\x7a\x08\xd9\x9e\xec\x03\x00\x00\xeb\x02\xdd\xd8\xd9\x86\xec\x03\x00\x00\x6a\x00\xd9\x5c\x24\x10\x6a\x01\xd9\x44\x24\x14\xd9\x96","xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",32,(void*)txt_sct,txt_len);

#ifdef LOG_SHIT
	log("Found zoom-hack check #1 at: %p\n",addr);
#endif
	
	if(!addr)
		MessageBox(0,"Unable to patch zoom-hack check #1!","SRLoader",MB_ICONINFORMATION);
	else
		write_patch("\xeb","x",1,(void*)addr);

	addr=sig_find("\x7a\x08\xd9\x9e\xec\x03\x00\x00\xeb\x02\xdd\xd8\xd9\x86\xec\x03\x00\x00\x6a\x00\xd9\x5c\x24\x10\x6a\x01\xd9\x44\x24\x14\x8b\xce","xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",32,(void*)txt_sct,txt_len);

#ifdef LOG_SHIT
	log("Found zoom-hack check #2 at: %p\n",addr);
#endif
	
	if(!addr)
		MessageBox(0,"Unable to patch zoom-hack check #2!","SRLoader",MB_ICONINFORMATION);
	else
		write_patch("\xeb","x",1,(void*)addr);

#ifdef LOG_PACKETS
	// Locate the address of MsgStreamBuffer::ReadData
	addr=sig_find("\xCC\x53\x8B\x5C\x24\x0C\x56\x8B\xF1\x8B\x46\x04\x03\xC3\x3B\x46\x08","xxxxxxxxxxxxxxxxx",17,(void*)txt_sct,txt_len);
#ifdef LOG_SHIT
	log("Found MsgStreamBuffer::ReadData at: %p\n",addr+1);
#endif

	if(!addr)
		MessageBox(0,"Unable to locate MsgStreamBuffer::ReadData!","SRLoader",MB_ICONINFORMATION);
	else {
		addr++;
		readbytes_det=detour_create((void*)addr,(void*)ReadBytes_);
		oReadBytes = (rbhook)readbytes_det->orig_func_trampoline;
	}
#endif // #ifdef LOG_PACKETS

#ifdef HOOK_CONNECT
	HMODULE hwinsock=GetModuleHandle("ws2_32.dll");
	gethost_det=detour_create((void*)GetProcAddress(hwinsock,"gethostbyname"),(void*)gethostbyname_);
	ogethostbyname = (ghpfn)gethost_det->orig_func_trampoline;

	connect_det=detour_create((void*)GetProcAddress(hwinsock,"connect"),(void*)connect_);
	oconnect = (bpfn)connect_det->orig_func_trampoline;
#endif
}


void do_unhook(){

#ifdef LOG_SHIT
	log("\n\n");
#endif

#ifdef HOOK_CONNECT
	if(gethost_det)		detour_remove(gethost_det);
	if(connect_det)		detour_remove(connect_det);
#endif

#ifdef LOG_PACKETS
	if(readbytes_det)	detour_remove(readbytes_det);
#endif
}