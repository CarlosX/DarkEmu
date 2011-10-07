///////////////////////////////////////////////////////////////////////////
// PROJECT SRX
// HOMEPAGE: WWW.XCODING.NET
///////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoginServer
{
    public partial class Systems
    {
        public static List<NewsList> News_List = new List<NewsList>();

        public static NewsList getarticle(string title)
        {
            foreach (NewsList article in News_List)
            {
                if (article.Head == title) return article;
            }
            return null;
        }
        public static void Load_NewsList()
        {
            try
            {
                int news_count = 0;
                try
                {
                    string[] fileEntries = Directory.GetFiles(Environment.CurrentDirectory + @"\Settings\News", @"????-??-??.*");
                    if (fileEntries.Length > 0)
                    {
                        Array.Sort(fileEntries);
                        Array.Reverse(fileEntries);

                        foreach (string fName in fileEntries)
                        {
                            if (news_count < 10)
                            {
                                DateTime aDate;
                                if (DateTime.TryParse(Path.GetFileNameWithoutExtension(fName), out aDate))
                                {
                                    using (StreamReader aFile = new StreamReader(fName, Encoding.ASCII))
                                    {
                                        string line = aFile.ReadLine(); // read Headline
                                        if (line != null)
                                        {
                                            string line2 = aFile.ReadToEnd(); // read Content
                                            if (line2 != null)
                                            {
                                                NewsList nl = new NewsList();
                                                nl.Head = line;
                                                nl.Msg = line2;
                                                nl.Day = (short)aDate.Day;
                                                nl.Month = (short)aDate.Month;
                                                nl.Year = (short)aDate.Year;
                                                Systems.News_List.Add(nl);
                                                news_count++;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Please remove old news, only 10 are loaded!");
                            }
                        }
                    }
                }
                catch { }
                string articles = "Article";
                if (news_count > 1) articles = "Articles";
                Console.WriteLine("[INFO] Loaded " + news_count + " news " + articles + "");
            }
            catch (Exception ex)
            {
                Console.WriteLine("News error " + ex);
            }
        }

    }


    public class NewsList
    {
        public int NewsID;
        public string Head, Msg;
        public short Day, Month, Year;
    }
}
