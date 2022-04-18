using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace FastReport.Dialog
{
    /// <summary>
    /// Represents a Windows control that displays a frame around a group of controls with an optional caption.
    /// Wraps the <see cref="System.Windows.Forms.GroupBox"/> control.
    /// </summary>
    public partial class GroupBoxControl : ParentControl
    {
        private GroupBox groupBox;

        #region Public Properties
        /// <summary>
        /// Gets an internal <b>GroupBox</b>.
        /// </summary>
        [Browsable(false)]
        public GroupBox GroupBox
        {
            get { return groupBox; }
        }        
        #endregion

        /// <summary>
        /// Initializes a new instance of the <b>GroupBoxControl</b> class with default settings. 
        /// </summary>
        public GroupBoxControl()
        {
            groupBox = new GroupBox();
            Control = groupBox;
#if !FRCORE
            DrawControl = new GroupBox();
            DrawControl.Size = new Size((int)(Control.Width * DpiScale), (int)(Control.Height * DpiScale));
#endif
        }
    }
}
