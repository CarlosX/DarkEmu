///////////////////////////////////////////////////////////////////////////
// DarkEmu: Action switch
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
        void ActionMain()
        {
            try
            {
                if (Character.State.Die || Character.Information.Scroll || Character.Action.Cast)
                    return;

                //Create new packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Read main action type
                byte type = Reader.Byte();
                //If attack action byte is not equal to 2               
                if (type != 2)
                {
                    //Read player action byte
                    byte PlayerAction = Reader.Byte();
                    //Create switch on player actions
                    switch (PlayerAction)
                    {
                        //Normal attacking
                        case 1:
                            //If character is allready using normal attack
                            if (Character.Action.nAttack)
                            {
                                //Return
                                return;
                            }
                            //If the character is riding a horse
                            if (Character.Transport.Right)
                            {
                                //Return, because character cannot attack while riding a horse.
                                return;
                            }
                            //If the character is picking up a item
                            if (Character.Action.PickUping)
                            {
                                //Stop pick up timer
                                StopPickUpTimer();
                            }
                            //Skip non needed byte
                            Reader.Skip(1);
                            //Read integer target id (DWORD).
                            int TargetID = Reader.Int32();
                            //Close packet reader
                            Reader.Close();
                            //Set target id for usage later in attacking.
                            Character.Action.Object = GetObjects(TargetID);
                            //Set bool normal attack to true
                            Character.Action.nAttack = true;
                            //Set character target id
                            Character.Action.Target = TargetID;
                            //Start attacking
                            StartAttackTimer(1425);
                            break;
                        case 2://pickup
                            if (Character.Action.nAttack) return;
                            if (Character.Action.sAttack) return;
                            if (Character.Action.sCasting) return;
                            if (Character.Action.PickUping) return;

                            Reader.Byte();
                            int id2 = Reader.Int32();
                            Reader.Close();

                            Character.Action.Target = id2;
                            Character.Action.PickUping = true;
                            StartPickupTimer(600);
                            break;
                        case 3://trace
                            if (Character.Action.sAttack) return;
                            if (Character.Action.sCasting) return;
                            if (Character.State.Sitting) return;
                            if (Character.Stall.Stallactive) return;

                            Reader.Byte();
                            int id3 = Reader.Int32();
                            Character.Action.Target = id3;
                            Reader.Close();
                            client.Send(Packet.ActionState(1, 1));
                            Character.Action.PickUping = false;
                            Player_Trace(id3);
                            break;
                        case 4://use skill
                            Character.Action.UsingSkillID = Reader.Int32();
                            SkillMain(Reader.Byte(), Reader);
                            break;
                        case 5:
                            int id4 = Reader.Int32();
                            byte b_index = SkillGetBuffIndex(id4);
                            SkillBuffEnd(b_index);
                            break;
                        default:
                            Console.WriteLine("ActionMain case: " + Reader.Byte());
                            break;
                    }
                }

                else
                    StopAttackTimer();
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
    }
}
