using FastReport.RichTextParser;
using FastReport.Utils;
using System;
using System.Drawing;
using System.Text;

namespace FastReport.Export.OoXML
{
  /// <summary>
  /// Save 
  /// </summary>
  public class RTF_ToDocX : IDisposable
  {
    private RichDocument doc;
    private Rectangle ext_padding = new Rectangle();
    #region Private methods
    private string Quoted(long p)
    {
      return String.Concat("\"", p.ToString(), "\" ");
    }
    private string Quoted(string p)
    {
      return String.Concat("\"", p, "\" ");
    }
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

    /// <inheritdoc>
    public void Dispose()
    {
    }

    /// <inheritdoc>
    public RTF_ToDocX(RichDocument doc)
    {
      this.doc = doc;
    }

    private void SaveDocumentBody(StringBuilder s)
    {
      foreach (Page p in doc.pages)
        SavePage(s, p, false);
    }

    private void SavePage(StringBuilder s, Page page, bool v)
    {
      SaveSequence(s, page.sequence, false);
    }

    private string GetFontName(RunFormat format)
    {
      string Name = "Arial";
      int font_idx = (int)format.font_idx;
      if (font_idx < doc.font_list.Count)
      {
        RFont rf = doc.font_list[font_idx];
        Name = rf.FontName;
      }
      return Name;
    }

    private void SaveRun(StringBuilder s, Run r)
    {
      s.Append("<w:r><w:rPr>");
      string FontName = GetFontName(r.format);
      s.Append("<w:rFonts w:ascii=" + Quoted(FontName) + " w:hAnsi=" + Quoted(FontName) + " w:cs=" + Quoted(FontName) + " /> ");
      if (r.format.bold)
        s.Append("<w:b />");
      if (r.format.italic)
        s.Append("<w:i />");
      if (r.format.underline)
        s.Append("<w:u w:val=" + Quoted("single") + "/>");
      s.Append("<w:color w:val=" + GetRGBString(r.format.color) + " />");
      s.Append("<w:sz w:val=" + Quoted(r.format.font_size) + " />");
      s.Append("<w:szCs w:val=" + Quoted(r.format.font_size) + " />");
      s.Append("</w:rPr>");
      s.Append("<w:t xml:space=\"preserve\">").Append(this.TranslateText(r.text)).Append("</w:t>");
      s.Append("</w:r>");
    }

    private void SavePargraph(StringBuilder s, RichTextParser.Paragraph par, bool InTable)
    {
      s.AppendLine("<w:p>");
      string halign = "";
      switch (par.format.align)
      {
        case RichTextParser.ParagraphFormat.HorizontalAlign.Left: halign = "left"; break;
        case RichTextParser.ParagraphFormat.HorizontalAlign.Right: halign = "right"; break;
        case RichTextParser.ParagraphFormat.HorizontalAlign.Centered: halign = "center"; break;
        case RichTextParser.ParagraphFormat.HorizontalAlign.Justified:
        default:
          halign = "both";
          break;
      }

      //switch (par.format.Valign)
      //{
      //  case RichTextParser.ParagraphFormat.VerticalAlign.Top: halign = "top"; break;
      //  case RichTextParser.ParagraphFormat.VerticalAlign.Center: halign = "center"; break;
      //  case RichTextParser.ParagraphFormat.VerticalAlign.Bottom: halign = "bottom"; break;
      //  default:
      //    halign = "top";
      //    break;
      //}

      s.Append("<w:pPr><w:jc w:val=").Append(Quoted(halign)).Append(" />");
      if (par.format.first_line_indent > 0)
        s.Append("<w:ind w:firstLine=").Append(Quoted(par.format.first_line_indent)).Append("/>");

      #region Paragraph spacing
      int before_space = par.format.space_before + ext_padding.Top;
      int after_space = par.format.space_after + ext_padding.Bottom;

      s.Append("<w:spacing ");
      if (before_space != 0)
      {
        s.Append("w:before=").Append(Quoted(before_space));
        ext_padding.Y = 0;
      }
      if (after_space != 0)
      {
        s.Append(" w:after=").Append(Quoted(after_space));
      }
      s.Append(" w:line=");
      int spacing;

      if (par.format.line_spacing != 0)
        spacing = (int)(par.format.line_spacing) * 10;
      else
        spacing = 0;

      switch (par.format.lnspcmult)
      {
        //case LineSpacingType.Single:
        //  sb.Append(Quoted(238));
        //  sb.Append(" w:lineRule=\"auto\"");
        //  break;
        case RichTextParser.ParagraphFormat.LnSpcMult.Multiply:
          s.Append(Quoted(spacing));
          s.Append(" w:lineRule=\"auto\"");
          break;
        //case LineSpacingType.AtLeast:
        //  sb.Append(Quoted((int)(20 * htmlTextRenderer.ParagraphFormat.LineSpacing * 0.75f)));
        //  sb.Append(" w:lineRule=\"atLeast\"");
        //  break;
        case RichTextParser.ParagraphFormat.LnSpcMult.Exactly:
          s.Append(Quoted(spacing));
          s.Append(" w:lineRule=\"exact\"");
          break;
      }
      s.Append(" w:beforeAutospacing=\"0\" w:afterAutospacing=\"0\" />");
      #endregion

      #region Paragraph indent
      if (ext_padding.Right + ext_padding.Left != 0)
      {
        s.Append("<w:ind");
        if (ext_padding.Right != 0)
          s.Append(" w:right=" + Quoted(ext_padding.Right));
        if (ext_padding.Left != 0)
          s.Append(" w:left=" + Quoted(ext_padding.Left));
        s.Append(" />");
      }
      #endregion
      s.Append("</w:pPr>"); ;

      foreach (Run r in par.runs)
        SaveRun(s, r);
      s.AppendLine("</w:p>");
    }

