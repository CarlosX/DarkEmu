/*    <DarkEmu GameServer>
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

namespace DarkEmu_GameServer
{
    class GameSocket
    {
        private static PacketReader reader;
        public static bool debug = false;
        public static DateTime ServerStartedTime;

        unsafe public static void ProcessData(byte[] buffer, int Index)
        {
            reader = new PacketReader(buffer, buffer.Length);
            reader.ModifyIndex(6);

            TPacket* tmpPacket = Silkroad.ToTPacket(buffer);
            if (debug)
                Console.WriteLine("[ProcessData][{0:X}][{1} bytes][Index {2}]\n{3}\n", tmpPacket->opcode, tmpPacket->size, Index, BitConverter.ToString(buffer, 6, tmpPacket->size).Replace('-', ' '));

            //Console.WriteLine("{0}", tmpPacket->opcode);

            switch (tmpPacket->opcode)
            {
                case CLIENT_OPCODES.GAME_CLIENT_KEEP_ALIVE:
                case CLIENT_OPCODES.GAME_CLIENT_ACCEPT_HANDSHAKE:
                    /*if(debug)
                        Debugx.DumpBuffer(buffer, 1, tmpPacket->opcode, tmpPacket->size);*/
                    //hansh(Index);
                    Auth.SendServerInfo(Index);  //force cliente acept xD
                    break;
                case CLIENT_OPCODES.GAME_CLIENT_INFO:
                    Auth.SendServerInfo(Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_PATCH_REQUEST:
                    Auth.SendPatchInfo(Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_AUTH:
                    Auth.OnAuth(reader,Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_CHARACTER:
                    Character.OnCharacter(reader, Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_INGAME_REQUEST:
                    Character.OnIngameRequest(reader,Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_INGAME_NOTIFY:
                    Players.OnIngameNotify(reader,Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_CHARACTER_STATE:
                    Character.OnState(reader, Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_MOVEMENT:
                    Movement.OnMovement(tmpPacket->data, Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_CLOSE:
                    OnGameQuit(reader,Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_EMOTION:
                    PlayerAction.OnEmotion(reader, Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_ITEM_MOVE:
                    Items.MoveItem(tmpPacket->data,Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_CHAT:
                    Chat.OnChat(reader,Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_TARGET:
                    Players.OnTarget(reader, Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_MASTERYUPDATE :
                    Mastery.OnMasteryUpdate(reader, Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_SKILLUPDATE:
                    Mastery.OnSkillUpdate(reader, Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_STR_UPDATE:
                    Stats.STRUpdate(Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_INT_UPDATE:
                    Stats.INTUpdate(Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_PLAYER_ACTION:
                    PlayerAction.Action(reader, Index);   
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_RESPAWN:
                    Character.ReSpawnMe(Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_GM:
                    GameMaster.OnGM(reader,Index);
                    break;

                case CLIENT_OPCODES.GAME_CLIENT_ITEM_USE:
                    Items.OnUseItem(reader, Index);
                    break;

                default:
                    Console.WriteLine("Default Opcode:{0}", tmpPacket->opcode);
                    break;
            }

        }
        private static void hansh(int Index)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(0x5000);
            writer.AppendByte(1);//NO HANDSHAKE
            ServerSocket.Send(writer.getWorkspace(), Index); 
        }
        private static void OnGameQuit(PacketReader reader_,int Index_) 
        {
            byte type = reader_.ReadByte();
        	byte countdown = 5;

            PacketWriter writer = new PacketWriter();
        	writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_COUNTDOWN);
            writer.AppendByte(1);
            writer.AppendByte(countdown);
            writer.AppendByte(type);
            ServerSocket.Send(writer.getWorkspace(), Index_);

        	System.Threading.Thread.Sleep(countdown * 1000);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_QUIT_GAME);
            ServerSocket.Send(writer.getWorkspace(), Index_);

        }

    }
}
