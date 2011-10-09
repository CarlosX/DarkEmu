
using DarkEmu_GameServer;
using System;
using Framework;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net;
using DarkEmu_GameServer.Network;

namespace GameServer
{

    class Program
    {
        public static Systems.Server net;
        public static Systems.CommandServer cmd;
        static bool cancelServer = false;
        static DateTime lastPromote = DateTime.MinValue;

        #region App Close Handling
        delegate bool ConsoleEventHandlerDelegate(ConsoleHandlerEventCode eventCode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleEventHandlerDelegate handlerProc, bool add);

        enum ConsoleHandlerEventCode : uint { CTRL_C_EVENT = 0, CTRL_BREAK_EVENT = 1, CTRL_CLOSE_EVENT = 2, CTRL_LOGOFF_EVENT = 5, CTRL_SHUTDOWN_EVENT = 6 }

        static ConsoleEventHandlerDelegate consoleHandler;

        static Program()
        {
            consoleHandler = new ConsoleEventHandlerDelegate(ConsoleEventHandler);
            SetConsoleCtrlHandler(consoleHandler, true);
        }

        static bool ConsoleEventHandler(ConsoleHandlerEventCode eventCode)
        {
            if (eventCode == ConsoleHandlerEventCode.CTRL_CLOSE_EVENT)
            {
                Console.WriteLine("Exiting because user closed Console", eventCode);
                Thread.Sleep(500);
                ExecuteCommand("//shutdown", null);
            }
            return false;
        }
        #endregion

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.GetCultureInfo(TxtFile.GetSystemDefaultLangID()).ToString());

            Framework.Ini ini;

            DarkEmu_GameServer.Systemcore.Definitions.Bootlogo._Load();

            Program pro = new Program();

            // Begin connection to our database.
            Systems.MsSQL.OnDatabaseError += new Systems.MsSQL.dError(db_OnDatabaseError);
            // Read our database settings from our ini file.
            Systems.MsSQL.OnConnectedToDatabase += new Systems.MsSQL.dConnected(db_OnConnectedToDatabase);
            // Check if our ini file excists.
            string sqlConnect = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=silkroad;Integrated Security=True;MultipleActiveResultSets=True;";
            string sIpIPC = "127.0.0.1";
            string sIpServer = "127.0.0.1";
            UInt16 iPortIPC = 15791;
            UInt16 iPortServer = 15780;
            UInt16 iPortCmd = 10101;
            if (File.Exists("./Settings/Settings.ini"))
            {
                //Load our ini file
                ini = new Framework.Ini(Environment.CurrentDirectory + @"\Settings\Settings.ini");
                //Read line below given value.
                sqlConnect = ini.GetValue("Database", "connectionstring", @"Data Source=(local)\SQLEXPRESS;Initial Catalog=silkroad;Integrated Security=True;MultipleActiveResultSets=True;").ToString();
                //Load our rates.
                Systems.Rate.Gold = Convert.ToByte(ini.GetValue("Rates", "Goldrate", 1));
                Systems.Rate.Item = Convert.ToByte(ini.GetValue("Rates", "Droprate", 1));
                Systems.Rate.Xp = Convert.ToByte(ini.GetValue("Rates", "XPrate", 1));
                Systems.Rate.Sp = Convert.ToByte(ini.GetValue("Rates", "SPrate", 1));
                Systems.Rate.Sox = Convert.ToByte(ini.GetValue("Rates", "Sealrate", 1));
                Systems.Rate.Elixir = Convert.ToByte(ini.GetValue("Rates", "Elixirsrate", 1));
                Systems.Rate.Alchemyd = Convert.ToByte(ini.GetValue("Rates", "Alchemyrate", 1));
                Systems.Rate.ETCd = Convert.ToByte(ini.GetValue("Rates", "ETCrate", 1));
                Systems.Rate.Spawns = Convert.ToByte(ini.GetValue("Rates", "Spawnrate", 1));
                iPortIPC = Convert.ToUInt16(ini.GetValue("IPC", "port", 15791));
                sIpIPC = ini.GetValue("IPC", "ip", "127.0.0.1");
                iPortServer = Convert.ToUInt16(ini.GetValue("Server", "port", 15780));
                sIpServer = ini.GetValue("Server", "ip", "127.0.0.1");
                iPortCmd = Convert.ToUInt16(ini.GetValue("CMD", "port", 10101));
                DarkEmu_GameServer.Systems.maxSlots = Convert.ToInt32(ini.GetValue("Server", "MaxSlots", 100));
            }
            else
            {
                Systems.Rate.Gold = 1;
                Systems.Rate.Item = 1;
                Systems.Rate.Xp = 1;
                Systems.Rate.Sp = 1;
                Systems.Rate.Sox = 1;
                Systems.Rate.Elixir = 1;
                Systems.Rate.Alchemyd = 1;
                Systems.Rate.ETCd = 1;
                Systems.Rate.Spawns = 1;
                Systems.maxSlots = 100;

            }
            Systems.MsSQL.Connection(sqlConnect);
            //Boot_Logo._Rates();
            // create servers
            try
            {
                net = new Systems.Server();

                net.OnConnect += new Systems.Server.dConnect(pro._OnClientConnect);
                net.OnError += new Systems.Server.dError(pro._ServerError);

                Systems.ServerStartedTime = DateTime.Now;

                Systems.Client.OnReceiveData += new Systems.Client.dReceive(pro._OnReceiveData);
                Systems.Client.OnDisconnect += new Systems.Client.dDisconnect(pro._OnClientDisconnect);

                #region CommandServer StartUp
                cmd = new Systems.CommandServer();
                cmd.OnCommandReceived += new Systems.CommandServer.dCommandReceived(pro._command);
                #endregion
                #region IPC Server StartUp
                Systems.IPC = new Servers.IPCServer();
                Systems.IPC.OnReceive += new Servers.IPCServer.dOnReceive(pro._OnIPC);
                Systems.LoadServers("LoginServers.ini", 15790);
                #endregion

            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }

