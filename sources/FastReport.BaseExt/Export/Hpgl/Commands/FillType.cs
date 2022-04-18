using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl.Commands
{
    /// <summary>
    /// Model of shading 
    /// </summary>
    public enum ShadingModel
    {
        /// <summary>
        /// Painting out interactive at space specified by PT command (FT command interval and angle are ignored)
        /// </summary>
        Interactive = 1,
        /// <summary>
        /// It is painting out (FT command space and angle are ignored) of the single direction at space specified by PT command.
        /// </summary>
        SingleDirection = 2,
        /// <summary>
        /// Hatching which is the single direction at space and angle which were specified by FT command
        /// </summary>
        Hatching = 3,
        /// <summary>
        /// It is crossing hatching at space and angle which were specified by FT command.
        /// </summary>
        CrossingHatching = 4,
        /// <summary>
        /// None (solid)
        /// </summary>
        None
    }
    /// <summary>
    /// FT command
    /// It is used together with FP, RA, RR, and WG command, and model of shading (painting out and hatching) is specified.
    /// </summary>
    public class FillType : CommandBase<float>
    {
        public FillType(ShadingModel shading) : base()
        {
            Name = "FT";
            if (shading != ShadingModel.None)
                Parameters.Add((int)shading);
        }

        public FillType(ShadingModel shading, int space) : this(shading)
        {
            Parameters.Add(space);
        }

        public FillType(ShadingModel shading, int space, int angle) : this(shading, space)
        {
            Parameters.Add(angle);
        }
    }
}
