///////////////////////////////////////////////////////////////////////////
// SRX Revo: guild invite system
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        void GuildInvite()
        {
            //Wrap our code into a catcher
            try
            {
                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Read lenght of invited character name
                Int16 InvitedCharacterLEN = Reader.Int16();
                //Read invited character name
                string InvitedCharacter = Reader.String(InvitedCharacterLEN);
                //Close packet reader
                Reader.Close();
                //Get information for target
                Systems sys = GetPlayerName(InvitedCharacter);
                //Set targetid information
                Character.Network.TargetID = sys.Character.Information.UniqueID;
                //If player allready has a guild
                if (sys.Character.Network.Guild.Guildid != 0)
                {
                    client.Send(Packet.IngameMessages(Systems.SERVER_GUILD_WAIT, IngameMessages.UIIT_MSG_GUILDERR_MEMBER_OF_ANOTHER_GUILD));
                    return;
                }
                //If player has to wait before the player can join another guild
                if (sys.Character.Information.JoinGuildWait)
                {
                    client.Send(Packet.IngameMessages(Systems.SERVER_GUILD_WAIT, IngameMessages.UIIT_MSG_GUILD_PENALTY));
                    return;
                }
                //If the guild has max members
                if (Character.Network.Guild.TotalMembers >= Character.Network.Guild.MaxMembers)
                {
                    client.Send(Packet.IngameMessages(Systems.SERVER_GUILD_WAIT, IngameMessages.UIIT_MSG_GUILDERR_MEMBER_FULL));
                }
                //If the character doesnt have join rights
                if (!Character.Network.Guild.joinRight)
                {
                    //This should not happen unless hack attempt, because button should be grayed out
                    return;
                }
                //Set targetid to the invited player
                sys.Character.Network.TargetID = this.Character.Information.UniqueID;
                //Send guild request packet
                sys.client.Send(Packet.PartyRequest(5, this.Character.Information.UniqueID, 0));
                //Set bools active
                Character.State.GuildInvite = true;
                sys.Character.State.GuildInvite = true;
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Guild invite error {0}", ex);
                //Write information to the debug log
                Systems.Debugger.Write(ex);
            }
        }
    }
}