using FastReport.Controls;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif


namespace FastReport.Forms
{
    /// <summary>
    /// A cap for ScaledSupportingForm in .NET
    /// </summary>
    public partial class ScaledSupportingForm : Form
    {
        #region classes
        /// <summary>
        /// Class that contains boolean values for handling scale controls.
        /// </summary>
        public class Bools
        {
            /// <summary>
            /// Determines whether the controls of this control should be traversed.
            /// </summary>
            public bool NeedCheckControl { get; set; }
            /// <summary>
            /// Determines if this control needs to be scaled.
            /// </summary>
            public bool NeedScaleThisControl { get; set; }
            /// <summary>
            /// Determines whether specific processing needs to be checked and performed on the control.
            /// </summary>
            public bool NeedSpecificConside { get; set; }
        }
        #endregion

        #region private fields
        private Bools bools;

        /// <summary>
        /// Dictionary, where the key is a control with non-standard anchors, and the value is the value of the anchor of this control
        /// </summary>
        protected Dictionary<Control, AnchorStyles> SpecificAnchors;

        #endregion

        /// <summary>
        /// A cap for .Net field FormRatio.
        /// </summary>
        public float FormRatio
        {
            get { return 1; }
        }

        #region events
        /// <summary>
        /// Represents the method that handles the start of scaling control event.
        /// </summary>
        public delegate void ControlScalingBeginEventHandler(object sender, Bools boolArgs);

        /// <summary>
        /// Called when the control is being scaled.
        /// </summary>
        public event ControlScalingBeginEventHandler ControlScalingBegin;

        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="ScaledSupportingForm"/> class.
        /// </summary>
        public ScaledSupportingForm()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }


        /// <summary>
        /// Scales all form elements to the current DPI.
        /// </summary>
        protected virtual void Scale()
        {
            return;
        }

        protected Font ParseFontSize(Font f, float size)
        {
            return f;
        }

        protected Bitmap GetImage(int index)
        {
            return Res.GetImage(index);
        }

        /// <summary>
        /// Scales control font to the current DPI.
        /// </summary>
        protected virtual void ScaleFont(Control c)
        {
            return;
        }

        /// <summary>
        /// Scales control size to the current DPI.
        /// </summary>
        protected virtual void ScaleSize(Control c)
        {
            return;
        }

        /// <summary>
        /// Scales control location to the current DPI.
        /// </summary>
        protected virtual void ScaleLocation(Control c)
        {
            return;
        }

        /// <summary>
        /// Scales form size to the current DPI.
        /// </summary>
        protected virtual void ScaleFormSize()
        {
            return;
        }

        /// <summary>
        /// Scales control.
        /// </summary>
        /// <param name="control">Control that needs to be scaled.</param>
        public void ScaleControl(Control control)
        {
            return;
        }

        /// <summary>
        /// Sets the anchor values of all controls in the collection to Top | Left
        /// </summary>
        internal void DisableAnchors(Control.ControlCollection controls)
        {
            // A cap
        }

        /// <summary>
        /// Returns anchor values to their original state
        /// </summary>
        internal void EnableAnchors()
        {
            // A cap
        }

        internal void AddHeight()
        {
            // A cap
        }
    }
}