            DarkEmu_GameServer.Systems.CheckDirectories();

            DarkEmu_GameServer.File.FileLoad.Load();
            //Update serverlist info
            DarkEmu_GameServer.Systems.clients.update += new EventHandler(Users.updateServerList);
            //Load random unique monsters.
            /*Random rand = new Random();*/

            /*
            DarkEmu_GameServer.GlobalUnique.StartTGUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn tiger girl
            DarkEmu_GameServer.GlobalUnique.StartUriUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn urichi
            DarkEmu_GameServer.GlobalUnique.StartIsyUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn isy
            DarkEmu_GameServer.GlobalUnique.StartLordUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn lord yarkan
            DarkEmu_GameServer.GlobalUnique.StartDemonUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn demon shaitan
            DarkEmu_GameServer.GlobalUnique.StartCerbUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn cerberus
            DarkEmu_GameServer.GlobalUnique.StartMedusa(rand.Next(10, 20) * 90000, 600);   //Random spawn medusa
            DarkEmu_GameServer.GlobalUnique.StartNeith(rand.Next(10, 20) * 90000, 600);   //Random spawn neith
            //Game.GlobalUnique.StartSphinx       (rand.Next(10, 20) * 90000, 600);   //Random spawn medusa
            DarkEmu_GameServer.GlobalUnique.StartIsis(rand.Next(10, 20) * 90000, 600);   //Random spawn isis
            //Game.GlobalUnique.StartRoc          (rand.Next(10, 20) * 90000, 600);   //Random spawn roc
            DarkEmu_GameServer.GlobalUnique.StartIvyUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn captain ivy
            */

            // start listening servers
            cmd.Start("127.0.0.1", iPortCmd);
            Systems.IPC.Start(sIpIPC, iPortIPC);
            net.Start(sIpServer, iPortServer);
            Systems.UpdateServerInfo();

