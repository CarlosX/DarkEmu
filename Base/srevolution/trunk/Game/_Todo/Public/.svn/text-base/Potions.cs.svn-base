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
        /* Rewriting potion system shortly */
        void HandlePotion(byte type, int ItemID)
        {
            try
            {
                if (type == 1) // hp
                {
                    long Total = (Character.Stat.Hp * Character.Information.Level * (long)Data.ItemBase[ItemID].Use_Time) / HandlePotionLevel(Character.Stat.Hp);
                    byte pslot = Getfreepotslot(Character.Information.Item.Potion);
                    StartPotionTimer(920, new int[] { (int)Total, type, pslot }, pslot);
                }
                else if (type == 2)//mp
                {
                    long Total = (Character.Stat.Mp * Character.Information.Level * (long)Data.ItemBase[ItemID].Use_Time2) / HandlePotionLevel(Character.Stat.Mp);
                    byte pslot = Getfreepotslot(Character.Information.Item.Potion);
                    StartPotionTimer(920, new int[] { (int)Total, type, pslot }, pslot);
                }
                else if (type == 3) //hp %25
                {
                    long Total = (Character.Stat.Hp * 25) / 100;
                    byte pslot = Getfreepotslot(Character.Information.Item.Potion);
                    Character.Information.Item.Potion[pslot] = 4;
                    StartPotionTimer(920, new int[] { (int)Total, type, pslot }, pslot);
                }
                else if (type == 4) //mp %25
                {
                    long Total = (Character.Stat.Mp * 25) / 100;
                    byte pslot = Getfreepotslot(Character.Information.Item.Potion);
                    Character.Information.Item.Potion[pslot] = 4;
                    StartPotionTimer(920, new int[] { (int)Total, type, pslot }, pslot);
                }
                else if (type == 5)// Vigor potions
                {
                    //This will be the definitions so they cannot be edited in pk2 
                    //Hard coded wont allow exploitations.
                    int hprecovery = 0;
                    int mprecovery = 0;

                    switch (Data.ItemBase[ItemID].Name)
                    {
                        case "ITEM_ETC_ALL_POTION_01":
                            hprecovery = 120;
                            mprecovery = 120;
                            break;
                        case "ITEM_ETC_ALL_POTION_02":
                            hprecovery = 220;
                            mprecovery = 220;
                            break;
                        case "ITEM_ETC_ALL_POTION_03":
                            hprecovery = 370;
                            mprecovery = 370;
                            break;
                        case "ITEM_ETC_ALL_POTION_04":
                            hprecovery = 570;
                            mprecovery = 570;
                            break;
                        case "ITEM_ETC_ALL_POTION_05":
                            hprecovery = 820;
                            mprecovery = 820;
                            break;
                    }

                    long Total = (Character.Stat.Hp * Character.Information.Level * (long)Data.ItemBase[ItemID].Use_Time) / HandlePotionLevel(Character.Stat.Hp);
                    byte pslot = Getfreepotslot(Character.Information.Item.Potion);
                    StartPotionTimer(920, new int[] { (int)Total, type, pslot }, pslot);
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }

        }
        public static int HandlePotionLevel(int hp)
        {
            int hpinfo = hp;
            byte info = 0;

            while (hpinfo > 0)
            {
                info++;
                hpinfo /= 10;
            }
            int b = 10;
            for (int i = 1; i <= info; i++)
            {
                b *= 10;
            }

            return b;
        }
    }
}
