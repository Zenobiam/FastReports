using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;

namespace FastReport.Export.Pdf
{
    public partial class PDFExport : ExportBase
    {
        byte[] pDF_PK = { 
            0x28, 0xBF, 0x4E, 0x5E, 0x4E, 0x75, 0x8A, 
            0x41, 0x64, 0x00, 0x4E, 0x56, 0xFF, 0xFA, 
            0x01, 0x08, 0x2E, 0x2E, 0x00, 0xB6, 0xD0, 
            0x68, 0x3E, 0x80, 0x2F, 0x0C, 0xA9, 0xFE, 
            0x64, 0x53, 0x69, 0x7A };

        private long encBits;
        private byte[] encKey;
        private byte[] oPass;
        private byte[] uPass;

        private string RC4CryptString(string source, byte[] key, long id)
        {
            byte[] k = new byte[21];
            Array.Copy(key, 0, k, 0, 16);
            k[16] = (byte)id;
            k[17] = (byte)(id >> 8);
            k[18] = (byte)(id >> 16);
            byte[] s = new byte[16];
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            Array.Copy(md5.ComputeHash(k), s, 16);
            RC4 rc4 = new RC4();
            rc4.Start(s);
            byte[] src = ExportUtils.StringToByteArray(source);
            byte[] target = rc4.Crypt(src);
            return ExportUtils.StringFromByteArray(target);
        }

        private void RC4CryptStream(Stream source, Stream target, byte[] key, long id)
        {
            byte[] k = new byte[21];
            Array.Copy(key, 0, k, 0, 16);
            k[16] = (byte)id;
            k[17] = (byte)(id >> 8);
            k[18] = (byte)(id >> 16);

            byte[] s = new byte[16];
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            Array.Copy(md5.ComputeHash(k), s, 16);

            byte[] buffSource = new byte[source.Length];
            source.Position = 0;
            source.Read(buffSource, 0, (int)source.Length);

            RC4 rc4 = new RC4();
            rc4.Start(s);
            byte[] buffTarget = rc4.Crypt(buffSource);
            target.Write(buffTarget, 0, buffTarget.Length);
        }

        private byte[] PadPassword(string password)
        {
            byte[] p = ExportUtils.StringToByteArray(password);
            byte[] result = new byte[32];
            int l = p.Length < 32 ? p.Length : 32;
            for (int i = 0; i < l; i++)
                result[i] = p[i];
            if (l < 32)
                for (int i = l; i < 32; i++)
                    result[i] = pDF_PK[i - l];
            return result;
        }

