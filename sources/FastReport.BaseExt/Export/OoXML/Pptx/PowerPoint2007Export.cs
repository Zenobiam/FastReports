using FastReport.Map;
using FastReport.Table;
using FastReport.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace FastReport.Export.OoXML
{
    /// <summary>
    /// Power point shape
    /// </summary>
    internal class PptShape : OoXMLBase
    {
        const float PPT_DIVIDER = 360000 / 37.8f;

        #region Class overrides
        public override string RelationType 
        { 
            get 
            {
                if (aObj.Fill is TextureFill || 
                    aObj is PictureObjectBase || 
                    aObj is Barcode.BarcodeObject ||
                    aObj is CheckBoxObject ||
                    aObj is ZipCodeObject ||
#if MSCHART
                    aObj is MSChart.MSChartObject ||
#endif
                    aObj is RichObject ||
                    aObj is TextObject ||
                    aObj is MapObject) 
                        return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
                // TODO check if throw realy need
                throw new Exception("Cant get relation type for this object"); 
            } 
        }
        public override string ContentType { get { throw new Exception(); } }
        
        public override string FileName 
        { 
            get 
            {
                if (aObj.Fill is TextureFill || 
                    aObj is PictureObjectBase || 
                    aObj is Barcode.BarcodeObject ||
                    aObj is CheckBoxObject ||
                    aObj is ZipCodeObject ||
#if MSCHART
                    aObj is MSChart.MSChartObject ||
#endif
                    aObj is RichObject ||
                    aObj is TextObject ||
                    aObj is MapObject) 
                        return name;
                // TODO check if throw realy need
                throw new Exception("Cannot store object as image"); 
            }

        }
        #endregion

        int id;
        int relationIdentifier;
        string name;
        PowerPoint2007Export pptExport;

        ulong x;
        ulong y;
        ulong cX;
        ulong cY;
        bool flipH;
        bool flipV;

        StringBuilder           textStrings;
        ReportComponentBase     aObj;

        #region "Private methods"
        private new string Quoted(string p) { return "\"" + p + "\" "; }
        private new string Quoted(long p) { return "\"" + p.ToString() + "\" "; }
        private string QuotedString(ulong a) { return "\"" + a.ToString() + "\" "; }
        private string GetRGBString(Color c)
        {
            return "\"" + ExportUtils.ByteToHex(c.R) + ExportUtils.ByteToHex(c.G) + ExportUtils.ByteToHex(c.B) + "\"";
        }
        private string GetDashType(LineStyle style)
        {
            switch (style)
            {
                case LineStyle.Solid:       return "<a:prstDash val=\"solid\" />";
                case LineStyle.Dot:         return "<a:prstDash val=\"sysDot\" />";
                case LineStyle.Dash:        return "<a:prstDash val=\"sysDash\" />";
                case LineStyle.DashDot:     return "<a:prstDash val=\"sysDashDot\" />";
                case LineStyle.DashDotDot:  return "<a:prstDash val=\"sysDashDotDot\" />";
                case LineStyle.Double: return "";
            }
            throw new Exception("Unsupported dash style");
        }

        private string TranslateText(string text)
        {
            StringBuilder TextStrings = new StringBuilder();
            int start_idx = 0;

            while (true)
            {
                int idx = text.IndexOfAny("&<>".ToCharArray(), start_idx);
                if (idx != -1)
                {
                    TextStrings.Append(text.Substring(start_idx, idx - start_idx));
                    switch (text[idx])
                    {
                        case '&': TextStrings.Append("&amp;"); break;
                        case '<': TextStrings.Append("&lt;"); break;
                        case '>': TextStrings.Append("&gt;"); break;
                    }
                    start_idx = ++idx;
                    continue;
                }
                TextStrings.Append(text.Substring(start_idx));
                break;
            }

            return TextStrings.ToString();
        }

        #endregion

        public int RelationID { get { return relationIdentifier; } }
        public ReportComponentBase Object { get { return aObj; } }

        #region Constructors

        public PptShape(int Id, int RelationID, string Name, ReportComponentBase obj, PowerPoint2007Export ppt_export)
        {
            this.id = Id;
            this.relationIdentifier = RelationID;
            this.name = Name;
            this.aObj = obj;
            this.pptExport = ppt_export;

            this.x = (ulong)(obj.AbsLeft * PPT_DIVIDER + pptExport.LeftMargin);
            this.y = (ulong)(obj.AbsTop * PPT_DIVIDER + pptExport.TopMargin); 

            if (obj.Height < 0) 
            { this.cY = (ulong) -(obj.Height * PPT_DIVIDER);  flipV = true; }
            else 
            { this.cY = (ulong)(obj.Height * PPT_DIVIDER); flipV = false; }

            if (obj.Width < 0)
            { this.cX = (ulong)-(obj.Width * PPT_DIVIDER); flipH = true; }
            else
            { this.cX = (ulong)(obj.Width * PPT_DIVIDER); flipH = false; }

            if (cX == 0) cX = 1; // 588;
            if (cY == 0) cY = 1588;

            textStrings = new StringBuilder();
        
        }
        #endregion

        private bool Export_Borders(Stream Out, bool rotated)
        {
            const long EMU = 12700;
            Border b = aObj.Border;

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

            if (aObj is LineObject || (aObj is ShapeObject) || (aObj is PolyLineObject)
                /*|| (FObject is TableCell)*/ ||
                ( same_border && ! rotated) )
            {
                ulong bw = (ulong)(EMU * aObj.Border.Width);
                ExportUtils.WriteLn(Out, "<a:ln w=" + QuotedString(bw) + ">");
                ExportUtils.WriteLn(Out, "<a:solidFill>");
                ExportUtils.WriteLn(Out, "<a:srgbClr val=" + GetRGBString(b.Color) + " />");
                ExportUtils.WriteLn(Out, "</a:solidFill>");
                ExportUtils.WriteLn(Out, GetDashType(b.Style));
                if (aObj is LineObject)
                {
                    LineObject line = aObj as LineObject;
                    string StartCap = null;
                    string EndCap = null;

                    switch (line.StartCap.Style)
                    {
                        case CapStyle.Arrow:    StartCap = "classic";     break;
                        case CapStyle.Circle:   StartCap = "oval";      break;
                        case CapStyle.Diamond:  StartCap = "diamond";   break;
                        case CapStyle.Square:   StartCap = "diamond";   break;
                    }
                    if(StartCap != null) ExportUtils.WriteLn(Out, "<a:tailEnd type="+Quoted(StartCap)+" />");

                    switch (line.EndCap.Style)
                    {
                        case CapStyle.Arrow:    EndCap = "classic";   break;
                        case CapStyle.Circle:   EndCap = "oval";    break;
                        case CapStyle.Diamond:  EndCap = "diamond"; break;
                        case CapStyle.Square:   EndCap = "diamond"; break;
                    }
                    if (EndCap != null) ExportUtils.WriteLn(Out, "<a:headEnd type=" + Quoted(EndCap) + " />");
                }
                ExportUtils.WriteLn(Out, "</a:ln>");
            }

            return (!same_border || rotated) && (aObj.Border.Lines != BorderLines.None);
        }

        internal void ExportFourBorders(Stream Out, int id)
        {
            Border b = aObj.Border;

            if ((b.Lines & BorderLines.Left) == BorderLines.Left)
            {
                Export_Line(Out, x, y, 0, cY, b.LeftLine.Color, b.LeftLine.Width, b.LeftLine.Style, id++);
            }
            if ((b.Lines & BorderLines.Bottom) == BorderLines.Bottom)
            {
                Export_Line(Out, x, y + cY, cX, 0, b.BottomLine.Color, b.BottomLine.Width, b.BottomLine.Style, id++);
            }
            if ((b.Lines & BorderLines.Right) == BorderLines.Right)
            {
                Export_Line(Out, x + cX, y, 0, cY, b.RightLine.Color, b.RightLine.Width, b.RightLine.Style, id++);
            }
            if ((b.Lines & BorderLines.Top) == BorderLines.Top)
            {
                Export_Line(Out, x, y, cX, 0, b.TopLine.Color, b.TopLine.Width, b.TopLine.Style, id++);
            }
        }

        private void Export_Fills(Stream Out)
        {
            const long PXA = 60000;
            if (aObj.Fill is LinearGradientFill)
            {
                LinearGradientFill linear = aObj.Fill as LinearGradientFill;
                ExportUtils.WriteLn(Out, "<a:gradFill flip=\"none\" rotWithShape=\"1\">");
                ExportUtils.WriteLn(Out, "<a:gsLst>");
                ExportUtils.WriteLn(Out, "<a:gs pos=\"0\">");
                ExportUtils.WriteLn(Out, "<a:srgbClr val="+ GetRGBString(linear.StartColor) +" />");
                ExportUtils.WriteLn(Out, "</a:gs>");
                ExportUtils.WriteLn(Out, "<a:gs pos=\"100000\">");
                ExportUtils.WriteLn(Out, "<a:srgbClr val="+ GetRGBString(linear.EndColor) +" />");
                ExportUtils.WriteLn(Out, "</a:gs>");
                ExportUtils.WriteLn(Out, "</a:gsLst>");
                ExportUtils.WriteLn(Out, "<a:lin ang="+Quoted((linear.Angle * PXA).ToString())+" scaled=\"1\" />");
                ExportUtils.WriteLn(Out, "<a:tileRect />");
                ExportUtils.WriteLn(Out, "</a:gradFill>");
            }
            else if (aObj.Fill is SolidFill)
            {
                SolidFill fill = aObj.Fill as SolidFill;
                if (fill.IsTransparent)
                {
                    ExportUtils.WriteLn(Out, "<a:noFill />");
                }
                else
                {
                    ExportUtils.WriteLn(Out, "<a:solidFill>");
                    ExportUtils.WriteLn(Out, "<a:srgbClr val=" + GetRGBString(fill.Color) + " />");
                    ExportUtils.WriteLn(Out, "</a:solidFill>");
                }
            }
            else if (aObj.Fill is GlassFill)
            { 
                GlassFill fill = aObj.Fill as GlassFill;
                ExportUtils.WriteLn(Out, "<a:gradFill flip=\"none\" rotWithShape=\"1\">");
                ExportUtils.WriteLn(Out, "<a:gsLst>");
                ExportUtils.WriteLn(Out, "<a:gs pos=\"0\">");
                ExportUtils.WriteLn(Out, "<a:srgbClr val="+ GetRGBString(fill.Color) +">");
                ExportUtils.WriteLn(Out, "<a:alpha val=\"50000\" />");
                ExportUtils.WriteLn(Out, "</a:srgbClr>");
                ExportUtils.WriteLn(Out, "</a:gs>");
                ExportUtils.WriteLn(Out, "<a:gs pos=\"50000\">");
                ExportUtils.WriteLn(Out, "<a:srgbClr val="+ GetRGBString(fill.Color) +">");
                ExportUtils.WriteLn(Out, "<a:alpha val=\"50000\" />");
                ExportUtils.WriteLn(Out, "</a:srgbClr>");
                ExportUtils.WriteLn(Out, "</a:gs>");
                ExportUtils.WriteLn(Out, "<a:gs pos=\"50001\">");
                ExportUtils.WriteLn(Out, "<a:srgbClr val="+ GetRGBString(fill.Color) +" />");
                ExportUtils.WriteLn(Out, "</a:gs>");
                ExportUtils.WriteLn(Out, "<a:gs pos=\"100000\">");
                ExportUtils.WriteLn(Out, "<a:srgbClr val="+ GetRGBString(fill.Color) +" />");
                ExportUtils.WriteLn(Out, "</a:gs>");
                ExportUtils.WriteLn(Out, "</a:gsLst>");
                ExportUtils.WriteLn(Out, "<a:lin ang=" + Quoted(5400000) + " scaled=\"1\" />");
                ExportUtils.WriteLn(Out, "<a:tileRect />");
                ExportUtils.WriteLn(Out, "</a:gradFill>");
            }
            else if(aObj.Fill is HatchFill)
            {
                ExportUtils.WriteLn(Out, "<a:blipFill dpi=\"0\" rotWithShape=\"1\">");
                ExportUtils.WriteLn(Out, "<a:blip r:embed=" + Quoted(rId) + " cstate=\"print\" />");
                ExportUtils.WriteLn(Out, "<a:srcRect />");
                ExportUtils.WriteLn(Out, "<a:stretch>");
                ExportUtils.WriteLn(Out, "<a:fillRect />");
                ExportUtils.WriteLn(Out, "</a:stretch>");
                ExportUtils.WriteLn(Out, "</a:blipFill>");

            }
            else if (aObj.Fill is PathGradientFill)
            {
                ExportUtils.WriteLn(Out, "<a:blipFill dpi=\"0\" rotWithShape=\"1\">");
                ExportUtils.WriteLn(Out, "<a:blip r:embed=" + Quoted(rId) + " cstate=\"print\" />");
                ExportUtils.WriteLn(Out, "<a:srcRect />");
                ExportUtils.WriteLn(Out, "<a:stretch>");
                ExportUtils.WriteLn(Out, "<a:fillRect />");
                ExportUtils.WriteLn(Out, "</a:stretch>");
                ExportUtils.WriteLn(Out, "</a:blipFill>");
            }
            else if (aObj.Fill is TextureFill)
            {
                //not implemented yet;
            }
            else
            {
                throw new Exception("Unknown fill");
            }

        }

        private string Get_Anchor()
        {
            if(aObj is TextObject) switch ( (aObj as TextObject).VertAlign )
            {
                case VertAlign.Top:     return "\"t\"";
                case VertAlign.Center:  return "\"ctr\"";
                case VertAlign.Bottom:  return "\"b\"";
            }
            if (aObj is TableBase) return "\"ctr\"";
            if (aObj is ShapeObject ||aObj is PolyLineObject) return "\"ctr\"";

            throw new Exception("Bad vertical align");
        }

        private void GetCoords(PointF point, out ulong x, out ulong y)
        {
            x = (ulong)(point.X * PPT_DIVIDER);
            y = (ulong)(point.Y * PPT_DIVIDER);
        }

        private void ExportGdLst(Stream Out, PointF[] points)
        {
            ulong x, y;
            ExportUtils.WriteLn(Out, "<a:gdLst>");
            for (int i = 0; i < points.Length; i++)
            {
                GetCoords(points[i], out x, out y);
                ExportUtils.WriteLn(Out, "<a:gd name=\"connsiteX" + i + "\" fmla=\"*/ " + x + " w " + cX + "\"/>");
                ExportUtils.WriteLn(Out, "<a:gd name=\"connsiteY" + i + "\" fmla=\"*/ " + y + " h " + cY + "\"/>");
            }
            if (Object is PolygonObject)
            {
                GetCoords(points[0], out x, out y);
                ExportUtils.WriteLn(Out, "<a:gd name=\"connsiteX" + points.Length + "\" fmla=\"*/ " + x + " w " + cX + "\"/>");
                ExportUtils.WriteLn(Out, "<a:gd name=\"connsiteY" + points.Length + "\" fmla=\"*/ " + y + " h " + cY + "\"/>");
            }
            ExportUtils.WriteLn(Out, "</a:gdLst>");
        }

        private void ExportPathLst(Stream Out, PointF[] points)
        {
            ulong x, y;
            ExportUtils.WriteLn(Out, "<a:pathLst>");
            ExportUtils.WriteLn(Out, "<a:path w=\"" + cX + "\" h=\"" + cY + "\">");
            for (int i = 0; i < points.Length; i++)
            {
                GetCoords(points[i], out x, out y);
                if (i == 0)
                {
                    ExportUtils.WriteLn(Out, "<a:moveTo>");
                    ExportUtils.WriteLn(Out, "<a:pt x=\"" + x + "\" y=\"" + y + "\"/>");
                    ExportUtils.WriteLn(Out, "</a:moveTo>");
                }
                else
                {
                    ExportUtils.WriteLn(Out, "<a:lnTo>");
                    ExportUtils.WriteLn(Out, "<a:pt x=\"" + x + "\" y=\"" + y + "\"/>");
                    ExportUtils.WriteLn(Out, "</a:lnTo>");
                }
            }
            if (Object is PolygonObject)
            {
                GetCoords(points[0], out x, out y);
                ExportUtils.WriteLn(Out, "<a:lnTo>");
                ExportUtils.WriteLn(Out, "<a:pt x=\"" + x + "\" y=\"" + y + "\"/>");
                ExportUtils.WriteLn(Out, "</a:lnTo>");
                ExportUtils.WriteLn(Out, "<a:close/>");
            }
            ExportUtils.WriteLn(Out, "</a:path>");
            ExportUtils.WriteLn(Out, "</a:pathLst>");
        }

        internal bool Export_XFRM(Stream Out, char Ch)
        {
            bool swap = false;
            bool rotated = false;
            // <a:xfrm>
            if (aObj is LineObject && (aObj as LineObject).Diagonal)
            {
                if (flipV == true) y = y - cY;
                if (flipH == true) x = x - cX;
                ExportUtils.WriteLn(Out, "<" + Ch + ":xfrm flipH=" + Quoted(flipH ? "0" : "1") + " flipV=" + Quoted(flipV ? "0" : "1") + " >");
            }
            else if (aObj is TextObject)
            {
                TextObject obj = aObj as TextObject;
                long dy;
                
                unchecked { dy = ((long)(cY - cX)) / 2; }

                switch (obj.Angle)
                {
                    case 0:
                        ExportUtils.WriteLn(Out, "<" + Ch + ":xfrm>");
                        break;
                    case 90:
                        y = (ulong)((long)y + dy);
                        x = (ulong)((long)x - dy);
                        ExportUtils.WriteLn(Out, "<" + Ch + ":xfrm rot=\"5400000\">");
                        swap = true;
                        break;
                    case 180:
                        ExportUtils.WriteLn(Out, "<" + Ch + ":xfrm rot=\"10800000\">");
                        break;
                    case 270:
                        y = (ulong)((long)y + dy);
                        x = (ulong)((long)x - dy);
                        ExportUtils.WriteLn(Out, "<" + Ch + ":xfrm rot=\"16200000\">");
                        swap = true;
                        break;
                    default:
                        ExportUtils.WriteLn(Out, "<" + Ch + ":xfrm rot=\"" + 60000 * obj.Angle + "\">");
                        rotated = true;
                        break;
                }
            }
            else
                ExportUtils.WriteLn(Out, "<" + Ch + ":xfrm>");

            ExportUtils.WriteLn(Out, "<a:off x=" + QuotedString(x) + " y=" + QuotedString(y) + " />");
            if (swap)
                ExportUtils.WriteLn(Out, "<a:ext cx=" + QuotedString(cY) + " cy=" + QuotedString(cX) + " />");
            else
                ExportUtils.WriteLn(Out, "<a:ext cx=" + QuotedString(cX) + " cy=" + QuotedString(cY) + " />");

            ExportUtils.WriteLn(Out, "</" + Ch + ":xfrm>");

            return rotated;
        }

        internal bool Export_spPr(Stream Out, string PresetGeometry)
        {
            bool do_borders = false;
            bool rotated;

            if (cX != 0 && cY != 0)
            {
                ExportUtils.WriteLn(Out, "<p:spPr>");

                rotated = Export_XFRM(Out, 'a');

                if (PresetGeometry != null)
                {
                    ExportUtils.WriteLn(Out, "<a:prstGeom prst=" + Quoted(PresetGeometry) + ">");
                    ExportUtils.WriteLn(Out, "<a:avLst />");
                    ExportUtils.WriteLn(Out, "</a:prstGeom>");
                }
                else if (Object is PolyLineObject) //polyline or polygon
                {
                    PolyLineObject polyline = Object as PolyLineObject;

                    //transform coords
                    PointF[] points = polyline.PointsArray;
                    for (int i = 0; i < points.Length; i++)
                    {
                        if (points[i].X < 0)
                        {
                            for (int j = 0; j < points.Length; j++)
                            {
                                if (j != i)
                                    points[j].X += Math.Abs(points[i].X);
                            }
                            points[i].X = 0;
                        }
                        if (points[i].Y < 0)
                        {
                            for (int j = 0; j < points.Length; j++)
                            {
                                if (j != i)
                                    points[j].Y += Math.Abs(points[i].Y);
                            }
                            points[i].Y = 0;
                        }
                    }

                    ExportUtils.WriteLn(Out, "<a:custGeom>");
                    ExportUtils.WriteLn(Out, "<a:avLst/>");
                    ExportGdLst(Out, points);
                    ExportUtils.WriteLn(Out, "<a:ahLst/>");
                    ExportPathLst(Out, points);
                    ExportUtils.WriteLn(Out, "</a:custGeom>");
                }

                if (Object.Fill is PathGradientFill || Object.Fill is HatchFill)
                    Object.Fill = new SolidFill(ExportUtils.GetColorFromFill(Object.Fill));            
                Export_Fills(Out);

                do_borders = Export_Borders(Out, rotated);

                if (do_borders)
                {
                    float left, right, top, bottom;

                    if ((aObj.Border.Lines & BorderLines.Left) == BorderLines.Left)
                        left = aObj.Border.LeftLine.Width * PPT_DIVIDER;
                    else left = 0;
                    if ((aObj.Border.Lines & BorderLines.Top) == BorderLines.Top)
                        top = aObj.Border.TopLine.Width * PPT_DIVIDER;
                    else top = 0;
                    if ((aObj.Border.Lines & BorderLines.Right) == BorderLines.Right)
                        right = aObj.Border.RightLine.Width * PPT_DIVIDER;
                    else right = 0;
                    if ((aObj.Border.Lines & BorderLines.Bottom) == BorderLines.Bottom)
                        bottom = aObj.Border.BottomLine.Width * PPT_DIVIDER;
                    else bottom = 0;

                    //X += (ulong)left;
                    //Y += (ulong)top;
                    //CX -= (ulong)(left + right);
                    //CY -= (ulong)(top + bottom);
                }

                ExportUtils.WriteLn(Out, "</p:spPr>");
            }
            else
                ExportUtils.WriteLn(Out, "<p:spPr />");

            return do_borders;
        }

        #region "Export Non-Visual prroperties"
        internal void Export_nvPicPr(Stream Out, int PicCount)
        {
            ExportUtils.WriteLn(Out, "<p:nvPicPr>");
            ExportUtils.WriteLn(Out, "<p:cNvPr id=" + Quoted(id.ToString()) +
                " name=" + Quoted("Picture" + id) + " descr=" + Quoted("image" + PicCount.ToString() + ".png") + " />");
            ExportUtils.WriteLn(Out, "<p:cNvPicPr>");
            ExportUtils.WriteLn(Out, "<a:picLocks noChangeAspect=\"1\" />");
            ExportUtils.WriteLn(Out, "</p:cNvPicPr>");
            ExportUtils.WriteLn(Out, "<p:nvPr />");
            ExportUtils.WriteLn(Out, "</p:nvPicPr>");
        }
        internal void Export_nvSpPr(Stream Out)
        {
            ExportUtils.WriteLn(Out, "<p:nvSpPr>");
            ExportUtils.WriteLn(Out, "<p:cNvPr id=" + Quoted(id.ToString()) + " name=" + Quoted(name) + " />");
            ExportUtils.WriteLn(Out, "<p:cNvSpPr>");
            ExportUtils.WriteLn(Out, "<a:spLocks noGrp=" + Quoted("1") + " />");  // fix me
            ExportUtils.WriteLn(Out, "</p:cNvSpPr>");
            ExportUtils.WriteLn(Out, "<p:nvPr>");

            // Out placeholder
            ExportUtils.Write(Out, "<p:ph ");
            ExportUtils.Write(Out, "/>");

            ExportUtils.WriteLn(Out, "</p:nvPr>");
            ExportUtils.WriteLn(Out, "</p:nvSpPr>");
        }
        internal void Export_nvGraphicFramePr(Stream Out)
        { 
            ExportUtils.Write(Out, "<p:nvGraphicFramePr>");
            ExportUtils.Write(Out, "<p:cNvPr id="+ Quoted(id.ToString()) +" name="+Quoted(name)+" />");
            ExportUtils.Write(Out, "<p:cNvGraphicFramePr>");
            ExportUtils.Write(Out, "<a:graphicFrameLocks noGrp=\"1\" />");
            ExportUtils.Write(Out, "</p:cNvGraphicFramePr>");
            ExportUtils.Write(Out, "<p:nvPr />");
            ExportUtils.Write(Out, "</p:nvGraphicFramePr>");
        }
        internal void Export_nvCxnSpPr(Stream Out)
        {
            ExportUtils.WriteLn(Out, "<p:nvCxnSpPr>");
            ExportUtils.WriteLn(Out, "<p:cNvPr id=" + Quoted(id.ToString()) + " name=" + Quoted(name) + " />");
            ExportUtils.WriteLn(Out, "<p:cNvCxnSpPr />");
            ExportUtils.WriteLn(Out, "<p:nvPr />");
            ExportUtils.WriteLn(Out, "</p:nvCxnSpPr>");
        }
        #endregion

        internal void Export_blipFill(Stream Out)
        {
            ExportUtils.WriteLn(Out, "<p:blipFill>");
            ExportUtils.WriteLn(Out, "<a:blip r:embed=" + Quoted("rId" + this.RelationID.ToString()) + " cstate=\"print\" />");
            ExportUtils.WriteLn(Out, "<a:stretch>");
            ExportUtils.WriteLn(Out, "<a:fillRect />");
            ExportUtils.WriteLn(Out, "</a:stretch>");
            ExportUtils.WriteLn(Out, "</p:blipFill>");
        }


        internal void Open_Paragraph()
        {
            string align = "ctr";
            TextObject text_obj = (aObj is TextObject) ? aObj as TextObject : null;

            if (aObj is TextObject) switch (text_obj.HorzAlign)
                {
                    case HorzAlign.Left:    align ="l"; break;
                    case HorzAlign.Right:   align = "r";  break;
                    case HorzAlign.Center:  align ="ctr"; break;
                    case HorzAlign.Justify: align ="just"; break;
                }
            
            textStrings.AppendLine("<a:p><a:pPr algn=" + Quoted(align) + " />"); 
        }

        internal void Add_Run(
            Font                Font,
            Color               TextColor,
            string              Text
            )
        {
            float FDpiFX = 96f / DrawUtils.ScreenDpi;
            long                Size = (long) (Font.Size * 100 / FDpiFX);
            bool                Italic = Font.Italic;
            bool                Underline = Font.Underline;

            if (Text != null)
            {
                textStrings.AppendLine("<a:r>");
                textStrings.AppendLine("<a:rPr lang=\"en-US\" sz=" + Quoted(Size) +
                    "b=" + Quoted(Font.Bold ? "1" : "0") +
                    "i=" + Quoted(Font.Italic ? "1" : "0") +
                    (Font.Underline ? ("u=" + Quoted("sng")) : "") +
                    " smtClean=\"0\" >");
                textStrings.AppendLine("<a:solidFill><a:srgbClr val=" + GetRGBString(TextColor) + ">");
                if (TextColor.A != Byte.MaxValue) 
                {
                    long alpha = (100000 * TextColor.A) / Byte.MaxValue;
                    textStrings.AppendLine("<a:alpha val=" + Quoted(alpha)  + " />");
                }
                textStrings.AppendLine("</a:srgbClr></a:solidFill>");
                textStrings.AppendLine("<a:latin typeface=" + Quoted(Font.Name) + /*"pitchFamily=" + Quoted( "22" ) +*/ " />");
                textStrings.AppendLine("</a:rPr>");

                textStrings.AppendLine("<a:t>" + this.TranslateText(Text) + "</a:t>");
                textStrings.AppendLine("</a:r>");
            }

            textStrings.AppendLine("<a:r><a:rPr lang=\"en-US\" sz=" + Quoted(Size) + "></a:rPr><a:t> </a:t></a:r>");
        }

        internal void Close_Paragraph()
        {
            textStrings.AppendLine("</a:p>");
        }

        internal void Export_txBody(Stream Out)
        {
            ExportUtils.WriteLn(Out, "<p:txBody>");

            ExportUtils.WriteLn(Out, "<a:bodyPr vert=\"horz\" lIns=\"45720\" tIns=\"22860\" rIns=\"45720\" bIns=\"22860\" rtlCol=\"0\" anchor=" + Get_Anchor() + ">");
            
            ExportUtils.WriteLn(Out, "<a:normAutofit />");
            ExportUtils.WriteLn(Out, "</a:bodyPr>");
            ExportUtils.WriteLn(Out, "<a:lstStyle />");

            ExportUtils.WriteLn(Out, textStrings.ToString());
            
            ExportUtils.WriteLn(Out, "</p:txBody>");
        }

        internal void ResetText()
        {
            textStrings = null;
            textStrings = new StringBuilder();
        }

        internal void MoveObject(ReportComponentBase obj)
        {
            this.aObj = obj;
            this.x = (ulong)(obj.AbsLeft * PPT_DIVIDER + pptExport.LeftMargin);
            this.y = (ulong)(obj.AbsTop * PPT_DIVIDER + pptExport.TopMargin);
            this.cX = (ulong)(obj.Width * PPT_DIVIDER);

            if (obj.Height < 0)
            { this.cY = (ulong)-(obj.Height * PPT_DIVIDER); flipV = true; }
            else { this.cY = (ulong)(obj.Height * PPT_DIVIDER); flipV = false; }

            if (obj.Width < 0)
            { this.cX = (ulong)-(obj.Width * PPT_DIVIDER); flipH = true; }
            else
            { this.cX = (ulong)(obj.Width * PPT_DIVIDER); flipH = false; }

            if (cX == 0) cX = 1588;
            if (cY == 0) cY = 1588;
        }

        private void Export_Line(Stream Out, ulong x, ulong y, ulong dx, ulong dy, Color LineColor, float width, LineStyle style, int id )
        {
            ExportUtils.WriteLn(Out, "<p:cxnSp>");
            ExportUtils.WriteLn(Out, "<p:nvCxnSpPr><p:cNvPr id="+Quoted(id) + " name=\"Straight Connector 61\" /><p:cNvCxnSpPr />");
            ExportUtils.WriteLn(Out, "<p:nvPr />");
            ExportUtils.WriteLn(Out, "</p:nvCxnSpPr>");
            ExportUtils.WriteLn(Out, "<p:spPr>");
            
            ExportUtils.WriteLn(Out, "<a:xfrm>"+
                "<a:off x=" + Quoted(x.ToString()) + " y=" + Quoted(y.ToString()) + " />" + 
                "<a:ext cx=" + Quoted(dx.ToString()) + " cy="+Quoted(dy.ToString())+" />"+
                "</a:xfrm>");

            width *= 12700;

            ExportUtils.WriteLn(Out, "<a:prstGeom prst=\"line\"><a:avLst /> </a:prstGeom>");
            ExportUtils.WriteLn(Out, "<a:ln w="+Quoted(width.ToString())+"><a:solidFill>");
            ExportUtils.WriteLn(Out, "<a:srgbClr val=" + GetRGBString(LineColor) + " />");
            ExportUtils.WriteLn(Out, "</a:solidFill>" + GetDashType(style) + "</a:ln></p:spPr>");
            ExportUtils.WriteLn(Out, "<p:style><a:lnRef idx=\"1\"><a:schemeClr val=\"accent1\" /></a:lnRef><a:fillRef idx=\"0\">");
            ExportUtils.WriteLn(Out, "<a:schemeClr val=\"accent1\" /></a:fillRef><a:effectRef idx=\"0\"><a:schemeClr val=\"accent1\" /></a:effectRef>");
            ExportUtils.WriteLn(Out, "<a:fontRef idx=\"minor\"><a:schemeClr val=\"tx1\" /></a:fontRef></p:style></p:cxnSp>");
        }
        
        internal void Export_Shadow(Stream Out, int id)
        { 
            ulong x, y, cx, cy;

            x = (ulong)((Object.AbsLeft + Object.Border.ShadowWidth /*- 1*/) * PPT_DIVIDER + this.pptExport.LeftMargin);
            y = (ulong)((Object.AbsBottom + Object.Border.ShadowWidth / 2) * PPT_DIVIDER + this.pptExport.TopMargin);
            cx = (ulong)(Object.Width * PPT_DIVIDER);
            Export_Line(Out, x, y, cx, 0, Object.Border.ShadowColor, Object.Border.ShadowWidth, LineStyle.Solid, id++ );

            x = (ulong)((Object.AbsRight + Object.Border.ShadowWidth / 2) * PPT_DIVIDER + this.pptExport.LeftMargin);
            y = (ulong)((Object.AbsTop + Object.Border.ShadowWidth /*- 1*/) * PPT_DIVIDER + this.pptExport.TopMargin);
            cy = (ulong)(Object.Height * PPT_DIVIDER);
            Export_Line(Out, x, y, 0, cy, Object.Border.ShadowColor, Object.Border.ShadowWidth, LineStyle.Solid, id);
        }
    };

    /// <summary>
    /// Power Point Layout Descriptor
    /// </summary>
    internal class PptLayoutDescriptor
    {
        public string name;
        public string type;
        public PptShape[] shapes;

        public PptLayoutDescriptor(string Type, string Name, PptShape[] Shapes) 
        {
            this.type = Type;
            this.name = Name;
            this.shapes = Shapes;
        }
    }

    /// <summary>
    /// Power Point base class for style element
    /// </summary>
    internal class PptStyleBase
    {
        private uint level = 0;
        private long marL=342900;
        private long indent = -342900;
        private char algn = 'l';
        private long defTabSz = 914400;
        private long rtl = 0;
        private long eaLnBrk = 1;
        private long latinLnBrk = 0;
        private long hangingPunct = 1;

        private string Quoted(long v) { return "\"" + v.ToString() + "\" "; }
        private string Quoted(char v) { return "\"" + v + "\" "; }
        private string Quoted(string v) { return "\"" + v + "\" "; }

        internal void Export(Stream Out)
        { 
            ExportUtils.WriteLn(Out, "<a:lvl" + level.ToString() + "pPr ");
            ExportUtils.WriteLn(Out, "marL=" + Quoted(marL));
            ExportUtils.WriteLn(Out, "indent=" + Quoted(indent));
            ExportUtils.WriteLn(Out, "algn=" + Quoted(algn));
            ExportUtils.WriteLn(Out, "defTabSz=" + Quoted(defTabSz));
            ExportUtils.WriteLn(Out, "rtl=" + Quoted(rtl));
            ExportUtils.WriteLn(Out, "eaLnBrk=" + Quoted(eaLnBrk));
            ExportUtils.WriteLn(Out, "latinLnBrk=" + Quoted(latinLnBrk));
            ExportUtils.WriteLn(Out, "hangingPunct=" + Quoted(hangingPunct));
            ExportUtils.WriteLn(Out, ">");

            ExportUtils.WriteLn(Out, "<a:spcBef>");
            ExportUtils.WriteLn(Out, "<a:spcPct val=" + Quoted("0") + "/>");
            ExportUtils.WriteLn(Out, "</a:spcBef>");
            
            ExportUtils.WriteLn(Out, "<a:buNone />");

            ExportUtils.WriteLn(Out, "<a:defRPr sz=" + Quoted("4400") + "kern=" + Quoted("1200") + ">");
            ExportUtils.WriteLn(Out, "<a:solidFill>");
            ExportUtils.WriteLn(Out, "<a:schemeClr val=" + Quoted("tx1") + "/>");
            ExportUtils.WriteLn(Out, "</a:solidFill>");
            ExportUtils.WriteLn(Out, "<a:latin typeface="  + Quoted("+mj-lt") + "/>");
            ExportUtils.WriteLn(Out, "<a:ea typeface="     + Quoted("+mj-ea") + "/>");
            ExportUtils.WriteLn(Out, "<a:cs typeface="     + Quoted("+mj-cs") + "/>");
            ExportUtils.WriteLn(Out, "</a:defRPr>");

            ExportUtils.WriteLn(Out, "</a:lvl" + level.ToString() + "pPr>");
        }

        internal PptStyleBase(long MarL)
        {
            this.marL = MarL;
        }
    }

    /// <summary>
    /// Base class for styles group
    /// </summary>
    internal class PptStyleGroupBase
    {
        private ArrayList styleGroup = new ArrayList();

        internal void Export(Stream Out)
        {
            foreach (PptStyleBase style in styleGroup)
            {
                style.Export(Out);
            }
        }

        internal void AddStyle(PptStyleBase style)
        {
            styleGroup.Add(style);
        }
    }

    /// <summary>
    /// Base class for slides, masters, and layouts
    /// </summary>
    internal abstract class OoSlideBase : OoXMLBase
    {
        protected PowerPoint2007Export pptExport;

        protected ulong slideMasterId;
        
        protected void ExportShape(Stream Out, PptShape shape, string ShapeType, int id)
        {
            bool do_borders;
            ExportUtils.WriteLn(Out, "<p:sp>");
            shape.Export_nvSpPr(Out);
            do_borders = shape.Export_spPr(Out, ShapeType);
            shape.Export_txBody(Out);
            ExportUtils.WriteLn(Out, "</p:sp>");

            if (shape.Object.Border.Shadow)
            {
                shape.Export_Shadow(Out, id);
                id += 2;
            }

            if (do_borders == true)
            {
                shape.ExportFourBorders(Out, id);
                id += 4;
            }
        }

        protected void ExportColorMapOverride(Stream Out)
        {
            ExportUtils.WriteLn(Out, "<p:clrMapOvr>");
            ExportUtils.WriteLn(Out, "<a:masterClrMapping />");
            ExportUtils.WriteLn(Out, "</p:clrMapOvr>");
        }

        protected void ExportShapeTree(Stream Out, PptShape[] shape_list)
        {
            ExportUtils.WriteLn(Out, "<p:spTree>");

            ExportUtils.WriteLn(Out, "<p:nvGrpSpPr>");
            ExportUtils.WriteLn(Out, "<p:cNvPr id=\"1\" name=\"\" />");
            ExportUtils.WriteLn(Out, "<p:cNvGrpSpPr />");
            ExportUtils.WriteLn(Out, "<p:nvPr />");
            ExportUtils.WriteLn(Out, "</p:nvGrpSpPr>");

            ExportUtils.WriteLn(Out, "<p:grpSpPr>");
            ExportUtils.WriteLn(Out, "<a:xfrm>");
            ExportUtils.WriteLn(Out, "<a:off x=\"0\" y=\"0\" />");
            ExportUtils.WriteLn(Out, "<a:ext cx=\"0\" cy=\"0\" />");
            ExportUtils.WriteLn(Out, "<a:chOff x=\"0\" y=\"0\" />");
            ExportUtils.WriteLn(Out, "<a:chExt cx=\"0\" cy=\"0\" />");
            ExportUtils.WriteLn(Out, "</a:xfrm>");
            ExportUtils.WriteLn(Out, "</p:grpSpPr>");

#if false
            for (int i = 0; i < shape_list.Length; i++)
            {
                ExportShape(Out, shape_list[i]);
            }
#endif

            ExportUtils.WriteLn(Out, "</p:spTree>");
        }

        protected void ExportSlideBackground(Stream Out)
        {
            ExportUtils.WriteLn(Out, "<p:bg>");
            ExportUtils.WriteLn(Out, "<p:bgRef idx=\"1001\">");
            ExportUtils.WriteLn(Out, "<a:schemeClr val=\"bg1\" />");
            ExportUtils.WriteLn(Out, "</p:bgRef>");
            ExportUtils.WriteLn(Out, "</p:bg>");
        }

        internal ulong SlideMasterId { get { return slideMasterId; } }

        internal OoSlideBase(PowerPoint2007Export ppt_export)
        {
            pptExport = ppt_export;
            slideMasterId = this.pptExport.slideMasterID;
            this.pptExport.slideMasterID++;
        }
    }

    /// <summary>
    /// Slide masters object
    /// </summary>
    internal class OoPptSlideMaster : OoSlideBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideMaster"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.presentationml.slideMaster+xml"; } }
        public override string FileName { get { return "ppt/slideMasters/slideMaster1.xml"; } }
        #endregion

        #region Private methods

        private void ExportLayoutIDList( Stream Out, ArrayList LayoutList )
        {
            ExportUtils.WriteLn(Out, "<p:sldLayoutIdLst>");

            foreach (OoSlideBase layout_item in LayoutList)
            {
                ExportUtils.WriteLn(Out, "<p:sldLayoutId id=" + Quoted(layout_item.SlideMasterId.ToString()) + "r:id=" + Quoted(layout_item.rId) + "/>");
            }

            ExportUtils.WriteLn(Out, "</p:sldLayoutIdLst>");
        }

        private void ExportTitleStyles(Stream Out)
        {
            ExportUtils.WriteLn(Out, "<p:titleStyle>");
#if false
            ExportUtils.WriteLn(Out, "<a:lvl1pPr algn=\"ctr\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\">");
            ExportUtils.WriteLn(Out, "<a:spcBef>");
            ExportUtils.WriteLn(Out, "<a:spcPct val=\"0\" />");
            ExportUtils.WriteLn(Out, "</a:spcBef>");
            ExportUtils.WriteLn(Out, "<a:buNone />");
            ExportUtils.WriteLn(Out, "<a:defRPr sz=\"4400\" kern=\"1200\">");
            ExportUtils.WriteLn(Out, "<a:solidFill>");
            ExportUtils.WriteLn(Out, "<a:schemeClr val=\"tx1\" />");
            ExportUtils.WriteLn(Out, "</a:solidFill>");
            ExportUtils.WriteLn(Out, "<a:latin typeface=\"+mj-lt\" />");
            ExportUtils.WriteLn(Out, "<a:ea typeface=\"+mj-ea\" />");
            ExportUtils.WriteLn(Out, "<a:cs typeface=\"+mj-cs\" />");
            ExportUtils.WriteLn(Out, "</a:defRPr>");
            ExportUtils.WriteLn(Out, "</a:lvl1pPr>");
#endif
            ExportUtils.WriteLn(Out, "</p:titleStyle>");
        }

        private void ExportBodyStyles(Stream Out)
        {
            ExportUtils.WriteLn(Out, "<p:bodyStyle>");
#if false
            for (int i = 0; i < diffs.Length; i++)
            {
                ExportUtils.WriteLn(Out, "<a:lvl" + (1 + i) + "pPr marL=" + Quoted(diffs[i].MarL) + "indent=" + Quoted(diffs[i].ident) + "algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\">");
                ExportUtils.WriteLn(Out, "<a:spcBef>");
                ExportUtils.WriteLn(Out, "<a:spcPct val=\"20000\" />");
                ExportUtils.WriteLn(Out, "</a:spcBef>");
                ExportUtils.WriteLn(Out, "<a:buFont typeface=\"Arial\" pitchFamily=\"34\" charset=\"0\" />");
                ExportUtils.WriteLn(Out, "<a:buChar char=" + Quoted(diffs[i].buChar) + " />");
                ExportUtils.WriteLn(Out, "<a:defRPr sz=" + Quoted(diffs[i].sz) + " kern=\"1200\">");
                ExportUtils.WriteLn(Out, "<a:solidFill>");
                ExportUtils.WriteLn(Out, "<a:schemeClr val=\"tx1\" />");
                ExportUtils.WriteLn(Out, "</a:solidFill>");
                ExportUtils.WriteLn(Out, "<a:latin typeface=\"+mn-lt\" />");
                ExportUtils.WriteLn(Out, "<a:ea typeface=\"+mn-ea\" />");
                ExportUtils.WriteLn(Out, "<a:cs typeface=\"+mn-cs\" />");
                ExportUtils.WriteLn(Out, "</a:defRPr>");
                ExportUtils.WriteLn(Out, "</a:lvl" + (1 + i) + "pPr>");
            }
#endif
            ExportUtils.WriteLn(Out, "</p:bodyStyle>");
        }

        private void ExportOtherStyles(Stream Out)
        {
            ExportUtils.WriteLn(Out, "<p:otherStyle>");
            ExportUtils.WriteLn(Out, "<a:defPPr>");
            ExportUtils.WriteLn(Out, "<a:defRPr lang=\"en-US\" />");
            ExportUtils.WriteLn(Out, "</a:defPPr>");
#if false
            for (int i = 0; i < MarL.Length; i++)
            {
                ExportUtils.WriteLn(Out, "<a:lvl" + (1 + i) + "pPr marL=" + MarL[i] + " algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\">");
                ExportUtils.WriteLn(Out, "<a:defRPr sz=\"1800\" kern=\"1200\">");
                ExportUtils.WriteLn(Out, "<a:solidFill>");
                ExportUtils.WriteLn(Out, "<a:schemeClr val=\"tx1\" />");
                ExportUtils.WriteLn(Out, "</a:solidFill>");
                ExportUtils.WriteLn(Out, "<a:latin typeface=\"+mn-lt\" />");
                ExportUtils.WriteLn(Out, "<a:ea typeface=\"+mn-ea\" />");
                ExportUtils.WriteLn(Out, "<a:cs typeface=\"+mn-cs\" />");
                ExportUtils.WriteLn(Out, "</a:defRPr>");
                ExportUtils.WriteLn(Out, "</a:lvl" + (1 + i) + "pPr>");
            }
#endif
            ExportUtils.WriteLn(Out, "</p:otherStyle>");
        }

        #endregion

        #region Internal methods
        internal void Export(PowerPoint2007Export OoXML)
        {
            ExportRelations(OoXML);

            MemoryStream file = new MemoryStream(); 

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<p:sldMaster xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:p=\"http://schemas.openxmlformats.org/presentationml/2006/main\">");
            ExportUtils.WriteLn(file, "<p:cSld>");
            ExportSlideBackground(file);
            ExportShapeTree(file, null /*master_shapes*/ );
            ExportUtils.WriteLn(file, "</p:cSld>");
            ExportUtils.WriteLn(file, "<p:clrMap bg1=\"lt1\" tx1=\"dk1\" bg2=\"lt2\" tx2=\"dk2\" accent1=\"accent1\" accent2=\"accent2\" accent3=\"accent3\" accent4=\"accent4\" accent5=\"accent5\" accent6=\"accent6\" hlink=\"hlink\" folHlink=\"folHlink\" />");
            ExportLayoutIDList(file, OoXML.SlideLayoutList);

            ExportUtils.WriteLn(file, "<p:txStyles>");
            ExportTitleStyles(file);
            ExportBodyStyles(file);
            ExportOtherStyles(file);
            ExportUtils.WriteLn(file, "</p:txStyles>");

            ExportUtils.WriteLn(file, "</p:sldMaster>");

            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);            
        }
        #endregion

        internal OoPptSlideMaster(PowerPoint2007Export ppt_export)
            : base(ppt_export)
        { 
        }

    }

    /// <summary>
    /// Ordinaty slide 
    /// </summary>
    internal class OoPptSlide : OoSlideBase
    {
        public Dictionary<string, PptShape> checkboxList = new Dictionary<string, PptShape>();

        const float PPT_DIVIDER = 360000 / 37.8f; 
        const int NO_RELATION = 0;

        #region "Class overrides"
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.presentationml.slide+xml"; } }
        public override string FileName { get { return "ppt/slides/slide" + slideNumber +".xml"; } }
        #endregion
    
        #region "Private fields"
        private int slideNumber;
        private int slideID;
        private int id { get { return pptExport.identifier; } set { pptExport.identifier = value; } }
        #endregion

        #region "Public properties"
        public int SlideID { get { return slideID; } }
        public int PictureCount { get { return pptExport.PictureCount; } set { pptExport.PictureCount = value; } }
        #endregion

        #region "Private methods"
        private ulong GetTop(float p)
        {
            return (ulong) ( /* FMarginWoBottom - */ p * PPT_DIVIDER );
        }

        private ulong GetLeft(float p)
        {
            return (ulong) ( /* FMarginLeft + */ p * PPT_DIVIDER );
        }
        #endregion

        // Constructor
        internal OoPptSlide(PowerPoint2007Export ppt_export) : base(ppt_export)
        {
            slideNumber = ++pptExport.SlideCount;
            slideID = ++pptExport.SlideIDCount;
            id = 1;
        }

        internal void Reset()
        {
            pptExport.SlideCount = 1;
            slideNumber = 1;

            slideID = pptExport.SlideIDCount = 256;
            pptExport.slideMasterID = 2147483648;
        }

        private void AddBandObject(Stream outstream, BandBase band)
        {
            if (band.HasBorder || band.HasFill ) using (TextObject newObj = new TextObject())
            {
                newObj.Left = band.AbsLeft;
                newObj.Top = band.AbsTop;
                newObj.Width = band.Width;
                newObj.Height = band.Height;
                newObj.Fill = band.Fill;
                newObj.Border = band.Border;
                newObj.Text = "";
                AddTextObject(outstream, 0, newObj);
            }
        }

        private void AddPolyline(Stream Out, PolyLineObject polyline)
        {
            PptShape shape = new PptShape(id + 1, NO_RELATION, polyline.ToString() + " " + id, polyline, this.pptExport);

            using (Font f = new Font("system", 8))
            {
                // append epmty space to avoid anoying PPt notification
                shape.Open_Paragraph();
                shape.Add_Run(f, polyline.FillColor, null);
                shape.Close_Paragraph();
            }
            ExportShape(Out, shape, null, id);
            id += 8;
        }

        private void AddShape(Stream Out, ShapeObject shape_object)
        {
            string shape_name;

            PptShape shape = new PptShape(id + 1, NO_RELATION, shape_object.ToString() + " " + id, shape_object, this.pptExport);

            switch (shape_object.Shape)
            {
                case ShapeKind.Diamond: shape_name = "diamond"; break;
                case ShapeKind.Ellipse: shape_name = "ellipse"; break;
                case ShapeKind.Rectangle: shape_name = "rect"; break;
                case ShapeKind.RoundRectangle: shape_name = "roundRect"; break;
                case ShapeKind.Triangle: shape_name = "triangle"; break;
                default: throw new Exception("Unsupported shape kind");
            }

            using (Font f = new Font("system", 8))
            {
                // append epmty space to avoid anoying PPt notification
                shape.Open_Paragraph();
                shape.Add_Run(f, shape_object.FillColor, null);
                shape.Close_Paragraph();
            }

            ExportShape(Out, shape, shape_name, id);
            id+=8;
        }

        private void AddTextObject(Stream outstream, int nRelationID, TextObject obj)
        {
            PptShape shape;
            if (obj.Fill is HatchFill || obj.Fill is PathGradientFill)
            {
                string TempString = obj.Text;
                obj.Text = "";
                shape = SaveImage(obj, nRelationID, "ppt/media/HatchFill", false);
                this.AddRelation(nRelationID, shape);
                obj.Text = TempString;
            }
            else
            {
                shape = new PptShape(id + 1, NO_RELATION, "TextBox " + id, obj, pptExport);
            }


            float FDpiFX = 96f / DrawUtils.ScreenDpi;


            IGraphics g = pptExport.Report.MeasureGraphics;
            using (Font f = new Font(obj.Font.Name, obj.Font.Size * FDpiFX, obj.Font.Style))
            using (GraphicCache cache = new GraphicCache())
            {
                RectangleF textRect = new RectangleF(
                  obj.AbsLeft + obj.Padding.Left,
                  obj.AbsTop + obj.Padding.Top,
                  obj.Width - obj.Padding.Horizontal,
                  obj.Height - obj.Padding.Vertical);

                StringFormat format = obj.GetStringFormat(cache, 0);
                Brush textBrush = cache.GetBrush(obj.TextColor);
                AdvancedTextRenderer renderer = new AdvancedTextRenderer(obj.Text, g, f, textBrush, null,
                    textRect, format, obj.HorzAlign, obj.VertAlign, obj.LineHeight, obj.Angle, obj.FontWidthRatio,
                    obj.ForceJustify, obj.Wysiwyg, obj.HasHtmlTags, true, FDpiFX, FDpiFX, obj.InlineImageCache);

                float w = f.Height * 0.1f; // to match .net char X offset
                // render
                if (renderer.Paragraphs.Count == 0)
                {
                    // append empty space
                    shape.Open_Paragraph();
                    shape.Add_Run(f, obj.TextColor, null);
                    shape.Close_Paragraph();
                }
                else foreach (AdvancedTextRenderer.Paragraph paragraph in renderer.Paragraphs)
                    {
                        shape.Open_Paragraph();
                        foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                        {
                            foreach (AdvancedTextRenderer.Word word in line.Words)
                                if (renderer.HtmlTags)
                                    foreach (AdvancedTextRenderer.Run run in word.Runs)
                                        using (Font fnt = run.GetFont())
                                        {
                                            shape.Add_Run(fnt, run.Style.Color, run.Text);
                                        }
                                else
                                    shape.Add_Run(f, obj.TextColor, word.Text);
                        }
                        shape.Close_Paragraph();
                    }
            }

            ExportShape(outstream, shape, "rect", id);
            id+= 6;
        }

        private void AddLine(Stream Out, LineObject obj)
        {
            PptShape shape = new PptShape(id + 1, NO_RELATION, "LineObject " + id, obj, pptExport);

            ExportUtils.WriteLn(Out, "<p:cxnSp>");

            shape.Export_nvCxnSpPr(Out);

            if (obj.Diagonal) 
                shape.Export_spPr(Out, "line"); 
            else 
                shape.Export_spPr(Out, "straightConnector1");

            //ExportUtils.WriteLn(Out, "<a:ln>");
            //ExportUtils.WriteLn(Out, "<a:tailEnd type="arrow" />");
            //ExportUtils.WriteLn(Out, "</a:ln>");
            //ExportUtils.WriteLn(Out, "</p:spPr>"); <<-- fix it

            ExportUtils.WriteLn(Out, "<p:style>");
            ExportUtils.WriteLn(Out, "<a:lnRef idx=\"1\">");
            ExportUtils.WriteLn(Out, "<a:schemeClr val=\"accent1\" />");
            ExportUtils.WriteLn(Out, "</a:lnRef>");
            ExportUtils.WriteLn(Out, "<a:fillRef idx=\"0\">");
            ExportUtils.WriteLn(Out, "<a:schemeClr val=\"accent1\" />");
            ExportUtils.WriteLn(Out, "</a:fillRef>");
            ExportUtils.WriteLn(Out, "<a:effectRef idx=\"0\">");
            ExportUtils.WriteLn(Out, "<a:schemeClr val=\"accent1\" />");
            ExportUtils.WriteLn(Out, "</a:effectRef>");
            ExportUtils.WriteLn(Out, "<a:fontRef idx=\"minor\">");
            ExportUtils.WriteLn(Out, "<a:schemeClr val=\"tx1\" />");
            ExportUtils.WriteLn(Out, "</a:fontRef>");
            ExportUtils.WriteLn(Out, "</p:style>");

            ExportUtils.WriteLn(Out, "</p:cxnSp>");

            id++;
        }

        // Save any object as image file
        private PptShape SaveImage(ReportComponentBase obj, int rId, string FileName, bool ClearBackground)
        {
            return SaveImage(obj, rId, FileName, ClearBackground, 1);
        }
        // Save any object as image file
        private PptShape SaveImage(ReportComponentBase obj, int rId, string FileName, bool ClearBackground, float printZoom)
        {
            pptExport.PictureCount++; // Increase picture counter

            string file_extension = "png";
            System.Drawing.Imaging.ImageFormat image_format = System.Drawing.Imaging.ImageFormat.Png;
            if (this.pptExport.ImageFormat == PptImageFormat.Jpeg)
            {
                file_extension = "jpg";
                image_format = System.Drawing.Imaging.ImageFormat.Jpeg;
            }

            string ImageFileName = FileName + pptExport.PictureCount.ToString() + "." + file_extension;

            using (System.Drawing.Image image = new System.Drawing.Bitmap((int)Math.Round(obj.Width * printZoom), (int)Math.Round(obj.Height * printZoom)))
            using (Graphics g = Graphics.FromImage(image))
            using (GraphicCache cache = new GraphicCache())
            {
                g.TranslateTransform(-obj.AbsLeft * printZoom, -obj.AbsTop * printZoom);
                if (ClearBackground)
                {
                    g.Clear(Color.White);
                }
                obj.Draw(new FRPaintEventArgs(g, printZoom, printZoom, cache));
                MemoryStream ms = new MemoryStream();
                image.Save(ms, image_format);
                ms.Position = 0;
                pptExport.Zip.AddStream(ImageFileName, ms);
            }

            return new PptShape(id + 1, rId, ImageFileName, obj, pptExport);
        }

        private void AddCheckboxObject(Stream Out, int rId, CheckBoxObject checkbox, out int rIdOut)
        {
            PptShape shape;
            string KEY = checkbox.Name + checkbox.Checked.ToString();

            if (!checkboxList.ContainsKey(KEY))
            {
                rId++;

                shape = SaveImage(checkbox, rId, "ppt/media/Checkbox", false);
                this.AddRelation(rId, shape);
                checkboxList.Add(KEY, shape);
            }
            else
            {
                shape = checkboxList[KEY];
                shape.MoveObject(checkbox);
            }

            ExportUtils.WriteLn(Out, "<p:pic>");
            shape.Export_nvPicPr(Out, id);
            shape.Export_blipFill(Out);
            shape.Export_spPr(Out, "rect");
            ExportUtils.WriteLn(Out, "</p:pic>");

            rIdOut = rId;
            id++;
        }

        private void AddPictureObject(Stream Out, int rId, ReportComponentBase obj, string FileName)
        {
            PptShape shape = SaveImage(obj, rId, FileName, !(obj is PictureObject), 3);

            ExportUtils.WriteLn(Out, "<p:pic>");
            shape.Export_nvPicPr(Out, pptExport.PictureCount);
            shape.Export_blipFill(Out);
            shape.Export_spPr(Out, "rect");
            ExportUtils.WriteLn(Out, "</p:pic>");

            this.AddRelation(rId, shape);

            id++;
        }

        private void AddTable(Stream Out, TableBase table)
        {
            using (TextObject tableBack = new TextObject())
            {
                tableBack.Left = table.AbsLeft;
                tableBack.Top = table.AbsTop;
                float tableWidth = 0;
                for (int i = 0; i < table.ColumnCount; i++)
                    tableWidth += table[i, 0].Width;
                tableBack.Width = (tableWidth < table.Width) ? tableWidth : table.Width;
                tableBack.Height = table.Height;
                tableBack.Fill = table.Fill;
                tableBack.Text = "";

                // exporting the table fill
                AddTextObject(Out, 0, tableBack);

                // exporting the table cells
                float x = 0;
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    float y = 0;
                    for (int i = 0; i < table.RowCount; i++)
                    {
                        if (!table.IsInsideSpan(table[j, i]))
                        {
                            TableCell textcell = table[j, i];

                            textcell.Left = x;
                            textcell.Top = y;

                            AddTextObject(Out, 0, textcell);
                        }
                        y += (table.Rows[i]).Height;
                    }
                    x += (table.Columns[j]).Width;
                }

                // exporting the table border
                tableBack.Fill = new SolidFill();
                tableBack.Border = table.Border;
                AddTextObject(Out, 0, tableBack);
            }
        }

        private MemoryStream file;
        private int r_Id;
        internal void ExportBegin(PowerPoint2007Export OoXML, ReportPage page)
        {
            r_Id = 0;

            foreach (OoPptSlideLayout layout in OoXML.SlideLayoutList)
            {
                AddRelation( ++r_Id, OoXML.SlideLayoutList[0] as OoPptSlideLayout);
            }

            // Export slide
            file = new MemoryStream(); 

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<p:sld xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:p=\"http://schemas.openxmlformats.org/presentationml/2006/main\">");
            ExportUtils.WriteLn(file, "<p:cSld>");
            ExportUtils.WriteLn(file, "<p:spTree>");

            ExportUtils.WriteLn(file, "<p:nvGrpSpPr>");
            ExportUtils.WriteLn(file, "<p:cNvPr id=\"1\" name=\"\" />");
            ExportUtils.WriteLn(file, "<p:cNvGrpSpPr />");
            ExportUtils.WriteLn(file, "<p:nvPr />");
            ExportUtils.WriteLn(file, "</p:nvGrpSpPr>");

            ExportUtils.WriteLn(file, "<p:grpSpPr>");
            ExportUtils.WriteLn(file, "<a:xfrm>");
            ExportUtils.WriteLn(file, "<a:off x=\"0\" y=\"0\" />");
            ExportUtils.WriteLn(file, "<a:ext cx=\"0\" cy=\"0\" />");
            ExportUtils.WriteLn(file, "<a:chOff x=\"0\" y=\"0\" />");
            ExportUtils.WriteLn(file, "<a:chExt cx=\"0\" cy=\"0\" />");
            ExportUtils.WriteLn(file, "</a:xfrm>");
            ExportUtils.WriteLn(file, "</p:grpSpPr>");

            // text watermark on bottom
            if (page.Watermark.Enabled && !page.Watermark.ShowTextOnTop)
                AddTextWatermark(file, page);
        }

        internal void ExportBand(PowerPoint2007Export OoXML, Base band)
        {
            if ((band as BandBase).Fill is TextureFill)
                AddPictureObject(file, ++r_Id, band as BandBase, "ppt/media/FixMeImage");
            else
                AddBandObject(file, band as BandBase);
            foreach (Base c in band.ForEachAllConvectedObjects(this))
            {
                ReportComponentBase obj = c as ReportComponentBase;
                if (obj == null)
                    continue;
                else if (obj.Fill is TextureFill)
                {
                    AddPictureObject(file, ++r_Id, obj as ReportComponentBase, "ppt/media/FixMeImage");
                    continue;
                }
                if (obj is CellularTextObject)
                    obj = (obj as CellularTextObject).GetTable();
                if (obj is TableCell)
                    continue;
                else if (obj is TableBase)
                    AddTable(file, obj as TableBase);
                else if (obj is TextObject)
                    AddTextObject(file, ++r_Id, obj as TextObject);
                else if (obj is BandBase)
                    AddBandObject(file, obj as BandBase);
                else if (obj is LineObject)
                    AddLine(file, obj as LineObject);
                else if (obj is ShapeObject)
                    AddShape(file, obj as ShapeObject);
                else if (obj is PolyLineObject)
                    AddPolyline(file, obj as PolyLineObject);
                else if (obj is PictureObject)
                    AddPictureObject(file, ++r_Id, obj as PictureObject, "ppt/media/image");
                else if (obj is Barcode.BarcodeObject)
                    AddPictureObject(file, ++r_Id, obj as ReportComponentBase, "ppt/media/BarcodeImage");
                else if (obj is ZipCodeObject)
                    AddPictureObject(file, ++r_Id, obj as ReportComponentBase, "ppt/media/ZipCodeImage");
#if MSCHART
                else if (obj is MSChart.MSChartObject)
                    AddPictureObject(file, ++r_Id, obj as ReportComponentBase, "ppt/media/MSChartImage");
#endif
                else if (obj is RichObject)
                    AddPictureObject(file, ++r_Id, obj as ReportComponentBase, "ppt/media/RichTextImage");
                else if (obj is CheckBoxObject)
                    AddCheckboxObject(file, r_Id, obj as CheckBoxObject, out r_Id);
                else
                {
                    AddPictureObject(file, ++r_Id, obj as ReportComponentBase, "ppt/media/FixMeImage");
                }
            }
        }

        internal void ExportEnd(PowerPoint2007Export OoXML, ReportPage page)
        {
            // text watermark on top
            if (page.Watermark.Enabled && page.Watermark.ShowTextOnTop)
                AddTextWatermark(file, page);
            ExportUtils.WriteLn(file, "</p:spTree>");
            ExportUtils.WriteLn(file, "</p:cSld>");
            ExportColorMapOverride(file);
            ExportUtils.WriteLn(file, "</p:sld>");
            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
            ExportRelations(OoXML);
        }

        private void AddTextWatermark(Stream Out, ReportPage page)
        {
            TextObject obj = page.Watermark.TextObject;
            if (obj == null) return;


            RectangleF displayRect = new RectangleF(
                page.LeftMargin * 3,
                page.TopMargin * 3,
                ExportUtils.GetPageWidth(page) * 3,
                ExportUtils.GetPageHeight(page) * 3);

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

            AddTextObject(Out, 0, obj);
        }
    }

    /// <summary>
    /// Slide layout object
    /// </summary>
    internal class OoPptSlideLayout : OoSlideBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideLayout"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.presentationml.slideLayout+xml"; } }
        public override string FileName { get { return "ppt/slideLayouts/slideLayout" + index.ToString() + ".xml"; } }
        #endregion

        #region Private fields
        private PptLayoutDescriptor descriptor;
        private int index;
        #endregion

        internal void Export(PowerPoint2007Export OoXML)
        {
            ExportRelations( OoXML );

            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<p:sldLayout ");
            ExportUtils.WriteLn(file, "xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" ");
            ExportUtils.WriteLn(file, "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" ");
            ExportUtils.WriteLn(file, "xmlns:p=\"http://schemas.openxmlformats.org/presentationml/2006/main\" type=" + Quoted(descriptor.type) + " preserve=\"1\">");

            ExportUtils.WriteLn(file, "<p:cSld name=" + Quoted(descriptor.name) + ">");
            ExportShapeTree(file, descriptor.shapes);
            ExportUtils.WriteLn(file, "</p:cSld>");
            ExportColorMapOverride(file);
            ExportUtils.WriteLn(file, "</p:sldLayout>");

            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }

        // Constructor
        internal OoPptSlideLayout(PowerPoint2007Export ppt_export, int Index, PptLayoutDescriptor descriptor) :
            base(ppt_export)
        {
            this.descriptor = descriptor;
            index = Index;
        }
    }

    /// <summary>
    /// Presentation class
    /// </summary>
    internal class OoPptPresentation : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.presentationml.presentation.main+xml"; } }
        public override string FileName { get { return "ppt/presentation.xml"; } }
        #endregion

        internal void Export(PowerPoint2007Export OoXML)
        {

            ExportRelations(OoXML);

            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<p:presentation xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:p=\"http://schemas.openxmlformats.org/presentationml/2006/main\" saveSubsetFonts=\"1\">");
            ExportUtils.WriteLn(file, "<p:sldMasterIdLst>");
            ExportUtils.WriteLn(file, "<p:sldMasterId id=" + Quoted(OoXML.SlideMaster.SlideMasterId.ToString()) + "r:id=" + Quoted(OoXML.SlideMaster.rId) + "/> ");
            ExportUtils.WriteLn(file, "</p:sldMasterIdLst>");

            ExportUtils.WriteLn(file, "<p:sldIdLst>");
            foreach (OoXMLBase obj in RelationList) if (obj is OoPptSlide)
                {
                    OoPptSlide slide = obj as OoPptSlide;
                    ExportUtils.WriteLn(file, "<p:sldId id=" + Quoted(slide.SlideID) + " r:id=" + Quoted(slide.rId) + " />");
                }
            ExportUtils.WriteLn(file, "</p:sldIdLst>");

            ExportUtils.WriteLn(file, "<p:sldSz cx=" + Quoted(OoXML.PaperWidth) + " cy=" + Quoted(OoXML.PaperHeight) + " type=\"custom\" /> ");

            ExportUtils.WriteLn(file, "<p:notesSz cx=\"6858000\" cy=\"9144000\" /> ");
            //
            ExportUtils.WriteLn(file, "<p:defaultTextStyle>");
            ExportUtils.WriteLn(file, "<a:defPPr>");
            ExportUtils.WriteLn(file, "<a:defRPr lang=\"en-US\" />");
            ExportUtils.WriteLn(file, "</a:defPPr>");
            ExportUtils.WriteLn(file, "</p:defaultTextStyle>");
            //
            ExportUtils.WriteLn(file, "</p:presentation>");

            file.Position = 0;
            OoXML.Zip.AddStream("ppt/presentation.xml", file);
        }

    }

    /// <summary>
    /// PPt Application Properties class
    /// </summary>
    internal class OoPptApplicationProperties : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.extended-properties+xml"; } }
        public override string FileName { get { return "docProps/app.xml"; } }
        #endregion

        public void Export(OOExportBase OoXML)
        {
            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, 
                "<Properties xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties\" xmlns:vt=\"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes\">" +
                "<TotalTime>1</TotalTime>" +
                "<Words>0</Words>" +
                "<Application>Microsoft Office PowerPoint</Application>" +
                "<PresentationFormat>On-screen Show (4:3)</PresentationFormat>" +
                "<Paragraphs>0</Paragraphs>" +
                "<Slides>0</Slides>" +
                "<Notes>0</Notes> " +
                "<HiddenSlides>0</HiddenSlides>" +
                "<MMClips>0</MMClips>" +
                "<ScaleCrop>false</ScaleCrop>" +
                "<HeadingPairs>" +
                "<vt:vector size=\"4\" baseType=\"variant\">" +
                "<vt:variant>" +
                "<vt:lpstr>Theme</vt:lpstr>" +
                "</vt:variant>" +
                "<vt:variant>" +
                "<vt:i4>1</vt:i4>" +
                "</vt:variant>" +
                "<vt:variant>" +
                "<vt:lpstr>Slide Titles</vt:lpstr>" +
                "</vt:variant>" +
                "<vt:variant>" +
                "<vt:i4>0</vt:i4>" +
                "</vt:variant>" +
                "</vt:vector>" +
                "</HeadingPairs>" +
                "<TitlesOfParts>" +
                "<vt:vector size=\"1\" baseType=\"lpstr\">" +
                "<vt:lpstr>Office Theme</vt:lpstr>" +
                "</vt:vector>" +
                "</TitlesOfParts>" +
                "<LinksUpToDate>false</LinksUpToDate>" +
                "<SharedDoc>false</SharedDoc>" +
                "<HyperlinksChanged>false</HyperlinksChanged>" +
                "<AppVersion>12.0000</AppVersion>" +
                "</Properties>");

            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }
    }

    /// <summary>
    /// Ppt Table styles class
    /// </summary>
    internal class OoPptTableStyles : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/tableStyles"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.presentationml.tableStyles+xml"; } }
        public override string FileName { get { return "ppt/tableStyles.xml"; } }
        #endregion

        internal void Export(PowerPoint2007Export OoXML)
        {
            MemoryStream file = new MemoryStream(); 

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<a:tblStyleLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" def=\"{5C22544A-7EE6-4342-B048-85BDC9FD1C3A}\"/>");

            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }
    }

    /// <summary>
    /// Ppt Presentation properties class
    /// </summary>
    internal class OoPptPresProperties : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/presProps"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.presentationml.presProps+xml"; } }
        public override string FileName { get { return "ppt/presProps.xml"; } }
        #endregion

        internal void Export(PowerPoint2007Export OoXML)
        {
            MemoryStream file = new MemoryStream();
            
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<p:presentationPr xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:p=\"http://schemas.openxmlformats.org/presentationml/2006/main\" />");

            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }
    }

    /// <summary>
    /// Ppt View Properties class
    /// </summary>
    internal class OoPptViewProps : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/viewProps"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.presentationml.viewProps+xml"; } }
        public override string FileName { get { return "ppt/viewProps.xml"; } }
        #endregion

        internal void Export(PowerPoint2007Export OoXML)
        {
            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<p:viewPr xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:p=\"http://schemas.openxmlformats.org/presentationml/2006/main\" lastView=\"sldThumbnailView\">");
            ExportUtils.WriteLn(file, "<p:normalViewPr showfilelineIcons=\"0\">");
            ExportUtils.WriteLn(file, "<p:restoredLeft sz=\"15620\" autoAdjust=\"0\" />");
            ExportUtils.WriteLn(file, "<p:restoredTop sz=\"94660\" autoAdjust=\"0\" />");
            ExportUtils.WriteLn(file, "</p:normalViewPr>");
            ExportUtils.WriteLn(file, "<p:slideViewPr>");
            ExportUtils.WriteLn(file, "<p:cSldViewPr>");
            ExportUtils.WriteLn(file, "<p:cViewPr varScale=\"1\">");
            ExportUtils.WriteLn(file, "<p:scale>");
            ExportUtils.WriteLn(file, "<a:sx n=\"104\" d=\"100\" />");
            ExportUtils.WriteLn(file, "<a:sy n=\"104\" d=\"100\" />");
            ExportUtils.WriteLn(file, "</p:scale>");
            ExportUtils.WriteLn(file, "<p:origin x=\"-222\" y=\"-90\" />");
            ExportUtils.WriteLn(file, "</p:cViewPr>");
            ExportUtils.WriteLn(file, "<p:guideLst>");
            ExportUtils.WriteLn(file, "<p:guide orient=\"horz\" pos=\"2160\" />");
            ExportUtils.WriteLn(file, "<p:guide pos=\"2880\" />");
            ExportUtils.WriteLn(file, "</p:guideLst>");
            ExportUtils.WriteLn(file, "</p:cSldViewPr>");
            ExportUtils.WriteLn(file, "</p:slideViewPr>");
            ExportUtils.WriteLn(file, "<p:outlineViewPr>");
            ExportUtils.WriteLn(file, "<p:cViewPr>");
            ExportUtils.WriteLn(file, "<p:scale>");
            ExportUtils.WriteLn(file, "<a:sx n=\"33\" d=\"100\" />");
            ExportUtils.WriteLn(file, "<a:sy n=\"33\" d=\"100\" />");
            ExportUtils.WriteLn(file, "</p:scale>");
            ExportUtils.WriteLn(file, "<p:origin x=\"0\" y=\"0\" />");
            ExportUtils.WriteLn(file, "</p:cViewPr>");
            ExportUtils.WriteLn(file, "</p:outlineViewPr>");
            ExportUtils.WriteLn(file, "<p:notesTextViewPr>");
            ExportUtils.WriteLn(file, "<p:cViewPr>");
            ExportUtils.WriteLn(file, "<p:scale>");
            ExportUtils.WriteLn(file, "<a:sx n=\"100\" d=\"100\" />");
            ExportUtils.WriteLn(file, "<a:sy n=\"100\" d=\"100\" />");
            ExportUtils.WriteLn(file, "</p:scale>");
            ExportUtils.WriteLn(file, "<p:origin x=\"0\" y=\"0\" />");
            ExportUtils.WriteLn(file, "</p:cViewPr>");
            ExportUtils.WriteLn(file, "</p:notesTextViewPr>");
            ExportUtils.WriteLn(file, "<p:gridSpacing cx=\"73736200\" cy=\"73736200\" />");
            ExportUtils.WriteLn(file, "</p:viewPr>");

            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }
    }

    /// <summary>
    /// Specifies the image format in PowerPoint export.
    /// </summary>
    public enum PptImageFormat
    {
        /// <summary>
        /// Specifies the .png format.
        /// </summary>
        Png,

        /// <summary>
        /// Specifies the .jpg format.
        /// </summary>
        Jpeg
    }

    /// <summary>
    /// Represents the PowerPoint 2007 export.
    /// </summary>
    public partial class PowerPoint2007Export : OOExportBase
    {
        #region Slide layouts initializer
        PptLayoutDescriptor[] layoutDescriptors = 
            {
                new PptLayoutDescriptor("blank", "Blank", new PptShape[]
                    {
                        //new PptShape( 2, "Date Placeholder 1", 0, 0, 0, 0, "dt", "half", null, 10),
                        //new PptShape( 3, "Footer Placeholder 2", 0, 0, 0, 0, "ftr", "quarter", null, 11),
                        //new PptShape( 4, "Slide Number Placeholder 3", 0, 0, 0, 0, "sldNum", "quarter", null, 12)
                    }
                )
            };
        #endregion

        #region Private fields
        private OoPptPresentation           presentation;
        private OoXMLCoreDocumentProperties coreDocProp;
        private OoPptApplicationProperties  appProp;
        private OoPptViewProps              viewProps;
        private OoPptTableStyles            tableStyles;
        private OoPptPresProperties         presentationProperties;
        private OoXMLThemes                 themes;

        private long                        paperWidth;
        private long                        paperHeight;
        private long                        leftMargin;
        private long                        topMargin;

        private OoPptSlideMaster            slideMasters;
        private ArrayList                   slideLayouts;
        private ArrayList                   slideList;
        private PptImageFormat              imageFormat;
        private int                         pictureCount;
        private int                         slideIDCount;
        private int                         slideCount;
        internal int                        identifier;
        internal uint                       slideMasterID;
        #endregion

        #region Properties
        internal int PictureCount
        {
          get { return pictureCount; }
          set { pictureCount = value; }
        }
        internal OoPptSlideMaster SlideMaster { get { return slideMasters; } }
        internal ArrayList SlideLayoutList { get { return slideLayouts; } }
        internal ArrayList SlideList { get { return slideList; } }
        internal long PaperWidth { get { return paperWidth; } }
        internal long PaperHeight { get { return paperHeight; } }
        internal long LeftMargin { get { return leftMargin; } }
        internal long TopMargin { get { return topMargin; } }

        internal int SlideCount { get { return slideCount; } set { slideCount = value; } }
        internal int SlideIDCount { get { return slideIDCount; } set { slideIDCount = value; } }
        
        /// <summary>
        /// Gets or sets the image format used when exporting.
        /// </summary>
        public PptImageFormat ImageFormat
        {
            get { return imageFormat; }
            set { imageFormat = value; }
        }

        #endregion

        #region Private Methods

        private void CreateRelations()
        {
            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId3\" Type=" + Quoted(appProp.RelationType) + " Target=" + Quoted(appProp.FileName) + " />");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId2\" Type=" + Quoted(coreDocProp.RelationType) + " Target=" + Quoted(coreDocProp.FileName) + " />");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId1\" Type=" + Quoted(presentation.RelationType) + " Target=" + Quoted(presentation.FileName) + " />");
            ExportUtils.WriteLn(file, "</Relationships>");

            file.Position = 0;
            Zip.AddStream("_rels/.rels", file);
        }

        private void CreateContentTypes()
        {
            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.Write(file, "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");

            for (int i = 0; i < slideLayouts.Count; i++)
            {
                OoPptSlideLayout layout = slideLayouts[i] as OoPptSlideLayout;
                ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(layout.FileName) + " ContentType=" + Quoted(layout.ContentType) + "/>");
            }

            for (int i = 0; i < slideList.Count; i++)
            {
                OoPptSlide slide = slideList[i] as OoPptSlide;
                ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(slide.FileName) + " ContentType=" + Quoted(slide.ContentType) + "/>");
            }

            ExportUtils.Write(file, "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\" />");
            ExportUtils.Write(file, "<Default Extension=\"xml\" ContentType=\"application/xml\" />");
            ExportUtils.Write(file, "<Default Extension=\"png\" ContentType=\"image/png\"/>");
            ExportUtils.Write(file, "<Default Extension=\"jpg\" ContentType=\"image/jpeg\"/>");

            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(presentation.FileName) + " ContentType=" + Quoted(presentation.ContentType) + "/>");
            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(coreDocProp.FileName) + " ContentType=" + Quoted(coreDocProp.ContentType) + "/>");
            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(appProp.FileName) + " ContentType=" + Quoted(appProp.ContentType) + "/>");
            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(SlideMaster.FileName) + " ContentType=" + Quoted(SlideMaster.ContentType) + "/>");
            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(tableStyles.FileName) + " ContentType=" + Quoted(tableStyles.ContentType) + "/>");
            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(viewProps.FileName) + " ContentType=" + Quoted(viewProps.ContentType) + "/>");
            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(presentationProperties.FileName) + " ContentType=" + Quoted(presentationProperties.ContentType) + "/>");
            ExportUtils.Write(file, "<Override PartName=" + QuotedRoot(themes.FileName) + " ContentType=" + Quoted(themes.ContentType) + "/>");

            ExportUtils.Write(file, "</Types>");

            file.Position = 0;
            Zip.AddStream("[Content_Types].xml", file);
            
        }

        private void ExportOOPPT(Stream Stream)
        {
            CreateContentTypes();

            presentationProperties.Export( this );
            tableStyles.Export(this);
            viewProps.Export(this);
            themes.Export(this, "FastReport.Resources.theme1.xml", "/ppt/theme/theme1.xml" );

            CreateRelations();
          
            appProp.Export(this);
            coreDocProp.Export(this);
            presentation.Export(this);
            slideMasters.Export(this);

            foreach (OoPptSlideLayout layout in slideLayouts)
            {
                layout.Export(this);
            }
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            //Init
            pictureCount = 0;
            slideIDCount = 0;
            slideCount = 0;
            identifier = 0;
            slideMasterID = 0;

            slideMasterID = 2147483648;

            presentation = new OoPptPresentation();
            coreDocProp = new OoXMLCoreDocumentProperties();
            appProp = new OoPptApplicationProperties();

            slideMasters = new OoPptSlideMaster(this);

            viewProps = new OoPptViewProps();
            tableStyles = new OoPptTableStyles();
            presentationProperties = new OoPptPresProperties();
            themes = new OoXMLThemes();

            // Set relations to presentation.xml.rels
            presentation.AddRelation(1, slideMasters);
            presentation.AddRelation(2, presentationProperties);
            presentation.AddRelation(3, viewProps);
            presentation.AddRelation(4, themes);
            presentation.AddRelation(5, tableStyles);

            slideLayouts = new ArrayList();
            slideList = new ArrayList();

            // Set relations between layouts and Slide Master
            for (int i = 0; i < layoutDescriptors.Length; i++)
            {
                OoPptSlideLayout current_layout = new OoPptSlideLayout(this, 1 + i, layoutDescriptors[i]);
                current_layout.AddRelation(1, slideMasters);
                slideMasters.AddRelation(1 + i, current_layout);
                slideLayouts.Add(current_layout);
            }

            slideMasters.AddRelation(15000, themes);

            Zip = new ZipArchive();
        }

        private OoPptSlide slide;

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            const long EMUpmm = 36000;

            this.paperWidth = (long)(ExportUtils.GetPageWidth(page) * EMUpmm);
            this.paperHeight = (long)(ExportUtils.GetPageHeight(page) * EMUpmm);
            this.leftMargin = (long)(page.LeftMargin * EMUpmm);
            this.topMargin = (long)(page.TopMargin * EMUpmm);
            
            slide = new OoPptSlide(this);

            if (SlideIDCount < 255) slide.Reset();

            slideList.Add(slide);
            int relatives_count = presentation.RelationList.Count;
            presentation.AddRelation(relatives_count + 1, slide);
            slide.ExportBegin(this, page);
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            slide.ExportBand(this, band);
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            slide.ExportEnd(this, page);
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            ExportOOPPT(Stream);
            Zip.SaveToStream(Stream);
            Zip.Clear();
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("PptxFile");
        }
        #endregion




        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
          base.Serialize(writer);
          writer.WriteValue("ImageFormat", ImageFormat);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PowerPoint2007Export"/> class with the default settings.
        /// </summary>
        public PowerPoint2007Export()
        { 
        }
    }
}
