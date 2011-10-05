namespace LoginServer
{
    partial class Manage_Servers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manage_Servers));
            this.serverName = new System.Windows.Forms.TextBox();
            this.serverIP = new System.Windows.Forms.TextBox();
            this.protCode = new System.Windows.Forms.TextBox();
            this.serverPort = new System.Windows.Forms.TextBox();
            this.srvList = new System.Windows.Forms.ListBox();
            this.rmvServer = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.dbName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // serverName
            // 
            this.serverName.Location = new System.Drawing.Point(481, 240);
            this.serverName.Name = "serverName";
            this.serverName.Size = new System.Drawing.Size(152, 20);
            this.serverName.TabIndex = 1;
            // 
            // serverIP
            // 
            this.serverIP.Location = new System.Drawing.Point(481, 188);
            this.serverIP.Name = "serverIP";
            this.serverIP.Size = new System.Drawing.Size(152, 20);
            this.serverIP.TabIndex = 2;
            // 
            // protCode
            // 
            this.protCode.Location = new System.Drawing.Point(481, 266);
            this.protCode.Name = "protCode";
            this.protCode.Size = new System.Drawing.Size(152, 20);
            this.protCode.TabIndex = 3;
            // 
            // serverPort
            // 
            this.serverPort.Location = new System.Drawing.Point(481, 214);
            this.serverPort.Name = "serverPort";
            this.serverPort.Size = new System.Drawing.Size(152, 20);
            this.serverPort.TabIndex = 4;
            // 
            // srvList
            // 
            this.srvList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(8)))), ((int)(((byte)(8)))));
            this.srvList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.srvList.ForeColor = System.Drawing.Color.White;
            this.srvList.FormattingEnabled = true;
            this.srvList.Location = new System.Drawing.Point(66, 201);
            this.srvList.Name = "srvList";
            this.srvList.Size = new System.Drawing.Size(177, 130);
            this.srvList.TabIndex = 5;
            // 
            // rmvServer
            // 
            this.rmvServer.BackColor = System.Drawing.Color.Maroon;
            this.rmvServer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rmvServer.BackgroundImage")));
            this.rmvServer.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.rmvServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rmvServer.ForeColor = System.Drawing.Color.White;
            this.rmvServer.Location = new System.Drawing.Point(137, 331);
            this.rmvServer.Name = "rmvServer";
            this.rmvServer.Size = new System.Drawing.Size(106, 25);
            this.rmvServer.TabIndex = 6;
            this.rmvServer.Text = "Remove Selected";
            this.rmvServer.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(414, 191);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Server ip";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(392, 243);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Server name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(397, 269);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Server code";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(401, 217);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Server port";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Location = new System.Drawing.Point(649, 7);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(15, 13);
            this.panel2.TabIndex = 16;
            this.panel2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseClick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Location = new System.Drawing.Point(669, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(15, 13);
            this.panel1.TabIndex = 15;
            this.panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseClick);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Maroon;
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(84, 331);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(47, 25);
            this.button1.TabIndex = 17;
            this.button1.Text = "Edit";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Maroon;
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(527, 331);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(106, 25);
            this.button2.TabIndex = 18;
            this.button2.Text = "Submit Information";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(377, 295);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Database name";
            // 
            // dbName
            // 
            this.dbName.Location = new System.Drawing.Point(481, 292);
            this.dbName.Name = "dbName";
            this.dbName.Size = new System.Drawing.Size(152, 20);
            this.dbName.TabIndex = 19;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(321, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(281, 39);
            this.label6.TabIndex = 21;
            this.label6.Text = "Information:\r\n- When you have not selected to edit a server\r\n- All information be" +
                "low will create a new server.";
            // 
            // Manage_Servers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(700, 400);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dbName);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rmvServer);
            this.Controls.Add(this.srvList);
            this.Controls.Add(this.serverPort);
            this.Controls.Add(this.protCode);
            this.Controls.Add(this.serverIP);
            this.Controls.Add(this.serverName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Manage_Servers";
            this.Text = "[SRX] Server manager";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Manage_Servers_MouseDown_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox serverName;
        public System.Windows.Forms.TextBox serverIP;
        public System.Windows.Forms.TextBox protCode;
        public System.Windows.Forms.TextBox serverPort;
        public System.Windows.Forms.ListBox srvList;
        public System.Windows.Forms.Button rmvServer;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox dbName;
        public System.Windows.Forms.Label label6;
    }
}