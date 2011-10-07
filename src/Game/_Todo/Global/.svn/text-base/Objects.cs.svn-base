///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using SrxRevo;
using System;
using Framework;
using System.Linq;
using System.Text;
using SrxRevo.Global;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

namespace SrxRevo
{
    public class Data
    {
        public static item_database[] ItemBase = new item_database[40000];
        public static objectdata[] ObjectBase = new objectdata[39000];
        public static point[] PointBase = new point[250];
        public static cavepoint[] cavePointBase = new cavepoint[250];// Added for cave telepad locations
        public static s_data[] SkillBase = new s_data[35000];
        public static int[] MasteryBase = new int[121];
        public static levelgold[] LevelGold = new levelgold[141];
        public static List<JobLevel> Joblevelinfo = new List<JobLevel>(141);
        public static List<shop_data> ShopData = new List<shop_data>(500);
        public static long[] LevelData = new long[141];
        public static Dictionary<int, itemblue> ItemBlue = new Dictionary<int, itemblue>();
        public static List<MagicOption> MagicOptions = new List<MagicOption>(500);
        public static List<region> RegionBase = new List<region>(1954);
        public static List<region> safeZone = new List<region>(1000);
        public static List<region> Cave = new List<region>(50);
        public static List<CaveTeleports> CaveTeleports = new List<CaveTeleports>(80);
        public static List<TeleportPrice> TeleportPrice = new List<TeleportPrice>(800);
        public static reverse[] ReverseData = new reverse[43];
        public static Dictionary<short, SectorObject> MapObject = new Dictionary<short, SectorObject>();
        public static List<quest_database> QuestData = new List<quest_database>(900);
        //###########################################################
        // Drop databases
        //###########################################################
        public static drop_database SoxDataBase = new drop_database();
        public static drop_database StoneDataBase = new drop_database();
        public static drop_database ElixirDataBase = new drop_database();
        public static drop_database MaterialDataBase = new drop_database();
        public static drop_database EventDataBase = new drop_database();
        public static drop_database ArmorDataBase = new drop_database();
        public static drop_database WeaponDataBase = new drop_database();
        public static drop_database EtcDatabase = new drop_database();
        public static drop_database JewelDataBase = new drop_database();
        public static Dictionary<string, drop_database> DropBase = new Dictionary<string, drop_database>();
        //###########################################################
        public static double[] AngleSin = new double[360];
        public static double[] AngleCos = new double[360];
        static Data()
        {
            for (short i = 0; i < 360; i++)  // precalculated sin/cos tables for degrees 0-359 for speedup
            {
                AngleSin[i] = Math.Sin(i * (Math.PI / 180));
                AngleCos[i] = Math.Cos(i * (Math.PI / 180));
            }
        }
    }
    public class player
    {
        public string AccountName, Password;
        public int ID, CreatingCharID;
        public long pGold;
        public byte wSlots;
        public int Silk, SilkPrem;
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
    public class character
    {
        public _Information Information;
        public _job Job;
        public _Messagespm Getmessageinfo;
        public _Stats Stat;
        public _pos Position;
        public _ticket Ticket;
        public _speed Speed;
        public _action Action;
        public _network Network;
        public _alchemy Alchemy;
        public _stall Stall;
        public _guild GuildNetWork;
        public _state State;
        public _Guide Guideinfo;
        public _account Account;
        public BuyPack Buy_Pack = new BuyPack();
        public _trans Transport;
        public _grabpet Grabpet;
        public _attackpet Attackpet;
        public ID Ids;
        public Guildinfo Guild;
        public _Quest Quest;
        public _Blues Blues;
        public premium Premium;
        public List<int> Spawn = new List<int>();
        public bool[] aRound = new bool[10];
        public bool InGame, Spawning, deSpawning, Teleport, Transformed;
        public int LogNum;

        public character()
        {
            this.Action.Buff.OverID = new int[20];
            this.Action.Buff.SkillID = new int[20];
            this.Action.Buff.UpdatedStats = new _Stats[20];
            this.Action.Buff.InfiniteBuffs = new Dictionary<string, byte>();
            this.Action.DeBuff.Effect.EffectID = new Effect.EffectNumbers[20];
            this.Action.DeBuff.Effect.EffectImpactTimer = new Timer[20];
            this.Action.DeBuff.Effect.SkillID = new int[20];
            this.Speed.Updateded = new float[21];
            this.Information.Item.Potion = new byte[20];
            this.Action.MonsterID = new List<int>(8);
            this.LogNum = Global.RandomID.GetRandom(50, 100);
            this.Action.upmasterytimer = false;
            this.Action.upskilltimer = false;
            this.Action.movementskill = false;
            this.Network.Guild = new guild();
        }

        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }

        public struct _account
        {
            public long StorageGold;
            public int ID;
            public byte StorageSlots;
        }

