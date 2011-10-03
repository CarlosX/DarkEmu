﻿/*    <DarkEmu GameServer>
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
using System.Collections;
using System.Text;

namespace DarkEmu_GameServer
{
    class Program
    {
        private static bool exit = false;

        static void Main(string[] args)
        {
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine(" <Silkroad Gameserver>  Copyright (C) <2011>  <DarkEmu>");
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Loading essentiel data and connecting to database.\nThis could take some minutes!");
            Database.Connect(Settings.ReadSettings("Settings.txt"));

            Database.CheckTables(new string[] { "mastery", "user", "characters", "skills", "items" });

            DatabaseCore.SetPulseTime(5000);
            DatabaseCore.SetPulseFlag(false);
            DatabaseCore.SetQueryLocation("tmpQueryGameServer.txt");
            DatabaseCore.Start();

            Console.WriteLine("Data from database loaded.Changes on the database wont effect the server now!");

            Timers.LoadTimers();
            Silkroad.DumpObjects();

            ServerSocket server = new ServerSocket("127.0.0.1", 15780);
            server.Start();

            Console.WriteLine("Use 'help' to get all commands.");
            while (exit == false)
                Commands(Console.ReadLine());

        }

        private static void Commands(string cmd)
        {
            switch (cmd)
            {                    
                case "show c":
                    string tmp = System.IO.File.ReadAllText("GPL.txt");
                    Console.WriteLine("Part 1:(press enter for part 2)");
                    Console.WriteLine(tmp.Substring(0, tmp.Length / 2));
                    Console.ReadLine();
                    Console.WriteLine(tmp.Substring(tmp.Length / 2));
                    break;

                case "help":
                    Console.WriteLine("clear - clears the console\nplayers - shows how many players are online\nnotice - send a global server notice\ndebug - see the packetflow\nexit - exit this application");
                    break;

                case "clear":
                    Console.Clear();
                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine(" <Silkroad Gameserver>  Copyright (C) <2011>  <DarkEmu>\nThis program comes with ABSOLUTELY NO WARRANTY; for details take a look at the \nGPL.txt.This is free software, and you are welcome to redistribute it\nunder certain conditions.For more information look at the GPL.txt or type ‘show c’ for details.");
                    Console.WriteLine("-------------------------------------------------------");
                    break;

                case "players":
                    Console.WriteLine("Player Online:{0}", ServerSocket.CountPlayers());
                    break;

                case "notice":
                    Console.WriteLine("msg:");
                    string msg = Console.ReadLine();
                    if (msg.Length != 0)
                        Chat.SendExternNotice(msg);
                    break;
                case "debug":
                    if (GameSocket.debug)
                        Console.WriteLine("Stopped debugging!");
                    else
                        Console.WriteLine("Started debugging!");
                    GameSocket.debug = !GameSocket.debug;
                    break;
                case "exit":
                    exit = true;
                    Console.WriteLine("Thanks for using. Server is shutting down!");
                    break;
            }
        }
    }
}
