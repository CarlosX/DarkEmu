#include "client_context.h"

// ---------------------------------------------------------------------

void ClientContext::OnAuthentication(PacketContainer* packet)
{
	// Maybe should cache login stuff. It could speed up the login time.
	uint8_t locale = packet->data.Read<uint8_t>();

	std::string username = packet->data.Read_Ascii(packet->data.Read<uint16_t>());
	std::string password = packet->data.Read_Ascii(packet->data.Read<uint16_t>());

	uint32_t gsID        = packet->data.Read<uint16_t>();

	// Get a new/old connection, a connection which isn't in use for at least 30secs will timeout.
	mysqlpp::Connection* con = DatabasePtr()->GetConnection();
	if(!con)
	{
		printl("Couldn't create a new connection to mysql server.\n");
		return;
	}

	StreamUtility answere = StreamUtility();

	if(DatabasePtr()->ExecuteFunction<uint8_t>(con, "_CheckUser", 2, username, password) == 1)
	{
		// Vaild.
		if(DatabasePtr()->ExecuteFunction<uint8_t>(con, "_IsBanned", 1, username) == 0)
		{
			// not banned
			if(GlobalManager::getSingleton()->IsUserConnected(username))
			{
				answere.Write<uint8_t>(0x02); // auth failed.
				answere.Write<uint8_t>(0x03); // already connected.
			}
			else
			{
				// check whether server is full.
				std::map<uint32_t,Gameserver>::iterator gameserver = GameserverManager::getSingleton()->GetGameserver(gsID);
				if(gameserver != GameserverManager::getSingleton()->End())
				{
					if(gameserver->second.Current == gameserver->second.Maximal)
					{
						StreamUtility fullPacket = StreamUtility();
						fullPacket.Write<uint8_t>(0x04);
						m_security.Send(LOGIN_PHASE_SERVER_FULL, fullPacket);
						SendQueue();

						// Release the connection.
						DatabasePtr()->ReleaseConnection(con);

						return; // finished, client will talk to us later to reconnect :)
					}
				}
				else
				{
					// Release the connection.
					DatabasePtr()->ReleaseConnection(con);

					return; // sent gameserver isn't vaild. should not happen with a real client. 
				}
			

				// so, welcome to the server.
				uint32_t session = GlobalManager::getSingleton()->AddSession(gsID);
				
				answere.Write<uint8_t>	(0x01); // success.
				answere.Write<uint32_t> (session);
				answere.Write<uint16_t> (gameserver->IPAddress.length());
				answere.Write_Ascii		(gameserver->IPAddress);
				answere.Write<uint16_t> (gameserver->Port);
			}
		}
		else
		{
			// todo: check whether ban is over.

			// banned
			uint32_t id = DatabasePtr()->ExecuteFunction<uint32_t>(con, "_GetIdFromUsername", 1, username);
			std::string query = "SELECT * FROM _baninfo WHERE id=";
			query += id;
			query += ";";
			mysqlpp::StoreQueryResult res = con->query(query).store();

			answere.Write<uint8_t>	(0x02); // failed auth
			answere.Write<uint8_t>	(0x02); // failed auth
			answere.Write<uint8_t>	(0x01); // because user is banned.
			answere.Write<uint16_t>	(res[0]["reason"].length());
			answere.Write_Ascii		(res[0]["reason"].data(), res[0]["reason"].length());
			answere.Write<uint16_t>	(res[0]["year"]);
			answere.Write<uint16_t> (res[0]["month"]);
			answere.Write<uint16_t> (res[0]["day"]);
			answere.Write<uint16_t> (res[0]["hour"]);
			answere.Write<uint16_t> (res[0]["minute"]);
			answere.Write<uint16_t> (res[0]["second"]);
			answere.Write<uint32_t> (0);
		}
	}
	else
	{
		// Wrong pw. or may not existing account.
		answere.Write<uint8_t>(0x02); // failed auth.
		answere.Write<uint8_t>(0x01); // wrong account info.
		answere.Write<uint32_t>(0);   // max fails allowed. flood/brute force protection.
		answere.Write<uint32_t>(5);   // todo: implement flood/brute force protection.
	}

	// Send Message
	m_security.Send	(LOGIN_PHASE_AUTH_ANSWERE, answere);
	SendQueue();

	// Release the connection.
	DatabasePtr()->ReleaseConnection(con);
}