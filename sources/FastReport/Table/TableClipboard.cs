using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FastReport.Table
{
  internal class TableClipboard
  {
    #region Fields
    private TableBase table;
    private List<TableColumn> columns;
    private List<TableRow> rows;
    private TableCell[,] cells;
    private Size cellsSize;
    #endregion

    #region Properties
    public bool CanPasteColumns
    {
      get { return columns.Count > 0; }
    }
    
    public bool CanPasteRows
    {
      get { return rows.Count > 0; }
    }

    public bool CanPasteCells
    {
      get { return !CanPasteColumns && cells != null; }
    }
    #endregion
    
    #region Private Methods
    private void ClearColumns()
    {
      columns.Clear();
      cells = null;
    }
    
    private void ClearRows()
    {
      rows.Clear();
    }

    private void CutCells(Rectangle selection, bool remove)
    {
      cells = new TableCell[selection.Width, selection.Height];
      cellsSize = selection.Size;
      for (int x = 0; x < selection.Width; x++)
      {
        for (int y = 0; y < selection.Height; y++)
        {
          cells[x, y] = table[x + selection.X, y + selection.Y];
          if (remove)
            table[x + selection.X, y + selection.Y] = new TableCell();
        }
      }
    }
    #endregion

    #region Public Methods
    public void CutColumns(TableColumn[] columns)
    {
      ClearColumns();
      cells = new TableCell[columns.Length, table.RowCount];

      for (int x = 0; x < columns.Length; x++)
      {
                this.columns.Add(columns[x]);
        for (int y = 0; y < table.RowCount; y++)
        {
          cells[x, y] = table[columns[x].Index, y];
        }
      }
      foreach (TableColumn c in columns)
      {
        table.Columns.Remove(c);
      }
    }

    public void PasteColumns(int index)
    {
      for (int x = 0; x < columns.Count; x++)
      {
        table.Columns.Insert(index + x, columns[x]);
        for (int y = 0; y < table.RowCount; y++)
        {
          table[index + x, y] = cells[x, y];
        }
      }

      ClearColumns();
    }

    public void CutRows(TableRow[] rows)
    {
      ClearRows();
      foreach (TableRow r in rows)
      {
        table.Rows.Remove(r);
                this.rows.Add(r);
      }
    }

    public void PasteRows(int index)
    {
      for (int i = 0; i < rows.Count; i++)
      {
        table.Rows.Insert(index + i, rows[i]);
      }
      ClearRows();
    }

    public void CutCells(Rectangle selection)
    {
      ClearColumns();
      CutCells(selection, true);
    }

    public void CopyCells(Rectangle selection)
    {
      CutCells(selection, false);
    }

    public void PasteCells(Point newLocation)
    {
      for (int x = 0; x < cellsSize.Width; x++)
      {
        for (int y = 0; y < cellsSize.Height; y++)
        {
          table[x + newLocation.X, y + newLocation.Y] = cells[x, y].Clone();
        }
      }
    }
    #endregion
    
    public TableClipboard(TableBase table)
    {
            this.table = table;
      columns = new List<TableColumn>();
      rows = new List<TableRow>();
    }
  }
}