        private void PrepareKeys()
        {
            encBits = -64; // 0xFFFFFFC0;
            if (allowPrint)
                encBits += 4;
            if (allowModify)
                encBits += 8;
            if (allowCopy)
                encBits += 16;
            if (allowAnnotate)
                encBits += 32;

            // OWNER KEY            
            if (String.IsNullOrEmpty(ownerPassword))
                ownerPassword = userPassword;

            byte[] p = PadPassword(ownerPassword);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            md5.Initialize();
            byte[] s = new byte[16];
            md5.TransformBlock(p, 0, 32, p, 0);
            md5.TransformFinalBlock(p, 0, 0);
            Array.Copy(md5.Hash, s, 16);
            for (byte i = 1; i <= 50; i++)
            {
                md5.Initialize();
                Array.Copy(md5.ComputeHash(s), 0, s, 0, 16);
            }

            RC4 rc4 = new RC4();
            p = PadPassword(userPassword);
            rc4.Start(s);
            byte[] s1 = rc4.Crypt(p);
            byte[] p1 = new byte[16];
            for (byte i = 1; i <= 19; i++)
            {
                for (byte j = 1; j <= 16; j++)
                    p1[j - 1] = (byte)(s[j - 1] ^ i);
                rc4.Start(p1);
                s1 = rc4.Crypt(s1);
            }
            oPass = new byte[32];
            Array.Copy(s1, oPass, 32);

            // ENCRYPTION KEY
            p = PadPassword(userPassword);

            md5.Initialize();
            md5.TransformBlock(p, 0, 32, p, 0);
            md5.TransformBlock(oPass, 0, 32, oPass, 0);

            byte[] ext = new byte[4];
            ext[0] = (byte)encBits;
            ext[1] = (byte)(encBits >> 8);
            ext[2] = (byte)(encBits >> 16);
            ext[3] = (byte)(encBits >> 24);
            md5.TransformBlock(ext, 0, 4, ext, 0);

            byte[] fid = new byte[16];
            for (byte i = 1; i <= 16; i++)
                fid[i - 1] = Convert.ToByte(String.Concat(fileID[i * 2 - 2], fileID[i * 2 - 1]), 16);
            md5.TransformBlock(fid, 0, 16, fid, 0);
            md5.TransformFinalBlock(ext, 0, 0);
            Array.Copy(md5.Hash, 0, s, 0, 16);

            for (byte i = 1; i <= 50; i++)
            {
                md5.Initialize();
                Array.Copy(md5.ComputeHash(s), 0, s, 0, 16);
            }
            encKey = new byte[16];
            Array.Copy(s, 0, encKey, 0, 16);

            // USER KEY
            md5.Initialize();
            md5.TransformBlock(pDF_PK, 0, 32, pDF_PK, 0);
            md5.TransformBlock(fid, 0, 16, fid, 0);
            md5.TransformFinalBlock(fid, 0, 0);
            Array.Copy(md5.Hash, s, 16);

            s1 = new byte[16];
            Array.Copy(encKey, s1, 16);

            rc4.Start(s1);
            s = rc4.Crypt(s);

            p1 = new byte[16];
            for (byte i = 1; i <= 19; i++)
            {
                for (byte j = 1; j <= 16; j++)
                    p1[j - 1] = (byte)(s1[j - 1] ^ i);
                rc4.Start(p1);
                s = rc4.Crypt(s);
            }
            uPass = new byte[32];
            Array.Copy(s, 0, uPass, 0, 16);
        }

        private string GetEncryptionDescriptor()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/Encrypt <<");
            sb.AppendLine("/Filter /Standard");
            sb.AppendLine("/V 2");
            sb.AppendLine("/R 3");
            sb.AppendLine("/Length 128");
            sb.AppendLine("/P " + encBits.ToString());
            sb.Append("/O (");
            EscapeSpecialChar(ExportUtils.StringFromByteArray(oPass), sb);
            sb.AppendLine(")");
            sb.Append("/U (");
            EscapeSpecialChar(ExportUtils.StringFromByteArray(uPass), sb);
            sb.AppendLine(")");
            sb.AppendLine(">>");
            return sb.ToString();
        }

        #region DigitalSignature

        private struct SignatureDictIndicies
        {
            public long byteRangeIndex;
            public long contentsIndex;
        }

        private byte[] MSSign(X509Certificate2 cert, byte[] data)
        {
            ContentInfo content = new ContentInfo(new Oid("1.2.840.113549.1.7.1"), data);
            SignedCms signedCms = new SignedCms(content, true);
            
            CmsSigner signer = new CmsSigner(cert);
            //signer.DigestAlgorithm = new Oid(CryptoConfig.MapNameToOID("SHA256"));

            signedCms.ComputeSignature(signer);

            return signedCms.Encode();
        }

