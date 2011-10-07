///////////////////////////////////////////////////////////////////////////
// Copyright (C) <2011>  <DarkEmu>
// HOMEPAGE: WWW.XFSGAMES.COM.AR
///////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace LoginServer
{
    public partial class Systems
    {
        public static Dictionary<int, Systems.SRX_Serverinfo> GSList = new Dictionary<int, Systems.SRX_Serverinfo>();

        public class SRX_Serverinfo
        {
            public UInt16 id = 1;
            public UInt16 port = 15780;
            public UInt16 ipcport = 15791;
            public UInt16 maxSlots = 100;
            public UInt16 usedSlots = 0;

            public string name = "test";
            public string ip = "127.0.0.1";
            public string extip = "1";
            public string code = "t";

            public byte status = 0;

            public int Version = 0;

            public DateTime lastPing=DateTime.MinValue;
        }

        public static SRX_Serverinfo GetServerByEndPoint(string ip, int port)
        {
            SRX_Serverinfo GS = null;

            foreach (KeyValuePair<int, Systems.SRX_Serverinfo> GSI in GSList)
            {
                if (GSI.Value.ip == ip && GSI.Value.ipcport == port)
                {
                    GS = GSI.Value;
                }
            }
            return GS;
        }

        public static void CheckServerExpired(int seconds)
        {
        }

        public static int LoadServers(string serverFile, UInt16 defaultPort)
        {
            try
            {
                if (File.Exists(Environment.CurrentDirectory + @"\Settings\" + serverFile))
                {
                    Systems.Ini ini = new Systems.Ini(Environment.CurrentDirectory + @"\Settings\" + serverFile);
                    string[] sList = null;
                    sList = ini.GetEntryNames("SERVERS");
                    if (sList!=null && sList.Length > 0)
                    {
                        foreach (string sectname in sList)
                        {
                            string sName = ini.GetValue("SERVERS", sectname, "");
                            Systems.SRX_Serverinfo SServerInfo = new Systems.SRX_Serverinfo();
                            SServerInfo.id = Convert.ToUInt16(ini.GetValue(sName, "id", 0));
                            SServerInfo.ip = ini.GetValue(sName, "ip", "");
                            SServerInfo.name = ini.GetValue(sName, "name", sName);
                            SServerInfo.port = Convert.ToUInt16(ini.GetValue(sName, "port", defaultPort));
                            SServerInfo.ipcport = Convert.ToUInt16(ini.GetValue(sName, "ipcport", "15791"));
                            SServerInfo.code = ini.GetValue(sName, "code", "t");
                            SServerInfo.Version = Convert.ToInt32(ini.GetValue(sName, "version", 0));
                            if (SServerInfo.ip == "" || SServerInfo.port == 0 || SServerInfo.id == 0 || SServerInfo.ipcport == 0 || GSList.ContainsKey(SServerInfo.id))
                            {
                                Console.WriteLine("IPC: Error on Server "+ sName +" in "+ serverFile +": field missing or id already in use!");
                                SServerInfo = null;
                            }
                            else
                            {
                                GSList.Add(SServerInfo.id, SServerInfo);
                            }
                        }
                    }
                    if (GSList.Count() > 0)
                    {
                        string servers = "Server";
                        if (GSList.Count > 1) servers = "Servers";
                        Console.WriteLine("[INFO] Loaded " + GSList.Count() + " " + servers + " from server settings");
                    }
                    else
                    {
                        Systems.SRX_Serverinfo GServer = new Systems.SRX_Serverinfo();
                        GServer.id = 1;
                        GServer.ip = "127.0.0.1";
                        if (Global.Network.multihomed)
                        {
                            //Multihomed
                        }
                        else
                        {
                            GServer.extip = Global.Network.LocalIP;
                        }
                        GServer.name = "[DARKEMU] Default";
                        GServer.port = defaultPort;
                        GServer.ipcport = 15791;
                        GServer.code = "t";
                        GSList.Add(GServer.id, GServer);
                    }
                    sList = null;
                    ini = null;
                    return GSList.Count();
                }
                else
                {
                    Systems.SRX_Serverinfo GServer = new Systems.SRX_Serverinfo();
                    GServer.id = 1;
                    GServer.ip = "127.0.0.1";
                    if (Global.Network.multihomed)
                    {
                        //Multihomed
                    }
                    else
                    {
                        //No servers
                        GServer.extip = Global.Network.LocalIP;
                    }
                    GServer.name = "[DARKEMU "+ Global.Versions.appVersion +"]";
                    GServer.port = defaultPort;
                    GServer.ipcport = 15791;
                    GServer.code = "";
                    GSList.Add(GServer.id, GServer);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading GameServer settings "+ ex +"");
                return -2;
            }
        }
    }
}
