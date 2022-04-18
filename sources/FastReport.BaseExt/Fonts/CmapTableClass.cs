using System;
using System.Runtime.InteropServices;

namespace FastReport.Fonts
{

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // Cmap table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class CmapTableClass : TrueTypeTable
    {
        #region "Type definition"
        public enum EncodingFormats
        {
            ByteEncoding = 0,
            HighByteMapping = 2,
            SegmentMapping = 4,
            TrimmedTable = 6,
            TrimmedArray = 10,
            SegmentedCoverage = 12,
            ManyToOneRangeMapping = 13,
            UnicodeVariationSequences = 14
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Table_CMAP
        {
            [FieldOffset(0)]
            public ushort TableVersion;
            [FieldOffset(2)]
            public ushort NumSubTables;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Table_SUBMAP
        {
            [FieldOffset(0)]
            public ushort Platform;
            [FieldOffset(2)]
            public ushort EncodingID;
            [FieldOffset(4)]
            public uint TableOffset;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Table_Encode
        {
            [FieldOffset(0)]
            public ushort Format;
            [FieldOffset(2)]
            public ushort Length;
            [FieldOffset(4)]
            public ushort Version;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct SegmentMapping
        {
            [FieldOffset(0)]
            public ushort segCountX2;       // 2 x segCount.
            [FieldOffset(2)]
            public ushort searchRange;      // 2 x (2**floor(log2(segCount)))
            [FieldOffset(4)]
            public ushort entrySelector;    // log2(searchRange/2)
            [FieldOffset(6)]
            public ushort rangeShift;       // 2 x segCount - searchRange
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct SequentialMapGroup
        {
            [FieldOffset(0)]
            public uint startCharCode;
            [FieldOffset(4)]
            public uint searchRange;
            [FieldOffset(8)]
            public uint entrySelector;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Format6 // Trimmed table
        {
            [FieldOffset(0)]
            public ushort format;
            [FieldOffset(4)]
            public ushort length;
            [FieldOffset(6)]
            public ushort language;
            [FieldOffset(8)]
            public ushort startCharCode;
            [FieldOffset(10)]
            public ushort numChars;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Format8
        {
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Format10 // Trimmed array
        {
            [FieldOffset(0)]
            public ushort format;
            [FieldOffset(2)]
            public ushort reserved;
            [FieldOffset(4)]
            public uint length;
            [FieldOffset(8)]
            public uint language;
            [FieldOffset(12)]
            public uint startCharCode;
            [FieldOffset(16)]
            public uint numChars;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct Format12
        {
            [FieldOffset(0)]
            public ushort format;       //  Subtable format; set to 12.
            [FieldOffset(2)]
            public ushort reserved;     //  Reserved; set to 0
            [FieldOffset(4)]
            public uint length;         //  Byte length of this subtable (including the header)
            [FieldOffset(8)]
            public uint language;
            [FieldOffset(12)]
            public uint numGroups;      // Number of groupings which follow
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct VariationSequences
        {
            [FieldOffset(0)]
            public ushort Format;
            [FieldOffset(2)]
            public uint Length;
            [FieldOffset(6)]
            public uint NumRecords;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct VariationSequenceRecord
        {
            [FieldOffset(0)]
            public byte _b0;       // 
            [FieldOffset(1)]
            public byte _b1;       // 
            [FieldOffset(2)]
            public byte _b2;       // 
            [FieldOffset(3)]
            public uint defaultUVSOffset;      // 
            [FieldOffset(7)]
            public uint nonDefaultUVSOffset;    //
            // Not shure that followin property is allowed in packed structures
            public uint VariantSelector { get { return (uint)(_b0 | (_b1 << 8) | (_b2 << 16)); } }
        }

        #endregion

        // Format 4: Segment mapping to delta values
        int segment_count;
        ushort[] endCount;
        ushort[] startCount;
        short[] idDelta;
        ushort[] idRangeOffset;
        ushort[] glyphIndexArray;

        // Format 6 and 10
        uint trimmedStat;
        ushort[] trimmed;

        // Format 12: Segmented coverage
        SequentialMapGroup[] mapGroup;

        private ushort[] LoadCmapSegment(IntPtr segment_ptr, int segment_count, short[] temp_mem)
        {
            ushort[] result = new ushort[segment_count];

            Marshal.Copy(segment_ptr, temp_mem, 0, segment_count);
            for (int i = 0; i < segment_count; i++)
            {
                byte[] buf = BitConverter.GetBytes(temp_mem[i]);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buf);
                result[i] = BitConverter.ToUInt16(buf, 0);
            }

            return result;
        }

        private short[] LoadSignedCmapSegment(IntPtr segment_ptr, int segment_count, short[] temp_mem)
        {
            short[] result = new short[segment_count];

            Marshal.Copy(segment_ptr, temp_mem, 0, segment_count);
            for (int i = 0; i < segment_count; i++)
            {
                result[i] = SwapInt16(temp_mem[i]);
            }

            return result;
        }

        internal void LoadCmapTable(IntPtr font)
        {
            IntPtr subtable_ptr;
            IntPtr cmap_ptr = Increment(font, (int)this.Offset);
            Table_CMAP cmap = (Table_CMAP)Marshal.PtrToStructure(cmap_ptr, typeof(Table_CMAP));
            int subtables_count = SwapUInt16(cmap.NumSubTables);

            IntPtr submap_ptr = Increment(cmap_ptr, Marshal.SizeOf(cmap));
            IntPtr payload_ptr;
            Table_SUBMAP submap;

            for (int j = 0; j < subtables_count; j++)
            {
                submap = (Table_SUBMAP)Marshal.PtrToStructure(submap_ptr, typeof(Table_SUBMAP));
                submap_ptr = Increment(submap_ptr, Marshal.SizeOf(submap));

                submap.Platform = SwapUInt16(submap.Platform);
                submap.EncodingID = SwapUInt16(submap.EncodingID);
                submap.TableOffset = SwapUInt32(submap.TableOffset);

                // --- Skip non microsft unicode charmaps
                // --- No no no! We will try to parse as much as ppossible
                // if ((submap.Platform != 3 || submap.EncodingID != 1)) continue;

                IntPtr encode_ptr = Increment(cmap_ptr, (int)submap.TableOffset);
                Table_Encode encode = (Table_Encode)Marshal.PtrToStructure(encode_ptr, typeof(Table_Encode));
                subtable_ptr = encode_ptr;

                encode.Format = SwapUInt16(encode.Format);
                encode.Length = SwapUInt16(encode.Length);
                encode.Version = SwapUInt16(encode.Version);

                switch ((EncodingFormats)encode.Format)
                {
                    case EncodingFormats.ByteEncoding:
                        //throw new Exception("TO DO: ByteEncoding cmap format not implemented");
                        continue;

                    case EncodingFormats.HighByteMapping:
                        //throw new Exception("TO DO: HighByteMapping cmap format not implemented");
                        continue;

                    case EncodingFormats.SegmentMapping:
                        payload_ptr = Increment(encode_ptr, Marshal.SizeOf(encode));
                        SegmentMapping segment = (SegmentMapping)Marshal.PtrToStructure(payload_ptr, typeof(SegmentMapping));
                        segment.segCountX2 = SwapUInt16(segment.segCountX2);        // 2 x segCount.
                        segment.searchRange = SwapUInt16(segment.searchRange);      // 2 x (2**floor(log2(segCount)))
                        segment.entrySelector = SwapUInt16(segment.entrySelector);  // log2(searchRange/2)
                        segment.rangeShift = SwapUInt16(segment.rangeShift);        // 2 x segCount - searchRange

                        segment_count = segment.segCountX2 / 2;

                        // Euristic algoritmm for selection best representation. Not sure about it.
                        if (startCount == null || startCount.Length < segment_count)
                        {

                            short[] toFix = new short[segment_count];
                            payload_ptr = Increment(payload_ptr, Marshal.SizeOf(segment));
                            endCount = LoadCmapSegment(payload_ptr, segment_count, toFix);
                            payload_ptr = Increment(payload_ptr, segment.segCountX2 + sizeof(ushort));
                            startCount = LoadCmapSegment(payload_ptr, segment_count, toFix);
                            payload_ptr = Increment(payload_ptr, segment.segCountX2);
                            idDelta = LoadSignedCmapSegment(payload_ptr, segment_count, toFix);
                            payload_ptr = Increment(payload_ptr, segment.segCountX2);
                            idRangeOffset = LoadCmapSegment(payload_ptr, segment_count, toFix);
                            toFix = null;

                            uint index_array_size = (8 + 4 * (uint)segment_count) * 2;
                            index_array_size = (encode.Length - index_array_size) / 2;
                            payload_ptr = Increment(payload_ptr, segment.segCountX2);
                            //                        int checksize = encode.Length - 3 * segment_count * 2;
                            toFix = new short[index_array_size];
                            glyphIndexArray = LoadCmapSegment(payload_ptr, (int)index_array_size, toFix);
                            toFix = null;

#if false
						string[] debug = new string[segment_count];
						for (int z = 0; z < segment_count; z++)
						{ 
						debug[z] = ""+(char)startCount[z]+" - "+(char)endCount[z] +" = " + idDelta[z].ToString() + " & " + idRangeOffset[z].ToString();
						}
#endif
                        }
                        continue;

                    case EncodingFormats.TrimmedTable:
                        Format6 format6 = (Format6)Marshal.PtrToStructure(encode_ptr, typeof(Format6));
                        format6.length = SwapUInt16(format6.length);
                        format6.language = SwapUInt16(format6.language);
                        format6.startCharCode = SwapUInt16(format6.startCharCode);
                        format6.numChars = SwapUInt16(format6.numChars);
                        trimmed = new ushort[format6.numChars];
                        payload_ptr = Increment(encode_ptr, Marshal.SizeOf(format6));
                        short[] temp_6 = new short[format6.numChars];
                        Marshal.Copy(payload_ptr, temp_6, 0, (int)format6.numChars);
                        for (int i = 0; i < format6.numChars; ++i)
                        {
                            byte[] buf = BitConverter.GetBytes(temp_6[i]);
                            if (BitConverter.IsLittleEndian)
                                Array.Reverse(buf);
                            trimmed[i] = BitConverter.ToUInt16(buf, 0);
                        }
                        temp_6 = null;
                        trimmedStat = format6.startCharCode;
                        continue;

                    case EncodingFormats.TrimmedArray:
                        Format10 format10 = (Format10)Marshal.PtrToStructure(encode_ptr, typeof(Format10));
                        format10.length = SwapUInt32(format10.length);
                        format10.language = SwapUInt32(format10.language);
                        format10.startCharCode = SwapUInt32(format10.startCharCode);
                        format10.numChars = SwapUInt32(format10.numChars);
                        trimmed = new ushort[format10.numChars];
                        payload_ptr = Increment(encode_ptr, Marshal.SizeOf(format10));
                        short[] temp_10 = new short[format10.numChars];
                        Marshal.Copy(payload_ptr, temp_10, 0, (int)format10.numChars);
                        for (int i = 0; i < format10.numChars; ++i)
                        {
                            byte[] buf = BitConverter.GetBytes(temp_10[i]);
                            if (BitConverter.IsLittleEndian)
                                Array.Reverse(buf);
                            trimmed[i] = BitConverter.ToUInt16(buf, 0);
                        }
                        temp_10 = null;
                        trimmedStat = format10.startCharCode;
                        continue;

                    case EncodingFormats.SegmentedCoverage:
                        Format12 format12 = (Format12)Marshal.PtrToStructure(encode_ptr, typeof(Format12));
                        format12.length = SwapUInt32(format12.length);
                        format12.language = SwapUInt32(format12.language);
                        format12.numGroups = SwapUInt32(format12.numGroups);
                        mapGroup = new SequentialMapGroup[format12.numGroups];
                        payload_ptr = Increment(encode_ptr, Marshal.SizeOf(format12));
                        for (int i = 0; i < format12.numGroups; ++i)
                        {
                            mapGroup[i] = (SequentialMapGroup)Marshal.PtrToStructure(payload_ptr, typeof(SequentialMapGroup));
                            mapGroup[i].startCharCode = SwapUInt32(mapGroup[i].startCharCode);
                            mapGroup[i].searchRange = SwapUInt32(mapGroup[i].searchRange);
                            mapGroup[i].entrySelector = SwapUInt32(mapGroup[i].entrySelector);
                            payload_ptr = Increment(payload_ptr, Marshal.SizeOf(mapGroup[0]));
                        }
                        continue;

                    case EncodingFormats.ManyToOneRangeMapping:
                        //throw new Exception("TO DO: ManyToOneRangeMapping cmap format not implemented");
                        continue;

                    case EncodingFormats.UnicodeVariationSequences:
                        VariationSequences seq_header = (VariationSequences)Marshal.PtrToStructure(encode_ptr, typeof(VariationSequences));
                        seq_header.Length = SwapUInt32(seq_header.Length);
                        seq_header.NumRecords = SwapUInt32(seq_header.NumRecords);
                        // Not parsed, but this table is optional
                        continue;

                    default:
                        throw new Exception("cmap format not known");
                }
            }
        }

        internal ushort GetGlyphIndex(ushort ch)
        {
            ushort GlyphIDX = 0;
            if (segment_count != 0)
            {
                for (int i = 0; i < segment_count; i++)
                {
                    if (endCount[i] >= ch)
                    {
                        if (startCount[i] <= ch)
                        {
                            if (idRangeOffset[i] == 0)
                            {
                                GlyphIDX = (ushort)((ch + idDelta[i]) % 65536);
                            }
                            else
                            {
                                int j = (ushort)(idRangeOffset[i] / 2 + (ch - startCount[i]) - (segment_count - i));
                                GlyphIDX = this.glyphIndexArray[j];
                            }
                        }
                        return GlyphIDX;
                    }
                }
            }
            if (mapGroup != null && mapGroup.Length != 0)
            {
                for (int i = 0; i < mapGroup.Length; ++i)
                {
                    if (ch >= mapGroup[i].startCharCode && ch <= mapGroup[i].searchRange)
                    {
                        GlyphIDX = (ushort)((uint)ch - mapGroup[i].startCharCode + mapGroup[i].entrySelector);
                        return GlyphIDX;
                    }
                }
            }
            if (trimmed != null && trimmed.Length != 0)
            {
                uint idx;
                if (ch > trimmedStat)
                {
                    idx = (uint)(ch - trimmedStat);
                    if (idx < trimmed.Length)
                        GlyphIDX = trimmed[idx];
                }
            }
            return GlyphIDX;
        }

        public CmapTableClass(TrueTypeTable src) : base(src) { }
    }

}