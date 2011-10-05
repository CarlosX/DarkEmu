///////////////////////////////////////////////////////////////////////////
// SRX Revo: Party request packets
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;

namespace SrxRevo
{
    public partial class Packet
    {
        public static byte[] PartyRequest(byte Type, int id, byte type)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode
            Writer.Create(Systems.SERVER_PARTY_REQUEST);
            //Write type byte
            Writer.Byte(Type);
            //Create switch on type
            switch (Type)
            {
                case 6:
                    //Union invite
                    Writer.DWord(id);
                    break;
                case 5:
                    //Guild invitation
                    Writer.DWord(id);
                    Systems InvitedPlayer = Systems.GetPlayer(id);
                    Writer.Word(InvitedPlayer.Character.Information.Name.Length);
                    Writer.String(InvitedPlayer.Character.Information.Name);
                    Writer.Word(InvitedPlayer.Character.Network.Guild.Name.Length);
                    Writer.String(InvitedPlayer.Character.Network.Guild.Name);
                    break;
                case 2:
                    //Party invite
                    Writer.DWord(id);
                    Writer.Byte(type);
                    break;
                case 1:
                    //Exchange invite
                    Writer.DWord(id);
                    break;
            }

            return Writer.GetBytes();
        }
    }
}