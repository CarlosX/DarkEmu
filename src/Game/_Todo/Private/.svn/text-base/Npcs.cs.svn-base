///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SrxRevo
{
    public partial class Systems
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Close Npc
        /////////////////////////////////////////////////////////////////////////////////
        protected void Close_NPC()
        {
            #region Close npc
            Character.State.Busy = false;
            client.Send(Packet.CloseNPC());
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Open Npc
        /////////////////////////////////////////////////////////////////////////////////
        protected void Open_NPC()
        {
            #region Open Npc
            try
            {
                Character.State.Busy = true;
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                Reader.UInt32();
                byte type = Reader.Byte();

                if (type == 1)
                {
                    client.Send(Packet.OpenNPC(type));
                }
                else
                {
                    client.Send(Packet.OpenNPC(type));
                }
                Reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            #endregion
        }
        /////////////////////////////////////////////////////////////////////////////////
        // Npc Class
        /////////////////////////////////////////////////////////////////////////////////
        public class NPC
        {
            /////////////////////////////////////////////////////////////////////////////////
            // Talk to npc
            /////////////////////////////////////////////////////////////////////////////////
            public static void Chat(int model, PacketWriter Writer)
            {
                #region Talk to npc
                string[] name = Data.ObjectBase[model].Name.Split('_');
                Console.WriteLine("NPC NAME: {0}  FULL NAME {1}", name[2], Data.ObjectBase[model].Name);

                switch (name[2])
                {
                    case "GUILD":
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0x0F);
                        Writer.Byte(0);
                        break;
                    case "THIEF":
                        if (name.Contains("GATE"))
                        {
                            // Thief Gate
                            Writer.Byte(1);
                            Writer.Word(0xA640);
                            Writer.Byte(0);
                            Writer.Byte(2);
                            Writer.Byte(8);
                            Writer.Byte(8);
                        }
                        else
                        {
                            //Thief npc
                            Writer.Byte(0);
                            Writer.Byte(2);
                            Writer.Byte(1);
                            Writer.Byte(0x15);
                            Writer.Byte(0);
                            if (Info(Data.ObjectBase[model].Name)) Writer.Word(0x00);
                        }
                        break;
                    case "GENARAL":
                        //Jangan Npc Guild
                        if (name.Contains("SP"))
                        {
                            Writer.Byte(0);
                            Writer.Byte(3);
                            Writer.Byte(1);
                            Writer.Byte(2);
                            Writer.Byte(0x0F);
                            Writer.Byte(0);
                            if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        }
                        else
                        {
                            Writer.Byte(0);
                            Writer.Byte(2);
                            Writer.Byte(1);
                            Writer.Byte(0x16);
                            Writer.Byte(0);
                            if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        }
                        break;
                    case "DESIGNER":
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "DOCTOR":
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0x14);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "ISLAM":
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "MERCHANT":
                        if (name.Contains("GATE"))
                        {
                            Writer.Byte(1);
                            Writer.Word(0xA640);
                            Writer.Byte(0);
                            Writer.Byte(2);
                            Writer.Byte(8);
                            Writer.Byte(8);
                        }
                        else
                        {
                            Writer.Byte(0);
                            Writer.Byte(2);
                            Writer.Byte(1);
                            Writer.Byte(0);
                            Writer.Byte(0);
                            if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        }
                        break;
                    case "EVENT":
                        Writer.Byte(0);
                        Writer.Byte(1);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "MARKET":
                    case "ARENA":
                    case "TICKET":
                    case "LOTTERY":
                    case "BIGMAN":
                    case "CHEF":
                    case "MOONSHADOW":
                    case "INDIA":
                    case "PRIEST":
                    case "KISAENG1":
                    case "KISAENG2":
                    case "KISAENG3":
                    case "KISAENG4":
                    case "KISAENG5":
                    case "ADVENTURER":
                    case "UNION1":
                    case "UNION2":
                    case "CHINABOY":
                    case "M":
                    case "WITCH":
                    case "ADVICE":
                    case "ADVICE3":
                    case "CONSUL":
                    case "HUNTER":
                    case "COMMERCE1":
                    case "GENERAL":
                    case "FORTRESS":
                    case "BEGGARBOY":
                    case "GACHA":
                        /*
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0x11);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                        */
                    case "KISAENG6":
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        /*
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0x08);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        */
                        break;
                    case "EUROPE":
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0x02);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "SMITH":
                        //00 < Byte 04 < amount of 01020420 < types 00 < end byte 1400 < tax like jangan
                        Writer.Byte(0);
                        Writer.Byte(4);
                        Writer.Byte(1);
                        Writer.Byte(2);
                        Writer.Byte(4);
                        Writer.Byte(0x20);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "HORSE":
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0xB);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "ARMOR":
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(4);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "POTION":
                        Writer.Byte(0);
                        Writer.Byte(1);
                        Writer.Byte(1);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "SPECIAL":
                        if (name.Contains("CH"))
                        {
                            Writer.Byte(0);
                            Writer.Byte(1);
                            Writer.Byte(2);
                            Writer.Byte(0);
                            Writer.Word(0x14);
                        }
                        else if (name.Contains("KT"))
                        {
                            Writer.Byte(0);
                            Writer.Byte(1);
                            Writer.Byte(2);
                            Writer.Byte(0);
                            Writer.Word(0x14);
                        }
                        else
                        {
                            Writer.Byte(0);
                            Writer.Byte(2);
                            Writer.Byte(1);
                            Writer.Byte(0x0C);
                            Writer.Byte(0);
                            if (Info(Data.ObjectBase[model].Name))
                                Writer.Word(0);
                        }
                        break;
                    case "SPECIAL2":
                        if (name.Contains("CH"))
                        {
                            Writer.Byte(0);
                            Writer.Byte(1);
                            Writer.Byte(2);
                            Writer.Byte(0);
                            Writer.Word(0x14);
                        }
                        else
                        {
                            Writer.Byte(0);
                            Writer.Byte(2);
                            Writer.Byte(1);
                            Writer.Byte(0x0C);
                            Writer.Byte(0);
                            if (Info(Data.ObjectBase[model].Name))
                                Writer.Word(0);
                        }
                        break;
                    case "ACCESSORY":
                        Writer.Byte(0);
                        Writer.Byte(1);
                        Writer.Byte(1);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name))
                            Writer.Word(0x14);
                        break;
                    case "WAREHOUSE":
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(3);
                        Writer.Byte(0);
                        break;
                    case "TO":
                    case "FIELD":
                    case "FORT":
                    case "CHARGE":
                    case "GLORY":
                    case "REVIVAL":
                    case "A0":
                    case "GATE":
                    case "GATE1":
                    case "GATE2":
                        Writer.Byte(2);
                        Writer.Byte(7);
                        Writer.Byte(8);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "FERRY":
                    case "FERRY1":
                    case "FERRY2":
                    case "FERRY3":
                    case "FLYSHIP":
                    case "FLYSHIP1":
                    case "FLYSHIP2":
                    case "TUNNEL":
                        Writer.Byte(0);
                        Writer.Byte(1);
                        Writer.Byte(8);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "INVESTIGATION":
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        break;
                    case "SOLDIER":
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        /*
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(2);
                        Writer.Byte(8);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        */
                        break;
                    case "PRIEST3":
                        Writer.Byte(0);
                        Writer.Byte(1);
                        Writer.Byte(2);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name))
                            Writer.Word(0);
                        break;
                    case "TAHOMET":
                         Writer.DWord(0x50);
                         Writer.DWord(0x60);
                         Writer.Byte(0);//Probably level indicator enable / disable
                        break;
                    default:
                        Writer.Byte(0);
                        Writer.Byte(2);
                        Writer.Byte(1);
                        Writer.Byte(0);
                        Writer.Byte(0);
                        if (Info(Data.ObjectBase[model].Name)) Writer.Word(0);
                        Console.WriteLine("Npc name non coded case: " + name[2]);
                        break;
                }
                #endregion
            }
            public static bool Info(string name)
            {
                #region Npc content info
                switch (name)
                {
                    case "NPC_CH_SMITH":
                    case "NPC_CH_ARMOR":
                    case "NPC_CH_POTION":
                    case "NPC_CH_ACCESSORY":
                    case "NPC_KT_SMITH":
                    case "NPC_KT_ARMOR":
                    case "NPC_KT_POTION":
                    case "NPC_KT_ACCESSORY":
                    case "STORE_CH_GATE":
                    case "STORE_KT_GATE":
                    case "NPC_CH_FERRY":
                    case "NPC_CH_FERRY2":
                    case "NPC_KT_FERRY":
                    case "NPC_KT_FERRY2":
                    case "NPC_KT_FERRY3":
                    case "NPC_CH_GUILD":
                    case "NPC_WC_GUILD":
                    case "NPC_KT_GUILD":
                        //case "NPC_CH_SPECIAL":

                        return true;
                }
                return false;
                #endregion
            }
        }
    }
}

