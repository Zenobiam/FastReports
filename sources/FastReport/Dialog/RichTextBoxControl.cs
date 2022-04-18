using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using FastReport.Utils;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FastReport.Dialog
{
    /// <summary>
    /// Represents a Windows rich text box control.
    /// Wraps the <see cref="System.Windows.Forms.RichTextBox"/> control.
    /// </summary>
    public partial class RichTextBoxControl : DialogControl
    {
    private RichTextBox5 richTextBox;

        #region Properties
        /// <summary>
        /// Gets an internal <b>RichTextBox</b>.
        /// </summary>
        [Browsable(false)]
        public RichTextBox RichTextBox
        {
            get { return richTextBox; }
        }

        /// <summary>
        /// Gets or sets the text of the RichTextBox control, including all rich text format (RTF) codes.
        /// Wraps the <see cref="System.Windows.Forms.RichTextBox.Rtf"/> property.
        /// </summary>
        [Category("Appearance")]
        public string Rtf
        {
            get { return RichTextBox.Rtf; }
            set { RichTextBox.Rtf = value; (DrawControl as RichTextBox).Rtf = value; }
        }

        /// <summary>
        /// Gets or sets the type of scroll bars to display in the RichTextBox control.
        /// Wraps the <see cref="System.Windows.Forms.RichTextBox.ScrollBars"/> property.
        /// </summary>
        [DefaultValue(RichTextBoxScrollBars.Both)]
        [Category("Appearance")]
        public RichTextBoxScrollBars ScrollBars
        {
            get { return RichTextBox.ScrollBars; }
            set { RichTextBox.ScrollBars = value; (DrawControl as RichTextBox).ScrollBars = value; }
        }
        #endregion

        #region Public Methods        

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            RichTextBoxControl c = writer.DiffObject as RichTextBoxControl;
            base.Serialize(writer);

            if (Rtf != c.Rtf)
                writer.WriteStr("Rtf", Rtf);
            if (ScrollBars != c.ScrollBars)
                writer.WriteValue("ScrollBars", ScrollBars);
        }

        /// <summary>
        /// Loads rtf from a file.
        /// </summary>
        /// <param name="path">File to load from.</param>
        public void LoadFile(string path)
        {
            RichTextBox.LoadFile(path);
        }

        /// <summary>
        /// Loads rtf from a stream using specified stream type.
        /// </summary>
        /// <param name="data">Stream to load from.</param>
        /// <param name="fileType">Type of a stream.</param>
        public void LoadFile(Stream data, RichTextBoxStreamType fileType)
        {
            RichTextBox.LoadFile(data, fileType);
        }

        /// <summary>
        /// Loads rtf from a file using specified stream type.
        /// </summary>
        /// <param name="path">File to load from.</param>
        /// <param name="fileType">Type of a stream.</param>
        public void LoadFile(string path, RichTextBoxStreamType fileType)
        {
            RichTextBox.LoadFile(path, fileType);
        }
        #endregion

        #region Protected methods
        #endregion

        /// <summary>
        /// Initializes a new instance of the <b>RichTextBoxControl</b> class with default settings. 
        /// </summary>
        public RichTextBoxControl()
        {
            richTextBox = new RichTextBox5();
            Control = richTextBox;
            DrawControl = new RichTextBox5();
        }

    internal class RichTextBox5 : RichTextBox
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr LoadLibrary(string lpFileName);

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cparams = base.CreateParams;
                if (LoadLibrary("msftedit.dll") != IntPtr.Zero)
                {
                    cparams.ClassName = "RICHEDIT50W";
                }
                return cparams;
}
        }
    }
  }
}
