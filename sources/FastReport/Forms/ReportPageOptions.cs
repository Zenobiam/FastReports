using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Design;
using FastReport.Design.PageDesigners.Page;
using System.Globalization;
using FastReport.Controls;

namespace FastReport.Forms
{
  internal class ReportPageOptions : DesignerOptionsPage
  {
    private CheckBox cbSnapToGrid;
    private CheckBox cbShowGrid;
    private TextBox tbSize;
    private Label lblSnapSize;
    private RadioButton rbCorners;
    private RadioButton rbRectangle;
    private RadioButton rbHundrethsOfInch;
    private RadioButton rbInches;
    private RadioButton rbCentimeters;
    private RadioButton rbMillimeters;
    private CheckBox cbDottedGrid;
    private RadioButton rbClassic;
    private RadioButton rbStructure;
    private CheckBox cbEditAfterInsert;
    private GroupBox gbPageUnits;
    private GroupBox gbBandView;
    private GroupBox gbMarkers;
    private GroupBox gbGrid;
    private TabPage tab2;
    private CheckBox cbEventObjectIndicator;
    private CheckBox cbEventBandIndicator;
    private CheckBox cbxEnableBacklight;
    private ReportPageDesigner pageDesigner;

    private void InitializeComponent()
    {
            this.cbDottedGrid = new ScalableCheckBox();
            this.tbSize = new System.Windows.Forms.TextBox();
            this.cbSnapToGrid = new ScalableCheckBox();
            this.cbShowGrid = new ScalableCheckBox();
            this.lblSnapSize = new System.Windows.Forms.Label();
            this.rbCorners = new ScalableRadioButton();
            this.rbRectangle = new ScalableRadioButton();
            this.rbHundrethsOfInch = new ScalableRadioButton();
            this.rbInches = new ScalableRadioButton();
            this.rbCentimeters = new ScalableRadioButton();
            this.rbMillimeters = new ScalableRadioButton();
            this.rbClassic = new ScalableRadioButton();
            this.rbStructure = new ScalableRadioButton();
            this.cbEditAfterInsert = new ScalableCheckBox();
            this.gbPageUnits = new System.Windows.Forms.GroupBox();
            this.gbGrid = new System.Windows.Forms.GroupBox();
            this.gbMarkers = new System.Windows.Forms.GroupBox();
            this.gbBandView = new System.Windows.Forms.GroupBox();
            this.tab2 = new System.Windows.Forms.TabPage();
            this.cbEventBandIndicator = new ScalableCheckBox();
            this.cbEventObjectIndicator = new ScalableCheckBox();
            this.cbxEnableBacklight = new ScalableCheckBox();
            this.tc1.SuspendLayout();
            this.tab1.SuspendLayout();
            this.gbPageUnits.SuspendLayout();
            this.gbGrid.SuspendLayout();
            this.gbMarkers.SuspendLayout();
            this.gbBandView.SuspendLayout();
            this.tab2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tc1
            // 
            this.tc1.Controls.Add(this.tab2);
            this.tc1.Margin = new System.Windows.Forms.Padding(4);
            this.tc1.Size = new System.Drawing.Size(376, 280);
            this.tc1.Controls.SetChildIndex(this.tab2, 0);
            this.tc1.Controls.SetChildIndex(this.tab1, 0);
            // 
            // tab1
            // 
            this.tab1.Controls.Add(this.gbBandView);
            this.tab1.Controls.Add(this.gbMarkers);
            this.tab1.Controls.Add(this.gbGrid);
            this.tab1.Controls.Add(this.gbPageUnits);
            this.tab1.Controls.Add(this.cbEditAfterInsert);
            this.tab1.Margin = new System.Windows.Forms.Padding(4);
            this.tab1.Padding = new System.Windows.Forms.Padding(4);
            this.tab1.Size = new System.Drawing.Size(368, 254);
            // 
            // cbDottedGrid
            // 
            this.cbDottedGrid.AutoSize = true;
            this.cbDottedGrid.Location = new System.Drawing.Point(12, 66);
            this.cbDottedGrid.Name = "cbDottedGrid";
            this.cbDottedGrid.Size = new System.Drawing.Size(80, 17);
            this.cbDottedGrid.TabIndex = 8;
            this.cbDottedGrid.Text = "Dotted grid";
            this.cbDottedGrid.UseVisualStyleBackColor = true;
            // 
            // tbSize
            // 
            this.tbSize.Location = new System.Drawing.Point(92, 88);
            this.tbSize.Name = "tbSize";
            this.tbSize.Size = new System.Drawing.Size(59, 21);
            this.tbSize.TabIndex = 7;
            // 
            // cbSnapToGrid
            // 
            this.cbSnapToGrid.AutoSize = true;
            this.cbSnapToGrid.Location = new System.Drawing.Point(12, 43);
            this.cbSnapToGrid.Name = "cbSnapToGrid";
            this.cbSnapToGrid.Size = new System.Drawing.Size(84, 17);
            this.cbSnapToGrid.TabIndex = 4;
            this.cbSnapToGrid.Text = "Snap to grid";
            this.cbSnapToGrid.UseVisualStyleBackColor = true;
            // 
            // cbShowGrid
            // 
            this.cbShowGrid.AutoSize = true;
            this.cbShowGrid.Location = new System.Drawing.Point(12, 20);
            this.cbShowGrid.Name = "cbShowGrid";
            this.cbShowGrid.Size = new System.Drawing.Size(73, 17);
            this.cbShowGrid.TabIndex = 3;
            this.cbShowGrid.Text = "Show grid";
            this.cbShowGrid.UseVisualStyleBackColor = true;
            // 
            // lblSnapSize
            // 
            this.lblSnapSize.AutoSize = true;
            this.lblSnapSize.Location = new System.Drawing.Point(12, 92);
            this.lblSnapSize.Name = "lblSnapSize";
            this.lblSnapSize.Size = new System.Drawing.Size(56, 13);
            this.lblSnapSize.TabIndex = 6;
            this.lblSnapSize.Text = "Snap size:";
            // 
            // rbCorners
            // 
            this.rbCorners.AutoSize = true;
            this.rbCorners.Location = new System.Drawing.Point(12, 43);
            this.rbCorners.Name = "rbCorners";
            this.rbCorners.Size = new System.Drawing.Size(63, 17);
            this.rbCorners.TabIndex = 1;
            this.rbCorners.TabStop = true;
            this.rbCorners.Text = "Corners";
            this.rbCorners.UseVisualStyleBackColor = true;
            // 
            // rbRectangle
            // 
            this.rbRectangle.AutoSize = true;
            this.rbRectangle.Location = new System.Drawing.Point(12, 20);
            this.rbRectangle.Name = "rbRectangle";
            this.rbRectangle.Size = new System.Drawing.Size(73, 17);
            this.rbRectangle.TabIndex = 0;
            this.rbRectangle.TabStop = true;
            this.rbRectangle.Text = "Rectangle";
            this.rbRectangle.UseVisualStyleBackColor = true;
            // 
            // rbHundrethsOfInch
            // 
            this.rbHundrethsOfInch.AutoSize = true;
            this.rbHundrethsOfInch.Location = new System.Drawing.Point(12, 89);
            this.rbHundrethsOfInch.Name = "rbHundrethsOfInch";
            this.rbHundrethsOfInch.Size = new System.Drawing.Size(110, 17);
            this.rbHundrethsOfInch.TabIndex = 3;
            this.rbHundrethsOfInch.TabStop = true;
            this.rbHundrethsOfInch.Text = "Hundreths of inch";
            this.rbHundrethsOfInch.UseVisualStyleBackColor = true;
            this.rbHundrethsOfInch.CheckedChanged += new System.EventHandler(this.rbMillimeters_CheckedChanged);
            // 
            // rbInches
            // 
            this.rbInches.AutoSize = true;
            this.rbInches.Location = new System.Drawing.Point(12, 66);
            this.rbInches.Name = "rbInches";
            this.rbInches.Size = new System.Drawing.Size(57, 17);
            this.rbInches.TabIndex = 2;
            this.rbInches.TabStop = true;
            this.rbInches.Text = "Inches";
            this.rbInches.UseVisualStyleBackColor = true;
            this.rbInches.CheckedChanged += new System.EventHandler(this.rbMillimeters_CheckedChanged);
            // 
            // rbCentimeters
            // 
            this.rbCentimeters.AutoSize = true;
            this.rbCentimeters.Location = new System.Drawing.Point(12, 43);
            this.rbCentimeters.Name = "rbCentimeters";
            this.rbCentimeters.Size = new System.Drawing.Size(83, 17);
            this.rbCentimeters.TabIndex = 1;
            this.rbCentimeters.TabStop = true;
            this.rbCentimeters.Text = "Centimeters";
            this.rbCentimeters.UseVisualStyleBackColor = true;
            this.rbCentimeters.CheckedChanged += new System.EventHandler(this.rbMillimeters_CheckedChanged);
            // 
            // rbMillimeters
            // 
            this.rbMillimeters.AutoSize = true;
            this.rbMillimeters.Location = new System.Drawing.Point(12, 20);
            this.rbMillimeters.Name = "rbMillimeters";
            this.rbMillimeters.Size = new System.Drawing.Size(74, 17);
            this.rbMillimeters.TabIndex = 0;
            this.rbMillimeters.TabStop = true;
            this.rbMillimeters.Text = "Millimeters";
            this.rbMillimeters.UseVisualStyleBackColor = true;
            this.rbMillimeters.CheckedChanged += new System.EventHandler(this.rbMillimeters_CheckedChanged);
            // 
            // rbClassic
            // 
            this.rbClassic.AutoSize = true;
            this.rbClassic.Location = new System.Drawing.Point(12, 44);
            this.rbClassic.Name = "rbClassic";
            this.rbClassic.Size = new System.Drawing.Size(57, 17);
            this.rbClassic.TabIndex = 0;
            this.rbClassic.TabStop = true;
            this.rbClassic.Text = "Classic";
            this.rbClassic.UseVisualStyleBackColor = true;
            // 
            // rbStructure
            // 
            this.rbStructure.AutoSize = true;
            this.rbStructure.Location = new System.Drawing.Point(12, 20);
            this.rbStructure.Name = "rbStructure";
            this.rbStructure.Size = new System.Drawing.Size(70, 17);
            this.rbStructure.TabIndex = 0;
            this.rbStructure.TabStop = true;
            this.rbStructure.Text = "Structure";
            this.rbStructure.UseVisualStyleBackColor = true;
            // 
            // cbEditAfterInsert
            // 
            this.cbEditAfterInsert.AutoSize = true;
            this.cbEditAfterInsert.Location = new System.Drawing.Point(16, 220);
            this.cbEditAfterInsert.Name = "cbEditAfterInsert";
            this.cbEditAfterInsert.Size = new System.Drawing.Size(101, 17);
            this.cbEditAfterInsert.TabIndex = 4;
            this.cbEditAfterInsert.Text = "Edit after insert";
            this.cbEditAfterInsert.UseVisualStyleBackColor = true;
            // 
            // gbPageUnits
            // 
            this.gbPageUnits.Controls.Add(this.rbHundrethsOfInch);
            this.gbPageUnits.Controls.Add(this.rbMillimeters);
            this.gbPageUnits.Controls.Add(this.rbCentimeters);
            this.gbPageUnits.Controls.Add(this.rbInches);
            this.gbPageUnits.Location = new System.Drawing.Point(16, 12);
            this.gbPageUnits.Name = "gbPageUnits";
            this.gbPageUnits.Size = new System.Drawing.Size(164, 120);
            this.gbPageUnits.TabIndex = 9;
            this.gbPageUnits.TabStop = false;
            this.gbPageUnits.Text = "Page units";
            // 
            // gbGrid
            // 
            this.gbGrid.Controls.Add(this.cbDottedGrid);
            this.gbGrid.Controls.Add(this.cbShowGrid);
            this.gbGrid.Controls.Add(this.tbSize);
            this.gbGrid.Controls.Add(this.lblSnapSize);
            this.gbGrid.Controls.Add(this.cbSnapToGrid);
            this.gbGrid.Location = new System.Drawing.Point(188, 12);
            this.gbGrid.Name = "gbGrid";
            this.gbGrid.Size = new System.Drawing.Size(164, 120);
            this.gbGrid.TabIndex = 10;
            this.gbGrid.TabStop = false;
            this.gbGrid.Text = "Grid";
            // 
            // gbMarkers
            // 
            this.gbMarkers.Controls.Add(this.rbCorners);
            this.gbMarkers.Controls.Add(this.rbRectangle);
            this.gbMarkers.Location = new System.Drawing.Point(16, 136);
            this.gbMarkers.Name = "gbMarkers";
            this.gbMarkers.Size = new System.Drawing.Size(164, 72);
            this.gbMarkers.TabIndex = 11;
            this.gbMarkers.TabStop = false;
            this.gbMarkers.Text = "Markers";
            // 
            // gbBandView
            // 
            this.gbBandView.Controls.Add(this.rbClassic);
            this.gbBandView.Controls.Add(this.rbStructure);
            this.gbBandView.Location = new System.Drawing.Point(188, 136);
            this.gbBandView.Name = "gbBandView";
            this.gbBandView.Size = new System.Drawing.Size(164, 72);
            this.gbBandView.TabIndex = 12;
            this.gbBandView.TabStop = false;
            this.gbBandView.Text = "Band view";
            // 
            // tab2
            // 
            this.tab2.Controls.Add(this.cbxEnableBacklight);
            this.tab2.Controls.Add(this.cbEventBandIndicator);
            this.tab2.Controls.Add(this.cbEventObjectIndicator);
            this.tab2.Location = new System.Drawing.Point(4, 22);
            this.tab2.Margin = new System.Windows.Forms.Padding(2);
            this.tab2.Name = "tab2";
            this.tab2.Padding = new System.Windows.Forms.Padding(2);
            this.tab2.Size = new System.Drawing.Size(368, 254);
            this.tab2.TabIndex = 1;
            this.tab2.Text = "tabPage2";
            this.tab2.UseVisualStyleBackColor = true;
            // 
            // cbEventBandIndicator
            // 
            this.cbEventBandIndicator.AutoSize = true;
            this.cbEventBandIndicator.Location = new System.Drawing.Point(16, 17);
            this.cbEventBandIndicator.Name = "cbEventBandIndicator";
            this.cbEventBandIndicator.Size = new System.Drawing.Size(202, 17);
            this.cbEventBandIndicator.TabIndex = 6;
            this.cbEventBandIndicator.Text = "Show indicator on bands with events";
            this.cbEventBandIndicator.UseVisualStyleBackColor = true;
            // 
            // cbEventObjectIndicator
            // 
            this.cbEventObjectIndicator.AutoSize = true;
            this.cbEventObjectIndicator.Location = new System.Drawing.Point(16, 40);
            this.cbEventObjectIndicator.Name = "cbEventObjectIndicator";
            this.cbEventObjectIndicator.Size = new System.Drawing.Size(208, 17);
            this.cbEventObjectIndicator.TabIndex = 5;
            this.cbEventObjectIndicator.Text = "Show indicator on objects with events";
            this.cbEventObjectIndicator.UseVisualStyleBackColor = true;
            // 
            // cbxEnableBacklight
            // 
            this.cbxEnableBacklight.AutoSize = true;
            this.cbxEnableBacklight.Location = new System.Drawing.Point(16, 63);
            this.cbxEnableBacklight.Name = "cbxEnableBacklight";
            this.cbxEnableBacklight.Size = new System.Drawing.Size(130, 17);
            this.cbxEnableBacklight.TabIndex = 7;
            this.cbxEnableBacklight.Text = "Enable band backlight";
            this.cbxEnableBacklight.UseVisualStyleBackColor = true;
            // 
            // ReportPageOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(420, 336);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ReportPageOptions";
            this.tc1.ResumeLayout(false);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.gbPageUnits.ResumeLayout(false);
            this.gbPageUnits.PerformLayout();
            this.gbGrid.ResumeLayout(false);
            this.gbGrid.PerformLayout();
            this.gbMarkers.ResumeLayout(false);
            this.gbMarkers.PerformLayout();
            this.gbBandView.ResumeLayout(false);
            this.gbBandView.PerformLayout();
            this.tab2.ResumeLayout(false);
            this.tab2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }


