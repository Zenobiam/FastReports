using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Controls;
using FastReport.Forms;
using FastReport.Design.PageDesigners.Page;
#if !MONO
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design.Toolbars
{
    internal class TextToolbar : ToolbarBase
    {
        #region Fields
#if !MONO
        public FontComboBoxItem cbxName;
        public FontSizeComboBoxItem cbxSize;
        public ButtonItem btnBold;
        public ButtonItem btnItalic;
        public ButtonItem btnUnderline;
        public ButtonItem btnLeft;
        public ButtonItem btnCenter;
        public ButtonItem btnRight;
        public ButtonItem btnJustify;
        public ButtonItem btnTop;
        public ButtonItem btnMiddle;
        public ButtonItem btnBottom;
        public ColorButtonItem btnColor;
        public ButtonItem btnHighlight;
        public ButtonItem btnAngle;
#else
        public ToolStripFontComboBox cbxName;
        public ToolStripFontSizeComboBox cbxSize;
        public ToolStripButton btnBold;
        public ToolStripButton btnItalic;
        public ToolStripButton btnUnderline;
        public ToolStripButton btnLeft;
        public ToolStripButton btnCenter;
        public ToolStripButton btnRight;
        public ToolStripButton btnJustify;
        public ToolStripButton btnTop;
        public ToolStripButton btnMiddle;
        public ToolStripButton btnBottom;
        public ToolStripColorButton btnColor;
        public ToolStripButton btnHighlight;
        public ToolStripTextAngleButton btnAngle;
#endif
        #endregion

        #region Private Methods
        private void UpdateControls()
        {
            bool enabled = Designer.SelectedTextObjects.Enabled;

            cbxName.Enabled = enabled;
            cbxSize.Enabled = enabled;
            btnBold.Enabled = enabled;
            btnItalic.Enabled = enabled;
            btnUnderline.Enabled = enabled;
            btnLeft.Enabled = enabled;
            btnCenter.Enabled = enabled;
            btnRight.Enabled = enabled;
            btnJustify.Enabled = enabled;
            btnTop.Enabled = enabled;
            btnMiddle.Enabled = enabled;
            btnBottom.Enabled = enabled;
            btnColor.Enabled = enabled;
            btnHighlight.Enabled = enabled;
            btnAngle.Enabled = enabled;

            if (enabled)
            {
                TextObject text = Designer.SelectedTextObjects.First;

                cbxName.FontName = text.Font.Name;
                cbxSize.FontSize = text.Font.Size;
                btnBold.Checked = text.Font.Bold;
                btnItalic.Checked = text.Font.Italic;
                btnUnderline.Checked = text.Font.Underline;
                btnLeft.Checked = text.HorzAlign == HorzAlign.Left;
                btnCenter.Checked = text.HorzAlign == HorzAlign.Center;
                btnRight.Checked = text.HorzAlign == HorzAlign.Right;
                btnJustify.Checked = text.HorzAlign == HorzAlign.Justify;
                btnTop.Checked = text.VertAlign == VertAlign.Top;
                btnMiddle.Checked = text.VertAlign == VertAlign.Center;
                btnBottom.Checked = text.VertAlign == VertAlign.Bottom;
                if (text.TextFill is SolidFill)
                    btnColor.Color = (text.TextFill as SolidFill).Color;
            }
            else
            {
#if MONO
                cbxName.Text = "";
                cbxSize.Text = "";
#endif
                btnBold.Checked = false;
                btnItalic.Checked = false;
                btnUnderline.Checked = false;
                btnLeft.Checked = false;
                btnCenter.Checked = false;
                btnRight.Checked = false;
                btnJustify.Checked = false;
                btnTop.Checked = false;
                btnMiddle.Checked = false;
                btnBottom.Checked = false;
            }
        }

        private void cbxName_FontSelected(object sender, EventArgs e)
        {
            (Designer.ActiveReportTab.ActivePageDesigner as ReportPageDesigner).Workspace.Focus();
            Designer.SelectedTextObjects.SetFontName(cbxName.FontName);
        }

        private void cbxSize_SizeSelected(object sender, EventArgs e)
        {
            (Designer.ActiveReportTab.ActivePageDesigner as ReportPageDesigner).Workspace.Focus();
            Designer.SelectedTextObjects.SetFontSize(cbxSize.FontSize);
        }

        private void btnBold_Click(object sender, EventArgs e)
        {
            btnBold.Checked = !btnBold.Checked;
            Designer.SelectedTextObjects.ToggleFontStyle(FontStyle.Bold, btnBold.Checked);
        }

        private void btnItalic_Click(object sender, EventArgs e)
        {
            btnItalic.Checked = !btnItalic.Checked;
            Designer.SelectedTextObjects.ToggleFontStyle(FontStyle.Italic, btnItalic.Checked);
        }

        private void btnUnderline_Click(object sender, EventArgs e)
        {
            btnUnderline.Checked = !btnUnderline.Checked;
            Designer.SelectedTextObjects.ToggleFontStyle(FontStyle.Underline, btnUnderline.Checked);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetHAlign(HorzAlign.Left);
        }

        private void btnCenter_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetHAlign(HorzAlign.Center);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetHAlign(HorzAlign.Right);
        }

        private void btnJustify_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetHAlign(HorzAlign.Justify);
        }

        private void btnTop_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetVAlign(VertAlign.Top);
        }

        private void btnMiddle_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetVAlign(VertAlign.Center);
        }

        private void btnBottom_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetVAlign(VertAlign.Bottom);
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetTextColor(btnColor.DefaultColor);
        }

        private void btnHighlight_Click(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.InvokeHighlightEditor();
        }

