///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
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
        public void AddMastery(short masteryid, int newCharName)
        {
            try
            {
                MsSQL.InsertData("INSERT INTO mastery (owner, mastery) VALUES ('" + newCharName + "','" + masteryid + "')");
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        public void Mastery_Up()
        {
            try
            {
                List<byte> Masteries = new List<byte>();
                MsSQL mastery = new MsSQL("SELECT * FROM mastery WHERE owner='"+Character.Information.CharacterID+"'");
                 using (SqlDataReader reader = mastery.Read())
                    {
                        while (reader.Read())
                        {
                            Masteries.Add(reader.GetByte(2));
                        }

                 }
                int totalmastery = 0;
                int masterylimit = 360;
                bool euchar = false;
                if (Character.Information.Model >= 10000 && Character.Information.Model <= 16000)
                {
                    masterylimit = 239;
                    euchar = true;
                }
                
                for(int i = 0;i < Masteries.Count;i++)
                {
                    totalmastery += Masteries[i];
                }
                if (totalmastery <= masterylimit)
                {
                    if (!Character.Action.upmasterytimer)
                    {
                        Character.Action.upmasterytimer = true;
                        MasteryupTimer(150);

                        PacketReader Reader = new PacketReader(PacketInformation.buffer);
                        int masteryid = Reader.Int32();
                        byte level = Reader.Byte();
                        byte m_index = MasteryGet(masteryid);

                        if (m_index == 0)
                        {
                            return;
                        }

                        if (!(Character.Information.SkillPoint < Data.MasteryBase[Character.Stat.Skill.Mastery_Level[m_index]]))
                        {
                            if (euchar == true)
                            {
                                if (Character.Stat.Skill.Mastery_Level[m_index] < Character.Information.Level)
                                {

                                    Character.Stat.Skill.Mastery_Level[m_index]++;
                                    Character.Information.SkillPoint -= Data.MasteryBase[Character.Stat.Skill.Mastery_Level[m_index]];

                                    client.Send(Packet.InfoUpdate(2, Character.Information.SkillPoint, 0));
                                    client.Send(Packet.MasteryUpPacket(masteryid, Character.Stat.Skill.Mastery_Level[m_index]));

                                    SaveMaster();

                                }
                            }
                            else
                            {
                                if (Character.Stat.Skill.Mastery_Level[m_index] < Character.Information.Level)
                                {
                                    if (!(Character.Stat.Skill.Mastery_Level[m_index] == 120))
                                    {
                                        Character.Stat.Skill.Mastery_Level[m_index]++;
                                        Character.Information.SkillPoint -= Data.MasteryBase[Character.Stat.Skill.Mastery_Level[m_index]];

                                        client.Send(Packet.InfoUpdate(2, Character.Information.SkillPoint, 0));
                                        client.Send(Packet.MasteryUpPacket(masteryid, Character.Stat.Skill.Mastery_Level[m_index]));

                                        SaveMaster();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //client.Send(Packet.IngameMessages(SERVER_ACTIONSTATE, IngameMessages.UIIT_STT_SKILL_LEARN_MASTERY_TOTAL_LIMIT));
                    return;
                }
            }
            

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        byte MasteryGet(int id)
        {
            try
            {
                for (byte b = 1; b <= 7; b++)
                    if (Character.Stat.Skill.Mastery[b] == id) return b;
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return 0;
        }
        void SaveMaster()
        {
            try
            {
                for (byte b = 1; b <= 7; b++)
                {
                    if (Character.Stat.Skill.Mastery[b] != 0) MsSQL.InsertData("update mastery set level='" + Character.Stat.Skill.Mastery_Level[b] + "' where owner='" + Character.Information.CharacterID + "' AND mastery='" + Character.Stat.Skill.Mastery[b] + "'");
                }
                MsSQL.InsertData("update character set sp='" + Character.Information.SkillPoint + "' where id='" + Character.Information.CharacterID + "'");
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        void SaveSkill(int skillid)
        {
            try
            {
                int info = MsSQL.GetRowsCount("SELECT * from saved_skills WHERE owner='" + Character.Information.CharacterID + "' AND Skillid='" + skillid + "'");
                if (info != 0) return;
                MsSQL.InsertData("INSERT INTO saved_skills (skillid, owner, level) VALUES ('" + skillid + "','" + Character.Information.CharacterID + "','1') ");
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
            return;
        }      
        void Mastery_Skill_Up()
        {
            try
            {
                if (!Character.Action.upskilltimer)
                {
                    Character.Action.upskilltimer = true;
                    SkillUpTimer(250);

                    PacketReader Reader = new PacketReader(PacketInformation.buffer);
                    int skillid = Reader.Int32();
                    if (!(Character.Information.SkillPoint < Data.SkillBase[skillid].SkillPoint))
                    {
                        Character.Information.SkillPoint -= Data.SkillBase[skillid].SkillPoint;
                        client.Send(Packet.InfoUpdate(2, Character.Information.SkillPoint, 0));
                        client.Send(Packet.SkillUpdate(skillid));
                        client.Send(Packet.PlayerStat(Character));

                        SaveSkill(skillid);

                        MsSQL ms = new MsSQL("SELECT * FROM saved_skills WHERE owner='" + Character.Information.CharacterID + "'");
                        using (SqlDataReader reader = ms.Read())
                            Character.Stat.Skill.AmountSkill = ms.Count();
                        int i = 1;
                        using (SqlDataReader reader = ms.Read())
                        {
                            while (reader.Read())
                            {
                                Character.Stat.Skill.Skill[i] = reader.GetInt32(2);
                                i++;
                            }
                        }
                        ms.Close();

                        SkillGetOpened(skillid);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
        byte MasteryGetPower(int SkillID)
        {
            return Character.Stat.Skill.Mastery_Level[MasteryGet(Data.SkillBase[SkillID].Mastery)];
        }
        public byte MasteryGetBigLevel
        {
            get
            {
                return Character.Stat.Skill.Mastery_Level.Max();
            }
        }
    }
}
