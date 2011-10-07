///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Packet
    {
        ///////////////////////////////////////////////////////////////////////////
        // Despawn Pet
        ///////////////////////////////////////////////////////////////////////////
        public static byte[] PetSpawn(int petid, byte type, pet_obj o)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PLAYER_SPAWN_PET);   //Select Opcode
            Writer.DWord(petid);                              //Pet ID
            switch (type)
            {
                case 1:
                    //Despawn pet
                    Writer.Byte(1);
                    break;
                case 2:
                    //Rename pet respawn
                    Writer.Byte(5);
                    Writer.Text(o.Petname);
                    break;
                case 3:
                    //Attack pet respawn for exp info
                    Writer.Byte(3);//Static
                    Writer.LWord(o.exp);//New exp
                    Writer.DWord(o.OwnerID);//Owner id
                    break;
            }
            return Writer.GetBytes();
        }
        ///////////////////////////////////////////////////////////////////////////
        // Grab pet information packet
        ///////////////////////////////////////////////////////////////////////////
        public static byte[] Pet_Information_grab(pet_obj o, byte slot)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PET_INFORMATION);
            try
            {
                //////////////////////////////////////////////////////////////////////////////////////
                // Grabpet structure
                //////////////////////////////////////////////////////////////////////////////////////
                Writer.DWord(o.UniqueID);                   //Unique ID
                Writer.DWord(o.Model);                      //Pet Model
                Writer.DWord(0x00006D);                     //Settings
                Writer.DWord(0x00006D);                     //Settings
                Writer.DWord(0x000047);                     //Settings 0x47 grab pet active 0 disabled
                if (o.Petname != "No name")                 //###############
                    Writer.Text(o.Petname);                 // Name region
                else                                        //
                    Writer.Word(0);                         //###############
                Writer.Byte(o.Slots);                       //Slots count inventory pet
                //////////////////////////////////////////////////////////////////////////////////////
                // Grabpet item inventory
                //////////////////////////////////////////////////////////////////////////////////////
                Systems.MsSQL ms = new Systems.MsSQL("SELECT * FROM char_items WHERE owner='" + o.OwnerID + "' AND pet_storage_id='" + o.UniqueID + "'");
                Writer.Byte(ms.Count());
                using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                {
                    while (reader.Read())
                        Item.AddItemPacket(Writer, reader.GetByte(5), reader.GetInt32(2), reader.GetByte(4), reader.GetInt16(6), reader.GetInt32(7), reader.GetInt32(0), reader.GetInt32(9), reader.GetInt32(30));
                }
                ms.Close();
                //////////////////////////////////////////////////////////////////////////////////////
                // Other
                //////////////////////////////////////////////////////////////////////////////////////
                Writer.DWord(o.OwnerID);                    //Character ID
                Writer.Byte(slot);                          //Slot location of the pet
                //////////////////////////////////////////////////////////////////////////////////////
            }
            catch (Exception ex)
            {
                Console.WriteLine("Pet load error: " + ex);
            }
            return Writer.GetBytes();
        }
        ///////////////////////////////////////////////////////////////////////////
        // Grab pet settings
        ///////////////////////////////////////////////////////////////////////////
        public static byte[] ChangePetSettings(byte option, int petid, byte type, int settings)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_GPET_SETTINGS);
            Writer.Byte(option);
            Writer.DWord(petid);
            Writer.Byte(type);
            Writer.DWord(settings);
            return Writer.GetBytes();
        }
    }
}
