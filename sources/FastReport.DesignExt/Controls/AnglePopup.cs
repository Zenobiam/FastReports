using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Design;

namespace FastReport.Controls
{
  internal class AnglePopup : PopupWindow
  {
    private AngleControl control;

    public event EventHandler AngleChanged;

    public int Angle
    {
      get { return control.Angle; }
      set { control.Angle = value; }
    }

    private void FControl_AngleChanged(object sender, EventArgs e)
    {
      if (AngleChanged != null)
        AngleChanged(this, EventArgs.Empty);
    }

    public AnglePopup(Form ownerForm) : base(ownerForm)
    {
      control = new AngleControl();
      control.ShowBorder = false;
      control.AngleChanged += new EventHandler(FControl_AngleChanged);
      Controls.Add(control);
      Font = ownerForm.Font;
      ClientSize = control.Size;
    }

  }
}
