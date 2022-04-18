using FastReport.Export.Dxf.Groups;
using System.Collections.Generic;

namespace FastReport.Export.Dxf
{
    public class GroupsStore
    {
        #region Private Fields

        private List<GroupBase> groups;

        #endregion Private Fields

        #region Public Properties

        public List<GroupBase> Groups
        {
            get { return groups; }
            set { groups = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        public GroupsStore()
        {
            groups = new List<GroupBase>();
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddColor(byte aciColor)
        {
            Groups.Add(GroupUtils.CreateGroup(62, aciColor));
        }

        public void AddEntityThickness(float value)
        {
            AddGroupDouble(370, (int)(value * 100));
        }

        public void AddFourthPoint(float x, float y)
        {
            AddGroupDouble(13, x);
            AddGroupDouble(23, y);
        }

        public void AddGroup<T>(int x, T y)
        {
            GroupUtils.AppendGroup(groups, x, y);
        }

        public void AddGroup(GroupBase group)
        {
            groups.Add(group);
        }

        public void AddGroupDouble(int x, float y)
        {
            Groups.Add(GroupUtils.CreateGroupDouble(x, y));
        }

        public void AddName(string value)
        {
            Groups.Add(GroupUtils.CreateName(value));
        }

        public void AddPrimary2DPoint(float x, float y)
        {
            AddGroupDouble(10, x);
            AddGroupDouble(20, y);
        }

        public void AddSecondPoint(float x, float y)
        {
            AddGroupDouble(11, x);
            AddGroupDouble(21, y);
        }

        public void AddThirdPoint(float x, float y)
        {
            AddGroupDouble(12, x);
            AddGroupDouble(22, y);
        }

        public void AddTypeName(string value)
        {
            Groups.Add(GroupUtils.CreateTypeName(value));
        }

        #endregion Public Methods
    }
}