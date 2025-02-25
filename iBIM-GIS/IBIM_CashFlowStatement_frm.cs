using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Microsoft.Reporting.WinForms;
using System.IO;
namespace iBIM_GIS
{
    public partial class IBIM_CashFlowStatement_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        public IBIM_CashFlowStatement_frm()
        {
            InitializeComponent();
            dgvinflows.CellClick += dgvinflows_CellClick;
            this.KeyPreview = true;
            this.KeyDown += IBIM_CashFlowStatement_frm_KeyDown;
            dgvLiquidation.CellClick += dgvLiquidation_CellClick;
        }
        private void dgvLiquidation_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && dgvLiquidation.Columns[e.ColumnIndex].Name == "AttachmentImage" && e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvLiquidation.Rows[e.RowIndex];
                Image image = selectedRow.Cells["AttachmentImage"].Value as Image;

                if (image != null)
                {
                    ImagePopupForm popup = new ImagePopupForm(image);
                    popup.StartPosition = FormStartPosition.CenterParent;
                    popup.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No image available for this record.", "No Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void IBIM_CashFlowStatement_frm_Load(object sender, EventArgs e)
        {
            panelLiquidation.Visible = false; LoadCashInflowsData(); // Load data into dgvinflows
            LoadLiquidationData(); 
            RefreshData(); 
            DisplayGrandTotal();
            DisplayTotalExpenses();
            ListColor1();
            LoadCashoutflows();
            CustomizeDataGridView();
        }
        private void CustomizeDataGridView()
        {
            dgvinflows.EnableHeadersVisualStyles = false;
            dgvinflows.ColumnHeadersDefaultCellStyle.Font = new Font("Impact", 14, FontStyle.Regular);
        }

        private void DisplayTotalExpenses()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT SUM(Amount) FROM tblLiquidation";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        object result = cmd.ExecuteScalar();
                        decimal grandTotal = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                        lblGrandTotalOutflow.Text = "₱" + grandTotal.ToString("N2"); 
                        lblTotalExapences.Text = "₱" + grandTotal.ToString("N2"); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching the GrandTotal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DisplayGrandTotal()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT SUM(GrandTotal) FROM tblcashinflows"; // Modify the query as needed
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        object result = cmd.ExecuteScalar();
                        decimal grandTotal = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                        lblCI_GrandTotal.Text = "₱" + grandTotal.ToString("N2"); // "N2" formats to two decimal places
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching the GrandTotal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadLiquidationData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ORDate,LiquidationID, Particulars, Attachment, Amount FROM tblLiquidation";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataTable.Columns.Add("AttachmentImage", typeof(Image));
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (row["Attachment"] != DBNull.Value)
                        {
                            byte[] imageBytes = (byte[])row["Attachment"];
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                row["AttachmentImage"] = Image.FromStream(ms);
                            }
                        }
                    }
                    dataTable.Columns.Remove("Attachment");
                    dgvLiquidation.DataSource = dataTable;
                    dgvLiquidation.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvLiquidation.Columns["ORDate"].HeaderText = "Official Receipt Date";
                    dgvLiquidation.Columns["LiquidationID"].HeaderText = "Liquidation ID";
                    dgvLiquidation.Columns["Particulars"].HeaderText = "Particulars";
                    dgvLiquidation.Columns["AttachmentImage"].HeaderText = "Attachment"; // Updated to display the image
                    dgvLiquidation.Columns["Amount"].HeaderText = "Amount";
                    dgvLiquidation.Columns["ORDate"].Width = 250; // 
                    dgvLiquidation.Columns["LiquidationID"].Width = 200;
                    dgvLiquidation.RowTemplate.Height = 35; // Adjust height as needed
                    dgvLiquidation.ColumnHeadersHeight = 40; // Adjust height as needed
                    DataGridViewImageColumn imageColumn = (DataGridViewImageColumn)dgvLiquidation.Columns["AttachmentImage"];
                    imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom; // Fit the image to the cell
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading liquidation data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadCashoutflows()
                {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ORDate,LiquidationID, Particulars, Amount FROM tblLiquidation";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvexpenses.DataSource = dataTable;
                    dgvexpenses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvexpenses.Columns["ORDate"].HeaderText = "Official Receipt Date";
                    dgvexpenses.Columns["LiquidationID"].HeaderText = "Liquidation ID";
                    dgvexpenses.Columns["Particulars"].HeaderText = "Particulars";
                    dgvexpenses.Columns["Amount"].HeaderText = "Amount";
                    dgvexpenses.RowTemplate.Height = 35; // Adjust height as needed
                    dgvexpenses.ColumnHeadersHeight = 40; // Adjust height as needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading liquidation data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void IBIM_CashFlowStatement_frm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                RefreshData();
            }
        }
        private void RefreshData()
        {
            LoadCashInflowsData();
        }
        private void dgvinflows_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || dgvinflows.Rows[e.RowIndex].Cells["InflowsID"].Value == null)
                {
                    MessageBox.Show("No data available for the selected row.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataGridViewRow selectedRow = dgvinflows.Rows[e.RowIndex];
                panelLiquidation.Visible = true;
                dgvinflows.Visible = false;
                groupBox1.Visible = false;
                txtInflowID.Text = selectedRow.Cells["InflowsID"].Value?.ToString() ?? "";
                txtAmtDis.Text = selectedRow.Cells["GrandTotal"].Value?.ToString() ?? "";
                string inflowsID = txtInflowID.Text;
                string particularsCategory = selectedRow.Cells["ParticularsCategory"].Value?.ToString() ?? "";
                lblcParticulars.Text = particularsCategory;

                decimal totalAmountCollected = GetTotalAmountCollected(inflowsID);
                decimal totalExpenses = GetSumOfAmountForInflowsID(inflowsID);
                txtLessExp.Text = totalExpenses.ToString("F2");

                DataTable tblImageData = new DataTable();
                DataTable tblLiquidationData = new DataTable();
                DataTable tblCashInflowsData = new DataTable();
                DataTable dsetinflowsinformation = new DataTable();
                string chairmanFullName = "";
                string treasurerFullName = "";
                decimal netBalance = 0;
                string collectedBy = "";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT FullName, Signature FROM tblofficialinfo WHERE Position = 'Chairman'", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                chairmanFullName = reader["FullName"].ToString();
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM tblImage", connection))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(tblImageData);
                    }
                    using (MySqlCommand cmd = new MySqlCommand("SELECT ParticularsCategory FROM tblcashinflows WHERE InflowsID = @InflowsID", connection))
                    {
                        cmd.Parameters.AddWithValue("@InflowsID", inflowsID);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(tblCashInflowsData);
                    }
                    using (MySqlCommand cmd = new MySqlCommand("SELECT FullName, Signature FROM tblofficialinfo WHERE Position = 'Treasurer'", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                treasurerFullName = reader["FullName"].ToString();
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand("SELECT CollectedBy FROM tblinflowsinformation WHERE ParticularsCategory = @ParticularsCategory", connection))
                    {
                        cmd.Parameters.AddWithValue("@ParticularsCategory", particularsCategory);
                        collectedBy = cmd.ExecuteScalar()?.ToString() ?? "";
                    }
                    using (MySqlCommand cmd = new MySqlCommand("SELECT GrandTotal FROM tblcashinflows WHERE InflowsID = @InflowsID", connection))
                    {
                        cmd.Parameters.AddWithValue("@InflowsID", inflowsID);
                        netBalance = Convert.ToDecimal(cmd.ExecuteScalar() ?? 0);
                    }
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM tblLiquidation WHERE InflowsID = @InflowsID", connection))
                    {
                        cmd.Parameters.AddWithValue("@InflowsID", inflowsID);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(tblLiquidationData);
                    }
                }

                DataSet reportDataSet = new DataSet();
                reportDataSet.Tables.Add(tblImageData);
                reportDataSet.Tables.Add(tblLiquidationData);
                reportDataSet.Tables.Add(tblCashInflowsData);
                reportDataSet.Tables.Add(dsetinflowsinformation);

                string reportPath = System.Environment.CurrentDirectory + "\\Report\\LiquidationReport.rdlc";
                rptLiquidationViewer.LocalReport.ReportPath = reportPath;
                rptLiquidationViewer.LocalReport.DataSources.Clear();
                rptLiquidationViewer.LocalReport.DataSources.Add(new ReportDataSource("dsetimage", tblImageData));
                rptLiquidationViewer.LocalReport.DataSources.Add(new ReportDataSource("dsetLiquidation", tblLiquidationData));
                rptLiquidationViewer.LocalReport.DataSources.Add(new ReportDataSource("dsetcashinflows", tblCashInflowsData));
                rptLiquidationViewer.LocalReport.DataSources.Add(new ReportDataSource("dsetinflowsinformation", dsetinflowsinformation));

                rptLiquidationViewer.LocalReport.SetParameters(new ReportParameter("TotalExpenses", totalExpenses.ToString("F2")));
                rptLiquidationViewer.LocalReport.SetParameters(new ReportParameter("TotalAmountCollected", (totalAmountCollected + Convert.ToDecimal(txtAmtDis.Text)).ToString("F2")));
                rptLiquidationViewer.LocalReport.SetParameters(new ReportParameter("NetBalance", netBalance.ToString("F2")));
                rptLiquidationViewer.LocalReport.SetParameters(new ReportParameter("PunongBarangay", chairmanFullName));
                rptLiquidationViewer.LocalReport.SetParameters(new ReportParameter("Treasurer", treasurerFullName));
                rptLiquidationViewer.LocalReport.SetParameters(new ReportParameter("CollectedBy", collectedBy));

                rptLiquidationViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                rptLiquidationViewer.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;
                rptLiquidationViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private decimal GetTotalAmountCollected(string inflowsID)
        {
            decimal totalAmount = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Calculate the sum of Amount from tblinflowsinformation
                using (MySqlCommand cmd = new MySqlCommand("SELECT SUM(Amount) FROM tblLiquidation WHERE InflowsID = @InflowsID", connection))
                {
                    cmd.Parameters.AddWithValue("@InflowsID", inflowsID);
                    object result = cmd.ExecuteScalar();
                    totalAmount = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
            }

            return totalAmount;
        }
        private decimal GetSumOfAmountForInflowsID(string inflowsID)
        {
            decimal totalExpenses = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT SUM(Amount) FROM tblLiquidation WHERE InflowsID = @InflowsID", connection))
                {
                    cmd.Parameters.AddWithValue("@InflowsID", inflowsID);
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                        totalExpenses = Convert.ToDecimal(result);
                }
            }
            return totalExpenses;
        }
        private void UpdateGrandTotalInCashInflows(string inflowsID, decimal updatedGrandTotal)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE tblcashinflows SET GrandTotal = @GrandTotal WHERE InflowsID = @InflowsID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@GrandTotal", updatedGrandTotal);
                        cmd.Parameters.AddWithValue("@InflowsID", inflowsID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating the GrandTotal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadCashInflowsData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM tblcashinflows";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvinflows.DataSource = dataTable;
                    dgvinflows.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    ListColor1();
                    dgvinflows.Columns["InflowsID"].HeaderText = "Inflows ID";
                    dgvinflows.Columns["ParticularsCategory"].HeaderText = "Particulars";
                    dgvinflows.Columns["GrandTotal"].HeaderText = "Grand Amount";
                    dgvinflows.Columns["NumberofRemitter"].HeaderText = "Remitters";
                    dgvinflows.Columns["InflowsID"].Width = 100; // Set width for ORDate column
                    dgvinflows.Columns["ParticularsCategory"].Width = 250; // Set width for ORDate column
                    dgvinflows.Columns["NumberofRemitter"].Width = 100; // Set width for ORDate column
                    dgvinflows.Columns["GrandTotal"].Width = 100; // Set width for ORDate column
                    dgvinflows.RowTemplate.Height = 35; // Adjust height as needed
                    dgvinflows.ColumnHeadersHeight = 40; // Adjust height as needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading cash inflows: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnClosePanel_Click(object sender, EventArgs e)
        {
            panelLiquidation.Visible = false;
            dgvinflows.Visible = true;
            groupBox1.Visible = true;
            txtInflowID.Clear();
            txtAmtDis.Clear();
            txtLessExp.Clear();
            RefreshData();
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Select an Image";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    picImageOR.Image = Image.FromFile(filePath);
                    picImageOR.SizeMode = PictureBoxSizeMode.StretchImage; // Adjust the image to fit
                    txtAttachment.Text = filePath;
                }
            }
        }
        private void btnsaveLiquidation_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInflowID.Text) || string.IsNullOrEmpty(txtAmtDis.Text) ||
                string.IsNullOrEmpty(txtLessExp.Text) || string.IsNullOrEmpty(txtORDate.Text) ||
                string.IsNullOrEmpty(txtciParticulars.Text) || string.IsNullOrEmpty(txtAmount.Text))
            {
                MessageBox.Show("Please fill in all fields before saving.", "Validation Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
             try{using (MySqlConnection connection = new MySqlConnection(connectionString)){
                    connection.Open();
                    string liquidationID = GenerateLiquidationID(connection);
                    string query = @"INSERT INTO tblLiquidation 
                            (LiquidationID, InflowsID, Amounttodisburse, LessTotalExpenses, ORDate, 
                             Particulars, Attachment, Amount) 
                            VALUES 
                            (@LiquidationID, @InflowsID, @AmtDisburse, @LessExpenses, @ORDate, 
                             @Particulars, @Attachment, @Amount)";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@LiquidationID", liquidationID);
                        cmd.Parameters.AddWithValue("@InflowsID", txtInflowID.Text);
                        cmd.Parameters.AddWithValue("@AmtDisburse", decimal.Parse(txtAmtDis.Text));
                        cmd.Parameters.AddWithValue("@LessExpenses", decimal.Parse(txtLessExp.Text));
                        cmd.Parameters.AddWithValue("@ORDate", DateTime.Parse(txtORDate.Text));
                        cmd.Parameters.AddWithValue("@Particulars", txtciParticulars.Text);
                        cmd.Parameters.AddWithValue("@Attachment", GetImageBlob(txtAttachment.Text));
                        cmd.Parameters.AddWithValue("@Amount", decimal.Parse(txtAmount.Text));
                        cmd.ExecuteNonQuery();
                        decimal amountToDeduct = decimal.Parse(txtAmount.Text);
                        string inflowsID = txtInflowID.Text;
                        DeductAmountFromCashInflows(inflowsID, amountToDeduct);
                        MessageBox.Show("Liquidation saved successfully.", "Success",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFields();
                        RefreshData(); 
                        LoadLiquidationData();
                        DisplayGrandTotal();
                        DisplayTotalExpenses();
                        LoadCashoutflows();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving liquidation: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DeductAmountFromCashInflows(string inflowsID, decimal amountToDeduct)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE tblcashinflows SET GrandTotal = GrandTotal - @Amount WHERE InflowsID = @InflowsID";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Amount", amountToDeduct);
                        cmd.Parameters.AddWithValue("@InflowsID", inflowsID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while deducting the amount: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GenerateLiquidationID(MySqlConnection connection)
        {
            string query = "SELECT COALESCE(MAX(LiquidationID), 'LQ000') FROM tblLiquidation";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                string lastID = cmd.ExecuteScalar().ToString();
                int numericPart = int.Parse(lastID.Substring(2)) + 1;
                return "LQ" + numericPart.ToString("D3");
            }
        }
        private byte[] GetImageBlob(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null; 

            return File.ReadAllBytes(filePath);
        }
        private void ClearFields()
        {
            txtInflowID.Clear();
            txtAmtDis.Clear();
            txtLessExp.Clear();
            txtORDate.Value = DateTime.Now;
            txtciParticulars.Clear();
            txtAttachment.Clear();
            txtAmount.Clear();
            picImageOR.Image = null;
        }
        private void ListColor1()
        {
            for (int i = 0; i < dgvinflows.Rows.Count; i++)
            {
                if (i % 2 == 0)
                {
                    dgvinflows.Rows[i].DefaultCellStyle.BackColor = Color.AliceBlue;
                }
                else
                {
                    dgvinflows.Rows[i].DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void dgvinflows_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}