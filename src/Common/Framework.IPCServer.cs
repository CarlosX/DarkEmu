using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

namespace DarkEmu_GameServer.Network
{
    public enum IPCCommand { IPC_REQUEST_SERVERINFO, IPC_REQUEST_LOGIN, IPC_INFO_SERVER, IPC_INFO_LOGIN }

    public class IPCPacket : IDisposable
    {
        MemoryStream ms;
        BinaryWriter bw;

        public IPCPacket()
        {
            ms = new MemoryStream();
            bw = new BinaryWriter(ms);
        }

        ~IPCPacket()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (ms != null)
            {
                bw.Close();
                ms.Close();
                bw = null;
                ms = null;
            }
        }

        public void WriteByte(int aByte) 
        {
            bw.Write((byte)aByte);
        }

        public void WriteWord(int aWord)
        {
            bw.Write((UInt16)aWord);
        }

        public void WriteString(string aString)
        {
            WriteByte(aString.Length);
            bw.Write(Encoding.ASCII.GetBytes(aString));
        }

        public void WriteBytes(byte[] aBytes)
        {
            bw.Write(aBytes);
        }

        public void AddCRC()
        {
            long oPos = ms.Position;
            ms.Position = 0;
            bw.Flush();
            byte crc = Servers.BCRC(ms.ToArray());
            ms.Position = oPos;
            WriteByte(crc);
        }

