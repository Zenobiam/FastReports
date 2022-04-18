﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using FastReport.Controls;
using FastReport.Forms;
using FastReport.Utils;

namespace FastReport.TypeEditors
{
    class DataBandColumnEditor : UITypeEditor
    {
        private IWindowsFormsEditorService edSvc;

        /// <inheritdoc/>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context.Instance is object[])
                return base.GetEditStyle(context);
            return UITypeEditorEditStyle.Modal;
        }

        /// <inheritdoc/>
        public override object EditValue(ITypeDescriptorContext context,
          IServiceProvider provider, object Value)
        {
            edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            BandColumns columns = Value as BandColumns;

            if (columns != null)
                return Editors.EditBandColumns(columns);
            return Value;
        }
    }
}
