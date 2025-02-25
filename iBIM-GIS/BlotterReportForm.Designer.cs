
namespace iBIM_GIS
{
    partial class BlotterReportForm
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
            this.reportViewerblotter = new Microsoft.Reporting.WinForms.ReportViewer();
            this.label9 = new System.Windows.Forms.Label();
            this.btncloseform = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // reportViewerblotter
            // 
            this.reportViewerblotter.Location = new System.Drawing.Point(3, 80);
            this.reportViewerblotter.Name = "reportViewerblotter";
            this.reportViewerblotter.ServerReport.BearerToken = null;
            this.reportViewerblotter.Size = new System.Drawing.Size(1087, 665);
            this.reportViewerblotter.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Century Gothic", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(379, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(261, 38);
            this.label9.TabIndex = 12;
            this.label9.Text = "BLOTTER REPORT";
            // 
            // btncloseform
            // 
            this.btncloseform.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btncloseform.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btncloseform.ForeColor = System.Drawing.Color.White;
            this.btncloseform.Location = new System.Drawing.Point(1056, -1);
            this.btncloseform.Name = "btncloseform";
            this.btncloseform.Size = new System.Drawing.Size(38, 32);
            this.btncloseform.TabIndex = 13;
            this.btncloseform.Text = "X";
            this.btncloseform.UseVisualStyleBackColor = true;
            this.btncloseform.Click += new System.EventHandler(this.btncloseform_Click);
            // 
            // BlotterReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(21)))), ((int)(((byte)(98)))));
            this.ClientSize = new System.Drawing.Size(1093, 749);
            this.Controls.Add(this.btncloseform);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.reportViewerblotter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BlotterReportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BlotterReportForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewerblotter;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btncloseform;
    }
}