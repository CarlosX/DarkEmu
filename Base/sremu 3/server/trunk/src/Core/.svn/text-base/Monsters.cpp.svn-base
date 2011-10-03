#include "GameSocket.h"

void GameSocket::SpawnMonster(const _Position* pos, unsigned int id) 
{
	Writer.Create(GAME_SERVER_SPAWN);
		Writer.WriteDWord(id);
		Writer.WriteDWord(0x1337);	// Unique monster ID
		Writer.WriteByte (pos->XSector);
		Writer.WriteByte (pos->YSector);
		Writer.WriteFloat(pos->X);
		Writer.WriteFloat(pos->Z);
		Writer.WriteFloat(pos->Y);
		Writer.WriteWord (0);		// Angle
		Writer.WriteByte (0);		// Walk flag?
		Writer.WriteByte (1);
		Writer.WriteByte (0);
		Writer.WriteWord (0);		// Angle
		Writer.WriteWord (1);	
		Writer.WriteByte (0);		// Berserk
		Writer.WriteFloat(5.0f);	// Walk speed
		Writer.WriteFloat(5.0f);	// Run speed
		Writer.WriteFloat(5.0f);	// Berserk speed
		Writer.WriteByte (0);
		Writer.WriteByte (0);
	Writer.Finalize();
	Broadcast(true);
}