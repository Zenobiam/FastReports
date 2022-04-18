using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using FastReport.TypeEditors;
using FastReport.Utils;
using FastReport.Forms;

using FastReport.RichTextParser;
using FastReport.Code;
using FastReport.Table;

namespace FastReport
{
    /// <summary>
    /// Represents a RichText object that can display formatted text.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="Text"/> property to set the object's text. The text may include
    /// the RTF formatting tags.
    /// </remarks>
    public partial class RichObject : TextObjectBase, IParent, IHasEditor
    {
        #region Fields
        private string dataColumn;
        private int actualTextStart;
        private int actualTextLength;
        private string savedDataColumn;
        private bool oldBreakStyle;
        private bool translateObject;
        private DiffEventHandler event_handler = null;

        RichDocument rtf;
        List<Base> clone_list = new List<Base>();
        List<string> saved_text = new List<string>();
        float document_total_height;
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
                dataColumn = "";
                base.Text = value;
            }
        }


        /// <summary>
        /// Gets or sets a name of the data column bound to this control.
        /// </summary>
        /// <remarks>
        /// Value must contain the datasource name, for example: "Datasource.Column".
        /// </remarks>
        [Editor(typeof(DataColumnEditor), typeof(UITypeEditor))]
        [Category("Data")]
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
            get { return translateObject; }
            set { translateObject = value; }
        }
        #endregion

        #region Private Methods
        private void LoadRich(float vertical_offset)
        {
            using (RTF_DocumentParser parser = new RTF_DocumentParser())
            {
                parser.Load(Text);
                rtf = parser.Document;
            }
            Convert2ReportObjects(vertical_offset, out document_total_height);
        }

        private void DrawRich(FRPaintEventArgs e)
        {
            // avoid GDI+ errors
            if (Width < Padding.Horizontal + 1 || Height < Padding.Vertical + 1)
                return;

            if (Text.Length == 0)
            {
                e.Graphics.DrawString("Double click to load RichText", DrawUtils.DefaultReportFont, Brushes.Gray,
                  new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX, Height * e.ScaleY));
                return;
            }
            // Do nothing. Child objectd drawing itself?????
            for (int i = 0; i < clone_list.Count; i++)
                clone_list[i].SetParent(this);
        }

        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
#endregion

#region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            this.SetReport(source.Report);
            this.SetParent(source.Parent);
            base.Assign(source);
            TextObjectBase src = source as TextObjectBase;
            RichObject source_rich = source as RichObject;
            DataColumn = source_rich.DataColumn;
            ActualTextStart = source_rich.ActualTextStart;
#if OLD
      ActualTextLength = src.ActualTextLength;
      OldBreakStyle = src.OldBreakStyle;
