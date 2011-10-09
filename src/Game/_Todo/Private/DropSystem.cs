///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
// File info: Public packet data
///////////////////////////////////////////////////////////////////////////
using DarkEmu_GameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DarkEmu_GameServer
{
    public partial class obj
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Drop System Base
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SkillDrop(obj o, character c, string type)
        {
            try
            {
                world_item sitem = new world_item();
                switch (type)
                {
                    case "mask":
                        sitem.Model = 10364;
                        sitem.Ids = new Global.ID(Global.ID.IDS.World);
                        sitem.UniqueID = sitem.Ids.GetUniqueID;
                        sitem.PlusValue = 0;
                        sitem.x = this.x + rnd.Next(0, 7);
                        sitem.z = this.z;
                        sitem.y = this.y + rnd.Next(0, 6);
                        sitem.xSec = this.xSec;
                        sitem.ySec = this.ySec;
                        sitem.Type = 3;
                        sitem.fromType = 5;
                        sitem.downType = true;
                        sitem.fromOwner = this.UniqueID;
                        sitem.Owner = ((Systems)this.GetTarget()).Character.Account.ID;

                        Systems.aRound(ref sitem.x, ref sitem.y, 0);
                        Systems.WorldItem.Add(sitem);
                        sitem.Send(Packet.ObjectSpawn(sitem), true);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Skill drop error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        public void MonsterDrop()
        {
            try
            {
                if (this.GetDie || this.Die)
                {
                    if (this.Type != 16)
                    {
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        // Set Target Information
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        Systems sys = (Systems)this.GetTarget();
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        // If There's no target return
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        if (sys == null) return;

                        sbyte Leveldiff = (sbyte)(sys.Character.Information.Level - Data.ObjectBase[ID].Level);
                        int Amountinfo = 0;

                        if (Math.Abs(Leveldiff) < 10 || Math.Abs(Leveldiff) == 0)
                        {
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            // Gold Drop
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            #region Gold
                            int Golddrop = rnd.Next(Data.LevelGold[Data.ObjectBase[ID].Level].min, Data.LevelGold[Data.ObjectBase[ID].Level].max);
                            Amountinfo = 0;
                            if (this.Type == 4 && (rnd.Next(0, 200) < 200 * Systems.Rate.Gold)) Amountinfo = (byte)(rnd.Next(1, 3));
                            if (this.Type == 3 && (rnd.Next(0, 200) < 200 * Systems.Rate.Gold)) Amountinfo = (byte)(rnd.Next(4, 6));
                            if (this.Type == 1 && (rnd.Next(0, 200) < 200 * Systems.Rate.Gold)) Amountinfo = (byte)(rnd.Next(1, 3));
                            if (this.Type == 0 && (rnd.Next(0, 200) < 100 * Systems.Rate.Gold)) Amountinfo = 1;

                            for (byte a = 1; a <= Amountinfo; )
                            {
                                world_item Gold_Drop = new world_item();

                                Gold_Drop.amount = Golddrop * Systems.Rate.Gold;
                                Gold_Drop.Model = 1;

                                if (Gold_Drop.amount < 1000)
                                    Gold_Drop.Model = 1;
                                else if (Gold_Drop.amount > 1000 && Gold_Drop.amount < 10000)
                                    Gold_Drop.Model = 2;
                                else if (Gold_Drop.amount > 10000)
                                    Gold_Drop.Model = 3;

                                Gold_Drop.Ids = new Global.ID(Global.ID.IDS.World);
                                Gold_Drop.UniqueID = Gold_Drop.Ids.GetUniqueID;
                                Gold_Drop.x = this.x + rnd.Next(0, 7);
                                Gold_Drop.z = this.z;
                                Gold_Drop.y = this.y + rnd.Next(0, 7);
                                Gold_Drop.xSec = this.xSec;
                                Gold_Drop.ySec = this.ySec;
                                Gold_Drop.Type = 1;
                                Gold_Drop.downType = true;
                                Gold_Drop.fromType = 5;

                                Systems.aRound(ref Gold_Drop.x, ref Gold_Drop.y, 0);
                                Systems.WorldItem.Add(Gold_Drop);
                                Gold_Drop.Send(Packet.ObjectSpawn(Gold_Drop), true);
                                a++;

                                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                // Send Info To Grabpet
                                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                if (((Systems)this.GetTarget()).Character.Grabpet.Active)
                                {
                                    ((Systems)this.GetTarget()).Pet_PickupItem(Gold_Drop);
                                }
                            }
                            #endregion
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            // Drop Database
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            #region Drop Databases
                            foreach (KeyValuePair<string, DarkEmu_GameServer.Global.drop_database> p in Data.DropBase)
                            {
                                Amountinfo = p.Value.GetAmount(Data.ObjectBase[ID].Type, p.Key);
                                if (Amountinfo > 0)
                                {
                                    for (byte c = 1; c <= Amountinfo; c++)
                                    {
                                        world_item Dropped_Item = new world_item();
                                        Dropped_Item.Model = p.Value.GetDrop(Data.ObjectBase[this.ID].Level, this.ID, p.Key, sys.Character.Information.Race);
                                        if (Dropped_Item.Model == -1) continue; 
                                        Dropped_Item.Ids = new Global.ID(Global.ID.IDS.World);
                                        Dropped_Item.UniqueID = Dropped_Item.Ids.GetUniqueID;
                                        Dropped_Item.PlusValue = Function.Items.RandomPlusValue();
                                        Dropped_Item.MagAtt = Function.Items.RandomStatValue();
                                        Dropped_Item.x = this.x + rnd.Next(0, 11);
                                        Dropped_Item.z = this.z;
                                        Dropped_Item.y = this.y + rnd.Next(0, 11);
                                        Dropped_Item.xSec = this.xSec;
                                        Dropped_Item.ySec = this.ySec;
                                        Dropped_Item.Type = p.Value.GetSpawnType(p.Key);
                                        Dropped_Item.fromType = 5;
                                        Dropped_Item.downType = true;
                                        Dropped_Item.fromOwner = this.UniqueID;
                                        Dropped_Item.amount = p.Value.GetQuantity(this.Type, p.Key);
                                        Dropped_Item.Owner = ((Systems)this.GetTarget()).Character.Account.ID;
                                        Systems.WorldItem.Add(Dropped_Item);
                                        Dropped_Item.Send(Packet.ObjectSpawn(Dropped_Item), true);
                                    }
                                }
                            }
                        }
                        #endregion

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Drop system error: {0}", ex);
            }
        }
    }
}