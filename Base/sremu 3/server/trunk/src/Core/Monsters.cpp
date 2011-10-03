#include "GameSocket.h"

void GameSocket::SpawnMonster(const _Position* pos, unsigned int id) 
{
	unsigned char num2 = Reader.ReadByte();
	int num3;
	if (num2 == 1)
    {
      num2 = 0;
    }
   num3 = onlineMob + 0x1337;
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
		Writer.WriteFloat(5.0f);	   // Walk speed
		Writer.WriteFloat(5.0f);	   // Run speed
		Writer.WriteFloat(5.0f);	// Berserk speed
		Writer.WriteByte (0);
		Writer.WriteByte (0);
		Writer.WriteByte (num2);
		Writer.WriteByte (1);

	Writer.Finalize();
	Broadcast(true);
	onlineMob += 0x1337;
	//printf("SpawnMonster enviado \n");
}
void GameSocket::SpawnMnstll(unsigned int id, unsigned char posx, unsigned char posy, float fx, float fz, float fy)
{
   	unsigned char num2 = Reader.ReadByte();
	int num3;
	if (num2 == 1)
    {
      num2 = 0;
    }
	onlineMob += 1;
	num3 = onlineMob + 0x1337;
	Writer.Create(GAME_SERVER_SPAWN);
		Writer.WriteDWord(id);
		Writer.WriteDWord(num3);	// Unique monster ID 0x1337
		Writer.WriteByte (posx);
		Writer.WriteByte (posy);
		Writer.WriteFloat(fx);
		Writer.WriteFloat(fz);
		Writer.WriteFloat(fy);
		Writer.WriteWord (0);		// Angle
		Writer.WriteByte (0);		// Walk flag?
		Writer.WriteByte (1);
		Writer.WriteByte (0);
		Writer.WriteWord (0);		// Angle
		Writer.WriteWord (1);	
		Writer.WriteByte (0);		// Berserk
		Writer.WriteFloat(5.0f);	   // Walk speed
		Writer.WriteFloat(5.0f);	   // Run speed
		Writer.WriteFloat(5.0f);	// Berserk speed
		Writer.WriteByte (0);
		Writer.WriteByte (0);
		Writer.WriteByte (num2);
		Writer.WriteByte (1);
    
	Writer.Finalize();
	Broadcast(true);
	int dataint = db.Query("SELECT * FROM monsters WHERE ID=%d Hp", id).store().num_rows();
	switch (num2)
	{
		case 2:
             dataint *= 2;
             break;

        case 4:
             dataint *= 20;
             break;

        case 6:
             dataint *= 4;
             break;

        case 0x10:
             dataint *= 10;
             break;

        case 0x11:
             dataint *= 20;
             break;

        case 20:
             dataint *= 200;
             break;
	}

	/*MobID = id;
	MobUniqueID = num3;
	MobType = num2;
	MobX = fx;
	MobZ = fz;
	MobY = fy;
	MobxSector = posx;
	MobySector = posy;
	MobHP = dataint;
	MobIndex = onlineMob;*/

    //onlineMob += 0x1337;
}
void GameSocket::SpawnAllM()
{
  
	/*unsigned int id = 1933;

    _Position* fc = new _Position;
	fc->XSector = 168;
	fc->YSector = 96;
	fc->X = 1318;
	fc->Y = 1221;
	fc->Z = 65530;

	SpawnMnstll(fc, id);
    delete fc;*/

	//           id   Xsec Ysec  X     Z      Y
    SpawnMnstll( 1933, 168, 96, 1318, 65530, 1221);
    SpawnMnstll( 1933, 168, 96, 1405, 3, 1242);
	SpawnMnstll( 1954, 168, 96, 1376, 65535, 1218);
    printf("Auto SpawnMonster\n");
  
}
void GameSocket::MovementMonster()
{

    
}