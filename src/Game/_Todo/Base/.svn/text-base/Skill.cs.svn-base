///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;

namespace SrxRevo.Base
{
    class Skill
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Using skill details
        /////////////////////////////////////////////////////////////////////////////////
        #region Using skills
        public static SrxRevo.character._usingSkill Info(int SkillID, character Character)
        {
            //Create new global information
            SrxRevo.character._usingSkill info = new SrxRevo.character._usingSkill();
            //Wrap our function inside a catcher
            try
            {
                //Set default skill information
                info.MainSkill = SkillID;
                info.SkillID = new int[10];
                info.FoundID = new int[10];
                info.TargetType = new bool[10];
                info.NumberOfAttack = NumberAttack(SkillID, ref info.SkillID);
                info.Targethits = 1;
                info.Distance = Convert.ToByte(Data.SkillBase[SkillID].Distance);
                info.Tdistance = 0;
                info.canUse = true;
                //Switch on skills series
                switch (Data.SkillBase[SkillID].Series)
                {
                    case "SKILL_EU_ROG_TRANSFORMA_MASK_A":
                        break;
                    #region Bicheon
                    #region Smashing Series
                    case "SKILL_CH_SWORD_SMASH_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_SMASH_B":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_SMASH_C":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_SMASH_D":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_SMASH_E":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_SMASH_F":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    #endregion
                    #region Chain Sword Attack Series
                    case "SKILL_CH_SWORD_CHAIN_A":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_B":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_C":
                        info.NumberOfAttack = 5;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_D":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_E":
                        info.NumberOfAttack = 5;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_F":
                        info.NumberOfAttack = 6;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_G":
                        info.NumberOfAttack = 6;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_CHAIN_H":
                        info.NumberOfAttack = 7;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.SkillID[7] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Blade Force Series
                    case "SKILL_CH_SWORD_GEOMGI_A":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_GEOMGI_B":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_GEOMGI_C":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_GEOMGI_D":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_GEOMGI_E":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SWORD_GEOMGI_F":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Hidden Blade Series
                    case "SKILL_CH_SWORD_KNOCKDOWN_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 4;
                        break;
                    case "SKILL_CH_SWORD_KNOCKDOWN_B":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        info.OzelEffect = 4;
                        break;
                    case "SKILL_CH_SWORD_KNOCKDOWN_C":
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 4;
                        break;
                    case "SKILL_CH_SWORD_KNOCKDOWN_D":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 3;
                        info.OzelEffect = 4;
                        break;
                    case "SKILL_CH_SWORD_KNOCKDOWN_E":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    #endregion
                    #region Killing Heaven Blade Series
                    case "SKILL_CH_SWORD_DOWNATTACK_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    case "SKILL_CH_SWORD_DOWNATTACK_B":
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    case "SKILL_CH_SWORD_DOWNATTACK_C":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    case "SKILL_CH_SWORD_DOWNATTACK_D":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    case "SKILL_CH_SWORD_DOWNATTACK_E":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    case "SKILL_CH_SWORD_DOWNATTACK_F":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.OzelEffect = 5;
                        break;
                    #endregion
                    #region Sword Dance Series
                    case "SKILL_CH_SWORD_SPECIAL_A":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 7;
                        break;
                    case "SKILL_CH_SWORD_SPECIAL_B":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 15;
                        break;
                    case "SKILL_CH_SWORD_SPECIAL_C":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 15;
                        break;
                    case "SKILL_CH_SWORD_SPECIAL_D":
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 15;
                        break;
                    case "SKILL_CH_SWORD_SPECIAL_E":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.Distance = 4;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 15;
                        break;
                    #endregion
                    #endregion

                    #region Heuksal
                    #region Annihilating Blade Series
                    case "SKILL_CH_SPEAR_PIERCE_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_PIERCE_B":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_PIERCE_C":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_PIERCE_D":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_PIERCE_E":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_PIERCE_F":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Heuksal Spear Series
                    case "SKILL_CH_SPEAR_FRONTAREA_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 1;
                        break;
                    case "SKILL_CH_SPEAR_FRONTAREA_B":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 1;
                        break;
                    case "SKILL_CH_SPEAR_FRONTAREA_C":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 1;
                        break;
                    case "SKILL_CH_SPEAR_FRONTAREA_D":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    case "SKILL_CH_SPEAR_FRONTAREA_E":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    case "SKILL_CH_SPEAR_FRONTAREA_F":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 3;
                        break;
                    #endregion
                    #region Soul Departs Spear Series
                    case "SKILL_CH_SPEAR_STUN_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_STUN_B":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_STUN_C":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 2;
                        break;
                    case "SKILL_CH_SPEAR_STUN_D":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 2;
                        break;
                    case "SKILL_CH_SPEAR_STUN_E":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        break;
                    case "SKILL_CH_SPEAR_STUN_F":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        break;
                    #endregion
                    #region Ghost Spear Attack Series
                    case "SKILL_CH_SPEAR_ROUNDAREA_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 2;
                        break;
                    case "SKILL_CH_SPEAR_ROUNDAREA_B":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 3;
                        break;
                    case "SKILL_CH_SPEAR_ROUNDAREA_C":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 4;
                        break;
                    case "SKILL_CH_SPEAR_ROUNDAREA_D":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_CH_SPEAR_ROUNDAREA_E":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_CH_SPEAR_ROUNDAREA_F":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 5;
                        break;
                    #endregion
                    #region Chain Spear Attack Series
                    case "SKILL_CH_SPEAR_CHAIN_A":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_CHAIN_B":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_CHAIN_C":
                        info.NumberOfAttack = 5;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    case "SKILL_CH_SPEAR_CHAIN_D":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_CHAIN_E":
                        info.NumberOfAttack = 6;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 3;
                        break;
                    case "SKILL_CH_SPEAR_CHAIN_F":
                        info.NumberOfAttack = 6;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 3;
                        break;
                    case "SKILL_CH_SPEAR_CHAIN_G":
                        info.NumberOfAttack = 7;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.SkillID[7] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 3;
                        break;
                    #endregion
                    #region Flying Dragon Spear Series
                    case "SKILL_CH_SPEAR_SHOOT_A":
                        info.Instant = 0;
                        info.Distance = 6;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_SHOOT_B":
                        info.Instant = 0;
                        info.Distance = 6;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_SHOOT_C":
                        info.Instant = 0;
                        info.Distance = 6;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_SHOOT_D":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.Distance = 6;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_SPEAR_SHOOT_E":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.Distance = 6;
                        info.P_M = false;
                        break;
                    #endregion
                    #endregion

                    #region Pacheon
                    #region Anti Devil Bow Series
                    case "SKILL_CH_BOW_CRITICAL_A":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CRITICAL_B":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CRITICAL_C":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CRITICAL_D":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CRITICAL_E":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CRITICAL_F":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CRITICAL_G":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Arrow Combo Attack Series
                    case "SKILL_CH_BOW_CHAIN_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CHAIN_B":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CHAIN_C":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CHAIN_D":
                        info.NumberOfAttack = 5;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.Instant = 0;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CHAIN_E":
                        info.NumberOfAttack = 6;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.Instant = 0;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_CHAIN_F":
                        info.NumberOfAttack = 7;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.SkillID[7] = SkillID;
                        info.Instant = 0;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Autumn Wind Arrow Series
                    case "SKILL_CH_BOW_PIERCE_A":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_PIERCE_B":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_PIERCE_C":
                        info.Instant = 2;
                        info.Distance = 7;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_PIERCE_D":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_PIERCE_E":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_PIERCE_F":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Explosion Arrow Series
                    case "SKILL_CH_BOW_AREA_A":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 4;
                        break;
                    case "SKILL_CH_BOW_AREA_B":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 4;
                        break;
                    case "SKILL_CH_BOW_AREA_C":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 4;
                        break;
                    case "SKILL_CH_BOW_AREA_D":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_CH_BOW_AREA_E":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 6;
                        break;
                    #endregion
                    #region Strong Bow Series
                    case "SKILL_CH_BOW_POWER_A":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_POWER_B":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_CH_BOW_POWER_C":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Tdistance = 4;
                        break;
                    case "SKILL_CH_BOW_POWER_D":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_CH_BOW_POWER_E":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Tdistance = 6;
                        break;
                    #endregion
                    #region Mind Bow Series
                    case "SKILL_CH_BOW_SPECIAL_A":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Targethits = 2;
                        info.Tdistance = 20;
                        break;
                    case "SKILL_CH_BOW_SPECIAL_B":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 22;
                        break;
                    case "SKILL_CH_BOW_SPECIAL_C":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 23;
                        break;
                    case "SKILL_CH_BOW_SPECIAL_D":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 24;
                        break;
                    #endregion
                    #endregion

                    #region Cold
                    #region Snow Storm Series
                    case "SKILL_CH_COLD_GIGONGSUL_A":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = true;

                        break;
                    case "SKILL_CH_COLD_GIGONGSUL_B":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 6;
                        break;
                    case "SKILL_CH_COLD_GIGONGSUL_C":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = true;
                        break;
                    case "SKILL_CH_COLD_GIGONGSUL_D":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 7;
                        break;
                    case "SKILL_CH_COLD_GIGONGSUL_E":
                        info.Instant = 2;
                        info.Distance = 15;
                        info.P_M = true;
                        break;
                    #endregion
                    #endregion

                    #region Light
                    #region Lion Shout Series
                    case "SKILL_CH_LIGHTNING_CHUNDUNG_A":
                    case "SKILL_CH_LIGHTNING_CHUNDUNG_B":
                    case "SKILL_CH_LIGHTNING_CHUNDUNG_C":
                    case "SKILL_CH_LIGHTNING_CHUNDUNG_D":
                    case "SKILL_CH_LIGHTNING_CHUNDUNG_E":
                    case "SKILL_CH_LIGHTNING_CHUNDUNG_F":
                        info.Instant = 0;
                        info.Distance = 10;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    #endregion
                    #region Thunderbolt Force Series
                    case "SKILL_CH_LIGHTNING_STORM_A":
                    case "SKILL_CH_LIGHTNING_STORM_B":
                    case "SKILL_CH_LIGHTNING_STORM_C":
                    case "SKILL_CH_LIGHTNING_STORM_D":
                    case "SKILL_CH_LIGHTNING_STORM_E":
                        info.Instant = 2;
                        info.Distance = 12;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    #endregion
                    #endregion

                    #region Fire
                    #region Flame Wave Series
                    case "SKILL_CH_FIRE_GIGONGSUL_A":
                    case "SKILL_CH_FIRE_GIGONGSUL_B":
                    case "SKILL_CH_FIRE_GIGONGSUL_D":
                    case "SKILL_CH_FIRE_GIGONGSUL_E":
                    case "SKILL_CH_FIRE_GIGONGSUL_G":
                        info.Instant = 2;
                        info.Distance = 12;
                        info.P_M = true;
                        break;
                    case "SKILL_CH_FIRE_GIGONGSUL_F":
                    case "SKILL_CH_FIRE_GIGONGSUL_C":
                        info.Instant = 2;
                        info.Distance = 12;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 6;
                        break;
                    #endregion
                    #endregion

                    #region Force
                    case "SKILL_CH_WATER_CURE_A":
                    case "SKILL_CH_WATER_CURE_B":
                    case "SKILL_CH_WATER_CURE_C":
                    case "SKILL_CH_WATER_CURE_D":
                    case "SKILL_CH_WATER_CURE_E":
                    case "SKILL_CH_WATER_CURE_F":
                    case "SKILL_CH_WATER_HEAL_A":
                    case "SKILL_CH_WATER_HEAL_B":
                    case "SKILL_CH_WATER_HEAL_C":
                    case "SKILL_CH_WATER_HEAL_D":
                    case "SKILL_CH_WATER_HEAL_E":
                    case "SKILL_CH_WATER_HEAL_F":
                        if (Character.Action.Object != null || Character.Action.Object.GetType().ToString() == "SrxRevo.Systems") info.canUse = false;

                        break;
                    case "SKILL_CH_WATER_CANCEL_A":
                    case "SKILL_CH_WATER_CANCEL_B":
                    case "SKILL_CH_WATER_CANCEL_C":
                    case "SKILL_CH_WATER_CANCEL_D":
                    case "SKILL_CH_WATER_CANCEL_E":
                    case "SKILL_CH_WATER_CANCEL_F":
                    case "SKILL_CH_WATER_CANCEL_G":
                    case "SKILL_CH_WATER_CANCEL_H":
                        if (Character.Action.Object != null || Character.Action.Object.GetType().ToString() == "SrxRevo.Systems") info.canUse = false;

                        break;
                    case "SKILL_CH_WATER_RESURRECTION_A":
                    case "SKILL_CH_WATER_RESURRECTION_B":
                    case "SKILL_CH_WATER_RESURRECTION_C":
                    case "SKILL_CH_WATER_RESURRECTION_D":
                    case "SKILL_CH_WATER_RESURRECTION_E":
                        if (Character.Action.Object != null || Character.Action.Object.GetType().ToString() == "SrxRevo.Systems") info.canUse = false;

                        break;
                    #endregion

                    #region Europe
                    #region Melee
                    #region Europe Warrior
                    #region One-Handed
                    case "SKILL_EU_WARRIOR_ONEHANDA_STRIKE_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_SHIELD_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_SHIELD_B":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_PIERCE_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_PIERCE_B":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_CRITICAL_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_ONEHANDA_CRITICAL_B":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Two-Handed
                    case "SKILL_EU_WARRIOR_TWOHANDA_DASH_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 1;
                        break;
                    case "SKILL_EU_WARRIOR_TWOHANDA_RISING_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_TWOHANDA_CHARGE_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_TWOHANDA_CHARGE_B":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_TWOHANDA_CRY_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        break;
                    case "SKILL_EU_WARRIOR_TWOHANDA_CRY_B":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        break;
                    #endregion
                    #region Axe
                    case "SKILL_EU_WARRIOR_DUALA_CROSS_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_TWIST_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_TWIST_B":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_STUN_A":
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 8;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_COUNTER_A":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_COUNTER_B":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 2;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_WHIRLWIND_A":
                        info.NumberOfAttack = 4;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 1;
                        break;
                    case "SKILL_EU_WARRIOR_DUALA_WHIRLWIND_B":
                        info.NumberOfAttack = 5;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 1;
                        break;
                    #endregion
                    #region Warrior Others
                    case "SKILL_EU_WARRIOR_FRENZYA_TOUNT_A":
                        info.Instant = 0;
                        info.Distance = 15;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_EU_WARRIOR_FRENZYA_TOUNT_SPRINT_A":
                        info.Instant = 0;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    #endregion
                    #endregion
                    #region Europe Rogue
                    #region Rogue
                    case "SKILL_EU_ROG_BOWA_POWER_A":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_BOWA_POWER_B":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_BOWA_FAST_A":
                        info.Instant = 0;
                        info.Distance = 30;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_BOWA_FAST_B":
                        info.Instant = 0;
                        info.Distance = 30;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_ROG_BOWA_RANGE_A":
                        info.Instant = 1;
                        info.Distance = 22;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_BOWA_RANGE_B":
                        info.Instant = 1;
                        info.Distance = 22;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_BOWA_KNOCK_A":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_BOWA_KNOCK_B":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = false;
                        break;
                    #endregion
                    #region Dagger
                    case "SKILL_EU_ROG_DAGGERA_CHAIN_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_DAGGERA_WOUND_A":
                        info.NumberOfAttack = 2;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_DAGGERA_WOUND_B":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_DAGGERA_SCREW_A":
                        info.Instant = 1;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_DAGGERA_SLASH_A":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_DAGGERA_SLASH_B":
                        info.NumberOfAttack = 5;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_ROG_STEALTHA_ATTACK_A":
                        info.Instant = 0;
                        info.P_M = false;
                        break;
                    #endregion
                    #endregion
                    #endregion

                    #region Caster
                    #region Europe Wizard
                    #region Earth
                    case "SKILL_EU_WIZARD_EARTHA_POINT_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_EU_WIZARD_EARTHA_POINT_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_EU_WIZARD_EARTHA_AREA_A":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_EARTHA_AREA_B":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_EARTHA_ABNORMAL_A":
                        info.Instant = 1;
                        info.Distance = 10;
                        info.P_M = true;
                        break;
                    case "SKILL_EU_WIZARD_EARTHA_ABNORMAL_B":
                        info.Instant = 1;
                        info.Distance = 10;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 5;
                        break;
                    #endregion
                    #region Cold
                    case "SKILL_EU_WIZARD_COLDA_POINT_A":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        break;
                    case "SKILL_EU_WIZARD_COLDA_POINT_B":
                        info.NumberOfAttack = 3;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        break;
                    case "SKILL_EU_WIZARD_COLDA_AREA_A":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_COLDA_AREA_B":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 15;
                        break;
                    case "SKILL_EU_WIZARD_COLDA_MANADRY_A":
                        info.Instant = 1;
                        info.Distance = 10;
                        info.P_M = true;
                        break;
                    case "SKILL_EU_WIZARD_COLDA_MANADRY_B":
                        info.Instant = 1;
                        info.Distance = 10;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 8;
                        break;
                    #endregion
                    #region Fire
                    case "SKILL_EU_WIZARD_FIREA_POINT_A":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        break;
                    case "SKILL_EU_WIZARD_FIREA_POINT_B":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 3;
                        break;
                    case "SKILL_EU_WIZARD_FIREA_SPRAY_A":
                        info.NumberOfAttack = 7;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.SkillID[7] = SkillID;
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 3;
                        break;
                    case "SKILL_EU_WIZARD_FIREA_SPRAY_B":
                        info.NumberOfAttack = 7;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.SkillID[7] = SkillID;
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 3;
                        break;
                    case "SKILL_EU_WIZARD_FIREA_TRAP_A":
                        info.Instant = 1;
                        info.Distance = 10;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 7;
                        break;
                    case "SKILL_EU_WIZARD_FIREA_TRAP_B":
                        info.Instant = 1;
                        info.Distance = 10;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 7;
                        break;
                    #endregion
                    #region Light
                    case "SKILL_EU_WIZARD_PSYCHICA_LIGHT_A":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 2;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_PSYCHICA_LIGHT_B":
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_PSYCHICA_AREA_A":
                        info.NumberOfAttack = 6;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_PSYCHICA_AREA_B":
                        info.NumberOfAttack = 6;
                        info.SkillID[1] = SkillID;
                        info.SkillID[2] = SkillID;
                        info.SkillID[3] = SkillID;
                        info.SkillID[4] = SkillID;
                        info.SkillID[5] = SkillID;
                        info.SkillID[6] = SkillID;
                        info.Instant = 1;
                        info.Distance = 15;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WIZARD_PSYCHICA_UNTOUCH_A":
                        info.Instant = 1;
                        info.Distance = 10;
                        info.P_M = true;
                        break;
                    case "SKILL_EU_WIZARD_PSYCHICA_UNTOUCH_B":
                        info.Instant = 1;
                        info.Distance = 10;
                        info.P_M = true;
                        info.Targethits = 5;
                        info.Tdistance = 12;
                        break;
                    #endregion
                    #endregion
                    #region Europe Warlock
                    #region Dark Mentalist
                    case "SKILL_EU_WARLOCK_DOTA_BURN_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_DOTA_BURN_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 7;
                        break;
                    case "SKILL_EU_WARLOCK_DOTA_POISON_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_DOTA_POISON_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 7;
                        break;
                    case "SKILL_EU_WARLOCK_DOTA_BLOODING_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_DOTA_BLOODING_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 7;
                        break;
                    case "SKILL_EU_WARLOCK_DOTA_DISEASE_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_DOTA_DISEASE_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 7;
                        break;
                    #endregion
                    #region Raze
                    case "SKILL_EU_WARLOCK_RAZEA_PHYSICAL_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_RAZEA_PHYSICAL_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WARLOCK_RAZEA_MAGICAL_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_RAZEA_MAGICAL_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WARLOCK_RAZEA_STR_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_RAZEA_STR_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WARLOCK_RAZEA_INT_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_RAZEA_INT_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    #endregion
                    #region Blood
                    case "SKILL_EU_WARLOCK_BLOODA_POINT_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_BLOODA_POINT_B":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_EU_WARLOCK_BLOODA_EXPLOSION_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_BLOODA_EXPLOSION_B":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    #endregion
                    #region Sould Pressure
                    case "SKILL_EU_WARLOCK_SOULA_STUN_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_SOULA_STUN_B":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WARLOCK_CONFUSIONA_ILLUSION_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_CONFUSIONA_ILLUSION_B":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_CONFUSIONA_RANGE_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_CONFUSIONA_RANGE_B":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    #endregion
                    #region Cruel Spell
                    case "SKILL_EU_WARLOCK_BLOODA_LIFEDRAIN_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_BLOODA_LIFEDRAIN_B":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 15;
                        break;
                    case "SKILL_EU_WARLOCK_SOULA_CHAOS_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_EU_WARLOCK_SOULA_CHAOS_B":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_EU_WARLOCK_SOULA_STUNLINK_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_SOULA_RETURN_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_SOULA_RETURN_B":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 8;
                        info.Tdistance = 30;
                        break;
                    #endregion
                    #region Dim Haze
                    case "SKILL_EU_WARLOCK_SOULA_MEZA_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_SOULA_MEZA_B":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WARLOCK_CONFUSIONA_HEAL_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_WARLOCK_CONFUSIONA_HEAL_B":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 10;
                        break;
                    case "SKILL_EU_WARLOCK_CONFUSIONA_AGGROLOW_A":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 4;
                        info.Tdistance = 25;
                        break;
                    case "SKILL_EU_WARLOCK_CONFUSIONA_AGGROLOW_B":
                        info.Instant = 0;
                        info.Distance = 0;
                        info.P_M = false;
                        info.Targethits = 8;
                        info.Tdistance = 30;
                        break;
                    #endregion
                    #endregion
                    #endregion

                    #region Buff
                    #region Europe Bard
                    #region Battle Chord
                    case "SKILL_EU_BARD_BATTLAA_DAMAGE_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_BARD_BATTLAA_DAMAGE_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_EU_BARD_BATTLAA_MPSTEAL_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    #endregion
                    #endregion
                    #region Europe Cleric
                    #region Cardinal Praise
                    case "SKILL_EU_CLERIC_BATTLEA_CROSS_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_CLERIC_BATTLEA_CROSS_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 5;
                        break;
                    #endregion
                    #region Mortal Recovery
                    case "SKILL_EU_CLERIC_BATTLEA_OVERHEAL_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        break;
                    case "SKILL_EU_CLERIC_BATTLEA_OVERHEAL_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 3;
                        info.Tdistance = 5;
                        break;
                    #endregion
                    #region Sacrifice
                    case "SKILL_EU_CLERIC_BATTLEA_SACRIFICE_A":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 5;
                        break;
                    case "SKILL_EU_CLERIC_BATTLEA_SACRIFICE_B":
                        info.Instant = 1;
                        info.Distance = 5;
                        info.P_M = false;
                        info.Targethits = 5;
                        info.Tdistance = 8;
                        break;
                    #endregion
                    #endregion
                    #endregion
                        #endregion
                    default:
                        //Set default for skills that havent been added
                        info.Targethits = info.NumberOfAttack;
                        info.Tdistance = 0;
                        info.Instant = 2;
                        info.P_M = false;
                        info.canUse = true;
                        //Write info if skill not added
                        Console.WriteLine("Number of attacks : " + info.NumberOfAttack + " Skillname: " + Data.SkillBase[SkillID].Series);
                        break;
                }
                //Return information
                return info;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Using skill error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            return info;
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Using skill count number of attacks
        /////////////////////////////////////////////////////////////////////////////////
        #region Number of attacks
        public static byte NumberAttack(int SkillID, ref int[] ID)
        {
            //Set default values information
            byte NumberAttack = 0;
            int Next1 = Data.SkillBase[SkillID].NextSkill;
            ID[NumberAttack] = SkillID;
            NumberAttack++;
            //Wrap our function inside a catcher
            try
            {
                //If next isnt null
                if (Next1 != 0)
                {
                    //Set id + number of attack +
                    ID[NumberAttack] = Next1;
                    NumberAttack++;
                    while (Next1 != 0)
                    {
                        if (Data.SkillBase[Next1].NextSkill != 0)
                        {
                            ID[NumberAttack] = Data.SkillBase[Next1].NextSkill;
                            NumberAttack++;
                            Next1 = Data.SkillBase[Next1].NextSkill;
                        }
                        else
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Skill count number of attack error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            return NumberAttack;
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Using skill check weapon
        /////////////////////////////////////////////////////////////////////////////////
        #region Check weapon
        public static bool CheckWeapon(int itemid, int skillid)
        {
            //Wrap our function inside a catcher
            try
            {

                if (Data.SkillBase[skillid].Weapon1 == 255)
                    return true;
                byte[] weapons = new byte[2];
                
                weapons[0] = Data.SkillBase[skillid].Weapon1;
                weapons[1] = Data.SkillBase[skillid].Weapon2;
                
                if (weapons[1] == 255) 
                    weapons[1] = 6;
                
                foreach (byte b in weapons)
                {
                    if (b == Data.ItemBase[itemid].Class_C) 
                        return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Check weapon error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            return false;
        }
        #endregion
    }
}
