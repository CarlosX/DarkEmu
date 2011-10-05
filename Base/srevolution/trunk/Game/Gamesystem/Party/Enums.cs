///////////////////////////////////////////////////////////////////////////
// SRX Revo: Party listening
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
        public enum PartyTypes
        {
            NONSHARE_NO_PERMISSION,
            EXPSHARE_NO_PERMISSION,
            ITEMSHARE_NO_PERMISSION,
            FULLSHARE_NO_PERMISSION,
            NONSHARE,
            EXPSHARE,
            ITEMSHARE,
            FULLSHARE
        };

        public enum PartyPurpose
        {
            HUNTING,
            QUEST,
            TRADE,
            THIEF
        };
    }
}
