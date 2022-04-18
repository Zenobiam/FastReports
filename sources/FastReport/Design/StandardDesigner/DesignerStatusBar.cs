using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Forms;
using FastReport.Utils;
using FastReport.DevComponents.DotNetBar;
using FastReport.Design.PageDesigners.Page;
using FastReport.DevComponents;

namespace FastReport.Design.StandardDesigner
{
  /// <summary>
  /// Represents the designer's statusbar.
  /// </summary>
  [ToolboxItem(false)]
  public class DesignerStatusBar : Bar, IDesignerPlugin
  {
    #region Fields
    private Designer designer;
    private LabelItem lblLocation;
    private LabelItem lblSize;
    private LabelItem lblText;
    private ItemContainer pnZoom;
    private ItemContainer pnZoomButtons;
    private ButtonItem btnZoomPageWidth;
    private ButtonItem btnZoomWholePage;
    private ButtonItem btnZoom100;
    private SliderItem slZoom;
    private bool updatingZoom;
    private Timer zoomTimer;
    private float zoomToUpdate;
    #endregion

    #region Properties
    private Designer Designer
    {
      get { return designer; }
    }
    
    private float Zoom
    {
      get { return ReportWorkspace.Scale; }
      set
      {
        if (Workspace != null)
          Workspace.Zoom(value);
      }
    }
    
    private ReportWorkspace Workspace
    {
      get
      {
        if (designer.ActiveReportTab != null && designer.ActiveReportTab.ActivePageDesigner is ReportPageDesigner)
          return (Designer.ActiveReportTab.ActivePageDesigner as ReportPageDesigner).Workspace;
        return null;  
      }
    }
    #endregion

    #region Private Methods
    private void UpdateZoom()
    {
      updatingZoom = true;

      int zoom = (int)(Zoom * 100 / DpiHelper.Multiplier);
      slZoom.Text = zoom.ToString() + "%";
      if (zoom < 100)
        zoom = (int)Math.Round((zoom - 25) / 0.75f);
      else if (zoom > 100)
        zoom = (zoom - 100) / 4 + 100;
      slZoom.Value = zoom;

      updatingZoom = false;
    }

    private void btnZoomPageWidth_Click(object sender, EventArgs e)
    {
      if (Workspace != null)
        Workspace.FitPageWidth();
    }

    private void btnZoomWholePage_Click(object sender, EventArgs e)
    {
      if (Workspace != null)
        Workspace.FitWholePage();
    }

    private void btnZoom100_Click(object sender, EventArgs e)
    {
      if (Workspace != null)
        Zoom = DpiHelper.ConvertUnits(1);
    }

    private void slZoom_ValueChanged(object sender, EventArgs e)
    {
      if (updatingZoom)
        return;

      int val = slZoom.Value;
      if (val < 100)
        val = (int)Math.Round(val * 0.75f) + 25;
      else
        val = (val - 100) * 4 + 100;

      slZoom.Text = val.ToString() + "%";
      zoomToUpdate = val / 100f;
      zoomTimer.Start();
    }

    private void FZoomTimer_Tick(object sender, EventArgs e)
    {
      zoomTimer.Stop();
      Zoom = zoomToUpdate;
    }
    #endregion

    #region IDesignerPlugin
    /// <inheritdoc/>
    public string PluginName
    {
      get { return Name; }
    }

    /// <inheritdoc/>
    public void SaveState()
    {
    }

    /// <inheritdoc/>
    public void RestoreState()
    {
    }

    /// <inheritdoc/>
    public void SelectionChanged()
    {
      UpdateContent();
    }

    /// <inheritdoc/>
    public void UpdateContent()
    {
      UpdateZoom();
    }

    /// <inheritdoc/>
    public void Lock()
    {
    }

    /// <inheritdoc/>
    public void Unlock()
    {
      UpdateContent();
    }

    /// <inheritdoc/>
    public void Localize()
    {
      UpdateContent();
    }

    /// <inheritdoc/>
    public DesignerOptionsPage GetOptionsPage()
    {
      return null;
    }

