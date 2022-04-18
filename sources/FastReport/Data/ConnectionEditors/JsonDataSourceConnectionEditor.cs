using FastReport.Data.JsonConnection;
using FastReport.Forms;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FastReport.Data.ConnectionEditors
{
    /// <summary>
    ///
    /// </summary>
    public partial class JsonDataSourceConnectionEditor : ConnectionEditorBase
    {

        #region Internal Properties

        internal string JsonSchema
        {
            get
            {
                return tbJsonSchema.Text;
            }
            set
            {
                tbJsonSchema.Text = value;
            }
        }

        private string lastJson;

        internal bool IsJsonChanged
        {
            get
            {
                return lastJson != tbJson.Text;
            }
            set
            {
                if (value)
                {
                    lastJson = tbJson.Text;
                }
                else
                {
                    lastJson = null;
                }
            }
        }

        #endregion Internal Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public JsonDataSourceConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionString()
        {
            JsonDataSourceConnectionStringBuilder builder = new JsonDataSourceConnectionStringBuilder();
            builder.Json = tbJson.Text;
            builder.JsonSchema = tbJsonSchema.Text;
            //foreach (DataGridViewRow row in dgvHeaders.Rows)
            //{
            //    if (row.Cells[0].Value != null && row.Cells[1].Value != null)
            //    {
            //        var headerName = row.Cells[0].Value.ToString();
            //        var headerData = row.Cells[1].Value.ToString();
            //        builder.Headers.Add(headerName, headerData);
            //    }
            //}
            for (int i = 0; i < dgvHeaders.Rows.Count; i++)
            {
                DataGridViewRow row = dgvHeaders.Rows[i];
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    var headerName = row.Cells[0].Value.ToString();
                    var headerData = row.Cells[1].Value.ToString();
                    builder.Headers.Add(headerName, headerData);
                }
            }
            if (cbEnconding.SelectedItem is MyEncodingInfo)
            {
                builder.Encoding = (cbEnconding.SelectedItem as MyEncodingInfo).Name;
            }
            else
            {
                builder.Encoding = Encoding.UTF8.WebName;
            }
            return builder.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        protected override void SetConnectionString(string value)
        {
            JsonDataSourceConnectionStringBuilder builder = new JsonDataSourceConnectionStringBuilder(value);
            tbJson.Text = builder.Json;
            tbJsonSchema.Text = builder.JsonSchema;
            string encoding = builder.Encoding;

            foreach (KeyValuePair<string, string> header in builder.Headers)
            {
                dgvHeaders.Rows.Add(header.Key, header.Value);
            }

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

            lastJson = tbJson.Text;
        }

        #endregion Protected Methods

        #region Private Methods

        private void BtnJson_Click(object sender, EventArgs e)
        {
            JsonEditorForm form = new JsonEditorForm();
            form.JsonText = tbJson.Text;
            if (form.ShowDialog() == DialogResult.OK)
            {
                tbJson.Text = form.JsonText;
            }
        }

        private void BtnJsonSchema_Click(object sender, EventArgs e)
        {
            JsonEditorForm form = new JsonEditorForm();
            form.JsonText = tbJsonSchema.Text;
            if (form.ShowDialog() == DialogResult.OK)
            {
                tbJsonSchema.Text = form.JsonText;
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbEnconding.SelectedIndex > 0 && !(cbEnconding.SelectedItem is MyEncodingInfo))
            {
                cbEnconding.SelectedIndex = 0;
            }
        }

        private void Localize()
        {
            gbConnection.Text = Res.Get("ConnectionEditors,Json,Settings");
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

            lbEncoding.Text = Res.Get("ConnectionEditors,Json,Encoding");
            lbJson.Text = Res.Get("ConnectionEditors,Json,Input");
            lbJsonSchema.Text = Res.Get("ConnectionEditors,Json,Schema");
            lbHeaders.Text = Res.Get("ConnectionEditors,Json,RequestHeaders");
            dgvHeaders.Columns[0].HeaderText = Res.Get("ConnectionEditors,Json,HeaderKey");
            dgvHeaders.Columns[1].HeaderText = Res.Get("ConnectionEditors,Json,HeaderVal");
        }




        #endregion Private Methods

    }

}