using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// CT command
    /// </summary>
    public class ChordTolerance : CommandBase<int>
    {
        public ChordTolerance(int n) : base()
        {
            Name = "CT";
            Parameters.Add(n);
        }
    }
}
