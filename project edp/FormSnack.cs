using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
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
    public partial class FormSnack : Form
    {
        ResourceManager rm = new ResourceManager("project_edp.Strings", typeof(FormStart).Assembly);
        private string currentLang;
        public decimal total = 0;

        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Initial Catalog=VendingSystemDB;Integrated Security=True;Connect Timeout=30";

        public FormSnack(string lang)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            currentLang = lang;
            ApplyLanguage();

            // 💡 FIXED: Uses a centralized transaction ID so it persists across screens
            if (string.IsNullOrEmpty(CartData.CurrentTransactionID))
            {
                CartData.CurrentTransactionID = Guid.NewGuid().ToString();
            }
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
            labelTitle.Text = rm.GetString("SelectSnacks");

            labelStock1.Text = rm.GetString("InStock");
            labelStock2.Text = rm.GetString("InStock");
            labelStock3.Text = rm.GetString("InStock");
            labelStock4.Text = rm.GetString("InStock");
            labelStock5.Text = rm.GetString("InStock");
            labelStock6.Text = rm.GetString("InStock");
            labelStock7.Text = rm.GetString("InStock");
            labelStock8.Text = rm.GetString("InStock");
            labelStock9.Text = rm.GetString("InStock");
            labelStock10.Text = rm.GetString("InStock");
            labelStock11.Text = rm.GetString("InStock");
            labelStock12.Text = rm.GetString("InStock");

            btnPopoMuruku.Text = rm.GetString("AddToCart");
            btnSuperRing.Text = rm.GetString("AddToCart");
            btnChipsMore.Text = rm.GetString("AddToCart");
            btnOreo.Text = rm.GetString("AddToCart");

            btnViewDrinks.Text = rm.GetString("ViewDrinks");
            btnCheckout.Text = rm.GetString("Checkout");
        }

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
            LoadCartFromDatabaseAndShow();
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
            // 💡 FIXED: Resets global transaction tracker upon exit cancel actions
            CartData.CurrentTransactionID = null;
            CartData.Items.Clear();
            CartData.TotalAmount = 0;
            FormStart start = new FormStart();
            start.Show();
            this.Hide();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            FormDrink drink = new FormDrink(currentLang);
            drink.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormDrink drink = new FormDrink(currentLang);
            drink.Show();
            this.Hide();
        }

        private void FormSnack_Load(object sender, EventArgs e)
        {
            total = CartData.TotalAmount;
            lblHeaderTotal.Text = "RM " + total.ToString("F2");
            btnCheckout.Text = $"✓ Checkout (RM {total:F2})";

            btnPopoMuruku.Tag = new { ItemID = "IT001", ItemName = "Popo Muruku", Price = 2.70 };
            btnSuperRing.Tag = new { ItemID = "IT002", ItemName = "Super Ring", Price = 2.30 };
            btnChipsMore.Tag = new { ItemID = "IT003", ItemName = "Chips More", Price = 2.00 };
            btnOreo.Tag = new { ItemID = "IT004", ItemName = "Oreo", Price = 2.00 };
            btnChocolateMuffin.Tag = new { ItemID = "IT005", ItemName = "Chocolate Muffin", Price = 3.20 };
            btnIkanBilisBun.Tag = new { ItemID = "IT006", ItemName = "Ikan Bilis Bun", Price = 3.00 };
            btnKitkatChuncky.Tag = new { ItemID = "IT007", ItemName = "Kitkat Chuncky", Price = 4.50 };
            btnKinderBueno.Tag = new { ItemID = "IT008", ItemName = "Kinder Bueno", Price = 4.80 };
            btnHoneyCashewNuts.Tag = new { ItemID = "IT009", ItemName = "Honey Cashew Nuts", Price = 2.40 };
            btnChocolateMuffinLarge.Tag = new { ItemID = "IT010", ItemName = "Chocolate Muffin (Large)", Price = 4.50 };
            btnMaggiHotCup.Tag = new { ItemID = "IT011", ItemName = "Maggi Hot Cup", Price = 3.50 };
            btnMieSedap.Tag = new { ItemID = "IT012", ItemName = "Mie Sedap", Price = 4.50 };

            btnPopoMuruku.Click += AddToCart_Click;
            btnSuperRing.Click += AddToCart_Click;
            btnChipsMore.Click += AddToCart_Click;
            btnOreo.Click += AddToCart_Click;
            btnChocolateMuffin.Click += AddToCart_Click;
            btnIkanBilisBun.Click += AddToCart_Click;
            btnKitkatChuncky.Click += AddToCart_Click;
            btnKinderBueno.Click += AddToCart_Click;
            btnHoneyCashewNuts.Click += AddToCart_Click;
            btnChocolateMuffinLarge.Click += AddToCart_Click;
            btnMaggiHotCup.Click += AddToCart_Click;
            btnMieSedap.Click += AddToCart_Click;
        }

        private void AddToCart_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            dynamic item = btn.Tag;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string checkTransaction = "SELECT COUNT(*) FROM [Transaction] WHERE TransactionID = @tid";
                SqlCommand cmdCheck = new SqlCommand(checkTransaction, conn);
                cmdCheck.Parameters.AddWithValue("@tid", CartData.CurrentTransactionID);
                int exists = (int)cmdCheck.ExecuteScalar();

                if (exists == 0)
                {
                    string insertTransaction = "INSERT INTO [Transaction] (TransactionID, TransactionDate, TotalAmount, Status) VALUES (@tid, @date, 0, @status)";
                    SqlCommand cmdTrans = new SqlCommand(insertTransaction, conn);
                    cmdTrans.Parameters.AddWithValue("@tid", CartData.CurrentTransactionID);
                    cmdTrans.Parameters.AddWithValue("@date", DateTime.Now);
                    cmdTrans.Parameters.AddWithValue("@status", "Pending");
                    cmdTrans.ExecuteNonQuery();
                }

                string query = "INSERT INTO TransactionDetail (TransactionID, ItemID, Quantity, Price) VALUES (@transactionID, @itemID, @quantity, @price)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@transactionID", CartData.CurrentTransactionID);
                cmd.Parameters.AddWithValue("@itemID", item.ItemID);
                cmd.Parameters.AddWithValue("@quantity", 1);
                cmd.Parameters.AddWithValue("@price", item.Price);
                cmd.ExecuteNonQuery();

                string updateTotal = "UPDATE [Transaction] SET TotalAmount = TotalAmount + @price WHERE TransactionID = @tid";
                SqlCommand cmdUpdate = new SqlCommand(updateTotal, conn);
                cmdUpdate.Parameters.AddWithValue("@price", item.Price);
                cmdUpdate.Parameters.AddWithValue("@tid", CartData.CurrentTransactionID);
                cmdUpdate.ExecuteNonQuery();

                string updateStock = "UPDATE Inventory SET Stock = Stock - 1 WHERE ItemID = @itemID";
                SqlCommand cmdStock = new SqlCommand(updateStock, conn);
                cmdStock.Parameters.AddWithValue("@itemID", item.ItemID);
                cmdStock.ExecuteNonQuery();

                string checkStock = "SELECT Stock FROM Inventory WHERE ItemID = @itemID";
                SqlCommand cmdCheckStock = new SqlCommand(checkStock, conn);
                cmdCheckStock.Parameters.AddWithValue("@itemID", item.ItemID);
                int currentStock = Convert.ToInt32(cmdCheckStock.ExecuteScalar());

                if (currentStock <= 0)
                {
                    string labelName = "lbl_" + item.ItemID;
                    Control[] foundLabels = this.Controls.Find(labelName, true);

                    if (foundLabels.Length > 0)
                    {
                        Label lblStatus = foundLabels[0] as Label;
                        lblStatus.Text = "Out of Stock";
                        lblStatus.BackColor = Color.Red;
                        lblStatus.ForeColor = Color.White;
                        btn.Enabled = false;
                    }
                }

                total += (decimal)item.Price;
                CartData.TotalAmount = total;
                lblHeaderTotal.Text = "RM " + total.ToString("F2");
                btnCheckout.Text = $"✓ Checkout (RM {total:F2})";

                MessageBox.Show($"{item.ItemName} added to cart!");
            }
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            LoadCartFromDatabaseAndShow();
        }

        private void LoadCartFromDatabaseAndShow()
        {
            CartData.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT i.ItemName, td.Quantity, i.Price
                    FROM TransactionDetail td
                    INNER JOIN Inventory i ON td.ItemID = i.ItemID
                    WHERE td.TransactionID = @tid";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@tid", CartData.CurrentTransactionID);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string name = reader["ItemName"].ToString();
                    int qty = Convert.ToInt32(reader["Quantity"]);
                    decimal price = Convert.ToDecimal(reader["Price"]);
                    decimal subtotal = qty * price;

                    CartData.Items.Add($"{name} x{qty} - RM{subtotal:F2}");
                }

                reader.Close();

                FormView view = new FormView(currentLang, CartData.Items, CartData.TotalAmount);
                view.Show();
                this.Hide();
            }
        }
    }
}