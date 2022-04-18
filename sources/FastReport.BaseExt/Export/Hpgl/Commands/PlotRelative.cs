using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// PR command
    /// </summary>
    public class PlotRelative : CommandBase<float>
    {
        public PlotRelative(float x, float y) : base()
        {
            Name = "PR";
            Parameters.Add(x);
            Parameters.Add(y);
        }
    }
}
