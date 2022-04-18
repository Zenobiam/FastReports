using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
    partial class CellularTextObject
    {
        #region Property hiding
        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool AutoWidth
        {
            get { return base.AutoWidth; }
            set { base.AutoWidth = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new AutoShrinkMode AutoShrink
        {
            get { return base.AutoShrink; }
            set { base.AutoShrink = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new float AutoShrinkMinSize
        {
            get { return base.AutoShrinkMinSize; }
            set { base.AutoShrinkMinSize = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new int Angle
        {
            get { return base.Angle; }
            set { base.Angle = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool Underlines
        {
            get { return base.Underlines; }
            set { base.Underlines = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool RightToLeft
        {
            get { return base.RightToLeft; }
            set { base.RightToLeft = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new StringTrimming Trimming
        {
            get { return base.Trimming; }
            set { base.Trimming = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new float FontWidthRatio
        {
            get { return base.FontWidthRatio; }
            set { base.FontWidthRatio = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new float LineHeight
        {
            get { return base.LineHeight; }
            set { base.LineHeight = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new float FirstTabOffset
        {
            get { return base.FirstTabOffset; }
            set { base.FirstTabOffset = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new float TabWidth
        {
            get { return base.TabWidth; }
            set { base.TabWidth = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool Clip
        {
            get { return base.Clip; }
            set { base.Clip = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool Wysiwyg
        {
            get { return base.Wysiwyg; }
            set { base.Wysiwyg = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool ForceJustify
        {
            get { return base.ForceJustify; }
            set { base.ForceJustify = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This property is not relevant to this class.")]
        public new bool HtmlTags
        {
            get { return base.HtmlTags; }
            set { base.HtmlTags = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Padding Padding
        {
            get { return base.Padding; }
            set { base.Padding = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool CanBreak
        {
            get { return base.CanBreak; }
            set { base.CanBreak = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new BreakableComponent BreakTo
        {
            get { return base.BreakTo; }
            set { base.BreakTo = value; }
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void OnBeforeInsert(int flags)
        {
            base.OnBeforeInsert(flags);
            // to avoid applying last formatting
            Border.Lines = BorderLines.All;
        }

        /// <inheritdoc/>
        public override SizeF GetPreferredSize()
        {
            if ((Page as ReportPage).IsImperialUnitsUsed)
                return new SizeF(Units.Inches * 2.5f, Units.Inches * 0.3f);
            return new SizeF(Units.Centimeters * 6, Units.Centimeters * 0.75f);
        }
        #endregion


        private float GetCellWidthInternal(float fontHeight)
        {
            return (int)Math.Round((fontHeight + 10) / Page.SnapSize.Width) * Page.SnapSize.Width;
        }
    }
}