using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FastReport.Utils;
using FastReport.Code;
using FastReport.RichTextParser;
using System.Windows.Forms;
using System.Drawing.Design;
#if !FRCORE
using FastReport.Controls;
#endif

namespace FastReport
{
    /// <summary>
    /// Represents a RichText object that can display formatted text.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="Text"/> property to set the object's text. The text may include
    /// the RTF formatting tags.
    /// </remarks>
    public partial class RichObject : TextObjectBase, IParent
    {
        #region Fields
        private string dataColumn;
        private int actualTextStart;
        private int actualTextLength;
        private string savedText;
        private string savedDataColumn;
#if !FRCORE && !MONO
        private Metafile cachedMetafile;
#endif
        private bool oldBreakStyle;
        private bool translateObject;
        private bool allowExpressionFormat;
        private DiffEventHandler event_handler = null;
        private bool textChanged = false;
        private ReportComponentCollection objects;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the object's text.
        /// </summary>
        /// <remarks>
        /// This property returns the formatted text with rtf tags.
        /// </remarks>
        [Category("Data")]
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                dataColumn = "";
                textChanged = true;
#if !FRCORE && !MONO
                DestroyCachedMetafile();
#endif
            }
        }

        /// <summary>
        /// Gets or sets a name of the data column bound to this control.
        /// </summary>
        /// <remarks>
        /// Value must contain the datasource name, for example: "Datasource.Column".
        /// </remarks>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.DataColumnEditor, FastReport", typeof(UITypeEditor))]
        public string DataColumn
        {
            get { return dataColumn; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (!String.IsNullOrEmpty(Brackets))
                    {
                        string[] brackets = Brackets.Split(new char[] { ',' });
                        Text = brackets[0] + value + brackets[1];
                    }
                }
                dataColumn = value;
            }
        }

        /// <summary>
        /// Gets the actual text start.
        /// </summary>
        /// <remarks>
        /// This property is for internal use only; you should not use it in your code.
        /// </remarks>
        [Browsable(false)]
        public int ActualTextStart
        {
            get { return actualTextStart; }
            set { actualTextStart = value; }
        }

        /// <summary>
        /// Gets the actual text length.
        /// </summary>
        /// <remarks>
        /// This property is for internal use only; you should not use it in your code.
        /// </remarks>
        [Browsable(false)]
        public int ActualTextLength
        {
            get { return actualTextLength; }
            set { actualTextLength = value; }
        }

        /// <summary>
        /// Gets or sets the break style.
        /// </summary>
        /// <remarks> 
        /// Set this property to true if you want editable rich text when you edit the prepared report page.
        /// </remarks>
        public bool OldBreakStyle
        {
            get { return oldBreakStyle; }
            set { oldBreakStyle = value; }
        }

        /// <summary>
        /// Experimental feature for translation of RichText into report objects
        /// </summary>
        public bool ConvertRichText
        {
            get
            {
#if !FRCORE && !MONO
                return translateObject;
#else
                return true;
#endif
            }
            set { translateObject = value; }
        }
        public bool KeepExpressionFormat
        {
            get { return allowExpressionFormat; }
            set { allowExpressionFormat = value; }
        }
        #endregion

        #region Private Methods

