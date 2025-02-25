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

namespace iBIM_GIS
{
    public partial class IBIM_Particulars_frm : Form
    {
        private string connectionString = "Server=localhost;Database=dbiBIM;User=root;Password=;";
        public IBIM_Particulars_frm()
        {
            InitializeComponent();
            LoadParticularsData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate that all fields are filled
            if (string.IsNullOrWhiteSpace(txtparticularsid.Text) || string.IsNullOrWhiteSpace(txtparticulars.Text))
            {
                MessageBox.Show("Please fill in all fields before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO tblparticulars (ParticularsID, ParticularsCategory) VALUES (@ParticularsID, @ParticularsCategory)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ParticularsID", txtparticularsid.Text.Trim());
                        cmd.Parameters.AddWithValue("@ParticularsCategory", txtparticulars.Text.Trim());

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Particular saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh the DataGridView and clear the textboxes
                        LoadParticularsData();
                        ClearFields();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btndelete_Click(object sender, EventArgs e)
        {
            if (dgvparticulars.SelectedRows.Count > 0)
            {
                // Show a confirmation dialog before deleting
                DialogResult result = MessageBox.Show("Are you sure you want to delete this particular?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "DELETE FROM tblparticulars WHERE ParticularsID = @ParticularsID";
                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {
                                string selectedID = dgvparticulars.SelectedRows[0].Cells["ParticularsID"].Value.ToString();
                                cmd.Parameters.AddWithValue("@ParticularsID", selectedID);

                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Particular deleted successfully!");

                                // Refresh the DataGridView
                                LoadParticularsData();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
                // If the user selects "No", do nothing and exit the method
            }
            else
            {
                MessageBox.Show("Please select a particular to delete.");
            }
        }
        private void LoadParticularsData()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ParticularsID, ParticularsCategory FROM tblparticulars";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dgvparticulars.DataSource = dataTable;

                        // Set custom header names
                        dgvparticulars.Columns["ParticularsID"].HeaderText = "Particulars ID";
                        dgvparticulars.Columns["ParticularsCategory"].HeaderText = "Particulars";

                        // Adjust column widths
                        dgvparticulars.Columns["ParticularsID"].Width = 200; // Adjust as needed
                        dgvparticulars.Columns["ParticularsCategory"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                        // Set row height
                        dgvparticulars.RowTemplate.Height = 35; // Adjust height as needed

                        // Set header row height
                        dgvparticulars.ColumnHeadersHeight = 40; // Adjust height as needed
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgvparticulars_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvparticulars.Rows[e.RowIndex];
                txtparticularsid.Text = row.Cells["ParticularsID"].Value.ToString();
                txtparticulars.Text = row.Cells["ParticularsCategory"].Value.ToString();
            }
        }
        private void ClearFields()
        {
            txtparticularsid.Clear();
            txtparticulars.Clear();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
