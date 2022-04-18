using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FastReport.Map
{
    partial class ShapePoint
    {
        private void DrawDesign(FRPaintEventArgs e, ref Brush brush, ref float size)
        {
            // display the selection in the designer
            Report report = Map.Report;
            if (report != null && report.IsDesigning && report.Designer != null &&
              report.Designer.SelectedObjects != null)
            {
                if (report.Designer.SelectedObjects.Contains(this))
                    brush = e.Cache.GetBrush(Color.Orange);
            }

            // display the selection in the preview
            if (Map.HotPoint == this)
            {
                size *= 1.5f;
            }
        }
    }
}
