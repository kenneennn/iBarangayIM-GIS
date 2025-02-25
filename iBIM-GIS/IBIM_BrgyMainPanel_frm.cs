using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using LiveCharts.WinForms;  // Use the WinForms version of LiveCharts
using LiveCharts;
using LiveCharts.Wpf; // Required for column charts
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyAxisPosition = OxyPlot.Axes.AxisPosition;




namespace iBIM_GIS
{
    public partial class IBIM_BrgyMainPanel_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        private Timer timer;
        private DateTime lastUpdate;
        public string LoggedInAccountType { get; set; }

        public IBIM_BrgyMainPanel_frm()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
            customizeDesing();
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(IBIM_BrgyMainPanel_frm_KeyDown);
            gmapdashboard.MouseMove += gmapdashboard_MouseMove;
        }
        private void IBIM_BrgyMainPanel_frm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                RefreshAllData();
                LoadTotalHousehold();
                LoadTotalPopulation();
                LoadGenderCount();
                LoadSeniorCitizens();
                LoadPWDCount();
                LoadUserAccountCount();
                LoadTotalPhilhealthMember();
                LoadBlotterChart();
                LoadSumonChart();
                LoadInflowsChart();
            }
            else if (e.KeyCode == Keys.F1)
            {
                if (activeForm != null)
                {
                    activeForm.Close();
                    activeForm = null;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                var result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    this.Close();
                    IBIM_Login_frm loginForm = new IBIM_Login_frm();
                    loginForm.Show();
                }
            }
        }
        private void RefreshAllData()
        {
            try
            {
                LoadAccountName();
                LoadBarangayName();
                LoadImages();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing data: " + ex.Message);
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            if (now.Minute != lastUpdate.Minute)
            {
                label9.Text = now.ToString("hh:mm tt").ToUpper();

                label8.Text = now.ToString("dddd, yyyy-MM-dd");

                lastUpdate = now;
            }
        }
        private void Dashboard_Load(object sender, EventArgs e)
        {
            if (LoggedInAccountType != null)
            {
                btnsystemuser.Enabled = LoggedInAccountType.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                btnsystemuser.Enabled = false;
            }
            LoadAccountName();
            LoadBarangayName();
            LoadImages();
            DisplayBarangayInfo();
            LoadTotalHousehold();
            LoadTotalPopulation();
            LoadGenderCount();
            LoadSeniorCitizens();
            LoadPWDCount();
            LoadUserAccountCount();
            LoadTotalPhilhealthMember();
            LoadBlotterChart();
            LoadSumonChart();
            LoadInflowsChart();
            LoadKasambahayChart();
            LoadJobSeekerChart();
        }
        private void LoadInflowsChart()
        {
            string query = "SELECT ParticularsCategory, SUM(GrandTotal) AS TotalAmount FROM tblcashinflows GROUP BY ParticularsCategory";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Clear previous data in the chart
                    InflowsChart.Series.Clear();
                    InflowsChart.ChartAreas.Clear();
                    InflowsChart.Titles.Clear();

                    // Set up the chart area
                    ChartArea chartArea = new ChartArea("InflowsArea");
                    chartArea.BackColor = Color.WhiteSmoke;  // Light background for clarity
                    chartArea.Area3DStyle.Enable3D = true;   // Enable 3D effect
                    chartArea.Area3DStyle.Rotation = 45;     // Set rotation for better 3D effect
                    chartArea.Area3DStyle.Inclination = 30;  // Incline for depth
                    chartArea.AxisX.LineColor = Color.Transparent; // Hide X-axis line
                    chartArea.AxisY.LineColor = Color.Transparent; // Hide Y-axis line
                    InflowsChart.ChartAreas.Add(chartArea);

                    // Customize chart appearance
                    InflowsChart.BackColor = Color.Transparent;
                    InflowsChart.BorderlineDashStyle = ChartDashStyle.Solid;
                    InflowsChart.BorderlineWidth = 2;
                    InflowsChart.BorderlineColor = ColorTranslator.FromHtml("#005B96"); // Blue border color

                    // Add a chart title
                    Title title = new Title("CASH INFLOWS BY PARTICULARS", Docking.Top, new Font("Impact", 14, FontStyle.Bold), ColorTranslator.FromHtml("#005B96"));
                    title.ForeColor = ColorTranslator.FromHtml("#004080");
                    InflowsChart.Titles.Add(title);

                    // Create a series for the chart (Bar chart in this case)
                    var series = InflowsChart.Series.Add("Inflows");
                    series.ChartType = SeriesChartType.Bar;  // Bar chart type
                    series.Color = ColorTranslator.FromHtml("#00796B"); // Color for bars
                    series.Font = new Font("Segoe UI", 10, FontStyle.Bold); // Data point label font
                    series.LabelForeColor = Color.White; // White text for labels
                    series.Label = "#VAL"; // Display values
                    series.ToolTip = "#VALX: #VAL"; // Tooltip with value and category
                    series.BorderWidth = 1;
                    series.BorderColor = Color.White;

                    // Add data points to the chart
                    foreach (DataRow row in dt.Rows)
                    {
                        string category = row["ParticularsCategory"].ToString();
                        decimal totalAmount = Convert.ToDecimal(row["TotalAmount"]);

                        // Add data point with x-value as the category and y-value as the total amount
                        series.Points.AddXY(category, totalAmount);
                    }

                    // Customize the legend (if needed)
                    Legend legend = new Legend
                    {
                        Docking = Docking.Bottom,
                        Font = new Font("Segoe UI", 12, FontStyle.Regular),
                        BackColor = Color.Transparent,
                        ForeColor = ColorTranslator.FromHtml("#005B96"),
                        BorderColor = Color.Transparent
                    };
                    InflowsChart.Legends.Clear();
                    InflowsChart.Legends.Add(legend);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadSumonChart()
        {
            string query = "SELECT Remarks, COUNT(*) AS Count FROM tblsumon WHERE Remarks IN ('Settled', 'Unsettled') GROUP BY Remarks";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Adjust chart size
                    SumonChart.Width = 336;  // Set width
                    SumonChart.Height = 246; // Set height

                    // Clear previous data in the chart
                    SumonChart.Series.Clear();
                    SumonChart.ChartAreas.Clear();
                    SumonChart.Titles.Clear();

                    // Set up the chart area
                    ChartArea chartArea = new ChartArea("SumonArea");
                    chartArea.BackColor = Color.WhiteSmoke;  // Subtle background for better clarity
                    chartArea.Area3DStyle.Enable3D = true;   // Enable 3D effect
                    chartArea.Area3DStyle.Rotation = 45;    // Set rotation for 3D
                    chartArea.Area3DStyle.Inclination = 25; // Incline for better visual depth
                    chartArea.AxisX.LineColor = Color.Transparent; // Hide X-axis line
                    chartArea.AxisY.LineColor = Color.Transparent; // Hide Y-axis line
                    SumonChart.ChartAreas.Add(chartArea);

                    // Customize chart appearance
                    SumonChart.BackColor = Color.White;
                    SumonChart.BorderlineDashStyle = ChartDashStyle.Solid;
                    SumonChart.BorderlineWidth = 2;
                    SumonChart.BorderlineColor = ColorTranslator.FromHtml("#005B96"); // Blue border color

                    // Add a chart title
                    Title title = new Title("SUMON STATUS SUMMARY", Docking.Top, new Font("Impact", 14, FontStyle.Bold), ColorTranslator.FromHtml("#005B96"));
                    title.ForeColor = ColorTranslator.FromHtml("#004080");
                    SumonChart.Titles.Add(title);

                    // Create a series for the chart
                    var series = SumonChart.Series.Add("Sumon Status");
                    series.ChartType = SeriesChartType.Pie; // Pie chart
                    series.Font = new Font("Segoe UI", 10, FontStyle.Bold); // Label font
                    series.LabelForeColor = Color.White; // Label color
                    series.Label = "#PERCENT{P2}"; // Display percentages
                    series.ToolTip = "#VALX: #PERCENT{P2} (#VAL)"; // Tooltip with percentage and count
                    series.BorderWidth = 1;
                    series.BorderColor = Color.White;

                    // Add data points with custom colors
                    string[] customColors = { "#4CAF50", "#FF5722" }; // Green for Settled, Red for Unsettled
                    int colorIndex = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        string remarks = row["Remarks"].ToString();
                        int count = Convert.ToInt32(row["Count"]);

                        // Add the point and customize its appearance
                        int pointIndex = series.Points.AddXY(remarks, count);
                        series.Points[pointIndex].Color = ColorTranslator.FromHtml(customColors[colorIndex % customColors.Length]);
                        series.Points[pointIndex].Font = new Font("Segoe UI", 9, FontStyle.Bold); // Font for data labels
                        series.Points[pointIndex].LegendText = $"{remarks}"; // Legend text
                        colorIndex++;
                    }

                    // Customize legend
                    Legend legend = new Legend
                    {
                        Docking = Docking.Bottom,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular),
                        BackColor = Color.WhiteSmoke,
                        ForeColor = ColorTranslator.FromHtml("#005B96"),
                        BorderColor = ColorTranslator.FromHtml("#005B96"),
                        BorderWidth = 1,
                        Title = "Sumon Status",
                        TitleFont = new Font("Segoe UI", 10, FontStyle.Bold),
                        TitleForeColor = ColorTranslator.FromHtml("#005B96")
                    };
                    SumonChart.Legends.Clear();
                    SumonChart.Legends.Add(legend);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadBlotterChart()
        {
            string query = "SELECT Remarks, COUNT(*) AS Count FROM tblblotter WHERE Remarks IN ('Settled', 'Unsettled') GROUP BY Remarks";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Adjust chart size
                    BlotterChart.Width = 330;  // Set width
                    BlotterChart.Height = 246; // Set height

                    // Clear previous data in the chart
                    BlotterChart.Series.Clear();
                    BlotterChart.ChartAreas.Clear();
                    BlotterChart.Titles.Clear();

                    // Set up the chart area
                    ChartArea chartArea = new ChartArea("BlotterArea");
                    chartArea.BackColor = Color.WhiteSmoke;  // Subtle background
                    chartArea.Area3DStyle.Enable3D = true;   // Enable 3D effect
                    chartArea.Area3DStyle.Rotation = 45;    // 3D rotation for better visualization
                    chartArea.Area3DStyle.Inclination = 25; // Add inclination
                    chartArea.AxisX.LineColor = Color.Transparent;
                    chartArea.AxisY.LineColor = Color.Transparent;
                    BlotterChart.ChartAreas.Add(chartArea);

                    // Customize chart appearance
                    BlotterChart.BackColor = Color.White;
                    BlotterChart.BorderlineDashStyle = ChartDashStyle.Solid;
                    BlotterChart.BorderlineWidth = 2;
                    BlotterChart.BorderlineColor = ColorTranslator.FromHtml("#004080");

                    // Add a chart title
                    Title title = new Title("BLOTTER STATUS SUMMARY", Docking.Top, new Font("Impact", 14, FontStyle.Bold), ColorTranslator.FromHtml("#004080"));
                    title.ForeColor = ColorTranslator.FromHtml("#004080");
                    BlotterChart.Titles.Add(title);

                    // Create a series for the chart
                    var series = BlotterChart.Series.Add("Blotter Status");
                    series.ChartType = SeriesChartType.Pie; // Set as Pie chart
                    series.Font = new Font("Segoe UI", 10, FontStyle.Bold); // Font for labels
                    series.LabelForeColor = Color.White; // Label color
                    series.Label = "#PERCENT{P2}"; // Display percentages
                    series.ToolTip = "#VALX: #PERCENT{P2} (#VAL)"; // Tooltip with percentage and count
                    series.BorderWidth = 1;
                    series.BorderColor = Color.White;

                    // Add data points with custom colors
                    string[] customColors = { "#4CAF50", "#FF5722" }; // Green and Red for Settled/Unsettled
                    int colorIndex = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        string remarks = row["Remarks"].ToString();
                        int count = Convert.ToInt32(row["Count"]);

                        // Add the point and customize its appearance
                        int pointIndex = series.Points.AddXY(remarks, count);
                        series.Points[pointIndex].Color = ColorTranslator.FromHtml(customColors[colorIndex % customColors.Length]);
                        series.Points[pointIndex].Font = new Font("Segoe UI", 9, FontStyle.Bold); // Font for data labels
                        series.Points[pointIndex].LegendText = $"{remarks}"; // Legend text
                        colorIndex++;
                    }

                    // Customize legend
                    Legend legend = new Legend
                    {
                        Docking = Docking.Bottom,
                        Font = new Font("Segoe UI", 10, FontStyle.Regular),
                        BackColor = Color.WhiteSmoke,
                        ForeColor = ColorTranslator.FromHtml("#004080"),
                        BorderColor = ColorTranslator.FromHtml("#004080"),
                        BorderWidth = 1,
                        Title = "Blotter Status",
                        TitleFont = new Font("Segoe UI", 10, FontStyle.Bold),
                        TitleForeColor = ColorTranslator.FromHtml("#004080")
                    };
                    BlotterChart.Legends.Clear();
                    BlotterChart.Legends.Add(legend);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadAccountName()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    string query = "SELECT AccountName, AccountType FROM tbluser WHERE AccountType = @AccountType";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AccountType", LoggedInAccountType);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblaccountname.Text = reader["AccountName"].ToString();
                                lblaccounttype.Text = reader["AccountType"].ToString();
                            }
                            else
                            {
                                lblaccountname.Text = "Unknown User";
                                lblaccounttype.Text = "Unknown Type";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading account information: " + ex.Message);
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

                                if (!reader.IsDBNull(reader.GetOrdinal("Image1")))
                                {
                                    byte[] imageBytes1 = (byte[])reader["Image1"];
                                    using (MemoryStream ms1 = new MemoryStream(imageBytes1))
                                    {
                                        pblogo1.Image = Image.FromStream(ms1);
                                    }
                                }
                                if (!reader.IsDBNull(reader.GetOrdinal("Image2")))
                                {
                                    byte[] imageBytes2 = (byte[])reader["Image2"];
                                    using (MemoryStream ms2 = new MemoryStream(imageBytes2))
                                    {
                                        pblogo2.Image = Image.FromStream(ms2);
                                    }
                                }
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
        private void customizeDesing()
        {
            panelsetup.Visible = false;
            paneltrans.Visible = false;
            panelreports.Visible = false;

        }
        private void hideSubMenu()
        {
            if (panelsetup.Visible == true)
                panelsetup.Visible = false;
            if (paneltrans.Visible == true)
                paneltrans.Visible = false;
            if (panelreports.Visible == true)
                panelreports.Visible = false;
        }
        private void showSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                hideSubMenu();
                subMenu.Visible = true;
            }
            else
                subMenu.Visible = false;
        }
        private void btnsetup_Click_1(object sender, EventArgs e)
        {
            showSubMenu(panelsetup);
        }
        private void btnlogout_Click_1(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();

                IBIM_Login_frm loginForm = new IBIM_Login_frm();
                loginForm.Show();
            }
        }
        private Form activeForm = null;
        private void openfrm(Form BRGY_OfficialInformation)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = BRGY_OfficialInformation;
            BRGY_OfficialInformation.TopLevel = false;
            BRGY_OfficialInformation.FormBorderStyle = FormBorderStyle.None;
            BRGY_OfficialInformation.Dock = DockStyle.Fill;
            panelmaincontent.Controls.Add(BRGY_OfficialInformation);
            panelmaincontent.Tag = BRGY_OfficialInformation;
            BRGY_OfficialInformation.BringToFront();
            BRGY_OfficialInformation.Show();
        }

        private void btntransaction_Click(object sender, EventArgs e)
        {
            showSubMenu(paneltrans);
        }
        private void btnofficialinfo_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_OfficialInformation_frm());
        }

        private void btnHHmappingandtagging_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_HH_mappingtagggin_frm());
        }

        private void btnpurokinfo_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_PurokInformation_frm());
        }

        private void btnresidentprofiling_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_HouseHoldProfiling_frm());
        }

        private void btnProjectinfo_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_ProjectInformation_frm());
        }

        private void btnannouncement_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_Announcement_frm());
        }

        private void btnblotter_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_BrgyCases_frm());
        }

        private void btnParticulars_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_Particulars_frm());
        }

        private void btnpayment_Click(object sender, EventArgs e)
        {
            // Ensure that lblaccountname exists in the IBIM_BrgyMainPanel_frm
            string accountName = lblaccountname.Text; // Get the account name from lblaccountname
            openfrm(new IBIM_BrgyCollectionInformation_frm(accountName)); // Pass accountName to the payment form
        }

        private void btnCF_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_CashFlowStatement_frm());
        }

        private void btnDOCREQ_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_BrgyRequestedDocs_frm());
        }
        private void btnclassifbyage_Click_1(object sender, EventArgs e)
        {
            openfrm(new IBIM_SUMMARYOFPOPULATIONBYAGE_HRG_frm());
        }

        private void btnclassifbyHRGH_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_SummaryofCases_frm());
        }

        private void btnSUMCF_Click_1(object sender, EventArgs e)
        {
            openfrm(new IBIM_BrgySummaryOfCashFlow_frm());
        }

        private void btncollections_Click_2(object sender, EventArgs e)
        {
            openfrm(new IBIM_BrgyCollections_frm());
        }
        private void btnsystemuser_Click_2(object sender, EventArgs e)
        {
            openfrm(new IBIM_SystemUser_frm());
        }

        private void btnreports_Click(object sender, EventArgs e)
        {
            showSubMenu(panelreports);
        }

        private void btnhome_Click(object sender, EventArgs e)
        {
            // Close any currently active form in panelmaincontent
            if (activeForm != null)
            {
                activeForm.Close();
                activeForm = null; // Reset activeForm to indicate no form is open
            }

            // Optionally, add code here to reset the panel to its default state if necessary
            // e.g., panelmaincontent.Controls.Clear(); or set default labels and text.
        }

        private void BtnKasambahay_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_Kasambahay_frm());
        }
        private void btnAvailee_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_Availee_frm());
        }
        private void btncleanupdrive_Click(object sender, EventArgs e)
        {
            openfrm(new IBIM_CleanupDrive_frm());
        }
        private void DisplayBarangayInfo()
        {
            string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
            string barangayName = string.Empty;
            string termYearRange = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string queryBarangayName = "SELECT BarangayName FROM tblsetbrgyname LIMIT 1";
                        using (MySqlCommand cmd = new MySqlCommand(queryBarangayName, connection))
                        {
                            object result = cmd.ExecuteScalar();
                            barangayName = result != null ? result.ToString() : "Unknown Barangay";
                        }
                        string queryDateElected = "SELECT DateElected FROM tblofficialinfo LIMIT 1";
                        using (MySqlCommand cmd = new MySqlCommand(queryDateElected, connection))
                        {
                            object result = cmd.ExecuteScalar();
                            if (result != null && DateTime.TryParse(result.ToString(), out DateTime dateElected))
                            {
                                int startYear = dateElected.Year;
                                int endYear = startYear + 15;
                                termYearRange = $"{startYear}-{endYear}";
                            }
                            else
                            {
                                termYearRange = "Unknown Term Range";
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }
        private void LoadTotalPhilhealthMember()
        {
            try
            {
                string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(DISTINCT HHHEADPhilhealthMember) AS TotalPhilhealthMember FROM tblresidentprofiling WHERE HHHEADPhilhealthMember = 'MEMBER'";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            label17.Text = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadTotalHousehold()
        {
            try
            {
                string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;"; // Replace with your actual database connection string
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(DISTINCT HouseHoldNo) AS TotalHouseholds FROM tblresidentprofiling";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            labelTotalHousehold.Text = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadTotalPopulation()
        {
            try
            {
                string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;"; // Replace with your actual database connection string
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) AS TotalPopulation FROM tblresidentprofiling";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            LabelTotalPopulation.Text = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadGenderCount()
        {
            try
            {
                string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;"; // Replace with your actual database connection string
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT " +
                                   "(SELECT COUNT(*) FROM tblresidentprofiling WHERE Sex = 'Male') AS TotalMale, " +
                                   "(SELECT COUNT(*) FROM tblresidentprofiling WHERE Sex = 'Female') AS TotalFemale";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                LabelMale.Text = reader["TotalMale"].ToString();
                                LabelFemale.Text = reader["TotalFemale"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadSeniorCitizens()
        {
            try
            {
                string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;"; // Replace with your actual database connection string
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) AS TotalSeniorCitizens FROM tblresidentprofiling WHERE SCStatus = 'MEMBER'";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            LabelSeniorCitizen.Text = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadPWDCount()
        {
            try
            {
                string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;"; // Replace with your actual database connection string
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) AS TotalPWD FROM tblresidentprofiling WHERE ClassificationbyAgeandHRG LIKE '%PWD%'";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            LabelPWD.Text = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadUserAccountCount()
        {
            try
            {
                string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;"; // Replace with your actual database connection string
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) AS TotalUsers FROM tbluser";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            LabelUserAccount.Text = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gmapdashboard_Load_1(object sender, EventArgs e)
        {
            gmapdashboard.MapProvider = GMap.NET.MapProviders.EmptyProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gmapdashboard.Position = new PointLatLng(17.0240, 121.6350);
            gmapdashboard.MinZoom = 12;
            gmapdashboard.MaxZoom = 18;
            gmapdashboard.Zoom = (int)13;
            gmapdashboard.ShowCenter = true;
            gmapdashboard.BackColor = Color.FromArgb(23, 27, 82); // A darker blue-gray tone
            string geoJsonContent = @"
                {
                    ""type"": ""FeatureCollection"",
                    ""features"": [
                        {
                            ""type"": ""Feature"",
                            ""properties"": {
                               ""PurokName"": ""Purok Libis""
                            },
                            ""geometry"": {
                                ""coordinates"": [
                                    [
                                        [121.63432175802444, 17.02310903297672],
                                        [121.63678919768239, 17.00686651304271],
                                        [121.63898437781933, 17.00774681531965],
                                        [121.64522842651417, 17.008486579629476],
                                        [121.65524838064806, 17.009711612072905],
                                        [121.66412513433397, 17.01482637250487],
                                        [121.6556434342221, 17.02720908173022],
                                        [121.6409752889279, 17.024186446162787],
                                        [121.63432175802444, 17.02310903297672]
                                    ]
                                ],
                                ""type"": ""Polygon""
                            },
                            ""id"": 0
                        },
                        {
                            ""type"": ""Feature"",
                            ""properties"": {
                                ""PurokName"": ""Purok Liwanag""
                            },
                            ""geometry"": {
                                ""coordinates"": [
                                    [
                                        [121.61073710021589, 17.048686631699084],
                                        [121.61861777720685, 17.04048540691757],
                                        [121.62971936533086, 17.036611375725528],
                                        [121.63021662423057, 17.031502581085675],
                                        [121.63773971686794, 17.032491941318966],
                                        [121.64249685392281, 17.039934160976628],
                                        [121.65085865140281, 17.050178986606895],
                                        [121.64100913283767, 17.054680534071778],
                                        [121.63240882150501, 17.058263320839586],
                                        [121.61789879903432, 17.05109767857259],
                                        [121.61073710021589, 17.048686631699084]
                                    ]
                                ],
                                ""type"": ""Polygon""
                            },
                            ""id"": 1
                        },
                        {
                            ""type"": ""Feature"",
                            ""properties"": {
                                ""PurokName"": ""Purok Mabini""
                            },
                            ""geometry"": {
                                ""coordinates"": [
                                    [
                                        [121.60457071504993, 17.034604312262587],
                                        [121.59912019466822, 17.028446710859782],
                                        [121.6302376569808, 17.031405015535086],
                                        [121.6295987419727, 17.0365697835894],
                                        [121.61856876203603, 17.040435391843005],
                                        [121.61078561557878, 17.04848753604547],
                                        [121.60457071504993, 17.034604312262587]
                                    ]
                                ],
                                ""type"": ""Polygon""
                            },
                            ""id"": 2
                        },
                        {
                            ""type"": ""Feature"",
                            ""properties"": {
                                ""PurokName"": ""Purok Pag-Asa""
                            },
                            ""geometry"": {
                                ""coordinates"": [
                                    [
                                        [121.64256012219954, 17.039873866313016],
                                        [121.65496037391853, 17.02835635768703],
                                        [121.66423234053792, 17.014803933180318],
                                        [121.68018009125342, 17.024290427653497],
                                        [121.67786210423145, 17.025797583442483],
                                        [121.67452420291892, 17.03812034289622],
                                        [121.67229893537672, 17.040070633271924],
                                        [121.67331286672947, 17.042831039889876],
                                        [121.67257111088213, 17.05630501830619],
                                        [121.65059659390909, 17.050100017377815],
                                        [121.64256012219954, 17.039873866313016]
                                    ]
                                ],
                                ""type"": ""Polygon""
                            },
                            ""id"": 3
                        },
                        {
                            ""type"": ""Feature"",
                            ""properties"": {
                                ""PurokName"": ""Purok Maligaya""
                            },
                            ""geometry"": {
                                ""coordinates"": [
                                    [
                                        [121.63432050384984, 17.023100372065258],
                                        [121.62867034555347, 17.022148719969778],
                                        [121.60815862894265, 17.018862049269927],
                                        [121.6028160847095, 17.01520769663604],
                                        [121.6080510930567, 17.008347561967597],
                                        [121.61311232517869, 16.990983476192184],
                                        [121.63400232032717, 17.005634758485158],
                                        [121.63678963119366, 17.00680088419098],
                                        [121.63432050384984, 17.023100372065258]
                                    ]
                                ],
                                ""type"": ""Polygon""
                            },
                            ""id"": 4
                        },
                        {
                            ""type"": ""Feature"",
                            ""properties"": {
                                ""PurokName"": ""Sitio Mananao""
                            },
                            ""geometry"": {
                                ""coordinates"": [
                                    [
                                        [121.63433940330037, 17.02311214893109],
                                        [121.63356563367432, 17.02741179515351],
                                        [121.63390570212323, 17.03195192802849],
                                        [121.62993499014817, 17.03135246979153],
                                        [121.59920033987862, 17.028386452050285],
                                        [121.5981902464797, 17.021092774008537],
                                        [121.60282588227233, 17.01528463556575],
                                        [121.60818982652057, 17.01891497054079],
                                        [121.63433940330037, 17.02311214893109]
                                    ]
                                ],
                                ""type"": ""Polygon""
                            },
                            ""id"": 5
                        },
                        {
                            ""type"": ""Feature"",
                            ""properties"": {
                                ""PurokName"": ""Purok Mabuhay""
                            },
                            ""geometry"": {
                                ""coordinates"": [
                                    [
                                        [121.63432932442475, 17.023110230797982],
                                        [121.64095262214363, 17.02425171187693],
                                        [121.65566313868266, 17.02725493869015],
                                        [121.65493956607565, 17.028355149060943],
                                        [121.64248177472348, 17.039919729117557],
                                        [121.6378164775387, 17.032539436097167],
                                        [121.63388659177753, 17.0319760307448],
                                        [121.63358839345273, 17.027429899087025],
                                        [121.63432932442475, 17.023110230797982]
                                    ]
                                ],
                                ""type"": ""Polygon""
                            },
                            ""id"": 6
                        }
                    ]
                }";
            JObject geoJson = JObject.Parse(geoJsonContent);

            var purokNames = new HashSet<string>();
            string connectionString = "Server=localhost;Database=dbibim;Uid=root;Pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT PurokName FROM tblpurokinfo";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            purokNames.Add(reader["PurokName"].ToString());
                        }
                    }
                }
            }
            var overlay = CreatePurokOverlay(geoJson, purokNames);
            gmapdashboard.Overlays.Add(overlay);
            AddMapInteractivity(overlay);
        }
        public class LabelMarker : GMapMarker
        {
            private string labelText;
            public LabelMarker(string text, PointLatLng position) : base(position)
            {
                labelText = text;
            }
            public override void OnRender(Graphics g)
            {
                var point = new PointF(LocalPosition.X, LocalPosition.Y);
                var font = new Font("Century Ghotic", 8, FontStyle.Bold);
                var brush = new SolidBrush(Color.White);  
                var backgroundBrush = new SolidBrush(Color.FromArgb(220, 0, 0, 104)); 
                var textSize = g.MeasureString(labelText, font);
                var padding = 8;
                var backgroundRect = new RectangleF(point.X - textSize.Width / 2 - padding, point.Y - textSize.Height / 2 - padding,
                                                    textSize.Width + 2 * padding, textSize.Height + 2 * padding);
                var roundedRectPath = new System.Drawing.Drawing2D.GraphicsPath();
                roundedRectPath.AddArc(backgroundRect.X, backgroundRect.Y, 20, 20, 180, 90);  // Top-left corner
                roundedRectPath.AddArc(backgroundRect.X + backgroundRect.Width - 20, backgroundRect.Y, 20, 20, 270, 90);  // Top-right corner
                roundedRectPath.AddArc(backgroundRect.X + backgroundRect.Width - 20, backgroundRect.Y + backgroundRect.Height - 20, 20, 20, 0, 90);  // Bottom-right corner
                roundedRectPath.AddArc(backgroundRect.X, backgroundRect.Y + backgroundRect.Height - 20, 20, 20, 90, 90);  // Bottom-left corner
                roundedRectPath.CloseFigure();
                g.FillPath(backgroundBrush, roundedRectPath);
                var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)); // Subtle shadow effect
                g.FillPath(shadowBrush, roundedRectPath);
                g.DrawString(labelText, font, brush, point.X - textSize.Width / 2, point.Y - textSize.Height / 2);
                var borderPen = new Pen(Color.FromArgb(255, 28, 28, 132), 2);
                g.DrawPath(borderPen, roundedRectPath);
            }
        }
        private GMapOverlay CreatePurokOverlay(JObject geoJson, HashSet<string> purokNames)
        {
            var overlay = new GMapOverlay("geojsonOverlay");

            foreach (var feature in geoJson["features"])
            {
                var geometry = feature["geometry"];
                var properties = feature["properties"];
                string purokName = properties["PurokName"].ToString();

                if (!purokNames.Contains(purokName))
                {
                    continue;
                }

                if (geometry["type"].ToString() == "Polygon")
                {
                    var polygons = geometry["coordinates"];
                    foreach (var polygon in polygons)
                    {
                        var points = new List<PointLatLng>();
                        double totalLat = 0, totalLng = 0;
                        foreach (var point in polygon)
                        {
                            double lng = point[0].Value<double>();
                            double lat = point[1].Value<double>();
                            points.Add(new PointLatLng(lat, lng));
                            totalLat += lat;
                            totalLng += lng;
                        }
                        double centroidLat = totalLat / points.Count;
                        double centroidLng = totalLng / points.Count;
                        PointLatLng centroid = new PointLatLng(centroidLat, centroidLng);
                        var polygonShape = new GMapPolygon(points, purokName)
                        {
                            Stroke = new Pen(Color.FromArgb(255, 255, 255), 1)  // Light border with 1px width
                            {
                                DashStyle = System.Drawing.Drawing2D.DashStyle.Solid  // Solid border
                            },
                            Fill = new SolidBrush(Color.FromArgb(64, 255, 255, 255))  // Semi-transparent white (similar to rgba(255, 255, 255, 0.25))
                            {
                            },
                            Tag = purokName
                        };
                        var shadowPolygon = new GMapPolygon(points, purokName)
                        {
                            Stroke = new Pen(Color.FromArgb(31, 38, 135), 2),  // Darker shadow color
                            Fill = new SolidBrush(Color.FromArgb(31, 38, 135)),  // Shadow fill with a slight transparency
                            Tag = purokName 
                        };
                        overlay.Polygons.Add(shadowPolygon);
                        overlay.Polygons.Add(polygonShape);
                        var label = new LabelMarker(purokName, centroid);
                        overlay.Markers.Add(label);
                    }
                }
            }

            return overlay;
        }
        private void AddMapInteractivity(GMapOverlay overlay)
        {
            gmapdashboard.OnPolygonEnter += (item) =>
            {
                if (item is GMapPolygon polygon)
                {
                    string purokName = polygon.Tag?.ToString(); // Get the PurokName from the polygon tag
                    if (!string.IsNullOrEmpty(purokName))
                    {
                        toolTip1.BackColor = Color.White;
                        toolTip1.ForeColor = Color.Black;
                        toolTip1.IsBalloon = true; // Modern balloon-style tooltip
                        toolTip1.Show($"Purok: {purokName}", gmapdashboard, MousePosition.X - this.Location.X + 10, MousePosition.Y - this.Location.Y + 10);
                        polygon.Stroke = new Pen(Color.Red, 3); // Red border with width 3 for emphasis
                        polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red)); // Semi-transparent red fill
                        DisplayResidents(purokName);
                    }
                }
            };
            gmapdashboard.OnPolygonLeave += (item) =>
            {
                if (item is GMapPolygon polygon)
                {
                    string purokName = polygon.Tag?.ToString();
                    if (!string.IsNullOrEmpty(purokName))
                    {
                        polygon.Stroke = new Pen(Color.Navy, 2); // Reset to original stroke color and width
                        polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Navy)); // Reset to original fill color
                        toolTip1.Hide(gmapdashboard);
                    }
                }
            };
        gmapdashboard.OnPolygonLeave += (item) =>
            {
                if (item is GMapPolygon polygon)
                {
                    toolTip1.Hide(gmapdashboard);
                    polygon.Stroke = new Pen(Color.Navy, 2);
                    polygon.Fill = Brushes.Transparent;
                }
            };
            gmapdashboard.MouseMove += (s, e) =>
            {
                var map = s as GMap.NET.WindowsForms.GMapControl;
                var latLng = map.FromLocalToLatLng(e.X, e.Y);
                foreach (var polygon in overlay.Polygons)
                {if (polygon.IsInside(latLng)) 
                    {string purokName = polygon.Tag?.ToString();
                        if (!string.IsNullOrEmpty(purokName))
                        {
                            DisplayResidents(purokName);
                            break; 
                        }
                    }
                }
            };
        }
        private void DisplayResidents(string purokName)
        {
            string connectionString = "Server=localhost;Database=dbibim;Uid=root;Pwd=;";
            int residentCount = 0;
            int maleCount = 0;
            int femaleCount = 0;
            Dictionary<string, int> classificationCounts = new Dictionary<string, int>()
            {
                {"Newborn", 0},
                {"Infant", 0},
                {"Under-five", 0},
                {"School-Aged Children", 0},
                {"Adolescents", 0},
                {"Pregnant", 0},
                {"Persons with Disability", 0},
                {"Adolescent Pregnant", 0},
                {"Post Partum", 0},
                {"WRA", 0},
                {"Senior Citizen", 0},
                {"Adult", 0}
            };
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT 
                COUNT(*) AS TotalResidents, 
                SUM(CASE WHEN Sex = 'Male' THEN 1 ELSE 0 END) AS MaleCount,
                SUM(CASE WHEN Sex = 'Female' THEN 1 ELSE 0 END) AS FemaleCount,
                ClassificationbyAgeandHRG
            FROM tblresidentprofiling 
            WHERE PurokName = @PurokName
            GROUP BY ClassificationbyAgeandHRG";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PurokName", purokName);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.HasRows)
                            {
                                residentCount = reader.IsDBNull(reader.GetOrdinal("TotalResidents")) ? residentCount : reader.GetInt32("TotalResidents");
                                maleCount = reader.IsDBNull(reader.GetOrdinal("MaleCount")) ? maleCount : reader.GetInt32("MaleCount");
                                femaleCount = reader.IsDBNull(reader.GetOrdinal("FemaleCount")) ? femaleCount : reader.GetInt32("FemaleCount");

                                string classification = reader.IsDBNull(reader.GetOrdinal("ClassificationbyAgeandHRG")) ? "" : reader.GetString("ClassificationbyAgeandHRG");
                                if (classificationCounts.ContainsKey(classification))
                                {
                                    classificationCounts[classification]++;
                                }
                            }
                        }
                    }
                }
            }
            StringBuilder tooltipText = new StringBuilder();
            tooltipText.AppendLine($"{purokName}");
            tooltipText.AppendLine($"Total Residents: {residentCount}");
            tooltipText.AppendLine($"Total Males: {maleCount}");
            tooltipText.AppendLine($"Total Females: {femaleCount}");
            foreach (var classification in classificationCounts)
            {
                tooltipText.AppendLine($"{classification.Key}: {classification.Value}");
            }
            toolTip1.ToolTipTitle = "Purok Record Overview"; 
            toolTip1.Show(tooltipText.ToString(),
               gmapdashboard,
               gmapdashboard.PointToClient(MousePosition).X + 10,
               gmapdashboard.PointToClient(MousePosition).Y + 10);
        }
        private void gmapdashboard_MouseMove(object sender, MouseEventArgs e)
        {
            bool isOverPolygon = false;

            foreach (var overlay in gmapdashboard.Overlays)
            {
                foreach (var polygon in overlay.Polygons)
                {
                    if (polygon.IsInside(gmapdashboard.FromLocalToLatLng(e.X, e.Y)))
                    {
                        isOverPolygon = true;
                        break;
                    }
                }if (isOverPolygon)break;
            }
            if (!isOverPolygon)
            {
                toolTip1.Hide(gmapdashboard); 
            }
        }
        private void LoadKasambahayChart()
        {
            // SQL query to count KasambahayName
            string query = "SELECT COUNT(KasambahayName) AS TotalCount FROM tblkasambahay";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    int totalCount = Convert.ToInt32(cmd.ExecuteScalar()); // Get the total count

                    // Clear previous data in the chart
                    KasambahayChart.Series.Clear();
                    KasambahayChart.ChartAreas.Clear();
                    KasambahayChart.Titles.Clear();

                    // Set up the chart area
                    ChartArea chartArea = new ChartArea("KasambahayArea");
                    chartArea.BackColor = Color.White; // Background color of the chart area
                    chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
                    chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
                    chartArea.AxisX.Title = "Category";
                    chartArea.AxisY.Title = "Count";
                    chartArea.AxisX.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);
                    chartArea.AxisY.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);
                    chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                    chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                    chartArea.AxisX.LineColor = ColorTranslator.FromHtml("#001A6E");
                    chartArea.AxisY.LineColor = ColorTranslator.FromHtml("#001A6E");
                    KasambahayChart.ChartAreas.Add(chartArea);

                    // Customize chart appearance
                    KasambahayChart.BackColor = Color.WhiteSmoke; // Chart background
                    KasambahayChart.BorderlineDashStyle = ChartDashStyle.Solid;
                    KasambahayChart.BorderlineWidth = 2;
                    KasambahayChart.BorderlineColor = ColorTranslator.FromHtml("#003366");

                    // Add a chart title
                    Title title = new Title("TOTAL KASAMBAHAY", Docking.Top, new Font("Impact", 16, FontStyle.Bold), ColorTranslator.FromHtml("#003366"));
                    title.ForeColor = ColorTranslator.FromHtml("#003366");
                    KasambahayChart.Titles.Add(title);

                    // Create a series for the chart
                    var series = KasambahayChart.Series.Add("Kasambahay Count");
                    series.ChartType = SeriesChartType.Pie; // Make it a pie chart for better visualization
                    series.Points.AddXY("TOTAL KASAMBAHAY", totalCount); // Add the total count
                    series.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                    series.LabelForeColor = Color.White; // Label color
                    series.Label = "#PERCENT{P1}"; // Display percentage inside the pie
                    series.ToolTip = "#VALX: #VAL"; // Add tooltip on hover
                    series.Palette = ChartColorPalette.BrightPastel; // Use a pastel color palette
                    series.BorderColor = Color.White;
                    series.BorderWidth = 2;

                    // Customize legend
                    Legend legend = new Legend
                    {
                        Docking = Docking.Bottom,
                        Font = new Font("Segoe UI", 12, FontStyle.Regular),
                        BackColor = Color.WhiteSmoke,
                        ForeColor = ColorTranslator.FromHtml("#003366"),
                        BorderColor = ColorTranslator.FromHtml("#003366"),
                        BorderWidth = 1
                    };
                    KasambahayChart.Legends.Clear();
                    KasambahayChart.Legends.Add(legend);

                    // Enable 3D style for the pie chart
                    chartArea.Area3DStyle.Enable3D = true;
                    chartArea.Area3DStyle.Inclination = 30; // Adjust inclination for 3D
                    chartArea.Area3DStyle.Rotation = 45;    // Adjust rotation for 3D
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadJobSeekerChart()
        {
            // SQL query to count AvaileeID
            string query = "SELECT COUNT(AvaileeID) AS TotalCount FROM tblavailee";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    int totalCount = Convert.ToInt32(cmd.ExecuteScalar()); // Get the total count

                    // Clear previous data in the chart
                    JOBSEEKERchart.Series.Clear();
                    JOBSEEKERchart.ChartAreas.Clear();
                    JOBSEEKERchart.Titles.Clear();

                    // Set up the chart area
                    ChartArea chartArea = new ChartArea("JobSeekerArea");
                    chartArea.BackColor = Color.White; // Chart area background
                    chartArea.AxisX.MajorGrid.LineColor = Color.LightGray; // Grid lines
                    chartArea.AxisY.MajorGrid.LineColor = Color.LightGray; // Grid lines
                    chartArea.AxisX.Title = "Category";
                    chartArea.AxisY.Title = "Count";
                    chartArea.AxisX.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);
                    chartArea.AxisY.TitleFont = new Font("Segoe UI", 12, FontStyle.Bold);
                    chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                    chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                    chartArea.AxisX.LineColor = ColorTranslator.FromHtml("#004080");
                    chartArea.AxisY.LineColor = ColorTranslator.FromHtml("#004080");
                    JOBSEEKERchart.ChartAreas.Add(chartArea);

                    // Customize chart appearance
                    JOBSEEKERchart.BackColor = Color.WhiteSmoke; // Chart background
                    JOBSEEKERchart.BorderlineDashStyle = ChartDashStyle.Solid;
                    JOBSEEKERchart.BorderlineWidth = 2;
                    JOBSEEKERchart.BorderlineColor = ColorTranslator.FromHtml("#004080");

                    // Add a chart title
                    Title title = new Title("TOTAL JOB SEEKER ", Docking.Top, new Font("Impact", 16, FontStyle.Bold), ColorTranslator.FromHtml("#004080"));
                    title.ForeColor = ColorTranslator.FromHtml("#004080");
                    JOBSEEKERchart.Titles.Add(title);

                    // Create a series for the chart
                    var series = JOBSEEKERchart.Series.Add("Job Seekers");
                    series.ChartType = SeriesChartType.Column; // Use a column chart for job seekers
                    series.Points.AddXY("TOTAL JOB SEEKERS", totalCount); // Add the total count
                    series.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                    series.LabelForeColor = Color.White; // Label color
                    series.Label = "#VAL"; // Show the exact value on the chart
                    series.ToolTip = "#VALX: #VAL"; // Add tooltip on hover
                    series.Color = ColorTranslator.FromHtml("#1E90FF"); // Blue color for the bar
                    series.BorderWidth = 2;

                    // Customize legend
                    Legend legend = new Legend
                    {
                        Docking = Docking.Bottom,
                        Font = new Font("Segoe UI", 12, FontStyle.Regular),
                        BackColor = Color.WhiteSmoke,
                        ForeColor = ColorTranslator.FromHtml("#004080"),
                        BorderColor = ColorTranslator.FromHtml("#004080"),
                        BorderWidth = 1
                    };
                    JOBSEEKERchart.Legends.Clear();
                    JOBSEEKERchart.Legends.Add(legend);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BlotterChart_Click(object sender, EventArgs e)
        {

        }
    }
}
