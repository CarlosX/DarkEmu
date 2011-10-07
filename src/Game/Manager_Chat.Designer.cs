namespace Game
{
    partial class Manager_Chat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manager_Chat));
            this.ingameChat = new System.Windows.Forms.RichTextBox();
            this.sndNoticeMessage = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sndNoticeButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // ingameChat
            // 
            this.ingameChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.ingameChat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ingameChat.ForeColor = System.Drawing.Color.White;
            this.ingameChat.Location = new System.Drawing.Point(43, 162);
            this.ingameChat.Name = "ingameChat";
            this.ingameChat.Size = new System.Drawing.Size(519, 305);
            this.ingameChat.TabIndex = 0;
            this.ingameChat.Text = "";
            this.ingameChat.TextChanged += new System.EventHandler(this.ingameChat_TextChanged);
            // 
            // sndNoticeMessage
            // 
            this.sndNoticeMessage.BackColor = System.Drawing.Color.Maroon;
            this.sndNoticeMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sndNoticeMessage.ForeColor = System.Drawing.Color.White;
            this.sndNoticeMessage.Location = new System.Drawing.Point(213, 79);
            this.sndNoticeMessage.Name = "sndNoticeMessage";
            this.sndNoticeMessage.Size = new System.Drawing.Size(222, 21);
            this.sndNoticeMessage.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(165, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Notice";
            // 
            // sndNoticeButton
            // 
            this.sndNoticeButton.BackgroundImage = global::Game.Properties.Resources.bgbutton;
            this.sndNoticeButton.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.sndNoticeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sndNoticeButton.Location = new System.Drawing.Point(441, 75);
            this.sndNoticeButton.Name = "sndNoticeButton";
            this.sndNoticeButton.Size = new System.Drawing.Size(96, 27);
            this.sndNoticeButton.TabIndex = 8;
            this.sndNoticeButton.Text = "Send";
            this.sndNoticeButton.UseVisualStyleBackColor = true;
            this.sndNoticeButton.Click += new System.EventHandler(this.sndNoticeButton_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Location = new System.Drawing.Point(568, 10);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(16, 15);
            this.panel2.TabIndex = 11;
            this.panel2.Click += new System.EventHandler(this.panel2_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Location = new System.Drawing.Point(540, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(16, 15);
            this.panel1.TabIndex = 10;
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            // 
            // Manager_Chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BackgroundImage = global::Game.Properties.Resources.bg_chat;
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.sndNoticeButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sndNoticeMessage);
            this.Controls.Add(this.ingameChat);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Manager_Chat";
            this.ShowIcon = false;
            this.Text = "..:: [SRX] ::.. Chat Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Manager_Chat_FormClosing);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Manager_Chat_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.RichTextBox ingameChat;
        public System.Windows.Forms.TextBox sndNoticeMessage;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Button sndNoticeButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
    }
}