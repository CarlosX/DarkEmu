using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Game
{
    public partial class Load : Form
    {
        public Load()
        {
            InitializeComponent();
            Systems.mm.Show();
        }

        private void Load_Paint(object sender, PaintEventArgs e)
        {
            Hide();
        }
    }
}