            //Run for commands in console.
            Program launch = new Program();
            Thread run = new Thread(new ThreadStart(launch.run));
            run.Start();

            // main loop
            lastPromote = DateTime.Now;
            while (run.IsAlive)
            {
                Thread.Sleep(10);
                if (lastPromote.AddSeconds(60) < DateTime.Now)
                {
                    lastPromote = DateTime.Now;
                    Systems.UpdateServerInfo();
                }
            }
            Systems.UpdateServerInfo(0);
            net.ServerCheck(false);

        }

        #region Events
        public void _OnReceiveData(Systems.Decode de)
        {
            try
            {
                //Todo: Add exception details for catching bad information.
                Systems.oPCode(de);
            }
            catch (Exception)
            {

            }
        }
        public void _OnClientConnect(ref object de, Systems.Client net)
        {
            try
            {
                de = new Systems(net);
            }
            catch (Exception)
            {

            }
        }
        public void _OnClientDisconnect(object o)
        {
            try
            {
                if (o != null)
                {
                    Systems s = (Systems)o;
                    s.Disconnect("normal");
                }
            }
            catch (Exception)
            {

            }
        }
        private void _ServerError(Exception ex)
        {
            Console.WriteLine("@Gameserver:         Error:", ex.Message);
            Console.WriteLine("@Gameserver:         {0}", ex.StackTrace);
        }
        private static void db_OnDatabaseError(Exception ex)
        {
            Console.WriteLine("@Gameserver:         Error::{0}", ex);
        }
        private static void db_OnConnectedToDatabase()
        {
            Console.WriteLine("@Gameserver:         Database connection established");
        }
        #endregion
        #region CommandServer
        public void _command(string aCommand, Socket aSocket)
        {
            ExecuteCommand(aCommand, aSocket);
        }
        public static void sendSocket(Socket aSocket, string aText)
        {
            if (aSocket != null)
            {
                try
                {
                    aSocket.Send(Encoding.ASCII.GetBytes(aText));
                }
                catch { }
            }
        }
        #endregion
        #region IPCServer
        public void _OnIPC(Socket aSocket, EndPoint ep, byte[] data)
        {
            try
            {
                if (data.Length >= 6)
                {
                    UInt16 pServer = (UInt16)(data[0] + (data[1] << 8));
                    Systems.SRX_Serverinfo remoteLoginServer = Systems.GetServerByEndPoint(((IPEndPoint)ep).Address.ToString(), pServer);
                    if (remoteLoginServer != null)
                    {
                        // decode data
                        Servers.IPCdeCode(ref data, remoteLoginServer.code);

                        byte pCmd = data[3];
                        int dLen = (int)(data[4] + (data[5] << 8));
                        byte crc = Servers.BCRC(data, data.Length - 1);
                        if (data[data.Length - 1] != crc) // wrong CRC
                        {
                            Console.WriteLine("[IPC] Wrong Checksum from Server {0}. Please Check !", remoteLoginServer.id);
                            return;
                        }
                        if (data.Length >= (dLen + 6))
                        {
                            if (pCmd == (byte)IPCCommand.IPC_REQUEST_SERVERINFO)
                            {
                                remoteLoginServer.lastPing = DateTime.Now;
                                byte[] rspBuf = Systems.IPC.PacketResponseServerInfo((UInt16)Servers.IPCPort, 1, 100, (UInt16)Systems.clients.Count, (UInt16)DarkEmu_GameServer.Global.Versions.clientVersion);
                                Servers.IPCenCode(ref rspBuf, remoteLoginServer.code);
                                Systems.IPC.Send(remoteLoginServer.ip, remoteLoginServer.ipcport, rspBuf);
                            }
                            else if (pCmd == (byte)IPCCommand.IPC_REQUEST_LOGIN)
                            {
                                remoteLoginServer.lastPing = DateTime.Now;
                                if (dLen > 4)
                                {
                                    int bp = 6;
                                    byte cLen = data[bp++];
                                    byte[] tmpbuf = new byte[cLen];
                                    Buffer.BlockCopy(data, bp, tmpbuf, 0, cLen);
                                    bp += cLen;
                                    string tmpID = System.Text.ASCIIEncoding.ASCII.GetString(tmpbuf);
                                    cLen = data[bp++];
                                    tmpbuf = new byte[cLen];
                                    Buffer.BlockCopy(data, bp, tmpbuf, 0, cLen);
                                    bp += cLen;
                                    UInt16 rCode = (UInt16)(data[bp] + (data[bp + 1] << 8));
                                    string tmpPW = System.Text.ASCIIEncoding.ASCII.GetString(tmpbuf);
                                    player tmpPlayer = null;
                                    int lResult = Systems.LoginUser(tmpID, ref tmpPW, ref tmpPlayer, false);
                                    tmpPlayer = null;
                                    tmpbuf = Systems.IPC.PacketResponseLogin(Servers.IPCPort, (UInt16)lResult, rCode, lResult == 4 ? tmpPW : "");
                                    Servers.IPCenCode(ref tmpbuf, remoteLoginServer.code);
                                    Systems.IPC.Send(remoteLoginServer.ip, remoteLoginServer.ipcport, tmpbuf);
                                }
                                else
                                {
                                    Console.WriteLine("[IPC] content to short");
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
                        Console.WriteLine("[IPC] can't find the LoginServer {0}:{1}", ((IPEndPoint)ep).Address.ToString(), pServer);
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
        #endregion
        public static void ExecuteCommand(string aCommand, Socket aSocket)
        {
            try
            {
                if (aCommand != null)
                {
                    string[] command = aCommand.Split(' ');
                    if (command[0] == "/help")
                    {

                        if (aSocket == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("                     Ingame notice: Type 1 space then message.");
                            Console.WriteLine("                     //clear");
                            Console.WriteLine("                     //repairitems");
                            Console.WriteLine("                     //respawn_unique");
                            Console.WriteLine("                     //event");
                            Console.WriteLine("                     //shutdown");
                            Console.WriteLine("                     //manager");

                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        else
                        {
                            sendSocket(aSocket, "Ingame notice: Type 1 space then message." + Environment.NewLine);
                            sendSocket(aSocket, "  //clear = cleanup unused memory" + Environment.NewLine);
                            sendSocket(aSocket, "  //repairitems" + Environment.NewLine);
                            sendSocket(aSocket, "  //respawn_unique" + Environment.NewLine);
                            sendSocket(aSocket, "  //event" + Environment.NewLine);
                            sendSocket(aSocket, "  //shutdown" + Environment.NewLine);
                        }
                    }
                    else if (command[0] == "//clear")
                    {
                        System.GC.Collect();
                        GC.Collect(0, GCCollectionMode.Forced);
                        sendSocket(aSocket, "done memory cleanup" + Environment.NewLine);
                    }
                    else if (command[0] == "//shutdown")
                    {
                        byte waitTime = 5;
                        if (command.Length > 1)
                        {
                            waitTime = System.Convert.ToByte(command[1]);
                        }
                        if (aSocket != null)
                        {
                            aSocket.Send(Encoding.ASCII.GetBytes("SHUTDOWN_START" + Environment.NewLine));
                        }

                        lock (Systems.clients)
                        {
                            Console.WriteLine("{0}stopping server and sending notice to clients ...", DarkEmu_GameServer.Global.Product.Prefix);
                            sendSocket(aSocket, "SHUTDOWN: stopping server and sending notice to clients ..." + Environment.NewLine);
                            net.ServerCheck(true);
                            net.Stop();  // disable any new connection
                            try
                            {
                                Systems.SendAll(Packet.ChatPacket(7, 0, "The server is stopping, your information will be saved.", ""));
                                Systems.SendAll(Packet.StartingLeaveGame(waitTime, 0));
                            }
                            catch { }
                            Thread.Sleep(waitTime);
                            Console.WriteLine("@SHUTDOWN: logoff clients ...");
                            sendSocket(aSocket, "@SHUTDOWN: logoff clients ..." + Environment.NewLine);
                            while (Systems.clients.Count > 0)
                            {
                                try
                                {
                                    try
                                    {
                                        Systems.clients[0].Send(Packet.EndLeaveGame());
                                    }
                                    catch { }
                                    //Ignore new character case (used for disconnect kick).
                                    Systems c = new Systems();
                                    Systems.clients[0].Disconnect("normal");
                                }
                                catch { }
                            }
                        }
                        sendSocket(aSocket, "SHUTDOWN_END" + Environment.NewLine);
                        cancelServer = true;
                        Systems.UpdateServerInfo(0);
                        //Environment.Exit(0);
                    }
                    else if (command[0] == "//repairitems")
                    {
                        int fixeditem = 0;
                        Systems.MsSQL ms = new Systems.MsSQL("SELECT * FROM char_items");
                        using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
                        {
                            while (reader.Read())
                            {
                                short amount = reader.GetInt16(7);
                                if (amount < 1)
                                {
                                    fixeditem++;
                                    amount = 1;
                                    Systems.MsSQL.InsertData("UPDATE char_items SET quantity='" + amount + "' WHERE itemnumber='" + "item" + reader.GetByte(5) + "' AND owner='" + reader.GetInt32(3) + "' AND itemid='" + reader.GetInt32(2) + "'");
                                }
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("@Gameserver:         Items Repaired:           {0}", fixeditem);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        sendSocket(aSocket, String.Format("@Gameserver: Items Repaired: {0}", fixeditem) + Environment.NewLine);
                    }
                    else if (command[0] == "//respawn_unique")
                    {
                        DarkEmu_GameServer.GlobalUnique.StartTGUnique(6000 * 10, 6000 * 10);
                        DarkEmu_GameServer.GlobalUnique.StartUriUnique(7000 * 10, 7000 * 10);
                        DarkEmu_GameServer.GlobalUnique.StartIsyUnique(8000 * 10, 8000 * 10);
                        DarkEmu_GameServer.GlobalUnique.StartLordUnique(9000 * 10, 9000 * 10);
                        DarkEmu_GameServer.GlobalUnique.StartDemonUnique(10000 * 10, 10000 * 10);
                        DarkEmu_GameServer.GlobalUnique.StartCerbUnique(11000 * 10, 11000 * 10);
                        DarkEmu_GameServer.GlobalUnique.StartIvyUnique(11000 * 10, 11000 * 10);
                        //Game.GlobalUnique.StartRoc(11000 * 10, 11000 * 10);
                        DarkEmu_GameServer.GlobalUnique.StartMedusa(11000 * 10, 11000 * 10);
                        DarkEmu_GameServer.GlobalUnique.StartNeith(11000 * 10, 11000 * 10);
                        //Game.GlobalUnique.StartSphinx(11000 * 10, 11000 * 10);
                        DarkEmu_GameServer.GlobalUnique.StartIsis(11000 * 10, 11000 * 10);
                        sendSocket(aSocket, "done respawn" + Environment.NewLine);
                    }
                    else if (command[0] == "//event")
                    {
                        EventMain eventnew = new EventMain(System.IO.Directory.GetCurrentDirectory() + "/data/event.txt");
                        eventnew.Start();
                        sendSocket(aSocket, "Event Started" + Environment.NewLine);
                    }
                    else if (command[0] == "")
                    {
                        string information = aCommand;
                        DarkEmu_GameServer.Systems c = new DarkEmu_GameServer.Systems();
                        DarkEmu_GameServer.Systems.SendAll(c.sendnoticecon(7, 0, information, ""));

                        Console.WriteLine("Notice: " + information);
                        sendSocket(aSocket, "Sent Notice: " + information + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program.Main {0}", ex);
            }
        }
        public void run()
        {
            while (!cancelServer)
            {
                try
                {
                    string read = Console.ReadLine();
                    ExecuteCommand(read, null);
                }
                catch { }
            }
        }
    }
}