        private SignatureDictIndicies AddSignatureDict(ReportComponentBase report_obj)
        {
            SignatureDictIndicies res = new SignatureDictIndicies();
            if (digitalSignCertificate != null && report_obj == null)
            {
                int sigSize = 16384;
                long objNo = UpdateXRef();
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(ObjNumber(objNo));
                sb.Append("<<");
                sb.Append("/Type/Sig");
                sb.Append("/Filter/Adobe.PPKLite");
                sb.Append("/SubFilter/adbe.pkcs7.detached");

                sb.Append("/Reason");
                if (digitalSignReason == null)
                    digitalSignReason = "";
                PrepareString(digitalSignReason, encKey, encrypted, objNo, sb);

                sb.Append("/Location");
                if (digitalSignLocation == null)
                    digitalSignLocation = "";
                PrepareString(digitalSignLocation, encKey, encrypted, objNo, sb);

                sb.Append("/ContactInfo");
                if (digitalSignContactInfo == null)
                    digitalSignContactInfo = "";
                PrepareString(digitalSignContactInfo, encKey, encrypted, objNo, sb);

                sb.Append("/PropBuild<</App<<");
                sb.Append("/Name");
                PrepareString("FastReport.Net", encKey, encrypted, objNo, sb);
                sb.Append(">>>>");

                string date = "D:" + digitalSignCreationDate.ToString("yyyyMMddHHmmss");
                sb.Append("/M(").Append(date).Append(")");
                //PrepareString(date, encKey, encrypted, infoNumber, sb);

                sb.Append("/ByteRange [");
                res.byteRangeIndex = sb.Length + xRef[(int)objNo - 1];
                sb.Append(new string(' ', 81));
                sb.Append("/Contents<");
                res.contentsIndex = sb.Length + xRef[(int)objNo - 1];
                sb.Append(new string('0', sigSize));
                sb.AppendLine(">>>");
                sb.AppendLine("endobj");

                Write(pdf, sb.ToString());
                
                AddSignatureAppearence(objNo, report_obj);
            }
            else
            {
                AddSignatureAppearence(-1, report_obj);
            }

            return res;
        }

        private long AddSignatureAppearenceLayer(string right, string top)
        {
            return AddSignatureAppearenceLayer(right, top, "% DSBlank\n");
        }

        private long AddSignatureAppearenceLayer(string right, string top, string streamContent)
        {
            long res = UpdateXRef();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(ObjNumber(res));
            sb.Append("<<");
            sb.Append("/Type/XObject");
            sb.Append("/Subtype/Form");
            sb.Append("/Resources<<>>");
            sb.Append(String.Format("/BBox[0 0 {0} {1}]", right, top));
            sb.Append("/FormType 1");
            sb.AppendLine("/Matrix [1 0 0 1 0 0]");

            Write(pdf, sb.ToString());
            using (MemoryStream ms = new MemoryStream(ExportUtils.StringToByteArray(streamContent)))
            {
                WritePDFStream(pdf, ms, 0, compressed, encrypted, false, true);
            }

            return res;
        }

        private long AddSignatureAppearenceLayersContainer(string right, string top)
        {
            long layerN0 = AddSignatureAppearenceLayer(right, top);
            long layerN2 = AddSignatureAppearenceLayer(right, top);

            long res = UpdateXRef();
            StringBuilder sb = new StringBuilder();

            sb.Append("<<");
            sb.Append("/Type/XObject");
            sb.Append("/Subtype/Form");
            sb.Append("/Resources<</XObject << /n0" + ObjNumberRef(layerN0) +
                " /n2" + ObjNumberRef(layerN2) + ">>>>");
            sb.Append(String.Format("/BBox[0 0 {0} {1}]", right, top));
            sb.Append("/FormType 1");
            sb.AppendLine("/Matrix [1 0 0 1 0 0]");

            Write(pdf, sb.ToString());
            using (MemoryStream ms = new MemoryStream(ExportUtils.StringToByteArray("q 1 0 0 1 0 0 cm /n0 Do Q\nq 1 0 0 1 0 0 cm /n2 Do Q\n")))
            {
                WritePDFStream(pdf, ms, 0, compressed, encrypted, false, true);
            }

            return res;
        }

        private long AddEmptyAppearence()
        {
            long res = UpdateXRef();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(ObjNumber(res));
            sb.Append("<<");
            sb.Append("/Type/XObject");
            sb.Append("/Subtype/Form");
            sb.Append("/Resources<<>>");
            sb.Append("/BBox[0 0 0 0]");
            sb.Append("/FormType 1");
            sb.AppendLine("/Matrix [1 0 0 1 0 0]");

            Write(pdf, sb.ToString());
            using (MemoryStream ms = new MemoryStream(ExportUtils.StringToByteArray("% DSBlank\n")))
            {
                WritePDFStream(pdf, ms, 0, compressed, encrypted, false, true);
            }

            return res;
        }

