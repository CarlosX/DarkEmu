///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Game
{
    public partial class Systems
    {
        // #########################################################################
        //
        public class GamePacket : IDisposable
        {
            #region [Privates]
            private bool isDisposed = false;
            #endregion
            #region [Publics]
            public bool isValid;
            public UInt16 contentSize;
            public UInt16 OPCODE;
            public UInt16 SECURITY;
            public byte[] content;
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
                    isValid = false;
                    content = null;
                }
            }

            // ------------------------------------------------------------------------
            //
            ~GamePacket()
            {
                Dispose(false);
            }
            #endregion
        }

        // #########################################################################
        //
        public class PacketData : IDisposable
        {
            #region [Statics]
            static int DEFAULT_MAX_BUFFER = 8192;
            #endregion
            #region [Privates]
            private bool isDisposed = false;
            private MemoryStream mStream;
            #endregion
            #region [Publics]
            public Socket clientSocket;
            public GamePacket Packet=new GamePacket();
            public object userData;
            public byte[] tmpbuf = new byte[128];
            public string Endpoint = "";
            public int bufPos = 0;
            public BinaryReader memReader;
            #endregion
            #region [Constructors]
            private void _PacketData(Socket aSocket, int bufSize)
            {
                this.clientSocket = aSocket;
                this.userData = null;
                this.Packet.content = new byte[bufSize];
                this.mStream = new MemoryStream(this.Packet.content);
                this.memReader = new BinaryReader(this.mStream);
                this.Packet.isValid = false;
                this.Packet.OPCODE = 0;
                this.Packet.contentSize = 0;
                this.Packet.SECURITY = 0;
                if (aSocket != null) 
                    this.Endpoint = aSocket.RemoteEndPoint.ToString();
            }
            public PacketData(Socket aSocket, int bufSize)
            {
                _PacketData(aSocket, bufSize);
            }
            public PacketData(Socket aSocket)
            {
                _PacketData(aSocket, DEFAULT_MAX_BUFFER);
            }
            #endregion
            #region [Destructors]
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            protected virtual void Dispose(bool disposing)
            {
                if (!isDisposed)
                {
                    isDisposed = true;
                    this.memReader.Dispose();
                    this.mStream.Dispose();
                    Packet = null;
                    userData = null;
                    tmpbuf = null;
                }
            }
            ~PacketData()
            {
                Dispose(false);
            }
            #endregion

            // ------------------------------------------------------------------------
            //
            public void resetReaderPosition()
            {
                if (this.memReader != null)
                {
                    this.memReader.BaseStream.Seek(0, SeekOrigin.Begin);
                }
            }
        }


        // #########################################################################
        //
        public class PacketServer : IDisposable
        {

            #region [Delegates]
            public delegate void dPacketReceived(PacketData userData);
            public delegate void dConnect(PacketData userData);
            public delegate void dError(PacketData userData, Exception ex);
            public delegate void dDisconnect(PacketData userData);
            public delegate void dLogging(PacketData userData, string logText);
            #endregion
            #region [Privates]
            private bool isDisposed = false;
            private Socket listenSocket;
            #endregion
            #region [Publics]
            public event dConnect OnConnect;
            public event dError OnException;
            public event dDisconnect OnDisconnect;
            public event dPacketReceived OnPacketReceived;
            public event dLogging OnLogging;
            public List<Socket> socketList = new List<Socket>();
            #endregion
            #region [Constructors]
            public PacketServer()
            {
            }
            #endregion
            #region [Destructors]
            // ------------------------------------------------------------------------
            //
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
                    OnConnect = null;
                    OnPacketReceived = null;

                    if (listenSocket != null)
                    {
                        try
                        {
                            if (listenSocket.Connected)
                            {
                                try
                                {
                                    listenSocket.Close();
                                }
                                catch { }
                            }
                            listenSocket.Dispose();
                        }
                        catch { }
                    }
                    ((IDisposable)socketList).Dispose();

                    OnDisconnect = null;
                    OnException = null;
                    OnLogging = null;
                }
            }

            // ------------------------------------------------------------------------
            //
            ~PacketServer()
            {
                foreach (Socket aSocket in socketList)
                {
                    try
                    {
                        aSocket.Close();
                    }
                    catch { }
                }
                Dispose(false);
            }            
            #endregion
            #region [wrapper functions]
            private void doLog(PacketData pData, string logText)
            {
                if (OnLogging != null)
                {
                    try
                    {
                        OnLogging(pData, logText);
                    }
                    catch { }
                }
            }

            private void doException(PacketData pData, Exception ex)
            {
                if (OnException != null)
                {
                    try
                    {
                        OnException(pData, ex);
                    }
                    catch { }
                }
            }

            private void doConnect(PacketData pData)
            {
                socketList.Add(pData.clientSocket);
                if (OnConnect != null)
                {
                    try
                    {
                        OnConnect(pData);
                    }
                    catch { }
                }
            }

            private void doDisconnect(PacketData pData)
            {
                doLog(pData, String.Format("[PacketServer] Client {0} disconnects", pData.Endpoint));
                try
                {
                    socketList.Remove(pData.clientSocket);
                }
                catch { }
                if (OnDisconnect != null)
                {
                    try
                    {
                        OnDisconnect(pData);
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
                }
                catch { }

            }

            private void doPacketReceived(PacketData pData)
            {
                if (OnPacketReceived != null)
                {
                    try
                    {
                        OnPacketReceived(pData);
                    }
                    catch { }
                }
            }
            #endregion

            // ------------------------------------------------------------------------
            //
            public void Start(string ip, int PORT)
            {
                try
                {
                    IPAddress.Parse(ip);
                    IPEndPoint psEndPoint = new IPEndPoint(IPAddress.Any, PORT);
                    listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    listenSocket.Bind(psEndPoint);
                    listenSocket.Listen(5);
                    listenSocket.BeginAccept(new AsyncCallback(ClientConnected), null);
                }
                catch (Exception ex)
                {
                    if (OnException != null)
                    {
                        OnException(null, ex);
                    } 
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

                            PacketData pData = new PacketData(wSocket);

                            doLog(pData, String.Format("[PacketServer] client connected from {0}", wSocket.RemoteEndPoint));
                            doConnect(pData);

                            wSocket.BeginReceive(pData.tmpbuf, 0, pData.tmpbuf.Length, SocketFlags.None, new AsyncCallback(DataReceived), pData);
                        }
                        catch (Exception ex)
                        {
                            doException(null, ex);
                        }
                        listenSocket.BeginAccept(new AsyncCallback(ClientConnected), null);
                    }
                }
                catch (Exception ex)
                {
                    doException(null, ex);
                }
            }

            // ------------------------------------------------------------------------
            //
            private void DataReceived(IAsyncResult aResult)
            {
                PacketData pd = (PacketData)aResult.AsyncState;
                try
                {
                    if (pd.clientSocket.Connected)
                    {
                        bool checkData = true;
                        int recvSize = pd.clientSocket.EndReceive(aResult);  // get the count of received bytes
                        if (recvSize > 0)
                        {
                            if ((recvSize + pd.bufPos) > pd.Packet.content.Length)  // that may be a try to force buffer overflow, we don't allow that ;)
                            {
                                doLog(pd, String.Format("Buffer exhausted in client ReceiveData (maybe buffer overflow try)!!!"));
                                doDisconnect(pd);
                                checkData = false;
                            }
                            else
                            {  // we have something in input buffer and it is not beyond our limits
                                Buffer.BlockCopy(pd.tmpbuf, 0, pd.Packet.content, pd.bufPos, recvSize); // copy the new data to our buffer
                                pd.bufPos += recvSize; // increase our buffer-counter
                            }
                            doLog(pd, String.Format("added {0} bytes to buffer, now buffer is {1} in size", recvSize, pd.bufPos));
                        }
                        else
                        {   // 0 bytes received, this should be a disconnect
                            doDisconnect(pd);
                            checkData = false;
                        }

                        while (checkData) // repeat while we have 
                        {
                            checkData = false;
                            if (pd.bufPos >= 6) // a minimum of 6 byte is required for us
                            {
                                pd.resetReaderPosition();
                                pd.Packet.contentSize = pd.memReader.ReadUInt16();
                                if (pd.bufPos >= (6 + pd.Packet.contentSize))  // that's a complete packet, lets call the handler
                                {
                                    pd.Packet.OPCODE = pd.memReader.ReadUInt16();
                                    pd.Packet.SECURITY = pd.memReader.ReadUInt16();
                                    pd.Packet.isValid = true;
                                    // remove header from buffer
                                    Buffer.BlockCopy(pd.Packet.content, 6, pd.Packet.content, 0, pd.bufPos - 6);
                                    pd.bufPos -= 6;
                                    pd.resetReaderPosition();
                                    doPacketReceived(pd); // call the handling routine
                                    pd.Packet.isValid = false;
                                    pd.bufPos -= pd.Packet.contentSize; // decrease buffer-counter
                                    if (pd.bufPos > 0) // was the buffer greater than the packet needs ? then it may be the next packet
                                    {
                                        Buffer.BlockCopy(pd.Packet.content, pd.Packet.contentSize, pd.Packet.content, 0, pd.bufPos); // move the rest to buffer start
                                        checkData = true; // loop for next packet
                                        doLog(pd,"repeated packet handling (buffer already contains next packet)");
                                    }
                                    doLog(pd, String.Format("Packet completed, starting next one. Buffer starts with size of {0}", pd.bufPos));
                                }
                            }
                        }
                        // start the next async read
                        if (pd.clientSocket.Connected)
                        {
                            pd.clientSocket.BeginReceive(pd.tmpbuf, 0, pd.tmpbuf.Length, SocketFlags.None, new AsyncCallback(DataReceived), pd);
                        }
                    }
                    else
                    {
                        doDisconnect(pd);
                    }
                }
                catch (Exception ex) // other exceptions
                {
                    doException(pd, ex);
                    doDisconnect(pd);
                }

            }
        }
    }
}
