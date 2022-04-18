using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using FastReport.Utils;
using FastReport.TypeConverters;
using FastReport.Design.PageDesigners.Page;
using System.Globalization;

namespace FastReport.Forms
{
  internal partial class CustomLabelForm : BaseDialogForm
  {
    private XmlItem labelParameters;
    private PrinterSettings printerSettings;
    private Report sampleReport;
    private bool updating;

    public XmlItem LabelParameters
    {
      get { return labelParameters; }
    }
    
    private string ConvertMMtoUnits(float value)
    {
      return Converter.ToString(value, typeof(PaperConverter));
    }
    
    private float ConvertUnitsToInches(string value)
    {
      float floatValue = Converter.StringToFloat(value, true);
      switch (ReportWorkspace.Grid.GridUnits)
      {
        case PageUnits.Centimeters:
          floatValue /= 2.54f;
          break;
        
        case PageUnits.HundrethsOfInch:
          floatValue /= 100;
          break;
        
        case PageUnits.Inches:
          // do nothing
          break;
        
        case PageUnits.Millimeters:
          floatValue /= 25.4f;
          break;
      }
      
      return floatValue;
    }
    
    private string ConvertToXml(float value)
    {
      return Converter.ToString(Math.Round(value, 3));
    }

    private void UpdateSample()
    {
      float paperWidth = ConvertUnitsToInches(tbPaperWidth.Text);
      float paperHeight = ConvertUnitsToInches(tbPaperHeight.Text);
      float leftMargin = ConvertUnitsToInches(tbLeftMargin.Text);
      float topMargin = ConvertUnitsToInches(tbTopMargin.Text);

      float labelWidth = ConvertUnitsToInches(tbLabelWidth.Text);
      float labelHeight = ConvertUnitsToInches(tbLabelHeight.Text);
      int rows = (int)udRows.Value;
      int columns = (int)udColumns.Value;
      float rowGap = ConvertUnitsToInches(tbRowGap.Text);
      float columnGap = ConvertUnitsToInches(tbColumnGap.Text);
      
      labelParameters.Name = "";
            labelParameters.ClearProps();

      labelParameters.SetProp("Name",tbName.Text);
      labelParameters.SetProp("Width", ConvertToXml(labelWidth));
      labelParameters.SetProp("Height", ConvertToXml(labelHeight));
      labelParameters.SetProp("PaperWidth", ConvertToXml(paperWidth));
      labelParameters.SetProp("PaperHeight", ConvertToXml(paperHeight));
      if (cbLandscape.Checked)
        labelParameters.SetProp("Landscape", "true");
      labelParameters.SetProp("LeftMargin", ConvertToXml(leftMargin));
      labelParameters.SetProp("TopMargin", ConvertToXml(topMargin));
      labelParameters.SetProp("Rows", rows.ToString());
      labelParameters.SetProp("Columns", columns.ToString());
      labelParameters.SetProp("RowGap", ConvertToXml(rowGap));
      labelParameters.SetProp("ColumnGap", ConvertToXml(columnGap));

      ReportPage page = sampleReport.Pages[0] as ReportPage;
      page.Clear();
      page.Landscape = cbLandscape.Checked;
      page.PaperWidth = paperWidth * 25.4f;
      page.PaperHeight = paperHeight * 25.4f;
      page.LeftMargin = leftMargin * 25.4f;
      page.TopMargin = topMargin * 25.4f;

      DataBand band = new DataBand();
      band.Parent = page;
      
      bool fit = true;
      for (int x = 0; x < columns; x++)
      {
        for (int y = 0; y < rows; y++)
        {
          ShapeObject shape = new ShapeObject();
          shape.Parent = band;
          shape.Shape = ShapeKind.RoundRectangle;
          shape.Border.Color = Color.Gray;
          shape.Bounds = new RectangleF(x * (labelWidth + columnGap) * 96,
            y * (labelHeight + rowGap) * 96,
            labelWidth * 96,
            labelHeight * 96);
          if (shape.Right / Units.Millimeters > page.PaperWidth - page.LeftMargin + 0.1f || 
            shape.Bottom / Units.Millimeters > page.PaperHeight - page.TopMargin + 0.1f)
          {  
            (shape.Fill as SolidFill).Color = Color.Red;
            fit = false;
          }  
        }
      }
      
      rcSample.Report = sampleReport;
      lblWarning.Visible = !fit;
      btnOk.Enabled = fit;
    }