    private void rbMillimeters_CheckedChanged(object sender, EventArgs e)
    {
      string units = "";
      float size = 0;
      if (rbMillimeters.Checked)
      {
        units = "Misc,ShortUnitsMm";
        size = ReportWorkspace.Grid.SnapSizeMillimeters;
      }
      else if (rbCentimeters.Checked)
      {
        units = "Misc,ShortUnitsCm";
        size = ReportWorkspace.Grid.SnapSizeCentimeters;
      }
      else if (rbInches.Checked)
      {
        units = "Misc,ShortUnitsIn";
        size = ReportWorkspace.Grid.SnapSizeInches;
      }
      else if (rbHundrethsOfInch.Checked)
      {
        units = "Misc,ShortUnitsHi";
        size = ReportWorkspace.Grid.SnapSizeHundrethsOfInch;
      }
      tbSize.Text = size.ToString() + " " + Res.Get(units);
    }

    private void Localize()
    {
      MyRes res = new MyRes("Forms,ReportPageOptions");
      tab1.Text = res.Get("");
      tab2.Text = res.Get("ObjectsAppearance");
      gbPageUnits.Text = res.Get("Units");
      rbMillimeters.Text = res.Get("Millimeters");
      rbCentimeters.Text = res.Get("Centimeters");
      rbInches.Text = res.Get("Inches");
      rbHundrethsOfInch.Text = res.Get("HundrethsOfInch");
      gbGrid.Text = res.Get("Grid");
      cbShowGrid.Text = res.Get("ShowGrid");
      cbSnapToGrid.Text = res.Get("SnapToGrid");
      cbDottedGrid.Text = res.Get("DottedGrid");
      lblSnapSize.Text = res.Get("Size");
      gbMarkers.Text = res.Get("Markers");
      rbRectangle.Text = res.Get("Rectangle");
      rbCorners.Text = res.Get("Corners");
      gbBandView.Text = res.Get("BandView");
      rbStructure.Text = res.Get("Structure");
      rbClassic.Text = res.Get("Classic");
      cbEditAfterInsert.Text = res.Get("EditAfterInsert");
      cbEventBandIndicator.Text = res.Get("EventBandIndicator");
      cbEventObjectIndicator.Text = res.Get("EventObjectIndicator");
      cbxEnableBacklight.Text = res.Get("EnableBacklight");
    }

