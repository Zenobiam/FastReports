using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Forms;
#if !MONO
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design
{
#if !MONO
    internal class ObjectsToolbar : Bar, IDesignerPlugin
#else
    internal class ObjectsToolbar : ToolStrip, IDesignerPlugin
#endif
    {
        #region Fields
        private Designer designer;
        private Report currentReport;
        private PageBase currentPage;
        private ObjectInfo nowInserting;
#if !MONO
        public ButtonItem btnSelect;
#else
        public ToolStripButton btnSelect;
#endif
        #endregion

        #region Properties
        internal Designer Designer
        {
            get { return designer; }
        }

        /// <inheritdoc/>
        public string PluginName
        {
            get { return Name; }
        }
        #endregion

        #region Private Methods
#if !MONO
        private void DoCreateButtons(ObjectInfo rootItem, SubItemsCollection items)
        {
            foreach (ObjectInfo item in rootItem.Items)
            {
                if (!item.Enabled)
                    continue;

                ButtonItem button = new ButtonItem();
                button.Image = item.Image;

                string text = Res.TryGet(item.Text);
                if (items == Items)
                {
                    button.Tooltip = text;
                    button.FixedSize = DpiHelper.ConvertUnits(new Size(25, 25));
                }
                else
                {
                    button.Text = text;
                    button.ButtonStyle = eButtonStyle.ImageAndText;
                }

                if (item.Items.Count > 0)
                {
                    // it's a category
                    DoCreateButtons(item, button.SubItems);
                    button.PopupSide = ePopupSide.Right;
                    button.AutoExpandOnClick = true;
                    button.FixedSize = DpiHelper.ConvertUnits(new Size(25, 32));
                    button.Tag = item;
                }
                else
                {
                    button.Tag = item;
                    button.Click += button_Click;
                }

                items.Add(button);
            }
        }

        private void CreateSelectBtn()
        {
            btnSelect = new ButtonItem();
            btnSelect.Image = Res.GetImage(100);
            btnSelect.Click += btnSelect_Click;
            btnSelect.FixedSize = DpiHelper.ConvertUnits(new Size(25, 25));

            Items.Add(btnSelect);
        }

        private void SortButtons()
        {
            List<BaseItem> tempItems = new List<BaseItem>();
            List<BaseItem> lastItems = new List<BaseItem>();
            tempItems.Add(Items[0]);
            for (int i = 1; i < Items.Count; i++)
            {
                for (int j = 1; j < Items.Count; j++)
                {
                    ObjectInfo objInfo = (Items[j] as ButtonItem).Tag as ObjectInfo;
                    if (objInfo != null)
                    {
                        if (objInfo.ButtonIndex == i)
                        {
                            tempItems.Add(Items[j]);
                        }
                        if (objInfo.ButtonIndex < 0 || objInfo.ButtonIndex >= Items.Count)
                        {
                            lastItems.Add(Items[j]);
                        }
                    }
                }
            }
            Items.Clear();
            Items.AddRange(tempItems.ToArray());
            Items.AddRange(lastItems.ToArray());
        }
#else
        private void DoCreateButtons(ObjectInfo rootItem)
        {
            foreach (ObjectInfo item in rootItem.Items)
            {
                if (item.Items.Count == 0)
                {
                    // it's an object
                    ToolStripButton button = new ToolStripButton();
                    button.AutoSize = false;
                    button.Size = new Size(22, 23);
                    button.Image = item.Image;
                    button.ToolTipText = Res.TryGet(item.Text);
                    button.Tag = item;
                    button.Click += button_Click;
                    Items.Add(button);
                }
                else
                {
                    // it's a category
                    ToolStripDropDownButton button = new ToolStripDropDownButton();
                    button.Image = item.Image;
                    button.ToolTipText = Res.TryGet(item.Text);
                    button.ShowDropDownArrow = false;
                    button.AutoSize = false;
                    button.Size = new Size(22, 23);
                    DoCreateMenus(item, button.DropDownItems);
                    Items.Add(button);
                }
            }
        }

        private void DoCreateMenus(ObjectInfo rootItem, ToolStripItemCollection items)
        {
            foreach (ObjectInfo item in rootItem.Items)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Image = item.Image;
                menuItem.Text = Res.TryGet(item.Text);
                menuItem.Tag = item;
                menuItem.Font = DrawUtils.DefaultFont;
                items.Add(menuItem);

                if (item.Items.Count != 0)
                {
                    // it's a category
                    DoCreateMenus(item, menuItem.DropDownItems);
                }
                else
                {
                    menuItem.Click += button_Click;
                }
            }
        }

        private void CreateSelectBtn()
        {
            btnSelect = new ToolStripButton(Res.GetImage(100));
            btnSelect.AutoSize = false;
            btnSelect.Size = new Size(22, 23);
            btnSelect.ToolTipText = Res.Get("Designer,Toolbar,Objects,Select");
            btnSelect.Click += btnSelect_Click;

            Items.Add(btnSelect);
        }
#endif

        private void CreateButtons()
        {
            if (Designer.ActiveReport != null && Designer.ActiveReport == currentReport &&
              Designer.ActiveReportTab.ActivePage == currentPage)
                return;
            currentReport = Designer.ActiveReport;
            if (Designer.ActiveReportTab != null)
                currentPage = Designer.ActiveReportTab.ActivePage;
            else
                currentPage = null;

            // delete all buttons except pointer
            int i = 0;
            while (i < Items.Count)
            {
                if (Items[i] == btnSelect)
                    i++;
                else
                {
                    Items[i].Dispose();
#if !MONO
                    Items.RemoveAt(i);
#endif
                }
            }
#if !MONO
            if (currentPage == null)
            {
                RecalcLayout();
                return;
            }

            // create object buttons
            ObjectInfo pageItem = RegisteredObjects.FindObject(currentPage);
            if (pageItem != null)
            {
                DoCreateButtons(pageItem, Items);
            }
            SortButtons();

            RecalcLayout();
#else
            // create object buttons
            ObjectInfo pageItem = new ObjectInfo();
            ObjectInfo info = RegisteredObjects.FindObject(currentPage);
            bool contain = true;
            if (info != null)
            {
                for (int item = 0; item < info.Items.Count; item++)
                {
                    contain = true;
                    if (pageItem.Items.Count != 0)
                    {
                        for (int l = 0; l < pageItem.Items.Count; l++)
                        {
                            if (info.Items[item].Text == pageItem.Items[l].Text)
                            {
                                contain = false;
                                break;
                            }
                        }
                    }
                    if (contain)
                    {
                        pageItem.Items.Add(info.Items[item]);
                    }
                }

                DoCreateButtons(pageItem);
            }
#endif
        }

        private void button_Click(object sender, EventArgs e)
        {
            Designer.FormatPainter = false;
            if (!Designer.cmdInsert.Enabled)
                return;

            ResetButtons();
#if !MONO
            if (sender is ButtonItem && (sender as ButtonItem).IsOnBar)
                (sender as ButtonItem).Checked = true;
            nowInserting = (sender as ButtonItem).Tag as ObjectInfo;
#else
            if (sender is ToolStripButton)
                (sender as ToolStripButton).Checked = true;
            nowInserting = (sender as ToolStripItem).Tag as ObjectInfo;
#endif
            Designer.InsertObject(nowInserting, InsertFrom.NewObject);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            Designer.FormatPainter = false;
            if (Designer.ActiveReportTab == null)
                return;
            DoClickSelectButton(true);
        }

        private void DoClickSelectButton(bool ignoreMultiInsert)
        {
            if (!btnSelect.Checked)
            {
                if (nowInserting != null && nowInserting.MultiInsert && !ignoreMultiInsert)
                {
                    Designer.InsertObject(nowInserting, InsertFrom.NewObject);
                }
                else
                {
                    ResetButtons();
                    btnSelect.Checked = true;
                    Designer.CancelPaste();
                }
            }
        }

        private void ResetButtons()
        {
#if !MONO
            foreach (BaseItem item in Items)
            {
                if (item is ButtonItem)
                    (item as ButtonItem).Checked = false;
            }
#else
            foreach (ToolStripItem item in Items)
            {
                if (item is ToolStripButton)
                    (item as ToolStripButton).Checked = false;
            }
#endif
        }
        #endregion

        #region Public Methods
        public void ClickSelectButton(bool ignoreMultiInsert)
        {
            DoClickSelectButton(ignoreMultiInsert);
        }
        #endregion

        #region IDesignerPlugin
        /// <inheritdoc/>
        public void SaveState()
        {
        }

        /// <inheritdoc/>
        public void RestoreState()
        {
        }

        /// <inheritdoc/>
        public void SelectionChanged()
        {
            CreateButtons();
        }

        /// <inheritdoc/>
        public void UpdateContent()
        {
            CreateButtons();
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
        public void Localize()
        {
            MyRes res = new MyRes("Designer,Toolbar,Objects");
            Text = res.Get("");
#if !MONO
            btnSelect.Tooltip = res.Get("Select");
#else
            btnSelect.ToolTipText = res.Get("Select");
#endif
        }

        /// <inheritdoc/>
        public DesignerOptionsPage GetOptionsPage()
        {
            return null;
        }

        /// <inheritdoc/>
        public void UpdateUIStyle()
        {
#if MONO
            Renderer = UIStyleUtils.GetToolStripRenderer(Designer.UIStyle);
#endif
        }

        public void ReinitDpiSize()
        {
#if !MONO
            this.Font = DpiHelper.ConvertUnits(DrawUtils.Default96Font);
            btnSelect.Image = Res.GetImage(100);
            btnSelect.FixedSize = DpiHelper.ConvertUnits(new Size(25, 25));
            UpdateDpiDependencies();

            int i = 0;
            while (i < Items.Count)
            {
                if (Items[i] == btnSelect)
                    i++;
                else
                {
                    Items[i].Dispose();
                    Items.RemoveAt(i);
                }
            }
            if (currentPage == null)
            {
                RecalcLayout();
                return;
            }

            // create object buttons
            ObjectInfo pageItem = RegisteredObjects.FindObject(currentPage);
            if (pageItem != null)
            {
                DoCreateButtons(pageItem, Items);
            }
#endif
        }
        #endregion

        public ObjectsToolbar(Designer designer)
            : base()
        {
            this.designer = designer;
            Name = "ObjectsToolbar";
            Font = DpiHelper.ConvertUnits(DrawUtils.Default96Font);
            Dock = DockStyle.Left;
#if !MONO
            DockOrientation = eOrientation.Vertical;
            RoundCorners = false;
#else
            GripStyle = ToolStripGripStyle.Hidden;
            AutoSize = false;
            Width = 26;
            if (Config.IsRunningOnMono)
                Padding = new Padding(1, 0, 0, 0);
#endif

            CreateSelectBtn();
            Localize();
#if !MONO
            Parent = Designer.DotNetBarManager.ToolbarLeftDockSite;
#else
            Parent = Designer;
#endif
        }
    }

}
