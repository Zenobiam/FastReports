using FastReport.Table;
using FastReport.Utils;
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FastReport.Export.OoXML
{
    /// <summary>
    ///  Picture container
    /// </summary>
    internal class XPS_Picture : OoXMLBase
    {
        #region "Class overrides"
        public override string RelationType { get { return "http://schemas.microsoft.com/xps/2005/06/required-resource"; } }
        public override string ContentType { get { return null; } } //"application/vnd.openxmlformats-officedocument.presentationml.slide+xml"; } }
        public override string FileName { get { return imageFileName; } }
        #endregion

        #region "Private fields"
        private ReportComponentBase picture;
        private string imageFileName;
//        private XPSExport FExport;
        #endregion

        internal string SaveImage(XPSExport export, ReportComponentBase obj, bool ClearBackground)
        {
            export.picturesCount++;
            imageFileName = "/Resources/" + obj.Name + "." + export.picturesCount + ".png";

            using (System.Drawing.Image image = new System.Drawing.Bitmap((int)Math.Round(obj.Width), (int)Math.Round(obj.Height)))
            {
                using (Graphics g = Graphics.FromImage(image))
                using (GraphicCache cache = new GraphicCache())
                {
                    g.TranslateTransform(-obj.AbsLeft, -obj.AbsTop);
                    if (ClearBackground)
                    {
                        g.Clear(Color.Transparent);
                    }
                    obj.Draw(new FRPaintEventArgs(g, 1, 1, cache));
                    MemoryStream ms = new MemoryStream();
                    (image as Bitmap).SetResolution(96, 96);
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    export.Zip.AddStream(ExportUtils.TruncLeadSlash(imageFileName), ms);
                }
            }
            return imageFileName;
        }

        public XPS_Picture(ReportComponentBase obj)
        {
            picture = obj;
        }
    }

    /// <summary>
    ///  List of all document fonts
    /// </summary>
    internal class XPS_FontList : IDisposable
    {
        private Hashtable fonthash;

        internal XPS_Font AddFont(XPSExport export, Font f, out bool FirstOccurrence)
        {
            int HashCode;
            XPS_Font font;

            FontStyle style = f.Style & (FontStyle.Bold | FontStyle.Italic);
            HashCode = f.Name.GetHashCode() + style.GetHashCode();

            if (fonthash.ContainsKey(HashCode) == false)
            {
                font = new XPS_Font(f);
                fonthash.Add(HashCode, font);
                FirstOccurrence = true;
            }
            else
            {
                font = fonthash[HashCode] as XPS_Font;
                font.SourceFont = f;
                FirstOccurrence = false;
            }

            return font;
        }

        internal XPS_FontList()
        {
            fonthash = new Hashtable();
        }

        internal void Export(XPSExport export)
        {
            foreach (XPS_Font font in fonthash.Values)
            {
                font.ExportFont(export);
            }
        }

        public void Dispose()
        {
            foreach (XPS_Font font in fonthash.Values)
                    font.Dispose();
        }

    }

    /// <summary>
    ///  Single page export
    /// </summary>
    internal class XPS_PageContent : OoXMLBase
    {
        const float FontMultiplier = 1.32805F;
        const float MetrixMultiplier = 3.776F;
        
        #region "Class overrides"
        public override string RelationType { get { return null;}} // "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"; } }
        public override string ContentType { get { return null;}} //"application/vnd.openxmlformats-officedocument.presentationml.slide+xml"; } }
        public override string FileName { get { return "/Documents/1/Pages/" + pageNumber + ".fpage"; } }
        #endregion

        NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
        
        #region Private fields
        int pageNumber;
        int relationCount;
        XPSExport export;
        private float marginLeft;
        private float marginTop;
        #endregion

        #region Helpers
        private string GetAlphaColor(Color color)
        {
            return Quoted("#" + color.A.ToString("x2") + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2"));
        }

        private string GetColor(Color color)
        {
            return Quoted("#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2"));
        }

        private string TranslateText(string text)
        {
            StringBuilder TextStrings = new StringBuilder();
            int start_idx = 0;

            while (true)
            {
                int idx = text.IndexOfAny("&<>\"".ToCharArray(), start_idx);
                if (idx != -1)
                {
                    TextStrings.Append(text.Substring(start_idx, idx - start_idx));
                    switch (text[idx])
                    {
                        case '&': TextStrings.Append("&amp;"); break;
                        case '<': TextStrings.Append("&lt;"); break;
                        case '>': TextStrings.Append("&gt;"); break;
                        case '"': TextStrings.Append("&quot;"); break;
                    }
                    start_idx = ++idx;
                    continue;
                }
                TextStrings.Append(text.Substring(start_idx));
                break;
            }

            return TextStrings.ToString();
        }

        private bool CheckGlyph(string text, XPS_Font font)
        {
            bool empty = true;
            foreach (char ch in text)
            {
                if ( empty && !Char.IsWhiteSpace(ch)) empty = false;
            }
            return empty;
        }

        private void Add_Glyphs(
            Stream Out, 
            TextObject obj, 
            XPS_Font fnt, 
            Color color, 
            float left, float top, float width, string text)
        {
            string render_transform = "";
            string font_indexes = "";
            string unicode_string = "";
            string bidi_level = "";


            if ( ! CheckGlyph( text,  fnt ) )
            {
                if (obj.RightToLeft)
                {
                    ;
                }
                string remaped_text = fnt.AddString(text, obj.RightToLeft);

                if (obj.Angle != 0 || obj.FontWidthRatio != 1)
                {
                    float fix_angle = (float)(obj.Angle * Math.PI / 180);

                    float c = (float)Math.Round(Math.Cos(fix_angle), 5);
                    float s = (float)Math.Round(Math.Sin(fix_angle), 5);

                    float x = 0;
                    float y = 0;

                    if (obj.FontWidthRatio == 1)
                    {
                        // Fix padding
                        x = obj.AbsLeft + marginLeft + obj.Width / 2;
                        y = obj.AbsTop + marginTop + obj.Height / 2;
                    }

                    render_transform = "RenderTransform=" + Quoted(
                            (c * obj.FontWidthRatio).ToString(nfi) + "," + s.ToString(nfi) + "," +
                            (-s).ToString(nfi) + "," + c.ToString(nfi) + "," +
                            x.ToString(nfi) + "," + y.ToString(nfi));
                }

                font_indexes = fnt.GetXpsIndexes(remaped_text);

                //if (obj.RightToLeft == true)
                //{
                //    bidi_level = "BidiLevel=" + Quoted(1) + " ";
                //}

                if (this.export.humanReadable)
                {
                    unicode_string = " UnicodeString=" + Quoted(TranslateText(text));
                }

                if (obj.TextFill is SolidFill)
                {
                    ExportUtils.WriteLn(Out, "<Glyphs Fill=" + GetAlphaColor(color) +
                            bidi_level +
                            " FontUri=" + Quoted(fnt.FileName) +
                            " FontRenderingEmSize=" + Quoted(fnt.size * FontMultiplier) +
                            " StyleSimulations=" + Quoted("None") +
                            " OriginX=" + Quoted(left) +
                            " OriginY=" + Quoted(top + fnt.size * FontMultiplier) +
                            font_indexes +
                            unicode_string +
                            render_transform +
                        " />");
                }
                else if(obj.TextFill is LinearGradientFill)
                {
                    LinearGradientFill fill = obj.TextFill as LinearGradientFill;

                    float right = marginLeft + obj.AbsRight;
                    float bottom = top + fnt.size * FontMultiplier;
                    float x = 0;

                    ExportUtils.WriteLn(Out, "<Glyphs" +
                            bidi_level +
                            " FontUri=" + Quoted(fnt.FileName) +
                            " FontRenderingEmSize=" + Quoted(fnt.size * FontMultiplier) +
                            " StyleSimulations=" + Quoted("None") +
                            " OriginX=" + Quoted(left) +
                            " OriginY=" + Quoted(top + fnt.size * FontMultiplier) +
                            font_indexes +
                            unicode_string +
                            render_transform + " >");

                    ExportUtils.Write(Out, "<Glyphs.Fill>" +
                        "<LinearGradientBrush MappingMode=\"Absolute\" StartPoint=" +
                        Quoted(x.ToString(nfi) + "," + top.ToString(nfi)) +
                        " EndPoint=" +
                        Quoted(x.ToString(nfi) + "," + bottom.ToString(nfi)) + ">" +
                        "<LinearGradientBrush.GradientStops>" +
                        "<GradientStop Color=" + GetColor(fill.EndColor) + " Offset=\"0\" />" +
                        "<GradientStop Color=" + GetColor(fill.StartColor) + " Offset=\"1\" />" +
                        "</LinearGradientBrush.GradientStops></LinearGradientBrush></Glyphs.Fill>");

                    ExportUtils.WriteLn(Out, "</Glyphs>");
                }

                if ( (fnt.style & FontStyle.Underline) == FontStyle.Underline)
                {
                    float y = top + (fnt.size + 2) * FontMultiplier;
                    ExportUtils.Write(Out, "<Path Data=" + Quoted( 
                            "M " + left.ToString(nfi) + "," + y.ToString(nfi) + 
                            " L " + (left + width).ToString(nfi) + "," + y.ToString(nfi) ) + 
                        " Stroke=" + GetAlphaColor(color)+
                        " StrokeThickness=" + Quoted( (fnt.size / 16).ToString(nfi) ) + 
                        render_transform + " />");
                }

                if ( (fnt.style & FontStyle.Strikeout) == FontStyle.Strikeout)
                {
                    float y = top + (fnt.height / 2) * FontMultiplier;
                    ExportUtils.Write(Out, "<Path Data=" + Quoted(
                            "M " + left.ToString(nfi) + "," + y.ToString(nfi) +
                            " L " + (left + width).ToString(nfi) + "," + y.ToString(nfi)) +
                        " Stroke=" + GetAlphaColor(color) +
                        " StrokeThickness=" + Quoted( (fnt.size / 32).ToString(nfi)) + 
                        render_transform + " />");
                }
            }
        }

        private void FillRectangle(Stream Out, ReportComponentBase obj)
        {
            float left = marginLeft + obj.AbsLeft;
            float right = marginLeft + obj.AbsRight;
            float top = marginTop + obj.AbsTop;
            float bottom = marginTop + obj.AbsBottom;

            if (obj.Fill is SolidFill)
            {
                if (obj.FillColor.A == 0) return;

                ExportUtils.Write(Out, "<Path Data=\"F1 M " + left.ToString(nfi) + "," + top.ToString(nfi) +
                    " H " + right.ToString(nfi) +
                    " V " + bottom.ToString(nfi) +
                    " H " + left.ToString(nfi) +
                    " z\" Fill=" + GetAlphaColor(obj.FillColor) + " />");
            }
            else if (obj.Fill is GlassFill)
            {
                GlassFill fill = obj.Fill as GlassFill;
                ExportUtils.Write(Out, "<Path Data=\"F1 M " + left.ToString(nfi) + "," + top.ToString(nfi) +
                    " H " + right.ToString(nfi) +
                    " V " + (bottom - obj.Height / 2).ToString(nfi) +
                    " H " + left.ToString(nfi) +
                    " z\" Opacity="+Quoted((1 - fill.Blend).ToString(nfi)) +" Fill=" + GetAlphaColor(fill.Color) + " />");

                ExportUtils.Write(Out, "<Path Data=\"F1 M " + left.ToString(nfi) + "," + (top + obj.Height / 2).ToString(nfi) +
                    " H " + right.ToString(nfi) +
                    " V " + bottom.ToString(nfi) +
                    " H " + left.ToString(nfi) +
                    " z\" Fill=" + GetAlphaColor(fill.Color) + " />");
            }
            else if (obj.Fill is LinearGradientFill)
            {
                LinearGradientFill fill = obj.Fill as LinearGradientFill;

                float x = 0;
                // to do: support focus, contrast

                ExportUtils.Write(Out, "<Path>");
                ExportUtils.Write(Out, "<Path.Fill>" +
                    "<LinearGradientBrush MappingMode=\"Absolute\" StartPoint=" + 
                    Quoted( x.ToString(nfi) + "," + top.ToString(nfi) ) + 
                    " EndPoint=" +
                    Quoted( x.ToString(nfi) + "," + bottom.ToString(nfi) ) + ">" +
                    "<LinearGradientBrush.GradientStops>" +
                    "<GradientStop Color=" + GetColor(fill.EndColor) + " Offset=\"0\" />" +
                    "<GradientStop Color="+ GetColor(fill.StartColor) + " Offset=\"1\" />" + 
				    "</LinearGradientBrush.GradientStops></LinearGradientBrush></Path.Fill>");

                ExportUtils.Write(Out, "<Path.Data><PathGeometry><PathFigure StartPoint=" + 
                    Quoted( left.ToString(nfi) +","+ top.ToString(nfi) )+">" +
                    "<PolyLineSegment Points=" + 
                    Quoted(right.ToString(nfi)+ "," + top.ToString(nfi) + " " + 
                    right.ToString(nfi) +"," + bottom.ToString(nfi) + " " + 
                    left.ToString(nfi)+ "," + bottom.ToString(nfi)) + " />" +
                    "</PathFigure></PathGeometry></Path.Data>");

                ExportUtils.Write(Out, "</Path>");
            }
        }

        private string GetLineStyle(LineStyle style)
        {
            string result_style = "StrokeDashArray=";
            switch(style)
            {
                case LineStyle.Solid: return "";
                case LineStyle.Dot:
                    result_style += Quoted("1.0 1.0");
                    break;
                case LineStyle.Dash:
                    result_style += Quoted("2.75 1.0");
                    break;
                case LineStyle.DashDot:
                    result_style += Quoted("2.75 1.0 1.0 1.0");
                    break;
                case LineStyle.DashDotDot:
                    result_style += Quoted("2.75 1.0 1.0 1.0 1.0 1.0");
                    break;
            }
            return result_style;
        }

        private void DrawBorder(Stream Out, ReportComponentBase obj)
        {
            Border b = obj.Border;

            if (b.Lines == BorderLines.None) return;

            float left = marginLeft + obj.AbsLeft;
            float right = marginLeft + obj.AbsRight;
            float top = marginTop + obj.AbsTop;
            float bottom = marginTop + obj.AbsBottom;

            bool same_border =
                b.Lines == BorderLines.All &&
                (b.BottomLine.Color == b.LeftLine.Color) &&
                (b.BottomLine.Color == b.TopLine.Color) &&
                (b.BottomLine.Color == b.RightLine.Color) &&

                (b.BottomLine.DashStyle == b.LeftLine.DashStyle) &&
                (b.BottomLine.DashStyle == b.TopLine.DashStyle) &&
                (b.BottomLine.DashStyle == b.RightLine.DashStyle) &&

                (b.BottomLine.Width == b.LeftLine.Width) &&
                (b.BottomLine.Width == b.TopLine.Width) &&
                (b.BottomLine.Width == b.RightLine.Width);

            if (same_border)
            {
                ExportUtils.Write(Out, "<Path Data=\"M " + left.ToString(nfi) + "," + top.ToString(nfi) +
                    " H " + right.ToString(nfi) +
                    " V " + bottom.ToString(nfi) +
                    " H " + left.ToString(nfi) +
                    " z\" Stroke=" + GetAlphaColor(obj.Border.Color) + 
                    GetLineStyle(obj.Border.Style) + 
                    " StrokeThickness=" + Quoted(obj.Border.Width) + " />");
            }
            else
            {
                if ((b.Lines & BorderLines.Left) == BorderLines.Left)
                {
                    ExportUtils.Write(Out, "<Path Data=\"M " + left.ToString(nfi) + "," + top.ToString(nfi) + " V " + bottom.ToString(nfi) +
                        "\" Stroke=" + GetAlphaColor(b.LeftLine.Color) +
                        GetLineStyle(b.LeftLine.Style) +
                        " StrokeThickness=" + Quoted(b.LeftLine.Width) + " />");
                }
                if ((b.Lines & BorderLines.Bottom) == BorderLines.Bottom)
                {
                    ExportUtils.Write(Out, "<Path Data=\"M " + left.ToString(nfi) + "," + bottom.ToString(nfi) + " H " + right.ToString(nfi) +
                        "\" Stroke=" + GetAlphaColor(b.BottomLine.Color) +
                        GetLineStyle(b.BottomLine.Style) +
                        " StrokeThickness=" + Quoted(b.BottomLine.Width) + " />");
                }
                if ((b.Lines & BorderLines.Right) == BorderLines.Right)
                {
                    ExportUtils.Write(Out, "<Path Data=\"M " + right.ToString(nfi) + "," + top.ToString(nfi) + " V " + bottom.ToString(nfi) +
                        "\" Stroke=" + GetAlphaColor(b.RightLine.Color) +
                        GetLineStyle(b.RightLine.Style) +
                        " StrokeThickness=" + Quoted(b.RightLine.Width) + " />");
                }
                if ((b.Lines & BorderLines.Top) == BorderLines.Top)
                {
                    ExportUtils.Write(Out, "<Path Data=\"M " + left.ToString(nfi) + "," + top.ToString(nfi) + " H " + right.ToString(nfi) +
                        "\" Stroke=" + GetAlphaColor(b.TopLine.Color) +
                        GetLineStyle(b.TopLine.Style) +
                        " StrokeThickness=" + Quoted(b.TopLine.Width) + " />");
                }
            }
        }

        private void DrawShadow(Stream Out, ReportComponentBase obj)
        {
            if (obj.Border.Shadow)
            {
                float x, y, sz;

                sz = obj.Border.ShadowWidth;
                x = marginLeft + obj.AbsRight;
                y = marginTop + obj.AbsTop;

                ExportUtils.Write(Out, "<Path Data=\"F1 M " + x.ToString(nfi) + "," + (y + sz).ToString(nfi) +
                    " H " + (x + sz).ToString(nfi) +
                    " V " + (y + sz + obj.Height).ToString(nfi) +
                    " H " + (x + sz - obj.Width).ToString(nfi) +
                    " V " + (y + obj.Height).ToString(nfi) +
                    " H " + x.ToString(nfi) +
                    " z\" Fill=" + GetAlphaColor(obj.Border.ShadowColor) + " />");
            }
        }

        #endregion

        private MemoryStream file;

        internal void ExportBegin(XPSExport export, ReportPage page)
        {
            this.export = export;
            marginLeft = page.LeftMargin * MetrixMultiplier;
            marginTop = page.TopMargin * MetrixMultiplier;

            file = new MemoryStream();

            ExportUtils.WriteLn(file, "<FixedPage xmlns=\"http://schemas.microsoft.com/xps/2005/06\"" +
                " Width=" + Quoted(ExportUtils.GetPageWidth(page) * MetrixMultiplier) +
                " Height=" + Quoted(ExportUtils.GetPageHeight(page) * MetrixMultiplier) +
                " xml:lang=\"und\">");

            // bitmap watermark on bottom
            if (page.Watermark.Enabled && !page.Watermark.ShowImageOnTop)
                AddBitmapWatermark(file, page);

            // text watermark on bottom
            if (page.Watermark.Enabled && !page.Watermark.ShowTextOnTop)
                AddTextWatermark(file, page);
        }

        internal void ExportBand(XPSExport export, Base band)
        {
            if (!((band as BandBase).Fill is TextureFill))
                AddBandObject(file, band as BandBase);
            else
                AddPictureObject(file, rId, band as ReportComponentBase, "ppt/media/FixMeImage");
            foreach (Base c in band.ForEachAllConvectedObjects(export))
            {
                ReportComponentBase obj = c as ReportComponentBase;
                if (obj is CellularTextObject)
                    obj = (obj as CellularTextObject).GetTable();
                if (obj is TableCell)
                    continue;
                else if (obj is TableBase)
                    AddTable(file, obj as TableBase);
                else if (obj is TextObject && !(obj as TextObject).TextOutline.Enabled)
                    AddTextObject(file, obj as TextObject);
                else if (obj is BandBase && !(obj.Fill is TextureFill))
                    AddBandObject(file, obj as BandBase);
                else if (obj is LineObject)
                    AddLine(file, obj as LineObject);
                else if (obj is ShapeObject && !(obj.Fill is TextureFill))
                    AddShape(file, obj as ShapeObject);
                else if (obj is PictureObject)
                    AddPictureObject(file, rId, obj as PictureObject, "ppt/media/image");
                else if (obj is Barcode.BarcodeObject)
                    AddPictureObject(file, rId, obj as ReportComponentBase, "ppt/media/BarcodeImage");
                else if (obj is ZipCodeObject)
                    AddPictureObject(file, rId, obj as ReportComponentBase, "ppt/media/ZipCodeImage");
#if MSCHART
                else if (obj is MSChart.MSChartObject)
                    AddPictureObject(file, rId, obj as ReportComponentBase, "ppt/media/MSChartImage");
#endif
#if !FRCORE
                else if (obj is RichObject)
                    AddPictureObject(file, rId, obj as ReportComponentBase, "ppt/media/RichTextImage");
#endif
                else if (obj is CheckBoxObject)
                    AddCheckboxObject(file, obj as CheckBoxObject);
                else if (obj == null)
                {
                    ;
                }
                else
                {
                    AddPictureObject(file, rId, obj as ReportComponentBase, "ppt/media/FixMeImage");
                }
            }
        }

        internal void ExportEnd(XPSExport export, ReportPage page)
        {
            // bitmap watermark on top
            if (page.Watermark.Enabled && page.Watermark.ShowImageOnTop)
                AddBitmapWatermark(file, page);

            // text watermark on top
            if (page.Watermark.Enabled && page.Watermark.ShowTextOnTop)
                AddTextWatermark(file, page);

            ExportUtils.WriteLn(file, "</FixedPage>");

            file.Position = 0;

            export.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);

            this.ExportRelations(this.export);
        }

        #region "Export report object primitives"
        private void AddCheckboxObject(Stream Out, CheckBoxObject checkBox)
        {
            if (checkBox.HideIfUnchecked && !checkBox.Checked) return;

            RectangleF drawRect = new RectangleF(
                marginLeft + checkBox.AbsLeft, marginTop + checkBox.AbsTop, checkBox.Width, checkBox.Height);

            FillRectangle(Out, checkBox);
            DrawBorder(Out, checkBox);

            if (!checkBox.Checked && checkBox.UncheckedSymbol == UncheckedSymbol.None) return;

            ExportUtils.Write(Out, "<Path Data=\"");

            float ratio = checkBox.Width / (Units.Millimeters * 5);
            drawRect.Inflate(-4 * ratio, -4 * ratio);

            if (checkBox.Checked)
            {
                switch (checkBox.CheckedSymbol)
                {
                    case CheckedSymbol.Check:
                        ExportUtils.Write(Out, "M " + drawRect.Left.ToString(nfi) + "," +
                            (drawRect.Top + drawRect.Height / 10 * 5).ToString(nfi) + " L " +
                            (drawRect.Left + drawRect.Width / 10 * 4).ToString(nfi) + "," +
                            (drawRect.Bottom - drawRect.Height / 10).ToString(nfi) + " " +
                            drawRect.Right.ToString(nfi) + "," +
                            (drawRect.Top + drawRect.Height / 10).ToString(nfi)
                        );
                        break;

                    case CheckedSymbol.Cross:
                        ExportUtils.Write(Out, "M " + drawRect.Left.ToString(nfi) + "," + drawRect.Top.ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) +
                            " M " + drawRect.Left.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + drawRect.Top.ToString(nfi)
                        );
                        break;

                    case CheckedSymbol.Plus:
                        ExportUtils.Write(Out, "M " + drawRect.Left.ToString(nfi) + "," + (drawRect.Top + drawRect.Height / 2).ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + (drawRect.Top + drawRect.Height / 2).ToString(nfi) +
                            " M " + (drawRect.Left + drawRect.Width / 2).ToString(nfi) + "," + drawRect.Top.ToString(nfi) +
                            " L " + (drawRect.Left + drawRect.Width / 2).ToString(nfi) + "," + drawRect.Bottom.ToString(nfi)
                        );
                        break;

                    case CheckedSymbol.Fill:
                        ExportUtils.Write(Out, "M " + drawRect.Left.ToString(nfi) + "," + drawRect.Top.ToString(nfi) + " L " +
                            drawRect.Right.ToString(nfi) + "," + drawRect.Top.ToString(nfi) + " " +
                            drawRect.Right.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) + " " +
                            drawRect.Left.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) + " z "
                        );
                        break;
                }
            }
            else
            {
                switch (checkBox.UncheckedSymbol)
                {
                    case UncheckedSymbol.Cross:
                        ExportUtils.Write(Out, "M " + drawRect.Left.ToString(nfi) + "," + drawRect.Top.ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) +
                            " M " + drawRect.Left.ToString(nfi) + "," + drawRect.Bottom.ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + drawRect.Top.ToString(nfi)
                        );
                        break;

                    case UncheckedSymbol.Minus:
                        ExportUtils.Write(Out, "M " + drawRect.Left.ToString(nfi) + "," + (drawRect.Top + drawRect.Height / 2).ToString(nfi) +
                            " L " + drawRect.Right.ToString(nfi) + "," + (drawRect.Top + drawRect.Height / 2).ToString(nfi)
                        );
                        break;
                }
            }
            ExportUtils.WriteLn(Out, "\" Stroke=" + GetAlphaColor(checkBox.CheckColor) + " StrokeThickness=" + Quoted(checkBox.Border.Width * 1.5F) + ">");
            if (checkBox.Checked && checkBox.CheckedSymbol == CheckedSymbol.Fill)
            {
                ExportUtils.WriteLn(Out, "<Path.Fill><SolidColorBrush Color=" + GetColor(checkBox.CheckColor) + "/></Path.Fill>");
            }
            ExportUtils.WriteLn(Out, "</Path>");
        }

        private void AddPictureObject(Stream Out, object p, ReportComponentBase pictureObject, string ImageNameMask)
        {
            if (pictureObject.Width > 0 && pictureObject.Height > 0)
            {
                XPS_Picture xps_picture = new XPS_Picture(pictureObject);

                float left = marginLeft + pictureObject.AbsLeft;
                float right = marginLeft + pictureObject.AbsRight;
                float top = marginTop + pictureObject.AbsTop;
                float bottom = marginTop + pictureObject.AbsBottom;

                string picturePath = xps_picture.SaveImage(export, pictureObject, true);

                ExportUtils.Write(Out, "<Path Data=\"");
                ExportUtils.Write(Out, "M " + left.ToString(nfi) + "," + top.ToString(nfi) +
                    " L " + right.ToString(nfi) + "," + top.ToString(nfi) +
                    " " + right.ToString(nfi) + "," + bottom.ToString(nfi) +
                    " " + left.ToString(nfi) + "," + bottom.ToString(nfi) + " z\">");
                ExportUtils.WriteLn(Out, "<Path.Fill>");

                ExportUtils.Write(Out, "<ImageBrush TileMode=\"None\" ViewboxUnits=\"Absolute\" ViewportUnits=\"Absolute\"");
                ExportUtils.Write(Out, " ImageSource=" + Quoted(picturePath));
                ExportUtils.Write(Out, " Viewbox=\"0,0," +
                    pictureObject.Width.ToString(nfi) + "," +
                    pictureObject.Height.ToString(nfi) + "\"");
                ExportUtils.Write(Out, " Viewport=\"" +
                    left.ToString(nfi) + "," +
                    top.ToString(nfi) + "," +
                    pictureObject.Width.ToString(nfi) + "," +
                    pictureObject.Height.ToString(nfi) + "\" />");

                ExportUtils.WriteLn(Out, "</Path.Fill></Path>");

                DrawBorder(Out, pictureObject);

                if (this.AddRelation(relationCount, xps_picture) == true) relationCount++;
            }
        }

        private void AddShape(Stream Out, ShapeObject shapeObject)
        {
            float x = marginLeft + (shapeObject.AbsLeft + shapeObject.Border.Width / 2);
            float y = marginTop + (shapeObject.AbsTop + shapeObject.Border.Width / 2);
            float dx = (shapeObject.Width - shapeObject.Border.Width) - 1;
            float dy = (shapeObject.Height - shapeObject.Border.Width) - 1;
            float x1 = x + dx;
            float y1 = y + dy;

            ExportUtils.Write(Out, "<Path Data=\"");

            switch (shapeObject.Shape)
            {
                case ShapeKind.Diamond:
                    ExportUtils.Write(Out, "M " + (x + dx / 2).ToString(nfi) + "," + y.ToString(nfi) + " L " +
                        x1.ToString(nfi) + "," + (y+dy/2).ToString(nfi) + " " +
                        (x+dx/2).ToString(nfi) + "," + y1.ToString(nfi) + " " +
                        x.ToString(nfi) + "," + (y+dy/2).ToString(nfi) + " ");
                    break;
                case ShapeKind.Ellipse:
                    x1 = x + dx / 2;
                    dx /= 2;
                    dy /= 2;
                    ExportUtils.Write(Out, "M " + x1.ToString(nfi) + "," + y.ToString(nfi) +
                        " A " + dx.ToString(nfi) + "," + dy.ToString(nfi) +
                        " 0 1 0 " + (x1 + 0.1).ToString(nfi) + "," + y.ToString(nfi) + " ");
                    break;
                case ShapeKind.Rectangle:
                    ExportUtils.Write(Out, "M " + x.ToString(nfi) + "," + y.ToString(nfi) + " L " + 
                        x1.ToString(nfi) + "," + y.ToString(nfi) + " " +
                        x1.ToString(nfi) + "," + y1.ToString(nfi) + " " +
                        x.ToString(nfi) + "," + y1.ToString(nfi) + " ");
                    break;
                case ShapeKind.RoundRectangle:
                    float min = Math.Min(dx, dy);
                    if (shapeObject.Curve == 0) min = min / 4; else min = Math.Min(min, shapeObject.Curve * 10);
                    ExportUtils.Write(Out, "M " + (x + min).ToString(nfi) + "," + y.ToString(nfi) + " L " +
                        (x1 - min).ToString(nfi) + "," + y.ToString(nfi) +
                        " A " + min.ToString(nfi) + "," + min.ToString(nfi) + 
                            " 0 0 1 " + x1.ToString(nfi) + "," + (y + min).ToString(nfi) +
                        " L " + x1.ToString(nfi) + "," + (y1 - min).ToString(nfi) +
                        " A " + min.ToString(nfi) + "," + min.ToString(nfi) + 
                            " 0 0 1 " + (x1 - min).ToString(nfi) + "," + y1.ToString(nfi) +
                        " L " + (x + min).ToString(nfi) + "," + y1.ToString(nfi) +
                        " A " + min.ToString(nfi) + "," + min.ToString(nfi) + 
                            " 0 0 1 " + x.ToString(nfi) + "," + (y1 - min).ToString(nfi) +
                        " L " + x.ToString(nfi) + "," + (y + min).ToString(nfi) +
                        " A " + min.ToString(nfi) + "," + min.ToString(nfi) + 
                            " 0 0 1 " + (x + min).ToString(nfi) + "," + y.ToString(nfi) + " ");

                    break;
                case ShapeKind.Triangle:
                    ExportUtils.Write(Out, "M " + x1.ToString(nfi) + "," + y1.ToString(nfi) + " L " +
                        x.ToString(nfi) + "," + y1.ToString(nfi) + " " +
                        (x + dx /2).ToString(nfi) + "," + y.ToString(nfi) + " ");
                    break;
                default: throw new Exception("Unsupported shape kind");
            }
            ExportUtils.WriteLn(Out, "z\" Stroke=" + GetAlphaColor(shapeObject.Border.Color) + " StrokeThickness=" + Quoted(shapeObject.Border.Width) + " >");
            ExportUtils.WriteLn(Out, "</Path>");
        }

        private void AddLine(Stream Out, LineObject lineObject)
        {
            float left = marginLeft + lineObject.AbsLeft;
            float right = marginLeft + lineObject.AbsRight;
            float top = marginTop + lineObject.AbsTop;
            float bottom = marginTop + lineObject.AbsBottom;

            ExportUtils.Write(Out, "<Path Data=\"");
            ExportUtils.Write(Out, "M " + left.ToString(nfi) + "," + top.ToString(nfi) + " L " + right.ToString(nfi) + "," + bottom.ToString(nfi));
            ExportUtils.WriteLn(Out, "\" Stroke=" + GetAlphaColor(lineObject.Border.Color) + " StrokeThickness=" + Quoted(lineObject.Border.Width) + " >");
            ExportUtils.WriteLn(Out, "</Path>");
        }

        private void AddBandObject(Stream Out, BandBase bandBase)
        {
            if (bandBase.HasBorder)
            {
                DrawBorder(Out, bandBase);
            }
            if (bandBase.HasFill) 
            {
                FillRectangle(Out, bandBase);
            }
        }

        private void AddTextObject(Stream Out, TextObject obj)
        {
            float FDpiFX = 96f / DrawUtils.ScreenDpi;
            bool FirstOccurrence;

            FillRectangle(Out, obj);
            DrawShadow(Out, obj);
            DrawBorder(Out, obj);

            IGraphics g = export.Report.MeasureGraphics;
            using (Font f = new Font(obj.Font.Name, obj.Font.Size * FDpiFX, obj.Font.Style))
            {
                XPS_Font font = export.FontList.AddFont(export, obj.Font, out FirstOccurrence);
                if (this.AddRelation(relationCount, font) == true) relationCount++;
                using (GraphicCache cache = new GraphicCache())
                {
                    RectangleF textRect = new RectangleF(
                      marginLeft + obj.AbsLeft + obj.Padding.Left,
                      marginTop + obj.AbsTop + obj.Padding.Top,
                      obj.Width - obj.Padding.Horizontal,
                      obj.Height - obj.Padding.Vertical);

                    StringFormat format = obj.GetStringFormat(cache, 0);
                    Brush textBrush = cache.GetBrush(obj.TextColor);
                    AdvancedTextRenderer renderer = new AdvancedTextRenderer(obj.Text, g, f, textBrush, null,
                        textRect, format, obj.HorzAlign, obj.VertAlign, obj.LineHeight, obj.Angle, obj.FontWidthRatio,
                        obj.ForceJustify, obj.Wysiwyg, obj.HasHtmlTags, true, FDpiFX, FDpiFX, obj.InlineImageCache);

                    foreach (AdvancedTextRenderer.Paragraph paragraph in renderer.Paragraphs)
                    {
                        foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                        {
                            foreach (AdvancedTextRenderer.Word word in line.Words)
                            {
                              float fix_hpos = f.Height * 0.1f; // to match .net char X offset
                              // invert offset in case of rtl
                              if (obj.RightToLeft)
                                fix_hpos = -fix_hpos;
                              // we don't need this offset if text is centered
                              if (obj.HorzAlign == HorzAlign.Center)
                                fix_hpos = 0;  

                              
                                if (renderer.HtmlTags)
                                    foreach (AdvancedTextRenderer.Run run in word.Runs)
                                    {
                                        Font runFont = run.GetFont();
                                        using (Font htmlFont =new Font(runFont.FontFamily, runFont.Size / FDpiFX, runFont.Style))
                                        {
                                            XPS_Font mod_font = export.FontList.AddFont(export, htmlFont, out FirstOccurrence);
                                            if (this.AddRelation(relationCount, mod_font) == true) relationCount++;
                                            Add_Glyphs(Out, obj, mod_font, run.Style.Color, fix_hpos + run.Left, run.Top, run.Width, run.Text);
                                        }
                                    }
                                else
                                    Add_Glyphs(Out, obj, font, obj.TextColor, fix_hpos + word.Left, word.Top, word.Width, word.Text);
                            }
                        }
                    }
                }
            }
        }

        private void AddTable(Stream Out, TableBase table)
        {
            TableBase frame = new TableBase();

            frame.Assign(table);
            frame.Left = table.AbsLeft;
            frame.Top = table.AbsTop;

            float x = 0;
            float y = 0;
            for (int j = 0; j < table.Columns.Count; j++)
            {
                y = 0;
                for (int i = 0; i < table.RowCount; i++)
                {
                    if (!table.IsInsideSpan(table[j, i]))
                    {
                        TableCell textcell = table[j, i];

                        textcell.Left = x;
                        textcell.Top = y;

                        AddTextObject(Out, textcell);
                    }
                    y += (table.Rows[i]).Height;
                }
                x += (table.Columns[j]).Width;
            }

            frame.Width = x;
            frame.Height = y;
            DrawBorder(Out, frame);
        }

        private void AddTextWatermark(Stream Out, ReportPage page)
        {
            TextObject obj = page.Watermark.TextObject;
            if (obj == null) return;

            RectangleF displayRect = new RectangleF(
                -marginLeft,
                -marginTop,
                ExportUtils.GetPageWidth(page) * MetrixMultiplier,
                ExportUtils.GetPageHeight(page) * MetrixMultiplier);

            obj.Bounds = displayRect;
            int angle = 0;
            switch (page.Watermark.TextRotation)
            {
                case WatermarkTextRotation.Horizontal:
                    angle = 0;
                    break;
                case WatermarkTextRotation.Vertical:
                    angle = 270;
                    break;
                case WatermarkTextRotation.ForwardDiagonal:
                    angle = 360 - (int)(Math.Atan(displayRect.Height / displayRect.Width) * (180 / Math.PI));
                    break;
                case WatermarkTextRotation.BackwardDiagonal:
                    angle = (int)(Math.Atan(displayRect.Height / displayRect.Width) * (180 / Math.PI));
                    break;
            }
            obj.Angle = angle;

            AddTextObject(Out, obj);
        }

        private void AddBitmapWatermark(Stream Out, ReportPage page)
        {
            PictureObject pic = page.Watermark.PictureObject;
            if (pic.Image == null) return;

            RectangleF displayRect = new RectangleF( 
                -marginLeft, 
                -marginTop, 
                ExportUtils.GetPageWidth(page) * MetrixMultiplier, 
                ExportUtils.GetPageHeight(page) * MetrixMultiplier);

            pic.Bounds = displayRect;

            PictureBoxSizeMode sizeMode = PictureBoxSizeMode.Normal;
            if (page.Watermark.ImageSize == WatermarkImageSize.Stretch)
                sizeMode = PictureBoxSizeMode.StretchImage;
            else if (page.Watermark.ImageSize == WatermarkImageSize.Zoom)
                sizeMode = PictureBoxSizeMode.Zoom;
            else if (page.Watermark.ImageSize == WatermarkImageSize.Center)
                sizeMode = PictureBoxSizeMode.CenterImage;
            pic.SizeMode = sizeMode;
            pic.Tile = page.Watermark.ImageSize == WatermarkImageSize.Tile;

            AddPictureObject(Out, rId, pic, "BackgroundPic");
        }

        #endregion

        internal XPS_PageContent(int pageNo)
        {
            pageNumber = pageNo;
        }
    }




    /// <summary>
    ///  Document structure descriptor
    /// </summary>
    internal class XPS_FixedDocument : OoXMLBase
    {
        #region "Class overrides"
        public override string RelationType { get { return null; } } // "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"; } }
        public override string ContentType { get { return null; } } //"application/vnd.openxmlformats-officedocument.presentationml.slide+xml"; } }
        public override string FileName { get { return "/Documents/1/FixedDocument.fdoc"; } }
        #endregion

        #region Private fields
        private ArrayList pageList;
        #endregion

        internal void AddPage(XPS_PageContent page)
        {
            pageList.Add(page);
        }

        internal void Export(XPSExport export)
        {
            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, "<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            ExportUtils.WriteLn(file, "<FixedDocument xmlns=\"http://schemas.microsoft.com/xps/2005/06\">");
            foreach (XPS_PageContent page in pageList)
            {
                ExportUtils.WriteLn(file, "<PageContent Source=" + Quoted(page.FileName) + "/>");
            }
            ExportUtils.WriteLn(file, "</FixedDocument>");

            file.Position = 0;

            export.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }

        internal XPS_FixedDocument()
        {
            pageList = new ArrayList();
        }
    }

    /// <summary>
    ///  Main class of XML export
    /// </summary>
    public partial class XPSExport : OOExportBase, IDisposable
    {
        #region Private fields
        private OoXMLCoreDocumentProperties coreDocProp;
        private XPS_FixedDocument           fixedDocument;
        private XPS_FontList                fontList;
        #endregion

        #region Internal properties
        internal XPS_FontList FontList { get { return fontList; } }
        #endregion

        #region Public fields
        /// <summary>
        /// PicturesCount
        /// </summary>
        public int picturesCount;
        /// <summary>
        /// HumanReadable
        /// </summary>
        public bool humanReadable; 
        #endregion

        #region Private methods
        private void CreateContentTypes()
        {
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");
            ExportUtils.WriteLn(file, "<Default Extension=\"png\" ContentType=\"image/png\" /> ");
            ExportUtils.WriteLn(file, "<Default Extension=\"jpeg\" ContentType=\"image/jpg\" /> ");
            ExportUtils.WriteLn(file, "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\" /> ");
            ExportUtils.WriteLn(file, "<Default Extension=\"xml\" ContentType=\"application/xml\" /> ");
            ExportUtils.WriteLn(file, "<Default Extension=\"fdseq\" ContentType=\"application/vnd.ms-package.xps-fixeddocumentsequence+xml\" /> ");
            ExportUtils.WriteLn(file, "<Default Extension=\"fpage\" ContentType=\"application/vnd.ms-package.xps-fixedpage+xml\" /> ");
            ExportUtils.WriteLn(file, "<Default Extension=\"struct\" ContentType=\"application/vnd.ms-package.xps-documentstructure+xml\" /> ");
            ExportUtils.WriteLn(file, "<Default Extension=\"jpg\" ContentType=\"image/jpeg\" /> ");
            ExportUtils.WriteLn(file, "<Default Extension=\"odttf\" ContentType=\"application/vnd.ms-package.obfuscated-opentype\" /> ");
            ExportUtils.WriteLn(file, "<Default Extension=\"fdoc\" ContentType=\"application/vnd.ms-package.xps-fixeddocument+xml\" /> ");
            ExportUtils.WriteLn(file, "<Override PartName=\"/docProps/core.xml\" ContentType=\"application/vnd.openxmlformats-package.core-properties+xml\" /> ");
            ExportUtils.WriteLn(file, "</Types>");
            file.Position = 0;
            Zip.AddStream("[Content_Types].xml", file);            
        }
        private void CreateRelations()
        {
            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId3\" Type=" + Quoted(coreDocProp.RelationType) + " Target=" + Quoted(coreDocProp.FileName) + " /> ");
            //                Out.WriteLine("<Relationship Id="rId2" Type="http://schemas.openxmlformats.org/package/2006/relationships/metadata/thumbnail" Target="docProps/thumbnail.jpeg" /> ");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId1\" Type=\"http://schemas.microsoft.com/xps/2005/06/fixedrepresentation\" Target=\"FixedDocSeq.fdseq\" /> ");
            ExportUtils.WriteLn(file, "</Relationships>");

            file.Position = 0;
            Zip.AddStream("_rels/.rels", file);            
        }
        private void CreateFixedDocumentSequence()
        {
            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<FixedDocumentSequence xmlns=\"http://schemas.microsoft.com/xps/2005/06\">");
            ExportUtils.WriteLn(file, "<DocumentReference Source=\"/Documents/1/FixedDocument.fdoc\"/>");
            ExportUtils.WriteLn(file, "</FixedDocumentSequence>");

            file.Position = 0;
            Zip.AddStream("FixedDocSeq.fdseq", file);
        }
        
        private void ExportXPS(Stream Stream)
        {
            CreateContentTypes();
            CreateRelations();
            CreateFixedDocumentSequence();

            coreDocProp.Export(this);
            fixedDocument.Export(this);
            fontList.Export(this);
        }

        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            Zip = new ZipArchive();

            if (fontList != null)
                fontList.Dispose();
            picturesCount = 0;
            coreDocProp = new OoXMLCoreDocumentProperties();
            fixedDocument = new XPS_FixedDocument();
            fontList = new XPS_FontList();

            pageNo = 0;
        }

        private int pageNo;
        private XPS_PageContent xps_page;

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            xps_page = new XPS_PageContent(pageNo+++1);
            fixedDocument.AddPage(xps_page);
            xps_page.ExportBegin(this, page);
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            xps_page.ExportBand(this, band);
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            xps_page.ExportEnd(this, page);            
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            ExportXPS(Stream);
            Zip.SaveToStream(Stream);
            Zip.Clear();
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("XpsFile");
        }
        #endregion

        /// <summary>
        /// Constructor of XPSExport
        /// </summary>
        public XPSExport()
        {
            humanReadable = false;
        }

        /// <summary>
        /// Destructor of XPSExport
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            if(fontList != null)
                fontList.Dispose();
        }

    }
}