        public byte[] GetBytes()
        {
            long oPos = ms.Position;
            ms.Position = 0;
            bw.Flush();
            byte[] arr = ms.ToArray();
            ms.Position = oPos;
            return arr;
        }
    }

    public partial class Servers
    {
        public static int IPCPort = 0;

        public static byte BCRC(byte[] aBytes)
        {
            return BCRC(aBytes, aBytes.Length);
        }

        public static byte BCRC(byte[] aBytes, int aLen)
        {
            byte crc = 0;
            for (int i = 0; i < aLen; i++)
            {
                crc ^= aBytes[i];
            }
            return crc;
        }


        public static void IPCdeCode(ref byte[] data, string code, int aLen)
        {
            if (code != "")
            {
                IPCenCode(ref data, code, aLen);
            }
        }

        public static void IPCdeCode(ref byte[] data, string code)
        {
            IPCdeCode(ref data, code, data.Length);
        }        
        
        public static void IPCenCode(ref byte[] data, string code, int aLen)
        {
            if (code != "")
            {
                int keyindex = 0;
                byte vKey = (byte)(data[0] ^ data[1] ^ data[2]);
                string ret = string.Empty;
                byte[] key = (new System.Text.ASCIIEncoding()).GetBytes(code);
                for (int i = 3; i < aLen; i++)
                {
                    data[i] = (byte)(data[i] ^ key[keyindex++] ^ 0x96 ^ i ^ vKey);
                    if (keyindex >= key.Length)
                    {
                        keyindex = 0;
                    }
                }
            }
        }

        public static void IPCenCode(ref byte[] data, string code)
        {
            IPCenCode(ref data, code, data.Length);
        }
        

        public class IPCServer
        {
            public static int MAX_BUFFER = 8192;
            private Random rnd = new Random();
            public Socket theServer;
            Socket sendSocket = null;
            public byte[] buf = new byte[MAX_BUFFER];

            public delegate void dOnReceive(Socket aServerSocket, EndPoint remoteEndPoint, byte[] data);

            public event dOnReceive OnReceive;

            public IPCServer()
            {
            }

            public void Start(string listenIP, int PORT)
            {
                IPCPort = PORT;
                IPAddress ip = IPAddress.Any;
                if (listenIP != "")
                {
                    ip = IPAddress.Parse(listenIP);
                }
                IPEndPoint ep = new IPEndPoint(ip, PORT);

                // create remote end point for reference
                IPEndPoint rep = new IPEndPoint(IPAddress.Any, 0);
                EndPoint xep = (EndPoint)rep;

                // create udp socket and bind it
                try
                {
                    theServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    theServer.Bind(ep);

                    try
                    {
                        theServer.BeginReceiveFrom(buf, 0, buf.Length, SocketFlags.None, ref xep, new AsyncCallback(UdpReceiveCallback), theServer);
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine("IPC Close");
                        theServer.Close();
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10048)
                    {
                        
                    }
                    else
                    {
                    }
                }
                catch (Exception)
                {
                }

                try
                {
                    sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                }
                catch (Exception)
                {
                    sendSocket = null;
                }
            }

            public void UdpReceiveCallback(IAsyncResult ar)
            {
                Socket u = (Socket)ar.AsyncState;
                IPEndPoint e=new IPEndPoint(IPAddress.Any, 0);
                EndPoint tmpEP = (EndPoint)e;

                try
                {

                    int nBytes = u.EndReceiveFrom(ar, ref tmpEP);
                    if (nBytes > 0)
                    {
                        try
                        {
                            if (OnReceive != null)
                            {
                                byte[] newbuf = new byte[nBytes];
                                Buffer.BlockCopy(buf, 0, newbuf, 0, nBytes);
                                try
                                {
                                    OnReceive(u, tmpEP, newbuf);
                                }
                                catch (Exception) 
                                { 
                                }
                                newbuf = null;
                            }
                            else
                            {
                            }
                        }
                        catch (Exception)
                        {
                        }
                        u.BeginReceiveFrom(buf, 0, buf.Length, SocketFlags.None, ref tmpEP, new AsyncCallback(UdpReceiveCallback), u);
                    }
                    else
                    {
                        theServer.Close();
                    }
                }
                catch (SocketException sex)
                {
                    if (sex.ErrorCode == 10054) // exception thrown when udp send is not possible (ICMP response: port unreachable)
                    {
                        u.BeginReceiveFrom(buf, 0, buf.Length, SocketFlags.None, ref tmpEP, new AsyncCallback(UdpReceiveCallback), u);
                    }
                    else
                    {
                    }
                }
                catch (Exception)
                {

                    theServer.Close();
                }
            }

            public void Send(string destIP, int destPort, byte[] data)
            {
                try
                {
                    IPAddress rip = IPAddress.Parse(destIP);
                    IPEndPoint ep = new IPEndPoint(rip, destPort);

                    //Console.WriteLine("[IPC] sending data to {0}", ep.ToString());
                    sendSocket.SendTo(data, ep);
                    ep = null;
                }
                catch (Exception)
                {
                }
            }

            public byte[] PacketRequestServerInfo(int iPort)
            {
                byte[] b;
                using (IPCPacket IPP = new IPCPacket())
                {
                    IPP.WriteWord(iPort);
                    IPP.WriteByte(rnd.Next(1, 250));
                    IPP.WriteByte((byte)IPCCommand.IPC_REQUEST_SERVERINFO);
                    IPP.WriteWord(0);
                    IPP.AddCRC();
                    b = IPP.GetBytes();
                }
                return b;
            }

            public byte[] PacketResponseServerInfo(int iPort, byte bStatus, UInt16 iMaxSlots, UInt16 iUsedSlots, UInt16 iVersion)
            {
                byte[] b;
                using (IPCPacket IPP = new IPCPacket())
                {

                    IPP.WriteWord(iPort);
                    IPP.WriteByte(rnd.Next(1, 250));
                    IPP.WriteByte((byte)IPCCommand.IPC_INFO_SERVER);
                    IPP.WriteWord(5);
                    IPP.WriteByte(bStatus);
                    IPP.WriteWord(iMaxSlots);
                    IPP.WriteWord(iUsedSlots);
                    IPP.WriteWord(iVersion);
                    IPP.AddCRC();
                    b = IPP.GetBytes();
                }
                return b;
            }

            public byte[] PacketRequestLogin(int iPort, string sUserID, string sPassword, UInt16 IPCid)
            {
                int dLen = sUserID.Length + sPassword.Length + 4;
                byte[] b;
                using (IPCPacket IPP = new IPCPacket())
                {
                    IPP.WriteWord(iPort);
                    IPP.WriteByte(rnd.Next(1,250));
                    IPP.WriteByte((byte)IPCCommand.IPC_REQUEST_LOGIN);
                    IPP.WriteWord(dLen);
                    IPP.WriteString(sUserID);
                    IPP.WriteString(sPassword);
                    IPP.WriteWord(IPCid);
                    IPP.AddCRC();
                    b = IPP.GetBytes();
                }
                return b;
            }

            public byte[] PacketResponseLogin(int iPort, UInt16 wResult, UInt16 wID)
            {
                return PacketResponseLogin(iPort, wResult, wID, "");
            }

            public byte[] PacketResponseLogin(int iPort, UInt16 wResult, UInt16 wID, string sBanReason)
            {
                int dLen = sBanReason.Length + 5;
                byte[] b;
                using (IPCPacket IPP = new IPCPacket())
                {
                    IPP.WriteWord(iPort);
                    IPP.WriteByte(rnd.Next(1, 250));
                    IPP.WriteByte((byte)IPCCommand.IPC_INFO_LOGIN);
                    IPP.WriteWord(dLen);
                    IPP.WriteWord(wID);
                    IPP.WriteWord(wResult);
                    IPP.WriteString(sBanReason);
                    IPP.AddCRC();
                    b = IPP.GetBytes();
                }
                return b;
            }
        }
    }
}
