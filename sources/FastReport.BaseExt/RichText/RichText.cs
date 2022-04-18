// 
// FastReport RichText infrastructure
// 

using System.Collections.Generic;

namespace FastReport.RichTextParser
{
#pragma warning disable 1591
  /// <summary>
  /// Internal representation of RichText document
  /// </summary>
  public struct RichDocument
  {
    public long size;
    public List<Page> pages;
    // RTF header
    public List<RFont> font_list;
    public List<System.Drawing.Color> color_list;
    public List<Style> style_list;

    public long codepage;
    public long default_font;
    public long default_lang;
    // RTF body
    public long paper_width;
    public long paper_height;
    public long global_margin_left;
    public long global_margin_top;
    public long global_margin_right;
    public long global_margin_bottom;
    public long default_tab_width;
    public long view_kind;
  }

  public struct Page
  {
    public long size;
    public bool soft_break;
    public long page_width;
    public long page_heigh;
    public long margin_top;
    public long margin_left;
    public long margin_right;
    public long margin_bottom;
    public RichObjectSequence sequence;
        public RichObjectSequence header;
        public RichObjectSequence footer;
  }

  public struct RichObjectSequence
  {
    public long size;
    public List<RichObject> objects;
  }

  public struct RichObject
  {
    public enum Type { Paragraph, Table, Picture }
    public long size;
    public Type type;
    public Paragraph pargraph;
    public Table table;
    public Picture picture;
  }

  public struct Paragraph
  {
    public long size;
    public List<Run> runs;
    public ParagraphFormat format;
  }

  public struct ParagraphFormat
  {
    public enum VerticalAlign { Top, Center, Bottom }
    public enum HorizontalAlign { Centered, Justified, Left, Right, Distributed, Kashida, Thai }
    public enum LnSpcMult { Exactly, Multiply }
    public enum Direction { LeftToRight, RighgToLeft }

    public HorizontalAlign align;
    public VerticalAlign Valign;
    public int line_spacing;
    public int space_before;
    public int space_after;
    public int left_indent;
    public int right_indent;
    public int first_line_indent;
    public LnSpcMult lnspcmult;
    public int pnstart; // Level of bullets/numbering. Zero for ordinary paragraph
    public Direction text_direction;
    public List<Run> list_id;
    public List<int> tab_positions;
  }

#if READONLY_STRUCTS
    public readonly struct Run
#else
    public struct Run
#endif
    {
        public readonly string text;
        public readonly RunFormat format;

        public Run(string text, RunFormat format)
        {
            this.text = text;
            this.format = format;
        }
    }

  public struct RunFormat
  {
    public enum ScriptType { PlainText, Subscript, Superscript };

    public bool bold;
    public bool italic;
    public bool underline;
    public ScriptType script_type;
    public System.Drawing.Color color;
    public System.Drawing.Color BColor;
    public System.Drawing.Color FillColor;
    public uint font_idx;
    public int font_size;

    public bool IsSameAs(RunFormat rf)
    {
      return bold == rf.bold && italic == rf.italic && underline == rf.underline && script_type == rf.script_type &&
        color == rf.color && font_idx == rf.font_idx && font_size == rf.font_size;
    }

#if false
    internal ulong HashKey()
    {
      ulong key = 0;
      if (bold) key |= 1;
      if (italic) key |= 2;
      key |= (ulong)(color_idx << 2);
      key ^= (ulong)(font_idx << 10);
      return key;
    }
#endif
  }

  public struct RFont
  {
    public enum Family { Nil, Rroman, Swiss, Modern, Script, Decor, Tech, Bidi }

    public uint font_id;
    public long charset;
    public Family family;
    public string FontName;
  }

  public struct BorderLine
  {
    public enum Style { Thin, Thick, Double, Dotted };

    public Style style;
    public uint width;
    public System.Drawing.Color color;
  }

  public struct Picture
  {
    public long size;
    public System.Drawing.Image image;
    public int width, height;
    public int scalex, scaley;
    public int desired_width, desired_height;
    public int crop_left;
    public int crop_top;
    public int crop_right;
    public int crop_bottom;
    public bool picprop; // Indicates that shape properties are applied to an inline picture. 
    public int tag;
    public int units_per_inch;
  }

  public struct Column
  {
    public enum VertAlign { Top, Center, Bottom };

    public VertAlign valign;
    public System.Drawing.Color back_color;
    public bool verticallY_merged;
    public bool horizontally_merged;
    public BorderLine border_top;
    public BorderLine border_left;
    public BorderLine border_right;
    public BorderLine border_bottom;
    private uint width;

    public uint Width { get { return width; } set { width = value; } }
  }

  public struct TableRow
  {
    public long size;
    public int height;
    public uint trgaph; // Half the space between the cells of a table row in twips.
    public int default_pad_left;
    public int default_pad_right;
    public List<RichObjectSequence> cells;
  }

  public struct Table
  {
    public long size;
    public List<Column> columns;
    public List<TableRow> rows;
  }

  public struct Style
  {
    public int styledef;
    public ParagraphFormat paragraph_style;
    public RunFormat run_style;
    public string stylename;
  }
#pragma warning restore 1591
}