#if !FRCORE && !MONO
        private FRRichTextBox CreateRich()
        {
            FRRichTextBox rich = new FRRichTextBox();

            if (Text != null && Text.StartsWith(@"{\rtf"))
                rich.Rtf = Text;
            else
                rich.Text = Text;

            Color color = Color.White;
            if (Fill is SolidFill)
                color = (Fill as SolidFill).Color;
            if (color == Color.Transparent)
                color = Color.White;
            rich.BackColor = color;

            rich.DetectUrls = false;

            return rich;
        }

        private Metafile CreateMetafile(FRPaintEventArgs e)
        {
            Graphics measureGraphics = Report == null ? e.Graphics.Graphics : Report.PrintSettings.MeasureGraphics.Graphics;
            if (measureGraphics == null)
                measureGraphics = e.Graphics.Graphics;

            float scaleX = measureGraphics.DpiX / 96f;
            float scaleY = measureGraphics.DpiY / 96f;
            IntPtr hdc = measureGraphics.GetHdc();
            Metafile emf = new Metafile(hdc,
              new RectangleF(0, 0, (Width - Padding.Horizontal) * scaleX, (Height - Padding.Vertical) * scaleY),
              MetafileFrameUnit.Pixel);
            measureGraphics.ReleaseHdc(hdc);

            // create metafile canvas and draw on it
            using (Graphics g = Graphics.FromImage(emf))
            using (FRRichTextBox rich = CreateRich())
            {
                int textStart = ActualTextStart;
                int textLength = ActualTextLength != 0 ? ActualTextLength : rich.TextLength - textStart;
                rich.FormatRange(g, measureGraphics,
                  new RectangleF(0, 0, Width - Padding.Horizontal, Height - Padding.Vertical),
                  textStart, textStart + textLength, false);
            }

            return emf;
        }

        private void DestroyCachedMetafile()
        {
            if (cachedMetafile != null)
            {
                cachedMetafile.Dispose();
                cachedMetafile = null;
            }
        }

        private void DrawRich(FRPaintEventArgs e)
        {
            // avoid GDI+ errors
            if (Width < Padding.Horizontal + 1 || Height < Padding.Vertical + 1)
                return;

            // draw to emf because we need to zoom the image
            if (cachedMetafile == null)
                cachedMetafile = CreateMetafile(e);

            if (Fill.IsTransparent == false)
            {
                e.Graphics.DrawImage(cachedMetafile,
                    new RectangleF((AbsLeft + Padding.Left) * e.ScaleX,
                    (AbsTop + Padding.Top) * e.ScaleY,
                    (Width - Padding.Horizontal) * e.ScaleX,
                    (Height - Padding.Vertical) * e.ScaleY));
            }
            else
            {
                int w = (int)Math.Floor((Width - Padding.Horizontal) * e.ScaleX);
                int h = (int)Math.Floor((Height - Padding.Vertical) * e.ScaleY);

                using (var target = new Bitmap(w, h))
                using (var g = Graphics.FromImage(target))
                {
                    g.DrawImage(cachedMetafile, 0, 0, w, h);
                    target.MakeTransparent(Color.White);
                    e.Graphics.DrawImage(target, (AbsLeft + Padding.Left) * e.ScaleX, (AbsTop + Padding.Top) * e.ScaleY);
                }
            }

            if (IsDesigning)
                DestroyCachedMetafile();
        }

        private void PrintRich(FRPaintEventArgs e)
        {

            // avoid GDI+ errors
            if (Width < Padding.Horizontal + 1 || Height < Padding.Vertical + 1)
                return;

            if (ConvertRichText == false)
            {
                // FormatRange method uses GDI and does not respect transform settings of GDI+.
                RectangleF textRect = new RectangleF(
                  (AbsLeft + Padding.Left) + e.Graphics.Transform.OffsetX / e.ScaleX,
                  (AbsTop + Padding.Top) + e.Graphics.Transform.OffsetY / e.ScaleY,
                  (Width - Padding.Horizontal),
                  (Height - Padding.Vertical));

                IGraphics measureGraphics = Report == null ? e.Graphics : Report.PrintSettings.MeasureGraphics;
                if (measureGraphics == null)
                    measureGraphics = e.Graphics;

                using (FRRichTextBox rich = CreateRich())
                {
                    int textStart = ActualTextStart;
                    int textLength = ActualTextLength != 0 ? ActualTextLength : rich.TextLength - textStart;
                    rich.FormatRange(e.Graphics.Graphics, measureGraphics.Graphics, textRect, textStart, textStart + textLength, false);
                }
                return;
            }

            // draw to emf because we need to zoom the image
            if (cachedMetafile == null)
                cachedMetafile = CreateMetafile(e);

            if (Fill.IsTransparent == false)
            {
                e.Graphics.DrawImage(cachedMetafile,
                    new RectangleF((AbsLeft + Padding.Left) * e.ScaleX,
                    (AbsTop + Padding.Top) * e.ScaleY,
                    (Width - Padding.Horizontal) * e.ScaleX,
                    (Height - Padding.Vertical) * e.ScaleY));
            }
            else
            {
                int w = (int)Math.Floor((Width - Padding.Horizontal) * e.ScaleX);
                int h = (int)Math.Floor((Height - Padding.Vertical) * e.ScaleY);

                using (var target = new Bitmap(w, h))
                using (var g = Graphics.FromImage(target))
                {
                    g.DrawImage(cachedMetafile, 0, 0, w, h);
                    target.MakeTransparent(Color.White);
                    e.Graphics.DrawImage(target, (AbsLeft + Padding.Left) * e.ScaleX, (AbsTop + Padding.Top) * e.ScaleY);
                }
            }
        }
