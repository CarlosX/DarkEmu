///////////////////////////////////////////////////////////////////////////
// DarkEmu: Boot logo
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DarkEmu_GameServer.Systemcore.Definitions
{
    class Bootlogo
    {
        public static void _Load()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Title = "DarkEmu GameServer " + Global.Versions.appVersion;
            Console.WriteLine("Loading...");
        }
    }
}
