using FastReport.Utils;
using FastReport.Data;
using FastReport.Table;
using FastReport.Export.Image;
using FastReport.Export.RichText;
using FastReport.Export.Xml;
using FastReport.Export.Html;
using FastReport.Export.Mht;
using FastReport.Export.Odf;
using FastReport.Export.Pdf;
using FastReport.Export.Csv;
using FastReport.Export.Dbf;
using FastReport.Export.Dxf;
using FastReport.Export.XAML;
using FastReport.Export.Svg;
using FastReport.Export.Ppml;
using FastReport.Export.PS;
using FastReport.Export.BIFF8;
using FastReport.Export.OoXML;
using FastReport.Export.Json;
using FastReport.Export.LaTeX;
using FastReport.Export.Text;
using FastReport.Export.Zpl;
using FastReport.Barcode;
using FastReport.Matrix;
using FastReport.CrossView;
using FastReport.Format;
using FastReport.Functions;
using FastReport.Map;
using FastReport.Gauge.Linear;
using FastReport.Gauge.Simple;
using FastReport.Gauge.Radial;
using FastReport.Gauge.Simple.Progress;
using System.Collections.Generic;
using FastReport.Dialog;

namespace FastReport
{
    /// <summary>
    /// The FastReport.dll assembly initializer.
    /// </summary>
    public sealed class AssemblyInitializerBaseExt : AssemblyInitializerBase
    {
        /// <summary>
        /// Registers all core objects, wizards, export filters.
        /// </summary>
        public AssemblyInitializerBaseExt()
        {
#if !COMMUNITY
            // exports

            //RegisteredObjects.AddExport(typeof(PDFExport), "Uncategorized", "Export,Pdf,File", 201);
            //RegisteredObjects.AddExport(typeof(RTFExport), "Office", "Export,RichText,File", 190);
            //RegisteredObjects.AddExport(typeof(HTMLExport), "Web", "Export,Html,File");
            //RegisteredObjects.AddExport(typeof(MHTExport), "Web", "Export,Mht,File");
            //RegisteredObjects.AddExport(typeof(XMLExport), "XML", "Export,Xml,File", 191);
            //RegisteredObjects.AddExport(typeof(Excel2007Export), "Office", "Export,Xlsx,File", 191);
            //RegisteredObjects.AddExport(typeof(Excel2003Document), "Office", "Export,Xls,File", 191);
            //RegisteredObjects.AddExport(typeof(Word2007Export), "Office", "Export,Docx,File", 190);
            //RegisteredObjects.AddExport(typeof(PowerPoint2007Export), "Office", "Export,Pptx,File");
            //RegisteredObjects.AddExport(typeof(ODSExport), "Office", "Export,Ods,File");
            //RegisteredObjects.AddExport(typeof(ODTExport), "Office", "Export,Odt,File");
            //RegisteredObjects.AddExport(typeof(XPSExport), "Other", "Export,Xps,File");
            //RegisteredObjects.AddExport(typeof(CSVExport), "DataBase", "Export,Csv,File");
            //RegisteredObjects.AddExport(typeof(DBFExport), "DataBase", "Export,Dbf,File");
            //RegisteredObjects.AddExport(typeof(DxfExport), "Other", "Export,Dxf,File");
            //RegisteredObjects.AddExport(typeof(TextExport), "Print", "Export,Text,File");
            //RegisteredObjects.AddExport(typeof(ZplExport), "Print", "Export,Zpl,File");
            //RegisteredObjects.AddExport(typeof(XAMLExport), "XML", "Export,Xaml,File");
            //RegisteredObjects.AddExport(typeof(SVGExport), "Image", "Export,Svg,File");
            //RegisteredObjects.AddExport(typeof(PPMLExport), "Print", "Export,Ppml,File");
            //RegisteredObjects.AddExport(typeof(PSExport), "Print", "Export,PS,File");
            //RegisteredObjects.AddExport(typeof(JsonExport), "DataBase", "Export,Json,File");
            //RegisteredObjects.AddExport(typeof(LaTeXExport), "Other", "Export,LaTeX,File");

            ExportsOptions.GetInstance().RegisterExports();

            // Objects
            RegisteredObjects.Add(typeof(MapObject), "ReportPage", 153, 16);
            RegisteredObjects.Add(typeof(MapLayer), "", 169);
            RegisteredObjects.Add(typeof(ShapePoint), "", 103);
            RegisteredObjects.Add(typeof(ShapePolyLine), "", 103);
            RegisteredObjects.Add(typeof(ShapePolygon), "", 103);
            RegisteredObjects.Add(typeof(DigitalSignatureObject), "ReportPage", 251, 19);

            // report objects
#if MSCHART
            RegisteredObjects.Add(typeof(MSChart.MSChartObject), "ReportPage", 125, 12);
            RegisteredObjects.Add(typeof(MSChart.MSChartSeries), "", 130);
            RegisteredObjects.Add(typeof(MSChart.SparklineObject), "ReportPage", 130, 13);
#endif

#if !COMMUNITY

            // dialog controls
            RegisteredObjects.Add(typeof(ButtonControl), "DialogPage", 115);
            RegisteredObjects.Add(typeof(CheckBoxControl), "DialogPage", 116);
            RegisteredObjects.Add(typeof(CheckedListBoxControl), "DialogPage", 148);
            RegisteredObjects.Add(typeof(ComboBoxControl), "DialogPage", 119);
            RegisteredObjects.Add(typeof(DateTimePickerControl), "DialogPage", 120);
            RegisteredObjects.Add(typeof(LabelControl), "DialogPage", 112);
            RegisteredObjects.Add(typeof(ListBoxControl), "DialogPage", 118);
            RegisteredObjects.Add(typeof(MonthCalendarControl), "DialogPage", 145);
            RegisteredObjects.Add(typeof(RadioButtonControl), "DialogPage", 117);
            RegisteredObjects.Add(typeof(TextBoxControl), "DialogPage", 113);
            RegisteredObjects.Add(typeof(GroupBoxControl), "DialogPage", 143);
            RegisteredObjects.Add(typeof(PictureBoxControl), "DialogPage", 103);



            // pages
            RegisteredObjects.AddPage(typeof(DialogPage), "DialogPage", 136);
#endif


#if DOTNET_4
            RegisteredObjects.Add(typeof(SVG.SVGObject), "ReportPage", 249, 3);
#endif
            RegisteredObjects.Add(typeof(RichObject), "ReportPage", 126, 11);
#endif
        }
    }
}
