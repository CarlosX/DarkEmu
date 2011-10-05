///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
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
        void StopTimers()
        {
            try
            {
                if (Timer.Attack != null) StopAttackTimer();
                if (Timer.Casting != null) return;
                if (Timer.Berserker != null) StopBerserkTimer();
                if (Timer.Pvp != null) StopPvpTimer();
                if (Timer.Regen != null) { StopHPRegen(); StopMPRegen(); }
                if (Timer.Scroll != null) return;
                if (Timer.Sitting != null) StopSitDownTimer();
                if (Timer.SkillCasting != null) StopSkillTimer();
                if (Timer.Movement != null) StopMovementTimer();
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        //###########################################################################################
        // Start teleporting
        //###########################################################################################
        void Teleport_Start()
        {
            try
            {
                lock (this)
                {
                    //Checks before we continue
                    if (Character.Action.PickUping) return;
                    //Timer checks
                    StopTimers();
                    //Open the packet reader
                    PacketReader Reader = new PacketReader(PacketInformation.buffer);
                    //Teleport id
                    int teleportidinfo = Reader.Int32();
                    //Number
                    byte number = Reader.Byte();
                    //Teleport selected
                    int teleportselect = Reader.Int32();
                    Reader.Close();
                    //Find price information
                    int price = Data.TeleportPrice.Find(pc => (pc.ID == number)).price;
                    //If the user has less gold then it costs
                    if (this.Character.Information.Gold < price)
                    {
                        //Send error message
                        this.client.Send(Packet.IngameMessages(SERVER_TELEPORTSTART, IngameMessages.UIIT_MSG_INTERACTION_FAIL_NOT_ENOUGH_MONEY));
                        return;
                    }
                    //If the user level is lower then the required level                   
                    if (Data.TeleportPrice.Find(dd => (dd.ID == teleportselect)).level > 0 && Character.Information.Level < Data.TeleportPrice.Find(dd => (dd.ID == teleportselect)).level)
                    {
                        client.Send(Packet.IngameMessages(SERVER_TELEPORTSTART, IngameMessages.UIIT_MSG_INTERACTION_FAIL_OUT_OF_REQUIRED_LEVEL_FOR_TELEPORT));
                        return;
                    }
                    //If the user is currently with job transport (TODO).

                    //Update players gold
                    this.Character.Information.Gold -= price;
                    //Update players gold in database
                    SaveGold();
                    //Close buffs
                    BuffAllClose();
                    //Send teleport packet #1
                    this.client.Send(Packet.TeleportStart());
                    //Set state
                    this.Character.InGame = false;
                    //Update location
                    Teleport_UpdateXYZ(Convert.ToByte(teleportselect));
                    //Despawn objects
                    ObjectDeSpawnCheck();
                    //Despawn player to other players
                    DeSpawnMe();
                    //Required
                    client.Send(Packet.TeleportStart2());
                    //Send loading screen image
                    this.client.Send(Packet.TeleportImage(Data.PointBase[Convert.ToByte(teleportselect)].xSec, Data.PointBase[Convert.ToByte(teleportselect)].ySec));
                    //Set bool
                    this.Character.Teleport = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Teleport select error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }

        //###########################################################################################
        // Data
        //###########################################################################################
        void Teleport_Data()
        {
            try
            {
                if (Character.Teleport)
                {
                    StopBerserkTimer();
                    CheckCharStats(Character);
                    client.Send(Packet.StartPlayerLoad());
                    client.Send(Packet.Load(Character));
                    client.Send(Packet.EndPlayerLoad());

                    //snowflake event packet no need
                    //client.Send(Packet.PlayerUnknowPack(Character.Information.UniqueID));
                    //client.Send(Packet.UnknownPacket());
                    SavePlayerPosition();


                    if (Character.Action.MonsterID.Count > 0)
                    {
                        Character.Action.MonsterID.Clear();
                    }
                    if (Character.Transport.Right)
                    {
                        pet_obj o = Character.Transport.Horse;
                        Character.Transport.Spawned = true;
                        Character.Transport.Horse.Information = true;
                        Send(Packet.Player_UpToHorse(this.Character.Information.UniqueID, true, o.UniqueID));
                    }
                    if (Character.Attackpet.Active)
                    {
                        pet_obj o = Character.Attackpet.Details;
                        //Global.slotItem item =
                        //client.Send(Packet.Pet_Information_grab(o, slot));
                    }
                    if (Character.Grabpet.Active)
                    {
                        //pet_obj o = Character.Grabpet.Details;
                        //client.Send(Packet.Pet_Information_grab(o, slot));
                    }
                    ObjectSpawnCheck();
                    Character.Teleport = false;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
                Console.WriteLine(ex);
            }
        }
        //###########################################################################################
        // Update x y z
        //###########################################################################################
        void Teleport_UpdateXYZ(byte number)
        {

            this.Character.Position.xSec = Data.PointBase[number].xSec;
            this.Character.Position.ySec = Data.PointBase[number].ySec;
            this.Character.Position.x = (float)Data.PointBase[number].x;
            this.Character.Position.z = (float)Data.PointBase[number].z;
            this.Character.Position.y = (float)Data.PointBase[number].y;

            if (Character.Transport.Right)
            {
                this.Character.Transport.Horse.xSec = Data.PointBase[number].xSec;
                this.Character.Transport.Horse.ySec = Data.PointBase[number].ySec;
                this.Character.Transport.Horse.x = (float)Data.PointBase[number].x;
                this.Character.Transport.Horse.z = (float)Data.PointBase[number].z;
                this.Character.Transport.Horse.y = (float)Data.PointBase[number].y;
            }

            if (Character.Grabpet.Active)
            {
                Character.Grabpet.Details.xSec = Data.PointBase[number].xSec;
                Character.Grabpet.Details.ySec = Data.PointBase[number].ySec;
                Character.Grabpet.Details.x = (float)Data.PointBase[number].x + rnd.Next(1, 3);
                Character.Grabpet.Details.z = (float)Data.PointBase[number].z;
                Character.Grabpet.Details.y = (float)Data.PointBase[number].y + rnd.Next(1, 3);
            }

            if (Character.Attackpet.Active)
            {
                Character.Attackpet.Details.xSec = Data.PointBase[number].xSec;
                Character.Attackpet.Details.ySec = Data.PointBase[number].ySec;
                Character.Attackpet.Details.x = (float)Data.PointBase[number].x + rnd.Next(1, 3);
                Character.Attackpet.Details.z = (float)Data.PointBase[number].z;
                Character.Attackpet.Details.y = (float)Data.PointBase[number].y + rnd.Next(1, 3);
            }
            //return BitConverter.ToInt16(new byte[2] { Data.PointBase[number].xSec, Data.PointBase[number].ySec }, 0);
        }
        //###########################################################################################
        // Cave teleports
        //###########################################################################################
        void TeleportCave(int number)
        {
            try// Changed to cavePointbase for the telepad locations
            {
                BuffAllClose();

                DeSpawnMe();
                ObjectDeSpawnCheck();
                this.client.Send(Packet.TeleportOtherStart());

                this.Character.Position.xSec = Data.cavePointBase[number].xSec;
                this.Character.Position.ySec = Data.cavePointBase[number].ySec;
                this.Character.Position.x = (float)Data.cavePointBase[number].x;
                this.Character.Position.z = (float)Data.cavePointBase[number].z;
                this.Character.Position.y = (float)Data.cavePointBase[number].y;
                this.Character.InGame = false;
                this.Character.Teleport = true;

                this.client.Send(Packet.TeleportImage(Data.cavePointBase[number].xSec, Data.cavePointBase[number].ySec));
                this.Character.Teleport = true;
                Timer.Scroll.Dispose();
                Timer.Scroll = null;
                this.Character.Information.Scroll = false;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        //###########################################################################################
        // Reverse scrolls
        //###########################################################################################
        void HandleReverse(int itemid ,byte select, int objectlocationid)
        {
            try
            {
                //Remove loc // add loc to case 7
                if (Character.Position.Walking) StopMovementTimer();
                if (Character.Action.PickUping) return;
                if (Character.Stall.Stallactive) return;
                Character.Information.Scroll = true;
                StopTimers();
                switch (select)
                {
                    //Move to return point
                    case 2:
                        Send(Packet.StatePack(Character.Information.UniqueID, 0x0B, 0x01, false));                        
                        StartScrollTimer(1000);
                        SavePlayerReturn();
                        break;
                    //Move to last recall point
                    case 3:
                        MsSQL ms = new MsSQL("SELECT * FROM character_rev WHERE charname='" + Character.Information.Name + "'");
                        using (SqlDataReader reader = ms.Read())
                        {
                            while (reader.Read())
                            {
                                byte xSec = reader.GetByte(2);
                                byte ySec = reader.GetByte(3);
                                float x = reader.GetInt32(4);
                                float z = reader.GetInt32(6);
                                float y = reader.GetInt32(5);

                                BuffAllClose();
                                DeSpawnMe();
                                ObjectDeSpawnCheck();
                                client.Send(Packet.TeleportOtherStart());
                                //Set state
                                this.Character.InGame = false;
                                Character.Position.xSec = xSec;
                                Character.Position.ySec = ySec;
                                Character.Position.x = x;
                                Character.Position.z = z;
                                Character.Position.y = y;

                                client.Send(Packet.TeleportImage(xSec, ySec));
                                Character.Teleport = true;
                                Timer.Scroll = null;
                                Character.Information.Scroll = false;
                            }
                        }
                        ms.Close();
                        break;
                    //Teleport to map location
                    case 7:
                        BuffAllClose();
                        ObjectDeSpawnCheck();
                        DeSpawnMe();

                        client.Send(Packet.TeleportOtherStart());

                        Character.Position.xSec = Data.ReverseData[objectlocationid].xSec;
                        Character.Position.ySec = Data.ReverseData[objectlocationid].ySec;
                        Character.Position.x = (float)Data.ReverseData[objectlocationid].x;
                        Character.Position.z = (float)Data.ReverseData[objectlocationid].z;
                        Character.Position.y = (float)Data.ReverseData[objectlocationid].y;
                        //Set state
                        this.Character.InGame = false;

                        client.Send(Packet.TeleportImage(Data.ReverseData[objectlocationid].xSec, Data.ReverseData[objectlocationid].ySec));
                        Character.Teleport = true;
                        Timer.Scroll = null;
                        Character.Information.Scroll = false;
                        break;
                }
            }

            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        //###########################################################################################
        // Thief scrolls
        //###########################################################################################
        void HandleThiefScroll(int ItemID)
        {
            try
            {
                //TODO: Add check if user is wearing thief suit.
                if (Character.Position.Walking) return;
                if (Character.Action.PickUping) return;
                if (Character.Stall.Stallactive) return;

                BuffAllClose();

                DeSpawnMe();
                ObjectDeSpawnCheck();
                client.Send(Packet.TeleportOtherStart());

                byte xSec = 182;
                byte ySec = 96;
                float x = 9119;
                float z = 3;
                float y = 890;

                Character.Position.xSec = xSec;
                Character.Position.ySec = ySec;
                Character.Position.x = x;
                Character.Position.z = z;
                Character.Position.y = y;

                UpdateXY();

                client.Send(Packet.TeleportImage(xSec, ySec));
                Character.Teleport = true;
                Timer.Scroll.Dispose();
                Timer.Scroll = null;
                Character.Information.Scroll = false;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }

        }
        //###########################################################################################
        // Normal return scrolls
        //###########################################################################################
        void HandleReturnScroll(int ItemID)
        {
            try
            {
                if (Character.Position.Walking) return;
                if (Character.Action.PickUping) return;
                if (Character.Stall.Stallactive) return;
                Send(Packet.StatePack(Character.Information.UniqueID, 0x0B, 0x01, false));
                Character.Information.Scroll = true;
                StartScrollTimer(Data.ItemBase[ItemID].Use_Time);
                SavePlayerReturn();
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
    }
}
