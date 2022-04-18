using System;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.Gauge;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Controls;

namespace FastReport.Gauge
{
    /// <inheritdoc />
    public partial class GaugeEditorForm : BaseDialogForm
    {
        #region Fields
        GaugeObject gauge;
        private bool deleteLabel;
        #endregion // Fields

        #region Properties

        internal GaugeObject Gauge { get { return gauge; } }

        internal bool DeleteLabel { set { deleteLabel = value; } }

        #endregion // Properties

        #region Constructors

        /// <inheritdoc />
        public GaugeEditorForm() : base()
        {
            deleteLabel = false;
            InitializeComponent();
            Init();
            Localize();
        }

        /// <inheritdoc />
        public GaugeEditorForm(GaugeObject gauge) : base()
        {
            deleteLabel = false;
            this.gauge = gauge;
            InitializeComponent();
            Init();
            Localize();
            Scale();
        }

        #endregion //Constructors

        #region Protected Methods
        /// <inheritdoc/>
        protected override void Scale()
        {
            base.Scale();
            tbLabelText.Location = new Point(tbLabelText.Location.X, lblLabelText.Top);
        }

        /// <inheritdoc />
        protected virtual void Init()
        {
            if(deleteLabel)
                pageControl1.Pages.RemoveByKey("pgLabel");
            if (gauge != null)
            {
                numericUpDowMajorWidth.Value = gauge.Scale.MajorTicks.Width;
                cbxMajorTicksColor.Color = gauge.Scale.MajorTicks.Color;
                numericUpDowMinorWidth.Value = gauge.Scale.MinorTicks.Width;
                cbxMinorTicksColor.Color = gauge.Scale.MinorTicks.Color;

                textBoxButtonScaleFont.Text = gauge.Scale.Font.FontFamily.Name + ", " + gauge.Scale.Font.SizeInPoints + "pt";
                if (gauge.Scale.Font.Style != FontStyle.Regular)
                    textBoxButtonScaleFont.Text += ", style=" + gauge.Scale.Font.Style.ToString();

                textBoxButtonScaleFont.ImageIndex = 59;
                cbxPointerBorderColor.Color = gauge.Pointer.BorderColor;

                if (pgLabel != null)
                {
                    textBoxButtonLabelFont.Text = gauge.Label.Font.FontFamily.Name + ", " + gauge.Label.Font.SizeInPoints + "pt";
                    if (gauge.Label.Font.Style != FontStyle.Regular)
                        textBoxButtonLabelFont.Text += ", style=" + gauge.Label.Font.Style.ToString();
                    textBoxButtonLabelFont.ImageIndex = 59;
                    cbLabelColor.Color = gauge.Label.Color;
                    tbLabelText.Text = gauge.Label.Text;
                }
            }
        }

        #endregion // Protected Methods

        #region Public Methods

        /// <inheritdoc />
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Objects,Gauge,GaugeForms,PageGeneral");
            pgGeneral.Text = res.Get("");
            btnExpression.Text = res.Get("Expression");
            btnGeneralBorder.Text = res.Get("Border");
            btnGeneralFill.Text = res.Get("Fill");
            res = new MyRes("Objects,Gauge,GaugeForms,PageScale");
            pgScale.Text = res.Get("");
            res = new MyRes("Objects,Gauge,GaugeForms,PageScale,TabTicks");
            tabControl3.TabPages[0].Text = res.Get("");
            gpBoxMjr.Text = res.Get("MajorTicks");
            lblWidthMjr.Text = res.Get("MajorTicks,Width");
            lblColorMjr.Text = res.Get("MajorTicks,Color");
            gpBoxMnr.Text = res.Get("MinorTicks");
            lblWidthMnr.Text = res.Get("MinorTicks,Width");
            lblColorMnr.Text = res.Get("MinorTicks,Color");
            res = new MyRes("Objects,Gauge,GaugeForms,PageScale,TabText");
            tabControl3.TabPages[1].Text = res.Get("");
            lblScaleFont.Text = res.Get("Font");
            buttonFill.Text = res.Get("TextFill");
            res = new MyRes("Objects,Gauge,GaugeForms,PagePointer");
            pgPointer.Text = res.Get("");
            lblPtrBorderColor.Text = res.Get("BorderColor");
            btnPointerFill.Text = res.Get("PointerFill");
            res = new MyRes("Objects,Gauge,GaugeForms,PageLabel");
            pgLabel.Text = res.Get("");
            lblLblColor.Text = res.Get("LabelColor");
            lblLabelFont.Text = res.Get("Font");
            lblLabelText.Text = res.Get("Text");
        }

