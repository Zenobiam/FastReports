using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FastReport.RichTextParser
{
  internal class View_Table : RTF_View.CommonViewObject
  {
    internal class View_Cell
    {
      internal List<RTF_View.CommonViewObject> view_paragraphs = new List<RTF_View.CommonViewObject>();
      internal int Height = 0;
      internal int Top = 0;
      internal int Left = 0;
      internal int Width = 0;

      internal void SplitToRuns(RichObjectSequence seq, RTF_View view)
      {
        foreach (RichObject cell in seq.objects)
        {
          switch (cell.type)
          {
            case RichObject.Type.Paragraph:
              View_Paragraph par = new View_Paragraph(view);
              par.SplitParagraph(cell.pargraph);
              view_paragraphs.Add(par);
              break;
            case RichObject.Type.Table:
              View_Table table = new View_Table(view);
              throw new NotImplementedException("Table in table");
            // break;
            case RichObject.Type.Picture:
              View_Picture pict = new View_Picture(view);
              pict.SpitToRuns(cell);
              view_paragraphs.Add(pict);
              break;
          }
        }
      }

      internal void DrawBorder(Graphics g, PointF pos, Column col)
      {
        Pen pen = new Pen(Color.Black);

        int left = (int) (Left + pos.X);
        int top = Top + (int) pos.Y;
        int right = left + Width;
        int bottom = top + Height - 1;

#if false // This is a test

                pen.Width = 1;
                pen.Color = Color.Red;
                g.DrawLine(pen, Left, top, right, top);
                g.DrawLine(pen, Left, top, Left, bottom);
                g.DrawLine(pen, Left, bottom, right, bottom);
                g.DrawLine(pen, right, top, right, bottom);
#else

        if (col.border_top.width != 0)
        {
          pen.Width = RTF_View.Twips2Pixels((int)col.border_top.width);
          pen.Color = col.border_top.color;
          g.DrawLine(pen, left, top, right, top);
        }
        if (col.border_left.width != 0)
        {
          pen.Width = RTF_View.Twips2Pixels((int)col.border_left.width);
          pen.Color = col.border_left.color;
          g.DrawLine(pen, left, top, left, bottom);
        }
        if (col.border_bottom.width != 0)
        {
          pen.Width = RTF_View.Twips2Pixels((int)col.border_bottom.width);
          pen.Color = col.border_bottom.color;
          g.DrawLine(pen, left, bottom, right, bottom);
        }
        if (col.border_right.width != 0)
        {
          pen.Width = RTF_View.Twips2Pixels((int)col.border_right.width);
          pen.Color = col.border_right.color;
          g.DrawLine(pen, right, top, right, bottom);
        }
#endif
            }
        }

            internal class View_Row
    {
      internal TableRow table_row;

      internal List<View_Cell> view_cells = new List<View_Cell>();
      internal int Height = 0;
      internal int Top = 0;

      internal void SplitToRuns(RTF_View view)
      {
        foreach(RichObjectSequence seq in table_row.cells)
        {
          View_Cell cell = new View_Cell();
          cell.SplitToRuns(seq, view);
          view_cells.Add(cell);
        }
      }

      internal View_Row(TableRow table_row)
      {
        this.table_row = table_row;
      }
    }

    internal List<View_Row> rows;
    internal List<Column> columns;

    internal override void SpitToRuns(RichObject parser_object)
    {
      foreach (TableRow row in parser_object.table.rows)
      {
        View_Row view_row = new View_Row(row);
        view_row.SplitToRuns(view);
        rows.Add(view_row);
      }

      foreach (Column column in parser_object.table.columns)
      {
        columns.Add(column);
      }
    }

    private void OrderCellsVertically(View_Row row, int to_H)
    {
      int i = 0;
      foreach (Column column in columns)
      {
        int top = 0;
        if (row.view_cells.Count == i)
          continue;
        View_Cell view_cell = row.view_cells[i++];
        view_cell.Height = to_H;

        int from_H = 0;
        foreach (RTF_View.CommonViewObject view_par in view_cell.view_paragraphs)
          from_H += view_par.Height;
        switch (column.valign)
        {
          case Column.VertAlign.Top:
            break;
          case Column.VertAlign.Center:
            top = (to_H - from_H) / 2;
            break;
          case Column.VertAlign.Bottom:
            top = to_H - from_H;
            break;
        }
        foreach (RTF_View.CommonViewObject view_par in view_cell.view_paragraphs)
        {
          if(view_par is View_Paragraph)
            (view_par as View_Paragraph).ShiftLinesDown(top);
          top += view_par.Height;
        }
      }
    }

    internal override int Prepare(Graphics g)
    {
      int table_height = 0, cell_height;
      int pdx = 0;
      int first_row_height = 0;

      foreach (View_Row row in this.rows)
      {
        int pos_x = Left;
        int col_idx = 0;
        int row_height = 0;
        row.Top = table_height;
        foreach (View_Cell cell in row.view_cells)
        {
          Column col = columns[col_idx++];

          int dx = RTF_View.Twips2Pixels((int)col.Width) - pdx;
          pdx += dx;

          cell_height = 0;
          foreach (RTF_View.CommonViewObject par in cell.view_paragraphs)
          {
            par.UpdatePads(row.table_row, Top, pos_x, dx);
                        par.Top = cell_height;
            par.Height = par.Prepare(g);
                cell_height += par.Height;
          }
          cell.Left = pos_x;
          cell.Width = dx;
          row_height = (row_height < cell_height) ? cell_height : row_height;
          pos_x += dx + 1;
        }
        // Reuse variables here. Bad style...
        pdx = RTF_View.Twips2Pixels(row.table_row.height);
        if (pdx != 0)
        {
          first_row_height = pdx;
          row_height = pdx < 0 ? -pdx : row_height < pdx ? pdx : row_height;
        }
        else
        {
          row_height = first_row_height < 0 ? -first_row_height : row_height < first_row_height ? first_row_height : row_height;
        }
        OrderCellsVertically(row, row_height);
        row.Height = row_height;
        table_height += row_height;
        pdx = 0;
      }
      Height = table_height;
      return table_height;
    }

    internal override int Paint(Graphics g, PointF pos)
    {
      foreach (View_Row row in this.rows)
      {
        int col_idx = 0;
        foreach (View_Cell cell in row.view_cells)
        {
          Column col = columns[col_idx++];

          Brush brush = new SolidBrush(col.back_color);
          g.FillRectangle(brush, cell.Left + pos.X, pos.Y, cell.Width, cell.Height);

          foreach (RTF_View.CommonViewObject par in cell.view_paragraphs)
          {
//            par.Paint(g, top + par.Top);
            par.Paint(g, pos);
          }
          cell.DrawBorder(g, pos, col);
        }
        pos.Y += row.Height;
      }
      return (int) pos.Y;
    }

    //internal override int FindString(string findWhat, out int line, out int position)
    //{
    //  throw new NotImplementedException("Find string in RTF table");
    //}

    public override void Dispose()
    {
      throw new NotImplementedException("Dispose RTF table");
    }

    internal override void UpdatePads(TableRow row, int top, int pos_x, int dx)
    {
      throw new NotImplementedException();
    }

        internal override void InsertText(int position_within_object, string text)
        {
            throw new NotImplementedException("TODO: inset text into table");
        }

        internal View_Table(RTF_View parent)
    {
      view = parent;
      columns = new List<Column>();
      rows = new List<View_Row>() ;
    }
  }
}
