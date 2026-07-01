using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace project_edp
{
    public partial class FormAdmin : Form
    {
        // Connection String & Timer
        private readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Initial Catalog=VendingSystemDB;Integrated Security=True;Connect Timeout=30";
        private SqlConnection connection;
        private System.Windows.Forms.Timer refreshTimer;

        // Untuk PDF/Print/CSV Export
        private List<string> printLines = new List<string>();
        private int printLineIndex = 0;
        private string currentReportTitle = "";

        // UI Components Dinamik (Ditambah melalui kod)
        private TextBox txtSearchProduct;
        private ComboBox cmbCategoryFilter;
        private Label lblProductStats;
        private TextBox txtSearchMaint;
        private ComboBox cmbMaintStatus;
        private Label lblMaintStats;
        private TextBox txtSearchUser;
        private ComboBox cmbRoleFilter;
        private Label lblUserStats;
        private Label lblPromoStats;
        private ComboBox cmbReportType;
        private DateTimePicker dtpReportFrom;
        private DateTimePicker dtpReportTo;
        private DataGridView dgvReport;
        private Label lblReportSummary;
        private Panel pnlReportHeader;

        public FormAdmin()
        {
            InitializeComponent();
            connection = new SqlConnection(connectionString);

            // Pemasa automatik (30 saat) - Mengemas kini Dashboard & Ringkasan Pembayaran
            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 30000;
            refreshTimer.Tick += (s, e) => {
                LoadDashboard();
                RefreshPaymentSummary();
            };
            refreshTimer.Start();
        }

        // ============================================================
        //  FORM LOAD & REFRESH LOGIC
        // ============================================================
        private void Form7_Load(object sender, EventArgs e)
        {
            try
            {
                this.maintenanceTableAdapter.Fill(this.dataSet1.Maintenance);
                this.transactionPromotionTableAdapter.Fill(this.dataSet1.TransactionPromotion);
                this.promotionTableAdapter.Fill(this.dataSet1.Promotion);
                this.transactionDetailTableAdapter.Fill(this.dataSet1.TransactionDetail);
                this.userTableAdapter.Fill(this.dataSet1.User);
                this.transactionTableAdapter.Fill(this.dataSet1.Transaction);
                this.inventoryTableAdapter.Fill(this.dataSet1.Inventory);
            }
            catch { }

            // Jalankan semua tetapan komponen UI & Data
            LoadDashboard();
            RefreshPaymentSummary();
            SetupProductSearch();
            SetupPaymentSummary();
            SetupMaintenanceSearch();
            SetupUserSearch();
            SetupPromotionAnalytics();
            SetupReportTab();
        }

        private void UpdateLastRefreshLabel()
        {
            Control lbl = FindControlRecursive(this, "lblLastRefresh");
            if (lbl != null) lbl.Text = "Last Refresh: " + DateTime.Now.ToString("h:mm:ss tt");
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
            LoadDashboard();
        }

        // ============================================================
        //  MAIN DASHBOARD LOGIC (KPI Cards & Charts)
        // ============================================================
        private void LoadDashboard()
        {
            try
            {
                LoadKPICards();
                LoadSalesChart();
                LoadPaymentChart();
                LoadStockChart();
                LoadPromotionChart();
                UpdateLastRefreshLabel();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Dashboard error: " + ex.Message);
            }
        }

        private void LoadKPICards()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1. Total Sales Today
                object salesToday = new SqlCommand(
                    "SELECT ISNULL(SUM(TotalAmount),0) FROM [Transaction] WHERE CAST(TransactionDate AS DATE)=CAST(GETDATE() AS DATE)", conn)
                    .ExecuteScalar();
                SetLabel("lblTotalSalesToday", "RM " + Convert.ToDecimal(salesToday).ToString("N2"));

                // 2. Total Transactions Today
                object txToday = new SqlCommand(
                    "SELECT COUNT(*) FROM [Transaction] WHERE CAST(TransactionDate AS DATE)=CAST(GETDATE() AS DATE)", conn)
                    .ExecuteScalar();
                SetLabel("lblTotalTransactions", txToday.ToString());

                // 3. Low Stock Items (<=5)
                object lowStock = new SqlCommand(
                    "SELECT COUNT(*) FROM [Inventory] WHERE Stock <= 5", conn)
                    .ExecuteScalar();
                SetLabel("lblLowStockItem", lowStock.ToString() + " Items");

                // 4. Top Selling Product (Milo dll.)
                SqlCommand cmdTop = new SqlCommand(
                    @"SELECT TOP 1 I.ItemName FROM [TransactionDetail] TD
                      JOIN [Inventory] I ON TD.ItemID=I.ItemID
                      GROUP BY I.ItemName ORDER BY SUM(TD.Quantity) DESC", conn);
                object topProd = cmdTop.ExecuteScalar();
                SetLabel("lblTopProduct", topProd != null ? topProd.ToString() : "N/A");

                // 5. New Customers (Role='Customer')
                object custCount = new SqlCommand(
           "SELECT COUNT(*) FROM [Transaction] WHERE TransactionID IS NOT NULL", conn
       ).ExecuteScalar();

                SetLabel("lblNewCustomer", custCount.ToString());

                // 6. Total Revenue This Month
                object monthRev = new SqlCommand(
                    @"SELECT ISNULL(SUM(TotalAmount),0) FROM [Transaction]
                      WHERE MONTH(TransactionDate)=MONTH(GETDATE()) AND YEAR(TransactionDate)=YEAR(GETDATE())", conn)
                    .ExecuteScalar();
                SetLabel("lblMonthRevenue", "RM " + Convert.ToDecimal(monthRev).ToString("N2"));
            }
        }

        private void LoadSalesChart()
        {
            try
            {
                string query = @"SELECT FORMAT(TransactionDate,'MMM yyyy') AS MonthLabel,
                                        SUM(TotalAmount) AS TotalSales, COUNT(*) AS TotalTx
                                 FROM [Transaction]
                                 WHERE TransactionDate >= DATEADD(MONTH,-6,GETDATE())
                                 GROUP BY FORMAT(TransactionDate,'MMM yyyy'),YEAR(TransactionDate),MONTH(TransactionDate)
                                 ORDER BY YEAR(TransactionDate),MONTH(TransactionDate)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    chartSales.Series.Clear();
                    chartSales.ChartAreas[0].AxisX.LabelStyle.Angle = -30;
                    chartSales.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 8);
                    chartSales.ChartAreas[0].AxisY.Title = "Revenue (RM)";
                    chartSales.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                    chartSales.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Segoe UI", 8);

                    // Bar Series - Jualan (Kiri)
                    Series serRev = new Series("Revenue (RM)");
                    serRev.ChartType = SeriesChartType.Column;
                    serRev.Color = Color.FromArgb(139, 0, 0);
                    serRev.IsValueShownAsLabel = false; // Ditutup supaya tidak bertindih dengan garisan

                    // Line Series - Bilangan Urus Niaga (Paksi Kanan)
                    Series serTx = new Series("# Transactions");
                    serTx.ChartType = SeriesChartType.Line;
                    serTx.Color = Color.FromArgb(255, 140, 0);
                    serTx.BorderWidth = 3;
                    serTx.MarkerStyle = MarkerStyle.Circle;
                    serTx.MarkerSize = 8;
                    serTx.YAxisType = AxisType.Secondary;
                    serTx.IsValueShownAsLabel = true; // Hanya papar nombor transaksi di titik bulat graf garisan
                    serTx.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                    foreach (DataRow row in dt.Rows)
                    {
                        serRev.Points.AddXY(row["MonthLabel"].ToString(), Convert.ToDouble(row["TotalSales"]));
                        serTx.Points.AddXY(row["MonthLabel"].ToString(), Convert.ToInt32(row["TotalTx"]));
                    }

                    chartSales.Series.Add(serRev);
                    chartSales.Series.Add(serTx);

                    chartSales.ChartAreas[0].AxisY2.Title = "Transactions Count";
                    chartSales.ChartAreas[0].AxisY2.LabelStyle.Font = new Font("Segoe UI", 8);
                    chartSales.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;

                    chartSales.Titles.Clear();
                    Title t = chartSales.Titles.Add("Sales Performance - Last 6 Months");
                    t.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("SalesChart: " + ex.Message); }
        }

        private void LoadPaymentChart()
        {
            try
            {
                string query = @"SELECT
                    CASE WHEN Status LIKE '%QR%' THEN 'QR Payment'
                         WHEN Status LIKE '%Wallet%' OR Status LIKE '%wallet%' THEN 'E-Wallet'
                         WHEN Status LIKE '%Banking%' OR Status LIKE '%banking%' OR Status LIKE '%Online%' THEN 'Online Banking'
                         WHEN Status LIKE '%Cash%' THEN 'Cash'
                         ELSE ISNULL(Status,'Unknown') END AS PayMethod,
                    COUNT(*) AS Cnt, ISNULL(SUM(TotalAmount),0) AS Total
                    FROM [Transaction] GROUP BY
                    CASE WHEN Status LIKE '%QR%' THEN 'QR Payment'
                         WHEN Status LIKE '%Wallet%' OR Status LIKE '%wallet%' THEN 'E-Wallet'
                         WHEN Status LIKE '%Banking%' OR Status LIKE '%banking%' OR Status LIKE '%Online%' THEN 'Online Banking'
                         WHEN Status LIKE '%Cash%' THEN 'Cash'
                         ELSE ISNULL(Status,'Unknown') END";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    chartPayment.Series.Clear();
                    Series ser = new Series("Payment");
                    ser.ChartType = SeriesChartType.Doughnut;
                    ser["DoughnutRadius"] = "60";
                    ser.IsValueShownAsLabel = true;

                    Color[] palette = { Color.FromArgb(139, 0, 0), Color.FromArgb(220, 20, 60), Color.FromArgb(255, 99, 71), Color.FromArgb(255, 165, 0) };
                    int i = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        DataPoint dp = new DataPoint();
                        dp.SetValueXY(row["PayMethod"].ToString(), Convert.ToInt32(row["Cnt"]));
                        dp.Label = row["PayMethod"] + "\n" + row["Cnt"] + " txn\nRM" + Convert.ToDecimal(row["Total"]).ToString("N2");
                        dp.Color = palette[i % palette.Length];
                        ser.Points.Add(dp);
                        i++;
                    }
                    chartPayment.Series.Add(ser);
                    chartPayment.Titles.Clear();
                    chartPayment.Titles.Add("Payment Method Distribution");
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("PaymentChart: " + ex.Message); }
        }

        private void LoadStockChart()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT TOP 10 ItemName, Stock FROM [Inventory] ORDER BY Stock ASC", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    chartStock.Series.Clear();
                    chartStock.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                    chartStock.ChartAreas[0].AxisY.Title = "Stock Qty";
                    chartStock.ChartAreas[0].AxisY.StripLines.Clear();

                    StripLine sl = new StripLine();
                    sl.IntervalOffset = 5; sl.StripWidth = 0.5;
                    sl.BackColor = Color.FromArgb(60, 255, 0, 0);
                    sl.Text = "Low Stock Threshold (<=5)";
                    chartStock.ChartAreas[0].AxisY.StripLines.Add(sl);

                    Series ser = new Series("Stock");
                    ser.ChartType = SeriesChartType.Bar;
                    ser.IsValueShownAsLabel = true;

                    foreach (DataRow row in dt.Rows)
                    {
                        int stock = Convert.ToInt32(row["Stock"]);
                        DataPoint dp = new DataPoint();
                        dp.SetValueXY(row["ItemName"].ToString(), stock);
                        dp.Color = stock <= 5 ? Color.Red : Color.FromArgb(139, 0, 0);
                        ser.Points.Add(dp);
                    }
                    chartStock.Series.Add(ser);
                    chartStock.Titles.Clear();
                    chartStock.Titles.Add("Stock Level (Red = Low ≤5)");
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("StockChart: " + ex.Message); }
        }

        private void LoadPromotionChart()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(
                        "SELECT PromotionName, Discount, DATEDIFF(DAY,GETDATE(),EndDate) AS DaysLeft FROM [Promotion] ORDER BY Discount DESC", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    chartPromotion.Series.Clear();
                    chartPromotion.ChartAreas[0].AxisX.LabelStyle.Angle = -30;
                    chartPromotion.ChartAreas[0].AxisY.Title = "Discount (%)";
                    chartPromotion.ChartAreas[0].AxisY.Maximum = 100;

                    Series ser = new Series("Discount");
                    ser.ChartType = SeriesChartType.Column;
                    ser.IsValueShownAsLabel = true;
                    ser.LabelFormat = "#'%'";

                    foreach (DataRow row in dt.Rows)
                    {
                        int daysLeft = row["DaysLeft"] == DBNull.Value ? -1 : Convert.ToInt32(row["DaysLeft"]);
                        DataPoint dp = new DataPoint();
                        dp.SetValueXY(row["PromotionName"].ToString(), Convert.ToDouble(row["Discount"]));
                        dp.ToolTip = "Days Left: " + (daysLeft >= 0 ? daysLeft.ToString() : "Expired");
                        dp.Color = daysLeft >= 0 ? Color.FromArgb(139, 0, 0) : Color.Gray;
                        ser.Points.Add(dp);
                    }
                    chartPromotion.Series.Add(ser);
                    chartPromotion.Titles.Clear();
                    chartPromotion.Titles.Add("Active Promotions (Gray=Expired)");
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("PromoChart: " + ex.Message); }
        }

        // ============================================================
        //  PAYMENT MANAGEMENT TAB LOGIC (Live Counter QR/Online/Wallet)
        // ============================================================
        private void SetupPaymentSummary()
        {
            RefreshPaymentSummary();
        }

        private void RefreshPaymentSummary()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT
                        CASE WHEN Status LIKE '%QR%' THEN 'QR'
                             WHEN Status LIKE '%Wallet%' OR Status LIKE '%wallet%' THEN 'Wallet'
                             WHEN Status LIKE '%Banking%' OR Status LIKE '%banking%' OR Status LIKE '%Online%' THEN 'Banking'
                             ELSE 'Other' END AS PayType,
                        COUNT(*) AS Cnt, ISNULL(SUM(TotalAmount),0) AS Total
                        FROM [Transaction] GROUP BY
                        CASE WHEN Status LIKE '%QR%' THEN 'QR'
                             WHEN Status LIKE '%Wallet%' OR Status LIKE '%wallet%' THEN 'Wallet'
                             WHEN Status LIKE '%Banking%' OR Status LIKE '%banking%' OR Status LIKE '%Online%' THEN 'Banking'
                             ELSE 'Other' END";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Tetapan nilai lalai (0) sekiranya tiada data dalam DB
                    SetLabel("lblQRCount", "0"); SetLabel("lblQRTotal", "RM0.00");
                    SetLabel("lblWalletCount", "0"); SetLabel("lblWalletTotal", "RM0.00");
                    SetLabel("lblBankingCount", "0"); SetLabel("lblBankingTotal", "RM0.00");

                    foreach (DataRow row in dt.Rows)
                    {
                        string type = row["PayType"].ToString();
                        int cnt = Convert.ToInt32(row["Cnt"]);
                        decimal total = Convert.ToDecimal(row["Total"]);

                        if (type == "QR")
                        {
                            SetLabel("lblQRCount", cnt.ToString());
                            SetLabel("lblQRTotal", "RM " + total.ToString("N2"));
                        }
                        else if (type == "Wallet")
                        {
                            SetLabel("lblWalletCount", cnt.ToString());
                            SetLabel("lblWalletTotal", "RM " + total.ToString("N2"));
                        }
                        else if (type == "Banking")
                        {
                            SetLabel("lblBankingCount", cnt.ToString());
                            SetLabel("lblBankingTotal", "RM " + total.ToString("N2"));
                        }
                    }

                    object grandTotal = new SqlCommand("SELECT ISNULL(SUM(TotalAmount),0) FROM [Transaction]", conn).ExecuteScalar();
                    SetLabel("lblGrandTotal", "Grand Total Revenue: RM " + Convert.ToDecimal(grandTotal).ToString("N2"));
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("PaymentSummary Error: " + ex.Message); }
        }

        // ============================================================
        //  PRODUCT MANAGEMENT TAB LOGIC
        // ============================================================
        private void SetupProductSearch()
        {
            txtSearchProduct = new TextBox();
            txtSearchProduct.Text = "🔍 Search item name or ID...";
            txtSearchProduct.ForeColor = Color.Gray;
            txtSearchProduct.Font = new Font("Segoe UI", 10);
            txtSearchProduct.Width = 250;
            txtSearchProduct.Location = new Point(20, 10);
            txtSearchProduct.TextChanged += (s, e) => RefreshProductGrid();

            txtSearchProduct.GotFocus += (s, e) => {
                if (txtSearchProduct.Text == "🔍 Search item name or ID...")
                {
                    txtSearchProduct.Text = ""; txtSearchProduct.ForeColor = Color.Black;
                }
            };
            txtSearchProduct.LostFocus += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtSearchProduct.Text))
                {
                    txtSearchProduct.Text = "🔍 Search item name or ID..."; txtSearchProduct.ForeColor = Color.Gray;
                }
            };
            tabPage2.Controls.Add(txtSearchProduct);
            txtSearchProduct.BringToFront();

            cmbCategoryFilter = new ComboBox();
            cmbCategoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategoryFilter.Font = new Font("Segoe UI", 10);
            cmbCategoryFilter.Width = 150;
            cmbCategoryFilter.Location = new Point(280, 10);
            cmbCategoryFilter.Items.Add("All Categories");
            cmbCategoryFilter.SelectedIndex = 0;
            cmbCategoryFilter.SelectedIndexChanged += (s, e) => RefreshProductGrid();
            tabPage2.Controls.Add(cmbCategoryFilter);
            cmbCategoryFilter.BringToFront();

            lblProductStats = new Label();
            lblProductStats.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblProductStats.ForeColor = Color.FromArgb(139, 0, 0);
            lblProductStats.Width = 400; lblProductStats.Height = 20;
            lblProductStats.Location = new Point(440, 14);
            tabPage2.Controls.Add(lblProductStats);
            lblProductStats.BringToFront();

            LoadCategoryDropdown();
            RefreshProductGrid();

            if (dgvProducts != null)
                dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;
        }

        private void LoadCategoryDropdown()
        {
            if (cmbCategoryFilter == null) return;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataReader rdr = new SqlCommand("SELECT DISTINCT Category FROM [Inventory] WHERE Category IS NOT NULL ORDER BY Category", conn).ExecuteReader();
                    while (rdr.Read())
                        if (!cmbCategoryFilter.Items.Contains(rdr[0].ToString()))
                            cmbCategoryFilter.Items.Add(rdr[0].ToString());
                }
            }
            catch { }
        }

        private void RefreshProductGrid()
        {
            string search = txtSearchProduct?.Text.Trim() ?? "";
            if (search == "🔍 Search item name or ID...") search = "";
            string cat = cmbCategoryFilter?.SelectedItem?.ToString() ?? "All Categories";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT ItemID, ItemName, Category, Stock, Price,
                                     CASE WHEN Stock<=0 THEN 'Out of Stock'
                                          WHEN Stock<=5 THEN 'Low Stock'
                                          ELSE 'In Stock' END AS Status
                                     FROM [Inventory]
                                     WHERE (@search='' OR ItemName LIKE @search OR ItemID LIKE @search)
                                     AND (@cat='All Categories' OR Category=@cat)
                                     ORDER BY Stock ASC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                    cmd.Parameters.AddWithValue("@cat", cat);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dgvProducts != null)
                    {
                        dgvProducts.DataSource = dt;
                        foreach (DataGridViewRow row in dgvProducts.Rows)
                        {
                            string status = row.Cells["Status"]?.Value?.ToString() ?? "";
                            if (status == "Low Stock") row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 180);
                            else if (status == "Out of Stock") row.DefaultCellStyle.BackColor = Color.FromArgb(255, 180, 180);
                        }
                    }
                    if (lblProductStats != null)
                        lblProductStats.Text = $"Total: {dt.Rows.Count} items | Low/Out: {dt.Select("Status<>'In Stock'").Length}";
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("ProductGrid: " + ex.Message); }
        }

        private void DgvProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;
            DataGridViewRow r = dgvProducts.CurrentRow;
            try
            {
                itemIDTextBox.Text = r.Cells["ItemID"]?.Value?.ToString() ?? "";
                itemNameTextBox.Text = r.Cells["ItemName"]?.Value?.ToString() ?? "";
                Control catBox = FindControlRecursive(this, "categoryTextBox");
                if (catBox != null) catBox.Text = r.Cells["Category"]?.Value?.ToString() ?? "";
                stockTextBox.Text = r.Cells["Stock"]?.Value?.ToString() ?? "";
                priceTextBox.Text = r.Cells["Price"]?.Value?.ToString() ?? "";
                statusTextBox.Text = r.Cells["Status"]?.Value?.ToString() ?? "";
            }
            catch { }
        }

        private void dgvProducts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            RefreshProductGrid();
        }

        // ============================================================
        //  PRODUCT CRUD ACTIONS
        // ============================================================
        private void AddItem_Click(object sender, EventArgs e)
        {
            if (!ValidateInventoryFields()) return;
            string query = "INSERT INTO [Inventory] (ItemID, ItemName, Stock, Price, Status) VALUES (@ItemID, @ItemName, @Stock, @Price, @Status)";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                int stock = Convert.ToInt32(stockTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@ItemID", itemIDTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@ItemName", itemNameTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Stock", stock);
                cmd.Parameters.AddWithValue("@Price", Convert.ToDecimal(priceTextBox.Text.Trim()));
                cmd.Parameters.AddWithValue("@Status", stock > 5 ? "In Stock" : stock > 0 ? "Low Stock" : "Out of Stock");
                ExecuteCRUD(cmd, "Product added successfully!", () => {
                    ClearInventoryFields();
                    this.inventoryTableAdapter.Fill(this.dataSet1.Inventory);
                    RefreshProductGrid();
                    LoadDashboard();
                });
            }
        }

        private void EditItem_Click(object sender, EventArgs e)
        {
            if (!ValidateInventoryFields()) return;
            string query = "UPDATE [Inventory] SET ItemName=@ItemName, Stock=@Stock, Price=@Price, Status=@Status WHERE ItemID=@ItemID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                int stock = Convert.ToInt32(stockTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@ItemName", itemNameTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Stock", stock);
                cmd.Parameters.AddWithValue("@Price", Convert.ToDecimal(priceTextBox.Text.Trim()));
                cmd.Parameters.AddWithValue("@Status", stock > 5 ? "In Stock" : stock > 0 ? "Low Stock" : "Out of Stock");
                cmd.Parameters.AddWithValue("@ItemID", itemIDTextBox.Text.Trim());
                ExecuteCRUD(cmd, "Product updated!", () => {
                    ClearInventoryFields();
                    this.inventoryTableAdapter.Fill(this.dataSet1.Inventory);
                    RefreshProductGrid();
                    LoadDashboard();
                });
            }
        }

        private void DeleteItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(itemIDTextBox.Text)) { MessageBox.Show("Select a product first."); return; }
            if (MessageBox.Show($"Delete item '{itemNameTextBox.Text}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            string query = "DELETE FROM [Inventory] WHERE ItemID=@ItemID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@ItemID", itemIDTextBox.Text.Trim());
                ExecuteCRUD(cmd, "Product deleted!", () => {
                    ClearInventoryFields();
                    this.inventoryTableAdapter.Fill(this.dataSet1.Inventory);
                    RefreshProductGrid();
                    LoadDashboard();
                });
            }
        }

        private bool ValidateInventoryFields()
        {
            if (string.IsNullOrWhiteSpace(itemIDTextBox.Text)) { MessageBox.Show("Item ID required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            if (!int.TryParse(stockTextBox.Text.Trim(), out _)) { MessageBox.Show("Stock must be a number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            if (!decimal.TryParse(priceTextBox.Text.Trim(), out _)) { MessageBox.Show("Price must be a number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            return true;
        }

        private void ClearInventoryFields()
        {
            itemIDTextBox.Clear(); itemNameTextBox.Clear();
            stockTextBox.Clear(); priceTextBox.Clear(); statusTextBox.Clear();
        }

        // ============================================================
        //  TRANSACTION CRUD ACTIONS (Payment Tab)
        // ============================================================
        private void AddTransaction_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO [Transaction] (TransactionID, UserID, TransactionDate, TotalAmount, Status) VALUES (@ID, @UID, @Date, @Amount, @Status)";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@ID", transactionIDTextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@UID", userIDTextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@Date", transactionDateDateTimePicker.Value);
                cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(totalAmountTextBox.Text.Trim()));
                cmd.Parameters.AddWithValue("@Status", statusTextBox.Text.Trim());
                ExecuteCRUD(cmd, "Transaction added.", () => {
                    transactionIDTextBox1.Clear(); userIDTextBox1.Clear();
                    totalAmountTextBox.Clear(); statusTextBox.Clear();
                    this.transactionTableAdapter.Fill(this.dataSet1.Transaction);
                    RefreshPaymentSummary();
                    LoadDashboard();
                });
            }
        }

        private void DeleteTransaction_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete this transaction?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            string query = "DELETE FROM [Transaction] WHERE TransactionID=@ID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@ID", transactionIDTextBox1.Text.Trim());
                ExecuteCRUD(cmd, "Transaction deleted.", () => {
                    transactionIDTextBox1.Clear(); userIDTextBox1.Clear();
                    totalAmountTextBox.Clear(); statusTextBox.Clear();
                    this.transactionTableAdapter.Fill(this.dataSet1.Transaction);
                    RefreshPaymentSummary();
                    LoadDashboard();
                });
            }
        }

        private void UpdateTransaction_Click(object sender, EventArgs e)
        {
            string query = "UPDATE [Transaction] SET UserID=@UID, TransactionDate=@Date, TotalAmount=@Amount, Status=@Status WHERE TransactionID=@ID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@UID", userIDTextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@Date", transactionDateDateTimePicker.Value);
                cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(totalAmountTextBox.Text.Trim()));
                cmd.Parameters.AddWithValue("@Status", statusTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@ID", transactionIDTextBox1.Text.Trim());
                ExecuteCRUD(cmd, "Transaction updated.", () => {
                    transactionIDTextBox1.Clear(); userIDTextBox1.Clear();
                    totalAmountTextBox.Clear(); statusTextBox.Clear();
                    this.transactionTableAdapter.Fill(this.dataSet1.Transaction);
                    RefreshPaymentSummary();
                    LoadDashboard();
                });
            }
        }

        private void AddTransactionDetail_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO [TransactionDetail] (TransactionID, ItemID, Quantity, Price) VALUES (@TXID, @ItemID, @Qty, @Price)";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@TXID", transactionIDTextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@ItemID", itemIDTextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@Qty", Convert.ToInt32(quantityTextBox.Text.Trim()));
                cmd.Parameters.AddWithValue("@Price", Convert.ToDecimal(priceTextBox1.Text.Trim()));
                ExecuteCRUD(cmd, "Detail added.", () => {
                    transactionIDTextBox1.Clear(); itemIDTextBox1.Clear();
                    quantityTextBox.Clear(); priceTextBox1.Clear();
                    this.transactionDetailTableAdapter.Fill(this.dataSet1.TransactionDetail);
                    LoadDashboard();
                });
            }
        }

        private void DeleteTransactionDetail_Click(object sender, EventArgs e)
        {
            string query = "DELETE FROM [TransactionDetail] WHERE TransactionID=@ID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@ID", transactionIDTextBox1.Text.Trim());
                ExecuteCRUD(cmd, "Detail deleted.", () => {
                    transactionIDTextBox1.Clear(); itemIDTextBox1.Clear();
                    quantityTextBox.Clear(); priceTextBox1.Clear();
                    this.transactionDetailTableAdapter.Fill(this.dataSet1.TransactionDetail);
                    LoadDashboard();
                });
            }
        }

        private void UpdateTransactionDetail_Click(object sender, EventArgs e)
        {
            string query = "UPDATE [TransactionDetail] SET ItemID=@ItemID, Quantity=@Qty, Price=@Price WHERE TransactionID=@TXID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@ItemID", itemIDTextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@Qty", Convert.ToInt32(quantityTextBox.Text.Trim()));
                cmd.Parameters.AddWithValue("@Price", Convert.ToDecimal(priceTextBox1.Text.Trim()));
                cmd.Parameters.AddWithValue("@TXID", transactionIDTextBox1.Text.Trim());
                ExecuteCRUD(cmd, "Detail updated.", () => {
                    transactionIDTextBox1.Clear(); itemIDTextBox1.Clear();
                    quantityTextBox.Clear(); priceTextBox1.Clear();
                    this.transactionDetailTableAdapter.Fill(this.dataSet1.TransactionDetail);
                    LoadDashboard();
                });
            }
        }

        // ============================================================
        //  MAINTENANCE TAB LOGIC
        // ============================================================
        private void SetupMaintenanceSearch()
        {
            TabControl tc = FindControlRecursive(this, "tabControl1") as TabControl;
            if (tc == null) return;
            TabPage tabMaint = tc.TabPages.Cast<TabPage>().FirstOrDefault(tp => tp.Text.ToLower().Contains("maint"));
            if (tabMaint == null) return;

            txtSearchMaint = new TextBox { Text = "🔍 Search by ID, ItemID or Technician...", Font = new Font("Segoe UI", 10), Width = 280, Location = new Point(20, 10) };
            txtSearchMaint.TextChanged += (s, e) => RefreshMaintenanceGrid();
            tabMaint.Controls.Add(txtSearchMaint);
            txtSearchMaint.BringToFront();

            cmbMaintStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10), Width = 140, Location = new Point(310, 10) };
            cmbMaintStatus.Items.AddRange(new[] { "All Status", "Pending", "In Progress", "Completed", "Cancelled" });
            cmbMaintStatus.SelectedIndex = 0;
            cmbMaintStatus.SelectedIndexChanged += (s, e) => RefreshMaintenanceGrid();
            tabMaint.Controls.Add(cmbMaintStatus);
            cmbMaintStatus.BringToFront();

            lblMaintStats = new Label { Font = new Font("Segoe UI", 9, FontStyle.Italic), ForeColor = Color.FromArgb(139, 0, 0), Width = 350, Height = 20, Location = new Point(460, 14) };
            tabMaint.Controls.Add(lblMaintStats);
            lblMaintStats.BringToFront();

            RefreshMaintenanceGrid();
        }

        private void RefreshMaintenanceGrid()
        {
            TabControl tc = FindControlRecursive(this, "tabControl1") as TabControl;
            if (tc == null) return;
            TabPage tabMaint = tc.TabPages.Cast<TabPage>().FirstOrDefault(tp => tp.Text.ToLower().Contains("maint"));
            if (tabMaint == null) return;

            DataGridView maintDgv = tabMaint.Controls.OfType<DataGridView>().FirstOrDefault();
            if (maintDgv == null) return;

            string search = txtSearchMaint?.Text.Trim() ?? "";
            if (search.StartsWith("🔍")) search = "";
            string status = cmbMaintStatus?.SelectedItem?.ToString() ?? "All Status";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT MaintenanceID, ItemID, TechnicianID, IssueDescription, MaintenanceDate, Status
                                     FROM [Maintenance]
                                     WHERE (@s='' OR MaintenanceID LIKE @s OR ItemID LIKE @s OR TechnicianID LIKE @s)
                                     AND (@st='All Status' OR Status=@st)
                                     ORDER BY MaintenanceDate DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@s", "%" + search + "%");
                    cmd.Parameters.AddWithValue("@st", status);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    maintDgv.DataSource = dt;

                    foreach (DataGridViewRow row in maintDgv.Rows)
                    {
                        string st = row.Cells["Status"]?.Value?.ToString() ?? "";
                        if (st == "Pending") row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 180);
                        else if (st == "In Progress") row.DefaultCellStyle.BackColor = Color.FromArgb(180, 220, 255);
                        else if (st == "Completed") row.DefaultCellStyle.BackColor = Color.FromArgb(180, 255, 180);
                    }

                    if (lblMaintStats != null)
                    {
                        int pending = dt.Select("Status='Pending'").Length;
                        int inProg = dt.Select("Status='In Progress'").Length;
                        int done = dt.Select("Status='Completed'").Length;
                        lblMaintStats.Text = $"Total: {dt.Rows.Count} | Pending: {pending} | In Progress: {inProg} | Done: {done}";
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("MaintGrid: " + ex.Message); }
        }

        private void AddMaintenance_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO [Maintenance] (MaintenanceID, ItemID, TechnicianID, IssueDescription, MaintenanceDate, Status) VALUES (@MID,@ItemID,@TechID,@Issue,@Date,@Status)";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@MID", maintenanceIDTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@ItemID", itemIDTextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@TechID", technicianIDTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Issue", issueDescriptionTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Date", maintenanceDateDateTimePicker.Value);
                cmd.Parameters.AddWithValue("@Status", statusTextBox.Text.Trim());
                ExecuteCRUD(cmd, "Maintenance record added!", () => {
                    maintenanceIDTextBox.Clear(); itemIDTextBox1.Clear();
                    technicianIDTextBox.Clear(); issueDescriptionTextBox.Clear(); statusTextBox.Clear();
                    this.maintenanceTableAdapter.Fill(this.dataSet1.Maintenance);
                    RefreshMaintenanceGrid();
                });
            }
        }

        private void DeleteMaintenence_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete this maintenance record?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            string query = "DELETE FROM [Maintenance] WHERE MaintenanceID=@MID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@MID", maintenanceIDTextBox.Text.Trim());
                ExecuteCRUD(cmd, "Record deleted!", () => {
                    maintenanceIDTextBox.Clear(); itemIDTextBox1.Clear();
                    technicianIDTextBox.Clear(); issueDescriptionTextBox.Clear(); statusTextBox.Clear();
                    this.maintenanceTableAdapter.Fill(this.dataSet1.Maintenance);
                    RefreshMaintenanceGrid();
                });
            }
        }

        private void UpdateMaintenence_Click(object sender, EventArgs e)
        {
            string query = "UPDATE [Maintenance] SET ItemID=@ItemID, TechnicianID=@TechID, IssueDescription=@Issue, MaintenanceDate=@Date, Status=@Status WHERE MaintenanceID=@MID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@ItemID", itemIDTextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@TechID", technicianIDTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Issue", issueDescriptionTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Date", maintenanceDateDateTimePicker.Value);
                cmd.Parameters.AddWithValue("@Status", statusTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@MID", maintenanceIDTextBox.Text.Trim());
                ExecuteCRUD(cmd, "Record updated!", () => {
                    maintenanceIDTextBox.Clear(); itemIDTextBox1.Clear();
                    technicianIDTextBox.Clear(); issueDescriptionTextBox.Clear(); statusTextBox.Clear();
                    this.maintenanceTableAdapter.Fill(this.dataSet1.Maintenance);
                    RefreshMaintenanceGrid();
                });
            }
        }

        // ============================================================
        //  USER & ACCESS CONTROL TAB LOGIC
        // ============================================================
        private void SetupUserSearch()
        {
            TabControl tc = FindControlRecursive(this, "tabControl1") as TabControl;
            if (tc == null) return;
            TabPage tabUser = tc.TabPages.Cast<TabPage>().FirstOrDefault(tp => tp.Text.ToLower().Contains("user") || tp.Text.ToLower().Contains("access"));
            if (tabUser == null) return;

            txtSearchUser = new TextBox { Text = "🔍 Search by username, email, or ID...", Font = new Font("Segoe UI", 10), Width = 260, Location = new Point(20, 10) };
            txtSearchUser.TextChanged += (s, e) => RefreshUserGrid();
            tabUser.Controls.Add(txtSearchUser);
            txtSearchUser.BringToFront();

            cmbRoleFilter = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10), Width = 130, Location = new Point(290, 10) };
            cmbRoleFilter.Items.AddRange(new[] { "All Roles", "Admin", "Customer", "Technician" });
            cmbRoleFilter.SelectedIndex = 0;
            cmbRoleFilter.SelectedIndexChanged += (s, e) => RefreshUserGrid();
            tabUser.Controls.Add(cmbRoleFilter);
            cmbRoleFilter.BringToFront();

            lblUserStats = new Label { Font = new Font("Segoe UI", 9, FontStyle.Italic), ForeColor = Color.FromArgb(139, 0, 0), Width = 350, Height = 20, Location = new Point(430, 14) };
            tabUser.Controls.Add(lblUserStats);
            lblUserStats.BringToFront();

            RefreshUserGrid();
        }

        private void RefreshUserGrid()
        {
            TabControl tc = FindControlRecursive(this, "tabControl1") as TabControl;
            if (tc == null) return;
            TabPage tabUser = tc.TabPages.Cast<TabPage>().FirstOrDefault(tp => tp.Text.ToLower().Contains("user") || tp.Text.ToLower().Contains("access"));
            if (tabUser == null) return;

            DataGridView userDgv = tabUser.Controls.OfType<DataGridView>().FirstOrDefault();
            if (userDgv == null) return;

            string search = txtSearchUser?.Text.Trim() ?? "";
            if (search.StartsWith("🔍")) search = "";
            string role = cmbRoleFilter?.SelectedItem?.ToString() ?? "All Roles";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT UserID, Username, PasswordUser, Role, Email, Phone FROM [User]
                                     WHERE (@s='' OR Username LIKE @s OR Email LIKE @s OR CAST(UserID AS VARCHAR) LIKE @s)
                                     AND (@r='All Roles' OR Role=@r)
                                     ORDER BY Username";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@s", "%" + search + "%");
                    cmd.Parameters.AddWithValue("@r", role);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    userDgv.DataSource = dt;

                    foreach (DataGridViewRow row in userDgv.Rows)
                    {
                        string r = row.Cells["Role"]?.Value?.ToString() ?? "";
                        if (r == "Admin") row.DefaultCellStyle.BackColor = Color.FromArgb(220, 200, 255);
                        else if (r == "Technician") row.DefaultCellStyle.BackColor = Color.FromArgb(180, 230, 255);
                    }

                    if (lblUserStats != null)
                    {
                        int admins = dt.Select("Role='Admin'").Length;
                        int techs = dt.Select("Role='Technician'").Length;
                        int custs = dt.Select("Role='Customer'").Length;
                        lblUserStats.Text = $"Total: {dt.Rows.Count} | Admins: {admins} | Techs: {techs} | Customers: {custs}";
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("UserGrid: " + ex.Message); }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(userIDTextBox1.Text) || string.IsNullOrWhiteSpace(usernameTextBox.Text))
            { MessageBox.Show("User ID and Username are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string query = "INSERT INTO [User] (UserID, Username, PasswordUser, Role, Email, Phone) VALUES (@UserID, @Username, @Password, @Role, @Email, @Phone)";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@UserID", userIDTextBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@Username", usernameTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Password", passwordUserTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Role", roleTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", emailTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", phoneTextBox.Text.Trim());
                ExecuteCRUD(cmd, "User added!", () => {
                    userIDTextBox1.Clear(); usernameTextBox.Clear(); passwordUserTextBox.Clear();
                    roleTextBox.Clear(); emailTextBox.Clear(); phoneTextBox.Clear();
                    this.userTableAdapter.Fill(this.dataSet1.User);
                    RefreshUserGrid();
                    LoadDashboard();
                });
            }
        }

        private void DeleteUser_Click(object sender, EventArgs e)
        {
            string query = "UPDATE [User] SET Username=@Username, PasswordUser=@Password, Role=@Role, Email=@Email, Phone=@Phone WHERE UserID=@UserID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Username", usernameTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Password", passwordUserTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Role", roleTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", emailTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", phoneTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@UserID", userIDTextBox1.Text.Trim());
                ExecuteCRUD(cmd, "User updated!", () => {
                    userIDTextBox1.Clear(); usernameTextBox.Clear(); passwordUserTextBox.Clear();
                    roleTextBox.Clear(); emailTextBox.Clear(); phoneTextBox.Clear();
                    this.userTableAdapter.Fill(this.dataSet1.User);
                    RefreshUserGrid();
                });
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete this user?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            string query = "DELETE FROM [User] WHERE UserID=@UserID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@UserID", userIDTextBox1.Text.Trim());
                ExecuteCRUD(cmd, "User deleted!", () => {
                    userIDTextBox1.Clear(); usernameTextBox.Clear(); passwordUserTextBox.Clear();
                    roleTextBox.Clear(); emailTextBox.Clear(); phoneTextBox.Clear();
                    this.userTableAdapter.Fill(this.dataSet1.User);
                    RefreshUserGrid();
                    LoadDashboard();
                });
            }
        }

        // ============================================================
        //  PROMOTION TAB LOGIC
        // ============================================================
        private void SetupPromotionAnalytics()
        {
            TabControl tc = FindControlRecursive(this, "tabControl1") as TabControl;
            if (tc == null) return;
            TabPage tabPromo = tc.TabPages.Cast<TabPage>().FirstOrDefault(tp => tp.Text.ToLower().Contains("promo"));
            if (tabPromo == null) return;

            lblPromoStats = new Label { Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(139, 0, 0), Width = 700, Height = 20, Location = new Point(20, 10) };
            tabPromo.Controls.Add(lblPromoStats);
            lblPromoStats.BringToFront();

            RefreshPromotionStats();
        }

        private void RefreshPromotionStats()
        {
            if (lblPromoStats == null) return;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    int total = Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM [Promotion]", conn).ExecuteScalar());
                    int active = Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM [Promotion] WHERE EndDate >= GETDATE()", conn).ExecuteScalar());
                    int expired = total - active;
                    object maxDisc = new SqlCommand("SELECT ISNULL(MAX(Discount),0) FROM [Promotion] WHERE EndDate>=GETDATE()", conn).ExecuteScalar();
                    object avgDisc = new SqlCommand("SELECT ISNULL(AVG(Discount),0) FROM [Promotion] WHERE EndDate>=GETDATE()", conn).ExecuteScalar();

                    lblPromoStats.Text = $"Total Promotions: {total}  |  Active: {active}  |  Expired: {expired}  |  " +
                                         $"Highest Discount: {Convert.ToDecimal(maxDisc):N1}%  |  Avg Discount: {Convert.ToDecimal(avgDisc):N1}%";
                }
            }
            catch { }
        }

        private void AddPromotion_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO [Promotion] (PromotionID, PromotionName, Discount, StartDate, EndDate, Status) VALUES (@PID,@PName,@Discount,@SDate,@EDate,@Status)";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@PID", promotionIDTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@PName", promotionNameTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Discount", Convert.ToDecimal(discountTextBox.Text.Trim()));
                cmd.Parameters.AddWithValue("@SDate", startDateDateTimePicker.Value);
                cmd.Parameters.AddWithValue("@EDate", endDateDateTimePicker.Value);
                cmd.Parameters.AddWithValue("@Status", statusTextBox.Text.Trim());
                ExecuteCRUD(cmd, "Promotion added!", () => {
                    promotionIDTextBox.Clear(); promotionNameTextBox.Clear();
                    discountTextBox.Clear(); statusTextBox.Clear();
                    this.promotionTableAdapter.Fill(this.dataSet1.Promotion);
                    RefreshPromotionStats();
                    LoadDashboard();
                });
            }
        }

        private void DeletePromotion_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete this promotion?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            string query = "DELETE FROM [Promotion] WHERE PromotionID=@PID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@PID", promotionIDTextBox.Text.Trim());
                ExecuteCRUD(cmd, "Promotion deleted!", () => {
                    promotionIDTextBox.Clear(); promotionNameTextBox.Clear();
                    discountTextBox.Clear(); statusTextBox.Clear();
                    this.promotionTableAdapter.Fill(this.dataSet1.Promotion);
                    RefreshPromotionStats();
                    LoadDashboard();
                });
            }
        }

        private void UpdatePromotion_Click(object sender, EventArgs e)
        {
            string query = "UPDATE [Promotion] SET PromotionName=@PName, Discount=@Discount, StartDate=@SDate, EndDate=@EDate, Status=@Status WHERE PromotionID=@PID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@PName", promotionNameTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@Discount", Convert.ToDecimal(discountTextBox.Text.Trim()));
                cmd.Parameters.AddWithValue("@SDate", startDateDateTimePicker.Value);
                cmd.Parameters.AddWithValue("@EDate", endDateDateTimePicker.Value);
                cmd.Parameters.AddWithValue("@Status", statusTextBox.Text.Trim());
                cmd.Parameters.AddWithValue("@PID", promotionIDTextBox.Text.Trim());
                ExecuteCRUD(cmd, "Promotion updated!", () => {
                    promotionIDTextBox.Clear(); promotionNameTextBox.Clear();
                    discountTextBox.Clear(); statusTextBox.Clear();
                    this.promotionTableAdapter.Fill(this.dataSet1.Promotion);
                    RefreshPromotionStats();
                    LoadDashboard();
                });
            }
        }

        // ============================================================
        //  REPORT GENERATOR & EXPORT TAB (PDF, CSV, TXT)
        // ============================================================
        private void SetupReportTab()
        {
            TabControl tc = FindControlRecursive(this, "tabControl1") as TabControl;
            if (tc == null) return;
            TabPage tabReport = tc.TabPages.Cast<TabPage>().FirstOrDefault(tp => tp.Text.ToLower().Contains("report"));
            if (tabReport == null) return;

            tabReport.Controls.Clear();
            tabReport.Padding = new Padding(10);

            pnlReportHeader = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = Color.FromArgb(240, 240, 240) };
            tabReport.Controls.Add(pnlReportHeader);

            Label lblType = new Label { Text = "Report Type:", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(10, 15), AutoSize = true };
            pnlReportHeader.Controls.Add(lblType);

            cmbReportType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10), Width = 220, Location = new Point(10, 35) };
            cmbReportType.Items.AddRange(new[] { "Sales Summary Report", "Inventory Stock Report", "Payment Method Report", "Top Selling Products", "Maintenance Report", "Promotion Report", "User Activity Report" });
            cmbReportType.SelectedIndex = 0;
            pnlReportHeader.Controls.Add(cmbReportType);

            Label lblFrom = new Label { Text = "From:", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(245, 15), AutoSize = true };
            pnlReportHeader.Controls.Add(lblFrom);

            dtpReportFrom = new DateTimePicker { Font = new Font("Segoe UI", 10), Width = 160, Location = new Point(245, 35), Value = DateTime.Now.AddMonths(-1), Format = DateTimePickerFormat.Short };
            pnlReportHeader.Controls.Add(dtpReportFrom);

            Label lblTo = new Label { Text = "To:", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(420, 15), AutoSize = true };
            pnlReportHeader.Controls.Add(lblTo);

            dtpReportTo = new DateTimePicker { Font = new Font("Segoe UI", 10), Width = 160, Location = new Point(420, 35), Value = DateTime.Now, Format = DateTimePickerFormat.Short };
            pnlReportHeader.Controls.Add(dtpReportTo);

            Button btnGenerate = new Button { Text = "📊 Generate Report", Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.FromArgb(139, 0, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 160, Height = 35, Location = new Point(596, 28) };
            btnGenerate.Click += BtnGenerateReport_Click;
            pnlReportHeader.Controls.Add(btnGenerate);

            Button btnPDF = new Button { Text = "📄 Export PDF", Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.FromArgb(180, 0, 0), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 140, Height = 35, Location = new Point(766, 28) };
            btnPDF.Click += BtnExportPDF_Click;
            pnlReportHeader.Controls.Add(btnPDF);

            Button btnCSV = new Button { Text = "📑 Export CSV", Font = new Font("Segoe UI", 10, FontStyle.Bold), BackColor = Color.FromArgb(34, 139, 34), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Width = 130, Height = 35, Location = new Point(916, 28) };
            btnCSV.Click += BtnExportCSV_Click;
            pnlReportHeader.Controls.Add(btnCSV);

            lblReportSummary = new Label { Font = new Font("Segoe UI", 9, FontStyle.Bold | FontStyle.Italic), ForeColor = Color.FromArgb(139, 0, 0), Width = 1000, Height = 22, Location = new Point(10, 80) };
            pnlReportHeader.Controls.Add(lblReportSummary);

            dgvReport = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackColor = Color.White, BorderStyle = BorderStyle.None, RowHeadersVisible = false, AllowUserToAddRows = false, Font = new Font("Segoe UI", 9) };
            dgvReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(139, 0, 0);
            dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvReport.EnableHeadersVisualStyles = false;
            dgvReport.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 245, 245);
            tabReport.Controls.Add(dgvReport);
            dgvReport.BringToFront();

            BtnGenerateReport_Click(null, null);
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            if (cmbReportType == null || dgvReport == null) return;
            string reportType = cmbReportType.SelectedItem?.ToString() ?? "";
            DateTime from = dtpReportFrom?.Value ?? DateTime.Now.AddMonths(-1);
            DateTime to = dtpReportTo?.Value ?? DateTime.Now;
            currentReportTitle = reportType;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlDataAdapter da = null;
                    string summary = "";

                    switch (reportType)
                    {
                        case "Sales Summary Report":
                            da = new SqlDataAdapter(@"
                                SELECT FORMAT(TransactionDate,'dd/MM/yyyy') AS [Date],
                                       COUNT(*) AS [Total Transactions],
                                       SUM(TotalAmount) AS [Total Revenue (RM)],
                                       AVG(TotalAmount) AS [Avg per Transaction (RM)],
                                       MAX(TotalAmount) AS [Highest Sale (RM)],
                                       MIN(TotalAmount) AS [Lowest Sale (RM)]
                                FROM [Transaction]
                                WHERE TransactionDate BETWEEN @from AND @to
                                GROUP BY FORMAT(TransactionDate,'dd/MM/yyyy'), CAST(TransactionDate AS DATE)
                                ORDER BY CAST(TransactionDate AS DATE)", conn);
                            da.SelectCommand.Parameters.AddWithValue("@from", from.Date);
                            da.SelectCommand.Parameters.AddWithValue("@to", to.Date.AddDays(1));
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                decimal totalRev = dt.AsEnumerable().Sum(r => r.Field<decimal>("Total Revenue (RM)"));
                                summary = $"Period: {from:dd/MM/yyyy} - {to:dd/MM/yyyy}  |  Days: {dt.Rows.Count}  |  Total Revenue: RM {totalRev:N2}";
                            }
                            break;

                        case "Inventory Stock Report":
                            da = new SqlDataAdapter(@"
                                SELECT ItemID AS [Item ID], ItemName AS [Item Name],
                                       Category, Stock AS [Current Stock], Price AS [Price (RM)],
                                       Stock * Price AS [Stock Value (RM)],
                                       CASE WHEN Stock<=0 THEN 'OUT OF STOCK'
                                            WHEN Stock<=5 THEN 'LOW STOCK'
                                            ELSE 'In Stock' END AS [Status]
                                FROM [Inventory] ORDER BY Stock ASC", conn);
                            da.Fill(dt);
                            int outOfStock = dt.Select("Status='OUT OF STOCK'").Length;
                            int lowStock2 = dt.Select("Status='LOW STOCK'").Length;
                            decimal stockVal = dt.AsEnumerable().Sum(r => Convert.ToDecimal(r["Stock Value (RM)"]));
                            summary = $"Total Items: {dt.Rows.Count}  |  Out of Stock: {outOfStock}  |  Low Stock: {lowStock2}  |  Total Stock Value: RM {stockVal:N2}";
                            break;

                        case "Payment Method Report":
                            da = new SqlDataAdapter(@"
                                SELECT
                                    CASE WHEN Status LIKE '%QR%' THEN 'QR Payment'
                                         WHEN Status LIKE '%Wallet%' OR Status LIKE '%wallet%' THEN 'E-Wallet'
                                         WHEN Status LIKE '%Banking%' OR Status LIKE '%banking%' OR Status LIKE '%Online%' THEN 'Online Banking'
                                         WHEN Status LIKE '%Cash%' THEN 'Cash'
                                         ELSE ISNULL(Status,'Unknown') END AS [Payment Method],
                                    COUNT(*) AS [Total Transactions],
                                    SUM(TotalAmount) AS [Total Amount (RM)],
                                    AVG(TotalAmount) AS [Avg Amount (RM)]
                                FROM [Transaction]
                                WHERE TransactionDate BETWEEN @from AND @to
                                GROUP BY CASE WHEN Status LIKE '%QR%' THEN 'QR Payment'
                                              WHEN Status LIKE '%Wallet%' OR Status LIKE '%wallet%' THEN 'E-Wallet'
                                              WHEN Status LIKE '%Banking%' OR Status LIKE '%banking%' OR Status LIKE '%Online%' THEN 'Online Banking'
                                              WHEN Status LIKE '%Cash%' THEN 'Cash'
                                              ELSE ISNULL(Status,'Unknown') END", conn);
                            da.SelectCommand.Parameters.AddWithValue("@from", from.Date);
                            da.SelectCommand.Parameters.AddWithValue("@to", to.Date.AddDays(1));
                            da.Fill(dt);
                            summary = $"Period: {from:dd/MM/yyyy} - {to:dd/MM/yyyy}  |  Payment Methods: {dt.Rows.Count}";
                            break;

                        case "Top Selling Products":
                            da = new SqlDataAdapter(@"
                                SELECT I.ItemID AS [Item ID], I.ItemName AS [Item Name], I.Category,
                                       SUM(TD.Quantity) AS [Units Sold], I.Price AS [Unit Price (RM)],
                                       SUM(TD.Quantity * TD.Price) AS [Total Revenue (RM)], I.Stock AS [Remaining Stock]
                                FROM [TransactionDetail] TD
                                JOIN [Inventory] I ON TD.ItemID = I.ItemID
                                JOIN [Transaction] T ON TD.TransactionID = T.TransactionID
                                WHERE T.TransactionDate BETWEEN @from AND @to
                                GROUP BY I.ItemID, I.ItemName, I.Category, I.Price, I.Stock
                                ORDER BY SUM(TD.Quantity) DESC", conn);
                            da.SelectCommand.Parameters.AddWithValue("@from", from.Date);
                            da.SelectCommand.Parameters.AddWithValue("@to", to.Date.AddDays(1));
                            da.Fill(dt);
                            decimal topRev = dt.AsEnumerable().Sum(r => Convert.ToDecimal(r["Total Revenue (RM)"]));
                            summary = $"Period: {from:dd/MM/yyyy} - {to:dd/MM/yyyy}  |  Products Sold: {dt.Rows.Count}  |  Total Revenue: RM {topRev:N2}";
                            break;

                        case "Maintenance Report":
                            da = new SqlDataAdapter(@"
                                SELECT MaintenanceID AS [Maint. ID], ItemID AS [Item ID], TechnicianID AS [Technician ID],
                                       IssueDescription AS [Issue], FORMAT(MaintenanceDate,'dd/MM/yyyy') AS [Date], Status
                                FROM [Maintenance] WHERE MaintenanceDate BETWEEN @from AND @to ORDER BY MaintenanceDate DESC", conn);
                            da.SelectCommand.Parameters.AddWithValue("@from", from.Date);
                            da.SelectCommand.Parameters.AddWithValue("@to", to.Date.AddDays(1));
                            da.Fill(dt);
                            int mPending = dt.Select("Status='Pending'").Length;
                            int mDone = dt.Select("Status='Completed'").Length;
                            summary = $"Period: {from:dd/MM/yyyy} - {to:dd/MM/yyyy}  |  Total Records: {dt.Rows.Count}  |  Pending: {mPending}  |  Completed: {mDone}";
                            break;

                        case "Promotion Report":
                            da = new SqlDataAdapter(@"
                                SELECT PromotionID AS [Promo ID], PromotionName AS [Promotion Name], Discount AS [Discount (%)],
                                       FORMAT(StartDate,'dd/MM/yyyy') AS [Start Date], FORMAT(EndDate,'dd/MM/yyyy') AS [End Date], Status,
                                       CASE WHEN EndDate >= GETDATE() THEN CAST(DATEDIFF(DAY,GETDATE(),EndDate) AS VARCHAR)+' days left' ELSE 'EXPIRED' END AS [Validity]
                                FROM [Promotion] ORDER BY EndDate DESC", conn);
                            da.Fill(dt);
                            int pActive = dt.Select("Validity<>'EXPIRED'").Length;
                            summary = $"Total Promotions: {dt.Rows.Count}  |  Active: {pActive}  |  Expired: {dt.Rows.Count - pActive}";
                            break;

                        case "User Activity Report":
                            da = new SqlDataAdapter(@"
                                SELECT U.UserID AS [User ID], U.Username, U.Role, U.Email,
                                       COUNT(T.TransactionID) AS [Total Transactions], ISNULL(SUM(T.TotalAmount),0) AS [Total Spent (RM)]
                                FROM [User] U
                                LEFT JOIN [Transaction] T ON U.UserID = T.UserID AND T.TransactionDate BETWEEN @from AND @to
                                GROUP BY U.UserID, U.Username, U.Role, U.Email ORDER BY [Total Spent (RM)] DESC", conn);
                            da.SelectCommand.Parameters.AddWithValue("@from", from.Date);
                            da.SelectCommand.Parameters.AddWithValue("@to", to.Date.AddDays(1));
                            da.Fill(dt);
                            summary = $"Period: {from:dd/MM/yyyy} - {to:dd/MM/yyyy}  |  Total Registered Users: {dt.Rows.Count}";
                            break;
                    }

                    dgvReport.DataSource = dt;
                    foreach (DataGridViewRow row in dgvReport.Rows)
                        if (!row.IsNewRow && row.Index % 2 == 0)
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 248);

                    if (lblReportSummary != null)
                        lblReportSummary.Text = "📋 " + summary;
                }
            }
            catch (Exception ex) { MessageBox.Show("Report error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnExportPDF_Click(object sender, EventArgs e)
        {
            if (dgvReport == null || dgvReport.Rows.Count == 0) { MessageBox.Show("Generate a report first!", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF Files (*.pdf)|*.pdf", FileName = currentReportTitle.Replace(" ", "_") + "_" + DateTime.Now.ToString("yyyyMMdd"), Title = "Save Report as PDF" };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                BuildPrintLines();
                PrintDocument pd = new PrintDocument { DocumentName = currentReportTitle };
                pd.PrintPage += PrintPage_Handler;
                pd.DefaultPageSettings.Landscape = true;
                pd.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(40, 40, 40, 40);


                bool pdfPrinterFound = false;
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    if (printer.ToLower().Contains("pdf") || printer.ToLower().Contains("print to pdf"))
                    {
                        pd.PrinterSettings.PrinterName = printer;
                        pd.PrinterSettings.PrintToFile = true;
                        pd.PrinterSettings.PrintFileName = sfd.FileName;
                        pdfPrinterFound = true; break;
                    }
                }
                if (pdfPrinterFound)
                {
                    printLineIndex = 0; pd.Print();
                    MessageBox.Show("PDF exported successfully!\n\nSaved to:\n" + sfd.FileName, "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ExportAsFormattedText(sfd.FileName.Replace(".pdf", "_report.txt"));
                    MessageBox.Show("PDF printer not found. Report saved as formatted text file instead.\n\nSaved to: " + sfd.FileName.Replace(".pdf", "_report.txt"), "Saved as Text", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show("Export error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BuildPrintLines()
        {
            printLines.Clear();
            printLines.Add("=".PadRight(120, '='));
            printLines.Add("KIOSK ADMINISTRATION SYSTEM");
            printLines.Add(currentReportTitle.ToUpper());
            printLines.Add("Generated: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            printLines.Add("Period: " + dtpReportFrom.Value.ToString("dd/MM/yyyy") + " - " + dtpReportTo.Value.ToString("dd/MM/yyyy"));
            printLines.Add("=".PadRight(120, '='));
            printLines.Add("");
            if (lblReportSummary != null) printLines.Add(lblReportSummary.Text.Replace("📋 ", "SUMMARY: "));
            printLines.Add("");

            if (dgvReport.Columns.Count > 0)
            {
                StringBuilder header = new StringBuilder();
                foreach (DataGridViewColumn col in dgvReport.Columns)
                    header.Append(col.HeaderText.PadRight(18).Substring(0, Math.Min(18, col.HeaderText.Length)).PadRight(18) + " | ");
                printLines.Add(header.ToString());
                printLines.Add("-".PadRight(120, '-'));

                foreach (DataGridViewRow row in dgvReport.Rows)
                {
                    if (row.IsNewRow) continue;
                    StringBuilder line = new StringBuilder();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        string val = cell.Value?.ToString() ?? "";
                        if (val.Length > 17) val = val.Substring(0, 14) + "...";
                        line.Append(val.PadRight(18) + " | ");
                    }
                    printLines.Add(line.ToString());
                }
            }
            printLines.Add(""); printLines.Add("-".PadRight(120, '-'));
            printLines.Add("END OF REPORT - Total Records: " + dgvReport.Rows.Count);
        }

        private void PrintPage_Handler(object sender, PrintPageEventArgs e)
        {
            Font titleFont = new Font("Courier New", 14, FontStyle.Bold);
            Font headerFont = new Font("Courier New", 9, FontStyle.Bold);
            Font normalFont = new Font("Courier New", 8, FontStyle.Regular);
            float x = e.MarginBounds.Left, y = e.MarginBounds.Top;
            float lineHeight = normalFont.GetHeight(e.Graphics) + 2, maxY = e.MarginBounds.Bottom;

            while (printLineIndex < printLines.Count)
            {
                if (y + lineHeight > maxY) { e.HasMorePages = true; return; }
                string line = printLines[printLineIndex];
                Font f = normalFont;
                if (printLineIndex <= 1) f = titleFont;
                else if (printLineIndex <= 4) f = headerFont;
                else if (line.StartsWith("-") || line.StartsWith("=")) f = normalFont;
                else if (line.Contains(" | ") && printLineIndex < 10) f = headerFont;

                e.Graphics.DrawString(line, f, Brushes.Black, x, y);
                y += f == titleFont ? lineHeight * 1.5f : lineHeight;
                printLineIndex++;
            }
            e.HasMorePages = false;
        }

        private void ExportAsFormattedText(string filePath)
        {
            BuildPrintLines();
            File.WriteAllLines(filePath, printLines, Encoding.UTF8);
        }

        private void BtnExportCSV_Click(object sender, EventArgs e)
        {
            if (dgvReport == null || dgvReport.Rows.Count == 0) { MessageBox.Show("Generate a report first!", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV Files (*.csv)|*.csv", FileName = currentReportTitle.Replace(" ", "_") + "_" + DateTime.Now.ToString("yyyyMMdd") + ".csv", Title = "Export Report as CSV" };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("KIOSK ADMINISTRATION SYSTEM - " + currentReportTitle.ToUpper());
                sb.AppendLine("Generated:," + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                sb.AppendLine("Period:," + dtpReportFrom.Value.ToString("dd/MM/yyyy") + "," + dtpReportTo.Value.ToString("dd/MM/yyyy"));
                sb.AppendLine();

                List<string> headers = new List<string>();
                foreach (DataGridViewColumn col in dgvReport.Columns) headers.Add("\"" + col.HeaderText + "\"");
                sb.AppendLine(string.Join(",", headers));

                foreach (DataGridViewRow row in dgvReport.Rows)
                {
                    if (row.IsNewRow) continue;
                    List<string> cells = new List<string>();
                    foreach (DataGridViewCell cell in row.Cells)
                        cells.Add("\"" + (cell.Value?.ToString() ?? "").Replace("\"", "'") + "\"");
                    sb.AppendLine(string.Join(",", cells));
                }
                sb.AppendLine(); sb.AppendLine("Total Records:," + dgvReport.Rows.Count);
                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("CSV exported successfully!\n\nSaved to:\n" + sfd.FileName, "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("CSV export error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ============================================================
        //  GLOBAL DATABASE HELPERS & DESIGN LOGOUT
        // ============================================================
        private void ExecuteCRUD(SqlCommand cmd, string successMsg, Action callback)
        {
            cmd.Connection = connection;
            try
            {
                connection.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    MessageBox.Show(successMsg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    callback?.Invoke();
                }
                else
                {
                    MessageBox.Show("No records affected. Check the ID entered.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { if (connection.State == ConnectionState.Open) connection.Close(); }
        }

        private Control FindControlRecursive(Control parent, string name)
        {
            foreach (Control c in parent.Controls)
            {
                if (c.Name == name) return c;
                Control found = FindControlRecursive(c, name);
                if (found != null) return found;
            }
            return null;
        }

        private void SetLabel(string name, string value)
        {
            Control c = FindControlRecursive(this, name);
            if (c != null) c.Text = value;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            refreshTimer.Stop();
            FormStart mainForm = new FormStart();
            mainForm.Show();
            this.Hide();
        }

        private void inventoryBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.inventoryBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dataSet1);
        }
    }
}