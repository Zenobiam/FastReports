using FastReport.Export.Dxf.Groups;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Export.Dxf.Sections
{
    public class Section
    {
        #region Private Fields

        private List<GroupBase> endSection;
        private string name;
        private List<GroupBase> startSection;

        #endregion Private Fields

        #region Public Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        public Section(string name)
        {
            Name = name;
            StartSection();
            EndSection();
        }

        #endregion Public Constructors

        #region Public Methods

        public virtual void AppendTo(StringBuilder s)
        {
            StartSectionAppendTo(s);
            s.Append("\n");
            EndSectionAppendTo(s);
        }

        public void Clear()
        {
            name = string.Empty;
        }

        public void EndSectionAppendTo(StringBuilder s)
        {
            GroupUtils.GroupsAppendTo(endSection, s);
        }

        public void StartSectionAppendTo(StringBuilder s)
        {
            GroupUtils.GroupsAppendTo(startSection, s);
        }

        #endregion Public Methods

        #region Private Methods

        private void EndSection()
        {
            endSection = new List<GroupBase>();
            endSection.Add(GroupUtils.CreateTypeName("ENDSEC"));
        }

        private void StartSection()
        {
            startSection = new List<GroupBase>();
            startSection.Add(GroupUtils.CreateTypeName("SECTION"));
            startSection.Add(GroupUtils.CreateName(name));
        }

        #endregion Private Methods
    }
}