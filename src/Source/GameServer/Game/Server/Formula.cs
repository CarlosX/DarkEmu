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
    class Formula
    {
        private static Random random = new Random();   
        public static double CalculateDmg(double BasicAP,double SkillAP,double APIncreaseRate,double EnemyAbsorbation,double EnemyDefence,int Level,int Str,int Int,double TotalDMGIncreaseRate,double SkillAPRate)
        {
            double M = 28 + Level * 4;
            double PhyBal = 100 - 100 * 2 / 3 * (M - Str) / M;
            double MagBal = 100 * Int / M;
            double BalanceRate = PhyBal / MagBal;

            double Damage = ((BasicAP + SkillAP) * (1 + APIncreaseRate) / (1 + EnemyAbsorbation) - EnemyDefence) * BalanceRate * (1 + TotalDMGIncreaseRate) * SkillAPRate;

            if (Damage < 0)            
                Damage = 1;            
            return Damage;
        }
     
        public static void CalculateHP(int Index_)
        {
            Player.Stats[Index_].HP = (uint)((double)Math.Pow(1.02, (Player.Stats[Index_].Level - 1)) * Player.Stats[Index_].Strength * 10); 
          //  Database.ChangeData("update characters set hp='" + Player.Stats[Index_].HP + "' where name='" + Player.General[Index_].CharacterName + "'");
        }

        public static void CalculateMP(int Index_)
        {      
            Player.Stats[Index_].MP = (uint)((double)Math.Pow(1.02, (Player.Stats[Index_].Level - 1)) * Player.Stats[Index_].Intelligence * 10);
            //Database.ChangeData("update characters set mp='" + Player.Stats[Index_].MP + "' where name='" + Player.General[Index_].CharacterName + "'");
        }     

        public static double CalculateDistance(double distance_x, double distance_y)
        {            
            double x;
            if ((distance_x < 0) && (distance_x != 0))
            {
                x = distance_x * -1;
            }
            else
            {
                x = distance_x;
            }
            if ((distance_y < 0) && (distance_y != 0))
            {
                return ((distance_y * -1) + x);
            }
            return (x + distance_y);
        }

        public static double CalculateDistance(_Position Object_1, _Position Object_2)
        {
            double distance_x = Object_1.X - Object_2.X;
            double distance_y = Object_1.Y - Object_2.Y;

            double x = 0;
            if (distance_x < 0 && distance_x != 0)            
                x = distance_x * -1;            
            else            
                x = distance_x;            
            if (distance_y < 0 && distance_y != 0)            
                return ((distance_y * -1) + x);            
            return x + distance_y;
        }
    }
}