#endif

        internal List<ComponentBase> Convert2ReportObjects(out float height)
        {
            List<ComponentBase> clone_list = new List<ComponentBase>();

            #region "Copy properties from original objects"
            TextObjectBase tob = null;

            if (this.Border.Lines != BorderLines.None ||
                (this.FillColor != Color.White && this.FillColor != Color.Transparent))
            {
                tob = new TextObject();
                tob.Border = this.Border.Clone();
                tob.Bounds = this.Bounds;
                tob.ClientSize = this.ClientSize;
                tob.BreakTo = this.BreakTo;
                //   tob.Parent = this.Parent;
                tob.Top = 0; // this.Padding.Top;
                tob.Left = 0; // this.Padding.Left;
                tob.Fill = this.Fill.Clone();
                tob.FillColor = this.FillColor;
                if (CanGrow)
                {
                    tob.CanGrow = true;
                    tob.GrowToBottom = true;
                }
                tob.ZOrder = 0;
                tob.SaveState();
                clone_list.Add(tob);
            }

            #endregion
            using (RichText2ReportObject convertor = new RichText2ReportObject())
            using (RTF_DocumentParser parser = new RTF_DocumentParser())
            {
                parser.Load(Text);
                RichDocument rtf = parser.Document;
                if (tob != null)
                    tob.FillColor = parser.GetFillColor();
                clone_list.AddRange(convertor.RichObject2ReportObjects(this, ref rtf, out height));
            }
            if (CanGrow && height > this.Height || CanShrink && height < this.Height)
            {
                if (tob != null)
                    tob.Height = height;
                //this.Height = height;
            }
            else
            {
                if (tob != null)
                    tob.Height = this.Height;
            }
            return clone_list;
        }
        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
#if !FRCORE && !MONO
            if (disposing)
                DestroyCachedMetafile();
#endif
            base.Dispose(disposing);
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            RichObject src = source as RichObject;
            DataColumn = src.DataColumn;
            ActualTextStart = src.ActualTextStart;
            ActualTextLength = src.ActualTextLength;
            OldBreakStyle = src.OldBreakStyle;
            ConvertRichText = src.ConvertRichText;
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);

#if !FRCORE && !MONO
            try
            {
                if (!this.translateObject || IsDesigning)
                {
                    if (IsPrinting)
                        PrintRich(e);
                    else
                        DrawRich(e);
                }
            }
            catch (Exception ex)
            {
                e.Graphics.DrawString(ex.ToString(), DrawUtils.DefaultReportFont, Brushes.Red,
                  new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX, Height * e.ScaleY));
            }
            DrawMarkers(e);
#endif
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
        }
        #endregion

        #region Report Engine
        /// <inheritdoc/>
        public override void SaveState()
        {
            base.SaveState();
            savedText = Text;
            savedDataColumn = DataColumn;
        }

        /// <inheritdoc/>
        public override void RestoreState()
        {
            base.RestoreState();
            Text = savedText;
            dataColumn = savedDataColumn;
        }

        /// <inheritdoc/>
        public override string[] GetExpressions()
        {
            List<string> expressions = new List<string>();
            expressions.AddRange(base.GetExpressions());

            if (!String.IsNullOrEmpty(Brackets))
            {
                // collect expressions found in the text
                string[] brackets = Brackets.Split(new char[] { ',' });

                if (ConvertRichText)
                {
                    string decoded_text;
                    using (RTF_DocumentParser parser = new RTF_DocumentParser())
                    {
                        parser.Load(Text);
                        using (RTF_ToTextSaver saver = new RTF_ToTextSaver(parser.Document))
                            decoded_text = saver.PlainText;
                    }

                    expressions.AddRange(CodeUtils.GetExpressions(decoded_text, brackets[0], brackets[1]));
                }
                else
                {
                    // Don't worry. CORE and MONO must bever reach this block of code
#if !FRCORE && !MONO
                    using (FRRichTextBox rich = CreateRich())
                    {
                        expressions.AddRange(CodeUtils.GetExpressions(rich.Text, brackets[0], brackets[1]));
                    }
#endif
                }
            }

            if (!String.IsNullOrEmpty(DataColumn))
                expressions.Add(DataColumn);
            return expressions.ToArray();
        }

