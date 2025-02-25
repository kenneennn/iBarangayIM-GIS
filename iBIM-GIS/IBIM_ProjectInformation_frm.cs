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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace iBIM_GIS
{
    public partial class IBIM_ProjectInformation_frm : Form
    {
        string connectionString = "server=localhost;user=root;database=dbiBIM;password=;";

        public IBIM_ProjectInformation_frm()
        {
            InitializeComponent();
            comboBoxMapType.Items.Add("Map");
            comboBoxMapType.Items.Add("Satellite");
            comboBoxMapType.Items.Add("Hybrid");
            comboBoxMapType.SelectedIndex = 0;

            // Attach the MouseClick event handler
            gmapProject.MouseClick += gmapProject_MouseClick;

            // Add this line in your form's constructor (after InitializeComponent) to attach the event
            gmapProject.OnMarkerClick += GmapProject_OnMarkerClick;

            // Register the DataGridView SelectionChanged event
            dgvprojectinformation.SelectionChanged += dgvprojectinformation_SelectionChanged;

        }
        private void GmapProject_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            // Get the coordinates of the clicked marker
            double latitude = item.Position.Lat;
            double longitude = item.Position.Lng;

            // Construct the Street View URL using the marker coordinates
            string streetViewUrl = $"https://maps.google.com/maps?q=&layer=c&cbll={latitude},{longitude}";

            // Load the URL into the WebView2 control
            webViewProject.Source = new Uri(streetViewUrl);
        }
        private void gmapProject_MouseClick(object sender, MouseEventArgs e)
        {
            // Check if the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Get the coordinates of the clicked point
                PointLatLng point = gmapProject.FromLocalToLatLng(e.X, e.Y);
                double latitude = point.Lat;
                double longitude = point.Lng;

                // Construct the Street View URL using the clicked coordinates
                string streetViewUrl = $"https://maps.google.com/maps?q=&layer=c&cbll={latitude},{longitude}";

                // Load the URL into the WebView2 control
                webViewProject.Source = new Uri(streetViewUrl);

                // Display the coordinates in the textboxes
                txtLatitude.Text = point.Lat.ToString();
                txtLongitude.Text = point.Lng.ToString();
            }
        }
        private void gmapProject_Load(object sender, EventArgs e)
        {
            {
                // Initial map settings
                gmapProject.MapProvider = GMapProviders.GoogleMap;
                gmapProject.Position = new PointLatLng(17.0233, 121.6314);
                gmapProject.MinZoom = 2;
                gmapProject.MaxZoom = 18;
                gmapProject.Zoom = 17;
                gmapProject.CanDragMap = true;
                gmapProject.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
                gmapProject.IgnoreMarkerOnMouseWheel = true;
                gmapProject.NegativeMode = false;
                gmapProject.ShowTileGridLines = false;

                // Create a marker overlay
                GMapOverlay markersOverlay = new GMapOverlay("markers");

                // Example: Adding a marker to the map
                GMapMarker marker = new GMarkerGoogle(new PointLatLng(16.4023, 120.5960), GMarkerGoogleType.red_dot);
                markersOverlay.Markers.Add(marker);

                // Add the overlay to the map
                gmapProject.Overlays.Add(markersOverlay);
            }
        }

        private void comboBoxMapType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxMapType.SelectedItem.ToString())
            {
                case "Map":
                    gmapProject.MapProvider = GMapProviders.GoogleMap;
                    break;
                case "Satellite":
                    gmapProject.MapProvider = GMapProviders.GoogleSatelliteMap;
                    break;
                case "Hybrid":
                    gmapProject.MapProvider = GMapProviders.GoogleHybridMap;
                    break;
                default:
                    gmapProject.MapProvider = GMapProviders.GoogleMap;
                    break;
            }
        }

        private void btnSaveProject_Click(object sender, EventArgs e)
        {
            // Validate all required fields
            if (string.IsNullOrWhiteSpace(cbOfficial.Text) ||
                string.IsNullOrWhiteSpace(txtprocattype.Text) ||
                string.IsNullOrWhiteSpace(cbprogresstracker.Text) ||
                string.IsNullOrWhiteSpace(cbRisklevel.Text) ||
                string.IsNullOrWhiteSpace(txtLatitude.Text) ||
                string.IsNullOrWhiteSpace(txtLongitude.Text) ||
                string.IsNullOrWhiteSpace(cbprostatus.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Exit without saving
            }

            // Attempt to parse Latitude and Longitude fields
            if (!double.TryParse(txtLatitude.Text, out double latitude) ||
                !double.TryParse(txtLongitude.Text, out double longitude))
            {
                MessageBox.Show("Please enter valid numerical values for Latitude and Longitude.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Exit without saving
            }

            // Auto-generate ProjectID based on the current date
            string projectID = DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 6);

            // Prepare SQL query with parameters
            string query = @"INSERT INTO tblprojectinformation 
                    (ProjectID, ProjectCoordinator, ProjectType, ProjectTracker, RiskLevel, Latitude, Longitude, DateStart, DateEnded, ProjectStatus,ProjectBudget) 
                    VALUES (@ProjectID, @ProjectCoordinator, @ProjectType, @ProjectTracker, @RiskLevel, @Latitude, @Longitude, @DateStart, @DateEnded, @ProjectStatus,@ProjectBudget)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Bind parameters to the query
                        cmd.Parameters.AddWithValue("@ProjectID", projectID);
                        cmd.Parameters.AddWithValue("@ProjectCoordinator", cbOfficial.Text);
                        cmd.Parameters.AddWithValue("@ProjectType", txtprocattype.Text);
                        cmd.Parameters.AddWithValue("@ProjectTracker", cbprogresstracker.Text);
                        cmd.Parameters.AddWithValue("@RiskLevel", cbRisklevel.Text);
                        cmd.Parameters.AddWithValue("@Latitude", latitude);
                        cmd.Parameters.AddWithValue("@Longitude", longitude);
                        cmd.Parameters.AddWithValue("@DateStart", dateTimePickerDataStarted.Value);
                        cmd.Parameters.AddWithValue("@DateEnded", dateTimePickerDateEnded.Value);
                        cmd.Parameters.AddWithValue("@ProjectStatus", cbprostatus.Text);
                        cmd.Parameters.AddWithValue("@ProjectBudget", txtprojectbudget.Text);

                        // Execute the query
                        int result = cmd.ExecuteNonQuery();

                        // Check if the insert was successful
                        if (result > 0)
                        {
                            MessageBox.Show("Project saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadProjectInformation();
                            LoadProjectPins();
                            Clearfiels();
                        }
                        else
                        {
                            MessageBox.Show("Failed to save project.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadOfficials()
        {
            string query = "SELECT FullName FROM tblofficialinfo WHERE Position = 'Kagawad'";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Clear existing items to avoid duplicates
                            cbOfficial.Items.Clear();

                            // Loop through the result set and add each FullName to the ComboBox
                            while (reader.Read())
                            {
                                cbOfficial.Items.Add(reader["FullName"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors
                    MessageBox.Show($"Failed to load officials: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void LoadProjectInformation()
        {
            string query = "SELECT ProjectID, ProjectCoordinator, ProjectType FROM tblprojectinformation";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgvprojectinformation.DataSource = dt;

                        // Set header names
                        dgvprojectinformation.Columns[0].HeaderText = "Project ID";
                        dgvprojectinformation.Columns[1].HeaderText = "Coordinator";
                        dgvprojectinformation.Columns[2].HeaderText = "Project";
                        dgvprojectinformation.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        // Set row height
                        dgvprojectinformation.RowTemplate.Height = 35; // Adjust height as needed

                        // Set header row height
                        dgvprojectinformation.ColumnHeadersHeight = 40; // Adjust height as needed
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load project information: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvprojectinformation.SelectedRows.Count > 0)
            {
                string projectID = dgvprojectinformation.SelectedRows[0].Cells["ProjectID"].Value.ToString();

                string query = @"UPDATE tblprojectinformation 
                        SET ProjectCoordinator = @ProjectCoordinator, 
                            ProjectType = @ProjectType, 
                            ProjectTracker = @ProjectTracker, 
                            RiskLevel = @RiskLevel, 
                            Latitude = @Latitude, 
                            Longitude = @Longitude, 
                            DateStart = @DateStart, 
                            DateEnded = @DateEnded, 
                            ProjectStatus = @ProjectStatus,
                            ProjectBudget = @ProjectBudget
                        WHERE ProjectID = @ProjectID";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            // Bind parameters
                            cmd.Parameters.AddWithValue("@ProjectID", projectID);
                            cmd.Parameters.AddWithValue("@ProjectCoordinator", cbOfficial.Text);
                            cmd.Parameters.AddWithValue("@ProjectType", txtprocattype.Text);
                            cmd.Parameters.AddWithValue("@ProjectTracker", cbprogresstracker.Text);
                            cmd.Parameters.AddWithValue("@RiskLevel", cbRisklevel.Text);
                            cmd.Parameters.AddWithValue("@Latitude", double.Parse(txtLatitude.Text));
                            cmd.Parameters.AddWithValue("@Longitude", double.Parse(txtLongitude.Text));
                            cmd.Parameters.AddWithValue("@DateStart", dateTimePickerDataStarted.Value);
                            cmd.Parameters.AddWithValue("@DateEnded", dateTimePickerDateEnded.Value);
                            cmd.Parameters.AddWithValue("@ProjectStatus", cbprostatus.Text);
                            cmd.Parameters.AddWithValue("@ProjectBudget", txtprojectbudget.Text);

                            int result = cmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Project updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadProjectInformation();
                                Clearfiels();
                            }
                            else
                            {
                                MessageBox.Show("Failed to update project.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a project to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvprojectinformation.SelectedRows.Count > 0)
            {
                string projectID = dgvprojectinformation.SelectedRows[0].Cells["ProjectID"].Value.ToString();

                var confirmResult = MessageBox.Show("Are you sure you want to delete this project?",
                                                    "Confirm Delete",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    string query = "DELETE FROM tblprojectinformation WHERE ProjectID = @ProjectID";

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ProjectID", projectID);

                                int result = cmd.ExecuteNonQuery();

                                if (result > 0)
                                {
                                    MessageBox.Show("Project deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadProjectInformation();  // Refresh the DataGridView
                                    LoadProjectPins();
                                    Clearfiels();
                                }
                                else
                                {
                                    MessageBox.Show("Failed to delete project.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a project to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            // Clear the fields to allow entry of a new project
            cbOfficial.SelectedIndex = -1;
            txtprocattype.Clear();
            cbprogresstracker.SelectedIndex = -1;
            cbRisklevel.SelectedIndex = -1;
            txtLatitude.Clear();
            txtLongitude.Clear();
            dateTimePickerDataStarted.Value = DateTime.Now;
            dateTimePickerDateEnded.Value = DateTime.Now;
            cbprostatus.SelectedIndex = -1;
            txtprojectbudget.Clear();
            // Optionally focus the first input field
            cbOfficial.Focus();
            // Enable the Save button
            btnSaveProject.Enabled = true;
            // Reset WebView2 (clear the street view)
            webViewProject.Source = new Uri("about:blank"); // Navigate to a blank page to clear the view

            // Reset GMap (clear all markers)
            GMapOverlay markersOverlay = gmapProject.Overlays.FirstOrDefault(o => o.Id == "markers");
            if (markersOverlay != null)
            {
                markersOverlay.Markers.Clear(); // Remove all markers from the map
            }

            LoadProjectPins();

            // Refresh the map to apply the changes
            gmapProject.Refresh();
        }

        private void Clearfiels()
        {
            // Clear the fields to allow entry of a new project
            cbOfficial.SelectedIndex = -1;
            txtprocattype.Clear();
            cbprogresstracker.SelectedIndex = -1;
            cbRisklevel.SelectedIndex = -1;
            txtLatitude.Clear();
            txtLongitude.Clear();
            dateTimePickerDataStarted.Value = DateTime.Now;
            dateTimePickerDateEnded.Value = DateTime.Now;
            cbprostatus.SelectedIndex = -1;
            txtprojectbudget.Clear();

            // Optionally focus the first input field
            cbOfficial.Focus();
        }
        private void IBIM_ProjectInformation_frm_Load(object sender, EventArgs e)
        {
            // Load officials into the cbOfficial ComboBox
            LoadOfficials();
            // Load project information into the DataGridView
            LoadProjectInformation();
            LoadProjectPins();
            dgvprojectinformation.ClearSelection();

        }
        private void LoadProjectPins()
        {
            string query = @"SELECT ProjectCoordinator, ProjectType, ProjectTracker, 
                            ProjectStatus, Latitude, Longitude,  ProjectBudget
                     FROM tblprojectinformation";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Clear existing overlays to avoid duplicates
                            gmapProject.Overlays.Clear();

                            // Create a new overlay for project markers
                            GMapOverlay projectOverlay = new GMapOverlay("projects");

                            while (reader.Read())
                            {
                                // Extract the project details from the database
                                string coordinator = reader["ProjectCoordinator"].ToString();
                                string type = reader["ProjectType"].ToString();
                                string tracker = reader["ProjectTracker"].ToString();
                                string status = reader["ProjectStatus"].ToString();
                                double latitude = Convert.ToDouble(reader["Latitude"]);
                                double longitude = Convert.ToDouble(reader["Longitude"]);

                                // Create a marker for the project
                                GMapMarker marker = new GMarkerGoogle(
                                    new PointLatLng(latitude, longitude),
                                    GMarkerGoogleType.blue_pushpin
                                );

                                // Set the tooltip with relevant project details
                                marker.ToolTipText = $"Coordinator: {coordinator}\n" +
                                                     $"Type: {type}\n" +
                                                     $"Tracker: {tracker}\n" +
                                                     $"Status: {status}";
                                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;

                                // Add the marker to the overlay
                                projectOverlay.Markers.Add(marker);
                            }

                            // Add the overlay to the map
                            gmapProject.Overlays.Add(projectOverlay);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load project pins: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvprojectinformation_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvprojectinformation.SelectedRows.Count > 0)
            {
                var projectIDCell = dgvprojectinformation.SelectedRows[0].Cells["ProjectID"];

                if (projectIDCell.Value != null && !string.IsNullOrEmpty(projectIDCell.Value.ToString()))
                {
                    string projectID = projectIDCell.Value.ToString();
                    LoadProjectDetails(projectID);
                }
                else
                {
                    MessageBox.Show("The selected row does not contain a valid ProjectID.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                btnSaveProject.Enabled = false;
            }
        }
        private void LoadProjectDetails(string projectID)
        {
            string query = @"SELECT ProjectCoordinator, ProjectType, ProjectTracker, RiskLevel, Latitude, Longitude, 
                     DateStart, DateEnded, ProjectStatus, ProjectBudget 
                     FROM tblprojectinformation 
                     WHERE ProjectID = @ProjectID";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProjectID", projectID);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate the form fields with project details
                                cbOfficial.Text = reader["ProjectCoordinator"]?.ToString() ?? "N/A";
                                txtprocattype.Text = reader["ProjectType"]?.ToString() ?? "N/A";
                                cbprogresstracker.Text = reader["ProjectTracker"]?.ToString() ?? "N/A";
                                cbRisklevel.Text = reader["RiskLevel"]?.ToString() ?? "N/A";
                                txtLatitude.Text = reader["Latitude"]?.ToString() ?? "0";
                                txtLongitude.Text = reader["Longitude"]?.ToString() ?? "0";
                                dateTimePickerDataStarted.Value = reader["DateStart"] == DBNull.Value
                                    ? DateTime.Now
                                    : Convert.ToDateTime(reader["DateStart"]);
                                dateTimePickerDateEnded.Value = reader["DateEnded"] == DBNull.Value
                                    ? DateTime.Now
                                    : Convert.ToDateTime(reader["DateEnded"]);
                                cbprostatus.Text = reader["ProjectStatus"]?.ToString() ?? "N/A";
                                txtprojectbudget.Text = reader["ProjectBudget"]?.ToString() ?? "0";

                                // Get Latitude and Longitude values
                                if (double.TryParse(reader["Latitude"]?.ToString(), out double latitude) &&
                                    double.TryParse(reader["Longitude"]?.ToString(), out double longitude))
                                {
                                    // Create a GMapOverlay and add a marker to it
                                    GMapOverlay markersOverlay = new GMapOverlay("markers");

                                    // Create a tooltip with color and details
                                    var marker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.blue_dot)
                                    {
                                        ToolTipMode = MarkerTooltipMode.Always
                                    };

                                    // Customizing the tooltip
                                    marker.ToolTip = new GMapToolTip(marker)
                                    {
                                        Font = new Font("Century Ghotic", 12, FontStyle.Bold),
                                        Foreground = Brushes.White, // Text color
                                        Fill = new SolidBrush(Color.FromArgb(4, 21, 98)), // Background color
                                        Stroke = new Pen(Color.LightGray, 2) // Border color and width
                                    };

                                    // Add detailed text to the tooltip
                                    marker.ToolTipText = $"📌 **Project Details**\n" +
                                                         $"───────────────────────\n" +
                                                         $"Type: {txtprocattype.Text}\n" +
                                                         $"Coordinator: {cbOfficial.Text}\n" +
                                                         $"Status: {cbprostatus.Text}\n" +
                                                         $"Risk Level: {cbRisklevel.Text}\n" +
                                                         $"Budget: {txtprojectbudget.Text}\n" +
                                                         $"Start Date: {dateTimePickerDataStarted.Value:MMMM dd, yyyy}\n" +
                                                         $"End Date: {dateTimePickerDateEnded.Value:MMMM dd, yyyy}";

                                    // Add the marker to the overlay
                                    markersOverlay.Markers.Add(marker);

                                    // Add the overlay to the GMapControl
                                    gmapProject.Overlays.Clear(); // Clear existing overlays
                                    gmapProject.Overlays.Add(markersOverlay);

                                    // Update map position and zoom
                                    gmapProject.Position = new PointLatLng(latitude, longitude);
                                    gmapProject.Zoom = 18;

                                    // Update the Street View in webViewProject
                                    string streetViewUrl = $"https://www.google.com/maps?q=&layer=c&cbll={latitude},{longitude}&cbp=11,0,0,0,0";
                                    webViewProject.Source = new Uri(streetViewUrl);
                                }
                                else
                                {
                                    MessageBox.Show("Coordinates are missing or invalid. Map update skipped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    gmapProject.Overlays.Clear(); // Clear map overlays
                                }
                            }
                            else
                            {
                                MessageBox.Show($"No data found for ProjectID: {projectID}", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void txtsearchbyProjectTypeandProject_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtsearchbyProjectTypeandProject.Text.Trim();

            string query = @"SELECT ProjectCoordinator, ProjectType FROM tblprojectinformation 
                     WHERE ProjectCoordinator LIKE @SearchText 
                     OR ProjectType LIKE @SearchText";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dgvprojectinformation.DataSource = dt; // Bind results to the DataGridView
                                                                   // Set row height
                            dgvprojectinformation.RowTemplate.Height = 35; // Adjust height as needed

                            // Set header row height
                            dgvprojectinformation.ColumnHeadersHeight = 40; // Adjust height as needed
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
