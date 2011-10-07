///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
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
        public static byte[] ActionPacket(byte type1, byte type2, int skillid, int ownerid, int castingid, int target)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ACTION_DATA);
            Writer.Byte(type1);
            Writer.Byte(type2);
            Writer.Byte(0x30);

            Writer.DWord(skillid);
            Writer.DWord(ownerid);
            Writer.DWord(castingid);
            Writer.DWord(target);
            Writer.Byte(0);

            return Writer.GetBytes();
        }

        public static byte[] ActionPacket(byte type1, byte type2)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_ACTION_DATA);
            Writer.Byte(type1);
            Writer.Byte(type2);
            Writer.Byte(0x30);
            return Writer.GetBytes();
        }
        public static byte[] IngameMessages2(ushort opcode, ushort id)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(opcode);
            Writer.Byte(3);
            Writer.Byte(0);
            Writer.Word(id);
            return Writer.GetBytes();
        }
        public static byte[] SkillPacket(byte type, int castingid, int ownerid = 0)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SKILL_DATA);
            Writer.Byte(1);
            Writer.DWord(castingid);
            Writer.Byte(type);
            switch (type)
            {
                case 0:
                    Writer.DWord(ownerid);
                    break;
                default:
                    Console.WriteLine("Skill packet case: " + type);
                    break;
            }
            return Writer.GetBytes();
        }
        public static byte[] SkillIconPacket(int ownerid, int skillid, int overid, bool eu)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SKILL_ICON);
            Writer.DWord(ownerid);
            Writer.DWord(skillid);
            Writer.DWord(overid);
            if (eu) Writer.Byte(1);
            return Writer.GetBytes();
        }
        public static byte[] SkillEndBuffPacket(int overid)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SKILL_ENDBUFF);
            Writer.Bool(true);
            Writer.DWord(overid);
            return Writer.GetBytes();
        }

        public static byte[] EffectUpdate(int objectid, Effect.EffectNumbers effectid, bool start)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_SKILL_EFFECTS);
            Writer.DWord(objectid);
            Writer.Byte(1);
            Writer.Bool(start);
            Writer.Byte(4); // effect change
            Writer.DWord(effectid);
            return Writer.GetBytes();
        }
        public static byte[] Effects2Dmg(int id, int dmg)
        {
            PacketWriter Writer = new PacketWriter();
            Writer.Create(Systems.SERVER_EFFECT_DAMAGE);
            Writer.DWord(id);
            Writer.DWord(dmg);
            return Writer.GetBytes();
        }
    }
}
