using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace LoginServer
{
    public partial class Manage_News : Form
    {
        //Enable form dragging
        #region Drag windowless form
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();
        #endregion

        public Manage_News()
        {
            InitializeComponent();
        }

        private void editNews_Click(object sender, EventArgs e)
        {
            if (nwsList.SelectedItem == null)
            {
                MessageBox.Show("You must first select a news item to edit..", "[SRX]", MessageBoxButtons.OK,MessageBoxIcon.Hand);
            }
            else
            {
                //Populate informational gui boxes
                NewsList article = Systems.getarticle(nwsList.SelectedItem.ToString());
                nwsTitle.Text = article.Head;
                nwsBody.Text = article.Msg;
            }
        }

        private void rmvNews_Click(object sender, EventArgs e)
        {
            foreach (NewsList news in Systems.News_List)
            {
                if (nwsList.SelectedItem != null)
                {
                    if (news.Head == nwsList.SelectedItem.ToString())
                    {
                        
                        string zeroday = "0";
                        string zeromonth = "0";

                        if (news.Day > 9) zeroday = "";
                        if (news.Month > 9) zeromonth = "";

                        string file = (System.Environment.CurrentDirectory + @"\Settings\News\" + news.Year + "-" + zeromonth + "" + news.Month + "-" + zeroday + "" + news.Day + ".txt");
                        File.Delete(file);
                        
                    }
                }
                else
                {
                    MessageBox.Show("You must select a article to delete first", "[SRX]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            Systems.News_List.Clear();
            nwsList.Items.Clear();
            ReloadArticles();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            Hide();
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void sbmNews_Click(object sender, EventArgs e)
        {
            if (nwsList.SelectedItem == null)
            {
                CreateNewArticle();
            }
            else
            {
                EditArticle();
            }
        }
        public void CreateNewArticle()
        {
            string zeroday = "0";
            string zeromonth = "0";
            if (nwsDate.Value.Day > 9) zeroday = "";
            if (nwsDate.Value.Month > 9) zeromonth = "";

            if (!System.IO.File.Exists(System.Environment.CurrentDirectory + @"\Settings\News\" + nwsDate.Value.Year + "-" + zeromonth + "" + nwsDate.Value.Month + "-" + zeroday + "" + nwsDate.Value.Day + ".txt"))
            {
                
                string lines = "" + nwsTitle.Text + "" + Environment.NewLine + "" + nwsBody.Text + "";
                System.IO.StreamWriter file = new System.IO.StreamWriter(System.Environment.CurrentDirectory + @"\Settings\News\" + nwsDate.Value.Year + "-" + zeromonth + "" + nwsDate.Value.Month + "-" + zeroday + "" + nwsDate.Value.Day + ".txt");
                file.WriteLine(lines);
                file.Close();
                nwsList.Items.Clear();
                Systems.News_List.Clear();
                //Reload news
                ReloadArticles();
            }
            else
            {
                MessageBox.Show("There's allready a file with this date\n Try another date.", "[SRX]", MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        public void EditArticle()
        {

            foreach (NewsList news in Systems.News_List)
            {
                if (news.Head == nwsList.SelectedItem.ToString())
                {
                    string zeroday = "0";
                    string zeromonth = "0";
                    if (nwsDate.Value.Day > 9) zeroday = "";
                    if (nwsDate.Value.Month > 9) zeromonth = "";

                    string lines = "" + nwsTitle.Text + "" + Environment.NewLine + "" + nwsBody.Text + "";
                    System.IO.StreamWriter file = new System.IO.StreamWriter(System.Environment.CurrentDirectory + @"\Settings\News\" + news.Year + "-" + zeromonth + "" + news.Month + "-" + zeroday + "" + news.Day + ".txt");
                    file.WriteLine(lines);
                    file.Close();
                    nwsList.Items.Clear();
                    Systems.News_List.Clear();
                    //Reload news
                    ReloadArticles();
                }
            }
        }
        public void ReloadArticles()
        {
            int news_count = 0;
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
                            using (StreamReader aFile = new StreamReader(fName))
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
                                        nwsList.Items.Add(nl.Head);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            nwsList.SetSelected(0, false);
        }

        private void Manage_News_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0xa1, 0x2, 0);
            }
        }
    }
}
