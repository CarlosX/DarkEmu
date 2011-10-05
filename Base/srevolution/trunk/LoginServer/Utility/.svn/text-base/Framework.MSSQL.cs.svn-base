using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace LoginServer
{
    public partial class Systems
    {
        public class MsSQL
        {
            #region public
            public static SqlConnection connection;
            public SqlConnection connection1;
            public SqlCommand cmd1;
            public string SQL;
            public SqlDataReader reader;

            public MsSQL(string sql)
            {
                if (reader != null) { Close(); }
                SQL = sql;
            }
            public SqlDataReader Read()
            {
                cmd1 = new SqlCommand(SQL, connection);
                reader = cmd1.ExecuteReader();
                return reader;
            }
            public void Close()
            {
                if (!reader.IsClosed)
                {
                    reader.Dispose();
                    reader.Close();
                }
            }
            public int Count()
            {
                int count = 0;
                try
                {
                    da = new SqlDataAdapter(SQL, connection);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    count = ds.Tables[0].Rows.Count;
                }
                catch (Exception ex)
                {
                    OnDatabaseError(ex);
                }
                return count;
            }

            public string GetString(string column)
            {
                return reader[column].ToString();
            }
            public byte GetByte(string column)
            {
                return Convert.ToByte(reader[column].ToString());
            }
            public float GetSingle(string column)
            {
                return Convert.ToSingle(reader[column].ToString());
            }
            public double GetDouble(string column)
            {
                return Convert.ToDouble(reader[column].ToString());
            }
            public short GetInt16(string column)
            {
                return Convert.ToInt16(reader[column].ToString());
            }
            public int GetInt32(string column)
            {
                return Convert.ToInt32(reader[column].ToString());
            }
            public long GetInt64(string column)
            {
                return Convert.ToInt64(reader[column].ToString());
            }

            public float GetDataFloat(string column)
            {
                float Get = 0;
                cmd1 = new SqlCommand(SQL, connection);
                reader = cmd1.ExecuteReader();
                while (reader.Read()) Get = Convert.ToSingle(reader[column].ToString());
                return Get;
            }
            public double GetDataDouble(string column)
            {
                double Get = 0;
                cmd1 = new SqlCommand(SQL, connection);
                reader = cmd1.ExecuteReader();
                while (reader.Read())
                {
                    Get = Convert.ToDouble(reader[column].ToString());
                }
                return Get;
            }

            public delegate void dError(Exception ex);
            public static event dError OnDatabaseError;
            public delegate void dConnected();
            public static event dConnected OnConnectedToDatabase;


            public static SqlDataAdapter da;
            #endregion

            #region Baglanti
            public static void Connection(string connections)
            {
                string Connection = connections;
                if (connection != null)
                    connection.Close();

                try
                {
                    connection = new SqlConnection();

                    connection.ConnectionString = Connection;
                    connection.Open();
                    OnConnectedToDatabase();
                }
                catch (Exception ex)
                {
                    OnDatabaseError(ex);
                }

            }
            #endregion

            #region Single data
            public static string GetData(string sql, string column)
            {
                string GetResults = null;

                SqlDataReader reader = null;
                SqlCommand cmd = new SqlCommand(sql, connection);


                try
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        GetResults = reader[column].ToString();
                    }
                }
                catch (Exception ex)
                {
                    OnDatabaseError(ex);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
                return GetResults;
            }
            public static double GetDataDouble(string sql, string column)
            {
                double GetResults = 0;

                SqlDataReader reader = null;
                SqlCommand cmd = new SqlCommand(sql, connection);


                try
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        GetResults = Convert.ToDouble(reader[column].ToString());
                    }
                }
                catch (Exception ex)
                {
                    OnDatabaseError(ex);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
                return GetResults;
            }
            public static int GetDataInt(string sql, string column)
            {
                int GetResults = 0;

                SqlDataReader reader = null;
                SqlCommand cmd = new SqlCommand(sql, connection);

                try
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        GetResults = Convert.ToInt32(reader[column].ToString());
                    }
                }
                catch (Exception ex)
                {
                    OnDatabaseError(ex);
                }
                finally
                {
                    reader.Close();
                }
                return GetResults;
            }
            public static long GetDataLong(string sql, string column)
            {
                long GetResults = 0;

                SqlDataReader reader = null;
                SqlCommand cmd = new SqlCommand(sql, connection);


                try
                {
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        GetResults = Convert.ToInt64(reader[column].ToString());
                    }
                }
                catch (Exception ex)
                {
                    OnDatabaseError(ex);
                }
                finally
                {
                    reader.Close();
                }
                return GetResults;
            }
            #endregion

            #region Rows Count
            public static int GetRowsCount(string command)
            {
                int count = 0;
                try
                {
                    da = new SqlDataAdapter(command, connection);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    count = ds.Tables[0].Rows.Count;
                }
                catch (Exception ex)
                {
                    OnDatabaseError(ex);
                }
                return count;
            }
            #endregion

            #region Insert Data
            public static void InsertData(string command)
            {
                SqlCommand cmd = new SqlCommand(command, connection);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    OnDatabaseError(ex);
                }
            }
            #endregion

            #region Update Data

            public static void UpdateData(string command)
            {
                InsertData(command);
            }

            #endregion

            #region Delete Data

            public static void DeleteData(string command)
            {
                InsertData(command);
            }

            #endregion

            public static string CharacterData(string charname, string toGet)
            {
                string result;
                result = MsSQL.GetData("SELECT * FROM karakterler WHERE name='" + charname + "'", toGet);
                return result;
            }

            public static int CharacterDataInt(string charname, string toGet)
            {
                int result;
                result = MsSQL.GetDataInt("SELECT * FROM karakterler WHERE name='" + charname + "'", toGet);
                return result;
            }
        }
    }
}
