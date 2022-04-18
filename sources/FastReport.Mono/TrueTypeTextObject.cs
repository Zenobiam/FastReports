using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastReport.Utils;
using FastReport.Fonts;
using System.Drawing.Drawing2D;
using System.Drawing;

#if DEBUG
using System.Diagnostics;
#endif


namespace FastReport
{
    /// <summary>
    /// Represents the TrueType Text object that may display one or several text lines.
    /// </summary>
    /// <remarks>
    /// Specify the object's text in the <see cref="TextObject.Text">Text</see> property. 
    /// Text may contain expressions and data items, for example: "Today is [Date]". When report 
    /// is running, all expressions are calculated and replaced with actual values, so the text 
    /// would be "Today is 01.01.2008".
    /// <para/>The symbols used to find expressions in a text are set in the 
    /// <see cref="TextObjectBase.Brackets">Brackets</see> property. You also may disable expressions 
    /// using the <see cref="TextObjectBase.AllowExpressions">AllowExpressions</see> property.
    /// <para/>To format an expression value, use the <see cref="Format"/> property.
    /// </remarks>
    public class TrueTypeTextObject : TextObject
    {
        /// <summary>
        /// Draws a text.
        /// </summary>
        /// <param name="e">Paint event data.</param>
        public override void DrawText(FRPaintEventArgs e)
        {
            string text = Text;
            if (!String.IsNullOrEmpty(text))
            {
                TrueTypeFont font = Config.FontCollection[this.Font];
                GraphicsPath path;

                float sz = Font.Size * 1.3f;

                RectangleF textRect = new RectangleF(
                  (AbsLeft + Padding.Left) * e.ScaleX,
                  (AbsTop + Padding.Top + sz) * e.ScaleY,
                  (Width - Padding.Horizontal) * e.ScaleX,
                  (Height - Padding.Vertical) * e.ScaleY);

                Point pos = new Point( (int) textRect.X, (int) textRect.Y);
                Pen pen = e.Cache.GetPen(this.TextColor, e.ScaleX / 3, DashStyle.Solid);

                Brush textBrush = null;
                if (TextFill is SolidFill)
                    textBrush = e.Cache.GetBrush((TextFill as SolidFill).Color);
                else
                    textBrush = TextFill.CreateBrush(textRect);

#if false
                foreach (char ch in text)
                {
                    int width;
                    path = font.GetGlyphPath(ch, (int) (Font.Size * e.ScaleX), pos,  out width);
                    pos.X += width;
                    //e.Graphics.DrawPath(pen, path);
                    e.Graphics.FillPath(textBrush, path);
                }
#else
                path = font.DrawString(text, pos, (int)(sz * e.ScaleX));
                e.Graphics.FillPath(textBrush, path);
#endif
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TrueTypeTextObject()
        {
#if DEBUG
            Debug.WriteLine("Constructor called!\n");
#endif
        }
    }
}
