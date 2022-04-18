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
  internal class Tab
  {
    private FRTabStrip FTabControl;
    private string FText;
    private object FTag;
    private Image FImage;
    private bool FVisible;
    private bool FCloseButton;
    private bool FAllowMove;
    private Rectangle FDisplayRectangle;
    
    public FRTabStrip TabControl
    {
      get { return FTabControl; }
      set { FTabControl = value; }
    }
    
    public string Text
    {
      get { return FText; }
      set { FText = value; }
    }
    
    public object Tag
    {
      get { return FTag; }
      set { FTag = value; }
    }
    
    public Image Image
    {
      get { return FImage; }
      set { FImage = value; }
    }

    public bool Visible
    {
      get { return FVisible; }
      set { FVisible = value; }
    }
    
    public bool CloseButton
    {
      get { return FCloseButton; }
      set { FCloseButton = value; }
    }

    public bool AllowMove
    {
      get { return FAllowMove; }
      set { FAllowMove = value; }
    }

    public Rectangle DisplayRectangle
    {
      get { return FDisplayRectangle; }
      set { FDisplayRectangle = value; }
    }
    
    public int CalcWidth()
    {
      if (!FVisible)
        return 0;
      int result = 2;
      if (FImage != null)
        result += 16;
      result += TextRenderer.MeasureText(FText, FTabControl.Font).Width;
      if (FCloseButton)
        result += 16;
      return result;
    }

    public Tab()
    {
      FAllowMove = true;
      FCloseButton = true;
      FVisible = true;
    }

    public Tab(string text) : this()
    {
      FText = text;
    }

    public Tab(string text, Image image) : this(text)
    {
      FImage = image;
    }
  }

  internal class TabCollection : CollectionBase
  {
    private FRTabStrip FOwner;
    
    public Tab this[int index]  
    {
      get { return List[index] as Tab; }
      set { List[index] = value; }
    }

    public void AddRange(Tab[] range)
    {
      foreach (Tab t in range)
      {
        Add(t);
      }
    }
    
    public int Add(Tab value)
    {
      return List.Add(value);
    }

    public void Insert(int index, Tab value)  
    {
      List.Insert(index, value);
    }
    
    public void Remove(Tab value)  
    {
      List.Remove(value);
    }

    public int IndexOf(Tab value)
    {
      return List.IndexOf(value);
    }

    public bool Contains(Tab value)  
    {
      return List.Contains(value);
    }

    protected override void OnInsert(int index, Object value)
    {
      if (FOwner != null)
      {
        (value as Tab).TabControl = FOwner;
        FOwner.UpdateSelectedIndex(index, true);
      }
    }

    protected override void OnRemove(int index, object value)
    {
      if (FOwner != null)
        FOwner.UpdateSelectedIndex(index, false);
    }

    public TabCollection(FRTabStrip owner)
    {
      FOwner = owner;
    }
  }

  internal delegate void TabMovedEventHandler(object sender, TabMovedEventArgs e);
  
  internal class TabMovedEventArgs
  {
    public int OldIndex;
    public int NewIndex;
    
    public TabMovedEventArgs(int oldIndex, int newIndex)
    {
      OldIndex = oldIndex;
      NewIndex = newIndex;
    }
  }

  internal enum TabOrientation
  {
    Top,
    Bottom
  }

  internal class FRTabStrip : Control
  {
    private TabCollection FTabs;
    private int FSelectedTabIndex;
    private ToolStripRenderer FRenderer;
    private bool FHighlightLeftBtn;
    private bool FHighlightRightBtn;
    private int FHighlightTabCloseButton;
    private int FOffset;
    private int FSaveTabIndex;
    private bool FMovingTab;
    private bool FTabMoved;
    private int FMouseOffset;
    private bool FButtons;
    private bool FAllowTabReorder;
    private TabOrientation FTabOrientation;
    private UIStyle FStyle;
    
    public TabCollection Tabs
    {
      get { return FTabs; }
    }
    
    public int SelectedTabIndex
    {
      get { return FSelectedTabIndex; }
      set 
      {
        if (value < 0)
          value = -1;
        if (value > FTabs.Count - 1)
          value = FTabs.Count - 1;
        FSelectedTabIndex = value;
        EnsureTabVisible();  
        Refresh();
        if (SelectedTabChanged != null)
          SelectedTabChanged(this, null);
      }
    }

    public Tab SelectedTab
    {
      get 
      {
        if (FSelectedTabIndex == -1 || FSelectedTabIndex >= FTabs.Count)
          return null;
        return FTabs[FSelectedTabIndex];  
      }
      set
      {
        SelectedTabIndex = FTabs.IndexOf(value);
      }
    }
    
    public bool AllowTabReorder
    {
      get { return FAllowTabReorder; }
      set { FAllowTabReorder = value; }
    }

    public ToolStripRenderer Renderer
    {
      get { return FRenderer; }
      set { FRenderer = value; }
    }

    public TabOrientation TabOrientation
    {
      get { return FTabOrientation; }
      set 
      { 
        FTabOrientation = value;
        Refresh();
      }
    }

    public UIStyle Style
    {
      get { return FStyle; }
      set
      {
        FStyle = value;
        Refresh();
      }
    }

    public event EventHandler SelectedTabChanged;
    public event EventHandler TabClosed;
    public event TabMovedEventHandler TabMoved;

    private void EnsureTabVisible()
    {
      Tab tab = SelectedTab;
      if (tab == null)
        return;
      int add = FButtons ? 40 : 0;
      if (tab.DisplayRectangle.Left < 0)
        FOffset += -tab.DisplayRectangle.Left + 4;
      else if (tab.DisplayRectangle.Right > Width - add)
        FOffset -= tab.DisplayRectangle.Right - (Width - add) + 4;
      if (FOffset > 0)
        FOffset = 0;  
    }

    private void CalcItems()
    {
      int x = 4 + FOffset;
      foreach (Tab tab in FTabs)
      {
        if (!tab.Visible)
          continue;
        int width = tab.CalcWidth();
        if (TabOrientation == TabOrientation.Top)
          tab.DisplayRectangle = new Rectangle(x, 3, width, Height - 3);
        else
          tab.DisplayRectangle = new Rectangle(x, 0, width, Height - 4);
        x += width;
      }
    }
    
    private void HighlightButton(Graphics g, int x, int y)
    {
      if (!FButtons) 
        return;
      using (Brush brush = new SolidBrush(Color.FromArgb(193, 210, 238)))
      using (Pen pen = new Pen(Color.FromArgb(49, 106, 197)))
      {
        g.FillRectangle(brush, x, y, 14, 14);
        g.DrawRectangle(pen, x, y, 14, 14);
      }
    }

    private void ScrollLeft()
    {
      if (FOffset < 0)
      {
        FOffset += SelectedTab.DisplayRectangle.Width;
        if (FOffset > 0)
          FOffset = 0;
      }
      Refresh();
    }

    private void ScrollRight()
    {
      int tabsWidth = 0;
      for (int i = 0; i < FTabs.Count - 1; i++)
        tabsWidth += FTabs[i].DisplayRectangle.Width;
      if (-FOffset < tabsWidth)
        FOffset -= SelectedTab.DisplayRectangle.Width;
      Refresh();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      CalcItems();
      Graphics g = e.Graphics;
      Pen pen = new Pen(UIStyleUtils.GetColorTable(Style).TabStripBorder);
      LinearGradientBrush brush = new LinearGradientBrush(DisplayRectangle,
        UIStyleUtils.GetColorTable(Style).TabStripGradientBegin,
        UIStyleUtils.GetColorTable(Style).TabStripGradientEnd,
        LinearGradientMode.Vertical);
      g.FillRectangle(brush, DisplayRectangle);

      if (TabOrientation == TabOrientation.Top)
        g.DrawLine(pen, 0, Height - 1, Width - 1, Height - 1);
      else
        g.DrawLine(pen, 0, 0, Width - 1, 0);

      int tabsWidth = 0;
      for (int i = 0; i < FTabs.Count; i++)
      {
        Tab tab = FTabs[i];
        if (!tab.Visible)
          continue;
        
        Rectangle rect = tab.DisplayRectangle;
        tabsWidth += rect.Width;
        int x = rect.Left;

        if (TabOrientation == TabOrientation.Top)
        {
          if (i == FSelectedTabIndex)
          {
            g.FillRectangle(SystemBrushes.Window, rect.Left + 1, rect.Top + 1, rect.Width - 1, rect.Height - 1);
            g.DrawLine(pen, rect.Left, rect.Bottom, rect.Left, rect.Top + 2);
            g.DrawLine(pen, rect.Left, rect.Top + 2, rect.Left + 2, rect.Top);
            g.DrawLine(pen, rect.Left + 2, rect.Top, rect.Right - 2, rect.Top);
            g.DrawLine(pen, rect.Right - 2, rect.Top, rect.Right, rect.Top + 2);
            g.DrawLine(pen, rect.Right, rect.Top + 2, rect.Right, rect.Bottom);
          }
          else if (i < FTabs.Count - 1)
          {
            g.DrawLine(pen, rect.Right, rect.Top + 2, rect.Right, rect.Bottom - 4);
          }

          if (tab.Image != null)
          {
            g.DrawImage(tab.Image, rect.Left + 3, rect.Top + 2);
            x += 16;
          }

          TextRenderer.DrawText(g, tab.Text, Font, new Point(x + 2, rect.Top + 3),
            i == FSelectedTabIndex ? SystemColors.WindowText : SystemColors.ControlDarkDark);

          if (tab.CloseButton)
            g.DrawImage(Res.GetImage(i == FHighlightTabCloseButton ? 175 : 177), rect.Right - 16, rect.Top + 4);
        }
        else
        {
          if (i == FSelectedTabIndex)
          {
            g.FillRectangle(SystemBrushes.Window, rect.Left + 1, rect.Top, rect.Width - 1, rect.Height);
            g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom - 2);
            g.DrawLine(pen, rect.Left, rect.Bottom - 2, rect.Left + 2, rect.Bottom);
            g.DrawLine(pen, rect.Left + 2, rect.Bottom, rect.Right - 2, rect.Bottom);
            g.DrawLine(pen, rect.Right - 2, rect.Bottom, rect.Right, rect.Bottom - 2);
            g.DrawLine(pen, rect.Right, rect.Bottom - 2, rect.Right, rect.Top);
          }
          else if (i < FTabs.Count - 1)
          {
            g.DrawLine(pen, rect.Right, rect.Top + 3, rect.Right, rect.Bottom - 3);
          }

          if (tab.Image != null)
          {
            g.DrawImage(tab.Image, rect.Left + 3, rect.Top + 3);
            x += 16;
          }

          TextRenderer.DrawText(g, tab.Text, Font, new Point(x + 2, rect.Top + 3),
            i == FSelectedTabIndex ? SystemColors.WindowText : SystemColors.ControlDarkDark);

          if (tab.CloseButton)
            g.DrawImage(Res.GetImage(i == FHighlightTabCloseButton ? 175 : 177), rect.Right - 16, rect.Top + 4);
        }
      }

      FButtons = tabsWidth > Width;
      if (FButtons)
      {
        g.FillRectangle(brush, Width - 40, 1, 40, Height - 2);
        if (FHighlightLeftBtn)
          HighlightButton(g, Width - 38, 4);
        g.DrawImage(Res.GetImage(186), Width - 38, 4);
        if (FHighlightRightBtn)
          HighlightButton(g, Width - 21, 4);
        g.DrawImage(Res.GetImage(187), Width - 22, 4);
      }
      
      pen.Dispose();
      brush.Dispose();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      FMovingTab = false;
      if (FButtons && e.X > Width - 38 && e.X < Width - 24)
      {
        ScrollLeft();
      }
      else if (FButtons && e.X > Width - 20 && e.X < Width - 6)
      {
        ScrollRight();
      }
      else
      {
        Tab tab = TabAt(e.X);
        if (tab != null)
        {
          SelectedTab = tab;
          if (e.X > tab.DisplayRectangle.Right - 16 && e.X < tab.DisplayRectangle.Right)
          {
            CloseTab(tab);
            return;
          }
          FMovingTab = true;
        }  
      }
      FSaveTabIndex = FSelectedTabIndex;
      FTabMoved = false;
      FMouseOffset = 0;
    }

    private void CloseTab(Tab tab)
    {
      if (TabClosed != null)
        TabClosed(tab, null);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (FButtons && e.Button == MouseButtons.None)
      {
        bool b = e.X > Width - 38 && e.X < Width - 24;
        if (b != FHighlightLeftBtn)
        {
          FHighlightLeftBtn = b;
          Refresh();
        }
        b = e.X > Width - 20 && e.X < Width - 6;
        if (FHighlightRightBtn != b)
        {
          FHighlightRightBtn = b;
          Refresh();
        }

        int tabIndex = -1;
        Tab tab = TabAt(e.X);
        if (tab != null)
        {
          if (e.X > tab.DisplayRectangle.Right - 16 && e.X < tab.DisplayRectangle.Right)
            tabIndex = Tabs.IndexOf(tab);
        }
        if (FHighlightTabCloseButton != tabIndex)
        {
          FHighlightTabCloseButton = tabIndex;
          Refresh();
        }
      }
      else if (e.Button == MouseButtons.Left && FMovingTab && FAllowTabReorder)
      {
        Tab oldTab = SelectedTab;
        Tab newTab = TabAt(e.X + FMouseOffset);
        if (oldTab != null && newTab != null && newTab != oldTab && newTab.AllowMove && oldTab.AllowMove)
        {
          int oldIndex = FTabs.IndexOf(oldTab);
          int newIndex = FTabs.IndexOf(newTab);
          FTabs.Remove(oldTab);
          FTabs.Insert(newIndex, oldTab);
          Refresh();
          SelectedTabIndex = newIndex;
          FTabMoved = true;
          if (oldIndex < newIndex)
            FMouseOffset = oldTab.DisplayRectangle.Left - e.X;
          else
            FMouseOffset = e.X - oldTab.DisplayRectangle.Right;
        }
      }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      if (FTabMoved)
      {
        if (TabMoved != null)
          TabMoved(this, new TabMovedEventArgs(FSaveTabIndex, FSelectedTabIndex));
      }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      if (FButtons)
      {
        FHighlightLeftBtn = false;
        FHighlightRightBtn = false;
        FHighlightTabCloseButton = -1;
        Refresh();
      }
    }

    internal void UpdateSelectedIndex(int index, bool insert)
    {
      if (insert && FSelectedTabIndex < index)
        FSelectedTabIndex++;
      if (!insert && FSelectedTabIndex > index)
        FSelectedTabIndex--;
    }

    public Tab TabAt(int x)
    {
      foreach (Tab tab in FTabs)
      {
        if (!tab.Visible)
          continue;
        if (x >= tab.DisplayRectangle.Left && x <= tab.DisplayRectangle.Right)
          return tab;
      }
      return null;
    }
    
    public FRTabStrip()
    {
      FTabs = new TabCollection(this);
      FButtons = true;
      FAllowTabReorder = true;
      FHighlightTabCloseButton = -1;
      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
      Style = UIStyle.VisualStudio2005;
      Height = 24;
    }
  }
}
