using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using FastReport.Export.TTF;
using FastReport.Fonts;

namespace FastReport.Export.Pdf
{
    public partial class PDFExport : ExportBase
    {
        private List<ExportTTFFont> fonts;
        private List<ExportTTFFont> pageFonts;
        private bool skipCurrentFont;

        private void AppendFont(StringBuilder Result, int fontNumber, float fontSize, Color fontColor)
        {
            if (!textInCurves)
            {
                ExportTTFFont pdffont = pageFonts[fontNumber];
                Result.Append(pdffont.Name).Append(" ").Append(FloatToString(fontSize)).AppendLine(" Tf");
                GetPDFFillColor(fontColor, Result);
            }
        }

        private void AppendFontAcroForm(StringBuilder Result, int fontNumber, float fontSize, Color fontColor)
        {
            ExportTTFFont pdffont = fonts[fontNumber];
            Result.Append(pdffont.Name).Append(" ").Append(FloatToString(fontSize)).AppendLine(" Tf");
            GetPDFFillColor(fontColor, Result);
        }

        private int GetObjFontNumber(Font font)
        {
            //if (!FTextInCurves)
            //{
                int i;
                for (i = 0; i < pageFonts.Count; i++)
                    if (FontEquals(font, pageFonts[i].SourceFont))
                        break;
                if (i < pageFonts.Count)
                    return i;
                else
                {
                    pageFonts.Add(GetGlobalFont(font));
                    return pageFonts.Count - 1;
                }
            //}
            //else
             //   return 0;
        }

        private int GetObjFontNumberAcroForm(Font font)
        {
            int i;
            for (i = 0; i < fonts.Count; i++)
                if (FontEquals(font, fonts[i].SourceFont))
                    break;
            if (i < fonts.Count)
                return i;
            else
            {
                GetGlobalFont(font);
                return fonts.Count - 1;
            }
        }

        private ExportTTFFont GetGlobalFont(Font font)
        {
            int i;
            for (i = 0; i < fonts.Count; i++)
                if (FontEquals(font, fonts[i].SourceFont))
                    break;
            if (i < fonts.Count)
                return fonts[i];
            else
            {
                ExportTTFFont fontitem = null;
                try
                {
                    fontitem = new ExportTTFFont(font);
                    fontitem.FillOutlineTextMetrix();
                    fontitem.FillEmulate();
                    //// 20180604
                    //bool his_italic = fontitem.TextMetric.otmItalicAngle != 0;
                    //bool his_bold = fontitem.TextMetric.otmTextMetrics.tmWeight > 550;
                    //if (font.Bold != his_bold)
                    //  fontitem.NeedSimulateBold = true;
                    //if (font.Italic != his_italic)
                    //  fontitem.NeedSimulateItalic = true;
                    fonts.Add(fontitem);
                    fontitem.Name = "/F" + (fonts.Count - 1).ToString();
                }
                catch(Exception ex)
                {
                    // TODO: This exeption conflicts with caller's finally statement. Fix caller.
                    throw ex;
                }
                return fontitem;
            }
        }

