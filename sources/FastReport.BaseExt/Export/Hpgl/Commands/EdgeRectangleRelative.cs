using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// ER command
    /// Rectangle which makes diagonal relative coordinate (x, y) from the present position 
    /// and the present position is plotted. Pen position after command and pen up / down state
    /// will be in state before command.
    /// </summary>
    public class EdgeRectangleRelative : CommandBase<float>
    {
        public EdgeRectangleRelative(float x, float y) : base()
        {
            Name = "ER";
            Parameters.Add(x);
            Parameters.Add(y);
        }
    }
}
