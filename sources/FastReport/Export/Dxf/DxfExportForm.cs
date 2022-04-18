using FastReport.Export;
using FastReport.Export.Dxf;
using FastReport.Utils;
using System.Windows.Forms;

namespace FastReport.Forms
{
    /// <summary>
    /// Form for <see cref="DxfExport"/>.
    /// For internal use only.
    /// </summary>
    public partial class DxfExportForm : BaseExportForm
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DxfExportForm"/> class.
        /// </summary>
        public DxfExportForm()
        {
            InitializeComponent();
            Scale();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void Init(ExportBase export)
        {
            base.Init(export);
            DxfExport dxfExport = Export as DxfExport;
            cbFillMode.SelectedIndex = dxfExport.FillMode == DxfFillMode.Border ? 0 : 1;
            nuBarcodesGap.Value = (decimal)dxfExport.BarcodesGap;
        }

        /// <inheritdoc/>
        public override void Localize()
        {
            base.Localize();

            MyRes res = new MyRes("Export,Dxf");
            Text = res.Get("");
            this.lblDxfFillMode.Text = res.Get("FillMode");
            this.cbFillMode.Items[0] = res.Get("FillModeBorder");
            this.cbFillMode.Items[1] = res.Get("FillModeSolid");
            this.label1.Text = res.Get("BarcodesGap");
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

                // move parent components from rigth to left
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <inheritdoc/>
        protected override void Done()
        {
            base.Done();
            DxfExport dxfExport = Export as DxfExport;
            dxfExport.FillMode = cbFillMode.SelectedIndex == 0 ? DxfFillMode.Border : DxfFillMode.Solid;
            dxfExport.BarcodesGap = (float)nuBarcodesGap.Value;
        }

        #endregion Protected Methods
    }
}