    public override void Init()
    {
      switch (ReportWorkspace.Grid.GridUnits)
      {
        case PageUnits.Millimeters:
          rbMillimeters.Checked = true;
          break;
        case PageUnits.Centimeters:
          rbCentimeters.Checked = true;
          break;
        case PageUnits.Inches:
          rbInches.Checked = true;
          break;
        case PageUnits.HundrethsOfInch:
          rbHundrethsOfInch.Checked = true;
          break;  
      }
      rbMillimeters_CheckedChanged(null, null);
      cbShowGrid.Checked = ReportWorkspace.ShowGrid;
      cbSnapToGrid.Checked = ReportWorkspace.SnapToGrid;
      cbDottedGrid.Checked = ReportWorkspace.Grid.Dotted;
      rbRectangle.Checked = ReportWorkspace.MarkerStyle == MarkerStyle.Rectangle;
      rbCorners.Checked = ReportWorkspace.MarkerStyle == MarkerStyle.Corners;
      rbStructure.Checked = !ReportWorkspace.ClassicView;
      rbClassic.Checked = ReportWorkspace.ClassicView;
      cbEditAfterInsert.Checked = ReportWorkspace.EditAfterInsert;
      cbEventObjectIndicator.Checked = ReportWorkspace.EventObjectIndicator;
      cbEventBandIndicator.Checked = ReportWorkspace.EventBandIndicator;
//      cbxEnableBacklight.Checked = ReportWorkspace.EnableBacklight;
    }

