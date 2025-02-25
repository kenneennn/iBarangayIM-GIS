
namespace iBIM_GIS
{
    partial class IBIM_SUMMARYOFPOPULATIONBYAGE_HRG_frm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblTotalPopulationPerPurok = new System.Windows.Forms.Label();
            this.lblPurokName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewHRGGG = new System.Windows.Forms.ListView();
            this.listViewHRGGGG = new System.Windows.Forms.ListView();
            this.listViewHGGR = new System.Windows.Forms.ListView();
            this.listViewHRG = new System.Windows.Forms.ListView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.DGWListofgovernmentassistance = new System.Windows.Forms.DataGridView();
            this.lbltotalmemberperpuork = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxpurok = new System.Windows.Forms.ComboBox();
            this.cbxgovernmentassit = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgw_purok = new System.Windows.Forms.DataGridView();
            this.label11 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGWListofgovernmentassistance)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgw_purok)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(21)))), ((int)(((byte)(98)))));
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Impact", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(8, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1668, 914);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SUMMARY OF POPULATION BY AGE/HRG";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Impact", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(366, 88);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1302, 824);
            this.tabControl1.TabIndex = 38;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(21)))), ((int)(((byte)(98)))));
            this.tabPage1.Controls.Add(this.lblTotalPopulationPerPurok);
            this.tabPage1.Controls.Add(this.lblPurokName);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.listViewHRGGG);
            this.tabPage1.Controls.Add(this.listViewHRGGGG);
            this.tabPage1.Controls.Add(this.listViewHGGR);
            this.tabPage1.Controls.Add(this.listViewHRG);
            this.tabPage1.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.ForeColor = System.Drawing.Color.White;
            this.tabPage1.Location = new System.Drawing.Point(4, 35);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1294, 785);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "SUMMARY OF POPULATION BY HRG";
            // 
            // lblTotalPopulationPerPurok
            // 
            this.lblTotalPopulationPerPurok.AutoSize = true;
            this.lblTotalPopulationPerPurok.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalPopulationPerPurok.ForeColor = System.Drawing.Color.White;
            this.lblTotalPopulationPerPurok.Location = new System.Drawing.Point(244, 56);
            this.lblTotalPopulationPerPurok.Name = "lblTotalPopulationPerPurok";
            this.lblTotalPopulationPerPurok.Size = new System.Drawing.Size(15, 32);
            this.lblTotalPopulationPerPurok.TabIndex = 7;
            this.lblTotalPopulationPerPurok.Text = "\r\n";
            this.lblTotalPopulationPerPurok.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPurokName
            // 
            this.lblPurokName.AutoSize = true;
            this.lblPurokName.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPurokName.ForeColor = System.Drawing.Color.White;
            this.lblPurokName.Location = new System.Drawing.Point(6, 13);
            this.lblPurokName.Name = "lblPurokName";
            this.lblPurokName.Size = new System.Drawing.Size(152, 34);
            this.lblPurokName.TabIndex = 6;
            this.lblPurokName.Text = "PUROK NAME";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(6, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 34);
            this.label1.TabIndex = 5;
            this.label1.Text = "TOTAL POPULATION:";
            // 
            // listViewHRGGG
            // 
            this.listViewHRGGG.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewHRGGG.AutoArrange = false;
            this.listViewHRGGG.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.listViewHRGGG.Font = new System.Drawing.Font("Impact", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewHRGGG.FullRowSelect = true;
            this.listViewHRGGG.HideSelection = false;
            this.listViewHRGGG.Location = new System.Drawing.Point(0, 448);
            this.listViewHRGGG.Name = "listViewHRGGG";
            this.listViewHRGGG.Size = new System.Drawing.Size(1280, 149);
            this.listViewHRGGG.TabIndex = 4;
            this.listViewHRGGG.UseCompatibleStateImageBehavior = false;
            // 
            // listViewHRGGGG
            // 
            this.listViewHRGGGG.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewHRGGGG.AutoArrange = false;
            this.listViewHRGGGG.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.listViewHRGGGG.Font = new System.Drawing.Font("Impact", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewHRGGGG.FullRowSelect = true;
            this.listViewHRGGGG.HideSelection = false;
            this.listViewHRGGGG.Location = new System.Drawing.Point(3, 625);
            this.listViewHRGGGG.Name = "listViewHRGGGG";
            this.listViewHRGGGG.Size = new System.Drawing.Size(1280, 149);
            this.listViewHRGGGG.TabIndex = 3;
            this.listViewHRGGGG.UseCompatibleStateImageBehavior = false;
            // 
            // listViewHGGR
            // 
            this.listViewHGGR.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewHGGR.AutoArrange = false;
            this.listViewHGGR.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.listViewHGGR.Font = new System.Drawing.Font("Impact", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewHGGR.FullRowSelect = true;
            this.listViewHGGR.HideSelection = false;
            this.listViewHGGR.Location = new System.Drawing.Point(6, 269);
            this.listViewHGGR.Name = "listViewHGGR";
            this.listViewHGGR.Size = new System.Drawing.Size(1280, 149);
            this.listViewHGGR.TabIndex = 1;
            this.listViewHGGR.UseCompatibleStateImageBehavior = false;
            // 
            // listViewHRG
            // 
            this.listViewHRG.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewHRG.AutoArrange = false;
            this.listViewHRG.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.listViewHRG.Font = new System.Drawing.Font("Impact", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewHRG.FullRowSelect = true;
            this.listViewHRG.HideSelection = false;
            this.listViewHRG.Location = new System.Drawing.Point(6, 100);
            this.listViewHRG.Name = "listViewHRG";
            this.listViewHRG.Size = new System.Drawing.Size(1280, 149);
            this.listViewHRG.TabIndex = 0;
            this.listViewHRG.UseCompatibleStateImageBehavior = false;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(21)))), ((int)(((byte)(98)))));
            this.tabPage2.Controls.Add(this.DGWListofgovernmentassistance);
            this.tabPage2.Controls.Add(this.lbltotalmemberperpuork);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.cbxpurok);
            this.tabPage2.Controls.Add(this.cbxgovernmentassit);
            this.tabPage2.Location = new System.Drawing.Point(4, 35);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1294, 785);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "SUMMARY OF Member of Government Assistance";
            // 
            // DGWListofgovernmentassistance
            // 
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black;
            this.DGWListofgovernmentassistance.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle9;
            this.DGWListofgovernmentassistance.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Impact", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGWListofgovernmentassistance.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.DGWListofgovernmentassistance.ColumnHeadersHeight = 41;
            this.DGWListofgovernmentassistance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DGWListofgovernmentassistance.Location = new System.Drawing.Point(3, 178);
            this.DGWListofgovernmentassistance.Name = "DGWListofgovernmentassistance";
            this.DGWListofgovernmentassistance.ReadOnly = true;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Impact", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGWListofgovernmentassistance.RowHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.DGWListofgovernmentassistance.RowHeadersVisible = false;
            this.DGWListofgovernmentassistance.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Century Gothic", 14F);
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.Black;
            this.DGWListofgovernmentassistance.RowsDefaultCellStyle = dataGridViewCellStyle12;
            this.DGWListofgovernmentassistance.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGWListofgovernmentassistance.Size = new System.Drawing.Size(1288, 566);
            this.DGWListofgovernmentassistance.TabIndex = 8;
            // 
            // lbltotalmemberperpuork
            // 
            this.lbltotalmemberperpuork.AutoSize = true;
            this.lbltotalmemberperpuork.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbltotalmemberperpuork.Location = new System.Drawing.Point(178, 749);
            this.lbltotalmemberperpuork.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbltotalmemberperpuork.Name = "lbltotalmemberperpuork";
            this.lbltotalmemberperpuork.Size = new System.Drawing.Size(0, 34);
            this.lbltotalmemberperpuork.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(2, 749);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(172, 34);
            this.label5.TabIndex = 6;
            this.label5.Text = "Total Member:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Impact", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(281, 130);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(740, 45);
            this.label4.TabIndex = 5;
            this.label4.Text = "List of the Member of Government Assistance";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(5, 67);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(423, 33);
            this.label3.TabIndex = 4;
            this.label3.Text = "Select Government Assistance:\r\n";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(5, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 33);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select Purok:\r\n";
            // 
            // cbxpurok
            // 
            this.cbxpurok.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxpurok.FormattingEnabled = true;
            this.cbxpurok.Location = new System.Drawing.Point(193, 12);
            this.cbxpurok.Name = "cbxpurok";
            this.cbxpurok.Size = new System.Drawing.Size(1098, 41);
            this.cbxpurok.TabIndex = 1;
            this.cbxpurok.SelectedIndexChanged += new System.EventHandler(this.cbxpurok_SelectedIndexChanged);
            // 
            // cbxgovernmentassit
            // 
            this.cbxgovernmentassit.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbxgovernmentassit.FormattingEnabled = true;
            this.cbxgovernmentassit.Items.AddRange(new object[] {
            "Assistance to Individuals in Crisis Situations (AICS)",
            "Kabuhayan Program (DOLE)",
            "Balik Probinsya, Bagong Pag-asa Program (BP2)",
            "Social Pension Program for Indigent Senior Citizens",
            "Unconditional Cash Transfer (UCT)",
            "Sustainable Livelihood Program (SLP)",
            "Pantawid Pamilyang Pilipino Program (4Ps)",
            "ACAP (Abot Kamay ang Pagtulong)",
            "TUPAD (Tulong Panghanapbuhay sa Ating Disadvantaged/Displaced Workers)",
            "Not Applicable"});
            this.cbxgovernmentassit.Location = new System.Drawing.Point(435, 59);
            this.cbxgovernmentassit.Name = "cbxgovernmentassit";
            this.cbxgovernmentassit.Size = new System.Drawing.Size(856, 41);
            this.cbxgovernmentassit.TabIndex = 0;
            this.cbxgovernmentassit.SelectedIndexChanged += new System.EventHandler(this.cbxgovernmentassit_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgw_purok);
            this.groupBox2.Font = new System.Drawing.Font("Impact", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(12, 75);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(348, 837);
            this.groupBox2.TabIndex = 37;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select Purok to Generate Report";
            // 
            // dgw_purok
            // 
            dataGridViewCellStyle13.ForeColor = System.Drawing.Color.Black;
            this.dgw_purok.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle13;
            this.dgw_purok.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Impact", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgw_purok.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle14;
            this.dgw_purok.ColumnHeadersHeight = 41;
            this.dgw_purok.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgw_purok.Location = new System.Drawing.Point(6, 30);
            this.dgw_purok.Name = "dgw_purok";
            this.dgw_purok.ReadOnly = true;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgw_purok.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.dgw_purok.RowHeadersVisible = false;
            this.dgw_purok.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Century Gothic", 14F);
            dataGridViewCellStyle16.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.Black;
            this.dgw_purok.RowsDefaultCellStyle = dataGridViewCellStyle16;
            this.dgw_purok.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgw_purok.Size = new System.Drawing.Size(336, 797);
            this.dgw_purok.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Century Gothic", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(836, 62);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(0, 23);
            this.label11.TabIndex = 36;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 10);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(8, 914);
            this.panel2.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1676, 10);
            this.panel1.TabIndex = 6;
            // 
            // IBIM_SUMMARYOFPOPULATIONBYAGE_HRG_frm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(111)))));
            this.ClientSize = new System.Drawing.Size(1676, 924);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "IBIM_SUMMARYOFPOPULATIONBYAGE_HRG_frm";
            this.Load += new System.EventHandler(this.IBIM_SUMMARYOFPOPULATIONBYAGE_HRG_frm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGWListofgovernmentassistance)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgw_purok)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListView listViewHRG;
        private System.Windows.Forms.ListView listViewHGGR;
        private System.Windows.Forms.ListView listViewHRGGGG;
        private System.Windows.Forms.Label lblTotalPopulationPerPurok;
        private System.Windows.Forms.Label lblPurokName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView listViewHRGGG;
        private System.Windows.Forms.DataGridView dgw_purok;
        private System.Windows.Forms.ComboBox cbxpurok;
        private System.Windows.Forms.ComboBox cbxgovernmentassit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbltotalmemberperpuork;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView DGWListofgovernmentassistance;
    }
}