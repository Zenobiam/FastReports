using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FastReport.Utils;

namespace FastReport.Export.Html
{
    public partial class HTMLExport : ExportBase
    {
        private int imagesCount;
        private ExportMatrix matrix;

        private void ExportPageStyles(FastString styles, ExportMatrix FMatrix, int PageNumber)
        {
            if (FMatrix.StylesCount - prevStyleListIndex > 0)
            {
                pageStyleName = "frpage" + currentPage;
                styles.Append(HTMLGetStylesHeader());
                for (int i = prevStyleListIndex; i < FMatrix.StylesCount; i++)
                {
                    ExportIEMStyle EStyle = FMatrix.StyleById(i);
                    styles.Append(HTMLGetStyleHeader(i, PageNumber));
                    HTMLGetStyle(styles, EStyle.Font, EStyle.TextColor, 
                        EStyle.FillColor, EStyle.HAlign, EStyle.VAlign, EStyle.Border, 
                        EStyle.Padding, EStyle.RTL, EStyle.WordWrap, EStyle.LineHeight, EStyle.ParagraphOffset);
                }
                styles.AppendLine(HTMLGetStylesFooter());
            }
        }

        private string HTMLSaveImage(ExportIEMObject obj, int PageNumber, int CurrentPage, int ImageNumber, bool isSvg)
        {
            if (pictures)
                return HTMLGetImageTag(HTMLGetImage(PageNumber, CurrentPage, ImageNumber, obj.Hash, obj.Base, obj.Metafile, obj.PictureStream, isSvg));
            else
                return String.Empty;
        }

        private void SetUpMatrix(ExportMatrix FMatrix)
        {
            if (singlePage && prevStyleList != null)
                FMatrix.Styles = prevStyleList;
            if (wysiwyg)
                FMatrix.Inaccuracy = 0.5f;
            else
                FMatrix.Inaccuracy = 10;

            if (webMode)
            {
                singlePage = false;
                navigator = false;
            }
            FMatrix.Watermarks = true;
            FMatrix.HTMLMode = true;
            FMatrix.FillAsBitmap = true;
            FMatrix.Zoom = Zoom;
            FMatrix.RotatedAsImage = true;
            FMatrix.PlainRich = true;
            FMatrix.CropAreaFill = false;
            FMatrix.AreaFill = true;
            FMatrix.Report = Report;
            FMatrix.FramesOptimization = true;
            FMatrix.ShowProgress = false;
            FMatrix.FullTrust = false;
        }

        private void GetColumnSizes(FastString Page, ExportMatrix FMatrix)
        {
            Page.Append("<tr style=\"height: 1px\">");
            for (int x = 0; x < FMatrix.Width - 1; x++)
                Page.Append("<td style=\"boder-collapse:separate;border:none;padding:0;\" width=\"").
                    Append(SizeValue(Math.Round(FMatrix.XPosById(x + 1) - FMatrix.XPosById(x)), FMatrix.MaxWidth, widthUnits)).
                    Append("\"/>");
            if (FMatrix.Width < 2)
                Page.Append("<td/>");
            Page.AppendLine("</tr>");            
        }

