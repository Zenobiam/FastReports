using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FastReport.Design;
using FastReport.Format;

namespace FastReport
{
  internal class SelectedTextBaseObjects
  {
    private List<TextObjectBase> list;
    private Designer designer;

    public TextObjectBase First
    {
      get { return list.Count > 0 ? list[0] : null; }
    }

    public int Count
    {
      get { return list.Count; }
    }

    public bool Enabled
    {
      get
      {
        return Count > 1 || (Count == 1 && CanModify(First));
      }
    }

    private List<TextObjectBase> ModifyList
    {
      get { return list.FindAll(CanModify); }
    }

    private bool CanModify(TextObjectBase c)
    {
      return !c.HasRestriction(Restrictions.DontModify);
    }

    public void SetAllowExpressions(bool value)
    {
      foreach (TextObjectBase text in ModifyList)
      {
        text.AllowExpressions = value;
      }
    }

    public void SetFormat(FormatCollection value)
    {
      foreach (TextObjectBase text in ModifyList)
      {
        text.Formats.Assign(value);
      }
    }

    public void SetHideZeros(bool value)
    {
        foreach (TextObjectBase text in ModifyList)
        {
            text.HideZeros = value;
        }
    }

    public void Update()
    {
      list.Clear();
      if (designer.SelectedObjects != null)
      {
        foreach (Base c in designer.SelectedObjects)
        {
          if (c is TextObjectBase)
            list.Add(c as TextObjectBase);
        }
      }
    }

    public SelectedTextBaseObjects(Designer designer)
    {
      this.designer = designer;
      list = new List<TextObjectBase>();
    }
  }
}