        private int signaturesCount = 0;

        private void AddSignatureAppearence(long sigObjNo, ReportComponentBase report_obj)
        {
            ++signaturesCount;
            long appearence = AddEmptyAppearence();

            long objNo = UpdateXRef();
            acroFormsRefs.Add(objNo);
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(ObjNumber(objNo));
            sb.Append("<<");
            sb.Append("/FT/Sig");
            sb.Append("/T");
            PrepareString(string.Format("Signature{0}", signaturesCount), encKey, encrypted, objNo, sb);
            if (sigObjNo >= 0)
            {
                sb.Append("/V ");
                sb.Append(ObjNumberRef(sigObjNo));
            }
            sb.Append("/F 132");
            sb.Append("/Type/Annot");
            sb.Append("/Subtype/Widget");
            if (report_obj == null)
            {
                sb.Append("/Rect[0 0 0 0]");
            }
            else
            {
                string left = FloatToString(GetLeft(report_obj.AbsLeft));
                string bottom = FloatToString(GetTop(report_obj.AbsTop + report_obj.Height));
                string right = FloatToString(GetLeft(report_obj.AbsLeft + report_obj.Width));
                string top = FloatToString(GetTop(report_obj.AbsTop));
                sb.Append(String.Format("/Rect[{0} {1} {2} {3}]", left, bottom, right, top));
            }
            UpdateXRef(objNo);
            sb.Append("/AP<</N " + ObjNumberRef(appearence) + ">>");
            pageAnnots.Append(ObjNumberRef(objNo) + " ");
            sb.Append("/DR<<>>");
            sb.AppendLine(">>");
            sb.AppendLine("endobj");

            Write(pdf, sb.ToString());
        }

        private string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(ExportUtils.ByteToHex(b).ToLower());
            }

            return sb.ToString();
        }

        private void AddSignature(X509Certificate2 cert)
        {
            pdf.Flush();

            pdf.Seek(digitalSignByteRange[0], SeekOrigin.Begin);
            byte[] data = new byte[digitalSignByteRange[1] + digitalSignByteRange[3]];
            pdf.Read(data, 0, (int)digitalSignByteRange[1]);
            pdf.Seek(digitalSignByteRange[2], SeekOrigin.Begin);
            pdf.Read(data, (int)digitalSignByteRange[1], (int)digitalSignByteRange[3]);

            if (cert != null)
            {
                byte[] signature = Encoding.ASCII.GetBytes(BytesToHex(MSSign(cert, data)));


                if (signature.Length > 16384)
                {
                    throw new OverflowException("Signature value was too big");
                }

                pdf.Flush();

                pdf.Seek(signatureDictIndicies.contentsIndex, SeekOrigin.Begin);
                pdf.Write(signature, 0, signature.Length);
            }
        }
        #endregion // DigitalSignature
    }

    internal class RC4
    {
        private byte[] fKey;

        public void Start(byte[] key)
        {
            byte[] k = new byte[256];
            int l = key.GetLength(0);
            if (key.Length > 0 && l <= 256)
            {
                for (int i = 0; i < 256; i++)
                {
                    fKey[i] = (byte)i;
                    k[i] = key[i % l];
                }
            }

            byte j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (byte)(j + fKey[i] + k[i]);
                byte tmp = fKey[i];
                fKey[i] = fKey[j];
                fKey[j] = tmp;
            }
        }

        public byte[] Crypt(byte[] source)
        {
            byte i = 0;
            byte j = 0;
            int l = source.GetLength(0);
            byte[] result = new byte[l];
            for (int k = 0; k < l; k++)
            {
                i = (byte)(i + 1);
                j = (byte)(j + fKey[i]);
                byte tmp = fKey[i];
                fKey[i] = fKey[j];
                fKey[j] = tmp;
                result[k] = (byte)(source[k] ^ fKey[(byte)(fKey[i] + fKey[j])]);
            }
            return result;
        }

        public RC4()
        {
            fKey = new byte[256];
        }
    }
}
