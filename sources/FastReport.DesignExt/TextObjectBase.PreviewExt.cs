using System;
using System.Collections.Generic;
using System.Drawing;
using FastReport.Utils;

namespace FastReport
{
  partial class TextObjectBase : ISearchable
  {
    #region ISearchable Members

    /// <inheritdoc/>
    public CharacterRange[] SearchText(string text, bool matchCase, bool wholeWord)
    {
      List<CharacterRange> ranges = new List<CharacterRange>();
      string nonWordChars = " `-=[];',./~!@#$%^&*()+{}:\"<>?\\|\r\n\t";
      int startIndex = 0;

      while (startIndex < Text.Length)
      {
        int i = Text.IndexOf(text, startIndex, matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
        if (i >= 0)
        {
          bool skip = false;
          if (wholeWord)
          {
            if (i > 0 && nonWordChars.IndexOf(Text[i - 1]) == -1)
              skip = true;
            if (i + text.Length < Text.Length && nonWordChars.IndexOf(Text[i + text.Length]) == -1)
              skip = true;
          }
          if (!skip)
            ranges.Add(new CharacterRange(i, text.Length));
          startIndex = i + text.Length;
        }
        else
          break;
      }

      if (ranges.Count > 0)
        return new CharacterRange[] { ranges[0] };
      return null;
    }

    /// <inheritdoc/>
    public virtual void DrawSearchHighlight(FRPaintEventArgs e, CharacterRange range)
    {
      RectangleF rangeRect = new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY,
        Width * e.ScaleX, Height * e.ScaleY);

      using (Brush brush = new SolidBrush(Color.FromArgb(128, SystemColors.Highlight)))
      {
        e.Graphics.FillRectangle(brush, rangeRect);
      }
    }

    #endregion
  }
}
