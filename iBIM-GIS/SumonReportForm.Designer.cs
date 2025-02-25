
namespace iBIM_GIS
{
    partial class SumonReportForm
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
            this.rv_SumonReport = new Microsoft.Reporting.WinForms.ReportViewer();
            this.label9 = new System.Windows.Forms.Label();
            this.btncloseform = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rv_SumonReport
            // 
            this.rv_SumonReport.Location = new System.Drawing.Point(4, 82);
            this.rv_SumonReport.Name = "rv_SumonReport";
            this.rv_SumonReport.ServerReport.BearerToken = null;
            this.rv_SumonReport.Size = new System.Drawing.Size(1122, 638);
            this.rv_SumonReport.TabIndex = 7;
            this.rv_SumonReport.Load += new System.EventHandler(this.rv_SumonReport_Load);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Century Gothic", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(385, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(317, 38);
            this.label9.TabIndex = 11;
            this.label9.Text = "SUMON INVITATION";
            // 
            // btncloseform
            // 
            this.btncloseform.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btncloseform.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btncloseform.ForeColor = System.Drawing.Color.White;
            this.btncloseform.Location = new System.Drawing.Point(1093, 0);
            this.btncloseform.Name = "btncloseform";
            this.btncloseform.Size = new System.Drawing.Size(38, 32);
            this.btncloseform.TabIndex = 12;
            this.btncloseform.Text = "X";
            this.btncloseform.UseVisualStyleBackColor = true;
            this.btncloseform.Click += new System.EventHandler(this.btncloseform_Click);
            // 
            // SumonReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(21)))), ((int)(((byte)(98)))));
            this.ClientSize = new System.Drawing.Size(1129, 724);
            this.Controls.Add(this.btncloseform);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.rv_SumonReport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SumonReportForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SumonReportForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer rv_SumonReport;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btncloseform;
    }
}