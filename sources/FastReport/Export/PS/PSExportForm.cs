using System.Windows.Forms;
using FastReport.Export;
using FastReport.Export.PS;
using FastReport.Utils;

namespace FastReport.Forms
{
    /// <summary>
    /// Form for <see cref="PSExport"/>.
    /// For internal use only.
    /// </summary>
    public partial class PSExportForm : BaseExportForm
    {
        /// <inheritdoc/>
        public override void Init(ExportBase export)
        {
            base.Init(export);
            PSExport PSExport = Export as PSExport;
            chCurves.Checked = PSExport.TextInCurves;
            chPages.Checked = PSExport.PagesInDiffFiles;
            chImages.Checked = PSExport.SaveImagesSeparately;
            nudJpegQuality.Value = PSExport.Quality;
        }
        
        /// <inheritdoc/>
        protected override void Done()
        {
            base.Done();
            PSExport PSExport = Export as PSExport;
            PSExport.TextInCurves = chCurves.Checked;
            PSExport.PagesInDiffFiles = chPages.Checked;
            PSExport.SaveImagesSeparately = chImages.Checked;
            PSExport.Quality = (int)nudJpegQuality.Value;
        }
        
        /// <inheritdoc/>
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Export,PS");
            Text = res.Get("");
            chPages.Text = res.Get("PagesInDiffFiles");
            chImages.Text = res.Get("SaveImagesSeparately");
            res = new MyRes("Export,Misc");
            gbOptions.Text = res.Get("Options");
            res = new MyRes("Export,Pdf");
            chCurves.Text = res.Get("TextInCurves");
            lblQuality.Text = res.Get("JpegQuality");
        }

        ///<inheritdoc/>
        public override void CheckRtl()
        {
            base.CheckRtl();

            // apply Right to Left layout
            if (Config.RightToLeft)
            {
                RightToLeft = RightToLeft.Yes;

                // move components to other side
                chCurves.Left = gbOptions.Width - chCurves.Left - chCurves.Width;
                chCurves.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                chPages.Left = gbOptions.Width - chPages.Left - chPages.Width;
                chPages.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                chImages.Left = gbOptions.Width - chImages.Left - chImages.Width;
                chImages.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                lblQuality.Left = gbOptions.Width - lblQuality.Left - lblQuality.Width;
                lblQuality.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                nudJpegQuality.Left = gbOptions.Width - nudJpegQuality.Left - nudJpegQuality.Width;
                nudJpegQuality.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                // move parent components from left to right
                cbOpenAfter.Left = ClientSize.Width - cbOpenAfter.Left - cbOpenAfter.Width;
                cbOpenAfter.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                // move parent components from rigth to left
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PSExportForm"/> class.
        /// </summary>
        public PSExportForm()
        {
            InitializeComponent();
            Scale();
        }
    }
}

