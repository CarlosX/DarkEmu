///////////////////////////////////////////////////////////////////////////
// SRX Revo: Private message system
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {

        void PrivateMessage()
        {
            //Wrap our code inside a try / catch to catch bad errors
            try
            {
                //Create a new mssql query to get information from messages table
                Systems.MsSQL ms = new Systems.MsSQL("SELECT * FROM message WHERE receiver='" + Character.Information.Name + "' ORDER BY time DESC");
                //Set integer definition for count rows
                int count = ms.Count();
                //If we have more then zero messages
                if (count > 0)
                {
                    //Tmp test for desc / asc
                    int Tempdate = 0;
                    //Create a new packet writer
                    PacketWriter Writer = new PacketWriter();
                    //Add the opcode for messages
                    Writer.Create(Systems.SERVER_PM_MESSAGE);
                    //Write static 1 byte
                    Writer.Byte(1);
                    //Write total messages count
                    Writer.Byte(Convert.ToByte(count));
                    //Create new sql data reader for reading colums.
                    using (SqlDataReader reader = ms.Read())
                    {
                        //While our reader is reading information
                        while (reader.Read())
                        {
                            //Set definition for pm sender
                            string MessageFrom = reader.GetString(1);
                            //Set definition for pm receiver
                            string MessageTo = reader.GetString(2);
                            //Set definition for message
                            string MessageContent = reader.GetString(3);
                            //Set byte definition for status (Read unread) 0 = unread, 1 = read
                            byte pmstatus = reader.GetByte(4);
                            //Get date time when message have been send
                            DateTime MessageDate = Convert.ToDateTime(reader.GetDateTime(5));
                            //Write text message sender
                            Writer.Text(MessageFrom);
                            //Write time
                            //DateTime Time = reader.GetDateTime(5);
                            //Time = Time..ToString();
                            Writer.DWord(0);
                            //Write date
                            Writer.DWord(Tempdate += 1);
                            //Write byte status 0 = unread , 1 = read
                            Writer.Byte(pmstatus);
                        }
                        //Send bytes to client
                        client.Send(Writer.GetBytes());
                    }
                }
                //Close mssql query
                ms.Close();
            }
            //If any error accures
            catch (Exception ex)
            {
                Console.WriteLine("Error reading private messages: {0}",ex);
                //Write the information to the debug file.
                Systems.Debugger.Write(ex);
            }
        }

        void PrivateMessageOpen()
        {
            try
            {
                //Create new packet reader for reading packet information
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Create new packetwriter for writing our packet
                PacketWriter Writer = new PacketWriter();
                //Read selected message id from packet
                byte SelectedMessageID = Reader.Byte();
                //Close packet reader
                Reader.Close();
                //Create new mssql query for getting the messages
                Systems.MsSQL ms = new Systems.MsSQL("SELECT * FROM message WHERE receiver='" + Character.Information.Name + "' ORDER BY time DESC");
                //Create new integer for calculating what message must be selected.
                int Messageid = -1;
                //Create new sql data reader for reading column information
                using (SqlDataReader reader = ms.Read())
                {
                    //While our sql data reader is reading
                    while (reader.Read())
                    {
                        //Read message content
                        string MessageContent = reader.GetString(3);
                        //Increase our message id integer
                        Messageid += 1;
                        //Check if the messageid equals to our selected message
                        if (Messageid == SelectedMessageID)
                        {
                            //Add opcode to packet
                            Writer.Create(Systems.SERVER_PM_OPEN);
                            //Write static byte
                            Writer.Byte(1);
                            //Write byte selected message id
                            Writer.Byte(SelectedMessageID);
                            //Write text message content
                            Writer.Text(MessageContent);
                            //Send packet to client
                            client.Send(Writer.GetBytes());
                            //Update message state to read in database
                            MsSQL.UpdateData("UPDATE message SET status='1' WHERE receiver='" + Character.Information.Name + "' AND message ='" + MessageContent + "'");
                        }                        
                    }
                }
                //Close mssql
                ms.Close();
            }
            //If an error accures we catch it here
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Error PrivateMessages.cs : {0}" + ex);
                //Write information to the debug logger
                Systems.Debugger.Write(ex);
            }
        }

        void PrivateMessageSend()
        {
            try
            {
                //Create new packet reader for reading packet data
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Read lenght of charactername we send the message to
                short ToCharacterLen = Reader.Int16();
                //Read the name of the character we send the message to
                string ToCharacter = Reader.String(ToCharacterLen);
                //Read lenght of message characters
                short MessageLen = Reader.Int16();
                //Read message
                string Message = Reader.String(MessageLen);
                //Close packet reader
                Reader.Close();
                //Create new mssql query for sending and checking
                MsSQL ms = new MsSQL("SELECT * FROM character WHERE name='" + ToCharacter + "'");
                //Check if the player exists
                int PlayerExists = ms.Count();
                //If the player exists
                if (PlayerExists > 0)
                {
                    //First get details of the player we send the message to.
                    Systems sys = GetPlayerName(ToCharacter);
                    //Make sure we dont get a null error
                    if (sys.Character != null)
                    {
                        //Check how many messages the player has if inbox is full or not
                        int TargetMessageCount = MsSQL.GetRowsCount("SELECT * FROM message WHERE receiver='" + sys.Character.Information.CharacterID + "'");
                        //If less then 50 we continue
                        if (TargetMessageCount < 50)
                        {
                            //Set temp int to character data for new message order
                            sys.Character.Information.MessageCount = TargetMessageCount;
                            //Insert new message into the database
                            MsSQL.InsertData("INSERT INTO message (sender, receiver, message, status, time) VALUES ('" + Character.Information.Name + "','" + ToCharacter + "','" + Message + "','0','" + DateTime.Now + "')");
                            //Send packet message has been send to our client
                            client.Send(PrivateMessageRespond(2));
                            //Send packet to receiver information new message has arrived
                            sys.Send(Packet.FriendData(sys.Character.Information.UniqueID, 5, ToCharacter, Character, false));
                        }
                        //If inbox is full
                        else
                        {
                            //Send message to sender and receiver inbox full
                            client.Send(PrivateMessageRespond(3));
                            sys.client.Send(PrivateMessageRespond(3));
                        }
                    }
                }
                //If player doesn't exist
                else
                {
                    //Send packet message failed to send to our client.
                    client.Send(PrivateMessageRespond(1));
                }
            }
            //Catch any bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Error sending messages : {0}" + ex);
                //Write info to the debug logger.
                Systems.Debugger.Write(ex);
            }
        }
        //Packet for response of message sending
        public static byte[] PrivateMessageRespond(byte type)
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add the opcode to the packet
            Writer.Create(Systems.SERVER_PM_SEND);
            //Switch on type sended by our function
            switch (type)
            {
                case 1:
                    //Failed
                    Writer.Byte(0x02);
                    Writer.Byte(0x0D);
                    Writer.Byte(0x64);
                    break;
                case 2:
                    //Success
                    Writer.Byte(0x01);
                    break;
                case 3:
                    //Inbox full
                    Writer.Byte(2);
                    Writer.Word(0x6414);
                    break;
            }
            //Return all bytes to the void
            return Writer.GetBytes();
        }

        void PrivateMessageDelete()
        {
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Read message selected id
                byte SelectedMessageID = Reader.Byte();
                //Close packet reader
                Reader.Close();
                //Create new integer to get message id
                int Messageid = -1;
                //Create new mssql query
                MsSQL ms = new MsSQL("SELECT * FROM message WHERE receiver='" + Character.Information.Name + "' ORDER BY time DESC");
                //Create new sql data reader for reading column information
                using (SqlDataReader reader = ms.Read())
                {
                    //While our sql data reader is reading
                    while (reader.Read())
                    {
                        //Read message content
                        string MessageContent = reader.GetString(3);
                        //Increase our message id integer
                        Messageid += 1;
                        //Check if the messageid equals to our selected message
                        if (Messageid == SelectedMessageID)
                        {
                            //Create new packet writer
                            PacketWriter Writer = new PacketWriter();
                            //Add opcode for deleting message
                            Writer.Create(Systems.SERVER_PM_DELETE);
                            //Write static byte 1
                            Writer.Byte(0x01);
                            //Write byte selected message id
                            Writer.Byte(SelectedMessageID);
                            //Send packet to client
                            client.Send(Writer.GetBytes());
                            //Update database
                            MsSQL.DeleteData("DELETE FROM message WHERE message='" + reader.GetString(3) + "' AND receiver='"+ Character.Information.CharacterID +"'");
                        }
                    }
                }
                //Close mssql
                ms.Close();
            }
            //If an error happens
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Delete private message error: " + ex);
                //Write info to debugger
                Systems.Debugger.Write(ex);
            }
        }
    }
}