        private int GetTableHeader(FastString Page, ExportMatrix FMatrix, int PageNumber, int CurrentPage, int ImagesCount)
        {
            Page.Append("<table width=\"").
                Append(SizeValue(Math.Round(FMatrix.MaxWidth), Math.Round(FMatrix.MaxWidth), widthUnits)).
                Append("\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-collapse:separate;border:none;padding:0;margin:0;table-layout:fixed;");
            // add watermark
            if (FMatrix.Pages[0].WatermarkPictureStream != null)
            {
                string wName;
                if (prevWatermarkSize != FMatrix.Pages[0].WatermarkPictureStream.Length)
                {
                    ExportIEMObject watermark = new ExportIEMObject();
                    watermark.Width = (FMatrix.Pages[0].Width - FMatrix.Pages[0].LeftMargin - FMatrix.Pages[0].RightMargin) * Units.Millimeters;
                    watermark.Height = (FMatrix.Pages[0].Height - FMatrix.Pages[0].TopMargin - FMatrix.Pages[0].BottomMargin) * Units.Millimeters;
                    watermark.PictureStream = FMatrix.Pages[0].WatermarkPictureStream;
                    prevWatermarkSize = FMatrix.Pages[0].WatermarkPictureStream.Length;
                    FMatrix.CheckPicsCache(watermark);
                    prevWatermarkName = HTMLGetImage(PageNumber, CurrentPage, ImagesCount++, watermark.Hash, watermark.Base, watermark.Metafile, watermark.PictureStream, false);
                }
                wName = prevWatermarkName;
                Page.Append(" background: url(").Append(wName).Append(") no-repeat;");
            }
            if (singlePage && PageNumber > 1 && pageBreaks)
                Page.Append("\" class=\"page_break");
            Page.AppendLine("\" >");

            return ImagesCount;
        }

        private int GetTable(FastString Page, ExportMatrix FMatrix, int PageNumber, int CurrentPage, int ImagesCount)
        {
            string pgStyle;
            if (singlePage)
                pgStyle = String.Empty;
            else
                pgStyle = String.Concat("-", PageNumber.ToString());

            for (int y = 0; y < FMatrix.Height - 1; y++)
            {
                int drow = (int)Math.Round((FMatrix.YPosById(y + 1) - FMatrix.YPosById(y)));
                if (drow == 0)
                    drow = 1;
                Page.Append("<tr style=\"height:").Append(SizeValue(drow, FMatrix.MaxHeight, heightUnits)).Append("\">");
                for (int x = 0; x < FMatrix.Width - 1; x++)
                {
                    int i = FMatrix.Cell(x, y);
                    if (i != -1)
                    {
                        ExportIEMObject obj = FMatrix.ObjectById(i);
                        if (obj.Counter == 0)
                        {
                            int fx, fy, dx, dy;
                            FMatrix.ObjectPos(i, out fx, out fy, out dx, out dy);
                            obj.Counter = 1;
                            Page.Append("<td").
                                Append((dx > 1 ? " colspan=\"" + dx.ToString() + "\"" : String.Empty)).
                                Append((dy > 1 ? " rowspan=\"" + dy.ToString() + "\"" : String.Empty)).
                                Append(" class=\"").Append(stylePrefix).Append("s").Append(obj.StyleIndex.ToString()).
                                Append(pgStyle).
                                Append("\"");
                            FastString style = new FastString(256);
                            if (obj.Text.Length == 0)
                                style.Append("font-size:1px;");
                            if (obj.PictureStream != null && obj.IsText)
                                style.Append("background-image: url(").
                                    Append(HTMLGetImage(PageNumber, CurrentPage, ImagesCount++, obj.Hash, obj.Base, obj.Metafile, obj.PictureStream, false)).
                                    Append(");");
                            if (style.Length > 0)
                                Page.Append(" style=\"").Append(style).Append("\"");
                            if (!obj.Style.WordWrap)
                                Page.Append(" nowrap ");
                            Page.Append(">");

                            // TEXT
                            if (!String.IsNullOrEmpty(obj.URL))
                                Page.Append("<a href=\"" + obj.URL + "\">");
                            if (obj.IsText)
                            {
                                if (obj.Text.Length > 0)
                                {
                                    switch (obj.TextRenderType)
                                    {
                                        case TextRenderType.HtmlParagraph:
                                            Page.Append(GetHtmlParagraph(obj));
                                            break;
                                        default:
                                            Page.Append(ExportUtils.HtmlString(obj.Text, obj.TextRenderType));
                                            break;
                                    }
                                }
                                else
                                    Page.Append(NBSP);
                            }
                            else if (obj.TextRenderType == TextRenderType.HtmlTags)
                            {
                                Page.Append(obj.Text);
                            }
                            else
                                Page.Append(HTMLSaveImage(obj, PageNumber, CurrentPage, ImagesCount++, obj.IsSvg));
                            if (!String.IsNullOrEmpty(obj.URL))
                                Page.Append("</a>");

                            Page.Append("</td>");
                        }
                    }
                    else
                        Page.Append("</td>");
                }
                Page.AppendLine("</tr>");
            }
            return ImagesCount;
        }

