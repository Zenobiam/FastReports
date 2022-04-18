using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using System.Drawing.Drawing2D;

namespace FastReport.Dialog
{
    /// <summary>
    /// Represents a Windows picture box control for displaying an image.
    /// Wraps the <see cref="System.Windows.Forms.PictureBox"/> control.
    /// </summary>
    public partial class PictureBoxControl : DialogControl
    {
        private PictureBox pictureBox;

        #region Properties
        /// <summary>
        /// Gets an internal <b>PictureBox</b>.
        /// </summary>
        [Browsable(false)]
        public PictureBox PictureBox
        {
            get { return pictureBox; }
        }

        /// <summary>
        /// Indicates the border style for the control.
        /// Wraps the <see cref="System.Windows.Forms.PictureBox.BorderStyle"/> property.
        /// </summary>
        [DefaultValue(BorderStyle.None)]
        [Category("Appearance")]
        public BorderStyle BorderStyle
        {
            get { return PictureBox.BorderStyle; }
            set 
            { 
                PictureBox.BorderStyle = value;
#if !FRCORE
                (DrawControl as PictureBox).BorderStyle = value; 
#endif
            }
        }

        /// <summary>
        /// Gets or sets the image that the PictureBox displays.
        /// Wraps the <see cref="System.Windows.Forms.PictureBox.Image"/> property.
        /// </summary>
        [Category("Appearance")]
        public Image Image
        {
            get { return PictureBox.Image; }
            set 
            { 
                PictureBox.Image = value;
#if !FRCORE
                (DrawControl as PictureBox).Image = value; 
#endif
            }
        }

        /// <summary>
        /// Indicates how the image is displayed. 
        /// Wraps the <see cref="System.Windows.Forms.PictureBox.SizeMode"/> property.
        /// </summary>
        [DefaultValue(PictureBoxSizeMode.Normal)]
        [Category("Behavior")]
        public PictureBoxSizeMode SizeMode
        {
            get { return PictureBox.SizeMode; }
            set 
            { 
                PictureBox.SizeMode = value;
#if !FRCORE
                (DrawControl as PictureBox).SizeMode = value; DrawControl.Size = new Size((int)(Control.Width * DpiScale), (int)(Control.Height * DpiScale)); 
#endif
            }
        }
        
#endregion

#region Public Methods
        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            PictureBoxControl c = writer.DiffObject as PictureBoxControl;
            base.Serialize(writer);

            if (BorderStyle != c.BorderStyle)
                writer.WriteValue("BorderStyle", BorderStyle);
            if (!writer.AreEqual(Image, c.Image))
                writer.WriteValue("Image", Image);
            if (SizeMode != c.SizeMode)
                writer.WriteValue("SizeMode", SizeMode);
        }

        ///<inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);
            Pen pen = e.Cache.GetPen(Color.Gray, 1, DashStyle.Dash);
            e.Graphics.DrawRectangle(pen, AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX - 1, Height * e.ScaleY - 1);
        }

#endregion

        /// <summary>
        /// Initializes a new instance of the <b>PictureBoxControl</b> class with default settings. 
        /// </summary>
        public PictureBoxControl()
        {
            pictureBox = new PictureBox();
            Control = pictureBox;
#if !FRCORE
            DrawControl = new PictureBox();
            DrawControl.Size = new Size((int)(Control.Width * DpiScale), (int)(Control.Height * DpiScale));
#endif
        }
    }
}