///////////////////////////////////////////////////////////////////////////
// DarkEmu: Character creation
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Text;
using System.Text.RegularExpressions;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        //String replacer for bad characters in the character name.
        public static string Removebadchars(string Charactername)
        {
            //Return the replaced characters (They are replaced with no character).
            return Regex.Replace(Charactername, "[^a-zA-Z0-9.]+", "", RegexOptions.Compiled);
        }

        //Create temporary bool for race check default false = chinese, true = european
        bool European;
        //Set definition for ini usage
        Framework.Ini ini;
        //Temp random location definition
        int randomx = 0;
        int randomy = 0;
        int randomz = 0;
        int randomysec = 0;
        //Void creation for characters
        void CharacterCreate()
        {
            //Start wrapper for catching errors
            try
            {
                //Check the amount of characters created (If 4 then we return).
                if (MsSQL.GetRowsCount("SELECT * FROM character WHERE account='" + Player.AccountName + "'") == 4)
                    return;
                //Open our packet reader
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                //Need to be checked
                byte tocheck = Reader.Byte();
                //String character name
                string NewCharacterName = Reader.Text();
                //Integer for character model
                int NewCharacterModel = Reader.Int32();
                //Byte for character volume
                byte NewCharacterVolume = Reader.Byte();
                //Set new integer for item creation [4] total
                int[] Item = new int[4];
                //Item id one integer
                Item[0] = Reader.Int32();
                //Item id two integer
                Item[1] = Reader.Int32();
                //Item id three integer
                Item[2] = Reader.Int32();
                //Item id four integer
                Item[3] = Reader.Int32();
                //Close packet reader
                Reader.Close();
                //Set default character stats
                double MagicalDefense = 3;
                double PhysicalDefense = 6;
                double ParryRatio = 11;
                double HitRatio = 11;
                double MinimalPhysicalAttack = 6;
                double MaxPhysicalAttack = 9;
                double MinimalMagicalAttack = 6;
                double MaxMagicalAttack = 10;
                //Set mag defense
                MagicalDefense += Data.ItemBase[Item[0]].Defans.MinMagDef;
                MagicalDefense += Data.ItemBase[Item[1]].Defans.MinMagDef;
                MagicalDefense += Data.ItemBase[Item[2]].Defans.MinMagDef;
                //Set phy defence
                PhysicalDefense += Data.ItemBase[Item[0]].Defans.MinPhyDef;
                PhysicalDefense += Data.ItemBase[Item[1]].Defans.MinPhyDef;
                PhysicalDefense += Data.ItemBase[Item[2]].Defans.MinPhyDef;
                //Set parry information
                ParryRatio += Data.ItemBase[Item[0]].Defans.Parry;
                ParryRatio += Data.ItemBase[Item[1]].Defans.Parry;
                ParryRatio += Data.ItemBase[Item[2]].Defans.Parry;
                //Set hit ratio
                HitRatio += Data.ItemBase[Item[3]].Attack.MinAttackRating;
                //Set phy attack
                MinimalPhysicalAttack += Data.ItemBase[Item[3]].Attack.Min_LPhyAttack;
                MaxPhysicalAttack += Data.ItemBase[Item[3]].Attack.Min_HPhyAttack;
                //Set mag attack
                MinimalMagicalAttack += Data.ItemBase[Item[3]].Attack.Min_LMagAttack;
                MaxMagicalAttack += Data.ItemBase[Item[3]].Attack.Min_HMagAttack;
                //Set bool for european or chinese characters
                if (NewCharacterModel >= 14715 && NewCharacterModel <= 14745)
                    European = true;
                //Replace any bad character from the name like [ , ] etc.
                NewCharacterName = Removebadchars(NewCharacterName);
                //Check the character name before creation
                if (CharacterCheck(NewCharacterName))
                {
                    //If bad send informational packet
                    client.Send(Packet.CharacterName(4));
                    //Finally return.
                    return;
                }
                //Random x y z output
                int random = rnd.Next(1, 3);
                //If player is creating a european character
                if (European)
                {
                    //Set random start location for european
                    if (random == 1)
                    { randomx = 1030; randomz = 80; randomy = 512; }
                    if (random == 2)
                    { randomx = 889; randomz = 83; randomy = 1104; }
                    if (random == 3)
                    { randomx = 453; randomz = 80; randomy = 1249; }
                    //Insert the basic information into the database
                    MsSQL.InsertData("INSERT INTO character (account, name, chartype, volume, xsect, ysect, xpos, zpos, ypos, GuideData) VALUES ('" + Player.AccountName + "','" + NewCharacterName + "', '" + NewCharacterModel + "', '" + NewCharacterVolume + "','79','105','" + randomx + "','" + randomz + "','" + randomy + "','0000000000000000')");
                    //Insert reverse scroll data into the database
                    MsSQL.InsertData("INSERT INTO character_rev (charname, revxsec, revysec, revx, revy, revz) VALUES ('" + NewCharacterName + "','79','105','"+randomx+"','"+randomy+"','"+randomz+"')");
                    //Set definition for the character id information
                    Player.CreatingCharID = MsSQL.GetDataInt("Select * from character Where name='" + NewCharacterName + "'", "id");
                    //If the 3rd item is a sword or a dark staff
                    if (Item[3] == 10730 || Item[3] == 10734)
                    {
                        //Add the mag def information
                        MagicalDefense += Data.ItemBase[251].Defans.MinMagDef;
                        //Add the phy def information
                        PhysicalDefense += Data.ItemBase[251].Defans.MinPhyDef;
                        //Add parry ration
                        ParryRatio += Data.ItemBase[251].Defans.Parry;
                        //Add shield item
                        AddItem(10738, 0, 7, Player.CreatingCharID, 0);
                    }
                    //If the 3rd item is a crossbow
                    if (Item[3] == 10733)
                    {
                        //We add our base bolts 250
                        AddItem(10376, 250, 7, Player.CreatingCharID, 0);
                    }

                    //Add base mastery's for europe characters
                    AddMastery(513, Player.CreatingCharID); //Warrior
                    AddMastery(515, Player.CreatingCharID); //Rogue
                    AddMastery(514, Player.CreatingCharID); //Wizard
                    AddMastery(516, Player.CreatingCharID); //Warlock
                    AddMastery(517, Player.CreatingCharID); //Bard
                    AddMastery(518, Player.CreatingCharID); //Cleric  
                }
                //If the character model is an chinese character
                else
                {
                    //Set random start location for chinese
                    if (random == 1)
                    { randomx = 1030; randomz = 80; randomy = 512; randomysec = 97; }
                    if (random == 2)
                    { randomx = 959; randomz = 20; randomy = 421; randomysec = 98; }
                    if (random == 3)
                    { randomx = 964; randomz = 0; randomy = 235; randomysec = 97; }
                    //Add character default information into the database
                    MsSQL.InsertData("INSERT INTO character (account, name, chartype, volume, xsect, ysect, xpos, zpos, ypos, GuideData) VALUES ('" + Player.AccountName + "','" + NewCharacterName + "', '" + NewCharacterModel + "', '" + NewCharacterVolume + "','168','" + randomysec + "','"+ randomx +"','"+ randomz +"','"+ randomy +"','0000000000000000')");
                    //Add character reverse scroll information into the database
                    MsSQL.InsertData("INSERT INTO character_rev (charname, revxsec, revysec, revx, revy, revz) VALUES ('" + NewCharacterName + "','168','"+randomysec+"','"+randomx+"','"+randomy+"','"+randomz+"')");
                    //Get new character id
                    Player.CreatingCharID = MsSQL.GetDataInt("Select * from character Where name='" + NewCharacterName + "'", "id");
                    //If the item chosen is a blade or sword
                    if (Item[3] == 3632 || Item[3] == 3633)
                    {
                        //Set magical defense for shield
                        MagicalDefense += Data.ItemBase[251].Defans.MinMagDef;
                        //Set physical defense for shield
                        PhysicalDefense += Data.ItemBase[251].Defans.MinPhyDef;
                        //Set parry ratio for shield
                        ParryRatio += Data.ItemBase[251].Defans.Parry;
                        //Add the shield to the character
                        AddItem(251, 0, 7, Player.CreatingCharID, 0);
                    }
                    //If the item is a bow
                    if (Item[3] == 3636)
                    {
                        //Add 250 arrows to the new character
                        AddItem(62, 250, 7, Player.CreatingCharID, 0);
                    }
                    AddMastery(257, Player.CreatingCharID); //blade
                    AddMastery(258, Player.CreatingCharID); //heuksal
                    AddMastery(259, Player.CreatingCharID); //bow
                    AddMastery(273, Player.CreatingCharID); //cold
                    AddMastery(274, Player.CreatingCharID); //light
                    AddMastery(275, Player.CreatingCharID); //fire
                    AddMastery(276, Player.CreatingCharID); //force
                }
                //Add job mastery same for both races
                AddMastery(1000, Player.CreatingCharID);
                //Add the base items
                AddItem(Item[0], 0, 1, Player.CreatingCharID, 0);
                AddItem(Item[1], 0, 4, Player.CreatingCharID, 0);
                AddItem(Item[2], 0, 5, Player.CreatingCharID, 0);
                AddItem(Item[3], 0, 6, Player.CreatingCharID, 0);
                //Open settings information for start items
                ini = new Framework.Ini(Environment.CurrentDirectory + @"\Settings\Settings.ini");
                try
                {
                    //European start items custom
                    string Item1 = ini.GetValue("European.Start", "Item1", "").ToString();
                    string Item2 = ini.GetValue("European.Start", "Item2", "").ToString();
                    string Item3 = ini.GetValue("European.Start", "Item3", "").ToString();
                    string Item4 = ini.GetValue("European.Start", "Item4", "").ToString();
                    string Item5 = ini.GetValue("European.Start", "Item5", "").ToString();
                    //Chinese start items cusom
                    string Item6 = ini.GetValue("Chinese.Start", "Item1", "").ToString();
                    string Item7 = ini.GetValue("Chinese.Start", "Item2", "").ToString();
                    string Item8 = ini.GetValue("Chinese.Start", "Item3", "").ToString();
                    string Item9 = ini.GetValue("Chinese.Start", "Item4", "").ToString();
                    string Item10 = ini.GetValue("Chinese.Start", "Item5", "").ToString();
                    //The amount related information for european items custom
                    string Amount1 = ini.GetValue("European.Start", "Amount1", "").ToString();
                    string Amount2 = ini.GetValue("European.Start", "Amount2", "").ToString();
                    string Amount3 = ini.GetValue("European.Start", "Amount3", "").ToString();
                    string Amount4 = ini.GetValue("European.Start", "Amount4", "").ToString();
                    string Amount5 = ini.GetValue("European.Start", "Amount5", "").ToString();
                    //The amount related information for chinese items custom
                    string Amount6 = ini.GetValue("Chinese.Start", "Amount1", "").ToString();
                    string Amount7 = ini.GetValue("Chinese.Start", "Amount2", "").ToString();
                    string Amount8 = ini.GetValue("Chinese.Start", "Amount3", "").ToString();
                    string Amount9 = ini.GetValue("Chinese.Start", "Amount4", "").ToString();
                    string Amount10 = ini.GetValue("Chinese.Start", "Amount5", "").ToString();
                    //Add the custom items (European)
                    AddItem(Convert.ToInt32(Item1), Convert.ToByte(Amount1), 13, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item2), Convert.ToByte(Amount2), 14, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item3), Convert.ToByte(Amount3), 15, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item4), Convert.ToByte(Amount4), 16, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item5), Convert.ToByte(Amount5), 17, Player.CreatingCharID, 0);
                    //Add the custom items (Chinese)
                    AddItem(Convert.ToInt32(Item6), Convert.ToByte(Amount1), 13, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item7), Convert.ToByte(Amount2), 14, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item8), Convert.ToByte(Amount3), 15, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item9), Convert.ToByte(Amount4), 16, Player.CreatingCharID, 0);
                    AddItem(Convert.ToInt32(Item10), Convert.ToByte(Amount5), 17, Player.CreatingCharID, 0);
                }
                catch (Exception) { }

                //Update database information for stats
                MsSQL.UpdateData("update character set min_phyatk='" + (int)Math.Round(MinimalPhysicalAttack) +
                        "', max_phyatk='" + Math.Round(MaxPhysicalAttack) +
                        "', min_magatk='" + Math.Round(MinimalMagicalAttack) +
                        "', max_magatk='" + Math.Round(MaxMagicalAttack) +
                        "', phydef='" + Math.Round(PhysicalDefense) +
                        "', magdef='" + Math.Round(PhysicalDefense) +
                        "', parry='" + Math.Round(ParryRatio) +
                        "', hit='" + Math.Round(HitRatio) +
                        "' where name='" + NewCharacterName + "'");
                //Send information to the console
                Console.WriteLine("Character: " + NewCharacterName + " has been created");
                //Send packet success to the client
                client.Send(Packet.ScreenSuccess(1));
            }
            //If a error happens just catch it.
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        //This is for checking the new character name
        public void CharacterCheck(byte[] buff)
        {
            //Wrap our function into a catcher
            try
            {
                //Open our packet reader
                PacketReader Reader = new PacketReader(buff);
                //Ignore first byte
                Reader.Skip(1);
                //Read character name
                string Charactername = Reader.Text();
                //Close packet reader
                Reader.Close();
                //If Character name is in use
                if (CharacterCheck(Charactername))
                    //Send in use packet
                    client.Send(Packet.CharacterName(4));
                //If character name is ok
                else 
                    //Send succes packet
                    client.Send(Packet.ScreenSuccess(4));
            }
            catch (Exception ex)
            {
                //Write any error to debog log
                Systems.Debugger.Write(ex);
            }
        }

        //This is used while checking for name returning a bool true or false.
        public bool CharacterCheck(string name)
        {
            //Set new bool taken
            bool Taken = false;
            //Wrap with catcher
            try
            {
                //If name lenght higher is then 3 characters and lower then 12 characters
                if (name.Length > 3 && name.Length < 12)
                {
                    //Check name in database
                    string dbname = MsSQL.GetData("SELECT name FROM character WHERE name='" + name + "'", "name");
                    //If the name does not excist
                    if (dbname == null)
                    {
                        //Set bool to false
                        Taken = false;
                    }
                    //If name excists, bool is true
                    else Taken = true;
                }
                //Finally if name lenght is wrong taken is true
                else
                {
                    Taken = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Character Name Check Error {0}", ex);
                Systems.Debugger.Write(ex);
            }
            //Return the bool value
            return Taken;
        }           
    }
}
