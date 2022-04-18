using FastReport.Export.Dxf.Groups;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Export.Dxf.Sections
{
    public class HeaderItem : GroupsStore
    {
        // 9
        // $ACADVER
        // 1
        // AC1015
        // private List<GroupBase> groups;

        #region Public Constructors

        public HeaderItem(string name, List<GroupBase> values)
        {
            AddGroup(9, name);
            foreach (GroupBase val in values)
            {
                AddGroup(val);
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public void AppendTo(StringBuilder s)
        {
            foreach (GroupBase group in Groups)
            {
                group.AppendTo(s);
                s.Append("\n");
            }
        }

        #endregion Public Methods
    }

    public class SectionHeader : Section
    {
        #region Private Fields

        private List<HeaderItem> headerItems;

        #endregion Private Fields

        #region Public Constructors

        public SectionHeader() : base("HEADER")
        {
            headerItems = new List<HeaderItem>();
            AddVersion();
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddVersion()
        {
            List<GroupBase> acadverValues = new List<GroupBase>();
            GroupUtils.AppendGroup(acadverValues, 1, "AC1015");
            headerItems.Add(new HeaderItem("$ACADVER", acadverValues));
        }

        public override void AppendTo(StringBuilder s)
        {
            StartSectionAppendTo(s);
            s.Append("\n");
            foreach (HeaderItem item in headerItems)
            {
                item.AppendTo(s);
            }
            EndSectionAppendTo(s);
        }

        #endregion Public Methods
    }
}