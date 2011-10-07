///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Team Srevolution
// Website: www.xcoding.net
///////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Framework
{
    public class PacketReader
    {
        MemoryStream ms;
        BinaryReader br;

        public PacketReader(byte[] data)
        {
            ms = new MemoryStream(data);
            br = new BinaryReader(ms);
        }
        public byte Byte()
        {
            return br.ReadByte();
        }
        public ushort UInt16()
        {
            return br.ReadUInt16();
        }
        public uint UInt32()
        {
            return br.ReadUInt32();
        }
        public ulong UInt64()
        {
            return br.ReadUInt64();
        }
        public short Int16()
        {
            return br.ReadInt16();
        }
        public int Int32()
        {
            return br.ReadInt32();
        }
        public long Int64()
        {
            return br.ReadInt64();
        }
        public float Single()
        {
            return br.ReadSingle();
        }

        public string Stringu(int len)
        {
            byte[] chars = br.ReadBytes(len);
            string converted = Encoding.Unicode.GetString(chars);
            return converted;
        }
        
        public string String(int len)
        {
            StringBuilder sb = new StringBuilder();
            char[] chars = br.ReadChars(len);
            foreach (char c in chars)
            {
                sb.Append(c.ToString());
            }
            
            return sb.ToString();
        }
        public string Text()
        {
            int len = Int16();
            return String(len);
        }
        public string Text3()//Added for Isro each letter is a word
        {
            int len = Int16() * 2;
            return Stringu((short)len);
        }
        public void Skip(int HowMany)
        {
            for (int x = 1; x <= HowMany; x++)
            {
                br.ReadByte();
            }
        }

        public void Close()
        {
            br.Dispose();
            ms.Dispose();
            br.Close();
            ms.Close();
            GC.Collect(GC.GetGeneration(this));
        }
    }
}
