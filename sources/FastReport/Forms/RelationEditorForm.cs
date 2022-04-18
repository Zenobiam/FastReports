using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Data;

namespace FastReport.Forms
{
  internal partial class RelationEditorForm : BaseDialogForm
  {
    private Relation relation;

    private void cbxParent_DrawItem(object sender, DrawItemEventArgs e)
    {
      e.DrawBackground();
      if (e.Index >= 0)
      {
        DataSourceBase dataSource = (sender as ComboBox).Items[e.Index] as DataSourceBase;
        using (Brush b = new SolidBrush(e.ForeColor))
        {
          e.Graphics.DrawString(dataSource.Alias, e.Font, b, e.Bounds.X, e.Bounds.Y);
        }
      }
    }
    
    private string[] GetColumns(DataSourceBase data)
    {
      string[] result = new string[data.Columns.Count + 1];
      result[0] = "";
      for (int i = 0; i < data.Columns.Count; i++)
      {
        result[i + 1] = data.Columns[i].Alias;
      }
      return result;
    }
    
    private void GetColumns(List<string> parentColumns, List<string> childColumns)
    {
      foreach (DataGridViewRow row in gvColumns.Rows)
      {
        object column = row.Cells[0].Value;
        if (column != null && column.ToString() != "")
          parentColumns.Add(column.ToString());
        column = row.Cells[1].Value;
        if (column != null && column.ToString() != "")
          childColumns.Add(column.ToString());
      }
    }

    private void cbxParent_SelectedIndexChanged(object sender, EventArgs e)
    {
      foreach (DataGridViewRow row in gvColumns.Rows)
      {
        row.Cells[0].Value = "";
      }
      DataSourceBase data = cbxParent.SelectedItem as DataSourceBase;
      clParent.Items.Clear();
      clParent.Items.AddRange(GetColumns(data));
    }

    private void cbxChild_SelectedIndexChanged(object sender, EventArgs e)
    {
      foreach (DataGridViewRow row in gvColumns.Rows)
      {
        row.Cells[1].Value = "";
      }
      DataSourceBase data = cbxChild.SelectedItem as DataSourceBase;
      clChild.Items.Clear();
      clChild.Items.AddRange(GetColumns(data));
    }

    private void RelationEditorForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult == DialogResult.OK)
      {
        List<string> parentColumns = new List<string>();
        List<string> childColumns = new List<string>();
        GetColumns(parentColumns, childColumns);
        e.Cancel = !(parentColumns.Count > 0 && childColumns.Count > 0 && parentColumns.Count == childColumns.Count);
        if (e.Cancel)
          FRMessageBox.Error(Res.Get("Forms,RelationEditor,Error"));
      }
    }

    private void RelationEditorForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Done();
    }

    private int DropDownWidth(ComboBox cbx)
    {
        int maxWidth = 0;
        int temp = 0;
        foreach (object obj in cbx.Items)
        {
            temp = TextRenderer.MeasureText(cbx.GetItemText(obj), cbx.Font).Width;
            if (temp > maxWidth)
                maxWidth = temp;
        }
        return maxWidth + SystemInformation.VerticalScrollBarWidth;
    }
    
    private void Init()
    {
      Dictionary dictionary = relation.Report.Dictionary;
      ObjectCollection allObjects = dictionary.AllObjects;
      SortedList<string, DataSourceBase> tables = new SortedList<string, DataSourceBase>();
      foreach (Base c in allObjects)
      {
        if (c is DataSourceBase)
        {
          DataSourceBase table = c as DataSourceBase;
          if (!tables.ContainsKey(table.Alias))
            tables.Add(table.Alias, table);
        }
      }

      foreach (KeyValuePair<string, DataSourceBase> keyValue in tables)
      {
        DataSourceBase table = keyValue.Value;
        cbxParent.Items.Add(table);
        cbxChild.Items.Add(table);
      }
      
      cbxParent.SelectedItem = relation.ParentDataSource;
      cbxChild.SelectedItem = relation.ChildDataSource;
      
      if (relation.ParentColumns != null)
      {
        for (int i = 0; i < relation.ParentColumns.Length; i++)
        {
          string[] row = new string[2] { relation.ParentColumns[i], relation.ChildColumns[i] };
          gvColumns.Rows.Add(row);
        }
      }

      cbxParent.DropDownWidth = DropDownWidth(cbxParent);
      cbxChild.DropDownWidth = DropDownWidth(cbxChild);
    }
    
    private void Done()
    {
      if (DialogResult == DialogResult.OK)
      {
        relation.ParentDataSource = cbxParent.SelectedItem as DataSourceBase;
        relation.ChildDataSource = cbxChild.SelectedItem as DataSourceBase;
        List<string> parentColumns = new List<string>();
        List<string> childColumns = new List<string>();
        GetColumns(parentColumns, childColumns);
        relation.ParentColumns = parentColumns.ToArray();
        relation.ChildColumns = childColumns.ToArray();
      }
    }
    
    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,RelationEditor");
      Text = res.Get("");
      lblParentTable.Text = res.Get("ParentTable");
      lblChildTable.Text = res.Get("ChildTable");
      clParent.HeaderText = res.Get("Parent");
      clChild.HeaderText = res.Get("Child");
      lblColumns.Text = res.Get("Columns");
    }
    
    public RelationEditorForm(Relation relation)
    {
            this.relation = relation;
      InitializeComponent();
      Localize();
      Init();
            Scale();
    }
  }
}

