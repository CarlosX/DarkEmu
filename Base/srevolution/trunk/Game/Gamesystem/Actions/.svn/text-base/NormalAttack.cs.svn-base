///////////////////////////////////////////////////////////////////////////
// SRX Revo: Action normal attack
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
        void ActionNormalAttack()
        {
            try
            {
                float x = 0, y = 0;

                bool[] aRound = null;

                if (Character.Action.Object != null)
                {
                    //Set target object as object
                    obj TargetInformation = Character.Action.Object as obj;
                    //If our target object is a monster
                    #region Attacking a monster
                    if (Data.ObjectBase[TargetInformation.ID].Object_type == Global.objectdata.NamdedType.MONSTER)
                    {
                        //If state is dead
                        if (TargetInformation.State == 4)
                        {
                            //Set normal attack bool to false
                            Character.Action.nAttack = false;
                            //Stop attack timer
                            StopAttackTimer();
                        }

                        if (TargetInformation.Agro == null)
                            TargetInformation.Agro = new List<_agro>();
                        x = (float)TargetInformation.x;
                        y = (float)TargetInformation.y;

                        if (!TargetInformation.Attacking)
                            TargetInformation.AddAgroDmg(Character.Information.UniqueID, 1);
                        if (TargetInformation.Die || TargetInformation.GetDie)
                        {
                            StopAttackTimer();
                            return;
                        }
                    }
                    #endregion
                    //If attacking a player
                    #region Attacking a player
                    if (Data.ObjectBase[TargetInformation.ID].Object_type == Global.objectdata.NamdedType.PLAYER)
                    {
                        if (!Character.Information.PvP)
                        {
                            Character.Action.nAttack = false;
                            StopAttackTimer();
                            return;
                        }

                        Systems sys = Character.Action.Object as Systems;
                        if (sys.Character.State.LastState == 4)
                        {
                            StopAttackTimer();
                            return;
                        }

                        if (!(Character.Information.PvP && sys.Character.Information.PvP))
                        {
                            StopAttackTimer();
                            return;
                        }
                        if (!Character.InGame)
                        {
                            StopAttackTimer();
                            return;
                        }
                        x = sys.Character.Position.x;
                        y = sys.Character.Position.y;
                        aRound = sys.Character.aRound;
                    }
                    #endregion

                    double distance = Formule.gamedistance(Character.Position.x,
                            Character.Position.y,
                            x,
                            y);

                    if (Character.Information.Item.wID == 0)
                        distance -= 0.5;
                    else
                        distance -= Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;

                    if (distance > 0)
                    {
                        float farkx = x;
                        float farky = y;

                        if (Character.Information.Item.wID == 0)
                        {
                            Character.Position.wX = farkx - Character.Position.x - 0;
                            Character.Position.wY = farky - Character.Position.y - 0;
                            Character.Position.kX = Character.Position.wX;
                            Character.Position.kY = Character.Position.wY;
                        }
                        else
                        {
                            Character.Position.wX = farkx - Character.Position.x - (float)Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                            Character.Position.wY = farky - Character.Position.y - (float)Data.ItemBase[Character.Information.Item.wID].ATTACK_DISTANCE;
                            Character.Position.kX = Character.Position.wX;
                            Character.Position.kY = Character.Position.wY;
                        }

                        Send(Packet.Movement(new SrxRevo.Global.vektor(Character.Information.UniqueID,
                                    (float)Formule.packetx((float)farkx, Character.Position.xSec),
                                    (float)Character.Position.z,
                                    (float)Formule.packety((float)farky, Character.Position.ySec),
                                    Character.Position.xSec,
                                    Character.Position.ySec)));

                        Character.Position.Time = (distance / (Character.Speed.RunSpeed * 0.0768)) * 1000.0;
                        Character.Position.RecordedTime = Character.Position.Time;

                        Character.Position.packetxSec = Character.Position.xSec;
                        Character.Position.packetySec = Character.Position.ySec;

                        Character.Position.packetX = (ushort)Formule.packetx((float)farkx, Character.Position.xSec);
                        Character.Position.packetY = (ushort)Formule.packety((float)farky, Character.Position.ySec);

                        Character.Position.Walking = true;

                        StartMovementTimer((int)(Character.Position.Time * 0.1));

                        return;
                    }
                }
                ActionAttack();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Normal Attack Error : {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
    }
}
