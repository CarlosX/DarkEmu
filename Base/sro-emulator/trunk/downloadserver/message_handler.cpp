#include "client_context.h"

#include "FileMgr.h"

//////////////////////////////////////////////////////////////////////////////////////////////////////////

void ClientContext::HandleMessage(PacketContainer* packet)
{
	switch(packet->opcode)
	{
	case PING_FROM_CLIENT:
		break;

	case SERVER_INFO_REQUEST:
		OnRequestInformation_Server(packet);
		break;

	case REQUEST_FILE:
		OnRequestFile(packet);
		break;

	default:
		printl("Unknown message with opcode 0x%.4X sent. Message size: %d.\n", packet->opcode, packet->data.GetStreamVector().size());
		break;
	}
}

void ClientContext::OnRequestInformation_Server(PacketContainer* packet)
{
	StreamUtility stream =  StreamUtility();
	stream.Write<uint16_t>	(14);
	stream.Write_Ascii		("DownloadServer", 14);
	stream.Write<uint8_t>	(0x00);
	m_security.Send         (SERVER_INFO_REQUEST, stream);
	SendQueue               ();
	stream.Clear            ();
	

	stream.Write<uint8_t>	(0x01);
	stream.Write<uint8_t>	(0x00);
	stream.Write<uint8_t>	(0x01);
	stream.Write<uint8_t>	(0x48);
	stream.Write<uint8_t>	(0x01);
	stream.Write<uint8_t>	(0x05);
	stream.Write<uint8_t>	(0x00);
	stream.Write<uint8_t>	(0x00);
	stream.Write<uint8_t>	(0x00);
	stream.Write<uint8_t>	(0x02);
	m_security.Send(DOWNLOAD_INFO1, stream, false, true);
	stream.Clear();
	SendQueue();

	stream.Write<uint8_t>	(0x03);
	stream.Write<uint8_t>	(0x00);
	stream.Write<uint8_t>	(0x02);
	stream.Write<uint8_t>	(0x00);
	stream.Write<uint8_t>	(0x02);
	m_security.Send(DOWNLOAD_INFO2, stream, false, true);
	stream.Clear();


	SendQueue();
}


void ClientContext::OnRequestFile(PacketContainer* packet)
{
	uint32_t file_id = packet->data.Read<uint32_t>();
	uint32_t unkw_id = packet->data.Read<uint32_t>();

	StreamUtility stream = StreamUtility();
	
	bool onError = false;
	File* file_ptr = FileMgr::getSingleton()->GetFile(file_id, onError);

	if(onError)
	{
		printl("Client requested an unknown file.\n");

		stream.Write<uint8_t>(0x00); // Failed
		m_security.Send(DOWNLOAD_COMPLETED, stream);
		SendQueue();
		return;
	}

	// open the file (readonly)
	std::ifstream ifs (file_ptr->FullPath.string(), std::ios::binary );

	uint32_t position = 0;
	while((position != file_ptr->Size))
	{
		uint32_t calcSize = 0;

		if((file_ptr->Size - position) >= DownloadConfig::BufferSize())
			calcSize = DownloadConfig::BufferSize();
		else
			calcSize = file_ptr->Size - position;

		char* part = new char[calcSize];
		ifs.read(part, calcSize);

		stream.Write<char>(part, calcSize);
		position += calcSize;

		m_security.Send(DOWNLOAD_PART, stream);
		stream.Clear();
		SendQueue();
	}
	ifs.close();

	stream.Write<uint8_t>(0x01); // Completed
	m_security.Send(DOWNLOAD_COMPLETED, stream);
	SendQueue();
}