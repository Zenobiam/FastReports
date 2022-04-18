using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using FastReport.Utils;
using FastReport.Export;
using FastReport.Preview;
using FastReport.Export.TTF;

namespace FastReport.Export.Pdf
{
    internal class PDFExportAnnotation
    {
        public long reference;
        public string rect;
        public string hyperlink;
        public int destPage;
        public int destY;
    }

    public partial class PDFExport : ExportBase
    {
        private List<PDFExportAnnotation> annots;
        private StringBuilder pageAnnots;

        private string GetPageAnnots()
        {
            return "/Annots [ " + pageAnnots.ToString() + "]";
        }

        private void WriteAnnots()
        {
            foreach (PDFExportAnnotation annot in annots)
            {
                xRef[(int)annot.reference - 1] = pdf.Position;

                WriteLn(pdf, ObjNumber(annot.reference));
                WriteLn(pdf, "<<");
                WriteLn(pdf, "/Type /Annot");
                WriteLn(pdf, "/Subtype /Link");
                if (isPdfA())
                    WriteLn(pdf, "/F 4");
                WriteLn(pdf, "/Rect [" + annot.rect + "]");


                if (!isPdfX() && !String.IsNullOrEmpty(annot.hyperlink))
                {
                    WriteLn(pdf, "/BS << /W 0 >>");
                    WriteLn(pdf, "/A <<");
                    WriteLn(pdf, "/URI (" + annot.hyperlink + ")");
                    WriteLn(pdf, "/Type /Action");
                    WriteLn(pdf, "/S /URI");
                    WriteLn(pdf, ">>");
                }
                else
                {
                    WriteLn(pdf, "/Border [16 16 0]");
                    if (annot.destPage < pagesRef.Count)
                    {
                        WriteLn(pdf, "/Dest [" + pagesRef[annot.destPage].ToString() +
                          " 0 R /XYZ null " + ((int)(pagesHeights[annot.destPage] - annot.destY)).ToString() + " null]");
                    }
                }

                WriteLn(pdf, ">>");
                WriteLn(pdf, "endobj");
            }
        }

        private void AddAnnot(ReportComponentBase obj)
        {
            string Left = FloatToString(GetLeft(obj.AbsLeft));
            string Top = FloatToString(GetTop(obj.AbsTop));
            string Right = FloatToString(GetLeft(obj.AbsLeft + obj.Width));
            string Bottom = FloatToString(GetTop(obj.AbsTop + obj.Height));
            AddAnnot(obj, Left + " " + Bottom + " " + Right + " " + Top);
        }

        private void AddAnnot(ReportComponentBase obj, string rect)
        {
            if ((obj.Hyperlink.Kind == HyperlinkKind.Bookmark ||
              obj.Hyperlink.Kind == HyperlinkKind.PageNumber ||
              obj.Hyperlink.Kind == HyperlinkKind.URL) && !String.IsNullOrEmpty(obj.Hyperlink.Value))
            {
                long reference = UpdateXRef();
                pageAnnots.Append(ObjNumberRef(reference)).Append(" ");
                PDFExportAnnotation annot = new PDFExportAnnotation();
                annot.reference = reference;
                annot.rect = rect;
                annots.Add(annot);

                switch (obj.Hyperlink.Kind)
                {
                    case HyperlinkKind.URL:

                        try
                        {
                            Uri uri = new Uri(obj.Hyperlink.Value);
                            annot.hyperlink = uri.AbsoluteUri;
                        }
                        catch (Exception)
                        {  }
                        //annot.hyperlink = obj.Hyperlink.Value.Replace("\\", "\\\\");
                        break;

                    case HyperlinkKind.Bookmark:
                        Bookmarks.BookmarkItem bookmark = Report.PreparedPages.Bookmarks.Find(obj.Hyperlink.Value);
                        if (bookmark != null)
                        {
                            annot.destPage = bookmark.pageNo;
                            annot.destY = (int)(bookmark.offsetY * PDF_DIVIDER);
                        }
                        break;

                    case HyperlinkKind.PageNumber:
                        annot.destPage = int.Parse(obj.Hyperlink.Value) - 1;
                        annot.destY = 0;
                        break;
                }
            }
        }