#if DEBUG_GET_RTF_DATA
        static int debug_rtf = 0;
#endif


        /// <inheritdoc/>
#if !FRCORE && !MONO
        public override void GetData()
        {
            base.GetData();
#if DIAGNOSTIC
            System.Diagnostics.Trace.WriteLine(this.Name + " GetData() ");
#endif
            if (!String.IsNullOrEmpty(DataColumn))
            {
                object value = Report.GetColumnValue(DataColumn);
                if (value is byte[])
                {
                    Text = value == null ? "" : System.Text.Encoding.UTF8.GetString(value as byte[]);
                }
                else
                {
                    Text = value == null ? "" : value.ToString();
                }
#if DEBUG_GET_RTF_DATA
                debug_rtf++;
                string FileName = @"rtf___" + debug_rtf.ToString() +".rtf";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileName))
                {
                    file.Write(Text);
                }
#endif
                if (ConvertRichText)
                    SerializePreview(this.Parent as BandBase);
            }
            else if (AllowExpressions)
            {
#if USE_ORIGINAL_RICH_EXPRESSION_CODE // Prior 20180423
        // process expressions
        if (!String.IsNullOrEmpty(Brackets))
        {
          using (FRRichTextBox rich = CreateRich())
          {
            string[] brackets = Brackets.Split(new char[] { ',' });
            FindTextArgs args = new FindTextArgs();
            args.Text = new FastString(rich.Text);
            args.OpenBracket = brackets[0];
            args.CloseBracket = brackets[1];
            args.StartIndex = ActualTextStart;
            int expressionIndex = 0;

            while (args.StartIndex < args.Text.Length - 2)
            {
              string expression = CodeUtils.GetExpression(args, false);
              if (expression == "")
                break;

              string formattedValue = CalcAndFormatExpression(expression, expressionIndex);
              // strip off the "\r" characters since rich uses only "\n" for new line
              formattedValue = formattedValue.Replace("\r", "");

              args.Text = args.Text.Remove(args.StartIndex, args.EndIndex - args.StartIndex);
              args.Text = args.Text.Insert(args.StartIndex, formattedValue);

              // fix for win8.1 and later
              // because their Selection* properties also includes control symbols
              {
                /*rich.SelectionStart = args.StartIndex;
                rich.SelectionLength = args.EndIndex - args.StartIndex;
                rich.SelectedText = formattedValue;*/

                int richIndex = rich.Find(args.OpenBracket + expression + args.CloseBracket, args.StartIndex, RichTextBoxFinds.None);
                if (richIndex != -1)
                {
                  rich.SelectedText = formattedValue;
                }
              }

              args.StartIndex += formattedValue.Length;
              expressionIndex++;
            }

            Text = rich.Rtf;
          }
        }
#else  // functions defined in RichText/RichObject.Ext.cs
                CalculateExpressions();
#endif
            }
        }

        /// <inheritdoc/>
        public override float CalcHeight()
        {
            using (FRRichTextBox rich = CreateRich())
            {
                int textStart = ActualTextStart;
                int textLength = ActualTextLength != 0 ? ActualTextLength : rich.TextLength - textStart;
                float h = SelectionHeight(rich, textStart, textLength);
                return h;
            }
        }

        private int SelectionHeight(FRRichTextBox rich, int start, int length)
        {
            using (Graphics g = rich.CreateGraphics())
            {
                int n1 = 0;
                int n2 = 100000;
                Graphics measureGraphics = Report == null ? g : Report.PrintSettings.MeasureGraphics.Graphics;
                if (measureGraphics == null)
                    measureGraphics = g;

                // find the height using halfway point
                for (int i = 0; i < 20; i++)
                {
                    int mid = (n1 + n2) / 2;

                    RectangleF textRect = new RectangleF(0, 0, Width - Padding.Horizontal, mid);
                    int fit = rich.FormatRange(g, measureGraphics, textRect, start, start + length, true) - start;

                    if (fit >= length)
                        n2 = mid;
                    else
                        n1 = mid;

                    if (Math.Abs(n1 - n2) < 2)
                        break;
                }

                int height = Math.Max(n1, n2);
                // workaround bug in richtext control (one-line text returns 0 height)
                if (rich.TextLength > 0 && height <= 2)
                {
                    RectangleF textRect = new RectangleF(0, 0, Width - Padding.Horizontal, 1000000);
                    rich.FormatRange(g, measureGraphics, textRect, start, start + length, true, out height);
                }
                return height + Padding.Vertical;
            }
        }

        /// <inheritdoc/>
        public override bool Break(BreakableComponent breakTo)
        {
            using (FRRichTextBox rich = CreateRich())
            using (Graphics g = rich.CreateGraphics())
            {
                // determine number of characters fit in the bounds. Set less height to prevent possible data loss.
                RectangleF textRect = new RectangleF(0, 0, Width - Padding.Horizontal, Height - Padding.Vertical - 20);
                Graphics measureGraphics = Report == null ? g : Report.PrintSettings.MeasureGraphics.Graphics;
                if (measureGraphics == null)
                    measureGraphics = g;

                // prevent page break when height is <= 0
                if (textRect.Height <= 0)
                    return false;

                int textStart = ActualTextStart;
                int textLength = ActualTextLength != 0 ? ActualTextLength : rich.TextLength - textStart;
                int charsFit = rich.FormatRange(g, measureGraphics, textRect, textStart, textStart + textLength, true) - textStart;

                if (charsFit <= 0 || charsFit == textLength + 1)
                    return false;

                // perform break
                if (breakTo != null)
                {
                    RichObject richTo = breakTo as RichObject;

                    if (OldBreakStyle)
                    {
                        // copy out-of-bounds rtf to the breakTo
                        rich.SelectionStart = charsFit;
                        rich.SelectionLength = rich.TextLength - charsFit;
                        richTo.Text = rich.SelectedRtf;

                        // leave text that fit in this object
                        rich.SelectedText = "";
                        Text = rich.Rtf;
                    }
                    else
                    {
                        richTo.Text = Text;
                        richTo.ActualTextStart = textStart + charsFit;
                        ActualTextLength = charsFit;
                    }
                }

                return true;
            }
        }
