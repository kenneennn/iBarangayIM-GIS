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
    public partial class IBIM_SUMMARYOFPOPULATIONBYAGE_HRG_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        public IBIM_SUMMARYOFPOPULATIONBYAGE_HRG_frm()
        {
            InitializeComponent();
            dgw_purok.SelectionChanged += dgw_purok_SelectionChanged;

        }
        private void IBIM_SUMMARYOFPOPULATIONBYAGE_HRG_frm_Load(object sender, EventArgs e)
        {
            LoadPurokData();
            InitializeListViewHRG();
            InitializeListViewHRGGG();
            InitializeListViewHRGGGG();
            InitializeListViewHRGG();
            LoadPurokNamesToComboBox();

        }
        private void LoadPurokData()
        {
            try
            {
                string query = "SELECT PurokName FROM tblpurokinfo";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgw_purok.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        dgw_purok.DataSource = dt;

                        // Design the column header for "Purok Name"
                        dgw_purok.Columns[0].HeaderText = "Purok Name";
                        dgw_purok.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgw_purok.Columns[0].HeaderCell.Style.Font = new Font("Impact", 14F, FontStyle.Regular);
                        dgw_purok.Columns[0].HeaderCell.Style.ForeColor = Color.White;
                        dgw_purok.Columns[0].HeaderCell.Style.BackColor = Color.FromArgb(30, 144, 255); // DodgerBlue

                        // Set row height
                        dgw_purok.RowTemplate.Height = 41; // Adjust height as needed
                        dgw_purok.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        dgw_purok.DefaultCellStyle.Font = new Font("Century Ghotic", 14F);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadGovernmentAssistanceData()
        {
            // Get selected values from the ComboBoxes
            string selectedPurok = cbxpurok.SelectedItem?.ToString();
            string selectedAssistance = cbxgovernmentassit.SelectedItem?.ToString();
            try
            {
                // SQL Query to fetch data
                string query = @"
            SELECT 
                ResidentName,
                HouseHoldNo,
                PurokName,
                cbMember AS Assistance,
                Occupation
            FROM 
                tblresidentprofiling
            WHERE 
                PurokName = @PurokName AND 
                cbMember = @Assistance";

                // Create a connection and command
                using (MySqlConnection conn = new MySqlConnection("server=localhost;database=dbibim;uid=root;pwd=;"))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@PurokName", selectedPurok);
                        cmd.Parameters.AddWithValue("@Assistance", selectedAssistance);

                        // Execute the query
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            // Bind the data to the DataGridView
                            DGWListofgovernmentassistance.DataSource = dataTable;
                            DGWListofgovernmentassistance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            // Customize the appearance
                            CustomizeDataGridView();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomizeDataGridView()
        {
           

            // Customize header style
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle
            {
                BackColor = Color.Navy,
                ForeColor = Color.White,
                Font = new Font("Impact", 14, FontStyle.Regular),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            DGWListofgovernmentassistance.ColumnHeadersDefaultCellStyle = headerStyle;

            // Customize row style
            DataGridViewCellStyle rowStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Century Ghotic", 14),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };
            DGWListofgovernmentassistance.DefaultCellStyle = rowStyle;

            // Adjust row height
            DGWListofgovernmentassistance.RowTemplate.Height = 30;

            // Set alternating row colors for better readability
            DGWListofgovernmentassistance.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            // Set column headers height
            DGWListofgovernmentassistance.ColumnHeadersHeight = 40;

            // Disable row headers if not needed
            DGWListofgovernmentassistance.RowHeadersVisible = false;

            // Optional: Make grid lines more visible
            DGWListofgovernmentassistance.GridColor = Color.Black;
            DGWListofgovernmentassistance.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // Set selection mode to full row
            DGWListofgovernmentassistance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Make the DataGridView read-only
            DGWListofgovernmentassistance.ReadOnly = true;
        }

        private void dgw_purok_SelectionChanged(object sender, EventArgs e)
        {
            if (dgw_purok.SelectedRows.Count > 0)
            {
                // Get the selected Purok Name from the DataGridView
                string selectedPurokName = dgw_purok.SelectedRows[0].Cells[0].Value.ToString();

                // Display the selected PurokName in the label
                lblPurokName.Text = selectedPurokName;

                // Count the active residents and display the result
                CountActiveResidents(selectedPurokName);

                // Load the respective data into list views
                LoadHRGData(selectedPurokName);
                LoadHGGRData(selectedPurokName);
                LoadHRGGGData(selectedPurokName);
                LoadHRGGGGData(selectedPurokName);
            }
        }

        private void CountActiveResidents(string purokName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Assuming "Status" column contains the active status of the resident
                    string query = "SELECT COUNT(*) FROM tblresidentprofiling WHERE PurokName = @PurokName AND Status = 'Active'";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PurokName", purokName);

                    int activeResidents = Convert.ToInt32(cmd.ExecuteScalar());

                    // Display the active residents count in the label
                    lblTotalPopulationPerPurok.Text = activeResidents.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error counting active residents: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadHRGGGGData(string purokName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT Sex, COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND Sex = 'MALE'
                GROUP BY Sex;

                SELECT Sex, COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND Sex = 'FEMALE'
                GROUP BY Sex;

                SELECT RelationshipHHHead, COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND RelationshipHHHead = 'Head'
                GROUP BY RelationshipHHHead;

                SELECT COUNT(DISTINCT HouseHoldNo) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName;

                SELECT SanitaryToilet, COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND SanitaryToilet = 'With'
                GROUP BY SanitaryToilet;

                SELECT SanitaryToilet, COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND SanitaryToilet = 'Without'
                GROUP BY SanitaryToilet;

                SELECT UsingLevel, COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND UsingLevel = 'I'
                GROUP BY UsingLevel;

                SELECT UsingLevel, COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND UsingLevel = 'II'
                GROUP BY UsingLevel;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PurokName", purokName);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    int totalMale = 0, totalFemale = 0, totalFamilyHead = 0, totalHouseHold = 0,
                        totalWithSanitaryToilet = 0, totalWithoutSanitaryToilet = 0,
                        totalUsingLevelI = 0, totalUsingLevelII = 0;
                    if (reader.Read())
                    {
                        totalMale = Convert.ToInt32(reader["Total"]);
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalFemale = Convert.ToInt32(reader["Total"]);
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalFamilyHead = Convert.ToInt32(reader["Total"]);
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalHouseHold = Convert.ToInt32(reader["Total"]);
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalWithSanitaryToilet = Convert.ToInt32(reader["Total"]);
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalWithoutSanitaryToilet = Convert.ToInt32(reader["Total"]);
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalUsingLevelI = Convert.ToInt32(reader["Total"]);
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalUsingLevelII = Convert.ToInt32(reader["Total"]);
                    }
                    listViewHRGGGG.Items.Clear();
                    ListViewItem item = new ListViewItem(totalMale.ToString());
                    item.SubItems.Add(totalFemale.ToString());
                    item.SubItems.Add(totalFamilyHead.ToString());
                    item.SubItems.Add(totalHouseHold.ToString());
                    item.SubItems.Add(totalWithSanitaryToilet.ToString());
                    item.SubItems.Add(totalWithoutSanitaryToilet.ToString());
                    item.SubItems.Add(totalUsingLevelI.ToString());
                    item.SubItems.Add(totalUsingLevelII.ToString());
                    listViewHRGGGG.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading HRGGGG data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadHRGGGData(string purokName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    Status,
                    COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND Status = 'Death'
                GROUP BY Status;

                SELECT 
                    HHHEADPhilhealthMember,
                    COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND HHHEADPhilhealthMember = 'YES'
                GROUP BY HHHEADPhilhealthMember;

                SELECT 
                    Transferre,
                    COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND Transferre = 'YES'
                GROUP BY Transferre;

                SELECT 
                    Occupation,
                    COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND Occupation = 'Farmer'
                GROUP BY Occupation;

                SELECT 
                    Occupation,
                    COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND Occupation = 'OFW'
                GROUP BY Occupation;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PurokName", purokName);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    int totalDeath = 0, totalPhilHealthMember = 0, totalTransferredIn = 0,
                        totalFarmer = 0, totalOFW = 0;
                    if (reader.Read())
                    {
                        totalDeath = Convert.ToInt32(reader["Total"]);
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalPhilHealthMember = Convert.ToInt32(reader["Total"]);
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalTransferredIn = Convert.ToInt32(reader["Total"]);
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalFarmer = Convert.ToInt32(reader["Total"]);
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalOFW = Convert.ToInt32(reader["Total"]);
                    }
                    listViewHRGGG.Items.Clear();
                    ListViewItem item = new ListViewItem(totalDeath.ToString());
                    item.SubItems.Add(totalPhilHealthMember.ToString());
                    item.SubItems.Add(totalTransferredIn.ToString());
                    item.SubItems.Add(totalFarmer.ToString());
                    item.SubItems.Add(totalOFW.ToString());
                    listViewHRGGG.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading HRGGG data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadHGGRData(string purokName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    ClassificationbyAgeandHRG,
                    COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName
                GROUP BY ClassificationbyAgeandHRG;

                SELECT 
                    SCStatus,
                    COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName AND SCStatus = 'MEMBER'
                GROUP BY SCStatus;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PurokName", purokName);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    int totalPregnant = 0, totalPWD = 0, totalAdolescentPregnant = 0,
                        totalPostPartum = 0, totalWRA = 0, totalSeniorCitizen = 0;
                    while (reader.Read())
                    {
                        string classification = reader["ClassificationbyAgeandHRG"].ToString();
                        int count = Convert.ToInt32(reader["Total"]);

                        switch (classification)
                        {
                            case "Pregnant":
                                totalPregnant = count;
                                break;
                            case "Persons with Disability":
                                totalPWD = count;
                                break;
                            case "Adolescent Pregnant":
                                totalAdolescentPregnant = count;
                                break;
                            case "Post Partum":
                                totalPostPartum = count;
                                break;
                            case "WRA":
                                totalWRA = count;
                                break;
                        }
                    }
                    if (reader.NextResult() && reader.Read())
                    {
                        totalSeniorCitizen = Convert.ToInt32(reader["Total"]);
                    }
                    listViewHGGR.Items.Clear();
                    ListViewItem item = new ListViewItem(totalPregnant.ToString());
                    item.SubItems.Add(totalPWD.ToString());
                    item.SubItems.Add(totalAdolescentPregnant.ToString());
                    item.SubItems.Add(totalPostPartum.ToString());
                    item.SubItems.Add(totalWRA.ToString());
                    item.SubItems.Add(totalSeniorCitizen.ToString());
                    listViewHGGR.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading HGGR data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadHRGData(string purokName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    ClassificationbyAgeandHRG,
                    COUNT(*) AS Total
                FROM tblresidentprofiling
                WHERE PurokName = @PurokName
                GROUP BY ClassificationbyAgeandHRG";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PurokName", purokName);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    listViewHRG.Items.Clear();
                    int totalAdult = 0, totalNewborn = 0, totalInfant = 0, totalUnderFive = 0, totalSchoolAged = 0, totalAdolescents = 0;
                    while (reader.Read())
                    {
                        string classification = reader["ClassificationbyAgeandHRG"].ToString();
                        int count = Convert.ToInt32(reader["Total"]);

                        // Assign counts based on classification
                        switch (classification)
                        {
                            case "Adult":
                                totalAdult = count;
                                break;
                            case "Newborn":
                                totalNewborn = count;
                                break;
                            case "Infant":
                                totalInfant = count;
                                break;
                            case "Under-five":
                                totalUnderFive = count;
                                break;
                            case "School-Aged Children":
                                totalSchoolAged = count;
                                break;
                            case "Adolescents":
                                totalAdolescents = count;
                                break;
                        }
                    }

                    ListViewItem item = new ListViewItem(totalAdult.ToString());
                    item.SubItems.Add(totalNewborn.ToString());
                    item.SubItems.Add(totalInfant.ToString());
                    item.SubItems.Add(totalUnderFive.ToString());
                    item.SubItems.Add(totalSchoolAged.ToString());
                    item.SubItems.Add(totalAdolescents.ToString());
                    listViewHRG.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading HRG data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InitializeListViewHRG()
        {
            listViewHRG.Columns.Clear();
            listViewHRG.Columns.Add("Total Adult", 200, HorizontalAlignment.Left);
            listViewHRG.Columns.Add("Total Newborn", 200, HorizontalAlignment.Left);
            listViewHRG.Columns.Add("Total Infant", 200, HorizontalAlignment.Left);
            listViewHRG.Columns.Add("Total Under-five", 200, HorizontalAlignment.Left);
            listViewHRG.Columns.Add("Total School-Aged Children", 300, HorizontalAlignment.Left);
            listViewHRG.Columns.Add("Total Adolescents", 200, HorizontalAlignment.Left);
            listViewHRG.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewHRG.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewHRG.View = View.Details;
            listViewHRG.GridLines = true;
            listViewHRG.FullRowSelect = true;
        }
        private void InitializeListViewHRGG()
        {
            listViewHGGR.Columns.Clear();
            listViewHGGR.Columns.Add("Total Pregnant", 200, HorizontalAlignment.Left);
            listViewHGGR.Columns.Add("Total Persons with Disability", 280, HorizontalAlignment.Left);
            listViewHGGR.Columns.Add("Total Adolescent Pregnant", 300, HorizontalAlignment.Left);
            listViewHGGR.Columns.Add("Total Post Partum", 200, HorizontalAlignment.Left);
            listViewHGGR.Columns.Add("Total Women Reproductive Age", 380, HorizontalAlignment.Left);
            listViewHGGR.Columns.Add("Total Senior Citizen", 200, HorizontalAlignment.Left);
            listViewHGGR.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewHGGR.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewHGGR.View = View.Details;
            listViewHGGR.GridLines = true;
            listViewHGGR.FullRowSelect = true;
        }
        private void InitializeListViewHRGGG()
        {
            listViewHGGR.Columns.Clear();
            listViewHRGGG.Columns.Add("Total Death", 200, HorizontalAlignment.Left);
            listViewHRGGG.Columns.Add("Total PhilHealth Member", 280, HorizontalAlignment.Left);
            listViewHRGGG.Columns.Add("Total Transferred-IN", 250, HorizontalAlignment.Left);
            listViewHRGGG.Columns.Add("Total Farmer", 200, HorizontalAlignment.Left);
            listViewHRGGG.Columns.Add("Total OFW", 200, HorizontalAlignment.Left);
            listViewHRGGG.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewHRGGG.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewHRGGG.View = View.Details;
            listViewHRGGG.GridLines = true;
            listViewHRGGG.FullRowSelect = true;
        }
        private void InitializeListViewHRGGGG()
        {
            listViewHGGR.Columns.Clear();
            listViewHRGGGG.Columns.Add("Total Male", 200, HorizontalAlignment.Left);
            listViewHRGGGG.Columns.Add("Total Female", 280, HorizontalAlignment.Left);
            listViewHRGGGG.Columns.Add("Total Family Head", 250, HorizontalAlignment.Left);
            listViewHRGGGG.Columns.Add("Total HouseHold", 250, HorizontalAlignment.Left);
            listViewHRGGGG.Columns.Add("Total HH With Sanitary Toilet", 380, HorizontalAlignment.Left);
            listViewHRGGGG.Columns.Add("Total HH Without Sanitary Toilet", 380, HorizontalAlignment.Left);
            listViewHRGGGG.Columns.Add("Total HH Using Level I", 300, HorizontalAlignment.Left);
            listViewHRGGGG.Columns.Add("Total HH Using Level II", 380, HorizontalAlignment.Left);
            listViewHRGGGG.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewHRGGGG.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listViewHRGGGG.View = View.Details;
            listViewHRGGGG.GridLines = true;
            listViewHRGGGG.FullRowSelect = true;
        }
        private void LoadPurokNamesToComboBox()
        {
            try
            {
                string query = "SELECT PurokName FROM tblpurokinfo";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            cbxpurok.Items.Clear(); // Clear existing items
                            while (reader.Read())
                            {
                                cbxpurok.Items.Add(reader["PurokName"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading Purok names: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CountTotalMembersPerPurok()
        {
            // Get selected values from the ComboBoxes
            string selectedPurok = cbxpurok.SelectedItem?.ToString();
            string selectedAssistance = cbxgovernmentassit.SelectedItem?.ToString();

            try
            {
                // SQL Query to count rows
                string query = @"
            SELECT 
                COUNT(*) 
            FROM 
                tblresidentprofiling
            WHERE 
                PurokName = @PurokName AND 
                cbMember = @Assistance";

                // Create a connection and command
                using (MySqlConnection conn = new MySqlConnection("server=localhost;database=dbibim;uid=root;pwd=;"))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@PurokName", selectedPurok);
                        cmd.Parameters.AddWithValue("@Assistance", selectedAssistance);

                        // Execute the query and get the count
                        int totalMembers = Convert.ToInt32(cmd.ExecuteScalar());

                        // Update the label text
                        lbltotalmemberperpuork.Text = $" {totalMembers}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while counting members: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbxpurok_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGovernmentAssistanceData();
            CountTotalMembersPerPurok();
        }

        private void cbxgovernmentassit_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGovernmentAssistanceData();
            CountTotalMembersPerPurok();
        }
    }
}
