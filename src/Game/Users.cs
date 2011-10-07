///////////////////////////////////////////////////////////////////////////
// DarkEmu: Users
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace DarkEmu_GameServer
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
