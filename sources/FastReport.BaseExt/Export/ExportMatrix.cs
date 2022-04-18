using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FastReport.Format;
using FastReport.Barcode;
using FastReport.Table;
using FastReport.Utils;
#if DOTNET_4
using FastReport.SVG;
#endif

namespace FastReport.Export
{

  internal class ExportMatrix : IDisposable
  {
    #region Private fields

    private Font defaultOneSizeFont;

    private List<ExportIEMObject> iemObjectList;
    private List<ExportIEMStyle> iemStyleList;

    private BinaryTree xPos;
    private BinaryTree yPos;

    private List<ExportIEMPage> pages;
    private int width;
    private int height;
    private float maxWidth;
    private float maxHeight;
    private float minLeft;
    private float minTop;
    private int[] matrix;
    private float deltaY;
    private bool showProgress;
    private float inaccuracy;
    private bool rotatedImage;
    private bool plainRich;
    private bool cropFillArea;
    private bool fillArea;
    private bool optFrames;
    private float left;
    private bool printable;
    private bool images;
    private int imageResolution;
    private bool wrap;
    private bool brushAsBitmap;
    private List<string> fontList;
    private bool dotMatrix;
    private Report report;
    private MyRes res;
    private ExportIEMPage currentPage;
    private float zoom;
    private bool fullTrust;
    private bool dataOnly;
    private ImageFormat pictureFormat;
    private int jpegQuality;
    private bool fillAsBitmap;
    private bool htmlMode;
    private bool watermarks;
    private IDictionary picsCache;
    private string rowHeightIs;
    private bool seamless;
    private bool firstPage;
    private bool repeatDataband;
    private bool keepRichText;
    #endregion

    #region Properties

    public bool Seamless
    {
      get { return seamless; }
      set { seamless = value; }
    }

    public List<ExportIEMStyle> Styles
    {
      get { return iemStyleList; }
      set { iemStyleList = value; }
    }

    public List<ExportIEMPage> Pages
    {
      get { return pages; }
    }

    public bool Watermarks
    {
      get { return watermarks; }
      set { watermarks = value; }
    }

    public bool HTMLMode
    {
      get { return htmlMode; }
      set { htmlMode = value; }
    }

    public bool FillAsBitmap
    {
      get { return fillAsBitmap; }
      set { fillAsBitmap = value; }
    }

    public int JpegQuality
    {
      get { return jpegQuality; }
      set { jpegQuality = value; }
    }

    public ImageFormat ImageFormat
    {
      get { return pictureFormat; }
      set { pictureFormat = value; }
    }

    public bool FullTrust
    {
      get { return fullTrust; }
      set { fullTrust = value; }
    }

    public float Zoom
    {
      get { return zoom; }
      set { zoom = value; }
    }

    public int Width
    {
      get { return width; }
    }
    public int Height
    {
      get { return height; }
    }
    public float MaxWidth
    {
      get { return maxWidth; }
    }
    public float MaxHeight
    {
      get { return maxHeight; }
    }
    public float MinLeft
    {
      get { return minLeft; }
    }
    public float MinTop
    {
      get { return minTop; }
    }
    public bool ShowProgress
    {
      get { return showProgress; }
      set { showProgress = value; }
    }
    public float MaxCellHeight
    {
      get { return yPos.MaxDistance; }
      set { yPos.MaxDistance = value; }
    }
    public float MaxCellWidth
    {
      get { return xPos.MaxDistance; }
      set
      {
        xPos.MaxDistance = value;
      }
    }
    public int PagesCount
    {
      get { return pages.Count; }
    }
    public int StylesCount
    {
      get { return iemStyleList.Count; }
    }
    public int ObjectsCount
    {
      get { return iemObjectList.Count; }
    }
    public float Inaccuracy
    {
      get { return xPos.Inaccuracy; }
      set
      {
        xPos.Inaccuracy = value;
        yPos.Inaccuracy = value;
      }
    }
    public bool RotatedAsImage
    {
      get { return rotatedImage; }
      set { rotatedImage = value; }
    }
    public bool PlainRich
    {
      get { return plainRich; }
      set { plainRich = value; }
    }
    public bool AreaFill
    {
      get { return fillArea; }
      set { fillArea = value; }
    }
    public bool CropAreaFill
    {
      get { return cropFillArea; }
      set { cropFillArea = value; }
    }
    public bool FramesOptimization
    {
      get { return optFrames; }
      set { optFrames = value; }
    }
    public float Left
    {
      get { return left; }
    }
    public bool Printable
    {
      get { return printable; }
      set { printable = value; }
    }
    public bool Images
    {
      get { return images; }
      set { images = value; }
    }
    public int ImageResolution
    {
      get { return imageResolution; }
      set { imageResolution = value; }
    }
    public bool WrapText
    {
      get { return wrap; }
      set { wrap = value; }
    }
    public bool BrushAsBitmap
    {
      get { return brushAsBitmap; }
      set { brushAsBitmap = value; }
    }

    public bool DotMatrix
    {
      get { return dotMatrix; }
      set { dotMatrix = value; }
    }

    public bool DataOnly
    {
      get { return dataOnly; }
      set { dataOnly = value; }
    }

    public Report Report
    {
      get { return report; }
      set { report = value; }
    }
    public string RowHeightIs
    {
      get { return this.rowHeightIs; }
      set { this.rowHeightIs = value; }
    }
    public bool KeepRichText
    {
      get { return this.keepRichText; }
      set { this.keepRichText = value; }
    }

    #endregion

    #region Private Methods

    private int AddStyleInternal(ExportIEMStyle Style)
    {
      for (int i = iemStyleList.Count - 1; i >= 0; i--)
        if (Style.Equals(iemStyleList[i]))
          return i;
      iemStyleList.Add(Style);
      return iemStyleList.Count - 1;
    }

