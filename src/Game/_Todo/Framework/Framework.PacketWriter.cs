///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Framework
{

    public class PacketWriter
    {

        MemoryStream ms = new MemoryStream();
        BinaryWriter bw;

        public PacketWriter()
        {

        }
        public void AddBuffer(byte[] buffer)
        {
            bw.Write(buffer);
        }
        public PacketWriter(bool b)
        {
            if (b)
            {
                bw = null;
                ms = null;
                ms = new MemoryStream();

                bw = new BinaryWriter(ms);
                bw.Write((ushort)0);
            }
        }
        public void Textlen(string data)
        {
            Word((short)data.Length / 2);//Added for Isro Server Notice
        }
        public void Stringadd(string data)
        {
            char[] chars = new char[data.Length * 2];

            for (int x = 0; x < data.Length; x++)
            {
                chars[x] = Convert.ToChar(data.Substring(x, 1));
                bw.Write(chars[x]);
            }
        }
        public int Length()
        {
            return (int)(ms.Position - 6);
        }
        public void Byte(byte data)
        {
            bw.Write(data);

        }
        public void Byte(object data)
        {
            bw.Write(Convert.ToByte(data));

        }
        public void Create(ushort opcode)
        {
            bw = null;
            ms = null;
            ms = new MemoryStream();
            bw = new BinaryWriter(ms);

            bw.Write((ushort)0);

            Word(opcode);
            Word(0);
        }
        public void Word(ushort data)
        {
            bw.Write(data);

        }
        public void Word(object data)
        {
            try
            {
                if (Convert.ToInt32(data) < 0)
                {
                    bw.Write(Convert.ToInt16(data));
                }
                else
                {
                    bw.Write(Convert.ToUInt16(data));
                }
            }catch(Exception ep)
            {
            }
        }
        public void Word(short data)
        {
            bw.Write(data);
        }
        public void DWord(uint data)
        {
            bw.Write(data);
        }
        public void DWord(object data)
        {
            if (Convert.ToInt64(data) > 0)
            {
                bw.Write(Convert.ToUInt32(data));

            }
            else
            {
                bw.Write(Convert.ToInt32(data));

            }
        }
        public void DWord(int data)
        {
            bw.Write(data);

        }
        public void LWord(ulong data)
        {
            bw.Write(data);

        }
        public void LWord(object data)
        {
            bw.Write(Convert.ToInt64(data));

        }
        public void LWord(long data)
        {
            bw.Write(data);

        }
        public void Float(float data)
        {
            bw.Write(data);

        }
        public void Float(object data)
        {
            bw.Write(Convert.ToSingle(data));

        }
        public void FloatFour(float data)
        {
            bw.Write(data);

        }
        public void Text3(string data)
        {
            Word((short)data.Length);//Added for Isro each letter is a word
            Stringu(data);
        }

        public void Text(object data)
        {
            Word((short)Convert.ToString(data).Length);
            String((string)data);
        }
        public void Bool(bool b)
        {
            bw.Write(b);

        }
        public void Bool(object b)
        {
            bw.Write((bool)b);

        }
        public void Buffer(byte[] b)
        {
            bw.Write(b, 0, b.Length);
        }

        public void Stringu(string data)
        {
            byte[] chars = Encoding.Unicode.GetBytes(data);
            for (int x = 0; x < data.Length * 2; x++)
            {
                bw.Write(chars[x]);
                bw.Write(false);
                x = x + 1;
            }
        }

        public void String(string data)
        {
            char[] chars = new char[data.Length];

            for (int x = 0; x < data.Length; x++)
            {
                chars[x] = Convert.ToChar(data.Substring(x, 1));
                bw.Write(chars[x]);
            }
        }
        public void StringTest(string data)
        {
            char[] chars = new char[data.Length];

            for (int x = 0; x < data.Length; x++)
            {
                chars[x] = Convert.ToChar(data.Substring(x, 1));
                bw.Write(chars[x]);
            }
        }

        public void UString(string data)
        {
            char[] chars = new char[data.Length];

            for (int x = 0; x < data.Length; x++)
            {
                chars[x] = Convert.ToChar(data.Substring(x, 1));
                bw.Write(chars[x]);
                bw.Write((byte)0);
            }
        }

        public void HexString(string data)
        {
            char[] chars = new char[data.Length];

            for (int x = 0; x < data.Length; x++)
            {
                chars[x] = Convert.ToChar(data.Substring(x, 1));
                bw.Write(chars[x]);
            }
        }

        public byte[] GetBytes()
        {

            byte[] data = { 0 };
            ushort len = (ushort)(ms.Position - 6);
            ms.Position = 0;

            bw.Write(len);
            bw.Flush();
            bw.Close();
            data = ms.ToArray();
            ms.Close();
            return data;
        }
        public short InvShort(short check)
        {
            int[] sort = new int[8];
            for (int i = 0; i < 8; i++)
            {
                if ((check & Convert.ToInt32((Math.Pow(2, i)))) != 0)
                {
                    sort[7 - i] = 1;
                }
                else
                {
                    sort[7 - i] = 0;
                }
            }
            double res = 0;

            for (int i = 0; i < 8; i++)
            {
                res += sort[i] * Math.Pow(2, i);
            }
            return (short)res;
        }
    }
}

