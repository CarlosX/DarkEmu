///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Save guide
        /////////////////////////////////////////////////////////////////////////////////
        #region Save guide
        public void SaveGuideInfo()
        {
            //Wrap our function inside a catcher
            try
            {
                //Set default value for guidehex
                string GuideHex = "";
                //For 8 repeating
                for (int i = 0; i < 8; ++i)
                {
                    //Num lenght for guideinfo
                    string Numlen = String.Format("{0:X}", Character.Guideinfo.G1[i]);
                    //If lenght is 1, we set the string to 0 + lenght
                    if (Numlen.Length == 1) Numlen = "0" + Numlen;
                    //Set total
                    GuideHex = GuideHex + Numlen;
                }
                //Update database information
                MsSQL.UpdateData("update character set GuideData='" + GuideHex + "' where id='" + Character.Information.CharacterID + "'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Saving Guide error: {0}", ex);
                Console.WriteLine(ex);
            }   
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Save hp / mp
        /////////////////////////////////////////////////////////////////////////////////
        #region Save player hp, mp
        public void SavePlayerHPMP()
        {
            //Wrap our function inside a catcher
            try
            {
                //Update sql database with the hp / mp information
                MsSQL.UpdateData(string.Format("update character set s_hp='{0}', s_mp='{1}' where id='{2}'",
                      Character.Stat.SecondHp, Character.Stat.SecondMP, Character.Information.CharacterID));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Saving player's HP/MP error: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        } 
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Save player information
        /////////////////////////////////////////////////////////////////////////////////
        #region Save player information
        public void SavePlayerInfo()
        {
            //Wrap our function inside a catcher
            try
            {
                MsSQL.UpdateData("update character set min_phyatk='" + (Int32)(Math.Round(Character.Stat.MinPhyAttack)) +
                    "', max_phyatk='" + (Int32)(Math.Round(Character.Stat.MaxPhyAttack)) +
                    "', min_magatk='" + (Int32)(Math.Round(Character.Stat.MinMagAttack)) +
                    "', max_magatk='" + (Int32)(Math.Round(Character.Stat.MaxMagAttack)) +
                    "', phydef='" + (Int32)(Math.Round(Character.Stat.PhyDef - Character.Stat.uPhyDef)) +
                    "', magdef='" + (Int32)(Math.Round(Character.Stat.MagDef - Character.Stat.uMagDef)) +
                    "', hit='" + (Int16)(Math.Round(Character.Stat.Hit)) +
                    "', parry='" + (Int16)(Math.Round(Character.Stat.Parry)) +
                    "', hp='" + Character.Stat.Hp +
                    "', mp='" + Character.Stat.Mp +
                    "', s_hp='" + Character.Stat.SecondHp +
                    "', s_mp='" + Character.Stat.SecondMP +
                    "', attribute='" + Character.Information.Attributes +
                    "', strength='" + Character.Stat.Strength +
                    "', intelligence='" + Character.Stat.Intelligence +
                    "', experience='" + (Int64)(Character.Information.XP) +
                    "', spbar='" + Character.Information.SpBar +
                    "', sp='" + Character.Information.SkillPoint +
                    "', level='" + Character.Information.Level +
                    "', mag_absord='" + Character.Stat.mag_Absorb +
                    "', phy_absord='" + Character.Stat.phy_Absorb +
                    "' where id='" + Character.Information.CharacterID + "'");
                //Save guid information
                SaveGuideInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Saving player error: {0}", ex);
                Systems.Debugger.Write(ex);
            } 
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Save exp
        /////////////////////////////////////////////////////////////////////////////////
        #region Save exp
        public void SavePlayerExperince()
        {
            //Write our function inside a catcher
            try
            {
                //Update database information
                MsSQL.UpdateData("update character set  experience='" + Convert.ToUInt64(Character.Information.XP) +
                    "', spbar='" + Character.Information.SpBar +
                    "', sp='" + Character.Information.SkillPoint +
                    "' where id='" + Character.Information.CharacterID + "'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Save Experience error: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Save exp attack pet
        /////////////////////////////////////////////////////////////////////////////////
        #region Save exp
        public void SaveAttackPetExp()
        {
            //Wrap our function inside a catcher
            try
            {
                //Update database
                MsSQL.UpdateData("update pets set pet_experience='" + Convert.ToUInt64(Character.Attackpet.Details.exp) +"' where id='" + Character.Attackpet.Details.UniqueID + "'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Save Experience error: {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Save position
        /////////////////////////////////////////////////////////////////////////////////
        #region Save position
        protected void SavePlayerPosition()
        {
            //Wrap our function inside a catcher
            try
            {
                //Update database
                if (!File.FileLoad.CheckCave(Character.Position.xSec, Character.Position.ySec))
                {
                    MsSQL.UpdateData("update character set xsect='" + Character.Position.xSec +
                        "', ysect='" + Character.Position.ySec +
                        "', xpos='" + Math.Round(Formule.packetx(Character.Position.x, Character.Position.xSec)) +
                        "', ypos='" + Math.Round(Formule.packety(Character.Position.y, Character.Position.ySec)) +
                        "', zpos='" + Math.Round(Character.Position.z) +
                        "' where id='" + Character.Information.CharacterID + "'");
                }
                else
                {
                    MsSQL.UpdateData("update character set xsect='" + Character.Position.xSec +
                       "', ysect='" + Character.Position.ySec +
                       "', xpos='" + Math.Round(Formule.cavepacketx(Character.Position.x)) +
                       "', ypos='" + Math.Round(Formule.cavepackety(Character.Position.y)) +
                       "', zpos='" + Math.Round(Character.Position.z) +
                       "' where id='" + Character.Information.CharacterID + "'");
                }
               
               }
            catch (Exception ex)
            {
                Console.WriteLine("Save position error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Save return / last place of death etc
        /////////////////////////////////////////////////////////////////////////////////
        #region Save last point of death or return scroll usage
        protected void SavePlayerReturn()
        {
            //Wrap our function inside a catcher
            try
            {
                //Update database
                MsSQL.UpdateData("update character_rev set revxsec='" + Character.Position.xSec +
                    "', revysec='" + Character.Position.ySec +
                    "', revx='" + Math.Round(Formule.packetx(Character.Position.x, Character.Position.xSec)) +
                    "', revy='" + Math.Round(Formule.packety(Character.Position.y, Character.Position.ySec)) +
                    "', revz='" + Math.Round(Character.Position.z) +
                    "' where charname='" + Character.Information.Name + "'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Save return position error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        } 
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Set balance
        /////////////////////////////////////////////////////////////////////////////////
        #region Set Balance
        protected void SetBalance()
        {
            //Wrap our function inside a catcher
            try
            {
                //Set maxstats info
                int maxstat = 28 + Character.Information.Level * 4;
                //Set physical and magical balance
                Character.Information.Phy_Balance = (byte)(99 - (100 * 2 / 3 * (maxstat - Character.Stat.Strength) / maxstat));
                Character.Information.Mag_Balance = (byte)(100 * Character.Stat.Intelligence / maxstat);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Set balance error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Save gold
        /////////////////////////////////////////////////////////////////////////////////
        #region Save gold
        protected void SaveGold()
        {
            //Wrap our function inside a catcher
            try
            {
                //Update database with inventory gold
                MsSQL.UpdateData("update character set gold='" + Character.Information.Gold + "' where id='" + Character.Information.CharacterID + "'");
                //Update database with storage gold
                MsSQL.UpdateData("update users set gold='" + Character.Account.StorageGold + "' where id='" + Player.AccountName + "'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Save gold error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        protected void SaveGuildGold()
        {
            //Wrap our function inside a catcher
            try
            {
                //Update database with inventory gold
                MsSQL.UpdateData("update guild set guild_storage_gold='" + Character.Network.Guild.StorageGold +
                        "' where id='" + Character.Network.Guild.Guildid + "'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Save gold error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Save quick bar and autopotion
        /////////////////////////////////////////////////////////////////////////////////
        #region Save quick bar & autopotion
        protected void Save()
        {
            //Wrap our function inside a catcher
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                string player_path;
                byte[] file;
                //Switch on byte
                switch (Reader.Byte())
                {
                    case 1:
                        //Save quickbar information
                        player_path = Environment.CurrentDirectory + @"\player\info\quickbar\" + Character.Information.Name + ".dat";
                        file = System.IO.File.ReadAllBytes(player_path);

                        byte Slot = Reader.Byte();
                        byte sType = Reader.Byte();

                        Slot *= 5;

                        file[Slot] = sType;
                        file[Slot + 1] = Reader.Byte();
                        file[Slot + 2] = Reader.Byte();
                        file[Slot + 3] = Reader.Byte();
                        file[Slot + 4] = Reader.Byte();
                        System.IO.File.WriteAllBytes(player_path, file);
                        break;
                    case 2:
                        //Save autopotion information
                        player_path = Environment.CurrentDirectory + @"\player\info\autopot\" + Character.Information.Name + ".dat";
                        file = System.IO.File.ReadAllBytes(player_path);
                        file[0] = Reader.Byte();
                        file[1] = Reader.Byte();
                        file[2] = Reader.Byte();
                        file[3] = Reader.Byte();
                        file[4] = Reader.Byte();
                        file[5] = Reader.Byte();
                        file[6] = Reader.Byte();
                        System.IO.File.WriteAllBytes(player_path, file);
                        UpdateHp();
                        UpdateMp();
                        break;
                }
                Reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Save quickbar and autopotion error {0}", ex);
                Systems.Debugger.Write(ex);
            }        
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////
        // Save return point
        /////////////////////////////////////////////////////////////////////////////////
        #region Save return point
        protected void SavePlace()
        {
            //Wrap our function inside a catcher
            try
            {
                //Open our packet reader
                PacketReader reader = new PacketReader(PacketInformation.buffer);
                int ObjectID = reader.Int32();
                //Get object information
                obj o = GetObject(ObjectID);
                //Defaul value for type
                byte type = 0;
                //Switch by object name
                switch (Data.ObjectBase[o.ID].Name)
                {
                    case "STORE_CH_GATE":
                        type = 1;
                        break;
                    case "STORE_WC_GATE":
                        type = 2;
                        break;
                    case "STORE_KT_GATE":
                        type = 5;
                        break;
                    case "STORE_EU_GATE":
                        type = 20;
                        break;
                    case "STORE_CA_GATE":
                        type = 25;
                        break;
                    case "STORE_SD_GATE1":
                        type = 175;
                        break;
                    case "STORE_SD_GATE2":
                        type = 176;
                        break;
                    case "STORE_EU_TO_JUPITER_FIELD_GATE":
                        type = 207;
                        break;
                    case "STORE_JUPITER_FIELD_TO_EU_GATE":
                        type = 208;
                        break;
                    case "STORE_JUPITER_FIELD_TO_JUPITER_GATE":
                        type = 209;
                        break;
                    case "STORE_JUPITER_A0_START_GATE":
                        type = 220;
                        break;
                }
                //Set new return global information
                Character.Information.Place = type;
                //Update database
                MsSQL.InsertData("update character set savearea='" + Character.Information.Place + "' where id='" + Character.Information.CharacterID + "'");
                //Send confirmation packet
                client.Send(Packet.UpdatePlace());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Save return point error {0}", ex);
                Systems.Debugger.Write(ex);
            }
        }
        #endregion
    }
}
