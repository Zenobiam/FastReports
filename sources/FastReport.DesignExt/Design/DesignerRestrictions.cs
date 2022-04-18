using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FastReport.Design
{
  /// <summary>
  /// Represents a set of designer's restrictions.
  /// </summary>
  public class DesignerRestrictions
  {
    #region Fields

    private bool dontLoadReport;
    private bool dontSaveReport;
    private bool dontCreateReport;
    private bool dontPreviewReport;
    private bool dontShowRecentFiles;
    private bool dontEditCode;
    private bool dontEditData;
    private bool dontCreateData;
    private bool dontSortDataSources;
    private bool dontChangeReportOptions;
    private bool dontInsertObject;
    private bool dontInsertBand;
    private bool dontDeletePage;
    private bool dontCreatePage;
    private bool dontCopyPage;
    private bool dontChangePageOptions;
    // if you add something new, don't forget to add it in the Assign method too!

    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value that enables or disables the "Open" action.
    /// </summary>
    [DefaultValue(false)]
    public bool DontLoadReport
    {
      get { return dontLoadReport; }
      set { dontLoadReport = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the "Save/Save as" actions.
    /// </summary>
    [DefaultValue(false)]
    public bool DontSaveReport
    {
      get { return dontSaveReport; }
      set { dontSaveReport = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the "New..." action.
    /// </summary>
    [DefaultValue(false)]
    public bool DontCreateReport
    {
      get { return dontCreateReport; }
      set { dontCreateReport = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the "Preview" action.
    /// </summary>
    [DefaultValue(false)]
    public bool DontPreviewReport
    {
      get { return dontPreviewReport; }
      set { dontPreviewReport = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the recent files list.
    /// </summary>
    [DefaultValue(false)]
    public bool DontShowRecentFiles
    {
      get { return dontShowRecentFiles; }
      set { dontShowRecentFiles = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the "Code" tab.
    /// </summary>
    [DefaultValue(false)]
    public bool DontEditCode
    {
      get { return dontEditCode; }
      set { dontEditCode = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the "Data" menu.
    /// </summary>
    [DefaultValue(false)]
    public bool DontEditData
    {
      get { return dontEditData; }
      set { dontEditData = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the "Data|Add New Data Source..." menu.
    /// </summary>
    [DefaultValue(false)]
    public bool DontCreateData
    {
      get { return dontCreateData; }
      set { dontCreateData = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the "Data|Sort Data Sources" menu.
    /// </summary>
    [DefaultValue(false)]
    public bool DontSortDataSources
    {
        get { return dontSortDataSources; }
        set { dontSortDataSources = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the "Report|Options..." menu.
    /// </summary>
    [DefaultValue(false)]
    public bool DontChangeReportOptions
    {
      get { return dontChangeReportOptions; }
      set { dontChangeReportOptions = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables insertion of objects.
    /// </summary>
    [DefaultValue(false)]
    public bool DontInsertObject
    {
      get { return dontInsertObject; }
      set { dontInsertObject = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the insertion of bands.
    /// </summary>
    [DefaultValue(false)]
    public bool DontInsertBand
    {
      get { return dontInsertBand; }
      set { dontInsertBand = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the "Delete Page" action.
    /// </summary>
    [DefaultValue(false)]
    public bool DontDeletePage
    {
      get { return dontDeletePage; }
      set { dontDeletePage = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the creation of report/dialog pages.
    /// </summary>
    [DefaultValue(false)]
    public bool DontCreatePage
    {
      get { return dontCreatePage; }
      set { dontCreatePage = value; }
    }

    /// <summary>
    /// Gets or set a value that enables or disbles the "Copy Page" action.
    /// </summary>
    [DefaultValue(false)]
    public bool DontCopyPage
    {
        get { return dontCopyPage; }
        set { dontCopyPage = value; }
    }

    /// <summary>
    /// Gets or sets a value that enables or disables the "Page Setup" action.
    /// </summary>
    [DefaultValue(false)]
    public bool DontChangePageOptions
    {
      get { return dontChangePageOptions; }
      set { dontChangePageOptions = value; }
    }
    #endregion

    /// <summary>
    /// Copies the contents of another, similar object.
    /// </summary>
    /// <param name="source">Source object to copy the contents from.</param>
    public void Assign(DesignerRestrictions source)
    {
      DontLoadReport = source.DontLoadReport;
      DontSaveReport = source.DontSaveReport;
      DontCreateReport = source.DontCreateReport;
      DontPreviewReport = source.DontPreviewReport;
      DontShowRecentFiles = source.DontShowRecentFiles;
      DontEditCode = source.DontEditCode;
      DontEditData = source.DontEditData;
      DontCreateData = source.DontCreateData;
      DontSortDataSources = source.DontSortDataSources;
      DontChangeReportOptions = source.DontChangeReportOptions;
      DontInsertObject = source.DontInsertObject;
      DontInsertBand = source.DontInsertBand;
      DontDeletePage = source.DontDeletePage;
      DontCreatePage = source.DontCreatePage;
      DontCopyPage = source.DontCopyPage;
      DontChangePageOptions = source.DontChangePageOptions;
    }
    
    /// <summary>
    /// Creates exact copy of this object.
    /// </summary>
    /// <returns>The copy of this object.</returns>
    public DesignerRestrictions Clone()
    {
      DesignerRestrictions restrictions = new DesignerRestrictions();
      restrictions.Assign(this);
      return restrictions;
    }
  }
}
