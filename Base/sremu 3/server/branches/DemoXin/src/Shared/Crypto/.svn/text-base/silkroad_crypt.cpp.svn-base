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

#include "silkroad_crypt.h"

#ifdef USE_SECURITY
#if USE_SECURITY&1

unsigned long SilkroadCrypt::mutate(unsigned long* m){
	unsigned long val = *m;
	for( int i = 0; i < 32; i++ )
		val = (((((((((((val >> 2)^val) >> 2)^val) >> 1)^val) >> 1)^val) >> 1)^val)&1)|((((val&1) << 31)|(val >> 1))&0xFFFFFFFE);
	return ( *m = val );
}

// Don't expect documentation on this function.. uh.. cuz.. i don't understand wtf they were doing
// but yeah :>  Table init, reversed by jMerliN
void SilkroadCrypt::startup(){
	table = new unsigned long[0x10000];		// Wowza.  Eating too much chinese?  Remember, Bono is still #1.

	unsigned long* tptr=table;
	for(int i=0;i<256;i++){
		unsigned long cur_base=base_table[i];
		for(int j=0;j<256;j++){
			unsigned long eax=j;
			for(int k=0;k<8;k++){
				if(eax&1)
					eax=(eax>>1)^cur_base;
				else
					eax>>=1;
			}
			*tptr++ = eax;
		}
	}
}

void SilkroadCrypt::shutdown(){
	delete [] table;						// Talk about flushing a huge turd.
}

void SilkroadCrypt::init(unsigned long cnt_seed,unsigned long chk_seed){
	check_seed=chk_seed&0xFF;

	if(!cnt_seed) cnt_seed = 0x9ABFB3B6;
	unsigned long mut = cnt_seed;
	unsigned long mut1 = mutate(&mut);
	unsigned long mut2 = mutate(&mut);
	unsigned long mut3 = mutate(&mut);
	mutate(&mut);
	unsigned char byte1 = (mut&0xFF)^(mut3&0xFF);
	unsigned char byte2 = (mut1&0xFF)^(mut2&0xFF);
	if(!byte1) byte1 = 1;
	if(!byte2) byte2 = 1;
	cnt_bytes[0] = byte1^byte2;
	cnt_bytes[1] = byte2;
	cnt_bytes[2] = byte1;
}

unsigned char SilkroadCrypt::get_counter(void){
	unsigned char result = (cnt_bytes[2]*(~cnt_bytes[0]+cnt_bytes[1]));
	result = result^(result>>4 );
	cnt_bytes[0] = result;
	return result;
}

unsigned char SilkroadCrypt::get_checksum(const char* data,unsigned int len){
	if(!data)return 0;
	unsigned long checksum=0xffffffff;
	len &= 0x7fff;
	unsigned long moddedseed = check_seed<<8;
	for(unsigned int i=0;i<len;i++)
		checksum = (checksum>>8)^table[moddedseed+((*(data++)^checksum)&0xff)];
	unsigned char result=((checksum>>24)&0xff)+((checksum>>8)&0xff)+((checksum>>16)&0xff)+(checksum&0xff);
	return result;
}


unsigned long* SilkroadCrypt::table=0;
unsigned long SilkroadCrypt::base_table[256] = {
0x968BD6B1, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F, 0xE963A535, 0x9E6495A3,
0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,
0x1DB71064, 0x6AB020F2, 0xF3B17148, 0x8CBE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7,
0x136C9856, 0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5,
0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59,
0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8B7A50F,
0x2802B89E, 0x5F058808, 0xC6ECD9B2, 0xB10BE924, 0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D,
0x76DC4190, 0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433,
0x7807C9A2, 0x0FA0F934, 0x9609A88E, 0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457,
0x63B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB,
0x4369E96A, 0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9,
0x5005713C, 0x270241AA, 0xBE0B1010, 0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
0x5EDEF90E, 0x29D9C908, 0xB0D09822, 0xC757A8B4, 0x59B33D17, 0x3EB40D81, 0xB7BD5C3B, 0xC0BA6CAD,
0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683,
0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1,
0xF00F9344, 0x8708A2D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB, 0x196C3671, 0x6E6B06E7,
0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B,
0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x46690E79,
0xCB51B38C, 0xBC63831A, 0x256FD2A0, 0x5268E236, 0xCC0C7795, 0xBB0B4703, 0x220214B9, 0x5505262F,
0xC5B63BBE, 0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D,
0x9B6472B0, 0xECE3F226, 0x756AA39C, 0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076885, 0x05000713,
0x95BF4882, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B, 0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21,
0x86D3D294, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
0x88085AE0, 0xF10F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45,
0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB,
0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
0xBDBDF91C, 0xCABACD8A, 0x53B39E30, 0x24BCA3A6, 0xBAD03B05, 0xCDD706A3, 0x54DE57E9, 0x23D967BF,
0xB366722E, 0xC4614AB8, 0x5D381B02, 0x2B6F2B94, 0xB4CBBE37, 0xC3CC8EA1, 0x5A0DDF1B, 0x2D02ED8D };

#endif // #if USE_SECURITY&1
#endif // #ifdef USE_SECURITY