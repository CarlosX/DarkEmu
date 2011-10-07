///////////////////////////////////////////////////////////////////////////
// PROJECT SRX
// HOMEPAGE: WWW.XCODING.NET
///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using LoginServer.Global;
using System.Text;

namespace LoginServer
{
    class Boot_Logo
    {
        public static void Load()
        {
            Console.Title = "..:: "+ Product.Productname +" ::.. Version : "+ Versions.appVersion +"";
            _Free();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("################################################################################");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                       ######  ########  ##     ##");
            Console.WriteLine("                      ##    ## ##     ##  ##   ## ");
            Console.WriteLine("                      ##       ##     ##   ## ##  ");
            Console.WriteLine("                       ######  ########     ###   ");
            Console.WriteLine("                            ## ##   ##     ## ##  ");
            Console.WriteLine("                      ##    ## ##    ##   ##   ## ");
            Console.WriteLine("                       ######  ##     ## ##     ##");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("             Project version : {0} , Client version : 1.{1}", Versions.appVersion,Versions.clientVersion);
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("################################################################################");
            Console.ForegroundColor = ConsoleColor.Green;
            
        }
        public static void _Free()
        {
            for (int i = 0; i < 20; i++)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  This software is free, and should not be sold by anyone! http://xcoding.net");
                System.Threading.Thread.Sleep(100);
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  This software is free, and should not be sold by anyone! http://xcoding.net");
                System.Threading.Thread.Sleep(100);
                Console.Clear();
            }
        }
    }
}
