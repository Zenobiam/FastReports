using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
using System.Drawing.Drawing2D;
using System.IO;
using FastReport.Controls;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
  internal partial class FillEditorForm : BaseDialogForm
  {
    private FillBase fill;
    private FillBase[] fills;
    private bool updating;
    
    private SolidFill SolidFill
    {
      get { return fills[0] as SolidFill; }
      set { fills[0] = value; }
    }
    
    private LinearGradientFill LinearGradientFill
    {
      get { return fills[1] as LinearGradientFill; }
      set { fills[1] = value; }
    }

    private PathGradientFill PathGradientFill
    {
      get { return fills[2] as PathGradientFill; }
      set { fills[2] = value; }
    }

    private HatchFill HatchFill
    {
      get { return fills[3] as HatchFill; }
      set { fills[3] = value; }
    }

    private GlassFill GlassFill
    {
      get { return fills[4] as GlassFill; }
      set { fills[4] = value; }
    }

    private TextureFill TextureFill
    {
        get { return fills[5] as TextureFill; }
        set { fills[5] = value; }
    }

    public FillBase Fill
    {
      get { return fill; }
      set 
      { 
        fill = value; 
        UpdateControls();
      }
    }
   
    private void PopulateFills()
    {
      fills = new FillBase[] { 
        new SolidFill(Color.Silver), new LinearGradientFill(), new PathGradientFill(), 
        new HatchFill(), new GlassFill(), new TextureFill() }; 
      Rectangle rect = new Rectangle(0, 0, DpiHelper.ConvertUnits(31), DpiHelper.ConvertUnits(31));
      ImageList images = new ImageList();
      images.ImageSize = DpiHelper.ConvertUnits(new Size(32, 32));
      images.ColorDepth = ColorDepth.Depth24Bit;
      
      foreach (FillBase fill in fills)
      {
        Bitmap bmp = DpiHelper.ConvertBitmap(new Bitmap(32, 32));
        using (Graphics g = Graphics.FromImage(bmp))
        using (Brush brush = fill.CreateBrush(new RectangleF(0, 0, DpiHelper.ConvertUnits(31), DpiHelper.ConvertUnits(31))))
        {
          g.FillRectangle(brush, rect);
          g.DrawRectangle(Pens.Black, rect);
        }
        images.Images.Add(bmp, Color.Transparent);
      }
      (fills[0] as SolidFill).Color = Color.Transparent;
    }

    private void UpdateControls()
    {
      if (fill is SolidFill)
      {
        SolidFill = fill as SolidFill;
        cbxSolidColor.Color = SolidFill.Color;
        pcPanels.ActivePageIndex = 0;
      }
      else if (fill is LinearGradientFill)
      {
        LinearGradientFill = fill as LinearGradientFill;
        cbxLinearStartColor.Color = LinearGradientFill.StartColor;
        cbxLinearEndColor.Color = LinearGradientFill.EndColor;
        trbLinearFocus.Value = (int)Math.Round(LinearGradientFill.Focus * 100);
        trbLinearContrast.Value = (int)Math.Round(LinearGradientFill.Contrast * 100);
        acLinearAngle.Angle = LinearGradientFill.Angle;
        pcPanels.ActivePageIndex = 1;
      }
      else if (fill is PathGradientFill)
      {
        PathGradientFill = fill as PathGradientFill;
        cbxPathCenterColor.Color = PathGradientFill.CenterColor;
        cbxPathEdgeColor.Color = PathGradientFill.EdgeColor;
        rbPathEllipse.Checked = PathGradientFill.Style == PathGradientStyle.Elliptic;
        rbPathRectangle.Checked = PathGradientFill.Style == PathGradientStyle.Rectangular;
        pcPanels.ActivePageIndex = 2;
      }
      else if (fill is HatchFill)
      {
        HatchFill = fill as HatchFill;
        cbxHatchForeColor.Color = HatchFill.ForeColor;
        cbxHatchBackColor.Color = HatchFill.BackColor;
        cbxHatch.Style = HatchFill.Style;
        pcPanels.ActivePageIndex = 3;
      }
      else if (fill is GlassFill)
      {
        GlassFill = fill as GlassFill;
        cbxGlassColor.Color = GlassFill.Color;
        trbGlassBlend.Value = (int)Math.Round(GlassFill.Blend * 100);
        cbGlassHatch.Checked = GlassFill.Hatch;
        pcPanels.ActivePageIndex = 4;
      }
      else if (fill is TextureFill)
      {
          TextureFill = fill as TextureFill;
          cbWrapMode.SelectedItem = TextureFill.WrapMode.ToString();
          numImgWidth.Value = TextureFill.ImageWidth;
          numImgHeight.Value = TextureFill.ImageHeight;
          chBxPreserveRatio.Checked = TextureFill.PreserveAspectRatio;
          numOffsetX.Value = TextureFill.ImageOffsetX;
          numOffsetY.Value = TextureFill.ImageOffsetY;
          pcPanels.ActivePageIndex = 5;
      }
    }
    
    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,Fill");
      Text = res.Get("");

      pnSolid.Text = res.Get("Solid");
      pnLinear.Text = res.Get("LinearGradient");
      pnPath.Text = res.Get("PathGradient");
      pnHatch.Text = res.Get("Hatch");
      pnGlass.Text = res.Get("Glass");
      pnTexture.Text = res.Get("Texture");
      lblWrapMode.Text = res.Get("WrapMode");

      gpTextureFill.Text = res.Get("ChooseImage");
      gpImageSize.Text = res.Get("ImageSize");
      lblImgWidth.Text = res.Get("ImageWidth");
      lblImgHeight.Text = res.Get("ImageHeight");
      chBxPreserveRatio.Text = res.Get("PreserveAspectRatio");
      gbOffset.Text = res.Get("ImageOffset");
      gbSolidColors.Text = res.Get("Color");
      lblSolidColor.Text = res.Get("Color");
      gbLinearColors.Text = res.Get("Colors");
      lblLinearStartColor.Text = res.Get("StartColor");
      lblLinearEndColor.Text = res.Get("EndColor");
      gbLinearOptions.Text = res.Get("Options");
      lblLinearFocus.Text = res.Get("Focus");
      lblLinearContrast.Text = res.Get("Contrast");
      gbLinearAngle.Text = res.Get("Angle");
      gbPathColors.Text = res.Get("Colors");
      lblPathCenterColor.Text = res.Get("CenterColor");
      lblPathEdgeColor.Text = res.Get("EdgeColor");
      gbPathShape.Text = res.Get("Shape");
      rbPathEllipse.Text = res.Get("Ellipse");
      rbPathRectangle.Text = res.Get("Rectangle");
      gbHatchColors.Text = res.Get("Colors");
      lblHatchForeColor.Text = res.Get("ForeColor");
      lblHatchBackColor.Text = res.Get("BackColor");
      gbHatchStyle.Text = res.Get("Style");
      gbGlassOptions.Text = res.Get("Options");
      lblGlassColor.Text = res.Get("Color");
      lblGlassBlend.Text = res.Get("Blend");
      cbGlassHatch.Text = res.Get("GlassHatch");

      btnOpenFileDialog.Image = Res.GetImage(1);
    }

    private void pcPanels_PageSelected(object sender, EventArgs e)
    {
      if (updating)
        return;
        
      updating = true;  
      int currentFill = pcPanels.ActivePageIndex;
      Fill = fills[currentFill];
      pnSample.Refresh();
      updating = false;
    }
    
    private void pnSample_Paint(object sender, PaintEventArgs e)
    {
      Graphics g = e.Graphics;
      if (Fill is TextureFill)
      {
        (Fill as TextureFill).Draw(new FRPaintEventArgs(g, 1, 1, null), 
          new RectangleF(0, 0, pnSample.Width, pnSample.Height));
      }
      if (Fill is GlassFill)
      {
        (Fill as GlassFill).Draw(new FRPaintEventArgs(g, 1, 1, null), 
          new RectangleF(0, 0, pnSample.Width, pnSample.Height));
      }
      else if (Fill != null)
      {
        Brush brush = null;
        
        if (Fill is SolidFill && Fill.IsTransparent)
          brush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.Gainsboro);
        else
          brush = Fill.CreateBrush(new RectangleF(0, 0, pnSample.Width, pnSample.Height));
          
        g.FillRectangle(brush, 0, 0, pnSample.Width, pnSample.Height);
        brush.Dispose();
      }
      
      g.DrawRectangle(SystemPens.ControlDark, 0, 0, pnSample.Width - 1, pnSample.Height - 1);
    }
    
    private void Change()
    {
      pnSample.Refresh();
    }

    private void cbxSolidColor_ColorSelected(object sender, EventArgs e)
    {
      SolidFill.Color = cbxSolidColor.Color;
      Change();
    }

    private void cbxLinearStartColor_ColorSelected(object sender, EventArgs e)
    {
      LinearGradientFill.StartColor = cbxLinearStartColor.Color;
      Change();
    }

    private void cbxLinearEndColor_ColorSelected(object sender, EventArgs e)
    {
      LinearGradientFill.EndColor = cbxLinearEndColor.Color;
      Change();
    }

    private void trbLinearFocus_ValueChanged(object sender, EventArgs e)
    {
      LinearGradientFill.Focus = trbLinearFocus.Value / 100f;
      Change();
    }

    private void trbLinearContrast_ValueChanged(object sender, EventArgs e)
    {
      LinearGradientFill.Contrast = trbLinearContrast.Value / 100f;
      Change();
    }

    private void acLinearAngle_AngleChanged(object sender, EventArgs e)
    {
      LinearGradientFill.Angle = acLinearAngle.Angle;
      Change();
    }

    private void cbxPathCenterColor_ColorSelected(object sender, EventArgs e)
    {
      PathGradientFill.CenterColor = cbxPathCenterColor.Color;
      Change();
    }

    private void cbxPathEdgeColor_ColorSelected(object sender, EventArgs e)
    {
      PathGradientFill.EdgeColor = cbxPathEdgeColor.Color;
      Change();
    }

    private void rbPathEllipse_CheckedChanged(object sender, EventArgs e)
    {
      PathGradientFill.Style = rbPathEllipse.Checked ? PathGradientStyle.Elliptic : PathGradientStyle.Rectangular;
      Change();
    }

    private void cbxHatchForeColor_ColorSelected(object sender, EventArgs e)
    {
      HatchFill.ForeColor = cbxHatchForeColor.Color;
      Change();
    }

    private void cbxHatchBackColor_ColorSelected(object sender, EventArgs e)
    {
      HatchFill.BackColor = cbxHatchBackColor.Color;
      Change();
    }

    private void cbxHatch_HatchSelected(object sender, EventArgs e)
    {
      HatchFill.Style = cbxHatch.Style;
      Change();
    }

    private void cbxGlassColor_ColorSelected(object sender, EventArgs e)
    {
      GlassFill.Color = cbxGlassColor.Color;
      Change();
    }

    private void trbGlassBlend_ValueChanged(object sender, EventArgs e)
    {
      GlassFill.Blend = trbGlassBlend.Value / 100f;
      Change();
    }

    private void cbGlassHatch_CheckedChanged(object sender, EventArgs e)
    {
      GlassFill.Hatch = cbGlassHatch.Checked;
      Change();
    }

   private void btnOpenFileDialog_Click(object sender, EventArgs e)
   {
       using (OpenFileDialog dialog = new OpenFileDialog())
       {
           dialog.Filter = Res.Get("FileFilters,Images");
           if (dialog.ShowDialog() == DialogResult.OK)
           {
               TextureFill = new TextureFill(File.ReadAllBytes(dialog.FileName));
               Fill = TextureFill;
               Change();
           }
       }
   }

  private void numImgWidth_ValueChanged(object sender, EventArgs e)
  {
      if (TextureFill.ImageWidth == (int)numImgWidth.Value)
          return; 
      TextureFill.ImageWidth = (int)numImgWidth.Value;
      numImgHeight.Value = TextureFill.ImageHeight;
      Change();
  }

  private void numImgHeight_ValueChanged(object sender, EventArgs e)
  {
      if (TextureFill.ImageHeight == (int)numImgHeight.Value)
          return;
      TextureFill.ImageHeight = (int)numImgHeight.Value;
      Change();
      numImgWidth.Value = TextureFill.ImageWidth;
  }

  private void numOffsetX_ValueChanged(object sender, EventArgs e)
  {
      if (TextureFill.ImageOffsetX == (int)numOffsetX.Value)
            return;
      TextureFill.ImageOffsetX = (int)numOffsetX.Value;
      Change();
      numOffsetX.Value = TextureFill.ImageOffsetX;
  }

  private void numOffsetY_ValueChanged(object sender, EventArgs e)
  {
      if (TextureFill.ImageOffsetY == (int)numOffsetY.Value)
            return;
      TextureFill.ImageOffsetY = (int)numOffsetY.Value;
      Change();
      numOffsetY.Value = TextureFill.ImageOffsetY;
  }
  private void chBxPreserveRatio_CheckedChanged(object sender, EventArgs e)
  {
      TextureFill.PreserveAspectRatio = chBxPreserveRatio.Checked;
      Fill = TextureFill;
      Change();
  }

  private void cbWrapMode_SelectedIndexChanged(object sender, EventArgs e)
  {
      switch (Convert.ToString(cbWrapMode.SelectedItem))
      {
          case "Clamp":
              TextureFill.WrapMode = WrapMode.Clamp;
              break;
          case "Tile":
              TextureFill.WrapMode = WrapMode.Tile;
              break;
          case "TileFlipX":
              TextureFill.WrapMode = WrapMode.TileFlipX;
              break;
          case "TileFlipY":
              TextureFill.WrapMode = WrapMode.TileFlipY;
              break;
          case "TileFlipXY":
              TextureFill.WrapMode = WrapMode.TileFlipXY;
              break;
      }
      Fill = TextureFill;
      Change();
  }

    public FillEditorForm()
    {
      InitializeComponent();
      PopulateFills();
      Localize();
            ControlScalingBegin += FillEditorForm_ControlScalingBegin;
            Scale();
    }

        private void FillEditorForm_ControlScalingBegin(object sender, Bools boolArgs)
        {
#if !MONO
            if(sender is AngleControl)
            {
                AngleControl ac = sender as AngleControl;
                ac.Font = ParseFontSize(ac.Font, 9);// DpiHelper.ConvertUnits(ac.Font, true);
                ac.NumericAngle.Font = ParseFontSize(DrawUtils.DefaultFont, DrawUtils.DefaultFont.Size);//DpiHelper.ConvertUnits(DrawUtils.Default96Font);

                ac.Location = DpiHelper.ConvertUnits(ac.Location);
                ac.Size = DpiHelper.ConvertUnits(ac.Size);
                ac.Padding = DpiHelper.ConvertUnits(ac.Padding);
                ac.Margin = DpiHelper.ConvertUnits(ac.Margin);

                ac.NumericAngle.Location = new Point(10, ac.Height - ac.NumericAngle.Height - DpiHelper.ConvertUnits(8));
                ac.NumericAngle.Size = new Size(Width - 20, 20);
                boolArgs.NeedCheckControl = false;
                boolArgs.NeedScaleThisControl = false;
            }
#endif
        }

        internal class FillSample : Panel
    {
      public FillSample()
      {
        Size = new Size(156, 156);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      }
    }
  }
}