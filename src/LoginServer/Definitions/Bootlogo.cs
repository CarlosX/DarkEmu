using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoginServer.Definitions
{
    class Bootlogo
    {
        public static void _Load()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Title = "DarkEmu LoginServer " + Global.Versions.appVersion;
            Console.WriteLine("DarkEmu Login Server");
        }
    }
}
