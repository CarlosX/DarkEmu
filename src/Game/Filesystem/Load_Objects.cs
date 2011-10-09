///////////////////////////////////////////////////////////////////////////
// DarkEmu: Load Objects
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
    class Load_Objects
    {
        public static void ObjectDataBase(string path)
        {
            //Split lines
            TxtFile.ReadFromFile(path, '\t');
            //Set string definition
            string s = null;
            //Repeat for each line in the file
            for (int l = 0; l <= TxtFile.amountLine - 1; l++)
            {
                //General data
                #region General data
                s = TxtFile.lines[l].ToString();
                TxtFile.commands = s.Split('\t');
                int ID = (Int32)(TxtFile.commands[1]);
                objectdata o = new objectdata();
                o.ID = ID;
                o.Name = TxtFile.commands[2];
                o.Level = (byte)(TxtFile.commands[57]);
                o.Exp = (Int32)(TxtFile.commands[79]);
                o.HP = (Int32)(TxtFile.commands[59]);
                o.Type = (byte)(TxtFile.commands[11]);
                o.ObjectType = (byte)(TxtFile.commands[15]);
                o.PhyDef = (Int32)(TxtFile.commands[71]);
                o.MagDef = (Int32)(TxtFile.commands[72]);
                o.HitRatio = (Int32)(TxtFile.commands[75]);
                o.ParryRatio = (Int32)(TxtFile.commands[77]);
                o.Agresif = (byte)(TxtFile.commands[93]);
                o.Skill = new int[500];
                o.Speed1 = (Int32)(TxtFile.commands[46]);
                o.Speed2 = (Int32)(TxtFile.commands[47]);
                o.SpeedWalk = (Int32)(TxtFile.commands[46]);
                o.SpeedRun = (Int32)(TxtFile.commands[47]);
                o.SpeedZerk = (Int32)(TxtFile.commands[48]);
                #endregion
                //Normal monsters
                #region Normal monsters
                if (o.Type == 1 && o.Name.Contains("MOB_") && !o.Name.Contains("HUNTER") && !o.Name.Contains("THIEF"))
                {
                    o.Object_type = objectdata.NamdedType.MONSTER;
                }
                #endregion
                //Npc's and structures
                #region Npc / Structures
                if (o.Type == 2)
                {
                    if (o.Name.Contains("NPC"))
                        o.Object_type = objectdata.NamdedType.NPC;
                    else
                        o.Object_type = objectdata.NamdedType.STRUCTURE;
                }
                #endregion
                //Pet objects
                #region Pet objects
                if (o.Type == 3)
                {
                    if (o.Name.Contains("COS_T_") && !o.Name.Contains("TRADE") && !o.Name.Contains("FORTR"))
                        o.Object_type = objectdata.NamdedType.JOBTRANSPORT;
                    if (o.Name.Contains("COS_P_"))
                        o.Object_type = objectdata.NamdedType.GRABPET;
                    if (o.Name.Contains("COS_C_"))
                        o.Object_type = objectdata.NamdedType.NORMALTRANSPORT;
                }
                #endregion
                //Fortress war objects
                #region Fw objects
                if (o.Type == 4)
                {
                    if (o.Name.Contains("FW"))
                        o.Object_type = objectdata.NamdedType.FORTRESSWARMONSTER;
                }
                #endregion
                //Structures
                #region structures
                if (o.Type == 5)
                {
                    o.Object_type = objectdata.NamdedType.STRUCTURE;
                }
                #endregion
                //Player objects
                #region Player objects
                if (o.Name.Contains("CHAR_CH"))
                {
                    o.Race = 0;
                    o.Object_type = objectdata.NamdedType.PLAYER;
                }
                if (o.Name.Contains("CHAR_EU"))
                {
                    o.Race = 1;
                    o.Object_type = objectdata.NamdedType.PLAYER;
                }
                #endregion
                //Job objects
                #region Job
                if (o.Name.Contains("THIEF_NPC") || o.Name.Contains("HUNTER_NPC"))
                {
                    o.Object_type = objectdata.NamdedType.JOBMONSTER;
                    o.Type = 4;
                    o.Agresif = 1;
                }
                #endregion
                //Skills for the objects
                for (byte sk = 0; sk <= 8; sk++)
                {
                    //Get information from line 83 if not null
                    if ((Int32)(TxtFile.commands[83 + sk]) != 0 && Data.SkillBase[(Int32)(TxtFile.commands[83 + sk])].MagPer != 0)
                    {
                        //Read skill info
                        o.Skill[o.amountSkill] = (Int32)(TxtFile.commands[83 + sk]);
                        //Add amount skill to objects
                        o.amountSkill++;
                    }
                }
                //Add object to object database
                Data.ObjectBase[ID] = o;
            }
            //Write information to the console
            Console.WriteLine("[INFO] Loaded " + TxtFile.amountLine + " objects");
        }
    }
}
