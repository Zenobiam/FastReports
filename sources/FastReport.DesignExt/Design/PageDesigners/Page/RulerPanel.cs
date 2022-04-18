using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Controls;
#if !MONO
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design.PageDesigners.Page
{
  internal class RulerPanel : SplitContainer
  {
    #region Fields
    private HorzRuler horzRuler;
    private VertRuler vertRuler;
    private Button btnSwitchView;
    private ControlContainer controlContainer;
    private BandStructure structure;
    private ReportPageDesigner pageDesigner;
    private ReportWorkspace workspace;
    private ReportPage page;
    #endregion

    #region Properties
    public HorzRuler HorzRuler
    {
      get { return horzRuler; }
    }

    public VertRuler VertRuler
    {
      get { return vertRuler; }
    }

    public BandStructure Structure
    {
      get { return structure; }
    }

    public ReportWorkspace Workspace
    {
      get { return workspace; }
    }

    private Designer Designer
    {
      get { return pageDesigner.Designer; }
    }
    #endregion

    #region Private Methods
    private void AdjustOffset()
    {
      if(Config.RightToLeft)
      {
        horzRuler.Offset = workspace.Left;
      }
      else
      {
        horzRuler.Offset = workspace.Left + DpiHelper.ConvertUnits(24);        
      }
      horzRuler.Refresh();
      vertRuler.Offset = workspace.Top;
      vertRuler.Refresh();
      structure.Offset = workspace.Top;
      structure.Refresh();
    }

    private void Workspace_LocationChanged(object sender, EventArgs e)
    {
      AdjustOffset();
    }

    private void btnSwitchView_Click(object sender, EventArgs e)
    {
      ReportWorkspace.ClassicView = !ReportWorkspace.ClassicView;
      pageDesigner.UpdateContent();
    }

    private void RulerPanel_SplitterMoved(object sender, SplitterEventArgs e)
    {
        if (Config.RightToLeft)
        {
            if(workspace.Page != null)
                workspace.Refresh(); 
        }
        workspace.Focus();
    }
    #endregion

    public override void Refresh()
    {
      base.Refresh();
      workspace.Refresh();
      horzRuler.Refresh();
      vertRuler.Refresh();
      structure.Refresh();
    }

    public void SetStructureVisible(bool visible)
    {
      structure.Visible = visible;
      Panel1Collapsed = !visible;
    }

    public void UpdateUIStyle()
    {
#if !MONO
      controlContainer.ColorSchemeStyle = UIStyleUtils.GetDotNetBarStyle(Designer.UIStyle);
      controlContainer.Office2007ColorTable = UIStyleUtils.GetOffice2007ColorScheme(Designer.UIStyle);
      Color color = UIStyleUtils.GetControlColor(Designer.UIStyle);

      structure.BackColor = color;
      horzRuler.BackColor = color;
      vertRuler.BackColor = color;
      workspace.BackColor = color;
      BackColor = color;
#else
      Color color = UIStyleUtils.GetColorTable(Designer.UIStyle).ControlBackColor;
      structure.BackColor = color;
      horzRuler.BackColor = color;
      vertRuler.BackColor = color;
      BackColor = color;
      color = UIStyleUtils.GetColorTable(Designer.UIStyle).WorkspaceBackColor;
      controlContainer.BackColor = color;
      workspace.BackColor = color;
#endif
   }

        public void UpdateDpiDependencies()
        {
            btnSwitchView.Image = Res.GetImage(81);
            horzRuler.Height = DpiHelper.ConvertUnits(24);
            vertRuler.Width = DpiHelper.ConvertUnits(24);
            btnSwitchView.Location = DpiHelper.ConvertUnits(new Point(4, 4));
            btnSwitchView.Size = DpiHelper.ConvertUnits(new Size(16, 16));
            SplitterDistance = DpiHelper.ConvertUnits(120);
            horzRuler.Update();
            vertRuler.Update();
            structure.ReinitDpiSize();
            workspace.UpdateDpiDependencies();
            AdjustOffset();
        }

    public RulerPanel(ReportPageDesigner pd) : base()
    {
      pageDesigner = pd;
      page = pd.Page as ReportPage;
      workspace = new ReportWorkspace(pageDesigner);
      workspace.LocationChanged += new EventHandler(Workspace_LocationChanged);

      horzRuler = new HorzRuler(pd);
      horzRuler.Height = DpiHelper.ConvertUnits(24);
      horzRuler.Dock = DockStyle.Top;
      vertRuler = new VertRuler(pd);
      vertRuler.Width = DpiHelper.ConvertUnits(24);

      btnSwitchView = new Button();
      btnSwitchView.Location = DpiHelper.ConvertUnits(new Point(4, 4));
      btnSwitchView.Size = DpiHelper.ConvertUnits(new Size(16, 16));

      // apply Right to Left layout if needed
      vertRuler.Dock = Config.RightToLeft ? DockStyle.Right : DockStyle.Left;
      if (Config.RightToLeft)
      {
          btnSwitchView.Dock = DockStyle.Right;
          horzRuler.Left -= btnSwitchView.Width;
      }

      btnSwitchView.FlatStyle = FlatStyle.Flat;
      btnSwitchView.FlatAppearance.BorderColor = SystemColors.ButtonFace;
      btnSwitchView.FlatAppearance.BorderSize = 0;
      btnSwitchView.Cursor = Cursors.Hand;
      btnSwitchView.Image = Res.GetImage(81);
      btnSwitchView.Click += new EventHandler(btnSwitchView_Click);
      horzRuler.Controls.Add(btnSwitchView);

      structure = new BandStructure(pageDesigner);
      structure.Dock = DockStyle.Fill;

      controlContainer = new ControlContainer(this);
      controlContainer.Dock = DockStyle.Fill;
            
      SplitterDistance = DpiHelper.ConvertUnits(120);
      Panel1.Controls.Add(structure);
      Panel2.Controls.AddRange(new Control[] { controlContainer, vertRuler, horzRuler });
      Panel1MinSize = 20;
      FixedPanel = FixedPanel.Panel1;
      SplitterMoved += new SplitterEventHandler(RulerPanel_SplitterMoved);

      AdjustOffset();
    }


#if !MONO
    private class ControlContainer : PanelX
    {
	    private RulerPanel rulerPanel;
	    private float oldScale;
	    private Point scrollPos;
	    private Point mousePositionOnContent;
	    private bool allowPaint = true;
	    private const int WM_PAINT = 0x000F;

	    private void UpdateContentLocation()
	    {
		    rulerPanel.Workspace.Location = new Point(AutoScrollPosition.X, AutoScrollPosition.Y);
	    }

	    private void content_Resize(object sender, EventArgs e)
	    {
		    Size maxSize = new Size(rulerPanel.Workspace.Width + 10, rulerPanel.Workspace.Height + 10);
		    if (maxSize.Width > Width)
			    maxSize.Height += SystemInformation.HorizontalScrollBarHeight;
		    if (maxSize.Height > Height)
			    maxSize.Width += SystemInformation.VerticalScrollBarWidth;
		    AutoScrollMinSize = maxSize;
		    AutoScrollPosition = AutoScrollPosition;
		    UpdateContentLocation();
		    Refresh();
	    }

	    protected override void OnScroll(ScrollEventArgs se)
	    {
		    base.OnScroll(se);
		    UpdateContentLocation();
	    }

	    protected override void OnPaint(PaintEventArgs e)
	    {
		    base.OnPaint(e);

		    // draw shadow around page
		    ShadowPaintInfo pi = new ShadowPaintInfo();
		    pi.Graphics = e.Graphics;
		    pi.Rectangle = rulerPanel.Workspace.Bounds;
		    pi.Size = 6;

            // draw shadow Right to Left if needed
            if (Config.RightToLeft)
            {
                ShadowPainter.Paint2RightToLeft(pi);
            }
            else
            {
		        ShadowPainter.Paint2(pi);
            }
        }

	    protected override void WndProc(ref Message m)
	    {
		    if ((m.Msg != WM_PAINT) || (allowPaint && m.Msg == WM_PAINT))
		    {
			    base.WndProc(ref m);
		    }
	    }

	    public ControlContainer(RulerPanel rulerPanel)
	    {
            this.rulerPanel = rulerPanel;
		    AutoScroll = true;
		    FastScrolling = false;
            Controls.Add(this.rulerPanel.Workspace);
            this.rulerPanel.Workspace.Resize += new EventHandler(content_Resize);
            this.rulerPanel.Workspace.BeforeZoom += new EventHandler(content_BeforeZoom);
            this.rulerPanel.Workspace.AfterZoom += new EventHandler(content_AfterZoom);
	    }

	    private void content_BeforeZoom(object sender, EventArgs e)
	    {
		    // avoid unnecessary redraws while zooming
		    allowPaint = false;
		    rulerPanel.VertRuler.allowPaint = false;
		    rulerPanel.HorzRuler.allowPaint = false;
		    rulerPanel.Structure.allowPaint = false;
		    VScrollBar.AllowPaint = false;
		    HScrollBar.AllowPaint = false;

		    oldScale = ReportWorkspace.Scale;
		    scrollPos = rulerPanel.Workspace.Location;

		    Point mousePositionOnScreen = PointToClient(Cursor.Position);
		    if (mousePositionOnScreen.X < 0 ||
			    mousePositionOnScreen.X > Width ||
			    mousePositionOnScreen.Y < 0 ||
			    mousePositionOnScreen.Y > Height)
			    mousePositionOnScreen = new Point(Width / 2, Height / 2);

		    mousePositionOnContent = new Point(
			    (int)((mousePositionOnScreen.X - scrollPos.X) / oldScale),
			    (int)((mousePositionOnScreen.Y - scrollPos.Y) / oldScale));
	    }

	    private void content_AfterZoom(object sender, EventArgs e)
	    {
		    float zoomchange = ReportWorkspace.Scale - oldScale;
		    float offsetX = mousePositionOnContent.X * zoomchange;
		    float offsetY = mousePositionOnContent.Y * zoomchange;
		    scrollPos.X -= (int)offsetX;
		    scrollPos.Y -= (int)offsetY;
		    AutoScrollPosition = scrollPos;

		    // redraw
		    rulerPanel.VertRuler.allowPaint = true;
		    rulerPanel.HorzRuler.allowPaint = true;
		    rulerPanel.Structure.allowPaint = true;
		    VScrollBar.AllowPaint = true;
		    HScrollBar.AllowPaint = true;

            if (!Config.RightToLeft)
            {
		        UpdateContentLocation();
            }

		    allowPaint = true;
		    Refresh();
	    }
    }
#else
    private class ControlContainer : FRScrollablePanel
    {
      private Control FContent;

      private void UpdateContentLocation()
      {
        FContent.Location = new Point(AutoScrollPosition.X, AutoScrollPosition.Y);
      }

      private void content_Resize(object sender, EventArgs e)
      {
        Size maxSize = new Size(FContent.Width + 10, FContent.Height + 10);
        if (maxSize.Width > Width)
          maxSize.Height += SystemInformation.HorizontalScrollBarHeight;
        if (maxSize.Height > Height)
          maxSize.Width += SystemInformation.VerticalScrollBarWidth;
        AutoScrollMinSize = maxSize;
        AutoScrollPosition = AutoScrollPosition;
        UpdateContentLocation();
        Refresh();
      }
      
      protected override void OnScroll(ScrollEventArgs se)
      {
        base.OnScroll(se);
        UpdateContentLocation();
      }

      public ControlContainer(RulerPanel rulerPanel)
      {
        FContent = rulerPanel.Workspace;
        AutoScroll = true;
        Controls.Add(FContent);
        FContent.Resize += content_Resize;
      }
    }
#endif	
  }
}
