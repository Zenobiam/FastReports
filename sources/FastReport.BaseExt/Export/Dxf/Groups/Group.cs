using System;
using System.Text;

namespace FastReport.Export.Dxf.Groups
{
    public class Group<T> : GroupBase
    {
        #region Private Fields

        private T value;

        #endregion Private Fields

        #region Public Properties

        public virtual T Value
        {
            get { return value; }
            set { this.value = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        public Group(int code, T value)
        {
            Code = code;
            Value = value;
        }

        #endregion Public Constructors

        #region Public Methods

        public override void AppendTo(StringBuilder s)
        {
            s.Append(Convert.ToString(Code)).Append("\n").Append(Convert.ToString(Value));
        }

        public override bool IsEndSectionGroup()
        {
            if (Code == 0 && (Value.GetType() == typeof(string) && (Value as string) == "ENDSEC"))
                return true;
            else
                return false;
        }

        #endregion Public Methods
    }

    public abstract class GroupBase
    {
        #region Private Fields

        private int code;

        #endregion Private Fields

        #region Public Properties

        public virtual int Code
        {
            get { return code; }
            set { code = value; }
        }

        #endregion Public Properties

        #region Public Methods

        public abstract void AppendTo(StringBuilder s);

        public abstract bool IsEndSectionGroup();

        #endregion Public Methods
    }
}