///////////////////////////////////////////////////////////////////////////
// DarkEmu: Send information
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////

using System;
using Framework;
using System.IO;
using System.Linq;
using System.Text;
using DarkEmu_GameServer.Network;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }

        public void Send(byte[] buff)
        {
            try
            {
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            if (Systems.clients[i] != null)
                            {
                                if (Systems.clients[i] != this)
                                {
                                    if (Character.Spawned(Systems.clients[i].Character.Information.UniqueID) && Character.InGame)
                                    {
                                        if (Systems.clients[i].Character.Spawned(this.Character.Information.UniqueID) && Systems.clients[i].Character.InGame)
                                            Systems.clients[i].client.Send(buff);
                                    }
                                }
                                else
                                    client.Send(buff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Send(player) Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("player::send::error");
                Systems.Debugger.Write(ex);
            }
        }

        public void Send(List<int> list, byte[] buff)
        {
            try
            {
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            if (Systems.clients[i] != null)
                            {
                                if (Systems.clients[i] != this)
                                {
                                    if (CheckSpawned(list, Systems.clients[i].Character.Information.UniqueID) && Character.InGame)
                                    {
                                        if (Systems.clients[i].Character.Spawned(this.Character.Information.UniqueID) && Systems.clients[i].Character.InGame)
                                            Systems.clients[i].client.Send(buff);
                                    }
                                }
                                else
                                    client.Send(buff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Send(List) Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send List Error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        
        public static bool CheckSpawned(List<int> Spawn, int id)
        {
            bool result = Spawn.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
        
        public static void SendAll(byte[] buff)
        {
            lock (Systems.clients)
            {
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    try
                    {
                        if (Systems.clients[i] != null && Systems.clients[i].Character.InGame)
                            Systems.clients[i].client.Send(buff);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("SendAll Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                        Systems.Debugger.Write(ex);
                    }
                }
            }
        }
    }
}
