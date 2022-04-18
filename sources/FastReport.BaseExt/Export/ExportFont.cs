#define USE_SYSTEM_COMPONENT_MODEL
//#define WITHOUT_UNISCRIBE

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FastReport.Utils;
using System.Drawing.Drawing2D;
using FastReport.Fonts;

#if USE_SYSTEM_COMPONENT_MODEL
using System.ComponentModel;
#endif

namespace FastReport.Export.TTF
{
    /// <summary>
    /// Specifies the export font class.
    /// </summary>
    internal class ExportTTFFont : IDisposable
    {

        #region DLL import !WITHOUT_UNISCRIBE
#if !WITHOUT_UNISCRIBE
        [DllImport("Gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("Gdi32.dll")]
        private static extern IntPtr DeleteObject(IntPtr hgdiobj);
        [DllImport("Gdi32.dll")]
        private static extern int GetOutlineTextMetrics(IntPtr hdc, int cbData, ref TrueTypeFont.OutlineTextMetric lpOTM);
        [DllImport("Gdi32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetGlyphIndices(IntPtr hdc, string lpstr, int c, [In, Out] ushort[] pgi, uint fl);
        [DllImport("Gdi32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetFontData(IntPtr hdc, uint dwTable, uint dwOffset, [In, Out] byte[] lpvBuffer, uint cbData);
        [DllImport("Gdi32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetFontData(IntPtr hdc, uint dwTable, uint dwOffset, [In, Out] IntPtr lpvBuffer, uint cbData);

        [DllImport("usp10.dll")]
        private static extern int ScriptFreeCache(ref IntPtr psc);
        [DllImport("usp10.dll")]
        private static extern int ScriptItemize(
            [MarshalAs(UnmanagedType.LPWStr)] string pwcInChars, int cInChars, int cMaxItems,
            ref SCRIPT_CONTROL psControl, ref SCRIPT_STATE psState, [In, Out] SCRIPT_ITEM[] pItems, ref int pcItems);
        [DllImport("usp10.dll")]
        private static extern int ScriptLayout(
            int cRuns, [MarshalAs(UnmanagedType.LPArray)] byte[] pbLevel,
            [MarshalAs(UnmanagedType.LPArray)] int[] piVisualToLogical,
            [MarshalAs(UnmanagedType.LPArray)] int[] piLogicalToVisual);
        [DllImport("usp10.dll")]
        private static extern int ScriptShape(
            IntPtr hdc, ref IntPtr psc, [MarshalAs(UnmanagedType.LPWStr)] string pwcChars,
            int cChars, int cMaxGlyphs, ref SCRIPT_ANALYSIS psa,
            [Out, MarshalAs(UnmanagedType.LPArray)] ushort[] pwOutGlyphs,
            [Out, MarshalAs(UnmanagedType.LPArray)] ushort[] pwLogClust,
            [Out, MarshalAs(UnmanagedType.LPArray)] SCRIPT_VISATTR[] psva, ref int pcGlyphs);
        [DllImport("usp10.dll")]
        private static extern int ScriptPlace(
            IntPtr hdc, ref IntPtr psc, [MarshalAs(UnmanagedType.LPArray)] ushort[] pwGlyphs,
            int cGlyphs, [MarshalAs(UnmanagedType.LPArray)] SCRIPT_VISATTR[] psva,
            ref SCRIPT_ANALYSIS psa, [MarshalAs(UnmanagedType.LPArray)] int[] piAdvance,
            [Out, MarshalAs(UnmanagedType.LPArray)] GOFFSET[] pGoffset, ref ABC pABC);
        [DllImport("usp10.dll")]
        private static extern int ScriptJustify(
            [MarshalAs(UnmanagedType.LPArray)] SCRIPT_VISATTR[] psva,
            [MarshalAs(UnmanagedType.LPArray)] int[] piAdvance,
            int cGlyphs, int iDx, int iMinKashida,
            [Out, MarshalAs(UnmanagedType.LPArray)] int[] piJustify);
        [DllImport("usp10.dll")]
        private static extern uint ScriptRecordDigitSubstitution(uint lcid, ref SCRIPT_DIGITSUBSTITUTE psds);
        [DllImport("usp10.dll")]
        private static extern int ScriptApplyDigitSubstitution(
            ref SCRIPT_DIGITSUBSTITUTE psds, ref SCRIPT_CONTROL psc, ref SCRIPT_STATE pss);
#endif
        #endregion


        /// <summary>
        /// These fonts not support Bold or Itailc styles
        /// </summary>
        static string[] NeedStyelSimulationList = new string[7]
        {
            "\uff2d\uff33 \u30b4\u30b7\u30c3\u30af",
            "\uff2d\uff33 \uff30\u30b4\u30b7\u30c3\u30af",
            "\uff2d\uff33 \u660e\u671d",
            "\uff2d\uff33 \uff30\u660e\u671d",
            "MS Gothic",
            "MS UI Gothic",
            "Arial Black"
        };
        static TrueTypeCollection font_collection = new TrueTypeCollection();

        #region Font Structures
        /// <summary>
        /// Description of SCRIPT_STATE structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_STATE
        {
            /// <summary>
            /// data
            /// </summary>
            public short data;
            /// <summary>
            /// uBidiLevel
            /// </summary>
            public int uBidiLevel
            {
                get { return data & 0x001F; }
            }
            /// <summary>
            /// SetRtl
            /// </summary>
            public void SetRtl()
            {
                data = 0x801;
            }
        }

        /// <summary>
        /// Description of SCRIPT_ANALYSIS structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_ANALYSIS
        {
            /// <summary>
            /// data
            /// </summary>
            public short data;
            /// <summary>
            /// state
            /// </summary>
            public SCRIPT_STATE state;
        }

        /// <summary>
        /// Description of SCRIPT_CONTROL structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_CONTROL
        {
            /// <summary>
            /// data
            /// </summary>
            public int data;
        }

        /// <summary>
        /// Description of SCRIPT_DIGITSUBSTITUTE structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_DIGITSUBSTITUTE
        {
            /// <summary>
            /// NationalDigitLanguage
            /// </summary>
            public short NationalDigitLanguage;
            /// <summary>
            /// TraditionalDigitLanguage
            /// </summary>
            public short TraditionalDigitLanguage;
            /// <summary>
            /// DigitSubstitute
            /// </summary>
            public byte DigitSubstitute;
            /// <summary>
            /// dwReserved
            /// </summary>
            public int dwReserved;
        }

        /// <summary>
        /// Description of SCRIPT_ITEM structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_ITEM
        {
            /// <summary>
            /// iCharPos
            /// </summary>
            public int iCharPos;
            /// <summary>
            /// analysis
            /// </summary>
            public SCRIPT_ANALYSIS analysis;
        }

        /// <summary>
        /// Description of SCRIPT_VISATTR structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SCRIPT_VISATTR
        {
            // WORD uJustification : 4;
            // WORD fClusterStart : 1;
            // WORD fDiacritic : 1;
            // WORD fZeroWidth : 1;
            // WORD fReserved : 1;
            // WORD fShapeReserved : 8;

            /// <summary>
            /// data
            /// </summary>
            public ushort data;

            public ushort uJustification
            {
                get
                {
                    return (ushort)(data & 15u);
                }
            }

            public bool fClusterStart
            {
                get
                {
                    return (data & 16u) == 16u;
                }
            }

            public bool fDiacritic
            {
                get
                {
                    return (data & 32u) == 32u;
                }
            }

            public bool fZeroWidth
            {
                get
                {
                    return (data & 64u) == 64u;
                }
            }

            public bool fReserved
            {
                get
                {
                    return (data & 128u) == 128u;
                }

            }

            public byte fShapeReserved
            {
                get
                {
                    return (byte)((data & 65280u) / 256);
                }
            }

            public override string ToString()
            {
                return String.Format(
                    "uJustification : {0};\n" +
                    "fClusterStart : {1};\n" +
                    "fDiacritic : {2};\n" +
                    "fZeroWidth : {3};\n" +
                    "fReserved : {4};\n" +
                    "fShapeReserved: {5};",
                    uJustification, fClusterStart, fDiacritic, fZeroWidth, fReserved, fShapeReserved);
            }
        }

        /// <summary>
        /// Description of GOFFSET structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct GOFFSET
        {
            /// <summary>
            /// du
            /// </summary>
            public int du;
            /// <summary>
            /// dv
            /// </summary>
            public int dv;
        }

        /// <summary>
        /// Description of ABC structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ABC
        {
            /// <summary>
            /// abcA
            /// </summary>
            public int abcA;
            /// <summary>
            /// abcB
            /// </summary>
            public int abcB;
            /// <summary>
            /// abcC
            /// </summary>
            public int abcC;
        }

        /// <summary>
        /// Description of FontPanose structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FontPanose
        {
            public byte bFamilyType;
            public byte bSerifStyle;
            public byte bWeight;
            public byte bProportion;
            public byte bContrast;
            public byte bStrokeVariation;
            public byte ArmStyle;
            public byte bLetterform;
            public byte bMidline;
            public byte bXHeight;
        }

        /// <summary>
        /// Description of FontRect structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FontRect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        /// <summary>
        /// Description of FontPoint structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FontPoint
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// Description of FontTextMetric structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FontTextMetric
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public char tmFirstChar;
            public char tmLastChar;
            public char tmDefaultChar;
            public char tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;
        }


        #endregion

        #region Private variables
        private Bitmap tempBitmap;
        private IntPtr uSCache;
        private SCRIPT_DIGITSUBSTITUTE digitSubstitute;
        private float dpiFX;

        private Dictionary<ushort, GlyphChar> usedGlyphChars;
        private TrueTypeFont.OutlineTextMetric textMetric;
        private string name;
        private Font sourceFont;
        private float baseSize;
        private long reference;
        private bool saved;
        private bool simulateBold;
        private bool simulateItalic;
        private bool editable;
        private bool repack_I2L = false;

        #endregion

        #region Public fields

        public IEnumerable<GlyphChar> UsedGlyphChars
        {
            get { return usedGlyphChars.Values; }
        }

        public int UsedGlyphCharsCount
        {
            get
            {
                return usedGlyphChars.Count;
            }
        }

        /// <summary>
        /// Return text metric structure, need to use after FillOutlineTextMetrix()
        /// </summary>
        public TrueTypeFont.OutlineTextMetric TextMetric
        {
            get { return textMetric; }
        }

        /// <summary>
        /// Gets or sets internal font name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Return source font used in constructor
        /// </summary>
        public Font SourceFont
        {
            get { return sourceFont; }
        }

        /// <summary>
        /// Returns multiplier for stroke bold emulation
        /// </summary>
        public double SourceFontBoldMultiplier
        {
            get
            {
                return Math.E / (2 * Math.PI) * baseSize / 9;
            }
        }

        /// <summary>
        /// Gets or sets internal reference
        /// </summary>
        public long Reference
        {
            get { return reference; }
            set { reference = value; }
        }

        /// <summary>
        /// Gets or sets internal property - save flag 
        /// </summary>
        public bool Saved
        {
            get { return saved; }
            set { saved = value; }
        }

        /// <summary>
        /// True if bold style is not supported by font
        /// </summary>
        public bool NeedSimulateBold
        {
            get { return simulateBold; }
            set { simulateBold = value; }
        }

        /// <summary>
        /// True if italic style is not supported by font
        /// </summary>
        public bool NeedSimulateItalic
        {
            get { return simulateItalic; }
            set { simulateItalic = value; }
        }

        /// <summary>
        /// Mark font as editable for InteractiveForms
        /// </summary>
        public bool Editable
        {
            get { return editable; }
            set { editable = value; }
        }
        #endregion 

        #region Public Methods
        /// <summary>
        /// Run fill outline text metric structure
        /// </summary>
        public bool FillOutlineTextMetrix()
        {
            bool Done = false;
            if (sourceFont != null)
            {
#if ! WITHOUT_UNISCRIBE
                using (Graphics g = Graphics.FromImage(tempBitmap))
                {
                    IntPtr hdc = g.GetHdc();
                    IntPtr f = sourceFont.ToHfont();
                    try
                    {
                        SelectObject(hdc, f);
                        GetOutlineTextMetrics(hdc, Marshal.SizeOf(typeof(TrueTypeFont.OutlineTextMetric)), ref textMetric);
                    }
                    finally
                    {
                        DeleteObject(f);
                        g.ReleaseHdc(hdc);
                    }
                }
#else
                TrueTypeFont font = Config.FontCollection[sourceFont];
                if (font != null)
                {
                    font.GetOutlineTextMetrics(ref textMetric);
                    Done = true;
                }
#endif
            }
            return Done;
        }

        /// <summary>
        /// Return glyph width
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int GetGlyphWidth(char c)
        {
            return (int)usedGlyphChars[c].Width;
        }
#if !WITHOUT_UNISCRIBE
        private void GDI32_GetFontData(Font font, out IntPtr font_data, out FontType CollectionMode, out uint fontDataSize)
        {
            font_data = IntPtr.Zero;
            Bitmap tempBitmap = new Bitmap(1, 1);

            using (Graphics g = Graphics.FromImage(tempBitmap))
            {
                IntPtr hdc = g.GetHdc();
                IntPtr f = font.ToHfont();
                SelectObject(hdc, f);

                try
                {
                    // Try to read TrueTypeCollection
                    CollectionMode = FontType.TrueTypeCollection;
                    fontDataSize = GetFontData(hdc, (uint)CollectionMode, 0, IntPtr.Zero, 0);
                    if (fontDataSize == uint.MaxValue)
                    {
                        CollectionMode = FontType.TrueTypeFont;
                        fontDataSize = GetFontData(hdc, (uint)CollectionMode, 0, IntPtr.Zero, 0);
                    }
                    font_data = Marshal.AllocHGlobal((int)fontDataSize);
                    GetFontData(hdc, (uint)CollectionMode, 0, font_data, fontDataSize);
                }
                finally
                {
                    DeleteObject(f);
                    g.ReleaseHdc(hdc);
                    tempBitmap.Dispose();
                }
            }
        }


        private TrueTypeFont GetTrueTypeFont(Font source_font)
        {
            TrueTypeFont ttfont = null;

            string fast_font =
                source_font.FontFamily.Name +
                (source_font.Bold ? "-B" : string.Empty) +
                (source_font.Italic ? "-I" : string.Empty);

            if (!font_collection.Collection.ContainsKey(fast_font))
            {
                IntPtr font_image;
                FontType mode;
                uint size;
                GDI32_GetFontData(sourceFont, out font_image, out mode, out size);
                foreach (TrueTypeFont ttf in TrueTypeCollection.AddFontData((FontType)mode, font_image))
                {
                    if (!ttf.IsClassicFormat && !ttf.IsCollection)
                        ttf.FileSize = size;
                    TrueTypeCollection.ParseFont(true, ttf, "");
                    ttfont = ttf as TrueTypeFont;
                    break;
                }
            }
            else
                ttfont = font_collection[fast_font];

            return ttfont;
        }
#endif

        /// <summary>
        /// Return font file
        /// </summary>
        /// <returns></returns>
        public byte[] GetFontData(bool subsetting)
        {
            byte[] result = null;
            if (sourceFont != null)
            {
#if !WITHOUT_UNISCRIBE
                TrueTypeFont font = GetTrueTypeFont(sourceFont);
#else
                TrueTypeFont font = Config.FontCollection[sourceFont];
#endif
                if (font != null)
                {
                    int LicensingRights = font.LicensingRights;

#if USE_SYSTEM_COMPONENT_MODEL
                    if ((LicensingRights & 0x000f) == 2)
                        throw new System.ComponentModel.LicenseException(typeof(TrueTypeFont), this, 
                            String.Format(new MyRes("Fonts").Get("FontMustNotBeModifiedEmbeddedOrExchanged"), font.Name));
                    if ((LicensingRights & 0x0200) == 0x0200)
                        throw new System.ComponentModel.LicenseException(typeof(TrueTypeFont), this,
                            String.Format("Font {0}: Bitmap embedding only", font.Name));
#else
                    if ((LicensingRights & 0x000f) == 2)
                        throw new Exception(String.Format(new MyRes("Fonts").Get("FontMustNotBeModifiedEmbeddedOrExchanged"), font.Name));
                    if ((LicensingRights & 0x0200) == 0x0200)
                        throw new Exception(String.Format("Font {0}:  Bitmap embedding only". ", font.Name));
#endif
                    if (subsetting)
                    {
                        if ((LicensingRights & 0x0100) == 0x0100)
                        {

#if USE_SYSTEM_COMPONENT_MODEL
                            throw new System.ComponentModel.LicenseException(typeof(TrueTypeFont), this,
                                String.Format(new MyRes("Fonts").Get("FontMayNotBeSubsettedPriorToEmbedding"), font.Name));
#else
                            throw new Exception(String.Format(new MyRes("Fonts").Get("FontMayNotBeSubsettedPriorToEmbedding"), font.Name));
#endif
                        }
                        font.PrepareIndexes();
                        if (font.IsClassicFormat)
                        {
                            TrueTypeFont.FontPackOptions options = TrueTypeFont.FontPackOptions.Defatult_Options;
                            if (this.repack_I2L)
                                options = TrueTypeFont.FontPackOptions.Pack_Indexes;
                            result = font.PackFont(ref usedGlyphChars, options);
                        }
                        else if (font.Type == FontType.OpenTypeFont)
                        {
                            // TODO: Pack Compact Font Format 
                            result = font.GetFontData();
                        }
                        else
                        {
                            // Generate warning and load full 
                            result = font.GetRawFontData();
                        }
                    }
                    else
                        result = font.GetRawFontData();

#region Style simulation here
                    if (sourceFont.Bold != font.Bold)
                    {
                        simulateBold = true;
                    }
                    if (sourceFont.Italic != font.Italic)
                    {
                        simulateItalic = true;
                    }
#endregion

                }
            }
            return result;
        }

        /// <summary>
        /// Get font data and set NeedSimulateBold and NeedSimulateItalic properties. Call this method after FillOutlineTextMetrix
        /// </summary>
        public void FillEmulate()
        {
#if !WITHOUT_UNISCRIBE
            TrueTypeFont ttf = GetTrueTypeFont(sourceFont);
#else
            TrueTypeFont ttf = Config.FontCollection[sourceFont];
#endif
            NeedSimulateBold = SourceFont.Bold && !ttf.Bold;
            NeedSimulateItalic = SourceFont.Italic && !ttf.Italic;
        }

        /// <summary>
        /// Remap str in glyph indexes. Return string with glyph indexes.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="rtl"></param>
        /// <returns></returns>
        public string RemapString(string str, bool rtl)
        {
            // control chars should not be here...
            str = str.Replace("\r", "").Replace("\n", "").Replace((char)0xa0, ' ');
            int maxGlyphs = str.Length * 3;
            GlyphChar[] glyphChars = new GlyphChar[maxGlyphs];
            int actualLength = 0;

#if !WITHOUT_UNISCRIBE
            using (Graphics g = Graphics.FromImage(tempBitmap))
            {
                IntPtr hdc = g.GetHdc();
                IntPtr f = sourceFont.ToHfont();
                try
                {
                    SelectObject(hdc, f);
                    ABC abc;
                    actualLength = GetGlyphIndices(hdc, str, glyphChars, rtl, out abc);
                }
                finally
                {
                    DeleteObject(f);
                    g.ReleaseHdc(hdc);
                }
            }
            FastString sb = new FastString(actualLength);
            for (int i = 0; i < actualLength; i++)
            {
                if (glyphChars[i] != null)
                {
                    ushort c = glyphChars[i].Glyph;
                    if (!usedGlyphChars.ContainsKey(c) || !usedGlyphChars.ContainsValue(glyphChars[i]))
                    {
                        int element_count = usedGlyphChars.Count;
                        usedGlyphChars[c] = glyphChars[i];
                        if(this.repack_I2L)
                        {
                            usedGlyphChars[c].Glyph = (char)element_count;
                            usedGlyphChars[c].GlyphType = GlyphType.Index;
                        }
                        sb.Append((char)usedGlyphChars[c].Glyph);
                    }
                    else
                        sb.Append((char)usedGlyphChars[c].Glyph);
                }
            }
            return sb.ToString();
#else
            ushort[] glyphs = new ushort[maxGlyphs];
            float[] widths = new float[maxGlyphs];
            TrueTypeFont font = Config.FontCollection[sourceFont];
            if (rtl)
                font.Script = "arab";
            actualLength = font.GetGlyphIndices(str, sourceFont.Size / 0.75f, out glyphs, out widths, rtl);
            StringBuilder sb = new StringBuilder(actualLength);
            for (int i = 0; i < actualLength; i++)
            {
                ushort c = glyphs[i];
                if (!usedGlyphChars.ContainsKey(c))
                {
                    char result;
                    if (actualLength > str.Length) // ligatures found - skip entire string
                        result = textMetric.otmTextMetrics.tmDefaultChar;
                    else
                        result = str[rtl ? actualLength - i - 1 : i];
                    usedGlyphChars[c] = new GlyphChar(c, widths[i], result);
                }
                sb.Append((char)c);
            }
            return sb.ToString();
#endif
        }

        /// <summary>
        /// Set pattern for generation of alphabet's subset
        /// </summary>
        /// <param name="pattern">Regular expression with pattern generator</param>
        /// <param name="rtl">Use left-to-right rules</param>
        /// <returns></returns>
        public string SetPattern(string pattern, bool rtl)
        {
            // Insert pattern extractor here
            RegexPatternExtractor regex = new RegexPatternExtractor();
            regex.AddExpression(pattern);
            return RemapString(regex.Pattern, rtl);
        }

        public class GlyphTTF
        {
            public GraphicsPath path;
            public float width;

            public float minX;
            public float minY;
            public float maxX;
            public float maxY;
            public float baseLine;

            public GlyphTTF(GraphicsPath aPath, float aWidth)
            {
                path = aPath;
                width = aWidth;
            }

            public GlyphTTF(GraphicsPath aPath, float aWidth, float minX, float minY, float maxX, float maxY) : this(aPath, aWidth)
            {
                this.minX = minX;
                this.minY = minY;
                this.maxX = maxX;
                this.maxY = maxY;
            }

            public GlyphTTF(GraphicsPath aPath, float aWidth, float minX, float minY, float maxX, float maxY, float v) : this(aPath, aWidth, minX, minY, maxX, maxY)
            {
                this.baseLine = v;
            }

            public GlyphTTF(GraphicsPath aPath, float aWidth, float v) : this(aPath, aWidth)
            {
                this.baseLine = v;
            }
        }

        public GlyphTTF[] getGlyphString(string str, bool rtl, float size, out float paddingX, out float paddingY)
        {
            // control chars should not be here...
            str = str.Replace("\r", "").Replace("\n", "");
            int maxGlyphs = str.Length * 3;
            GlyphChar[] glyphChars = new GlyphChar[maxGlyphs];

            int actualLength = 0;
            paddingX = 0;
            paddingY = 0;
            ABC abc;
#if !WITHOUT_UNISCRIBE
            using (Graphics g = Graphics.FromImage(tempBitmap))
            {
                IntPtr hdc = g.GetHdc();
                IntPtr f = sourceFont.ToHfont();
                try
                {
                    SelectObject(hdc, f);
                    actualLength = GetGlyphIndices(hdc, str, glyphChars, rtl, out abc);
                }
                finally
                {
                    DeleteObject(f);
                    g.ReleaseHdc(hdc);
                }
            }
#else
            ushort[] glyphs = new ushort[maxGlyphs];
            float[] widths = new float[maxGlyphs];
            TrueTypeFont font = Config.FontCollection[sourceFont];
            actualLength = font.GetGlyphIndices(str, sourceFont.Size / 0.75f, out glyphs, out widths, rtl);
#endif

            //StringBuilder sb = new StringBuilder(actualLength);
            List<GlyphTTF> aList = new List<GlyphTTF>();
            for (int i = 0; i < actualLength; i++)
            {
#if !WITHOUT_UNISCRIBE
                if (glyphChars[i] != null)
                {
                    ushort c = glyphChars[i].Glyph;
                    float width = glyphChars[i].Width;
                    if (!usedGlyphChars.ContainsKey(c))
                        usedGlyphChars[c] = glyphChars[i];

                    TrueTypeFont ttfont = GetTrueTypeFont(sourceFont);
                    //float minX, minY, maxX, maxY;
                    FastGraphicsPath aPath = ttfont.GetGlyph(c, size);
                    GraphicsPath path;
                    if (aPath.PointCount != 0)
                    {
                        path = new GraphicsPath(aPath.PathPoints, aPath.PathTypes, FillMode.Winding);
                    }
                    else
                    {
                        path = new GraphicsPath();
                    }

                    aList.Add(new GlyphTTF(
                                    path,
                                    width / sourceFont.Size * size,
                                    ttfont.baseLine * size / 0.75f));
                }
#else
                ushort c = glyphs[i];
                if (!usedGlyphChars.ContainsKey(c))
                {
                    char result;
                    if (actualLength > str.Length) // ligatures found - skip entire string
                        result = textMetric.otmTextMetrics.tmDefaultChar;
                    else
                        result = str[(rtl ? actualLength - i - 1 : i)];
                    usedGlyphChars[c] = new GlyphChar(c, widths[i], result);
                }
                //float minX, minY, maxX, maxY;
                FastGraphicsPath aPath = font.GetGlyph(c, size);
                //if (c == 3) aPath = new GraphicsPath();//space SPACE SPACE!!!! what?
                GraphicsPath gp;
                if (aPath.PathPoints.Length != 0)
                {
                    gp = new GraphicsPath(aPath.PathPoints, aPath.PathTypes, FillMode.Winding);
                }
                else
                {
                    gp = new GraphicsPath();
                }
                aList.Add(new GlyphTTF(
                    gp,
                    widths[i] / sourceFont.Size * size,
                    font.baseLine * size / 0.75f));
#endif
            }
#if !WITHOUT_UNISCRIBE
            paddingX = abc.abcA / sourceFont.Size * size;
            paddingY = abc.abcC / sourceFont.Size * size;
#else
            paddingX = 0; // abc.abcA / FSourceFont.Size * size;
            paddingY = 0; // abc.abcC / FSourceFont.Size * size;
#endif
            return aList.ToArray();
        }

        /// <summary>
        /// Return english name of source font
        /// </summary>
        /// <returns></returns>
        public string GetEnglishFontName()
        {
            // get the english name of a font
            string fontName = sourceFont.FontFamily.GetName(1033);
            FastString Result = new FastString(fontName.Length * 3);
            foreach (char c in fontName)
            {
                switch (c)
                {
                    case ' ':
                    case '%':
                    case '(':
                    case ')':
                    case '<':
                    case '>':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                    case '/':
                    case '#':
                        Result.Append("#");
                        Result.Append(((int)c).ToString("X2"));
                        break;
                    default:
                        if ((int)c > 126 || (int)c < 32)
                        {
                            Result.Append('#');
                            Result.Append(((int)c).ToString("X2"));
                        }
                        else
                            Result.Append(c);
                        break;
                }
            }
            FastString Style = new FastString(9);
            if ((sourceFont.Style & FontStyle.Bold) > 0 && !this.simulateBold)
                Style.Append("Bold");
            if ((sourceFont.Style & FontStyle.Italic) > 0 && !this.simulateItalic)
                Style.Append("Italic");
            if (Style.Length > 0)
                Result.Append(",").Append(Style.ToString());
            return Result.ToString();
        }

        /// <summary>
        /// Return PANOSE string
        /// </summary>
        /// <returns></returns>
        public string GetPANOSE()
        {
            //01 05 02 02 04 00 00 00 00 00 00 00
            FastString panose = new FastString(36);
            panose.Append("01 05 ");
            panose.Append(textMetric.otmPanoseNumber.bFamilyType.ToString("X")).Append(" ");
            panose.Append(textMetric.otmPanoseNumber.bSerifStyle.ToString("X")).Append(" ");
            panose.Append(textMetric.otmPanoseNumber.bWeight.ToString("X")).Append(" ");
            panose.Append(textMetric.otmPanoseNumber.bProportion.ToString("X")).Append(" ");
            panose.Append(textMetric.otmPanoseNumber.bContrast.ToString("X")).Append(" ");
            panose.Append(textMetric.otmPanoseNumber.bStrokeVariation.ToString("X")).Append(" ");
            panose.Append(textMetric.otmPanoseNumber.ArmStyle.ToString("X")).Append(" ");
            panose.Append(textMetric.otmPanoseNumber.bLetterform.ToString("X")).Append(" ");
            panose.Append(textMetric.otmPanoseNumber.bMidline.ToString("X")).Append(" ");
            panose.Append(textMetric.otmPanoseNumber.bXHeight.ToString("X"));
            return panose.ToString();
        }
#endregion

#region Private methods
#if !WITHOUT_UNISCRIBE

        private int GetGlyphIndices(IntPtr hdc, string text, GlyphChar[] glyphChars, bool rtl, out ABC abc)
        {
            abc = new ABC();
            if (String.IsNullOrEmpty(text))
                return 0;

            int destIndex = 0;
            int maxGlyphs = text.Length * 3;
            int maxItems = text.Length * 2;

            List<Run> runs = Itemize(text, rtl, maxItems);
            runs = Layout(runs, rtl);
            foreach (Run run in runs)
            {
                GlyphChar[] tempGlyphChars = new GlyphChar[maxGlyphs];
                int length = GetGlyphs(hdc, run, tempGlyphChars, maxGlyphs, rtl, out abc);
                Array.Copy(tempGlyphChars, 0, glyphChars, destIndex, length);
                destIndex += length;
            }

            return destIndex;
        }

        private int GetGlyphs(IntPtr hdc, Run run, GlyphChar[] glyphChars, int maxGlyphs, bool rtl, out ABC abc)
        {
            // initialize structures
            SCRIPT_ANALYSIS psa = run.analysis;
            ushort[] pwLogClust = new ushort[maxGlyphs];
            int pcGlyphs = 0;
            SCRIPT_VISATTR[] psva = new SCRIPT_VISATTR[maxGlyphs];
            GOFFSET[] pGoffset = new GOFFSET[maxGlyphs];
            ABC pABC = new ABC();
            ushort[] glyphs = new ushort[maxGlyphs];
            int[] widths = new int[maxGlyphs];
            // make glyphs
            ScriptShape(hdc, ref uSCache, run.text, run.text.Length, glyphs.Length, ref psa, glyphs, pwLogClust, psva, ref pcGlyphs);

            /*
            // If number of unicode characters less then number of glyphs
            if (run.text.Length < pcGlyphs)
            {
                // then one of character generates several glyph
                // or complex case
            } else
            // If number of unicode characters large then number of glyphs
            if( run.text.Length > pcGlyphs)
            {
                // then several of character generates one glyph
                // or complex case
            } else
            // If number of unicode characters equals number of glyphs
            {
                // then one character equals one glyphs
                // or complex case
            }
            */

            // make widths
            ScriptPlace(hdc, ref uSCache, glyphs, pcGlyphs, psva, ref psa, widths, pGoffset, ref pABC);
            abc = pABC;

            try
            {
                int index = 0;
                if (rtl)
                    index = run.text.Length - 1;
                for (int i = 0; i < pcGlyphs; i++)
                {
                    if (psva[i].fClusterStart)
                    {
                        int start_index = index;
                        int count = 1;
                        if (rtl)
                        {
                            while (index - 1 >= 0 && pwLogClust[index - 1] == pwLogClust[start_index])
                            {
                                index--;
                                count++;
                            }
                        }
                        else
                        {
                            while (index + 1 < run.text.Length && pwLogClust[index + 1] == pwLogClust[start_index])
                            {
                                index++;
                                count++;
                            }
                        }
                        if (count == 1)
                        {
                            glyphChars[i] = new GlyphChar(glyphs[i], widths[i], run.text[start_index]);
                        }
                        else
                        {
                            if (rtl)
                                glyphChars[i] = new GlyphChar(glyphs[i], widths[i], run.text.Substring(start_index - count + 1, count));
                            else
                                glyphChars[i] = new GlyphChar(glyphs[i], widths[i], run.text.Substring(start_index, count));
                        }
                        if (rtl)
                        {
                            index--;
                        }
                        else
                        {
                            index++;
                        }
                    }
                    else
                    {
                        glyphChars[i] = new GlyphChar(glyphs[i], widths[i]);
                    }
                }
            }
            catch
            {
                // catch the uniscribe errors
            }


            //int[] oldW = new int[widths.Length];
            //Array.Copy(widths, oldW, widths.Length);
            //ScriptJustify(psva, oldW, pcGlyphs, 75, 0, widths);
            return pcGlyphs;
        }

        private List<Run> Itemize(string s, bool rtl, int maxItems)
        {
            SCRIPT_ITEM[] pItems = new SCRIPT_ITEM[maxItems];
            int pcItems = 0;

            // initialize Control and State
            SCRIPT_CONTROL control = new SCRIPT_CONTROL();
            SCRIPT_STATE state = new SCRIPT_STATE();
            if (rtl)
            {
                // this is needed to start paragraph from right
                state.SetRtl();
                // to substitute arabic digits
                ScriptApplyDigitSubstitution(ref digitSubstitute, ref control, ref state);
            }

            // itemize
            ScriptItemize(s, s.Length, pItems.Length, ref control, ref state, pItems, ref pcItems);

            // create Run list. Note that ScriptItemize actually returns pcItems+1 items, 
            // so this can be used to calculate char range easily
            List<Run> list = new List<Run>();
            for (int i = 0; i < pcItems; i++)
            {
                string text = s.Substring(pItems[i].iCharPos, pItems[i + 1].iCharPos - pItems[i].iCharPos);
                list.Add(new Run(text, pItems[i].analysis));
            }

            return list;
        }

        private List<Run> Layout(List<Run> runs, bool rtl)
        {
            byte[] pbLevel = new byte[runs.Count];
            int[] piVisualToLogical = new int[runs.Count];

            // build the pbLevel array
            for (int i = 0; i < runs.Count; i++)
                pbLevel[i] = (byte)runs[i].analysis.state.uBidiLevel;

            // layout runs
            ScriptLayout(runs.Count, pbLevel, piVisualToLogical, null);

            // return runs in their visual order
            List<Run> visualRuns = new List<Run>();
            for (int i = 0; i < piVisualToLogical.Length; i++)
                visualRuns.Add(runs[piVisualToLogical[i]]);

            return visualRuns;
        }
#endif
#endregion

        /// <summary>
        /// Create object of ExportTTFFont.
        /// </summary>
        /// <param name="font"></param>
        public ExportTTFFont(Font font)
        {
            dpiFX = 96f / DrawUtils.ScreenDpi;
            baseSize = font.Size;
            sourceFont = new Font(font.Name, 750 * dpiFX, font.Style);
            //FSourceFont = font;
            saved = false;
            textMetric = new TrueTypeFont.OutlineTextMetric();
            usedGlyphChars = new Dictionary<ushort, GlyphChar>();
            tempBitmap = new Bitmap(1, 1);
            uSCache = IntPtr.Zero;
            digitSubstitute = new SCRIPT_DIGITSUBSTITUTE();
            simulateItalic = false;
            simulateBold = false;
            editable = false;
            foreach (string s in NeedStyelSimulationList)
                if (s == font.Name)
                {
                    simulateBold = true;
                    simulateItalic = true;
                    break;
                }
#if !WITHOUT_UNISCRIBE
            ScriptRecordDigitSubstitution(0x0400, ref digitSubstitute);
#endif
        }

        /// <summary>
        /// Destructor
        /// </summary>
        public void Dispose()
        {
            // free cache
#if !WITHOUT_UNISCRIBE
            ScriptFreeCache(ref uSCache);
#endif
            tempBitmap.Dispose();
            sourceFont.Dispose();
        }

#region Private clases
        private class Run
        {
            public SCRIPT_ANALYSIS analysis;
            public string text;

            public Run(string text, SCRIPT_ANALYSIS a)
            {
                this.text = text;
                this.analysis = a;
            }
        }

#endregion
    }
}