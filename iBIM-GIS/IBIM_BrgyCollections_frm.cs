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
using System.Globalization;

namespace iBIM_GIS
{
    public partial class IBIM_BrgyCollections_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        public IBIM_BrgyCollections_frm()
        {
            InitializeComponent();
        }
      
        private void LoadInflowsInformation(string particularsCategory)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Query to fetch only the specified columns based on ParticularsCategory
                    string query = @"
                SELECT 
                    TransactionID, 
                    ParticularsCategory, 
                    TotalAmount, 
                    TransactionStatus
                FROM tblinflowsinformation 
                WHERE ParticularsCategory = @ParticularsCategory;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ParticularsCategory", particularsCategory);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    DGWRECORDSPERPARTICULARS.DataSource = dataTable;
                    DGWRECORDSPERPARTICULARS.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    // Adjust row and header heights
                    DGWRECORDSPERPARTICULARS.RowTemplate.Height = 35; // Adjust height as needed
                    DGWRECORDSPERPARTICULARS.ColumnHeadersHeight = 40; // Adjust height as needed

                    // Calculate the total of the TotalAmount column
                    query = "SELECT SUM(TotalAmount) FROM tblinflowsinformation WHERE ParticularsCategory = @ParticularsCategory;";
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ParticularsCategory", particularsCategory);

                    object result = cmd.ExecuteScalar();
                    decimal totalAmount = result != DBNull.Value ? Convert.ToDecimal(result) : 0;

                    // Format the total amount in Philippine Peso (₱)
                    LBLTOTALCOLLECTIONPERPARTICULARS.Text = $" {totalAmount.ToString("C", new CultureInfo("ph-PH"))}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void LoadParticulars()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ParticularsCategory FROM tblparticulars;";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvParticulars.DataSource = dataTable;
                    dgvParticulars.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvParticulars.RowTemplate.Height = 35; // Adjust height as needed
                    dgvParticulars.ColumnHeadersHeight = 40; // Adjust height as needed
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void IBIM_BrgyCollections_frm_Load(object sender, EventArgs e)
        {
            LoadParticulars();
        }

        private void dgvParticulars_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure a valid row is clicked
            {
                string particularsCategory = dgvParticulars.Rows[e.RowIndex].Cells["ParticularsCategory"].Value?.ToString();
                LoadInflowsInformation(particularsCategory);
            }
        }
    }
}
