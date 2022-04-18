using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Export.XAML
{
    partial class XAMLExport
    {
        /// <inheritdoc/>
        public override bool ShowDialog()
        {
            using (FastReport.Forms.XAMLExportForm dialog = new FastReport.Forms.XAMLExportForm())
            {
                return dialog.ShowDialog(this);
            }
        }
    }
}
