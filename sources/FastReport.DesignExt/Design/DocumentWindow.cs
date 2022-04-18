using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
#if !MONO
using FastReport.DevComponents.DotNetBar;
#else
using FastReport.Controls;
#endif

namespace FastReport.Design
{
#if !MONO
    internal class DocumentWindow : TabItem
    {
        public Control ParentControl
        {
            get { return AttachedControl; }
        }

        public void AddToTabControl(FastReport.DevComponents.DotNetBar.TabControl tabs)
        {
            TabControlPanel panel = AttachedControl as TabControlPanel;
            tabs.Tabs.Add(this);
            tabs.Controls.Add(panel);
            tabs.ApplyDefaultPanelStyle(panel);
            panel.Padding = new System.Windows.Forms.Padding(0);

            if (tabs.Style == eTabStripStyle.VS2005Document)
            {
                panel.Style.BorderSide = eBorderSide.Bottom;
                panel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
                panel.Style.BorderColor.Color = SystemColors.ControlDark;
            }
        }

        public void Activate()
        {
            Parent.SelectedTab = this;
        }

        public void Close()
        {
            Parent.Tabs.Remove(this);
            Dispose();
        }

        public DocumentWindow()
        {
            TabControlPanel panel = new TabControlPanel();
            panel.Dock = DockStyle.Fill;

            panel.TabItem = this;
            AttachedControl = panel;
        }
    }
#else
    internal class DocumentWindow : PageControlPage
    {
        private FRTabControl parent;

        public Control ParentControl
        {
            get { return parent; }
        }

        public void AddToTabControl(FRTabControl tabs)
        {
            tabs.Tabs.Add(this);
            parent = tabs;
        }

        public void Activate()
        {
            parent.SelectedTab = this;
        }

        public void Close()
        {
            parent.Tabs.Remove(this);
            Dispose();
        }

        public DocumentWindow()
        {
            Dock = DockStyle.Fill;
        }
    }
#endif
}
