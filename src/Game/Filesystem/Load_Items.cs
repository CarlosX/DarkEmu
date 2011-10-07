///////////////////////////////////////////////////////////////////////////
// DarkEmu: Load Items
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.IO;
using System.Linq;
using System.Text;
using DarkEmu_GameServer.Global;

using System.Collections.Generic;

namespace DarkEmu_GameServer.File
{
    class Load_Items
    {
        public static void SetTimerItems(string listfile)
        {
            //Split information lines
            TxtFile.ReadFromFile(listfile, '\t');
            //Set string info
            string Stringinformation = null;
            //Repeat for each line in the file
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                Stringinformation = TxtFile.lines[l].ToString();
                TxtFile.commands = Stringinformation.Split('\t');
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Timer items");
        }
        
        public static void EventItems(string path)
        {
            //Split information lines
            TxtFile.ReadFromFile(path, '\t');
            //Set string info
            string Stringinformation = null;
            //Repeat for each line in the file
            for (int CurrentItem = 0; CurrentItem <= TxtFile.amountLine - 1; CurrentItem++)
            {
                //Create full string
                Stringinformation = TxtFile.lines[CurrentItem].ToString();
                //Split string
                TxtFile.commands = Stringinformation.Split('\t');
                //Read id from first row
                int id = Convert.ToInt32(TxtFile.commands[0]);
                //Add item to database
                Data.EventDataBase.ID.Add(id);
            }
            //Set definition for console name drop
            string drops = "Drop";
            //If there are more then 1 event drops
            if (TxtFile.amountLine > 1)
                //Set string to DROPS
                drops = "Drops";
            //Write information to the console
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Event " + drops + "");
        }
        public static void EventMonsters(string path)
        {
            //Split information lines
            TxtFile.ReadFromFile(path, '\t');
            //Set string info
            string Stringinformation = null;
            //Repeat for each line in the file
            for (int CurrentItem = 0; CurrentItem <= TxtFile.amountLine - 1; CurrentItem++)
            {
                //Create full string
                Stringinformation = TxtFile.lines[CurrentItem].ToString();
                //Split string
                TxtFile.commands = Stringinformation.Split('\t');
                //Read id from first row
                int id = Convert.ToInt32(TxtFile.commands[0]);
                //Add item to database
                Data.EventDataBase.ID.Add(id);
            }
            //Set definition for console name drop
            string monsters = "Monster";
            //If there are more then 1 event drops
            if (TxtFile.amountLine > 1)
                //Set string to DROPS
                monsters = "Monsters";
            //Write information to the console
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " Event " + monsters + "");
        }
        public static void ItemDatabase(string path)
        {
            //Split information lines
            TxtFile.ReadFromFile(path, '\t');
            //Set string info
            string Stringinformation = null;
            //Repeat for each line in the file
            for (int t = 0; t <= TxtFile.amountLine - 1; t++)
            {
                //Load from file
                #region Load file info
                Stringinformation = TxtFile.lines[t].ToString();
                TxtFile.commands = Stringinformation.Split('\t');
                item_database it = new item_database();
                it.Name = TxtFile.commands[2];
                it.ID = Convert.ToInt32(TxtFile.commands[1]);
                it.Class_A = Convert.ToInt32(TxtFile.commands[9]);
                it.Class_D = Convert.ToInt32(TxtFile.commands[10]);
                it.Class_B = Convert.ToInt32(TxtFile.commands[11]);
                it.Class_C = Convert.ToInt32(TxtFile.commands[12]);
                it.Race = Convert.ToByte(TxtFile.commands[14]);
                it.SOX = Convert.ToByte(TxtFile.commands[15]);
                it.SoulBound = Convert.ToByte(TxtFile.commands[18]);
                it.Shop_price = Convert.ToInt32(TxtFile.commands[26]);
                it.Storage_price = Convert.ToInt32(TxtFile.commands[30]);
                it.Sell_Price = Convert.ToInt32(TxtFile.commands[31]);
                it.Level = Convert.ToByte(TxtFile.commands[33]);
                it.Max_Stack = Convert.ToInt32(TxtFile.commands[57]);
                it.Gender = Convert.ToByte(TxtFile.commands[58]);
                it.Degree = Convert.ToByte(TxtFile.commands[61]);
                //Stone related (Stone creation)
                #region Stone creation
                it.EARTH_ELEMENTS_AMOUNT_REQ = Convert.ToInt32(TxtFile.commands[118]);
                it.EARTH_ELEMENTS_NAME = Convert.ToString(TxtFile.commands[119]);
                it.WATER_ELEMENTS_AMOUNT_REQ = Convert.ToInt32(TxtFile.commands[120]);
                it.WATER_ELEMENTS_NAME = Convert.ToString(TxtFile.commands[121]);
                it.FIRE_ELEMENTS_AMOUNT_REQ = Convert.ToInt32(TxtFile.commands[122]);
                it.FIRE_ELEMENTS_NAME = Convert.ToString(TxtFile.commands[123]);
                it.WIND_ELEMENTS_AMOUNT_REQ = Convert.ToInt32(TxtFile.commands[124]);
                it.WIND_ELEMENTS_NAME = Convert.ToString(TxtFile.commands[125]);
                #endregion
                it.Defans.Durability = Convert.ToDouble(TxtFile.commands[63]);
                it.Defans.MinPhyDef = Convert.ToDouble(TxtFile.commands[65]);
                it.Defans.PhyDefINC = Convert.ToDouble(TxtFile.commands[67]);
                it.Defans.Parry = Convert.ToDouble(TxtFile.commands[68]);
                it.Defans.MinBlock = Convert.ToByte(Math.Round(Convert.ToDouble(TxtFile.commands[74])));
                it.Defans.MaxBlock = Convert.ToByte(Math.Round(Convert.ToDouble(TxtFile.commands[75])));
                it.Defans.MinMagDef = Convert.ToDouble(TxtFile.commands[76]);
                it.Defans.MagDefINC = Convert.ToDouble(TxtFile.commands[78]);
                it.Defans.PhyAbsorb = Convert.ToDouble(TxtFile.commands[79]);
                it.Defans.MagAbsorb = Convert.ToDouble(TxtFile.commands[80]);
                it.Defans.AbsorbINC = Convert.ToDouble(TxtFile.commands[81]);
                it.needEquip = Convert.ToBoolean(Convert.ToByte(TxtFile.commands[93]));
                it.ATTACK_DISTANCE = Convert.ToInt16(TxtFile.commands[94]);
                it.Attack.Min_LPhyAttack = Convert.ToDouble(TxtFile.commands[95]);
                it.Attack.Min_HPhyAttack = Convert.ToDouble(TxtFile.commands[97]);
                it.Attack.PhyAttackInc = Convert.ToDouble(TxtFile.commands[99]);
                it.Attack.Min_LMagAttack = Convert.ToDouble(TxtFile.commands[100]);
                it.Attack.Min_HMagAttack = Convert.ToDouble(TxtFile.commands[102]);
                it.Attack.MagAttackINC = Convert.ToDouble(TxtFile.commands[104]);
                it.Attack.MinAttackRating = Convert.ToDouble(TxtFile.commands[113]);
                it.Attack.MaxAttackRating = Convert.ToDouble(TxtFile.commands[114]);
                it.Attack.MinCrit = Convert.ToByte(Convert.ToDouble(TxtFile.commands[116]));
                it.Attack.MaxCrit = Convert.ToByte(Convert.ToDouble(TxtFile.commands[117]));
                it.ObjectName = TxtFile.commands[119];
                it.Use_Time = Convert.ToInt32(TxtFile.commands[118]);
                it.Use_Time2 = Convert.ToInt32(TxtFile.commands[122]);
                it.MaxBlueAmount = Convert.ToByte(TxtFile.commands[158]);

                if (it.ObjectName.Contains("SKILL"))
                    foreach (s_data sd in Data.SkillBase)
                    {
                        if (sd != null)
                            if (sd.Name == it.ObjectName)
                            {
                                it.SkillID = sd.ID;
                                break;
                            }
                    }
                Data.ItemBase[it.ID] = it;
                #endregion
                //Item slot types all ot
                #region Item Slot Types
                if (it.Name.Contains("SHIELD") && it.Name.Contains("EU") && !it.Name.Contains("ETC"))
                    it.Itemtype = item_database.ItemType.EU_SHIELD;
                else if (it.Name.Contains("SHIELD") && it.Name.Contains("CH") && !it.Name.Contains("ETC"))
                    it.Itemtype = item_database.ItemType.CH_SHIELD;
                else if (it.Class_B == 4 && it.Class_C == 1 && it.Class_D == 3)
                    it.Itemtype = item_database.ItemType.ARROW;
                else if (it.Class_B == 4 && it.Class_C == 2 && it.Class_D == 3)
                    it.Itemtype = item_database.ItemType.BOLT;
                else if (it.Class_B == 1 && it.Class_C == 1 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.HAT;
                else if (it.Class_B == 3 && it.Class_C == 2 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.SHOULDER;
                else if (it.Class_B == 3 && it.Class_C == 3 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.SUIT;
                else if (it.Class_B == 3 && it.Class_C == 4 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.TROUSERS;
                else if (it.Class_B == 3 && it.Class_C == 5 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.HANDS;
                else if (it.Class_B == 3 && it.Class_C == 6 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.SHOES;
                else if (it.Class_B == 6 && it.Class_C == 7 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.EU_SWORD;
                else if (it.Class_B == 6 && it.Class_C == 8 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.EU_TSWORD;
                else if (it.Class_B == 6 && it.Class_C == 9 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.EU_AXE;
                else if (it.Class_B == 6 && it.Class_C == 10 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.EU_DARKSTAFF;
                else if (it.Class_B == 6 && it.Class_C == 11 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.EU_TSTAFF;
                else if (it.Class_B == 6 && it.Class_C == 12 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.EU_CROSSBOW;
                else if (it.Class_B == 6 && it.Class_C == 13 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.EU_DAGGER;
                else if (it.Class_B == 6 && it.Class_C == 14 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.EU_HARP;
                else if (it.Class_B == 6 && it.Class_C == 15 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.EU_STAFF;
                else if (it.Class_B == 5 && it.Class_C == 1 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.EARRING;
                else if (it.Class_B == 5 && it.Class_C == 3 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.RING;
                else if (it.Class_B == 5 && it.Class_C == 2 && it.Class_D == 1)
                    it.Itemtype = item_database.ItemType.NECKLACE;
                else if (it.Class_A == 3 && it.Class_C == 2 && it.Class_D == 1 && it.Class_B == 6)
                    it.Itemtype = item_database.ItemType.SWORD;
                else if (it.Class_A == 3 && it.Class_C == 3 && it.Class_D == 1 && it.Class_B == 6)
                    it.Itemtype = item_database.ItemType.BLADE;
                else if (it.Class_A == 3 && it.Class_C == 4 && it.Class_D == 1 && it.Class_B == 6)
                    it.Itemtype = item_database.ItemType.SPEAR;
                else if (it.Class_A == 3 && it.Class_C == 5 && it.Class_D == 1 && it.Class_B == 6)
                    it.Itemtype = item_database.ItemType.GLAVIE;
                else if (it.Class_A == 3 && it.Class_C == 6 && it.Class_D == 1 && it.Class_B == 6)
                    it.Itemtype = item_database.ItemType.BOW;
                #endregion
                //Item armor type:
                #region Item Armor Types
                if (it.Name.Contains("LIGHT") && it.Name.Contains("EU"))
                    it.Type = item_database.ArmorType.LIGHT;
                else if (it.Name.Contains("LIGHT") && it.Name.Contains("CH"))
                    it.Type = item_database.ArmorType.PROTECTOR;
                else if (it.Name.Contains("HEAVY") && it.Name.Contains("EU"))
                    it.Type = item_database.ArmorType.HEAVY;
                else if (it.Name.Contains("HEAVY") && it.Name.Contains("CH"))
                    it.Type = item_database.ArmorType.ARMOR;
                else if (it.Name.Contains("CLOTHES") && it.Name.Contains("EU"))
                    it.Type = item_database.ArmorType.ROBE;
                else if (it.Name.Contains("CLOTHES") && it.Name.Contains("CH"))
                    it.Type = item_database.ArmorType.GARMENT;
                else if (it.Name.Contains("C_SUPER"))
                    it.Type = item_database.ArmorType.GM;
                #endregion
                //Jewelry
                #region Jewelry
                else if (it.Name.Contains("_RING_") && !it.Name.Contains("ETC"))
                    it.Itemtype = Global.item_database.ItemType.RING;
                else if (it.Name.Contains("_EARRING_") && !it.Name.Contains("ETC"))
                    it.Itemtype = Global.item_database.ItemType.EARRING;
                else if (it.Name.Contains("_NECKLACE_") && !it.Name.Contains("ETC"))
                    it.Itemtype = Global.item_database.ItemType.NECKLACE;
                #endregion
                //Grabpets
                #region Grabpet
                else if (it.Name.Contains("COS_P") && it.Class_A == 3 && it.Class_B == 1 && it.Class_C == 2 && it.Class_D == 2)
                    it.Pettype = item_database.PetType.GRABPET;
                #endregion
                //Attackpets
                #region Attack pets
                else if (it.Name.Contains("COS_P") && it.Class_A == 3 && it.Class_B == 1 && it.Class_C == 1 && it.Class_D == 2)
                    it.Pettype = item_database.PetType.ATTACKPET;
                #endregion
                //Transport horses
                #region Transport horses
                else if (it.Name.Contains("COS_T"))
                    it.Pettype = item_database.PetType.JOBTRANSPORT;
                #endregion
                //Normal horses
                #region Transport horses
                else if (it.Name.Contains("COS_C"))
                    it.Pettype = item_database.PetType.TRANSPORT;
                #endregion
                //Quest items , define more later.
                #region Quest Items
                else if (it.Name.Contains("QNQ"))
                    it.Questtype = item_database.QuestType.QUEST;
                #endregion
                //Avatars
                #region Avatar related
                #region Avatars
                else if (it.Name.Contains("AVATAR") && it.Name.Contains("AVATAR"))
                    it.Type = item_database.ArmorType.AVATAR;
                #endregion
                //Avatars Attach
                #region Avatars Attach
                else if (it.Name.Contains("AVATAR") && it.Name.Contains("ATTACH"))
                    it.Type = item_database.ArmorType.AVATARATTACH;
                #endregion
                //Avatars Hat
                #region Avatars Hat
                else if (it.Name.Contains("AVATAR") && it.Name.Contains("HAT"))
                    it.Type = item_database.ArmorType.AVATARHAT;
                #endregion
                #endregion
                //Potions
                #region Potions
                #region Normal potions
                else if (it.Name.Contains("CANDY") && it.Name.Contains("RED"))
                    it.Etctype = item_database.EtcType.HP_POTION;
                else if (it.Name.Contains("POTION") && it.Name.Contains("HP") && it.Class_C == 1 && it.Item_Mall_Type != 2)
                    it.Etctype = item_database.EtcType.HP_POTION;
                else if (it.Name.Contains("CANDY") && it.Name.Contains("BLUE"))
                    it.Etctype = item_database.EtcType.MP_POTION;
                else if (it.Name.Contains("POTION") && it.Name.Contains("MP") && it.Class_C == 2 && it.Item_Mall_Type != 2)
                    it.Etctype = item_database.EtcType.MP_POTION;
                else if (it.Name.Contains("CANDY") && it.Name.Contains("VIOLET"))
                    it.Etctype = item_database.EtcType.VIGOR_POTION;
                else if (it.Name.Contains("ITEM_ETC_ALL_POTION"))
                    it.Etctype = item_database.EtcType.VIGOR_POTION;

                #endregion
                #region HP / MP Changing potions
                else if (it.Name.Contains("HP_INC"))
                    it.Etctype = item_database.EtcType.HPSTATPOTION;
                else if (it.Name.Contains("MP_INC"))
                    it.Etctype = item_database.EtcType.MPSTATPOTION;
                #endregion
                #region Speed Potions
                else if (it.Name.Contains("POTION_SPEED"))
                    it.Etctype = item_database.EtcType.SPEED_POTION;
                #endregion
                #region Berserk potions
                else if (it.Name.Contains("HWAN_POTION"))
                    it.Etctype = Global.item_database.EtcType.BERSERKPOTION;
                #endregion
                #endregion
                //Tickets all types
                #region Silver Skill Tickets
                else if (it.Name.Contains("TICKET") && it.Name.Contains("SILVER") && it.Name.Contains("1D") && it.Name.Contains("SKILL"))
                    it.Ticket = item_database.Tickets.SKILL_SILVER_1_DAY;
                else if (it.Name.Contains("TICKET") && it.Name.Contains("SILVER") && it.Name.Contains("4W") && it.Name.Contains("SKILL"))
                    it.Ticket = item_database.Tickets.SKILL_SILVER_4_WEEKS;
                else if (it.Name.Contains("TICKET") && it.Name.Contains("SILVER") && it.Name.Contains("8W") && it.Name.Contains("SKILL"))
                    it.Ticket = item_database.Tickets.SKILL_SILVER_8_WEEKS;
                else if (it.Name.Contains("TICKET") && it.Name.Contains("SILVER") && it.Name.Contains("12W") && it.Name.Contains("SKILL"))
                    it.Ticket = item_database.Tickets.SKILL_SILVER_12_WEEKS;
                else if (it.Name.Contains("TICKET") && it.Name.Contains("SILVER") && it.Name.Contains("16W") && it.Name.Contains("SKILL"))
                    it.Ticket = item_database.Tickets.SKILL_SILVER_16_WEEKS;
                #endregion
                #region Gold Skill Tickets
                else if (it.Name.Contains("TICKET") && it.Name.Contains("GOLD") && it.Name.Contains("1D") && it.Name.Contains("SKILL"))
                    it.Ticket = item_database.Tickets.SKILL_GOLD_1_DAY;
                else if (it.Name.Contains("TICKET") && it.Name.Contains("GOLD") && it.Name.Contains("4W") && it.Name.Contains("SKILL"))
                    it.Ticket = item_database.Tickets.SKILL_GOLD_4_WEEKS;
                else if (it.Name.Contains("TICKET") && it.Name.Contains("GOLD") && it.Name.Contains("8W") && it.Name.Contains("SKILL"))
                    it.Ticket = item_database.Tickets.SKILL_GOLD_8_WEEKS;
                else if (it.Name.Contains("TICKET") && it.Name.Contains("GOLD") && it.Name.Contains("12W") && it.Name.Contains("SKILL"))
                    it.Ticket = item_database.Tickets.SKILL_GOLD_12_WEEKS;
                else if (it.Name.Contains("TICKET") && it.Name.Contains("GOLD") && it.Name.Contains("16W") && it.Name.Contains("SKILL"))
                    it.Ticket = item_database.Tickets.SKILL_GOLD_16_WEEKS;
                #endregion
                #region Silver Tickets
                else if (it.Name.Contains("MALL_SILVER_TIME") && it.Name.Contains("1D"))
                    it.Ticket = item_database.Tickets.SILVER_1_DAY;
                else if (it.Name.Contains("MALL_SILVER_TIME") && it.Name.Contains("4W"))
                    it.Ticket = item_database.Tickets.SILVER_4_WEEKS;
                else if (it.Name.Contains("MALL_SILVER_TIME") && it.Name.Contains("8W"))
                    it.Ticket = item_database.Tickets.SILVER_8_WEEKS;
                else if (it.Name.Contains("MALL_SILVER_TIME") && it.Name.Contains("12W"))
                    it.Ticket = item_database.Tickets.SILVER_12_WEEKS;
                else if (it.Name.Contains("MALL_SILVER_TIME") && it.Name.Contains("16W"))
                    it.Ticket = item_database.Tickets.SILVER_16_WEEKS;
                #endregion
                #region Gold Tickets
                else if (it.Name.Contains("MALL_GOLD_TIME") && it.Name.Contains("1D"))
                    it.Ticket = item_database.Tickets.GOLD_1_DAY;
                else if (it.Name.Contains("MALL_GOLD_TIME") && it.Name.Contains("4W"))
                    it.Ticket = item_database.Tickets.GOLD_4_WEEKS;
                else if (it.Name.Contains("MALL_GOLD_TIME") && it.Name.Contains("8W"))
                    it.Ticket = item_database.Tickets.GOLD_8_WEEKS;
                else if (it.Name.Contains("MALL_GOLD_TIME") && it.Name.Contains("12W"))
                    it.Ticket = item_database.Tickets.GOLD_12_WEEKS;
                else if (it.Name.Contains("MALL_GOLD_TIME") && it.Name.Contains("16W"))
                    it.Ticket = item_database.Tickets.GOLD_16_WEEKS;
                #endregion
                #region Premium Quest Tickets
                else if (it.Name.Contains("TICKET") && it.Name.Contains("PREM"))
                    it.Ticket = item_database.Tickets.PREMIUM_QUEST_TICKET;
                #endregion
                #region Open Market
                else if (it.Name.Contains("OPEN_MARKET"))
                    it.Ticket = item_database.Tickets.OPEN_MARKET;
                #endregion
                #region Dungeon tickets
                else if (it.Name.Contains("TICKET") && it.Name.Contains("EGYPT"))
                    it.Ticket = item_database.Tickets.DUNGEON_EGYPT;
                else if (it.Name.Contains("TICKET") && it.Name.Contains("FORGOTTEN"))
                    it.Ticket = item_database.Tickets.DUNGEON_FORGOTTEN_WORLD;
                else if (it.Name.Contains("TICKET") && it.Name.Contains("BATTLE_ARENA"))
                    it.Ticket = item_database.Tickets.BATTLE_ARENA;
                else if (it.Name.Contains("ITEM_ETC_TELEPORT_HOLE"))
                    it.Ticket = item_database.Tickets.DUNGEON_FORGOTTEN_WORLD;
                #endregion
                #region Warehouse
                else if (it.Name.Contains("TICKET") && it.Name.Contains("WAREHOUSE"))
                    it.Ticket = item_database.Tickets.WAREHOUSE;
                #endregion
                #region Auto Potion Ticket
                else if (it.Name.Contains("TICKET") && it.Name.Contains("AUTO_POTION"))
                    it.Ticket = item_database.Tickets.AUTO_POTION;
                #endregion
                #region Beginner tickets
                else if (it.Name.Contains("ETC") && it.Name.Contains("_HELP"))
                    it.Ticket = item_database.Tickets.BEGINNER_HELPERS;
                #endregion
                //Chat related items
                #region Global chat
                else if (it.Name.Contains("GLOBAL") && it.Name.Contains("CHAT"))
                    it.Etctype = item_database.EtcType.GLOBALCHAT;
                #endregion
                //Stall decoration
                #region Stall decoration
                else if (it.Name.Contains("BOOTH"))
                    it.Etctype = item_database.EtcType.STALLDECORATION;
                #endregion
                //Monster mask
                #region Monster Masks
                else if (it.Name.Contains("TRANS_MONSTER"))
                    it.Etctype = item_database.EtcType.MONSTERMASK;
                #endregion
                //Elixirs
                #region Elixirs
                else if (it.Name.Contains("ETC") && it.Name.Contains("REINFORCE") && it.Name.Contains("RECIPE") && it.Name.Contains("_B") && it.Class_C == 1)
                    it.Etctype = item_database.EtcType.ELIXIR;
                #endregion
                //Job suits
                #region Job Suits
                //Hunter suits
                else if (it.Name.Contains("TRADE_HUNTER") && it.Name.Contains("CH"))
                    it.Type = item_database.ArmorType.HUNTER;
                else if (it.Name.Contains("TRADE_HUNTER") && it.Name.Contains("EU"))
                    it.Type = item_database.ArmorType.HUNTER;
                //Thief suits
                else if (it.Name.Contains("TRADE_THIEF") && it.Name.Contains("CH"))
                    it.Type = item_database.ArmorType.THIEF;
                else if (it.Name.Contains("TRADE_THIEF") && it.Name.Contains("EU"))
                    it.Type = item_database.ArmorType.THIEF;
                #endregion
                //Return scrolls
                #region Return scrolls
                else if (it.Name.Contains("SCROLL_RETURN"))
                    it.Etctype = item_database.EtcType.RETURNSCROLL;
                #endregion
                //Reverse scrolls
                #region Reverse scrolls
                else if (it.Name.Contains("SCROLL") && it.Name.Contains("REVERSE"))
                    it.Etctype = item_database.EtcType.REVERSESCROLL;
                #endregion
                //Thief scrolls
                #region Thief scrolls
                else if (it.Name.Contains("SCROLL") && it.Name.Contains("THIEF"))
                    it.Etctype = item_database.EtcType.BANDITSCROLL;
                #endregion
                //Summon scrolls
                #region Summon scrolls
                else if (it.Name.Contains("SUMMON") && it.Name.Contains("SCROLL"))
                    it.Etctype = item_database.EtcType.SUMMONSCROLL;
                #endregion
                //Skin change scrolls
                #region Skin change scrolls
                else if (it.Name.Contains("SKIN_CHANGE"))
                    it.Etctype = item_database.EtcType.CHANGESKIN;
                #endregion
                //Inventory expansion
                #region Inventory expansions
                else if (it.Name.Contains("INVENTORY") && it.Name.Contains("ADDITION"))
                    it.Etctype = item_database.EtcType.INVENTORYEXPANSION;
                #endregion
                //Warehouse expansion
                #region Warehouse expansions
                else if (it.Name.Contains("WAREHOUSE") && it.Name.Contains("ADDITION"))
                    it.Etctype = item_database.EtcType.WAREHOUSE;
                #endregion
                //Alchemy materials
                #region Alchemy materials
                else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("MAT"))
                    it.Etctype = item_database.EtcType.ALCHEMY_MATERIAL;
                #endregion
                //Tablets
                #region Tablets
                // later have to differ assimilate stones that has % not like astral and steady
                else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("MAGICTABLET"))
                    it.Itemtype = item_database.ItemType.MAGICTABLET;
                else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("ATTRTABLET"))
                    it.Itemtype = item_database.ItemType.MAGICTABLET;
                #endregion
                //Elements
                #region Elements
                else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("ELEMENT"))
                    it.Etctype = item_database.EtcType.ELEMENTS;
                #endregion
                //Stones (Note: Need to filter it more detailed later to deny some stones).
                #region Stones
                else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("ATTRSTONE") && !it.Name.Contains("AST"))
                    it.Etctype = item_database.EtcType.STONES;
                else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("MAGICSTONE") && !it.Name.Contains("AST"))
                    it.Etctype = item_database.EtcType.STONES;
                else if (it.Name.Contains("ARCHEMY") && it.Name.Contains("MAGICSTONE") && it.Name.Contains("ASTRAL"))
                    it.Etctype = item_database.EtcType.ASTRALSTONE;
                #endregion
                //Destroyer rondo
                #region Destroyer rondo
                else if (it.Name.Contains("ITEM_ETC_ARCHEMY_RONDO_02"))
                    it.Etctype = item_database.EtcType.DESTROYER_RONDO;
                #endregion
                //Gender switch tools
                #region Gender switch
                else if (it.Name.Contains("TRANSGENDER"))
                    it.Etctype = item_database.EtcType.ITEMCHANGETOOL;
                #endregion
                //Guild items
                #region Guild Items
                else if (it.Name.Contains("GUILD_CREST"))
                    it.Etctype = item_database.EtcType.GUILD_ICON;
                #endregion
                //Event items
                #region Event
                else if (it.Name.Contains("TREASURE"))
                    it.Etctype = item_database.EtcType.EVENT;
                #endregion
                //########################################################
                // Silk Prices Definitions
                //########################################################     
                #region Silk pricing
                if (it.Etctype == item_database.EtcType.AVATAR28D ||
                    it.Etctype == item_database.EtcType.CHANGESKIN ||
                    it.Etctype == item_database.EtcType.GLOBALCHAT ||
                    it.Etctype == item_database.EtcType.INVENTORYEXPANSION ||
                    it.Etctype == item_database.EtcType.RETURNSCROLL ||
                    it.Etctype == item_database.EtcType.REVERSESCROLL ||
                    it.Etctype == item_database.EtcType.STALLDECORATION ||
                    it.Etctype == item_database.EtcType.WAREHOUSE ||
                    it.Pettype == item_database.PetType.GRABPET ||
                    it.Type == item_database.ArmorType.AVATAR ||
                    it.Type == item_database.ArmorType.AVATARHAT ||
                    it.Type == item_database.ArmorType.AVATARATTACH)
                    it.Shop_price = Systems.SetSilk(it.ID);
                #endregion
                //########################################################
                // Drop Database
                //########################################################
                //Armors
                #region Armors
                if ((it.Name.Contains("LIGHT") && it.Name.Contains("EU") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER")) ||
                    (it.Name.Contains("LIGHT") && it.Name.Contains("CH") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER")) ||
                    (it.Name.Contains("HEAVY") && it.Name.Contains("EU") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER")) ||
                    (it.Name.Contains("HEAVY") && it.Name.Contains("CH") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER")) ||
                    (it.Name.Contains("CLOTHES") && it.Name.Contains("EU") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER")) ||
                    (it.Name.Contains("CLOTHES") && it.Name.Contains("CH") && !it.Name.Contains("DEF") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("SUPER"))
                    && it.SOX == 0)
                {
                    Data.ArmorDataBase.ID.Add(it.ID);
                }
                #endregion
                //Weapons
                #region Weapons
                if ((it.Name.Contains("ITEM_EU_AXE") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_CROSSBOW") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_DAGGER") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_DARKSTAFF") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_HARP") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_STAFF") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_SWORD") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_TSTAFF") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_TSWORD") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_TBLADE") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_BLADE") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_BOW") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_EU_SHIELD") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_SHIELD") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_SPEAR") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")) ||
                    (it.Name.Contains("ITEM_CH_SWORD") && !it.Name.Contains("_DEF") && !it.Name.Contains("RARE") && !it.Name.Contains("SET")))
                {
                    Data.WeaponDataBase.ID.Add(it.ID);
                }
                #endregion
                //Jewelerys
                #region Jewelerys
                if ((it.Name.Contains("RING") && !it.Name.Contains("DEF")) && (it.Name.Contains("ITEM_CH") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("BASIC")) ||
                    (it.Name.Contains("NECKLACE") && !it.Name.Contains("DEF")) && (it.Name.Contains("ITEM_CH") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("BASIC")) ||
                    (it.Name.Contains("NECKLACE") && !it.Name.Contains("DEF")) && (it.Name.Contains("ITEM_EU") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("BASIC")) ||
                    (it.Name.Contains("RING") && !it.Name.Contains("DEF")) && (it.Name.Contains("ITEM_EU") && !it.Name.Contains("RARE") && !it.Name.Contains("SET") && !it.Name.Contains("BASIC")))
                {
                    Data.JewelDataBase.ID.Add(it.ID);
                }
                #endregion
                //Seal items
                #region Seal items
                if (it.SOX == 2 && !it.Name.Contains("_SET") && !it.Name.Contains("_HONOR") && !it.Name.Contains("EVENT") && !it.Name.Contains("ETC") && !it.Name.Contains("ROC") && !it.Name.Contains("BASIC") && !it.Name.Contains("DEF"))
                {
                    Data.SoxDataBase.ID.Add(it.ID);
                }
                #endregion
                //Stones
                #region Stones
                else if (it.Itemtype == Global.item_database.ItemType.MAGICTABLET && !it.Name.Contains("ASTRAL") || it.Itemtype == Global.item_database.ItemType.ATTRTABLET && !it.Name.Contains("ASTRAL"))
                {
                    Data.StoneDataBase.ID.Add(it.ID);
                }
                #endregion
                //Alchemy materials
                #region Alchemy materials
                else if (it.Etctype == Global.item_database.EtcType.ALCHEMY_MATERIAL)
                {
                    Data.MaterialDataBase.ID.Add(it.ID);
                }
                #endregion
                //Elixirs
                #region Elixirs
                else if (it.Etctype == Global.item_database.EtcType.ELIXIR)
                {
                    Data.ElixirDataBase.ID.Add(it.ID);
                }
                #endregion
                //Soulbound
                #region Soulbound information
                //Tmp
                if (it.Name.Contains("PRE_MALL"))
                    it.Accountbound = 0;

                #endregion
                //Distances
                #region Distances
                if (it.Itemtype == item_database.ItemType.SWORD || it.Itemtype == item_database.ItemType.EU_SWORD)
                    it.ATTACK_DISTANCE = 1;
                else if (it.Itemtype == item_database.ItemType.SPEAR)
                    it.ATTACK_DISTANCE = 2;
                else if (it.Itemtype == item_database.ItemType.GLAVIE)
                    it.ATTACK_DISTANCE = 2;
                else if (it.Itemtype == item_database.ItemType.BOW || it.Itemtype == item_database.ItemType.EU_CROSSBOW)
                    it.ATTACK_DISTANCE = 8;
                #endregion
                //Race fixes
                #region Race fixes
                if (it.Name.Contains("_EU_") || it.Name.Contains("BOLT"))
                    it.Race = 1;
                else if (it.Name.Contains("_CH_") || it.Name.Contains("ARROW") || it.Name.Contains("QUIVER"))
                    it.Race = 0;
                #endregion
                //1 damage weapon fixes.
                #region Weapon fixes
                if (it.Name.Contains("DEF") && it.Name.Contains("STAFF"))
                {
                    it.Attack.Min_HPhyAttack = it.Attack.Min_HMagAttack;
                    it.Attack.Min_LPhyAttack = it.Attack.Min_LMagAttack;
                }
                #endregion
            }
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " items");
        }
    }
}
