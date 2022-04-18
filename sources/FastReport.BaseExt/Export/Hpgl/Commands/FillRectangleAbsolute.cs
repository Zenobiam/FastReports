using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// RA command
    /// It is used with FT and PT command and shading of the inside of rectangle which makes
    /// diagonal point coordinates (x, y) specified the present position and here is carried out. 
    /// Pen position after command and pen up / down state will be in state before command.
    /// </summary>
    public class FillRectangleAbsolute : CommandBase<float>
    {
        public FillRectangleAbsolute(float x, float y) : base()
        {
            Name = "RA";
            Parameters.Add(x);
            Parameters.Add(y);
        }
    }
}
