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
    public partial class FormView : Form
    {
        private string currentLang;
        private decimal total;

        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Initial Catalog=VendingSystemDB;Integrated Security=True;Connect Timeout=30";

        ResourceManager rm = new ResourceManager("project_edp.Strings", typeof(FormStart).Assembly);

        public FormView(string lang, List<string> cartDetails, decimal total)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(lang))
            {
                lang = "en";
            }

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            currentLang = lang;
            this.total = total;

            ApplyLanguage();

            textBoxCart.Clear();
            foreach (string item in CartData.Items)
            {
                textBoxCart.AppendText(item + Environment.NewLine);
            }
            textBoxCart.AppendText("-----------------------------" + Environment.NewLine);
            textBoxCart.AppendText("TOTAL: RM " + CartData.TotalAmount.ToString("F2"));
            UpdateReceiptText();
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

            labelTitle.Text = rm.GetString("ShoppingCart");
            labelPromoTitle.Text = rm.GetString("PromoTitle");
            labelEnterCode.Text = rm.GetString("EnterCode");

            btnAdd.Text = rm.GetString("Add");
            lbltotal.Text = rm.GetString("GoToPayment");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string promoCode = txtPromo.Text.Trim();

            if (string.IsNullOrEmpty(promoCode))
            {
                MessageBox.Show("Please enter a promo code first.");
                return;
            }

            string query = "SELECT Discount FROM Promotion WHERE PromotionName=@name AND Status='Active' AND @today BETWEEN StartDate AND EndDate";

            // 💡 FIXED: Swapped legacy hardcoded path out for central connectionString
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", promoCode);
                    cmd.Parameters.AddWithValue("@today", DateTime.Today);

                    try
                    {
                        connection.Open();
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            decimal discountPercent = Convert.ToDecimal(result);
                            decimal discountAmount = CartData.TotalAmount * (discountPercent / 100);
                            CartData.TotalAmount -= discountAmount;

                            UpdateReceiptText(promoCode, discountAmount);

                            txtPromo.Clear();
                            MessageBox.Show("Promo code applied successfully!");
                            btnAdd.Enabled = false;
                        }
                        else
                        {
                            MessageBox.Show("Invalid, expired, or inactive promo code.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

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

        private void btnExit_Click(object sender, EventArgs e)
        {
            FormStart start = new FormStart();
            start.Show();
            this.Hide();
        }

        private void FormView_Load(object sender, EventArgs e) { }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            CartData.TotalAmount = total;
            FormPayment paymentForm = new FormPayment(currentLang, CartData.TotalAmount);
            paymentForm.Show();
            this.Hide();
        }

        private void UpdateReceiptText(string promoCode = "", decimal discountAmount = 0)
        {
            textBoxCart.Clear();
            textBoxCart.AppendText("=== YOUR SHOPPING CART ===" + Environment.NewLine);
            textBoxCart.AppendText("--------------------------------------" + Environment.NewLine);

            foreach (var item in CartData.Items)
            {
                textBoxCart.AppendText(item + Environment.NewLine);
            }

            textBoxCart.AppendText("--------------------------------------" + Environment.NewLine);

            if (!string.IsNullOrEmpty(promoCode))
            {
                decimal originalTotal = CartData.TotalAmount + discountAmount;
                textBoxCart.AppendText($"Subtotal: RM{originalTotal:F2}" + Environment.NewLine);
                textBoxCart.AppendText($"Promo Applied: {promoCode} (-RM{discountAmount:F2})" + Environment.NewLine);
                textBoxCart.AppendText("--------------------------------------" + Environment.NewLine);
            }

            textBoxCart.AppendText($"TOTAL TO PAY: RM{CartData.TotalAmount:F2}" + Environment.NewLine);
        }

        private void txtCartDetails_TextChanged(object sender, EventArgs e) { }
    }
}