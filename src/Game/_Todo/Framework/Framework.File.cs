///////////////////////////////////////////////////////////////////////////
// <2011>  <DarkEmu>
// Programmed by: Xfs Games
// Website: www.xfsgames.com.ar
///////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

namespace Framework
{
    public class IniFile
    {
        public string path;
        [DllImport("kernel32")]
        public static extern short GetSystemDefaultLangID();
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            255, this.path);
            return temp.ToString();
        }
    }
    public class TxtFile
    {
            [DllImport("kernel32")]
            public static extern short GetSystemDefaultLangID();
            public static ArrayList lines = new ArrayList(20000);
            public static string[] commands = new string[20000];
            public static int amountLine;
            public static void Clear()
            {
                commands = null;
                amountLine = 0;
                lines.Clear();
            }
            public static void ReadFromFile(string filename, char splitType)
            {
                Clear();

                StreamReader SR;
                string S = "";
                try
                {
                    SR = File.OpenText(Environment.CurrentDirectory + @filename);

                    char[] sp = { splitType };
                    String curLine;

                    while ((curLine = SR.ReadLine()) != null)
                    {
                        lines.Add(curLine);
                        amountLine += 1;
                    }

                    S = lines[0].ToString();
                    commands = S.Split(sp);
                    SR.Close();
                }
                catch (Exception ex) 
                {
                    Console.WriteLine("FW.File.TxtFile.ReadFromFile: {0}", ex);
                }
            }
    }
}
