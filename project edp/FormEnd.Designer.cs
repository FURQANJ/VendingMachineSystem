namespace project_edp
{
    partial class FormEnd
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.labelProgress = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblVerifying = new System.Windows.Forms.Label();
            this.lblPercentage = new System.Windows.Forms.Label();
            this.lblProcessing = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Pink;
            this.panel1.Controls.Add(this.panel8);
            this.panel1.Controls.Add(this.lblProcessing);
            this.panel1.Location = new System.Drawing.Point(10, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1126, 437);
            this.panel1.TabIndex = 3;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(247)))));
            this.panel8.Controls.Add(this.labelProgress);
            this.panel8.Controls.Add(this.lblPercent);
            this.panel8.Controls.Add(this.progressBar1);
            this.panel8.Controls.Add(this.lblVerifying);
            this.panel8.Controls.Add(this.lblPercentage);
            this.panel8.Location = new System.Drawing.Point(25, 60);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(1078, 344);
            this.panel8.TabIndex = 6;
            // 
            // labelProgress
            // 
            this.labelProgress.AutoSize = true;
            this.labelProgress.Location = new System.Drawing.Point(264, 151);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(62, 16);
            this.labelProgress.TabIndex = 10;
            this.labelProgress.Text = "Progress";
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.Location = new System.Drawing.Point(777, 151);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(40, 16);
            this.lblPercent.TabIndex = 9;
            this.lblPercent.Text = "100%";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(267, 124);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(550, 23);
            this.progressBar1.TabIndex = 8;
            // 
            // lblVerifying
            // 
            this.lblVerifying.AutoSize = true;
            this.lblVerifying.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblVerifying.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.lblVerifying.Location = new System.Drawing.Point(490, 70);
            this.lblVerifying.Name = "lblVerifying";
            this.lblVerifying.Size = new System.Drawing.Size(88, 23);
            this.lblVerifying.TabIndex = 7;
            this.lblVerifying.Text = "Verifiyng...";
            // 
            // lblPercentage
            // 
            this.lblPercentage.AutoSize = true;
            this.lblPercentage.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblPercentage.Location = new System.Drawing.Point(444, 24);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(197, 46);
            this.lblPercentage.TabIndex = 2;
            this.lblPercentage.Text = "Please wait";
            // 
            // lblProcessing
            // 
            this.lblProcessing.AutoSize = true;
            this.lblProcessing.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblProcessing.Location = new System.Drawing.Point(447, 11);
            this.lblProcessing.Name = "lblProcessing";
            this.lblProcessing.Size = new System.Drawing.Size(219, 46);
            this.lblProcessing.TabIndex = 2;
            this.lblProcessing.Text = "Processing...";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormEnd
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(247)))));
            this.ClientSize = new System.Drawing.Size(1148, 461);
            this.Controls.Add(this.panel1);
            this.Name = "FormEnd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form10";
            this.Load += new System.EventHandler(this.Form10_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblProcessing;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label lblPercentage;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblVerifying;
        private System.Windows.Forms.Timer timer1;
    }
}