    private int AddStyle(ReportComponentBase Obj)
    {
      ExportIEMStyle Style = new ExportIEMStyle();
      if (Obj is TextObject)
      {
        TextObject MObj = Obj as TextObject;
        Style.Font = new Font(MObj.Font.FontFamily, MObj.Font.Size * zoom, MObj.Font.Style,
            MObj.Font.Unit, MObj.Font.GdiCharSet, MObj.Font.GdiVerticalFont);
        Style.TextFill = MObj.TextFill;
        Style.RTL = MObj.RightToLeft;
        Style.WordWrap = MObj.WordWrap;
        Style.Fill = MObj.Fill;
        Style.Format = MObj.Format;
        Style.VAlign = MObj.VertAlign;
        if (Style.RTL)
          Style.HAlign = MObj.HorzAlign == HorzAlign.Left ?
              HorzAlign.Right : (MObj.HorzAlign == HorzAlign.Right ? HorzAlign.Left : MObj.HorzAlign);
        else
          Style.HAlign = MObj.HorzAlign;
        Style.Padding = new Padding(
            (int)Math.Round(MObj.Padding.Left * zoom),
            (int)Math.Round(MObj.Padding.Top * zoom),
            (int)Math.Round(MObj.Padding.Right * zoom),
            (int)Math.Round(MObj.Padding.Bottom * zoom));
        Style.FirstTabOffset = MObj.FirstTabOffset;

        Style.Border = MObj.Border.Clone();

        Style.Border.ZoomBorder(zoom);

        Style.Angle = MObj.Angle;
        Style.LineHeight = MObj.LineHeight;
        Style.ParagraphOffset = MObj.ParagraphOffset;
        Style.FontWidthRatio = MObj.FontWidthRatio;

        Style.ForceJustify = MObj.ForceJustify;
      }
      else if (Obj is HtmlObject)
      {
        HtmlObject MObj = Obj as HtmlObject;
        Style.RTL = MObj.RightToLeft;
        Style.Fill = MObj.Fill;
        Style.Format = MObj.Format;
        Style.WordWrap = true;
        Style.TextFill = new SolidFill(Color.Black);
        Style.Padding = new Padding(
            (int)Math.Round(MObj.Padding.Left * zoom),
            (int)Math.Round(MObj.Padding.Top * zoom),
            (int)Math.Round(MObj.Padding.Right * zoom),
            (int)Math.Round(MObj.Padding.Bottom * zoom));

        Style.Border = MObj.Border.Clone();

        Style.Border.ZoomBorder(zoom);
      }
      else if (Obj is BandBase)
      {
        BandBase Band = Obj as BandBase;
        Style.Fill = Band.Fill;
        Style.Border = Band.Border;
      }
      else if (IsLine(Obj))
      {
        Style.Border = Obj.Border;
        if (Obj.Width == 0)
          Style.Border.Lines = BorderLines.Left;
        else if (Obj.Height == 0)
          Style.Border.Lines = BorderLines.Top;
        Style.Font = defaultOneSizeFont;
      }
        else if (Obj is ShapeObject)
        {
            if ((Obj as ShapeObject).Shape == ShapeKind.Rectangle &&
                Obj.Fill.IsTransparent)
            {
                Style.Border = Obj.Border;
                Style.Border.Lines = BorderLines.All;
                Style.Fill = Obj.Fill;
            }

            //do nothing
        }
        else if (Obj is PolygonObject ||
                Obj is BarcodeObject)
        {
            //do nothing
        }
#if DOTNET_4
        else if (Obj is SVGObject)
        {
            if (Obj.Fill is SolidFill && !Obj.Fill.IsTransparent)
                Style.Fill = Obj.Fill;

            Style.Border = Obj.Border;
            Style.HAlign = HorzAlign.Center;
            Style.VAlign = VertAlign.Center;
        }
#endif
      else
      {
        Style.Border = Obj.Border;
        Style.Fill = Obj.Fill;
        Style.Font = defaultOneSizeFont;
        Style.Border.LeftLine.Width = Style.Border.LeftLine.Width > 1 ?
            Style.Border.LeftLine.Width * zoom : Style.Border.LeftLine.Width;
        Style.Border.RightLine.Width = Style.Border.RightLine.Width > 1 ?
            Style.Border.RightLine.Width * zoom : Style.Border.RightLine.Width;
        Style.Border.TopLine.Width = Style.Border.TopLine.Width > 1 ?
            Style.Border.TopLine.Width * zoom : Style.Border.TopLine.Width;
        Style.Border.BottomLine.Width = Style.Border.BottomLine.Width > 1 ?
            Style.Border.BottomLine.Width * zoom : Style.Border.BottomLine.Width;
      }
      return AddStyleInternal(Style);
    }

    private int AddInternalObject(ExportIEMObject Obj, int x, int y, int dx, int dy)
    {
      Obj.x = x;
      Obj.y = y;
      Obj.dx = dx;
      Obj.dy = dy;
      iemObjectList.Add(Obj);
      return iemObjectList.Count - 1;
    }

    private bool IsMemo(ReportComponentBase Obj)
    {
      if (Obj is TextObject)
      {
        TextObject aObj = Obj as TextObject;
        if ((aObj.Angle == 0 || !rotatedImage) && !aObj.TextOutline.Enabled)
          return true;
      }
      return false;
    }

    private bool IsLine(ReportComponentBase Obj)
    {
      return (Obj is LineObject) && ((Obj.Width == 0) || (Obj.Height == 0));
    }

    private bool IsRect(ReportComponentBase Obj)
    {
      return (Obj is ShapeObject) && ((Obj as ShapeObject).Shape == ShapeKind.Rectangle);
    }

    private void FillArea(int x, int y, int dx, int dy, int Value)
    {
      int k;
      int ddx = x + dx;
      int ddy = y + dy;
      for (int i = y; i < ddy; i++)
      {
        k = width * i;
        for (int j = x; j < ddx; j++)
          matrix[k + j] = Value;
      }
    }

    private void ReplaceRect(int ObjIndex, int x, int y, int dx, int dy, int Value)
    {
      int k;
      int ddx = x + dx;
      int ddy = y + dy;
      for (int i = y; i < ddy; i++)
      {
        k = width * i;
        for (int j = x; j < ddx; j++)
          if (matrix[k + j] == ObjIndex)
            matrix[k + j] = Value;
      }
    }

    private void FindRect(int x, int y, out int dx, out int dy)
    {
      int Obj = matrix[width * y + x];
      int px = x;
      int py = y;
      int ky;
      dx = 0;
      while (matrix[width * py + px] == Obj)
      {
        ky = width * py;
        while (matrix[ky + px] == Obj)
          px++;
        if (dx == 0)
          dx = px - x;
        else if ((px - x) < dx)
          break;
        py++;
        px = x;
      }
      dy = py - y;
    }

    private void Cut(int ObjIndex, int x, int y, int dx, int dy)
    {
      ExportIEMObject Obj = iemObjectList[ObjIndex];
      ExportIEMObject NewObject = new ExportIEMObject();
      NewObject.StyleIndex = Obj.StyleIndex;
      NewObject.Style = Obj.Style;
      NewObject.Left = xPos.Nodes[x].value;
      NewObject.Top = yPos.Nodes[y].value;
      NewObject.Width = xPos.Nodes[x + dx].value - xPos.Nodes[x].value;
      NewObject.Height = yPos.Nodes[y + dy].value - yPos.Nodes[y].value;
      NewObject.Parent = Obj;
      NewObject.IsText = Obj.IsText;
      NewObject.IsRichText = Obj.IsRichText;
      NewObject.Metafile = Obj.Metafile;
      NewObject.PictureStream = Obj.PictureStream;
      NewObject.Base = Obj.Base;
      NewObject.Hash = Obj.Hash;
      NewObject.InlineImageCache = Obj.InlineImageCache;
      float fdy = Obj.Top + Obj.Height - NewObject.Top;
      float fdx = Obj.Left + Obj.Width - NewObject.Left;
      NewObject.ParagraphFormat = Obj.ParagraphFormat;
      NewObject.TabWidth = Obj.TabWidth;
      if ((fdy > Obj.Height / 3) && (fdx > Obj.Width / 3))
      {
        NewObject.IsText = Obj.IsText;
        NewObject.Text = Obj.Text;
        NewObject.Value = Obj.Value;
        NewObject.IsNumeric = Obj.IsNumeric;
        NewObject.IsDateTime = Obj.IsDateTime;
        NewObject.TextRenderType = Obj.TextRenderType;
        NewObject.OriginalText = Obj.OriginalText;
        Obj.Text = String.Empty;
        Obj.OriginalText = null;
        Obj.Value = null;
        Obj.IsText = true;
      }
      int NewIndex = AddInternalObject(NewObject, x, y, dx, dy);
      ReplaceRect(ObjIndex, x, y, dx, dy, NewIndex);
      CloneFrames(ObjIndex, NewIndex);
      iemObjectList[NewIndex].Exist = true;
    }

