///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
// File info: Public packet data
///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Data.SqlClient;

namespace SrxRevo
{
    public partial class Packet
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Movement Packet
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public static byte[] Movement(SrxRevo.Global.vektor p)
        {
           PacketWriter Writer = new PacketWriter();
                Writer.Create(Systems.SERVER_MOVEMENT);     //Select opcode
                Writer.DWord(p.ID);                         //Player ID
                Writer.Bool(true);                          //Bool 1
                Writer.Byte(p.xSec);                        //Player X Sector
                Writer.Byte(p.ySec);                        //Player Y Sector
                if (!File.FileLoad.CheckCave(p.xSec, p.ySec))
                {
                Writer.Word(p.x);                    //Player X Location
                Writer.Word(p.z);                    //Player Z Location
                Writer.Word(p.y);                    //Player Y Location
                }
                else
                {
                    if (p.x < 0)
                    {
                        Writer.Word(p.x);
                        Writer.Word(0xFFFF);
                    }
                    else
                    {
                        Writer.DWord(p.x);
                    }
                    Writer.DWord(p.z);

                    if (p.y < 0)
                    {
                        Writer.Word(p.y);
                        Writer.Word(0xFFFF);
                    }
                    else
                    {
                        Writer.DWord(p.y);
                    }
                }
                Writer.Bool(false);
                return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Movement Pickup
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] MovementOnPickup(SrxRevo.Global.vektor p)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_PICKUPITEM_MOVE);
            Writer.DWord(p.ID);
            Writer.Byte(p.xSec);
            Writer.Byte(p.ySec);
            Writer.Float(p.x);
            Writer.Float(p.z);
            Writer.Float(p.y);
            Writer.Word(0);
            return Writer.GetBytes();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Movement Angle
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] Angle(int id, ushort angle)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ANGLE);    //Select opcode
            Writer.DWord(id);                       //Character ID
            Writer.Word(angle);                     //Angle
            return Writer.GetBytes();
        }
    }
}
