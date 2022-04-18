using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using FastReport.Utils;

namespace FastReport.Dialog
{
    /// <summary>
    /// Used to group collections of controls.
    /// Wraps the <see cref="System.Windows.Forms.Panel"/> control.
    /// </summary>
    public partial class PanelControl : ParentControl
    {
        private Panel panel;

        #region Properties
        /// <summary>
        /// Gets an internal <b>Panel</b>.
        /// </summary>
        [Browsable(false)]
        public Panel Panel
        {
            get { return panel; }
        }

        /// <summary>
        /// Indicates the border style for the control. 
        /// Wraps the <see cref="System.Windows.Forms.Panel.BorderStyle"/> property.
        /// </summary>
        [DefaultValue(BorderStyle.None)]
        [Category("Appearance")]
        public BorderStyle BorderStyle
        {
            get { return Panel.BorderStyle; }
            set { Panel.BorderStyle = value; (DrawControl as Panel).BorderStyle = value; }
        }

        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);
            Pen pen = e.Cache.GetPen(Color.Gray, 1, DashStyle.Dash);
            e.Graphics.DrawRectangle(pen, AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX - 1, Height * e.ScaleY - 1);
        }
        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            PanelControl c = writer.DiffObject as PanelControl;
            base.Serialize(writer);

            if (BorderStyle != c.BorderStyle)
                writer.WriteValue("BorderStyle", BorderStyle);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <b>PanelControl</b> class with default settings. 
        /// </summary>
        public PanelControl()
        {
            panel = new Panel();
            Control = panel;
            DrawControl = new Panel();
            DrawControl.Size = new Size((int)(Control.Width * DpiScale), (int)(Control.Height * DpiScale));
        }
    }
}