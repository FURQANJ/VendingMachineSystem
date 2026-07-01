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
    public partial class FormTopUp : Form
    {
        ResourceManager rm = new ResourceManager("project_edp.Strings", typeof(FormStart).Assembly);
        private string currentLang;

        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Initial Catalog=VendingSystemDB;Integrated Security=True;Connect Timeout=30";

        public FormTopUp(string lang)
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
            labelReminder.Text = rm.GetString("ChooseProviderInstruction");

            labelTap1.Text = rm.GetString("TapToSelect");
            labelTap2.Text = rm.GetString("TapToSelect");
            labelTap3.Text = rm.GetString("TapToSelect");
            labelTap4.Text = rm.GetString("TapToSelect");

            labelTopUp.Text = rm.GetString("EnterTopUpAmount");
            labelReminder1.Text = rm.GetString("SelectProviderReminder");
            btnProceed.Text = rm.GetString("Proceed");
        }

        private void label7_Click(object sender, EventArgs e) { }

        private void btnMain_Click(object sender, EventArgs e)
        {
            FormMainMenu main = new FormMainMenu(currentLang);
            main.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormDrink drink = new FormDrink(currentLang);
            drink.Show();
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

        private void button6_Click(object sender, EventArgs e)
        {
            FormTopUp topUp = new FormTopUp(currentLang);
            topUp.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormRoadTax tax = new FormRoadTax(currentLang);
            tax.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FormStart start = new FormStart();
            start.Show();
            this.Hide();
        }

        private string selectedProvider = "";
        private void ProviderLabel_Click(object sender, EventArgs e)
        {
            Label clickedLabel = sender as Label;
            if (clickedLabel != null)
            {
                selectedProvider = clickedLabel.Text;
                labelReminder.Text = $"You selected {selectedProvider}. Please enter top-up amount.";
                labelReminder.Visible = true;
                labelReminder1.Visible = false;

                lblRM.Visible = true;
                cmbAmount.Visible = true;
                lblTotal.Visible = true;
                txtTotal.Visible = true;
                btnProceed.Visible = true;
                labelTopUp.Visible = true;
                labelTopUp.Text = "Enter Top - Up Amount";
            }
        }

        private void btnProceed_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedProvider))
            {
                MessageBox.Show("Sila pilih penyedia perkhidmatan terlebih dahulu.", "Makluman", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(cmbAmount.Text, out decimal topUpAmount) || topUpAmount <= 0)
            {
                MessageBox.Show("Sila masukkan jumlah top-up yang sah.", "Makluman", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string transactionID = GenerateTransactionID();
            string status = "Paid";
            decimal totalAmount = topUpAmount;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO [Transaction] (TransactionID, TransactionDate, TotalAmount, Status) VALUES (@id, @date, @amount, @status)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", transactionID);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@amount", totalAmount);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.ExecuteNonQuery();
                }
            }

            FormEnd end = new FormEnd(currentLang);
            end.Show();
            this.Hide();
        }
        private string GenerateTransactionID()
        {
            string newID = "TXN001";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1 TransactionID FROM [Transaction] ORDER BY TransactionDate DESC";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        string lastID = result.ToString();
                        int num = 0;
                        if (lastID.StartsWith("TXN") && int.TryParse(lastID.Substring(3), out int parsed))
                            num = parsed + 1;
                        else
                            num = 1; // fallback when DB value is unexpected
                        newID = "TXN" + num.ToString("D3");
                    }
                }
            }
            return newID;
        }

        private void FormTopUp_Load(object sender, EventArgs e)
        {
            CelcomDigi.Tag = "Celcom/Digi";
            Maxis.Tag = "Maxis/Hotlink";
            UMobile.Tag = "U Mobile";
            Yes4G.Tag = "Yes 4G";

            CelcomDigi.Click += ProviderLabel_Click;
            Maxis.Click += ProviderLabel_Click;
            UMobile.Click += ProviderLabel_Click;
            Yes4G.Click += ProviderLabel_Click;

            lblRM.Visible = false;
            cmbAmount.Visible = false;
            lblTotal.Visible = false;
            txtTotal.Visible = false;
            labelTopUp.Visible = false;
            btnProceed.Visible = false;

            labelReminder.Visible = true;
            labelReminder1.Visible = true;
            labelReminder1.Text = "Please select a provider to continue";

            panelTopUp.Visible = true;
            labelTap1.Text = "Tap to select";
            labelTap2.Text = "Tap to select";
            labelTap3.Text = "Tap to select";
            labelTap4.Text = "Tap to select";
            cmbAmount.Items.AddRange(new object[] { "5","10","30", "60", "90", "120" });
            cmbAmount.SelectedIndex = 0;
        }

        private void txtTotal_TextChanged(object sender, EventArgs e) { }

        private void cmbAmount_SelectedIndexChanged(object sender, EventArgs e)
        {
            double serviceFee = 1.00;
            double selectedAmount = double.Parse(cmbAmount.SelectedItem.ToString());
            double total = selectedAmount + serviceFee;

            txtTotal.Text = $"Service Fee: RM {serviceFee:F2}\n" +
                            $"Amount: RM {selectedAmount:F2}\n" +
                            $"-----------------------------\n" +
                            $"Total: RM {total:F2}";
        }
    }
}