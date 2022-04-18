using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace FastReport.Map
{
    partial class ShapePolygon
    {
        private void DrawDesign(FRPaintEventArgs e, ref Brush brush, ref bool disposeBrush)
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
                brush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.White, (brush as SolidBrush).Color);
                disposeBrush = true;
            }
        }
    }
}
