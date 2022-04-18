using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using System.ComponentModel;
using FastReport.Data;

namespace FastReport.Controls
{
  internal class ParametersComboBox : TextBoxElement
  {
    private Button comboButton;
    private Report report;
    private DataColumnDropDown dropDown;
    private Timer timer;
    private int closedTicks;
    
    public event EventHandler DropDownOpening;


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
      Text = String.IsNullOrEmpty(dropDown.Column) ? "" : dropDown.Column;
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

            DropDownOpening?.Invoke(this, EventArgs.Empty);

            dropDown.Column = Text;
      dropDown.SetSize(Width, 250);
      dropDown.Show(this, new Point(0, Height));
    }

    private void FTimer_Tick(object sender, EventArgs e)
    {
      FindForm().BringToFront();
      TextBox.Focus();
      TextBox.Select(Text.Length, 0);
      timer.Stop();
    }

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
        TextBox.Width = Width - Height - 3;
      }
      if (comboButton != null)
      {
        comboButton.Location = new Point(Width - Height + 1, 1);
        comboButton.Size = new Size(Height - 2, Height - 2);
      }
    }

    //protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    //{
    //  height = textBoxHeight + 1;
    //  base.SetBoundsCore(x, y, width, height, specified);
    //  LayoutControls();
    //}

    protected override void OnLayout(LayoutEventArgs e)
    {
      base.OnLayout(e);
      LayoutControls();
    }

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

    public ParametersComboBox()
    {

      comboButton = new Button();
      comboButton.FlatStyle = FlatStyle.Flat;
      comboButton.FlatAppearance.BorderSize = 0;
      comboButton.Image = Res.GetImage(182);
      comboButton.Click += new EventHandler(FComboButton_Click);
      Controls.Add(comboButton);

      dropDown = new DataColumnDropDown();
      dropDown.DataTree.ShowColumns = false;
      dropDown.DataTree.ShowDataSources = false;
      dropDown.DataTree.ShowRelations = false;
      dropDown.DataTree.ShowParameters = true;
      dropDown.ColumnSelected += new EventHandler(FDropDown_ColumnSelected);
      dropDown.Closed += new ToolStripDropDownClosedEventHandler(FDropDown_Closed);

      timer = new Timer();
      timer.Interval = 50;
      timer.Tick += new EventHandler(FTimer_Tick);

      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
      OnResize(EventArgs.Empty);
    }
        /// <inheritdoc/>
        public override void UpdateImages()
        {
            comboButton.Image = Res.GetImage(182);
        }
    }
}