        public bool Spawned(int id)
        {
            bool result = Spawn.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
        public struct _trans
        {
            public bool Right;
            public bool Spawned;
            public pet_obj Horse;
        }
        public struct _job
        {
            public int exp, state;
            public string Jobname;
            public byte type, rank, level;
            public bool jobactive;
        }
        public struct _grabpet
        {
            public bool Active;
            public bool Spawned;
            public bool Picking;
            public int Grabpetid;
            public pet_obj Details;
        }
        public struct _attackpet
        {
            public bool Active;
            public bool Spawned;
            public bool Picking;
            public int Uniqueid;
            public pet_obj Details;
        }
        public struct premium
        {
            public int TicketItemID, TicketID, OwnerID;
            public bool Active;
            public DateTime StartTime, EndTime;
        }
        public struct Guildinfo
        {
            public string Name, MessageTitle, Message, MyGrant, MyGuildName, UnionGuildName, UnionLeader, UnionGuildLeader;
            public int GuildStorageSlots, GuildWarGold, GuildID, MyGuildAuth, GuildMemberId, GuildPoints, MyGuildPoints, UnionCount, UnionMemberCount, UnionGuildLeaderID, UnionGuildID, UnionGuildLeaderModel;
            public byte GuildMemberCount, MyGuildRank, MyGuildPosition, Level, UnionLevel;
            public bool Inguild, UsingStorage;
        }
        public struct _Messagespm
        {
            public string from, to, message;
            public short status;
            public DateTime time;
        }
        public struct _Quest
        {
            public bool QuestActive;
            public int TalkToNpc, QuestItem, QuestDrop, QuestNPC;
        }
        public struct _Information
        {
            //Need to clean this up (TODO:)
            public bool SkyDroming,Storage,FirstLogin, Handle, Quit, Scroll, Casting, Skill, TuruncuZerk, WelcomeMessage, Murderer, Invisible;
            public int Model, SpBar, SkillPoint, CharacterID, Slots, Pvpstate, StallModel, MessageCount;
            public double Angle;
            public int UniqueID, MaskModel, Online;
            public byte Level, HighLevel, BerserkBar, Volume, GM, Place, Title, Phy_Balance, Mag_Balance, Pvptype,BerserkOran, ExpandedStorage, Race;
            public short Attributes;
            public string Name;
            public long XP, Gold;
            public bool Berserking, PvpWait, PvP, Job, Autonotice, CheckParty, JoinGuildWait;
            public List<byte> InventorylistSlot;
            public _item Item;
        }
        public struct _Blues
        {
            public int Luck, Resist_Frostbite, Resist_Eshock, Resist_Burn, Resist_Poison, Resist_Zombie, Resist_Stun, Resist_CSMP, Resist_Disease, Resist_Sleep, Resist_Fear, Resist_All,UniqueDMGInc,MonsterIgnorance;
           public _Stats[] UpdatedStats;
           public double hpregen, mpregen;
        }
        public struct _state
        {
            public bool Die, Busy, Sitting, Standing, Exchanging, Inparty, GuildInvite, Sendonce, UnionApply;
			public bool SafeState;
            public bool Frozen, Frostbite, Burned, Shocked;
            public byte LastState, DeadType;
        }
        public struct _Guide
        {
            public int[] G1;
            public int[] Gchk;
        }
        public struct _item
        {
            public int wID, sID;
            public short sAmount;
            public byte[] Potion;
        }

        public struct _Stats
        {
            public double MinPhyAttack, MaxPhyAttack, MinMagAttack, MaxMagAttack, PhyDef, MagDef, uMagDef, uPhyDef;
            public double Absorb_mp, UpdatededPhyAttack, UpdatededMagAttack, AttackPower, EkstraMetre;
            public short Strength, Intelligence;
            public short phy_Absorb, mag_Absorb;
            public byte BowRange;
            public double Hit, Parry;
            public int Hp, Mp, SecondHp, SecondMP, BlockRatio, CritParryRatio;
            public _skill Skill;
        }
        public struct _pos
        {
            public float x, y, z;
            public float wX, wY, wZ, kX, kY;
            public byte packetxSec, packetySec;
            public ushort packetX, packetZ, packetY;
            public byte xSec, ySec, wxSec, wySec;
            public double RecordedTime, Time;
            public bool Walking, Walk;
            public ushort Angle;
            public Stopwatch Movementwatch;
        }
        public struct _speed
        {
            public float WalkSpeed, RunSpeed, BerserkSpeed, DefaultSpeed;
            public float[] Updateded;
            public double AttackSpeedModifier;
            public int DefaultSpeedSkill;
        }
        public struct _skill
        {
            public int[] Mastery;
            public byte[] Mastery_Level;
            public int[] Skill;
            public int SkillCastingID;
            public int AmountSkill;
        }
        public struct _action
        {
            public int Target, UsingSkillID, CastingSkill, ImbueID, AttackingID;
            public bool Cast, nAttack, PickUping, sAttack, sCasting, upmasterytimer, upskilltimer,movementskill,repair,normalattack;
            public _buff Buff;
            public _debuff DeBuff;
            public object Object;
            public _usingSkill Skill;
            public List<int> MonsterID;
            public byte sSira;
            public bool Check()
            
            {
                if (MonsterID == null) return false;
                if (MonsterID.Count >= 8) return true;
                return false;
            }
            public bool MonsterIDCheck(int id)
            {
                bool result = MonsterID.Exists(
                        delegate(int bk)
                        {
                            return bk == id;
                        }
                        );
                return result;
            }
        }
        public struct _usingSkill
        {
            public int MainSkill, MainCasting;
            public bool P_M;
            public byte Distance, Instant, NumberOfAttack, Targethits, Tdistance, Found, sSira;
            public int[] SkillID;
            public int[] FoundID;
            public bool[] TargetType;
            public byte OzelEffect;
            public bool canUse;
        }

        public struct _buff
        {
            public int[] OverID;
            public int[] SkillID;
            public Dictionary<string, byte> InfiniteBuffs;
            public byte slot, count;
            public short castingtime;
            public bool Casting;
            public _Stats[] UpdatedStats;
        }

        public struct _debuff
        {
            public _effect Effect;
        }

        public struct _effect
        {
            public Effect.EffectNumbers[] EffectID;
            public Timer[] EffectImpactTimer;
            public int[] SkillID;
        }

        public struct _network
        {
            public int TargetID;
            public party Party;
            public guild Guild;
            public _exchange Exchange;
            public stall Stall;
        }
        public struct _exchange
        {
            public List<slotItem> ItemList;
            public long Gold;
            public bool Approved;
            public bool Window;
        }
        public struct _stall
        {
            public List<slotItem> ItemList;
            public List<string> StallName;
            public List<int> Stallsid;
            public Thread StallThread;
            public bool Stallactive;
        }
        public struct _guild
        {
            public List<string> GuildList;
            public List<int> MemberList;
            public List<string> CharName;
            public string UniqueGuild;
        }
        public struct _alchemy
        {
            public bool working;
            public List<slotItem> ItemList;
            public List<int> StonesList;
            public List<int> Elementlist;
            public Thread AlchemyThread;
            public slotItem AlchemyItem;
            public slotItem Elixir, Stone;
            public slotItem LuckyPowder;
        }
        public struct _ticket
        {
            public int[] TicketItemID;
            public int[] TicketSecondTimeLeft;
            public int[] TicketTimeGaveOut;
            public int[] TicketFullTimeLeft;
            public bool TicketActive;
            public bool[] TicketSecondActive;
            public DateTime[] ticketbufftime;
            public DateTime[] ticketstarttime;
            public int[] TicketOverID;
            public DateTime[] ticketsecbufftime;
            public byte ticketcount;
            public int Exp;
            public _Stats PlusStat;
        }
    }
    
