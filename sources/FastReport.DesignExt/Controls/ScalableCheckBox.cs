using System;
using System.Drawing;
using System.Windows.Forms;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Controls
{
    class ScalableCheckBox : CheckBox
    {
        private bool isHover = false;
        private bool isClicked = false;
        private float thiswidth;
        // If user have more than one monitor, he can move components to it. In this case we need to calc new dpi and scale checkbox size, because the size of the checkbox is set during startup and changes from changing dpi is blurry.
        // The user can also change the dpi dynamically and with olny one monitor, in this case the checkboxes will stretch and blur until the application will restart.
        internal bool applyOwnBehavior = false;

        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { if (applyOwnBehavior) base.AutoSize = false; else base.AutoSize = value; }
        }

        public override string Text { get { return base.Text; } set { base.Text = value; CalcWidth(); } }

        public ScalableCheckBox()
        {
            if(Screen.AllScreens.Length > 1)
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

        private void ScalableCheckBox_MouseEnter(object sender, EventArgs e)
        {
            isHover = true;
            Invalidate();
        }

        private void ScalableCheckBox_FontChanged(object sender, EventArgs e)
        {
            CalcWidth();
        }

        private void ScalableCheckBox_TextChanged(object sender, EventArgs e)
        {
            CalcWidth();
        }

        private void ScalableCheckBox_SizeChanged(object sender, EventArgs e)
        {
            this.Width = (int)(Height * 0.85) + (int)CreateGraphics().MeasureString(Text, Font).Width + DpiHelper.ConvertUnits(3);
        }

        internal void CalcWidth()
        {
            if (!applyOwnBehavior)
                return;
            base.AutoSize = true;
            thiswidth = CreateGraphics().MeasureString(Text, Font).Width + DpiHelper.ConvertUnits(3) + (int)(Height * 0.85);// + DpiHelper.ConvertUnits(5);
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
            int onelineheight = (int)e.Graphics.MeasureString("Aa", Font, 100000).Height;
            int squareSide = (int)(onelineheight * 0.85);

            Rectangle rect = new Rectangle(new Point(0, Height / 2 - squareSide / 2), new Size(squareSide, squareSide));

            ButtonState state = Checked ? ButtonState.Checked : ButtonState.Normal;
            if (!Enabled) state = state | ButtonState.Inactive;

            ControlPaint.DrawCheckBox(e.Graphics, rect, state);

            rect.Inflate(new Size(-1, -1));

            if (isClicked)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(125, 0, 166, 207)))
                    e.Graphics.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
            }
            else if (isHover)
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(125, 147, 209, 238)))
                    e.Graphics.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
            }

            Font f = new Font(this.Font.FontFamily, this.Font.Size * 96f / e.Graphics.DpiX, this.Font.Style, GraphicsUnit.Point, this.Font.GdiCharSet, this.Font.GdiVerticalFont);
            SizeF textSize = e.Graphics.MeasureString(Text, DpiHelper.HighDpiEnabled ? Font : f);
            if (RightToLeft == RightToLeft.No)
            {
                e.Graphics.DrawString(this.Text, DpiHelper.HighDpiEnabled ? Font : f, Enabled ? Brushes.Black : Brushes.Gray, rect.Right + DpiHelper.ConvertUnits(2), (Height * 1f) / 2 - textSize.Height / 2);
            }
            else
            {
                e.Graphics.DrawString(this.Text, DpiHelper.HighDpiEnabled ? Font : f, Enabled ? Brushes.Black : Brushes.Gray, Width - textSize.Width, (Height * 1f) / 2 - textSize.Height / 2);
            }
        }
    }
}
