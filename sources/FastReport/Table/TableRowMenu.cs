using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Design;
using FastReport.Utils;

namespace FastReport.Table
{
  internal class TableRowMenu : TableMenuBase
  {
    #region Fields
    private SelectedObjectCollection selection;
    private TableBase table;
    private TableRow row;
    public ContextMenuItem miInsertRowAbove;
    public ContextMenuItem miInsertRowBelow;
    public ContextMenuItem miAutoSize;
    public ContextMenuItem miCut;
    public ContextMenuItem miPaste;
    public ContextMenuItem miDelete;
    #endregion

    #region Properties
    #endregion

    #region Private Methods
    private void miInsertRowAbove_Click(object sender, EventArgs e)
    {
      TableRow row = new TableRow();
            table.Rows.Insert(table.Rows.IndexOf(this.row), row);
      table.CreateUniqueNames();
      selection.Clear();
      selection.Add(row);
      Change();
    }

    private void miInsertRowBelow_Click(object sender, EventArgs e)
    {
      TableRow row = new TableRow();
            table.Rows.Insert(table.Rows.IndexOf(this.row) + 1, row);
      table.CreateUniqueNames();
      selection.Clear();
      selection.Add(row);
      Change();
    }

    private void miAutoSize_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < selection.Count; i++)
      {
        (selection[i] as TableRow).AutoSize = miAutoSize.Checked;
      }
      Change();
    }

    private void miCut_Click(object sender, EventArgs e)
    {
      TableRow[] rows = new TableRow[selection.Count];
      for (int i = 0; i < selection.Count; i++)
      {
        rows[i] = selection[i] as TableRow;
      }
      table.Clipboard.CutRows(rows);
      selection.Clear();
      selection.Add(table);
      Change();
    }

    private void miPaste_Click(object sender, EventArgs e)
    {
      table.Clipboard.PasteRows(table.Rows.IndexOf(row));
      selection.Clear();
      selection.Add(table);
      Change();
    }

    private void miDelete_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < selection.Count; i++)
      {
        TableRow row = selection[i] as TableRow;
        if (row != null)
        {
          if (row.IsAncestor)
          {
            FRMessageBox.Error(String.Format(Res.Get("Messages,DeleteAncestor"), row.Name));
            break;
          }
          else
            table.Rows.Remove(row);
        }  
      }
      selection.Clear();
      selection.Add(table);
      Change();
    }
    #endregion

    public TableRowMenu(Designer designer) : base(designer)
    {
      selection = Designer.SelectedObjects;
      row = selection[0] as TableRow;
      table = row.Parent as TableBase;

      miInsertRowAbove = CreateMenuItem(Res.GetImage(218), Res.Get("ComponentMenu,TableRow,InsertAbove"), new EventHandler(miInsertRowAbove_Click));
      miInsertRowBelow = CreateMenuItem(Res.GetImage(219), Res.Get("ComponentMenu,TableRow,InsertBelow"), new EventHandler(miInsertRowBelow_Click));
      miAutoSize = CreateMenuItem(null, Res.Get("ComponentMenu,TableRow,AutoSize"), new EventHandler(miAutoSize_Click));
      miAutoSize.BeginGroup = true;
      miAutoSize.CheckOnClick = true;
      miCut = CreateMenuItem(Res.GetImage(5), Res.Get("Designer,Menu,Edit,Cut"), new EventHandler(miCut_Click));
      miCut.BeginGroup = true;
      miPaste = CreateMenuItem(Res.GetImage(7), Res.Get("Designer,Menu,Edit,Paste"), new EventHandler(miPaste_Click));
      miDelete = CreateMenuItem(Res.GetImage(51), Res.Get("Designer,Menu,Edit,Delete"), new EventHandler(miDelete_Click));
      
      miAutoSize.Checked = row.AutoSize;
      miCut.Enabled = table.Rows.Count > 1;
      miPaste.Enabled = table.Clipboard.CanPasteRows;
      miDelete.Enabled = table.Rows.Count > 1;

      Items.AddRange(new ContextMenuItem[] {
        miInsertRowAbove, miInsertRowBelow, 
        miAutoSize,
        miCut, miPaste, miDelete });
    }
  }
}
