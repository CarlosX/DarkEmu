///////////////////////////////////////////////////////////////////////////
// DarkEmu: Guild messages
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        void GuildMessage()
        {
            //Wrap our function inside a catcher
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Read short int16 for title lenght
                short TitleL = Reader.Int16();
                //Read string for title
                string Title = Reader.String(TitleL);
                //Read short Message lenght
                short MessageL = Reader.Int16();
                //Read message
                string Message = Reader.String(MessageL);
                //Close reader
                Reader.Close();

                //Update database guild message title and message
                MsSQL.InsertData("UPDATE guild SET guild_news_t='" + Title + "',guild_news_m='" + Message + "' WHERE guild_name='" + Character.Network.Guild.Name + "'");

                //Set new message info to current member for sending packet update.
                Character.Network.Guild.NewsTitle = Title;
                Character.Network.Guild.NewsMessage = Message;
                //Repeat for each member in our guild
                foreach (int member in Character.Network.Guild.Members)
                {
                    //Make sure the member is there
                    if (member != 0)
                    {
                        //Get detailed information from member main id
                        Systems characterinformation = GetPlayerid(member);
                        //Set the current member news title and message information
                        characterinformation.Character.Network.Guild.NewsMessage = Message;
                        characterinformation.Character.Network.Guild.NewsTitle = Title;
                        //Send packet to the member to update guild message.
                        characterinformation.client.Send(Packet.GuildUpdate(characterinformation.Character, 11, 0, 0, 0));
                    }
                }
            }
            //Catch any bad exception error
            catch (Exception ex)
            {
                //Write error to the console
                Console.WriteLine("Guild Message Error: {0}", ex);
                //Write information to the debug log
                Systems.Debugger.Write(ex);
            }
        }
    }
}
