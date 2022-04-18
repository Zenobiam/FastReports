using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.DevComponents;
using FastReport.Utils;

namespace FastReport.Forms
{
  internal partial class PreviewForm : Form
  {
    private void PreviewForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Done();
    }

    private void PreviewForm_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyData == Keys.Escape)
        Close();
    }

    private void PreviewForm_Shown(object sender, EventArgs e)
    {
      Preview.Focus();
    }

    private void Init()
    {
      Config.RestoreFormState(this);
    }

    private void Done()
    {
      Config.SaveFormState(this);
    }

    public PreviewForm()
    {
      InitializeComponent();
      Init();
    }

        /// <inheritdoc/>
        protected override void WndProc(ref Message m)
        {
            const int WM_DPICHANGED = 0x02E0;
            if (m.Msg == WM_DPICHANGED)
            {
                if(DpiHelper.HighDpiEnabled)
                {
                    float multiplier = 1;
                    if (Config.Root.FindItem("Forms").Find("PreviewForm") != -1)
                    {
                        XmlItem xi = Config.Root.FindItem("Forms").FindItem("PreviewForm");
                        string left = xi.GetProp("Left") + 10;
                        string top = xi.GetProp("Top") + 10;
                        Screen screen = Screen.FromControl(this);

                        multiplier = 1;
                        uint dpi = screen.GetDpi();
                        multiplier *= (dpi / 96f);
                    }

                    DpiHelper.RescaleWithNewDpi(() =>
                    {
                        Preview.UpdateDpiDependecies(multiplier);
                    }, multiplier);
                }

            }
            base.WndProc(ref m);
        }
    }
}