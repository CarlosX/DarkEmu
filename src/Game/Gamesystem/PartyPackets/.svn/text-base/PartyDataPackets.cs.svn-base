///////////////////////////////////////////////////////////////////////////
// SRX Revo: Party data packets
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Packet
    {
        public static byte[] JoinResponseMessage(short type)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode
            Writer.Create(Systems.SERVER_PARTY_MESSAGES);
            //Add static byte
            Writer.Byte(1);
            //Add type short (1 = joined , 2 = no reponse)
            Writer.Word(type);
            //Return bytes to client
            return Writer.GetBytes();
        }
        //If the player is party leader send this.
        public static byte[] PartyOwnerInformation(int leaderid)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode
            Writer.Create(Systems.SERVER_PARTY_OWNER);
            //Static byte
            Writer.Byte(1);
            //Write leader id integer (deword).
            Writer.DWord(leaderid);
            //Return all bytes for sending
            return Writer.GetBytes();
        }
        //If the player is a party member send this.
        public static byte[] Party_Member(int memberid)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode
            Writer.Create(Systems.SERVER_PARTY_MEMBER);
            //Static byte
            Writer.Byte(1);
            //Write member id (int) Dword
            Writer.DWord(memberid);
            //Return all bytes for sending
            return Writer.GetBytes();
        }
        public static byte[] Party_DataMember(party p)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode
            Writer.Create(Systems.SERVER_PARTYMEMBER_DATA);
            //Write static byte
            Writer.Byte(0xFF);
            //Write party id
            Writer.DWord(p.ptid);
            //Write leader id
            Writer.DWord(p.LeaderID);
            //Write byte party type
            Writer.Byte(p.Type);
            //Write total amount of members in party
            Writer.Byte(p.Members.Count);
            //Repeat for each member in party -1
            for (byte b = 0; b <= p.Members.Count -1; b++)
            {
                //Get player detail information
                Systems PartyMemberInfo = Systems.GetPlayer(p.Members[b]);
                //Calculate hp and mp
                int partyPercentMP = (int)Math.Round((decimal)(PartyMemberInfo.Character.Stat.SecondMP * 10) / PartyMemberInfo.Character.Stat.Mp) << 4;
                int partyPercentHP = (int)Math.Round((decimal)(PartyMemberInfo.Character.Stat.SecondHp * 10) / PartyMemberInfo.Character.Stat.Hp);
                //Set percent
                int partyPercent = partyPercentHP | partyPercentMP;
                //Write static byte
                Writer.Byte(0xff);
                //Write unique member id
                Writer.DWord(PartyMemberInfo.Character.Information.UniqueID);
                //Write character name
                Writer.Text(PartyMemberInfo.Character.Information.Name);
                //Write character model
                Writer.DWord(PartyMemberInfo.Character.Information.Model);
                //Write character level
                Writer.Byte(PartyMemberInfo.Character.Information.Level);
                //Write stat hp mp information
                Writer.Byte((byte)partyPercent);
                //Write x and y sector
                Writer.Byte(PartyMemberInfo.Character.Position.xSec);
                Writer.Byte(PartyMemberInfo.Character.Position.ySec);
                //Write x z y
                Writer.Word(Formule.packetx(PartyMemberInfo.Character.Position.x, PartyMemberInfo.Character.Position.xSec));
                Writer.Word(PartyMemberInfo.Character.Position.z);
                Writer.Word(Formule.packety(PartyMemberInfo.Character.Position.y, PartyMemberInfo.Character.Position.ySec));
                //Write double word 1
                Writer.Word(1);
                Writer.Word(1);
                //If player has a guild
                if (PartyMemberInfo.Character.Network.Guild.Name != null)
                    //Write guild name
                    Writer.Text(PartyMemberInfo.Character.Network.Guild.Name);
                //If player has no guild
                else
                    //Write word 0 value
                    Writer.Word(0);
                //Write static byte
                Writer.Byte(0);
                //Write dword 
                Writer.DWord(0);
                Writer.DWord(0);
            }
            //Return all bytes for sending
            return Writer.GetBytes();
        }

        public static byte[] Party_Data(byte type, int id)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode
            Writer.Create(Systems.SERVER_PARTY_DATA);
            //Write type byte
            Writer.Byte(type);
            //Create switch for type given
            switch (type)
            {
                case 1:
                    //Remove party state
                    Writer.Byte(0x0b);
                    Writer.Byte(0);
                    //Return all bytes for sending
                    return Writer.GetBytes();
                case 2:
                    //Formed party new member update
                    Systems CharacterInformation = Systems.GetPlayer(id);
                    //Write static byte
                    Writer.Byte(0xFF);
                    //Write unique character id
                    Writer.DWord(CharacterInformation.Character.Information.UniqueID);
                    //Write character name
                    Writer.Text(CharacterInformation.Character.Information.Name);
                    //Write character model
                    Writer.DWord(CharacterInformation.Character.Information.Model);
                    //Write character level
                    Writer.Byte(CharacterInformation.Character.Information.Level);
                    //Write static byte
                    Writer.Byte(0xAA);
                    //Write x and y sector
                    Writer.Byte(CharacterInformation.Character.Position.xSec);
                    Writer.Byte(CharacterInformation.Character.Position.ySec);
                    //Write x z y
                    Writer.Word(Formule.packetx(CharacterInformation.Character.Position.x, CharacterInformation.Character.Position.xSec));
                    Writer.Word(CharacterInformation.Character.Position.z);
                    Writer.Word(Formule.packety(CharacterInformation.Character.Position.y, CharacterInformation.Character.Position.ySec));
                    //Write double word 1
                    Writer.Word(1);
                    Writer.Word(1);
                    //If character is in a guild
                    if (CharacterInformation.Character.Network.Guild.Name != null)
                        //Write guild name
                        Writer.Text(CharacterInformation.Character.Network.Guild.Name);
                    //If character has no guild
                    else
                        //Write 0 word value
                        Writer.Word(0);
                    //Static byte
                    Writer.Byte(0);
                    //Permissions
                    Writer.DWord(0);
                    Writer.DWord(0);
                    //Return all bytes for sending
                    return Writer.GetBytes();
                case 3:
                    //Write character id
                    Writer.DWord(id);
                    //Write static byte 4
                    Writer.Byte(4);
                    //Return all bytes for sending
                    return Writer.GetBytes();
                case 6:
                    //Update player location and stat
                    CharacterInformation = Systems.GetPlayer(id);
                    //Calculate hp and mp
                    int partyPercentMP = (int)Math.Round((decimal)(CharacterInformation.Character.Stat.SecondMP * 10) / CharacterInformation.Character.Stat.Mp) << 4;
                    int partyPercentHP = (int)Math.Round((decimal)(CharacterInformation.Character.Stat.SecondHp * 10) / CharacterInformation.Character.Stat.Hp);
                    //Set percent information
                    int partyPercent = partyPercentHP | partyPercentMP;
                    //Write character id
                    Writer.DWord(id);
                    //If character is moving
                    if (CharacterInformation.Character.Position.Walking)
                    {
                        //Write byte 20
                        Writer.Byte(0x20);
                        //Write location information
                        Writer.Byte(CharacterInformation.Character.Position.packetxSec);
                        Writer.Byte(CharacterInformation.Character.Position.packetySec);
                        Writer.Word(CharacterInformation.Character.Position.packetX);
                        Writer.Word(CharacterInformation.Character.Position.packetZ);
                        Writer.Word(CharacterInformation.Character.Position.packetY);
                        //Write double word 1
                        Writer.Word(1);
                        Writer.Word(1);
                    }
                    //If not walking
                    else
                    {
                        //Write static byte 4
                        Writer.Byte(4);
                        //Write hp mp information
                        Writer.Byte((byte)partyPercent);
                    }
                    //Return all bytes for sending
                    return Writer.GetBytes();
                case 9:
                    //New leader id
                    Writer.DWord(id);
                    //Return all bytes for sending
                    return Writer.GetBytes();
            }
            //Return all bytes for sending
            return Writer.GetBytes();
        }
    }
}