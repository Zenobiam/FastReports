using System.Windows.Forms;
using FastReport.Export;
using FastReport.Export.Zpl;
using FastReport.Utils;

namespace FastReport.Forms
{
    /// <summary>
    /// Form for <see cref="ZplExport"/>.
    /// For internal use only.
    /// </summary>
    public partial class ZplExportForm : BaseExportForm
    {
        /// <inheritdoc/>
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Export,Zpl");
            Text = res.Get("");
            gbPrinterSettings.Text = res.Get("PrinterSettings");
            lblDensity.Text = res.Get("Density");
            lblFontScale.Text = res.Get("FontScale");
            cbPrintAsBitmap.Text = res.Get("PrintAsBitmap");
        }

        /// <inheritdoc/>
        public override void Init(ExportBase export)
        {
            base.Init(export);
            ZplExport zplExport = Export as ZplExport;
            switch(zplExport.Density)
            {
                case ZplExport.ZplDensity.d6_dpmm_152_dpi:
                    cbDensity.SelectedIndex = 0;
                    break;
                case ZplExport.ZplDensity.d8_dpmm_203_dpi:
                    cbDensity.SelectedIndex = 1;
                    break;
                case ZplExport.ZplDensity.d12_dpmm_300_dpi:
                    cbDensity.SelectedIndex = 2;
                    break;
                case ZplExport.ZplDensity.d24_dpmm_600_dpi:
                    cbDensity.SelectedIndex = 3;
                    break;
            }
            cbPrintAsBitmap.Checked = zplExport.PrintAsBitmap;
            nudFontScale.Value = (decimal)zplExport.FontScale;
        }

        ///<inheritdoc/>
        public override void CheckRtl()
        {
            base.CheckRtl();

            // apply Right to Left layout
            if (Config.RightToLeft)
            {
                RightToLeft = RightToLeft.Yes;

                // move parent components from left to right
                cbOpenAfter.Left = ClientSize.Width - cbOpenAfter.Left - cbOpenAfter.Width;
                cbOpenAfter.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                gbPrinterSettings.RightToLeft = RightToLeft.Yes;

                cbPrintAsBitmap.Left = cbDensity.Right - cbPrintAsBitmap.Width;

                // move parent components from rigth to left
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }

        /// <inheritdoc/>
        protected override void Done()
        {
            base.Done();
            ZplExport zplExport = Export as ZplExport;
            switch(cbDensity.SelectedIndex)
            {
                case 0:
                    zplExport.Density = ZplExport.ZplDensity.d6_dpmm_152_dpi;
                    break;
                case 1:
                    zplExport.Density = ZplExport.ZplDensity.d8_dpmm_203_dpi;
                    break;
                case 2:
                    zplExport.Density = ZplExport.ZplDensity.d12_dpmm_300_dpi;
                    break;
                case 3:
                    zplExport.Density = ZplExport.ZplDensity.d24_dpmm_600_dpi;
                    break;
            }
            zplExport.PrintAsBitmap = cbPrintAsBitmap.Checked;
            zplExport.FontScale = (float)nudFontScale.Value;            
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ZplExportForm"/> class.
        /// </summary>
        public ZplExportForm()
        {
            InitializeComponent();
            Scale();
        }
    }
}
