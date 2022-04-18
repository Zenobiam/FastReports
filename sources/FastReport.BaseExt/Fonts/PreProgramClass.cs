using System;
using System.Runtime.InteropServices;

namespace FastReport.Fonts
{

    /////////////////////////////////////////////////////////////////////////////////////////////////
    // PreProgramm table
    /////////////////////////////////////////////////////////////////////////////////////////////////
    class PreProgramClass : TrueTypeTable
    {
        private byte[] program;

        internal override void Load(IntPtr font)
        {
            uint length = ((this.Length + 3) / 4) * 4;
            program = new byte[length];
            IntPtr program_ptr = Increment(font, (int)this.Offset);
            Marshal.Copy(program_ptr, program, 0, program.Length);
        }

        internal override uint Save(IntPtr font, uint offset)
        {
            this.Offset = offset;
            IntPtr program_ptr = Increment(font, (int)offset);
            Marshal.Copy(program, 0, program_ptr, program.Length);
            return offset + (uint)program.Length;
        }

        public PreProgramClass(TrueTypeTable src) : base(src) { }
    }

}