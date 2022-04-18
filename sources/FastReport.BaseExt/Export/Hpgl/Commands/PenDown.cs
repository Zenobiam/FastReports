using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// PD command
    /// </summary>
    public class PenDown : CommandBase<int>
    {
        public PenDown() : base()
        {
            Name = "PD";
        }
    }
}
