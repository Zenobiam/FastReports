using System.Drawing;
using System.ComponentModel;
using System.Drawing.Printing;
using FastReport.Utils;

namespace FastReport
{
  partial class ReportPage
  {
    #region Fields

    private int firstPageSource;
    private int otherPagesSource;
    private int lastPageSource;
    private Duplex duplex;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the paper source for the first printed page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property represents the paper source (printer tray) that will be used when printing
    /// the first page. To set the source for other pages, use
    /// <see cref="LastPageSource"/> and <see cref="OtherPagesSource"/> properties.
    /// </para>
    /// <para>
    /// Note: This property uses the <b>raw</b> number of the paper source.
    /// </para>
    /// </remarks>
    [DefaultValue(7)]
    [Category("Print")]
    public int FirstPageSource
    {
      get { return firstPageSource; }
      set { firstPageSource = value; }
    }

    /// <summary>
    /// Gets or sets the paper source for all printed pages except the first one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property represents the paper source (printer tray) that will be used when printing
    /// all pages except the first one and the last one. To set source for first and last pages, use 
    /// <see cref="FirstPageSource"/> and <see cref="LastPageSource"/> properties.
    /// </para>
    /// <para>
    /// Note: This property uses the <b>raw</b> number of the paper source.
    /// </para>
    /// </remarks>
    [DefaultValue(7)]
    [Category("Print")]
    public int OtherPagesSource
    {
      get { return otherPagesSource; }
      set { otherPagesSource = value; }
    }

    /// <summary>
    /// Gets or sets the paper source for the last printed page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property represents the paper source (printer tray) that will be used when printing
    /// the last page. To set the source for other pages, use
    /// <see cref="FirstPageSource"/> and <see cref="OtherPagesSource"/> properties.
    /// </para>
    /// <para>
    /// Note: This property uses the <b>raw</b> number of the paper source.
    /// </para>
    /// </remarks>
    [DefaultValue(7)]
    [Category("Print")]
    public int LastPageSource
    {
      get { return lastPageSource; }
      set { lastPageSource = value; }
    }

    /// <summary>
    /// Gets or sets the printer duplex mode that will be used when printing this page.
    /// </summary>
    [DefaultValue(Duplex.Default)]
    [Category("Print")]
    public Duplex Duplex
    {
      get { return duplex; }
      set { duplex = value; }
    }

    #endregion

    #region Public Methods

    internal void DrawSearchHighlight(FRPaintEventArgs e, int objIndex, CharacterRange range)
    {
      IGraphics g = e.Graphics;
      float leftMargin = LeftMargin * Units.Millimeters * e.ScaleX;
      float topMargin = TopMargin * Units.Millimeters * e.ScaleY;

      ObjectCollection allObjects = AllObjects;
      if (objIndex < 0 && objIndex >= allObjects.Count)
        return;
      ISearchable obj = allObjects[objIndex] as ISearchable;
      if (obj != null)
      {
        g.TranslateTransform(leftMargin, topMargin);
        try
        {
          obj.DrawSearchHighlight(e, range);
        }
        finally
        {  
          g.TranslateTransform(-leftMargin, -topMargin);
        }
      }  
    }

    internal void Print(FRPaintEventArgs e)
    {
      try
      {
        SetPrinting(true);
        SetDesigning(false);
        Draw(e);
      }
      finally
      {
        SetPrinting(false);
      }
    }
    
    internal ReportComponentBase HitTest(PointF mouse)
    {
      mouse.X -= LeftMargin * Units.Millimeters;
      mouse.Y -= TopMargin * Units.Millimeters;

      ObjectCollection allObjects = AllObjects;
      for (int i = allObjects.Count - 1; i >= 0; i--)
      {
        ReportComponentBase c = allObjects[i] as ReportComponentBase;
        if (c != null)
        {
          if (c.PointInObject(mouse))
            return c;
        }
      }
      return null;
    }

        #endregion

        private void AssignPreview(ReportPage src)
        {
            FirstPageSource = src.FirstPageSource;
            OtherPagesSource = src.OtherPagesSource;
            LastPageSource = src.LastPageSource;
            Duplex = src.Duplex;
        }

        private void WritePreview(FRWriter writer, ReportPage c)
        {
            if (FirstPageSource != c.FirstPageSource)
                writer.WriteInt("FirstPageSource", FirstPageSource);
            if (OtherPagesSource != c.OtherPagesSource)
                writer.WriteInt("OtherPagesSource", OtherPagesSource);
            if (LastPageSource != c.LastPageSource)
                writer.WriteInt("LastPageSource", LastPageSource);
            if (Duplex != c.Duplex)
                writer.WriteValue("Duplex", Duplex);
        }

        private void InitPreview()
        {
            firstPageSource = 7;
            otherPagesSource = 7;
            lastPageSource = 7;
            duplex = Duplex.Default;
        }
    }
}
