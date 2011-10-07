///////////////////////////////////////////////////////////////////////////
// DarkEmu: Get information
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
        public Client GetClient
        {
            get
            {
                return this.client;
            }
        }
        public static Systems GetGuildPlayer(int id)
        {
            lock (Systems.clients)
            {
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    try
                    {
                        if (Systems.clients[i] != null && Systems.clients[i].Character.Network.Guild.Guildid == id)
                            return Systems.clients[i];
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get from guild Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                        Systems.Debugger.Write(ex);
                    }
                }
            }
            return null;
        }

        public static Systems GetPlayer(int id)
        {
            lock (Systems.clients)
            {
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    try
                    {
                        if (Systems.clients[i] != null && Systems.clients[i].Character.Information.UniqueID == id)
                            return Systems.clients[i];
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("GetPlayer Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                        Systems.Debugger.Write(ex);
                    }
                }
            }
            return null;
        }

        public static Systems GetPlayerMainid(int id)
        {
            lock (Systems.clients)
            {
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    try
                    {
                        if (Systems.clients[i] != null && Systems.clients[i].Character.Information.CharacterID == id)
                            return Systems.clients[i];
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("GetPlayerMainId Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                        Systems.Debugger.Write(ex);
                    }
                }
            }
            return null;
        }

        public static party GetPartyInfo(int id)
        {
            for (int i = 0; i < Systems.Party.Count; i++)
            {
                if (Systems.Party[i].ptid == id)
                {
                    return Systems.Party[i];
                }
            }
            return null;
        }

        public static Systems GetPlayerName(string name)
        {
            lock (Systems.clients)
            {
                for (int i = 0; i < Systems.clients.Count; i++)
                {
                    try
                    {
                        if (Systems.clients[i] != null && Systems.clients[i].Character.Information.Name == name)
                            return Systems.clients[i];
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("GetPlayerName Error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                        Systems.Debugger.Write(ex);
                    }
                }
            }
            return null;
        }

        public static pet_obj GetPet(int id)
        {
            for (int i = 0; i < Systems.HelperObject.Count; i++)
            {
                if (Systems.HelperObject[i] != null && Systems.HelperObject[i].UniqueID == id)
                    return Systems.HelperObject[i];
            }
            return null;
        }

        public static int GetOnlineClientCount
        {
            get
            {
                return Systems.clients.Count;
            }
        }

        public static void CheckDirectories()
        {
            string cur_path = Environment.CurrentDirectory + @"\player\info\";
            System.IO.Directory.CreateDirectory(cur_path + "quickbar");
            System.IO.Directory.CreateDirectory(cur_path + "autopot");
            System.IO.Directory.CreateDirectory(cur_path + "debug");
        }

    }

}
