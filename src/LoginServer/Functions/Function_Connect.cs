///////////////////////////////////////////////////////////////////////////
// Copyright (C) <2011>  <DarkEmu>
// HOMEPAGE: WWW.XFSGAMES.COM.AR
///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using DarkEmu_GameServer.Network;
using System.Linq;
using System.Text;

namespace LoginServer
{
    public partial class Systems
    {
        public void Connect()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);

                if (Reader.Byte() == 18)
                {
                    string ID = Reader.Text();
                    string PW = Reader.Text();
                    byte ukn = Reader.Byte(); // 0xff
                    UInt16 ServerID = Reader.UInt16();
                    int lResult = 99;

                    //Console.WriteLine("Id:{0} Pass:{1} ServerID:{2}",ID,PW,ServerID);

                    SRX_Serverinfo SSI = GSList[ServerID];
                    if (SSI != null)
                    {
                        UInt16 myKey = 0;
                        string sReason = "";
                        lock (Program.IPCResultList)
                        {
                            myKey = Program.IPCNewId++;
                        }
                        byte[] rqp = Program.IPCServer.PacketRequestLogin(Program.IPCPort, ID, PW, myKey);
                        Servers.IPCenCode(ref rqp, SSI.code);
                        lock (Program.IPCResultList)
                        {
                            Program.IPCResultList.Add(myKey, new IPCItem());
                            Program.IPCResultList[myKey].resultCode = 0x8000;
                        }
                        Program.IPCServer.Send(SSI.ip, SSI.ipcport, rqp);
                        DateTime tOut = DateTime.Now.AddSeconds(30);
                        while ((tOut >= DateTime.Now) && (Program.IPCResultList[myKey].resultCode == 0x8000) && (client.clientSocket.Connected))
                        {
                            System.Threading.Thread.Sleep(10);
                        }
                        lResult = Program.IPCResultList[myKey].resultCode;
                        sReason = Program.IPCResultList[myKey].banReason;
                        lock (Program.IPCResultList)
                        {
                            Program.IPCResultList[myKey] = null;
                            Program.IPCResultList.Remove(myKey);
                        }
                        rqp = null;
                        //Console.WriteLine("Resultado de login: {0}",lResult);
                        switch (lResult)
                        {
                            case 0:
                                client.Send(ConnectSucces(SSI.ip, SSI.port, 1));
                                return;
                            case 1:
                                if (WrongPassword < 3)
                                {
                                    client.Send(WrongInformation());
                                    WrongPassword++;
                                    return;
                                }
                                else
                                {
                                    client.Disconnect(PacketInformation.Client);
                                    return;
                                }
                            case 2:
                                client.Send(ServerIsFull());
                                client.Disconnect(PacketInformation.Client);
                                return;
                            case 3:
                                client.Send(AllreadyConnected());
                                client.Disconnect(PacketInformation.Client);
                                return;
                            case 4:
                                client.Send(BannedUser(sReason));
                                client.Disconnect(PacketInformation.Client);
                                return;
                            default:
                                if (lResult == 0x8000)
                                {
                                    Console.WriteLine("[IPC] Timeout");
                                }
                                else
                                {
                                    Console.WriteLine("[IPC] Result unknown {0}", lResult);
                                }
                                return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Fuck No fun.. SSI");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Error: " + ex);
            }
        }

        bool CheckCrowed(ushort serverid)
        {
            SRX_Serverinfo SI = GSList[serverid];
            if (SI != null)
            {
                if (SI.usedSlots >= SI.maxSlots)
                {
                    return true;
                }
            }
            return false;
        }
    }
}