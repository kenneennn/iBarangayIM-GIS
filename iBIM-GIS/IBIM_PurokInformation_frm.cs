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

namespace iBIM_GIS
{
    public partial class IBIM_PurokInformation_frm : Form
    {
        private CheckBox lastCheckedCheckbox = null;
        public IBIM_PurokInformation_frm()
        {
            InitializeComponent();

            comboBoxMapType.Items.Add("Map");
            comboBoxMapType.Items.Add("Satellite");
            comboBoxMapType.Items.Add("Hybrid");
            comboBoxMapType.SelectedIndex = 0;
            panelDescription.Visible = false;
            panel2.Visible = false;

            // Add KeyDown event handler
            this.KeyDown += IBIM_PurokInformation_frm_KeyDown;
            this.KeyPreview = true; // Enable KeyPreview to capture key events
        }
        private void RefreshPurokData()
        {
            try
            {
                // Load Purok data from the database
                DataTable purokData = LoadPurokData();

                // Clear existing checkboxes
                panelCheckboxes.Controls.Clear();

                // Setup for consistent layout
                int padding = 20; // Padding between checkboxes
                int yPos = padding; // Starting Y position with padding

                foreach (DataRow row in purokData.Rows)
                {
                    string purokName = row["PurokName"].ToString();
                    string formattedPurokName = AddSpacesToPurokName(purokName);

                    // Create a new CheckBox control
                    CheckBox purokCheckBox = new CheckBox
                    {
                        Text = formattedPurokName,
                        Tag = purokName,
                        AutoSize = false,
                        Size = new Size(250, 30),
                        Font = new Font("Century Gothic", 16, FontStyle.Regular),
                        ForeColor = Color.FromArgb(4, 21, 98),
                        Location = new Point(padding, yPos),
                        BackColor = Color.White,
                        FlatStyle = FlatStyle.Flat
                    };

                    // Customize the FlatStyle appearance
                    purokCheckBox.FlatAppearance.BorderSize = 1;
                    purokCheckBox.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);

                    // Attach the CheckedChanged event handler
                    purokCheckBox.CheckedChanged += checkBox_CheckedChanged;

                    // Add the CheckBox to the panel
                    panelCheckboxes.Controls.Add(purokCheckBox);

                    // Increment Y position for the next checkbox
                    yPos += purokCheckBox.Height + padding;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing Purok data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void IBIM_PurokInformation_frm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                RefreshPurokData();
            }
        }
        private void btnADDPUROK_Click(object sender, EventArgs e)
        {
            IBIM_ModalformPurok_frm IBIM_ModalformPurok_frm = new IBIM_ModalformPurok_frm();
            IBIM_ModalformPurok_frm.ShowDialog();
        }
        private void gmapPurok_Load(object sender, EventArgs e)
        {
            // Initial map settings
            gmapPurok.MapProvider = GMapProviders.GoogleMap;
            gmapPurok.Position = new PointLatLng(17.0233, 121.6314);
            gmapPurok.MinZoom = 2;
            gmapPurok.MaxZoom = 18;
            gmapPurok.Zoom = 17;
            gmapPurok.CanDragMap = true;
            gmapPurok.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            gmapPurok.IgnoreMarkerOnMouseWheel = true;
            gmapPurok.NegativeMode = false;
            gmapPurok.ShowTileGridLines = false;

            // Create a marker overlay
            GMapOverlay markersOverlay = new GMapOverlay("markers");

            // Example: Adding a marker to the map
            GMapMarker marker = new GMarkerGoogle(new PointLatLng(16.4023, 120.5960), GMarkerGoogleType.red_dot);
            markersOverlay.Markers.Add(marker);

            // Add the overlay to the map
            gmapPurok.Overlays.Add(markersOverlay);
        }
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox currentCheckBox = sender as CheckBox;

