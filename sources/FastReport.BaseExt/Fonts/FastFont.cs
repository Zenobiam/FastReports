using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Fonts
{
#pragma warning disable CS1591  // Missing XML comment for publicly visible type or member

  /// <summary>
  /// Font descriptor
  /// </summary>
  public class FastFont
    {
        private string originalFontName;
        private bool bold;
        private bool italic;

        public string OriginalFontName
        {
            get
            {
                return originalFontName;
            }
            set
            {
                originalFontName = value;
            }
        }
        public bool Bold
        {
            get
            {
                return bold;
            }
            set
            {
                bold = value;
            }
        }
        public bool Italic
        {
            get
            {
                return italic;
            }
            set
            {
                italic = value;
            }
        }

        public string FastName
        {
          get
          {
            return OriginalFontName + (Bold ? "-B" : string.Empty) + (Italic ? "-I" : string.Empty);
          }
        }
        public FastFont(string name, bool bold, bool italic)
        {
            OriginalFontName = name;
            Bold = bold;
            Italic = italic;
        }
    }
}
