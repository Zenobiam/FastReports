using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;

namespace FastReport.FastQueryBuilder
{
    internal partial class JoinEditorForm : Form
    {
        internal Link link;

        public JoinEditorForm()
        {
            InitializeComponent();        
        }

        private void JoinEditorForm_Load(object sender, EventArgs e)
        {
            MyRes res = new MyRes("Forms,QueryBuilder");
            Text = res.Get("JoinEditor");
            label3.Text = res.Get("Tables");
            label4.Text = res.Get("Condition");

            label1.Text = link.from.table.Name;
            label2.Text = link.to.table.Name;

            label6.Text = link.from.Name;
            label5.Text = link.to.Name;

            comboBox2.DataSource = QueryEnums.JoinTypesToStr;
            comboBox2.SelectedIndex = (int)link.join;
            
            res = new MyRes("Forms,QueryBuilder,Where");
            string[] whereTypes = new string[] {
              res.Get("Equal"),
              res.Get("NotEqual"),
              res.Get("GtOrEqual"),
              res.Get("Gt"),
              res.Get("LtOrEqual"),
              res.Get("Lt"),
              res.Get("Like"),
              res.Get("NotLike") };
            comboBox1.DataSource = whereTypes;
            comboBox1.SelectedIndex = (int)link.where;
            button1.Text = Res.Get("Buttons,OK");
            button2.Text = Res.Get("Buttons,Cancel");
        }

        private void JoinEditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                link.where = (QueryEnums.WhereTypes)comboBox1.SelectedIndex;
                link.join = (QueryEnums.JoinTypes)comboBox2.SelectedIndex;
            }
        }
    }
}