            if (currentCheckBox.Checked)
            {
                // Uncheck the last checked checkbox only if it's different from the current one
                if (lastCheckedCheckbox != null && lastCheckedCheckbox != currentCheckBox)
                {
                    // Remove previous markers and polygons from the map before updating
                    RemovePurokFromMap(lastCheckedCheckbox.Tag.ToString());

                    // Temporarily detach event to avoid recursion
                    lastCheckedCheckbox.CheckedChanged -= checkBox_CheckedChanged;
                    lastCheckedCheckbox.Checked = false;
                    lastCheckedCheckbox.CheckedChanged += checkBox_CheckedChanged;
                }

                // Update the last checked checkbox reference
                lastCheckedCheckbox = currentCheckBox;

                // Display the corresponding data on the map and in the details panel
                string purokName = currentCheckBox.Tag.ToString();
                DisplayPurokOnMap(purokName);
                DisplayPurokDetails(purokName);

                // Show the panels since a checkbox is selected
                panelDescription.Visible = true;
                panel2.Visible = true;
            }
            else if (lastCheckedCheckbox == currentCheckBox)
            {
                // If the current checkbox is unchecked, clear the previous data
                RemovePurokFromMap(currentCheckBox.Tag.ToString());
                lastCheckedCheckbox = null; // No checkbox is checked now

                // Hide the panels since no checkbox is selected
                panelDescription.Visible = false;
                panel2.Visible = false;
            }
        }

        private void DisplayPurokDetails(string purokName)
        {
            // Clear previous data (labels and image)
            lblpurokname.Text = string.Empty;
            labeltotalpopulation.Text = "0";
            labeltotalhouseholds.Text = "0";
            labelmale.Text = "0";
            labelfemale.Text = "0";
            labelseniorcitizen.Text = "0";
            labelpwd.Text = "0";
            pictureBoxPurokImage.Image = null;

            DataTable purokData = LoadPurokData(); // Load Purok metadata
            DataTable populationData = LoadPopulationDataByPurok(purokName); // Load population data for the selected Purok

            // Loop through the Purok data to find the matching Purok name
            foreach (DataRow row in purokData.Rows)
            {
                if (row["PurokName"].ToString() == purokName)
                {
                    // Set the label for the Purok name
                    lblpurokname.Text = row["PurokName"].ToString();

                    // Load the image into the PictureBox if available
                    if (row["PurokImage"] != DBNull.Value)
                    {
                        byte[] imageBytes = (byte[])row["PurokImage"];
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            try
                            {
                                pictureBoxPurokImage.Image = Image.FromStream(ms);
                                pictureBoxPurokImage.SizeMode = PictureBoxSizeMode.Zoom;
                                pictureBoxPurokImage.Padding = new Padding(20);
                                pictureBoxPurokImage.BackColor = Color.White;
                                pictureBoxPurokImage.BorderStyle = BorderStyle.FixedSingle;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error loading image: " + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        pictureBoxPurokImage.Image = null; // Clear image if none found
                    }

                    // Display population data in the labels
                    if (populationData.Rows.Count > 0)
                    {
                        DataRow popRow = populationData.Rows[0];
                        labeltotalpopulation.Text = popRow["TotalPopulation"].ToString();
                        labeltotalhouseholds.Text = popRow["TotalHouseholds"].ToString();
                        labelmale.Text = popRow["MaleCount"].ToString();
                        labelfemale.Text = popRow["FemaleCount"].ToString();
                        labelseniorcitizen.Text = popRow["SeniorCitizenCount"].ToString();
                        labelpwd.Text = popRow["PWDCount"].ToString();
                    }
                    else

                    {
                        // Clear labels if no population data is found
                        labeltotalpopulation.Text = "0";
                        labeltotalhouseholds.Text = "0";
                        labelmale.Text = "0";
                        labelfemale.Text = "0";
                        labelseniorcitizen.Text = "0";
                        labelpwd.Text = "0";
                    }
                }
            }
        }


        private DataTable LoadPopulationDataByPurok(string purokName)
        {
            DataTable dt = new DataTable();
            string connectionString = "server=localhost;user id=root;password=;database=dbibim;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    COUNT(*) AS TotalPopulation,
                    COUNT(DISTINCT HouseholdNo) AS TotalHouseholds,
                    SUM(CASE WHEN Sex = 'Male' THEN 1 ELSE 0 END) AS MaleCount,
                    SUM(CASE WHEN Sex = 'Female' THEN 1 ELSE 0 END) AS FemaleCount,
                    COUNT(CASE WHEN ClassificationbyAgeandHRG = 'SC - Senior Citizen' THEN 1 ELSE NULL END) AS SeniorCitizenCount,
                    COUNT(CASE WHEN ClassificationbyAgeandHRG = 'PWD - Persons with Disability' THEN 1 ELSE NULL END) AS PWDCount
                FROM 
                    tblresidentprofiling
                WHERE 
                    PurokName = @PurokName";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PurokName", purokName);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return dt;
        }


