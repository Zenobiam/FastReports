using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using FastReport.Utils;
using FastReport.Data;
#if !MONO
using FastReport.DevComponents.DotNetBar;
#endif
using FastReport.Editor.Syntax.Parsers;

namespace FastReport.Controls
{
    internal class DescriptionControl : Panel
    {
#if !MONO
        private LabelX lblDescription;
#else
        private Panel pnDescription;
        private Report txReport;
        private TextObject txDescription;
#endif

        private void RecalcDescriptionSize()
        {
#if !MONO
            // hack to force GetPreferredSize do the work
            lblDescription.UseMnemonic = false;
            int maxWidth = Width - SystemInformation.VerticalScrollBarWidth - 8;
            Size preferredSize = lblDescription.GetPreferredSize(new Size(maxWidth, 0));
            lblDescription.Size = preferredSize;
#else
            txDescription.Width = Width - 20;
            pnDescription.Size = new Size(Width - 20, (int)txDescription.CalcHeight());
            pnDescription.Refresh();
#endif
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RecalcDescriptionSize();
        }

        public void ShowDescription(Report report, object info)
        {
            string descr = "";

            if (info is SystemVariable)
            {
                descr = "<b>" + (info as SystemVariable).Name + "</b>";
                descr += "<br/><br/>" + ReflectionRepository.DescriptionHelper.GetDescription(info.GetType());
            }
            else if (info is MethodInfo)
            {
                descr = report.CodeHelper.GetMethodSignature(info as MethodInfo, true);
                descr += "<br/><br/>" + ReflectionRepository.DescriptionHelper.GetDescription(info as MethodInfo);

                foreach (ParameterInfo parInfo in (info as MethodInfo).GetParameters())
                {
                    // special case - skip "thisReport" parameter
                    if (parInfo.Name == "thisReport")
                        continue;

                    string s = ReflectionRepository.DescriptionHelper.GetDescription(parInfo);
                    s = s.Replace("<b>", "{i}").Replace("</b>:", "{/i}").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("{i}", "<i>").Replace("{/i}", "</i>");
                    descr += "<br/><br/>" + s;
                }
            }

            descr = descr.Replace("\t", "<br/>");
#if !MONO	
            lblDescription.Text = descr;
            RecalcDescriptionSize();
#else
            txDescription.Text = descr;
            RecalcDescriptionSize();
            pnDescription.Refresh();
#endif
        }

#if MONO
        private void pnDescription_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(SystemBrushes.Window, new Rectangle(0, 0, pnDescription.Width, pnDescription.Height));
            txDescription.Width = pnDescription.Width;
            txDescription.Height = pnDescription.Height;
            txDescription.DrawText(new FRPaintEventArgs(e.Graphics, 1, 1, txDescription.Report.GraphicCache));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            txDescription.Dispose();
            txReport.Dispose();
        }
#endif

        public DescriptionControl()
        {
#if !MONO
            lblDescription = new LabelX();
            lblDescription.AntiAlias = false;
            lblDescription.PaddingLeft = 2;
            lblDescription.PaddingRight = 2;
            lblDescription.PaddingTop = 2;
            lblDescription.TextLineAlignment = StringAlignment.Near;
            lblDescription.UseMnemonic = false;
            lblDescription.WordWrap = true;
            Controls.Add(lblDescription);
            lblDescription.BackColor = SystemColors.Window;
#else
            pnDescription = new Panel();
            pnDescription.Padding = new Padding(2, 2, 2, 0);
            pnDescription.Paint += pnDescription_Paint;
            txReport = new Report();
            txDescription = new TextObject();
            txDescription.SetReport(txReport);
            txDescription.TextRenderType = TextRenderType.HtmlTags;
            Controls.Add(pnDescription);
#endif
            AutoScroll = true;
            BackColor = SystemColors.Window;
        }
    }
}
