using System;
using System.Collections.Generic;
using System.Drawing;
using FastReport.Utils;

namespace FastReport.Barcode
{
    public partial class BarcodeObject
    {
#if DOTNET_4
        /// <summary>
        /// 
        /// </summary>
        public override bool IsHaveToConvert(object sender)
        {
            if (AsBitmap) return false;
            if (
                (sender is Export.Html.HTMLExport) && ((sender as Export.Html.HTMLExport).EnableVectorObjects && (sender as Export.Html.HTMLExport).Layers) ||
                (sender is Export.Pdf.PDFExport) && !((sender as Export.Pdf.PDFExport).SvgAsPicture)
               )
                return true;
            return base.IsHaveToConvert(sender);
        }

        /// <summary>
        /// 
        /// </summary>
        public override IEnumerable<Base> GetConvertedObjects()
        {
            SVG.SVGObject svgObject = new SVG.SVGObject();
            svgObject.SetReport(Report);
            svgObject.Assign(this);
            svgObject.SetParentCore(this.Parent);
            svgObject.Left = Left;
            svgObject.Top = Top;
            svgObject.Padding = Padding;

            if (svgObject.Width == 0)
                svgObject.Width = 1;
            if (svgObject.Height == 0)
                svgObject.Height = 1;

            using (XmlDocument document = new XmlDocument())
            {
                using (SvgGraphics g = new SvgGraphics(document))
                {

                    bool error = false;
                    string errorText = "";

                    if (String.IsNullOrEmpty(Text))
                    {
                        error = true;
                        errorText = NoDataText;
                    }
                    else
                        try
                        {
                            UpdateAutoSize();
                        }
                        catch (Exception ex)
                        {
                            error = true;
                            errorText = ex.Message;
                        }

                    g.ViewPort = new RectangleF(0, 0, Width, Height);

                    if (!error)
                        barcode.DrawBarcode(g, new RectangleF(0, 0, Width, Height));
                    else
                    {
                        g.DrawString(errorText, DrawUtils.DefaultReportFont, Brushes.Red,
                          new RectangleF(0, 0, Width, Height));
                    }
                }
                svgObject.SetSVGByContent(document.ToString());


            }

            // Fill for HTML & Pdf
            if(this.Fill != null && !this.Fill.IsTransparent)
            {
                TextObject fill = new TextObject();
                fill.Fill = this.Fill.Clone();
                fill.Left = this.Left;
                fill.Top = this.Top;
                fill.Width = this.Width;
                fill.Height = this.Height;
                fill.Parent = this.Parent;
                //fill.Font = null;

                yield return fill;
            }

            yield return svgObject;
        }
#endif
    }
}