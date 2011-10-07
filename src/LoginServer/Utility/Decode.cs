using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace LoginServer
{
    public partial class Systems
    {
        public class Decode
        {
            private ushort OPCODE;

            private byte[] BUFFER;
            private Socket socket;
            private object NET;
            private object packet;
            public UInt16 dataSize;
            public ushort opcode
            {
                get { return OPCODE; }
            }
            public byte[] buffer
            {
                get { return BUFFER; }
            }
            public Socket Client
            {
                get { return socket; }
            }
            public object Networking
            {
                get { return NET; }
            }
            public object Packet
            {
                get { return packet; }
            }
            MemoryStream ms;
            BinaryReader br;

            public Decode(byte[] buffer)
            {
                try
                {
                    ms = new MemoryStream(buffer);
                    br = new BinaryReader(ms);
                    dataSize = br.ReadUInt16();
                    br.Close();
                    ms.Close();
                    br.Dispose();
                    ms.Dispose();
                }
                catch (Exception) { }
            } 

            public Decode(Socket wSock, byte[] buffer, Client net, object packetf)
            {
                try
                {
                    UInt16 security;
                    packet = packetf;

                        ms = new MemoryStream(buffer);
                        br = new BinaryReader(ms);

                        dataSize = br.ReadUInt16();

                        byte[] b = new byte[dataSize];
                        Array.Copy(buffer, 6, b, 0, dataSize);

                        BUFFER = b;
                        OPCODE = br.ReadUInt16();

                        security = br.ReadUInt16();
                    
                    socket = wSock;
                    NET = net;
                }
                catch (Exception)
                {
                }
            }
            public static string StringToPack(byte[] buff)
            {
                string s = null;
                foreach (byte b in buff) s += b.ToString("X2");
                return s;
            }
        }
    }
}
