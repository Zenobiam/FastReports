using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;

namespace FastReport
{
    partial class CheckBoxObject
    {
        #region Public Methods
        /// <inheritdoc/>
        public override SizeF GetPreferredSize()
        {
            if ((Page as ReportPage).IsImperialUnitsUsed)
                return new SizeF(Units.Inches * 0.2f, Units.Inches * 0.2f);
            return new SizeF(Units.Millimeters * 5, Units.Millimeters * 5);
        }

        /// <inheritdoc/>
        public override SmartTagBase GetSmartTag()
        {
            return new CheckBoxSmartTag(this);
        }

        /// <inheritdoc/>
        public override void OnMouseDown(MouseEventArgs e)
        {

            if (Editable && !Config.WebMode)
            {
                Checked = !Checked;
                Report report = Report;
                if (report != null)
                {
                    Preview.PreviewControl preview = report.Preview;
                    if (preview != null)
                    {
                        // update current page in a cache
                        report.PreparedPages.ModifyPage(Report.Preview.PageNo - 1, Page as ReportPage);
                        // redraw the preview
                        preview.Refresh();
                    }
                }
            }
            else base.OnMouseDown(e);
        }
        #endregion
    }
}