    private void CloneFrames(int Obj1, int Obj2)
    {
      ExportIEMObject FOld, FNew;
      BorderLines FrameTyp;
      FOld = iemObjectList[Obj1];
      FNew = iemObjectList[Obj2];
      if ((FOld.Style != null) && (FNew.Style != null))
      {
        FrameTyp = BorderLines.None;
        if (((BorderLines.Top & FOld.Style.Border.Lines) != 0) &&
            (Math.Abs(FOld.Top - FNew.Top) <= inaccuracy))
          FrameTyp |= BorderLines.Top;
        if (((BorderLines.Left & FOld.Style.Border.Lines) != 0) &&
            (Math.Abs(FOld.Left - FNew.Left) <= inaccuracy))
          FrameTyp |= BorderLines.Left;
        if (((BorderLines.Bottom & FOld.Style.Border.Lines) != 0) &&
            (Math.Abs((FOld.Top + FOld.Height) - (FNew.Top + FNew.Height)) <= inaccuracy))
          FrameTyp |= BorderLines.Bottom;
        if (((BorderLines.Right & FOld.Style.Border.Lines) != 0) &&
            (Math.Abs((FOld.Left + FOld.Width) - (FNew.Left + FNew.Width)) <= inaccuracy))
          FrameTyp |= BorderLines.Right;

        if (FrameTyp != FNew.Style.Border.Lines)
        {
          ExportIEMStyle NewStyle = new ExportIEMStyle();
          NewStyle.Assign(FOld.Style);
          NewStyle.Border.Lines = FrameTyp;
          FNew.StyleIndex = AddStyleInternal(NewStyle);
          FNew.Style = iemStyleList[FNew.StyleIndex];
        }
      }
    }

    private void Render()
    {
      int i, k, Old;
      ExportIEMObject Obj;
      ExportIEMStyle Style;
      FillBase OldColor;

      xPos.Close();
      yPos.Close();

      width = xPos.Count;
      height = yPos.Count;

      k = width * height;

      matrix = new int[k];

      for (i = 0; i < k; i++)
        matrix[i] = -1;

      for (i = 0; i < iemObjectList.Count; i++)
      {
        Obj = iemObjectList[i];

        int xPos0 = this.xPos.IndexOf(Obj.Left);
        if (xPos0 != -1)
        {
          iemObjectList[i].x = xPos0;
          Obj.Left = this.xPos.Nodes[xPos0].value;
          k = this.xPos.IndexOf(Obj.Left + Obj.Width);
          iemObjectList[i].dx = k - xPos0;
        }

        int yPos0 = this.yPos.IndexOf(Obj.Top);
        if (yPos0 != -1)
        {
          iemObjectList[i].y = yPos0;
          Obj.Top = this.yPos.Nodes[yPos0].value;
          k = this.yPos.IndexOf(Obj.Top + Obj.Height);
          iemObjectList[i].dy = k - yPos0;
        }

        if ((Obj.Style != null) && Obj.Style.FillColor.A == 0)
        {
          Old = matrix[width * Obj.y + Obj.x];
          if (Old != -1 && iemObjectList[Old].Style != null)
          {
            OldColor = iemObjectList[Old].Style.Fill;
            if ((ExportUtils.GetColorFromFill(OldColor) != Obj.Style.FillColor) &&
                (ExportUtils.GetColorFromFill(OldColor) != Obj.Style.TextColor))
            {
              Style = new ExportIEMStyle();
              Style.Assign(Obj.Style);
              Style.Fill = OldColor;
              Obj.StyleIndex = AddStyleInternal(Style);
              Obj.Style = iemStyleList[Obj.StyleIndex];
            }
          }
          else if (Obj.Style.FillColor != Color.Transparent)
          {
            Style = new ExportIEMStyle();
            Style.Assign(Obj.Style);
            Style.Fill = new SolidFill(Color.Transparent);
            Obj.StyleIndex = AddStyleInternal(Style);
            Obj.Style = iemStyleList[Obj.StyleIndex];
          }
        }

        int bottomIndex = matrix[width * Obj.y + Obj.x];
        if (bottomIndex != -1)
        {
          ExportIEMObject bottomObject = iemObjectList[bottomIndex];
          if (Obj.Style != null && bottomObject.Style != null)
          {
            CheckFrameFromBottmObject(Obj, bottomObject,
                Obj.y + Obj.dy, bottomObject.y + bottomObject.dy, BorderLines.Bottom);
            CheckFrameFromBottmObject(Obj, bottomObject,
                Obj.x + Obj.dx, bottomObject.x + bottomObject.dx, BorderLines.Right);
            CheckFrameFromBottmObject(Obj, bottomObject,
                Obj.x, bottomObject.x, BorderLines.Left);
            CheckFrameFromBottmObject(Obj, bottomObject,
                Obj.y, bottomObject.y, BorderLines.Top);
          }
        }

        if (!(Obj.IsBand && Obj.Style != null &&
                ((Obj.Style.FillColor == Color.White) || (Obj.Style.FillColor.A == 0)) &&
                (Obj.Style.Border.Lines == BorderLines.None)))
          FillArea(Obj.x, Obj.y, Obj.dx, Obj.dy, i);

        if (i % 1000 == 0)
            Config.DoEvent();
      }
    }

    private void CheckFrameFromBottmObject(ExportIEMObject Obj, ExportIEMObject bottomObject, int pos1, int pos2, BorderLines line)
    {
      if (pos1 == pos2 &&
          ((line & Obj.Style.Border.Lines) == 0) &&
          ((line & bottomObject.Style.Border.Lines) != 0))
      {
        ExportIEMStyle Style = new ExportIEMStyle();
        Style.Assign(Obj.Style);
        Style.Border.Lines = Obj.Style.Border.Lines | line;
        if (line == BorderLines.Bottom)
          Style.Border.BottomLine = bottomObject.Style.Border.BottomLine;
        else if (line == BorderLines.Top)
          Style.Border.TopLine = bottomObject.Style.Border.TopLine;
        else if (line == BorderLines.Left)
          Style.Border.LeftLine = bottomObject.Style.Border.LeftLine;
        else if (line == BorderLines.Right)
          Style.Border.RightLine = bottomObject.Style.Border.RightLine;
        Obj.StyleIndex = AddStyleInternal(Style);
        Obj.Style = iemStyleList[Obj.StyleIndex];
      }
    }

    private void Analyze()
    {
      for (int i = 0; i < height - 1; i++)
      {
        int ki = width * i;
        for (int j = 0; j < width - 1; j++)
        {
          int k = matrix[ki + j];
          if (k != -1)
          {
            ExportIEMObject obj = iemObjectList[k];
            if (!obj.Exist)
            {
              int dx, dy;
              FindRect(j, i, out dx, out dy);
              if (j + dx >= xPos.Count)
                dx = xPos.Count - j - 1;
              if (i + dy >= yPos.Count)
                dy = yPos.Count - i - 1;
              if ((obj.x != j) || (obj.y != i) || (obj.dx != dx) || (obj.dy != dy))
              {
                if (!obj.Exist)
                  Cut(k, j, i, dx, dy);
                else
                  obj.Exist = true;
              }
              else
              {
                obj.Exist = true;
              }
            }
          }
        }
        if (i % 1000 == 0)
            Config.DoEvent();
      }
    }

