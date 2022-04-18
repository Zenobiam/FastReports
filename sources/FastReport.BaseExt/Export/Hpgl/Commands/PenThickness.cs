using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// PT command
    /// It is used together with FP, FT, RR, RA, and WG command, and space (unit mm) of painting out
    /// is specified between 0.1 and 5.0 in accordance with thickness of pen. Initial value is 0.3mm.
    /// </summary>
    public class PenThickness : CommandBase<float>
    {
        public PenThickness(float thickness) : base()
        {
            Name = "PT";
            Parameters.Add(thickness);
        }
    }
}
