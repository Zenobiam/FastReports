using FastReport.Export.Hpgl.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FastReport.Export.Hpgl
{
    public static class Extensions
    {
        public static void SelectPen(this List<CommandBase> commands)
        {
            commands.Add(new SelectPen());
        }
        public static void PenUp(this List<CommandBase> commands)
        {
            commands.Add(new PenUp());
        }
        public static void PlotAbsolute(this List<CommandBase> commands, float x, float y)
        {
            commands.Add(new PlotAbsolute(x, y));
        }
        public static void PlotRelative(this List<CommandBase> commands, float x, float y)
        {
            commands.Add(new PlotRelative(x, y));
        }
        public static void PenDown(this List<CommandBase> commands)
        {
            commands.Add(new PenDown());
        }

        /// <summary>
        /// PT command
        /// It is used together with FP, FT, RR, RA, and WG command, and space (unit mm) of painting out
        /// is specified between 0.1 and 5.0 in accordance with thickness of pen. Initial value is 0.3mm.
        /// </summary>
        public static void PenThickness(this List<CommandBase> commands, float thickness)
        {
            float t = (thickness > 5.0f) ? 5.0f : ((thickness < 0.1f) ? 0.1f : thickness);
            commands.Add(new PenThickness(t));
        }
        public static void LineType(this List<CommandBase> commands, LinePattern pattern)
        {
            commands.Add(new LineType(pattern));
        }
        public static void Initialize(this List<CommandBase> commands)
        {
            commands.Add(new Initialize());
        }
        public static void InputP1P2(this List<CommandBase> commands, PointF p1, PointF p2)
        {
            commands.Add(new InputP1P2(p1, p2));
        }
        public static void Scale(this List<CommandBase> commands, int xMin, int xMax, int yMin, int yMax)
        {
            commands.Add(new Scale(xMin, xMax, yMin, yMax));
        }
        public static void EdgePolygon(this List<CommandBase> commands)
        {
            commands.Add(new EdgePolygon());
        }
        public static void PolygonMode(this List<CommandBase> commands, PolygonType type)
        {
            commands.Add(new PolygonMode(type));
        }
        public static void FillType(this List<CommandBase> commands, ShadingModel shading)
        {
            commands.Add(new FillType(shading));
        }
        public static void FillPolygon(this List<CommandBase> commands)
        {
            commands.Add(new FillPolygon());
        }
    }
}
