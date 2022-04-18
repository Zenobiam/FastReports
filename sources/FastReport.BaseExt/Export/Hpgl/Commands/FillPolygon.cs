using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// FP command
    /// Shading of the inside of polygon defined as polygon buffer by PM, PA/PR,
    /// PU/PD, AA/AR, CI, and CT command is carried out. Pen position after command 
    /// and pen up / down state will be in state before command.
    /// </summary>
    public class FillPolygon : CommandBase<float>
    {
        public FillPolygon() : base()
        {
            Name = "FP";
        }
    }
}
