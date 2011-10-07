///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace DarkEmu_GameServer.Global
{
    public static class Versions
    {
        public static string appVersion = "0.0.1";
        public static int clientVersion = 322;
    }
    public static class Product
    {
        public static string Productname = "SRXTREME";
        public static string Homepage = "http://www.xfsgames.com.ar";
        public static string Prefix = "[SRX]: ";
    }
    public sealed class trader_data
    {
        public int Amount;
        public int Stars;
        public stars Details;
        public enum stars
        {
            ONESTAR     = 1,
            TWOSTARS    = 2,
            THREESTARS  = 3,
            FOURSTARS   = 4,
            FIVESTARS   = 5
        }
    }
    public struct guild_player
    {
        public int MemberID, Model, DonateGP;
        public string Name, GrantName;
        public byte Level, FWrank, Rank;
        public byte Xsector, Ysector;
        public bool Online;
        public bool joinRight, withdrawRight, unionRight, guildstorageRight, noticeeditRight;
    }
    
    public sealed class drop_database
    {
        public List<int> ID;
        Random rnd;

        public drop_database()
        {
            rnd = new Random();
            ID = new List<int>();
        }
        public byte GetQuantity(byte mobType, string itemType)
        {
            byte Quantity = 0;
            switch (itemType)
            {
                case "armors":
                case "weapons":
                case "jewelery":
                case "sox":
                case "tablets":
                case "elixir":
                case "potions":
                case "scrolls":
                case "alchemymaterial":
                case "event":
                    Quantity = 1;
                    break;
                case "arrows":
                    if (mobType == 4 && (rnd.Next(0, 300) < 7 * Systems.Rate.ETCd)) Quantity = Convert.ToByte(150 / rnd.Next(1, 2));
                    if (mobType == 3 && (rnd.Next(0, 300) < 300 * Systems.Rate.ETCd)) Quantity = Convert.ToByte(200 / rnd.Next(1, 3));
                    if (mobType == 1 && (rnd.Next(0, 300) < 5 * Systems.Rate.ETCd)) Quantity = Convert.ToByte(100 / rnd.Next(1, 2));
                    if (mobType == 0 && (rnd.Next(0, 300) < 3 * Systems.Rate.ETCd)) Quantity = 50;
                    if (Quantity == 1) Quantity = 50;
                    break;
            }
            return Quantity;
        }
        public int GetAmount(byte mobType, string itemType)
        {
            byte Amountinfo = 0;
            switch (itemType)
            {
                case "sox"://Seal drops (Should be defined per seal type).
                    #region Seal Items
                    if (mobType == 4 && (rnd.Next(0, 400) < 3 * Systems.Rate.Sox)) Amountinfo = 1;
                    if (mobType == 3 && (rnd.Next(0, 400) < 5 * Systems.Rate.Sox)) Amountinfo = Convert.ToByte(rnd.Next(1, 3));
                    if (mobType == 1 && (rnd.Next(0, 400) < 2 * Systems.Rate.Sox)) Amountinfo = 1;
                    if (mobType == 0 && (rnd.Next(0, 400) < 2 * Systems.Rate.Sox)) Amountinfo = 1;
                    #endregion
                    break;
                case "tablets"://Tablets (Defined per degree / Level drop).
                    #region Alchemy Tablets
                    if (mobType == 4 && (rnd.Next(0, 300) < 7 * Systems.Rate.Alchemyd)) Amountinfo = Convert.ToByte(rnd.Next(1, 3));
                    if (mobType == 3 && (rnd.Next(0, 300) < 300 * Systems.Rate.Alchemyd)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 1 && (rnd.Next(0, 300) < 5 * Systems.Rate.Alchemyd)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 0 && (rnd.Next(0, 300) < 3 * Systems.Rate.Alchemyd)) Amountinfo = 1;
                    #endregion
                    break;
                case "elixir"://Elixir drops speak for itself.
                    #region Elixirs
                    if (mobType == 4 && (rnd.Next(0, 300) < 7 * Systems.Rate.Alchemyd)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 3 && (rnd.Next(0, 300) < 300 * Systems.Rate.Alchemyd)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 1 && (rnd.Next(0, 300) < 5 * Systems.Rate.Alchemyd)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 0 && (rnd.Next(0, 300) < 3 * Systems.Rate.Alchemyd)) Amountinfo = 1;
                    #endregion
                    break;
                case "alchemymaterial"://Etc drops would contains (Potions, Arrows, Material etc).
                    //Need to specify event rates later
                case "event":
                    #region Alchemy Materials
                    if (mobType == 4 && (rnd.Next(0, 300) < 7 * Systems.Rate.Alchemyd)) Amountinfo = Convert.ToByte(rnd.Next(2, 4));
                    if (mobType == 3 && (rnd.Next(0, 300) < 300 * Systems.Rate.Alchemyd)) Amountinfo = Convert.ToByte(rnd.Next(4, 6));
                    if (mobType == 1 && (rnd.Next(0, 300) < 5 * Systems.Rate.Alchemyd)) Amountinfo = Convert.ToByte(rnd.Next(1, 3));
                    if (mobType == 0 && (rnd.Next(0, 300) < 3 * Systems.Rate.Alchemyd)) Amountinfo = 1;
                    #endregion
                    break;
                case "arrows"://This contains bolts and arrows).
                    #region Ammos
                    if (mobType == 4 && (rnd.Next(0, 300) < 7 * Systems.Rate.ETCd)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 3 && (rnd.Next(0, 300) < 300 * Systems.Rate.ETCd)) Amountinfo = Convert.ToByte(rnd.Next(1, 3));
                    if (mobType == 1 && (rnd.Next(0, 300) < 5 * Systems.Rate.ETCd)) Amountinfo = 1;
                    if (mobType == 0 && (rnd.Next(0, 300) < 3 * Systems.Rate.ETCd)) Amountinfo = 1;
                    #endregion
                    break;
                case "potions"://Potions (Enough said).
                    #region Potions
                    if (mobType == 4 && (rnd.Next(0, 300) < 7 * Systems.Rate.ETCd)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 3 && (rnd.Next(0, 300) < 300 * Systems.Rate.ETCd)) Amountinfo = Convert.ToByte(rnd.Next(4, 9));
                    if (mobType == 1 && (rnd.Next(0, 300) < 5 * Systems.Rate.ETCd)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 0 && (rnd.Next(0, 300) < 3 * Systems.Rate.ETCd)) Amountinfo = 1;
                    #endregion
                    break;
                case "event_items"://Event items (Letters, scrolls etc Should add in config later for event handler.).
                    break;
                case "quest_items"://Quest items (Will be called from the quest active list (id of drops).
                    break;
                case "scrolls"://Return scrolls (And related to that).
                    #region return scrolls
                    if (mobType == 4 && (rnd.Next(0, 300) < 7 * Systems.Rate.ETCd)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 3 && (rnd.Next(0, 300) < 300 * Systems.Rate.ETCd)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 1 && (rnd.Next(0, 300) < 5 * Systems.Rate.ETCd)) Amountinfo = 1;
                    if (mobType == 0 && (rnd.Next(0, 300) < 3 * Systems.Rate.ETCd)) Amountinfo = 1;
                    #endregion
                    break;
                case "jewelery":
                case "weapons"://Weapon drops(speaks for itself).
                case "armors"://Armor drops (Garm, Prot etc).
                    #region Normal Items
                    if (mobType == 4 && (rnd.Next(0, 300) < 7 * Systems.Rate.Item)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 3 && (rnd.Next(0, 300) < 300 * Systems.Rate.Item)) Amountinfo = Convert.ToByte(rnd.Next(1, 2));
                    if (mobType == 1 && (rnd.Next(0, 300) < 5 * Systems.Rate.Item)) Amountinfo = 1;
                    if (mobType == 0 && (rnd.Next(0, 300) < 3 * Systems.Rate.Item)) Amountinfo = 1;
                    #endregion
                    break;
            }
            return Amountinfo;
        }
        public byte GetSpawnType(string itemType)
        {
            byte type = 0;
            switch (itemType)
            { 
                case "tablets":
                case "elixir":
                case "potions":
                case "scrolls":
                case "alchemymaterial":
                case "event":
                    type = 3;
                    break;
                case "armors":
                case "weapons":
                case "jewelery":
                case "sox":
                    type = 2;
                    break;
                case "arrows":
                    type = 3;
                    break;
            }
            return type;
        }
        public int GetDrop(int moblevel, int mobID, string itemType, int filterRace)
        {
            List<int> filter = new List<int>();
            switch (itemType)
            {
                case "tablets"://Tablets (Defined per degree / Level drop).
                    #region tablets
                    if (Data.ObjectBase[mobID].Level < 8)
                    {
                        filter = ID.FindAll(item => Data.ItemBase[item].Degree == 1);
                    }
                    else if (Data.ObjectBase[mobID].Level < 16 && Data.ObjectBase[mobID].Level >= 8)
                    {
                        filter = ID.FindAll(item => Data.ItemBase[item].Degree == 2);
                    }
                    else if (Data.ObjectBase[mobID].Level < 24 && Data.ObjectBase[mobID].Level >= 16)
                    {
                        filter = ID.FindAll(item => Data.ItemBase[item].Degree == 3);
                    }
                    else if (Data.ObjectBase[mobID].Level < 32 && Data.ObjectBase[mobID].Level >= 24)
                    {
                        filter = ID.FindAll(item => Data.ItemBase[item].Degree == 4);
                    }
                    else if (Data.ObjectBase[mobID].Level < 42 && Data.ObjectBase[mobID].Level >= 32)
                    {
                        filter = ID.FindAll(item => Data.ItemBase[item].Degree == 5);
                    }
                    else if (Data.ObjectBase[mobID].Level < 52 && Data.ObjectBase[mobID].Level >= 42)
                    {
                        filter = ID.FindAll(item => Data.ItemBase[item].Degree == 6);
                    }
                    else if (Data.ObjectBase[mobID].Level < 64 && Data.ObjectBase[mobID].Level >= 52)
                    {
                        filter = ID.FindAll(item => Data.ItemBase[item].Degree == 7);
                    }
                    else if (Data.ObjectBase[mobID].Level < 76 && Data.ObjectBase[mobID].Level >= 64)
                    {
                        filter = ID.FindAll(item => Data.ItemBase[item].Degree == 8);
                    }
                    else if (Data.ObjectBase[mobID].Level < 90 && Data.ObjectBase[mobID].Level >= 76)
                    {
                        filter = ID.FindAll(item => Data.ItemBase[item].Degree == 9);
                    }
                    else if (Data.ObjectBase[mobID].Level < 101 && Data.ObjectBase[mobID].Level >= 90)
                    {
                        filter = ID.FindAll(item => Data.ItemBase[item].Degree == 10);
                    }
                    else if (Data.ObjectBase[mobID].Level > 101)
                    {
                        filter = ID.FindAll(item => Data.ItemBase[item].Degree == 11);
                    }
                    #endregion
                    break;
                case "elixir"://Elixir drops speak for itself.
                    #region elixir
                    filter = ID;
                    #endregion  
                    break;
                case "alchemymaterial"://Etc drops would contains (Potions, Arrows, Material etc).
                case "event":
                    #region Alchemy materials
                    string truncatedMobName = Data.ObjectBase[mobID].Name.Substring(Data.ObjectBase[mobID].Name.IndexOf("_")+1); 
                    filter = ID.FindAll(item => Data.ItemBase[item].Name.Contains(truncatedMobName) );
                    #endregion
                    break;
                case "arrows"://This contains bolts and arrows (Define eu/ch happens before).
                    #region Arrows
                    if (Data.ObjectBase[mobID].Type == 1)
                    {
                        filter.Add(62); // arrow
                    }
                    else
                    {
                        filter.Add(10376); // bolt
                    }
                    #endregion
                    break;
                case "potions"://Potions (+unipills)
                    #region Potions
                    filter.Add(9); 
                    filter.Add(16);
                    filter.Add(23);

                    if (Data.ObjectBase[mobID].Level < 20)
                    {
                        filter.Add(4);
                        filter.Add(11);
                        filter.Add(18);
                        filter.Add(55);
                    }
                    else if (Data.ObjectBase[mobID].Level < 40 && Data.ObjectBase[mobID].Level >= 20)
                    {
                        filter.Add(5);
                        filter.Add(12);
                        filter.Add(19);
                        filter.Add(56);
                    }
                    else if (Data.ObjectBase[mobID].Level < 60 && Data.ObjectBase[mobID].Level >= 40)
                    {
                        filter.Add(6); 
                        filter.Add(13);
                        filter.Add(20);
                        filter.Add(57);
                    }
                    else if (Data.ObjectBase[mobID].Level < 80 && Data.ObjectBase[mobID].Level >= 60)
                    {
                        filter.Add(7);
                        filter.Add(14);
                        filter.Add(21);
                        filter.Add(58);
                    }
                    else if (Data.ObjectBase[mobID].Level >= 80)
                    {
                        filter.Add(8);
                        filter.Add(15);
                        filter.Add(22);
                        filter.Add(59);
                    }
                    #endregion
                    break;
                case "event_items"://Event items (Letters, scrolls etc Should add in config later for event handler.)
                    #region Event items
                    // todo: event system 
                    #endregion
                    break;
                case "quest_items"://Quest items (Will be called from the quest active list (id of drops).
                    #region Quest related items
                    // todo: quest system 
                    #endregion
                    break;
                case "scrolls"://Return scrolls (And related to that).
                    #region Scrolls
                    filter.Add(61);
                    #endregion
                    break;
                case "weapons"://Weapon drops(speaks for itself).
                case "armors"://Armor drops (Garm, Prot etc).
                case "sox"://Seal drops (Should be defined per seal type).
                case "jewelery":
                    #region Normal wearable items
                    List<int> LevelFilter = ID.FindAll(item => Data.ItemBase[item].Level >= Data.ObjectBase[mobID].Level - 4 && Data.ItemBase[item].Level <= Data.ObjectBase[mobID].Level + 4);
                    filter = LevelFilter.FindAll(item => Data.ItemBase[item].Race == filterRace);
                    #endregion
                    break;
            }
            if (filter.Count <= 0)
                return -1;
            else
                return filter[rnd.Next(0, filter.Count-1)];         
        }
    }
    public sealed class quest_database
    {
        public string Questname, Rewardname, QuestNPC;
        public int Questid, Rewardid, QuestNpcID, QuestLevel;
        public enum requirments
        {
            LEVEL,
            RACE,
            ITEMS
        }
    }
    public sealed class item_database
    {
        public static int GetItem(string name)
        {
            for (int i = 0; i < Data.ItemBase.Length; i++)
            {
                if (Data.ItemBase[i] != null && Data.ItemBase[i].Name == name) return i;
            }
            return 0;
        }

        public int ID;
        public byte Level, SOX, Gender, Race, SoulBound, SealType,MaxBlueAmount, Degree, Accountbound, Tradable;
        public int Class_B, Class_C, Class_A, Shop_price, Item_Mall_Type, Max_Stack, Use_Time, Use_Time2, Sell_Price, Class_D, Storage_price, Armorinfo, ItemMallType, SkillID, EARTH_ELEMENTS_AMOUNT_REQ, WATER_ELEMENTS_AMOUNT_REQ, FIRE_ELEMENTS_AMOUNT_REQ, WIND_ELEMENTS_AMOUNT_REQ;
        public double ATTACK_DISTANCE;
        public string Name, ObjectName, StoneName, EARTH_ELEMENTS_NAME, WATER_ELEMENTS_NAME, FIRE_ELEMENTS_NAME, WIND_ELEMENTS_NAME;
        public bool needEquip;
        public ItemType Itemtype;
        public enum ItemType {WEARABLE, CH_SHIELD, EU_SHIELD, AMMO, HAT, TROUSERS, HANDS, SHOES, SUIT, SHOULDER, COS, EARRING, RING, NECKLACE, SWORD, BLADE, GLAVIE, SPEAR, BOW, TRADERSUIT, HUNTERSUIT, THIEFSUIT, ALCHEMY, AVATAR, EVENT, EU_SWORD, EU_TSWORD, EU_AXE, EU_DARKSTAFF, EU_TSTAFF, EU_CROSSBOW, EU_DAGGER, EU_HARP, EU_STAFF, FORTRESS, MAGICSTONE, ATTRSTONE, MAGICTABLET, ATTRTABLET, ARROW, BOLT };
        public ArmorType Type;
        public enum ArmorType { NULL, ARMOR, PROTECTOR, GARMENT, ROBE, HEAVY, LIGHT, GM, AVATAR, AVATARATTACH, AVATARHAT, THIEF, HUNTER};
        public EtcType Etctype;
        public enum EtcType { NULL, ITEMMALL, HP_POTION, MP_POTION, VIGOR_POTION, STALLDECORATION, MONSTERMASK, ELIXIR, RETURNSCROLL, REVERSESCROLL, BANDITSCROLL, SUMMONSCROLL, INVENTORYEXPANSION ,GLOBALCHAT,
                              WAREHOUSE, CHANGESKIN, HPSTATPOTION, MPSTATPOTION,BERSERKPOTION, AVATAR28D, SPEED_POTION, ALCHEMY_MATERIAL, ELEMENTS, DESTROYER_RONDO,VOID_RONDO,ITEMCHANGETOOL, STONES, ASTRALSTONE,
                              GUILD_ICON, EVENT
                            };
        public PetType Pettype;
        public enum PetType { NULL, GRABPET, ATTACKPET, TRANSPORT, JOBTRANSPORT };
        public QuestType Questtype;
        public enum QuestType { QUEST };
        public Tickets Ticket;
        public enum Tickets {   SILVER_1_DAY, SILVER_4_WEEKS, SILVER_8_WEEKS, SILVER_12_WEEKS, SILVER_16_WEEKS,
                                GOLD_1_DAY, GOLD_4_WEEKS, GOLD_8_WEEKS, GOLD_12_WEEKS, GOLD_16_WEEKS,
                                SKILL_SILVER_1_DAY, SKILL_SILVER_4_WEEKS, SKILL_SILVER_8_WEEKS, SKILL_SILVER_12_WEEKS, SKILL_SILVER_16_WEEKS,
                                SKILL_GOLD_1_DAY, SKILL_GOLD_4_WEEKS, SKILL_GOLD_8_WEEKS, SKILL_GOLD_12_WEEKS, SKILL_GOLD_16_WEEKS,
                                BEGINNER_HELPERS,
                                PREMIUM_QUEST_TICKET,
                                OPEN_MARKET,
                                DUNGEON_EGYPT,
                                DUNGEON_FORGOTTEN_WORLD,
                                BATTLE_ARENA,
                                WAREHOUSE,
                                AUTO_POTION
                             }; 
        attack_items attack = new attack_items();
        public attack_items Attack { get { return attack; } }

        def_items def = new def_items();
        public def_items Defans { get { return def; } }
    }
    public sealed class attack_items
    {
        public double Min_LPhyAttack, Min_HPhyAttack, PhyAttackInc, Min_LMagAttack, Min_HMagAttack, MagAttackINC, MinAttackRating, MaxAttackRating;
        public byte MinCrit, MaxCrit;
    }
    public sealed class def_items
    {
        public double MinMagDef, MagDefINC, MinPhyDef, PhyDefINC;
        public double PhyAbsorb, MagAbsorb, AbsorbINC, Durability, Parry;
        public byte MinBlock, MaxBlock;
    }
    public sealed class objectdata
    {
        public static int GetItem(string name)
        {
            for (int i = 0; i < Data.ObjectBase.Length; i++)
            {
                if (Data.ObjectBase[i] != null && Data.ObjectBase[i].Name == name) return i;
            }
            return 0;
        }
        public int ID;
        public string Name;
        public int HP, Exp;
        public int[] Skill = new int[9];
        public byte amountSkill;
        public int MagDef, PhyDef, ParryRatio, HitRatio;
        public byte Agresif, Type, ObjectType, Level, Race;
        public float SpeedWalk, SpeedRun, SpeedZerk;
        public string[] Shop = new string[10];
        public string[] Tab = new string[30];
        public string StoreName;
        public float Speed1, Speed2;

        //Public type
        public NamdedType Object_type;
        //Define types as named enum's
        public enum NamdedType
        {
            //NPC Object
            NPC,
            //Player Object
            PLAYER,
            //Monster Object
            MONSTER,
            MONSTERUNIQUE,
            //Job object
            JOBMONSTER,
            //Event monster
            EVENTMONSTER,
            //Fortress war monster
            FORTRESSWARMONSTER,
            //Pet Object
            GRABPET,
            ATTACKPET,
            NORMALTRANSPORT,
            JOBTRANSPORT,
            //Structures
            STRUCTURE,
            //Teleporters
            TELEPORT
        }
    }
    public sealed class vektor
    {
        public float x, y, z;
        public byte xSec, ySec;
        public int ID;

        public vektor()
        {
            this.x = 0;
            this.z = 0;
            this.y = 0;
            this.xSec = 0;
            this.ySec = 0;
            this.ID = 0;
        }

        public vektor(int ID, float x, float z, float y, byte xSec, byte ySec)
        {
            this.x = x;
            this.z = z;
            this.y = y;
            this.xSec = xSec;
            this.ySec = ySec;
            this.ID = ID;
        }
    }
    public sealed class slotItem
    {
        public int ID, dbID, Durability, BlueStr;
        public byte PlusValue, Type, Slot;
        public short Amount = 1;
    }

    public sealed class point
    {
        public double x, z, y;
        public byte xSec, ySec, test;
        public int ID, Number, Price;
        public string Name;
    }

    public sealed class cavepoint // Added for cave telepad locations
    {
        public double x, z, y;
        public byte xSec, ySec, test;
        public int ID, Number, Price;
        public string Name;
    }
    
    public class s_data
    {
        public enum Definedtype
        {
            Imbue,Buff,Attack
        }
        public enum RadiusTypes
        { FRONTRANGERADIUS = 1, SURROUNDRANGERADIUS = 2, TRANSFERRANGE = 3, PENETRATION = 4, PENETRATIONRANGED = 5, MULTIPLETARGET = 6, ONETARGET = 7 } ;

        public enum SkillTypes
        { PASSIVE = 0, ACTIVE = 2, IMBUE = 1 } ;

        public enum ItemTypes
        { SHIELD, EUSHIELD, BOW, ONEHANDED, TWOHANDED, AXE, WARLOCKROD, CLERICROD, STAFF, XBOW, DAGGER, BARD, LIGHTARMOR, DEVILSPIRIT};

        public enum TargetTypes
        { MOB = 0x001, PLAYER = 0x010, NOTHING = 0x100 } ;

        public Dictionary<string, int> Properties1, Properties2, Properties3, Properties4, Properties5, Properties6;
        public List<ItemTypes> ReqItems;
        public List<summon_data> SummonList;
        public int MinAttack, MaxAttack, PhyPer, reUseTime, Per, efrUnk1, EffectLvl1;
        public int ID, SkillPoint, NextSkill, tmpProp, Mana, MagPer, CastingTime, CastingTimePhase1, CastingTimePhase2, CastingTimePhase3, AttackTime, AmountEffect, Time, Distance, SimultAttack, AttackCount;
        public short Mastery;
        public byte sType, eLevel;
        public string Name, Series;
        public byte Weapon1, Weapon2;
        public bool isAttackSkill;
        public RadiusTypes RadiusType;
        public SkillTypes SkillType;
        public Definedtype Definedname;
        public TargetTypes TargetType;
        public bool canSelfTargeted;
        public bool needPVPstate;

        public struct summon_data
        {
            public int ID;
            public byte Type, MinSummon, MaxSummon;
        }
        public s_data()
        {
            this.SummonList = new List<summon_data>();
            this.ReqItems = new List<ItemTypes>();
            this.Properties1 = new Dictionary<string, int>();
            this.Properties2 = new Dictionary<string, int>();
            this.Properties3 = new Dictionary<string, int>();
            this.Properties4 = new Dictionary<string, int>();
            this.Properties5 = new Dictionary<string, int>();
            this.Properties6 = new Dictionary<string, int>();
        }
    }
    public class JobLevel
    {
        public byte level, jobtitle;
        public Int64 exp;
    }
    public class levelgold
    {
        public short min, max;
    }
    public class shop_data
    {
        public string tab;
        public string[] Item = new string[300];
        public static shop_data GetShopIndex(string name)
        {
            shop_data result = Data.ShopData.Find(delegate(shop_data bk)
                    {
                        return bk.tab == name;
                    }
                    );
            return result;
            //if (result == id) return true;
            /*for (int i = 0; i < Data.ShopData.Count; i++)
            {
                if (Data.ShopData[i].tab == name) return Data.ShopData[i];
            }
            return null;*/
        }
    }
    public class itemblue
    {
        public int totalblue = 0;
        public ArrayList blue;
        public ArrayList blueamount;
    }
    public class GuildUniqueList
    {
        public string GuildUnique;
    }
    public sealed class region
    {
        public int ID;
        public int SecX;
        public int SecY;
        public string Name;
    }
    public class CaveTeleports
    {
        public string name;
        public byte xsec, ysec;
        public double x, z, y;
    }
    public class reverse
    {
        public int ID;
        public short area;
        public double x, z, y;
        public byte xSec, ySec;
    }
    public class TeleportPrice
    {
        public int price;
        public int ID;
        public int level;
    }
    public sealed class SectorObject
    {
        public struct n7nEntity
        {
            public struct sPoint
            {
                public float x, y, z;
            };

            public struct sLine
            {
                public short PointA, PointB;
                public byte flag;
            };

            public byte ObjectMapflag;
            public sPoint Position;
            public List<sPoint> Points;
            public List<sLine> OutLines;
        }

        public float GetHeightAt(float x, float y)
        {
	        return this.heightmap[(int)y * 97 + (int)x];
        }

        public float[] heightmap = new float[9409];
        public int entityCount;
        public List<n7nEntity> entitys = new List<n7nEntity>(); 
    }
}
