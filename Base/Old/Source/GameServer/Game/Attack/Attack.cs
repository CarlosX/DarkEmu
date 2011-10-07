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
    class Attack
    {
        private static byte GetAmountAndSkillIdByWeaponType(int Index_,ref uint SkillId)
        {
            byte Amount = 1;
            switch (Player.General[Index_].WeaponType)
            {
                case 0:
                    SkillId = 1;
                    break;

                case 2:
                case 3:
                    Amount = 2;
                    SkillId = 2;
                    break;

                case 4:
                case 5:
                    SkillId = 40;
                    break;

                case 6:
                    SkillId = 70;
                    break;

                case 7:
                    SkillId = 7127;
                    break;
                case 8:
                    SkillId = 7128;
                    break;
                case 9:
                    Amount = 2;
                    SkillId = 7129;
                    break;
                case 10:
                    SkillId = 9069;
                    break;
                case 11:
                    SkillId = 8454;
                    break;
                case 12:
                    SkillId = 7909;
                    break;
                case 13:
                    Amount = 2;
                    SkillId = 7910;
                    break;
                case 14:
                    SkillId = 9606;
                    break;
                case 15:
                    SkillId = 9970;
                    break;

            }
            return Amount;
        }
        private static Random random = new Random();

        public static void NormalAttack(int Index_)
        {
            uint SkillId = 0;
            byte AttackAmount = GetAmountAndSkillIdByWeaponType(Index_, ref SkillId);

            bool AttackingPlayer = false;
            for (int i = 0; i <= Player.PlayersOnline; i++)
            {
                if (Player.Objects[Index_].AttackingObjectId == Player.General[i].UniqueID)
                    AttackingPlayer = true;
            }

            int ObjectIndex = Players.GetObjectIndexAndType(Index_, Player.Objects[Index_].AttackingObjectId);

            Silkroad.Skill_ tmpAttackSkill = Silkroad.GetSkillById(SkillId);

            uint BasicAttackPower = 0;
            if (tmpAttackSkill.Type == Silkroad.TypeTable.Phy)
                BasicAttackPower = (uint)random.Next(Player.Stats[Index_].MinPhy, Player.Stats[Index_].MaxPhy);
            else if (tmpAttackSkill.Type == Silkroad.TypeTable.Mag)
                BasicAttackPower = (uint)random.Next(Player.Stats[Index_].MinMag, Player.Stats[Index_].MaxMag);

            double SkillAttackPower = 1;//cause its normal attack
            double SkillIncreaseRate = 0;// needs to be calculated from passive skills/active buffs

            double EnemyAbsorbation = 0;
            if (AttackingPlayer)
                EnemyAbsorbation = Player.Stats[ObjectIndex].TotalAccessoriesAbsorption / (double)100;
            
            double EnemyDefence = 0;

            if (AttackingPlayer)
            {
                if (tmpAttackSkill.Type == Silkroad.TypeTable.Phy)
                    EnemyDefence = Player.Stats[ObjectIndex].PhyDef;
                else if (tmpAttackSkill.Type == Silkroad.TypeTable.Mag)
                    EnemyDefence = Player.Stats[ObjectIndex].MagDef;
            }
            else
            {
                if (tmpAttackSkill.Type == Silkroad.TypeTable.Phy)
                    EnemyDefence = Silkroad.GetObjectById(Monsters.General[ObjectIndex].ID).PhyDef;
                else if (tmpAttackSkill.Type == Silkroad.TypeTable.Mag)
                    EnemyDefence = Silkroad.GetObjectById(Monsters.General[ObjectIndex].ID).MagDef;
            }

            double TotalDamageIncreaseRate = 0;//needs to be calculated from the players equipment

            double Damage = Formula.CalculateDmg(
                BasicAttackPower,
                SkillAttackPower,
                SkillIncreaseRate,
                EnemyAbsorbation,
                EnemyDefence,
                Player.Stats[Index_].Level,
                Player.Stats[Index_].Strength,
                Player.Stats[Index_].Intelligence,
                TotalDamageIncreaseRate,
                tmpAttackSkill.PwrPercent / (double)100);

            byte Critical = 0;

            Player.Objects[Index_].NormalAttack = true;

            if (Player.Objects[Index_].SelectedObjectType == 2)
                Timers.MonsterMovement[ObjectIndex].Stop();

            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ATTACK);
            writer.AppendByte(1);
            writer.AppendByte(2);
            writer.AppendDword(SkillId);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendDword((uint)(random.Next(1000, 10000) + Player.General[Index_].UniqueID));
            writer.AppendDword(Player.Objects[Index_].AttackingObjectId);
            writer.AppendByte(1);
            writer.AppendByte(AttackAmount);
            writer.AppendByte(1);
            writer.AppendDword(Player.Objects[Index_].AttackingObjectId);

            byte afterstate = 0;
            for (int i = 1; i <= AttackAmount; i++)
            {
                if (tmpAttackSkill.Type == Silkroad.TypeTable.Phy)
                {
                    if (random.Next(1, 10) >= 7)
                    {
                        Damage *= 2;
                        Critical = 2;
                    }
                }
                if (Player.Objects[Index_].SelectedObjectType == 2)
                {
                    Monsters.General[ObjectIndex].HP -= (int)Damage;
                    if (Monsters.General[ObjectIndex].HP < 0)
                    {
                        afterstate = 0x80;
                        Monsters.General[ObjectIndex].HP = 0;
                    }
                }
                else if (Player.Objects[Index_].SelectedObjectType == 1)
                {
                    int tmpHp = (int)(Player.Stats[ObjectIndex].CHP - Damage);
                    if (tmpHp < 0)
                    {
                        afterstate = 0x80;
                        Player.Stats[ObjectIndex].CHP = 0;
                    }
                    else
                        Player.Stats[ObjectIndex].CHP -= (int)Damage;
                }

                writer.AppendByte(afterstate);
                writer.AppendByte(Critical);
                writer.AppendDword((uint)Damage);
                writer.AppendByte(0);
                writer.AppendWord(0);
            }

            ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

            Player.General[Index_].Busy = true;
            Player.Objects[Index_].NormalAttack = true;
            Timers.PlayerAttack[Index_].Interval = 1350.0;
            Timers.PlayerAttack[Index_].Start();


            if (afterstate == 0x80)
            {
                Player.Objects[Index_].AttackingObjectDeadID = Player.Objects[Index_].AttackingObjectId;

                Player.General[Index_].Busy = false;
                Timers.PlayerAttack[Index_].Stop();
                Player.Objects[Index_].NormalAttack = false;
                if (Player.Objects[Index_].SelectedObjectType == 1)
                {
                    Player.General[ObjectIndex].State = 10;
                    Player.Objects[ObjectIndex].NormalAttack = false;
                    Player.General[ObjectIndex].Busy = false;
                    Timers.PlayerAttack[ObjectIndex].Stop();

                    Character.Die(ObjectIndex);
                    Character.Die2(ObjectIndex);
                    Player.Flags[ObjectIndex].Dead = true;
                }
                if (Player.Objects[Index_].SelectedObjectType == 2)
                {
                    Stats.GetBerserk(Index_, ObjectIndex);
                    Stats.GetXP(Index_, ObjectIndex);

                    if (Monsters.General[ObjectIndex].Type == 3)
                        Unique.OnUnique((uint)Monsters.General[ObjectIndex].ID, true, Player.General[Index_].CharacterName);

                    Monsters.General[ObjectIndex].Dead = true;

                    Timers.MonsterAttack[ObjectIndex].Stop();
                    Timers.MonsterDeath[Index_].Interval = 3000.0;
                    Timers.MonsterDeath[Index_].Start();
                }
            }
            else
            {
                if (Player.Objects[Index_].SelectedObjectType == 2)
                {
                    if (!Timers.MonsterAttack[ObjectIndex].Enabled)
                    {
                        Monsters.General[ObjectIndex].AttackingObjectIndex = Index_;
                        Timers.MonsterAttack[ObjectIndex].Interval = 2350;
                        Timers.MonsterAttack[ObjectIndex].Start();
                    }
                }
            }
        }

        public static void BeginSkill(int Index_)
        {
            Player.Objects[Index_].UsingSkill = true;

            Silkroad.Skill_ tmpSkill = Silkroad.GetSkillById(Player.Objects[Index_].AttackingSkillID);

            Player.Stats[Index_].CMP -= tmpSkill.RequiredMp;
            Stats.MPUpdate(Index_, true);

            Player.Objects[Index_].AttackingCastingID = (uint)random.Next(500, 50000);

            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SKILL_ATTACK);
            writer.AppendByte(1);
            writer.AppendByte(1);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ATTACK);
            writer.AppendWord(0x201);
            writer.AppendDword(Player.Objects[Index_].AttackingSkillID);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendDword(Player.Objects[Index_].AttackingCastingID);
            writer.AppendDword(Player.Objects[Index_].AttackingObjectId);
            writer.AppendByte(0);

            ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

            if (tmpSkill.CastTime <= 0)
                EndSkill(Index_);

            else
            {
                Timers.CastAttackTimer[Index_].Interval = tmpSkill.CastTime * 1000;
                Timers.CastAttackTimer[Index_].Start();
            }
        }


        public static void EndSkill(int Index_)
        {
            Silkroad.Skill_ tmpSkill = Silkroad.GetSkillById(Player.Objects[Index_].AttackingSkillID);
           
            bool AttackingPlayer = false;
            for (int i = 0; i <= Player.PlayersOnline; i++)
            {
                if (Player.Objects[Index_].AttackingObjectId == Player.General[i].UniqueID)
                    AttackingPlayer = true;
            }

            int ObjectIndex = Players.GetObjectIndexAndType(Index_, Player.Objects[Index_].AttackingObjectId);

            Silkroad.Skill_ tmpAttackSkill = Silkroad.GetSkillById(Player.Objects[Index_].AttackingSkillID);

            uint BasicAttackPower = 0;
            if (tmpAttackSkill.Type == Silkroad.TypeTable.Phy)
                BasicAttackPower = (uint)random.Next(Player.Stats[Index_].MinPhy, Player.Stats[Index_].MaxPhy);
            else if (tmpAttackSkill.Type == Silkroad.TypeTable.Mag)
                BasicAttackPower = (uint)random.Next(Player.Stats[Index_].MinMag, Player.Stats[Index_].MaxMag);

            double SkillAttackPower = random.Next(tmpSkill.PwrMin, tmpSkill.PwrMax);

            double SkillIncreaseRate =  0;// needs to be calculated from passive skills/active buffs

            double EnemyAbsorbation = 0;
            if (AttackingPlayer)
                EnemyAbsorbation = Player.Stats[ObjectIndex].TotalAccessoriesAbsorption / (double)100;

            double EnemyDefence = 0;

            if (AttackingPlayer)
            {
                if (tmpAttackSkill.Type == Silkroad.TypeTable.Phy)
                    EnemyDefence = Player.Stats[ObjectIndex].PhyDef;
                else if (tmpAttackSkill.Type == Silkroad.TypeTable.Mag)
                    EnemyDefence = Player.Stats[ObjectIndex].MagDef;
            }
            else
            {
                if (tmpAttackSkill.Type == Silkroad.TypeTable.Phy)
                    EnemyDefence = Silkroad.GetObjectById(Monsters.General[ObjectIndex].ID).PhyDef;
                else if (tmpAttackSkill.Type == Silkroad.TypeTable.Mag)
                    EnemyDefence = Silkroad.GetObjectById(Monsters.General[ObjectIndex].ID).MagDef;
            }

            double TotalDamageIncreaseRate = 0;//needs to be calculated from the players equipment

            double Damage = Formula.CalculateDmg(
                BasicAttackPower,
                SkillAttackPower,
                SkillIncreaseRate,
                EnemyAbsorbation,
                EnemyDefence,
                Player.Stats[Index_].Level,
                Player.Stats[Index_].Strength,
                Player.Stats[Index_].Intelligence,
                TotalDamageIncreaseRate,
                tmpAttackSkill.PwrPercent / (double)100);

            byte Critical = 1;
            byte AfterState = 0;

            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_END_SKILL);
            writer.AppendByte(1);
            writer.AppendDword(Player.Objects[Index_].AttackingCastingID);
            writer.AppendDword(Player.Objects[Index_].AttackingObjectId);
            writer.AppendByte(1);
            writer.AppendByte(tmpSkill.NumberOfAttacks);
            writer.AppendByte(1);
            writer.AppendDword(Player.Objects[Index_].AttackingObjectId);

            for (int i = 0; i < tmpSkill.NumberOfAttacks; i++)
            {
                if (i > 0)
                {
                    Player.Objects[Index_].AttackingSkillID = tmpSkill.NextId;
                    Silkroad.Skill_ tmp = Silkroad.GetSkillById(tmpSkill.NextId);
                    SkillAttackPower = random.Next(tmp.PwrMin, tmp.PwrMax);                         
                    Damage = Formula.CalculateDmg(
                        BasicAttackPower,
                        SkillAttackPower,
                        SkillIncreaseRate,
                        EnemyAbsorbation,
                        EnemyDefence,
                        Player.Stats[Index_].Level,
                        Player.Stats[Index_].Strength,
                        Player.Stats[Index_].Intelligence,
                        TotalDamageIncreaseRate,
                        tmp.PwrPercent / (double)100);
                }

                if (tmpAttackSkill.Type == Silkroad.TypeTable.Phy)
                {
                    if (random.Next(1, 10) >= 7)
                    {
                        Critical = 2;
                        Damage *= 2;
                    }
                }

                if (Player.Objects[Index_].SelectedObjectType == 2)
                {
                    Monsters.General[ObjectIndex].HP -= (int)Damage;
                    if (Monsters.General[ObjectIndex].HP <= 0)
                    {
                        AfterState = 0x80;
                        Monsters.General[ObjectIndex].HP = 0;
                    }
                }
                else if (Player.Objects[Index_].SelectedObjectType == 1)
                {
                    int tmpHp = (int)(Player.Stats[ObjectIndex].CHP - Damage);
                    if (tmpHp < 0)
                    {
                        AfterState = 0x80;
                        Player.Stats[ObjectIndex].CHP = 0;
                    }
                    else
                        Player.Stats[ObjectIndex].CHP -= (int)Damage;
                }

                writer.AppendByte(AfterState);
                writer.AppendByte(Critical);
                writer.AppendDword((uint)Damage);
                writer.AppendByte(0);
                writer.AppendWord(0);
            }

            ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SKILL_ATTACK);
            writer.AppendByte(2);
            writer.AppendByte(0);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            Player.Objects[Index_].NormalAttack = false;
            Player.General[Index_].Busy = true;
            Timers.PlayerAttack[Index_].Interval = tmpSkill.NumberOfAttacks * 1000;
            Timers.PlayerAttack[Index_].Start();

            if (AfterState == 0x80)
            {
                Player.General[Index_].Busy = false;
                Timers.PlayerAttack[Index_].Stop();
                Player.Objects[Index_].NormalAttack = false;
                Player.Objects[Index_].UsingSkill = false;
                Player.Objects[Index_].AttackingObjectDeadID = Player.Objects[Index_].AttackingObjectId;

                if (Player.Objects[Index_].SelectedObjectType == 1)
                {
                    Player.General[Index_].State = 10;
                    Player.Objects[Index_].NormalAttack = false;
                    Player.General[Index_].Busy = false;
                    Timers.PlayerAttack[ObjectIndex].Stop();

                    Character.Die(ObjectIndex);
                    Character.Die2(ObjectIndex);

                    Player.Flags[ObjectIndex].Dead = true;
                }
                if (Player.Objects[Index_].SelectedObjectType == 2)
                {
                    Stats.GetBerserk(Index_, ObjectIndex);
                    Stats.GetXP(Index_, ObjectIndex);                    

                    if (Monsters.General[ObjectIndex].Type == 3)                    
                        Unique.OnUnique((uint)Monsters.General[ObjectIndex].ID, true, Player.General[Index_].CharacterName);

                    Monsters.General[ObjectIndex].Dead = true;
                    
                    Timers.MonsterDeath[Index_].Interval = 3000.0;
                    Timers.MonsterDeath[Index_].Start();
                }
                Timers.MonsterAttack[ObjectIndex].Stop();
            }
            else
            {
                if (Player.Objects[Index_].SelectedObjectType == 2)
                {
                    if (!Timers.MonsterAttack[ObjectIndex].Enabled)
                    {
                        Monsters.General[ObjectIndex].AttackingObjectIndex = Index_;
                        Timers.MonsterAttack[ObjectIndex].Interval = 2350;
                        Timers.MonsterAttack[ObjectIndex].Start();
                    }
                }
            }
        }

        public static void OnMonsterAttack(int Index_)
        {
            uint SkillId = 161;

            if (Monsters.General[Index_].Skills.Count != 0)
            {
                int rnd = random.Next(0, Monsters.General[Index_].Skills.Count);
                SkillId = (uint)Monsters.General[Index_].Skills[rnd];
            }

            if (Player.General[Monsters.General[Index_].AttackingObjectIndex].UniqueID != 0)
            {
             /*   Silkroad.Skill_ tmpAttackSkill = Silkroad.GetSkillById(SkillId);
                Silkroad.Object_ tmpMonster = Silkroad.GetObjectById(Monsters.General[Index_].ID);

                uint BasicAttackPower = (uint)(tmpMonster.ParryRatio * 10);//parry ratio == att ratio?
 
                double SkillAttackPower = 1;
                double SkillIncreaseRate = 0;

                double EnemyAbsorbation = Player.Stats[Monsters.General[Index_].AttackingObjectIndex].TotalAccessoriesAbsorption / (double)100;     
                double EnemyDefence =( Player.Stats[Monsters.General[Index_].AttackingObjectIndex].PhyDef + Player.Stats[Monsters.General[Index_].AttackingObjectIndex].MagDef) / 2;
                double TotalDamageIncreaseRate = 0;

                double Damage = Formula.CalculateDmg(
                    BasicAttackPower,
                    SkillAttackPower,
                    SkillIncreaseRate,
                    EnemyAbsorbation,
                    EnemyDefence,
                    tmpMonster.Level,
                    0,
                   0,
                    TotalDamageIncreaseRate,
                    tmpAttackSkill.PwrPercent / (double)100);

                Console.WriteLine(Damage);*/
                double Damage = (double)random.Next(1, 100);

                if (Monsters.General[Index_].Type == 3 || Monsters.General[Index_].Type == 0x10 || Monsters.General[Index_].Type == 20)
                    Damage *= 2;

                Player.Stats[Monsters.General[Index_].AttackingObjectIndex].CHP -= (int)Damage;
                byte AfterState = 0;

                if (Player.Stats[Monsters.General[Index_].AttackingObjectIndex].CHP <= 0)
                {
                    AfterState = 0x80;
                    Player.Stats[Monsters.General[Index_].AttackingObjectIndex].CHP = 0;
                }
                Stats.HPUpdate(Monsters.General[Index_].AttackingObjectIndex, true);

                PacketWriter writer = new PacketWriter();
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_ATTACK);
                writer.AppendByte(1);
                writer.AppendByte(2);
                writer.AppendDword(SkillId);
                writer.AppendDword(Monsters.General[Index_].UniqueID);
                writer.AppendDword((uint)random.Next(65536, 1048575));
                writer.AppendDword(Player.General[Monsters.General[Index_].AttackingObjectIndex].UniqueID);
                writer.AppendByte(1);
                writer.AppendByte(1);
                writer.AppendByte(1);
                writer.AppendDword(Player.General[Monsters.General[Index_].AttackingObjectIndex].UniqueID);
                writer.AppendByte(AfterState);
                writer.AppendByte(1);
                writer.AppendDword((uint)Damage);
                writer.AppendByte(0);
                writer.AppendWord(0);

                ServerSocket.SendPacketIfMonsterIsSpawned(writer.getWorkspace(), Index_);

                if (AfterState == 0x80)
                {
                    Timers.MonsterAttack[Index_].Stop();
                    Player.General[Index_].State = 1;
                    Character.Die(Monsters.General[Index_].AttackingObjectIndex);
                    Character.Die2(Monsters.General[Index_].AttackingObjectIndex);
                    Player.Flags[Monsters.General[Index_].AttackingObjectIndex].Dead = true;
                }
            }
        }

    }

}
