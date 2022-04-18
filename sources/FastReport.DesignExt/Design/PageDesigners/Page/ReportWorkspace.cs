using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using FastReport.Data;
using FastReport.Utils;
using FastReport.TypeConverters;
using FastReport.Design.ToolWindows;
using FastReport.Format;
using System.Net;
#if !MONO
using FastReport.DevComponents;
using FastReport.DevComponents.DotNetBar;
using FastReport.Design.ImportPlugins;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design.PageDesigners.Page
{
    internal class ReportWorkspace : DesignWorkspaceBase
    {
        #region Fields
        private bool allowPaint = true;
        private const int WM_PAINT = 0x000F;
        private bool zoomLock = false;
        private EventIndicator eventIndicator;
        private Guides guides;
        private static float scale;
        #endregion

        #region Properties
        public static new float Scale 
        { 
            get { return scale * DpiHelper.Multiplier; }
            set { scale = value; }
        }
        public static Grid Grid = new Grid();
        public static bool ShowGrid;
        public static bool SnapToGrid;
        public static bool ShowGuides;
        public static bool AutoGuides;
        public static MarkerStyle MarkerStyle;
        public static bool ClassicView;
        public static bool EditAfterInsert;
        public static bool EventObjectIndicator;
        public static bool EventBandIndicator;
        public static bool EnableBacklight;

        public event EventHandler BeforeZoom;
        public event EventHandler AfterZoom;

        public ReportPage Page
        {
            get { return PageDesigner.Page as ReportPage; }
        }

        public Guides Guides
        {
            get { return guides; }
        }

        private RulerPanel RulerPanel
        {
            get { return (PageDesigner as ReportPageDesigner).RulerPanel; }
        }
        #endregion

        #region Private Methods
        private RectangleF GetSelectedRect()
        {
            RectangleF result = new RectangleF(100000, 100000, -100000, -100000);
            foreach (Base obj in Designer.SelectedObjects)
            {
                if (obj is ComponentBase)
                {
                    ComponentBase c = obj as ComponentBase;
                    if (c.Left < result.Left)
                        result.X = c.Left;
                    if (c.Right > result.Right)
                        result.Width = c.Right - result.Left;
                    float cTop = c is BandBase ? 0 : c.Top;
                    if (cTop < result.Top)
                        result.Y = cTop;
                    float cBottom = c is BandBase ? c.Height : c.Bottom;
                    if (cBottom > result.Bottom)
                        result.Height = cBottom - result.Top;
                }
            }
            if (result.Top == 100000)
                result = new RectangleF();
            return result;
        }

        private void AddBand(BandBase band, BandCollection list)
        {
            if (band != null)
            {
                if (band.Child != null && band.Child.FillUnusedSpace)
                {
                    AddBand(band.Child, list);
                    list.Add(band);
                }
                else
                {
                    list.Add(band);
                    AddBand(band.Child, list);
                }
            }
        }

        private void EnumDataBand(DataBand band, BandCollection list)
        {
            if (band == null)
                return;

            AddBand(band.Header, list);
            AddBand(band, list);
            foreach (BandBase b in band.Bands)
            {
                EnumBand(b, list);
            }
            AddBand(band.Footer, list);
        }

        private void EnumGroupHeaderBand(GroupHeaderBand band, BandCollection list)
        {
            if (band == null)
                return;

            AddBand(band.Header, list);
            AddBand(band, list);
            EnumGroupHeaderBand(band.NestedGroup, list);
            EnumDataBand(band.Data, list);
            AddBand(band.GroupFooter, list);
            AddBand(band.Footer, list);
        }

        private void EnumBand(BandBase band, BandCollection list)
        {
            if (band is DataBand)
                EnumDataBand(band as DataBand, list);
            else if (band is GroupHeaderBand)
                EnumGroupHeaderBand(band as GroupHeaderBand, list);
        }

        private void EnumBands(BandCollection list)
        {
            if (Page.TitleBeforeHeader)
            {
                AddBand(Page.ReportTitle, list);
                AddBand(Page.PageHeader, list);
            }
            else
            {
                AddBand(Page.PageHeader, list);
                AddBand(Page.ReportTitle, list);
            }
            AddBand(Page.ColumnHeader, list);
            foreach (BandBase b in Page.Bands)
            {
                EnumBand(b, list);
            }
            AddBand(Page.ColumnFooter, list);
            AddBand(Page.ReportSummary, list);
            AddBand(Page.PageFooter, list);
            AddBand(Page.Overlay, list);
        }

        private void AdjustBands()
        {
            BandCollection bands = new BandCollection();
            EnumBands(bands);
            float curY = ClassicView ? BandBase.HeaderSize : 0;
            float pageWidth = (Page.PaperWidth - Page.LeftMargin - Page.RightMargin) * Units.Millimeters;
            float columnWidth = Page.Columns.Width * Units.Millimeters;

            // lineup bands
            foreach (BandBase b in bands)
            {
                b.Left = 0;
                if (Page.Columns.Count > 1 && b.IsColumnDependentBand)
                    b.Width = columnWidth;
                else
                    b.Width = pageWidth;
                b.Top = curY;
                curY += b.Height + (ClassicView ? BandBase.HeaderSize : 4 / Scale);
            }
            // update size
            // since we are changing the size inside the OnPaint, avoid weird effects
            int width = (int)Math.Round(pageWidth * Scale) + 1;
            if (Page.ExtraDesignWidth)
                width *= 5;
            int height = (int)Math.Round(curY * Scale);
            if (ClassicView)
                height -= BandBase.HeaderSize - 4;

            if (!mouseDown || height > Height || Top == 0)
                Size = new Size(width, height);
            else
                Width = width;

            // apply Right to Left layout if needed
            if (Config.RightToLeft)
            {
                Left = RulerPanel.VertRuler.Left - Width;
            }
        }

        private void UpdateStatusBar()
        {
            RectangleF selectedRect = GetSelectedRect();
            bool emptyBounds = Designer.SelectedObjects.IsPageSelected || Designer.SelectedObjects.IsReportSelected ||
              (selectedRect.Width == 0 && selectedRect.Height == 0);
            string location = emptyBounds ? "" :
              Converter.ToString(selectedRect.Left, typeof(UnitsConverter)) + "; " +
              Converter.ToString(selectedRect.Top, typeof(UnitsConverter));
            string size = emptyBounds ? "" :
              Converter.ToString(selectedRect.Width, typeof(UnitsConverter)) + "; " +
              Converter.ToString(selectedRect.Height, typeof(UnitsConverter));

            string text = "";
            if (Designer.SelectedObjects.Count == 1)
            {
                Base obj = Designer.SelectedObjects[0];
                if (obj is TextObject)
                {
#if !MONO
                    text = Converter.ToXml((obj as TextObject).Text);
#else
                    text = (obj as TextObject).Text;
#endif					

                }
                else if (obj is BandBase)
                    text = (obj as BandBase).GetInfoText();
            }

            Designer.ShowStatus(location, size, text);
        }

        private void UpdateAutoGuides()
        {
            if (!AutoGuides)
                return;

            Page.Guides.Clear();
            foreach (Base c in Designer.Objects)
            {
                if (c is BandBase)
                    (c as BandBase).Guides.Clear();
            }
            foreach (Base c in Designer.Objects)
            {
                if (c is ReportComponentBase && !(c is BandBase))
                {
                    ReportComponentBase obj = c as ReportComponentBase;
                    float g = obj.AbsLeft;
                    if (!Page.Guides.Contains(g))
                        Page.Guides.Add(g);
                    g = obj.AbsRight;
                    if (!Page.Guides.Contains(g))
                        Page.Guides.Add(g);
                    BandBase band = obj.Band;
                    if (band != null)
                    {
                        g = obj.AbsTop - band.AbsTop;
                        if (!band.Guides.Contains(g))
                            band.Guides.Add(g);
                        g = obj.AbsBottom - band.AbsTop;
                        if (!band.Guides.Contains(g))
                            band.Guides.Add(g);
                    }
                }
            }
            RulerPanel.HorzRuler.Refresh();
            RulerPanel.VertRuler.Refresh();
        }

        private void CreateTitlesForInsertedObjects(DictionaryWindow.DraggedItemCollection items)
        {
            if (items == null)
                return;

            bool changed = false;

            // try to create title for the inserted items
            for (int i = 0; i < items.Count; i++)
            {
                string text = items[i].text;
                Column column = items[i].obj as Column;
                ComponentBase insertedObject = Designer.SelectedObjects[i] as ComponentBase;

                if (insertedObject.Parent is DataBand)
                {
                    DataBand dataBand = insertedObject.Parent as DataBand;

                    // connect databand to data if not connected yet
                    if (dataBand.DataSource == null && column != null)
                    {
                        // find a parent datasource for a column. Use the "text" which
                        // contains full-qualified name of the column.
                        dataBand.DataSource = DataHelper.GetDataSource(Report.Dictionary, text);
                        changed = true;
                    }

                    // find a header where to insert the column title
                    BandBase header = dataBand.Header;
                    if (header == null)
                        header = (dataBand.Page as ReportPage).PageHeader;
                    if (header == null)
                        header = (dataBand.Page as ReportPage).ColumnHeader;
                    if (header != null)
                    {
                        // check for empty space on a header
                        RectangleF newBounds = insertedObject.Bounds;
                        newBounds.Inflate(-1, -1);
                        newBounds.Y = 0;
                        bool hasEmptySpace = true;
                        foreach (ReportComponentBase obj in header.Objects)
                        {
                            if (obj.Bounds.IntersectsWith(newBounds))
                            {
                                hasEmptySpace = false;
                                break;
                            }
                        }

                        // create the title
                        if (hasEmptySpace)
                        {
                            TextObject newObject = new TextObject();
                            newObject.Bounds = insertedObject.Bounds;
                            newObject.Top = 0;
                            newObject.Parent = header;
                            newObject.CreateUniqueName();

                            if (column != null)
                                text = column.Alias;
                            newObject.Text = text;
                            Designer.Objects.Add(newObject);

                            // apply last formatting to the new object
                            Designer.LastFormatting.SetFormatting(newObject);
                            changed = true;
                        }
                    }
                }
            }

            if (changed)
                Designer.SetModified(null, "Change");
        }

        private void DrawSelectionRect(Graphics g)
        {
            RectangleF rect = NormalizeSelectionRect();
            Brush b = Report.GraphicCache.GetBrush(Color.FromArgb(80, SystemColors.Highlight));
            float scale = Scale;
            g.FillRectangle(b, rect.Left * scale, rect.Top * scale, rect.Width * scale, rect.Height * scale);
            Pen pen = Report.GraphicCache.GetPen(SystemColors.Highlight, 1, DashStyle.Dash);
            g.DrawRectangle(pen, rect.Left * scale, rect.Top * scale, rect.Width * scale, rect.Height * scale);
        }
        #endregion

        #region Protected Methods
        protected override float GridSnapSize
        {
            get { return Grid.SnapSize; }
        }

        protected override bool CheckGridStep()
        {
            bool al = SnapToGrid;
            if ((ModifierKeys & Keys.Alt) > 0)
                al = !al;

            bool result = true;
            float kx = eventArgs.delta.X;
            float ky = eventArgs.delta.Y;
            if (al)
            {
                result = kx >= GridSnapSize || kx <= -GridSnapSize || ky >= GridSnapSize || ky <= -GridSnapSize;
                if (result)
                {
                    kx = (int)(kx / GridSnapSize) * GridSnapSize;
                    ky = (int)(ky / GridSnapSize) * GridSnapSize;
                }
            }
            else
            {
                result = kx != 0 || ky != 0;
            }
            if (ShowGuides && !AutoGuides)
                (Guides as Guides).CheckGuides(ref kx, ref ky);
            eventArgs.delta.X = kx;
            eventArgs.delta.Y = ky;
            return result;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Locked)
                return;
            Graphics g = e.Graphics;
            FRPaintEventArgs paintArgs = new FRPaintEventArgs(g, Scale, Scale, Report.GraphicCache);

            // check if workspace is active (in the mdi mode).
            ObjectCollection objects = Designer.Objects;
            if (Designer.ActiveReport != Report)
            {
                objects = Page.AllObjects;
                objects.Add(Page);
            }

            // draw bands
            foreach (Base obj in objects)
            {
                obj.SetDesigning(true);
                if (obj is BandBase)
                {
                    BandBase band = obj as BandBase;
                    band.Draw(paintArgs);
                    if (EventBandIndicator && eventIndicator.HaveToDraw(band))
                        eventIndicator.DrawIndicator(band, paintArgs);
                }
            }
            // draw objects
            foreach (Base obj in objects)
            {
                if (obj is ComponentBase && !(obj is BandBase) && obj.HasFlag(Flags.CanDraw))
                {

                    (obj as ComponentBase).Draw(paintArgs);
                    if (EventObjectIndicator && eventIndicator.HaveToDraw(obj as ComponentBase))
                        eventIndicator.DrawIndicator(obj as ComponentBase, paintArgs);
                }
            }
            // draw selection
            if (!mouseDown && Designer.ActiveReport == Report)
            {
                foreach (Base obj in Designer.SelectedObjects)
                {
                    if (obj is ComponentBase && obj.HasFlag(Flags.CanDraw))
                        (obj as ComponentBase).DrawSelection(paintArgs);
                }
            }

            // draw page margins in "ExtraDesignWidth" mode
            if (Page.ExtraDesignWidth)
            {
                float pageWidth = (Page.PaperWidth - Page.LeftMargin - Page.RightMargin) * Units.Millimeters * Scale;
                Pen pen = Report.GraphicCache.GetPen(Color.Red, 1, DashStyle.Dot);
                for (float x = pageWidth; x < Width; x += pageWidth)
                {
                    g.DrawLine(pen, x, 0, x, Height);
                }
            }

            if (ShowGuides)
                Guides.Draw(g);
            virtualGuides.Draw(g);
            if (mode2 == WorkspaceMode2.SelectionRect)
                DrawSelectionRect(g);
        }

        protected override void ShowLocationSizeToolTip(int x, int y)
        {
            string s = "";
            RectangleF selectedRect = GetSelectedRect();
            if (eventArgs.activeObject is BandBase)
                selectedRect = new RectangleF(0, 0, eventArgs.activeObject.Width, eventArgs.activeObject.Height);
            if (mode2 == WorkspaceMode2.Move)
            {
                s = Converter.ToString(selectedRect.Left, typeof(UnitsConverter)) + "; " +
                  Converter.ToString(selectedRect.Top, typeof(UnitsConverter));
            }
            else if (mode2 == WorkspaceMode2.Size)
            {
                s = Converter.ToString(selectedRect.Width, typeof(UnitsConverter)) + "; " +
                  Converter.ToString(selectedRect.Height, typeof(UnitsConverter));
            }
            ShowToolTip(s, x, y);
        }

        protected override void Refresh1()
        {
            // if active object is band (we are resizing it), update the band structure and vertical ruler
            if (eventArgs.activeObject is BandBase)
                UpdateBands();
            else
            {
                Refresh();
                if (EnableBacklight)
                    RulerPanel.VertRuler.Refresh();
            }
        }

        protected override void Refresh2()
        {
            UpdateBands();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Add:
                    ZoomIn();
                    break;

                case Keys.Subtract:
                    ZoomOut();
                    break;
            }

            if ((e.Modifiers & Keys.Control) != 0)
            {
                switch (e.KeyCode)
                {
                    case Keys.B:
                        if (Designer.SelectedTextObjects.Count > 0)
                            Designer.SelectedTextObjects.ToggleFontStyle(FontStyle.Bold,
                              !Designer.SelectedTextObjects.First.Font.Bold);
                        break;

                    case Keys.I:
                        if (Designer.SelectedTextObjects.Count > 0)
                            Designer.SelectedTextObjects.ToggleFontStyle(FontStyle.Italic,
                              !Designer.SelectedTextObjects.First.Font.Italic);
                        break;

                    case Keys.U:
                        if (Designer.SelectedTextObjects.Count > 0)
                            Designer.SelectedTextObjects.ToggleFontStyle(FontStyle.Underline,
                              !Designer.SelectedTextObjects.First.Font.Underline);
                        break;
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (Locked)
                return;

            if ((ModifierKeys & Keys.Control) != 0)
            {
                if (e.Delta > 0)
                    ZoomIn();
                else if (e.Delta < 0)
                    ZoomOut();
            }
            else if (ModifierKeys == Keys.Shift)
            {
                eventArgs.wheelDelta = e.Delta;
                eventArgs.handled = false;


#if !MONO
                (Parent as PanelX).DoHorizontalMouseWheel(e);
#endif
            }
            else
            {
                eventArgs.wheelDelta = e.Delta;
                eventArgs.handled = false;

                // serve all objects
                foreach (Base c in Designer.Objects)
                {
                    if (c is ComponentBase)
                    {
                        (c as ComponentBase).HandleMouseWheel(eventArgs);
                        if (eventArgs.handled)
                        {
                            Refresh();
                            return;
                        }
                    }
                }

#if !MONO
                (Parent as PanelX).DoMouseWheel(e);
#endif
            }
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            if (!Designer.cmdInsert.Enabled)
            {
                drgevent.Effect = DragDropEffects.None;
                return;
            }

            if (mode1 != WorkspaceMode1.DragDrop)
            {
                DictionaryWindow.DraggedItemCollection draggedItems = DictionaryWindow.DragUtils.GetAll(drgevent);
                if (draggedItems == null || draggedItems.Count == 0)
                {
                    if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        object obj = drgevent.Data.GetData(DataFormats.FileDrop);
                        if (obj != null)
                        {
                            string[] fileNames = obj as string[];
                            string fileName = fileNames[0];
                            string path = Path.GetExtension(fileName).ToString().ToLower();

                            // determine type of object to insert
                            Type objectType = null;
                            if (path == ".txt")
                            {
                                objectType = typeof(TextObject);
                            }
                            else if (path == ".rtf")
                            {
                                objectType = typeof(RichObject);
                            }
                            else if (path == ".png" || path == ".jpg" || path == ".gif" || path == ".jpeg" || path == ".ico" ||
                                path == ".bmp" || path == ".tif" || path == ".tiff" || path == ".emf" || path == ".wmf")
                            {
                                objectType = typeof(PictureObject);
                            }

                            if (objectType == null)
                            {
                                drgevent.Effect = DragDropEffects.None;
                                return;
                            }

                            // do insertion
                            Designer.InsertObject(RegisteredObjects.FindObject(objectType), InsertFrom.Dictionary);

                            // load content from a file
                            Base insertedObj = Designer.SelectedObjects[0];
                            if (insertedObj is TextObject)
                            {
                                (insertedObj as TextObject).Text = File.ReadAllText(fileName);
                            }
                            else if (insertedObj is RichObject)
                            {
                                (insertedObj as RichObject).Text = File.ReadAllText(fileName);
                            }
                            else if (insertedObj is PictureObject)
                            {
                                (insertedObj as PictureObject).Image = Image.FromFile(fileName);
                            }

                            eventArgs.DragSource = insertedObj as ComponentBase;
                        }
                        else
                        {
                            drgevent.Effect = DragDropEffects.None;
                            return;
                        }
                    }
                    else if (drgevent.Data.GetDataPresent(DataFormats.Text))
                    {
                        byte[] imgData = new byte[0];
                        string text = "";
                        Type objectType = null;
                        string file = (string)drgevent.Data.GetData(DataFormats.UnicodeText, false);
                        {
                            bool isImage = false;
                            Uri uri = new Uri("https://www.fastreport.ru/");
                            if (Uri.TryCreate(file, UriKind.Absolute, out uri))
                            {
                                // link can be not image path
                                try
                                {
                                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                                    using (WebClient client = new WebClient())
                                    {
                                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                                        using (Stream stream = client.OpenRead(uri.AbsoluteUri))
                                        using (Bitmap bmp = new Bitmap(stream))
                                        {
                                            if (bmp != null)
                                            {
                                                using (MemoryStream tempImgStream = new MemoryStream())
                                                {
                                                    bmp.Save(tempImgStream, bmp.GetImageFormat());
                                                    imgData = tempImgStream.ToArray();
                                                }
                                                objectType = typeof(PictureObject);
                                                isImage = true;
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                            if (!isImage)
                            {
                                text = file;

                                objectType = typeof(TextObject);
                            }
                        }

                        // do insertion
                        Designer.InsertObject(RegisteredObjects.FindObject(objectType), InsertFrom.Dictionary);

                        // load content from a file
                        Base insertedObj = Designer.SelectedObjects[0];
                        if (insertedObj is TextObject)
                        {
                            (insertedObj as TextObject).Text = text;
                        }
                        else if (insertedObj is PictureObject)
                        {
                            (insertedObj as PictureObject).Image = Image.FromStream(new MemoryStream(imgData));
                        }

                        eventArgs.DragSource = insertedObj as ComponentBase;
                    }

                }
                else
                {
                    ObjectInfo[] infos = new ObjectInfo[draggedItems.Count];
                    for (int i = 0; i < draggedItems.Count; i++)
                    {
                        Type objType = typeof(TextObject);

                        if (draggedItems[i].obj is Column)
                        {
                            Column c = draggedItems[i].obj as Column;
                            objType = c.GetBindableControlType();
                        }

                        infos[i] = RegisteredObjects.FindObject(objType);
                    }

                    Designer.InsertObject(infos, InsertFrom.Dictionary);

                    List<ComponentBase> dragSources = new List<ComponentBase>();

                    if (Designer.SelectedObjects.Count == draggedItems.Count)
                    {
                        for (int i = 0; i < Designer.SelectedObjects.Count; i++)
                        {
                            ComponentBase obj = Designer.SelectedObjects[i] as ComponentBase;
                            DictionaryWindow.DraggedItem item = draggedItems[i];

                            if (obj == null || item == null)
                                continue;

                            if (obj is TextObject)
                            {
                                TextObject textObj = obj as TextObject;
                                textObj.Text = textObj.GetTextWithBrackets(item.text);
                                if (item.obj is Column)
                                {
                                    Column c = item.obj as Column;
                                    if (c.DataType == typeof(float) || c.DataType == typeof(double) || c.DataType == typeof(decimal))
                                    {
                                        textObj.HorzAlign = HorzAlign.Right;
                                        textObj.WordWrap = false;
                                        textObj.Trimming = StringTrimming.EllipsisCharacter;
                                    }
                                    textObj.Format = c.GetFormat();
                                }
                            }
                            else if (obj is PictureObject)
                            {
                                (obj as PictureObject).DataColumn = item.text;
                            }
                            else if (obj is CheckBoxObject)
                            {
                                (obj as CheckBoxObject).DataColumn = item.text;
                            }

                            dragSources.Add(obj);
                        }
                    }

                    eventArgs.dragSources = dragSources.ToArray();
                }
            }

            mode1 = WorkspaceMode1.DragDrop;
            Point pt = PointToClient(new Point(drgevent.X, drgevent.Y));
            OnMouseMove(new MouseEventArgs(MouseButtons.Left, 0, pt.X, pt.Y, 0));
            drgevent.Effect = drgevent.AllowedEffect;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            Point pt = PointToClient(new Point(drgevent.X, drgevent.Y));
            OnMouseUp(new MouseEventArgs(MouseButtons.Left, 0, pt.X, pt.Y, 0));

            if (eventArgs.dragSources != null && eventArgs.dragSources.Length > 0)
            {
                DictionaryWindow.DraggedItemCollection items = DictionaryWindow.DragUtils.GetAll(drgevent);
                if (items == null)
                    return;
                CreateTitlesForInsertedObjects(items);
                items.Clear();
            }
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            CancelPaste();
        }

        protected override void WndProc(ref Message m)
        {
            if ((m.Msg != WM_PAINT) || (allowPaint && m.Msg == WM_PAINT))
            {
                base.WndProc(ref m);
            }
        }
        #endregion

        #region Public Methods
        public override float GetScale()
        {
            return Scale;
        }

        protected override Base GetParentForPastedObjects()
        {
            BandCollection bands = new BandCollection();
            EnumBands(bands);
            return bands[0];
        }

        public override void Paste(ObjectCollection list, InsertFrom source)
        {
            base.Paste(list, source);
            // find left-top edge of pasted objects
            float minLeft = 100000;
            float minTop = 100000;
            foreach (Base c in list)
            {
                if (c is ComponentBase)
                {
                    ComponentBase c1 = c as ComponentBase;
                    if (c1.Left < minLeft)
                        minLeft = c1.Left;
                    if (c1.Top < minTop)
                        minTop = c1.Top;
                }
            }
            foreach (Base c in list)
            {
                // correct the left-top
                if (c is ComponentBase)
                {
                    ComponentBase c1 = c as ComponentBase;
                    c1.Left -= minLeft + Grid.SnapSize * 1000;
                    c1.Top -= minTop + Grid.SnapSize * 1000;
                    if (c1.Width == 0 && c1.Height == 0)
                    {
                        SizeF preferredSize = c1.GetPreferredSize();
                        c1.Width = preferredSize.Width;
                        c1.Height = preferredSize.Height;
                        if (SnapToGrid)
                        {
                            c1.Width = (int)Math.Round(c1.Width / Grid.SnapSize) * Grid.SnapSize;
                            c1.Height = (int)Math.Round(c1.Height / Grid.SnapSize) * Grid.SnapSize;
                        }
                    }
                }
            }
            mode1 = WorkspaceMode1.Insert;
            mode2 = WorkspaceMode2.Move;
            eventArgs.activeObject = null;
            int addSize = 0;
            if (ClassicView)
                addSize = BandBase.HeaderSize;
            OnMouseDown(new MouseEventArgs(MouseButtons.Left, 0,
              10 - (int)(Grid.SnapSize * 1000 * Scale), 10 + addSize - (int)(Grid.SnapSize * 1000 * Scale), 0));
        }

        public void UpdateBands()
        {
            Refresh();
            RulerPanel.VertRuler.Refresh();
            RulerPanel.Structure.Refresh();
        }

        public override void Refresh()
        {
            AdjustBands();
            UpdateStatusBar();
            UpdateAutoGuides();
            base.Refresh();
        }

        public void DeleteHGuides()
        {
            foreach (Base c in Designer.Objects)
            {
                if (c is BandBase)
                    (c as BandBase).Guides.Clear();
            }
            Refresh();
            RulerPanel.VertRuler.Refresh();
            Designer.SetModified(PageDesigner, "DeleteHGuides");
        }

        public void DeleteVGuides()
        {
            Page.Guides.Clear();
            Refresh();
            RulerPanel.HorzRuler.Refresh();
            Designer.SetModified(PageDesigner, "DeleteVGuides");
        }

        public void Zoom(float zoom)
        {
            if (zoomLock)
                return;

            allowPaint = false;
            zoomLock = true;

            try
            {
                EventHandler beforeZoom = BeforeZoom;
                if (beforeZoom != null)
                    beforeZoom.Invoke(this, EventArgs.Empty);

                Scale = zoom / DpiHelper.Multiplier;
                Designer.UpdatePlugins(null);

                EventHandler afterZoom = AfterZoom;
                if (afterZoom != null)
                    afterZoom.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                zoomLock = false;
                allowPaint = true;
                Refresh();
            }
        }

        public void ZoomIn()
        {
            float zoom = Scale;
            zoom += 0.25f;
            if (zoom > 8f)
                zoom = 8f;
            Zoom(zoom);
        }

        public void ZoomOut()
        {
            float zoom = Scale;
            zoom -= 0.25f;
            if (zoom < 0.25f)
                zoom = 0.25f;
            Zoom(zoom);
        }

        public void FitPageWidth()
        {
            float actualWidth = Width / Scale;
            Zoom((Parent.Width - 10) / actualWidth);
        }

        public void FitWholePage()
        {
            float actualWidth = Width / Scale;
            float actualHeight = Height / Scale;
            float scaleX = (Parent.Width - 10) / actualWidth;
            float scaleY = (Parent.Height - 10) / actualHeight;
            Zoom(scaleX < scaleY ? scaleX : scaleY);
        }

        public void SelectAll()
        {
            Base parent = null;

            if (Designer.SelectedObjects.Count == 0)
                return;

            if (Designer.SelectedObjects[0] is Report || Designer.SelectedObjects[0] is PageBase ||
              Designer.SelectedObjects[0].Report == null)
                parent = Page;
            else if (Designer.SelectedObjects[0] is BandBase)
                parent = Designer.SelectedObjects[0];
            else
            {
                parent = Designer.SelectedObjects[0];
                while (parent != null && !(parent is BandBase))
                {
                    parent = parent.Parent;
                }
            }

            Designer.SelectedObjects.Clear();
            if (parent is PageBase)
            {
                // if page is selected, select all bands on the page
                foreach (Base c in parent.AllObjects)
                {
                    if (c is BandBase)
                        Designer.SelectedObjects.Add(c);
                }
            }
            else if (parent is BandBase)
            {
                // if band is selected, select all objects on the band. Do not select sub-bands.
                foreach (Base c in parent.ChildObjects)
                {
                    if (!(c is BandBase))
                        Designer.SelectedObjects.Add(c);
                }
            }
            if (Designer.SelectedObjects.Count == 0)
                Designer.SelectedObjects.Add(parent);
            Designer.SelectionChanged(null);
        }

        #endregion

        internal ReportWorkspace(ReportPageDesigner pageDesigner)
            : base(pageDesigner)
        {
            guides = new Guides(this);
            eventIndicator = new EventIndicator();
            Size = new Size(1, 1);
        }

        static ReportWorkspace()
        {
            XmlItem xi = Config.Root.FindItem("Designer").FindItem("Report");
            string units = xi.GetProp("Units");
            switch (units)
            {
                case "Millimeters":
                    Grid.GridUnits = PageUnits.Millimeters;
                    break;
                case "Centimeters":
                    Grid.GridUnits = PageUnits.Centimeters;
                    break;
                case "Inches":
                    Grid.GridUnits = PageUnits.Inches;
                    break;
                case "HundrethsOfInch":
                    Grid.GridUnits = PageUnits.HundrethsOfInch;
                    break;
            }
            string size = xi.GetProp("SnapSizeMillimeters");
            if (size != "")
                Grid.SnapSizeMillimeters = Converter.StringToFloat(size);
            size = xi.GetProp("SnapSizeCentimeters");
            if (size != "")
                Grid.SnapSizeCentimeters = Converter.StringToFloat(size);
            size = xi.GetProp("SnapSizeInches");
            if (size != "")
                Grid.SnapSizeInches = Converter.StringToFloat(size);
            size = xi.GetProp("SnapSizeHundrethsOfInch");
            if (size != "")
                Grid.SnapSizeHundrethsOfInch = Converter.StringToFloat(size);
            Grid.Dotted = xi.GetProp("DottedGrid") != "0";
            ShowGrid = xi.GetProp("ShowGrid") != "0";
            SnapToGrid = xi.GetProp("SnapToGrid") != "0";
            MarkerStyle = xi.GetProp("MarkerStyle") == "Rectangle" ?
              MarkerStyle.Rectangle : MarkerStyle.Corners;
            string s = xi.GetProp("Scale");
            if (s != "")
                Scale = Converter.StringToFloat(s);
            else
                Scale = 1;
            AutoGuides = xi.GetProp("AutoGuides") == "1";
            ClassicView = xi.GetProp("ClassicView") == "1";
            EditAfterInsert = xi.GetProp("EditAfterInsert") == "1";
            ShowGuides = true;
            EventObjectIndicator = xi.GetProp("EventObjectIndicator") == "1";
            EventBandIndicator = xi.GetProp("EventBandIndicator") == "1";
            EnableBacklight = xi.GetProp("EnableBacklight") == "1";
        }

    }
}
