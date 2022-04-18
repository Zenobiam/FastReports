using FastReport.Design;
using FastReport.SVG;
using System.Collections.Generic;
using System.Windows.Forms;

#pragma warning disable

namespace FastReport
{
    class SelectedSVGObjects
    {
        private List<SVGObject> FList;
        private Designer FDesigner;

        public SVGObject First
        {
            get { return FList.Count > 0 ? FList[0] : null; }
        }

        public int Count
        {
            get { return FList.Count; }
        }

        public bool Enabled
        {
            get
            {
                return Count > 1 || (Count == 1 && CanModify(First));
            }
        }

        private List<SVGObject> ModifyList
        {
            get { return FList.FindAll(CanModify); }
        }

        private bool CanModify(PictureObjectBase c)
        {
            return !c.HasRestriction(Restrictions.DontModify);
        }

        public void Update()
        {
            FList.Clear();
            if (FDesigner.SelectedObjects != null)
            {
                foreach (Base c in FDesigner.SelectedObjects)
                {
                    if (c is SVGObject)
                        FList.Add(c as SVGObject);
                }
            }
        }

        public void SetSizeMode(PictureBoxSizeMode value)
        {
            foreach (SVGObject c in ModifyList)
            {
                c.SizeMode = value;
            }
        }

        public SelectedSVGObjects(Designer designer)
        {
            FDesigner = designer;
            FList = new List<SVGObject>();
        }
    }
}


#pragma warning restore