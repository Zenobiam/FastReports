using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// EP command
    /// Perimeter of polygon defined as polygon buffer by PM, PA/PR, PU/PD,
    /// AA/AR, CI, and CT command is plotted. Pen position after command and
    /// pen up / down state will be in state before command.
    /// </summary>
    public class EdgePolygon : CommandBase<float>
    {
        public EdgePolygon() : base()
        {
            Name = "EP";
        }
    }
}
