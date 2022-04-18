using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using FastReport.Utils;
using FastReport.Design;
using FastReport.Forms;
using FastReport.TypeConverters;
using FastReport.Design.PageDesigners.Page;
using FastReport.TypeEditors;

namespace FastReport
{
  partial class ReportPage : IHasEditor
  {
    #region Properties

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override float Left
    {
      get { return base.Left; }
      set { base.Left = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override float Top
    {
      get { return base.Top; }
      set { base.Top = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override float Width
    {
      get { return base.Width; }
      set { base.Width = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override float Height
    {
      get { return base.Height; }
      set { base.Height = value; }
    }
    
    /// <inheritdoc/>
    public override SizeF SnapSize
    {
      get { return new SizeF(ReportWorkspace.Grid.SnapSize, ReportWorkspace.Grid.SnapSize); }
    }

    /// <summary>
    /// Gets a value indicating that imperial units (inches, hundreths of inches) are used.
    /// </summary>
    [Browsable(false)]
    public bool IsImperialUnitsUsed
    {
      get
      {
        return ReportWorkspace.Grid.GridUnits == PageUnits.Inches ||
          ReportWorkspace.Grid.GridUnits == PageUnits.HundrethsOfInch;
      }
    }

    #endregion

    #region Private Methods

    private bool ShouldSerializeBorder()
    {
      return !Border.Equals(new Border());
    }

    private bool ShouldSerializeFill()
    {
      return !(Fill is SolidFill) || (Fill as SolidFill).Color != SystemColors.Window;
    }

    #endregion

    #region Public Methods

    /// <inheritdoc/>
    public override void SetDefaults()
    {
      switch (Config.ReportSettings.DefaultPaperSize)
      {
        case DefaultPaperSize.A4:
          PaperWidth = 210;
          PaperHeight = 297;
          break;

        case DefaultPaperSize.Letter:
          PaperWidth = 215.9f;
          PaperHeight = 279.4f;
          break;
      }
      
      float baseHeight = Units.Millimeters * 10;
      if (IsImperialUnitsUsed)
        baseHeight = Units.Inches * 0.4f;

      ReportTitle = new ReportTitleBand();
      ReportTitle.CreateUniqueName();
      ReportTitle.Height = baseHeight;
      
      PageHeader = new PageHeaderBand();
      PageHeader.CreateUniqueName();
      PageHeader.Height = baseHeight * 0.75f;
      
      DataBand data = new DataBand();
      Bands.Add(data);
      data.CreateUniqueName();
      data.Height = baseHeight * 2;
      
      PageFooter = new PageFooterBand();
      PageFooter.CreateUniqueName();
      PageFooter.Height = baseHeight * 0.5f;

      base.SetDefaults();
    }
    
    /// <inheritdoc/>
    public override void DrawSelection(FRPaintEventArgs e)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override void HandleMouseHover(FRMouseEventArgs e)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override void HandleMouseDown(FRMouseEventArgs e)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override void HandleMouseMove(FRMouseEventArgs e)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override void HandleMouseUp(FRMouseEventArgs e)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public override Type GetPageDesignerType()
    {
      return typeof(ReportPageDesigner);
    }

    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
        return new ReportPageMenu(Report.Designer);
    }

    /// <summary>
    /// Invokes the object's editor.
    /// </summary>
    public bool InvokeEditor()
    {
      using (PageSetupForm editor = new PageSetupForm())
      {
        editor.Page = this;
        return editor.ShowDialog() == DialogResult.OK;
      }
    }

    #endregion
  }
}