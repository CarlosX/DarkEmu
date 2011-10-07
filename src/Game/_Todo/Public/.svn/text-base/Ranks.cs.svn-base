///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
// File info: Public packet data
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
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Job Ranks
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void RankList()
        {
            try
            {
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                /////////////////////////////////////////////////////////////////////////////////////
                int Notneeded = Reader.Int32();
                byte Type = Reader.Byte();
                byte Choice = Reader.Byte();
                /////////////////////////////////////////////////////////////////////////////////////

                /////////////////////////////////////////////////////////////////////////////////////
                // Rank Trader
                /////////////////////////////////////////////////////////////////////////////////////
                if (Type == 1)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    // Rank Trader Merchant Activity
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (Choice == 0)
                    {
                        try
                        {
                            MsSQL readsql = new MsSQL("SELECT TOP 50 * FROM rank_job_activity WHERE job_type='1'");
                            int begin = 0;
                            int count = readsql.Count();
                            using (SqlDataReader readinfo = readsql.Read())
                            {
                                while (readinfo.Read())
                                {
                                    for (begin = 0; begin < count; )
                                    {
                                        begin++;
                                        if (begin.Equals(count))
                                        {
                                            client.Send(Packet.RankListsActivityTrader());
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Rank Trader Merchant Activity error: " + ex);
                        }
                    }
                    /////////////////////////////////////////////////////////////////////////////////////
                    // Rank Trader Weekly Donation
                    /////////////////////////////////////////////////////////////////////////////////////
                    else if (Choice == 1)
                    {
                        try
                        {
                            MsSQL readsql = new MsSQL("SELECT TOP 50 * FROM rank_job_donate WHERE job_type='1'");
                            int begin = 0;
                            int count = readsql.Count();
                            using (SqlDataReader readinfo = readsql.Read())
                            {
                                while (readinfo.Read())
                                {
                                    for (begin = 0; begin < count; )
                                    {
                                        begin++;
                                        if (begin.Equals(count))
                                        {
                                            client.Send(Packet.RankListsDonateTrader());
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Rank Trader Weekly Donation: " + ex);
                        }
                    }
                }
                /////////////////////////////////////////////////////////////////////////////////////
                // Rank List Thief
                /////////////////////////////////////////////////////////////////////////////////////
                else if (Type == 3)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    // Rank List Thief Activity
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (Choice == 0)
                    {
                        try
                        {
                            MsSQL readsql = new MsSQL("SELECT TOP 50 * FROM rank_job_activity WHERE job_type='3'");
                            int begin = 0;
                            int count = readsql.Count();
                            using (SqlDataReader readinfo = readsql.Read())
                            {
                                while (readinfo.Read())
                                {
                                    for (begin = 0; begin < count; )
                                    {
                                        begin++;
                                        if (begin.Equals(count))
                                        {
                                            client.Send(Packet.RankListsActivityThief());
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Thief rank activity error: " + ex);
                        }
                    }
                    /////////////////////////////////////////////////////////////////////////////////////
                    // Rank List Thief Weekly Donation
                    /////////////////////////////////////////////////////////////////////////////////////
                    else if (Choice == 1)
                    {
                        try
                        {
                            MsSQL readsql = new MsSQL("SELECT TOP 50 * FROM rank_job_donate WHERE job_type='2'");
                            int begin = 0;
                            int count = readsql.Count();
                            using (SqlDataReader readinfo = readsql.Read())
                            {
                                while (readinfo.Read())
                                {
                                    for (begin = 0; begin < count; )
                                    {
                                        begin++;
                                        if (begin.Equals(count))
                                        {
                                            client.Send(Packet.RankListsDonateThief());
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Rank List Thief Weekly Donation: " + ex);
                        }
                    }
                }
                /////////////////////////////////////////////////////////////////////////////////////
                // Rank List Hunter
                /////////////////////////////////////////////////////////////////////////////////////
                else if (Type == 2) //Type 2 = Hunter
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    // Rank List Hunter Activity
                    /////////////////////////////////////////////////////////////////////////////////////
                    if (Choice == 0)
                    {
                        try
                        {
                            MsSQL readsql = new MsSQL("SELECT TOP 50 * FROM rank_job_activity WHERE job_type='2'");
                            int begin = 0;
                            int count = readsql.Count();
                            using (SqlDataReader readinfo = readsql.Read())
                            {
                                while (readinfo.Read())
                                {
                                    for (begin = 0; begin < count; )
                                    {
                                        begin++;
                                        if (begin.Equals(count))
                                        {
                                            client.Send(Packet.RankListsActivityHunter());
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Hunter rank activity error: " + ex);
                        }
                    }
                    /////////////////////////////////////////////////////////////////////////////////////
                    // Rank List Hunter Weekly Contribution
                    /////////////////////////////////////////////////////////////////////////////////////
                    else if (Choice == 1)
                    {
                        try
                        {
                            MsSQL readsql = new MsSQL("SELECT TOP 50 * FROM rank_job_donate WHERE job_type='3'");
                            int begin = 0;
                            int count = readsql.Count();
                            using (SqlDataReader readinfo = readsql.Read())
                            {
                                while (readinfo.Read())
                                {
                                    for (begin = 0; begin < count; )
                                    {
                                        begin++;
                                        if (begin.Equals(count))
                                        {
                                            client.Send(Packet.RankListsDonateHunter());
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Hunter Weekly Contribution error: " + ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Job rank loading error : " + ex);
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////
        // Honor rank
        /////////////////////////////////////////////////////////////////////////////////////
        void HonorRank()
        {
            /////////////////////////////////////////////////////////////////////////////////////
            MsSQL readsql = new MsSQL("SELECT TOP 50 * FROM rank_honor");
            int count = readsql.Count();
            /////////////////////////////////////////////////////////////////////////////////////
            if (count > 0)
            {
                client.Send(Packet.HonorRank(Character));
            }
        }
    }
}
