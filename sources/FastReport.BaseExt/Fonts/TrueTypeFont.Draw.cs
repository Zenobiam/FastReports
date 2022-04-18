using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace FastReport.Fonts
{
    partial class TrueTypeFont
    {
        /// <summary>
        /// Create outline for group of characters
        /// </summary>
        /// <param name="text">text as array of glyph's indexes</param>
        /// <param name="position">position of text</param>
        /// <param name="size">font size</param>
        /// <returns>text in form of outline vectors</returns>
        public FastGraphicsPath DrawString(ushort[] text, PointF position, float size)
        {
            FastGraphicsPath path = new FastGraphicsPath(FastFillMode.Winding);
            float rsize = (float)this.Header.unitsPerEm / size;

            uint location;
            ushort glyph_size;
            GlyphTableClass.GlyphHeader gheader;

            foreach (ushort idx in text)
            {
                float glyph_width = 0;
                //gheader.xMin = 0;
                //gheader.xMax = 10;
                glyph_size = this.index_to_location.GetGlyph(idx, Header, out location);
                ArrayList composed_indexes = this.glyph_table.CheckGlyph((int)location, (int)glyph_size);
                FastGraphicsPath glyph_path = this.glyph_table.GetGlyph((int)location, glyph_size, rsize, position, this.index_to_location, this.Header, out gheader);
                if (glyph_path.PointCount != 0)
                    path.AddPath(glyph_path, false);

                HorizontalMetrixClass.longHorMetric hm = horizontal_metrix_table[idx];
                //glyph_width = (float)(hm.advanceWidth + hm.lsb) / rsize;
                glyph_width = (float)hm.advanceWidth / rsize;
                position.X += glyph_width;
            }

            return path;
        }

        /// <summary>
        /// Create outline for text string
        /// </summary>
        /// <param name="text">text which will be transformed to outline</param>
        /// <param name="position">position of text</param>
        /// <param name="size">font size in px</param>
        /// <returns>text in form of outline vectors</returns>

        public FastGraphicsPath DrawString(string text, PointF position, float size)
        {
            ushort[] glyphs;
            float[] widhths;
            int len = GetGlyphIndices(text, size, out glyphs, out widhths, false);
            return DrawString(glyphs, position, size);
        }

        /// <inheritdoc/>
        public Dictionary<string, List<FastGraphicsPath>> DrawGSUBTable(int lineHeight, int columnWidth, float fontSize, GlyphSubstitutionClass.LookupTypes[] types)
        {
            Dictionary<string, List<FastGraphicsPath>> result = new Dictionary<string, List<FastGraphicsPath>>();
            foreach (string script in Scripts)
            {
                foreach (string language in GetLanguages(script))
                {
                    foreach (string feature in GetFeatures(script, language))
                    {
                        List<FastGraphicsPath> list = new List<FastGraphicsPath>();
                        PointF point = PointF.Empty;
                        foreach (KeyValuePair<ushort[], ushort[]> sub in gsub_table.GetSubstitutions(script, language, feature, types))
                        {
                            point = PointF.Add(point, new SizeF(0, lineHeight));
                            FastGraphicsPath path1 = DrawString(sub.Key, point, fontSize);
                            FastGraphicsPath path2 = DrawString(sub.Value, PointF.Add(point, new SizeF(columnWidth, 0)), fontSize);
                            path1.AddPath(path2, false);
                            list.Add(path1);
                        }

                        result.Add(
                            (script == string.Empty ? "default" : script).Replace(' ', '_')
                            + "\\"
                            + (language == string.Empty ? "default" : language).Replace(' ', '_')
                            + "\\"
                            + (feature == string.Empty ? "default" : feature).Replace(' ', '_'),
                            list);
                    }
                }
            }
            return result;
        }


    }
}
