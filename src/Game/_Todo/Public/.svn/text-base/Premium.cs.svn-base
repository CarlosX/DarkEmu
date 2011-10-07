///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        /////////////////////////////////////////////////////////////////////////////
        // Handle new premium tickets
        /////////////////////////////////////////////////////////////////////////////
        void HandlePremiumType(int ItemID)
        {
            //First we check if the user allready has a ticket active or not.
            if (!Character.Premium.Active)
            {
                //Set default values that we will use for the ticket.
                DateTime StartTime = DateTime.Now;
                double Timespan = StartTime.Minute;
                byte NewRate = 0;

                //Check which ticket the user has selected.
                if (Data.ItemBase[ItemID].Ticket == Global.item_database.Tickets.GOLD_1_DAY)
                {

                }
                else if (Data.ItemBase[ItemID].Ticket == Global.item_database.Tickets.GOLD_4_WEEKS)
                {
                    NewRate = 2;
                }
                    //Send visual packet for the icon
                    
            }
            else
            {
                //Send message to user that an active ticket excists.
                client.Send(Packet.IngameMessages(Systems.SERVER_TICKET, IngameMessages.UIIT_MSG_PREMIUM_NOT_USE));
            }
        }
    }
}