        private void AddTextField(TextObject obj, long defaultValueXref)
        {
            StringBuilder sb = new StringBuilder();
            string Left = FloatToString(GetLeft(obj.AbsLeft + obj.Padding.Left));
            string Top = FloatToString(GetTop(obj.AbsTop + obj.Padding.Top));
            string Right = FloatToString(GetLeft(obj.AbsLeft + obj.Width - obj.Padding.Right));
            string Bottom = FloatToString(GetTop(obj.AbsTop + obj.Height - obj.Padding.Bottom));


            int ObjectFontNumber = GetObjFontNumberAcroForm(obj.Font);
            ExportTTFFont pdffont = fonts[ObjectFontNumber];
            pdffont.Editable = true;
            AppendFontAcroForm(sb, ObjectFontNumber, obj.Font.Size, obj.TextColor);
            if (!acroFormsFonts.Contains(ObjectFontNumber))
                acroFormsFonts.Add(ObjectFontNumber);

            long xref = UpdateXRef();
            //FAcroFormsAnnotsRefs.Add(xref);
            acroFormsRefs.Add(xref);
            pageAnnots.Append(ObjNumberRef(xref)).Append(" ");
            WriteLn(pdf, ObjNumber(xref));
            WriteLn(pdf, "<<");
            WriteLn(pdf, "/Type /Annot  /Subtype /Widget /F 4");
            Write(pdf, "/FT /Tx " + (obj.VertAlign != VertAlign.Center ? "/Ff 4096 " : "") + "/H /N ");// /MK << >> ");
            if (defaultValueXref >= 0)
            {
                Write(pdf, "/AP << /N " + ObjNumberRef(defaultValueXref) + " >>");
            }
            StringBuilder text = new StringBuilder(obj.Text.Length);
            StrToUTF16(obj.Text, text);
            string align = "0";
            if (obj.HorzAlign == HorzAlign.Center) align = "1";
            else if (obj.HorzAlign == HorzAlign.Right) align = "2";
            Write(pdf, " /DA ( " + sb.ToString() + " ) /Q " + align + " /Rect [ " + Left + " " + Bottom + " " + Right + " " + Top + " ] /T (" + obj.Name + acroFormsRefs.Count + ") /V <" + text.ToString() + "> ");

            WriteLn(pdf, ">>");
            WriteLn(pdf, "endobj");
            //<< /AP << /N 10 0 R >> /DA (/Helv 8.64 Tf 0 g) /F 4 /FT /Tx /MK << >> /P 8 0 R /Q 0 /Rect [ 123.12 691.2 284.4 726.48 ] /Subtype /Widget /T (untitled1) >>

        }

        private void AddCheckBoxField(CheckBoxObject obj)
        {

            bool chk = obj.Checked;

            string Left = FloatToString(GetLeft(obj.AbsLeft));
            string Top = FloatToString(GetTop(obj.AbsTop));
            string Right = FloatToString(GetLeft(obj.AbsLeft + obj.Width));
            string Bottom = FloatToString(GetTop(obj.AbsTop + obj.Height));

            obj.Checked = true;
            long yesXRef1 = AddCheckBox(obj);

            obj.Checked = false;
            long offXRef1 = AddCheckBox(obj);

            obj.Checked = chk;

            //ExportTTFFont font;
            ////TODO font kludge
            //if (FFonts.Count == 0)
            //{
            //  font = GetGlobalFont(new Font(FontFamily.GenericSansSerif, 12));
            //  font.Reference = UpdateXRef();
            //  font.Saved = true;
            //}
            //else
            //  font = FFonts[0];

            long xref = UpdateXRef();
            //FAcroFormsAnnotsRefs.Add(xref);
            acroFormsRefs.Add(xref);
            pageAnnots.Append(ObjNumberRef(xref)).Append(" ");
            WriteLn(pdf, ObjNumber(xref));
            WriteLn(pdf, "<<");
            WriteLn(pdf, "/Type /Annot  /Subtype /Widget /F 4");
            WriteLn(pdf, "/AP << /N << /Off " + ObjNumberRef(offXRef1) + " /Yes " + ObjNumberRef(yesXRef1) + " >>" /*+" /D << /Off " + ObjNumberRef(offXRef1) + " /Yes " + ObjNumberRef(yesXRef2) + " >>"*/ + " >> ");
            Write(pdf, "/FT /Btn /H /N ");// /DA (" + font.Name + " " + FloatToString(12) + " Tf 0 g)");
            Write(pdf, " " + (chk ? "/AS /Yes" : "/AS /Off") + " /Rect [ " + Left + " " + Bottom + " " + Right + " " + Top + " ] /T (" + obj.Name + acroFormsRefs.Count + ") " + (chk ? "/V /Yes" : "/V /Off"));

            WriteLn(pdf, ">>");
            WriteLn(pdf, "endobj");
            //  /F 4 /FT /Btn /H /P /MK << /CA (4) >> >>
        }

        const string mark = "\u2714";


