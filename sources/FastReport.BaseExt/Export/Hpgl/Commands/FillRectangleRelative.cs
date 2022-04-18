using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// RR command
    ///  It is used with FT and PT command and shading of the inside of rectangle which makes diagonal point
    ///  relative coordinate (x, y) from the present position and there is carried out. Pen position after command
    ///  and pen up / down state will be in state before command.
    /// </summary>
    public class FillRectangleRelative : CommandBase<float>
    {
        public FillRectangleRelative(float x, float y) : base()
        {
            Name = "RR";
            Parameters.Add(x);
            Parameters.Add(y);
        }
    }
}
