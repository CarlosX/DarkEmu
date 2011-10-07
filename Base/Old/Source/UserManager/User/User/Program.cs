/*    <DarkEmu UserManager>
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
using System.Linq;
using System.Text;

namespace UserManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine("Connecting to the database!");
            Database.Connect(Settings.ReadSettings("Settings.txt"));
            Console.WriteLine("Create a new user account:\ntype in your new accountname:");
            string user = Console.ReadLine();
            Console.WriteLine("type in your password:");
            string password = Console.ReadLine();
            Console.WriteLine("Writing to the database...");
            Database.ChangeData("INSERT INTO user (name, password) VALUE ('" + user + "','" + password +  "')");
            Console.WriteLine("Finished!\nPress enter to exit.");
            Console.ReadLine();
        }
    }
}
