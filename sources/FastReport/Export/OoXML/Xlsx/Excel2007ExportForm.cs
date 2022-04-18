using System.Windows.Forms;
using FastReport.Export;
using FastReport.Export.OoXML;
using FastReport.Utils;

namespace FastReport.Forms
{
    /// <summary>
    /// Form for <see cref="Excel2007Export"/>.
    /// For internal use only.
    /// </summary>
    public partial class Excel2007ExportForm : BaseExportForm
    {
        /// <inheritdoc/>
        public override void Init(ExportBase export)
        {
            base.Init(export);
            MyRes res = new MyRes("Export,Xlsx");
            Text = res.Get("");
            Excel2007Export ooxmlExport = Export as Excel2007Export;
            cbWysiwyg.Checked = ooxmlExport.Wysiwyg;
            cbPageBreaks.Checked = ooxmlExport.PageBreaks;
            cbDataOnly.Checked = ooxmlExport.DataOnly;
            cbSeamless.Checked = ooxmlExport.Seamless;
            cbPrintOptimized.Checked = ooxmlExport.PrintOptimized;
            cbSplitPages.Checked = ooxmlExport.SplitPages;
            nudFontScale.Value = (decimal)ooxmlExport.FontScale;
            switch(ooxmlExport.PrintFit)
            {
                case Excel2007Export.PrintFitMode.NoScaling:
                    cbPrintScaling.SelectedIndex = 0;
                    break;
                case Excel2007Export.PrintFitMode.FitSheetOnOnePage:
                    cbPrintScaling.SelectedIndex = 1;
                    break;
                case Excel2007Export.PrintFitMode.FitAllColumsOnOnePage:
                    cbPrintScaling.SelectedIndex = 2;
                    break;
                case Excel2007Export.PrintFitMode.FitAllRowsOnOnePage:
                    cbPrintScaling.SelectedIndex = 3;
                    break;
            }
        }
        
        /// <inheritdoc/>
        protected override void Done()
        {
            base.Done();
            Excel2007Export ooxmlExport = Export as Excel2007Export;
            ooxmlExport.Wysiwyg = cbWysiwyg.Checked;
            ooxmlExport.PageBreaks = cbPageBreaks.Checked;
            ooxmlExport.DataOnly = cbDataOnly.Checked;
            ooxmlExport.Seamless = cbSeamless.Checked;
            ooxmlExport.PrintOptimized = cbPrintOptimized.Checked;
            ooxmlExport.SplitPages = cbSplitPages.Checked;
            ooxmlExport.FontScale = (float)nudFontScale.Value;
            switch(cbPrintScaling.SelectedIndex)
            {
                case 0:
                    ooxmlExport.PrintFit = Excel2007Export.PrintFitMode.NoScaling;
                    break;
                case 1:
                    ooxmlExport.PrintFit = Excel2007Export.PrintFitMode.FitSheetOnOnePage;
                    break;
                case 2:
                    ooxmlExport.PrintFit = Excel2007Export.PrintFitMode.FitAllColumsOnOnePage;
                    break;
                case 3:
                    ooxmlExport.PrintFit = Excel2007Export.PrintFitMode.FitAllRowsOnOnePage;
                    break;
            }
        }
        
        /// <inheritdoc/>
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Export,Misc");
            gbOptions.Text = res.Get("Options");
            cbWysiwyg.Text = res.Get("Wysiwyg");
            cbPageBreaks.Text = res.Get("PageBreaks");
            cbSeamless.Text = res.Get("Seamless");
            cbDataOnly.Text = Res.Get("Export,Csv,DataOnly");
            cbPrintOptimized.Text = res.Get("PrintOptimized");
            cbSplitPages.Text = res.Get("SplitPages");
            lblFontScale.Text = Res.Get("Export,Xlsx,FontScale");
            lblPrintScaling.Text = Res.Get("Export,Xlsx,PrintScaling");
            cbPrintScaling.Items.Add(Res.Get("Export,Xlsx,NoScaling"));
            cbPrintScaling.Items.Add(Res.Get("Export,Xlsx,FitSheetOnOnePage"));
            cbPrintScaling.Items.Add(Res.Get("Export,Xlsx,FitAllColumsOnOnePage"));
            cbPrintScaling.Items.Add(Res.Get("Export,Xlsx,FitAllRowsOnOnePage"));
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
                cbWysiwyg.Left = gbOptions.Width - cbWysiwyg.Left - cbWysiwyg.Width;
                cbWysiwyg.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbPageBreaks.Left = gbOptions.Width - cbPageBreaks.Left - cbPageBreaks.Width;
                cbPageBreaks.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbDataOnly.Left = gbOptions.Width - cbDataOnly.Left - cbDataOnly.Width;
                cbDataOnly.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbSeamless.Left = gbOptions.Width - cbSeamless.Left - cbSeamless.Width;
                cbSeamless.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbPrintOptimized.Left = gbOptions.Width - cbPrintOptimized.Left - cbPrintOptimized.Width;
                cbPrintOptimized.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbSplitPages.Left = gbOptions.Width - cbSplitPages.Left - cbSplitPages.Width;
                cbSplitPages.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                lblFontScale.Left = gbOptions.Width - lblFontScale.Left - lblFontScale.Width;
                lblFontScale.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                lblPrintScaling.Left = gbOptions.Width - lblPrintScaling.Left - lblPrintScaling.Width;
                lblPrintScaling.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                cbPrintScaling.Left = gbOptions.Width - cbPrintScaling.Left - cbPrintScaling.Width;
                cbPrintScaling.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                nudFontScale.Left = lblFontScale.Left - nudFontScale.Width - (int)(CurrentDpi * 16);

                // move parent components from left to right
                cbOpenAfter.Left = ClientSize.Width - cbOpenAfter.Left - cbOpenAfter.Width;
                cbOpenAfter.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                // move parent components from rigth to left
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Excel2007ExportForm"/> class.
        /// </summary>
        public Excel2007ExportForm()
        {
            InitializeComponent();
            Scale();
        }
    }
}

