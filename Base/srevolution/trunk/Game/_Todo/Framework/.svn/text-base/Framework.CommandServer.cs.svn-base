using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        public class CommandPacket : IDisposable 
        {
            #region [Privates]
            private bool isDisposed = false;
            #endregion
            #region [Publics]
            public int bufPos = 0;
            public byte[] tmpbuf = new byte[8192];
            public Socket clientSocket;
            public string command;
            #endregion
            #region Destructors
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            // ------------------------------------------------------------------------
            //
            protected virtual void Dispose(bool disposing)
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    tmpbuf = null;
                    command = "";
                }
            }

            // ------------------------------------------------------------------------
            //
            ~CommandPacket()
            {
                Dispose(false);
            }
            #endregion        
        }

        public class CommandServer
        {
            public delegate void dCommandReceived(string aCommand, Socket aSocket);
            public delegate void dConnect(Socket aSocket);
            public delegate void dDisconnect(Socket aSocket);

            private Socket listenSocket;
            private List<Socket> socketList=new List<Socket>();

            public event dConnect OnConnect;
            public event dDisconnect OnDisconnect;
            public event dCommandReceived OnCommandReceived;

            #region wrappers
            private void doConnect(CommandPacket pData)
            {
                lock (socketList)
                {
                    socketList.Add(pData.clientSocket);
                }
                if (OnConnect != null)
                {
                    try
                    {
                        OnConnect(pData.clientSocket);
                    }
                    catch { }
                }
            }

            private void doCommandReceived(CommandPacket pData)
            {
                if (OnCommandReceived != null)
                {
                    try
                    {
                        OnCommandReceived(pData.command, pData.clientSocket);
                    }
                    catch { }
                }
            }

            private void doDisconnect(ref CommandPacket pData)
            {
                lock (socketList)
                {
                    try
                    {
                        socketList.Remove(pData.clientSocket);
                    }
                    catch { }
                }
                if (OnDisconnect != null)
                {
                    try
                    {
                        OnDisconnect(pData.clientSocket);
                    }
                    catch { }
                }
                try
                {
                    pData.clientSocket.Close();
                }
                catch { }
                try
                {
                    pData.Dispose();
                    pData = null;
                }
                catch { }

            }
            #endregion

            // ------------------------------------------------------------------------
            //
            public void Start(string ip, int PORT)
            {
                try
                {
                    IPAddress psIP;
                    if (ip != "")
                    {
                        psIP = IPAddress.Parse(ip);
                    }
                    else
                    {
                        psIP = IPAddress.Any;
                    }
                    IPEndPoint psEndPoint = new IPEndPoint(psIP, PORT);
                    listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    listenSocket.Bind(psEndPoint);
                    listenSocket.Listen(1);
                    listenSocket.BeginAccept(new AsyncCallback(ClientConnected), null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[CommandServer.Start] {0}", ex);
                }
            }

            // ------------------------------------------------------------------------
            //
            private void ClientConnected(IAsyncResult aResult)
            {
                try
                {
                    if (!listenSocket.Connected)
                    {
                        try
                        {
                            Socket wSocket = listenSocket.EndAccept(aResult);
                            wSocket.DontFragment = false;

                            CommandPacket pData = new CommandPacket();
                            pData.clientSocket = wSocket;

                            doConnect(pData);

                            wSocket.BeginReceive(pData.tmpbuf, 0, pData.tmpbuf.Length, SocketFlags.None, new AsyncCallback(DataReceived), pData);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("[CommandServer.ClientConnected] {0}", ex);
                        }
                        listenSocket.BeginAccept(new AsyncCallback(ClientConnected), null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[CommandServer.ClientConnected] {0}", ex);
                }
            }

            // ------------------------------------------------------------------------
            //
            private void DataReceived(IAsyncResult aResult)
            {
                CommandPacket pd = (CommandPacket)aResult.AsyncState;
                try
                {
                    if (pd.clientSocket.Connected)
                    {
                        int recvSize = pd.clientSocket.EndReceive(aResult);  // get the count of received bytes
                        if (recvSize > 0)
                        {
                            int inPos = 0;
                            while (inPos<recvSize) 
                            {
                                if (pd.tmpbuf[inPos] == 10)
                                {
                                    doCommandReceived(pd);
                                    pd.command = "";
                                }
                                else
                                {
                                    if ((pd.tmpbuf[inPos] >= 32) && (pd.tmpbuf[inPos] <= 127))
                                    {
                                        pd.command += Encoding.ASCII.GetString(pd.tmpbuf, inPos, 1);
                                    }
                                }
                                inPos++;
                            }
                        }
                        else
                        {   // 0 bytes received, this should be a disconnect
                            doDisconnect(ref pd);
                        }


                        // start the next async read
                        if (pd!=null && pd.clientSocket.Connected)
                        {
                            pd.clientSocket.BeginReceive(pd.tmpbuf, 0, pd.tmpbuf.Length, SocketFlags.None, new AsyncCallback(DataReceived), pd);
                        }
                    }
                    else
                    {
                        doDisconnect(ref pd);
                    }
                }
                catch (Exception ex) // other exceptions
                {
                    Console.WriteLine("[CommandServer.DataReceived] {0}", ex);
                    doDisconnect(ref pd);
                }

            }

        }






    }
}