    private void SaveTable(StringBuilder s, RichTextParser.Table tbl, bool InTable)
    {
      s.Append("<w:tbl>");

      // Table properties
      s.Append("<w:tblPr>");
      s.Append("<w:tblW w:w=\"0\" w:type=\"auto\" />");
      s.Append("<w:tblCellMar><w:left w:w=\"0\" w:type=\"dxa\"/><w:right w:w=\"0\" w:type=\"dxa\"/></w:tblCellMar>");
      s.Append("<w:tblBorders>");
      s.Append("<w:top w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
      s.Append("<w:left w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
      s.Append("<w:bottom w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
      s.Append("<w:right w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
      s.Append("<w:insideH w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
      s.Append("<w:insideV w:val=\"none\" w:sz=\"0\" w:space=\"0\" w:color=\"auto\" />");
      s.Append("</w:tblBorders>");
      s.Append("</w:tblPr>");

      // Table grid
      s.Append("<w:tblGrid>");
      foreach (Column column in tbl.columns)
      {
        s.Append("<w:gridCol w:w=" + Quoted(column.Width) + " />");
      }
      s.Append("</w:tblGrid>");

      foreach (TableRow row in tbl.rows)
        SaveRow(s, row);
      s.Append("</w:tbl>");
    }

    private void SaveRow(StringBuilder s, TableRow row)
    {
      s.Append("<w:tr><w:trPr>");
      s.Append("<w:trHeight w:hRule=\"").Append("exact").
          Append("\" w:val=").Append(Quoted(row.height)).Append("/></w:trPr>");

      foreach (RichObjectSequence seq in row.cells)
      {
        s.Append("<w:tc><w:tcPr>");
        s.Append("<w:tcW w:w=").Append(Quoted(9237)).Append(" w:type=\"dxa\"/>"); // Fix cell width
        s.AppendLine("</w:tcPr><w:p>");
        SaveSequence(s, seq, true);
        s.AppendLine("</w:p></w:tc>");
      }
      s.AppendLine("</w:tr>");
    }

    private void SaveSequence(StringBuilder s, RichObjectSequence seq, bool InTable)
    {
      foreach (RichTextParser.RichObject robj in seq.objects)
      {
        switch (robj.type)
        {
          case RichTextParser.RichObject.Type.Paragraph:
            SavePargraph(s, robj.pargraph, InTable);
            break;
          case RichTextParser.RichObject.Type.Picture:
            // SkipPicture
            break;
          case RichTextParser.RichObject.Type.Table:
            SaveTable(s, robj.table, InTable);
            break;
        }
      }
    }

    /// <summary>
    /// Padding over ridh object
    /// </summary>
    public Rectangle Padding
    {
      get { return ext_padding; }
      set { ext_padding = value; }
    }

    /// <summary>
    /// This prperty keep RTF to DOC translation result, i.e. DOCX XML fragment
    /// </summary>
    public string DocX
    {
      get
      {
        StringBuilder s = new StringBuilder();
        SaveDocumentBody(s);
        return s.ToString();
      }
    }
  }


}

