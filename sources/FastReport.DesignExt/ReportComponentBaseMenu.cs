using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Design;
using FastReport.Utils;
using FastReport.Forms;

namespace FastReport
{
  /// <summary>
  /// The class introduces some menu items specific 
  /// to the <b>ReportComponentBase</b>.
  /// </summary>
  public class ReportComponentBaseMenu : ComponentBaseMenu
  {
    #region Fields

    /// <summary>
    /// The "Can Grow" menu item.
    /// </summary>
    public ContextMenuItem miCanGrow;

    /// <summary>
    /// The "Can Shrink" menu item.
    /// </summary>
    public ContextMenuItem miCanShrink;

    /// <summary>
    /// The "Grow to Bottom" menu item.
    /// </summary>
    public ContextMenuItem miGrowToBottom;

    /// <summary>
    /// The "Hyperlink" menu item.
    /// </summary>
    public ContextMenuItem miHyperlink;

    /// <summary>
    /// The "Style" menu item.
    /// </summary>
    public ContextMenuItem miStyle;

    #endregion Fields

    #region Private Methods

    private void miCanGrow_Click(object sender, EventArgs e)
    {
      Designer.SelectedReportComponents.SetCanGrow(miCanGrow.Checked);
    }

    private void miCanShrink_Click(object sender, EventArgs e)
    {
      Designer.SelectedReportComponents.SetCanShrink(miCanShrink.Checked);
    }

    private void miGrowToBottom_Click(object sender, EventArgs e)
    {
      Designer.SelectedReportComponents.SetGrowToBottom(miGrowToBottom.Checked);
    }

    private void miHyperlink_Click(object sender, EventArgs e)
    {
      Designer.SelectedReportComponents.InvokeHyperlinkEditor();
    }

    private void miStyle_Click(object sender, EventArgs e)
    {
        ContextMenuItem subItem = sender as ContextMenuItem;
        ReportComponentBase first = Designer.SelectedReportComponents.First as ReportComponentBase;
        if (first.Style == subItem.Text)
        {
            Designer.SelectedReportComponents.SetStyle("");
        }
        else
        {
            Designer.SelectedReportComponents.SetStyle(subItem.Text);
        }
    }

    #endregion Private Methods

    /// <summary>
    /// Initializes a new instance of the <b>ReportComponentBaseMenu</b> 
    /// class with default settings. 
    /// </summary>
    /// <param name="designer">The reference to a report designer.</param>
    public ReportComponentBaseMenu(Designer designer) : base(designer)
    {
      miCanGrow = CreateMenuItem(Res.Get("ComponentMenu,ReportComponent,CanGrow"), new EventHandler(miCanGrow_Click));
      miCanGrow.CheckOnClick = true;
      miCanGrow.BeginGroup = true;
      miCanShrink = CreateMenuItem(Res.Get("ComponentMenu,ReportComponent,CanShrink"), new EventHandler(miCanShrink_Click));
      miCanShrink.CheckOnClick = true;
      miGrowToBottom = CreateMenuItem(Res.Get("ComponentMenu,ReportComponent,GrowToBottom"), new EventHandler(miGrowToBottom_Click));
      miGrowToBottom.CheckOnClick = true;
      miHyperlink = CreateMenuItem(Res.GetImage(167), Res.Get("ComponentMenu,ReportComponent,Hyperlink"), new EventHandler(miHyperlink_Click));
      miStyle = CreateMenuItem(Res.Get("ComponentMenu,ReportComponent,Style"));

      int insertPos = Items.IndexOf(miEdit) + 1;
      Items.Insert(insertPos, miStyle);
      Items.Insert(insertPos + 1, miHyperlink);
      insertPos = Items.IndexOf(miCut);
      Items.Insert(insertPos, miCanGrow);
      Items.Insert(insertPos + 1, miCanShrink);
      Items.Insert(insertPos + 2, miGrowToBottom);

      if (!miEdit.Visible)
        miStyle.BeginGroup = true;
      
      bool enabled = Designer.SelectedReportComponents.Enabled;
      miCanGrow.Enabled = enabled;
      miCanShrink.Enabled = enabled;
      miGrowToBottom.Enabled = enabled;

      miStyle.Enabled = enabled && Designer.Report.Styles.Count > 0;
      if (miStyle.Enabled)
      {
          ReportComponentBase first = Designer.SelectedReportComponents.First;
          foreach (Style style in Designer.Report.Styles)
          {
              ContextMenuItem subItem = CreateMenuItem(style.Name, miStyle_Click);
              if (first.Style == style.Name)
              {
                  subItem.Checked = true;
              }
              miStyle.DropDownItems.Add(subItem);
          }
      }

      if (enabled)
      {
        ReportComponentBase first = Designer.SelectedReportComponents.First;
        miCanGrow.Checked = first.CanGrow;
        miCanShrink.Checked = first.CanShrink;
        miGrowToBottom.Checked = first.GrowToBottom;
      }  
    }
  }
}
