using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace LoginServer
{
    public partial class Manage_Servers : Form
    {
        //Enable form dragging
        #region Drag windowless form
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();
        #endregion

        public Manage_Servers()
        {
            InitializeComponent();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            Hide();
        }
        private void Manage_Servers_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0xa1, 0x2, 0);
            }
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (srvList.SelectedItem == null)
            {
                MessageBox.Show("You must select a server to edit!", "[SRX]", MessageBoxButtons.OK);
            }
            else
            {
                Systems.SRX_Serverinfo information = Getserverbyname(srvList.SelectedItem.ToString());
                serverIP.Text = information.ip;
                serverName.Text = information.name;
                serverPort.Text = information.port.ToString();
                protCode.Text = information.code;
                //Add db by command console from gs
            }
        }
        public static Systems.SRX_Serverinfo Getserverbyname(string name)
        {
            Systems.SRX_Serverinfo GS = null;

            foreach (KeyValuePair<int, Systems.SRX_Serverinfo> GSI in Systems.GSList)
            {
                if (GSI.Value.name == name)
                {
                    GS = GSI.Value;
                }
            }
            return GS;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (srvList.SelectedItem != null)
            {
                //Edit excisting server

            }
            else
            {
                //Create new server

            }
        }
    }
}