        #endregion // Public Methods

        #region Events Handlers

        /// <inheritdoc />
        protected virtual void GaugeEditorForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
                try
                {
                    gauge.Scale.MajorTicks.Width = (int)numericUpDowMajorWidth.Value;
                    gauge.Scale.MajorTicks.Color = cbxMajorTicksColor.Color;
                    gauge.Scale.MinorTicks.Width = (int)numericUpDowMinorWidth.Value;
                    gauge.Scale.MinorTicks.Color = cbxMinorTicksColor.Color;
                    gauge.Pointer.BorderColor = cbxPointerBorderColor.Color;

                    if (pgLabel != null)
                    {
                        gauge.Label.Color = cbLabelColor.Color;
                        gauge.Label.Text = tbLabelText.Text;
                    }
                }
                catch (Exception ex)
                {
                    if (!Config.WebMode)
                    {
                        FRMessageBox.Error(ex.Message);
                    }
                }

        }

        private void buttonFill_Click(object sender, EventArgs e)
        {
            using (FillEditorForm form = new FillEditorForm())
            {
                form.Fill = gauge.Scale.TextFill;
                if (form.ShowDialog() == DialogResult.OK)
                    gauge.Scale.TextFill = form.Fill;
            }
        }

        private void textBoxButtonScaleFont_ButtonClick(object sender, EventArgs e)
        {
            using (FontDialog dialog = new FontDialog())
            {
                dialog.Font = gauge.Scale.Font;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    dialog.Font = new Font(dialog.Font.FontFamily, (float)Math.Round(dialog.Font.Size), dialog.Font.Style);
                    textBoxButtonScaleFont.Text = Converter.ToString(dialog.Font);
                    gauge.Scale.Font = dialog.Font;
                }
            }
        }

        private void btnPointerFill_Click(object sender, EventArgs e)
        {
            using (FillEditorForm form = new FillEditorForm())
            {
                form.Fill = gauge.Pointer.Fill;
                if (form.ShowDialog() == DialogResult.OK)
                    gauge.Pointer.Fill = form.Fill;
            }
        }

        private void btnGeneralBorder_Click(object sender, EventArgs e)
        {
            using (BorderEditorForm form = new BorderEditorForm())
            {
                form.Border = gauge.Border;
                if (form.ShowDialog() == DialogResult.OK)
                    gauge.Border = form.Border;
            }
        }

        private void btnGeneralFill_Click(object sender, EventArgs e)
        {
            using (FillEditorForm form = new FillEditorForm())
            {
                form.Fill = gauge.Fill;
                if (form.ShowDialog() == DialogResult.OK)
                    gauge.Fill = form.Fill;
            }
        }

        private void textBoxButtonLabelFont_ButtonClick(object sender, EventArgs e)
        {
            using (FontDialog dialog = new FontDialog())
            {
                dialog.Font = gauge.Label.Font;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    dialog.Font = new Font(dialog.Font.FontFamily, (float)Math.Round(dialog.Font.Size), dialog.Font.Style);
                    textBoxButtonLabelFont.Text = Converter.ToString(dialog.Font);
                    gauge.Label.Font = dialog.Font;
                }
            }
        }

        private void btnExpression_Click(object sender, EventArgs e)
        {
            using (ExpressionEditorForm expressionForm = new ExpressionEditorForm(Gauge.Report))
            {
                expressionForm.ExpressionText = gauge.Expression;
                if (expressionForm.ShowDialog() == DialogResult.OK)
                {
                    gauge.Expression = expressionForm.ExpressionText;
                }
            }
        }

        #endregion // Events Handlers
    }
}
