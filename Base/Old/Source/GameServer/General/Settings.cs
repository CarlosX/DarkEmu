/*    <DarkEmu GameServer>
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
using System.IO;

namespace DarkEmu_GameServer
{
    struct DatabaseSettings
    {
        public string Ip;
        public int Port;
        public string DatabaseName;
        public string User;
        public string Password;
    }

    class Settings
    {
        public static DatabaseSettings ReadSettings(string path)
        {
            string[] tmpString = File.ReadAllLines(path);
            DatabaseSettings tmpSettings = new DatabaseSettings();
            tmpSettings.Ip = tmpString[0].Split(':')[1];
            tmpSettings.Port = Convert.ToInt32(tmpString[1].Split(':')[1]);
            tmpSettings.DatabaseName = tmpString[2].Split(':')[1];
            tmpSettings.User = tmpString[3].Split(':')[1];
            tmpSettings.Password = tmpString[4].Split(':')[1];
            return tmpSettings;
        }
    }
}
