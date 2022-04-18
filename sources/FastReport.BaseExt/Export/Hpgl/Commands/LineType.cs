using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// Line Type
    /// </summary>
    public enum LinePattern
    {
        /// <summary>
        /// Point is plotted at specifying point
        /// </summary>
        Point = 0,
        /// <summary>
        /// Dotted line of point
        /// </summary>
        Dotted = 1,
        /// <summary>
        /// Short dotted line
        /// </summary>
        ShortDotted = 2,
        /// <summary>
        /// Long dotted line
        /// </summary>
        LongDotted = 3,
        /// <summary>
        /// Short dashed line
        /// </summary>
        ShortDashed = 4,
        /// <summary>
        /// Long dashed line
        /// </summary>
        LongDashed = 5,
        /// <summary>
        /// Two-point phantom line
        /// </summary>
        TwoPointPhantom = 6,
        /// <summary>
        /// No any patterns needed
        /// </summary>
        None
    }

    /// <summary>
    /// LT command
    /// </summary>
    public class LineType : CommandBase<int>
    {
        public LineType(LinePattern pattern) : base()
        {
            Name = "LT";
            if (pattern != LinePattern.None)
                Parameters.Add((int)pattern);
        }
        public LineType(LinePattern pattern, int patternLength) : this(pattern)
        {
            if (pattern != LinePattern.None)
                Parameters.Add(patternLength);
        }
    }
}
