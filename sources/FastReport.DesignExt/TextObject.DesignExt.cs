using System;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using FastReport.Design.PageDesigners.Page;
using FastReport.Forms;

namespace FastReport
{
    partial class TextObject : IHasEditor
    {
        #region Fields
        private TextBox textBox;
        internal bool dragAccept;
        #endregion

        #region Properties
        /// <inheritdoc/>
        public override float Left
        {
            get { return base.Left; }
            set
            {
                base.Left = value;
                if (IsEditing)
                    UpdateEditorPosition();
            }
        }

        /// <inheritdoc/>
        public override float Top
        {
            get { return base.Top; }
            set
            {
                base.Top = value;
                if (IsEditing)
                    UpdateEditorPosition();
            }
        }

        /// <inheritdoc/>
        public override float Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                if (IsEditing)
                    UpdateEditorPosition();
            }
        }

        /// <inheritdoc/>
        public override float Height
        {
            get { return base.Height; }
            set
            {
                base.Height = value;
                if (IsEditing)
                    UpdateEditorPosition();
            }
        }

        private bool IsEditing
        {
            get { return IsDesigning && textBox != null; }
        }
        #endregion

        #region Private Methods
        //private bool ShouldSerializeFont()
        //{
        //    return Font.Name != "Arial" || Font.Size != 10 || Font.Style != FontStyle.Regular;
        //}

        private bool ShouldSerializeTextFill()
        {
            return !(TextFill is SolidFill) || (TextFill as SolidFill).Color != Color.Black;
        }

        private void UpdateEditorPosition()
        {
            textBox.Location = new Point((int)Math.Round(AbsLeft * ReportWorkspace.Scale) + 1,
              (int)Math.Round(AbsTop * ReportWorkspace.Scale) + 1);
            textBox.Size = new Size((int)Math.Round(Width * ReportWorkspace.Scale) - 1,
              (int)Math.Round(Height * ReportWorkspace.Scale) - 1);
        }

        private void FTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                FinishEdit(false);

            if (e.Control && e.KeyCode == Keys.Enter)
                FinishEdit(true);
        }

        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void AssignFormat(ReportComponentBase source)
        {
            base.AssignFormat(source);

            TextObject src = source as TextObject;
            if (src == null)
                return;

            HorzAlign = src.HorzAlign;
            VertAlign = src.VertAlign;
            Angle = src.Angle;
            Underlines = src.Underlines;
            Font = src.Font;
            TextFill = src.TextFill.Clone();
            TextOutline.Assign(src.TextOutline);
            Trimming = src.Trimming;
            FontWidthRatio = src.FontWidthRatio;
            FirstTabOffset = src.FirstTabOffset;
            TabWidth = src.TabWidth;
            Clip = src.Clip;
            Wysiwyg = src.Wysiwyg;
            LineHeight = src.LineHeight;
            TextRenderType = src.TextRenderType;
            ParagraphOffset = src.ParagraphOffset;
        }


        /// <inheritdoc/>
        public override void HandleDragOver(FRMouseEventArgs e)
        {
            if (PointInObject(new PointF(e.x, e.y)) && e.DragSource is TextObject)
                e.handled = true;
            dragAccept = e.handled;
        }

        /// <inheritdoc/>
        public override void HandleDragDrop(FRMouseEventArgs e)
        {
            Text = (e.DragSource as TextObject).Text;
            dragAccept = false;
        }

        /// <inheritdoc/>
        public override void HandleKeyDown(Control sender, KeyEventArgs e)
        {
            if (IsSelected && e.KeyCode == Keys.Enter && HasFlag(Flags.CanEdit) && !HasRestriction(Restrictions.DontEdit))
            {
                textBox = new TextBox();
                textBox.Font = new Font(Font.Name, Font.Size * ReportWorkspace.Scale * DrawUtils.ScreenDpiFX, Font.Style);
                textBox.BorderStyle = BorderStyle.None;
                textBox.Multiline = true;
                textBox.AcceptsTab = true;
                if (Fill is SolidFill)
                    textBox.BackColor = Color.FromArgb(255, (Fill as SolidFill).Color);
                if (TextFill is SolidFill)
                    textBox.ForeColor = Color.FromArgb(255, (TextFill as SolidFill).Color);

                textBox.Text = Text;
                textBox.KeyDown += new KeyEventHandler(FTextBox_KeyDown);
                UpdateEditorPosition();
                sender.Controls.Add(textBox);
                textBox.SelectAll();
                textBox.Focus();
                e.Handled = true;
            }
        }
        /// <inheritdoc/>
        public override void OnMouseDown(MouseEventArgs e)
        {
            if (Editable && !Config.WebMode && InvokeEditor())
            {
                Report report = Report;
                if (report != null)
                {
                    Preview.PreviewControl preview = report.Preview;
                    if (preview != null)
                    {
                        // update current page in a cache
                        report.PreparedPages.ModifyPage(Report.Preview.PageNo - 1, Page as ReportPage);
                        // redraw the preview
                        preview.Refresh();
                    }
                }
            }
            else
                base.OnMouseDown(e);
        }

        /// <inheritdoc/>
        public override void SelectionChanged()
        {
            FinishEdit(true);
        }

        /// <inheritdoc/>
        public override ContextMenuBase GetContextMenu()
        {
            return new TextObjectMenu(Report.Designer);
        }

        /// <inheritdoc/>
        public override SmartTagBase GetSmartTag()
        {
            return new TextObjectSmartTag(this);
        }

        /// <inheritdoc/>
        public virtual bool InvokeEditor()
        {
            using (TextEditorForm form = new TextEditorForm(Report))
            {
                form.ExpressionText = Text;
                form.Brackets = Brackets;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Text = form.ExpressionText;
                    return true;
                }
            }
            return false;
        }

        internal virtual void FinishEdit(bool accept)
        {
            if (textBox == null)
                return;

            if (textBox.Modified && accept)
            {
                Text = textBox.Text;
                if (Report != null)
                    Report.Designer.SetModified(null, "Change", Name);
            }
            textBox.Dispose();
            textBox = null;
        }
        #endregion


        private void DrawDesign(FRPaintEventArgs e)
        {
            if (dragAccept)
                DrawDragAcceptFrame(e, Color.Silver);
        }
    }
}