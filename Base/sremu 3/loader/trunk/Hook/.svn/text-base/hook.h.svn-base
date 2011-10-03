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

#ifndef _HOOK_H_
#define _HOOK_H_

// Written by jMerliN
//#define LOG_SHIT "srloader.txt"		// Comment out to stop logging information or change ou
#define LOG_PACKETS						// Comment out to stop logging packets
//#define USE_HIDING					// Comment for 64bit or if you don't want hiding code...
//#define HOOK_CONNECT					// This allows us to easily log packets on official servers.

// Include our headers
#define WINVER 0x0510
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <cstdlib>
#include <cstdio>
#include <ctime>
#include <cstdarg>
#include <winsock2.h>

#ifdef LOG_SHIT
void log(const char* fmt,...);
#endif


#ifdef USE_HIDING
struct _UNICODE_STRING {
	unsigned short Length;
	unsigned short MaximumLength;
	wchar_t* Buffer;
};

struct _LDR_DATA_TABLE_ENTRY {
	_LIST_ENTRY InLoadOrderLinks;
	_LIST_ENTRY InMemoryOrderLinks;
	_LIST_ENTRY InInitializationOrderLinks;
	void* DllBase;
	void* EntryPointer;
	unsigned long SizeOfImage;
	_UNICODE_STRING FullDllName;
	_UNICODE_STRING BaseDllName;
	unsigned long Flags;
	unsigned short LoadCount;
	unsigned short TlsIndex;
	_LIST_ENTRY HashLinks;
	void* SelectionPointer;
	unsigned long CheckSum;
	unsigned long TimeDateStamp;
	void* LoadedImports;
	void* EntryPointActivationContext;
	void* PatchInformation;
};

struct _PEB_LDR_DATA {
	unsigned long Length;
	unsigned char Initialized;
	void* SsHandle;
	_LIST_ENTRY InLoadOrderModuleList;
	_LIST_ENTRY InMemoryOrderModuleList;
	_LIST_ENTRY InInitializationOrderModuleList;
	void* EntryInProgress;
};

struct _CURDIR {
	_UNICODE_STRING DosPath;
	void* Handle;
};

struct _STRING {
	unsigned short Length;
	unsigned short MaximumLength;
	char* Buffer;
};

struct _RTL_DRIVE_LETTER_CURDIR {
	unsigned short Flags;
	unsigned short Length;
	unsigned long Timestamp;
	_STRING DosPath;
};

struct _RTL_USER_PROCESS_PARAMETERS {
	unsigned long MaximumLength;
	unsigned long Length;
	unsigned long Flags;
	unsigned long DebugFlags;
	void* ConsoleHandle;
	unsigned long ConsoleFlags;
	void* StandardInput;
	void* StandardOutput;
	void* StandardError;
	_CURDIR CurrentDirectory;
	_UNICODE_STRING DllPath;
	_UNICODE_STRING ImagePathName;
	_UNICODE_STRING CommandLine;
	void* Environment;
	unsigned long StartingX;
	unsigned long StartingY;
	unsigned long CountX;
	unsigned long CountY;
	unsigned long CountCharsX;
	unsigned long CountCharsY;
	unsigned long FillAttribute;
	unsigned long WindowFlags;
	unsigned long ShowWindowFlags;
	_UNICODE_STRING WindowTitle;
	_UNICODE_STRING desktopInfo;
	_UNICODE_STRING ShellInfo;
	_UNICODE_STRING RuntimeData;
	_RTL_DRIVE_LETTER_CURDIR CurrentDirectories[32];
};

struct _PEB_FREE_BLOCK {
	_PEB_FREE_BLOCK* Next;
	unsigned long Size;
};

