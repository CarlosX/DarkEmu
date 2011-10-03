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
 * TODO:
 * Hook the client->gameserver packet writing functions...
 * Will do this after auth/core server integration with new framework...
 **/

#ifdef LOG_PACKETS

struct MsgStreamBuffer {
	unsigned long vtable;
	unsigned long offset;
	unsigned long size;
	unsigned long unknown1;
	unsigned long unknown2;	// Ptr to packet buffer as well?
	char* buffer;
	unsigned short cur_opcode;
};

void __stdcall OnReadBytes(void* thispotr,unsigned long retaddr,char* out_buffer,int read_len){
	MsgStreamBuffer* thisptr=(MsgStreamBuffer*)thispotr;

	char filename[256];
	sprintf(filename,".\\packets\\%.4X.txt",thisptr->cur_opcode);
	FILE* f_pak = fopen(filename,"a+");
	FILE* f_tot = fopen(".\\packets\\dump.txt","a+");

	if(f_pak&&f_tot) {
		if(!thisptr->offset) {
			fprintf(f_pak,"\n== NEW PACKET ===================================================\n");
			fprintf(f_tot,"\n== NEW PACKET, opcode: %.4X =====================================\n",thisptr->cur_opcode);

			// Dump the entire packet buffer
			unsigned int pos = 0;
			unsigned char* p = (unsigned char*)(thisptr->buffer + 4);
			while(pos < thisptr->size){
				fprintf(f_pak,"%.2X(%c) ",*p,isalpha(*p)?*p:'.');
				fprintf(f_tot,"%.2X(%c) ",*p,isalpha(*p)?*p:'.');

				if(!((pos+1)%8)){
					fprintf(f_pak,"\n");
					fprintf(f_tot,"\n");
				}

				pos++;
				p++;
			}

			// Write a few lines...
			fprintf(f_pak,"\n\n");
			fprintf(f_tot,"\n\n");
		}

		// Now write the data the client reads
		unsigned char* p=(unsigned char*)(thisptr->buffer + thisptr->offset + 4);
		fprintf(f_pak,"%p: ",retaddr);
		fprintf(f_tot,"%p: ",retaddr);
		for(int i = 0; i < read_len;i++){
			fprintf(f_pak,"%.2X ",*p);
			fprintf(f_tot,"%.2X ",*p);
			p++;
		}
		fprintf(f_pak,"\n");
		fprintf(f_tot,"\n");

		fclose(f_pak);
		fclose(f_tot);
	}

	if(f_pak)fclose(f_pak);
	if(f_tot)fclose(f_tot);
}

#endif // #ifdef LOG_PACKETS