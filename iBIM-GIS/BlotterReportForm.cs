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
    public partial class BlotterReportForm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        private string blotterID;
        public BlotterReportForm(string blotterID)
        {
            InitializeComponent();
            this.blotterID = blotterID;
            LoadBlotterReport();
        }

        private void LoadBlotterReport()
        {
            try
            {
                reportViewerblotter.LocalReport.DataSources.Clear();

                DataSet4 dataset = new DataSet4();
                string chairmanFullName = string.Empty;
                string chairmanSignatureBase64 = string.Empty;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Load data from tblblotter based on BlotterID
                    MySqlCommand cmdBlotter = new MySqlCommand("SELECT * FROM tblblotter WHERE BlotterID = @BlotterID", conn);
                    cmdBlotter.Parameters.AddWithValue("@BlotterID", blotterID);

                    MySqlDataAdapter blotterAdapter = new MySqlDataAdapter(cmdBlotter);
                    blotterAdapter.Fill(dataset, "tblblotter");

                    // Load data from tblimage
                    MySqlCommand cmdImage = new MySqlCommand("SELECT * FROM tblimage", conn);
                    MySqlDataAdapter imageAdapter = new MySqlDataAdapter(cmdImage);
                    imageAdapter.Fill(dataset, "tblimage");

                    // Load Chairman's FullName and Signature
                    MySqlCommand cmdOfficialInfo = new MySqlCommand("SELECT FullName FROM tblofficialinfo WHERE Position = 'Chairman'", conn);
                    using (MySqlDataReader reader = cmdOfficialInfo.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            chairmanFullName = reader["FullName"].ToString();
                        }
                    }
                }

                if (string.IsNullOrEmpty(chairmanFullName))
                    throw new Exception("Chairman's information is incomplete.");

                reportViewerblotter.LocalReport.ReportPath = System.Environment.CurrentDirectory + "\\Report\\BlotterReport.rdlc";
                reportViewerblotter.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local;
                reportViewerblotter.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("tblblotter", dataset.Tables["tblblotter"]));
                reportViewerblotter.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("tblimage", dataset.Tables["tblimage"]));

                var punongBarangayParam = new Microsoft.Reporting.WinForms.ReportParameter("PunongBarangay", chairmanFullName);
                reportViewerblotter.LocalReport.SetParameters(new[] { punongBarangayParam });

                reportViewerblotter.DocumentMapCollapsed = true;
                reportViewerblotter.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                reportViewerblotter.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;
                reportViewerblotter.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btncloseform_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

