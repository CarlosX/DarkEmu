/*    <DarkEmu LoginServer>
    Copyright (C) <2011>  <DarkEmu>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace DarkEmu_LoginServer
{
    class Program
    {
        private static bool exit = false;

        static void Main(string[] args)
        {
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine(" <Silkroad Loginserver>  Copyright (C) <2011>.");
            Console.WriteLine("-------------------------------------------------------");

            Database.Connect(Settings.ReadSettings("Settings.txt"));
            Database.CheckTables(new string[]{"news", "server", "user"});

            DatabaseCore.SetPulseTime(20000);
            DatabaseCore.SetPulseFlag(false);
            DatabaseCore.SetQueryLocation("tmpQueryLoginServer.txt");
            DatabaseCore.Start();

            Console.WriteLine("Data from database loaded, Changes on the database wont effect the server now!");

            ServerSocket server = new ServerSocket("127.0.0.1", 15779);
            server.Start();

            Console.WriteLine("Use 'help' to get all commands.");
            while(exit == false)            
                Commands(Console.ReadLine());                              

        }

        private static void Commands(string cmd)
        {
            switch (cmd)
            { 
                case "help":
                    Console.WriteLine("clear - clears the console\nplayers - shows how many players are online\ncheck - server goes in checking state\ndebug - see the packetflow\nexit - exit this application");
                    break;

                case "clear":
                    Console.Clear();
                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine(" <Silkroad Loginserver>  Copyright (C) <2011>  <DarkEmu>");
                    Console.WriteLine("-------------------------------------------------------");
                    break;

                case "players":
                    Console.WriteLine("Player Online:{0}", ServerSocket.CountPlayers());
                    break;

                case "debug":
                    if (LoginSocket.debug)
                        Console.WriteLine("Stopped debugging!");
                    else
                        Console.WriteLine("Started debugging!");
                    LoginSocket.debug = !LoginSocket.debug;
                    break;
                case "exit":
                    exit = true;                    
                    Console.WriteLine("Thanks for using. Server is shutting down!");
                    break;
            }
        }
    }   
}
