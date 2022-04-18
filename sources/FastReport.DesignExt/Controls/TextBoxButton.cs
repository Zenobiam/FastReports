using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Controls
{
  /// <summary>
  /// Represents the control that combines a textbox and a button.
  /// </summary>
  [ToolboxItem(false)]
  public class TextBoxButton : TextBoxElement
  {
    private Button button;
    private int imageIndex;
    /// <summary>
    /// Occurs when the button is clicked.
    /// </summary>
    public event EventHandler ButtonClick;

    /// <summary>
    /// Gets or sets the button's image.
    /// </summary>
    public Image Image
    {
      get { return button.Image; }
      set { button.Image = value; }
    }
        /// <summary>
        /// Gets or sets the button's text.
        /// </summary>
        public string ButtonText
        {
            get { return button.Text; }
            set { button.Text = value; }
        }

        /// <summary>
        /// Gets or sets the button's imageindex.
        /// </summary>
        public int ImageIndex
        {
            get { return imageIndex; }
            set 
            { 
                button.Image = Res.GetImage(value);
                imageIndex = value;
            }
        }

    private void FButton_Click(object sender, EventArgs e)
    {
      if (ButtonClick != null)
        ButtonClick(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics g = e.Graphics;
      g.FillRectangle(Enabled ? SystemBrushes.Window : SystemBrushes.Control, DisplayRectangle);
      if (Enabled)
        ControlPaint.DrawVisualStyleBorder(g, new Rectangle(0, 0, Width - 1, Height - 1));
      else
        g.DrawRectangle(SystemPens.InactiveBorder, new Rectangle(0, 0, Width - 1, Height - 1));
    }

        /// <inheritdoc/>
    protected override void LayoutControls()
    {
      if (TextBox != null)
      {
        TextBox.Location = new Point(3, 3);
        TextBox.Width = Width - Height - 5;
      }
      if (button != null)
      {
        button.Location = new Point(Width - Height + 1, 1);
        button.Size = new Size(Height - 2, Height - 2);
      }
    }

    /// <inheritdoc/>
    //protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    //{
    //        if (isConsiderTbHeight)
    //            height = textBoxHeight + 1;
    //  base.SetBoundsCore(x, y, width, height, specified);
    //  LayoutControls();
    //}

    /// <inheritdoc/>
    protected override void OnLayout(LayoutEventArgs e)
    {
      base.OnLayout(e);
      LayoutControls();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TextBoxButton"/> class.
    /// </summary>
    public TextBoxButton()
    {
      button = new Button();
      button.FlatStyle = FlatStyle.Flat;
      button.FlatAppearance.BorderSize = 0;
      button.Click += new EventHandler(FButton_Click);
      Controls.Add(button);
      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
      OnResize(EventArgs.Empty);
    }
        /// <inheritdoc/>
        public override void UpdateImages()
        {
            ImageIndex = ImageIndex;
        }
        /// <inheritdoc/>
        protected override void OnSizeChanged(EventArgs e)
        {
            TextBox.Font = DpiHelper.GetFontForTextBoxHeight(Height - DpiHelper.ConvertUnits(1), TextBox.Font);
            base.OnSizeChanged(e);
        }
    }
}
