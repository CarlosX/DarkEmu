#include "download_manager.h"
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
DownloadManager::DownloadManager()
{
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void DownloadManager::HandlePacket(PacketContainer* packet)
{
	while(packet->data.Read<uint8_t>() == 0x01) // new file
	{
		File f;
		f.ID	   = packet->data.Read<uint32_t>	 ();
		f.Filename = packet->data.Read_Ascii (packet->data.Read<uint16_t>());
		f.Path     = packet->data.Read_Ascii (packet->data.Read<uint16_t>());
		f.Size     = packet->data.Read<uint32_t>();
		f.Pk2Compressed = packet->data.Read<uint8_t>();

		m_file_map.insert(std::pair<uint32_t, File>(static_cast<uint32_t>(LOWORD(f.ID)), f));
	}
}

std::multimap<uint32_t, File> * DownloadManager::GetMultimap()
{
	return &m_file_map;
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////