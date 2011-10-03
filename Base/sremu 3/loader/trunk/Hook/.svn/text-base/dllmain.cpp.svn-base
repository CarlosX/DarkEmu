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

// DllMain -- entrypoint of dll, does everything.  read the code
// for more information :)  (comment out the following line if you dont want things to be logged!)
BOOL __stdcall DllMain(HINSTANCE dll,DWORD reason,LPVOID unused){
	// If this is the startup call for the DLL
	if(reason==DLL_PROCESS_ATTACH){
#ifdef USE_HIDING
		polymorph();
		pe_hide((unsigned long)dll);
		peb_hide_mod((unsigned long)dll);
#endif

#ifdef LOG_SHIT
		time_t t;
		time(&t);
		log("SRLoader DLL Log!\nSTARTING -- TIME: %s\n",ctime(&t));
#endif

		do_hook();
	}else if(reason==DLL_PROCESS_DETACH){
		do_unhook();
	}
	return TRUE;
}