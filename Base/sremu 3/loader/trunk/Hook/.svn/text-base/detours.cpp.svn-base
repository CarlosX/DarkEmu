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

extern "C" {
#include "olly_disassembler/disasm.h"
}

/**
 * Simple Detours Implementation
 **/

// Warning: using detour_create when an active process thread is running is potentially stupid.
// this is safe in our context because all main app threads are suspended while this DLL's DllMain function runs!


// detour_create, creates a detour on a function at addr to newfunc
// returns a detour object
detour_s* detour_create(void* addr,void* newfunc){
#ifdef LOG_SHIT
	log("detouring function at %p to %p ...\n",addr,newfunc);
#endif

	// Compute length
	t_disasm td={0};
	char* eip=(char*)addr;
	unsigned long len=Disasm(eip,100,(ulong)eip,&td,DISASM_SIZE);
	while(len<5)
		len+=Disasm(eip+len,100,(ulong)eip+len,&td,DISASM_SIZE);
	unsigned long bytes_to_patch=len;


	// Change the permissions on memory
	unsigned long oldprot=0;
	VirtualProtect(addr,bytes_to_patch,PAGE_EXECUTE_READWRITE,&oldprot);

	// Create our detour object
	detour_s* det=new detour_s(bytes_to_patch,addr);
	memcpy(det->rawdata,addr,bytes_to_patch);
	memset(addr,0x90,bytes_to_patch);

	// Write the jump patch in
	char* ptr=(char*)addr;
	*ptr++ = (char)0xE9;
	unsigned long jmp_to_new = (unsigned long)newfunc - ((unsigned long)addr + 5);
	*((unsigned long*)ptr) = jmp_to_new;

	// Finish off our trampoline
	ptr = &det->rawdata[bytes_to_patch];
	*ptr++ = (char)0xE9;
	unsigned long jmp_to_orig = ((unsigned long)addr + bytes_to_patch) - ((unsigned long)ptr + 4);
	*((unsigned long*)ptr) = jmp_to_orig;

	det->orig_func_trampoline = det->rawdata;

	// Re-protect the memory
	VirtualProtect(addr,bytes_to_patch,oldprot,NULL);

#ifdef LOG_SHIT
	log("detour complete.  returning new detour object at %p!\n",det);
#endif

	// Return the new trampoline!
	return det;
}

// detour_remove, removes a detour with the given detour object
// also deletes the detour passed, do not manually clean this up!
void detour_remove(detour_s* det){
#ifdef LOG_SHIT
	log("removing detour from %p with %d byte patch...\n",det->orig_addr,det->bytes_patched);
#endif
	// Change the protection
	unsigned long oldprot=0;
	VirtualProtect(det->orig_addr,det->bytes_patched,PAGE_EXECUTE_READWRITE,&oldprot);
	// Copy the original bytes back in
	memcpy(det->orig_addr,det->orig_func_trampoline,det->bytes_patched);
	// Re-protect
	VirtualProtect(det->orig_addr,det->bytes_patched,oldprot,NULL);
	// Clean up our memory
	delete det;
#ifdef LOG_SHIT
	log("done removing detour!\n");
#endif
}
//========== END DETOURS ========================