using FastReport.Barcode;
using FastReport.Table;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FastReport.Export.Zpl
{
    /// <summary>
    /// Represents the Zpl export filter.
    /// </summary>
    public partial class ZplExport : ExportBase
    {
        #region Public properties

        /// <summary>
        /// Enum of densty types of Zebra printers.
        /// </summary>
        public enum ZplDensity
        {
            /// <summary>
            /// 6 dpmm(152 dpi)
            /// </summary>
            d6_dpmm_152_dpi,

            /// <summary>
            /// 8 dpmm(203 dpi)
            /// </summary>
            d8_dpmm_203_dpi,

            /// <summary>
            /// 12 dpmm(300 dpi)
            /// </summary>
            d12_dpmm_300_dpi,

            /// <summary>
            /// 24 dpmm(600 dpi)
            /// </summary>
            d24_dpmm_600_dpi
        }

        /// <summary>
        /// Sets the density of printer.
        /// </summary>
        public ZplDensity Density
        {
            get { return density; }
            set { density = value; }
        }

        /// <summary>
        /// Sets the init string for sending before printing the document.
        /// </summary>
        public string PrinterInit
        {
            get { return printerInit; }
            set { printerInit = value; }
        }

        /// <summary>
        /// Sets the code page of document. Default is UTF-8 (^CI28).
        /// </summary>
        public string CodePage
        {
            get { return codePage; }
            set { codePage = value; }
        }

        /// <summary>
        /// Sets the string for sending after printing the document.
        /// </summary>
        public string PrinterFinish
        {
            get { return printerFinish; }
            set { printerFinish = value; }
        }

        /// <summary>
        /// Sets the string for sending before printing each page.
        /// </summary>
        public string PageInit
        {
            get { return pageInit; }
            set { pageInit = value; }
        }

        /// <summary>
        /// Sets the scale font size.
        /// </summary>
        public float FontScale
        {
            get { return fontScale / defaultFontScale; }
            set { fontScale = value * defaultFontScale; }
        }

        /// <summary>
        /// Sets the Printer Font, default value is "A".
        /// </summary>
        public string PrinterFont
        {
            get { return font; }
            set { font = value; }
        }

        /// <summary>
        /// Enable or disable export as bitmap.
        /// </summary>
        public bool PrintAsBitmap
        {
            get { return printAsBitmap; }
            set { printAsBitmap = value; }
        }

        #endregion Public properties

        #region Private constants

        private const float millimeters = 3.78f;
        private const float defaultFontScale = 1.4f;

        #endregion Private constants

        #region Private Fields

#if READONLY_STRUCTS
        private readonly struct ZplScale
#else
        private struct ZplScale
#endif
        {
            public readonly float PageScale;
            public readonly float BarcodeScale;
            public readonly int TwoDCodeScale;

            public ZplScale(float pageScale, float barcodeScale, int qrCodeScale)
            {
                PageScale = pageScale;
                BarcodeScale = barcodeScale;
                TwoDCodeScale = qrCodeScale;
            }
        }

        // scales of X and Y position and sized depend by ZplDensity
        // second value is multiplier for barcode zoom
        private ZplScale[] zplScaleArray =
        {
            //6 dpmm(152 dpi)
            new ZplScale(1.5833f, 2f, 7),
            //8 dpmm(203 dpi)
            new ZplScale(2.11458f, 2.5f, 8),
            //12 dpmm(300 dpi)
            new ZplScale(3.16667f, 4f, 10),
            //24 dpmm(600 dpi)
            new ZplScale(6.34375f, 8f, 10)
        };

        // Enable for Print as Bitmap;
        private bool printAsBitmap = true;

        private ZplDensity density = ZplDensity.d8_dpmm_203_dpi;

        // Printer Init String. Sends before document.
        private string printerInit;

        // Printer Finish String. Sends after document.
        private string printerFinish;

        // Printer page init string. Sends before page.
        private string pageInit;

        // Font scale factor.
        private float fontScale = defaultFontScale;

        // Position in ZipScale array by default (8 dpmm(203 dpi)
        private int scaleIndex = 1;

        private float leftMargin = 0;
        private float topMargin = 0;

        // font
        private string font = "A";

        private string codePage = "^CI28";

        // Bitmap for rendering objects
        private Bitmap pageBitmap = null;

        // Table with counters for compression the bitmaps
        private Dictionary<int, char> countTable;

        private int[] counts;

        #endregion Private Fields

        #region Private Methods

        /// <summary>
        /// Writes the string value in  stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        private void Write(Stream stream, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                byte[] buf = Encoding.UTF8.GetBytes(value);
                stream.Write(buf, 0, buf.Length);
            }
        }

        /// <summary>
        /// Writes the string value in stream with CRLF.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        private void WriteLn(Stream stream, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                byte[] buf = Encoding.UTF8.GetBytes(value);
                stream.Write(buf, 0, buf.Length);
                stream.WriteByte(13);
                stream.WriteByte(10);
            }
        }

        /// <summary>
        /// Gets the left position in zpl units.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        private int GetLeft(float left)
        {
            return (int)Math.Round(left * zplScaleArray[scaleIndex].PageScale + leftMargin);
        }

        /// <summary>
        /// Gets the top position in zpl units.
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        private int GetTop(float top)
        {
            return (int)Math.Round(top * zplScaleArray[scaleIndex].PageScale + topMargin);
        }

        private int GetWidth(float width, float height, int angle)
        {
            if (angle == 90 || angle == 270)
                return (int)Math.Round(height * zplScaleArray[scaleIndex].PageScale);
            else
                return (int)Math.Round(width * zplScaleArray[scaleIndex].PageScale);
        }

        private int GetHeight(float width, float height, int angle)
        {
            if (angle == 90 || angle == 270)
                return (int)Math.Round(width * zplScaleArray[scaleIndex].PageScale);
            else
                return (int)Math.Round(height * zplScaleArray[scaleIndex].PageScale);
        }

        private string GetBarcodeZoom(float zoom, BarcodeBase b)
        {
            return (Math.Round(zoom * zplScaleArray[scaleIndex].BarcodeScale) * (b is BarcodePDF417 ? 2 : 1)).ToString();
        }

        private string GetZPLText(string source)
        {
            source = source.Replace("\\", "\\\\").Replace("\r\n", "\\&").Replace("$", "\\$");
            return source;
        }

        private string GetOrientation(int angle)
        {
            if (angle == 90)
                return "^FWR";
            else if (angle == 180)
                return "^FWI";
            else if (angle == 270)
                return "^FWB";
            else
                return "^FWN";
        }

        /// <summary>
        /// Exports the TableObject.
        /// </summary>
        /// <param name="table"></param>
        private void ExportTableObject(TableBase table)
        {
            if (table.ColumnCount > 0 && table.RowCount > 0)
            {
                StringBuilder tableBorder = new StringBuilder(64);
                using (TextObject tableback = new TextObject())
                {
                    tableback.Border = table.Border;
                    tableback.Fill = table.Fill;
                    tableback.FillColor = table.FillColor;
                    tableback.Left = table.AbsLeft;
                    tableback.Top = table.AbsTop;
                    float tableWidth = 0;
                    float tableHeight = 0;
                    for (int i = 0; i < table.ColumnCount; i++)
                        tableWidth += table[i, 0].Width;
                    for (int i = 0; i < table.RowCount; i++)
                        tableHeight += table.Rows[i].Height;
                    tableback.Width = (tableWidth < table.Width) ? tableWidth : table.Width;
                    tableback.Height = tableHeight;
                    ExportTextObject(tableback);
                }
                AddTable(table);
            }
        }

        private void AddTable(TableBase table)
        {
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
                        if (textcell is TextObject)
                            ExportTextObject(textcell as TextObject);
                        else
                            ExportPictureObject(textcell as ReportComponentBase);
                    }
                    x += (table.Columns[j]).Width;
                }
                y += (table.Rows[i]).Height;
            }
        }

        // Copied from TIFF export with some changes
        private Bitmap ConvertToBitonal(Bitmap original)
        {
            Bitmap source = null;
            if (original.PixelFormat != PixelFormat.Format32bppArgb)
            {
                source = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
                source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                using (Graphics g = Graphics.FromImage(source))
                    g.DrawImageUnscaled(original, 0, 0);
            }
            else
                source = original;
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int imageSize = sourceData.Stride * sourceData.Height;
            byte[] sourceBuffer = new byte[imageSize];
            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);
            source.UnlockBits(sourceData);
            Bitmap destination = new Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);
            BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
            imageSize = destinationData.Stride * destinationData.Height;
            byte[] destinationBuffer = new byte[imageSize];
            int sourceIndex = 0;
            int destinationIndex = 0;
            int pixelTotal = 0;
            byte destinationValue = 0;
            int pixelValue = 128;
            int height = source.Height;
            int width = source.Width;
            int threshold = 500;
            for (int y = 0; y < height; y++)
            {
                sourceIndex = y * sourceData.Stride;
                destinationIndex = y * destinationData.Stride;
                destinationValue = 0;
                pixelValue = 128;
                for (int x = 0; x < width; x++)
                {
                    pixelTotal = sourceBuffer[sourceIndex + 1] +
                        sourceBuffer[sourceIndex + 2] +
                        sourceBuffer[sourceIndex + 3];
                    if (pixelTotal > threshold)
                        destinationValue += (byte)pixelValue;
                    if (pixelValue == 1)
                    {
                        destinationBuffer[destinationIndex++] = destinationValue;
                        destinationValue = 0;
                        pixelValue = 128;
                    }
                    else
                        pixelValue >>= 1;
                    sourceIndex += 4;
                }
                if (pixelValue != 128)
                    destinationBuffer[destinationIndex] = destinationValue;
            }
            Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);
            destination.UnlockBits(destinationData);
            if (source != original)
                source.Dispose();
            return destination;
        }

        private void ExportPictureObject(ReportComponentBase pic)
        {
            WriteLn(Stream, SetPosition(GetLeft(pic.AbsLeft), GetTop(pic.AbsTop)));
            Border existingBorder = pic.Border.Clone();
            pic.Border.Lines = BorderLines.None;
            WriteLn(Stream, DrawPictureObject(pic));
            pic.Border = existingBorder;
            DrawBorders(pic.Border, pic.AbsLeft, pic.AbsTop, pic.Width, pic.Height);
        }

        private string DrawPictureObject(ReportComponentBase pic)
        {
            string result = String.Empty;
            if (pic.Width > 0 && pic.Height > 0)
            {
                float zoom = zplScaleArray[scaleIndex].PageScale;
                int picWidth = (int)Math.Ceiling(pic.Width * zoom);
                int picHeight = (int)Math.Ceiling(pic.Height * zoom);
                using (Bitmap image = new Bitmap(picWidth, picHeight, PixelFormat.Format32bppArgb))
                {
                    DrawObjectOnBitmap(image, pic, false);
                    result = DrawBWPicture(image);
                }
            }
            return result;
        }

        private void DrawObjectOnBitmap(Bitmap image, ReportComponentBase pic, bool usePosition)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                using (GraphicCache cache = new GraphicCache())
                {
                    if (!usePosition)
                        g.Clear(Color.White);
                    float Left = pic.Width >= 0 ? pic.AbsLeft : pic.AbsLeft + pic.Width;
                    float Top = pic.Height >= 0 ? pic.AbsTop : pic.AbsTop + pic.Height;
                    float zoom = zplScaleArray[scaleIndex].PageScale;
                    if (!usePosition)
                        g.TranslateTransform(-Left * zoom, -Top * zoom);
                    pic.Draw(new FRPaintEventArgs(g, zoom, zoom, cache));
                }
            }
        }

        private string DrawBWPicture(Bitmap image)
        {
            string result = String.Empty;
            // Convert to BW
            using (Bitmap bwImage = ConvertToBitonal(image))
            // Save in ZPL
            {
                result = CompressBWImage(bwImage);
            }
            return result;
        }

        private string CompressBWImage(Bitmap image)
        {
            BitmapData imgData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
            int imageBitsSize = imgData.Stride * imgData.Height;
            byte[] picBytes = new byte[imageBitsSize];
            Marshal.Copy(imgData.Scan0, picBytes, 0, imageBitsSize);
            image.UnlockBits(imgData);

            int widthInBytes = (int)Math.Ceiling(image.Width / 8f);
            int imageSize = widthInBytes * image.Height;
            int freeBits = 8 - image.Width % 8;
            byte lastByteMask = (byte)((1 << freeBits) - 1);
            StringBuilder zplImage = new StringBuilder(imageSize * 2);
            zplImage.AppendFormat("^GFA,{0},{1},{2},",
                    imageSize.ToString(),
                    imageSize.ToString(),
                    widthInBytes.ToString());
            StringBuilder previousLine = new StringBuilder();
            for (int i = 0; i < image.Height; i++)
            {
                StringBuilder line = new StringBuilder(widthInBytes * 2);
                StringBuilder computedLine = new StringBuilder(widthInBytes);
                bool lastZeros = true;
                for (int j = 0; j < widthInBytes; j++)
                {
                    byte b = (byte)~(picBytes[i * imgData.Stride + j]);
                    if (j == widthInBytes - 1)
                    {
                        b = (byte)(b & ~lastByteMask);
                    }
                    if (b != 0)
                    {
                        lastZeros = false;
                        computedLine.Append(line);
                        line.Length = 0;
                    }
                    else
                    {
                        if (!lastZeros)
                        {
                            computedLine.Append(line);
                            line.Length = 0;
                        }
                        lastZeros = true;
                    }
                    line.Append(b.ToString("X2"));
                }
                if (lastZeros)
                    computedLine.Append(",");
                else
                    computedLine.Append(line);
                if (previousLine.Equals(computedLine))
                    zplImage.Append(":");
                else
                {
                    zplImage.Append(CompressLine(computedLine));
                    previousLine = computedLine;
                }
            }
            zplImage.Append("^FS");
            return zplImage.ToString();
        }

        private StringBuilder CompressLine(StringBuilder line)
        {
            StringBuilder result = new StringBuilder();
            char previousChar = '-';  // different initial char
            int counter = 0;
            for (int i = 0; i < line.Length; i++)
            {
                char currentChar = line[i]; // this line for better understanding
                if (currentChar == previousChar) // char is repeating
                {
                    if (counter == 0) // char is not counted
                    {
                        result.Length = result.Length - 1; // remove last char and keep count
                        counter += 2;  // add previous and current
                    }
                    else
                        counter++; // keep count
                }
                else
                {
                    if (counter > 0) // current char is different and we have repeated before
                    {
                        result. // append to result
                            Append(GetCount(counter)).  // count
                            Append(previousChar).       // compressed char
                            Append(currentChar);        // current char
                        counter = 0; // reset counter
                    }
                    else
                        result.Append(currentChar);  // just add current char to result
                    previousChar = currentChar;  // save current char as previous
                }
            }
            if (counter > 0)
            {
                result.Append(GetCount(counter)).Append(previousChar);
            }
            return result;
        }

        private StringBuilder GetCount(int counter)
        {
            StringBuilder sb = new StringBuilder();
            int remainder = counter;
            int index = counts.Length - 1;
            while (remainder > 0 && index >= 0)
            {
                if (counts[index] <= remainder)
                {
                    sb.Append(countTable[counts[index]]);
                    remainder -= counts[index];
                }
                index--;
            }
            return sb;
        }

        /// <summary>
        /// Exports the LineObject.
        /// </summary>
        /// <param name="lineObject"></param>
        private void ExportLineObject(LineObject lineObject)
        {
            if (lineObject.Width == 0 || lineObject.Height == 0)
                ExportRectangle(lineObject.AbsLeft,
                    lineObject.AbsTop,
                    lineObject.Width,
                    lineObject.Height,
                    lineObject.Border.Width);
            else
                ExportLine(lineObject.AbsLeft,
                    lineObject.AbsTop,
                    lineObject.Width,
                    lineObject.Height,
                    lineObject.Border.Width);
        }

        /// <summary>
        /// Exports the ShapeObject.
        /// </summary>
        /// <param name="shapeObject"></param>
        private void ExportShapeObject(ShapeObject shapeObject)
        {
            if (shapeObject.Shape == ShapeKind.Ellipse) //-V3024
                ExportEllipse(shapeObject.AbsLeft,
                    shapeObject.AbsTop,
                    shapeObject.Width,
                    shapeObject.Height,
                    shapeObject.Border.Width);
            else
                ExportRectangle(shapeObject.AbsLeft,
                    shapeObject.AbsTop,
                    shapeObject.Width,
                    shapeObject.Height,
                    shapeObject.Border.Width);
        }

        private void ExportRectangle(float Left, float Top, float Width, float Height, float LineWidth)
        {
            int left = Width > 0 ? GetLeft(Left) : GetLeft(Left + Width);
            int top = Height > 0 ? GetLeft(Top) : GetTop(Top + Height);
            int width = (int)Math.Abs(Math.Round(Width * zplScaleArray[scaleIndex].PageScale));
            int height = (int)Math.Abs(Math.Round(Height * zplScaleArray[scaleIndex].PageScale));
            int lineWidth = (int)Math.Round(LineWidth * zplScaleArray[scaleIndex].PageScale);
            WriteLn(Stream, SetPosition(left, top));
            WriteLn(Stream, DrawRectangle(width, height, lineWidth));
        }

        private void ExportEllipse(float Left, float Top, float Width, float Height, float LineWidth)
        {
            int left = Width > 0 ? GetLeft(Left) : GetLeft(Left + Width);
            int top = Height > 0 ? GetLeft(Top) : GetTop(Top + Height);
            int width = (int)Math.Abs(Math.Round(Width * zplScaleArray[scaleIndex].PageScale));
            int height = (int)Math.Abs(Math.Round(Height * zplScaleArray[scaleIndex].PageScale));
            int lineWidth = (int)Math.Round(LineWidth * zplScaleArray[scaleIndex].PageScale);
            WriteLn(Stream, SetPosition(left, top));
            WriteLn(Stream, DrawEllipse(width, height, lineWidth));
        }

        private void ExportLine(float Left, float Top, float Width, float Height, float LineWidth)
        {
            int left = Width > 0 ? GetLeft(Left) : GetLeft(Left + Width);
            int top = Height > 0 ? GetLeft(Top) : GetTop(Top + Height);
            int width = (int)Math.Abs(Math.Round(Width * zplScaleArray[scaleIndex].PageScale));
            int height = (int)Math.Abs(Math.Round(Height * zplScaleArray[scaleIndex].PageScale));
            bool direction = (Width > 0 && Height > 0) || (Width < 0 && Height < 0);
            int lineWidth = (int)Math.Round(LineWidth * zplScaleArray[scaleIndex].PageScale);
            WriteLn(Stream, SetPosition(left, top));
            WriteLn(Stream, DrawLine(width, height, lineWidth, direction));
        }

        /// <summary>
        /// Exports the TextObject.
        /// </summary>
        /// <param name="textObject"></param>
        private void ExportTextObject(TextObject textObject)
        {
            // calc the width of object
            int width = GetWidth(textObject.Width, textObject.Height, textObject.Angle);
            // calc lines count
            // to-do: need fix with AdvancedTextRenderer
            int lines = (int)Math.Round(textObject.Height / Math.Round(textObject.Font.Height * DrawUtils.ScreenDpiFX));
            // calc line height
            int lineHeight = (int)Math.Round(textObject.Font.Height * zplScaleArray[scaleIndex].PageScale);
            // calc font height
            int fontHeight = (int)Math.Round(Math.Round(textObject.Font.Height * DrawUtils.ScreenDpiFX) * zplScaleArray[scaleIndex].PageScale / fontScale);
            // calc font width
            int fontWidth = (int)Math.Round((float)fontHeight / 2);
            // calc top position of text
            int top;
            if (textObject.VertAlign == VertAlign.Top)
                top = GetTop(textObject.AbsTop);
            else if (textObject.VertAlign == VertAlign.Bottom)
                top = GetTop(textObject.AbsTop) +
                    GetHeight(textObject.Width, textObject.Height, textObject.Angle) -
                    fontHeight * lines;
            else
                top = GetTop(textObject.AbsTop) +
                    (int)(GetHeight(textObject.Width, textObject.Height, textObject.Angle) / 2) -
                    (int)Math.Round((float)(fontHeight * lines) / 2);
            // set-up position
            WriteLn(Stream, SetPosition(GetLeft(textObject.AbsLeft), top));
            // set-up the text attribs
            WriteLn(Stream, SetTextAttributes(width, lines, 0, textObject.HorzAlign, 0));
            WriteLn(Stream, GetOrientation(textObject.Angle));
            // draw text
            WriteLn(Stream, DrawText(fontHeight, fontWidth, textObject.Text));
            // draw borders
            DrawBorders(textObject.Border, textObject.AbsLeft, textObject.AbsTop, textObject.Width, textObject.Height);
        }

        private void DrawBorders(Border border, float AbsLeft, float AbsTop, float Width, float Height)
        {
            if (border.Width > 0)
            {
                if (border.Lines == BorderLines.All)
                    ExportRectangle(AbsLeft, AbsTop, Width, Height, border.Width);
                else
                {
                    int left = Width > 0 ? GetLeft(AbsLeft) : GetLeft(AbsLeft + Width);
                    int top = Height > 0 ? GetLeft(AbsTop) : GetTop(AbsTop + Height);
                    int width = (int)Math.Abs(Math.Round(Width * zplScaleArray[scaleIndex].PageScale));
                    int height = (int)Math.Abs(Math.Round(Height * zplScaleArray[scaleIndex].PageScale));
                    int lineWidth = (int)Math.Round(border.Width * zplScaleArray[scaleIndex].PageScale);

                    if ((BorderLines.Top & border.Lines) != 0)
                    {
                        lineWidth = (int)Math.Round(border.TopLine.Width * zplScaleArray[scaleIndex].PageScale);
                        WriteLn(Stream, SetPosition(left, top));
                        WriteLn(Stream, DrawRectangle(width, 0, lineWidth));
                    }
                    if ((BorderLines.Left & border.Lines) != 0)
                    {
                        lineWidth = (int)Math.Round(border.LeftLine.Width * zplScaleArray[scaleIndex].PageScale);
                        WriteLn(Stream, SetPosition(left, top));
                        WriteLn(Stream, DrawRectangle(0, height, lineWidth));
                    }
                    if ((BorderLines.Bottom & border.Lines) != 0)
                    {
                        lineWidth = (int)Math.Round(border.BottomLine.Width * zplScaleArray[scaleIndex].PageScale);
                        WriteLn(Stream, SetPosition(left, top + height));
                        WriteLn(Stream, DrawRectangle(width, 0, lineWidth));
                    }
                    if ((BorderLines.Right & border.Lines) != 0)
                    {
                        lineWidth = (int)Math.Round(border.RightLine.Width * zplScaleArray[scaleIndex].PageScale);
                        WriteLn(Stream, SetPosition(left + width, top));
                        WriteLn(Stream, DrawRectangle(0, height, lineWidth));
                    }
                }
            }
        }

        private void ExportBarcodeObject(BarcodeObject b)
        {
            int height = GetHeight(b.Width, b.Height, b.Angle);
            WriteLn(Stream, SetPosition(GetLeft(b.AbsLeft), GetTop(b.AbsTop)));
            if (!(b.Barcode is BarcodeQR))
            {
                WriteLn(Stream, "^BY" + GetBarcodeZoom(b.Zoom, b.Barcode) + ",," + height.ToString());
                WriteLn(Stream, GetOrientation(b.Angle));
                char printLine = (b.ShowText ? 'Y' : 'N');
                if (b.Barcode is Barcode128 || b.Barcode is BarcodeEAN128)
                    WriteLn(Stream, String.Format("^BC,,{0},N,N", printLine));
                else if (b.Barcode is Barcode2of5Industrial)
                    WriteLn(Stream, String.Format("^BI,,{0},N", printLine));
                else if (b.Barcode is Barcode2of5Interleaved)
                    WriteLn(Stream, String.Format("^B2,,{0},N,N", printLine));
                else if (b.Barcode is Barcode2of5Matrix)
                    WriteLn(Stream, String.Format("^BJ,,{0},N", printLine));
                else if (b.Barcode is Barcode39Extended)
                    WriteLn(Stream, String.Format("^B3,Y,,{0},N", printLine));
                else if (b.Barcode is Barcode39)
                    WriteLn(Stream, String.Format("^B3,N,,{0},N", printLine));
                else if (b.Barcode is Barcode93Extended)
                    WriteLn(Stream, String.Format("^BA,,{0},N,N", printLine));
                else if (b.Barcode is Barcode93)
                    WriteLn(Stream, String.Format("^BA,,{0},N,N", printLine));
                else if (b.Barcode is BarcodeCodabar)
                    WriteLn(Stream, String.Format("^BK,N,{0},N,,", printLine));
                else if (b.Barcode is BarcodeUPC_A)
                    WriteLn(Stream, String.Format("^BU,,{0},N,Y", printLine));
                else if (b.Barcode is BarcodeUPC_E0)
                    WriteLn(Stream, String.Format("^B9,,{0},N,Y", printLine));
                else if (b.Barcode is BarcodeUPC_E1)
                    WriteLn(Stream, String.Format("^B9,,{0},N,Y", printLine));
                else if (b.Barcode is BarcodeEAN8)
                    WriteLn(Stream, String.Format("^B8,,{0},N", printLine));
                else if (b.Barcode is BarcodeEAN13)
                    WriteLn(Stream, String.Format("^BE,,{0},N", printLine));
                else if (b.Barcode is BarcodeIntelligentMail)
                    WriteLn(Stream, String.Format("^BZ,,{0},N,3", printLine));
                else if (b.Barcode is BarcodeMSI)
                    WriteLn(Stream, String.Format("^BM,,,{0},N,N", printLine));
                else if (b.Barcode is BarcodePDF417)
                    WriteLn(Stream, "^B7,,,,,");
                else if (b.Barcode is BarcodePlessey)
                    WriteLn(Stream, String.Format("^BP,,{0},N", printLine));
                else if (b.Barcode is BarcodePostNet)
                    WriteLn(Stream, String.Format("^BZ,,{0},N,0", printLine));
                else if (b.Barcode is BarcodeAztec)
                    WriteLn(Stream, String.Format("^BO,{0},,,,,",
                        zplScaleArray[scaleIndex].TwoDCodeScale.ToString()));
                else if (b.Barcode is BarcodeDatamatrix)
                    WriteLn(Stream, "^BX,,,,,,");
                else if (b.Barcode is BarcodeMaxiCode)
                {
                    BarcodeMaxiCode maxiCode = b.Barcode as BarcodeMaxiCode;
                    WriteLn(Stream, String.Format("^BD{0},,", maxiCode.Mode.ToString()));
                }
                WriteLn(Stream, "^FD" + GetZPLText(b.Text) + "^FS");
                // to-do BarcodeSupplement2, BarcodeSupplement5, BarcodePharmacode - didn't match
            }
            else // QR code
            {
                WriteLn(Stream, "^BY2,2,0");
                WriteLn(Stream, String.Format("^BQ,2,{0}",
                    Math.Round((float)zplScaleArray[scaleIndex].TwoDCodeScale * b.Zoom).ToString()));
                WriteLn(Stream, "^FDMA," + GetZPLText(b.Text) + "^FS");
            }
        }

        /// <summary>
        /// Gets the position of object in ZPL code.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        private string SetPosition(int left, int top)
        {
            return String.Format("^FO{0},{1}", left, top);
        }

        /// <summary>
        /// Gets the text attributes in ZPL code.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="lines"></param>
        /// <param name="leading"></param>
        /// <param name="horizAlign"></param>
        /// <param name="gap"></param>
        /// <returns></returns>
        private string SetTextAttributes(int width, int lines, int leading, HorzAlign horizAlign, int gap)
        {
            return String.Format("^FB{0},{1},{2},{3},{4}",
                width,
                lines,
                leading,
                GetHorizAlign(horizAlign),
                gap
                );
        }

        /// <summary>
        /// Gets the text with font width and height in ZPL code.
        /// </summary>
        /// <param name="fontHeight"></param>
        /// <param name="fontWidth"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private string DrawText(int fontHeight, int fontWidth, string text)
        {
            return String.Format("^A{0},{1},{2}^FD{3}^FS",
                font,
                fontHeight,
                fontWidth,
                GetZPLText(text)
                );
        }

        /// <summary>
        /// Gets the horiz align in ZPL code.
        /// </summary>
        /// <param name="horizAlign"></param>
        /// <returns></returns>
        private string GetHorizAlign(HorzAlign horizAlign)
        {
            switch (horizAlign)
            {
                case HorzAlign.Left:
                    return "L";

                case HorzAlign.Center:
                    return "C";

                case HorzAlign.Right:
                    return "R";

                default:
                    return "J";
            }
        }

        /// <summary>
        /// Gets the rectangle in ZPL code.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="lineWidth"></param>
        /// <returns></returns>
        private string DrawRectangle(int width, int height, int lineWidth)
        {
            return String.Format("^GB{0},{1},{2}^FS",
                width,
                height,
                lineWidth);
        }

        private string DrawEllipse(int width, int height, int lineWidth)
        {
            return String.Format("^GE{0},{1},{2}^FS",
                width,
                height,
                lineWidth);
        }

        private string DrawLine(int width, int height, int lineWidth, bool direction)
        {
            return String.Format("^GD{0},{1},{2},,{3}^FS",
                width.ToString(),
                height.ToString(),
                lineWidth.ToString(),
                direction ? "L" : "R");
        }

        #endregion Private Methods

        #region Protected Methods

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            // Init of scale index.
            switch (density)
            {
                case ZplDensity.d6_dpmm_152_dpi:
                    scaleIndex = 0;
                    break;

                case ZplDensity.d8_dpmm_203_dpi:
                    scaleIndex = 1;
                    break;

                case ZplDensity.d12_dpmm_300_dpi:
                    scaleIndex = 2;
                    break;

                case ZplDensity.d24_dpmm_600_dpi:
                    scaleIndex = 3;
                    break;
            }
            WriteLn(Stream, printerInit);
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            WriteLn(Stream, printerFinish);
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("ZplFile");
        }

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            WriteLn(Stream, String.Format("^XA{0}", codePage));
            WriteLn(Stream, pageInit);
            leftMargin = page.LeftMargin * millimeters * zplScaleArray[scaleIndex].PageScale;
            topMargin = page.TopMargin * millimeters * zplScaleArray[scaleIndex].PageScale;
            if (printAsBitmap)
            {
                pageBitmap = new Bitmap(
                    (int)Math.Round(ExportUtils.GetPageWidth(page) * millimeters * zplScaleArray[scaleIndex].PageScale),
                    (int)Math.Round(ExportUtils.GetPageHeight(page) * millimeters * zplScaleArray[scaleIndex].PageScale),
                    PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(pageBitmap))
                    g.Clear(Color.White);
            }
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            if (printAsBitmap)
            {
                WriteLn(Stream, SetPosition((int)leftMargin, (int)topMargin));
                WriteLn(Stream, DrawBWPicture(pageBitmap));
                pageBitmap.Dispose();
            }
            base.ExportPageEnd(page);
            WriteLn(Stream, "^XZ");
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            if (band.Parent == null)
                return;

            if (printAsBitmap)
                DrawObjectOnBitmap(pageBitmap, band as ReportComponentBase, true);

            foreach (Base c in band.ForEachAllConvectedObjects(this))
            {
                if (c is ReportComponentBase && (c as ReportComponentBase).Exportable)
                {
                    ReportComponentBase bandObject = c as ReportComponentBase;
                    if (printAsBitmap)
                        DrawObjectOnBitmap(pageBitmap, bandObject, true);
                    else
                    {
                        if (bandObject is CellularTextObject)
                            bandObject = (bandObject as CellularTextObject).GetTable();
                        if (bandObject is TableCell)
                            continue;
                        else if (bandObject is TableBase)
                            ExportTableObject(bandObject as TableBase);
                        else if (bandObject is TextObject)
                            ExportTextObject(bandObject as TextObject);
                        else if (bandObject is ShapeObject)
                            ExportShapeObject(bandObject as ShapeObject);
                        else if (bandObject is LineObject)
                            ExportLineObject(bandObject as LineObject);
                        else if (bandObject is BarcodeObject)
                            ExportBarcodeObject(bandObject as BarcodeObject);
                        else
                            ExportPictureObject(bandObject);
                    }
                }
            }
        }

        #endregion Protected Methods

        #region Public Methods

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);

            // Options
            writer.WriteValue("Density", Density);
            writer.WriteBool("PrintAsBitmap", PrintAsBitmap);
            writer.WriteValue("FontScale", FontScale);
            // end
        }

        #endregion Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ZplExport"/> class.
        /// </summary>
        public ZplExport()
        {
            countTable = new Dictionary<int, char>();
            countTable.Add(1, 'G');
            countTable.Add(2, 'H');
            countTable.Add(3, 'I');
            countTable.Add(4, 'J');
            countTable.Add(5, 'K');
            countTable.Add(6, 'L');
            countTable.Add(7, 'M');
            countTable.Add(8, 'N');
            countTable.Add(9, 'O');
            countTable.Add(10, 'P');
            countTable.Add(11, 'Q');
            countTable.Add(12, 'R');
            countTable.Add(13, 'S');
            countTable.Add(14, 'T');
            countTable.Add(15, 'U');
            countTable.Add(16, 'V');
            countTable.Add(17, 'W');
            countTable.Add(18, 'X');
            countTable.Add(19, 'Y');
            countTable.Add(20, 'g');
            countTable.Add(40, 'h');
            countTable.Add(60, 'i');
            countTable.Add(80, 'j');
            countTable.Add(100, 'k');
            countTable.Add(120, 'l');
            countTable.Add(140, 'm');
            countTable.Add(160, 'n');
            countTable.Add(180, 'o');
            countTable.Add(200, 'p');
            countTable.Add(220, 'q');
            countTable.Add(240, 'r');
            countTable.Add(260, 's');
            countTable.Add(280, 't');
            countTable.Add(300, 'u');
            countTable.Add(320, 'v');
            countTable.Add(340, 'w');
            countTable.Add(360, 'x');
            countTable.Add(380, 'y');
            countTable.Add(400, 'z');
            // prepare for indexed access to keys
            counts = new int[countTable.Count];
            countTable.Keys.CopyTo(counts, 0);
        }
    }
}