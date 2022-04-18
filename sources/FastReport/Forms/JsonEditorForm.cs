using FastReport.Utils;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
    internal partial class JsonEditorForm : BaseDialogForm
    {
        #region Private Fields

        private bool textChanged;

        #endregion Private Fields

        #region Public Properties

        public string JsonText
        {
            get { return tbJson.Text; }
            set
            {
                tbJson.Text = value;
                textChanged = false;
            }
        }

        public void SetToReadOnly()
        {
            tbJson.ReadOnly = true;
            btnOpen.Enabled = false;
            btnFormat.Enabled = false;

            btnOk.Left = btnCancel.Left;
            btnOk.Top = btnCancel.Top;

            btnCancel.Visible = false;
            btnCancel.Enabled = false;

            MyRes res = new MyRes("Forms,JsonEditor");
            Text = res.Get("");

            if (tbJson.ReadOnly)
            {
                Text += " " + res.Get("Readonly");
            }
        }

        #endregion Public Properties

        #region Public Constructors

        public JsonEditorForm()
        {
            InitializeComponent();
            Localize();
            Init();
            Scale();
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Forms,TextEditor");
            
            cbWordWrap.Text = res.Get("WordWrap");

            res = new MyRes("Forms,JsonEditor");
            Text = res.Get("");

            if(tbJson.ReadOnly)
            {
                Text += " " + res.Get("Readonly");
            }


            btnOpen.Text = res.Get("Open");
            btnSave.Text = res.Get("Save");
            btnFormat.Text = res.Get("FormatJson");
            btnOpen.Image = Res.GetImage(1);
            btnSave.Image = Res.GetImage(2);
            btnFormat.Image = Res.GetImage(29);

            cbEnconding.Items.Clear();
            cbEnconding.Items.Add(new MyEncodingInfo(Encoding.UTF8));
            cbEnconding.SelectedIndex = 0;

            cbEnconding.Items.Add(new MyEncodingInfo(Encoding.ASCII));
            cbEnconding.Items.Add(new MyEncodingInfo(Encoding.Unicode));
            cbEnconding.Items.Add(new MyEncodingInfo(Encoding.BigEndianUnicode));
            cbEnconding.Items.Add(new MyEncodingInfo(Encoding.UTF7));
            cbEnconding.Items.Add(new MyEncodingInfo(Encoding.UTF32));

            cbEnconding.Items.Add("—————");

            foreach (MyEncodingInfo info in MyEncodingInfo.GetEncodings())
            {
                cbEnconding.Items.Add(info);
            }
        }

        #endregion Public Methods

        #region Private Methods
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Res.Get("Forms,JsonEditor,Filter");

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    tbJson.Text = File.ReadAllText(ofd.FileName, Encoding.GetEncoding((cbEnconding.SelectedItem as MyEncodingInfo).Name));
                }
                catch (Exception ex)
                {
                    ExceptionForm exceptionForm = new ExceptionForm(ex);
                    exceptionForm.ShowDialog();
                }
            }
        }

        private void CbEnconding_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbEnconding.SelectedIndex > 0 && !(cbEnconding.SelectedItem is MyEncodingInfo))
            {
                cbEnconding.SelectedIndex = 0;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Res.Get("Forms,JsonEditor,Filter");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(sfd.FileName, tbJson.Text, Encoding.GetEncoding((cbEnconding.SelectedItem as MyEncodingInfo).Name));
                }
                catch (Exception ex)
                {
                    ExceptionForm exceptionForm = new ExceptionForm(ex);
                    exceptionForm.ShowDialog();
                }
            }
        }

        private void BtnFormat_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbJson.Text.Trim().Length > 0)
                {
                    JsonBase obj = JsonBase.FromString(tbJson.Text);
                    StringBuilder sb = new StringBuilder();
                    obj.WriteTo(sb, 2);
                    tbJson.Text = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionForm exceptionForm = new ExceptionForm(ex);
                exceptionForm.ShowDialog();
            }
        }

        private void CbWordWrap_CheckedChanged(object sender, EventArgs e)
        {
            tbJson.WordWrap = cbWordWrap.Checked;
        }

        private void Done()
        {
            Config.SaveFormState(this);
            XmlItem xi = Config.Root.FindItem("Forms").FindItem("JsonEditorForm");
            xi.SetProp("WordWrap", cbWordWrap.Checked ? "1" : "0");

            if (cbEnconding.SelectedItem is MyEncodingInfo)
            {
                xi.SetProp("Encoding", (cbEnconding.SelectedItem as MyEncodingInfo).Name);
            }
            else
            {
                xi.SetProp("Encoding", Encoding.UTF8.WebName);
            }
            
        }

        private void Init()
        {
            tbJson.Font = DrawUtils.FixedFont;
            StartPosition = FormStartPosition.Manual;
            Config.RestoreFormState(this);
            XmlItem xi = Config.Root.FindItem("Forms").FindItem("JsonEditorForm");
            cbWordWrap.Checked = xi.GetProp("WordWrap") != "0";


            string encoding = xi.GetProp("Encoding");
            int i = 0;
            foreach (object item in cbEnconding.Items)
            {
                if (item is MyEncodingInfo)
                    if ((item as MyEncodingInfo).Name == encoding)
                    {
                        cbEnconding.SelectedIndex = i;
                        break;
                    }
                i++;
            }
            if (i >= cbEnconding.Items.Count)
            {
                cbEnconding.SelectedIndex = 0;
            }
        }

        private void JsonEditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Done();
        }

        private void JsonEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK && DialogResult != DialogResult.Abort && textChanged)
            {
                string askText = Res.Get("Forms,TextEditor,ConfirmChanges");
                DialogResult askResult = FRMessageBox.Confirm(askText, MessageBoxButtons.YesNoCancel);

                switch (askResult)
                {
                    case DialogResult.Yes:
                        DialogResult = DialogResult.OK;
                        break;

                    case DialogResult.No:
                        break;

                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void TbJson_TextChanged(object sender, EventArgs e)
        {
            textChanged = true;
        }

        #endregion Private Methods

        protected override void Scale()
        {
            //tbJson.Multiline = false;
            base.Scale();
            tbJson.Height = Height - toolStrip1.Height - btnCancel.Height - DpiHelper.ConvertUnits(10) - (Height - btnCancel.Bottom);

            cbWordWrap.Location = new System.Drawing.Point(cbWordWrap.Location.X, tbJson.Bottom + DpiHelper.ConvertUnits(5));
        }
    }
}