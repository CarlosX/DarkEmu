///////////////////////////////////////////////////////////////////////////
// DarkEmu: guild points
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        void DonateGP()
        {
            //First we write our function inside a catcher
            try
            {
                //Max level of guild wont allow new gp donations.
                if (Character.Network.Guild.Level == 5)
                {
                    //Send error message to client
                    client.Send(Packet.IngameMessages(SERVER_GUILD_PROMOTE_MSG, IngameMessages.UIIT_MSG_GUILD_LACK_GP));
                    return;
                }
                //Open our packet reader
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                //Read donated gp amount (int).
                int donatedgp = reader.Int32();
                //Close packet reader
                reader.Close();
                //Anti hack checking (If donated gp higher is then the player skillpoints.
                if (donatedgp > Character.Information.SkillPoint) 
                    return;
                //Calculate total
                int totalgp = Character.Network.Guild.PointsTotal + donatedgp;
                //Set guild points total
                Character.Network.Guild.PointsTotal += donatedgp;
                //Set skill points minus donated amount
                Character.Information.SkillPoint -= donatedgp;
                //Set donated gp + donated skill points
                Character.Network.Guild.DonateGP += donatedgp;
                //Save our information (Skill points).
                SavePlayerInfo();
                //Update database total guild points
                MsSQL.UpdateData("UPDATE guild SET guild_points='" + totalgp + "' WHERE guild_name='" + Character.Network.Guild.Name + "'");
                //Update database donated player guild points amount
                MsSQL.UpdateData("UPDATE guild_members SET guild_points='" + Character.Network.Guild.DonateGP + "' WHERE guild_member_id='"+ Character.Information.CharacterID +"'");
                //Send packets to donator.
                client.Send(Packet.InfoUpdate(1, totalgp, 0));
                //Send donated gp info
                PacketWriter writer = new PacketWriter();
                //Add opcode
                writer.Create(Systems.SERVER_GUILD_DONATE_GP);
                //Write static byte 1
                writer.Byte(1);
                //Write dword int value donated gp amount.
                writer.DWord(donatedgp);
                //Send bytes to client
                client.Send(writer.GetBytes());
                //Repeat for each member in our guild
                foreach (int member in Character.Network.Guild.Members)
                {
                    //Make sure that the member isnt null
                    if (member != 0)
                    {
                        //Now we get the detailed information for each member
                        Systems guildmember = GetPlayerMainid(member);
                        //Make sure the guildmember is still there
                        if (guildmember != null)
                        {
                            //Send guild update packets to each member (Donated gp information and % bar update).
                            guildmember.client.Send(Packet.GuildUpdate(Character, 13, 0, 0, totalgp));
                            guildmember.client.Send(Packet.GuildUpdate(Character, 9, 0, 0, totalgp));
                        }
                    }
                }
            }
            //Catch any bad exception error
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("Donate GP Error {0}", ex);
                //Write information to the debug log.
                Systems.Debugger.Write(ex);
            }
        }
        //This is used while a player is fighting / killing
        void UpdateGP()
        {
            //TODO: GP Donate on kill for each member in the guild update packet.
        }
    }
}