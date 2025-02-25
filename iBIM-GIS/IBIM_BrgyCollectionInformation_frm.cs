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
    public partial class IBIM_BrgyCollectionInformation_frm : Form
    {
        private string accountName;
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        public IBIM_BrgyCollectionInformation_frm(string accountName)
        {
            InitializeComponent();
            this.accountName = accountName; // Assign the passed accountName
            txtCollectedBy.Text = accountName; // Set the txtCollectedBy to the accountName
            LoadResidentProfileData();
            dgvResidentProfile.CellClick += dgvResidentProfile_CellClick;
            txtorganization.Enabled = false;
            txtresidentname.Enabled = false;
            txtHHNO.Enabled = false;
            txtorganization.Text = "Disabled"; // Display 'Disabled' text
            txtresidentname.Text = "Disabled"; // Display 'Disabled' text
            txtHHNO.Text = "Disabled"; // Display 'Disabled' text
            dgvpayment.CellClick += dgvpayment_CellClick;
        }
        private void LoadResidentProfileData(string searchQuery = "")
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT `HouseHoldNo`, `ResidentName` FROM tblresidentprofiling";

                    // Modify the query if a search term is provided
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        query += " WHERE `HouseHoldNo` LIKE @Search OR `ResidentName` LIKE @Search";
                    }
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchQuery))
                        {
                            cmd.Parameters.AddWithValue("@Search", "%" + searchQuery + "%");
                        }
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dgvResidentProfile.DataSource = dataTable;
                        dgvResidentProfile.Columns["HouseHoldNo"].HeaderText = "HH NO.";
                        dgvResidentProfile.Columns["ResidentName"].HeaderText = "Resident Name";
                        dgvResidentProfile.Columns["HouseHoldNo"].Width = 100; // Example width, adjust as needed
                        dgvResidentProfile.Columns["ResidentName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Fill remaining space
                                                                                                                       // Set row height and header height
                        dgvResidentProfile.RowTemplate.Height = 35; // Adjust height as needed
                        dgvResidentProfile.ColumnHeadersHeight = 40; // Adjust height as needed
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        private void txtsearch_TextChanged(object sender, EventArgs e)
        {
            LoadResidentProfileData(txtsearch.Text.Trim());
        }
        private void dgvResidentProfile_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvResidentProfile.Rows[e.RowIndex];
                string selectedHHNo = selectedRow.Cells["HouseHoldNo"].Value.ToString();
                string selectedResidentName = selectedRow.Cells["ResidentName"].Value.ToString();
                txtHHNO.Text = selectedHHNo;
                txtresidentname.Text = selectedResidentName;
            }
        }
        private void LoadParticulars()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT `ParticularsCategory` FROM tblparticulars";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    cbParticulars.Items.Clear();
                    while (reader.Read())
                    {
                        cbParticulars.Items.Add(reader["ParticularsCategory"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void IBIM_BrgyPayment_frm_Load(object sender, EventArgs e)
        
        {
            LoadParticulars();
            LoadPaymentData();
        }
        private void LoadPaymentData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT TransactionID, TransactionType, ResidentName, OrganizationName,TransactionStatus FROM tblinflowsinformation";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvpayment.DataSource = dataTable;
                    dgvpayment.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvpayment.RowTemplate.Height = 35; // Adjust height as needed
                    dgvpayment.ColumnHeadersHeight = 40; // Adjust height as needed
                    dgvpayment.Columns["TransactionID"].HeaderText = "Transaction ID";
                    dgvpayment.Columns["TransactionType"].HeaderText = "Transaction Type";
                    dgvpayment.Columns["ResidentName"].HeaderText = "Resident Name";
                    dgvpayment.Columns["OrganizationName"].HeaderText = "Organization Name";
                    dgvpayment.Columns["TransactionStatus"].HeaderText = "Status";
                    dgvpayment.Columns["TransactionID"].Width = 100;
                    dgvpayment.Columns["TransactionType"].Width = 150;
                    dgvpayment.Columns["ResidentName"].Width = 150;
                    dgvpayment.Columns["OrganizationName"].Width = 150;
                    dgvpayment.Columns["TransactionStatus"].Width = 150;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading payment data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Helper method to retrieve categories
        private List<string> GetCategories(MySqlConnection connection, string query)
        {
            List<string> categories = new List<string>();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    categories.Add(reader["ParticularsCategory"].ToString());
            }
            return categories;
        }

        private void UpdateCashInflows()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        // Get unique categories with the latest TotalAmount based on DateofTransaction
                        string latestAmountQuery = @"
                SELECT ParticularsCategory, TotalAmount, DateofTransaction 
                FROM tblinflowsinformation 
                WHERE TransactionStatus = 'Completed' AND DateofTransaction = 
                (SELECT MAX(DateofTransaction) 
                 FROM tblinflowsinformation AS sub 
                 WHERE sub.ParticularsCategory = tblinflowsinformation.ParticularsCategory 
                 AND sub.TransactionStatus = 'Completed')";

                        using (MySqlCommand latestAmountCmd = new MySqlCommand(latestAmountQuery, connection, transaction))
                        using (MySqlDataReader reader = latestAmountCmd.ExecuteReader())
                        {
                            Dictionary<string, (decimal TotalAmount, DateTime DateofTransaction)> latestEntries = new Dictionary<string, (decimal, DateTime)>();

                            while (reader.Read())
                            {
                                string particularsCategory = reader["ParticularsCategory"].ToString();
                                decimal totalAmount = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                                DateTime dateOfTransaction = reader.GetDateTime(2);
                                latestEntries[particularsCategory] = (totalAmount, dateOfTransaction);
                            }
                            reader.Close();

                            foreach (var entry in latestEntries)
                            {
                                string particularsCategory = entry.Key;
                                decimal latestTotalAmount = entry.Value.TotalAmount;
                                DateTime latestDateofTransaction = entry.Value.DateofTransaction;

                                // Check if the ParticularsCategory already exists in tblcashinflows
                                string checkExistingCategoryQuery = @"
                        SELECT GrandTotal, DateofTransaction FROM tblcashinflows 
                        WHERE ParticularsCategory = @ParticularsCategory 
                        ORDER BY DateofTransaction DESC LIMIT 1";

                                decimal currentGrandTotal = 0;
                                DateTime lastRecordedDate = DateTime.MinValue;
                                bool categoryExists = false;

                                using (MySqlCommand checkCmd = new MySqlCommand(checkExistingCategoryQuery, connection, transaction))
                                {
                                    checkCmd.Parameters.AddWithValue("@ParticularsCategory", particularsCategory);
                                    using (MySqlDataReader existingReader = checkCmd.ExecuteReader())
                                    {
                                        if (existingReader.Read())
                                        {
                                            currentGrandTotal = existingReader.GetDecimal(0);
                                            lastRecordedDate = existingReader.GetDateTime(1);
                                            categoryExists = true;
                                        }
                                    }
                                }

                                // If category exists and the transaction date is newer, add to the GrandTotal
                                if (categoryExists && latestDateofTransaction > lastRecordedDate)
                                {
                                    string updateQuery = @"
                            UPDATE tblcashinflows 
                            SET GrandTotal = GrandTotal + @LatestTotalAmount,
                                NumberofRemitter = NumberofRemitter + 1,
                                DateofTransaction = @DateofTransaction 
                            WHERE ParticularsCategory = @ParticularsCategory";

                                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection, transaction))
                                    {
                                        updateCmd.Parameters.AddWithValue("@LatestTotalAmount", latestTotalAmount);
                                        updateCmd.Parameters.AddWithValue("@DateofTransaction", latestDateofTransaction);
                                        updateCmd.Parameters.AddWithValue("@ParticularsCategory", particularsCategory);
                                        updateCmd.ExecuteNonQuery();
                                    }
                                }
                                else if (!categoryExists)
                                {
                                    // Insert new record if the category doesn't exist
                                    string generatedInflowsID = GenerateInflowsID(particularsCategory);

                                    string insertQuery = @"
                            INSERT INTO tblcashinflows (InflowsID, ParticularsCategory, GrandTotal, NumberofRemitter, DateofTransaction) 
                            VALUES (@InflowsID, @ParticularsCategory, @GrandTotal, 1, @DateofTransaction)";

                                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection, transaction))
                                    {
                                        insertCmd.Parameters.AddWithValue("@InflowsID", generatedInflowsID);
                                        insertCmd.Parameters.AddWithValue("@ParticularsCategory", particularsCategory);
                                        insertCmd.Parameters.AddWithValue("@GrandTotal", latestTotalAmount);
                                        insertCmd.Parameters.AddWithValue("@DateofTransaction", latestDateofTransaction);
                                        insertCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating cash inflows: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DateTime GetLatestTransactionDate(MySqlConnection connection, string category)
        {
            // Get the latest DateofTransaction for the given ParticularsCategory where TransactionStatus is 'Completed'
            string query = "SELECT DateofTransaction FROM tblinflowsinformation WHERE ParticularsCategory = @Category AND TransactionStatus = 'Completed' ORDER BY DateofTransaction DESC LIMIT 1";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Category", category);
            return Convert.ToDateTime(cmd.ExecuteScalar() ?? DateTime.MinValue);
        }

        private DateTime GetLastCashInflowsUpdateDate(MySqlConnection connection, string category)
        {
            // Retrieve the last update timestamp for the ParticularsCategory in tblcashinflows
            string query = "SELECT MAX(DateofTransaction) FROM tblcashinflows WHERE ParticularsCategory = @Category";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Category", category);
            object result = cmd.ExecuteScalar();
            return result == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(result);
        }

        private decimal GetLatestAmountTotal(MySqlConnection connection, string category)
        {
            // Get the latest TotalAmount for the given ParticularsCategory where TransactionStatus is 'Completed'
            string query = "SELECT TotalAmount FROM tblinflowsinformation WHERE ParticularsCategory = @Category AND TransactionStatus = 'Completed' ORDER BY DateofTransaction DESC LIMIT 1";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Category", category);
            return Convert.ToDecimal(cmd.ExecuteScalar() ?? 0);
        }

        private decimal GetCurrentGrandTotal(MySqlConnection connection, string category)
        {
            // Retrieve the current GrandTotal for the ParticularsCategory in tblcashinflows
            string query = "SELECT GrandTotal FROM tblcashinflows WHERE ParticularsCategory = @Category";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Category", category);
            return Convert.ToDecimal(cmd.ExecuteScalar() ?? 0);
        }

        private int GetRemitterCount(MySqlConnection connection, string category)
        {
            // Count the number of remitters for completed transactions in this category
            string query = "SELECT COUNT(*) FROM tblinflowsinformation WHERE ParticularsCategory = @Category AND TransactionStatus = 'Completed'";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Category", category);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private bool RecordExists(MySqlConnection connection, string category)
        {
            // Ensure that a record exists for the specific ParticularsCategory in tblcashinflows
            string query = "SELECT COUNT(*) FROM tblcashinflows WHERE ParticularsCategory = @Category";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Category", category);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        private void UpdateCashInflowsRecord(MySqlConnection connection, string category, decimal grandTotal, int remitters)
        {
            // Update the GrandTotal and NumberofRemitter for the ParticularsCategory
            string query = "UPDATE tblcashinflows SET GrandTotal = @GrandTotal, NumberofRemitter = @Remitters WHERE ParticularsCategory = @Category";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@GrandTotal", grandTotal);
            cmd.Parameters.AddWithValue("@Remitters", remitters);
            cmd.Parameters.AddWithValue("@Category", category);
            cmd.ExecuteNonQuery();
        }

        private void InsertCashInflowsRecord(MySqlConnection connection, string category, decimal grandTotal, int remitters)
        {
            // Insert a new record with the GrandTotal and NumberofRemitter for the ParticularsCategory
            string inflowsID = GenerateInflowsID(category); // Assuming GenerateInflowsID is a valid method
            string query = "INSERT INTO tblcashinflows (InflowsID, ParticularsCategory, GrandTotal, NumberofRemitter) VALUES (@ID, @Category, @GrandTotal, @Remitters)";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ID", inflowsID);
            cmd.Parameters.AddWithValue("@Category", category);
            cmd.Parameters.AddWithValue("@GrandTotal", grandTotal);
            cmd.Parameters.AddWithValue("@Remitters", remitters);
            cmd.ExecuteNonQuery();
        }
        private string GenerateInflowsID(string category)
        {
            string datePart = DateTime.Now.ToString("yyyyMMddHHmmss");
            Random random = new Random();
            int randomPart = random.Next(1000, 9999);

            return $"INF-{category}-{datePart}-{randomPart}";
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string generatedInflowsID = GenerateInflowsID(cbParticulars.SelectedItem.ToString());
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO tblinflowsinformation " +
                                   "(TransactionID, TransactionType, ResidentName, HouseHoldNo, OrganizationName, Purpose, ParticularsCategory, TotalAmount, CollectedBy, Attachment, TransactionStatus, DateofTransaction) " +
                                   "VALUES (@TransactionID, @TransactionType, @ResidentName, @HouseHoldNo, @OrganizationName, @Purpose, @ParticularsCategory, @TotalAmount, @CollectedBy, @Attachment, @TransactionStatus, @DateofTransaction)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransactionID", generatedInflowsID);
                        cmd.Parameters.AddWithValue("@TransactionType", cbTransactionType.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@ResidentName", txtresidentname.Text);
                        cmd.Parameters.AddWithValue("@HouseHoldNo", txtHHNO.Text);
                        cmd.Parameters.AddWithValue("@OrganizationName", txtorganization.Text);
                        cmd.Parameters.AddWithValue("@Purpose", txtpurpose.Text);
                        cmd.Parameters.AddWithValue("@ParticularsCategory", cbParticulars.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@TotalAmount", decimal.Parse(txtAmount.Text)); // Assuming TotalAmount is a decimal/number field
                        cmd.Parameters.AddWithValue("@CollectedBy", txtCollectedBy.Text);
                        cmd.Parameters.AddWithValue("@Attachment", Convert.FromBase64String(txtattachment.Text)); // Assuming attachment is a longblob
                        cmd.Parameters.AddWithValue("@TransactionStatus", cbTransactionStatus.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@DateofTransaction", dateTimePickertransaction.Value);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Transaction information saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Call UpdateCashInflows to refresh tblcashinflows
                        UpdateCashInflows();

                        LoadPaymentData();
                        btnClear_Click(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving the transaction: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateTransID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMddHHmmss");
            Random random = new Random();
            int randomPart = random.Next(1000, 9999);

            return $"TRANSs -{datePart}-{randomPart}";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtHHNO.Clear();
            txtresidentname.Clear();
            txtAmount.Clear();
            txtattachment.Clear();
            txtpurpose.Clear();
            cbParticulars.SelectedIndex = -1;
            dgvResidentProfile.ClearSelection();
            txtsearch.Clear();
            txtseachpayment.Clear();
            LoadResidentProfileData();
            cbTransactionType.SelectedIndex = -1;
            cbTransactionStatus.SelectedIndex = -1;
            txtorganization.Clear();
        }
        private void cbTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTransactionType.SelectedItem != null)
            {
                string selectedType = cbTransactionType.SelectedItem.ToString();

                if (selectedType == "Resident")
                {
                    txtorganization.Enabled = false;
                    txtorganization.Text = "Disabled"; 
                    txtresidentname.Enabled = true;
                    txtresidentname.Clear(); // Clear any existing placeholder or data
                    txtHHNO.Enabled = true;
                    txtHHNO.Clear(); // Clear any existing placeholder or data
                }
                else if (selectedType == "Organization")
                {
                    txtorganization.Enabled = true;
                    txtorganization.Clear(); // Clear any placeholder or previous data
                    txtresidentname.Enabled = false;
                    txtresidentname.Text = "Disabled"; // Display 'Disabled' text
                    txtHHNO.Enabled = false;
                    txtHHNO.Text = "Disabled"; // Display 'Disabled' text
                }
            }
        }
        private void btnuploadreceipt_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Title = "Select an Image"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Image image = Image.FromFile(openFileDialog.FileName))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, image.RawFormat);
                        byte[] imageBytes = ms.ToArray();
                        const int maxSize = 16777215; // LONGBLOB max size (~16MB)
                        if (imageBytes.Length > maxSize)
                        {
                            MessageBox.Show("The selected image is too large. Please choose a smaller one.", "Image Size Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        txtattachment.Text = Convert.ToBase64String(imageBytes);
                    }
                }
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE tblinflowsinformation SET " +
                                   "TransactionType = @TransactionType, ResidentName = @ResidentName, " +
                                   "HouseHoldNo = @HouseHoldNo, OrganizationName = @OrganizationName, " +
                                   "Purpose = @Purpose, ParticularsCategory = @ParticularsCategory, " +
                                   "TotalAmount = @TotalAmount, CollectedBy = @CollectedBy, " +
                                   "Attachment = @Attachment, TransactionStatus = @TransactionStatus, " +
                                   "DateofTransaction = @DateofTransaction WHERE TransactionID = @TransactionID";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TransactionID", dgvpayment.CurrentRow.Cells["TransactionID"].Value.ToString());
                        cmd.Parameters.AddWithValue("@TransactionType", cbTransactionType.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@ResidentName", txtresidentname.Text);
                        cmd.Parameters.AddWithValue("@HouseHoldNo", txtHHNO.Text);
                        cmd.Parameters.AddWithValue("@OrganizationName", txtorganization.Text);
                        cmd.Parameters.AddWithValue("@Purpose", txtpurpose.Text);
                        cmd.Parameters.AddWithValue("@ParticularsCategory", cbParticulars.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@TotalAmount", decimal.Parse(txtAmount.Text));
                        cmd.Parameters.AddWithValue("@CollectedBy", txtCollectedBy.Text);
                        cmd.Parameters.AddWithValue("@Attachment", Convert.FromBase64String(txtattachment.Text));
                        cmd.Parameters.AddWithValue("@TransactionStatus", cbTransactionStatus.SelectedItem?.ToString());
                        cmd.Parameters.AddWithValue("@DateofTransaction", dateTimePickertransaction.Value);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPaymentData();
                        UpdateCashInflows();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating the record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvpayment.CurrentRow == null)
            {
                MessageBox.Show("Please select a record to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM tblinflowsinformation WHERE TransactionID = @TransactionID";
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@TransactionID", dgvpayment.CurrentRow.Cells["TransactionID"].Value.ToString());
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Record deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadPaymentData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while deleting the record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void dgvpayment_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvpayment.Rows[e.RowIndex];
                string selectedInflowsID = selectedRow.Cells["TransactionID"].Value.ToString();
                LoadRecordByInflowsID(selectedInflowsID);
            }
        }
        private void LoadRecordByInflowsID(string inflowsID)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT TransactionType, ResidentName, HouseHoldNo, OrganizationName, Purpose, ParticularsCategory, TotalAmount, CollectedBy, Attachment, TransactionStatus, DateofTransaction FROM tblinflowsinformation WHERE TransactionID = @TransactionID";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@TransactionID", inflowsID);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        cbTransactionType.Text = reader["TransactionType"].ToString();
                        txtresidentname.Text = reader["ResidentName"].ToString();
                        txtHHNO.Text = reader["HouseHoldNo"].ToString();
                        txtorganization.Text = reader["OrganizationName"].ToString();
                        txtpurpose.Text = reader["Purpose"].ToString();
                        cbParticulars.Text = reader["ParticularsCategory"].ToString();
                        txtAmount.Text = reader["TotalAmount"].ToString();
                        txtCollectedBy.Text = reader["CollectedBy"].ToString();
                        txtattachment.Text = Convert.ToBase64String((byte[])reader["Attachment"]);
                        cbTransactionStatus.Text = reader["TransactionStatus"].ToString();
                        dateTimePickertransaction.Value = Convert.ToDateTime(reader["DateofTransaction"]);
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading record details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtseachpayment_TextChanged(object sender, EventArgs e)
        {
            LoadPaymentData(txtseachpayment.Text.Trim());
        }
        private void LoadPaymentData(string searchQuery = "")
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT TransactionID, TransactionType, ResidentName, OrganizationName, TransactionStatus FROM tblinflowsinformation";
                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        query += " WHERE TransactionType LIKE @Search OR ResidentName LIKE @Search OR OrganizationName LIKE @Search";
                    }
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchQuery))
                        {
                            cmd.Parameters.AddWithValue("@Search", "%" + searchQuery + "%");
                        }
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dgvpayment.DataSource = dataTable;
                        dgvpayment.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dgvpayment.Columns["TransactionID"].HeaderText = "Transaction ID";
                        dgvpayment.Columns["TransactionType"].HeaderText = "Transaction Type";
                        dgvpayment.Columns["ResidentName"].HeaderText = "Resident Name";
                        dgvpayment.Columns["OrganizationName"].HeaderText = "Organization Name";
                        dgvpayment.Columns["TransactionStatus"].HeaderText = "Transaction Status";
                        dgvpayment.Columns["InflowsID"].Width = 100;
                        dgvpayment.Columns["TransactionType"].Width = 150;
                        dgvpayment.Columns["ResidentName"].Width = 150;
                        dgvpayment.Columns["OrganizationName"].Width = 150;
                        dgvpayment.Columns["TransactionStatus"].Width = 150;
                        dgvpayment.RowTemplate.Height = 35; // Adjust height as needed
                        dgvpayment.ColumnHeadersHeight = 40; // Adjust height as needed
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading payment data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
  