        private void ClearPurokInformation()
        {
            lblpurokname.Text = string.Empty;
            panelDescription.Controls.Clear();
            pictureBoxPurokImage.Image = null;
        }
        private void DisplayPurokOnMap(string purokName)
        {
            DataTable purokData = LoadPurokData();

            foreach (DataRow row in purokData.Rows)
            {
                if (row["PurokName"].ToString() == purokName)
                {
                    double latitude = Convert.ToDouble(row["PurokLatitude"]);
                    double longitude = Convert.ToDouble(row["PurokLongitude"]);
                    string description = row["PurokDescription"].ToString();
                    string coordinates = row["PurokCoordinates"].ToString();

                    // Add a marker for the main point
                    GMapMarker marker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.red_dot)
                    {
                        ToolTipText = $"{purokName}: {description}"
                    };

                    GMapOverlay markersOverlay = gmapPurok.Overlays.FirstOrDefault(o => o.Id == "markers");
                    if (markersOverlay == null)
                    {
                        markersOverlay = new GMapOverlay("markers");
                        gmapPurok.Overlays.Add(markersOverlay);
                    }

                    markersOverlay.Markers.Add(marker);

                    // Draw additional coordinates
                    List<PointLatLng> points = new List<PointLatLng>();
                    string[] coordinatePairs = coordinates.Split(';');

                    foreach (string pair in coordinatePairs)
                    {
                        string[] latLon = pair.Split(',');
                        if (latLon.Length == 2)
                        {
                            double lat = Convert.ToDouble(latLon[0]);
                            double lon = Convert.ToDouble(latLon[1]);
                            points.Add(new PointLatLng(lat, lon));
                        }
                    }

                    if (points.Count > 0)
                    {
                        GMapPolygon polygon = new GMapPolygon(points, purokName);
                        GMapOverlay polygonsOverlay = gmapPurok.Overlays.FirstOrDefault(o => o.Id == "polygons");
                        if (polygonsOverlay == null)
                        {
                            polygonsOverlay = new GMapOverlay("polygons");
                            gmapPurok.Overlays.Add(polygonsOverlay);
                        }

                        polygonsOverlay.Polygons.Add(polygon);
                    }
                }
            }

