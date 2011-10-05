using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Game
{
    public partial class Manager_Chat : Form
    {
        //Enable form dragging
        #region Drag windowless form
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern bool ReleaseCapture();
        #endregion

        delegate void UpdateActivityBox(string chat);

        public Manager_Chat()
        {
            InitializeComponent();
        }
        public void ChatBox(string information)
        {
            if (ingameChat.InvokeRequired)
            {
                UpdateActivityBox del = new UpdateActivityBox(ChatBox);
                this.ingameChat.Invoke(del, new object[] { information });
            }
            else
            {
                this.ingameChat.AppendText(information + " \n");
            }
        }
        private void sndNoticeButton_Click(object sender, EventArgs e)
        {
            
            string message = sndNoticeMessage.Text;
            Systems.SendAll(Packet.ChatPacket(7, 0, message, ""));
        }

        private void Manager_Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void Manager_Chat_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0xa1, 0x2, 0);
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void ingameChat_TextChanged(object sender, EventArgs e)
        {
            this.ingameChat.ScrollToCaret();
        }
    }
}
