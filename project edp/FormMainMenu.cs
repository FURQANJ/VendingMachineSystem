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
    public partial class FormMainMenu : Form
    {
        ResourceManager rm = new ResourceManager("project_edp.Strings", typeof(FormStart).Assembly);
        private string currentLang;

        public FormMainMenu(string lang)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            currentLang = lang;
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            labelNavigation.Text = rm.GetString("Navigation");
            labelVending.Text = rm.GetString("VENDING");
            labelServices.Text = rm.GetString("SERVICES");

            btnMainMenu.Text = rm.GetString("MainMenu");
            btnDrinks.Text = rm.GetString("Drinks");
            btnSnacks.Text = rm.GetString("Snacks");
            btnView.Text = rm.GetString("ViewCart");
            btnRoadTax.Text = rm.GetString("RoadTax");
            btnMobileTopUp.Text = rm.GetString("MobileTopUp");
            btnExit.Text = rm.GetString("ExitCancel");

            labelChooseService.Text = rm.GetString("ChooseService");
            labelSelectOption.Text = rm.GetString("SelectOption");
            labelSnacksDrinks.Text = rm.GetString("SnacksDrinks");
            labelRoadTaxRenewal.Text = rm.GetString("RoadTaxRenewal");
            labelMobileTopUpService.Text = rm.GetString("MobileTopUp");

            labelSnacksDesc.Text = rm.GetString("SnacksDesc");
            labelRoadTaxDesc.Text = rm.GetString("RoadTaxDesc");
            labelTopUpDesc.Text = rm.GetString("TopUpDesc");
        }

        private void btnMobileTopUp_Click(object sender, EventArgs e)
        {
            FormTopUp topUp = new FormTopUp(currentLang);
            topUp.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            FormStart start = new FormStart();
            start.Show();
            this.Hide();
        }

        private void btnRoadTax_Click(object sender, EventArgs e)
        {
            FormRoadTax tax = new FormRoadTax(currentLang);
            tax.Show();
            this.Hide();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            FormView view = new FormView(currentLang, CartData.Items, CartData.TotalAmount);
            view.Show();
            this.Hide();
        }

        private void btnSnacks_Click(object sender, EventArgs e)
        {
            FormSnack snacks = new FormSnack(currentLang);
            snacks.Show();
            this.Hide();
        }

        private void btnDrinks_Click(object sender, EventArgs e)
        {
            FormDrink drink = new FormDrink(currentLang);
            drink.Show();
            this.Hide();
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            FormMainMenu main = new FormMainMenu(currentLang);
            main.Show();
            this.Hide();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            FormSnack snack = new FormSnack(currentLang);
            snack.Show();
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            FormRoadTax tax = new FormRoadTax(currentLang);
            tax.Show();
            this.Hide();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            FormTopUp topUp = new FormTopUp(currentLang);
            topUp.Show();
            this.Hide();
        }
    }
}