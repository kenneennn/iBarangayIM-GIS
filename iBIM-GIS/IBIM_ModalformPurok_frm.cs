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
    public partial class IBIM_ModalformPurok_frm : Form
    {
        private const string ConnectionString = "Server=localhost;Database=dbibim;Uid=root;Pwd=;";
        private GMapOverlay markersOverlay;
        private GMapOverlay drawingOverlay;
        private List<PointLatLng> drawingPoints;
        private GMapPolygon currentPolygon;
        private GMapRoute currentRoute;
        private bool isDrawing;
        private Pen drawingPen = new Pen(Color.Red, 2);
        private SolidBrush polygonFillBrush = new SolidBrush(Color.FromArgb(50, Color.Blue));
        private Pen polygonStrokePen = new Pen(Color.Blue, 2);
        public IBIM_ModalformPurok_frm()
        {
            InitializeComponent();drawingPoints = new List<PointLatLng>();isDrawing = true;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnupload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Title = "Select an Image"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Image image = Image.FromFile(openFileDialog.FileName))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, image.RawFormat);
                        byte[] imageBytes = ms.ToArray();
                        txtImagePath.Text = Convert.ToBase64String(imageBytes);
                    }
                }
            }
        }
        private void gmapPurok_Load(object sender, EventArgs e)
        {
            gmapPurok.MapProvider = GMapProviders.GoogleSatelliteMap;
            gmapPurok.Position = new PointLatLng(17.0233, 121.6314); 
            gmapPurok.MinZoom = 2;
            gmapPurok.MaxZoom = 18;
            gmapPurok.Zoom = 17; 
            gmapPurok.Manager.Mode = AccessMode.ServerOnly;
            gmapPurok.ShowCenter = false;
            markersOverlay = new GMapOverlay("markers");
            drawingOverlay = new GMapOverlay("drawing");
            gmapPurok.Overlays.Add(markersOverlay);
            gmapPurok.Overlays.Add(drawingOverlay);
            gmapPurok.MouseDown += gmapPurok_MouseDown;
            gmapPurok.MouseMove += gmapPurok_MouseMove; 
            LoadPuroks();
        }
        private void gMapurok_OnPolygonEnter(GMapPolygon item)
        {
         item.Stroke = new Pen(Color.Yellow, 2);gmapPurok.Refresh();
        }
        private void gMapurok_OnPolygonLeave(GMapPolygon item)
        {
          item.Stroke = polygonStrokePen;gmapPurok.Refresh();
        }
        private void LoadPuroks()
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string query = "SELECT PurokID, PurokName, PurokDescription, PurokImage, PurokLatitude, PurokLongitude, PurokCoordinates FROM tblpurokinfo";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                try
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string PurokID = reader.GetString("PurokID");
                        string purokName = reader.GetString("PurokName");
                        string purokDescription = reader.GetString("PurokDescription");
                        double latitude = reader.GetDouble("PurokLatitude");
                        double longitude = reader.GetDouble("PurokLongitude");

                        AddMarker(PurokID, purokName, latitude, longitude);

                        if (!reader.IsDBNull(reader.GetOrdinal("PurokCoordinates")))
                        {
                            string coordinates = reader.GetString("PurokCoordinates");
                            List<PointLatLng> points = coordinates.Split(';').Select(coord =>
                            {
                                string[] latLng = coord.Split(',');
                                return new PointLatLng(double.Parse(latLng[0]), double.Parse(latLng[1]));
                            }).ToList();

                            GMapPolygon polygon = new GMapPolygon(points, PurokID)
                            {
                                Stroke = polygonStrokePen,
                                Fill = polygonFillBrush
                            };
                            drawingOverlay.Polygons.Add(polygon);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading Puroks: {ex.Message}");
                }
            }
        }
        private void AddMarker(string PurokID, string purokName, double latitude, double longitude)
        {
            PointLatLng point = new PointLatLng(latitude, longitude);
            GMarkerGoogleType markerType = GetMarkerTypeByLatitude(latitude);

            GMarkerGoogle marker = new GMarkerGoogle(point, markerType)
            {
                ToolTipText = purokName
            };

            markersOverlay.Markers.Add(marker);
        }
        private GMarkerGoogleType GetMarkerTypeByLatitude(double latitude)
        {
            if (latitude > 17.00)
                return GMarkerGoogleType.green_dot;
            else if (latitude > 16.95)
                return GMarkerGoogleType.blue_dot;
            else
                return GMarkerGoogleType.red_dot;
        }
        private void gmapPurok_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointLatLng point = gmapPurok.FromLocalToLatLng(e.X, e.Y);
                drawingPoints.Add(point);

                if (drawingPoints.Count > 1)
                {
                    if (currentRoute != null)
                    {
                        drawingOverlay.Routes.Remove(currentRoute);
                    }

                    currentRoute = new GMapRoute(SmoothPoints(drawingPoints), "DrawingRoute")
                    {
                        Stroke = drawingPen
                    };
                    drawingOverlay.Routes.Add(currentRoute);
                }

                PurokLatitude.Text = point.Lat.ToString();
                PurokLongitude.Text = point.Lng.ToString();
            }
            else if (e.Button == MouseButtons.Right && isDrawing)
            {
                CompletePolygon();
            }
        }
        private void gmapPurok_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && drawingPoints.Count > 1)
            {
                PointLatLng point = gmapPurok.FromLocalToLatLng(e.X, e.Y);
                List<PointLatLng> previewPoints = new List<PointLatLng>(drawingPoints) { point };

                if (currentRoute != null)
                {
                    drawingOverlay.Routes.Remove(currentRoute);
                }

                currentRoute = new GMapRoute(SmoothPoints(previewPoints), "DrawingPreview")
                {
                    Stroke = drawingPen
                };
                drawingOverlay.Routes.Add(currentRoute);

                gmapPurok.Refresh();
            }
        }
        private void CompletePolygon()
        {
            if (drawingPoints.Count > 2)
            {
                currentPolygon = new GMapPolygon(SmoothPoints(drawingPoints), "DrawingPolygon")
                {
                    Stroke = polygonStrokePen,
                    Fill = polygonFillBrush
                };
                drawingOverlay.Polygons.Add(currentPolygon);

                // Clear the current drawing state
                drawingPoints.Clear();
                if (currentRoute != null)
                {
                    drawingOverlay.Routes.Remove(currentRoute);
                    currentRoute = null;
                }

                gmapPurok.Refresh();
            }
        }
        private List<PointLatLng> SmoothPoints(List<PointLatLng> points)
        {
            return points;
        }
        private void LoadPuroksdgv()
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string query = "SELECT PurokID, PurokName FROM tblpurokinfo";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                try
                {
                    conn.Open();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    dgvpurok.DataSource = dataTable;
                    dgvpurok.Columns["PurokID"].HeaderText = "Purok ID";
                    dgvpurok.Columns["PurokName"].HeaderText = "Purok Name";
                    dgvpurok.RowTemplate.Height = 35; // Adjust height as needed
                    dgvpurok.ColumnHeadersHeight = 40; // Adjust height as needed
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading Puroks: {ex.Message}");
                }
            }
        }
        private void IBIM_ModalformPurok_frm_Load(object sender, EventArgs e)
        {
            LoadPuroksdgv();
            DisplayBarangayName();
            dgvpurok.CellClick += dgvpurok_CellClick; // Add event handler
        }
        private void ClearfieldsSave()
        {
            PurokName.Text = string.Empty;
            PurokDescription.Text = string.Empty;
            PurokLatitude.Text = string.Empty;
            PurokLongitude.Text = string.Empty;
            txtImagePath.Text = string.Empty;
        }
        private void ClearFields()
        {
            PurokName.Text = string.Empty;
            PurokDescription.Text = string.Empty;
            PurokLatitude.Text = string.Empty;
            PurokLongitude.Text = string.Empty;
            txtImagePath.Text = string.Empty;
            drawingPoints.Clear(); 
            if (currentRoute != null)
            {
                drawingOverlay.Routes.Remove(currentRoute);
                currentRoute = null;
            }
            gmapPurok.Refresh(); // Refresh the map to reflect changes
        }
        private void dgvpurok_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvpurok.Rows[e.RowIndex];
                string purokID = row.Cells["PurokID"].Value.ToString();
                string purokName = row.Cells["PurokName"].Value.ToString();
                PurokName.Text = purokName;
                ClearFields();
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    string query = "SELECT PurokDescription, PurokImage, PurokLatitude, PurokLongitude, PurokCoordinates FROM tblpurokinfo WHERE PurokID = @PurokID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PurokID", purokID);

                    try
                    {
                        conn.Open();
                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            PurokDescription.Text = reader["PurokDescription"].ToString();
                            PurokLatitude.Text = reader["PurokLatitude"].ToString();
                            PurokLongitude.Text = reader["PurokLongitude"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("PurokImage")))
                            {
                                byte[] imgBytes = (byte[])reader["PurokImage"];
                                string base64String = Convert.ToBase64String(imgBytes);
                                txtImagePath.Text = base64String;
                                using (MemoryStream ms = new MemoryStream(imgBytes))
                                {}
                            }
                            else
                            {
                                txtImagePath.Text = string.Empty;
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("PurokCoordinates")))
                            {
                                string coordinates = reader["PurokCoordinates"].ToString();
                                drawingPoints = coordinates.Split(';').Select(coord =>
                                {string[] latLng = coord.Split(',');
                                    return new PointLatLng(double.Parse(latLng[0]), double.Parse(latLng[1]));
                                }).ToList();
                                drawingOverlay.Polygons.Clear();
                                if (drawingPoints.Count > 2)
                                {
                                    currentPolygon = new GMapPolygon(SmoothPoints(drawingPoints), "DrawingPolygon")
                                    {
                                        Stroke = polygonStrokePen,
                                        Fill = polygonFillBrush
                                    };
                                    drawingOverlay.Polygons.Add(currentPolygon);
                                    gmapPurok.Refresh();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error retrieving Purok details: {ex.Message}");
                    }
                }
            }
        }
        private void btnupdateee_Click(object sender, EventArgs e)
        {
            if (dgvpurok.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a Purok record to update.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string selectedPurokID = dgvpurok.SelectedRows[0].Cells[0].Value.ToString();
            if (string.IsNullOrEmpty(selectedPurokID))
            {
                MessageBox.Show("Invalid Purok selected.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string query = "UPDATE tblpurokinfo SET " +
                               "PurokName = @PurokName, " +
                               "PurokDescription = @PurokDescription, " +
                               "PurokImage = @PurokImage " +
                               "WHERE PurokID = @PurokID";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PurokID", selectedPurokID);
                cmd.Parameters.AddWithValue("@PurokName", PurokName.Text);
                cmd.Parameters.AddWithValue("@PurokDescription", PurokDescription.Text);
                if (!string.IsNullOrEmpty(txtImagePath.Text))
                {
                    try
                    {
                        byte[] imgBytes = Convert.FromBase64String(txtImagePath.Text);
                        cmd.Parameters.AddWithValue("@PurokImage", imgBytes);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("The image data in the text box is not a valid Base64 string.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@PurokImage", DBNull.Value);
                }

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Purok information updated successfully!", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        markersOverlay.Markers.Clear();
                        drawingOverlay.Polygons.Clear();
                        LoadPuroks();
                        LoadPuroksdgv();
                        ClearfieldsSave();
                    }
                    else
                    {
                        MessageBox.Show("No changes were made or Purok not found.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating Purok: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GeneratePurokID()
        {
            return "P-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private void btnsaveee_Click(object sender, EventArgs e)
        {
            string generatedPurokID = GeneratePurokID();
            if (string.IsNullOrWhiteSpace(PurokName.Text) ||
                string.IsNullOrWhiteSpace(PurokDescription.Text) ||
                string.IsNullOrWhiteSpace(PurokLatitude.Text) ||
                string.IsNullOrWhiteSpace(PurokLongitude.Text) ||
                currentPolygon == null ||
                string.IsNullOrWhiteSpace(txtImagePath.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string query = "INSERT INTO tblpurokinfo (PurokID, PurokName, PurokDescription, PurokImage, PurokLatitude, PurokLongitude, PurokCoordinates) " +
                               "VALUES (@PurokID, @PurokName, @PurokDescription, @PurokImage, @PurokLatitude, @PurokLongitude, @PurokCoordinates)";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@PurokID", generatedPurokID);
                cmd.Parameters.AddWithValue("@PurokName", PurokName.Text);
                cmd.Parameters.AddWithValue("@PurokDescription", PurokDescription.Text);
                cmd.Parameters.AddWithValue("@PurokLatitude", double.Parse(PurokLatitude.Text));
                cmd.Parameters.AddWithValue("@PurokLongitude", double.Parse(PurokLongitude.Text));

                if (!string.IsNullOrEmpty(txtImagePath.Text))
                {
                    try
                    {
                        byte[] imgBytes = Convert.FromBase64String(txtImagePath.Text);
                        cmd.Parameters.AddWithValue("@PurokImage", imgBytes);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("The image data in the text box is not a valid Base64 string.");
                        return;
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@PurokImage", DBNull.Value);
                }

                if (currentPolygon != null)
                {
                    string coordinates = string.Join(";", currentPolygon.Points.Select(p => $"{p.Lat},{p.Lng}"));
                    cmd.Parameters.AddWithValue("@PurokCoordinates", coordinates);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@PurokCoordinates", DBNull.Value);
                }

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Purok information saved successfully!");

                    markersOverlay.Markers.Clear();
                    drawingOverlay.Polygons.Clear();
                    LoadPuroks();
                    LoadPuroksdgv();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving Purok: {ex.Message}");
                }
            }
        }
        private void btndeleteee_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this Purok?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                if (dgvpurok.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a Purok to delete.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string selectedPurokID = dgvpurok.SelectedRows[0].Cells[0].Value.ToString();

                if (string.IsNullOrWhiteSpace(selectedPurokID))
                {
                    MessageBox.Show("Invalid Purok selected.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    string query = "DELETE FROM tblpurokinfo WHERE PurokID = @PurokID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PurokID", selectedPurokID);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Purok deleted successfully.");
                            markersOverlay.Markers.Clear();
                            drawingOverlay.Polygons.Clear();
                            LoadPuroks();
                            LoadPuroksdgv();
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("Error: Purok not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting Purok: {ex.Message}");
                    }
                }
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
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
                        label8.Text = $"{barangayName} MONITORING MAP PER PUROK".ToUpper();  // Ensure the whole text is uppercase
                    }
                    else
                    {
                        label8.Text = "Barangay information not found.".ToUpper();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtsearchpurok_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtsearchpurok.Text.Trim(); // Get the search text

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    // Query to filter residents by ResidentName based on the search text
                    string query = "SELECT PurokID, PurokName FROM tblpurokinfo WHERE PurokName LIKE @PurokName";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PurokName", "%" + searchValue + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable); // Fill the DataTable with the filtered data
                    dgvpurok.DataSource = dataTable; // Bind the filtered data to the DataGridView
                    dgvpurok.RowTemplate.Height = 35; // Adjust height as needed
                    dgvpurok.ColumnHeadersHeight = 40; // Adjust height as needed
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message); // Display error message
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