#if !MONO
        private void btnAngle_Click(object sender, EventArgs e)
        {
            AnglePopup popup = new AnglePopup(Designer.FindForm());
            popup.Angle = Designer.SelectedTextObjects.First.Angle;
            popup.AngleChanged += popup_AngleChanged;
            popup.Show(this, btnAngle.Bounds.Left, btnAngle.Bounds.Bottom);
        }

        private void popup_AngleChanged(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetAngle((sender as AnglePopup).Angle);
        }
#else
        private void btnAngle_AngleChanged(object sender, EventArgs e)
        {
            Designer.SelectedTextObjects.SetAngle((sender as ToolStripTextAngleButton).Angle);
        }
#endif
        #endregion

        #region Public Methods
        public override void SelectionChanged()
        {
            base.SelectionChanged();
            UpdateControls();
        }

        public override void UpdateContent()
        {
            base.UpdateContent();
            UpdateControls();
        }

        public override void SaveState()
        {
            XmlItem xi = Config.Root.FindItem("Designer").FindItem(Name);
            xi.SetProp("MruFonts", cbxName.MruFonts);
        }

        public override void RestoreState()
        {
            XmlItem xi = Config.Root.FindItem("Designer").FindItem(Name);
            cbxName.MruFonts = xi.GetProp("MruFonts");
        }

        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Designer,Toolbar,Text");
            Text = res.Get("");

            SetItemText(cbxName, res.Get("Name"));
            SetItemText(cbxSize, res.Get("Size"));
            SetItemText(btnBold, res.Get("Bold"));
            SetItemText(btnItalic, res.Get("Italic"));
            SetItemText(btnUnderline, res.Get("Underline"));
            SetItemText(btnLeft, res.Get("Left"));
            SetItemText(btnCenter, res.Get("Center"));
            SetItemText(btnRight, res.Get("Right"));
            SetItemText(btnJustify, res.Get("Justify"));
            SetItemText(btnTop, res.Get("Top"));
            SetItemText(btnMiddle, res.Get("Middle"));
            SetItemText(btnBottom, res.Get("Bottom"));
            SetItemText(btnColor, res.Get("Color"));
            SetItemText(btnHighlight, res.Get("Highlight"));
            SetItemText(btnAngle, res.Get("Angle"));
        }

        public override void UpdateUIStyle()
        {
            base.UpdateUIStyle();
            btnColor.SetStyle(Designer.UIStyle);
#if !MONO
            Color controlColor = UIStyleUtils.GetControlColor(Designer.UIStyle);
            cbxName.ComboBoxEx.DisabledBackColor = controlColor;
            cbxSize.ComboBoxEx.DisabledBackColor = controlColor;
#endif
        }
#if !MONO
        public override void ReinitDpiSize()
        {
            base.ReinitDpiSize();
            cbxName.UpdateDpiDependencies();
            cbxName.ComboWidth = DpiHelper.ConvertUnits(120);
            cbxSize.UpdateDpiDependencies();
            cbxSize.ComboWidth = DpiHelper.ConvertUnits(50);
            cbxSize.DropDownHeight = DpiHelper.ConvertUnits(300);
            btnBold.Image = Res.GetImage(20);
            btnItalic.Image = Res.GetImage(21);
            btnUnderline.Image = Res.GetImage(22);
            btnLeft.Image = Res.GetImage(25);
            btnCenter.Image = Res.GetImage(26);
            btnRight.Image = Res.GetImage(27);
            btnJustify.Image = Res.GetImage(28);
            btnTop.Image = Res.GetImage(29);
            btnMiddle.Image = Res.GetImage(30);
            btnBottom.Image = Res.GetImage(31);
            btnColor.ImageIndex = 23;
            btnHighlight.Image = Res.GetImage(24);
            btnAngle.Image = Res.GetImage(64);
        }
