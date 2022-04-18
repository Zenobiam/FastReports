using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.TypeConverters;

namespace FastReport.Data
{
  partial class Relation : IHasEditor
  {
    #region Public Methods
    /// <inheritdoc/>
    public bool InvokeEditor()
    {
      using (RelationEditorForm form = new RelationEditorForm(this))
      {
        return form.ShowDialog() == DialogResult.OK;
      }
    }
    #endregion
  }
}
