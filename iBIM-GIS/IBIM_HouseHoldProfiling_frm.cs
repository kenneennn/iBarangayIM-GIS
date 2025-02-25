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
    public partial class IBIM_HouseHoldProfiling_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        public IBIM_HouseHoldProfiling_frm()
        {
            InitializeComponent();
            dgvresidentrecord.CellClick += dgvresidentrecord_CellClick;
            txtPHILID.Enabled = false;
            txtCategory.Enabled = false;
            cbHHPM.SelectedIndexChanged += cbHHPM_SelectedIndexChanged;
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(IBIM_HouseHoldProfiling_frm_KeyDown);
        }
        private void IBIM_HouseHoldProfiling_frm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                RefreshAllData();
                LoadHouseHoldNumbers();
                LoadPurokNames();
                LoadHouseHoldNumbers();
                LoaddataGridViewdateofvisit();
                LoadPurokNames();
                LoadClassificationByAgeAndHRG();
                LoadResidentRecords();
            }
            
        }

        private void RefreshAllData()
        {
            try
            {
                LoadHouseHoldNumbers();
                LoadPurokNames();
                LoadHouseHoldNumbers();
                LoaddataGridViewdateofvisit();
                LoadPurokNames();
                LoadClassificationByAgeAndHRG();
                LoadResidentRecords();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing data: " + ex.Message);
            }
        }
        private void LoadHouseHoldNumbers()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT `HouseHoldNo` FROM tblmapping";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    cbHHNo.Items.Clear();
                    while (reader.Read())
                    {
                        cbHHNo.Items.Add(reader["HouseHoldNo"].ToString());
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
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
                    cbPurokName.Items.Clear();
                    while (reader.Read())
                    {
                        cbPurokName.Items.Add(reader["PurokName"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void IBIM_ResidentProfiling_frm_Load(object sender, EventArgs e)
        {
            LoadHouseHoldNumbers();
            LoaddataGridViewdateofvisit();
            LoadPurokNames();
            LoadClassificationByAgeAndHRG();
            LoadResidentRecords();
            LoadGovernmentAssistance();
            DateTime selectedDate = dateTimePickerBday.Value.Date;
            

        }
        private void LoadGovernmentAssistance()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT GovernmentAssistance FROM tblgovernmentassistance";  // SQL Query

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            cbMember.Items.Clear(); // Clear previous items

                            while (reader.Read())
                            {
                                cbMember.Items.Add(reader["GovernmentAssistance"].ToString());
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
        private void LoadClassificationByAgeAndHRG()
        {
            cbClassifbyAgeandHRG.Items.Clear();
            cbClassifbyAgeandHRG.Items.AddRange(new string[]
            {
        "Newborn",
        "Infant",
        "Under-five",
        "School-Aged Children",
        "Adolescents",
        "Pregnant",
        "Persons with Disability",
        "Adolescent Pregnant",
        "Post Partum",
        "WRA",
        "Senior Citizen",
        "Adult"

            });
            cbClassifbyAgeandHRG.SelectedIndex = -1; // Optionally, set no default selection
        }
        private string GenerateResidentID()
        {
            return "RES" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        private void ClearForm()
        {
            txtresidentname.Clear();
            cbHHNo.SelectedIndex = -1;
            cbrelHHHead.SelectedIndex = -1;
            cbNHTSHH.SelectedIndex = -1;
            cbIPrNIP.SelectedIndex = -1;
            cbHHPM.SelectedIndex = -1;
            txtPHILID.Clear();
            txtCategory.Clear();
            cbSex.SelectedIndex = -1;
            dateTimePickerBday.Value = DateTime.Now;
            txtAge.Clear();
            cbPurokName.SelectedIndex = -1;
            cbClassifbyAgeandHRG.SelectedIndex = -1;
            txtremarks.Clear();
            cbeduattainment.SelectedIndex = -1;
            cbsanitarytoilet.SelectedIndex = -1;
            txtOccupation.Clear();
            cbusinglevel.SelectedIndex = -1;
            cbhousesurface.SelectedIndex = -1;
            cbStatus.SelectedIndex = -1;
            cbSCstatus.SelectedIndex = -1;
            txtfirstquarter.Clear();
            txtsecondquarter.Clear();
            txtthirdquarter.Clear();
            txtfourthquater.Clear();
            cbtransferre.SelectedIndex = -1;
            txtlastcurrentaddress.Clear();
            CBclassification.SelectedIndex = -1;
            cbMember.SelectedIndex = -1;
        }
        private void LoadResidentRecords()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT  ResidentID, ResidentName, HouseHoldNo, Status FROM tblresidentprofiling";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvresidentrecord.DataSource = dt;
                    dgvresidentrecord.RowTemplate.Height = 35; // Adjust height as needed
                    dgvresidentrecord.ColumnHeadersHeight = 40; // Adjust height as needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void LoaddataGridViewdateofvisit()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ResidentID, ResidentName, HouseHoldNo, PurokName, RelationshipHHHead, Status FROM tblresidentprofiling";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridViewdateofvisit.DataSource = dt;
                    dataGridViewdateofvisit.RowTemplate.Height = 35; // Adjust height as needed
                    dataGridViewdateofvisit.ColumnHeadersHeight = 40; // Adjust height as needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dgvresidentrecord_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    // Retrieve ResidentID and ResidentName from the selected row
                    DataGridViewRow row = dgvresidentrecord.Rows[e.RowIndex];
                    string residentID = row.Cells["ResidentID"].Value.ToString();
                    string residentName = row.Cells["ResidentName"].Value.ToString();

                    // Query database to fetch all details for the selected ResidentID and ResidentName
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "SELECT * FROM tblresidentprofiling WHERE ResidentID = @ResidentID AND ResidentName = @ResidentName";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ResidentID", residentID);
                            cmd.Parameters.AddWithValue("@ResidentName", residentName);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Populate form fields with data from the database
                                    txtresidentname.Text = reader["ResidentName"].ToString();
                                    cbHHNo.SelectedItem = reader["HouseHoldNo"].ToString();
                                    cbrelHHHead.SelectedItem = reader["RelationshipHHHead"].ToString();
                                    cbNHTSHH.SelectedItem = reader["NHTSHH"].ToString();
                                    cbIPrNIP.SelectedItem = reader["IPorNonIP"].ToString();
                                    cbHHPM.SelectedItem = reader["HHHEADPhilhealthMember"].ToString();
                                    txtPHILID.Text = reader["PhilhealthID"].ToString();
                                    txtCategory.Text = reader["Category"].ToString();
                                    cbSex.SelectedItem = reader["Sex"].ToString();
                                    dateTimePickerBday.Value = Convert.ToDateTime(reader["Birthdate"]);
                                    txtAge.Text = reader["Age"].ToString();
                                    cbPurokName.SelectedItem = reader["PurokName"].ToString();
                                    cbClassifbyAgeandHRG.SelectedItem = reader["ClassificationbyAgeandHRG"].ToString();
                                    CBclassification.SelectedItem = reader["HealthConditions"].ToString();
                                    cbSCstatus.SelectedItem = reader["SCStatus"].ToString();
                                    txtremarks.Text = reader["Remarks"].ToString();
                                    cbeduattainment.SelectedItem = reader["EducationalAttainment"].ToString();
                                    cbsanitarytoilet.SelectedItem = reader["SanitaryToilet"].ToString();
                                    cbusinglevel.SelectedItem = reader["UsingLevel"].ToString();
                                    txtOccupation.Text = reader["Occupation"].ToString();
                                    cbhousesurface.SelectedItem = reader["ConcreteSurface"].ToString();
                                    cbStatus.SelectedItem = reader["Status"].ToString();
                                    cbMember.SelectedItem = reader["cbMember"].ToString();
                                    txtlastcurrentaddress.Text = reader["LastCurrentAddress"].ToString();
                                    cbtransferre.SelectedItem = reader["Transferre"].ToString();
                                    txtfirstquarter.Text = reader["FirstQuarter"].ToString();
                                    txtsecondquarter.Text = reader["SecondQuarter"].ToString();
                                    txtthirdquarter.Text = reader["ThirdQuarter"].ToString();
                                    txtfourthquater.Text = reader["FourthQuarter"].ToString();

                                    // Disable save button
                                    btnSave.Enabled = false;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void LoadResidentRecords(string searchQuery = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT  ResidentID, ResidentName, HouseHoldNo, Status FROM tblresidentprofiling";
                    if (!string.IsNullOrWhiteSpace(searchQuery))
                    {
                        query += " WHERE HouseHoldNo LIKE @SearchQuery OR ResidentName LIKE @SearchQuery";
                    }
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (!string.IsNullOrWhiteSpace(searchQuery))
                    {
                        cmd.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                    }
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvresidentrecord.DataSource = dt;
                    dgvresidentrecord.RowTemplate.Height = 35; // Adjust height as needed
                    dgvresidentrecord.ColumnHeadersHeight = 40; // Adjust height as needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void cbHHPM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbHHPM.SelectedItem != null && cbHHPM.SelectedItem.ToString().Trim().Equals("YES", StringComparison.OrdinalIgnoreCase))
            {
                // Enable txtPHILID and txtCategory if "YES" is selected
                txtPHILID.Enabled = true;
                txtPHILID.Clear(); // Clear any existing placeholder or data

                txtCategory.Enabled = true;
                txtCategory.Clear(); // Clear any existing placeholder or data

                txtremarks.Enabled = true;
                txtremarks.Clear(); // Clear any existing placeholder or data
            }
            else
            {
                // Disable txtPHILID and txtCategory if "NO" or other values are selected
                txtPHILID.Enabled = false;
                txtPHILID.Text = ""; // Display 'Disabled' text when disabled

                txtCategory.Enabled = false;
                txtCategory.Text = ""; // Display 'Disabled' text when disabled

                txtremarks.Enabled = false;
                txtremarks.Text = ""; // Display 'Disabled' text when disabled
            }
        }
        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtresidentname.Text) ||
                cbHHNo.SelectedIndex == -1 ||
                cbrelHHHead.SelectedIndex == -1 ||
                cbNHTSHH.SelectedIndex == -1 ||
                cbIPrNIP.SelectedIndex == -1 ||
                cbHHPM.SelectedIndex == -1 ||
                cbSex.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtAge.Text) ||
                cbPurokName.SelectedIndex == -1 ||
                cbStatus.SelectedIndex == -1 ||
                cbClassifbyAgeandHRG.SelectedIndex == -1 ||
                cbSCstatus.SelectedIndex == -1 ||
                cbeduattainment.SelectedIndex == -1 ||
                cbsanitarytoilet.SelectedIndex == -1 ||
                cbusinglevel.SelectedIndex == -1  ||
                string.IsNullOrWhiteSpace(txtOccupation.Text) ||
                cbhousesurface.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all required fields before saving.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string houseHoldNo = cbHHNo.SelectedItem.ToString();
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string residentID = GenerateResidentID();
                    string query = $@"INSERT INTO tblresidentprofiling 
                         (ResidentID, ResidentName, HouseHoldNo, RelationshipHHHead, NHTSHH, IPorNonIP, HHHEADPhilhealthMember, 
                          PhilhealthID, Category, Sex, Birthdate, Age, PurokName, ClassificationbyAgeandHRG, SCStatus, Remarks, 
                          EducationalAttainment, SanitaryToilet, UsingLevel, Occupation, ConcreteSurface, Status, Transferre, 
                          LastCurrentAddress, HealthConditions,cbMember) 
                         VALUES (@ResidentID, @ResidentName, @HouseHoldNo, @RelationshipHHHead, @NHTSHH, @IPorNonIP, 
                                 @HHHEADPhilhealthMember, @PhilhealthID, @Category, @Sex, @Birthdate, @Age, @PurokName, 
                                 @ClassificationbyAgeandHRG, @SCStatus, @Remarks, @EducationalAttainment, @SanitaryToilet, 
                                 @UsingLevel, @Occupation, @ConcreteSurface, @Status, @Transferre, @LastCurrentAddress, @HealthConditions,@cbMember)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ResidentID", residentID);
                    cmd.Parameters.AddWithValue("@ResidentName", txtresidentname.Text);
                    cmd.Parameters.AddWithValue("@HouseHoldNo", houseHoldNo);
                    cmd.Parameters.AddWithValue("@RelationshipHHHead", cbrelHHHead.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@NHTSHH", cbNHTSHH.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@IPorNonIP", cbIPrNIP.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@HHHEADPhilhealthMember", cbHHPM.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@PhilhealthID", txtPHILID.Text);
                    cmd.Parameters.AddWithValue("@Category", txtCategory.Text);
                    cmd.Parameters.AddWithValue("@Sex", cbSex.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Birthdate", dateTimePickerBday.Value.Date.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Age", txtAge.Text);
                    cmd.Parameters.AddWithValue("@PurokName", cbPurokName.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@ClassificationbyAgeandHRG", cbClassifbyAgeandHRG.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@SCStatus", cbSCstatus.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Remarks", txtremarks.Text);
                    cmd.Parameters.AddWithValue("@EducationalAttainment", cbSCstatus.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@SanitaryToilet", cbsanitarytoilet.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@UsingLevel", cbusinglevel.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Occupation", txtOccupation.Text);
                    cmd.Parameters.AddWithValue("@ConcreteSurface", cbhousesurface.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Status", cbStatus.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@cbMember", cbMember.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Transferre", cbtransferre.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@LastCurrentAddress", txtlastcurrentaddress.Text);
                    cmd.Parameters.AddWithValue("@HealthConditions", CBclassification.SelectedItem.ToString()); // Add this line for health conditions
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Resident profile saved successfully!");
                    ClearForm();
                    LoadResidentRecords();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (dgvresidentrecord.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to delete.", "No Record Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataGridViewRow selectedRow = dgvresidentrecord.SelectedRows[0];
            string residentID = selectedRow.Cells["ResidentID"].Value.ToString();
            DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM tblresidentprofiling WHERE ResidentID = @ResidentID";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@ResidentID", residentID);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadResidentRecords();
                            ClearForm();
                        }
                        else
                        {
                            MessageBox.Show("No record found to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnClear_Click_1(object sender, EventArgs e)
        {
            ClearForm();
            btnSave.Enabled = true;
        }
        private void btnupdateprofile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtresidentname.Text) ||
                cbHHNo.SelectedIndex == -1 ||
                cbrelHHHead.SelectedIndex == -1 ||
                cbNHTSHH.SelectedIndex == -1 ||
                cbIPrNIP.SelectedIndex == -1 ||
                cbHHPM.SelectedIndex == -1 ||
                cbStatus.SelectedIndex == -1 ||
                cbSCstatus.SelectedIndex == -1 ||
                cbSex.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtAge.Text) ||
                cbPurokName.SelectedIndex == -1 ||
                cbClassifbyAgeandHRG.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all required fields before updating.",
                                "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                DataGridViewRow selectedRow = dgvresidentrecord.CurrentRow;
                if (selectedRow == null)
                {
                    MessageBox.Show("Please select a resident to update.",
                                    "No Resident Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string residentID = selectedRow.Cells["ResidentID"].Value.ToString();
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                UPDATE tblresidentprofiling 
                SET ResidentName = @ResidentName, HouseHoldNo = @HouseHoldNo, 
                    RelationshipHHHead = @RelationshipHHHead, NHTSHH = @NHTSHH, 
                    IPorNonIP = @IPorNonIP, HHHEADPhilhealthMember = @HHHEADPhilhealthMember, 
                    PhilhealthID = @PhilhealthID, Category = @Category, Sex = @Sex, 
                    Birthdate = @Birthdate, Age = @Age, PurokName = @PurokName, 
                    ClassificationbyAgeandHRG = @ClassificationbyAgeandHRG,SCStatus = @SCStatus, Remarks = @Remarks, 
                    EducationalAttainment = @EducationalAttainment, SanitaryToilet = @SanitaryToilet, 
                    UsingLevel = @UsingLevel, Occupation = @Occupation, 
                    ConcreteSurface = @ConcreteSurface, Status = @Status, Transferre = @Transferre, LastCurrentAddress = @LastCurrentAddress, HealthConditions=@HealthConditions,cbMember = @cbMember
                WHERE ResidentID = @ResidentID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ResidentID", residentID);
                    cmd.Parameters.AddWithValue("@ResidentName", txtresidentname.Text);
                    cmd.Parameters.AddWithValue("@HouseHoldNo", cbHHNo.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@RelationshipHHHead", cbrelHHHead.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@NHTSHH", cbNHTSHH.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@IPorNonIP", cbIPrNIP.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@HHHEADPhilhealthMember", cbHHPM.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@PhilhealthID", txtPHILID.Text);
                    cmd.Parameters.AddWithValue("@Category", txtCategory.Text);
                    cmd.Parameters.AddWithValue("@Sex", cbSex.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Birthdate", dateTimePickerBday.Value.Date);
                    cmd.Parameters.AddWithValue("@Age", txtAge.Text);
                    cmd.Parameters.AddWithValue("@PurokName", cbPurokName.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@ClassificationbyAgeandHRG", cbClassifbyAgeandHRG.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@SCStatus", cbSCstatus.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Remarks", txtremarks.Text);
                    cmd.Parameters.AddWithValue("@EducationalAttainment", cbeduattainment.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@SanitaryToilet", cbsanitarytoilet.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@UsingLevel", cbusinglevel.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Occupation", txtOccupation.Text);
                    cmd.Parameters.AddWithValue("@ConcreteSurface", cbhousesurface.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@Status", cbStatus.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@cbMember", cbMember.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@LastCurrentAddress", txtlastcurrentaddress.Text);
                    cmd.Parameters.AddWithValue("@Transferre", cbtransferre.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@HealthConditions", CBclassification.SelectedItem.ToString());
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Profile updated successfully!",
                                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadResidentRecords(); // Refresh DataGridView
                    ClearForm(); // Clear input fields
                    btnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Update Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtsearch_TextChanged_1(object sender, EventArgs e)
        {
            string searchQuery = txtsearch.Text.Trim();
            LoadResidentRecords(searchQuery);
        }
        private void btnsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtResidentID.Text))
                {
                    MessageBox.Show("Please enter a valid Household Number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MySqlDataAdapter sDAa = new MySqlDataAdapter();
                DataSet1 aDSa = new DataSet1();
                DataSet dsetimage = new DataSet(); // For the image dataset
                string barangayName = ""; // To hold the BarangayName value

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Fetch BarangayName
                    string brgyNameQuery = "SELECT BarangayName FROM tblsetbrgyname LIMIT 1;";
                    using (MySqlCommand brgyCmd = new MySqlCommand(brgyNameQuery, conn))
                    {
                        object result = brgyCmd.ExecuteScalar();
                        barangayName = result?.ToString() ?? "Unknown Barangay";
                    }

                    // Fetch resident profiling data
                    string query = @"
            SELECT 
                HouseHoldNo, ResidentID, ResidentName, NHTSHH, IPorNonIP, 
                HHHEADPhilhealthMember, PhilhealthID, Category, Sex, 
                Birthdate, Age, ClassificationbyAgeandHRG, Remarks, 
                RelationshipHHHead, 
                IFNULL(FirstQuarter, '') AS FirstQuarter, 
                IFNULL(SecondQuarter, '') AS SecondQuarter, 
                IFNULL(ThirdQuarter, '') AS ThirdQuarter, 
                IFNULL(FourthQuarter, '') AS FourthQuarter
            FROM tblresidentprofiling
            WHERE HouseHoldNo = @HouseHoldNo AND Status = 'Active'
            ORDER BY CASE WHEN RelationshipHHHead = 'Head' THEN 0 ELSE 1 END, ResidentName ASC;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@HouseHoldNo", txtResidentID.Text.Trim());
                    sDAa.SelectCommand = cmd;

                    sDAa.Fill(aDSa.Tables[0]);

                    if (aDSa.Tables[0].Rows.Count == 0)
                    {
                        MessageBox.Show("No matching active records found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Fetch all image data (no filtering)
                    string imageQuery = "SELECT * FROM tblimage;";
                    MySqlDataAdapter imageAdapter = new MySqlDataAdapter(imageQuery, conn);
                    imageAdapter.Fill(dsetimage, "tblimage");
                }

            

                // Set up report parameters
                ReportParameter[] reportParameters = new ReportParameter[]
                {
            new ReportParameter("txtResidentName", aDSa.Tables[0].Rows[0]["ResidentName"].ToString()), // Household head name
            new ReportParameter("BarangayName", barangayName)
            
                };

                string reportPath = Path.Combine(Environment.CurrentDirectory, "Report", "HHReport.rdlc");
                rv_HHprofile.LocalReport.ReportPath = reportPath;
                rv_HHprofile.LocalReport.DataSources.Clear();

                // Add the resident profiling data source
                rv_HHprofile.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("tblresidentprofiling", aDSa.Tables[0]));

                // Add the image data source
                rv_HHprofile.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("dsetimage", dsetimage.Tables["tblimage"]));

                rv_HHprofile.LocalReport.SetParameters(reportParameters);
                rv_HHprofile.DocumentMapCollapsed = true;
                rv_HHprofile.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                rv_HHprofile.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;
                rv_HHprofile.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cbtransferre_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbtransferre.SelectedItem != null)
            {
                if (cbtransferre.SelectedItem.ToString() == "YES")
                {
                    txtlastcurrentaddress.Enabled = true;
                    txtlastcurrentaddress.Clear();
                }
                else
                {
                    txtlastcurrentaddress.Enabled = false;
                    txtlastcurrentaddress.Text = ""; 
                }
            }
        }

        private void rv_HHprofile_Load(object sender, EventArgs e)
        {

        }

        private void dateTimePickerBday_ValueChanged_1(object sender, EventArgs e)
        {
            int age = DateTime.Now.Year - dateTimePickerBday.Value.Year;
            if (DateTime.Now.DayOfYear < dateTimePickerBday.Value.DayOfYear)
                age--;
            txtAge.Text = age.ToString();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cbHHNo.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a HouseHoldNo to update quarterly data.",
                                "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string houseHoldNo = cbHHNo.SelectedItem.ToString();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string selectQuery = @"
                SELECT FirstQuarter, SecondQuarter, ThirdQuarter, FourthQuarter 
                FROM tblresidentprofiling 
                WHERE HouseHoldNo = @HouseHoldNo 
                LIMIT 1"; // Only need one record to get existing values
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@HouseHoldNo", houseHoldNo);
                    MySqlDataReader reader = selectCmd.ExecuteReader();
                    string existingFirstQuarter = "", existingSecondQuarter = "",
                           existingThirdQuarter = "", existingFourthQuarter = "";
                    if (reader.Read())
                    {
                        existingFirstQuarter = reader["FirstQuarter"].ToString();
                        existingSecondQuarter = reader["SecondQuarter"].ToString();
                        existingThirdQuarter = reader["ThirdQuarter"].ToString();
                        existingFourthQuarter = reader["FourthQuarter"].ToString();
                    }
                    reader.Close();
                    string firstQuarter = string.IsNullOrWhiteSpace(txtfirstquarter.Text)
                                          ? existingFirstQuarter : txtfirstquarter.Text;
                    string secondQuarter = string.IsNullOrWhiteSpace(txtsecondquarter.Text)
                                           ? existingSecondQuarter : txtsecondquarter.Text;
                    string thirdQuarter = string.IsNullOrWhiteSpace(txtthirdquarter.Text)
                                          ? existingThirdQuarter : txtthirdquarter.Text;
                    string fourthQuarter = string.IsNullOrWhiteSpace(txtfourthquater.Text)
                                           ? existingFourthQuarter : txtfourthquater.Text;
                    string updateQuery = @"
                UPDATE tblresidentprofiling 
                SET FirstQuarter = @FirstQuarter, SecondQuarter = @SecondQuarter, 
                    ThirdQuarter = @ThirdQuarter, FourthQuarter = @FourthQuarter
                WHERE HouseHoldNo = @HouseHoldNo";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@FirstQuarter", firstQuarter);
                    updateCmd.Parameters.AddWithValue("@SecondQuarter", secondQuarter);
                    updateCmd.Parameters.AddWithValue("@ThirdQuarter", thirdQuarter);
                    updateCmd.Parameters.AddWithValue("@FourthQuarter", fourthQuarter);
                    updateCmd.Parameters.AddWithValue("@HouseHoldNo", houseHoldNo);
                    updateCmd.ExecuteNonQuery();
                    MessageBox.Show("Quarterly data updated successfully for all matching residents!",
                                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadResidentRecords(); // Refresh the DataGridView
                    ClearForm(); // Clear input fields
                    btnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Update Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewdateofvisit_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = dgvresidentrecord.Rows[e.RowIndex];
                    string residentID = row.Cells["ResidentID"].Value.ToString();
                    string residentName = row.Cells["ResidentName"].Value.ToString();
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "SELECT * FROM tblresidentprofiling WHERE ResidentID = @ResidentID AND ResidentName = @ResidentName";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ResidentID", residentID);
                            cmd.Parameters.AddWithValue("@ResidentName", residentName);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Populate form fields with data from the database
                                    txtresidentname.Text = reader["ResidentName"].ToString();
                                    cbHHNo.SelectedItem = reader["HouseHoldNo"].ToString();
                                    txthouseholdno.Text = reader["HouseHoldNo"].ToString();
                                    cbrelHHHead.SelectedItem = reader["RelationshipHHHead"].ToString();
                                    cbNHTSHH.SelectedItem = reader["NHTSHH"].ToString();
                                    cbIPrNIP.SelectedItem = reader["IPorNonIP"].ToString();
                                    cbHHPM.SelectedItem = reader["HHHEADPhilhealthMember"].ToString();
                                    txtPHILID.Text = reader["PhilhealthID"].ToString();
                                    txtCategory.Text = reader["Category"].ToString();
                                    cbSex.SelectedItem = reader["Sex"].ToString();
                                    dateTimePickerBday.Value = Convert.ToDateTime(reader["Birthdate"]);
                                    txtAge.Text = reader["Age"].ToString();
                                    cbPurokName.SelectedItem = reader["PurokName"].ToString();
                                    cbClassifbyAgeandHRG.SelectedItem = reader["ClassificationbyAgeandHRG"].ToString();
                                    CBclassification.SelectedItem = reader["HealthConditions"].ToString();
                                    cbSCstatus.SelectedItem = reader["SCStatus"].ToString();
                                    txtremarks.Text = reader["Remarks"].ToString();
                                    cbeduattainment.SelectedItem = reader["EducationalAttainment"].ToString();
                                    cbsanitarytoilet.SelectedItem = reader["SanitaryToilet"].ToString();
                                    cbusinglevel.SelectedItem = reader["UsingLevel"].ToString();
                                    txtOccupation.Text = reader["Occupation"].ToString();
                                    cbhousesurface.SelectedItem = reader["ConcreteSurface"].ToString();
                                    cbStatus.SelectedItem = reader["Status"].ToString();
                                    cbMember.SelectedItem = reader["cbMember"].ToString();
                                    txtlastcurrentaddress.Text = reader["LastCurrentAddress"].ToString();
                                    cbtransferre.SelectedItem = reader["Transferre"].ToString();
                                    txtfirstquarter.Text = reader["FirstQuarter"].ToString();
                                    txtsecondquarter.Text = reader["SecondQuarter"].ToString();
                                    txtthirdquarter.Text = reader["ThirdQuarter"].ToString();
                                    txtfourthquater.Text = reader["FourthQuarter"].ToString();

                                    // Disable save button
                                    btnSave.Enabled = false;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void txtsearchresrec_TextChanged(object sender, EventArgs e)
        {
            string searchQuery = txtsearchresrec.Text.Trim();
            LoadrResidentRecords(searchQuery);
        }
        private void LoadrResidentRecords(string searchQuery = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ResidentID, ResidentName, HouseHoldNo, PurokName, RelationshipHHHead, Status FROM tblresidentprofiling";
                    if (!string.IsNullOrWhiteSpace(searchQuery))
                    {
                        query += " WHERE HouseHoldNo LIKE @SearchQuery OR ResidentName LIKE @SearchQuery";
                    }
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (!string.IsNullOrWhiteSpace(searchQuery))
                    {
                        cmd.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");
                    }
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridViewdateofvisit.DataSource = dt;
                    dataGridViewdateofvisit.RowTemplate.Height = 35; // Adjust height as needed
                    dataGridViewdateofvisit.ColumnHeadersHeight = 40; // Adjust height as needed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
