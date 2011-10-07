using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using DarkEmu_GameServer.Network;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {

        /////////////////////////////////////////////////////////////////////////////////
        // Update Serverinfo
        /////////////////////////////////////////////////////////////////////////////////
        public static void UpdateServerInfo(byte bStatus)
        {
            foreach (KeyValuePair<int, Systems.SRX_Serverinfo> LS in Systems.LSList)
            {
                try
                {
                    byte[] tBuf = Systems.IPC.PacketResponseServerInfo(Servers.IPCPort, bStatus, (UInt16)Systems.maxSlots, (UInt16)Systems.clients.Count, (UInt16)DarkEmu_GameServer.Global.Versions.clientVersion);
                    Servers.IPCenCode(ref tBuf, LS.Value.code);
                    Systems.IPC.Send(LS.Value.ip, LS.Value.ipcport, tBuf);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[IPC] Error informing LoginServer {0}:{1}> {2}", LS.Value.ip, LS.Value.ipcport, ex);
                }
            }
        }

        public static void UpdateServerInfo()
        {
            UpdateServerInfo(1);
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Login user
        /////////////////////////////////////////////////////////////////////////////////
        public static int LoginUser(string aID, ref string aPass, ref DarkEmu_GameServer.player aPlayer, bool localConnect)
        {
            //Console.WriteLine("Login User: {0} - {1}",aID,aPass);

            MsSQL ms = new MsSQL("SELECT * FROM users WHERE password='" + aPass + "'");
            using (System.Data.SqlClient.SqlDataReader reader = ms.Read())
            {

                if (Systems.clients.Count>=Systems.maxSlots)  
                {
                    ms.Close();
                    return 2; // crowded
                }

                while (reader.Read())
                {
                    if (reader.GetString(1).ToLower() == aID.ToLower()) // id
                    {
                        if (reader.GetByte(3) == 1) // online
                        {
                            ms.Close();
                            return 3; // already online
                        }

                        if (reader.GetInt32(5) == 1) // banned
                        {
                            aPass = reader.GetString(4);
                            ms.Close();
                            return 4; // banned
                        }

                        if (aPlayer == null && localConnect) MsSQL.UpdateData("UPDATE users SET online=1 WHERE userid='" + reader.GetInt32(0) + "'");
                        aPlayer = new player();
                        aPlayer.AccountName = aID;
                        aPlayer.Password = aPass; // Nukei: ?? whats the reason for saving password in memory ?
                        aPlayer.ID = reader.GetInt32(0);
                        aPlayer.pGold = reader.GetInt64(7);
                        aPlayer.Silk = reader.GetInt32(6);
                        aPlayer.SilkPrem = reader.GetInt32(9);
                        aPlayer.wSlots = reader.GetByte(11);
                        ms.Close();
                        //Console.WriteLine("Login..!!");
                        return 0;
                    }
                }
            }
            ms.Close();
            return 1; // not found
        }
    }
}
