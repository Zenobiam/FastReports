using System;
using System.Runtime.InteropServices;

#pragma warning disable CS3001, CS3002, CS3003, CS1591

namespace FastReport.Fonts
{
    /// <summary>
    /// MaximumProfile table
    /// </summary>
    public class MaximumProfileClass : TrueTypeTable
    {
        #region "Structure definition"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct MaximumProfile
        {
            [FieldOffset(0)]
            public uint Version; // version number	0x00010000 for version 1.0.
            [FieldOffset(4)]
            public ushort numGlyphs; //	The number of glyphs in the font.
            [FieldOffset(6)]
            public ushort maxPoints; //	Maximum points in a non-composite glyph.
            [FieldOffset(8)]
            public ushort maxContours; //	Maximum contours in a non-composite glyph.
            [FieldOffset(10)]
            public ushort maxCompositePoints; //	Maximum points in a composite glyph.
            [FieldOffset(12)]
            public ushort maxCompositeContours; //	Maximum contours in a composite glyph.
            [FieldOffset(14)]
            public ushort maxZones; //	1 if instructions do not use the twilight zone (Z0), or 2 if instructions do use Z0; should be set to 2 in most cases.
            [FieldOffset(16)]
            public ushort maxTwilightPoints; //	Maximum points used in Z0.
            [FieldOffset(18)]
            public ushort maxStorage; //	Number of Storage Area locations. 
            [FieldOffset(20)]
            public ushort maxFunctionDefs; //	Number of FDEFs.
            [FieldOffset(22)]
            public ushort maxInstructionDefs; //	Number of IDEFs.
            [FieldOffset(24)]
            public ushort maxStackElements; //	Maximum stack depth .
            [FieldOffset(26)]
            public ushort maxSizeOfInstructions; //	Maximum byte count for glyph instructions.
            [FieldOffset(28)]
            public ushort maxComponentElements; //	Maximum number of components referenced at “top level” for any composite glyph.
            [FieldOffset(30)]
            public ushort maxComponentDepth; //	Maximum levels of recursion; 1 for simple components.
        }
        #endregion

        private MaximumProfile max_profile;

        private void ChangeEndian()
        {
            max_profile.Version = SwapUInt32(max_profile.Version);
            max_profile.numGlyphs = SwapUInt16(max_profile.numGlyphs);
            max_profile.maxPoints = SwapUInt16(max_profile.maxPoints);
            max_profile.maxContours = SwapUInt16(max_profile.maxContours);
            max_profile.maxCompositePoints = SwapUInt16(max_profile.maxCompositePoints);
            max_profile.maxCompositeContours = SwapUInt16(max_profile.maxCompositeContours);
            max_profile.maxZones = SwapUInt16(max_profile.maxZones);
            max_profile.maxTwilightPoints = SwapUInt16(max_profile.maxTwilightPoints);
            max_profile.maxStorage = SwapUInt16(max_profile.maxStorage);
            max_profile.maxFunctionDefs = SwapUInt16(max_profile.maxFunctionDefs);
            max_profile.maxInstructionDefs = SwapUInt16(max_profile.maxInstructionDefs);
            max_profile.maxStackElements = SwapUInt16(max_profile.maxStackElements);
            max_profile.maxSizeOfInstructions = SwapUInt16(max_profile.maxSizeOfInstructions);
            max_profile.maxComponentElements = SwapUInt16(max_profile.maxComponentElements);
            max_profile.maxComponentDepth = SwapUInt16(max_profile.maxComponentDepth);
        }

        internal override void Load(IntPtr font)
        {
            IntPtr mprofile_ptr = Increment(font, (int)this.Offset);
            max_profile = (MaximumProfile)Marshal.PtrToStructure(mprofile_ptr, typeof(MaximumProfile));
            ChangeEndian();
        }

        internal override uint Save(IntPtr font, uint offset)
        {
            this.Offset = offset;

            ChangeEndian();
            IntPtr profile_ptr = Increment(font, (int)offset);
            Marshal.StructureToPtr(max_profile, profile_ptr, false);
            ChangeEndian();

            return offset + (uint)this.Length;
        }

        public MaximumProfileClass(TrueTypeTable src) : base(src) { }
        public int GlyphsCount { get { return max_profile.numGlyphs; } set { max_profile.numGlyphs = (ushort)value; } }
    }

}
#pragma warning restore