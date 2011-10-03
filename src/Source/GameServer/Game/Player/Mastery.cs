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
    class Mastery
    {
        private static PacketWriter writer = new PacketWriter();

        public static void OnMasteryUpdate(PacketReader reader_, int Index_)
        {
            uint mastery = reader_.ReadDword();

            int[] CharacterMasteryIndex = DatabaseCore.Mastery.GetIndexByName(Player.General[Index_].CharacterName, Player.Stats[Index_].Model >= 14717);

            byte CurrentMasteryIndex  = 0;
            for(byte i = 0;i<CharacterMasteryIndex.Length;i++)
            {
             if(DatabaseCore.Mastery.MasteryId[CharacterMasteryIndex[i]] == mastery)
                 CurrentMasteryIndex = i;
            }
            ulong sp_required = Silkroad.GetLevelDataByLevel(DatabaseCore.Mastery.MasteryLevel[CharacterMasteryIndex[CurrentMasteryIndex]]).Skillpoints;

            if (Player.Stats[Index_].Skillpoints >= sp_required)
            {
                DatabaseCore.Mastery.MasteryLevel[CharacterMasteryIndex[CurrentMasteryIndex]]++;
                Player.Stats[Index_].Skillpoints -= (uint)sp_required;
                SPUpdate(Index_);

                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_MASTERYUPDATE);
                writer.AppendByte(1);
                writer.AppendDword(mastery);
                writer.AppendByte(DatabaseCore.Mastery.MasteryLevel[CharacterMasteryIndex[CurrentMasteryIndex]]);
                ServerSocket.Send(writer.getWorkspace(), Index_);

                DatabaseCore.WriteQuery("UPDATE mastery SET level='{0}' WHERE mastery='{1}' AND owner='{2}'", DatabaseCore.Mastery.MasteryLevel[CharacterMasteryIndex[CurrentMasteryIndex]], mastery, Player.General[Index_].CharacterName);
            }
        }

        public static void OnSkillUpdate(PacketReader reader_, int Index_)
        {
            int CharacterSkillIndex = DatabaseCore.Skill.GetIndexByName(Player.General[Index_].CharacterName);

            uint SkillId = reader_.ReadDword();
            Silkroad.Skill_ NewSkill = Silkroad.GetSkillById(SkillId);
            if (Player.Stats[Index_].Skillpoints >= NewSkill.RequiredSp)
            {
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SKILLUPDATE);
                writer.AppendByte(1);
                writer.AppendDword(SkillId);
                ServerSocket.Send(writer.getWorkspace(), Index_);

                DatabaseCore.Skill.SkillAmount[CharacterSkillIndex]++;
                for (int i = 1; i <= DatabaseCore.Skill.SkillAmount[CharacterSkillIndex]; i++)
                {
                    if (DatabaseCore.Skill.Skills[CharacterSkillIndex].SkillId[i] == SkillId - 1)
                    {
                        DatabaseCore.WriteQuery("UPDATE skills SET Skill{0}='{1}' WHERE Skill{0}='{2}' AND owner='{3}'", i, SkillId, DatabaseCore.Skill.Skills[CharacterSkillIndex].SkillId[i], Player.General[Index_].CharacterName);
                        DatabaseCore.Skill.Skills[CharacterSkillIndex].SkillId[i + 1] = SkillId;
                    }
                }
                DatabaseCore.WriteQuery("UPDATE skills SET Skill{0}='{1}' WHERE owner='{2}'", DatabaseCore.Skill.SkillAmount[CharacterSkillIndex], SkillId, Player.General[Index_].CharacterName);
                DatabaseCore.WriteQuery("UPDATE skills SET AmountSkill='{0}' WHERE owner='{1}'", DatabaseCore.Skill.SkillAmount[CharacterSkillIndex], Player.General[Index_].CharacterName);

                DatabaseCore.Skill.Skills[CharacterSkillIndex].SkillId[DatabaseCore.Skill.SkillAmount[CharacterSkillIndex] - 1] = SkillId;

                Player.Stats[Index_].Skillpoints -= (uint)NewSkill.RequiredSp;
                SPUpdate(Index_);
      
            }
        }
        
        public static void SPUpdate(int Index_)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SKILLPOINTS);
            writer.AppendByte(2);
            writer.AppendDword(Player.Stats[Index_].Skillpoints);
            writer.AppendByte(0);
            ServerSocket.Send(writer.getWorkspace(), Index_);
            DatabaseCore.WriteQuery("UPDATE characters SET sp='{0}' WHERE name='{1}'", Player.Stats[Index_].Skillpoints, Player.General[Index_].CharacterName);
        }

    }
}