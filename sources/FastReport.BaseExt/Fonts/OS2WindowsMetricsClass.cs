using System;
using System.Runtime.InteropServices;

#pragma warning disable CS3001, CS3002, CS3003, CS1591

namespace FastReport.Fonts
{
    /// <summary>
    /// OS/2 and Windows Metrics table 
    /// </summary>
    public class OS2WindowsMetricsClass : TrueTypeTable
    {
        #region "Type definition"

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

        /// <summary>
        /// Description of FontPanose structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
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


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct OS2WindowsMetrics
        {
            public ushort Version; // version number 0x0004
            public short xAvgCharWidth;
            public ushort usWeightClass;
            public ushort usWidthClass;
            public ushort fsType;
            public short ySubscriptXSize;
            public short ySubscriptYSize;
            public short ySubscriptXOffset;
            public short ySubscriptYOffset;
            public short ySuperscriptXSize;
            public short ySuperscriptYSize;
            public short ySuperscriptXOffset;
            public short ySuperscriptYOffset;
            public short yStrikeoutSize;
            public short yStrikeoutPosition;
            public short sFamilyClass;
#if false
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] panose; // = new byte[10];
#else
            public FontPanose panose;
#endif
            public uint ulUnicodeRange1;
            public uint ulUnicodeRange2;
            public uint ulUnicodeRange3;
            public uint ulUnicodeRange4;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] achVendID; //  = new vyte[4];
            public ushort fsSelection;
            public ushort usFirstCharIndex;
            public ushort usLastCharIndex;
            public short sTypoAscender;
            public short sTypoDescender;
            public short sTypoLineGap;
            public ushort usWinAscent;
            public ushort usWinDescent;
            public uint ulCodePageRange1;
            public uint ulCodePageRange2;
            public short sxHeight;
            public short sCapHeight;
            public ushort usDefaultChar;
            public ushort usBreakChar;
            public ushort usMaxContext;
        }
        #endregion

        private OS2WindowsMetrics win_metrix;

        private void ChangeEndian()
        {
            win_metrix.Version = SwapUInt16(win_metrix.Version);
            win_metrix.xAvgCharWidth = SwapInt16(win_metrix.xAvgCharWidth);
            win_metrix.usWeightClass = SwapUInt16(win_metrix.usWeightClass);
            win_metrix.usWidthClass = SwapUInt16(win_metrix.usWidthClass);
            win_metrix.fsType = SwapUInt16(win_metrix.fsType);
            win_metrix.ySubscriptXSize = SwapInt16(win_metrix.ySubscriptXSize);
            win_metrix.ySubscriptYSize = SwapInt16(win_metrix.ySubscriptYSize);
            win_metrix.ySubscriptXOffset = SwapInt16(win_metrix.ySubscriptXOffset);
            win_metrix.ySubscriptYOffset = SwapInt16(win_metrix.ySubscriptYOffset);
            win_metrix.ySuperscriptXSize = SwapInt16(win_metrix.ySuperscriptXSize);
            win_metrix.ySuperscriptYSize = SwapInt16(win_metrix.ySuperscriptYSize);
            win_metrix.ySuperscriptXOffset = SwapInt16(win_metrix.ySuperscriptXOffset);
            win_metrix.ySuperscriptYOffset = SwapInt16(win_metrix.ySuperscriptYOffset);
            win_metrix.yStrikeoutSize = SwapInt16(win_metrix.yStrikeoutSize);
            win_metrix.yStrikeoutPosition = SwapInt16(win_metrix.yStrikeoutPosition);
            win_metrix.sFamilyClass = SwapInt16(win_metrix.sFamilyClass);
            win_metrix.ulUnicodeRange1 = SwapUInt32(win_metrix.ulUnicodeRange1);
            win_metrix.ulUnicodeRange2 = SwapUInt32(win_metrix.ulUnicodeRange2);
            win_metrix.ulUnicodeRange3 = SwapUInt32(win_metrix.ulUnicodeRange3);
            win_metrix.ulUnicodeRange4 = SwapUInt32(win_metrix.ulUnicodeRange4);
            win_metrix.fsSelection = SwapUInt16(win_metrix.fsSelection);
            win_metrix.usFirstCharIndex = SwapUInt16(win_metrix.usFirstCharIndex);
            win_metrix.usLastCharIndex = SwapUInt16(win_metrix.usLastCharIndex);
            win_metrix.sTypoAscender = SwapInt16(win_metrix.sTypoAscender);
            win_metrix.sTypoDescender = SwapInt16(win_metrix.sTypoDescender);
            win_metrix.sTypoLineGap = SwapInt16(win_metrix.sTypoLineGap);
            win_metrix.usWinAscent = SwapUInt16(win_metrix.usWinAscent);
            win_metrix.usWinDescent = SwapUInt16(win_metrix.usWinDescent);
            win_metrix.ulCodePageRange1 = SwapUInt32(win_metrix.ulCodePageRange1);
            win_metrix.ulCodePageRange2 = SwapUInt32(win_metrix.ulCodePageRange2);
            win_metrix.sxHeight = SwapInt16(win_metrix.sxHeight);
            win_metrix.sCapHeight = SwapInt16(win_metrix.sCapHeight);
            win_metrix.usDefaultChar = SwapUInt16(win_metrix.usDefaultChar);
            win_metrix.usBreakChar = SwapUInt16(win_metrix.usBreakChar);
            win_metrix.usMaxContext = SwapUInt16(win_metrix.usMaxContext);
        }

        internal override void Load(IntPtr font)
        {
            IntPtr win_metrix_ptr = Increment(font, (int)this.Offset);
            win_metrix = (OS2WindowsMetrics)Marshal.PtrToStructure(win_metrix_ptr, typeof(OS2WindowsMetrics));
            ChangeEndian();
        }

        internal override uint Save(IntPtr font, uint offset)
        {
            this.Offset = offset;
            ChangeEndian();
            IntPtr win_metrix_ptr = Increment(font, (int)offset);
            Marshal.StructureToPtr(win_metrix, win_metrix_ptr, false);
            ChangeEndian();
            offset = (offset + this.Length + 3) & 0xfffffffc;
            return offset;
        }

        // Public properties
        public bool IsBold { get { return win_metrix.usWeightClass >= 600; } }
        public ushort Ascent { get { return win_metrix.usWinAscent; } }
        public ushort Descent { get { return win_metrix.usWinDescent; } }
        public short AvgCharWidth { get { return win_metrix.xAvgCharWidth; } }
        public ushort Weight { get { return win_metrix.usWeightClass; } }
        public ushort WidthClass { get { return win_metrix.usWidthClass; } }
        public short Height { get { return win_metrix.sxHeight; } }
        public ushort BreakChar { get { return win_metrix.usBreakChar; } }
        public ushort DefaultChar { get { return win_metrix.usDefaultChar; } }
        public ushort FirstCharIndex { get { return win_metrix.usFirstCharIndex; } }
        public ushort LastCharIndex { get { return win_metrix.usLastCharIndex; } }
        public ushort LicensingRights { get { return win_metrix.fsType; } }
        public OS2WindowsMetricsClass.FontPanose Panose { get { return win_metrix.panose; } }
        public OS2WindowsMetricsClass(TrueTypeTable src) : base(src) { }
        public uint UnicodeRange1 { get { return win_metrix.ulUnicodeRange1; } }
        public uint UnicodeRange2 { get { return win_metrix.ulUnicodeRange2; } }
        public uint UnicodeRange3 { get { return win_metrix.ulUnicodeRange3; } }
        public uint UnicodeRange4 { get { return win_metrix.ulUnicodeRange4; } }
        public short FamilyClass { get { return win_metrix.sFamilyClass; } }
        public ushort Selection { get { return win_metrix.fsSelection; } }
        public uint CodepageRange1 { get { return win_metrix.ulCodePageRange1; } }
        public uint CodepageRange2 { get { return win_metrix.ulCodePageRange2; } }
        public string VendorID
        {
          get
          {
          return "" + 
            (char) win_metrix.achVendID[0] +
            (char) win_metrix.achVendID[1] +
            (char) win_metrix.achVendID[2] +
            (char) win_metrix.achVendID[3];
          }
    }
  }

}
#pragma warning restore