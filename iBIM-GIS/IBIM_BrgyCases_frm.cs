using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MySql.Data.MySqlClient;
using System.IO;
using Microsoft.Reporting.WinForms;

namespace iBIM_GIS
{
    public partial class IBIM_BrgyCases_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        public IBIM_BrgyCases_frm()
        {
            InitializeComponent();
            // Subscribe to the MouseClick event
            gmapComplaints.MouseClick += gmapComplaints_MouseClick;
        }
        private void gmapComplaints_MouseClick(object sender, MouseEventArgs e)
        {
            // Check if the user clicked on the map
            if (e.Button == MouseButtons.Left)
            {
                // Get the latitude and longitude of the clicked point
                PointLatLng clickedPosition = gmapComplaints.FromLocalToLatLng(e.X, e.Y);

                // Set the latitude and longitude to the textboxes
                txtLatitude.Text = clickedPosition.Lat.ToString();
                txtLongitude.Text = clickedPosition.Lng.ToString();
            }
        }
        private void LoadComplaintRecords()
        {
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                        // SQL query to select specific columns
                        string query = "SELECT ComplaintID, ComplaintName, ComplaintType, StatusComplaint FROM tblcomplaint";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        DataTable dt = new DataTable();

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(dt);

                        // Check if DataTable has data
                        if (dt.Rows.Count > 0)
                        {
                            dgv_Complaint.DataSource = dt;
                            dgv_Complaint.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            // Set headers if columns exist
                            if (dgvblotterrecord.Columns.Count >= 5)
                            {
                                dgv_Complaint.Columns[0].HeaderText = "Complaint ID";
                                dgv_Complaint.Columns[1].HeaderText = "Complaint Type";
                                dgv_Complaint.Columns[2].HeaderText = "Complaint Name";
                                dgv_Complaint.Columns[3].HeaderText = "Status";
                                // Set row height
                                dgv_Complaint.RowTemplate.Height = 35; // Adjust height as needed

                                // Set header row height
                                dgv_Complaint.ColumnHeadersHeight = 40; // Adjust height as needed


                            }
                        }
                        else
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading blotter complaint: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void LoadBlotterRecords()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // SQL query to select specific columns
                    string query = "SELECT BlotterID, IncidentType, ComplainantName, Status FROM tblblotter";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    DataTable dt = new DataTable();

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    // Set row height
                    dgvblotterrecord.RowTemplate.Height = 35; // Adjust height as needed

                    // Set header row height
                    dgvblotterrecord.ColumnHeadersHeight = 40; // Adjust height as needed

                    // Check if DataTable has data
                    if (dt.Rows.Count > 0)
                    {
                        dgvblotterrecord.DataSource = dt;
                        dgvblotterrecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        // Set headers if columns exist
                        if (dgvblotterrecord.Columns.Count >= 4) // Update to 4 since we will add a button column
                        {
                            dgvblotterrecord.Columns[0].HeaderText = "Blotter ID";
                            dgvblotterrecord.Columns[1].HeaderText = "Incident";
                            dgvblotterrecord.Columns[2].HeaderText = "Complainant";
                            dgvblotterrecord.Columns[3].HeaderText = "Status";
                        }

                        // Add the button column for Print Blotter
                        if (!dgvblotterrecord.Columns.Contains("PrintBlotter"))
                        {
                            DataGridViewButtonColumn printButtonColumn = new DataGridViewButtonColumn();
                            printButtonColumn.Name = "PrintBlotter";
                            printButtonColumn.HeaderText = "Print Blotter";
                            printButtonColumn.Text = "Print";
                            printButtonColumn.UseColumnTextForButtonValue = true;
                            dgvblotterrecord.Columns.Add(printButtonColumn);
                        }
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading blotter records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadSumonRecords()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT SumonID, TxtRespondent, cbstatusSumon FROM tblsumon";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    DataTable dt = new DataTable();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    dgv_SumonRecord.DataSource = dt;
                    dgv_SumonRecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Adjust column headers
                    dgv_SumonRecord.Columns[0].HeaderText = "Summon ID";
                    dgv_SumonRecord.Columns[1].HeaderText = "Respondent";
                    dgv_SumonRecord.Columns[2].HeaderText = "Status";

                    // Add the button column at index 3 if it doesn't already exist
                    if (!dgv_SumonRecord.Columns.Contains("Action"))
                    {
                        DataGridViewButtonColumn actionColumn = new DataGridViewButtonColumn
                        {
                            Name = "Action",
                            HeaderText = "Print Summon",
                            Text = "Print Summon Invitation",
                            UseColumnTextForButtonValue = true
                        };

                        // Insert the button column at index 3
                        dgv_SumonRecord.Columns.Insert(3, actionColumn);
                    }
                }

                // Set row height
                dgv_SumonRecord.RowTemplate.Height = 35;

                // Set header row height
                dgv_SumonRecord.ColumnHeadersHeight = 40;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Summon records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtIncidentname.Clear();
            txtComplainantname.Clear();
            txtRespondentname.Clear();
            IncidentDetails.Clear();
            cbCRRelationship.SelectedIndex = -1;
            cbOfficerInCharge.SelectedIndex = -1;
            cbstatus.SelectedIndex = -1;
            cbremarks.SelectedIndex = -1;
            dateTimePickerIncidentDate.Value = DateTime.Now; // Reset date to current date
        }
        private void ClearForm()
        {
            txtIncidentname.Clear();
            txtComplainantname.Clear();
            txtRespondentname.Clear();
            IncidentDetails.Clear();
            cbCRRelationship.SelectedIndex = -1;
            cbOfficerInCharge.SelectedIndex = -1;
            cbstatus.SelectedIndex = -1;
            dateTimePickerIncidentDate.Value = DateTime.Now; // Reset date to current date
        }
        private void IBIM_BrgyBlotter_frm_Load(object sender, EventArgs e)
        {
            LoadOfficersInCharge();
            LoadOfficersInChargeSUmon();
            LoadSumonRecords();
            dgvblotterrecord.CellClick += dgvblotterrecord_CellClick;
            LoadSavedMarkers();
            LoadComplaintRecords();
            LoadBlotterRecords();
        }
        private void LoadBlotterDetails(string incidentType, string complainantName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Query to retrieve the full record based on the selected IncidentType and ComplainantName
                    string query = "SELECT * FROM tblblotter WHERE IncidentType = @IncidentType AND ComplainantName = @ComplainantName";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Bind the parameters
                    cmd.Parameters.AddWithValue("@IncidentType", incidentType);
                    cmd.Parameters.AddWithValue("@ComplainantName", complainantName);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate the form fields with the retrieved data
                            txtIncidentname.Text = reader["IncidentType"].ToString();
                            txtComplainantname.Text = reader["ComplainantName"].ToString();
                            txtRespondentname.Text = reader["RespondentName"].ToString();
                            IncidentDetails.Text = reader["IncidentDetails"].ToString();
                            cbCRRelationship.SelectedItem = reader["ComplainantNameRelationship"].ToString();
                            cbOfficerInCharge.SelectedItem = reader["OfficerInCharge"].ToString();
                            cbstatus.SelectedItem = reader["Status"].ToString();
                            dateTimePickerIncidentDate.Value = Convert.ToDateTime(reader["IncidentDate"]);
                            cbremarks.SelectedItem = reader["Remarks"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading blotter details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgvblotterrecord_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the user clicked on a valid row (not header)
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvblotterrecord.Columns["PrintBlotter"].Index)
            {
                // Get the selected row
                DataGridViewRow row = dgvblotterrecord.Rows[e.RowIndex];

                // Get the BlotterID from the selected row
                string blotterID = row.Cells["BlotterID"].Value.ToString();

                // Call a method to display the report
                ShowBlotterReport(blotterID);
            }
        }

