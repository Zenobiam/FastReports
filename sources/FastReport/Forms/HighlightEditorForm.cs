using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Controls;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
  internal partial class HighlightEditorForm : BaseDialogForm
  {
    private Report report;
    private ImageList images;
    private ConditionCollection conditions;
    private Timer refreshTimer;
    private bool updating;
    
    public ConditionCollection Conditions
    {
      get { return conditions; }
      set
      {
        conditions = new ConditionCollection();
        conditions.Assign(value);
        PopulateConditions();
      }
    }
    
    private HighlightCondition CurrentCondition
    {
      get
      {
        if (lvConditions.SelectedItems.Count == 0)
          return null;
        return lvConditions.SelectedItems[0].Tag as HighlightCondition;
      }
    }

    private Bitmap GetImage(HighlightCondition c)
    {
      Bitmap bmp = new Bitmap((int)(16 * FormRatio), (int)(16 * FormRatio));
      using (Graphics g = Graphics.FromImage(bmp))
      {
        g.FillRectangle(Brushes.White, new RectangleF(0, 0, 16 * FormRatio, 16 * FormRatio));

        using (TextObject sample = new TextObject())
        {
          sample.Bounds = new RectangleF(0, 0, 16, 16);
          sample.ApplyCondition(c);
          sample.Font = ParseFontSize(new Font("Times New Roman", 12), 12); //DpiHelper.ConvertUnits(new Font("Times New Roman", 12, FontStyle.Bold));
          sample.Text = "A";
          sample.HorzAlign = HorzAlign.Center;
          sample.VertAlign = VertAlign.Center;
          
          using (GraphicCache cache = new GraphicCache())
          {
            sample.Draw(new FRPaintEventArgs(g, 1 * FormRatio, 1 * FormRatio, cache));
          }
        }
      }

      return bmp;
    }

    private int GetImageIndex(HighlightCondition c)
    {
      Bitmap bmp = GetImage(c);
      images.Images.Add(bmp, bmp.GetPixel(0, 15));
      return images.Images.Count - 1;
    }

    private void RefreshSample()
    {
      pnSample.Refresh();
    }
    
    private void UpdateControls()
    {
      bool enabled = CurrentCondition != null;
      btnDelete.Enabled = enabled;
      btnEdit.Enabled = enabled;
      btnUp.Enabled = enabled;
      btnDown.Enabled = enabled;
      gbStyle.Enabled = enabled;
      
      if (enabled)
      {
        updating = true;
        cbApplyBorder.Checked = CurrentCondition.ApplyBorder;
        cbApplyFill.Checked = CurrentCondition.ApplyFill;
        cbApplyTextFill.Checked = CurrentCondition.ApplyTextFill;
        cbApplyFont.Checked = CurrentCondition.ApplyFont;
        cbVisible.Checked = CurrentCondition.Visible;

        btnBorder.Enabled = CurrentCondition.ApplyBorder;
        btnFill.Enabled = CurrentCondition.ApplyFill;
        btnTextColor.Enabled = CurrentCondition.ApplyTextFill;
        btnFont.Enabled = CurrentCondition.ApplyFont;
        updating = false;
      }
    }

    private void SetApply()
    {
      foreach (ListViewItem li in lvConditions.SelectedItems)
      {
        HighlightCondition c = li.Tag as HighlightCondition;
        c.ApplyBorder = cbApplyBorder.Checked;
        c.ApplyFill = cbApplyFill.Checked;
        c.ApplyTextFill = cbApplyTextFill.Checked;
        c.ApplyFont = cbApplyFont.Checked;
        c.Visible = cbVisible.Checked;
        li.ImageIndex = GetImageIndex(c);
      }
      RefreshSample();
    }
    
    private void PopulateConditions()
    {
      foreach (HighlightCondition c in conditions)
      {
        ListViewItem li = lvConditions.Items.Add(c.Expression, GetImageIndex(c));
        li.Tag = c;
      }
      if (lvConditions.Items.Count > 0)
        lvConditions.Items[0].Selected = true;
      UpdateControls();  
    }

    private void pnSample_Paint(object sender, PaintEventArgs e)
    {
      if (CurrentCondition == null)
        return;
      
      TextObject sample = new TextObject();
      sample.Text = Res.Get("Misc,Sample");
      sample.ApplyCondition(CurrentCondition);
      sample.Bounds = new RectangleF(2, 2, pnSample.Width - 4, pnSample.Height - 4);
      sample.HorzAlign = HorzAlign.Center;
      sample.VertAlign = VertAlign.Center;
            sample.Font = ParseFontSize(sample.Font, 10);
      using (GraphicCache cache = new GraphicCache())
      {
        sample.Draw(new FRPaintEventArgs(e.Graphics, 1, 1, cache));
      }
    }

    private void FTimer_Tick(object sender, EventArgs e)
    {
      UpdateControls();
      RefreshSample();
      refreshTimer.Stop();
    }

    private void lvConditions_SelectedIndexChanged(object sender, EventArgs e)
    {
      refreshTimer.Start();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      using (ExpressionEditorForm form = new ExpressionEditorForm(report))
      {
        form.ExpressionText = report.ScriptLanguage == Language.CSharp ? "Value == 0" : "Value = 0";
        if (form.ShowDialog() == DialogResult.OK)
        {
          HighlightCondition c = new HighlightCondition();
          conditions.Add(c);
          c.Expression = form.ExpressionText;

          ListViewItem li = lvConditions.Items.Add(c.Expression, GetImageIndex(c));
          li.Tag = c;
          lvConditions.SelectedItems.Clear();
          li.Selected = true;
        }
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      while (lvConditions.SelectedItems.Count > 0)
      {
        HighlightCondition c = lvConditions.SelectedItems[0].Tag as HighlightCondition;
        conditions.Remove(c);
        lvConditions.Items.Remove(lvConditions.SelectedItems[0]);
      }
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
      if (lvConditions.SelectedItems.Count == 1)
      {
        using (ExpressionEditorForm form = new ExpressionEditorForm(report))
        {
          form.ExpressionText = CurrentCondition.Expression;
          if (form.ShowDialog() == DialogResult.OK)
          {
            CurrentCondition.Expression = form.ExpressionText;
            lvConditions.SelectedItems[0].Text = CurrentCondition.Expression;
          }
        }
      }  
    }

    private void btnUp_Click(object sender, EventArgs e)
    {
      if (lvConditions.SelectedItems.Count != 1)
        return;
      int index = lvConditions.SelectedIndices[0];
      if (index > 0)
      {
        ListViewItem li = lvConditions.SelectedItems[0];
        lvConditions.Items.Remove(li);
        lvConditions.Items.Insert(index - 1, li);
        HighlightCondition c = li.Tag as HighlightCondition;
        conditions.Remove(c);
        conditions.Insert(index - 1, c);
      }
    }

    private void btnDown_Click(object sender, EventArgs e)
    {
      if (lvConditions.SelectedItems.Count != 1)
        return;
      int index = lvConditions.SelectedIndices[0];
      if (index < lvConditions.Items.Count - 1)
      {
        ListViewItem li = lvConditions.SelectedItems[0];
        lvConditions.Items.Remove(li);
        lvConditions.Items.Insert(index + 1, li);
        HighlightCondition c = li.Tag as HighlightCondition;
        conditions.Remove(c);
        conditions.Insert(index + 1, c);
      }
    }

    private void cbApplyBorder_CheckedChanged(object sender, EventArgs e)
    {
      if (updating || CurrentCondition == null)
        return;
      btnBorder.Enabled = cbApplyBorder.Checked;
      SetApply();
    }

    private void cbApplyFill_CheckedChanged(object sender, EventArgs e)
    {
      if (updating || CurrentCondition == null)
        return;
      btnFill.Enabled = cbApplyFill.Checked;
      SetApply();
    }

    private void cbApplyTextFill_CheckedChanged(object sender, EventArgs e)
    {
      if (updating || CurrentCondition == null)
        return;
      btnTextColor.Enabled = cbApplyTextFill.Checked;
      SetApply();
    }

    private void cbApplyFont_CheckedChanged(object sender, EventArgs e)
    {
      if (updating || CurrentCondition == null)
        return;
      btnFont.Enabled = cbApplyFont.Checked;
      SetApply();
    }

    private void btnBorder_Click(object sender, EventArgs e)
    {
      if (CurrentCondition == null)
        return;

      using (BorderEditorForm editor = new BorderEditorForm())
      {
        editor.Border = CurrentCondition.Border.Clone();
        if (editor.ShowDialog() == DialogResult.OK)
        {
          foreach (ListViewItem li in lvConditions.SelectedItems)
          {
            HighlightCondition c = li.Tag as HighlightCondition;
            c.Border = editor.Border.Clone();
            li.ImageIndex = GetImageIndex(c);
          }
          RefreshSample();
        }
      }
    }

    private void btnFill_Click(object sender, EventArgs e)
    {
      if (CurrentCondition == null)
        return;
      using (FillEditorForm editor = new FillEditorForm())
      {
        editor.Fill = CurrentCondition.Fill.Clone();
        if (editor.ShowDialog() == DialogResult.OK)
        {
          foreach (ListViewItem li in lvConditions.SelectedItems)
          {
            HighlightCondition c = li.Tag as HighlightCondition;
            c.Fill = editor.Fill.Clone();
            li.ImageIndex = GetImageIndex(c);
          }
          RefreshSample();
        }
      }
    }

    private void btnTextColor_Click(object sender, EventArgs e)
    {
      if (CurrentCondition == null)
        return;
#if !MONO
            DpiHelper.RescaleWithNewDpi(() => 
            {
#endif
                ColorPopup popup = new ColorPopup(this);
                if (CurrentCondition.TextFill is SolidFill)
                    popup.Color = (CurrentCondition.TextFill as SolidFill).Color;
                popup.ColorSelected += new EventHandler(popup_ColorSelected);
                popup.Show(btnTextColor, 0, btnTextColor.Height);
#if !MONO
        }, FormRatio);
#endif
    }

    private void popup_ColorSelected(object sender, EventArgs e)
    {
      Color color = (sender as ColorPopup).Color;
      foreach (ListViewItem li in lvConditions.SelectedItems)
      {
        HighlightCondition c = li.Tag as HighlightCondition;
        c.TextFill = new SolidFill(color);
        li.ImageIndex = GetImageIndex(c);
      }
      RefreshSample();
    }

    private void btnFont_Click(object sender, EventArgs e)
    {
      if (CurrentCondition == null)
        return;
      using (FontDialog dialog = new FontDialog())
      {
        dialog.Font = CurrentCondition.Font;
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          foreach (ListViewItem li in lvConditions.SelectedItems)
          {
            HighlightCondition c = li.Tag as HighlightCondition;
            c.Font = dialog.Font;
            li.ImageIndex = GetImageIndex(c);
          }
          RefreshSample();
        }
      }
    }

    private void cbVisible_CheckedChanged(object sender, EventArgs e)
    {
      if (updating || CurrentCondition == null)
        return;
      SetApply();
    }

    private void HighlightEditorForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Done();
    }

    private void Init()
    {
      images = new ImageList();
      images.ImageSize = new Size((int)(16 * FormRatio), (int)(16 * FormRatio));
      images.ColorDepth = ColorDepth.Depth24Bit;
      lvConditions.SmallImageList = images;
      refreshTimer = new Timer();
      refreshTimer.Interval = 50;
      refreshTimer.Tick += new EventHandler(FTimer_Tick);
      StartPosition = FormStartPosition.Manual;
      Config.RestoreFormState(this);
    }
    
    private void Done()
    {
      images.Dispose();
      Config.SaveFormState(this);
    }

    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,HighlightEditor");
      Text = res.Get("");

      gbConditions.Text = res.Get("Conditions");
      btnAdd.Text = res.Get("Add");
      btnDelete.Text = res.Get("Delete");
      btnEdit.Text = res.Get("Edit");
      btnUp.Image = Res.GetImage(208);
      btnDown.Image = Res.GetImage(209);

      gbStyle.Text = res.Get("Style");
      btnBorder.Image = Res.GetImage(36);
      btnBorder.Text = Res.Get("Forms,Style,Border");
      btnFill.Image = Res.GetImage(38);
      btnFill.Text = res.Get("Fill");
      btnTextColor.Image = Res.GetImage(23);
      btnTextColor.Text = res.Get("TextColor");
      btnFont.Text = res.Get("Font");
      btnFont.Image = Res.GetImage(59);
      cbVisible.Text = res.Get("Visible");
    }
    
    public HighlightEditorForm(Report report)
    {
            this.report = report;
      InitializeComponent();
      Init();
      Localize();
            Scale();
    }

#if !MONO
        protected override void UpdateResources()
        {
            btnUp.Image = GetImage(208);
            btnDown.Image = GetImage(209);
            btnBorder.Image = GetImage(36);
            btnFill.Image = GetImage(38);
            btnTextColor.Image = GetImage(23);
            btnFont.Image = GetImage(59);
        }
#endif
    }
}

