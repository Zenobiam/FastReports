using System.Text;
using System.Windows.Forms;
using FastReport.Export;
using FastReport.Export.Hpgl;
using FastReport.Utils;
using System.Globalization;

namespace FastReport.Forms
{
    /// <summary>
    /// Form for <see cref="HpglExport"/>.
    /// For internal use only.
    /// </summary>
    public partial class HpglExportForm : BaseExportForm
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new instance of the <see cref="HpglExportForm"/> class.
        /// </summary>
        public HpglExportForm()
        {
            InitializeComponent();

            // apply Right to Left layout
            if (Config.RightToLeft)
            {
                RightToLeft = RightToLeft.Yes;

                // move components to other side
                //lblCodepage.Left = gbOptions.Width - lblCodepage.Left - lblCodepage.Width;
                //lblCodepage.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                //cbbCodepage.Left = gbOptions.Width - cbbCodepage.Left - cbbCodepage.Width;
                //cbbCodepage.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                //lblFieldNames.Left = gbOptions.Width - lblFieldNames.Left - lblFieldNames.Width;
                //lblFieldNames.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                //tbFieldNames.Left = gbOptions.Width - tbFieldNames.Left - tbFieldNames.Width;
                //tbFieldNames.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                //cbDataOnly.Left = gbOptions.Width - cbDataOnly.Left - cbDataOnly.Width;
                //cbDataOnly.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                // move parent components from left to right
                cbOpenAfter.Left = ClientSize.Width - cbOpenAfter.Left - cbOpenAfter.Width;
                cbOpenAfter.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                // move parent components from rigth to left
                btnOk.Left = ClientSize.Width - btnOk.Left - btnOk.Width;
                btnCancel.Left = ClientSize.Width - btnCancel.Left - btnCancel.Width;
            }
        }

        #endregion // Constructors

        #region Protected Methods
    
        /// <inheritdoc/>
        protected override void Done()
        {
            base.Done();
            HpglExport hpglExport = Export as HpglExport;
            if (cbFillMode.SelectedIndex == 0)
                hpglExport.FillMode = HpglFillMode.Solid;
            else
                hpglExport.FillMode = HpglFillMode.Border;
            //if (cbbCodepage.SelectedIndex == 0)
            //    HpglExport.Encoding = Encoding.Default;
            //else if (cbbCodepage.SelectedIndex == 1)
            //    HpglExport.Encoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            //HpglExport.FieldNames = tbFieldNames.Text;
            //HpglExport.DataOnly = cbDataOnly.Checked;
        }

        #endregion // Protected Methods

        #region Public Methods
    
        /// <inheritdoc/>
        public override void Init(ExportBase export)
        {
            base.Init(export);
            HpglExport hpglExport = Export as HpglExport;
            if (hpglExport.FillMode == HpglFillMode.Solid)
                cbFillMode.SelectedIndex = 0;
            else
                cbFillMode.SelectedIndex = 1;
            //if (HpglExport.Encoding == Encoding.Default)
            //    cbbCodepage.SelectedIndex = 0;
            //else if (HpglExport.Encoding == Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage))
            //    cbbCodepage.SelectedIndex = 1;
            //tbFieldNames.Text = HpglExport.FieldNames;
            //cbDataOnly.Checked = HpglExport.DataOnly;
        }
        
        /// <inheritdoc/>
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Export,Hpgl");
            Text = res.Get("");
            this.lblFillMode.Text = res.Get("FillMode");
            this.cbFillMode.Items[0] = res.Get("FillModeSolid");
            this.cbFillMode.Items[1] = res.Get("FillModeBorder");
            //lblCodepage.Text = res.Get("Codepage");                        
            //cbbCodepage.Items[0] = res.Get("Default");
            //cbbCodepage.Items[1] = res.Get("OEM");
            //lblFieldNames.Text = res.Get("FieldNames");
            //cbDataOnly.Text = res.Get("DataOnly");
            res = new MyRes("Export,Misc");            
            gbOptions.Text = res.Get("Options");
        }

        #endregion // Public Methods
    }
}