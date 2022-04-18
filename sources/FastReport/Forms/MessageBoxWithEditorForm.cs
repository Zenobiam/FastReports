using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace FastReport.Forms
{
    /// <summary>
    /// The form for message box with text editor
    /// </summary>
    public partial class MessageBoxWithEditorForm : BaseDialogForm
    {
        /// <summary>
        /// Gets or sets text
        /// </summary>
        public string InnerText
        {
            get
            {
                return tbMessage.Text;
            }
            set
            {
                tbMessage.Text = value;
            }
        }

        /// <summary>
        /// Defualt constructor
        /// </summary>
        public MessageBoxWithEditorForm()
        {
            InitializeComponent();
            btnOk.Left = Width / 2 - btnOk.Width / 2;
            Localize();
            Scale();
            tbMessage.Focus();
        }

        /// <inheritdoc/>
        public override void Localize()
        {
            base.Localize();
            this.Text = Res.Get("Forms,PrinterSetup,CmdCommand");
            lbDescription.Text = Res.Get("Forms,PrinterSetup,CmdDescription");
        }
    }
}
