using System;
using System.Runtime.InteropServices;

namespace FastReport.Fonts
{

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // HorizontalHeader table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class HorizontalHeaderClass : TrueTypeTable
    {
        #region "Type definition"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct HorizontalHeader
        {
            [FieldOffset(0)]
            public uint Version; // version number	0x00010000 for version 1.0.
            [FieldOffset(4)]
            public short Ascender; // Typographic ascent.
            [FieldOffset(6)]
            public short Descender; // Typographic descent.
            [FieldOffset(8)]
            public short LineGap; //	Typographic line gap. Negative LineGap values are treated as zero in Windows 3.1, System 6, and System 7.
            [FieldOffset(10)]
            public ushort advanceWidthMax; //	Maximum advance width value in ‘hmtx’ table.
            [FieldOffset(12)]
            public short minLeftSideBearing; // Minimum left sidebearing value in ‘hmtx’ table.
            [FieldOffset(14)]
            public short minRightSideBearing; // Minimum right sidebearing value; calculated as Min(aw - lsb - (xMax - xMin)).
            [FieldOffset(16)]
            public short xMaxExtent;     //  Max(lsb + (xMax - xMin)).
            [FieldOffset(18)]
            public short caretSlopeRise; // Used to calculate the slope of the cursor (rise/run); 1 for vertical.
            [FieldOffset(20)]
            public short caretSlopeRun;  // 0 for vertical.
            [FieldOffset(22)]
            public short reserved1;  // set to 0
            [FieldOffset(24)]
            public short reserved2;  // set to 0
            [FieldOffset(26)]
            public short reserved3;  // set to 0
            [FieldOffset(28)]
            public short reserved4;  // set to 0
            [FieldOffset(30)]
            public short reserved5;  // set to 0
            [FieldOffset(32)]
            public short metricDataFormat; //	0 for current format.
            [FieldOffset(34)]
            public ushort numberOfHMetrics; //	Number of hMetric entries in  ‘hmtx’ table; may be smaller than the total number of glyphs in the font.
        }
        #endregion

        private HorizontalHeader horizontal_header;

        private void ChangeEndian()
        {
            horizontal_header.Version = SwapUInt32(horizontal_header.Version);
            horizontal_header.Ascender = SwapInt16(horizontal_header.Ascender);
            horizontal_header.Descender = SwapInt16(horizontal_header.Descender);
            horizontal_header.LineGap = SwapInt16(horizontal_header.LineGap);
            horizontal_header.advanceWidthMax = SwapUInt16(horizontal_header.advanceWidthMax);
            horizontal_header.minLeftSideBearing = SwapInt16(horizontal_header.minLeftSideBearing);
            horizontal_header.minRightSideBearing = SwapInt16(horizontal_header.minRightSideBearing);
            horizontal_header.xMaxExtent = SwapInt16(horizontal_header.xMaxExtent);
            horizontal_header.caretSlopeRise = SwapInt16(horizontal_header.caretSlopeRise);
            horizontal_header.caretSlopeRun = SwapInt16(horizontal_header.caretSlopeRun);
            horizontal_header.metricDataFormat = SwapInt16(horizontal_header.metricDataFormat);
            horizontal_header.numberOfHMetrics = SwapUInt16(horizontal_header.numberOfHMetrics);
        }

        internal override void Load(IntPtr font)
        {
            IntPtr horizontal_header_ptr = Increment(font, (int)this.Offset);
            horizontal_header = (HorizontalHeader)Marshal.PtrToStructure(horizontal_header_ptr, typeof(HorizontalHeader));
            ChangeEndian();
        }

        internal override uint Save(IntPtr font, uint offset)
        {
            this.Offset = offset;
        
            ChangeEndian();
            IntPtr horizontal_header_ptr = Increment(font, (int)offset);
            Marshal.StructureToPtr(horizontal_header, horizontal_header_ptr, false);
            ChangeEndian();

            return offset + (uint)this.Length;
        }

        public short Ascender { get { return horizontal_header.Ascender; } }
        public short Descender { get { return horizontal_header.Descender; } }
        public short LineGap { get { return horizontal_header.LineGap; } }
        public ushort MaxWidth { get { return horizontal_header.advanceWidthMax; } }
        public short MinLeftSideBearing { get { return horizontal_header.minLeftSideBearing; } }
        public ushort NumberOfHMetrics { get { return horizontal_header.numberOfHMetrics; } set { horizontal_header.numberOfHMetrics = value; } }
        public HorizontalHeaderClass(TrueTypeTable src) : base(src) { }
    }

}
