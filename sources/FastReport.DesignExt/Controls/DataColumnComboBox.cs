using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using System.ComponentModel;
using FastReport.Data;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Controls
{
  /// <summary>
  /// Represents the combobox used to select a data column.
  /// </summary>
  [ToolboxItem(false)]
  public class DataColumnComboBox : TextBoxElement
    {
    private Button button;
    private Button comboButton;
    private Report report;
    private DataColumnDropDown dropDown;
    private Timer timer;
    private int closedTicks;

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DataSourceBase DataSource
    {
      get { return dropDown.DataSource; }
      set 
      { 
        // to recreate datasource list
        Report = Report;
        dropDown.DataSource = value; 
      }
    }

    /// <summary>
    /// Gets or sets the Report.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Report Report
    {
      get { return report; }
      set
      {
        report = value;
        dropDown.CreateNodes(value);
      }
    }

    private void FDropDown_ColumnSelected(object sender, EventArgs e)
    {
      Text = String.IsNullOrEmpty(dropDown.Column) ? "" : "[" + dropDown.Column + "]";
      timer.Start();
    }

    private void FDropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
    {
      closedTicks = Environment.TickCount;
    }

    private void FComboButton_Click(object sender, EventArgs e)
    {
      if (Math.Abs(Environment.TickCount - closedTicks) < 100)
        return;
      
      string column = Text.Replace("[", "");
      column = column.Replace("]", "");
      dropDown.Column = column;
      dropDown.SetSize(Width, 250);
      dropDown.Show(this, new Point(0, Height));
    }

    private void FButton_Click(object sender, EventArgs e)
    {
      Text = Editors.EditExpression(Report, Text);
      timer.Start();
    }

    private void FTimer_Tick(object sender, EventArgs e)
    {
      FindForm().BringToFront();
      TextBox.Focus();
      TextBox.Select(Text.Length, 0);
      timer.Stop();
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

    private void LayoutControls()
    {
      if (TextBox != null)
      {
        TextBox.Location = new Point(3, 3);
        TextBox.Width = Width - Height * 2 - DpiHelper.ConvertUnits(3);
      }
      if (comboButton != null)
      {
        comboButton.Location = new Point(Width - Height * 2 + 6, 2);
        comboButton.Size = new Size(Height - 6, Height - 4);
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

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (timer != null)
          timer.Dispose();
        timer = null;
      }
      base.Dispose(disposing);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataColumnComboBox"/> class.
    /// </summary>
    public DataColumnComboBox()
    {
      comboButton = new Button();
      comboButton.FlatStyle = FlatStyle.Flat;
      comboButton.FlatAppearance.BorderSize = 0;
      comboButton.Image = Res.GetImage(182);
      comboButton.Click += new EventHandler(FComboButton_Click);
      Controls.Add(comboButton);

      button = new Button();
      button.FlatStyle = FlatStyle.Flat;
      button.FlatAppearance.BorderSize = 0;
      button.Image = Res.GetImage(52);
      button.Click += new EventHandler(FButton_Click);
      Controls.Add(button);

      dropDown = new DataColumnDropDown();
      dropDown.ColumnSelected += new EventHandler(FDropDown_ColumnSelected);
      dropDown.Closed += new ToolStripDropDownClosedEventHandler(FDropDown_Closed);

      timer = new Timer();
      timer.Interval = 50;
      timer.Tick += new EventHandler(FTimer_Tick);

      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
      OnResize(EventArgs.Empty);
    }

#if !MONO
        /// <inheritdoc/>
        public override void UpdateImages()
        {
            dropDown.DataTree.Font = DpiHelper.ParseFontSize(DrawUtils.DefaultFont, 9, DpiHelper.GetMultiplierForScreen(Screen.FromControl(this)));
            comboButton.Image = Res.GetImage(182);
            button.Image = Res.GetImage(52);
        }
#endif

        /// <inheritdoc/>
        protected override void OnSizeChanged(EventArgs e)
        {
            TextBox.Font = DpiHelper.GetFontForTextBoxHeight(Height - DpiHelper.ConvertUnits(1), TextBox.Font);
            base.OnSizeChanged(e);
        }
  }
}
