using System;
using System.Drawing;
using FastReport.Utils;

namespace FastReport
{
    /// <summary>
    /// The class for representing visible digital signature in the report.
    /// </summary>
    public partial class DigitalSignatureObject : PictureObjectBase
    {
        protected override float ImageHeight { get { return 1; } }

        protected override float ImageWidth { get { return 1; } }

        public override void DrawImage(FRPaintEventArgs e)
        {
            if (IsDesigning)
            {
                e.Graphics.DrawString(Res.Get("Objects,DigitalSignatureObject"), DrawUtils.DefaultFont,
                    new SolidBrush(Color.Black), AbsLeft * e.ScaleX + 2, AbsTop * e.ScaleY + 0.5f);
            }
        }

        public override void LoadImage()
        {
            //throw new NotImplementedException();
        }

        protected override void DrawImageInternal2(IGraphics graphics, PointF upperLeft, PointF upperRight, PointF lowerLeft)
        {
            //throw new NotImplementedException();
        }

        protected override void ResetImageIndex()
        {
            //imageIndex = -1;
        }
    }
}