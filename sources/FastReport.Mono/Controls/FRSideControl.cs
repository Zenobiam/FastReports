using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;
using FastReport.Utils;

namespace FastReport.Controls
{
  internal class FRSideControl : Panel
  {
    private FRTabControl FTopPanel;
    private FRTabControl FBottomPanel;
    private Splitter FSplitter;

    public FRTabControl TopPanel
    {
      get { return FTopPanel; }
    }

    public FRTabControl BottomPanel
    {
      get { return FBottomPanel; }
    }

    public UIStyle Style
    {
      get { return FTopPanel.Style; }
      set
      {
        FTopPanel.Style = value;
        FBottomPanel.Style = value;
        FSplitter.BackColor = UIStyleUtils.GetColorTable(value).ControlBackColor;
      }
    }

    public void AddToTopPanel(PageControlPage page)
    {
      page.Dock = DockStyle.Fill;
      TopPanel.Tabs.Add(page);
    }

    public void AddToBottomPanel(PageControlPage page)
    {
      page.Dock = DockStyle.Fill;
      BottomPanel.Tabs.Add(page);
    }

    public void RefreshLayout()
    {
      TopPanel.Refresh();
      TopPanel.SelectedTabIndex = 0;
      BottomPanel.Refresh();
      BottomPanel.SelectedTabIndex = 0;
    }

    public void SaveState(XmlItem root)
    {
      XmlItem xi = root.FindItem("Designer").FindItem("SideControl");
      xi.SetProp("Width", Width.ToString());
      xi.SetProp("TopPanelHeight", TopPanel.Height.ToString());
    }

    public void RestoreState(XmlItem root)
    {
      XmlItem xi = root.FindItem("Designer").FindItem("SideControl");
      string width = xi.GetProp("Width");
      if (width != "")
        Width = int.Parse(width);
      string topPanelHeight = xi.GetProp("TopPanelHeight");
      if (topPanelHeight != "")
        TopPanel.Height = int.Parse(topPanelHeight);
    }

    public FRSideControl()
    {
      FTopPanel = new FRTabControl();
      FTopPanel.Dock = DockStyle.Top;
      FTopPanel.CloseButtons = false;
      FBottomPanel = new FRTabControl();
      FBottomPanel.Dock = DockStyle.Fill;
      FBottomPanel.CloseButtons = false;
      FSplitter = new Splitter();
      FSplitter.Dock = DockStyle.Top;
      FSplitter.MinSize = 0;
      FSplitter.MinExtra = 0;
      Controls.AddRange(new Control[] { FBottomPanel, FSplitter, FTopPanel });
    }
  }
}
