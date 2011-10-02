#include "client_context.h"

#include <boost/foreach.hpp>

#include "news_mgr.h"
#include "download_manager.h"
#include "global_manager.h"

#include "../common/global_configs.h"
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void ClientContext::OnRequestInformation_Server(PacketContainer* packet)
{
	StreamUtility stream = StreamUtility();
	
	stream.Write<uint16_t>	(13);
	stream.Write_Ascii		("GatewayServer", 13);
	stream.Write<uint8_t>   (0x00);
	
	m_security.Send			(SERVER_INFO_REQUEST,  stream);
	SendQueue();
};

void ClientContext::OnRequestNews(PacketContainer* packet)
{
	uint8_t locale = packet->data.Read<uint8_t>	();

	StreamUtility stream = StreamUtility();

	stream.Write<uint8_t>	(NewsMgr::Count());

	uint8_t actuallyCount;

	if(NewsMgr::Count() > 6)
		actuallyCount = 6;
	else
		actuallyCount = NewsMgr::Count();

	std::list<News>::iterator it ;

	int i = 0;
	for(it = NewsMgr::GetNewest()->begin(); i < actuallyCount; it++)
	{
		stream.Write<uint16_t>		(it->Title.length());
		stream.Write_Ascii			(it->Title         );
		stream.Write<uint16_t>		(it->Text.length() );
		stream.Write_Ascii			(it->Text          );
		
		stream.Write<uint16_t>		(it->Creation.tm_year);
		stream.Write<uint16_t>		(it->Creation.tm_mon );
		stream.Write<uint16_t>		(it->Creation.tm_yday);
		stream.Write<uint16_t>		(it->Creation.tm_hour);
		stream.Write<uint16_t>		(it->Creation.tm_min );
		stream.Write<uint16_t>		(it->Creation.tm_sec );
		stream.Write<uint32_t>		(0                   );		// come on joymax, why the fuck you sent millisecond xD, what a fail.

		i++;
	}

	m_security.Send(SEND_NEWS, stream, false, true); // yay put this as massive, but player spawn not, good job joymax.
	SendQueue();
}

void ClientContext::OnRequestInformation_Download(PacketContainer* packet)
{
	uint8_t		locale  =		packet->data.Read<uint8_t>();
	std::string cl_name =		packet->data.Read_Ascii(	packet->data.Read<uint16_t>());
	uint32_t	version =		packet->data.Read<uint32_t>();

	StreamUtility stream = StreamUtility();
	stream.Write<uint8_t>(0x01);
	stream.Write<uint8_t>(0x00);
	stream.Write<uint8_t>(0x01);
	stream.Write<uint8_t>(0x47);
	stream.Write<uint8_t>(0x01);
	stream.Write<uint8_t>(0x05);
	stream.Write<uint8_t>(0x00);
	stream.Write<uint8_t>(0x00);
	stream.Write<uint8_t>(0x00);
	stream.Write<uint8_t>(0x02);
	m_security.Send(DOWNLOAD_INFO1, stream, false, true);
	stream.Clear();
	SendQueue();

	stream.Write<uint8_t>(0x03);
	stream.Write<uint8_t>(0x00);
	stream.Write<uint8_t>(0x02);
	stream.Write<uint8_t>(0x00);
	stream.Write<uint8_t>(0x02);
	m_security.Send(DOWNLOAD_INFO2, stream, false, true);
	stream.Clear();
	SendQueue();

	// Client is up to date.
	if((GlobalConfigs::Current_Version() == version) || cl_name == "SMC")
	{
		stream.Write<uint8_t>(0x01); // Client is up to date
	}
	// to old, can´t patch that client.
	else if(version < GlobalConfigs::Oldest_Version())
	{
		stream.Write<uint8_t>(0x02); // Not the current version
		stream.Write<uint8_t>(0x05); // to old
	}
	// to new, can`t back patch that client.
	else if(version > GlobalConfigs::Current_Version())
	{
		stream.Write<uint8_t>(0x02); // not the current version
		stream.Write<uint8_t>(0x01); // to new
	}
	// old, but we can patch that client.
	else
	{
		stream.Write<uint8_t>(0x02); // not the current version
		stream.Write<uint8_t>(0x02); // patchable

		// give addess to our dl server.
		stream.Write<uint16_t>(GatewayConfig::DownloadServerAddress().length());
		stream.Write_Ascii    (GatewayConfig::DownloadServerAddress());
		stream.Write<uint16_t>(GatewayConfig::DownloadServerPort());
		stream.Write<uint32_t>(GlobalConfigs::Current_Version());

		// give available files to download (may duplications, but client will download the newest)
		std::multimap<uint32_t, File>::iterator iter;

		for(iter = DownloadManager::getSingleton()->GetMultimap()->begin(); iter != DownloadManager::getSingleton()->GetMultimap()->end(); ++iter)
		{
			if(iter->first >= GlobalConfigs::Current_Version() && iter->first > version)
			{
				stream.Write<uint8_t> (0x01);								// new file
				stream.Write<uint32_t>(iter->second.ID);
				stream.Write<uint16_t>(iter->second.Filename.length());
				stream.Write_Ascii    (iter->second.Filename);
				stream.Write<uint16_t>(iter->second.Path.length());
				stream.Write_Ascii	  (iter->second.Path);
				stream.Write<uint32_t>(iter->second.Size);
				stream.Write<uint8_t> (iter->second.Pk2Compressed);
			}
		}
		stream.Write<uint8_t>         (0x00);
	}

	m_security.Send(DOWNLOAD_INFO3, stream, false, true);
	stream.Clear();
	SendQueue();
};

void ClientContext::OnRequestServerList(PacketContainer* packet)
{
	std::string name     = "Silkroad_Taiwan_Official";

	StreamUtility stream = StreamUtility();
	stream.Write<uint8_t>	(0x01);
	stream.Write<uint8_t>	(0x08);
	stream.Write<uint16_t>	(name.length());     // length of the string below.
	stream.Write_Ascii		(name);
	stream.Write<uint8_t>	(0x00);

	std::map<uint32_t, Gameserver>::iterator iter;

	for(iter  =	GameserverManager::getSingleton()->Begin();
		iter != GameserverManager::getSingleton()->End  ();
		iter ++)
	{
		stream.Write<uint8_t>	(0x01);
		stream.Write<uint16_t>	(iter->second.ID);
		stream.Write<uint16_t>	(iter->second.Name.length());
		stream.Write_Ascii		(iter->second.Name);
		stream.Write<uint16_t>	(iter->second.Current);
		stream.Write<uint16_t>	(iter->second.Maximal);
		stream.Write<uint8_t>	(iter->second.Status);
	}

	stream.Write<uint8_t>	(0x00);
	m_security.Send			(LOGIN_PHASE_SEND_SERVERLIST, stream);
	SendQueue();
}