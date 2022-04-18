using System.Windows.Forms;
using FastReport.Export;
using FastReport.Export.RichText;
using FastReport.Utils;

namespace FastReport.Forms
{
    /// <summary>
    /// Form for <see cref="RTFExport"/>.
    /// For internal use only.
    /// </summary>
    public partial class RTFExportForm : BaseExportForm
    {
        /// <inheritdoc/>
        public override void Init(ExportBase export)
        {
            base.Init(export);
            RTFExport rtfExport = Export as RTFExport;
            cbWysiwyg.Checked = rtfExport.Wysiwyg;
            cbPageBreaks.Checked = rtfExport.PageBreaks;
            if (rtfExport.Pictures)
                cbbPictures.SelectedIndex = rtfExport.ImageFormat == RTFImageFormat.Metafile ? 1 : (rtfExport.ImageFormat == RTFImageFormat.Jpeg ? 2 : 3);
            else 
                cbbPictures.SelectedIndex = 0;
            if(rtfExport.EmbedRichObject)
                cbbRTF.SelectedIndex = 1;
            else
                cbbRTF.SelectedIndex = 0;
        }

        /// <inheritdoc/>
        protected override void Done()
        {
            base.Done();
            RTFExport rtfExport = Export as RTFExport;
            rtfExport.Wysiwyg = cbWysiwyg.Checked;
            rtfExport.PageBreaks = cbPageBreaks.Checked;
            rtfExport.Pictures = cbbPictures.SelectedIndex > 0;
            if (cbbPictures.SelectedIndex == 1)
                rtfExport.ImageFormat = RTFImageFormat.Metafile;
            else if (cbbPictures.SelectedIndex == 2)
                rtfExport.ImageFormat = RTFImageFormat.Jpeg;
            else
                rtfExport.ImageFormat = RTFImageFormat.Png;
            if (cbbRTF.SelectedIndex == 1)
                rtfExport.EmbedRichObject = true;
            else
                rtfExport.EmbedRichObject = false;
        }

        /// <inheritdoc/>
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Export,RichText");
            Text = res.Get("");
            lblRTF.Text = res.Get("RTFObjectAs");
            cbbRTF.Items[0] = res.Get("Picture");
            cbbRTF.Items[1] = res.Get("EmbeddedRTF");
            res = new MyRes("Export,Misc");            
            gbOptions.Text = res.Get("Options");
            cbWysiwyg.Text = res.Get("Wysiwyg");
            cbPageBreaks.Text = res.Get("PageBreaks");
            lblPictures.Text = res.Get("Pictures");
            cbbPictures.Items[0] = res.Get("None");
            cbbPictures.Items[1] = res.Get("Metafile");
        }

        ///<inheritdoc/>
        public override void CheckRtl()
        {
            base.CheckRtl();

            if (Config.RightToLeft)
            {
                RightToLeft = RightToLeft.Yes;

                // move components from left to right
                cbWysiwyg.Left = gbOptions.Width - cbWysiwyg.Left - cbWysiwyg.Width;
                cbWysiwyg.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbPageBreaks.Left = gbOptions.Width - cbPageBreaks.Left - cbPageBreaks.Width;
                cbPageBreaks.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                lblPictures.Left = cbWysiwyg.Right - lblPictures.Width;
                lblPictures.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                lblRTF.Left = cbWysiwyg.Right - lblRTF.Width;
                lblRTF.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                // move parent components from left to right
                cbOpenAfter.Left = ClientSize.Width - cbOpenAfter.Left - cbOpenAfter.Width;
                cbOpenAfter.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                // move components from right to left
                cbbPictures.Left = (int)(16 * CurrentDpi);
                cbbPictures.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbbRTF.Left = (int)(16 * CurrentDpi);
                cbbRTF.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                // move parent components from rigth to left
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RTFExportForm"/> class.
        /// </summary>
        public RTFExportForm()
        {
            InitializeComponent();
            Scale();
        }
    }
}

