using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using System;

namespace FastReport
{
  partial class TextObject
  {
    #region ISearchable Members

    /// <inheritdoc/>
    public override void DrawSearchHighlight(FRPaintEventArgs e, CharacterRange range)
    {
      if (Angle == 0 && FontWidthRatio == 1 && HorzAlign != HorzAlign.Justify)
      {
        IGraphics g = e.Graphics;
        Font font = e.Cache.GetFont(Font.Name, Font.Size * e.ScaleX * 96f / DrawUtils.ScreenDpi, Font.Style);
        StringFormat format = GetStringFormat(e.Cache, 0);

        RectangleF textRect = new RectangleF(
          (AbsLeft + Padding.Left) * e.ScaleX,
          (AbsTop + Padding.Top) * e.ScaleY,
          (Width - Padding.Horizontal) * e.ScaleX,
          (Height - Padding.Vertical) * e.ScaleY);

        RectangleF rangeRect;
        if (Angle == 0 && FontWidthRatio == 1 && HorzAlign != HorzAlign.Justify)
        {
          format.SetMeasurableCharacterRanges(new CharacterRange[] { range });
          Region[] regions = g.MeasureCharacterRanges(Text, font, textRect, format);
          rangeRect = regions[0].GetBounds(g.Graphics);
          regions[0].Dispose();
                    regions[0] = null;
        }
        else
          rangeRect = new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX, Height * e.ScaleY);

        using (Brush brush = new SolidBrush(Color.FromArgb(128, SystemColors.Highlight)))
        {
          g.FillRectangle(brush, rangeRect);
        }
      }
      else
        base.DrawSearchHighlight(e, range);  
    }

     
        #endregion
    }
}
