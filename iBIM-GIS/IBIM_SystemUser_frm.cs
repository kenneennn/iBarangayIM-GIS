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
    public partial class IBIM_SystemUser_frm : Form
    {

        string connectionString = "server=localhost;user=root;database=dbiBIM;password=;";
        private bool isDataGridClicked = false;
        private Image Image;
        public IBIM_SystemUser_frm()
        {
            InitializeComponent();
            DisplayAllUserData();
            dgvuser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;


            // Set the font and fill modes for DataGridView
            dgvuser.DefaultCellStyle.Font = new Font("Century Gothic", 12);
            dgvuser.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvuser.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Customize header styles
            dgvuser.ColumnHeadersDefaultCellStyle.Font = new Font("Impact", 14, FontStyle.Regular);
            dgvuser.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvuser.RowHeadersDefaultCellStyle.Font = new Font("Century Gothic", 14);


            this.KeyPreview = true;
            this.KeyPress += new KeyPressEventHandler(Form1_KeyPress);

            // Other initializations
            txtuserid.Enter += new EventHandler(txtuserid_Enter);
            txtaccountname.Enter += new EventHandler(txtaccountname_Enter);
            cbAccountType.Enter += new EventHandler(cbAccountType_Enter);

            txtusername.Enter += new EventHandler(txtusername_Enter);
            txtpassword.Enter += new EventHandler(txtpassword_Enter);

            dgvuser.RowPrePaint += new DataGridViewRowPrePaintEventHandler(dataGridView1_RowPrePaint);
            dgvuser.CellClick += dataGridView1_CellClick_1;
            dgv_mayorinfo.CellClick += dgv_mayorinfo_CellClick;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            DataGridViewRow row = dgvuser.Rows[e.RowIndex];
            row.DefaultCellStyle.Font = new Font("Century Gothic", 14);
            row.DefaultCellStyle.ForeColor = Color.Black;

        }

        private string CaesarCipherEncrypt(string input, int shift)
        {
            StringBuilder encryptedText = new StringBuilder();

            foreach (char ch in input)
            {
                if (char.IsLetter(ch))
                {
                    char shiftedChar = (char)(ch + shift);
                    if ((char.IsLower(ch) && shiftedChar > 'z') || (char.IsUpper(ch) && shiftedChar > 'Z'))
                    {
                        shiftedChar = (char)(ch - (26 - shift));
                    }
                    encryptedText.Append(shiftedChar);
                }
                else
                {
                    encryptedText.Append(ch);
                }
            }

            return encryptedText.ToString();
        }
        private string CaesarCipherDecrypt(string input, int shift)
        {
            StringBuilder decryptedText = new StringBuilder();

            foreach (char ch in input)
            {
                if (char.IsLetter(ch))
                {
                    char shiftedChar = (char)(ch - shift);
                    if ((char.IsLower(ch) && shiftedChar < 'a') || (char.IsUpper(ch) && shiftedChar < 'A'))
                    {
                        shiftedChar = (char)(ch + (26 - shift));
                    }
                    decryptedText.Append(shiftedChar);
                }
                else
                {
                    decryptedText.Append(ch);
                }
            }

            return decryptedText.ToString();
        }
        private bool AreFieldsValid()
        {
            return !string.IsNullOrWhiteSpace(txtuserid.Text) &&
                   !string.IsNullOrWhiteSpace(txtaccountname.Text) &&
                   !string.IsNullOrWhiteSpace(cbAccountType.Text) &&
                   !string.IsNullOrWhiteSpace(txtusername.Text) &&
                   !string.IsNullOrWhiteSpace(txtpassword.Text);
        }
        private void ClearFields()
        {
            txtuserid.Text = "";
            txtbarangayname.Text = "";
            txtaccountname.Text = "";
            cbAccountType.SelectedIndex = -1;
            txtusername.Text = "";
            txtpassword.Text = "";
            isDataGridClicked = false;
        }

        private void btnShiftA_Click(object sender, EventArgs e)
        {
            if (isDataGridClicked)
            {
                string decryptedPassword = CaesarCipherDecrypt(txtpassword.Text, 5);
                txtpassword.Text = decryptedPassword;
            }
            else
            {
                MessageBox.Show("Please click on a row in the DataGridView first.");
            }
        }
        private void DisplayAllUserData()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM tbluser";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            dgvuser.DataSource = dataTable;

                            // Set column header names
                            dgvuser.Columns["UserID"].HeaderText = "User ID";
                            dgvuser.Columns["AccountName"].HeaderText = "Account Name";
                            dgvuser.Columns["AccountType"].HeaderText = "Account Type";
                            dgvuser.Columns["username"].HeaderText = "Username";
                            dgvuser.Columns["password"].HeaderText = "Password";

                            dgvuser.Columns["UserID"].Width = 100;
                            dgvuser.Columns["AccountName"].Width = 350;
                            dgvuser.Columns["AccountType"].Width = 250;
                            dgvuser.Columns["username"].Width = 120;
                            dgvuser.Columns["password"].Width = 150;


                            dgvuser.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dgvuser.RowTemplate.Height = 35; // Adjust height as needed
                            dgvuser.ColumnHeadersHeight = 40; // Adjust height as needed
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching data: " + ex.Message);
                }
            }
        }

        private void DisplayAllImageData()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM tblimage";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            // Clear existing columns and rows if any
                            dgLogo.Columns.Clear();
                            dgLogo.Rows.Clear();

                            // Configure the DataGridViewImageColumns
                            DataGridViewImageColumn imgColumn1 = new DataGridViewImageColumn
                            {
                                HeaderText = "LGU LOGO",
                                Name = "Image1",
                                ImageLayout = DataGridViewImageCellLayout.Zoom // Maintain aspect ratio
                            };

                            DataGridViewImageColumn imgColumn2 = new DataGridViewImageColumn
                            {
                                HeaderText = "Barangay LOGO",
                                Name = "Image2",
                                ImageLayout = DataGridViewImageCellLayout.Zoom
                            };

                            DataGridViewImageColumn imgColumn3 = new DataGridViewImageColumn
                            {
                                HeaderText = "System LOGO",
                                Name = "Image3",
                                ImageLayout = DataGridViewImageCellLayout.Zoom
                            };

                            // Add the columns to DataGridView
                            dgLogo.Columns.Add(imgColumn1);
                            dgLogo.Columns.Add(imgColumn2);
                            dgLogo.Columns.Add(imgColumn3);

                            // Set row height for a larger display
                            dgLogo.RowTemplate.Height = 200; // Adjust based on the size of your images

                            // Load data and convert byte arrays to images
                            foreach (DataRow row in dataTable.Rows)
                            {
                                // Convert each image byte array to an Image
                                byte[] imageBytes1 = (byte[])row["Image1"];
                                byte[] imageBytes2 = (byte[])row["Image2"];
                                byte[] imageBytes3 = (byte[])row["Image3"];

                                Image img1 = ByteArrayToImage(imageBytes1);
                                Image img2 = ByteArrayToImage(imageBytes2);
                                Image img3 = ByteArrayToImage(imageBytes3);

                                // Add a row with the images
                                dgLogo.Rows.Add(img1, img2, img3);
                            }

                            // Auto-resize columns to fit content
                            dgLogo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            // Adjust the display to avoid additional rows
                            dgLogo.AllowUserToAddRows = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching data: " + ex.Message);
                }
            }
        }

        // Helper method to convert a byte array to an Image
        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift && e.KeyChar == 'X')
            {
                if (isDataGridClicked)
                {
                    string decryptedPassword = CaesarCipherDecrypt(txtpassword.Text, 5);
                    txtpassword.Text = decryptedPassword;
                }
                else
                {
                    MessageBox.Show("Please click on a row in the DataGridView first.");
                }
            }
        }
        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            DataGridViewRow row = dgvuser.Rows[e.RowIndex];
            txtuserid.Text = row.Cells["UserID"].Value.ToString();
            txtaccountname.Text = row.Cells["AccountName"].Value.ToString();
            cbAccountType.SelectedItem = row.Cells["AccountType"].Value.ToString();
            txtusername.Text = row.Cells["username"].Value.ToString();
            txtpassword.Text = row.Cells["password"].Value.ToString();
            
            isDataGridClicked = true;
                btnsavee.Enabled = true;
                btnsavee.Text = "New";
        }
    }

        private void txtuserid_Enter(object sender, EventArgs e)
        {
            btnsavee.Enabled = true;
        }
        private void txtaccountname_Enter(object sender, EventArgs e)
        {
            btnsavee.Enabled = true;
        }
        private void txtaccounttype_Enter(object sender, EventArgs e)
        {
            btnsavee.Enabled = true;
        }
        private void txtusername_Enter(object sender, EventArgs e)
        {
            btnsavee.Enabled = true;
        }
        private void txtpassword_Enter(object sender, EventArgs e)
        {
            btnsavee.Enabled = true;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnUpload1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Load the selected image into the PictureBox
                pbleft.Image = Image.FromFile(openFileDialog.FileName);

                // Update the officialImage variable with the newly selected image
                Image = pbleft.Image;
            }
        }
        private void btnUpload2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Load the selected image into the PictureBox
                pbright.Image = Image.FromFile(openFileDialog.FileName);

                // Update the officialImage variable with the newly selected image
                Image = pbright.Image;
            }
        }
        private void btnSave1_Click(object sender, EventArgs e)
        {
            // Check if the PictureBox has an image
            if (pbleft.Image != null)
            {
                // Convert the image to a byte array
                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    pbleft.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // You can use other formats like .Jpeg
                    imageBytes = ms.ToArray();
                }

                // Check if an image already exists in the database
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // Check if there is already an image saved
                        string checkQuery = "SELECT COUNT(*) FROM tblImage";
                        MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                        int imageCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        string query;
                        if (imageCount > 0)
                        {
                            // If an image exists, update the existing one
                            query = "UPDATE tblImage SET Image1 = @Image1";
                        }
                        else
                        {
                            // If no image exists, insert a new one
                            query = "INSERT INTO tblImage (Image1) VALUES (@Image1)";
                        }

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Image1", imageBytes);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Image saved successfully!");
                                // Call additional methods after saving the image
                                ConfigureDataGridView();  // Apply DataGridView settings
                                LoadBarangayNames();      // Load data into DataGridView
                                DisplayAllImageData();    // Display all image data
                                ConfigureDataGridView();  // Apply DataGridView settings again (if needed)
                                DisplayAllUserData();     // Display all user data
                            }
                            else
                            {
                                MessageBox.Show("Failed to save image.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("No image selected in the PictureBox.");
            }
        }
        private void btnSave2_Click(object sender, EventArgs e)
        {
            // Check if the PictureBox has an image
            if (pbright.Image != null)
            {
                // Convert the image to a byte array
                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    pbright.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // You can use other formats like .Jpeg
                    imageBytes = ms.ToArray();
                }

                // Check if an image already exists in the database
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // Check if there is already an image saved
                        string checkQuery = "SELECT COUNT(*) FROM tblImage";
                        MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                        int imageCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        string query;
                        if (imageCount > 0)
                        {
                            // If an image exists, update the existing one
                            query = "UPDATE tblImage SET Image2 = @Image2";
                        }
                        else
                        {
                            // If no image exists, insert a new one
                            query = "INSERT INTO tblImage (Image2) VALUES (@Image2)";
                        }

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Image2", imageBytes);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Image saved successfully!");
                                // Call additional methods after saving the image
                                ConfigureDataGridView();  // Apply DataGridView settings
                                LoadBarangayNames();      // Load data into DataGridView
                                DisplayAllImageData();    // Display all image data
                                ConfigureDataGridView();  // Apply DataGridView settings again (if needed)
                                DisplayAllUserData();     // Display all user data
                            }
                            else
                            {
                                MessageBox.Show("Failed to save image.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("No image selected in the PictureBox.");
            }
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();   
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnupdatee_Click(object sender, EventArgs e)
        {
            if (isDataGridClicked && AreFieldsValid())
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "UPDATE tbluser SET AccountName = @AccountName, AccountType = @AccountType, username = @username, password = @password WHERE UserID = @UserID";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@UserID", txtuserid.Text);
                            command.Parameters.AddWithValue("@AccountName", txtaccountname.Text);
                            command.Parameters.AddWithValue("@AccountType", cbAccountType.SelectedItem.ToString());
                            command.Parameters.AddWithValue("@username", txtusername.Text);
                            string encryptedPassword = CaesarCipherEncrypt(txtpassword.Text, 5);
                            command.Parameters.AddWithValue("@password", encryptedPassword);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Record updated successfully!");
                                ClearFields();
                                DisplayAllUserData();
                            }
                            else
                            {
                                MessageBox.Show("Failed to update record!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please click on a row in the DataGridView and ensure all fields are filled in.");
            }
        }
        private void btndeletee_Click(object sender, EventArgs e)
        {
            if (isDataGridClicked)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = "DELETE FROM tbluser WHERE UserID = @UserID";

                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@UserID", txtuserid.Text);
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Record deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ClearFields();
                                    DisplayAllUserData(); // Refresh DataGridView

                                    // Generate the next UserID after deletion
                                    GenerateUserID();
                                }
                                else
                                {
                                    MessageBox.Show("Failed to delete record!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please click on a row in the DataGridView first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private void btnsavee_Click(object sender, EventArgs e)
        {
            if (btnsavee.Text == "New")
            {
                ClearFields();
                btnsavee.Text = "Save";
                btnsavee.Enabled = true;
                GenerateUserID(); // Generate new UserID for a fresh entry
                return;
            }

            if (!AreFieldsValid())
            {
                MessageBox.Show("Please fill in all required fields.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedAccountType = cbAccountType.SelectedItem.ToString();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if the UserID already exists
                    string checkExistenceQuery = "SELECT COUNT(*) FROM tbluser WHERE UserID = @UserID";
                    using (MySqlCommand checkExistenceCommand = new MySqlCommand(checkExistenceQuery, connection))
                    {
                        checkExistenceCommand.Parameters.AddWithValue("@UserID", txtuserid.Text);
                        int count = Convert.ToInt32(checkExistenceCommand.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("UserID already exists in the database!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Check if there is an existing user with the same AccountType
                    string checkAccountTypeQuery = "SELECT COUNT(*) FROM tbluser WHERE AccountType = @AccountType";
                    using (MySqlCommand checkAccountTypeCommand = new MySqlCommand(checkAccountTypeQuery, connection))
                    {
                        checkAccountTypeCommand.Parameters.AddWithValue("@AccountType", selectedAccountType);
                        int accountTypeCount = Convert.ToInt32(checkAccountTypeCommand.ExecuteScalar());

                        if (accountTypeCount > 0)
                        {
                            MessageBox.Show($"Only one {selectedAccountType} is allowed. Please check the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Insert new user
                    string query = "INSERT INTO tbluser (UserID, AccountName, AccountType, username, password) VALUES (@UserID, @AccountName, @AccountType, @username, @password)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", txtuserid.Text);
                        command.Parameters.AddWithValue("@AccountName", txtaccountname.Text);
                        command.Parameters.AddWithValue("@AccountType", selectedAccountType);
                        command.Parameters.AddWithValue("@username", txtusername.Text);

                        string encryptedPassword = CaesarCipherEncrypt(txtpassword.Text, 5);
                        command.Parameters.AddWithValue("@password", encryptedPassword);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DisplayAllUserData(); // Refresh DataGridView
                            ClearFields();
                            btnsavee.Text = "New";

                            // Generate a new UserID for the next entry
                            GenerateUserID();
                        }
                        else
                        {
                            MessageBox.Show("Failed to save data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnsavebrgyname_Click(object sender, EventArgs e)
        {
            // Check if the txtbarangayname field is empty
            if (string.IsNullOrWhiteSpace(txtbarangayname.Text))
            {
                // Display a warning icon and message
                MessageBox.Show("Please fill in all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Optionally, set focus to the required field
                txtbarangayname.Focus();
                return; // Exit the method without saving
            }

            string newBarangayID = GenerateBarangayID();  // Generate new ID
            string query = "INSERT INTO tblsetbrgyname (BarangayID, BarangayName) VALUES (@BarangayID, @BarangayName)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BarangayID", newBarangayID);
                    cmd.Parameters.AddWithValue("@BarangayName", txtbarangayname.Text);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Barangay name saved successfully!");
            LoadBarangayNames();  // Refresh the DataGridView
        }


        private void btnupdatebrgyname_Click(object sender, EventArgs e)
        {
            if (dgv_barangayname.SelectedRows.Count > 0)
            {
                string id = dgv_barangayname.SelectedRows[0].Cells["BarangayID"].Value.ToString();
                string query = "UPDATE tblsetbrgyname SET BarangayName = @BarangayName WHERE BarangayID = @BarangayID";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BarangayName", txtbarangayname.Text);
                        cmd.Parameters.AddWithValue("@BarangayID", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Barangay name updated successfully!");
                LoadBarangayNames();  // Refresh the DataGridView
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }
        private void btndeletebrgyname_Click(object sender, EventArgs e)
        {
            if (dgv_barangayname.SelectedRows.Count > 0)
            {
                string id = dgv_barangayname.SelectedRows[0].Cells["BarangayID"].Value.ToString();
                string query = "DELETE FROM tblsetbrgyname WHERE BarangayID = @BarangayID";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BarangayID", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Barangay name deleted successfully!");
                LoadBarangayNames();  // Refresh the DataGridView
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }
        private void LoadBarangayNames()
        {
            string query = "SELECT BarangayID, BarangayName FROM tblsetbrgyname";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgv_barangayname.DataSource = dt;
                    dgv_barangayname.RowTemplate.Height = 35; // Adjust height as needed
                    dgv_barangayname.ColumnHeadersHeight = 40; // Adjust height as needed
                }
            }

            // Ensure the DataGridView adjusts its size to fit the content
            dgv_barangayname.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }


        private void dgv_barangayname_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgv_barangayname.Rows[e.RowIndex];
                txtbarangayname.Text = row.Cells["BarangayName"].Value.ToString();
            }
        }
        private void IBIM_CreateUser_frm_Load(object sender, EventArgs e)
        {
            LoadCommittee();
            LoadGovernmentAssistance();
            ConfigureDataGridView();  // Apply DataGridView settings
            LoadBarangayNames();      // Load data into DataGridView
            DisplayAllImageData();
            ConfigureDataGridView();
            DisplayAllUserData();
            LoadMayorInfo();
            txtuserid.Enabled = false;
            GenerateUserID(); 

            try
            {
                // Open connection and load images into PictureBoxes
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    LoadImagesIntoPictureBoxes(connection); // Load images from db to PictureBoxes
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void GenerateUserID()
        {
            string connectionString = "server=localhost;user=root;database=dbiBIM;password=;"; // Replace with your actual connection string
            string query = "SELECT MAX(UserID) FROM tbluser"; // Replace 'tblusers' and 'UserID' with your actual table and column names

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    var result = cmd.ExecuteScalar();

                    string newUserID;
                    if (result != DBNull.Value && result != null)
                    {
                        // Extract the numeric part and increment it
                        string lastUserID = result.ToString();
                        int numericPart = int.Parse(lastUserID.Substring(5)); // Extract numeric part, assuming "UIDNO" is 5 characters
                        newUserID = "UIDNO" + (numericPart + 1).ToString("D3"); // Increment and format as three digits
                    }
                    else
                    {
                        // Start from UIDNO001 if no users exist
                        newUserID = "UIDNO001";
                    }

                    txtuserid.Text = newUserID; // Assign the new UserID to the textbox
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error generating UserID: " + ex.Message);
                }
            }
        }


        private string GenerateBarangayID()
        {
            string newID = "BRGY001";  // Default ID if no records exist
            string query = "SELECT BarangayID FROM tblsetbrgyname ORDER BY BarangayID DESC LIMIT 1";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        // Extract the numeric part and increment it
                        string lastID = result.ToString();  // e.g., "BRGY005"
                        int numericPart = int.Parse(lastID.Substring(4)) + 1;  // Extract and increment
                        newID = "BRGY" + numericPart.ToString("D3");  // Format as BRGYxxx
                    }
                }
            }
            return newID;
        }
        private void ConfigureDataGridView()
        {
            // Set general properties
            dgv_barangayname.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;  // Auto-fill columns to fit grid
            dgv_barangayname.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;    // Auto-adjust row height
            dgv_barangayname.SelectionMode = DataGridViewSelectionMode.FullRowSelect;     // Full row selection on click
            dgv_barangayname.MultiSelect = false;                                         // Disable multi-select
            dgv_barangayname.ReadOnly = true;                                             // Make cells read-only
            dgv_barangayname.AllowUserToAddRows = false;                                  // Disable manual row addition
            dgv_barangayname.AllowUserToResizeRows = false;                               // Disable resizing rows manually
            dgv_barangayname.AllowUserToResizeColumns = true;                             // Allow resizing columns

            // Set alternating row color for better readability
            dgv_barangayname.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // Set font for column headers
            dgv_barangayname.ColumnHeadersDefaultCellStyle.Font = new Font("Impact", 14F, FontStyle.Regular);
            dgv_barangayname.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv_barangayname.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Set font and alignment for cells
            dgv_barangayname.DefaultCellStyle.Font = new Font("Century Gothic", 12F, FontStyle.Regular);
            dgv_barangayname.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Set row height to auto-size
            dgv_barangayname.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);

            // Remove row headers if not needed
            dgv_barangayname.RowHeadersVisible = false;

            // Highlight the entire row on hover
            dgv_barangayname.DefaultCellStyle.SelectionBackColor = Color.SkyBlue;
            dgv_barangayname.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Add tooltip for better user experience
            dgv_barangayname.ShowCellToolTips = true;

            // Customize image columns (stretch images to fit cells)
            foreach (DataGridViewColumn column in dgv_barangayname.Columns)
            {
                if (column is DataGridViewImageColumn imageColumn)
                {
                    imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom; // Use Zoom for proper aspect ratio
                    column.Width = 100; // Adjust the column width to your preference
                }
            }

            // Set default row height to ensure enough space for images
            dgv_barangayname.RowTemplate.Height = 100;

            // Set a custom gridline color
            dgv_barangayname.GridColor = Color.LightGray;

            // Set a border style for the grid
            dgv_barangayname.BorderStyle = BorderStyle.FixedSingle;

            // Enable double-buffering to reduce flickering (optional)
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.SetProperty,
                null, dgv_barangayname, new object[] { true });
        }

        private void btnnew_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnsavelsystemogo_Click(object sender, EventArgs e)
        {
            // Check if the PictureBox has an image
            if (pBsystemlogo.Image != null)
            {
                // Convert the image to a byte array
                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    pBsystemlogo.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // You can use other formats like .Jpeg
                    imageBytes = ms.ToArray();
                }

                // Check if an image already exists in the database
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // Check if there is already an image saved
                        string checkQuery = "SELECT COUNT(*) FROM tblImage";
                        MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                        int imageCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        string query;
                        if (imageCount > 0)
                        {
                            // If an image exists, update the existing one
                            query = "UPDATE tblImage SET Image3 = @Image3";
                        }
                        else
                        {
                            // If no image exists, insert a new one
                            query = "INSERT INTO tblImage (Image3) VALUES (@Image3)";
                        }

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Image3", imageBytes);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Image saved successfully!");
                                // Call additional methods after saving the image
                                ConfigureDataGridView();  // Apply DataGridView settings
                                LoadBarangayNames();      // Load data into DataGridView
                                DisplayAllImageData();    // Display all image data
                                ConfigureDataGridView();  // Apply DataGridView settings again (if needed)
                                DisplayAllUserData();     // Display all user data
                            }
                            else
                            {
                                MessageBox.Show("Failed to save image.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("No image selected in the PictureBox.");
            }
        }

        private void btnuploadsystemlogo_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Load the selected image into the PictureBox
                pBsystemlogo.Image = Image.FromFile(openFileDialog.FileName);

                // Update the officialImage variable with the newly selected image
                Image = pBsystemlogo.Image;
            }
        }

        private void btndeletesystemlogo_Click(object sender, EventArgs e)
        {
            // Ensure a cell is selected in the DataGridView
            if (dgLogo.SelectedCells.Count > 0)
            {
                // Get the selected row index from the DataGridView
                int selectedRowIndex = dgLogo.SelectedCells[0].RowIndex;

                // Connection string for MySQL/MariaDB
                string connectionString = "server=localhost;user=root;database=dbiBIM;password=;";

                // SQL query to delete Image3 for the specific row, using row position
                string query = $@"
            UPDATE tblimage 
            SET Image3 = NULL 
            WHERE (
                SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1
                FROM tblimage) = {selectedRowIndex}";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        // Open the connection
                        connection.Open();

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            // Execute the query
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Image3 cleared successfully for the selected row.");
                                pBsystemlogo.Image = null; // Clear the PictureBox

                                 // Call additional methods after saving the image
                                ConfigureDataGridView();  // Apply DataGridView settings
                                LoadBarangayNames();      // Load data into DataGridView
                                DisplayAllImageData();    // Display all image data
                                ConfigureDataGridView();  // Apply DataGridView settings again (if needed)
                                DisplayAllUserData();     // Display all user data
                                LoadImage3FromDatabase();
                            }
                            else
                            {
                                MessageBox.Show("No matching record found.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a cell in the DataGridView.");
            }
        }
        private void LoadImage2FromDatabase()
        {
            // Connection string for MySQL/MariaDB
            string connectionString = "server=localhost;user=root;database=dbiBIM;password=;";

            // Query to fetch the Image3 from tblImage (assuming you have only one row per image)
            string query = "SELECT Image2 FROM tblImage WHERE Image2 IS NOT NULL LIMIT 1";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Check if a row is returned
                            {
                                // Check if the Image3 column value is not DBNull
                                if (!reader.IsDBNull(0))
                                {
                                    byte[] imageBytes = (byte[])reader["Image2"];

                                    // Convert byte array to image and display in PictureBox
                                    using (MemoryStream ms = new MemoryStream(imageBytes))
                                    {
                                        pbright.Image = Image.FromStream(ms);
                                    }
                                }
                                else
                                {
                                    // If the value is NULL, clear the PictureBox
                                    pbright.Image = null;
                                    MessageBox.Show("No image available.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("No matching record found.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching data: " + ex.Message);
                }
            }
        }
        private void LoadImage3FromDatabase()
        {
            // Connection string for MySQL/MariaDB
            string connectionString = "server=localhost;user=root;database=dbiBIM;password=;";

            // Query to fetch the Image3 from tblImage (assuming you have only one row per image)
            string query = "SELECT Image3 FROM tblImage WHERE Image3 IS NOT NULL LIMIT 1";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Check if a row is returned
                            {
                                // Check if the Image3 column value is not DBNull
                                if (!reader.IsDBNull(0))
                                {
                                    byte[] imageBytes = (byte[])reader["Image3"];

                                    // Convert byte array to image and display in PictureBox
                                    using (MemoryStream ms = new MemoryStream(imageBytes))
                                    {
                                        pBsystemlogo.Image = Image.FromStream(ms);
                                    }
                                }
                                else
                                {
                                    // If the value is NULL, clear the PictureBox
                                    pBsystemlogo.Image = null;
                                    MessageBox.Show("No image available.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("No matching record found.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while fetching data: " + ex.Message);
                }
            }
        }

        private void btnDelete2_Click(object sender, EventArgs e)
        {
            // Ensure a cell is selected in the DataGridView
            if (dgLogo.SelectedCells.Count > 0)
            {
                // Get the selected row index from the DataGridView
                int selectedRowIndex = dgLogo.SelectedCells[0].RowIndex;

                // Connection string for MySQL/MariaDB
                string connectionString = "server=localhost;user=root;database=dbiBIM;password=;";

                // SQL query to delete Image3 for the specific row, using row position
                string query = $@"
            UPDATE tblimage 
            SET Image2 = NULL 
            WHERE (
                SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1
                FROM tblimage) = {selectedRowIndex}";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        // Open the connection
                        connection.Open();

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            // Execute the query
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Image2 cleared successfully for the selected row.");
                                pbright.Image = null; // Clear the PictureBox

                                // Call additional methods after saving the image
                                ConfigureDataGridView();  // Apply DataGridView settings
                                LoadBarangayNames();      // Load data into DataGridView
                                DisplayAllImageData();    // Display all image data
                                ConfigureDataGridView();  // Apply DataGridView settings again (if needed)
                                DisplayAllUserData();     // Display all user data
                                LoadImage2FromDatabase();
                            }
                            else
                            {
                                MessageBox.Show("No matching record found.");


                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a cell in the DataGridView.");
            }
        }

        private void btnDelete1_Click(object sender, EventArgs e)
        {
            // Ensure a cell is selected in the DataGridView
            if (dgLogo.SelectedCells.Count > 0)
            {
                // Get the selected row index from the DataGridView
                int selectedRowIndex = dgLogo.SelectedCells[0].RowIndex;

                // Connection string for MySQL/MariaDB
                string connectionString = "server=localhost;user=root;database=dbiBIM;password=;";

                // SQL query to delete Image3 for the specific row, using row position
                string query = $@"
            UPDATE tblimage 
            SET Image1 = NULL 
            WHERE (
                SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1
                FROM tblimage) = {selectedRowIndex}";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        // Open the connection
                        connection.Open();

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            // Execute the query
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Image1 cleared successfully for the selected row.");
                                pbleft.Image = null; // Clear the PictureBox

                                // Call additional methods after saving the image
                                LoadBarangayNames();      // Load data into DataGridView
                                DisplayAllImageData();    // Display all image data
                                ConfigureDataGridView();  // Apply DataGridView settings again (if needed)
                                DisplayAllUserData();     // Display all user data
                            }
                            else
                            {
                                MessageBox.Show("No matching record found.");

                            }
                          
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a cell in the DataGridView.");
            }
        }

        private void btnupload4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Load the selected image into the PictureBox
                pbsignaturemayor.Image = Image.FromFile(openFileDialog.FileName);

                // Update the officialImage variable with the newly selected image
                Image = pbsignaturemayor.Image;
            }
        }

        private void btnsave4_Click(object sender, EventArgs e)
        {
            // Generate a new MayorID (unique identifier)
            string mayorID = Guid.NewGuid().ToString();

            // Check if the PictureBox has an image
            if (pbsignaturemayor.Image != null)
            {
                // Convert the image to a byte array
                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    pbsignaturemayor.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    imageBytes = ms.ToArray();
                }

                // Check if an image already exists in the database
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // Check if there is already an image saved
                        string checkQuery = "SELECT COUNT(*) FROM tblmayorsignature";
                        MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection);
                        int imageCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                        string query;
                        if (imageCount > 0)
                        {
                            // If an image exists, update the existing one (no change to MayorID)
                            query = "UPDATE tblmayorsignature SET MayorSignature = @MayorSignature, MayorName = @MayorName WHERE MayorID = @MayorID";
                        }
                        else
                        {
                            // If no image exists, insert a new one with a new MayorID
                            query = "INSERT INTO tblmayorsignature (MayorID, MayorSignature, MayorName) VALUES (@MayorID, @MayorSignature, @MayorName)";
                        }

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@MayorID", mayorID);
                            command.Parameters.AddWithValue("@MayorSignature", imageBytes);
                            command.Parameters.AddWithValue("@MayorName", txtMayorName.Text);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Image and Mayor's name saved successfully!");
                                txtMayorName.Clear();
                                pbsignaturemayor.Image = null;
                                LoadMayorInfo(); // Refresh the DataGridView
                            }
                            else
                            {
                                MessageBox.Show("Failed to save data.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("No image selected in the PictureBox.");
            }
        }

        private void dgLogo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the click is on a valid cell (ignore header rows and out of bounds clicks)
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = dgLogo.Rows[e.RowIndex].Cells[e.ColumnIndex];

                // Check which column was clicked
                if (dgLogo.Columns[e.ColumnIndex].Name == "Image1" && cell.Value is Image)
                {
                    // Display Image1 in the pbleft PictureBox
                    pbleft.Image = (Image)cell.Value;
                }
                else if (dgLogo.Columns[e.ColumnIndex].Name == "Image2" && cell.Value is Image)
                {
                    // Display Image2 in the pbright PictureBox
                    pbright.Image = (Image)cell.Value;
                }
                else if (dgLogo.Columns[e.ColumnIndex].Name == "Image3" && cell.Value is Image)
                {
                    // Display Image3 in the pBsystemlogo PictureBox
                    pBsystemlogo.Image = (Image)cell.Value;
                }
            }
        }

        private void btnclear1_Click(object sender, EventArgs e)
        {
            // Clear the image in pbleft
            pbleft.Image = null;
        }


        private void btnclear2_Click(object sender, EventArgs e)
        {
            // Clear the image in pbright
            pbright.Image = null;
        }

        private void btnclear3_Click(object sender, EventArgs e)
        {
            // Clear the image in pBsystemlogo
            pBsystemlogo.Image = null;
        }

        private void cbAccountType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void cbAccountType_Enter(object sender, EventArgs e)
        {
            // Logic when cbAccountType gains focus
        }
        // This method should be added to your form class
        private void LoadMayorInfo()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT MayorID, MayorName, MayorSignature FROM tblmayorsignature";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgv_mayorinfo.Columns.Clear();
                    dgv_mayorinfo.DataSource = null;

                    // Add TextBox column for MayorID
                    dgv_mayorinfo.Columns.Add("MayorID", "Mayor ID");

                    // Add TextBox column for MayorName
                    dgv_mayorinfo.Columns.Add("MayorName", "Mayor Name");

                    // Add Image column for MayorSignature
                    DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                    imageColumn.Name = "MayorSignature";
                    imageColumn.HeaderText = "Signature";
                    dgv_mayorinfo.Columns.Add(imageColumn);

                    // Populate the DataGridView with data from the DataTable
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int rowIndex = dgv_mayorinfo.Rows.Add();
                        dgv_mayorinfo.Rows[rowIndex].Cells["MayorID"].Value = row["MayorID"].ToString();
                        dgv_mayorinfo.Rows[rowIndex].Cells["MayorName"].Value = row["MayorName"].ToString();

                        if (row["MayorSignature"] != DBNull.Value)
                        {
                            byte[] imageBytes = (byte[])row["MayorSignature"];
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                dgv_mayorinfo.Rows[rowIndex].Cells["MayorSignature"].Value = Image.FromStream(ms);
                            }
                        }
                    }

                    dgv_mayorinfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgv_mayorinfo.RowTemplate.Height = 35; // Adjust height as needed
                    dgv_mayorinfo.ColumnHeadersHeight = 40; // Adjust height as needed
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while loading data: " + ex.Message);
                }
            }
        }
        private void dgv_mayorinfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the click is on a valid row, not on the column headers
            if (e.RowIndex >= 0)
            {
                // Get the selected MayorID from the clicked row
                DataGridViewRow selectedRow = dgv_mayorinfo.Rows[e.RowIndex];
                string mayorID = selectedRow.Cells["MayorID"].Value.ToString();

                // Retrieve the data from the database using the selected MayorID
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT MayorName, MayorSignature FROM tblmayorsignature WHERE MayorID = @MayorID";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@MayorID", mayorID);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Display the retrieved data in the fields
                                txtMayorName.Text = reader["MayorName"].ToString();

                                // Display the image if it exists
                                if (reader["MayorSignature"] != DBNull.Value)
                                {
                                    byte[] imageBytes = (byte[])reader["MayorSignature"];
                                    using (MemoryStream ms = new MemoryStream(imageBytes))
                                    {
                                        pbsignaturemayor.Image = Image.FromStream(ms);
                                    }
                                }
                                else
                                {
                                    pbsignaturemayor.Image = null;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while retrieving data: " + ex.Message);
                    }
                }
            }
        }

        private void btndelete4_Click(object sender, EventArgs e)
        {
            if (dgv_mayorinfo.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgv_mayorinfo.SelectedRows[0];
                string mayorID = selectedRow.Cells["MayorID"].Value.ToString();

                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this mayor record?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = "DELETE FROM tblmayorsignature WHERE MayorID = @MayorID";
                            MySqlCommand command = new MySqlCommand(query, connection);
                            command.Parameters.AddWithValue("@MayorID", mayorID);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Mayor record deleted successfully.");
                                LoadMayorInfo(); // Refresh the DataGridView
                            }
                            else
                            {
                                MessageBox.Show("No records deleted. Please check the Mayor ID.");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("An error occurred while deleting data: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a mayor record to delete.");
            }
        }
        private void btnupdate4_Click(object sender, EventArgs e)
        {
            if (dgv_mayorinfo.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgv_mayorinfo.SelectedRows[0];
                string mayorID = selectedRow.Cells["MayorID"].Value.ToString();
                string mayorName = txtMayorName.Text;

                // Convert the signature image to a byte array if it exists
                byte[] mayorSignature = null;
                if (pbsignaturemayor.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pbsignaturemayor.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Use the appropriate format
                        mayorSignature = ms.ToArray();
                    }
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "UPDATE tblmayorsignature SET MayorName = @MayorName, MayorSignature = @MayorSignature WHERE MayorID = @MayorID";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@MayorName", mayorName);
                        command.Parameters.AddWithValue("@MayorSignature", (object)mayorSignature ?? DBNull.Value); // Use DBNull.Value for null images
                        command.Parameters.AddWithValue("@MayorID", mayorID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Mayor information updated successfully.");
                            LoadMayorInfo(); // Refresh the DataGridView
                        }
                        else
                        {
                            MessageBox.Show("No records updated. Please check the Mayor ID.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while updating data: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a mayor record to update.");
            }
        }
        private void LoadImagesIntoPictureBoxes(MySqlConnection connection)
        {
           
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate that all fields are filled
            if (string.IsNullOrWhiteSpace(txtgassistanceid.Text) || string.IsNullOrWhiteSpace(txtgassistance.Text))
            {
                MessageBox.Show("Please fill in all fields before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO tblgovernmentassistance (GovernmentAssistanceID, GovernmentAssistance) VALUES (@GovernmentAssistanceID, @GovernmentAssistance)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@GovernmentAssistanceID", txtgassistanceid.Text.Trim());
                        cmd.Parameters.AddWithValue("@GovernmentAssistance", txtgassistance.Text.Trim());

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Government Assistance saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh the DataGridView and clear the textboxes
                        LoadGovernmentAssistance();
                        ClearFieldsGovernmentAssistance();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearFieldsGovernmentAssistance()
        {
            txtgassistanceid.Clear();
            txtgassistance.Clear();
        }
        private void btndelete_Click(object sender, EventArgs e)
        {
            if (dgvgvnmtassistance.SelectedRows.Count > 0)
            {
                // Show a confirmation dialog before deleting
                DialogResult result = MessageBox.Show("Are you sure you want to delete this Government Assistance?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "DELETE FROM tblgovernmentassistance WHERE GovernmentAssistanceID = @GovernmentAssistanceID";
                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {
                                string selectedID = dgvgvnmtassistance.SelectedRows[0].Cells["GovernmentAssistanceID"].Value.ToString();
                                cmd.Parameters.AddWithValue("@GovernmentAssistanceID", selectedID);

                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Government Assistance deleted successfully!");

                                // Refresh the DataGridView
                                LoadGovernmentAssistance();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
                // If the user selects "No", do nothing and exit the method
            }
            else
            {
                MessageBox.Show("Please select a Government Assistance to delete.");
            }
        }
        private void LoadGovernmentAssistance()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT GovernmentAssistanceID, GovernmentAssistance FROM tblgovernmentassistance";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dgvgvnmtassistance.DataSource = dataTable;

                        // Set custom header names
                        dgvgvnmtassistance.Columns["GovernmentAssistanceID"].HeaderText = "Government Assitance ID";
                        dgvgvnmtassistance.Columns["GovernmentAssistance"].HeaderText = "Government Assitance";

                        // Adjust column widths
                        dgvgvnmtassistance.Columns["GovernmentAssistanceID"].Width = 350; // Adjust as needed
                        dgvgvnmtassistance.Columns["GovernmentAssistance"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                        // Set row height
                        dgvgvnmtassistance.RowTemplate.Height = 35; // Adjust height as needed

                        // Set header row height
                        dgvgvnmtassistance.ColumnHeadersHeight = 40; // Adjust height as needed
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvgvnmtassistance_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvgvnmtassistance.Rows[e.RowIndex];
                txtgassistanceid.Text = row.Cells["GovernmentAssistanceID"].Value.ToString();
                txtgassistance.Text = row.Cells["GovernmentAssistance"].Value.ToString();
            }
        }

        private void btnsavecomm_Click(object sender, EventArgs e)
        {
            {
                // Validate that all fields are filled
                if (string.IsNullOrWhiteSpace(txtcommitteeID.Text) || string.IsNullOrWhiteSpace(txtCommittee.Text))
                {
                    MessageBox.Show("Please fill in all fields before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO tblcommittee (CommitteeID, Committee) VALUES (@CommitteeID, @Committee)";
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@CommitteeID", txtcommitteeID.Text.Trim());
                            cmd.Parameters.AddWithValue("@Committee", txtCommittee.Text.Trim());

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Committee saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Refresh the DataGridView and clear the textboxes
                            LoadCommittee();
                            ClearFieldsCommittee();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ClearFieldsCommittee()
        {
            txtcommitteeID.Clear();
            txtCommittee.Clear();
        }
        private void btndeletecomm_Click(object sender, EventArgs e)
        {
            if (dgvcommittee.SelectedRows.Count > 0)
            {
                // Show a confirmation dialog before deleting
                DialogResult result = MessageBox.Show("Are you sure you want to delete this Committee?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "DELETE FROM tblcommittee WHERE CommitteeID = @CommitteeID";
                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {
                                string selectedID = dgvcommittee.SelectedRows[0].Cells["CommitteeID"].Value.ToString();
                                cmd.Parameters.AddWithValue("@CommitteeID", selectedID);

                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Committee deleted successfully!");

                                // Refresh the DataGridView
                                LoadCommittee();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
                // If the user selects "No", do nothing and exit the method
            }
            else
            {
                MessageBox.Show("Please select a Committee to delete.");
            }
        }
        private void LoadCommittee()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT CommitteeID, Committee FROM tblcommittee";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dgvcommittee.DataSource = dataTable;

                        // Set custom header names
                        dgvcommittee.Columns["CommitteeID"].HeaderText = "Committee ID";
                        dgvcommittee.Columns["Committee"].HeaderText = "Committee";

                        // Adjust column widths
                        dgvcommittee.Columns["CommitteeID"].Width = 350; // Adjust as needed
                        dgvcommittee.Columns["Committee"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                        // Set row height
                        dgvcommittee.RowTemplate.Height = 35; // Adjust height as needed

                        // Set header row height
                        dgvcommittee.ColumnHeadersHeight = 40; // Adjust height as needed
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgvcommittee_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvcommittee.Rows[e.RowIndex];
                txtcommitteeID.Text = row.Cells["CommitteeID"].Value.ToString();
                txtCommittee.Text = row.Cells["Committee"].Value.ToString();
            }
        }
    }
}
