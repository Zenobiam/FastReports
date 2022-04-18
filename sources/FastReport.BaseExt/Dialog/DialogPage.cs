using System;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.TypeConverters;
using System.Windows.Forms;
using System.Drawing.Design;
#if !FRCORE
using FastReport.Forms;
#endif
#if !MONO && !FRCORE
using FastReport.DevComponents;
#endif
#if MONO
using FastReport.MonoCap;
#endif

namespace FastReport.Dialog
{
    /// <summary>
    /// Represents the special kind of report page that wraps the <see cref="System.Windows.Forms.Form"/>
    /// and used to display dialog forms.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="Controls"/> property to add/remove controls to/from a dialog form.
    /// <para/>If you set the <b>Visible</b> property to <b>false</b>, this dialog form will be
    /// skippen when you run a report.
    /// </remarks>
    /// <example>This example shows how to create a dialog form with one button in code.
    /// <code>
    /// DialogPage form = new DialogPage();
    /// // set the width and height in pixels
    /// form.Width = 200;
    /// form.Height = 200;
    /// form.Name = "Form1";
    /// // create a button
    /// ButtonControl button = new ButtonControl();
    /// button.Location = new Point(20, 20);
    /// button.Size = new Size(75, 25);
    /// button.Text = "The button";
    /// // add the button to the form
    /// form.Controls.Add(button);
    /// </code>
    /// </example>
    public partial class DialogPage : PageBase, IParent
    {
        #region Fields
        private ButtonControl acceptButton;
        private ButtonControl cancelButton;
#if !FRCORE
        private ScaledSupportingForm form;
#else

        private Form form;
#endif
        private DialogComponentCollection controls;
        private string loadEvent;
        private string formClosedEvent;
        private string formClosingEvent;
        private string shownEvent;
        private string resizeEvent;
        private string paintEvent;
        private DialogControl errorControl;
        private Color errorControlBackColor;
        private Timer errorControlTimer;
        private int errorControlTimerTickCount;
        private bool activeInWeb;
        private bool wasScaled = false;
        #endregion

        #region Properties
        /// <summary>
        /// Occurs before a form is displayed for the first time.
        /// Wraps the <see cref="System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        public event EventHandler Load;

        /// <summary>
        /// Occurs after the form is closed.
        /// Wraps the <see cref="System.Windows.Forms.Form.FormClosed"/> event.
        /// </summary>
        public event FormClosedEventHandler FormClosed;

        /// <summary>
        /// Occurs before the form is closed.
        /// Wraps the <see cref="System.Windows.Forms.Form.FormClosing"/> event.
        /// </summary>
        public event FormClosingEventHandler FormClosing;

        /// <summary>
        /// Occurs whenever the form is first displayed.
        /// Wraps the <see cref="System.Windows.Forms.Form.Shown"/> event.
        /// </summary>
        public event EventHandler Shown;

        /// <summary>
        /// Occurs when the form is resized.
        /// Wraps the <see cref="System.Windows.Forms.Control.Resize"/> event.
        /// </summary>
        public event EventHandler Resize;

        /// <summary>
        /// Occurs when the form is redrawn.
        /// Wraps the <see cref="System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        public event PaintEventHandler Paint;

        /// <summary>
        /// Gets an internal <b>Form</b>.
        /// </summary>
        [Browsable(false)]
#if !FRCORE
        public ScaledSupportingForm Form
        {
            get { return form; }
        }
#else
        public Form Form
        {
            get { return form; }
        }
#endif

        /// <summary>
        /// Gets or sets an active state in Web application.
        /// </summary>
        [Browsable(false)]
        public bool ActiveInWeb
        {
            get { return activeInWeb; }
            set { activeInWeb = value; }
        }

