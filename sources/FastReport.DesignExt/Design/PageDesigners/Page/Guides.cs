using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;

namespace FastReport.Design.PageDesigners.Page
{
  internal class Guides
  {
      private ReportWorkspace workspace;
      private List<LinkInfo> guideLinks;

    public ReportPage Page
    {
      get { return workspace.Page; }
    }

    public Designer Designer
    {
        get { return workspace.Designer; }
    }

    private void CheckVGuide(ref float kx, float coord)
    {
      if (Page.Guides == null)
        return;

      float closestGuide = float.PositiveInfinity;
      foreach (float f in Page.Guides)
      {
        if (Math.Abs(coord + kx - f) < Math.Abs(coord + kx - closestGuide))
          closestGuide = f;
      }

      if (Math.Abs(coord + kx - closestGuide) < ReportWorkspace.Grid.SnapSize * 2)
        kx = closestGuide - coord;
    }

    private void CheckHGuide(ref float ky, float coord, ComponentBase c)
    {
      BandBase band = c.Parent as BandBase;
      if (band == null || band.Guides == null)
        return;
      
      float closestGuide = float.PositiveInfinity;
      foreach (float f in band.Guides)
      {
        if (Math.Abs(coord + ky - f) < Math.Abs(coord + ky - closestGuide))
          closestGuide = f;
      }

      if (Math.Abs(coord + ky - closestGuide) < ReportWorkspace.Grid.SnapSize * 2)
        ky = closestGuide - coord;
    }

    private void DrawGuides(Graphics g, object obj)
    {
      FloatCollection guides = null;
      bool vertical = false;
      float offs = 0;

      if (obj is ReportPage)
      {
        guides = (obj as ReportPage).Guides;
        vertical = true;
      }
      else if (obj is BandBase)
      {
        guides = (obj as BandBase).Guides;
        offs = (obj as BandBase).Top;
      }
      if (guides != null)
      {
        Pen pen = new Pen(Color.CornflowerBlue);
        pen.DashStyle = DashStyle.Dot;
        foreach (float f in guides)
        {
          float scale = workspace.GetScale();
          if (vertical)
            g.DrawLine(pen, f * scale, 0, f * scale, workspace.Height);
          else
          {
            if (f > 0 && f < (obj as BandBase).Height)
              g.DrawLine(pen, 0, (f + offs) * scale, workspace.Width, (f + offs) * scale);
          }  
        }
        pen.Dispose();
      }
    }

    public void Draw(Graphics g)
    {
      // draw guides
      DrawGuides(g, Page);
      foreach (Base obj in Designer.Objects)
      {
        if (obj is BandBase)
          DrawGuides(g, obj);
      }
    }

    public void CheckGuides(ref float kx, ref float ky)
    {
      foreach (Base obj in Designer.SelectedObjects)
      {
        if (obj is ComponentBase && !(obj is BandBase))
        {
          ComponentBase c = obj as ComponentBase;
          CheckVGuide(ref kx, c.Left);
          CheckVGuide(ref kx, c.Right);
          CheckHGuide(ref ky, c.Top, c);
          CheckHGuide(ref ky, c.Bottom, c);
        }
      }
    }

    public void BeforeMoveHGuide(BandBase band, int guide)
    {
      guideLinks.Clear();
      foreach (Base obj in Designer.Objects)
      {
        if (obj is ReportComponentBase && !(obj is BandBase) && obj.Parent == band)
        {
          ReportComponentBase c = obj as ReportComponentBase;
          LinkPoint link = LinkPoint.None;
          if (Math.Abs(c.Top - band.Guides[guide]) < 0.01)
            link = LinkPoint.Top;
          else if (Math.Abs(c.Bottom - band.Guides[guide]) < 0.01)
            link = LinkPoint.Bottom;
          if (link != LinkPoint.None)
          {
            LinkInfo info = new LinkInfo();
            info.obj = c;
            info.link = link;
            // check if object is also linked to another guide
            int i = band.Guides.IndexOf(c.Top);
            if (i != -1 && i != guide)
              info.doubleLinked = true;
            i = band.Guides.IndexOf(c.Bottom);
            if (i != -1 && i != guide)
              info.doubleLinked = true;
            guideLinks.Add(info);
          }
        }
      }
    }
    
    public void MoveHGuide(BandBase band, int guide, float ky)
    {
      foreach (LinkInfo link in guideLinks)
      {
        if (!link.doubleLinked)
          link.obj.Top += ky;
        else
        {
          if (link.link == LinkPoint.Top)
          {
            link.obj.Top += ky;
            link.obj.Height -= ky;
          }
          else if (link.link == LinkPoint.Bottom)
            link.obj.Height += ky;
        }
      }
    }

    public void BeforeMoveVGuide(int guide)
    {
      guideLinks.Clear();
      foreach (Base obj in Designer.Objects)
      {
        if (obj is ReportComponentBase && !(obj is BandBase))
        {
          ReportComponentBase c = obj as ReportComponentBase;
          LinkPoint link = LinkPoint.None;
          if (Math.Abs(c.Left - Page.Guides[guide]) < 0.01)
            link = LinkPoint.Left;
          else if (Math.Abs(c.Right - Page.Guides[guide]) < 0.01)
            link = LinkPoint.Right;
          if (link != LinkPoint.None)
          {
            LinkInfo info = new LinkInfo();
            info.obj = c;
            info.link = link;
            // check if object is also linked to another guide
            int i = Page.Guides.IndexOf(c.Left);
            if (i != -1 && i != guide)
              info.doubleLinked = true;
            i = Page.Guides.IndexOf(c.Right);
            if (i != -1 && i != guide)
              info.doubleLinked = true;
            guideLinks.Add(info);
          }
        }
      }
    }

    public void MoveVGuide(int guide, float kx)
    {
      foreach (LinkInfo link in guideLinks)
      {
        if (!link.doubleLinked)
          link.obj.Left += kx;
        else
        {
          if (link.link == LinkPoint.Left)
          {
            link.obj.Left += kx;
            link.obj.Width -= kx;
          }
          else if (link.link == LinkPoint.Right)
            link.obj.Width += kx;
        }
      }
    }

    public Guides(ReportWorkspace w)
    {
        workspace = w;
        guideLinks = new List<LinkInfo>();
    }

    
    private enum LinkPoint { None, Left, Top, Right, Bottom }

    private class LinkInfo
    {
      public ReportComponentBase obj;
      public LinkPoint link;
      public bool doubleLinked;
    }

  }
}
