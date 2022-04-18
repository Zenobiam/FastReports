using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// SC command
    /// </summary>
    public class Scale : CommandBase<float>
    {
        public Scale(int xMin, int xMax, int yMin, int yMax) : base()
        {
            Name = "SC";
            Parameters.Add(xMin);
            Parameters.Add(xMax);
            Parameters.Add(yMin);
            Parameters.Add(yMax);
        }
    }
}
