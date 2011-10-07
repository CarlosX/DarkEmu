///////////////////////////////////////////////////////////////////////////
// DarkEmu: Load Drops
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.IO;
using System.Linq;
using System.Text;
using DarkEmu_GameServer.Global;

namespace DarkEmu_GameServer.File
{
    class Load_Drops
    {
        public static void Databases()
        {
            Data.DropBase.Add("alchemymaterial", Data.MaterialDataBase);
            Data.DropBase.Add("event", Data.EventDataBase);
            Data.DropBase.Add("sox", Data.SoxDataBase);
            Data.DropBase.Add("armors", Data.ArmorDataBase);
            Data.DropBase.Add("weapons", Data.WeaponDataBase);
            Data.DropBase.Add("jewelery", Data.JewelDataBase);
            Data.DropBase.Add("tablets", Data.StoneDataBase);
            Data.DropBase.Add("elixir", Data.ElixirDataBase);
            Data.DropBase.Add("arrows", Data.EtcDatabase);
            Data.DropBase.Add("potions", Data.EtcDatabase);
            Data.DropBase.Add("scrolls", Data.EtcDatabase);
        }
    }
}
