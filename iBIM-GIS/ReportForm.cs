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
using Microsoft.Reporting.WinForms;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;

namespace iBIM_GIS
{
    public partial class ReportForm : Form
    {
        private string connectionString;
        private string officialId;
        public ReportForm(string connectionString, string officialId)
        {
            InitializeComponent();
            this.connectionString = connectionString;
            this.officialId = officialId;

            // Load the report with the official ID
            LoadReport();
        }
        private void LoadReport()
        {
            try
            {
                MySqlDataAdapter sDAa = new MySqlDataAdapter();
                MySqlDataAdapter sDAb = new MySqlDataAdapter();
                DataSet1 aDSa = new DataSet1();
                DataSet3 aDSb = new DataSet3();

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Query for tblofficialinfo
                    MySqlCommand cmd1 = new MySqlCommand(
                        "SELECT * FROM tblofficialinfo WHERE OfficialID = @OfficialID", conn);
                    cmd1.Parameters.AddWithValue("@OfficialID", officialId);
                    sDAa.SelectCommand = cmd1;
                    sDAa.Fill(aDSa.Tables[0]);

                    // Query for tblmayorsignature
                    MySqlCommand cmd2 = new MySqlCommand("SELECT * FROM tblmayorsignature", conn);
                    sDAb.SelectCommand = cmd2;
                    sDAb.Fill(aDSb.Tables[0]);

                    // Query for tblimage
                    MySqlCommand cmd3 = new MySqlCommand("SELECT * FROM tblimage", conn);
                    sDAb.SelectCommand = cmd3;
                    sDAb.Fill(aDSb.Tables[0]);

                    // Configure ReportViewer
                    reportViewer.LocalReport.ReportPath = System.Environment.CurrentDirectory + "\\Report\\OfficialID.rdlc";
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.LocalReport.DataSources.Clear();

                    // Add DataSources for ReportViewer
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", aDSa.Tables[0]));
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet3", aDSb.Tables[0]));
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("dsetimage", aDSb.Tables[0]));
                    reportViewer.LocalReport.DataSources.Add(new ReportDataSource("dsetmayor", aDSb.Tables[0]));
                    reportViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    reportViewer.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;
                    reportViewer.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btncloseform_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close(); // Close the form explicitly
        }
    }
}