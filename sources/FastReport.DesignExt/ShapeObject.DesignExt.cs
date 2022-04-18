using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.TypeEditors;
using FastReport.Forms;

namespace FastReport
{
  partial class ShapeObject : IHasEditor
  {
    #region Public Methods
    /// <inheritdoc/>
    public bool InvokeEditor()
    {
      using (FillEditorForm editor = new FillEditorForm())
      {
        editor.Fill = Fill.Clone();
        if (editor.ShowDialog() == DialogResult.OK)
        {
          Fill = editor.Fill;
          return true;
        }
        return false;
      }
    }

    /// <inheritdoc/>
    public override void OnBeforeInsert(int flags)
    {
      shape = (ShapeKind)flags;
    }
    #endregion
  }
}