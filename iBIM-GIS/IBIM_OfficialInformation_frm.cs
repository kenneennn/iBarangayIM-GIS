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
using AForge.Video;
using AForge.Video.DirectShow;


namespace iBIM_GIS
{
    public partial class IBIM_OfficialInformation_frm : Form
    {
        string connectionString = "server=localhost;user=root;database=dbiBIM;password=;";
        private Image officialImage;
        public IBIM_OfficialInformation_frm()
        {
            InitializeComponent();
            LoadOfficialData();
            dgvofficial.CellClick += dgvofficial_CellContentClick_1;
            dtpBirthdate.ValueChanged += dtpBirthdate_ValueChanged;
            cbPosition.SelectedIndexChanged += cbPosition_SelectedIndexChanged;
        }
        public void SetOfficialImage(Image image)
        {
            officialImage = image;
            pbOfficialImage.Image = officialImage;
            pbOfficialImage.Image = image;
        }
        private void ResetFields()
        {
            txtOfficialID.Clear();
            txtFullname.Clear();
            cbPosition.SelectedIndex = -1;
            dtpBirthdate.Value = DateTime.Now;
            txtAge.Clear();
            cbSex.SelectedIndex = -1;
            txtBirthplace.Clear();
            cbCivilstatus.SelectedIndex = -1;
            cbReligion.SelectedIndex = -1;
            txtMobilenumber.Clear();
            cbCommittee.SelectedIndex = -1;
            cbareaofassingment.SelectedIndex = -1;
            txtHeight.Clear();
            txtBloodtype.Clear();
            txtWeight.Clear();
            cbTermofposition.SelectedIndex = -1;
            dtpDateelected.Value = DateTime.Now;
            txtContactperson.Clear();
            txtContactaddress.Clear();
            txtContactnumber.Clear();
            txtRelationship.Clear();
            pbOfficialImage.Image = null;
            pBSignature.Image = null;

            btnSave.Enabled = true;
        }
        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
        private void LoadOfficialData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = "SELECT OfficialID, Fullname, Position FROM tblofficialinfo";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvofficial.RowTemplate.Height = 35; // Adjust height as needed
                    dgvofficial.ColumnHeadersHeight = 40; // Adjust height as needed
                    dgvofficial.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading data: " + ex.Message);
            }
        }
       
        private bool IsCameraAvailable()
        {
            try
            {
                var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                return videoDevices.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking camera: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public Size GetOfficialImageSize()
        {
            return pbOfficialImage.Size;
        }
        // Declare a class-level variable to hold the ReportForm instance
        private ReportForm reportFormInstance;

        private void dgvofficial_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Check if the click is on a valid row and column
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    // Check if the clicked cell is in the button column named "Print ID"
                    if (dgvofficial.Columns[e.ColumnIndex].Name == "Print ID")
                    {
                        // Get the OfficialID from the selected row
                        object officialIdValue = dgvofficial.Rows[e.RowIndex].Cells["OfficialID"].Value;
                        if (officialIdValue != null)
                        {
                            string officialId = officialIdValue.ToString();

                            // Check if the form instance already exists and is not disposed
                            if (reportFormInstance == null || reportFormInstance.IsDisposed)
                            {
                                // Create a new instance if it doesn't exist or has been disposed
                                reportFormInstance = new ReportForm(connectionString, officialId);
                            }

                            // Bring the form to the front if it's already open
                            reportFormInstance.Show();
                            reportFormInstance.BringToFront();
                        }
                        else
                        {
                            // Show an error message if the OfficialID is not available
                            MessageBox.Show("Official ID is not available for this row.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        // Handle other columns here (if any)
                        // Populate the text boxes with the row data for fields other than "Print ID"
                        txtOfficialID.Text = dgvofficial.Rows[e.RowIndex].Cells["OfficialID"].Value?.ToString() ?? string.Empty;
                        txtFullname.Text = dgvofficial.Rows[e.RowIndex].Cells["Fullname"].Value?.ToString() ?? string.Empty;
                        cbPosition.SelectedItem = dgvofficial.Rows[e.RowIndex].Cells["Position"].Value?.ToString() ?? string.Empty;

                        // Disable the Save button if loading details for an existing official
                        btnSave.Enabled = false;

                        // Load additional official details (if required)
                        LoadOfficialDetails(txtOfficialID.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                // Show an error message if something goes wrong
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOfficialDetails(string officialID)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = @"SELECT Birthdate, Age, Sex, Birthplace, CivilStatus, Address, Religion, MobileNo, 
                                     Committee, AreaOfAssignment, Height, Bloodtype,Weight, Termsofposition, DateElected, ContactPerson, 
                                     ContactAddress, ContactNo, Relationship, Image,Signature
                                     FROM tblofficialinfo WHERE OfficialID = @OfficialID";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OfficialID", officialID);

                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dtpBirthdate.Value = reader.GetDateTime("Birthdate");
                            txtAge.Text = reader["Age"].ToString();
                            cbSex.SelectedItem = reader["Sex"].ToString();
                            txtBirthplace.Text = reader["Birthplace"].ToString();
                            cbCivilstatus.SelectedItem = reader["CivilStatus"].ToString();
                            txtAddress.Text = reader["Address"].ToString();
                            cbReligion.SelectedItem = reader["Religion"].ToString();
                            txtMobilenumber.Text = reader["MobileNo"].ToString();
                            cbCommittee.SelectedItem = reader["Committee"].ToString();
                            cbareaofassingment.SelectedItem = reader["AreaOfAssignment"].ToString();
                            txtHeight.Text = reader["Height"].ToString();
                            txtBloodtype.Text = reader["Bloodtype"].ToString();
                            txtWeight.Text = reader["Weight"].ToString();
                            cbTermofposition.SelectedItem = reader["Termsofposition"].ToString();
                            dtpDateelected.Value = reader.GetDateTime("DateElected");
                            txtContactperson.Text = reader["ContactPerson"].ToString();
                            txtContactaddress.Text = reader["ContactAddress"].ToString();
                            txtContactnumber.Text = reader["ContactNo"].ToString();
                            txtRelationship.Text = reader["Relationship"].ToString();

                            if (!reader.IsDBNull(reader.GetOrdinal("Image")))
                            {
                                byte[] imageBytes = (byte[])reader["Image"];
                                using (var ms = new System.IO.MemoryStream(imageBytes))
                                {
                                    pbOfficialImage.Image = Image.FromStream(ms);
                                }
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Signature")))
                            {
                                byte[] imageBytes = (byte[])reader["Signature"];
                                using (var ms = new System.IO.MemoryStream(imageBytes))
                                {
                                    pBSignature.Image = Image.FromStream(ms);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading details: " + ex.Message);
            }
        }
        private void dtpBirthdate_ValueChanged(object sender, EventArgs e)
        {
            DateTime birthDate = dtpBirthdate.Value;
            int age = DateTime.Now.Year - birthDate.Year;

            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
            {
                age--;
            }

            txtAge.Text = age.ToString();
        }
        private void IBIM_OfficialInformation_frm_Load(object sender, EventArgs e)
        {
            LoadPurokNames();
            LoadCommittee();
            LoadBarangayName();
            DisplayBarangayName();
            // Add Print ID button column to the DataGridView if it doesn't exist
            if (dgvofficial.Columns["Print ID"] == null)
            {
                DataGridViewButtonColumn btnColumn = new DataGridViewButtonColumn();
                btnColumn.Name = "Print ID";
                btnColumn.HeaderText = "Print ID";
                btnColumn.Text = "Print ID";
                btnColumn.UseColumnTextForButtonValue = true; // This allows the button to display the text
                dgvofficial.Columns.Add(btnColumn);
            }
        }
        private void DisplayBarangayName()
        {
            string query = "SELECT BarangayName FROM tblsetbrgyname LIMIT 1";  // Assuming you want to get the first entry

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())  // Check if there's at least one record
                    {
                        string barangayName = reader["BarangayName"].ToString().ToUpper();  // Convert to uppercase
                                                                                            // Set the label text with the fetched BarangayName in uppercase
                        label25.Text = $" {barangayName} Official And Staff Record".ToUpper();  // Ensure the whole text is uppercase
                    }
                    else
                    {
                        // If no data found
                        label25.Text = "Barangay information not found.".ToUpper();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void cbPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ensure an item is selected before accessing SelectedItem
            if (cbPosition.SelectedItem != null)
            {
                string selectedPosition = cbPosition.SelectedItem.ToString();

                // Define disabled text as a constant
                const string disabledText = "Disabled";

                // Secretary, Treasurer, SK Chairman, SK Kagawad, SK Secretary, and SK Treasurer: Disable cbCommittee and cbareaofassingment
                if (selectedPosition == "Secretary" || selectedPosition == "Treasurer" ||
                    selectedPosition == "SK Chairman" || selectedPosition == "SK Kagawad" ||
                    selectedPosition == "SK Secretary" || selectedPosition == "SK Treasurer")
                {
                    cbCommittee.Enabled = false;
                    cbCommittee.SelectedIndex = -1;
                    cbCommittee.Text = disabledText;

                    cbareaofassingment.Enabled = false;
                    cbareaofassingment.SelectedIndex = -1;
                    cbareaofassingment.Text = disabledText;

                    cbTermofposition.Enabled = true;
                    cbTermofposition.ResetText();

                    dtpDateelected.Enabled = true;
                }
                // Chairman: Enable all fields
                else if (selectedPosition == "Chairman")
                {
                    cbCommittee.Enabled = false;
                    cbareaofassingment.Enabled = false;
                    cbTermofposition.Enabled = true;
                    dtpDateelected.Enabled = true;

                    // Clear the "Disabled" text if necessary
                    cbCommittee.ResetText();
                    cbareaofassingment.ResetText();
                    cbTermofposition.ResetText();
                }
                // Barangay Health Workers: Only enable cbareaofassingment
                else if (selectedPosition == "Barangay Health Workers")
                {
                    cbCommittee.Enabled = false;
                    cbCommittee.SelectedIndex = -1;
                    cbCommittee.Text = disabledText;

                    cbareaofassingment.Enabled = true;
                    cbareaofassingment.ResetText();

                    cbTermofposition.Enabled = false;
                    cbTermofposition.SelectedIndex = -1;
                    cbTermofposition.Text = disabledText;

                    dtpDateelected.Enabled = false;
                    dtpDateelected.Value = DateTime.Now; // Set to current date or desired default
                }
                // Barangay Tanod, Utility Workers, Nutrition Scholar, Midwife, and Chief of Tanod: Disable all fields
                else if (selectedPosition == "Barangay Tanod" ||
                         selectedPosition == "Barangay Utility Workers" || selectedPosition == "Barangay Nutrition Scholar" ||
                         selectedPosition == "Midwife" || selectedPosition == "Chief of Tanod")
                {
                    cbCommittee.Enabled = false;
                    cbCommittee.SelectedIndex = -1;
                    cbCommittee.Text = disabledText;

                    cbareaofassingment.Enabled = false;
                    cbareaofassingment.SelectedIndex = -1;
                    cbareaofassingment.Text = disabledText;

                    cbTermofposition.Enabled = false;
                    cbTermofposition.SelectedIndex = -1;
                    cbTermofposition.Text = disabledText;

                    dtpDateelected.Enabled = false;
                    dtpDateelected.Value = DateTime.Now; // Set to current date or desired default
                }
                // Default: Enable all fields
                else
                {
                    cbCommittee.Enabled = true;
                    cbCommittee.ResetText();

                    cbareaofassingment.Enabled = true;
                    cbareaofassingment.ResetText();

                    cbTermofposition.Enabled = true;
                    cbTermofposition.ResetText();

                    dtpDateelected.Enabled = true;
                }
            }
        }
        private void txtAreaofassignment_TextChanged(object sender, EventArgs e)
        {
            if (cbPosition.SelectedItem != null)
            {
                string selectedPosition = cbPosition.SelectedItem.ToString();

                if (selectedPosition == "Secretary" || selectedPosition == "Treasurer" || selectedPosition == "Chairman")
                {
                    cbareaofassingment.Enabled = false;
                    cbareaofassingment.SelectedIndex = -1; // Clear the text when disabling
                }
                else if (selectedPosition == "Kagawad")
                {
                    cbareaofassingment.Enabled = true;
                }
            }
        }

        private void LoadCommittee()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Committee FROM tblcommittee";  // SQL Query

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            cbCommittee.Items.Clear(); // Clear previous items

                            while (reader.Read())
                            {
                                cbCommittee.Items.Add(reader["Committee"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        private void LoadPurokNames()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT PurokName FROM tblpurokinfo";  // SQL Query

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            cbareaofassingment.Items.Clear(); // Clear previous items

                            while (reader.Read())
                            {
                                cbareaofassingment.Items.Add(reader["PurokName"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        private void LoadBarangayName()
        {
            // Use your global connection string
            string connectionString = "server=localhost;user=root;database=dbiBIM;password=;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Query to get BarangayName from tblsetbrgyname
                    string query = "SELECT BarangayName FROM tblsetbrgyname LIMIT 1"; // Adjust as needed if you expect only one row

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        // Execute the command and retrieve the result
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            txtAddress.Text = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("No Barangay Name found in the table.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Barangay Name: " + ex.Message);
                }
            }
        }

        private void btnUpload_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pbOfficialImage.Image = Image.FromFile(openFileDialog.FileName);
                        pbOfficialImage.SizeMode = PictureBoxSizeMode.StretchImage; // Optional
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while uploading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if ((txtOfficialID.Enabled && string.IsNullOrWhiteSpace(txtOfficialID.Text)) ||
                (txtFullname.Enabled && string.IsNullOrWhiteSpace(txtFullname.Text)) ||
                (cbPosition.Enabled && cbPosition.SelectedIndex == -1) ||
                (txtAge.Enabled && string.IsNullOrWhiteSpace(txtAge.Text)) ||
                (cbSex.Enabled && cbSex.SelectedIndex == -1) ||
                (txtBirthplace.Enabled && string.IsNullOrWhiteSpace(txtBirthplace.Text)) ||
                (cbCivilstatus.Enabled && cbCivilstatus.SelectedIndex == -1) ||
                (txtAddress.Enabled && string.IsNullOrWhiteSpace(txtAddress.Text)) ||
                (cbReligion.Enabled && cbReligion.SelectedIndex == -1) ||
                (txtMobilenumber.Enabled && string.IsNullOrWhiteSpace(txtMobilenumber.Text)) ||
                (cbCommittee.Enabled && cbCommittee.SelectedIndex == -1) ||
                (cbareaofassingment.Enabled && cbareaofassingment.SelectedIndex == -1) ||
                (txtHeight.Enabled && string.IsNullOrWhiteSpace(txtHeight.Text)) ||
                (txtBloodtype.Enabled && string.IsNullOrWhiteSpace(txtBloodtype.Text)) ||
                (txtWeight.Enabled && string.IsNullOrWhiteSpace(txtWeight.Text)) ||
                (cbTermofposition.Enabled && cbTermofposition.SelectedIndex == -1) ||
                (txtContactperson.Enabled && string.IsNullOrWhiteSpace(txtContactperson.Text)) ||
                (txtContactaddress.Enabled && string.IsNullOrWhiteSpace(txtContactaddress.Text)) ||
                (txtContactnumber.Enabled && string.IsNullOrWhiteSpace(txtContactnumber.Text)) ||
                (txtRelationship.Enabled && string.IsNullOrWhiteSpace(txtRelationship.Text)))
            {
                MessageBox.Show("Please fill out all the required fields.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string checkOfficialIDQuery = "SELECT COUNT(*) FROM tblofficialinfo WHERE OfficialID = @OfficialID";
                    MySqlCommand checkOfficialIDCmd = new MySqlCommand(checkOfficialIDQuery, conn);
                    checkOfficialIDCmd.Parameters.AddWithValue("@OfficialID", txtOfficialID.Text);

                    int idCount = Convert.ToInt32(checkOfficialIDCmd.ExecuteScalar());

                    if (idCount > 0)
                    {
                        MessageBox.Show("The Official ID already exists. Please enter a unique Official ID.", "Duplicate ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = @"INSERT INTO tblofficialinfo 
                         (OfficialID, Fullname, Position, Birthdate, Age, Sex, Birthplace, CivilStatus, Address, Religion, MobileNo, 
                          Committee, AreaOfAssignment, Height, Bloodtype, Weight, Termsofposition, DateElected, ContactPerson, 
                          ContactAddress, ContactNo, Relationship, Image, Signature) 
                         VALUES 
                         (@OfficialID, @Fullname, @Position, @Birthdate, @Age, @Sex, @Birthplace, @CivilStatus, @Address, 
                          @Religion, @MobileNo, @Committee, @AreaOfAssignment, @Height, @Bloodtype, @Weight, @Termsofposition, 
                          @DateElected, @ContactPerson, @ContactAddress, @ContactNo, @Relationship, @Image, @Signature)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@OfficialID", txtOfficialID.Text);
                    cmd.Parameters.AddWithValue("@Fullname", txtFullname.Text);
                    cmd.Parameters.AddWithValue("@Position", cbPosition.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Birthdate", dtpBirthdate.Value);
                    cmd.Parameters.AddWithValue("@Age", txtAge.Text);
                    cmd.Parameters.AddWithValue("@Sex", cbSex.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Birthplace", txtBirthplace.Text);
                    cmd.Parameters.AddWithValue("@CivilStatus", cbCivilstatus.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@Religion", cbReligion.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@MobileNo", txtMobilenumber.Text);

                    if (cbCommittee.Enabled && cbCommittee.SelectedItem != null)
                        cmd.Parameters.AddWithValue("@Committee", cbCommittee.SelectedItem.ToString());
                    else
                        cmd.Parameters.AddWithValue("@Committee", DBNull.Value);

                    if (cbareaofassingment.Enabled && cbareaofassingment.SelectedItem != null)
                        cmd.Parameters.AddWithValue("@AreaOfAssignment", cbareaofassingment.SelectedItem.ToString());
                    else
                        cmd.Parameters.AddWithValue("@AreaOfAssignment", DBNull.Value);

                    cmd.Parameters.AddWithValue("@Height", txtHeight.Text);
                    cmd.Parameters.AddWithValue("@Bloodtype", txtBloodtype.Text);
                    cmd.Parameters.AddWithValue("@Weight", txtWeight.Text);

                    if (cbTermofposition.Enabled && cbTermofposition.SelectedItem != null)
                        cmd.Parameters.AddWithValue("@Termsofposition", cbTermofposition.SelectedItem.ToString());
                    else
                        cmd.Parameters.AddWithValue("@Termsofposition", DBNull.Value);

                    cmd.Parameters.AddWithValue("@DateElected", dtpDateelected.Value);
                    cmd.Parameters.AddWithValue("@ContactPerson", txtContactperson.Text);
                    cmd.Parameters.AddWithValue("@ContactAddress", txtContactaddress.Text);
                    cmd.Parameters.AddWithValue("@ContactNo", txtContactnumber.Text);
                    cmd.Parameters.AddWithValue("@Relationship", txtRelationship.Text);

                    if (pbOfficialImage.Image != null)
                        cmd.Parameters.AddWithValue("@Image", ImageToByteArray(pbOfficialImage.Image));
                    else
                        cmd.Parameters.AddWithValue("@Image", DBNull.Value); // Set to NULL if no image

                    // Saving Signature
                    if (pBSignature.Image != null)
                        cmd.Parameters.AddWithValue("@Signature", ImageToByteArray(pBSignature.Image));
                    else
                        cmd.Parameters.AddWithValue("@Signature", DBNull.Value); // Set to NULL if no signature

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Record saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetFields();
                        LoadOfficialData();
                    }
                    else
                    {
                        MessageBox.Show("Failed to save the record.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void btnConnect_Click_1(object sender, EventArgs e)
        {
            if (IsCameraAvailable())
            {
                IBIM_Camera_frm cameraForm = new IBIM_Camera_frm(this);
                cameraForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("No camera is available for use.", "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnuploadsignature_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "C:\\";
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        pBSignature.Image = Image.FromFile(filePath);
                        pBSignature.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while uploading the signature: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (dgvofficial.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }
            DataGridViewRow selectedRow = dgvofficial.SelectedRows[0];
            string officialID = selectedRow.Cells["OfficialID"].Value.ToString();
            string fullname = selectedRow.Cells["Fullname"].Value.ToString();

            DialogResult result = MessageBox.Show($"Are you sure you want to delete the record for {fullname}?",
                                                  "Confirm Deletion",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        string query = "DELETE FROM tblofficialinfo WHERE OfficialID = @OfficialID";

                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@OfficialID", officialID);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record deleted successfully.");
                            LoadOfficialData();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the record.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            if (dgvofficial.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row first.");
                return;
            }
            string officialID = dgvofficial.SelectedRows[0].Cells["OfficialID"].Value?.ToString();
            if (string.IsNullOrEmpty(officialID))
            {
                MessageBox.Show("Selected row does not have a valid Official ID.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if the selected Committee already exists in the database
                    if (cbCommittee.SelectedItem != null)
                    {
                        string selectedCommittee = cbCommittee.SelectedItem.ToString();

                        string checkQuery = "SELECT COUNT(*) FROM tblofficialinfo WHERE Committee = @Committee AND OfficialID != @OfficialID";
                        MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@Committee", selectedCommittee);
                        checkCmd.Parameters.AddWithValue("@OfficialID", officialID);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show($"The Committee '{selectedCommittee}' is already assigned to another official. Please choose a different committee.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // Exit the method without updating if the committee already exists
                        }
                    }
                    string query = @"UPDATE tblofficialinfo 
                    SET Fullname = @Fullname, Position = @Position, Birthdate = @Birthdate, Age = @Age, Sex = @Sex, 
                    Birthplace = @Birthplace, CivilStatus = @CivilStatus, Address = @Address, Religion = @Religion, 
                    MobileNo = @MobileNo, Committee = @Committee, AreaOfAssignment = @AreaOfAssignment, Height = @Height, Bloodtype = @Bloodtype, 
                    Weight = @Weight, Termsofposition = @Termsofposition, DateElected = @DateElected, 
                    ContactPerson = @ContactPerson, ContactAddress = @ContactAddress, ContactNo = @ContactNo, 
                    Relationship = @Relationship";

                    if (pbOfficialImage.Image != null)
                    {
                        query += ", Image = @Image";
                    }

                    if (pBSignature.Image != null)
                    {
                        query += ", Signature = @Signature";
                    }

                    query += " WHERE OfficialID = @OfficialID";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@OfficialID", txtOfficialID.Text);
                    cmd.Parameters.AddWithValue("@Fullname", txtFullname.Text);

                    if (cbPosition.SelectedItem != null)
                    {
                        cmd.Parameters.AddWithValue("@Position", cbPosition.SelectedItem.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Please select a row to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    cmd.Parameters.AddWithValue("@Birthdate", dtpBirthdate.Value);
                    cmd.Parameters.AddWithValue("@Age", txtAge.Text);

                    if (cbSex.SelectedItem != null)
                    {
                        cmd.Parameters.AddWithValue("@Sex", cbSex.SelectedItem.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Please select a Sex.");
                        return;
                    }

                    cmd.Parameters.AddWithValue("@Birthplace", txtBirthplace.Text);

                    if (cbCivilstatus.SelectedItem != null)
                    {
                        cmd.Parameters.AddWithValue("@CivilStatus", cbCivilstatus.SelectedItem.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Please select a Civil Status.");
                        return;
                    }

                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);

                    if (cbReligion.SelectedItem != null)
                    {
                        cmd.Parameters.AddWithValue("@Religion", cbReligion.SelectedItem.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Please select a Religion.");
                        return;
                    }

                    cmd.Parameters.AddWithValue("@MobileNo", txtMobilenumber.Text);

                    cmd.Parameters.AddWithValue("@Committee", cbCommittee.Enabled ? (object)cbCommittee.SelectedItem.ToString() : DBNull.Value);

                    cmd.Parameters.AddWithValue("@AreaOfAssignment", cbareaofassingment.Enabled ? (object)cbareaofassingment.SelectedItem.ToString() : DBNull.Value);

                    cmd.Parameters.AddWithValue("@Height", txtHeight.Text);
                    cmd.Parameters.AddWithValue("@Bloodtype", txtBloodtype.Text);
                    cmd.Parameters.AddWithValue("@Weight", txtWeight.Text);

                    if (cbTermofposition.SelectedItem != null)
                    {
                        cmd.Parameters.AddWithValue("@Termsofposition", cbTermofposition.SelectedItem.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Please select row to update data.");
                        return;
                    }

                    cmd.Parameters.AddWithValue("@DateElected", dtpDateelected.Value);
                    cmd.Parameters.AddWithValue("@ContactPerson", txtContactperson.Text);
                    cmd.Parameters.AddWithValue("@ContactAddress", txtContactaddress.Text);
                    cmd.Parameters.AddWithValue("@ContactNo", txtContactnumber.Text);
                    cmd.Parameters.AddWithValue("@Relationship", txtRelationship.Text);

                    if (pbOfficialImage.Image != null)
                    {
                        cmd.Parameters.AddWithValue("@Image", ImageToByteArray(pbOfficialImage.Image));
                    }

                    if (pBSignature.Image != null)
                    {
                        cmd.Parameters.AddWithValue("@Signature", ImageToByteArray(pBSignature.Image));
                    }

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetFields();
                        LoadOfficialData();
                    }
                    else
                    {
                        MessageBox.Show("Update Error. Official ID cannot be Updated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (MySqlException mySqlEx)
            {
                MessageBox.Show("Database error: " + mySqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void btnClear_Click_1(object sender, EventArgs e)
        {
            txtOfficialID.Clear();
            txtFullname.Clear();
            txtAge.Clear();
            txtBirthplace.Clear();
            txtMobilenumber.Clear();
            cbareaofassingment.SelectedIndex = -1;
            txtHeight.Clear();
            txtBloodtype.Clear();
            txtWeight.Clear();
            txtContactperson.Clear();
            txtContactaddress.Clear();
            txtContactnumber.Clear();
            txtRelationship.Clear();

            cbPosition.SelectedIndex = -1;
            cbSex.SelectedIndex = -1;
            cbCivilstatus.SelectedIndex = -1;
            cbReligion.SelectedIndex = -1;
            cbCommittee.SelectedIndex = -1;
            cbTermofposition.SelectedIndex = -1;

            pbOfficialImage.Image = null;

            pBSignature.Image = null;
            btnSave.Enabled = true;
        }

        private void txtSearch_TextChanged_1(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = @"SELECT OfficialID, Fullname, Position 
                             FROM tblofficialinfo 
                             WHERE Fullname LIKE @searchText OR 
                                   Position LIKE @searchText OR 
                                   OfficialID LIKE @searchText";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvofficial.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while searching: " + ex.Message);
            }
        }
    }
}
