using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Controls;
#if !MONO
using System.Runtime.InteropServices;
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents.DotNetBar.Controls;
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design.ToolWindows
{
    /// <summary>
    /// Represents the "Properties" window.
    /// </summary>
    public class PropertiesWindow : ToolWindowBase
    {
        #region Fields
        private bool updating;
        private FRPropertyGrid propertiesGrid;
        private FRPropertyGrid eventsGrid;
        private PageControl pageControl;
        private bool saving;
#if !MONO
        private ComboBoxEx cbxObjects;
        private Bar toolbar;
        private ButtonItem btnCategory;
        private ButtonItem btnAlphabetical;
        private ButtonItem btnProperties;
        private ButtonItem btnEvents;
#else
        private ComboBox cbxObjects;
        private ToolStrip toolbar;
        private ToolStripButton btnCategory;
        private ToolStripButton btnAlphabetical;
        private ToolStripButton btnProperties;
        private ToolStripButton btnEvents;
#endif
        #endregion

        #region Public properties

        /// <inheritdoc/>
        public bool Saving
        {
            get { return saving; }
            set { saving = value; }
        }
        #endregion

#region Private Methods

#if !MONO
        private void Reinit(float ratio = 0)
        {
            if (ratio == 0)
                ratio = DpiHelper.Multiplier;
            base.ReinitDpiSize();

            btnCategory.Image = Res.GetImage(69, ratio);
            btnAlphabetical.Image = Res.GetImage(67, ratio);
            btnProperties.Image = Res.GetImage(78, ratio);
            btnEvents.Image = Res.GetImage(79, ratio);
            Image = Res.GetImage(68, ratio);

            toolbar.UpdateDpiDependencies();
            toolbar.Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
            using (Bitmap bmp = new Bitmap(1, 1))
            using (StringFormat sf = new StringFormat())
            {
                Graphics g = Graphics.FromImage(bmp);
                cbxObjects.ItemHeight = (int)g.MeasureString("Wg", DpiHelper.ConvertUnits(DrawUtils.Default96Font)).Height + DpiHelper.ConvertUnits(2);
            }
        }
#endif

        private void cbxObjects_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Graphics g = e.Graphics;
            if (e.Index >= 0)
            {
                Base c = cbxObjects.Items[e.Index] as Base;
                using (Font f = new Font(e.Font, FontStyle.Bold))
                {
                    SizeF sz = TextRenderer.MeasureText(c.Name, f);
                    if (c is Report)
                    {
                        TextRenderer.DrawText(g, c.ClassName, f, new Point(e.Bounds.X, e.Bounds.Y), e.ForeColor);
                    }
                    else
                    {
                        TextRenderer.DrawText(g, c.Name, f, e.Bounds,
                          e.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                        TextRenderer.DrawText(g, c.ClassName, e.Font,
                          new Rectangle(e.Bounds.X + (int)sz.Width, e.Bounds.Y, e.Bounds.Width - (int)sz.Width, e.Bounds.Height),
                          e.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                    }
                }
            }
            else
            {
                SelectedObjectCollection selection = Designer.SelectedObjects;
                string s = selection.Count == 0 ? "" : selection.Count > 1 ?
                  String.Format(Res.Get("Designer,ToolWindow,Properties,NObjectsSelected"), selection.Count) :
                  selection[0].Name;
                TextRenderer.DrawText(g, s, e.Font, new Point(e.Bounds.X, e.Bounds.Y), e.ForeColor);
            }
        }

        private void cbxObjects_SelectedValueChanged(object sender, EventArgs e)
        {
            if (updating)
                return;
            Base c = cbxObjects.SelectedItem as Base;
            if (!(c is Report) && Designer.ActiveReportTab != null)
            {
                updating = true;
                Designer.ActiveReportTab.ActivePage = c.Page;
                updating = false;
            }
            if (Designer.SelectedObjects != null)
            {
                Designer.SelectedObjects.Clear();
                Designer.SelectedObjects.Add(c);
                Designer.SelectionChanged(null);
            }
        }

        private void grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Designer.SetModified(null, "Change");
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            propertiesGrid.PropertySort = PropertySort.Categorized;
            eventsGrid.PropertySort = propertiesGrid.PropertySort;
            btnCategory.Checked = true;
            btnAlphabetical.Checked = false;
        }

        private void btnAlphabetical_Click(object sender, EventArgs e)
        {
            propertiesGrid.PropertySort = PropertySort.Alphabetical;
            eventsGrid.PropertySort = propertiesGrid.PropertySort;
            btnCategory.Checked = false;
            btnAlphabetical.Checked = true;
        }

        private void btnProperties_Click(object sender, EventArgs e)
        {
            pageControl.ActivePageIndex = 0;
            btnProperties.Checked = true;
            btnEvents.Checked = false;
        }

        private void btnEvents_Click(object sender, EventArgs e)
        {
            pageControl.ActivePageIndex = 1;
            btnProperties.Checked = false;
            btnEvents.Checked = true;
        }
#endregion

#region Public Methods
        /// <inheritdoc/>
        public override void SaveState()
        {
            XmlItem xi = Config.Root.FindItem("Designer").FindItem(Name);
            xi.SetProp("Sort", propertiesGrid.PropertySort.ToString());
            xi.SetProp("TabIndex", pageControl.ActivePageIndex.ToString());
        }

        /// <inheritdoc/>
        public override void RestoreState()
        {
            XmlItem xi = Config.Root.FindItem("Designer").FindItem(Name);
            propertiesGrid.PropertySort = xi.GetProp("Sort") == "Alphabetical" ? PropertySort.Alphabetical : PropertySort.CategorizedAlphabetical;
            eventsGrid.PropertySort = propertiesGrid.PropertySort;
            string tabIndexProp = xi.GetProp("TabIndex");
            if (!String.IsNullOrEmpty(tabIndexProp))
            {
                int tabIndex = 0;
                Int32.TryParse(tabIndexProp, out tabIndex);
                pageControl.ActivePageIndex = tabIndex;
                btnProperties.Checked = (tabIndex == 0);
                btnEvents.Checked = (tabIndex == 1);
            }
            btnCategory.Checked = propertiesGrid.PropertySort == PropertySort.CategorizedAlphabetical;
            btnAlphabetical.Checked = !btnCategory.Checked;

        }

        /// <inheritdoc/>
        public override void SelectionChanged()
        {
            if (updating)
                return;
            // prevent fire SelectedValueChanged
            updating = true;
            try
            {
                if (Designer.SelectedObjects != null && Designer.SelectedObjects.Count == 1)
                {
                    cbxObjects.SelectedIndex = cbxObjects.Items.IndexOf(Designer.SelectedObjects[0]);
                    cbxObjects.Refresh();
                    propertiesGrid.SelectedObjects = Designer.SelectedObjects.ToArray();
                    eventsGrid.SelectedObjects = propertiesGrid.SelectedObjects;
                }
                else
                {
                    cbxObjects.SelectedItem = null;
                    if (Designer.SelectedObjects != null)
                        propertiesGrid.SelectedObjects = Designer.SelectedObjects.ToArray();
                    else
                        propertiesGrid.SelectedObjects = null;
                    eventsGrid.SelectedObjects = propertiesGrid.SelectedObjects;
                    cbxObjects.Refresh();
                }
            }
            finally
            {
                updating = false;
            }
        }

        private void EnumComponents(Base rootComponent, SortedList<string, Base> list)
        {
            string name = rootComponent is Report ? "" : rootComponent.Name;
            if (!list.ContainsKey(name))
                list.Add(name, rootComponent);

            if (rootComponent.HasFlag(Flags.CanShowChildrenInReportTree))
            {
                foreach (Base component in rootComponent.ChildObjects)
                    EnumComponents(component, list);
            }
        }

        /// <inheritdoc/>
        public override void UpdateContent()
        {
            cbxObjects.BeginUpdate();
            try
            {
                if (!Saving)
                {
                    cbxObjects.Items.Clear();
                    if (Designer.ActiveReport != null)
                    {
                        Report report = Designer.ActiveReport.Report;
                        SortedList<string, Base> sl = new SortedList<string, Base>();
                        EnumComponents(report, sl);
                        foreach (Base c in sl.Values)
                        {
                            cbxObjects.Items.Add(c);
                        }
                    }
                    SelectionChanged();
                }
            }
            finally
            {
                cbxObjects.EndUpdate();
            }
        }

        /// <inheritdoc/>
        public override void Lock()
        {
            base.Lock();
            propertiesGrid.SelectedObjects = null;
            eventsGrid.SelectedObjects = propertiesGrid.SelectedObjects;
        }

        /// <inheritdoc/>
        public override void Localize()
        {
            Text = Res.Get("Designer,ToolWindow,Properties");
            UpdateContent();
        }

        /// <inheritdoc/>
        public override void UpdateUIStyle()
        {
            base.UpdateUIStyle();
#if !MONO
            toolbar.Style = UIStyleUtils.GetDotNetBarStyle(Designer.UIStyle);
            cbxObjects.Style = toolbar.Style;
            Color color = UIStyleUtils.GetControlColor(Designer.UIStyle);
            ParentControl.BackColor = color;
#else
            toolbar.Renderer = UIStyleUtils.GetToolStripRenderer(Designer.UIStyle);
            propertiesGrid.Style = Designer.UIStyle;
            eventsGrid.Style = propertiesGrid.Style;
            Color color = UIStyleUtils.GetColorTable(Designer.UIStyle).ControlBackColor;
#endif
            propertiesGrid.BackColor = color;
            propertiesGrid.LineColor = color;
            propertiesGrid.HelpBackColor = color;
            eventsGrid.BackColor = color;
            eventsGrid.LineColor = color;
            eventsGrid.HelpBackColor = color;

        }

#if !MONO
        /// <inheritdoc/>
        public override void ReinitDpiSize()
        {
            if (!Bar.Docked)
                return;
            Reinit();
        }

        ///<inheritdoc/>
        public override void CallReinit(float ratio)
        {
            Reinit(ratio);
        }
#endif

#if !MONO
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
#endif

        internal void TypeChar(char c)
        {
#if !MONO
            FRPropertyGrid grid = pageControl.ActivePageIndex == 0 ? propertiesGrid : eventsGrid;
            grid.Focus();
            SendMessage(grid.ActiveControl.Handle, 0x0102, (int)c, IntPtr.Zero);
#endif
        }
#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesWindow"/> class with default settings.
        /// </summary>
        /// <param name="designer">The report designer.</param>
        public PropertiesWindow(Designer designer)
            : base(designer)
        {
            Name = "PropertiesWindow";
#if !MONO
            Image = Res.GetImage(68);
            Shortcut = eShortcut.F4;

            cbxObjects = new ComboBoxEx();
            cbxObjects.Dock = DockStyle.Top;
            cbxObjects.DisableInternalDrawing = true;
            cbxObjects.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxObjects.DrawMode = DrawMode.OwnerDrawFixed;
            //cbxObjects.ItemHeight = DrawUtils.DefaultItemHeight + 2;
            using (Bitmap bmp = new Bitmap(1, 1))
            using (StringFormat sf = new StringFormat())
            {
                Graphics g = Graphics.FromImage(bmp);
                cbxObjects.ItemHeight = (int)g.MeasureString("Wg", DpiHelper.ConvertUnits(DrawUtils.Default96Font)).Height + DpiHelper.ConvertUnits(2);
            }

            cbxObjects.DrawItem += new DrawItemEventHandler(cbxObjects_DrawItem);
            cbxObjects.SelectedValueChanged += new EventHandler(cbxObjects_SelectedValueChanged);

            toolbar = new Bar();
            toolbar.Dock = DockStyle.Top;
            toolbar.Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
            toolbar.RoundCorners = false;

            btnCategory = new ButtonItem();
            btnCategory.AutoCheckOnClick = true;
            btnCategory.OptionGroup = "1";
            btnCategory.Image = Res.GetImage(69);
            btnCategory.Click += new EventHandler(btnCategory_Click);
            btnAlphabetical = new ButtonItem();
            btnAlphabetical.AutoCheckOnClick = true;
            btnAlphabetical.OptionGroup = "1";
            btnAlphabetical.Image = Res.GetImage(67);
            btnAlphabetical.Click += new EventHandler(btnAlphabetical_Click);
            btnProperties = new ButtonItem();
            btnProperties.BeginGroup = true;
            btnProperties.AutoCheckOnClick = true;
            btnProperties.OptionGroup = "2";
            btnProperties.Checked = true;
            btnProperties.Image = Res.GetImage(78);
            btnProperties.Click += new EventHandler(btnProperties_Click);
            btnEvents = new ButtonItem();
            btnEvents.AutoCheckOnClick = true;
            btnEvents.OptionGroup = "2";
            btnEvents.Image = Res.GetImage(79);
            btnEvents.Click += new EventHandler(btnEvents_Click);

            toolbar.Items.AddRange(new ButtonItem[] { btnCategory, btnAlphabetical, btnProperties, btnEvents });
#else
            cbxObjects = new ComboBox();
            cbxObjects.Dock = DockStyle.Top;
            cbxObjects.FlatStyle = FlatStyle.Popup;
            cbxObjects.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxObjects.DrawMode = DrawMode.OwnerDrawFixed;
            cbxObjects.ItemHeight = DrawUtils.DefaultItemHeight + 2;
            cbxObjects.DrawItem += cbxObjects_DrawItem;
            cbxObjects.SelectedValueChanged += cbxObjects_SelectedValueChanged;

            toolbar = new ToolStrip();
            toolbar.Dock = DockStyle.Top;
            toolbar.Font = DrawUtils.DefaultFont;
            toolbar.GripStyle = ToolStripGripStyle.Hidden;

            btnCategory = new ToolStripButton("", Res.GetImage(69), btnCategory_Click);
            btnCategory.Checked = true;
            // mono fix
            btnCategory.AutoSize = false;
            btnCategory.Size = new Size(23, 22);
            btnAlphabetical = new ToolStripButton("", Res.GetImage(67), btnAlphabetical_Click);
            btnProperties = new ToolStripButton("", Res.GetImage(78), btnProperties_Click);
            btnProperties.Checked = true;
            btnEvents = new ToolStripButton("", Res.GetImage(79), btnEvents_Click);

            toolbar.Items.AddRange(new ToolStripItem[] { 
        btnCategory, btnAlphabetical, new ToolStripSeparator(),
        btnProperties, btnEvents });
#endif

            pageControl = new PageControl();
            pageControl.Dock = DockStyle.Fill;
            PageControlPage propPage = new PageControlPage();
            propPage.Dock = DockStyle.Fill;
            pageControl.Pages.Add(propPage);
            PageControlPage eventsPage = new PageControlPage();
            eventsPage.Dock = DockStyle.Fill;
            pageControl.Pages.Add(eventsPage);

            propertiesGrid = new FRPropertiesGrid();
            propertiesGrid.ToolbarVisible = false;
            propertiesGrid.Dock = DockStyle.Fill;
            propertiesGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(grid_PropertyValueChanged);
            propertiesGrid.Parent = propPage;

            eventsGrid = new FREventsGrid();
            eventsGrid.ToolbarVisible = false;
            eventsGrid.Dock = DockStyle.Fill;
            eventsGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(grid_PropertyValueChanged);
            eventsGrid.Parent = eventsPage;

#if !MONO
            ParentControl.Controls.AddRange(new Control[] { pageControl, toolbar, cbxObjects });
#else
            Controls.AddRange(new Control[] { pageControl, toolbar, cbxObjects });
#endif
            Localize();
        }
    }

}
