///////////////////////////////////////////////////////////////////////////
// SRX Revo: Guild War system
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        void GuildWarGold()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int guildid = Reader.Int32();
                Reader.Close();

                if (Character.Guild.GuildWarGold == 0)
                {
                    //Send Packet Message No War Gold Received
                    client.Send(Packet.GuildWarMsg(2));
                }
                else
                {
                    //Sniff packet for war gold
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
    }
}