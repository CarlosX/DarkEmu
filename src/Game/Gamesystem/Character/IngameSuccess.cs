///////////////////////////////////////////////////////////////////////////
// DarkEmu: Ingame success
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    partial class Systems
    {
        void InGameSuccess()
        {
            //Wrap our function inside a catcher
            try
            {
                //If character isnt ingame yet
                if (!Character.InGame)
                {
                    //Load player data
                    PlayerDataLoad();
                    //Load premium ticket
                    LoadTicket(Character);
                    //Load guild information
                    LoadPlayerGuildInfo(true);
                    //Load spawns
                    ObjectSpawnCheck();
                    //Send stats packet
                    client.Send(Packet.PlayerStat(Character));
                    //Send player state packet
                    client.Send(Packet.StatePack(Character.Information.UniqueID, 0x04, 0x02, false));
                    //Send complete load packet
                    client.Send(Packet.Completeload());
                    //Send player silk packet
                    client.Send(Packet.Silk(Player.Silk, Player.SilkPrem));
                    //Set new movement stopwatch
                    Character.Position.Movementwatch = new System.Diagnostics.Stopwatch();
                    //Load friends
                    GetFriendsList();
                    //Set player online
                    MsSQL.UpdateData("UPDATE character SET online='1' WHERE id='" + Character.Information.CharacterID + "'");
                    //Set ingame bool to true
                    this.Character.InGame = true;
                    //Load message welcome (Copyright message leave intact).
                    LoadMessage();
                    //Load guild data
                    GetGuildData();
                    //Update hp and mp
                    UpdateHp();
                    UpdateMp();
                    //Set player to normal state (4 0) / Not attackable = (4 2)
                    client.Send(Packet.StatePack(Character.Information.UniqueID, 0x04, 0x00, false));
                    
                    //Disabled until fully reworked and updated and optimized.
                    //Load transport
                    //LoadTransport();
                    //Load grabpet if active
                    //LoadGrabPet();

                    //Regen
                    this.HPregen(4000);
                    this.MPregen(4000);
                }
            }
            //Catch bad exception errors
            catch (Exception ex)
            {
                //Write information to the console
                Console.WriteLine("InGame Success Error: {0}", ex);
                //Write error to debug log
                Systems.Debugger.Write(ex);
            }
        }
    }
}