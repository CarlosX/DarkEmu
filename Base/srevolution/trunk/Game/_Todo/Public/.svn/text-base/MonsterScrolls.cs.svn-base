///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
// File info: Public packet data
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Monster Summon Scrolls
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandleSummon(int scrollid)
        {
            try
            {
                //if (this.Character.Information.Level < 10) return;

                int count = 1;//Default should be set to 1

                //single scroll
                if (scrollid == 3936)
                {
                    count = 5;
                }
                //party scroll
                if (scrollid == 3935)
                {
                    if (this.Character.Network.Party == null) return;
                    if (this.Character.Network.Party.Members.Count < 5) return;

                    count = this.Character.Network.Party.Members.Count;
                }

                int model = GetStrongMobByLevel(this.Character.Information.Level);
                byte type = Data.ObjectBase[model].ObjectType;

                for (int i = 1; i <= count; i++)
                {
                    obj Spawn = new obj();

                    Spawn.ID = model;
                    Spawn.Type = type;
                    Spawn.Ids = new Global.ID(Global.ID.IDS.Object);
                    Spawn.UniqueID = Spawn.Ids.GetUniqueID;
                    Spawn.x = Character.Position.x;
                    Spawn.z = Character.Position.z;
                    Spawn.y = Character.Position.y;
                    Spawn.oX = Spawn.x;
                    Spawn.oY = Spawn.y;
                    Spawn.xSec = Character.Position.xSec;
                    Spawn.ySec = Character.Position.ySec;
                    Spawn.AutoMovement = true;
                    Spawn.State = 1;
                    Spawn.Move = 1;
                    Spawn.SpeedWalk = Data.ObjectBase[Spawn.ID].SpeedWalk;
                    Spawn.SpeedRun = Data.ObjectBase[Spawn.ID].SpeedRun;
                    Spawn.SpeedZerk = Data.ObjectBase[Spawn.ID].SpeedZerk;
                    Spawn.HP = Data.ObjectBase[model].HP;
                    Spawn.Agresif = Data.ObjectBase[model].Agresif;
                    Spawn.LocalType = 1;
                    Spawn.AutoSpawn = false;
                    Spawn.Kat = 1;
                    Systems.aRound(ref Spawn.x, ref Spawn.y, 1);
                    Systems.Objects.Add(Spawn);
                    Spawn.SpawnMe();
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public int GetStrongMobByLevel(byte Level)
        {
            try
            {
                int LevelDiff = 110;
                int NearestModel = 0;

                foreach (Global.objectdata mob in Data.ObjectBase)
                {
                    if (mob != null)
                    {
                        if (mob.Name.Contains("_STRONG_"))
                        {
                            if (LevelDiff > Math.Abs(Level - mob.Level))
                            {
                                LevelDiff = Math.Abs(Level - mob.Level);
                                NearestModel = mob.ID;
                            }
                        }
                    }
                }
                return NearestModel;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return 0;
        }
    }
}
