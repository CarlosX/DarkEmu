///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
// File info: Public packet data
///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Data.SqlClient;

namespace DarkEmu_GameServer
{
    public partial class Packet
    {
        ///////////////////////////////////////////////////////////////////////////
        // Spawn Pet Objects
        ///////////////////////////////////////////////////////////////////////////
        public static byte[] ObjectSpawn(pet_obj o)
        {
            PacketWriter Writer = new PacketWriter();

            Writer.Create(Systems.SERVER_SOLO_SPAWN);
            switch (o.Named)
            {
                ///////////////////////////////////////////////////////////////////////////
                // Job transports
                ///////////////////////////////////////////////////////////////////////////
                case 4:
                    Writer.DWord(o.Model);                                      //Pet Model id
                    Writer.DWord(o.UniqueID);                                   //Pet Unique id
                    Writer.Byte(o.xSec);                                        //X sector
                    Writer.Byte(o.ySec);                                        //Y sector
                    Writer.Float(Formule.packetx((float)o.x, o.xSec)); //X
                    Writer.Float(o.z);                                          //Z
                    Writer.Float(Formule.packety((float)o.y, o.ySec)); //Y

                    Writer.Word(0);                                             //Angle

                    Writer.Byte(1);                                             //Walking state
                    Writer.Byte(1);                                             //Static

                    Writer.Byte(o.xSec);                                        //X sector
                    Writer.Byte(o.ySec);                                        //Y sector

                    Writer.Word(0);                                             //Static
                    Writer.Word(0);                                             //
                    Writer.Word(0);                                             //

                    Writer.Word(1);
                    Writer.Word(3);

                    Writer.Byte(0);
                    Writer.Float(o.Walk);                                       //Object walking
                    Writer.Float(o.Run);                                        //Object running
                    Writer.Float(o.Zerk);                                       //Object zerk
                    Writer.Byte(0);//new ?
                    Writer.Word(0);                                             //
                   
                    Writer.Text(o.OwnerName);

                    Writer.Word(2);                                             //
                    Writer.DWord(o.OwnerID);                                    //Owner unique id
                    Writer.Byte(4);                                             //Static byte 4
                    break;
                case 3:
                    ///////////////////////////////////////////////////////////////////////////
                    // Attack pet main packet
                    ///////////////////////////////////////////////////////////////////////////
                    Writer.DWord(o.Model);
                    Writer.DWord(o.UniqueID);
                    Writer.Byte(o.xSec);
                    Writer.Byte(o.ySec);
                    Writer.Float(Formule.packetx((float)o.x, o.xSec));
                    Writer.Float(o.z);
                    Writer.Float(Formule.packety((float)o.y, o.ySec));
                    Writer.Word(0);//angle
                    Writer.Byte(0);
                    Writer.Byte(o.Level);//level
                    Writer.Byte(0);
                    Writer.Word(0);//angle
                    Writer.Byte(1);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Byte(2);
                    Writer.Byte(0);
                    Writer.Float(o.Walk);                                       //Object walking
                    Writer.Float(o.Run);                                        //Object running
                    Writer.Float(o.Zerk);                                       //Object zerk
                    Writer.Byte(0);//new ?
                    Writer.Byte(0);
                    Writer.Byte(0);
                    if (o.Named == 1) 
                        Writer.Text(o.Petname);
                    else 
                        Writer.Word(0);
                    Writer.Text(o.OwnerName);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.DWord(o.OwnerID);
                    Writer.Byte(1);
                    break;
                case 2:
                    ///////////////////////////////////////////////////////////////////////////
                    // Grab pet main packet
                    ///////////////////////////////////////////////////////////////////////////
                    Writer.DWord(o.Model);                                      //Pet Model id
                    Writer.DWord(o.UniqueID);                                   //Pet Unique id
                    Writer.Byte(o.xSec);                                        //X sector
                    Writer.Byte(o.ySec);                                        //Y sector
                    Writer.Float(Formule.packetx((float)o.x, o.xSec)); //X
                    Writer.Float(o.z);                                          //Z
                    Writer.Float(Formule.packety((float)o.y, o.ySec)); //Y

                    Writer.Word(0xDC72);                                        //Angle

                    Writer.Byte(0);                                             //Walking state
                    Writer.Byte(1);                                             //Static
                    Writer.Byte(0);                                             //Static
                    Writer.Word(0xDC72);                                        //Angle

                    Writer.Byte(1);                                             //Static
                    Writer.Word(0);                                             //
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Float(o.Walk);                                       //Object walking
                    Writer.Float(o.Run);                                        //Object running
                    Writer.Float(o.Zerk);                                       //Object zerk
                    Writer.Byte(0);//new ?
                    Writer.Word(0);                                             //
                    if (o.Petname != "No name")
                        Writer.Text(o.Petname);
                    else
                        Writer.Word(0);
                    Writer.Text(o.OwnerName);                                   //Pet owner name
                    Writer.Byte(4);                                             //Static byte 4?
                    Writer.DWord(o.OwnerID);                                    //Owner unique id
                    Writer.Byte(1);                                             //Static byte 1

                    ///////////////////////////////////////////////////////////////////////////
                    break;
                default:
                    ///////////////////////////////////////////////////////////////////////////
                    // // Horse //
                    ///////////////////////////////////////////////////////////////////////////
                    Writer.DWord(o.Model);
                    Writer.DWord(o.UniqueID);
                    Writer.Byte(o.xSec);
                    Writer.Byte(o.ySec);
                    Writer.Float(Formule.packetx((float)o.x, o.xSec));
                    Writer.Float(o.z);
                    Writer.Float(Formule.packety((float)o.y, o.ySec));

                    Writer.Word(0);
                    Writer.Byte(0);
                    Writer.Byte(1);
                    Writer.Byte(0);
                    Writer.Word(0);
                    Writer.Byte(1);
                    Writer.Word(0);
                    Writer.Byte(0);

                    Writer.Float(o.Speed1);
                    Writer.Float(o.Speed2);
                    Writer.Float(o.Zerk);
                    Writer.Byte(0);//new ?
                    Writer.Word(0);
                    Writer.Byte(1);
                    ///////////////////////////////////////////////////////////////////////////
                    break;
            }
            return Writer.GetBytes();
        }
        ///////////////////////////////////////////////////////////////////////////
        // World spawning objects
        ///////////////////////////////////////////////////////////////////////////
        public static byte[] ObjectSpawn(spez_obj so)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SOLO_SPAWN);
            Writer.DWord(0xFFFFFFFF);                                           //Static
            Writer.DWord(so.spezType);                                           //Type
            Writer.DWord(so.ID);                                                //skillid
            Writer.DWord(so.UniqueID);                                          //UniqueID of spawn
            Writer.Byte(so.xSec);                                               //XSec
            Writer.Byte(so.ySec);                                               //Ysec
            Writer.Float(Formule.packetx((float)so.x, so.xSec));       //X
            Writer.Float(so.z);                                                 //Z
            Writer.Float(Formule.packety((float)so.y, so.ySec));       //Y
            Writer.Word(0);                                                     //Angle
            Writer.Byte(1);                                                     //Static
            return Writer.GetBytes();
        }
        public static byte[] ObjectSpawn(obj o)//Monster spawns
        {
            //Create new packet writer
            PacketWriter Writer = new PacketWriter();
            //Add opcode
            Writer.Create(Systems.SERVER_SOLO_SPAWN);
            //Add dword object id
            Writer.DWord(o.ID);
            //Add dword object world id
            Writer.DWord(o.UniqueID);
            //Add x and y sector for object
            Writer.Byte(o.xSec);
            Writer.Byte(o.ySec);
            //Add float x y z for object
            Writer.Float(Formule.packetx((float)o.x, o.xSec));
            Writer.Float(o.z);
            Writer.Float(Formule.packety((float)o.y, o.ySec));
            //Switch on spawn type
            switch (Data.ObjectBase[o.ID].Object_type)
            {
                //Normal monsters
                case Global.objectdata.NamdedType.MONSTER:

                    Writer.Word(0);
                    Writer.Word(1);
                    Writer.Byte(o.xSec);
                    Writer.Byte(o.ySec);

                    if (!File.FileLoad.CheckCave(o.xSec, o.ySec))
                    {
                        Writer.Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
                        Writer.Word(o.z);
                        Writer.Word(Formule.packety((float)(o.y + o.wy), o.ySec));
                    }
                    else
                    {
                        if (o.x < 0)
                        {
                            Writer.Word(Formule.cavepacketx((float)(o.x + o.wx)));
                            Writer.Word(0xFFFF);
                        }
                        else
                        {
                            Writer.DWord(Formule.cavepacketx((float)(o.x + o.wx)));
                        }

                        Writer.DWord(o.z);

                        if (o.y < 0)
                        {
                            Writer.Word(Formule.cavepackety((float)(o.y + o.wy)));
                            Writer.Word(0xFFFF);
                        }
                        else
                        {
                            Writer.DWord(Formule.cavepackety((float)(o.y + o.wy)));
                        }
                    }

                    Writer.Byte(0);
                    Writer.Byte(o.Walking == true ? 2 : 0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Float(o.SpeedWalk);// Walk speed
                    Writer.Float(o.SpeedRun);// Run speed
                    Writer.Float(o.SpeedZerk);// Berserk speed
                    Writer.Byte(0);
                    Writer.Byte(2);
                    Writer.Byte(1);
                    Writer.Byte(5);
                    Writer.Byte(o.Type);
                    Writer.Byte(4);
                    break;

                case Global.objectdata.NamdedType.NPC:
                    Writer.Word(o.rotation);
                    Writer.Byte(0);
                    Writer.Byte(1);
                    Writer.Byte(0);
                    Writer.Word(o.rotation);
                    Writer.Byte(1);//Non static
                    Writer.Byte(0);//Non static
                    Writer.Byte(0);//Non static
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.Float(100);
                    Writer.Byte(0);
                    Writer.Byte(2);
                    Writer.Byte(0);//Static
                    Writer.Byte(1);//Non static
                    break;

                case Global.objectdata.NamdedType.TELEPORT:
                    Writer.Word(0);
                    Writer.Byte(1);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Byte(1);
                    Writer.DWord(0);
                    Writer.DWord(0);
                    Writer.Byte(0);
                    break;
                case Global.objectdata.NamdedType.JOBMONSTER:
                    // Thiefs and trader spawns
                    Writer.Word(0);
                    Writer.Word(1);
                    Writer.Byte(o.xSec);
                    Writer.Byte(o.ySec);

                    if (!File.FileLoad.CheckCave(o.xSec, o.ySec))
                    {
                        Writer.Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
                        Writer.Word(o.z);
                        Writer.Word(Formule.packety((float)(o.y + o.wy), o.ySec));
                    }
                    else
                    {
                        if (o.x < 0)
                        {
                            Writer.Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
                            Writer.Word(0xFFFF);
                        }
                        else
                        {
                            Writer.DWord(Formule.packetx((float)(o.x + o.wx), o.xSec));
                        }

                        Writer.DWord(o.z);

                        if (o.y < 0)
                        {
                            Writer.Word(Formule.packety((float)(o.y + o.wy), o.ySec));
                            Writer.Word(0xFFFF);
                        }
                        else
                        {
                            Writer.DWord(Formule.packety((float)(o.y + o.wy), o.ySec));
                        }
                    }

                    Writer.Byte(1);
                    Writer.Byte(o.Walking == true ? 2 : 0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Float(o.SpeedWalk);// Walk speed
                    Writer.Float(o.SpeedRun);// Run speed
                    Writer.Float(o.SpeedZerk);// Berserk speed
                    Writer.Byte(0);
                    Writer.Byte(2);
                    Writer.Byte(1);
                    Writer.Byte(5);
                    Writer.Byte(0);
                    Writer.Byte(0xE3); // Need to check what this is...
                    Writer.Byte(1);
                    break;
            }
            return Writer.GetBytes();
        }
        ///////////////////////////////////////////////////////////////////////////
        public static byte[] SpawnPortal(obj o, character c, int itemid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SOLO_SPAWN);
            Writer.DWord(o.ID);
            Writer.DWord(o.UniqueID);
            Writer.Byte(o.xSec);
            Writer.Byte(o.ySec);
            Writer.Float(Formule.packetx((float)o.x, o.xSec));
            Writer.Float(o.z);
            Writer.Float(Formule.packety((float)o.y, o.ySec));
            Writer.Word(0);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Byte(1);
            Writer.Byte(6);
            Writer.Text(c.Information.Name);
            Writer.DWord(itemid);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] ObjectSpawn(world_item w)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SOLO_SPAWN);
            Writer.DWord(w.Model);
            switch (w.Type)
            {
                case 1:
                    Writer.DWord(w.amount);
                    Writer.DWord(w.UniqueID);
                    Writer.Byte(w.xSec);
                    Writer.Byte(w.ySec);
                    Writer.Float(Formule.packetx((float)w.x, w.xSec));
                    Writer.Float(w.z);
                    Writer.Float(Formule.packety((float)w.y, w.ySec));
                    Writer.DWord(0);
                    Writer.Byte(w.fromType);
                    Writer.DWord(0);
                    break;
                case 2:
                    //Weapon and armory drops
                    Writer.Byte(w.PlusValue);
                    Writer.DWord(w.UniqueID);
                    Writer.Byte(w.xSec);
                    Writer.Byte(w.ySec);
                    Writer.Float(Formule.packetx((float)w.x, w.xSec));
                    Writer.Float(w.z);
                    Writer.Float(Formule.packety((float)w.y, w.ySec));
                    Writer.Byte(0);
                    Writer.Byte(0);
                    Writer.Bool(w.downType);
                    if (w.downType)
                        Writer.DWord(w.Owner);
                    Writer.Byte(0);
                    Writer.Byte(w.fromType);
                    Writer.DWord(w.fromOwner);
                    break;
                case 3:
                    //Other item types
                    //TODO: Define more detailed drops (Quest items, Mall types etc).
                    Writer.DWord(w.UniqueID);
                    Writer.Byte(w.xSec);
                    Writer.Byte(w.ySec);
                    Writer.Float(Formule.packetx((float)w.x, w.xSec));
                    Writer.Float(w.z);
                    Writer.Float(Formule.packety((float)w.y, w.ySec));
                    Writer.Word(0);
                    Writer.Bool(w.downType);
                    if (w.downType) Writer.DWord(w.Owner);
                    Writer.Byte(0);
                    Writer.Byte(w.fromType);
                    Writer.DWord(w.fromOwner);
                    break;
                    //Case 4 will be for event drop treasure box etc.
                case 4:
                    Writer.Word(0);
                    Writer.DWord(w.fromOwner);
                    Writer.Byte(w.xSec);
                    Writer.Byte(w.ySec);
                    Writer.Float(Formule.packetx((float)w.x, w.xSec));
                    Writer.Float(w.z);
                    Writer.Float(Formule.packety((float)w.y, w.ySec));
                    Writer.Byte(1);
                    Writer.DWord(w.UniqueID);
                    Writer.Byte(0);
                    Writer.Byte(5);
                    Writer.DWord(0);

                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] ObjectSpawnJob(character c)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SOLO_SPAWN);

            /////////////////////////////////////////////////////// Character basic info
            #region Basic info
            Writer.DWord(c.Information.Model);
            Writer.Byte(c.Information.Volume);                      //Char Volume
            Writer.Byte(c.Information.Title);                       //Char Title
            Writer.Byte(c.Information.Pvpstate);                    //Pvp state
            if (c.Information.Pvpstate != 0) c.Information.PvP = true;
            Writer.Bool((c.Information.Level < 20 ? true : false)); //Beginners Icon

            Writer.Byte(c.Information.Slots);                       // Amount of items
            #endregion
            /////////////////////////////////////////////////////// Item info
            #region Item info
            Function.Items.PrivateItemPacket(Writer, c.Information.CharacterID, 8, 0,true);
            Writer.Byte(5);
            Function.Items.PrivateItemPacket(Writer, c.Information.CharacterID, 5, 1,true);
            Writer.Byte(0);
            #endregion
            /////////////////////////////////////////////////////// Character Location / id
            #region Location info / state
            Writer.DWord(c.Information.UniqueID);
            Writer.Byte(c.Position.xSec);
            Writer.Byte(c.Position.ySec);
            Writer.Float(Formule.packetx(c.Position.x, c.Position.xSec));
            Writer.Float(c.Position.z);
            Writer.Float(Formule.packety(c.Position.y, c.Position.ySec));
            Writer.Word(0);//angle
            Writer.Bool(c.Position.Walking);
            Writer.Byte(1); // walk:0 run:1 ;)
            
            if (c.Position.Walking)
            {
                Writer.Byte(c.Position.packetxSec);
                Writer.Byte(c.Position.packetySec);

                if (!DarkEmu_GameServer.File.FileLoad.CheckCave(c.Position.packetxSec, c.Position.packetySec))
                {
                    Writer.Word(c.Position.packetX);
                    Writer.Word(c.Position.packetZ);
                    Writer.Word(c.Position.packetY);
                }
                else
                {
                    if (c.Position.packetX < 0)
                    {
                        Writer.Word(c.Position.packetX);
                        Writer.Word(0xFFFF);
                    }
                    else
                    {
                        Writer.DWord(c.Position.packetX);
                    }

                    Writer.DWord(c.Position.packetZ);

                    if (c.Position.packetY < 0)
                    {
                        Writer.Word(c.Position.packetY);
                        Writer.Word(0xFFFF);
                    }
                    else
                    {
                        Writer.DWord(c.Position.packetY);
                    }
                }
            }
            else
            {
                Writer.Byte(1);
                Writer.Word(0);//angle
            }


            Writer.Byte((byte)(c.State.LastState == 128 ? 2 : 1));
            Writer.Byte(0);
            Writer.Byte(3);
            Writer.Byte((byte)(c.Information.Berserking ? 1 : 0));

            Writer.Float(c.Speed.WalkSpeed);
            Writer.Float(c.Speed.RunSpeed);
            Writer.Float(c.Speed.BerserkSpeed);

            Writer.Byte(c.Action.Buff.count);
            for (byte b = 0; b < c.Action.Buff.SkillID.Length; b++)
            {
                if (c.Action.Buff.SkillID[b] != 0)
                {
                    Writer.DWord(c.Action.Buff.SkillID[b]);
                    Writer.DWord(c.Action.Buff.OverID[b]);
                }
            }
            #endregion
            /////////////////////////////////////////////////////// Character Job information / name
            #region Job information & name
            Writer.Text(c.Job.Jobname);
            Writer.Byte(1);
            Writer.Byte(c.Job.level);//Level job
            Writer.Byte(c.Information.Level);//Level char
            Writer.Byte(0);

            if (c.Transport.Right)
            {
                Writer.Byte(1);
                Writer.Byte(0);
                Writer.DWord(c.Transport.Horse.UniqueID);
            }

            else
            {
                Writer.Byte(0);
                Writer.Byte(0);
            }

            Writer.Byte(0);
            Writer.Byte(0);

            if (c.Network.Guild.Guildid > 0)
            {
                Writer.Text(c.Network.Guild.Name);
            }
            else
            {
                Writer.Word(0);//No guild
            }

            Writer.Byte(0);
            Writer.Byte(0xFF);
            Writer.Byte(4);
            #endregion
            return Writer.GetBytes();
        }
        public static byte[] ObjectSpawn(character c)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SOLO_SPAWN);
            /////////////////////////////////////////////////////// Character basic info
            #region Basic info
            Writer.DWord(c.Information.Model);
            Writer.Byte(c.Information.Volume);                      //Char Volume
            Writer.Byte(c.Information.Title);                       //Char Title
            Writer.Byte(c.Information.Pvpstate);                    //Pvp state
            if (c.Information.Pvpstate != 0) c.Information.PvP = true;
            Writer.Bool((c.Information.Level < 20 ? true : false)); //Beginners Icon
            
            Writer.Byte(c.Information.Slots);                       // Amount of items
            #endregion
            /////////////////////////////////////////////////////// Item info
            #region Item info
            Function.Items.PrivateItemPacket(Writer, c.Information.CharacterID, 8, 0,true);
            Writer.Byte(5);
            Function.Items.PrivateItemPacket(Writer, c.Information.CharacterID, 5, 1,true);
            Writer.Byte(0);
            #endregion
            /////////////////////////////////////////////////////// Character Location / id
            #region Location info / state
            Writer.DWord(c.Information.UniqueID);
            Writer.Byte(c.Position.xSec);
            Writer.Byte(c.Position.ySec);
            Writer.Float(Formule.packetx(c.Position.x, c.Position.xSec));
            Writer.Float(c.Position.z);
            Writer.Float(Formule.packety(c.Position.y, c.Position.ySec));
            Writer.Word(0);//angle
            Writer.Bool(c.Position.Walking);
            Writer.Byte(1); // walk:0 run:1 ;)
            //This should send the location information while moving. and where we moving 
            if (c.Position.Walking)
            {
                Writer.Byte(c.Position.packetxSec);
                Writer.Byte(c.Position.packetySec);

                if (!DarkEmu_GameServer.File.FileLoad.CheckCave(c.Position.packetxSec, c.Position.packetySec))
                {
                    Writer.Word(c.Position.packetX);
                    Writer.Word(c.Position.packetZ);
                    Writer.Word(c.Position.packetY);
                }
                else
                {
                    if(c.Position.packetX < 0)
                    {       
                        Writer.Word(c.Position.packetX);
                        Writer.Word(0xFFFF);
                    }
                    else
                    {
                        Writer.DWord(c.Position.packetX);
                    }

                    Writer.DWord(c.Position.packetZ);

                    if(c.Position.packetY < 0)
                    {       
                        Writer.Word(c.Position.packetY);
                        Writer.Word(0xFFFF);
                    }
                    else
                    {
                        Writer.DWord(c.Position.packetY);
                    }
                }
                /*byte[] x = BitConverter.GetBytes(c.Position.packetX);
                Array.Reverse(x);
                Writer.Buffer(x);

                Writer.Word(c.Position.packetZ);

                byte[] y = BitConverter.GetBytes(c.Position.packetY);
                Array.Reverse(y);
                Writer.Buffer(y);*/


            }
            else
            {
                Writer.Byte(1);
                Writer.Word(0);//angle
            }


            Writer.Byte((byte)(c.State.LastState == 128 ? 2 : 1));
            
            Writer.Byte(0);
            //Info : If a player spawns at your location and is walking it send byte 3, else 0 byte.
            if (c.Transport.Right)
                Writer.Byte(c.Transport.Horse.Walking == true ? 3 : 0);
            else
            Writer.Byte(c.Position.Walking == true ? 3 : 0);
            
            Writer.Byte((byte)(c.Information.Berserking ? 1 : 0));
            Writer.Byte(0);
            Writer.Float(c.Speed.WalkSpeed);
            Writer.Float(c.Speed.RunSpeed);
            Writer.Float(c.Speed.BerserkSpeed);

            Writer.Byte(c.Action.Buff.count);
            for (byte b = 0; b < c.Action.Buff.SkillID.Length; b++)
            {
                if (c.Action.Buff.SkillID[b] != 0)
                {
                    Writer.DWord(c.Action.Buff.SkillID[b]);
                    Writer.DWord(c.Action.Buff.OverID[b]);
                }
            }
            #endregion
            /////////////////////////////////////////////////////// Character Job information / name
            #region Job information & name
            Writer.Text(c.Information.Name);
            Writer.Byte(0);
            if (c.Transport.Right)
            {
                Writer.Byte(1);
                Writer.Byte(0);
                Writer.DWord(c.Transport.Horse.UniqueID);
            }

            else
            {
                Writer.Byte(0);
                Writer.Byte(0);
            }

            Writer.Byte(0);
            if (c.Network.Stall != null && c.Network.Stall.ownerID == c.Information.UniqueID)
                Writer.Byte(0x04);
            else
                Writer.Byte(0);
            //Writer.Byte(0);

            if (c.Network.Guild.Guildid > 0)
            {
                Writer.Text(c.Network.Guild.Name);
                if (c.Network.Guild.GrantName != "")
                {
                    Writer.DWord(0);//Icon ?
                    Writer.Text(c.Network.Guild.GrantName);
                }
                else
                {
                    Writer.DWord(0);//Icon
                    Writer.Word(0);//No grantname
                }
            }
            else
            {
                Writer.Word(0);//No guild
                Writer.DWord(0);//No icon
                Writer.Word(0);//No grantname
            }

            Writer.DWord(0);//need to check not static
            Writer.DWord(0);
            Writer.DWord(0);

            Writer.Byte(0);
            Writer.Byte(0);
            if (c.Network.Stall != null && c.Network.Stall.ownerID == c.Information.UniqueID)
            {
                Writer.Text3(c.Network.Stall.StallName);
                Writer.DWord(c.Information.StallModel);
            }
            Writer.Byte(0);

            #endregion
            /////////////////////////////////////////////////////// Pvp state
            #region pvpstate
            if (c.Information.Pvpstate > 0 || c.Information.Murderer)
            {
                Writer.Byte(0x22);
            }
            else
            {
                Writer.Byte(0xFF);
            }
            #endregion
            Writer.Byte(4);
            return Writer.GetBytes();
        }

        public static byte[] ObjectDeSpawn(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SOLO_DESPAWN);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
    }
    public class DeGroupSpawn
    {
        PacketWriter buff_start = new PacketWriter();
        PacketWriter buff_data = new PacketWriter();
        PacketWriter buff_end = new PacketWriter();
        public ushort added;
        public byte[] StartDeGroup()
        {
            buff_start.Create(Systems.SERVER_GROUPSPAWN_START);
            buff_start.Byte(2);
            buff_start.Word(added);
            return buff_start.GetBytes();
        }
        public void StartData()
        {
            buff_data.Create(Systems.SERVER_GROUPSPAWN_DATA);
        }
        public void AddObject(int id)
        {
            buff_data.DWord(id);
            added++;
        }
        public byte[] EndData()
        {
            return buff_data.GetBytes();
        }
        public byte[] EndGroup()
        {
            buff_end.Create(Systems.SERVER_GROUPSPAWN_END);
            return buff_end.GetBytes();
        }
    }
    public class GroupSpawn
    {
        PacketWriter buff_start;
        PacketWriter [] buff_data;
        PacketWriter buff_end;
        public ushort added;

        public GroupSpawn()
        {
            buff_data = new PacketWriter[10]; 
            for(int i = 0; i < 10; ++i)
                buff_data[i] = new PacketWriter();

        }

        public byte[] StartGroup()
        {
            buff_start = new PacketWriter();
            buff_start.Create(Systems.SERVER_GROUPSPAWN_START);
            buff_start.Byte(1);
            if (added >= 10)
            {
                buff_start.Word(10);
                added -= 10;
            }
            else
            {
                buff_start.Word(added);
                added = 0;
            }
            return buff_start.GetBytes();
        }
        public byte[] EndGroup()
        {
            buff_end = new PacketWriter();
            buff_end.Create(Systems.SERVER_GROUPSPAWN_END);
            return buff_end.GetBytes();
        }
        public void StartData()
        {
            buff_data[(added) / 10].Create(Systems.SERVER_GROUPSPAWN_DATA);
        }
        public byte[] EndData(int count)
        {
            return buff_data[count].GetBytes(); //ok
        }
        public void AddObject(obj o)
        {
            if(added % 10 == 0) 
                this.StartData();

            buff_data[added /10].DWord(o.ID);
            buff_data[added / 10].DWord(o.UniqueID); 
            buff_data[added / 10].Byte(o.xSec);
            buff_data[added / 10].Byte(o.ySec);
            buff_data[added / 10].Float(Formule.packetx((float)o.x, o.xSec));
            buff_data[added / 10].Float(o.z);
            buff_data[added / 10].Float(Formule.packety((float)o.y, o.ySec));

            switch (o.LocalType)
            {
                case 0:

                    buff_data[added / 10].Word(0);
                    buff_data[added / 10].Word(1);
                    buff_data[added / 10].Byte(o.xSec);
                    buff_data[added / 10].Byte(o.ySec);

                    if (!File.FileLoad.CheckCave(o.xSec, o.ySec))
                    {
                        buff_data[added / 10].Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
                        buff_data[added / 10].Word(o.z);
                        buff_data[added / 10].Word(Formule.packety((float)(o.y + o.wy), o.ySec));
                    }
                    else
                    {
                        if (o.x < 0)
                        {
                            buff_data[added / 10].Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
                            buff_data[added / 10].Word(0xFFFF);
                        }
                        else
                        {
                            buff_data[added / 10].DWord(Formule.packetx((float)(o.x + o.wx), o.xSec));
                        }

                        buff_data[added / 10].DWord(o.z);

                        if (o.y < 0)
                        {
                            buff_data[added / 10].Word(Formule.packety((float)(o.y + o.wy), o.ySec));
                            buff_data[added / 10].Word(0xFFFF);
                        }
                        else
                        {
                            buff_data[added / 10].DWord(Formule.packety((float)(o.y + o.wy), o.ySec));
                        }
                    }

                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].Byte(o.Walking == true ? 2 : 0);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Float(o.SpeedWalk);// Walk speed
                    buff_data[added / 10].Float(o.SpeedRun);// Run speed
                    buff_data[added / 10].Float(o.SpeedZerk);// Berserk speed
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(2);
                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].Byte(5);
                    buff_data[added / 10].Byte(o.Type);
                    break;

                case 1:
                    buff_data[added / 10].Word(0);
                    buff_data[added / 10].Byte(1); // has destination
                    buff_data[added / 10].Byte(0); // walk flag
                    buff_data[added / 10].Byte(o.xSec);
                    buff_data[added / 10].Byte(o.ySec);

                    if (!File.FileLoad.CheckCave(o.xSec, o.ySec))
                    {
                        buff_data[added / 10].Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
                        buff_data[added / 10].Word(o.z);
                        buff_data[added / 10].Word(Formule.packety((float)(o.y + o.wy), o.ySec));
                    }
                    else
                    {
                        if (o.x < 0)
                        {
                            buff_data[added / 10].Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
                            buff_data[added / 10].Word(0xFFFF);
                        }
                        else
                        {
                            buff_data[added / 10].DWord(Formule.packetx((float)(o.x + o.wx), o.xSec));
                        }

                        buff_data[added / 10].DWord(o.z);

                        if (o.y < 0)
                        {
                            buff_data[added / 10].Word(Formule.packety((float)(o.y + o.wy), o.ySec));
                            buff_data[added / 10].Word(0xFFFF);
                        }
                        else
                        {
                            buff_data[added / 10].DWord(Formule.packety((float)(o.y + o.wy), o.ySec));
                        }
                    }

                    buff_data[added / 10].Byte(1); // 01: alive - 02: dead
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(o.Walking == true ? 2 : 0); // movement flag
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Float(o.SpeedWalk);// Walk speed
                    buff_data[added / 10].Float(o.SpeedRun);// Run speed
                    buff_data[added / 10].Float(o.SpeedZerk);// Berserk speed
                    buff_data[added / 10].Byte(0); // number of buffs later must be added
                    buff_data[added / 10].Byte(2);
                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].Byte(5);
                    buff_data[added / 10].Byte(o.Type);
                    //buff_data.Byte(0); // fix mob spawn 740
                    break;
                case 2: // npc/shops
                    buff_data[added / 10].Word(o.rotation); // Should be angle? yet not changing ingame..
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Word(o.rotation); // Should be angle? yet not changing ingame..
                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].DWord(0);
                    buff_data[added / 10].DWord(0);
                    buff_data[added / 10].Float(100);
                    buff_data[added / 10].Byte(0);//#####################################
                    buff_data[added / 10].Byte(2);//    Npc fix 740
                    buff_data[added / 10].Byte(0);//
                    //buff_data.Byte(0);//#####################################  i dunno this sure we need to test it
                    break;

                case 3: // teleporter
                    buff_data[added / 10].Word(0);
                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].DWord(0);
                    buff_data[added / 10].DWord(0);
                    //buff_data.Byte(0);// teleporter fix 740 dunno :) lets try like this then // this if needed
                    break;
                case 4:
                    // Thiefs and trader spawns
                    buff_data[added / 10].Word(0);
                    buff_data[added / 10].Word(1);
                    buff_data[added / 10].Byte(o.xSec);
                    buff_data[added / 10].Byte(o.ySec);

                    if (!File.FileLoad.CheckCave(o.xSec, o.ySec))
                    {
                        buff_data[added / 10].Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
                        buff_data[added / 10].Word(o.z);
                        buff_data[added / 10].Word(Formule.packety((float)(o.y + o.wy), o.ySec));
                    }
                    else
                    {
                        if (o.x < 0)
                        {
                            buff_data[added / 10].Word(Formule.packetx((float)(o.x + o.wx), o.xSec));
                            buff_data[added / 10].Word(0xFFFF);
                        }
                        else
                        {
                            buff_data[added / 10].DWord(Formule.packetx((float)(o.x + o.wx), o.xSec));
                        }

                        buff_data[added / 10].DWord(o.z);

                        if (o.y < 0)
                        {
                            buff_data[added / 10].Word(Formule.packety((float)(o.y + o.wy), o.ySec));
                            buff_data[added / 10].Word(0xFFFF);
                        }
                        else
                        {
                            buff_data[added / 10].DWord(Formule.packety((float)(o.y + o.wy), o.ySec));
                        }
                    }

                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].Byte(o.Walking == true ? 2 : 0);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Float(o.SpeedWalk);// Walk speed
                    buff_data[added / 10].Float(o.SpeedRun);// Run speed
                    buff_data[added / 10].Float(o.SpeedZerk);// Berserk speed
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(2);
                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].Byte(5);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(0xE3); // Need to check what this is...
                    //buff_data.Byte(1);
                    break;
                case 5:
                    // Static monster spawns
                    buff_data[added / 10].Word(0); // angle
                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].Byte(0); // walk / run
                    buff_data[added / 10].Byte(o.xSec);
                    buff_data[added / 10].Byte(o.ySec);

                    if (!File.FileLoad.CheckCave(o.xSec, o.ySec))
                    {
                        buff_data[added / 10].Word(Formule.packetx((float)o.x, o.xSec));
                        buff_data[added / 10].Word(o.z);
                        buff_data[added / 10].Word(Formule.packety((float)o.y, o.ySec));
                    }
                    else
                    {
                        if (o.x < 0)
                        {
                            buff_data[added / 10].Word(Formule.packetx((float)o.x, o.xSec));
                            buff_data[added / 10].Word(0xFFFF);
                        }
                        else
                        {
                            buff_data[added / 10].DWord(Formule.packetx((float)o.x, o.xSec));
                        }

                        buff_data[added / 10].DWord(o.z);

                        if (o.y < 0)
                        {
                            buff_data[added / 10].Word(Formule.packety((float)o.y, o.ySec));
                            buff_data[added / 10].Word(0xFFFF);
                        }
                        else
                        {
                            buff_data[added / 10].DWord(Formule.packety((float)o.y, o.ySec));
                        }
                    }

                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Float(0);    // Walk speed
                    buff_data[added / 10].Float(0);    // Run speed
                    buff_data[added / 10].Float(100);  // Berserk speed
                    buff_data[added / 10].Byte(0);
                    buff_data[added / 10].Byte(2);
                    buff_data[added / 10].Byte(1);
                    buff_data[added / 10].Byte(5);
                    buff_data[added / 10].Byte(o.Type);
                    //buff_data.Byte(0);

                    o.LocalType = 1; //fix for static flowers,ishades reverts back to type for when selected
                    break;
                default:
                    break;
            }
            added++;
        }
        public void AddObject(character c)
        {
            if (added % 10 == 0)
                this.StartData();
            /////////////////////////////////////////////////////// Character basic info
            #region Basic info
            buff_data[added / 10].DWord(c.Information.Model);
            buff_data[added / 10].Byte(c.Information.Volume);                      //Char Volume
            buff_data[added / 10].Byte(c.Information.Title);                       //Char Title
            buff_data[added / 10].Byte(c.Information.Pvpstate);                    //Pvp state
            if (c.Information.Pvpstate != 0) c.Information.PvP = true;
            buff_data[added / 10].Bool((c.Information.Level < 20 ? true : false)); //Beginners Icon
            buff_data[added / 10].Byte(c.Information.Slots);                       // Amount of items
            #endregion
            /////////////////////////////////////////////////////// Item info
            #region Item info
            Function.Items.PrivateItemPacket(buff_data[added / 10], c.Information.CharacterID, 8, 0,true);
            buff_data[added / 10].Byte(5);
            
            Function.Items.PrivateItemPacket(buff_data[added / 10], c.Information.CharacterID, 5, 1,true);
            buff_data[added / 10].Byte(0);
            #endregion
            /////////////////////////////////////////////////////// Character Location / id
            #region Location info / state
            buff_data[added / 10].DWord(c.Information.UniqueID);
            buff_data[added / 10].Byte(c.Position.xSec);
            buff_data[added / 10].Byte(c.Position.ySec);
            buff_data[added / 10].Float(Formule.packetx(c.Position.x, c.Position.xSec));
            buff_data[added / 10].Float(c.Position.z);
            buff_data[added / 10].Float(Formule.packety(c.Position.y, c.Position.ySec));
            buff_data[added / 10].Word(0);
            buff_data[added / 10].Bool(c.Position.Walking);
            buff_data[added / 10].Byte(1); // walk:0 run:1 ;)
            if (c.Position.Walking)
            {
                buff_data[added / 10].Byte(c.Position.packetxSec);
                buff_data[added / 10].Byte(c.Position.packetySec);

                if (!DarkEmu_GameServer.File.FileLoad.CheckCave(c.Position.packetxSec, c.Position.packetySec))
                {
                    buff_data[added / 10].Word(c.Position.packetX);
                    buff_data[added / 10].Word(c.Position.packetZ);
                    buff_data[added / 10].Word(c.Position.packetY);
                }
                else
                {
                    if(c.Position.packetX < 0)
                    {
                        buff_data[added / 10].Word(c.Position.packetX);
                        buff_data[added / 10].Word(0xFFFF);
                    }
                    else
                    {
                        buff_data[added / 10].DWord(c.Position.packetX);
                    }

                    buff_data[added / 10].DWord(c.Position.packetZ);

                    if(c.Position.packetY < 0)
                    {
                        buff_data[added / 10].Word(c.Position.packetY);
                        buff_data[added / 10].Word(0xFFFF);
                    }
                    else
                    {
                        buff_data[added / 10].DWord(c.Position.packetY);
                    }
                }
            }
            else
            {
                buff_data[added / 10].Byte(0);
                buff_data[added / 10].Word(0);
            }

            buff_data[added / 10].Byte((byte)(c.State.LastState == 128 ? 2 : 1));
            buff_data[added / 10].Byte(0);
            buff_data[added / 10].Byte(1);
            buff_data[added / 10].Byte((byte)(c.Information.Berserking ? 1 : 0));

            buff_data[added / 10].Float(c.Speed.WalkSpeed);
            buff_data[added / 10].Float(c.Speed.RunSpeed);
            buff_data[added / 10].Float(c.Speed.BerserkSpeed);

            buff_data[added / 10].Byte(c.Action.Buff.count);
            for (byte b = 0; b < c.Action.Buff.SkillID.Length; b++)
            {
                if (c.Action.Buff.SkillID[b] != 0)
                {
                    buff_data[added / 10].DWord(c.Action.Buff.SkillID[b]);
                    buff_data[added / 10].DWord(c.Action.Buff.OverID[b]);
                }
            }
            #endregion
            /////////////////////////////////////////////////////// Character Job information / name
            #region Job information & name
            //if (c.Jobinformation.Onjob)
            //{
            //    buff_data.Text(c.Jobinformation.Jobname);
            //    buff_data.Byte(1);
            //}
            // else if (!c.Jobinformation.Onjob)
            //{
            //    buff_data.Text(c.Information.Name);
            //    buff_data.Byte(2);
            //}
            buff_data[added / 10].Text(c.Information.Name);
            buff_data[added / 10].Byte(2);
            buff_data[added / 10].Byte(1);
            buff_data[added / 10].Byte(0);

            if (c.Transport.Right)
            {
                buff_data[added / 10].Byte(1);
                buff_data[added / 10].Byte(0);
                buff_data[added / 10].DWord(c.Transport.Horse.UniqueID);
            }

            else
            {
                buff_data[added / 10].Byte(0);
                buff_data[added / 10].Byte(0);
            }

            buff_data[added / 10].Byte(0);

            if (c.Network.Stall != null && c.Network.Stall.ownerID == c.Information.UniqueID)
                buff_data[added / 10].Byte(0x04);
            else
                buff_data[added / 10].Byte(0);

            buff_data[added / 10].Byte(0);

            if (c.Network.Guild.Guildid != 0)
            {
                buff_data[added / 10].Text(c.Network.Guild.Name);
                if (c.Network.Guild.GrantName != "")
                {
                    buff_data[added / 10].DWord(0);
                    buff_data[added / 10].Text(c.Network.Guild.GrantName);
                }
                else
                {
                    buff_data[added / 10].DWord(0);
                    buff_data[added / 10].Word(0);
                }
            }
            else
            {
                buff_data[added / 10].Word(0);
                buff_data[added / 10].DWord(0);
                buff_data[added / 10].Word(0);
            }

            buff_data[added / 10].DWord(0);  //GUILD AMBLEM
            buff_data[added / 10].DWord(0);  //UNION ID
            buff_data[added / 10].DWord(0);

            buff_data[added / 10].Byte(0); //myb it is 0 or 1
            buff_data[added / 10].Byte(0);

            if (c.Network.Stall != null && c.Network.Stall.ownerID == c.Information.UniqueID)
            {
                buff_data[added / 10].Text3(c.Network.Stall.StallName);
                buff_data[added / 10].DWord(c.Information.StallModel);
            }

            buff_data[added / 10].Byte(0);

            #endregion
            /////////////////////////////////////////////////////// Pvp state
            #region pvpstate
            if (c.Information.Pvpstate > 0 || c.Information.Murderer)
            {
                buff_data[added / 10].Byte(0x22);
            }
            else
            {
                buff_data[added / 10].Byte(0xFF);
            }
            #endregion
            #region Pvpstate for jobs etc
            /*if (c.Jobinformation.Onjob)
            {
                buff_data.Byte(0xFF);
                buff_data.Byte(1);
            }
            else
            {
                buff_data.Byte(4);
            }
             */
            #endregion
            added++;
        }
        public void AddObject(world_item w) 
        {
            if (added % 10 == 0)
                this.StartData();

            buff_data[added / 10].DWord(w.Model);

            if (w.Type == 1)
            {
                buff_data[added / 10].DWord(w.amount);
                buff_data[added / 10].DWord(w.UniqueID);
                buff_data[added / 10].Byte(w.xSec);
                buff_data[added / 10].Byte(w.ySec);
                buff_data[added / 10].Float(Formule.packetx((float)w.x, w.xSec));
                buff_data[added / 10].Float(w.z);
                buff_data[added / 10].Float(Formule.packety((float)w.y, w.ySec));
                buff_data[added / 10].DWord(0);
            }
            else if (w.Type == 2)
            {
                buff_data[added / 10].Byte(w.PlusValue);
                buff_data[added / 10].DWord(w.UniqueID);

                buff_data[added / 10].Byte(w.xSec);
                buff_data[added / 10].Byte(w.ySec);
                buff_data[added / 10].Float(Formule.packetx((float)w.x, w.xSec));
                buff_data[added / 10].Float(w.z);
                buff_data[added / 10].Float(Formule.packety((float)w.y, w.ySec));
                buff_data[added / 10].Byte(0);
                buff_data[added / 10].Byte(0);
                buff_data[added / 10].Bool(w.downType);
                if (w.downType)
                    buff_data[added / 10].DWord(w.Owner);

                buff_data[added / 10].Byte(0);
            }
            else if (w.Type == 3)
            {
                buff_data[added / 10].DWord(w.UniqueID);
                buff_data[added / 10].Byte(w.xSec);
                buff_data[added / 10].Byte(w.ySec);
                buff_data[added / 10].Float(Formule.packetx((float)w.x, w.xSec));
                buff_data[added / 10].Float(w.z);
                buff_data[added / 10].Float(Formule.packety((float)w.y, w.ySec));
                buff_data[added / 10].Word(0);

                buff_data[added / 10].Bool(w.downType);
                if (w.downType) buff_data[added / 10].DWord(w.Owner);

                buff_data[added / 10].Byte(0);
            }
            added++;
        }
    }
}
