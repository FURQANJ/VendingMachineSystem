namespace project_edp
{
    partial class FormPayment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayment));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.groupBoxPayment = new System.Windows.Forms.GroupBox();
            this.radioEWallet = new System.Windows.Forms.RadioButton();
            this.radioQR = new System.Windows.Forms.RadioButton();
            this.radioOnlineBanking = new System.Windows.Forms.RadioButton();
            this.btnProceed = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelEwallet = new System.Windows.Forms.Panel();
            this.comboBank = new System.Windows.Forms.ComboBox();
            this.comboEWallet = new System.Windows.Forms.ComboBox();
            this.picQR = new System.Windows.Forms.PictureBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.labelCategories = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.labelChoose = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel8.SuspendLayout();
            this.groupBoxPayment.SuspendLayout();
            this.panelEwallet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Pink;
            this.panel1.Controls.Add(this.panel8);
            this.panel1.Location = new System.Drawing.Point(12, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1113, 436);
            this.panel1.TabIndex = 2;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(247)))));
            this.panel8.Controls.Add(this.groupBoxPayment);
            this.panel8.Controls.Add(this.btnProceed);
            this.panel8.Controls.Add(this.btnBack);
            this.panel8.Controls.Add(this.labelTitle);
            this.panel8.Controls.Add(this.panelEwallet);
            this.panel8.Location = new System.Drawing.Point(14, 3);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(1085, 430);
            this.panel8.TabIndex = 5;
            // 
            // groupBoxPayment
            // 
            this.groupBoxPayment.Controls.Add(this.radioEWallet);
            this.groupBoxPayment.Controls.Add(this.radioQR);
            this.groupBoxPayment.Controls.Add(this.radioOnlineBanking);
            this.groupBoxPayment.Location = new System.Drawing.Point(22, 84);
            this.groupBoxPayment.Name = "groupBoxPayment";
            this.groupBoxPayment.Size = new System.Drawing.Size(255, 100);
            this.groupBoxPayment.TabIndex = 8;
            this.groupBoxPayment.TabStop = false;
            this.groupBoxPayment.Text = "Please choose Payment ?Methode";
            // 
            // radioEWallet
            // 
            this.radioEWallet.AutoSize = true;
            this.radioEWallet.Location = new System.Drawing.Point(28, 21);
            this.radioEWallet.Name = "radioEWallet";
            this.radioEWallet.Size = new System.Drawing.Size(79, 20);
            this.radioEWallet.TabIndex = 5;
            this.radioEWallet.TabStop = true;
            this.radioEWallet.Text = "E-Wallet";
            this.radioEWallet.UseVisualStyleBackColor = true;
            this.radioEWallet.CheckedChanged += new System.EventHandler(this.radioEwallet_CheckedChanged);
            // 
            // radioQR
            // 
            this.radioQR.AutoSize = true;
            this.radioQR.Location = new System.Drawing.Point(28, 73);
            this.radioQR.Name = "radioQR";
            this.radioQR.Size = new System.Drawing.Size(82, 20);
            this.radioQR.TabIndex = 7;
            this.radioQR.TabStop = true;
            this.radioQR.Text = "QR code";
            this.radioQR.UseVisualStyleBackColor = true;
            this.radioQR.CheckedChanged += new System.EventHandler(this.radioQR_CheckedChanged);
            // 
            // radioOnlineBanking
            // 
            this.radioOnlineBanking.AutoSize = true;
            this.radioOnlineBanking.Location = new System.Drawing.Point(28, 47);
            this.radioOnlineBanking.Name = "radioOnlineBanking";
            this.radioOnlineBanking.Size = new System.Drawing.Size(118, 20);
            this.radioOnlineBanking.TabIndex = 6;
            this.radioOnlineBanking.TabStop = true;
            this.radioOnlineBanking.Text = "Online Banking";
            this.radioOnlineBanking.UseVisualStyleBackColor = true;
            this.radioOnlineBanking.CheckedChanged += new System.EventHandler(this.radioOnline_CheckedChanged);
            // 
            // btnProceed
            // 
            this.btnProceed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(233)))));
            this.btnProceed.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnProceed.Location = new System.Drawing.Point(867, 385);
            this.btnProceed.Name = "btnProceed";
            this.btnProceed.Size = new System.Drawing.Size(212, 35);
            this.btnProceed.TabIndex = 4;
            this.btnProceed.Text = "Proceed";
            this.btnProceed.UseVisualStyleBackColor = false;
            this.btnProceed.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(233)))));
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBack.Location = new System.Drawing.Point(22, 383);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(212, 35);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "<-- Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click_1);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(14, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(345, 46);
            this.labelTitle.TabIndex = 2;
            this.labelTitle.Text = "Select Your Payment";
            // 
            // panelEwallet
            // 
            this.panelEwallet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(233)))));
            this.panelEwallet.Controls.Add(this.comboBank);
            this.panelEwallet.Controls.Add(this.comboEWallet);
            this.panelEwallet.Controls.Add(this.picQR);
            this.panelEwallet.Controls.Add(this.textBox2);
            this.panelEwallet.Controls.Add(this.textBox1);
            this.panelEwallet.Controls.Add(this.label4);
            this.panelEwallet.Controls.Add(this.label3);
            this.panelEwallet.Controls.Add(this.panel4);
            this.panelEwallet.Location = new System.Drawing.Point(368, 9);
            this.panelEwallet.Name = "panelEwallet";
            this.panelEwallet.Size = new System.Drawing.Size(711, 330);
            this.panelEwallet.TabIndex = 9;
            // 
            // comboBank
            // 
            this.comboBank.FormattingEnabled = true;
            this.comboBank.Location = new System.Drawing.Point(3, 65);
            this.comboBank.Name = "comboBank";
            this.comboBank.Size = new System.Drawing.Size(121, 24);
            this.comboBank.TabIndex = 16;
            // 
            // comboEWallet
            // 
            this.comboEWallet.FormattingEnabled = true;
            this.comboEWallet.Location = new System.Drawing.Point(3, 65);
            this.comboEWallet.Name = "comboEWallet";
            this.comboEWallet.Size = new System.Drawing.Size(121, 24);
            this.comboEWallet.TabIndex = 15;
            // 
            // picQR
            // 
            this.picQR.Image = ((System.Drawing.Image)(resources.GetObject("picQR.Image")));
            this.picQR.Location = new System.Drawing.Point(254, 178);
            this.picQR.Name = "picQR";
            this.picQR.Size = new System.Drawing.Size(174, 149);
            this.picQR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picQR.TabIndex = 14;
            this.picQR.TabStop = false;
            this.picQR.Visible = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(146, 156);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '*';
            this.textBox2.Size = new System.Drawing.Size(397, 22);
            this.textBox2.TabIndex = 13;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(146, 95);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(397, 22);
            this.textBox1.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(146, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(143, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "Username";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Pink;
            this.panel4.Controls.Add(this.labelCategories);
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(708, 59);
            this.panel4.TabIndex = 0;
            // 
            // labelCategories
            // 
            this.labelCategories.AutoSize = true;
            this.labelCategories.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.labelCategories.Location = new System.Drawing.Point(286, 9);
            this.labelCategories.Name = "labelCategories";
            this.labelCategories.Size = new System.Drawing.Size(146, 46);
            this.labelCategories.TabIndex = 3;
            this.labelCategories.Text = "Method";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(165, 176);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(352, 22);
            this.txtPassword.TabIndex = 10;
            // 
            // txtUsername
            // 
            this.txtUsername.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtUsername.Location = new System.Drawing.Point(165, 107);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(35, 22);
            this.txtUsername.TabIndex = 9;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(162, 156);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(67, 16);
            this.labelPassword.TabIndex = 8;
            this.labelPassword.Text = "Password";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(159, 88);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(70, 16);
            this.labelUsername.TabIndex = 7;
            this.labelUsername.Text = "Username";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Pink;
            this.panel3.Controls.Add(this.label1);
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(677, 46);
            this.panel3.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(288, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 28);
            this.label1.TabIndex = 4;
            this.label1.Text = "E-Wallet";
            // 
            // labelChoose
            // 
            this.labelChoose.FormattingEnabled = true;
            this.labelChoose.Items.AddRange(new object[] {
            "Choose",
            "Shoppe E-Wallet",
            "tng E-Wallet",
            "grab E-Walet"});
            this.labelChoose.Location = new System.Drawing.Point(3, 52);
            this.labelChoose.Name = "labelChoose";
            this.labelChoose.Size = new System.Drawing.Size(121, 24);
            this.labelChoose.TabIndex = 0;
            this.labelChoose.Tag = "";
            this.labelChoose.Text = "Choose";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(233)))));
            this.panel2.Controls.Add(this.txtPassword);
            this.panel2.Controls.Add(this.txtUsername);
            this.panel2.Controls.Add(this.labelPassword);
            this.panel2.Controls.Add(this.labelUsername);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.labelChoose);
            this.panel2.Location = new System.Drawing.Point(402, 9);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(677, 295);
            this.panel2.TabIndex = 9;
            // 
            // FormPayment
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(247)))));
            this.ClientSize = new System.Drawing.Size(1148, 461);
            this.Controls.Add(this.panel1);
            this.Name = "FormPayment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form9";
            this.panel1.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.groupBoxPayment.ResumeLayout(false);
            this.groupBoxPayment.PerformLayout();
            this.panelEwallet.ResumeLayout(false);
            this.panelEwallet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.RadioButton radioQR;
        private System.Windows.Forms.RadioButton radioOnlineBanking;
        private System.Windows.Forms.RadioButton radioEWallet;
        private System.Windows.Forms.Button btnProceed;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Panel panelEwallet;
        private System.Windows.Forms.GroupBox groupBoxPayment;
        private System.Windows.Forms.ComboBox labelChoose;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label labelCategories;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.PictureBox picQR;
        private System.Windows.Forms.ComboBox comboEWallet;
        private System.Windows.Forms.ComboBox comboBank;
    }
}