///////////////////////////////////////////////////////////////////////////
// SRX Revo: Party joining packets
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Text;
using Framework;

namespace SrxRevo
{
    public partial class Packet
    {
        public static byte[] CreateFormedParty(party pt)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode to packet
            Writer.Create(Systems.SERVER_FORMED_PARTY_CREATED);
            //Static byte
            Writer.Byte(1);
            //Party id
            Writer.DWord(pt.ptid);
            //0 Dword value
            Writer.DWord(0);
            //Party type
            Writer.Byte(pt.Type);
            //Party purpose
            Writer.Byte(pt.ptpurpose);
            //Party min level required
            Writer.Byte(pt.minlevel);
            //Party max level allowed
            Writer.Byte(pt.maxlevel);
            //Party name
            Writer.Text3(pt.partyname);
            //Return all bytes to send
            return Writer.GetBytes();
        }

        public static byte[] JoinFormedRequest(character requesting, character owner)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode for packet
            Writer.Create(Systems.SERVER_PARTY_JOIN_FORMED);
            //Character model information (Req).
            Writer.DWord(requesting.Information.Model);
            //Leader id
            Writer.DWord(requesting.Information.UniqueID);
            //Party id
            Writer.DWord(owner.Network.Party.ptid);
            //Static
            Writer.DWord(0);
            Writer.DWord(0);
            Writer.Byte(0);
            Writer.Byte(0xFF);
            //Write character unique id
            Writer.DWord(requesting.Information.UniqueID);
            //Write character name
            Writer.Text(requesting.Information.Name);
            //Write model information
            Writer.DWord(requesting.Information.Model);
            //Write level information
            Writer.Byte(requesting.Information.Level);
            //Static
            Writer.Byte(0xAA);
            //X and Y Sector
            Writer.Byte(requesting.Position.xSec);
            Writer.Byte(requesting.Position.ySec);
            //Static
            Writer.Word(0);
            Writer.Word(0);
            Writer.Word(0);
            Writer.Word(1);
            Writer.Word(1);
            //If character is in a guild
            if (requesting.Network.Guild != null)
                //Write guild name
                Writer.Text(requesting.Network.Guild.Name);
            //If character is not in a guild
            else
                //Write word value 0
                Writer.Word(0);
            //Static
            Writer.Byte(0);
            Writer.DWord(0);
            Writer.DWord(0);
            //Return all bytes to send
            return Writer.GetBytes();
        }
    }
}