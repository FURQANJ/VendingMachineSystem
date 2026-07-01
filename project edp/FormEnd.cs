using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project_edp
{
    public partial class FormEnd : Form
    {
        private string currentLang;
        ResourceManager rm = new ResourceManager("project_edp.Strings", typeof(FormStart).Assembly);
      
        public FormEnd(String lang)
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(lang))
                lang = "en";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            currentLang = lang;
          
            ApplyLanguage();
        }
        private void ApplyLanguage()
        {
           
            lblProcessing.Text = rm.GetString("Process");
            lblPercentage.Text = rm.GetString("wait");
            lblVerifying.Text = rm.GetString("verify");
            labelProgress.Text = rm.GetString("Progres");
        }


    

        private void Form10_Load(object sender, EventArgs e)
        {
            timer1.Interval = 50; // kelajuan loading (ms)
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Increment(2); // naik 2% setiap tick
            lblPercentage.Text = progressBar1.Value + "%";

            if (progressBar1.Value >= 100)
            {
                timer1.Stop();
           
                FormStart home = new FormStart(); // form utama
                home.Show();
                this.Hide(); // sembunyi form loading
                lblProcessing.Text = rm.GetString("ThankYou");
                lblVerifying.Text = rm.GetString("VerificationComplete");
                lblPercentage.Text = "100%"; // Kekalkan 100% supaya nampak padat & complete
            }
        }

       
    }
}
