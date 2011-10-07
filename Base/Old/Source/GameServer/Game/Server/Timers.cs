/*    <DarkEmu GameServer>
    Copyright (C) <2011>  <DarkEmu>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Microsoft.VisualBasic;

namespace DarkEmu_GameServer
{
    class Timers
    {
        public static Timer[] PlayerAttack = new Timer[15000];
        public static Timer[] MonsterMovement = new Timer[15000];
        public static Timer[] MonsterDeath = new Timer[15000];
        public static Timer[] MonsterAttack = new Timer[15000];
        public static Timer[] CastAttackTimer = new Timer[15000];
        public static Timer[] CastBuffTimer = new Timer[15000];
        public static Timer[] UsingItemTimer = new Timer[15000];

        public static void LoadTimers()
        {
            Console.Write("Loading Timers...");
            for (int i = 0; i <= 14999; i++)
            {
                PlayerAttack[i] = new Timer();
                PlayerAttack[i].Elapsed += new ElapsedEventHandler(AttackTimer_Elapsed);
                MonsterDeath[i] = new Timer();
                MonsterDeath[i].Elapsed += new ElapsedEventHandler(MonsterDeath_Elapsed);
                CastAttackTimer[i] = new Timer();
                CastAttackTimer[i].Elapsed += new ElapsedEventHandler(CastAttackTimer_Elapsed);
                CastBuffTimer[i] = new Timer();
                CastBuffTimer[i].Elapsed += new ElapsedEventHandler(CastBuffTimer_Elapsed);
                MonsterAttack[i] = new Timer();
                MonsterAttack[i].Elapsed += new ElapsedEventHandler(MonsterAttackTimer_Elapsed);
                MonsterMovement[i] = new Timer();
                MonsterMovement[i].Elapsed += new ElapsedEventHandler(MonsterMovement_Elapsed);
                UsingItemTimer[i] = new Timer();
                UsingItemTimer[i].Elapsed += new ElapsedEventHandler(UsingItem_Elapsed);
            }
            Console.WriteLine("finished!");
        }

        public static void AttackTimer_Elapsed(object sender, EventArgs e)
        {
            Timer objB = (Timer)sender;
            int Index = -1;
            for (int i = Information.LBound(PlayerAttack, 1); i <= Information.UBound(PlayerAttack, 1); i++)
            {
                if (object.ReferenceEquals(PlayerAttack[i], objB))
                {
                    Index = i;
                    break;
                }
            }
            if (Index > -1)
            {
                if (Player.Objects[Index].NormalAttack)
                {
                    Player.General[Index].Busy = false;
                    Attack.NormalAttack(Index);
                }
                if (Player.Objects[Index].UsingSkill)
                {
                    Player.Objects[Index].UsingSkill = false;
                    Player.General[Index].Busy = false;
                    PlayerAttack[Index].Stop();
                }
            }
        }

        public static void MonsterAttackTimer_Elapsed(object sender, EventArgs e)
        {
            Timer objB = (Timer)sender;
            int Index = -1;
            for (int i = Information.LBound(MonsterAttack, 1); i <= Information.UBound(MonsterAttack, 1); i++)
            {
                if (object.ReferenceEquals(MonsterAttack[i], objB))
                {
                    Index = i;
                    break;
                }
            }
            if (Index > -1)
            {
                MonsterAction.CheckAttack(Index);
            }
        }


        public static void MonsterDeath_Elapsed(object sender, EventArgs e)
        {
            Timer objB = (Timer)sender;
            int Index = -1;
            for (int i = Information.LBound(MonsterDeath, 1); i <= Information.UBound(MonsterDeath, 1); i++)
            {
                if (object.ReferenceEquals(MonsterDeath[i], objB))
                {
                    Index = i;
                    break;
                }
            }
            if (Index > -1)
            {
                MonsterSpawn.OnDeSpawn(Index, (uint)Player.Objects[Index].AttackingObjectDeadID);
                MonsterDeath[Index].Stop();
            }
        }

        public static void MonsterMovement_Elapsed(object sender, EventArgs e)
        {
            Timer objB = (Timer)sender;
            int Index = -1;
            for (int i = Information.LBound(MonsterMovement, 1); i <= Information.UBound(MonsterMovement, 1); i++)
            {
                if (object.ReferenceEquals(MonsterMovement[i], objB))
                {
                    Index = i;
                    break;
                }
            }
            if (Index > -1)
            {
                MonsterAction.MonsterMovement(Index);
            }
        }

        public static void CastAttackTimer_Elapsed(object sender, EventArgs e)
        {
            Timer objB = (Timer)sender;
            int Index = -1;
            for (int i = Information.LBound(CastAttackTimer, 1); i <= Information.UBound(CastAttackTimer, 1); i++)
            {
                if (object.ReferenceEquals(CastAttackTimer[i], objB))
                {
                    Index = i;
                    break;
                }
            }
            if (Index > -1)
            {
                Attack.EndSkill(Index);
                CastAttackTimer[Index].Stop();
            }
        }

        public static void CastBuffTimer_Elapsed(object sender, EventArgs e)
        {
            Timer objB = (Timer)sender;
            int Index = -1;
            for (int i = Information.LBound(CastBuffTimer, 1); i <= Information.UBound(CastBuffTimer, 1); i++)
            {
                if (object.ReferenceEquals(CastBuffTimer[i], objB))
                {
                    Index = i;
                    break;
                }
            }
            if (Index > -1)
            {
                Buffs.BeginBuff(Index);
                CastBuffTimer[Index].Stop();
            }
        }

        public static void UsingItem_Elapsed(object sender, EventArgs e)
        {
            Timer objB = (Timer)sender;
            int Index = -1;
            for (int i = Information.LBound(UsingItemTimer, 1); i <= Information.UBound(UsingItemTimer, 1); i++)
            {
                if (object.ReferenceEquals(UsingItemTimer[i], objB))
                {
                    Index = i;
                    break;
                }
            }
            if (Index > -1)
            {
                Items.EquipCape(Index);
                UsingItemTimer[Index].Stop();
            }
        }
    }
}