struct _PEB {
	unsigned char InheritedAddressSpace;
	unsigned char ReadImageFileExecOptions;
	unsigned char BeingDebugged;
	unsigned char SpareBool;
	void* Mutant;
	void* ImageBaseAddress;
	_PEB_LDR_DATA* Ldr;
	_RTL_USER_PROCESS_PARAMETERS* ProcessParameters;
	void* SubSystemData;
	void* ProcessHeap;
	_RTL_CRITICAL_SECTION FastPebLock;
	void* FastPebLockRoutine;
	void* FastPebUnlockRoutine;
	unsigned long EnvironmentUpdateCount;
	void* KernelCallbackTable;
	unsigned long SystemReserved[1];
	unsigned long AtlThunkSListPtr32;
	_PEB_FREE_BLOCK FreeList;
	unsigned long TlsExpansionCounter;
	void* TlsBitmap;
	unsigned long TlsBitmapBits[2];
	void* ReadOnlySharedMemoryBase;
	void* ReadOnlySharedMemoryHeap;
	void** ReadOnlyStaticServerData;
	void* AnsiCodePageData;
	void* OemCodePageData;
	void* UnicodeCaseTableData;
	unsigned long NumberOfProcessors;
	unsigned long NtGlobalFlag;
	_LARGE_INTEGER CriticalSectionTimeout;
	unsigned long HeapSegmentReserve;
	unsigned long HeapSegmentCommit;
	unsigned long HeapDeCommitTotalFreeThreshold;
	unsigned long HeapDeCommitFreeBlockThreshold;
	unsigned long NumberOfHeaps;
	unsigned long MaximumNumberOfHeaps;
	void** ProcessHeaps;
	void* GdiSharedHandleTable;
	void* ProcessStarterHelper;
	unsigned long GdiDCAttributeList;
	void* LoaderLock;
	unsigned long OSMajorVersion;
	unsigned long OSMinorVersion;
	unsigned short OSBuildNumber;
	unsigned short OSCSDVersion;
	unsigned long OSPlatformId;
	unsigned long ImageSubsystem;
	unsigned long ImageSubsystemMajorVersion;
	unsigned long ImageSubsystemMinorVersion;
	unsigned long ImageProcessAffinityMask;
	unsigned long GdiHandleBuffer[34];
	void* PostProcessInitRoutine;
	void* TlsExpansionBitmap;
	unsigned long TlsExpansionBitmapBits[32];
	unsigned long SessionId;
	_ULARGE_INTEGER AppCompatFlags;
	_ULARGE_INTEGER AppCompatFlagsUser;
	void* pShimData;
	void* AppCompatInfo;
	_UNICODE_STRING CSDVersion;
	void* ActivationContextData;
	void* ProcessAssemblyStorageMap;
	void* SystemDefaultActivationContextData;
	void* SystemAssemblyStorageMap;
	unsigned long MinimumStackCommit;
};

void disable_unicode_string(_UNICODE_STRING& string);
bool peb_hide_mod(unsigned long mod);
void pe_hide(unsigned long mod);
void polymorph(void);
#endif // #ifdef USE_HIDING


struct detour_s {
	detour_s(int bp,void* orig):bytes_patched(bp),orig_addr(orig),orig_func_trampoline(0){
		memset(rawdata,0x90,64);
	}
	int bytes_patched;			// # of bytes from start of func that need to be backed up before patching (must be >= 5!)
	void* orig_addr;			// Original address of function
	void* orig_func_trampoline;	// pfn to trampoline function to call orig (just ptr to rawdata)
	char rawdata[64];			// raw data, stores original bytes & jmp to orig function + # of bytes
};

// Detours
detour_s* detour_create(void* addr,void* newfunc);
void detour_remove(detour_s* det);

// Signatures
bool get_mod(const char* module,const char* section,unsigned long& address,unsigned long& size);
bool sig_cmp(const char* sig, const char* mask, unsigned long len, void* addr);
unsigned long sig_find(const char* sig,const char* mask,unsigned long len,void* start,unsigned long size);
void write_patch(const char* sig,const char* mask,unsigned long len,void* addr);

// Hooking
void do_hook(void);
void do_unhook(void);


#ifdef LOG_PACKETS
// Typedefs for PFNs
typedef hostent* (__stdcall*ghpfn)(const char* name);
typedef int (__stdcall*bpfn)(SOCKET s,const sockaddr* name,int namelen);
#endif

#ifdef LOG_PACKETS
typedef int (__stdcall*rbhook)(char* out_buffer,int read_len);
extern rbhook oReadBytes;
void __stdcall OnReadBytes(void* thisptr,unsigned long retaddr,char* out_buffer,int read_len);
#endif // #ifdef LOG_PACKETS


#endif // #ifndef _HOOK_H_