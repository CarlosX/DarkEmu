///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Packet
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Horse Stats
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] Pet_Information(int id, int model, int hp, int charid, pet_obj o)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PET_INFORMATION);  //Select opcode
            Writer.DWord(id);                               //Horse ID
            Writer.DWord(model);                            //Horse Model
            Writer.DWord(hp);                               //Horse HP
            Writer.DWord(hp);                               //Horse SEC HP
            Writer.Byte(0);                                 //Static byte ?
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Player move up to horse
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] Player_UpToHorse(int ownerID, bool type, int petID)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_UPTOHORSE); //Select opcode
            Writer.Byte(1);                                 //Static byte
            Writer.DWord(ownerID);                          //Horse Owner ID
            Writer.Byte(type);                              //Horse Type
            Writer.DWord(petID);                            //Horse ID
            return Writer.GetBytes();
        }
    }
}
