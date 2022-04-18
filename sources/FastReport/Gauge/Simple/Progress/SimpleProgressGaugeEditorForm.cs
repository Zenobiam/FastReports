using FastReport.Controls;
using FastReport.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FastReport.Gauge.Simple.Progress
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SimpleProgressGaugeEditorForm : SimpleGaugeEditorForm
    {
        private Label lblDecimals;
        private TextBox tbDecimals;
        private Label lblPtrType;
        private ComboBox cbPtrType;

        #region Constructors

        /// <inheritdoc />
        public SimpleProgressGaugeEditorForm() : base()
        {
            InitializeComponent();
            Localize();
        }

        /// <inheritdoc />
        public SimpleProgressGaugeEditorForm(GaugeObject gauge) : base(gauge)
        {
            InitializeComponent();
            Localize();
        }

        #endregion Constructors // Constructors

        #region ProtectedMethods

        /// <inheritdoc />
        protected override void Init()
        {
            base.Init();

            #region ControlsDesign
            pageControl1.Pages.RemoveByKey("pgScale");
            tbLabelText.Visible = false;
            tbLabelText.Enabled = false;
            lblLabelText.Visible = false;

            lblDecimals = new Label();
            tbDecimals = new TextBox();
            lblPtrType = new Label();
            cbPtrType = new ComboBox();
            int margin = 5;

            lblDecimals.AutoSize = true;
            lblDecimals.Margin = new Padding(margin);
            lblDecimals.Location = lblLabelText.Location;
            lblDecimals.Name = "lblDecimals";
            lblDecimals.Text = "Decimals :";

            tbDecimals.AutoSize = true;
            tbDecimals.Margin = new Padding(margin);
            tbDecimals.Location = tbLabelText.Location;
            tbDecimals.Size = textBoxButtonLabelFont.Size;
            tbDecimals.Name = "tbDecimals";

            lblPtrType.AutoSize = true;
            lblPtrType.Margin = new Padding(margin);
            lblPtrType.Location = new Point(lblPtrBorderColor.Location.X, btnPointerFill.Location.Y + btnPointerFill.Height + margin);
            lblPtrType.Name = "lblPtrType";
            lblPtrType.Text = "Type :";

            cbPtrType.AutoSize = true;
            cbPtrType.Size = btnPointerFill.Size;
            cbPtrType.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPtrType.Margin = new Padding(margin);
            cbPtrType.Location = new Point(btnPointerFill.Location.X, lblPtrType.Location.Y);
            cbPtrType.Items.Add("Full");
            cbPtrType.Items.Add("Small");
            cbPtrType.SelectedItem = "Full";
            cbPtrType.Name = "cbPtrType";

            pgLabel.Controls.AddRange(new Control[] { lblDecimals, tbDecimals });
            pgPointer.Controls.AddRange(new Control[] { lblPtrType, cbPtrType });
            #endregion // ControlsDesign

            if (Gauge != null)
            {
                try
                {
                    tbDecimals.Text = Convert.ToString(((Gauge as SimpleProgressGauge).Label as SimpleProgressLabel).Decimals);
                    cbPtrType.SelectedItem = ((Gauge as SimpleProgressGauge).Pointer as SimpleProgressPointer).Type == SimpleProgressPointerType.Full ? "Full" : "Small";
                }
                catch (Exception ex)
                {
                    if (!Config.WebMode)
                    {
                        FRMessageBox.Error(ex.Message);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void GaugeEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                base.GaugeEditorForm_FormClosing(sender, e);
                try
                {
                    ((Gauge as SimpleProgressGauge).Label as SimpleProgressLabel).Decimals = int.Parse(tbDecimals.Text);
                    ((Gauge as SimpleProgressGauge).Pointer as SimpleProgressPointer).Type = cbPtrType.SelectedItem.ToString() == "Full" ? SimpleProgressPointerType.Full : SimpleProgressPointerType.Small;
                }
                catch (Exception ex)
                {
                    if (!Config.WebMode)
                    {
                        FRMessageBox.Error(ex.Message);
                    }
                }
            }
        }

        #endregion //Protected Methods

        /// <inheritdoc />
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Objects,Gauge,GaugeForms,SimpleProgressGauge");
            Text = res.Get("");
            res = new MyRes("Objects,Gauge,GaugeForms,PagePointer");
            lblPtrType.Text = res.Get("LabelPointerType");
            res = new MyRes("Objects,Gauge,GaugeForms,PageLabel");
            lblDecimals.Text = res.Get("Decimals");
        }
    }
}
