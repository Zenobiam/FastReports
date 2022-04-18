using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Forms;
using FastReport.Utils;
using FastReport.Design.PageDesigners.Page;

namespace FastReport.Design.StandardDesigner
{
  /// <summary>
  /// Represents the designer's statusbar.
  /// </summary>
  [ToolboxItem(false)]
  public class DesignerStatusBar : StatusStrip, IDesignerPlugin
  {
    #region Fields
    private Designer FDesigner;
    private ToolStripStatusLabel lblName;
    private ToolStripStatusLabel lblLocation;
    private ToolStripStatusLabel lblSize;
    private ToolStripStatusLabel lblText;
    #endregion

    #region Properties
    private Designer Designer
    {
      get { return FDesigner; }
    }
    
    private ReportWorkspace Workspace
    {
      get
      {
        if (FDesigner.ActiveReportTab != null && FDesigner.ActiveReportTab.ActivePageDesigner is ReportPageDesigner)
          return (Designer.ActiveReportTab.ActivePageDesigner as ReportPageDesigner).Workspace;
        return null;  
      }
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
        public void ReinitDpiSize()
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
      UpdateText("");
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
      Renderer = UIStyleUtils.GetToolStripRenderer(Designer.UIStyle);
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
      SelectedObjectCollection selection = FDesigner.SelectedObjects;
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
      FDesigner = designer;
      Font = DrawUtils.Default96Font;

      lblLocation = new ToolStripStatusLabel(Res.GetImage(62));
      lblLocation.AutoSize = false;
      lblLocation.Size = new Size(160, 16);
      lblLocation.TextAlign = ContentAlignment.MiddleLeft;
      lblLocation.ImageAlign = ContentAlignment.MiddleLeft;

      lblSize = new ToolStripStatusLabel(Res.GetImage(63));
      lblSize.AutoSize = false;
      lblSize.Size = new Size(160, 16);
      lblSize.TextAlign = ContentAlignment.MiddleLeft;
      lblSize.ImageAlign = ContentAlignment.MiddleLeft;

      lblText = new ToolStripStatusLabel();
      lblText.AutoSize = true;
      lblText.TextAlign = ContentAlignment.MiddleLeft;

      Items.AddRange(new ToolStripItem[] { lblLocation, lblSize, lblText });
      Dock = DockStyle.Bottom;
      FDesigner.Controls.Add(this);
    }
  }

}
