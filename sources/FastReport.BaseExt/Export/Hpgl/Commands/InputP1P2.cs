using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// IP command
    /// Sets origin position of system of coordinates
    /// </summary>
    public class InputP1P2 : CommandBase<float>
    {
        public InputP1P2(PointF p1, PointF p2) : base()
        {
            Name = "IP";
            Parameters.Add(p1.X);
            Parameters.Add(p1.Y);
            Parameters.Add(p2.X);
            Parameters.Add(p2.Y);
        }
    }
}
