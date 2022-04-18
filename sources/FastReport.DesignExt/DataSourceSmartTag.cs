using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Data;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport
{
  /// <summary>
  /// Represents a smart tag that is used to choose a data source.
  /// </summary>
  public class DataSourceSmartTag : SmartTagBase
  {
    private DataSourceBase dataSource;
    
    /// <summary>
    /// Gets or sets a data source.
    /// </summary>
    public DataSourceBase DataSource
    {
      get { return dataSource; }
      set { dataSource = value; }
    }

    private void AddDataSource(ToolStripItemCollection items, DataSourceBase data)
    {
      ToolStripMenuItem item = new ToolStripMenuItem();
      item.Tag = data;
      item.Text = data.Alias;
      item.Image = Res.GetImage(222);
      item.ImageScaling = ToolStripItemImageScaling.None;
      item.Click += new EventHandler(item_Click);
      items.Add(item);
      if (DataSource == data)
        item.Font = new Font(item.Font.Name, DpiHelper.ParseFontSize(item.Font.Size), FontStyle.Bold);
      else
        item.Font = new Font(item.Font.Name, DpiHelper.ParseFontSize(item.Font.Size), FontStyle.Regular);
    }

    private void item_Click(object sender, EventArgs e)
    {
      dataSource = (sender as ToolStripMenuItem).Tag as DataSourceBase;
      ItemClicked();
    }

    /// <inheritdoc/>
    protected override void CreateItems()
    {
      ToolStripMenuItem noneItem = new ToolStripMenuItem();
      noneItem.Text = Res.Get("Misc,None");
      noneItem.Image = Res.GetImage(76);
      noneItem.ImageScaling = ToolStripItemImageScaling.None;   
      noneItem.Click += new EventHandler(item_Click);
      if (DataSource == null)
        noneItem.Font = new Font(noneItem.Font.Name, DpiHelper.ParseFontSize(noneItem.Font.Size), FontStyle.Bold);
      else
        noneItem.Font = new Font(noneItem.Font.Name, DpiHelper.ParseFontSize(noneItem.Font.Size), FontStyle.Regular);
      Menu.Items.Add(noneItem);

      foreach (DataConnectionBase connection in Obj.Report.Dictionary.Connections)
      {
        foreach (DataSourceBase data in connection.Tables)
        {
          if (data.Enabled)
            AddDataSource(Menu.Items, data);
        }
      }
      
      foreach (DataSourceBase data in Obj.Report.Dictionary.DataSources)
      {
        if (data.Enabled)
          AddDataSource(Menu.Items, data);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSourceSmartTag"/> class with default settings.
    /// </summary>
    /// <param name="obj">Report object that owns this smart tag.</param>
    public DataSourceSmartTag(ComponentBase obj) : base(obj)
    {
    }
  }
}
