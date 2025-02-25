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
    public partial class SumonReportForm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        private string sumonID; // Change from int to string

        public SumonReportForm(string sumonID)
        {
            InitializeComponent();
            this.sumonID = sumonID; // Store the string value
            LoadReport();
        }
        private void LoadReport()
        {
            try
            {
                // Clear existing data sources
                rv_SumonReport.LocalReport.DataSources.Clear();

                // Create new instances of your datasets
                DataSet4 dataset = new DataSet4();
                string chairmanFullName = string.Empty;
                string chairmanSignatureBase64 = string.Empty;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Load data from tblimage
                    MySqlCommand cmdImage = new MySqlCommand("SELECT * FROM tblimage", conn);
                    MySqlDataAdapter imageAdapter = new MySqlDataAdapter(cmdImage);
                    imageAdapter.Fill(dataset, "tblimage");

                    // Load specific sumon details based on SumonID
                    MySqlCommand cmdSumon = new MySqlCommand("SELECT * FROM tblsumon WHERE SumonID = @SumonID", conn);
                    cmdSumon.Parameters.AddWithValue("@SumonID", sumonID);
                    MySqlDataAdapter sumonAdapter = new MySqlDataAdapter(cmdSumon);
                    sumonAdapter.Fill(dataset, "tblsumon");

                    // Load Chairman's FullName and Signature from tblofficialinfo
                    MySqlCommand cmdOfficialInfo = new MySqlCommand(
                        "SELECT FullName FROM tblofficialinfo WHERE Position = 'Chairman'", conn);
                    using (MySqlDataReader reader = cmdOfficialInfo.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            chairmanFullName = reader["FullName"].ToString();
                        }
                    }

                    // Load data from tblsetbrgyname
                    MySqlCommand cmdBrgyName = new MySqlCommand("SELECT * FROM tblsetbrgyname", conn);
                    MySqlDataAdapter brgyNameAdapter = new MySqlDataAdapter(cmdBrgyName);
                    brgyNameAdapter.Fill(dataset, "tblsetbrgyname");

                    conn.Close();
                }
                if (string.IsNullOrEmpty(chairmanFullName))
                {
                    throw new Exception("Chairman's FullName or Signature is missing.");
                }

                // Set Report Path and add DataSources
                rv_SumonReport.LocalReport.ReportPath = System.Environment.CurrentDirectory + "\\Report\\SumonReport.rdlc";
                rv_SumonReport.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("dsetSumon", dataset.Tables["tblsumon"]));
                rv_SumonReport.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("dsetimage", dataset.Tables["tblimage"]));
                rv_SumonReport.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("dsetbarangayname", dataset.Tables["tblsetbrgyname"]));

                // Set Chairman parameters
                var punongBarangayParam = new Microsoft.Reporting.WinForms.ReportParameter("PunongBarangay", chairmanFullName);
                rv_SumonReport.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter[]
                {
                punongBarangayParam
                });

                rv_SumonReport.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                rv_SumonReport.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.FullPage;
                rv_SumonReport.RefreshReport();
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

        private void rv_SumonReport_Load(object sender, EventArgs e)
        {

        }
    }
}