    public override void Done(DialogResult result)
    {
      if (result == DialogResult.OK)
      {
        if (rbMillimeters.Checked)
          ReportWorkspace.Grid.GridUnits = PageUnits.Millimeters;
        else if (rbCentimeters.Checked)
          ReportWorkspace.Grid.GridUnits = PageUnits.Centimeters;
        else if (rbInches.Checked)
          ReportWorkspace.Grid.GridUnits = PageUnits.Inches;
        else if (rbHundrethsOfInch.Checked)
          ReportWorkspace.Grid.GridUnits = PageUnits.HundrethsOfInch;
        ReportWorkspace.ShowGrid = cbShowGrid.Checked;
        ReportWorkspace.SnapToGrid = cbSnapToGrid.Checked;
        ReportWorkspace.Grid.Dotted = cbDottedGrid.Checked;
        ReportWorkspace.Grid.SnapSize = Converter.StringToFloat(tbSize.Text, true);
        ReportWorkspace.MarkerStyle = rbRectangle.Checked ? MarkerStyle.Rectangle : MarkerStyle.Corners;
        ReportWorkspace.ClassicView = rbClassic.Checked;
        ReportWorkspace.EditAfterInsert = cbEditAfterInsert.Checked;
        ReportWorkspace.EventObjectIndicator = cbEventObjectIndicator.Checked;
        ReportWorkspace.EventBandIndicator = cbEventBandIndicator.Checked;
//        ReportWorkspace.EnableBacklight = cbxEnableBacklight.Checked;
      }
    }

        public ReportPageOptions(ReportPageDesigner pd) : base()
        {
            pageDesigner = pd;
            InitializeComponent();
            Localize();
            Scale();
        }

   
  }
}
