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
        /////////////////////////////////////////////////////////////////////////////////
        // Movement
        /////////////////////////////////////////////////////////////////////////////////
        public void Movement()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Character.Action.PickUping = false;
                byte Type = Reader.Byte();

                ////////////////////////////////////////////////////////////////////////////// Sky drome movement
                if (Type == 0)
                {
                    if (!Character.Stall.Stallactive && !Character.Action.PickUping && !Character.State.Die && !Character.Action.sCasting && !Character.Action.sAttack && !Character.Action.nAttack && !Character.Information.Scroll && !Character.State.Sitting && !Character.Information.SkyDroming)
                    {
                        /*
                        if (File.FileLoad.CheckCave(Character.Position.xSec, Character.Position.ySec))
                        {
                            return;
                        }
                        else
                        {
                            
                            Character.Information.SkyDroming = true;
                            byte info = Reader.Byte();
                            ushort angle = Reader.UInt16();

                            Character.Information.Angle = angle / (65535.0 / 360.0);

                            Character.Position.packetxSec = Character.Position.xSec;
                            Character.Position.packetySec = Character.Position.ySec;
                            Character.Position.packetX = (ushort)Game.Formule.packetx(Character.Position.x, Character.Position.xSec);
                            Character.Position.packetY = (ushort)Game.Formule.packetx(Character.Position.x, Character.Position.xSec);
                            
                            double distance = Formule.gamedistance(
                                Character.Position.x,
                                Character.Position.y,
                                Formule.gamex(Character.Position.x, Character.Position.xSec),
                                Formule.gamey(Character.Position.y, Character.Position.ySec));

                            Character.Position.Time = (distance / (Character.Speed.RunSpeed * 0.0768)) * 1000.0;
                            Character.Position.RecordedTime = Character.Position.Time;


                            PacketWriter Writer = new PacketWriter();
                            Writer.Create(Systems.SERVER_MOVEMENT);
                            Writer.DWord(Character.Information.UniqueID);
                            Writer.Byte(0);
                            Writer.Byte(info);
                            Writer.Word(angle);
                            Writer.Byte(1);
                            Writer.Byte(Character.Position.xSec);
                            Writer.Byte(Character.Position.ySec);
                            Writer.Word(Character.Position.packetX);
                            Writer.DWord(Character.Position.z);
                            Writer.Word(Character.Position.packetY);
                            Send(Writer.GetBytes());

                            StartSkyDromeTimer(1000);
                        }*/
                    }
                }

                //------------------------- Normal movement -------------------------//
                if (Type == 1)
                {
                    //If character is in a guild
                    if (Character.Network.Guild.Guildid != 0)
                    {
                        //Repeat for each client in the guild
                        foreach (Client memberclient in Character.Network.Guild.MembersClient)
                        {
                            //Make sure the client is not null
                            if (memberclient != null)
                            {
                                //Send update packet for location of player
                                memberclient.Send(Packet.GuildUpdate(Character, 10, Character.Information.UniqueID, 0, 0));
                            }
                        }
                    }

                    if (Character.Stall.Stallactive) return;
                    if (Character.Action.PickUping) return;
                    StopPickUpTimer();
                    if (Character.State.Die) return;
                    if (Character.Information.Scroll) return;
                    if (Character.State.Sitting) return;
                    if (Character.Information.SkyDroming)
                    {
                        StopSkyDromeTimer();
                    }
                    if (Character.Action.nAttack)
                    {
                        StopAttackTimer();
                        Character.Action.nAttack = false;
                    }
                    if (Character.Action.sAttack)
                    {
                        StopAttackTimer();
                        Character.Action.sAttack = false;
                    }
                    if (Character.Action.sCasting)
                    {
                        StopAttackTimer();
                        Character.Action.sCasting = false;
                    }
                    if (Character.Information.PvpWait)
                    {
                        Send(Packet.PvpInterupt(Character.Information.UniqueID));
                        Character.Information.PvpWait = false;
                        Character.Information.Pvptype = 0;
                        StopPvpTimer();
                    }
                    Character.Position.Walking = true;
                    byte xsec = Reader.Byte();
                    byte ysec = Reader.Byte();
                    float x, y, z;

                    if (!File.FileLoad.CheckCave(xsec, ysec))
                    {
                        x = Reader.Int16();
                        z = Reader.Int16();
                        y = Reader.Int16();
                        double distance = Formule.gamedistance(
                                Character.Position.x,
                                Character.Position.y,
                                Formule.gamex(x, xsec),
                                Formule.gamey(y, ysec));

                        Character.Position.xSec = xsec;
                        Character.Position.ySec = ysec;
                        Character.Position.wX = Formule.gamex(x, xsec) - Character.Position.x;
                        Character.Position.wZ = z;
                        Character.Position.wY = Formule.gamey(y, ysec) - Character.Position.y;

                        Character.Position.packetxSec = xsec;
                        Character.Position.packetySec = ysec;
                        Character.Position.packetX = (ushort)x;
                        Character.Position.packetZ = (ushort)z;
                        Character.Position.packetY = (ushort)y;

                        if (xsec != 0 && ysec != 0)
                        {
                            Send(Packet.Movement(new Global.vektor(Character.Information.UniqueID, x, z, y, xsec, ysec)));
                        }
                        if (Character.Information.Berserking)
                        {
                            Character.Position.Time = (distance / (Character.Speed.RunSpeed * 0.109)) * 1000.0;
                        }
                        else
                        {
                            Character.Position.Time = (distance / (Character.Speed.RunSpeed * 0.109)) * 1000.0;
                        }
                    }
                    else  // Added for cave telepad locations
                    {
                        x = Formule.cavegamex(Reader.Int16(),Reader.Int16()); //Reads the location and retunrs the coords for the cave postion x
                        z = Formule.cavegamez(Reader.Int16(), Reader.Int16()); //Reads the location and retunrs the coords for the cave postion z
                        y = Formule.cavegamey(Reader.Int16(), Reader.Int16()); //Reads the location and retunrs the coords for the cave postion y
                       
                        double distance = Formule.gamedistance(Character.Position.x,Character.Position.y,Formule.cavegamex(x),Formule.cavegamey(y));

                        Character.Position.xSec = xsec;
                        Character.Position.ySec = ysec;
                        Character.Position.wX = Formule.cavegamex(x) - Character.Position.x;
                        Character.Position.wZ = z;
                        Character.Position.wY = Formule.cavegamey(y) - Character.Position.y;

                        Character.Position.packetxSec = xsec;
                        Character.Position.packetySec = ysec;
                        Character.Position.packetX = (ushort)x;
                        Character.Position.packetZ = (ushort)z;
                        Character.Position.packetY = (ushort)y;

                        if (xsec != 0 && ysec != 0)
                        {
                            Send(Packet.Movement(new Global.vektor(Character.Information.UniqueID, x, z, y, xsec, ysec)));
                        }
                        if (Character.Information.Berserking)
                        {
                            Character.Position.Time = (distance / (Character.Speed.BerserkSpeed * 0.0768)) * 1000.0;
                        }
                        else
                        {
                            Character.Position.Time = (distance / (Character.Speed.RunSpeed * 0.0768)) * 1000.0;
                        }
                    }

                    Reader.Close();

                    if (xsec != 0 && ysec != 0)
                    {
                        Send(Packet.Movement(new Global.vektor(Character.Information.UniqueID, x, z, y, xsec, ysec)));
                    }

                    if (Character.Grabpet.Active)
                    {
                        Send(Packet.Movement(new Global.vektor(Character.Grabpet.Details.UniqueID, x + rnd.Next(10, 25), z, y + rnd.Next(10, 25), xsec, ysec)));
                    }
                    if (Character.Attackpet.Active)
                    {
                        Send(Packet.Movement(new Global.vektor(Character.Attackpet.Details.UniqueID, x + rnd.Next(10, 25), z, y + rnd.Next(10, 25), xsec, ysec)));
                    }
                  
                    Character.Position.RecordedTime = Character.Position.Time;
                    StartMovementTimer((int)(Character.Position.Time * 0.1));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Movement error: {0}", ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Pet movement
        /////////////////////////////////////////////////////////////////////////////////
        void MovementPet()
        {
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //This one happens to all pets.
                int petid = Reader.Int32();
                //We switch on type (2 = attack pet, 1 = horse).
                byte type = Reader.Byte();

                switch (type)
                {
                    //Horse pet movement
                    case 1:
                        byte movetype = Reader.Byte();

                        //Normal movement type
                        if (movetype == 1)
                        {
                            //Read xsector information
                            byte xsec = Reader.Byte();
                            //Read ysector information
                            byte ysec = Reader.Byte();
                            //Read x
                            float x = Reader.Int16();
                            //Read z
                            float z = Reader.Int16();
                            //Read y
                            float y = Reader.Int16();
                            Reader.Close();
                            //Make sure attack timer is gone
                            StopAttackTimer();
                            //Set pickup to false
                            Character.Action.PickUping = false;
                            //Set movement active
                            Character.Position.Walking = true;
                            //Calculate distance
                            double distance = Formule.gamedistance(Character.Position.x,
                                Character.Position.y,
                                Formule.gamex(x, xsec),
                                Formule.gamey(y, ysec));
                            //Set character position
                            Character.Position.xSec = xsec;
                            Character.Position.ySec = ysec;
                            Character.Position.wX = Formule.gamex(x, xsec) - Character.Position.x;
                            Character.Position.wZ = z;
                            Character.Position.wY = Formule.gamey(y, ysec) - Character.Position.y;

                            Character.Position.packetxSec = xsec;
                            Character.Position.packetySec = ysec;
                            Character.Position.packetX = (ushort)x;
                            Character.Position.packetZ = (ushort)z;
                            Character.Position.packetY = (ushort)y;

                            Send(Packet.Movement(new Global.vektor(petid, x, z, y, xsec, ysec)));
                            Character.Position.Time = (distance / (95.0 * 0.0768)) * 1000.0;
                            Character.Position.RecordedTime = Character.Position.Time;

                            StartMovementTimer((int)(Character.Position.Time * 0.1));
                        }
                        break;
                    //Attack pet movement
                    case 2:
                        //Set pet info
                        Character.Attackpet.Details.x = Character.Position.x;
                        Character.Attackpet.Details.y = Character.Position.y;
                        Character.Attackpet.Details.z = Character.Position.z;
                        Character.Attackpet.Details.xSec = Character.Position.xSec;
                        Character.Attackpet.Details.ySec = Character.Position.ySec;
                        //Target id information
                        int targetid = Reader.Int32();
                        Reader.Close();
                        //Set pet speed information
                        Send(Packet.SetSpeed(petid, 50, 100));//Need to make correct speed info later
                        //Check distances / target detailed.

                        //Send attack packet (new void pet attack to be created).
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("movement pet error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Movement angle
        /////////////////////////////////////////////////////////////////////////////////
        public void Angle()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                int id = Character.Information.UniqueID;
                ushort angle = Reader.UInt16();
                client.Send(Packet.Angle(id, angle));
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Cave movement teleport
        /////////////////////////////////////////////////////////////////////////////////
        public void Movement_CaveTeleport()// This was changed due to going in and out of caves to change the movment patten
        {
            try
            {
                // if our destination is caveteleport
                foreach (Global.CaveTeleports r in Data.CaveTeleports)
                {
                    if (!File.FileLoad.CheckCave(Character.Position.xSec, Character.Position.ySec))
                    {
                        if (Formule.gamedistance(Formule.packetx(Character.Position.x, Character.Position.xSec), Formule.packety(Character.Position.y, Character.Position.ySec), (float)r.x, (float)r.y) <= 10)
                        {
                            foreach (Global.cavepoint p in Data.cavePointBase)
                            {
                                if (p != null)
                                    if (p.Name == r.name)
                                    {
                                        TeleportCave(p.Number);
                                        break;
                                    }
                            }
                            break;
                        }
                    }
                    else
                    {
                        if (Formule.gamedistance(Formule.cavepacketx(Character.Position.x), Formule.cavepackety(Character.Position.y), (float)r.x, (float)r.y) <= 10)
                        {
                            foreach (Global.cavepoint p in Data.cavePointBase)
                            {
                                if (p != null)
                                    if (p.Name == r.name)
                                    {
                                        TeleportCave(p.Number);
                                        break;
                                    }
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
    }
}
