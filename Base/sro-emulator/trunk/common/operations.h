#ifndef _PACKET_OPCODES_H_
#define _PACKET_OPCODES_H_

// GLobals -> Security
#define HANDSHAKE_STUFF_1	0x5000
#define HANDSHAKE_STUFF_2	0x9000

// Globals -> Other
#define PING_FROM_CLIENT	0x2002

// Globals -> info request
#define SERVER_INFO_REQUEST	0x2001

// Gatewayserver
#define NEWS_REQUEST		0x6104
#define SEND_NEWS			0xA104
#define DOWNLOAD_INFO1		0x2005
#define DOWNLOAD_INFO2		0x6005
#define DOWNLOAD_INFO3		0xA100

// Downloadserver
#define REQUEST_DOWNLOAD	0x6100
#define REQUEST_FILE		0x6004
#define DOWNLOAD_PART		0x1001
#define DOWNLOAD_COMPLETED	0xA004

// ----------------------------------------------------------------------------------------------

// Client@Login Phase
#define LOGIN_PHASE_REQ_SERVERLIST		0x6101
#define LOGIN_PHASE_SEND_SERVERLIST		0xA101
#define LOGIN_PASHE_AUTH			    0x6102	
#define LOGIN_PHASE_AUTH_ANSWERE		0xA102
#define LOGIN_PHASE_SERVER_FULL			0xA103

#endif // _PACKET_OPCODES_H_