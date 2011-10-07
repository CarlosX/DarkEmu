///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
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
        void Handle()
        {
            try
            {
                //Read packet information
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte iSlot = Reader.Byte();
                //Get global item information
                Global.slotItem uItemID = GetItem((uint)Character.Information.CharacterID, iSlot, 0);
                //Checks before continuing
                if (uItemID.ID != 0 || !Character.State.Die)
                {
                    //###########################################################################################
                    // Grabpets
                    //###########################################################################################
                    #region Pets (PET OBJECTS)
                    
                    // Grabpets
                    #region Grabpets
                    if (Data.ItemBase[uItemID.ID].Pettype == Global.item_database.PetType.GRABPET)
                    {
                        //Check if we have pet active allready.
                        if (!Character.Grabpet.Active && Character.Action.MonsterID.Count == 0)
                        {
                            //If not active , add new pet object.
                            HandleGrabPet(iSlot, uItemID.ID);
                            //Need to change this to active effect on slot.
                            HandleUpdateSlotn(iSlot, uItemID, Reader.Int32());
                        }
                    }
                    #endregion
                    // Attackpets
                    #region Attackpets
                    else if (Data.ItemBase[uItemID.ID].Pettype == Global.item_database.PetType.ATTACKPET)
                    {
                        //Check if we have pet active allready.
                        if (!Character.Attackpet.Active && Character.Action.MonsterID.Count == 0 || !Character.Attackpet.Active && Character.Action.MonsterID == null)
                        {
                            //If not active , add new pet object.
                            if (!Character.Attackpet.Active)
                            {
                                HandleUpdateSlotn(iSlot, uItemID, Reader.Int32());
                                HandleAttackPet(iSlot, uItemID.ID);
                                client.Send(Packet.Update2(iSlot));
                                client.Send(Packet.ChangeStatus(Character.Information.UniqueID, 5, 0));
                            }
                        }
                    }
                    #endregion
                    #endregion
                    // Horses (NOTE: Detail error messages per if statement (if level, summoned etc).
                    #region Horses
                    if (Data.ItemBase[uItemID.ID].Pettype == Global.item_database.PetType.TRANSPORT)
                    {
                        //Checks before we continue
                        if (!Character.Stall.Stallactive && !Character.Transport.Right && Character.Action.MonsterID.Count == 0 && !Character.State.Sitting && !Character.Information.Scroll)
                        {
                            //Check if level is high enough
                            if (this.Character.Information.Level >= Data.ItemBase[uItemID.ID].Level)
                            {
                                HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                                HandleHorseScroll(uItemID.ID);
                            }
                            //Else
                            else
                            {
                                client.Send(Packet.IngameMessages(SERVER_PLAYER_UPTOHORSE, IngameMessages.UIIT_MSG_COSPETERR_CANT_PETSUM_HIGHLEVEL));
                            }
                        }
                    }
                    #endregion
                    // Special transport
                    #region Special transport
                    else if (Data.ItemBase[uItemID.ID].Pettype == Global.item_database.PetType.ATTACKPET)
                    {

                    }
                    #endregion
                    // Job Transport
                    #region Job Transport
                    else if (Data.ItemBase[uItemID.ID].Pettype == Global.item_database.PetType.JOBTRANSPORT)
                    {
                        //HandleJobTransport(uItemID.ID);
                    }
                    #endregion
                    //###########################################################################################
                    // Potions
                    //###########################################################################################
                    #region Potions
                    #region HP potion
                    if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.HP_POTION && Data.ItemBase[uItemID.ID].Etctype != Global.item_database.EtcType.HPSTATPOTION)
                    {
                        HandlePotion(1, uItemID.ID);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                    }
                    else if (Data.ItemBase[uItemID.ID].Name.Contains("_HP_") && Data.ItemBase[uItemID.ID].Name.Contains("_SUPERSET_"))
                    {
                        HandlePotion(1, uItemID.ID);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                    }
                    #endregion
                    #region HP STAT Potions
                    else if (Data.ItemBase[uItemID.ID].Etctype != Global.item_database.EtcType.HP_POTION && Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.HPSTATPOTION)
                    {

                    }
                    #endregion
                    #region MP potions
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.MP_POTION && Data.ItemBase[uItemID.ID].Etctype != Global.item_database.EtcType.MPSTATPOTION)
                    {
                        HandlePotion(2, uItemID.ID);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                    }
                    else if (Data.ItemBase[uItemID.ID].Name.Contains("_MP_") && Data.ItemBase[uItemID.ID].Name.Contains("_SUPERSET_"))
                    {
                        HandlePotion(1, uItemID.ID);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                    }
                    #endregion
                    #region HP STAT Potions
                    else if (Data.ItemBase[uItemID.ID].Etctype != Global.item_database.EtcType.HP_POTION && Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.MPSTATPOTION)
                    {

                    }
                    #endregion
                    #region Vigor potions
                    /*
                        else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.VIGOR_POTION)
                        {
                            HandlePotion(5, uItemID.ID);
                            HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                        }
                    */
                    else if (Data.ItemBase[uItemID.ID].Name.Contains("ITEM_ETC_ALL_"))
                    {
                        HandlePotion(5, uItemID.ID);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                    }
                    #endregion
                    #region Speed potions
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.SPEED_POTION)
                    {
                        if (Character.Speed.DefaultSpeed == Character.Speed.RunSpeed ||
                            Character.Speed.DefaultSpeed + Character.Speed.Updateded[20] != Character.Speed.RunSpeed && Character.Information.Berserking)
                        {
                            this.SpecialBuff(Data.ItemBase[uItemID.ID].SkillID);
                            HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                        }
                    }
                    #endregion
                    #region Berserk Potion
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.BERSERKPOTION)
                    {
                        if (Character.Information.BerserkBar < 5)
                        {
                            if (Character.Information.Berserking == false)
                            {
                                this.Character.Information.BerserkBar = 5;
                                client.Send(Packet.InfoUpdate(4, 0, Character.Information.BerserkBar));
                                HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                            }
                        }
                    }
                    #endregion
                    #endregion
                    //###########################################################################################
                    // Tickets
                    //###########################################################################################
                    #region Tickets
                    //Forgotten world
                    #region Forgotten world
                    else if (Data.ItemBase[uItemID.ID].Ticket == Global.item_database.Tickets.DUNGEON_FORGOTTEN_WORLD)
                    {
                        //Must add check if user location currently is forgotten world.
                        //Must add level check of portal.
                        ForgottenWorld(uItemID.ID);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                    }
                    #endregion
                    #endregion
                    //###########################################################################################
                    // Global chat
                    //###########################################################################################
                    #region Global Chat
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.GLOBALCHAT)
                    {
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                        byte something = Reader.Byte();
                        string text = Reader.Text3();
                        Reader.Close();
                        SendAll(Packet.ChatPacket(6, 0, text, Character.Information.Name));
                    }
                    #endregion
                    //###########################################################################################
                    // Stall decoration
                    //###########################################################################################
                    #region Stall decoration
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.STALLDECORATION)
                    {
                        StallDeco(uItemID.ID, iSlot);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                    }
                    #endregion
                    //###########################################################################################
                    // Monster masks
                    //###########################################################################################
                    #region Monster masks
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.MONSTERMASK)
                    {
                        //If character monster mask isnt enabled allready.
                        if (!Character.Transformed)
                        {
                            if (Character.Information.Level >= Data.ItemBase[uItemID.ID].Level)
                            {
                                //Load Mask
                                MonsterMasks(uItemID.ID, iSlot);
                                HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                            }
                            else
                            {
                                //Send msg level to low
                            }
                        }
                        else
                        {
                            //Send msg allready in use
                        }
                    }
                    #endregion
                    //###########################################################################################
                    // Return scrolls
                    //###########################################################################################
                    #region Return scrolls
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.RETURNSCROLL && Data.ItemBase[uItemID.ID].Etctype != Global.item_database.EtcType.REVERSESCROLL)
                    {
                        if (Character.Information.Scroll) return;
                        if (Timer.Movement != null)
                        {
                            Timer.Movement.Dispose();
                            Character.Position.Walking = false;
                        }
                        HandleReturnScroll(uItemID.ID);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());

                    }
                    #endregion
                    //###########################################################################################
                    // Reverse scrolls
                    //###########################################################################################
                    #region Reverse scrolls
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.REVERSESCROLL)
                    {
                        if (Character.Information.Scroll) return;
                        if (Character.Position.Walking) return;
                        //Read item id
                        int itemid = Reader.Int32();
                        //Our switch byte
                        byte type = Reader.Byte();
                        //locations for reverse
                        int locid = 0;
                        if (type == 7) locid = Reader.Int32();
                        //Start our handle
                        HandleReverse(itemid, type, locid);
                        //Update slot
                        HandleUpdateSlot(iSlot, uItemID, itemid);
                    }
                    #endregion
                    //###########################################################################################
                    // Thief scrolls
                    //###########################################################################################
                    #region Thief scrolls
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.BANDITSCROLL)
                    {
                        if (Character.Information.Scroll) return;
                        HandleThiefScroll(uItemID.ID);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                    }
                    #endregion
                    //###########################################################################################
                    // Summon scrolls
                    //###########################################################################################
                    #region Summon scrolls
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.SUMMONSCROLL)
                    {
                        HandleSummon(Character.Information.Level);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                    }
                    #endregion
                    //###########################################################################################
                    // Skin change scrolls
                    //###########################################################################################
                    #region Skin change scrolls
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.CHANGESKIN)
                    {
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                        int skinmodel = Reader.Int32();
                        HandleSkinScroll(skinmodel, uItemID.ID);
                    }
                    #endregion
                    //###########################################################################################
                    // Item change tool
                    //###########################################################################################
                    #region Skin change scrolls
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.ITEMCHANGETOOL)
                    {
                        int itemid = Reader.Int32();
                        byte targetslot = Reader.Byte();
                        //Make sure the item target is not equiped.
                        if (targetslot < 13) return;
                        //Continue 
                        HandleUpdateSlot(iSlot, uItemID, itemid);
                        HandleItemChange(uItemID.ID, iSlot, targetslot);
                    }
                    #endregion
                    //###########################################################################################
                    // Dungeon items
                    //###########################################################################################
                    #region Forgotten world
                    else if (Data.ItemBase[uItemID.ID].Ticket == Global.item_database.Tickets.DUNGEON_FORGOTTEN_WORLD)
                    {
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                        ForgottenWorld(uItemID.ID);
                    }
                    #endregion
                    //###########################################################################################
                    // Inventory expansion
                    //###########################################################################################
                    #region Inventory expansion
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.INVENTORYEXPANSION)
                    {
                        HandleInventoryExp(uItemID.ID);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                    }
                    #endregion
                    //###########################################################################################
                    // Warehouse expansion
                    //###########################################################################################
                    #region Warehouse expansion
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.WAREHOUSE)
                    {
                        HandleWareHouse(uItemID.ID);
                        HandleUpdateSlot(iSlot, uItemID, Reader.Int32());
                    }
                    #endregion
                    //###########################################################################################
                    // Guild related
                    //###########################################################################################
                    #region Guild Icon
                    else if (Data.ItemBase[uItemID.ID].Etctype == Global.item_database.EtcType.GUILD_ICON)
                    {
                        HandleRegisterIcon();
                    }
                    #endregion
                    //###########################################################################################
                    // Non coded types
                    //###########################################################################################
                    else
                    {
                        //Need to make message in progress or such
                        Console.WriteLine("Non Coded Item ID: " + Data.ItemBase[uItemID.ID].ID + " Name: " + Data.ItemBase[uItemID.ID].Name + "");
                    }
                }
                // Close our packet reader
                Reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Item usage error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
    }
}

            /* OLD SYSTEM REF ONLY ATM LATER REMOVAL
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Repair items
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case 3784:
                case 24432:
                case 35249:
                    if (!Character.Action.repair)
                    {
                        Character.Action.repair = true;
                        RepairTimer(30000);
                    //Check if there are any items that need repair
                    double durability = Data.ItemBase[uItemID.ID].Defans.Durability;
                    double currentdurability = Data.ItemBase[uItemID.dbID].Defans.Durability;
                    

                        if (currentdurability < durability)
                        {
                            int countrepairs = MsSQL.GetRowsCount("SELECT * FROM char_items WHERE owner='" + Character.Information.CharacterID + "'");
                            if (countrepairs == 0)
                            {
                                //Do nothing client sends message automaticly
                            }
                            else
                            {
                                //Start repairing call handle
                                HandleRepair(iSlot, uItemID.dbID);
                                HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                            }
                        }
                    }
                    break;
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // MP Potions
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 3820:
                case 3821:
                case 3822:
                case 5913:
                    type = 2;
                    HandlePotion(type, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                case 9:
                    type = 3;
                    HandlePotion(type, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                case 16:
                    type = 4;
                    HandlePotion(type, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Vigor Potions
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 3739:
                case 4013:
                case 5881:
                case 5944:
                case 23316:
                    type = 5;
                    HandlePotion(type, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Buff Items
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case 7100:
                    SpecialBuff(3977);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                case 7098:
                    SpecialBuff(3975);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                case 23311:
                    if (Character.Information.Berserking) return;
                    int style = 1;
                    HandleRegenZerk(style, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                case 23938:
                    if (Character.Information.Berserking) return;
                    style = 2;
                    HandleRegenZerk(style, uItemID.ID);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Balloons
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                case 24342:
                    //HandleBalloon(iSlot, 31157);
                    SpecialBuff(31182);
                    HandleUpdateSlot(iSlot, uItemID, Reader.UInt16());
                    break;
             */