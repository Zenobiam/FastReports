using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Data;
using FastReport.Table;

namespace FastReport.Matrix
{
  internal class MatrixCellSmartTag : DataColumnSmartTag
  {
    private MatrixObject matrix;
    private MatrixDescriptor descriptor;
    
    protected override DataSourceBase FindRootDataSource()
    {
      return matrix.DataSource;
    }

    protected override void ItemClicked()
    {
      descriptor.Expression = DataColumn == "" ? "" : "[" + DataColumn + "]";
      matrix.BuildTemplate();
      base.ItemClicked();
    }
    
    public MatrixCellSmartTag(MatrixObject matrix, MatrixDescriptor descriptor) : base(descriptor.TemplateCell)
    {
            this.matrix = matrix;
            this.descriptor = descriptor;
      
      string expression = descriptor.Expression;
      if (expression.StartsWith("[") && expression.EndsWith("]"))
        expression = expression.Substring(1, expression.Length - 2);
      DataColumn = expression;
    }
  }
}
