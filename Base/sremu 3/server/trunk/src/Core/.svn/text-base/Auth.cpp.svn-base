#include "GameSocket.h"

void GameSocket::Init() 
{
	Writer.Create(GAME_SERVER_HANDSHAKE);
		Writer.WriteByte(1);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());
}

void GameSocket::SendServerInfo() 
{
	unsigned char name[] = "AgentServer";
	int namelen = strlen((char*)name);

	Writer.Create(GAME_SERVER_INFO);
		Writer.WriteWord(namelen);
		Writer.WriteString(name, namelen);
		Writer.WriteByte(0);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());
}

void GameSocket::SendPatchInfo() 
{
	Writer.Create(GAME_SERVER_PATCH_INFO);
		Writer.WriteDWord(0x05000101);
		Writer.WriteByte(0x20);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(GAME_SERVER_PATCH_INFO);
		Writer.WriteDWord(0x01000100);
		Writer.WriteDWord(0x00050628);
		Writer.WriteWord(0);
		Writer.WriteByte(2);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(GAME_SERVER_PATCH_INFO);
		Writer.WriteDWord(0x05000101);
		Writer.WriteByte(0x60);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(GAME_SERVER_PATCH_INFO);
		Writer.WriteDWord(0x02000300);
		Writer.WriteWord(0x0200);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(GAME_SERVER_PATCH_INFO);
		Writer.WriteDWord(0x00000101);
		Writer.WriteByte(0xA1);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	Writer.Create(GAME_SERVER_PATCH_INFO);
		Writer.WriteWord(0x0100);
	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());
}

void GameSocket::OnAuth() 
{
	unsigned int session = Reader.ReadDWord();

	unsigned short userlen = Reader.ReadWord();
	char* user = new char[userlen+1];
	memset(user, 0x00, userlen+1);
	Reader.ReadString((unsigned char*)user, userlen);

	unsigned short passlen = Reader.ReadWord();
	char* pass	= new char[passlen+1];
	memset(pass, 0x00, passlen+1);
	Reader.ReadString((unsigned char*)pass, passlen);

	Writer.Create(0xA103);

		int id = IsAuthorized(user, pass);
		if(id) {

			Writer.WriteByte(0x01);
			Player.General.AccountID = id;

		} else {

			Writer.WriteByte(0x02);
			Writer.WriteByte(0x01);

		}

	Writer.Finalize();
	Send(Writer.Buffer, Writer.Size());

	delete[] user;
	delete[] pass;
}

int GameSocket::IsAuthorized(char* username, char* password) 
{
	mysqlpp::StoreQueryResult res = 
		db.Query("select * from users where username='%s' and password='%s'", username, password).store();
	return res.num_rows() == 0 ? 0 : atoi(res[0][0]);
}