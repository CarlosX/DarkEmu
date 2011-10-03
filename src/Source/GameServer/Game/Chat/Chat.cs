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
    class Chat
    {
        private static PacketReader reader;

        public static void OnChat(PacketReader reader_, int Index_)
        {
            reader = reader_;

            byte type = reader.ReadByte();
            byte chatoffset = reader.ReadByte();

            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHAT_ACCEPT);
            writer.AppendByte(1);
            writer.AppendByte(type);
            writer.AppendByte(chatoffset);

            ServerSocket.Send(writer.getWorkspace(), Index_);

            switch (type)
            {
                case 1:
                case 3:
                    OnChatPublic(reader_, Index_);
                    break;
                case 2:
                    OnChatWhisper(reader_, Index_);
                    break;
                case 4:
                    //	OnChatParty();
                    break;
                case 5:
                    //	OnChatGuild();
                    break;
                case 6:
                    //	OnChatGlobal();
                    break;
                case 7:
                    OnChatNotice(reader_);
                    break;
                case 11:
                    //	OnChatUnion();
                    break;
            }
        }

        private static void OnChatPublic(PacketReader reader_, int Index_)
        {
            ushort msglen = reader_.ReadWord();
            byte[] bmsg = reader_.ReadByteArray(msglen * 2);

            if (Player.Flags[Index_].GM == 1)
            {
                string msg = Encoding.Unicode.GetString(bmsg);
                if (msg.ToCharArray()[0] == '.' && msg.Contains("level"))
                {
                    string[] tmpString = msg.Split(' ');
                    Stats.GetXP(Index_, Convert.ToUInt64(tmpString[1]), Convert.ToUInt64(tmpString[2]));
                }
            }

            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHAT);
            writer.AppendByte(1);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendWord(msglen);
            writer.AppendByteArray(bmsg, bmsg.Length);
            ServerSocket.SendPacketInRange(writer.getWorkspace(), Index_);
        }

        private static void OnChatWhisper(PacketReader reader_, int Index_)
        {
            ushort charlen = reader_.ReadWord();
            string name = reader.ReadString(false, charlen);

            if (name.ToCharArray(0, 1)[0] == '[')
                name = name.Substring(3);

            ushort msglen = reader_.ReadWord();
            byte[] bmsg = reader_.ReadByteArray(msglen * 2);


            if (DatabaseCore.Character.GetIndexByName(name) != -1)
            {
                PacketWriter writer = new PacketWriter();
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHAT);
                writer.AppendByte(2);
                writer.AppendWord((ushort)Player.General[Index_].CharacterName.Length);
                writer.AppendString(false, Player.General[Index_].CharacterName);
                writer.AppendWord((ushort)(msglen / 2));
                writer.AppendByteArray(bmsg, bmsg.Length);
                for (int i = 0; i <= Player.PlayersOnline; i++)
                {
                    if (Player.General[i].CharacterName == name)
                    {
                        ServerSocket.Send(writer.getWorkspace(), i);
                        break;
                    }
                }
            }
        }

        private static void OnChatNotice(PacketReader reader_)
        {
            ushort msglen = reader_.ReadWord();

            byte[] bmsg = reader_.ReadByteArray(msglen * 2);

            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHAT);
            writer.AppendByte(7);
            writer.AppendWord(msglen);
            writer.AppendByteArray(bmsg, bmsg.Length);
            ServerSocket.SendToAllIngame(writer.getWorkspace());
        }

        public static void SendExternNotice(string msg)
        {
            byte[] bmsg = Encoding.Unicode.GetBytes(msg);

            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHAT);
            writer.AppendByte(7);
            writer.AppendWord((ushort)(bmsg.Length / 2));
            writer.AppendByteArray(bmsg, bmsg.Length);
            ServerSocket.SendToAllIngame(writer.getWorkspace());
        }
    }
}
