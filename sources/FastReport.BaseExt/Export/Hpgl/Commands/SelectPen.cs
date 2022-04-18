using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// SP command
    /// </summary>
    public class SelectPen : CommandBase<int>
    {
        public SelectPen() : base()
        {
            Name = "SP";
            Parameters.Add(0);
        }
    }
}
