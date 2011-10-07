///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        public static int MAX_BUFFER = 8192;
        public class Server
        {
            public bool sockdone;
            public delegate void dReceive(Decode de);
            public delegate void dConnect(ref object de, Client net);
            public delegate void dError(Exception ex);
            public delegate void dDisconnect(object o);

            public event dConnect OnConnect;
            public event dError OnError;

            Socket serverSocket;

            public void Start(string ip, int PORT)
            {
                try
                {
                    IPAddress myIp=IPAddress.Any;
                    if (ip != "")
                    {
                        myIp = IPAddress.Parse(ip);
                    }
                    IPEndPoint EndPoint = new IPEndPoint(myIp, PORT);
                    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    serverSocket.Bind(EndPoint);
                    serverSocket.Listen(5);
                    serverSocket.BeginAccept(new AsyncCallback(ClientConnect), serverSocket);
                    MsSQL.UpdateData("UPDATE users SET online='0'");
                    MsSQL.UpdateData("UPDATE server SET users_current='0'");
                }
                catch (Exception ex)
                {
                    OnError(ex);
                }
                finally { }
            }
            public void Stop()
            {
                serverSocket.Close();
            }
            public void ServerCheck(bool state)
            {
                MsSQL.UpdateData(String.Format("UPDATE server SET state={0}", state ? 0 : 1));
            }
            private void ClientConnect(IAsyncResult ar)
            {
                Socket handlingSocket = (Socket)ar.AsyncState;
                try
                {
                    Socket wSocket = handlingSocket.EndAccept(ar);

                    wSocket.DontFragment = false;

                    //Console.WriteLine("client connected from {0}", wSocket.RemoteEndPoint);

                    object p = null;
                    Client Player = new Client();

                    try
                    {
                        OnConnect(ref p, Player);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Server.ClientConnect.OnConnect] {0}", ex);
                    }

                    Player.Packets = p;
                    Player.clientSocket = wSocket;

                    handlingSocket.BeginAccept(new AsyncCallback(ClientConnect), handlingSocket);

                    try
                    {
                        wSocket.Send(new byte[] { 0x01, 0x00, 0x00, 0x50, 0x00, 0x00, 0x01 });
                        wSocket.BeginReceive(Player.tmpbuf, 0, Player.tmpbuf.Length, SocketFlags.None, new AsyncCallback(Player.ReceiveData), wSocket);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.ErrorCode == 10054) // client did not wait for accept and gone
                        {
                            // ToDo: OnDisconnect
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Server.ClientConnect.Send+Receive] {0}", ex);
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    Systems.Debugger.Write(ex);
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10054) // Accept failed because client is no longer trying to connect, thus start new Accept
                    {
                        handlingSocket.BeginAccept(new AsyncCallback(ClientConnect), handlingSocket);
                    }
                    else
                    {
                        Console.WriteLine("[Server.ClientConnect.SocketException] {0}", ex);
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Server.ClientConnect.Exception] {0}", ex);
                    OnError(ex);
                }
            }
        }
        public class Client
        {
            public bool State;
            public delegate void dReceive(Decode de);
            public delegate void dDisconnect(object o);

            public static event dReceive OnReceiveData;
            public static event dDisconnect OnDisconnect;
            public Socket clientSocket;

            public object Packets { get; set; }
            public int bufCount = 0; // packet buffer used
            public byte[] buffer = new byte[MAX_BUFFER]; // packet buffer
            public byte[] tmpbuf = new byte[128]; // async read buffer
            public bList<byte[]> BuffList = new bList<byte[]>();
            public void ReceiveData(IAsyncResult ar)
            {
                Socket wSocket = (Socket)ar.AsyncState;
                try
                {
                    if (wSocket.Connected)
                    {
                        int recvSize = wSocket.EndReceive(ar);  // get the count of received bytes
                        bool checkData = true;
                        if (recvSize > 0)
                        {
                            if ((recvSize + bufCount) > MAX_BUFFER)  // that may be a try to force buffer overflow, we don't allow that ;)
                            {
                                checkData = false;
                                LocalDisconnect(wSocket);
                            }
                            else
                            {  // we have something in input buffer and it is not beyond our limits
                                Buffer.BlockCopy(tmpbuf, 0, buffer, bufCount, recvSize); // copy the new data to our buffer
                                bufCount += recvSize; // increase our buffer-counter
                            }
                        }
                        else
                        {   // 0 bytes received, this should be a disconnect
                            checkData = false;
                            LocalDisconnect(wSocket);
                        }

                        while (checkData) // repeat while we have 
                        {
                            checkData = false;
                            if (bufCount >= 6) // a minimum of 6 byte is required for us
                            {
                                Decode de = new Decode(buffer);
                                if (bufCount >= (6 + de.dataSize))  // that's a complete packet, lets call the handler
                                {
                                    de = new Decode(wSocket, buffer, this, Packets);  // build up the Decode structure for next step
                                    OnReceiveData(de); // call the handling routine
                                    //Console.WriteLine("[CLIENT PACKET] {0}", BytesToString(buffer));
                                    bufCount -= (6 + de.dataSize); // decrease buffer-counter
                                    if (bufCount > 0) // was the buffer greater than the packet needs ? then it may be the next packet
                                    {
                                        Buffer.BlockCopy(buffer, 6 + de.dataSize, buffer, 0, bufCount); // move the rest to buffer start
                                        checkData = true; // loop for next packet
                                    }
                                }
                                de = null;
                            }
                        }
                        // start the next async read
                        if (wSocket!=null && wSocket.Connected)
                        {
                            wSocket.BeginReceive(tmpbuf, 0, tmpbuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), wSocket);
                            State = true;
                        }
                    }
                    else
                    {
                        LocalDisconnect(wSocket);
                    }
                }
                catch (SocketException se)  // explicit handling of SocketException
                {
                    if (se.ErrorCode == 10054)
                    {
                        State = false;
                    }
                    LocalDisconnect(wSocket);
                }
                catch (Exception ex) // other exceptions
                {
                    State = false;
                    Console.WriteLine("Error in client ReceiveData: {0}", ex);
                    LocalDisconnect(wSocket);
                }

            }

            public string BytesToString(byte[] buff)
            {
                string pack = null;
                System.IO.MemoryStream ms = new System.IO.MemoryStream(buff);
                System.IO.BinaryReader br = new System.IO.BinaryReader(ms);
                ushort datasize = br.ReadUInt16();
                ushort opcode = br.ReadUInt16();
                br.ReadUInt16();
                pack = String.Format("{0}->{1}", opcode.ToString("X2"), Decode.StringToPack(br.ReadBytes(datasize)));
                return pack;
            }

            public void Send(byte[] buff)
            {
                try
                {
                    if (clientSocket.Connected && buff != null)
                    {
                        while (this.BuffList.Count > 100) // to avoid memory leaks only store last 100 packets, think that woule be enough
                        {
                            this.BuffList.RemoveAt(0);
                        }
                        this.BuffList.Add(buff);

                        if (buff.Length > 0 && clientSocket.Connected) 
                            clientSocket.Send(buff);
                        //Console.WriteLine("[SERVER PACKET] {0}", BytesToString(buff));
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                    System.IO.StreamWriter Writer = System.IO.File.AppendText(Environment.CurrentDirectory + @"\Error List.txt");
                    Writer.WriteLine(ex);
                    Writer.Close();
                    Console.WriteLine(ex.StackTrace);
                }
                catch (ObjectDisposedException exx)
                {
                    Console.WriteLine(exx);
                    System.IO.StreamWriter Writer = System.IO.File.AppendText(Environment.CurrentDirectory + @"\Error List.txt");
                    Writer.WriteLine(exx);
                    Writer.Close();
                    Console.WriteLine(exx.StackTrace);
                }
                catch (ArgumentNullException exxx)
                {
                    Console.WriteLine(exxx);
                    System.IO.StreamWriter Writer = System.IO.File.AppendText(Environment.CurrentDirectory + @"\Error List.txt");
                    Writer.WriteLine(exxx);
                    Writer.Close();
                    Console.WriteLine(exxx.StackTrace);
                }
                catch (Exception x)
                {
                    Console.WriteLine(x);
                    System.IO.StreamWriter Writer = System.IO.File.AppendText(Environment.CurrentDirectory + @"\Error List.txt");
                    Writer.WriteLine(x);
                    Writer.Close();
                    Console.WriteLine(x.StackTrace);
                }
               
            }

            void LocalDisconnect(Socket s)
            {
                if (s != null)
                {
                    try
                    {
                        if (OnDisconnect != null)
                        {
                            OnDisconnect(this.Packets);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Client.LocalDisconnect] {0}", ex);
                    }
                }
            }

            public void Disconnect(Socket s)
            {
                try
                {
                    if (s != null && s.Connected)
                    {
                        s.Disconnect(true);
                    }
                    //s.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Client.Disconnect] {0}", ex);
                }
            }
            public void Close()
            {
                clientSocket.Close();
                Array.Clear(buffer, 0, buffer.Length);
            }
        }
    }
}