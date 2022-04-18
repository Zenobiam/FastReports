using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.FastQueryBuilder
{
    abstract class ParserBase 
    {
        public virtual bool CanParse(SqlParser parser)
        {
            return false;
        }
        public abstract void Parse(SqlParser parser);
    }
}
