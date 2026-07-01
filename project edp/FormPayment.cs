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
    public partial class FormPayment : Form
    {
        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Initial Catalog=VendingSystemDB;Integrated Security=True;Connect Timeout=30";

        private string currentLang;
        ResourceManager rm = new ResourceManager("project_edp.Strings", typeof(FormStart).Assembly);
        private decimal totalAmount;

        public FormPayment(string lang, decimal total)
        {
            InitializeComponent();

            currentLang = lang;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            ApplyLanguage();

            comboEWallet.Visible = false;
            comboBank.Visible = false;
            txtUsername.Visible = false;
            txtPassword.Visible = false;
            picQR.Visible = false;

            comboEWallet.Items.AddRange(new string[] { "Touch 'n Go", "Boost", "GrabPay" });
            comboBank.Items.AddRange(new string[] { "RHB", "Maybank", "Bank Islam", "CIMB" });
            totalAmount = total;
        }

        private void ApplyLanguage()
        {
            labelTitle.Text = rm.GetString("SelectPayment");
            groupBoxPayment.Text = rm.GetString("ChoosePaymentMethod");

            labelChoose.Text = rm.GetString("Choose");
            labelUsername.Text = rm.GetString("Username");
            labelPassword.Text = rm.GetString("Password");
            labelCategories.Text = rm.GetString("Method");

            btnBack.Text = rm.GetString("Back");
            btnProceed.Text = rm.GetString("Proceed");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FormMainMenu mainForm = new FormMainMenu(currentLang);
            mainForm.Show();
            this.Hide();
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            // 💡 FIXED: Replaced hardcoded path literal with the clean centralized connection field
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string method = "";

                    if (radioEWallet.Checked)
                    {
                        method = comboEWallet.SelectedItem != null ? comboEWallet.SelectedItem.ToString() : "Paid (E-Wallet)";
                    }
                    else if (radioOnlineBanking.Checked)
                    {
                        method = comboBank.SelectedItem != null ? comboBank.SelectedItem.ToString() : "Paid (Online Banking)";
                    }
                    else if (radioQR.Checked)
                    {
                        method = "Paid (QR Code)";
                    }
                    else
                    {
                        method = "Unknown";
                    }

                    string transactionID = GenerateTransactionID(connection);

                    string query = "INSERT INTO [Transaction] (TransactionID, TransactionDate, TotalAmount, Status) " +
                                   "VALUES (@id, @date, @amount, @status)";

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", transactionID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@amount", totalAmount);
                        cmd.Parameters.AddWithValue("@status", method ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }

                    FormEnd end = new FormEnd(currentLang);
                    end.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message + "\n\nStack Trace: " + ex.StackTrace);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private string GenerateTransactionID(SqlConnection connection)
        {
            string newID = "TXN001";
            string query = "SELECT TOP 1 TransactionID FROM [Transaction] ORDER BY TransactionID DESC";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    string lastID = result.ToString().Trim();

                    if (lastID.Length > 3)
                    {
                        string numericPart = lastID.Substring(3);

                        if (int.TryParse(numericPart, out int number))
                        {
                            number++;
                            newID = "TXN" + number.ToString("D3");
                        }
                    }
                }
            }
            return newID;
        }

        private void radioEwallet_CheckedChanged(object sender, EventArgs e)
        {
            if (radioEWallet.Checked)
            {
                comboEWallet.Visible = true;
                comboBank.Visible = false;
                txtUsername.Visible = true;
                txtPassword.Visible = true;
                picQR.Visible = false;
                labelCategories.Visible = true;
                labelCategories.Text = "E-Wallet";
            }
        }

        private void radioOnline_CheckedChanged(object sender, EventArgs e)
        {
            if (radioOnlineBanking.Checked)
            {
                comboBank.Visible = true;
                comboEWallet.Visible = false;
                txtUsername.Visible = true;
                txtPassword.Visible = true;
                picQR.Visible = false;
                labelCategories.Text = "Online Banking";
            }
        }

        private void radioQR_CheckedChanged(object sender, EventArgs e)
        {
            if (radioQR.Checked)
            {
                comboEWallet.Visible = false;
                comboBank.Visible = false;
                txtUsername.Visible = false;
                txtPassword.Visible = false;
                picQR.Visible = true;
                labelCategories.Text = "QR Code";
            }
        }

        private void btnBack_Click_1(object sender, EventArgs e)
        {
            FormView view = new FormView(currentLang, new List<string>(), 0);
            view.Show();
            this.Hide();
        }
    }
}