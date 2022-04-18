using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using FastReport.Preview;
using FastReport.Table;
using FastReport.Utils;



namespace FastReport.Export.Text
{
    /// <summary>
    /// Represents the text export.
    /// </summary>
    public partial class TextExport : ExportBase
    {
        #region Constants
        private const float XDiv = 7.5f;
        private const float YDiv = 4f;

        private const float XDivAdv = 0.1015f;
        private const float YDivAdv = 0.06f;
        private const float CalculateStep = 0.1f;
        private const float CalculateStepBack = 0.01f;

        private const float divPaperX = 4.8f;
        private const float divPaperY = 3.6f;
        private const int CalcIterations = 10;
        byte[] u_HEADER = { 239, 187, 191 };
        string[] ref_frames = {
            // 0 left, 1 top, 2 left-up, 3 right-top, 4 right-down, 5 left-down, 6 left-t, 
            // 7 right-t,  8 up-t, 9 down-t, 10 cross
            "|-+++++++++",
            "\u2502\u2500\u250c\u2510\u2518\u2514\u251c\u2524\u252c\u2534\u253c" };
        #endregion

        #region Private fields
        private bool pageBreaks;
        private MyRes res;
        private bool frames;
        private bool textFrames;
        private bool emptyLines;
        private int screenWidth;
        private int screenHeight;
        private StringBuilder screen;
        private float scaleX;
        private float scaleY;
        private float scaleXStart;
        private float scaleYStart;
        private Encoding encoding;
        private bool dataOnly;
        private bool previewMode;
        private int pageWidth;
        private int pageHeight;
        private bool dataSaved;
        private bool dataLossBreak;
        private string frameChars;
        private List<TextExportPrinterType> printerTypes;
        private int printerType;
        private bool printAfterExport;
        private string printerName;
        private int copies;
        private bool avoidDataLoss;
        #endregion

        #region Properties

        /// <summary>
        /// Enable or disable the Data loss avoiding. 
        /// Auto calculation of ScaleX and ScaleY will be launched when dialogue window will be off.        
        /// </summary>
        public bool AvoidDataLoss
        {
            get { return avoidDataLoss; }
            set { avoidDataLoss = value; }
        }

        /// <summary>
        /// Gets or sets the count of copies for printing of results.
        /// </summary>
        public int Copies
        {
            get { return copies; }
            set { copies = value; }
        }

        /// <summary>
        /// Gets or sets the printer name for printing of results.
        /// </summary>
        public string PrinterName
        {
            get { return printerName; }
            set { printerName = value; }
        }

        /// <summary>
        /// Enable or disable the printing results after export.
        /// </summary>
        public bool PrintAfterExport
        {
            get { return printAfterExport; }
            set { printAfterExport = value; }
        }

        /// <summary>
        /// Gets or sets the active index of registered printer type.
        /// </summary>
        public int PrinterType
        {
            get { return printerType; }
            set { printerType = value; }
        }

        /// <summary>
        /// Gets or sets the list of printer types. <see cref="TextExportPrinterType"/>
        /// </summary>
        public List<TextExportPrinterType> PrinterTypes
        {
            get { return printerTypes; }
            set { printerTypes = value; }
        }

        /// <summary>
        /// Gets or sets the scale by X axis for correct text objects placement.
        /// </summary>
        public float ScaleX
        {
            get { return scaleX; }
            set
            {
                scaleX = value;
                scaleXStart = value;
            }
        }

        /// <summary>
        /// Gets or sets the scale by Y axis for correct text objects placement.
        /// </summary>
        public float ScaleY
        {
            get { return scaleY; }
            set
            {
                scaleY = value;
                scaleYStart = value;
            }
        }

        /// <summary>
        /// Gets or sets the encoding of resulting document. 
        /// </summary>
        /// <example>
        /// Windows ANSI encoding
        /// <code>TextExport.Encoding = Encoding.Default;</code>
        /// Unicode UTF-8 encoding
        /// <code>TextExport.Encoding = Encoding.UTF8;</code>
        /// OEM encoding for current system locale sessings
        /// <code>TextExport.Encoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);</code>
        /// </example>
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        /// <summary>
        /// Enable or disable the data only output without any headers. Default value is false.
        /// </summary>
        public bool DataOnly
        {
            get { return dataOnly; }
            set { dataOnly = value; }
        }