    private void OptimizeFrames()
    {
      int x, y;
      ExportIEMObject Obj, PrevObj;
      BorderLines CurrentLines;
      ExportIEMStyle Style;
      for (y = 0; y < Height; y++)
      {
        for (x = 0; x < Width; x++)
        {
          Obj = GetObject(x, y);
          if (Obj == null)
            continue;
          if (Obj.Style != null && Obj.Style.Border.Lines != BorderLines.None)
          {
            CurrentLines = Obj.Style.Border.Lines;
            if (((BorderLines.Top & CurrentLines) > 0) && (y > 0))
            {
              PrevObj = GetObject(x, y - 1);
              if ((PrevObj != null) && (PrevObj != Obj) && (PrevObj.Style != null)
                      && ((BorderLines.Bottom & PrevObj.Style.Border.Lines) > 0)
                  && (PrevObj.Style.Border.BottomLine.Width == Obj.Style.Border.TopLine.Width)
                  && (PrevObj.Style.Border.BottomLine.Color == Obj.Style.Border.TopLine.Color)
                  )
              {
                CurrentLines = CurrentLines & ~BorderLines.Top;
                Style = new ExportIEMStyle();
                Style.Assign(Obj.Style);
                Style.Border.Lines = CurrentLines;
                Obj.StyleIndex = AddStyleInternal(Style);
                Obj.Style = iemStyleList[Obj.StyleIndex];
              }
            }
            if (((BorderLines.Left & CurrentLines) > 0) && (x > 0))
            {
              PrevObj = GetObject(x - 1, y);
              if ((PrevObj != null) && (PrevObj != Obj) && (PrevObj.Style != null)
                      && ((BorderLines.Right & PrevObj.Style.Border.Lines) > 0)
                  && (PrevObj.Style.Border.RightLine.Width == Obj.Style.Border.LeftLine.Width)
                  && (PrevObj.Style.Border.RightLine.Color == Obj.Style.Border.LeftLine.Color)
                  )
              {
                CurrentLines = CurrentLines & ~BorderLines.Left;
                Style = new ExportIEMStyle();
                Style.Assign(Obj.Style);
                Style.Border.Lines = CurrentLines;
                Obj.StyleIndex = AddStyleInternal(Style);
                Obj.Style = iemStyleList[Obj.StyleIndex];
              }
            }
          }
        }
        if (y % 1000 == 0)
            Config.DoEvent();
      }
    }

    private void AddSetObjectPos(ReportComponentBase Obj, ref ExportIEMObject FObj)
    {
      float Left = Obj.AbsLeft;
      float Top = Obj.AbsTop;

      float Width = Obj.Width;
      float Height = Obj.Height;

      if (Left >= 0)
        FObj.Left = Width > 0 ? Left * zoom : (Left + Width) * zoom;
      else
        FObj.Left = 0;

      if (Top >= 0)
        FObj.Top = Height > 0 ? deltaY + Top * zoom : deltaY + (Top + Height) * zoom;
      else
        FObj.Top = deltaY;

      if (IsLine(Obj))
      {
        FObj.Width = Math.Abs(Width) > 0 ? Math.Abs(Width) * zoom : Obj.Border.Width * zoom;
        FObj.Height = Math.Abs(Height) > 0 ? Math.Abs(Height) * zoom : Obj.Border.Width * zoom;
      }
      else
      {
        FObj.Width = Math.Abs(Width) * zoom;
        FObj.Height = Math.Abs(Height) * zoom;
      }

      if ((FObj.Left + FObj.Width) > (currentPage.Width - currentPage.LeftMargin - currentPage.RightMargin) * Units.Millimeters)
        FObj.Width = (currentPage.Width - currentPage.LeftMargin - currentPage.RightMargin) * Units.Millimeters - FObj.Left;

      if ((FObj.Top + FObj.Height) > maxHeight)
        maxHeight = FObj.Top + FObj.Height;
      if ((FObj.Left + FObj.Width) > maxWidth)
        maxWidth = FObj.Left + FObj.Width;
      if (FObj.Left < minLeft)
        minLeft = FObj.Left;
      if (FObj.Top < minTop)
        minTop = FObj.Top;
      if ((FObj.Left < left) || (left == 0))
        left = FObj.Left;

      float bottom = FObj.Top + FObj.Height;

      xPos.Add(FObj.Left);
      xPos.Add(FObj.Left + FObj.Width);
      yPos.Add(FObj.Top);
      yPos.Add(bottom);

      AddInternalObject(FObj, 0, 0, 1, 1);
    }

    #endregion

    #region Public Methods

    public ExportIEMStyle StyleById(int Index)
    {
      return iemStyleList[Index];
    }

    public int Cell(int x, int y)
    {
      if ((x < width) && (y < height) && (x >= 0) && (y >= 0))
        return matrix[width * y + x];
      return -1;
    }

    public ExportIEMObject ObjectById(int ObjIndex)
    {
      if (ObjIndex < iemObjectList.Count)
        return iemObjectList[ObjIndex];
      return null;
    }

    public bool IsYPosByIdNull(int PosIndex)
    {
      return yPos.Nodes[PosIndex] == null;
    }

    public float XPosById(int PosIndex)
    {
      return xPos.Nodes[PosIndex].value;
    }

    public float YPosById(int PosIndex)
    {
      return yPos.Nodes[PosIndex].value;
    }

    public ExportIEMObject GetObject(int x, int y)
    {
      int i = matrix[width * y + x];
      if (i == -1)
        return null;
      return iemObjectList[i];
    }

    public void Clear()
    {
      foreach (KeyValuePair<string, MemoryStream> cacheItem in (Dictionary<string, MemoryStream>)picsCache)
        cacheItem.Value.Dispose();

      picsCache.Clear();
      iemObjectList.Clear();
      xPos.Clear();
      yPos.Clear();
      pages.Clear();

      foreach (ExportIEMStyle style in Styles)
      {
        if (!style.Font.Equals(DrawUtils.DefaultFont))
        {
          style.Font.Dispose();
          style.Font = null;
        }
      }

      iemStyleList.Clear();
      fontList.Clear();
      deltaY = 0;
      firstPage = true;

      if (PagesCount > 100)
        GC.Collect(2);
    }

    public void AddBandObject(BandBase Obj)
    {
      if ((Obj as BandBase).Fill is TextureFill)
          return;
      if (Obj.HasFill || Obj.HasBorder)
      {
        ExportIEMObject FObj = new ExportIEMObject();
        FObj.StyleIndex = AddStyle(Obj);
        if (FObj.StyleIndex != -1)
          FObj.Style = iemStyleList[FObj.StyleIndex];
        FObj.IsText = true;
        FObj.IsBand = true;
        AddSetObjectPos(Obj, ref FObj);
      }
    }

