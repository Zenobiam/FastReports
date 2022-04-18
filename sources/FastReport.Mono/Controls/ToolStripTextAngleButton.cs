using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Controls
{
    internal class ToolStripTextAngleButton : ToolStripDropDownButton
    {
        private AngleControl control;
        public event EventHandler AngleChanged;

        public int Angle
        {
            get { return control.Angle; }
            set
            {
                control.Angle = value;
            }
        }

        void control_AngleChanged(object sender, EventArgs e)
        {
            DropDown.Close();
            if (AngleChanged != null)
                AngleChanged(this, EventArgs.Empty);
        }

        public ToolStripTextAngleButton()
        {
            DisplayStyle = ToolStripItemDisplayStyle.Image;

            control = new AngleControl();
            control.AngleChanged += control_AngleChanged;
            DropDown = new FRToolStripDropDown(control);
        }

    }
}