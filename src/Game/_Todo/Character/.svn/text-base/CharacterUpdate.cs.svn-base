///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;

namespace SrxRevo
{
    public partial class Systems
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Update HP
        /////////////////////////////////////////////////////////////////////////////////
        #region Update HP
        public void UpdateHp()
        {
            //Wrap our function inside a catcher
            try
            {
                //Send packet to update player hp
                Send(Packet.UpdatePlayer(this.Character.Information.UniqueID, 0x20, 1, this.Character.Stat.SecondHp));
                //If Character is in a party
                if (Character.Network.Party != null)
                {
                    //Update party visual hp update
                    Character.Network.Party.Send(Packet.Party_Data(6, this.Character.Information.UniqueID));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update hp error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Update mp
        /////////////////////////////////////////////////////////////////////////////////
        #region Update MP
        public void UpdateMp()
        {
            //Wrap our function inside a catcher
            try
            {
                //Send update mp packet
                Send(Packet.UpdatePlayer(this.Character.Information.UniqueID, 0x10, 2, this.Character.Stat.SecondMP));
                //If character is in a party
                if (Character.Network.Party != null)
                {
                    //Send mp update packet to party
                    Character.Network.Party.Send(Packet.Party_Data(6, this.Character.Information.UniqueID));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update MP Error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Update set stats
        /////////////////////////////////////////////////////////////////////////////////
        #region Set Stats
        public void SetStat()
        {
            //Wrap our function inside a catcher
            try
            {
                //Update stats hp / mp
                Character.Stat.Hp = Formule.gamePhp(Character.Information.Level, Character.Stat.Strength);
                Character.Stat.Mp = Formule.gamePmp(Character.Information.Level, Character.Stat.Intelligence);
                //Send visual update packet
                client.Send(Packet.PlayerStat(Character));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Set stats error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Update str info
        /////////////////////////////////////////////////////////////////////////////////
        #region Update Str info
        public void UpdateStrengthInfo(sbyte amount)
        {
            //Wrap our function inside a catcher
            try
            {
                //Set new min / max phy attack and phy def
                Character.Stat.MinPhyAttack += (0.45 * amount);
                Character.Stat.MaxPhyAttack += (0.65 * amount);
                Character.Stat.PhyDef += (0.40 * amount);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update strenght info error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Update int info
        /////////////////////////////////////////////////////////////////////////////////
        #region Update Int info
        public void UpdateIntelligenceInfo(sbyte amount)
        {
            //Wrap our function inside a catcher
            try
            {
                //Update min / mag attack and magdef
                Character.Stat.MinMagAttack += (0.45 * amount);
                Character.Stat.MaxMagAttack += (0.65 * amount);
                Character.Stat.MagDef += (0.40 * amount);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update int info error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Insert str points
        /////////////////////////////////////////////////////////////////////////////////
        #region Instert STR points
        public void InsertStr()
        {
            //Wrap our function inside a catcher
            try
            {
                //If player has attributes to spend
                if (Character.Information.Attributes > 0)
                {
                    //Remove attribute
                    Character.Information.Attributes -= 1;
                    //Update stat str
                    Character.Stat.Strength++;
                    //Update strenght info
                    UpdateStrengthInfo(1);
                    //Update visual packet
                    client.Send(Packet.UpdateStr());
                    //Set new stats
                    SetStat();
                    //Save new stats
                    SavePlayerInfo();
                }
            }
            catch (Exception ex)
            {
                Console.Write("Insert str points error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Instert int points
        /////////////////////////////////////////////////////////////////////////////////
        #region Instert INT points
        public void InsertInt()
        {
            //Wrap our function inside a catcher
            try
            {
                //Check if player has attribute point
                if (Character.Information.Attributes > 0)
                {
                    //Reduce attribute point
                    Character.Information.Attributes -= 1;
                    //Set intelligence +
                    Character.Stat.Intelligence++;
                    //Update int information
                    UpdateIntelligenceInfo(1);
                    //Send visual update
                    client.Send(Packet.UpdateInt());
                    //Set new stats
                    SetStat();
                    //Save stats
                    SavePlayerInfo();
                }
            }
            catch (Exception ex)
            {
                Console.Write("Insert INT point error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Update gamex / y
        /////////////////////////////////////////////////////////////////////////////////
        #region UpdateXY
        void UpdateXY()
        {
            //Wrap our function inside a catcher
            try
            {
                //Update x and y of character
                if (!File.FileLoad.CheckCave(Character.Position.xSec, Character.Position.ySec))
                {
                    Character.Position.x = Formule.gamex(Character.Position.x, Character.Position.xSec);
                    Character.Position.y = Formule.gamey(Character.Position.y, Character.Position.ySec);
                }
                else
                {
                //New cave update x y
                    Character.Position.x = Formule.cavegamex(Character.Position.x);
                    Character.Position.y = Formule.cavegamey(Character.Position.y);
                }
                 
                 }
            catch (Exception ex)
            {
                Console.WriteLine("Update xy error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Load player quick bar and autopotion
        /////////////////////////////////////////////////////////////////////////////////
        #region Quick bar / potion
        void CheckFile()
        {
            //Wrap our function inside a catcher
            try
            {
                //Set path for quickbar
                string player_path = Environment.CurrentDirectory + @"\player\info\";
                if (!System.IO.File.Exists(player_path + @"quickbar\" + Character.Information.Name + ".dat"))
                {
                    byte[] by = new byte[255];
                    for (byte i = 0; i <= 254; i++) by[i] = 0x00;
                    System.IO.File.Create(player_path + @"quickbar\" + Character.Information.Name + ".dat").Close();
                    System.IO.File.WriteAllBytes(player_path + @"quickbar\" + Character.Information.Name + ".dat", by);
                }
                //Set path for autopotion
                if (!System.IO.File.Exists(player_path + @"autopot\" + Character.Information.Name + ".dat"))
                {
                    byte[] by = new byte[255];
                    for (byte i = 0; i <= 6; i++) by[i] = 0x00;
                    System.IO.File.Create(player_path + @"autopot\" + Character.Information.Name + ".dat").Close();
                    System.IO.File.WriteAllBytes(player_path + @"autopot\" + Character.Information.Name + ".dat", by);
                }
                //Set debug information
                if (!System.IO.File.Exists(player_path + @"debug\" + Character.Information.Name + ".txt"))
                {
                    System.IO.File.Create(player_path + @"debug\" + Character.Information.Name + ".txt").Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Quickbar and potion error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Char stats check
        /////////////////////////////////////////////////////////////////////////////////
        #region Check stats
        public void CheckCharStats(character ch)
        {
            //Wrap our code inside a catcher
            try
            {
                //Stats checks
                if (ch.Stat.AttackPower < 0)
                    Math.Abs(ch.Stat.AttackPower);
                if (ch.Stat.Hit < 0)
                    Math.Abs(ch.Stat.Hit);
                if (ch.Stat.MagDef < 0)
                    Math.Abs(ch.Stat.MagDef);
                if (ch.Stat.PhyDef < 0)
                    Math.Abs(ch.Stat.PhyDef);
                if (ch.Stat.MinPhyAttack < 0)
                    Math.Abs(ch.Stat.MinPhyAttack);
                if (ch.Stat.MinMagAttack < 0)
                    Math.Abs(ch.Stat.MinMagAttack);
                if (ch.Stat.MaxPhyAttack < 0)
                    Math.Abs(ch.Stat.MaxPhyAttack);
                if (ch.Stat.MaxMagAttack < 0)
                    Math.Abs(ch.Stat.MaxMagAttack);
                if (ch.Information.XP < 0)
                    Math.Abs(ch.Information.XP);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Check Char Stats Error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
    }
}