        /// <summary>
        /// Gets or sets the button on the form that is clicked when the user presses the ENTER key.
        /// Wraps the <see cref="System.Windows.Forms.Form.AcceptButton"/> property.
        /// </summary>
        [Category("Misc")]
        [Editor("FastReport.TypeEditors.PageComponentRefEditor, FastReport", typeof(UITypeEditor))]
        [TypeConverter(typeof(FastReport.TypeConverters.ComponentRefConverter))]
        public ButtonControl AcceptButton
        {
            get { return acceptButton; }
            set
            {
                if (acceptButton != value)
                {
                    if (acceptButton != null)
                        acceptButton.Disposed -= new EventHandler(AcceptButton_Disposed);
                    if (value != null)
                        value.Disposed += new EventHandler(AcceptButton_Disposed);
                }
                acceptButton = value;
                Form.AcceptButton = value == null ? null : value.Button;
            }
        }

        /// <summary>
        /// Gets or sets the button control that is clicked when the user presses the ESC key.
        /// Wraps the <see cref="System.Windows.Forms.Form.CancelButton"/> property.
        /// </summary>
        [Category("Misc")]
        [Editor("FastReport.TypeEditors.PageComponentRefEditor, FastReport", typeof(UITypeEditor))]
        [TypeConverter(typeof(FastReport.TypeConverters.ComponentRefConverter))]
        public ButtonControl CancelButton
        {
            get { return cancelButton; }
            set
            {
                if (cancelButton != value)
                {
                    if (cancelButton != null)
                        cancelButton.Disposed -= new EventHandler(CancelButton_Disposed);
                    if (value != null)
                        value.Disposed += new EventHandler(CancelButton_Disposed);
                }
                cancelButton = value;
                Form.CancelButton = value == null ? null : value.Button;
            }
        }

