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
using System.Data;
using MySql.Data.MySqlClient;

namespace DarkEmu_LoginServer
{
    class Database
    {
        private static MySqlConnection connection;
        private static MySqlDataAdapter adapter;

        private static void Connect(string DBIp, int DBPort, string DB, string DBUsername, string DBPassword)
        {
            try
            {
                if (connection != null)
                {
                    connection.Close();
                }
                connection = new MySqlConnection(string.Format("server={0};port={1} ;user id={2}; password={3}; database={4}; pooling=false", DBIp, DBPort, DBUsername, DBPassword, DB));
                connection.Open();
                Console.WriteLine("Database connected!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Connect(DatabaseSettings settings)
        {
            Connect(settings.Ip, settings.Port, settings.DatabaseName, settings.User, settings.Password);
        }

        public static DataSet GetDataSet(string Query)
        {
            DataSet tmpDataSet = new DataSet();
            adapter = new MySqlDataAdapter(Query, connection);
            adapter.Fill(tmpDataSet);
            return tmpDataSet;
        }

        public static void ExecuteQueryAsnyc(string MySqlCommand)
        {
            if (MySqlCommand != null)
            {
                Console.WriteLine(MySqlCommand);
                MySqlCommand Command = new MySqlCommand(MySqlCommand, connection);

                IAsyncResult result = Command.BeginExecuteNonQuery();
                while (!result.IsCompleted)
                {
                    System.Threading.Thread.Sleep(10);
                }
                Command.EndExecuteNonQuery(result);
            }
        }

        public static void CheckTables(string[] tables)
        {
            try
            {
                for (int i = 0; i <= tables.Length - 1; i++)
                {
                    MySqlCommand command = new MySqlCommand(string.Format("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'emulator' AND table_name = '{0}'", tables[i]), connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        int count = reader.GetInt32(0);

                        if (count == 1)
                        {
                            Console.WriteLine("Table:{0} is existing!", tables[i]);
                        }
                        else if (count == 0)
                        {
                            Console.WriteLine("Table:{0} is not existing!", tables[i]);
                            Console.WriteLine("Please try to fix the table. If the Emulator cant find the table there will be problems ingame!");
                        }
                    }
                    reader.Close();
                }
            }
            catch (MySqlException ep)
            {
                Console.WriteLine(ep.Message);
            }
        }
    }
}