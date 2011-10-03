/*    <DarkEmu LoginServer>
    Copyright (C) <2011>  <DarkEmu>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Net;
using System.Net.Sockets;

namespace DarkEmu_LoginServer
{
    public class ServerSocket
    {
        private static Socket[] ClientList = new Socket[30001];
        private static Socket winSock;

        private static IPAddress ServerIP;
        private static int ServerPort;

        private static byte[] RecvBuffer = new byte[8024];

        public ServerSocket(string IpAdress, int Port)
        {
            ServerIP = IPAddress.Parse(IpAdress);
            ServerPort = Port;
        }

        ~ServerSocket()
        {
            for (int i = 0; i < CountPlayers(); i++)
                DisconnectSocket(i);
            ServerIP = IPAddress.None;
            ServerPort = 0;
        }


        public static void AddClient(Socket socket)
        {
            for (int i = 0; i < ClientList.Length; i++)
            {
                if (ClientList[i] == null)
                {
                    ClientList[i] = socket;
                    return;
                }
            }
        }
        public static void DeleteClient(int Index)
        {
            if (ClientList[Index] != null)
            {
                ClientList[Index] = null;
            }
        }
        public static int FindIndex(Socket socket)
        {
            for (int i = 0; i < ClientList.Length; i++)
            {
                if (socket == ClientList[i])
                {
                    return i;
                }
            }
            return -1;
        }
        public static Socket GetSocket(int Index)
        {
            Socket socket = null;
            if (ClientList[Index] != null && ClientList[Index].Connected)
            {
                socket = ClientList[Index];
            }
            return socket;
        }

        public void Start()
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, ServerPort);
            winSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                winSock.Bind(localEP);
                winSock.Listen(5);
                winSock.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Started the server succesfully at {0}", DateTime.Now.ToString());
                Console.WriteLine("-------------------------------------------------------");

            }
        }


        private void OnClientConnect(IAsyncResult ar)
        {
            try
            {
                Socket winSock2 = winSock.EndAccept(ar);

                AddClient(winSock2);

                int Index = FindIndex(winSock2);
                Silkroad.SendHandshake(Index);

                winSock2.BeginReceive(RecvBuffer, 0, RecvBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveData), winSock2);
                winSock.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void OnReceiveData(IAsyncResult ar)
        {
            Socket asyncState = (Socket)ar.AsyncState;
            int Index = FindIndex(asyncState);
            if (asyncState.Connected)
            {
                try
                {
                    if (asyncState.EndReceive(ar) > 0)
                    {
                        LoginSocket.ProcessData(RecvBuffer, Index);
                        Array.Clear(RecvBuffer, 0, RecvBuffer.Length);
                    }
                    asyncState.BeginReceive(RecvBuffer, 0, RecvBuffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveData), asyncState);
                }
                catch (SocketException Sex)
                {
                    if (Sex.ErrorCode == 10054)//Connection reset by peer                    
                        DeleteClient(Index);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void Send(byte[] buffer, int Index)
        {
            GetSocket(Index).Send(buffer);
        }

        public static void DisconnectSocket(int Index)
        {
            Socket closingSocket = GetSocket(Index);
            closingSocket.Shutdown(SocketShutdown.Both);
            DeleteClient(Index);
        }

        public static void Close()
        {
            winSock.Close();
        }

        public static int CountPlayers()
        {
            int j = 0;
            for (int i = 0; i <= 3000; i++)
            {
                Socket socket = GetSocket(i);
                if (socket != null && socket.Connected)
                {
                    j++;
                }
            }
            return j;
        }

    }
}

