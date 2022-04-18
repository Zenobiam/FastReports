using FastReport.Design;
using FastReport.Forms;
using FastReport.Utils;
using System.IO;

namespace FastReport.Wizards
{
    internal class FastM1nesweeperWizard : EasterEggWizard
    {
        #region Private Fields

        private int bombs;
        private int columns;
        private int rows;

        #endregion Private Fields

        #region Public Methods

        public override bool Run(Designer designer)
        {
            using (Stream s = ResourceLoader.GetStream("Games.FastM1nesweeper.frx"))
            {
                if (LoadStreamToDesigner(s, designer))
                    return true;
            }

            return false;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void ProcessPages(Report report)
        {
            base.ProcessPages(report);

            TextObject tBombs = report.FindObject("tBombs") as TextObject;
            if (tBombs != null)
                tBombs.Text = bombs.ToString();

            TextObject tRows = report.FindObject("tRows") as TextObject;
            if (tRows != null)
                tRows.Text = rows.ToString();

            TextObject tColumns = report.FindObject("tColumns") as TextObject;
            if (tColumns != null)
                tColumns.Text = columns.ToString();
        }

        protected override bool ShowDialog()
        {
            FastM1nesweeperForm form = new FastM1nesweeperForm();
            bool result = form.ShowDialog() == System.Windows.Forms.DialogResult.OK;
            bombs = form.Bombs;
            columns = form.Columns;
            rows = form.Rows;
            return result;
        }

        #endregion Protected Methods
    }
}