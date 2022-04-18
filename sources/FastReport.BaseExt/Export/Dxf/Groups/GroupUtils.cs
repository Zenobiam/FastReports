using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Export.Dxf.Groups
{
    public static class GroupUtils
    {
        #region Public Methods

        public static void AppendGroup<T>(List<GroupBase> groups, int code, T value)
        {
            groups.Add(CreateGroup(code, value));
        }

        public static GroupBase CreateGroup<T>(int code, T value)
        {
            GroupBase group;
            if (typeof(T) == typeof(double))
                group = new GroupDouble(code, Convert.ToDouble(value));
            else if (typeof(T) == typeof(string))
                group = new GroupString(code, Convert.ToString(value));
            else
                group = new Group<T>(code, value);
            return group;
        }

        public static GroupBase CreateGroupComment(int code, string value)
        {
            return CreateGroup(code, value);
        }

        public static GroupBase CreateGroupDouble(int code, double value)
        {
            return CreateGroup(code, value);
        }

        public static GroupBase CreateGroupString(int code, string value)
        {
            return CreateGroup(code, value);
        }

        /// <summary>
        /// Sets Name group
        /// </summary>
        /// <param name="value">Name (attribute tag, block name, and so on)</param>
        public static GroupBase CreateName(string value)
        {
            return CreateGroupString(2, value);
        }

        /// <summary>
        /// Sets Name group
        /// </summary>
        /// <param name="value">Name (attribute tag, block name, ENDSEC, and so on)</param>
        public static GroupBase CreateTypeName(string value)
        {
            return CreateGroupString(0, value);
        }

        public static void GroupsAppendTo(List<GroupBase> groups, StringBuilder sectionString)
        {
            foreach (GroupBase group in groups)
            {
                group.AppendTo(sectionString);
                if (group != groups[groups.Count - 1])
                    sectionString.Append("\n");
            }
        }

        #endregion Public Methods
    }
}