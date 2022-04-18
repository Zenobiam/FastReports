using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Design;
using FastReport.Utils;

#if !MONO
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport
{
    /// <summary>
    /// The base class for the context menu item.
    /// </summary>
    [ToolboxItem(false)]
    public class ContextMenuItem
#if !MONO
        : ButtonItem
    {
        /// <summary>
        /// Gets a collection of menu items.
        /// </summary>
        public SubItemsCollection DropDownItems
        {
            get { return SubItems; }
        }

        /// <summary>
        /// Gets or sets "Check on click" property.
        /// </summary>
        public bool CheckOnClick
        {
            get { return AutoCheckOnClick; }
            set { AutoCheckOnClick = value; }
        }

        /// <summary>
        /// Sets bold font.
        /// </summary>
        public void SetFontBold()
        {
            FontBold = true;
            HotFontBold = true;
        }
    }
#else
        : ToolStripMenuItem
    {
        private bool beginGroup;
        private bool visibleInternal = true;

        /// <summary>
        /// Gets or sets a value indicating that a separator is needed before this menu item.
        /// </summary>
        public bool BeginGroup
        {
            get { return beginGroup; }
            set { beginGroup = value; }
        }

        /// <summary>
        /// Gets or sets the internal visibility value.
        /// </summary>
        public new bool Visible
        {
            // Visible flag is always false when menu is not displayed
            get { return visibleInternal; }
            set { visibleInternal = value; base.Visible = value; }
        }

        /// <summary>
        /// Sets bold font.
        /// </summary>
        public void SetFontBold()
        {
            Font = new Font(Font, FontStyle.Bold);
        }
    }
#endif
    /// <summary>
    /// The base class for the context menu of the report component.
    /// </summary>
    /// <remarks>
    /// This class represents a context menu of the report component that is displayed when the object 
    /// is right-clicked in the designer.
    /// </remarks>
    [ToolboxItem(false)]
    public class ContextMenuBase 
#if !MONO
        : ContextMenuBar
#else
        : ContextMenuStrip
#endif
    {
        #region Fields
        private Designer designer;
#if !MONO
        private ContextMenuItem mnuContextRoot;
#endif

        #endregion

        #region Properties
        /// <summary>
        /// The reference to the report designer.
        /// </summary>
        public Designer Designer
        {
            get { return designer; }
        }

#if !MONO
        /// <summary>
        /// Gets a collection of menu items.
        /// </summary>
        /// <remarks>
        /// You should add new items to this collection.
        /// </remarks>
        public new SubItemsCollection Items
        {
            get { return mnuContextRoot.SubItems; }
        }
#endif
        #endregion

        #region Private Methods
        #endregion

        #region Protected Methods
        /// <summary>
        /// This method is called to reflect changes in the designer.
        /// </summary>
        protected virtual void Change()
        {
            Designer.SetModified(null, "Change");
        }

        /// <summary>
        /// Creates a new menu item.
        /// </summary>
        /// <param name="text">Item's text.</param>
        /// <returns>New item.</returns>
        protected ContextMenuItem CreateMenuItem(string text)
        {
            return CreateMenuItem(null, text, null);
        }

        /// <summary>
        /// Creates a new menu item.
        /// </summary>
        /// <param name="text">Item's text.</param>
        /// <param name="click">Click handler.</param>
        /// <returns>New item.</returns>
        protected ContextMenuItem CreateMenuItem(string text, EventHandler click)
        {
            return CreateMenuItem(null, text, click);
        }

        /// <summary>
        /// Creates a new menu item.
        /// </summary>
        /// <param name="image">Item's image.</param>
        /// <param name="text">Item's text.</param>
        /// <param name="click">Click handler.</param>
        /// <returns>New item.</returns>
        protected ContextMenuItem CreateMenuItem(Image image, string text, EventHandler click)
        {
            ContextMenuItem item = new ContextMenuItem();
            item.Image = image;
            item.Text = text;
#if MONO
            item.Font = DrawUtils.DefaultFont; // workaround Mono behavior
#endif
            if (click != null)
                item.Click += click;
            return item;
        }
        #endregion

#if !MONO
        /// <summary>
        /// Displays context menu.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="location">Location.</param>
        public void Show(Control parent, Point location)
        {
            mnuContextRoot.PopupMenu(parent.PointToScreen(location));
        }
#else
        /// <summary>
        /// Displays context menu.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="location">Location.</param>
        public new void Show(Control parent, Point location)
        {
            // convert item.BeginGroup to a separator
            for (int i = 0; i < Items.Count; i++)
            {
                ContextMenuItem item = Items[i] as ContextMenuItem;
                if (i > 0 && item != null && item.Visible && item.BeginGroup)
                {
                    Items.Insert(i, new ToolStripSeparator());
                    i++;
                }
            }
            
            base.Show(parent, location);
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <b>ComponentBaseMenu</b> class with default settings. 
        /// </summary>
        /// <param name="designer">The reference to a report designer.</param>
        public ContextMenuBase(Designer designer)
            : base()
        {
            this.designer = designer;
#if !MONO
            Style = UIStyleUtils.GetDotNetBarStyle(Designer.UIStyle);
            Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
            mnuContextRoot = new ContextMenuItem();
            base.Items.Add(mnuContextRoot);
#else
            Font = DrawUtils.DefaultFont;
            Renderer = UIStyleUtils.GetToolStripRenderer(Designer.UIStyle);
#endif
        }
    }
}
