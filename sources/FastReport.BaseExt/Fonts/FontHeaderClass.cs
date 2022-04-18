using System;
using System.Runtime.InteropServices;

#pragma warning disable CS3001, CS3002, CS3003, CS1591

namespace FastReport.Fonts
{
  /// <summary>
  /// FontHeader table
  /// </summary>
  public class FontHeaderClass : TrueTypeTable
  {
    #region "Type definitions"
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct FontHeader
    {
      [FieldOffset(0)]
      public uint version;     // FIXED	Table version number	0x00010000 for version 1.0.
      [FieldOffset(4)]
      public uint revision; // FIXED	fontRevision	Set by font manufacturer.
      [FieldOffset(8)]
      public uint checkSumAdjustment; // ULONG	checkSumAdjustment	To compute:  set it to 0, sum the entire font as ULONG, then store 0xB1B0AFBA - sum.
      [FieldOffset(12)]
      public uint magicNumber; // ULONG	magicNumber	Set to 0x5F0F3CF5.
      [FieldOffset(16)]
      public ushort flags; // USHORT	flags	Bit 0 - baseline for font at y=0;
                           // Bit 1 - left sidebearing at x=0;
                           // Bit 2 - instructions may depend on point size;
                           // Bit 3 - force ppem to integer values for all internal scaler math; may use fractional ppem sizes if this bit is clear;
                           // Bit 4 - instructions may alter advance width (the advance widths might not scale linearly);
                           // Note: All other bits must be zero.
      [FieldOffset(18)]
      public ushort unitsPerEm; // USHORT	unitsPerEm	Valid range is from 16 to 16384
      [FieldOffset(20)]
      public ulong CreatedDateTime; // created	International date (8-byte field).
      [FieldOffset(28)]
      public ulong ModifiedDateTime; // modified	International date (8-byte field).
      [FieldOffset(36)]
      public short xMin; //	For all glyph bounding boxes.
      [FieldOffset(38)]
      public short yMin; // For all glyph bounding boxes.
      [FieldOffset(40)]
      public short xMax; // For all glyph bounding boxes.
      [FieldOffset(42)]
      public short yMax; // For all glyph bounding boxes.
      [FieldOffset(44)]
      public ushort macStyle; // Bit 0 bold (if set to 1); Bit 1 italic (if set to 1) Bits 2-15 reserved (set to 0).
      [FieldOffset(46)]
      public ushort lowestRecPPEM; // Smallest readable size in pixels.
      [FieldOffset(48)]
      public short fontDirectionHint;
      // 0   Fully mixed directional glyphs;
      // 1   Only strongly left to right;
      // 2   Like 1 but also contains neutrals ;
      //-1   Only strongly right to left;
      //-2   Like -1 but also contains neutrals.
      [FieldOffset(50)]
      public short indexToLocFormat; //	0 for short offsets, 1 for long.
      [FieldOffset(52)]
      public short glyphDataFormat; //	0 for current format.
    }

    public enum IndexToLoc
    {
      ShortType = 0,
      LongType = 1
    }
    #endregion

    private FontHeader font_header;

    public IndexToLoc indexToLocFormat
        {
            get
            {
                return (IndexToLoc)font_header.indexToLocFormat;
            }
            set
            {
                font_header.indexToLocFormat = (short) value;
            }
        }
    internal uint checkSumAdjustment { set { font_header.checkSumAdjustment = value; } }
    public ushort unitsPerEm { get { return font_header.unitsPerEm; } }
    public ushort Flags { get { return font_header.flags; } }
    public ushort MacStyle { get { return font_header.macStyle; } set { font_header.macStyle = value; } }
    public short xMin { get { return font_header.xMin; } }
    public short xMax { get { return font_header.xMax; } }
    public short yMin { get { return font_header.yMin; } }
    public short yMax { get { return font_header.yMax; } }
    public ushort LowestRecPPEM { get { return font_header.lowestRecPPEM; } }
    public short FontDirectionHint { get { return font_header.fontDirectionHint; } }
    public ulong CreatedDateTime { get { return font_header.CreatedDateTime; } }
    public ulong ModifiedDateTime { get { return font_header.ModifiedDateTime; } }

    private void ChangeEndian()
    {
      font_header.indexToLocFormat = SwapInt16(font_header.indexToLocFormat);
      font_header.magicNumber = SwapUInt32(font_header.magicNumber);
      font_header.unitsPerEm = SwapUInt16(font_header.unitsPerEm);
      font_header.xMin = SwapInt16(font_header.xMin);
      font_header.xMax = SwapInt16(font_header.xMax);
      font_header.yMin = SwapInt16(font_header.yMin);
      font_header.yMax = SwapInt16(font_header.yMax);
      font_header.lowestRecPPEM = SwapUInt16(font_header.lowestRecPPEM);
      font_header.fontDirectionHint = SwapInt16(font_header.fontDirectionHint);
      font_header.CreatedDateTime = SwapUInt64(font_header.CreatedDateTime);
      font_header.ModifiedDateTime = SwapUInt64(font_header.ModifiedDateTime);
      font_header.checkSumAdjustment = 0;

      if (font_header.macStyle != 0)
      {
        font_header.macStyle = SwapUInt16(font_header.macStyle);
      }
    }

    internal override void Load(IntPtr font)
    {
      IntPtr header_ptr = Increment(font, (int)this.Offset);
      font_header = (FontHeader)Marshal.PtrToStructure(header_ptr, typeof(FontHeader));
      font_header.checkSumAdjustment = 0;
      Marshal.StructureToPtr(font_header, header_ptr, false);

      ChangeEndian();
    }

    internal void SaveFontHeader(IntPtr header_ptr, uint CheckSum)
    {
      ChangeEndian();

      header_ptr = Increment(header_ptr, (int)this.PackedOfffset);
      font_header.checkSumAdjustment = SwapUInt32(CheckSum);
      Marshal.StructureToPtr(font_header, header_ptr, true);
    }

    public FontHeaderClass(TrueTypeTable src) : base(src)
    {
      // This form is empty
    }
    public FontHeaderClass(FontHeaderClass src) : base(src)
    {
      this.font_header = (src as FontHeaderClass).font_header;
    }
  }

}

#pragma warning restore