#else
                /// <inheritdoc/>
        public override float CalcHeight()
        {
            float height;
            if (objects.Count > 0)
                objects.Clear();

            List<ComponentBase> clone_list = Convert2ReportObjects(out height);
            foreach(ComponentBase clone in clone_list)
            {
                string baseName = clone.BaseName[0].ToString().ToLower();
                if (Report.PreparedPages == null)
                    clone.CreateUniqueName();
                else
                    clone.Name = Report.PreparedPages.Dictionary.CreateUniqueName(baseName);
                clone.Parent = this;
            }

            if (objects.Count == 0)
                height = Height;
            else if(this.CanShrink && height < Height)
                height = Height;
#if DIAGNOSTIC
            string decoded_text;
            using (RTF_DocumentParser parser = new RTF_DocumentParser())
            {
                parser.Load(Text);
                using (RTF_ToTextSaver saver = new RTF_ToTextSaver(parser.Document))
                    decoded_text = saver.PlainText;
            }
            System.Diagnostics.Trace.WriteLine(decoded_text);
            System.Diagnostics.Trace.WriteLine(height);
#endif
            return height;
        }

        /// <inheritdoc/>
        public override bool Break(BreakableComponent breakTo)
        {
            // determine number of characters fit in the bounds. Set less height to prevent possible data loss.
            RectangleF textRect = new RectangleF(0, 0, Width - Padding.Horizontal, Height - Padding.Vertical - 20);
            // prevent page break when height is <= 0
            if (textRect.Height <= 0)
                return false;

            int textStart = ActualTextStart;
            int charsFit = 0;
            bool allFit = true;
            List<ReportComponentBase> overlap = new List<ReportComponentBase>();
            foreach (ReportComponentBase obj in this.ChildObjects)
            {
                if (obj.Bottom > this.Bottom)
                {
                    allFit = false;
                    if (breakTo != null)
                    {
                        overlap.Add(obj);
                        objects.Remove(obj);
                        continue;
                    }
                    break;
                }
                charsFit += (obj as TextObject).Text.Length;
            }

            if (charsFit <= 0 || allFit)
                return false;

            // perform break
            if (breakTo != null)
            {
                RichObject richTo = breakTo as RichObject;
                breakTo.Clear();
                foreach (ReportComponentBase obj in overlap)
                    richTo.ChildObjects.Add(obj);

                richTo.ActualTextStart = textStart + charsFit;
                ActualTextLength = charsFit;
            }

            return true;
        }


        /// <inheritdoc/>
        public override void GetData()
        {
            base.GetData();
            if (!String.IsNullOrEmpty(DataColumn))
            {
                object value = Report.GetColumnValue(DataColumn);
                if (value is byte[])
                {
                    Text = value == null ? "" : System.Text.Encoding.UTF8.GetString(value as byte[]);
                }
                else
                {
                    Text = value == null ? "" : value.ToString();
                }
            }
            else if (AllowExpressions)
            {
                CalculateExpressions();
            }
        }


