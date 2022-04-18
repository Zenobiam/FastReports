using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichTextParser.RTF
{
    class RTF_View_Selection
    {
        RTF_View_Position start;
        RTF_View_Position stop;

        public bool Active { get { return start.Position < stop.Position; } }

        public RTF_View_Selection(FastReport.RichTextParser.RTF_View view)
        {
            start = new RTF_View_Position(view);
            stop = new RTF_View_Position(view);
        }
    }
}
