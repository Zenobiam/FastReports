using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport
{
  internal class PictureObjectSmartTag : DataColumnSmartTag
  {
    protected override void ItemClicked()
    {
      (Obj as PictureObjectBase).DataColumn = DataColumn;
      base.ItemClicked();
    }

    public PictureObjectSmartTag(ComponentBase obj) : base(obj)
    {
      DataColumn = (Obj as PictureObjectBase).DataColumn;
    }
  }
}
