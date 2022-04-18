using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{

    /// <summary>
    /// CI command
    /// </summary>
    public class Circle : CommandBase<float>
    {
        public Circle(int radius, float resolution) : base()
        {
            Name = "CI";
            Parameters.Add(radius);
            Parameters.Add(resolution);
        }
    }
}
