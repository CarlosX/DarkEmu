///////////////////////////////////////////////////////////////////////////
// DarkEmu: Guild player title
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
        void GuildTitle()
        {
            //Wrap our function inside a catcher
            try
            {
                //Extra hack check
                if (Character.Network.Guild.Level < 4) 
                    return;
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Read integer guild member selected
                int SelectedGuildMember = Reader.Int32();
                //Read short lenght of title for guild member
                short GuildMemberTitleLEN = Reader.Int16();
                //Read string guild member title
                string GuildMemberTitle = Reader.String(GuildMemberTitleLEN);
                //Close packet reader
                Reader.Close();
                //Get selected guild member information
                Systems playerinfo = GetPlayerMainid(SelectedGuildMember);
                //Make sure the character is still there
                if (playerinfo.Character != null)
                {
                    //Update database set new title
                    MsSQL.InsertData("UPDATE guild_members SET guild_grant='" + GuildMemberTitle + "' WHERE guild_member_id='" + playerinfo.Character.Information.CharacterID + "'");
                    //Send new character guild title update to each player in spawn reach
                    Send(Packet.GuildSetTitle(Character.Guild.GuildID, playerinfo.Character.Information.Name, GuildMemberTitle));
                    //Send Final packet to client
                    playerinfo.client.Send(Packet.GuildSetTitle2(Character.Guild.GuildID, SelectedGuildMember, GuildMemberTitle));
                }
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Guild Title Error: {0}", ex);
                //Write information to debug log
                Systems.Debugger.Write(ex);
            }
        }
    }
}
