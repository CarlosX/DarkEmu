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
using System.Collections.Generic;
using System.Text;

namespace DarkEmu_LoginServer
{
    class LoginSocket
    {
        public static bool debug = false;

        unsafe public static void ProcessData(byte[] buffer, int ClientIndex)
        {
            TPacket* tmpPacket = Silkroad.ToTPacket(buffer);
            if (debug)            
                Console.WriteLine("[ProcessData][{0:X}][{1} bytes][Index {2}]\n{3}\n", tmpPacket->opcode, tmpPacket->size,ClientIndex,BitConverter.ToString(buffer,6,tmpPacket->size).Replace('-',' '));
           
            switch (tmpPacket->opcode)
            {
                case CLIENT_OPCODES.LOGIN_CLIENT_KEEP_ALIVE:
                case CLIENT_OPCODES.LOGIN_CLIENT_ACCEPT_HANDSHAKE:
                    break;
                case CLIENT_OPCODES.LOGIN_CLIENT_INFO:
                    SendServerInfo(ClientIndex);
                    break;
                case CLIENT_OPCODES.LOGIN_CLIENT_PATCH_REQUEST:
                    SendPatchInfo(ClientIndex);
                    break;
                case CLIENT_OPCODES.LOGIN_CLIENT_LAUNCHER:
                    SendLauncherInfo(ClientIndex);
                    break;
                case CLIENT_OPCODES.LOGIN_CLIENT_SERVERLIST_REQUEST:
                    SendServerList(ClientIndex);
                    break;
                case CLIENT_OPCODES.LOGIN_CLIENT_AUTH:
                    /*if (debug)
                        Debugx.DumpBuffer(buffer, 1, tmpPacket->opcode, tmpPacket->size);*/
                    SendLogin(buffer, ClientIndex);
                    break;
                case CLIENT_OPCODES.LOGIN_CLIENT_LAUNCHER_UNK1:
                    SendLauncherUnk1(ClientIndex);
                    break;
                case CLIENT_OPCODES.LOGIN_CLIENT_AUTH_UNK1:
                    SendServerUnk1(ClientIndex);
                    break;
                default:
                    //Debugx.DumpBuffer(buffer, 1, tmpPacket->opcode, tmpPacket->size);
                    Console.WriteLine("default Opcode: {0}", tmpPacket->opcode);
                    break;
            }

        }

