using FastReport.Controls;
using System.Windows.Forms;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using Microsoft.Win32;
using System.Linq;
using System.Reflection;
#if MSCHART
using FastReport.MSChart;
#endif
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
    /// <summary>
    /// Base class for all dialog forms with two buttons, OK and Cancel. 
    /// </summary>
    public partial class ScaledSupportingForm : Form
    {
        private const int DEFAULT_CAPTION_HEIGHT = 25;

        #region classes
        /// <summary>
        /// Class that contains boolean values for handling scale controls.
        /// </summary>
        public class Bools
        {
            /// <summary>
            /// Determines whether the controls of this control should be traversed.
            /// </summary>
            public bool NeedCheckControl { get; set; }
            /// <summary>
            /// Determines if this control needs to be scaled.
            /// </summary>
            public bool NeedScaleThisControl { get; set; }
            /// <summary>
            /// Determines whether specific processing needs to be checked and performed on the control.
            /// </summary>
            public bool NeedSpecificConside { get; set; }
        }
        #endregion

        #region private fields
        private Screen actualScreen;
        private bool inited = false;
        private Bools bools;
        public float OpenDialogMult;
        private float tempMultiplier;
        private int openCaptionHeight;
        private Dictionary<Control, Font> initialFonts;
        private bool dynamicScaled = false;
        private float? formRatio;
        #endregion

        /// <summary>
        /// Dictionary with controls that have a anchots with a value other than left top
        /// </summary>
        protected Dictionary<Control, AnchorStyles> SpecificAnchors;

        /// <summary>
        /// Gets the Dpi ratio of current form.
        /// </summary>
        public float FormRatio
        {
            get
            {
                if (formRatio == null)
                    return DpiHelper.Multiplier;
                return formRatio.Value;
            }
        }

        /// <summary>
        /// Returns form Dpi
        /// </summary>
        public float CurrentDpi { get; private set; }

        #region events
        /// <summary>
        /// Represents the method that handles the start of scaling control event.
        /// </summary>
        public delegate void ControlScalingBeginEventHandler(object sender, Bools boolArgs);

        /// <summary>
        /// Called when the control is being scaled.
        /// </summary>
        public event ControlScalingBeginEventHandler ControlScalingBegin;

        /// <summary>
        /// Called when the scaling is end.
        /// </summary>
        public event EventHandler ScaleEnded;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaledSupportingForm"/> class.
        /// </summary>
        public ScaledSupportingForm()
        {
            InitializeComponent();
            CurrentDpi = DpiHelper.Multiplier;
            tempMultiplier = DpiHelper.Multiplier;
            bools = new Bools();
            SpecificAnchors = new Dictionary<Control, AnchorStyles>();
            initialFonts = new Dictionary<Control, Font>();
            OpenDialogMult = DpiHelper.Multiplier;
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            StartPosition = FormStartPosition.CenterParent;
            actualScreen = Screen.FromControl(this);
            this.Text = "Form";
        }

        ///<inheritdoc/>
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            Screen scr = Screen.FromPoint(Bounds.Location);
            if (!scr.Equals(actualScreen))
            {
                if (inited)
                {
                    actualScreen = scr;
                    float multiplier = 1;
                    Screen screen = Screen.FromPoint(Bounds.Location);
                    uint dpi = screen.GetDpi();
                    multiplier *= (dpi / 96f);

                    if (OpenDialogMult != multiplier)
                    {
                        RescaleFormDynamic(multiplier);
                        return;
                    }
                }
            }
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            try
            {
                if (DpiHelper.GetMultiplierForScreen(DpiHelper.ApplicationScreen) != DpiHelper.GetMultiplierForScreen(Screen.FromControl(this)))
                    return;
            }
            catch { }
            RescaleFormDynamic(DpiHelper.Multiplier);
            return;
        }

        private void RescaleFormDynamic(float multiplier)
        {
            dynamicScaled = true;
            CurrentDpi = (int)DpiHelper.ScreenScale;
            tempMultiplier = DpiHelper.Multiplier;
            //Height += ((int)(DEFAULT_CAPTION_HEIGHT * (DpiHelper.GetScreenScaleAsPersent() / 100f)) - openCaptionHeight);
            DpiHelper.RescaleWithNewDpi(() =>
            {
                formRatio = multiplier;
                Scale();
                foreach (var entry in initialFonts)
                {
                    entry.Key.Font = new Font(entry.Value.Name, entry.Value.Size * multiplier * 96f / DpiHelper.BaseDpi, entry.Value.Style);
                }
            }, multiplier / OpenDialogMult);
            OpenDialogMult = multiplier;
            openCaptionHeight = (int)(DEFAULT_CAPTION_HEIGHT * (DpiHelper.GetScreenScaleAsPersent() / 100f));
        }

        #region public methods

        /// <summary>
        /// Scales control.
        /// </summary>
        /// <param name="control">Control that needs to be scaled.</param>
        public void ScaleControl(Control control)
        {
            control.SuspendLayout();
            bools.NeedScaleThisControl = true;
            bools.NeedCheckControl = true;
            bools.NeedSpecificConside = true;
            ControlScalingBegin?.Invoke(control, bools);
            if (bools.NeedSpecificConside)
                SpecificConside(control, bools);
            if (bools.NeedScaleThisControl)
            {
                if (!IsParentContainer(control))
                {
                    ScaleFont(control);
                }

                ScaleLocation(control);
                ScaleSize(control);

                control.Padding = DpiHelper.ConvertUnits(control.Padding);
                control.Margin = DpiHelper.ConvertUnits(control.Margin);
                CheckAfterScale(control);
            }
            if (bools.NeedCheckControl)
                foreach (Control c in control.Controls)
                {
                    ScaleControl(c);
                }
            control.ResumeLayout();
            control.PerformLayout();
        }

        /// <summary>
        /// Scale font of control
        /// </summary>
        /// <param name="control">Control wich must to b scaled</param>
        protected void ScaleFont(Control control)
        {
            if (!dynamicScaled)
            {
                initialFonts.Add(control, (Font)control.Font.Clone());
                control.Font = DpiHelper.ConvertUnits(control.Font, true);
            }
            else
            {
                //initialFonts.TryGetValue(control, out Font f);
                DpiHelper.RescaleWithNewDpi(() =>
                {
                    control.Font = DpiHelper.ConvertUnits(control.Font, true);
                }, FormRatio);
            }
        }

        #endregion

        #region protected methods

        /// <summary>
        /// Scales all form elements and form size to the current DPI.
        /// </summary>
        protected virtual void Scale()
        {
            if (!DpiHelper.HighDpiEnabled)
                return;

            if (!inited)
            {
                float multiplier = DpiHelper.GetMultiplierForScreen(Screen.FromControl(this));
                DpiHelper.RescaleWithNewDpi(() =>
                {
                    ScaleInternal();
                }, multiplier);
            }
            else
                ScaleInternal();
        }

        protected void ScaleInternal()
        {
            SuspendLayout();
            DisableAnchors(Controls);
            ScaleFormSize();
            UpdateResources();
            ScaleControls();
            EnableAnchors();
            //AddHeight();
            SpecificAnchors.Clear();
            if (FormBorderStyle == FormBorderStyle.Sizable)
            {
                var mult = FormRatio;// * 96f / (int)DpiHelper.ScreenScale);
                ClientSize = new Size((int)(ClientSize.Width / mult), (int)(ClientSize.Height / mult));
            }
            ScaleMinimumSize();
            ResumeLayout();
            PerformLayout();
            AddHeight();
            ScaleEnded?.Invoke(this, EventArgs.Empty);

            inited = true;

            OpenDialogMult = DpiHelper.Multiplier;
        }

        internal void AddHeight()
        {
            DisableAnchors(Controls);
            Control b = GetMinimumBottom();
            if (b == null)
                return;
            if (b.Bottom > this.ClientSize.Height)
                ClientSize = new Size(ClientSize.Width, ClientSize.Height + b.Bottom - ClientSize.Height + (int)(7 * FormRatio));
            else
                ClientSize = new Size(ClientSize.Width, b.Bottom + (int)(10 * FormRatio));
            EnableAnchors();
        }

        /// <summary>
        /// Scales all form elements
        /// </summary>
        protected void ScaleControls()
        {
            if (!DpiHelper.HighDpiEnabled)
                return;
            foreach (Control c in Controls)
            {
                ScaleControl(c);
            }
        }

        /// <summary>
        /// Updates resources to be resized.
        /// </summary>
        protected virtual void UpdateResources()
        {
        }

        /// <summary>
        /// Scales form size to the current DPI.
        /// </summary>
        protected void ScaleFormSize()
        {
            this.ClientSize = DpiHelper.ConvertUnits(this.ClientSize);
        }

        /// <summary>
        /// Sets the anchor values of all controls in the collection to Top | Left
        /// </summary>
        internal void DisableAnchors(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c.Anchor != (AnchorStyles.Left | AnchorStyles.Top))
                {
                    SpecificAnchors.Add(c, c.Anchor);
                    c.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                }
                DisableAnchors(c.Controls);
            }
        }

        /// <summary>
        /// Returns anchor values to their original state
        /// </summary>
        internal void EnableAnchors()
        {
            foreach (var entry in SpecificAnchors)
            {
                entry.Key.Anchor = entry.Value;
            }
            SpecificAnchors.Clear();
        }

        /// <summary>
        /// Scale label with autosize = non property.
        /// </summary>
        protected void ScaleAutosizeLabel(Control control)
        {
            if (!IsParentContainer(control))
                ScaleFont(control);
            bools.NeedScaleThisControl = false;
            ScaleLocation(control);
            ScaleSize(control);
            if (control.Right > control.Parent.Width)
                control.Width = control.Parent.Width - control.Left;
            control.Height = (int)CreateGraphics().MeasureString(control.Text, control.Font, control.Width).Height;
        }

        protected Font ParseFontSize(Font baseFont, float fontsize)
        {
            return new Font(baseFont.Name, fontsize * FormRatio * 96f / DpiHelper.BaseDpi, baseFont.Style);
        }

        /// <summary>
        /// Scale location of control
        /// </summary>
        /// <param name="control">Control wich must to b scaled</param>
        protected void ScaleLocation(Control control)
        {
            control.Location = DpiHelper.ConvertUnits(control.Location);
        }


        protected Bitmap GetImage(int index)
        {
            return Res.GetImage(index, FormRatio);
        }

        /// <summary>
        /// Gets a scaled bitmap from specified FastReport assembly resource.
        /// </summary>
        /// <param name="resource">Resource name.</param>
        /// <returns>Bitmap object.</returns>
        protected Bitmap GetBitmap(string resource)
        {
            Bitmap bmp = ResourceLoader.GetBitmap(resource);
            return new Bitmap(bmp, new Size((int)(bmp.Width * (FormRatio / DpiHelper.Multiplier)), (int)(bmp.Height * (FormRatio / DpiHelper.Multiplier))));
        }

        /// <summary>
        /// Scale size of control
        /// </summary>
        /// <param name="control">Control wich must to b scaled</param>
        protected void ScaleSize(Control control)
        {
            control.Size = DpiHelper.ConvertUnits(control.Size);
        }
        #endregion

        #region private methods
        private bool IsParentContainer(Control c)
        {
            if (c.Parent != null)
            {
                if ((c.Parent is Panel || c.Parent is ContainerControl || c.Parent is TabPage || c.Parent is GroupBox || c.Parent is TabControl) && !(c.Parent is Form))
                    return true;
            }
            return false;
        }

        private void ScaleMinimumSize()
        {
            if (MinimumSize != Size.Empty)
            {
                if (this is BaseWizardForm)
                    this.MinimumSize = DpiHelper.ConvertUnits(new Size(400, 400));
                else
                    this.MinimumSize = DpiHelper.ConvertUnits(MinimumSize);
            }
        }

        private void CheckAfterScale(Control c)
        {
            if (c is ScalableCheckBox)
            {
                (c as ScalableCheckBox).CalcWidth();
            }
            else if (c is ScalableRadioButton)
            {
                (c as ScalableRadioButton).CalcWidth();
            }
        }

        /// <summary>
        /// Gets the control that has minimum bottom value.
        /// </summary>
        protected virtual Control GetMinimumBottom()
        {
            if (Controls.Count < 1)
                return null;
            Control minBControl = Controls[0];
            foreach (Control control in Controls)
            {
                if (control.Bottom > minBControl.Bottom)
                    minBControl = control;
            }
            return minBControl;
        }

        private void SpecificConside(Control control, Bools bools)
        {
            if (control is ListBox)
            {
                (control as ListBox).ItemHeight = DpiHelper.ConvertUnits((control as ListBox).ItemHeight);
            }
            else if (control is ListView)
            {
                ListView lv = control as ListView;
                foreach (ColumnHeader col in lv.Columns)
                {
                    col.Width = DpiHelper.ConvertUnits(col.Width);
                }
            }
            else if (control is PageControl)
            {
                PageControl pc = (control as PageControl);
                pc.SelectorTabHeight = DpiHelper.ConvertUnits(pc.SelectorTabHeight);

#if MSCHART
                if (pc.Parent is ChartEditorControl || pc.Parent is SeriesEditorControl)
                    ScaleFont(pc);
#endif
            }
            else if (control is PropertyGrid)
            {
                bools.NeedCheckControl = false;
                bools.NeedScaleThisControl = false;
                ScalePropertyGrid(control as PropertyGrid);
            }
            else if (control is StatusStrip)
            {
                bools.NeedCheckControl = false;
                bools.NeedScaleThisControl = false;
                StatusStrip ss = control as StatusStrip;
                ScaleToolStrip(ss);
            }
            else if (control is NumericUpDown)
            {
                NumericUpDown nud = control as NumericUpDown;
                var field = typeof(NumericUpDown).GetField("defaultButtonsWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                try
                {
                    field.SetValue(nud, (int)(16 * FormRatio));
                }
                catch { }
            }
            else if (control is ToolStrip)
            {
                bools.NeedScaleThisControl = false;
                bools.NeedCheckControl = false;

                ToolStrip ts = control as ToolStrip;

                ScaleToolStrip(ts);
            }
            else if (control is TextBoxElement)
            {
                TextBoxElement element = control as TextBoxElement;
                ScaleSize(element);
                ScaleLocation(element);
                bools.NeedScaleThisControl = false;
                bools.NeedCheckControl = false;

                element.UpdateImages();

                element.DoLayout();
            }
            else if (control is HatchComboBox)
            {
                bools.NeedScaleThisControl = false;
                HatchComboBox hcb = control as HatchComboBox;
                Size size = new Size(DpiHelper.ConvertUnits(hcb.Width), DpiHelper.ConvertUnits(hcb.Height));
                hcb.Font = DpiHelper.GetFontForTextBoxHeight(size.Height, hcb.Font);
                hcb.Location = DpiHelper.ConvertUnits(hcb.Location);
                hcb.Width = size.Width;
                hcb.ItemHeight = size.Height;
            }
            else if (control is BandComboBox)
            {
                BandComboBox cbx = control as BandComboBox;
                cbx.ItemHeight = DpiHelper.ConvertUnits(cbx.ItemHeight);
            }
            else if (control is Label && (control as Label).AutoSize == false)
            {
                ScaleAutosizeLabel(control);
            }
            else if (control is DataTreeView)
            {
                (control as DataTreeView).UpdateImages();
            }
            else if (control is ColorComboBox)
            {
                ColorComboBox cbb = control as ColorComboBox;
                cbb.UpdateControlRatio(DpiHelper.GetMultiplierForScreen(actualScreen));
            }
            else if (control is LineStyleControl)
            {
                (control as LineStyleControl).UpdateDpiDependencies(FormRatio);
            }
            else if (control is DataGridView)
            {
                DataGridView gridView = control as DataGridView;
                foreach (DataGridViewColumn column in gridView.Columns)
                {
                    column.Width = DpiHelper.ConvertUnits(column.Width);
                }
            }
        }

        private void ScaleToolStrip(ToolStrip ts)
        {
            ts.AutoSize = false;
            ts.Size = DpiHelper.ConvertUnits(ts.Size);
            ts.Location = DpiHelper.ConvertUnits(ts.Location);
            ts.Margin = DpiHelper.ConvertUnits(ts.Margin);
            ts.Padding = DpiHelper.ConvertUnits(ts.Padding);

            ts.ImageScalingSize = DpiHelper.ConvertUnits(ts.ImageScalingSize);
            ts.Font = ParseFontSize(ts.Font, 8);//DpiHelper.ConvertUnits(ts.Font, true);
            ts.Margin = DpiHelper.ConvertUnits(ts.Margin);
            for (int i = 0; i < ts.Items.Count; i++)
            {
                if (ts.Items[i] is ToolStripFontComboBox)
                {
                    ToolStripFontComboBox tscb = (ts.Items[i] as ToolStripFontComboBox);
                    DpiHelper.RescaleWithNewDpi(() =>
                    {
                        tscb.UpdateDpiDependencies();
                    }, tempMultiplier);
                    if (!dynamicScaled)
                        tscb.Font = DpiHelper.ConvertUnits(tscb.Font, true);
                    else
                        DpiHelper.RescaleWithNewDpi(() =>
                        {
                            tscb.Font = DpiHelper.ConvertUnits(tscb.Font, true);
                        }, tempMultiplier);
                    tscb.Size = DpiHelper.ConvertUnits(tscb.Size);
                }
                else if (ts.Items[i] is ToolStripFontSizeComboBox)
                {
                    ToolStripFontSizeComboBox tscb = (ts.Items[i] as ToolStripFontSizeComboBox);
                    DpiHelper.RescaleWithNewDpi(() =>
                    {
                        tscb.UpdateDpiDependencies();
                    }, tempMultiplier);
                    if (!dynamicScaled)
                        tscb.Font = DpiHelper.ConvertUnits(tscb.Font, true);
                    else
                        DpiHelper.RescaleWithNewDpi(() =>
                        {
                            tscb.Font = DpiHelper.ConvertUnits(tscb.Font, true);
                        }, tempMultiplier);
                    tscb.Size = DpiHelper.ConvertUnits(tscb.Size);
                }
                else if (ts.Items[i] is ToolStripComboBox)
                {
                    ToolStripComboBox tscb = (ts.Items[i] as ToolStripComboBox);
                    if (!dynamicScaled)
                        tscb.Font = DpiHelper.ConvertUnits(tscb.Font, true);
                    else
                        DpiHelper.RescaleWithNewDpi(() =>
                        {
                            tscb.Font = DpiHelper.ConvertUnits(tscb.Font, true);
                        }, tempMultiplier);
                    tscb.Size = DpiHelper.ConvertUnits(tscb.Size);
                }
            }
        }

        private void ScalePropertyGrid(PropertyGrid propertyGrid)
        {
            propertyGrid.Font = DpiHelper.ConvertUnits(propertyGrid.Font, true);

            FieldInfo fieldInfo = typeof(PropertyGrid).GetField("largeButtonSize", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
            Size propSize = new Size((int)(16 * FormRatio), (int)(16 * FormRatio));
            try
            {
                fieldInfo.SetValue(propertyGrid, propSize);
                propertyGrid.LargeButtons = false;
                propertyGrid.LargeButtons = true;
            }
            catch { }
            // need to call resize toolstrip's method

            ScaleSize(propertyGrid);
            ScaleLocation(propertyGrid);
        }

        #endregion

    }
}
