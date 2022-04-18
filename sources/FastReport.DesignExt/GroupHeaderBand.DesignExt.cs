using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.Data;
using FastReport.TypeEditors;

namespace FastReport
{
  partial class GroupHeaderBand : IHasEditor
  {
    #region Public Methods
    /// <inheritdoc/>
    public override void Delete()
    {
      if (!CanDelete)
        return;
      // remove only this band, keep its subbands
      BandBase nextBand = null;
      if (NestedGroup != null)
        nextBand = NestedGroup;
      else if (Data != null)
        nextBand = Data;
      nextBand.Parent = null;
      Base parent = Parent;
      int zOrder = ZOrder;
      Dispose();
      nextBand.Parent = parent;
      nextBand.ZOrder = zOrder;
    }

    internal override string GetInfoText()
    {
      string condition = Condition;
      condition = condition.Replace("[", "");
      condition = condition.Replace("]", "");
      if (DataHelper.IsValidColumn(Report.Dictionary, condition))
      {
        string[] parts = condition.Split(new char[] { '.' });
        return parts[parts.Length - 1];
      }  
      return Condition;
    }

    /// <inheritdoc/>
    public bool InvokeEditor()
    {
      using (GroupBandEditorForm form = new GroupBandEditorForm(this))
      {
        return form.ShowDialog() == DialogResult.OK;
      }
    }

    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
      return new GroupHeaderBandMenu(Report.Designer);
    }
    #endregion
    
  }
}