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
using System.Linq;
using System.Data.SqlClient;

namespace DarkEmu_GameServer
{
    class Character
    {
        private static PacketReader reader;

        public static void OnCharacter(PacketReader Reader_, int Index_)
        {
            reader = Reader_;

            byte pType = reader.ReadByte();
            Console.WriteLine("pType: {0}",pType);
            switch (pType)
            {
                case 1:
                    Console.WriteLine("Char Creation");
                    OnCharCreation(Reader_, Index_);
                    break;
                case 2:
                    Console.WriteLine("Char List");
                    SendCharacterList(Index_);
                    break;
                case 3:
                    //  OnCharDeletion(Reader_, Index_);
                    break;
                case 4:
                    Console.WriteLine("Char Name Check");
                    OnCharnameCheck(Reader_, Index_);
                    break;
                case 5:
                    //  OnCharRestore();
                    break;
            }
        }

        public static void SendCharacterList(int Index_)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHARACTER);
            writer.AppendWord(0x0102);

            int UserIndex = DatabaseCore.User.GetIndexByName(Player.General[Index_].User);

            writer.AppendByte(DatabaseCore.User.CharacterCount[UserIndex]);

            for (int i = 0; i < DatabaseCore.User.CharacterCount[UserIndex]; i++)
            {
                int tmpCharacterIndex = DatabaseCore.Character.GetIndexByName(DatabaseCore.User.Characters[UserIndex].CharacterName[i]);
                int[] CharacterItemIndex = DatabaseCore.Item.GetIndexByName(DatabaseCore.User.Characters[UserIndex].CharacterName[i]);

                writer.AppendDword(DatabaseCore.Character.Model[tmpCharacterIndex]); 
                writer.AppendWord((ushort)DatabaseCore.Character.CharacterName[tmpCharacterIndex].Length);
                writer.AppendString(false, DatabaseCore.Character.CharacterName[tmpCharacterIndex]);
                writer.AppendByte(DatabaseCore.Character.Volume[tmpCharacterIndex]);
                writer.AppendByte(DatabaseCore.Character.Level[tmpCharacterIndex]);
                writer.AppendLword(DatabaseCore.Character.Experience[tmpCharacterIndex]);
                writer.AppendWord(DatabaseCore.Character.Strength[tmpCharacterIndex]);
                writer.AppendWord(DatabaseCore.Character.Intelligence[tmpCharacterIndex]);
                writer.AppendWord(DatabaseCore.Character.Attributes[tmpCharacterIndex]);
                writer.AppendInt(DatabaseCore.Character.CHP[tmpCharacterIndex]);
                writer.AppendInt(DatabaseCore.Character.CMP[tmpCharacterIndex]);
                //writer.AppendDword(0x00);
                writer.AppendByte(0x00);
                writer.AppendByte(0x00);
                writer.AppendByte(0x00);
                writer.AppendByte(0x01);

                byte PlayerItemCount = 0;
                for (byte j = 0; j < 10; j++)
                {
                    if (DatabaseCore.Item.ItemId[CharacterItemIndex[j]] != 0)
                        PlayerItemCount++;
                }
                writer.AppendByte(PlayerItemCount);

                for (int j = 0; j < 10; j++)
                {
                    if (DatabaseCore.Item.ItemId[CharacterItemIndex[j]] != 0)
                    {
                        writer.AppendDword(DatabaseCore.Item.ItemId[CharacterItemIndex[j]]);
                        writer.AppendByte(DatabaseCore.Item.PlusValue[CharacterItemIndex[j]]);
                    }
                }

                writer.AppendByte(0x00);
                writer.AppendByte(0x00);
            }

