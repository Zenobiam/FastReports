using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;

#pragma warning disable CS3001, CS3002, CS3003, CS1591

namespace FastReport.Fonts
{
    /// <summary>
    /// Table with encoded glyphs' outline
    /// </summary>
    public class GlyphTableClass : TrueTypeTable
    {
        #region "Type definitions"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct GlyphHeader
        {
            [FieldOffset(0)]
            public short numberOfContours;
            [FieldOffset(2)]
            public short xMin;
            [FieldOffset(4)]
            public short yMin;
            [FieldOffset(6)]
            public short xMax;
            [FieldOffset(8)]
            public short yMax;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct CompositeGlyphHeader
        {
            [FieldOffset(0)]
            public ushort flags;        //  	component flag
            [FieldOffset(2)]
            public ushort glyphIndex;   // 	glyph index of component        
        }

        public enum CompositeFlags
        {
            ARG_1_AND_2_ARE_WORDS = 0x0001,     // If this is set, the arguments are words; otherwise, they are bytes.
            ARGS_ARE_XY_VALUES = 0x0002,        // If this is set, the arguments are xy values; otherwise, they are points.
            ROUND_XY_TO_GRID = 0x0004,          // For the xy values if the preceding is true.
            WE_HAVE_A_SCALE = 0x0008,           // This indicates that there is a simple scale for the component. Otherwise, scale = 1.0.
            RESERVED = 0x0010,                  // This bit is reserved. Set it to 0.
            MORE_COMPONENTS = 0x0020,           // Indicates at least one more glyph after this one.
            WE_HAVE_AN_X_AND_Y_SCALE = 0x0040,  // The x direction will use a different scale from the y direction.
            WE_HAVE_A_TWO_BY_TWO = 0x0080,      // There is a 2 by 2 transformation that will be used to scale the component.
            WE_HAVE_INSTRUCTIONS = 0x0100,      // Following the last component are instructions for the composite character.
            USE_MY_METRICS = 0x0200,            // If set, this forces the aw and lsb (and rsb) for the composite to be equal to those from this original glyph. This works for hinted and unhinted characters.
            OVERLAP_COMPOUND = 0x0400,          // Used by Apple in GX fonts.
            SCALED_COMPONENT_OFFSET = 0x0800,   // Composite designed to have the component offset scaled (designed for Apple rasterizer).
            UNSCALED_COMPONENT_OFFSET = 0x10000 // Composite designed not to have the component offset scaled (designed for the Microsoft TrueType rasterizer).        
        }

        #endregion

        IntPtr glyph_table_ptr;

        internal override void Load(IntPtr font)
        {
            glyph_table_ptr = Increment(font, (int)(this.Offset));
        }

        internal GlyphHeader GetGlyphHeader(int glyph_offset)
        {
            GlyphHeader gheader;
            IntPtr glyph_ptr = Increment(glyph_table_ptr, glyph_offset);
            gheader = (GlyphHeader)Marshal.PtrToStructure(glyph_ptr, typeof(GlyphHeader));

            gheader.numberOfContours = SwapInt16(gheader.numberOfContours);
            gheader.xMax = SwapInt16(gheader.xMax);
            gheader.yMax = SwapInt16(gheader.yMax);
            gheader.xMin = SwapInt16(gheader.xMin);
            gheader.yMin = SwapInt16(gheader.yMin);

            return gheader;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Return list of glyph idexes of composite glyph
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public ArrayList CheckGlyph(int glyph_offset, int glyph_size)
        {
            ArrayList CompositeIndexes = new ArrayList();
            IntPtr glyph_ptr = Increment(glyph_table_ptr, glyph_offset);

            GlyphHeader gheader = GetGlyphHeader(glyph_offset);

            if (gheader.numberOfContours < 0)
            {
                CompositeGlyphHeader cgh;
                IntPtr composite_header_ptr = Increment(glyph_ptr, Marshal.SizeOf(gheader));
                do
                {
                    cgh = (CompositeGlyphHeader)Marshal.PtrToStructure(composite_header_ptr, typeof(CompositeGlyphHeader));
                    cgh.flags = SwapUInt16(cgh.flags);
                    cgh.glyphIndex = SwapUInt16(cgh.glyphIndex);

                    CompositeIndexes.Add(cgh.glyphIndex);

                    composite_header_ptr = Increment(composite_header_ptr, Marshal.SizeOf(cgh));

                    int x, y;

                    if ((cgh.flags & (ushort)CompositeFlags.ARG_1_AND_2_ARE_WORDS) != 0)
                    {
                        x = SwapInt16(Marshal.ReadInt16(composite_header_ptr));
                        composite_header_ptr = Increment(composite_header_ptr, 2);
                        y = SwapInt16(Marshal.ReadInt16(composite_header_ptr));
                        composite_header_ptr = Increment(composite_header_ptr, 2);

                    }
                    else
                    {
                        x = Marshal.ReadByte(composite_header_ptr);
                        composite_header_ptr = Increment(composite_header_ptr, 1);
                        y = Marshal.ReadByte(composite_header_ptr);
                        composite_header_ptr = Increment(composite_header_ptr, 1);
                    }

                    if ((cgh.flags & (ushort)CompositeFlags.WE_HAVE_A_SCALE) != 0)
                    {
                        composite_header_ptr = Increment(composite_header_ptr, 2);
                        //F2Dot14 scale;    /* Format 2.14 */
                    }
                    else if ((cgh.flags & (ushort)CompositeFlags.WE_HAVE_AN_X_AND_Y_SCALE) != 0)
                    {
                        composite_header_ptr = Increment(composite_header_ptr, 4);
                        //F2Dot14 xscale;    /* Format 2.14 */
                        //F2Dot14 yscale;    /* Format 2.14 */
                    }
                    else if ((cgh.flags & (ushort)CompositeFlags.WE_HAVE_A_TWO_BY_TWO) != 0)
                    {
                        composite_header_ptr = Increment(composite_header_ptr, 8);
                        //F2Dot14 xscale;    /* Format 2.14 */
                        //F2Dot14 scale01;   /* Format 2.14 */
                        //F2Dot14 scale10;   /* Format 2.14 */
                        //F2Dot14 yscale;    /* Format 2.14 */
                    }

                }
                while ((cgh.flags & (ushort)CompositeFlags.MORE_COMPONENTS) != 0);

                if ((cgh.flags & (ushort)CompositeFlags.WE_HAVE_INSTRUCTIONS) != 0)
                {
                    ushort num_instr = SwapUInt16((ushort)Marshal.PtrToStructure(composite_header_ptr, typeof(ushort)));
                    composite_header_ptr = Increment(composite_header_ptr, 2 + num_instr);
                    //USHORT numInstr
                    //BYTE instr[numInstr]
                }
            }
            else
            {
                ; // Simple glyph
            }
            return CompositeIndexes;
        }

        private IntPtr ReadRawByte(IntPtr ptr, out byte val)
        {
            val = (byte)Marshal.ReadByte(ptr);
            return Increment(ptr, 1);

        }

        internal enum GlyphFlags
        {
            ON_CURVE = 0x01, //  	If set, the point is on the curve; otherwise, it is off the curve.
            X_SHORT = 0x02, // 	If set, the corresponding x-coordinate is 1 byte long. If not set, 2 bytes.
            Y_SHORT = 0x04, // 	If set, the corresponding y-coordinate is 1 byte long. If not set, 2 bytes.
            REPEAT = 0x08, //    If set, the next byte specifies the number of additional times this set of flags is to be repeated. In this way, the number of flags listed can be smaller than the number of points in a character.
            X_SAME = 0x10, //    This flag has two meanings, depending on how the x-Short Vector flag is set. If x-Short Vector is set, this bit describes the sign of the value, with 1 equalling positive and 0 negative. If the x-Short Vector bit is not set and this bit is set, then the current x-coordinate is the same as the previous x-coordinate. If the x-Short Vector bit is not set and this bit is also not set, the current x-coordinate is a signed 16-bit delta vector.
            Y_SAME = 0x20,  //    This flag has two meanings, depending on how the y-Short Vector flag is set. If y-Short Vector is set, this bit describes the sign of the value, with 1 equalling positive and 0 negative. If the y-Short Vector bit is not set and this bit is set, then the current y-coordinate is the same as the previous y-coordinate. If the y-Short Vector bit is not set and this bit is also not set, the current y-coordinate is a signed 16-bit delta vector.        }
            X_POSITIVE = 0x10,
            Y_POSITIVE = 0x20
        }

        internal class GlyphPoint
        {
            public float x;
            public float y;
            public bool on_curve;
            public bool end_of_contour;

            public PointF Point { get { return new PointF(x, y); } }
        }

        const ushort F2Dot14 = 0x4000;

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // Return Graphics path of Glyph
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public FastGraphicsPath GetGlyph(
            int glyph_offset,
            ushort glyph_data_size,
            float font_rsize,
            PointF position,
            IndexToLocationClass index_to_location,
            FontHeaderClass font_header,
            out GlyphHeader gheader)
        {
            if (glyph_data_size == 0)
            {
                IntPtr glyph_ptr1 = Increment(glyph_table_ptr, glyph_offset);
                gheader = (GlyphHeader)Marshal.PtrToStructure(glyph_ptr1, typeof(GlyphHeader));
                //space is example have size 0
                return new FastGraphicsPath(FastFillMode.Alternate);
            }
            IntPtr glyph_ptr = Increment(glyph_table_ptr, glyph_offset);
            gheader = (GlyphHeader)Marshal.PtrToStructure(glyph_ptr, typeof(GlyphHeader));

            gheader.numberOfContours = SwapInt16(gheader.numberOfContours);
            gheader.xMax = SwapInt16(gheader.xMax);
            gheader.yMax = SwapInt16(gheader.yMax);
            gheader.xMin = SwapInt16(gheader.xMin);
            gheader.yMin = SwapInt16(gheader.yMin);

            ushort instructions_count;
            if (gheader.numberOfContours < 0)
            {
                FastGraphicsPath result = new FastGraphicsPath(FastFillMode.Alternate);
                IntPtr c_glyph_ptr = Increment(glyph_ptr, Marshal.SizeOf(typeof(GlyphHeader)));
                CompositeGlyphHeader cgheader;
                short argument1 = 0;
                short argument2 = 0;

                float m00 = 1;
                float m01 = 0;
                float m02 = 0;
                float m10 = 0;
                float m11 = 1;
                float m12 = 0;

                ushort arg1arg2;
                do
                {
                    cgheader = (CompositeGlyphHeader)Marshal.PtrToStructure(c_glyph_ptr, typeof(CompositeGlyphHeader));
                    cgheader.flags = SwapUInt16(cgheader.flags);
                    cgheader.glyphIndex = SwapUInt16(cgheader.glyphIndex);
                    //
                    uint location;
                    //ushort i2l_idx = this.cmap_table.GetGlyphIndex((ushort)ch);
                    ushort length = index_to_location.GetGlyph(cgheader.glyphIndex, font_header, out location);
                    FastGraphicsPath aPath = GetGlyph((int)location, length, font_rsize, Point.Empty, index_to_location, font_header, out gheader);
                    //


                    c_glyph_ptr = Increment(c_glyph_ptr, Marshal.SizeOf(typeof(CompositeGlyphHeader)));
                    if ((cgheader.flags & 0x1) > 0)
                    {
                        argument1 = (short)Marshal.PtrToStructure(c_glyph_ptr, typeof(short));
                        argument1 = SwapInt16(argument1);
                        c_glyph_ptr = Increment(c_glyph_ptr, Marshal.SizeOf(typeof(short)));
                        argument2 = (short)Marshal.PtrToStructure(c_glyph_ptr, typeof(short));
                        argument2 = SwapInt16(argument2);
                        c_glyph_ptr = Increment(c_glyph_ptr, Marshal.SizeOf(typeof(short)));
                        //read short x2
                    }
                    else
                    {
                        arg1arg2 = (ushort)Marshal.PtrToStructure(c_glyph_ptr, typeof(ushort));
                        c_glyph_ptr = Increment(c_glyph_ptr, Marshal.SizeOf(typeof(ushort)));
                        arg1arg2 = SwapUInt16(arg1arg2);
                        argument1 = (sbyte)(arg1arg2 >> 8);
                        argument2 = (sbyte)(arg1arg2 & 0xFF);
                        //unk what to do with sign
                        //read ushort
                    }
                    if ((cgheader.flags & 0x2) > 0)
                    {
                        //ARGS_ARE_XY_VALUES
                        m02 = argument1 / font_rsize;
                        m12 = -argument2 / font_rsize;
                    }
                    else
                    {
                        //ARGS_ARE_XY_A_NOT_VALUES
                        //unk what to do
                    }
                    short scale;
                    if ((cgheader.flags & 0x8) > 0)
                    {
                        scale = (short)Marshal.PtrToStructure(c_glyph_ptr, typeof(short));
                        scale = SwapInt16(scale);
                        float fScale = scale / (float)F2Dot14;
                        c_glyph_ptr = Increment(c_glyph_ptr, Marshal.SizeOf(typeof(ushort)));
                        m00 = fScale;
                        m11 = fScale;
                    }
                    else if ((cgheader.flags & 0x40) > 0)
                    {
                        scale = (short)Marshal.PtrToStructure(c_glyph_ptr, typeof(short));
                        scale = SwapInt16(scale);
                        float fXScale = scale / (float)F2Dot14;
                        c_glyph_ptr = Increment(c_glyph_ptr, Marshal.SizeOf(typeof(ushort)));
                        scale = (short)Marshal.PtrToStructure(c_glyph_ptr, typeof(short));
                        scale = SwapInt16(scale);
                        float fYScale = scale / (float)F2Dot14;
                        c_glyph_ptr = Increment(c_glyph_ptr, Marshal.SizeOf(typeof(ushort)));

                        m00 = fXScale;
                        m11 = fYScale;
                    }
                    else if ((cgheader.flags & 0x80) > 0)
                    {
                        scale = (short)Marshal.PtrToStructure(c_glyph_ptr, typeof(short));
                        scale = SwapInt16(scale);
                        float f00Scale = scale / (float)F2Dot14;
                        c_glyph_ptr = Increment(c_glyph_ptr, Marshal.SizeOf(typeof(ushort)));
                        scale = (short)Marshal.PtrToStructure(c_glyph_ptr, typeof(short));
                        scale = SwapInt16(scale);
                        float f01Scale = scale / (float)F2Dot14;
                        c_glyph_ptr = Increment(c_glyph_ptr, Marshal.SizeOf(typeof(ushort)));
                        scale = (short)Marshal.PtrToStructure(c_glyph_ptr, typeof(short));
                        scale = SwapInt16(scale);
                        float f10Scale = scale / (float)F2Dot14;
                        c_glyph_ptr = Increment(c_glyph_ptr, Marshal.SizeOf(typeof(ushort)));
                        scale = (short)Marshal.PtrToStructure(c_glyph_ptr, typeof(short));
                        scale = SwapInt16(scale);
                        float f11Scale = scale / (float)F2Dot14;
                        c_glyph_ptr = Increment(c_glyph_ptr, Marshal.SizeOf(typeof(ushort)));

                        m00 = f00Scale;
                        m11 = f11Scale;
                        m01 = f01Scale;
                        m10 = f10Scale;
                    }
                    aPath.Transform(m00, m01, m10, m11, m02 + position.X, m12 + position.Y);
                    result.AddPath(aPath, false);
                } while ((cgheader.flags & 0x20) > 0);

                if ((cgheader.flags & 0x100) > 0)
                {
                    ushort aInstructions_count = SwapUInt16((ushort)Marshal.PtrToStructure(c_glyph_ptr, typeof(ushort)));
                    c_glyph_ptr = Increment(c_glyph_ptr, sizeof(ushort));

                    byte[] aInstructions = new byte[aInstructions_count];
                    Marshal.Copy(c_glyph_ptr, aInstructions, 0, (int)aInstructions.Length);
                    //ptr = Increment(ptr, instructions.Length);
                }
                return result;
            }
            ushort[] endPtsOfContours = new ushort[gheader.numberOfContours];
            IntPtr ptr = Increment(glyph_ptr, Marshal.SizeOf(gheader));

            for (int i = 0; i < endPtsOfContours.Length; i++)
            {
                endPtsOfContours[i] = SwapUInt16((ushort)Marshal.PtrToStructure(ptr, typeof(ushort)));
                ptr = Increment(ptr, sizeof(ushort));
            }
            instructions_count = SwapUInt16((ushort)Marshal.PtrToStructure(ptr, typeof(ushort)));
            ptr = Increment(ptr, sizeof(ushort));

            byte[] instructions = new byte[instructions_count];
            Marshal.Copy(ptr, instructions, 0, (int)instructions.Length);
            ptr = Increment(ptr, instructions.Length);

            int number_of_points = endPtsOfContours[endPtsOfContours.Length - 1] + 1;

            byte[] flags = new byte[number_of_points];
            GlyphPoint[] points = new GlyphPoint[number_of_points];

            byte repeatCount = 0;
            byte repeatFlag = 0;

            for (int i = 0; i < number_of_points; i++)
            {
                if (repeatCount > 0)
                {
                    flags[i] = repeatFlag;
                    repeatCount--;
                }
                else
                {
                    ptr = ReadRawByte(ptr, out flags[i]);
                    if ((flags[i] & (byte)GlyphFlags.REPEAT) != 0)
                    {
                        ptr = ReadRawByte(ptr, out repeatCount);
                        repeatFlag = flags[i];
                    }
                }

                points[i] = new GlyphPoint();
                points[i].on_curve = (flags[i] & (byte)GlyphFlags.ON_CURVE) != 0;
            }

            for (int i = 0; i < endPtsOfContours.Length; i++)
            {
                points[endPtsOfContours[i]].end_of_contour = true;
            }

            short last = 0;
            for (int i = 0; i < number_of_points; i++)
            {
                byte val;
                bool sign = (flags[i] & (byte)GlyphFlags.X_POSITIVE) != 0;

                if ((flags[i] & (byte)GlyphFlags.X_SHORT) != 0)
                {
                    val = Marshal.ReadByte(ptr);
                    ptr = Increment(ptr, 1);
                    last += (short)(sign ? val : -val);
                }
                else
                {
                    if (!sign)
                    {
                        last += SwapInt16(Marshal.ReadInt16(ptr));
                        ptr = Increment(ptr, 2);
                    }
                }
                points[i].x = last / font_rsize;
            }

            last = 0;
            for (int i = 0; i < number_of_points; i++)
            {
                byte val;
                bool sign = (flags[i] & (byte)GlyphFlags.Y_POSITIVE) != 0;

                if ((flags[i] & (byte)GlyphFlags.Y_SHORT) != 0)
                {
                    val = Marshal.ReadByte(ptr);
                    ptr = Increment(ptr, 1);
                    last += (short)(sign ? val : -val);
                }
                else
                {
                    if (!sign)
                    {
                        last += SwapInt16(Marshal.ReadInt16(ptr));
                        ptr = Increment(ptr, 2);
                    }
                }
                points[i].y = last / font_rsize;
            }

            // Draw glyphs outline
            bool start_new_contour = true;
            int idx = 0;
            FastGraphicsPath path = new FastGraphicsPath(FastFillMode.Winding);
            GlyphPoint first_point;

            first_point = points[idx];

            start_new_contour = true;
            PointF beg, first, end, next, implied;
            bool curent_on_curve, next_on_curve;

            first = beg = new PointF(points[0].Point.X + position.X, position.Y - points[0].Point.Y);

            for (idx = 0; idx < points.Length; idx++)
            {
                curent_on_curve = points[idx].on_curve;
                if (idx + 1 < points.Length)
                {
                    next = new PointF(points[idx + 1].Point.X + position.X, position.Y - points[idx + 1].Point.Y);
                    next_on_curve = points[idx + 1].on_curve;
                }
                else
                {
                    next = new PointF(points[0].Point.X + position.X, position.Y - points[0].Point.Y);
                    next_on_curve = points[0].on_curve;
                }

                if (start_new_contour == true)
                {
                    path.StartFigure();
                    first = beg;
                    start_new_contour = false;
                }

                if (points[idx].end_of_contour)
                {
                    start_new_contour = true;
                    //path.CloseFigure();

                    implied = new PointF(points[idx].Point.X + position.X, position.Y - points[idx].Point.Y);
                    end = first;
                    if (curent_on_curve)
                    {
                        //end = next;
                        path.AddLine(beg, end);
                    }
                    else
                    {
                        AddSpline(path, beg, implied, end, position);
                    }
                    beg = next;
                    continue;
                }

                ////////////////////////////////////////////////////////////////////
                if (curent_on_curve) // 
                {
                    if (next_on_curve) // 
                    {
                        end = next;
                        path.AddLine(beg, end);
                        beg = end;
                    }
                    else
                    {
                        // 
                    }
                }
                else // 
                {
                    implied = new PointF(points[idx].Point.X + position.X, position.Y - points[idx].Point.Y);
                    if (next_on_curve) // 
                    {
                        end = next;
                        AddSpline(path, beg, implied, end, position);
                        beg = end;
                    }
                    else
                    {
                        float X = position.X + ((points[idx + 1].x - points[idx].x) / 2) + points[idx].x;
                        float Y = position.Y - (((points[idx + 1].y - points[idx].y) / 2) + points[idx].y);
                        end = new PointF(X, Y);
                        AddSpline(path, beg, implied, end, position);
                        beg = end;
                    }
                }
            }
            return path;
        }

        private void AddSpline(FastGraphicsPath path, PointF pntStart, PointF pntB, PointF pntEnd, PointF position)
        {
            // Start and end points are unmodified.
            PointF pnt1 = pntStart;        // Ïåðâàÿ êîíòðîëüíàÿ òî÷êà Áåçüå
            pnt1.X += (2.0f / 3.0f) * (pntB.X - pntStart.X);
            pnt1.Y += (2.0f / 3.0f) * (pntB.Y - pntStart.Y);

            PointF pnt2 = pntB;            // Âòîðàÿ êîíòðîëüíàÿ òî÷êà Áåçüå
            pnt2.X += (pntEnd.X - pntB.X) / 3.0f;
            pnt2.Y += (pntEnd.Y - pntB.Y) / 3.0f;

            path.AddBezier(pntStart, pnt1, pnt2, pntEnd);
        }

        public GlyphTableClass(TrueTypeTable src) : base(src)
        {
        }

        internal GlyphTableClass(GlyphTableClass parent, IntPtr glyphs) : base(parent)
        {
            glyph_table_ptr = glyphs;
        }

        public IntPtr GlypsPtr { get { return glyph_table_ptr; } }
    }

}

#pragma warning restore