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
  partial class OoXMLDocument : OoXMLBase
  {
    private void DefineShapes(MemoryStream file)
    {
      // Line shape
      ExportUtils.WriteLn(file, "<v:shapetype id=\"_x0000_t32\" coordsize=\"21600,21600\" o:spt=\"32\" o:oned=\"t\" path=\"m,l21600,21600e\" filled=\"f\">");
      ExportUtils.WriteLn(file, "<v:path arrowok=\"t\" fillok=\"f\" o:connecttype=\"none\"/>");
      ExportUtils.WriteLn(file, "</v:shapetype>");

      // Shape diamond
      ExportUtils.WriteLn(file, "<v:shapetype id=\"_x0000_t4\" coordsize=\"21600,21600\" o:spt=\"4\" path=\"m10800,l,10800,10800,21600,21600,10800xe\">");
      ExportUtils.WriteLn(file, "<v:stroke joinstyle=\"miter\" />");
      ExportUtils.WriteLn(file, "<v:path gradientshapeok=\"t\" o:connecttype=\"rect\" textboxrect=\"5400,5400,16200,16200\" />");
      ExportUtils.WriteLn(file, "</v:shapetype>");

      // Shape triangle
      ExportUtils.WriteLn(file, "<v:shapetype id=\"_x0000_t5\" coordsize=\"21600,21600\" o:spt=\"5\" adj=\"10800\" path=\"m@0,l,21600r21600,xe\">");
      ExportUtils.WriteLn(file, "<v:stroke joinstyle=\"miter\" /> ");
      ExportUtils.WriteLn(file, "<v:formulas>");
      ExportUtils.WriteLn(file, "<v:f eqn=\"val #0\" />");
      ExportUtils.WriteLn(file, "<v:f eqn=\"prod #0 1 2\" />");
      ExportUtils.WriteLn(file, "<v:f eqn=\"sum @1 10800 0\" />");
      ExportUtils.WriteLn(file, "</v:formulas>");
      ExportUtils.WriteLn(file, "<v:path gradientshapeok=\"t\" o:connecttype=\"custom\" o:connectlocs=\"@0,0;@1,10800;0,21600;10800,21600;21600,21600;@2,10800\" textboxrect=\"0,10800,10800,18000;5400,10800,16200,18000;10800,10800,21600,18000;0,7200,7200,21600;7200,7200,14400,21600;14400,7200,21600,21600\" />");
      ExportUtils.WriteLn(file, "<v:handles>");
      ExportUtils.WriteLn(file, "<v:h position=\"#0,topLeft\" xrange=\"0,21600\" />");
      ExportUtils.WriteLn(file, "</v:handles>");
      ExportUtils.WriteLn(file, "</v:shapetype>");
    }

    private void WritePageBorders(MemoryStream file)
    {
      ExportUtils.WriteLn(file, "<w:pgBorders w:offsetFrom=\"page\">");
      ExportUtils.WriteLn(file, "<w:top w:val=\"single\" w:sz=\"4\" w:space=\"24\" w:color=\"auto\"/>");
      ExportUtils.WriteLn(file, "<w:left w:val=\"single\" w:sz=\"4\" w:space=\"24\" w:color=\"auto\"/>");
      ExportUtils.WriteLn(file, "<w:bottom w:val=\"single\" w:sz=\"4\" w:space=\"24\" w:color=\"auto\"/>");
      ExportUtils.WriteLn(file, "<w:right w:val=\"single\" w:sz=\"4\" w:space=\"24\" w:color=\"auto\"/>");
      ExportUtils.WriteLn(file, "</w:pgBorders>");
    }

    private void WtiteSectionProperties(Word2007Export OoXML, MemoryStream file)
    {
      ExportUtils.WriteLn(file, "<w:sectPr>");
      ExportUtils.WriteLn(file, "<w:type w:val=\"continuous\"/>");

      ExportUtils.Write(file, GetPageSize(current_page_state));

      Border border = OoXML.Border;
      if (border.Lines != BorderLines.None)
        WritePageBorders(file);

      ExportUtils.WriteLn(file, "<w:cols w:space=" + Quoted(304) + "/>");
      ExportUtils.WriteLn(file, "<w:docGrid w:linePitch=" + Quoted(360) + "/>");
      ExportUtils.WriteLn(file, "</w:sectPr>");
    }

    // Translatr RTF to DOCX
    private void AddRichObject(RichObject rich_obj)
    {
      textStrings.Append("<w:r><w:rPr><w:noProof /></w:rPr><w:pict>").
          Append("<v:rect id=").Append(Quoted(rich_obj.Name)).
          Append(" style=\"position:absolute;margin-left:").Append(rich_obj.AbsLeft).Append(";").
          Append("margin-top:").Append(rich_obj.AbsTop).Append(";").
          Append("width:").Append(rich_obj.Width).Append(";").
          Append("height:").Append(rich_obj.Height).Append("\" stroked=\"f\">").
          Append("<v:textbox inset=\"0,0,0,0\">").
          Append("<w:txbxContent>");

      using (RTF_DocumentParser parser = new RTF_DocumentParser())
      {
        parser.Load(rich_obj.Text);
        using (RTF_ToDocX saver = new RTF_ToDocX(parser.Document))
          textStrings.Append(saver.DocX);
      }

      textStrings.AppendLine("</w:txbxContent></v:textbox>").
        AppendLine("</v:rect>").
        Append("</w:pict></w:r>");
    }

  private void AddTextObject(ReportComponentBase reportObject, string Fill_String)
  {
      string vertical_anchor = "";

      if (reportObject is TextObject) switch ((reportObject as TextObject).VertAlign)
        {
          case VertAlign.Top: vertical_anchor = "top"; break;
          case VertAlign.Center: vertical_anchor = "middle"; break;
          case VertAlign.Bottom: vertical_anchor = "bottom"; break;
        }

      string fill_color = "";
      string filled = "";

      if (reportObject.Fill is SolidFill)
      {
        SolidFill fill = reportObject.Fill as SolidFill;
        if (!fill.IsTransparent)
          fill_color = "fillcolor=" + GetRGBString(fill.Color) + " ";
        else
            if (Fill_String == null) filled = "filled=\"f\" ";
      }
      else
      {
        if (Fill_String != null && !(reportObject.Fill is TextureFill))
          throw new Exception("Fill conflict.");

        int pictureID = Word2007Export.RID_SHIFT + (++pictureCount);
        string file_extension = "png";
        string ImageFileName = "word/media/FillMap" + pictureCount.ToString() + "." + file_extension;
        OoPictureObject pic = new OoPictureObject(ImageFileName);
        pic.Export(reportObject, this.wordExport, true);
        AddRelation(pictureID, pic);
        Fill_String = "<v:fill r:id=" + Quoted("rId" + pictureID.ToString()) +
            " o:title=" + Quoted(pictureCount) +
            " recolor=\"f\" type=\"frame\" />";
      }

      textStrings.Append("<w:r><w:rPr><w:noProof /></w:rPr><w:pict><v:rect id=").Append(Quoted(reportObject.Name));
      if (reportObject.Hyperlink.Value != null && reportObject.Hyperlink.Value.Length > 0 && reportObject.Hyperlink.Kind == HyperlinkKind.URL)
      {
        textStrings.Append(string.Format(" href={0}", Quoted(Utils.Converter.ToXml(reportObject.Hyperlink.Value))));
      }
      textStrings.Append(" style=\"position:absolute;margin-left:").Append(reportObject.AbsLeft).Append(";").
          Append("margin-top:").Append(reportObject.AbsTop).Append(";").
          Append("width:").Append(reportObject.Width).Append(";").
          Append("height:").Append(reportObject.Height).Append(";").
          Append("v-text-anchor:").Append(vertical_anchor).Append("\" ").
          Append(filled).Append(fill_color);

      if (reportObject.Border.Lines == BorderLines.All &&
              reportObject.Border.LeftLine.Equals(reportObject.Border.RightLine) &&
              reportObject.Border.LeftLine.Equals(reportObject.Border.TopLine) &&
              reportObject.Border.LeftLine.Equals(reportObject.Border.BottomLine))
      {
        textStrings.Append(" stroked=\"t\" strokecolor=").
            Append(GetRGBString(reportObject.Border.Color)).
            Append(" strokeweight=\"").Append(reportObject.Border.Width.ToString()).Append("pt\"");
        reportObject.Border.Lines = BorderLines.None;
      }
      else
        textStrings.Append(" stroked=\"f\"");

      textStrings.Append(">");

      if (Fill_String != null)
        textStrings.AppendLine(Fill_String);

      if (reportObject.Border.Shadow)
        textStrings.AppendLine("<v:shadow on=\"t\" opacity=\".5\" offset=\"6pt,6pt\" />");

      if (reportObject is TextObject)
      {
        textStrings.AppendLine("<v:textbox inset=\"0,0,0,0\">");
        textStrings.AppendLine("<w:txbxContent>");

        TextObject textObject = reportObject as TextObject;

        IGraphics g = wordExport.Report.MeasureGraphics;
        using (Font f = new Font(textObject.Font.Name, textObject.Font.Size * DrawUtils.ScreenDpiFX, textObject.Font.Style))
        using (GraphicCache cache = new GraphicCache())
        {
          RectangleF textRect = new RectangleF(
            textObject.AbsLeft + textObject.Padding.Left,
            textObject.AbsTop + textObject.Padding.Top,
            textObject.Width - textObject.Padding.Horizontal,
            textObject.Height - textObject.Padding.Vertical);

          StringFormat format = textObject.GetStringFormat(cache, 0);
          switch (textObject.TextRenderType)
          {
            case TextRenderType.HtmlParagraph:
              textStrings.Append(Export_HtmlParagraph(textObject.GetHtmlTextRenderer(g, 1, 1, textRect, format), true).ToString());
              break;
            default:
              Brush textBrush = cache.GetBrush(textObject.TextColor);
              AdvancedTextRenderer renderer = new AdvancedTextRenderer(textObject.Text, g, f, textBrush, null,
                  textRect, format, textObject.HorzAlign, textObject.VertAlign,
                  textObject.LineHeight, textObject.Angle, textObject.FontWidthRatio,
                                textObject.ForceJustify, textObject.Wysiwyg, textObject.HasHtmlTags, true, DrawUtils.ScreenDpiFX, DrawUtils.ScreenDpiFX, textObject.InlineImageCache);

              //float w = f.Height * 0.1f; // to match .net char X offset

              float FirstTabOffset = textObject.FirstTabOffset;
              if (FirstTabOffset == 0) FirstTabOffset = 720;
              else FirstTabOffset *= 16;

              for (int j = 0; j < renderer.Paragraphs.Count; ++j)
              {
                AdvancedTextRenderer.Paragraph paragraph = renderer.Paragraphs[j];
                Open_Paragraph(textObject, j == 0, j == renderer.Paragraphs.Count - 1);
                if (renderer.HtmlTags)
                {
                  if (paragraph.Lines[0].Text != "" && paragraph.Lines[0].Text[0] == '\t')
                    this.InsertTabulation(FirstTabOffset);
                  foreach (AdvancedTextRenderer.Line line in paragraph.Lines)
                    foreach (AdvancedTextRenderer.Word word in line.Words)
                      foreach (AdvancedTextRenderer.Run run in word.Runs)
                        using (Font fnt = run.GetFont())
                          Add_Run(fnt, run.Style.Color, run.Text, /*textObject.Hyperlink.Value*/null, true,
                              (reportObject as TextObject).RightToLeft,
                              run.Style.BaseLine,
                              (reportObject as TextObject).LineHeight,
                              false, false);
                }
                else
                {
                  if (paragraph.Lines[0].Text != "" && paragraph.Lines[0].Text[0] == '\t')
                    this.InsertTabulation(FirstTabOffset);
                  for (int i = 0; i < paragraph.Lines.Count; i++)
                  {
                    AdvancedTextRenderer.Line line = paragraph.Lines[i];
                    Add_Run(f, textObject.TextColor, line.Text, /*textObject.Hyperlink.Value*/null,
                        false, (reportObject as TextObject).RightToLeft,
                        AdvancedTextRenderer.BaseLine.Normal,
                        (reportObject as TextObject).LineHeight,
                        i < (paragraph.Lines.Count - 1) && paragraph.Lines.Count > 1, false);
                  }
                }
                Close_Paragraph();
              }
              break;
          }

        }
        textStrings.AppendLine("</w:txbxContent></v:textbox>");
      }
      textStrings.AppendLine("</v:rect></w:pict></w:r>");
    }

    private OoPictureObject AddPictureObject(ReportComponentBase pictureObject, string imageName)
    {
      int pictureID = Word2007Export.RID_SHIFT + (++pictureCount);
      string file_extension = "png";
      string ImageFileName = imageName + pictureCount.ToString() + "." + file_extension;
      OoPictureObject pic = new OoPictureObject(ImageFileName);
      pic.Export(pictureObject, this.wordExport, false);
      AddRelation(pictureID, pic);
      string fill = "<v:fill r:id=" + Quoted("rId" + pictureID.ToString()) +
          " o:title=" + Quoted(pictureCount) +
          " recolor=\"f\" type=\"frame\" />";
      AddTextObject(pictureObject, fill);
      return pic;
    }

    private void AddShape(ShapeObject shape)
    {
      textStrings.Append("<w:r><w:pict>");

      switch (shape.Shape)
      {
        case ShapeKind.Rectangle:
          textStrings.AppendLine("<v:rect style=\"position:absolute;" +
              "margin-left:" + shape.AbsLeft + ";" +
              "margin-top:" + shape.AbsTop + ";" +
              "width:" + shape.Width + ";" +
              "height:" + shape.Height + "\"");
          break;
        case ShapeKind.Diamond:
          textStrings.AppendLine("<v:shape type=\"#_x0000_t4\" style=\"position:absolute;" +
              "margin-left:" + shape.AbsLeft + ";" +
              "margin-top:" + shape.AbsTop + ";" +
              "width:" + shape.Width + ";" +
              "height:" + shape.Height + "\"");
          break;
        case ShapeKind.Ellipse:
          textStrings.AppendLine("<v:oval style=\"position:absolute;" +
              "margin-left:" + shape.AbsLeft + ";" +
              "margin-top:" + shape.AbsTop + ";" +
              "width:" + shape.Width + ";" +
              "height:" + shape.Height + "\"");
          break;
        case ShapeKind.RoundRectangle:
          textStrings.AppendLine("<v:roundrect style=\"position:absolute;" +
              "margin-left:" + shape.AbsLeft + ";" +
              "margin-top:" + shape.AbsTop + ";" +
              "width:" + shape.Width + ";" +
              "height:" + shape.Height + "\"" +
              " arcsize=\"10923f\"");
          break;
        case ShapeKind.Triangle:
          textStrings.AppendLine("<v:shape type=\"#_x0000_t5\" style=\"position:absolute;" +
              "margin-left:" + shape.AbsLeft + ";" +
              "margin-top:" + shape.AbsTop + ";" +
              "width:" + shape.Width + ";" +
              "height:" + shape.Height + "\"");
          break;
        default: throw new Exception("Unsupported shape kind");
      }

      textStrings.AppendLine(" strokeweight=\"" + shape.Border.Width + "pt\" />");
      textStrings.AppendLine("</w:pict>");
      textStrings.AppendLine("</w:r>");
    }

    private void AddLine(LineObject line)
    {
      textStrings.AppendLine("<w:r>");
      textStrings.AppendLine("<w:pict>");

      textStrings.AppendLine("<v:shape type=\"#_x0000_t32\"" +
          " style=\"position:absolute;" +
          "margin-left:" + line.AbsLeft + ";" +
          "margin-top:" + line.AbsTop + ";" +
          "width:" + line.Width + ";" +
          "height:" + line.Height + "\"" +
          " o:connectortype=\"straight\"" +
          " strokecolor=" + GetRGBString(line.Border.Color) +
          " strokeweight=\"" + line.Border.Width + "pt\"" +
          ">");

      string StartCap = null;
      string EndCap = null;

      switch (line.StartCap.Style)
      {
        case CapStyle.Arrow: StartCap = "classic"; break;
        case CapStyle.Circle: StartCap = "oval"; break;
        case CapStyle.Diamond: StartCap = "diamond"; break;
        case CapStyle.Square: StartCap = "diamond"; break;
      }

      switch (line.EndCap.Style)
      {
        case CapStyle.Arrow: EndCap = "classic"; break;
        case CapStyle.Circle: EndCap = "oval"; break;
        case CapStyle.Diamond: EndCap = "diamond"; break;
        case CapStyle.Square: EndCap = "diamond"; break;
      }
      if (EndCap != null)
        textStrings.AppendLine("<v:stroke endarrow=" + Quoted(EndCap) + "/>");
      if (StartCap != null)
        textStrings.AppendLine("<v:stroke startarrow=" + Quoted(StartCap) + "/>");

      textStrings.AppendLine("</v:shape></w:pict></w:r>");
    }

    internal void ExportBand(Base band)
    {
      if ((band as BandBase).Fill is TextureFill)
          AddPictureObject(band as BandBase, "word/media/image");
      else
          AddBandObject(band as BandBase);
      foreach (Base c in band.ForEachAllConvectedObjects(wordExport))
      {
        ReportComponentBase obj = c as ReportComponentBase;
        if (obj == null)
            continue;
        else if (obj.Fill is TextureFill)
            {
                AddPictureObject(obj, "word/media/image");
                continue;
            }
        if (obj is CellularTextObject)
          obj = (obj as CellularTextObject).GetTable();
        if (obj is TableCell)
          continue;

        else if (obj is TextObject)
          AddTextObject(obj as TextObject, null);
        else if (obj is PictureObject)
          AddPictureObject(obj, "word/media/image");
        else if (obj is ZipCodeObject)
          AddPictureObject(obj, "word/media/ZipCodeImage");
        else if (obj is Barcode.BarcodeObject)
          AddPictureObject(obj, "word/media/BarcodeImage");
#if MSCHART
        else if (obj is MSChart.MSChartObject)
          AddPictureObject(obj, "word/media/MSChartImage");
#endif
        else if (obj is RichObject)
          AddRichObject(obj as RichObject);
        else if (obj is TableBase)
          AddTable(obj as TableBase);
        else if (obj is BandBase)
          AddBandObject(obj as BandBase);
        else if (obj is LineObject)
          AddLine(obj as LineObject);
        else if (obj is ShapeObject)
          AddShape(obj as ShapeObject);
        else if (obj is CheckBoxObject)
          //AddCheckboxObject(obj as CheckBoxObject);
          AddPictureObject(obj, "word/media/image");
        else if (obj is Gauge.GaugeObject)
          AddPictureObject(obj, "word/media/GaugeImage");
        else
        {
          AddPictureObject(obj, "word/media/image");
        }
      }
    }

    internal void ExportLayers(Word2007Export OoXML)
    {
      MemoryStream file = new MemoryStream();

      ExportUtils.WriteLn(file, xml_header);
      ExportUtils.WriteLn(file, "<w:document xmlns:ve=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"" +
                  " xmlns:o=\"urn:schemas-microsoft-com:office:office\"" +
                  " xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"" +
                  " xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\"" +
                  " xmlns:v=\"urn:schemas-microsoft-com:vml\"" +
                  " xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\"" +
                  " xmlns:w10=\"urn:schemas-microsoft-com:office:word\"" +
                  " xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\"" +
                  " xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\">");

      ExportUtils.WriteLn(file, "<w:body>");
      DefineShapes(file);
      ExportUtils.WriteLn(file, "<w:p>");
      ExportUtils.WriteLn(file, textStrings.ToString());
      ExportUtils.WriteLn(file, "</w:p>");

      WtiteSectionProperties(OoXML, file);

      ExportUtils.WriteLn(file, "</w:body>");
      ExportUtils.WriteLn(file, "</w:document>");

      file.Position = 0;
      OoXML.Zip.AddStream(ExportUtils.TruncLeadSlash(FileName), file);

      ExportRelations(OoXML);
    }
  }
}