    private void AddLineObject(ReportComponentBase Obj)
    {
      ExportIEMObject FObj = new ExportIEMObject();
      FObj.StyleIndex = AddStyle(Obj);
      if (FObj.StyleIndex != -1)
        FObj.Style = iemStyleList[FObj.StyleIndex];
      FObj.IsText = true;
      AddSetObjectPos(Obj, ref FObj);
    }

    private Size GetImageSize(ExportIEMObject FObj, ReportComponentBase Obj)
    {
      float dx = FObj.Width;
      float dy = FObj.Height;
      if (htmlMode)
      {
        if ((Obj.Border.Lines & BorderLines.Left) != 0)
          dx -= Obj.Border.LeftLine.Width * zoom;
        if ((Obj.Border.Lines & BorderLines.Right) != 0)
          dx -= Obj.Border.RightLine.Width * zoom;
        if ((Obj.Border.Lines & BorderLines.Top) != 0)
          dy -= Obj.Border.TopLine.Width * zoom;
        if ((Obj.Border.Lines & BorderLines.Bottom) != 0)
          dy -= Obj.Border.BottomLine.Width * zoom;
      }
      dx = (dx >= 1 ? dx : 1);
      dy = (dy >= 1 ? dy : 1);

      return new Size((int)Math.Round(dx), (int)Math.Round(dy));
    }

    private void DrawImage(Graphics g, ReportComponentBase Obj)
    {
      using (GraphicCache cache = new GraphicCache())
      {
        if (Obj is TextObjectBase)
          g.Clear(Color.White);

        float Left = Obj.Width >= 0 ? Obj.AbsLeft : Obj.AbsLeft + Obj.Width;
        float Top = Obj.Height >= 0 ? Obj.AbsTop : Obj.AbsTop + Obj.Height;

        if (htmlMode)
        {
          float dx = (Obj.Border.Lines & BorderLines.Left) != 0 ? Obj.Border.LeftLine.Width : 0;
          float dy = (Obj.Border.Lines & BorderLines.Top) != 0 ? Obj.Border.TopLine.Width : 0;
          g.TranslateTransform((-Left - dx) * zoom, (-Top - dy) * zoom);
        }
        else
          g.TranslateTransform(-Left * zoom, -Top * zoom);
        BorderLines oldLines = Obj.Border.Lines;
        Obj.Border.Lines = BorderLines.None;
        Obj.Draw(new FRPaintEventArgs(g, zoom, zoom, cache));
        Obj.Border.Lines = oldLines;
      }
    }

    public void AddPictureObject(ReportComponentBase Obj)
    {
      if (IsVisible(Obj))
      {
        ExportIEMObject FObj = new ExportIEMObject();
        FObj.URL = Obj.Hyperlink.Kind == HyperlinkKind.URL ? Obj.Hyperlink.Value : "";
        Padding oldPadding = new Padding();
        if (Obj is TextObject)
        {
          oldPadding = (Obj as TextObject).Padding;
          (Obj as TextObject).Padding = new Padding();
        }
        FObj.StyleIndex = AddStyle(Obj);
        if (Obj is TextObject)
          (Obj as TextObject).Padding = oldPadding;
        if (FObj.StyleIndex != -1)
          FObj.Style = iemStyleList[FObj.StyleIndex];
        FObj.IsText = false;
        AddSetObjectPos(Obj, ref FObj);
        FObj.PictureStream = new MemoryStream();
        using (Bitmap bmp = new Bitmap(1, 1))
        {
          using (Graphics g = Graphics.FromImage(bmp))
          {
            IntPtr hdc = g.GetHdc();
            try
            {
              FObj.Metafile = new Metafile(FObj.PictureStream, hdc,
                  new Rectangle(new Point(0, 0), GetImageSize(FObj, Obj)), MetafileFrameUnit.Pixel);
            }
            finally
            {
              g.ReleaseHdc(hdc);
            }
          }
        }

        if (pictureFormat != ImageFormat.Emf)
        {
          Size imageSize = GetImageSize(FObj, Obj);
          using (System.Drawing.Image image = new Bitmap(imageSize.Width, imageSize.Height))
          {
            using (Graphics g = Graphics.FromImage(image))
            {
              DrawImage(g, Obj);
            }

            if (pictureFormat == ImageFormat.Jpeg)
              ExportUtils.SaveJpeg(image, FObj.PictureStream, jpegQuality);
            else
              image.Save(FObj.PictureStream, pictureFormat);
          }
        }
        else
          using (Graphics g = Graphics.FromImage(FObj.Metafile))
          {
            DrawImage(g, Obj);
          }

        FObj.PictureStream.Position = 0;
        CheckPicsCache(FObj);
        AddShadow(Obj);
      }
    }

    public void AddPictureObject_Safe(ReportComponentBase Obj)
    {
      if (IsVisible(Obj))
      {
        ExportIEMObject FObj = new ExportIEMObject();
        FObj.URL = Obj.Hyperlink.Kind == HyperlinkKind.URL ? Obj.Hyperlink.Value : "";
        Padding oldPadding = new Padding();
        if (Obj is TextObject)
        {
          oldPadding = (Obj as TextObject).Padding;
          (Obj as TextObject).Padding = new Padding();
        }
        FObj.StyleIndex = AddStyle(Obj);
        if (Obj is TextObject)
          (Obj as TextObject).Padding = oldPadding;
        if (FObj.StyleIndex != -1)
          FObj.Style = iemStyleList[FObj.StyleIndex];
        FObj.IsText = false;
        AddSetObjectPos(Obj, ref FObj);

        FObj.PictureStream = new MemoryStream();

        float zoom = imageResolution / 96f;

        Size imageSize = GetImageSize(FObj, Obj);
        using (System.Drawing.Image image =
            new Bitmap((int)Math.Round(imageSize.Width * zoom),
            (int)Math.Round(imageSize.Height * zoom)))
        {
          using (Graphics g = Graphics.FromImage(image))
          {
            g.ScaleTransform(zoom, zoom);
            DrawImage(g, Obj);
          }

          if (pictureFormat == ImageFormat.Jpeg)
            ExportUtils.SaveJpeg(image, FObj.PictureStream, jpegQuality);
          else
            image.Save(FObj.PictureStream, pictureFormat);
        }
        FObj.PictureStream.Position = 0;
        CheckPicsCache(FObj);
        AddShadow(Obj);
      }
    }

#if DOTNET_4
        private void AddSVG(SVGObject Obj)
        {
            if (IsVisible(Obj))
            {
                ExportIEMObject FObj = new ExportIEMObject();
                FObj.StyleIndex = AddStyle(Obj);
                if (FObj.StyleIndex != -1)
                    FObj.Style = iemStyleList[FObj.StyleIndex];
                //FObj.Text = Obj.Text;
                FObj.IsNumeric = false;
                FObj.TextRenderType = TextRenderType.Default;
                FObj.URL = Obj.Hyperlink.Kind == HyperlinkKind.URL ? Obj.Hyperlink.Value : "";
                FObj.IsText = false;
                FObj.IsRichText = false;
                FObj.IsSvg = true;
                AddSetObjectPos(Obj, ref FObj);
                FObj.PictureStream = new MemoryStream();
                if (Obj.Grayscale)
                    Obj.SVGGrayscale.Write(FObj.PictureStream);
                else
                    Obj.SvgDocument.Write(FObj.PictureStream);
                CheckPicsCache(FObj);
            }
        }
#endif
    private void AddHtml(HtmlObject Obj)
    {
      if (IsVisible(Obj))
      {
        ExportIEMObject FObj = new ExportIEMObject();
        FObj.StyleIndex = AddStyle(Obj);
        if (FObj.StyleIndex != -1)
          FObj.Style = iemStyleList[FObj.StyleIndex];
        FObj.Text = Obj.Text;
        FObj.IsNumeric = false;
        FObj.TextRenderType = TextRenderType.HtmlTags;
        FObj.URL = Obj.Hyperlink.Kind == HyperlinkKind.URL ? Obj.Hyperlink.Value : "";
        FObj.IsText = false;
        FObj.IsRichText = false;
        AddSetObjectPos(Obj, ref FObj);
        AddShadow(Obj);
        if (fillAsBitmap && !(FObj.Style.Fill is SolidFill))
        {
          float dx = FObj.Width;
          float dy = FObj.Height;
          dx = (dx >= 1 ? dx : 1);
          dy = (dy >= 1 ? dy : 1);
          System.Drawing.Image image = new Bitmap((int)dx, (int)dy);

          using (Graphics g = Graphics.FromImage(image))
          {
            g.Clear(Color.Transparent);
            g.TranslateTransform(-Obj.AbsLeft * zoom, -Obj.AbsTop * zoom);
            BorderLines oldLines = Obj.Border.Lines;
            Obj.Border.Lines = BorderLines.None;
            string oldText = Obj.Text;
            Obj.Text = String.Empty;
            Obj.Draw(new FRPaintEventArgs(g, zoom, zoom, Report.GraphicCache));
            Obj.Text = oldText;
            Obj.Border.Lines = oldLines;
            FObj.PictureStream = new MemoryStream();
            image.Save(FObj.PictureStream, pictureFormat);
            CheckPicsCache(FObj);
          }
        }
      }
    }

