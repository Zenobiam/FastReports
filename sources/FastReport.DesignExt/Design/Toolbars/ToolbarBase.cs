using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Forms;
using FastReport.Design.PageDesigners;
#if !MONO
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design.Toolbars
{
    /// <summary>
    /// Base class for all designer toolbars.
    /// </summary>
    /// <remarks>
    /// Use this class to write own designer's toolbar. To do this:
    /// <para>- in the constructor, set the <b>Name</b> property and create toolbar buttons. 
    /// The <b>Name</b> will be used to restore toolbar's state;</para>
    /// <para>- override the <b>SelectionChanged</b> method. This method is called when current selection
    /// is changed. In this method, you should update buttons state to reflect the current selection. 
    /// Selected objects can be accessed via <b>Designer.SelectedObjects</b> property;</para>
    /// <para>- override the <b>UpdateContent</b> method. This method is called when the report
    /// content was changed. Typically you need to do the same actions in <b>SelectionChanged</b> and
    /// <b>UpdateContent</b> methods;</para>
    /// <para>- to register a toolbar, add its type to the <see cref="DesignerPlugins"/> global collection:
    /// <code>
    /// DesignerPlugins.Add(typeof(MyToolbar));
    /// </code>
    /// </para>
    /// </remarks>
    [ToolboxItem(false)]
#if !MONO
    public class ToolbarBase : Bar, IDesignerPlugin
#else
    public class ToolbarBase : ToolStrip, IDesignerPlugin
#endif
    {
        #region Fields
        private Designer designer;
#if !MONO
        private CustomizeItem customizeItem;
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets the report designer.
        /// </summary>
        public Designer Designer
        {
            get { return designer; }
        }

        /// <inheritdoc/>
        public string PluginName
        {
            get { return Name; }
        }

#if !MONO
        internal CustomizeItem CustomizeItem
        {
            get { return customizeItem; }
        }
#endif
        #endregion

        #region IDesignerPlugin
        /// <inheritdoc/>
        public virtual void SaveState()
        {
#if MONO
            XmlItem xi = Config.Root.FindItem("Designer").FindItem(Name);
            xi.SetProp("Visible", Visible ? "1" : "0");
//            xi.SetProp("Left", Location.X.ToString());
//            xi.SetProp("Top", Location.Y.ToString());
#endif
        }

        /// <inheritdoc/>
        public virtual void RestoreState()
        {
#if MONO
            XmlItem xi = Config.Root.FindItem("Designer").FindItem(Name);
            Visible = xi.GetProp("Visible") != "0";
//            string left = xi.GetProp("Left");
//            string top = xi.GetProp("Top");
//            if (left != "" && top != "")
//                Location = new Point(int.Parse(left), int.Parse(top));
#endif
        }

        /// <inheritdoc/>
        public virtual void SelectionChanged()
        {
        }

        /// <inheritdoc/>
        public virtual void UpdateContent()
        {
        }

        /// <inheritdoc/>
        public void Lock()
        {
        }

        /// <inheritdoc/>
        public void Unlock()
        {
            UpdateContent();
        }

        /// <inheritdoc/>
        public virtual void Localize()
        {
#if !MONO
            customizeItem.Text = Res.Get("Designer,Toolbar,AddOrRemove");
#endif
        }

        /// <inheritdoc/>
        public virtual DesignerOptionsPage GetOptionsPage()
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual void UpdateUIStyle()
        {
#if !MONO
            Style = UIStyleUtils.GetDotNetBarStyle(Designer.UIStyle);
#else
            Renderer = UIStyleUtils.GetToolStripRenderer(Designer.UIStyle);
#endif
        }
        /// <inheritdoc/>
        public virtual void ReinitDpiSize()
        {
            Font = DpiHelper.ConvertUnits(DrawUtils.Default96Font);
#if !MONO
            CustomizeItem.UpdateDpiDependencies();
            UpdateDpiDependencies();
#endif
        }
#endregion

#region Public Methods
#if !MONO

        /// <summary>
        /// Creates a new button.
        /// </summary>
        /// <param name="name">Button's name.</param>
        /// <param name="image">Button's image.</param>
        /// <param name="click">Click handler.</param>
        /// <returns>New button.</returns>
        public ButtonItem CreateButton(string name, Image image, EventHandler click)
        {
            return CreateButton(name, image, "", click);
        }

        /// <summary>
        /// Creates a new button.
        /// </summary>
        /// <param name="name">Button's name.</param>
        /// <param name="image">Button's image.</param>
        /// <param name="tooltip">Button's tooltip text.</param>
        /// <param name="click">Click handler.</param>
        /// <returns>New button.</returns>
        public ButtonItem CreateButton(string name, Image image, string tooltip, EventHandler click)
        {
            ButtonItem item = new ButtonItem();
            item.Name = name;
            item.Image = (Bitmap)image;
            item.Tooltip = tooltip;
            if (click != null)
                item.Click += click;
            return item;
        }

        internal void SetItemText(BaseItem item, string text)
        {
            item.Tooltip = text;
            item.Text = text;
        }
#else
        /// <summary>
        /// Creates a new button.
        /// </summary>
        /// <param name="name">Button's name.</param>
        /// <param name="image">Button's image.</param>
        /// <param name="click">Click handler.</param>
        /// <returns>New button.</returns>
        public ToolStripButton CreateButton(string name, Image image, EventHandler click)
        {
            ToolStripButton item = new ToolStripButton();
            item.Name = name;
            item.Image = image;
            item.DisplayStyle = ToolStripItemDisplayStyle.Image;
            item.AutoSize = false;
            item.Size = new Size(23, 22);
            if (click != null)
                item.Click += click;
            return item;
        }

        /// <summary>
        /// Creates a new split button.
        /// </summary>
        /// <param name="name">Button's name.</param>
        /// <param name="image">Button's image.</param>
        /// <param name="click">Click handler.</param>
        /// <returns>New split button.</returns>
        public ToolStripSplitButton CreateSplitButton(string name, Image image, EventHandler click)
        {
            ToolStripSplitButton item = new ToolStripSplitButton();
            item.Name = name;
            item.Image = image;
            item.DisplayStyle = ToolStripItemDisplayStyle.Image;
            if (click != null)
                item.Click += click;
            return item;
        }

        internal void SetItemText(ToolStripItem item, string text)
        {
            item.ToolTipText = text;
            item.Text = text;
        }
#endif
#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarBase"/> class with default settings.
        /// </summary>
        /// <param name="designer">The report designer.</param>
        /// <remarks>
        /// You don't need to call this constructor. The designer will do this automatically.
        /// </remarks>
        public ToolbarBase(Designer designer)
            : base()
        {
            this.designer = designer;
            Font = DpiHelper.ConvertUnits(DrawUtils.Default96Font);
#if !MONO
            GrabHandleStyle = eGrabHandleStyle.Office2003;

            customizeItem = new CustomizeItem();
            customizeItem.CustomizeItemVisible = false;
#else
            Dock = DockStyle.None;
#endif
        }
    }


}
