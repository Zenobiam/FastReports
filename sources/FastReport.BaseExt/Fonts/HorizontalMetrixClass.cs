using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#pragma warning disable CS3001, CS3002, CS3003, CS1591

namespace FastReport.Fonts
{
    /// <summary>
    /// HorizontalMetrix table 
    /// </summary>
    public class HorizontalMetrixClass : TrueTypeTable
    {
        #region "Type definition"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct longHorMetric
        {
            [FieldOffset(0)]
            public ushort advanceWidth;
            [FieldOffset(2)]
            public short lsb;
        };
        #endregion

        longHorMetric[] metrixTable;
        short[] lsbExt;

        public ushort numberOfMetrics;
        public longHorMetric this[int index]
        {
            get
            {
                // Microsoft say: "If the font is monospaced, only one entry need be in the array, but that entry is required"
                if (index >= metrixTable.Length) index = 0;
                return metrixTable[index];
            }
        }

        public void RepackWithDictionary(ref Dictionary<ushort, GlyphChar> dict, int source_glyph_count)
        {
            longHorMetric[]  metrix = new longHorMetric[dict.Count];
            ushort[] lsb = new ushort[source_glyph_count - metrixTable.Length];
            int i = 0;
            foreach( ushort key in dict.Keys)
            {
                GlyphChar gc = dict[key];
                if (key < metrixTable.Length)
                {
                    metrix[gc.Glyph] = metrixTable[key];
                }
                else
                {
                    // TODO: Correct LSB
                    throw new Exception("Check me in RepackWithDictionary");
                    // short lsb = lsbExt[key];
                }
            }
            metrixTable = metrix;
            numberOfMetrics =(ushort) metrix.Length;
        }

        internal override void Load(IntPtr font)
        {
            metrixTable = new longHorMetric[numberOfMetrics];

            IntPtr h_metrix_ptr = Increment(font, (int)this.Offset);

            for (int i = 0; i < numberOfMetrics; i++)
            {
                metrixTable[i] = (longHorMetric)Marshal.PtrToStructure(h_metrix_ptr, typeof(longHorMetric));
                metrixTable[i].advanceWidth = SwapUInt16(metrixTable[i].advanceWidth);
                metrixTable[i].lsb = SwapInt16(metrixTable[i].lsb);
                h_metrix_ptr = Increment(h_metrix_ptr, 4);
            }
        }

        internal override uint Save(IntPtr font, uint offset)
        {
            this.Offset = offset;
            IntPtr horizontal_header_ptr = Increment(font, (int)offset);

            IntPtr h_metrix_ptr = Increment(font, (int)offset);
            for (int i = 0; i < numberOfMetrics; i++)
            {
                longHorMetric rec;
                rec.advanceWidth = SwapUInt16(metrixTable[i].advanceWidth);
                rec.lsb = SwapInt16(metrixTable[i].lsb);
                Marshal.StructureToPtr(rec, h_metrix_ptr, false);
                h_metrix_ptr = Increment(h_metrix_ptr, 4);
            }
            uint len = (uint)numberOfMetrics * 4;
            SetLenght(len);

            return offset + len;
        }

        internal override uint Save(IntPtr src, IntPtr dst, uint offset)
        {
            return base.Save(src, dst, offset);
        }

        public HorizontalMetrixClass(TrueTypeTable src)
            : base(src)
        {
        }
    }

}
#pragma warning restore