    public void AddTextObject(TextObject Obj)
    {
      AddTextObject(Obj, false);
    }

    public void AddTextObject(TextObject Obj, bool isHeader)
    {
      if (IsVisible(Obj))
      {
        ExportIEMObject FObj = new ExportIEMObject();
        FObj.StyleIndex = AddStyle(Obj);
        if (FObj.StyleIndex != -1)
          FObj.Style = iemStyleList[FObj.StyleIndex];
        FObj.Text = Obj.Text;
        if (isHeader && Obj.OriginalComponent != null)
          FObj.OriginalText = (Obj.OriginalComponent as TextObject).Text;
        FObj.TabWidth = Obj.TabWidth;
        if (Obj.ParagraphFormat != null)
          FObj.ParagraphFormat = Obj.ParagraphFormat.MultipleScale(1);
        if (Obj.TextRenderType == TextRenderType.HtmlParagraph)
          FObj.InlineImageCache = Obj.InlineImageCache;
        DateTime date;
        FObj.IsDateTime = ExportUtils.ParseTextToDateTime(FObj.Text, FObj.Style.Format, out date);
        decimal percent;
        FObj.IsPercent = ExportUtils.ParseTextToPercent(FObj.Text, FObj.Style.Format, out percent);
        decimal numeric = 0;
        FObj.IsNumeric = ExportUtils.ParseTextToDecimal(FObj.Text, FObj.Style.Format, out numeric);
        if (FObj.IsDateTime)
          FObj.Value = date;
        else if (FObj.IsPercent)
          FObj.Value = percent;
        else if (FObj.IsNumeric)
          FObj.Value = numeric;
        FObj.TextRenderType = Obj.TextRenderType;
        FObj.URL = Obj.Hyperlink.Kind == HyperlinkKind.URL ? Obj.Hyperlink.Value : "";
        FObj.IsText = true;
        FObj.IsRichText = false;
        if (wrap)
          FObj.WrappedText = WrapTextObject(Obj);
        AddSetObjectPos(Obj, ref FObj);
        AddShadow(Obj);
        if (fillAsBitmap && !(FObj.Style.Fill is SolidFill))
        {
          float dx = FObj.Width;
          float dy = FObj.Height;
          dx = (dx >= 1 ? dx : 1);
          dy = (dy >= 1 ? dy : 1);
          System.Drawing.Image image = new Bitmap((int)dx, (int)dy);
          using (Graphics g = Graphics.FromImage(image))
          {
            g.Clear(Color.Transparent);
            g.TranslateTransform(-Obj.AbsLeft * zoom, -Obj.AbsTop * zoom);
            BorderLines oldLines = Obj.Border.Lines;
            Obj.Border.Lines = BorderLines.None;
            string oldText = Obj.Text;
            Obj.Text = String.Empty;
            Obj.Draw(new FRPaintEventArgs(g, zoom, zoom, Report.GraphicCache));
            Obj.Text = oldText;
            Obj.Border.Lines = oldLines;
            FObj.PictureStream = new MemoryStream();
            image.Save(FObj.PictureStream, pictureFormat);
            CheckPicsCache(FObj);
          }
        }
      }
    }

    private bool IsVisible(ReportComponentBase Obj)
    {
      return !((Obj.AbsLeft + Obj.Width) < 0 ||
          (Obj.AbsTop + Obj.Height) < 0 ||
          (Obj.AbsLeft > (currentPage.Width / zoom * Units.Millimeters)) ||
          (Obj.AbsTop > (currentPage.Height / zoom * Units.Millimeters)));
    }

    private void AddShadow(ReportComponentBase Obj)
    {
      if (Obj.Border.Shadow)
      {
        using (TextObject shadow = new TextObject())
        {
          shadow.Left = Obj.AbsLeft + Obj.Width;
          shadow.Width = Obj.Border.ShadowWidth;
          shadow.Top = Obj.AbsTop + Obj.Border.ShadowWidth;
          shadow.Height = Obj.Height;
          shadow.Fill = new SolidFill(Obj.Border.ShadowColor);
          AddTextObject(shadow);
        }
        using (TextObject shadow = new TextObject())
        {
          shadow.Left = Obj.AbsLeft + Obj.Border.ShadowWidth;
          shadow.Width = Obj.Width - Obj.Border.ShadowWidth;
          shadow.Top = Obj.AbsTop + Obj.Height;
          shadow.Height = Obj.Border.ShadowWidth;
          shadow.Fill = new SolidFill(Obj.Border.ShadowColor);
          AddTextObject(shadow);
        }
      }
    }