#endif
#endregion

        public TextToolbar(Designer designer)
            : base(designer)
        {
            Name = "TextToolbar";

#if !MONO
            cbxName = new FontComboBoxItem();
            cbxName.Name = "cbxTextName";
            cbxName.ComboWidth = DpiHelper.ConvertUnits(120);
            cbxName.FontSelected += cbxName_FontSelected;
            cbxSize = new FontSizeComboBoxItem();
            cbxSize.Name = "cbxTextSize";
            cbxSize.ComboWidth = DpiHelper.ConvertUnits(50);
            cbxSize.DropDownHeight = DpiHelper.ConvertUnits(300);
            cbxSize.SizeSelected += cbxSize_SizeSelected;
            btnBold = CreateButton("btnTextBold", Res.GetImage(20), btnBold_Click);
            btnItalic = CreateButton("btnTextItalic", Res.GetImage(21), btnItalic_Click);
            btnUnderline = CreateButton("btnTextUnderline", Res.GetImage(22), btnUnderline_Click);
            btnLeft = CreateButton("btnTextLeft", Res.GetImage(25), btnLeft_Click);
            btnLeft.BeginGroup = true;
            btnCenter = CreateButton("btnTextCenter", Res.GetImage(26), btnCenter_Click);
            btnRight = CreateButton("btnTextRight", Res.GetImage(27), btnRight_Click);
            btnJustify = CreateButton("btnTextJustify", Res.GetImage(28), btnJustify_Click);
            btnTop = CreateButton("btnTextTop", Res.GetImage(29), btnTop_Click);
            btnTop.BeginGroup = true;
            btnMiddle = CreateButton("btnTextMiddle", Res.GetImage(30), btnMiddle_Click);
            btnBottom = CreateButton("btnTextBottom", Res.GetImage(31), btnBottom_Click);
            btnColor = new ColorButtonItem(23, Color.Black);
            btnColor.BeginGroup = true;
            btnColor.Name = "btnTextColor";
            btnColor.Click += btnColor_Click;
            btnHighlight = CreateButton("btnTextHighlight", Res.GetImage(24), btnHighlight_Click);
            btnAngle = CreateButton("btnTextAngle", Res.GetImage(64), btnAngle_Click);

            Items.AddRange(new BaseItem[] {
        cbxName, cbxSize, btnBold, btnItalic, btnUnderline,
        btnLeft, btnCenter, btnRight, btnJustify, 
        btnTop, btnMiddle, btnBottom, 
        btnColor, btnHighlight, btnAngle, CustomizeItem });
#else
            cbxName = new ToolStripFontComboBox();
            cbxName.Name = "cbxTextName";
            cbxName.Font = Font;
            cbxName.FontSelected += cbxName_FontSelected;
            cbxSize = new ToolStripFontSizeComboBox();
            cbxSize.Name = "cbxTextSize";
            cbxSize.Font = Font;
            cbxSize.DropDownHeight = 300;
            cbxSize.SizeSelected += cbxSize_SizeSelected;
            btnBold = CreateButton("btnTextBold", Res.GetImage(20), btnBold_Click);
            btnItalic = CreateButton("btnTextItalic", Res.GetImage(21), btnItalic_Click);
            btnUnderline = CreateButton("btnTextUnderline", Res.GetImage(22), btnUnderline_Click);
            btnLeft = CreateButton("btnTextLeft", Res.GetImage(25), btnLeft_Click);
            btnCenter = CreateButton("btnTextCenter", Res.GetImage(26), btnCenter_Click);
            btnRight = CreateButton("btnTextRight", Res.GetImage(27), btnRight_Click);
            btnJustify = CreateButton("btnTextJustify", Res.GetImage(28), btnJustify_Click);
            btnTop = CreateButton("btnTextTop", Res.GetImage(29), btnTop_Click);
            btnMiddle = CreateButton("btnTextMiddle", Res.GetImage(30), btnMiddle_Click);
            btnBottom = CreateButton("btnTextBottom", Res.GetImage(31), btnBottom_Click);
            btnColor = new ToolStripColorButton(23, Color.Black);
            btnColor.Name = "btnTextColor";
            btnColor.ButtonClick += btnColor_Click;
            btnHighlight = CreateButton("btnTextHighlight", Res.GetImage(24), btnHighlight_Click);
            btnAngle = new ToolStripTextAngleButton();
            btnAngle.Name = "btnTextAngle";
            btnAngle.Image = Res.GetImage(64);
            btnAngle.AngleChanged += btnAngle_AngleChanged;

            Items.AddRange(new ToolStripItem[] {
        cbxName, cbxSize, btnBold, btnItalic, btnUnderline, new ToolStripSeparator(),
        btnLeft, btnCenter, btnRight, btnJustify, new ToolStripSeparator(),
        btnTop, btnMiddle, btnBottom, new ToolStripSeparator(),
        btnColor, btnHighlight, btnAngle });
#endif

            Localize();
        }

    }

}
