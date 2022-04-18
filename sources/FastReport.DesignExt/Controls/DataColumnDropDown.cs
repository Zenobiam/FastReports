using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using FastReport.Data;

namespace FastReport.Controls
{
  internal class DataColumnDropDown : ToolStripDropDown
  {
    private ToolStripControlHost host;
    private DataTreeView tree;

    public event EventHandler ColumnSelected;

    public DataTreeView DataTree
    {
      get { return tree; }
    }

    public DataSourceBase DataSource
    {
      get { return tree.DataSource; }
      set { tree.DataSource = value; }
    }

    public string Column
    {
      get { return tree.SelectedItem; }
      set { tree.SelectedItem = value; }
    }

    private void FTree_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if ((e.Node.Tag == null || Column != "") && ColumnSelected != null)
      {
        Close();
        ColumnSelected(this, EventArgs.Empty);
      }
    }

    public void CreateNodes(Report report)
    {
      tree.CreateNodes(report.Dictionary);
    }

    public void SetSize(int width, int height)
    {
      tree.Size = new Size(width - 2, height);
    }

    public DataColumnDropDown()
    {
      tree = new DataTreeView();
      tree.Size = new Size(200, 250);
      tree.BorderStyle = BorderStyle.None;
      tree.HideSelection = false;
      tree.ShowEnabledOnly = true;
      tree.ShowNone = true;
      tree.ShowRelations = true;
      tree.ShowVariables = false;
      tree.ShowParameters = false;
      tree.Font = DrawUtils.DefaultFont;
      tree.AfterSelect += new TreeViewEventHandler(FTree_AfterSelect);
      host = new ToolStripControlHost(tree);
      Items.Add(host);
    }
  }
}
