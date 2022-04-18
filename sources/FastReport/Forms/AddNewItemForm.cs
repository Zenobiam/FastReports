using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Design;
using FastReport.Wizards;

namespace FastReport.Forms
{
    internal partial class AddNewItemForm : BaseDialogForm
    {
        private Designer designer;
        private bool isEasterEggMode;

        public WizardBase SelectedWizard
        {
            get
            {
                if (CurrentItem == null)
                    return null;
                return Activator.CreateInstance(CurrentItem.Object) as WizardBase;
            }
        }

        private ObjectInfo CurrentItem
        {
            get { return lvWizards.SelectedItems.Count > 0 ? lvWizards.SelectedItems[0].Tag as ObjectInfo : null; }
        }

        private void PopulateWizardList()
        {
            List<ObjectInfo> objects = new List<ObjectInfo>();
            RegisteredObjects.Objects.EnumItems(objects);
            bool reportActive = designer.ActiveReport != null;

            foreach (ObjectInfo info in objects)
            {
                if (info.Object != null)
                {
                    if (info.Object.IsSubclassOf(typeof(WizardBase)) && (reportActive || info.Flags != 1))
                    {

                        if (!isEasterEggMode && info.Object.IsSubclassOf(typeof(EasterEggWizard)))
                        {
                            continue;
                        }

                        if (!IsListHasObject(info))
                        {
                            ListViewItem li = lvWizards.Items.Add(Res.TryGet(info.Text), info.ImageIndex);
                            li.Tag = info;
                            li.Group = lvWizards.Groups[info.Flags.ToString()];
                        }
                    }
                }
            }

            if (lvWizards.Items.Count > 0)
                lvWizards.Items[0].Selected = true;
        }

        private bool IsListHasObject(ObjectInfo info)
        {
            foreach(ListViewItem item in lvWizards.Items)
            {
                if (item.Tag == info)
                    return true;
            }
            return false;
        }

        private void lvWizards_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = CurrentItem != null;
        }

        private void lvWizards_DoubleClick(object sender, EventArgs e)
        {
            if (CurrentItem != null)
                DialogResult = DialogResult.OK;
        }

        private void Init()
        {
            lvWizards.SmallImageList = Res.GetImages();
            PopulateWizardList();
        }

        protected override void Scale()
        {
            lvWizards.SmallImageList = Res.GetImages(FormRatio);
            base.Scale();
        }

        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Forms,AddNewItem");
            Text = res.Get("");
            btnOk.Text = res.Get("Add");

            lvWizards.Groups.Add("0", res.Get("Reports"));
            lvWizards.Groups.Add("1", res.Get("ReportItems"));
        }

        public AddNewItemForm(Designer designer)
        {
            this.designer = designer;
            InitializeComponent();
            Localize();
            Scale();
            Init();
        }

        int gameStep = 0;

        private void AddNewItemForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (!isEasterEggMode)
            {
                switch (gameStep)
                {
                    case 0:
                        if (e.KeyCode == Keys.G)
                        {
                            gameStep = 1;
                        }
                        break;
                    case 1:
                        if (e.KeyCode == Keys.A)
                        {
                            gameStep = 2;
                        }
                        break;
                    case 2:
                        if (e.KeyCode == Keys.M)
                        {
                            gameStep = 3;
                        }
                        break;
                    case 3:
                        if (e.KeyCode == Keys.E)
                        {
                            isEasterEggMode = true;
                            gameStep = 4;
                        }
                        break;
                    default:
                        break;
                }

                if (isEasterEggMode)
                {
                    lvWizards.Groups.Add("2", Res.Get("Forms,AddNewItem,Games"));
                    PopulateWizardList();
                    
                }
            }
        }
    }
}
