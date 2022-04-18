using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace FastReport.RichTextParser
{
  class RTF_TextUtils
  {
#if false

    public override int PageRTF_Find(string FindWhat)
    {
#if false
      int line, column;
      RTF_View.CommonViewObject paragraph;
      int absolute_position = rtf_text.FindString(FindWhat, out paragraph, out line, out column);
      return line;
#else
      throw new NotImplementedException("Find string disabled at this time");
#endif
    }

    public override int PageRTF_ReplaceAll(string ReplaceWhat, string ReplaceWith)
    {
      int replace_count = rtf_text.ReplaceAll(ReplaceWhat, ReplaceWith);
      if (replace_count != 0)
      {
        int height = rtf_text.InvalidateView(ClientRectangle);
//        RefreshScrollBar(height);
        Refresh();
      }
      return replace_count;
    }

    internal int PageRTF_FindString(string findWhat, out CommonViewObject paragraph, out int line, out int position)
    {
      int abs_position = -1;
      int prev_size = 0;
      paragraph = null;
      line = 0;
      position = 0;
      foreach (CommonViewObject par in view_objects)
      {
        abs_position = par.FindString(findWhat, out line, out position);
        if (abs_position >= 0)
        {
          paragraph = par;
          break;
        }
        prev_size += par.Length;
      }
      return prev_size + abs_position;
    }

    internal int ReplaceAll(string replaceWhat, string replaceWith)
    {
      throw new NotImplementedException();
    }

    internal override void Row_ReplaceText(int start, int len, string value)
    {
      int cells_length = 0;
      int prev_length = 0;
      foreach (RTF_CommonRichElement item in cells)
      {
        prev_length = cells_length;
        cells_length += item.Lenght;
        if (cells_length < start)
          continue;
        item.ReplaceText(start - prev_length, len, value);
        break;
      }
    }
    internal int Run_ReplaceAll(string findWhat, string replaceWith)
    {
      if (run.text.Contains(findWhat))
      {
        run.text = run.text.Replace(findWhat, replaceWith);
        return 1;
      }
      return 0;
    }

    internal override void Paragraph_ReplaceText(int start, int len, string value)
    {
      foreach (RTF_Run run in runs)
      {
        if (run.text.Length <= start)
        {
          start -= run.text.Length == 0 ? 1 : run.text.Length;
          continue;
        }
        Lenght -= run.text.Length;
        string s = run.text.Remove(start, len);
        run.text = s.Insert(start, value);
        Lenght += run.text.Length;
        break;
      }
    }

    
    
    /// BODY
    int FSelectionStart;
    int FSelectionLength;
    /// <summary>
    /// 
    /// </summary>
    public int SelectionStart { get { return FSelectionStart; } internal set { FSelectionStart = value; } }
    /// <summary>
    /// 
    /// </summary>
    public int SelectionLength { get { return FSelectionLength; } internal set { FSelectionLength = value; } }
    /// <summary>
    /// 
    /// </summary>
    public string SelectedText
    {
      get
      {
        return Text.Substring(FSelectionStart, FSelectionLength);
      }
      internal set
      {
        document.ReplaceText(FSelectionStart, FSelectionLength, value);
      }
    }



    //internal void Body_ReplaceText(int fSelectionStart, int fSelectionLength, string value)
    //{
    //  int par_len, start, len;
    //  foreach (RTF_Page item in pages)
    //  {
    //    par_len = item.Lenght;
    //    if (par_len <= fSelectionStart)
    //    {
    //      // TDOD: check EndOfLine sequence - seems to be platfom dependent LF of CR/LF
    //      fSelectionStart -= par_len + 2;
    //      continue;
    //    }
    //    start = fSelectionStart;
    //    len = (par_len - start < fSelectionLength) ? par_len - start : fSelectionLength;
    //    item.ReplaceText(start, len, value);
    //    break;
    //  }
    //}


    internal string Body_Text2HTML
    {
      get
      {
        StringBuilder str = new StringBuilder();
        foreach (RTF_Page item in body.pages)
          str.AppendLine(item.Text2HTML);
        return str.ToString();
      }
    }

    internal string Body_Text
    {
      get
      {
        StringBuilder str = new StringBuilder();
        foreach (RTF_Page item in body.pages)
          str.AppendLine(item.Text);
        return str.ToString();
      }
    }


    public int Document_ReplaceAll(string findWhat, string replaceWith)
    {
      int replacement_counter = 0;
      if (this.document.Pages != null)
        foreach (RTF_Page page in this.document.Pages)
        {
          replacement_counter += page.ReplaceAll(findWhat, replaceWith);
        }
      return replacement_counter;
    }


    internal void Page_ReplaceText(int fSelectionStart, int fSelectionLength, string value)
    {
      int par_len, start, len;
      foreach (RTF_CommonRichElement item in paragraphs)
      {
        par_len = item.Lenght;
        if (par_len <= fSelectionStart)
        {
          // TDOD: check EndOfLine sequence - seems to be platfom dependent LF of CR/LF
          fSelectionStart -= par_len + 2;
          continue;
        }
        start = fSelectionStart;
        len = (par_len - start < fSelectionLength) ? par_len - start : fSelectionLength;
        item.ReplaceText(start, len, value);
        break;
      }
    }

    internal int Page_ReplaceAll(string findWhat, string replaceWith)
    {
      int replacement_counter = 0;
      //foreach (RTF_CommonRichElement par in this.ParagraphsAndTables)
      //{
      //  replacement_counter += par.RepaceAll(findWhat, replaceWith);
      //}
      return replacement_counter;
    }

    internal string Page_Text2HTML
    {
      get
      {
        StringBuilder str = new StringBuilder();
        foreach (RTF_CommonRichElement item in page.paragraphs)
        {
          str.AppendLine(item.GetHTMLText(document));
        }
        return str.ToString();
      }
    }

    internal string Page_Text
    {
      get
      {
        StringBuilder str = new StringBuilder();
        foreach (RichObject item in page.objects)
        {
          //str.AppendLine(item.GetText());
        }
        return str.ToString();
      }
    }

    internal override string Table_GetText()
    {
      StringBuilder sb = new StringBuilder("TODO: Text of raw cells");
      //foreach (RTF_CommonRichElement item in some_row.cells)
      //{
      //  sb.AppendFormat("{0}\t", item.GetText());
      //}
      return sb.ToString();
    }

    internal override string Paragraph_GetText()
    {
      StringBuilder sb = new StringBuilder();
      foreach (Run run in Runs)
      {
        if (run.text.Length == 0)
          sb.Append('\t');
        else
          sb.Append(run.text);
      }
      return sb.ToString();
    }

    internal override string Picture_GetText()
    {
      return string.Empty;
    }

    internal override string TableGetHTMLText(RTF_DocumentParser doc)
    {
      throw new NotImplementedException("RTF Table to HTML");
    }

    internal override string Paragraph_GetHTMLText(RTF_DocumentParser doc)
    {
      StringBuilder sb = new StringBuilder();
      RunFormat prev_format = new RunFormat();

      foreach (Run run in Runs)
      {
        if (run.text.Length == 0)
        {
            // sb.Append('\t');
            sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
        }
        else
        {
          if (run.format.IsSameAs(prev_format))
            sb.Append(run.text);
          else
          {
            if (run.format.bold != prev_format.bold)
            {
              sb.Append(run.format.bold ? "<b>" : "</b>");
              prev_format.bold = run.format.bold;
            }
            if (run.format.italic != prev_format.italic)
            {
              sb.Append(run.format.italic ? "<i>" : "</i>");
              prev_format.italic = run.format.italic;
            }
            if (run.format.underline != prev_format.underline)
            {
              sb.Append(run.format.underline ? "<u>" : "</u>");
              prev_format.underline = run.format.underline;
            }
            if(run.format.script_type != prev_format.script_type)
            {
              if (run.format.script_type == RunFormat.ScriptType.Subscript)
                sb.Append("<sub>");
              else if (run.format.script_type == RunFormat.ScriptType.Superscript)
                sb.Append("<sup>");
              else if(prev_format.script_type == RunFormat.ScriptType.Subscript)
                sb.Append("</sub>");
              else if (prev_format.script_type == RunFormat.ScriptType.Superscript)
                sb.Append("</sup>");
              prev_format.script_type = run.format.script_type;
            }
            if (run.format.color_idx != prev_format.color_idx)
            {
              RColor col = doc.ColorList[(int)run.format.color_idx];
              string colorname = string.Format("\"#{0:X2}{1:X2}{2:X2}\"", col.red, col.green, col.blue);

              sb.Append(run.format.color_idx == 0 ? "</font>" : "<font color="+colorname+">");
              prev_format.color_idx = run.format.color_idx;
            }
            //if(run.format.font_size != prev_format.font_size)
            //{
            //  int fs = run.format.font_size / 2;
            //  sb.Append("<font size=\"" + fs +"\">");
            //  prev_format.font_size = run.format.font_size;
            //}
            sb.Append(run.text);
          }
        }
      }
      // Clean collected tags
      if (prev_format.bold)
        sb.Append("</b>");
      if (prev_format.italic)
        sb.Append("</i>");
      if (prev_format.underline)
        sb.Append("</u>");
      return sb.ToString();
    }

    internal override int Paragraph_RepaceAll(string findWhat, string replaceWith)
    {
      int replacement_counter = 0;
      foreach (Run run in Runs)
      {
        replacement_counter += run.ReplaceAll(findWhat, replaceWith);
      }
      return replacement_counter;
    }

    internal override int Table_RepaceAll(string findWhat, string replaceWith)
    {
      throw new NotImplementedException();
    }


#endif
  }
}
