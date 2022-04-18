using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using FastReport.Design;
using FastReport.Utils;
using FastReport.Design.Toolbars;

namespace FastReport.Design.StandardDesigner
{
  /// <summary>
  /// Represents the standard report designer.
  /// </summary>
  /// <remarks>
  /// This control extends the <see cref="FastReport.Design.Designer"/> control with 
  /// standard menu, status bar, and toolbars.
  /// <para/>To choose toolbars and tool windows in design-time, click the "View" menu
  /// in this control and select what you want to see. Toolbars can be reordered using the mouse.
  /// <para/>To restore the designer layout at runtime, you need to call the 
  /// <see cref="FastReport.Design.Designer.RefreshLayout">RefreshLayout</see> method in your
  /// form's <b>Load</b> event handler. 
  /// </remarks>
  [ToolboxItem(true), ToolboxBitmap(typeof(Report), "Resources.DesignerControl.bmp")]
  public partial class DesignerControl : Designer
  {
    #region Fields
    private DesignerMenu mainMenu;
    private DesignerStatusBar statusBar;
    private Panel FToolStripPanel;
    private StandardToolbar standardToolbar;
    private TextToolbar textToolbar;
    private BorderToolbar borderToolbar;
    private LayoutToolbar layoutToolbar;
    private StyleToolbar styleToolbar;
    private PolygonToolbar polygonToolbar;
    private ContextMenuStrip mnuContext;
    
    private bool showMainMenu;
    private bool showStatusBar;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the main menu.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DesignerMenu MainMenu
    {
      get { return mainMenu; }
    }
    
    /// <summary>
    /// Gets or sets a value indicating whether the main menu should be displayed or not.
    /// </summary>
    [SRCategory("Toolbars")]
    [DefaultValue(true)]
    public bool ShowMainMenu
    {
      get { return showMainMenu; }
      set
      {
        showMainMenu = value;
        mainMenu.Visible = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the status bar should be displayed or not.
    /// </summary>
    [SRCategory("Toolbars")]
    [DefaultValue(true)]
    public bool ShowStatusBar
    {
      get { return showStatusBar; }
      set
      {
        showStatusBar = value;
        statusBar.Visible = value;
      }
    }
    #endregion
    
    #region Private Methods
    private void CreateToolbarMenu()
    {
      mnuContext = new ContextMenuStrip();
      FToolStripPanel.ContextMenuStrip = mnuContext;
      mnuContext.Opening += mnuContext_Opening;

      foreach (IDesignerPlugin plugin in Plugins)
      {
        if (plugin is ToolbarBase)
        {
          ToolStripMenuItem menuItem = new ToolStripMenuItem();
          menuItem.Text = (plugin as ToolbarBase).Text;
          menuItem.Tag = plugin;
          menuItem.Click += toolbar_Click;
          mnuContext.Items.Add(menuItem);
        }
      }
    }

    private void mnuContext_Opening(object sender, EventArgs e)
    {
      mnuContext.Renderer = UIStyleUtils.GetToolStripRenderer(UIStyle);
      foreach (ToolStripItem item in mnuContext.Items)
      {
        ToolStripMenuItem menuItem = item as ToolStripMenuItem;
        ToolbarBase toolbar = item.Tag as ToolbarBase;
        menuItem.Text = toolbar.Text;
        menuItem.Checked = toolbar.Visible;
      }
    }

    private void toolbar_Click(object sender, EventArgs e)
    {
      ToolbarBase toolbar = (sender as ToolStripItem).Tag as ToolbarBase;
      toolbar.Visible = !toolbar.Visible;
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override void InitPlugins()
    {
      base.InitPlugins();

      FToolStripPanel = new Panel();
      FToolStripPanel.Dock = DockStyle.Top;
      FToolStripPanel.Paint += new PaintEventHandler(FToolStripPanel_Paint);
      Controls.Add(FToolStripPanel);

      mainMenu = new DesignerMenu(this);
      statusBar = new DesignerStatusBar(this);

      standardToolbar = new StandardToolbar(this);
      textToolbar = new TextToolbar(this);
      borderToolbar = new BorderToolbar(this);
      layoutToolbar = new LayoutToolbar(this);
      styleToolbar = new StyleToolbar(this);
      polygonToolbar = new PolygonToolbar(this);

      // set toolbars position
      FToolStripPanel.Controls.AddRange(new Control[] {
        standardToolbar, layoutToolbar, textToolbar, borderToolbar, styleToolbar, polygonToolbar });
      standardToolbar.Location = new Point(0, 0);
      layoutToolbar.Location = new Point(standardToolbar.Right, 0);
      textToolbar.Location = new Point(0, standardToolbar.Bottom);
      borderToolbar.Location = new Point(textToolbar.Right, textToolbar.Top);
      styleToolbar.Location = new Point(borderToolbar.Right, borderToolbar.Top);
      polygonToolbar.Location = new Point(styleToolbar.Right, styleToolbar.Top);
      FToolStripPanel.Height = styleToolbar.Bottom;
      
      Plugins.AddRange(new IDesignerPlugin[] {
        mainMenu, statusBar, standardToolbar, textToolbar, borderToolbar,
        layoutToolbar, styleToolbar, polygonToolbar });

      CreateToolbarMenu();
    }

    private void FToolStripPanel_Paint(object sender, PaintEventArgs e)
    {
      Rectangle rect = new Rectangle(0, 0, FToolStripPanel.Width, FToolStripPanel.Height);
      using (LinearGradientBrush brush = new LinearGradientBrush(
        rect,
        UIStyleUtils.GetColorTable(UIStyle).ToolStripPanelGradientBegin,
        UIStyleUtils.GetColorTable(UIStyle).ToolStripPanelGradientEnd,
        LinearGradientMode.Horizontal))
      {
        e.Graphics.FillRectangle(brush, rect);
      }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void ShowStatus(string location, string size, string text)
    {
      statusBar.UpdateLocationAndSize(location, size);
      statusBar.UpdateText(text);  
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DesignerControl"/> class with default settings.
    /// </summary>
    public DesignerControl()
    {
      InitializeComponent();

      ShowMainMenu = true;
      ShowStatusBar = true;
    }
  }
}