    public class pet_obj
    {
        public string Petname, OwnerName;
        public int Model;
        public int UniqueID, OwnerID;
        public Int64 exp;
        public double x, z, y;
        public byte xSec, ySec, Slots, Named, Level;
        public int Hp, Hpg;
        public bool Information, Walking, Attacking, Defensive;
        public ID Ids;
        public float Walk = 45, Run = 95, Zerk = 100, Speed1, Speed2;
        public List<int> Spawn = new List<int>();

        public List<int> statussend()
        {
            try
            {

                if (this.Model != 0)
                {
                    lock (Systems.clients)
                    {
                        for (int i = 0; i < Systems.clients.Count; i++)
                        {
                            try
                            {
                                if (Systems.clients[i].Character.Position.x >= (this.x - 50) && Systems.clients[i].Character.Position.x <= ((this.x - 50) + 100) && Systems.clients[i].Character.Position.y >= (this.y - 50) && Systems.clients[i].Character.Position.y <= ((this.y - 50) + 100))
                                {
                                    Systems.clients[i].client.Send(Packet.ChangeStatus(this.UniqueID, 3, 0));
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("statussendpet::send error on index {0}/{1}", i, Systems.clients.Count);
                                Console.WriteLine(ex);
                                Systems.Debugger.Write(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("statussendpet::send error");
                Console.WriteLine(ex);
                Systems.Debugger.Write(ex);
            }
            return Spawn;
        }

        public List<int> Speedsend()
        {
            try
            {

                if (this.Model != 0)
                {
                    lock (Systems.clients)
                    {
                        for (int i = 0; i < Systems.clients.Count; i++)
                        {
                            try
                            {
                                if (Systems.clients[i].Character.Position.x >= (this.x - 50) && Systems.clients[i].Character.Position.x <= ((this.x - 50) + 100) && Systems.clients[i].Character.Position.y >= (this.y - 50) && Systems.clients[i].Character.Position.y <= ((this.y - 50) + 100))
                                {
                                    Systems.clients[i].client.Send(Packet.SetSpeed(this.UniqueID, this.Speed1, this.Speed2));//Global Speed Update
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("speedsendpet::send error on index {0}/{1}", i, Systems.clients.Count);
                                Console.WriteLine(ex);
                                Systems.Debugger.Write(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("speedsendpet::send error");
                Console.WriteLine(ex);
                Systems.Debugger.Write(ex);
            }
            return Spawn;
        }

        public List<int> SpawnMe()
        {
            try
            {

                if (this.Model != 0)
                {
                    lock (Systems.clients)
                    {
                        for (int i = 0; i < Systems.clients.Count; i++)
                        {
                            try
                            {
                                if (!Spawned(Systems.clients[i].Character.Information.UniqueID))
                                {
                                    if (Systems.clients[i].Character.Position.x >= (this.x - 50) && Systems.clients[i].Character.Position.x <= ((this.x - 50) + 100) && Systems.clients[i].Character.Position.y >= (this.y - 50) && Systems.clients[i].Character.Position.y <= ((this.y - 50) + 100))
                                    {
                                        Spawn.Add(Systems.clients[i].Character.Information.UniqueID);
                                        Systems.clients[i].client.Send(Packet.ObjectSpawn(this));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("worlditem3::send error on index {0}/{1}", i, Systems.clients.Count);
                                Console.WriteLine(ex);
                                Systems.Debugger.Write(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("worlditem3::send error");
                Console.WriteLine(ex);
                Systems.Debugger.Write(ex);
            }
            return Spawn;
        }
        public void Send(byte[] buff)
        {
            try
            {
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            if (Spawned(Systems.clients[i].Character.Information.UniqueID))
                            {
                                Systems.clients[i].client.Send(buff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Systems.Debugger.Write(ex);
                            Console.WriteLine("pet_obj::send error on index {0}/{1}", i, Systems.clients.Count);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
                Console.WriteLine("pet_obj::send error");
            }
        }
        public void Send(Systems sys, byte[] buff)
        {
            try
            {
                if (Spawned(sys.Character.Information.UniqueID))
                {
                    sys.client.Send(buff);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send error: {0}", ex);
            }
        }
        public void DeSpawnMe()
        {
            try
            {
                byte[] buff = Packet.ObjectDeSpawn(this.UniqueID);
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            if (Spawned(Systems.clients[i].Character.Information.UniqueID))
                            {
                                Systems.clients[i].client.Send(buff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("pet_obj::DeSpawnMe error on index {0}/{1}", i, Systems.clients.Count);
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
                Spawn.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("pet_obj::DeSpawnMe error");
                Systems.Debugger.Write(ex);
            }
            finally
            {
                Global.ID.Delete(this.UniqueID);
                Systems.HelperObject.Remove(this);
                Dispose();
            }
        }
        public void DeSpawnMe(bool t)
        {
            try
            {
                byte[] buff = Packet.ObjectDeSpawn(this.UniqueID);
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            if (Spawned(Systems.clients[i].Character.Information.UniqueID))
                            {
                                Systems.clients[i].client.Send(buff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("worlditem::DeSpawnMe error on index {0}/{1}", i, Systems.clients.Count);
                            Console.WriteLine(ex);
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
                Spawn.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("worlditem::DeSpawnMe error");
                Console.WriteLine(ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
        public bool Spawned(int id)
        {
            bool result = Spawn.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
    }
    public class world_item
    {
        public int Model, amount = 1;
        public int UniqueID, fromOwner, Owner;
        public double x, z, y;
        public byte xSec, ySec;
        public byte PlusValue, Type, fromType, PhyAtt, PhyDef, MagAtt, MagDef, BlockRatio, AttackRating;
        public List<MagicOption> blues = new List<MagicOption>(500);
        public bool downType;
        public Time timer;
        public ID Ids;
        public List<int> Spawn = new List<int>();
        static int seed = Environment.TickCount;
        Random randomtest = new Random(Interlocked.Increment(ref seed));

        public void Send(byte[] buff, bool b)
        {
            try
            {
                if (b && this.Model != 0)
                {
                    lock (Systems.clients)
                    {
                        for (int i = 0; i < Systems.clients.Count; i++)
                        {
                            try
                            {
                                if (!Spawned(Systems.clients[i].Character.Information.UniqueID))
                                {
                                    if (Systems.clients[i].Character.Position.x >= (this.x - 50) && Systems.clients[i].Character.Position.x <= ((this.x - 50) + 100) && Systems.clients[i].Character.Position.y >= (this.y - 50) && Systems.clients[i].Character.Position.y <= ((this.y - 50) + 100))
                                    {
                                        Spawn.Add(Systems.clients[i].Character.Information.UniqueID);
                                        Systems.clients[i].client.Send(buff);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("worlditem::send error on index {0}/{1}", i, Systems.clients.Count);
                                Console.WriteLine(ex);
                                Systems.Debugger.Write(ex);
                            }
                        }
                    }

                    StartDeleteTimer(randomtest.Next(4000, 6000)*10);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("worlditem::send error");
                Console.WriteLine(ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void Send(byte[] buff)
        {
            try
            {
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            if (Spawned(Systems.clients[i].Character.Information.UniqueID))
                            {
                                Systems.clients[i].client.Send(buff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("worlditem2::send error on index {0}/{1}", i, Systems.clients.Count);
                            Console.WriteLine(ex);
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("worlditem2::send error");
                Console.WriteLine(ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void DeSpawnMe()
        {
            try
            {
                byte[] buff = Packet.ObjectDeSpawn(this.UniqueID);
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            if (Spawned(Systems.clients[i].Character.Information.UniqueID))
                            {
                                Systems.clients[i].client.Send(buff);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("worlditem::DeSpawnMe error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
                
                Spawn.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("worlditem::DeSpawnMe error {0}",ex);
                Systems.Debugger.Write(ex);
            }
        }

        public bool Spawned(int id)
        {
            bool result = Spawn.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }
        public struct Time
        {
            public Timer Delete;
        }
        protected void delete_callback(object e)
        {
            lock (this)
            {
                try
                {
                    if (this != null)
                    {
                        this.DeSpawnMe();
                        //RandomID.Delete(this.UniqueID);
                        StopDeleteTimer();
                        //this.Dispose();
                        Systems.WorldItem.Remove(this);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("world item error..");
                    Systems.Debugger.Write(ex);
                } 
            }
        }
        public void StopDeleteTimer()
        {
            if (timer.Delete != null)
            {
                timer.Delete.Dispose();
                timer.Delete = null;
            }
        }
        void StartDeleteTimer(int time)
        {
            if (timer.Delete == null) timer.Delete = new Timer(new TimerCallback(delete_callback), 0, time, 0);
        }
        public void Dispose()
        {
            // this gc uses a lots of resource ( really lot )
            //GC.Collect(GC.GetGeneration(this));
        }
    }

    #region Special Objects Class
    public partial class spez_obj
    {
        public string Name;
        public int ID;
        public int UniqueID;
        public ID Ids;
        public byte xSec, ySec;
        public double x, z, y;
        public short spezType;
        public int Radius; // radius for harmony
        public List<int> Inside = new List<int>();
        public List<int> Spawn = new List<int>();
        public Time timer;

 
        public struct Time
        {
            public Timer Delete;
            public Timer HarmonyBuff;
        }

        public bool Spawned(int id)
        {
            bool result = Spawn.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );
            return result;
        }

        protected void delete_callback(object e)
        {
            try
            {
                if (this != null)
                {
                    this.DeSpawnMe();
                    //RandomID.Delete(this.UniqueID);
                    Systems.SpecialObjects.Remove(this);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("spez_obj delete error..");
                Systems.Debugger.Write(ex);
            }
        }

        public void StopDeleteTimer()
        {
            if (timer.Delete != null)
            {
                timer.Delete.Dispose();
                timer.Delete = null;
            }
        }
        void StartDeleteTimer(int time)
        {
            if (timer.Delete == null) timer.Delete = new Timer(new TimerCallback(delete_callback), 0, time, 0);
        }

        public void SpawnMe(int duration)
        {
            try
            {
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            Systems sys = Systems.clients[i];
                            if (this.x >= (sys.Character.Position.x - 50) && this.x <= ((sys.Character.Position.x - 50) + 100) && this.y >= (sys.Character.Position.y - 50) && this.y <= ((sys.Character.Position.y - 50) + 100) && this.Spawned(sys.Character.Information.UniqueID) == false)
                            {
                                this.Spawn.Add(sys.Character.Information.UniqueID);
                                sys.client.Send(Packet.ObjectSpawn(this));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("spez_obj SpawnMe error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                            Systems.Debugger.Write(ex);
                        }
                    }
                }
                if (duration != 0) StartDeleteTimer(duration);

                if (this.Name.Contains("HARMONY")) { StartHarmonyBuff(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine("spez_obj SpawnMe error: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }

        public void DeSpawnMe()
        {
            try
            {
                byte[] buff = Packet.ObjectDeSpawn(this.UniqueID);
                lock (Systems.clients)
                {
                    for (int i = 0; i < Systems.clients.Count; i++)
                    {
                        try
                        {
                            if (Spawned(Systems.clients[i].Character.Information.UniqueID))
                            {
                                Systems.clients[i].client.Send(buff);

                                //end buff
                                if (Formule.GetSurroundRange(new Global.vektor(Systems.clients[i].Character.Information.UniqueID, Systems.clients[i].Character.Position.packetX, Systems.clients[i].Character.Position.packetZ, Systems.clients[i].Character.Position.packetY, Systems.clients[i].Character.Position.xSec, Systems.clients[i].Character.Position.ySec), new Global.vektor(0, (float)this.x, (float)this.z, (float)this.y, this.xSec, this.ySec), this.Radius))
                                {
                                    if (Systems.clients[i].Character.Action.Buff.InfiniteBuffs.ContainsKey(this.Name))
                                    {
                                        Systems.clients[i].SkillBuffEnd(Systems.clients[i].Character.Action.Buff.InfiniteBuffs[this.Name]);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("spez_obj::DeSpawnMe error on index {1}/{2}: {0}", ex, i, Systems.clients.Count);
                            Systems.Debugger.Write(ex);
                        }
                    }
                }

                Spawn.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("spez_obj::DeSpawnMe error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }

        private void StartHarmonyBuff()
        {
            if (timer.HarmonyBuff != null) timer.HarmonyBuff.Dispose();
            timer.HarmonyBuff = new Timer(new TimerCallback(HarmonyBuff_callback), 0, 0, 500);
        }
        private void HarmonyBuff_callback(object e)
        {
            try
            {
                foreach (int p in this.Spawn)
                {
                    Systems s = Systems.GetPlayer(p);

                    //double distance = Formule.gamedistance((float)this.x, (float)this.y, s.Character.Position.x, s.Character.Position.y);
                    double distance = Formule.gamedistance(this, s.Character.Position);
                    if (distance <= this.Radius && !s.Character.Action.Buff.InfiniteBuffs.ContainsKey(this.Name))
                    {
                        byte slot = s.SkillBuffGetFreeSlot();
                        if (slot == 255) return;

                        //add properties
                        foreach (KeyValuePair<string, int> a in Data.SkillBase[this.ID].Properties1)
                        {
                            if (s.SkillAdd_Properties(s, a.Key, true, slot)) { return; };
                        }

                        s.Character.Action.Buff.SkillID[slot] = this.ID;
                        s.Character.Action.Buff.OverID[slot] = s.Character.Ids.GetBuffID();
                        s.Character.Action.Buff.slot = slot;
                        s.Character.Action.Buff.count++;

                        s.Send(s.Character.Spawn, Packet.SkillIconPacket(s.Character.Information.UniqueID, this.ID, s.Character.Action.Buff.OverID[s.Character.Action.Buff.slot], false));
                        s.Character.Action.Buff.InfiniteBuffs.Add(this.Name, s.Character.Action.Buff.slot);

                        Inside.Add(s.Character.Information.UniqueID);
                    }
                    else if (distance >= this.Radius && s.Character.Action.Buff.InfiniteBuffs.ContainsKey(this.Name))
                    {
                        if (Inside.Contains(s.Character.Information.UniqueID))
                        {
                            s.SkillBuffEnd(s.Character.Action.Buff.InfiniteBuffs[this.Name]);
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }            
        }
    }
    #endregion

    #region Monster Class
    public partial class obj
    {
        public List<int> Spawn = new List<int>();
        public byte xSec, ySec, Agresif, oldAgresif, Type, Kat, State, LocalType;
        public double AttackSpeed = 1.0;
        public int ID;
        public double x, z, y, wx, wy, WalkingTime, RecordedTime, oX, oY;
        public short area;
        public Int32 rotation;
        public object Target;
        public int HP, aTime, MonsterAgroCount;
        public int UniqueID, LastCasting;
        public sbyte Move;
        public bool Busy, AutoMovement, OrgMovement, Walking, Attacking, Die, GetDie, AutoSpawn, bSleep, Frozen, Frostbite, Burned, Shock, Darkness;
        public float SpeedWalk, SpeedRun, SpeedZerk;
        public bool[] aRound = new bool[10];
        public bool[] guard = new bool[3];
        public byte spawnOran;
        public List<_agro> Agro = new List<_agro>(10);
        public ID Ids;
        public Random rnd = new Random();
        public _debuff DeBuff;

        public obj()
        {
            DeBuff.Effect.EffectID = new Effect.EffectNumbers[20];
            DeBuff.Effect.EffectImpactTimer = new Timer[20];
            DeBuff.Effect.SkillID = new int[20];

            SpawnWatch.Start();
        }
        public struct _debuff
        {
            public _effect Effect;
        }
        public struct _effect
        {
            public Effect.EffectNumbers[] EffectID;
            public Timer[] EffectImpactTimer;
            public int[] SkillID;
        }

        public object GetTarget()
        {

            int id = 0;
            if (Agro != null && Agro.Count > 0)
                for (byte b = 0; b < Agro.Count; b++)
                {
                    if (Agro[b].playerDMD == Agro.Max(f => f.playerDMD))
                    {
                        id = Agro[b].playerID;
                        break;
                    }
                }
            return Systems.GetPlayer(id);
        }

        public void DeleteTarget()
        {
            try
            {
                for (byte b = 0; b < Agro.Count; b++)
                {
                    if (Agro[b].playerDMD == Agro.Max(f => f.playerDMD))
                    {
                        if (Agro.Count > 1)
                        {
                            Agro.Remove(Agro[b]);
                            return;
                        }
                        else StopAttackTimer();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteTarget()::error");
                Systems.Debugger.Write(ex);
            }
        }
        public void AddAgroDmg(int playerid, int dmg)
        {
            try
            {
                if (Agro != null)
                {
                    for (byte b = 0; b < Agro.Count; b++)
                    {
                        if (Agro[b].playerID == playerid)
                        {
                            Agro[b].playerDMD += dmg;
                            return;
                        }
                    }
                    _agro asf = new _agro();
                    asf.playerID = playerid;
                    asf.playerDMD = dmg;
                    Agro.Add(asf);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddAgroDmg())::error");
                Systems.Debugger.Write(ex);
            }
        }
        public _agro GetAgroClass(int id)
        {
            for (byte b = 0; b <= Agro.Count - 1; b++)
            {
                if (Agro[b].playerID == id) return Agro[b];
            }
            return null;
        }

        Timer Time;
        Timer Movement;
        Timer AggresiveTimer;
        Timer Dead;
        public Timer Attack;
        Timer AutoRun;
        Timer ganimet;
        Timer objeSleep;
        Stopwatch SpawnWatch = new Stopwatch();
        public Timer[] EffectTimer = new Timer[20];

        public void StartObjeSleep(int time)
        {
            if (!bSleep)
            {
                bSleep = true;
                objeSleep = new Timer(new TimerCallback(ObjeSleepCallBack), 0, time, 0);
            }
        }
        public void ObjeSleepCallBack(object e)
        {
            try
            {
                this.State = 3;
                this.bSleep = false;
                
                Send(Packet.Movement(new SrxRevo.Global.vektor(this.UniqueID,
                (float)Formule.packetx((float)this.x, this.xSec),
                (float)this.z,
                (float)Formule.packety((float)this.y, this.ySec),
                this.xSec,
                this.ySec)));
                objeSleep.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ObjeSleepCallBack()::error");
                Systems.Debugger.Write(ex);
            }
        }
        public bool Spawned(int id)
        {
            bool result = Spawn.Exists(
                    delegate(int bk)
                    {
                        return bk == id;
                    }
                    );

            return result;
        }
        public void StartRunTimer(int time)
        {
            
            if (AutoRun != null) AutoRun.Dispose();
            AutoRun = new Timer(new TimerCallback(AutoRunCallBack), 0, 0, time);

        }
        public void StopAutoRunTimer()
        {
        	if (AutoRun != null)
        		AutoRun.Dispose();
        	WalkingTime = 0;
        	Walking = false;
        	this.wx = 0;
        	this.wy = 0;
        }
		/// <summary>
		/// Check the distance between the mob and player every <paramref name="time"/> 
		/// </summary>
		/// <param name="time">
		/// The time in milliseconds.
		/// A <see cref="System.Int32"/>
		/// </param>
        public void StartAgressiveTimer(int time)
        {
            if (AggresiveTimer != null)
            {
                AggresiveTimer.Dispose();
                AggresiveTimer = null;
            }
            MonsterAgroCount += 1;
            AggresiveTimer = new Timer(new TimerCallback(CheckEveryOne), 0, time, 0);
        }
        public void StopAgressiveTimer()
        {
            if (AggresiveTimer != null)
            {
                AggresiveTimer.Dispose();
                AggresiveTimer = null;
            }
            MonsterAgroCount -= 1;
        }
        public void AutoRunCallBack(object e)
        {
            try
            {
                if (this.AutoMovement && !this.Die && this.LocalType == 1 && !this.Busy && !this.bSleep)
                {
                    double reX = oX, reY = oY;
                    Systems.aRound(ref reX, ref reY, 1);
                    this.x = reX;
                    this.y = reY;
                    Send(Packet.Movement(new SrxRevo.Global.vektor(this.UniqueID,
                                                    (float)Formule.packetx((float)this.x, this.xSec),
                                                    (float)z,
                                                    (float)Formule.packety((float)this.y, this.ySec),
                                                    this.xSec,
                                                    this.ySec)));

                }
                CheckEveryOne();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Run callback: ");
                Systems.Debugger.Write(ex);
            }
        }
        /*
        public void AutoRunCallBack(object e)
        {  
            try
            {
              
                if (this.AutoMovement && !this.Die && (this.LocalType == 1 || this.LocalType == 4) && !this.Busy && !this.bSleep && this.SpeedWalk != 0 && this.Walking == false)
                {
                    double reX = oX, reY = oY;
                    //Systems.aRound(ref reX, ref reY, 4, ref this.lastMovementRnd, this.rnd);
                    int angle = (randomtest.Next(1, 35) * 10);
                    reX = reX + 10 * Data.AngleCos[angle];
                    reY = reY + 10 * Data.AngleSin[angle];
                    this.aRound = new bool[8];

                    double distance = Formule.gamedistance((float)this.x, (float)this.y, (float)reX, (float)reY);
                    this.wx = reX - this.x;
                    this.wy = reY - this.y;
                    WalkingTime = (double)(distance / (this.SpeedRun * 0.0768)) * 1000.0;
                    //WalkingTime = (double)((distance) / (this.SpeedWalk)) * 12000;
                    RecordedTime = WalkingTime;

                    if (this.Movement != null) this.Movement.Dispose();
                    this.StartMovement((int)(WalkingTime / 10));

                    this.Walking = true;
                    bool cave = Game.File.FileLoad.CheckCave(this.xSec, this.ySec);
                    if (this.xSec != 0 && this.ySec != 0 && this.x != 0 && this.y != 0 && this.z != 0 && this.UniqueID != 0)
                    {
                        Send(Packet.Movement(new Game.Global.vektor(this.UniqueID,
                                                        (float)Formule.packetx((float)reX, this.xSec),
                                                        (float)z,
                                                        (float)Formule.packety((float)reY, this.ySec),
                                                        this.xSec,
                                                        this.ySec)));
                    }
                    CheckEveryOne();
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("AutoRunCallBack::error");
                deBug.Write(ex);
            }
        }
         */
        /*
        public void AutoRunCallBack(object e)
        {
            try
            {
                if (this.AutoMovement && !this.Die && this.LocalType == 1 && !this.Busy && !this.bSleep)
                {
                    double reX = oX, reY = oY;
                    Systems.aRound(ref reX, ref reY, 1);
                    this.x = reX;
                    this.y = reY;
                    this.aRound = new bool[8];
                    Send(Packet.Movement(new Game.Global.vektor(this.UniqueID,
                                                    (float)Formule.packetx((float)this.x, this.xSec),
                                                    (float)z,
                                                    (float)Formule.packety((float)this.y, this.ySec),
                                                    this.xSec,
                                                    this.ySec)));

                }
                CheckEveryOne();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("AutoRunCallBack error {0}", ex);
                //deBug.Write(ex);
            }
        }*/
        static int seed = Environment.TickCount;
        Random randomtest = new Random(Interlocked.Increment(ref seed));

        public void Sleep(int time)
        {
            this.Busy = true;
            Time = new Timer(new TimerCallback(sleepcallback), 0, time, 0);
        }
        public void Regen(int time)
        {
            this.Busy = true;
            Time = new Timer(new TimerCallback(sleepcallback), 0, time, 0);
        }
        public void StartGanimet(int time)
        {
            this.Busy = true;
            ganimet = new Timer(new TimerCallback(ganicallback), 0, time, 0);
        }
        void ganicallback(object e)
        {
            try
            {
                StopAttackTimer();
                SetExperience();
                MonsterDrop();
                ganimet.Dispose();
                ganimet = null;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Kill error: {0}",ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void StartEffectTimer(int time, byte e_index)
        {
            if (EffectTimer[e_index] != null) EffectTimer[e_index].Dispose();
            EffectTimer[e_index] = new Timer(new TimerCallback(Mob_Effect_CallBack), e_index, time, 0);
        }
        void Mob_Effect_CallBack(object e)
        {
            try
            {
                StopEffectTimer((byte)e);

                foreach (KeyValuePair<string, int> p in Data.SkillBase[this.DeBuff.Effect.SkillID[(byte)e]].Properties1)
                {
                    switch (p.Key)
                    {
                        case "fb":
                            SrxRevo.Effect.DeleteEffect_fb(this, (byte)e);
                            break;
                        case "bu":
                            //GenerateEffect_bu(target, EffectNumbers.BURN, Data.SkillBase[skillid].Properties1["bu"], Data.SkillBase[skillid].Properties2["bu"], Data.SkillBase[skillid].Properties3["bu"]);
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
                Console.WriteLine("Mob_Effect_CallBack() error.. {0}",ex);
            }
        }
        public void StopEffectTimer(byte e_index)
        {
            if (EffectTimer[e_index] != null)
            {
                EffectTimer[e_index].Dispose();
                EffectTimer[e_index] = null;
                Send(Packet.EffectUpdate(this.UniqueID, this.DeBuff.Effect.EffectID[e_index], false));
                this.DeBuff.Effect.EffectID[e_index] = 0;
            }
            //damage timer again
            if (this.DeBuff.Effect.EffectImpactTimer[e_index] != null)
            {
                this.DeBuff.Effect.EffectImpactTimer[e_index].Dispose();
                this.DeBuff.Effect.EffectImpactTimer[e_index] = null;
            }
        }
        public void StartAttackTimer_old(int Time)
        {
            if (Attack != null) Attack.Dispose();

            aTime = Time;
            Attack = new Timer(new TimerCallback(AttackCallBack), 0, 0, aTime);
        }
        /*public void StartAttackTimer(int Time)
        {
            if (Attack != null) Attack.Dispose();

            aTime = Time;
            Attack = new Timer(new TimerCallback(AttackHim), 0, 0, aTime);
        }*/
        public void StopAttackTimer()
        {
            if (Attack != null) Attack.Dispose();
            Attack = null;
        }
        void AttackCallBack(object e)
        {
            try
            {
                if (Attack != null)
                {
                    if (aTime < 1999)
                    {
                        aTime = 2000;
                        Attack.Change(0, aTime);
                    }
                    AttackMain();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AttackCallBack::error");
                Systems.Debugger.Write(ex);
            }
        }
        void sleepcallback(object e)
        {
            try
            {
                this.Time.Dispose();
                if (this.AutoSpawn) reSpawn();
                else 
                { 
                    if (this.Spawn.Count != 0) 
                        SrxRevo.GlobalUnique.ClearObject(this); 
                   
                    if (this.LastCasting != 0) 
                        this.Dispose();

                    Systems.Objects.Remove(this);
                    return; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("sleepcallback(e)::eror...");
                Systems.Debugger.Write(ex);
            }
        }
        void deadcallback(object e)
        {
            try
            {
                this.Die = true;
                this.DeSpawnMe();
                this.Dead.Dispose();
                Sleep(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("deadcallback::error");
                Systems.Debugger.Write(ex);
            }
        }
        public void StartDeadTimer(int time)
        {
            if (Dead != null) Dead.Dispose();
            Dead = new Timer(new TimerCallback(deadcallback), 0, time, 0);
        }
        public void StartMovement(int perTime)
        {
            Movement = new Timer(new TimerCallback(walkcallback), 0, 0, perTime);
        }
        public void StopMovement()
        {
            if (Movement != null)
            {
                Movement.Dispose();
                Movement = null;
            }
        }
        void walkcallback(object e)
        {
            try
            {
                if (this != null)
                {
                    if (this.RecordedTime <= 0)
                    {
                        this.Walking = false;
                        //if (Attacking) AttackHim();
                        this.StopMovement();
                    }
                    else
                    {
                        if (this.RecordedTime <= this.WalkingTime * 0.50 && this.Attacking && Agro != null)
                        {
                            
                        }

                        this.x += wx * 0.1;
                        this.y += wy * 0.1;
                        this.RecordedTime -= (this.WalkingTime * 0.1);
                    }

                    if (SpawnWatch.ElapsedMilliseconds >= 1000)
                    {
                        CheckEveryOne();
                        SpawnWatch.Restart();
                    }
                }
            }
            catch (Exception ex)
            {
                
                Systems.Debugger.Write(ex);
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
    #endregion

    #region Object
    public class _agro
    {
        public int playerID;
        public int playerDMD;
    }
    public sealed class targetObject
    {
        private float o_x, o_y;
        private double magdef, phydef;
        public obj os;
        public Systems sys, main;
        public bool type;
        private int id;
        private short absorbphy, absorbmag;
        private int hps;
        private byte xsec, ysec;
        private byte state;
        private double mabsrob;
        public targetObject(object o, Systems player)
        {
            try
            {
                os = null;
                o_x = 0;
                o_y = 0;
                magdef = 0;
                phydef = 0;
                type = false;

                if (o == null) return;
                main = player;
                if (main == null) return;
                if (o.GetType().ToString() == "SrxRevo.obj")
                {
                    os = o as obj;
                    if (os.Die) { player.StopAttackTimer(); return; }
                    o_x = (float)os.x;
                    o_y = (float)os.y;
                    xsec = os.xSec;
                    ysec = os.ySec;
                    magdef = Data.ObjectBase[os.ID].MagDef;
                    phydef = Data.ObjectBase[os.ID].PhyDef;
                    id = os.UniqueID;
                    type = false;
                    hps = os.HP;
                    state = os.State;
                    main.Character.Action.MonsterID.Add(os.UniqueID);
                    mabsrob = 0;
                    os.Target = player;
                }
                if (o.GetType().ToString() == "SrxRevo.Systems")
                {
                    sys = o as Systems;
                    o_x = sys.Character.Position.x;
                    o_y = sys.Character.Position.y;
                    xsec = sys.Character.Position.xSec;
                    ysec = sys.Character.Position.ySec;
                    magdef = sys.Character.Stat.MagDef;
                    phydef = sys.Character.Stat.PhyDef;
                    id = sys.Character.Information.UniqueID;
                    absorbphy = sys.Character.Stat.phy_Absorb;
                    absorbmag = sys.Character.Stat.mag_Absorb;
                    state = sys.Character.State.LastState;
                    hps = sys.Character.Stat.SecondHp;
                    type = true;
                    mabsrob = sys.Character.Stat.Absorb_mp;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Target object error :  {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void GetDead()
        {
            try
            {
                if (type)
                {
                    if (main.Character.Information.PvP && sys.Character.Information.PvP)
                        sys.Character.State.DeadType = 2;
                    else
                        sys.Character.State.DeadType = 1;
                    sys.BuffAllClose();
                    sys.Character.State.Die = true;
                }
                else
                {
                    os.Die = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetDead()::Error");
                Systems.Debugger.Write(ex);
            }
        }
        public void Sleep(byte types)
        {
            try
            {
                if (!type)
                {
                    Random rnd = new Random();
                    os.State = types;
                    os.StartObjeSleep(rnd.Next(9000, 15000));
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void MP(int mpdusur)
        {
            try
            {

                if (type)
                {
                    sys.Character.Stat.SecondMP -= mpdusur;
                    if (sys.Character.Stat.SecondMP < 0) sys.Character.Stat.SecondMP = 0;
                    sys.UpdateMp();
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public byte HP(int hpdusur)
        {
            try
            {
                if (type)
                {
                    sys.Character.Stat.SecondHp -= hpdusur;
                    sys.UpdateHp();
                    if (sys.Character.Stat.SecondHp <= 0)
                    {
                        sys.Character.Stat.SecondHp = 0;
                        sys.Character.State.Die = true;
                        sys.BuffAllClose();
                        main.StopAttackTimer();
                        sys.StopAttackTimer();
                        sys.StopSkillTimer();
                        return 128;
                    }
                }
                else
                {
                    if (os != null)
                    {
                        os.CheckUnique();
                        if (!os.GetDie)
                            os.HP -= hpdusur;

                        os.AddAgroDmg(main.Character.Information.UniqueID, hpdusur);
                        os.CheckAgro();

                        main.GetBerserkOrb();

                        if (os.HP <= 0)
                        {
                            if (!os.GetDie)
                            {
                                Systems tg = (Systems)os.GetTarget();
                                if (tg.Character.Action.MonsterID != null && tg.Character.Action.MonsterIDCheck(os.UniqueID)) tg.Character.Action.MonsterID.Remove(os.UniqueID);
                                os.CheckUnique(tg);
                                os.StopAttackTimer();
                                os.StopMovement();
                                os.GetDie = true;
                                os.StartGanimet(50);
                                main.StopAttackTimer();
                                os.StartDeadTimer(4000);
                            }
                            return 128;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return 0;
        }
        public double MagDef
        {
            get { return magdef; }
        }
        public double PhyDef
        {
            get { return phydef; }
        }
        public float x
        {
            get { return o_x; }
        }
        public float y
        {
            get { return o_y; }
        }
        public int ID
        {
            get { return id; }
        }
        public short AbsrobPhy
        {
            get { return absorbphy; }
        }
        public short AbsrobMag
        {
            get { return absorbmag; }
        }
        public int GetHp
        {
            get { return hps; }
        }
        public byte xSec
        {
            get { return xsec; }
        }
        public byte ySec
        {
            get { return ysec; }
        }
        public byte State
        {
            get { return state; }
        }
        public double mAbsorb()
        {
            if (type)
                return mabsrob;
            else return 0;
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }

    }
    #endregion
    public sealed partial class stall
    {
        public List<stallItem> ItemList;
        public List<int> Members = new List<int>();
        public List<Systems.Client> MembersClient = new List<Systems.Client>();

        public int ownerID;
        public bool isOpened;
        public string StallName;
        public string WelcomeMsg;

        public sealed class stallItem
        {
            public slotItem Item;
            public ulong price;
            public byte stallSlot;
        }
        public void Send(byte[] buff)
        {
            for (byte b = 0; b <= MembersClient.Count - 1; b++)
            {
                MembersClient[b].Send(buff);
            }
        }
        public void Send(byte[] buff, Systems.Client client)
        {
            for (byte b = 0; b <= MembersClient.Count - 1; b++)
            {
                if (MembersClient[b] != client)
                    MembersClient[b].Send(buff);
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
    public sealed partial class party
    {
        public byte Type;
        public List<int> Members = new List<int>();
        public List<Systems.Client> MembersClient = new List<Systems.Client>();
        public int LeaderID, Race;
        public bool IsFormed,InParty, SingleSend;
        public void Send(byte[] buff)
        {
            for (byte b = 0; b <= MembersClient.Count - 1; b++)
            {
                MembersClient[b].Send(buff);
            }
        }
        public void UpdateCoordinate()
        {
            for (byte b = 0; b <= MembersClient.Count - 1; b++)
            {
                Systems s = Systems.GetPlayer(Members[b]);
                MembersClient[b].Send(Packet.Party_Data(6, Members[b]));
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
    public sealed partial class guild
    {
        public List<int> Members = new List<int>();
        public List<int> Unions = new List<int>();
        public List<int> UnionMembers = new List<int>();
        public List<Systems.Client> MembersClient = new List<Systems.Client>();
        public List<Global.guild_player> MembersInfo = new List<guild_player>();

        public int GuildOwner, Guildid, UniqueUnion, UnionLeader;
        public long StorageGold;
        public string Name, GrantName;
        public int DonateGP, LastDonate;
        public byte FWrank, TotalMembers;
        public bool joinRight, withdrawRight, unionRight, guildstorageRight, noticeeditRight, SingleSend, UsingStorage, UnionActive;

        public byte Level, MaxMembers;
        public string NewsTitle, NewsMessage;
        public int PointsTotal, StorageSlots, Wargold;

        public void Send(byte[] buff)
        {
            for (byte b = 0; b <= MembersClient.Count -1; b++)
            {
                if (MembersClient[b] != null)
                {
                    MembersClient[b].Send(buff);
                }
            }
        }
        public void Dispose()
        {
            GC.Collect(GC.GetGeneration(this));
        }
    }
}


