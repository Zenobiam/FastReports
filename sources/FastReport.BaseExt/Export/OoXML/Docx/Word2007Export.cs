using FastReport.RichTextParser;
using FastReport.Table;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;

namespace FastReport.Export.OoXML
{
    abstract class OoXMLGenerator : OoXMLBase
    {

        protected void Export(OOExportBase OoXML, string resource_file)
        {
            Assembly a = Assembly.GetExecutingAssembly();

            // get a list of resource names from the manifest
            string[] resNames = a.GetManifestResourceNames();

            using (Stream o = a.GetManifestResourceStream(resource_file))
            {
                XmlDocument style = new XmlDocument();
                style.Load(o);
                XmlItem lang = style.Root.FindItem("w:docDefaults")
                    .FindItem("w:rPrDefault")
                    .FindItem("w:rPr")
                    .FindItem("w:lang");
                lang.SetProp("w:val", Res.LocaleName);
                lang.SetProp("w:eastAsia", Res.LocaleName);

                MemoryStream fs = new MemoryStream();
                style.Save(fs);
                fs.Position = 0;

                OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), fs);
            }
        }
    }

    class OoXMLWordStyles : OoXMLGenerator
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml"; } }
        public override string FileName { get { return "word/styles.xml"; } }
        #endregion
        public void Export(OOExportBase OoXML) { Export(OoXML, "FastReport.Resources.OoXML.styles.xml"); }
    }

    class OoXMLFootNotes : OoXMLBase
    {
        #region Class overrides
        public override string RelationType
        {
            get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footnotes"; }
        }

        public override string ContentType
        {
            get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.footnotes+xml"; }
        }

        public override string FileName
        {
            get { return "word/footnotes.xml"; }
        }
        #endregion

        public void Export(OOExportBase OoXML)
        {
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<w:footnotes xmlns:wpc=\"http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas\" xmlns:cx=\"http://schemas.microsoft.com/office/drawing/2014/chartex\" xmlns:cx1=\"http://schemas.microsoft.com/office/drawing/2015/9/8/chartex\" xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:wp14=\"http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing\" xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\" xmlns:w10=\"urn:schemas-microsoft-com:office:word\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" xmlns:w14=\"http://schemas.microsoft.com/office/word/2010/wordml\" xmlns:w15=\"http://schemas.microsoft.com/office/word/2012/wordml\" xmlns:w16se=\"http://schemas.microsoft.com/office/word/2015/wordml/symex\" xmlns:wpg=\"http://schemas.microsoft.com/office/word/2010/wordprocessingGroup\" xmlns:wpi=\"http://schemas.microsoft.com/office/word/2010/wordprocessingInk\" xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\" xmlns:wps=\"http://schemas.microsoft.com/office/word/2010/wordprocessingShape\" mc:Ignorable=\"w14 w15 w16se wp14\">");
            ExportUtils.WriteLn(file, "<w:footnote w:type=\"separator\" w:id=\"-1\"><w:p>");
            ExportUtils.WriteLn(file, "<w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr><w:r><w:separator/></w:r></w:p></w:footnote>");
            ExportUtils.WriteLn(file, "<w:footnote w:type=\"continuationSeparator\" w:id=\"0\"><w:p>");
            ExportUtils.WriteLn(file, "<w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr><w:r><w:continuationSeparator/>");
            ExportUtils.WriteLn(file, "</w:r></w:p></w:footnote></w:footnotes>");
            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }
    }

    class OoXMLEndNotes : OoXMLBase
    {
        #region Class overrides
        public override string RelationType
        {
            get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/endnotes"; }
        }

        public override string ContentType
        {
            get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml"; }
        }

        public override string FileName
        {
            get { return "word/endnotes.xml"; }
        }
        #endregion

        public void Export(OOExportBase OoXML)
        {
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<w:endnotes xmlns:wpc=\"http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas\" xmlns:cx=\"http://schemas.microsoft.com/office/drawing/2014/chartex\" xmlns:cx1=\"http://schemas.microsoft.com/office/drawing/2015/9/8/chartex\" xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:wp14=\"http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing\" xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\" xmlns:w10=\"urn:schemas-microsoft-com:office:word\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" xmlns:w14=\"http://schemas.microsoft.com/office/word/2010/wordml\" xmlns:w15=\"http://schemas.microsoft.com/office/word/2012/wordml\" xmlns:w16se=\"http://schemas.microsoft.com/office/word/2015/wordml/symex\" xmlns:wpg=\"http://schemas.microsoft.com/office/word/2010/wordprocessingGroup\" xmlns:wpi=\"http://schemas.microsoft.com/office/word/2010/wordprocessingInk\" xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\" xmlns:wps=\"http://schemas.microsoft.com/office/word/2010/wordprocessingShape\" mc:Ignorable=\"w14 w15 w16se wp14\">");
            ExportUtils.WriteLn(file, "<w:endnote w:type=\"separator\" w:id=\" - 1\">");
            ExportUtils.WriteLn(file, "<w:p>");
            ExportUtils.WriteLn(file, "<w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/></w:pPr>");
            ExportUtils.WriteLn(file, "<w:r><w:separator/></w:r></w:p></w:endnote>");
            ExportUtils.WriteLn(file, "<w:endnote w:type=\"continuationSeparator\" w:id=\"0\">");
            ExportUtils.WriteLn(file, "<w:p><w:pPr>");
            ExportUtils.WriteLn(file, "<w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/>");
            ExportUtils.WriteLn(file, "</w:pPr><w:r><w:continuationSeparator/></w:r></w:p></w:endnote></w:endnotes>");
            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }
    }

    class OoXMLHeader : OoXMLBase
    {
        string filename;
        #region Class overrides
        public override string RelationType
        {
            get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/header"; }
        }

        public override string ContentType
        {
            get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml"; }
        }

        public override string FileName
        {
            get { return filename; }
        }
        public OoXMLHeader(int number)
        {
            filename = "word/header" + number.ToString() + ".xml";
        }
        #endregion

        public void Export(OOExportBase OoXML)
        {
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<w:hdr xmlns:wpc=\"http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas\" xmlns:cx=\"http://schemas.microsoft.com/office/drawing/2014/chartex\" xmlns:cx1=\"http://schemas.microsoft.com/office/drawing/2015/9/8/chartex\" xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:wp14=\"http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing\" xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\" xmlns:w10=\"urn:schemas-microsoft-com:office:word\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" xmlns:w14=\"http://schemas.microsoft.com/office/word/2010/wordml\" xmlns:w15=\"http://schemas.microsoft.com/office/word/2012/wordml\" xmlns:w16se=\"http://schemas.microsoft.com/office/word/2015/wordml/symex\" xmlns:wpg=\"http://schemas.microsoft.com/office/word/2010/wordprocessingGroup\" xmlns:wpi=\"http://schemas.microsoft.com/office/word/2010/wordprocessingInk\" xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\" xmlns:wps=\"http://schemas.microsoft.com/office/word/2010/wordprocessingShape\" mc:Ignorable=\"w14 w15 w16se wp14\">");
            if (OoXML is Word2007Export)
            {
                Word2007Export wordExport = OoXML as Word2007Export;

                // fix for hyperlinks in page header and footer
                // remove any url from objects
                ExportMatrix matrix = wordExport.HeaderMatrix;
                int count = matrix.ObjectsCount;
                for (int i = 0; i < count; i++)
                {
                    ExportIEMObject obj = matrix.ObjectById(i);
                    obj.URL = null;
                }
                
                wordExport.OoXMLDocument.ExportSinglePage(matrix, false, file, true, true);
            }
            ExportUtils.WriteLn(file, "</w:hdr>");
            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }
    }


    class OoXMLFooter : OoXMLBase
    {
        private string fileName; 
        #region Class overrides
        public override string RelationType
        {
            get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer"; }
        }

        public override string ContentType
        {
            get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml"; }
        }

        public override string FileName
        {
            get { return fileName; }
        }

        #endregion

        public OoXMLFooter(int number)
        {
            fileName = "word/footer" + number.ToString() + ".xml";
        }

        public void Export(OOExportBase OoXML)
        {
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<w:ftr xmlns:wpc=\"http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas\" xmlns:cx=\"http://schemas.microsoft.com/office/drawing/2014/chartex\" xmlns:cx1=\"http://schemas.microsoft.com/office/drawing/2015/9/8/chartex\" xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:wp14=\"http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing\" xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\" xmlns:w10=\"urn:schemas-microsoft-com:office:word\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" xmlns:w14=\"http://schemas.microsoft.com/office/word/2010/wordml\" xmlns:w15=\"http://schemas.microsoft.com/office/word/2012/wordml\" xmlns:w16se=\"http://schemas.microsoft.com/office/word/2015/wordml/symex\" xmlns:wpg=\"http://schemas.microsoft.com/office/word/2010/wordprocessingGroup\" xmlns:wpi=\"http://schemas.microsoft.com/office/word/2010/wordprocessingInk\" xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\" xmlns:wps=\"http://schemas.microsoft.com/office/word/2010/wordprocessingShape\" mc:Ignorable=\"w14 w15 w16se wp14\">");
            if (OoXML is Word2007Export)
            {
                Word2007Export wordExport = OoXML as Word2007Export;
                // fix for hyperlinks in page header and footer
                // remove any url from objects
                ExportMatrix matrix = wordExport.FooterMatrix;
                int count = matrix.ObjectsCount;
                for (int i = 0; i < count; i++)
                {
                    ExportIEMObject obj = matrix.ObjectById(i);
                    obj.URL = null;
                }
                wordExport.OoXMLDocument.ExportSinglePage(matrix, false, file, true, true);
            }
            ExportUtils.WriteLn(file, "</w:ftr>");
            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }
    }

    class OoXMLWordSettings : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml"; } }
        public override string FileName { get { return "word/settings.xml"; } }
        #endregion

        public void Export(OOExportBase OoXML)
        {
            bool doNotExpandShiftReturn = true;
            if (OoXML is Word2007Export)
            {
                doNotExpandShiftReturn = (OoXML as Word2007Export).DoNotExpandShiftReturn;
            }
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            //before compat
            ExportUtils.Write(file, "<w:settings xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:w10=\"urn:schemas-microsoft-com:office:word\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" xmlns:sl=\"http://schemas.openxmlformats.org/schemaLibrary/2006/main\"><w:zoom w:percent=\"100\"/><w:embedSystemFonts/><w:bordersDoNotSurroundHeader/><w:bordersDoNotSurroundFooter/><w:defaultTabStop w:val=\"720\"/><w:drawingGridHorizontalSpacing w:val=\"120\"/><w:drawingGridVerticalSpacing w:val=\"120\"/><w:displayHorizontalDrawingGridEvery w:val=\"0\"/><w:displayVerticalDrawingGridEvery w:val=\"3\"/><w:doNotUseMarginsForDrawingGridOrigin/><w:doNotShadeFormData/><w:characterSpacingControl w:val=\"compressPunctuation\"/><w:doNotValidateAgainstSchema/><w:doNotDemarcateInvalidXml/><w:compat>");
            //compats
            ExportUtils.Write(file, "<w:spaceForUL/>");
            ExportUtils.Write(file, "<w:balanceSingleByteDoubleByteWidth/>");
            ExportUtils.Write(file, "<w:doNotLeaveBackslashAlone/>");
            ExportUtils.Write(file, "<w:ulTrailSpace/>");
            if (doNotExpandShiftReturn)
                ExportUtils.Write(file, "<w:doNotExpandShiftReturn/>");
            ExportUtils.Write(file, "<w:adjustLineHeightInTable/>");
            ExportUtils.Write(file, "<w:useFELayout/>");
            //after compats
            ExportUtils.Write(file, "</w:compat><w:rsids><w:rsidRoot w:val=\"00D31453\"/><w:rsid w:val=\"0002418B\"/><w:rsid w:val=\"001F0BC7\"/><w:rsid w:val=\"00D31453\"/><w:rsid w:val=\"00E209E2\"/></w:rsids><m:mathPr><m:mathFont m:val=\"Cambria Math\"/><m:brkBin m:val=\"before\"/><m:brkBinSub m:val=\"--\"/><m:smallFrac m:val=\"off\"/><m:dispDef/><m:lMargin m:val=\"0\"/><m:rMargin m:val=\"0\"/><m:defJc m:val=\"centerGroup\"/><m:wrapIndent m:val=\"1440\"/><m:intLim m:val=\"subSup\"/><m:naryLim m:val=\"undOvr\"/></m:mathPr><w:themeFontLang w:val=\"en-US\"/><w:clrSchemeMapping w:bg1=\"light1\" w:t1=\"dark1\" w:bg2=\"light2\" w:t2=\"dark2\" w:accent1=\"accent1\" w:accent2=\"accent2\" w:accent3=\"accent3\" w:accent4=\"accent4\" w:accent5=\"accent5\" w:accent6=\"accent6\" w:hyperlink=\"hyperlink\" w:followedHyperlink=\"followedHyperlink\"/><w:doNotIncludeSubdocsInStats/><w:doNotAutoCompressPictures/><w:shapeDefaults><o:shapedefaults v:ext=\"edit\" spidmax=\"2050\"/><o:shapelayout v:ext=\"edit\"><o:idmap v:ext=\"edit\" data=\"1\"/></o:shapelayout></w:shapeDefaults><w:decimalSymbol w:val=\",\"/><w:listSeparator w:val=\";\"/></w:settings>");
            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }
    }

    class OoXMLFontTable : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/fontTable"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.fontTable+xml"; } }
        public override string FileName { get { return "word/fontTable.xml"; } }
        #endregion

        internal void Export(Word2007Export OoXML)
        {
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<w:fonts xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\">");
            ExportUtils.WriteLn(file, "<w:font w:name=\"Tahoma\">");
            ExportUtils.WriteLn(file, "<w:panose1 w:val=\"020B0604030504040204\" />");
            ExportUtils.WriteLn(file, "<w:charset w:val=\"CC\" />");
            ExportUtils.WriteLn(file, "<w:family w:val=\"swiss\" />");
            ExportUtils.WriteLn(file, "<w:pitch w:val=\"variable\" />");
            ExportUtils.WriteLn(file, "<w:sig w:usb0=\"E1002AFF\" w:usb1=\"C000605B\" w:usb2=\"00000029\" w:usb3=\"00000000\" w:csb0=\"000101FF\" w:csb1=\"00000000\" />");
            ExportUtils.WriteLn(file, "</w:font>");
            ExportUtils.WriteLn(file, "</w:fonts>");
            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);
        }
    }

    class OoPictureObject : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image"; } }
        public override string ContentType { get { throw new Exception("Content type not defined"); } }
        public override string FileName { get { return imageFileName; } }
        #endregion

        string imageFileName;

        internal void SetFileName(string fileName)
        {
            imageFileName = fileName;
        }

        internal string ComputeHash(Bitmap image)
        {
            int raw_size = image.Width * image.Height;
            int[] raw_picture = new int[raw_size];
            BitmapData bmpdata = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            IntPtr ptr = bmpdata.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(ptr, raw_picture, 0, raw_size);
            image.UnlockBits(bmpdata);
            byte[] raw_picture_byte = new byte[raw_picture.Length * sizeof(int)];
            Buffer.BlockCopy(raw_picture, 0, raw_picture_byte, 0, raw_picture_byte.Length);
            return Crypter.ComputeHash(raw_picture_byte);
        }

        internal void Export(ReportComponentBase pictureObject, OOExportBase export, bool Background)
        {
            float printZoom = (export as Word2007Export).PrintOptimized ? 3 : 1;

            bool ClearBackground = false;
            System.Drawing.Imaging.ImageFormat image_format = System.Drawing.Imaging.ImageFormat.Png;

            using (System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Round(pictureObject.Width * printZoom), (int)Math.Round(pictureObject.Height * printZoom)))
            using (Graphics g = Graphics.FromImage(image))
            using (GraphicCache cache = new GraphicCache())
            {
                g.TranslateTransform(-pictureObject.AbsLeft * printZoom, -pictureObject.AbsTop * printZoom);
                if (ClearBackground)
                {
                    g.Clear(Color.White);
                }
                if (Background)
                    pictureObject.DrawBackground(new FRPaintEventArgs(g, printZoom, printZoom, cache));
                else
                    pictureObject.Draw(new FRPaintEventArgs(g, printZoom, printZoom, cache));

                string hash = ComputeHash(image);
                imageFileName = Path.Combine(Path.GetDirectoryName(imageFileName),
                    hash + Path.GetExtension(imageFileName)).Replace("\\", "/");

                if (!export.Zip.FileExists(imageFileName))
                {
                    MemoryStream ms = new MemoryStream(); // the stream will dispose after saving a zip file
                    image.Save(ms, image_format);
                    ms.Position = 0;
                    export.Zip.AddStream(ExportUtils.TruncLeadSlash(imageFileName), ms);
                }
            }
        }

        internal OoPictureObject(string FileName)
        {
            imageFileName = FileName;
        }
    }

    partial class OoXMLDocument : OoXMLBase
    {
        #region Class overrides
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"; } }
        public override string ContentType { get { return "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"; } }
        public override string FileName { get { return "word/document.xml"; } }
        #endregion

        #region "Private properties"
        OOExportBase wordExport;
        StringBuilder textStrings;
        Dictionary<string, OoPictureObject> checkboxList;
        int pictureCount;


        #endregion

        #region "Private methods"
        private string GetRGBString(Color c)
        {
            return "\"#" + /*ExportUtils.ByteToHex(c.A) +*/ ExportUtils.ByteToHex(c.R) + ExportUtils.ByteToHex(c.G) + ExportUtils.ByteToHex(c.B) + "\"";
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

        #region "Tables postponed"
        private void Export_TableProperties(Stream file)
        {
            ExportUtils.WriteLn(file, "<w:tblPr>");
            ExportUtils.WriteLn(file, "<w:tblW w:w=\"0\" w:type=\"auto\" />");
            ExportUtils.WriteLn(file, "<w:tblCellMar><w:left w:w=\"0\" w:type=\"dxa\"/><w:right w:w=\"0\" w:type=\"dxa\"/></w:tblCellMar>");
            ExportUtils.WriteLn(file, "<w:tblBorders>");
            ExportUtils.WriteLn(file, "<w:top w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
            ExportUtils.WriteLn(file, "<w:left w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
            ExportUtils.WriteLn(file, "<w:bottom w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
            ExportUtils.WriteLn(file, "<w:right w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
            ExportUtils.WriteLn(file, "<w:insideH w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
            ExportUtils.WriteLn(file, "<w:insideV w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
            ExportUtils.WriteLn(file, "</w:tblBorders>");
            ExportUtils.WriteLn(file, "</w:tblPr>");
        }

        private void Export_Picture(ExportIEMObject Obj, StringBuilder xml, bool ParagraphBased)
        {
            pictureCount++; // Increase picture counter
            OoPictureObject picture = new OoPictureObject("word/media/image" + pictureCount.ToString() + ".png");
            this.AddRelation(pictureCount + Word2007Export.RID_SHIFT, picture);

            picture.SetFileName(Path.Combine(Path.GetDirectoryName(picture.FileName),
                Obj.Hash + Path.GetExtension(picture.FileName)).Replace("\\", "/"));

            if (!wordExport.Zip.FileExists(picture.FileName))
            {
                Obj.PictureStream.Position = 0;
                wordExport.Zip.AddStream(picture.FileName, Obj.PictureStream);
            }

            long cx = (long)(Obj.Width * 360000 / 37.8f);
            long cy = (long)(Obj.Height * 360000 / 37.8f);

            xml.Append("<w:p>");

            if (Obj.URL != null && Obj.URL.Length > 0)
            {
                ++pictureCount;
                int rId = Word2007Export.RID_SHIFT + pictureCount;
                AddRelation(rId, new OoHyperlink(Utils.Converter.ToXml(Obj.URL)));
                xml.Append(string.Format("<w:hyperlink r:id = \"rId{0}\">", rId));
            }

            xml.Append("<w:r>");
            xml.Append("<w:rPr><w:noProof /></w:rPr>");
            xml.Append("<w:drawing>");
            xml.Append("<wp:inline distT=\"0\" distB=\"0\" distL=\"0\" distR=\"0\">");
            xml.Append("<wp:extent cx=").Append(Quoted(cx)).Append(" cy=").Append(Quoted(cy)).Append(" />");
            //              Out.WriteLine("<wp:effectExtent l="19050" t="0" r="0" b="0" />");
            xml.Append("<wp:docPr id=").Append(Quoted(pictureCount)).Append(" name=").Append(Quoted(pictureCount)).Append(" descr=\"Autogenerated\" />");
            xml.Append("<wp:cNvGraphicFramePr>");
            xml.Append("<a:graphicFrameLocks xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" noChangeAspect=\"1\" />");
            xml.Append("</wp:cNvGraphicFramePr>");
            xml.Append("<a:graphic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">");
            xml.Append("<a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">");
            xml.Append("<pic:pic xmlns:pic=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">");
            xml.Append("<pic:nvPicPr>");
            xml.Append("<pic:cNvPr id=").Append(Quoted(pictureCount)).Append(" name=").Append(Quoted(pictureCount)).Append(" />");
            xml.Append("<pic:cNvPicPr />");
            xml.Append("</pic:nvPicPr>");
            xml.Append("<pic:blipFill><a:blip r:embed=").Append(Quoted(picture.rId)).Append(" /><a:stretch><a:fillRect /></a:stretch></pic:blipFill>");
            xml.Append("<pic:spPr>");
            xml.Append("<a:xfrm><a:off x=\"0\" y=\"0\" /><a:ext cx=").Append(Quoted(cx)).Append(" cy=").Append(Quoted(cy)).Append(" /></a:xfrm>");
            xml.Append("<a:prstGeom prst=\"rect\"><a:avLst /></a:prstGeom>");
            xml.Append("</pic:spPr>");
            xml.Append("</pic:pic>");
            xml.Append("</a:graphicData>");
            xml.Append("</a:graphic>");
            xml.Append("</wp:inline>");
            xml.Append("</w:drawing>");
            xml.Append("</w:r>");

            if (Obj.URL != null && Obj.URL.Length > 0)
            {
                xml.Append("</w:hyperlink>");
            }

            xml.Append("</w:p>");
        }

        private void Export_Paragraph(ExportIEMObject Obj, StringBuilder xml, bool ParagraphBased, bool withVars)
        {
            IGraphics g = wordExport.Report.MeasureGraphics;
            using (Font f = new Font(Obj.Style.Font.Name, Obj.Style.Font.Size * DrawUtils.ScreenDpiFX, Obj.Style.Font.Style))
            {
                int fontSize = (int)(f.Size * 2 / DrawUtils.ScreenDpiFX);

                using (GraphicCache cache = new GraphicCache())
                {
                    RectangleF textRect = new RectangleF(Obj.Left, Obj.Top, Obj.Width, Obj.Height);

                    StringAlignment align = StringAlignment.Near;
                    if (Obj.Style.HAlign == HorzAlign.Center)
                        align = StringAlignment.Center;
                    else if (Obj.Style.HAlign == HorzAlign.Right)
                        align = StringAlignment.Far;

                    StringAlignment lineAlign = StringAlignment.Near;
                    if (Obj.Style.VAlign == VertAlign.Center)
                        lineAlign = StringAlignment.Center;
                    else if (Obj.Style.VAlign == VertAlign.Bottom)
                        lineAlign = StringAlignment.Far;

                    StringFormatFlags flags = 0;
                    if (Obj.Style.RTL)
                        flags |= StringFormatFlags.DirectionRightToLeft;

                    StringFormat format = cache.GetStringFormat(align, lineAlign, StringTrimming.Word, flags,
                        Obj.Style.FirstTabOffset, Obj.TabWidth);

                    Brush textBrush = cache.GetBrush(Obj.Style.TextColor);

                    switch (Obj.TextRenderType)
                    {
                        case TextRenderType.HtmlParagraph:
                            textRect.Width -= Obj.Style.Padding.Horizontal;
                            Color color = Color.Black; if (Obj.Style.TextFill is SolidFill) color = (Obj.Style.TextFill as SolidFill).Color;
                            using (HtmlTextRenderer htmlTextRenderer = new HtmlTextRenderer(Obj.Text, g, Obj.Style.Font.Name, Obj.Style.Font.Size, Obj.Style.Font.Style, color,
                                Obj.Style.TextColor, textRect, Obj.Style.Underlines,
                                format, Obj.Style.HAlign, Obj.Style.VAlign, Obj.ParagraphFormat.MultipleScale(DrawUtils.ScreenDpiFX), Obj.Style.ForceJustify,
                                               DrawUtils.ScreenDpiFX, DrawUtils.ScreenDpiFX, Obj.InlineImageCache == null ? new InlineImageCache() : Obj.InlineImageCache))
                            {
                                xml.Append(Export_HtmlParagraph(htmlTextRenderer, !ParagraphBased).ToString());
                            }
                            break;
                        default:
                            AdvancedTextRenderer renderer = new AdvancedTextRenderer(Obj.Text, g, f, textBrush, null,
                        textRect, format, Obj.Style.HAlign, Obj.Style.VAlign,
                        Obj.Style.Font.Height, Obj.Style.Angle, Obj.Style.FontWidthRatio,
                                      Obj.Style.HAlign == HorzAlign.Justify, true, Obj.HtmlTags, true, DrawUtils.ScreenDpiFX, DrawUtils.ScreenDpiFX, Obj.InlineImageCache);

                            //float w = f.Height * 0.1f; // to match .net char X offset

                            for (int j = 0; j < renderer.Paragraphs.Count; ++j)
                            {
                                AdvancedTextRenderer.Paragraph paragraph = renderer.Paragraphs[j];
                                string halign = "";
                                switch (Obj.Style.HAlign)
                                {
                                    case HorzAlign.Left: halign = "left"; break;
                                    case HorzAlign.Right: halign = "right"; break;
                                    case HorzAlign.Center: halign = "center"; break;
                                    case HorzAlign.Justify: halign = "both"; break;
                                }

                                float FirstTabOffset = Obj.Style.FirstTabOffset * 16;

                                textStrings.Append("<w:p><w:pPr><w:jc w:val=").Append(Quoted(halign)).Append(" />");
                                textStrings.Append("<w:ind w:left=").Append(Quoted(Obj.Style.Padding.Left * 1440 / DrawUtils.ScreenDpi));
                                textStrings.Append(" w:right=").Append(Quoted(Obj.Style.Padding.Right * 1440 / DrawUtils.ScreenDpi));
                                if (paragraph.Text.StartsWith("\t") && FirstTabOffset != 0)
                                    textStrings.Append(" w:firstLine=").Append(Quoted(FirstTabOffset));
                                textStrings.Append("/>");

                                long spacing = 238; //240 is single interval

                                textStrings.Append("<w:spacing w:before=").Append(j == 0 ? Quoted(Obj.Style.Padding.Top * 1440 / DrawUtils.ScreenDpi) : Quoted(0));
                                textStrings.Append(" w:after=").Append(j == renderer.Paragraphs.Count - 1 ? Quoted(Obj.Style.Padding.Bottom * 1440 / DrawUtils.ScreenDpi) : Quoted(0)).Append(" w:line=").
                                    Append(Quoted(spacing)).Append(" w:lineRule=\"auto\" w:beforeAutospacing=\"0\" w:afterAutospacing=\"0\" />");
                                textStrings.Append("<w:rPr><w:sz w:val=").Append(Quoted(fontSize)).Append(" /><w:szCs w:val=").Append(Quoted(fontSize)).
                                    Append(" /></w:rPr></w:pPr>");

                                if (renderer.HtmlTags)
                                {
                                    foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                                    {
                                        foreach (AdvancedTextRenderer.Word word in line.Words)
                                            foreach (AdvancedTextRenderer.Run run in word.Runs)
                                                using (Font fnt = run.GetFont())
                                                    Add_Run(fnt, run.Style.Color, run.Text, Obj.URL, true, Obj.Style.RTL,
                                                        run.Style.BaseLine,
                                                        Obj.Style.LineHeight,
                                                        false, withVars);
                                    }
                                }
                                else
                                {
                                    /*                        StringBuilder text = new StringBuilder();
                                                            foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                                                                text.Append(" ").Append(line.Text);
                                                            Add_Run(f, Obj.Style.TextColor, text.ToString(), false, Obj.Style.RTL, AdvancedTextRenderer.BaseLine.Normal);
                                    */

                                    for (int i = 0; i < paragraph.Lines.Count; i++)
                                    {
                                        AdvancedTextRenderer.Line line = paragraph.Lines[i];
                                        Add_Run(f, Obj.Style.TextColor, line.Text, Obj.URL, false,
                                            Obj.Style.RTL, AdvancedTextRenderer.BaseLine.Normal,
                                            Obj.Style.LineHeight,
                                            i < (paragraph.Lines.Count - 1) && paragraph.Lines.Count > 1, withVars);
                                    }
                                }
                                xml.Append(textStrings);

                                xml.AppendLine("</w:p>");
                                textStrings = null;
                                textStrings = new StringBuilder();
                            }
                            if (renderer.Paragraphs.Count == 0)
                                xml.Append(GetEmptyParagraph(fontSize));
                            break;
                    }

                }
            }
        }

        private FastString Export_HtmlParagraph(HtmlTextRenderer htmlTextRenderer, bool enableBr)
        {
            FastString sb = new FastString();

            string halign = "";
            switch (htmlTextRenderer.HorzAlign)
            {
                case HorzAlign.Left: halign = "left"; break;
                case HorzAlign.Right: halign = "right"; break;
                case HorzAlign.Center: halign = "center"; break;
                case HorzAlign.Justify: halign = "both"; break;
            }

            foreach (HtmlTextRenderer.Paragraph paragraph in htmlTextRenderer.Paragraphs)
            {
                sb.Append("<w:p><w:pPr><w:jc w:val=").Append(Quoted(halign)).Append(" />");
                if (htmlTextRenderer.ParagraphFormat.FirstLineIndent > 0)
                    sb.Append("<w:ind w:firstLine=").Append(Quoted(4 + htmlTextRenderer.ParagraphFormat.FirstLineIndent * 15)).Append("/>");

                float[] tabStops = htmlTextRenderer.TabPositions;

                sb.Append("<w:tabs>");
                float tabstop = 0;
                for (int i = 0; i < tabStops.Length; i++)
                    if ((tabstop += tabStops[i]) > 0)
                        sb.Append("<w:tab w:val=\"left\" w:pos=\"").Append((int)(4 + tabstop * 15)).Append("\" />");
                sb.Append("</w:tabs>");
                //tabs

                sb.Append("<w:spacing w:before=\"0\" w:after=\"0\" w:line=");
                switch (htmlTextRenderer.ParagraphFormat.LineSpacingType)
                {
                    case LineSpacingType.Single:
                        sb.Append(Quoted(238));
                        sb.Append(" w:lineRule=\"auto\"");
                        break;
                    case LineSpacingType.Multiple:
                        sb.Append(Quoted((int)(238 * htmlTextRenderer.ParagraphFormat.LineSpacingMultiple)));
                        sb.Append(" w:lineRule=\"auto\"");
                        break;
                    case LineSpacingType.AtLeast:
                        sb.Append(Quoted((int)(20 * htmlTextRenderer.ParagraphFormat.LineSpacing * 0.75f)));
                        sb.Append(" w:lineRule=\"atLeast\"");
                        break;
                    case LineSpacingType.Exactly:
                        sb.Append(Quoted((int)(20 * htmlTextRenderer.ParagraphFormat.LineSpacing * 0.75f)));
                        sb.Append(" w:lineRule=\"exact\"");
                        break;
                }
                sb.Append(" w:beforeAutospacing=\"0\" w:afterAutospacing=\"0\" />");
                sb.Append("</w:pPr>"); ;
                bool notFirstLine = false;
                foreach (HtmlTextRenderer.Line line in paragraph.Lines)
                {
                    if (enableBr)
                    {
                        if (notFirstLine) { sb.Append("<w:r><w:br/></w:r>"); }
                        else notFirstLine = true;
                    }
                    foreach (HtmlTextRenderer.Word word in line.Words)
                    {
                        foreach (HtmlTextRenderer.Run run in word.Runs)
                        {
                            if (run is HtmlTextRenderer.RunText)
                            {
                                HtmlTextRenderer.RunText runText = run as HtmlTextRenderer.RunText;
                                long size = (long)(runText.Style.Size * 2 / DrawUtils.ScreenDpiFX);
                                bool italic = (runText.Style.FontStyle & FontStyle.Italic) == FontStyle.Italic;
                                bool underline = (runText.Style.FontStyle & FontStyle.Underline) == FontStyle.Underline;
                                bool bold = (runText.Style.FontStyle & FontStyle.Bold) == FontStyle.Bold;
                                bool strikeout = (runText.Style.FontStyle & FontStyle.Strikeout) == FontStyle.Strikeout;

                                sb.Append("<w:r><w:rPr>");
                                //                sb.AppendLine("<w:solidFill><w:srgbClr val=" + GetRGBString(TextColor) + " /></w:solidFill>");
                                sb.Append("<w:rFonts w:ascii=" + Quoted(runText.Style.Font) + " w:hAnsi=" + Quoted(runText.Style.Font) + " w:cs=" + Quoted(runText.Style.Font) + " /> ");
                                switch (runText.Style.BaseLine)
                                {
                                    case HtmlTextRenderer.BaseLine.Normal:
                                        break;
                                    case HtmlTextRenderer.BaseLine.Subscript:
                                        //size = (long)(runText.Style.Size * 2 * DrawUtils.ScreenDpiFX / 0.6);
                                        sb.Append("<w:vertAlign w:val=\"subscript\"/>");
                                        break;
                                    case HtmlTextRenderer.BaseLine.Superscript:
                                        //size = (long)(runText.Style.Size * 2 * DrawUtils.ScreenDpiFX / 0.6);
                                        sb.Append("<w:vertAlign w:val=\"superscript\"/>");
                                        break;
                                }
                                if (bold)
                                    sb.Append("<w:b />");
                                if (italic)
                                    sb.Append("<w:i />");
                                if (underline) sb.Append("<w:u w:val=" + Quoted("single") + "/>");
                                sb.Append("<w:color w:val=" + GetRGBString(run.Style.Color) + " />");
                                sb.Append("<w:sz w:val=" + Quoted(size) + " />");
                                sb.Append("<w:szCs w:val=" + Quoted(size) + " />");
                                if (htmlTextRenderer.RightToLeft)
                                    sb.Append("<w:rtl/>");
                                if (run.Style.BackgroundColor.A > 0)
                                {
                                    sb.Append("<w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"")
                                        .Append(run.Style.BackgroundColor.R.ToString("X2"))
                                        .Append(run.Style.BackgroundColor.G.ToString("X2"))
                                        .Append(run.Style.BackgroundColor.B.ToString("X2"))
                                        .Append("\" />");
                                }
                                sb.Append("</w:rPr>");
                                sb.Append("<w:t xml:space=\"preserve\">").Append(this.TranslateText(runText.Text)).Append("</w:t>");
                                sb.Append("</w:r>");
                            }
                            else if (run is HtmlTextRenderer.RunImage)
                            {
                                Export_HtmlParagraph(run as HtmlTextRenderer.RunImage, sb);
                            }
                        }
                    }
                }
                //runs
                sb.AppendLine("</w:p>");
            }
            return sb;
        }

        private void Export_HtmlParagraph(HtmlTextRenderer.RunImage runImage, FastString sb)
        {

            pictureCount++; // Increase picture counter
            OoPictureObject picture = new OoPictureObject("word/media/image" + pictureCount.ToString() + ".png");
            this.AddRelation(pictureCount + Word2007Export.RID_SHIFT, picture);
            float w, h;
            using (Bitmap bmp = runImage.GetBitmap(out w, out h))
            {
                string hash = picture.ComputeHash(bmp);
                picture.SetFileName(Path.Combine(Path.GetDirectoryName(picture.FileName),
                    hash + Path.GetExtension(picture.FileName)).Replace("\\", "/"));

                if (!wordExport.Zip.FileExists(picture.FileName))
                {
                    MemoryStream ms = new MemoryStream(); // stream will dispose after Zip ctreating
                    bmp.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    wordExport.Zip.AddStream(picture.FileName, ms);
                }
            }

            long cx = (long)(w * 360000 / 37.8f);
            long cy = (long)(h * 360000 / 37.8f);
            sb.Append("<w:drawing><wp:inline distT=\"0\" distB=\"0\" distL=\"0\" distR=\"0\">");
            sb.Append("<wp:extent cx=").Append(Quoted(cx)).Append(" cy=").Append(Quoted(cy)).Append(" />");
            sb.Append("<wp:docPr id=").Append(Quoted(pictureCount)).Append(" name=").Append(Quoted(pictureCount)).Append(" descr=\"Generated by FastReport\" />");
            sb.Append("<wp:cNvGraphicFramePr><a:graphicFrameLocks xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" noChangeAspect=\"1\" /></wp:cNvGraphicFramePr><a:graphic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"><a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/picture\"><pic:pic xmlns:pic=\"http://schemas.openxmlformats.org/drawingml/2006/picture\"><pic:nvPicPr>");
            sb.Append("<pic:cNvPr id=").Append(Quoted(pictureCount)).Append(" name=").Append(Quoted(pictureCount)).Append(" descr=\"Generated by FastReport\" />");
            sb.Append("<pic:cNvPicPr><a:picLocks noChangeAspect=\"1\" noChangeArrowheads=\"1\" /></pic:cNvPicPr></pic:nvPicPr><pic:blipFill>");
            sb.Append("<a:blip r:embed=").Append(Quoted(picture.rId)).Append(">");
            sb.Append("<a:extLst><a:ext uri=\"{28A0092B-C50C-407E-A947-70E740481C1C}\"><a14:useLocalDpi xmlns:a14=\"http://schemas.microsoft.com/office/drawing/2010/main\" val=\"0\" /></a:ext></a:extLst></a:blip><a:srcRect /><a:stretch><a:fillRect /></a:stretch></pic:blipFill><pic:spPr bwMode=\"auto\"><a:xfrm><a:off x=\"0\" y=\"0\" />");
            sb.Append("<a:ext cx=").Append(Quoted(cx)).Append(" cy=").Append(Quoted(cy)).Append(" />");
            sb.Append("</a:xfrm><a:prstGeom prst=\"rect\"><a:avLst /></a:prstGeom><a:noFill /><a:ln><a:noFill /></a:ln></pic:spPr></pic:pic></a:graphicData></a:graphic></wp:inline></w:drawing>");
        }

        private string GetEmptyParagraph(int fontSize)
        {
            return String.Format("<w:p><w:pPr><w:spacing w:before=\"0\" w:after=\"0\" w:line=\"0\" w:lineRule=\"auto\" w:beforeAutospacing=\"0\" w:afterAutospacing=\"0\" /><w:rPr><w:sz w:val=\"{0}\"/><w:szCs w:val=\"{0}\"/></w:rPr></w:pPr></w:p>", fontSize); //<w:r><w:rPr></w:rPr><w:t> </w:t></w:r>
        }

        private string Get_BorderLineStyle(BorderLine Obj)
        {
            switch (Obj.Style)
            {
                case LineStyle.Solid: return Quoted("single");
                case LineStyle.Double: return Quoted("double");
                case LineStyle.Dot: return Quoted("dotted");
                case LineStyle.Dash: return Quoted("dash");
                case LineStyle.DashDot: return Quoted("dotDash");
                case LineStyle.DashDotDot: return Quoted("dashDotDot");
            }
            return "";
        }

        private string Get_VerticalAlign(VertAlign align)
        {
            switch (align)
            {
                case VertAlign.Top: return Quoted("top");
                case VertAlign.Center: return Quoted("center");
                case VertAlign.Bottom: return Quoted("bottom");
            }
            return "";
        }

        private string Get_TabledRotation(ExportIEMObject obj)
        {
            switch (obj.Style.Angle)
            {
                case 90: return Quoted("tbRlV");
                case 270: return Quoted("btLr");
            }
            return Quoted("lrTb");
        }

        private void Export_TableCell(ExportIEMObject Obj, int CellWidth, int dx, int dy, StringBuilder xml)
        {
            xml.Append("<w:tc><w:tcPr>");
            if (Obj != null)
                xml.Append("<w:tcW w:w=").Append(Quoted((Obj.Width + 1) * 15)).Append(" w:type=\"dxa\" />");
            else
                xml.Append("<w:tcW w:w=").Append(Quoted((CellWidth + 1))).Append(" w:type=\"dxa\" />");

            if (dx > 1)
                xml.Append("<w:gridSpan w:val=").Append(Quoted(dx)).Append(" />");

            if (Obj != null)
            {
                Border border = Obj.Style.Border;

                xml.Append("<w:tcBorders>");
                if ((border.Lines & BorderLines.Top) != 0)
                    xml.Append("<w:top w:val=").
                        Append(Get_BorderLineStyle(border.TopLine)).Append("w:sz=").Append(Quoted(8 * border.TopLine.Width)).
                        Append(" w:space=\"0\" w:color=").Append(GetRGBString(border.TopLine.Color)).Append(" />");

                if ((border.Lines & BorderLines.Left) != 0)
                    xml.Append("<w:left w:val=").
                        Append(Get_BorderLineStyle(border.LeftLine)).Append("w:sz=").Append(Quoted(8 * border.LeftLine.Width)).
                        Append(" w:space=\"0\" w:color=").Append(GetRGBString(border.LeftLine.Color)).Append(" />");

                if ((border.Lines & BorderLines.Bottom) != 0)
                    xml.Append("<w:bottom w:val=").
                        Append(Get_BorderLineStyle(border.BottomLine)).Append("w:sz=").Append(Quoted(8 * border.BottomLine.Width)).
                        Append(" w:space=\"0\" w:color=").Append(GetRGBString(border.BottomLine.Color)).Append(" />");

                if ((border.Lines & BorderLines.Right) != 0)
                    xml.Append("<w:right w:val=").
                        Append(Get_BorderLineStyle(border.RightLine)).Append("w:sz=").Append(Quoted(8 * border.RightLine.Width)).
                        Append(" w:space=\"0\" w:color=").Append(GetRGBString(border.RightLine.Color)).Append(" />");

                xml.Append("</w:tcBorders>");

                if (dy > 1)
                {
                    if (Obj.Counter == 0)
                        xml.Append("<w:vMerge w:val=\"restart\" />");
                    else
                        xml.Append("<w:vMerge />");
                    Obj.Counter++;
                }

                string text_color = GetRGBString(Obj.Style.TextColor);
                if (Obj.Style.Fill is SolidFill)
                {
                    SolidFill fill = Obj.Style.Fill as SolidFill;
                    xml.Append("<w:shd w:val=\"clear\" w:color=").
                        Append(text_color).Append(" w:fill=").Append(GetRGBString(fill.Color)).Append(" />");
                }
                else if (Obj.Style.Fill is GlassFill)
                {
                    GlassFill fill = Obj.Style.Fill as GlassFill;
                    xml.Append("<w:shd w:val=\"clear\" w:color=").
                        Append(text_color).Append(" w:fill=").Append(GetRGBString(fill.Color)).Append(" />");
                }
                else if (Obj.Style.Fill is LinearGradientFill)
                {
                    LinearGradientFill fill = Obj.Style.Fill as LinearGradientFill;
                    Color col = fill.StartColor;
                    xml.Append("<w:shd w:val=\"clear\" w:color=").
                        Append(text_color).Append(" w:fill=").Append(GetRGBString(col)).Append(" />");
                }

                xml.Append("<w:vAlign w:val=").Append(Get_VerticalAlign(Obj.Style.VAlign)).Append(" />");

                // Rotation
                if (Obj.Style.Angle != 0)
                    xml.Append("<w:textDirection w:val=").Append(Get_TabledRotation(Obj)).Append(" />");

                xml.Append("<w:tcMar>");
                //if (Obj.Style.Padding.Top != 0)
                //    xml.Append("<w:top w:w=").Append(Quoted(Obj.Style.Padding.Top * 15)).Append(" w:type=\"dxa\"/>");
                xml.Append("<w:left w:w=").Append(Quoted(4 + Obj.Style.Padding.Left * 15)).Append(" w:type=\"dxa\"/>");
                //if (Obj.Style.Padding.Bottom != 0)
                //    xml.Append("<w:bottom w:w=").Append(Quoted(Obj.Style.Padding.Bottom * 15)).Append(" w:type=\"dxa\"/>");
                xml.Append("<w:right w:w=").Append(Quoted(4 + Obj.Style.Padding.Right * 15)).Append(" w:type=\"dxa\"/>").
                    Append("</w:tcMar>");
            }
            xml.Append("</w:tcPr>");
            if (Obj != null && Obj.Counter < 2)
            {
                if (Obj.IsText)
                    Export_Paragraph(Obj, xml, false, false);
                else
                    Export_Picture(Obj, xml, false);
            }
            else
                xml.Append(GetEmptyParagraph(1));
            xml.Append("</w:tc>");
        }

        private void Export_ColumnCell(ExportIEMObject Obj, int CellWidth, int dx, int dy, StringBuilder xml)
        {
            if (Obj != null)
            {
                if (dy > 1)
                    Obj.Counter++;

                string text_color = GetRGBString(Obj.Style.TextColor);
                if (Obj.Style.Fill is SolidFill)
                {
                    SolidFill fill = Obj.Style.Fill as SolidFill;
                    xml.Append("<w:shd w:val=\"clear\" w:color=").
                        Append(text_color).Append(" w:fill=").Append(GetRGBString(fill.Color) + " />");
                }
                else if (Obj.Style.Fill is GlassFill)
                {
                    GlassFill fill = Obj.Style.Fill as GlassFill;
                    xml.Append("<w:shd w:val=\"clear\" w:color=").
                        Append(text_color).Append(" w:fill=").Append(GetRGBString(fill.Color) + " />");
                }
                else if (Obj.Style.Fill is LinearGradientFill)
                {
                    LinearGradientFill fill = Obj.Style.Fill as LinearGradientFill;
                    Color col = fill.StartColor;
                    xml.Append("<w:shd w:val=\"clear\" w:color=").
                        Append(text_color).Append(" w:fill=").Append(GetRGBString(col)).Append(" />");
                }

                xml.Append("<w:pMar>").
                    Append("<w:left w:w=").Append(Quoted(4 + Obj.Style.Padding.Left * 15)).Append(" w:type=\"dxa\"/>").
                    Append("<w:right w:w=").Append(Quoted(4 + Obj.Style.Padding.Right * 15)).Append(" w:type=\"dxa\"/>").
                    Append("</w:pMar>");

                if (Obj.Counter < 2)
                {
                    if (Obj.IsText)
                        Export_CellParagraph(Obj, xml);
                    else
                        Export_Picture(Obj, xml, true);
                }
            }
            else
                //xml.Append("<w:p><w:pPr><w:rPr></w:rPr></w:pPr><w:r><w:rPr></w:rPr><w:t> </w:t></w:r></w:p>");
                xml.Append(GetEmptyParagraph(1));
        }

        private void Expport_RichParagraph(ExportIEMObject richObject, StringBuilder xml)
        {
            using (RTF_DocumentParser parser = new RTF_DocumentParser())
            {
                parser.Load(richObject.Text);
                using (RTF_ToDocX saver = new RTF_ToDocX(parser.Document))
                {
                    Rectangle pad = new Rectangle(
                      (int)(20 * richObject.Left),
                      (int)0,
                      (int)(20 * richObject.Left),
                      (int)0);
                    saver.Padding = pad;
                    xml.Append(saver.DocX);
                }
            }
        }

        private void Export_CellParagraph(ExportIEMObject Obj, StringBuilder xml)
        {
            if (Obj.OriginalText != null)
            {
                bool withVars = false;
                if (Obj.OriginalText.IndexOf("[Page#]") != -1)
                {
                    Obj.Text = Obj.OriginalText;
                    withVars = true;
                }
                if (!Obj.IsRichText)
                    Export_Paragraph(Obj, xml, true, withVars);
                else
                    Expport_RichParagraph(Obj, xml);
            }
            else
            {
                if (!Obj.IsRichText)
                    Export_Paragraph(Obj, xml, true, false);
                else
                    Expport_RichParagraph(Obj, xml);
            }

        }

        private void Export_TableRow(ExportMatrix Matrix, int y, Stream file_xml, bool ParagraphBased, string pgSz)
        {
            List<int> colWidths = new List<int>();
            StringBuilder xml = new StringBuilder(256);

            if (!ParagraphBased)
            {
                xml.Append("<w:tr><w:trPr>");
                int ht;
                if (Matrix.RowHeightIs == "min")
                    ht = (int)Math.Round((Matrix.YPosById(y + 1) - Matrix.YPosById(y)) * 14f);
                else
                    ht = (int)Math.Round((Matrix.YPosById(y + 1) - Matrix.YPosById(y)) * 15f);
                xml.Append("<w:trHeight w:hRule=\"").Append(Matrix.RowHeightIs).
                    Append("\" w:val=").Append(Quoted(ht)).Append("/></w:trPr>");
            }

            for (int x = 0; x < Matrix.Width - 1;)
            {
                int w = (int)Math.Round((Matrix.XPosById(x + 1) - Matrix.XPosById(x)) * 15f, 0);
                int i = Matrix.Cell(x, y);
                if (i != -1)
                {
                    int fx, fy, dx, dy;
                    Matrix.ObjectPos(i, out fx, out fy, out dx, out dy);
                    ExportIEMObject Obj = Matrix.ObjectById(i);
                    if (ParagraphBased)
                        Export_ColumnCell(Obj, w, dx, dy, xml);
                    else
                        Export_TableCell(Obj, w, dx, dy, xml);
                    int colw = (int)Math.Round((Matrix.XPosById(x + dx) - Matrix.XPosById(x)) * 15f, 0);
                    colWidths.Add(colw);
                    x += dx;
                }
                else
                {
                    if (!ParagraphBased)
                        Export_TableCell(null, w, 0, 0, xml);
                    x++;
                }
            }

            if (!ParagraphBased)
                xml.Append("</w:tr>");
            else
            {
                if (pgSz != null)
                {
                    xml.Append("<w:p><w:pPr><w:rPr></w:rPr><w:sectPr><w:type w:val=\"continuous\"/>");
                    xml.Append(pgSz);
                    if (colWidths.Count > 1)
                    {
                        xml.AppendFormat("<w:cols w:num=\"{0}\" w:space=\"0\" w:equalWidth=\"0\">", colWidths.Count);
                        foreach (int width in colWidths)
                            xml.AppendFormat("<w:col w:w=\"{0}\" w:space=\"0\"/>", width);
                        xml.Append("</w:cols>");
                    }
                    else
                        xml.Append("<w:cols w:space=\"0\"/>");
                    xml.Append("</w:sectPr></w:pPr></w:p>");
                }
            }
            ExportUtils.Write(file_xml, xml);
        }

        private void Export_TableGrid(ExportMatrix Matrix, Stream file)
        {
            ExportUtils.WriteLn(file, "<w:tblGrid>");
            for (int x = 1; x < Matrix.Width; x++)
            {
                float w = (float)Math.Round((Matrix.XPosById(x) - Matrix.XPosById(x - 1)) * 15f);// / 6.3f;
                string s = ExportUtils.FloatToString(w);
                ExportUtils.WriteLn(file, "<w:gridCol w:w=" + Quoted(s) + " />");
            }
            ExportUtils.WriteLn(file, "</w:tblGrid>");
        }
        #endregion

        internal void AppendLine(string str)
        {
            textStrings.AppendLine(str);
        }

        internal void ExportSinglePage(ExportMatrix FMatrix, bool IsLastPage, Stream file, bool ParagraphBased, bool isHeaderFooter)
        {
            if (!ParagraphBased)
            {
                ExportUtils.WriteLn(file, "<w:tbl>");
                Export_TableProperties(file);
                Export_TableGrid(FMatrix, file);
            }

            for (int y = 0; y < FMatrix.Height - 1; y++)
                Export_TableRow(FMatrix, y, file, ParagraphBased, null);

            if (!ParagraphBased)
                ExportUtils.WriteLn(file, "</w:tbl>");

            {
                const int PageNo = 0;
                page_state state = new page_state(
                    FMatrix.Landscape(PageNo),
                    (long)(FMatrix.PageWidth(PageNo) * 567 + 4) / 10,
                    (long)(FMatrix.PageHeight(PageNo) * 567 + 4) / 10,
                    (long)(FMatrix.PageTMargin(PageNo) * 530 + 4) / 10,
                    (long)(FMatrix.PageBMargin(PageNo) * 530 + 4) / 10,
                    (long)(FMatrix.PageLMargin(PageNo) * 567 + 4) / 10,
                    (long)(FMatrix.PageRMargin(PageNo) * 567 + 4) / 10,
                    ParagraphBased,
                    isHeaderFooter);

                if (!IsLastPage && !ParagraphBased)
                    ExportUtils.WriteLn(file, "<w:p><w:pPr><w:rPr> <w:sz w:val=\"0\"" +
                              " /><w:szCs w:val=\"0\"/></w:rPr></w:pPr><w:r>" +
                              "<w:br w:type=\"page\" /></w:r></w:p>");

                string pgsz = GetPageSize(state);

                if (!String.IsNullOrEmpty(pgsz))
                {

                    if (!IsLastPage)
                    {
                        ExportUtils.WriteLn(file, "<w:p><w:pPr><w:sectPr>");
                        ExportUtils.WriteLn(file, GetPageSize(state));
                        ExportUtils.WriteLn(file, "</w:sectPr></w:pPr></w:p>");
                    }
                    else
                    {
                        ExportUtils.WriteLn(file, "<w:sectPr>");
                        ExportUtils.WriteLn(file, GetPageSize(state));
                        ExportUtils.WriteLn(file, "</w:sectPr>");
                    }
                }
            }
        }

        internal void Export(Word2007Export OoXML, List<ExportMatrix> pages)
        {
            MemoryStream file = new MemoryStream();

            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<w:document xmlns:ve=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\" xmlns:w10=\"urn:schemas-microsoft-com:office:word\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\">");
            ExportUtils.WriteLn(file, "<w:body>");

            int CurPage = 0;
            foreach (ExportMatrix matrix in pages)
            {
                ++CurPage;
                ExportSinglePage(matrix, CurPage == pages.Count, file, OoXML.ParagraphBased, false);
            }

            //insert empty text with minimum size for avoiding creating a new page
            //*
            if (!OoXML.ParagraphBased)
            {
                ExportUtils.Write(file, "<w:p>");
                ExportUtils.Write(file, "<w:r>");
                ExportUtils.Write(file, "<w:rPr>");
                ExportUtils.Write(file, "<w:color w:val = \"#FFFFFF\"/>");
                ExportUtils.Write(file, "<w:sz w:val = \"2\"/>");
                ExportUtils.Write(file, "<w:szCs w:val = \"2\"/>");
                ExportUtils.Write(file, "</w:rPr>");
                ExportUtils.Write(file, "<w:t>.</w:t>");
                ExportUtils.Write(file, "</w:r>");
                ExportUtils.Write(file, "</w:p>");
            }
            /// WtiteSectionProperties(OoXML, file);
            ExportUtils.WriteLn(file, "</w:body>");
            ExportUtils.WriteLn(file, "</w:document>");

            file.Position = 0;
            OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);

            ExportRelations(OoXML);
        }

#if READONLY_STRUCTS
        internal readonly struct page_state
#else
        internal struct page_state
#endif
        {
            internal readonly bool Landscape;
            internal readonly long PageWidth;
            internal readonly long PageHeight;
            internal readonly long Top;
            internal readonly long Bottom;
            internal readonly long Left;
            internal readonly long Right;
            internal readonly bool ParagraphBased;
            internal readonly bool IsHeaderFooter;

            public page_state(bool landscape, long pageWidth, long pageHeight, long top, long bottom,
                long left, long right, bool paragraphBased, bool isHeaderFooter)
            {
                Landscape = landscape;
                PageWidth = pageWidth;
                PageHeight = pageHeight;
                Top = top;
                Bottom = bottom;
                Left = left;
                Right = right;
                ParagraphBased = paragraphBased;
                IsHeaderFooter = isHeaderFooter;
            }
        }

        internal page_state current_page_state;

        internal string GetPageSize(page_state state)
        {
            StringBuilder sb = new StringBuilder();
            current_page_state = state;

            if (!state.IsHeaderFooter)
            {
                if (state.ParagraphBased)
                {
                    sb.AppendLine("<w:headerReference w:type=\"default\" r:id=\"rId6\"/>");
                    sb.AppendLine("<w:footerReference w:type=\"default\" r:id=\"rId7\"/>");
                }


                if (state.Landscape == false)
                {
                    sb.AppendLine("<w:pgSz w:w=" + Quoted(state.PageWidth) + " w:h=" + Quoted(state.PageHeight) + "/>");
                    sb.AppendLine("<w:pgMar w:top=" + Quoted(state.Top) +
                        " w:right=" + Quoted(state.Right) +
                        " w:bottom=" + Quoted(state.Bottom) +
                        " w:left=" + Quoted(state.Left) +
                        " w:header=" + Quoted(state.Top) +
                        " w:footer=" + Quoted(state.Bottom) +
                        " w:gutter=" + Quoted(0) + "/>");
                }
                else
                {
                    sb.AppendLine("<w:pgSz w:w=" + Quoted(state.PageWidth) + " w:h=" + Quoted(state.PageHeight) + "w:orient=" + Quoted("landscape") + "/>");
                    sb.AppendLine("<w:pgMar w:top=" + Quoted(state.Top) +
                        " w:right=" + Quoted(state.Right) +
                        " w:bottom=" + Quoted(state.Bottom) +
                        " w:left=" + Quoted(state.Left) +
                        " w:header=" + Quoted(304) +
                        " w:footer=" + Quoted(304) +
                        " w:gutter=" + Quoted(0) + "/>");
                }
            }
            return sb.ToString();
        }

        internal string GetPageSize(Word2007Export OoXML)
        {
            StringBuilder sb = new StringBuilder();
            long PageWidth = (long)(OoXML.PageWidth * 567 + 4) / 10;
            long PageHeight = (long)(OoXML.PageHeight * 567 + 4) / 10;
            long Top = (long)(OoXML.TopMargin * 530 + 4) / 10;
            long Bottom = (long)(OoXML.BottomMargin * 530 + 4) / 10;
            long Left = (long)(OoXML.LeftMargin * 567 + 4) / 10;
            long Right = (long)(OoXML.RightMargin * 567 + 4) / 10;

            if (OoXML.ParagraphBased)
            {
                sb.AppendLine("<w:headerReference w:type=\"default\" r:id=\"rId6\"/>");
                sb.AppendLine("<w:footerReference w:type=\"default\" r:id=\"rId7\"/>");
            }

            if (OoXML.Landscape == false)
            {
                sb.AppendLine("<w:pgSz w:w=" + Quoted(PageWidth) + " w:h=" + Quoted(PageHeight) + "/>");
                sb.AppendLine("<w:pgMar w:top=" + Quoted(Top) +
                    " w:right=" + Quoted(Right) +
                    " w:bottom=" + Quoted(Bottom) +
                    " w:left=" + Quoted(Left) +
                    " w:header=" + Quoted(Top) +
                    " w:footer=" + Quoted(Bottom) +
                    " w:gutter=" + Quoted(0) + "/>");
            }
            else
            {
                sb.AppendLine("<w:pgSz w:w=" + Quoted(PageWidth) + " w:h=" + Quoted(PageHeight) + "w:orient=" + Quoted("landscape") + "/>");
                sb.AppendLine("<w:pgMar w:top=" + Quoted(Top) +
                    " w:right=" + Quoted(Right) +
                    " w:bottom=" + Quoted(Bottom) +
                    " w:left=" + Quoted(Left) +
                    " w:header=" + Quoted(304) +
                    " w:footer=" + Quoted(304) +
                    " w:gutter=" + Quoted(0) + "/>");
            }
            return sb.ToString();
        }

        internal void ExportPageBegin(ReportPage page, bool matrix_based, bool last_page)
        {
            //if(! matrix_based)
            //{
            //  AppendLine("<w:p>");
            //}
        }

        internal void ExportPageEnd(ReportPage page, bool matrix_based, bool last_page)
        {
            if (!matrix_based)
            {
                OoXMLDocument.page_state state = new OoXMLDocument.page_state(
                    page.Landscape,
                    (long)(page.Width * 567 / Units.Millimeters + 4) / 10,
                    (long)(page.Height * 567 / Units.Millimeters + 4) / 10,
                    (long)(page.TopMargin * 530 + 4) / 10,
                    (long)(page.BottomMargin * 530 + 4) / 10,
                    (long)(page.LeftMargin * 567 + 4) / 10,
                    (long)(page.RightMargin * 567 + 4) / 10,
                    default(bool),
                    default(bool));

                //AppendLine("</w:p>");

                if (!state.Equals(current_page_state))
                {
                    AppendLine("</w:p><w:p><w:pPr><w:sectPr>");
                    AppendLine(GetPageSize(state));
                    AppendLine("</w:sectPr></w:pPr>");
                    if (!last_page)
                        AppendLine("<w:br w:type=" + Quoted("page") + " />");
                    AppendLine("</w:p><w:p>");
                    current_page_state = state;
                }
                else if (!last_page)
                    AppendLine("</w:p><w:p><w:br w:type=" + Quoted("page") + " /></w:p><w:p>");
            }
        }

        internal void Open_Paragraph(ReportComponentBase Obj, bool exportTopPadding, bool exportBottomPadding)
        {
            string align = "ctr";
            float font_size = 12;
            TextObject text_obj = (Obj is TextObject) ? Obj as TextObject : null;

            if (Obj is TextObject)
            {
                switch (text_obj.HorzAlign)
                {
                    case HorzAlign.Left: align = "left"; break;
                    case HorzAlign.Right: align = "right"; break;
                    case HorzAlign.Center: align = "center"; break;
                    case HorzAlign.Justify: align = "both"; break;
                }
                font_size = 2 * text_obj.Font.Size;
            }
            long spacing = 240;// (long)(15 * text_obj.Font.Height + 10 * text_obj.LineHeight);
                               //now, 240 is a single interval

            textStrings.Append(
                "<w:p><w:pPr><w:spacing w:after=");
            if (Obj is TextObject)
            {
                TextObject text = Obj as TextObject;
                textStrings.Append(exportBottomPadding ? Quoted(text.Padding.Bottom * 1440 / DrawUtils.ScreenDpi) : Quoted(0));
                if (exportTopPadding)
                {
                    textStrings.Append(" w:before=" + Quoted(text.Padding.Top * 1440 / DrawUtils.ScreenDpi));
                }
            }
            else
            {
                textStrings.Append("\"0\"");
            }
            textStrings.AppendLine(" w:line=" + Quoted(spacing) + " w:lineRule=\"auto\"/>");

            if (Obj.Border.Lines != BorderLines.None)
            {
                textStrings.AppendLine("<w:pBdr>");
                string borderLine = "<w:{0} w:val=\"single\" w:sz=\"{1}\" w:space=\"1\" w:color={2}/>";
                if ((Obj.Border.Lines & BorderLines.Top) != 0 && Obj.Border.TopLine.Width > 0)
                    textStrings.AppendFormat(borderLine, "top",
                        Obj.Border.TopLine.Width * 8, GetRGBString(Obj.Border.TopLine.Color));
                if ((Obj.Border.Lines & BorderLines.Bottom) != 0 && Obj.Border.BottomLine.Width > 0)
                    textStrings.AppendFormat(borderLine, "bottom",
                        Obj.Border.BottomLine.Width * 8, GetRGBString(Obj.Border.BottomLine.Color));
                if ((Obj.Border.Lines & BorderLines.Left) != 0 && Obj.Border.LeftLine.Width > 0)
                    textStrings.AppendFormat(borderLine, "left",
                        Obj.Border.LeftLine.Width * 8, GetRGBString(Obj.Border.LeftLine.Color));
                if ((Obj.Border.Lines & BorderLines.Right) != 0 && Obj.Border.RightLine.Width > 0)
                    textStrings.AppendFormat(borderLine, "right",
                        Obj.Border.RightLine.Width * 8, GetRGBString(Obj.Border.RightLine.Color));
                textStrings.AppendLine("</w:pBdr>");
            }

            if (Obj is TextObject)
            {
                float left = (Obj as TextObject).Padding.Left / Utils.Units.Millimeters * 56.7f;
                float right = (Obj as TextObject).Padding.Right / Utils.Units.Millimeters * 56.7f;
                if (left > 0 || right > 0)
                    textStrings.AppendLine(
                        "<w:ind w:left=" + Quoted(left) + " w:right=" + Quoted(right) + "/>"
                        );
            }
            textStrings.AppendLine("<w:jc w:val=" + Quoted(align) +
                " /><w:rPr><w:sz w:val=" + Quoted(font_size) +
                "/><w:szCs w:val=" + Quoted(font_size) + "/></w:rPr></w:pPr>");
        }

        private void Add_Run(
            Font Font,
            Color TextColor,
            string Text,
            string URL,
            bool AddSpace,
            bool RTL,
            AdvancedTextRenderer.BaseLine Baseline,
            float LineHeight,
            bool addBR,
            bool withVars)
        {
            long Size = (long)(Font.Size * 2 / DrawUtils.ScreenDpiFX);
            bool Italic = Font.Italic;
            bool Underline = Font.Underline;

            if (Text != null)
            {
                if (URL != null && URL.Length > 0)
                {
                    ++pictureCount;
                    int rId = Word2007Export.RID_SHIFT + pictureCount;
                    AddRelation(rId, new OoHyperlink(Utils.Converter.ToXml(URL)));
                    textStrings.Append(string.Format("<w:hyperlink r:id = \"rId{0}\">", rId));
                }

                textStrings.Append("<w:r><w:rPr>");
                //                FTextStrings.AppendLine("<w:solidFill><w:srgbClr val=" + GetRGBString(TextColor) + " /></w:solidFill>");
                textStrings.Append("<w:rFonts w:ascii=" + Quoted(Font.Name) + " w:hAnsi=" + Quoted(Font.Name) + " w:cs=" + Quoted(Font.Name) + " w:eastAsia="+Quoted(Font.Name)+ " /> ");
                switch (Baseline)   
                {
                    case AdvancedTextRenderer.BaseLine.Normal:
                        break;
                    case AdvancedTextRenderer.BaseLine.Subscript:
                        Size = (long)(Font.Size * 2 / DrawUtils.ScreenDpiFX / 0.6);
                        textStrings.Append("<w:vertAlign w:val=\"subscript\"/>");
                        break;
                    case AdvancedTextRenderer.BaseLine.Superscript:
                        Size = (long)(Font.Size * 2 / DrawUtils.ScreenDpiFX / 0.6);
                        textStrings.Append("<w:vertAlign w:val=\"superscript\"/>");
                        break;
                }
                if (Font.Bold)
                    textStrings.Append("<w:b />");
                if (Font.Italic)
                    textStrings.Append("<w:i />");
                if (Font.Underline)
                    textStrings.Append("<w:u w:val=" + Quoted("single") + "/>");
                if (Font.Strikeout)
                    textStrings.Append("<w:strike w:val=" + Quoted("true") + "/>");
                textStrings.Append("<w:color w:val=" + GetRGBString(TextColor) + " />");
                textStrings.Append("<w:sz w:val=" + Quoted(Size) + " />");
                textStrings.Append("<w:szCs w:val=" + Quoted(Size) + " />");
                if (RTL)
                    textStrings.Append("<w:rtl/>");
                textStrings.Append("</w:rPr>");
                if (withVars)
                {
                    Text = Text.Replace("[Page#]", "<w:r><w:fldChar w:fldCharType=\"begin\"/></w:r><w:r><w:instrText>PAGE   \\* MERGEFORMAT</w:instrText></w:r><w:r><w:fldChar w:fldCharType=\"end\"/></w:r>");
                    textStrings.Append("<w:t>").Append(Text).Append("</w:t>");
                }
                else
                    textStrings.Append("<w:t>").Append(this.TranslateText(Text)).Append("</w:t>");
                textStrings.Append("</w:r>");
                if (URL != null && URL.Length > 0)
                {
                    textStrings.Append("</w:hyperlink>");
                }
                if (addBR)
                    textStrings.Append("<w:br/>");
                if (AddSpace)
                    textStrings.Append("<w:r><w:rPr /><w:t xml:space=\"preserve\"> </w:t></w:r>");
            }
        }

        internal void Close_Paragraph()
        {
            textStrings.AppendLine("</w:p>");
        }

        private void InsertTabulation(float p)
        {
            textStrings.AppendLine("<w:pPr><w:ind w:firstLine=" + Quoted(p) + "/></w:pPr>");
        }

        private void AddTable(TableBase table)
        {
            using (TextObject tableBack = new TextObject())
            {
                tableBack.Left = table.AbsLeft;
                tableBack.Top = table.AbsTop;
                float tableWidth = 0;
                if (table.RowCount != 0)
                    for (int i = 0; i < table.ColumnCount; i++)
                        tableWidth += table[i, 0].Width;
                tableBack.Width = (tableWidth < table.Width) ? tableWidth : table.Width;
                tableBack.Height = table.Height;
                tableBack.Fill = table.Fill;
                tableBack.Text = "";

                // exporting the table fill
                AddTextObject(tableBack, null);

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

                            AddTextObject(textcell, null);
                        }
                        y += (table.Rows[i]).Height;
                    }
                    x += (table.Columns[j]).Width;
                }

                // exporting the table border
                tableBack.Fill = new SolidFill();
                tableBack.Border = table.Border;
                AddTextObject(tableBack, null);
            }
        }

        private void AddBandObject(BandBase band)
        {
            if (band.HasBorder || band.HasFill) using (TextObject newObj = new TextObject())
                {
                    newObj.Left = band.AbsLeft;
                    newObj.Top = band.AbsTop;
                    newObj.Width = band.Width;
                    newObj.Height = band.Height;
                    newObj.Fill = band.Fill;
                    newObj.Border = band.Border;
                    newObj.Text = "";
                    AddTextObject(newObj, null);
                }
        }

        public OoXMLDocument(OOExportBase OoXML)
        {
            wordExport = OoXML;
            textStrings = new StringBuilder();
            checkboxList = new Dictionary<string, OoPictureObject>();
            pictureCount = 0;
        }

        private void AddCheckboxObject(CheckBoxObject checkbox)
        {
            OoPictureObject pic;
            string KEY = checkbox.Name + checkbox.Checked.ToString();
            if (!checkboxList.ContainsKey(KEY))
            {
                pic = AddPictureObject(checkbox, "word/media/RichTextImage");
                checkboxList.Add(KEY, pic);
            }
            else
            {
                pic = checkboxList[KEY];
                //                pic.MoveObject(checkbox);
            }

        }
    }

    /// <summary>
    /// MS Word 2007 export class
    /// </summary>
    public partial class Word2007Export : OOExportBase
    {
        internal const int RID_SHIFT = 20;
        const int PRINT_OPTIMIZED_DPI = 300;

        /// <summary>
        /// Types of table rows height
        /// </summary>
        public enum RowHeightType
        {
            /// <summary>
            /// Exactly height
            /// </summary>
            Exactly,
            /// <summary>
            /// Minimum height
            /// </summary>
            Minimum
        }

        #region Public properties
        /// <summary>
        /// Enable or disable matrix view of document
        /// </summary>
        public bool MatrixBased
        {
            get { return matrixBased; }
            set { matrixBased = value; }
        }

        /// <summary>
        /// Enable or disable Paragraph view of document
        /// </summary>
        public bool ParagraphBased
        {
            get { return paragraphBased; }
            set
            {
                if (value)
                    MatrixBased = true;
                paragraphBased = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the wysiwyg mode should be used 
        /// for better results.
        /// </summary>
        /// <remarks>
        /// Default value is <b>true</b>. In wysiwyg mode, the resulting Excel file will look
        /// as close as possible to the prepared report. On the other side, it may have a lot 
        /// of small rows/columns, which will make it less editable. If you set this property
        /// to <b>false</b>, the number of rows/columns in the resulting file will be decreased.
        /// You will get less wysiwyg, but more editable file.
        /// </remarks>
        public bool Wysiwyg
        {
            get { return wysiwyg; }
            set { wysiwyg = value; }
        }

        /// <summary>
        /// Gets or sets the type of height calculation.
        /// </summary>
        [System.Obsolete("RowHeightIs property is deprecated, please use RowHeight property instead.")]
        public string RowHeightIs
        {
            get { return rowHeightIs; }
            set
            {
                rowHeightIs = string.IsNullOrEmpty(value) || IsWrongRowHeight(value) ? "exact" : value;
            }
        }
        /// <summary>
        /// Gets or sets the type of height calculation.
        /// </summary>
        public RowHeightType RowHeight
        {
            get { return rowHeight; }
            set
            {
                rowHeight = value;
                if (value == RowHeightType.Exactly)
                    rowHeightIs = "exact";
                else if (value == RowHeightType.Minimum)
                    rowHeightIs = "min";
            }
        }

        /// <summary>
        /// Enable or disable a resolution optimization.
        /// </summary>
        public bool PrintOptimized
        {
            get { return printOptimized; }
            set { printOptimized = value; }
        }

        #endregion

        #region Private fields
        private OoXMLCoreDocumentProperties coreDocProp;
        private OoXMLApplicationProperties applicationProp;
        private OoXMLDocument document;
        private OoXMLFontTable fontTable;
        private OoXMLWordStyles wordStyles;
        private OoXMLFootNotes footNotes;
        private OoXMLEndNotes endNotes;
        private OoXMLHeader header;
        private OoXMLFooter footer;
        private OoXMLWordSettings wordSettings;
        internal List<ExportMatrix> matrixList;
        private bool matrixBased = true;
        private bool paragraphBased = false;
        private bool wysiwyg = true;
        private string rowHeightIs;
        private RowHeightType rowHeight = RowHeightType.Exactly;
        private ReportPage prepared_page;
        private bool doNotExpandShiftReturn = true;
        private ExportMatrix matrix;
        private ExportMatrix headerMatrix;
        private ExportMatrix footerMatrix;
        private int pageNo;
        private bool firstPage = true;
        private bool printOptimized = false;
        #endregion

        #region Properties
        internal OoXMLFontTable FontTable { get { return fontTable; } }
        internal OoXMLWordStyles WordStyles { get { return wordStyles; } }
        internal OoXMLWordSettings WordSettings { get { return wordSettings; } }

        internal ExportMatrix HeaderMatrix { get { return headerMatrix; } }
        internal ExportMatrix FooterMatrix { get { return footerMatrix; } }


        internal ReportPage BasePage
        {
            get
            {
                if (prepared_page == null)
                    prepared_page = this.GetPage(0);
                return prepared_page;
            }
        }
        internal OoXMLDocument OoXMLDocument
        {
            get { return document; }
        }
        internal float PageWidth
        {
            get { return ExportUtils.GetPageWidth(BasePage); }
        }
        internal float PageHeight
        {
            get { return ExportUtils.GetPageHeight(BasePage); }
        }
        internal float TopMargin
        {
            get { return BasePage.TopMargin; }
        }
        internal float LeftMargin
        {
            get { return BasePage.LeftMargin; }
        }
        internal float RightMargin
        {
            get { return BasePage.RightMargin; }
        }
        internal float BottomMargin
        {
            get { return BasePage.BottomMargin; }
        }
        internal bool Landscape
        {
            get { return BasePage.Landscape; }
        }
        internal Border Border
        {
            get { return BasePage.Border; }
        }

        /// <summary>
        /// Enable or disable DoNotExpandShiftReturn.
        /// </summary>
        public bool DoNotExpandShiftReturn
        {
            get { return doNotExpandShiftReturn; }
            set { doNotExpandShiftReturn = value; }
        }
        #endregion

        #region Protected Methods

        private void CreateContentTypes()
        {
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");
            ExportUtils.WriteLn(file, "<Default Extension=\"png\" ContentType=\"image/png\" />");
            ExportUtils.WriteLn(file, "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\" />");
            ExportUtils.WriteLn(file, "<Default Extension=\"xml\" ContentType=\"application/xml\" />");
            ExportUtils.WriteLn(file, "<Override PartName=" + QuotedRoot(document.FileName) + " ContentType=" + Quoted(document.ContentType) + "/>");
            ExportUtils.WriteLn(file, "<Override PartName=" + QuotedRoot(wordStyles.FileName) + " ContentType=" + Quoted(wordStyles.ContentType) + "/>");
            ExportUtils.WriteLn(file, "<Override PartName=" + QuotedRoot(applicationProp.FileName) + " ContentType=" + Quoted(applicationProp.ContentType) + "/>");
            ExportUtils.WriteLn(file, "<Override PartName=" + QuotedRoot(wordSettings.FileName) + " ContentType=" + Quoted(wordSettings.ContentType) + "/>");
            ExportUtils.WriteLn(file, "<Override PartName=" + QuotedRoot(fontTable.FileName) + " ContentType=" + Quoted(fontTable.ContentType) + "/>");
            ExportUtils.WriteLn(file, "<Override PartName=" + QuotedRoot(coreDocProp.FileName) + " ContentType=" + Quoted(coreDocProp.ContentType) + "/>");
            if (paragraphBased)
            {
                ExportUtils.WriteLn(file, "<Override PartName=" + QuotedRoot(header.FileName) + " ContentType=" + Quoted(header.ContentType) + "/>");
                ExportUtils.WriteLn(file, "<Override PartName=" + QuotedRoot(footer.FileName) + " ContentType=" + Quoted(footer.ContentType) + "/>");
                ExportUtils.WriteLn(file, "<Override PartName=" + QuotedRoot(footNotes.FileName) + " ContentType=" + Quoted(footNotes.ContentType) + "/>");
                ExportUtils.WriteLn(file, "<Override PartName=" + QuotedRoot(endNotes.FileName) + " ContentType=" + Quoted(endNotes.ContentType) + "/>");
            }
            ExportUtils.WriteLn(file, "</Types>");
            file.Position = 0;
            Zip.AddStream("[Content_Types].xml", file);
        }

        private void CreateRelations()
        {
            MemoryStream file = new MemoryStream();
            ExportUtils.WriteLn(file, xml_header);
            ExportUtils.WriteLn(file, "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId3\" Type=" + Quoted(applicationProp.RelationType) + " Target=" + Quoted(applicationProp.FileName) + " />");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId2\" Type=" + Quoted(coreDocProp.RelationType) + " Target=" + Quoted(coreDocProp.FileName) + " />");
            ExportUtils.WriteLn(file, "<Relationship Id=\"rId1\" Type=" + Quoted(document.RelationType) + " Target=" + Quoted(document.FileName) + " />");
            ExportUtils.WriteLn(file, "</Relationships>");
            file.Position = 0;
            Zip.AddStream("_rels/.rels", file);
        }

        private void CreateHeadersAndNotes()
        {
            if (paragraphBased && headerMatrix != null && footerMatrix != null)
            {
                headerMatrix.Prepare();
                footerMatrix.Prepare();
                header.Export(this);
                footer.Export(this);
                footNotes.Export(this);
                endNotes.Export(this);
            }
        }

        private void ExportOOXML(Stream Stream)
        {
            CreateHeadersAndNotes();
            CreateRelations();
            CreateContentTypes();
            applicationProp.Export(this);
            coreDocProp.Export(this);
            fontTable.Export(this);
            wordStyles.Export(this);
            wordSettings.Export(this);
            if (MatrixBased)
                document.Export(this, matrixList);
            else
                document.ExportLayers(this);
        }
        #endregion

        

        #region Protected Methods
        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            Zip = new ZipArchive();
            coreDocProp = new OoXMLCoreDocumentProperties();
            applicationProp = new OoXMLApplicationProperties();
            document = new OoXMLDocument(this);
            fontTable = new OoXMLFontTable();
            wordStyles = new OoXMLWordStyles();
            wordSettings = new OoXMLWordSettings();

            // Set relations to presentation.xml.rels
            document.AddRelation(1, wordSettings);
            document.AddRelation(2, wordStyles);
            document.AddRelation(3, fontTable);

            matrixList = new List<ExportMatrix>();
            pageNo = 0;
            firstPage = true;
            if (paragraphBased)
            {
                header = new OoXMLHeader(1);
                footer = new OoXMLFooter(1);
                
                footNotes = new OoXMLFootNotes();
                endNotes = new OoXMLEndNotes();

                document.AddRelation(4, footNotes);
                document.AddRelation(5, endNotes);
                document.AddRelation(6, header);
                document.AddRelation(7, footer);
                

                headerMatrix = new ExportMatrix();
                footerMatrix = new ExportMatrix();

                if (PrintOptimized)
                {
                    headerMatrix.ImageResolution = PRINT_OPTIMIZED_DPI;
                    footerMatrix.ImageResolution = PRINT_OPTIMIZED_DPI;
                }

            }
        }

        private bool IsWrongRowHeight(string rowHeight)
        {
            if (rowHeight != "exact" || rowHeight != "min")
                return true;
            else
                return false;
        }

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            if (MatrixBased == true)
            {
                matrix = new ExportMatrix();
                if (wysiwyg)
                    matrix.Inaccuracy = paragraphBased ? 5f : 0.3f;
                else
                    matrix.Inaccuracy = 10f;
                if (PrintOptimized)
                    matrix.ImageResolution = PRINT_OPTIMIZED_DPI;
                matrix.RowHeightIs = rowHeightIs;
                matrix.PlainRich = true;
                matrix.AreaFill = false;
                matrix.CropAreaFill = true;
                matrix.Report = Report;
                matrix.Images = true;
                matrix.WrapText = false;
                matrix.FullTrust = false;
                matrix.KeepRichText = paragraphBased ? true : false; // TOFO: Fix it for table based export
                matrix.AddPageBegin(page);
                if (paragraphBased && firstPage)
                {
                    headerMatrix.AddPageBegin(page);
                    footerMatrix.AddPageBegin(page);
                }
            }
            else
            {
                bool last_page = (1 + pageNo) >= Pages.Length;
                document.ExportPageBegin(page, false, last_page);
            }
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            if (MatrixBased == true)
            {
                if (paragraphBased && ((band is PageHeaderBand) || (band is PageFooterBand)))
                {
                    if (firstPage)
                    {
                        if (band is PageHeaderBand)
                        {
                            
                            headerMatrix.AddBand(band, this, true);
                        }
                        else if (band is PageFooterBand)
                        {
                            footerMatrix.AddBand(band, this, true);
                            
                        }
                    }
                }
                else
                    matrix.AddBand(band, this);
            }
            else
                document.ExportBand(band);
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            
            if (MatrixBased)
            {
                if (paragraphBased && firstPage)
                {
                    headerMatrix.AddPageEnd(page);
                    footerMatrix.AddPageEnd(page);
                }
                matrix.AddPageEnd(page);
                matrix.Prepare();
                matrixList.Add(matrix);
            }
            else
            {
                bool last_page = (1 + pageNo++) >= Pages.Length;
                document.ExportPageEnd(page, false, last_page);
            }
            firstPage = false;
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            ExportOOXML(Stream);
            Zip.SaveToStream(Stream);
            Zip.Clear();
            foreach (ExportMatrix matrix in matrixList)
                matrix.Dispose();
            if (paragraphBased)
            {
                headerMatrix.Dispose();
                footerMatrix.Dispose();
            }
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("DocxFile");
        }
        #endregion


        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            writer.WriteBool("Wysiwyg", Wysiwyg);
            writer.WriteBool("MatrixBased", MatrixBased);
            writer.WriteBool("ParagraphBased", ParagraphBased);
            writer.WriteValue("RowHeight", RowHeight);
            writer.WriteBool("PrintOptimized", PrintOptimized);
            writer.WriteBool("DoNotExpandShiftReturn", DoNotExpandShiftReturn);
        }

        /// <summary>
        /// Initializes a new instance of the Word2007Export class.
        /// </summary>
        public Word2007Export()
        {
            RowHeight = RowHeightType.Exactly;
            Wysiwyg = true;
        }
    }

    class OoHyperlink : OoXMLBase
    {
        public override string RelationType { get { return "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink"; } }

        public override string ContentType { get { throw new Exception("Content type not defined"); } }

        public override string FileName { get { return "External"; } }

        private string url;

        public string URL { get { return url; } }

        public OoHyperlink(string URL) : base()
        {
            url = URL;
        }
    }
}