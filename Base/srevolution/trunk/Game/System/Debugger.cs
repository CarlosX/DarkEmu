///////////////////////////////////////////////////////////////////////////
// SRX Revo: Debugger system
// Created by: http://xcoding.net
///////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace SrxRevo
{
    public partial class Systems
    {
        public Systems()
        {
        }

        public class Debugger
        {
            public static void Write(Exception ex)
            {
                try
                {
                    string debuginfo = String.Format("[{0}] [MESSAGE]: {1} [SOURCE]: {2}", DateTime.Now.ToString(), ex, ex.Source);
                    StreamWriter Writer = System.IO.File.AppendText(Environment.CurrentDirectory + @"\Error List.txt");
                    Writer.WriteLine(debuginfo);
                    Writer.Close();
                }
                catch
                {
                    Console.WriteLine("Debugger System: ({0})", DateTime.Now.ToString());
                }
            }
        }
    }
}