        /// <summary>
        /// Enable or disable the breaks of pages in resulting document. Default value is true.
        /// </summary>
        public bool PageBreaks
        {
            get { return pageBreaks; }
            set { pageBreaks = value; }
        }

        /// <summary>
        /// Enable or disable frames in resulting document. Default value is true.
        /// </summary>
        public bool Frames
        {
            get { return frames; }
            set { frames = value; }
        }

        /// <summary>
        /// Enable or disable the text (non graphic) frames in resulting document. Default value is false.
        /// </summary>
        public bool TextFrames
        {
            get { return textFrames; }
            set { textFrames = value; }
        }

        /// <summary>
        /// Enable or disable the output of empty lines in resulting document. Default value is false.
        /// </summary>
        public bool EmptyLines
        {
            get { return emptyLines; }
            set { emptyLines = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DataSaved
        {
            get { return dataSaved; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DataLossBreak
        {
            get { return dataLossBreak; }
            set { dataLossBreak = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PageHeight
        {
            get { return pageHeight; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PageWidth
        {
            get { return pageWidth; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool PreviewMode
        {
            get { return previewMode; }
            set { previewMode = value; }
        }
        #endregion

        #region Private Methods

        private char ScreenGet(int x, int y)
        {
            if ((x < screenWidth) && (y < screenHeight) &&
                (x >= 0) && (y >= 0))
                return screen[screenWidth * y + x];
            else
                return ' ';
        }

        private void ScreenType(int x, int y, char c)
        {
            if ((x < screenWidth) && (y < screenHeight) &&
                (x >= 0) && (y >= 0))
            {
                if (c != ' ')
                {
                    char current = screen[screenWidth * y + x];
                    if (current != ' ' && !(frames && IsFrame(current) && IsFrame(c)))
                        dataSaved = false;
                    screen[screenWidth * y + x] = c;
                }
            }
            else
                if (c != ' ')
                    dataSaved = false;
        }

        private bool IsFrame(char c)
        {
            return c == ' ' ? false : (frameChars.IndexOf(c) != -1);
        }

        private StringBuilder AlignStr(string s, HorzAlign align, int width)
        {
            if (align == HorzAlign.Right)
                return RightStr(s, width - 1);
            else if (align == HorzAlign.Center)
                return CenterStr(s, width - 1);
            else
                return LeftStr(s, width - 1);
        }

        private StringBuilder LeftStr(string s, int width)
        {
            return AddCharR(' ', s, width);
        }

        private StringBuilder AddCharR(char p, string s, int width)
        {
            width = width > 0 ? width : 0;
            StringBuilder result = new StringBuilder(width);
            if (s.Length < width)
                return result.Append(s).Append(new String(p, width - s.Length));
            else
                return result.Append(s);
        }

        private StringBuilder CenterStr(string s, int width)
        {
            if (width < s.Length)
                width = s.Length;
            StringBuilder result = new StringBuilder(width);
            if (s.Length < width)
            {
                result.Append(new String(' ', (int)(width / 2) - (int)(s.Length / 2))).Append(s);
                result.Append(new String(' ', width - result.Length));
            }
            else
                result.Append(s);
            return result;
        }

        private StringBuilder RightStr(string s, int width)
        {
            return AddChar(' ', s, width);
        }

        private StringBuilder AddChar(char p, string s, int width)
        {
            width = width > 0 ? width : 0;
            StringBuilder result = new StringBuilder(width);
            if (s.Length < width)
                result.Append(new String(p, width - s.Length)).Append(s);
            else
                result.Append(s);
            return result;
        }

        private void ScreenString(int x, int y, string s)
        {
            for (int i = 0; i < s.Length; i++)
                ScreenType(x + i, y, s[i]);
        }

        private void InitScreen()
        {
            screen = new StringBuilder(screenWidth * screenHeight);
            screen.Append(' ', screenWidth * screenHeight);
        }

        private void DrawLineObject(LineObject lineObject)
        {
            if (lineObject.Width == 0 || lineObject.Height == 0)
            {
                int d = frames ? 1 : 0;
                int curx = (int)Math.Round(lineObject.AbsLeft * scaleX * XDivAdv) + d;
                int cury = (int)Math.Round(lineObject.AbsTop * YDivAdv * scaleY) + d;
                int cury2 = (int)Math.Floor((lineObject.AbsTop + lineObject.Height) * scaleY * YDivAdv) + d;
                int curx2 = (int)Math.Floor((lineObject.AbsLeft + lineObject.Width) * scaleX * XDivAdv) + d;
                int height = cury2 - cury;
                int width = curx2 - curx;
                if (lineObject.Width == 0)
                    for (int i = 0; i < height; i++)
                        ScreenType(curx, cury + i, frameChars[0]);
                else if (lineObject.Height == 0)
                    for (int i = 0; i < width; i++)
                        ScreenType(curx + i, cury, frameChars[1]);
            }
        }

        private void DrawTextObject(TextObject textObject)
        {
            int linesBefore = 0;
            int d = frames ? 1 : 0;
            int curx = (int)(textObject.AbsLeft * scaleX * XDivAdv) + d;
            int cury = (int)(textObject.AbsTop * YDivAdv * scaleY) + d;
            int cury2 = (int)Math.Floor((textObject.AbsTop + textObject.Height) * scaleY * YDivAdv) + d;
            int curx2 = (int)Math.Floor((textObject.AbsLeft + textObject.Width) * scaleX * XDivAdv) + d;
            int height = cury2 - cury;
            int width = curx2 - curx;

            List<string> lines = WrapTextObject(textObject);

            if (textObject.VertAlign == VertAlign.Bottom)
                linesBefore = height - lines.Count;
            else if (textObject.VertAlign == VertAlign.Center)
                linesBefore = (int)((height - lines.Count) / 2);

            for (int i = 0; i < lines.Count; i++)
            {
                string s = AlignStr(lines[i], textObject.HorzAlign, width).ToString();
                ScreenString(curx, cury + i + linesBefore, s);
                if (dataLossBreak && !dataSaved)
                    return;
            }
            if (frames)
            {
                if ((textObject.Border.Lines & BorderLines.Left) > 0)
                    for (int i = 0; i < height; i++)
                        ScreenType(curx - 1, cury + i, frameChars[0]);
                if ((textObject.Border.Lines & BorderLines.Right) > 0)
                    for (int i = 0; i < height; i++)
                        ScreenType(curx + width - 1, cury + i, frameChars[0]);
                if ((textObject.Border.Lines & BorderLines.Top) > 0)
                    for (int i = 0; i < width; i++)
                        ScreenType(curx + i, cury - 1, frameChars[1]);
                if ((textObject.Border.Lines & BorderLines.Bottom) > 0)
                    for (int i = 0; i < width; i++)
                        ScreenType(curx + i, cury + height - 1, frameChars[1]);
            }
        }

        private List<string> WrapTextObject(TextObject obj)
        {
            float FDpiFX = 96f / DrawUtils.ScreenDpi;
            List<string> result = new List<string>();
            DrawText drawer = new DrawText();            
            using(Bitmap b = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(b))
            using (Font f = new Font(obj.Font.Name, obj.Font.Size * FDpiFX, obj.Font.Style))
            {
                float h = f.Height - f.Height / 4;
                float memoWidth = obj.Width - obj.Padding.Horizontal;

                string text = obj.Text;

                float memoHeight = drawer.CalcHeight(text, g, f,
                    memoWidth, obj.Height - obj.Padding.Vertical,
                    obj.HorzAlign, obj.LineHeight, obj.ForceJustify, obj.RightToLeft, obj.WordWrap, obj.Trimming);

                float y, prevy = 0;
                StringBuilder line = new StringBuilder(256);
                foreach (Paragraph par in drawer.Paragraphs)
                {
                    foreach (Word word in par.Words)
                    {
                        if (!word.visible)
                            break;
                        y = word.top + 1;
                        if (prevy == 0)
                            prevy = y;
                        if (y != prevy)
                        {
                            result.Add(line.ToString());
                            line.Length = 0;
                            prevy = y;
                        }
                        line.Append(word.text).Append(' ');
                    }
                }
                result.Add(line.ToString());
            }
            return result;
        }
        #endregion

        #region Protected Methods

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            if (avoidDataLoss)
                CalculateScale(null);
            if (printerType >= 0 && printerType < printerTypes.Count)
                foreach (TextExportPrinterCommand command in printerTypes[printerType].Commands)
                    if (command.Active)
                        foreach (byte esc in command.SequenceOn)
                            Stream.WriteByte(esc);
            if (Encoding == Encoding.UTF8)
                Stream.Write(u_HEADER, 0, 3);
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            if (printerType >= 0 && printerType < printerTypes.Count)
                foreach (TextExportPrinterCommand command in printerTypes[printerType].Commands)
                    if (command.Active)
                        foreach (byte esc in command.SequenceOff)
                            Stream.WriteByte(esc);
            FinishInternal();
            scaleX = scaleXStart;
            scaleY = scaleYStart;
        }

        


        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("TxtFile");
        }

        #endregion

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            if (frames)
                frameChars = textFrames ? ref_frames[0] : ref_frames[1];

            pageWidth = pageHeight = 0;
            dataSaved = true;
            screenWidth = (int)Math.Floor(ExportUtils.GetPageWidth(page) * divPaperX * XDivAdv * scaleX);
            screenHeight = (int)Math.Floor(ExportUtils.GetPageHeight(page) * divPaperY * YDivAdv * scaleY);
            InitScreen();
        }

        private void ExportObject(Base c)
        {
            if (c is ReportComponentBase && (c as ReportComponentBase).Exportable)
            {
                ReportComponentBase obj = c as ReportComponentBase;
                if (dataOnly && (obj.Parent == null || !(obj.Parent is DataBand)))
                    return;
                if (obj is TableCell)
                    return;
                else
                    if (obj is TableBase)
                {
                    TableBase table = obj as TableBase;
                    using (TextObject tableback = new TextObject())
                    {
                        tableback.Border = table.Border;
                        tableback.Fill = table.Fill;
                        tableback.FillColor = table.FillColor;
                        tableback.Left = table.AbsLeft;
                        tableback.Top = table.AbsTop;
                        float tableWidth = 0;
                        for (int i = 0; i < table.ColumnCount; i++)
                            tableWidth += table[i, 0].Width;
                        tableback.Width = (tableWidth < table.Width) ? tableWidth : table.Width;
                        tableback.Height = table.Height;
                        DrawTextObject(tableback);
                    }
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
                                DrawTextObject(textcell);
                            }
                            x += (table.Columns[j]).Width;
                        }
                        y += (table.Rows[i]).Height;
                    }
                }
                else if (obj is TextObject)
                    DrawTextObject(obj as TextObject);
                else if (obj is LineObject && frames)
                    DrawLineObject(obj as LineObject);
                //if (FDataLossBreak && !FDataSaved)
                //    return;
            }
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            ExportObject(band);
            foreach (Base c in band.ForEachAllConvectedObjects(this))
            {
                ExportObject(c);
            }
        }

        private StringBuilder builder;
        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            builder = new StringBuilder(screenHeight * screenWidth);
            for (int y = 0; y < screenHeight; y++)
            {
                bool empty = true;
                StringBuilder buf = new StringBuilder(screenWidth);
                for (int x = 0; x < screenWidth; x++)
                {
                    char c = ScreenGet(x, y);
                    if (frames && (c == ' ' || IsFrame(c))) // && c != ' ' && IsFrame(c)
                    {
                        bool up = ScreenGet(x, y - 1) == frameChars[0];
                        bool down = ScreenGet(x, y + 1) == frameChars[0];
                        bool left = ScreenGet(x - 1, y) == frameChars[1];
                        bool right = ScreenGet(x + 1, y) == frameChars[1];
                        if (down && right && !left && !up)
                            c = frameChars[2];
                        else if (down && !right && left && !up)
                            c = frameChars[3];
                        else if (!down && !right && left && up)
                            c = frameChars[4];
                        else if (!down && right && !left && up)
                            c = frameChars[5];
                        else if (down && right && !left && up)
                            c = frameChars[6];
                        else if (down && !right && left && up)
                            c = frameChars[7];
                        else if (down && right && left && !up)
                            c = frameChars[8];
                        else if (!down && right && left && up)
                            c = frameChars[9];
                        else if (up && down && left && right)
                            c = frameChars[10];
                        else if (up && down && !left && !right)
                            c = frameChars[0];
                        else if (!up && !down && left && right)
                            c = frameChars[1];
                    }
                    buf.Append(c);
                    if (c != ' ' && (!frames || c != frameChars[0]))
                        empty = false;
                }
                if (!empty || emptyLines)
                {
                    string s = buf.ToString().TrimEnd((char)32);
                    builder.AppendLine(s);
                    if (s.Length > pageWidth)
                        pageWidth = s.Length;
                    pageHeight++;
                }
            }
            if (pageBreaks)
                builder.AppendLine("\u000c");
            if (!previewMode)
            {
                byte[] bytes = encoding.GetBytes(builder.ToString());
                Stream.Write(bytes, 0, bytes.Length);
            }
        }

        #region Internal methods
        /// <summary>
        /// Exports the page.
        /// </summary>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        public string ExportPage(int pageNo)
        {
            PreparedPage ppage = Report.PreparedPages.GetPreparedPage(pageNo);
            ReportPage page = null;
            try
            {
                page = ppage.StartGetPage(pageNo);
                ExportPageBegin(page);
                foreach (Base obj in ppage.GetPageItems(page, false))
                    ExportBand(obj);
                ExportPageEnd(page);
            }
            finally
            {
                ppage.EndGetPage(page);
            }

            if (previewMode)
                return builder.ToString();
            else
                return String.Empty;
        }

        /// <summary>
        /// Calculates scale.
        /// </summary>
        /// <param name="progress"></param>
        public void CalculateScale(object progress)
        {
            bool oldPreviewMode = previewMode;
            dataLossBreak = true;
            previewMode = true;
            float initX = CalculateStep;
            float initY = CalculateStep;
            for (int p = 0; p < Report.PreparedPages.Count; p++)
            {
                if (IsAborted(progress))
                    break;
                ExportPage(p);
                int j = CalcIterations;
                while (!dataSaved && --j > 0)
                {
                    if (IsAborted(progress))
                        break;
                    int i = CalcIterations;
                    float oldX = ScaleX;
                    while (!dataSaved && --i > 0)
                    {
                        if (IsAborted(progress))
                            break;
                        scaleX += CalculateStep;
                        ExportPage(p);
                    }
                    i = CalcIterations;
                    while (!dataSaved && --i > 0)
                    {
                        if (IsAborted(progress))
                            break;
                        scaleY += CalculateStep;
                        ExportPage(p);
                    }

                    if (dataSaved && i < CalcIterations)
                    {
                        i = CalcIterations;
                        scaleX = oldX;
                        ExportPage(p);
                        while (!dataSaved && --i > 0)
                        {
                            if (IsAborted(progress))
                                break;
                            scaleX += CalculateStep;
                            ExportPage(p);
                        }
                    }
                }
                if (dataSaved && frames)
                {
                    int i = CalcIterations;
                    float oldY = scaleY;
                    while (dataSaved && --i > 0)
                    {
                        if (IsAborted(progress))
                            break;
                        oldY = scaleY;
                        scaleY -= CalculateStepBack;
                        if (scaleY < initY)
                            break;
                        ExportPage(p);
                    }
                    scaleY = oldY;
                    dataSaved = true;

                    i = CalcIterations;
                    float oldX = scaleX;
                    while (dataSaved && --i > 0)
                    {
                        if (IsAborted(progress))
                            break;
                        oldX = scaleX;
                        scaleX -= CalculateStepBack;
                        if (scaleX < initX)
                            break;
                        ExportPage(p);
                    }
                    scaleX = oldX;
                    dataSaved = true;
                }
                initX = scaleX;
                initY = scaleY;
                if (j == 0)
                    break;
            }
            dataLossBreak = false;
            previewMode = oldPreviewMode;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TextExport"/> class.
        /// </summary>       
        public TextExport()
        {
            pageBreaks = true;
            emptyLines = false;
            frames = true;
            textFrames = false;
            encoding = Encoding.UTF8;
            dataOnly = false;
            scaleX = 1.0f;
            scaleXStart = 1.0f;
            scaleY = 1.0f;
            scaleYStart = 1.0f;
            previewMode = false;
            dataLossBreak = false;
            avoidDataLoss = true;
            printerTypes = new List<TextExportPrinterType>();

            TextExportPrinterType printer = new TextExportPrinterType();
            printer.Name = "Epson ESC/P2";
            printerTypes.Add(printer);

            TextExportPrinterCommand command = new TextExportPrinterCommand();
            command.Name = "Reset";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(64);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Normal";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(120);
            command.SequenceOn.Add(0);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Pica";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(120);
            command.SequenceOn.Add(1);
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(107);
            command.SequenceOn.Add(0);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Elite";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(120);
            command.SequenceOn.Add(1);
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(107);
            command.SequenceOn.Add(1);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Condenced";
            command.SequenceOn.Add(15);
            command.SequenceOff.Add(18);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Bold";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(71);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(72);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Italic";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(52);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(53);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Wide";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(87);
            command.SequenceOn.Add(1);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(87);
            command.SequenceOff.Add(0);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "12cpi";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(77);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(80);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Linefeed 1/8\"";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(48);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Linefeed 7/72\"";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(49);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Linefeed 1/6\"";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(50);
            printer.Commands.Add(command);

            printer = new TextExportPrinterType();
            printer.Name = "HP PCL";
            printerTypes.Add(printer);

            command = new TextExportPrinterCommand();
            command.Name = "Reset";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(69);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Landscape";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(38);
            command.SequenceOn.Add(108);
            command.SequenceOn.Add(49);
            command.SequenceOn.Add(79);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(38);
            command.SequenceOff.Add(108);
            command.SequenceOff.Add(48);
            command.SequenceOff.Add(79);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Italic";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(40);
            command.SequenceOn.Add(115);
            command.SequenceOn.Add(49);
            command.SequenceOn.Add(83);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(40);
            command.SequenceOff.Add(115);
            command.SequenceOff.Add(48);
            command.SequenceOff.Add(83);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Bold";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(40);
            command.SequenceOn.Add(115);
            command.SequenceOn.Add(51);
            command.SequenceOn.Add(66);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(40);
            command.SequenceOff.Add(115);
            command.SequenceOff.Add(48);
            command.SequenceOff.Add(66);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Draft EconoMode";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(40);
            command.SequenceOn.Add(115);
            command.SequenceOn.Add(49);
            command.SequenceOn.Add(81);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(40);
            command.SequenceOff.Add(115);
            command.SequenceOff.Add(50);
            command.SequenceOff.Add(81);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Condenced";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(40);
            command.SequenceOn.Add(115);
            command.SequenceOn.Add(49);
            command.SequenceOn.Add(50);
            command.SequenceOn.Add(72);
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(38);
            command.SequenceOn.Add(108);
            command.SequenceOn.Add(56);
            command.SequenceOn.Add(68);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(40);
            command.SequenceOff.Add(115);
            command.SequenceOff.Add(49);
            command.SequenceOff.Add(48);
            command.SequenceOff.Add(72);
            printer.Commands.Add(command);

            printer = new TextExportPrinterType();
            printer.Name = "Canon/IBM";
            printerTypes.Add(printer);

            command = new TextExportPrinterCommand();
            command.Name = "Reset";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(64);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Normal";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(120);
            command.SequenceOn.Add(0);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Pica";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(48);
            command.SequenceOn.Add(73);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Elite";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(56);
            command.SequenceOn.Add(73);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Condenced";
            command.SequenceOn.Add(15);
            command.SequenceOff.Add(18);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Bold";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(71);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(72);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "Italic";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(52);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(53);
            printer.Commands.Add(command);

            command = new TextExportPrinterCommand();
            command.Name = "12cpi";
            command.SequenceOn.Add(27);
            command.SequenceOn.Add(77);
            command.SequenceOff.Add(27);
            command.SequenceOff.Add(80);
            printer.Commands.Add(command);

            printerType = 0;
            copies = 1;

            OpenAfterExport = false;
            printAfterExport = false;

            res = new MyRes("Export,Misc");
        }
    }

    /// <summary>
    /// Represents the printer command class
    /// </summary>
    public class TextExportPrinterCommand
    {
        private List<byte> sequenceOn;
        private List<byte> sequenceOff;
        private string name;
        private bool active;

        /// <summary>
        /// Gets or sets the active state of command. Default value is false.
        /// </summary>
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /// <summary>
        /// Gets or sets the command name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the list of "on sequence". 
        /// </summary>
        public List<byte> SequenceOn
        {
            get { return sequenceOn; }
            set { sequenceOn = value; }
        }

        /// <summary>
        /// Gets or sets the list of "off sequence". 
        /// </summary>
        public List<byte> SequenceOff
        {
            get { return sequenceOff; }
            set { sequenceOff = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextExportPrinterCommand"/> class.
        /// </summary>
        public TextExportPrinterCommand()
        {
            sequenceOn = new List<byte>();
            sequenceOff = new List<byte>();
            active = false;
        }
    }

    /// <summary>
    /// Represents of the printer type class.
    /// </summary>
    public class TextExportPrinterType
    {
        private List<TextExportPrinterCommand> commands;
        private string name;

        /// <summary>
        /// Gets or sets the printer name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the list of printer commands. <see cref="TextExportPrinterCommand"/>
        /// </summary>
        public List<TextExportPrinterCommand> Commands
        {
            get { return commands; }
            set { commands = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextExportPrinterType"/> class.
        /// </summary>
        public TextExportPrinterType()
        {
            commands = new List<TextExportPrinterCommand>();
        }
    }
}
