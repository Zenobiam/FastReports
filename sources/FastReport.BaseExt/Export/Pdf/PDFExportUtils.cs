﻿using FastReport.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace FastReport.Export.Pdf
{
    public partial class PDFExport : ExportBase
    {
        private float GetTop(float p)
        {
            return marginWoBottom - p * PDF_DIVIDER;
        }

        private float GetLeft(float p)
        {
            return marginLeft + p * PDF_DIVIDER;
        }

        private string FloatToString(double value)
        {
            return Convert.ToString(Math.Round(value, 2), numberFormatInfo);
        }

        private string FloatToStringSmart(double value)
        {
            if (value > 1 || value < -1)
                return FloatToString(value);
            if (value > 0)
            {
                double testValue = value;
                int i;
                for (i = 0; i < 6; i++)
                    if ((testValue *= 10) > 1)
                        break;
                return FloatToString(value, i + 2);
            }
            if (value < 0)
            {
                double testValue = -value;
                int i;
                for (i = 0; i < 6; i++)
                    if ((testValue *= 10) > 1)
                        break;
                return FloatToString(value, i + 2);
            }
            return "0";
        }

        private string FloatToString(double value, int digits)
        {
          return Convert.ToString(Math.Round(value, digits), numberFormatInfo);
        }

        private string StringToPdfUnicode(string s)
        {
            StringBuilder sb = new StringBuilder(s.Length * 2 + 2);
            sb.Append((char)254).Append((char)255);
            foreach (char c in s)
                sb.Append((char)(c >> 8)).Append((char)(c & 0xFF));
            return sb.ToString();
        }

        private void PrepareString(string text, byte[] key, bool encode, long id, StringBuilder sb)
        {
            string s = encode ? RC4CryptString(StringToPdfUnicode(text), key, id) : StringToPdfUnicode(text);

            if (PdfCompliance == PdfStandard.PdfA_1a)
            {
                s = encode ? RC4CryptString(text, key, id) : text;
            }

            sb.Append("(");
            EscapeSpecialChar(s, sb);
            sb.Append(")");
        }

        private void Write(Stream stream, string value)
        {
            stream.Write(ExportUtils.StringToByteArray(value), 0, value.Length);
        }

        private void WriteLn(Stream stream, string value)
        {
            stream.Write(ExportUtils.StringToByteArray(value), 0, value.Length);
            stream.WriteByte(0x0d);
            stream.WriteByte(0x0a);
        }

        private void StrToUTF16(string str, StringBuilder sb)
        {
            if (!string.IsNullOrEmpty(str))
            {
                sb.Append("FEFF");
                foreach (char c in str)
                    sb.Append(((int)c).ToString("X4"));
            }
        }

        private void EscapeSpecialChar(string input, StringBuilder sb)
        {
            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '(':
                        sb.Append(@"\(");
                        break;
                    case ')':
                        sb.Append(@"\)");
                        break;
                    case '\\':
                        sb.Append(@"\\");
                        break;
                    case '\r':
                        sb.Append(@"\r");
                        break;
                    case '\n':
                        sb.Append(@"\n");
                        break;
                    default:
                        sb.Append(input[i]);
                        break;
                }
            }
        }

        private float GetBaseline(Font f)
        {
            float baselineOffset = f.SizeInPoints / f.FontFamily.GetEmHeight(f.Style) * f.FontFamily.GetCellAscent(f.Style);
            return DrawUtils.ScreenDpi / 72f * baselineOffset;
        }

        private void GetPDFFillColor(Color color, StringBuilder sb)
        {
            GetPDFFillTransparent(color, sb);
            if (ColorSpace == PdfColorSpace.CMYK)
            {
                GetCMYKColor(color, sb);
                sb.AppendLine(" k");
            }
            else if (ColorSpace == PdfColorSpace.RGB)
            {
                GetPDFColor(color, sb);
                sb.AppendLine(" rg");
            }
        }

        private void GetPDFFillTransparent(Color color, StringBuilder sb)
        {
            string value = FloatToString((float)color.A / 255f);

            if (PdfCompliance == PdfStandard.PdfA_1a)
            {
                value = "1";
            }

            int i = trasparentStroke.IndexOf(value);
            if (i == -1)
            {
                trasparentStroke.Add(value);
                i = trasparentStroke.Count - 1;
            }

            sb.Append("/GS").Append(i.ToString()).AppendLine("S gs");
        }

        private void GetPDFStrokeColor(Color color, StringBuilder sb)
        {
            GetPDFStrokeTransparent(color, sb);
            if (ColorSpace == PdfColorSpace.CMYK)
            {
                GetCMYKColor(color, sb);
                sb.AppendLine(" K");
            }
            else if (ColorSpace == PdfColorSpace.RGB)
            {
                GetPDFColor(color, sb);
                sb.AppendLine(" RG");
            }
        }

        private void GetPDFStrokeTransparent(Color color, StringBuilder sb)
        {
            string value = FloatToString((float)color.A / 255f);

            if (PdfCompliance == PdfStandard.PdfA_1a)
            {
                value = "1";
            }

            int i = trasparentFill.IndexOf(value);
            if (i == -1)
            {
                trasparentFill.Add(value);
                i = trasparentFill.Count - 1;
            }

            sb.Append("/GS").Append(i.ToString()).AppendLine("F gs");
        }

        private void GetPDFColor(Color color, StringBuilder sb)
        {
            if (color == Color.Black)
                sb.Append("0 0 0");
            else if (color == Color.White)
                sb.Append("1 1 1");
            else
            {
                sb.Append(FloatToString((float)color.R / 255f, 3)).Append(" ").
                    Append(FloatToString((float)color.G / 255f, 3)).Append(" ").
                    Append(FloatToString((float)color.B / 255f, 3));
            }
        }

        private void GetCMYKColor(Color color, StringBuilder sb)
        {
            if (color == Color.Black)
                sb.Append("0 0 0 1");
            else if (color == Color.White)
                sb.Append("0 0 0 0");
            else
            {
                CMYKColor cmyk = new CMYKColor(color);
                sb.Append(FloatToString((float)cmyk.c / 100f, 3)).Append(" ").
                    Append(FloatToString((float)cmyk.m / 100f, 3)).Append(" ").
                    Append(FloatToString((float)cmyk.y / 100f, 3)).Append(" ").
                    Append(FloatToString((float)cmyk.k / 100f, 3));
            }
        }

        private bool FontEquals(Font font1, Font font2)
        {
            // 20200415 - Underline and strikeout styles should not be considered as different fonts
            FontStyle style1 = font1.Style & (FontStyle.Regular | FontStyle.Bold | FontStyle.Italic);
            FontStyle style2 = font2.Style & (FontStyle.Regular | FontStyle.Bold | FontStyle.Italic);
            return (font1.Name == font2.Name) && style1.Equals(style2);
        }

        private string PrepXRefPos(long p)
        {
            string pos = p.ToString();
            return new string('0', 10 - pos.Length) + pos;
        }

        private string ObjNumber(long FNumber)
        {
            return String.Concat(FNumber.ToString(), " 0 obj");
        }

        private string ObjNumberRef(long FNumber)
        {
            return String.Concat(FNumber.ToString(), " 0 R");
        }

        private long UpdateXRef()
        {
            xRef.Add(pdf.Position);
            return xRef.Count;
        }

        /// <summary>
        /// Update stream position for object number, only for int value
        /// </summary>
        /// <param name="objectNumber">int value</param>
        private void UpdateXRef(long objectNumber)
        {
            xRef[(int)(objectNumber - 1)] = pdf.Position;
        }

        private void WritePDFStream(Stream target, Stream source, long id, bool compress, bool encrypt, bool startingBrackets, bool endingBrackets)
        {
            WritePDFStream(target, source, id, compress, encrypt, startingBrackets, endingBrackets, false);
        }

        private void WritePDFStream(Stream target, Stream source, long id, bool compress, bool encrypt, bool startingBrackets, bool endingBrackets, bool enableLength1)
        {
            MemoryStream tempStream;

            if (startingBrackets)
                WriteLn(target, "<<");

            using (tempStream = new MemoryStream())
            {
                if (compress)
                {
                    ExportUtils.ZLibDeflate(source, tempStream);
                    WriteLn(pdf, "/Filter /FlateDecode");
                }
                else
                {
                    source.CopyTo(tempStream, (int)source.Length);
                    tempStream.Position = 0;
                }

                WriteLn(pdf, "/Length " + tempStream.Length.ToString());

                if (enableLength1)
                    WriteLn(pdf, "/Length1 " + source.Length.ToString());

                if (endingBrackets)
                    WriteLn(target, ">>");
                else
                    WriteLn(target, "");

                WriteLn(target, "stream");

                if (encrypt)
                    RC4CryptStream(tempStream, target, encKey, id);
                else
                    tempStream.WriteTo(target);

                target.WriteByte(0x0a);
                WriteLn(target, "endstream");
                WriteLn(target, "endobj");
            }
        }

    }
}
