using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Export.Pdf
{
    public partial class PDFExport : ExportBase
    {
        private class PDFOutlineNode
        {
            public string text;
            public int page;
            public float offset;
            public long number;
            public int countTree;
            public int count;
            public PDFOutlineNode first;
            public PDFOutlineNode last;
            public PDFOutlineNode parent;
            public PDFOutlineNode prev;
            public PDFOutlineNode next;

            public PDFOutlineNode()
            {
                text = String.Empty;
                offset = -1;
                number = 0;
                count = 0;
                countTree = 0;
            }
        }

        private PDFOutlineNode outlineTree;

        private long BuildOutline(PDFOutlineNode node, XmlItem xml)
        {
            PDFOutlineNode prev = null;
            PDFOutlineNode current = null;
            long currNumber = node.number;
            for (int i = 0; i < xml.Count; i++)
            {
                int page = 0;
                float offset = 0f;
                
                string s = xml[i].GetProp("Page");
                if (s != "")
                {
                    page = int.Parse(s);
                    s = xml[i].GetProp("Offset");
                    if (s != "")
                        offset = (float)Converter.FromString(typeof(float), s) * PDF_DIVIDER;
                }
                
                // add check of page range
                
                current = new PDFOutlineNode();
                current.text = xml[i].GetProp("Text");
                current.page = page;
                current.offset = offset;
                current.prev = prev;
                if (prev != null)
                    prev.next = current;
                else
                    node.first = current;
                prev = current;
                current.parent = node;
                current.number = currNumber + 1;
                currNumber = BuildOutline(current, xml[i]);                
                node.count++;
                node.countTree += current.countTree + 1;
            }
            node.last = current;
            return currNumber;
        }

        private void WriteOutline(PDFOutlineNode item)
        {
            long number;
            if (item.parent != null)
                number = UpdateXRef();
            else
                number = item.number;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(ObjNumber(number));
            sb.AppendLine("<<");
            if (item.text != String.Empty)
            {
                sb.Append("/Title ");
                PrepareString(item.text, encKey, encrypted, number, sb);
                sb.AppendLine();
            }
            if (item.parent != null)
                sb.Append("/Parent ").AppendLine(ObjNumberRef(item.parent.number));
            if (item.count > 0)
                sb.Append("/Count ").AppendLine(item.count.ToString());
            if (item.first != null)
                sb.Append("/First ").AppendLine(ObjNumberRef(item.first.number));
            if (item.last != null)
                sb.Append("/Last ").AppendLine(ObjNumberRef(item.last.number));
            if (item.prev != null)
                sb.Append("/Prev ").AppendLine(ObjNumberRef(item.prev.number));
            if (item.next != null)
                sb.Append("/Next ").AppendLine(ObjNumberRef(item.next.number));


            if (item.page < pagesRef.Count)
            {
                sb.Append("/Dest [");
                sb.Append(ObjNumberRef(pagesRef[item.page]));
                sb.Append(" /XYZ 0 ");
                sb.Append(Math.Round(pagesHeights[item.page] - item.offset).ToString());
                sb.Append(" 0]");               
            }
            sb.AppendLine(">>");
            sb.AppendLine("endobj");
            WriteLn(pdf, sb.ToString());            

            if (item.first != null)
                WriteOutline(item.first);
            if (item.next != null)
                WriteOutline(item.next);
        }
    }
}
