using System;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Forms;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Controls
{
    class ScalableRadioButton : RadioButton
    {
        private bool isHover = false;
        private bool isClicked = false;
        private float thiswidth;
        // If user have more than one monitor, he can move components to it. In this case we need to calc new dpi and scale radiobutton size, because the size of the checkbox is set during startup and changes from changing dpi is blurry.
        // The user can also change the dpi dynamically and with olny one monitor, in this case the radiobuttons will stretch and blur until the application will restart.
        internal bool applyOwnBehavior = false;

        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = false; }
        }

        public ScalableRadioButton()
        {
            if (Screen.AllScreens.Length > 1)
            {
                applyOwnBehavior = true;
                FontChanged += ScalableCheckBox_SizeChanged;
                FontChanged += ScalableCheckBox_FontChanged;
                TextChanged += ScalableCheckBox_TextChanged;
                MouseEnter += ScalableCheckBox_MouseEnter;
                MouseLeave += ScalableCheckBox_MouseLeave;
                MouseDown += ScalableCheckBox_MouseDown;
                MouseUp += ScalableCheckBox_MouseUp;
                AutoSize = false;
            }
        }

        private void ScalableCheckBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isHover = true;
                isClicked = false;
                Invalidate();
            }
        }

        private void ScalableCheckBox_MouseEnter(object sender, EventArgs e)
        {
            isHover = true;
            Invalidate();
        }

        private void ScalableCheckBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isClicked = true;
                Invalidate();
            }
        }

        private void ScalableCheckBox_MouseLeave(object sender, EventArgs e)
        {
            isHover = false;
            Invalidate();
        }

        private void ScalableCheckBox_TextChanged(object sender, EventArgs e)
        {
            CalcWidth();
        }

        private void ScalableCheckBox_FontChanged(object sender, EventArgs e)
        {
            CalcWidth();
        }

        private void ScalableCheckBox_SizeChanged(object sender, EventArgs e)
        {
            this.Width = (int)(Height) + (int)CreateGraphics().MeasureString(Text, Font).Width + DpiHelper.ConvertUnits(3);// + DpiHelper.ConvertUnits(5);
        }

        internal void CalcWidth()
        {
            if (!applyOwnBehavior)
                return;
            base.AutoSize = true;
            thiswidth = CreateGraphics().MeasureString(Text, Font).Width + DpiHelper.ConvertUnits(3) + (int)(Height) + DpiHelper.ConvertUnits(5);
            base.AutoSize = false;
            this.Width = (int)thiswidth;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!applyOwnBehavior)
            {
                base.OnPaint(e);
                return;
            }
            if (thiswidth > Height)
                this.Width = (int)thiswidth;
            if (BackColor == Color.Transparent)
                BackColor = SystemColors.Window;
            e.Graphics.Clear(BackColor);

            int squareSide = (int)(Height * 0.8);
            //if (!DpiHelper.HighDpiEnabled)
            //    squareSide = (int)(squareSide * 96f / e.Graphics.DpiX);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Font f = new Font(this.Font.FontFamily, this.Font.Size * 96f / e.Graphics.DpiX, this.Font.Style, GraphicsUnit.Point, this.Font.GdiCharSet, this.Font.GdiVerticalFont);
            Rectangle rect = new Rectangle(new Point(0, Height / 2 - squareSide / 2), new Size(squareSide, squareSide));
            SizeF textSize = e.Graphics.MeasureString(Text, DpiHelper.HighDpiEnabled ? Font : f);
            if (RightToLeft == RightToLeft.Yes)
            {
                rect.X = (int)(Width - textSize.Width - rect.Width - DpiHelper.ConvertUnits(2));
            }

            e.Graphics.DrawEllipse(Enabled ? Pens.Black : Pens.Gray, rect);
            float ratio = DpiHelper.Multiplier;
            if (this.FindForm() is ScaledSupportingForm)
                ratio = (FindForm() as ScaledSupportingForm).FormRatio;
            if (Checked)
            {
                rect.Inflate(new Size((int)(-3 * ratio), (int)(-3 * ratio)));
                e.Graphics.FillEllipse(Enabled ? Brushes.Black : Brushes.Gray, rect);
                rect.Inflate(new Size((int)(3 * ratio), (int)(3 * ratio)));
            }

            if (isClicked)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(125, 0, 166, 207)))
                    e.Graphics.FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
            }
            else if (isHover)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(125, 147, 209, 238)))
                    e.Graphics.FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
            }

            if (RightToLeft == RightToLeft.No)
            {
                e.Graphics.DrawString(this.Text, DpiHelper.HighDpiEnabled ? Font : f, Enabled ? Brushes.Black : Brushes.Gray, rect.Right + DpiHelper.ConvertUnits(2), (Height * 1f) / 2 - textSize.Height / 2 + DpiHelper.ConvertUnits(1));
            }
            else
            {
                e.Graphics.DrawString(this.Text, DpiHelper.HighDpiEnabled ? Font : f, Enabled ? Brushes.Black : Brushes.Gray, Width - textSize.Width, (Height * 1f) / 2 - textSize.Height / 2 + DpiHelper.ConvertUnits(1));
            }
        }

    }
}
