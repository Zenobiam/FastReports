using System.Windows.Forms;
using FastReport.Forms;
using FastReport.Utils;

namespace FastReport
{
  partial class DataBand: IHasEditor
  {
    #region Private Methods
    private bool ShouldSerializeColumns()
    {
      return Columns.Count > 1;
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Delete()
    {
      if (!CanDelete)
        return;
      // remove only this band, keep its subbands
      if (Parent is ReportPage || Parent is DataBand)
      {
        Base parent = Parent;
        int zOrder = ZOrder;
        Parent = null;
        while (Bands.Count > 0)
        {
          BandBase band = Bands[Bands.Count - 1];
          band.Parent = parent;
          band.ZOrder = zOrder;
        }
        Dispose();
      }
    }

    /// <inheritdoc/>
    public override SmartTagBase GetSmartTag()
    {
      return new DataBandSmartTag(this);
    }

    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
      return new DataBandMenu(Report.Designer);
    }

    internal override string GetInfoText()
    {
      return DataSource == null ? "" : DataSource.FullName;
    }

    /// <inheritdoc/>
    public bool InvokeEditor()
    {
      using (DataBandEditorForm form = new DataBandEditorForm(this))
      {
        return form.ShowDialog(Config.MainForm) == DialogResult.OK;
      }
    }
    
    /// <summary>
    /// Invokes column editor
    /// </summary>
    public bool InvokeColumnsEditor()
    {
      using (DataBandColumnEditorForm form = new DataBandColumnEditorForm(this.Columns))
      {
        return form.ShowDialog(Config.MainForm) == DialogResult.OK;
      }
    }    
    #endregion
  }
}