#endif
        #endregion

        #region Serialization

        private void GetDiff(object sender, DiffEventArgs e)
        {
            if (Report != null)
            {
                if (e.Object is Report)
                    e.DiffObject = Report;
                else if (e.Object is Base)
                    e.DiffObject = Report.FindObject((e.Object as Base).Name);
            }
        }

        private void SerializeSourcePages(FRWriter writer, BandBase band, bool SaveState)
        {
            objects.Clear();

            float height;
            List<ComponentBase> clone_list = Convert2ReportObjects(out height);

            foreach (ReportComponentBase clone_item in clone_list)
            {
                string baseName = clone_item.BaseName[0].ToString().ToLower();
                if (Report.PreparedPages == null)
                    clone_item.CreateUniqueName();
                else
                    clone_item.Name = Report.PreparedPages.Dictionary.CreateUniqueName(baseName);
                clone_item.Parent = this;
            }
#if DIAGNOSTIC
            System.Diagnostics.Debug.WriteLine("Change height {0} -> {1}", height, Height);
#endif
            this.Height = height;
        }

        private void SerializePreview(BandBase band)
        {
            float design_height = this.Height;

            float height;
            List<ComponentBase> clone_list = Convert2ReportObjects(out height);

            int i = Report.AllObjects.Count + 10;

            foreach (ReportComponentBase obj in clone_list)
            {
                obj.SetParent(band);
                string baseName = obj.BaseName[0].ToString().ToLower();
                obj.Name = Name.Length > 0 ? Name + "_" + i : "rtf_conv" + i;
                obj.Alias = Report.PreparedPages.Dictionary.AddUnique(baseName, obj.Name, obj);
                obj.Left = this.Left;
                i++;

                if (obj.AbsBottom > AbsBottom)
                {
                    if (CanGrow)
                    {
                        Height = obj.AbsBottom - AbsTop;
                    }
                }

                if (band.CanGrow || band.Bottom > obj.Bottom)
                {
#if DIAGNOSTIC
                            System.Diagnostics.Trace.WriteLine(this.Name +
                                " add object " + obj.Name +
                                " left " + obj.Left +
                                " top " + obj.Top +
                                " bottom " + obj.Bottom +
                                " Text: " + (obj as TextObject).Text);
#endif
                    obj.SaveState();
                    band.AddChild(obj);
                }
            }

#if DIAGNOSTIC
                    System.Diagnostics.Trace.WriteLine("RICH_OBJ: " + this.Name +
                        " left " + this.Left +
                        " top " + this.Top);
#endif
            this.Visible = false;
        }

        private void ConvertSerialize(FRWriter writer)
        {
            RichObject c = writer.DiffObject as RichObject;
            if (writer.SerializeTo == SerializeTo.SourcePages)
            {
                if (Parent is BandBase)
                {
                    if (FloatDiff(Top, c.Top))
                        writer.WriteFloat("Top", Top);
                    writer.GetDiff += event_handler;
                    SerializeSourcePages(writer, Parent as BandBase, false);
                    writer.GetDiff -= event_handler;
                }
                else
                {
                    throw new Exception("Not a band based RichObject");
                }
            }
            else if (writer.SerializeTo == SerializeTo.Preview)
            {
                if (Parent is BandBase)
                {
                    writer.GetDiff += event_handler;
                    SerializeSourcePages(writer, Parent as BandBase, false);
                    writer.GetDiff -= event_handler;

                    if (FloatDiff(Top, c.Top))
                        writer.WriteFloat("Top", Top);
                    if (FloatDiff(Left, c.Left))
                        writer.WriteFloat("Left", Left);
                    if (FloatDiff(Width, c.Width))
                        writer.WriteFloat("Width", Width);
                    if (FloatDiff(Height, c.Height))
                        writer.WriteFloat("Height", Height);
                }
                else
                {
                    throw new Exception("Not a band based RichObject");
                }
            }
            else
            {
                c.SetReport(this.Report);
                base.Serialize(writer);
            }
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
#if DIAGNOSTIC
            System.Diagnostics.Trace.WriteLine(this.Name + " Serialize to " + writer.SerializeTo.ToString());
#endif
            if (this.Text == null)
                return;
            if (this.ConvertRichText == true &&
                (
                //writer.SerializeTo == SerializeTo.SourcePages || 
                writer.SerializeTo == SerializeTo.Preview))
            {
                if (this.Text.Length < 5 || !this.Text.StartsWith(@"{\rtf"))
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(@"{\rtf1 ");
                    sb.Append(this.Text);
                    sb.Append(@"}");
                    this.Text = sb.ToString();
                }
                ConvertSerialize(writer);
                writer.SaveChildren = true;
                return;
            }
            base.Serialize(writer);
            if (writer.DiffObject is RichObject)
            {
                RichObject c = writer.DiffObject as RichObject;

                if (ActualTextStart != c.ActualTextStart)
                    writer.WriteInt("ActualTextStart", ActualTextStart);
                if (ActualTextLength != c.ActualTextLength)
                    writer.WriteInt("ActualTextLength", ActualTextLength);
                if (writer.SerializeTo != SerializeTo.Preview)
                {
                    if (DataColumn != c.DataColumn)
                        writer.WriteStr("DataColumn", DataColumn);
                }
                if (OldBreakStyle != c.OldBreakStyle)
                    writer.WriteBool("OldBreakStyle", OldBreakStyle);
                if (ConvertRichText != c.ConvertRichText)
                    writer.WriteBool("ConvertRichText", ConvertRichText);
                if (KeepExpressionFormat != c.KeepExpressionFormat)
                    writer.WriteBool("KeepExpressionFormat", KeepExpressionFormat);
            }
        }

        /// <inheritdoc/>
        public override void Deserialize(FRReader reader)
        {
#if DIAGNOSTIC
            System.Diagnostics.Trace.WriteLine(this.Name + " Deserialize from " + reader.DeserializeFrom.ToString());
#endif
            base.SetReport(reader.Report);
            base.Deserialize(reader);
        }
