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
 * Simple signature searching & patching code
 **/

// get_mod, given the name of an existing module running in the current process space,
// finds the named section given and returns the address & length of the section in the referenced parameters
// address & size.  returns true for success, false if the module or section could not be found.
bool get_mod(const char* module,const char* section,unsigned long& address,unsigned long& size){
	unsigned long base = (unsigned long)GetModuleHandle( module );
	if( base == 0 ) return false;

	IMAGE_DOS_HEADER* dos = (IMAGE_DOS_HEADER*)base;
	IMAGE_NT_HEADERS* nt = (IMAGE_NT_HEADERS*)(base + (unsigned long)dos->e_lfanew);
	unsigned long headers = (unsigned long)nt + sizeof( IMAGE_NT_HEADERS );

	for(int i=0;i<nt->FileHeader.NumberOfSections;i++){
		IMAGE_SECTION_HEADER* hdr = (IMAGE_SECTION_HEADER*)headers;
		if( !_stricmp( (char*)hdr->Name, section ) ) {
			address = hdr->VirtualAddress + base;
			size = hdr->Misc.VirtualSize;
			return true;
		}
		headers += sizeof(IMAGE_SECTION_HEADER);
	}
	return false;
}


// sig_cmp, given a byte signature & a mask ('x' denotes match, '?' denotes wildcard) and the length of the
// sig, checks if the memory at 'addr' matches this signature and mask
bool inline sig_cmp(const char* sig, const char* mask, unsigned long len, void* addr){
	for(unsigned long register i=0;i<len;i++)
		if(mask[i]=='x'&&((char*)addr)[i]!=sig[i]) return false;
	return true;
}

// sig_find, given a byte signature & mask with the length of the signature, and a starting address and
// size, attempts to run through the entire address range from start to start+size looking for a match
// to the indicated signature & mask
unsigned long sig_find(const char* sig,const char* mask,unsigned long len,void* start,unsigned long size){
	unsigned long end=((unsigned long)start+size)-len;
	for(unsigned long addr=(unsigned long)start;addr<end;addr++)
		if(sig_cmp(sig,mask,len,(void*)addr))
			return addr;
	return 0;
}

// write_patch, given a simple patch & mask, writes the given patch to the indicated memory address
void write_patch(const char* sig,const char* mask,unsigned long len,void* addr){
	unsigned long oldprot=0;
	VirtualProtect(addr,len,PAGE_EXECUTE_READWRITE,&oldprot);

	// Write the patch
	for(unsigned long i=0;i<len;i++)
		if(mask[i]=='x')
			((char*)addr)[i]=sig[i];

	VirtualProtect(addr,len,oldprot,NULL);
}

//============ END SIG & PATCH ===============================