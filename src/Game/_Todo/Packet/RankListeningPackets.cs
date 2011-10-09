///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
// File info: Public packet data
///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using System.Data.SqlClient;

namespace DarkEmu_GameServer
{
    public partial class Packet
    {
        public static byte[] RankListsActivityTrader()
        {
            Systems.MsSQL ms = new Systems.MsSQL("SELECT TOP 50 * FROM rank_job_activity WHERE job_type='1'");
            PacketWriter Writer = new PacketWriter();
            int countinfo = ms.Count();
            int i = 0;

            Writer.Create(Systems.SERVER_RANK_LISTS);
            Writer.Byte(1);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Byte((byte)(countinfo));

            using (SqlDataReader reader = ms.Read())
            {
                for (i = 0; i < countinfo; )
                {
                    while (reader.Read())
                    {
                        byte rank = reader.GetByte(2);
                        string name = reader.GetString(3);
                        byte level = reader.GetByte(4);
                        int exp = reader.GetInt32(5);
                        byte title = reader.GetByte(6);

                        Writer.Byte(rank);
                        Writer.Text(name);
                        Writer.Byte(level);
                        Writer.DWord(exp);
                        Writer.Byte(title);
                        i++;
                    }
                }
                ms.Close();
            }
            return Writer.GetBytes();
        }
        public static byte[] RankListsActivityThief()
        {
            Systems.MsSQL ms = new Systems.MsSQL("SELECT TOP 50 * FROM rank_job_activity WHERE job_type='2'");
            PacketWriter Writer = new PacketWriter();
            int countinfo = ms.Count();
            int i = 0;

            Writer.Create(Systems.SERVER_RANK_LISTS);
            Writer.Byte(1);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Byte((byte)(countinfo));

            using (SqlDataReader reader = ms.Read())
            {
                for (i = 0; i < countinfo; )
                {
                    while (reader.Read())
                    {
                        byte rank = reader.GetByte(2);
                        string name = reader.GetString(3);
                        byte level = reader.GetByte(4);
                        int exp = reader.GetInt32(5);
                        byte title = reader.GetByte(6);

                        Writer.Byte(rank);
                        Writer.Text(name);
                        Writer.Byte(level);
                        Writer.DWord(exp);
                        Writer.Byte(title);
                        i++;
                    }
                }
                ms.Close();
            }
            return Writer.GetBytes();
        }
        public static byte[] RankListsActivityHunter()
        {
            Systems.MsSQL ms = new Systems.MsSQL("SELECT TOP 50 * FROM rank_job_activity WHERE job_type='3'");
            PacketWriter Writer = new PacketWriter();
            int countinfo = ms.Count();
            int i = 0;

            Writer.Create(Systems.SERVER_RANK_LISTS);
            Writer.Byte(1);
            Writer.Byte(1);
            Writer.Byte(0);
            Writer.Byte((byte)(countinfo));

            using (SqlDataReader reader = ms.Read())
            {
                for (i = 0; i < countinfo; )
                {
                    while (reader.Read())
                    {
                        byte rank = reader.GetByte(2);
                        string name = reader.GetString(3);
                        byte level = reader.GetByte(4);
                        int exp = reader.GetInt32(5);
                        byte title = reader.GetByte(6);

                        Writer.Byte(rank);
                        Writer.Text(name);
                        Writer.Byte(level);
                        Writer.DWord(exp);
                        Writer.Byte(title);
                        i++;
                    }
                }
                ms.Close();
            }
            return Writer.GetBytes();
        }
        public static byte[] RankListsDonateTrader()
        {
            Systems.MsSQL ms = new Systems.MsSQL("SELECT TOP 50 * FROM rank_job_donate WHERE job_type='1'");
            PacketWriter Writer = new PacketWriter();
            int countinfo = ms.Count();
            int i = 0;

            Writer.Create(Systems.SERVER_RANK_LISTS);
            Writer.Byte(1);
            Writer.Byte(1);
            Writer.Byte(1);
            Writer.Byte((byte)(countinfo));

            using (SqlDataReader reader = ms.Read())
            {
                for (i = 0; i < countinfo; )
                {
                    while (reader.Read())
                    {
                        byte rank = reader.GetByte(2);
                        string name = reader.GetString(3);
                        byte level = reader.GetByte(4);
                        int donate = reader.GetInt32(5);

                        Writer.Byte(rank);
                        Writer.Text(name);
                        Writer.Byte(level);
                        Writer.DWord(donate);
                        i++;
                    }
                }
                ms.Close();
            }
            return Writer.GetBytes();
        }
        public static byte[] RankListsDonateThief()
        {
            Systems.MsSQL ms = new Systems.MsSQL("SELECT TOP 50 * FROM rank_job_donate WHERE job_type='3'");
            PacketWriter Writer = new PacketWriter();
            int countinfo = ms.Count();
            int i = 0;

            Writer.Create(Systems.SERVER_RANK_LISTS);
            Writer.Byte(1);
            Writer.Byte(2);
            Writer.Byte(1);
            Writer.Byte((byte)(countinfo));

            using (SqlDataReader reader = ms.Read())
            {
                for (i = 0; i < countinfo; )
                {
                    while (reader.Read())
                    {
                        byte rank = reader.GetByte(2);
                        string name = reader.GetString(3);
                        byte level = reader.GetByte(4);
                        int donate = reader.GetInt32(5);

                        Writer.Byte(rank);
                        Writer.Text(name);
                        Writer.Byte(level);
                        Writer.DWord(donate);
                        i++;
                    }
                }
                ms.Close();
            }
            return Writer.GetBytes();
        }
        public static byte[] RankListsDonateHunter()
        {
            Systems.MsSQL ms = new Systems.MsSQL("SELECT TOP 50 * FROM rank_job_donate WHERE job_type='2'");
            PacketWriter Writer = new PacketWriter();
            int countinfo = ms.Count();
            int i = 0;

            Writer.Create(Systems.SERVER_RANK_LISTS);
            Writer.Byte(1);
            Writer.Byte(3);
            Writer.Byte(1);
            Writer.Byte((byte)(countinfo));

            using (SqlDataReader reader = ms.Read())
            {
                for (i = 0; i < countinfo; )
                {
                    while (reader.Read())
                    {
                        byte rank = reader.GetByte(2);
                        string name = reader.GetString(3);
                        byte level = reader.GetByte(4);
                        int donate = reader.GetInt32(5);

                        Writer.Byte(rank);
                        Writer.Text(name);
                        Writer.Byte(level);
                        Writer.DWord(donate);
                        i++;
                    }
                }
                ms.Close();
            }
            return Writer.GetBytes();
        }
        public static byte[] HonorRank(character c)
        {
            Systems.MsSQL ms = new Systems.MsSQL("SELECT * FROM rank_honor");
            PacketWriter Writer = new PacketWriter();
            int countinfo = ms.Count();

            Writer.Create(Systems.SERVER_HONOR_RANK);
            Writer.Byte(1);
            int i = 0;
            Writer.Byte((byte)(countinfo));
            using (SqlDataReader reader = ms.Read())
            {
                while (reader.Read())
                {
                    byte rankicon = reader.GetByte(1);
                    string rankname = reader.GetString(2);
                    byte ranklevel = reader.GetByte(3);
                    byte ranklevelc = reader.GetByte(4);
                    int graduatesc = reader.GetInt32(5);
                    int rankposc = reader.GetInt32(6);

                    Writer.DWord(i + 1);
                    Writer.Byte(rankicon);
                    Writer.Text(rankname);
                    Writer.Byte(ranklevel);
                    Writer.Byte(ranklevelc);
                    Writer.DWord(graduatesc);
                    //If player has no guild, we write a 0 word value
                    if (c.Network.Guild.Name == null)
                    {
                        Writer.Word(0);
                    }
                    //If player has a guild we write the guild name.
                    else
                    {
                        Writer.Text(c.Network.Guild.Name);
                    }
                    i++;
                }
                ms.Close();
            }
            return Writer.GetBytes();
        }
    }
}
