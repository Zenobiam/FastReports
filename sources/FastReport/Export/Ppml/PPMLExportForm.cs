using System.Windows.Forms;
using FastReport.Export;
using FastReport.Export.Ppml;
using FastReport.Utils;

namespace FastReport.Forms
{
    /// <summary>
    /// Form for <see cref="PPMLExport"/>.
    /// For internal use only.
    /// </summary>
    public partial class PPMLExportForm : BaseExportForm
    {
        /// <inheritdoc/>
        public override void Init(ExportBase export)
        {
            base.Init(export);
            PPMLExport PPMLExport = Export as PPMLExport;
            chCurves.Checked = PPMLExport.TextInCurves;
            nudJpegQuality.Value = PPMLExport.Quality;
        }
        
        /// <inheritdoc/>
        protected override void Done()
        {
            base.Done();
            PPMLExport PPMLExport = Export as PPMLExport;
            PPMLExport.TextInCurves = chCurves.Checked;
            PPMLExport.Quality = (int)nudJpegQuality.Value;
        }
        
        /// <inheritdoc/>
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Export,PPML");
            Text = res.Get("");
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

                // move parent components from left to right
                cbOpenAfter.Left = ClientSize.Width - cbOpenAfter.Left - cbOpenAfter.Width;
                cbOpenAfter.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                lblQuality.Left = chCurves.Right - lblQuality.Width;
                lblQuality.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                nudJpegQuality.Left = lblQuality.Left - nudJpegQuality.Width - (int)(CurrentDpi * 16);
                nudJpegQuality.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                // move parent components from rigth to left
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PPMLExportForm"/> class.
        /// </summary>
        public PPMLExportForm()
        {
            InitializeComponent();
            Scale();
        }
    }
}
