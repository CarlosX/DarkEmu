///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Collections.Generic;
namespace SrxRevo
{
    public partial class Systems
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Item Switch
        /////////////////////////////////////////////////////////////////////////////////
        void ItemMain()
        {
            #region Item Actions
            try
            {
                if (Character.State.Die || Character.Information.Scroll) return;
                PacketReader Reader = new PacketReader(PacketInformation.buffer);
                byte iType = Reader.Byte();

                switch (iType)
                {
                    case 0:
                        if (Character.State.Busy) return;
                        ItemMove(Reader.Byte(), Reader.Byte(), Reader.Int16());
                        break;
                    case 1:
                        ItemMoveInStorage(Reader.Byte(), Reader.Byte(), Reader.Int16());
                        break;
                    case 2:
                        Player_MoveItemToStorage(Reader.Byte(), Reader.Byte(), Reader.Int32());
                        break;
                    case 3:
                        Player_MoveStorageItemToInv(Reader.Byte(), Reader.Byte(), Reader.Int32());
                        break;
                    case 4:
                        ItemMoveToExhangePage(Reader.Byte());
                        break;
                    case 5:
                        ItemMoveExchangeToInv(Reader.Byte());
                        break;
                    case 8:
                        Player_BuyItem(Reader.Byte(), Reader.Byte(), Reader.Int16(), Reader.Int32());
                        break;
                    case 9:
                        Player_SellItem(Reader.Byte(), Reader.Int16(), Reader.UInt16());
                        break;
                    case 7:
                        if (Character.State.Busy) return;
                        Player_DropItem(Reader.Byte());
                        break;
                    case 10:
                        if (Character.State.Busy) return;
                        Player_DropGold(Reader.UInt64());
                        break;
                    case 11:
                        Player_TakeGoldW(iType, Reader.Int64());
                        break;
                    case 12:
                        Player_GiveGoldW(iType, Reader.Int64());
                        break;
                    case 13:
                        ItemExchangeGold(Reader.Int64());
                        break;
                    case 16:
                        MovePetToPet(Reader.Int32(), Reader.Byte(), Reader.Byte(), Reader.Int16());
                        break;
                    case 24:
                        Player_BuyItemFromMall(Reader.Byte(), Reader.Byte(), Reader.Byte(), Reader.Byte(), Reader.Byte(), Reader.Text());
                        break;
                    case 26:
                        MoveItemFromPet(Reader.Int32(), Reader.Byte(), Reader.Byte());
                        break;
                    case 27:
                        MoveItemToPet(Reader.Int32(), Reader.Byte(), Reader.Byte());
                        break;
                    case 29:
                        ItemMoveInStorage(Reader.Byte(), Reader.Byte(), Reader.Int16());//Move inside guild storage
                        break;
                    case 30:
                        Player_MoveItemToStorage(Reader.Byte(), Reader.Byte(), Reader.Int32());//Move to guild storage
                        break;
                    case 31:
                        Player_MoveStorageItemToInv(Reader.Byte(), Reader.Byte(), Reader.Int32());//Move from guild storage
                        break;
                    case 32:
                        Player_GiveGoldW(iType, Reader.Int64());//Guild storage
                        break;
                    case 33:
                        Player_TakeGoldW(iType, Reader.Int64());//Guild storage
                        break;
                    case 35:
                        ItemAvatarUnEquip(Reader.Byte(), Reader.Byte());
                        break;
                    case 36:
                        ItemAvatarEquip(Reader.Byte(), Reader.Byte());
                        break;
                    default:
                        Print.Format("Unknown Item Function:{0}:{1}", iType, Decode.StringToPack(PacketInformation.buffer));
                        break;
                }
                Reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Item Switch Error {0}", ex);
            }
            #endregion
        }
    }
}