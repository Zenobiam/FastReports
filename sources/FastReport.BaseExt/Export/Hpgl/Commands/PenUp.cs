using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// PU command
    /// </summary>
    public class PenUp : CommandBase<int>
    {
        public PenUp() : base()
        {
            Name = "PU";
        }
    }
}
