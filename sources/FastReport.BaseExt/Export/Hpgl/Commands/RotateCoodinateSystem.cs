using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// RO command
    /// </summary>
    public class RotateCoodinateSystem : CommandBase<float>
    {
        public RotateCoodinateSystem() : base()
        {
            Name = "RO";
        }
    }
}
