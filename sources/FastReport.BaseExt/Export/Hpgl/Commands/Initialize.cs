using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// IN command
    /// Plotter is changed into initial state.
    /// </summary>
    public class Initialize : CommandBase<float>
    {
        public Initialize() : base()
        {
            Name = "IN";
        }
    }
}
