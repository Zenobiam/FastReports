using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.TypeEditors;
using FastReport.Forms;

namespace FastReport.Data
{
  partial class Total : IHasEditor
  {
    #region Public Methods
    /// <inheritdoc/>
    public bool InvokeEditor()
    {
      using (TotalEditorForm form = new TotalEditorForm(Report.Designer))
      {
        form.Total = this;
        return form.ShowDialog() == DialogResult.OK;
      }
    }
    #endregion
  }
}
