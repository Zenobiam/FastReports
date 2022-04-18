﻿using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace FastReport.Utils
{
    /// <summary>
    /// Class for ACMYK color conversions
    /// </summary>
    public class CMYKColor
    {
        /// <summary>
        /// Alpha transparency 0..255
        /// </summary>
        public byte a;
        /// <summary>
        /// Cyan 0..100
        /// </summary>
        public byte c;
        /// <summary>
        /// Magenta 0..100
        /// </summary>
        public byte m;
        /// <summary>
        /// Yellow 0..100
        /// </summary>
        public byte y;
        /// <summary>
        /// Black 0..100
        /// </summary>
        public byte k;

        /// <summary>
        /// Returns ACMYK as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Concat(a.ToString(), ", ", c.ToString(), ", ", m.ToString(), ", ", y.ToString(), ", ", c.ToString());
        }

        /// <summary>
        /// Gets CMYKA from string.
        /// </summary>
        /// <param name="color"></param>
        public void FromString(string color)
        {
            a = c = m = y = k = 0;
            string[] values = color.Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                switch(i)
                {
                    case 0:
                        a = Convert.ToByte(values[i]);
                        break;
                    case 1:
                        c = Convert.ToByte(values[i]);
                        break;
                    case 2:
                        m = Convert.ToByte(values[i]);
                        break;
                    case 3:
                        y = Convert.ToByte(values[i]);
                        break;
                    case 4:
                        c = Convert.ToByte(values[i]);
                        break;
                }
            }
        }

        /// <summary>
        /// Converts Color value to ACMYK
        /// </summary>
        /// <param name="color"></param>
        public void FromColor(Color color)
        {
            float r = ((float)color.R) / 255f;
            float g = ((float)color.G) / 255f;
            float b = ((float)color.B) / 255f;
            float k = 1 - Math.Max(Math.Max(r, g), b);
            float c = (1 - r - k) / (1 - k);
            float m = (1 - g - k) / (1 - k);
            float y = (1 - b - k) / (1 - k);
            this.c = (byte)Math.Round(c * 100);
            this.m = (byte)Math.Round(m * 100);
            this.y = (byte)Math.Round(y * 100);
            this.k = (byte)Math.Round(k * 100);
            a = color.A;
        }

        /// <summary>
        /// Converts separate ARGB values in ACMYK
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="R"></param>
        /// <param name="G"></param>
        /// <param name="B"></param>
        public void FromARGB(byte alpha, byte R, byte G, byte B)
        {
            FromColor(Color.FromArgb(alpha, R, G, B));
        }

        /// <summary>
        /// Returns ARGB color value
        /// </summary>
        /// <returns></returns>
        public Color ToArgb()
        {            
            float c = this.c / 100;
            float m = this.m / 100;
            float y = this.y / 100;
            float k = this.k / 100;                       
            byte r = (byte)Math.Round(255 * (1 - c) * (1 - k));
            byte g = (byte)Math.Round(255 * (1 - m) * (1 - k));
            byte b = (byte)Math.Round(255 * (1 - y) * (1 - k));            
            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Creates CMYKColor from ARGB values
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public CMYKColor(byte alpha, byte r, byte g, byte b)
        {
            FromARGB(alpha, r, g, b);
        }

        /// <summary>
        /// Creates CMYKColor from ACMYK values
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="c"></param>
        /// <param name="m"></param>
        /// <param name="y"></param>
        /// <param name="k"></param>
        public CMYKColor(byte alpha, byte c, byte m, byte y, byte k)
        {
            a = alpha;
            this.c = c;
            this.m = m;
            this.y = y;
            this.k = k;
        }

        /// <summary>
        /// Creates CMYKColor from string (comma separated values)
        /// </summary>
        /// <param name="acmykString"></param>
        public CMYKColor(string acmykString)
        {
            FromString(acmykString);
        }

        /// <summary>
        /// Creates CMYKColor from Color value
        /// </summary>
        /// <param name="color"></param>
        public CMYKColor(Color color)
        {
            FromColor(color);
        }
    }

    /// <summary>
    /// Color Utilities
    /// </summary>
    public static class ColorUtils
    {
        /// <summary>
        /// Return true for CMYK Jpeg image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static bool isCMYKJpeg(Image image)
        {
            ImageFlags flagValues = (ImageFlags)Enum.Parse(typeof(ImageFlags), image.Flags.ToString());
            return flagValues.ToString().ToLower().IndexOf("ycck") != -1;
        }
    }
}