        private string GetToUnicode(ExportTTFFont pdfFont)
        {
            StringBuilder toUnicode = new StringBuilder(2048);
            toUnicode.AppendLine("/CIDInit /ProcSet findresource begin");
            toUnicode.AppendLine("12 dict begin");
            toUnicode.AppendLine("begincmap");
            toUnicode.AppendLine("/CIDSystemInfo");
            toUnicode.AppendLine("<< /Registry (Adobe)");
            toUnicode.AppendLine("/Ordering (UCS)");
            toUnicode.AppendLine("/Ordering (Identity)");
            toUnicode.AppendLine("/Supplement 0");
            toUnicode.AppendLine(">> def");
            toUnicode.Append("/CMapName /").Append(pdfFont.GetEnglishFontName().Replace(',', '+')).AppendLine(" def");
            toUnicode.AppendLine("/CMapType 2 def");
            toUnicode.AppendLine("1 begincodespacerange");
            toUnicode.AppendLine("<0000> <FFFF>");
            toUnicode.AppendLine("endcodespacerange");

            int charCount = 0;
            foreach (GlyphChar glyphChar in pdfFont.UsedGlyphChars)
            {
                if (glyphChar.GlyphType == GlyphType.Character ||
                    glyphChar.GlyphType == GlyphType.None)
                    charCount++;
            }
            int stringCount = 0;

            foreach (GlyphChar glyphChar in pdfFont.UsedGlyphChars)
            {
                if (glyphChar.GlyphType == GlyphType.String)
                    stringCount++;
            }

            if (charCount > 0)
            {
                toUnicode.Append(charCount.ToString()).AppendLine(" beginbfchar");
                foreach (GlyphChar glyphChar in pdfFont.UsedGlyphChars)
                {
                    if (glyphChar.GlyphType == GlyphType.Character)
                    {
                        toUnicode.Append("<").Append(glyphChar.Glyph.ToString("X4")).Append("> ");
                        toUnicode.Append("<").Append(((ushort)glyphChar.Character).ToString("X4")).AppendLine(">");
                    }
                    else if (glyphChar.GlyphType == GlyphType.None)
                    {
                        toUnicode.Append("<").Append(glyphChar.Glyph.ToString("X4")).Append("> ");
                        toUnicode.AppendLine("<0000>");
                    }
                }
                toUnicode.AppendLine("endbfchar");
            }

            if (stringCount > 0)
            {
                toUnicode.Append(stringCount.ToString()).AppendLine(" beginbfrange");
                foreach (GlyphChar glyphChar in pdfFont.UsedGlyphChars)
                {
                    if (glyphChar.GlyphType == GlyphType.String)
                    {

                        toUnicode.Append("<").Append(glyphChar.Glyph.ToString("X4")).Append("> ");
                        toUnicode.Append("<").Append(glyphChar.Glyph.ToString("X4")).Append("> ");
                        toUnicode.Append("[<");
                        foreach (char c in glyphChar.String)
                            toUnicode.Append(((ushort)c).ToString("X4"));
                        toUnicode.AppendLine(">]");
                    }
                }
                toUnicode.AppendLine("endbfrange");
            }

            toUnicode.AppendLine("endcmap");
            toUnicode.AppendLine("CMapName currentdict /CMap defineresource pop");
            toUnicode.AppendLine("end");
            toUnicode.AppendLine("end");
            return toUnicode.ToString();
        }

