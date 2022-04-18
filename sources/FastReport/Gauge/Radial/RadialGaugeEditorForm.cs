using FastReport.Controls;
using FastReport.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Gauge.Radial
{
    public partial class RadialGaugeEditorForm : GaugeEditorForm
    {
#region Fields

        private Label lblType;
        private FlagsControl flagsControlType;
        private Label lblPosition;
        private FlagsControl flagsControlPosition;

#endregion // Fields

#region Constructors

        /// <inheritdoc />
        public RadialGaugeEditorForm() : base()
        {
            InitializeComponent();
            Localize();
        }

        /// <inheritdoc />
        public RadialGaugeEditorForm(GaugeObject gauge) : base(gauge)
        {
            InitializeComponent();
            Localize();
        }

#endregion //Constructors

#region ProtectedMethods

        /// <inheritdoc />
        protected override void Init()
        {
            base.Init();

#region ControlsDesign

            if (Gauge != null)
            {
                try
                {
                    lblType = new Label();
                    flagsControlType = new FlagsControl();
                    lblPosition = new Label();
                    flagsControlPosition = new FlagsControl();
                    int margin = 5;

                    lblType.AutoSize = true;
                    lblType.Margin = new Padding(margin);
                    lblType.Location = new Point(btnGeneralBorder.Location.X, btnGeneralBorder.Location.Y + btnGeneralBorder.Size.Height + margin * 2);
                    lblType.Name = "lblType";
                    lblType.Text = "Type :";
                    flagsControlType.AllowMultipleFlags = false;
                    flagsControlType.BorderStyle = BorderStyle.FixedSingle;
                    flagsControlType.Margin = new Padding(margin);
                    flagsControlType.Location = new Point(btnGeneralFill.Location.X, lblType.Location.Y);
                    flagsControlType.Name = "flagsControlType";
                    flagsControlType.Flags = (Gauge as RadialGauge).Type;
                    flagsControlType.Width = btnGeneralFill.Width;

                    lblPosition.AutoSize = true;
                    lblPosition.Margin = new Padding(margin);
                    lblPosition.Location = new Point(lblType.Location.X, flagsControlType.Location.Y + flagsControlType.Size.Height + margin * 2);
                    lblPosition.Name = "lblPosition";
                    lblPosition.Text = "Position :";
                    flagsControlPosition.BorderStyle = BorderStyle.FixedSingle;
                    flagsControlPosition.Margin = new Padding(margin);
                    flagsControlPosition.Location = new Point(flagsControlType.Location.X, lblPosition.Location.Y);
                    flagsControlPosition.Name = "flagsControlType";
                    flagsControlPosition.Flags = (Gauge as RadialGauge).Position;
                    flagsControlPosition.Width = btnGeneralFill.Width;

                    pgGeneral.Controls.AddRange(new Control[] { lblType, flagsControlType, lblPosition, flagsControlPosition });
                }
                catch (Exception ex)
                {
                    if (!Config.WebMode)
                    {
                        FRMessageBox.Error(ex.Message);
                    }
                }
            }

#endregion // ControlsDesign
        }

#if !MONO
        /// <inheritdoc/>
        protected override void Scale()
        {
            base.Scale();
            flagsControlPosition.Font = DpiHelper.ConvertUnits(flagsControlPosition.Font, true);
            int itemH = (int)CreateGraphics().MeasureString(flagsControlPosition.Items[0].ToString(), flagsControlPosition.Font).Height + 3;

            int x = itemH * flagsControlPosition.Items.Count;
            float y = (96f / (int)DpiHelper.ScreenScale);

            flagsControlPosition.Height = (int)(itemH * flagsControlPosition.Items.Count);

            flagsControlType.Font = DpiHelper.ConvertUnits(flagsControlType.Font, true);
            flagsControlType.Height = (int)(itemH * flagsControlType.Items.Count);
        }

#endif

        /// <inheritdoc />
        protected override void GaugeEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                base.GaugeEditorForm_FormClosing(sender, e);
                try
                {
                    (Gauge as RadialGauge).Type = (RadialGaugeType)flagsControlType.Flags;
                    (Gauge as RadialGauge).Position = (RadialGaugePosition)flagsControlPosition.Flags;
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

#endregion // Protected Methods

        /// <inheritdoc />
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Objects,Gauge,GaugeForms,RadialGauge");
            Text = res.Get("");
            res = new MyRes("Objects,Gauge,GaugeForms,PageGeneral");
            lblType.Text = res.Get("Type");
            lblPosition.Text = res.Get("Position");
        }
    }
}
