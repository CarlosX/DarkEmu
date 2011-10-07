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
        public static byte[] AttackPetStats(pet_obj c,byte slot)
        {
            PacketWriter writer = new PacketWriter();
            writer.Create(Systems.SERVER_PET_INFORMATION);
            writer.DWord(c.UniqueID);
            writer.DWord(c.Model);
            writer.DWord(0x00000168);//stats
            writer.DWord(0x00000168);//stats
            writer.LWord(c.exp);//Experience
            writer.Byte(c.Level);//Level
            writer.Word(0);//Angle
            writer.DWord(0x00000001);//1 = Attack icon enabled, 2 = disabled
            if (c.Petname != "No name") writer.Text(c.Petname);//Petname
            else 
                writer.Word(0);//No name

            writer.Byte(0);//Static perhaps
            writer.DWord(c.OwnerID);//Owner
            writer.Byte(slot);
            return writer.GetBytes();
        }
        public static byte[] AttackPetHGP(pet_obj c)
        {
            PacketWriter writer = new PacketWriter();
            writer.Create(Systems.SERVER_PET_HGP);
            writer.DWord(c.UniqueID);
            writer.Byte(3);
            writer.Byte(0);
            writer.Byte(0);
            return writer.GetBytes();
        }
    }
}