    /// <inheritdoc/>
    public void UpdateUIStyle()
    {
      Style = UIStyleUtils.GetDotNetBarStyle(Designer.UIStyle);
    }
        /// <inheritdoc/>
        public void ReinitDpiSize()
        {
            Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
            PaddingBottom = DpiHelper.ConvertUnits(2);
            PaddingTop = DpiHelper.ConvertUnits(3);
            lblLocation.Image = Res.GetImage(62);
            lblLocation.Width = DpiHelper.ConvertUnits(160);
            lblLocation.Height = DpiHelper.ConvertUnits(19);
            lblSize.Image = Res.GetImage(63);
            lblSize.Width = DpiHelper.ConvertUnits(160);
            lblSize.Height = DpiHelper.ConvertUnits(19);
            lblText.Height = DpiHelper.ConvertUnits(19);
            btnZoomPageWidth.Image = ResourceLoader.GetBitmap("ZoomPageWidth.png");
            btnZoomWholePage.Image = ResourceLoader.GetBitmap("ZoomWholePage.png");
            btnZoom100.Image = ResourceLoader.GetBitmap("Zoom100.png");
            slZoom.Width = DpiHelper.ConvertUnits(120);
        }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the information about location and size.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <param name="size">The size.</param>
    public void UpdateLocationAndSize(string location, string size)
    {
      lblLocation.Visible = !String.IsNullOrEmpty(location);
      lblLocation.Text = location;
      lblSize.Visible = !String.IsNullOrEmpty(size);
      lblSize.Text = size;
    }
    
    /// <summary>
    /// Updates the name and text information.
    /// </summary>
    /// <param name="s">The text.</param>
    public void UpdateText(string s)
    {
      SelectedObjectCollection selection = designer.SelectedObjects;
      string text = selection.Count == 0 ? "" : selection.Count > 1 ?
        String.Format(Res.Get("Designer,ToolWindow,Properties,NObjectsSelected"), selection.Count) :
        selection[0].Name;
      if (!String.IsNullOrEmpty(s))
        text += ":  " + s.Replace('\r', ' ').Replace('\n', ' ');

      lblText.Text = text;
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DesignerStatusBar"/> class with default settings.
    /// </summary>
    /// <param name="designer">The report designer.</param>
    public DesignerStatusBar(Designer designer) : base()
    {
      Name = "StatusBar";
            this.designer = designer;
      BarType = eBarType.StatusBar;
      GrabHandleStyle = eGrabHandleStyle.ResizeHandle;
      Font = DpiHelper.ConvertUnits(DrawUtils.Default96Font);
      PaddingBottom = DpiHelper.ConvertUnits(2);
      PaddingTop = DpiHelper.ConvertUnits(3);

      lblLocation = new LabelItem();
      lblLocation.Image = Res.GetImage(62);
      lblLocation.Width = DpiHelper.ConvertUnits(160);
      lblLocation.Height = DpiHelper.ConvertUnits(19);

      lblSize = new LabelItem();
      lblSize.Image = Res.GetImage(63);
      lblSize.Width = DpiHelper.ConvertUnits(160);
      lblSize.Height = DpiHelper.ConvertUnits(19);

      lblText = new LabelItem();
      lblText.Height = DpiHelper.ConvertUnits(19);
      lblText.Stretch = true;
      lblText.EnableMarkup = false;

      pnZoom = new ItemContainer();
      pnZoom.BackgroundStyle.Class = "Office2007StatusBarBackground2";

      pnZoomButtons = new ItemContainer();
      pnZoomButtons.BeginGroup = true;
      pnZoomButtons.VerticalItemAlignment = eVerticalItemsAlignment.Middle;

      btnZoomPageWidth = new ButtonItem();
      btnZoomPageWidth.Image = ResourceLoader.GetBitmap("ZoomPageWidth.png");
      btnZoomPageWidth.Click += new EventHandler(btnZoomPageWidth_Click);

      btnZoomWholePage = new ButtonItem();
      btnZoomWholePage.Image = ResourceLoader.GetBitmap("ZoomWholePage.png");
      btnZoomWholePage.Click += new EventHandler(btnZoomWholePage_Click);

      btnZoom100 = new ButtonItem();
      btnZoom100.Image = ResourceLoader.GetBitmap("Zoom100.png");
      btnZoom100.Click += new EventHandler(btnZoom100_Click);
      
      pnZoomButtons.SubItems.AddRange(new BaseItem[] { btnZoomPageWidth, btnZoomWholePage, btnZoom100 });

      slZoom = new SliderItem();
      slZoom.Maximum = 200;
      slZoom.Step = 5;
      slZoom.Text = "100%";
      slZoom.Value = 100;
      slZoom.Width = DpiHelper.ConvertUnits(120);
      slZoom.ValueChanged += new EventHandler(slZoom_ValueChanged);
      
      pnZoom.SubItems.AddRange(new BaseItem[] { pnZoomButtons, slZoom });

      Items.AddRange(new BaseItem[] { lblLocation, lblSize, lblText, pnZoom });
      Dock = DockStyle.Bottom;
            this.designer.Controls.Add(this);
      
      zoomTimer = new Timer();
      zoomTimer.Interval = 50;
      zoomTimer.Tick += new EventHandler(FZoomTimer_Tick);
    }
  }

}
