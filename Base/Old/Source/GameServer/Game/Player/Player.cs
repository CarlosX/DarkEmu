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

namespace DarkEmu_GameServer
{
    class Player
    {
        public static int PlayersOnline = 0;
        public struct _Stats
        {
            public uint HP, MP;
            public int CHP, CMP;
            public uint Model;
            public uint Skillpoints;
            public byte Volume;
            public byte Level;
            public ushort MinMag, MaxMag;
            public ushort MinPhy, MaxPhy;
            public ushort PhyDef, MagDef;
            public ushort Hit, Parry;
            public ushort Strength;
            public ushort Intelligence;
            public ushort Attributes;
            public ulong Gold;
            public ulong Experience;
            public byte BerserkBar;
            public ushort SkillpointBar;
            public double TotalAccessoriesAbsorption;
        }

        public struct _Flags
        {
            public byte GM;
            public byte Berserk;
            public byte Ingame;
            public sbyte PvP;
            public bool Invisible;
            public bool WearingCape;
            public bool Dead;
        }

        public struct _Speeds
        {
            public float WalkSpeed;
            public float RunSpeed;
            public float BerserkSpeed;
        }

        public struct _General
        {
            public string User;
            public string Pass;
            public long Index;
            public uint AccountID;
            public uint CharacterID;
            public uint UniqueID;
            public string CharacterName;
            public byte MaxSlots;
            public byte WeaponType;
            public bool Busy;
            public byte State;
        }

        public struct _Objects
        {
            public int SelectedObjectType;
            public uint SelectedObjectId;
            public uint AttackingObjectId;
            public bool NormalAttack;
            public bool UsingSkill;
            public uint AttackingSkillID;
            public uint UsingSkillID;
            public uint AttackingCastingID;
            public uint BuffCastingID;
            public uint AttackingObjectDeadID;
            public int SourceItemIndex;
            public int DestinationItemIndex;
            public _Buffs[] ActiveBuffs;
            public List<int> SpawnedIndex;
            public List<int> SpawnedMonsterIndex;
            public List<int> SpawnedItemsIndex;

            public struct _Buffs
            {
                public uint Id;
                public uint OverId;
            }
        }
 
        public static _Stats[] Stats = new _Stats[500];
        public static _Position[] Position = new _Position[500];
        public static _Flags[] Flags = new _Flags[500];
        public static _Speeds[] Speeds = new _Speeds[500];
        public static _General[] General = new _General[500];
        public static _Objects[] Objects = new _Objects[500];


       
    }
}
    
       