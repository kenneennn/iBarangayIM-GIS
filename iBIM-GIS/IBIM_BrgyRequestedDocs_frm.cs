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
using System.Net.Mail;



namespace iBIM_GIS
{
    public partial class IBIM_BrgyRequestedDocs_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        public IBIM_BrgyRequestedDocs_frm()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(IBIM_BrgyRequestedDocs_frm_KeyDown);

        }
        private void IBIM_BrgyRequestedDocs_frm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5) // Check if F5 is pressed
            {
                RefreshCertRequestedData(); // Call the method to refresh data
                e.Handled = true; // Mark the event as handled
            }
        }

        private void RefreshCertRequestedData()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT RequestID, RequestedBy, ApprovalStatus FROM tblrequestcert"; // Replace with your table name and query
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable); // Fill the DataTable with data
                    dgv_certrequested.DataSource = dataTable; // Bind the DataTable to dgv_certrequested
                    dgv_certrequested.RowTemplate.Height = 35; // Adjust height as needed
                    dgv_certrequested.ColumnHeadersHeight = 40; // Adjust height as needed
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error refreshing data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection.Close(); // Ensure the connection is closed
                }
            }
        }

        private void LoadResidents()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ResidentName, PurokName FROM tblresidentprofiling"; // SQL query to select specific columns
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable); // Fill the DataTable with data

                    // Bind the data to the DataGridView
                    dgvResidents.DataSource = dataTable;

                    // Adjust DataGridView properties
                    dgvResidents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Rename column headers
                    dgvResidents.Columns["ResidentName"].HeaderText = "Resident Name";
                    dgvResidents.Columns["PurokName"].HeaderText = "Purok Name";

                    // Adjust column widths
                    dgvResidents.Columns["ResidentName"].Width = 350; // Adjust as needed
                    dgvResidents.Columns["PurokName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    // Set row height
                    dgvResidents.RowTemplate.Height = 35; // Adjust height as needed

                    // Set header row height
                    dgvResidents.ColumnHeadersHeight = 40; // Adjust height as needed
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message); // Display error message
                }
                finally
                {
                    connection.Close(); // Ensure the connection is closed
                }
            }
        }

        private void LoadReport()
        {
            // Check the selected value in the ComboBox and set the report path accordingly
            string selectedCertification = cbcertifications.SelectedItem?.ToString();
            string reportPath = string.Empty;

            if (selectedCertification == "Barangay Clearance")
            {
                reportPath = System.Environment.CurrentDirectory + "\\Report\\BrgyClearance.rdlc";
                txtpurpose.Enabled = false; // Disable txtpurpose for Barangay Clearance
            }
            else if (selectedCertification == "Certification of Residency")
            {
                reportPath = System.Environment.CurrentDirectory + "\\Report\\CertificationofResidency.rdlc";
                txtpurpose.Enabled = false; // Disable txtpurpose for Certification of Residency
            }
            else if (selectedCertification == "Certification of Indigency")
            {
                reportPath = System.Environment.CurrentDirectory + "\\Report\\CertificationofIndigency.rdlc";
                txtpurpose.Enabled = true; // Enable txtpurpose for Certification of Indigency
            }
            else
            {
                MessageBox.Show("Please select a valid certification type.");
                return;
            }

            // Retrieve the highlighted resident's name from dgvResidents
            string residentName = dgvResidents.CurrentRow?.Cells["ResidentName"].Value.ToString();

            // Initialize PurokName variable
            string purokName = string.Empty;

            // Query tblresidentprofiling for PurokName of the selected resident
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string purokQuery = "SELECT PurokName FROM tblresidentprofiling WHERE ResidentName = @ResidentName LIMIT 1";
                    MySqlCommand purokCommand = new MySqlCommand(purokQuery, connection);
                    purokCommand.Parameters.AddWithValue("@ResidentName", residentName);
                    purokName = purokCommand.ExecuteScalar()?.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving PurokName: " + ex.Message);
                }
            }

            // Query official names and positions from tblofficialinfo
            string punongBarangay = string.Empty;
            string chairmanSignatureBase64 = string.Empty;
            List<string> kagawads = new List<string>();
            string skChairman = string.Empty;
            string secretary = string.Empty;
            string treasurer = string.Empty;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Query for Punong Barangay
                    string chairmanQuery = "SELECT FullName FROM tblofficialinfo WHERE Position = 'Chairman' LIMIT 1";
                    MySqlCommand command = new MySqlCommand(chairmanQuery, connection);
                    punongBarangay = command.ExecuteScalar()?.ToString();

                    // Query for Chairman's signature
                    string chairmanSignatureQuery = "SELECT Signature FROM tblofficialinfo WHERE Position = 'Chairman' LIMIT 1";
                    command = new MySqlCommand(chairmanSignatureQuery, connection);
                    byte[] chairmanSignatureBytes = command.ExecuteScalar() as byte[];
                    if (chairmanSignatureBytes != null)
                    {
                        chairmanSignatureBase64 = Convert.ToBase64String(chairmanSignatureBytes);
                    }

                    // Query all officials in tblofficialinfo
                    string officialsQuery = "SELECT FullName, Position FROM tblofficialinfo";
                    command = new MySqlCommand(officialsQuery, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string position = reader["Position"].ToString();
                        string fullName = reader["FullName"].ToString();
                        string nameWithPosition = $"{position}: {fullName}";

                        if (position.Contains("Kagawad") && kagawads.Count < 7)
                        {
                            kagawads.Add(nameWithPosition);
                        }
                        else if (position == "SK Chairman")
                        {
                            skChairman = nameWithPosition;
                        }
                        else if (position == "Secretary")
                        {
                            secretary = nameWithPosition;
                        }
                        else if (position == "Treasurer")
                        {
                            treasurer = nameWithPosition;
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving official data: " + ex.Message);
                }
            }

            // Load data from tblimage into a DataTable
            DataTable tblImageData = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM tblimage"; // Query to select all records
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(tblImageData); // Fill the DataTable with data
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image data: " + ex.Message);
                }
            }

            // Load the selected report into the ReportViewer
            rv_Certifications.LocalReport.ReportPath = reportPath;

            // Clear existing data sources and add the new data source
            rv_Certifications.LocalReport.DataSources.Clear();
            rv_Certifications.LocalReport.DataSources.Add(new ReportDataSource("dsetimage", tblImageData));

            // Get the current date information
            string currentDay = DateTime.Now.Day.ToString();
            string currentMonth = DateTime.Now.ToString("MMMM");
            string currentYear = DateTime.Now.Year.ToString();

            // Retrieve the purpose text
            string purpose = txtpurpose.Text;

            // Set parameters for the report
            ReportParameter[] reportParameters = new ReportParameter[]
            {
        new ReportParameter("ResidentName", residentName ?? ""),
        new ReportParameter("PurokName", purokName ?? ""),
        new ReportParameter("Day", currentDay),
        new ReportParameter("Month", currentMonth),
        new ReportParameter("Year", currentYear),
        new ReportParameter("PunongBarangay", "Hon. " + (punongBarangay ?? "")),
        new ReportParameter("ChairmanSignature", chairmanSignatureBase64 ?? ""), // Adding Chairman's Signature
        new ReportParameter("txtkagawad1", kagawads.Count > 0 ? kagawads[0] : ""),
        new ReportParameter("txtkagawad2", kagawads.Count > 1 ? kagawads[1] : ""),
        new ReportParameter("txtkagawad3", kagawads.Count > 2 ? kagawads[2] : ""),
        new ReportParameter("txtkagawad4", kagawads.Count > 3 ? kagawads[3] : ""),
        new ReportParameter("txtkagawad5", kagawads.Count > 4 ? kagawads[4] : ""),
        new ReportParameter("txtkagawad6", kagawads.Count > 5 ? kagawads[5] : ""),
        new ReportParameter("txtkagawad7", kagawads.Count > 6 ? kagawads[6] : ""),
        new ReportParameter("txtSKChairman", skChairman),
        new ReportParameter("txtSecretary", secretary),
        new ReportParameter("txtTreasurer", treasurer),
        new ReportParameter("Purpose", purpose) // Adding Purpose parameter
            };

            rv_Certifications.LocalReport.SetParameters(reportParameters);

            // Refresh and set display mode
            rv_Certifications.RefreshReport();
            rv_Certifications.SetDisplayMode(DisplayMode.PrintLayout);
            rv_Certifications.ZoomMode = ZoomMode.FullPage;
        }

        private void IBIM_BrgyRequestedDocs_frm_Load(object sender, EventArgs e)
        {
            LoadResidents();
            LoadRequestedCertifications();
            dgv_certrequested.RowTemplate.Height = 35; // Adjust height as needed
            dgv_certrequested.ColumnHeadersHeight = 40; // Adjust height as needed
        }
        private void btnLoadReportViewer_Click(object sender, EventArgs e)
        {
            LoadReport();
        }
        private void cbcertifications_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check the selected certification type and enable/disable txtpurpose accordingly
            string selectedCertification = cbcertifications.SelectedItem?.ToString();
            if (selectedCertification == "Certification of Indigency")
            {
                txtpurpose.Enabled = true;
                txtpurpose.Text = "";
            }
            else if (selectedCertification == "Barangay Clearance")
            {
                txtpurpose.Enabled = false;
                txtpurpose.Text = "Disabled";
            }
            else if (selectedCertification == "Certification of Residency")
            {
                txtpurpose.Enabled = false;
                txtpurpose.Text = "Disabled";
            }
            else
            {
                txtpurpose.Enabled = true;
            }
        }
        private void txtsearchbyresidentname_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtsearchbyresidentname.Text.Trim(); // Get the search text

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Query to filter residents by ResidentName based on the search text
                    string query = "SELECT ResidentName, PurokName FROM tblresidentprofiling WHERE ResidentName LIKE @ResidentName";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ResidentName", "%" + searchValue + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable); // Fill the DataTable with the filtered data
                    dgvResidents.DataSource = dataTable; // Bind the filtered data to the DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message); // Display error message
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        private void LoadRequestedCertifications(string searchKeyword = "")
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open(); // Open the connection

                    // Modify the query to use a LIKE clause if searchKeyword is provided
                    string query = "SELECT RequestID, RequestedBy, ApprovalStatus FROM tblrequestcert";
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        query += " WHERE RequestedBy LIKE @searchKeyword";
                    }

                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Add the parameter for the LIKE search if searchKeyword is not empty
                    if (!string.IsNullOrEmpty(searchKeyword))
                    {
                        command.Parameters.AddWithValue("@searchKeyword", "%" + searchKeyword + "%");
                    }

                    // Execute the command and retrieve the data
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable); // Fill the DataTable with data

                    // Set AutoSizeColumnsMode for better visual presentation
                    dgv_certrequested.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Bind the DataTable to the DataGridView
                    dgv_certrequested.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message); // Display error message
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void dgv_certrequested_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0) // Assuming RequestedID is the first column
            {
                string requestedID = dgv_certrequested.Rows[e.RowIndex].Cells["RequestID"].Value.ToString();
                LoadCertificationDetails(requestedID);
            }
        }

        private void LoadCertificationDetails(string requestedID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM tblrequestcert WHERE RequestID = @RequestID";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RequestID", requestedID);

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        txtrequestdate.Text = reader["RequestDate"] != DBNull.Value ? reader["RequestDate"].ToString() : string.Empty;
                        txtcertificationtype.Text = reader["CertificationType"] != DBNull.Value ? reader["CertificationType"].ToString() : string.Empty;
                        txtpurposecert.Text = reader["PurposeofCertification"] != DBNull.Value ? reader["PurposeofCertification"].ToString() : string.Empty;
                        txtreqby.Text = reader["RequestedBy"] != DBNull.Value ? reader["RequestedBy"].ToString() : string.Empty;
                        txtemail.Text = reader["EmailInformation"] != DBNull.Value ? reader["EmailInformation"].ToString() : string.Empty;
                        string approvalStatus = reader["ApprovalStatus"] != DBNull.Value ? reader["ApprovalStatus"].ToString() : string.Empty;
                        cbAprroval.Text = approvalStatus; // This sets the text even if SelectedItem fails
                        dtPdate.Value = reader["ApprovalDate"] != DBNull.Value ? Convert.ToDateTime(reader["ApprovalDate"]) : DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        private void btnapprove_Click(object sender, EventArgs e)
        {
            string requestedID = dgv_certrequested.CurrentRow?.Cells["RequestID"].Value.ToString();
            string approvalStatus = cbAprroval.Text;
            DateTime approvalDate = dtPdate.Value;
            string email = txtemail.Text;
            string requestedBy = txtreqby.Text;  // Get the name of the person who requested
            string certificationType = txtcertificationtype.Text;  // Get the type of certification

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string updateQuery = "UPDATE tblrequestcert SET ApprovalStatus = @ApprovalStatus, ApprovalDate = @ApprovalDate WHERE RequestID = @RequestID";
                    MySqlCommand command = new MySqlCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@ApprovalStatus", approvalStatus);
                    command.Parameters.AddWithValue("@ApprovalDate", approvalDate);
                    command.Parameters.AddWithValue("@RequestID", requestedID);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Approval status updated successfully.");
                        if (approvalStatus == "Approved") 
                        {
                            SendEmailNotification(email, requestedBy, certificationType);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to update approval status.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void SendEmailNotification(string email, string requestedBy, string certificationType)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                using (SmtpClient smtpServer = new SmtpClient("smtp.gmail.com"))
                {
                    mail.From = new MailAddress("barangaydistrictno01@gmail.com");
                    mail.To.Add(email);
                    mail.Subject = "Certificate Ready Notification";
                    mail.Body = $"Dear {requestedBy},\n\n" +
                                $"Your '{certificationType}'has been prepared and is now ready for pickup..\n\n" +
                                "Thank you,\nBarangay Office";
                    smtpServer.Port = 587; // Port for TLS
                    smtpServer.Credentials = new System.Net.NetworkCredential("barangaydistrictno01@gmail.com", "yhco kyod icxr qhvg"); // Replace with your new app password
                    smtpServer.EnableSsl = true; // Enable SSL (TLS)
                    smtpServer.Send(mail);
                    MessageBox.Show("Email notification sent successfully.");
                }
            }
            catch (SmtpException ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtrequestdate.Text = string.Empty;
            txtcertificationtype.Text = string.Empty;
            txtpurposecert.Text = string.Empty;
            txtreqby.Text = string.Empty;
            txtemail.Text = string.Empty;
            cbAprroval.SelectedIndex = -1;
            cbcertifications.SelectedIndex = -1;
            txtsearchbyresidentname.Text = string.Empty;
            dtPdate.Value = DateTime.Now;
            dgvResidents.ClearSelection();
            dgv_certrequested.ClearSelection();
            txtpurpose.Enabled = false;
            txtpurpose.Text = string.Empty;
        }

        private void txtsearchrequester_TextChanged(object sender, EventArgs e)
        {
            LoadRequestedCertifications(txtsearchrequester.Text);
        }
        private void btnclearr_Click(object sender, EventArgs e)
        {
            cbcertifications.SelectedIndex = -1;
            txtpurpose.Text = string.Empty;
            rv_Certifications.Reset();
            rv_Certifications.LocalReport.DataSources.Clear();
            rv_Certifications.LocalReport.ReportEmbeddedResource = null;
            rv_Certifications.RefreshReport();
        }
    }
}
