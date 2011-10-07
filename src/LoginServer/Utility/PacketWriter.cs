using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LoginServer
{
    public partial class Systems
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
            public int Length()
            {
                return (int)(ms.Position - 6);
            }
            public void Byte(byte data)
            {
                bw.Write(data);
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
            public void Word(short data)
            {
                bw.Write(data);
            }
            public void WordInt(short data)
            {
                bw.Write(data);
            }
            public void DWord(uint data)
            {
                bw.Write(data);
            }
            public void DWord(int data)
            {
                bw.Write(data);
            }
            public void DWordInt(int data)
            {
                bw.Write(data);
            }
            public void LWord(ulong data)
            {
                bw.Write(data);
            }
            public void LWord(long data)
            {
                bw.Write(data);
            }
            public void Float(float data)
            {
                bw.Write(data);
            }
            public void FloatFour(float data)
            {
                bw.Write(data);
            }
            public void Text(string data)
            {
                Word((short)data.Length);
                String(data);
            }
            public void Bool(bool b)
            {
                bw.Write(b);
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
        }
    }
}
