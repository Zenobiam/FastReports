using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Format;
using FastReport.Code;
using System.Windows.Forms;
using System.Drawing.Design;

namespace FastReport
{
   
    public partial class TextObject 
    {
#if DOTNET_4
        /// <inheritdoc/>
        public override bool IsHaveToConvert(object sender)
        {
            if (TextRenderType == TextRenderType.HtmlParagraph)
            {
                if (sender is Export.Pdf.PDFExport ||
                  sender is Export.OoXML.Word2007Export ||
                  sender is Export.Html.HTMLExport ||
                  sender is Export.Image.ImageExport)
                    return base.IsHaveToConvert(sender);

                return true;
            }
            return base.IsHaveToConvert(sender);
        }

        /// <inheritdoc/>
        public override IEnumerable<Base> GetConvertedObjects()
        {
            SVG.SVGObject svgObject = new SVG.SVGObject();
            svgObject.SetReport(Report);
            svgObject.Assign(this);
            svgObject.SetParentCore(this.Parent);
            svgObject.Left = Left;
            svgObject.Top = Top;

            RectangleF textRect = new RectangleF(Padding.Left, Padding.Top, Width - Padding.Horizontal, Height - Padding.Vertical);
            System.Globalization.NumberFormatInfo nf = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;

            StringBuilder sb = new StringBuilder();
            sb.Append("<svg height=\"").Append(Height.ToString(nf)).Append("\" width=\"").Append(Width.ToString(nf)).Append("\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">");

            using (HtmlTextRenderer htmlTextRenderer = GetHtmlTextRenderer(Report.MeasureGraphics, textRect, 1, 1))
            {
                foreach (HtmlTextRenderer.RectangleFColor rect in htmlTextRenderer.Backgrounds)
                {
                    if (rect.Color.A > 0)
                    {
                        sb.Append("<rect x=\"").Append(rect.Left.ToString(nf))
                          .Append("\" y=\"").Append(rect.Top.ToString(nf))
                          .Append("\" width=\"").Append(rect.Width.ToString(nf))
                          .Append("\" height=\"").Append(rect.Height.ToString(nf))
                          .Append("\" fill=\"rgb(")
                          .Append(rect.Color.R).Append(",")
                          .Append(rect.Color.G).Append(",")
                          .Append(rect.Color.B).Append("")
                          .Append(")\" fill-opacity=\"")
                          .Append((((float)rect.Color.A) / 255f).ToString(nf))
                          .Append("\"/>");
                    }
                }

                foreach (HtmlTextRenderer.Paragraph paragraph in htmlTextRenderer.Paragraphs)
                {
                    foreach (HtmlTextRenderer.Line line in paragraph.Lines)
                    {
                        foreach (HtmlTextRenderer.Word word in line.Words)
                        {
                            if (word.Type == HtmlTextRenderer.WordType.Normal)
                                foreach (HtmlTextRenderer.Run run in word.Runs)
                                {
                                    if (run is HtmlTextRenderer.RunText)
                                    {
                                        string text = (run as HtmlTextRenderer.RunText).Text;
                                        HtmlTextRenderer.StyleDescriptor style = run.Style;
                                        float fontSize = style.Size / DrawUtils.ScreenDpiFX;
                                        if (style.BaseLine != HtmlTextRenderer.BaseLine.Normal) fontSize *= 0.6f;
                                        sb.Append("<text");
                                        sb.Append(" font-size=\"").Append((fontSize / 0.75f).ToString(nf)).Append("\"");
                                        sb.Append(" font-family=\"").Append(Converter.ToXml(style.Font)).Append("\"");
                                        sb.Append(" x=\"").Append(run.Left.ToString(nf)).Append("\"");
                                        sb.Append(" y=\"").Append((run.Top + run.BaseLine).ToString(nf)).Append("\"");
                                        sb.Append(" fill=\"rgb(")
                                        .Append(style.Color.R).Append(",")
                                        .Append(style.Color.G).Append(",")
                                        .Append(style.Color.B).Append("")
                                        .Append(")\" fill-opacity=\"")
                                        .Append((((float)style.Color.A) / 255f).ToString(nf))
                                        .Append("\"");
                                        if ((style.FontStyle & FontStyle.Italic) == FontStyle.Italic)
                                            sb.Append(" font-style=\"italic\"");
                                        if ((style.FontStyle & FontStyle.Bold) == FontStyle.Bold)
                                            sb.Append(" font-weight=\"bold\"");
                                        if (htmlTextRenderer.RightToLeft)
                                            sb.Append(" text-anchor=\"end\"");
                                        sb.Append(">");
                                        foreach (char ch in text)
                                        {
                                            switch (ch)
                                            {
                                                case '"':
                                                    sb.Append("&quot;");
                                                    break;
                                                case '&':
                                                    sb.Append("&amp;");
                                                    break;
                                                case '<':
                                                    sb.Append("&lt;");
                                                    break;
                                                case '>':
                                                    sb.Append("&gt;");
                                                    break;
                                                case '\t':
                                                    sb.Append("&Tab;");
                                                    break;
                                                default:
                                                    sb.Append(ch);
                                                    break;
                                            }
                                        }
                                        sb.Append("</text>");
                                    }
                                    else if (run is HtmlTextRenderer.RunImage)
                                    {
                                        HtmlTextRenderer.RunImage runImage = run as HtmlTextRenderer.RunImage;
                                        if (runImage.Image != null)
                                        {
                                            using (Bitmap bmp = new Bitmap(runImage.Image.Width, runImage.Image.Height))
                                            {
                                                using (Graphics g = Graphics.FromImage(bmp))
                                                {
                                                    g.DrawImage(runImage.Image, Point.Empty);
                                                }
                                                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                                                {
                                                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                                    ms.Flush();

                                                    sb.Append("<image")
                                                    .Append(" xlink:href=\"data:image/png;base64,").Append(Convert.ToBase64String(ms.ToArray())).Append("\"")
                                                    .Append(" x=\"").Append(run.Left.ToString(nf)).Append("\"")
                                                    .Append(" y=\"").Append(run.Top.ToString(nf)).Append("\"")
                                                    .Append(" width=\"").Append(run.Width.ToString(nf)).Append("\"")
                                                    .Append(" height=\"").Append(run.Height.ToString(nf)).Append("\"")
                                                    .Append("/>");
                                                }
                                            }
                                        }
                                    }
                                }
                        }
                    }
                }

                foreach (HtmlTextRenderer.LineFColor line in htmlTextRenderer.Underlines)
                {
                    sb.Append("<line")
                      .Append(" x1=\"").Append(line.Left.ToString(nf)).Append("\"")
                      .Append(" y1=\"").Append(line.Top.ToString(nf)).Append("\"")
                      .Append(" x2=\"").Append(line.Right.ToString(nf)).Append("\"")
                      .Append(" y2=\"").Append(line.Top.ToString(nf)).Append("\"")
                      .Append(" stroke-width=\"").Append(line.Width.ToString(nf)).Append("\"")
                      .Append(" stroke=\"rgb(")
                        .Append(line.Color.R).Append(",")
                        .Append(line.Color.G).Append(",")
                        .Append(line.Color.B).Append("")
                        .Append(")\" stroke-opacity=\"")
                        .Append((((float)line.Color.A) / 255f).ToString(nf))
                        .Append("\"")
                      .Append("/>");
                }
                foreach (HtmlTextRenderer.LineFColor line in htmlTextRenderer.Stikeouts)
                {
                    sb.Append("<line")
                      .Append(" x1=\"").Append(line.Left.ToString(nf)).Append("\"")
                      .Append(" y1=\"").Append(line.Top.ToString(nf)).Append("\"")
                      .Append(" x2=\"").Append(line.Right.ToString(nf)).Append("\"")
                      .Append(" y2=\"").Append(line.Top.ToString(nf)).Append("\"")
                      .Append(" stroke-width=\"").Append(line.Width.ToString(nf)).Append("\"")
                      .Append(" stroke=\"rgb(")
                        .Append(line.Color.R).Append(",")
                        .Append(line.Color.G).Append(",")
                        .Append(line.Color.B).Append("")
                        .Append(")\" stroke-opacity=\"")
                        .Append((((float)line.Color.A) / 255f).ToString(nf))
                        .Append("\"")
                      .Append("/>");
                }
                sb.Append("</svg>");
                svgObject.SetSVGByContent(sb.ToString());
                yield return svgObject;
            }
        }
#endif
    }
}