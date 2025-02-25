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
    public partial class IBIM_CleanupDrive_frm : Form
    {

        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        int imageUploadIndex = 1;
        public IBIM_CleanupDrive_frm()
        {
            InitializeComponent();
            dgvcleanupdrive.CellClick += dgvcleanupdrive_CellClick;
            dgvbarco.CellClick += dgvbarco_CellClick;
            txtsearchbyweekno.TextChanged += txtsearchbyweekno_TextChanged;

        }
        private void txtsearchbyweekno_TextChanged(object sender, EventArgs e)
        {
            SearchByWeekNo(txtsearchbyweekno.Text);
        }
        private void SearchByWeekNo(string weekNo)
        {
            try
            {
                string query = @"SELECT CleanupDriveID, WeekNo 
                         FROM tblcleanupdrive 
                         WHERE WeekNo LIKE @WeekNo";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@WeekNo", "%" + weekNo + "%");

                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvcleanupdrive.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dgvcleanupdrive.DataSource = dt;
                        dgvcleanupdrive.RowTemplate.Height = 35; // Adjust height as needed

                        // Set header row height
                        dgvcleanupdrive.ColumnHeadersHeight = 40; // Adjust height as needed
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while searching: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgvcleanupdrive_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure it's not a header row
            {
                // Get the CleanupDriveID from the selected row
                string selectedID = dgvcleanupdrive.Rows[e.RowIndex].Cells["CleanupDriveID"].Value.ToString();

                // Load and display details for the selected CleanupDriveID
                LoadCleanupDriveDetails(selectedID);
                // Get the WeekNo from the clicked row
                string weekNo = dgvcleanupdrive.Rows[e.RowIndex].Cells["WeekNo"].Value.ToString();

                // Load and display the report for the selected WeekNo
                LoadReportForWeekNo(weekNo);
            }
        }
        private void LoadReportForWeekNo(string weekNo)
        {
            try
            {
                // Define queries
                string queryCleanupDrive = @"SELECT CleanupDriveID, WeekNo, NumberofPax, NoofBO, NoofBarangayPersonel, GarbageCollected, Image1, Image2, Image3, Image4, Image5, Image6
                                     FROM tblcleanupdrive
                                     WHERE WeekNo = @WeekNo";
                string queryImages = "SELECT * FROM tblimage";
                string queryBarangayName = "SELECT * FROM tblsetbrgyname";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // Prepare datasets
                    DataSet dsetcleaupdrive = new DataSet();
                    DataSet dsetimage = new DataSet();
                    DataSet dsetbarangayname = new DataSet();

                    // Fetch CleanupDrive data
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(queryCleanupDrive, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@WeekNo", weekNo);
                        adapter.Fill(dsetcleaupdrive, "tblcleanupdrive");
                    }

                    // Fetch Image data
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(queryImages, conn))
                    {
                        adapter.Fill(dsetimage, "tblimage");
                    }

                    // Fetch Barangay Name data
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(queryBarangayName, conn))
                    {
                        adapter.Fill(dsetbarangayname, "tblsetbrgyname");
                    }

                    // Load the RDLC report
                    string reportPath = Path.Combine(Environment.CurrentDirectory, "Report", "CleanUpDriveReport.rdlc");
                    rv_CLEANUPDRIVEReports.LocalReport.ReportPath = reportPath;

                    // Set datasets for the report
                    rv_CLEANUPDRIVEReports.LocalReport.DataSources.Clear();
                    rv_CLEANUPDRIVEReports.LocalReport.DataSources.Add(new ReportDataSource("dsetcleaupdrive", dsetcleaupdrive.Tables["tblcleanupdrive"]));
                    rv_CLEANUPDRIVEReports.LocalReport.DataSources.Add(new ReportDataSource("dsetimage", dsetimage.Tables["tblimage"]));
                    rv_CLEANUPDRIVEReports.LocalReport.DataSources.Add(new ReportDataSource("dsetbarangayname", dsetbarangayname.Tables["tblsetbrgyname"]));

                    rv_CLEANUPDRIVEReports.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    rv_CLEANUPDRIVEReports.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;
                    rv_CLEANUPDRIVEReports.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCleanupDriveDetails(string cleanupDriveID)
        {
            try
            {
                string query = @"
            SELECT WeekNo, NumberofPax, NoofBO, NoofBarangayPersonel, GarbageCollected, Image1, Image2, Image3, Image4, Image5,Image6   
            FROM tblcleanupdrive 
            WHERE CleanupDriveID = @CleanupDriveID";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CleanupDriveID", cleanupDriveID);

                        conn.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            // Populate the fields with data from the database
                            txtweeknumber.Text = reader["WeekNo"].ToString();
                            txtnumberofpax.Text = reader["NumberofPax"].ToString();
                            txtnumberofbo.Text = reader["NoofBO"].ToString();
                            txttotalpersonel.Text = reader["NoofBarangayPersonel"].ToString();
                            txtgarbagecolleted.Text = reader["GarbageCollected"].ToString();

                            // Load images if they are not null in the database
                            pbImage1.Image = ByteArrayToImage(reader["Image1"] as byte[]);
                            pbImage2.Image = ByteArrayToImage(reader["Image2"] as byte[]);
                            pbImage3.Image = ByteArrayToImage(reader["Image3"] as byte[]);
                            pbImage4.Image = ByteArrayToImage(reader["Image4"] as byte[]);
                            pbImage5.Image = ByteArrayToImage(reader["Image5"] as byte[]);
                            pbImage6.Image = ByteArrayToImage(reader["Image6"] as byte[]);
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading the details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private Image ByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return null;

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }
        private void IBIM_CleanupDrive_frm_Load(object sender, EventArgs e)
        {

            LoadCleanupDriveData();
            LoadBarcoData();
        }

        private void LoadCleanupDriveData()
        {
            try
            {
                string query = "SELECT CleanupDriveID, WeekNo FROM tblcleanupdrive";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvcleanupdrive.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        dgvcleanupdrive.DataSource = dt;
                        // Set row height
                        dgvcleanupdrive.RowTemplate.Height = 35; // Adjust height as needed

                        // Set header row height
                        dgvcleanupdrive.ColumnHeadersHeight = 40; // Adjust height as needed
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnuploadimage1_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Image selectedImage = Image.FromFile(openFileDialog.FileName);

                        // Determine which PictureBox to update based on the counter
                        switch (imageUploadIndex)
                        {
                            case 1:
                                pbImage1.Image = selectedImage;
                                pbImage1.SizeMode = PictureBoxSizeMode.StretchImage; // Optional
                                break;
                            case 2:
                                pbImage2.Image = selectedImage;
                                pbImage2.SizeMode = PictureBoxSizeMode.StretchImage;
                                break;
                            case 3:
                                pbImage3.Image = selectedImage;
                                pbImage3.SizeMode = PictureBoxSizeMode.StretchImage;
                                break;
                            case 4:
                                pbImage4.Image = selectedImage;
                                pbImage4.SizeMode = PictureBoxSizeMode.StretchImage;
                                break;
                            case 5:
                                pbImage5.Image = selectedImage;
                                pbImage5.SizeMode = PictureBoxSizeMode.StretchImage;
                                break;
                            case 6:
                                pbImage6.Image = selectedImage;
                                pbImage6.SizeMode = PictureBoxSizeMode.StretchImage;
                                break;
                            default:
                                MessageBox.Show("All PictureBoxes are already filled!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return; // Exit if all PictureBoxes are filled
                        }

                        // Increment the counter for the next PictureBox
                        imageUploadIndex++;

                        // Reset the counter if it exceeds the number of PictureBoxes
                        if (imageUploadIndex > 6)
                        {
                            imageUploadIndex = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while uploading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate that required fields are filled out (adjust as necessary)
                if (string.IsNullOrWhiteSpace(txtweeknumber.Text) ||
                    string.IsNullOrWhiteSpace(txtnumberofpax.Text) ||
                    string.IsNullOrWhiteSpace(txtnumberofbo.Text) ||
                    string.IsNullOrWhiteSpace(txttotalpersonel.Text) ||
                    string.IsNullOrWhiteSpace(txtgarbagecolleted.Text))
                {
                    MessageBox.Show("Please fill out all fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Convert the images to byte arrays
                byte[] image1Bytes = ImageToByteArray(pbImage1.Image);
                byte[] image2Bytes = ImageToByteArray(pbImage2.Image);
                byte[] image3Bytes = ImageToByteArray(pbImage3.Image);
                byte[] image4Bytes = ImageToByteArray(pbImage4.Image);
                byte[] image5Bytes = ImageToByteArray(pbImage5.Image);
                byte[] image6Bytes = ImageToByteArray(pbImage6.Image);

                // Generate the next CleanupDriveID
                string cleanupDriveID = GenerateCleanupDriveID();

                // Create the insert query
                string query = @"
            INSERT INTO tblcleanupdrive (CleanupDriveID, WeekNo, NumberofPax, NoofBO, NoofBarangayPersonel, GarbageCollected, Image1, Image2, Image3, Image4,Image5,Image6)
            VALUES (@CleanupDriveID, @WeekNo, @NumberofPax, @NoofBO, @NoofBarangayPersonel, @GarbageCollected, @Image1, @Image2, @Image3, @Image4,@Image5,@Image6)";

                // Create a MySqlConnection and MySqlCommand
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CleanupDriveID", cleanupDriveID);
                        cmd.Parameters.AddWithValue("@WeekNo", txtweeknumber.Text);
                        cmd.Parameters.AddWithValue("@NumberofPax", txtnumberofpax.Text);
                        cmd.Parameters.AddWithValue("@NoofBO", txtnumberofbo.Text);
                        cmd.Parameters.AddWithValue("@NoofBarangayPersonel", txttotalpersonel.Text);
                        cmd.Parameters.AddWithValue("@GarbageCollected", txtgarbagecolleted.Text);
                        cmd.Parameters.AddWithValue("@Image1", image1Bytes);
                        cmd.Parameters.AddWithValue("@Image2", image2Bytes);
                        cmd.Parameters.AddWithValue("@Image3", image3Bytes);
                        cmd.Parameters.AddWithValue("@Image4", image4Bytes);
                        cmd.Parameters.AddWithValue("@Image5", image5Bytes);
                        cmd.Parameters.AddWithValue("@Image6", image6Bytes);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Cleanup drive information saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CLearForm();
                        LoadCleanupDriveData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving the data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
        private string GenerateCleanupDriveID()
        {
            string lastID = GetLastCleanupDriveID();
            int newIDNumber = 1;

            if (!string.IsNullOrEmpty(lastID))
            {
                string numberPart = lastID.Substring(4);  // "CDID" part is 4 characters
                if (int.TryParse(numberPart, out int lastIDNumber))
                {
                    newIDNumber = lastIDNumber + 1; // Increment the number
                }
            }
            return "CDID" + newIDNumber.ToString("D3"); // Ensure the ID is in the "CDID001" format
        }
        private string GetLastCleanupDriveID()
        {
            string lastID = string.Empty;

            string query = "SELECT CleanupDriveID FROM tblcleanupdrive ORDER BY CleanupDriveID DESC LIMIT 1";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lastID = reader.GetString(0); // Fetch the last CleanupDriveID
                    }
                    conn.Close();
                }
            }

            return lastID;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvcleanupdrive.CurrentRow != null)
            {
                string cleanupDriveID = dgvcleanupdrive.CurrentRow.Cells["CleanupDriveID"].Value.ToString();

                DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection conn = new MySqlConnection(connectionString))
                        {
                            string query = "DELETE FROM tblcleanupdrive WHERE CleanupDriveID = @CleanupDriveID";
                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@CleanupDriveID", cleanupDriveID);
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                                MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadCleanupDriveData();
                                CLearForm();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while deleting the record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvcleanupdrive.CurrentRow != null)
            {
                string cleanupDriveID = dgvcleanupdrive.CurrentRow.Cells["CleanupDriveID"].Value.ToString();

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        string query = @"UPDATE tblcleanupdrive 
                        SET WeekNo = @WeekNo, 
                            NumberofPax = @NumberofPax, 
                            NoofBO = @NoofBO, 
                            NoofBarangayPersonel = @NoofBarangayPersonel, 
                            GarbageCollected = @GarbageCollected,
                            Image1 = @Image1,
                            Image2 = @Image2,
                            Image3 = @Image3,
                            Image4 = @Image4,
                            Image5 = @Image5,
                            Image6 = @Image6
                        WHERE CleanupDriveID = @CleanupDriveID";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@CleanupDriveID", cleanupDriveID);
                            cmd.Parameters.AddWithValue("@WeekNo", txtweeknumber.Text);
                            cmd.Parameters.AddWithValue("@NumberofPax", txtnumberofpax.Text);
                            cmd.Parameters.AddWithValue("@NoofBO", txtnumberofbo.Text);
                            cmd.Parameters.AddWithValue("@NoofBarangayPersonel", txttotalpersonel.Text);
                            cmd.Parameters.AddWithValue("@GarbageCollected", txtgarbagecolleted.Text);

                            // Check and convert images only if they are not null
                            cmd.Parameters.AddWithValue("@Image1", ConvertImageToByteArray(pbImage1));
                            cmd.Parameters.AddWithValue("@Image2", ConvertImageToByteArray(pbImage2));
                            cmd.Parameters.AddWithValue("@Image3", ConvertImageToByteArray(pbImage3));
                            cmd.Parameters.AddWithValue("@Image4", ConvertImageToByteArray(pbImage4));
                            cmd.Parameters.AddWithValue("@Image5", ConvertImageToByteArray(pbImage5));
                            cmd.Parameters.AddWithValue("@Image6", ConvertImageToByteArray(pbImage6));

                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            MessageBox.Show("Record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadCleanupDriveData();
                            CLearForm();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while updating the record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private byte[] ConvertImageToByteArray(PictureBox pictureBox)
        {
            if (pictureBox.Image == null)
            {
                return null; // or you can return an empty byte array depending on your database design
            }
            using (MemoryStream ms = new MemoryStream())
            {
                pictureBox.Image.Save(ms, pictureBox.Image.RawFormat);
                return ms.ToArray();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            CLearForm();
        }
        private void CLearForm()
        {
            {
                txtweeknumber.Clear();
                txtnumberofpax.Clear();
                txtnumberofbo.Clear();
                txttotalpersonel.Clear();
                txtgarbagecolleted.Clear();
                pbImage1.Image = null;
                pbImage2.Image = null;
                pbImage3.Image = null;
                pbImage4.Image = null;
                pbImage5.Image = null;
                pbImage6.Image = null;
            }
        }
        private void btnuploadimage5_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pbImage5.Image = Image.FromFile(openFileDialog.FileName);
                        pbImage5.SizeMode = PictureBoxSizeMode.StretchImage; // Optional
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while uploading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnuploadimage6_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pbImage6.Image = Image.FromFile(openFileDialog.FileName);
                        pbImage6.SizeMode = PictureBoxSizeMode.StretchImage; // Optional
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while uploading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnsaveBarco_Click(object sender, EventArgs e)
        {
            // Validate that all fields (except the image fields) are filled out
            if (string.IsNullOrEmpty(cbmonth.Text) ||
                string.IsNullOrEmpty(cbconductRCO.Text) ||
                string.IsNullOrEmpty(txtStreet.Text) ||
                string.IsNullOrEmpty(txtroadlength.Text) ||
                string.IsNullOrEmpty(txtactiontaken.Text) ||
                string.IsNullOrEmpty(txtremarks.Text))
            {
                MessageBox.Show("Please fill out all fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Exit the method if any required field is empty
            }

            string newBarcoID = GenerateBarcoID();

            // Get values from the form fields
            string month = cbmonth.Text;
            string conductedRco = cbconductRCO.Text;
            string location = txtStreet.Text;
            string roadLength = txtroadlength.Text;
            DateTime dateOfClearingOperation = dtpdateofclearingoperation.Value;
            string actionTaken = txtactiontaken.Text;
            string remarks = txtremarks.Text;
            string noOfBO = txtnoofBO.Text;
            string skoff = txttotalOff.Text;
            string totalPersonnel = txttotalpersonnel.Text;

            // Initialize image fields as null (if no image is provided)
            byte[] image1 = null;
            byte[] image2 = null;
            byte[] image3 = null;

            // Check if image fields are not null or empty, then retrieve their byte arrays
            if (Image1.Image != null)
            {
                image1 = GetImageBytes(Image1.Image);
            }

            if (Image2.Image != null)
            {
                image2 = GetImageBytes(Image2.Image);
            }

            if (Image3.Image != null)
            {
                image3 = GetImageBytes(Image3.Image);
            }

            // SQL Insert command
            string query = "INSERT INTO tblbarco (BarcoID, Month, ConductedRCo, Location, RoadLength, DateofClearingOperation, ActionTaken, Remarks, NoofBO, TotalPersonel,TotalSKOfficial, Image1, Image2, Image3) " +
                           "VALUES (@BarcoID, @Month, @ConductedRCo, @Location, @RoadLength, @DateofClearingOperation, @ActionTaken, @Remarks, @NoofBO, @TotalPersonel,@TotalSKOfficial, @Image1, @Image2, @Image3)";

            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=dbiBIM;User=root;Password=;"))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@BarcoID", newBarcoID);
                        cmd.Parameters.AddWithValue("@Month", month);
                        cmd.Parameters.AddWithValue("@ConductedRCo", conductedRco);
                        cmd.Parameters.AddWithValue("@Location", location);
                        cmd.Parameters.AddWithValue("@RoadLength", roadLength);
                        cmd.Parameters.AddWithValue("@DateofClearingOperation", dateOfClearingOperation);
                        cmd.Parameters.AddWithValue("@ActionTaken", actionTaken);
                        cmd.Parameters.AddWithValue("@Remarks", remarks);
                        cmd.Parameters.AddWithValue("@NoofBO", noOfBO);
                        cmd.Parameters.AddWithValue("@TotalPersonel", totalPersonnel);
                        cmd.Parameters.AddWithValue("@TotalSKOfficial", skoff);
                        cmd.Parameters.AddWithValue("@Image1", (object)image1 ?? DBNull.Value); // Handle null images
                        cmd.Parameters.AddWithValue("@Image2", (object)image2 ?? DBNull.Value); // Handle null images
                        cmd.Parameters.AddWithValue("@Image3", (object)image3 ?? DBNull.Value); // Handle null images

                        // Execute command
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Data saved successfully.");
                        LoadBarcoData();
                        ResetFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void ResetFields()
        {
            cbmonth.SelectedIndex = -1; // Reset ComboBox
            cbconductRCO.SelectedIndex = -1; // Reset ComboBox
            txtStreet.Clear(); // Clear TextBox
            txtroadlength.Clear(); // Clear TextBox
            dtpdateofclearingoperation.Value = DateTime.Now; // Reset DateTimePicker to current date
            txtactiontaken.Clear(); // Clear TextBox
            txtremarks.Clear(); // Clear TextBox
            txtnoofBO.Clear(); // Clear TextBox
            txttotalpersonnel.Clear(); // Clear TextBox
            txttotalOff.Clear(); // Clear TextBox

            // Reset PictureBoxes
            Image1.Image = null;
            Image2.Image = null;
            Image3.Image = null;
        }
        private string GenerateBarcoID()
        {
            string latestID = "";
            string newID = "BarcoID001"; // Default if there are no records

            string query = "SELECT BarcoID FROM tblbarco ORDER BY BarcoID DESC LIMIT 1";

            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=dbiBIM;User=root;Password=;"))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            latestID = result.ToString();
                            int idNumber = int.Parse(latestID.Substring(7)) + 1;
                            newID = "BarcoID" + idNumber.ToString("D3");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while generating BarcoID: " + ex.Message);
                }
            }

            return newID;
        }

        // Helper function to convert an image to byte array
        private byte[] GetImageBytes(Image img)
        {
            if (img == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private void btnUpload1_Click(object sender, EventArgs e)
        {
            switch (imageUploadIndex)
            {
                case 1:
                    UploadImageToPictureBox(Image1);
                    break;
                case 2:
                    UploadImageToPictureBox(Image2);
                    break;
                case 3:
                    UploadImageToPictureBox(Image3);
                    break;
                default:
                    MessageBox.Show("All PictureBoxes are already filled!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return; // Exit if all PictureBoxes are filled
            }

            // Increment the counter for the next PictureBox
            imageUploadIndex++;

            // Reset the counter if it exceeds the number of PictureBoxes
            if (imageUploadIndex > 3)
            {
                imageUploadIndex = 1;
            }
        }

        private void UploadImageToPictureBox(PictureBox pictureBox)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox.Image = Image.FromFile(openFileDialog.FileName);
                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage; // Optional for resizing
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while uploading the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadBarcoData()
        {
            string query = "SELECT DISTINCT Month FROM tblbarco WHERE YEAR(DateofClearingOperation) = @CurrentYear";

            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=dbiBIM;User=root;Password=;"))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Set the current year as a parameter to fetch only current year's data
                        cmd.Parameters.AddWithValue("@CurrentYear", DateTime.Now.Year);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dgvbarco.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dgvbarco.DataSource = dt;
                            // Set row height
                            dgvbarco.RowTemplate.Height = 35; // Adjust height as needed

                            // Set header row height
                            dgvbarco.ColumnHeadersHeight = 40; // Adjust height as needed
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvbarco_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected row and the 'Month' value from the selected row
                DataGridViewRow selectedRow = dgvbarco.Rows[e.RowIndex];
                var selectedMonthCell = selectedRow.Cells["Month"];

                // Check if the 'Month' cell value is DBNull or empty
                if (selectedMonthCell.Value == DBNull.Value || string.IsNullOrEmpty(selectedMonthCell.Value.ToString()))
                {
                    // Show an error message if the Month is DBNull or empty
                    MessageBox.Show("The selected record does not have a valid Month value. Please select a valid record.", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit the method if Month value is invalid
                }

                // Get the selected month value
                string selectedMonth = selectedMonthCell.Value.ToString();

                // Load the report based on the selected month
                LoadReport(selectedMonth);
            }
        }
        private void btndeleteBarco_Click(object sender, EventArgs e)
        {
            // Check if a row is selected in the DataGridView
            if (dgvbarco.SelectedRows.Count > 0)
            {
                // Get the selected row and the Month value from the 'Month' column
                var selectedMonthCell = dgvbarco.SelectedRows[0].Cells["Month"];

                // Check if the selected month is DBNull (null in database)
                if (selectedMonthCell.Value == DBNull.Value || string.IsNullOrEmpty(selectedMonthCell.Value.ToString()))
                {
                    MessageBox.Show("The selected record has no valid Month value. Please select a valid record.", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit the method if the Month value is DBNull or empty
                }

                string selectedMonth = selectedMonthCell.Value.ToString();

                // Confirm deletion
                var confirmResult = MessageBox.Show("Are you sure you want to delete all records for the selected month?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    // SQL Delete command, using 'Month' column to filter
                    string query = "DELETE FROM tblbarco WHERE Month = @Month";

                    using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=dbiBIM;User=root;Password=;"))
                    {
                        try
                        {
                            conn.Open();
                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                // Use the selected month as a parameter
                                cmd.Parameters.AddWithValue("@Month", selectedMonth);

                                // Execute the command
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Records for the selected month deleted successfully.");
                                    LoadBarcoData();  // Reload data after deletion
                                }
                                else
                                {
                                    MessageBox.Show("No records found for the selected month.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a record to delete.");
            }
        }
        private void btnnewBarco_Click(object sender, EventArgs e)
        {
            // Clear all input fields
            cbmonth.Text = string.Empty;
            cbconductRCO.Text = string.Empty;
            txtStreet.Clear();
            txtroadlength.Clear();
            dtpdateofclearingoperation.Value = DateTime.Now; // Reset to current date
            txtactiontaken.Clear();
            txtremarks.Clear();
            txtnoofBO.Clear();
            txttotalpersonnel.Clear();
            txttotalOff.Clear();

            // Clear images
            Image1.Image = null;
            Image2.Image = null;
            Image3.Image = null;

            // Enable the btnsaveBarco button for a new entry
            btnsaveBarco.Enabled = true;
        }
        private void LoadReport(string selectedMonth)
        {
            // Define the path to the report
            string reportPath = System.Environment.CurrentDirectory + "\\Report\\BaRCOReport.rdlc";
            rv_barcoReport.LocalReport.ReportPath = reportPath;

            // Clear existing data sources
            rv_barcoReport.LocalReport.DataSources.Clear();

            // Load the BarangayName parameter
            string barangayName = GetBarangayName();
            if (barangayName != null)
            {
                ReportParameter barangayNameParam = new ReportParameter("BarangayName", barangayName);
                rv_barcoReport.LocalReport.SetParameters(barangayNameParam);
            }

            // Set the MonthYear parameter to display for the selected month
            string formattedMonthYear = $"For the Month of {selectedMonth}";
            ReportParameter monthYearParam = new ReportParameter("MonthYear", formattedMonthYear);
            rv_barcoReport.LocalReport.SetParameters(monthYearParam);

            // Set PageNumber parameter (set manually or from your logic)
            string pageNumber = "1";  // Replace with logic to get the current page if needed
            ReportParameter pageNumberParam = new ReportParameter("PageNumber", pageNumber);
            rv_barcoReport.LocalReport.SetParameters(pageNumberParam);

            // Load the Chairman and Secretary information
            SetOfficialParameters();

            // Load data from tblimage into the report
            DataTable imageData = GetImageData();
            if (imageData != null)
            {
                ReportDataSource imageDataSource = new ReportDataSource("dsetimage", imageData);
                rv_barcoReport.LocalReport.DataSources.Add(imageDataSource);
            }

            // Load data from tblbarco into the report, filtering by selected month
            DataTable barcoData = GetBarcoDataByMonth(selectedMonth);
            if (barcoData != null)
            {
                ReportDataSource barcoDataSource = new ReportDataSource("dsetbarco", barcoData);
                rv_barcoReport.LocalReport.DataSources.Add(barcoDataSource);
            }
            // Load data for official information (Kagawad and Chairman)
            DataTable officialInfoData = GetOfficialInfoData();
            if (officialInfoData != null)
            {
                ReportDataSource officialInfoDataSource = new ReportDataSource("dsetofficialinfo", officialInfoData);
                rv_barcoReport.LocalReport.DataSources.Add(officialInfoDataSource);
            }
            // Load data for SK officials (SK Chairman and SK Kagawad)
            DataTable skOfficialInfoData = GetSKOfficialInfoData();
            if (skOfficialInfoData != null)
            {
                ReportDataSource skOfficialInfoDataSource = new ReportDataSource("dsetskofficialinfo", skOfficialInfoData);
                rv_barcoReport.LocalReport.DataSources.Add(skOfficialInfoDataSource);
            }
            // Load data for Barangay Tanod
            DataTable barangayTanodInfoData = GetBarangayTanodInfoData();
            if (barangayTanodInfoData != null)
            {
                ReportDataSource barangayTanodInfoDataSource = new ReportDataSource("dsetbarangaytanod", barangayTanodInfoData);
                rv_barcoReport.LocalReport.DataSources.Add(barangayTanodInfoDataSource);
            }
            // Set display mode and refresh the report
            rv_barcoReport.SetDisplayMode(DisplayMode.PrintLayout);
            rv_barcoReport.ZoomMode = ZoomMode.FullPage;
            rv_barcoReport.RefreshReport();
        }
        private DataTable GetBarangayTanodInfoData()
        {
            DataTable dt = new DataTable();

            // SQL query to get Barangay Tanod information
            string query = "SELECT FullName, Position FROM tblofficialinfo WHERE Position = 'Barangay Tanod'";

            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=dbiBIM;User=root;Password=;"))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the query result
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching Barangay Tanod info: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return dt;
        }
        private DataTable GetSKOfficialInfoData()
        {
            DataTable dt = new DataTable();
            // SQL query to get officials with Position = 'SK Chairman' or 'SK Kagawad'
            string query = "SELECT FullName, Position FROM tblofficialinfo WHERE Position IN ('SK Chairman', 'SK Kagawad')";

            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=dbiBIM;User=root;Password=;"))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the query result
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching SK official info: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return dt;
        }
        private DataTable GetOfficialInfoData()
        {
            DataTable dt = new DataTable();

            // SQL query to get officials with Position = 'Kagawad' or 'Chairman'
            string query = "SELECT FullName, Position FROM tblofficialinfo WHERE Position IN ('Kagawad', 'Chairman')";

            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=dbiBIM;User=root;Password=;"))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the query result
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching official info: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return dt;
        }
        private DataTable GetBarcoDataByMonth(string selectedMonth)
        {
            DataTable dataTable = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM tblbarco WHERE Month = @Month";  // Filter by Month
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Month", selectedMonth);

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                try
                {
                    adapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching BARCO data: " + ex.Message);
                }
            }
            return dataTable;
        }
        private void SetOfficialParameters()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                // SQL queries to count officials based on their position
                string queryBarangayOfficials = "SELECT COUNT(*) FROM tblofficialinfo WHERE Position IN ('Chairman', 'Secretary', 'Treasurer', 'Kagawad')";
                string querySKOfficials = "SELECT COUNT(*) FROM tblofficialinfo WHERE Position IN ('SK Chairman', 'SK Kagawad')";
                string queryBarangayTanods = "SELECT COUNT(*) FROM tblofficialinfo WHERE Position = 'Barangay Tanod'";

                // Queries to count attended officials based on the tblbarco table
                string queryBarangayOfficialsAttended = "SELECT SUM(NoofBO) FROM tblbarco WHERE NoofBO IS NOT NULL";
                string querySKOfficialsAttended = "SELECT COUNT(TotalSKOfficial) FROM tblbarco WHERE TotalSKOfficial IS NOT NULL";
                string queryBarangayTanodsAttended = "SELECT SUM(TotalPersonel) FROM tblbarco WHERE TotalPersonel IS NOT NULL";

                try
                {
                    connection.Open();

                    // Counting Barangay Officials
                    MySqlCommand cmdBarangayOfficials = new MySqlCommand(queryBarangayOfficials, connection);
                    int totalBarangayOfficials = Convert.ToInt32(cmdBarangayOfficials.ExecuteScalar());

                    // Counting SK Officials
                    MySqlCommand cmdSKOfficials = new MySqlCommand(querySKOfficials, connection);
                    int totalSKOfficials = Convert.ToInt32(cmdSKOfficials.ExecuteScalar());

                    // Counting Barangay Tanods
                    MySqlCommand cmdBarangayTanods = new MySqlCommand(queryBarangayTanods, connection);
                    int totalBarangayTanods = Convert.ToInt32(cmdBarangayTanods.ExecuteScalar());

                    // Calculate Total Officials by summing all categories
                    int totalOfficials = totalBarangayOfficials + totalSKOfficials + totalBarangayTanods;

                    // Counting attended officials from tblbarco
                    MySqlCommand cmdBarangayOfficialsAttended = new MySqlCommand(queryBarangayOfficialsAttended, connection);
                    int totalBarangayOfficialsAttended = Convert.ToInt32(cmdBarangayOfficialsAttended.ExecuteScalar());

                    MySqlCommand cmdSKOfficialsAttended = new MySqlCommand(querySKOfficialsAttended, connection);
                    int totalSKOfficialsAttended = Convert.ToInt32(cmdSKOfficialsAttended.ExecuteScalar());

                    MySqlCommand cmdBarangayTanodsAttended = new MySqlCommand(queryBarangayTanodsAttended, connection);
                    int totalBarangayTanodsAttended = Convert.ToInt32(cmdBarangayTanodsAttended.ExecuteScalar());

                    // Calculate Total Attendees (Sum of all attended)
                    int totalAttendees = totalBarangayOfficialsAttended + totalSKOfficialsAttended + totalBarangayTanodsAttended;

                    // Calculate SCORE: (TotalAttendees / TotalOfficials) * 20
                    double score = 0;
                    if (totalOfficials > 0) // Prevent division by zero
                    {
                        score = (double)totalAttendees / totalOfficials * 20;
                    }

                    // Set the report parameters with the counts
                    ReportParameter totalBarangayOfficialsParam = new ReportParameter("TotalBarangayOfficials", totalBarangayOfficials.ToString());
                    rv_barcoReport.LocalReport.SetParameters(totalBarangayOfficialsParam);

                    ReportParameter totalSKOfficialsParam = new ReportParameter("TotalSKofficials", totalSKOfficials.ToString());
                    rv_barcoReport.LocalReport.SetParameters(totalSKOfficialsParam);

                    ReportParameter totalBarangayTanodsParam = new ReportParameter("TotalBarangayTanods", totalBarangayTanods.ToString());
                    rv_barcoReport.LocalReport.SetParameters(totalBarangayTanodsParam);

                    // Set the TotalOfficials parameter
                    ReportParameter totalOfficialsParam = new ReportParameter("TotalOfficials", totalOfficials.ToString());
                    rv_barcoReport.LocalReport.SetParameters(totalOfficialsParam);

                    // Set the attended parameters
                    ReportParameter totalBarangayOfficialsAttendedParam = new ReportParameter("TotalBarangayOfficialsAttended", totalBarangayOfficialsAttended.ToString());
                    rv_barcoReport.LocalReport.SetParameters(totalBarangayOfficialsAttendedParam);

                    ReportParameter totalSKOfficialsAttendedParam = new ReportParameter("TotalSKofficialsAttended", totalSKOfficialsAttended.ToString());
                    rv_barcoReport.LocalReport.SetParameters(totalSKOfficialsAttendedParam);

                    ReportParameter totalBarangayTanodsAttendedParam = new ReportParameter("TotalBarangayTanodsAttended", totalBarangayTanodsAttended.ToString());
                    rv_barcoReport.LocalReport.SetParameters(totalBarangayTanodsAttendedParam);

                    // Set the TotalAttendees parameter
                    ReportParameter totalAttendeesParam = new ReportParameter("TotalAttendees", totalAttendees.ToString());
                    rv_barcoReport.LocalReport.SetParameters(totalAttendeesParam);

                    // Set the SCORE parameter
                    ReportParameter scoreParam = new ReportParameter("SCORE", score.ToString("F2")); // Format to 2 decimal places
                    rv_barcoReport.LocalReport.SetParameters(scoreParam);

                    // You can also load the Chairman, Secretary, and other details as before
                    LoadChairmanSecretaryInfo(connection);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching official information: " + ex.Message);
                }
            }
        }
        private void LoadChairmanSecretaryInfo(MySqlConnection connection)
        {
            string query = "SELECT FullName, Position FROM tblofficialinfo WHERE Position IN ('Chairman', 'Secretary')";
            MySqlCommand command = new MySqlCommand(query, connection);

            MySqlDataReader reader = command.ExecuteReader();

            string chairmanName = null;
            string secretaryName = null;

            while (reader.Read())
            {
                string position = reader["Position"].ToString();
                if (position == "Chairman")
                {
                    chairmanName = reader["FullName"].ToString();
                }
                else if (position == "Secretary")
                {
                    secretaryName = reader["FullName"].ToString();
                }
            }

            if (chairmanName != null)
            {
                rv_barcoReport.LocalReport.SetParameters(new ReportParameter("Chairman", chairmanName));
            }
            if (secretaryName != null)
            {
                rv_barcoReport.LocalReport.SetParameters(new ReportParameter("Secretary", secretaryName));
            }
        }
        private string ConvertSignatureToImage(byte[] signatureBlob)
        {
            if (signatureBlob == null || signatureBlob.Length == 0)
                return null;
            using (MemoryStream ms = new MemoryStream(signatureBlob))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                string base64Image = Convert.ToBase64String(signatureBlob); // Convert image to Base64 string for ReportViewer
                return base64Image;
            }
        }
        private string GetBarangayName()
        {
            string barangayName = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT BarangayName FROM tblsetbrgyname LIMIT 1"; // Assumes only one row in tblsetbrgyname
                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        barangayName = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching BarangayName: " + ex.Message);
                }
            }

            return barangayName;
        }
        private DataTable GetImageData()
        {
            DataTable dataTable = new DataTable();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM tblimage";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);

                try
                {
                    adapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching image data: " + ex.Message);
                }
            }
            return dataTable;
        }
        private void cbmonth_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }
        private bool IsMonthSaved(string selectedMonth)
        {
            bool isSaved = false;
            string query = "SELECT COUNT(*) FROM tblbarco WHERE Month = @Month";

            using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=dbiBIM;User=root;Password=;"))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Month", selectedMonth);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        isSaved = count > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while checking for the month: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return isSaved;
        }
    }
}