        private long AddCheckBox(CheckBoxObject obj)
        {

            StringBuilder sb_in = new StringBuilder();

            long imageIndex = AddPictureObject(obj as ReportComponentBase, true, jpegQuality, sb_in, true);


            string Width = FloatToString(obj.Width * PDF_DIVIDER);
            string Height = FloatToString(obj.Height * PDF_DIVIDER);


            long xref = UpdateXRef();
            WriteLn(pdf, ObjNumber(xref));

            WriteLn(pdf, "<< /BBox [ 0 0 " + Width + " " + Height + " ] /Resources << /XObject << " + (imageIndex < 0 ? "" : "/Im" + imageIndex + " " + ObjNumberRef(imageIndex)) + " >> /ProcSet [ /PDF /Text /ImageC /ImageI /ImageB ] >> /Subtype /Form /Type /XObject ");
            using (MemoryStream ms = new MemoryStream(ExportUtils.StringToByteArray(sb_in.ToString())))
                WritePDFStream(pdf, ms, 0, false, false, false, true);
            //<< /BBox [ 0 0 163.44002 101.51996 ] /Resources << /Font << /Verdana,Bold 6 0 R >> /ProcSet [ /PDF /Text ] >> /Subtype /Form /Type /XObject /Length 98 >>

            return xref;
        }



        private long AddTextDefaultValueForEditable(TextObject obj)
        {

            if (!String.IsNullOrEmpty(obj.Text))
            {
                //TODO for forms
                if (obj.VertAlign != VertAlign.Center)
                    obj.VertAlign = VertAlign.Top;
                obj.Angle = 0;
                string Width = FloatToString(obj.Width * PDF_DIVIDER);
                string Height = FloatToString(obj.Height * PDF_DIVIDER);
                StringBuilder sb = new StringBuilder();
                int ObjectFontNumber = GetObjFontNumber(obj.Font);

                using (Font f = new Font(obj.Font.Name, obj.Font.Size * dpiFX, obj.Font.Style))
                {

                    RectangleF textRect = new RectangleF(
                      0,
                      0,
                      obj.Width - obj.Padding.Horizontal,
                      obj.Height - obj.Padding.Vertical);


                    if (!pageFonts[ObjectFontNumber].Saved)
                    {
                        pageFonts[ObjectFontNumber].Reference = UpdateXRef();
                        pageFonts[ObjectFontNumber].Saved = true;
                    }

                    sb.AppendLine("/Tx BMC");

                    if (!obj.HasHtmlTags)
                        AppendFont(sb, ObjectFontNumber, obj.Font.Size, obj.TextColor);

                    AddTextObjectInternal(obj, textRect, false, ObjectFontNumber, sb, f, true);

                    sb.AppendLine("EMC");

                    long xref = UpdateXRef();
                    WriteLn(pdf, ObjNumber(xref));
                    int fontnumber = GetObjFontNumberAcroForm(obj.Font);
                    ExportTTFFont font = fonts[fontnumber];
                    //todo make BBOX without fill page
                    WriteLn(pdf, "<< /BBox [ 0 0 " + Width + " " + Height + " ] /Resources << /Font << " + font.Name + " " + ObjNumberRef(font.Reference) + " >> /ProcSet [ /PDF /Text ] >> /Subtype /Form /Type /XObject ");
                    using (MemoryStream ms = new MemoryStream(ExportUtils.StringToByteArray(sb.ToString())))
                        WritePDFStream(pdf, ms, 0, false, false, false, true);
                    //<< /BBox [ 0 0 163.44002 101.51996 ] /Resources << /Font << /Verdana,Bold 6 0 R >> /ProcSet [ /PDF /Text ] >> /Subtype /Form /Type /XObject /Length 98 >>

                    return xref;
                }
            }
            return -1;
        }

        private long AddAcroForm()
        {
            if (acroFormsRefs.Count > 0)
            {
                long acroForm = UpdateXRef();
                WriteLn(pdf, ObjNumber(acroForm));
                Write(pdf, "<<  /SigFlags 3 /DR << /Font <<");
                foreach (int fontnumber in acroFormsFonts)
                {
                    ExportTTFFont font = fonts[fontnumber];
                    Write(pdf, font.Name + " " + ObjNumberRef(font.Reference) + " ");
                }
                Write(pdf, ">> >> /Fields [ ");
                foreach (long val in acroFormsRefs)
                {
                    Write(pdf, ObjNumberRef(val));
                    Write(pdf, " ");
                }
                WriteLn(pdf, "] >>");
                WriteLn(pdf, "endobj");
                //<< /DR << /Font << /Verdana,Bold 6 0 R >> >>  << /Font << /Helv 6 0 R >> >> /Fields [ 7 0 R ] >>
                return acroForm;
            }
            return -1;
        }
    }
}