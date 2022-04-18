using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Design;
using FastReport.Utils;
using System.Drawing;

namespace FastReport.Table
{
  internal class TableColumnMenu : TableMenuBase
  {
    #region Fields
    private SelectedObjectCollection selection;
    private TableBase table;
    private TableColumn column;
    public ContextMenuItem miInsertColumnToLeft;
    public ContextMenuItem miInsertColumnToRight;
    public ContextMenuItem miAutoSize;
    public ContextMenuItem miCut;
    public ContextMenuItem miPaste;
    public ContextMenuItem miDelete;
    #endregion

    #region Properties
    #endregion

    #region Private Methods
    private void miInsertColumnToLeft_Click(object sender, EventArgs e)
    {
      TableColumn column = new TableColumn();
            table.Columns.Insert(table.Columns.IndexOf(this.column), column);
      table.CreateUniqueNames();
      selection.Clear();
      selection.Add(column);
      Change();
    }

    private void miInsertColumnToRight_Click(object sender, EventArgs e)
    {
      TableColumn column = new TableColumn();
            table.Columns.Insert(table.Columns.IndexOf(this.column) + 1, column);
      table.CreateUniqueNames();
      selection.Clear();
      selection.Add(column);
      Change();
    }

    private void miAutoSize_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < selection.Count; i++)
      {
        (selection[i] as TableColumn).AutoSize = miAutoSize.Checked;
      }
      Change();
    }

    private void miCut_Click(object sender, EventArgs e)
    {
      TableColumn[] columns = new TableColumn[selection.Count];
      for (int i = 0; i < selection.Count; i++)
      {
        columns[i] = selection[i] as TableColumn;
      }
      table.Clipboard.CutColumns(columns);
      selection.Clear();
      selection.Add(table);
      Change();
    }

    private void miPaste_Click(object sender, EventArgs e)
    {
      table.Clipboard.PasteColumns(table.Columns.IndexOf(column));
      Change();
    }

    private void miDelete_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < selection.Count; i++)
      {
        TableColumn column = selection[i] as TableColumn;
        if (column != null)
        {
          if (column.IsAncestor)
          {
            FRMessageBox.Error(String.Format(Res.Get("Messages,DeleteAncestor"), column.Name));
            break;
          }
          else
            table.Columns.Remove(column);
        }  
      }
      selection.Clear();
      selection.Add(table);
      Change();
    }
    #endregion

    public TableColumnMenu(Designer designer) : base(designer)
    {
      selection = Designer.SelectedObjects;
      column = selection[0] as TableColumn;
      table = column.Parent as TableBase;

      miInsertColumnToLeft = CreateMenuItem(Res.GetImage(220), Res.Get("ComponentMenu,TableColumn,InsertToLeft"), new EventHandler(miInsertColumnToLeft_Click));
      miInsertColumnToRight = CreateMenuItem(Res.GetImage(221), Res.Get("ComponentMenu,TableColumn,InsertToRight"), new EventHandler(miInsertColumnToRight_Click));
      miAutoSize = CreateMenuItem(null, Res.Get("ComponentMenu,TableRow,AutoSize"), new EventHandler(miAutoSize_Click));
      miAutoSize.BeginGroup = true;
      miAutoSize.CheckOnClick = true;
      miCut = CreateMenuItem(Res.GetImage(5), Res.Get("Designer,Menu,Edit,Cut"), new EventHandler(miCut_Click));
      miCut.BeginGroup = true;
      miPaste = CreateMenuItem(Res.GetImage(7), Res.Get("Designer,Menu,Edit,Paste"), new EventHandler(miPaste_Click));
      miDelete = CreateMenuItem(Res.GetImage(51), Res.Get("Designer,Menu,Edit,Delete"), new EventHandler(miDelete_Click));

      miAutoSize.Checked = column.AutoSize;
      miCut.Enabled = table.Columns.Count > 1;
      miPaste.Enabled = table.Clipboard.CanPasteColumns;
      miDelete.Enabled = table.Columns.Count > 1;

      Items.AddRange(new ContextMenuItem[] {
        miInsertColumnToLeft, miInsertColumnToRight, 
        miAutoSize,
        miCut, miPaste, miDelete });
    }
  }
}
