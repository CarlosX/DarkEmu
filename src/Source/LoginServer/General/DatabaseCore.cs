using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.IO;
using System.Data;

namespace DarkEmu_LoginServer
{
    class DatabaseCore
    {
        private static Timer PulseTimer;
        private static bool PulseFlag;
        public static News_ News;
        public static Server_ Server;
        public static User_ User;

        public static void SetPulseTime(int iTime)
        {
            PulseTimer = new Timer();
            PulseTimer.Interval = iTime;
        }

        public static void SetPulseFlag(bool Flag)
        {
            PulseFlag = Flag;
        }

        public static void Start()
        {
            ExecuteSavedQueries();
            PulseTimer.Elapsed += new ElapsedEventHandler(Pulse);
            News = new News_();
            Server = new Server_();
            User = new User_();
            if (PulseFlag)
                PulseTimer.Start();
            Pulse(null, null);
        }

        private static void Pulse(object sender, EventArgs e)
        {
            ExecuteQuery();
            News.Pulse();
            Server.Pulse();
            User.Pulse();
        }

        public class News_
        {
            public byte NumberOfNews = 0;
            public string[] Head = new string[0];
            public string[] Text = new string[0];
            public byte[] Day = new byte[0];
            public byte[] Month = new byte[0];


            public void Pulse()
            {
                DataSet tmpDataSet = Database.GetDataSet("SELECT * FROM news");

                NumberOfNews = (byte)tmpDataSet.Tables[0].Rows.Count;
                Head = new string[NumberOfNews];
                Text = new string[NumberOfNews];
                Day = new byte[NumberOfNews];
                Month = new byte[NumberOfNews];

                for (int i = 0; i < NumberOfNews; i++)
                {
                    Head[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    Text[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[2].ToString();
                    Day[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[3]);
                    Month[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[4]);
                }

                tmpDataSet.Clear();
            }
        }

        public class Server_
        {
            public byte NumberOfServer = 0;
            public string[] ServerName = new string[0];
            public ushort[] ServerId = new ushort[0];
            public ushort[] CurUser = new ushort[0];
            public ushort[] MaxUser = new ushort[0];
            public byte[] ServerState = new byte[0];
            public string[] ServerIp = new string[0];
            public ushort[] ServerPort = new ushort[0];


            public void Pulse()
            {
                DataSet tmpDataSet = Database.GetDataSet("SELECT * FROM server");
                NumberOfServer = (byte)tmpDataSet.Tables[0].Rows.Count;

                ServerName = new string[NumberOfServer];
                ServerId = new ushort[NumberOfServer];
                CurUser = new ushort[NumberOfServer];
                MaxUser = new ushort[NumberOfServer];
                ServerState = new byte[NumberOfServer];
                ServerIp = new string[NumberOfServer];
                ServerPort = new ushort[NumberOfServer];

                for (int i = 0; i < NumberOfServer; i++)
                {
                    ServerId[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[0]);
                    ServerName[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    CurUser[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[2]);
                    MaxUser[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[3]);
                    ServerState[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[4]);
                    ServerIp[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[5].ToString();
                    ServerPort[i] = Convert.ToUInt16(tmpDataSet.Tables[0].Rows[i].ItemArray[6]);
                }

                tmpDataSet.Clear();
            }

            public int GetIndexById(ushort Id)
            {
                for (int i = 0; i < NumberOfServer; i++)
                {
                    if (ServerId[i] == Id)
                        return i;
                }
                return 0;
            }
        }

        public class User_
        {
            public int NumberOfUsers = 0;
            public string[] User = new string[0];
            public string[] Password  = new string[0];
            public byte[] FailedLogins = new byte[0];
            public bool[] Online = new bool[0];
            public bool[] Banned = new bool[0];

            public void Pulse()
            {
                DataSet tmpDataSet = Database.GetDataSet("SELECT * FROM user");
                NumberOfUsers = (byte)tmpDataSet.Tables[0].Rows.Count;

                User = new string[NumberOfUsers];
                Password = new string[NumberOfUsers];
                FailedLogins = new byte[NumberOfUsers];
                Online = new bool[NumberOfUsers];
                Banned = new bool[NumberOfUsers];

                for (int i = 0; i < NumberOfUsers; i++)
                {
                    User[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    Password[i] = tmpDataSet.Tables[0].Rows[i].ItemArray[2].ToString();
                    FailedLogins[i] = Convert.ToByte(tmpDataSet.Tables[0].Rows[i].ItemArray[3]);
                    Online[i] = Convert.ToBoolean(tmpDataSet.Tables[0].Rows[i].ItemArray[4]);
                    Banned[i] = Convert.ToBoolean(tmpDataSet.Tables[0].Rows[i].ItemArray[10]);
                }

                tmpDataSet.Clear();
            }

            public int GetIndexByName(string UserName)
            {
                for (int i = 0; i < NumberOfUsers; i++)
                {
                    if (User[i] == UserName)
                        return i;
                }
                return 0;
            }
        }

        private static StreamWriter QueryWriter;
        private static string Path;

        public static void SetQueryLocation(string path)
        {
            Path = path;       
        }

        public static void WriteQuery(string Query)
        {
            if (QueryWriter != null)
            {
                QueryWriter.WriteLine(Query);
                QueryWriter.Flush();
            }
            else
                Console.WriteLine("Error:QueryWriter == NULL\t-Could not save the queries.");
        }

        public static void WriteQuery(string Query, params object[] args)
        {
            WriteQuery(string.Format(null, Query, args));
        }

        public static void ExecuteQuery()
        {            
            QueryWriter.Close();
            string[] tmpString = File.ReadAllLines(Path);
            for (int i = 0; i < tmpString.Length; i++)
            {
                Console.WriteLine("{0} queries to execute!", tmpString.Length);
                if (tmpString[i] != null)
                    Database.ExecuteQueryAsnyc(tmpString[i]);                
            }
            QueryWriter = new StreamWriter(Path);
        }

        public static void ExecuteSavedQueries()
        {
            if (File.Exists(Path))
            {
                string[] tmpString = File.ReadAllLines(Path);
                for (int i = 0; i < tmpString.Length; i++)
                {
                    if (tmpString[i] != null)
                        Database.ExecuteQueryAsnyc(tmpString[i]);
                }
            }

            Console.Clear();
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine(" <Silkroad Loginserver>  Copyright (C) <2011>  <DarkEmu>");
            Console.WriteLine("-------------------------------------------------------"); 
            Console.WriteLine("Loading essentiel data and connecting to database.\nThis could take some minutes!");
            Database.CheckTables(new string[] { "news", "server", "user" });
            Console.WriteLine("Executed the queries!");

            QueryWriter = new StreamWriter(Path);
        }
    }
}