        /// <summary>
        /// Gets or sets the background color for the form.
        /// Wraps the <see cref="System.Windows.Forms.Form.BackColor"/> property.
        /// </summary>
        [Category("Appearance")]
        [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
        public Color BackColor
        {
            get { return Form.BackColor; }
            set
            {
                Form.BackColor = value;
                ResetFormBitmap();
            }
        }

        /// <summary>
        /// Gets or sets the font of the text displayed by the control.
        /// Wraps the <see cref="System.Windows.Forms.Control.Font"/> property.
        /// </summary>
        [Category("Appearance")]
        public Font Font
        {
            get { return Form.Font; }
            set { Form.Font = value; }
        }

        /// <summary>
        /// Gets or sets the border style of the form.
        /// Wraps the <see cref="System.Windows.Forms.Form.FormBorderStyle"/> property.
        /// </summary>
        [DefaultValue(FormBorderStyle.FixedDialog)]
        [Category("Appearance")]
        public FormBorderStyle FormBorderStyle
        {
            get { return Form.FormBorderStyle; }
            set
            {
                Form.FormBorderStyle = value;
                ResetFormBitmap();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts.
        /// Wraps the <see cref="System.Windows.Forms.Control.RightToLeft"/> property.
        /// </summary>
        [DefaultValue(RightToLeft.No)]
        [Category("Appearance")]
        public RightToLeft RightToLeft
        {
            get { return Form.RightToLeft; }
            set
            {
                Form.RightToLeft = value;
                ResetFormBitmap();
            }
        }

        /// <summary>
        /// Gets or sets the text associated with this form.
        /// Wraps the <see cref="System.Windows.Forms.Form.Text"/> property.
        /// </summary>
        [Category("Appearance")]
        public string Text
        {
            get { return Form.Text; }
            set
            {
                Form.Text = value;
                ResetFormBitmap();
            }
        }

        /// <summary>
        /// Gets or sets a script method name that will be used to handle the 
        /// <see cref="Load"/> event.
        /// </summary>
        [Category("Events")]
        public string LoadEvent
        {
            get { return loadEvent; }
            set { loadEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script method name that will be used to handle the 
        /// <see cref="FormClosed"/> event.
        /// </summary>
        [Category("Events")]
        public string FormClosedEvent
        {
            get { return formClosedEvent; }
            set { formClosedEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script method name that will be used to handle the 
        /// <see cref="FormClosing"/> event.
        /// </summary>
        [Category("Events")]
        public string FormClosingEvent
        {
            get { return formClosingEvent; }
            set { formClosingEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script method name that will be used to handle the 
        /// <see cref="Shown"/> event.
        /// </summary>
        [Category("Events")]
        public string ShownEvent
        {
            get { return shownEvent; }
            set { shownEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script method name that will be used to handle the 
        /// <see cref="Resize"/> event.
        /// </summary>
        [Category("Events")]
        public string ResizeEvent
        {
            get { return resizeEvent; }
            set { resizeEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script method name that will be used to handle the 
        /// <see cref="Paint"/> event.
        /// </summary>
        [Category("Events")]
        public string PaintEvent
        {
            get { return paintEvent; }
            set { paintEvent = value; }
        }

        /// <summary>
        /// Gets the collection of controls contained within the form.
        /// </summary>
        [Browsable(false)]
        public DialogComponentCollection Controls
        {
            get { return controls; }
        }

        /// <inheritdoc/>
        public override float Width
        {
            get { return Form.Width; }
            set
            {
                if (!IsDesigning || !HasRestriction(Restrictions.DontResize))
                    Form.Width = (int)value;
                ResetFormBitmap();
            }
        }

        /// <inheritdoc/>
        public override float Height
        {
            get { return Form.Height; }
            set
            {
                if (!IsDesigning || !HasRestriction(Restrictions.DontResize))
                    Form.Height = (int)value;
                ResetFormBitmap();
            }
        }

        /// <inheritdoc/>
        public override SizeF ClientSize
        {
            get { return new SizeF(Form.ClientSize.Width, Form.ClientSize.Height); }
            set { Form.ClientSize = new Size((int)value.Width, (int)value.Height); }
        }
#endregion

#region Private Methods


        private string CreateButtonName(string baseName)
        {
            if (Report.FindObject(baseName) == null)
                return baseName;

            int i = 1;
            while (Report.FindObject(baseName + i.ToString()) != null)
            {
                i++;
            }
            return baseName + i.ToString();
        }

        private void AcceptButton_Disposed(object sender, EventArgs e)
        {
            AcceptButton = null;
        }

        private void CancelButton_Disposed(object sender, EventArgs e)
        {
            CancelButton = null;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            OnLoad(e);
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            OnFormClosed(e);
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            OnFormClosing(e);
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            OnShown(e);
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            OnResize(e);
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
        }

        private void SetErrorControl(DialogControl control)
        {
            errorControl = control;
            if (control != null)
            {
                control.Focus();
                if (errorControlTimer == null)
                {
                    errorControlTimerTickCount = 0;
                    errorControlBackColor = errorControl.BackColor;
                    errorControlTimer = new Timer();
                    errorControlTimer.Interval = 300;
                    errorControlTimer.Tick += new EventHandler(FErrorControlTimer_Tick);
                    errorControlTimer.Start();
                }
            }
        }

        private void FErrorControlTimer_Tick(object sender, EventArgs e)
        {
            errorControl.BackColor = errorControlTimerTickCount % 2 == 0 ? Color.Red : errorControlBackColor;

            errorControlTimerTickCount++;
            if (errorControlTimerTickCount > 5)
            {
                errorControlTimer.Stop();
                errorControlTimer.Dispose();
                errorControlTimer = null;
            }
        }
#endregion

#region Protected Methods
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                Form.Dispose();
        }
#endregion

#region IParent
        /// <inheritdoc/>
        public virtual void GetChildObjects(ObjectCollection list)
        {
            foreach (DialogComponentBase c in controls)
            {
                list.Add(c);
            }
        }

        /// <inheritdoc/>
        public virtual bool CanContain(Base child)
        {
            return (child is DialogComponentBase);
        }

        /// <inheritdoc/>
        public virtual void AddChild(Base child)
        {
            if (child is DialogComponentBase)
                controls.Add(child as DialogComponentBase);
        }

        /// <inheritdoc/>
        public virtual void RemoveChild(Base child)
        {
            if (child is DialogComponentBase)
                controls.Remove(child as DialogComponentBase);
        }

        /// <inheritdoc/>
        public virtual int GetChildOrder(Base child)
        {
            return controls.IndexOf(child as DialogComponentBase);
        }

        /// <inheritdoc/>
        public virtual void SetChildOrder(Base child, int order)
        {
            int oldOrder = child.ZOrder;
            if (oldOrder != -1 && order != -1 && oldOrder != order)
            {
                if (order > controls.Count)
                    order = controls.Count;
                if (oldOrder <= order)
                    order--;
                controls.Remove(child as DialogComponentBase);
                controls.Insert(order, child as DialogComponentBase);
            }
        }

        /// <inheritdoc/>
        public virtual void UpdateLayout(float dx, float dy)
        {
            // do nothing
        }
#endregion

#region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            BaseAssign(source);
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            DialogPage c = writer.DiffObject as DialogPage;
            base.Serialize(writer);

            if (AcceptButton != c.AcceptButton)
                writer.WriteRef("AcceptButton", AcceptButton);
            if (CancelButton != c.CancelButton)
                writer.WriteRef("CancelButton", CancelButton);
            if (BackColor != c.BackColor)
                writer.WriteValue("BackColor", BackColor);
            if ((writer.SerializeTo != SerializeTo.Preview || !Font.Equals(c.Font)) && writer.ItemName != "inherited")
                writer.WriteValue("Font", Font);
            if (FormBorderStyle != c.FormBorderStyle)
                writer.WriteValue("FormBorderStyle", FormBorderStyle);
            if (RightToLeft != c.RightToLeft)
                writer.WriteValue("RightToLeft", RightToLeft);
            if (Text != c.Text)
                writer.WriteStr("Text", Text);

            if (LoadEvent != c.LoadEvent)
                writer.WriteStr("LoadEvent", LoadEvent);
            if (FormClosedEvent != c.FormClosedEvent)
                writer.WriteStr("FormClosedEvent", FormClosedEvent);
            if (FormClosingEvent != c.FormClosingEvent)
                writer.WriteStr("FormClosingEvent", FormClosingEvent);
            if (ShownEvent != c.ShownEvent)
                writer.WriteStr("ShownEvent", ShownEvent);
            if (ResizeEvent != c.ResizeEvent)
                writer.WriteStr("ResizeEvent", ResizeEvent);
            if (PaintEvent != c.PaintEvent)
                writer.WriteStr("PaintEvent", PaintEvent);

            writer.WriteValue("Width", Width);
            writer.WriteValue("Height", Height);
        }

        internal void InitializeControls()
        {
            Form.Hide();
#if !FRCORE
            if(!wasScaled)
            {
                Form.DisableAnchors(Form.Controls);
                Form.ClientSize = DpiHelper.ConvertUnits(Form.ClientSize);
                Form.EnableAnchors();
            }
#endif
            Form.StartPosition = FormStartPosition.CenterScreen;
            Form.Load += new EventHandler(Form_Load);
            Form.FormClosed += new FormClosedEventHandler(Form_FormClosed);
            Form.FormClosing += new FormClosingEventHandler(Form_FormClosing);
            Form.Shown += new EventHandler(Form_Shown);
            Form.Resize += new EventHandler(Form_Resize);
            Form.Paint += new PaintEventHandler(Form_Paint);

            ObjectCollection allObjects = AllObjects;
            foreach (Base c in allObjects)
            {
                if (c is DialogControl)
                {
                    (c as DialogControl).InitializeControl();
                    if (!wasScaled)
                        (c as DialogControl).ScaleControl();
                }
            }
#if !FRCORE
            if (!wasScaled)
                Form.AddHeight();
#endif
            wasScaled = true;
        }

        internal void FinalizeControls()
        {
            Form.Load -= new EventHandler(Form_Load);
            Form.FormClosed -= new FormClosedEventHandler(Form_FormClosed);
            Form.FormClosing -= new FormClosingEventHandler(Form_FormClosing);
            Form.Shown -= new EventHandler(Form_Shown);
            Form.Resize -= new EventHandler(Form_Resize);
            Form.Paint -= new PaintEventHandler(Form_Paint);

            ObjectCollection allObjects = AllObjects;
            foreach (Base c in allObjects)
            {
                if (c is DialogControl)
                    (c as DialogControl).FinalizeControl();
            }
        }

        /// <summary>
        /// Shows the form as a modal dialog box with the currently active window set as its owner.
        /// Wraps the <see cref="System.Windows.Forms.Form.ShowDialog()"/> method.
        /// </summary>
        /// <returns>One of the <b>DialogResult</b> values.</returns>
        public DialogResult ShowDialog()
        {
            try
            {
                InitializeControls();
                return Form.ShowDialog();
            }
            finally
            {
                FinalizeControls();
            }
        }

        /// <summary>
        /// This method fires the <b>Load</b> event and the script code connected to the <b>LoadEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public void OnLoad(EventArgs e)
        {
            if (Load != null)
                Load(this, e);
            InvokeEvent(LoadEvent, e);
        }

        /// <summary>
        /// This method fires the <b>FormClosed</b> event and the script code connected to the <b>FormClosedEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public void OnFormClosed(FormClosedEventArgs e)
        {
            if (FormClosed != null)
                FormClosed(this, e);
            InvokeEvent(FormClosedEvent, e);
        }

        /// <summary>
        /// This method fires the <b>FormClosing</b> event and the script code connected to the <b>FormClosingEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public void OnFormClosing(FormClosingEventArgs e)
        {
            if (form.DialogResult == DialogResult.OK)
            {
                // filter data
                SetErrorControl(null);
                foreach (Base c in AllObjects)
                {
                    DataFilterBaseControl c1 = c as DataFilterBaseControl;
                    if (c1 != null && c1.Enabled)
                    {
                        try
                        {
                            if (c1.AutoFilter)
                                c1.FilterData();
                            c1.SetReportParameter();
                        }
                        catch
                        {
                            SetErrorControl(c1);
                        }
                    }

                    if (errorControl != null)
                    {
                        e.Cancel = true;
                        break;
                    }
                }
            }

            if (FormClosing != null)
                FormClosing(this, e);
            InvokeEvent(FormClosingEvent, e);
        }

        /// <summary>
        /// This method fires the <b>Shown</b> event and the script code connected to the <b>ShownEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public void OnShown(EventArgs e)
        {
            if (Shown != null)
                Shown(this, e);
            InvokeEvent(ShownEvent, e);
        }

        /// <summary>
        /// This method fires the <b>Resize</b> event and the script code connected to the <b>ResizeEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public void OnResize(EventArgs e)
        {
            if (Resize != null)
                Resize(this, e);
            InvokeEvent(ResizeEvent, e);
        }

        /// <summary>
        /// This method fires the <b>Paint</b> event and the script code connected to the <b>PaintEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public void OnPaint(PaintEventArgs e)
        {
            if (Paint != null)
                Paint(this, e);
            InvokeEvent(PaintEvent, e);
        }

#endregion

        /// <summary>
        /// Initializes a new instance of the <b>DialogPage</b> class. 
        /// </summary>
        public DialogPage()
        {
            controls = new DialogComponentCollection(this);
#if !FRCORE
            form = new ScaledSupportingForm();
#else
            form = new Form();
#endif
            form.ShowIcon = false;
            form.ShowInTaskbar = false;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.Font = DrawUtils.DefaultFont;
            activeInWeb = false;
            BaseName = "Form";
        }
    }
}