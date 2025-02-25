using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
namespace iBIM_GIS
{
    public partial class IBIM_Login_frm : Form
    {
        private int loginAttempts = 0;
        private const int maxLoginAttempts = 3;
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        private Timer timer;
        private DateTime lastUpdate;

        public IBIM_Login_frm()
        {
            InitializeComponent();
            this.AcceptButton = btnlogin;
            txtpassword.UseSystemPasswordChar = true;

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();

            cbshowpassword.CheckedChanged += cbshowpassword_CheckedChanged;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (now.Minute != lastUpdate.Minute)
            {
                label9.Text = now.ToString("hh:mm tt").ToUpper();
                lastUpdate = now;
            }
        }
        private void btnlogin_Click(object sender, EventArgs e)
        {
            // Check for network availability, but allow login regardless
            if (!IsNetworkAvailable())
            {
                DialogResult result = MessageBox.Show(
                    "No internet connection detected. You are logging in offline.\n\n" +
                    "Please note:\n" +
                    "- Some features may not work properly.\n" +
                    "- Data synchronization will be delayed until you reconnect.\n\n" +
                    "Do you want to continue logging in?",
                    "Network Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No)
                {
                    return; // Exit the login process if the user chooses not to continue
                }
            }

            // Proceed with the login process
            string username = txtusername.Text;
            string encryptedPassword = CaesarCipherEncrypt(txtpassword.Text, 5);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(encryptedPassword))
            {
                MessageBox.Show("Please input Username and Password.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT password, AccountType FROM tbluser WHERE BINARY username = @username";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        string storedEncryptedPassword = Convert.ToString(command.ExecuteScalar());

                        if (string.IsNullOrEmpty(storedEncryptedPassword))
                        {
                            MessageBox.Show("Login Failed! Username is not registered.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string storedDecryptedPassword = CaesarCipherDecrypt(storedEncryptedPassword, 5);

                        if (encryptedPassword == storedEncryptedPassword)
                        {
                            string accountTypeQuery = "SELECT AccountType FROM tbluser WHERE username = @username";
                            using (MySqlCommand accountTypeCommand = new MySqlCommand(accountTypeQuery, connection))
                            {
                                accountTypeCommand.Parameters.AddWithValue("@username", username);
                                string accountType = Convert.ToString(accountTypeCommand.ExecuteScalar());

                                Form dashboard = null;

                                switch (accountType)
                                {
                                    case "Admin":
                                        MessageBox.Show("Admin Login Successful!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        dashboard = new IBIM_BrgyMainPanel_frm
                                        {
                                            LoggedInAccountType = accountType // Pass the AccountType
                                        };
                                        break;
                                    case "Secretary":
                                        MessageBox.Show("SECRETARY Login Successful!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        dashboard = new IBIM_BrgyMainPanel_frm
                                        {
                                            LoggedInAccountType = accountType // Pass the AccountType
                                        };
                                        break;
                                    case "Treasurer":
                                        MessageBox.Show("TREASURER Login Successful!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        dashboard = new IBIM_BrgyMainPanel_frm
                                        {
                                            LoggedInAccountType = accountType // Pass the AccountType
                                        };
                                        break;
                                    default:
                                        MessageBox.Show("Login successful, but account type is not recognized.", "Account Type Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        break;
                                }

                                if (dashboard != null)
                                {
                                    this.Hide();
                                    dashboard.Show();
                                }
                            }

                            txtusername.Text = "";
                            txtpassword.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Login failed! Incorrect password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        // Method to check network availability
        private bool IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
        private void CheckMaxLoginAttemptsOnLoad()
        {

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
        private void Form1_Load_1(object sender, EventArgs e)
        {
            CheckMaxLoginAttemptsOnLoad();
            if (loginAttempts >= maxLoginAttempts)
            {
                Application.Exit();
            }
            LoadImages();
            LoadBarangayName();
        }
        private void cbshowpassword_CheckedChanged(object sender, EventArgs e)
        {
            if (cbshowpassword.Checked)
            {
                txtpassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtpassword.UseSystemPasswordChar = true;
            }
        }
        private void btnexit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void LoadBarangayName()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    string query = "SELECT BarangayName FROM tblsetbrgyname LIMIT 1"; // Get the first BarangayName

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        object result = cmd.ExecuteScalar(); // Fetch single value (BarangayName)

                        if (result != null)
                        {
                            lblbarangayname.Text = result.ToString(); // Display in the label
                        }
                        else
                        {
                            lblbarangayname.Text = "No Barangay Name Found.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Barangay name: " + ex.Message);
            }
        }
        private void LoadImages()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    string query = "SELECT Image1, Image2, Image3 FROM tblimage";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Load Image1 into pblogo1
                                if (!reader.IsDBNull(reader.GetOrdinal("Image1")))
                                {
                                    byte[] imageBytes1 = (byte[])reader["Image1"];
                                    using (MemoryStream ms1 = new MemoryStream(imageBytes1))
                                    {
                                        pblogo1.Image = Image.FromStream(ms1);
                                    }
                                }

                                // Load Image2 into pblogo2
                                if (!reader.IsDBNull(reader.GetOrdinal("Image2")))
                                {
                                    byte[] imageBytes2 = (byte[])reader["Image2"];
                                    using (MemoryStream ms2 = new MemoryStream(imageBytes2))
                                    {
                                        pblogo2.Image = Image.FromStream(ms2);
                                    }
                                }

                                // Load Image3 into pbsystemlogo
                                if (!reader.IsDBNull(reader.GetOrdinal("Image3")))
                                {
                                    byte[] imageBytes3 = (byte[])reader["Image3"];
                                    using (MemoryStream ms3 = new MemoryStream(imageBytes3))
                                    {
                                        pblogo3.Image = Image.FromStream(ms3);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("No images found in the database.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading images: " + ex.Message);
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) // Check if the ESC key is pressed
            {
                this.Close(); // Close the form
                return true;  // Indicate that the key press has been handled
            }
            return base.ProcessCmdKey(ref msg, keyData); // Call the base method for other keys
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        // Attach events to all textboxes
    }
}
