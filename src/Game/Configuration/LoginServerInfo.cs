///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        public static Dictionary<int, Systems.SRX_Serverinfo> LSList = new Dictionary<int, Systems.SRX_Serverinfo>();

        public class SRX_Serverinfo
        {
            public UInt16 id = 1;
            public string ip = "127.0.0.1";
            public string code = "t";
            public UInt16 ipcport = 15790;
            public DateTime lastPing=DateTime.MinValue;
        }

        public static SRX_Serverinfo GetServerByEndPoint(string ip, int port)
        {
            SRX_Serverinfo LS = null;

            foreach (KeyValuePair<int, Systems.SRX_Serverinfo> LSI in LSList)
            {
                if (LSI.Value.ip == ip && LSI.Value.ipcport == port)
                {
                    LS = LSI.Value;
                }
            }
            return LS;
        }

        public static void CheckServerExpired(int seconds)
        {
        }

        public static int LoadServers(string serverFile, UInt16 defaultPort)
        {
            try
            {
                if (System.IO.File.Exists(Environment.CurrentDirectory + @"\Settings\" + serverFile))
                {
                    Framework.Ini ini = new Framework.Ini(Environment.CurrentDirectory + @"\Settings\" + serverFile);
                    string[] sList = null;
                    sList = ini.GetEntryNames("SERVERS");
                    if (sList!=null && sList.Length > 0)
                    {

                        foreach (string sectname in sList)
                        {
                            string sName = ini.GetValue("SERVERS", sectname, "");
                            Systems.SRX_Serverinfo SServerInfo = new Systems.SRX_Serverinfo();
                            SServerInfo.id = (UInt16)(ini.GetValue(sName, "id", 0));
                            SServerInfo.ip = ini.GetValue(sName, "ip", "127.0.0.1");
                            SServerInfo.ipcport = (UInt16)(ini.GetValue(sName, "ipcport", defaultPort));
                            SServerInfo.code = ini.GetValue(sName, "code", "t");
                            if (SServerInfo.ip == "" || SServerInfo.id == 0 || SServerInfo.ipcport == 0 || LSList.ContainsKey(SServerInfo.id))
                            {
                                Console.WriteLine("IPC: Error on Server \"{0}\" in {1}: Mandatory field missing or id already in use!", sName, serverFile);
                                SServerInfo = null;
                            }
                            else
                            {
                                LSList.Add(SServerInfo.id, SServerInfo);
                            }
                        }
                    }
                    if (LSList.Count() > 0)
                    {
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("------------------------------------[SERVER]------------------------------------");
                        string defServer = "Server";
                        if (LSList.Count > 1) defServer = "Servers";
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("                          Added:");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" {0} ", LSList.Count());
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("{0} to the list", defServer);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("[IPC] Info: No LoginServers configured, using a default local LoginServer.");
                        Systems.SRX_Serverinfo LServer = new Systems.SRX_Serverinfo();
                        LServer.id = 1;
                        LServer.ip = "127.0.0.1";
                        LServer.ipcport = defaultPort;
                        LServer.code = "t";
                        LSList.Add(LServer.id, LServer);
                    }
                    sList = null;
                    ini = null;
                    return LSList.Count();
                }
                else
                {
                    Console.WriteLine("[IPC] Info: No LoginServers configured, using a default local LoginServer.");
                    Systems.SRX_Serverinfo LServer = new Systems.SRX_Serverinfo();
                    LServer.id = 1;
                    LServer.ip = "127.0.0.1";
                    LServer.ipcport = defaultPort;
                    LServer.code = "t";
                    LSList.Add(LServer.id, LServer);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading LoginServer settings {0}", ex);
                return -2;
            }
        }
    }
}
