///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Packet
    {
        public static byte[] FriendGroupManage(string type,string groupname, short groupid, int targetid)
        {
            PacketWriter Writer = new PacketWriter();
            switch (type)
            {
                case "ADD":
                    Writer.Create(Systems.SERVER_FRIEND_GROUP);
                    Writer.Byte(1);
                    Writer.Text(groupname);
                    Writer.Word(groupid);
                    break;
                case "REMOVE":
                    Writer.Create(Systems.SERVER_FRIEND_GROUP_REMOVE);
                    Writer.Byte(1);
                    Writer.Word(groupid);
                    break;
                case "MOVE":
                    Writer.Create(Systems.SERVER_FRIEND_GROUP_MANAGE_FRIEND);
                    Writer.Byte(1);
                    Writer.DWord(targetid);
                    Writer.Word(groupid);
                    break;
            }
            return Writer.GetBytes();
        }

        public static byte[] FriendData(int target, byte type, string name, character c, bool state)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_FRIEND_DATA);
            switch (type)
            {
                case 2:
                    //Friend invite accepted
                    Writer.Byte(2);
                    Writer.DWord(target); //Id
                    Writer.Text(c.Information.Name);  //Name
                    Writer.DWord(c.Information.Model); //Model
                    break;
                case 3:
                    //Remove friend from own list
                    Writer.Byte(3);
                    Writer.DWord(target);
                    break;
                case 4:
                    //Fried online / offline update
                    Writer.Byte(4);
                    Writer.DWord(target);
                    Writer.Byte(state);
                    break;
                case 5:
                    //Send private message..
                    Writer.Byte(8);
                    Writer.Text(name);
                    Writer.DWord(0);
                    Writer.DWord(0);//time info
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] FriendDecline(string name)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_FRIEND_INVITE);
            Writer.Byte(2);
            Writer.Word(0x640B);
            Writer.Byte(0);
            Writer.Word(0x000B);
            Writer.Text(name);
            return Writer.GetBytes();
        }
        public static byte[] FriendInviteTarget(character c)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_FRIEND_INVITE_SEND);
            Writer.DWord(c.Information.CharacterID);
            Writer.DWord(c.Information.UniqueID);
            Writer.Text(c.Information.Name);
            return Writer.GetBytes();
        }
        public static byte[] FriendRemovalTarget(int target)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_FRIEND_REMOVAL_TARGET);
            Writer.Byte(1);
            Writer.DWord(target);
            return Writer.GetBytes();
        }
    }
}
