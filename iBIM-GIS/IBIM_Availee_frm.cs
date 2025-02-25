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
using AForge.Video;
using AForge.Video.DirectShow;

namespace iBIM_GIS
{
    public partial class IBIM_Availee_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        private Image availeeimage;
        public IBIM_Availee_frm()
        {
            InitializeComponent();
            cbeducationallevel.SelectedIndexChanged += CbEducationalLevel_SelectedIndexChanged; 
            dgvavaileerecord.CellClick += DgvAvaileeRecord_CellClick;
        }
        public void SetAvaileeImage(Image image)
        {
            availeeimage = image;
            pbavaileeimage.Image = availeeimage;
            pbavaileeimage.Image = image;
        }
        public Size GetAvaileeImageSize()
        {
            return pbavaileeimage.Size;
        }
        private void DgvAvaileeRecord_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string availeeID = dgvavaileerecord.Rows[e.RowIndex].Cells["AvaileeID"].Value.ToString();
                string query = "SELECT * FROM tblavailee WHERE AvaileeID = @AvaileeID";
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@AvaileeID", availeeID);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Load textual data
                                    txtlastname.Text = reader["Lastname"].ToString();
                                    txtfirstname.Text = reader["Firstname"].ToString();
                                    txtmiddlename.Text = reader["Middlename"].ToString();
                                    txtAge.Text = reader["Age"].ToString();
                                    dtpdateofbirth.Value = DateTime.Parse(reader["DateOfBirth"].ToString());
                                    cbSex.SelectedItem = reader["Sex"].ToString();
                                    cbeducationallevel.SelectedItem = reader["EducationalLevel"].ToString();
                                    txtCourse.Text = reader["Course"] != DBNull.Value ? reader["Course"].ToString() : string.Empty;
                                    txtOSY.Text = reader["OSY"].ToString();

