///////////////////////////////////////////////////////////////////////////
// Copyright (C) <2011>  <DarkEmu>
// HOMEPAGE: WWW.XFSGAMES.COM.AR
///////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Net;
using System.Text;
using LoginServer;
using System.Threading;
using LoginServer.Global;
using System.Net.Sockets;
using DarkEmu_GameServer.Network;
using System.Collections.Generic;

namespace LoginServer
{
    public class IPCItem
    {
        public UInt16 resultCode;
        public string banReason;
    }
    public class Program
    {
        public static DarkEmu_GameServer.Network.Servers.IPCServer IPCServer;
        public static Dictionary<UInt16, IPCItem> IPCResultList = new Dictionary<UInt16, IPCItem>();
        public static UInt16 IPCNewId = 0;
        public static int IPCPort = 15790;

        public static void Main(string[] args)
        {

            Program pro = new Program();
            Definitions.Bootlogo._Load();
            LoginServer.Systems.Ini ini = null;
            #region Default Settings
            int LSPort = 15779;
            int IPCPort = 15790;
            string LSIP = "127.0.0.1";
            string IPCIP = "127.0.0.1";
            Systems.DownloadServer = "127.0.0.1";
            #endregion

            #region Load Settings
            try
            {
                if (File.Exists(Environment.CurrentDirectory + @"\Settings\Settings.ini"))
                {
                    ini = new LoginServer.Systems.Ini(Environment.CurrentDirectory + @"\Settings\Settings.ini");
                    LSPort = Convert.ToInt32(ini.GetValue("Server", "port", 15779));
                    LSIP = ini.GetValue("Server", "ip", "").ToString();
                    IPCPort = Convert.ToInt32(ini.GetValue("IPC", "port", 15790));
                    IPCIP = ini.GetValue("IPC", "ip", "").ToString();
                    Systems.DownloadServer = ini.GetValue("Patch_Server", "ip", "");
                    Systems.DownloadPort = Convert.ToInt16(ini.GetValue("Patch_Server", "port", ""));
                    ini = null;
                    Console.WriteLine("{0}Has loaded your ip settings successfully", Product.Prefix);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("{0}Settings.ini could not be found, using default setting", Product.Prefix);
                }
            }
            catch (Exception)
            {
                return;
            }
            #endregion

            #region get local ip

            if (args.Length > 0)
            {
                if (args[0] == "extip")
                {
                    HttpWebRequest HWR = (HttpWebRequest)WebRequest.Create("http://checkip.dyndns.org/");
                    HWR.Method = "GET";
                    WebResponse MWR = HWR.GetResponse();
                    StreamReader Hsr = new StreamReader(MWR.GetResponseStream(), System.Text.Encoding.UTF8);
                    string theip = Hsr.ReadToEnd();
                }
            }

            Network.multihomed = false;
            if (Network.LocalIP == "")
            {
                IPAddress[] lIpList = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress aIP in lIpList)
                {
                    if (aIP.AddressFamily.Equals(AddressFamily.InterNetwork))
                    {
                        if (!aIP.Equals(IPAddress.Loopback))
                        {
                            if (Network.LocalIP != "")
                            {
                                Network.multihomed = true;
                            }
                            else
                            {
                                Network.LocalIP = aIP.ToString();
                            }
                        }
                    }
                }
            }
            #endregion

            Systems.Server net = new Systems.Server();

            net.OnConnect += new Systems.Server.dConnect(pro._OnClientConnect);
            net.OnError += new Systems.Server.dError(pro._ServerError);

            Systems.Client.OnReceiveData += new Systems.Client.dReceive(pro._OnReceiveData);
            Systems.Client.OnDisconnect += new Systems.Client.dDisconnect(pro._OnClientDisconnect);
            //Content.Patches.LoadPatches();
            Systems.Load_NewsList();

