using FastReport.Export.Dxf.Sections.Tables;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Export.Dxf.Sections
{
    public class SectionTables : Section
    {
        #region Private Fields

        private List<TableBase> tables;

        #endregion Private Fields

        #region Public Properties

        public TableLtype Ltype
        {
            get
            {
                foreach (TableBase t in Tables)
                {
                    if (t.Name == "LTYPE" && t.GetType() == typeof(TableLtype))
                        return (TableLtype)t;
                }
                // else
                TableLtype ltype = new TableLtype();
                Tables.Add(ltype);
                return ltype;
            }
            set
            {
                for (int i = 0; i < Tables.Count; i++)
                {
                    TableBase t = Tables[i];
                    if (t.Name == "LTYPE")
                        t = value;
                }
            }
        }

        public List<TableBase> Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        public SectionTables() : base("TABLES")
        {
            Tables = new List<TableBase>();
        }

        #endregion Public Constructors

        #region Public Methods

        public override void AppendTo(StringBuilder s)
        {
            StartSectionAppendTo(s);
            s.Append("\n");
            foreach (TableBase t in Tables)
            {
                t.AppendTo(s);
                s.Append("\n");
            }
            EndSectionAppendTo(s);
        }

        #endregion Public Methods
    }
}