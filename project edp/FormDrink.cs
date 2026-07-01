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
    public partial class FormDrink : Form
    {
        ResourceManager rm = new ResourceManager("project_edp.Strings", typeof(FormStart).Assembly);
        private string currentLang;
        private decimal total = 0;

        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Initial Catalog=VendingSystemDB;Integrated Security=True;Connect Timeout=30";

        public FormDrink(string lang)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            currentLang = lang;
            ApplyLanguage();

            // 💡 FIXED: Uses the unified operational transaction tracking reference
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
            labelTitle.Text = rm.GetString("SelectDrinks");

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

            btn100plus.Text = rm.GetString("AddToCart");
            btnNescafeLatte.Text = rm.GetString("AddToCart");
            btnMilo.Text = rm.GetString("AddToCart");
            btnZussEspresso.Text = rm.GetString("AddToCart");
            btnDutchLady.Text = rm.GetString("AddToCart");
            btnFNLemon.Text = rm.GetString("AddToCart");
            btnSunquickOren.Text = rm.GetString("AddToCart");
            btnSoya.Text = rm.GetString("AddToCart");
            btnVIDABlackcurrant.Text = rm.GetString("AddToCart");
            btnMineralWater.Text = rm.GetString("AddToCart");
            btnHouseBoom.Text = rm.GetString("AddToCart");
            btnFruzetea.Text = rm.GetString("AddToCart");

            btnViewSnacks.Text = rm.GetString("ViewSnacks");
            btnCheckout.Text = rm.GetString("Checkout");
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

        private void button4_Click(object sender, EventArgs e)
        {
            LoadCartFromDatabaseAndShow();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            FormSnack snack = new FormSnack(currentLang);
            snack.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // 💡 FIXED: Safely scrub execution scopes when dropping out to main screens
            CartData.CurrentTransactionID = null;
            CartData.Items.Clear();
            CartData.TotalAmount = 0;
            FormStart start = new FormStart();
            start.Show();
            this.Hide();
        }

        private void button13_Click(object sender, EventArgs e)
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

        private void FormDrink_Load(object sender, EventArgs e)
        {
            total = CartData.TotalAmount;
            lblHeaderTotal.Text = "RM " + total.ToString("F2");
            btnCheckout.Text = $"✓ Checkout (RM {total:F2})";

            btn100plus.Tag = new { ItemID = "IT013", ItemName = "100plus", Price = 2.70 };
            btnNescafeLatte.Tag = new { ItemID = "IT014", ItemName = "Nescafe Latte", Price = 2.30 };
            btnMilo.Tag = new { ItemID = "IT015", ItemName = "Milo", Price = 2.00 };
            btnZussEspresso.Tag = new { ItemID = "IT016", ItemName = "Zuss Espresso", Price = 2.00 };
            btnDutchLady.Tag = new { ItemID = "IT017", ItemName = "Dutch Lady", Price = 3.20 };
            btnFNLemon.Tag = new { ItemID = "IT018", ItemName = "F&N Lemon", Price = 3.00 };
            btnSunquickOren.Tag = new { ItemID = "IT019", ItemName = "Sunquick Oren", Price = 4.50 };
            btnSoya.Tag = new { ItemID = "IT020", ItemName = "Soya", Price = 4.80 };
            btnVIDABlackcurrant.Tag = new { ItemID = "IT021", ItemName = "VIDA Blackcurrant", Price = 2.40 };
            btnMineralWater.Tag = new { ItemID = "IT022", ItemName = "Mineral Water", Price = 4.50 };
            btnHouseBoom.Tag = new { ItemID = "IT023", ItemName = "House Boom", Price = 3.50 };
            btnFruzetea.Tag = new { ItemID = "IT024", ItemName = "Fruzetea", Price = 4.50 };

            btn100plus.Click += AddToCart_Click;
            btnNescafeLatte.Click += AddToCart_Click;
            btnMilo.Click += AddToCart_Click;
            btnZussEspresso.Click += AddToCart_Click;
            btnDutchLady.Click += AddToCart_Click;
            btnFNLemon.Click += AddToCart_Click;
            btnSunquickOren.Click += AddToCart_Click;
            btnSoya.Click += AddToCart_Click;
            btnVIDABlackcurrant.Click += AddToCart_Click;
            btnMineralWater.Click += AddToCart_Click;
            btnHouseBoom.Click += AddToCart_Click;
            btnFruzetea.Click += AddToCart_Click;
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
    }
}