using System;
using System.Runtime.InteropServices;

#pragma warning disable CS3001, CS3002, CS3003, CS1591

namespace FastReport.Fonts
{
    /////////////////////////////////////////////////////////////////////////////////////////////////
    // PostScript table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    public class PostScriptClass : TrueTypeTable
    {
        #region "Type definition"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct PostScript
        {
            [FieldOffset(0)]
            public uint Version; // version number	0x00010000 for version 1.0.
            [FieldOffset(4)]
            public int ItalicAngle; // Italic angle in counter-clockwise degrees from the vertical. Zero for upright text, negative for text that leans to the right (forward).
            [FieldOffset(8)]
            public short underlinePosition; // This is the suggested distance of the top of the underline from the baseline (negative values indicate below baseline). 
            [FieldOffset(10)]
            public short underlineThickness; // Suggested values for the underline thickness.
            [FieldOffset(12)]
            public uint isFixedPitch; // Set to 0 if the font is proportionally spaced, non-zero if the font is not proportionally spaced (i.e. monospaced).
            [FieldOffset(16)]
            public uint minMemType42; // Minimum memory usage when an OpenType font is downloaded.
            [FieldOffset(20)]
            public uint maxMemType42; // Maximum memory usage when an OpenType font is downloaded.
            [FieldOffset(24)]
            public uint minMemType1; // Minimum memory usage when an OpenType font is downloaded as a Type 1 font.
            [FieldOffset(28)]
            public uint maxMemType1; // Maximum memory usage when an OpenType font is downloaded as a Type 1 font.
        }
        #endregion

        internal PostScript post_script;
        public bool IsItalic
        {
            get
            {
                return post_script.ItalicAngle != 0;
            }

            set
            {
                post_script.ItalicAngle = value ? post_script.ItalicAngle != 0 ? post_script.ItalicAngle : 1 : 0;
            }
        }
        public bool IsFixedPitch { get { return post_script.isFixedPitch != 0; } }

    private void ChangeEndian()
    {
      post_script.Version = SwapUInt32(post_script.Version);
      post_script.ItalicAngle = SwapInt32(post_script.ItalicAngle);
      post_script.underlinePosition = SwapInt16(post_script.underlinePosition);
      post_script.underlineThickness = SwapInt16(post_script.underlineThickness);
      post_script.isFixedPitch = SwapUInt32(post_script.isFixedPitch);
      post_script.minMemType42 = SwapUInt32(post_script.minMemType42);
      post_script.maxMemType42 = SwapUInt32(post_script.maxMemType42);
      post_script.minMemType1 = SwapUInt32(post_script.minMemType1);
      post_script.maxMemType1 = SwapUInt32(post_script.maxMemType1);
    }

    internal override void Load(IntPtr font)
    {
      IntPtr post_script_ptr = Increment(font, (int)this.Offset);
      post_script = (PostScript)Marshal.PtrToStructure(post_script_ptr, typeof(PostScript));
      ChangeEndian();
    }

    internal override uint Save(IntPtr font, uint offset)
    {
      this.Offset = offset;

      post_script.Version = 0x00030000;
      if (this.Length != (uint)Marshal.SizeOf(typeof(PostScript)))
        throw new Exception("Unexpected format of PostScript table ");

      ChangeEndian();
      IntPtr post_script_ptr = Increment(font, (int)offset);
      Marshal.StructureToPtr(post_script, post_script_ptr, false);
      ChangeEndian();

      return offset + (uint)this.Length;
    }

    public int ItalicAngle { get { return post_script.ItalicAngle; } }


    public PostScriptClass(TrueTypeTable src) : base(src) { }
  }

}