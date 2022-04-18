using System;

namespace FastReport.Export.Dxf.Groups
{
    public class GroupString : Group<string>
    {
        #region Public Properties

        public override int Code
        {
            get { return base.Code; }
            set
            {
                if (value >= 0 || value <= 9)
                    base.Code = value;
                else
                    throw new Exception("Code must be in range 0 - 9, but have got: " + value);
            }
        }

        public override string Value
        {
            get { return base.Value; }
            set
            {
                if (value.Length > 255)
                    throw new Exception("Value's length must be less then 255!, but have got: " + value.Length);
                else if (value.EndsWith("\r\n") || value.EndsWith("\n"))
                    throw new Exception("Value must not include the newline at the end of the line!");
                else
                    base.Value = value;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Code range: 0-9
        /// String (with the introduction of extended symbol names in AutoCAD 2000, the 255-character
        /// limit has been increased to 2049 single-byte characters not including the newline at the end
        /// of the line)
        /// </summary>
        public GroupString(int code, string value) : base(code, value)
        {
        }

        #endregion Public Constructors
    }
}