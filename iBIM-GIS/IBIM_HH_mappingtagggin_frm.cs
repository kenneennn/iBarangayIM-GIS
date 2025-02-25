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
using System.Diagnostics;
using Microsoft.Web.WebView2.WinForms;

namespace iBIM_GIS
{
    public partial class IBIM_HH_mappingtagggin_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";

        public IBIM_HH_mappingtagggin_frm()
        {
            InitializeComponent();
            comboBoxMapType.Items.Add("Map");
            comboBoxMapType.Items.Add("Satellite");
            comboBoxMapType.Items.Add("Hybrid");
            comboBoxMapType.SelectedIndex = 0;
            dgvmap.CellClick += dgvmap_CellClick;
            btnSave.Text = "Save";
            btnSave.Enabled = true;
            gmapPurok.OnMarkerClick += gmapPurok_OnMarkerClick;
            gmapPurok.MouseClick += gmapProject_MouseClick;
        }
        private void gmapProject_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointLatLng point = gmapPurok.FromLocalToLatLng(e.X, e.Y);
                double latitude = point.Lat;
                double longitude = point.Lng;
                string streetViewUrl = $"https://maps.google.com/maps?q=&layer=c&cbll={latitude},{longitude}";
                webViewPurok.Source = new Uri(streetViewUrl);
                txtLatitude.Text = point.Lat.ToString();
                txtLongitude.Text = point.Lng.ToString();
            }
        }
        private void dgvmap_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvmap.Rows[e.RowIndex];
                txtHHNumber.Text = row.Cells["HouseHoldNo"].Value.ToString();
                txtHHName.Text = row.Cells["HouseHoldName"].Value.ToString();
                string hhNumber = txtHHNumber.Text;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT HouseHoldName, PurokName, Latitude, Longitude FROM tblmapping WHERE HouseHoldNo = @HouseHoldNo";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@HouseHoldNo", hhNumber);

                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string hhName = reader["HouseHoldName"].ToString();
                                    string purokName = reader["PurokName"].ToString();
                                    double latitude = Convert.ToDouble(reader["Latitude"]);
                                    double longitude = Convert.ToDouble(reader["Longitude"]);

                                    txtHHName.Text = hhName;
                                    cbPurokName.SelectedItem = purokName;
                                    txtLatitude.Text = latitude.ToString();
                                    txtLongitude.Text = longitude.ToString();

                                    // Update the marker on the map
                                    AddMarkerToMap(hhNumber, hhName, latitude, longitude);

                                    // Update the Street View in webViewPurok
                                    string streetViewUrl = $"https://www.google.com/maps?q=&layer=c&cbll={latitude},{longitude}&cbp=11,0,0,0,0";

                                    // Update the WebView2 source with the Street View URL
                                    webViewPurok.Source = new Uri(streetViewUrl);

                                    // Ensure WebView2 is fully initialized before navigating
                                    if (webViewPurok.CoreWebView2 != null)
                                    {
                                        webViewPurok.CoreWebView2.Navigate(streetViewUrl);
                                    }
                                    else
                                    {
                                        webViewPurok.CoreWebView2InitializationCompleted += (s, args) =>
                                        {
                                            if (args.IsSuccess)
                                            {
                                                webViewPurok.CoreWebView2.Navigate(streetViewUrl);
                                            }
                                        };
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading data: " + ex.Message);
                    }
                }
                btnSave.Enabled = false;
            }
        }
        private void AddMarkerToMap(string hhNumber, string hhName, double latitude, double longitude)
        {
            // Check if the markers overlay exists
            GMapOverlay markersOverlay = gmapPurok.Overlays.FirstOrDefault(o => o.Id == "markers");
            if (markersOverlay == null)
            {
                markersOverlay = new GMapOverlay("markers");
                gmapPurok.Overlays.Add(markersOverlay);
            }

            // Clear previous markers for a fresh display (optional, remove if you want to keep old markers)
            markersOverlay.Markers.Clear();

            // Initialize counts with default values
            int totalResidents = 0;
            int totalSeniorCitizens = 0;
            int totalMales = 0;
            int totalFemales = 0;

            // Query the database for resident statistics
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                COUNT(*) AS TotalResidents,
                SUM(CASE WHEN SCStatus = 'MEMBER' THEN 1 ELSE 0 END) AS TotalSeniorCitizens,
                SUM(CASE WHEN Sex = 'Male' THEN 1 ELSE 0 END) AS TotalMales,
                SUM(CASE WHEN Sex = 'Female' THEN 1 ELSE 0 END) AS TotalFemales
            FROM tblresidentprofiling 
            WHERE HouseHoldNo = @HouseHoldNo";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@HouseHoldNo", hhNumber);

                try
                {
                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalResidents = reader["TotalResidents"] != DBNull.Value ? reader.GetInt32("TotalResidents") : 0;
                            totalSeniorCitizens = reader["TotalSeniorCitizens"] != DBNull.Value ? reader.GetInt32("TotalSeniorCitizens") : 0;
                            totalMales = reader["TotalMales"] != DBNull.Value ? reader.GetInt32("TotalMales") : 0;
                            totalFemales = reader["TotalFemales"] != DBNull.Value ? reader.GetInt32("TotalFemales") : 0;
                        }
                    }
                }
                catch
                {
                    // Ignore errors and use default values
                }
            }

            // Add a new marker
            GMapMarker marker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.red_dot)
            {
                ToolTipText = $"HouseHold No: {hhNumber}\n" +
                              $"HouseHold Name: {hhName}\n" +
                              $"Total Residents: {totalResidents}\n" +
                              $"Total Senior Citizens: {totalSeniorCitizens}\n" +
                              $"Total Males: {totalMales}\n" +
                              $"Total Females: {totalFemales}\n" +
                              $"Coordinates: {latitude}, {longitude}"
            };

            // Customize the tooltip
            var toolTip = new GMapToolTip(marker)
            {
                Font = new Font("Impact", 14, FontStyle.Bold),
                Foreground = Brushes.White, // Text color
                Fill = new SolidBrush(Color.FromArgb(4, 21, 98)), // Background color
                Stroke = new Pen(Color.LightGray, 2) // Border color and width
            };

            marker.ToolTip = toolTip;
            marker.ToolTipMode = MarkerTooltipMode.Always; // Always show the tooltip

            // Add the marker to the overlay
            markersOverlay.Markers.Add(marker);

            // Refresh the map to show updates
            gmapPurok.Refresh();
        }

        private void IBIM_HH_mappingtagggin_frm_Load(object sender, EventArgs e)
        {
            PopulatePurokNames();
            LoadSavedMarkers();  
            LoadDataToDataGridView(); 
            CustomizeDataGridView();
            gmapPurok.OnMarkerClick += new MarkerClick(gmapPurok_OnMarkerClick);
            DisplayBarangayName();


        }
        private void DisplayBarangayName()
        {
            string connectionString = "server=localhost;user=root;database=dbiBIM;password=;";
            string query = "SELECT BarangayName FROM tblsetbrgyname LIMIT 1";  // Assuming you want to get the first entry

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())  
                    {
                        string barangayName = reader["BarangayName"].ToString().ToUpper();
                        label1.Text = $"{barangayName} MONITORING MAP PER HOUSEHOLD".ToUpper(); 
                    }
                    else
                    {
                        label1.Text = "Barangay information not found.".ToUpper();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gmapPurok_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            double latitude = item.Position.Lat;
            double longitude = item.Position.Lng;
            string streetViewUrl = $"https://maps.google.com/maps?q=&layer=c&cbll={latitude},{longitude}";
            webViewPurok.Source = new Uri(streetViewUrl);
        }
        private void CustomizeDataGridView()
        {
            dgvmap.Columns[0].Width = 140; // Set the width of the first column (HouseHold No)
            dgvmap.Columns[1].Width = 200; // Set the width of the second column (HouseHold Name)
            dgvmap.Columns[0].HeaderText = "HH#";   // Set header text for the first column
            dgvmap.Columns[1].HeaderText = "HouseHold Name"; // Set header text for the second column
            dgvmap.DefaultCellStyle.Font = new Font("Impact", 14);
            dgvmap.ColumnHeadersDefaultCellStyle.Font = new Font("Impact", 14, FontStyle.Bold);
        }
        private void PopulatePurokNames()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT PurokName FROM tblpurokinfo";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbPurokName.Items.Add(reader["PurokName"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Purok names: " + ex.Message);
                }
            }
        }
        private void gmapPurok_Load(object sender, EventArgs e)
        {
            gmapPurok.MapProvider = GMapProviders.GoogleMap;
            gmapPurok.Position = new PointLatLng(17.0233, 121.6314);
            gmapPurok.MinZoom = 2;
            gmapPurok.MaxZoom = 18;
            gmapPurok.Zoom = 16;
            gmapPurok.CanDragMap = true;
            gmapPurok.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            gmapPurok.IgnoreMarkerOnMouseWheel = true;
            gmapPurok.NegativeMode = false;
            gmapPurok.ShowTileGridLines = false;
            GMapOverlay markersOverlay = new GMapOverlay("markers");
            GMapMarker marker = new GMarkerGoogle(new PointLatLng(16.4023, 120.5960), GMarkerGoogleType.red_dot);
            markersOverlay.Markers.Add(marker);
            gmapPurok.Overlays.Add(markersOverlay);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string hhNumber = txtHHNumber.Text;
            string hhName = txtHHName.Text;
            string purokName = cbPurokName.SelectedItem?.ToString();
            string latitude = txtLatitude.Text;
            string longitude = txtLongitude.Text;

            if (string.IsNullOrWhiteSpace(hhNumber) || string.IsNullOrWhiteSpace(hhName) ||
                string.IsNullOrWhiteSpace(purokName) || string.IsNullOrWhiteSpace(latitude) ||
                string.IsNullOrWhiteSpace(longitude))
            {
                MessageBox.Show("Please fill in all the fields before saving.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string hhID = GenerateHHID();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(*) FROM tblmapping WHERE HouseHoldNo = @HouseHoldNo";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@HouseHoldNo", hhNumber);
                        int existingRecords = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (existingRecords > 0)
                        {
                            MessageBox.Show("The HouseHoldNo already exists. Please use a unique HouseHoldNo.", "Duplicate HouseHoldNo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string query = "INSERT INTO tblmapping (HHID, HouseHoldNo, HouseHoldName, PurokName, Latitude, Longitude) " +
                                   "VALUES (@HHID, @HouseHoldNo, @HouseHoldName, @PurokName, @Latitude, @Longitude)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@HHID", hhID);
                        command.Parameters.AddWithValue("@HouseHoldNo", hhNumber);
                        command.Parameters.AddWithValue("@HouseHoldName", hhName);
                        command.Parameters.AddWithValue("@PurokName", purokName);
                        command.Parameters.AddWithValue("@Latitude", latitude);
                        command.Parameters.AddWithValue("@Longitude", longitude);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            AddMarkerToMap(double.Parse(latitude), double.Parse(longitude), hhNumber, hhName);
                            ClearFields();
                            LoadDataToDataGridView();
                        }
                        else
                        {
                            MessageBox.Show("Failed to save the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while saving the record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private string GenerateHHID()
        {
            return "HH" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        private void LoadSavedMarkers()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT HouseHoldNo, HouseHoldName, Latitude, Longitude FROM tblmapping";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string hhNumber = reader["HouseHoldNo"].ToString();
                            string hhName = reader["HouseHoldName"].ToString();
                            double latitude = double.Parse(reader["Latitude"].ToString());
                            double longitude = double.Parse(reader["Longitude"].ToString());
                            AddMarkerToMap(latitude, longitude, hhNumber, hhName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading saved markers: " + ex.Message);
                }
            }
        }
        private void AddMarkerToMap(double latitude, double longitude, string hhNumber, string hhName)
        {
            GMapOverlay markersOverlay = gmapPurok.Overlays.FirstOrDefault(o => o.Id == "markers");
            if (markersOverlay == null)
            {
                markersOverlay = new GMapOverlay("markers");
                gmapPurok.Overlays.Add(markersOverlay);
            }
            GMapMarker marker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.red_dot);
            marker.ToolTipText = $"HouseHold No: {hhNumber}\nHouseHold Name: {hhName}\nCoordinates: {latitude}, {longitude}";
            var toolTip = new GMapToolTip(marker)
            {
                Foreground = Brushes.White, 
                Fill = Brushes.DarkSlateGray, 
                Font = new Font("Century Gothic", 10, FontStyle.Bold), 
                Stroke = new Pen(Brushes.Black, 2) 
            };
            marker.ToolTip = toolTip;
            marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            markersOverlay.Markers.Add(marker);
            gmapPurok.Refresh();
        }
        private void CustomizeFormFont(Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is Label || ctrl is TextBox || ctrl is ComboBox || ctrl is Button)
                {
                    ctrl.Font = new Font("Century Gothic", 10, FontStyle.Regular);
                }

                if (ctrl.HasChildren)
                {
                    CustomizeFormFont(ctrl);
                }
            }
        }

        private void ClearFields()
        {
            // Clear text fields
            txtHHNumber.Clear();
            txtHHName.Clear();
            cbPurokName.SelectedIndex = -1;
            txtLatitude.Clear();
            txtLongitude.Clear();
            btnSave.Enabled = true;

            // Reset WebView2 (clear the street view)
            webViewPurok.Source = new Uri("about:blank"); // Navigate to a blank page to clear the view

            // Reset GMap (clear all markers)
            GMapOverlay markersOverlay = gmapPurok.Overlays.FirstOrDefault(o => o.Id == "markers");
            if (markersOverlay != null)
            {
                markersOverlay.Markers.Clear(); // Remove all markers from the map
            }

            // Reload previously saved markers
            LoadSavedMarkers();  // Add this to reload markers

            // Refresh the map to apply the changes
            gmapPurok.Refresh();
        }


        private void comboBoxMapType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxMapType.SelectedItem.ToString())
            {
                case "Map":
                    gmapPurok.MapProvider = GMapProviders.GoogleMap;
                    break;
                case "Satellite":
                    gmapPurok.MapProvider = GMapProviders.GoogleSatelliteMap;
                    break;
                case "Hybrid":
                    gmapPurok.MapProvider = GMapProviders.GoogleHybridMap;
                    break;
                default:
                    gmapPurok.MapProvider = GMapProviders.GoogleMap;
                    break;
            }
        }
        private void LoadDataToDataGridView()
        {using (MySqlConnection connection = new MySqlConnection(connectionString)){try{connection.Open();string query = "SELECT HouseHoldNo, HouseHoldName FROM tblmapping";using (MySqlCommand command = new MySqlCommand(query, connection))
        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command)){DataTable dataTable = new DataTable();adapter.Fill(dataTable);dgvmap.DataSource = dataTable;dgvmap.Columns[0].HeaderText = "HouseHold No";dgvmap.Columns[1].HeaderText = "HouseHold Name";
                        dgvmap.RowTemplate.Height = 35; // Adjust height as needed
                        dgvmap.ColumnHeadersHeight = 40; // Adjust height as needed
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading data: " + ex.Message);
                }
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        { ClearFields();}
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string hhNumber = txtHHNumber.Text;
            string hhName = txtHHName.Text;
            string purokName = cbPurokName.SelectedItem?.ToString();
            string latitude = txtLatitude.Text;
            string longitude = txtLongitude.Text;
            if (string.IsNullOrWhiteSpace(hhNumber) || string.IsNullOrWhiteSpace(hhName) ||
                string.IsNullOrWhiteSpace(purokName) || string.IsNullOrWhiteSpace(latitude) ||
                string.IsNullOrWhiteSpace(longitude))
            {
                MessageBox.Show("Please fill in all the fields before updating.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string checkQuery = "SELECT HouseHoldName FROM tblmapping WHERE HouseHoldNo = @HouseHoldNo AND HouseHoldName != @HouseHoldName";
                    string existingHouseHoldName = null;

                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@HouseHoldNo", hhNumber);
                        checkCommand.Parameters.AddWithValue("@HouseHoldName", hhName);
                        using (MySqlDataReader reader = checkCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                existingHouseHoldName = reader["HouseHoldName"].ToString();
                            }
                        }
                    }
                    if (existingHouseHoldName != null)
                    {
                        MessageBox.Show($"The HouseHoldNo {hhNumber} is already assigned to a different HouseHoldName ({existingHouseHoldName}).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string updateQuery = "UPDATE tblmapping SET HouseHoldNo = @HouseHoldNo, PurokName = @PurokName, Latitude = @Latitude, Longitude = @Longitude WHERE HouseHoldName = @HouseHoldName";

                    using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@HouseHoldNo", hhNumber);
                        updateCommand.Parameters.AddWithValue("@PurokName", purokName);
                        updateCommand.Parameters.AddWithValue("@Latitude", latitude);
                        updateCommand.Parameters.AddWithValue("@Longitude", longitude);
                        updateCommand.Parameters.AddWithValue("@HouseHoldName", hhName);

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("HouseHoldNo updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearFields();
                            LoadDataToDataGridView();
                            gmapPurok.Overlays.Clear();
                            LoadSavedMarkers();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while updating the record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string hhNumber = txtHHNumber.Text;

            if (string.IsNullOrWhiteSpace(hhNumber))
            {
                MessageBox.Show("Please select a record to delete.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "DELETE FROM tblmapping WHERE HouseHoldNo = @HouseHoldNo";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@HouseHoldNo", hhNumber);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Record deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ClearFields();
                                LoadDataToDataGridView();
                                gmapPurok.Overlays.Clear();
                                LoadSavedMarkers();
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void txtsearchHHNo_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtsearchHHNo.Text.Trim();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = @"SELECT HouseholdNo, HouseHoldName
                             FROM tblmapping 
                             WHERE HouseholdNo LIKE @searchText ";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvmap.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while searching: " + ex.Message);
            }
        }
    }
}