        private static void SendServerInfo(int ClientIndex)
        {
            string name = "GatewayServer";
            ushort namelen = (ushort)name.Length;

            PacketWriter writer = new PacketWriter();

            writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_INFO);
            writer.AppendWord(namelen);
            writer.AppendString(false, name);
            writer.AppendByte(0x00);
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);
        }

        private static void SendPatchInfo(int ClientIndex)
        {
            PacketWriter writer = new PacketWriter();

            writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_PATCH_INFO);
            writer.AppendDword(0x05000101);
            writer.AppendByte(0x20);
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);

            writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_PATCH_INFO);
            writer.AppendDword(0x01000100);
            writer.AppendDword(0x00050628);
            writer.AppendWord(0x00);
            writer.AppendByte(0x02);
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);

            writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_PATCH_INFO);
            writer.AppendDword(0x05000101);
            writer.AppendByte(0x60);
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);

            writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_PATCH_INFO);
            writer.AppendDword(0x02000300);
            writer.AppendWord(0x0200);
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);

            writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_PATCH_INFO);
            writer.AppendDword(0x00000101);
            writer.AppendByte(0xA1);
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);

            writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_PATCH_INFO);
            writer.AppendWord(0x0100);
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);
        }

        private static void SendLauncherInfo(int ClientIndex)
        {
            PacketWriter writer = new PacketWriter();

            writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_PATCH_INFO);
            writer.AppendDword(0x04000101);
            writer.AppendByte(0xA1);
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);

            writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_LAUNCHER);
            writer.AppendByte(0x00);

            writer.AppendByte(DatabaseCore.News.NumberOfNews);
            for (int i = 0; i < DatabaseCore.News.NumberOfNews; i++)
            {

                writer.AppendWord((ushort)DatabaseCore.News.Head[i].Length);
                writer.AppendString(false,DatabaseCore.News.Head[i]);

                writer.AppendWord((ushort)DatabaseCore.News.Text[i].Length);
                writer.AppendString(false, DatabaseCore.News.Text[i]);

                writer.AppendWord(0x07D8);

                writer.AppendByte(DatabaseCore.News.Month[i]);
                writer.AppendByte(0x00);

                writer.AppendByte(DatabaseCore.News.Day[i]);
                writer.AppendByte(0x00);

                writer.AppendDword(0x0025000B);
                writer.AppendDword(0xAFC00012);
                writer.AppendWord(0x35D2);

            }
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);
        }
        private static void SendLauncherUnk1(int ClientIndex)
        {
            PacketWriter2 writer = new PacketWriter2(SERVER_OPCODES.LOGIN_SERVER_LAUNCHER_UNK1, 1026, true);
            //01 01 04 02
            writer.AddByte(0x01);
            writer.AddByte(0x01);
            writer.AddByte(0x04);
            writer.AddByte(0x02);
            ServerSocket.Send(writer.GetBytes(), ClientIndex);
        }
        private static void SendServerUnk1(int ClientIndex)
        {
            /*
             43 00 L
             07 A1 Opcode
             00 00 Secu
             
             03 00 count
             
             11 00 
             67 73 70 6B 72 31 2E 6A 6F 79 6D 61 78 2E 63 6F 6D BD string_ip
             32 01 port
             */
            PacketWriter2 writer = new PacketWriter2(SERVER_OPCODES.LOGIN_SERVER_AUTH_UNK1);
            writer.AddByte(0x01);
            writer.AddByte(0x00);
            string str_ip = "127.0.0.1";
            writer.AddByte(0x09);
            writer.AddByte(0x00);
            writer.AddString(str_ip, str_ip.Length);
            writer.AddByte(0x3D);
            writer.AddByte(0xA4);
            ServerSocket.Send(writer.GetBytes(), ClientIndex);

        }
        private static void SendServerList(int ClientIndex)
        {
            PacketWriter writer = new PacketWriter();

            string name = "SRO_Global_TestBed";
            ushort namelen = (ushort)name.Length;

            writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_LIST);

            writer.AppendByte(0x01);
            writer.AppendByte(0x15);

            writer.AppendWord(namelen);
            writer.AppendString(false, name);
            writer.AppendByte(0x00);

            for (int i = 1; i < DatabaseCore.Server.NumberOfServer; i++)
            {
                writer.AppendByte(0x01);
                writer.AppendWord(DatabaseCore.Server.ServerId[i]);
                writer.AppendWord((ushort)(DatabaseCore.Server.ServerName[i].Length+1));
                writer.AppendByte(0x31); // flag xD usa
                writer.AppendString(false, DatabaseCore.Server.ServerName[i]);
                writer.AppendWord(DatabaseCore.Server.CurUser[i]);
                writer.AppendWord(DatabaseCore.Server.MaxUser[i]);
                writer.AppendByte(DatabaseCore.Server.ServerState[i]);
            }
            writer.AppendByte(0x00);
            ServerSocket.Send(writer.getWorkspace(), ClientIndex);
        }        

        private static void SendLogin(byte[] buffer, int ClientIndex)
        {
            /*
            16 00 L
            02 61 Opcode
            00 00 Secu
            
            12 Leng_total
            07 00 Leng_String
            63 61 72 6C 6F 73 78 = carlosx
            07 00 Leng_String
            63 61 72 6C 6F 73 78 = carlosx
            
            FF unk
            03 ServerID
            */
            PacketWriter writer = new PacketWriter();
            PacketReader reader = new PacketReader(buffer, buffer.Length);
            reader.ModifyIndex(7);

            ushort userlen = reader.ReadWord();
            string user = reader.ReadString(false, userlen);

            ushort passlen = reader.ReadWord();
            string pass = reader.ReadString(false, passlen);

            reader.ReadByte();
            ushort serverid = reader.ReadByte();

            Console.WriteLine("User:{0} - Password:{1} - ServerId:{2}",user,pass,serverid);

            int UserIndex = DatabaseCore.User.GetIndexByName(user);

            if (DatabaseCore.User.Banned[UserIndex])
            {
                ServerSocket.DisconnectSocket(ClientIndex);
                return;
            }
            else if (DatabaseCore.User.Online[UserIndex])
            {
                writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_AUTH_INFO);
                writer.AppendWord(0x302);
                ServerSocket.Send(writer.getWorkspace(), ClientIndex);
                ServerSocket.DisconnectSocket(ClientIndex);
            }
            else if (DatabaseCore.User.User[UserIndex] != user || DatabaseCore.User.Password[UserIndex] != pass)
            {
                writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_AUTH_INFO);

                DatabaseCore.User.FailedLogins[UserIndex]++;
                DatabaseCore.WriteQuery("UPDATE user SET failed_logins = '{0}' WHERE id = '{1}'", DatabaseCore.User.FailedLogins[UserIndex], DatabaseCore.User.User[UserIndex]); 

                writer.AppendWord(0x0102);
                writer.AppendByte(DatabaseCore.User.FailedLogins[UserIndex]);
                writer.AppendDword(0x03000000);
                writer.AppendWord(0x00);
                writer.AppendByte(0x00);

                ServerSocket.Send(writer.getWorkspace(), ClientIndex);

                if (DatabaseCore.User.FailedLogins[UserIndex] == 3)
                {
                    DatabaseCore.User.FailedLogins[UserIndex] = 0;
                    DatabaseCore.WriteQuery("UPDATE user SET failed_logins = '{0}' WHERE id = '{1}'", DatabaseCore.User.FailedLogins[UserIndex], DatabaseCore.User.User[UserIndex]); 

                    ServerSocket.DisconnectSocket(ClientIndex);
                }
            }
            else
            {
                int ServerIndex = DatabaseCore.Server.GetIndexById(serverid);

                if (DatabaseCore.Server.MaxUser[ServerIndex] <= DatabaseCore.Server.CurUser[ServerIndex])
                {
                    writer.SetOpcode(0xA103);
                    writer.AppendWord(0x402);
                    ServerSocket.Send(writer.getWorkspace(), ClientIndex);
                    return;
                }
                else
                {
                    writer.SetOpcode(SERVER_OPCODES.LOGIN_SERVER_AUTH_INFO);
                    writer.AppendByte(0x01);
                    writer.AppendDword(0x1234);

                    ushort leng_ip = (ushort)DatabaseCore.Server.ServerIp[ServerIndex].Length;
                    string str_ip = DatabaseCore.Server.ServerIp[ServerIndex];
                    ushort port = DatabaseCore.Server.ServerPort[ServerIndex];

                    writer.AppendWord(leng_ip);
                    writer.AppendString(false, str_ip);
                    writer.AppendWord(port);
                    ServerSocket.Send(writer.getWorkspace(), ClientIndex);
                    Console.WriteLine("Lengip:{0} - Ip:{1} - Port:{2}",leng_ip,str_ip,port);

                    //DatabaseCore.Server.CurUser[ServerIndex]++;
                    //DatabaseCore.WriteQuery("UPDATE server SET users_current = '{0}' WHERE name = '{1}'", DatabaseCore.Server.CurUser[ServerIndex], DatabaseCore.Server.ServerName[ServerIndex]);
                }
            }
        }
    }
}
