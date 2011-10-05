///////////////////////////////////////////////////////////////////////////
// SRX Revo: Users
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace SrxRevo
{
    public class Users
    {
        Socket userSocket;
        static Users user;
        byte[] buffer = new byte[8096];

        public static void updateServerList(object List, EventArgs a)
        {
            Systems.UpdateServerInfo();
        }
        
        public void ClientReceive(IAsyncResult ar)
        {
            try
            {
               if (userSocket.Connected) 
                    userSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ClientReceive), userSocket);
            }
            catch (Exception) { }
        }
    }
}