            ServerSocket.Send(writer.getWorkspace(), Index_);
        }

        private static void OnCharCreation(PacketReader Reader_, int Index_)
        {
            PacketWriter writer = new PacketWriter();

            ushort charlen = Reader_.ReadWord();
            string name = Reader_.ReadString(false, charlen);

            uint model = Reader_.ReadDword();
            byte volume = Reader_.ReadByte();

            uint[] _item = new uint[5];

            _item[1] = Reader_.ReadDword();
            _item[2] = Reader_.ReadDword();
            _item[3] = Reader_.ReadDword();
            _item[4] = Reader_.ReadDword();

            int UserIndex = DatabaseCore.User.GetIndexByName(Player.General[Index_].User);

            if (DatabaseCore.Character.GetIndexByName(name) != -1)
            {
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHARACTER);
                writer.AppendWord(0x204);
                writer.AppendByte(0x10);
                ServerSocket.Send(writer.getWorkspace(), Index_);
            }
            else
            {
                double magdefmin = 3.0;
                double phydefmin = 6.0;
                ushort parrymin = 11;
                ushort phyatkmin = 6;
                ushort phyatkmax = 9;
                ushort magatkmin = 6;
                ushort magatkmax = 10;

                Silkroad.Item_ _item_1 = Silkroad.GetItemById(_item[1]);
                Silkroad.Item_ _item_2 = Silkroad.GetItemById(_item[2]);
                Silkroad.Item_ _item_3 = Silkroad.GetItemById(_item[3]);
                Silkroad.Item_ _item_4 = Silkroad.GetItemById(_item[4]);

                DatabaseCore.User.CharacterCount[UserIndex]++;
                Array.Resize<string>(ref DatabaseCore.User.Characters[UserIndex].CharacterName, DatabaseCore.User.CharacterCount[UserIndex]);
                DatabaseCore.User.Characters[UserIndex].CharacterName[DatabaseCore.User.CharacterCount[UserIndex] - 1] = name;

                DatabaseCore.Character.NumberOfCharacters++;
                Array.Resize<string>(ref DatabaseCore.Character.CharacterName, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<string>(ref DatabaseCore.Character.CharacterName, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<uint>(ref DatabaseCore.Character.CharacterId, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<uint>(ref DatabaseCore.Character.UniqueId, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<uint>(ref DatabaseCore.Character.HP, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<uint>(ref DatabaseCore.Character.MP, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<int>(ref DatabaseCore.Character.CHP, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<int>(ref DatabaseCore.Character.CMP, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<uint>(ref DatabaseCore.Character.Model, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<byte>(ref DatabaseCore.Character.Volume, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<byte>(ref DatabaseCore.Character.Level, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ulong>(ref DatabaseCore.Character.Experience, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ulong>(ref DatabaseCore.Character.Gold, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<uint>(ref DatabaseCore.Character.SkillPoints, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.Attributes, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<byte>(ref DatabaseCore.Character.BerserkBar, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<byte>(ref DatabaseCore.Character.Berserk, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<float>(ref DatabaseCore.Character.WalkSpeed, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<float>(ref DatabaseCore.Character.RunSpeed, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<float>(ref DatabaseCore.Character.BerserkSpeed, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.MinPhy, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.MaxPhy, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.MinMag, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.MaxMag, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.PhyDef, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.MagDef, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.Hit, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.Parry, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.Strength, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.Intelligence, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<ushort>(ref DatabaseCore.Character.SkillPointBar, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<byte>(ref DatabaseCore.Character.GM, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<sbyte>(ref DatabaseCore.Character.PVP, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<byte>(ref DatabaseCore.Character.XSector, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<byte>(ref DatabaseCore.Character.YSector, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<float>(ref DatabaseCore.Character.X, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<float>(ref DatabaseCore.Character.Z, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<float>(ref DatabaseCore.Character.Y, DatabaseCore.Character.NumberOfCharacters);
                Array.Resize<byte>(ref DatabaseCore.Character.MaxSlots, DatabaseCore.Character.NumberOfCharacters);

                int NewCharacterIndex = DatabaseCore.Character.NumberOfCharacters - 1;
                DatabaseCore.Character.CharacterName[NewCharacterIndex] = name;
                DatabaseCore.Character.CharacterId[NewCharacterIndex] = DatabaseCore.Character.CharacterId[NewCharacterIndex] + 1;
                DatabaseCore.Character.UniqueId[NewCharacterIndex] = DatabaseCore.Character.UniqueId[NewCharacterIndex] + 300000;
                DatabaseCore.Character.HP[NewCharacterIndex] = 200;
                DatabaseCore.Character.MP[NewCharacterIndex] = 200;
                DatabaseCore.Character.CHP[NewCharacterIndex] = 200;
                DatabaseCore.Character.CMP[NewCharacterIndex] = 200;
                DatabaseCore.Character.Model[NewCharacterIndex] = model;
                DatabaseCore.Character.Volume[NewCharacterIndex] = volume;
                DatabaseCore.Character.Level[NewCharacterIndex] = 1;

                DatabaseCore.Character.WalkSpeed[NewCharacterIndex] = 16;
                DatabaseCore.Character.RunSpeed[NewCharacterIndex] = 50;
                DatabaseCore.Character.BerserkSpeed[NewCharacterIndex] = 100;
                DatabaseCore.Character.Strength[NewCharacterIndex] = 20;
                DatabaseCore.Character.Intelligence[NewCharacterIndex] = 20;
                DatabaseCore.Character.PVP[NewCharacterIndex] = -1;
                DatabaseCore.Character.XSector[NewCharacterIndex] = 168;
                DatabaseCore.Character.YSector[NewCharacterIndex] = 98;
                DatabaseCore.Character.X[NewCharacterIndex] = 978;
                DatabaseCore.Character.Z[NewCharacterIndex] = 1097;
                DatabaseCore.Character.Y[NewCharacterIndex] = 40;
                DatabaseCore.Character.MaxSlots[NewCharacterIndex] = 45;

                DatabaseCore.Item.NumberOfItems += 46;
                Array.Resize<string>(ref DatabaseCore.Item.CharacterName, DatabaseCore.Item.NumberOfItems);
                Array.Resize<uint>(ref DatabaseCore.Item.ItemId, DatabaseCore.Item.NumberOfItems);
                Array.Resize<byte>(ref DatabaseCore.Item.PlusValue, DatabaseCore.Item.NumberOfItems);
                Array.Resize<byte>(ref DatabaseCore.Item.Quantity, DatabaseCore.Item.NumberOfItems);
                Array.Resize<byte>(ref DatabaseCore.Item.Type, DatabaseCore.Item.NumberOfItems);
                Array.Resize<byte>(ref DatabaseCore.Item.Slot, DatabaseCore.Item.NumberOfItems);
                Array.Resize<byte>(ref DatabaseCore.Item.Durability, DatabaseCore.Item.NumberOfItems);
                Array.Resize<byte>(ref DatabaseCore.Item.BlueAmount, DatabaseCore.Item.NumberOfItems);
                Array.Resize<DatabaseCore.Item_.Blue_>(ref DatabaseCore.Item.Blue, DatabaseCore.Item.NumberOfItems);

                for (byte i = 0; i < 46; i++)
                {
                    int tmpItemIndex = DatabaseCore.Item.NumberOfItems - 46 + i;
                    DatabaseCore.Item.Slot[tmpItemIndex] = i;
                    DatabaseCore.Item.Durability[tmpItemIndex] = 30;
                    DatabaseCore.Item.CharacterName[tmpItemIndex] = name;
                    DatabaseCore.Item.Blue[tmpItemIndex] = new DatabaseCore.Item_.Blue_();
                    DatabaseCore.Item.Blue[tmpItemIndex].Blue = new uint[9];
                    DatabaseCore.Item.Blue[tmpItemIndex].BlueAmount = new byte[9];
                }

                DatabaseCore.WriteQuery("UPDATE user SET char_{0}='{1}' WHERE name='{2}'", DatabaseCore.User.CharacterCount[UserIndex], name, Player.General[Index_].User);
                DatabaseCore.WriteQuery("UPDATE user SET char_count='{0}' WHERE name='{1}'", DatabaseCore.User.CharacterCount[UserIndex], Player.General[Index_].User);
                DatabaseCore.WriteQuery("INSERT INTO characters (account, name, chartype, volume) VALUE ('{0}','{1}', '{2}', '{3}')", Player.General[Index_].User, name, model, volume);

                for (int i = 0; i <= 45; i++)
                    DatabaseCore.WriteQuery("INSERT INTO items (itemnumber, owner, slot) VALUE ('item{0}','{1}', '{0}')", i, name);

                DatabaseCore.WriteQuery("UPDATE items SET itemid='{0}',plusvalue='0' ,durability='{1}' WHERE itemnumber='item1' AND owner ='{2}'", _item[1], _item_1.MIN_DURA, name);
                DatabaseCore.WriteQuery("UPDATE items SET itemid='{0}',plusvalue='0' ,durability='{1}' WHERE itemnumber='item4' AND owner ='{2}'", _item[2], _item_2.MIN_DURA, name);
                DatabaseCore.WriteQuery("UPDATE items SET itemid='{0}',plusvalue='0' ,durability='{1}' WHERE itemnumber='item5' AND owner ='{2}'", _item[3], _item_3.MIN_DURA, name);
                DatabaseCore.WriteQuery("UPDATE items SET itemid='{0}',plusvalue='0' ,durability='{1}' WHERE itemnumber='item6' AND owner ='{2}'", _item[4], _item_4.MIN_DURA, name);

                DatabaseCore.Item.ItemId[DatabaseCore.Item.NumberOfItems - 45] = _item[1];
                DatabaseCore.Item.Durability[DatabaseCore.Item.NumberOfItems - 45] = (byte)_item_1.MIN_DURA;
                DatabaseCore.Item.ItemId[DatabaseCore.Item.NumberOfItems - 42] = _item[2];
                DatabaseCore.Item.Durability[DatabaseCore.Item.NumberOfItems - 42] = (byte)_item_2.MIN_DURA;
                DatabaseCore.Item.ItemId[DatabaseCore.Item.NumberOfItems - 41] = _item[3];
                DatabaseCore.Item.Durability[DatabaseCore.Item.NumberOfItems - 41] = (byte)_item_3.MIN_DURA;
                DatabaseCore.Item.ItemId[DatabaseCore.Item.NumberOfItems - 40] = _item[4];
                DatabaseCore.Item.Durability[DatabaseCore.Item.NumberOfItems - 40] = (byte)_item_4.MIN_DURA;

                phydefmin += _item_1.MIN_PHYSDEF + _item_2.MIN_PHYSDEF + _item_3.MIN_PHYSDEF;
                magdefmin += _item_1.MAGDEF_MIN + _item_2.MAGDEF_MIN + _item_3.MAGDEF_MIN;
                parrymin += (ushort)(_item_1.MIN_PARRY + _item_2.MIN_PARRY + _item_3.MIN_PARRY);
                phyatkmin += (ushort)_item_4.MIN_LPHYATK;
                phyatkmax += (ushort)_item_4.MIN_HPHYATK;
                magatkmin += (ushort)_item_4.MIN_LMAGATK;
                magatkmax += (ushort)_item_4.MIN_HMAGATK;

                if (model >= 1907 && model <= 1932)
                {
                    DatabaseCore.Mastery.NumberOfMasteries += 7;
                    Array.Resize<string>(ref DatabaseCore.Mastery.CharacterName, DatabaseCore.Mastery.NumberOfMasteries);
                    Array.Resize<ushort>(ref DatabaseCore.Mastery.MasteryId, DatabaseCore.Mastery.NumberOfMasteries);
                    Array.Resize<byte>(ref DatabaseCore.Mastery.MasteryLevel, DatabaseCore.Mastery.NumberOfMasteries);
                    Array.Resize<uint>(ref DatabaseCore.Character.CharacterId, DatabaseCore.Character.NumberOfCharacters);
                    for (byte i = 0; i < 7; i++)
                        DatabaseCore.Mastery.CharacterName[DatabaseCore.Mastery.NumberOfMasteries - 1 - i] = name;

                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 7] = 257;
                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 6] = 258;
                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 5] = 259;
                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 4] = 273;
                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 3] = 274;
                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 2] = 275;
                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 1] = 276;

                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_1')", name, 257);
                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_2')", name, 258);
                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_3')", name, 259);
                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_4')", name, 273);
                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_5')", name, 274);
                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_6')", name, 275);
                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_7')", name, 276);

                }

                else if (model >= 14717 && model <= 14742)
                {
                    DatabaseCore.Mastery.NumberOfMasteries += 6;
                    Array.Resize<string>(ref DatabaseCore.Mastery.CharacterName, DatabaseCore.Mastery.NumberOfMasteries);
                    Array.Resize<ushort>(ref DatabaseCore.Mastery.MasteryId, DatabaseCore.Mastery.NumberOfMasteries);
                    Array.Resize<byte>(ref DatabaseCore.Mastery.MasteryLevel, DatabaseCore.Mastery.NumberOfMasteries);
                    Array.Resize<uint>(ref DatabaseCore.Character.CharacterId, DatabaseCore.Character.NumberOfCharacters);
                    for (byte i = 0; i < 6; i++)
                        DatabaseCore.Mastery.CharacterName[DatabaseCore.Mastery.NumberOfMasteries - 1 - i] = name;

                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 6] = 513;
                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 5] = 514;
                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 4] = 515;
                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 3] = 516;
                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 2] = 517;
                    DatabaseCore.Mastery.MasteryId[DatabaseCore.Mastery.NumberOfMasteries - 1] = 518;

                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_1')", name, 513);
                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_2')", name, 514);
                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_3')", name, 515);
                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_4')", name, 516);
                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_5')", name, 517);
                    DatabaseCore.WriteQuery("INSERT INTO mastery (owner, mastery, level, mastery_row) VALUE ('{0}','{1}', '0', 'mastery_6')", name, 518);
                }

                DatabaseCore.WriteQuery("INSERT INTO skills (owner, AmountSkill) VALUE ('{0}','0')", name);
                DatabaseCore.Skill.NumberOfSkills++;
                Array.Resize<string>(ref DatabaseCore.Skill.CharacterName, DatabaseCore.Skill.NumberOfSkills);
                Array.Resize<int>(ref DatabaseCore.Skill.SkillAmount, DatabaseCore.Skill.NumberOfSkills);
                Array.Resize<DatabaseCore.Skill_.Skills_>(ref DatabaseCore.Skill.Skills, DatabaseCore.Skill.NumberOfSkills);
                DatabaseCore.Skill.CharacterName[DatabaseCore.Skill.NumberOfSkills - 1] = name;

                if (_item[4] == 3632 || _item[4] == 3633)
                {
                    Silkroad.Item_ tmpItem = Silkroad.GetItemById(251);
                    phydefmin += tmpItem.MIN_PHYSDEF;
                    magdefmin += tmpItem.MAGDEF_MIN;
                    parrymin += (ushort)tmpItem.MIN_PARRY;

                    DatabaseCore.Item.ItemId[DatabaseCore.Item.NumberOfItems - 39] = 251;
                    DatabaseCore.Item.Durability[DatabaseCore.Item.NumberOfItems - 39] = (byte)tmpItem.MIN_DURA;

                    DatabaseCore.WriteQuery("UPDATE items SET itemid='251',plusvalue='0' ,durability='{0}' WHERE itemnumber='item7' AND owner ='{1}'", tmpItem.MIN_DURA, name);
                    DatabaseCore.WriteQuery("update characters set min_phyatk='{0}', max_phyatk='{1}', min_magatk='{2}', max_magatk='{3}', phydef='{4}', magdef='{5}', parry='{6}' where name='{7}'", phyatkmin, phyatkmax, magatkmin, magatkmax, (int)phydefmin, (int)magatkmin, parrymin, name);

                }
                else if (_item[4] == 10730 || _item[4] == 10734 || _item[4] == 10737)
                {
                    Silkroad.Item_ tmpItem = Silkroad.GetItemById(11387);
                    phydefmin += tmpItem.MIN_PHYSDEF;
                    magdefmin += tmpItem.MAGDEF_MIN;
                    parrymin += (ushort)tmpItem.MIN_PARRY;

                    DatabaseCore.Item.ItemId[DatabaseCore.Item.NumberOfItems - 39] = 11387;
                    DatabaseCore.Item.Durability[DatabaseCore.Item.NumberOfItems - 39] = (byte)tmpItem.MIN_DURA;

                    DatabaseCore.WriteQuery("UPDATE items SET itemid='11387',plusvalue='0' ,durability='{0}' WHERE itemnumber='item7' AND owner ='{1}'", tmpItem.MIN_DURA, name);
                    DatabaseCore.WriteQuery("update characters set min_phyatk='{0}', max_phyatk='{1}', min_magatk='{2}', max_magatk='{3}', phydef='{4}', magdef='{5}', parry='{6}' where name='{7}'", phyatkmin, phyatkmax, magatkmin, magatkmax, (int)phydefmin, (int)magatkmin, parrymin, name);
                }
                else if (_item[4] == 3636)
                {
                    DatabaseCore.Item.ItemId[DatabaseCore.Item.NumberOfItems - 39] = 62;
                    DatabaseCore.Item.Quantity[DatabaseCore.Item.NumberOfItems - 39] = 250;
                    DatabaseCore.Item.Type[DatabaseCore.Item.NumberOfItems - 39] = 1;

                    DatabaseCore.WriteQuery("UPDATE items SET itemid='62',quantity='250',type='1'  WHERE itemnumber='item7' AND owner ='{0}'", name);
                }

                else if (_item[4] == 10733)
                {
                    DatabaseCore.Item.ItemId[DatabaseCore.Item.NumberOfItems - 39] = 10727;
                    DatabaseCore.Item.Quantity[DatabaseCore.Item.NumberOfItems - 39] = 250;
                    DatabaseCore.Item.Type[DatabaseCore.Item.NumberOfItems - 39] = 1;
                    DatabaseCore.WriteQuery("UPDATE items SET itemid='10727',quantity='250',type='1'  WHERE itemnumber='item7' AND owner ='{0}'", name);
                }

                DatabaseCore.Character.MinPhy[NewCharacterIndex] = phyatkmin;
                DatabaseCore.Character.MaxPhy[NewCharacterIndex] = phyatkmax;
                DatabaseCore.Character.MinMag[NewCharacterIndex] = magatkmin;
                DatabaseCore.Character.MaxMag[NewCharacterIndex] = magatkmax;
                DatabaseCore.Character.PhyDef[NewCharacterIndex] = (ushort)phydefmin;
                DatabaseCore.Character.MagDef[NewCharacterIndex] = (ushort)magdefmin;
                DatabaseCore.Character.Hit[NewCharacterIndex] = 11;
                DatabaseCore.Character.Parry[NewCharacterIndex] = parrymin;

                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHARACTER);
                writer.AppendWord(0x101);
                ServerSocket.Send(writer.getWorkspace(), Index_);
            }
        }

        private static void OnCharnameCheck(PacketReader reader, int Index_)
        {
            PacketWriter writer = new PacketWriter();

            ushort charlen = reader.ReadWord();
            string name = reader.ReadString(false, charlen);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHARACTER);
            writer.AppendByte(0x04);
            if (DatabaseCore.Character.GetIndexByName(name) < 1)
                writer.AppendByte(0x01);
            else
            {
                writer.AppendByte(0x02);
                writer.AppendByte(0x10);
            }
            ServerSocket.Send(writer.getWorkspace(), Index_);
        }

        public static void OnIngameRequest(PacketReader Reader_, int Index_)
        {
            PacketWriter writer = new PacketWriter();
            reader = Reader_;

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_INGAME_ACCEPT);
            writer.AppendByte(0x01);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_LOADING_START);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            ushort characternamelen = reader.ReadWord();
            string charactername = reader.ReadString(false, characternamelen);
            
            int CharacterIndex = DatabaseCore.Character.GetIndexByName(charactername);
            int[] CharacterItemIndex = DatabaseCore.Item.GetIndexByName(charactername);
            int[] CharacterMasteryIndex = DatabaseCore.Mastery.GetIndexByName(charactername, DatabaseCore.Character.Model[CharacterIndex] >= 14717);
            int CharacterSkillIndex = DatabaseCore.Skill.GetIndexByName(charactername);

            Player.General[Index_].CharacterName = DatabaseCore.Character.CharacterName[CharacterIndex];
            Player.General[Index_].CharacterID = DatabaseCore.Character.CharacterId[CharacterIndex];
            Player.General[Index_].UniqueID = DatabaseCore.Character.UniqueId[CharacterIndex];
            Player.Stats[Index_].HP = DatabaseCore.Character.HP[CharacterIndex];
            Player.Stats[Index_].MP = DatabaseCore.Character.MP[CharacterIndex];
            Player.Stats[Index_].CHP = DatabaseCore.Character.CHP[CharacterIndex];
            Player.Stats[Index_].CMP = DatabaseCore.Character.CMP[CharacterIndex];
            Player.Stats[Index_].Model = DatabaseCore.Character.Model[CharacterIndex];
            Player.Stats[Index_].Volume = DatabaseCore.Character.Volume[CharacterIndex];
            Player.Stats[Index_].Level = DatabaseCore.Character.Level[CharacterIndex];
            Player.Stats[Index_].Experience = DatabaseCore.Character.Experience[CharacterIndex];
            Player.Stats[Index_].Gold = DatabaseCore.Character.Gold[CharacterIndex];
            Player.Stats[Index_].Skillpoints = DatabaseCore.Character.SkillPoints[CharacterIndex];
            Player.Stats[Index_].Attributes = DatabaseCore.Character.Attributes[CharacterIndex];
            Player.Stats[Index_].BerserkBar = DatabaseCore.Character.BerserkBar[CharacterIndex];
            Player.Flags[Index_].Berserk = DatabaseCore.Character.Berserk[CharacterIndex];
            Player.Speeds[Index_].WalkSpeed = DatabaseCore.Character.WalkSpeed[CharacterIndex];
            Player.Speeds[Index_].RunSpeed = DatabaseCore.Character.RunSpeed[CharacterIndex];
            Player.Speeds[Index_].BerserkSpeed = DatabaseCore.Character.BerserkSpeed[CharacterIndex];
            Player.Stats[Index_].MinPhy = DatabaseCore.Character.MinPhy[CharacterIndex];
            Player.Stats[Index_].MaxPhy = DatabaseCore.Character.MaxPhy[CharacterIndex];
            Player.Stats[Index_].MinMag = DatabaseCore.Character.MinMag[CharacterIndex];
            Player.Stats[Index_].MaxMag = DatabaseCore.Character.MaxMag[CharacterIndex];
            Player.Stats[Index_].PhyDef = DatabaseCore.Character.PhyDef[CharacterIndex];
            Player.Stats[Index_].MagDef = DatabaseCore.Character.MagDef[CharacterIndex];
            Player.Stats[Index_].Hit = DatabaseCore.Character.Hit[CharacterIndex];
            Player.Stats[Index_].Parry = DatabaseCore.Character.Parry[CharacterIndex];
            Player.Stats[Index_].Strength = DatabaseCore.Character.Strength[CharacterIndex];
            Player.Stats[Index_].Intelligence = DatabaseCore.Character.Intelligence[CharacterIndex];
            Player.Stats[Index_].SkillpointBar = DatabaseCore.Character.SkillPointBar[CharacterIndex];
            Player.Flags[Index_].GM = DatabaseCore.Character.GM[CharacterIndex];
            Player.Flags[Index_].PvP = DatabaseCore.Character.PVP[CharacterIndex];
            Player.Position[Index_].XSector = DatabaseCore.Character.XSector[CharacterIndex];
            Player.Position[Index_].YSector = DatabaseCore.Character.YSector[CharacterIndex];
            Player.Position[Index_].X = DatabaseCore.Character.X[CharacterIndex];
            Player.Position[Index_].Y = DatabaseCore.Character.Z[CharacterIndex];
            Player.Position[Index_].Z = DatabaseCore.Character.Y[CharacterIndex];
            Player.General[Index_].MaxSlots = DatabaseCore.Character.MaxSlots[CharacterIndex];

            Player.Objects[Index_].ActiveBuffs = new Player._Objects._Buffs[20];
            Player.General[Index_].Busy = false;
            Player.General[Index_].State = 0;
            Player.Objects[Index_].UsingSkill = false;
            Player.Objects[Index_].NormalAttack = false;
            Player.Objects[Index_].SpawnedIndex = new List<int>();
            Player.Objects[Index_].SpawnedMonsterIndex = new List<int>();
            Player.Objects[Index_].SpawnedItemsIndex = new List<int>();

            Player.General[Index_].WeaponType = Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterItemIndex[6]]).CLASS_C;

            if (DatabaseCore.Item.ItemId[CharacterItemIndex[8]] != 0)
                Player.Flags[Index_].WearingCape = true;

            for (byte i = 9; i < 13; i++)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemIndex[i]] != 0)
                    Player.Stats[0].TotalAccessoriesAbsorption += Silkroad.GetItemById(DatabaseCore.Item.ItemId[CharacterItemIndex[i]]).ABSORB_INC;
            }

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHARDATA);

            //writer.AppendDword(Player.Stats[Index_].Model);
            //8B 16 C7 C0 
            //74 07 00 00 
            //44 
            writer.AppendByte(0x8B);
            writer.AppendByte(0x16);
            writer.AppendByte(0xC7);
            writer.AppendByte(0xC0);

            writer.AppendByte(0x74);
            //writer.AppendByte(Player.Stats[Index_].Volume);

            writer.AppendByte(0x07);
            writer.AppendByte(0x00);
            writer.AppendByte(0x00);

            writer.AppendByte(0x44);


            writer.AppendByte(Player.Stats[Index_].Level);
            writer.AppendByte(Player.Stats[Index_].Level);
            writer.AppendLword(Player.Stats[Index_].Experience);
            writer.AppendDword(Player.Stats[Index_].SkillpointBar);
            //writer.AppendWord(0);
            writer.AppendLword(Player.Stats[Index_].Gold);
            writer.AppendDword(Player.Stats[Index_].Skillpoints);
            writer.AppendWord(Player.Stats[Index_].Attributes);
            writer.AppendByte(Player.Stats[Index_].BerserkBar);
            writer.AppendDword(0);
            writer.AppendDword(Player.Stats[Index_].HP);
            writer.AppendDword(Player.Stats[Index_].MP);
            if (Player.Flags[Index_].GM == 1)
                writer.AppendByte(1);
            else
                writer.AppendByte((byte)(Player.Stats[Index_].Level < 20 ? 1 : 0));

            //Pk
            writer.AppendByte(0);
            writer.AppendWord(0);
            writer.AppendDword(0);
            
            //Title
            writer.AppendByte(0);

            //Pvp
            writer.AppendByte(0);

            //Items
            writer.AppendByte(Player.General[Index_].MaxSlots);

            byte PlayerItemCount = 0;
            for (byte i = 0; i <= Player.General[Index_].MaxSlots; i++)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemIndex[i]] != 0)
                    PlayerItemCount++;
            }
            writer.AppendByte(PlayerItemCount);



            for (byte i = 1; i <= Player.General[Index_].MaxSlots; i++)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemIndex[i]] != 0)
                {
                    writer.AppendByte(i);
                    writer.AppendDword(DatabaseCore.Item.ItemId[CharacterItemIndex[i]]);
                    switch (DatabaseCore.Item.Type[CharacterItemIndex[i]])
                    {
                        case 0:
                            writer.AppendByte(DatabaseCore.Item.PlusValue[CharacterItemIndex[i]]);
                            writer.AppendLword(0);
                            writer.AppendDword(DatabaseCore.Item.Durability[CharacterItemIndex[i]]);
                            writer.AppendByte(DatabaseCore.Item.BlueAmount[CharacterItemIndex[i]]);
                            for (byte j = 0; j <= 8; j++)
                            {
                                if (DatabaseCore.Item.Blue[CharacterItemIndex[i]].Blue[j] != 0)
                                {
                                    writer.AppendDword(DatabaseCore.Item.Blue[CharacterItemIndex[i]].Blue[j]);
                                    writer.AppendDword(DatabaseCore.Item.Blue[CharacterItemIndex[i]].BlueAmount[j]);
                                }
                            }
                            break;
                        case 1:
                            writer.AppendWord(DatabaseCore.Item.Quantity[CharacterItemIndex[i]]);
                            break;
                        case 2:
                            writer.AppendByte(0);
                            writer.AppendWord(1);
                            break;
                    }
                }
            }

            //Avatar
            writer.AppendByte(5);

            //Load Items
            writer.AppendByte(0);

            //unk
            writer.AppendByte(0);

            //Job Mastery
            writer.AppendByte(0x0B);
            writer.AppendByte(0x00);
            writer.AppendByte(0x00);

            // Mastery
            for (byte i = 0; i < CharacterMasteryIndex.Length; i++)
            {
                if (DatabaseCore.Mastery.MasteryId[CharacterMasteryIndex[i]] != 0)
                {
                    writer.AppendByte(1);
                    writer.AppendDword(DatabaseCore.Mastery.MasteryId[CharacterMasteryIndex[i]]);
                    writer.AppendByte(DatabaseCore.Mastery.MasteryLevel[CharacterMasteryIndex[i]]);
                }
            }

            //Skills
            //writer.AppendWord(2); //unk

            for (int i = 1; i < DatabaseCore.Skill.SkillAmount[CharacterSkillIndex]; i++)
            {
                writer.AppendByte(1);
                writer.AppendDword(DatabaseCore.Skill.Skills[CharacterSkillIndex].SkillId[i]);
                writer.AppendByte(0);
            }           

            writer.AppendByte(2); //unk

            // Quest
            /*
            writer.AppendWord(0);  // how many Quest ids completed/aborted
            writer.AppendDword(2); // Quest id
            writer.AppendByte(0);  // number of Quests that are live*/

            //writer.AppendByte(2);  // unk

            // Talisman
            //writer.AppendWord(0x01); //unk
            //writer.AppendWord(0x01); //unk
            //writer.AppendDword(0x00); //unk
            //writer.AppendDword(0x00); //unk

            // unk
            writer.AppendByte(0x00);
            writer.AppendByte(0x02);
            writer.AppendWord(0x01);
            writer.AppendDword(0x01);
            writer.AppendByte(0x00);
            writer.AppendByte(0x00);
            writer.AppendDword(0x00);

            //Position
            writer.AppendDword(Player.General[Index_].UniqueID);

            writer.AppendByte(Player.Position[Index_].XSector);
            writer.AppendByte(Player.Position[Index_].YSector);
            writer.AppendFloat(Player.Position[Index_].X);
            writer.AppendFloat(Player.Position[Index_].Z);
            writer.AppendFloat(Player.Position[Index_].Y);

            writer.AppendWord(0);  //Angle
            writer.AppendByte(0);
            writer.AppendByte(1);
            writer.AppendByte(0);
            writer.AppendWord(0);  //Angle
            writer.AppendWord(0);
            writer.AppendByte(0);

            writer.AppendByte(Player.Flags[Index_].Berserk);
            //writer.AppendByte(0);  //unk
            writer.AppendFloat(Player.Speeds[Index_].WalkSpeed);
            writer.AppendFloat(Player.Speeds[Index_].RunSpeed);
            writer.AppendFloat(Player.Speeds[Index_].BerserkSpeed);

            writer.AppendByte(0); //ITEM_MALL_GOLD_TIME_SERVICE_TICKET_4W

            //Gm Name
            if (Player.Flags[Index_].GM == 1)
            {
                string gmname = string.Format("[GM]{0}", charactername);
                writer.AppendWord((ushort)gmname.Length);
                writer.AppendString(false, gmname);
            }
            else
            {
                writer.AppendWord((ushort)charactername.Length);
                writer.AppendString(false, charactername);
            }

            //Job System
            //Need Implement
            //if (c.Job.state == 1)
            //{
            //    Writer.Text(c.Job.Jobname);
            //    Writer.Byte(1);
            //    Writer.Byte(c.Job.level);//Level job
            //    Writer.Byte(c.Information.Level);//Level char
            //    Writer.Byte(1); // job level? myb
            //    Writer.LWord(0);// job exp probably y
            //    Writer.Byte(0);
            //    Writer.Byte(0);
            //    Writer.Byte(0);
            //    Writer.Byte(0);
            //}
            //else
            //{

            //    Writer.Word(0);
            //    Writer.Byte(0);
            //    Writer.Byte(0);
            //    Writer.Byte(2); // job type
            //    Writer.Byte(1); // job level? myb
            //    Writer.LWord(0);// job exp probably y
            //    Writer.Byte(0);
            //    Writer.Byte(0);
            //    Writer.Byte(0);
            //    Writer.Byte(0);
            //}

            writer.AppendWord(0);
            writer.AppendByte(0);
            writer.AppendByte(0);
            writer.AppendByte(0); //job type
            writer.AppendByte(1); //job level?
            writer.AppendLword(0); //job exp
            writer.AppendByte(0);
            writer.AppendByte(0);
            writer.AppendByte(0);
            writer.AppendByte(0);

            // Pvp pk state
            writer.AppendByte((byte)Player.Flags[Index_].PvP);

            // Guild Data
            //D3 02 A0 00 00 00 00 00
            writer.AppendByte(0x00);
            writer.AppendByte(0x00);
            writer.AppendByte(0x00);
            writer.AppendByte(0x00);
            writer.AppendByte(0x00);
            writer.AppendByte(0x00);
            writer.AppendByte(0x00);
            writer.AppendByte(0x00);

            // AccountId + Gm Info
            writer.AppendDword(Player.General[Index_].AccountID);
            writer.AppendByte(Player.Flags[Index_].GM);

            // Bar Information
            writer.AppendByte(0x00);
            writer.AppendByte(0x00);

            //unk
            writer.AppendLword(0x00);

            // Academy
            writer.AppendByte(0);
            writer.AppendByte(0);
            writer.AppendWord(1);
            writer.AppendWord(1);
            writer.AppendByte(0);
            writer.AppendByte(2);

            ServerSocket.Send(writer.getWorkspace(), Index_);

            //Stats.OnStatPacket(Index_);

            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_LOADING_END);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            //PlayerUnk1(Index_);
            
        }

        public static void PlayerUnk1(int Index_)
        {
            TimeSpan ts = GameSocket.ServerStartedTime - DateTime.Now;
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHARACTER_CELESTIAL_POSITION);
            writer.AppendDword((uint)Index_);
            writer.AppendDword((uint)Math.Abs(ts.TotalSeconds));
            ServerSocket.Send(writer.getWorkspace(), Index_);
        }

        public static void SpawnMe(int Index_)
        {
            if (Player.Flags[Index_].Ingame == 1)
                ServerSocket.SendPacketInRangeExceptMe(CreateSpawnPacket(Player.General[Index_], Player.Flags[Index_], Player.Position[Index_], Player.Stats[Index_], Player.Speeds[Index_]), Index_);
            
        }

        public static void SpawnOtherPlayer(int Index_)
        {
            for (int i = 0; i < Player.PlayersOnline; i++)
            {
                if (Player.General[i].CharacterID != 0 && Index_ != i)
                {
                    double Distance = Formula.CalculateDistance(Player.Position[Index_], Player.Position[i]);
                    if (Distance <= 800)
                    {
                        if (!Player.Objects[Index_].SpawnedIndex.Contains(i))
                        {
                            ServerSocket.Send(Character.CreateSpawnPacket(Player.General[i], Player.Flags[i], Player.Position[i], Player.Stats[i], Player.Speeds[i]), Index_);
                            Player.Objects[Index_].SpawnedIndex.Add(i);
                        }
                    }                
                }
            }
        }

        public static byte[] CreateSpawnPacket(Player._General general, Player._Flags flags, _Position position, Player._Stats stats, Player._Speeds speeds)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SPAWN);
            writer.AppendDword(stats.Model);
            writer.AppendByte(stats.Volume);
            if (flags.GM == 1)
                writer.AppendByte(1);

            else
                writer.AppendByte((byte)(stats.Level < 20 ? 1 : 0));

            writer.AppendByte(1);
            writer.AppendByte(general.MaxSlots);
     

            int tmpCharacterIndex = DatabaseCore.Character.GetIndexByName(general.CharacterName);
            int[] CharacterItemIndex = DatabaseCore.Item.GetIndexByName(general.CharacterName);

            byte PlayerItemCount = 0;
            for (byte j = 0; j < 10; j++)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemIndex[j]] != 0)
                    PlayerItemCount++;
            }
            writer.AppendByte(PlayerItemCount);

            for (int j = 0; j < 10; j++)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemIndex[j]] != 0)
                {
                    writer.AppendDword(DatabaseCore.Item.ItemId[CharacterItemIndex[j]]);
                    writer.AppendByte(DatabaseCore.Item.PlusValue[CharacterItemIndex[j]]);
                }
            }

            writer.AppendByte(4);
            writer.AppendByte(0);
            writer.AppendByte(0);
            writer.AppendDword((uint)general.UniqueID);

            writer.AppendByte(position.XSector);
            writer.AppendByte(position.YSector);
            writer.AppendFloat(position.X);
            writer.AppendFloat(position.Z);
            writer.AppendFloat(position.Y);

            writer.AppendWord(0);
            writer.AppendByte(0);
            writer.AppendByte(1);
            writer.AppendByte(0);
            writer.AppendWord(0);

            writer.AppendWord(1);

            writer.AppendByte(flags.Berserk);
            writer.AppendFloat(speeds.WalkSpeed);
            writer.AppendFloat(speeds.RunSpeed);
            writer.AppendFloat(speeds.BerserkSpeed);

            writer.AppendByte(0);

            if (flags.GM == 1)
            {
                string gmname = string.Format("[GM]{0}", general.CharacterName);
                writer.AppendWord((ushort)gmname.Length);
                writer.AppendString(false, gmname);
            }
            else
            {
                writer.AppendWord((ushort)general.CharacterName.Length);
                writer.AppendString(false, general.CharacterName);
            }

            writer.AppendByte(0);
            writer.AppendByte(1);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendByte(1);
            writer.AppendByte((byte)flags.PvP);
            return writer.getWorkspace();

        }

        public static byte[] CreateSpawnPacket(Player._General general, Player._Flags flags, _Position position, _Position newposition, Player._Stats stats, Player._Speeds speeds)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_SPAWN);
            writer.AppendDword(stats.Model);
            writer.AppendByte(stats.Volume);
            if (flags.GM == 1)
                writer.AppendByte(1);

            else
                writer.AppendByte((byte)(stats.Level < 20 ? 1 : 0));

            writer.AppendByte(1);
            writer.AppendByte(general.MaxSlots);


            int tmpCharacterIndex = DatabaseCore.Character.GetIndexByName(general.CharacterName);
            int[] CharacterItemIndex = DatabaseCore.Item.GetIndexByName(general.CharacterName);

            byte PlayerItemCount = 0;
            for (byte j = 0; j < 10; j++)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemIndex[j]] != 0)
                    PlayerItemCount++;
            }
            writer.AppendByte(PlayerItemCount);

            for (int j = 0; j < 10; j++)
            {
                if (DatabaseCore.Item.ItemId[CharacterItemIndex[j]] != 0)
                {
                    writer.AppendDword(DatabaseCore.Item.ItemId[CharacterItemIndex[j]]);
                    writer.AppendByte(DatabaseCore.Item.PlusValue[CharacterItemIndex[j]]);
                }
            }

            writer.AppendByte(4);
            writer.AppendByte(0);
            writer.AppendByte(0);
            writer.AppendDword((uint)general.UniqueID);

            writer.AppendByte(position.XSector);
            writer.AppendByte(position.YSector);
            writer.AppendFloat(position.X);
            writer.AppendFloat(position.Z);
            writer.AppendFloat(position.Y);

            writer.AppendWord(0);

            writer.AppendByte(1);
            writer.AppendByte(newposition.XSector);
            writer.AppendByte(newposition.YSector);
            writer.AppendFloat(newposition.X);
            writer.AppendFloat(newposition.Z);
            writer.AppendFloat(newposition.Y);
            writer.AppendByte(1);
            writer.AppendByte(0);
            writer.AppendWord(0);

            writer.AppendWord(1);

            writer.AppendByte(flags.Berserk);
            writer.AppendFloat(speeds.WalkSpeed);
            writer.AppendFloat(speeds.RunSpeed);
            writer.AppendFloat(speeds.BerserkSpeed);

            writer.AppendByte(0);

            if (flags.GM == 1)
            {
                string gmname = string.Format("[GM]{0}", general.CharacterName);
                writer.AppendWord((ushort)gmname.Length);
                writer.AppendString(false, gmname);
            }
            else
            {
                writer.AppendWord((ushort)general.CharacterName.Length);
                writer.AppendString(false, general.CharacterName);
            }

            writer.AppendByte(0);
            writer.AppendByte(1);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendDword(0);
            writer.AppendByte(1);
            writer.AppendByte((byte)flags.PvP);
            return writer.getWorkspace();

        }

        public static void DeSpawnMe(int Index_)
        {
            if (Player.Flags[Index_].Ingame == 1)
            {
                PacketWriter writer = new PacketWriter();
                writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_DESPAWN);
                writer.AppendDword(Player.General[Index_].UniqueID);

                byte[] tmpBuffer = writer.getWorkspace();

                for (int i = 0; i < Player.PlayersOnline; i++)
                {
                    if (Player.General[i].CharacterID != 0 && i != Index_)
                    {
                        if (Formula.CalculateDistance(Player.Position[Index_], Player.Position[i]) <= 800 && Player.Objects[i].SpawnedIndex.Contains(Index_))
                        {
                            Console.WriteLine(i);
                            ServerSocket.Send(tmpBuffer, i);
                            Player.Objects[i].SpawnedIndex.Remove(Index_);
                        }
                    }
                }
             //   ServerSocket.SendToAllExceptMe(writer.getWorkspace(), Index_);
            }
        }

        public static void Die(int Index_)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_DEAD);
            writer.AppendByte(4);
            ServerSocket.Send(writer.getWorkspace(), Index_);
        }
        public static void Die2(int Index_)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_DEAD2);
            writer.AppendDword(0);
            ServerSocket.Send(writer.getWorkspace(), Index_);
        }

        public static void ReSpawnMe(int Index_)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_DEAD2);
            writer.AppendDword(0);
            ServerSocket.Send(writer.getWorkspace(), Index_);

            OnState(Index_,0, 1);

            Player.Stats[Index_].CHP = (int)Player.Stats[Index_].HP / 2;
            Stats.HPUpdate(Index_, false);
            Player.General[Index_].State = 0;
            Player.General[Index_].Busy = false;

            Player.Objects[Index_].AttackingSkillID = 0;
            Player.Objects[Index_].AttackingObjectId = 0;
            Player.Objects[Index_].NormalAttack = false;
            Player.Objects[Index_].UsingSkill = false;
            Player.Flags[Index_].Dead = false;

        }
        public static void OnState(PacketReader reader_, int Index_)
        {
            PacketWriter writer = new PacketWriter();
            byte state = reader_.ReadByte();
            if (Player.General[Index_].State != 4 && Player.General[Index_].State != 10 && Player.General[Index_].State != 1)
                Player.General[Index_].State = state;
            else
            {
                state = 0;
                Player.General[Index_].State = state;
            }
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHARACTER_STATE);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendByte(1);
            writer.AppendByte(state);

            ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);
        }

        public static void OnState(int Index_, byte type, byte state)
        {
            PacketWriter writer = new PacketWriter();
            writer.SetOpcode(SERVER_OPCODES.GAME_SERVER_CHARACTER_STATE);
            writer.AppendDword(Player.General[Index_].UniqueID);
            writer.AppendByte(type);
            writer.AppendByte(state);

            ServerSocket.SendPacketIfPlayerIsSpawned(writer.getWorkspace(), Index_);
        }
    }
}
