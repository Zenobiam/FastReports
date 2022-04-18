using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Design;

namespace FastReport
{
  internal class SelectedPictureObjects
  {
    private List<PictureObject> list;
    private Designer designer;

    public PictureObject First
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

    private List<PictureObject> ModifyList
    {
      get { return list.FindAll(CanModify); }
    }

    private bool CanModify(PictureObject c)
    {
      return !c.HasRestriction(Restrictions.DontModify);
    }

    public void Update()
    {
      list.Clear();
      if (designer.SelectedObjects != null)
      {
        foreach (Base c in designer.SelectedObjects)
        {
          if (c is PictureObject)
            list.Add(c as PictureObject);
        }
      }
    }

    public void SetSizeMode(PictureBoxSizeMode value)
    {
      foreach (PictureObject c in ModifyList)
      {
        c.SizeMode = value;
      }
    }

    public SelectedPictureObjects(Designer designer)
    {
      this.designer = designer;
      list = new List<PictureObject>();
    }
  }
}
