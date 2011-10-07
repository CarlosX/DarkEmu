using Game;
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
using SRX.Network;

namespace Game
{
    public partial class Manager_Main : Form
    {
        //Enable form dragging
        #region Drag windowless form
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();
        #endregion
        delegate void UpdateActivityBox(string chat);
        delegate void UpdatePlayerList(string name);
        delegate void UpdatePlayerListRemove(string name);

        public Manager_Main()
        {
            InitializeComponent();
            Show();
            Thread dowork = new Thread(Manager_Work);
            dowork.Start();
        }

        public void Manager_Work()
        {
            Framework.Ini ini;
            // Begin connection to our database.
            Systems.MsSQL.OnDatabaseError += new Systems.MsSQL.dError(db_OnDatabaseError);
            // Read our database settings from our ini file.
            Systems.MsSQL.OnConnectedToDatabase += new Systems.MsSQL.dConnected(db_OnConnectedToDatabase);
            // Check if our ini file excists.
            string sqlConnect = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=silk;Integrated Security=True;MultipleActiveResultSets=True;";
            string sIpIPC = "";
            string sIpServer = "";
            UInt16 iPortIPC = 15780;
            UInt16 iPortServer = 15780;
            UInt16 iPortCmd = 10101;
            if (System.IO.File.Exists("./Settings/Settings.ini"))
            {
                //Load our ini file
                ini = new Framework.Ini(Environment.CurrentDirectory + @"\Settings\Settings.ini");
                //Read line below given value.
                sqlConnect = ini.GetValue("Database", "connectionstring", @"Data Source=(local)\SQLEXPRESS;Initial Catalog=silk;Integrated Security=True;MultipleActiveResultSets=True;").ToString();
                //Load our rates.
                Game.Rate.Gold = Convert.ToByte(ini.GetValue("Rates", "Goldrate", 1));
                Game.Rate.Item = Convert.ToByte(ini.GetValue("Rates", "Droprate", 1));
                Game.Rate.Xp = Convert.ToByte(ini.GetValue("Rates", "XPrate", 1));
                Game.Rate.Sp = Convert.ToByte(ini.GetValue("Rates", "SPrate", 1));
                Game.Rate.Sox = Convert.ToByte(ini.GetValue("Rates", "Sealrate", 1));
                Game.Rate.Elixir = Convert.ToByte(ini.GetValue("Rates", "Elixirsrate", 1));
                Game.Rate.Alchemyd = Convert.ToByte(ini.GetValue("Rates", "Alchemyrate", 1));
                Game.Rate.ETCd = Convert.ToByte(ini.GetValue("Rates", "ETCrate", 1));
                Game.Rate.Spawns = Convert.ToByte(ini.GetValue("Rates", "Spawnrate", 1));
                iPortIPC = Convert.ToUInt16(ini.GetValue("IPC", "port", 15780));
                sIpIPC = ini.GetValue("IPC", "ip", "");
                iPortServer = Convert.ToUInt16(ini.GetValue("Server", "port", 15780));
                sIpServer = ini.GetValue("Server", "ip", "");
                iPortCmd = Convert.ToUInt16(ini.GetValue("CMD", "port", 10101));
                Game.Systems.maxSlots = Convert.ToInt32(ini.GetValue("Server", "MaxSlots", 100));
            }
            else
            {
                Game.Rate.Gold = 1;
                Game.Rate.Item = 1;
                Game.Rate.Xp = 1;
                Game.Rate.Sp = 1;
                Game.Rate.Sox = 1;
                Game.Rate.Elixir = 1;
                Game.Rate.Alchemyd = 1;
                Game.Rate.ETCd = 1;
                Game.Rate.Spawns = 1;
                Game.Systems.maxSlots = 100;

            }
            Systems.MsSQL.Connection(sqlConnect);
            // create servers
            try
            {
                net = new Systems.Server();

                net.OnConnect += new Systems.Server.dConnect(_OnClientConnect);
                net.OnError += new Systems.Server.dError(_ServerError);

                Systems.ServerStartedTime = DateTime.Now;

                Systems.Client.OnReceiveData += new Systems.Client.dReceive(_OnReceiveData);
                Systems.Client.OnDisconnect += new Systems.Client.dDisconnect(_OnClientDisconnect);

                #region CommandServer StartUp
                cmd = new Systems.CommandServer();
                cmd.OnCommandReceived += new Systems.CommandServer.dCommandReceived(_command);
                #endregion
                #region IPC Server StartUp
                Systems.IPC = new Servers.IPCServer();
                Systems.IPC.OnReceive += new Servers.IPCServer.dOnReceive(_OnIPC);
                Systems.LoadServers("LoginServers.ini", 15779);
                #endregion

            }
            catch (Exception)
            {
                //Activity(err);
            }

            Game.Systems.CheckDirectories();

            Game.File.FileLoad.Load(this);
            //Update serverlist info
            Game.Systems.clients.update += new EventHandler(Users.updateServerList);
            //Load random unique monsters.
            Random rand = new Random();

            Game.GlobalUnique.StartTGUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn tiger girl
            Game.GlobalUnique.StartUriUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn urichi
            Game.GlobalUnique.StartIsyUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn isy
            Game.GlobalUnique.StartLordUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn lord yarkan
            Game.GlobalUnique.StartDemonUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn demon shaitan
            Game.GlobalUnique.StartCerbUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn cerberus
            Game.GlobalUnique.StartMedusa(rand.Next(10, 20) * 90000, 600);   //Random spawn medusa
            Game.GlobalUnique.StartNeith(rand.Next(10, 20) * 90000, 600);   //Random spawn neith
            //Game.GlobalUnique.StartSphinx       (rand.Next(10, 20) * 90000, 600);   //Random spawn medusa
            Game.GlobalUnique.StartIsis(rand.Next(10, 20) * 90000, 600);   //Random spawn isis
            //Game.GlobalUnique.StartRoc          (rand.Next(10, 20) * 90000, 600);   //Random spawn roc
            Game.GlobalUnique.StartIvyUnique(rand.Next(10, 20) * 60000, 600);   //Random spawn captain ivy

            // start listening servers
            cmd.Start("127.0.0.1", iPortCmd);
            Systems.IPC.Start(sIpIPC, iPortIPC);
            net.Start(sIpServer, iPortServer);
            Systems.UpdateServerInfo();
            // main loop
            lastPromote = DateTime.Now;
            Thread check = new Thread(run);
            check.Start();
            while (check.IsAlive)
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
        public static Systems.Server net;
        public static Systems.CommandServer cmd;
        static bool cancelServer = false;
        static DateTime lastPromote = DateTime.MinValue;

        private void Manager_Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0xa1, 0x2, 0);
            }
        }
        #region Events
        public static void _OnReceiveData(Systems.Decode de)
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
        public static void _OnClientConnect(ref object de, Systems.Client net)
        {
            try
            {
                de = new Systems(net);
            }
            catch (Exception)
            {

            }
        }
        public static void _OnClientDisconnect(object o)
        {
            try
            {
                if (o != null)
                {
                    Systems s = (Systems)o;
                    s.PrintLastPack();
                    s.Disconnect("normal");
                }
            }
            catch (Exception)
            {
                
            }
        }
        private static void _ServerError(Exception ex)
        {
            Console.WriteLine("@Gameserver:         Error:", ex.Message);
            Console.WriteLine("@Gameserver:         {0}", ex.StackTrace);
        }
        private static void db_OnDatabaseError(Exception ex)
        {
            Console.WriteLine("@Gameserver:         Error::{0}", ex);
        }
        private void db_OnConnectedToDatabase()
        {
            Activity("[DATABASE] Connected to database...");
        }
        #endregion
        #region CommandServer
        public static void _command(string aCommand, Socket aSocket)
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
                    Systems.SrevoServerInfo remoteLoginServer = Systems.GetServerByEndPoint(((IPEndPoint)ep).Address.ToString(), pServer);
                    if (remoteLoginServer != null)
                    {
                        // decode data
                        Servers.IPCdeCode(ref data, remoteLoginServer.code);

                        byte pCmd = data[3];
                        int dLen = (int)(data[4] + (data[5] << 8));
                        byte crc = Servers.BCRC(data, data.Length - 1);
                        if (data[data.Length - 1] != crc) // wrong CRC
                        {
                            Activity("[IPC] Wrong Checksum from Server "+ remoteLoginServer.id +". Please Check !");
                            return;
                        }
                        if (data.Length >= (dLen + 6))
                        {
                            if (pCmd == (byte)IPCCommand.IPC_REQUEST_SERVERINFO)
                            {

                                remoteLoginServer.lastPing = DateTime.Now;
                                byte[] rspBuf = Systems.IPC.PacketResponseServerInfo((UInt16)Servers.IPCPort, 1, 100, (UInt16)Systems.clients.Count, (UInt16)Game.Global.Versions.clientVersion);
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
                            sendSocket(aSocket, "  //shutdown" + Environment.NewLine);
                        }
                    }
                    else if (command[0] == "//clear")
                    {
                        System.GC.Collect();
                        GC.Collect(0, GCCollectionMode.Forced);
                        sendSocket(aSocket, "done memory cleanup" + Environment.NewLine);
                    }
                    else if (command[0] == "//manager")
                    {
                        if (aSocket == null)
                        {
                            Application.EnableVisualStyles();
                            Application.Run(new Main());
                        }
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
                            Console.WriteLine("{0}stopping server and sending notice to clients ...", Game.Global.Product.Prefix);
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
                        Game.GlobalUnique.StartTGUnique(6000 * 10, 6000 * 10);
                        Game.GlobalUnique.StartUriUnique(7000 * 10, 7000 * 10);
                        Game.GlobalUnique.StartIsyUnique(8000 * 10, 8000 * 10);
                        Game.GlobalUnique.StartLordUnique(9000 * 10, 9000 * 10);
                        Game.GlobalUnique.StartDemonUnique(10000 * 10, 10000 * 10);
                        Game.GlobalUnique.StartCerbUnique(11000 * 10, 11000 * 10);
                        Game.GlobalUnique.StartIvyUnique(11000 * 10, 11000 * 10);
                        //Game.GlobalUnique.StartRoc(11000 * 10, 11000 * 10);
                        Game.GlobalUnique.StartMedusa(11000 * 10, 11000 * 10);
                        Game.GlobalUnique.StartNeith(11000 * 10, 11000 * 10);
                        //Game.GlobalUnique.StartSphinx(11000 * 10, 11000 * 10);
                        Game.GlobalUnique.StartIsis(11000 * 10, 11000 * 10);
                        sendSocket(aSocket, "done respawn" + Environment.NewLine);
                    }
                    else if (command[0] == "")
                    {
                        string information = aCommand;
                        Game.Systems c = new Game.Systems();
                        Game.Systems.SendAll(c.sendnoticecon(7, 0, information, ""));

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
        public void Activity(string information)
        {
            if (guiActivity.InvokeRequired)
            {
                UpdateActivityBox del = new UpdateActivityBox(Activity);
                this.guiActivity.Invoke(del, new object[] { information });
            }
            else
            {
                this.guiActivity.AppendText(information + " \n");
            }
        }
        public void UpdatePlayers(string name)
        {
            if (guiActivity.InvokeRequired)
            {
                UpdatePlayerList del = new UpdatePlayerList(UpdatePlayers);
                this.srvPlayerlist.Invoke(del, new object[] { name });
            }
            else
            {
                this.srvPlayerlist.Items.Add(name);
            }
        }
        public void UpdatePlayerDEL(string name)
        {
            if (guiActivity.InvokeRequired)
            {
                UpdatePlayerListRemove del = new UpdatePlayerListRemove(UpdatePlayerDEL);
                this.srvPlayerlist.Invoke(del, new object[] { name });
            }
            else
            {
                this.srvPlayerlist.Items.Remove(name);
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Systems.mchat.Show();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the server?", "[SRX] Closing...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Systems.UpdateServerInfo(0);
                Environment.Exit(0);
            }
        }

        private void guiActivity_TextChanged(object sender, EventArgs e)
        {
            this.guiActivity.ScrollToCaret();
        }
    }
}
