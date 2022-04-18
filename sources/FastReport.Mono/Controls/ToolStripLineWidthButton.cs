using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Controls
{
  internal class ToolStripLineWidthButton : ToolStripDropDownButton
  {
    private float FLineWidth;
    private ItemSelector FSelector;
    private float[] FStdWidths;
    public event EventHandler WidthSelected;

    public float LineWidth
    {
      get { return FLineWidth; }
      set
      {
        FLineWidth = value;
        for (int i = 0; i < FStdWidths.Length; i++)
        {
          if (Math.Abs(value - FStdWidths[i]) < 1e-4)
          {
            FSelector.SelectedIndex = i;
            break;
          }
        }
      }
    }

    private void AddItem(float width, string text)
    {
      Bitmap bmp = new Bitmap(64, 14);
      using (Graphics g = Graphics.FromImage(bmp))
      using (Pen pen = new Pen(Color.Black, width))
      {
        g.DrawLine(pen, 4, 7, width >= 2 ? 61 : 60, 7);
      }
      FSelector.AddItem(bmp, text);
    }

    private void FSelector_ItemSelected(object sender, EventArgs e)
    {
      FLineWidth = FStdWidths[FSelector.SelectedIndex];
      DropDown.Close();
      if (WidthSelected != null)
        WidthSelected(this, EventArgs.Empty);
    }

    private void AddItems()
    {
      AddItem(0.25f, "0.25");
      AddItem(0.5f, "0.5");
      AddItem(1, "1");
      AddItem(1.5f, "1.5");
      AddItem(2, "2");
      AddItem(3, "3");
      AddItem(4, "4");
      AddItem(5, "5");
      FSelector.CalcSize();
    }

    public ToolStripLineWidthButton()
    {
      DisplayStyle = ToolStripItemDisplayStyle.Image;
      ImageAlign = ContentAlignment.MiddleLeft;
      if (!Config.IsRunningOnMono)
      {
        AutoSize = false;
        Size = new Size(32, 22);
      }

      FStdWidths = new float[] { 0.25f, 0.5f, 1, 1.5f, 2, 3, 4, 5 };
      FSelector = new ItemSelector();
      FSelector.ItemSelected += new EventHandler(FSelector_ItemSelected);
      AddItems();
      DropDown = new FRToolStripDropDown(FSelector);
    }
  }
}