///////////////////////////////////////////////////////////////////////////
// DarkEmu: Party listening
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Text;
using Framework;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        void ListPartyMatching(List<party> pt)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode
            Writer.Create(Systems.SERVER_SEND_PARTYLIST);
            //Write static bytes
            Writer.Byte(1);
            Writer.Byte(4);
            Writer.Byte(0);
            //Write total count of partys
            Writer.Byte(pt.Count);
            //If party count higher is then zero
            if (pt.Count > 0)
            {
                //Repeat for each party in list of party's
                foreach (party currpt in pt)
                {
                    //Get player information using leaderid
                    Systems s = Systems.GetPlayer(currpt.LeaderID);
                    //Write party id
                    Writer.DWord(currpt.ptid);
                    //Write leader id
                    Writer.DWord(currpt.LeaderID);
                    //Write charactername
                    Writer.Text(s.Character.Information.Name);
                    //Write static byte 1
                    Writer.Byte(1);
                    //Write current party players count
                    Writer.Byte(currpt.Members.Count);
                    //Write party type
                    Writer.Byte(currpt.Type);
                    //Write party purpose
                    Writer.Byte(currpt.ptpurpose);
                    //Write min level required
                    Writer.Byte(currpt.minlevel);
                    //Write max level to join the party
                    Writer.Byte(currpt.maxlevel);
                    //Write party name
                    Writer.Text3(currpt.partyname);
                }
            }
            //Send bytes to the client
            client.Send(Writer.GetBytes());
        }
    }
}
