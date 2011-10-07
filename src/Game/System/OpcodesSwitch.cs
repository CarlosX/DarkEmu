///////////////////////////////////////////////////////////////////////////
// DarkEmu: Opcode switch
// Created by: http://www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////

using System;
using Framework;
using System.IO;
using System.Linq;
using System.Text;
using DarkEmu_GameServer.Network;
using System.Collections.Generic;

namespace DarkEmu_GameServer
{
    public partial class Systems
    {
        public static void oPCode(Decode de)
        {
            try
            {
                Systems sys = (Systems)de.Packet;
                sys.PacketInformation = de;
                //Console.WriteLine("Recv: (0x{0})", de.opcode.ToString("X4"));
                switch (de.opcode)
                {
                    case 0x7481:
                        break;
                    case CLIENT_PING:
                    case CLIENT_PING2:
                        break;
                    case CLIENT_PATCH:
                        sys.Patch();
                        break;
                    case CLIENT_CONNECTION:
                        sys.Connect();
                        break;
                    case CLIENT_CHARACTERSCREEN:
                        sys.CharacterScreen();
                        sys.Ping();
                        break;
                    case CLIENT_INGAME_REQUEST:
                        sys.IngameLogin();
                        break;
                    case CLIENT_INGAME_SUCCESS:
                        sys.InGameSuccess();
                        break;
                    case CLIENT_REQUEST_WEATHER:
                        sys.LoadWeather();
                        break;
                    case CLIENT_SIT:
                        sys.Doaction();
                        break;
                    case CLIENT_QUESTMARK:
                        sys.QuestionMark();
                        break;
                    case CLIENT_MOVEMENT:
                        sys.Movement();
                        break;
                    case CLIENT_ANGLE_MOVE:
                        sys.Angle();
                        break;
                    case CLIENT_SAVE_BAR:
                        sys.Save();
                        break;
                    case CLIENT_LEAVE_REQUEST:
                        sys.LeaveGame();
                        break;
                    case CLIENT_LEAVE_CANCEL:
                        sys.CancelLeaveGame();
                        break;
                    case CLIENT_ITEM_MOVE:
                        sys.ItemMain();
                        break;
                    case CLIENT_SELECT_OBJECT:
                        sys.SelectObject();
                        break;
                    case CLIENT_GM:
                        sys.GM();
                        break;
                    case CLIENT_EMOTE:
                        sys.Emote();
                        break;
                    case CLIENT_TELEPORTSTART:
                        sys.Teleport_Start();
                        break;
                    case CLIENT_TELEPORTDATA:
                        sys.Teleport_Data();
                        break;
                    case CLIENT_CHAT:
                        sys.Chat();
                        break;
                    case CLIENT_MAINACTION:
                        sys.ActionMain();
                        break;
                    case CLIENT_MASTERY_UP:
                        sys.Mastery_Up();
                        break;
                    case CLIENT_SKILL_UP:
                        sys.Mastery_Skill_Up();
                        break;
                    case CLIENT_GETUP:
                        sys.Player_Up();
                        break;
                    case CLIENT_REQUEST_PARTY:
                        sys.NormalRequest();
                        break;
                    case CLIENT_PARTY_REQUEST:
                        sys.CharacterRequest();
                        break;
                    case CLIENT_EXCHANGE_REQUEST:
                        sys.Exchange_Request();
                        break;
                    case CLIENT_EXCHANGE_WINDOWS_CLOSE:
                        sys.Exchange_Close();
                        break;
                    case CLIENT_EXCHANGE_ACCEPT:
                        sys.Exchange_Accept();
                        break;
                    case CLIENT_EXCHANGE_APPROVE:
                        sys.Exchange_Approve();
                        break;
                    case CLIENT_PARTY_ADDMEMBERS:
                        sys.PartyAddmembers();
                        break;
                    case CLIENT_PARTY_LEAVE:
                        sys.LeaveParty();
                        break;
                    case CLIENT_PARTY_BANPLAYER:
                        sys.PartyBan();
                        break;
                    case CLIENT_GUIDE:
                        sys.Gameguide();
                        break;
                    case CLIENT_PLAYER_UPDATE_INT:
                        sys.InsertInt();
                        break;
                    case CLIENT_PLAYER_UPDATE_STR:
                        sys.InsertStr();
                        break;
                    case CLIENT_PLAYER_HANDLE:
                        sys.Handle();
                        break;
                    case CLIENT_PLAYER_BERSERK:
                        sys.Player_Berserk_Up();
                        break;
                    case CLIENT_CLOSE_NPC:
                        sys.Close_NPC();
                        break;
                    case CLIENT_OPEN_NPC:
                        sys.Open_NPC();
                        break;
                    case CLIENT_NPC_BUYPACK:
                        sys.Player_BuyPack();
                        break;
                    case CLIENT_OPEN_WAREHOUSE:
                        sys.Open_Warehouse();
                        break;
                    case CLIENT_CLOSE_SCROLL:
                        sys.StopScrollTimer();
                        break;
                    case CLIENT_SAVE_PLACE:
                        sys.SavePlace();
                        break;
                    case CLIENT_ALCHEMY:
                        sys.AlchemyElixirMain();
                        break;
                    case CLIENT_ALCHEMY_CREATE_STONE:
                        sys.AlchemyCreateStone();
                        break;
                    case CLIENT_PET_MOVEMENT:
                        sys.MovementPet();
                        break;
                    case CLIENT_PET_TERMINATE:
                        sys.HandleClosePet();
                        break;
                    case CLIENT_PARTYMATCHING_LIST_REQUEST:
                        sys.ListPartyMatching(Party);
                        break;
                    case CLIENT_CREATE_FORMED_PARTY:
                        sys.CreateFormedParty();
                        break;
                    case CLIENT_FORMED_PARTY_DELETE:
                        sys.DeleteFormedParty(0);
                        break;
                    case CLIENT_JOIN_FORMED_RESPONSE:
                        sys.FormedResponse();
                        break;
                    case CLIENT_CHANGE_PARTY_NAME:
                        sys.RenameParty();
                        break;
                    case CLIENT_JOIN_FORMED_PARTY:
                        sys.JoinFormedParty();
                        break;
                    case CLIENT_START_PK:
                        sys.PkPlayer();
                        break;
                    case CLIENT_GUILD:
                        sys.GuildCreate();
                        break;
                    case CLIENT_GUILD_TRANSFER:
                        sys.GuildTransferLeaderShip();
                        break;
                    case CLIENT_GUILD_PERMISSIONS:
                        sys.GuildPermissions();
                        break;
                    case CLIENT_GUILD_PROMOTE:
                        sys.GuildPromote();
                        break;
                    case CLIENT_GUILD_DISBAND:
                        sys.GuildDisband();
                        break;
                    case CLIENT_GUILD_MESSAGE:
                        sys.GuildMessage();
                        break;
                    case CLIENT_OPEN_GUILD_STORAGE:
                        sys.GuildStorage();
                        break;
                    case CLIENT_CLOSE_GUILD_STORAGE:
                        sys.GuildStorageClose();
                        break;
                    case CLIENT_GUILD_WAR_GOLD:
                        sys.GuildWarGold();
                        break;
                    case CLIENT_OPEN_GUILD_STORAGE2:
                        sys.GuildStorage2();
                        break;
                    case CLIENT_GUILD_KICK:
                        sys.KickFromGuild();
                        break;
                    case CLIENT_GUILD_LEAVE:
                        sys.GuildLeave();
                        break;
                    case CLIENT_GUILD_TITLE_SET:
                        sys.GuildTitle();
                        break;
                    case CLIENT_GUILD_INVITE:
                        sys.GuildInvite();
                        break;
                    case CLIENT_GUILD_DONATE_GP:
                        sys.DonateGP();
                        break;
                    case CLIENT_GACHA_PLAY:
                        //Add function
                        break;
                    case CLIENT_JOIN_MERC:
                        sys.JoinMerc();
                        break;
                    case CLIENT_RANKING_LISTS:
                        sys.RankList();
                        break;
                    case CLIENT_PREV_JOB:
                        sys.PrevJob();
                        break;
                    case CLIENT_HONOR_RANK:
                        sys.HonorRank();
                        break;
                    case CLIENT_PM_MESSAGE:
                        sys.PrivateMessage();
                        break;
                    case CLIENT_PM_SEND:
                        sys.PrivateMessageSend();
                        break;
                    case CLIENT_PM_OPEN:
                        sys.PrivateMessageOpen();
                        break;
                    case CLIENT_PM_DELETE:
                        sys.PrivateMessageDelete();
                        break;
                    case CLIENT_PET_UNSUMMON:
                        sys.UnSummonPet();
                        break;
                    case CLIENT_PET_RENAME:
                        sys.RenamePet();
                        break;
                    case CLIENT_GPET_SETTINGS:
                        sys.GrabPetSettings();
                        break;
                    case CLIENT_MAKE_ALIAS:
                        sys.MakeAlias();
                        break;
                    case CLIENT_LEAVE_JOB:
                        sys.LeaveJob();
                        break;
                    case CLIENT_DISSEMBLE_ITEM:
                        sys.BreakItem();
                        break;
                    case CLIENT_STALL_OPEN:
                        sys.StallOpen();
                        break;
                    case CLIENT_STALL_CLOSE:
                        sys.StallClose();
                        break;
                    case CLIENT_STALL_BUY:
                        sys.StallBuy();
                        break;
                    case CLIENT_STALL_ACTION:
                        sys.StallMain();
                        break;
                    case CLIENT_STALL_OTHER_OPEN:
                        sys.EnterStall();
                        break;
                    case CLIENT_STALL_OTHER_CLOSE:
                        sys.LeaveStall();
                        break;
                    case CLIENT_PVP:
                        sys.StartPvpTimer(10000);
                        break;
                    case CLIENT_ALCHEMY_STONE:
                        sys.AlchemyStoneMain();
                        break;
                    case CLIENT_ITEM_MALL_WEB:
                        sys.ItemMallWeb();
                        break;
                    case CLIENT_ITEM_STORAGE_BOX:
                        sys.ItemStorageBox();
                        break;
                    case CLIENT_ITEM_BOX_LOG:
                        sys.ItemStorageBoxLog();
                        break;
                    case CLIENT_FRIEND_REMOVAL:
                        sys.FriendRemoval();
                        break;
                    case CLIENT_FRIEND_INVITE:
                        sys.FriendAdd();
                        break;
                    case CLIENT_FRIEND_GROUP:
                        sys.FriendGroup("ADD");
                        break;
                    case CLIENT_FRIEND_GROUP_REMOVE:
                        sys.FriendGroup("REMOVE");
                        break;
                    case CLIENT_FRIEND_GROUP_MANAGE_FRIEND:
                        sys.FriendGroup("MOVE");
                        break;
                    case CLIENT_FRIEND_INVITE_RESPONSE:
                        sys.FriendAddResponse();
                        break;
                    case CLIENT_UNION_APPLY:
                        sys.unionapply();
                        break;
                    case CLIENT_ICON_REQUEST:
                        sys.RequestIcons();
                        break;
                    default:
                        Print.Format("(0x{0}) {1}", de.opcode.ToString("X4"), Decode.StringToPack(sys.PacketInformation.buffer));
                        break;
                }
            }
            catch (Exception ex)
            {
                Systems.Debugger.Write(ex);
            }
        }
    }
}
