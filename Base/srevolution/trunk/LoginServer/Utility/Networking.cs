using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace LoginServer
{
    public partial class Systems
    {
        public static int MAX_BUFFER = 8192;
        public class Server
        {
            public delegate void dReceive(Decode de);
            public delegate void dConnect(ref object de, Client net);
            public delegate void dError(Exception ex);
            public delegate void dDisconnect(object o);

            public event dConnect OnConnect;
            public event dError OnError;

            Socket serverSocket;

            public void Start(string ip, int PORT)
            {
                IPAddress myIP = IPAddress.Any;
                if (ip != "")
                {
                    myIP = IPAddress.Parse(ip);
                }
                IPEndPoint EndPoint = new IPEndPoint(myIP, PORT);
                
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    serverSocket.Bind(EndPoint);
                    serverSocket.Listen(5);
                    serverSocket.BeginAccept(new AsyncCallback(ClientConnect), null);
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10049)
                    {
                    }
                    else if (ex.ErrorCode == 10048)
                    {
                    } 
                    else 
                    {
                    }
                }
                catch (Exception ex)
                {
                    OnError(ex);
                }
                finally { }
            }
            private void ClientConnect(IAsyncResult ar)
            {
                try
                {
                    Socket wSocket = serverSocket.EndAccept(ar);
                    wSocket.DontFragment = false;


                    object p = null;
                    Client Player = new Client();
                    try
                    {
                        OnConnect(ref p, Player);
                    }
                    catch (Exception)
                    {
                        
                    }

                    Player.Packets = p;
                    Player.clientSocket = wSocket;

                    serverSocket.BeginAccept(new AsyncCallback(ClientConnect), null);
                    try
                    {
                        wSocket.Send(new byte[] { 0x01, 0x00, 0x00, 0x50, 0x00, 0x00, 0x01 });
                        wSocket.BeginReceive(Player.tmpbuf, 0, Player.tmpbuf.Length, SocketFlags.None, new AsyncCallback(Player.ReceiveData), wSocket);
                    }
                    catch (SocketException){}
                    catch (Exception)
                    {
                    }
                    
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception ex)
                {
                    OnError(ex);
                }

            }
        }
        public class Client
        {
            public delegate void dReceive(Decode de);
            public delegate void dDisconnect(object o);

            public static event dReceive OnReceiveData;
            public static event dDisconnect OnDisconnect;
            public Socket clientSocket;

            public object Packets { get; set; }
            public int bufCount = 0;
            public byte[] buffer = new byte[MAX_BUFFER];
            public byte[] tmpbuf = new byte[128];
            public int Version = 0;

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
                                Decode de = new Decode(buffer); // only get get the size first
                                if (bufCount >= (6 + de.dataSize))  // that's a complete packet, lets call the handler
                                {
                                    de = new Decode(wSocket, buffer, this, Packets);  // build up the Decode structure for next step
                                    OnReceiveData(de); // call the handling routine
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
                        if (wSocket != null && wSocket.Connected)
                        {
                            wSocket.BeginReceive(tmpbuf, 0, tmpbuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), wSocket);
                        }
                    }
                    else
                    {
                        LocalDisconnect(wSocket);
                    }
                }
                catch (SocketException)  // explicit handling of SocketException
                {
                    LocalDisconnect(wSocket);
                }
                catch (Exception) // other exceptions
                {
                    LocalDisconnect(wSocket);
                }
            }
            public void Send(byte[] buff)
            {
                try
                {
                    if (buff!=null && buff.Length>0 && clientSocket.Connected)
                    {
                        clientSocket.Send(buff);
                    }
                }
                catch (Exception)
                {
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
                    catch (Exception)
                    {
                    }
                }
            }

            public void Disconnect(Socket s)
            {
                if (s.Connected)
                {
                    s.Shutdown(SocketShutdown.Both);
                    s.Disconnect(true);
                    s.Close();
                }
            }
        }
    }
}
