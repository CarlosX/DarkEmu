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

/**
 * Hook Hiding Code
 **/

#include "hook.h"

#ifdef USE_HIDING

void disable_unicode_string(_UNICODE_STRING& string){
	for( int i = 0; i < string.MaximumLength; i++ )
		string.Buffer[i] = 0;
	string.Length = 0;
	string.MaximumLength = 0;
}

bool peb_hide_mod(unsigned long mod) {
	_PEB *peb;
	__asm {
		push eax;
		mov eax, dword ptr fs:[0x30];
		mov [peb], eax;
		pop eax;
	}
	_LDR_DATA_TABLE_ENTRY *first = (_LDR_DATA_TABLE_ENTRY*)peb->Ldr->InLoadOrderModuleList.Flink;
	_LDR_DATA_TABLE_ENTRY *current = (_LDR_DATA_TABLE_ENTRY*)first->InLoadOrderLinks.Flink;
	while( current != first ) {
		if( mod == (unsigned long)current->DllBase ) {
			current->InLoadOrderLinks.Blink->Flink = current->InLoadOrderLinks.Flink;
			current->InLoadOrderLinks.Flink->Blink = current->InLoadOrderLinks.Blink;
			current->InMemoryOrderLinks.Blink->Flink = current->InMemoryOrderLinks.Flink;
			current->InMemoryOrderLinks.Flink->Blink = current->InMemoryOrderLinks.Blink;
			current->InInitializationOrderLinks.Blink->Flink = current->InInitializationOrderLinks.Flink;
			current->InInitializationOrderLinks.Flink->Blink = current->InInitializationOrderLinks.Blink;
			
			disable_unicode_string( current->BaseDllName );
			disable_unicode_string( current->FullDllName );

			current->InLoadOrderLinks.Flink = 0;
			current->InLoadOrderLinks.Blink = 0;
			current->InMemoryOrderLinks.Flink = 0;
			current->InMemoryOrderLinks.Blink = 0;
			current->InInitializationOrderLinks.Flink = 0;
			current->InInitializationOrderLinks.Blink = 0;
			current->DllBase = 0;
			current->EntryPointer = 0;
			current->SizeOfImage = 0;
			current->Flags = 0;
			current->LoadCount = 0;
			current->TlsIndex = 0;
			current->SelectionPointer = 0;
			current->CheckSum = 0;
			current->TimeDateStamp = 0;
			current->LoadedImports = 0;
			current->EntryPointActivationContext = 0;
			current->PatchInformation = 0;

			//current->HashLinks.Blink = 0;
			//current->HashLinks.Flink = 0;
			return true;
		}
		current = (_LDR_DATA_TABLE_ENTRY*)current->InLoadOrderLinks.Flink;
	}
	return false;
}

// Completely erases the PE headers.
void pe_hide(unsigned long mod){
	IMAGE_DOS_HEADER* dos = (IMAGE_DOS_HEADER*)mod;
	IMAGE_NT_HEADERS* nt = (IMAGE_NT_HEADERS*)(mod + (unsigned long)dos->e_lfanew );
	unsigned long headersize = nt->OptionalHeader.SizeOfHeaders;
	unsigned long oldprotect;
	VirtualProtect( (void*)mod, headersize, PAGE_READWRITE, &oldprotect );
	memset( (void*)dos, 0, headersize );
	VirtualProtect( (void*)mod, headersize, oldprotect, 0 );
}

void __declspec( naked ) pfunc(void){
	__asm {
		nop;
		nop;
		nop;
		nop;
		nop;
		nop;
		nop;
		nop;
		nop;
		nop;
	}
}

void polymorph(void){
	DWORD prot = 0;
	VirtualProtect( (void*)pfunc, 10, PAGE_EXECUTE_READWRITE, &prot );
	srand( GetTickCount( ) );
	char* pp = (char*)pfunc;
	for( int i = 0; i < 10; i++ )
		pp[i] = (char)( 0xAA ^ ( 0xFF & rand( ) ) );
	VirtualProtect( (void*)pfunc, 10, prot, 0 );
}

#endif // #ifdef USE_HIDING