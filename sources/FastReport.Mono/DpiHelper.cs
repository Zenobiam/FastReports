using System;
using System.Drawing;
using System.Windows.Forms;
namespace FastReport.MonoCap
{
    public static class DpiHelper
    {
        public static float Multiplier { get { return 1; } }
        public static bool HighDpiEnabled { get { return false; } }
        public static Point ConvertUnits(Point originalNumber, bool considerScale = false)
        {
            return originalNumber;
        }
        public static float ConvertUnits(float originalNumber, bool considerScale = false)
        {
            return originalNumber;
        }
        public static int ConvertUnits(int originalNumber, bool considerScale = false)
        {
            return originalNumber;
        }
        public static Size ConvertUnits(Size originalSize, bool considerScale = false)
        {
            return originalSize;
        }
        public static SizeF ConvertUnits(SizeF originalSize, bool considerScale = false)
        {
            return originalSize;
        }
        public static Font ConvertUnits(Font originalValue, bool considerScale = false)
        {
            return originalValue;
        }
        public static Padding ConvertUnits(Padding originalValue)
        {
            return originalValue;
        }
        public static RectangleF ConvertUnits(RectangleF originalValue)
        {
            return originalValue;
        }

        public static Bitmap ConvertBitmap(Bitmap originalBitmap)
        {
            return originalBitmap;
        }

        public static float ParseFontSize(float size)
        {
            return size;
        }

        public static Bitmap ConvertButton16(Bitmap img)
        {
            return img;
        }

        internal static Font GetFontForTextBoxHeight(int height, Font font)
        {
            return font;
        }

        internal static int GetDpiMultiplier()
        {
            return 1;
        }
    }
}
