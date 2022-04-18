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
  internal class FRTabControl : UserControl
  {
    private FRTabStrip FTabStrip;
    private PageControl FPageControl;
    private Panel FCaptionPanel;
    private bool FShowTabs;
    private bool FCloseButtons;

    public event EventHandler SelectedTabChanged;
    public event EventHandler TabClosed;
    public event TabMovedEventHandler TabMoved;

    public TabOrientation TabOrientation
    {
      get { return FTabStrip.TabOrientation; }
      set
      {
        FTabStrip.TabOrientation = value;
        if (value == TabOrientation.Top)
        {
          FTabStrip.Dock = DockStyle.Top;
        }
        else if (value == TabOrientation.Bottom)
        {
          FTabStrip.Dock = DockStyle.Bottom;
        }
      }
    }
    
    public ControlCollection Tabs
    {
      get { return FPageControl == null ? null : FPageControl.Pages; }
    }

    public bool ShowCaption
    {
      get { return FCaptionPanel.Visible; }
      set { FCaptionPanel.Visible = value && TabOrientation == TabOrientation.Bottom; }
    }

    public bool ShowTabs
    {
      get { return FShowTabs; }
      set
      {
        FShowTabs = value;
        UpdateTabStripVisible();
      }
    }

    public bool CloseButtons
    {
      get { return FCloseButtons; }
      set { FCloseButtons = value; }
    }

    public UIStyle Style
    {
      get { return FTabStrip.Style; }
      set 
      { 
        FTabStrip.Style = value;
        FCaptionPanel.BackColor = UIStyleUtils.GetColorTable(value).ToolWindowCaptionColor;
      }
    }
    
    public PageControlPage SelectedTab
    {
      get { return FPageControl.ActivePage as PageControlPage; }
      set { SelectedTabIndex = GetTabIndex(value); }
    }

    public int SelectedTabIndex
    {
      get { return FTabStrip.SelectedTabIndex; }
      set { FTabStrip.SelectedTabIndex = value; }
    }

    private int GetTabIndex(PageControlPage page)
    {
      for (int i = 0; i < FTabStrip.Tabs.Count; i++)
      {
        if (FTabStrip.Tabs[i].Tag == page)
          return i;
      }
      return -1;
    }
    
    private void UpdateTabStripVisible()
    {
      if(FTabStrip != null)
        FTabStrip.Visible = FShowTabs && FTabStrip.Tabs.Count > 1;
    }

    private void FPageControl_ControlAdded(object sender, ControlEventArgs e)
    {
      PageControlPage page = e.Control as PageControlPage;
//      Tab tab = new Tab(page.Text, page.Image);  // Version before unfication
      Tab tab = new Tab(page.Text);
      tab.Tag = page;
      tab.CloseButton = FCloseButtons;
      FTabStrip.Tabs.Add(tab);
      UpdateTabStripVisible();
    }

    private void FPageControl_ControlRemoved(object sender, ControlEventArgs e)
    {
      PageControlPage page = e.Control as PageControlPage;
      int tabIndex = GetTabIndex(page);
      if (tabIndex != -1)
      {
        FTabStrip.Tabs.RemoveAt(tabIndex);
        SelectedTabIndex = SelectedTabIndex;
        UpdateTabStripVisible();
      }
    }

    private void FTabStrip_TabChanged(object sender, EventArgs e)
    {
      int idx = FTabStrip.SelectedTabIndex;
      if (idx >= 0)
            {
                FPageControl.ActivePageIndex = idx;
                // update the title
                Refresh();
            }

            if (SelectedTabChanged != null)
        SelectedTabChanged(this, e);
    }
    
    private void FTabStrip_TabClosed(object sender, EventArgs e)
    {
      if (TabClosed != null)
        TabClosed(this, e);
    }

    private void FTabStrip_TabMoved(object sender, TabMovedEventArgs e)
    {
      if (TabMoved != null)
        TabMoved(this, e);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        FTabStrip.Dispose();
        FTabStrip = null;
        FPageControl.Dispose();
        FPageControl = null;
        FCaptionPanel.Dispose();
        FCaptionPanel = null;
      }
      base.Dispose(disposing);
    }

    private void CaptionPanel_Paint(object sender, PaintEventArgs e)
    {
      if (ShowCaption && FTabStrip != null && FTabStrip.SelectedTab != null)
        TextRenderer.DrawText(e.Graphics, FTabStrip.SelectedTab.Text, Font, new Point(1, 2), ForeColor);
    }

    public FRTabControl()
    {
      FCaptionPanel = new Panel();
      FCaptionPanel.Dock = DockStyle.Top;
      FCaptionPanel.Height = 18;
      FCaptionPanel.BackColor = Color.FromArgb(204, 199, 186);
      FCaptionPanel.Paint += CaptionPanel_Paint;

      FTabStrip = new FRTabStrip();
      FTabStrip.Dock = DockStyle.Bottom;
      FTabStrip.TabOrientation = TabOrientation.Bottom;
      FTabStrip.AllowTabReorder = false;
      FTabStrip.Visible = false;
      FTabStrip.SelectedTabChanged += new EventHandler(FTabStrip_TabChanged);
      FTabStrip.TabClosed += new EventHandler(FTabStrip_TabClosed);
      FTabStrip.TabMoved += new TabMovedEventHandler(FTabStrip_TabMoved);

      FPageControl = new PageControl();
      FPageControl.Dock = DockStyle.Fill;
      FPageControl.ControlAdded += new ControlEventHandler(FPageControl_ControlAdded);
      FPageControl.ControlRemoved += new ControlEventHandler(FPageControl_ControlRemoved);

      Controls.Add(FPageControl);
      Controls.Add(FTabStrip);
      Controls.Add(FCaptionPanel);

      // caption area backcolor
      TabOrientation = TabOrientation.Bottom;
      ShowCaption = true;
      ShowTabs = true;
      CloseButtons = true;
    }
  }
}