        private void ShowBlotterReport(string blotterID)
        {
            // Create an instance of your report form
            BlotterReportForm reportForm = new BlotterReportForm(blotterID);
            reportForm.ShowDialog(); // Show the form as a dialog
        }
        private void LoadOfficersInCharge()
        {
            string query = "SELECT FullName FROM tblofficialinfo WHERE Position = 'Kagawad'";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Clear existing items to avoid duplicates
                            cbOfficerInCharge.Items.Clear();

                            // Loop through the result set and add each FullName to the ComboBox
                            while (reader.Read())
                            {
                                cbOfficerInCharge.Items.Add(reader["FullName"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors
                    MessageBox.Show($"Failed to load officials: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Generate a unique BlotterID (you can use a timestamp-based approach)
            string blotterID = "BLTR-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // Retrieve the values from the form fields
            string incidentType = txtIncidentname.Text.Trim();
            string complainantName = txtComplainantname.Text.Trim();
            DateTime incidentDate = dateTimePickerIncidentDate.Value;
            string respondentName = txtRespondentname.Text.Trim();
            string incidentDetails = IncidentDetails.Text.Trim();
            string complainantRelationship = cbCRRelationship.SelectedItem?.ToString();
            string officerInCharge = cbOfficerInCharge.SelectedItem?.ToString();
            string status = cbstatus.SelectedItem?.ToString();
            string remarks = cbremarks.SelectedItem?.ToString(); // Retrieve Remarks from ComboBox

            // Check for empty required fields
            if (string.IsNullOrEmpty(incidentType) || string.IsNullOrEmpty(complainantName) ||
                string.IsNullOrEmpty(respondentName) || string.IsNullOrEmpty(incidentDetails) ||
                string.IsNullOrEmpty(complainantRelationship) || string.IsNullOrEmpty(officerInCharge) ||
                string.IsNullOrEmpty(status) || string.IsNullOrEmpty(remarks)) // Check Remarks
            {
                MessageBox.Show("Please fill in all the required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Insert the data into the tblblotter table
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO tblblotter (BlotterID, IncidentType, ComplainantName, IncidentDate, " +
                                   "RespondentName, IncidentDetails, ComplainantNameRelationship, OfficerInCharge, Status, Remarks) " +
                                   "VALUES (@BlotterID, @IncidentType, @ComplainantName, @IncidentDate, @RespondentName, " +
                                   "@IncidentDetails, @ComplainantNameRelationship, @OfficerInCharge, @Status, @Remarks)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Bind the parameters
                        cmd.Parameters.AddWithValue("@BlotterID", blotterID);
                        cmd.Parameters.AddWithValue("@IncidentType", incidentType);
                        cmd.Parameters.AddWithValue("@ComplainantName", complainantName);
                        cmd.Parameters.AddWithValue("@IncidentDate", incidentDate);
                        cmd.Parameters.AddWithValue("@RespondentName", respondentName);
                        cmd.Parameters.AddWithValue("@IncidentDetails", incidentDetails);
                        cmd.Parameters.AddWithValue("@ComplainantNameRelationship", complainantRelationship);
                        cmd.Parameters.AddWithValue("@OfficerInCharge", officerInCharge);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@Remarks", remarks);

                        // Execute the query
                        cmd.ExecuteNonQuery();

                        // Optionally, refresh the DataGridView to reflect the updated data
                        LoadBlotterRecords();
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that may have occurred
                MessageBox.Show($"Error saving blotter record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Check if any row is selected in dgvblotterrecord
            if (dgvblotterrecord.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to update.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the BlotterID from the selected row
            DataGridViewRow selectedRow = dgvblotterrecord.SelectedRows[0];
            string blotterID = selectedRow.Cells["BlotterID"].Value.ToString(); // Assuming you have a BlotterID column in the dgv

            // Retrieve the values from the form fields
            string incidentType = txtIncidentname.Text.Trim();
            string complainantName = txtComplainantname.Text.Trim();
            DateTime incidentDate = dateTimePickerIncidentDate.Value;
            string respondentName = txtRespondentname.Text.Trim();
            string incidentDetails = IncidentDetails.Text.Trim();
            string complainantRelationship = cbCRRelationship.SelectedItem?.ToString();
            string officerInCharge = cbOfficerInCharge.SelectedItem?.ToString();
            string status = cbstatus.SelectedItem?.ToString();
            string remarks = cbremarks.SelectedItem?.ToString(); // Include Remarks field

            // Check for empty required fields
            if (string.IsNullOrEmpty(incidentType) || string.IsNullOrEmpty(complainantName) ||
                string.IsNullOrEmpty(respondentName) || string.IsNullOrEmpty(incidentDetails) ||
                string.IsNullOrEmpty(complainantRelationship) || string.IsNullOrEmpty(officerInCharge) ||
                string.IsNullOrEmpty(status) || string.IsNullOrEmpty(remarks)) // Check for empty Remarks
            {
                MessageBox.Show("Please fill in all the required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Update the record in the database
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE tblblotter SET IncidentType = @IncidentType, ComplainantName = @ComplainantName, " +
                                   "IncidentDate = @IncidentDate, RespondentName = @RespondentName, IncidentDetails = @IncidentDetails, " +
                                   "ComplainantNameRelationship = @ComplainantNameRelationship, OfficerInCharge = @OfficerInCharge, " +
                                   "Status = @Status, Remarks = @Remarks WHERE BlotterID = @BlotterID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Bind the parameters
                        cmd.Parameters.AddWithValue("@BlotterID", blotterID);  // Use BlotterID from the selected row
                        cmd.Parameters.AddWithValue("@IncidentType", incidentType);
                        cmd.Parameters.AddWithValue("@ComplainantName", complainantName);
                        cmd.Parameters.AddWithValue("@IncidentDate", incidentDate);
                        cmd.Parameters.AddWithValue("@RespondentName", respondentName);
                        cmd.Parameters.AddWithValue("@IncidentDetails", incidentDetails);
                        cmd.Parameters.AddWithValue("@ComplainantNameRelationship", complainantRelationship);
                        cmd.Parameters.AddWithValue("@OfficerInCharge", officerInCharge);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@Remarks", remarks); // Add Remarks parameter

                        // Execute the update query
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Record updated successfully!", "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Optionally, refresh the DataGridView to reflect the updated data
                        LoadBlotterRecords();
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating record: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Ensure a record is selected in dgvblotterrecord
            if (dgvblotterrecord.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to delete.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the BlotterID from the selected row
            DataGridViewRow selectedRow = dgvblotterrecord.SelectedRows[0];
            string blotterID = selectedRow.Cells["BlotterID"].Value.ToString();  // Assuming you have a BlotterID column

            // Ask for confirmation before deletion
            DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                // Delete the record from the database
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM tblblotter WHERE BlotterID = @BlotterID";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            // Bind the BlotterID parameter
                            cmd.Parameters.AddWithValue("@BlotterID", blotterID); // Use BlotterID from the selected row

                            // Execute the delete query
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Record deleted successfully!", "Delete Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Optionally, refresh the DataGridView to reflect the deletion
                            LoadBlotterRecords();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting record: {ex.Message}", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnSaveSumon_Click(object sender, EventArgs e)
        {
            // Generate a unique SumonID
            string SumonID = "SMN-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // Retrieve the values from the form fields
            string officerinchargesumon = CBSumonOfficerInCharge.SelectedItem?.ToString();
            string respondent = TxtRespondent.Text.Trim();
            DateTime dtPSumonDate = dtPSumon.Value;
            string Statussumon = cbstatusSumon.SelectedItem?.ToString();
            string sumondetails = rtbSumonDetails.Text.Trim();
            string Remarks = cbRemarksSumon.SelectedItem?.ToString();

            // Check for empty required fields
            if (string.IsNullOrEmpty(officerinchargesumon) || string.IsNullOrEmpty(respondent) ||
                string.IsNullOrEmpty(Statussumon) || string.IsNullOrEmpty(sumondetails) ||
                string.IsNullOrEmpty(Remarks))
            {
                MessageBox.Show("Please fill in all the required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Insert the data into the tblsumon table
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO tblsumon (SumonID, CBSumonOfficerInCharge, dtPSumon, cbstatusSumon, " +
                                   "rtbSumonDetails, TxtRespondent, Remarks) " +
                                   "VALUES (@SumonID, @CBSumonOfficerInCharge, @dtPSumon, @cbstatusSumon, @rtbSumonDetails, " +
                                   "@TxtRespondent, @Remarks)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SumonID", SumonID);
                        cmd.Parameters.AddWithValue("@CBSumonOfficerInCharge", officerinchargesumon);
                        cmd.Parameters.AddWithValue("@dtPSumon", dtPSumonDate);
                        cmd.Parameters.AddWithValue("@cbstatusSumon", Statussumon);
                        cmd.Parameters.AddWithValue("@rtbSumonDetails", sumondetails);
                        cmd.Parameters.AddWithValue("@TxtRespondent", respondent);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);

                        cmd.ExecuteNonQuery();
                        LoadSumonRecords();
                        ClearFormSUmon();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving sumon record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFormSUmon()
        {
            rtbSumonDetails.Clear();
            TxtRespondent.Clear();
            CBSumonOfficerInCharge.SelectedIndex = -1;
            cbstatusSumon.SelectedIndex = -1;
            dtPSumon.Value = DateTime.Now;
            cbRemarksSumon.SelectedIndex = -1;
        }
        private void LoadOfficersInChargeSUmon()
        {
            string query = "SELECT FullName FROM tblofficialinfo WHERE Position = 'Kagawad'";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Clear existing items to avoid duplicates
                            CBSumonOfficerInCharge.Items.Clear();

                            // Loop through the result set and add each FullName to the ComboBox
                            while (reader.Read())
                            {
                                CBSumonOfficerInCharge.Items.Add(reader["FullName"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors
                    MessageBox.Show($"Failed to load officials: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnClearSumon_Click(object sender, EventArgs e)
        {
            ClearFormSUmon();
        }
        private void btnUpdateSumon_Click(object sender, EventArgs e)
        {
            // Check if any row is selected in dgv_SumonRecord
            if (dgv_SumonRecord.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to update.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the SumonID from the selected row
            DataGridViewRow selectedRow = dgv_SumonRecord.SelectedRows[0];
            string SumonID = selectedRow.Cells["SumonID"].Value.ToString(); // Assuming you have a SumonID column in the dgv

            // Retrieve the values from the form fields
            string officerinchargesumon = CBSumonOfficerInCharge.SelectedItem?.ToString();
            string respondent = TxtRespondent.Text.Trim();
            DateTime dtPSumonDate = dtPSumon.Value;
            string Statussumon = cbstatusSumon.SelectedItem?.ToString();
            string sumondetails = rtbSumonDetails.Text.Trim();
            string Remarks = cbRemarksSumon.SelectedItem?.ToString();

            // Check for empty required fields
            if (string.IsNullOrEmpty(officerinchargesumon) || string.IsNullOrEmpty(respondent) ||
                string.IsNullOrEmpty(Statussumon) || string.IsNullOrEmpty(sumondetails) || string.IsNullOrEmpty(Remarks))
            {
                MessageBox.Show("Please fill in all the required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Update the record in the database
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE tblsumon SET CBSumonOfficerInCharge = @CBSumonOfficerInCharge, dtPSumon = @dtPSumon, " +
                                   "cbstatusSumon = @cbstatusSumon, rtbSumonDetails = @rtbSumonDetails, TxtRespondent = @TxtRespondent, " +
                                   "Remarks = @Remarks WHERE SumonID = @SumonID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Bind the parameters
                        cmd.Parameters.AddWithValue("@SumonID", SumonID);
                        cmd.Parameters.AddWithValue("@CBSumonOfficerInCharge", officerinchargesumon);
                        cmd.Parameters.AddWithValue("@dtPSumon", dtPSumonDate);
                        cmd.Parameters.AddWithValue("@cbstatusSumon", Statussumon);
                        cmd.Parameters.AddWithValue("@rtbSumonDetails", sumondetails);
                        cmd.Parameters.AddWithValue("@TxtRespondent", respondent);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);

                        // Execute the update query
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Record updated successfully!", "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Optionally, refresh the DataGridView to reflect the updated data
                        LoadSumonRecords();
                        ClearFormSUmon();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating record: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnDeleteSumon_Click(object sender, EventArgs e)
        {
            // Ensure a record is selected in dgvblotterrecord
            if (dgv_SumonRecord.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to delete.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the BlotterID from the selected row
            DataGridViewRow selectedRow = dgv_SumonRecord.SelectedRows[0];
            string SumonID = selectedRow.Cells["SumonID"].Value.ToString();  // Assuming you have a BlotterID column

            // Ask for confirmation before deletion
            DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                // Delete the record from the database
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM tblsumon WHERE SumonID = @SumonID";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            // Bind the BlotterID parameter
                            cmd.Parameters.AddWithValue("@SumonID", SumonID); // Use BlotterID from the selected row

                            // Execute the delete query
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Record deleted successfully!", "Delete Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Optionally, refresh the DataGridView to reflect the deletion
                            LoadSumonRecords();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting record: {ex.Message}", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgv_SumonRecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Check if the clicked cell is a button cell
                if (e.ColumnIndex == dgv_SumonRecord.Columns["Action"].Index && e.RowIndex >= 0)
                {
                    // Get the SumonID of the selected row
                    string sumonID = dgv_SumonRecord.Rows[e.RowIndex].Cells["SumonID"].Value.ToString();

                    // Create and show the SumonReportForm
                    SumonReportForm reportForm = new SumonReportForm(sumonID);
                    reportForm.ShowDialog(); // Use ShowDialog to open as a modal dialog
                }
                else if (e.ColumnIndex == dgv_SumonRecord.Columns["SumonID"].Index && e.RowIndex >= 0)
                {
                    // Get the SumonID of the selected row
                    string sumonID = dgv_SumonRecord.Rows[e.RowIndex].Cells["SumonID"].Value.ToString();

                    // Load the details into the fields
                    LoadSumonDetails(sumonID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSumonDetails(string sumonID)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Query to retrieve the full record based on the selected SumonID
                    string query = "SELECT * FROM tblsumon WHERE SumonID = @SumonID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Bind the parameters
                    cmd.Parameters.AddWithValue("@SumonID", sumonID); // Note: Adjust parameter if necessary based on the actual column name and type

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate the form fields with the retrieved data
                            TxtRespondent.Text = reader["TxtRespondent"].ToString();
                            rtbSumonDetails.Text = reader["rtbSumonDetails"].ToString();
                            CBSumonOfficerInCharge.SelectedItem = reader["CBSumonOfficerInCharge"].ToString();
                            cbstatusSumon.SelectedItem = reader["cbstatusSumon"].ToString();
                            cbRemarksSumon.SelectedItem = reader["Remarks"].ToString();
                            dtPSumon.Value = Convert.ToDateTime(reader["dtPSumon"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sumon details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnsavecomplaint_Click(object sender, EventArgs e)
        {
            // Generate a unique ComplaintID (timestamp-based)
            string ComplaintID = "CID-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            // Retrieve values from the form fields
            string complaintType = txtComplaintType.Text.Trim();
            string complaintName = txtComplaintName.Text.Trim();
            string description = rtBDescription.Text.Trim();
            string remarks = cbremarkscomplaints.SelectedItem?.ToString(); // Added Remarks field
            DateTime dateFiled = dtPDateFiled.Value;
            DateTime expectedTime = dtpExpectedResolution.Value;
            string status = cbComplaintstatus.SelectedItem?.ToString();
            string latitude = txtLatitude.Text.Trim();
            string longitude = txtLongitude.Text.Trim();

            // Check for empty required fields
            if (string.IsNullOrEmpty(complaintType) || string.IsNullOrEmpty(complaintName) ||
                string.IsNullOrEmpty(description) || string.IsNullOrEmpty(status) ||
                string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude) ||
                string.IsNullOrEmpty(remarks)) // Validate Remarks
            {
                MessageBox.Show("Please fill in all required fields, including Latitude, Longitude, and Remarks.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate latitude and longitude input (numeric check)
            if (!double.TryParse(latitude, out double lat) || !double.TryParse(longitude, out double lng))
            {
                MessageBox.Show("Latitude and Longitude must be valid numeric values.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate date consistency (DateFiled should not be in the future)
            if (dateFiled > DateTime.Now)
            {
                MessageBox.Show("Date Filed cannot be in the future.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate expected resolution date (should not be before the date filed)
            if (expectedTime < dateFiled)
            {
                MessageBox.Show("Expected Resolution Date cannot be earlier than Date Filed.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Insert the data into the tblcomplaint table
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO tblcomplaint (ComplaintID, ComplaintName, DateFiled, ComplaintType, " +
                                   "ExpectedTime, Description, StatusComplaint, Latitude, Longitude, Remarks) " + // Added Remarks
                                   "VALUES (@ComplaintID, @ComplaintName, @DateFiled, @ComplaintType, @ExpectedTime, " +
                                   "@Description, @StatusComplaint, @Latitude, @Longitude, @Remarks)"; // Added Remarks

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Bind parameters
                        cmd.Parameters.AddWithValue("@ComplaintID", ComplaintID);
                        cmd.Parameters.AddWithValue("@ComplaintName", complaintName);
                        cmd.Parameters.AddWithValue("@DateFiled", dateFiled);
                        cmd.Parameters.AddWithValue("@ComplaintType", complaintType);
                        cmd.Parameters.AddWithValue("@ExpectedTime", expectedTime);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@StatusComplaint", status);
                        cmd.Parameters.AddWithValue("@Latitude", latitude);
                        cmd.Parameters.AddWithValue("@Longitude", longitude);
                        cmd.Parameters.AddWithValue("@Remarks", remarks); // Added Remarks

                        // Execute the query
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Complaint record saved successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Add marker to the map with correct variable names
                        AddMarkerToMap(lat, lng, complaintName, dateFiled.ToString(), complaintType, status, remarks);

                        // Refresh the records and clear the form
                        LoadComplaintRecords();
                        ClearFormComplaint();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving complaint record: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearFormComplaint()
        {
            txtComplaintType.Clear();
            txtComplaintName.Clear();
            rtBDescription.Clear();
            cbComplaintstatus.SelectedIndex = -1;
            cbremarkscomplaints.SelectedIndex = -1;
            dtPDateFiled.Value = DateTime.Now;
            dtpExpectedResolution.Value = DateTime.Now;
            txtLongitude.Clear();
            txtLatitude.Clear();
        }

        private void btnupdatecomplaint_Click(object sender, EventArgs e)
        {
            // Check if any row is selected in dgv_Complaint
            if (dgv_Complaint.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to update.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the ComplaintID from the selected row
            DataGridViewRow selectedRow = dgv_Complaint.SelectedRows[0];
            string ComplaintID = selectedRow.Cells["ComplaintID"].Value.ToString(); // Ensure the column name matches your DataGridView

            // Retrieve the values from the form fields
            string complaintType = txtComplaintType.Text.Trim();
            string complaintName = txtComplaintName.Text.Trim();
            string description = rtBDescription.Text.Trim();
            string remarks = cbremarkscomplaints.SelectedItem?.ToString(); // Added Remarks
            DateTime dateFiled = dtPDateFiled.Value;
            DateTime expectedTime = dtpExpectedResolution.Value;
            string status = cbComplaintstatus.SelectedItem?.ToString();
            string latitude = txtLatitude.Text.Trim();
            string longitude = txtLongitude.Text.Trim();

            // Check for empty required fields
            if (string.IsNullOrEmpty(complaintType) || string.IsNullOrEmpty(complaintName) ||
                string.IsNullOrEmpty(description) || string.IsNullOrEmpty(status) ||
                string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude) ||
                string.IsNullOrEmpty(remarks)) // Validate Remarks
            {
                MessageBox.Show("Please fill in all the required fields, including Latitude, Longitude, and Remarks.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate latitude and longitude input (numeric check)
            if (!double.TryParse(latitude, out double lat) || !double.TryParse(longitude, out double lng))
            {
                MessageBox.Show("Latitude and Longitude must be valid numeric values.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate date consistency (DateFiled should not be in the future)
            if (dateFiled > DateTime.Now)
            {
                MessageBox.Show("Date Filed cannot be in the future.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate expected resolution date (should not be before the date filed)
            if (expectedTime < dateFiled)
            {
                MessageBox.Show("Expected Resolution Date cannot be earlier than Date Filed.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Update the record in the database
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE tblcomplaint SET ComplaintName = @ComplaintName, DateFiled = @DateFiled, " +
                                   "ComplaintType = @ComplaintType, ExpectedTime = @ExpectedTime, Description = @Description, " +
                                   "StatusComplaint = @StatusComplaint, Latitude = @Latitude, Longitude = @Longitude, Remarks = @Remarks " + // Added Remarks
                                   "WHERE ComplaintID = @ComplaintID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Bind parameters
                        cmd.Parameters.AddWithValue("@ComplaintID", ComplaintID);
                        cmd.Parameters.AddWithValue("@ComplaintName", complaintName);
                        cmd.Parameters.AddWithValue("@DateFiled", dateFiled);
                        cmd.Parameters.AddWithValue("@ComplaintType", complaintType);
                        cmd.Parameters.AddWithValue("@ExpectedTime", expectedTime);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@StatusComplaint", status);
                        cmd.Parameters.AddWithValue("@Latitude", latitude);
                        cmd.Parameters.AddWithValue("@Longitude", longitude);
                        cmd.Parameters.AddWithValue("@Remarks", remarks); // Added Remarks

                        // Execute the query
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Complaint record updated successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh the records and clear the form
                        LoadComplaintRecords();
                        ClearFormComplaint();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating complaint record: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btndeletecomplaint_Click(object sender, EventArgs e)
        {
            // Ensure a record is selected in dgvblotterrecord
            if (dgv_Complaint.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to delete.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the BlotterID from the selected row
            DataGridViewRow selectedRow = dgv_Complaint.SelectedRows[0];
            string ComplaintID = selectedRow.Cells["ComplaintID"].Value.ToString();  // Assuming you have a BlotterID column

            // Ask for confirmation before deletion
            DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                // Delete the record from the database
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM tblcomplaint WHERE ComplaintID = @ComplaintID";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            // Bind the BlotterID parameter
                            cmd.Parameters.AddWithValue("@ComplaintID", ComplaintID); // Use BlotterID from the selected row

                            // Execute the delete query
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Record deleted successfully!", "Delete Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Optionally, refresh the DataGridView to reflect the deletion
                            LoadComplaintRecords();
                            ClearFormComplaint();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting record: {ex.Message}", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void LoadComplaintDetails(string respondent)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Query to retrieve the full record based on the selected IncidentType and ComplainantName
                    string query = "SELECT * FROM tblcomplaint WHERE  ComplaintName = @ComplaintName";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Bind the parameters
                    cmd.Parameters.AddWithValue("@ComplaintName", respondent);


                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate the form fields with the retrieved data
                            txtComplaintName.Text = reader["ComplaintName"].ToString();
                            txtComplaintType.Text = reader["ComplaintType"].ToString();
                            rtBDescription.Text = reader["Description"].ToString();
                            cbComplaintstatus.SelectedItem = reader["StatusComplaint"].ToString();
                            cbremarkscomplaints.SelectedItem = reader["Remarks"].ToString();
                            dtPDateFiled.Value = Convert.ToDateTime(reader["DateFiled"]);
                            dtpExpectedResolution.Value = Convert.ToDateTime(reader["ExpectedTime"]);
                            txtLatitude.Text = reader["Latitude"].ToString();
                            txtLongitude.Text = reader["Longitude"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading blotter details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgv_Complaint_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgv_Complaint.Rows[e.RowIndex];
                string respondent = row.Cells["ComplaintName"].Value.ToString();
                LoadComplaintDetails(respondent);
            }
        }
        private void gmapComplaints_Load(object sender, EventArgs e)
        {
            // Initialize map settings
            gmapComplaints.MapProvider = GMapProviders.GoogleMap;
            gmapComplaints.Position = new PointLatLng(17.0233, 121.6314); // Default position
            gmapComplaints.MinZoom = 2;
            gmapComplaints.MaxZoom = 18;
            gmapComplaints.Zoom = 16;
            gmapComplaints.CanDragMap = true;
            gmapComplaints.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            gmapComplaints.IgnoreMarkerOnMouseWheel = true;
            gmapComplaints.NegativeMode = false;
            gmapComplaints.ShowTileGridLines = false;

            // Clear existing overlays if any
            gmapComplaints.Overlays.Clear();
            LoadSavedMarkers();



        }
        private void LoadSavedMarkers()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ComplaintName, DateFiled, ComplaintType, Latitude, Longitude, StatusComplaint, Remarks FROM tblcomplaint";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Retrieve data with validation
                            string ComplaintName = reader["ComplaintName"]?.ToString() ?? "Unknown";
                            string DateFiled = reader["DateFiled"]?.ToString() ?? "Unknown";
                            string ComplaintType = reader["ComplaintType"]?.ToString() ?? "Unknown";
                            string StatusComplaint = reader["StatusComplaint"]?.ToString() ?? "Unknown";
                            string Remarks = reader["Remarks"]?.ToString() ?? "No Remarks";
                            double latitude;
                            double longitude;

                            // Parse latitude and longitude safely
                            if (!double.TryParse(reader["Latitude"]?.ToString(), out latitude) ||
                                !double.TryParse(reader["Longitude"]?.ToString(), out longitude))
                            {
                                // Skip invalid entries
                                continue;
                            }

                            // Add marker to the map
                            AddMarkerToMap(latitude, longitude, ComplaintName, DateFiled, ComplaintType, StatusComplaint, Remarks);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading saved markers: " + ex.Message);
                }
            }
        }

        private void AddMarkerToMap(double latitude, double longitude, string ComplaintName, string DateFiled, string ComplaintType, string StatusComplaint, string Remarks)
        {
            // Find or create overlay
            GMapOverlay markersOverlay = gmapComplaints.Overlays.FirstOrDefault(o => o.Id == "markers");
            if (markersOverlay == null)
            {
                markersOverlay = new GMapOverlay("markers");
                gmapComplaints.Overlays.Add(markersOverlay);
            }

            // Create a marker
            GMapMarker marker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.red_dot);

            // Set the tooltip with all details
            marker.ToolTipText = $"Complaint: {ComplaintName}\n" +
                                 $"Date Filed: {DateFiled}\n" +
                                 $"Type: {ComplaintType}\n" +
                                 $"Status: {StatusComplaint}\n" +
                                 $"Remarks: {Remarks}\n" +
                                 $"Coordinates: {latitude}, {longitude}";

            // Style the tooltip
            var toolTip = new GMapToolTip(marker)
            {
                Foreground = Brushes.White,
                Fill = Brushes.DarkSlateGray,
                Font = new Font("Century Gothic", 10, FontStyle.Bold),
                Stroke = new Pen(Brushes.Black, 2)
            };

            marker.ToolTip = toolTip;
            marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;

            // Add marker to the overlay
            markersOverlay.Markers.Add(marker);

            // Refresh the map
            gmapComplaints.Refresh();
        }

        private void btnclearcomplaint_Click(object sender, EventArgs e)
        {
            ClearFormComplaint();
        }

        private void txtsearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtsearch.Text.Trim();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Search query with LIKE for partial matches
                    string query = @"SELECT BlotterID, IncidentType, ComplainantName, Status 
                             FROM tblblotter
                             WHERE IncidentType LIKE @searchText 
                             OR ComplainantName LIKE @searchText 
                             OR Status LIKE @searchText";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                    DataTable dt = new DataTable();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    // Set row height and header height
                    dgvblotterrecord.RowTemplate.Height = 35; // Adjust height as needed
                    dgvblotterrecord.ColumnHeadersHeight = 40; // Adjust height as needed

                    // Bind search results to the DataGridView
                    dgvblotterrecord.DataSource = dt;
                    dgvblotterrecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Set headers if columns exist
                    if (dgvblotterrecord.Columns.Count >= 4)
                    {
                        dgvblotterrecord.Columns[0].HeaderText = "Blotter ID";
                        dgvblotterrecord.Columns[1].HeaderText = "Incident";
                        dgvblotterrecord.Columns[2].HeaderText = "Complainant";
                        dgvblotterrecord.Columns[3].HeaderText = "Status";
                    }

                    // Add the Print Blotter button column if not already present
                    if (!dgvblotterrecord.Columns.Contains("PrintBlotter"))
                    {
                        DataGridViewButtonColumn printButtonColumn = new DataGridViewButtonColumn
                        {
                            Name = "PrintBlotter",
                            HeaderText = "Print Blotter",
                            Text = "Print",
                            UseColumnTextForButtonValue = true
                        };
                        dgvblotterrecord.Columns.Add(printButtonColumn);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while searching: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvblotterrecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Ensure the row index is valid
                if (e.RowIndex >= 0)
                {
                    // Retrieve the DataGridView row where the click occurred
                    DataGridViewRow row = dgvblotterrecord.Rows[e.RowIndex];

                    // Extract values from the row's cells (adjust the column names or indices as needed)
                    string incidentType = row.Cells["IncidentType"].Value.ToString();
                    string complainantName = row.Cells["ComplainantName"].Value.ToString();

                    // Call the LoadBlotterDetails method with the retrieved values
                    LoadBlotterDetails(incidentType, complainantName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtsumonsearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtsumonsearch.Text.Trim();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = @"SELECT SumonID, TxtRespondent, cbstatusSumon
                             FROM tblsumon
                             WHERE TxtRespondent LIKE @searchText";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgv_SumonRecord.DataSource = dataTable;
                    // Set row height
                    dgv_SumonRecord.RowTemplate.Height = 35; // Adjust height as needed

                    // Set header row height
                    dgv_SumonRecord.ColumnHeadersHeight = 40; // Adjust height as needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while searching: " + ex.Message);
            }
        }

        private void txtSearch_Complaint_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch_Complaint.Text.Trim();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // SQL query to search by ComplaintName
                    string query = @"SELECT ComplaintID, ComplaintName, ComplaintType, StatusComplaint 
                             FROM tblcomplaint
                             WHERE ComplaintName LIKE @searchText";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                    DataTable dt = new DataTable();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dt);

                    // Bind search results to the DataGridView
                    dgv_Complaint.DataSource = dt;
                    dgv_Complaint.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // Set headers if columns exist
                    if (dgv_Complaint.Columns.Count >= 4)
                    {
                        dgv_Complaint.Columns[0].HeaderText = "Complaint ID";
                        dgv_Complaint.Columns[1].HeaderText = "Complaint Name";
                        dgv_Complaint.Columns[2].HeaderText = "Complaint Type";
                        dgv_Complaint.Columns[3].HeaderText = "Status";
                    }

                    // Adjust row and header heights
                    dgv_Complaint.RowTemplate.Height = 35; // Adjust height as needed
                    dgv_Complaint.ColumnHeadersHeight = 40; // Adjust height as needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while searching complaints: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
