using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using FastReport.Table;
using FastReport.Utils;

namespace FastReport.Export.LaTeX
{
    /// <summary>
    /// Represents the LaTeX export filter.
    /// </summary>
    public partial class LaTeXExport : ExportBase
    {
        const float PT_MM = 2.834645669291f;
        const float PT_INCH = 72;
        const float PT_PX = 72f / 96f;

        private float leftMargin = 0;
        private float topMargin = 0;

        private float leftStartPos = 0;
        private float topStartPos = 0;

        #region Private Methods
        private void Write(Stream stream, string value)
        {
            byte[] buf = Encoding.UTF8.GetBytes(value);
            stream.Write(buf, 0, buf.Length);
        }

        private void WriteLn(Stream stream, string value)
        {
            byte[] buf = Encoding.UTF8.GetBytes(value);
            stream.Write(buf, 0, buf.Length);
            stream.WriteByte(13);
            stream.WriteByte(10);
        }

        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            WriteLn(Stream, @"\documentclass{report}");
            WriteLn(Stream, @"\usepackage{xltxtra}");
            WriteLn(Stream, @"\usepackage{vmargin}");
            WriteLn(Stream, @"\usepackage[absolute,overlay]{textpos}");
            WriteLn(Stream, @"\usepackage{ragged2e}");

            WriteLn(Stream, @"\author {" + PrepareString(Report.ReportInfo.Author) + "}");
            WriteLn(Stream, @"\title {" + PrepareString(Report.ReportInfo.Name) + "}");
            WriteLn(Stream, @"\date {\today}");

            WriteLn(Stream, @"\newcommand{\textout}[3]{\setlength{\parindent}{0mm}{\fontspec{#1}{\fontsize{#2}{12}\selectfont {#3}}}}");

            WriteLn(Stream, @"\newcommand{\text}[6]{");
            WriteLn(Stream, @"\raggedright\begin{textblock*}{#1}(#2,#3)");
            WriteLn(Stream, @"{\textout{#4}{#5}{#6}}");
            WriteLn(Stream, @"\end{textblock*}}");

            WriteLn(Stream, @"\newcommand{\textright}[6]{");
            WriteLn(Stream, @"\raggedleft\begin{textblock*}{#1}(#2,#3)");
            WriteLn(Stream, @"{\textout{#4}{#5}{#6}}");
            WriteLn(Stream, @"\end{textblock*}}");

            WriteLn(Stream, @"\newcommand{\textcenter}[6]{");
            WriteLn(Stream, @"\centering\begin{textblock*}{#1}(#2,#3)");
            WriteLn(Stream, @"{\textout{#4}{#5}{#6}}");
            WriteLn(Stream, @"\end{textblock*}}");

            WriteLn(Stream, @"\newcommand{\textjustify}[6]{");
            WriteLn(Stream, @"\justify\begin{textblock*}{#1}(#2,#3)");
            WriteLn(Stream, @"{\textout{#4}{#5}{#6}}");
            WriteLn(Stream, @"\end{textblock*}}");

