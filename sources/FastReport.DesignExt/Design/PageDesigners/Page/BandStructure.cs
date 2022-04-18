using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Forms;
using System.Drawing.Drawing2D;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design.PageDesigners.Page
{
  internal class BandStructure : Control
  {
    private BandStructureNode root;
    private ReportPageDesigner pageDesigner;
    private int offset;

    public Button btnConfigure;
    public bool allowPaint = true;

    public ReportPage Page
    {
      get { return pageDesigner.Page as ReportPage; }
    }

    public Designer Designer
    {
      get { return pageDesigner.Designer; }
    }

    public int Offset
    {
      get { return offset; }
      set { offset = value; }
    }

    private void btnConfigure_Click(object sender, EventArgs e)
    {
      using (ConfigureBandsForm form = new ConfigureBandsForm(Designer))
      {
        form.Page = Page;
        form.ShowDialog();
      }
    }

    private void AddBand(BandBase band, BandStructureNode root)
    {
      if (band == null)
        return;
      BandStructureNode node = root.Add();
      node.AddBand(band);
    }

    private void EnumDataBand(DataBand band, BandStructureNode root)
    {
      if (band == null)
        return;
      BandStructureNode node = root.Add();

      node.AddBand(band.Header);

      node.AddBand(band);
      foreach (BandBase b in band.Bands)
      {
        EnumBand(b, node);
      }

      node.AddBand(band.Footer);
    }

    private void EnumGroupHeaderBand(GroupHeaderBand band, BandStructureNode root)
    {
      if (band == null)
        return;
      BandStructureNode node = root.Add();

      node.AddBand(band.Header);
      node.AddBand(band);
      EnumGroupHeaderBand(band.NestedGroup, node);
      EnumDataBand(band.Data, node);
      node.AddBand(band.GroupFooter);
      node.AddBand(band.Footer);
    }

    private void EnumBand(BandBase band, BandStructureNode root)
    {
      if (band is DataBand)
        EnumDataBand(band as DataBand, root);
      else if (band is GroupHeaderBand)
        EnumGroupHeaderBand(band as GroupHeaderBand, root);
    }

    private void EnumPageBands()
    {
      if (Page == null)
        return;

      if (Page.TitleBeforeHeader)
      {
        AddBand(Page.ReportTitle, root);
        AddBand(Page.PageHeader, root);
      }
      else
      {
        AddBand(Page.PageHeader, root);
        AddBand(Page.ReportTitle, root);
      }
      AddBand(Page.ColumnHeader, root);
      foreach (BandBase b in Page.Bands)
      {
        EnumBand(b, root);
      }
      AddBand(Page.ColumnFooter, root);
      AddBand(Page.ReportSummary, root);
      AddBand(Page.PageFooter, root);
      AddBand(Page.Overlay, root);
    }

    private void EnumNodes(List<BandStructureNode> list, BandStructureNode root)
    {
      if (root != this.root)
        list.Add(root);
      foreach (BandStructureNode node in root.childNodes)
      {
        EnumNodes(list, node);
      }
    }

    private void DrawItems(Graphics g)
    {
      List<BandStructureNode> list = new List<BandStructureNode>();
      EnumNodes(list, root);

      foreach (BandStructureNode node in list)
      {
        int offs = DpiHelper.ConvertUnits(24) + Offset - DpiHelper.ConvertUnits(2);

        RectangleF fillRect = new RectangleF(node.left, node.top + offs, Width - node.left - 1, node.height);
        BandBase bandFill = null;

        foreach (BandBase band in node.bands)
        {
          bandFill = band;
          if (band is GroupHeaderBand || band is DataBand)
            break;
        }

        // fill node area
        bandFill.DrawBandHeader(g, fillRect, false);
        g.DrawRectangle(SystemPens.ControlDark, node.left, node.top + offs, Width - node.left - 1, node.height);

        // draw band title
        float scale = ReportWorkspace.Scale;
        using (StringFormat sf = new StringFormat())
        {
          foreach (BandBase band in node.bands)
          {
            g.DrawLine(SystemPens.ControlDark, node.left + 8, band.Top * scale + offs, Width, band.Top * scale + offs);

            RectangleF textRect = new RectangleF(node.left + 4, band.Top * scale + offs + 4,
              Width - (node.left + 4) - 4, band.Height * scale - 4);
            ObjectInfo info = RegisteredObjects.FindObject(band);
            string text = Res.Get(info.Text);
            if (band.GetInfoText() != "")
              text += ": " + band.GetInfoText();

            float textHeight = DrawUtils.MeasureString(g, text, DpiHelper.ConvertUnits(DrawUtils.Default96Font), textRect, sf).Height;
            TextFormatFlags flags = textHeight > textRect.Height ?
              TextFormatFlags.WordBreak : TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
            TextRenderer.DrawText(g, text, DpiHelper.ConvertUnits(DrawUtils.Default96Font),
              new Rectangle((int)textRect.Left, (int)textRect.Top, (int)textRect.Width, (int)textRect.Height),
              SystemColors.WindowText, flags);

            if (band.IsAncestor)
              g.DrawImage(Res.GetImage(99), (int)(node.left + Width - 10), (int)(band.Top * scale + offs + 3));
          }
        }
      }
    }

    private bool PointInRect(Point point, Rectangle rect)
    {
      return point.X >= rect.Left && point.X <= rect.Right && point.Y >= rect.Top && point.Y <= rect.Bottom;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      if (pageDesigner.Locked)
        return;
      root.Clear();
      EnumPageBands();
      DrawItems(e.Graphics);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      if (pageDesigner.Locked)
        return;

      List<BandStructureNode> list = new List<BandStructureNode>();
      EnumNodes(list, root);

      float scale = ReportWorkspace.Scale;
      int offs = 24 + Offset;
      foreach (BandStructureNode node in list)
      {
        foreach (BandBase band in node.bands)
        {
          if (PointInRect(e.Location, new Rectangle((int)node.left, (int)(band.Top * scale) + offs,
            Width - (int)node.left, (int)(band.Height * scale))))
          {
            Designer.CancelPaste();
            Designer.SelectedObjects.Clear();
            Designer.SelectedObjects.Add(band);
            Designer.SelectionChanged(null);
            break;
          }
        }
      }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      if (pageDesigner.Locked)
        return;

      if (e.Button == MouseButtons.Right && Designer.SelectedObjects.Count > 0
        && Designer.SelectedObjects[0] is BandBase)
      {
        ContextMenuBase menu = Designer.SelectedObjects[0].GetContextMenu();
        if (menu != null)
        {
          menu.Show(this, e.Location);
        }
      }
    }

    protected override void OnDoubleClick(EventArgs e)
    {
      base.OnDoubleClick(e);
      if (Designer.SelectedObjects.Count == 1 && Designer.SelectedObjects[0] is BandBase)
      {
        BandBase band = Designer.SelectedObjects[0] as BandBase;
        if (!band.HasRestriction(Restrictions.DontEdit))
          band.HandleDoubleClick();
      }
    }

#if !MONO
    private const int WM_PAINT = 0x000F;
    protected override void WndProc(ref Message m)
    {
        if ((m.Msg != WM_PAINT) || (allowPaint && m.Msg == WM_PAINT))
        {
            base.WndProc(ref m);
        }
    }
#endif

        public void ReinitDpiSize()
        {
            btnConfigure.Height = DpiHelper.ConvertUnits(24);
            btnConfigure.Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
        }

    public void Localize()
    {
      btnConfigure.Text = Res.Get("Designer,Workspace,Report,ConfigureBands");
    }

    public BandStructure(ReportPageDesigner pd)
    {
      pageDesigner = pd;
      root = new BandStructureNode();

      btnConfigure = new Button();
      btnConfigure.Height = DpiHelper.ConvertUnits(24);
      btnConfigure.Dock = DockStyle.Top;
      btnConfigure.FlatStyle = FlatStyle.Flat;
      btnConfigure.FlatAppearance.BorderColor = SystemColors.ButtonFace;
      btnConfigure.FlatAppearance.BorderSize = 0;
      btnConfigure.Font = DpiHelper.ConvertUnits(DrawUtils.Default96Font);
      btnConfigure.TextAlign = ContentAlignment.MiddleLeft;
      btnConfigure.Cursor = Cursors.Hand;
      btnConfigure.Click += new EventHandler(btnConfigure_Click);
      Controls.Add(btnConfigure);
      Localize();

#if !MONO
      SetStyle(ControlStyles.UserPaint, true);
#endif	  
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      SetStyle(ControlStyles.DoubleBuffer, true);
      SetStyle(ControlStyles.ResizeRedraw, true);
    }


    private class BandStructureNode
    {
      public float left;
      public float top;
      public float height;
      public List<BandBase> bands;
      public List<BandStructureNode> childNodes;
      public BandStructureNode parent;

      public void AdjustHeight(float height)
      {
                this.height += height;
        if (parent != null)
          parent.AdjustHeight(height);
      }

      public BandStructureNode Add()
      {
        BandStructureNode result = new BandStructureNode();
        childNodes.Add(result);
        result.parent = this;
        if (left == -1)
          result.left = 7;
        else
          result.left = left + 8;
        return result;
      }

      public void AddBand(BandBase band)
      {
        if (band != null)
        {
          if (band.Child != null && band.Child.FillUnusedSpace)
          {
            AddBand(band.Child);
            AddBandInternal(band);
          }
          else
          {
            AddBandInternal(band);
            AddBand(band.Child);
          }
        }
      }

      private void AddBandInternal(BandBase band)
      {
        if (band != null)
        {
          bands.Add(band);
          float scale = ReportWorkspace.Scale;
          if (bands.Count == 1)
            top = band.Top * scale;
          float height = band.Height * scale + 4;
          AdjustHeight(height);
        }
      }

      public void Clear()
      {
        bands.Clear();
        while (childNodes.Count > 0)
        {
          childNodes[0].Clear();
          childNodes.RemoveAt(0);
        }
      }

      public BandStructureNode()
      {
        bands = new List<BandBase>();
        childNodes = new List<BandStructureNode>();
        left = -9;
      }
    }
  }
}
