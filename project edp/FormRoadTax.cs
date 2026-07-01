using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
    public partial class FormRoadTax : Form
    {
        ResourceManager rm = new ResourceManager("project_edp.Strings", typeof(FormStart).Assembly);
        private string currentLang;

        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Initial Catalog=VendingSystemDB;Integrated Security=True;Connect Timeout=30";

        public FormRoadTax(string lang)
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
            labelTitle.Text = rm.GetString("SelectProvider");

            labelVehicle.Text = rm.GetString("VehicleRegNumber");
            labelIC.Text = rm.GetString("ICNumber");
            labelOwner.Text = rm.GetString("OwnerName");
            labelTopUp.Text = rm.GetString("EnterAmount");

            btnProceed.Text = rm.GetString("Proceed");
        }

        private void label5_Click(object sender, EventArgs e) { }

        private void btnMain_Click(object sender, EventArgs e)
        {
            FormMainMenu main = new FormMainMenu(currentLang);
            main.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormSnack snack = new FormSnack(currentLang);
            snack.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FormView view = new FormView(currentLang, new List<string>(), 0);
            view.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormRoadTax tax = new FormRoadTax(currentLang);
            tax.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FormTopUp topUp = new FormTopUp(currentLang);
            topUp.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FormStart start = new FormStart();
            start.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormDrink drink = new FormDrink(currentLang);
            drink.Show();
            this.Hide();
        }

        private void KiraRoadTax()
        {
            double serviceFee = 1.00;
            double baseRoadTax = 0.00;

            if (double.TryParse(cmbCC.Text, out double cc))
            {
                if (cc <= 1000) { baseRoadTax = 20.00; }
                else if (cc <= 1600) { baseRoadTax = 90.00; }
                else if (cc <= 2000) { baseRoadTax = 380.00; }
                else { baseRoadTax = 550.00; }

                if (rb6Month.Checked)
                {
                    baseRoadTax = baseRoadTax * 0.5;
                }

                double total = baseRoadTax + serviceFee;

                txtTotal.Text = "--- ROAD TAX SUMMARY ---\n" +
                                "Vehicle No: " + txtVehicleNo.Text + "\n" +
                                "Engine CC: " + cc + " CC\n" +
                                "Base Price: RM " + baseRoadTax.ToString("F2") + "\n" +
                                "Service Fee: RM " + serviceFee.ToString("F2") + "\n" +
                                "------------------------\n" +
                                "Total Payable: RM " + total.ToString("F2");
            }
            else
            {
                txtTotal.Text = "RM 0.00";
            }
        }

        private void txtCC_TextChanged(object sender, EventArgs e)
        {
            KiraRoadTax();
        }

        private void rb6Month_CheckedChanged(object sender, EventArgs e)
        {
            KiraRoadTax();
        }

        private void rb12Month_CheckedChanged(object sender, EventArgs e)
        {
            KiraRoadTax();
        }

        private void btnProceed_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbCC.Text))
            {
                MessageBox.Show("Sila masukkan enjin CC terlebih dahulu.", "Makluman", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal serviceFee = 1.00m;
            decimal baseRoadTax = 0.00m;

            if (decimal.TryParse(cmbCC.Text, out decimal cc))
            {
                if (cc <= 1000) { baseRoadTax = 20.00m; }
                else if (cc <= 1600) { baseRoadTax = 90.00m; }
                else if (cc <= 2000) { baseRoadTax = 380.00m; }
                else { baseRoadTax = 550.00m; }

                if (rb6Month.Checked)
                {
                    baseRoadTax = baseRoadTax * 0.5m;
                }

                decimal totalAmount = baseRoadTax + serviceFee;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO [Transaction] (TransactionID, TransactionDate, TotalAmount, Status) VALUES (@id, @date, @amount, @status)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", GenerateTransactionID());
                        cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@amount", totalAmount);
                        cmd.Parameters.AddWithValue("@status", "Paid");
                        cmd.ExecuteNonQuery();
                    }
                }

                FormEnd end = new FormEnd(currentLang);
                end.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Sila pastikan nilai CC kenderaan adalah sah.", "Makluman", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string GenerateTransactionID()
        {
            string newID = "TXN001";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1 TransactionID FROM [Transaction] ORDER BY TransactionID DESC";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        string lastID = result.ToString();
                        int num = int.Parse(lastID.Substring(3)) + 1;
                        newID = "TXN" + num.ToString("D3");
                    }
                }
            }
            return newID;
        }

        private void FormRoadTax_Load(object sender, EventArgs e)
        {
            cmbCC.Items.AddRange(new object[] { "800", "1000", "1300", "1600", "2000", "2500" });
            cmbCC.SelectedIndex = 0;
        }
    }
}