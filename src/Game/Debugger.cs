///////////////////////////////////////////////////////////////////////////
// Srevolution 2010 / 2011
// Programmed by: Xsense
// Website: www.xcoding.net
// File info: Public packet data
///////////////////////////////////////////////////////////////////////////
using System;
using Framework;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Game
{
    public partial class Systems
    {
        public Systems()
        {
            // TODO: Complete member initialization
        }
    }

    public class deBug
    {
        public static void Write(Exception ex)
        {
            try
            {
                string debuginfo = String.Format("[{0}] [MESSAGE]: {1} [TRACE]: {2}", DateTime.Now.ToString(), ex.Message, ex.StackTrace);
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
            
