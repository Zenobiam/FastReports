using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Controls
{
    /// <summary>
    /// Represents the base control that combines a textbox and other elements
    /// </summary>
    [ToolboxItem(false)]
    public class TextBoxElement : Control
    {
        /// <summary>
        /// TextBox inside control.
        /// </summary>
        public TextBox TextBox { get; private set; }

        /// <inheritdoc/>
        public override string Text
        {
            get { return TextBox.Text; }
            set { TextBox.Text = value; }
        }

        /// <summary>
        /// Occurs when the text portion of the combobox is changed.
        /// </summary>
        public new event EventHandler TextChanged;
        private void FTextBox_TextChanged(object sender, EventArgs e)
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Rebuilds layout
        /// </summary>
        protected virtual void LayoutControls()
        {
            // do nothing
        }

        /// <summary>
        /// Calls LayoutControls method
        /// </summary>
        public void DoLayout()
        {
            LayoutControls();
        }
        /// <summary>
        /// Update images with correct size.
        /// </summary>
        public virtual void UpdateImages()
        {
            // do nothing
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxElement"/> class.
        /// </summary>
        public TextBoxElement()
        {
            DoubleBuffered = true;
            TextBox = new TextBox();
            TextBox.BorderStyle = BorderStyle.None;
            TextBox.TextChanged += new EventHandler(FTextBox_TextChanged);
            Controls.Add(TextBox);
        }

        /// <inheritdoc/>
        protected override void OnFontChanged(EventArgs e)
        {
            TextBox.Font = Font;
            base.OnFontChanged(e);
        }
    }
}
