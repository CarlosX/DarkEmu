///////////////////////////////////////////////////////////////////////////
// DarkEmu: Party definitions
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
    }
    public sealed partial class party
    {
        //Party name information
        public string partyname = "";
        //Party id
        public int ptid = 0;
        //Purpose
        public byte ptpurpose;
        //Minimal level
        public byte minlevel;
        //Maximal level
        public byte maxlevel;
    }
}