    private List<string> WrapTextObject(TextObject obj)
    {
      float FDpiFX = 96f / DrawUtils.ScreenDpi;
      List<string> result = new List<string>();
      DrawText drawer = new DrawText();
      using (Bitmap b = new Bitmap(1, 1))
      using (Graphics g = Graphics.FromImage(b))
      using (Font f = new Font(obj.Font.Name, obj.Font.Size * FDpiFX, obj.Font.Style))
      {
        float h = f.Height - f.Height / 4;
        float memoWidth = obj.Width - obj.Padding.Horizontal;

        float memoHeight = drawer.CalcHeight(obj.Text, g, f,
            memoWidth, obj.Height - obj.Padding.Vertical,
            obj.HorzAlign, obj.LineHeight, obj.ForceJustify, obj.RightToLeft, obj.WordWrap, obj.Trimming);

        float y, prevy = 0;
        StringBuilder line = new StringBuilder(256);
        foreach (Paragraph par in drawer.Paragraphs)
        {
          foreach (Word word in par.Words)
          {
            if (!word.visible)
              break;
            y = word.top + 1;
            if (prevy == 0)
              prevy = y;
            if (y != prevy)
            {
              result.Add(line.ToString());
              line.Length = 0;
              prevy = y;
            }
            line.Append(word.text).Append(' ');
          }
        }
        result.Add(line.ToString());
      }
      return result;
    }

    public void AddPageBegin(ReportPage page)
    {
      currentPage = new ExportIEMPage();
      currentPage.Landscape = page.Landscape;
      currentPage.Width = ExportUtils.GetPageWidth(page) * zoom;
      currentPage.Height = ExportUtils.GetPageHeight(page) * zoom;
      currentPage.RawPaperSize = page.RawPaperSize;
      currentPage.LeftMargin = page.LeftMargin * zoom;
      currentPage.TopMargin = page.TopMargin * zoom;
      currentPage.RightMargin = page.RightMargin * zoom;
      currentPage.BottomMargin = page.BottomMargin * zoom;
      repeatDataband = false;
    }

    public void AddBand(Base band, object sender)
    {
      AddBand(band, sender, false);
    }

    public void AddBand(Base band, object sender, bool isHeader)
    {
      if (dataOnly && !(band is DataBand))
        return;

      if (seamless && (((band is PageHeaderBand) && !firstPage) || (band is PageFooterBand)))
        return;

      if (!((band as BandBase).Fill is TextureFill))
          AddBandObject(band as BandBase);
      else
            if (images)
            {
                if (fullTrust)
                  AddPictureObject(band as ReportComponentBase);
                else
                  AddPictureObject_Safe(band as ReportComponentBase);
            }
      foreach (Base c in band.ForEachAllConvectedObjects(sender))
      {
        if (c is ReportComponentBase && (c as ReportComponentBase).Exportable)
        {
          ReportComponentBase obj = c as ReportComponentBase;

          if (obj is CellularTextObject)
            obj = (obj as CellularTextObject).GetTable();
          if (dataOnly && (obj.Parent == null || !(obj.Parent is DataBand)))
            continue;
          if (seamless &&
                  (
                      (obj.Parent is PageFooterBand) || (obj is PageFooterBand) ||
                      (((obj.Parent is PageHeaderBand) || (obj is PageHeaderBand)) && !firstPage)
                  )
              )
            continue;
          if (obj is TableCell)
            continue;
          else if (obj is TableBase)
          {
            AddTableObject(obj as TableBase);
            repeatDataband = true;
          }
          else if (IsMemo(obj))
            AddTextObject(obj as TextObject, isHeader);
          else if (obj is BandBase)
          {
            if (!repeatDataband)
              AddBandObject(obj as BandBase);
            repeatDataband = false;
          }
          else if (IsLine(obj) ||
                    (IsRect(obj) && obj.Fill.IsTransparent))
            AddLineObject(obj);
          else if ((obj is HtmlObject) && htmlMode)
            AddHtml(obj as HtmlObject);
          else if (keepRichText && (obj is RichObject))
            AddRich(obj as RichObject);
          else if (!((obj is HtmlObject) && !htmlMode))
          {
            if (images)
            {
#if DOTNET_4
                            if ((obj is SVGObject) && htmlMode)
                                AddSVG(obj as SVGObject);
                            else
#endif
              {
                if (fullTrust)
                  AddPictureObject(obj as ReportComponentBase);
                else
                  AddPictureObject_Safe(obj as ReportComponentBase);
              }
            }
          }
        }
      }
    }

    private void AddRich(RichObject richObject)
    {
      if (IsVisible(richObject))
      {
        ExportIEMObject FObj = new ExportIEMObject();
        FObj.StyleIndex = AddStyle(richObject);
        if (FObj.StyleIndex != -1)
          FObj.Style = iemStyleList[FObj.StyleIndex];
        FObj.Text = richObject.Text;
#if CHECK_LATER
        FObj.TabWidth = richObject.TabWidth;
        if (Obj.ParagraphFormat != null)
          FObj.ParagraphFormat = Obj.ParagraphFormat.MultipleScale(1);
        if (Obj.TextRenderType == TextRenderType.HtmlParagraph)
          FObj.InlineImageCache = Obj.InlineImageCache;
        DateTime date;
#endif
        FObj.IsDateTime = false;
        FObj.IsPercent = false;
        FObj.IsNumeric = false;
        FObj.Value = null;
        FObj.TextRenderType = TextRenderType.Default;
        FObj.URL = richObject.Hyperlink.Kind == HyperlinkKind.URL ? richObject.Hyperlink.Value : "";
        FObj.IsText = true;
        FObj.IsRichText = true;
#if CHECK_LATER
        if (wrap)
          FObj.WrappedText = WrapTextObject(richObject);
#endif
        AddSetObjectPos(richObject, ref FObj);
        AddShadow(richObject);
        if (fillAsBitmap && !(FObj.Style.Fill is SolidFill))
        {
          float dx = FObj.Width;
          float dy = FObj.Height;
          dx = (dx >= 1 ? dx : 1);
          dy = (dy >= 1 ? dy : 1);
          System.Drawing.Image image = new Bitmap((int)dx, (int)dy);
          using (Graphics g = Graphics.FromImage(image))
          {
            g.Clear(Color.Transparent);
            g.TranslateTransform(-richObject.AbsLeft * zoom, -richObject.AbsTop * zoom);
            BorderLines oldLines = richObject.Border.Lines;
            richObject.Border.Lines = BorderLines.None;
            string oldText = richObject.Text;
            richObject.Text = String.Empty;
            richObject.Draw(new FRPaintEventArgs(g, zoom, zoom, Report.GraphicCache));
            richObject.Text = oldText;
            richObject.Border.Lines = oldLines;
            FObj.PictureStream = new MemoryStream();
            image.Save(FObj.PictureStream, pictureFormat);
            CheckPicsCache(FObj);
          }
        }
      }
    }

    public void AddTableObject(TableBase table)
    {
      if (table != null && table.ColumnCount > 0 && table.RowCount > 0)
      {
        table.EmulateOuterBorder();
        using (TextObject tableback = new TextObject())
        {
          tableback.Border = table.Border;
          tableback.Fill = table.Fill;
          tableback.Left = table.AbsLeft;
          tableback.Top = table.AbsTop;
          float tableWidth = 0;
          for (int i = 0; i < table.ColumnCount; i++)
            tableWidth += table.Columns[i].Width;// table[i, 0].Width;
          float tableHeight = 0;
          for (int i = 0; i < table.RowCount; i++)
            tableHeight += table.Rows[i].Height;
          tableback.Width = tableWidth;// (tableWidth < table.Width) ? tableWidth : table.Width;
          tableback.Height = tableHeight;
          AddTextObject(tableback);
        }
        float y = 0;
        for (int i = 0; i < table.RowCount; i++)
        {
          float x = 0;
          for (int j = 0; j < table.ColumnCount; j++)
          {
            if (!table.IsInsideSpan(table[j, i]))
            {
              TableCell textcell = table[j, i];
              textcell.Left = x;
              textcell.Top = y;
              if (IsMemo(textcell))
                AddTextObject(textcell as TextObject);
              else if (images)
                if (fullTrust)
                  AddPictureObject(textcell as ReportComponentBase);
                else
                  AddPictureObject_Safe(textcell as ReportComponentBase);
            }
            x += (table.Columns[j]).Width;
          }
          y += (table.Rows[i]).Height;
        }
      }
    }

