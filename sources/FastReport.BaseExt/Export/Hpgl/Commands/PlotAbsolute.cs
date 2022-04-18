using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// PA command
    /// </summary>
    public class PlotAbsolute : CommandBase<float>
    {
        public PlotAbsolute(float x, float y) : base()
        {
            Name = "PA";
            Parameters.Add(x);
            Parameters.Add(y);
        }
    }
}
