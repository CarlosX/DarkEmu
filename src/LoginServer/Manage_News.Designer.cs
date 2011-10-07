namespace LoginServer
{
    partial class Manage_News
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manage_News));
            this.editNews = new System.Windows.Forms.Button();
            this.rmvNews = new System.Windows.Forms.Button();
            this.nwsList = new System.Windows.Forms.ListBox();
            this.nwsTitle = new System.Windows.Forms.TextBox();
            this.nwsBody = new System.Windows.Forms.TextBox();
            this.sbmNews = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nwsDate = new System.Windows.Forms.DateTimePicker();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // editNews
            // 
            this.editNews.BackColor = System.Drawing.Color.Maroon;
            this.editNews.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("editNews.BackgroundImage")));
            this.editNews.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.editNews.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editNews.ForeColor = System.Drawing.Color.White;
            this.editNews.Image = ((System.Drawing.Image)(resources.GetObject("editNews.Image")));
            this.editNews.Location = new System.Drawing.Point(133, 334);
            this.editNews.Name = "editNews";
            this.editNews.Size = new System.Drawing.Size(47, 25);
            this.editNews.TabIndex = 20;
            this.editNews.Text = "Edit";
            this.editNews.UseVisualStyleBackColor = false;
            this.editNews.Click += new System.EventHandler(this.editNews_Click);
            // 
            // rmvNews
            // 
            this.rmvNews.BackColor = System.Drawing.Color.Maroon;
            this.rmvNews.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rmvNews.BackgroundImage")));
            this.rmvNews.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rmvNews.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rmvNews.ForeColor = System.Drawing.Color.White;
            this.rmvNews.Image = ((System.Drawing.Image)(resources.GetObject("rmvNews.Image")));
            this.rmvNews.Location = new System.Drawing.Point(186, 334);
            this.rmvNews.Name = "rmvNews";
            this.rmvNews.Size = new System.Drawing.Size(54, 25);
            this.rmvNews.TabIndex = 19;
            this.rmvNews.Text = "Delete";
            this.rmvNews.UseVisualStyleBackColor = false;
            this.rmvNews.Click += new System.EventHandler(this.rmvNews_Click);
            // 
            // nwsList
            // 
            this.nwsList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(8)))), ((int)(((byte)(8)))));
            this.nwsList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.nwsList.ForeColor = System.Drawing.Color.White;
            this.nwsList.FormattingEnabled = true;
            this.nwsList.Location = new System.Drawing.Point(63, 204);
            this.nwsList.Name = "nwsList";
            this.nwsList.Size = new System.Drawing.Size(177, 130);
            this.nwsList.TabIndex = 18;
            // 
            // nwsTitle
            // 
            this.nwsTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.nwsTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nwsTitle.ForeColor = System.Drawing.Color.White;
            this.nwsTitle.Location = new System.Drawing.Point(416, 102);
            this.nwsTitle.Name = "nwsTitle";
            this.nwsTitle.Size = new System.Drawing.Size(216, 20);
            this.nwsTitle.TabIndex = 21;
            // 
            // nwsBody
            // 
            this.nwsBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.nwsBody.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nwsBody.ForeColor = System.Drawing.Color.White;
            this.nwsBody.Location = new System.Drawing.Point(311, 144);
            this.nwsBody.Multiline = true;
            this.nwsBody.Name = "nwsBody";
            this.nwsBody.Size = new System.Drawing.Size(321, 124);
            this.nwsBody.TabIndex = 23;
            // 
            // sbmNews
            // 
            this.sbmNews.BackColor = System.Drawing.Color.Maroon;
            this.sbmNews.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("sbmNews.BackgroundImage")));
            this.sbmNews.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.sbmNews.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sbmNews.ForeColor = System.Drawing.Color.White;
            this.sbmNews.Image = ((System.Drawing.Image)(resources.GetObject("sbmNews.Image")));
            this.sbmNews.Location = new System.Drawing.Point(526, 319);
            this.sbmNews.Name = "sbmNews";
            this.sbmNews.Size = new System.Drawing.Size(106, 25);
            this.sbmNews.TabIndex = 24;
            this.sbmNews.Text = "Submit";
            this.sbmNews.UseVisualStyleBackColor = false;
            this.sbmNews.Click += new System.EventHandler(this.sbmNews_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(328, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "News Title";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(546, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "News Content";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(493, 271);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Date (New article only)";
            // 
            // nwsDate
            // 
            this.nwsDate.CalendarFont = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nwsDate.CalendarForeColor = System.Drawing.Color.Black;
            this.nwsDate.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.nwsDate.CalendarTitleForeColor = System.Drawing.Color.Black;
            this.nwsDate.Location = new System.Drawing.Point(311, 287);
            this.nwsDate.Name = "nwsDate";
            this.nwsDate.Size = new System.Drawing.Size(321, 20);
            this.nwsDate.TabIndex = 28;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Location = new System.Drawing.Point(650, 7);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(15, 13);
            this.panel2.TabIndex = 30;
            this.panel2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseClick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Location = new System.Drawing.Point(670, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(15, 13);
            this.panel1.TabIndex = 29;
            this.panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseClick);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Maroon;
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(65, 334);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 25);
            this.button1.TabIndex = 31;
            this.button1.Text = "Deselect";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Manage_News
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(700, 400);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.nwsDate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sbmNews);
            this.Controls.Add(this.nwsBody);
            this.Controls.Add(this.nwsTitle);
            this.Controls.Add(this.editNews);
            this.Controls.Add(this.rmvNews);
            this.Controls.Add(this.nwsList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Manage_News";
            this.Text = "Manage_News";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Manage_News_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button editNews;
        public System.Windows.Forms.Button rmvNews;
        public System.Windows.Forms.ListBox nwsList;
        public System.Windows.Forms.TextBox nwsTitle;
        public System.Windows.Forms.TextBox nwsBody;
        public System.Windows.Forms.Button sbmNews;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.DateTimePicker nwsDate;
        public System.Windows.Forms.Button button1;
    }
}