        private string GetHtmlParagraph(ExportIEMObject obj)
        {

            RectangleF textRect = new RectangleF(0, 0, obj.Width - obj.Style.Padding.Horizontal, obj.Height - obj.Style.Padding.Vertical);
            Color color = Color.Black; if (obj.Style.TextFill is SolidFill) color = (obj.Style.TextFill as SolidFill).Color;
            StringFormatFlags flags = 0;
            float scale = 1;
            StringAlignment align = StringAlignment.Near;
            if (obj.Style.HAlign == HorzAlign.Center) align = StringAlignment.Center;
            else if (obj.Style.HAlign == HorzAlign.Right) align = StringAlignment.Far;
            StringAlignment lineAlign = StringAlignment.Near;
            if (obj.Style.VAlign == VertAlign.Center) lineAlign = StringAlignment.Center;
            else if (obj.Style.VAlign == VertAlign.Bottom) lineAlign = StringAlignment.Far;
            if (obj.Style.RTL) flags |= StringFormatFlags.DirectionRightToLeft;
            if (!obj.Style.WordWrap) flags |= StringFormatFlags.NoWrap;
            flags |= StringFormatFlags.NoClip;
            StringFormat format = Report.GraphicCache.GetStringFormat(align, lineAlign, StringTrimming.None, flags, obj.Style.FirstTabOffset * scale, obj.TabWidth * scale);
            if (obj.ParagraphFormat == null) obj.ParagraphFormat = new ParagraphFormat();

            using (HtmlTextRenderer renderer = new HtmlTextRenderer(obj.Text, Report.MeasureGraphics, obj.Style.Font.Name, obj.Style.Font.Size * DrawUtils.ScreenDpiFX, obj.Style.Font.Style, color,
              obj.Style.TextColor, textRect, obj.Style.Underlines,
              format, obj.Style.HAlign, obj.Style.VAlign, obj.ParagraphFormat.MultipleScale(1), obj.Style.ForceJustify,
              1, 1, obj.InlineImageCache == null ? new InlineImageCache() : obj.InlineImageCache))
            {

                return GetHtmlParagraph(renderer).ToString();
            }
        }

        private void GetTableFooter(FastString Page)
        {
            Page.AppendLine("</table>");
        }

        private void ExportHTMLPageTabledBegin(HTMLData d)
        {
            imagesCount = 0;
            matrix = new ExportMatrix();
            SetUpMatrix(matrix);
            matrix.AddPageBegin(d.page);
        }

        private void ExportBandTable(Base band)
        {
            matrix.AddBand(band, this);
        }

        private void ExportHTMLPageTabledEnd(HTMLData d)
        {
            matrix.AddPageEnd(d.page);
            matrix.Prepare();
            FastString Page = new FastString(4096);
            ExportPageStyles(Page, matrix, d.ReportPage); 
            if (singlePage)
            {
                prevStyleListIndex = matrix.StylesCount;
                prevStyleList = matrix.Styles;
            }
            ExportHTMLPageStart(Page, d.PageNumber, d.CurrentPage);
            // Ancor
            Page.Append(HTMLGetAncor(d.PageNumber.ToString()));
            // Table header
            imagesCount = GetTableHeader(Page, matrix, d.PageNumber, d.CurrentPage, imagesCount);
            // Column sizes
            GetColumnSizes(Page, matrix);
            // Table
            imagesCount = GetTable(Page, matrix, d.ReportPage /*d.PageNumber*/, d.CurrentPage, imagesCount);
            // Table footer
            GetTableFooter(Page);
            ExportHTMLPageFinal(null, Page, d, matrix.MaxWidth, matrix.MaxHeight);
        }
    }
}
