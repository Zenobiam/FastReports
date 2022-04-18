using System;
using System.Collections;
using System.Runtime.InteropServices;

#pragma warning disable CS3001, CS3002, CS3003, CS1591

namespace FastReport.Fonts
{
    /// <summary>
    /// Kerning table
    /// </summary>
    public class KerningTableClass : TrueTypeTable
    {
        #region "Type definition"
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct KerningTableHeader
        {
            public ushort Version;
            public ushort nTables;
        }

        internal class KerningSubtableClass : TTF_Helpers
        {
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct CommonKerningHeader
            {
                public ushort Version;
                public ushort Length;
                public ushort Coverage;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct FormatZero
            {
                public ushort nPairs;           // 	This gives the number of kerning pairs in the table.
                public ushort searchRange;      //  The largest power of two less than or equal to the value of nPairs, multiplied by the size in bytes of an entry in the table.
                public ushort entrySelector;    //  This is calculated as log2 of the largest power of two less than or equal to the value of nPairs. This value indicates how many iterations of the search loop will have to be made. (For example, in a list of eight items, there would have to be three iterations of the loop).
                public ushort rangeShift;       // 	The value of nPairs minus the largest power of two less than or equal to nPairs, and then multiplied by the size in bytes of an entry in the table.            
            }

            public CommonKerningHeader common_header;
            FormatZero format_zero;

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Kern_Zero_Pair
            {
                public ushort left;   /* index of left  glyph in pair */
                public ushort right;  /* index of right glyph in pair */
                public short value;  /* kerning value                */
            };

            public Kern_Zero_Pair[] kerning0_pairs;

            public int Length { get { return common_header.Length; } }

            public KerningSubtableClass(IntPtr kerning_table_ptr)
            {
                common_header = (CommonKerningHeader)Marshal.PtrToStructure(kerning_table_ptr, typeof(CommonKerningHeader));
                common_header.Length = SwapUInt16(common_header.Length);
                common_header.Coverage = SwapUInt16(common_header.Coverage);

                kerning_table_ptr = Increment(kerning_table_ptr, Marshal.SizeOf(common_header));

                if (common_header.Coverage == 1)
                {
                    format_zero = (FormatZero)Marshal.PtrToStructure(kerning_table_ptr, typeof(FormatZero));
                    format_zero.nPairs = SwapUInt16(format_zero.nPairs);
                    format_zero.searchRange = SwapUInt16(format_zero.searchRange);
                    format_zero.entrySelector = SwapUInt16(format_zero.entrySelector);
                    format_zero.rangeShift = SwapUInt16(format_zero.rangeShift);

                    kerning_table_ptr = Increment(kerning_table_ptr, Marshal.SizeOf(format_zero));
                    kerning0_pairs = new Kern_Zero_Pair[format_zero.nPairs];
                    for (int i = 0; i < format_zero.nPairs; i++)
                    {
                        kerning0_pairs[i] = (Kern_Zero_Pair)Marshal.PtrToStructure(kerning_table_ptr, typeof(Kern_Zero_Pair));
                        kerning0_pairs[i].left = SwapUInt16(kerning0_pairs[i].left);
                        kerning0_pairs[i].right = SwapUInt16(kerning0_pairs[i].right);
                        kerning0_pairs[i].value = SwapInt16(kerning0_pairs[i].value);
                        kerning_table_ptr = Increment(kerning_table_ptr, Marshal.SizeOf(typeof(Kern_Zero_Pair)));
                    }
                }
                else if (common_header.Coverage == 0x8000)
                {
                    // new Apple-dialect of kerning table
                }
                else if (common_header.Coverage == 0x0000)
                {
                    // classic Apple-dialect of kerning table
                }
                else
                {
                    throw new Exception("An unknown dialect of kerning table");
                }
            }
        }

        #endregion

        public KerningTableHeader kerning_table_header;
        private ArrayList kerning_subtables_collection;
        internal KerningSubtableClass format_zero_kern = null;

        private void ChangeEndian()
        {
            kerning_table_header.nTables = SwapUInt16(kerning_table_header.nTables);
        }

        internal override void Load(IntPtr font)
        {
            IntPtr kerning_table_ptr = Increment(font, (int)this.Offset);
            kerning_table_header = (KerningTableHeader)Marshal.PtrToStructure(kerning_table_ptr, typeof(KerningTableHeader));
            ChangeEndian();
            IntPtr subtable_ptr = Increment(kerning_table_ptr, Marshal.SizeOf(kerning_table_header));
            for (int i = 0; i < kerning_table_header.nTables; i++)
            {
                KerningSubtableClass subtable = new KerningSubtableClass(subtable_ptr);
                if(subtable.common_header.Coverage == 1)
                {
                    this.format_zero_kern = subtable;
                }
                kerning_subtables_collection.Add(subtable);
                subtable_ptr = Increment(subtable_ptr, subtable.Length);
            }
        }

        internal short TryKerning(ushort left, ushort right)
        {
            long target_idx = (((long)left) << 16) + right;
            short min, max, middle;
            short new_min = 0;
            short new_max = (short)(this.format_zero_kern.kerning0_pairs.Length - 1);
            short fix_kerning = 0;
            do
            {
                min = new_min;
                max = new_max;
                middle = (short)(max - ((max - min) >> 1));

                long current_idx = (this.format_zero_kern.kerning0_pairs[middle].left << 16) +
                              this.format_zero_kern.kerning0_pairs[middle].right;

                if (target_idx == current_idx)
                {
                    fix_kerning += this.format_zero_kern.kerning0_pairs[middle].value;
                    break;
                }
                else if (target_idx < current_idx)
                {
                    if (middle == min)
                        break;
                    new_max = (short)(middle - 1);
                }
                else
                {
                    if (middle == max)
                        break;
                    new_min = (short)(middle + 1);
                }
            } while (min < max);
            return fix_kerning;
        }

        public KerningTableClass(TrueTypeTable src) : base(src)
        {
            kerning_subtables_collection = new ArrayList();
        }
    }

}
#pragma warning restore
