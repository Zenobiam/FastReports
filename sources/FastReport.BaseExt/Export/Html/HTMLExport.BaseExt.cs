#if DOTNET_4
using FastReport.SVG;
#endif
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace FastReport.Export.Html
{

    public partial class HTMLExport
    {
        private void ExportHTMLPageBegin(object data)
        {
            HTMLData d = (HTMLData)data;
            if (layers)
                ExportHTMLPageLayeredBegin(d);
            else
                ExportHTMLPageTabledBegin(d);
        }

        private void ExportHTMLPageEnd(object data)
        {
            HTMLData d = (HTMLData)data;
            if (layers)
                ExportHTMLPageLayeredEnd(d);
            else
                ExportHTMLPageTabledEnd(d);
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            if (ExportMode == ExportType.Export)
                base.ExportBand(band);
            //
            if (Layers)
                ExportBandLayers(band);
            else
                ExportBandTable(band);
        }

        private bool HasExtendedExport(ReportComponentBase obj)
        {
#if DOTNET_4
            return obj is SVGObject;
#else
            return false;
#endif
        }

        private void ExtendExport(FastString Page, ReportComponentBase obj, FastString text)
        {
#if DOTNET_4
            if(obj is SVGObject)
                LayerSVG(htmlPage, obj as SVGObject, null);
#endif
        }

#if DOTNET_4
        #region svg

        private string GetLayerSVG(SVGObject svg, out float Width, out float Height)
        {
            Width = 0;
            Height = 0;

            if (svg != null)
            {
                if (pictures)
                {
                    Width = svg.Width == 0 ? svg.Border.LeftLine.Width : svg.Width;
                    Height = svg.Height == 0 ? svg.Border.TopLine.Width : svg.Height;

                    if (Math.Abs(Width) * Zoom < 1 && Zoom > 0)
                        Width = 1 / Zoom;

                    if (Math.Abs(Height) * Zoom < 1 && Zoom > 0)
                        Height = 1 / Zoom;
                    MemoryStream svgStream = new MemoryStream();
                    if (svg.Grayscale)
                        svg.SVGGrayscale.Write(svgStream);
                    else
                        svg.SvgDocument.Write(svgStream);
                    svgStream.Position = 0;
                    string result = HTMLGetImage(0, 0, 0, Crypter.ComputeHash(svgStream), true, null, svgStream, true);
                    return result;
                }
            }
            return null;
        }
        private void LayerSVG(FastString Page, ReportComponentBase obj, FastString text)
        {
            if (!pictures)
                return;
            float Width, Height;
            string svg = GetLayerSVG(obj as SVGObject, out Width, out Height);

            FastString addstyle = new FastString(64);
            addstyle.Append(GetStyle());
            addstyle.Append(" background: url('").Append(svg).Append("') center center no-repeat !important;-webkit-print-color-adjust:exact;");

            float x = obj.Width > 0 ? obj.AbsLeft : (obj.AbsLeft + obj.Width);
            float y = obj.Height > 0 ? hPos + obj.AbsTop : (hPos + obj.AbsTop + obj.Height);
            Layer(Page, obj, x, y, obj.Width, obj.Height, text, null, addstyle);

            // cleanup SVG 
            obj.Dispose();
        }

        #endregion
#endif
    }
}