            WriteLn(Stream, @"\begin{document}");
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            WriteLn(Stream, @"\end{document}");
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("LaTeXFile");
        }

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);

            leftMargin = page.LeftMargin * PT_MM;
            topMargin = page.TopMargin * PT_MM;
            leftStartPos = leftMargin + PT_INCH;
            topStartPos = topMargin + PT_INCH;

            WriteLn(Stream, @"\setpapersize{custom}{" + page.PaperWidth.ToString() +"mm}{" + page.PaperHeight.ToString() +"mm}");
            WriteLn(Stream, @"\setmarginsrb{" + 
                Math.Round(leftMargin, 2).ToString("R") + "pt}{" +
                Math.Round(topMargin, 2).ToString("R") + "pt}{" +
                Math.Round(page.RightMargin * PT_MM, 2).ToString("R") + "pt}{" +
                Math.Round(page.BottomMargin * PT_MM, 2).ToString("R") + "pt}{0pt}{0pt}{0pt}{0pt}");
            WriteLn(Stream, @"\thispagestyle{empty}");
            WriteLn(Stream, @"\ ");
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            base.ExportPageEnd(page);
            
            WriteLn(Stream, @"\newpage");
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);

            if (band.Parent == null) 
		        return;

            // add export of band

            foreach(Base bandObject in band.ForEachAllConvectedObjects(this))
            {
                if (bandObject is TableBase)
                {
                    ExportTableObject(bandObject as TableBase);
                }
                else if (bandObject is TextObject)
                {
                    ExportTextObject(bandObject as TextObject);
                }
            }
        }

        private StringBuilder PrepareString(string line)
        {
            StringBuilder sb = new StringBuilder(line.Length);
            foreach(char c in line)
            {
                switch(c)
                {
                    case '%':
                        sb.Append(@"\%");
                        break;
                    case '$':
                        sb.Append(@"\$");
                        break;
                    case '&':
                        sb.Append(@"\&");
                        break;
                    case '_':
                        sb.Append(@"\_");
                        break;
                    case '{':
                        sb.Append(@"\{");
                        break;
                    case '}':
                        sb.Append(@"\}");
                        break;
                    case '#':
                        sb.Append(@"\#");
                        break;
                    case '^':
                        sb.Append(@"\^");
                        break;
                    case '~':
                        sb.Append(@"\~");
                        break;
                    case '\\':
                        sb.Append(@"\textbackslash");
                        break;
                    case '\n':
                        sb.Append("\n\\\\");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb;
        }

        private void ExportTableObject(TableBase table)
        {
            if (table.ColumnCount > 0 && table.RowCount > 0)
            {
                float y = 0;
                for (int i = 0; i < table.RowCount; i++)
                {
                    float x = 0;
                    for (int j = 0; j < table.ColumnCount; j++)
                    {
                        if (!table.IsInsideSpan(table[j, i]))
                        {
                            TableCell textcell = table[j, i];
                            textcell.Left = x;
                            textcell.Top = y;
                            if (textcell is TextObject)
                                ExportTextObject(textcell as TextObject);
                        }
                        x += (table.Columns[j]).Width;
                    }
                    y += (table.Rows[i]).Height;
                }
            }
        }

        private void ExportTextObject(TextObject textObject)
        {
            StringBuilder sb = new StringBuilder();
            DrawTextFrame(sb, textObject.Text, textObject.Font, textObject.AbsLeft, textObject.AbsTop, textObject.Width, textObject.HorzAlign);
            Write(Stream, sb.ToString());
        }

        private void DrawTextFrame(StringBuilder sb, string text, Font font, float left, float top, float width, HorzAlign horizAlign)
        {
            sb.Append(@"\").Append(GetBlockJustify(horizAlign)).Append("{").
                Append(Math.Round(width * PT_PX, 2).ToString("R")).Append("pt}{").
                Append(Math.Round(leftStartPos + left * PT_PX, 2).ToString("R")).Append("pt}{").
                Append(Math.Round(topStartPos + top * PT_PX, 2).ToString("R")).Append("pt}{").
                Append(font.Name).Append("}{").Append(font.Size.ToString()).Append("}{").
                Append(GetFontStyle(font)).
                Append(PrepareString(text)).
                AppendLine("}");
        }

        private string GetBlockJustify(HorzAlign horizAlign)
        {
            if (horizAlign == HorzAlign.Left)
                return "text";
            else if (horizAlign == HorzAlign.Right)
                return "textright";
            else if (horizAlign == HorzAlign.Center)
                return "textcenter";
            else
                return "textjustify";
        }

        private StringBuilder GetFontStyle(Font font)
        {
            StringBuilder sb = new StringBuilder();
            if (font.Bold)
                sb.Append(@"\bfseries ");
            if (font.Italic)
                sb.Append(@"\itshape ");
            return sb;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LaTeXExport"/> class.
        /// </summary>       
        public LaTeXExport()
        {

        }
    }
}
