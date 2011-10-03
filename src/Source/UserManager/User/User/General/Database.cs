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
using System.Data;
using MySql.Data.MySqlClient;

namespace UserManager
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

        public static string GetString(string MySqlCommand, string Column)
        {
            string tmp = null;
            MySqlDataReader DataReader = null;
            MySqlCommand Command = new MySqlCommand(MySqlCommand, connection);
            try
            {
                DataReader = Command.ExecuteReader();
                while (DataReader.Read())
                {
                    tmp = DataReader.GetString(Column);
                }
            }
            finally
            {
                if (DataReader != null)
                {
                    DataReader.Close();
                }
            }
            return tmp;
        }

        public static byte GetByte(string MySqlCommand, string Column)
        {
            byte tmp = 0;
            MySqlDataReader DataReader = null;
            MySqlCommand Command = new MySqlCommand(MySqlCommand, connection);
            try
            {
                DataReader = Command.ExecuteReader();
                while (DataReader.Read())                
                    tmp = (byte)DataReader.GetUInt16(Column);
                
            }
            finally
            {
                if (DataReader != null)                
                    DataReader.Close();
                
            }
            return tmp;
        }

        public static bool GetBool(string MySqlCommand, string Column)
        {
            bool tmp = false;
            MySqlDataReader DataReader = null;
            MySqlCommand Command = new MySqlCommand(MySqlCommand, connection);
            try
            {
                DataReader = Command.ExecuteReader();
                while (DataReader.Read())
                {
                    tmp = DataReader.GetBoolean(Column);
                }
            }
            finally
            {
                if (DataReader != null)
                {
                    DataReader.Close();
                }
            }
            return tmp;
        }

        public static ushort GetUShort(string MySqlCommand, string Column)
        {
            ushort tmp = 0;
            MySqlDataReader DataReader = null;
            MySqlCommand Command = new MySqlCommand(MySqlCommand, connection);
            try
            {
                DataReader = Command.ExecuteReader();
                while (DataReader.Read())
                {
                    tmp = DataReader.GetUInt16(Column);
                }
            }
            finally
            {
                if (DataReader != null)
                {
                    DataReader.Close();
                }
            }
            return tmp;
        }

        public static uint GetUInt(string MySqlCommand, string Column)
        {
            uint tmp = 0;
            MySqlDataReader DataReader = null;
            MySqlCommand Command = new MySqlCommand(MySqlCommand, connection);
            try
            {
                DataReader = Command.ExecuteReader();
                while (DataReader.Read())
                {
                    tmp = DataReader.GetUInt32(Column);
                }
            }
            finally
            {
                if (DataReader != null)
                {
                    DataReader.Close();
                }
            }
            return tmp;
        }

        public static ulong GetULong(string MySqlCommand, string Column)
        {
            ulong tmp = 0;
            MySqlDataReader DataReader = null;
            MySqlCommand Command = new MySqlCommand(MySqlCommand, connection);
            try
            {
                DataReader = Command.ExecuteReader();
                while (DataReader.Read())
                {
                    tmp = DataReader.GetUInt64(Column);
                }
            }
            finally
            {
                if (DataReader != null)
                {
                    DataReader.Close();
                }
            }
            return tmp;
        }


        public static int GetInt(string MySqlCommand, string Column)
        {
            int tmp = 0;
            MySqlDataReader DataReader = null;
            MySqlCommand Command = new MySqlCommand(MySqlCommand, connection);
            try
            {
                DataReader = Command.ExecuteReader();
                while (DataReader.Read())
                {
                    tmp = DataReader.GetInt32(Column);
                }
            }
            finally
            {
                if (DataReader != null)
                {
                    DataReader.Close();
                }
            }
            return tmp;
        }

        public static float GetFloat(string MySqlCommand, string Column)
        {
            float tmp = 0;
            MySqlDataReader DataReader = null;
            MySqlCommand Command = new MySqlCommand(MySqlCommand, connection);
            try
            {
                DataReader = Command.ExecuteReader();
                while (DataReader.Read())
                {
                    tmp = DataReader.GetFloat(Column);
                }
            }
            finally
            {
                if (DataReader != null)
                {
                    DataReader.Close();
                }
            }
            return tmp;
        }

        public static double GetDouble(string MySqlCommand, string Column)
        {
            double tmp = 0;
            MySqlDataReader DataReader = null;
            MySqlCommand Command = new MySqlCommand(MySqlCommand, connection);
            try
            {
                DataReader = Command.ExecuteReader();
                while (DataReader.Read())
                {
                    tmp = DataReader.GetDouble(Column);
                }
            }
            finally
            {
                if (DataReader != null)
                {
                    DataReader.Close();
                }
            }
            return tmp;
        }

        public static void ChangeData(string MySqlCommand)
        {
            MySqlCommand Command = new MySqlCommand(MySqlCommand, connection);
            try
            {
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static int GetRowsCount(string MySqlCommand)
        {
            int i = 0;
            try
            {
                adapter = new MySqlDataAdapter(MySqlCommand, connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                i = dataSet.Tables[0].Rows.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return i;
        }

        public static void CheckTables(string[] tables)
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
    }
}