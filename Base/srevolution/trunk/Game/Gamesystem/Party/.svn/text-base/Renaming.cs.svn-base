///////////////////////////////////////////////////////////////////////////
// SRX Revo: Party renaming
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Text;
using Framework;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        void RenameParty()
        {
            try
            {
                //Create new packet reader
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                //First integer is party id
                int partyid = reader.Int32();
                //Second integer is not needed
                int NotNeeded = reader.Int32();
                //Byte party type
                byte ptype = reader.Byte();
                //Byte party purpose
                byte purpose = reader.Byte();
                //Byte minimum level
                byte minlevel = reader.Byte();
                //Byte max level to enter party
                byte maxlevel = reader.Byte();
                //Party name lenght
                short namel = reader.Int16();
                //Party name each character is a word value using text3
                string pname = reader.Text3();
                //Create new packet writer
                PacketWriter Writer = new PacketWriter();
                //Add opcode to server packet
                Writer.Create(Systems.SERVER_PARTY_CHANGENAME);
                //Write static byte 1
                Writer.Byte(1);
                //Write party id
                Writer.DWord(partyid);
                //Write dword 0
                Writer.DWord(0);
                //Write party type
                Writer.Byte(ptype);
                //Write party purpose
                Writer.Byte(purpose);
                //Write party minimum level
                Writer.Byte(minlevel);
                //Write party max level
                Writer.Byte(maxlevel);
                //Write party name
                Writer.Text3(pname);
                //Send bytes to client
                client.Send(Writer.GetBytes());
            }
            //If a error happens
            catch (Exception ex)
            {
                //Write the exception to the log
                Systems.Debugger.Write(ex);
            }
        }
    }
}
