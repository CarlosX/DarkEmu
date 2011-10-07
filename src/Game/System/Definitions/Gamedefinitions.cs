///////////////////////////////////////////////////////////////////////////
// DarkEmu: Game definitions
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
        public Client client;
        public Decode PacketInformation;
        public player Player;
        public character Character;
        public DateTime lastPing;
        public Random rnd = new Random();
        public static int maxSlots;
        public static Network.Servers.IPCServer IPC;
        public static Random grnd = new Random();
        public static DateTime ServerStartedTime;
        public static aList<Systems> clients = new aList<Systems>();
        public static List<obj> Objects = new List<obj>();
        public static List<world_item> WorldItem = new List<world_item>();
        public static List<party> Party = new List<party>();
        public static List<pet_obj> HelperObject = new List<pet_obj>();
        public static List<spez_obj> SpecialObjects = new List<spez_obj>();

        public character Characterinfo
        {
            set { Character.InGame = false; }
        }

        public Systems(Client s)
        {
            client = s;
        }

        public class Rate
        {
            public static byte Gold, Item, Xp, Sp, Sox, AlchemyDrops, MobSpawn, Spawns, Elixir, Alchemyd, ETCd;
        }
    }
}