    private void cbxPaper_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (updating)
        return;
      foreach (PaperSize ps in printerSettings.PaperSizes)
      {
        if (ps.PaperName == (string)cbxPaper.SelectedItem)
        {
          float psWidth = ps.Width / 100f * 25.4f;
          float psHeight = ps.Height / 100f * 25.4f;
          if (ps.Kind == PaperKind.A4)
          {
            psWidth = 210;
            psHeight = 297;
          }
          if (cbLandscape.Checked)
          {
            float ex = psWidth;
            psWidth = psHeight;
            psHeight = ex;
          }  

          updating = true;
          tbPaperWidth.Text = ConvertMMtoUnits(psWidth);
          tbPaperHeight.Text = ConvertMMtoUnits(psHeight);
          updating = false;
          break;
        }
      }
      UpdateSample();
    }

    private void cbLandscape_CheckedChanged(object sender, EventArgs e)
    {
      if (updating)
        return;
      string w = tbPaperWidth.Text;
      string h = tbPaperHeight.Text;

      // avoid reset papersize to custom
      updating = true;
      tbPaperWidth.Text = h;
      tbPaperHeight.Text = w;
      updating = false;
      UpdateSample();
    }

    private void tbPaperWidth_TextChanged(object sender, EventArgs e)
    {
      if (updating)
        return;
      cbxPaper.SelectedIndex = 0;
      UpdateSample();
    }

    private void tbLeftMargin_TextChanged(object sender, EventArgs e)
    {
      if (updating)
        return;
      UpdateSample();
    }

    public void Init(string suggestedName)
    {
      labelParameters = new XmlItem();
      printerSettings = new PrinterSettings();
      sampleReport = new Report();
      sampleReport.Pages.Add(new ReportPage());
      sampleReport.SmoothGraphics = true;

      tbName.Text = suggestedName;

      // paper
      PaperSize defaultPaper = printerSettings.DefaultPageSettings.PaperSize;
      cbxPaper.Items.Add(Res.Get("Forms,PageSetup,Custom"));
      foreach (PaperSize ps in printerSettings.PaperSizes)
      {
        cbxPaper.Items.Add(ps.PaperName);
        if (ps.PaperName == defaultPaper.PaperName)
          cbxPaper.SelectedIndex = cbxPaper.Items.Count - 1;
      }
      
      tbLeftMargin.Text = ConvertMMtoUnits(0);
      tbTopMargin.Text = ConvertMMtoUnits(0);
      
      // label
      tbLabelWidth.Text = ConvertMMtoUnits(defaultPaper.Width / 100f * 25.4f / 2);
      tbLabelHeight.Text = ConvertMMtoUnits(defaultPaper.Height / 100f * 25.4f / 2);
      udRows.Value = 2;
      udColumns.Value = 2;
      tbRowGap.Text = ConvertMMtoUnits(0);
      tbColumnGap.Text = ConvertMMtoUnits(0);
    }

    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,CustomLabel");
      Text = res.Get("");

      lblName.Text = res.Get("Name");
      gbPaper.Text = res.Get("Paper");
      cbLandscape.Text = res.Get("Landscape");
      lblPaperWidth.Text = res.Get("PaperWidth");
      lblPaperHeight.Text = res.Get("PaperHeight");
      lblLeftMargin.Text = res.Get("LeftMargin");
      lblTopMargin.Text = res.Get("TopMargin");
      gbLabel.Text = res.Get("Label");
      lblLabelWidth.Text = res.Get("LabelWidth");
      lblLabelHeight.Text = res.Get("LabelHeight");
      lblRows.Text = Res.Get("Forms,LabelWizard,Rows");
      lblColumns.Text = Res.Get("Forms,LabelWizard,Columns");
      lblRowGap.Text = res.Get("RowGap");
      lblColumnGap.Text = res.Get("ColumnGap");
      gbSample.Text = Res.Get("Misc,Sample");
      lblWarning.Text = res.Get("Warning");
    }
    
    public CustomLabelForm()
    {
      InitializeComponent();
      Localize();
            Scale();
    }
  }
}