#region Translated objects list
        /// <inheritdoc/>
        public bool CanContain(Base child)
        {
            return (child is ReportComponentBase);
        }
        /// <inheritdoc/>
        public void GetChildObjects(ObjectCollection list)
        {
            foreach (ReportComponentBase c in objects)
            {
                list.Add(c);
            }
        }

        /// <inheritdoc/>
        public void AddChild(Base child)
        {
            if (child is ReportComponentBase)
                objects.Add(child as ReportComponentBase);
        }

        /// <inheritdoc/>
        public void RemoveChild(Base child)
        {
            if (child is ReportComponentBase)
                objects.Remove(child as ReportComponentBase);
        }

        /// <inheritdoc/>
        public int GetChildOrder(Base child)
        {
            return objects.IndexOf(child as ReportComponentBase);
        }

        /// <inheritdoc/>
        public void SetChildOrder(Base child, int order)
        {
            int oldOrder = child.ZOrder;
            if (oldOrder != -1 && order != -1 && oldOrder != order)
            {
                if (order > objects.Count)
                    order = objects.Count;
                if (oldOrder <= order)
                    order--;
                objects.Remove(child as ReportComponentBase);
                objects.Insert(order, child as ReportComponentBase);
            }
        }

        /// <inheritdoc/>
        public void UpdateLayout(float dx, float dy)
        {
        }
#endregion

#endregion Serialization


        /// <summary>
        /// Initializes a new instance of the <see cref="RichObject"/> class with default settings.
        /// </summary>
        public RichObject()
        {
            event_handler = new DiffEventHandler(GetDiff);
            dataColumn = "";
            SetFlags(Flags.HasSmartTag, true);
            objects = new ReportComponentCollection(this);
        }
    }
}