using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;

namespace FastReport.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ProgressForm : ScaledSupportingForm
    {
        private Report report;
        private bool aborted;
        private long ticks = 0;
        private bool closed = false;

        /// <summary>
        /// Gets Aborted state
        /// </summary>
        public bool Aborted
        {
            get { return aborted; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (aborted && ! closed)
            {
                Close();
            }
            if (report != null)
                report.Abort();
            aborted = true;
        }

        private void ProgressForm_Paint(object sender, PaintEventArgs e)
        {
            using (Pen p = new Pen(Color.Gray, 2))
            {
                p.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                e.Graphics.DrawRectangle(p, DisplayRectangle);
            }
        }

        private void ProgressForm_Shown(object sender, EventArgs e)
        {
            lblProgress.Width = Width - lblProgress.Left * 2;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowProgressMessage(string message)
        {
            lblProgress.Text = message;
            lblProgress.Refresh();
            if (ticks++ % 500 == 0)
                Config.DoEvent();
        }

        /// <summary>
        ///
        /// </summary>
        public ProgressForm(Report report)
        {
            aborted = false;
            this.report = report;
            InitializeComponent();
            btnCancel.Text = Res.Get("Buttons,Cancel");
            try
            {
                Owner = Config.MainForm as Form;
            }
            catch { Owner = null; }
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += ProgressForm_FormClosing;
            Scale();
        }

        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            closed = true;
        }

        private void ProgressForm_Load(object sender, System.EventArgs e)
        {
            CenterToScreen();
        }
    }
}