#endif
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);
            try
            {
                if(IsDesigning)
                {
                    e.Graphics.DrawString("Double click to load RichText", DrawUtils.DefaultReportFont, Brushes.Gray,
                      new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX, Height * e.ScaleY));
                }
                else
                {
                    e.Graphics.DrawString("This text will not be shown", DrawUtils.DefaultReportFont, Brushes.Gray,
                      new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX, Height * e.ScaleY));
                }
            }
            catch (Exception ex)
            {
                e.Graphics.DrawString(ex.ToString(), DrawUtils.DefaultReportFont, Brushes.Red,
                  new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY, Width * e.ScaleX, Height * e.ScaleY));
            }
            DrawMarkers(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
        }

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
        internal List<ComponentBase> Convert2ReportObjects(out float height)
        {
            List<ComponentBase> clone_list = new List<ComponentBase>();

            #region "Copy properties from original objects"
            TextObjectBase tob = new TextObject();
            tob.Border = this.Border.Clone();
            tob.Bounds = this.Bounds;
            tob.ClientSize = this.ClientSize;
            tob.BreakTo = this.BreakTo;
            tob.Parent = this.Parent;
            tob.Fill = this.Fill.Clone();
            tob.FillColor = this.FillColor;
            if(CanGrow)
            {
                tob.CanGrow = true;
                tob.GrowToBottom = true;
            }
            clone_list.Add(tob);
            #endregion

            using (RichText2ReportObject convertor = new RichText2ReportObject())
            using (RTF_DocumentParser parser = new RTF_DocumentParser())
            {
                parser.Load(Text);
                RichDocument rtf = parser.Document;
                clone_list.AddRange(convertor.RichObject2ReportObjects(this, ref rtf, out height));
            }
            if (CanGrow && height > this.Height || CanShrink && height < this.Height)
            {
                tob.Height = height;
                this.Height = height;
            }
            else
                tob.Height = this.Height;
            return clone_list;
        }

        private void SerializeSourcePages(BandBase band, bool SaveState)
        {
            // Find objects below Rich
            List<ComponentBase> shift_list = new List<ComponentBase>();
            foreach (ComponentBase item in band.AllObjects)
            {
                if (item.Top > this.Bottom)
                    shift_list.Add(item);
            }
            float design_height = this.Height;

            float height;
            List<ComponentBase> clone_list = Convert2ReportObjects(out height);

            int i = Report.AllObjects.Count + 10;
            foreach (ReportComponentBase clone_item in clone_list)
            {
                string baseName = clone_item.BaseName[0].ToString().ToLower();
                clone_item.Name = Name.Length > 0 ? Name + "_" + i : "rtf_conv" + i;
                clone_item.Alias = Report.PreparedPages.Dictionary.AddUnique(baseName, clone_item.Name, clone_item);
                band.AddChild(clone_item);
                if (SaveState)
                    clone_item.SaveState();
                i++;
            }

            // This works for serialization to SourcePages
            band.RemoveChild(this);

            foreach (ComponentBase item in shift_list)
            {
                item.Top += height - design_height;
            }
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
                i++;
                obj.Alias = Report.PreparedPages.Dictionary.AddUnique(baseName, obj.Name, obj);

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
            if (writer.SerializeTo == SerializeTo.SourcePages)
            {
                if (Parent is BandBase)
                {
                    writer.GetDiff += event_handler;
                    SerializeSourcePages(Parent as BandBase, false);
                    writer.GetDiff -= event_handler;
                }
            }
            else
            {
                RichObject c = writer.DiffObject as RichObject;
                c.SetReport(this.Report);
                c.SetFlags(Flags.HasGlobalName, false);
                base.Serialize(writer);
            }
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
#if DIAGNOSTIC
            System.Diagnostics.Trace.WriteLine(this.Name + " Serialize to " + writer.SerializeTo.ToString());
#endif
            if (this.Text.Length > 5 && this.Text.StartsWith(@"{\rtf"))
            {
                ConvertSerialize(writer);
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
                //if (KeepExpressionFormat != c.KeepExpressionFormat)
                //    writer.WriteBool("KeepExpressionFormat", KeepExpressionFormat);
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


        /// <summary>
        /// Invokes object's editor.
        /// </summary>
        /// <returns></returns>
        public bool InvokeEditor()
        {
            using (RichSelectorForm form = new RichSelectorForm(this))
            {
                form.ShowDialog();
                if (form.IsModified)
                {
                    Report.Designer.SetModified(this, "Modified");
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc/>
        public override SmartTagBase GetSmartTag()
        {
            return new RichObjectSmartTag(this);
        }

        /// <inheritdoc/>
        public override ContextMenuBase GetContextMenu()
        {
            return new TextObjectBaseMenu(Report.Designer);
        }
#endregion

#region Report Engine
        /// <inheritdoc/>
        public override string[] GetExpressions()
        {
            if (!AllowExpressions)
                return null;
            List<string> expressions = new List<string>();
            expressions.AddRange(base.GetExpressions());
            if (!String.IsNullOrEmpty(Brackets))
            {
                // collect expressions found in the text
#if false // Original code wich looks inside RichText
                string[] brackets = Brackets.Split(new char[] { ',' });
                GenerateExpressionList();
                expressions.AddRange(expression_list);
#else
                foreach(ReportComponentBase obj in this.clone_list /*this.ChildObjects*/)
                {
                    foreach(string expr in obj.GetExpressions())
                    {
                        expressions.Add(expr);
                    }
                }
#endif
            }
            if (!String.IsNullOrEmpty(DataColumn))
                expressions.Add(DataColumn);
                return expressions.ToArray();
        }

        /// <inheritdoc/>
        public override void SaveState()
        {
            base.SaveState();
            savedDataColumn = DataColumn;
            saved_text.Clear();
            foreach (ReportComponentBase richobj in this.clone_list)
                richobj.SaveState();
        }

        /// <inheritdoc/>
        public override void RestoreState()
        {
            base.RestoreState();
            dataColumn = savedDataColumn;
            foreach (ReportComponentBase richobj in this.clone_list)
                richobj.RestoreState();
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

        internal void Convert2ReportObjects(float vertical_offset, out float height)
        {
            clone_list.Clear();
            using (RichText2ReportObject convertor = new RichText2ReportObject())
            {
                List<ComponentBase> list = convertor.RichObject2ReportObjects(this, ref rtf, out height);
                foreach (ComponentBase obj in list)
                {
                    obj.SetParent(this);
                    obj.Top += vertical_offset;
                    obj.Name = obj.BaseName + "_fixme";
                    if (obj is TextObjectBase)
                    {
                        TextObjectBase text = obj as TextObjectBase;
                        text.Padding = new Padding(Padding.Left, 0, Padding.Right, 0);
                    }
                    else if(obj is PictureObjectBase)
                    {
                        PictureObject pic = obj as PictureObject;
                        pic.Padding = new Padding(Padding.Left, 0, Padding.Right, 0);
                    }
                    AddChild(obj);
                }
            }
        }

        /// <inheritdoc/>
        public override float CalcHeight()
        {
            float h = 0;
            float b = 0;
            float g = 0;

            foreach (ReportComponentBase obj in this.clone_list)
            {
                h += obj.CalcHeight();
                if (b < obj.AbsBottom)
                    b = obj.AbsBottom;
                if (g < obj.Bottom)
                    g = obj.Bottom;
            }

            document_total_height = b;

            return document_total_height;
        }


        /// <inheritdoc/>
        public override bool Break(BreakableComponent breakTo)
        {
            // Since 2020 Sep 15 the rich object translated to report objecs instead of using RTF_View
            return true;
        }
    #endregion

        #region IParent implementation
        /// <inheritdoc/>
        public bool CanContain(Base child)
        {
            return true;
        }

        /// <inheritdoc/>
        public void GetChildObjects(ObjectCollection list)
        {
            foreach(Base item in clone_list)
                list.Add(item);
        }

        /// <inheritdoc/>
        public void AddChild(Base child)
        {
           clone_list.Add(child);
        }

        /// <inheritdoc/>
        public void RemoveChild(Base child)
        {
            clone_list.Remove(child);
        }

        /// <inheritdoc/>
        public int GetChildOrder(Base child)
        {
            return clone_list.IndexOf(child);
        }

        /// <inheritdoc/>
        public void SetChildOrder(Base child, int order)
        {
        }
        /// <inheritdoc/>
        public void UpdateLayout(float dx, float dy)
        {
        }
#endregion

        /// <inheritdoc/>
        public override void ExtractMacros()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichObject"/> class with default settings.
        /// </summary>
        public RichObject()
        {
            dataColumn = "";
            SetFlags(Flags.HasSmartTag, true);
        }
    }
}
