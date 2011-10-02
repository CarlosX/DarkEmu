#include "client_context.h"
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void ClientContext::HandleMessage(PacketContainer* packet)
{
	switch(packet->opcode)
	{
		// Implemented in security class

		//case HANDSHAKE_STUFF_1: // security :)
		//	SendQueue();     
		//	break;

		//case HANDSHAKE_STUFF_2: // security
		//	SendQueue();
		//	break;

		case PING_FROM_CLIENT: // ignore for first.
			break;

		case SERVER_INFO_REQUEST:
			OnRequestInformation_Server(packet);
			break;

		case REQUEST_DOWNLOAD:
			OnRequestInformation_Download(packet);
			break;

		case NEWS_REQUEST:
			OnRequestNews(packet);
			break;

		case LOGIN_PHASE_REQ_SERVERLIST:
			OnRequestServerList(packet);
			break;

		case LOGIN_PASHE_AUTH:
			OnAuthentication(packet);
			break;

		default:
			printl("Unknown message with opcode 0x%.4X sent. Message size: %d.\n", packet->opcode, packet->data.GetStreamVector().size());
			break;
	}
}

