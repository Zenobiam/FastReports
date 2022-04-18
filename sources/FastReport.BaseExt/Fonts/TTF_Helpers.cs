using System;

#pragma warning disable

namespace FastReport.Fonts
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TTF_Helpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static ushort SwapUInt16(ushort v)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] buf = BitConverter.GetBytes(v);
                Array.Reverse(buf);
                return BitConverter.ToUInt16(buf, 0);
            }
            else
                return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static short SwapInt16(short v)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] buf = BitConverter.GetBytes(v);
                Array.Reverse(buf);
                return BitConverter.ToInt16(buf, 0);
            }
            else
                return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static uint SwapUInt32(uint v)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] buf = BitConverter.GetBytes(v);
                Array.Reverse(buf);
                return BitConverter.ToUInt32(buf, 0);
            }
            else
                return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static int SwapInt32(int v)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] buf = BitConverter.GetBytes(v);
                Array.Reverse(buf);
                return BitConverter.ToInt32(buf, 0);
            }
            else
                return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static ulong SwapUInt64(ulong v)
        {
          if (BitConverter.IsLittleEndian)
          {
            byte[] buf = BitConverter.GetBytes(v);
            Array.Reverse(buf);
            return BitConverter.ToUInt64(buf, 0);
          }
          else
            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="cbSize"></param>
        /// <returns></returns>
        public static IntPtr Increment(IntPtr ptr, int cbSize)
        {
            return new IntPtr(ptr.ToInt64() + cbSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="cbSize"></param>
        /// <returns></returns>
        public static IntPtr Increment(IntPtr ptr, uint cbSize)
        {
            return new IntPtr(ptr.ToInt64() + cbSize);
        }
    }

}
#pragma warning restore