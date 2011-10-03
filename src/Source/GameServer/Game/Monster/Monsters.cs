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
using System.Collections;
using System.Text;

namespace DarkEmu_GameServer
{
    class Monsters
    {
        public static int MonsterAmount = 0;
        public struct _General
        {
            public int HP;
            public uint ID;
            public byte Type;
            public uint UniqueID;
            public ulong Exp;
            public byte Level;
            public ArrayList Skills;
            public bool Dead;
            public int AttackingObjectIndex;
        }
        public static _General[] General = new _General[15000];
        public static _Position[] Position = new _Position[15000];
   
    }

}
