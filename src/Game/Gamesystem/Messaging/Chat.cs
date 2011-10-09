///////////////////////////////////////////////////////////////////////////
// DarkEmu: Chat system
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Text;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        public byte[] sendnoticecon(int type, int id, string text, string name)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode for chat
            Writer.Create(Systems.SERVER_CHAT);
            //Write byte chat type 7 = notice
            Writer.Byte(7);
            //Set message to message long from string text given
            string Message = MessageToMessagelong(text);
            //Write textlenght value
            Writer.Textlen(Message);
            //Repeat for message lenght
            for (int g = 0; g < Message.Length; )
            {
                //Writer word value per letter
                Writer.Word(int.Parse(Message.Substring(g, 2), System.Globalization.NumberStyles.HexNumber, null));
                //Set g + 2
                g = g + 2;
            }
            //Return all bytes
            return Writer.GetBytes();
        }
        void Chat()
        {
            /*
             *  We use for each now, to make sure there wont be any issues
             *  Incase the list we send the packet to can have none connected clients.
             *  To prevent bugging of chat we repeat and check each client before sending.
             */
            try
            {
                //Set our list of ranged players
                List<int> Rangedplayers = Character.Spawn;
                //Create new packet reader for reading packet data
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Chat type byte (for our switch case).
                byte chatType = Reader.Byte();
                //Byte chat index
                byte chatIndex = Reader.Byte();
                //Link count byte
                byte linkCount = Reader.Byte();
                //Create switch to switch on type of chat (normal party etc).
                Console.WriteLine("typo: {0}",chatType);
                switch (chatType)
                {
                    //--------------------------------- [Normal chat] ---------------------------------//
                    case 1:
                        //Read written text from packet data
                        string Text = Reader.Text3();
                        //Close packet reader
                        Reader.Close();

                        // .additem 111 12
                        if (Character.Information.GM == 1 && Text[0] == '.')
                        {
                            gmCommands(Text);
                        }

                        //Repeat for each in range player
                        foreach (int member in Rangedplayers)
                        {
                            //Make sure the member is not null
                            if (member != 0)
                            {
                                //Make sure its not sending to our client
                                if (member != Character.Information.UniqueID)
                                {
                                    //Get member detail
                                    Systems memberinfo = GetPlayer(member);
                                    //Send packet to the in range player
                                    memberinfo.client.Send(Packet.ChatPacket(chatType, Character.Information.UniqueID, Text, null));
                                }
                            }
                        }
                        //Send chatindex packet to ourselfs
                        client.Send(Packet.ChatIndexPacket(chatType, chatIndex));
                        break;
                    //--------------------------------- [Normal chat pink] ---------------------------------//
                    case 3:
                        //Check if our character has gm rights or not
                        if (Character.Information.GM == 1)
                        {
                            //Read packet information (text typed).
                            Text = Reader.Text3();
                            //Close packet reader
                            Reader.Close();
                            //Repeat for each player in our range list
                            foreach (int member in Rangedplayers)
                            {
                                //Check if the member is not null
                                if (member != 0)
                                {
                                    //Make sure its not sending to our own client
                                    if (member != Character.Information.UniqueID)
                                    {
                                        //Get member detail
                                        Systems memberinfo = GetPlayer(member);
                                        //Send packet to the member
                                        memberinfo.client.Send(Packet.ChatPacket(chatType, Character.Information.UniqueID, Text, null));
                                    }
                                }
                            }
                            //Send chat index to our client
                            client.Send(Packet.ChatIndexPacket(chatType, chatIndex));
                            break;
                        }
                        //If character tried to use pink chat but isnt a gm
                        else
                        {
                            //We ban the player for hacking attempt.
                            Disconnect("ban");
                        }
                        break;
                    //--------------------------------- [Private chat] ---------------------------------//
                    case 2:
                        //Read from packet data who we are sending the message to
                        string Target = Reader.Text();
                        //Get information from the given player we send to
                        Systems Targetplayer = GetPlayerName(Target);
                        //Make sure the player sending to is not null
                        if (Targetplayer != null)
                        {
                            //Make sure the player is ingame (Not teleporting or such).
                            if (Targetplayer.Character.InGame)
                            {
                                //Read the message from the packet data
                                Text = Reader.Text3();
                                //Close the packet reader
                                Reader.Close();
                                //Send packet to our target
                                Targetplayer.client.Send(Packet.ChatPacket(chatType, 0, Text, this.Character.Information.Name));
                                //Send chatindex packet to ourselfs
                                client.Send(Packet.ChatIndexPacket(chatType, chatIndex));
                            }
                        }
                        break;
                    //--------------------------------- [Party chat] ---------------------------------//
                    case 4:
                        //Make sure the player is in a party
                        if (Character.Network.Party != null)
                        {
                            //Read the text the player has typed from packet data
                            Text = Reader.Text3();
                            //Close packet reader
                            Reader.Close();
                            //Repeat for each member in the party member list
                            foreach (int member in Character.Network.Party.Members)
                            {
                                //Make sure the member isnt null (0)
                                if (member != 0)
                                {
                                    //Get detailed info for the player
                                    Systems memberinfo = GetPlayer(member);
                                    //Send packet information to the member for chat
                                    memberinfo.client.Send(Packet.ChatPacket(chatType, Character.Information.UniqueID, Text, this.Character.Information.Name));
                                }
                            }
                            //Finally send chatindex packet to ourselfs
                            client.Send(Packet.ChatIndexPacket(chatType, chatIndex));
                        }

                        break;
                    //--------------------------------- [Guild chat] ---------------------------------//
                    case 5:
                        //Read message beeing send from packet data
                        Text = Reader.Text3();
                        //Close packet reader
                        Reader.Close();
                        //Repeat for each member in the guild member list
                        foreach (int member in Character.Network.Guild.Members)
                        {
                            //Make sure the member is not null (0)
                            if (member != 0)
                            {
                                //Get guild member detailed information
                                Systems Guildmember = GetPlayerMainid(member);
                                //Again check if the guild member is not null
                                if (Guildmember != null)
                                {
                                    //Send chat packet to the member
                                    Guildmember.client.Send(Packet.ChatPacket(chatType, Character.Information.UniqueID, Text, this.Character.Information.Name));
                                }
                            }
                        }
                        //Send chat index packet to our client
                        client.Send(Packet.ChatIndexPacket(chatType, chatIndex));

                        break;
                    //--------------------------------- [Global chat] ---------------------------------//
                    case 6:
                        //Get sender name from packet data
                        string Sender = Reader.Text();
                        //Get message from packet data
                        Text = Reader.Text3();
                        //Send to every client connected and logged in
                        SendAll(Packet.ChatPacket(chatType, Character.Information.UniqueID, " " + Text, Sender));
                        break;
                    //--------------------------------- [Notice chat] ---------------------------------//
                    case 7:
                        //Make sure the character sending is a gm
                        if (Character.Information.GM == 1)
                        {
                            //Get message from packet data
                            Text = Reader.Text3();
                            //Close packet reader
                            Reader.Close();
                            //Send to everyone ingame (using void sendnoticecon).
                            SendAll(sendnoticecon(chatType, Character.Information.UniqueID, Text, null));
                        }
                        //If the character is not a gm
                        else
                        {
                            //Disconnect and ban the player for hack attempt
                            Disconnect("ban");
                        }
                        break;
                    //--------------------------------- [Stall chat] ---------------------------------//
                    case 9:
                        //Read message from packet data
                        Text = Reader.Text3();
                        //Close packet reader
                        Reader.Close();
                        //Repeat for each member in the stall
                        foreach (int stallmember in Character.Network.Stall.Members)
                        {
                            //Make sure the stall member isnt null 0
                            if (stallmember != 0)
                            {
                                //Get stall member details
                                Systems member = GetPlayer(stallmember);
                                //Make sure the member isnt null
                                if (member != null)
                                {
                                    //Send chat packet to the member
                                    member.client.Send(Packet.ChatPacket(chatType, Character.Network.Stall.ownerID, Text, this.Character.Information.Name));
                                }
                            }
                        }
                        //Send chat index to ourselfs
                        client.Send(Packet.ChatIndexPacket(chatType, chatIndex));
                        break;
                    //--------------------------------- [Academy chat] ---------------------------------//
                    case 10:
                        //Todo academy system then chat.
                        break;
                    //--------------------------------- [Union chat] ---------------------------------//
                    case 11:
                        //Read message from packet data
                        Text = Reader.Text3();
                        //Close packet reader
                        Reader.Close();
                        //If the character has no union
                        if (!Character.Network.Guild.UnionActive) 
                            //Return
                            return;
                        //Else for each member in the union
                        foreach (int member in Character.Network.Guild.UnionMembers)
                        {
                            //Make sure the union member is not null 0
                            if (member != 0)
                            {
                                //Get member detailed information
                                Systems tomember = GetPlayerMainid(member);
                                //Make sure the member is not null
                                if (tomember != null)
                                {
                                    //Make sure the member isnt ourself
                                    if(tomember.Character.Information.CharacterID != Character.Information.CharacterID)
                                    {
                                        //Send packet to the union member
                                        tomember.client.Send(Packet.ChatPacket(chatType, Character.Information.UniqueID, Text, Character.Information.Name));
                                    }
                                }
                            }
                        }
                        //Repeat for each member in the guild
                        foreach (int member in Character.Network.Guild.Members)
                        {
                            //Make sure the member isnt null 0
                            if (member != 0)
                            {
                                //Get member detailed information
                                Systems tomember = GetPlayerMainid(member);
                                //Make sure the member isnt null
                                if (tomember != null)
                                {
                                    //Make sure the member isnt ourself
                                    if (tomember.Character.Information.CharacterID != Character.Information.CharacterID)
                                    {
                                        //Send packet to the member
                                        tomember.Character.Network.Guild.SingleSend = false;
                                    }
                                }
                            }
                        }
                        //Finally send chat index packet to ourself
                        client.Send(Packet.ChatIndexPacket(chatType, chatIndex));
                        break;
                }
            }
            //If an exception error happens
            catch (Exception errorinformation)
            {
                //Write the information to the console
                Console.WriteLine("Chat error {0}", errorinformation);
                //Write the information to the log system
                Systems.Debugger.Write(errorinformation);
            }
        }
        void gmCommands(string text)
        {
            string proc = text.Replace(".", "");
            try
            {
                //proc = makeitem 122 12
                string[] comando = proc.Split(' ');

                // comando[0] = makeitem <- comando
                // comando[1] = 122     <- id_item

                switch (comando[0])
                {
                    case "makeitem":

                        break;
                    default:
                        Console.WriteLine("Command: {0} No exist",comando[0]);
                        break;
                }
            }
            catch (Exception ep)
            {

            }
        }
        //From message to messagelong conversion
        public string MessageToMessagelong(string asciiString)
        {
            //Set default string definition
            string hex = "";
            //Repeat for each character in the message
            foreach (char c in asciiString)
            {
                //Temporary in definition as c
                int tmp = c;
                //Set string formatting
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }
    }
}