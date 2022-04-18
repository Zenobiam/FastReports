using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Design;
using FastReport.Utils;
using FastReport.Forms;

namespace FastReport.Table
{
  internal class TableCellMenu : TableMenuBase
  {
    #region Fields
    private SelectedObjectCollection selection;
    private TableBase table;
    private TableCell topCell;
    public ContextMenuItem miFormat;
    public ContextMenuItem miJoinSplit;
    public ContextMenuItem miClear;
    public ContextMenuItem miCut;
    public ContextMenuItem miCopy;
    public ContextMenuItem miPaste;
    #endregion

    #region Private Methods
    private void miFormat_Click(object sender, EventArgs e)
    {
      using (FormatEditorForm form = new FormatEditorForm())
      {
        form.TextObject = topCell;
        if (form.ShowDialog() == DialogResult.OK)
        {
          SelectedTextBaseObjects components = new SelectedTextBaseObjects(Designer);
          components.Update();
          components.SetFormat(form.Formats);
          Change();
        }
      }
    }

    private void miJoinSplit_Click(object sender, EventArgs e)
    {
      if (miJoinSplit.Checked)
      {
        Rectangle rect = table.GetSelectionRect();
        
        // reset spans inside selection
        for (int x = 0; x < rect.Width; x++)
        {
          for (int y = 0; y < rect.Height; y++)
          {
            TableCell cell = table[x + rect.X, y + rect.Y];
            cell.ColSpan = 1;
            cell.RowSpan = 1;
            if (cell != topCell)
              cell.Text = "";
          }
        }
        
        topCell.ColSpan = rect.Width;
        topCell.RowSpan = rect.Height;
        selection.Clear();
        selection.Add(topCell);
      }
      else
      {
        topCell.ColSpan = 1;
        topCell.RowSpan = 1;
      }

      Change();
    }

    private void miClear_Click(object sender, EventArgs e)
    {
      foreach (Base c in selection)
      {
        if (c is TableCell)
          (c as TableCell).Text = "";
      }
      Change();
    }

    private void miCut_Click(object sender, EventArgs e)
    {
      table.Clipboard.CutCells(table.GetSelectionRect());
      table.CreateUniqueNames();
      Change();
    }

    private void miCopy_Click(object sender, EventArgs e)
    {
      table.Clipboard.CopyCells(table.GetSelectionRect());
    }

    private void miPaste_Click(object sender, EventArgs e)
    {
      table.Clipboard.PasteCells(topCell.Address);
      table.CreateUniqueNames();
      selection.Clear();
      selection.Add(table);
      Change();
    }

    #endregion

    public TableCellMenu(Designer designer) : base(designer)
    {
      selection = Designer.SelectedObjects;
      topCell = selection[0] as TableCell;
      table = topCell.Parent.Parent as TableBase;

      miFormat = CreateMenuItem(null, Res.Get("ComponentMenu,TextObject,Format"), new EventHandler(miFormat_Click));
      miJoinSplit = CreateMenuItem(Res.GetImage(217), Res.Get("ComponentMenu,TableCell,Join"), new EventHandler(miJoinSplit_Click));
      miJoinSplit.CheckOnClick = true;
      miClear = CreateMenuItem(Res.GetImage(82), Res.Get("ComponentMenu,TextObject,Clear"), new EventHandler(miClear_Click));
      miCut = CreateMenuItem(Res.GetImage(5), Res.Get("Designer,Menu,Edit,Cut"), new EventHandler(miCut_Click));
      miCut.BeginGroup = true;
      miCopy = CreateMenuItem(Res.GetImage(6), Res.Get("Designer,Menu,Edit,Copy"), new EventHandler(miCopy_Click));
      miPaste = CreateMenuItem(Res.GetImage(7), Res.Get("Designer,Menu,Edit,Paste"), new EventHandler(miPaste_Click));

      bool canJoin = selection.Count > 1;
      bool canSplit = selection.Count == 1 && topCell != null && (topCell.ColSpan > 1 || topCell.RowSpan > 1);
      miJoinSplit.Enabled = canJoin || canSplit;
      miJoinSplit.Checked = canSplit;
      if (miJoinSplit.Checked)
        miJoinSplit.Text = Res.Get("ComponentMenu,TableCell,Split");
      miPaste.Enabled = table.Clipboard.CanPasteCells;

      Items.AddRange(new ContextMenuItem[] {
        miFormat, miJoinSplit, miClear, 
        miCut, miCopy, miPaste });
    }
  }
}
