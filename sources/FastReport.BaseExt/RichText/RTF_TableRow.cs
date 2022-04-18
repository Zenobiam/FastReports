using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.RichTextParser
{
  /// <summary>
  /// This class represents a RTF column description.
  /// </summary>
  /// 
  class RTF_Column
  {
    private   Column column;

    private   RTF_BorderLine_Parser border_parser= new RichTextParser.RTF_BorderLine_Parser();
    enum CurrentSide { None, Top, Left, Bottom, Right }
    private CurrentSide side = CurrentSide.None;

    public Column Column { get { return column; } }

    internal bool Parse(RTF_Parser parser, RTF_Header header)
    {
      bool parsed = false;

      if (side != CurrentSide.None)
        parsed = border_parser.Parse(parser, header);

      if (parsed == false)
      {
        switch (side)
        {
          case CurrentSide.Top:
            column.border_top = border_parser.line;
            break;
          case CurrentSide.Left:
            column.border_left = border_parser.line;
            break;
          case CurrentSide.Bottom:
            column.border_bottom = border_parser.line;
            break;
          case CurrentSide.Right:
            column.border_right = border_parser.line;
            break;
        }
        side = CurrentSide.None;
        parsed = true;
        switch (parser.Control)
        {
          case "clcbpat":
            {
                int cidx = (int)parser.Number;
                if (cidx > 0)
                    --cidx;
                column.back_color = header.Document.color_list[cidx];
            }
            break;
          case "clvertalt":
            column.valign = Column.VertAlign.Top;
            parser.current_paragraph_format.Valign = ParagraphFormat.VerticalAlign.Top;
            break;
          case "clvertalc":
            column.valign = Column.VertAlign.Center;
            parser.current_paragraph_format.Valign = ParagraphFormat.VerticalAlign.Center;
            break;
          case "clvertalb":
            column.valign = Column.VertAlign.Bottom;
            parser.current_paragraph_format.Valign = ParagraphFormat.VerticalAlign.Bottom;
            break;
          case "cltxlrtb":
            // Ignore default text in a cell flows
            break;
          case "clbrdrt":
            side = CurrentSide.Top;
            border_parser.Clear();
            break;
          case "clbrdrl":
            side = CurrentSide.Left;
            border_parser.Clear();
            break;
          case "clbrdrb":
            side = CurrentSide.Bottom;
            border_parser.Clear();
            break;
          case "clbrdrr":
            side = CurrentSide.Right;
            border_parser.Clear();
            break;
          case "clvmgf":
            column.verticallY_merged = true;
            break;
          case "clmgf":
            column.horizontally_merged = true;
            break;
          default:
            parsed = false;
            break;
        }
      }
      return parsed;
    }

    internal void SetWidth(uint w)
    {
      column.Width = w;
    }

    internal RTF_Column()
    {
      column = new Column();
      column.back_color = System.Drawing.Color.Transparent;
    }
  }

  internal class RTF_Row : RTF_CommonRichElement
  {
    private RTF_SequenceParser sequence;

    private TableRow parsing_row;

    public TableRow Row { get { return parsing_row; } }

    internal override RichObject RichObject
    {
      get
      {
        RichObject rich = new RichObject();
        rich.type = RichObject.Type.Table;
        rich.table = sequence.CurrentTable;
        rich.size = sequence.CurrentTable.size;
        return rich;
      }
    }

    internal override bool Parse(RTF_Parser parser, RTF_Header header)
    {
      bool parsed = true;
      {
        switch (parser.Control)
        {
          case "trrh":
            parsing_row.height = (int)parser.Number;
            break;

          case "trgaph":
            parsing_row.trgaph = (uint)parser.Number;
            break;

          case "trpaddl":
            parsing_row.default_pad_left = (int)parser.Number;
            break;
          case "trpaddr":
            parsing_row.default_pad_right = (int)parser.Number;
            break;

          default:
            parsed = false;
            break;
        }
      }
      return parsed;
    }

    internal void AddCell(RichObjectSequence curr_par)
    {
      parsing_row.cells.Add(curr_par);
    }

    /// <summary>
    /// Create RTF row
    /// </summary>
    /// <param name="parent_sequence"></param>
    /// 
    internal RTF_Row(RTF_SequenceParser parent_sequence)
    {
      sequence = parent_sequence;
      parsing_row.cells = new List<RichObjectSequence>();
    }
  }
}