                                    // Load image
                                    if (reader["AvaileeImage"] != DBNull.Value)
                                    {
                                        byte[] imageBytes = (byte[])reader["AvaileeImage"];
                                        using (MemoryStream ms = new MemoryStream(imageBytes))
                                        {
                                            pbavaileeimage.Image = Image.FromStream(ms);
                                        }
                                    }
                                    else
                                    {
                                        pbavaileeimage.Image = null;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void CbEducationalLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbeducationallevel.SelectedItem?.ToString() == "COLLEGE")
            {
                txtCourse.Enabled = true; 
            }
            else
            {
                txtCourse.Enabled = false; 
                txtCourse.Clear(); 
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string availeeID = GenerateAvaileeID();
            string lastname = txtlastname.Text;
            string firstname = txtfirstname.Text;
            string middlename = txtmiddlename.Text;
            string age = txtAge.Text;
            string dateOfBirth = dtpdateofbirth.Value.ToString("yyyy-MM-dd");
            string sex = cbSex.SelectedItem?.ToString();
            string educationalLevel = cbeducationallevel.SelectedItem?.ToString();
            string course = educationalLevel == "COLLEGE" ? txtCourse.Text : null;
            string osy = txtOSY.Text;

            if (string.IsNullOrWhiteSpace(lastname) || string.IsNullOrWhiteSpace(firstname) ||
                string.IsNullOrWhiteSpace(middlename) || string.IsNullOrWhiteSpace(age) ||
                string.IsNullOrWhiteSpace(sex) || string.IsNullOrWhiteSpace(educationalLevel) ||
                string.IsNullOrWhiteSpace(osy))
            {
                MessageBox.Show("Please fill out all fields before saving.", "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] imageBytes = null;
            if (pbavaileeimage.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    pbavaileeimage.Image.Save(ms, pbavaileeimage.Image.RawFormat);
                    imageBytes = ms.ToArray();
                }
            }

            string query = "INSERT INTO tblavailee (AvaileeID, Lastname, Firstname, Middlename, Age, DateOfBirth, Sex, EducationalLevel, Course, OSY, AvaileeImage) " +
                           "VALUES (@AvaileeID, @Lastname, @Firstname, @Middlename, @Age, @DateOfBirth, @Sex, @EducationalLevel, @Course, @OSY, @AvaileeImage)";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AvaileeID", availeeID);
                        cmd.Parameters.AddWithValue("@Lastname", lastname);
                        cmd.Parameters.AddWithValue("@Firstname", firstname);
                        cmd.Parameters.AddWithValue("@Middlename", middlename);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                        cmd.Parameters.AddWithValue("@Sex", sex);
                        cmd.Parameters.AddWithValue("@EducationalLevel", educationalLevel);
                        cmd.Parameters.AddWithValue("@Course", course ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@OSY", osy);
                        cmd.Parameters.AddWithValue("@AvaileeImage", imageBytes ?? (object)DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearInputFields();
                            LoadAvaileeRecords();
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

        private void LoadAvaileeRecords()
        {
            string query = "SELECT AvaileeID, CONCAT(Lastname, ', ', Firstname, ' ', Middlename) AS AvaileeName FROM tblavailee";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            dgvavaileerecord.DataSource = dataTable;
                            dgvavaileerecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dgvavaileerecord.Columns["AvaileeID"].HeaderText = "Availee ID";
                            dgvavaileerecord.Columns["AvaileeName"].HeaderText = "Availee Name";
                            dgvavaileerecord.RowTemplate.Height = 35;
                            dgvavaileerecord.ColumnHeadersHeight = 40;
                        }
                    } }}
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GenerateAvaileeID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd"); 
            string uniquePart = new Random().Next(1000, 9999).ToString(); 
            return $"AVA-{datePart}-{uniquePart}";
        }
        private void ClearInputFields()
        {
            txtlastname.Clear();
            txtfirstname.Clear();
            txtmiddlename.Clear();
            txtAge.Clear(); 
            dtpdateofbirth.Value = DateTime.Now; 
            cbSex.SelectedIndex = -1; 
            cbeducationallevel.SelectedIndex = -1; 
            txtCourse.Clear();
            txtOSY.Clear();
            pbavaileeimage.Image = null;
        }
        private void DtpDateOfBirth_ValueChanged(object sender, EventArgs e)
        {
            DateTime dateOfBirth = dtpdateofbirth.Value;
            int age = CalculateAge(dateOfBirth);
            txtAge.Text = age.ToString();
        }
        private int CalculateAge(DateTime dateOfBirth)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
        private void Availees_Load(object sender, EventArgs e)
        {
            dtpdateofbirth.ValueChanged += DtpDateOfBirth_ValueChanged;
            txtAge.ReadOnly = true;
            LoadAvaileeRecords();
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvavaileerecord.CurrentRow == null)
            {
                MessageBox.Show("Please select a record to update.", "No Record Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string availeeID = dgvavaileerecord.CurrentRow.Cells["AvaileeID"].Value.ToString();
            string lastname = txtlastname.Text;
            string firstname = txtfirstname.Text;
            string middlename = txtmiddlename.Text;
            string age = txtAge.Text;
            string dateOfBirth = dtpdateofbirth.Value.ToString("yyyy-MM-dd");
            string sex = cbSex.SelectedItem?.ToString();
            string educationalLevel = cbeducationallevel.SelectedItem?.ToString();
            string course = educationalLevel == "COLLEGE" ? txtCourse.Text : null;
            string osy = txtOSY.Text;
            if (string.IsNullOrWhiteSpace(lastname) || string.IsNullOrWhiteSpace(firstname) ||
                string.IsNullOrWhiteSpace(middlename) || string.IsNullOrWhiteSpace(age) ||
                string.IsNullOrWhiteSpace(sex) || string.IsNullOrWhiteSpace(educationalLevel) ||
                string.IsNullOrWhiteSpace(osy))
            {
                MessageBox.Show("Please fill out all fields before updating.", "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string query = "UPDATE tblavailee SET Lastname = @Lastname, Firstname = @Firstname, Middlename = @Middlename, " +
                           "Age = @Age, DateOfBirth = @DateOfBirth, Sex = @Sex, EducationalLevel = @EducationalLevel, " +
                           "Course = @Course, OSY = @OSY WHERE AvaileeID = @AvaileeID";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AvaileeID", availeeID);
                        cmd.Parameters.AddWithValue("@Lastname", lastname);
                        cmd.Parameters.AddWithValue("@Firstname", firstname);
                        cmd.Parameters.AddWithValue("@Middlename", middlename);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                        cmd.Parameters.AddWithValue("@Sex", sex);
                        cmd.Parameters.AddWithValue("@EducationalLevel", educationalLevel);
                        if (course != null)
                            cmd.Parameters.AddWithValue("@Course", course);
                        else
                            cmd.Parameters.AddWithValue("@Course", DBNull.Value);

                        cmd.Parameters.AddWithValue("@OSY", osy);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearInputFields();
                            LoadAvaileeRecords();
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
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputFields();
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvavaileerecord.CurrentRow == null)
            {
                MessageBox.Show("Please select a record to update.", "No Record Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string availeeID = dgvavaileerecord.CurrentRow.Cells["AvaileeID"].Value.ToString();
            string lastname = txtlastname.Text;
            string firstname = txtfirstname.Text;
            string middlename = txtmiddlename.Text;
            string age = txtAge.Text;
            string dateOfBirth = dtpdateofbirth.Value.ToString("yyyy-MM-dd");
            string sex = cbSex.SelectedItem?.ToString();
            string educationalLevel = cbeducationallevel.SelectedItem?.ToString();
            string course = educationalLevel == "COLLEGE" ? txtCourse.Text : null;
            string osy = txtOSY.Text;

            if (string.IsNullOrWhiteSpace(lastname) || string.IsNullOrWhiteSpace(firstname) ||
                string.IsNullOrWhiteSpace(middlename) || string.IsNullOrWhiteSpace(age) ||
                string.IsNullOrWhiteSpace(sex) || string.IsNullOrWhiteSpace(educationalLevel) ||
                string.IsNullOrWhiteSpace(osy))
            {
                MessageBox.Show("Please fill out all fields before updating.", "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            byte[] availeeImage = null;
            if (pbavaileeimage.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    pbavaileeimage.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    availeeImage = ms.ToArray();
                }
            }
            string query = "UPDATE tblavailee SET Lastname = @Lastname, Firstname = @Firstname, Middlename = @Middlename, " +
                           "Age = @Age, DateOfBirth = @DateOfBirth, Sex = @Sex, EducationalLevel = @EducationalLevel, " +
                           "Course = @Course, OSY = @OSY, AvaileeImage = @AvaileeImage WHERE AvaileeID = @AvaileeID";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AvaileeID", availeeID);
                        cmd.Parameters.AddWithValue("@Lastname", lastname);
                        cmd.Parameters.AddWithValue("@Firstname", firstname);
                        cmd.Parameters.AddWithValue("@Middlename", middlename);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                        cmd.Parameters.AddWithValue("@Sex", sex);
                        cmd.Parameters.AddWithValue("@EducationalLevel", educationalLevel);
                        cmd.Parameters.AddWithValue("@Course", course ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@OSY", osy);
                        cmd.Parameters.AddWithValue("@AvaileeImage", availeeImage.Length == 0 ? (object)DBNull.Value : availeeImage);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearInputFields();
                            LoadAvaileeRecords();
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

        private void rv_AvaileeReports_Load(object sender, EventArgs e)
        {
            LoadAvaileeReport();
        }
        private void LoadAvaileeReport()
        {
            try
            {
                rv_AvaileeReports.Reset();
                rv_AvaileeReports.ProcessingMode = ProcessingMode.Local;
                string reportPath = Path.Combine(Environment.CurrentDirectory, "Report", "AvaileeReport.rdlc");
                if (!File.Exists(reportPath))
                {
                    MessageBox.Show("Report file not found: " + reportPath, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string punongBarangay = string.Empty;
                string secretary = string.Empty;
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
                rv_AvaileeReports.LocalReport.ReportPath = reportPath;
                DataTable reportData = GetAvaileeData();
                ReportDataSource rds = new ReportDataSource("dsetavailee", reportData);
                rv_AvaileeReports.LocalReport.DataSources.Clear();
                rv_AvaileeReports.LocalReport.DataSources.Add(rds);
                string barangayName = GetBarangayName();
                string monthYear = DateTime.Now.ToString("MMMM yyyy");
                string formattedDateNow = DateTime.Now.ToString("MMMM dd, yyyy");
                ReportParameter[] parameters = new ReportParameter[]
                {
            new ReportParameter("PunongBarangay", punongBarangay),
            new ReportParameter("Secretary", secretary),
            new ReportParameter("BarangayName", barangayName),
            new ReportParameter("MonthYear", monthYear),
            new ReportParameter("DateNow", formattedDateNow) 
                };
                rv_AvaileeReports.LocalReport.SetParameters(parameters);

                rv_AvaileeReports.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                rv_AvaileeReports.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;
                rv_AvaileeReports.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private Dictionary<string, string> GetOfficialInfo()
        {
            Dictionary<string, string> officialInfo = new Dictionary<string, string>();

            string query = "SELECT FullName, Signature, Position FROM tblofficialinfo WHERE Position IN ('Chairman', 'Secretary')";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string position = reader["Position"].ToString();
                            string fullName = reader["FullName"].ToString();
                            byte[] signatureBytes = reader["Signature"] as byte[];

                            if (position == "Chairman")
                            {
                                officialInfo["PunongBarangay"] = fullName;
                                officialInfo["ChairmanSignature"] = signatureBytes != null ? Convert.ToBase64String(signatureBytes) : string.Empty;
                            }
                            else if (position == "Secretary")
                            {
                                officialInfo["Secretary"] = fullName;
                                officialInfo["SecretarySignature"] = signatureBytes != null ? Convert.ToBase64String(signatureBytes) : string.Empty;
                            }
                        }
                    }
                }
            }

            return officialInfo;
        }

        private DataTable GetAvaileeData()
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM tblavailee"; // Replace with your actual query

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    } }}
            return dt;
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
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtSearch.Text.Trim();
            LoadAvaileeRecords(searchValue);
        }
        private void LoadAvaileeRecords(string searchValue = "")
        {
            string query = "SELECT AvaileeID, CONCAT(Lastname, ', ', Firstname, ' ', Middlename) AS AvaileeName FROM tblavailee";

            // Add filter condition if searchValue is not empty
            if (!string.IsNullOrEmpty(searchValue))
            {
                query += " WHERE AvaileeID LIKE @Search OR CONCAT(Lastname, ', ', Firstname, ' ', Middlename) LIKE @Search";
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(searchValue))
                        {
                            cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            dgvavaileerecord.DataSource = dataTable;
                            dgvavaileerecord.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dgvavaileerecord.Columns["AvaileeID"].HeaderText = "Availee ID";
                            dgvavaileerecord.Columns["AvaileeName"].HeaderText = "Availee Name";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnconnect_Click(object sender, EventArgs e)
        {
            if (IsCameraAvailable())
            {
                IBIM_AvaileeCamera_frm cameraForm = new IBIM_AvaileeCamera_frm(this);
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
        private void UploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pbavaileeimage.Image = Image.FromFile(openFileDialog.FileName);
                        pbavaileeimage.SizeMode = PictureBoxSizeMode.StretchImage; // Optional
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while uploading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
   