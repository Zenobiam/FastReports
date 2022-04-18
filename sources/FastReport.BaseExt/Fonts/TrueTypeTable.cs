using System;
using System.Runtime.InteropServices;

#pragma warning disable CS3001, CS3002, CS3003, CS3005, CS1591

namespace FastReport.Fonts
{
    /// <summary>
    /// Base class which is parent of any table in TrueType font or collection
    /// </summary>
    public class TrueTypeTable : TTF_Helpers
    {
        #region "Type definitions"
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct TableEntry
        {
            [FieldOffset(0)]
            public uint tag;
            [FieldOffset(4)]
            public uint checkSum;
            [FieldOffset(8)]
            public uint offset;
            [FieldOffset(12)]
            public uint length;
        }
        #endregion

        private TableEntry i_entry;

        private uint checksum;
        private uint offset;
        private uint length;


        #region "Public properties"
        public string TAG
        {
            get
            {
                return "" +
                    (char)(0xff & i_entry.tag) +
                    (char)(0xff & (i_entry.tag >> 8)) +
                    (char)(0xff & (i_entry.tag >> 16)) +
                    (char)(0xff & (i_entry.tag >> 24));
            }
        }
        public uint tag { get { return i_entry.tag; } }
        public uint Length { get { return i_entry.length; } }
        public uint Offset
        {
            get { return i_entry.offset; }
            set
            { offset = value; }
        }
        public uint checkSum
        {
            get { return i_entry.checkSum; }
            set
            { checksum = value; }
        }
        public int descriptor_size { get { return Marshal.SizeOf(i_entry); } }
        public void SetLenght(uint len)
        { length = len; }
        public uint PackedOfffset { get { return offset; } }
        #endregion

        private uint StoreTable(IntPtr source_ptr, IntPtr destination_ptr, uint output_offset)
        {
            int len = (int)((length + 3) / 4);

            IntPtr src = Increment(source_ptr, (int)offset);
            IntPtr dst = Increment(destination_ptr, (int)output_offset);

            if (src != dst)
            {
                int[] buffer = new int[len];
                Marshal.Copy(src, buffer, 0, len);
                Marshal.Copy(buffer, 0, dst, len);
                buffer = null;

                offset = output_offset;
            }
            else
            {
                ;
            }
            output_offset += (uint)(len * 4);
            return output_offset;
        }

        internal virtual void Load(IntPtr font)
        {
            // Do not parse this table
        }

        internal virtual uint Save(IntPtr font, uint offset)
        {
            uint table_size = StoreTable(font, font, offset);
            return table_size;
        }

        internal virtual uint Save(IntPtr src, IntPtr dst, uint offset)
        {
            uint table_size = StoreTable(src, dst, offset);
            return table_size;
        }

        internal IntPtr StoreDescriptor(IntPtr descriptor_ptr)
        {
            TableEntry o_entry;

            o_entry.tag = i_entry.tag;
            o_entry.checkSum = SwapUInt32(checkSum);
            o_entry.offset = SwapUInt32(offset);    
            o_entry.length = SwapUInt32(length);

            Marshal.StructureToPtr(o_entry, descriptor_ptr, false);
            return Increment(descriptor_ptr, Marshal.SizeOf(o_entry));
        }

        public TrueTypeTable(TrueTypeTable parent)
        {
            if (parent != null)
            {
                this.i_entry = parent.i_entry;
                this.checksum = parent.checkSum;
                this.offset = parent.offset;
                this.length = parent.length;
            }
        }

        public TrueTypeTable(IntPtr entry_ptr)
        {
            i_entry = (TableEntry)Marshal.PtrToStructure(entry_ptr, typeof(TableEntry));
            i_entry.checkSum = SwapUInt32(i_entry.checkSum);
            i_entry.offset = SwapUInt32(i_entry.offset);
            i_entry.length = SwapUInt32(i_entry.length);

            checksum = i_entry.checkSum;
            offset = i_entry.offset;
            length = i_entry.length;
        }
    }

}

#pragma warning restore