            gmapPurok.Refresh();
        }
        private void RemovePurokFromMap(string purokName)
        {
            // Remove markers
            GMapOverlay markersOverlay = gmapPurok.Overlays.FirstOrDefault(o => o.Id == "markers");
            if (markersOverlay != null)
            {
                // Find markers to remove by comparing the Tag property or ToolTipText
                var markersToRemove = markersOverlay.Markers
                    .Where(m => m.ToolTipText != null && m.ToolTipText.StartsWith(purokName))
                    .ToList();

                // Debug: Check how many markers are being removed
                Console.WriteLine($"Removing {markersToRemove.Count} markers for {purokName}.");

                // Remove each marker found
                foreach (var marker in markersToRemove)
                {
                    markersOverlay.Markers.Remove(marker);
                }
            }

            // Remove polygons
            GMapOverlay polygonsOverlay = gmapPurok.Overlays.FirstOrDefault(o => o.Id == "polygons");
            if (polygonsOverlay != null)
            {
                // Find polygons to remove by comparing the Name property
                var polygonsToRemove = polygonsOverlay.Polygons
                    .Where(p => p.Name == purokName)
                    .ToList();

                // Debug: Check how many polygons are being removed
                Console.WriteLine($"Removing {polygonsToRemove.Count} polygons for {purokName}.");

                // Remove each polygon found
                foreach (var polygon in polygonsToRemove)
                {
                    polygonsOverlay.Polygons.Remove(polygon);
                }
            }

            // Refresh the map to update the changes
            gmapPurok.Refresh();
        }

        private DataTable LoadPurokData()
        {
            DataTable dt = new DataTable();
            string connectionString = "server=localhost;user id=root;password=;database=dbibim;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT PurokID, PurokName, PurokDescription, PurokImage, PurokLatitude, PurokLongitude, PurokCoordinates FROM tblpurokinfo";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    adapter.Fill(dt);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return dt;
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
        private void IBIM_PurokInformation_frm_Load(object sender, EventArgs e)
        {
            try
            {
                // Load Purok data from the database
                DataTable purokData = LoadPurokData();

                // Clear any existing controls in the checkbox panel
                panelCheckboxes.Controls.Clear();

                // Setup for consistent layout
                int padding = 20; // Padding between checkboxes
                int yPos = padding; // Starting Y position with padding

                foreach (DataRow row in purokData.Rows)
                {
                    string purokName = row["PurokName"].ToString();

                    // Add spaces to PurokName for better readability
                    string formattedPurokName = AddSpacesToPurokName(purokName);

                    // Create a new CheckBox control
                    CheckBox purokCheckBox = new CheckBox
                    {
                        Text = formattedPurokName, // Use the formatted name
                        Tag = purokName,  // Store PurokName in Tag for easy access
                        AutoSize = false, // Disable auto-sizing to apply custom size
                        Size = new Size(250, 30), // Set a fixed size for consistency
                        Font = new Font("Century Gothic", 16, FontStyle.Regular),
                        ForeColor = Color.FromArgb(4, 21, 98), // Stylish color
                        Location = new Point(padding, yPos),
                        BackColor = Color.White,
                        FlatStyle = FlatStyle.Flat // Flat style for a cleaner look
                    };

                    // Customize the FlatStyle appearance
                    purokCheckBox.FlatAppearance.BorderSize = 1;
                    purokCheckBox.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);

                    // Attach the CheckedChanged event handler
                    purokCheckBox.CheckedChanged += checkBox_CheckedChanged;

                    // Add the CheckBox to the panel
                    panelCheckboxes.Controls.Add(purokCheckBox);

                    // Increment Y position for the next checkbox
                    yPos += purokCheckBox.Height + padding; // Add checkbox height and padding
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Purok data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                    if (reader.Read())  // Check if there's at least one record
                    {
                        string barangayName = reader["BarangayName"].ToString().ToUpper();  // Convert to uppercase
                                                                                            // Set the label text with the fetched BarangayName in uppercase
                        label1.Text = $"{barangayName} MONITORING MAP PER PUROK".ToUpper();  // Ensure the whole text is uppercase
                    }
                    else
                    {
                        // If no data found
                        label1.Text = "Barangay information not found.".ToUpper();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string AddSpacesToPurokName(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]); // Add the first character as is

            // Loop through each character starting from the second one
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                {
                    newText.Append(' '); // Insert space before the uppercase letter
                }
                newText.Append(text[i]);
            }

            return newText.ToString();
        }
        private void pictureBoxPurokImage_Click(object sender, EventArgs e)
        {
            pictureBoxPurokImage.Size = new Size(296, 220);
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Image selectedImage = Image.FromFile(openFileDialog.FileName);
                    pictureBoxPurokImage.Image = selectedImage;
                    pictureBoxPurokImage.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }
    }
}
