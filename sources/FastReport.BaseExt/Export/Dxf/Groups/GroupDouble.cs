using System;

namespace FastReport.Export.Dxf.Groups
{
    /// <summary>
    ///  Double precision 3D point value
    /// </summary>
    public class GroupDouble : Group<double>
    {
        #region Public Properties

        public override int Code
        {
            get { return base.Code; }
            set
            {
                if (value >= 10 || value <= 39)
                    base.Code = value;
                else
                    throw new Exception("Code must be in range 10 - 39, but have got: " + value);
            }
        }

        public override double Value
        {
            get { return base.Value; }
            set
            {
                base.Value = value;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        public GroupDouble(int code, double value) : base(code, value)
        {
        }

        #endregion Public Constructors
    }
}