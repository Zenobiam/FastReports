using FastReport.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FastReport.Gauge.Linear
{
    /// <inheritdoc /> 
    public partial class LinearGaugeEditorForm : GaugeEditorForm
    {
        ComboBox cbInverted;
        Label lbInverted;
        #region Constructors 

        /// <inheritdoc />
        public LinearGaugeEditorForm() : base()
        {
            InitializeComponent();
            Localize();
        }

        /// <inheritdoc />
        public LinearGaugeEditorForm(LinearGauge gauge) : base(gauge)
        {
            InitializeComponent();
            Localize();
        }

        #endregion // Constructors

        #region ProtectedMethods

        /// <inheritdoc />
        protected override void Init()
        {
            DeleteLabel = true;
            base.Init();

            #region ControlsDesign
            lbInverted = new Label();
            cbInverted = new ComboBox();
            int margin = 5;
            Button btnGeneralBorder = pgGeneral.Controls.Find("btnGeneralBorder", true)[0] as Button;
            lbInverted = new Label();
            lbInverted.AutoSize = true;
            lbInverted.Location = new Point(btnGeneralBorder.Location.X, btnGeneralBorder.Location.Y + btnGeneralBorder.Size.Height + margin * 2);
            lbInverted.Margin = new Padding(margin);
            lbInverted.Name = "lbInverted";
            lbInverted.Text = "Inverted :";

            cbInverted = new ComboBox();
            cbInverted.DropDownStyle = ComboBoxStyle.DropDownList;
            cbInverted.AutoSize = true;
            cbInverted.Location = new Point(btnGeneralFill.Location.X, lbInverted.Location.Y);
            cbInverted.Margin = new Padding(margin);
            cbInverted.Items.Add("true");
            cbInverted.Items.Add("false");
            cbInverted.SelectedItem = "false";
            cbInverted.Name = "cbInverted";

            pgGeneral.Controls.Add(lbInverted);
            pgGeneral.Controls.Add(cbInverted);

            #endregion // ControlsDesign

            if (Gauge != null)
            {
                cbInverted.SelectedItem = (Gauge as LinearGauge).Inverted ? "true" : "false";
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
                    (Gauge as LinearGauge).Inverted = cbInverted.SelectedItem.ToString() == "false" ? false : true;
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
            MyRes res = new MyRes("Objects,Gauge,GaugeForms");
            this.Text = res.Get("LinearGauge");
            res = new MyRes("Objects,Gauge,GaugeForms,PageGeneral");
            lbInverted.Text = res.Get("LinearInverdted");
        }
    }
}
