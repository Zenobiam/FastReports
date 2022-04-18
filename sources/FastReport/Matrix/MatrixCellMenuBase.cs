using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Design;
using FastReport.Utils;
using FastReport.Table;
using FastReport.Forms;

namespace FastReport.Matrix
{
  internal class MatrixCellMenuBase : TableMenuBase
  {
    private MatrixObject matrix;
    private MatrixElement element;
    private MatrixDescriptor descriptor;

    public ContextMenuItem miEdit;
    public ContextMenuItem miFormat;
    public ContextMenuItem miHyperlink;
    public ContextMenuItem miDelete;

    public MatrixObject Matrix
    {
      get { return matrix; }
    }
    
    public MatrixElement Element
    {
      get { return element; }
    }

    public MatrixDescriptor Descriptor
    {
      get { return descriptor; }
    }

    public TableCell Cell
    {
      get { return Designer.SelectedObjects[0] as TableCell; }
    }

    private void miEdit_Click(object sender, EventArgs e)
    {
      Matrix.HandleCellDoubleClick(Cell);
    }

    private void miFormat_Click(object sender, EventArgs e)
    {
      using (FormatEditorForm form = new FormatEditorForm())
      {
        form.TextObject = Cell;
        if (form.ShowDialog() == DialogResult.OK)
        {
          SelectedTextBaseObjects components = new SelectedTextBaseObjects(Designer);
          components.Update();
          components.SetFormat(form.Formats);
          Change();
        }
      }
    }

    private void miHyperlink_Click(object sender, EventArgs e)
    {
      using (HyperlinkEditorForm form = new HyperlinkEditorForm())
      {
        form.ReportComponent = Cell;
        if (Element == MatrixElement.Cell)
          form.IsMatrixHyperlink = true;
           
        if (form.ShowDialog() == DialogResult.OK)
        {
          SelectedReportComponents components = new SelectedReportComponents(Designer);
          components.Update();
          components.SetHyperlink(form.Hyperlink, form.ModifyAppearance, false);
          Change();
        }
      }
    }

    private void miDelete_Click(object sender, EventArgs e)
    {
      if ((Element == MatrixElement.Column || Element == MatrixElement.Row) &&
        (Descriptor as MatrixHeaderDescriptor).TemplateTotalCell == Cell)
      {  
        (Descriptor as MatrixHeaderDescriptor).Totals = false;
      }
      else
      {  
        switch (Element)
        {
          case MatrixElement.Column:
            Matrix.Data.Columns.Remove(Descriptor as MatrixHeaderDescriptor);
            break;
            
          case MatrixElement.Row:
            Matrix.Data.Rows.Remove(Descriptor as MatrixHeaderDescriptor);
            break;
            
          case MatrixElement.Cell:
            Matrix.Data.Cells.Remove(Descriptor as MatrixCellDescriptor);
            break;
        }
      }
      
      Change();
    }
    
    protected override void Change()
    {
      Matrix.BuildTemplate();
      Designer.SetModified(Matrix, "Change");
    }

    public MatrixCellMenuBase(MatrixObject matrix, MatrixElement element, MatrixDescriptor descriptor) :
      base(matrix.Report.Designer)
    {
            this.matrix = matrix;
            this.element = element;
            this.descriptor = descriptor;

      miEdit = CreateMenuItem(null, Res.Get("ComponentMenu,Component,Edit"), new EventHandler(miEdit_Click));
      miFormat = CreateMenuItem(Res.GetImage(168), Res.Get("ComponentMenu,TextObject,Format"), new EventHandler(miFormat_Click));
      miHyperlink = CreateMenuItem(Res.GetImage(167), Res.Get("ComponentMenu,ReportComponent,Hyperlink"), new EventHandler(miHyperlink_Click));
      miDelete = CreateMenuItem(Res.GetImage(51), Res.Get("Designer,Menu,Edit,Delete"), new EventHandler(miDelete_Click));
      miDelete.BeginGroup = true;

      Items.AddRange(new ContextMenuItem[] { 
        miEdit, miFormat, miHyperlink, 
        miDelete });
      
      bool enabled = Designer.SelectedObjects.Count == 1;
      miEdit.Enabled = enabled;
      miDelete.Enabled = enabled && descriptor != null && !matrix.IsAncestor;
    }
  }
}
