///////////////////////////////////////////////////////////////////////////
// DarkEmu: guild icon system
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        void HandleRegisterIcon()
        {
            try
            {
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                byte type = reader.Byte();
                int iconlenght = reader.Int32();
                string icon = reader.Text();
                reader.Close();

                string convertedicon = ConvertToHex(icon);
                //Save output to .dat file in hex formatting.
            }
            catch (Exception ex)
            {
                Console.WriteLine("Guild icon register error {0}", ex);
            }
        }

        string ConvertToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.(UInt32)(tmp.ToString()));
            }
            return hex;
        }
    }
}