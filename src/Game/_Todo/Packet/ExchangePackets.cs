///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
// File info: Private packet data
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
        public static byte[] Exchange_ItemPacket(int id, List<Global.slotItem> Exhange, bool mine)
        {
            
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_ITEM);
            Writer.DWord(id);
            Writer.Byte(Exhange.Count);

            for (byte i = 0; i < Exhange.Count; i++)
            {
                Systems.LoadBluesid(Exhange[i].dbID);
                if (mine) Writer.Byte(Exhange[i].Slot);

                Writer.Byte(i);
                Writer.DWord(0);
                Writer.DWord(Exhange[i].ID);

                if (Data.ItemBase[Exhange[i].ID].Type == Global.item_database.ArmorType.ARMOR ||
                    Data.ItemBase[Exhange[i].ID].Type == Global.item_database.ArmorType.GARMENT ||
                    Data.ItemBase[Exhange[i].ID].Type == Global.item_database.ArmorType.GM ||
                    Data.ItemBase[Exhange[i].ID].Type == Global.item_database.ArmorType.HEAVY ||
                    Data.ItemBase[Exhange[i].ID].Type == Global.item_database.ArmorType.LIGHT ||
                    Data.ItemBase[Exhange[i].ID].Type == Global.item_database.ArmorType.PROTECTOR ||
                    Data.ItemBase[Exhange[i].ID].Type == Global.item_database.ArmorType.ROBE ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.EARRING ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.RING ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.NECKLACE ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.BLADE ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.BOW ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.EU_AXE ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.EU_CROSSBOW ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.EU_DAGGER ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.EU_DARKSTAFF ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.EU_HARP ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.EU_STAFF ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.EU_SWORD ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.EU_TSTAFF ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.EU_TSWORD ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.GLAVIE ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.SPEAR ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.SWORD ||
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.CH_SHIELD || 
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.EU_SHIELD)
                {
                    Writer.Byte(Exhange[i].PlusValue);
                        Writer.LWord(0);
                        Writer.DWord(Data.ItemBase[Exhange[i].ID].Defans.Durability);
                        if (Data.ItemBlue[Exhange[i].dbID].totalblue != 0)
                        {
                            Writer.Byte(Convert.ToByte(Data.ItemBlue[Exhange[i].dbID].totalblue));
                            for (int a = 1; a <= Data.ItemBlue[Exhange[i].dbID].totalblue; a++)
                            {
                                Writer.DWord(Data.MagicOptions.Find(mg => (mg.Name == Convert.ToString(Data.ItemBlue[Exhange[i].dbID].blue[i]))).ID);
                                Writer.DWord(Data.ItemBlue[Exhange[i].dbID].blueamount[i]);
                            }
                        }

                        else
                        {
                            Writer.Byte(0);
                        }
                        Writer.Word(1);
                        Writer.Word(2);
                        Writer.Word(3);
                }
                else if (Data.ItemBase[Exhange[i].ID].Etctype == Global.item_database.EtcType.STONES)
                {
                    Writer.Word(Exhange[i].Amount);
                    Writer.Byte(0);
                }
                else if (Data.ItemBase[Exhange[i].ID].Etctype == Global.item_database.EtcType.MONSTERMASK)
                {
                    Writer.DWord(0);
                }
                else if (Data.ItemBase[Exhange[i].ID].Etctype == Global.item_database.EtcType.ELIXIR)
                {
                    Writer.Word(1);
                }
                    else if (Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.ARROW || 
                    Data.ItemBase[Exhange[i].ID].Itemtype == Global.item_database.ItemType.BOLT ||
                    Data.ItemBase[Exhange[i].ID].Etctype == Global.item_database.EtcType.HP_POTION ||
                    Data.ItemBase[Exhange[i].ID].Etctype == Global.item_database.EtcType.MP_POTION ||
                    Data.ItemBase[Exhange[i].ID].Etctype == Global.item_database.EtcType.VIGOR_POTION ||
                    Data.ItemBase[Exhange[i].ID].Etctype == Global.item_database.EtcType.SPEED_POTION ||
                    Data.ItemBase[Exhange[i].ID].Ticket == Global.item_database.Tickets.BEGINNER_HELPERS ||
                    Data.ItemBase[Exhange[i].ID].Etctype == Global.item_database.EtcType.ELIXIR ||
                    Data.ItemBase[Exhange[i].ID].Etctype == Global.item_database.EtcType.ALCHEMY_MATERIAL ||
                    Data.ItemBase[Exhange[i].ID].Etctype == Global.item_database.EtcType.EVENT ||
                    Data.ItemBase[Exhange[i].ID].Class_D == 3)
                {
                    Writer.Word(Exhange[i].Amount);
                }
            }
            return Writer.GetBytes();
        }
        public static byte[] Exchange_ItemSlot(byte type, byte slot)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.Byte(slot);
            if (type == 4) Writer.Byte(0);

            return Writer.GetBytes();
        }
        public static byte[] Exchange_Accept()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_ACCEPT);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Accept2()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_ACCEPT2);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Gold(long gold)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_GOLD);
            Writer.Byte(2);
            Writer.LWord(gold);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Approve()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_APPROVE);
            Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Finish()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_FINISHED);
            return Writer.GetBytes();
        }
        public static byte[] Exchange_Cancel()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_CANCEL);
            Writer.Byte(0x2C);
            Writer.Byte(0x18);
            return Writer.GetBytes();
        }
        public static byte[] ItemExchange_Gold(long gold)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(13);
            Writer.LWord(gold);
            return Writer.GetBytes();
        }
        public static byte[] GuildGoldUpdate(long info, byte type)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ITEM_MOVE);
            Writer.Byte(1);
            Writer.Byte(type);
            Writer.LWord(info);
            return Writer.GetBytes();
        }
        public static byte[] OpenExhangeWindow(int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_WINDOW);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        public static byte[] OpenExhangeWindow(byte type, int id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_PROCESS);
            Writer.Bool(true);
            Writer.DWord(id);
            return Writer.GetBytes();
        }
        public static byte[] CloseExhangeWindow()
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EXCHANGE_CLOSE);
            Writer.Bool(true);
            return Writer.GetBytes();
        }
    }
}
