///////////////////////////////////////////////////////////////////////////
// PROJECT SRX
// HOMEPAGE: WWW.XCODING.NET
///////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Net;
using SRX.Network;
using System.Text;
using LoginServer;
using System.Threading;
using LoginServer.Global;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace LoginServer
{
    public partial class Maincs : Form
    {
        //Enable form dragging
        #region Drag windowless form
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();
        #endregion

        //Definitions and forms
        #region Definitions and forms
        Manage_Servers servers = new Manage_Servers();
        Manage_News newsform = new Manage_News();
        public static Servers.IPCServer IPCServer;
        public static Dictionary<UInt16, IPCItem> IPCResultList = new Dictionary<UInt16, IPCItem>();
        public static UInt16 IPCNewId = 0;
        public static int IPCPort = 15779;
        delegate void UpdateActivityBox(string chat);

        public bool information;

        public class IPCItem
        {
            public UInt16 resultCode;
            public string banReason;
        }
        #endregion

        //Run
        #region run
        public Maincs()
        {
            //Set default settings
            #region Default Settings
            int LSPort  = 15779;
            int IPCPort = 15779;
            string LSIP = "";
            string IPCIP = "0.0.0.0";
            Systems.DownloadServer = "";
            #endregion

            //Initialize main
            InitializeComponent();
            //Set ini
            LoginServer.Systems.Ini ini = null;
            //Load settings
            #region Load Settings
            try
            {
                if (File.Exists(Environment.CurrentDirectory + @"\Settings\Settings.ini"))
                {
                    ini = new LoginServer.Systems.Ini(Environment.CurrentDirectory + @"\Settings\Settings.ini");
                    LSPort = Convert.ToInt32(ini.GetValue("Server", "port", 15779));
                    LSIP = ini.GetValue("Server", "ip", "").ToString();
                    IPCPort = Convert.ToInt32(ini.GetValue("IPC", "port", 15779));
                    IPCIP = ini.GetValue("IPC", "ip", "").ToString();
                    Systems.DownloadServer = ini.GetValue("Patch_Server", "ip", "");
                    Systems.DownloadPort = Convert.ToInt16(ini.GetValue("Patch_Server", "port", ""));
                    ini = null;
                    Activity("[INFO] Loaded your ip settings successfully");
                }
                else
                {
                    Activity("Could not find your settings.ini using defaults.");
                }
            }
            catch (Exception)
            {
                return;
            }
            #endregion

            #region get local ip
           
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

            net.OnConnect += new Systems.Server.dConnect(_OnClientConnect);
            net.OnError += new Systems.Server.dError(_ServerError);

            Systems.Client.OnReceiveData += new Systems.Client.dReceive(_OnReceiveData);
            Systems.Client.OnDisconnect += new Systems.Client.dDisconnect(_OnClientDisconnect);
            //Content.Patches.LoadPatches(this);
            Systems.Load_NewsList(this, newsform);

            try
            {
                net.Start(LSIP, LSPort);
            }
            catch (Exception ex)
            {
                Activity("Starting Server error "+ ex +"");
            }

            #region Load GameServers
            Systems.LoadServers("GameServers.ini", 15780, this, servers);
            #endregion

            #region IPC Listener
            IPCServer = new Servers.IPCServer();
            IPCServer.OnReceive += new Servers.IPCServer.dOnReceive(OnIPC);
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
                Activity("IPC start error " + ex + "");
            }
            #endregion
            Activity("[NET] Listening for gameserver connection...");
            this.ShowDialog();
            while (true)
            {
                Thread.Sleep(100);
                foreach (KeyValuePair<int, Systems.SRX_Serverinfo> SSI in Systems.GSList)
                {
                    if (SSI.Value.status != 0 && SSI.Value.lastPing.AddMinutes(5) < DateTime.Now) // server unavailable
                    {
                        SSI.Value.status = 0;
                        Activity("[ERROR] Server "+ SSI.Value.name +" has timed out");
                    }
                }
            }
        }
        #endregion

        //Form related
        #region Form
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
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the server?", "[SRX] Closing...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }
        private void Maincs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0xa1, 0x2, 0);
            }
        }
        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        #endregion

        //IPC & Data
        #region IPC & Data
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
                            Activity("[ERROR] Code for: "+ remoteGameServer.name +" does not match");
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
                                    Activity("[NET] " + remoteGameServer.name + ": players online " + remoteGameServer.usedSlots + "/" + remoteGameServer.maxSlots +"");
                                    if (remoteGameServer.status == 0 && data[6] != 0)
                                    {
                                        Activity("[NET] Server: " + remoteGameServer.name + " is now online");
                                    }
                                    if (remoteGameServer.status != 0 && data[6] == 0)
                                    {
                                        Activity("[NET] Server: " + remoteGameServer.name + " is now in check state");
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
                                            Activity("[ERROR] ResultList mismatch");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Activity("[ERROR] unknown command recevied");
                            }
                        }
                        else
                        {
                            Activity("[ERROR] data to short");
                        }
                    }
                    else
                    {
                        Activity("[ERROR] can't find the GameServer "+ ((IPEndPoint)ep).Address.ToString() +"");
                    }

                }
                else
                {
                    Activity("[ERROR] packet to short from "+ ep.ToString() +"");
                }
            }
            catch (Exception ex)
            {
                Activity("[ERROR] "+ ex +"");
            }
        }

        public static void _OnReceiveData(LoginServer.Systems.Decode de)
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
        public void _ServerError(Exception ex)
        {
            Activity("[ERROR] Server Error: "+ ex +"");
        }
        #endregion

        private void label3_Click(object sender, EventArgs e)
        {
            servers.ShowDialog();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            newsform.ShowDialog();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void guiActivity_TextChanged(object sender, EventArgs e)
        {
            this.guiActivity.ScrollToCaret();
        }
    }
}