        private void WriteFont(ExportTTFFont pdfFont)
        {
            long fontFileId = 0;
            string fontName = pdfFont.GetEnglishFontName();

            // embedded font 
            if (embeddingFonts)
            {
              if (interactiveForms && pdfFont.Editable)
              {
                if (interactiveFormsFontSetPattern.Length == 0)
                {
                    pdfFont.RemapString(" ", false);
                    for (int i = 0; i < 1024; i++)
                    {
                        StringBuilder sb = new StringBuilder(64);
                        for (int j = 0; j < 64; j++)
                            sb.Append((char)(64 * i + j));
                        pdfFont.RemapString(sb.ToString(), false);
                    }
                }
                else
                {
                    pdfFont.SetPattern(interactiveFormsFontSetPattern, false);
                }
              }

              byte[] fontfile = pdfFont.GetFontData(/*!(interactiveForms && thereIsEditableElement)*/ true);
              if (fontfile != null)
              {
                skipCurrentFont = false;
                fontFileId = UpdateXRef();
                WriteLn(pdf, ObjNumber(fontFileId));
                MemoryStream fontFileStream = new MemoryStream();
                fontFileStream.Write(fontfile, 0, fontfile.Length);
                WritePDFStream(pdf, fontFileStream, fontFileId, compressed, encrypted, true, true);
              }
              else
              {
                // If font cannot be embedded by some reason, for example license rights, then disable embedding
                skipCurrentFont = true;
              }
            }

            // descriptor
            long descriptorId = UpdateXRef();
            WriteLn(pdf, ObjNumber(descriptorId));
            WriteLn(pdf, "<<");
            WriteLn(pdf, "/Type /FontDescriptor");
            WriteLn(pdf, "/FontName /" + fontName);
            //WriteLn(pdf, "/FontFamily /" + fontName);
            WriteLn(pdf, "/Flags 32");
            WriteLn(pdf, "/FontBBox [" + pdfFont.TextMetric.otmrcFontBox.left.ToString() + " " +
                pdfFont.TextMetric.otmrcFontBox.bottom.ToString() + " " +
                pdfFont.TextMetric.otmrcFontBox.right.ToString() + " " +
                pdfFont.TextMetric.otmrcFontBox.top.ToString() + " ]");
            //WriteLn(pdf, "/Style << /Panose <" + pdfFont.GetPANOSE() + "> >>"); 
            int ItalicAnggle = pdfFont.TextMetric.otmItalicAngle;
            if (pdfFont.NeedSimulateItalic && ItalicAnggle == 0)
            WriteLn(pdf, "/ItalicAngle " + 16);
              else
            WriteLn(pdf, "/ItalicAngle " + pdfFont.TextMetric.otmItalicAngle.ToString());
            WriteLn(pdf, "/Ascent " + pdfFont.TextMetric.otmAscent.ToString());
            WriteLn(pdf, "/Descent " + pdfFont.TextMetric.otmDescent.ToString());
            WriteLn(pdf, "/Leading " + pdfFont.TextMetric.otmTextMetrics.tmInternalLeading.ToString());
            WriteLn(pdf, "/CapHeight " + pdfFont.TextMetric.otmTextMetrics.tmHeight.ToString());
            WriteLn(pdf, "/StemV " + (50 + Math.Round(Math.Sqrt((float)pdfFont.TextMetric.otmTextMetrics.tmWeight / 65f))).ToString());
            WriteLn(pdf, "/AvgWidth " + pdfFont.TextMetric.otmTextMetrics.tmAveCharWidth.ToString());
            WriteLn(pdf, "/MxWidth " + pdfFont.TextMetric.otmTextMetrics.tmMaxCharWidth.ToString());
            WriteLn(pdf, "/MissingWidth " + pdfFont.TextMetric.otmTextMetrics.tmAveCharWidth.ToString());
            if (embeddingFonts && !skipCurrentFont)
                WriteLn(pdf, "/FontFile2 " + ObjNumberRef(fontFileId));
            WriteLn(pdf, ">>");
            WriteLn(pdf, "endobj");

            // ToUnicode
            long toUnicodeId = UpdateXRef();
            WriteLn(pdf, ObjNumber(toUnicodeId));
            MemoryStream tounicodeStream = new MemoryStream();
            Write(tounicodeStream, GetToUnicode(pdfFont));
            WritePDFStream(pdf, tounicodeStream, toUnicodeId, compressed, encrypted, true, true);

            //CIDSystemInfo
            long cIDSystemInfoId = UpdateXRef();
            WriteLn(pdf, ObjNumber(cIDSystemInfoId));
            WriteLn(pdf, "<<");
            WriteLn(pdf, "/Registry (Adobe) /Ordering (Identity) /Supplement 0");
            WriteLn(pdf, ">>");
            WriteLn(pdf, "endobj");

            //DescendantFonts
            long descendantFontId = UpdateXRef();
            WriteLn(pdf, ObjNumber(descendantFontId));
            WriteLn(pdf, "<<");
            WriteLn(pdf, "/Type /Font");
            WriteLn(pdf, "/Subtype /CIDFontType2");
            WriteLn(pdf, "/BaseFont /" + fontName);
            WriteLn(pdf, "/CIDToGIDMap /Identity");
            WriteLn(pdf, "/CIDSystemInfo " + ObjNumberRef(cIDSystemInfoId));
            WriteLn(pdf, "/FontDescriptor " + ObjNumberRef(descriptorId));
            Write(pdf, "/W [ ");
            foreach(GlyphChar glyphChar in pdfFont.UsedGlyphChars)
                Write(pdf, glyphChar.Glyph.ToString() + " [" + FloatToString(glyphChar.Width) + "] ");
            WriteLn(pdf, "]");
            WriteLn(pdf, ">>");
            WriteLn(pdf, "endobj");

            // main
            xRef[(int)(pdfFont.Reference - 1)] = pdf.Position;
            WriteLn(pdf, ObjNumber(pdfFont.Reference));
            WriteLn(pdf, "<<");
            WriteLn(pdf, "/Type /Font");
            WriteLn(pdf, "/Subtype /Type0");
            WriteLn(pdf, "/BaseFont /" + fontName);
            WriteLn(pdf, "/Encoding /Identity-H");
            WriteLn(pdf, "/DescendantFonts [" + ObjNumberRef(descendantFontId) + "]");
            WriteLn(pdf, "/ToUnicode " + ObjNumberRef(toUnicodeId));
            WriteLn(pdf, ">>");
            WriteLn(pdf, "endobj");
        }
    }
}
