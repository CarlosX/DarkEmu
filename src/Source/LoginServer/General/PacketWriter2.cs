using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DarkEmu_LoginServer
{
    public unsafe class PacketWriter2
    {
        private MemoryStream ms;
        private bool lenIsSet = false;
        public PacketWriter2()
        {
            ms = new MemoryStream();
            //Set 0x0000 to header
            this.Write((ushort)0);  //Packet Length
            this.Write((ushort)0);  //Packet Code
            this.Write((ushort)0);  //Packet Security Seed
        }

        public PacketWriter2(ushort code)
        {
            ms = new MemoryStream();
            this.Write((ushort)0);     //Packet Length
            this.Write((ushort)code);  //Packet Code
            this.Write((ushort)0);     //Packet Security Seed
        }
        public PacketWriter2(ushort code, ushort length, bool propio)
        {
            ms = new MemoryStream();
            this.Write((ushort)length);
            this.Write((ushort)code);  //Packet Code
            lenIsSet = true;
        }

        public PacketWriter2(ushort code, ushort length)
        {
            ms = new MemoryStream();
            this.Write((ushort)length);  //Packet Length
            this.Write((ushort)code);    //Packet Code
            this.Write((ushort)0);       //Packet Security Seed
            lenIsSet = true;
        }

        public void SetLength()
        {
            if (!lenIsSet)
            {
                ms.Position = 0;
                ms.Write(BitConverter.GetBytes(Convert.ToUInt16(ms.Length)), 0, 2);
            }
        }

        public ushort Length
        {
            get { return (ushort)ms.Length; }
        }

        public void Write(ushort data)
        {
            ms.Write(BitConverter.GetBytes(data), 0, 2);
        }

        public void Write(uint data)
        {
            ms.Write(BitConverter.GetBytes(data), 0, 4);
        }
        public void AddInt16(short i)
        {
            ms.Write(BitConverter.GetBytes(i), 0, 2);
        }
        public void AddInt32(int i)
        {
            ms.Write(BitConverter.GetBytes(i), 0, 4);
        }
        
        public void Write(ulong data)
        {
            ms.Write(BitConverter.GetBytes(data), 0, 8);
        }

        public void Write(byte data)
        {
            ms.Write(BitConverter.GetBytes(data), 0, 1);
        }

        public void Write(string data1)
        {
            //byte[] data = tools.FromHex(data1);
            //ms.Write(data, 0, data.Length);
        }
        public void Write(byte data, string cero)
        {
            ms.Write(BitConverter.GetBytes(data), 0, 2);
        }

        public void Write(float data)
        {
            ms.Write(BitConverter.GetBytes(data), 0, 4);
        }

        public void Write(byte[] data)
        {
            ms.Write(data, 0, data.Length);
        }

        public void WriteASCII(string data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            ms.Write(bytes, 0, bytes.Length);
        }
        public void AddString(string s, int size)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            ms.Write(bytes, 0, bytes.Length);

            AddByteTimes(0x00, size - s.Length);
        }
        internal void AddEnum8(Enum message)
        {
            AddByte((byte)(int)(object)message);
        }
        public void AddByte(byte b)
        {
            ms.Write(BitConverter.GetBytes(b), 0, 1);
        }
        public void AddByteTimes(byte b, int times)
        {
            for(int i=0;i<times;i++)
            {
             ms.Write(BitConverter.GetBytes(b), 0, 1);
            }
        }
        public void WriteUnicode(string data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);
            ms.Write(bytes, 0, bytes.Length);
        }

        public byte[] GetBytes()
        {
            return ms.ToArray();
        }

        public void Close()
        {
            ms.Close();
        }

        public enum StringEncoding { ASCII = 1, UNICODE = 2 }

    }
}
