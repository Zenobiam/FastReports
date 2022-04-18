using System;
using System.Runtime.InteropServices;

#pragma warning disable CS3001, CS3002, CS3003, CS1591

namespace FastReport.Fonts
{
    /// <summary>
    /// IndexToLocation table 
    /// </summary>
    public class IndexToLocationClass : TrueTypeTable
    {
        private ushort[] shortIndexToLocation = null;
        private uint[] longIndexToLocation = null;

        internal ushort[] Short { get { return shortIndexToLocation; } }
        internal uint[] Long { get { return longIndexToLocation; } }

        internal void LoadIndexToLocation(IntPtr font, FontHeaderClass font_header)
        {
            int count;
            IntPtr i2l_ptr = Increment(font, (int)(this.Offset));
            switch (font_header.indexToLocFormat)
            {
                case FontHeaderClass.IndexToLoc.ShortType:
                    count = (int)this.Length / 2;
                    short[] ShortTemp = new short[count];
                    Marshal.Copy(i2l_ptr, ShortTemp, 0, count);
                    shortIndexToLocation = new ushort[count];
                    for (int i = 0; i < count; i++)
                    {
                        byte[] buf = BitConverter.GetBytes(ShortTemp[i]);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(buf);
                        shortIndexToLocation[i] = BitConverter.ToUInt16(buf, 0);
                    }
                    ShortTemp = null;
                    break;

                case FontHeaderClass.IndexToLoc.LongType:
                    count = (int)this.Length / 4;
                    int[] LongTemp = new int[count];
                    Marshal.Copy(i2l_ptr, LongTemp, 0, count);
                    longIndexToLocation = new uint[count];
                    for (int j = 0; j < count; j++)
                    {
                        byte[] buf = BitConverter.GetBytes(LongTemp[j]);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(buf);
                        longIndexToLocation[j] = BitConverter.ToUInt32(buf, 0);
                    }
                    LongTemp = null;
                    break;

                default:
                    throw new Exception("Unsupported Index to Location format");
            }
        }

        public ushort GetGlyph(ushort i2l_idx, FontHeaderClass font_header, out uint location)
        {
            location = 0;
            ushort length = 0;

            switch (font_header.indexToLocFormat)
            {
                case FontHeaderClass.IndexToLoc.ShortType:
                    if (i2l_idx >= shortIndexToLocation.Length)
                    {
                        location = 0;
                        break;
                    }
                    location = (uint)(2 * shortIndexToLocation[i2l_idx]);
                    length = (ushort)(2 * (shortIndexToLocation[i2l_idx + 1] - shortIndexToLocation[i2l_idx]));
                    break;

                case FontHeaderClass.IndexToLoc.LongType:
                    location = longIndexToLocation[i2l_idx];
                    length = (ushort)(longIndexToLocation[i2l_idx + 1] - longIndexToLocation[i2l_idx]);
                    break;

            }
            return length;
        }
        internal override uint Save(IntPtr font, uint offset)
        //        internal override uint Save(IntPtr src, IntPtr dst, uint offset)
        {
            this.Offset = offset;
            return base.Save(font, offset);
        }

        public IndexToLocationClass(TrueTypeTable src) : base(src) { }
    }

}

#pragma warning restore