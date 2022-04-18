using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;

namespace FastReport.Design.PageDesigners
{
    internal class VirtualGuides
    {
        private DesignWorkspaceBase workspace;
        private SortedList<float, ComponentBase> vGuides;
        private SortedList<float, ComponentBase> hGuides;
        private List<RectangleF> guides;

        public DesignWorkspaceBase Workspace
        {
            get { return workspace; }
        }

        public Designer Designer
        {
            get { return workspace.Designer; }
        }

        private void CheckVGuide(float x, ComponentBase c)
        {
            x = Converter.DecreasePrecision(x, 2);
            int i = vGuides.IndexOfKey(x);
            if (i != -1)
            {
                float scale = Workspace.GetScale();
                guides.Add(new RectangleF(x * scale, c.AbsTop * scale,
                  x * scale, vGuides.Values[i].AbsTop * scale));
            }
        }

        private void CheckHGuide(float y, ComponentBase c)
        {
            y = Converter.DecreasePrecision(y, 2);
            int i = hGuides.IndexOfKey(y);
            if (i != -1)
            {
                float scale = Workspace.GetScale();
                guides.Add(new RectangleF(c.AbsLeft * scale, y * scale,
                  hGuides.Values[i].AbsLeft * scale, y * scale));
            }
        }

        private void AddVGuide(float x, ComponentBase c)
        {
            x = Converter.DecreasePrecision(x, 2);
            if (!vGuides.ContainsKey(x))
                vGuides.Add(x, c);
        }

        private void AddHGuide(float y, ComponentBase c)
        {
            y = Converter.DecreasePrecision(y, 2);
            if (!hGuides.ContainsKey(y))
                hGuides.Add(y, c);
        }

        public void Draw(Graphics g)
        {
            using (Pen pen = new Pen(Color.CornflowerBlue))
            {
                pen.DashStyle = DashStyle.Dot;
                foreach (RectangleF rect in guides)
                {
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Width, rect.Height);
                }
            }
        }

        public void Create()
        {
            vGuides.Clear();
            hGuides.Clear();
            foreach (Base obj in Designer.Objects)
            {
                if (obj is ComponentBase && !(obj is BandBase) && !Designer.SelectedObjects.Contains(obj))
                {
                    ComponentBase c = obj as ComponentBase;
                    AddVGuide(c.AbsLeft, c);
                    AddVGuide(c.AbsRight, c);
                    AddHGuide(c.AbsTop, c);
                    AddHGuide(c.AbsBottom, c);
                }
            }
        }

        public void Check()
        {
            guides.Clear();
            foreach (Base obj in Designer.SelectedObjects)
            {
                if (obj is ComponentBase && !(obj is BandBase))
                {
                    ComponentBase c = obj as ComponentBase;
                    CheckVGuide(c.AbsLeft, c);
                    CheckVGuide(c.AbsRight, c);
                    CheckHGuide(c.AbsTop, c);
                    CheckHGuide(c.AbsBottom, c);
                }
            }
        }

        public void Clear()
        {
            guides.Clear();
        }

        public VirtualGuides(DesignWorkspaceBase w)
        {
            workspace = w;
            vGuides = new SortedList<float, ComponentBase>();
            hGuides = new SortedList<float, ComponentBase>();
            guides = new List<RectangleF>();
        }
    }
}
