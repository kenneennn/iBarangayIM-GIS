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
using System.IO;

namespace iBIM_GIS
{
    public partial class IBIM_Announcement_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        public IBIM_Announcement_frm()
        {
            InitializeComponent();
            LoadAnnouncements();
            dgvBarangayAnnouncement.CellClick += dgvBarangayAnnouncement_CellClick;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Load the selected image into the PictureBox
                pbAnnouncementImage.Image = Image.FromFile(openFileDialog.FileName);

                // Update the officialImage variable with the newly selected image
 
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txttitle.Text) ||
               string.IsNullOrWhiteSpace(rtxtcontent.Text) ||
               string.IsNullOrWhiteSpace(txtpostedby.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string announcementId = GenerateAnnouncementID(conn);
                    byte[] imageBytes = null;
                    if (pbAnnouncementImage.Image != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            pbAnnouncementImage.Image.Save(ms, pbAnnouncementImage.Image.RawFormat);
                            imageBytes = ms.ToArray();
                        }
                    }
                    string query = "INSERT INTO tblAnnouncement (AnnouncementID, Title, Content, Postedby, DatePosted, DateEnded, Image) " +
                                   "VALUES (@AnnouncementID, @Title, @Content, @Postedby, @DatePosted, @DateEnded, @Image)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AnnouncementID", announcementId);
                        cmd.Parameters.AddWithValue("@Title", txttitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@Content", rtxtcontent.Text.Trim());
                        cmd.Parameters.AddWithValue("@Postedby", txtpostedby.Text.Trim());
                        cmd.Parameters.AddWithValue("@DatePosted", dateTimePickerDatePosted.Value);
                        cmd.Parameters.AddWithValue("@DateEnded", dateTimePickerDateEnded.Value);
                        cmd.Parameters.AddWithValue("@Image", imageBytes);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Announcement saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFields();
                    LoadAnnouncements();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadAnnouncements()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT AnnouncementID, Title, Content, Postedby, DatePosted, DateEnded FROM tblAnnouncement";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dgvBarangayAnnouncement.DataSource = dt;
                        dgvBarangayAnnouncement.RowTemplate.Height = 35;
                        dgvBarangayAnnouncement.ColumnHeadersHeight = 40;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GenerateAnnouncementID(MySqlConnection conn)
        {
            string prefix = "AID";
            string yearMonth = DateTime.Now.ToString("yyyyMM");
            string id = prefix + yearMonth;

            string query = "SELECT COUNT(*) FROM tblAnnouncement WHERE AnnouncementID LIKE @IDPattern";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@IDPattern", id + "%");
                int count = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                id += count.ToString("D3"); // Ensure the count is three digits
            }
            return id;
        }
        private void ResetFields()
        {
            txttitle.Clear();
            rtxtcontent.Clear();
            txtpostedby.Clear();
            dateTimePickerDatePosted.Value = DateTime.Now;
            dateTimePickerDateEnded.Value = DateTime.Now;
            pbAnnouncementImage.Image = null;
        }
        private void dgvBarangayAnnouncement_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvBarangayAnnouncement.Rows[e.RowIndex];
                string announcementId = row.Cells["AnnouncementID"].Value.ToString();

                txttitle.Text = row.Cells["Title"].Value.ToString();
                rtxtcontent.Text = row.Cells["Content"].Value.ToString();
                txtpostedby.Text = row.Cells["Postedby"].Value.ToString();
                dateTimePickerDatePosted.Value = Convert.ToDateTime(row.Cells["DatePosted"].Value);
                dateTimePickerDateEnded.Value = Convert.ToDateTime(row.Cells["DateEnded"].Value);

                // Load the image from the database based on the selected AnnouncementID
                LoadImageFromDatabase(announcementId);
            }
        }

        private void LoadImageFromDatabase(string announcementId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Image FROM tblAnnouncement WHERE AnnouncementID = @AnnouncementID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AnnouncementID", announcementId);

                        object result = cmd.ExecuteScalar();

                        if (result != DBNull.Value && result != null)
                        {
                            byte[] imageBytes = (byte[])result;
                            if (imageBytes.Length > 0)
                            {
                                using (MemoryStream ms = new MemoryStream(imageBytes))
                                {
                                    pbAnnouncementImage.Image = Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                pbAnnouncementImage.Image = null; // Clear the PictureBox if no image is found
                            }
                        }
                        else
                        {
                            pbAnnouncementImage.Image = null; // Clear the PictureBox if no image is found
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvBarangayAnnouncement.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an announcement to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvBarangayAnnouncement.SelectedRows[0];
            string announcementId = selectedRow.Cells["AnnouncementID"].Value.ToString();

            if (string.IsNullOrWhiteSpace(txttitle.Text) ||
               string.IsNullOrWhiteSpace(rtxtcontent.Text) ||
               string.IsNullOrWhiteSpace(txtpostedby.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    byte[] imageBytes = null;
                    if (pbAnnouncementImage.Image != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            // Load the image into a new Bitmap object to ensure it can be saved without issues
                            using (Bitmap bmp = new Bitmap(pbAnnouncementImage.Image))
                            {
                                bmp.Save(ms, pbAnnouncementImage.Image.RawFormat);
                                imageBytes = ms.ToArray();
                            }
                        }
                    }

                    string query = "UPDATE tblAnnouncement SET " +
                                   "Title = @Title, " +
                                   "Content = @Content, " +
                                   "Postedby = @Postedby, " +
                                   "DatePosted = @DatePosted, " +
                                   "DateEnded = @DateEnded, " +
                                   "Image = @Image " +
                                   "WHERE AnnouncementID = @AnnouncementID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AnnouncementID", announcementId);
                        cmd.Parameters.AddWithValue("@Title", txttitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@Content", rtxtcontent.Text.Trim());
                        cmd.Parameters.AddWithValue("@Postedby", txtpostedby.Text.Trim());
                        cmd.Parameters.AddWithValue("@DatePosted", dateTimePickerDatePosted.Value);
                        cmd.Parameters.AddWithValue("@DateEnded", dateTimePickerDateEnded.Value);
                        cmd.Parameters.AddWithValue("@Image", imageBytes);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Announcement updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ResetFields();
                    LoadAnnouncements();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvBarangayAnnouncement_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetFields();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBarangayAnnouncement.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an announcement to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected announcement ID
            DataGridViewRow selectedRow = dgvBarangayAnnouncement.SelectedRows[0];
            string announcementId = selectedRow.Cells["AnnouncementID"].Value.ToString();

            // Confirm deletion
            DialogResult result = MessageBox.Show("Are you sure you want to delete this announcement?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Prepare the SQL query to delete the announcement
                    string query = "DELETE FROM tblAnnouncement WHERE AnnouncementID = @AnnouncementID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AnnouncementID", announcementId);

                        // Execute the query
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Announcement deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset the fields and reload announcements
                    ResetFields();
                    LoadAnnouncements();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
