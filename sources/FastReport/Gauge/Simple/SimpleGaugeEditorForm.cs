using FastReport.Gauge.Simple.Progress;
using FastReport.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Gauge.Simple
{
    /// <inheritdoc />
    public partial class SimpleGaugeEditorForm : FastReport.Gauge.GaugeEditorForm
    {
        #region Fields

        private GroupBox gpBoxFirstSubScale;
        private GroupBox gpBoxSecondSubScale;
        private TabPage tpSubscales;
        private ComboBox cbEnabled1;
        private Label lbEnabled1;
        private Label lbShowCaption1;
        private ComboBox cbShowCaption1;

        private ComboBox cbEnabled2;
        private Label lbEnabled2;
        private Label lbShowCaption2;
        private ComboBox cbShowCaption2;

        #endregion Fields // Fields

        #region Constructors

        /// <inheritdoc />
        public SimpleGaugeEditorForm() : base()
        {
            InitializeComponent();
            Localize();
        }

        /// <inheritdoc />
        public SimpleGaugeEditorForm(GaugeObject gauge) : base(gauge)
        {
            InitializeComponent();
            Localize();
        }

        #endregion Constructors // Constructors

        #region ProtectedMethods
        /// <inheritdoc/>
        protected override void Scale()
        {
            base.Scale();
            gpBoxFirstSubScale.Size = new Size(PgGeneral.Width - DpiHelper.ConvertUnits(30), cbShowCaption1.Bottom + cbShowCaption1.Margin.Bottom);
            gpBoxSecondSubScale.Size = new Size(PgGeneral.Width - DpiHelper.ConvertUnits(30), cbShowCaption2.Bottom + cbShowCaption2.Margin.Bottom);
            gpBoxSecondSubScale.Location = new Point(gpBoxFirstSubScale.Location.X, gpBoxFirstSubScale.Bottom + gpBoxFirstSubScale.Margin.Bottom);
        }

        /// <inheritdoc />
        protected override void Init()
        {
            DeleteLabel = Gauge is SimpleProgressGauge ? false : true;
            base.Init();

            #region ControlsDesign

            gpBoxFirstSubScale = new GroupBox();
            gpBoxSecondSubScale = new GroupBox();
            lbEnabled1 = new Label();
            cbEnabled1 = new ComboBox();
            lbShowCaption1 = new Label();
            cbShowCaption1 = new ComboBox();
            lbEnabled2 = new Label();
            cbEnabled2 = new ComboBox();
            lbShowCaption2 = new Label();
            cbShowCaption2 = new ComboBox();
            int margin = 5;

            //first subscale
            gpBoxFirstSubScale.Location = new System.Drawing.Point(5, 10);
            gpBoxFirstSubScale.Name = "gpBoxFirstSubScale";
            gpBoxFirstSubScale.Margin = new Padding(10);
            gpBoxFirstSubScale.AutoSize = false;
            gpBoxFirstSubScale.Size = new Size(268, 86);
            gpBoxFirstSubScale.TabStop = false;
            gpBoxFirstSubScale.Text = "First Subscale";

            lbEnabled1.AutoSize = true;
            lbEnabled1.Margin = new Padding(margin);
            lbEnabled1.Location = new Point(gpBoxFirstSubScale.Location.X, cbxPointerBorderColor.Location.Y + lbEnabled1.Margin.Vertical);
            lbEnabled1.Name = "lbEnabled";
            lbEnabled1.Text = "Enabled :";

            cbEnabled1.AutoSize = true;
            cbEnabled1.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEnabled1.Margin = new Padding(margin);
            cbEnabled1.Location = new Point(gpBoxFirstSubScale.Width - cbEnabled1.Width - DpiHelper.ConvertUnits(4), lbEnabled1.Location.Y);
            cbEnabled1.Items.Add("true");
            cbEnabled1.Items.Add("false");
            cbEnabled1.SelectedItem = "false";
            cbEnabled1.Name = "cbEnabled";
            cbEnabled1.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            lbShowCaption1.AutoSize = true;
            lbShowCaption1.Margin = new Padding(margin);
            lbShowCaption1.Location = new Point(lbEnabled1.Location.X, lbEnabled1.Location.Y + lbEnabled1.Size.Height + lbShowCaption1.Margin.Vertical);
            lbShowCaption1.Name = "lbShowCaption";
            lbShowCaption1.Text = "Show Caption:";

            cbShowCaption1.DropDownStyle = ComboBoxStyle.DropDownList;
            cbShowCaption1.AutoSize = true;
            cbShowCaption1.Margin = new Padding(margin);
            cbShowCaption1.Location = new Point(gpBoxFirstSubScale.Width - cbShowCaption1.Width - DpiHelper.ConvertUnits(4), lbShowCaption1.Location.Y);
            cbShowCaption1.Items.Add("true");
            cbShowCaption1.Items.Add("false");
            cbShowCaption1.SelectedItem = "false";
            cbShowCaption1.Name = "cbInverted";
            cbShowCaption1.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            gpBoxFirstSubScale.Controls.AddRange(new Control[] { lbEnabled1, cbEnabled1, lbShowCaption1, cbShowCaption1 });

            //second subscale
            gpBoxSecondSubScale.Location = new Point(gpBoxFirstSubScale.Location.X, gpBoxFirstSubScale.Location.Y + gpBoxFirstSubScale.Size.Height);
            gpBoxSecondSubScale.Name = "gpBoxSecondSubScale";
            gpBoxSecondSubScale.Margin = new Padding(10);
            gpBoxSecondSubScale.AutoSize = false;
            gpBoxSecondSubScale.Size = new Size(268, 86);
            gpBoxSecondSubScale.TabStop = false;
            gpBoxSecondSubScale.Text = "Second Subscale";

            lbEnabled2.AutoSize = true;
            lbEnabled2.Margin = new Padding(margin);
            lbEnabled2.Location = new Point(gpBoxSecondSubScale.Location.X, lbEnabled2.Margin.Vertical * 2);
            lbEnabled2.Name = "lbEnabled";
            lbEnabled2.Text = "Enabled :";

            cbEnabled2.AutoSize = true;
            cbEnabled2.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEnabled2.Margin = new Padding(margin);
            cbEnabled2.Location = new Point(gpBoxSecondSubScale.Width - cbEnabled2.Width - DpiHelper.ConvertUnits(4), lbEnabled2.Location.Y);
            cbEnabled2.Items.Add("true");
            cbEnabled2.Items.Add("false");
            cbEnabled2.SelectedItem = "false";
            cbEnabled2.Name = "cbEnabled";
            cbEnabled2.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            lbShowCaption2 = new Label();
            lbShowCaption2.AutoSize = true;
            lbShowCaption2.Margin = new Padding(margin);
            lbShowCaption2.Location = new Point(lbEnabled2.Location.X, lbEnabled2.Location.Y + lbEnabled2.Size.Height + lbShowCaption2.Margin.Vertical);
            lbShowCaption2.Name = "lbShowCaption";
            lbShowCaption2.Text = "Show Caption:";

            cbShowCaption2.AutoSize = true;
            cbShowCaption2.DropDownStyle = ComboBoxStyle.DropDownList;
            cbShowCaption2.Margin = new Padding(margin);
            cbShowCaption2.Location = new Point(gpBoxSecondSubScale.Width - cbShowCaption2.Width - DpiHelper.ConvertUnits(4), lbShowCaption2.Location.Y);
            cbShowCaption2.Items.Add("true");
            cbShowCaption2.Items.Add("false");
            cbShowCaption2.SelectedItem = "false";
            cbShowCaption2.Name = "cbInverted";
            cbShowCaption2.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            gpBoxSecondSubScale.Controls.AddRange(new Control[] { lbEnabled2, cbEnabled2, lbShowCaption2, cbShowCaption2 });

            //tab page
            tpSubscales = new TabPage();
            tpSubscales.Name = "tabPageSubscales";
            tpSubscales.Size = new System.Drawing.Size(418, 525);
            tpSubscales.Text = "Subscales";
            tpSubscales.UseVisualStyleBackColor = true;
            tpSubscales.Controls.AddRange(new Control[] { gpBoxFirstSubScale, gpBoxSecondSubScale });
            tabControl3.TabPages.Add(tpSubscales);

            #endregion // ControlsDesign

            if (Gauge != null)
            {
                try
                {
                    cbEnabled1.SelectedItem = ((Gauge as SimpleGauge).Scale as SimpleScale).FirstSubScale.Enabled ? "true" : "false";
                    cbShowCaption1.SelectedItem = ((Gauge as SimpleGauge).Scale as SimpleScale).FirstSubScale.ShowCaption ? "true" : "false";

                    cbEnabled2.SelectedItem = ((Gauge as SimpleGauge).Scale as SimpleScale).SecondSubScale.Enabled ? "true" : "false";
                    cbShowCaption2.SelectedItem = ((Gauge as SimpleGauge).Scale as SimpleScale).SecondSubScale.ShowCaption ? "true" : "false";
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
                    ((Gauge as SimpleGauge).Scale as SimpleScale).FirstSubScale.Enabled = cbEnabled1.SelectedItem.ToString() == "false" ? false : true;
                    ((Gauge as SimpleGauge).Scale as SimpleScale).FirstSubScale.ShowCaption = cbShowCaption1.SelectedItem.ToString() == "false" ? false : true;

                    ((Gauge as SimpleGauge).Scale as SimpleScale).SecondSubScale.Enabled = cbEnabled2.SelectedItem.ToString() == "false" ? false : true;
                    ((Gauge as SimpleGauge).Scale as SimpleScale).SecondSubScale.ShowCaption = cbShowCaption2.SelectedItem.ToString() == "false" ? false : true;
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
            MyRes res = new MyRes("Objects,Gauge,GaugeForms,SimpleGauge");
            Text = res.Get("");
            res = new MyRes("Objects,Gauge,GaugeForms,PageScale,Subscales");
            tabControl3.TabPages[2].Text = res.Get("");
            gpBoxFirstSubScale.Text = res.Get("FirstSubscale");
            lbEnabled1.Text = res.Get("FirstSubscale,Enabled");
            lbShowCaption1.Text = res.Get("FirstSubscale,ShowCaption");
            gpBoxSecondSubScale.Text = res.Get("SecondSubscale");
            lbEnabled2.Text = res.Get("SecondSubscale,Enabled");
            lbShowCaption2.Text = res.Get("SecondSubscale,ShowCaption");
        }
    }
}
