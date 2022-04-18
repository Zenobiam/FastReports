using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport
{
    partial class SubreportObject
    {
        #region Properties
        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool Printable
        {
            get { return base.Printable; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool Exportable
        {
            get { return base.Exportable; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public override Border Border
        {
            get { return base.Border; }
            set { base.Border = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public override FillBase Fill
        {
            get { return base.Fill; }
            set { base.Fill = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new Cursor Cursor
        {
            get { return base.Cursor; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new Hyperlink Hyperlink
        {
            get { return base.Hyperlink; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool CanGrow
        {
            get { return base.CanGrow; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool CanShrink
        {
            get { return base.CanShrink; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new string Style
        {
            get { return base.Style; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new string BeforePrintEvent
        {
            get { return base.BeforePrintEvent; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new string AfterPrintEvent
        {
            get { return base.AfterPrintEvent; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new string ClickEvent
        {
            get { return base.ClickEvent; }
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);

            IGraphics g = e.Graphics;
            RectangleF textRect = new RectangleF((AbsLeft + 20) * e.ScaleX, (AbsTop + 2) * e.ScaleY,
              (Width - 20) * e.ScaleX, (Height - 2) * e.ScaleY);
            float mult = DpiHelper.Multiplier;
            Bitmap origImage = Res.GetImage(104);
            Size size = new Size((int)(origImage.Width * e.ScaleX / DpiHelper.Multiplier), (int)(origImage.Height * e.ScaleY / DpiHelper.Multiplier));
            Bitmap srImage = new Bitmap(origImage, size);
            g.DrawImage(srImage, (int)(AbsLeft * e.ScaleX) + 2, (int)(AbsTop * e.ScaleY) + 2);
            Font font = new Font(DrawUtils.DefaultFont.Name, DrawUtils.DefaultFont.Size * e.ScaleX * 96f / DrawUtils.ScreenDpi, DrawUtils.DefaultFont.Style);
            g.DrawString(Name, font, Brushes.Black, textRect);
            DrawMarkers(e);
        }


        /// <inheritdoc/>
        public override void SetName(string value)
        {
            base.SetName(value);
            if (IsDesigning && ReportPage != null)
            {
                ReportPage.PageName = Name;
                Report.Designer.InitPages(Report.Pages.IndexOf(Page) + 1);
            }
        }

        /// <inheritdoc/>
        public override void OnAfterInsert(InsertFrom source)
        {
            ReportPage = new ReportPage();
            Report.Pages.Add(ReportPage);
            ReportPage.SetDefaults();
            ReportPage.ReportTitle.Dispose();
            ReportPage.PageHeader.Dispose();
            ReportPage.PageFooter.Dispose();
            ReportPage.CreateUniqueName();
            reportPage.PageName = Name;
            Report.Designer.InitPages(Report.Pages.Count);
        }

        /// <inheritdoc/>
        public override void Delete()
        {
            if (ReportPage != null)
            {
                RemoveSubReport(true);
                if (Report != null)
                    Report.Designer.InitPages(Report.Pages.IndexOf(Page) + 1);
            }
            ReportPage = null;
            base.Delete();
        }

        /// <inheritdoc/>
        public override ContextMenuBase GetContextMenu()
        {
            return new SubreportObjectMenu(Report.Designer);
        }
        #endregion

        /// <inheritdoc/>
        public override void HandleDoubleClick()
        {
            if (reportPage != null)
                reportPage.Report.Designer.ActiveReportTab.ActivePage = this.ReportPage;
        }
    }
}