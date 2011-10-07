using System;
using System.IO;

namespace DarkEmu_GameServer
{
    public class Debugx
    {

        public static void DumpBuffer(byte[] buffer, string t)
        {
            // If no logtype was specified, we default it to NetworkComms
            //DumpBuffer(buffer, 4);
        }
        public static DateTime d = new DateTime(2011, 10, 4);
        public static void DumpBuffer(byte[] buffer, int p, uint op, uint lng)
        {
            StringWriter sw = new StringWriter();
            sw = DumpBuffer(buffer, buffer.Length);
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Packet: {0} Opcode:{1} LngP:{2} LengB:{3}", p == 1 ? "C->" : "S->", op, lng, buffer.Length);
            Console.WriteLine("{0}", sw.GetStringBuilder());
            Console.WriteLine("---------------------------------------------------");
            tw.WriteLine("Packet: {0} Opcode:{1} LngP:{2} LengB:{3}", p == 1 ? "C->" : "S->", op, lng, buffer.Length);
            tw.Write(sw.GetStringBuilder());
            tw.WriteLine();
        }
        public static TextWriter tw = new StreamWriter("Debug_GameServer_" + d.Day.ToString() + d.Month.ToString() + ".txt");
        public static StringWriter DumpBuffer(byte[] buffer, int Length)
        {
            StringWriter tw = new StringWriter();
            int Count;
            int j = 0;
            lock (tw)
            {
                while (true)
                {
                    Count = Length - j;
                    if (Count > 16)
                        Count = 16;
                    else if (Count < 1)
                        break;

                    tw.Write("{0:X8} - ", j);

                    for (int i = 0; i < 16; ++i)
                    {
                        if (i < Count)
                            tw.Write("{0:X2} ", buffer[j + i]);
                        else
                            tw.Write("   ");
                    }
                    //  Print the printable characters, or a period if unprintable.
                    for (int i = 0; i < Count; ++i)
                    {
                        byte Current = buffer[j + i];
                        if ((Current < 32) || (Current > 126))
                            tw.Write(".");
                        else
                            tw.Write((char)Current);
                    }
                    tw.Write(Environment.NewLine);
                    j += 16;
                }
            }
            return tw;
        }
    }
}