            try
            {
                net.Start(LSIP, LSPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{1}Starting Server error: {0}", ex, Product.Prefix);
            }

            #region Load GameServers
            Systems.LoadServers("GameServers.ini", 15780);
            #endregion

            #region IPC Listener
            IPCServer = new Servers.IPCServer();
            IPCServer.OnReceive += new Servers.IPCServer.dOnReceive(pro.OnIPC);
            try
            {
                IPCServer.Start(IPCIP, IPCPort);
                foreach (KeyValuePair<int, Systems.SRX_Serverinfo> GS in Systems.GSList)
                {
                    byte[] rqPacket = IPCServer.PacketRequestServerInfo(IPCPort);
                    Servers.IPCenCode(ref rqPacket, GS.Value.code);
                    IPCServer.Send(GS.Value.ip, GS.Value.ipcport, rqPacket);
                    rqPacket = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error start ICP: {0}", ex);
            }
            #endregion
            Console.WriteLine("{0}Ready for gameserver connection...", Global.Product.Prefix);
            while (true)
            {
                Thread.Sleep(100);
                foreach (KeyValuePair<int, Systems.SRX_Serverinfo> SSI in Systems.GSList)
                {
                    if (SSI.Value.status != 0 && SSI.Value.lastPing.AddMinutes(5) < DateTime.Now) // server unavailable
                    {
                        SSI.Value.status = 0;
                        Console.WriteLine("{2}Server: ({1}) has timed out, status changed to check", SSI.Value.id, SSI.Value.name, Product.Prefix);
                    }
                }
            }

        }

        public void OnIPC(System.Net.Sockets.Socket aSocket, System.Net.EndPoint ep, byte[] data)
        {
            try
            {
                if (data.Length >= 6)
                {
                    UInt16 pServer = (UInt16)(data[0] + (data[1] << 8));
                    Systems.SRX_Serverinfo remoteGameServer = Systems.GetServerByEndPoint(((IPEndPoint)ep).Address.ToString(), pServer);
                    if (remoteGameServer != null)
                    {
                        // decode data
                        Servers.IPCdeCode(ref data, remoteGameServer.code);

                        byte pCmd = data[3];
                        int dLen = (int)(data[4] + (data[5] << 8));
                        byte crc = Servers.BCRC(data, data.Length - 1);
                        if (data[data.Length - 1] != crc) // wrong CRC
                        {
                            Console.WriteLine("{1} Wrong Checksum for Server {0}", remoteGameServer.name, Product.Prefix);
                            return;
                        }
                        if (data.Length >= (dLen + 6))
                        {
                            if (pCmd == (byte)IPCCommand.IPC_INFO_SERVER)
                            {
                                if (data.Length >= 11)
                                {
                                    remoteGameServer.maxSlots = (UInt16)(data[7] + (data[8] << 8));
                                    remoteGameServer.usedSlots = (UInt16)(data[9] + (data[10] << 8));
                                    remoteGameServer.lastPing = DateTime.Now;
                                    //Console.WriteLine("[IPC] Received SERVER-INFO from GameServer {1} ({0}): S={2}, MAX={3}, CUR={4}", remoteGameServer.name, remoteGameServer.id, data[6], remoteGameServer.maxSlots, remoteGameServer.usedSlots);
                                    if (remoteGameServer.status == 0 && data[6] != 0)
                                    {
                                        Console.WriteLine("{2}GameServer {0} ({1}) status changed to online", remoteGameServer.id, remoteGameServer.name, Product.Prefix);
                                    }
                                    if (remoteGameServer.status != 0 && data[6] == 0)
                                    {
                                        Console.WriteLine("{2}GameServer {0} ({1}) status changed to check", remoteGameServer.id, remoteGameServer.name, Product.Prefix);
                                    }
                                    remoteGameServer.status = data[6];
                                }
                                else
                                {
                                }
                            }
                            else if (pCmd == (byte)IPCCommand.IPC_INFO_LOGIN)
                            {
                                if (dLen >= 4)
                                {
                                    UInt16 IPCid = (UInt16)(data[6] + (data[7] << 8));
                                    UInt16 IPCResult = (UInt16)(data[8] + (data[9] << 8));
                                    byte sLen = data[10];
                                    lock (IPCResultList)
                                    {
                                        if (IPCResultList.ContainsKey(IPCid))
                                        {
                                            IPCResultList[IPCid].resultCode = IPCResult;
                                            if (sLen > 0)
                                            {
                                                IPCResultList[IPCid].banReason = System.Text.ASCIIEncoding.ASCII.GetString(data, 11, sLen);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[IPC] ResultList mismatch");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("[IPC] unknown command recevied");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[IPC] data to short");
                        }
                    }
                    else
                    {
                        Console.WriteLine("[IPC] can't find the GameServer {0}:{1}", ((IPEndPoint)ep).Address.ToString(), pServer);
                    }

                }
                else
                {
                    Console.WriteLine("[IPC] packet to short from {0}", ep.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[IPC.OnIPC] {0}", ex);
            }
        }

        public void _OnReceiveData(LoginServer.Systems.Decode de)
        {
            Systems.oPCode(de);
        }
        public void _OnClientConnect(ref object de, LoginServer.Systems.Client net)
        {
            de = new Systems(net);
        }

        public void _OnClientDisconnect(object o)
        {
            try
            {
                Systems s = (Systems)o;
                s.client.clientSocket.Close();
            }
            catch { }
        }
        private void _ServerError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            LoginServer.Systems.Print.Format("Error:{0}", ex.Message);
            LoginServer.Systems.Print.Format("Server:{0}", ex.Source);
        }
    }
}