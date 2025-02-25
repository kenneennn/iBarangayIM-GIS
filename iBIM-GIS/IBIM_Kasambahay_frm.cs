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
using System.Drawing.Printing;
using AForge.Video;
using AForge.Video.DirectShow;


namespace iBIM_GIS
{
    public partial class IBIM_Kasambahay_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        private Image kasambahayimage;
        public IBIM_Kasambahay_frm()
        {
            InitializeComponent();
            dgvkasambahayrecord.CellClick += new DataGridViewCellEventHandler(dgvkasambahayrecord_CellClick);
        }
         public void SetOfficialImage(Image image)
        {
            kasambahayimage = image;
            pbkasambahayimage.Image = kasambahayimage;
            pbkasambahayimage.Image = image;
        }
        public Size GetKasambahayImageSize()
        {
            return pbkasambahayimage.Size;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string kasambahayName = txtkasambahayname.Text;
            string sex = cbSex.SelectedItem?.ToString();
            string age = txtAge.Text;
            string civilStatus = cbCivilstatus.SelectedItem?.ToString();
            string purok = cbPurok.SelectedItem?.ToString();
            string educationalAttainment = cbEducationalattainment.SelectedItem?.ToString();
            string status = cbStatus.SelectedItem?.ToString();
            string natureOfWork = cbNatureofwork.SelectedItem?.ToString();
            string salary = cbSalary.SelectedItem?.ToString();

            // Check if all fields are filled
            if (string.IsNullOrWhiteSpace(kasambahayName) || string.IsNullOrWhiteSpace(sex) ||
                string.IsNullOrWhiteSpace(age) || string.IsNullOrWhiteSpace(civilStatus) ||
                string.IsNullOrWhiteSpace(educationalAttainment) || string.IsNullOrWhiteSpace(status) ||
                string.IsNullOrWhiteSpace(natureOfWork) || string.IsNullOrWhiteSpace(salary))
            {
                MessageBox.Show("Please fill out all fields before saving.", "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Convert the image to byte array if there is an image selected
            byte[] kasambahayImage = null;
            if (pbkasambahayimage.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    pbkasambahayimage.Image.Save(ms, pbkasambahayimage.Image.RawFormat);
                    kasambahayImage = ms.ToArray();
                }
            }

            // Generate KasambahayID and SQL query
            string kasambahayID = GenerateKasambahayID();
            string query = "INSERT INTO tblkasambahay (KasambahayID, KasambahayName, Sex, Age, CivilStatus, Purok, EducationalAttainment, Status, NatureOfWork, Salary, KasambahayImage) " +
                           "VALUES (@KasambahayID, @KasambahayName, @Sex, @Age, @CivilStatus, @Purok, @EducationalAttainment, @Status, @NatureOfWork, @Salary, @KasambahayImage)";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@KasambahayID", kasambahayID);
                        cmd.Parameters.AddWithValue("@KasambahayName", kasambahayName);
                        cmd.Parameters.AddWithValue("@Sex", sex);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@CivilStatus", civilStatus);
                        cmd.Parameters.AddWithValue("@Purok", purok);
                        cmd.Parameters.AddWithValue("@EducationalAttainment", educationalAttainment);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@NatureOfWork", natureOfWork);
                        cmd.Parameters.AddWithValue("@Salary", salary);

                        // Add the image parameter (use DBNull if no image is provided)
                        cmd.Parameters.AddWithValue("@KasambahayImage", kasambahayImage ?? (object)DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadKasambahayRecords(); // Refresh the DataGridView
                            ClearForm();
                        }
                        else
                        {
                            MessageBox.Show("Failed to save the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateKasambahayID()
        {
            string kasambahayID = "KID001"; // Default value
            string query = "SELECT MAX(CAST(SUBSTRING(KasambahayID, 4) AS UNSIGNED)) FROM tblkasambahay";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            int nextID = Convert.ToInt32(result) + 1; // Increment the highest ID
                            kasambahayID = "KID" + nextID.ToString("D3"); // Format as KID000
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating KasambahayID: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return kasambahayID;
        }
        private void LoadPurokNames()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT `PurokName` FROM tblpurokinfo";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    cbPurok.Items.Clear();
                    while (reader.Read())
                    {
                        cbPurok.Items.Add(reader["PurokName"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void LoadKasambahayRecords()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT KasambahayID, KasambahayName FROM tblkasambahay"; // Only select ID and Name
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvkasambahayrecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvkasambahayrecord.DataSource = dt;

                    // Set row height
                    dgvkasambahayrecord.RowTemplate.Height = 35; // Adjust height as needed

                    // Set header row height
                    dgvkasambahayrecord.ColumnHeadersHeight = 40; // Adjust height as needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgvkasambahayrecord_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvkasambahayrecord.Rows[e.RowIndex];
                string kasambahayID = row.Cells["KasambahayID"].Value.ToString();
                LoadKasambahayDetails(kasambahayID);
            }
        }

        private void LoadKasambahayDetails(string kasambahayID)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM tblkasambahay WHERE KasambahayID = @KasambahayID"; // Query for details
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@KasambahayID", kasambahayID);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Load data into the form fields
                        txtkasambahayname.Text = reader["KasambahayName"].ToString();
                        cbSex.SelectedItem = reader["Sex"].ToString();
                        txtAge.Text = reader["Age"].ToString();
                        cbCivilstatus.SelectedItem = reader["CivilStatus"].ToString();
                        cbPurok.SelectedItem = reader["Purok"].ToString();
                        cbEducationalattainment.SelectedItem = reader["EducationalAttainment"].ToString();
                        cbStatus.SelectedItem = reader["Status"].ToString();
                        cbNatureofwork.SelectedItem = reader["NatureOfWork"].ToString();
                        cbSalary.SelectedItem = reader["Salary"].ToString();

                        // Check if there is image data, and validate the byte array
                        if (reader["KasambahayImage"] != DBNull.Value)
                        {
                            byte[] imageBytes = (byte[])reader["KasambahayImage"];

                            // Ensure the byte array is not empty or invalid
                            if (imageBytes.Length > 0)
                            {
                                try
                                {
                                    using (MemoryStream ms = new MemoryStream(imageBytes))
                                    {
                                        pbkasambahayimage.Image = Image.FromStream(ms);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // If the image is invalid, handle the error gracefully
                                    MessageBox.Show("Error loading image: " + ex.Message, "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    pbkasambahayimage.Image = null; // Clear the PictureBox if image is invalid
                                }
                            }
                            else
                            {
                                // If image byte array is empty, clear the PictureBox
                                pbkasambahayimage.Image = null;
                            }
                        }
                        else
                        {
                            // No image in the database, clear the PictureBox
                            pbkasambahayimage.Image = null;
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Kasambahay_frm_Load(object sender, EventArgs e)
        {
            LoadPurokNames();
            LoadKasambahayRecords();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        private void ClearForm() 
        {
            txtkasambahayname.Clear();
            cbSex.SelectedIndex = -1;
            txtAge.Clear();
            cbCivilstatus.SelectedIndex = -1;
            cbPurok.SelectedIndex = -1;
            cbEducationalattainment.SelectedIndex = -1;
            cbStatus.SelectedIndex = -1;
            cbNatureofwork.SelectedIndex = -1;
            cbSalary.SelectedIndex = -1;
            pbkasambahayimage.Image = null;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvkasambahayrecord.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to delete.", "No Record Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataGridViewRow selectedRow = dgvkasambahayrecord.SelectedRows[0];
            string kasambahayName = selectedRow.Cells["KasambahayName"].Value.ToString(); // Get KasambahayName
            DialogResult confirmResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }
            string query = "DELETE FROM tblkasambahay WHERE KasambahayName = @KasambahayName";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@KasambahayName", kasambahayName);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadKasambahayRecords(); // Refresh the DataGridView
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvkasambahayrecord.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to update.", "No Record Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvkasambahayrecord.SelectedRows[0];
            string oldKasambahayName = selectedRow.Cells["KasambahayName"].Value.ToString(); // Get the original KasambahayName
            string newKasambahayName = txtkasambahayname.Text; // Get the updated KasambahayName from the input field
            string sex = cbSex.SelectedItem?.ToString();
            string age = txtAge.Text;
            string civilStatus = cbCivilstatus.SelectedItem?.ToString();
            string purok = cbPurok.SelectedItem?.ToString();
            string educationalAttainment = cbEducationalattainment.SelectedItem?.ToString();
            string status = cbStatus.SelectedItem?.ToString();
            string natureOfWork = cbNatureofwork.SelectedItem?.ToString();
            string salary = cbSalary.SelectedItem?.ToString();

            // Get the image from the PictureBox
            byte[] kasambahayImage = null;
            if (pbkasambahayimage.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Save the image in a specific format (e.g., PNG or JPEG)
                    pbkasambahayimage.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // You can change this to ImageFormat.Jpeg if needed
                    kasambahayImage = ms.ToArray(); // Convert image to byte array
                }
            }
            else
            {
                // Handle the case where there is no image, possibly by setting a default image or skipping image update
                kasambahayImage = new byte[0]; // This represents an empty image (or NULL if allowed in DB)
            }


            if (string.IsNullOrWhiteSpace(newKasambahayName) || string.IsNullOrWhiteSpace(sex) ||
                string.IsNullOrWhiteSpace(age) || string.IsNullOrWhiteSpace(civilStatus) ||
                string.IsNullOrWhiteSpace(educationalAttainment) || string.IsNullOrWhiteSpace(status) ||
                string.IsNullOrWhiteSpace(natureOfWork) || string.IsNullOrWhiteSpace(salary))
            {
                MessageBox.Show("Please fill out all fields before updating.", "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "UPDATE tblkasambahay SET KasambahayName = @KasambahayName, Sex = @Sex, Age = @Age, " +
                           "CivilStatus = @CivilStatus, Purok = @Purok, EducationalAttainment = @EducationalAttainment, " +
                           "Status = @Status, NatureOfWork = @NatureOfWork, Salary = @Salary, KasambahayImage = @KasambahayImage " +
                           "WHERE KasambahayName = @OldKasambahayName";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@KasambahayName", newKasambahayName);
                        cmd.Parameters.AddWithValue("@OldKasambahayName", oldKasambahayName);
                        cmd.Parameters.AddWithValue("@Sex", sex);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@CivilStatus", civilStatus);
                        cmd.Parameters.AddWithValue("@Purok", purok);
                        cmd.Parameters.AddWithValue("@EducationalAttainment", educationalAttainment);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@NatureOfWork", natureOfWork);
                        cmd.Parameters.AddWithValue("@Salary", salary);

                        // Add the image (it could be empty if there's no image)
                        cmd.Parameters.AddWithValue("@KasambahayImage", kasambahayImage.Length == 0 ? (object)DBNull.Value : kasambahayImage);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadKasambahayRecords();
                            ClearForm();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetBarangayName()
        {
            string barangayName = "Default Barangay Name"; // Default value
            string query = "SELECT BarangayName FROM tblsetbrgyname LIMIT 1";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        barangayName = result.ToString();
                    }
                }
            }
            return barangayName;
        }
        private void rv_KasamabahayReports_Load(object sender, EventArgs e)
        {
            // Load your report data for images
            DataTable tblImageData = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM tblImage", connection))
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(tblImageData);
                }
            }

            // Load your report data for Kasambahay
            DataTable tblkasambahay = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM tblkasambahay", connection))
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(tblkasambahay);
                }
            }

            // Load your report data for Purok-specific details
            DataTable tblpagasa = GetPurokData("Purok Pag-Asa");
            DataTable tblLibis = GetPurokData("Purok Libis");
            DataTable tblMabuhay = GetPurokData("Purok Mabuhay");
            DataTable tblMaligaya = GetPurokData("Purok Maligaya");
            DataTable tblMananao = GetPurokData("Sitio Mananao");
            DataTable tblMabini = GetPurokData("Purok Mabini");
            DataTable tblLiwanag = GetPurokData("Purok Liwanag");

            // Retrieve Chairman and Secretary details
            string punongBarangay = string.Empty;
            string secretary = string.Empty;
            string barangayName = GetBarangayName();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Retrieve Chairman data
                using (MySqlCommand cmd = new MySqlCommand("SELECT FullName FROM tblofficialinfo WHERE Position = 'Chairman'", connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            punongBarangay = reader["FullName"].ToString();
                        }
                    }
                }

                // Retrieve Secretary data
                using (MySqlCommand cmd = new MySqlCommand("SELECT FullName FROM tblofficialinfo WHERE Position = 'Secretary'", connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            secretary = reader["FullName"].ToString();
                        }
                    }
                }
            }

            string reportPath = Path.Combine(Environment.CurrentDirectory, "Report", "KasambahayReport.rdlc");
            rv_KasamabahayReports.LocalReport.ReportPath = reportPath;
            rv_KasamabahayReports.LocalReport.DataSources.Clear();
            rv_KasamabahayReports.LocalReport.DataSources.Add(new ReportDataSource("dsetimage", tblImageData));
            rv_KasamabahayReports.LocalReport.DataSources.Add(new ReportDataSource("dsetkasambahayy", tblkasambahay));
            rv_KasamabahayReports.LocalReport.DataSources.Add(new ReportDataSource("dsetpagasa", tblpagasa));
            rv_KasamabahayReports.LocalReport.DataSources.Add(new ReportDataSource("dsettblLibis", tblLibis));
            rv_KasamabahayReports.LocalReport.DataSources.Add(new ReportDataSource("dsetmabuhay", tblMabuhay));
            rv_KasamabahayReports.LocalReport.DataSources.Add(new ReportDataSource("dsetmaligaya", tblMaligaya));
            rv_KasamabahayReports.LocalReport.DataSources.Add(new ReportDataSource("dsetmananao", tblMananao));
            rv_KasamabahayReports.LocalReport.DataSources.Add(new ReportDataSource("dsetmabini", tblMabini));
            rv_KasamabahayReports.LocalReport.DataSources.Add(new ReportDataSource("dsetliwanag", tblLiwanag));

            int totalKasamabahay = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(KasambahayName) FROM tblkasambahay", connection))
                {
                    totalKasamabahay = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            string monthYearNow = DateTime.Now.ToString("MMMM yyyy");
            string remarks = "Remarks: The Kasambahay records are available for this Month.";
            ReportParameter[] parameters = new ReportParameter[6];
            parameters[0] = new ReportParameter("MonthYearNow", monthYearNow);
            parameters[1] = new ReportParameter("TotalKasamabahay", totalKasamabahay.ToString());
            parameters[2] = new ReportParameter("PunongBarangay", punongBarangay);
            parameters[3] = new ReportParameter("Secretary", secretary);
            parameters[4] = new ReportParameter("Remarks", remarks);
            parameters[5] = new ReportParameter("BarangayName", barangayName);

            rv_KasamabahayReports.LocalReport.SetParameters(parameters);
            rv_KasamabahayReports.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            rv_KasamabahayReports.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;
            rv_KasamabahayReports.RefreshReport();
        }

        private DataTable GetPurokData(string purokName)
        {
            DataTable dataTable = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand($"SELECT * FROM tblkasambahay WHERE Purok = '{purokName}'", connection))
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }


        private void txtKasambahay_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtKasambahay.Text.Trim();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = @"SELECT KasambahayID, KasambahayName
                             FROM tblkasambahay
                             WHERE KasambahayName LIKE @searchText OR  
                                   KasambahayID LIKE @searchText";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvkasambahayrecord.DataSource = dataTable;
                    // Set row height
                    dgvkasambahayrecord.RowTemplate.Height = 35; // Adjust height as needed

                    // Set header row height
                    dgvkasambahayrecord.ColumnHeadersHeight = 40; // Adjust height as needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while searching: " + ex.Message);
            }
        }

        private void dgvkasambahayrecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void UploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pbkasambahayimage.Image = Image.FromFile(openFileDialog.FileName);
                        pbkasambahayimage.SizeMode = PictureBoxSizeMode.StretchImage; // Optional
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while uploading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnconnect_Click(object sender, EventArgs e)
        {
            if (IsCameraAvailable())
            {
                IBIM_KasambahayCamera_frm cameraForm = new IBIM_KasambahayCamera_frm(this);
                cameraForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("No camera is available for use.", "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
