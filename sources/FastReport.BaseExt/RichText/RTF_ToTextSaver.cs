using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text;

namespace FastReport.RichTextParser
{
#pragma warning disable 1591
  /// <summary>
  /// Save RTF document to plain text stream
  /// </summary>
  public class RTF_ToTextSaver : IDisposable
  {
    private RichDocument doc;

    public RTF_ToTextSaver(RichDocument doc)
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

    private void SavePargraph(StringBuilder s, Paragraph par, bool InTable)
    {
        foreach (Run r in par.runs)
          if (r.text.Length == 0)
            s.Append("\t");
          else
            s.Append(r.text);
      s.AppendLine();
    }

    private void SaveTable(StringBuilder s, Table tbl, bool InTable)
    {
      foreach (TableRow row in tbl.rows)
        SaveRow(s, row);
    }

    private void SaveRow(StringBuilder s, TableRow row)
    {
      foreach (RichObjectSequence seq in row.cells)
        SaveSequence(s, seq, true);
    }

    private void SaveSequence(StringBuilder s, RichObjectSequence seq, bool InTable)
    {
      foreach(RichObject robj in seq.objects)
      {
        switch (robj.type)
        {
          case RichObject.Type.Paragraph:
            SavePargraph(s, robj.pargraph, InTable);
            break;
          case RichObject.Type.Picture:
            // SkipPicture
            break;
          case RichObject.Type.Table:
            SaveTable(s, robj.table, InTable);
            break;
        }
      }
    }

    public string PlainText
    {
      get
      {
        StringBuilder s = new StringBuilder();
        SaveDocumentBody(s);
        return s.ToString();
      }
    }

    public void Dispose()
    {
      // Perhaps that everything clean
    }

  }
#pragma warning restore 1591
}