    public void AddPageEnd(ReportPage page)
    {
      if (watermarks)
        AddWatermark(page, currentPage);
      deltaY = maxHeight;
      currentPage.Value = maxHeight;
      pages.Add(currentPage);
      firstPage = false;

      ObjectCollection allObjects = page.AllObjects;
      for (int i = 0; i < allObjects.Count; i++)
      {
        ReportComponentBase c = allObjects[i] as ReportComponentBase;
        if (c != null)
        {
          c.Dispose();
          c = null;
        }
      }
    }

    private void AddWatermark(ReportPage page, ExportIEMPage matrixPage)
    {
      if (page.Watermark.Enabled)
      {
        int dx = (int)(ExportUtils.GetPageWidth(page) * Units.Millimeters * zoom);
        int dy = (int)(ExportUtils.GetPageHeight(page) * Units.Millimeters * zoom);

        using (System.Drawing.Image image = new Bitmap(dx, dy))
        {
          matrixPage.WatermarkPictureStream = new MemoryStream();
          using (Graphics g = Graphics.FromImage(image))
          {
            g.Clear(Color.White);
            page.Watermark.DrawImage(new FRPaintEventArgs(g, 1, 1, page.Report.GraphicCache),
                new Rectangle(0, 0, dx, dy), Report, true);
            page.Watermark.DrawText(new FRPaintEventArgs(g, 1, 1, page.Report.GraphicCache),
                new Rectangle(0, 0, dx, dy), Report, true);
          }

          if (pictureFormat == ImageFormat.Jpeg)
            ExportUtils.SaveJpeg(image, matrixPage.WatermarkPictureStream, jpegQuality);
          else
            image.Save(matrixPage.WatermarkPictureStream, pictureFormat);

          matrixPage.WatermarkPictureStream.Position = 0;
        }
      }
    }

    public void CheckPicsCache(ExportIEMObject FObj)
    {
      FObj.Hash = Crypter.ComputeHash(FObj.PictureStream);
      FObj.Base = !((Dictionary<string, MemoryStream>)picsCache).ContainsKey(FObj.Hash);
      if (FObj.Base)
        picsCache.Add(FObj.Hash, FObj.PictureStream);
      else
        FObj.PictureStream = ((Dictionary<string, MemoryStream>)picsCache)[FObj.Hash];
    }

    public void Prepare()
    {
      ExportIEMStyle Style;
      ExportIEMObject FObj;
      ExportIEMObject FObjItem;

      if (fillArea)
      {
        Style = new ExportIEMStyle();
        Style.Fill = new SolidFill(Color.Transparent);
        FObj = new ExportIEMObject();
        FObj.StyleIndex = AddStyleInternal(Style);
        FObj.Style = Style;
        if (cropFillArea)
        {
          FObj.Left = minLeft;
          FObj.Top = minTop;
        }
        else
          FObj.Left = FObj.Top = 0;
        FObj.Width = MaxWidth;
        FObj.Height = MaxHeight;
        FObj.IsText = true;
        xPos.Add(0);
        yPos.Add(0);

        FObjItem = FObj;
        FObjItem.x = FObjItem.y = 0;
        FObjItem.dx = FObjItem.dy = 1;
        iemObjectList.Insert(0, FObjItem);
      }
      if (showProgress)
        Config.ReportSettings.OnProgress(report, res.Get("OrderByCells"));
      Config.DoEvent();
      Render();
      if (showProgress)
        Config.ReportSettings.OnProgress(report, res.Get("AnalyzeObjects"));
      Config.DoEvent();
      Analyze();
      if (optFrames)
        OptimizeFrames();
      if (showProgress)
        Config.ReportSettings.OnProgress(report, res.Get("SaveFile"));
      Config.DoEvent();
    }

    public void ObjectPos(int ObjIndex, out int x, out int y, out int dx, out int dy)
    {
      x = iemObjectList[ObjIndex].x;
      y = iemObjectList[ObjIndex].y;
      dx = iemObjectList[ObjIndex].dx;
      dy = iemObjectList[ObjIndex].dy;
    }

    public float PageBreak(int Page)
    {
      if (Page < pages.Count)
        return pages[Page].Value;
      return 0f;
    }

    public float PageWidth(int Page)
    {
      if (Page < pages.Count)
        return pages[Page].Width;
      return 0f;
    }

    public float PageHeight(int Page)
    {
      if (Page < pages.Count)
        return pages[Page].Height;
      return 0f;
    }

    public int RawPaperSize(int Page)
    {
      if (Page < pages.Count)
        return pages[Page].RawPaperSize;
      return 0;
    }

    public float PageLMargin(int Page)
    {
      if (Page < pages.Count)
        return pages[Page].LeftMargin;
      return 0f;
    }

    public float PageTMargin(int Page)
    {
      if (Page < pages.Count)
        return pages[Page].TopMargin;
      return 0f;
    }

    public float PageRMargin(int Page)
    {
      if (Page < pages.Count)
        return pages[Page].RightMargin;
      return 0f;
    }

    public float PageBMargin(int Page)
    {
      if (Page < pages.Count)
        return pages[Page].BottomMargin;
      return 0f;
    }

    public bool Landscape(int Page)
    {
      if (Page < pages.Count)
        return pages[Page].Landscape;
      return false;
    }

#endregion

    public void Dispose()
    {
      defaultOneSizeFont.Dispose();
      defaultOneSizeFont = null;
      Clear();
    }

    public ExportMatrix()
    {
      defaultOneSizeFont = new Font("Arial", 1);

      fontList = new List<string>();
      iemObjectList = new List<ExportIEMObject>();
      iemStyleList = new List<ExportIEMStyle>();

      xPos = new BinaryTree();
      yPos = new BinaryTree();

      pages = new List<ExportIEMPage>();
      maxWidth = 0;
      maxHeight = 0;
      minLeft = 99999;
      minTop = 99999;
      deltaY = 0;
      inaccuracy = 0.5F;
      rotatedImage = false;
      plainRich = true;
      fillArea = false;
      cropFillArea = false;
      optFrames = false;
      left = 0;
      printable = true;
      images = true;
      imageResolution = 96;
      wrap = false;
      brushAsBitmap = true;
      zoom = 1f;
      dataOnly = false;
      fullTrust = Config.FullTrust;
      pictureFormat = ImageFormat.Png;
      jpegQuality = 100;
      res = new MyRes("Export,Misc");
      fillAsBitmap = false;
      htmlMode = false;
      watermarks = false;
      seamless = false;
      firstPage = true;
      rowHeightIs = "exact";
      picsCache = new Dictionary<string, MemoryStream>();
    }
  }
}
