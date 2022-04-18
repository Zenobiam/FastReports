using FastReport.SVG;
using FastReport.Utils;
using Svg;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
    internal partial class SVGEditorAdvancedForm : ScaledSupportingForm 
    {
        private SVGObject svgObject;
        private readonly SVGObject originalSvgObject;
        private readonly string originalSvgString;

        public Image Image
        {
            get
            {
                return pictureBox.Image;
            }
        }

        public SVGObject SvgObject
        {
            get { return svgObject; }
            set { svgObject = value; }
        }

        public SVGEditorAdvancedForm(SVGObject svgObject)
        {
            InitializeComponent();
            Localize();
            this.svgObject = svgObject;
            originalSvgObject = (SVGObject)svgObject.Clone();
            originalSvgString = svgObject.SVGString;
            pictureBox.Image = Update(pictureBox.Width, pictureBox.Height);
            Reset();

            NUpDownMinX.ValueChanged += SetSvgObject;
            NUpDownMinY.ValueChanged += SetSvgObject;
            NUpDownWidth.ValueChanged += SetSvgObject;
            NUpDownHeight.ValueChanged += SetSvgObject;
            cbAspectRatio.CheckedChanged += SetSvgObject;
            rbGrayscale.CheckedChanged += SetSvgObject;
            rbNone.CheckedChanged += SetSvgObject;
            SizeChanged += delegate (object s, EventArgs e)
            {
                CenterImage();
            };
            Scale();
        }

        protected override void Scale()
        {
            base.Scale();
            //gbColor.Location = new Point(ClientSize.Width / 2 + DpiHelper.ConvertUnits(10));
        }

        #region Init & Reset

        private void ResetSvgObjectToOriginal()
        {
            svgObject = (SVGObject)originalSvgObject.Clone();
            if (!string.IsNullOrEmpty(originalSvgString))
                svgObject.SVGString = originalSvgString;
            if (svgObject.SvgDocument.AspectRatio.Align != SvgPreserveAspectRatio.none)
                svgObject.AspectRatio = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
        }

        private void Localize()
        {
            MyRes res = new MyRes("Forms,PictureEditorAdvanced");
            Text = res.Get("");
            btnOK.Text = Res.Get("Buttons,Ok");
            btnCancel.Text = Res.Get("Buttons,Cancel");
            btnReset.Text = res.Get("Reset");
            gbColor.Text = res.Get("Color");
            lblMinX.Text = res.Get("MinX");
            lblMinY.Text = res.Get("MinY");
            lblWidth.Text = res.Get("Horizontal");
            lblHeight.Text = res.Get("Vertical");
            cbAspectRatio.Text = res.Get("AspectRatio");
            rbNone.Text = Res.Get("Misc,None");
            rbGrayscale.Text = res.Get("Grayscale");
        }

        private void Reset()
        {
            ResetSvgObjectToOriginal();
            ResetAspectRatio();
            ResetViewBox();
            ResetGrayScale();
        }

        private void ResetAspectRatio()
        {
            cbAspectRatio.Checked = (svgObject.SvgDocument.AspectRatio.Align == SvgPreserveAspectRatio.none) ? false : true;
        }

        private void ResetViewBox()
        {
            NUpDownMinX.Minimum = 0;
            NUpDownMinX.Maximum = decimal.MaxValue;
            NUpDownMinX.Value = (decimal)svgObject.SvgDocument.ViewBox.MinX;

            NUpDownMinY.Minimum = 0;
            NUpDownMinY.Maximum = decimal.MaxValue;
            NUpDownMinY.Value = (decimal)svgObject.SvgDocument.ViewBox.MinY;

            NUpDownWidth.Minimum = 0;
            NUpDownWidth.Maximum = decimal.MaxValue;
            NUpDownWidth.Value = (decimal)svgObject.SvgDocument.ViewBox.Width;

            NUpDownHeight.Minimum = 0;
            NUpDownHeight.Maximum = decimal.MaxValue;
            NUpDownHeight.Value = (decimal)svgObject.SvgDocument.ViewBox.Height;
        }

        private void ResetGrayScale()
        {
            svgObject.Grayscale = originalSvgObject.Grayscale;
            if (svgObject.Grayscale)
                rbGrayscale.Checked = true;
            else
                rbNone.Checked = true;
        }
        #endregion

        private void SetSvgObject(object sender, EventArgs e)
        {
            if (cbAspectRatio.Checked)
                svgObject.AspectRatio = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
            else
            {
                svgObject.SizeMode = PictureBoxSizeMode.StretchImage;
                svgObject.AspectRatio = new SvgAspectRatio(SvgPreserveAspectRatio.none);
            }
            svgObject.Grayscale = rbGrayscale.Checked ? true : false;

            if (sender == NUpDownMinX)
                {
                    svgObject.ViewBox = new SvgViewBox((float)NUpDownMinX.Value,
                        svgObject.SvgDocument.ViewBox.MinY,
                        svgObject.SvgDocument.ViewBox.Width,
                        svgObject.SvgDocument.ViewBox.Height);
                }
                else if (sender == NUpDownMinY)
                {
                    svgObject.ViewBox = new SvgViewBox(svgObject.SvgDocument.ViewBox.MinX,
                                           (float)NUpDownMinY.Value,
                                           svgObject.SvgDocument.ViewBox.Width,
                                           svgObject.SvgDocument.ViewBox.Height);
                }
                else if (sender == NUpDownWidth)
                {
                    svgObject.ViewBox = new SvgViewBox(svgObject.SvgDocument.ViewBox.MinX,
                                           svgObject.SvgDocument.ViewBox.MinY,
                                           (float)NUpDownWidth.Value,
                                           svgObject.SvgDocument.ViewBox.Height);
                }
                else if (sender == NUpDownHeight)
                {
                    svgObject.ViewBox = new SvgViewBox(svgObject.SvgDocument.ViewBox.MinX,
                       svgObject.SvgDocument.ViewBox.MinY,
                       svgObject.SvgDocument.ViewBox.Width,
                       (float)NUpDownHeight.Value);
                }
           
            Redraw();
        }

        #region Main & Etc
        private void Redraw()
        {
            pictureBox.Image = Update(pictureBox.Width, pictureBox.Height);
            CenterImage();
        }

        private void CenterImage()
        {
            pictureBox.Left = Math.Max(0, (pictureBox.Parent.Width - pictureBox.Width) / 2);
            pictureBox.Top = Math.Max(0, (pictureBox.Parent.Height - pictureBox.Height) / 2);
        }

        private Image Update(int width, int height)
        {
            Image image;
            if (svgObject.Grayscale)
            {
                if (svgObject.SVGGrayscale == null)
                    svgObject.GetSVGGrayscale();
                image = svgObject.SVGGrayscale.Draw(width, height);
            }
            else
                image = svgObject.SvgDocument.Draw(width, height);

            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap destImage = new Bitmap(width, height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
            Redraw();
        }

        #endregion
    }
}
