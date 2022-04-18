using FastReport.Export.Dxf.Groups;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Export.Dxf.Sections.Tables
{
    public class TableBase : GroupsStore
    {
        #region Private Fields

        private List<GroupBase> endTable;
        private string name;
        //private List<GroupBase> groups;

        private List<GroupBase> startTable;

        #endregion Private Fields

        #region Public Properties

        public List<GroupBase> EndTable
        {
            get { return endTable; }
            set { endTable = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        //public List<GroupBase> Groups
        //{
        //    get { return groups; }
        //    set { groups = value; }
        //}
        public List<GroupBase> StartTable
        {
            get { return startTable; }
            set { startTable = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        public TableBase(string name)
        {
            Name = name;
            startTable = new List<GroupBase>();
            endTable = new List<GroupBase>();
            Groups = new List<GroupBase>();
            startTable.Add(GroupUtils.CreateTypeName("TABLE"));

            endTable.Add(GroupUtils.CreateTypeName("ENDTAB"));
        }

        #endregion Public Constructors

        #region Public Methods

        public void AppendTo(StringBuilder s)
        {
            StartTableAppendTo(s);
            s.Append("\n");
            foreach (GroupBase g in Groups)
            {
                g.AppendTo(s);
                s.Append("\n");
            }
            EndTableAppendTo(s);
        }

        public void EndTableAppendTo(StringBuilder s)
        {
            GroupUtils.GroupsAppendTo(endTable, s);
        }

        public void StartTableAppendTo(StringBuilder s)
        {
            GroupUtils.GroupsAppendTo(startTable, s);
        }

        #endregion Public Methods
    }
}