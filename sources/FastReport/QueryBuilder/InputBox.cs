using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Forms;
using FastReport.Utils;

namespace FastReport.FastQueryBuilder
{
    internal partial class InputBox : ScaledSupportingForm
    {
        public InputBox()
        {
            InitializeComponent();
            Font = DrawUtils.DefaultFont;
            Text = Res.Get("Forms,QueryBuilder,Alias");
            TextLabel.Text = Res.Get("Forms,QueryBuilder,InputAlias");
            button1.Text = Res.Get("Buttons,OK");
            button2.Text = Res.Get("Buttons,Cancel");
            Scale();
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            TextBox.Select();
        }
    }
}