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
    public partial class IBIM_SummaryofCases_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        public IBIM_SummaryofCases_frm()
        {
            InitializeComponent();
        }

        private void IBIM_SummaryofCases_frm_Load(object sender, EventArgs e)
        {
       
        }
        private void LoadReportWithParameter(string monthYear)
        {
            try
            {
                // Set the report path
                rv_CasesReport.LocalReport.ReportPath = System.Environment.CurrentDirectory + "\\Report\\TotalCasesReport.rdlc";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Query to fetch relevant data for the selected MonthYear
                    string query = @"
            SELECT 
                BlotterID AS ID, 
                IncidentDate AS Date, 
                IncidentDetails AS Details, 
                'Blotter' AS Source 
            FROM tblblotter 
            WHERE DATE_FORMAT(IncidentDate, '%M %Y') = @MonthYear
            UNION ALL
            SELECT 
                SumonID AS ID, 
                dtPSumon AS Date, 
                rtbSumonDetails AS Details, 
                'Sumon' AS Source 
            FROM tblsumon 
            WHERE DATE_FORMAT(dtPSumon, '%M %Y') = @MonthYear
            UNION ALL
            SELECT 
                ComplaintID AS ID, 
                DateFiled AS Date, 
                Description AS Details, 
                'Complaint' AS Source 
            FROM tblcomplaint 
            WHERE DATE_FORMAT(DateFiled, '%M %Y') = @MonthYear;
            ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MonthYear", monthYear);

                    // Load the main dataset
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet dset = new DataSet();
                    adapter.Fill(dset, "CasesData");

                    // Query to select all data from tblimage
                    string imageQuery = "SELECT * FROM tblimage";
                    MySqlCommand imageCmd = new MySqlCommand(imageQuery, conn);
                    MySqlDataAdapter imageAdapter = new MySqlDataAdapter(imageCmd);
                    DataSet dsetImage = new DataSet();
                    imageAdapter.Fill(dsetImage, "tblimage");


                    // Query to fetch the BarangayName
                    string barangayQuery = "SELECT BarangayName FROM tblsetbrgyname LIMIT 1";
                    MySqlCommand barangayCmd = new MySqlCommand(barangayQuery, conn);
                    string barangayName = barangayCmd.ExecuteScalar()?.ToString() ?? "Unknown Barangay";
                    // Query to fetch Chairman and Secretary data
                    string officialQuery = @"
                        SELECT FullName, Signature, Position 
                        FROM tblofficialinfo 
                        WHERE Position IN ('Chairman', 'Secretary');
";

                    MySqlCommand officialCmd = new MySqlCommand(officialQuery, conn);
                    MySqlDataReader officialReader = officialCmd.ExecuteReader();

                    string chairmanName = "Unknown";
                    string secretaryName = "Unknown";

                    while (officialReader.Read())
                    {
                        string position = officialReader["Position"].ToString();
                        string fullName = officialReader["FullName"].ToString();
                        if (position == "Chairman")
                        {
                            chairmanName = fullName;
                        }
                        else if (position == "Secretary")
                        {
                            secretaryName = fullName;
                        }
                    }
                    officialReader.Close();

                    // Query to count statuses across all tables
                    string statusQuery = @"
                    SELECT 'Criminal' AS Status, 
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Criminal') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Criminal') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Criminal') AS Count
                    UNION ALL
                    SELECT 'Civil',
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Civil') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Civil') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Civil')
                    UNION ALL
                    SELECT 'Others',
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Others') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Others') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Others')
                    UNION ALL
                    SELECT 'Mediation',
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Mediation') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Mediation') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Mediation')
                    UNION ALL
                    SELECT 'Conciliation',
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Conciliation') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Conciliation') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Conciliation')
                    UNION ALL
                    SELECT 'Arbitration',
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Arbitration') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Arbitration') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Arbitration')
                    UNION ALL
                    SELECT 'Repudiated',
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Repudiated') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Repudiated') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Repudiated')
                    UNION ALL
                    SELECT 'Withdrawn',
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Withdrawn') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Withdrawn') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Withdrawn')
                    UNION ALL
                    SELECT 'Pending',
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Pending') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Pending') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Pending')
                    UNION ALL
                    SELECT 'Dismissed',
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Dismissed') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Dismissed') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Dismissed')
                    UNION ALL
                    SELECT 'Certified to File Action',
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Certified to File Action') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Certified to File Action') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Certified to File Action')
                    UNION ALL
                    SELECT 'Referred to Concerned Agencies',
                           (SELECT COUNT(*) FROM tblblotter WHERE Status = 'Referred to Concerned Agencies') +
                           (SELECT COUNT(*) FROM tblsumon WHERE cbstatusSumon = 'Referred to Concerned Agencies') +
                           (SELECT COUNT(*) FROM tblcomplaint WHERE StatusComplaint = 'Referred to Concerned Agencies');
                    ";

                    MySqlCommand statusCmd = new MySqlCommand(statusQuery, conn);
                    MySqlDataReader reader = statusCmd.ExecuteReader();
                    Dictionary<string, int> statusCounts = new Dictionary<string, int>();

                    while (reader.Read())
                    {
                        string status = reader["Status"].ToString();
                        int count = Convert.ToInt32(reader["Count"]);
                        statusCounts[status] = count;
                    }
                    reader.Close();

                    // Calculate TotalNATUREOFDISPUTES (sum of Criminal, Civil, Others)
                    int totalNatureOfDisputes = 0;
                    if (statusCounts.ContainsKey("Criminal"))
                        totalNatureOfDisputes += statusCounts["Criminal"];
                    if (statusCounts.ContainsKey("Civil"))
                        totalNatureOfDisputes += statusCounts["Civil"];
                    if (statusCounts.ContainsKey("Others"))
                        totalNatureOfDisputes += statusCounts["Others"];

                    // Calculate TotalSettledCases (sum of Mediation, Conciliation, Arbitration)
                    int totalSettledCases = 0;
                    if (statusCounts.ContainsKey("Mediation"))
                        totalSettledCases += statusCounts["Mediation"];
                    if (statusCounts.ContainsKey("Conciliation"))
                        totalSettledCases += statusCounts["Conciliation"];
                    if (statusCounts.ContainsKey("Arbitration"))
                        totalSettledCases += statusCounts["Arbitration"];

                    // Calculate TotalSettledCases (sum of Mediation, Conciliation, Arbitration, Repudiated)
                    int TotalUnSettledCases = 0;
                    if (statusCounts.ContainsKey("Repudiated"))
                        TotalUnSettledCases += statusCounts["Repudiated"];
                    if (statusCounts.ContainsKey("Withdrawn"))
                        TotalUnSettledCases += statusCounts["Withdrawn"];
                    if (statusCounts.ContainsKey("Pending"))
                        TotalUnSettledCases += statusCounts["Pending"];
                    if (statusCounts.ContainsKey("Dismissed"))
                        TotalUnSettledCases += statusCounts["Dismissed"];
                    if (statusCounts.ContainsKey("Certified to File Action"))
                        TotalUnSettledCases += statusCounts["Certified to File Action"]; 
                     if (statusCounts.ContainsKey("Certified to File Action"))
                        TotalUnSettledCases += statusCounts["Referred to Concerned Agencies"];

                    // Create ReportDataSources
                    ReportDataSource rdsCases = new ReportDataSource("CasesDataset", dset.Tables["CasesData"]);
                    ReportDataSource rdsImage = new ReportDataSource("dsetimage", dsetImage.Tables["tblimage"]);

                    // Clear existing data sources and add the new ones
                    rv_CasesReport.LocalReport.DataSources.Clear();
                    rv_CasesReport.LocalReport.DataSources.Add(rdsCases);
                    rv_CasesReport.LocalReport.DataSources.Add(rdsImage);

                    // Create parameters for all statuses
                    List<ReportParameter> statusParameters = statusCounts.Select(kvp =>
                        new ReportParameter(kvp.Key.Replace(" ", ""), kvp.Value.ToString())).ToList();

                    // Add the new TotalNATUREOFDISPUTES parameter
                    ReportParameter rpTotalNATUREOFDISPUTES = new ReportParameter("TotalNATUREOFDISPUTES", totalNatureOfDisputes.ToString());

                    // Add the new TotalSettledCases parameter
                    ReportParameter rpTotalSettledCases = new ReportParameter("TotalSettledCases", totalSettledCases.ToString());

                    // Add the new TotalUnSettledCases parameter
                    ReportParameter rpTotalUnSettledCases = new ReportParameter("TotalUnSettledCases", TotalUnSettledCases.ToString());

                    // Add as report parameters
                    ReportParameter rpChairmanName = new ReportParameter("ChairmanName", chairmanName);
                    ReportParameter rpSecretaryName = new ReportParameter("SecretaryName", secretaryName);
      

                    // Add MonthYear and BarangayName parameters
                    ReportParameter rpBarangayName = new ReportParameter("BarangayName", barangayName);

                    rv_CasesReport.LocalReport.SetParameters(new ReportParameter[] {  rpBarangayName, rpTotalNATUREOFDISPUTES, rpTotalSettledCases, rpTotalUnSettledCases, rpChairmanName,
                    rpSecretaryName,
                    });
                    rv_CasesReport.LocalReport.SetParameters(statusParameters);

                    // Display mode settings
                    rv_CasesReport.SetDisplayMode(DisplayMode.PrintLayout);
                    rv_CasesReport.ZoomMode = ZoomMode.FullPage;

                    // Refresh the report
                    rv_CasesReport.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void rv_CasesReport_Load_1(object sender, EventArgs e)
        {
            // Example: Get the MonthYear value, e.g., "December 2024"
            string monthYear = DateTime.Now.ToString("MMMM yyyy");

            // Call the LoadReportWithParameter method with the MonthYear value
            LoadReportWithParameter(monthYear);

            // Refresh the report viewer
            rv_CasesReport.RefreshReport();
        }
    }
}
