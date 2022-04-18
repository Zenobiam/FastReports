using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// EA command
    /// Rectangle which makes diagonal coordinates (x, y) specified the present position and 
    /// here is plotted. Pen position after command and pen up / down state will be in state before command.
    /// </summary>
    public class EdgeRectangleAbsolute : CommandBase<float>
    {
        public EdgeRectangleAbsolute(float x, float y) : base()
        {
            Name = "EA";
            Parameters.Add(x);
            Parameters.Add(y);
        }
    }
}
