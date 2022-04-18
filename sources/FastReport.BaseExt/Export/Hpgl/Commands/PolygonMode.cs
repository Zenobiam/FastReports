using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// Polygon Type
    /// </summary>
    public enum PolygonType
    {
        /// <summary>
        /// Polygon buffer is cleared and it is made polygon definition mode.
        /// </summary>
        Cleared = 0,
        /// <summary>
        /// Polygon under definition is closed.
        /// </summary>
        Closed = 1,
        /// <summary>
        /// Polygon under definition is closed and polygon definition mode is canceled.
        /// </summary>
        ClosedAndCanceled = 2
    }

    /// <summary>
    /// PM command
    /// It is made polygon definition mode. PM command is used with 
    /// PA/PR, PU/PD, AA/AR, CI, and CT command, and can define polygon.
    /// </summary>
    public class PolygonMode : CommandBase<float>
    {
        public PolygonMode(PolygonType n) : base()
        {
            Name = "PM";
            Parameters.Add((int)n);
        }
    }
}
