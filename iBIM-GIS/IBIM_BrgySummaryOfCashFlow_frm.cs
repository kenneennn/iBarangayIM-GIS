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
using System.Net.Mail;

namespace iBIM_GIS
{
    public partial class IBIM_BrgySummaryOfCashFlow_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        public IBIM_BrgySummaryOfCashFlow_frm()
        {
            InitializeComponent();
        }

        private void IBIM_BrgySummaryOfCashFlow_frm_Load(object sender, EventArgs e)
        {
            LoadMonthlyReportDataGridView();
            CustomizeDGV();

        }
        private void LoadMonthlyReportDataGridView()
        {
            int currentYear = DateTime.Now.Year;

            dgvmonthlyreport.Columns.Clear();
            dgvmonthlyreport.Columns.Add("MonthYear", "Month and Year");

            for (int month = 1; month <= 12; month++)
            {
                string monthYear = new DateTime(currentYear, month, 1).ToString("MMMM yyyy");
                dgvmonthlyreport.Rows.Add(monthYear);
                dgvmonthlyreport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }
        private void CustomizeDGV()
        {
            dgvmonthlyreport.Columns.Clear();
            DataGridViewTextBoxColumn monthYearColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Month and Year",
                Name = "MonthYear",
                Width = 200,
                ReadOnly = true
            };
            dgvmonthlyreport.Columns.Add(monthYearColumn);
            dgvmonthlyreport.ColumnHeadersDefaultCellStyle.Font = new Font("Impact", 14F, FontStyle.Regular);
            dgvmonthlyreport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvmonthlyreport.ColumnHeadersHeight = 40;
            dgvmonthlyreport.DefaultCellStyle.Font = new Font("Century Gothic", 14F, FontStyle.Regular);
            dgvmonthlyreport.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvmonthlyreport.RowTemplate.Height = 40;
            dgvmonthlyreport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvmonthlyreport.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgvmonthlyreport.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvmonthlyreport.AllowUserToAddRows = false;
            dgvmonthlyreport.AllowUserToDeleteRows = false;
            dgvmonthlyreport.AllowUserToResizeRows = false;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT DISTINCT DATE_FORMAT(DateofTransaction, '%M %Y') AS MonthYear FROM tblinflowsinformation ORDER BY DateofTransaction";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    { dgvmonthlyreport.Rows.Clear();
                        while (reader.Read())
                        {
                            dgvmonthlyreport.Rows.Add(reader["MonthYear"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgvmonthlyreport_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvmonthlyreport.Columns[e.ColumnIndex].Name == "MonthYear")
            {
                string selectedMonthYear = dgvmonthlyreport.Rows[e.RowIndex].Cells["MonthYear"].Value.ToString();
                LoadReportByMonth(selectedMonthYear);
            }
        }
        private void LoadReportByMonth(string monthYear)
        {
            rwMonthlyReport.SetDisplayMode(DisplayMode.PrintLayout);
            rwMonthlyReport.ZoomMode = ZoomMode.FullPage;

            string reportPath = Path.Combine(Environment.CurrentDirectory, "Report", "SummaryOfCashflowsReport.rdlc");
            rwMonthlyReport.LocalReport.ReportPath = reportPath;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Retrieve image dataset
                    MySqlDataAdapter adapterImage = new MySqlDataAdapter("SELECT * FROM tblimage", conn);
                    DataSet dsImage = new DataSet();
                    adapterImage.Fill(dsImage, "dsetimage");

                    // Retrieve inflows information dataset
                    string queryInflows = "SELECT * FROM tblinflowsinformation WHERE DATE_FORMAT(DateofTransaction, '%M %Y') = @MonthYear";
                    MySqlDataAdapter adapterInflows = new MySqlDataAdapter(queryInflows, conn);
                    adapterInflows.SelectCommand.Parameters.AddWithValue("@MonthYear", monthYear);
                    DataSet dsInflows = new DataSet();
                    adapterInflows.Fill(dsInflows, "dsetinflowsinformation");

                    // Retrieve liquidation dataset
                    string queryLiquidation = "SELECT * FROM tblLiquidation WHERE DATE_FORMAT(ORDate, '%M %Y') = @MonthYear";
                    MySqlDataAdapter adapterLiquidation = new MySqlDataAdapter(queryLiquidation, conn);
                    adapterLiquidation.SelectCommand.Parameters.AddWithValue("@MonthYear", monthYear);
                    DataSet dsLiquidation = new DataSet();
                    adapterLiquidation.Fill(dsLiquidation, "dsetliquidation");

                    // Calculate totals
                    string queryTotalCashInflow = "SELECT SUM(TotalAmount) FROM tblinflowsinformation WHERE DATE_FORMAT(DateofTransaction, '%M %Y') = @MonthYear";
                    MySqlCommand cmdTotalCashInflow = new MySqlCommand(queryTotalCashInflow, conn);
                    cmdTotalCashInflow.Parameters.AddWithValue("@MonthYear", monthYear);
                    decimal totalCashInflow = cmdTotalCashInflow.ExecuteScalar() == DBNull.Value ? 0 : Convert.ToDecimal(cmdTotalCashInflow.ExecuteScalar());

                    string queryTotalCashOutflow = "SELECT SUM(Amount) FROM tblLiquidation WHERE DATE_FORMAT(ORDate, '%M %Y') = @MonthYear";
                    MySqlCommand cmdTotalCashOutflow = new MySqlCommand(queryTotalCashOutflow, conn);
                    cmdTotalCashOutflow.Parameters.AddWithValue("@MonthYear", monthYear);
                    decimal totalCashOutflow = cmdTotalCashOutflow.ExecuteScalar() == DBNull.Value ? 0 : Convert.ToDecimal(cmdTotalCashOutflow.ExecuteScalar());

                    decimal totalNetCashFlow = totalCashInflow - totalCashOutflow;

                    // Retrieve Barangay Name
                    MySqlCommand cmdBarangay = new MySqlCommand("SELECT BarangayName FROM tblsetbrgyname LIMIT 1", conn);
                    string barangayName = cmdBarangay.ExecuteScalar() == DBNull.Value ? "Unknown Barangay" : cmdBarangay.ExecuteScalar().ToString();

                    // Retrieve Chairman Name
                    string queryChairman = "SELECT Fullname FROM tblofficialinfo WHERE Position = 'Chairman' LIMIT 1";
                    MySqlCommand cmdChairman = new MySqlCommand(queryChairman, conn);
                    string chairmanName = cmdChairman.ExecuteScalar() == DBNull.Value ? "No Chairman" : cmdChairman.ExecuteScalar().ToString();

                    // Retrieve Secretary Name
                    string querySecretary = "SELECT Fullname FROM tblofficialinfo WHERE Position = 'Secretary' LIMIT 1";
                    MySqlCommand cmdSecretary = new MySqlCommand(querySecretary, conn);
                    string secretaryName = cmdSecretary.ExecuteScalar() == DBNull.Value ? "No Secretary" : cmdSecretary.ExecuteScalar().ToString();

                    // Set the report title and current month
                    string reportTitle = $"Monthly Financial Report for Barangay Operations – {monthYear}";
                    string monthNow = DateTime.Now.ToString("MMMM yyyy");

                    rwMonthlyReport.LocalReport.DataSources.Clear();
                    rwMonthlyReport.LocalReport.DataSources.Add(new ReportDataSource("dsetimage", dsImage.Tables["dsetimage"]));
                    rwMonthlyReport.LocalReport.DataSources.Add(new ReportDataSource("dsetinflowsinformation", dsInflows.Tables["dsetinflowsinformation"]));
                    rwMonthlyReport.LocalReport.DataSources.Add(new ReportDataSource("dsetliquidation", dsLiquidation.Tables["dsetliquidation"]));

                    // Add parameters for the report
                    List<ReportParameter> parameters = new List<ReportParameter>
            {
                new ReportParameter("BarangayName", barangayName),
                new ReportParameter("MonthYear", reportTitle),
                new ReportParameter("MonthNow", monthNow),
                new ReportParameter("TotalCashInflowsOfThisMonth", totalCashInflow.ToString("C2")),
                new ReportParameter("TotalCashOutflowsOfThisMonth", totalCashOutflow.ToString("C2")),
                new ReportParameter("TotalNetCashFlowinthisMonth", totalNetCashFlow.ToString("C2")),
                new ReportParameter("ChairmanName", chairmanName),
                new ReportParameter("SecretaryName", secretaryName)
            };

                    rwMonthlyReport.LocalReport.SetParameters(parameters);

                    // Refresh the report
                    rwMonthlyReport.RefreshReport();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading report: " + ex.Message);
                }
            }
        }
    }
}
