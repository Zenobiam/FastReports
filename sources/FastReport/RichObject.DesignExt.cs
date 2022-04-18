using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using FastReport.TypeEditors;
using FastReport.Utils;
using FastReport.Code;
using FastReport.Controls;
using FastReport.Forms;

namespace FastReport
{
  partial class RichObject : IHasEditor
  {
    #region Public Methods
    /// <inheritdoc/>
    public bool InvokeEditor()
    {
      using (RichEditorForm form = new RichEditorForm(this))
      {
        if (form.ShowDialog() == DialogResult.OK)
        {
          actualTextStart = 0;
          actualTextLength = 0;
          return true;
        }
      }
      
      return false;
    }

    /// <inheritdoc/>
    public override SmartTagBase GetSmartTag()
    {
      return new RichObjectSmartTag(this);
    }

    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
      return new TextObjectBaseMenu(Report.Designer);
    }
    #endregion

  }
}
