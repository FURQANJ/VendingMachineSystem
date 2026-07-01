using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace project_edp
{
    public partial class FormStart : Form
    {
        ResourceManager rm = new ResourceManager("project_edp.Strings", typeof(FormStart).Assembly);

        public FormStart()
        {
            InitializeComponent();

        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            // Tukar lblTime kepada nama komponen label jam anda jika berbeza
            // Kod ini mengambil masa semasa komputer dan memapar dalam format jam:minit:saat tt
            label4.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void btnEnglish_Click(object sender, EventArgs e)
        {
            SetLanguage("en");
        }

        private void btnMalay_Click(object sender, EventArgs e)
        {
            SetLanguage("ms");
        }
        private void SetLanguage(string lang)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            // Pastikan nama key sama dengan yang dalam .resx
            btnStart.Text = rm.GetString("Tapmassage");
            btnEnglish.Text = "English";
            btnMalay.Text = "Bahasa Melayu";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string currentLang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            // Buka Form2
            FormMainMenu form2 = new FormMainMenu(currentLang);
            form2.Show();

            // Sembunyikan Form1 (supaya tak nampak dua form serentak)
            this.Hide();
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            FormLoginAdmin LoginAdmin = new FormLoginAdmin();
            LoginAdmin.Show();
            this.Hide();
        }
    }
}
